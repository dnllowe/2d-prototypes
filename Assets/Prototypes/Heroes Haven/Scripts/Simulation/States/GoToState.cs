[System.Serializable]
public class GoToState : StateBase
{
    public float TravelSpeed = 0.3f;
    public bool ReachedDestination;

    public GoToState(Entity entity) : base(entity) {}
    public static GoToState FromTask(Task task, Entity entity)
    {
        var goToState = new GoToState(entity)
        {
            Task = task
        };

        return goToState;
    }

    public override StateResult Update(float deltaTime)
    {
        if (ReachedDestination) return StateResult.Complete;

        var position = PositionRegistry.Components.Get(Entity.Id);
        var destination = PositionRegistry.Components.Get(Task.Target.Id);
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