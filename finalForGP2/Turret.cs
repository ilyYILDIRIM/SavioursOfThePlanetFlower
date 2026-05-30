using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class Turret : IGameObject
{
    private Texture2D texture;
    private Texture2D bulletTexture;
    private Vector2 position;
    private float scale = 0.08f;

    private float shootTimer = 0f;
    private float shootCooldown = 5f;

    public int Health { get; private set; } = 5;
    public bool IsActive { get; private set; } = true;

    public event Action<Vector2> OnDestroyed;

    private List<Projectile> projectiles = new List<Projectile>();

    public Turret(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition)
    {
        this.texture = texture;
        this.bulletTexture = bulletTexture;
        this.position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        shootTimer -= deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            projectiles[i].Update(gameTime);

            if (!projectiles[i].IsActive)
            {
                projectiles.RemoveAt(i);
            }
        }
    }

    private void Shoot()
    {
        Vector2 spawnPosition = new Vector2(
            position.X + (texture.Width * scale) / 2f,
            position.Y
        );

        projectiles.Add(new Projectile(bulletTexture, spawnPosition));
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public List<Projectile> GetProjectiles()
    {
        return projectiles;
    }

    public void Revive()
    {
        Health   = 3;
        IsActive = true;
    }

    public void AddHealth(int amount)
    {
        Health += amount;
    }

    public void ImproveFireRate()
    {
        shootCooldown = Math.Max(0.3f, shootCooldown - 0.2f);
    }

    public float GetShootCooldown()
    {
        return shootCooldown;
    }

    public void TakeDamage()
    {
        Health--;
        SoundManager.PlayTurretHit();

        if (Health <= 0)
        {
            Disable();
        }
    }
    
    public void Disable()
    {
        IsActive = false;
        projectiles.Clear();

        if (OnDestroyed != null)
        {
            Vector2 center = new Vector2(
                position.X + (texture.Width  * scale) / 2f,
                position.Y + (texture.Height * scale) / 2f
            );
            OnDestroyed(center);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        foreach (Projectile projectile in projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );
    }
}
