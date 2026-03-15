// <copyright file="ChartAdornmentInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Globalization;
    using System.Windows.Media;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;
    using System.Windows.Markup;

    /// <summary>
    /// Represents adornments class configuration.
    /// </summary>
    /// <permission cref="System.Security.PermissionSet">
    /// Public Access
    /// </permission>
    /// <remarks>
    /// Chart adornments are used to show additional information about displaying series.
    /// </remarks>
    ///  <example>
    /// XAML:
    /// <code language="XAML">
    /// &lt;!--Chart with Adornments--&gt;
    ///   &lt;sfchart:Chart&gt;
    ///             &lt;sfchart:ChartArea Background="LightGray" GridBackground="White"&gt;  
    ///                 &lt;sfchart:ChartSeries Type="Column" &gt;
    ///                &lt;sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                         &lt;sfchart:ChartAdornmentInfo 
    /// LabelContentPath="DataPoint.X" Visible="True"  /&gt;
    ///                     &lt;/sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;       
    ///             &lt;/sfchart:ChartArea&gt;           
    ///         &lt;/sfchart:Chart&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// ChartSeries series = Chart1.Areas[0].Series[0];      
    /// ChartAdornmentInfo adornments = series.AdornmentsInfo;
    /// adornments.LabelContentPath = "DataPoint.X";
    /// adornments.Visible = true;
    /// </code>
    /// </example>
    /// <seealso cref="ChartAdornment"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public sealed class ChartAdornmentInfo : DependencyObject, IDisposable, IChartSerializer
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Visible dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty =
          DependencyProperty.Register("Visible", typeof(bool), typeof(ChartAdornmentInfo), new PropertyMetadata(false));

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
            DependencyProperty.Register("AdornmentsPosition", typeof(AdornmentsPosition), typeof(ChartAdornmentInfo), new PropertyMetadata(AdornmentsPosition.Top));

        /// <summary>
        /// Identifies the LabelContentPath dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelContentPathProperty =
          DependencyProperty.Register("LabelContentPath", typeof(string), typeof(ChartAdornmentInfo), new PropertyMetadata("DataPoint.Values[0]"));

        /// <summary>
        /// Identifies the ConnectorTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectorTemplateProperty =
                DependencyProperty.Register("ConnectorTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfo), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the SegmentLabelFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFormatProperty =
                DependencyProperty.Register("SegmentLabelFormat", typeof(string), typeof(ChartAdornmentInfo), new UIPropertyMetadata("0.00"));

        /// <summary>
        /// Identifies the SegmentLabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontFamilyProperty =
                DependencyProperty.Register("SegmentLabelFontFamily", typeof(FontFamily), typeof(ChartAdornmentInfo), new UIPropertyMetadata(new FontFamily("Times New Roman")));

        /// <summary>
        /// Identifies the SegmentLabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontSizeProperty =
                DependencyProperty.Register("SegmentLabelFontSize", typeof(int), typeof(ChartAdornmentInfo), new UIPropertyMetadata(10));

        /// <summary>
        /// Identifies the SegmentLabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFontWeightProperty =
                DependencyProperty.Register("SegmentLabelFontWeight", typeof(FontWeight), typeof(ChartAdornmentInfo), new UIPropertyMetadata(FontWeights.Normal));

       
       

        /// <summary>
        /// Identifies the AdornmentMargin dependency property.
        /// </summary>

        public static readonly DependencyProperty AdornmentMarginProperty =
                DependencyProperty.Register("AdornmentMargin", typeof(Thickness), typeof(ChartAdornmentInfo));

        /// <summary>
        /// Identifies the AdornmentWrapping dependency property.
        /// </summary>

        public static readonly DependencyProperty AdornmentWrappingProperty =
            DependencyProperty.Register("AdornmentWrapping", typeof(TextWrapping), typeof(ChartAdornmentInfo));

        /// <summary>
        /// Identifies the AdornmentFontStrech dependency property.
        /// </summary>

        public static readonly DependencyProperty AdornmentFontStretchProperty =
            DependencyProperty.Register("AdornmentFontStretch", typeof(FontStretch), typeof(ChartAdornmentInfo));
        
        /// <summary>
        /// Identifies the AdornmentForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentForegroundProperty =
                DependencyProperty.Register("AdornmentForeground", typeof(Brush), typeof(ChartAdornmentInfo), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the SegmentLabelRotation dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelRotationProperty =
                DependencyProperty.Register("SegmentLabelRotation", typeof(double), typeof(ChartAdornmentInfo), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the SegmentLabelDataTimeFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelDataTimeFormatProperty =
                DependencyProperty.Register("SegmentLabelDataTimeFormat", typeof(string), typeof(ChartAdornmentInfo), new UIPropertyMetadata("dd/MM/yyyy"));

        /// <summary>
        /// Identifies the SegmentLabelContent dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelContentProperty =
                DependencyProperty.Register("SegmentLabelContent", typeof(LabelContent), typeof(ChartAdornmentInfo), new UIPropertyMetadata(LabelContent.LabelContentPath));

        /// <summary>
        /// Identifies the SegmentHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentHorizontalAlignmentProperty =
                DependencyProperty.Register("SegmentHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentInfo), new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the SegmentVerticalAlignmen dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentVerticalAlignmentProperty =
                DependencyProperty.Register("SegmentVerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentInfo), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Identifies the SegmentShowLine dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentShowLineProperty =
                DependencyProperty.Register("SegmentShowLine", typeof(bool), typeof(ChartAdornmentInfo), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the SegmentIsOut dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentIsOutProperty =
                DependencyProperty.Register("SegmentIsOut", typeof(bool), typeof(ChartAdornmentInfo), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Symbol dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
          DependencyProperty.Register("Symbol", typeof(Symbol), typeof(ChartAdornmentInfo), new PropertyMetadata(Symbol.Custom));

        /// <summary>
        /// Identifies the SymbolInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolInteriorProperty =
             DependencyProperty.Register("SymbolInterior", typeof(Brush), typeof(ChartAdornmentInfo), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SymbolStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeProperty =
             DependencyProperty.Register("SymbolStroke", typeof(Brush), typeof(ChartAdornmentInfo), new PropertyMetadata(null));
        /// <summary>
        /// Identifies the SymbolHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeThicknessProperty =
         DependencyProperty.Register("SymbolStrokeThickness", typeof(double), typeof(ChartAdornmentInfo), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the SymbolHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolHeightProperty =
         DependencyProperty.Register("SymbolHeight", typeof(double), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(0d, null, new CoerceValueCallback(OnCoerceSymbolHeight)));

        /// <summary>
        /// Identifies the SymbolWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolWidthProperty =
          DependencyProperty.Register("SymbolWidth", typeof(double), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(0d, null, new CoerceValueCallback(OnCoerceSymbolWidth)));

        /// <summary>
        ///  Identifies the IsSegmentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSegmentAlignmentProperty =
     DependencyProperty.Register("IsSegmentAlignment", typeof(bool), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(false));

        /// <summary>
        ///  Identifies the IsLabelRotate dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLabelRotateProperty =
 DependencyProperty.Register("IsLabelRotate", typeof(bool), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(false));

        /// <summary>
        ///  Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(0d));

        /// <summary>
        ///  Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(0d));

        /// <summary>
        ///  Identifies the LineTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LineTemplateProperty =
DependencyProperty.Register("LineTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfo), new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the connector template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <see cref="ChartAdornmentInfo" /> provides flexible way to customize adornment's
        /// look. Represents line that connects <see cref="ChartSegment" /> with
        /// corresponding adorner.
        /// </remarks>
        /// <value>
        /// The connector template.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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
        /// Get and Set the LineTemplate property
        /// </summary>
        public DataTemplate LineTemplate
        {
            get
            {
                return (DataTemplate)GetValue(LineTemplateProperty);
            }

            set
            {
                SetValue(LineTemplateProperty, value);
            }
        }

        /// <summary>
        /// Get and Set the IsSegmentAlignment Property
        /// </summary>
        public bool IsSegmentAlignment
        {
            get
            {
                return (bool)GetValue(IsSegmentAlignmentProperty);
            }

            set
            {
                SetValue(IsSegmentAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Get and Set the IsLabelRotateProperty
        /// </summary>
        public bool IsLabelRotate
        {
            get
            {
                return (bool)GetValue(IsLabelRotateProperty);
            }

            set
            {
                SetValue(IsLabelRotateProperty, value);
            }
        }


        /// <summary>
        /// Get and Set the OffsetXProperty
        /// </summary>
        public double OffsetX
        {
            get
            {
                return (double)GetValue(OffsetXProperty);
            }

            set
            {
                SetValue(OffsetXProperty, value);
            }
        }

        /// <summary>
        /// Get and Set the OffsetYProperty
        /// </summary>
        public double OffsetY
        {
            get
            {
                return (double)GetValue(OffsetYProperty);
            }

            set
            {
                SetValue(OffsetYProperty, value);
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// Get and Set the AdornmentForegroundProperty
        /// </summary>
        public Brush AdornmentForeground
        {
            get
            {
                return (Brush)GetValue(AdornmentForegroundProperty);
            }
            set
            {
                SetValue(AdornmentForegroundProperty, value);
            }
        }

        /// <summary>
        /// Get and Set the AdornmentMarginProperty
        /// </summary>
        public Thickness AdornmentMargin
        {
            get
            {
                return (Thickness)GetValue(AdornmentMarginProperty);
            }
            set
            {
                SetValue(AdornmentMarginProperty, value);
            }
        }

        /// <summary>
        /// Get and Set the AdornmentWrappingProperty 
        /// </summary>
        public TextWrapping AdornmentWrapping
        {
            get
            {
                return (TextWrapping)GetValue(AdornmentWrappingProperty);
            }
            set
            {
                SetValue(AdornmentWrappingProperty, value);
            }
        }
       
        /// <summary>
        /// Get and Set the AdornmentFontStretchProperty
        /// </summary>
        public FontStretch AdornmentFontStretch
        {
            get
            {
                return (FontStretch)GetValue(AdornmentFontStretchProperty);
            }
            set
            {
                SetValue(AdornmentFontStretchProperty, value);
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///     &lt;syncfusion:ChartArea&gt;
        ///         &lt;syncfusion:ChartSeries Data="1 2 3 4 5 6"&gt;
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
        /// *Syncfusion WPF Chart libraries should be referenced in project in order to run code samples.
        /// </example>
        /// <value>The template.</value>
        /// <seealso cref="DataTemplate"/>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        ///     &lt;syncfusion:ChartArea&gt;
        ///         &lt;syncfusion:ChartSeries Data="1 2 3 4 5 6"&gt;
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
        /// *Syncfusion WPF Chart libraries should be referenced in project in order to run code samples.
        /// </example>
        /// <value>The symbol template.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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
        /// <example>
        /// C#:
        /// <code>
        /// <para>
        /// This property is intended to be used from XAML.
        /// </para>
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///   &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        ///     &lt;syncfusion:ChartAxis LabelRotateAngle="45" LabelsSource="{StaticResource ChartDataSource}" PositionPath="ID" ContentPath="City"/&gt;
        ///   &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        ///   &lt;syncfusion:ChartSeries Type="Column" Data="{syncfusion:ChartBindingData Source={StaticResource ChartDataSource}, XPath=ID, YPaths=Population}"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        /// <value>The label content path.</value>
        public string LabelContentPath
        {
            get { return (string)GetValue(LabelContentPathProperty); }
            set 
            {
                m_isLabelContentPathSet = true;
                SetValue(LabelContentPathProperty, value); 
            }
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
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        ///     &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
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
        /// Gets or sets the symbol stroke.
        /// </summary>
        /// <value>The symbol stroke.</value>
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
        /// Gets or sets the stroke thickness of the symbol.
        /// </summary>
        /// <value>The stroke thickness of the symbol.</value>
        public double SymbolStrokeThickness
        {
            get
            {
                return (double)GetValue(SymbolStrokeThicknessProperty);
            }

            set
            {
                SetValue(SymbolStrokeThicknessProperty, value);
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

        private static object OnCoerceSymbolHeight(DependencyObject d, object value)
        {
            var height = (double)value;
            return height > 0 ? value : 0d;
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

        private static object OnCoerceSymbolWidth(DependencyObject d, object value)
        {
            var width = (double)value;
            return width > 0 ? value : 0d;
        }
        #endregion

        #region Members
        internal bool m_isLabelContentPathSet;
        internal bool m_requiresSymmetricLabelling;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            base.OnPropertyChanged(e);
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.PropertyChanged = null;
            this.ConnectorTemplate = null;
            this.LabelContentPath = null;
            this.LabelTemplate = null;            
            this.SegmentLabelDataTimeFormat = null;            
            this.SymbolTemplate = null;            
        }

        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Return the Xaml String
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            _xamlString = XamlWriter.Save(this);
            return _xamlString;
        }

        /// <summary>
        /// Return the object from the String value.
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }
}
