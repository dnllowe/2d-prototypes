using UnityEngine;
using Sirenix.OdinInspector;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] Animation animation;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int frame;
    [SerializeField] float fpsMultiplier = 1;
    [SerializeField] float resumedFpsMultiplier = 1;
    [SerializeField] float frameTime;
    [SerializeField] bool paused;


    public void Update()
    {
        frameTime += fpsMultiplier * Time.deltaTime;
    }

    public void UpdateAnimation()
    {
        if (frameTime > animation.FrameRate)
        {
            frame++;
            frameTime = frameTime % animation.FrameRate;
        }
        if (frame >= animation.Frames.Count - 1)
        {
            frame = 0;
        }

        spriteRenderer.sprite = animation.Frames[frame];
    }

    [Button]
    public void SetAnimation(Animation newAnimation)
    {
        frame = 0;
        animation = newAnimation;
    }

    [Button]
    public void Pause()
    {
        resumedFpsMultiplier = fpsMultiplier;
        fpsMultiplier = 0;
    }

    [Button]
    public void Resume()
    {
        fpsMultiplier = resumedFpsMultiplier;
        resumedFpsMultiplier = 1;
    }

    [Button]
    public void SetFpsMultiplier(float multiplier)
    {
        fpsMultiplier = multiplier;
    }
}