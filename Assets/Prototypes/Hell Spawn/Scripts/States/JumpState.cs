using UnityEngine;

public class JumpState : State
{
    public HellSpawn.Character Character;
    public float JumpForce;
    public int TotalJumps = 2;
    public int RemainingJumps = 2;


    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
    }

    public override void Enter()
    {
        base.Enter();
        if (RemainingJumps <= 0) return;
        Character.Velocity.CurrentY = JumpForce;
        RemainingJumps--;
    }

    public override void Exit()
    {
        base.Exit();
        RemainingJumps = TotalJumps;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Exit(); 
    }
}
