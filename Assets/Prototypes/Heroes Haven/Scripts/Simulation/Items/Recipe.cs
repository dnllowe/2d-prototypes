using System.Collections.Generic;

[System.Serializable]
public class Recipe
{
    public RecipeResult Result;
    public ItemProperties Produces;
    public int TimeToProduce;
    public List<ItemRequirement> Requirements = new List<ItemRequirement>();
}
