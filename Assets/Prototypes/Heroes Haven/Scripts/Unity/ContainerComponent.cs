using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ContainerComponent : SerializedMonoBehaviour
{
    [NonSerialized]
    [OdinSerialize]
    public Container Container;
}

