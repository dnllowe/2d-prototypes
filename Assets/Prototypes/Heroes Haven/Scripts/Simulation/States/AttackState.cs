[System.Serializable]
public class AttackState : StateBase
{
    public AttackState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override void Enter()
    {
        base.Enter();
        UnityEngine.Debug.Log("Entered Attack State");
    }
    public override StateResult Tick(float deltaTime)
    {
        return StateResult.Running;
    }
}