using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace finalForGP2;

public static class SoundManager
{
    private static SoundEffect shootPlayer;
    private static SoundEffect shootEnemy;
    private static SoundEffect shootBoss;
    private static SoundEffect enemyDeath;
    private static SoundEffect bossDeath;
    private static SoundEffect planetHit;
    private static SoundEffect turretHit;

    private static SoundEffectInstance bgGame;
    private static SoundEffectInstance bgMenu;

    public static void Load(ContentManager content)
    {
        shootPlayer = content.Load<SoundEffect>("Sounds/playershoot");
        shootEnemy  = content.Load<SoundEffect>("Sounds/enemyshoot");
        shootBoss   = content.Load<SoundEffect>("Sounds/bossshot");
        enemyDeath  = content.Load<SoundEffect>("Sounds/enemydeath");
        bossDeath   = content.Load<SoundEffect>("Sounds/bossdeath (1)");
        planetHit   = content.Load<SoundEffect>("Sounds/planetgothit");
        turretHit   = content.Load<SoundEffect>("Sounds/turretgothit");

        bgGame = content.Load<SoundEffect>("Sounds/backgroundgame").CreateInstance();
        bgGame.IsLooped = true;
        bgGame.Volume   = 0.4f;

        bgMenu = content.Load<SoundEffect>("Sounds/backgroundmenu").CreateInstance();
        bgMenu.IsLooped = true;
        bgMenu.Volume   = 0.4f;
    }

    public static void PlayShootPlayer() => shootPlayer?.Play();
    public static void PlayShootEnemy()  => shootEnemy?.Play();
    public static void PlayShootBoss()   => shootBoss?.Play();
    public static void PlayEnemyDeath()  => enemyDeath?.Play();
    public static void PlayBossDeath()   => bossDeath?.Play();
    public static void PlayPlanetHit()   => planetHit?.Play();
    public static void PlayTurretHit()   => turretHit?.Play();

    public static void PlayGameMusic()
    {
        bgMenu?.Stop();
        if (bgGame?.State != SoundState.Playing)
            bgGame?.Play();
    }

    public static void PlayMenuMusic()
    {
        bgGame?.Stop();
        if (bgMenu?.State != SoundState.Playing)
            bgMenu?.Play();
    }

    public static void StopAll()
    {
        bgGame?.Stop();
        bgMenu?.Stop();
    }
}
