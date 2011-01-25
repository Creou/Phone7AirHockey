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
    public class Player : BaseGamePiece
    {
        private PlayerNumber _playerNumber;

        public Player(Game game, PlayerNumber playerNumber)
            : base(game)
        {
            _playerNumber = playerNumber; 
        }

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
    
       
    }
}
