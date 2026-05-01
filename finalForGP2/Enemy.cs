using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Enemy
{
    private Texture2D texture;
    private Vector2 position;
    private float scale = 0.1f;

    public bool IsActive { get; private set; } = true;

    private List<Projectile> enemyProjectiles;

    private Texture2D enemyProjectileTexture;

    public Enemy(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;

        enemyProjectiles = new List<Projectile>();
    }

    public void Move(Vector2 offset)
    {
        position += offset;
    }

    public void Destroy()
    {
        IsActive = false;
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

    private void Shoot()
    {
    float scaledWidth = texture.Width * scale;

    Vector2 spawnPosition = new Vector2(
        position.X + scaledWidth / 2f,
        position.Y - (texture.Height * scale) / 2f
    );

    enemyProjectiles.Add(new Projectile(enemyProjectileTexture, spawnPosition));
    }

    public List<Projectile> GetEnemyProjectiles()
    {
        return enemyProjectiles;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive)
            return;

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
    }
}