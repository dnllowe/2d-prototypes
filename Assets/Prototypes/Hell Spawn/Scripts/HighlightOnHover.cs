using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    MaterialController materialController;
    public Color HighlightColor;

    void Awake()
    {
        materialController = GetComponent<MaterialController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
