using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.Gathering;
using static Game.Mechanics.Tasks;

[RequireComponent(typeof(CanCutDownAuthoring))]
[DisallowMultipleComponent]
public class GathersResourcesAuthoring: MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject Storage;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var storage = conversionSystem.GetPrimaryEntity(Storage);

        dstManager.AddComponentData(entity, new gathers_resources(storage));
        dstManager.SetComponentData(entity, new can_execute_tasks(-1));
        
        conversionSystem.DeclareDependency(gameObject, Storage);
    }
}