using UnityEngine;
using System;

[Serializable]
public class BulletConfig
{
    public GameObject Prefab;
    public int Damage;    
    public float AttackerKnockBackGrounded;
    public float AttackerKnockBackAirborne;
    public float TargetKnockBackGrounded;
    public float TargetKnockBackAirborne;
}
