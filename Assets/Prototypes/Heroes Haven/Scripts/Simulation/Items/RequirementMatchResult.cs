using System.Collections.Generic;

[System.Serializable]
public class RequirementMatchResult
{
    public Dictionary<ItemRequirement, List<Item>> Matches = new Dictionary<ItemRequirement, List<Item>>();
    public List<ItemRequirement> RemainingRequirements = new List<ItemRequirement>();
    public bool IsSatisfied => RemainingRequirements.Count == 0;
}