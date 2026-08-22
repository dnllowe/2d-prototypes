using UnityEngine;

public class CraftState : StateBase
{
    public Items Item;
    public Container Container;
    public Recipe Recipe;

    public CraftState(Entity entity) : base(entity) {}
    public static CraftState FromTask(Task task, Entity entity, Recipe recipe)
    {
        var craft = new CraftState(entity);
        craft.Item = task.Item;
        craft.Container = entity.GetComponent<Container>();
        craft.Recipe = recipe;
        craft.Task = task;

        return craft;
    }

    public override StateResult Update(float deltaTime)
    {
        var requirements = Container.GetRequirements(Recipe);
        if (requirements.Count == 0) return StateResult.Running;

        return new StateResult
        {
            Status = StateStatus.NeedsTask,
            Task = new Task
            {
                Action = Actions.Gather,
                Item = requirements[0].Item,
                Value = requirements[0].Quantity,
            }
        };
    }
}
