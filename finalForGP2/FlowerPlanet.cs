using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class FlowerPlanet : IGameObject
{
    private Texture2D texture;
    private Rectangle bounds;

    public int MaxHealth { get; private set; } = 100;
    public int Health    { get; private set; } = 100;
    public bool IsActive { get; private set; } = true;

    public event Action OnDamaged;

    public FlowerPlanet(Texture2D texture, int screenWidth, int screenHeight)
    {
        this.texture = texture;

        int height = 40;
        bounds = new Rectangle(0, screenHeight - 30, screenWidth, height);
    }

    public void AddHealth(int amount)
    {
        MaxHealth += amount;
    }

    public void ResetHealth()
    {
        Health   = MaxHealth;
        IsActive = true;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;

        if (OnDamaged != null)
            OnDamaged();

        SoundManager.PlayPlanetHit();

        if (Health <= 0)
        {
            Health   = 0;
            IsActive = false;
            GameEvents.GameOver(ScoreManager.CurrentScore);
        }
    }

    public void Update(GameTime gameTime) { }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, bounds, Color.ForestGreen);

        float healthPercent = (float)Health / MaxHealth;
        int healthBarWidth  = (int)(bounds.Width * healthPercent);

        spriteBatch.Draw(
            texture,
            new Rectangle(bounds.X, bounds.Y - 10, bounds.Width, 8),
            Color.DarkGreen * 0.4f
        );

        spriteBatch.Draw(
            texture,
            new Rectangle(bounds.X, bounds.Y - 10, healthBarWidth, 8),
            Color.LimeGreen
        );
    }

    public Rectangle GetBounds()
    {
        return bounds;
    }
}
