using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    public Color HighlightColor;
    MaterialController materialController;
    Cursor cursor;

    void Awake()
    {
        cursor = FindAnyObjectByType<Cursor>();
        materialController = GetComponent<MaterialController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.GetObjectsUnderCursor().Contains(gameObject))
        {
            materialController.SetOutlineColor(HighlightColor);
            materialController.SetOutlineAlpha(1);
        }
        else
        {
            materialController.ResetOutlineAlpha();
            materialController.ResetOutlineColor();
        }
    }
}
