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
using System.Collections;
#else
using Windows.UI;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Collections;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for RangeSeriesBase
    /// </summary>
    public abstract class RangeSeriesBase : CartesianSeries
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the property path to retrieve High data from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for HighValues.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register("High", typeof(string), typeof(RangeSeriesBase), new PropertyMetadata(null, OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeSeriesBase).OnBindingPathChanged(e);
        }

        /// <summary>
        /// Gets or Sets the property path to retrieve Low data from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LowValues.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register("Low", typeof(string), typeof(RangeSeriesBase), new PropertyMetadata(null,OnYPathChanged));
        /// <summary>
        /// Get or Set HighValues property
        /// </summary>
        protected IList<double> HighValues { get; set; }
        /// <summary>
        /// Get or Set LowValues property
        /// </summary>
        protected internal IList<double> LowValues { get; set; }
        /// <summary>
        /// Get or Set Segment property
        /// </summary>
        protected ChartSegment Segment { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Called when instance created for RangeSeriesBase
        /// </summary>
        public RangeSeriesBase()
        {
            HighValues = new List<double>();
            LowValues = new List<double>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            HighValues.Clear();
            LowValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { High, Low }, HighValues, LowValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HighValues.Clear();
            LowValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low }, HighValues, LowValues);
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var highValue in HighValues)
            {
                if (double.IsNaN(highValue) && ShowEmptyPoints)
                    ValidateDataPoints(HighValues); break;
            }
            foreach (var lowValue in LowValues)
            {
                if (double.IsNaN(lowValue) && ShowEmptyPoints)
                    ValidateDataPoints(LowValues); break;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as RangeSeriesBase).High = this.High;
            (obj as RangeSeriesBase).Low = this.Low;
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
