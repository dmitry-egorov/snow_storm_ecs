using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.Storing;

[RequireComponent(typeof(InSpaceAuthoring))]
[DisallowMultipleComponent]
public class CanStoreAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Min(0)]
    public float DropOffRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new can_store(DropOffRange));
        dstManager.AddBuffer<stored_item>(entity);
    }
}