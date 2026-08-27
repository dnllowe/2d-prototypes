public static class GatherConstraints
{
    public static Constraints Get(World world, uint selfEntityId, Task task)
    {
        var missingTarget = ConstraintHelper.GetMissingTargetConstraint(task.Target);
        if (missingTarget != Constraints.None) return missingTarget;

        var outOfRange = ConstraintHelper.GetTargetOutOfRangeConstraint(world, selfEntityId, task.Target); 
        if (outOfRange != Constraints.None) return outOfRange;

        var alive = ConstraintHelper.GetAliveConstraint(world, task.Target);
        if (alive != Constraints.None) return alive;

        // TODO: container locked constraint

        return Constraints.None;
    }
}