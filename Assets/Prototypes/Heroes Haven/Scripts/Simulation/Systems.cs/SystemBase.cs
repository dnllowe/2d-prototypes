public class SystemBase
{
    public float TickRate = 0.1f;
    public float NextTick;

    public void Tick(float deltaTime)
    {
        NextTick -= deltaTime;
    }
}