using Sirenix.Serialization;

[System.Serializable]
public class EntityDefinition
{
    // Spacial
    public PositionDefinition Position;

    // Behavior
    public StateMachineDefinition StateMachine;

    // Items / Entities
    public WorldItemDefinition WorldItem;
    [OdinSerialize]
    public ResourceDefinition Resource;
    public ContainerDefinition Container;

    // Aspects
    public HealthDefinition Health;
}