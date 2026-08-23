using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Container : SerializedMonoBehaviour
{
    public Dictionary<Items, int> Items = new Dictionary<Items, int>();
    public int Capacity;

    /// <summary>
    /// Returns how much was taken
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int Remove(Items item, int amount)
    {
        if (amount <= 0) return 0;
        if (!Items.ContainsKey(item)) return 0;

        var taken = Mathf.Min(Items[item], amount);
        Items[item] -= taken;

        return taken;
    }

    /// <summary>
    /// Return how much was placed
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int PlaceInside(Items item, int amount)
    {
        if (amount <= 0) return 0;
        var availableSpace = GetAvailableSpace();
        if (!Items.ContainsKey(item)) Items.Add(item, Mathf.Min(availableSpace, amount));
        else Items[item] += Mathf.Min(availableSpace, amount);

        return Mathf.Min(amount, availableSpace);
    }

    /// <summary>
    /// Returns how much was moved
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public int TakeFrom(Items item, int amount, Container source)
    {
        if (amount <= 0) return 0;

        var availableSpace = GetAvailableSpace();
        return PlaceInside(item, source.Remove(item, Mathf.Min(availableSpace, amount)));
    }

    public bool Use(IEnumerable<Requirement> requirements)
    {
        if (!HasRequirements(requirements)) return false;
        foreach (var requirement in requirements)
        {
            Remove(requirement.Item, requirement.Quantity);
        }

        return true;
    }

    public int GetAvailableSpace()
    {
        var availableSpace = Capacity;
        foreach (var (item, quantity) in Items)
        {
            availableSpace -= quantity;
        }

        return availableSpace;
    }

    public bool HasRequirements(IEnumerable<Requirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (GetQuantity(requirement.Item) < requirement.Quantity) return false;
        }

        return true;
    }

    public int GetQuantity(Items item)
    {
        if (!Items.TryGetValue(item, out var amount)) return 0;
        return amount;
    }

    public List<Requirement> GetRemainingRequirements(IEnumerable<Requirement> requirements)
    {
        var additionalRequirements = new List<Requirement>();
        foreach (var requirement in requirements)
        {
            var needs = requirement.Quantity - GetQuantity(requirement.Item);
            if (needs > 0)  additionalRequirements.Add(new Requirement
            {
                Item = requirement.Item,
                Quantity = needs,
            });
        }

        return additionalRequirements;
    }
}

