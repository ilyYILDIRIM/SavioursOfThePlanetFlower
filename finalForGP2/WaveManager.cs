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
    private Texture2D bulletTexture;
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
        new WaveConfig(10,  10,  5),
        new WaveConfig(20,  10,  6),
        new WaveConfig(9,  4,  13),
        new WaveConfig(10, 5,  15),
        new WaveConfig(10, 6,  16),
        new WaveConfig(11, 6,  17),
        new WaveConfig(12, 7,  19),
        new WaveConfig(13, 7,  20),
        new WaveConfig(13, 8,  21),
        new WaveConfig(14, 8,  22),
        new WaveConfig(14, 9,  23),
        new WaveConfig(15, 9,  24),
        new WaveConfig(15, 10, 25),
    };

    public WaveManager(Texture2D enemyTexture, Texture2D bulletTexture, Texture2D pixelTexture, int screenWidth, int screenHeight)
    {
        this.enemyTexture  = enemyTexture;
        this.bulletTexture = bulletTexture;
        this.pixelTexture  = pixelTexture;
        this.screenWidth   = screenWidth;
        this.screenHeight  = screenHeight;
    }

    public void SpawnNextWave()
    {
        CurrentWave++;
        Enemies.Clear();
        ShootingEnemies.Clear();
        Boss = null;

        if (CurrentWave % 5 == 0)
        {
            Vector2 bossPos = new Vector2(0, 30f);
            Boss = new BossEnemy(enemyTexture, bulletTexture, pixelTexture, bossPos, screenWidth, screenHeight);
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
                ShootingEnemies.Add(new ShootingEnemy(enemyTexture, bulletTexture, pos, screenHeight));
                shootingPlaced++;
            }
            else
            {
                Enemies.Add(new Enemy(enemyTexture, pos));
            }
        }
    }

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
