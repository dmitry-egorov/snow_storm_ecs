using System;
using JetBrains.Annotations;
using Unity.Entities;
using static Game.Mechanics.Actions;
using static Game.Mechanics.Carrying;
using static Game.Mechanics.CoreComponentType;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreIndex;
using static Game.Mechanics.CoreReadability;
using static Game.Mechanics.CoreSystems;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Tasks;

[assembly: RegisterGenericJobType(typeof(assign_task<picks_up_target>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<picks_up_target>.fail_job))]
namespace Game.Mechanics
{
    // Merge with storing?
    public static class Carrying
    {
        [Serializable]
        public struct can_carry : IComponentData
        {
            public int max_weight;
            public uint pickup_cooldown;
            public float pickup_reach;
            public uint drop_off_cooldown;
            public float drop_off_reach;

            public can_carry(int max_weight, uint pickup_cooldown, float pickup_reach, uint drop_off_cooldown, float drop_off_reach)
            {
                this.pickup_cooldown = pickup_cooldown;
                this.max_weight = max_weight;
                this.pickup_reach = pickup_reach;
                this.drop_off_cooldown = drop_off_cooldown;
                this.drop_off_reach = drop_off_reach;
            }
        }
        
        [Serializable]
        public readonly struct can_be_carried : IComponentData
        {
            public readonly int weight;
            public readonly float pickup_range;
            public can_be_carried(int weight, float pickup_range)
            {
                this.weight = weight;
                this.pickup_range = pickup_range;
            }
        }
        
        [Serializable]
        public readonly struct carries: IComponentData
        {
            public readonly Entity target;
            public carries(Entity target) => this.target = target;
        }
        
        [Serializable]
        public readonly struct carried_by: IComponentData
        {
            public readonly Entity carrier;
            public carried_by(Entity target) => this.carrier = target;
        }
        
        [Serializable]
        public readonly struct picks_up_target: ITask
        {
            public readonly Entity target_ref;
            public picks_up_target(Entity target) => this.target_ref = target;
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class assign_picks_up_target : assign_task<picks_up_target> { }
        [UsedImplicitly] public class execute_picks_up_target: execute_task<picks_up_target>
        {
            EntityQuery fail_query;

            protected override void create()
            {
                fail_query = GetEntityQuery(
                      get_task_query_components().try_compose(
                      exclude<in_space>() // TODO: is it exclude any or all??
                    , exclude<can_carry>()
                ));
            }

            protected override void run_gameplay()
            {
                var global = get_global<in_space, can_be_carried, carried_by>();
                var context = get_task_context();

                Entities.WithNone<acting_on_cooldown>()
                .WithReadOnly(global)
                .ForEach((int entityInQueryIndex
                    , Entity self_ref
                    , in in_space self_in_space
                    , in can_carry self_can_carry
                    , in picks_up_target self_task
                ) =>
                {
                    var (in_space, can_be_carried, carried_by) = global;
                    var sort_key = entityInQueryIndex;
                    var self = shape(self_ref, self_in_space, self_can_carry);

                    if (all 
                        && self_task.target_ref.try_compose(in_space, can_be_carried, not(carried_by), out var target)
                        && is_not_too_heavy(self, target)
                        && is_within_pickup_range(self, target)
                    )
                    {
                        pick_up(sort_key, self, target, context.ecbs);
                        context.finish_task(sort_key, self);
                    }
                    else
                    {
                        context.fail_task(sort_key, self);
                    }
                }).ScheduleParallel();

                fail_all_tasks_of(fail_query);
            }
        }

        static void pick_up
        (
            index sort_key,
            in s<Entity, can_carry> self,
            in s<Entity, can_be_carried> target, 
            in EndFrameEcb end_ecb
        )
        {
            end_ecb.add(sort_key, target, new carried_by(self));
            end_ecb.add(sort_key, self, new carries(target));
            end_ecb.put_acting_on_cooldown(sort_key, self, self.c2<can_carry>().pickup_cooldown);
        }
        
        public static bool is_not_too_heavy(
            in can_carry self, 
            in can_be_carried target
        ) => target.weight <= self.max_weight;
        
        static bool is_within_pickup_range
        (
            in s<in_space, can_carry> self,
            in s<in_space, can_be_carried> target
        )
        {
            var range = self.c2<can_carry>().pickup_reach + target.c2<can_be_carried>().pickup_range;
            return is_within_range(self, target, range);
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class sync_position_when_carried : run_gameplay_system 
        {
            protected override void run_gameplay()
            {
                var in_space_data = get_type<in_space>();

                Entities
                .WithReadOnly(in_space_data)
                .WithNativeDisableContainerSafetyRestriction(in_space_data)
                .ForEach((
                    ref in_space self_in_space, 
                    in carried_by self_carried_by
                ) => {
                    // copy carrier position if it has one
                    var carrier = self_carried_by.carrier;
                    if (carrier.has(in_space_data, out var carrier_in_space))
                    {
                        self_in_space = carrier_in_space;
                    }
                }).ScheduleParallel();
            }
        }
    }
}