namespace DragonsAndSharks.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteManager : DrawableGameComponent
    {
        #region Const

        private const int ScreenChangeRightX = 350;
        private const int ScreenChangeLeftX = 250;

        #endregion

        #region Enumeration

        private enum CollisionMethodTypeEnumeration
        {
            MinimumDistanceOfGreatestVector,
            VectorSubstraction
        }

        #endregion

        #region Delegates

        private delegate Tuple<Vector2, bool> CollisionMethod(SpriteBase collisioner, SpriteBase collisioned);

        private CollisionMethod collisionMethod;

        #endregion

        #region Private fields

        private readonly Tuple<Vector2, bool> _tupleZero = new Tuple<Vector2, bool>(Vector2.Zero, false);
        private readonly Random _random = new Random();
        private readonly List<SpriteBase> _spriteList;
        private readonly SpriteBatch _spriteBatch;
        private readonly ContentManager _content;
        private readonly Rectangle _clientBounds;
        private Vector2 _screenChange = Vector2.Zero;
        public SpriteUser SpriteUser { get; set; }
        private SpriteStatic _btnLeft, _btnRight, _btnJump;
        private readonly List<Texture2D> _backgrounds = new List<Texture2D>();
        private readonly List<Rectangle> _rangeBackgrounds = new List<Rectangle>
        {
            new Rectangle(0, 0, 2048, 480)
        };
        private float _layoutTo = 1600;
        private float _layoutPrevious = -1600;

        private CollisionMethodTypeEnumeration collisionMethodType;

        #endregion

        #region Constructor

        public SpriteManager(Game game, SpriteBatch spriteBatch, ContentManager content, Rectangle clientBounds)
            : base(game)
        {
            _spriteBatch = spriteBatch;
            _content = content;
            _clientBounds = clientBounds;

            _spriteList = new List<SpriteBase>();

            collisionMethodType = CollisionMethodTypeEnumeration.MinimumDistanceOfGreatestVector;
            SetCollisionMethod(CollisionByMinimumDistanceOfGreatestVector);

            SetControlSprites();
        }

        private void SetCollisionMethod(CollisionMethod _collisionMethod)
        {
            collisionMethod = _collisionMethod;
        }

        #endregion

        #region Public methods

        public override void Update(GameTime gameTime)
        {
            // Get screen adjustament relative to the position
            _screenChange = ScreenAdjustmentToPlayer();
            SetRangeBackground();

            SpriteUser.Update(gameTime, _clientBounds, _screenChange);

            foreach (var sprite in _spriteList)
            {
                if (sprite is SpriteStatic)
                {
                    sprite.Update(gameTime, _clientBounds, _screenChange);

                    var resultVector = HandleCollision(SpriteUser, sprite);

                    if (!resultVector.Item1.Equals(Vector2.Zero))
                    {
                        if (resultVector.Item2) SpriteUser.IsAbleToJump = true;

                        SpriteUser.Position += resultVector.Item1;
                        SpriteUser.Speed += resultVector.Item1;
                    }
                }
                else if (sprite is SpriteDragon)
                {
                    sprite.Update(gameTime, _clientBounds, _screenChange);
                }
            }

            base.Update(gameTime);
        }

        private void SetRangeBackground()
        {
            if (!_screenChange.Equals(Vector2.Zero))
            {
                var rangeBackground =
                    _rangeBackgrounds[_rangeBackgrounds.Count - 1] =
                        new Rectangle((int) (_rangeBackgrounds[_rangeBackgrounds.Count - 1].X - SpriteUser.Speed.X), 0,
                            2048, 480);

                if (rangeBackground.X >= _layoutTo)
                {
                    _rangeBackgrounds.Add(new Rectangle(rangeBackground.X, 0, 2048, 480));
                    _rangeBackgrounds.Remove(_rangeBackgrounds.FirstOrDefault());
                    _layoutTo += 1600;
                }
                else if (rangeBackground.X <= _layoutPrevious)
                {
                    _rangeBackgrounds.Add(new Rectangle(rangeBackground.X - 1600, 0, 2048, 480));
                    _rangeBackgrounds.Remove(_rangeBackgrounds.FirstOrDefault());
                    _layoutTo = _layoutPrevious;
                    _layoutPrevious -= 1600;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            DrawContent(gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawContent(GameTime gameTime)
        {
            if (_backgrounds.Any())
            {
                _spriteBatch.Draw(_backgrounds.LastOrDefault(), _rangeBackgrounds.LastOrDefault(), Color.White);
            }

            foreach (var sprite in _spriteList)
            {
                sprite.Draw(gameTime, _spriteBatch);
            }

            SpriteUser.Draw(gameTime, _spriteBatch);
            _btnLeft.Draw(gameTime, _spriteBatch);
            _btnRight.Draw(gameTime, _spriteBatch);
            _btnJump.Draw(gameTime, _spriteBatch);
        }

        public void SetBackgroundLevel(Texture2D texture2D)
        {
            _backgrounds.Add(texture2D);
        }

        public void CreateDragon()
        {
            var rand = _random.Next(500, 600);
            _spriteList.Add(
                new SpriteDragon(
                    _content.Load<Texture2D>(@"Sprites/DragonSpriteStatic"),
                    _content.Load<Texture2D>(@"Sprites/DragonSpriteAnimated"),
                    new Vector2(rand, 5),
                    new Vector2(-2, 0),
                    new Frame(3, 2),
                    new Frame(129, 105),
                    new Frame(0, 0),
                    0)
                );
        }

        public void CreateUser()
        {
            SpriteUser = new SpriteUser(
                _content.Load<Texture2D>(@"Sprites/UserSpriteStatic"),
                _content.Load<Texture2D>(@"Sprites/UserSpriteAnimated"),
                _content.Load<Texture2D>(@"Sprites/UserSpriteJumpAnimated"),
                new Vector2(10, 0),
                new Vector2(14, 0),
                new Frame(6, 1),
                new Frame(47, 51),
                new Frame(0, 0),
                new Frame(6, 1),
                new Frame(33, 33),
                100)
            {
                Acceleration = new Vector2(0, 0.1f)
            };
        }

        public void CreatePlatforms()
        {
            _spriteList.AddRange(
                new List<SpriteBase>
                {
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(0, 440),
                        new Frame(3300, 150))
                    ,
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(300, 330),
                        new Frame(75, 150)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(375, 390),
                        new Frame(50, 150)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(450, 280),
                        new Frame(300, 150)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(675, 200),
                        new Frame(125, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(975, 400),
                        new Frame(125, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(1175, 350),
                        new Frame(125, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(1275, 300),
                        new Frame(125, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(1400, 230),
                        new Frame(175, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(1500, 310),
                        new Frame(250, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(1650, 100),
                        new Frame(400, 50)
                        ),
                    new SpriteStatic(
                        _content.Load<Texture2D>(@"Sprites/wallTexture"),
                        new Vector2(2100, 200),
                        new Frame(5500, 50)
                        )
                });
        }

        #endregion

        #region Private methods

        private Vector2 ScreenAdjustmentToPlayer()
        {
            var adjustment = Vector2.Zero;

            if ((SpriteUser.Position.X >= ScreenChangeRightX && SpriteUser.Speed.X > 0)
             || (SpriteUser.Position.X <= ScreenChangeLeftX && SpriteUser.Speed.X < 0))
            {
                adjustment = -new Vector2(SpriteUser.Speed.X, 0);
            }

            return adjustment;
        }

        private void SetControlSprites()
        {
            _btnLeft = new SpriteStatic(_content.Load<Texture2D>("Sprites/buttonLeft"), new Vector2(0, 220), new Frame(50, 50));

            _btnRight = new SpriteStatic(_content.Load<Texture2D>("Sprites/buttonRight"), new Vector2(100, 220), new Frame(50, 50));

            _btnJump = new SpriteStatic(_content.Load<Texture2D>("Sprites/buttonUp"), new Vector2(700, 380), new Frame(100, 100));
        }

        private Tuple<Vector2, bool> HandleCollision(SpriteBase spriteUser, SpriteBase sprite)
        {
            return Collision(spriteUser, sprite);
        }

        private Tuple<Vector2, bool> Collision(SpriteBase collisioner, SpriteBase collisioned)
        {
            return collisionMethod.Invoke(collisioner, collisioned);
        }

        private Tuple<Vector2, bool> CollisionByVectorSubstraction(SpriteBase collisioner, SpriteBase collisioned)
        {
            var x1 = collisioner.Position.X;
            var y1 = collisioner.Position.Y;
            var w1 = collisioner.CyclicalFrameSize.X;
            var h1 = collisioner.CyclicalFrameSize.Y;

            var x2 = collisioned.Position.X;
            var y2 = collisioned.Position.Y;
            var w2 = collisioned.Size.X;
            var h2 = collisioned.Size.Y;

            if ((y1 <= y2 && y1 + h1 > y2)
                || (y1 >= y2 && y1 <= y2 + h2)
                || (y1 >= y2 && y1 >= y2 + h2))
            {
                if (x1 > x2 && x1 + w1 < x2 + w2)
                    return new Tuple<Vector2, bool>(new Vector2(0, -collisioner.Speed.Y), true);

                double dx = x1 <= x2 && x1 + w1 > x2
                    ? Math.Abs(x1 + w1 - x2)
                    : x1 > x2 && x1 < x2 + w2
                        ? Math.Abs(x1 - x2 + w2)
                        : 0;

                if (dx.Equals(0)) return _tupleZero;

                double dy = y1 >= y2 ? Math.Abs(y1 - y2) : Math.Abs(y1 - y2 + h2);

                return dx >= dy
                    ? new Tuple<Vector2, bool>(new Vector2(-collisioner.Speed.X, 0), false)
                    : new Tuple<Vector2, bool>(new Vector2(0, -collisioner.Speed.Y), true);
            }
            return _tupleZero;
        }

        private Tuple<Vector2, bool> CollisionByMinimumDistanceOfGreatestVector(
            SpriteBase collisioner,
            SpriteBase collisioned)
        {
            var x1 = (int)collisioner.Position.X;
            var y1 = (int)collisioner.Position.Y;
            var w1 = (int)collisioner.CyclicalFrameSize.X;
            var h1 = (int)collisioner.CyclicalFrameSize.Y;

            var x2 = (int)collisioned.Position.X;
            var y2 = (int)collisioned.Position.Y;
            var w2 = (int)collisioned.Size.X;
            var h2 = (int)collisioned.Size.Y;

            var collisionVector = GetGreatestVector(collisioned, x1, w1, y1, h1, x2, y2);

            if (!collisionVector.Equals(Vector2.Zero))
            {
                var componentX = Math.Abs(collisionVector.X);
                var componentY = Math.Abs(collisionVector.Y);

                if (componentX < componentY)
                {
                    return new Tuple<Vector2, bool>(-new Vector2(componentX, 0), false);
                }
                else if (componentX.Equals(componentY))
                {
                    return new Tuple<Vector2, bool>(-new Vector2(componentX, componentY), false);
                }
                else
                {
                    return new Tuple<Vector2, bool>(-new Vector2(0, componentY), true);

                }
            }
            return new Tuple<Vector2, bool>(Vector2.Zero, false);
        }

        private Vector2 GetGreatestVector(SpriteBase collisioned, int x1, int w1, int y1, int h1, int x2, int y2)
        {
            var vector = new Tuple<Vector2, double>(Vector2.Zero, 0);

            for (int i = 0; i < 4; i++)
            {
                var x1PlusW1 = x1 + w1;
                var y1PlusH1 = y1 + h1;

                switch (i)
                {
                    case 0:
                    {
                        if (collisioned.RectangleCollision.Contains(x1, y1))
                        {
                            var candidate = GetVectorFromPointToPoint(x1, y1, x2, y2);
                            if (candidate.Item2 > vector.Item2)
                            {
                                vector.Item1 = candidate.Item1;
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        if (collisioned.RectangleCollision.Contains(x1PlusW1, y1))
                        {
                            var candidate = GetVectorFromPointToPoint(x1PlusW1, y1, x2, y2);
                            if (candidate.Item2 > vector.Item2)
                            {
                                vector.Item1 = candidate.Item1;
                            }
                        }
                        break;
                    }
                    case 2:
                    {
                        if (collisioned.RectangleCollision.Contains(x1, y1PlusH1))
                        {
                            var candidate = GetVectorFromPointToPoint(x1, y1PlusH1, x2, y2);
                            if (candidate.Item2 > vector.Item2)
                            {
                                vector.Item1 = candidate.Item1;
                            }
                        }
                        break;
                    }
                    case 3:
                    {
                        if (collisioned.RectangleCollision.Contains(x1PlusW1, y1PlusH1))
                        {
                            var candidate = GetVectorFromPointToPoint(x1PlusW1, y1PlusH1, x2, y2);
                            if (candidate.Item2 > vector.Item2)
                            {
                                vector.Item1 = candidate.Item1;
                            }
                        }
                        break;
                    }
                }
            }
            return vector.Item1;
        }

        private Tuple<Vector2, double> GetVectorFromPointToPoint(int x1, int y1, int x2, int y2)
        {
            var x = x2 - x1;
            var y = y2 - y1;
            return new Tuple<Vector2, double>
                (new Vector2(x, y), Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2)));
        }

        #endregion
    }
}
