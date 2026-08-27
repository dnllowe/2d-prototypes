[System.Serializable]
public class ItemCandidate
{
    public uint EntityId;
    public ItemSources Source;
    public Items Item;
    public int Quantity;

    public Resource Resource = null;
    public WorldItem WorldItem = null;
    public Container Container = null;
}