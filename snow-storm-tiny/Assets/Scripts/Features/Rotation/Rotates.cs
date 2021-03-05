using Unity.Entities;

namespace Tiny3D
{
    [GenerateAuthoringComponent]
    public struct Rotates : IComponentData
    {
        public float speedX;
        public float speedY;
        public float speedZ;
    }
}