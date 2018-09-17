using System;
using System.Collections.Generic;
using System.Linq;
using DragonsAndSharks.Contracts;
using DragonsAndSharks.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;


namespace DragonsAndSharks.Levels
{
    /// <summary>
    /// Componente de juego que implementa IUpdateable.
    /// </summary>
    public class LevelComponent : Microsoft.Xna.Framework.DrawableGameComponent, ISpriteManageable
    {
        public SpriteManager Spriter { get; set; }

        public LevelComponent(Game game, SpriteManager spriter)
            : base(game)
        {
            Spriter = spriter;

            CreatePlatforms();
            CreateUser();
            CreateDragon();
        }

        /// <summary>
        /// Permite al componente de juego realizar la inicialización necesaria antes de empezar a
        /// ejecutarse. En este punto puede consultar los servicios necesarios y cargar contenido.
        /// </summary>
        public override void Initialize()
        {
            

            base.Initialize();
        }

        /// <summary>
        /// Permite al componente de juego actualizarse.
        /// </summary>
        /// <param name="gameTime">Proporciona una instantánea de los valores de tiempo.</param>
        public override void Update(GameTime gameTime)
        {
            Spriter.Update(gameTime);

            base.Update(gameTime);
        }

        public void SetBackground(Texture2D background)
        {
            Spriter.SetBackgroundLevel(background);
        }

        private void CreatePlatforms()
        {
            Spriter.CreatePlatforms();
        }

        private void CreateDragon()
        {
            Spriter.CreateDragon();
        }

        private void CreateUser()
        {
            Spriter.CreateUser();
        }
    }
}
