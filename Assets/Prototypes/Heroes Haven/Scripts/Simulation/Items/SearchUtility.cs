using System.Collections.Generic;

[System.Serializable]
public class ItemSearchResult
{
    public List<ItemCandidate> WorldItems = new List<ItemCandidate>();
    public List<ItemCandidate> ContainerItems = new List<ItemCandidate>();
    public List<ItemCandidate> ResourceItems = new List<ItemCandidate>();

    public bool FoundItems()
    {
        return FoundWorldItems() || FoundContainers() || FoundResources(); 
    }

    public bool FoundWorldItems()
    {
        return WorldItems.Count > 0;
    }

    public bool FoundContainers()
    {
        return ContainerItems.Count > 0;
    }

    public bool FoundResources()
    {
        return ResourceItems.Count >0;
    }
}

public static class SearchUtility
{
    public static ItemSearchResult FindItemCandidates(ItemRequirement item)
    {
        return new ItemSearchResult
        {
            WorldItems = FindWorldItems(item),
            ContainerItems = FindContainers(item),
            ResourceItems = FindResources(item),
        };
    }

    public static List<ItemCandidate> FindWorldItems(ItemRequirement item, List<ItemCandidate> results = null)
    {
        if (results == null) results = new List<ItemCandidate>();
        return results;
    }

    public static List<ItemCandidate> FindContainers(ItemRequirement item, List<ItemCandidate> results = null)
    {
        if (results == null) results = new List<ItemCandidate>();
        return results;
    }

    public static List<ItemCandidate> FindResources(ItemRequirement item, List<ItemCandidate> results = null)
    {
        if (results == null) results = new List<ItemCandidate>();
        return results;
    }
}