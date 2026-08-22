
[System.Serializable]
public class ListenState : StateBase
{
    Entity Speaker;

    public ListenState(Entity entity) : base(entity) {}
    public ListenState(Entity speaker, Entity entity) : base(entity)
    {
        Speaker = speaker;
    }

    public static ListenState FromTask(Task task, Entity entity)
    {
        var listen = new ListenState(entity);
        listen.Task = task;

        return listen;
    }
}
