using Sirenix.OdinInspector;

[System.Serializable]
public class ContainerDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public int Capacity;
}