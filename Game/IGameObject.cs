using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public interface IGameObject
{
    bool IsActive { get; }

    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
    Rectangle GetBounds();
}
