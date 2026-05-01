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

    private List<Enemy> enemies = new List<Enemy>();
    

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
        Texture2D enemyBulletTexture = Content.Load<Texture2D>("Textures/bullet");

        player = new Player(ladyBugTexture, bulletTexture, new Vector2(300, 400));

        CreateEnemyWave(30,10);
    }

    private void CreateEnemyWave(int enemyCount, int maxColumns) //We can do waves however we want.
{
    enemies.Clear();

    float startX = 100f;
    float startY = 60f;

    float spacingX = 50f;
    float spacingY = 50f;

    for (int i = 0; i < enemyCount; i++)
    {
        int row = i / maxColumns;
        int col = i % maxColumns;

        Vector2 position = new Vector2(
            startX + col * spacingX,
            startY + row * spacingY
        );

        enemies.Add(new Enemy(enemyTexture, position));
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
    }

    private void CheckPlayerHit()
{
    Rectangle playerBounds = player.GetBounds();


}

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        player.Draw(_spriteBatch);

        foreach (Enemy enemy in enemies)
        {
            enemy.Draw(_spriteBatch);
        }


        _spriteBatch.End();

        base.Draw(gameTime);
    }
}