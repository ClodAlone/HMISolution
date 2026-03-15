// <copyright file="TickBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the tickbar used in the rulers.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class TickBar : FrameworkElement, INotifyPropertyChanged
    {
        #region local variables

        /// <summary>
        /// Used to store the interval value.
        /// </summary>
        private double intvalue = 50;

        /// <summary>
        /// Used to store the range.
        /// </summary>
        private TickBarRange m_range;

        /// <summary>
        /// Used to store the minor lines gap
        /// </summary>
        private double m_minorlinesgap;

        /// <summary>
        /// Used to store the Horizontal ruler instance.
        /// </summary>
        private HorizontalRuler hruler;

        /// <summary>
        /// Used to store the Vertical ruler instance.
        /// </summary>
        private VerticalRuler vruler;

        /// <summary>
        /// Used to store View Instance.
        /// </summary>
        private DiagramView view;

        /// <summary>
        /// Used to store the render point.
        /// </summary>
        private Point renderpoint;

        /// <summary>
        /// Used to store Control instance.
        /// </summary>
        private Control control = new Control();

        /// <summary>
        /// Used to store Unit Change information.
        /// </summary>
        private bool unitchanged = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBar"/> class.
        /// </summary>
        public TickBar()
        {
            this.Loaded += new RoutedEventHandler(TickBar_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the TickBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TickBar_Loaded(object sender, RoutedEventArgs e)
        {
            view = TickBar.GetView((FrameworkElement)this.TemplatedParent).View;
            this.PropertyChanged += new PropertyChangedEventHandler(TickBar_PropertyChanged);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the TickBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TickBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        #endregion

        #region   Properties

        /// <summary>
        /// Gets or sets the original interval.
        /// </summary>
        internal double OriginalInterval
        {
            get
            {
                return intvalue;
            }

            set
            {
                intvalue = value;
            }
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Specifies the MeasurementUnit Dependency property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(TickBar), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnMeasurementUnitChanged)));

        /// <summary>
        /// Specifies the TickBarOrientation Dependency property.
        /// </summary>
        public static readonly DependencyProperty TickBarOrientationProperty = DependencyProperty.Register("TickBarOrientation", typeof(Orientation), typeof(TickBar), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Specifies the Owner Dependency property.
        /// </summary>
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(DiagramControl), typeof(TickBar), new PropertyMetadata(null));

        /// <summary>
        /// Specifies the Interval Dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(TickBar), new PropertyMetadata(50d, new PropertyChangedCallback(OnIntervalChanged)));

        /// <summary>
        /// Specifies the MinorLinesCount Dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLinesCountProperty = DependencyProperty.Register("MinorLinesCount", typeof(double), typeof(TickBar), new PropertyMetadata(4d, new PropertyChangedCallback(OnIntervalChanged)));

        /// <summary>
        /// Specifies the MinorLinesStroke Dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLinesStrokeProperty = DependencyProperty.Register("MinorLinesStroke", typeof(Brush), typeof(TickBar), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnMinorLinesStrokeChanged)));

        /// <summary>
        /// Specifies the MajorLinesStroke Dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorLinesStrokeProperty = DependencyProperty.Register("MajorLinesStroke", typeof(Brush), typeof(TickBar), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnMinorLinesStrokeChanged)));

        /// <summary>
        /// Specifies the LabelFontColor Dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(TickBar), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnLabelFontColorChanged)));

        #endregion

        #region DP setter and getter

        /// <summary>
        /// Gets or sets the units
        /// </summary>
        internal MeasureUnits MeasurementUnits
        {
            get
            {
                return (MeasureUnits)GetValue(MeasurementUnitsProperty);
            }

            set
            {
                SetValue(MeasurementUnitsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tickbar orientation.
        /// </summary>
        internal Orientation TickBarOrientation
        {
            get
            {
                return (Orientation)GetValue(TickBarOrientationProperty);
            }

            set
            {
                SetValue(TickBarOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diagram Control.
        /// </summary>
        internal DiagramControl Owner
        {
            get
            {
                return (DiagramControl)GetValue(OwnerProperty);
            }

            set
            {
                SetValue(OwnerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Interval value
        /// </summary>
        internal double Interval
        {
            get
            {
                return (double)GetValue(IntervalProperty);
            }

            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MajorLines count.
        /// </summary>
        internal double MinorLinesCount
        {
            get
            {
                return (double)GetValue(MinorLinesCountProperty);
            }

            set
            {
                SetValue(MinorLinesCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MajorLinesStroke.
        /// </summary>
        internal Brush MajorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MajorLinesStrokeProperty);
            }

            set
            {
                SetValue(MajorLinesStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LabelFontColor.
        /// </summary>
        internal Brush LabelFontColor
        {
            get
            {
                return (Brush)GetValue(LabelFontColorProperty);
            }

            set
            {
                SetValue(LabelFontColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesStroke.
        /// </summary>
        internal Brush MinorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MinorLinesStrokeProperty);
            }

            set
            {
                SetValue(MinorLinesStrokeProperty, value);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [measurement unit changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMeasurementUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBar tick = (TickBar)d;
            DiagramControl dc = TickBar.GetView((FrameworkElement)tick.TemplatedParent);
            if (dc != null && dc.View != null)
            {
                DiagramView view = dc.View;
                if (view.IsPageEditable)
                {
                    tick.InvalidateVisual();
                }
            }
            tick.unitchanged = true;
        }

        /// <summary>
        /// Gets the DiagramControl.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns> The Diagram Control</returns>
        internal static DiagramControl GetView(FrameworkElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is DiagramControl)
                {
                    return parent as DiagramControl;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            DiagramControl dc = GetView((FrameworkElement)this.TemplatedParent);
            if (dc != null)
            {
                view = dc.View;
                if (view != null)
                {
                    hruler = view.HorizontalRuler;
                    vruler = view.VerticalRuler;
                    this.RenderRuler(drawingContext);
                }
            }
        }

        /// <summary>
        /// Calculates the tick bar range.
        /// </summary>
        /// <returns>The tick bar range</returns>
        internal TickBarRange CalculateTickBarRange()
        {
            if (this.TickBarOrientation == Orientation.Horizontal)
            {
                return new TickBarRange(0, this.ActualWidth);
            }
            else
            {
                return new TickBarRange(0, this.ActualHeight);
            }
        }

        /// <summary>
        /// Gets the ticks position.
        /// </summary>
        /// <returns>Interval value.</returns>
        private double GetPosition()
        {
            double c = MeasureUnitsConverter.FromPixels(50, this.MeasurementUnits);
            double mul = Interval / c;
            switch (this.MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return mul;
                case MeasureUnits.Display:
                    return mul * 40;
                case MeasureUnits.Document:
                    return mul * 150;
                case MeasureUnits.EighthInch:
                    return mul * 4;
                case MeasureUnits.Foot:
                    return mul * 0.04;
                case MeasureUnits.HalfInch:
                    return mul;
                case MeasureUnits.Inch:
                    return mul * .5;
                case MeasureUnits.Kilometer:
                    return mul * 0.00002;
                case MeasureUnits.Meter:
                    return mul * 0.01;
                case MeasureUnits.Mile:
                    return mul * .00001;
                case MeasureUnits.Millimeter:
                    return mul * 13;
                case MeasureUnits.Pixel:
                    double innt = Math.Truncate(Interval);
                    return innt;
                case MeasureUnits.Point:
                    return mul * 40;
                case MeasureUnits.QuarterInch:
                    return mul * 2;
                case MeasureUnits.SixteenthInch:
                    return mul * 8;
                case MeasureUnits.Yard:
                    return mul * .01;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculates the minor lines distance.
        /// </summary>
        /// <param name="range">The range value</param>
        /// <param name="interval">The interval</param>
        /// <param name="smalllinescount">The minor lines count</param>
        /// <returns>Minor lines distance in double</returns>
        internal double CalculateMinorLineDistance(TickBarRange range, double interval, double smalllinescount)
        {
            if (range != null)
            {
                return interval / (smalllinescount + 1);
            }

            return double.NaN;
        }

        /// <summary>
        /// Calculates the minor lines height
        /// </summary>
        /// <param name="orientation">Horizontal or Vertical</param>
        /// <returns>The Minor lines height.</returns>
        internal double CalculateMinorLinesHeight(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return this.ActualHeight / 1.75;
            }
            else
            {
                return this.ActualWidth / 1.75;
            }
        }

        /// <summary>
        /// Calculates the major lines height
        /// </summary>
        /// <param name="orientation">Horizontal or Vertical</param>
        /// <returns>The Major lines height</returns>
        internal double CalculateMajorLinesHeight(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return this.ActualHeight * 0.85;
            }
            else
            {
                return this.ActualWidth * 0.85;
            }
        }

        /// <summary>
        /// Gets the rounding value.
        /// </summary>
        /// <returns>The rounding value</returns>
        private int GetRounding()
        {
            switch (this.MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return 3;
                case MeasureUnits.Display:
                    return 1;
                case MeasureUnits.Document:
                    return 2;
                case MeasureUnits.EighthInch:
                    return 1;
                case MeasureUnits.Foot:
                    return 4;
                case MeasureUnits.HalfInch:
                    return 3;
                case MeasureUnits.Inch:
                    return 4;
                case MeasureUnits.Kilometer:
                    return 8;
                case MeasureUnits.Meter:
                    return 5;
                case MeasureUnits.Mile:
                    return 8;
                case MeasureUnits.Millimeter:
                    return 3;
                case MeasureUnits.Pixel:
                    return 3;
                case MeasureUnits.Point:
                    return 1;
                case MeasureUnits.QuarterInch:
                    return 2;
                case MeasureUnits.SixteenthInch:
                    return 0;
                case MeasureUnits.Yard:
                    return 5;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Renders the ruler.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void RenderRuler(DrawingContext context)
        {
            DiagramControl dc = TickBar.GetView((FrameworkElement)this.TemplatedParent);
            if (dc != null && dc.View != null)
            {
                DiagramView view = dc.View;
                if (unitchanged)
                {
                    view.UpdateRuler(view);
                    unitchanged = false;
                }

                m_range = CalculateTickBarRange();
                if (m_range != null)
                {
                    m_minorlinesgap = CalculateMinorLineDistance(m_range, Interval, this.MinorLinesCount);
                    DrawTickLines(context, m_range, this.TickBarOrientation, m_minorlinesgap);
                }
            }
        }

        private FormattedText getDateTimeString(double labelvalue, Brush labelcolor)
        {
            //labelvalue = MeasureUnitsConverter.ToPixels(labelvalue, (view.Page as DiagramPage).MeasurementUnits);
            if (view != null && view.DateTimeSettings.IsEnabled)
            {
                return new FormattedText(
                   string.Format(view.DateTimeSettings.FormatString == DateTimeFormatSetting.Auto ? view.DateTimeSettings.GetStringFormat() : view.DateTimeSettings.CustomFormatString,
                                labelvalue.ToDateTime(view.DateTimeSettings)).ToString(),
                   CultureInfo.GetCultureInfo("en-us"),
                   System.Windows.FlowDirection.LeftToRight,
                   new Typeface("TimesNewRoman"),
                   11,
                   labelcolor);
            }
            return null;
        }

        /// <summary>
        /// Draws the tick lines.
        /// </summary>
        /// <param name="context">Drawing Context</param>
        /// <param name="range">Range value</param>
        /// <param name="orientation">Horizontal or Vertical</param>
        /// <param name="minorlinesdistance">Minor lines distance</param>
        internal void DrawTickLines(DrawingContext context, TickBarRange range, Orientation orientation, double minorlinesdistance)
        {
            int countx = 0;
            int county = 0;
            renderpoint = new Point(0, 0);
            double minorlinesheight = this.CalculateMinorLinesHeight(orientation);
            double majorlinesheight = this.CalculateMajorLinesHeight(orientation);
            int countpan = (int)(view.ViewGridOrigin.X / minorlinesdistance);
            int countpanY = (int)(view.ViewGridOrigin.Y / minorlinesdistance);
            int c = (int)((range.End - range.Start) / minorlinesdistance);
            countx = c - countpan;
            county = c - countpanY + 15;
            Brush minorpen = this.MinorLinesStroke;
            Brush majorpen = this.MajorLinesStroke;
            double verminorwidth = 0, vermajorwidth = 0, horminorwidth = 0, hormajorwidth = 0;
            if (view.VerticalRuler != null)
            {
                verminorwidth = view.VerticalRuler.MinorLinesThickness;
                vermajorwidth = view.VerticalRuler.MajorLinesThickness;
            }

            if (view.HorizontalRuler != null)
            {
                horminorwidth = view.HorizontalRuler.MinorLinesThickness;
                hormajorwidth = view.HorizontalRuler.MajorLinesThickness;
            }

            Brush labelcolor = this.LabelFontColor;
            double labelvalue = 0;
            double lv = 0;
            double vlabelvalue = 0;
            double vlv = 0;

            if (DiagramView.ViewGridOriginChanged)
            {
                if (view.DupDeleted)
                {
                    //view.X = -(view.Scrollviewer.HorizontalOffset + (view.Page as DiagramPage).CurrentMinX);
                    //view.Y = -(view.Scrollviewer.VerticalOffset + (view.Page as DiagramPage).CurrentMinY);
                    //view.ViewGridOrigin = new Point(view.X, view.Y);
                    //view.DupHorthumbdrag = 0;
                    //view.DupVerthumbdrag = 0;
                }

                renderpoint.X = view.ViewGridOrigin.X;
                renderpoint.Y = view.ViewGridOrigin.Y;
            }
            else
            {
                labelvalue = 0;
                lv = 0;
            }

            int p = GetRounding();
            double defaultinterval = /*Math.Round(PxOriginalInterval, p);//*/Math.Round(MeasureUnitsConverter.FromPixels(OriginalInterval, this.MeasurementUnits), p);

            if (view.ShowHorizontalRulers)
            {
                if (orientation == Orientation.Horizontal)
                {
                    //if (view.CurrentZoom < 1)
                    //{
                    //    this.Width = view.HorizontalRuler.ActualWidth / view.CurrentZoom;
                    //}
                    //else
                    //{
                    //    this.Width = view.HorizontalRuler.ActualWidth * view.CurrentZoom;
                    //}

                    HorizontalRuler ruler = (HorizontalRuler)this.TemplatedParent;

                    for (int i = 0; i <= countx; i++)
                    {
                        if (i % (this.MinorLinesCount + 1) == 0)
                        {
                            string str;
                            FormattedText ftext;
                            if (this.MeasurementUnits == MeasureUnits.Kilometer)
                            {
                                if (labelvalue == 0)
                                {
                                    str = String.Format("{0}", labelvalue);
                                }
                                else
                                    if (view.CurrentZoom < .5)
                                    {
                                        str = String.Format("{0:#.00000}", labelvalue);
                                    }
                                    else if (view.CurrentZoom < 2)
                                    {
                                        str = String.Format("{0:#.000000}", labelvalue);
                                    }
                                    else
                                    {
                                        str = String.Format("{0:#.00000000}", labelvalue);
                                    }

                                ftext = new FormattedText(
                                    str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    System.Windows.FlowDirection.LeftToRight,
                                    new Typeface("TimesNewRoman"),
                                    11,
                                    labelcolor);
                            }
                            else if (this.MeasurementUnits == MeasureUnits.Mile)
                            {
                                if (labelvalue == 0)
                                {
                                    str = String.Format("{0}", labelvalue);
                                }
                                else
                                    if (view.CurrentZoom < .5)
                                    {
                                        str = String.Format("{0:#.00000}", labelvalue);
                                    }
                                    else if (view.CurrentZoom < 2)
                                    {
                                        str = String.Format("{0:#.000000}", labelvalue);
                                    }
                                    else
                                    {
                                        str = String.Format("{0:#.00000000}", labelvalue);
                                    }

                                ftext = new FormattedText(
                                    str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                  System.Windows.FlowDirection.LeftToRight,
                                  new Typeface("TimesNewRoman"),
                                  11,
                                  labelcolor);
                            }
                            else
                            {
                                ftext = new FormattedText(
                                    labelvalue.ToString(),
                                    CultureInfo.GetCultureInfo("en-us"),
                                    System.Windows.FlowDirection.LeftToRight,
                                    new Typeface("TimesNewRoman"),
                                    11,
                                    labelcolor);
                            }

                            if (view != null && view.HorizontalRuler != null && view.HorizontalRuler.ShowDateTime)
                            {
                                FormattedText dateTimeText = getDateTimeString(labelvalue, labelcolor);
                                if (dateTimeText != null)
                                {
                                    ftext = dateTimeText;
                                }
                            }

                            context.DrawText(ftext, new Point(renderpoint.X, majorlinesheight - 8));
                            double vrulerthickness = 0;
                            if (vruler != null)
                            {
                                vrulerthickness = vruler.RulerThickness + 8.5;
                            }

                            if ((renderpoint.X != view.ViewGridOrigin.X - vrulerthickness) && (renderpoint.X != 0))
                            {
                                context.DrawLine(new Pen(majorpen, hormajorwidth), renderpoint, new Point(renderpoint.X, majorlinesheight));
                            }

                            if (ruler != null)
                            {
                                if (hruler.ShowDateTime)
                                {
                                    lv = lv + OriginalInterval;
                                }
                                else
                                {
                                    lv = lv + defaultinterval;
                                }
                                if (this.MeasurementUnits == MeasureUnits.Meter || this.MeasurementUnits == MeasureUnits.Yard)
                                {
                                    labelvalue = Math.Round(lv, 5);
                                }
                                else
                                {
                                    labelvalue = lv;
                                }
                            }
                        }
                        else
                        {
                            context.DrawLine(new Pen(minorpen, horminorwidth), renderpoint, new Point(renderpoint.X, minorlinesheight));
                        }

                        renderpoint = new Point(Math.Round((renderpoint.X + minorlinesdistance), 2), 0);
                    }

                    lv = 0;
                    labelvalue = 0;
                    renderpoint.X = view.ViewGridOrigin.X;
                    if (DiagramView.ViewGridOriginChanged)
                    {
                        for (int i = 0; i <= countpan; i++)
                        {
                            if (i % (this.MinorLinesCount + 1) == 0)
                            {
                                string str;
                                FormattedText ftext;
                                if (this.MeasurementUnits == MeasureUnits.Kilometer)
                                {
                                    if (labelvalue == 0)
                                    {
                                        str = String.Format("{0}", labelvalue);
                                    }
                                    else
                                        if (view.CurrentZoom < .5)
                                        {
                                            str = String.Format("{0:#.00000}", labelvalue);
                                        }
                                        else if (view.CurrentZoom < 2)
                                        {
                                            str = String.Format("{0:#.000000}", labelvalue);
                                        }
                                        else
                                        {
                                            str = String.Format("{0:#.00000000}", labelvalue);
                                        }

                                    ftext = new FormattedText(
                                        str,
                                        CultureInfo.GetCultureInfo("en-us"),
                                        System.Windows.FlowDirection.LeftToRight,
                                        new Typeface("TimesNewRoman"),
                                        11,
                                        labelcolor);
                                }
                                else if (this.MeasurementUnits == MeasureUnits.Mile)
                                {
                                    if (labelvalue == 0)
                                    {
                                        str = String.Format("{0}", labelvalue);
                                    }
                                    else
                                        if (view.CurrentZoom < .5)
                                        {
                                            str = String.Format("{0:#.00000}", labelvalue);
                                        }
                                        else if (view.CurrentZoom < 2)
                                        {
                                            str = String.Format("{0:#.000000}", labelvalue);
                                        }
                                        else
                                        {
                                            str = String.Format("{0:#.00000000}", labelvalue);
                                        }

                                    ftext = new FormattedText(
                                        str,
                                        CultureInfo.GetCultureInfo("en-us"),
                                        System.Windows.FlowDirection.LeftToRight,
                                        new Typeface("TimesNewRoman"),
                                        11,
                                        labelcolor);
                                }
                                else
                                {
                                    ftext = new FormattedText(
                                        labelvalue.ToString(),
                                        CultureInfo.GetCultureInfo("en-us"),
                                        System.Windows.FlowDirection.LeftToRight,
                                        new Typeface("TimesNewRoman"),
                                        11,
                                        labelcolor);
                                }

                                if (view != null && view.HorizontalRuler != null && view.HorizontalRuler.ShowDateTime)
                                {
                                    FormattedText dateTimeText = getDateTimeString(labelvalue, labelcolor);
                                    if (dateTimeText != null)
                                    {
                                        ftext = dateTimeText;
                                    }
                                }

                                context.DrawText(ftext, new Point(renderpoint.X, majorlinesheight - 8));
                                context.DrawLine(new Pen(majorpen, hormajorwidth), renderpoint, new Point(renderpoint.X, majorlinesheight));
                                if (ruler != null)
                                {
                                    if (hruler.ShowDateTime)
                                    {
                                        lv = lv + OriginalInterval;
                                    }
                                    else
                                    {
                                        lv = lv + defaultinterval;
                                    }
                                    if (this.MeasurementUnits == MeasureUnits.Meter || this.MeasurementUnits == MeasureUnits.Yard)
                                    {
                                        labelvalue = Math.Round(-lv, 5);
                                    }
                                    else
                                    {
                                        labelvalue = -lv;
                                    }
                                }
                            }
                            else
                            {
                                context.DrawLine(new Pen(minorpen, horminorwidth), renderpoint, new Point(renderpoint.X, minorlinesheight));
                            }

                            renderpoint = new Point(Math.Round((renderpoint.X - minorlinesdistance), 2), 0);
                        }
                    }
                }
            }

            if (view.ShowVerticalRulers)
            {
                if (orientation == Orientation.Vertical)
                {
                    //if (view.CurrentZoom < 1)
                    //{
                    //    this.Height = view.VerticalRuler.ActualHeight / view.CurrentZoom;
                    //}
                    //else
                    //{
                    //    this.Height = view.VerticalRuler.ActualHeight * view.CurrentZoom;
                    //}

                    VerticalRuler ruler = (VerticalRuler)this.TemplatedParent;
                    for (int i = 0; i <= county; i++)
                    {
                        if (i % (this.MinorLinesCount + 1) == 0)
                        {
                            string str;
                            FormattedText ftext;
                            if (this.MeasurementUnits == MeasureUnits.Mile)
                            {
                                if (vlabelvalue == 0)
                                {
                                    str = String.Format("{0}", vlabelvalue);
                                }
                                else
                                    if (view.CurrentZoom < .5)
                                    {
                                        str = String.Format("{0:#.00000}", vlabelvalue);
                                    }
                                    else if (view.CurrentZoom < 2)
                                    {
                                        str = String.Format("{0:#.000000}", vlabelvalue);
                                    }
                                    else
                                    {
                                        str = String.Format("{0:#.00000000}", vlabelvalue);
                                    }

                                ftext = new FormattedText(
                                    str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    System.Windows.FlowDirection.LeftToRight,
                                    new Typeface("TimesNewRoman"),
                                    11,
                                    labelcolor);
                            }
                            else if (this.MeasurementUnits == MeasureUnits.Kilometer)
                            {
                                if (vlabelvalue == 0)
                                {
                                    str = String.Format("{0}", vlabelvalue);
                                }
                                else
                                    if (view.CurrentZoom < .5)
                                    {
                                        str = String.Format("{0:#.00000}", vlabelvalue);
                                    }
                                    else if (view.CurrentZoom < 2)
                                    {
                                        str = String.Format("{0:#.000000}", vlabelvalue);
                                    }
                                    else
                                    {
                                        str = String.Format("{0:#.00000000}", vlabelvalue);
                                    }

                                ftext = new FormattedText(
                                str,
                                CultureInfo.GetCultureInfo("en-us"),
                                System.Windows.FlowDirection.LeftToRight,
                                new Typeface("TimesNewRoman"),
                                11,
                                labelcolor);
                            }
                            else
                            {
                                ftext = new FormattedText(
                                    vlabelvalue.ToString(),
                                    CultureInfo.GetCultureInfo("en-us"),
                                    System.Windows.FlowDirection.LeftToRight,
                                    new Typeface("TimesNewRoman"),
                                    11,
                                    labelcolor);
                            }

                            if (view != null && view.HorizontalRuler != null && view.VerticalRuler.ShowDateTime)
                            {
                                FormattedText dateTimeText = getDateTimeString(vlabelvalue, labelcolor);
                                if (dateTimeText != null)
                                {
                                    ftext = dateTimeText;
                                }
                            }

                            RotateTransform rt = new RotateTransform(90);
                            rt.CenterX = 15;
                            rt.CenterY = renderpoint.Y;
                            context.PushTransform(rt);
                            if ((renderpoint.Y - majorlinesheight + 11) <= view.VerticalRuler.ActualHeight)
                            {
                                context.DrawText(ftext, new Point(17, renderpoint.Y - majorlinesheight + 11));
                            }

                            RotateTransform rt1 = new RotateTransform(-90);
                            rt1.CenterX = 15;
                            rt1.CenterY = renderpoint.Y;
                            context.PushTransform(rt1);

                            double hrulerthickness = 0;
                            if (hruler != null)
                            {
                                hrulerthickness = hruler.RulerThickness + 4.25;
                            }

                            if ((renderpoint.Y != view.ViewGridOrigin.Y - hrulerthickness) && (renderpoint.Y != 0))
                            {
                                context.DrawLine(new Pen(majorpen, vermajorwidth), renderpoint, new Point(majorlinesheight, renderpoint.Y));
                            }

                            if (ruler != null)
                            {
                                if (vruler.ShowDateTime)
                                {
                                    vlv = vlv + OriginalInterval;
                                }
                                else
                                {
                                    vlv = vlv + defaultinterval;
                                }
                                if (this.MeasurementUnits == MeasureUnits.Meter || this.MeasurementUnits == MeasureUnits.Yard)
                                {
                                    vlabelvalue = Math.Round(vlv, 5);
                                }
                                else
                                {
                                    vlabelvalue = vlv;
                                }
                            }
                        }
                        else
                        {
                            context.DrawLine(new Pen(minorpen, verminorwidth), renderpoint, new Point(minorlinesheight, renderpoint.Y));
                        }

                        renderpoint = new Point(0, Math.Round((renderpoint.Y + minorlinesdistance), 2));
                    }

                    vlv = 0;
                    vlabelvalue = 0;
                    renderpoint.Y = view.ViewGridOrigin.Y;
                    if (DiagramView.ViewGridOriginChanged)
                    {
                        for (int i = 0; i <= countpanY; i++)
                        {
                            if (i % (this.MinorLinesCount + 1) == 0)
                            {
                                string str;
                                FormattedText ftext;
                                if (this.MeasurementUnits == MeasureUnits.Kilometer)
                                {
                                    if (vlabelvalue == 0)
                                    {
                                        str = String.Format("{0}", vlabelvalue);
                                    }
                                    else
                                        if (view.CurrentZoom < .5)
                                        {
                                            str = String.Format("{0:#.00000}", vlabelvalue);
                                        }
                                        else if (view.CurrentZoom < 2)
                                        {
                                            str = String.Format("{0:#.000000}", vlabelvalue);
                                        }
                                        else
                                        {
                                            str = String.Format("{0:#.00000000}", vlabelvalue);
                                        }

                                    ftext = new FormattedText(
                                        str,
                                        CultureInfo.GetCultureInfo("en-us"),
                                       System.Windows.FlowDirection.LeftToRight,
                                       new Typeface("TimesNewRoman"),
                                       11,
                                       labelcolor);
                                }
                                else if (this.MeasurementUnits == MeasureUnits.Mile)
                                {
                                    if (vlabelvalue == 0)
                                    {
                                        str = String.Format("{0}", vlabelvalue);
                                    }
                                    else
                                        if (view.CurrentZoom < .5)
                                        {
                                            str = String.Format("{0:#.00000}", vlabelvalue);
                                        }
                                        else if (view.CurrentZoom < 2)
                                        {
                                            str = String.Format("{0:#.000000}", vlabelvalue);
                                        }
                                        else
                                        {
                                            str = String.Format("{0:#.00000000}", vlabelvalue);
                                        }

                                    ftext = new FormattedText(
                                        str,
                                        CultureInfo.GetCultureInfo("en-us"),
                                        System.Windows.FlowDirection.LeftToRight,
                                        new Typeface("TimesNewRoman"),
                                        11,
                                        labelcolor);
                                }
                                else
                                {
                                    ftext = new FormattedText(
                                        vlabelvalue.ToString(),
                                        CultureInfo.GetCultureInfo("en-us"),
                                        System.Windows.FlowDirection.LeftToRight,
                                        new Typeface("TimesNewRoman"),
                                        11,
                                        labelcolor);
                                }

                                if (view != null && view.HorizontalRuler != null && view.VerticalRuler.ShowDateTime)
                                {
                                    FormattedText dateTimeText = getDateTimeString(vlabelvalue, labelcolor);

                                    if (dateTimeText != null)
                                    {
                                        ftext = dateTimeText;
                                    }
                                }
                                RotateTransform rt = new RotateTransform(90);
                                rt.CenterX = 15;
                                rt.CenterY = renderpoint.Y;
                                context.PushTransform(rt);
                                context.DrawText(ftext, new Point(17, renderpoint.Y - majorlinesheight + 11));
                                RotateTransform rt1 = new RotateTransform(-90);
                                rt1.CenterX = 15;
                                rt1.CenterY = renderpoint.Y;
                                context.PushTransform(rt1);
                                context.DrawLine(new Pen(majorpen, vermajorwidth), renderpoint, new Point(majorlinesheight, renderpoint.Y));
                                if (ruler != null)
                                {
                                    if (vruler.ShowDateTime)
                                    {
                                        vlv = vlv + OriginalInterval;
                                    }
                                    else
                                    {
                                        vlv = vlv + defaultinterval;
                                    }
                                    if (this.MeasurementUnits == MeasureUnits.Meter || this.MeasurementUnits == MeasureUnits.Yard)
                                    {
                                        vlabelvalue = Math.Round(-vlv, 5);
                                    }
                                    else
                                    {
                                        vlabelvalue = -vlv;
                                    }
                                }
                            }
                            else
                            {
                                context.DrawLine(new Pen(minorpen, verminorwidth), renderpoint, new Point(minorlinesheight, renderpoint.Y));
                            }

                            renderpoint = new Point(0, Math.Round((renderpoint.Y - minorlinesdistance), 2));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [interval changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBar tick = (TickBar)d;
            tick.InvalidateVisual();
        }

        /// <summary>
        /// Called when [orientation changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBar tick = (TickBar)d;
            tick.InvalidateVisual();
        }

        /// <summary>
        /// Called when [minor lines stroke changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinorLinesStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBar tick = (TickBar)d;
            tick.InvalidateVisual();
        }

        /// <summary>
        /// Called when [label font color changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBar tick = (TickBar)d;
            tick.InvalidateVisual();
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Calls property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            this.InvalidateVisual();
        }
        #endregion
    }

    /// <summary>
    /// Represents the tickbar range.
    /// </summary>
    public class TickBarRange
    {
        #region Fields

        /// <summary>
        /// Used to store the start value
        /// </summary>
        private double startvalue;

        /// <summary>
        /// Used to store the end value
        /// </summary>
        private double endvalue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Start value .
        /// </summary>
        internal double Start
        {
            get
            {
                return startvalue;
            }

            set
            {
                startvalue = value;
            }
        }

        /// <summary>
        /// Gets or sets the End value .
        /// </summary>
        internal double End
        {
            get
            {
                return endvalue;
            }

            set
            {
                endvalue = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBarRange"/> class.
        /// </summary>
        public TickBarRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBarRange"/> class.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        public TickBarRange(double start, double end)
        {
            startvalue = start;
            endvalue = end;
        }
    }
}
