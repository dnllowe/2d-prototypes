using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationMapComponent", menuName = "Scriptable Objects/AnimationMapComponent")]
public class AnimationMapComponent : SerializedScriptableObject
{
    [OdinSerialize, NonSerialized]
    public AnimationMap Animations; 
}
