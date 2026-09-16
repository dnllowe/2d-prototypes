using System.Collections.Generic;

[System.Serializable]
public class World
{
    public GameConfig Config;
    public ComponentRegistry<Container> ContainerRegistry = new ComponentRegistry<Container>();
    public ComponentRegistry<Health> HealthRegistry = new ComponentRegistry<Health>();
    public ComponentRegistry<Resource> ResourceRegistry = new ComponentRegistry<Resource>();
    public ComponentRegistry<Position> PositionRegistry = new ComponentRegistry<Position>();
    public ComponentRegistry<WorldItem> WorldItemRegistry = new ComponentRegistry<WorldItem>();
    public ComponentRegistry<Character> CharacterRegistry = new ComponentRegistry<Character>();
    public HashSet<uint> Entities = new HashSet<uint>();
    public List<StateMachine> StateMachines = new List<StateMachine>();
    public List<WorldEvent> Events = new List<WorldEvent>();
    public IdProvider IdProvider = new IdProvider();

    public void Tick(float deltaTime)
    {
        foreach (var stateMachine in StateMachines) stateMachine.Tick(deltaTime);
    }

    public void Initialize(WorldState state)
    {
        foreach (var container in state.Containers) ContainerRegistry.Components.Add(container.EntityId, container);
        foreach (var health in state.Health) HealthRegistry.Components.Add(health.EntityId, health);
        foreach (var resource in state.Resources) ResourceRegistry.Components.Add(resource.EntityId, resource);
        foreach (var position in state.Positions) PositionRegistry.Components.Add(position.EntityId, position);
        foreach (var worldItem in state.WorldItems) WorldItemRegistry.Components.Add(worldItem.EntityId, worldItem);
        foreach (var character in state.Characters) CharacterRegistry.Components.Add(character.EntityId, character);
        foreach (var entity in state.Entities) Entities.Add(entity);
    }

    public uint SpawnEntity(EntityDefinition definition)
    {
        var id = IdProvider.GetNextId();
        Entities.Add(id);

        if (definition.Container.Assigned)
        {
            ContainerRegistry.Components.Add(id, new Container(id)
            {
                Capacity = definition.Container.Capacity,
            });
        }

        // There are some callbacks here
        if (definition.Health.Assigned)
        {
            HealthRegistry.Components.Add(id, new Health(id)
            {
                Total = definition.Health.MaxHealth,
                Current = definition.Health.MaxHealth,
            });
        }

        if (definition.Position.Assigned)
        {
            PositionRegistry.Components.Add(id, new Position
            {
                X = definition.Position.StartX,
                Y = definition.Position.StartY,
                Z = definition.Position.StartZ, 
            });
        }

        if (definition.Resource.Assigned)
        {
            ResourceRegistry.Components.Add(id, new Resource(id)
            {
                ExtractableItems = definition.Resource.ExtractableItems,
            });
        }

        if (definition.StateMachine.Assigned)
        {
            StateMachines.Add(new StateMachine(this, id));
        }

        if (definition.WorldItem.Assigned)
        {
            WorldItemRegistry.Components.Add(id, new WorldItem(id)
            {
                Properties = Config.ItemDefinitions[definition.WorldItem.Item].Properties,
            });
        }

        Events.Add(new WorldEvent { EntityId = id });

        return id;
    }

    public uint SpawnCharacter(Character character)
    {
        var id = IdProvider.GetNextId();
        Entities.Add(id);

        ContainerRegistry.Components.Add(id, new Container(id)
        {
            Capacity = 4,
        });

        HealthRegistry.Components.Add(id, new Health(id)
        {
            Total = character.MaxHealth,
            Current = character.MaxHealth,
        });

        PositionRegistry.Components.Add(id, new Position
        {
            X = 0,
            Y = 0,
            Z = 0, 
        });
        StateMachines.Add(new StateMachine(this, id));

        CharacterRegistry.Components.Add(id, character);

        return id;
    }
}