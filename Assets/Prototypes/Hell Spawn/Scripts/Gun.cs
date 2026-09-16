using UnityEngine;

public class Gun : Weapon
{
    public Transform DischargePoint;
    public FireWeaponState FireWeaponState;

    void Awake()
    {
        FireWeaponState = GetComponent<FireWeaponState>();
    }

    public override void Use()
    {
        FireWeaponState.Enter();
    }

    public override void EndUse()
    {
        FireWeaponState.Exit();
    }
}
