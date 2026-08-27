using System;

[System.Serializable]
public class ResolveConstraintState : StateBase
{
    public Constraints Constraints;
    public ResolveConstraintState(World world, uint entityId, Task task, Constraints constraints) : base(world, entityId, task)
    {
        Constraints = constraints;
    }

    public override StateResult Tick(float deltaTime)
    {
        foreach (var constraint in ConstraintHelper.GetAllConstraints())
        {
            if (ConstraintHelper.HasConstraint(constraint, Constraints))
            {
                UnityEngine.Debug.Log($"We have a {constraint} problem");
                var solution = ConstraintSolver.GetSolution(constraint);
                if (solution == Actions.None)
                {
                    UnityEngine.Debug.Log($"I don't have a solution for {constraint}");
                    return StateResult.Canceled;
                }
                UnityEngine.Debug.Log($"I have a solution for {constraint}: {solution}");
                return new StateResult
                {
                    Status = StateStatus.NeedsTask,
                    Task = new Task
                    {
                        Action = solution,
                        Item = Task.Item,
                    }
                };
            }
        }
        return StateResult.Canceled;
    }
}