[System.Serializable]
public class ConstraintInfo
{
    /// <summary>
    /// Should just be a single flag here so it maps to the exact value
    /// </summary>
    public Constraints Constraint;
    public float Value;
    public Entity Subject;
}