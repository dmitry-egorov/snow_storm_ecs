using System;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using static Game.Mechanics.Actions;
using static Game.Mechanics.CoreComponentType;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreJobs;
using static Game.Mechanics.CoreReadability;
using static Game.Mechanics.CoreSystems;
using static Game.Mechanics.CuttingDown;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Tasks;

[assembly: RegisterGenericComponentType(typeof(has<a_cut>))]
[assembly: RegisterGenericJobType(typeof(assign_task<cuts_down_target>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<cuts_down_target>.fail_job))]

namespace Game.Mechanics
{
    public static class CuttingDown
    {
        [Serializable]
        public readonly struct can_cut_down: IComponentData
        {
            public readonly uint strength;
            public readonly float range;
            public readonly uint cooldown;

            public can_cut_down(uint strength, float range, uint cooldown)
            {
                this.strength = strength;
                this.range = range;
                this.cooldown = cooldown;
            }
        }

        [Serializable]
        public struct can_be_cut_down: IComponentData
        {
            [UsedImplicitly] public int hp;
            public can_be_cut_down(int hp) => this.hp = hp;
        }

        [Serializable]
        public readonly struct will_transform_when_cut_down : IComponentData
        {
            public readonly Entity stump_prefab;
            public readonly spawn_prefab trunk_prefab_spawn;

            public will_transform_when_cut_down(Entity stump_prefab, spawn_prefab trunk_prefab_spawn)
            {
                this.stump_prefab = stump_prefab;
                this.trunk_prefab_spawn = trunk_prefab_spawn;
            }
        }

        [Serializable]
        public readonly struct a_cut: IBufferElementData
        {
            [UsedImplicitly] public readonly uint strength;
            public a_cut(uint strength) => this.strength = strength;
        }

        [Serializable]
        public readonly struct is_cut_down: IComponentData { }

        [Serializable]
        public readonly struct cuts_down_target: ITask
        {
            [UsedImplicitly] public readonly Entity target;
            public cuts_down_target(Entity target) => this.target = target;
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_cuts_down_target: assign_task<cuts_down_target> { }
        [UsedImplicitly] public class execute_cuts_down_target: execute_task<cuts_down_target>
        {
            EntityQuery fail_query;

            protected override void create()
            {
                fail_query = GetEntityQuery(
                      get_task_query_components().try_compose(
                      exclude<in_space>()
                    , exclude<can_cut_down>()
                ));
            }
            
            protected override void run_gameplay()
            {
                fail_all_tasks_of(fail_query);
                
                var global = get_global<in_space, is_cut_down, can_be_cut_down, acting_on_cooldown>();
                var c = get_task_context();

                Entities.WithAll<can_cut_down>()
                .WithReadOnly(global)
                .ForEach((int entityInQueryIndex
                    , Entity self_ref
                    , in cuts_down_target self_task
                    , in in_space self_in_space
                    , in can_cut_down self_can_cut_down
                ) =>
                {
                    var (in_space, is_cut_down, can_be_cut_down, acting_on_cooldown) = global;
                    var sort_key = entityInQueryIndex;
                    var self = shape(self_ref, self_in_space, self_can_cut_down);
                    var target_ref = self_task.target;

                    if (target_ref.has(is_cut_down))
                    {
                        c.finish_task(sort_key, self);
                        return;
                    }

                    if (self_ref.has(acting_on_cooldown)) //TODO: move to query when is_being_cut_down is implemented
                        return;

                    if (all 
                        && target_ref.try_compose(in_space, can_be_cut_down, out var target)
                        && target_within_cutting_range_of(self, target)
                    )
                    {
                        c.ecbs.cut(sort_key, self, target);
                    }
                    else
                    {
                        c.fail_task(sort_key, self);
                    }
                }).ScheduleParallel();
            }
        }
        
        static bool target_within_cutting_range_of(
            in s<in_space, can_cut_down> self,
            in in_space target
        ) => is_within_range(self, target, self.c2<can_cut_down>().range);

        static void cut(
            this in GameplayEcbs ecbs,
            int sort_key,
            in s<Entity, can_cut_down> self,
            in s<Entity, can_be_cut_down> target
        )
        {
            ecbs.comm.append_and_flag(sort_key, target, new a_cut(self.c2<can_cut_down>().strength));
            ecbs.end.put_acting_on_cooldown(sort_key, self, self.c2<can_cut_down>().cooldown);
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class apply_cuts : run_post_comm_gameplay_system
        {
            protected override Action generate()
            {
                var has_cuts_query = GetEntityQuery(read<has<a_cut>>());
                var full_query = GetEntityQuery(
                    read_write<can_be_cut_down>()
                    , read<has<a_cut>>()
                    , read_write<a_cut>()
                );
                return () =>
                {
                    var end_ecb = get_end_ecb();

                    has_cuts_query.remove_all<has<a_cut>>(end_ecb);

                    var job = new job();
                    set(out job.entity_read);
                    set(out job.write);
                    set(out job.buffer_write);
                    job.end_ecb = end_ecb.to_end_frame_ecb();

                    schedule_parallel(full_query, job);
                };
            }

            [BurstCompile]
            struct job : IJobEntityBatch
            {
                public EntityRead entity_read;
                public Write<can_be_cut_down> write;
                public BufferWrite<a_cut> buffer_write;
                public EndFrameEcb end_ecb;

                public void Execute(ArchetypeChunk chunk, int batch_i)
                {
                    var (hp_arr, self_arr) = chunk.get_data_of(write, entity_read).reinterpret<int>();
                    var cuts_acc = chunk.get_data_of(buffer_write);

                    for (var i = 0; i < self_arr.Length; i++)
                    {
                        var self = self_arr[i];
                        
                        var new_hp = hp_arr[i];
                        
                        var cut_strength_buf = cuts_acc[i].Reinterpret<int>();
                        for (var j = 0; j < cut_strength_buf.Length; j++) 
                            new_hp -= cut_strength_buf[j];

                        // update hp
                        hp_arr[i] = new_hp;
                        
                        // clear the buffer
                        cut_strength_buf.Clear();

                        // set "is cut down" when HP are exhausted
                        if (new_hp <= 0) 
                            end_ecb.add<is_cut_down>(batch_i, self);
                    }
                }
            }
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class transform_when_cut_down : run_gameplay_system
        {
            EntityQuery is_cut_query;

            protected override void create() => is_cut_query = GetEntityQuery(read<is_cut_down>());

            protected override void run_gameplay()
            {
                // destroy cut down entities
                destroy_all(is_cut_query);
                
                var moves = get_type<moves>();
                var end_ecb = get_job_end_ecb();

                Entities.WithAll<is_cut_down>()
                .WithReadOnly(moves)
                .ForEach((int entityInQueryIndex
                    , in in_space self_in_space
                    , in will_transform_when_cut_down self_transform
                ) => {
                    var sort_key = entityInQueryIndex;
                    
                    var stump_prefab = self_transform.stump_prefab;
                    var trunk_prefab_spawn = self_transform.trunk_prefab_spawn;
                    var trunk_prefab = trunk_prefab_spawn.prefab;

                    var stump = end_ecb.instantiate(sort_key, stump_prefab);
                    var trunk = end_ecb.instantiate(sort_key, trunk_prefab);
                        
                    end_ecb.set(sort_key, stump, self_in_space);

                    var trunk_position = self_in_space.position + trunk_prefab_spawn.offset;
                    end_ecb.set(sort_key, trunk, new in_space(trunk_position));
                    if (trunk_prefab.has(moves))
                    {
                        end_ecb.set(sort_key, trunk, new moves(trunk_position));
                    }
                }).ScheduleParallel();
            }
        }
    }
}