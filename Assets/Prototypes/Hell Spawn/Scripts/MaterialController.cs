using UnityEngine;

[ExecuteAlways]
public class MaterialController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Material material;
    public Color defaultColor;
    Color originalColor;
    float originalAlpha;
    Color originalGlowColor;
    int originalGlowIntensity;
    int originalGlobalGlowIntensity;
    Color originalOutlineColor;
    float originalOutlineAlpha;
    int originalOutlineGlowIntensity;
    static MaterialPropertyBlock sharedPropertyBlock;

    void Awake()
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        EnsureMaterial();
        GetInitialValues();
    }

    void OnValidate()
    {
        EnsureMaterial();
        SetColor(defaultColor);
    }

    public void EnsureMaterial()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
    }

    void GetInitialValues()
    {
        ReservePropertyBlock();
        originalColor = sharedPropertyBlock.GetColor("_Color");
        originalAlpha = sharedPropertyBlock.GetFloat("_Alpha");
        originalGlowColor = sharedPropertyBlock.GetColor("_GlowColor");
        originalGlowIntensity = sharedPropertyBlock.GetInt("_Glow");
        originalGlobalGlowIntensity = sharedPropertyBlock.GetInt("_GlowGlobal");
        originalOutlineColor = sharedPropertyBlock.GetColor("_OutlineColor");
        originalOutlineAlpha = sharedPropertyBlock.GetFloat("_OutlineAlpha");
        originalOutlineGlowIntensity = sharedPropertyBlock.GetInt("_OutlineGlow");
        SetPropertyBlock();
    }

    public void SetColor(Color color)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetColor("_Color", color);
        SetPropertyBlock();
    }

    public void TweenColor(Color color, float completion)
    {
        var tweenedR = (color.r * completion * color.a) + originalColor.r;
        var tweenedG = (color.g * completion * color.a) + originalColor.g;
        var tweenedB = (color.b * completion * color.a) + originalColor.b;
        var tweenedColor = new Color(tweenedR, tweenedG, tweenedB);
        
        SetColor(tweenedColor);
    }

    public void ResetColor()
    {
        SetColor(originalColor);
    }

    public void SetAlpha(float value)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetFloat("_Alpha", value);
        SetPropertyBlock();
    }

    public void ResetAlpha()
    {
        SetAlpha(originalAlpha);
    }

    public void SetGlowColor(Color color)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetColor("_GlowColor", color);
        SetPropertyBlock();
    }

    public void ResetGlowColor()
    {
        SetGlowColor(originalGlowColor);
    }

    public void SetGlowIntensity(int value)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetInt("_Glow", value);
        SetPropertyBlock();
    }

    public void ResetGlowIntensity()
    {
        SetGlowIntensity(originalGlowIntensity);
    }

    public void SetGlobalGlowIntensity(int value)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetInt("_GlowGlobal", value);
        SetPropertyBlock();
    }

    public void ResetGlobalGlowIntensity()
    {
        SetGlobalGlowIntensity(originalGlobalGlowIntensity);
    }

    public void SetOutlineColor(Color color)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetColor("_OutlineColor", color);
        SetPropertyBlock();
    }

    public void ResetOutlineColor()
    {
        SetOutlineColor(originalOutlineColor);
    }

    public void SetOutlineAlpha(float value)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetFloat("_OutlineAlpha", value);
        SetPropertyBlock();
    }

    public void ResetOutlineAlpha()
    {
        SetOutlineAlpha(originalOutlineAlpha);
    }

    public void SetOutlineGlowIntensity(int value)
    {
        ReservePropertyBlock();
        sharedPropertyBlock.SetInt("_OutlineGlow", value);
        SetPropertyBlock();
    }

    public void ResetOutlineGlowIntensity()
    {
        SetOutlineGlowIntensity(originalOutlineGlowIntensity);
    }

    public void Reset()
    {
        ResetColor();
        ResetAlpha();
        ResetGlowColor();
        ResetGlowIntensity();
        ResetGlobalGlowIntensity();
        ResetOutlineColor();
        ResetOutlineAlpha();
    }

    void ReservePropertyBlock()
    {
        sharedPropertyBlock.Clear();
        spriteRenderer.GetPropertyBlock(sharedPropertyBlock);
    }

    void SetPropertyBlock()
    {
        spriteRenderer.SetPropertyBlock(sharedPropertyBlock);
    }
}
