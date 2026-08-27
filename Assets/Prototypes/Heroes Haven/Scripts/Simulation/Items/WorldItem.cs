[System.Serializable]
public class WorldItem : Component
{
    public WorldItem(uint entityId) : base(entityId) {}
    public ItemProperties Properties = new ItemProperties();
}