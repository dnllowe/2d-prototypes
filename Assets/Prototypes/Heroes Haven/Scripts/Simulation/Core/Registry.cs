using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;

public class Registry<T> where T : Component
{
    public Dictionary<uint, T> Entries = new Dictionary<uint, T>();
    public void Register(T component, uint id)
    {
        Entries[id] = component;     
    }

    public void Unregister(uint id)
    {
        Entries.Remove(id);
    }

    public T Get(uint id)
    {
        Entries.TryGetValue(id, out var component);
        return component;
    }
}

[AutoStaticsCleanup] public static partial class HealthRegistry { public static Registry<Health> Components; }
[AutoStaticsCleanup] public static partial class ContainerRegistry { public static Registry<Container> Components = new Registry<Container>(); }
[AutoStaticsCleanup] public static partial class PositionRegistry { public static Registry<Position> Components = new Registry<Position>(); }
[AutoStaticsCleanup] public static partial class ResourceRegistry { public static Registry<Resource> Components = new Registry<Resource>(); }
[AutoStaticsCleanup] public static partial class WorldItemRegistry { public static Registry<WorldItem> Components = new Registry<WorldItem>(); }