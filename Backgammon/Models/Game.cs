using System;
using System.Collections.Generic;
using System.Text;
using Backgammon.Utils;

namespace Backgammon.Models
{

    /// <summary>
    /// Keeps track of the game state.
    /// </summary>
    public class Game : BaseNotifier
    {
        public Game ()
        {
            Board = new Board(24, 15);
            Dice1 = new Dice(6);
            Dice2 = new Dice(6);
        }

        protected Board board;
        public Board Board {
            get => board;
            private set => SetProperty(ref board, value); 
        }


        // True if it is blacks turn.
        protected bool blacksTurn;

        public bool BlacksTurn
        {
            get => blacksTurn;
            private set => SetProperty(ref blacksTurn, value);
        }


        protected Dice dice1;
        protected Dice dice2;
        public Dice Dice1
        {
            get => dice1;
            private set => SetProperty(ref dice1, value);
        }

        public Dice Dice2
        {
            get => dice2;
            private set => SetProperty(ref dice2, value);            
        }

        /// <summary>
        /// True when both dices are used and should be rethrown.
        /// </summary>
        public bool TurnDone { get => Dice1.Used && Dice2.Used; }

        /// <summary>
        /// Throw the dices and switch who is playing.
        /// </summary>
        public void NextTurn()
        {
            if(!(dice1.Used && dice2.Used))
            {
                throw new Exception("Not allowed to throw the dice"); 
            }
            dice1.ThrowNext();
            dice2.ThrowNext();
            BlacksTurn = !BlacksTurn;
        }
        
        /// <summary>
        /// Check if the move is legal.
        /// </summary>
        /// <param name="origin">Point where the checker originates.</param>
        /// <param name="destination">Destination of the checker.</param>
        /// <returns></returns>
        public bool IsLegalMove(Point origin, Point destination)
        {
            // Origin has no checkers on it.
            if (!origin.IsControlled)
                return false;

            // You can move a piece to a tile that is controlled by you or to a tile with at most 1 piece of the opponent.
            if (origin.OwnerColor == (blacksTurn ? PlayerColor.Black : PlayerColor.White))
            {
                if (destination.OwnerColor != origin.OwnerColor && destination.NumberOfPieces > 1)
                    return false;
 

                int diff = GetDiff(origin, destination);
     
                if ((!Dice1.Used) && Dice1.Value == diff)
                    return true;
                if ((!Dice2.Used) && Dice2.Value == diff)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if someone has won.
        /// </summary>
        /// <returns>True if the game is over.</returns>
        public bool CheckWinCondition()
        {
           return board.ColorHasWon(true) || board.ColorHasWon(false); ;

        }

        /// <summary>
        /// Check if the player can still move.
        /// </summary>
        /// <param name="blacksTurn">True if the current player is black.</param>
        /// <returns></returns>
        internal bool HasLegalMoves(bool blacksTurn)
        {
            if (!Dice1.Used)
            {
                if (HasLegalMoves(blacksTurn, Dice1.Value ?? 0))
                    return true;
            }
            if (!Dice2.Used)
            {
                if (HasLegalMoves(blacksTurn, Dice2.Value ?? 0))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the dice can be used for a legal move for the given player.
        /// </summary>
        /// <param name="blacksTurn"></param>
        /// <param name="diceValue"></param>
        /// <returns></returns>
        private bool HasLegalMoves(bool blacksTurn, int diceValue)
        {
            if (!blacksTurn)
                diceValue *= -1;

            // Check the respawn tiles.
            if (blacksTurn) {
                if (board.RespawnPointBlack.NumberOfPieces > 0)
                {
                    if (board.Points[diceValue - 1].OwnerColor == PlayerColor.Black || board.Points[diceValue - 1].NumberOfPieces <= 1)
                        return true;
                }

            } else
            {
                int end = board.Points.Length - 1;
                if (board.RespawnPointRed.NumberOfPieces > 0)
                {
                    if (board.Points[end + diceValue + 1].OwnerColor == PlayerColor.Black || board.Points[end + diceValue + 1].NumberOfPieces <= 1)
                        return true;
                }
            }

            // Check the other tiles.
            for (int i =0; i< board.Points.Length; i++)
            {
                if (board.Points[i].OwnerColor == (blacksTurn ? PlayerColor.Black : PlayerColor.White))
                {
                    int next = i + diceValue;
                    // Check if it will remove the tile from the board or if it can reach a legal other tile.
                    if (next < 0 || next >= board.Points.Length)
                        return true;
                    else if (board.Points[next].OwnerColor == board.Points[i].OwnerColor || board.Points[next].NumberOfPieces <= 1)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Execute the move. Move a piece from source to destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal void DoMove(Point source, Point destination)
        {
            if (!IsLegalMove(source, destination))
                throw new Exception("Illegal move");    
            // You ware already in control of the tile.
            if (destination.OwnerColor == source.OwnerColor)
            {
                destination.NumberOfPieces++;
            }
            else
            {
                // Check if you remove a checker of the opponent.
                if (destination.OwnerColor!= null && destination.NumberOfPieces == 1)
                {
                    if (destination.OwnerColor == PlayerColor.Black)
                        board.RespawnPointBlack.NumberOfPieces++;
                    else
                        board.RespawnPointRed.NumberOfPieces++;
                }
                destination.OwnerColor = source.OwnerColor;
                destination.NumberOfPieces = 1;
            }
            source.NumberOfPieces--;


            // Figure out which dice was used.
            int diff = GetDiff(source, destination);
            if ((!Dice1.Used) && Dice1.Value == diff)
            {
                Dice1.MarkAsUsed();
            } else
            {
                Dice2.MarkAsUsed();
            }
        }

        /// <summary>
        /// Check if the checker can be removed from the game.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool TryRemove(Point point)
        {
            int index = board.GetPointIndex(point);
            int end = (point.OwnerColor == PlayerColor.Black) ? board.Points.Length : -1;
            int diff = Math.Abs(end - index);
            if ((!Dice1.Used) && Dice1.Value >= diff)
            {
                point.NumberOfPieces--;
                Dice1.MarkAsUsed();
                return true;
            }
            if ((!Dice2.Used) && Dice2.Value >= diff)
            {
                point.NumberOfPieces--;
                Dice2.MarkAsUsed();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the number of places between two points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private int GetDiff(Point source, Point destination)
        {
            int startIndex = board.GetPointIndex(source);
            int endIndex = board.GetPointIndex(destination);
            int diff = endIndex - startIndex;
            if (!blacksTurn)
                diff *= -1;
            return diff;
        }
    }
}
