using System.Collections.Generic;

[System.Serializable]
public class Assignment
{
    public int Priority;
    public List<Task> Tasks = new List<Task>();   
}
