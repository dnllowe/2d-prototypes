using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StateMachine : SerializedMonoBehaviour
{
    public CurrentConfig Config;
    Entity entity;
    public StateBase CurrentState;
    public Stack<Task> SuspendedTasks = new Stack<Task>();

    void Awake()
    {
        entity = GetComponent<Entity>();
        CurrentState = new IdleState(entity);
    }

    void Update()
    {
        var result = CurrentState.Update(Time.deltaTime);

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

    [Button]
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
            default:
                Idle();
                break;
        }
    }

    public void ChangeState(StateBase state)
    {
        CurrentState.Exit();
        CurrentState = state;
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

    [Button]
    public void Speak(List<string> dialogue)
    {
        var dialogueState = new DialogueState(entity);
        dialogueState.SetDialogue(dialogue);

        ChangeState(dialogueState);
    }

    [Button]
    public void Craft(Task task)
    {
        var hasRecipe = Config.GameConfig.RecipeDefinitions.Recipes.TryGetValue(task.Item, out var recipe);
        if (!hasRecipe)
        {
            Debug.LogWarning($"No recipe for ${task.Item}");
            return;
        }

        var craft = CraftState.FromTask(task, entity, recipe.Recipe);
        ChangeState(craft);
    }

    [Button]
    public void Find(Task task)
    {
        ChangeState(FindState.FromTask(task, entity));
    }

    [Button]
    public void Attack(Task task)
    {
        ChangeState(AttackState.FromTask(task, entity));
    }
}
