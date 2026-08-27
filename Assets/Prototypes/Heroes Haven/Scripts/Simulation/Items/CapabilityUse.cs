using System.Collections.Generic;

[System.Serializable]
public class CapabilityUse
{
    public int Value = 1;
    public float MinimumEffectiveRange = 0;
    public float MaximumEffectiveRange = 1;
    public List<ItemRequirement> ItemRequirements;
    public ItemConditions RequiredConditions;
    public ItemConditions ForbiddenConditions;

     public CapabilityUse Copy()
    {
        var copy = new CapabilityUse
        {
            Value = Value,
            MinimumEffectiveRange = MinimumEffectiveRange,
            MaximumEffectiveRange = MaximumEffectiveRange,
            RequiredConditions = RequiredConditions,
            ForbiddenConditions = ForbiddenConditions,
            ItemRequirements = new List<ItemRequirement>()
        };

        foreach (var requirement in ItemRequirements)
        {
            copy.ItemRequirements.Add(requirement?.Copy());
        }

        return copy;
    }
}