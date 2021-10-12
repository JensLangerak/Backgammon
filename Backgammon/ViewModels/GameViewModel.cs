using Backgammon.Models;
using Backgammon.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Backgammon.Utils;

namespace Backgammon.ViewModels
{

    public class GameViewModel : BaseNotifier
    {
        public GameViewModel()
        {
            // Create the game
            game = new Game();
            game.Dice1.PropertyChanged += DiceChanged;
            game.Dice2.PropertyChanged += DiceChanged;


            game.Board.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) => OnPropertyChanged(nameof(Board)));
            game.PropertyChanged += GameChanged;
            Board = new BoardViewModel(game);

            ThrowDiceCommand = new RelayCommand(ThrowDice);

            // The respwan locations for the captured checkers. These are shown outside the board. 
            RespawnPointBlack = Board.AllPoints[0];
            RespawnPointRed = Board.AllPoints[Board.AllPoints.Length - 1];
        }

        /// <summary>
        /// Throw the dice, and go to the next turn.
        /// </summary>
        /// <param name="obj"></param>
        protected void ThrowDice(object obj)
        {
            game.NextTurn();
            Board.SetSelectable(game.BlacksTurn);
        }

        /// <summary>
        /// Dice changed callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DiceChanged(object sender, PropertyChangedEventArgs e)
        {
            DiceChached();
        }

        /// <summary>
        /// Dice has been changed, thus a move was made or a new turn was started. Check the win conditions and update the ui elements that might have changed.
        /// </summary>
        protected void DiceChached()
        {
            CheckGameEnd();
            CheckLegalMoves();

            OnPropertyChanged(nameof(Dice1));
            OnPropertyChanged(nameof(Dice2));
            OnPropertyChanged(nameof(Dice1Used));
            OnPropertyChanged(nameof(Dice2Used));


            OnPropertyChanged(nameof(CanThrow));
            OnPropertyChanged(nameof(PlayerString));
        }

        /// <summary>
        /// Check if the game is finished.
        /// </summary>
        public void CheckGameEnd()
        {
            if (game.CheckWinCondition())
            {
                if (game.Board.ColorHasWon(true))
                {
                    MessageBox.Show("Black has won");
                }
                else if (game.Board.ColorHasWon(false))
                {
                    MessageBox.Show("White has won");
                }
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Check if player can do a move using the current dice.
        /// </summary>
        protected void CheckLegalMoves()
        {
            if (!CanThrow)
            {
                if (!game.HasLegalMoves(game.BlacksTurn))
                {
                    game.Dice1.MarkAsUsed();
                    game.Dice2.MarkAsUsed();
                }
            }
        }

        /// <summary>
        /// Game has changed event callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GameChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PlayerString));
            DiceChached();
        }

        protected Game game;

        public int Dice1 { get => game.Dice1.Value ?? 1 ; }
        public int Dice2 { get => game.Dice2.Value ?? 1; }
        public bool Dice1Used { get => game.Dice1.Used; }
        public bool Dice2Used { get => game.Dice2.Used; }
        public BoardViewModel Board { get; private set; }

        /// <summary>
        /// The player should throw the dice when they are both used.
        /// </summary>
        public bool CanThrow { get => Dice1Used && Dice2Used; }

        /// <summary>
        /// Name of the player color whose turn it currently is.
        /// </summary>
        public String PlayerString
        {
            get
            {
                if (game.BlacksTurn)
                    return "Black";
                else
                    return "White";               
            }
        }

        public RelayCommand ThrowDiceCommand { get; private set; }


        public PointViewModel RespawnPointBlack { get; private set; }
        public PointViewModel RespawnPointRed { get; private set; }
    }
}
