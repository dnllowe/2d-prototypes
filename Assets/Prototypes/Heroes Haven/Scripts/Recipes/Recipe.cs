using System.Collections.Generic;

[System.Serializable]
public class Recipe
{
    public RecipeResult Result;
    public Items Produces;
    public ItemConditions ProducedCondition;
    public ItemMaterials Material;
    public int TimeToProduce;
    public List<Requirement> Requirements = new List<Requirement>();
}
