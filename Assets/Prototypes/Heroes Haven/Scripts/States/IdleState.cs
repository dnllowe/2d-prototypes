public class IdleState : StateBase
{
    public IdleState(Entity entity) : base(entity) {}

    public override StateResult Update(float deltaTime)
    {
        return StateResult.Running;
    }
}