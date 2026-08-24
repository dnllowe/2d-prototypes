using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class WorldItemComponent : SerializedMonoBehaviour
{
    [NonSerialized]
    [OdinSerialize]
    public WorldItem WorldItem;
}
