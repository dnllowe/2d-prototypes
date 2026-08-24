using Sirenix.OdinInspector;

public class TaskManager
{
    public CurrentConfig Config;
    public Task CurrentTask;
    public StateMachine StateMachine;
    
    [Button]
    public void AssignTask(Instruction instruction)
    {
        StateMachine.ChangeState(BuildTask(instruction));
    }

    public void QueueTask(Task newTask)
    {
    }

    public void PrioritizeTask(Task newTask)
    {
    }

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
                    Target = instruction.Target,
                };
            case Instructions.Craft:
                return new Task
                {
                    Action = Actions.Craft,
                    Item = instruction.Item,
                    Target = instruction.Target,
                };
            case Instructions.Gather:
                return new Task
                {
                    Action = Actions.Gather,
                    Item = instruction.Item,
                    Target = instruction.Target,
                };
            default:
                return new Task { Action = Actions.Idle };
        }
    }
}
