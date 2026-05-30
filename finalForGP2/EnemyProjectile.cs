using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class EnemyProjectile : IGameObject
{
    private Texture2D texture;
    private Vector2 position;
    private float speed = 300f;
    private int screenHeight;

    public bool IsActive { get; private set; } = true;

    public EnemyProjectile(Texture2D texture, Vector2 startPosition, int screenHeight)
    {
        this.texture = texture;
        this.position = startPosition;
        this.screenHeight = screenHeight;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y += speed * deltaTime;

        if (position.Y > screenHeight)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 40, 40), Color.White);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 40, 40);
    }
}
