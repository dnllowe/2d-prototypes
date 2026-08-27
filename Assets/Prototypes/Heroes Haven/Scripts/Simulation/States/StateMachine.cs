using System.Collections.Generic;

[System.Serializable]
public class StateMachine
{
    public World World;
    uint EntityId;
    public StateBase CurrentState;
    public Stack<Task> SuspendedTasks = new Stack<Task>();
    public List<StateBase> StateHistory = new List<StateBase>();

    public StateMachine(World world, uint entityId)
    {
        World = world;
        EntityId = entityId;
    }

    public void Update(float deltaTime)
    {
        var result = CurrentState.Tick(deltaTime);

        if (result.Status == StateStatus.NeedsTask)
        {
            SuspendedTasks.Push(CurrentState.Task);
            ChangeState(result.Task);
            return;
        }
        if (result.Status == StateStatus.Blocked)
        {
            SuspendedTasks.Push(CurrentState.Task);
            ChangeState(new ResolveConstraintState(World, EntityId, CurrentState.Task, result.Constraints));
            return;
        }
        if (result.Status == StateStatus.Complete)
        {
            if (SuspendedTasks.Count == 0) Idle();
            else
            {
               var resumeTask = SuspendedTasks.Pop();
               resumeTask.ApplyStateResult(result);
               ChangeState(resumeTask);
            }
        }
    }

    public void ChangeState(Task task)
    {
        switch (task.Action)
        {
            case Actions.None: 
            case Actions.Attack:
                Attack(task);
                break;
            case Actions.Sleep:
            case Actions.Idle:
                Idle();
                break;
            case Actions.Craft:
                Craft(task);
                break;
            case Actions.Gather:
                Gather(task);
                break;
            case Actions.GoTo:
                GoTo(task);
                break;
            case Actions.Find:
                Find(task);
                break;
            case Actions.Deliver:
                Deliver(task);
                break;
            default:
                Idle();
                break;
        }
    }

    public void ChangeState(StateBase state)
    {
        CurrentState.Exit();
        CurrentState = state;
        StateHistory.Add(state);
        CurrentState.Enter();
    }

    public void GoTo(Task task)
    {
        var goToState = new GoToState(World, EntityId, task);
        ChangeState(goToState);
    }

    public void Gather(Task task)
    {
        var gather = new GatherState(World, EntityId, task);
        ChangeState(gather);
    }

    public void Idle()
    {
        ChangeState(new IdleState(World, EntityId, new Task()));
    }

    public void Deliver(Task task)
    {
        var delivery = new DeliverState(World, EntityId, task);
        ChangeState(delivery);
    }

    public void Speak(List<string> dialogue)
    {
        var dialogueState = new DialogueState(World, EntityId, new Task());
        dialogueState.SetDialogue(dialogue);

        ChangeState(dialogueState);
    }

    public void Craft(Task task)
    {
        var craft = new CraftState(World, EntityId, task);
        ChangeState(craft);
    }

    public void Find(Task task)
    {
        ChangeState(new FindState(World, EntityId, task));
    }

    public void Attack(Task task)
    {
        ChangeState(new AttackState(World, EntityId, task));
    }
}
