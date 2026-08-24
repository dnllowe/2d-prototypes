using Unity.Scripting.LifecycleManagement;

[AutoStaticsCleanup]
public static partial class IdProvider
{
    [AutoStaticsCleanup]
    static uint nextId = 0;

    public static uint GetNextId()
    {
        var id = nextId;
        nextId++;

        return id;
    }
}