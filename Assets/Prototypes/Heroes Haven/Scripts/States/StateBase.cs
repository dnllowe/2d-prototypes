using Sirenix.OdinInspector;

[System.Serializable]
public class StateBase
{
    public Task Task;
    public Entity Entity;
    public Animation Animation;
    public float RemainingTime;

    public StateBase(Entity entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Returns true if state is complete
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public virtual StateResult Update(float deltaTime)
    {
        return StateResult.None;
    }

    [Button]
    public virtual void Enter() {}

    [Button]
    public virtual void Exit() {}
}
