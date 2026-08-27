using UnityEngine;

// TODO: use Container or World based on Find results
public class GatherState : StateBase
{
    public GatherState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        var constraints = GatherConstraints.Get(world, EntityId, Task);
        if (constraints != Constraints.None) return new StateResult
        {
            Status = StateStatus.Blocked,
            HasConstraints = true,
            Constraints = constraints,
        };

        RemainingTime -= deltaTime;

        if (RemainingTime > 0) return StateResult.Running;

        var container = world.ContainerRegistry.Get(EntityId);
        Debug.Log($"Space available: {container.GetAvailableSpace()}");
        var targetContainer = world.ContainerRegistry.Get(Task.Target);

        var moved = container.TakeFrom(Task.Item.ItemTypeRequirement, Task.Item.Quantity, targetContainer);
        Debug.Log($"Took {moved}");

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}