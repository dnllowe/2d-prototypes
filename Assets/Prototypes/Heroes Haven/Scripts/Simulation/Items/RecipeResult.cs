[System.Serializable]
public class RecipeResult
{
    public Items Item;
    public ItemConditions Conditions;
    public ItemCapabilities Capabilities;
    public ItemMaterials Material;
    public ItemGrades Grade;
    public int Quantity = 1;
}