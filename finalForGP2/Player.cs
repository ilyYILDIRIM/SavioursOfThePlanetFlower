using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace finalForGP2;

public class Player : IGameObject
{
    private Texture2D texture;
    private Texture2D projectileTexture;
    private Vector2 position;

    private float scale = 0.04f;
    private float speed = 300f;
    private float shootCooldown = 0.5f;
    private float shootTimer = 0f;
    public int health = 3;

    public bool IsActive { get; private set; } = true;

    public bool HasReflect    { get; private set; } = false;
    public bool IsReflecting  { get; private set; } = false;

    private float reflectTimer    = 0f;
    private float reflectCooldown = 0f;

    private const float ReflectDuration = 10f;
    private const float ReflectCooldownTime = 10f;

    private List<Projectile> projectiles = new List<Projectile>();

    public Player(Texture2D texture, Texture2D projectileTexture, Vector2 startPosition)
    {
        this.texture = texture;
        this.projectileTexture = projectileTexture;
        this.position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            position.X -= speed * deltaTime;
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            position.X += speed * deltaTime;
        }

        int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        position.X = MathHelper.Clamp(position.X, 0, screenWidth - texture.Width * scale);

        shootTimer -= deltaTime;

        if (HasReflect)
        {
            if (IsReflecting)
            {
                reflectTimer -= deltaTime;
                if (reflectTimer <= 0f)
                {
                    IsReflecting    = false;
                    reflectCooldown = ReflectCooldownTime;
                }
            }
            else
            {
                reflectCooldown -= deltaTime;
            }

            if (keyboard.IsKeyDown(Keys.E) && reflectCooldown <= 0f && !IsReflecting)
            {
                IsReflecting  = true;
                reflectTimer  = ReflectDuration;
            }
        }

        bool shootInput = keyboard.IsKeyDown(Keys.Space) || mouse.LeftButton == ButtonState.Pressed;

        if (shootInput && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
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
            position.X + (texture.Width * scale) / 2f,
            position.Y - (texture.Height * scale) / 2f
        );

        projectiles.Add(new Projectile(projectileTexture, spawnPosition));
        SoundManager.PlayShootPlayer();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        foreach (Projectile projectile in projectiles)
        {
            projectile.Draw(spriteBatch);
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

    public List<Projectile> GetProjectiles()
    {
        return projectiles;
    }

    public void InstantKill()
    {
        health = 0;
        IsActive = false;
    }

    public void TakeDamage()
    {
        health--;
        GameEvents.PlayerDamaged(health);

        if (health <= 0)
        {
            IsActive = false;
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public void UnlockReflect()
    {
        HasReflect = true;
    }

    public void AddReflectedProjectile(Vector2 position)
    {
        projectiles.Add(new Projectile(projectileTexture, position));
    }

    public float GetReflectCooldown()
    {
        return reflectCooldown;
    }

    public float GetReflectTimer()
    {
        return reflectTimer;
    }

    public void AddHealth(int amount)
    {
        health += amount;
    }

    public void ImproveSpeed(float amount)
    {
        speed += amount;
    }

    public void ImproveFireRate()
    {
        shootCooldown = Math.Max(0.1f, shootCooldown - 0.05f);
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetShootCooldown()
    {
        return shootCooldown;
    }
}
