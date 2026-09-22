using UnityEngine;

public class MaterialController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Material material;


    Color originalColor;
    float originalAlpha;
    Color originalGlowColor;
    int originalGlowIntensity;
    int originalGlobalGlowIntensity;
    Color originalOutlineColor;
    float originalOutlineAlpha;
    int originalOutlineGlowIntensity;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        GetInitialValues();
    }

    void GetInitialValues()
    {
        originalColor = material.GetColor("_Color");
        originalAlpha = material.GetFloat("_Alpha");
        originalGlowColor = material.GetColor("_GlowColor");
        originalGlowIntensity = material.GetInt("_Glow");
        originalGlobalGlowIntensity = material.GetInt("_GlowGlobal");
        originalOutlineColor = material.GetColor("_AlphaOutlineColor");
        originalOutlineAlpha = material.GetFloat("_AlphaOutlineBlend");
        originalOutlineGlowIntensity = material.GetInt("_AlphaOutlineGlow");
    }

    public void SetColor(Color color)
    {
        material.SetColor("_Color", color);
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
        material.SetFloat("_Alpha", value);
    }

    public void ResetAlpha()
    {
        SetAlpha(originalAlpha);
    }

    public void SetGlowColor(Color color)
    {
        material.SetColor("_GlowColor", color);
    }

    public void ResetGlowColor()
    {
        SetGlowColor(originalGlowColor);
    }

    public void SetGlowIntensity(int value)
    {
        material.SetInt("_Glow", value);
    }

    public void ResetGlowIntensity()
    {
        SetGlowIntensity(originalGlowIntensity);
    }

    public void SetGlobalGlowIntensity(int value)
    {
        material.SetInt("_GlowGlobal", value);
    }

    public void ResetGlobalGlowIntensity()
    {
        SetGlobalGlowIntensity(originalGlobalGlowIntensity);
    }

    public void SetOutlineColor(Color color)
    {
        material.SetColor("_AlphaOutlineColor", color);
    }

    public void ResetOutlineColor()
    {
        SetOutlineColor(originalOutlineColor);
    }

    public void SetOutlineAlpha(float value)
    {
        material.SetFloat("_AlphaOutlineBlend", value);
    }

    public void ResetOutlineAlpha()
    {
        SetOutlineAlpha(originalOutlineAlpha);
    }

    public void SetOutlineGlowIntensity(int value)
    {
        material.SetInt("_AlphaOutlineGlow", value);
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
}
