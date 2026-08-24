using System.Collections.Generic;
using Sirenix.Serialization;

[System.Serializable]
public class ItemRequirement
{
    public ItemProperties Properties = new ItemProperties();

    [OdinSerialize]
    public Dictionary<ItemCapabilities, int> MinimumCapabilityValues = new Dictionary<ItemCapabilities, int>();
    public int Quantity = 1;
    public ItemGrades MinimumGrade;

    public bool Matches(ItemProperties itemProperties)
    {
        // Exact item type, if specified.
        if (Properties.Type != Items.None && itemProperties.Type != Properties.Type)
        {
            return false;
        }

        // Required conditions.
        if (Properties.Conditions != ItemConditions.None
         && (itemProperties.Conditions & Properties.Conditions) != Properties.Conditions)
        {
            return false;
        }

        // Required capabilities.
        if (Properties.Capabilities != ItemCapabilities.None
         && (itemProperties.Capabilities & Properties.Capabilities) != Properties.Capabilities)
        {
            return false;
        }

        // Required material, if specified.
        if (Properties.Material != ItemMaterials.None && itemProperties.Material != Properties.Material)
        {
            return false;
        }

        // Minimum grade.
        if (MinimumGrade != ItemGrades.None && itemProperties.Grade < MinimumGrade)
        {
            return false;
        }

        // Minimum values for specific capabilities.
        if (MinimumCapabilityValues != null)
        {
            foreach (var requirement in MinimumCapabilityValues)
            {
                if (!itemProperties.CapabilityValues.TryGetValue(
                        requirement.Key,
                        out var value))
                {
                    return false;
                }

                if (value < requirement.Value)
                    return false;
            }
        }

        return true;
    }

    public bool Matches(Item item)
    {
        return item != null && Matches(item.Properties);
    }

    public ItemRequirement CopyWithNewQuantity(int newQuantity)
    {
        return new ItemRequirement
        {
            Properties = Properties,
            MinimumCapabilityValues = MinimumCapabilityValues,
            Quantity = newQuantity,
            MinimumGrade = MinimumGrade,
        };
    }
}