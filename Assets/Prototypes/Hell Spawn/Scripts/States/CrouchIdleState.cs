using UnityEngine;

public class CrouchIdleState : State
{
    public HellSpawn.Character Character;
    Vector3 originalScale;

    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
    }

    void Start()
    {
        originalScale = Character.Rb.transform.localScale;
    }

    public override void Enter()
    {
        base.Enter();
        Character.Rb.transform.localScale = Vector3.Scale(originalScale, new Vector3(1, 0.5f, 1));
        var newPosition = Character.Rb.position;
        newPosition.y -= Character.SpriteRenderer.bounds.extents.y;
        Character.Rb.MovePosition(newPosition);
    }

    public override void Exit()
    {
        base.Exit();
        Character.Rb.transform.localScale = originalScale;
        var newPosition = Character.Rb.position;
        newPosition.y += Character.SpriteRenderer.bounds.extents.y / 2;
        Character.Rb.MovePosition(newPosition);
    }
}
