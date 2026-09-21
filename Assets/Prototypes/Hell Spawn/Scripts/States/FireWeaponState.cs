using UnityEngine;

public enum FireState
{
    None,
    Discharging,
    Reloading,
    Charging,
}

public class FireWeaponState : State
{
    [SerializeField] HellSpawn.Character character;
    public WeaponConfig WeaponConfig;
    public BulletConfig BulletConfig;
    public Gun Gun;
    public float NextFireTime;
    public float NextDischargeTime;
    public float ReloadCompleteTime;
    public float ChargeCompleteTime;
    public FireState FireState;
    public int RemainingClip;
    public int RemainingDischarge;
    Unity.Mathematics.Random random;

    void Awake()
    {
        Gun = GetComponent<Gun>();
        random = new Unity.Mathematics.Random(11);
        RemainingClip = WeaponConfig.ClipSize;
    }

    public override void Enter()
    {
        base.Enter();
        if (WeaponConfig.Charges)
        {
            ChargeCompleteTime = Time.time + WeaponConfig.ChargeTime;
            FireState = FireState.Charging;
        }
        else if (RemainingClip <= 0)
        {
            RemainingDischarge = WeaponConfig.DischargeCount;
            RemainingClip = 0;
            FireState = FireState.Reloading;
        }
        else
        {
            RemainingDischarge = WeaponConfig.DischargeCount;
            FireState = FireState.Discharging;
        }
    }

    public override void Exit()
    {
        base.Exit();
        FireState = FireState.None;
    }

    void Update()
    {
        if (!Active) return;
        switch (FireState)
        {
            case FireState.None:
                break;
            case FireState.Discharging:
                HandleDischarge();
                break;
            case FireState.Reloading:
                HandleReload();
                break;
            case FireState.Charging:
                HandleCharge();
                break;
        }
    }

    void Fire()
    {
        var bulletObject = Instantiate(BulletConfig.Prefab);  
        var bullet = bulletObject.GetComponent<Bullet>(); 
        bullet.Speed = WeaponConfig.DischargeSpeed;
        bulletObject.transform.position = Gun.DischargePoint.position;
        bulletObject.transform.rotation = Quaternion.Euler(Gun.DischargePoint.rotation.eulerAngles + new Vector3(0, 0, random.NextFloat(-WeaponConfig.SpreadAngle, WeaponConfig.SpreadAngle)));

        if (WeaponConfig.Charges)
        {
            ChargeCompleteTime = Time.time + WeaponConfig.ChargeTime;
            FireState = FireState.Charging;
            return;
        }
        else
        {
            var newVelocity = character.Back * BulletConfig.AttackerKnockBackGrounded;
            character.Velocity.CurrentX = newVelocity.x;
            character.Velocity.CurrentY = newVelocity.y;
            character.Velocity.AccelerationX = character.Velocity.AccelerationX = character.Velocity.InertiaAccelerationX;

            RemainingClip--;

            if (RemainingClip <= 0)
            {
                RemainingClip = 0;
                FireState = FireState.Reloading;
                ReloadCompleteTime = Time.time + WeaponConfig.ReloadTime;

                return;
            }
        }
    }

    void HandleDischarge()
    {
        if (NextFireTime > Time.time) return;
        if (NextDischargeTime > Time.time) return;

        Fire();
        RemainingDischarge--;
        
        if (RemainingDischarge <= 0)
        {
            NextFireTime = Time.time + WeaponConfig.FireRate;
            RemainingDischarge = WeaponConfig.DischargeCount;
        }
        else NextDischargeTime = Time.time + WeaponConfig.DischargeRate;
    }

    void HandleReload()
    {
        if (ReloadCompleteTime > Time.time) return;
        RemainingClip = WeaponConfig.ClipSize;
        FireState = FireState.Discharging;
    }

    void HandleCharge()
    {
        if (ChargeCompleteTime > Time.time) return;
        Fire();
        ChargeCompleteTime = Time.time + WeaponConfig.ChargeTime;
    }
}
