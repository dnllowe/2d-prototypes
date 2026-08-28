using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
public class ResourceDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    [NonSerialized, OdinSerialize]
    public Dictionary<Items, Extraction> ExtractableItems = new Dictionary<Items, Extraction>();

    public Resource ToSimulation(uint entityId)
    {
        return new Resource(entityId)
        {
            ExtractableItems = ExtractableItems,
        };
    }
}