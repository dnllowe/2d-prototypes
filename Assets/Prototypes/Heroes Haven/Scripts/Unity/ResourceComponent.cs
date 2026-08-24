using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ResourceComponent : SerializedMonoBehaviour
{
    [NonSerialized]
    [OdinSerialize]
    public Resource Resource;
}
