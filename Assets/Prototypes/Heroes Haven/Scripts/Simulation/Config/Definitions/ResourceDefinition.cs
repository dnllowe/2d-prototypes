using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class ResourceDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public List<ResourceItemDefinition> ExtractableItems = new List<ResourceItemDefinition>();
}