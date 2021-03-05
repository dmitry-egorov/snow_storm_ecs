using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Game.Mechanics.CoreSystems;
using static Game.Mechanics.CuttingDown;

[RequireComponent(typeof(ConvertToEntity))]
[DisallowMultipleComponent]
public class CanBeCutDownAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [Min(1)]
    public int HP;

    public GameObject StumpPrefab;
    public GameObject TrunkPrefab;
    public float2 TrunkOffset;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(StumpPrefab);
        referencedPrefabs.Add(TrunkPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var stump_prefab = conversionSystem.GetPrimaryEntity(StumpPrefab);
        var trunk_prefab = conversionSystem.GetPrimaryEntity(TrunkPrefab);

        dstManager.AddComponentData(entity, new can_be_cut_down(HP));
        dstManager.AddBuffer<a_cut>(entity);
        
        var trunk_prefab_spawn = new spawn_prefab(trunk_prefab, TrunkOffset);
        dstManager.AddComponentData(entity, new will_transform_when_cut_down(stump_prefab, trunk_prefab_spawn));
    }
}