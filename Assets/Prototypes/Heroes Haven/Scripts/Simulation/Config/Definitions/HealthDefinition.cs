using Sirenix.OdinInspector;

[System.Serializable]
public class HealthDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public int MaxHealth;
}