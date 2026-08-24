[System.Serializable]
public class FindState : StateBase
{
    public FindState(Entity entity) : base(entity) {}

    public override StateResult Update(float deltaTime)
    {
        var searchResults = SearchUtility.FindItemCandidates(Task.Item);
        if (searchResults.FoundWorldItems())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.WorldItems[0].Entity,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Gather,
                    Item = Task.Item,
                    Target = searchResults.WorldItems[0].Entity,
                }
            };
        }

        if (searchResults.FoundContainers())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.ContainerItems[0].Entity,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Gather,
                    Item = Task.Item,
                    Target = searchResults.ContainerItems[0].Entity,
                }
            };
        }

        if (searchResults.FoundResources())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.ResourceItems[0].Entity,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Extract,
                    Item = Task.Item,
                    Target = searchResults.ResourceItems[0].Entity,
                }
            };
        }

        return StateResult.Canceled;
    }

    public static FindState FromTask(Task task, Entity entity)
    {
        var find = new FindState(entity)
        {
            Task = task
        };

        return find;
    }
}
