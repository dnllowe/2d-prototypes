using UnityEngine;

public class DeliverState : StateBase
{
    public Entity Target;
    public Vector3 Destination;
    public Items Item;
    public int Quantity;

    public DeliverState(Entity entity) : base(entity) {}
    public static DeliverState FromTask(Task task, Entity entity)
    {
        var deliver = new DeliverState(entity);
        deliver.Target = task.Target;
        deliver.Destination = task.Destination;
        deliver.Item = task.Item;
        deliver.Quantity = task.Value;
        deliver.Task = task;    

        return deliver;
    }

    public override StateResult Update(float deltaTime)
    {
        if (Vector3.Distance(Target.transform.position, Entity.transform.position) > 1)
        {
            Debug.Log("Not close enough");
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Task = new Task
                {
                    Action = Actions.GoTo,
                    Item = Task.Item,
                    Destination = Task.Destination,
                    Target = Task.Target,
                    Value = Task.Value,
                },
            };
        }

        Debug.Log("In range");

        // Delivering to a location
        if (Target == null || !Target.TryGetComponent<Container>(out var targetContainer))
        {
            Debug.Log("No container, dropping the items here");

            for (var i = 0; i < Quantity; i++)
            {
                var go = new GameObject
                {
                    name = Item.ToString()
                };
                go.transform.position = Entity.transform.position;
                var itemTag = go.AddComponent<ItemTag>();
                itemTag.Tag = Item;
            }
        }
        else
        {
            var container = Entity.GetComponents<Container>()[0];
            Debug.Log($"Space available: {container.GetAvailableSpace()}");

            var moved = targetContainer.TakeFrom(Item, Quantity, container);
            Debug.Log($"Gave {moved}");
        }

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}