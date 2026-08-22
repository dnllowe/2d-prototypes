using UnityEngine.Rendering;

[System.Serializable]
public class AnimationMap
{
    public SerializedDictionary<Actions, Animation> Animations = new SerializedDictionary<Actions, Animation>();
}