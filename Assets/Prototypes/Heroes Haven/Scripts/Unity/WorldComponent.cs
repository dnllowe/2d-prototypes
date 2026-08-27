using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class WorldComponent : SerializedMonoBehaviour
{
    public CurrentConfig CurrentConfig;
    [OdinSerialize, NonSerialized] public World World;

    public void Awake()
    {
        World.Config = new GameConfig();
        World.Config = CurrentConfig.GameConfig.ToSimulation();
    }

    public void Update()
    {
        World.Tick(Time.deltaTime);
    }
}