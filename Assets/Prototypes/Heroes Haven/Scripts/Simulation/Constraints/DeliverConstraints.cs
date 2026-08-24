using System.Collections.Generic;

public static class DeliverConstraints
{
    public static Constraints Get(Entity self, Task task)
    {
        var container = ContainerRegistry.Components.Get(self.Id);
        var requirements = new List<ItemRequirement>{{  task.Item }};
        var missingRequirements = ConstraintHelper.GetMissingRequirementsConstraint(container, requirements);
        if (missingRequirements != Constraints.None) return missingRequirements;

        var outOfRange = ConstraintHelper.GetTargetOutOfRangeConstraint(self, task.Target);
        if (outOfRange != Constraints.None) return outOfRange;

        return Constraints.None;
    }
}