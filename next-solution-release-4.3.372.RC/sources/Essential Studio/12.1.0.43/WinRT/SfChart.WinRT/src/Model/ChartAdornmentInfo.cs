#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using WindowsPolyLineSegment = System.Windows.Media.PolyLineSegment;
using WindowsLineSegment = System.Windows.Media.LineSegment;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using WindowsPolyLineSegment = Windows.UI.Xaml.Media.PolyLineSegment;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the class used for configuring chart adornments for chart.
    /// </summary>
    public abstract class ChartAdornmentInfoBase : DependencyObject, ICloneable
    {
        #region fields

        internal UIElementsRecycler<ContentControl> LabelPresenters { get; set; }

        internal UIElementsRecycler<Path> ConnectorLines { get; set; }

        internal UIElementsRecycler<ChartAdornmentContainer> adormentContainers;

        internal ChartSeriesBase series;

        bool isCollectionChanged;

        internal Size AdornmentInfoSize { get; set; }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable default adornment].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable default adornment]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSeriesPalette
        {
            get { return (bool)GetValue(UseSeriesPaletteProperty); }
            set { SetValue(UseSeriesPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableColorEach.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseSeriesPaletteProperty =
            DependencyProperty.Register("UseSeriesPalette", typeof(bool), typeof(ChartAdornmentInfoBase), new PropertyMetadata(false, OnDefaultAdornmentChanged));

        private static void OnDefaultAdornmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var adornmentInfo = ((ChartAdornmentInfoBase)d);
            if (adornmentInfo != null)
            {
                adornmentInfo.UpdateLabels();
                adornmentInfo.UpdateConnectingLines();
                adornmentInfo.OnAdornmentPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or Sets the horizontal alignment of the label
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        ///  Identifies the HorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
          DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentInfoBase), new PropertyMetadata(HorizontalAlignment.Center, OnAdornmentPropertyChanged));

        /// <summary>
        /// Gets or Sets the vertical alignment of the label
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }
        /// <summary>
        ///  Identifies the VerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
         DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentInfoBase), new PropertyMetadata(VerticalAlignment.Center, OnAdornmentPropertyChanged));

        /// <summary>
        /// Gets or Sets the connector line height
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ConnectorHeight
        {
            get { return (double)GetValue(ConnectorHeightProperty); }
            set { SetValue(ConnectorHeightProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ConnectorHeight.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ConnectorHeightProperty =
            DependencyProperty.Register("ConnectorHeight", typeof(double), typeof(ChartAdornmentInfoBase), new PropertyMetadata(0d,OnAdornmentPropertyChanged));

        /// <summary>
        /// Gets or Sets the connector line angle.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ConnectorRotationAngle
        {
            get { return (double)GetValue(ConnectorRotationAngleProperty); }
            set { SetValue(ConnectorRotationAngleProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ConnectorRotationAngle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ConnectorRotationAngleProperty =
            DependencyProperty.Register("ConnectorRotationAngle", typeof(double), typeof(ChartAdornmentInfoBase), new PropertyMetadata(90d, OnAdornmentPropertyChanged));

        /// <summary>
        /// Gets or Sets the connector line style
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Style ConnectorLineStyle
        {
            get { return (Style)GetValue(ConnectorLineStyleProperty); }
            set { SetValue(ConnectorLineStyleProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ConnectorLineStyle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ConnectorLineStyleProperty =
            DependencyProperty.Register("ConnectorLineStyle", typeof(Style), typeof(ChartAdornmentInfoBase), new PropertyMetadata(null, OnShowConnectingLine));

        /// <summary>
        /// Gets or Sets a value that determines whether to show/hide connector line.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowConnectorLine
        {
            get { return (bool)GetValue(ShowConnectorLineProperty); }
            set { SetValue(ShowConnectorLineProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowConnectorLine.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowConnectorLineProperty =
            DependencyProperty.Register("ShowConnectorLine", typeof(bool), typeof(ChartAdornmentInfoBase), new PropertyMetadata(false, OnShowConnectingLine));

        /// <summary>
        /// Gets or Sets the label template.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfoBase), new PropertyMetadata(null, OnLabelTemplatePropertyChanged));

        private static void OnLabelTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAdornmentInfoBase).UpdateLabels();
        }

        /// <summary>
        /// Gets or Sets the symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSymbol Symbol
        {
            get { return (ChartSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(ChartSymbol), typeof(ChartAdornmentInfoBase), new PropertyMetadata(ChartSymbol.Custom, OnAdornmentPropertyChanged));

        /// <summary>
        /// Gets or Sets the width of the symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double SymbolWidth
        {
            get { return (double)GetValue(SymbolWidthProperty); }
            set { SetValue(SymbolWidthProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolWidthProperty =
            DependencyProperty.Register("SymbolWidth", typeof(double), typeof(ChartAdornmentInfoBase), new PropertyMetadata(20d, OnSymbolSizeChanged));

        /// <summary>
        /// Gets or Sets the height of the symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double SymbolHeight
        {
            get { return (double)GetValue(SymbolHeightProperty); }
            set { SetValue(SymbolHeightProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolHeightProperty =
            DependencyProperty.Register("SymbolHeight", typeof(double), typeof(ChartAdornmentInfoBase), new PropertyMetadata(20d,OnSymbolSizeChanged));

        private static void OnSymbolSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateArea();
        }

        private static void OnAdornmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           (d as ChartAdornmentInfoBase).OnAdornmentPropertyChanged();
        }

        internal void OnAdornmentPropertyChanged()
        {
            if (this is ChartAdornmentInfo)
            {
                if (adormentContainers != null && adormentContainers.Count > 0)
                {
                    this.Measure(AdornmentInfoSize, null);
                    this.Arrange(AdornmentInfoSize);
                }
            }
            else if(series != null)
            {
                series.ActualArea.ScheduleUpdate();
            }
        }

        
        /// <summary>
        /// Gets or Sets the data template for symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate SymbolTemplate
        {
            get { return (DataTemplate)GetValue(SymbolTemplateProperty); }
            set { SetValue(SymbolTemplateProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SymbolTemplateProperty =
            DependencyProperty.Register("SymbolTemplate", typeof(DataTemplate), typeof(ChartAdornmentInfoBase), new PropertyMetadata(null, OnSymbolTemplateChanged));

        private static void OnSymbolTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as ChartAdornmentInfo).adormentContainers!=null)
                (d as ChartAdornmentInfo).adormentContainers.GenerateElements(0);
            (d as ChartAdornmentInfo).UpdateAdornments();
            (d as ChartAdornmentInfo).OnAdornmentPropertyChanged();
        }

        /// <summary>
        /// Gets or Sets the background of the symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SymbolInterior
        {
            get { return (Brush)GetValue(SymbolInteriorProperty); }
            set { SetValue(SymbolInteriorProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolInterior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolInteriorProperty =
            DependencyProperty.Register("SymbolInterior", typeof(Brush), typeof(ChartAdornmentInfoBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the stroke of the symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SymbolStroke
        {
            get { return (Brush)GetValue(SymbolStrokeProperty); }
            set { SetValue(SymbolStrokeProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeProperty =
            DependencyProperty.Register("SymbolStroke", typeof(Brush), typeof(ChartAdornmentInfoBase), new PropertyMetadata(null));

       
        /// <summary>
        /// Gets or sets the segment label font family.
        /// </summary>
        /// <remarks>
        /// Identifies font family that should be used to display adornment's text.
        /// </remarks>
        /// <value>
        /// The segment label font family.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets the owner series
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesBase Series
        {
            get
            {
                return series;
            }
            internal set
            {
                if (series != null)
                {
                    series.Adornments.CollectionChanged -= Adornments_CollectionChanged;
                }
                series = value;
                if (series != null)
                {
                    if (adormentContainers != null)
                    {
                        adormentContainers.GenerateElements(series.Adornments.Count);
                    }
                    series.Adornments.CollectionChanged += Adornments_CollectionChanged;
                }
            }
        }

        /// <summary>
        /// Identifies the SegmentLabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
                DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(ChartAdornmentInfoBase), new PropertyMetadata(new FontFamily("Times New Roman")));

        /// <summary>
        /// Gets or Sets the AdornmentsPosition
        /// <seealso cref="AdornmentsPosition"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public AdornmentsPosition AdornmentsPosition
        {
            get { return (AdornmentsPosition)GetValue(AdornmentsPositionProperty); }
            set { SetValue(AdornmentsPositionProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for AdornmentsPosition. This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty AdornmentsPositionProperty =
            DependencyProperty.Register("AdornmentsPosition", typeof(AdornmentsPosition), typeof(ChartAdornmentInfoBase), new PropertyMetadata(AdornmentsPosition.Top,OnAdornmentPositionChanged));

        private static void OnAdornmentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateArea();
        }

        /// <summary>
        /// Gets or Sets the actual label content to be displayed in the label
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public LabelContent SegmentLabelContent
        {
            get { return (LabelContent)GetValue(SegmentLabelContentProperty); }
            set { SetValue(SegmentLabelContentProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SegmentLabelContent.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SegmentLabelContentProperty =
            DependencyProperty.Register("SegmentLabelContent", typeof(LabelContent), typeof(ChartAdornmentInfoBase), new PropertyMetadata(LabelContent.YValue, OnLabelChanged));

        /// <summary>
        /// Gets or Sets the segment labels format
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string SegmentLabelFormat
        {
            get { return (string)GetValue(SegmentLabelFormatProperty); }
            set { SetValue(SegmentLabelFormatProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for SegmentLabelFormat.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFormatProperty =
            DependencyProperty.Register("SegmentLabelFormat", typeof(string), typeof(ChartAdornmentInfoBase), new PropertyMetadata(string.Empty,OnLabelChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether to show/hide marker symbol.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowMarker
        {
            get { return (bool)GetValue(ShowMarkerProperty); }
            set { SetValue(ShowMarkerProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowMarker.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ShowMarkerProperty =
            DependencyProperty.Register("ShowMarker", typeof(bool), typeof(ChartAdornmentInfoBase), new PropertyMetadata(true, OnShowMarker));

        /// <summary>
        /// Gets or sets a value that indicates whether to show/hide label.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool ShowLabel
        {
            get { return (bool)GetValue(ShowLabelProperty); }
            set { SetValue(ShowLabelProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowLabel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowLabelProperty =
            DependencyProperty.Register("ShowLabel", typeof(bool), typeof(ChartAdornmentInfoBase), new PropertyMetadata(false, OnLabelChanged));

        #endregion

        #region methods

        /// <summary>
        /// Aligns the element.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        internal void AlignElement(Control control, ChartAlignment verticalAlignment, ChartAlignment horizontalAlignment,
         double x, double y)
        {
            if (horizontalAlignment == ChartAlignment.Near)
            {
                x = x - control.DesiredSize.Width;
            }
            else if (horizontalAlignment == ChartAlignment.Center)
            {
                x = x - control.DesiredSize.Width / 2;
            }

            if (verticalAlignment == ChartAlignment.Near)
            {
                y = y - control.DesiredSize.Height;
            }
            else if (verticalAlignment == ChartAlignment.Center)
            {
                y = y - control.DesiredSize.Height / 2;
            }

            //control.Arrange(new Rect(x, y, control.DesiredSize.Width, control.DesiredSize.Height));
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        /// <summary>
        /// Smarts the labels for inside.
        /// </summary>
        /// <param name="adornment">The adornment.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="label">The label.</param>
        /// <param name="connectorHeight">Height of the connector.</param>
        /// <param name="labelRadiusFromOrigin">The label radius from origin.</param>
        /// <param name="pieRadius">The pie radius.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="center">The center.</param>
        /// <param name="currRect">The curr rect.</param>
        /// <returns></returns>
        private static Point SmartLabelsForInside(ChartAdornment adornment, IList<Rect> bounds, ContentControl label, double connectorHeight, double labelRadiusFromOrigin, double pieRadius, List<Point> drawingPoints, Point center, Rect currRect)
        {
            bool isIntersectedLabel, isIntersected = false;

            labelRadiusFromOrigin = pieRadius + connectorHeight + (label.DesiredSize.Width + label.DesiredSize.Height) / 2;
            var angle = adornment.ConnectorRotationAngle;
            double x = currRect.X, y = currRect.Y, baseAngle = angle;
            do
            {
                isIntersectedLabel = false;
                if (!bounds.IntersectWith(currRect)) continue;
                isIntersected = true;
                //Increment the angle by radiant to  check with the overlap.
                baseAngle += 0.01;
                x = center.X + (Math.Cos((baseAngle)) * labelRadiusFromOrigin);
                y = center.Y + (Math.Sin((baseAngle)) * labelRadiusFromOrigin);
                currRect.X = x;
                currRect.Y = y;
                isIntersectedLabel = true;
            } while (isIntersectedLabel);

            if (isIntersected)
            {
                //If the labels is intersected means, we need to draw connector line and it should be position as like outside
                drawingPoints.Clear();
                drawingPoints.Add(new Point(center.X + (Math.Cos((angle)) * pieRadius), center.Y + (Math.Sin((angle)) * pieRadius)));
                drawingPoints.Add(new Point(center.X + (Math.Cos((angle)) * (pieRadius)), center.Y + (Math.Sin((angle)) * (pieRadius ))));
            }

            drawingPoints.Add(new Point(x, y));
            bounds.Add(currRect);
            if (isIntersected)
            {
                //If the labels is intersected means, we need to position the labels outside with connector lines form edges.
                var isRight = ((angle % (Math.PI * 2) <= 1.55 && angle % (Math.PI * 2) >= 0) || (angle % (Math.PI * 2) >= 4.71));
                x += isRight ? label.DesiredSize.Width / 2 : -(label.DesiredSize.Width / 2);
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Smarts the labels for outside.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="currRect">The curr rect.</param>
        /// <param name="label">The label.</param>
        /// <param name="center">The center.</param>
        /// <param name="labelRadiusFromOrigin">The label radius from origin.</param>
        /// <param name="connectorHeight">Height of the connector.</param>
        /// <param name="explodedRadius">The exploded radius.</param>
        /// <param name="pieAdornment">The pie adornment.</param>
        /// <returns></returns>
        private Point SmartLabelsForOutside(IList<Rect> bounds, IList<Point> drawingPoints, Rect currRect, ContentControl label, Point center, double labelRadiusFromOrigin,
            double connectorHeight, double explodedRadius, ChartAdornment pieAdornment)
        {
            double x, y;
            var startAngle = 0d;
            var angle = pieAdornment.ConnectorRotationAngle;
            if (pieAdornment.Series is CircularSeriesBase)
                startAngle = ((CircularSeriesBase) pieAdornment.Series).StartAngle*Math.PI/180;
            var baseAngle = angle;
            bool isIntersected = false, isIntersectedLabel;
            //Since  no need to draw the lines to edges its need to like hipen.
            drawingPoints.RemoveAt(1);
            do
            {
                isIntersectedLabel = false;
                if (!bounds.IntersectWith(currRect)) continue;
                isIntersected = isIntersectedLabel = true;
                //If the label don’t have a place in chart area means we need to collapse the lables.
                if (angle > Math.PI * 2 + startAngle)
                {
                    label.Visibility = Visibility.Collapsed;
                    isIntersected = isIntersectedLabel = false;
                    var labelIndex = LabelPresenters.IndexOf(label);
                    if (ConnectorLines.Count > labelIndex)
                    {
                       ConnectorLines[labelIndex].Visibility = Visibility.Collapsed;
                    }
                }
                angle += 0.01;
                x = center.X + (Math.Cos((angle)) * labelRadiusFromOrigin);
                y = center.Y + (Math.Sin((angle)) * labelRadiusFromOrigin);
                currRect.X = x;
                currRect.Y = y;
            } while (isIntersectedLabel);

            x = currRect.X;
            y = currRect.Y;
            bounds.Add(currRect);
            drawingPoints.Add(isIntersected
                ? new Point(
                    pieAdornment.X + (Math.Cos((baseAngle)) * (connectorHeight + explodedRadius - connectorHeight / 1.5)),
                    pieAdornment.Y + (Math.Sin((baseAngle)) * (connectorHeight + explodedRadius - connectorHeight / 1.5)))
                : new Point(x, y));
            drawingPoints.Add(new Point(x, y));
            var pieAngle = angle % (Math.PI * 2);
            //Checks, whether this labels placed over right side or left side
            var isRight = ((pieAngle <= (isIntersected ? 1.35 : 1.55) && pieAngle >= 0) || (pieAngle >= (isIntersected ? 4.51 : 4.71)));
            var hipen = connectorHeight / 5;
            x += isRight ? hipen : -hipen;
            drawingPoints.Add(new Point(x, y));
            //Label will be placed at the edge of the connector lines
            x += isRight ? label.DesiredSize.Width / 2 : -label.DesiredSize.Width / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// Gets the adornment positions.
        /// </summary>
        /// <param name="pieRadius">The pie radius.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="finalSize">The final size.</param>
        /// <param name="adornment">The adornment.</param>
        /// <param name="labelIndex">Index of the label.</param>
        /// <param name="pieLeft">The pie left.</param>
        /// <param name="pieRight">The pie right.</param>
        /// <param name="label">The label.</param>
        /// <param name="series">The series.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        internal List<Point> GetAdornmentPositions(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment, int labelIndex, double pieLeft, double pieRight, ContentControl label, ChartSeriesBase series, ref double x, ref double y)
        {
            var connectorHeight = adornment.ConnectorHeight;
            var labelRadiusFromOrigin = connectorHeight;

            var drawingPoints = new List<Point> { new Point(x, y) };
            var center = new Point(x, y);
            var isPie = adornment is ChartPieAdornment || adornment is ChartPieAdornment3D;
            var angle = isPie ? adornment.ConnectorRotationAngle : (6.28*(1 - (adornment.ConnectorRotationAngle/360.0)));
            //We need to do some rotation if the series is circular series
            if (isPie)
            {
                drawingPoints.Clear();
                double explodedRadius;
                int explodeIndex;
                CircularSeriesLabelPosition labelPosition;
                bool enableSmartLabels;
                //Get the values like explode radius, index.., from pie series
                if (series is CircularSeriesBase)
                {
                    var circularSeriesBase = series as CircularSeriesBase;
                    explodedRadius = circularSeriesBase.ExplodeRadius;
                    explodeIndex = circularSeriesBase.ExplodeIndex;
                    labelPosition = circularSeriesBase.LabelPosition;
                    explodeIndex = circularSeriesBase.ExplodeAll ? -2 : explodeIndex;
                    enableSmartLabels = circularSeriesBase.EnableSmartLabels && ShowLabel;
                }
                else
                {
                    var circularSeriesBase3D = series as CircularSeriesBase3D;
                    explodedRadius = circularSeriesBase3D.ExplodeRadius;
                    explodeIndex = circularSeriesBase3D.ExplodeIndex;
                    labelPosition = circularSeriesBase3D.LabelPosition;
                    explodeIndex = circularSeriesBase3D.ExplodeAll ? -2 : explodeIndex;
                    enableSmartLabels = circularSeriesBase3D.EnableSmartLabels && ShowLabel;
                }
                explodedRadius = explodeIndex == labelIndex || explodeIndex == -2 ? explodedRadius : 0d;

                center = new Point(finalSize.Width / 2, finalSize.Height / 2);
                labelRadiusFromOrigin = pieRadius / 2 + connectorHeight;
                if (labelPosition != CircularSeriesLabelPosition.Inside)
                {
                    labelRadiusFromOrigin = pieRadius + connectorHeight;
                    center.X = center.X + (Math.Cos((angle))*explodedRadius);
                    center.Y = center.Y + (Math.Sin((angle))*explodedRadius);

                    drawingPoints.Add(new Point(center.X + (Math.Cos((angle))*pieRadius), center.Y + (Math.Sin((angle))*pieRadius)));
                    x = center.X + (Math.Cos((angle))*(labelRadiusFromOrigin));
                    y = center.Y + (Math.Sin((angle))*(labelRadiusFromOrigin));
                    drawingPoints.Add(new Point(x, y));
                }
                else
                {
                    x = x + (Math.Cos((angle))*explodedRadius);
                    y = y + (Math.Sin((angle))*explodedRadius);
                    drawingPoints.Add(new Point(x, y));
                    x = x + (Math.Cos((angle))*connectorHeight);
                    y = y + (Math.Sin((angle))*connectorHeight);
                    drawingPoints.Add(new Point(x, y));
                }
                //If the smart labels are enabled means we have to check for overlap else just place the labels in calculated positions
                if (enableSmartLabels)
                {
                    var currRect = new Rect(x, y, label.DesiredSize.Width, label.DesiredSize.Height);
                    switch (labelPosition)
                    {
                        case CircularSeriesLabelPosition.Inside:
                            {
                                var point = SmartLabelsForInside(adornment, bounds, label, connectorHeight, labelRadiusFromOrigin, pieRadius + explodedRadius, drawingPoints, center, currRect);
                                x = point.X;
                                y = point.Y;
                            }
                            break;
                        case CircularSeriesLabelPosition.Outside:
                            {
                                var point = SmartLabelsForOutside(bounds, drawingPoints, currRect, label, center, labelRadiusFromOrigin, connectorHeight, explodedRadius, adornment);
                                x = point.X;
                                y = point.Y;
                            }
                            break;
                    }
                }
                else if (labelPosition == CircularSeriesLabelPosition.OutsideExtended)
                {
                    //Calculation for default outside extended adornment position width
                    double baseRight = pieRight, baseLeft = pieLeft;
                    pieLeft = (finalSize.Width / 2 - pieRadius) - pieRadius;
                    pieRight = (finalSize.Width / 2 + pieRadius) + pieRadius;
                    x = center.X + (Math.Cos((angle)) * (pieRadius + pieRadius * 0.2));
                    y = center.Y + (Math.Sin((angle)) * (pieRadius + pieRadius * 0.2));
                    drawingPoints[1] = new Point(x,y);
                    pieRight = pieRight > baseRight ? baseRight : pieRight;
                    pieLeft = pieLeft < baseLeft ? baseLeft : pieLeft;
                    var pieAngle = angle % (Math.PI * 2);
                    if ((pieAngle <= 1.57 && pieAngle >= 0) || pieAngle >= 4.71)
                    {
                        x = x < pieRight ? pieRight : x;
                    }
                    else
                    {
                        x = x > pieLeft ? pieLeft : x;
                    }

                    drawingPoints.Add(new Point(x, y));
                }
            }
            else
            {
                x = center.X + (Math.Cos((angle)) * labelRadiusFromOrigin);
                y = center.Y + (Math.Sin((angle)) * labelRadiusFromOrigin);
                drawingPoints.Add(new Point(x, y));
            }
            return drawingPoints;
        }

        /// <summary>
        /// Gets the bezier approximation.
        /// </summary>
        /// <param name="controlPoints">The control points.</param>
        /// <param name="outputSegmentCount">The output segment count.</param>
        /// <returns></returns>
        internal List<Point> GetBezierApproximation(IList<Point> controlPoints, int outputSegmentCount)
        {
            var points = new List<Point>();
            for (var i = 0; i <= outputSegmentCount; i++)
            {
                var t = (double)i / outputSegmentCount;
                points.Add(GetBezierPoint(t, controlPoints, 0, controlPoints.Count));
            }
            return points;
        }

        /// <summary>
        /// Gets the bezier point.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="controlPoints">The control points.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        static Point GetBezierPoint(double t, IList<Point> controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var p0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var p1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Point((1 - t) * p0.X + t * p1.X, (1 - t) * p0.Y + t * p1.Y);
        }

        /// <summary>
        /// Draws the line segment.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="path">The path.</param>
        protected virtual void DrawLineSegment(List<Point> points, Path path)
        {

        }

        /// <summary>
        /// Draws the connecotr line.
        /// </summary>
        /// <param name="connectorIndex">Index of the connector.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="connectorLineMode">The connector line mode.</param>
        internal void DrawConnecotrLine(int connectorIndex, List<Point> drawingPoints, ConnectorMode connectorLineMode)
        {
            if (ConnectorLines.Count <= connectorIndex) return;
            var element = ConnectorLines[connectorIndex];
            if (connectorLineMode == ConnectorMode.Bezier)
                drawingPoints = GetBezierApproximation(drawingPoints, 256);
            DrawLineSegment(drawingPoints, element);
        }

        /// <summary>
        /// Panels the changed.
        /// </summary>
        /// <param name="panel">The panel.</param>
        internal void PanelChanged(Panel panel)
        {
            if (SymbolInterior == null)
            {
                Binding binding = new Binding();
                binding.Source = series;
                binding.Path = new PropertyPath("Interior");
                binding.Converter = new InteriorConverter(series);
                binding.ConverterParameter = 1;
                BindingOperations.SetBinding(this, ChartAdornmentInfoBase.SymbolInteriorProperty, binding);
            }

            if (SymbolStroke == null)
            {
                Binding binding = new Binding();
                binding.Source = series;
                binding.Path = new PropertyPath("Interior");
                binding.Converter = new InteriorConverter(series);
                binding.ConverterParameter = 1;
                BindingOperations.SetBinding(this, ChartAdornmentInfoBase.SymbolStrokeProperty, binding);
            }

            if (LabelPresenters == null)
                LabelPresenters = new UIElementsRecycler<ContentControl>(panel);

            if (ConnectorLines == null)
                ConnectorLines = new UIElementsRecycler<Path>(panel);

            if (adormentContainers == null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);

            UpdateAdornments();
            UpdateLabels();
            UpdateConnectingLines();
        }

        internal void ClearChildren()
        {
            if (LabelPresenters != null)
            {
                LabelPresenters.Clear();
            }

            if (adormentContainers != null)
            {
                adormentContainers.Clear();
            }

            if (ConnectorLines != null)
                ConnectorLines.Clear();
        }

        internal void AddAdornment(UIElement element, Panel panel)
        {
            if (adormentContainers == null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);
            adormentContainers.Add(element as ChartAdornmentContainer);
        }

        internal void RemoveAdornment(UIElement element)
        {
            adormentContainers.Remove(element as ChartAdornmentContainer);
        }

        private static void OnShowConnectingLine(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateConnectingLines();
            (d as ChartAdornmentInfoBase).OnAdornmentPropertyChanged();
        }

        private static void OnShowMarker(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateAdornments();
            if(Convert.ToBoolean(e.NewValue))
                (d as ChartAdornmentInfo).UpdateArea();
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateLabels();
            (d as ChartAdornmentInfoBase).OnAdornmentPropertyChanged();
          
        }

        void Adornments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            isCollectionChanged = true;
        }

        void UpdateArea()
        {
            if (this.Series != null && this.Series.ActualArea != null)
                this.Series.ActualArea.ScheduleUpdate();
        }

        private void UpdateAdornments()
        {
            if (adormentContainers != null && series != null && ShowMarker)
            {
                adormentContainers.GenerateElements(series.Adornments.Count);
            }
            else if (adormentContainers != null)
            {
                adormentContainers.GenerateElements(0);
            }
            if (series == null) return;
            foreach (var adornment in series.Adornments)
            {
                adornment.Series = series;
                adornment.CreateSegmentVisual(Size.Empty);
                Binding binding = new Binding { Source = this, Path = new PropertyPath("ConnectorHeight") };
                BindingOperations.SetBinding(adornment, ChartAdornment.ConnectorHeightProperty, binding);
                if (series is CircularSeriesBase || series is CircularSeriesBase3D)
                    binding = new Binding {Source = adornment, Path = new PropertyPath("Angle")};
                else
                    binding = new Binding {Source = this, Path = new PropertyPath("ConnectorRotationAngle")};
                BindingOperations.SetBinding(adornment, ChartAdornment.ConnectorRotationAngleProperty, binding);
            }
        }

        internal void UpdateLabels()
        {
            if (ShowLabel && LabelPresenters != null && series != null)
            {
                DataTemplate labelTemplate = null;
                    labelTemplate = LabelTemplate ?? (UseSeriesPalette ? ChartDictionaries.GenericCommonDictionary["AdornmentLabelTemplate"] as DataTemplate : LabelTemplate);
                LabelPresenters.GenerateElements(series.Adornments.Count);

                for (var i = 0; i < LabelPresenters.Count; i++)
                {
                    var label = LabelPresenters[i];
                    var binding = CreateAdormentBinding("ActualContent", Series.Adornments[i]);
                    label.SetBinding(ContentControl.ContentProperty, binding);
                    label.ContentTemplate = labelTemplate;
                }
            }
            else if (LabelPresenters != null)
            {
                LabelPresenters.GenerateElements(0);
            }
        }

        internal void UpdateConnectingLines()
        {
            if (ShowConnectorLine && ConnectorLines != null && series != null)
            {
                //if (!Connectors.BindingProvider.Keys.Contains(Line.StyleProperty))
                //{
                //    Binding binding = new Binding();
                //    binding.Source = this;
                //    binding.Path = new PropertyPath("ConnectorLineStyle");
                //    Connectors.BindingProvider.Add(Line.StyleProperty, binding);
                //}

                ConnectorLines.GenerateElements(Series.Adornments.Count);
                if (UseSeriesPalette)
                {
                    for (var i = 0; i < ConnectorLines.Count; i++)
                    {
                        var binding = new Binding
                        {
                            Source = series.Adornments[i],
                            Path = new PropertyPath("Interior")
                        };
                        ConnectorLines[i].SetBinding(Shape.StrokeProperty, binding);
                    }
                }
                else
                    foreach (var line in ConnectorLines.Where(item => item.Stroke != null))
                    {
                        line.ClearValue(Shape.StrokeProperty);
                    }

                var connectorStyle = ConnectorLineStyle ?? ChartDictionaries.GenericCommonDictionary["pathStyle"] as Style;
                foreach (var path in ConnectorLines)
                {
                    path.Style = connectorStyle;
                }
            }
            else if (ConnectorLines != null)
            {
                ConnectorLines.GenerateElements(0);
            }
        }

        internal virtual void Arrange(Size finalSize)
        { 
        
        }

        internal void UpdateElements()
        {
            if (isCollectionChanged)
            {
                UpdateAdornments();
                UpdateLabels();
                UpdateConnectingLines();
                isCollectionChanged = false;
            }
        }

        internal void Measure(Size availableSize, Panel panel)
        {
            if (LabelPresenters == null&& panel!=null)
                LabelPresenters = new UIElementsRecycler<ContentControl>(panel);

            if (ConnectorLines == null && panel != null)
                ConnectorLines = new UIElementsRecycler<Path>(panel);

            if (adormentContainers == null && panel != null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);

            int adornmentIndex = 0;
            foreach (ChartAdornmentContainer element in this.adormentContainers)
            {
                element.Adornment = series.Adornments[adornmentIndex];
                element.Measure(availableSize);
                adornmentIndex++;
            }

            int i = 0;
            if (ShowLabel)
            {
                for (; i < this.LabelPresenters.Count; i++)
                {
                    this.LabelPresenters[i].Measure(availableSize);
                    Canvas.SetZIndex(this.LabelPresenters[i], 4);
                }
            }
        }

        private Binding CreateAdormentBinding(string path, object source)
        {
            Binding bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(path);
            bindingProvider.Source = source;
            bindingProvider.Mode = BindingMode.OneWay;
            return bindingProvider;
        }

        protected ChartAlignment GetVerticalAlignment(VerticalAlignment alignment)
        {
            if (alignment == VerticalAlignment.Bottom)
                return ChartAlignment.Far;
            if (alignment == VerticalAlignment.Top)
                return ChartAlignment.Near;
            return ChartAlignment.Center;
        }

        protected ChartAlignment GetHorizontalAlignment(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.Right)
                return ChartAlignment.Far;
            if (alignment == HorizontalAlignment.Left)
                return ChartAlignment.Near;
            return ChartAlignment.Center;
        }

        internal virtual DependencyObject CloneAdornmentInfo()
        {
            return null;
        }

        /// <summary>
        /// Updates the spider labels.
        /// </summary>
        /// <param name="pieLeft">The pie left.</param>
        /// <param name="pieRight">The pie right.</param>
        /// <param name="finalSize"></param>
        internal void UpdateSpiderLabels(double pieLeft, double pieRight, Size finalSize, double radius)
        {
            var orderedAdornments = GetOrderedAdornments();

            var previousRectColl = new List<Rect>();
            var previousRect = new Rect();
            var pieCoefficient = series is CircularSeriesBase ? series is PieSeries ? ((PieSeries)series).PieCoefficient : 0.8 : (series as CircularSeriesBase3D).CircleCoefficient;
            var center = new Point(finalSize.Width / 2, finalSize.Height / 2);
            double baseRight = pieRight, baseLeft = pieLeft;

            pieLeft = (finalSize.Width / 2 - radius) - radius * 0.5;
            pieRight = (finalSize.Width / 2 + radius) + radius * 0.5;

            var connectorHeight = radius * 0.2;

            pieRight = pieRight > baseRight ? baseRight : pieRight;
            pieLeft = pieLeft < baseLeft ? baseLeft : pieLeft;

            double explodRadius, angle;
            ConnectorMode connectorMode;
            int explodeIndex = -1;
            if (series is CircularSeriesBase)
            {
                var circularSeriesBase = series as CircularSeriesBase;
                explodeIndex = circularSeriesBase.ExplodeAll ? -2 : circularSeriesBase.ExplodeIndex;
                explodRadius = circularSeriesBase.ExplodeRadius;
                connectorMode = circularSeriesBase.ConnectorType;
            }
            else
            {
                var circularSeriesBase3D = series as CircularSeriesBase3D;
                explodeIndex = circularSeriesBase3D.ExplodeAll ? -2 : circularSeriesBase3D.ExplodeIndex;
                explodRadius = circularSeriesBase3D.ExplodeRadius;
                connectorMode = circularSeriesBase3D.ConnectorType;
            }
            for (var i = 0; i < orderedAdornments.Count(); i++)
            {
                var renderingPoints = new List<Point>();
                ChartAdornment adornment;
                int adornmentIndex;
                adornment = orderedAdornments[i];
                adornmentIndex = series.Adornments.IndexOf(adornment);
                double explodedRadius = adornmentIndex == explodeIndex ? explodRadius : explodeIndex == -2 ? explodRadius : 0d;
                angle = adornment.ConnectorRotationAngle;
                var label = LabelPresenters[adornmentIndex];
                var x = center.X + (Math.Cos(angle) * radius);
                var y = center.Y + (Math.Sin(angle) * radius);

                x = x + (Math.Cos(angle) * (explodedRadius - radius / 10));
                y = y + (Math.Sin(angle) * (explodedRadius - radius / 10));
                

                renderingPoints.Add((new Point(x, y)));
                x = x + (Math.Cos(angle) * connectorHeight);
                y = y + (Math.Sin(angle) * connectorHeight);

                renderingPoints.Add(new Point(x, y));

                var pieAngle = angle % (Math.PI * 2);
                var isLeft = pieAngle > 1.57 && pieAngle < 4.71;
                double connectorLineEdge;
                if (isLeft)
                {
                    x = pieLeft - (label.DesiredSize.Width / 2);
                    connectorLineEdge = +label.DesiredSize.Width / 2;
                }
                else
                {
                    x = pieRight + (label.DesiredSize.Width / 2);
                    connectorLineEdge = -label.DesiredSize.Width / 2;
                }
                var distanceFromOrigin = (Math.Sqrt(Math.Pow(adornment.X - x, 2) + Math.Pow(adornment.Y - y, 2))) / 10;
                x = isLeft ? x + distanceFromOrigin : x - distanceFromOrigin;
                var currRect = new Rect(x, y, label.DesiredSize.Width, label.DesiredSize.Height);

                if (previousRectColl.IntersectWith(currRect))
                {
                    renderingPoints.Add(isLeft ? new Point(x + connectorHeight + connectorLineEdge, y) : new Point(x - connectorHeight + connectorLineEdge, y));
                    y = previousRect.Bottom + 2;
                }
                renderingPoints.Add(new Point(x + connectorLineEdge, y));
                currRect.Y = y;
                previousRect = currRect;
                previousRectColl.Add(currRect);

                DrawConnecotrLine(adornmentIndex, renderingPoints, connectorMode);

                if (!ShowLabel) continue;
                if (this is ChartAdornmentInfo)
                    AlignElement(label, GetVerticalAlignment(VerticalAlignment), GetHorizontalAlignment(HorizontalAlignment), x, y);
                else
                    ((ChartAdornmentInfo3D) this).AddLabel(label, x, y);
            }
        }

        private IList<ChartAdornment> GetOrderedAdornments()
        {
            return series.Adornments.Where(item => item.ConnectorRotationAngle%(Math.PI*2) > 1.57 &&
                                                   item.ConnectorRotationAngle%
                                                   (Math.PI*2) < 4.71).ToList().OrderBy(item => item.Y).Union
                (series.Adornments.Where(item => item.ConnectorRotationAngle%
                                                 (Math.PI*2) <= 1.57 || item.ConnectorRotationAngle%
                                                 (Math.PI*2) >= 4.71).ToList().OrderBy(item => item.Y)).ToList();
        }

        public DependencyObject Clone()
        {
            return CloneAdornmentInfo();
        }

        #endregion
    }

    /// <summary>
    /// Represents the class used for configuring chart adornments for 2D chart.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about the data point.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public sealed class ChartAdornmentInfo : ChartAdornmentInfoBase
    {
        #region methods

        protected override void DrawLineSegment(List<Point> points, Path path)
        {
            if (points.Count < 1 || path == null) return;
            var pathFigure = new PathFigure();
            var pathGeometry = new PathGeometry();

            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            pathFigure.StartPoint = points[0];
            var segment = new PolyLineSegment { Points = new PointCollection() };
            foreach (var item in points)
            {
                segment.Points.Add(item);
            }
            pathFigure.Segments.Add(segment);
        }

        private void UpdateLabelPos(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment, int adornmentIndex, double pieLeft, double pieRight)
        {
            if (adornment == null) return;
            var label = LabelPresenters[adornmentIndex];
            if (ShowLabel)
                label.Visibility = Visibility.Visible;
            //Reset the visibility if the visibility is collapsed from collision.
            if (ConnectorLines.Count > adornmentIndex)
            {
                ConnectorLines[adornmentIndex].Visibility = Visibility.Visible;
            }
            
            var circularSeriesBase = series as CircularSeriesBase;
            double x = adornment.X, y = adornment.Y;

            if (ShowConnectorLine || (circularSeriesBase != null && circularSeriesBase.EnableSmartLabels))
            {
                var connectorLineMode = circularSeriesBase != null ? circularSeriesBase.ConnectorType : ConnectorMode.Line;
                var points = GetAdornmentPositions(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y);
                DrawConnecotrLine(adornmentIndex, points, connectorLineMode);
            }
            if (!ShowLabel) return;
            AlignElement(label, GetVerticalAlignment(VerticalAlignment), GetHorizontalAlignment(HorizontalAlignment), x, y);
        }

        internal override void Arrange(Size finalSize)
        {
            double pieLeft = 0d, pieRight = 0d, pieRadius = 0d;

            var circularSeriesBase = series as CircularSeriesBase;

            var isPieSeriesExtendedLabels = circularSeriesBase != null && circularSeriesBase.LabelPosition == CircularSeriesLabelPosition.OutsideExtended && circularSeriesBase.EnableSmartLabels;
            AdornmentInfoSize = finalSize;

            var index = 0;
            if (LabelPresenters.Count > 0 && circularSeriesBase != null)
            {
                pieRadius = circularSeriesBase.Radius;
                foreach (var pieAdornment in Series.Adornments.Select(adornment => adornment as ChartPieAdornment))
                {
                    if (pieAdornment.ConnectorRotationAngle % (Math.PI * 2) <= 1.57 || pieAdornment.ConnectorRotationAngle % (Math.PI * 2) >= 4.71)
                    {
                        pieLeft = Math.Max(pieLeft, LabelPresenters[index].DesiredSize.Width);
                    }
                    else
                    {
                        pieRight = Math.Max(pieRight, LabelPresenters[index].DesiredSize.Width);
                    }
                    index++;
                }
                pieRight = finalSize.Width - pieRight;
            }

            var adornmentIndex = 0;
            var labelBounds = new List<Rect>();

            foreach (var adornment in series.Adornments)
            {
                var transformer = Series.CreateTransformer(finalSize, false);
                adornment.Update(transformer);

                if (adormentContainers != null && adornmentIndex < adormentContainers.Count)
                {
                    var adornmentPresenter = adormentContainers[adornmentIndex];
                    var adornmentRect = new Rect(new Point(), (adornmentPresenter.DesiredSize))
                    {
                        X = adornment.X - adornmentPresenter.SymbolOffset.X,
                        Y = adornment.Y - adornmentPresenter.SymbolOffset.Y
                    };

                    Canvas.SetZIndex(adornmentPresenter, 3);
                    Canvas.SetLeft(adornmentPresenter, adornmentRect.Left);
                    Canvas.SetTop(adornmentPresenter, adornmentRect.Top);
                    
                }
                //Update the outside and inside labels
                if (!isPieSeriesExtendedLabels)
                    UpdateLabelPos(pieRadius, labelBounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight);
                adornmentIndex++;
            }
            //Update the outside extended labels
            if (isPieSeriesExtendedLabels)
            {
                UpdateSpiderLabels(pieLeft, pieRight, finalSize, pieRadius);
            }
            foreach (var line in ConnectorLines)
            {
                Canvas.SetLeft(line, 0);
                Canvas.SetTop(line, 0);
            }
        }

        internal override DependencyObject CloneAdornmentInfo()
        {
            var adornment = new ChartAdornmentInfo
            {
                ShowLabel = ShowLabel,
                ShowMarker = ShowMarker,
                Symbol = Symbol,
                SymbolHeight = SymbolHeight,
                SymbolInterior = SymbolInterior,
                SymbolTemplate = SymbolTemplate,
                SymbolWidth = SymbolWidth,
                ShowConnectorLine = ShowConnectorLine,
                SegmentLabelFormat = SegmentLabelFormat,
                SegmentLabelContent = SegmentLabelContent,
                LabelTemplate = LabelTemplate,
                HorizontalAlignment = HorizontalAlignment,
                ConnectorLineStyle = ConnectorLineStyle
            };
            return adornment;
        }

        #endregion
    }

    /// <summary>
    /// Represents the class used for configuring chart adornments for 3D chart.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about the data point.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public sealed class ChartAdornmentInfo3D : ChartAdornmentInfoBase
    {
        private Graphics3D graphics3D;

        private double startDepth;

        #region methods

        private void UpdateLabelPos(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment,
           int labelIndex, double pieLeft, double pieRight)
        {
            if (adornment == null) return;
            var label = LabelPresenters[labelIndex];
            //Reset the visibility if the visibility is collapsed from collision.
            if (ConnectorLines.Count > labelIndex)
            {
                ConnectorLines[labelIndex].Visibility = Visibility.Visible;
            }
            label.Visibility = Visibility.Visible;

            var circularSeriesBase = series as CircularSeriesBase3D;
            double x = adornment.X, y = adornment.Y;

            if (ShowConnectorLine || (circularSeriesBase != null && circularSeriesBase.EnableSmartLabels))
            {
                var connectorMode = series is CircularSeriesBase3D ? ((CircularSeriesBase3D)series).ConnectorType : ConnectorMode.Line;
                var points = GetAdornmentPositions(pieRadius, bounds, finalSize, adornment, labelIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y);
                DrawConnecotrLine(labelIndex, points, connectorMode);
            }
            if (!ShowLabel) return;
            AddLabel(label, x, y);
        }

        internal void AddLabel(UIElement element, double x, double y)
        {
            graphics3D.AddVisual(Polygon3D.CreateUIElement(new Vector3D(x, y, startDepth - 1), element, 0, -element.DesiredSize.Height));
        }

        protected override void DrawLineSegment(List<Point> points, Path path)
        {
            graphics3D.AddVisual(Polygon3D.CreatePolyline(points.Get3DVector(startDepth), path));
            base.DrawLineSegment(points, path);
        }

        internal override void Arrange(Size finalSize)
        {
            graphics3D = ((SfChart3D) series.ActualArea).Graphics3D;
            if (series is CircularSeriesBase3D)
                (series as CircularSeriesBase3D).Area.IsAutoDepth = true;
            double pieLeft = 0d, pieRight = 0d, pieRadius = 0d;
            var circularSeriesBase3D = series as CircularSeriesBase3D;
            var isPieSeriesExtendedLabels = circularSeriesBase3D != null && circularSeriesBase3D.LabelPosition == CircularSeriesLabelPosition.OutsideExtended && circularSeriesBase3D.EnableSmartLabels;
            AdornmentInfoSize = finalSize;

            var index = 0;
            if (series != null && series.Adornments.Count > 0)
            {
                startDepth = (series.Adornments[0] as ChartAdornment3D).StartDepth;
            }
            if (LabelPresenters.Count > 0 && circularSeriesBase3D != null)
            {
                foreach (var pieAdornment in Series.Adornments.Select(adornment => adornment as ChartPieAdornment3D))
                {
                    if (pieAdornment.ConnectorRotationAngle % (Math.PI * 2) <= 1.57 || pieAdornment.ConnectorRotationAngle % (Math.PI * 2) >= 4.71)
                    {
                        pieLeft = Math.Max(pieLeft, LabelPresenters[index].DesiredSize.Width);
                    }
                    else
                    {
                        pieRight = Math.Max(pieRight, LabelPresenters[index].DesiredSize.Width);
                    }
                    index++;
                }
                if (series.Adornments.Count > 0)
                {
                    var pieAdornment3D = series.Adornments[0] as ChartPieAdornment3D;
                    
                    pieRadius = pieAdornment3D.Radius;
                }
                pieRight = finalSize.Width - pieRight;
            }

            var adornmentIndex = 0;
            var labelBounds = new List<Rect>();

            foreach (var adornment in series.Adornments)
            {
                var transformer = series.CreateTransformer(finalSize, false);
                adornment.Update(transformer);

                if (adormentContainers != null && adornmentIndex < adormentContainers.Count)
                {
                    var adornmentPresenter = adormentContainers[adornmentIndex];
                    var x = adornment.X - adornmentPresenter.SymbolOffset.X;
                    var y = adornment.Y - adornmentPresenter.SymbolOffset.Y;
                    adornmentPresenter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var size = adornmentPresenter.DesiredSize;
                    x += size.Width == 0 ? 0 : size.Width / 2;
                    y += size.Height == 0 ? 0 : size.Height / 2;
                    var depth = ((ChartAdornment3D)adornment).StartDepth;
                    ((ChartSeries3D) series).Area.Graphics3D.AddVisual(Polygon3D.CreateUIElement(new Vector3D(x,y,depth), adornmentPresenter, 0, -adornmentPresenter.DesiredSize.Height ));
                }
                //Update the outside and inside labels
                if (!isPieSeriesExtendedLabels)
                    UpdateLabelPos(pieRadius, labelBounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight);
                adornmentIndex++;
            }
            //Update the outside extended labels
            if (isPieSeriesExtendedLabels)
            {
                UpdateSpiderLabels(pieLeft, pieRight, finalSize, pieRadius);
            }

            foreach (var line in ConnectorLines)
            {
                Canvas.SetLeft(line, 0);
                Canvas.SetTop(line, 0);
            }
        }

        internal override DependencyObject CloneAdornmentInfo()
        {
            var adornment = new ChartAdornmentInfo3D();
            adornment.ShowLabel = this.ShowLabel;
            adornment.ShowMarker = this.ShowMarker;
            adornment.Symbol = this.Symbol;
            adornment.SymbolHeight = this.SymbolHeight;
            adornment.SymbolInterior = this.SymbolInterior;
            adornment.SymbolStroke = this.SymbolStroke;
            adornment.SymbolTemplate = this.SymbolTemplate;
            adornment.SymbolWidth = this.SymbolWidth;
            adornment.ShowConnectorLine = this.ShowConnectorLine;
            adornment.SegmentLabelFormat = this.SegmentLabelFormat;
            adornment.SegmentLabelContent = this.SegmentLabelContent;
            adornment.LabelTemplate = this.LabelTemplate;
            adornment.HorizontalAlignment = this.HorizontalAlignment;
            adornment.ConnectorLineStyle = this.ConnectorLineStyle;
            return adornment;
        }

        #endregion
    }
}
