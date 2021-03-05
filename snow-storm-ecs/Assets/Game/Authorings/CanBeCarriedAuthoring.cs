using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.Carrying;
using static Game.Mechanics.Spaces;

[RequireComponent(typeof(InSpaceAuthoring))]
[DisallowMultipleComponent]
public class CanBeCarriedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Min(1)]
    public int Weight;
    [Min(0)]
    public float PickupRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var position = dstManager.GetComponentData<in_space>(entity).position;
        dstManager.AddComponentData(entity, new can_be_carried(Weight, PickupRange));
        dstManager.AddComponentData(entity, new moves(position));
    }
}