using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ShootingEnemy : Enemy
{
    private Texture2D bulletTexture;

    private float shootTimer = 0f;
    private float shootCooldown = 2f;

    private List<EnemyProjectile> projectiles;

    public ShootingEnemy(
        Texture2D enemyTexture,
        Texture2D bulletTexture,
        Vector2 startPosition
    ) : base(enemyTexture, startPosition)
    {
        this.bulletTexture = bulletTexture;
        projectiles = new List<EnemyProjectile>();
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
            position.Y + (texture.Height * scale)
        );

        projectiles.Add(
            new EnemyProjectile(
                bulletTexture,
                spawnPosition
            )
        );
    }

    public List<EnemyProjectile> GetProjectiles()
    {
        return projectiles;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        foreach (EnemyProjectile projectile in projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }
}