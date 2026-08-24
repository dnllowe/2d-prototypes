using Sirenix.OdinInspector;

[System.Serializable]
public class ItemDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public ItemProperties Properties;
}