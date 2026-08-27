[System.Serializable]
public class StateBase
{
    public Task Task;
    protected World world;
    public uint EntityId;
    public Animation Animation;
    public float RemainingTime;

    public StateBase(World world, uint entityId, Task task)
    {
        this.world = world;
        EntityId = entityId;
        Task = task;
    }

    /// <summary>
    /// Returns true if state is complete
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public virtual StateResult Tick(float deltaTime)
    {
        return StateResult.None;
    }

    public virtual void Enter() {}

    public virtual void Exit() {}
}
