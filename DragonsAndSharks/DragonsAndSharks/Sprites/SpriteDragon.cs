namespace DragonsAndSharks.Sprites
{
    using DragonsAndSharks.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteDragon: SpriteBase
    {
        public SpriteDragon(
            Texture2D staticTexture, 
            Texture2D animationTexture, 
            Vector2 position, 
            Vector2 speed,
            Frame frameSheet,
            Frame frameSize, 
            Frame framePosition) : 
                base(staticTexture, animationTexture, position, speed, frameSheet, frameSize, framePosition, 0)
        {
        }

        public SpriteDragon(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Vector2 position,
            Vector2 speed,
            Frame frameSheet,
            Frame frameSize,
            Frame framePosition,
            int animationOn) :
            base(staticTexture, animationTexture, position, speed, frameSheet, frameSize, framePosition, animationOn)
        {
        }

        public SpriteDragon(
            Texture2D staticTexture, 
            Texture2D animationTexture, 
            Vector2 position, 
            Vector2 speed,
            Frame frameSheet,
            Frame frameSize, 
            Frame framePosition,
            int animationOn,
            int millisecondsPerFrame) : 
                base(staticTexture, animationTexture, position, speed, frameSheet, frameSize, framePosition, animationOn, millisecondsPerFrame)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Vector2 adjustment)
        {
  
            base.Update(gameTime, clientBounds, adjustment);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            base.Draw(gameTime, spriteBatch);
        }
    }
}
