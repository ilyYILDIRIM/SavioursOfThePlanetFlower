using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Player
{
    private int health = 3;
    private float scale = 0.04f;
    private Texture2D texture;
    private Vector2 position;
    private float speed = 300f;

    private float shootCooldown = 1f;
    private float shootTimer = 0f;

    private Texture2D projectileTexture;
    private List<Projectile> projectiles;

    public Player(Texture2D texture, Texture2D projectileTexture, Vector2 startPosition)
    {
        this.texture = texture;
        this.projectileTexture = projectileTexture;
        this.position = startPosition;

        projectiles = new List<Projectile>();
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();

        // Movement Inputs
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            position.X -= speed * deltaTime;
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            position.X += speed * deltaTime;
        }

        float screenWidth = 800; //Locking movement to 800 pixel 

        if (position.X < 0)
        {
            position.X = 0;
        }

        if (position.X > screenWidth - texture.Width * scale)
        {
            position.X = screenWidth - texture.Width * scale;
        }

        // Cooldown (Maybe we can delete this later in development)
        shootTimer -= deltaTime;

        bool shootInput =
            keyboard.IsKeyDown(Keys.Space) ||
            mouse.LeftButton == ButtonState.Pressed;

        if (shootInput && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

        // --- Update Projectiles ---
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
    float scaledWidth = texture.Width * scale;

    Vector2 spawnPosition = new Vector2(
        position.X + scaledWidth / 2f,
        position.Y - (texture.Height * scale) / 2f
    );

    projectiles.Add(new Projectile(projectileTexture, spawnPosition));
    }

   public void Draw(SpriteBatch spriteBatch)
    {
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

    foreach (Projectile projectile in projectiles)
    {
        projectile.Draw(spriteBatch);
    }
    }

    public List<Projectile> GetProjectiles()
    {
        return projectiles;
    }

    public void TakeDamage()
    {
    health--;

    if (health <= 0)
    {
        System.Diagnostics.Debug.WriteLine("Öldünüz fakat taretleriniz savaşmaya devam ediyor.");
    }
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
}