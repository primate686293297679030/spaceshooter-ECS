using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerPrefabComponent : IComponentData
{
    public Entity PlayerPrefab;
}
