using System.Collections.Generic;

[System.Serializable]
public class Character : Component
{
    public CharacterDescription Description;
    public Dictionary<ItemCapabilities, int> Skills = new Dictionary<ItemCapabilities, int>();
    public int MaxHealth;
    public Character(uint entityId) : base(entityId) {}

}