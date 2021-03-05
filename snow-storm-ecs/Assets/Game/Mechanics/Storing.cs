using System;
using JetBrains.Annotations;
using Unity.Entities;
using static Game.Mechanics.Actions;
using static Game.Mechanics.Carrying;
using static Game.Mechanics.CoreComponentType;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreReadability;
using static Game.Mechanics.CoreSystems;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Storing;
using static Game.Mechanics.Tasks;

[assembly: RegisterGenericComponentType(typeof(has<stored_item>))]
[assembly: RegisterGenericJobType(typeof(assign_task<stores_carried_item>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<stores_carried_item>.fail_job))]
namespace Game.Mechanics
{
    public static class Storing
    {
        [Serializable]
        public readonly struct can_store : IComponentData
        {
            public readonly float drop_off_range;
            public can_store(float dropOffRange) => drop_off_range = dropOffRange;
        }
        
        [Serializable]
        public readonly struct stored_at: IComponentData
        {
            public readonly Entity storage;
            public stored_at(Entity storage) => this.storage = storage;
        }
        
        [Serializable]
        public readonly struct stored_item : IBufferElementData
        {
            public readonly Entity item;
            public stored_item(Entity item) => this.item = item;
        }

        [Serializable]
        public readonly struct stores_carried_item: ITask
        {
            public readonly Entity storage;
            public stores_carried_item(Entity storage) => this.storage = storage;
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_picks_up_target : assign_task<stores_carried_item> { }
        [UsedImplicitly] public class execute_stores_carried_item: execute_task<stores_carried_item>
        {
            EntityQuery _fail_query;

            protected override void create()
            {
                var task_components = get_task_query_components();
                
                _fail_query = GetEntityQuery(
                      task_components.try_compose(
                      exclude<in_space>() // TODO: is it exclude any or all??
                    , exclude<can_carry>()
                    , exclude<carries>()
                ));
            }

            protected override void run_gameplay()
            {
                fail_all_tasks_of(_fail_query);
                
                var global = get_global<in_space, can_store, carried_by>();
                var c = get_task_context();
                
                Entities.WithNone<acting_on_cooldown>().WithReadOnly(global)
                .ForEach((int entityInQueryIndex
                    , Entity self_pointer
                    , in in_space self_in_space
                    , in can_carry self_can_carry
                    , in carries self_carries
                    , in stores_carried_item self_task
                ) =>
                {
                    var (in_space, can_store, carried_by) = global;
                    var sort_key = entityInQueryIndex;
                    var self = shape(self_pointer, self_in_space, self_can_carry);
                    
                    if (all 
                    && self_carries.target.try_compose(carried_by, out var target)// TODO: must_have
                    && self_task.storage.try_compose(in_space, can_store, out var storage)
                    && is_within_drop_off_range(self, storage)
                    )
                    {
                        //TODO: use events to write data to target and storage
                        c.ecbs.end.remove<carries>(sort_key, self);
                        c.ecbs.end.remove<carried_by>(sort_key, target);
                        c.ecbs.end.add(sort_key, target, new stored_at(storage));
                        c.ecbs.end.append(sort_key, storage, new stored_item(target));
                        c.ecbs.end.put_acting_on_cooldown(sort_key, self, self_can_carry.drop_off_cooldown);

                        c.finish_task(sort_key, self);
                    }
                    else
                    {
                        c.fail_task(sort_key, self);
                    }
                    
                }).ScheduleParallel();
            }
        }
        
        static bool is_within_drop_off_range
        (
            in s<in_space, can_carry> self,
            in s<in_space, can_store> storage
        )
        {
            var range = self.c2<can_carry>().drop_off_reach + storage.c2<can_store>().drop_off_range;
            return is_within_range(self, storage, range);
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class sync_position_when_stored : run_gameplay_system 
        {
            protected override void run_gameplay()
            {
                var in_space_data = get_type<in_space>();
                Entities
                .WithReadOnly(in_space_data)
                // assuming the carrier is not carried (?), so we don't read and write to the same place
                .WithNativeDisableContainerSafetyRestriction(in_space_data)
                .ForEach((
                     ref in_space self_in_space
                    , in stored_at self_stored_at
                ) => {
                    // copy storage position if it has one
                    var storage = self_stored_at.storage;
                    if (storage.has(in_space_data, out var storage_in_space))
                    {
                        self_in_space = storage_in_space;
                    }
                }).ScheduleParallel();
            }
        }
    }
}