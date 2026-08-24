[System.Serializable]
public class Entity
{
    public uint Id = Constants.NullEntityId;

    public void SetId(uint id)
    {
        Id = id;
    }

    public void GenerateId()
    {
        Id = IdProvider.GetNextId();
    }
}