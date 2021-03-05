using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.Carrying;

[RequireComponent(typeof(ConvertToEntity))]
[DisallowMultipleComponent]
public class CanCarryAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Min(1)]
    public int MaxWeight;
    [Min(100)]
    public uint PickupCooldown;
    [Min(0)]
    public float PickupReach;
    [Min(100)]
    public uint DropOffCooldown;
    [Min(0)]
    public float DropOffReach;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new can_carry(MaxWeight, PickupCooldown, PickupReach, DropOffCooldown, DropOffReach));
    }
}