using System.Collections.Generic;

[System.Serializable]
public class Task
{
    /// <summary>
    /// What to do
    /// </summary>
    public Actions Action; 
    /// <summary>
    /// Requirements for the task's item
    /// </summary>
    public ItemRequirement Item;
    /// <summary>
    /// The target
    /// </summary>
    public uint Target;
    /// <summary>
    /// Targets identified to be blocked and therefore ignored
    /// </summary>
    public HashSet<Entity> ExcludedTargets = new HashSet<Entity>();
    // public ConstraintContext Constraint;
    /// <summary>
    /// High level understanding of the root task
    /// </summary>
    public TaskContext TaskContext;

    public void ApplyStateResult(StateResult result)
    {
        if (result.HasTarget) Target = result.Target;
        if (result.HasExcludedTargets)
        {
            foreach (var exclusion in result.ExcludedTargets)
            {
                ExcludedTargets.Add(exclusion);
            }
        }
    }
}
