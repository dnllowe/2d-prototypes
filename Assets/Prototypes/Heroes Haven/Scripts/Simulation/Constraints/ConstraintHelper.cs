using System;
using System.Collections.Generic;
using UnityEngine;

public static class ConstraintHelper
{
    static List<Constraints> _allConstraints;

    public static List<Constraints> GetAllConstraints()
    {
        if (_allConstraints == null)
        {
            _allConstraints = new List<Constraints>((Constraints[])Enum.GetValues(typeof(Constraints)));
        }

        return _allConstraints;
    }

    public static bool HasConstraint(Constraints a, Constraints b)
    {
        if (a == Constraints.None) return false;
        return (a & b) == a;
    }

    public static Constraints GetMissingTargetConstraint(Entity target)
    {
        if (target.Id == Constants.NullEntityId) return Constraints.MissingTarget;
        return Constraints.None;
    }

    public static Constraints GetMissingRequirementsConstraint(Container container, List<ItemRequirement> requirements)
    {
        var remainingRequirements = container.GetRemainingRequirements(requirements);
        if (requirements.Count > 0) Debug.Log("We're going to need a few things...");

        if (remainingRequirements.Count > 0) return Constraints.MissingRequirements;
        return Constraints.None;
    }

    public static Constraints GetTargetOutOfRangeConstraint(Entity source, Entity target)
    {
        var sourcePosition = PositionRegistry.Components.Get(source.Id);
        var targetPosition = PositionRegistry.Components.Get(target.Id);
        var distance = Position.Distance(sourcePosition, targetPosition);
        if (distance > 1) return Constraints.TargetOutOfRange;
        return Constraints.None;
    }

    public static Constraints GetAliveConstraint(Entity target)
    {
        var health = HealthRegistry.Components.Get(target.Id);
        if (health != null && health.Current > 0) return Constraints.Alive;
        return Constraints.None;
    }

    public static Constraints OutOfStorageConstraint(Container container)
    {
        if (container.GetAvailableSpace() == 0) return Constraints.OutOfStorageSpace;
        return Constraints.None;
    }
}

