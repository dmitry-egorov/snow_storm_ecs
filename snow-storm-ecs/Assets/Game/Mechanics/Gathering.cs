using System;
using JetBrains.Annotations;
using Unity.Entities;
using static Game.Mechanics.Carrying;
using static Game.Mechanics.CoreComponentType;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreReadability;
using static Game.Mechanics.CuttingDown;
using static Game.Mechanics.Gathering;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Movement;
using static Game.Mechanics.Storing;
using static Game.Mechanics.Tasks;

[assembly: RegisterGenericJobType(typeof(assign_task<gathers_resources>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<gathers_resources>.fail_job))]
[assembly: RegisterGenericJobType(typeof(assign_task<finds_and_gathers_a_resource>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<finds_and_gathers_a_resource>.fail_job))]
[assembly: RegisterGenericJobType(typeof(assign_task<gathers_target>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<gathers_target>.fail_job))]
[assembly: RegisterGenericJobType(typeof(assign_task<finds_and_picks_up_a_resource>.job))]
[assembly: RegisterGenericJobType(typeof(execute_task<finds_and_picks_up_a_resource>.fail_job))]
[assembly: RegisterGenericJobType(typeof(execute_task<finds_and_picks_up_a_resource>.finish_job))]

namespace Game.Mechanics
{
    public static class Gathering
    {
        [Serializable]
        public struct gathers_resources: ITask
        {
            public Entity storage;
            public gathers_resources(Entity storage) => this.storage = storage;
        }
        
        [Serializable]
        public struct finds_and_gathers_a_resource: ITask
        {
            public Entity storage;
            public finds_and_gathers_a_resource(Entity storage) => this.storage = storage;
        }
        
        [Serializable]
        public struct gathers_target: ITask
        {
            public Entity target; 
            public Entity storage;

            public gathers_target(Entity target, Entity storage)
            {
                this.target = target;
                this.storage = storage;
            }

            public void Deconstruct(out Entity target, out Entity storage)
            {
                target = this.target;
                storage = this.storage;
            }
        }

        [Serializable]
        public struct finds_and_picks_up_a_resource: ITask
        {
            public int type; // Temporary field
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_gathers_resources: assign_task<gathers_resources> { }
        [UsedImplicitly] public class execute_gathers_resources : execute_task<gathers_resources>
        {
            EntityQuery fail_query;

            protected override void create() =>
                fail_query = GetEntityQuery(
                      get_task_query_components().try_compose(
                      exclude<in_space>()
                    , exclude<can_cut_down>()
                    , exclude<can_carry>()
                ));

            protected override void run_gameplay()
            {
                var global = get_global<in_space, can_store>();
                var c = get_task_context();

                Entities.WithAll<in_space, can_cut_down, can_carry>()
                .WithReadOnly(global)
                .ForEach((
                    int entityInQueryIndex
                    , Entity self_entity
                    , in gathers_resources self_task
                ) =>
                {
                    var (in_space, can_store) = global;
                    var sort_key = entityInQueryIndex;
                    var storage = self_task.storage;
                    
                    if (storage.has(can_store, in_space))
                    {
                        c.push_tasks(sort_key, self_entity, new finds_and_gathers_a_resource(storage));
                    }
                    else
                    {
                        c.fail_task(sort_key, self_entity);
                    }
                }).ScheduleParallel();
                
                fail_all_tasks_of(fail_query);
            }
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class assign_finds_and_gathers_a_resource: assign_task<finds_and_gathers_a_resource> { }
        [UsedImplicitly] public class execute_finds_and_gathers_a_resource : execute_task<finds_and_gathers_a_resource>
        {
            EntityQuery _fail_query;

            protected override void create() =>
                _fail_query = GetEntityQuery(
                      get_task_query_components().try_compose(
                      exclude<in_space>()
                    , exclude<can_cut_down>()
                    , exclude<can_carry>()
                ));

            protected override void run_gameplay()
            {
                fail_all_tasks_of(_fail_query);

                var global = get_global<in_space, can_be_cut_down>();
                var spatial_map = get_spatial_map();
                var c = get_task_context();

                Entities.WithAll<can_cut_down, can_carry>()
                .WithReadOnly(global).WithReadOnly(spatial_map)
                .ForEach((int entityInQueryIndex
                    , Entity self_entity
                    , in finds_and_gathers_a_resource self_task
                    , in in_space self_in_space
                ) =>
                {
                    var (in_space, can_be_cut_down) = global;
                    var sort_key = entityInQueryIndex;
                    var self = shape(self_entity, self_in_space);

                    // find the nearest entity that can be cut down
                    if (try_find_nearest(out var target))
                    {
                        c.replace_task_with(sort_key, self, new gathers_target(target, self_task.storage));
                    }
                    else
                    {
                        c.fail_task(sort_key, self);
                    }
                
                    bool try_find_nearest(out s<Entity, in_space, can_be_cut_down> result)
                    {
                        var found = false;
                        result = default;
                        var min_distance_sq = float.MaxValue;
                        foreach (var e in spatial_map.get_cell_entities(self))
                        {
                            if (all
                                && e != self
                                && e.try_compose(in_space, can_be_cut_down, out var entity)
                            )
                            {
                                var distance_sq = get_distance_sq(entity, self);
                                if (min_distance_sq > distance_sq)
                                {
                                    min_distance_sq = distance_sq;
                                    result = entity;
                                    found = true;
                                }
                            }
                        }

                        return found;
                    }
                }).ScheduleParallel();
            }
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class assign_gathers_target: assign_task<gathers_target> { }
        [UsedImplicitly] public class execute_gathers_target : execute_task<gathers_target>
        {
            EntityQuery fail_query;

            protected override void create() =>
                fail_query = GetEntityQuery(
                      get_task_query_components().try_compose(
                      exclude<in_space>()
                    , exclude<can_cut_down>()
                    , exclude<can_carry>()
                ));

            protected override void run_gameplay()
            {
                fail_all_tasks_of(fail_query);

                var global = get_global<in_space, can_be_cut_down, can_store>();
                var c = get_task_context();

                Entities.WithAll<in_space, can_carry, can_cut_down>().WithReadOnly(global)
                .ForEach((int entityInQueryIndex
                    , Entity self_entity
                    , in gathers_target self_task
                    , in can_cut_down self_can_cut_down
                ) =>
                {
                    var (in_space, can_be_cut_down, can_store) = global;
                    var sort_key = entityInQueryIndex;
                    var (target, storage) = self_task;

                    if (!target.has(can_be_cut_down, in_space))
                    {
                        c.finish_task(sort_key, self_entity);
                    }
                    else if (storage.has(can_store, in_space, out var storage_can_store))
                    {
                        c.push_tasks(sort_key, self_entity, 
                            new approaches_target(target, self_can_cut_down.range),
                            new cuts_down_target(target),
                            new finds_and_picks_up_a_resource(),
                            new approaches_target(storage, storage_can_store.drop_off_range),
                            new stores_carried_item(storage)
                        );
                    }
                    else
                    {
                        c.fail_task(sort_key, self_entity);
                    }
                }).ScheduleParallel();
            }
        }

        //------------------------------------------------------------
        [UsedImplicitly] public class assign_finds_and_picks_up_a_resource: assign_task<finds_and_picks_up_a_resource> { }
        [UsedImplicitly] public class execute_finds_and_picks_up_a_resource : execute_task<finds_and_picks_up_a_resource>
        {
            EntityQuery finish_query;
            EntityQuery fail_query;

            protected override void create()
            {
                var task_components = get_task_query_components();
                
                finish_query = GetEntityQuery(
                    task_components.try_compose(
                    read<in_space>()
                  , read<can_carry>()
                  , read<carries>()
                ));
                
                fail_query = GetEntityQuery(
                    task_components.try_compose(
                    exclude<in_space>()
                  , exclude<can_carry>()
                ));
            }

            protected override void run_gameplay()
            {
                finish_all_tasks_of(finish_query);
                fail_all_tasks_of(fail_query);
                
                var global = get_global<in_space, can_be_carried, carried_by, stored_at>();
                var spatial_map = get_spatial_map();

                var context = get_task_context();

                Entities.WithNone<carries>().WithReadOnly(global).WithReadOnly(spatial_map)
                .ForEach((int entityInQueryIndex
                    , Entity self_entity
                    , in finds_and_picks_up_a_resource self_task
                    , in in_space self_in_space
                    , in can_carry self_can_carry
                ) =>
                {
                    var (in_space, can_be_carried, carried_by, stored_at) = global;
                    var sort_key = entityInQueryIndex;
                    var self = shape(self_entity, self_in_space, self_can_carry);
                    
                    // find closest entity that can be carried
                    if (try_find_nearest(self, out var target))
                    {
                        var full_range = get_pickup_reach(self, target);
                        context.push_tasks(sort_key, self,
                            new approaches_target(target, full_range),
                            new picks_up_target(target)
                        );
                    }
                    else
                    {
                        context.fail_task(sort_key, self);
                    }
                    
                    float get_pickup_reach(can_carry self, can_be_carried target) => 
                        self.pickup_reach + target.pickup_range;

                    bool try_find_nearest(
                        in s<Entity, in_space, can_carry> self, 
                        out s<Entity, in_space, can_be_carried, n<carried_by>, n<stored_at>> target
                    )
                    {
                        var found = false;
                        target = default;
                        var min_distance_sq = float.MaxValue;
                        foreach (var e in spatial_map.get_cell_entities(self))
                        {
                            if (all
                                && e != self
                                && e.try_compose(in_space, can_be_carried, not(carried_by), not(stored_at), out var entity)
                                && is_not_too_heavy(self, entity)
                            )
                            {
                                var distance_sq = get_distance_sq(self, entity);
                                if (min_distance_sq > distance_sq)
                                {
                                    min_distance_sq = distance_sq;
                                    target = entity;
                                    found = true;
                                }
                            }
                        }

                        return found;
                    }
                }).ScheduleParallel();
            }
        }
    }
}