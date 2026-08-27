using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeDefinition", menuName = "Scriptable Objects/RecipeDefinition")]
public class RecipeDefinitionComponent : SerializedScriptableObject
{
    [NonSerialized, OdinSerialize]
    public RecipeDefinition RecipeDefinition;
}
