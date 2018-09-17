namespace DragonsAndSharks.Sprites
{
    using Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteBase
    {
        private const int DefaultMillisecondsPerFrame = 500;

        public int AnimationType { get; set; }

        public Texture2D StaticTexture { get; set; }
        public Texture2D AnimationTexture { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        protected Frame FrameNormalSheet;
        protected Frame FrameSizeNormal;

        public Frame CyclicalFrameSheet { get; set; }
        public Frame CyclicalFrameSize { get; set; }
        public Frame CyclicalFramePosition { get; set; }

        public Frame Size
        {
            get
            {
                return !Speed.Equals(Vector2.Zero) ? FrameSizeNormal : (CyclicalFrameSize ?? FrameSizeNormal);
            }
        }

        public Vector2 Acceleration { get; set; }

        public Rectangle RectangleCollision
        {
            get
            {
                return _rectangleCollision;
            }
            private set
            {
                _rectangleCollision = value;
            }
        }

        private int _timeSinceLastFrame;
        private readonly int _millisecondsPerFrame;
        protected Rectangle _rectangleCollision;

        public SpriteBase(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Vector2 position,
            Vector2 speed,
            Frame cyclicalFrameSheet,
            Frame cyclicalFrameSize,
            Frame cyclicalFramePosition)
            : this(staticTexture, animationTexture, position, speed, cyclicalFrameSheet, cyclicalFrameSize, cyclicalFramePosition, 0, DefaultMillisecondsPerFrame)
        {
        }

        public SpriteBase(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Vector2 position,
            Vector2 speed,
            Frame cyclicalFrameSheet,
            Frame cyclicalFrameSize,
            Frame cyclicalFramePosition,
            int animationOn)
            : this(staticTexture, animationTexture, position, speed, cyclicalFrameSheet, cyclicalFrameSize, cyclicalFramePosition, animationOn, DefaultMillisecondsPerFrame)
        {
        }

        public SpriteBase(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Vector2 position,
            Vector2 speed,
            Frame cyclicalFrameSheet,
            Frame cyclicalFrameSize,
            Frame cyclicalFramePosition,
            int animationOn,
            int millisecondsPerFrame)
        {
            StaticTexture = staticTexture;
            AnimationTexture = animationTexture;

            Position = position;
            Speed = speed;

            CyclicalFrameSheet = cyclicalFrameSheet;
            CyclicalFrameSize = cyclicalFrameSize;
            CyclicalFramePosition = cyclicalFramePosition;
            AnimationType = animationOn;

            _millisecondsPerFrame = millisecondsPerFrame;
        }

        public SpriteBase(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Vector2 position,
            Vector2 speed,
            Frame cyclicalFrameSheet,
            Frame cyclicalFrameSize,
            Frame cyclicalFramePosition,
            int animationOn,
            int millisecondsPerFrame,
            Vector2 acceleration)
            : this(staticTexture, animationTexture, position, speed, cyclicalFrameSheet, cyclicalFrameSize, cyclicalFramePosition, animationOn, millisecondsPerFrame)
        {
            Acceleration = acceleration;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds, Vector2 adjustment)
        {
            BaseAnimate(gameTime);

            Speed += Acceleration;
            Position += Speed + adjustment;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle rectangle;
            Texture2D texture;
            switch (AnimationType)
            {
                case 0:
                {
                    texture = StaticTexture;
                    rectangle = new Rectangle(0, 0, CyclicalFrameSize.X, CyclicalFrameSize.Y);
                    break;
                }
                case 1:
                {
                    texture = AnimationTexture;
                    rectangle = new Rectangle(CyclicalFramePosition.X*CyclicalFrameSize.X,
                        CyclicalFramePosition.Y*CyclicalFrameSize.Y, CyclicalFrameSize.X, CyclicalFrameSize.Y);
                    break;
                }
                default:
                {
                    texture = StaticTexture;
                    rectangle = new Rectangle(0, 0, CyclicalFrameSize.X, CyclicalFrameSize.Y);
                    break;
                }
            }

            spriteBatch.Draw(texture,
                Position,
                rectangle,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                Speed.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }

        public bool InCollision(SpriteBase spriteBase)
        {
            return RectangleCollision.Intersects(spriteBase.RectangleCollision);
        }

        public bool OutOfScreen()
        {
            return Position.X <= 0 || Position.Y <= 0;
        }

        #region Private methods

        private void BaseAnimate(GameTime gameTime)
        {
            switch (AnimationType)
            {
                case 1:
                {
                    _timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                    if (_timeSinceLastFrame > _millisecondsPerFrame)
                    {
                        _timeSinceLastFrame = 0;
                        ++CyclicalFramePosition.X;

                        if (CyclicalFramePosition.X >= CyclicalFrameSheet.X)
                        {
                            CyclicalFramePosition.X = 0;
                            ++CyclicalFramePosition.Y;
                            if (CyclicalFramePosition.Y >= CyclicalFrameSheet.Y)
                            {
                                CyclicalFramePosition.Y = 0;
                            }
                        }
                    }
                    break;
                }
            }

            if (CyclicalFrameSheet != null && FrameNormalSheet != null)
            {
                SetRectangleCollision(Speed.LengthSquared() > 0);
            }
        }

        private void SetRectangleCollision(bool moving)
        {
                _rectangleCollision.X = (int) Position.X;
                _rectangleCollision.Y = (int) Position.Y;
                _rectangleCollision.Width = moving ? CyclicalFrameSize.X : FrameSizeNormal.X;
                _rectangleCollision.Height = moving ? CyclicalFrameSize.Y : FrameSizeNormal.Y;
        }

        #endregion

    }
}
