using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour
{
    public float TargetX;
    public float CurrentX;
    public float AccelerationX;
    public float InertiaAccelerationX;
    public float OverrideX;
    public float TargetY;
    public float CurrentY;
    public float AccelerationY;
    public float DecelerationX;
    public float DecelerationY;
    public float OverrideY;
    public float UseOverrideX;
    public float UseOverrideY;
    public Vector2 Override;
    public bool UseOverride;
    public float GravityScale = 1;
    public float CollisionBuffer = 0.01f;
    Rigidbody2D rb;
    public bool Grounded;
    RaycastHit2D[] collisionHits = new RaycastHit2D[8];
    public LayerMask CollisionLayers;
    public ContactFilter2D ContactFilter;
    BoxCollider2D boxCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Grounded = GroundCheck();
    }

    void FixedUpdate()
    {
        Grounded = GroundCheck();
        UpdateVelocityX(Time.fixedDeltaTime);
        var actual = UseOverride ? Override : new Vector2(CurrentX, CurrentY);
        if (!Grounded && !UseOverride) CurrentY -= 9.81f * GravityScale * Time.fixedDeltaTime;

        var x = GetMoveHorizontal(actual.x * Time.fixedDeltaTime);
        var y = GetMoveVertical(actual.y * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + new Vector2(x, y));
    }

    public void SetTargetVelocityX(float velocity, float acceleration)
    {
        TargetX = velocity;
        AccelerationX = acceleration;
    }

    void UpdateVelocityX(float time)
    {
        CurrentX = Mathf.Lerp(CurrentX, TargetX, time * AccelerationX);
    }

    public void OverrideVelocity(Vector2 velocity)
    {
        UseOverride = true;
        OverrideX = velocity.x;
        OverrideY = velocity.y;
        Override = velocity;
    }

    public void CancelOverride()
    {
        CurrentX = OverrideX;
        CurrentY = OverrideY;
        UseOverride = false;
        Override = Vector2.zero;
        OverrideX = 0;
        OverrideY = 0;
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
            ContactFilter,
            collisionHits,
            absoluteDistance + CollisionBuffer
        );

        // TODO: remove later
        // Safety check for development
        if (hitCount >= collisionHits.Length)
        {
            Debug.LogWarning($"Hit buffer full ({hitCount}/{collisionHits.Length})! You might be missing colliders.");
        }
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
            return 0;
        }

        var direction = distance > 0
            ? Vector2.up
            : Vector2.down;

        var absoluteDistance = Mathf.Abs(distance);

        var hitCount = rb.Cast(
            direction,
            ContactFilter,
            collisionHits,
            absoluteDistance + CollisionBuffer
        );
        // TODO: remove later
        // Safety check for development
        if (hitCount >= collisionHits.Length)
        {
            Debug.LogWarning($"Hit buffer full ({hitCount}/{collisionHits.Length})! You might be missing colliders.");
        }

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

        if (blocked) CurrentY = 0;
        if (blocked) Override.y = 0;

        return direction.y * allowedDistance;
    }

    bool GroundCheck()
    {
        var hits = Physics2D.RaycastNonAlloc(transform.position, Vector2.down, collisionHits, boxCollider.bounds.extents.y + CollisionBuffer, CollisionLayers);
        return hits > 0;
    }
}
