using UnityEngine;

// TODO: use Container or World based on Find results
public class GatherState : StateBase
{
    public GatherState(Entity entity) : base(entity) {}
    public static GatherState FromTask(Task task, Entity entity)
    {
        var gather = new GatherState(entity)
        {
            Task = task
        };

        return gather;
    }

    public override StateResult Update(float deltaTime)
    {
        var constraints = GatherConstraints.Get(Entity, Task);
        if (constraints != Constraints.None) return new StateResult
        {
            Status = StateStatus.Blocked,
            HasConstraints = true,
            Constraints = constraints,
        };

        RemainingTime -= deltaTime;

        if (RemainingTime > 0) return StateResult.Running;

        var container = ContainerRegistry.Components.Get(Entity.Id);
        Debug.Log($"Space available: {container.GetAvailableSpace()}");
        var targetContainer = ContainerRegistry.Components.Get(Task.Target.Id);

        var moved = container.TakeFrom(Task.Item.Properties.Type, Task.Item.Quantity, targetContainer);
        Debug.Log($"Took {moved}");

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}