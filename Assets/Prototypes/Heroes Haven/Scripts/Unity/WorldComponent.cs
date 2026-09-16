using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class WorldComponent : SerializedMonoBehaviour
{
    public CurrentConfig CurrentConfig;
    [OdinSerialize, NonSerialized] public World World = new World();

    public void Awake()
    {
        World.Config = new GameConfig();
        World.Config = CurrentConfig.GameConfig.ToSimulation();
    }

    public void Update()
    {
        World.Tick(Time.deltaTime);
    }

    [Button]
    public void Initialize(WorldState state)
    {
        World.Initialize(state);
        InitializeFromState(state);
    }

    public void InitializeFromState(WorldState state)
    {
        foreach (var item in state.WorldItems)
        {

        }


    }
}