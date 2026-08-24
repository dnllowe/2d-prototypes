using System;

[System.Serializable]
public class ResolveConstraintState : StateBase
{
    public Constraints Constraints;
    public ResolveConstraintState(Entity entity) : base (entity) {}
    public static ResolveConstraintState FromConstraint(Constraints constraint, Task task, Entity entity)
    {
        UnityEngine.Debug.Log("I need to solve this...");
        var resolver = new ResolveConstraintState(entity)
        {
            Constraints = constraint,
            Task = task
        };

        return resolver;
    }

    public override StateResult Update(float deltaTime)
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