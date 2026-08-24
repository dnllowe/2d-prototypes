using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "RecipeDefinitions", menuName = "Scriptable Objects/RecipeDefinitions")]
public class RecipeDefinitionsComponent : SerializedScriptableObject
{
    public RecipeDefinitions RecipeDefinitions;
}
