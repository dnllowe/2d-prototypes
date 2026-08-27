public class IdleState : StateBase
{
    public IdleState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        return StateResult.Running;
    }
}