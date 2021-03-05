using System;
using JetBrains.Annotations;
using Unity.Entities;
using static Game.Mechanics.CoreEcb;
using static Game.Mechanics.CoreIndex;
using static Game.Mechanics.CoreSystems;

namespace Game.Mechanics
{
    public static class Actions
    {
        public const uint cooldown_ticks_per_sim_step = 100;

        [Serializable]
        public struct acting_on_cooldown: IComponentData
        {
            public float remaining_ticks;
            public acting_on_cooldown(float remaining_ticks) => this.remaining_ticks = remaining_ticks;
        }
        
        //------------------------------------------------------------
        [UsedImplicitly] public class tick_down_and_remove_cooldown : run_gameplay_system
        {
            protected override void run_gameplay()
            {
                var end_ecb = get_job_end_ecb();
                    
                Entities.ForEach((int entityInQueryIndex
                    , Entity self
                    , ref acting_on_cooldown cd
                ) =>
                {
                    var sort_key = entityInQueryIndex;
                    var rt = cd.remaining_ticks -= cooldown_ticks_per_sim_step;
                    if (rt <= 0)
                    {
                        end_ecb.remove<acting_on_cooldown>(sort_key, self);
                    }
                }).ScheduleParallel();
            }
        }

        public static void put_acting_on_cooldown(this in EndFrameEcb end_ecb, index sort_key, in Entity self, uint ticks) =>
            end_ecb.add(sort_key, self, new acting_on_cooldown(ticks));
    }
}