public class IdProvider
{
    uint nextId = 0;

    public uint GetNextId()
    {
        var id = nextId;
        nextId++;

        return id;
    }
}