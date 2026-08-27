using System.Collections.Generic;

[System.Serializable]
public class GameConfig
{
    public Dictionary<Items, RecipeDefinition> RecipeDefinitions = new Dictionary<Items, RecipeDefinition>();
    public Dictionary<Items, ItemDefinition> ItemDefinitions = new Dictionary<Items, ItemDefinition>();
}