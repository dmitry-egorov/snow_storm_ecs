using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Game.Mechanics.Spaces;

[RequireComponent(typeof(ConvertToEntity))]
[DisallowMultipleComponent]
public class InSpaceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var transform_position = transform.position;
        var position = new float2(transform_position.x, transform_position.z);
        dstManager.AddComponentData(entity, new in_space(position));
    }
}
