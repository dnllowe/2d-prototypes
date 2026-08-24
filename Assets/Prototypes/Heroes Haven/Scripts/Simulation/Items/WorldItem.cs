[System.Serializable]
public class WorldItem : Component
{
    public WorldItem(uint entityId) : base(entityId)
    {
        WorldItemRegistry.Components.Register(this, entityId);
    }

    public ItemProperties Properties = new ItemProperties();
}