using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace finalForGP2;

public class GameOverScreen
{
    private SpriteFont font;
    private Texture2D pixel;
    private int screenWidth;
    private int screenHeight;

    public bool Restart   { get; private set; } = false;
    public bool MainMenu  { get; private set; } = false;

    public GameOverScreen(SpriteFont font, Texture2D pixel, int screenWidth, int screenHeight)
    {
        this.font        = font;
        this.pixel       = pixel;
        this.screenWidth  = screenWidth;
        this.screenHeight = screenHeight;
    }

    public void Reset()
    {
        Restart  = false;
        MainMenu = false;
    }

    public void Update(MouseState currentMouse, MouseState previousMouse)
    {
        bool justClicked = currentMouse.LeftButton == ButtonState.Pressed &&
                           previousMouse.LeftButton == ButtonState.Released;

        if (!justClicked) return;

        if (GetRestartButtonRect().Contains(currentMouse.Position))
        {
            Restart = true;
        }

        if (GetMainMenuButtonRect().Contains(currentMouse.Position))
        {
            MainMenu = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch, int finalScore, string reason = "")
    {
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * 0.85f);

        string title     = "GAME OVER";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(screenWidth / 2f - titleSize.X / 2f, screenHeight / 2f - 200f),
            Color.Red);

        if (reason != "")
        {
            Vector2 reasonSize = font.MeasureString(reason);
            spriteBatch.DrawString(font, reason,
                new Vector2(screenWidth / 2f - reasonSize.X / 2f, screenHeight / 2f - 145f),
                Color.Orange);
        }

        string scoreText = "Final Score: " + finalScore;
        Vector2 scoreSize = font.MeasureString(scoreText);
        spriteBatch.DrawString(font, scoreText,
            new Vector2(screenWidth / 2f - scoreSize.X / 2f, screenHeight / 2f - 95f),
            Color.White);

        DrawButton(spriteBatch, GetRestartButtonRect(),  "Play Again", Color.DarkGreen, Color.White);
        DrawButton(spriteBatch, GetMainMenuButtonRect(), "Main Menu",  Color.DarkBlue,  Color.White);
    }

    private void DrawButton(SpriteBatch spriteBatch, Rectangle rect, string label, Color bgColor, Color textColor)
    {
        spriteBatch.Draw(pixel, rect, bgColor);

        Vector2 textSize = font.MeasureString(label);
        Vector2 textPos  = new Vector2(
            rect.X + rect.Width  / 2f - textSize.X / 2f,
            rect.Y + rect.Height / 2f - textSize.Y / 2f
        );

        spriteBatch.DrawString(font, label, textPos, textColor);
    }

    //Helper methods to get button rectangles for click detection.
    private Rectangle GetRestartButtonRect()
    {
        int w = 300;
        int h = 70;
        return new Rectangle(screenWidth / 2 - w / 2, screenHeight / 2 - 20, w, h);
    }

    private Rectangle GetMainMenuButtonRect()
    {
        int w = 300;
        int h = 70;
        return new Rectangle(screenWidth / 2 - w / 2, screenHeight / 2 + 80, w, h);
    }
}
