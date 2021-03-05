using System;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using static Game.Mechanics.Actions;
using static Game.Mechanics.CoreComponentType;
using static Game.Mechanics.CoreJobs;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Movement;
using static Game.Mechanics.Tasks;

[assembly: RegisterGenericJobType(typeof(assign_task<moves_to_position>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<moves_to_position>.fail_job))]
[assembly: RegisterGenericJobType(typeof(assign_task<approaches_target>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<approaches_target>.fail_job))]

namespace Game.Mechanics
{
    public static class Movement
    {
        [Serializable]
        public readonly struct can_move : IComponentData
        {
            [UsedImplicitly] public readonly float distance_per_frame;
            public can_move(float distance_per_frame) => this.distance_per_frame = distance_per_frame;
        }
    
        [Serializable]
        public readonly struct moves_to_position : ITask
        {
            [UsedImplicitly] public readonly float2 target_position;
            public moves_to_position(float2 target_position) => this.target_position = target_position;
        }
    
        [Serializable]
        public readonly struct approaches_target : ITask
        {
            public readonly Entity target;
            public readonly float range;
            public approaches_target(Entity target, float range)
            {
                this.target = target;
                this.range = range;
            }
        }
    
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_approaches_target: assign_task<approaches_target> { }
        [UsedImplicitly] public class execute_approaches_target: execute_task<approaches_target>
        {
            protected override Action generate()
            {
                var task_components = get_task_query_components();
                
                var execute_query = GetEntityQuery(
                    task_components.try_compose(
                    read<in_space>()
                ));

                var fail_query = GetEntityQuery(
                    task_components.try_compose(
                    exclude<in_space>()
                ));
                
                return () =>
                {
                    var job = new job();
                    set(out job.context);
                    set(out job.entity_read);
                    set(out job.read);
                    set(out job.global_read);
                    schedule_parallel(execute_query, job);
                
                    fail_all_tasks_of(fail_query);
                };
            }
            
            [BurstCompile] struct job : IJobEntityBatch
            {
                public TaskContext context;
                public EntityRead entity_read;
                public Read<in_space, approaches_target> read;
                public GlobalRead<in_space> global_read;
                
                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var sort_key = batch_i;
                    var (position_arr, approaches_target_arr, self_arr) = chunk.get_data_of(read, entity_read).reinterpret<float2>();
                    var in_space_type = global_read.type;
                    
                    for (var i = 0; i < self_arr.Length; i++)
                    {
                        var target = approaches_target_arr[i].target;
                        if (target.doesnt_have(in_space_type, out var target_in_space))
                        {
                            context.fail_task(sort_key, self_arr[i]);
                            continue;
                        }

                        var own_position = position_arr[i];
                        var target_position = target_in_space.position;

                        var offset = target_position - own_position;
                        var distance = math.length(offset);

                        var range = approaches_target_arr[i].range;

                        // TODO: move away when too close
                        // the target is within range, task is finished
                        if (distance < range + range_check_epsilon)
                        {
                            context.finish_task(sort_key, self_arr[i]);
                            continue;
                        }

                        // the target is outside the range
                        // move towards the target
                        var move_position = target_position - offset * (range / distance);
                        context.push_tasks(sort_key, self_arr[i], new moves_to_position(move_position));
                    }
                }
            }
        }
    
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_moves_to_position: assign_task<moves_to_position> { }
        [UsedImplicitly] public class execute_moves_to_position: execute_task<moves_to_position>
        {
            protected override Action generate()
            {
                var task_components = get_task_query_components();
                
                var execute_query = GetEntityQuery(
                    task_components.try_compose(
                    read_write<in_space>()
                    , read<can_move>()
                    , exclude<acting_on_cooldown>()
                ));

                var fail_query = GetEntityQuery(
                    task_components.try_compose(
                    exclude<can_move>()
                    , exclude<in_space>()
                ));
            
                return () =>
                {
                    var job = new job();
                    set(out job.context);
                    set(out job.entity_read);
                    set(out job.write);
                    set(out job.read);
                    schedule_parallel(execute_query, job);
                    
                    fail_all_tasks_of(fail_query);
                };
            }
            
            [BurstCompile] struct job : IJobEntityBatch
            {
                public EntityRead entity_read;
                public TaskContext context;
                public Write<in_space> write;
                public Read<can_move, moves_to_position> read;
                
                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var sort_key = batch_i;
                    var position_arr = chunk.get_data_of(write).Reinterpret<float2>();
                    var (step_arr, target_position_arr, self_arr) = chunk.get_data_of(read, entity_read).reinterpret<float, float2>();
                    
                    for (var i = 0; i < position_arr.Length; i++)
                    {
                        ref var position = ref position_arr.nocheck_ref(i);
                        var target = target_position_arr[i];
                        // step length on a single frame.
                        // not multiplying by time since the timestep is fixed and the speed is expressed per frame.
                        var step = step_arr[i];
                        var offset = target - position;
                        var distance = math.length(offset);

                        // TODO: math.select optimization? (do when there's a performance test)
                        // doesn't reach the target on this update
                        if (distance > step)
                        {
                            // step towards the target
                            position += offset * (step / distance);
                        }
                        // reached the target
                        else
                        {
                            // place at the target position and finish the task
                            position = target;
                            context.finish_task(sort_key, self_arr[i]);
                        }
                    }
                }
            }
        }
    }
}

