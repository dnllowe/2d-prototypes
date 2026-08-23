[System.Serializable]
public class AttackState : StateBase
{
    public AttackState(Entity entity) : base(entity) {}
    public static AttackState FromTask(Task task, Entity entity)
    {
        var attack = new AttackState(entity)
        {
            Task = task
        };
        return attack;
    }

    public override StateResult Update(float deltaTime)
    {
        return StateResult.Running;
    }
}