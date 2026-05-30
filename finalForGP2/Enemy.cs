using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace finalForGP2;

public class Enemy : IGameObject
{
    protected Texture2D _texture;
    protected Vector2   _position;
    protected float     _scale = 0.1f;

    public bool IsActive { get; private set; } = true;

    public event Action<Vector2> OnDestroyed;

    public Enemy(Texture2D texture, Vector2 startPosition)
    {
        _texture  = texture;
        _position = startPosition;
    }

    public void Move(Vector2 offset) => _position += offset;

    public virtual void TakeBulletDamage()
    {
        Destroy();
    }

    public virtual void Destroy()
    {
        IsActive = false;
        GameEvents.EnemyKilled(ScoreManager.CurrentScore + 10);
        ScoreManager.AddScore(10);

        if (OnDestroyed != null)
        {
            Vector2 center = new Vector2(
                _position.X + (_texture.Width  * _scale) / 2f,
                _position.Y + (_texture.Height * _scale) / 2f
            );
            OnDestroyed(center);
        }
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        spriteBatch.Draw(
            _texture,
            _position,
            null, Color.White, 0f, Vector2.Zero,
            _scale, SpriteEffects.None, 0f
        );
    }

    public virtual Rectangle GetBounds() => new Rectangle(
        (int)_position.X,
        (int)_position.Y,
        (int)(_texture.Width  * _scale),
        (int)(_texture.Height * _scale)
    );
}
