using UnityEngine;

public class FindState : StateBase
{
    public FindState(Entity entity) : base(entity) {}

    public override StateResult Update(float deltaTime)
    {
        if (!TaggedItems.Items.TryGetValue(Task.Item, out var items)) return StateResult.Failed;

        return new StateResult
        {
            Status = StateStatus.Complete,
            Target = items[0],
            HasTarget = true,
        };
    }

    public static FindState FromTask(Task task, Entity entity)
    {
        var find = new FindState(entity);
        find.Task = task;

        return find;
    }
}
