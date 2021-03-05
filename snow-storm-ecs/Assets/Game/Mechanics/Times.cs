using System;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Mechanics
{
    public static class Times
    {
        public const float simulation_timestep = 1f/60f;// / 20f;

        [Serializable]
        public readonly struct simulation_elapsed_time: IComponentData
        {
            public readonly double value;
            public simulation_elapsed_time(double value) => this.value = value;
        }
    
        /// <summary>
        /// Presentation frame progress to the next simulation frame (0 to 1)
        /// Used to interpolate entity position
        /// </summary>
        [Serializable]
        public readonly struct presentation_frame_progress: IComponentData
        {
            public readonly float value;
            public presentation_frame_progress(float value) => this.value = value;
        }
    
        //------------------------------------------------------------
        [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
        public struct create_singleton_entity : ISystemBase
        {
            public void OnCreate(ref SystemState state)
            {
                var singleton = state.EntityManager.CreateEntity(
                    typeof(simulation_elapsed_time),
                    typeof(presentation_frame_progress)
                );
                
                state.EntityManager.SetName(singleton, "Time Features Singleton");
            }

            public void OnDestroy(ref SystemState state) { }
            public void OnUpdate(ref SystemState state) { }
        }

        //------------------------------------------------------------
        [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
        public struct set_fixed_time_step : ISystemBase
        {
            public void OnCreate(ref SystemState state) => 
                state.World.GetExistingSystem<FixedStepSimulationSystemGroup>().Timestep = simulation_timestep;

            public void OnDestroy(ref SystemState state) { }
            public void OnUpdate(ref SystemState state) { }
        }
    
        //------------------------------------------------------------
        [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
        public struct update_elapsed_simulation_time: ISystemBase
        {
            public void OnCreate(ref SystemState state) { }
            public void OnDestroy(ref SystemState state) { }

            public void OnUpdate(ref SystemState state) => 
                state.SetSingleton(new simulation_elapsed_time(state.World.Time.ElapsedTime));
        }

        //------------------------------------------------------------
        [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
        [UpdateBefore(typeof(TransformSystemGroup))]
        public struct update_presentation_frame_progress : ISystemBase
        {
            public void OnCreate(ref SystemState state) { }
            public void OnDestroy(ref SystemState state) { }

            public void OnUpdate(ref SystemState state) 
            {
                var presentation_time = state.World.Time.ElapsedTime;
                var simulation_time = state.GetSingleton<simulation_elapsed_time>().value;

                var ratio = ((float)(presentation_time - simulation_time)) / simulation_timestep;
                state.SetSingleton(new presentation_frame_progress(ratio));
            }
        }
    }
}