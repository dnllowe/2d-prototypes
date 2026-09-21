using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
    public float Knockback;
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
        if (collision.gameObject.TryGetComponent<Velocity>(out var velocity)) velocity.SetInstantForce(Rb.transform.right, Knockback);
        else if (collision.gameObject.TryGetComponent<HellSpawn.Character>(out var character)) character.Velocity.SetInstantForce(Rb.transform.right, Knockback);
        if (collision.gameObject.TryGetComponent<HellSpawn.Health>(out var health)) health.Damage(Damage);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Velocity>(out var velocity)) velocity.SetInstantForce(Rb.transform.right, Knockback);
        else if (collision.gameObject.TryGetComponent<HellSpawn.Character>(out var character)) character.Velocity.SetInstantForce(Rb.transform.right, Knockback);
        if (collision.gameObject.TryGetComponent<HellSpawn.Health>(out var health)) health.Damage(Damage);
        Destroy(gameObject);
    }
}
