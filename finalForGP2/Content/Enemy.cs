using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Enemy
{
    private Texture2D texture;
    private Vector2 position;
    private float scale = 0.08f;

    public bool IsActive { get; private set; } = true;

    public Enemy(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
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