using UnityEngine;

public class CraftState : StateBase
{
    public Container Container;
    public Recipe Recipe;

    public CraftState(Entity entity) : base(entity) {}
    public static CraftState FromTask(Task task, Entity entity, Recipe recipe)
    {
        var craft = new CraftState(entity)
        {
            Container = ContainerRegistry.Components.Get(entity.Id),
            Recipe = recipe,
            Task = task,
            RemainingTime = recipe.TimeToProduce
        };

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
                Item = requirements[0],
            }
        };

        Debug.Log("Ok, I have everything I need");
        RemainingTime -= deltaTime;
        if (RemainingTime > 0) return StateResult.Running;

        var go = new GameObject
        {
            name = Task.Item.ToString(),
        };

        var position = PositionRegistry.Components.Get(Entity.Id);
        go.transform.position = new Vector3(position.X, position.Y, position.Z);

        var entity = new Entity();go.AddComponent<EntityComponent>();
        var itemTag = go.AddComponent<ItemTag>();
        itemTag.WorldItem.Properties = Task.Item.Properties;
        var item = new Item
        {
            Properties = Task.Item.Properties
        };
        Container.PlaceInside(item);

        return new StateResult
        {
            Status = StateStatus.Complete,
            Target = entity,
            HasTarget = true,
        };
    }
}
