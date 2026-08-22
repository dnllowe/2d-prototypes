using System;

[Flags] [Serializable]
public enum Constraints
{
    None = 0,
    OwnedByOther = 1 << 0,
    Alive = 1 << 1, 
    Locked = 1 << 2,
    Unreachable = 1 << 3,
    Dangerous = 1 << 4,
    WillMissDeadline = 1 << 5,
}
