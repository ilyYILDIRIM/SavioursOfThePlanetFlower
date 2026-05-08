using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Enemy
{
    protected Texture2D texture;
    protected Vector2 position;
    protected float scale = 0.1f;

    public bool IsActive { get; private set; } = true;

    protected List<Projectile> enemyProjectiles;


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

    public List<Projectile> GetEnemyProjectiles()
    {
        return enemyProjectiles;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
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