[System.Flags]
public enum ItemCapabilities
{
    None = 0,
    DealsDamage = 1 << 0,
    Protects = 1 << 1,
    ProvidesHeat = 1 << 2,
    ProvidesShelter = 1 << 3,
    Cleans = 1 << 4,
    Dirties = 1 << 5,
    Cuts = 1 << 6,
    Burns = 1 << 7,
    Extinguishes = 1 << 8,
    Freezes = 1 << 9,
    ProvidesSurface = 1 << 10,
    ProvidesMoisture = 1 << 11,
    ProvidesFuel = 1 << 12,
    ProvidesFood = 1 << 13,
    QuenchesThirst = 1 << 14,
    Heals = 1 << 15,
}