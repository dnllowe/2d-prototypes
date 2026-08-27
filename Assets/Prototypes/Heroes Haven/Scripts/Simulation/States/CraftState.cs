using System;
using UnityEngine;

public class CraftState : StateBase
{
    public CraftState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        var foundRecipe = world.Config.RecipeDefinitions.TryGetValue(Task.Item.ItemTypeRequirement, out var recipeDefinition);
        if (!foundRecipe)
        {
            throw new Exception($"No recipe found for {Task.Item.ItemTypeRequirement}");
        }

        var container = world.ContainerRegistry.Get(EntityId);
        if (container == null)
        {
            UnityEngine.Debug.Log("No container to look for craft items");
        }
        var requirements = container.GetRemainingRequirements(recipeDefinition.Recipe.Requirements);
        if (requirements.Count > 0) Debug.Log("We're going to need a few things...");

        if (!container.HasRequirements(recipeDefinition.Recipe.Requirements)) return new StateResult
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

        // TODO: request the item from the world
        var item = new Item
        {
            Properties = world.Config.ItemDefinitions[Task.Item.ItemTypeRequirement].Properties.Copy(),
        };
        var kept = container.PlaceInside(item);
        if (!kept)
        {
            // TODO: drop on ground as world item
        }

        return new StateResult
        {
            Status = StateStatus.Complete,
            Target = item.EntityId,
            HasTarget = true,
        };
    }
}
