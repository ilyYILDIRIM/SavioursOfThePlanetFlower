using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace finalForGP2;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Player player;

    Texture2D ladyBugTexture;
    Texture2D bulletTexture;
    Texture2D enemyTexture;
    Texture2D heartTexture;

    Texture2D shootingEnemyTexture;

    Texture2D turretTexture;
private List<Turret> turrets = new List<Turret>();

    private List<Enemy> enemies = new List<Enemy>();
    private List<ShootingEnemy> shootingEnemies = new List<ShootingEnemy>();

    

    private float enemyMoveSpeed = 60f;
    private float enemyStepDownAmount = 20f;
    private int enemyDirection = 1; // 1 = right, -1 = left
    float shootTimer = 0f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        ladyBugTexture = Content.Load<Texture2D>("Textures/ladybug");
        bulletTexture = Content.Load<Texture2D>("Textures/bullet");
        enemyTexture = Content.Load<Texture2D>("Textures/Enemy1"); 
        shootingEnemyTexture = Content.Load<Texture2D>("Textures/Enemy1");
        heartTexture = Content.Load<Texture2D>("Textures/kalpPNG");
        turretTexture = Content.Load<Texture2D>("Textures/ladybug");

        player = new Player(ladyBugTexture, bulletTexture, new Vector2(300, 400));

        float turretY = 320f; // player'ın biraz üstü

        float startX = 75f;
        float spacing = 300f;

        for (int i = 0; i < 3; i++)
        {
        Vector2 turretPosition = new Vector2(
        startX + (i * spacing),
        turretY
        );

        turrets.Add(
        new Turret(
            turretTexture,
            bulletTexture,
            turretPosition
        )
        );
}

        CreateEnemyWave(5,2,10);
    }

    private void CreateEnemyWave(int normalEnemyCount, int shootingEnemyCount, int maxColumns)
{
    enemies.Clear();
    shootingEnemies.Clear();

    float startX = 100f;
    float startY = 60f;

    float spacingX = 90f;
    float spacingY = 70f;

    int totalEnemies = normalEnemyCount + shootingEnemyCount;
    int shootingEnemiesPlaced = 0;

    for (int i = 0; i < totalEnemies; i++)
    {
        int row = i / maxColumns;
        int col = i % maxColumns;

        Vector2 position = new Vector2(
            startX + col * spacingX,
            startY + row * spacingY
        );

        // Her 3 enemy'den biri shooting enemy olsun
        if (shootingEnemiesPlaced < shootingEnemyCount && i % 3 == 0)
        {
            shootingEnemies.Add(
                new ShootingEnemy(
                    shootingEnemyTexture,
                    bulletTexture,
                    position
                )
            );

            shootingEnemiesPlaced++;
        }
        else
        {
            enemies.Add(
                new Enemy(
                    enemyTexture,
                    position
                )
            );
        }
    }
}

    protected override void Update(GameTime gameTime)
    {
        
       
        float shootTimer = 0f;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        player.Update(gameTime);

        foreach (Turret turret in turrets)
        {
        turret.Update(gameTime);
        }

        foreach (ShootingEnemy shootingEnemy in shootingEnemies)
    {
    if (shootingEnemy.IsActive)
    {
        shootingEnemy.Update(gameTime);
    }
    }

    CheckShootingEnemyBulletPlayerCollision();
    

        UpdateEnemies(gameTime);
        CheckBulletEnemyCollisions();

        base.Update(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        shootTimer -= deltaTime;

        if (shootTimer <= 0f && enemies.Count > 0)
        {
            shootTimer = 1.5f;

            Enemy randomEnemy = enemies[0]; // şimdilik ilk enemy

            Vector2 pos = new Vector2(
            randomEnemy.GetBounds().Center.X,
            randomEnemy.GetBounds().Bottom
        );

       
        }
    }
    
    

    private void UpdateEnemies(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float moveAmount = enemyMoveSpeed * deltaTime * enemyDirection;

        bool shouldStepDown = false;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.IsActive)
                continue;

            Rectangle bounds = enemy.GetBounds();
            float nextX = bounds.X + moveAmount;

            if (nextX <= 0 || nextX + bounds.Width >= _graphics.PreferredBackBufferWidth)
            {
                shouldStepDown = true;
                break;
            }
        }

        foreach (ShootingEnemy enemy in shootingEnemies)
        {
        if (!enemy.IsActive)
        continue;

        Rectangle bounds = enemy.GetBounds();
        float nextX = bounds.X + moveAmount;

        if (nextX <= 0 || nextX + bounds.Width >= _graphics.PreferredBackBufferWidth)
        {
        shouldStepDown = true;
        break;
        }
        }

        if (shouldStepDown)
        {
            enemyDirection *= -1;

            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsActive)
                {
                    enemy.Move(new Vector2(0f, enemyStepDownAmount));
                }
            }
            foreach (ShootingEnemy enemy in shootingEnemies)
            {
            if (enemy.IsActive)
            {
                enemy.Move(new Vector2(0f, enemyStepDownAmount));
            }
            }
        }
        else
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsActive)
                {
                    enemy.Move(new Vector2(moveAmount, 0f));
                }
            }

            foreach (ShootingEnemy enemy in shootingEnemies)
            {
            if (enemy.IsActive)            
            {
                enemy.Move(new Vector2(moveAmount, 0f));
            }
            }
        }
    }

    private void CheckBulletEnemyCollisions()
    {
        List<Projectile> projectiles = player.GetProjectiles();

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            Projectile projectile = projectiles[i];

            if (!projectile.IsActive)
                continue;

            Rectangle projectileBounds = projectile.GetBounds();

            for (int j = 0; j < enemies.Count; j++)
            {
                Enemy enemy = enemies[j];

                if (!enemy.IsActive)
                    continue;

                if (projectileBounds.Intersects(enemy.GetBounds()))
                {
                    enemy.Destroy();
                    projectiles.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            Projectile projectile = projectiles[i];

            if (!projectile.IsActive)
                continue;

            Rectangle projectileBounds = projectile.GetBounds();

            for (int j = 0; j < shootingEnemies.Count; j++)
            {
                ShootingEnemy enemy = shootingEnemies[j];

                if (!enemy.IsActive)
                    continue;

                if (projectileBounds.Intersects(enemy.GetBounds()))
                {
                    enemy.Destroy();
                    projectiles.RemoveAt(i);
                    break;
                }
            }
        }
        
    }
    

    private void CheckShootingEnemyBulletPlayerCollision()
    {
    Rectangle playerBounds = player.GetBounds();

    foreach (ShootingEnemy shootingEnemy in shootingEnemies)
    {
        List<EnemyProjectile> projectiles = shootingEnemy.GetProjectiles();

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            if (projectiles[i].GetBounds().Intersects(playerBounds))
            {
                player.TakeDamage();
                projectiles.RemoveAt(i);
            }
        }
    }
    }

    private void CheckPlayerHit()
    {
    Rectangle playerBounds = player.GetBounds();
    }

    private void DrawPlayerHealthUI()
{
    int playerHealth = player.GetHealth();

    int heartSize = 40;
    int spacing = 10;

    int startX = _graphics.PreferredBackBufferWidth - 150;
    int startY = 20;

    for (int i = 0; i < playerHealth; i++)
    {
        Rectangle destination = new Rectangle(
            startX + (i * (heartSize + spacing)),
            startY,
            heartSize,
            heartSize
        );

        _spriteBatch.Draw(heartTexture, destination, Color.White);
    }
}

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        player.Draw(_spriteBatch);
        DrawPlayerHealthUI();

        foreach (Enemy enemy in enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        foreach (Turret turret in turrets)
        {
        turret.Draw(_spriteBatch);
        }

        foreach (ShootingEnemy shootingEnemy in shootingEnemies)
        {
        shootingEnemy.Draw(_spriteBatch);
        }


        _spriteBatch.End();

        base.Draw(gameTime);
    }
}