using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public Rigidbody2D Rb;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Rb.MovePosition(Rb.position + (new Vector2(Rb.transform.right.x, Rb.transform.right.y) * Speed * Time.fixedDeltaTime));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<HellSpawn.Health>(out var health)) health.Damage(1);
        Destroy(gameObject);
    }
}
