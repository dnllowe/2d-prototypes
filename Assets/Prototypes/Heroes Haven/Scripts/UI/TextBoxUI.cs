using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class TextBoxUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Canvas canvas;

    void Awake()
    {
        SetText(string.Empty);
        Hide();
    }

    [Button]
    public void SetText(string newText)
    {
        text.text = newText;
    }

    [Button]
    public void Show()
    {
        canvas.enabled = true;
    }

    [Button]
    public void Hide()
    {
        canvas.enabled = false;
    }
}
