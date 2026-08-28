using System;

[System.Serializable]
public class Health : Component
{
    public int Current;
    public int Total;
    public bool DestroyOnDepleted = false;
    public Action<bool> OnDepleted;
    public Action<int> OnDamaged;
    public Action<int> OnHealed;

    public Health(uint entityId) : base(entityId)
    {
        Current = Total;
    }

    public void Damage(int amount)
    {
        Current -= amount;
        OnDamaged?.Invoke(amount);

        if (Current > 0) return;
        OnDepleted?.Invoke(DestroyOnDepleted);

        // if (DestroyOnDepleted) Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        Current = System.Math.Min(Current + amount, Total);
        OnHealed?.Invoke(amount);
    }
}
