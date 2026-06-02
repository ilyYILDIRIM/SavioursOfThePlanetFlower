using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class BossBullet : IGameObject
{
    private Texture2D texture;
    private Vector2   position;
    private float     speed = 250f;
    private int       screenHeight;

    public bool IsActive { get; private set; } = true;

    public BossBullet(Texture2D texture, Vector2 startPosition, int screenHeight)
    {
        this.texture      = texture;
        this.position     = startPosition;
        this.screenHeight = screenHeight;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y += speed * dt;

        if (position.Y > screenHeight)
            IsActive = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, new Rectangle((int)position.X - 15, (int)position.Y, 60, 60), Color.White);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X - 15, (int)position.Y, 60, 60);
    }
}
