using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// A task maps to a state to carry it out
// When the state is complete, the task is complete
// When the task is complete, 

// What do tasks distill into?
// Craft (go to -> gather -> go to -> craft)
// Gather (go to -> gather -> go to -> deliver)
// Deliver (go to -> gather -> go to -> deliver)
// Search / Find


// So command-level actions are
// Craft
// Deliver

// TODO: how do we know if we're doing a subtask or the task itself?
public class TaskManager : MonoBehaviour
{
    public CurrentConfig Config;
    public Task CurrentTask;
    public StateMachine StateMachine;
    public Container Container;
    Entity entity;

    void Awake()
    {
        entity = GetComponent<Entity>();
        Container = GetComponent<Container>();
        StateMachine = GetComponent<StateMachine>();
    }

    void Start()
    {
        StateMachine.ChangeState(StateFromTask(CurrentTask));    
    }

    [Button]
    public void AssignTask(Instruction instruction)
    {
        StateMachine.ChangeState(BuildTask(instruction));
    }

    public StateBase StateFromTask(Task task)
    {
        switch (task.Action)
        {
            case Actions.None: return new IdleState(entity);
            default: return new IdleState(entity);

        }
    }

    public void QueueTask(Task newTask)
    {
    }

    public void PrioritizeTask(Task newTask)
    {
    }

    [Button]
    public Task BuildTask(Instruction instruction)
    {
        switch (instruction.Type)
        {
            case Instructions.None:
                return new Task { Action = Actions.Idle };
            case Instructions.Deliver:
                return new Task
                {
                    Action = Actions.Deliver,
                    Item = instruction.Item,
                    Destination = instruction.Destination,
                    Target = instruction.Target,
                    Value  = 1,
                };
            case Instructions.Craft:
                return new Task
                {
                    Action = Actions.Craft,
                    Item = instruction.Item,
                    Destination = instruction.Destination,
                    Target = instruction.Target,
                    Value = 1,
                };
            default:
                return new Task { Action = Actions.Idle };
        }
    }
}
