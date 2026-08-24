using UnityEngine;
using TMPro;

public class TextBoxUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Canvas canvas;

    void Awake()
    {
        SetText(string.Empty);
        Hide();
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void Show()
    {
        canvas.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
