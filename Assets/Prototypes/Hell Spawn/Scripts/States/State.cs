using UnityEngine;

public class State : MonoBehaviour
{
    public bool Active;
    public virtual void Enter()
    {
        Active = true;
    }

    public virtual void Exit()
    {
        Active = false;
    }
}
