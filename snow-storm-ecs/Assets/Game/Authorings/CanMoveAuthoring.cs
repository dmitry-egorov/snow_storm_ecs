using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Movement;

[RequireComponent(typeof(CanExecuteTasksAuthoring))]
[RequireComponent(typeof(InSpaceAuthoring))]
[DisallowMultipleComponent]
public class CanMoveAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    /// <summary>
    ///  Travel distance per frame (speed multiplied by frame timestep)
    /// </summary>
    [FormerlySerializedAs("Speed")] public float DistancePerFrame;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var position = dstManager.GetComponentData<in_space>(entity).position;
        dstManager.AddComponentData(entity, new can_move(DistancePerFrame));
        dstManager.AddComponentData(entity, new moves(position));
    }
}
