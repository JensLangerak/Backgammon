using Backgammon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Utils;

namespace Backgammon.ViewModels
{
    public class BoardViewModel : BaseNotifier
    {
        public BoardViewModel() : this(new Game()) { }
        public BoardViewModel (Game game)
        {
            this.Board = game.Board;
            this.game = game;
        }
        protected Board board;
        protected Game game;
        protected Point selectedPoint;
        public Board Board { get => board; set {
                board = value;

                // Split the points in two 2 rows.
                topPoints = new PointViewModel[board.Size / 2];
                bottomPoints =  new PointViewModel[board.Size / 2];
                allPoints = new PointViewModel[board.Size + 2];
                allPoints[0] = new PointViewModel(board.RespawnPointBlack);
                for (int i =0; i < board.Size / 2; i++)
                {
                    var viewModel = new PointViewModel(board.Points[i], i % 2 == 0, true);
                    topPoints[i] = viewModel;
                    allPoints[i + 1] = viewModel;
                }
                for (int i = 0; i < board.Size / 2; i++)
                {
                    var viewModel = new PointViewModel(board.Points[board.Size - i - 1], i % 2 == 0, false);
                    bottomPoints[i] = viewModel;
                    allPoints[i + board.Size / 2 + 1] = viewModel;
                }
                allPoints[board.Size + 1] = new PointViewModel(board.RespawnPointRed);
               
                
                // Notify that the rows are changed.
                this.OnPropertyChanged(nameof(TopPoints));
                this.OnPropertyChanged(nameof(BottomPoints));
              
                // Subscrive to the changes in the points
                for (int i = 0; i < allPoints.Length; i++)
                {
                    allPoints[i].PropertyChanged += new PropertyChangedEventHandler(PointChanged);
                }
            }

    }

        protected void PointChanged(object sender, PropertyChangedEventArgs e)
        {
            // A point is selected
            if (sender is PointViewModel)
            {
                PointViewModel vm = (PointViewModel)sender;
                if (e.PropertyName == nameof(vm.IsSelected))
                {
                    UpdateSelected(vm);                
                }
            }
   
        }

        /// <summary>
        /// Update the clickable points based on the current selected point.
        /// </summary>
        /// <param name="vm"></param>
        protected void UpdateSelected(PointViewModel vm)
        {
            if (vm.IsSelected)
            {
                // First point that is selected.
                if (selectedPoint == null)
                {
                    // TODO add control/button for this.
                    // For now remove the checker as soon as possible.
                    if (!game.TryRemove(vm.Point))
                        SetReachable(vm.Point);
                }
                else
                {
                    // Two point are selected. Do the move.
                    game.DoMove(selectedPoint, vm.Point);
                    RemoveReachable();
                }
            }
            else //Point is deselected
            {
                RemoveReachable();
            }
        }

      
        /// <summary>
        /// Only the points that are reachable from the selected point are clickable.
        /// </summary>
        /// <param name="source"></param>
        public void SetReachable(Point source)
        {
            selectedPoint = source;
            for (int i =0; i < allPoints.Length; i++)
            {
                allPoints[i].IsSelectable = false;
                allPoints[i].IsReachable = game.IsLegalMove(source, allPoints[i].Point) ;
            }
        }

        /// <summary>
        /// There is not start point selected, thus remove the reachable properties.
        /// </summary>
        /// <param name="source"></param>
        public void RemoveReachable()
        {
            selectedPoint = null;
            for (int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].IsReachable = false;
                allPoints[i].IsSelected = false;
            }
            SetSelectable(game.BlacksTurn);
        }

        /// <summary>
        /// Make sure that we can select the checkers of our color.
        /// </summary>
        /// <param name="blacksTurn"></param>
        public void SetSelectable(bool blacksTurn)
        {
            if (game.TurnDone)
            {
                for (int i = 0; i < allPoints.Length; i++)
                    allPoints[i].IsSelectable = false;
            }
            else
            {
                for (int i = 0; i < allPoints.Length; i++)
                {
                    if (allPoints[i].IsControlled && allPoints[i].IsControlledByBlack == blacksTurn)
                        allPoints[i].IsSelectable = true;
                    else
                        allPoints[i].IsSelectable = false;
                }
            }
        }

        protected PointViewModel[] topPoints;

        protected PointViewModel[] bottomPoints;
        protected PointViewModel[] allPoints;
        public PointViewModel[] TopPoints { get => topPoints; }
        public PointViewModel[] BottomPoints { get => bottomPoints; }
        public PointViewModel[] AllPoints { get => allPoints; }
    }
}
