using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animation
{
    public List<Sprite> Frames = new List<Sprite>();
    public int FrameRate;
    public int MinFps;
    public int MaxFps;
}