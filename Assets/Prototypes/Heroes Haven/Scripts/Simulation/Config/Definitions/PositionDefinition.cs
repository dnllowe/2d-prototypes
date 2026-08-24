using Sirenix.OdinInspector;

[System.Serializable]
public class PositionDefinition
{
    public bool Assigned;

    [ShowIf(nameof(Assigned))]
    public float StartX;
    [ShowIf(nameof(Assigned))]
    public float StartY;
    [ShowIf(nameof(Assigned))]
    public float StartZ;
}