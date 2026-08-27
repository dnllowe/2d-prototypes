using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfigComponent : SerializedScriptableObject
{
    [OdinSerialize, NonSerialized]
    public ItemDefinitionsComponent ItemDefinitions; 

    [OdinSerialize, NonSerialized]
    public RecipeDefinitionsComponent RecipeDefinitions;

    public GameConfig ToSimulation()
    {
        return new GameConfig
        {
            ItemDefinitions = ItemDefinitions.ToSimulation(),
            RecipeDefinitions = RecipeDefinitions.ToSimulation(),
        };
    }
}
