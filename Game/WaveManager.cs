using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class WaveConfig
{
    public int NormalEnemyCount;
    public int ShootingEnemyCount;
    public int MaxColumns;

    public WaveConfig(int normalEnemyCount, int shootingEnemyCount, int maxColumns)
    {
        NormalEnemyCount   = normalEnemyCount;
        ShootingEnemyCount = shootingEnemyCount;
        MaxColumns         = maxColumns;
    }
}

public class WaveManager
{
    private Texture2D enemyTexture;
    private Texture2D shootingEnemyTexture;
    private Texture2D enemyBulletTexture;
    private Texture2D bossBulletTexture;
    private Texture2D pixelTexture;
    private int screenWidth;
    private int screenHeight;

    public int CurrentWave { get; private set; } = 0;

    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public List<ShootingEnemy> ShootingEnemies { get; private set; } = new List<ShootingEnemy>();
    public BossEnemy Boss { get; private set; } = null;

    private List<WaveConfig> waveConfigs = new List<WaveConfig>
    {
        new WaveConfig(15,  0,  5),
        new WaveConfig(10,  5,  5),
        new WaveConfig(15,  10,  5),
        new WaveConfig(20,  10,  6),
        new WaveConfig(20,  10,  6),
        new WaveConfig(20, 10,  6),
        new WaveConfig(5, 15,  5),
        new WaveConfig(10, 10,  4),
        new WaveConfig(10, 10,  5),
        new WaveConfig(13, 7,  10),
        new WaveConfig(13, 8,  10),
        new WaveConfig(10, 10,  4),
        new WaveConfig(15, 15,  10),
        new WaveConfig(10, 10,  5),
        new WaveConfig(15, 10, 25),
    };

    public WaveManager(Texture2D enemyTexture, Texture2D shootingEnemyTexture, Texture2D enemyBulletTexture, Texture2D bossBulletTexture, Texture2D pixelTexture, int screenWidth, int screenHeight)
    {
        this.enemyTexture         = enemyTexture;
        this.shootingEnemyTexture = shootingEnemyTexture;
        this.enemyBulletTexture   = enemyBulletTexture;
        this.bossBulletTexture    = bossBulletTexture;
        this.pixelTexture         = pixelTexture;
        this.screenWidth          = screenWidth;
        this.screenHeight         = screenHeight;
    }

    public void SpawnNextWave()
    {
        CurrentWave++;
        Enemies.Clear();
        ShootingEnemies.Clear();
        Boss = null;

        //Every 5 waves we spawn a boss instead of regular enemies. The boss has its own mechanics and is a more challenging enemy.
        if (CurrentWave % 5 == 0)
        {
            Vector2 bossPos = new Vector2(0, 30f);
            Boss = new BossEnemy(enemyTexture, bossBulletTexture, pixelTexture, bossPos, screenWidth, screenHeight);
            return;
        }

        WaveConfig config = GetConfig(CurrentWave);

        float startX   = screenWidth * 0.1f;
        float startY   = screenHeight * 0.05f;
        float spacingX = (screenWidth * 0.8f) / config.MaxColumns;
        float spacingY = 75f;

        int total          = config.NormalEnemyCount + config.ShootingEnemyCount;
        int shootingPlaced = 0;

        for (int i = 0; i < total; i++)
        {
            int row     = i / config.MaxColumns;
            int col     = i % config.MaxColumns;
            Vector2 pos = new Vector2(startX + col * spacingX, startY + row * spacingY);

            if (shootingPlaced < config.ShootingEnemyCount && i % 3 == 0)
            {
                ShootingEnemies.Add(new ShootingEnemy(shootingEnemyTexture, enemyBulletTexture, pos, screenHeight));
                shootingPlaced++;
            }
            else
            {
                Enemies.Add(new Enemy(enemyTexture, pos));
            }
        }
    }

    //Get the wave configuration based on the current wave number. If we exceed the predefined configs, we return the last one which is a more difficult configuration.
    private WaveConfig GetConfig(int waveNumber)
    {
        int index = waveNumber - 1;

        if (index >= 0 && index < waveConfigs.Count)
        {
            return waveConfigs[index];
        }

        return waveConfigs[waveConfigs.Count - 1];
    }

    public bool IsWaveCleared()
    {
        if (Boss != null)
            return !Boss.IsActive;

        return !Enemies.Exists(e => e.IsActive) && !ShootingEnemies.Exists(e => e.IsActive);
    }
}
