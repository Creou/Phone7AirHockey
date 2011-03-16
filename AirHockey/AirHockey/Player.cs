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
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;


namespace AirHockey
{
    public class PlayerTouchBinder
    {
        private Dictionary<int, PlayerNumber> _boundTouchPoints;

        public PlayerTouchBinder()
        {
            _boundTouchPoints = new Dictionary<int, PlayerNumber>();
        }

        public void ReleasePoint(int touchPoint)
        {
            _boundTouchPoints.Remove(touchPoint);
        }

        public bool IsTouchPointBoundToPlayer(int touchPoint, PlayerNumber player)
        {
            PlayerNumber boundPlayer;
            if (_boundTouchPoints.TryGetValue(touchPoint, out boundPlayer))
            {
                return boundPlayer == player;
            }
            else
            {
                return false;
            }
        }
        public bool IsTouchPointBound(int touchPoint)
        {
            return _boundTouchPoints.ContainsKey(touchPoint);
        }

        internal void Bind(int touchPoint, PlayerNumber player)
        {
            _boundTouchPoints.Add(touchPoint, player);
        }
    }

    public enum PlayerNumber
    {
        Player1 = 1,
        Player2 = 2
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : BaseGamePiece
    {
        private PlayerNumber _playerNumber;
        private PlayerTouchBinder _touchBinder;

        public Player(Game game, PlayerNumber playerNumber, PlayerTouchBinder touchBinder)
            : base(game)
        {
            _playerNumber = playerNumber;
            _touchBinder = touchBinder;
        }

        public override float Mass { get { return 10; } }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            switch (_playerNumber)
            {
                case PlayerNumber.Player1:
                    LoadTexture("Player1");
                    break;

                case PlayerNumber.Player2:
                    LoadTexture("Player2");
                    break;

                default:
                    break;
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            HandlePlayerInput();

            base.Update(gameTime);
        }

        //private int _boundTouchId;

        private void HandlePlayerInput()
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            foreach (TouchLocation touchLoc in touchCollection)
            {
                if (_touchBinder.IsTouchPointBound(touchLoc.Id) && _touchBinder.IsTouchPointBoundToPlayer(touchLoc.Id, _playerNumber))
                {
                    if (touchLoc.State == TouchLocationState.Released)
                    {
                        _touchBinder.ReleasePoint(touchLoc.Id);
                        //_boundTouchId = -1;
                    }
                    else if (touchLoc.State == TouchLocationState.Moved)
                    {
                        Vector2 newVelocity = touchLoc.Position - Position;
                        newVelocity = newVelocity * 0.01f;

                        newVelocity = RestrictMaxPlayerVelocity(newVelocity);
                        SetVelocity(newVelocity);
                    }
                }
                else if (touchLoc.State == TouchLocationState.Pressed || touchLoc.State == TouchLocationState.Moved)
                {
                    if ((touchLoc.Position.X < 400 && _playerNumber == PlayerNumber.Player1) || (touchLoc.Position.X > 400 && _playerNumber == PlayerNumber.Player2))
                    {
                        // Bind the touch location to this player so in the future this touch point always controls this player.
                        if (!_touchBinder.IsTouchPointBound(touchLoc.Id))
                        {
                            _touchBinder.Bind(touchLoc.Id, _playerNumber);
                        }
                    }
                }
            }
        }

        private Vector2 RestrictMaxPlayerVelocity(Vector2 playerVelocity)
        {
            if (playerVelocity.Length() > 1)
            {
                playerVelocity.Normalize();
                return playerVelocity;
            }
            else
            {
                return playerVelocity;
            }
        }


    }
}
