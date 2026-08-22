using UnityEngine;

[System.Serializable]
public class GoToState : StateBase
{
    public Entity Target;
    public Vector3 Destination;
    public float TravelSpeed = 0.1f;

    public bool ReachedDestination;

    public GoToState(Entity entity) : base(entity) {}
    public static GoToState FromTask(Task task, Entity entity)
    {
        var goToState = new GoToState(entity);
        goToState.Target = task.Target;
        goToState.Destination = task.Destination;
        goToState.Task = task;

        return goToState;
    }

    public override StateResult Update(float deltaTime)
    {
        if (ReachedDestination) return StateResult.Complete;

        if (Target != null) Destination = Target.transform.position;
        var direction = Destination - Entity.transform.position;
        direction.z = 0;
        direction.y = 0;

        Entity.transform.Translate(direction * TravelSpeed * deltaTime);
        var distance = Vector3.Distance(Destination, Entity.transform.position);

        if (distance < 1) ReachedDestination = true;

        if (ReachedDestination) return new StateResult
        {
            Status = StateStatus.Complete,
            Target = Task.Target,
            HasTarget = true,
            Destination = Task.Destination,
            HasDestination = true,
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
        Target = null;
        ReachedDestination = false;
    }
}