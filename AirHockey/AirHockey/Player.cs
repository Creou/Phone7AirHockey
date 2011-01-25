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
using CollisionLib;


namespace AirHockey
{
    public enum PlayerNumber 
    {
        Player1 = 1,
        Player2 = 2
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : DrawableGameComponent, ICollidableCircle
    {
        private Texture2D _playerTexture;
        private Vector2 _playerPosition;
        private Vector2 _playerVelocity;
        private float _playerScale = 1f;
        private PlayerNumber _playerNumber;

        private SpriteBatch _spriteBatch;

        private AirHockeyGame _game;

        public Player(Game game, PlayerNumber playerNumber)
            : base(game)
        {
            _playerNumber = playerNumber;
            _game = (AirHockeyGame)game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            switch (_playerNumber)
            {
                case PlayerNumber.Player1:
                    _playerTexture = this.Game.Content.Load<Texture2D>("Player1");
                    break;

                case PlayerNumber.Player2:
                    _playerTexture = this.Game.Content.Load<Texture2D>("Player2");
                    break;

                default:
                    break;
            }

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            HandlePlayerMovement(gameTime);

            base.Update(gameTime);
        }

        private void HandlePlayerMovement(GameTime gameTime)
        {
            _playerPosition.X = (float)(_playerPosition.X + (_playerVelocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
            _playerPosition.Y = (float)(_playerPosition.Y + (_playerVelocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));

            //_player2Position.X = (float)(_player2Position.X + (_player2Velocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
            //_player2Position.Y = (float)(_player2Position.Y + (_player2Velocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));
        }

        public override void Draw(GameTime gameTime)
        {
            DrawPlayer();

            base.Draw(gameTime);
        }

        private void DrawPlayer()
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_playerTexture, _playerPosition, null, Color.White * _game.GameOpacity, 0, new Vector2(_playerTexture.Width / 2, _playerTexture.Height / 2), _playerScale, SpriteEffects.None, 0);

            _spriteBatch.End();
            //_spriteBatch.Draw(_player2Texture, _player2Position, null, Color.White * _gameOpacity, 0, new Vector2(_player2Texture.Width / 2, _player2Texture.Height / 2), _p2Scale, SpriteEffects.None, 0);
        }

        public float Diameter
        {
            get { return _playerTexture.Width; }
        }

        public void SetState(Vector2 position, Vector2 velocity)
        {
            _playerPosition = position;
            _playerVelocity = velocity;
        }

        public Vector2 Position
        {
            get { return _playerPosition; }
        }

        public Vector2 Velocity
        {
            get { return _playerVelocity; }
        }
    }
}
