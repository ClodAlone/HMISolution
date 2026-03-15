#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CircularSeriesBase
    /// </summary>
    public abstract class CircularSeriesBase : AccumulationSeriesBase
    {
        #region Properties

        internal double Radius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable smart labels].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable smart labels]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSmartLabels
        {
            get { return (bool)GetValue(EnableSmartLabelsProperty); }
            set { SetValue(EnableSmartLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSmartLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSmartLabelsProperty =
            DependencyProperty.Register("EnableSmartLabels", typeof (bool), typeof (CircularSeriesBase), new PropertyMetadata(false, OnAdornmentPorpertyChanged));

        /// <summary>
        /// Gets or sets the connector mode.
        /// </summary>
        /// <value>
        /// The connector mode.
        /// </value>
        public ConnectorMode ConnectorType
        {
            get { return (ConnectorMode)GetValue(ConnectorTypeProperty); }
            set { SetValue(ConnectorTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectorMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorTypeProperty =
            DependencyProperty.Register("ConnectorType", typeof(ConnectorMode), typeof(CircularSeriesBase), new PropertyMetadata(ConnectorMode.Line, OnAdornmentPorpertyChanged));

        private static void OnAdornmentPorpertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var series = d as CircularSeriesBase;
            if (series != null && series.adornmentInfo != null)
            {
                series.adornmentInfo.OnAdornmentPropertyChanged();
            }
        }

        //public double CircleCoefficient
        //{
        //    get { return (double)GetValue(CircleCoefficientProperty); }
        //    set { SetValue(CircleCoefficientProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for PieCoefficiecnt.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CircleCoefficientProperty =
        //    DependencyProperty.Register("CircleCoefficient", typeof(double), typeof(CircularSeriesBase), new PropertyMetadata(0.4d));

        /// <summary>
        /// Gets or Sets label position for pie segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public CircularSeriesLabelPosition LabelPosition
        {
            get { return (CircularSeriesLabelPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start angle in degrees, of clockwise rotation..
        /// </summary>
        /// <value>
        /// The start angle.
        /// </value>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof (double), typeof (CircularSeriesBase),
                                        new PropertyMetadata(0d, OnStartAngleChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(CircularSeriesLabelPosition), typeof(CircularSeriesBase), new PropertyMetadata(CircularSeriesLabelPosition.Inside, OnAdornmentPorpertyChanged));

        // protected List<double> SegmentRadius { get; set; }

        #endregion

        #region methods

        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularSeriesBase series = d as CircularSeriesBase;
            if (series != null)
                series.UpdateArea();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as CircularSeriesBase).LabelPosition = this.LabelPosition;
            return base.CloneSeries(obj);
        }

        protected double DegreeToRadianConverter(double degree)
        {
            return degree * Math.PI / 180;
        }

        #endregion
    }
}
