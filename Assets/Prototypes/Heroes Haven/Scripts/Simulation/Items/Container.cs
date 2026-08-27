using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Container : Component
{
    public Container(uint entityId) : base(entityId) {}

    public List<Item> Entries = new List<Item>();
    public int Capacity;

    public int GetAvailableSpace()
    {
        return Math.Max(0, Capacity - Entries.Count);
    }

    public bool IsFull()
    {
        return Entries.Count >= Capacity;
    }

    /// <summary>
    /// Places an item inside the container.
    /// Returns whether it was successfully placed.
    /// </summary>
    public bool PlaceInside(Item item)
    {
        if (item == null) return false;
        if (IsFull()) return false;

        Entries.Add(item);
        return true;
    }

    /// <summary>
    /// Places as many items as possible inside the container.
    /// Returns the number successfully placed.
    /// </summary>
    public int PlaceInside(IEnumerable<Item> items)
    {
        if (items == null) return 0;

        var placed = 0;

        foreach (var item in items)
        {
            if (!PlaceInside(item)) break;
            placed++;
        }

        return placed;
    }

    /// <summary>
    /// Removes and returns up to the requested number of items
    /// matching the requirement.
    /// </summary>
    public List<Item> Remove(ItemRequirement requirement, int amount)
    {
        if (requirement == null || amount <= 0) return new List<Item>();

        var removed = Entries
            .Where(requirement.Matches)
            .Take(amount)
            .ToList();

        foreach (var item in removed)
        {
            Entries.Remove(item);
        }

        return removed;
    }

    /// <summary>
    /// Convenience overload for removing items by type.
    /// </summary>
    public List<Item> Remove(Items type, int amount)
    {
        if (amount <= 0) return new List<Item>();

        var removed = Entries
            .Where(item => item.Properties.Type == type)
            .Take(amount)
            .ToList();

        foreach (var item in removed)
        {
            Entries.Remove(item);
        }

        return removed;
    }

    /// <summary>
    /// Moves matching items from another container into this one.
    /// Returns the number moved.
    /// </summary>
    public int TakeFrom(ItemRequirement requirement, int amount, Container source)
    {
        if (requirement == null || source == null || amount <= 0) return 0;

        var amountToMove = Math.Min(amount, GetAvailableSpace());
        if (amountToMove <= 0) return 0;

        var itemsToMove = source.Entries
            .Where(requirement.Matches)
            .Take(amountToMove)
            .ToList();

        foreach (var item in itemsToMove)
        {
            source.Entries.Remove(item);
            Entries.Add(item);
        }

        return itemsToMove.Count;
    }

    /// <summary>
    /// Convenience overload for moving items by type.
    /// </summary>
    public int TakeFrom(Items type, int amount, Container source)
    {
        if (source == null || amount <= 0) return 0;

        var amountToMove = Math.Min(amount, GetAvailableSpace());
        if (amountToMove <= 0) return 0;

        var itemsToMove = source.Entries
            .Where(item => item.Properties.Type == type)
            .Take(amountToMove)
            .ToList();

        foreach (var item in itemsToMove)
        {
            source.Entries.Remove(item);
            Entries.Add(item);
        }

        return itemsToMove.Count;
    }

    public int GetQuantity(Items type)
    {
        return Entries.Count(item => item.Properties.Type == type);
    }

    public int GetQuantity(ItemRequirement requirement)
    {
        if (requirement == null) return 0;

        return Entries.Count(requirement.Matches);
    }

    /// <summary>
    /// Matches concrete item instances against a set of requirements.
    ///
    /// Each item can satisfy at most one required quantity.
    /// Scarcer requirements claim items first.
    /// </summary>
    public RequirementMatchResult MatchRequirements(IEnumerable<ItemRequirement> requirements)
    {
        var result = new RequirementMatchResult();
        var availableItems = new List<Item>(Entries);

        if (requirements == null) return result;

        var orderedRequirements = requirements
            .Where(requirement => requirement != null && requirement.Quantity > 0)
            .OrderBy(requirement => Entries.Count(requirement.Matches))
            .ThenByDescending(GetRequirementSpecificity)
            .ToList();

        foreach (var requirement in orderedRequirements)
        {
            var matched = availableItems
                .Where(requirement.Matches)
                .Take(requirement.Quantity)
                .ToList();

            result.Matches[requirement] = matched;

            foreach (var item in matched)
            {
                availableItems.Remove(item);
            }

            var remaining = requirement.Quantity - matched.Count;

            if (remaining > 0)
            {
                result.RemainingRequirements.Add(
                    requirement.CopyWithNewQuantity(remaining));
            }
        }

        return result;
    }

    public bool HasRequirements(IEnumerable<ItemRequirement> requirements)
    {
        return MatchRequirements(requirements).IsSatisfied;
    }

    public List<ItemRequirement> GetRemainingRequirements(IEnumerable<ItemRequirement> requirements)
    {
        return MatchRequirements(requirements).RemainingRequirements;
    }

    /// <summary>
    /// Uses the items required by the supplied requirements.
    /// Currently removes all matched items.
    /// </summary>
    public bool Use(IEnumerable<ItemRequirement> requirements)
    {
        var match = MatchRequirements(requirements);

        if (!match.IsSatisfied)
            return false;

        foreach (var pair in match.Matches)
        {
            var requirement = pair.Key;

            if (!requirement.ConsumedAfterUse)
                continue;

            foreach (var item in pair.Value)
            {
                Entries.Remove(item);
            }
        }

        return true;
    }

    private int GetRequirementSpecificity(ItemRequirement requirement)
    {
        var specificity = 0;

        if (requirement.ItemTypeRequirement != Items.None)
            specificity += 10;

        if (requirement.MaterialRequirement != ItemMaterials.None)
            specificity += 5;

        if (requirement.RequiredConditions != ItemConditions.None)
            specificity += 3;

        if (requirement.ForbiddenConditions != ItemConditions.None)
            specificity += 3;

        if (requirement.MinimumGrade != ItemGrades.None)
            specificity += 2;

        if (requirement.MinimumRequiredCapabilityValues != null)
            specificity += requirement.MinimumRequiredCapabilityValues.Count * 3;

        return specificity;
    }
}