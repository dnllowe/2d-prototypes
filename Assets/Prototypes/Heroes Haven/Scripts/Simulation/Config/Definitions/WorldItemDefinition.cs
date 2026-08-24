using Sirenix.OdinInspector;

[System.Serializable]
public class WorldItemDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public Items Item;
}