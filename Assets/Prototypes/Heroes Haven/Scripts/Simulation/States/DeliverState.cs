using UnityEngine;

public class DeliverState : StateBase
{
    public DeliverState(Entity entity) : base(entity) {}
    public static DeliverState FromTask(Task task, Entity entity)
    {
        var deliver = new DeliverState(entity)
        {
            Task = task
        };

        return deliver;
    }

    public override StateResult Update(float deltaTime)
    {
        var constraints = DeliverConstraints.Get(Entity, Task);
        if (constraints != Constraints.None) return new StateResult
        {
            Status = StateStatus.Blocked,
            HasConstraints = true,
            Constraints = constraints
        };

        // Delivering to a location
        var targetContainer = ContainerRegistry.Components.Get(Task.Target.Id);
        if (targetContainer == null)
        {
            Debug.Log("No container, dropping the items here");

            var position = PositionRegistry.Components.Get(Entity.Id);
            for (var i = 0; i < Task.Item.Quantity; i++)
            {
                var go = new GameObject
                {
                    name = Task.Item.Properties.Type.ToString()
                };
                go.transform.position = new Vector3(position.X, position.Y, position.Z);
                var itemTag = go.AddComponent<ItemTag>();
                itemTag.WorldItem.Properties = Task.Item.Properties;
            }
        }
        else
        {
            Debug.Log($"Space available: {targetContainer.GetAvailableSpace()}");

            var moved = targetContainer.TakeFrom(Task.Item.Properties.Type, Task.Item.Quantity, targetContainer);
            Debug.Log($"Gave {moved}");
        }

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}