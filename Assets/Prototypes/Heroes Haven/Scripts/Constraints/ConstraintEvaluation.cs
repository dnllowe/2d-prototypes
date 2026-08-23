using System.Collections.Generic;

[System.Serializable]
public class ConstraintEvaluation
{
    public Constraints Constraints;
    public List<ConstraintInfo> Details;
    public ConstraintContext Context;
}
