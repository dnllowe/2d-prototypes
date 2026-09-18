using UnityEngine;

public class MoveState : State
{
    public int Direction;
    public float TopSpeed;
    public float Acceleration;
    public float Deceleration;
    // public float CurrentSpeed;
    public HellSpawn.Character Character;

    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
    }

    public override void Enter()
    {
        base.Enter();
        // CurrentSpeed = 0;
        // Character.Velocity.Current.x = 0;
        Character.Velocity.SetTargetVelocityX(TopSpeed * Direction, Acceleration);
    }

    // void Update()
    // {
        // if (!Active) return;

        // CurrentSpeed += Acceleration * Time.deltaTime;
        // CurrentSpeed = Mathf.Min(TopSpeed, CurrentSpeed);
        // Character.Velocity.Current.x  = CurrentSpeed * Direction;
    // }

    public override void Exit()
    {
        base.Exit();
        // CurrentSpeed = 0;
        // Character.Velocity.Current.x = 0;
        Character.Velocity.SetTargetVelocityX(0, Deceleration);
    }
}
