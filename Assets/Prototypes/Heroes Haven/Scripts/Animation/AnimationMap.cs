using System.Collections.Generic;

[System.Serializable]
public class AnimationMap
{
    public Dictionary<Actions, Animation> Animations = new Dictionary<Actions, Animation>();
}