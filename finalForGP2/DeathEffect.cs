using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class DeathEffect
{
    //To make a death animation we set up a simple particle system.
    private struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float   Life;
        public Color   Color;
        public int     Size;
    }

    private Texture2D pixel;
    private List<Particle> particles = new List<Particle>();
    private float maxLife = 0.5f;

    public bool IsFinished { get; private set; } = false;

    public DeathEffect(Texture2D pixel, Vector2 center, Color color, float scale = 1f)
    {
        this.pixel = pixel;

        Random rng   = new Random();
        int count    = (int)(8 * scale);

        for (int i = 0; i < count; i++)
        {
            //Randomize angle and speed for each particle to create a nice explosion effect.
            float angle = (float)(rng.NextDouble() * Math.PI * 2);
            float speed = (float)(rng.NextDouble() * 150 + 60) * scale;

            particles.Add(new Particle
            {
                Position = center,
                Velocity = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed),
                Life     = maxLife,
                Color    = color,
                Size     = (int)(rng.Next(6, 14) * scale)
            });
        }
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            Particle p = particles[i];
            p.Position += p.Velocity * deltaTime;
            p.Life     -= deltaTime;
            particles[i] = p;

            if (p.Life <= 0f)
            {
                particles.RemoveAt(i);
            }
        }

        if (particles.Count == 0)
        {
            IsFinished = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Particle p in particles)
        {
            //Fade out particles as they die.
            float alpha = p.Life / maxLife;
            Color color = p.Color * alpha;

            spriteBatch.Draw(
                pixel,
                new Rectangle((int)p.Position.X, (int)p.Position.Y, p.Size, p.Size),
                color
            );
        }
    }
}
