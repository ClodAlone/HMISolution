#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Collections;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Collections;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CircularSeriesBase3D
    /// </summary>
    public abstract class CircularSeriesBase3D : ChartSeries3D
    {
        #region Properties

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
            DependencyProperty.Register("ConnectorType", typeof(ConnectorMode), typeof(CircularSeriesBase3D), new PropertyMetadata(ConnectorMode.Line, OnPropertyChanged));

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
            DependencyProperty.Register("EnableSmartLabels", typeof(bool), typeof(CircularSeriesBase3D), new PropertyMetadata(false, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the circle coefficient.
        /// </summary>
        /// <value>
        /// The circle coefficient.
        /// </value>
        public double CircleCoefficient
        {
            get { return (double)GetValue(CircleCoefficientProperty); }
            set { SetValue(CircleCoefficientProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircleCoefficient.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircleCoefficientProperty =
            DependencyProperty.Register("CircleCoefficient", typeof(double), typeof(CircularSeriesBase3D), new PropertyMetadata(0.8d, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets label position for pie segment
        /// </summary>
        /// <value>
        /// The label position.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public CircularSeriesLabelPosition LabelPosition
        {
            get { return (CircularSeriesLabelPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(CircularSeriesLabelPosition), typeof(CircularSeriesBase3D), new PropertyMetadata(CircularSeriesLabelPosition.Inside, OnPropertyChanged));

        /// <summary>
        /// Gets or Sets the property path to retrieve y data from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { SetValue(YBindingPathProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(CircularSeriesBase3D), new PropertyMetadata(null, OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CircularSeriesBase3D) d).OnBindingPathChanged(e);
        }

        /// <summary>
        /// Get or Set ExplodeRadius property
        /// </summary>
        public double ExplodeRadius
        {
            get { return (double)GetValue(ExplodeRadiusProperty); }
            set { SetValue(ExplodeRadiusProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for ExplodeRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
            DependencyProperty.Register("ExplodeRadius", typeof(double), typeof(CircularSeriesBase3D), new PropertyMetadata(30d, OnPropertyChanged));

        /// <summary>
        /// Get or Set ExplodeIndexProperty
        /// </summary>
        public int ExplodeIndex
        {
            get { return (int)GetValue(ExplodeIndexProperty); }
            set { SetValue(ExplodeIndexProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for ExplodeIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExplodeIndexProperty =
            DependencyProperty.Register("ExplodeIndex", typeof(int), typeof(CircularSeriesBase3D), new PropertyMetadata(-1, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((CircularSeriesBase3D) d).UpdateArea();
        }

        /// <summary>
        /// Get or Set ExplodeAllProperty
        /// </summary>
        public bool ExplodeAll
        {
            get { return (bool)GetValue(ExplodeAllProperty); }
            set { SetValue(ExplodeAllProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ExplodeAll.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExplodeAllProperty =
            DependencyProperty.Register("ExplodeAll", typeof(bool), typeof(CircularSeriesBase3D), new PropertyMetadata(false));

        /// <summary>
        /// Get or Set YValues property
        /// </summary>
        protected IList<double> YValues { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Called when instance created for AccumulationSeriesBase
        /// </summary>
        protected CircularSeriesBase3D()
        {
            YValues = new List<double>();
        }

        #endregion

        #region Methods

        internal int GetCircularSeriesCount()
        {
            return (from series in Area.VisibleSeries where series is CircularSeriesBase3D select series).ToList().Count();
        }

        internal int GetPieSeriesIndex()
        {
            int index;
            var pieSeries = (from series in Area.VisibleSeries where series is PieSeries3D select series).ToList();
            return (index = pieSeries.IndexOf(this)) >= 0 ? index : -1;
        }

        /// <summary>
        /// Called when DataSource property get changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            YValues.Clear();
            Segments.Clear();
            GeneratePoints(new[] { YBindingPath }, YValues);
            if (Area != null)
                Area.IsUpdateLegend = true;
            UpdateArea();
        }

        /// <summary>
        /// Degrees to radian converter.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns></returns>
        internal protected double DegreeToRadianConverter(double degree)
        {
            return degree * Math.PI / 180;
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:BindingPathChanged</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            Segments.Clear();
            if (Area != null)
                Area.IsUpdateLegend = true;
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation for Generate points for Indicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new[] { YBindingPath }, YValues);
        }
        #endregion
    }
}
