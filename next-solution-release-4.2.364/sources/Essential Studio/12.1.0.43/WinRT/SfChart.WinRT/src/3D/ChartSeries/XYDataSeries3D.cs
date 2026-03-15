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
using System.Collections;
#else
using Windows.UI.Xaml;
using System.Collections;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for XyDataseries3D 
    /// </summary>
    public abstract class XyDataSeries3D:CartesianSeries3D
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
        /// Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(XyDataSeries3D), new PropertyMetadata(null,OnYBindingPathChanged));

        private static void OnYBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xyDataSeries3D = d as XyDataSeries3D;
            if (xyDataSeries3D != null) xyDataSeries3D.OnBindingPathChanged(e);
        }

        /// <summary>
        /// Get or Set YValues
        /// </summary>
        protected internal IList<double> YValues
        {
            get;
            set;
        }

        #endregion

        #region Ctor
        /// <summary>
        /// Called when instance created for XyDataSeries 
        /// </summary>
        protected XyDataSeries3D()
        {
            YValues = new List<double>();
        }
        #endregion

        #region Methods

        internal DoubleRange GetSegmentDepth()
        {
            var actualDepth = Area.Depth;
            double start, end;

            if (Area.SideBySideSeriesPlacement)
            {
                var space = actualDepth / 4;
                start = space;
                end = space * 3;
            }
            else
            {
                var index = Area.VisibleSeries.IndexOf(this);
                var count = Area.VisibleSeries.Count;
                var space = actualDepth / ((count * 2) + count + 1);
                start = space + (space * index * 3);
                end = start + space * 2;
            }
            return new DoubleRange(start, end);
        }

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new[] { YBindingPath }, YValues);
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            YValues.Clear();
            GeneratePoints(new[] { YBindingPath }, YValues);
            UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var xyDataSeries = obj as XyDataSeries;
            if (xyDataSeries != null) xyDataSeries.YBindingPath = YBindingPath;
            return base.CloneSeries(obj);
        }

        #endregion 
    }
}
