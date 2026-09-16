using UnityEngine;

public class FireWeaponState : State
{
    public WeaponConfig WeaponConfig;
    public BulletConfig BulletConfig;
    public Gun Gun;
    public float NextFireTime;

    void Awake()
    {
        Gun = GetComponent<Gun>();
    }

    void Update()
    {
        if (!Active) return;
        if (NextFireTime > Time.time) return;

        Fire();
    }

    void Fire()
    {
        var bulletObject = Instantiate(BulletConfig.Prefab);  
        var bullet = bulletObject.GetComponent<Bullet>(); 
        bullet.Speed = WeaponConfig.DischargeSpeed;
        bulletObject.transform.position = Gun.DischargePoint.position;
        bulletObject.transform.rotation = Gun.DischargePoint.rotation;
        NextFireTime = Time.time + WeaponConfig.FireRate;
    }
}
