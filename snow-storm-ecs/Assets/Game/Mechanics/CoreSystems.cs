using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreJobs;
using static Unity.Entities.EntityCommandBuffer;

namespace Game.Mechanics
{
    public static class CoreSystems
    {
        [UpdateInGroup(typeof (FixedStepSimulationSystemGroup))]
        public class CommunicationsBarrier : EntityCommandBufferSystem
        {
        }
    
        /// <summary>
        /// SystemBase with extra functionality.
        /// Allows to request EntityCommandBuffer in simpler manner
        /// </summary>
        public abstract class run_general_system : SystemBase
        {
            protected virtual Action generate() {return default;}
            protected virtual void run() {}
            protected virtual void create() {}
            protected virtual void destroy() {}

            protected sealed override void OnCreate()
            {
                _systems = new Dictionary<Type, ComponentSystemBase>();
                run_lambda = generate();
                if (run_lambda == default)
                {
                    create();
                }
            }

            protected sealed override void OnUpdate()
            {
                run_lambda?.Invoke();
                run();
                
                foreach (var system in _systems.Values.OfType<EntityCommandBufferSystem>()) 
                    system.AddJobHandleForProducer(dependency);
            }

            protected sealed override void OnDestroy() => destroy();


            protected void schedule_parallel<TJob>(EntityQuery q, TJob job, int batches_per_chunk = 1) 
                where TJob : struct, IJobEntityBatch => 
                dependency = job.ScheduleParallel(q, batches_per_chunk, dependency);

            protected JobHandle dependency
            {
                get => Dependency;
                set => Dependency = value;
            }

            protected TSystem depend_on<TSystem>()
                where TSystem : run_general_system
            {
                var system = get_system<TSystem>();
                depend_on(system);
                return system;
            }

            protected void depend_on(run_general_system s)
            {
                dependency = dependency.merged_with(s.dependency);
            }

            protected T get_system<T>() where T : ComponentSystemBase
            {
                var type = typeof(T);
                if (!_systems.TryGetValue(type, out var system))
                {
                    system = _systems[type] = World.GetOrCreateSystem<T>();
                }

                return (T)system;
            }
            
            protected EntityCommandBuffer get_end_sim_ecb() =>
                get_ecb_for<EndSimulationEntityCommandBufferSystem>(); 

            protected ParallelWriter get_job_ecb_for<T>() 
                where T : EntityCommandBufferSystem =>
                get_system<T>().CreateCommandBuffer().AsParallelWriter();

            protected EntityCommandBuffer get_ecb_for<T>() 
                where T : EntityCommandBufferSystem =>
                get_system<T>().CreateCommandBuffer();
            
            protected EntityTypeHandle get_entity_handle() => GetEntityTypeHandle();
            protected ComponentTypeHandle<T> get_handle<T>(bool write = false) where T : struct, IComponentData => GetComponentTypeHandle<T>(!write);
            protected BufferTypeHandle<T> get_buffer_handle<T>(bool write = false) where T : struct, IBufferElementData => GetBufferTypeHandle<T>(!write);
            protected ComponentDataFromEntity<T> get_type<T>(bool write = false) where T : struct, IComponentData => GetComponentDataFromEntity<T>(!write);
            protected BufferFromEntity<T> get_buffer_data<T>(bool write = false) where T : struct, IBufferElementData => GetBufferFromEntity<T>(!write);
            
            // the following methods allow type inference
            protected void set(out EntityTypeHandle h) => h = GetEntityTypeHandle();
            protected void set<T>(out ComponentTypeHandle<T> h, bool write = false) where T : struct, IComponentData => h = GetComponentTypeHandle<T>(!write);
            protected void set<T>(out BufferTypeHandle<T> h, bool write = false) where T : struct, IBufferElementData => h = GetBufferTypeHandle<T>(!write);
            protected void set<T>(out ComponentDataFromEntity<T> d, bool write = false) where T : struct, IComponentData => d = GetComponentDataFromEntity<T>(!write);
            protected void set<T>(out BufferFromEntity<T> d, bool write = false) where T : struct, IBufferElementData => d = GetBufferFromEntity<T>(!write);
            
            protected void set(out EntityRead h) => h = new EntityRead (GetEntityTypeHandle());

            protected void set<T1>(out Optional<T1> jri)
                where T1 : struct, IComponentData 
                => jri = new Optional<T1>
                (
                    get_handle<T1>()
                );
            
            protected void set<T1>(out Read<T1> jri)
                where T1 : struct, IComponentData 
            => jri = new Read<T1>
            (
                get_handle<T1>()
            );
            
            protected void set<T1, T2>(out Read<T1, T2> jri)
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
            => jri = new Read<T1, T2>
            (
                get_handle<T1>(),
                get_handle<T2>()
            );
            
            protected void set<T1, T2, T3>(out Read<T1, T2, T3> jri)
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
                where T3 : struct, IComponentData 
            => jri = new Read<T1, T2, T3>
            (
                get_handle<T1>(),
                get_handle<T2>(),
                get_handle<T3>()
            );
            
            protected GlobalRead<T1> get_global<T1>()
                where T1 : struct, IComponentData 
            => new GlobalRead<T1>
            (
                get_type<T1>()
            );
            
            protected GlobalRead<T1, T2> get_global<T1, T2>()
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
            => new GlobalRead<T1, T2>
            (
                get_type<T1>(),
                get_type<T2>()
            );
            
            protected GlobalRead<T1, T2, T3> get_global<T1, T2, T3>()
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
                where T3 : struct, IComponentData 
            => new GlobalRead<T1, T2, T3>
            (
                get_type<T1>(),
                get_type<T2>(),
                get_type<T3>()
            );
            
            protected GlobalRead<T1, T2, T3, T4> get_global<T1, T2, T3, T4>()
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
                where T3 : struct, IComponentData 
                where T4 : struct, IComponentData 
            => new GlobalRead<T1, T2, T3, T4>
            (
                get_type<T1>(),
                get_type<T2>(),
                get_type<T3>(),
                get_type<T4>()
            );

            protected void set<T1>(out GlobalRead<T1> jri)
                where T1 : struct, IComponentData
            => jri = get_global<T1>();

            protected void set<T1, T2>(out GlobalRead<T1, T2> jri)
                where T1 : struct, IComponentData
                where T2 : struct, IComponentData
            => jri = get_global<T1, T2>();
            
            protected void set<T1, T2, T3>(out GlobalRead<T1, T2, T3> jri)
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData
                where T3 : struct, IComponentData 
            => jri = get_global<T1, T2, T3>();
            
            protected void set<T1>(out Write<T1> jri)
                where T1 : struct, IComponentData 
                => jri = new Write<T1>
                (
                    get_handle<T1>(true)
                );
            
            protected void set<T1, T2>(out Write<T1, T2> jri)
                where T1 : struct, IComponentData 
                where T2 : struct, IComponentData 
                => jri = new Write<T1, T2>
                (
                    get_handle<T1>(true),
                    get_handle<T2>(true)
                );
            
            protected void set<T1>(out BufferWrite<T1> jri)
                where T1 : struct, IBufferElementData 
                => jri = new BufferWrite<T1>
                (
                    get_buffer_handle<T1>(true)
                );
            
            protected void set<T1, T2>(out BufferWrite<T1, T2> jri)
                where T1 : struct, IBufferElementData 
                where T2 : struct, IBufferElementData 
                => jri = new BufferWrite<T1, T2>
                (
                    get_buffer_handle<T1>(true),
                    get_buffer_handle<T2>(true)
                );

            Dictionary<Type, ComponentSystemBase> _systems;
            Action run_lambda;
        }
        
        /// <summary>
        /// System executed in FixedStepSimulation.
        /// </summary>
        [UpdateInGroup(typeof (FixedStepSimulationSystemGroup))]
        public abstract class run_gameplay_system : run_general_system
        {
            protected void destroy_all(EntityQuery q) => q.destroy_all(get_end_ecb());
            protected EntityCommandBuffer get_end_ecb() => get_ecb_for<EndFixedStepSimulationEntityCommandBufferSystem>();
            protected EndFrameEcb get_job_end_ecb() => new EndFrameEcb(get_job_ecb_for<EndFixedStepSimulationEntityCommandBufferSystem>());
            
            protected void set(out EndFrameEcb w) => w = get_job_end_ecb();

            protected virtual void run_gameplay() {}

            protected sealed override void run()
            {
                run_gameplay();
                if (spacial_map_requested)
                {
                    get_system<Spaces.dispose_spatial_map>().add_dependency(Dependency);
                    spacial_map_requested = false;
                }
            }

            protected NativeMultiHashMap<int2, Entity> get_spatial_map()
            {
                spacial_map_requested = true;
                return depend_on<Spaces.update_spatial_map>().spatial_map;
            }

            bool spacial_map_requested;
        }
        
        /// <summary>
        /// A gameplay system that can communicate with entities outside its query, using an EntityCommandBuffer from CommunicationsBarrier 
        /// </summary>
        [UpdateBefore(typeof(CommunicationsBarrier))]
        public abstract class run_pre_comm_gameplay_system: run_gameplay_system
        {
            protected EntityCommandBuffer get_comm_ecb() => get_ecb_for<CommunicationsBarrier>();
            protected CommEcb get_job_comm_ecb() => new CommEcb(get_job_ecb_for<CommunicationsBarrier>());
            protected GameplayEcbs get_job_gameplay_ecbs() => new GameplayEcbs(get_job_end_ecb(), get_job_comm_ecb());

            protected void set(out GameplayEcbs w) => w = get_job_gameplay_ecbs();
            protected void set(out CommEcb w) => w = get_job_comm_ecb();
        }
        
        [UpdateAfter(typeof(CommunicationsBarrier))]
        public abstract class run_post_comm_gameplay_system: run_gameplay_system
        {
        }

        [Serializable]
        public readonly struct spawn_prefab
        {
            public readonly Entity prefab;
            public readonly float2 offset;

            public spawn_prefab(Entity prefab, float2 offset)
            {
                this.prefab = prefab;
                this.offset = offset;
            }
        }
    }
}