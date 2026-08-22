using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RecipeDefinitions", menuName = "Scriptable Objects/RecipeDefinitions")]
public class RecipeDefinitions : SerializedScriptableObject
{
    public Dictionary<Items, RecipeDefinition> Recipes = new Dictionary<Items, RecipeDefinition>();
}
