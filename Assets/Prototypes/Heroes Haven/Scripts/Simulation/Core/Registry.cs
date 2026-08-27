using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;

public class ComponentRegistry<T> where T : Component
{
    public Dictionary<uint, T> Components = new Dictionary<uint, T>();
    public void Register(T component, uint id)
    {
        Components[id] = component;     
    }

    public void Unregister(uint id)
    {
        Components.Remove(id);
    }

    public T Get(uint id)
    {
        Components.TryGetValue(id, out var component);
        return component;
    }
}