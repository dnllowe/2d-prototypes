using Sirenix.OdinInspector;
using UnityEngine;

public class StateMachineComponent : SerializedMonoBehaviour
{
    public CurrentConfig Config;
    [SerializeField] StateMachine stateMachine = new StateMachine();

    void Update()
    {
        stateMachine.Update(Time.deltaTime);
    }

    public void ChangeState(Task task)
    {
        stateMachine.ChangeState(task);
    }
}
