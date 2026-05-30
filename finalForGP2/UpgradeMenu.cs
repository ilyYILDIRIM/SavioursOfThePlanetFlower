using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace finalForGP2;

public class UpgradeEntry
{
    public string Label;
    public int Cost;
    public Action OnBuy;
    public Func<string> GetCurrentValue;

    public UpgradeEntry(string label, int cost, Action onBuy, Func<string> getCurrentValue)
    {
        Label           = label;
        Cost            = cost;
        OnBuy           = onBuy;
        GetCurrentValue = getCurrentValue;
    }
}

public class UpgradeMenu
{
    private Player player;
    private List<Turret> turrets;
    private FlowerPlanet flowerPlanet;
    private SpriteFont font;
    private Texture2D pixel;
    private int screenWidth;
    private int screenHeight;

    public bool ReadyForNextWave { get; private set; } = false;

    private List<UpgradeEntry> entries = new List<UpgradeEntry>();

    public UpgradeMenu(SpriteFont font, Texture2D pixel, int screenWidth, int screenHeight)
    {
        this.font        = font;
        this.pixel       = pixel;
        this.screenWidth  = screenWidth;
        this.screenHeight = screenHeight;
    }

    public void SetReferences(Player player, List<Turret> turrets, FlowerPlanet flowerPlanet)
    {
        this.player      = player;
        this.turrets     = turrets;
        this.flowerPlanet = flowerPlanet;

        ReadyForNextWave = false;
        BuildEntries();
    }

    private void BuildEntries()
    {
        entries.Clear();

        entries.Add(new UpgradeEntry(
            "Player: Move Speed +50",
            20,
            () => player.ImproveSpeed(50f),
            () => "Speed: " + (int)player.GetSpeed()
        ));

        entries.Add(new UpgradeEntry(
            "Player: Fire Rate",
            25,
            () => player.ImproveFireRate(),
            () => "Cooldown: " + player.GetShootCooldown().ToString("0.00") + "s"
        ));

        entries.Add(new UpgradeEntry(
            "Player: +1 HP",
            30,
            () => player.AddHealth(1),
            () => "HP: " + player.GetHealth()
        ));

        entries.Add(new UpgradeEntry(
            "FlowerPlanet: +20 HP",
            25,
            () => flowerPlanet.AddHealth(20),
            () => "HP: " + flowerPlanet.MaxHealth
        ));

        if (!player.HasReflect)
        {
            entries.Add(new UpgradeEntry(
                "Player: Unlock Reflect (E)",
                45,
                () => player.UnlockReflect(),
                () => "Not unlocked"
            ));
        }
        else
        {
            entries.Add(new UpgradeEntry(
                "Reflect: Unlocked",
                0,
                () => { },
                () => "Press E to activate"
            ));
        }

        for (int i = 0; i < turrets.Count; i++)
        {
            int index = i;
            Turret turret = turrets[i];

            if (!turret.IsActive)
            {
                entries.Add(new UpgradeEntry(
                    "Turret " + (index + 1) + ": Revive",
                    50,
                    () => turret.Revive(),
                    () => "DESTROYED"
                ));
                continue;
            }

            entries.Add(new UpgradeEntry(
                "Turret " + (index + 1) + ": +2 HP",
                20,
                () => turret.AddHealth(2),
                () => "HP: " + turret.Health
            ));

            entries.Add(new UpgradeEntry(
                "Turret " + (index + 1) + ": Fire Rate",
                25,
                () => turret.ImproveFireRate(),
                () => "Cooldown: " + turret.GetShootCooldown().ToString("0.00") + "s"
            ));
        }
    }

    public void Update(MouseState currentMouse, MouseState previousMouse)
    {
        bool justClicked = currentMouse.LeftButton == ButtonState.Pressed &&
                           previousMouse.LeftButton == ButtonState.Released;

        if (!justClicked) return;

        Rectangle nextWaveButton = GetNextWaveButtonRect();
        if (nextWaveButton.Contains(currentMouse.Position))
        {
            ReadyForNextWave = true;
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            Rectangle buttonRect = GetEntryRect(i);

            if (buttonRect.Contains(currentMouse.Position))
            {
                UpgradeEntry entry = entries[i];

                if (entry.Cost > 0 && ScoreManager.CurrentScore >= entry.Cost)
                {
                    ScoreManager.SpendScore(entry.Cost);
                    entry.OnBuy();
                    BuildEntries();
                }

                break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * 0.7f);

        string title = "UPGRADE SHOP";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title, new Vector2(screenWidth / 2f - titleSize.X / 2f, 40), Color.Gold);

        string scoreText = "Score: " + ScoreManager.CurrentScore + " pts";
        Vector2 scoreSize = font.MeasureString(scoreText);
        spriteBatch.DrawString(font, scoreText, new Vector2(screenWidth / 2f - scoreSize.X / 2f, 80), Color.White);

        for (int i = 0; i < entries.Count; i++)
        {
            UpgradeEntry entry = entries[i];
            Rectangle rect = GetEntryRect(i);

            bool canAfford = ScoreManager.CurrentScore >= entry.Cost;
            Color bgColor  = canAfford ? Color.DarkGreen : Color.DarkRed;

            spriteBatch.Draw(pixel, rect, bgColor);

            string label     = entry.Label;
            string costText  = entry.Cost + " pts";
            string valueText = entry.GetCurrentValue();

            spriteBatch.DrawString(font, label,     new Vector2(rect.X + 10, rect.Y + 8),  Color.White);
            spriteBatch.DrawString(font, valueText, new Vector2(rect.X + 10, rect.Y + 32), Color.LightGray);
            spriteBatch.DrawString(font, costText,  new Vector2(rect.Right - (int)font.MeasureString(costText).X - 10, rect.Y + 18), Color.Yellow);
        }

        Rectangle nextBtn = GetNextWaveButtonRect();
        spriteBatch.Draw(pixel, nextBtn, Color.DarkBlue);

        string nextText = "Start Next Wave";
        Vector2 nextSize = font.MeasureString(nextText);
        spriteBatch.DrawString(font, nextText,
            new Vector2(nextBtn.X + nextBtn.Width / 2f - nextSize.X / 2f, nextBtn.Y + nextBtn.Height / 2f - nextSize.Y / 2f),
            Color.White);
    }

    private Rectangle GetEntryRect(int index)
    {
        int buttonW  = 420;
        int buttonH  = 60;
        int gap      = 8;
        int startY   = 130;

        int col = index % 2;
        int row = index / 2;

        int totalW = buttonW * 2 + gap;
        int startX = screenWidth / 2 - totalW / 2;

        int x = startX + col * (buttonW + gap);
        int y = startY + row * (buttonH + gap);

        return new Rectangle(x, y, buttonW, buttonH);
    }

    private Rectangle GetNextWaveButtonRect()
    {
        int rows   = (int)Math.Ceiling(entries.Count / 2.0);
        int startY = 130 + rows * 68 + 20;

        int w = 300;
        int h = 60;
        return new Rectangle(screenWidth / 2 - w / 2, startY, w, h);
    }
}
