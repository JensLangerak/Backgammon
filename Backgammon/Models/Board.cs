using System;
using System.Collections.Generic;
using System.Text;
using Backgammon.Utils;

namespace Backgammon.Models
{
    /// <summary>
    /// Class that keeps track of the board.
    /// </summary>
    public class Board : BaseNotifier
    {
        // Tiles of the game.
        public Point[] Points { get; private set; }

        // Respawn point for captured tiles.
        public Point RespawnPointBlack { get; private set; }
        public Point RespawnPointRed { get; private set; }

  
        // Number of checkers in the game.
        protected int numberOfPieces;

        // Board size
        public int Size { get; }


        /// <summary>
        /// Create a new board.
        /// </summary>
        /// <param name="size">Size of the board.</param>
        /// <param name="numberOfPieces">Number of pieces on the board.</param>
        public Board(int size, int numberOfPieces)
        {
            if (size % 2 != 0 || size < 6)
            {
                throw new ArgumentException("Size should be even and larger than 6");
            }
            if (numberOfPieces <= 0)
            {
                throw new ArgumentException("Need at least 1 piece");
            }

            this.numberOfPieces = numberOfPieces;
            Size = size;

            //Init the tiles.
            Points = new Point[Size];
            for (int i = 0; i < size; i++)
            {
                Points[i] = new Point();
            }


            RespawnPointBlack = new Point();
            RespawnPointBlack.OwnerColor = PlayerColor.Black;
            RespawnPointBlack.NumberOfPieces = 0;

            RespawnPointRed = new Point();
            RespawnPointRed.OwnerColor = PlayerColor.White;
            RespawnPointRed.NumberOfPieces = 0;

            // For now place all peices at the first point.
            Points[0].OwnerColor = PlayerColor.Black;
            Points[0].NumberOfPieces = numberOfPieces;
            Points[size - 1].OwnerColor = PlayerColor.White;
            Points[size -1].NumberOfPieces = numberOfPieces;
            
        }

   

        /// <summary>
        /// Find the tile index of the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int GetPointIndex(Point point)
        {
            if (point == RespawnPointBlack)
                return -1;
            if (point == RespawnPointRed)
                return Points.Length;

            int res = Array.FindIndex(Points, (Point p) => p == point);
            if (res == -1)
                throw new KeyNotFoundException();
            return res;
        }

        /// <summary>
        /// Check if a color has managed to win.
        /// </summary>
        /// <param name="isBlack">True if it should check for black.</param>
        /// <returns></returns>
        public bool ColorHasWon(bool isBlack)
        {
            // Check the respawn tile.
            if (isBlack && RespawnPointBlack.NumberOfPieces > 0)
                return false;
            if ((!isBlack)&& RespawnPointRed.NumberOfPieces > 0)
                return false;

            // Check the remaining tiles.
            for (int i =0; i < Points.Length; i++)
            {
                if (Points[i].OwnerColor == (isBlack ? PlayerColor.Black : PlayerColor.White))
                {
                    if (Points[i].NumberOfPieces > 0)
                        return false;
                }
            }

            return true;

        }
    }
}
