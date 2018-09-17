namespace DragonsAndSharks.Sprites
{
    using System.Collections.Generic;
    using System.Linq;
    using DragonsAndSharks.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input.Touch;

    public class SpriteUser: SpriteBase
    {
        private int DeviceType;
        
        private const float MaxSpeedX = 4;
        private const float MaxJumpSpeedX = 3.5f;

        private readonly Vector2 StoppingXLeft = new Vector2(1f, 0);
        private readonly Vector2 StoppingXRight = new Vector2(-1f, 0);

        private Texture2D AnimationNormalTexture;
        private Texture2D AnimationJumpTexture;

        public bool IsAbleToJump { get; set; }
        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        public bool IsAttacking { get; private set; }

        public int AdvancingDirection { get; private set; }

        public bool JumpButtonPressed = true;

        private int ActionRemaining;

        private Frame FrameJumpingSheet;
        private Frame FrameSizeJumping;

        public SpriteUser(
            Texture2D staticTexture, 
            Texture2D animationTexture,
            Texture2D animationJumpTexture,
            Vector2 position, 
            Vector2 speed,
            Frame frameSheet,
            Frame frameSize,
            Frame framePosition,
            Frame frameJumpingSheet,
            Frame frameSizeJumping) 
            : base(staticTexture, animationTexture, position, speed, frameSheet, frameSize, framePosition, 1)
        {
            AnimationNormalTexture = animationTexture;
            AnimationJumpTexture = animationJumpTexture;
            FrameNormalSheet = frameSheet;
            FrameSizeNormal = frameSize;
            FrameJumpingSheet = frameJumpingSheet;
            FrameSizeJumping = frameSizeJumping;
            IsFalling = false;
            IsJumping = false;
        }
        public SpriteUser(
            Texture2D staticTexture,
            Texture2D animationTexture,
            Texture2D animationJumpTexture,
            Vector2 position,
            Vector2 speed,
            Frame frameSheet,
            Frame frameSize,
            Frame framePosition,
            Frame frameJumpingSheet,
            Frame frameSizeJumping,
            int millsecondsPerFrame)
            : base(staticTexture, animationTexture, position, speed, frameSheet, frameSize, framePosition, 1, millsecondsPerFrame)
        {
            AnimationNormalTexture = animationTexture;
            AnimationJumpTexture = animationJumpTexture;
            FrameNormalSheet = frameSheet;
            FrameSizeNormal = frameSize;
            FrameJumpingSheet = frameJumpingSheet;
            FrameSizeJumping = frameSizeJumping;
            IsFalling = false;
            IsJumping = false;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Vector2 adjustment)
        {
            var touches = TouchPanel.GetState();

            var pressed = touches.Where(t => t.State == TouchLocationState.Pressed).ToList();

            GetJumpButtonPressed(touches);

            SetInitialAdvancing(touches);

            if (IsAbleToJump)
            {
                if (IsJumping) Land();

                if (JumpButtonPressed)
                {
                    Jump();
                }
            }

            SetFinalAdvancing(pressed);

            MoveToAdvancingDirection();

            if (Speed.X.Equals(0) && !IsJumping)
            {
                SetCurrentFrame(new Frame(0, 0), new Frame(23, 57), new Frame(0, 0));
                AnimationTexture = StaticTexture;
            }
            base.Update(gameTime, clientBounds, adjustment);
        }

        private void SetFinalAdvancing(List<TouchLocation> pressed)
        {
            if (AdvancingDirection == 0)
            {
                AdvancingDirection = GetAdvancingDirection(pressed);

                if ((AdvancingDirection != 0) && !IsJumping)
                {
                    SetCurrentFrame(FrameNormalSheet, FrameSizeNormal, new Frame(0, 0));
                    AnimationTexture = AnimationNormalTexture;
                }
            }
        }

        private void MoveToAdvancingDirection()
        {
            switch (AdvancingDirection)
            {
                case 1:
                    GoLeft();
                    break;
                case 2:
                    GoRight();
                    break;
                default:
                    Stopping();
                    break;
            }
        }

        private void SetInitialAdvancing(TouchCollection touches)
        {
            if (touches.Any(t => t.State == TouchLocationState.Released))
            {
                if (touches.Any(t => t.State == TouchLocationState.Released
                    && !(t.Position.Y >= 380 && t.Position.Y <= 480 && t.Position.X >= 700)))
                {
                   AdvancingDirection = 0;
                }
            }
        }

        private void GetJumpButtonPressed(TouchCollection touches)
        {
            JumpButtonPressed = touches.Any(t =>
                (t.Position.Y >= 380 && t.Position.Y <= 480 && t.Position.X >= 700)
                && t.State == TouchLocationState.Pressed
                && t.State != TouchLocationState.Released
                );
        }

        private int GetAdvancingDirection(List<TouchLocation> touchesPressed)
        {
            if (touchesPressed.Count > 0)
            {
                var posTouched = touchesPressed[0].Position;
                if ((posTouched.Y >= 220) && (posTouched.Y <= 270))
                {
                    if ((posTouched.X >= 0) && (posTouched.X <= 50))
                    {
                        return 1;
                    }
                    if ((posTouched.X >= 100) && (posTouched.X <= 150))
                    {
                        return 2;
                    }
                }
            }
            return 0;
        }

        private void Stopping()
        {
            if (Speed.X > 0) Speed += StoppingXRight;
            else if (Speed.X < 0) Speed += StoppingXLeft;
        }

        private void GoLeft()
        {
            if (Speed.X > 0) Speed += new Vector2(-0.5f, 0);
            else
            {
                if (IsJumping)
                {
                    if (Speed.X >= -MaxJumpSpeedX) Speed += new Vector2(-0.5f, 0);
                }
                else if (Speed.X >= -MaxSpeedX)
                {
                    Speed += new Vector2(-1, 0);
                }
            }
        }

        private void GoRight()
        {
            if (Speed.X < 0) Speed = Speed += new Vector2(0.5f, 0);
            else
            {
                if (IsJumping)
                {
                    if (Speed.X <= MaxJumpSpeedX) Speed += new Vector2(0.5f, 0);
                }
                else
                {
                    if (Speed.X <= MaxSpeedX)
                    {
                        Speed += new Vector2(1, 0);
                    }
                }
            }
        }

        public void Jump()
        {
            IsAbleToJump = false;
            IsJumping = true;

            Speed += new Vector2(0, -5.5f);
            Acceleration = new Vector2(0, 0.1f);

            SetCurrentFrame(FrameJumpingSheet, FrameSizeJumping, new Frame(0, 0));
            AnimationTexture = AnimationJumpTexture;
        }

        private void Land()
        {
            IsJumping = false;
            SetCurrentFrame(FrameNormalSheet, FrameSizeNormal, new Frame(0, 0));
            AnimationTexture = AnimationNormalTexture;
        }

        private void SetCurrentFrame(Frame currentSheetFrame, Frame currentFrameSize, Frame positionFrame)
        {
            CyclicalFrameSheet = currentSheetFrame;
            CyclicalFrameSize = currentFrameSize;
            CyclicalFramePosition = positionFrame;
        }
    }
}
