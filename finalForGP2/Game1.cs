using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace finalForGP2;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;

    private int screenWidth;
    private int screenHeight;

    private KeyboardState previousKeyboard;
    private MouseState previousMouse;

    private Player player;
    private WaveManager waveManager;
    private List<Turret> turrets = new List<Turret>();

    private Texture2D ladyBugTexture;
    private Texture2D bulletTexture;
    private Texture2D enemyTexture;
    private Texture2D heartTexture;
    private Texture2D pixelTexture;
    private Texture2D backgroundTexture;
    private Texture2D turretTexture;
    private Texture2D shootingEnemyTexture;
    private Texture2D bossBulletTexture;
    private Texture2D enemyBulletTexture;
    private Texture2D turretBulletTexture;
    private Texture2D playerBulletTexture;

    private FlowerPlanet flowerPlanet;
    private UpgradeMenu upgradeMenu;
    private MainMenu mainMenu;
    private GameOverScreen gameOverScreen;
    private PauseMenu pauseMenu;
    private bool showMenu = true;
    private bool isPaused = false;

    private List<DeathEffect> deathEffects = new List<DeathEffect>();
    private ScreenShake screenShake = new ScreenShake();
    private float timeScale = 1f;

    private float enemyMoveSpeed = 100f;
    private float enemyStepDown = 100f;
    private int enemyDirection = 1;

    private bool   gameOver      = false;
    private bool   gameWon       = false;
    private bool   waveCleared   = false;
    private int    finalScore    = 0;
    private string gameOverReason = "";

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnEnemyKilled += score => System.Diagnostics.Debug.WriteLine("Enemy killed. Score: " + score);
        GameEvents.OnPlayerDamaged += hp => System.Diagnostics.Debug.WriteLine("Player damaged. HP: " + hp);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        screenWidth  = GraphicsDevice.Viewport.Width;
        screenHeight = GraphicsDevice.Viewport.Height;

        ladyBugTexture      = Content.Load<Texture2D>("Textures/LadyBug");
        bulletTexture       = Content.Load<Texture2D>("Textures/bullet");
        enemyTexture        = Content.Load<Texture2D>("Textures/Enemy1");
        heartTexture        = Content.Load<Texture2D>("Textures/kalpPNG");
        font                = Content.Load<SpriteFont>("Fonts/DefaultFont");
        backgroundTexture   = Content.Load<Texture2D>("Textures/background");
        turretTexture       = Content.Load<Texture2D>("Textures/turret");
        shootingEnemyTexture = Content.Load<Texture2D>("Textures/shootingEnemy");
        bossBulletTexture   = Content.Load<Texture2D>("Textures/bossbullet");
        enemyBulletTexture  = Content.Load<Texture2D>("Textures/enemybullet");
        turretBulletTexture = Content.Load<Texture2D>("Textures/turretbullet");
        playerBulletTexture = Content.Load<Texture2D>("Textures/playerbullet");

        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new Color[] { Color.White });

        flowerPlanet = new FlowerPlanet(pixelTexture, screenWidth, screenHeight);
        flowerPlanet.OnDamaged += () => screenShake.Trigger(0.3f, 12f);

        _ = ScoreManager.LoadAsync();

        SoundManager.Load(Content);
        SoundManager.PlayMenuMusic();

        mainMenu       = new MainMenu(font, pixelTexture, screenWidth, screenHeight);
        gameOverScreen = new GameOverScreen(font, pixelTexture, screenWidth, screenHeight);
        pauseMenu      = new PauseMenu(font, pixelTexture, screenWidth, screenHeight);
        waveManager = new WaveManager(enemyTexture, shootingEnemyTexture, enemyBulletTexture, bossBulletTexture, pixelTexture, screenWidth, screenHeight);
        upgradeMenu = new UpgradeMenu(font, pixelTexture, screenWidth, screenHeight);
    }

    private void CreatePlayer()
    {
        player = new Player(ladyBugTexture, playerBulletTexture, new Vector2(screenWidth / 2f, screenHeight - 360f));
    }

    private void SubscribeDeathEffects()
    {
        foreach (Enemy e in waveManager.Enemies)
        {
            e.OnDestroyed += center => deathEffects.Add(new DeathEffect(pixelTexture, center, Color.Orange));
        }

        foreach (ShootingEnemy se in waveManager.ShootingEnemies)
        {
            se.OnDestroyed += center => deathEffects.Add(new DeathEffect(pixelTexture, center, Color.Red));
        }

        if (waveManager.Boss != null)
        {
            waveManager.Boss.OnDestroyed += center => deathEffects.Add(new DeathEffect(pixelTexture, center, Color.Magenta, 4f));
        }
    }

    private void CreateTurrets()
    {
        turrets.Clear();

        float turretY = screenHeight - 260f;
        float spacing = screenWidth / 4.5f;
        float startX  = spacing / 2f;

        for (int i = 0; i < 4; i++)
        {
            Turret t = new Turret(
                turretTexture,
                turretBulletTexture,
                new Vector2(startX + i * spacing, turretY)
            );

            t.OnDestroyed += center => deathEffects.Add(new DeathEffect(pixelTexture, center, Color.Purple, 2.5f));

            turrets.Add(t);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboard = Keyboard.GetState();
        MouseState currentMouse = Mouse.GetState();

        if (showMenu)
        {
            mainMenu.Update(currentMouse, previousMouse);

            if (mainMenu.QuitGame)
            {
                Exit();
            }

            if (mainMenu.StartGame)
            {
                showMenu = false;
                StartNewGame();
            }

            previousKeyboard = currentKeyboard;
            previousMouse    = currentMouse;
            base.Update(gameTime);
            return;
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        {
            Exit();
        }

        if (currentKeyboard.IsKeyDown(Keys.Escape) && !previousKeyboard.IsKeyDown(Keys.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.Reset();
        }

        if (isPaused)
        {
            pauseMenu.Update(currentMouse, previousMouse);

            if (pauseMenu.Resume)
            {
                isPaused = false;
                pauseMenu.Reset();
            }
            else if (pauseMenu.MainMenu)
            {
                isPaused = false;
                pauseMenu.Reset();
                RestartGame();
            }

            previousKeyboard = currentKeyboard;
            previousMouse    = currentMouse;
            base.Update(gameTime);
            return;
        }

        if (gameOver)
        {
            gameOverScreen.Update(currentMouse, previousMouse);

            if (gameOverScreen.Restart)
            {
                gameOverScreen.Reset();
                StartNewGame();
            }
            else if (gameOverScreen.MainMenu)
            {
                gameOverScreen.Reset();
                RestartGame();
            }

            previousKeyboard = currentKeyboard;
            previousMouse    = currentMouse;
            base.Update(gameTime);
            return;
        }

        if (gameWon)
        {
            if (currentKeyboard.IsKeyDown(Keys.R) && !previousKeyboard.IsKeyDown(Keys.R))
            {
                StartNewGame();
            }

            previousKeyboard = currentKeyboard;
            previousMouse    = currentMouse;
            base.Update(gameTime);
            return;
        }

        if (waveCleared)
        {
            upgradeMenu.Update(currentMouse, previousMouse);

            if (upgradeMenu.ReadyForNextWave)
            {
                waveCleared    = false;
                enemyDirection = 1;
                flowerPlanet.ResetHealth();
                deathEffects.Clear();

                if (!player.IsActive)
                {
                    CreatePlayer();
                }

                waveManager.SpawnNextWave();
                SubscribeDeathEffects();
            }

            previousKeyboard = currentKeyboard;
            previousMouse    = currentMouse;
            base.Update(gameTime);
            return;
        }

        timeScale = player.IsActive ? 1f : 3f;
        GameTime scaledTime = new GameTime(
            gameTime.TotalGameTime,
            TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds * timeScale)
        );

        player.Update(scaledTime);

        foreach (Turret turret in turrets)
        {
            turret.Update(scaledTime);
        }

        foreach (ShootingEnemy se in waveManager.ShootingEnemies)
        {
            se.Update(scaledTime);
        }

        if (waveManager.Boss != null && waveManager.Boss.IsActive)
        {
            waveManager.Boss.Update(scaledTime);
        }

        UpdateEnemies(scaledTime);
        CheckBulletEnemyCollisions();
        CheckEnemyBulletCollisions();
        CheckBossBulletCollisions();

        for (int i = deathEffects.Count - 1; i >= 0; i--)
        {
            deathEffects[i].Update(scaledTime);
            if (deathEffects[i].IsFinished)
                deathEffects.RemoveAt(i);
        }

        screenShake.Update(gameTime);

        if (waveManager.IsWaveCleared())
        {
            if (waveManager.CurrentWave >= 15)
            {
                gameWon = true;
                _ = ScoreManager.SaveAsync();
            }
            else
            {
                waveCleared = true;
                upgradeMenu.SetReferences(player, turrets, flowerPlanet);
            }
        }

        CheckEnemyProximity();

        previousKeyboard = currentKeyboard;
        previousMouse    = currentMouse;
        base.Update(gameTime);
    }

    private void UpdateEnemies(GameTime gameTime)
    {
        float deltaTime  = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float moveAmount = enemyMoveSpeed * deltaTime * enemyDirection;

        bool hitWall = false;

        foreach (Enemy enemy in waveManager.Enemies.Where(e => e.IsActive))
        {
            Rectangle bounds = enemy.GetBounds();
            if (bounds.X + moveAmount <= 0 || bounds.X + bounds.Width + moveAmount >= screenWidth)
            {
                hitWall = true;
                break;
            }
        }

        if (!hitWall)
        {
            foreach (ShootingEnemy enemy in waveManager.ShootingEnemies.Where(e => e.IsActive))
            {
                Rectangle bounds = enemy.GetBounds();
                if (bounds.X + moveAmount <= 0 || bounds.X + bounds.Width + moveAmount >= screenWidth)
                {
                    hitWall = true;
                    break;
                }
            }
        }

        if (hitWall)
        {
            enemyDirection *= -1;

            foreach (Enemy enemy in waveManager.Enemies.Where(e => e.IsActive))
                enemy.Move(new Vector2(0f, enemyStepDown));

            foreach (ShootingEnemy enemy in waveManager.ShootingEnemies.Where(e => e.IsActive))
                enemy.Move(new Vector2(0f, enemyStepDown));
        }
        else
        {
            foreach (Enemy enemy in waveManager.Enemies.Where(e => e.IsActive))
                enemy.Move(new Vector2(moveAmount, 0f));

            foreach (ShootingEnemy enemy in waveManager.ShootingEnemies.Where(e => e.IsActive))
                enemy.Move(new Vector2(moveAmount, 0f));
        }
    }

    private void CheckBulletEnemyCollisions()
    {
        List<Projectile> allBullets = new List<Projectile>();
        allBullets.AddRange(player.GetProjectiles());

        foreach (Turret turret in turrets.Where(t => t.IsActive))
        {
            allBullets.AddRange(turret.GetProjectiles());
        }

        foreach (Projectile bullet in allBullets.Where(b => b.IsActive).ToList())
        {
            Rectangle bulletBounds = bullet.GetBounds();

            if (waveManager.Boss != null && waveManager.Boss.IsActive &&
                waveManager.Boss.GetBounds().Intersects(bulletBounds))
            {
                waveManager.Boss.TakeBulletDamage();
                bullet.Deactivate();
                continue;
            }

            Enemy hitEnemy = waveManager.Enemies.FirstOrDefault(e => e.IsActive && e.GetBounds().Intersects(bulletBounds));
            if (hitEnemy != null)
            {
                hitEnemy.TakeBulletDamage();
                bullet.Deactivate();
                continue;
            }

            ShootingEnemy hitSE = waveManager.ShootingEnemies.FirstOrDefault(e => e.IsActive && e.GetBounds().Intersects(bulletBounds));
            if (hitSE != null)
            {
                hitSE.TakeBulletDamage();
                bullet.Deactivate();
            }
        }
    }

    private void CheckEnemyBulletCollisions()
    {
        Rectangle playerBounds = player.GetBounds();

        List<EnemyProjectile> allEnemyBullets = new List<EnemyProjectile>();

        foreach (ShootingEnemy se in waveManager.ShootingEnemies)
        {
            allEnemyBullets.AddRange(se.GetProjectiles().Where(p => p.IsActive));
        }

        foreach (EnemyProjectile ep in allEnemyBullets)
        {
            Rectangle epBounds = ep.GetBounds();

            if (player.IsActive && epBounds.Intersects(playerBounds))
            {
                if (player.IsReflecting)
                {
                    player.AddReflectedProjectile(new Vector2(epBounds.X, epBounds.Y));
                    ep.Deactivate();
                }
                else
                {
                    player.TakeDamage();
                    ep.Deactivate();
                }
                continue;
            }

            Turret hitTurret = turrets.FirstOrDefault(t => t.IsActive && t.GetBounds().Intersects(epBounds));
            if (hitTurret != null)
            {
                hitTurret.TakeDamage();
                ep.Deactivate();
                continue;
            }

            if (flowerPlanet.IsActive && epBounds.Intersects(flowerPlanet.GetBounds()))
            {
                flowerPlanet.TakeDamage(5);
                ep.Deactivate();
            }
        }
    }

    private void CheckBossBulletCollisions()
    {
        if (waveManager.Boss == null || !waveManager.Boss.IsActive) return;

        foreach (BossBullet b in waveManager.Boss.GetBullets().Where(b => b.IsActive).ToList())
        {
            Rectangle bRect = b.GetBounds();

            if (player.IsActive && bRect.Intersects(player.GetBounds()))
            {
                if (player.IsReflecting)
                {
                    player.AddReflectedProjectile(new Vector2(bRect.X, bRect.Y));
                    b.Deactivate();
                }
                else
                {
                    player.InstantKill();
                    b.Deactivate();
                }
                continue;
            }

            Turret hitTurret = turrets.FirstOrDefault(t => t.IsActive && t.GetBounds().Intersects(bRect));
            if (hitTurret != null)
            {
                hitTurret.Disable();
                b.Deactivate();
                continue;
            }

            if (flowerPlanet.IsActive && bRect.Intersects(flowerPlanet.GetBounds()))
            {
                flowerPlanet.TakeDamage(50);
                b.Deactivate();
            }
        }
    }

    private void CheckEnemyProximity()
    {
        if (gameOver) return;

        float dangerY = screenHeight * 0.65f;

        bool tooClose = waveManager.Enemies.Any(e => e.IsActive && e.GetBounds().Y >= dangerY) ||
                        waveManager.ShootingEnemies.Any(e => e.IsActive && e.GetBounds().Y >= dangerY);

        if (tooClose)
        {
            TriggerGameOver("Enemies got too close and took over the planet!");
        }
    }

    private void HandleGameOver(int score)
    {
        gameOver       = true;
        finalScore     = score;
        gameOverReason = "";
        screenShake.Stop();
        _ = ScoreManager.SaveAsync();
    }

    private void TriggerGameOver(string reason)
    {
        gameOver       = true;
        finalScore     = ScoreManager.CurrentScore;
        gameOverReason = reason;
        screenShake.Stop();
        _ = ScoreManager.SaveAsync();
    }

    private void StartNewGame()
    {
        SoundManager.PlayGameMusic();
        gameOver       = false;
        gameWon        = false;
        waveCleared    = false;
        isPaused       = false;
        timeScale      = 1f;
        enemyDirection = 1;
        gameOverReason = "";
        ScoreManager.Reset();
        flowerPlanet = new FlowerPlanet(pixelTexture, screenWidth, screenHeight);
        flowerPlanet.OnDamaged += () => screenShake.Trigger(0.3f, 12f);
        waveManager  = new WaveManager(enemyTexture, shootingEnemyTexture, enemyBulletTexture, bossBulletTexture, pixelTexture, screenWidth, screenHeight);
        CreatePlayer();
        CreateTurrets();
        waveManager.SpawnNextWave();
        SubscribeDeathEffects();
    }

    private void RestartGame()
    {
        showMenu = true;
        mainMenu.Reset();
        SoundManager.PlayMenuMusic();
    }

    private void DrawHUD()
    {
        int heartSize = 32;
        int spacing   = 8;
        int startX    = screenWidth - 180;
        int startY    = 20;

        for (int i = 0; i < player.GetHealth(); i++)
        {
            spriteBatch.Draw(
                heartTexture,
                new Rectangle(startX + i * (heartSize + spacing), startY, heartSize, heartSize),
                Color.White
            );
        }

        spriteBatch.DrawString(font, "Score: " + ScoreManager.CurrentScore, new Vector2(startX - 150, startY + 7), Color.White);
        spriteBatch.DrawString(font, "Wave: " + waveManager.CurrentWave, new Vector2(20, startY + 7), Color.White);

        if (player.HasReflect)
        {
            string reflectText;
            Color reflectColor;

            if (player.IsReflecting)
            {
                reflectText  = "REFLECT: " + player.GetReflectTimer().ToString("0.0") + "s";
                reflectColor = Color.Cyan;
            }
            else if (player.GetReflectCooldown() > 0f)
            {
                reflectText  = "Reflect CD: " + player.GetReflectCooldown().ToString("0.0") + "s";
                reflectColor = Color.Gray;
            }
            else
            {
                reflectText  = "Reflect: READY (E)";
                reflectColor = Color.LightCyan;
            }

            spriteBatch.DrawString(font, reflectText, new Vector2(20, startY + 35), reflectColor);
        }
    }

    private void DrawTurretHealth()
    {
        int heartSize = 14;
        int spacing   = 3;

        foreach (Turret turret in turrets)
        {
            if (!turret.IsActive) continue;

            Vector2 turretPos = turret.GetPosition();

            for (int i = 0; i < turret.Health; i++)
            {
                spriteBatch.Draw(
                    heartTexture,
                    new Rectangle((int)turretPos.X + i * (heartSize + spacing), (int)turretPos.Y - heartSize - 4, heartSize, heartSize),
                    Color.White
                );
            }
        }
    }

    private Rectangle GetNextWaveButtonRect()
    {
        int buttonWidth  = 300;
        int buttonHeight = 70;
        int buttonX      = screenWidth  / 2 - buttonWidth  / 2;
        int buttonY      = screenHeight / 2 - buttonHeight / 2;
        return new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
    }

    private void DrawNextWaveScreen()
    {
        Rectangle buttonRect = GetNextWaveButtonRect();

        spriteBatch.Draw(pixelTexture, buttonRect, Color.DarkGreen);

        string buttonText = "Next Wave (Space)";
        Vector2 textSize  = font.MeasureString(buttonText);
        Vector2 textPos   = new Vector2(
            buttonRect.X + buttonRect.Width  / 2 - textSize.X / 2,
            buttonRect.Y + buttonRect.Height / 2 - textSize.Y / 2
        );

        spriteBatch.DrawString(font, buttonText, textPos, Color.White);

        string waveText = "Wave " + waveManager.CurrentWave + " cleared!";
        Vector2 waveTextSize = font.MeasureString(waveText);
        spriteBatch.DrawString(font, waveText, new Vector2(screenWidth / 2 - waveTextSize.X / 2, buttonRect.Y - 60), Color.White);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        Vector2 shakeOffset = screenShake.GetOffset();
        Matrix shakeMatrix  = Matrix.CreateTranslation(shakeOffset.X, shakeOffset.Y, 0f);
        spriteBatch.Begin(transformMatrix: shakeMatrix);

        if (showMenu)
        {
            mainMenu.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
            return;
        }

        if (isPaused)
        {
            pauseMenu.Draw(spriteBatch);
        }
        else if (gameOver)
        {
            gameOverScreen.Draw(spriteBatch, finalScore, gameOverReason);
        }
        else if (waveCleared)
        {
            upgradeMenu.Draw(spriteBatch);
        }
        else if (!gameWon)
        {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            flowerPlanet.Draw(spriteBatch);
            player.Draw(spriteBatch);
            DrawHUD();

            foreach (Enemy enemy in waveManager.Enemies)
                enemy.Draw(spriteBatch);

            foreach (ShootingEnemy se in waveManager.ShootingEnemies)
                se.Draw(spriteBatch);

            if (waveManager.Boss != null)
                waveManager.Boss.Draw(spriteBatch);

            foreach (Turret turret in turrets)
                turret.Draw(spriteBatch);

            foreach (DeathEffect effect in deathEffects)
                effect.Draw(spriteBatch);

            DrawTurretHealth();
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }

}
