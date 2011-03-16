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
using CollisionLib;
using System.Collections.ObjectModel;

namespace AirHockey
{
    public enum ActionPlayer
    {
        Player1 = 1,
        Player2 = 2,
    }

    public enum GameMode { Menu = 0, Game = 1 }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AirHockeyGame : Microsoft.Xna.Framework.Game
    {
        public GameMode GameMode { get; set; }

        //private bool _flicked = false;

        private int _playTo = 3;

        List<SoundEffect> _bounceEffects;

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

        private int _player1Score;
        private float _p1ScoreOpacity = 0.8f;
        private int _player2Score;
        private float _p2ScoreOpacity = 0.8f;

        //private Vector2 _puckPosition;
        //private Vector2 _player1Position;
        //private Vector2 _player2Position;

        //private Texture2D _puckTexture;
        //private Texture2D _player1Texture;
        //private Texture2D _player2Texture;

        //private Vector2 _player1Velocity;
        //private Vector2 _player2Velocity;
        //private Vector2 _puckVelocity;
        //private Vector2 _puckFriction;

        private CollisionManager _collisionManager;

        private Player _player1;
        private Player _player2;
        private Puck _puck;

        private float _puckScale = 1f;

        private float _gameOpacity = 0;

        public float GameOpacity
        {
            get { return _gameOpacity; }
        }

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

            var menu = new MainMenu(this);
            this.Components.Add(menu);

            var pitch = new Pitch(this);
            this.Components.Add(pitch);            

            GameMode = AirHockey.GameMode.Menu;
            
            _player1 = new Player(this, PlayerNumber.Player1);
            _player2 = new Player(this, PlayerNumber.Player2);
            _puck = new Puck(this);

            var collideables = new Collection<ICollidable>();
            collideables.Add(_player1);
            collideables.Add(_player2);
            collideables.Add(_puck);
            _collisionManager = new CollisionManager(this, collideables);

            this.Components.Add(_player1);
            this.Components.Add(_player2);
            this.Components.Add(_puck);

            _player1ScorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 - 10, 0);
            _player2ScorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 + 10, 0);
            _messageP1Position = new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height / 2);
            _messageP2Position = new Vector2(GraphicsDevice.Viewport.Width / 2 + 100, GraphicsDevice.Viewport.Height / 2);

            InitialiseToNewGameState();

            base.Initialize();
        }

        internal void NewGame()
        {
            InitialiseToNewGameState();
            _gameOpacity = 0;
            GameMode = AirHockey.GameMode.Game;
        }

        private void InitialiseToNewGameState()
        {
            _gameOver = false;

            _player1Score = 0;
            _player2Score = 0;

            InitialisePuckToStartingConditions();

            _player1.SetState(new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2), Vector2.Zero);
            _player2.SetState(new Vector2((GraphicsDevice.Viewport.Width / 4) * 3, GraphicsDevice.Viewport.Height / 2), Vector2.Zero);
            //_player1Position = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2);
            //_player2Position = new Vector2((GraphicsDevice.Viewport.Width / 4) * 3, GraphicsDevice.Viewport.Height / 2);
        }

        private void InitialisePuckToStartingConditions()
        {
            _puck.SetState(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2),  Vector2.Zero);
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

            //_puckTexture = Content.Load<Texture2D>("Puck");
            //_player1Texture = Content.Load<Texture2D>("Player1");
            //_player2Texture = Content.Load<Texture2D>("Player2");

            _bounceEffects = new List<SoundEffect>();
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce1"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce2"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce3"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce4"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce5"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce6"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce7"));
            _bounceEffects.Add(Content.Load<SoundEffect>("Bounce8"));

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
            ButtonState backButton = GamePad.GetState(PlayerIndex.One).Buttons.Back;
            if (backButton == ButtonState.Pressed)
            {
                if (GameMode == AirHockey.GameMode.Menu)
                {
                    this.Exit();
                }
                else if (GameMode == AirHockey.GameMode.Game) 
                {
                    _gameOver = true;
                    _gameOverTime = gameTime.TotalGameTime - new TimeSpan(0, 0, 2);
                }
            }

            if (GameMode == AirHockey.GameMode.Game)
            {
                if (!_gameOver && _gameOpacity < 1)
                {
                    _gameOpacity += 0.05f;
                }
                else if (_gameOver && (gameTime.TotalGameTime - _gameOverTime) > new TimeSpan(0,0,2))
                {
                    if (_gameOpacity > 0)
                    {
                        _gameOpacity -= 0.05f;
                    }
                    else 
                    {
                        GameMode = AirHockey.GameMode.Menu;
                    }
                }

                //HandlePlayerInput();
                //HandlePuckMovement(gameTime);
                //HandlePlayerMovement(gameTime);

                _collisionManager.ApplyCollisions(gameTime.ElapsedGameTime);
                //HandlePuckWallCollision();
                //HandlePuckPlayerCollision(_player1Position, _player1Velocity);
                //HandlePuckPlayerCollision(_player2Position, _player2Velocity);

                //ApplyPuckFriction();

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

                CheckForWin(gameTime);
            }

            base.Update(gameTime);
        }

        private void CheckForWin(GameTime gameTime)
        {
            if (!_gameOver)
            {
                if (_player1Score >= _playTo)
                {
                    GameOver(ActionPlayer.Player1, gameTime);
                }
                else if (_player2Score >= _playTo)
                {
                    GameOver(ActionPlayer.Player2, gameTime);
                }
            }
        }

        private bool _gameOver;
        private TimeSpan _gameOverTime;

        private void GameOver(ActionPlayer actionPlayer, GameTime gameTime)
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
            _gameOver = true;
            _gameOverTime = gameTime.TotalGameTime;
        }

        //private void HandlePuckPlayerCollision(Vector2 playerPosition, Vector2 playerVelocity)
        //{
        //    if (!_gameOver)
        //    {
        //        Vector2 distanceToPlayer = (_puckPosition - playerPosition);

        //        float collisionDistance = (_player1Texture.Width / 2f) + (_puckTexture.Width / 2f);

        //        if (distanceToPlayer.Length() < collisionDistance)
        //        {
        //            PlayPuckCollisionEffect();

        //            // First move the puck out to the collision point. 
        //            // (This is because sometimes the collision doesn't get
        //            // processed until after the puck has moved inside the 
        //            // player, so what we do is wind back to the point of 
        //            // contact.)
        //            Vector2 normalizedCollisionVector = distanceToPlayer;
        //            normalizedCollisionVector.Normalize();
        //            _puckPosition = playerPosition + (normalizedCollisionVector * collisionDistance);

        //            // Calculate the tangent and normals of the collision plane.
        //            Vector2 collisionNormalVector = new Vector2(playerPosition.X - _puckPosition.X, playerPosition.Y - _puckPosition.Y);
        //            collisionNormalVector.Normalize();

        //            Vector2 collisionTangentVector = new Vector2(-collisionNormalVector.Y, collisionNormalVector.X);

        //            // Calculate prior velocities relative the the collision plane's tangent and normal.
        //            float velPuckNormal = Vector2.Dot(collisionNormalVector, _puckVelocity);
        //            float velPuckTangent = Vector2.Dot(collisionTangentVector, _puckVelocity);
        //            float velPlayerNormal = Vector2.Dot(collisionNormalVector, playerVelocity);
        //            float velPlayerTangent = Vector2.Dot(collisionTangentVector, playerVelocity);

        //            float velPuckTangent_After = velPuckTangent;
        //            float velPlayerTangen_After = velPlayerTangent;

        //            float puckMass = 1;
        //            float playerMass = 1000;

        //            float velPuckNormal_After = ((velPuckNormal * (puckMass - playerMass)) + (2 * playerMass * velPlayerNormal)) / (puckMass + playerMass);
        //            float velPlayerNormal_After = ((velPlayerNormal * (playerMass - puckMass)) + (2 * puckMass * velPuckNormal)) / (puckMass + playerMass);
        //            //float v2n_a =

        //            Vector2 vec_velPuckNormal_After = velPuckNormal_After * collisionNormalVector;
        //            Vector2 vec_velPuckTangent_After = velPuckTangent_After * collisionTangentVector;
        //            Vector2 vec_velPlayerNormal_After = velPlayerNormal_After * collisionNormalVector;
        //            Vector2 vec_velPlayerTangen_After = velPlayerTangen_After * collisionTangentVector;

        //            Vector2 vec_velPuck_After = vec_velPuckNormal_After + vec_velPuckTangent_After;
        //            Vector2 vec_velPlayer_After = vec_velPlayerNormal_After + vec_velPlayerTangen_After;

        //            // Restrict the puck velocity to a max.
        //            if (vec_velPlayer_After.Length() > 1) 
        //            {
        //                vec_velPuck_After.Normalize();
        //            }
        //            _puckVelocity = vec_velPuck_After;
        //        }
        //    }
        //}

        //private void HandlePuckMovement(GameTime gameTime)
        //{
        //    _puckPosition.X = (float)(_puckPosition.X + (_puckVelocity.X * gameTime.ElapsedGameTime.TotalMilliseconds));
        //    _puckPosition.Y = (float)(_puckPosition.Y + (_puckVelocity.Y * gameTime.ElapsedGameTime.TotalMilliseconds));
        //}



        //private void HandlePuckWallCollision()
        //{
        //    int goalWidth = (GraphicsDevice.Viewport.Height / 2);
        //    int goalCentre = (GraphicsDevice.Viewport.Height / 2);
        //    int goalLeft = goalCentre - (goalWidth/2);
        //    int goalRight = goalCentre + (goalWidth / 2);
        //    int realativeGoalLeft = goalLeft + (_puckTexture.Width / 2);
        //    int realativeGoalRight = goalRight - (_puckTexture.Width / 2);

        //    if (_puckPosition.Y > (GraphicsDevice.Viewport.Height - (_puckTexture.Height / 2)) && _puckVelocity.Y > 0)
        //    {
        //        PlayPuckCollisionEffect();
        //        _puckVelocity.Y *= -1;
        //    }
        //    if (_puckPosition.Y < 0 + (_puckTexture.Height / 2) && _puckVelocity.Y < 0)
        //    {
        //        PlayPuckCollisionEffect();
        //        _puckVelocity.Y *= -1;
        //    }
        //    if (_puckPosition.X > (GraphicsDevice.Viewport.Width - (_puckTexture.Width / 2)) && _puckVelocity.X > 0)
        //    {
        //        if (_puckPosition.Y > realativeGoalLeft && _puckPosition.Y < realativeGoalRight)
        //        {
        //            GoalScored(ActionPlayer.Player1);
        //        }
        //        else
        //        {
        //            PlayPuckCollisionEffect();
        //            _puckVelocity.X *= -1;
        //        }
        //    }
        //    if (_puckPosition.X < 0 + (_puckTexture.Width / 2) && _puckVelocity.X < 0)
        //    {
        //        if (_puckPosition.Y > realativeGoalLeft && _puckPosition.Y < realativeGoalRight)
        //        {
        //            GoalScored(ActionPlayer.Player2);
        //        }
        //        else
        //        {
        //            PlayPuckCollisionEffect();
        //            _puckVelocity.X *= -1;
        //        }
        //    }
        //}

        private void PlayPuckCollisionEffect()
        {
            Random rGen = new Random();
            int collisionEffectIndex = rGen.Next(0, _bounceEffects.Count);
            Debug.WriteLine("Played collision effect {0}", collisionEffectIndex);
            _bounceEffects[collisionEffectIndex].Play();
        }

        private void GoalScored(ActionPlayer actionPlayer)
        {
            if (!_gameOver)
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
            }

            InitialisePuckToStartingConditions();
        }

        //private void ApplyPuckFriction()
        //{
        //    if (_puckVelocity.X < 0)
        //    {
        //        _puckVelocity.X += _puckFriction.X;
        //    }
        //    else if (_puckVelocity.X > 0)
        //    {
        //        _puckVelocity.X -= _puckFriction.X;
        //    }

        //    if (_puckVelocity.Y < 0)
        //    {
        //        _puckVelocity.Y += _puckFriction.Y;
        //    }
        //    else if (_puckVelocity.Y > 0)
        //    {
        //        _puckVelocity.Y -= _puckFriction.Y;
        //    }
        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (GameMode == AirHockey.GameMode.Game)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                //DrawPlayers();
                //DrawPuck();
                DrawScores();
                DrawMessages();

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawMessages()
        {
            Vector2 messageP1Size = _messageFont.MeasureString(_messageP1);
            Vector2 messageP2Size = _messageFont.MeasureString(_messageP2);

            _spriteBatch.DrawString(_messageFont, _messageP1, _messageP1Position, Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(90), new Vector2(messageP1Size.X / 2, messageP1Size.Y / 2), 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_messageFont, _messageP2, _messageP2Position, Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(270), new Vector2(messageP2Size.X / 2, messageP2Size.Y / 2), 1, SpriteEffects.None, 0);

            Vector2 win1Size = _messageFont.MeasureString("You have won");
            Vector2 win2Size = _messageFont.MeasureString("an XBox");

            if (_messageP2 == "WIN !")
            {

                _spriteBatch.DrawString(_messageFont, "You have won", _messageP2Position + new Vector2(50, 0), Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(270), new Vector2(win1Size.X / 2, win1Size.Y / 2), 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_messageFont, "an XBox", _messageP2Position + new Vector2(100, 0), Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(270), new Vector2(win2Size.X / 2, win2Size.Y / 2), 1, SpriteEffects.None, 0);
            }
            else if (_messageP1 == "WIN !")
            {
                _spriteBatch.DrawString(_messageFont, "You have won", _messageP1Position - new Vector2(50, 0), Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(90), new Vector2(win1Size.X / 2, win1Size.Y / 2), 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_messageFont, "an XBox", _messageP1Position - new Vector2(100, 0), Color.White * _messageOpacity * _gameOpacity, MathHelper.ToRadians(90), new Vector2(win2Size.X / 2, win2Size.Y / 2), 1, SpriteEffects.None, 0);
            }
        }

        private void DrawScores()
        {
            Vector2 size = _scoreFont.MeasureString(_player1Score.ToString());
            Vector2 player1ActualScorePosition = new Vector2(_player1ScorePosition.X - size.X, _player1ScorePosition.Y);
            _spriteBatch.DrawString(_scoreFont, _player1Score.ToString(), player1ActualScorePosition, Color.White * _p1ScoreOpacity * _gameOpacity);

            _spriteBatch.DrawString(_scoreFont, _player2Score.ToString(), _player2ScorePosition, Color.White * _p2ScoreOpacity * _gameOpacity);            
        }

        //private void DrawPuck()
        //{
        //    _spriteBatch.Draw(_puckTexture, _puckPosition, null, Color.White * _gameOpacity, 0, new Vector2(_puckTexture.Width / 2, _puckTexture.Height / 2), _puckScale, SpriteEffects.None, 0);            
        //}
    }
}
