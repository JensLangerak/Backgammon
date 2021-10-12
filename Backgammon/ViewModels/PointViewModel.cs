using Backgammon.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using System.ComponentModel;
using Backgammon.ViewModels.Commands;
using Backgammon.Utils;

namespace Backgammon.ViewModels
{
    public class PointViewModel : BaseNotifier
    {
        protected Point point;

        public Point Point { get => point; }
        public PointViewModel() : this(new Point(), true, true) { }
        public PointViewModel(Point point) : this(point, false, false)
        {

        }

        /// <summary>
        /// Create a new point view model
        /// </summary>
        /// <param name="point">Model of the point</param>
        /// <param name="isOddPosition">If the point is at an odd position.</param>
        /// <param name="isTop">True if the point belongs to the top row.</param>
        public PointViewModel(Point point, bool isOddPosition, bool isTop)
        {
            this.point = point;
            this.IsOddPosition = isOddPosition;
            this.IsTop = isTop;
            point.PropertyChanged += new PropertyChangedEventHandler(PointChanged);
            OnClickCommand = new RelayCommand(SelectChecker);
        }

        /// <summary>
        /// Handle the click command on the checker. Check if it could be clicked, and if it was allowed set it as selected.
        /// </summary>
        /// <param name="obj"></param>
        protected void SelectChecker(object obj)
        {
            // Check if it is allowed to click on the checker. Triggers an event that will be handled by the board.
            if (IsSelectable || IsReachable || IsSelected)
                IsSelected = !IsSelected;
        }

        /// <summary>
        /// Update all ui elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PointChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(NumberOfCheckers));
            OnPropertyChanged(nameof(NumberOfCheckersString));
            OnPropertyChanged(nameof(IsVissible));
            OnPropertyChanged(nameof(IsControlledByBlack));
        }

        public bool IsOddPosition { get; set; }
        public bool IsTop { get; set; }

        /// <summary>
        /// Number of checkers on the point.
        /// </summary>
        public int NumberOfCheckers { get => point.NumberOfPieces; }

        /// <summary>
        /// Should be visible when it contains checkers and when the selected checker can be moved to the point.
        /// </summary>
        public bool IsVissible
        {
            get => IsControlled || IsReachable;
        }

        public String NumberOfCheckersString { get => NumberOfCheckers.ToString(); }

        /// <summary>
        /// True when it is controlled by black. False when it is controlled by white.
        /// </summary>
        public bool IsControlledByBlack { get => point.OwnerColor == PlayerColor.Black; }

        /// <summary>
        /// True if it contains a piece of a player.
        /// </summary>
        public bool IsControlled { get => point.IsControlled; }


        public RelayCommand OnClickCommand { get; private set; }

        bool isSelected; // Point is selected as source.
        bool isSelectable; // Point can currently be selected as source.
        bool isReachable; // Point is reachable from the selected source.
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                SetProperty(ref isSelected, value);
                OnPropertyChanged(nameof(BorderColor));
            }
        }
        public bool IsSelectable
        {
            get => isSelectable;
            set
            {
                SetProperty(ref isSelectable, value);
                OnPropertyChanged(nameof(BorderColor));
            }
        }

        public bool IsReachable
        {
            get => isReachable;
            set
            {
                SetProperty(ref isReachable, value);
                OnPropertyChanged(nameof(BorderColor));
                OnPropertyChanged(nameof(IsVissible));
            }
        }

        /// <summary>
        /// Convert the current state to a border color.
        /// </summary>
        public SolidColorBrush BorderColor
        {
            get
            {
                if (IsSelected)
                    return Brushes.Yellow;
                if (IsReachable)
                    return Brushes.Blue;
                if (IsSelectable)
                    return Brushes.Purple;
                return Brushes.Black;
            }
        }
    }
}
