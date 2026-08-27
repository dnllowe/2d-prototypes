using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinitionComponent", menuName = "Scriptable Objects/ItemDefinitionComponent")]
public class ItemDefinitionComponent : SerializedScriptableObject
{
    [NonSerialized, OdinSerialize]
    public ItemDefinition ItemDefinition; 
}
