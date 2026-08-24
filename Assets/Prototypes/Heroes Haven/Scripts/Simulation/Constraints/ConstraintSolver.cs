using System.Collections.Generic;

public static class ConstraintSolver
{
    public static Actions GetSolution(Constraints constraint)
    {
        try
        {
            return Solutions[constraint][0];
        }
        catch
        {
            return Actions.None;
        }
    }

    public static Dictionary<Constraints, List<Actions>> Solutions = new Dictionary<Constraints, List<Actions>>
    {
        { Constraints.Alive, new List<Actions>{ Actions.Attack } },
        { Constraints.Dead, new List<Actions>{ Actions.Heal } },
        { Constraints.Wounded, new List<Actions>{ Actions.Heal } },
        { Constraints.Broken, new List<Actions>{ Actions.Repair } },
        { Constraints.TargetOutOfRange, new List<Actions>{ Actions.GoTo } },
        { Constraints.MissingRequirements, new List<Actions>{ Actions.Gather }},
        { Constraints.MissingTarget, new List<Actions>{ Actions.Find }},
        { Constraints.NotFound, new List<Actions>{ Actions.Craft, Actions.Trade }},
        { Constraints.Dangerous, new List<Actions>{ Actions.GoHome }},
        { Constraints.OutOfStorageSpace, new List<Actions>{ Actions.Deliver }},
    };
}