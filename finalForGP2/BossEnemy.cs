using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class BossEnemy : Enemy
{
    private Texture2D bulletTexture;
    private Texture2D pixelTexture;
    private int       screenWidth;
    private int       screenHeight;

    private int   bossHealth    = 2000;
    private float shootCooldown = 10f;
    private float shootTimer;

    private Rectangle barBounds;
    private List<BossBullet> bullets = new List<BossBullet>();
    private Random rng = new Random();

    public int BossHealth => bossHealth;

    public BossEnemy(Texture2D enemyTexture, Texture2D bulletTexture, Texture2D pixelTexture, Vector2 startPosition, int screenWidth, int screenHeight)
        : base(enemyTexture, startPosition)
    {
        this.bulletTexture = bulletTexture;
        this.pixelTexture  = pixelTexture;
        this.screenWidth   = screenWidth;
        this.screenHeight  = screenHeight;
        this.shootTimer    = shootCooldown;
        this._scale        = 0.1f;

        barBounds = new Rectangle(0, 120, screenWidth, 50);
    }

    public override void TakeBulletDamage()
    {
        bossHealth -= 5;

        if (bossHealth <= 0)
        {
            bossHealth = 0;
            Destroy();
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        shootTimer -= dt;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i].Update(gameTime);
            if (!bullets[i].IsActive)
                bullets.RemoveAt(i);
        }
    }

    private void Shoot()
    {
        float randomX = (float)(rng.NextDouble() * screenWidth);
        Vector2 spawnPos = new Vector2(randomX, barBounds.Bottom);
        bullets.Add(new BossBullet(bulletTexture, spawnPos, screenHeight));
    }

    public List<BossBullet> GetBullets()
    {
        return bullets;
    }

    public override Rectangle GetBounds()
    {
        return barBounds;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(pixelTexture, barBounds, Color.DarkMagenta);

        float pct      = bossHealth / 2000f;
        int   hpWidth  = (int)(screenWidth * pct);

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 120, screenWidth, 50), Color.DarkRed * 0.4f);
        spriteBatch.Draw(pixelTexture, new Rectangle(0, 120, hpWidth, 50), Color.Magenta);

        foreach (BossBullet b in bullets)
            b.Draw(spriteBatch);
    }
}
