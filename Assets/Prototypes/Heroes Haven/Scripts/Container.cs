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
    public int Take(Items item, int amount)
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
    public int Place(Items item, int amount)
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
    /// <param name="other"></param>
    /// <returns></returns>
    public int Move(Items item, int amount, Container other)
    {
        if (amount <= 0) return 0;

        var availableSpace = GetAvailableSpace();
        return Place(item, other.Take(item, Mathf.Min(availableSpace, amount)));
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

    public bool HasRequirements(Recipe recipe)
    {
        foreach (var requirement in recipe.Requirements)
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

    public List<Requirement> GetRequirements(Recipe recipe)
    {
        var requirements = new List<Requirement>();
        foreach (var requirement in recipe.Requirements)
        {
            var needs = requirement.Quantity - GetQuantity(requirement.Item);
            if (needs > 0)  requirements.Add(new Requirement
            {
                Item = requirement.Item,
                Quantity = needs,
            });
        }

        return requirements;
    }
}

