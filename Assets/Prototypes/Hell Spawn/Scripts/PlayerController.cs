using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public HellSpawn.Character Character;
    public CrouchIdleState CrouchIdleState;
    public JumpState JumpState;
    public MoveState MoveState;
    public DashState DashState;

    void Awake()
    {
        Character = GetComponent<HellSpawn.Character>();
        CrouchIdleState = GetComponent<CrouchIdleState>();
        JumpState = GetComponent<JumpState>();
        MoveState = GetComponent<MoveState>();
        DashState = GetComponent<DashState>();
    }
    
    void Update()
    {
        // Weapon states
        if (Mouse.current.leftButton.wasPressedThisFrame) Character.EquippedWeapon.Use();
        if (Mouse.current.leftButton.wasReleasedThisFrame) Character.EquippedWeapon.EndUse();
        
        // Locomotion states
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            MoveState.Exit();
            if (Keyboard.current.dKey.isPressed) DashState.Direction = Vector2.right;
            else if (Keyboard.current.aKey.isPressed) DashState.Direction = Vector2.left;
            else DashState.Direction = Vector2.right * Character.GetFacingDirection();
            DashState.Enter();
        }
        if (Keyboard.current.sKey.wasPressedThisFrame) CrouchIdleState.Enter();
        if (Keyboard.current.sKey.wasReleasedThisFrame) CrouchIdleState.Exit();
        if (Keyboard.current.spaceKey.wasPressedThisFrame) JumpState.Enter();
        if (Keyboard.current.aKey.wasReleasedThisFrame)
        {
            if (Keyboard.current.dKey.isPressed)
            {
                MoveState.Direction = 1;
                MoveState.Enter();
            }
            else
            {
                MoveState.Exit();
            }
        }
        if (Keyboard.current.dKey.wasReleasedThisFrame)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                MoveState.Direction = -1;
                MoveState.Enter();
            }
            else
            {
                MoveState.Exit();
            }
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            MoveState.Direction = -1;
            MoveState.Enter();
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            MoveState.Direction = 1;
            MoveState.Enter();
        }
        if (!DashState.Active && !MoveState.Active && Keyboard.current.aKey.isPressed)
        {
            MoveState.Direction = -1;
            MoveState.Enter();
        }
        if (!DashState.Active && !MoveState.Active && Keyboard.current.dKey.isPressed)
        {
            MoveState.Direction = 1;
            MoveState.Enter();
        }
    }
}
