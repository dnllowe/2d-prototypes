using UnityEngine;
using UnityEngine.InputSystem;

namespace HellSpawn
{
    public class Character : MonoBehaviour
    {
        public Rigidbody2D Rb;
        public Weapon EquippedWeapon;
        public SpriteRenderer SpriteRenderer;
        public Velocity Velocity;
        public Vector2 Forward 
        {
            get 
            {
                var mousePosition = Mouse.current.position.value;
                var objectScreenPos = Camera.main.WorldToScreenPoint(Rb.position);
                var difference = new Vector2(mousePosition.x - objectScreenPos.x, mousePosition.y - objectScreenPos.y);
                return difference.normalized;
            }
        }

        public Vector2 Back => -1 * Forward;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Rb = GetComponent<Rigidbody2D>(); 
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Velocity = GetComponent<Velocity>();
        }

        public int GetFacingDirection()
        {
            var objectScreenPos = Camera.main.WorldToScreenPoint(Rb.position);
            var mouseX = Mouse.current.position.x.value;

            if (objectScreenPos.x > mouseX) return -1;
            else return 1;
        }
    }
}

