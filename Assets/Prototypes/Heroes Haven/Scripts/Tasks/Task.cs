using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    /// <summary>
    /// What to do
    /// </summary>
    public Actions Action; 
    /// <summary>
    /// Item for task if relevant
    /// </summary>
    public Items Item;
    /// <summary>
    /// The target
    /// </summary>
    public Entity Target;
    /// <summary>
    /// The specific location if no target
    /// </summary>
    public Vector3 Destination;
    /// <summary>
    /// How much
    /// </summary>
    public int Value;
    /// <summary>
    /// Targets identified to be blocked and therefore ignored
    /// </summary>
    public HashSet<Entity> ExcludedTargets = new HashSet<Entity>();
    public ConstraintContext Constraint;
    /// <summary>
    /// High level understanding of the root task
    /// </summary>
    public TaskContext TaskContext;

    public void ApplyStateResult(StateResult result)
    {
        if (result.HasDestination) Destination = result.Destination;
        if (result.HasTarget) Target = result.Target;
        if (result.HasItem) Item = result.Item;
        if (result.HasExcludedTargets)
        {
            foreach (var exclusion in result.ExcludedTargets)
            {
                ExcludedTargets.Add(exclusion);
            }
        }

    }
}
