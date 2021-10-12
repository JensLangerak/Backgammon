using Backgammon.ViewModels;
using Backgammon.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Backgammon.Views
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {

        public Board()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler((object sender, RoutedEventArgs e) => FillBoard());
        }


        /// <summary>
        /// Fill the board with the points.
        /// </summary>
        private void FillBoard()
        {
            BoardViewModel view;
            if (DataContext is BoardViewModel)
                view = (BoardViewModel)DataContext;
            else
                throw new Exception("Wrong datacontext");

            // Create the grid dimensions
            for (int i = 0; i < view.TopPoints.Length; i++)
            {
                var columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(20, GridUnitType.Star);
                BoardGrid.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 0; i <2; i++)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(20, GridUnitType.Star);
                BoardGrid.RowDefinitions.Add(rowDefinition);
            }



            // Create the points
            for (int i = 0; i < view.TopPoints.Length; i++)
            {
                CreatePoint(view.TopPoints[i], 0, i);            
            }
            for (int i = 0; i < view.BottomPoints.Length; i++)
            {
                CreatePoint(view.BottomPoints[i], 1, i);
            }
          

        }

        /// <summary>
        /// Create a point and add it to the grid.
        /// </summary>
        /// <param name="pointViewModel"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        private void CreatePoint(PointViewModel pointViewModel, int y, int x)
        {
            Point point = new Point(pointViewModel);    
            point.SetValue(Grid.ColumnProperty, x);
            point.SetValue(Grid.RowProperty, y);
            BoardGrid.Children.Add(point);
        }
    }
}
