using UnityEngine;

[System.Serializable]
public class Instruction
{
    public Instructions Type;
    public InstructionSources Source;
    public Items Item;
    public Entity Target;
    public Vector3 Destination;
}