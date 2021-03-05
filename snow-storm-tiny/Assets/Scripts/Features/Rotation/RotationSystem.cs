using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tiny3D
{
    public class RotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = (float)Time.ElapsedTime;
            Entities.ForEach((ref Rotates rotate, ref Rotation rotation) =>
            {
                var qx = quaternion.RotateX(time * rotate.speedX);
                var qy = quaternion.RotateY(time * rotate.speedY);
                var qz = quaternion.RotateZ(time * rotate.speedZ);
                rotation.Value = math.normalize(math.mul(qz, math.mul(qy, qx)));
            }).ScheduleParallel();
        }
    }
}
