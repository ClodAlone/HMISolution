#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
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
    /// Class implementation for AccumulationSeriesBase
    /// </summary>
    public abstract class AccumulationSeriesBase:AdornmentSeries
    {
        #region Properties

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
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(AccumulationSeriesBase), new PropertyMetadata(null, OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AccumulationSeriesBase).OnBindingPathChanged(e);
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
            DependencyProperty.Register("ExplodeRadius", typeof(double), typeof(AccumulationSeriesBase), new PropertyMetadata(0d,OnExplodeRadiusChanged));

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
            DependencyProperty.Register("ExplodeIndex", typeof(int), typeof(AccumulationSeriesBase), new PropertyMetadata(-1, OnExplodeIndexChanged));

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
            DependencyProperty.Register("ExplodeAll", typeof(bool), typeof(AccumulationSeriesBase), new PropertyMetadata(false, OnExplodeAllChanged));


        /// <summary>
        /// Get or Set SegmentSelectionBrush property
        /// </summary>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SegmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(AccumulationSeriesBase), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set YValues property
        /// </summary>
        protected IList<double> YValues { get; set; }

        #endregion


        #region CallBacks

        private static void OnExplodeRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularSeriesBase = d as CircularSeriesBase;
            if (circularSeriesBase != null) circularSeriesBase.SetExplodeRadius();
        }

        private static void OnExplodeIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as CircularSeriesBase;
            if (series != null)
            {
            }
            if (series != null) series.SetExplodeIndex((int)e.NewValue);
        }

        private static void OnExplodeAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series=d as CircularSeriesBase;
            if (series != null)
            {
            }
            if (series != null) series.SetExplodeAll();
        }


        #endregion

        #region Ctor

        /// <summary>
        /// Called when instance created for AccumulationSeriesBase
        /// </summary>
        public AccumulationSeriesBase()
        {
            YValues = new List<double>();
        }

        #endregion

        #region Methods
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
            if (this.Area != null)
                this.Area.IsUpdateLegend = true;
            UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            Segments.Clear();
            if (this.Area != null)
                this.Area.IsUpdateLegend = true;
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implemetation for Generate points for Indicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new[] { YBindingPath }, YValues);
        }
        /// <summary>
        /// Method implementation for ExplodeIndex
        /// </summary>
        /// <param name="i"></param>
        protected virtual void SetExplodeIndex(int i)
        {
           
        }
        /// <summary>
        /// Virtual Method for ExplodeRadius
        /// </summary>
        protected virtual void SetExplodeRadius()
        {

        }
        /// <summary>
        /// Virtual method for ExplodeAll
        /// </summary>
        protected virtual void SetExplodeAll()
        {
           
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in YValues)
            {
                if (double.IsNaN(yValue) && ShowEmptyPoints)
                    ValidateDataPoints(YValues); break;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as AccumulationSeriesBase).YBindingPath = this.YBindingPath;
            (obj as AccumulationSeriesBase).ExplodeRadius = this.ExplodeRadius;
            (obj as AccumulationSeriesBase).ExplodeIndex = this.ExplodeIndex;
            (obj as AccumulationSeriesBase).ExplodeAll = this.ExplodeAll;
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
