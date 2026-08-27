using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RecipeDefinitions", menuName = "Scriptable Objects/RecipeDefinitions")]
public class RecipeDefinitionsComponent : SerializedScriptableObject
{
    public Dictionary<Items, RecipeDefinitionComponent> RecipeDefinitions = new Dictionary<Items, RecipeDefinitionComponent>();
    public Dictionary<Items, RecipeDefinition> ToSimulation()
    {
        var simulationDefintions = new Dictionary<Items, RecipeDefinition>();
        foreach (var (item, recipe) in RecipeDefinitions)
        {
            simulationDefintions.Add(item, recipe.RecipeDefinition);
        }

        return simulationDefintions;
    }
}
