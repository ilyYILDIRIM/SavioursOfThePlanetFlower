using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace finalForGP2;

public class MainMenu
{
    private SpriteFont font;
    private Texture2D pixel;
    private int screenWidth;
    private int screenHeight;

    public bool StartGame { get; private set; } = false;
    public bool QuitGame  { get; private set; } = false;

    public MainMenu(SpriteFont font, Texture2D pixel, int screenWidth, int screenHeight)
    {
        this.font         = font;
        this.pixel        = pixel;
        this.screenWidth  = screenWidth;
        this.screenHeight = screenHeight;
    }

    public void Reset()
    {
        StartGame = false;
        QuitGame  = false;
    }

    public void Update(MouseState currentMouse, MouseState previousMouse)
    {
        bool justClicked = currentMouse.LeftButton == ButtonState.Pressed &&
                           previousMouse.LeftButton == ButtonState.Released;

        if (!justClicked) return;

        if (GetStartButtonRect().Contains(currentMouse.Position))
        {
            StartGame = true;
        }

        if (GetQuitButtonRect().Contains(currentMouse.Position))
        {
            QuitGame = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

        string title    = "Saviours of The Planet Flower";
        Vector2 titleSize = font.MeasureString(title);
        float titleX    = screenWidth / 2f - titleSize.X / 2f;
        float titleY    = screenHeight / 2f - 180f;

        spriteBatch.DrawString(font, title, new Vector2(titleX + 2, titleY + 2), Color.DarkGreen);
        spriteBatch.DrawString(font, title, new Vector2(titleX, titleY), Color.LimeGreen);

        DrawButton(spriteBatch, GetStartButtonRect(), "Start", Color.DarkGreen, Color.White);
        DrawButton(spriteBatch, GetQuitButtonRect(),  "Quit",  Color.DarkRed,   Color.White);
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

    private Rectangle GetStartButtonRect()
    {
        int w = 300;
        int h = 70;
        return new Rectangle(screenWidth / 2 - w / 2, screenHeight / 2 - 40, w, h);
    }

    private Rectangle GetQuitButtonRect()
    {
        int w = 300;
        int h = 70;
        return new Rectangle(screenWidth / 2 - w / 2, screenHeight / 2 + 60, w, h);
    }
}
