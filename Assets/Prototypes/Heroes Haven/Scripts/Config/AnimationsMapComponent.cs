using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimationsMapComponent", menuName = "Scriptable Objects/AnimationsMapComponent")]
public class AnimationsMapComponent : SerializedScriptableObject
{
    public Dictionary<BodyTypes, AnimationMapComponent> BodyAnimations = new Dictionary<BodyTypes, AnimationMapComponent>();
}
