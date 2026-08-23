using System.Collections.Generic;

public static class ConstraintSolver
{
    public static Actions GetSolution(Constraints constraint, ConstraintContext context)
    {
        return Solutions[constraint][0];
    }

    public static Dictionary<Constraints, List<Actions>> Solutions = new Dictionary<Constraints, List<Actions>>
    {
        { Constraints.Alive, new List<Actions>{ Actions.Attack } },
        { Constraints.Dead, new List<Actions>{ Actions.Heal } },
        { Constraints.Wounded, new List<Actions>{ Actions.Heal } },
        { Constraints.Broken, new List<Actions>{ Actions.Repair } },
        { Constraints.OutOfRange, new List<Actions>{ Actions.GoTo } },
    };
}