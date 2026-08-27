using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class ItemProperties
{
    public Items Type;
    public ItemConditions Conditions;
    public ItemCapabilities CachedCapabilities;

    [OnValueChanged(nameof(RebuildCachedCapabilities), IncludeChildren = true)]
    public Dictionary<ItemCapabilities, CapabilityUse> Uses = new Dictionary<ItemCapabilities, CapabilityUse>();
    public ItemMaterials Material;
    public ItemGrades Grade;

    void RebuildCachedCapabilities()
    {
        CachedCapabilities = ItemCapabilities.None;

        foreach (var capability in Uses.Keys)
            CachedCapabilities |= capability;
    }

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
        return (CachedCapabilities & capabilities) == capabilities;
    }

    public float GetCapabilityValue(ItemCapabilities capability)
    {
        return Uses.TryGetValue(capability, out var use)
            ? use.Value
            : 0;
    }

    public ItemProperties Copy()
    {
        var copy = new ItemProperties
        {
            Type = Type,
            Conditions = Conditions,
            Material = Material,
            Grade = Grade,
            Uses = new Dictionary<ItemCapabilities, CapabilityUse>()
        };

        foreach (var (capability, use) in Uses)
        {
            copy.Uses[capability] = use?.Copy();
        }

        copy.RebuildCachedCapabilities();

        return copy;
    }
}