using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class EnemyProjectile
{
    private Texture2D texture;
    private Vector2 position;
    private float speed = 300f;

    public bool IsActive { get; private set; } = true;

    public EnemyProjectile(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        position.Y += speed * deltaTime;

        if (position.Y > 800)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle rect = new Rectangle((int)position.X, (int)position.Y, 10, 10);

        if (IsActive)
        {
            spriteBatch.Draw(texture, rect, Color.Red);
        }
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 10, 10);
    }
}