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
        craft.RemainingTime = recipe.TimeToProduce;

        return craft;
    }

    public override StateResult Update(float deltaTime)
    {
        var requirements = Container.GetRemainingRequirements(Recipe.Requirements);
        if (requirements.Count > 0) Debug.Log("We're going to need a few things...");

        if (!Container.HasRequirements(Recipe.Requirements)) return new StateResult
        {
            Status = StateStatus.NeedsTask,
            Task = new Task
            {
                Action = Actions.Gather,
                Item = requirements[0].Item,
                Value = requirements[0].Quantity,
            }
        };

        Debug.Log("Ok, I have everything I need");
        RemainingTime -= deltaTime;
        if (RemainingTime > 0) return StateResult.Running;

        var go = new GameObject
        {
            name = Task.Item.ToString(),
        };

        go.transform.position = Entity.transform.position;
        var entity = go.AddComponent<Entity>();
        var itemTag = go.AddComponent<ItemTag>();
        itemTag.Tag = Task.Item;
        Container.PlaceInside(Task.Item, Task.Value);

        return new StateResult
        {
            Status = StateStatus.Complete,
            Target = entity,
            Item = Task.Item,
            HasTarget = true,
            HasItem = true,
        };
    }
}
