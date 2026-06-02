using System;

namespace finalForGP2;

public static class GameEvents
{
    //Organize event manager for game-wide events like enemy kills, player damage, and game over.
    public static event Action<int> OnEnemyKilled;
    public static event Action<int> OnPlayerDamaged;
    public static event Action<int> OnGameOver;

    public static void EnemyKilled(int score)
    {
        if (OnEnemyKilled != null)
            OnEnemyKilled(score);
    }

    public static void PlayerDamaged(int health)
    {
        if (OnPlayerDamaged != null)
            OnPlayerDamaged(health);
    }

    public static void GameOver(int finalScore)
    {
        if (OnGameOver != null)
            OnGameOver(finalScore);
    }
}
