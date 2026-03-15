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
using System.Windows.Media;
using System.Collections;
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for FinancialSeriesBase
    /// </summary>
    public abstract class FinancialSeriesBase:CartesianSeries
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the property path to retrieve High data from ItemsSource.
        /// </summary>
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for High.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register("High", typeof(string), typeof(FinancialSeriesBase), new PropertyMetadata(null,OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialSeriesBase).OnBindingPathChanged(e);
        }

        /// <summary>
        /// Gets or Sets the property path to retrieve Low data from ItemsSource.
        /// </summary>
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Low.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register("Low", typeof(string), typeof(FinancialSeriesBase), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or Sets the property path to retrieve open data from ItemsSource.
        /// </summary>
        public string Open
        {
            get { return (string)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Open.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(string), typeof(FinancialSeriesBase), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or Sets the property path to retrieve close data from ItemsSource.
        /// </summary>
        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Close.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(string), typeof(FinancialSeriesBase), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Get or Set BearFillProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BearFillColor
        {
            get { return (Brush)GetValue(BearFillColorProperty); }
            set { SetValue(BearFillColorProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for BearFillColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BearFillColorProperty =
            DependencyProperty.Register("BearFillColor", typeof(Brush), typeof(FinancialSeriesBase), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnBearFillColorPropertyChanged)));

        /// <summary>
        /// Gets or Set BullFillColorProperty
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BullFillColor
        {
            get { return (Brush)GetValue(BullFillColorProperty); }
            set { SetValue(BullFillColorProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for BullFillColor.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty BullFillColorProperty =
            DependencyProperty.Register("BullFillColor", typeof(Brush), typeof(FinancialSeriesBase), new PropertyMetadata(new SolidColorBrush(Colors.Green), new PropertyChangedCallback(OnBullFillColorPropertyChanged)));

        /// <summary>
        /// Gets or Sets OpenValues property
        /// </summary>
        protected IList<double> OpenValues { get; set; }
        /// <summary>
        /// Gets or Sets HighValues
        /// </summary>
        protected IList<double> HighValues { get; set; }
        /// <summary>
        /// Gets or Sets LowValues property
        /// </summary>
        protected IList<double> LowValues { get; set; }
        /// <summary>
        /// Gets or Set CloseValues
        /// </summary>
        protected internal IList<double> CloseValues { get; set; }
        /// <summary>
        /// Gets or Sets Segments property
        /// </summary>
        protected ChartSegment Segment { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor
        /// </summary>
        public FinancialSeriesBase()
        {
            OpenValues = new List<double>();
            HighValues = new List<double>();
            LowValues = new List<double>();
            CloseValues = new List<double>();
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
            OpenValues.Clear();
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { High, Low, Open, Close }, HighValues, LowValues, OpenValues, CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            OpenValues.Clear();
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low, Open, Close }, HighValues, LowValues, OpenValues, CloseValues);
        }

        private static void OnBearFillColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinancialSeriesBase financialSeries = d as FinancialSeriesBase;

            foreach (ChartSegment segment in financialSeries.Segments)
            {
                if(segment is CandleSegment)
                  (segment as CandleSegment).BearFillColor = financialSeries.BearFillColor;
                else if(segment is HiLoOpenCloseSegment)
                    (segment as HiLoOpenCloseSegment).BearFillColor = financialSeries.BearFillColor;
            }
        }

        private static void OnBullFillColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinancialSeriesBase financialSeries = d as FinancialSeriesBase;

            foreach (ChartSegment segment in financialSeries.Segments)
            {
                if (segment is CandleSegment)
                    (segment as CandleSegment).BullFillColor = financialSeries.BullFillColor;
                else if (segment is HiLoOpenCloseSegment)
                    (segment as HiLoOpenCloseSegment).BullFillColor = financialSeries.BullFillColor;
            }
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
            foreach (var openValue in OpenValues)
            {
                if (double.IsNaN(openValue) && ShowEmptyPoints)
                    ValidateDataPoints(OpenValues); break;
            }
            foreach (var closeValue in CloseValues)
            {
                if (double.IsNaN(closeValue) && ShowEmptyPoints)
                    ValidateDataPoints(CloseValues); break;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as FinancialSeriesBase).High = High;
            (obj as FinancialSeriesBase).Low = Low;
            (obj as FinancialSeriesBase).Open = Open;
            (obj as FinancialSeriesBase).Close = Close;
            (obj as FinancialSeriesBase).BearFillColor = BearFillColor;
            (obj as FinancialSeriesBase).BullFillColor = BullFillColor;
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
