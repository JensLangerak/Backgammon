using Backgammon.ViewModels;
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
    /// Interaction logic for Point.xaml
    /// </summary>
    public partial class Point : UserControl
    {
        public Point()
        {

            InitializeComponent();

            SetTriangle();
            this.SizeChanged += new SizeChangedEventHandler(SizeChangedHandler);
        }       

        public PointViewModel PointViewModel { get; private set; }
        
        public Point(PointViewModel pointViewModel)
        {
            this.PointViewModel = pointViewModel;
            this.DataContext = pointViewModel;
            InitializeComponent();

            SetTriangle();
            this.SizeChanged += new SizeChangedEventHandler(SizeChangedHandler);
        }

        /// <summary>
        /// Redraw the controls after the size has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizeChangedHandler(object sender, EventArgs e)
        {
            var control = sender as Point;
            control.SetTriangle();
        }


        public static DependencyProperty FillColorProperty =
               DependencyProperty.Register(nameof(FillColor), typeof(Brush), typeof(Point), new PropertyMetadata(Brushes.Red));
    
        public Brush FillColor
        {
            get
            {
                return (Brush)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }

        private static DependencyProperty ImagePointsProperty =
                DependencyProperty.Register(nameof(ImagePoints), typeof(PointCollection), typeof(Point));

        private PointCollection ImagePoints
        {
            get
            {
                return (PointCollection)GetValue(ImagePointsProperty);
            }
            set
            {
                SetValue(ImagePointsProperty, value);
            }
        }


        /// <summary>
        /// Create the triangle points.
        /// </summary>
        private void SetTriangle()
        {
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            if (this.PointViewModel.IsTop)
            {
                this.ImagePoints = new PointCollection(new[] {
                    new System.Windows.Point(0, 0), 
                    new System.Windows.Point(width / 2, height * 0.8), 
                    new System.Windows.Point(width, 0) });
            }
            else
            {
                this.ImagePoints = new PointCollection(new[] { 
                    new System.Windows.Point(0, height), 
                    new System.Windows.Point(width / 2, height * 0.2), 
                    new System.Windows.Point(width, height) });
            };
        }


    }
}
