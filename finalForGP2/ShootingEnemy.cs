using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class ShootingEnemy : Enemy
{
    private Texture2D bulletTexture;
    private int screenHeight;

    private float shootCooldown = 5f;
    private float shootTimer;

    private List<EnemyProjectile> projectiles = new List<EnemyProjectile>();

    public ShootingEnemy(Texture2D enemyTexture, Texture2D bulletTexture, Vector2 startPosition, int screenHeight)
        : base(enemyTexture, startPosition)
    {
        this.bulletTexture = bulletTexture;
        this.screenHeight  = screenHeight;
        this.shootTimer    = shootCooldown;
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsActive)
        {
            shootTimer -= deltaTime;

            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
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
            _position.X + (_texture.Width * _scale) / 2f,
            _position.Y + (_texture.Height * _scale)
        );

        projectiles.Add(new EnemyProjectile(bulletTexture, spawnPosition, screenHeight));
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
