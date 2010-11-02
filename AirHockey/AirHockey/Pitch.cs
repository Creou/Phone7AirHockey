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
    public class Pitch : DrawableGameComponent
    {
        private SpriteBatch _pitchSpriteBatch;

        private Texture2D _pitchTexture;
        private Vector2 _pitchPosition;
        private float _pitchScale = 1f;

        public Pitch(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public AirHockeyGame AHGame { get { return this.Game as AirHockeyGame; } }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            // Create a sprite batch for the menu.
            _pitchSpriteBatch = new SpriteBatch(GraphicsDevice);

            _pitchPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        }

        protected override void LoadContent()
        {
            _pitchTexture = this.Game.Content.Load<Texture2D>("Pitch");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.AHGame.GameMode == GameMode.Game)
            {
                _pitchSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                _pitchSpriteBatch.Draw(_pitchTexture, _pitchPosition, null, Color.White * this.AHGame.GameOpacity, MathHelper.ToRadians(90), new Vector2(_pitchTexture.Width / 2, _pitchTexture.Height / 2), _pitchScale, SpriteEffects.None, 1);

                _pitchSpriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
