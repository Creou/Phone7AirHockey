using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace PhAirHockey
{
    public enum ActionPlayer
    {
        Player1 = 1,
        Player2 = 2,
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AirHockeyGame : Microsoft.Xna.Framework.Game
    {
        //private bool _flicked = false;

        private int _playTo = 3;

        private SpriteFont _scoreFont;
        private SpriteFont _messageFont;

        private String _messageP1 = "START";
        private String _messageP2 = "START";
        private Vector2 _messageP1Position;
        private Vector2 _messageP2Position;
        private float _messageOpacity = 1;

        private Vector2 _player1ScorePosition;
        private Vector2 _player2ScorePosition;
        

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _player1Score = 1;
        private float _p1ScoreOpacity = 0.8f;
        private int _player2Score = 2;
        private float _p2ScoreOpacity = 0.8f;

        private Texture2D _pitchTexture;
        private Vector2 _pitchPosition;
        private float _pitchScale = 1f;

        private Vector2 _puckPosition;
        private Vector2 _player1Position;
        private Vector2 _player2Position;

        private Texture2D _puckTexture;
        private Texture2D _player1Texture;
        private Texture2D _player2Texture;

        private Vector2 _player1Velocity;
        private Vector2 _player2Velocity;
        private Vector2 _puckVelocity;
        private Vector2 _puckFriction;

        private float _puckScale = 1f;
        private float _p1Scale = 1f;
        private float _p2Scale = 1f;

        private Vector2 _lastCollisionPointHandled;
        private bool withinCollisionZone = false;

        public AirHockeyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            
            _puckFriction = new Vector2(0.005f, 0.005f);

            _pitchPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            _player1ScorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 - 10, 0);
            _player2ScorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 + 10, 0);
            _messageP1Position = new Vector2(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2);
            _messageP2Position = new Vector2(GraphicsDevice.Viewport.Width / 2 + 50, GraphicsDevice.Viewport.Height / 2);
            
            InitialisePuckToStartingConditions();

            _player1Position = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2);
            _player2Position = new Vector2((GraphicsDevice.Viewport.Width / 4) * 3, GraphicsDevice.Viewport.Height / 2);
            
        }

        private void InitialisePuckToStartingConditions()
        {
            _puckVelocity = Vector2.Zero;   
            _puckPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _scoreFont = Content.Load<SpriteFont>("ScoreFont");
            _messageFont = Content.Load<SpriteFont>("MessageFont");

            _pitchTexture = Content.Load<Texture2D>("Pitch");
            _puckTexture = Content.Load<Texture2D>("Puck");
            _player1Texture = Content.Load<Texture2D>("Player1");
            _player2Texture = Content.Load<Texture2D>("Player2");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //if (gameTime.TotalGameTime > new TimeSpan(0, 0, 3) && !_flicked)
            //{
            //    _puckVelocity = new Vector2(1f, 1f);
            //    _flicked = true;
            //}

            HandleInputMultiTouch();
            HandlePuckMovement(gameTime);
            HandlePlayerMovement(gameTime);
            Debug.WriteLine(gameTime.ElapsedGameTime);

            HandlePuckWallCollision();
            HandlePuckPlayerCollision(_player1Position, _player1Velocity);
            HandlePuckPlayerCollision(_player2Position, _player2Velocity);

            ApplyPuckFriction();

            if (_messageOpacity > 0)
            {
                _messageOpacity -= 0.01f;
            }
            if (_p1ScoreOpacity > 0.5)
            {
                _p1ScoreOpacity -= 0.01f;
            }
            if (_p2ScoreOpacity > 0.5)
            {
                _p2ScoreOpacity -= 0.01f;
            }

            CheckForWin();

            base.Update(gameTime);
        }

        private void CheckForWin()
        {
            if (_player1Score >= _playTo)
            {
                GameOver(ActionPlayer.Player1);
            }
            else if (_player2Score >= _playTo)
            {
                GameOver(ActionPlayer.Player2);
            }
        }

        private void GameOver(ActionPlayer actionPlayer)
        {
            switch (actionPlayer)
            {
                case ActionPlayer.Player1:
                    _messageP1 = "WIN !";
                    _messageP2 = "LOSE !";
                    break;
                case ActionPlayer.Player2:
                    _messageP2 = "WIN !";
                    _messageP1 = "LOSE !";
                    break;
                default:
                    break;
            }

            _messageOpacity = 1;
        }

        private void HandlePuckPlayerCollision(Vector2 playerPosition, Vector2 playerVelocity)
        {

            if (withinCollisionZone)
            {
                Vector2 distanceFromLastCollision = (_puckPosition - _lastCollisionPointHandled);
                if (distanceFromLastCollision.Length() < (_player1Texture.Width))
                {
                    return;
                }
                withinCollisionZone = false;
            }

            Vector2 distanceToPlayer = (_puckPosition - playerPosition);

            if (distanceToPlayer.Length() < (_player1Texture.Width))
            {
                _lastCollisionPointHandled = playerPosition;
                withinCollisionZone = true;

                // Calculate the tangent and normals of the collision plane.
                Vector2 collisionNormalVector = new Vector2(playerPosition.X - _puckPosition.X, playerPosition.Y - _puckPosition.Y);
                collisionNormalVector.Normalize();

                Vector2 collisionTangentVector = new Vector2(-collisionNormalVector.Y, collisionNormalVector.X);

                // Calculate prior velocities relative the the collision plane's tangent and normal.
                float velPuckNormal = Vector2.Dot(collisionNormalVector, _puckVelocity);
                float velPuckTangent = Vector2.Dot(collisionTangentVector, _puckVelocity);
                float velPlayerNormal = Vector2.Dot(collisionNormalVector, playerVelocity);
                float velPlayerTangent = Vector2.Dot(collisionTangentVector, playerVelocity);

                float velPuckTangent_After = velPuckTangent;
                float velPlayerTangen_After = velPlayerTangent;

                float puckMass = 1;
                float playerMass = 1000;

                float velPuckNormal_After = ((velPuckNormal * (puckMass - playerMass)) + (2 * playerMass * velPlayerNormal)) / (puckMass + playerMass);
                float velPlayerNormal_After = ((velPlayerNormal * (playerMass - puckMass)) + (2 * puckMass * velPuckNormal)) / (puckMass + playerMass);
                //float v2n_a =

                Vector2 vec_velPuckNormal_After = velPuckNormal_After * collisionNormalVector;
                Vector2 vec_velPuckTangent_After = velPuckTangent_After * collisionTangentVector;
                Vector2 vec_velPlayerNormal_After = velPlayerNormal_After * collisionNormalVector;
                Vector2 vec_velPlayerTangen_After = velPlayerTangen_After * collisionTangentVector;

                Vector2 vec_velPuck_After = vec_velPuckNormal_After + vec_velPuckTangent_After;
                Vector2 vec_velPlayer_After = vec_velPlayerNormal_After + vec_velPlayerTangen_After;

                _puckVelocity = vec_velPuck_After;

                if (_puckVelocity.X > 0)
                {
                    _puckVelocity.X = Math.Min(_puckVelocity.X, 1);
                }
                else
                {
                    _puckVelocity.X = Math.Max(_puckVelocity.X, -1);
                }

                if (_puckVelocity.Y > 0)
                {
                    _puckVelocity.Y = Math.Min(_puckVelocity.Y, 1);
                }
                else
                {
                    _puckVelocity.Y = Math.Max(_puckVelocity.Y, -1);
                }
            }
        }

        private void HandlePuckMovement(GameTime gameTime)
        {
            _puckPosition.X = (float)(_puckPosition.X + (_puckVelocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
            _puckPosition.Y = (float)(_puckPosition.Y + (_puckVelocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));
        }

        private void HandlePlayerMovement(GameTime gameTime)
        {
            _player1Position.X = (float)(_player1Position.X + (_player1Velocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
            _player1Position.Y = (float)(_player1Position.Y + (_player1Velocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));

            _player2Position.X = (float)(_player2Position.X + (_player2Velocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
            _player2Position.Y = (float)(_player2Position.Y + (_player2Velocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));
        }

        private void HandlePuckWallCollision()
        {
            int goalWidth = (GraphicsDevice.Viewport.Height / 2);
            int goalCentre = (GraphicsDevice.Viewport.Height / 2);
            int goalLeft = goalCentre - (goalWidth/2);
            int goalRight = goalCentre + (goalWidth / 2);
            int realativeGoalLeft = goalLeft + (_puckTexture.Width / 2);
            int realativeGoalRight = goalRight - (_puckTexture.Width / 2);

            if (_puckPosition.Y > (GraphicsDevice.Viewport.Height - (_puckTexture.Height / 2)) && _puckVelocity.Y > 0)
            {
                _puckVelocity.Y *= -1;
            }
            if (_puckPosition.Y < 0 + (_puckTexture.Height / 2) && _puckVelocity.Y < 0)
            {
                _puckVelocity.Y *= -1;
            }
            if (_puckPosition.X > (GraphicsDevice.Viewport.Width - (_puckTexture.Width / 2)) && _puckVelocity.X > 0)
            {
                if (_puckPosition.Y > realativeGoalLeft && _puckPosition.Y < realativeGoalRight)
                {
                    GoalScored(ActionPlayer.Player1);
                }
                else
                {
                    _puckVelocity.X *= -1;
                }
            }
            if (_puckPosition.X < 0 + (_puckTexture.Width / 2) && _puckVelocity.X < 0)
            {
                if (_puckPosition.Y > realativeGoalLeft && _puckPosition.Y < realativeGoalRight)
                {
                    GoalScored(ActionPlayer.Player2);
                }
                else
                {
                    _puckVelocity.X *= -1;
                }
            }
        }

        private void GoalScored(ActionPlayer actionPlayer)
        {
            switch (actionPlayer)
            {
                case ActionPlayer.Player1:
                    _player1Score++;
                    _p1ScoreOpacity = 1;
                    break;
                case ActionPlayer.Player2:
                    _player2Score++;
                    _p2ScoreOpacity = 1;
                    break;
                default:
                    Debug.Assert(false, "Unexpected enumeration value");
                    break;
            }

            _messageP1 = "GOAL!";
            _messageP2 = "GOAL!";
            _messageOpacity = 1;

            InitialisePuckToStartingConditions();
        }

        private void ApplyPuckFriction()
        {
            if (_puckVelocity.X < 0) { _puckVelocity.X += _puckFriction.X; } else { _puckVelocity.X -= _puckFriction.X; }
            if (_puckVelocity.Y < 0) { _puckVelocity.Y += _puckFriction.Y; } else { _puckVelocity.Y -= _puckFriction.Y; }
        }

        private void HandleInputMultiTouch()
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            _player1Velocity = Vector2.Zero;
            _player2Velocity = Vector2.Zero;
            foreach (TouchLocation touchLoc in touchCollection)
            {
                if ((touchLoc.State == TouchLocationState.Pressed) || (touchLoc.State == TouchLocationState.Moved))
                {

                    if (touchLoc.Position.X < 400)
                    {
                        _player1Velocity = touchLoc.Position - _player1Position;
                        _player1Velocity = _player1Velocity * 0.01f;

                        _player1Velocity = RestrictMaxPlayerVelocity(_player1Velocity);
                    }
                    else
                    {
                        _player2Velocity = touchLoc.Position - _player2Position;
                        _player2Velocity = _player2Velocity * 0.01f;

                        _player2Velocity = RestrictMaxPlayerVelocity(_player2Velocity);
                    }
                }
            }
        }

        private Vector2 RestrictMaxPlayerVelocity(Vector2 playerVelocity)
        {
            Vector2 restrictedVelocity = new Vector2();
            if (playerVelocity.X > 0)
            {
                restrictedVelocity.X = Math.Min(playerVelocity.X, 1);
            }
            else
            {
                restrictedVelocity.X = Math.Max(playerVelocity.X, -1);
            }

            if (playerVelocity.Y > 0)
            {
                restrictedVelocity.Y = Math.Min(playerVelocity.Y, 1);
            }
            else
            {
                restrictedVelocity.Y = Math.Max(playerVelocity.Y, -1);
            }
            return restrictedVelocity;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw the background texture
            _spriteBatch.Begin();

            DrawPitch();
            DrawPlayers();
            DrawPuck();
            DrawScores();

            Vector2 messageP1Size = _messageFont.MeasureString(_messageP1);
            Vector2 messageP2Size = _messageFont.MeasureString(_messageP2);

            _spriteBatch.DrawString(_messageFont, _messageP1, _messageP1Position, Color.White * _messageOpacity, MathHelper.ToRadians(90), new Vector2(messageP1Size.X / 2, messageP1Size.Y / 2), 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_messageFont, _messageP2, _messageP2Position, Color.White * _messageOpacity, MathHelper.ToRadians(270), new Vector2(messageP2Size.X / 2, messageP2Size.Y / 2), 1, SpriteEffects.None, 0);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScores()
        {
            Vector2 size = _scoreFont.MeasureString(_player1Score.ToString());
            Vector2 player1ActualScorePosition = new Vector2(_player1ScorePosition.X - size.X, _player1ScorePosition.Y);
            _spriteBatch.DrawString(_scoreFont, _player1Score.ToString(), player1ActualScorePosition, Color.White * _p1ScoreOpacity);

            _spriteBatch.DrawString(_scoreFont, _player2Score.ToString(), _player2ScorePosition, Color.White * _p2ScoreOpacity);            
        }

        private void DrawPitch()
        {
            _spriteBatch.Draw(_pitchTexture, _pitchPosition, null, Color.White, MathHelper.ToRadians(90), new Vector2(_pitchTexture.Width / 2, _pitchTexture.Height / 2), _pitchScale, SpriteEffects.None, 0);
        }

        private void DrawPuck()
        {
            _spriteBatch.Draw(_puckTexture, _puckPosition, null, Color.White, 0, new Vector2(_puckTexture.Width / 2, _puckTexture.Height / 2), _puckScale, SpriteEffects.None, 0);            
        }

        private void DrawPlayers()
        {
            _spriteBatch.Draw(_player1Texture, _player1Position, null, Color.White, 0, new Vector2(_player1Texture.Width / 2, _player1Texture.Height / 2), _p1Scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(_player2Texture, _player2Position, null, Color.White, 0, new Vector2(_player2Texture.Width / 2, _player2Texture.Height / 2), _p2Scale, SpriteEffects.None, 0);
        }
    }
}
