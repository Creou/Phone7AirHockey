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
using AirHockey.Library;
using Microsoft.Xna.Framework.Input.Touch;


namespace AirHockey
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : DrawableGameComponent
    {
        private SpriteBatch _menuSpriteBatch;
        private SpriteFont _menuFont;

        private Vector2 _menuTitlePosition;
        private Vector2 _menuOption_1Player_Position;
        private Vector2 _menuOption_2Player_Position;

        private bool _menuStateChanging = false;
        private GameMode _targetStateChange;
        private float _menuOpacity = 1;

        public MainMenu(Game game)
            : base(game)
        {
        }

        public AirHockeyGame AHGame { get { return this.Game as AirHockeyGame; } }

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

            _menuTitlePosition = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2);
            _menuOption_1Player_Position = new Vector2(GraphicsDevice.Viewport.Width / 4 + 150, GraphicsDevice.Viewport.Height / 2);
            _menuOption_2Player_Position = new Vector2(GraphicsDevice.Viewport.Width / 4 + 250, GraphicsDevice.Viewport.Height / 2);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (AHGame.GameMode == GameMode.Menu)
            {
                HandleMenuInput();
                HandleMenuStateChange();
            }

            base.Update(gameTime);
        }

        private void HandleMenuStateChange()
        {
            if (_menuStateChanging && _menuOpacity > 0)
            {
                _menuOpacity -= 0.05f;
            }
            else if (_menuStateChanging && _menuOpacity <= 0)
            {
                _menuStateChanging = false;
                this.AHGame.NewGame();
            }
        }

        private void HandleMenuInput()
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation touchLoc in touchCollection)
            {
                if ((touchLoc.State == TouchLocationState.Pressed))
                {
                    Vector2 menuOption_2Player_TextSize = _menuFont.MeasureString(Strings.MainMenu_2Player);
                    Vector2 topLeft = new Vector2(_menuOption_2Player_Position.X - menuOption_2Player_TextSize.Y / 2, _menuOption_2Player_Position.Y - menuOption_2Player_TextSize.X / 2);
                    Vector2 bottomRight = new Vector2(_menuOption_2Player_Position.X + menuOption_2Player_TextSize.Y / 2, _menuOption_2Player_Position.Y + menuOption_2Player_TextSize.X / 2);

                    if (touchLoc.Position.X > topLeft.X && touchLoc.Position.X < bottomRight.X &&
                        touchLoc.Position.Y > topLeft.Y && touchLoc.Position.Y < bottomRight.Y)
                    {
                        _menuStateChanging = true;
                        _targetStateChange = GameMode.Game;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.AHGame.GameMode == GameMode.Menu)
            {
                _menuSpriteBatch.Begin();

                Vector2 menuTextSize = _menuFont.MeasureString(Strings.MainMenu_Title);
                _menuSpriteBatch.DrawString(_menuFont, Strings.MainMenu_Title, _menuTitlePosition, Color.White * _menuOpacity, MathHelper.ToRadians(270), menuTextSize.OriginVector(), 1, SpriteEffects.None, 0);

                Vector2 menuOption_1Player_TextSize = _menuFont.MeasureString(Strings.MainMenu_1Player);
                _menuSpriteBatch.DrawString(_menuFont, Strings.MainMenu_1Player, _menuOption_1Player_Position, Color.White * _menuOpacity * 0.2f, MathHelper.ToRadians(270), menuOption_1Player_TextSize.OriginVector(), 0.8f, SpriteEffects.None, 0);

                Vector2 menuOption_2Player_TextSize = _menuFont.MeasureString(Strings.MainMenu_2Player);
                _menuSpriteBatch.DrawString(_menuFont, Strings.MainMenu_2Player, _menuOption_2Player_Position, Color.White * _menuOpacity, MathHelper.ToRadians(270), menuOption_2Player_TextSize.OriginVector(), 0.8f, SpriteEffects.None, 0);
                _menuSpriteBatch.End();
            }
            
            base.Draw(gameTime);
        }
    }
}
