#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
#endif


namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DoughnutSeries3D
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class DoughnutSeries3D : PieSeries3D
    {
        /// <summary>
        /// Gets or Sets coefficient, which determines the radius of doughnut series.
        /// </summary>
        /// <value>
        /// The doughnut coefficient.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double DoughnutCoefficient
        {
            get { return (double)GetValue(DoughnutCoefficientProperty); }
            set { SetValue(DoughnutCoefficientProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for DoughnutCoefficient.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
            DependencyProperty.Register("DoughnutCoefficient", typeof(double), typeof(DoughnutSeries3D), new PropertyMetadata(0.4d, OnDoughnutCoefficientChanged));

        private static void OnDoughnutCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DoughnutSeries3D) d).UpdateArea();
        }

        protected override void CreatePoints()
        {
            if (Area.RootPanelDesiredSize != null)
            {
                actualWidth = Area.RootPanelDesiredSize.Value.Width;
                actualHeight = Area.RootPanelDesiredSize.Value.Height;
            }
            var doughnutIndex = GetPieSeriesIndex();
            var pieCount = GetCircularSeriesCount();
            var radius = ((((1d - ChartMath.MARGINS_RATIO) * Math.Min(actualWidth / 2, actualHeight / 2)) / pieCount) * 0.8) * (doughnutIndex + 1);
            InsideRadius = (doughnutIndex + 1) * (radius * DoughnutCoefficient);
            base.CreatePoints();
        }
    }
}
