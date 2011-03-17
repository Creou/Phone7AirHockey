using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirHockey
{
    /// <summary>
    /// Class to provide logic to bind players to touchpoints 
    /// so each touch point will always control a single player 
    /// until it is released.
    /// </summary>
    public class PlayerTouchBinder
    {
        private Dictionary<int, PlayerNumber> _boundTouchPoints;

        public PlayerTouchBinder()
        {
            _boundTouchPoints = new Dictionary<int, PlayerNumber>();
        }

        /// <summary>
        /// Method to check if a touch point is bound to a specific player.
        /// </summary>
        /// <param name="touchPointId">The touch point Id to check.</param>
        /// <param name="player">The player to check.</param>
        /// <returns>Flag indicating if the touch point specified is bound to the player specified.</returns>
        public bool IsTouchPointBoundToPlayer(int touchPointId, PlayerNumber player)
        {
            PlayerNumber boundPlayer;
            if (_boundTouchPoints.TryGetValue(touchPointId, out boundPlayer))
            {
                return boundPlayer == player;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Checks if a touch point is bound to any players.
        /// </summary>
        /// <param name="touchPointId">The touch point Id to check.</param>
        /// <returns>Flag indicating if the touch point Id specified is bound to any players.</returns>
        public bool IsTouchPointBound(int touchPointId)
        {
            return _boundTouchPoints.ContainsKey(touchPointId);
        }

        internal void Bind(int touchPointId, PlayerNumber player)
        {
            var playersTouchPoints = _boundTouchPoints.Where(btp => btp.Value == player).ToList();
            foreach (var touchPointBinding in playersTouchPoints)
            {
                _boundTouchPoints.Remove(touchPointBinding.Key);
            }

            _boundTouchPoints.Add(touchPointId, player);
        }

        public void Release(int touchPointId)
        {
            _boundTouchPoints.Remove(touchPointId);
        }
    }
}
