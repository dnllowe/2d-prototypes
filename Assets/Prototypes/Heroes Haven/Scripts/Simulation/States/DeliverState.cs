using UnityEngine;

public class DeliverState : StateBase
{
    public DeliverState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        var constraints = DeliverConstraints.Get(world, EntityId, Task);
        if (constraints != Constraints.None) return new StateResult
        {
            Status = StateStatus.Blocked,
            HasConstraints = true,
            Constraints = constraints
        };

        // Delivering to a location
        var targetContainer = world.ContainerRegistry.Get(Task.Target);
        if (targetContainer == null)
        {
            Debug.Log("No container, dropping the items here");

            // TODO -- spawn WorldItem
            var position = world.PositionRegistry.Get(EntityId);
            // for (var i = 0; i < Task.Item.Quantity; i++)
            // {
            //     var go = new GameObject
            //     {
            //         name = Task.Item.ItemTypeRequirement.ToString()
            //     };
            //     go.transform.position = new Vector3(position.X, position.Y, position.Z);
            //     var itemTag = go.AddComponent<ItemTag>();
            //     itemTag.WorldItem.Properties = WTask.Item.I;
            // }
        }
        else
        {
            Debug.Log($"Space available: {targetContainer.GetAvailableSpace()}");

            var moved = targetContainer.TakeFrom(Task.Item.ItemTypeRequirement, Task.Item.Quantity, targetContainer);
            Debug.Log($"Gave {moved}");
        }

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}