using System;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Game.Mechanics.CoreSystems;
using static Game.Mechanics.Times;

namespace Game.Mechanics
{
    public static class Spaces
    {
        public const float range_check_epsilon = math.EPSILON * 10f;
        const float map_cell_size = 10f;

        [Serializable]
        public readonly struct in_space : IComponentData
        {
            public readonly float2 position;
            public in_space(float2 position) => this.position = position;
        }

        [Serializable]
        public readonly struct moves : IComponentData
        {
            public readonly float2 prev_position;
            public moves(float2 prev_position) => this.prev_position = prev_position;
        }
        
        [Serializable]
        public readonly struct initial_position_is_set: IComponentData {}

        //------------------------------------------------------------
        [UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
        [UsedImplicitly] public class set_initial_position_to_transform : run_general_system
        {
            EntityQuery query;

            protected override void run()
            {
                query.add_to_all<initial_position_is_set>(get_end_sim_ecb());
                
                Entities.WithNone<initial_position_is_set>()
                .WithStoreEntityQueryInField(ref query)
                .ForEach((
                    ref Translation translation,
                    in in_space in_space
                ) =>
                {
                    var tpy = translation.Value.y;
                    var p = in_space.position;
                    translation.Value = new float3(p.x, tpy, p.y);
                }).ScheduleParallel();
            }
        }

        //------------------------------------------------------------
        [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
        [UpdateAfter(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]
        [UsedImplicitly] public class update_previous_position : SystemBase
        {
            protected override void OnUpdate() =>
            Entities.ForEach((
                ref moves moves
                , in in_space in_space
            ) => {
                //Note: maybe check if the position has changed?
                moves = new moves(in_space.position);
            }).ScheduleParallel();
        }
        
        //------------------------------------------------------------
        [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
        [UpdateAfter(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]
        [UsedImplicitly] public class update_spatial_map : run_general_system
        {
            public NativeMultiHashMap<int2, Entity> spatial_map;
            EntityQuery query;

            protected override void run()
            {
                spatial_map = new NativeMultiHashMap<int2, Entity>(query.CalculateEntityCount(), Allocator.TempJob);
                var spacial_map_writer = spatial_map.AsParallelWriter();
                    
                Entities.WithStoreEntityQueryInField(ref query)
                .ForEach((
                    Entity self
                    , in in_space in_space
                ) => 
                    spacial_map_writer.add(in_space.position, self)
                )
                .ScheduleParallel();
            }
        }
        
        //------------------------------------------------------------
        [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
        [UpdateAfter(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
        [UsedImplicitly] public class dispose_spatial_map : run_general_system
        {
            JobHandle external_dependencies;

            public void add_dependency(JobHandle dep) => external_dependencies = external_dependencies.merged_with(dep);
            
            protected override void run()
            {
                var spatial_map = depend_on<update_spatial_map>().spatial_map;
                dependency = Job
                .WithReadOnly(spatial_map)
                .WithDisposeOnCompletion(spatial_map)
                .WithCode(() => { var _ = spatial_map.Capacity; }) //dummy code
                .Schedule(dependency.merged_with(external_dependencies));
                
                external_dependencies = default;
            }
        }

        //------------------------------------------------------------
        [UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
        [UsedImplicitly] public class interpolate_and_convert_position_to_transform : SystemBase
        {
            protected override void OnUpdate()
            {
                var time_ratio = GetSingleton<presentation_frame_progress>().value;
                Entities
                .ForEach((
                    ref Translation translation, 
                    in in_space in_space, 
                    in moves moves
                ) =>
                {
                    var tpy = translation.Value.y;
                    var p = in_space.position;
                    var pp = moves.prev_position;
                    var il = math.lerp(pp, p, time_ratio);
                    translation.Value = new float3(il.x, tpy, il.y);
                })
                .ScheduleParallel();
            }
        }

        public static bool is_out_of_range_of(in in_space self_in_space, in in_space target_in_space, in float range) =>
            !position_is_within_range_of(self_in_space.position, target_in_space.position, range);
        
        public static bool is_within_range(in in_space self, in in_space target, in float range) =>
            position_is_within_range_of(self.position, target.position, range);

        public static float get_distance_sq(in in_space self, in in_space other) =>
            get_distance_sq(self.position, other.position);

        public static float get_distance_sq(in float2 self_in_space, in float2 target_in_space)
        {
            var offset = target_in_space - self_in_space;
            return math.lengthsq(offset);
        }
        
        public static bool position_is_within_range_of(in float2 self_in_space, in float2 target_in_space, in float range)
        {
            var distance_sq = Spaces.get_distance_sq(self_in_space, target_in_space);
            var range_sq = range * range;

            // the target is outside the range
            return distance_sq < range_sq + range_check_epsilon;
        }

        public static NativeMultiHashMap<int2, Entity>.Enumerator get_cell_entities(this NativeMultiHashMap<int2, Entity> map, in_space e)
            => map.get_cell_entities(e.position);
        public static NativeMultiHashMap<int2, Entity>.Enumerator get_cell_entities(this NativeMultiHashMap<int2, Entity> map, float2 position)
        {
            var spacial_index = get_spacial_index_of(position);
            return map.GetValuesForKey(spacial_index);
        }
        
        static void add(this NativeMultiHashMap<int2, Entity>.ParallelWriter map, float2 position, Entity entity)
        {
            var spacial_index = get_spacial_index_of(position);
            map.Add(spacial_index, entity);
        }
        
        static int2 get_spacial_index_of(float2 position) => math.int2(position / map_cell_size);
    }
}