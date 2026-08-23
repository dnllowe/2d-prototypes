[System.Serializable]
public class ResolveConstraintState : StateBase
{
    public ConstraintEvaluation Constraints;
    public ResolveConstraintState(Entity entity) : base (entity) {}
    public static ResolveConstraintState FromConstraint(ConstraintEvaluation constraint, Task task, Entity entity)
    {
        var resolver = new ResolveConstraintState(entity)
        {
            Constraints = constraint,
            Task = task
        };

        return resolver;
    }

    public override StateResult Update(float deltaTime)
    {
        var solution = ConstraintSolver.GetSolution(Constraints.Constraints, Constraints.Context);
        UnityEngine.Debug.Log($"I have a solution for {Constraints.Constraints}: {solution}");

        return StateResult.Running;
    }
}