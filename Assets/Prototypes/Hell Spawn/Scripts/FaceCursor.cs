using UnityEngine;
using UnityEngine.InputSystem;

public class FaceCursor : MonoBehaviour
{
    public float RotationOffset;

    void Update()
    {
        // var mousePosition = Mouse.current.position.value;
        // var direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        // var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + RotationOffset));

         // 1. Convert the object's position from World to Screen pixels
        Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        // 2. Get the direction vector directly in pixel space
        Vector2 direction = new Vector2(Mouse.current.position.x.value - objectScreenPos.x, Mouse.current.position.y.value - objectScreenPos.y);
        
        // 3. Math remains the same, but it's now perfectly accurate
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + RotationOffset));
    }
}
