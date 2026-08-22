using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

[AutoStaticsCleanup]
public static partial class TaggedItems
{
    public static Dictionary<Items, List<Entity>> Items = new Dictionary<Items, List<Entity>>();

    public static void Register(ItemTag taggedItem)
    {
        if (!Items.ContainsKey(taggedItem.Tag))
        {
            Items[taggedItem.Tag] = new List<Entity>();
        }

        Items[taggedItem.Tag].Add(taggedItem.Entity);
    }
}

public class ItemTag : MonoBehaviour
{
    public Items Tag;
    public ItemConditions Conditions;
    public ItemCapabilities Capabilities;
    public ItemMaterials Materials;
    public ItemGrades Grade;
    public Entity Entity;

    void Awake()
    {
        Entity = GetComponent<Entity>();
        TaggedItems.Register(this);
    }

    [Button]
    public bool HasConditions(ItemConditions conditions)
    {
        return (Conditions & conditions) == conditions;
    }

    [Button]
    public void AddConditions(ItemConditions conditions)
    {
        Conditions |= conditions;
    }

    [Button]
    public void RemoveConditions(ItemConditions conditions)
    {
        Conditions &= ~conditions;
    }

    [Button]
    public bool HasCapabilities(ItemCapabilities capabilities)
    {
        return (Capabilities & capabilities) == capabilities;
    }

    [Button]
    public void AddCapabilities(ItemCapabilities capabilities)
    {
        Capabilities |= capabilities;
    }

    [Button]
    public void RemoveCapabilities(ItemCapabilities capabilities)
    {
        Capabilities &= ~capabilities;
    }
}