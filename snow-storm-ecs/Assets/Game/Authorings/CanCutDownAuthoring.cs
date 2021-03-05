using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.CuttingDown;

[RequireComponent(typeof(CanExecuteTasksAuthoring))]
[DisallowMultipleComponent]
public class CanCutDownAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Min(1)]
    public uint Strength;
    [Min(0)]
    public float Range;
    [Min(100)]
    public uint Cooldown;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new can_cut_down(Strength, Range, Cooldown));
    }
}