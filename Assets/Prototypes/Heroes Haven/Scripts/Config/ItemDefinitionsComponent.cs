using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinitionsComponent", menuName = "Scriptable Objects/ItemDefinitionsComponent")]
public class ItemDefinitionsComponent : SerializedScriptableObject
{
    public Dictionary<Items, ItemDefinitionComponent> ItemDefinitions = new Dictionary<Items, ItemDefinitionComponent>(); 
    public Dictionary<Items, ItemDefinition> ToSimulation()
    {
        var simulationDefintions = new Dictionary<Items, ItemDefinition>();
        foreach (var (item, definition) in ItemDefinitions)
        {
            simulationDefintions.Add(item, definition.ItemDefinition);
        }

        return simulationDefintions;
    }
}
