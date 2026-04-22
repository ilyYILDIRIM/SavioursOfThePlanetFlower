using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Projectile
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
        Rectangle destination = new Rectangle(
        (int)position.X,
        (int)position.Y,
        16,   // genişlik
        16    // yükseklik
    );
    if (IsActive)
    {
        spriteBatch.Draw(texture, destination, Color.White);
    }
    
        
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)position.X,
            (int)position.Y,
            16,
            16
        );
    }
}