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
    public class Player : BaseGamePiece
    {
        private PlayerNumber _playerNumber;

        public Player(Game game, PlayerNumber playerNumber)
            : base(game)
        {
            _playerNumber = playerNumber; 
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

        private void HandlePlayerInput()
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            foreach (TouchLocation touchLoc in touchCollection)
            {
                if ((touchLoc.State == TouchLocationState.Pressed) || (touchLoc.State == TouchLocationState.Moved))
                {
                    if ((touchLoc.Position.X < 400 && _playerNumber == PlayerNumber.Player1) || (touchLoc.Position.X > 400 && _playerNumber == PlayerNumber.Player2))
                    {
                        Vector2 newVelocity = touchLoc.Position - Position;
                        newVelocity = Velocity * 0.01f;

                        newVelocity = RestrictMaxPlayerVelocity(Velocity);
                        SetVelocity(newVelocity);
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
