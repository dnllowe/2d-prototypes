using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinitionComponent", menuName = "Scriptable Objects/ItemDefinitionComponent")]
public class ItemDefinitionComponent : SerializedScriptableObject
{
    public Sprite Sprite;

    [NonSerialized, OdinSerialize]
    public ItemDefinition ItemDefinition; 
}
