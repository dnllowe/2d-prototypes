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
    Dead = 1 << 6,
    Raw = 1 << 7,
    Dirty = 1 << 8,
    Spoiled = 1 << 9,
    Wounded = 1 << 10,
    Broken = 1 << 11,
    NeedsSharpEdge = 1 << 12,
    MissingRequirements = 1 << 13,
    TargetOutOfRange = 1 << 14,
    MissingTarget = 1 << 16,
    NotFound = 1 << 17,
    OutOfStorageSpace = 1 << 18,
}