using UnityEngine;


public class GatherState : StateBase
{
    public Entity Target;
    public Items Item;
    public int Quantity;
    readonly float TIME_TO_GATHER = 2.0f;

    public GatherState(Entity entity) : base(entity) {}
    public static GatherState FromTask(Task task, Entity entity)
    {
        var gather = new GatherState(entity)
        {
            Target = task.Target,
            Item = task.Item,
            Quantity = task.Value,
            Task = task
        };
        gather.RemainingTime = task.Value * gather.TIME_TO_GATHER;

        return gather;
    }

    public override StateResult Update(float deltaTime)
    {
        if (Target == null)
        {
            Debug.Log($"Now, where can I find one of those {Item}...");
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Task = new Task
                {
                    Action = Actions.Find,
                    Item = Item,
                }
            };
        }

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
        RemainingTime -= deltaTime;

        if (RemainingTime > 0) return StateResult.Running;

        var container = Entity.gameObject.GetComponents<Container>()[0];
        Debug.Log($"Space available: {container.GetAvailableSpace()}");
        var targetContainer = Target.GetComponent<Container>();

        var moved = container.TakeFrom(Item, Quantity, targetContainer);
        Debug.Log($"Took {moved}");

        return new StateResult
        {
            Status = StateStatus.Complete,
        };
    }
}