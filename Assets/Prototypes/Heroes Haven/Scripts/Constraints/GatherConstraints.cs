using System.Collections.Generic;

public static class GatherConstraints
{
    public static ConstraintEvaluation Get(ConstraintContext context)
    {
        var constraints = Constraints.None;
        var constraintDetails = new List<ConstraintInfo>();

        var outOfRangeConstraint = ConstraintHelper.GetOutOfRangeConstraint(context.Source, context.Target);
        if (outOfRangeConstraint.Constraint != Constraints.None)
        {
            constraints |= outOfRangeConstraint.Constraint;
            constraintDetails.Add(outOfRangeConstraint);
        }
        var aliveConstraint = ConstraintHelper.GetAliveConstraint(context.Target);
        if (aliveConstraint.Constraint != Constraints.None)
        {
            constraints |= aliveConstraint.Constraint;
            constraintDetails.Add(aliveConstraint);
        }

        context.Target.GetComponents<ItemTag>();

        return new ConstraintEvaluation
        {
            Constraints = constraints,
            Details = constraintDetails,
            Context = context,
        };
    }
}