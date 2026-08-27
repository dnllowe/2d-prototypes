using System.Collections.Generic;

[System.Serializable]
public class World
{
    public GameConfig Config;
    public ComponentRegistry<Health> HealthRegistry = new ComponentRegistry<Health>();
    public ComponentRegistry<Container> ContainerRegistry = new ComponentRegistry<Container>();
    public ComponentRegistry<Position> PositionRegistry = new ComponentRegistry<Position>();
    public ComponentRegistry<Resource> ResourceRegistry = new ComponentRegistry<Resource>();
    public ComponentRegistry<WorldItem> WorldItemRegistry = new ComponentRegistry<WorldItem>();
    public List<SystemBase> Systems = new List<SystemBase>();
    public IdProivder IdProvider = new IdProvider();

    public void Tick(float deltaTime)
    {
        foreach (var system in Systems) system.Tick(deltaTime);
    }

    public uint SpawnEntity(EntityDefinition definition)
    {
        var entity = new Entity();
        entity.Id = IdProvider.GetNextId();
    }
}