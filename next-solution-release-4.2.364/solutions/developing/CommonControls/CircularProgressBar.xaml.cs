using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CommonControls
{
    /// <summary>
    /// A circular type progress bar, that is simliar to popular web based
    /// progress bars
    /// </summary>
    public partial class CircularProgressBar
    {
        #region Data
        private readonly DispatcherTimer animationTimer;

        Grid LayoutRoot;
        Ellipse C0;
        Ellipse C1;
        Ellipse C2;
        Ellipse C3;
        Ellipse C4;
        Ellipse C5;
        Ellipse C6;
        Ellipse C7;
        Ellipse C8;
        RotateTransform SpinnerRotate;
        #endregion

        #region Constructor
        public CircularProgressBar()
        {
            InitializeComponent();
            LayoutRoot = Content as Grid;
            var canvas = LayoutRoot.Children[0] as Canvas;
            C0 = canvas.Children[0] as Ellipse;
            C1 = canvas.Children[1] as Ellipse;
            C2 = canvas.Children[2] as Ellipse;
            C3 = canvas.Children[3] as Ellipse;
            C4 = canvas.Children[4] as Ellipse;
            C5 = canvas.Children[5] as Ellipse;
            C6 = canvas.Children[6] as Ellipse;
            C7 = canvas.Children[7] as Ellipse;
            C8 = canvas.Children[8] as Ellipse;
            SpinnerRotate = canvas.RenderTransform as RotateTransform;

            animationTimer = new DispatcherTimer(
                DispatcherPriority.Normal, Dispatcher);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
        }
        #endregion

        #region Private Methods
        private void Start()
        {
            // Mouse.OverrideCursor = Cursors.Wait;
            animationTimer.Tick += HandleAnimationTick;
            animationTimer.Start();
        }

        private void Stop()
        {
            animationTimer.Stop();
            // Mouse.OverrideCursor = Cursors.Arrow;
            animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            double step = Math.PI * 2 / (C0.Width / 2);

            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }
        
        private static void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0
                + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50
                + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        /*
        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        */

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }
        #endregion
    }
}
