using System;
using System.Collections.Generic;
using Sirenix.Serialization;

[System.Serializable]
public class Resource : Component
{
    public Resource(uint entityId) : base(entityId)
    {
        ResourceRegistry.Components.Register(this, entityId);
    }

    [NonSerialized, OdinSerialize]
    public Dictionary<Items, Extraction> ExtractableItems = new Dictionary<Items, Extraction>();

    public bool HasRequirements(IEnumerable<ItemRequirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (GetQuantity(requirement.Properties.Type) < requirement.Quantity) return false;
        }

        return true;
    }

    public int GetQuantity(Items item)
    {
        if (!ExtractableItems.TryGetValue(item, out var extractableItem)) return 0;
        return extractableItem.Count;
    }

    public List<ItemRequirement> GetRemainingRequirements(IEnumerable<ItemRequirement> requirements)
    {
        var additionalRequirements = new List<ItemRequirement>();
        foreach (var requirement in requirements)
        {
            var needs = requirement.Quantity - GetQuantity(requirement.Properties.Type);
            if (needs > 0)  additionalRequirements.Add(requirement.CopyWithNewQuantity(needs));
        }

        return additionalRequirements;
    }
}