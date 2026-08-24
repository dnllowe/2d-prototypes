using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class ItemDefinitions
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public Dictionary<Items, ItemDefinition> Items = new Dictionary<Items, ItemDefinition>();
}