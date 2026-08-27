[System.Serializable]
public class FindState : StateBase
{
    public FindState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        var searchResults = SearchUtility.FindItemCandidates(Task.Item);
        if (searchResults.FoundWorldItems())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.WorldItems[0].EntityId,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Gather,
                    Item = Task.Item,
                    Target = searchResults.WorldItems[0].EntityId,
                }
            };
        }

        if (searchResults.FoundContainers())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.ContainerItems[0].EntityId,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Gather,
                    Item = Task.Item,
                    Target = searchResults.ContainerItems[0].EntityId,
                }
            };
        }

        if (searchResults.FoundResources())
        {
            return new StateResult
            {
                Status = StateStatus.NeedsTask,
                Target = searchResults.ResourceItems[0].EntityId,
                HasTarget = true,
                Task = new Task
                {
                    Action = Actions.Extract,
                    Item = Task.Item,
                    Target = searchResults.ResourceItems[0].EntityId,
                }
            };
        }

        return StateResult.Canceled;
    }
}
