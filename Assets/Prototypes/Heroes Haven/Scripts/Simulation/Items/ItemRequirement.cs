using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class ItemRequirement
{
    /// <summary>
    /// Capabilities required, with their minimum effective values.
    /// </summary>
    [OnValueChanged(nameof(RebuildCachedRequiredCapabilities), IncludeChildren = true)]
    public Dictionary<ItemCapabilities, int> MinimumRequiredCapabilityValues = new();

    /// <summary>
    /// Fast lookup mask derived from MinimumRequiredCapabilityValues.
    /// </summary>
    [ReadOnly]
    public ItemCapabilities CachedRequiredCapabilities;

    public Items ItemTypeRequirement;

    /// <summary>
    /// All of these conditions must be present.
    /// </summary>
    public ItemConditions RequiredConditions;

    /// <summary>
    /// Any of these conditions prevent a match.
    /// </summary>
    public ItemConditions ForbiddenConditions;

    public ItemMaterials MaterialRequirement;
    public ItemGrades MinimumGrade;

    public int Quantity = 1;
    public bool ConsumedAfterUse;

    public void RebuildCachedRequiredCapabilities()
    {
        CachedRequiredCapabilities = ItemCapabilities.None;

        foreach (var capability in MinimumRequiredCapabilityValues.Keys)
        {
            CachedRequiredCapabilities |= capability;
        }
    }

    public bool Matches(ItemProperties itemProperties)
    {
        // Must possess every required capability.
        if ((itemProperties.CachedCapabilities & CachedRequiredCapabilities)
            != CachedRequiredCapabilities)
        {
            return false;
        }

        // Exact item type, if specified.
        if (ItemTypeRequirement != Items.None
            && itemProperties.Type != ItemTypeRequirement)
        {
            return false;
        }

        // All required conditions must be present.
        if ((itemProperties.Conditions & RequiredConditions)
            != RequiredConditions)
        {
            return false;
        }

        // Any forbidden condition invalidates the item.
        if ((itemProperties.Conditions & ForbiddenConditions) != 0)
        {
            return false;
        }

        // Required capability effectiveness.
        foreach (var (capability, minimumValue) in MinimumRequiredCapabilityValues)
        {
            if (itemProperties.GetCapabilityValue(capability) < minimumValue)
            {
                return false;
            }
        }

        // Exact material, if specified.
        if (MaterialRequirement != ItemMaterials.None
            && itemProperties.Material != MaterialRequirement)
        {
            return false;
        }

        // Minimum grade.
        if (MinimumGrade != ItemGrades.None
            && itemProperties.Grade < MinimumGrade)
        {
            return false;
        }

        return true;
    }

    public bool Matches(Item item)
    {
        return item != null && Matches(item.Properties);
    }

    public ItemRequirement Copy()
    {
        var copy = new ItemRequirement
        {
            ItemTypeRequirement = ItemTypeRequirement,
            RequiredConditions = RequiredConditions,
            ForbiddenConditions = ForbiddenConditions,
            MaterialRequirement = MaterialRequirement,
            MinimumGrade = MinimumGrade,
            Quantity = Quantity,
            ConsumedAfterUse = ConsumedAfterUse,

            MinimumRequiredCapabilityValues =
                MinimumRequiredCapabilityValues != null
                    ? new Dictionary<ItemCapabilities, int>(
                        MinimumRequiredCapabilityValues)
                    : new Dictionary<ItemCapabilities, int>()
        };

        copy.RebuildCachedRequiredCapabilities();

        return copy;
    }

    public ItemRequirement CopyWithNewQuantity(int newQuantity)
    {
        var copy = Copy();
        copy.Quantity = newQuantity;

        return copy;
    }
}