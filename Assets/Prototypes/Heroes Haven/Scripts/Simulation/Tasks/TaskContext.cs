using UnityEngine;

[System.Serializable]
public class TaskContext
{
    public Actions RootAction;
    public EntityComponent RootTarget; 
    public Vector3 RootDestination;
}
