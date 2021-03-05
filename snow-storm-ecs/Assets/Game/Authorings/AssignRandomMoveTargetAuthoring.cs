using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Game.Mechanics.Spaces;
using static Game.Mechanics.Movement;

[RequireComponent(typeof(ConvertToEntity))]
[RequireComponent(typeof(CanMoveAuthoring))]
[DisallowMultipleComponent]
public class AssignRandomMoveTargetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var position = dstManager.GetComponentData<in_space>(entity).position;
        dstManager.AddComponentData(entity, new moves_to_position(3f * noise.cellular(3f * position)));
    }
}