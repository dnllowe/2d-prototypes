using System.Collections.Generic;

public static class DeliverConstraints
{
    public static Constraints Get(World world, uint selfEntityId, Task task)
    {
        var container = world.ContainerRegistry.Get(selfEntityId);
        var requirements = new List<ItemRequirement>{{ task.Item }};
        var missingRequirements = ConstraintHelper.GetMissingRequirementsConstraint(container, requirements);
        if (missingRequirements != Constraints.None) return missingRequirements;

        var outOfRange = ConstraintHelper.GetTargetOutOfRangeConstraint(world, selfEntityId, task.Target);
        if (outOfRange != Constraints.None) return outOfRange;

        return Constraints.None;
    }
}