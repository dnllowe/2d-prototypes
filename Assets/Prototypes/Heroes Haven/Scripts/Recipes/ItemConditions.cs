using System;

[Flags] [Serializable]
public enum ItemConditions
{
    None = 0,
    Raw = 1 << 0,
    Cooked = 1 << 1,
    Clean = 1 << 2,
    Dirty = 1 << 3, 
    Frozen = 1 << 4,
    Spoiled = 1 << 5,
    Damaged = 1 << 6,
    Broken = 1 << 7,
    Wet = 1 << 8,
}
