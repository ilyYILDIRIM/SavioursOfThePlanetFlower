using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace finalForGP2;

public class HighScoreData
{
    public int HighScore { get; set; }
    public DateTime LastPlayed { get; set; }
}

public static class ScoreManager
{
    private static readonly string SavePath = "highscore.json";

    public static int CurrentScore { get; private set; } = 0;
    public static int HighScore    { get; private set; } = 0;

    public static void AddScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore > HighScore)
            HighScore = CurrentScore;
    }

    public static void Reset()
    {
        CurrentScore = 0;
    }

    public static void SpendScore(int amount)
    {
        CurrentScore -= amount;
        if (CurrentScore < 0)
            CurrentScore = 0;
    }

    public static async Task SaveAsync()
    {
        try
        {
            var data = new HighScoreData
            {
                HighScore  = HighScore,
                LastPlayed = DateTime.Now
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(SavePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ScoreManager] Save failed: {ex.Message}");
        }
    }

    public static async Task LoadAsync()
    {

        try
        {
            if (!File.Exists(SavePath))
                return;

            string json = await File.ReadAllTextAsync(SavePath);
            var data = JsonSerializer.Deserialize<HighScoreData>(json);

            if (data != null)
                HighScore = data.HighScore;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ScoreManager] Load failed: {ex.Message}");
        }
    }
}
