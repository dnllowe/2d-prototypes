[System.Serializable]
public class GoToState : StateBase
{
    public float TravelSpeed = 0.3f;
    public bool ReachedDestination;

    public GoToState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        if (ReachedDestination) return StateResult.Complete;

        var position = world.PositionRegistry.Get(EntityId);
        var destination = world.PositionRegistry.Get(Task.Target);
        var direction = destination - position;
        direction.Z = 0;
        direction.Y = 0;

        position.MoveBy(direction * TravelSpeed * deltaTime);
        var distance = Position.Distance(destination, position);

        if (distance < 1) ReachedDestination = true;

        if (ReachedDestination) return new StateResult
        {
            Status = StateStatus.Complete,
        };

        return StateResult.Running;
    }

    public override void Enter()
    {
        base.Enter();
        ReachedDestination = false;
    }

    public override void Exit()
    {
        base.Exit();
        ReachedDestination = false;
    }
}