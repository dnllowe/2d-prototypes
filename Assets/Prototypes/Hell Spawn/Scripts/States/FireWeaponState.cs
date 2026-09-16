using UnityEngine;

public class FireWeaponState : State
{
    public WeaponConfig WeaponConfig;
    public BulletConfig BulletConfig;
    public Gun Gun;
    public float NextFireTime;
    Unity.Mathematics.Random random;

    void Awake()
    {
        Gun = GetComponent<Gun>();
        random = new Unity.Mathematics.Random(11);
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
        bulletObject.transform.rotation = Quaternion.Euler(Gun.DischargePoint.rotation.eulerAngles + new Vector3(0, 0, random.NextFloat(-WeaponConfig.SpreadAngle, WeaponConfig.SpreadAngle)));
        NextFireTime = Time.time + WeaponConfig.FireRate;
    }
}
