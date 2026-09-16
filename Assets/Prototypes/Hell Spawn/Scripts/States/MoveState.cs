using UnityEngine;

public class MoveState : State
{
    public int Direction;
    public float TopSpeed;
    public float Acceleration;
    public float CurrentSpeed;
    public HellSpawn.Character Character;

    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
    }

    public override void Enter()
    {
        base.Enter();
        Character.Velocity.Current.x = TopSpeed * Direction;
    }

    public override void Exit()
    {
        base.Exit();
        Character.Velocity.Current.x = 0;
    }
}
