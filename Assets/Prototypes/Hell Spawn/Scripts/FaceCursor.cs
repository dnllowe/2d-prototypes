using UnityEngine;
using UnityEngine.InputSystem;

public class FaceCursor : MonoBehaviour
{
    public float RotationOffset;

    void Update()
    {
         // 1. Convert the object's position from World to Screen pixels
        var objectScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        // 2. Get the direction vector directly in pixel space
        var direction = new Vector2(Mouse.current.position.x.value - objectScreenPos.x, Mouse.current.position.y.value - objectScreenPos.y);
        // 3. Math remains the same, but it's now perfectly accurate
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + RotationOffset));
    }
}
