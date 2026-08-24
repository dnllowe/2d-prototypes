using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeDefinition", menuName = "Scriptable Objects/RecipeDefinition")]
public class RecipeDefinitionComponent : SerializedScriptableObject
{
    public RecipeDefinition RecipeDefinition;
}
