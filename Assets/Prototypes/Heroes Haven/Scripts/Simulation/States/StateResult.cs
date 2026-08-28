using System.Collections.Generic;

[System.Serializable]
public class StateResult
{
    public StateStatus Status;
    public Task Task;
    public uint Target;
    public bool HasTarget;
    public Constraints Constraints;
    public bool HasConstraints;
    public HashSet<uint> ExcludedTargets = new HashSet<uint>();
    public bool HasExcludedTargets;

    public static StateResult Running = new StateResult{ Status = StateStatus.Running };
    public static StateResult None = new StateResult{ Status = StateStatus.None };
    public static StateResult Complete = new StateResult{ Status = StateStatus.Complete };
    public static StateResult Failed = new StateResult{ Status = StateStatus.Failed };
    public static StateResult Canceled = new StateResult{ Status = StateStatus.Canceled };
}