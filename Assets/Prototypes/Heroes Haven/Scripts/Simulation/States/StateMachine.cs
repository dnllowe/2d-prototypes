using System.Collections.Generic;

[System.Serializable]
public class StateMachine
{
    public GameConfig GameConfig;
    Entity entity;
    public StateBase CurrentState;
    public Stack<Task> SuspendedTasks = new Stack<Task>();
    public List<StateBase> StateHistory = new List<StateBase>();

    public void Update(float deltaTime)
    {
        var result = CurrentState.Update(deltaTime);

        if (result.Status == StateStatus.NeedsTask)
        {
            SuspendedTasks.Push(CurrentState.Task);
            ChangeState(result.Task);
            return;
        }
        if (result.Status == StateStatus.Blocked)
        {
            SuspendedTasks.Push(CurrentState.Task);
            ChangeState(ResolveConstraintState.FromConstraint(result.Constraints, CurrentState.Task, CurrentState.Entity));
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
        var goToState = GoToState.FromTask(task, entity);
        ChangeState(goToState);
    }

    public void Gather(Task task)
    {
        var gather = GatherState.FromTask(task, entity);
        ChangeState(gather);
    }

    public void Idle()
    {
        ChangeState(new IdleState(entity));
    }

    public void Deliver(Task task)
    {
        var delivery = DeliverState.FromTask(task, entity);
        ChangeState(delivery);
    }

    public void Speak(List<string> dialogue)
    {
        var dialogueState = new DialogueState(entity);
        dialogueState.SetDialogue(dialogue);

        ChangeState(dialogueState);
    }

    public void Craft(Task task)
    {
        var hasRecipe = GameConfig.RecipeDefinitions.Recipes.TryGetValue(task.Item.Properties.Type, out var recipe);
        if (!hasRecipe)
        {
            UnityEngine.Debug.LogWarning($"No recipe for ${task.Item}");
            return;
        }

        var craft = CraftState.FromTask(task, entity, recipe.Recipe);
        ChangeState(craft);
    }

    public void Find(Task task)
    {
        ChangeState(FindState.FromTask(task, entity));
    }

    public void Attack(Task task)
    {
        ChangeState(AttackState.FromTask(task, entity));
    }
}
