using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateResult
{
    public StateStatus Status;
    public Task Task;
    public Vector3 Destination;
    public Entity Target;
    public Items Item;
    public HashSet<Entity> ExcludedTargets = new HashSet<Entity>();
    public bool HasTarget;
    public bool HasDestination;
    public bool HasItem;
    public bool HasExcludedTargets;

    public static StateResult Running = new StateResult{ Status = StateStatus.Running };
    public static StateResult None = new StateResult{ Status = StateStatus.None };
    public static StateResult Complete = new StateResult{ Status = StateStatus.Complete };
    public static StateResult Failed = new StateResult{ Status = StateStatus.Failed };
}