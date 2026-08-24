using System.Collections.Generic;
using Sirenix.Serialization;

[System.Serializable]
public class ItemProperties
{
    public Items Type;
    public ItemConditions Conditions;
    public ItemCapabilities Capabilities;

    [OdinSerialize]
    public Dictionary<ItemCapabilities, float> CapabilityValues = new Dictionary<ItemCapabilities, float>();
    [OdinSerialize]
    public Dictionary<ItemCapabilities, List<ItemRequirement>> UsageRequirements = new Dictionary<ItemCapabilities, List<ItemRequirement>>();
    public ItemMaterials Material;
    public ItemGrades Grade;

    public bool HasConditions(ItemConditions conditions)
    {
        return (Conditions & conditions) == conditions;
    }

    public void AddConditions(ItemConditions conditions)
    {
        Conditions |= conditions;
    }

    public void RemoveConditions(ItemConditions conditions)
    {
        Conditions &= ~conditions;
    }

    public bool HasCapabilities(ItemCapabilities capabilities)
    {
        return (Capabilities & capabilities) == capabilities;
    }

    public void AddCapabilities(ItemCapabilities capabilities)
    {
        Capabilities |= capabilities;
    }

    public void RemoveCapabilities(ItemCapabilities capabilities)
    {
        Capabilities &= ~capabilities;
    }

    public float GetCapabilityValue(ItemCapabilities capability)
    {
        return CapabilityValues.TryGetValue(capability, out var value)
            ? value
            : 0;
    }
}