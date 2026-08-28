using System.Collections.Generic;

[System.Serializable]
public class WorldState
{
    public List<Container> Containers = new List<Container>();
    public List<Health> Health = new List<Health>();
    public List<Resource> Resources = new List<Resource>();
    public List<Position> Positions = new List<Position>();
    public List<WorldItem> WorldItems = new List<WorldItem>();
    public List<uint> Entities = new List<uint>();
    public List<StateMachine> StateMachines = new List<StateMachine>();
}