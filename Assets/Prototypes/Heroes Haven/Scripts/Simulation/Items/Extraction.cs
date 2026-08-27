using System.Collections.Generic;

[System.Serializable]
public class Extraction
{
    public Items Produces;
    public int Count = 1;
    public float TimeToExtract = 2;
    public List<ItemRequirement> ItemsNeededForExtraction = new List<ItemRequirement>();
}