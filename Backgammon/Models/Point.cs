using System;
using System.Collections.Generic;
using System.Text;
using Backgammon.Utils;

namespace Backgammon.Models
{
    public enum PlayerColor { Black,White}
    /// <summary>
    /// Class the represents a tiles, or point as they are called with Backgammon. Keeps track of the number of checkers on each point.
    /// </summary>
    public class Point : BaseNotifier
    {

        public Point()
        {
            NumberOfPieces = 0;
        }

        // Keep track of who controls the point.
        protected PlayerColor? ownerColor;
        public PlayerColor? OwnerColor
        {
            set
            {
                SetProperty(ref ownerColor, value);
                if (value == null)
                    SetProperty(ref numberOfPieces, 0);
            }
            get => ownerColor;
        }

        // The number of checkers on the point.
        protected int numberOfPieces;
        public int NumberOfPieces
        {
            get => numberOfPieces;
            set
            {
                SetProperty(ref numberOfPieces, value);
            }
        }

       

        /// <summary>
        /// True if a player has places one or more checkers on the point.
        /// </summary>
        public bool IsControlled
        {
            get => numberOfPieces > 0;
        }
    }
}
