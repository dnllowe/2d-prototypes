using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour
{
    [SerializeField] Vector2 screenPosition;
    [SerializeField] Vector3 worldPosition;
    [SerializeField] List<GameObject> objectsUnderCursor = new List<GameObject>();
    RaycastHit2D[] hits = new RaycastHit2D[8];
    [SerializeField] LayerMask selectableLayers;

    void Update()
    {
        UpdateObjectsUnderCursor();
    }

    public Vector2 GetScreenPosition()
    {
        return screenPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return worldPosition;
    }

    public List<GameObject> GetObjectsUnderCursor()
    {
        return objectsUnderCursor;
    }

    void UpdateObjectsUnderCursor()
    {
        objectsUnderCursor.Clear();

        screenPosition = Mouse.current.position.value;        
        worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        var hitCount = Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, hits, 1, selectableLayers);

        for (var i = 0; i < hitCount; i++)
        {
            objectsUnderCursor.Add(hits[i].collider.gameObject);
        }
    }
}
