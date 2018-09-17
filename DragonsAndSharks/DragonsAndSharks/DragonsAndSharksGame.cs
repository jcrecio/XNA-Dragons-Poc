namespace DragonsAndSharks
{
    using System;
    using DragonsAndSharks.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using DragonsAndSharks.Levels;
    using DragonsAndSharks.Sprites;

    public class DragonsAndSharksGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private SpriteManager spriter;

        private GameStateManager gameStateManager;
        private GameState state;

        private DrawableGameComponent menuComponent;
        private DrawableGameComponent levelComponent;
        private DrawableGameComponent gameOverComponent;
        private DrawableGameComponent scoreComponent;
        private DrawableGameComponent optionsComponent;

        private DrawableGameComponent currentGameComponent;

        public DragonsAndSharksGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromTicks(333333);

            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            spriter = new SpriteManager(this, _spriteBatch, Content, new Rectangle());
            Components.Add(spriter);

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.IsFullScreen = true;
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeRight;
            _graphics.ApplyChanges();

            state = GameState.Level;

            menuComponent = new MenuComponent(this);
            gameOverComponent = new MenuComponent(this);

            levelComponent = new LevelComponent(this, spriter);
            var lvlComponent = levelComponent as LevelComponent;
            lvlComponent.SetBackground(Content.Load<Texture2D>("Sprites/background"));

            scoreComponent = new ScoreComponent(this);
            optionsComponent = new OptionsComponent(this);

            Components.Add(menuComponent);
            Components.Add(gameOverComponent);
            Components.Add(levelComponent);
            Components.Add(scoreComponent);
            Components.Add(optionsComponent);

            gameStateManager = GameStateManager.GetInstance();
            gameStateManager.SetGameState(state);
            currentGameComponent = levelComponent;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            state = gameStateManager.GetGameState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                if(state.Equals(GameState.Menu)) Exit();
                else if(state.Equals(GameState.Level)) gameStateManager.SetGameState(GameState.Menu);
            }
            switch (state)
                {
                    case GameState.Level:
                        if (currentGameComponent != levelComponent)
                        {
                            currentGameComponent.Enabled = false;
                            currentGameComponent.Visible = false;
                            levelComponent.Enabled = true;
                            levelComponent.Visible = true;

                            currentGameComponent = levelComponent;
                        }
                        
                        break;

                    case GameState.Options:
                        if (currentGameComponent != optionsComponent)
                        {
                            currentGameComponent.Enabled = false;
                            currentGameComponent.Visible = false;
                            optionsComponent.Enabled = true;
                            optionsComponent.Visible = true;

                            currentGameComponent = optionsComponent;
                        }

                        break;

                    case GameState.Scores:
                        if (currentGameComponent != scoreComponent)
                        {
                            currentGameComponent.Enabled = false;
                            currentGameComponent.Visible = false;
                            scoreComponent.Enabled = true;
                            scoreComponent.Visible = true;

                            currentGameComponent = scoreComponent;
                        }
                        break;

                    case GameState.GameOver:
                        if (currentGameComponent != gameOverComponent)
                        {
                            currentGameComponent.Enabled = false;
                            currentGameComponent.Visible = false;
                            gameOverComponent.Enabled = true;
                            gameOverComponent.Visible = true;

                            currentGameComponent = gameOverComponent;
                        }
                        break;
                }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }

        private void SetComponentAs(IGameComponent gameComponent, bool value)
        {
            currentGameComponent.Enabled = false;
            if (currentGameComponent != null)
            {
                SetDrawableComponentAs((DrawableGameComponent)currentGameComponent, value);
            }

            var component = (GameComponent) gameComponent;
            component.Enabled = value;

            var drawableGameComponent = gameComponent as DrawableGameComponent;
            if (drawableGameComponent != null)
            {
                SetDrawableComponentAs(drawableGameComponent, value);
            }
        }

        private void SetDrawableComponentAs(DrawableGameComponent gameComponent, bool value)
        {
            gameComponent.Visible = value;
        }
    }
}
