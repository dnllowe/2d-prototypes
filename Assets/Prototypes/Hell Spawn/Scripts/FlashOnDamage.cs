using UnityEngine;

public class FlashOnDamage : MonoBehaviour
{
    MaterialController materialController;
    HellSpawn.Health health;
    public Color FlashColor;
    public float FlashDuration;
    public float Remaining;
    bool isActive;

    void Awake()
    {
        materialController = GetComponent<MaterialController>(); 
        health = GetComponent<HellSpawn.Health>();
        health.OnDamaged += BeginFlash;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (Remaining > 0) Remaining -= Time.deltaTime;
        if (Remaining < 0) Remaining = 0;
        UpdateFlashAmount();

        if (Remaining == 0 && isActive) EndFlash();
    }

    public void BeginFlash(int _damageAmount)
    {
        isActive = true;
        Remaining = FlashDuration;
        materialController.TweenColor(FlashColor, 1);
    }

    public void UpdateFlashAmount()
    {
        materialController.TweenColor(FlashColor, GetCompletion());
    }

    public void EndFlash()
    {
        isActive = false;
        materialController.ResetColor();
    }

    float GetCompletion()
    {
        return Remaining / FlashDuration;
    }
}
