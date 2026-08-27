using Sirenix.OdinInspector;
using UnityEngine;

public class StateMachineComponent : SerializedMonoBehaviour
{
    public WorldComponent WorldComponent;

    [SerializeField] StateMachine stateMachine;

    void Awake()
    {
        // TODO: get an ID
        stateMachine = new StateMachine(WorldComponent.World, 0);
    }

    void Update()
    {
        stateMachine.Update(Time.deltaTime);
    }

    public void ChangeState(Task task)
    {
        stateMachine.ChangeState(task);
    }
}
