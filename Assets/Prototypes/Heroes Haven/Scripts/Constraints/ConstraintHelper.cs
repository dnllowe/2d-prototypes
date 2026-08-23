using UnityEngine;

public static class ConstraintHelper
{
    public static ConstraintInfo GetMissingTargetConstraint(Entity target)
    {
        if (target == null) return new ConstraintInfo
        {
            Constraint = Constraints.MissingTarget,
        };

        return new ConstraintInfo();
    }

    public static ConstraintInfo GetOutOfRangeConstraint(Entity source, Entity target)
    {
        var distance = Vector3.Distance(target.transform.position, source.transform.position);
        if (distance > 1)
        {
            Debug.Log("Not close enough");
            return new ConstraintInfo
            {
                Constraint = Constraints.OutOfRange,
                Value = distance,
                Subject = target,
            };
        }

        return new ConstraintInfo();
    }

    public static ConstraintInfo GetAliveConstraint(Entity target)
    {
        if (target.TryGetComponent<Health>(out var health) && health.Current > 0)
        {
            return new ConstraintInfo
            {
                Constraint = Constraints.Alive,
                Value = health.Current,
                Subject = target,
            };
        }

        return new ConstraintInfo();
    }
}

