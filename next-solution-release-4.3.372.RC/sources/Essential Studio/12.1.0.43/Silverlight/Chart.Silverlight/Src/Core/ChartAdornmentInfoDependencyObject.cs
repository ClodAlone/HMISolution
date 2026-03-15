#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents adornments class configuration.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about displaying series.
    /// </remarks>
    public class ChartAdornmentInfo : DependencyObject,IDisposable
    {


        /// <summary>
        /// Get or Set seriesProperty
        /// </summary>
        public ChartSeries series
        {
            get { return (ChartSeries)GetValue(seriesProperty); }
            internal set { SetValue(seriesProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for series.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty seriesProperty =
            DependencyProperty.Register("series", typeof(ChartSeries), typeof(ChartAdornmentInfo), new PropertyMetadata(null, OnSeriesPropertyChanged));

        private static void OnSeriesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is ChartAdornmentInfo)
            {
                //ChartAdornmentInfo adornment = obj as ChartAdornmentInfo;
                //Binding SymbolInteriorBinding = new Binding() {Source = adornment.series, Path=new PropertyPath("Interior") };
                //BindingOperations.SetBinding(adornment, ChartAdornmentInfo.SymbolInteriorProperty, SymbolInteriorBinding);
                //adornment.SymbolStroke = adornment.series.Stroke;
                //adornment.SymbolStrokeThickness = adornment.series.StrokeThickness;
            }
        }

        #region Dependency properties
        /// <summary>
        /// Identifies the Visible dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty =
          DependencyProperty.Register("Visible", typeof(bool), typeof(ChartAdornmentInfo), new PropertyMetadata(false, new PropertyChangedCallback(OnVisibleChanged)));

        /// <summary>
        /// Identifies the LabelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
          DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfo), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SymbolTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolTemplateProperty =
            DependencyProperty.Register("SymbolTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfo), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the VerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
          DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentInfo), new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Identifies the Horizontalalignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
          DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentInfo), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the AdornmentsPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsPositionProperty =
            DependencyProperty.Register("AdornmentsPosition", typeof(AdornmentsPosition), typeof(ChartAdornmentInfo), new PropertyMetadata(AdornmentsPosition.TopAndBottom, new PropertyChangedCallback(OnAdornmentPositionChanged)));

        /// <summary>
        /// Identifies the LabelContentPath dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelContentPathProperty =
          DependencyProperty.Register("LabelContentPath", typeof(string), typeof(ChartAdornmentInfo), new PropertyMetadata("DataPoint.Y"));

        /// <summary>
        /// Identifies the ConnectorTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectorTemplateProperty =
                DependencyProperty.Register("ConnectorTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfo), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SegmentLabelFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFormatProperty =
                DependencyProperty.Register("SegmentLabelFormat", typeof(string), typeof(ChartAdornmentInfo), new PropertyMetadata("0.00"));

        /// <summary>
        /// Identifies the SegmentLabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontFamilyProperty =
                DependencyProperty.Register("SegmentLabelFontFamily", typeof(FontFamily), typeof(ChartAdornmentInfo), new PropertyMetadata(new FontFamily("Times New Roman")));

        /// <summary>
        /// Identifies the SegmentLabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontSizeProperty =
                DependencyProperty.Register("SegmentLabelFontSize", typeof(int), typeof(ChartAdornmentInfo), new PropertyMetadata(10));

        /// <summary>
        /// Identifies the SegmentLabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontWeightProperty =
                DependencyProperty.Register("SegmentLabelFontWeight", typeof(FontWeight), typeof(ChartAdornmentInfo), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// Identifies the SegmentLabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelForegroundProperty =
                DependencyProperty.Register("SegmentLabelForeground", typeof(Brush), typeof(ChartAdornmentInfo), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the SegmentLabelRotation dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelRotationProperty =
                DependencyProperty.Register("SegmentLabelRotation", typeof(double), typeof(ChartAdornmentInfo), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the SegmentLabelDataTimeFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelDataTimeFormatProperty =
                DependencyProperty.Register("SegmentLabelDataTimeFormat", typeof(string), typeof(ChartAdornmentInfo), new PropertyMetadata("dd/MM/yyyy"));

        /// <summary>
        /// Identifies the SegmentLabelContent dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelContentProperty =
                DependencyProperty.Register("SegmentLabelContent", typeof(LabelContent), typeof(ChartAdornmentInfo), new PropertyMetadata(LabelContent.LabelContentPath, new PropertyChangedCallback(OnSegmentLabelContentChanged)));

        /// <summary>
        /// Identifies the SegmentHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentHorizontalAlignmentProperty =
                DependencyProperty.Register("SegmentHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentInfo), new PropertyMetadata(HorizontalAlignment.Center, new PropertyChangedCallback(OnAdornmentPositionChanged)));

        /// <summary>
        /// Identifies the SegmentVerticalAlignmen dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentVerticalAlignmentProperty =
                DependencyProperty.Register("SegmentVerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentInfo), new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnAdornmentPositionChanged)));

        /// <summary>
        /// Identifies the SegmentShowLine dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentShowLineProperty =
                DependencyProperty.Register("SegmentShowLine", typeof(bool), typeof(ChartAdornmentInfo), new PropertyMetadata(false, new PropertyChangedCallback(OnVisibleChanged)));

        /// <summary>
        /// Identifies the SegmentIsOut dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentIsOutProperty =
                DependencyProperty.Register("SegmentIsOut", typeof(bool), typeof(ChartAdornmentInfo), new PropertyMetadata(false, new PropertyChangedCallback(OnVisibleChanged)));

        /// <summary>
        /// Identifies the Symbol dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
          DependencyProperty.Register("Symbol", typeof(Symbol), typeof(ChartAdornmentInfo), new PropertyMetadata(Symbol.Custom, new PropertyChangedCallback(OnSymbolChanged)));

        /// <summary>
        /// Identifies the SymbolInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolInteriorProperty =
             DependencyProperty.Register("SymbolInterior", typeof(Brush), typeof(ChartAdornmentInfo), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the SymbolInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeProperty =
             DependencyProperty.Register("SymbolStroke", typeof(Brush), typeof(ChartAdornmentInfo), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

       
        /// <summary>
        /// Identifies the SymbolHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolHeightProperty =
         DependencyProperty.Register("SymbolHeight", typeof(double), typeof(ChartAdornmentInfo), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the SymbolWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolWidthProperty =
          DependencyProperty.Register("SymbolWidth", typeof(double), typeof(ChartAdornmentInfo), new PropertyMetadata(0d));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connector template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <see cref="ChartAdornmentInfo" /> provides flexible way to customize adornment's
        /// look. Represents line that connects <see>
        ///                                         <cref>ChartSegment</cref>
        ///                                     </see>
        ///     with
        /// corresponding adorner.
        /// </remarks>
        /// <value>
        /// The connector template.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo.ConnectorTemplate&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;Line X1="0" X2="10" Y1="0" Y2="0" Stroke="Black"/&gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartAdornmentInfo.ConnectorTemplate&gt;
        /// &lt;/syncfusion:ChartAdornmentInfo&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public DataTemplate ConnectorTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ConnectorTemplateProperty);
            }

            set
            {
                SetValue(ConnectorTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment label format.
        /// </summary>
        /// <remarks>
        /// Represents format string that should be used to represent numerical data in
        /// adorner.
        /// </remarks>
        /// <value>
        /// The segment label format.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label format for series' adorner.
        /// series.AdornmentsInfo.SegmentLabelFormat = "0.00";
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelFormat="0.00"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public string SegmentLabelFormat
        {
            get
            {
                return (string)GetValue(SegmentLabelFormatProperty);
            }

            set
            {
                SetValue(SegmentLabelFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment label data time format.
        /// </summary>
        /// <remarks>
        /// Represents format string that should be used to represent date or time in
        /// adorner.
        /// </remarks>
        /// <value>
        /// The segment label data time format.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label date-time format for series' adorner.
        /// series.AdornmentsInfo.SegmentLabelDataTimeFormat = "mm/dd/yy";
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelDataTimeFormat="mm/dd/yy"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public string SegmentLabelDataTimeFormat
        {
            get
            {
                return (string)GetValue(SegmentLabelDataTimeFormatProperty);
            }

            set
            {
                SetValue(SegmentLabelDataTimeFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment label font family.
        /// </summary>
        /// <remarks>
        /// Identifies font family that should be used to display adornment's text.
        /// </remarks>
        /// <value>
        /// The segment label font family.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Creating font family converter.
        /// FontFamilyConverter fontConverter = new FontFamilyConverter();
        /// //Setting Segment label font family for series' adorner.
        /// series.AdornmentsInfo.SegmentLabelFontFamily =
        /// (FontFamily)fontConverter.ConvertFromString("Arial");
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelFontFamily="Arial"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public FontFamily SegmentLabelFontFamily
        {
            get
            {
                return (FontFamily)GetValue(SegmentLabelFontFamilyProperty);
            }

            set
            {
                SetValue(SegmentLabelFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the segment label font.
        /// </summary>
        /// <remarks>
        /// Represents size the adornment's label should have.
        /// </remarks>
        /// <value>
        /// The size of the segment label font.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label font size.
        /// series.AdornmentsInfo.SegmentLabelFontSize = "20";
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelFontSize="20"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public int SegmentLabelFontSize
        {
            get
            {
                return (int)GetValue(SegmentLabelFontSizeProperty);
            }

            set
            {
                SetValue(SegmentLabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment label font weight.
        /// </summary>
        /// <remarks>
        /// Represents weight that adornment's font should have.
        /// </remarks>
        /// <value>
        /// The segment label font weight.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label font weight for series adorner's font.
        /// series.AdornmentsInfo.SegmentLabelFontWeight = FontWeights.Bold;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelFontWeight="Bold"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public FontWeight SegmentLabelFontWeight
        {
            get
            {
                return (FontWeight)GetValue(SegmentLabelFontWeightProperty);
            }

            set
            {
                SetValue(SegmentLabelFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SegmentLabel Foreground Color.
        /// </summary>
        public Brush SegmentLabelForeground
        {
            get
            {
                return (Brush)GetValue(SegmentLabelForegroundProperty);
            }

            set
            {
                SetValue(SegmentLabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment label rotation. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Segment's rotation angle can be set from 0 to 360 degrees.
        /// </remarks>
        /// <value>
        /// The segment label rotation.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label rotation angle to 45 degrees.
        /// series.AdornmentsInfo.SegmentLabelRotation = 45;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelRotation="45"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public double SegmentLabelRotation
        {
            get
            {
                return (double)GetValue(SegmentLabelRotationProperty);
            }

            set
            {
                SetValue(SegmentLabelRotationProperty, value);
            }
        }




        /// <summary>
        /// get or Set SegmentLabelVisibilityProperty
        /// </summary>
        public Visibility SegmentLabelVisibility
        {
            get { return (Visibility)GetValue(SegmentLabelVisibilityProperty); }
            set { SetValue(SegmentLabelVisibilityProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SegmentLabelVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentLabelVisibilityProperty =
            DependencyProperty.Register("SegmentLabelVisibility", typeof(Visibility), typeof(ChartAdornmentInfo), new PropertyMetadata(Visibility.Visible));

        

        /// <summary>
        /// Gets or sets the content of the segment label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Label can display several types of content. Types for display can be found in
        /// <see cref="LabelContent" /> enumeration.
        /// </remarks>
        /// <value>
        /// The <see cref="LabelContent" /> of the segment label.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label content type.
        /// series.AdornmentsInfo.SegmentLabelContent = LabelContent.YofTot;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentLabelContent="YofTot"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public LabelContent SegmentLabelContent
        {
            get
            {
                return (LabelContent)GetValue(SegmentLabelContentProperty);
            }

            set
            {
                SetValue(SegmentLabelContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment horizontal alignment. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Indicates where a label should be displayed horizontally relative to the
        /// segment.
        /// </remarks>
        /// <value>
        /// The segment horizontal alignment.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label content type.
        /// series.AdornmentsInfo.SegmentHorizontalAlignment = HorizontalAlignment.Center;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentHorizontalAlignment="Center"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public HorizontalAlignment SegmentHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(SegmentHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(SegmentHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segment vertical alignment. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Indicates where a label should be displayed vertically relative to the segment.
        /// </remarks>
        /// <value>
        /// The segment horizontal alignment.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label content type.
        /// series.AdornmentsInfo.SegmentVerticalAlignment = VerticalAlignment.Top;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentVerticalAlignment="Top"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public VerticalAlignment SegmentVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(SegmentVerticalAlignmentProperty);
            }

            set
            {
                SetValue(SegmentVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line that connects segment with label
        /// should be displayed.
        /// </summary>
        /// <remarks>
        /// When adorners are set to be displayed there is an ability to show the line that
        /// connects segment with adorner.
        /// </remarks>
        /// <value>
        /// <c>true</c> if segment shows the line; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment label line.
        /// series.AdornmentsInfo.SegmentShowLine = true;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentShowLine="True"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool SegmentShowLine
        {
            get
            {
                return (bool)GetValue(SegmentShowLineProperty);
            }

            set
            {
                SetValue(SegmentShowLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether segment is out.
        /// </summary>
        /// <remarks>
        /// Adorner can be either inside or outside of the segment. When Adorner is set to
        /// be outside - connecting line can be shown using <see cref="SegmentShowLine" />
        /// property.
        /// </remarks>
        /// <value>
        /// <c>true</c> if segment is out; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment out state.
        /// series.AdornmentsInfo.SegmentIsOut = true;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo SegmentIsOut="True"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool SegmentIsOut
        {
            get
            {
                return (bool)GetValue(SegmentIsOutProperty);
            }

            set
            {
                SetValue(SegmentIsOutProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <remarks>
        /// Per default template, adorner includes symbol and label. <see
        /// cref="VerticalAlignment" /> property is used to adjust symbol's alignment with
        /// respect to adornment's label. <seealso cref="HorizontalAlignment" />
        /// </remarks>
        /// <value>
        /// The vertical alignment.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment out state.
        /// series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Bottom;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo VerticalAlignment="Bottom"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <seealso cref="HorizontalAlignment">HorizontalAlignment</seealso>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <remarks>
        /// Per default template, adorner includes symbol and label. <see
        /// cref="HorizontalAlignment" /> property is used to adjust symbol's alignment with
        /// respect to adornment's label.
        /// </remarks>
        /// <value>
        /// The vertical alignment.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment out state.
        /// series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Bottom;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo HorizontalAlignment="Bottom"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <seealso cref="VerticalAlignment">VerticalAlignment</seealso>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Per default template, adorner includes symbol and label.
        /// property is used to set label's template.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// This property is intended to be used from XAML.
        /// </code>
        /// XAML*:
        /// <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        ///     &lt;syncfusion:ChartArea&gt;
        ///         &lt;syncfusion:ChartSeries Data="1,6,2,7,3,9"&gt;
        ///             &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///                 &lt;syncfusion:ChartAdornmentInfo
        ///                     VerticalAlignment="Top"
        ///                     Visible="True"
        ///                     SegmentLabelContent="YofTot"&gt;
        ///                     &lt;syncfusion:ChartAdornmentInfo.LabelTemplate&gt;
        ///                         &lt;DataTemplate&gt;
        ///                             &lt;Border   CornerRadius="1"
        ///                                       BorderBrush="Black"
        ///                                       Background="White"
        ///                                       BorderThickness="1"&gt;
        ///                             &lt;Label FontSize="20" Content="{Binding}"/&gt;
        ///                             &lt;/Border&gt;
        ///                         &lt;/DataTemplate&gt;
        ///                     &lt;/syncfusion:ChartAdornmentInfo.LabelTemplate&gt;
        ///                     &lt;syncfusion:ChartAdornmentInfo.SymbolTemplate&gt;
        ///                         &lt;DataTemplate&gt;
        ///                             &lt;Rectangle Stroke="Black" Fill="Red" Width="10" Height="10"/&gt;
        ///                         &lt;/DataTemplate&gt;
        ///                     &lt;/syncfusion:ChartAdornmentInfo.SymbolTemplate&gt;
        ///                 &lt;/syncfusion:ChartAdornmentInfo&gt;
        ///             &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///         &lt;/syncfusion:ChartSeries&gt;
        ///     &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <value>The template.</value>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the symbol template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Per default template, adorner includes symbol and label.
        /// property is used to set symbol's template.
        /// <seealso cref="DataTemplate"/>
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// This property is intended to be used from XAML.
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        ///     &lt;syncfusion:ChartArea&gt;
        ///         &lt;syncfusion:ChartSeries Data="1,5,2,8,3,7"&gt;
        ///             &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///                 &lt;syncfusion:ChartAdornmentInfo
        ///                     VerticalAlignment="Top"
        ///                     Visible="True"
        ///                     SegmentLabelContent="YofTot"&gt;
        ///                     &lt;syncfusion:ChartAdornmentInfo.LabelTemplate&gt;
        ///                         &lt;DataTemplate&gt;
        ///                             &lt;Border   CornerRadius="1"
        ///                                       BorderBrush="Black"
        ///                                       Background="White"
        ///                                       BorderThickness="1"&gt;
        ///                             &lt;Label FontSize="20" Content="{Binding}"/&gt;
        ///                             &lt;/Border&gt;
        ///                         &lt;/DataTemplate&gt;
        ///                     &lt;/syncfusion:ChartAdornmentInfo.LabelTemplate&gt;
        ///                     &lt;syncfusion:ChartAdornmentInfo.SymbolTemplate&gt;
        ///                         &lt;DataTemplate&gt;
        ///                             &lt;Rectangle Stroke="Black" Fill="Red" Width="10" Height="10"/&gt;
        ///                         &lt;/DataTemplate&gt;
        ///                     &lt;/syncfusion:ChartAdornmentInfo.SymbolTemplate&gt;
        ///                 &lt;/syncfusion:ChartAdornmentInfo&gt;
        ///             &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///         &lt;/syncfusion:ChartSeries&gt;
        ///     &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <value>The symbol template.</value>
        public DataTemplate SymbolTemplate
        {
            get { return (DataTemplate)GetValue(SymbolTemplateProperty); }
            set { SetValue(SymbolTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label content path. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Used to provide property name that should be used for representing label content.
        /// </remarks>
        public string LabelContentPath
        {
            get { return (string)GetValue(LabelContentPathProperty); }
            set { SetValue(LabelContentPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether label is visible.
        /// </summary>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series.
        /// ChartSeries series = new ChartSeries();
        /// //Setting Segment out state.
        /// series.AdornmentsInfo.Visible = true;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///     &lt;syncfusion:ChartSeries Data="1,5,2,7"&gt;
        ///         &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///             &lt;syncfusion:ChartAdornmentInfo Visible="True"/&gt;
        ///         &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        ///     &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the adornments position.
        /// </summary>
        /// <value>The adornments position.</value>
        public AdornmentsPosition AdornmentsPosition
        {
            get { return (AdornmentsPosition)GetValue(AdornmentsPositionProperty); }
            set { SetValue(AdornmentsPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        /// <value>The symbol.</value>
        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// Gets or sets the symbol interior.
        /// </summary>
        /// <value>The symbol interior.</value>
        public Brush SymbolInterior
        {
            get
            {
                return (Brush)GetValue(SymbolInteriorProperty);
            }

            set
            {
                SetValue(SymbolInteriorProperty, value);
            }
        }



        /// <summary>
        /// Get or Set SymbolStrokeThicknessProperty
        /// </summary>
        public double SymbolStrokeThickness
        {
            get { return (double)GetValue(SymbolStrokeThicknessProperty); }
            set { SetValue(SymbolStrokeThicknessProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SymbolStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeThicknessProperty =
            DependencyProperty.Register("SymbolStrokeThickness", typeof(double), typeof(ChartAdornmentInfo), new PropertyMetadata(0d, OnSymbolStrokeThicknessChanged));

        private static void OnSymbolStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        { 
            
        }



        /// <summary>
        /// Gets or sets the symbol Stroke.
        /// </summary>
        /// <value>The symbol Stroke.</value>
        public Brush SymbolStroke
        {
            get
            {
                return (Brush)GetValue(SymbolStrokeProperty);
            }

            set
            {
                SetValue(SymbolStrokeProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the height of the symbol.
        /// </summary>
        /// <value>The height of the symbol.</value>
        public double SymbolHeight
        {
            get
            {
                return (double)GetValue(SymbolHeightProperty);
            }

            set
            {
                SetValue(SymbolHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the symbol.
        /// </summary>
        /// <value>The width of the symbol.</value>
        public double SymbolWidth
        {
            get
            {
                return (double)GetValue(SymbolWidthProperty);
            }

            set
            {
                SetValue(SymbolWidthProperty, value);
            }
        }
        #endregion

        #region property changed callback

        /// <summary>
        /// Executes when A corresponding area changed
        /// </summary>
        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfo adornment = d as ChartAdornmentInfo;
            if (adornment.series != null && adornment.series.Area != null)
            {
                adornment.series.Area.LoadArea();
            }
        }

        private static void OnSegmentLabelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfo adornment = d as ChartAdornmentInfo;
            if (adornment.series != null && adornment.series.Area != null)
            {
                adornment.series.Area.LoadArea();
            }
        }

        private static void OnAdornmentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfo adornment = d as ChartAdornmentInfo;
            if (adornment.series != null && adornment.series.Area != null)
            {
                adornment.series.Area.LoadArea();
            }
        }

        private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfo adornment = d as ChartAdornmentInfo;
            switch ((Symbol)e.NewValue)
            {
                case Symbol.Cross:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Cross");
                    break;
                case Symbol.Diamond:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Diamond");
                    break;
                case Symbol.Hexagon:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Hexagon");
                    break;
                case Symbol.Ellipse:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Circle");
                    break;
                case Symbol.HorizontalLine:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "HorizoneLine");
                    break;
                case Symbol.InvertedTriangle:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "InvertedTriangle");
                    break;
                case Symbol.Pentagon:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Pentagon");
                    break;
                case Symbol.Plus:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Plus");
                    break;
                case Symbol.Square:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Square");
                    break;
                case Symbol.Triangle:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "Triangle");
                    break;
                case Symbol.VerticalLine:
                    adornment.SymbolTemplate = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "VerticalLine");
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.series != null)
                this.series = null;
        }

        #endregion
    }
}
