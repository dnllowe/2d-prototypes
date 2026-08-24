using System.Collections.Generic;

[System.Serializable]
public class RecipeDefinitions
{
    public Dictionary<Items, RecipeDefinition> Recipes = new Dictionary<Items, RecipeDefinition>();
}