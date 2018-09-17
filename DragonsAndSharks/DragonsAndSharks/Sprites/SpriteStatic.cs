namespace DragonsAndSharks.Sprites
{
    using DragonsAndSharks.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteStatic: SpriteBase
    {
        public SpriteStatic(
            Texture2D staticTexture,
            Vector2 position,
            Frame size) 
            : base(staticTexture, null, position, Vector2.Zero, null, null, null)
        {
            StaticTexture = staticTexture;
            FrameSizeNormal = size;
            _rectangleCollision = new Rectangle((int)position.X, (int)position.Y, FrameSizeNormal.X, FrameSizeNormal.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                StaticTexture, 
                new Rectangle(
                    (int)Position.X, 
                    (int)Position.Y, 
                    Size.X, 
                    Size.Y), 
                Color.White);
        }
    }
}
