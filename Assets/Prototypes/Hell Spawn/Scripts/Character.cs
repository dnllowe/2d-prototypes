using UnityEngine;

namespace HellSpawn
{
    public class Character : MonoBehaviour
    {
        public Rigidbody2D Rb;
        public Weapon EquippedWeapon;
        public SpriteRenderer SpriteRenderer;
        public Velocity Velocity;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Rb = GetComponent<Rigidbody2D>(); 
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Velocity = GetComponent<Velocity>();
        }
    }
}

