using UnityEngine;

public class DashState : State
{
    public HellSpawn.Character Character;
    public Vector2 Direction;
    public float DashSpeed;
    public float DashDeceleration;
    public float DashDuration;
    public float EndDashTime;

    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
    }

    public override void Enter()
    {
        base.Enter();
        EndDashTime = Time.time + DashDuration;
        Character.Velocity.OverrideVelocity(Direction * DashSpeed);
    }
    void FixedUpdate()
    {
        if (!Active) return;
        if (EndDashTime <= Time.time) Exit();
    }

    public override void Exit()
    {
        base.Exit();
        Direction = Vector2.zero;
        Character.Velocity.CancelOverride();
        Character.Velocity.SetTargetVelocityX(0, DashDeceleration);
    }
}
