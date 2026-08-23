[System.Serializable]
public class Requirement
{
    public Items Item; 
    public ItemConditions Conditions;
    public ItemMaterials Material;
    public ItemGrades Grade;
    public int Quantity;
    public bool ConsumedAfterUse;
}
