using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.Tasks;

[RequireComponent(typeof(ConvertToEntity))]
[DisallowMultipleComponent]
public class CanExecuteTasksAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new can_execute_tasks(-1));
        dstManager.AddBuffer<task_ref>(entity);
    }
}