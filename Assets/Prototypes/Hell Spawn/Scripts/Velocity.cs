using UnityEngine;

public class Velocity : MonoBehaviour
{
    public Vector2 Current;
    public float GravityScale = 1;
    public float CollisionBuffer = 0.01f;
    Rigidbody2D rb;
    public bool Grounded;
    RaycastHit2D[] collisionHits = new RaycastHit2D[8];

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Grounded = false;
    }

    void FixedUpdate()
    {
        if (!Grounded) Current.y -= 9.81f * GravityScale * Time.fixedDeltaTime;

        var x = GetMoveHorizontal(Current.x * Time.fixedDeltaTime);
        var y = GetMoveVertical(Current.y * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + new Vector2(x, y));
    }

    float GetMoveHorizontal(float distance)
    {
        if (Mathf.Approximately(distance, 0)) return 0;

        var direction = distance > 0
            ? Vector2.right
            : Vector2.left;

        var absoluteDistance = Mathf.Abs(distance);

        var hitCount = rb.Cast(
            direction,
            collisionHits,
            absoluteDistance + CollisionBuffer
        );

        var allowedDistance = absoluteDistance;

        for (int i = 0; i < hitCount; i++)
        {
            var hit = collisionHits[i];

            // Ignore surfaces that don't oppose horizontal movement.
            if (Vector2.Dot(hit.normal, direction) >= 0) continue;

            var distanceToSurface = Mathf.Max(0, hit.distance - CollisionBuffer);

            if (distanceToSurface < allowedDistance) allowedDistance = distanceToSurface;
        }

        return direction.x * allowedDistance;
    }

    float GetMoveVertical(float distance)
    {
        if (Mathf.Approximately(distance, 0))
        {
            Grounded = false;
            return 0;
        }

        var direction = distance > 0
            ? Vector2.up
            : Vector2.down;

        var absoluteDistance = Mathf.Abs(distance);

        var hitCount = rb.Cast(
            direction,
            collisionHits,
            absoluteDistance + CollisionBuffer
        );

        var allowedDistance = absoluteDistance;
        var blocked = false;

        for (int i = 0; i < hitCount; i++)
        {
            var hit = collisionHits[i];

            // Ignore surfaces that don't oppose vertical movement.
            if (Vector2.Dot(hit.normal, direction) >= 0)
                continue;

            var distanceToSurface = Mathf.Max(0, hit.distance - CollisionBuffer);

            if (distanceToSurface < allowedDistance)
            {
                allowedDistance = distanceToSurface;
                blocked = true;
            }
        }

        if (blocked) Current.y = 0;

        Grounded = direction == Vector2.down && blocked;

        return direction.y * allowedDistance;
    }

}
