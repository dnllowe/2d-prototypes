
[System.Serializable]
public class ListenState : StateBase
{
    public ListenState(Entity entity) : base(entity) {}

    public static ListenState FromTask(Task task, Entity entity)
    {
        var listen = new ListenState(entity)
        {
            Task = task
        };

        return listen;
    }
}
