// <copyright file="ChartStripLine.cs" company="Syncfusion">
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
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents ChartStripLine class. Contains the information about strip line.
    /// </summary>
    /// <remarks>
    /// Chart WPF enables the user to highlight a specific area of the chart by adding
    /// StripLines to a ChartAxis. The strip lines length and width can be customized, a
    /// text label can be specified and the look and feel can be customized too.
    /// </remarks>
    /// <example>
    /// 	<code language="C#">
    /// //Create a new Stripline
    /// ChartStripLine csX = new ChartStripLine();
    /// //Set whether this need to started from X Axis
    /// csX.StartFromAxis = true;
    /// //Set the offset from where this stripline should be placed
    /// csX.Offset = 2;
    /// //Set the width of the stripline
    /// csX.Width = 0.2;
    /// //Set the interior of the stripline
    /// csX.Interior = App.Current.Resources["imgBrush"] as ImageBrush;
    /// //Set the FormattedText of the Stripline
    /// csX.Text = new FormattedText("",
    /// CultureInfo.CurrentCulture,FlowDirection.LeftToRight, new Typeface("Arial"), 20,
    /// Brushes.Black);
    /// //Add the stripline to the Axis stripline collection
    /// chart1.Areas[0].PrimaryAxis.StripLines.Add(csX);
    /// </code>
    /// </example>
    /// <seealso cref="ChartStripLine"/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStripLine : FrameworkElement, IDisposable
    {
        #region Dependency Properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Alignment.  This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(ChartAlignment), typeof(ChartStripLine), new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Pen), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(FormattedText), typeof(ChartStripLine), new PropertyMetadata(new FormattedText(string.Empty, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Times New Roman"), 10, new SolidColorBrush(Colors.Black))));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartProperty =
          DependencyProperty.Register("Start", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RepeatUntilProperty =
            DependencyProperty.Register("RepeatUntil", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        /// </summary>
        public static new readonly DependencyProperty WidthProperty =
          DependencyProperty.Register("Width", typeof(double), typeof(ChartStripLine), new PropertyMetadata(OnAppearencePropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Period.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RepeatEveryProperty =
          DependencyProperty.Register("RepeatEvery", typeof(double), typeof(ChartStripLine), new PropertyMetadata(OnAppearencePropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
          DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartStripLine), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(150, 190, 190, 190)), new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty for the TextBackground
        /// </summary>
        public static readonly DependencyProperty TextBackgroundProperty =
    DependencyProperty.Register("TextBackground", typeof(Brush), typeof(ChartStripLine), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty for the VerticalText
        /// </summary>
        public static readonly DependencyProperty VerticalTextProperty =
    DependencyProperty.Register("VerticalText", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartFromAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartFromAxisProperty =
            DependencyProperty.Register("StartFromAxis", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(true, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Axis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsSegmented value for the StripLine
        /// </summary>
        public static readonly DependencyProperty IsSegmentedProperty =
         DependencyProperty.Register("IsSegmented", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        ///  Identifies the Segmented Start value for the StripLine
        /// </summary>
        public static readonly DependencyProperty SegmentStartValueProperty =
  DependencyProperty.Register("SegmentStartValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        ///  Identifies the Segmented end value for the StripLine
        /// </summary>
        public static readonly DependencyProperty SegmentEndValueProperty =
  DependencyProperty.Register("SegmentEndValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));


        /// <summary>
        ///  Identifies the Text OffsetX value for the StripLine
        /// </summary>
        public static readonly DependencyProperty TextOffsetXProperty =
          DependencyProperty.Register("TextOffsetX", typeof(double), typeof(ChartStripLine), new PropertyMetadata(1d, new PropertyChangedCallback(OnAppearencePropertyChanged)));


        /// <summary>
        ///  Identifies the TextOffsetY value for the StripLine
        /// </summary>
        public static readonly DependencyProperty TextOffsetYProperty =
          DependencyProperty.Register("TextOffsetY", typeof(double), typeof(ChartStripLine), new PropertyMetadata(1d, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        /// <summary>
        ///  Identifies the Pixel width for the StripLine
        /// </summary>
        public static readonly DependencyProperty IsPixelWidthProperty =
          DependencyProperty.Register("IsPixelWidth", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearencePropertyChanged)));

        #endregion

        #region members


        /// <summary>
        /// Set the StartfromAxis Value
        /// </summary>
        bool startFromAxis = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public FormattedText Text
        {
            get
            {
                return (FormattedText)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        public ChartAlignment TextAlignment
        {
            get { return (ChartAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the IsPixelWidth.
        /// </summary>
        /// <value>The IsPixelWidth.</value>
        public bool IsPixelWidth
        {
            get { return (bool)GetValue(IsPixelWidthProperty); }
            set { SetValue(IsPixelWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>The stroke.</value>
        public Pen Stroke
        {
            get { return (Pen)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior.
        /// </summary>
        /// <value>The interior.</value>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Background of stripline text area.
        /// </summary>
        /// <value>The textBackground.</value>
        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [vertical text].
        /// </summary>
        /// <value><c>true</c> if [vertical text]; otherwise, <c>false</c>.</value>
        public bool VerticalText
        {
            get { return (bool)GetValue(VerticalTextProperty); }
            set { SetValue(VerticalTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start
        {
            get
            {
                return (double)GetValue(StartProperty);
            }

            set
            {
                SetValue(StartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the repeat until.
        /// </summary>
        /// <value>The repeat until.</value>
        public double RepeatUntil
        {
            get { return (double)GetValue(RepeatUntilProperty); }
            set { SetValue(RepeatUntilProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public new double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the repeat every.
        /// </summary>
        /// <value>The repeat every.</value>
        public double RepeatEvery
        {
            get
            {
                return (double)GetValue(RepeatEveryProperty);
            }

            set
            {
                SetValue(RepeatEveryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>A double value in axis range metrics.</value>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [start from axis].
        /// </summary>
        /// <value>True if the stripline should start from the beginning of the axis. False, otherwise.</value>
        public bool StartFromAxis
        {
            get { return (bool)GetValue(StartFromAxisProperty); }
            set { SetValue(StartFromAxisProperty, value); }
        }

        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <value>The axis value.</value>
        public ChartAxis Axis
        {
            get { return (ChartAxis)GetValue(AxisProperty); }
            internal set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// gets or sets the IsSegmented value
        /// </summary>
        public bool IsSegmented
        {
            get { return (bool)GetValue(IsSegmentedProperty); }
            set { SetValue(IsSegmentedProperty, value); }
        }

        /// <summary>
        /// gets or sets the Segment start value
        /// </summary>
        public double SegmentStartValue
        {
            get { return (double)GetValue(SegmentStartValueProperty); }
            set { SetValue(SegmentStartValueProperty, value); }
        }

        /// <summary>
        /// gets or sets the Segment end value
        /// </summary>
        public double SegmentEndValue
        {
            get { return (double)GetValue(SegmentEndValueProperty); }
            set { SetValue(SegmentEndValueProperty, value); }
        }


        /// <summary>
        /// Gets or sets the text offset X.
        /// </summary>
        /// <value>The text offset X.</value>
        public double TextOffsetX
        {
            get
            {
                return (double)GetValue(TextOffsetXProperty);
            }

            set
            {
                SetValue(TextOffsetXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text offset Y.
        /// </summary>
        /// <value>The text offset Y.</value>
        public double TextOffsetY
        {
            get
            {
                return (double)GetValue(TextOffsetYProperty);
            }

            set
            {
                SetValue(TextOffsetYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Rotation Angle of the stripline text
        /// </summary>
        public int TextRotationAngle
        {
            get { return (int)GetValue(TextRotationAngleProperty); }
            set { SetValue(TextRotationAngleProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for TextRotationAngle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextRotationAngleProperty =
            DependencyProperty.Register("TextRotationAngle", typeof(int), typeof(ChartStripLine), new UIPropertyMetadata(0));

        

        #endregion

        #region Implementation
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"></see> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property.Name=="Visibility")
            {
                Axis.Area.Redraw();
            }
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Called when [appearence property changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAppearencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartStripLine chartStripLine = d as ChartStripLine;

            if (args.Property == IsSegmentedProperty)
            {
                if (chartStripLine.IsSegmented == true)
                {
                    chartStripLine.startFromAxis = chartStripLine.StartFromAxis;
                    chartStripLine.StartFromAxis = false;
                }
                else
                {
                    // chartStripLine.StartFromAxis = chartStripLine.startFromAxis;
                }
            }
            if (chartStripLine != null && chartStripLine.Axis != null && chartStripLine.Axis.Area!=null)
            {
                chartStripLine.Axis.Area.Redraw();
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Axis = null;
            this.Interior = null;
            this.Text = null;
            this.ClearValue(ChartStripLine.InteriorProperty);
            this.ClearValue(ChartStripLine.TextProperty);
        }

        #endregion
    }
}
