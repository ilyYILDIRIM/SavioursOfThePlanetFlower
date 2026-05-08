using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Turret
{
    private Texture2D texture;
    private Texture2D bulletTexture;
    private Vector2 position;

    private float scale = 0.04f;

    private float shootTimer = 0f;
    private float shootCooldown = 1.5f;

    private List<Projectile> projectiles;

    public Turret(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition)
    {
        this.texture = texture;
        this.bulletTexture = bulletTexture;
        this.position = startPosition;

        projectiles = new List<Projectile>();
    }

    public void Update(GameTime gameTime)
    {
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

        projectiles.Add(
            new Projectile(
                bulletTexture,
                spawnPosition
            )
        );
    }

    public List<Projectile> GetProjectiles()
    {
        return projectiles;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );

        foreach (Projectile projectile in projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }
}