using System;
using Microsoft.Xna.Framework;

namespace finalForGP2;

public class ScreenShake
{
    private float duration  = 0f;
    private float intensity = 0f;
    private Random rng = new Random();

    public bool IsShaking => duration > 0f;

    public void Stop()
    {
        duration  = 0f;
        intensity = 0f;
    }

    public void Trigger(float duration, float intensity)
    {
        this.duration  = duration;
        this.intensity = intensity;
    }

    public void Update(GameTime gameTime)
    {
        if (duration <= 0f) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        duration -= deltaTime;

        if (duration < 0f)
            duration = 0f;
    }

    public Vector2 GetOffset()
    {
        if (duration <= 0f)
            return Vector2.Zero;

        float x = (float)(rng.NextDouble() * 2 - 1) * intensity;
        float y = (float)(rng.NextDouble() * 2 - 1) * intensity;

        return new Vector2(x, y);
    }
}
