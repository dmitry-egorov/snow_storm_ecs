using System;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreJobs;

namespace Game.Mechanics
{
    public static class Tasks
    {
        public interface ITask: IComponentData
        {
        }
        
        [Serializable]
        public readonly struct task_ref: IBufferElementData
        {
            public readonly Entity task;
            public task_ref(Entity task) => this.task = task;
        }

        [Serializable]
        public readonly struct assigns_to : IComponentData
        {
            [UsedImplicitly] public readonly Entity target;
            public assigns_to(Entity target) => this.target = target;
        }
        
        [Serializable]
        public readonly struct can_execute_tasks: IComponentData
        {
            public readonly int fallback_task_index;
            public can_execute_tasks(int fallback_task_index) => this.fallback_task_index = fallback_task_index;
        }

        [Serializable]
        public readonly struct stores_a_task: IComponentData
        {
            // Used in copy_tasks_job with reinterpret 
            [UsedImplicitly] public readonly int fallback_task_index;
            public stores_a_task(int fallback_task_index) => this.fallback_task_index = fallback_task_index;
        }
        
        /*
         * to declare a new task, add its component, assigning system and assembly reference for the job (at the start of a file)
         * e.g:
         * [assembly: RegisterGenericJobType(typeof(assign_task<gathers_target>.job ))]
         *
         * [Serializable]
         * public readonly struct gathers_target: ITask
         * {
         *     public readonly Entity target; 
         * }
         * 
         * public class assign_gathers_target: assign_task<gathers_target> { }
         */
        
        public abstract class assign_task<TTaskData> : CoreSystems.run_post_comm_gameplay_system
            where TTaskData : struct, ITask
        {
            protected override Action generate()
            {
                var entity_query = GetEntityQuery(
                    CoreComponentType.read<TTaskData>(),
                    CoreComponentType.read<assigns_to>(),
                    CoreComponentType.read<stores_a_task>()
                );

                return () =>
                {
                    // destroy assigned tasks
                    destroy_all(entity_query);

                    var job = new job();
                    set(out job.end_ecb);
                    set(out job.read);
                    schedule_parallel(entity_query, job);
                };
            }

            [BurstCompile]
            public struct job : IJobEntityBatch
            {
                public Read<assigns_to, stores_a_task, TTaskData> read;
                public EndFrameEcb end_ecb;

                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var sort_key = batch_i;
                    var (targets_arr, can_execute_tasks_arr, task_data_arr) = chunk
                        .get_data_of(read)
                        .reinterpret<Entity, can_execute_tasks>();

                    // copy tasks to the target
                    for (var i = 0; i < task_data_arr.Length; i++)
                    {
                        var target = targets_arr[i];
                        end_ecb.add(sort_key, target, task_data_arr[i]);
                        end_ecb.set(sort_key, target, can_execute_tasks_arr[i]);
                    }
                }
            }
        }

        public abstract class execute_task<TTaskData> : CoreSystems.run_pre_comm_gameplay_system
            where TTaskData : struct, ITask
        {
            protected void set(out TaskContext tc) => tc = get_task_context(); 

            protected ComponentType[] get_task_query_components() => new[] {
                CoreComponentType.read<can_execute_tasks>(),
                CoreComponentType.read_write<task_ref>(),
                CoreComponentType.read<TTaskData>()
            };

            protected void fail_all_tasks_of(EntityQuery query)
            {
                var fail_job = new fail_job();
                set(out fail_job.task_context);
                set(out fail_job.entity_read);
                schedule_parallel(query, fail_job);
            }

            protected void finish_all_tasks_of(EntityQuery query)
            {
                var finish_job = new finish_job();
                set(out finish_job.task_context);
                set(out finish_job.entity_read);
                schedule_parallel(query, finish_job);
            }

            protected TaskContext get_task_context()
            {
                var c = new TaskContext();
                set(out c.tasks_stack, true);
                set(out c.can_execute_tasks);
                set(out c.task);
                set(out c.ecbs);
                return c;
            }

            [BurstCompile]
            public struct fail_job : IJobEntityBatch
            {
                [ReadOnly] public EntityRead entity_read;
                public TaskContext task_context;
                
                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var sort_key = batch_i;
                    var self_entity_arr = chunk.get_data_of(entity_read);

                    for (var i = 0; i < self_entity_arr.Length; i++)
                    {
                        task_context.fail_task(sort_key, self_entity_arr[i]);
                    }
                }
            }

            [BurstCompile]
            public struct finish_job : IJobEntityBatch
            {
                [ReadOnly] public EntityRead entity_read;
                public TaskContext task_context;
                
                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var sort_key = batch_i;
                    var self_entity_arr = chunk.get_data_of(entity_read);

                    for (var i = 0; i < self_entity_arr.Length; i++)
                    {
                        task_context.finish_task(sort_key, self_entity_arr[i]);
                    }
                }
            }
            
            public struct TaskContext 
            {
                [NativeDisableContainerSafetyRestriction] public BufferFromEntity<task_ref> tasks_stack;
                [ReadOnly] public ComponentDataFromEntity<can_execute_tasks> can_execute_tasks;
                [ReadOnly] public ComponentDataFromEntity<TTaskData> task;
                public GameplayEcbs ecbs;

                public static implicit operator GameplayEcbs(TaskContext tc) => tc.ecbs;

                public void finish_task(int sort_key, in Entity self_ref)
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    // remove the task from the self entity
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    if (self_tasks_stack.try_pop(out var element))
                    {
                        // load the next task from the stack and request its data
                        ecbs.comm.add(sort_key, element.task, new assigns_to(self_ref));
                    }
                }

                public void fail_task(int sort_key, in Entity self_ref)
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var fallback_i = can_execute_tasks[self_ref].fallback_task_index;
            
                    if (fallback_i < 0)
                    {
                        finish_task(sort_key, self_ref);
                        return;
                    }
            
                    // remove the task from self entity
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    // load the fallback task from the stack and request its data
                    ecbs.comm.add(sort_key, self_tasks_stack[fallback_i].task, new assigns_to(self_ref));
                    // discard all the intermediate tasks from the stack and entities
                    var tasks_count = self_tasks_stack.Length;
                    for (var j = fallback_i + 1; j < tasks_count; j++)
                    {
                        ecbs.end.destroy(sort_key, self_tasks_stack[j].task);
                    }
            
                    self_tasks_stack.RemoveRangeSwapBack(fallback_i, tasks_count - fallback_i);
                }

                public readonly void replace_task_with<TTaskData1>(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                ) 
                    where TTaskData1 : struct, ITask
                {
                    // remove the current task from self (stack is unchanged)
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    // add the new task to self (fallback is preserved)
                    ecbs.end.add(sort_key, self_ref, task1_data);
                }

                public readonly void push_tasks<TTaskData1>(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                )
                    where TTaskData1 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // add the new task to self and assign the fallback index
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }

                public void push_tasks<TTaskData1, TTaskData2>(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                    , in TTaskData2 task2_data
                ) 
                    where TTaskData1 : struct, ITask
                    where TTaskData2 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    // pushes the tasks to the stack in reverse order, so that they are executed in order
            
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // push intermediate tasks to the stack
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task2_data, new_fallback_task_i);
                    // add the first task to self
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }

                public void push_tasks<TTaskData1, TTaskData2, TTaskData3>(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                    , in TTaskData2 task2_data
                    , in TTaskData3 task3_data
                ) 
                    where TTaskData1 : struct, ITask
                    where TTaskData2 : struct, ITask
                    where TTaskData3 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    // pushes the tasks to the stack in reverse order, so that they are executed in order
            
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // push intermediate tasks to the stack
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task3_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task2_data, new_fallback_task_i);
                    // add the first task to self
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }
                
                public void push_tasks<
                      TTaskData1
                    , TTaskData2
                    , TTaskData3
                    , TTaskData4
                >(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                    , in TTaskData2 task2_data
                    , in TTaskData3 task3_data
                    , in TTaskData4 task4_data
                ) 
                    where TTaskData1 : struct, ITask
                    where TTaskData2 : struct, ITask
                    where TTaskData3 : struct, ITask
                    where TTaskData4 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    // pushes the tasks to the stack in reverse order, so that they are executed in order
            
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // push intermediate tasks to the stack
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task4_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task3_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task2_data, new_fallback_task_i);
                    // add the first task to self
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }
                
                public void push_tasks<
                      TTaskData1
                    , TTaskData2
                    , TTaskData3
                    , TTaskData4
                    , TTaskData5
                >(
                    int sort_key, in Entity self_ref
                    , in TTaskData1 task1_data
                    , in TTaskData2 task2_data
                    , in TTaskData3 task3_data
                    , in TTaskData4 task4_data
                    , in TTaskData5 task5_data
                ) 
                    where TTaskData1 : struct, ITask
                    where TTaskData2 : struct, ITask
                    where TTaskData3 : struct, ITask
                    where TTaskData4 : struct, ITask
                    where TTaskData5 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    // pushes the tasks to the stack in reverse order, so that they are executed in order
            
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // push intermediate tasks to the stack
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task5_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task4_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task3_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task2_data, new_fallback_task_i);
                    // add the first task to self
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }
                
                public void push_tasks<
                      TTaskData1
                    , TTaskData2
                    , TTaskData3
                    , TTaskData4
                    , TTaskData5
                    , TTaskData6
                >(
                    in Entity self_ref, int sort_key
                    , in TTaskData1 task1_data
                    , in TTaskData2 task2_data
                    , in TTaskData3 task3_data
                    , in TTaskData4 task4_data
                    , in TTaskData5 task5_data
                    , in TTaskData6 task6_data
                ) 
                    where TTaskData1 : struct, ITask
                    where TTaskData2 : struct, ITask
                    where TTaskData3 : struct, ITask
                    where TTaskData4 : struct, ITask
                    where TTaskData5 : struct, ITask
                    where TTaskData6 : struct, ITask
                {
                    var self_tasks_stack = tasks_stack[self_ref];
                    var self_can_execute_tasks = can_execute_tasks[self_ref];
                    var self_task_data = task[self_ref];
                    // pushes the tasks to the stack in reverse order, so that they are executed in order
            
                    // remove the current task from self and push it to the stack
                    ecbs.end.remove<TTaskData>(sort_key, self_ref);
                    var new_fallback_task_i = push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, self_task_data, self_can_execute_tasks.fallback_task_index);
                    // push intermediate tasks to the stack
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task6_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task5_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task4_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task3_data, new_fallback_task_i);
                    push_tasks(sort_key, self_ref, self_tasks_stack, ecbs.end, task2_data, new_fallback_task_i);
                    // add the first task to self
                    ecbs.end.add(sort_key, self_ref, task1_data);
                    ecbs.end.set(sort_key, self_ref, new can_execute_tasks(new_fallback_task_i));
                }

                static int push_tasks<TTaskData1>(
                    int sort_key, in Entity self
                    , in DynamicBuffer<task_ref> self_tasks_stack
                    , in EndFrameEcb end_ecb
                    , in TTaskData1 task_data
                    , in int fallback_task_index
                )
                    where TTaskData1 : struct, ITask
                {
                    // create temp entity for the task and add the data to it
                    var task = end_ecb.create(sort_key);
                    end_ecb.add(sort_key, task, task_data, new stores_a_task(fallback_task_index));
                    // push the reference onto the stack
                    end_ecb.append(sort_key, self, new task_ref(task));
                    return self_tasks_stack.Length;
                }
            }
        }
    }
}