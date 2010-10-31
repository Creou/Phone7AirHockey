using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AirHockey
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : DrawableGameComponent
    {
        private SpriteBatch _menuSpriteBatch;
        private SpriteFont _menuFont;

        public MainMenu(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Create a sprite batch for the menu.
            _menuSpriteBatch = new SpriteBatch(GraphicsDevice);

            _menuFont = Game.Content.Load<SpriteFont>("MenuFont");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public AirHockeyGame AHGame { get { return this.Game as AirHockeyGame; } }

        public override void Draw(GameTime gameTime)
        {
            if (this.AHGame.GameMode == GameMode.Menu)
            {
                _menuSpriteBatch.Begin();
                _menuSpriteBatch.DrawString(_menuFont, "Menu Test Text", Vector2.Zero, Color.White);
                _menuSpriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
