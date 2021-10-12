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

        public PointViewModel(Point point, bool isOddPosition, bool isTop)
        {
            this.point = point;
            this.IsOddPosition = isOddPosition;
            this.IsTop = isTop;
            point.PropertyChanged += new PropertyChangedEventHandler(PointChanged);
            OnClickCommand = new RelayCommand(SelectChecker);
        }

        private void SelectChecker(object obj)
        {
            // Check if it is allowed to click on the checker. Triggers an event that will be handled by the board.
            if (IsSelectable || IsReachable || IsSelected)
                IsSelected = !IsSelected;
        }

        private void PointChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(NumberOfCheckers));
            OnPropertyChanged(nameof(NumberOfCheckersString));
            OnPropertyChanged(nameof(IsVissible));
            OnPropertyChanged(nameof(IsControlledByBlack));
        }

        public bool IsOddPosition { get; set; }
        public bool IsTop { get; set; }
        public int NumberOfCheckers { get => point.NumberOfPieces; }

        public bool IsVissible
        {
            get => IsControlled || IsReachable;
        }

        public String NumberOfCheckersString { get => NumberOfCheckers.ToString(); }

        public System.Windows.Media.SolidColorBrush TrueColor { get => System.Windows.Media.Brushes.Red; }

        public bool IsControlledByBlack { get => point.OwnerColor == PlayerColor.Black; }
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
