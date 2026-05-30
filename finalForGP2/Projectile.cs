using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class Projectile : IGameObject
{
    private Texture2D texture;
    private Vector2 position;
    private float speed = 500f;

    public bool IsActive { get; private set; } = true;

    public Projectile(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y -= speed * deltaTime;

        if (position.Y < -texture.Height)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 32, 32), Color.White);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 16, 16);
    }
}
