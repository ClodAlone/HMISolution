#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using Syncfusion.Documentation;
using System.Globalization;
using Syncfusion.Windows.Forms.Chart.Scrolling;
using System.Collections.Generic;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Indicates allowed action with axis.
    /// </summary>
    public enum ChartZoomingAction
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Panning action is allowed.
        /// </summary>
        Panning = 0x01
    }

    /// <summary>
    /// The ChartAxis class represents an axis on the plot. An axis can be vertical or horizontal in orientation. There can be several
    /// axes in a chart. One X axis and one Y axis are treated as the primary X and primary Y axes. These are the ones that are visible by default.
    /// You can create and add additional axes to the <see cref="Syncfusion.Windows.Forms.Chart.ChartArea"/> using its <see cref="Syncfusion.Windows.Forms.Chart.ChartArea.Axes"/> collection. Any series
    /// on the chart can be plotted on any axis that is registered with the chart.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartAxis : IDisposable
    {
        #region Constants
        private const int c_rowsCount = 4;
        private const int c_maxRoundingPlaces = 15;
        private readonly static double[] c_intervalDivs = new double[] { 1d, 2d, 5d, 10 };
        private readonly static DateTime c_dateTimeZero = DateTime.FromOADate(0);
        #endregion

        #region Class members
        private bool m_needUpdateVisibleLables = true;
        private ChartAxisLabel[] m_visibleLables = null;

        private bool m_isFreezed = false;
        private bool m_scaleLables = false;
        private float m_scaleLablesCoef = 1f;
        private float m_scaleLength = 0f;
        private ChartZoomingAction m_zoomingAction = ChartZoomingAction.None;

        private ChartSetMode adjustPlotAreaMargins;
        private ChartMargins chartPlotAreaMargins;

        private double m_labelsOffset = 0d;
        private double m_labelOffset = 0d;
        

        private bool m_interlacedGrid = false;
        private BrushInfo m_interlacedGridInterior = new BrushInfo(Color.LightGray);

        private StringAlignment m_labelAligment = StringAlignment.Center;
        private bool m_margin = true;
        private bool m_autoValueType = false;
        private bool m_customOrigin = false;
        private string m_dateTimeFormat = "g";
        private int m_desiredIntervals = 6;
        private float m_dimension = 0;
        private bool m_needUpdateDimension = true;
        private float m_ticksAndLabelsDimension = 0;
        private float[] groupingLabelsRowsDimensions = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private bool m_drawGrid = true;
        private bool m_drawMinorGrid = false;
        private bool m_showAxisLabelTooltip = true;
        private ChartAxisGridDrawingMode m_gridDrawMode = ChartAxisGridDrawingMode.Default;
        private ChartAxisTickDrawingOperationMode m_tickDrawingOperationMode = ChartAxisTickDrawingOperationMode.NumberOfIntervalsFixed;
        private Font m_font = new Font("Verdana", 8f);
        private Font m_titleFont = new Font("Verdana", 8f);
        private Color m_foreColor = Color.Empty;
        private Color m_titleColor = Color.Empty;
        private string m_format = string.Empty;
        private LineInfo m_gridLineType;
        private ChartLabelIntersectAction m_intersectAction = ChartLabelIntersectAction.None;
        private ChartLabelIntersectionActionEffect m_intersectEffect = ChartLabelIntersectionActionEffect.All;
        private ChartDateTimeIntervalType m_intervalType = ChartDateTimeIntervalType.Auto;
        private bool m_hidePartialLabels = false;
        private bool m_inversed = false;
        private bool m_labelRotate = false;
        private bool m_rotateFromTicks = false;
        private int m_labelRotateAngle = 0;
        private IChartAxisLabelModel m_labels;
        private IChartAxisGroupingLabelModel m_groupingLabels;
        private LineInfo m_lineType;
        private LineInfo m_minorGridLineType = new LineInfo();
        private int m_logBase = 10;
        private bool m_needUpdate = true;
        private double m_offset = 0;
        private bool m_opposedPosition = false;
        private ChartOrientation m_orientation = ChartOrientation.Horizontal;
        private double m_origin = 0;
        private MinMaxInfo m_range = new MinMaxInfo(0, 10, 1);
        private MinMaxInfo m_visibleRange = new MinMaxInfo(0, 10, 1);
        private MinMaxInfo m_zoomedRange = new MinMaxInfo(0, 10, 1);
        private ChartAxisRangeType m_rangeType = ChartAxisRangeType.Auto;
        private bool requireInvertedAxes = false;
        private int m_roundingPlaces = 2;
        private Size m_smallTickSize = new Size(1, 1);
        private int m_smallTicksPerInterval;
        private ChartStripLineCollection m_stripLines;
        private Color m_tickColor = SystemColors.ControlText;
        private Size m_tickSize = new Size(1, 1);
        private ChartTitleDrawMode m_titleDrawMode = ChartTitleDrawMode.None;

        private string m_toolTip = "";

        private ChartValueType m_valueType = ChartValueType.Double;
        private ChartAxisTickLabelDrawingMode tickLabelDrawingMode = ChartAxisTickLabelDrawingMode.AutomaticMode;
        private double m_zoomFactor = 1;
        private double m_zoomPosition = 0;
        private ChartAxisRangePaddingType m_rangePaddingType = ChartAxisRangePaddingType.Calculate;
        private bool m_forceZeroToDouble = false;
        private bool m_forceZero = false;
        private bool m_preferZero = true;
        private bool m_autoSize = true;
        private float m_length = 0f;
        private PointF m_location = PointF.Empty;
        private double m_crossing =double.NaN;
        private ChartAxisLocationType m_locationType = ChartAxisLocationType.Auto;
        private ChartDateTimeInterval m_dateTimeInterval = null;
        private bool smartDateZoom = false;
        private string smartDateZoomLabelsCulture = "en-US";
        private string smartDateZoomYearLevelLabelFormat = "y";
        private string smartDateZoomMonthLevelLabelFormat = "MMMM d, yyyy";
        private string smartDateZoomWeekLevelLabelFormat = "MMM, ddd d, yyyy";
        private string smartDateZoomDayLevelLabelFormat = "g";
        private string smartDateZoomHourLevelLabelFormat = "t";
        private string smartDateZoomMinuteLevelLabelFormat = "T";
        private string smartDateZoomSecondLevelLabelFormat = "T";
        private string currentSmartDateTimeFormat = "g";
        private ChartAxisEdgeLabelsDrawingMode edgeLabelsDrawingMode;

        private string m_title = "";
        private StringAlignment m_titleAlignment = StringAlignment.Center;
        private float m_titleSpacing = 4f;

        private bool m_drawTickLabelGrid = false;
        private float m_tickLabelGridPadding = 5;
        private ChartOrientation m_defaultAxisOrienation = ChartOrientation.Horizontal;
        private IChartScrollBar m_scrollBar = null;

        private ChartAxisBreakInfo m_breakInfo = new ChartAxisBreakInfo();
        private ChartAxisRange m_breakRanges = null;
        private bool m_makeBreaks = true;

        private ChartArea m_area = null;
        private bool m_isVisible = true;

        private double m_pointOffset = 0;
        private ChartCustomLabelsParameter m_customLabelsParameter = ChartCustomLabelsParameter.Index;
        private ChartAxisLayout m_layout = null;
        private Pen[] m_pens = null;
		private StringFormat m_labelStringFormat =new StringFormat(StringFormat.GenericDefault);
		
		ChartPlacement m_axisLabelPlacement = ChartPlacement.Outside;
        #endregion

        #region Properties

        #region Layout properties
        /// <summary>
        ///  Gets or sets a value indicating whether the size of the chart axis should be calculated automatically.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public bool AutoSize
        {
            get
            {
                return m_autoSize;
            }
            set
            {
                m_autoSize = value;
            }
        }

        /// <summary>
        ///  Gets or sets the size of this axis.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public SizeF Size
        {
            get
            {
                SizeF res = SizeF.Empty;

                if (m_orientation == ChartOrientation.Horizontal)
                {
                    res = new SizeF(RealLength, 0);
                }
                else
                {
                    res = new SizeF(0, RealLength);
                }

                return res;
            }

            set
            {
                if (!m_autoSize)
                {
                    if (m_orientation == ChartOrientation.Horizontal && RealLength != value.Width)
                    {
                        RealLength = value.Width;
                    }
                    else if (m_orientation == ChartOrientation.Vertical && RealLength != value.Height)
                    {
                        RealLength = value.Height;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the location and size of the rectangular region occupied by the axis.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public RectangleF Rect
        {
            get
            {
                RectangleF res = RectangleF.Empty;

                if (m_orientation == ChartOrientation.Horizontal)
                {
                    float oppCoef;
                    if (AxisLabelPlacement == ChartPlacement.Inside)
                        oppCoef = OpposedPosition ? 0 : 1;
                    else
                        oppCoef = OpposedPosition ? 1 : 0;
                    res = new RectangleF(Location.X, Location.Y - oppCoef * this.Dimension, RealLength, this.Dimension);
                }
                else
                {
                    float oppCoef;
                    if (AxisLabelPlacement == ChartPlacement.Inside)
                        oppCoef = OpposedPosition ? 1 : 0;
                    else
                        oppCoef = OpposedPosition ? 0 : 1;
                    res = new RectangleF(Location.X - oppCoef * this.Dimension, Location.Y - RealLength, this.Dimension, RealLength);
                }

                return res;
            }

            set
            {
                if (!m_autoSize /*&& locationType == ChartAxisLocationType.Set*/ )
                {
                    if (m_orientation == ChartOrientation.Horizontal)
                    {
                        Location = new PointF(value.Left, value.Top);
                        RealLength = value.Width;
                    }
                    else
                    {
                        Location = new PointF(value.Left, value.Bottom);
                        RealLength = value.Width;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the length of this axis.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public float RealLength
        {
            get
            {
                return m_length;
            }

            set
            {
                if (m_length != value)
                {
                    m_length = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the cardinal dimension of the axis object. If the text that is rendered by the axis is of a dimension that is more, then
        /// that dimension will be used and this dimension is ignored.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]       
        public float Dimension
        {
            get
            {
                return m_dimension;
            }

            set
            {
                if (m_dimension != value)
                {
                    m_dimension = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// If LocationType is set to ChartAxisLocationType.Set,
        /// then this property is used to calculate RenderGlobalBounds
        /// and in such a way to define location of axes.
        /// </summary>
        [DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public PointF Location
        {
            get
            {
                return m_location;
            }

            set
            {
                if (/*(LocationType == ChartAxisLocationType.Set)&&*/(m_location != value))
                {
                    m_location = value;
                    if(double.IsNaN(this.Crossing))
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Specifies the double value.This property is used to locate the X and Y axes in
        /// chart area based on the specified value. specified value must be within the
        /// range of X and Y axes values.
        /// <para>      </para>
        /// </summary>
        [Description("Specifies how the axes are crossed based on the given double value.")]
        public double Crossing
        {
            get
            {
                return m_crossing;
            }
            set
            {
                m_crossing = value;
            }
        }

        /// <summary>
        /// Determines how location of axes is calculated. See <see cref="Location"/>.
        /// </summary>
        [DefaultValue(ChartAxisLocationType.Auto), NotifyParentProperty(true)]
        [Description("Determines how location of axes is calculated.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartAxisLocationType LocationType
        {
            get
            {
                return m_locationType;               
            }

            set
            {
                if (m_locationType != value)
                {
                    m_locationType = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets label alignment.
        /// </summary>
        [DefaultValue(StringAlignment.Center), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the alignment of labels.")]       
        public StringAlignment LabelAlignment
        {
            get
            {
                return m_labelAligment;
            }

            set
            {
                if (m_labelAligment != value)
                {
                    m_labelAligment = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether label is rotate from Ticks while using Far.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether labels can be rotated from Ticks.")]
        public bool RotateFromTicks
        {
            get
            {
                return m_rotateFromTicks;
            }

            set
            {

                if (m_rotateFromTicks != value)
                {
                    m_rotateFromTicks = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartAxis"/> Showa some space as margin.
        /// </summary>
        /// <value><c>true</c> if margin; otherwise, <c>false</c>.</value>
        public bool Margin
        {
            get
            {
                return m_margin;
            }
            set
            {
                if (m_margin != value)
                {
                    m_margin = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether label grid is visible.
        /// </summary>
        /// <value><c>true</c> if label grid is visible; otherwise, <c>false</c>.</value>
        [Browsable(true), NotifyParentProperty(true), DefaultValue(false)]
        [Description("Indicates whether label grid is visible.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool DrawTickLabelGrid
        {
            get
            {
                return m_drawTickLabelGrid;
            }

            set
            {
                if (m_drawTickLabelGrid != value)
                {
                    m_drawTickLabelGrid = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the tick label grid padding.
        /// </summary>
        /// <value>The tick label grid padding.</value>
        [DefaultValue(5f), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the padding of labels grid.")]        
        public float TickLabelGridPadding
        {
            get
            {
                return m_tickLabelGridPadding;
            }

            set
            {
                if (m_tickLabelGridPadding != value)
                {
                    m_tickLabelGridPadding = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        #region Breaks properties
        /// <summary>
        /// Gets the break ranges.
        /// </summary>
        /// <value>The break ranges.</value>
        [Browsable(false)]
        [Description("Contains information about breaks ranges.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public ChartAxisRange BreakRanges
        {
            get
            {
                return m_breakRanges;
            }
        }

        /// <summary>
        ///  Indicates whether the breaks should be shown for the specified axis.
        /// </summary>
        [DefaultValue(true)]
        [Description("Indicates whether the breaks should be shown for the specified axis.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool MakeBreaks
        {
            get
            {
                return m_makeBreaks;
            }

            set
            {
                if (m_makeBreaks != value)
                {
                    m_makeBreaks = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the break info.
        /// </summary>
        /// <value>The break info.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Content)]
        [Description("Contains information about breaks representation.")]
        public ChartAxisBreakInfo BreakInfo
        {
            get
            {
                return m_breakInfo;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (m_breakInfo != value)
                {
                    m_breakInfo.Changed -= new EventHandler(this.OnNeedRedraw);
                    m_breakInfo = value;
                    m_breakInfo.Changed += new EventHandler(this.OnNeedRedraw);

                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Obsolete properties
        /// <summary>
        ///  Gets the breaks for an axis.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property is useless. Use BreakInfo property.")]
        public MinMaxInfo[] Breaks
        {
            get
            {
                return null;// (MinMaxInfo[])m_breaks.ToArray(typeof(MinMaxInfo));
            }
        }

        /// <summary>
        /// Gets or sets the Label intersection control option.
        /// <seealso cref="ChartLabelIntersectionActionEffect"/>
        /// </summary>
        [DefaultValue(ChartLabelIntersectionActionEffect.All), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple), Obsolete("This property isn't used anymore.")]
        public ChartLabelIntersectionActionEffect LabelIntersectionActionEffect
        {
            get
            {
                return m_intersectEffect;
            }

            set
            {

                if (m_intersectEffect != value)
                {
                    m_intersectEffect = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether require inverted axes.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Obsolete("This property isn't used anymore.")]
        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool RequireInvertedAxes
        {
            get
            {
                return requireInvertedAxes;
            }

            set
            {
                requireInvertedAxes = value;
            }
        }

        /// <summary>
        /// This property is for internal use. Do not try to set it manually.
        /// It indicates whether this <see cref="ChartAxis"/> is indexed. Indexed axes
        /// have only positional value. They do not actually plot value data; only the position of the data
        /// is used for plotting. You may read the value, but do not set it manually.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if axis is indexed; <c>false</c> otherwise.
        /// </value>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property isn't used anymore"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Indexed
        {
            get
            {
                return this.IsIndexed;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether labels can be scaled.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Behavior of this property is incorrect. This property only for internal usage.")]
        public bool ScaleLabels
        {
            get
            {
                return m_scaleLables;
            }
            set
            {
                if (m_scaleLables != value)
                {
                    m_scaleLables = value;
                    m_scaleLength = RealLength;
                    m_scaleLablesCoef = 1f;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value type of this axis will be automatically assigned. Default is false.
        /// </summary>
        [Obsolete("This property isn't used.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(false), Description("Indicates whether the value type of this axis will be automatically assigned.")]        
        public bool AutoValueType
        {
            get
            {
                return m_autoValueType;
            }

            set
            {
                m_autoValueType = value;
            }
        }
        #endregion

        #region Smart date zooming
        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor and labels format 
        /// according to the improved date time zoom logic or to use default zooming behaviour.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates whether to set zoom factor and labels format according to the improved date time zoom logic or to use default zooming behaviour.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool SmartDateZoom
        {
            get
            {
                return smartDateZoom;
            }

            set
            {
                if (smartDateZoom != value)
                {
                    smartDateZoom = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour.
        /// </summary>
        [DefaultValue("en-US"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomLabelsCulture
        {
            get
            {
                return smartDateZoomLabelsCulture;
            }

            set
            {
                if (smartDateZoomLabelsCulture != value)
                {
                    smartDateZoomLabelsCulture = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "y".
        /// </summary>
        [DefaultValue("y"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]        
        public string SmartDateZoomYearLevelLabelFormat
        {
            get
            {
                return smartDateZoomYearLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomYearLevelLabelFormat != value)
                {
                    smartDateZoomYearLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "MMMM d, yyyy"
        /// </summary>
        [DefaultValue("MMMM d, yyyy"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]        
        public string SmartDateZoomMonthLevelLabelFormat
        {
            get
            {
                return smartDateZoomMonthLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomMonthLevelLabelFormat != value)
                {
                    smartDateZoomMonthLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "g"
        /// </summary>
        [DefaultValue("g"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomDayLevelLabelFormat
        {
            get
            {
                return smartDateZoomDayLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomDayLevelLabelFormat != value)
                {
                    smartDateZoomDayLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "MMM, ddd d, yyyy".
        /// </summary>
        [DefaultValue("MMM, ddd d, yyyy"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomWeekLevelLabelFormat
        {
            get
            {
                return smartDateZoomWeekLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomWeekLevelLabelFormat != value)
                {
                    smartDateZoomWeekLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor 
        /// and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "t".
        /// </summary>
        [DefaultValue("t"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomHourLevelLabelFormat
        {
            get
            {
                return smartDateZoomHourLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomHourLevelLabelFormat != value)
                {
                    smartDateZoomHourLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor and labels format according improved date time zoom logic or
        /// to use default zooming behaviour. Default is "T".
        /// </summary>
        [DefaultValue("T"), ChartTemplate(ChartTemplateSet.Simple), Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomMinuteLevelLabelFormat
        {
            get
            {
                return smartDateZoomMinuteLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomMinuteLevelLabelFormat != value)
                {
                    smartDateZoomMinuteLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour. Default is "T".
        /// </summary>
        [DefaultValue("T"), ChartTemplate(ChartTemplateSet.Simple)
        , Description("Indicates whether to set zoom factor and labels format according improved date time zoom logic or to use default zooming behaviour.")]
        public string SmartDateZoomSecondLevelLabelFormat
        {
            get
            {
                return smartDateZoomSecondLevelLabelFormat;
            }

            set
            {
                if (smartDateZoomSecondLevelLabelFormat != value)
                {
                    smartDateZoomSecondLevelLabelFormat = value;
                }
            }
        }

        /// <summary>
        /// Gets the current smart date time format.
        /// </summary>
        /// <value>The current smart date time format.</value>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public string CurrentSmartDateTimeFormat
        {
            get
            {
                this.SetSmartZoomFactor(this.ZoomFactor);
                return currentSmartDateTimeFormat;
            }
        }
        #endregion

        #region InterlacedGrid properties
        /// <summary>
        /// Gets or sets a value indicating whether interlaced grid is enabled.
        /// </summary>
        /// <value><c>true</c> if interlaced grid is enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        [NotifyParentProperty(true)]
        [Description("Indicates whether interlaced grid is enabled")]
        public bool InterlacedGrid
        {
            get
            {
                return m_interlacedGrid;
            }

            set
            {
                if (m_interlacedGrid != value)
                {
                    m_interlacedGrid = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the interlaced grid interior.
        /// </summary>
        /// <value>The interlaced grid interior.</value>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Interlaced grid interior")]
        public BrushInfo InterlacedGridInterior
        {
            get 
            {
                return m_interlacedGridInterior; 
            }

            set
            {
                if (m_interlacedGridInterior != value)
                {
                    m_interlacedGridInterior = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the edge labels drawing mode.
        /// </summary>
        [DefaultValue(ChartAxisEdgeLabelsDrawingMode.Center)]
        [NotifyParentProperty(true)]
        [Description("Indicates the edge labels drawing mode")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartAxisEdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get
            {
                return edgeLabelsDrawingMode;
            }

            set
            {
                if (edgeLabelsDrawingMode != value)
                {
                    edgeLabelsDrawingMode = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        #region Origin properties
        /// <summary>
        /// By default the axis will calculate the origin of the axis from data contained in the series. Using the <see cref="OriginDate"/>
        /// and <see cref="Origin"/> properties, you can change this origin. To do so, first set this property to true. Default is false.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the origin position can be changed.")]
        public bool CustomOrigin
        {
            get
            {
                return m_customOrigin;
            }

            set
            {

                if (m_customOrigin != value)
                {
                    m_customOrigin = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets custom origin for charts containing datas of double value type. By default, the axis will calculate the origin of the axis from data contained in the series. Using the <see cref="OriginDate"/>
        /// and <see cref="Origin"/> properties, you can change this origin. To enable the origin set with Origin or OriginDate, you have to
        /// set <see cref="CustomOrigin"/> to True.
        /// </summary>
        [DefaultValue(0.0), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the origin position.")]
        public double Origin
        {
            get
            {
                return m_origin;
            }

            set
            {

                if (m_origin != value)
                {
                    m_origin = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets custom origin for charts containing datas of DateTime value type. By default the axis will calculate the origin of the axis from data contained in the series. Using the <see cref="OriginDate"/>
        /// and <see cref="Origin"/> properties, you can change this origin. To enable the origin set with Origin or OriginDate, you have to
        /// set <see cref="CustomOrigin"/> to True.
        /// </summary>
        [DefaultValue(typeof(DateTime), "1899/12/30"), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the origin position for DateTime value type.")]
        public DateTime OriginDate
        {
            get
            {
                return DateTime.FromOADate(m_origin);
            }

            set
            {
                this.Origin = value.ToOADate();
            }
        }

        /// <summary>
        /// Gets the current origin.
        /// </summary>
        /// <value>The current origin.</value>        
        internal double CurrentOrigin
        {
            get
            {
                return m_customOrigin ? m_origin : (m_valueType == ChartValueType.Logarithmic ? 1 : 0);
            }
        }
        #endregion

        /// <summary>
        /// This format will be used to format axis labels of type DateTime for display. Default is "g".
        /// </summary>
        [DefaultValue("g"), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("This format will be used to format axis labels of type DateTime for display")]
        public string DateTimeFormat
        {
            get
            {
                return m_dateTimeFormat;
            }

            set
            {

                if (m_dateTimeFormat != value)
                {
                    m_dateTimeFormat = value;
                    m_needUpdateVisibleLables = true;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets an offset for axis intervals that contain DateTime datas. Depending on the data in the series provided to the Chart, the Chart will calculate and display a range of data on
        /// the ChartAxis. This will result in major grid lines being rendered along calculated intervals. However, sometimes
        /// you may wish to offset the calculated grid lines (major) by a certain factor. This is especially useful for DateTime values.
        /// For example, the default calculation always starts the intervals at Sunday (if the IntervalType is set to weeks). If you wish
        /// to start the intervals with Monday, you can simply specify a DateTimeOffset of one day. If your axis is not of type DateTime
        /// and you wish to take advantage of this property, please refer <see cref="Offset"/>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan DateTimeOffset
        {
            get
            {
                return DateTime.FromOADate(-m_offset) - DateTime.FromOADate(0);
            }

            set
            {
                Offset = DateTime.Now.Add(value).ToOADate() - DateTime.Now.ToOADate();
            }
        }

        /// <summary>
        ///     Specify the start and end dates and interval time for the axis. Use this if the data points are of datetime type.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]       
        public ChartDateTimeRange DateTimeRange
        {
            get
            {
                return new ChartDateTimeRange(DateTime.FromOADate(m_range.Min), DateTime.FromOADate(m_range.Max), m_range.Interval, this.IntervalType);
            }

            set
            {
                m_dateTimeInterval = value.DefaultInterval;
                this.m_intervalType = m_dateTimeInterval.Type;

                double start = value.Start.ToOADate();
                double end = value.End.ToOADate();

                if (value.DefaultInterval.Type == ChartDateTimeIntervalType.Auto)
                {
                    this.Range = new MinMaxInfo(start, end, (end - start) / this.Range.NumberOfIntervals);
                }
                else
                {
                    float numIntervals = (float)ChartDateTimeInterval.GetIntervalCount(value.DefaultInterval.Iterator());
                    this.Range = new MinMaxInfo(start, end, (end - start) / numIntervals);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the date time range of this axis as DateTime values.
        ///     Note: it works only if ValueType is DateTime.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartDateTimeInterval DateTimeInterval
        {
            get
            {
                return m_dateTimeInterval;
            }
        }

        /// <summary>
        /// Gets or sets the desired number of intervals for the range. Essential Chart includes a sophisticated automatic nice range calculation engine. The goal of this engine is to take
        /// raw data and convert it into human readable numbers. For example, if your raw numbers are 1.2 - 3.87, nice numbers could
        /// be 0-5 with 10 intervals of 0.5 each. The ChartAxis can do the same calculation for dates also. It offers precise control over
        /// how data types are to be interpreted when performing this calculation. With the DesiredIntervals setting, you can request the
        /// engine to calculate nice numbers such that they result in the number of intervals desired. Due to the nature of the calculation,
        /// the ChartAxis cannot provide precisely the same number of intervals but it will try to match the value to the extent possible.
        /// </summary>
        [DefaultValue(6), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Gets or sets the desired number of intervals for the range. Essential Chart includes a sophisticated automatic nice range calculation engine. The goal of this engine is to take raw data and convert it into human readable numbers. For example, if your raw numbers are 1.2 - 3.87, nice numbers could be 0-5 with 10 intervals of 0.5 each. The ChartAxis can do the same calculation for dates also. It offers precise control over how data types are to be interpreted when performing this calculation. With the DesiredIntervals setting, you can request the engine to calculate nice numbers such that they result in the number of intervals desired. Due to the nature of the calculation, the ChartAxis cannot provide precisely the same number of intervals but it will try to match the value to the extent possible.")]
        public int DesiredIntervals
        {
            get
            {
                return m_desiredIntervals;
            }

            set
            {
                if (m_desiredIntervals != value)
                {
                    m_desiredIntervals = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the padding that will be applied when calculating the axis range.
        /// </summary>
        [Description(@"Specifies the padding that will be applied when calculating the axis range."), DefaultValue(ChartAxisRangePaddingType.Calculate), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        public ChartAxisRangePaddingType RangePaddingType
        {
            
            get
            {
                return m_rangePaddingType;                
            }
            set
            {
                if (m_rangePaddingType != value)
                {
                    m_rangePaddingType = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates whether one boundary of the calculated range should always be tweaked to zero.
        /// </summary>
        [Description(@"Indicates if one boundary of the calculated range should always be tweaked to zero."), DefaultValue(false), NotifyParentProperty(true)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool ForceZero
        {
            get
            {
                return m_forceZero;
            }

            set
            {
                m_forceZero = value;
                this.OnIntervalsChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Indicates whether one boundary of the calculated range should always be tweaked to zero to both positive and negative value.
        /// </summary>
        [Description(@"Indicates if one boundary of the calculated range should always be tweaked to zero to both positive and negative value."), DefaultValue(false), NotifyParentProperty(true)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool ForceZeroToDouble
        {
            get
            {
                return m_forceZeroToDouble;
            }

            set
            {
                m_forceZeroToDouble = value;
                this.OnIntervalsChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Indicates that you would like one boundary of the calculated range to be tweaked to zero.
        /// </summary>
        [Description(@"Indicates that you would like one boundary of the calculated range to be tweaked to zero."), DefaultValue(true), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        public bool PreferZero
        {
            get
            {
                return m_preferZero;
            }

            set
            {
                m_preferZero = value;
                this.OnIntervalsChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Gets the tick and labels dimension.
        /// </summary>
        /// <value>The tick and labels dimension.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal float TickAndLabelsDimension
        {
            get
            {
                return m_ticksAndLabelsDimension;
            }
        }

        /// <summary>
        /// Gets the grouping labels rows dimensions.
        /// </summary>
        internal float[] GroupingLabelsRowsDimensions
        {
            get
            {
                return this.groupingLabelsRowsDimensions;
            }
        }

        #region Grid properties
        /// <summary>
        /// Gets or sets a value indicating whether the grid lines associated with the main interval points on the axis are to be rendered. This is
        /// set to True by default.
        /// </summary>
        [DefaultValue(true), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the grid lines associated with the main interval points on the axis are to be rendered")]
        public bool DrawGrid
        {
            get
            {
                return m_drawGrid;
            }

            set
            {

                if (m_drawGrid != value)
                {
                    m_drawGrid = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the tool tip associated with the axis labels are to be rendered. This is
        /// set to True by default.
        /// </summary>
        [DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the tool tip associated with the axis labels are to be rendered")]
        public bool ShowAxisLabelTooltip
        {
            get
            {
                return m_showAxisLabelTooltip;
            }

            set
            {
                m_showAxisLabelTooltip = value;
                                
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the minor grid lines associated with the main interval points on the axis are to be rendered. This is
        /// set to True by default.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the minor grid lines associated with the main interval points on the axis are to be rendered")]
        public bool DrawMinorGrid
        {
            get 
            { 
                return m_drawMinorGrid; 
            }

            set
            {
                if (m_drawMinorGrid != value)
                {
                    m_drawMinorGrid = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid drawing mode.
        /// </summary>
        /// <value>The grid draw mode.</value>
        [DefaultValue(ChartAxisGridDrawingMode.Default), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the grid drawing mode")]
        public ChartAxisGridDrawingMode GridDrawMode
        {
            get
            {
                return m_gridDrawMode;
            }

            set
            {

                if (m_gridDrawMode != value)
                {
                    m_gridDrawMode = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the attributes of the axis grid lines. Please refer to <see cref="LineInfo"/> for more information on these attributes and
        /// how they can change the appearance of the grid lines.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        [Description("Contains the attributes of the axis grid lines.")]
        [ChartTemplate(ChartTemplateSet.Content)]
        public LineInfo GridLineType
        {
            get
            {
                return m_gridLineType;
            }
        }

        /// <summary>
        /// Gets the attributes of the axis grid lines. Please refer to <see cref="LineInfo"/> for more information on these attributes and
        /// how they can change the appearance of the grid lines.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        [Description("Contains the attributes of the axis grid lines.")]
        [ChartTemplate(ChartTemplateSet.Content)]
        public LineInfo MinorGridLineType
        {
            get
            {
                return m_minorGridLineType;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the tick drawing operation mode.
        /// </summary>
        /// <value>The tick drawing operation mode.</value>
        [DefaultValue(ChartAxisTickDrawingOperationMode.NumberOfIntervalsFixed),
        NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the locations of the grid lines.")]
        public ChartAxisTickDrawingOperationMode TickDrawingOperationMode
        {
            get
            {
                return m_tickDrawingOperationMode;
            }

            set
            {

                if (m_tickDrawingOperationMode != value)
                {
                    m_tickDrawingOperationMode = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font that is to be used for text that is rendered in association with the axis (such as axis labels).
        /// </summary>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple), DefaultValue(typeof(Font), "Verdana, 8pt")]
        [Description("Indicates font that is to be used for text that is rendered in association with the axis (such as axis labels).")]
        public Font Font
        {
            get
            {
                return m_font == null ? m_area.Chart.Font : m_font;
            }

            set
            {
                if (m_font != value)
                {
                    m_font = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title font that is to be used for text that is rendered in association with the axis (such as axis title).
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple), NotifyParentProperty(true) , DefaultValue(typeof(Font), "Verdana, 8pt")]
        [Browsable(true)]
        [Description("Indicates title font that is to be used for text that is rendered in association with the axis (such as axis title).")]
        public Font TitleFont
        {
            get
            {
                return m_titleFont == null ? m_area.Chart.Font : m_titleFont;
            }

            set
            {
                if (m_titleFont != value)
                {
                    m_titleFont = value;
                    OnDimensionsChanged(EventArgs.Empty);



                }
            }
        }

        /// <summary>
        /// Gets or sets the color that is to be used for text that is rendered in association with the axis (such as axis labels).
        /// </summary>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the color that is to be used for text that is rendered in association with the axis (such as axis labels)")]
        public Color ForeColor
        {
            get
            {
                if (m_foreColor.IsEmpty)
                {
                    if (m_area == null)
                    {
                        return Color.Black;
                    }

                    return m_area.Chart.ForeColor;
                }

                return m_foreColor;
            }

            set
            {
                if (m_foreColor != value)
                {
                    m_foreColor = value;
                    m_needUpdateVisibleLables = true;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title color that is to be used for text that is rendered in association with the axis (such as axis title).
        /// </summary>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Browsable(true)]
        [Description("Indicates the title color that is to be used for text that is rendered in association with the axis (such as axis title)")]
        public Color TitleColor
        {
            get
            {
                if (m_titleColor.IsEmpty)
                {
                    if (m_area == null)
                    {
                        return Color.Black;
                    }

                    return m_area.Chart.ForeColor;
                }

                return m_titleColor;
            }

            set
            {
                if (m_titleColor != value)
                {
                    m_titleColor = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title draw mode.
        /// </summary>
        /// <value>The title draw mode.</value>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple), DefaultValue(ChartTitleDrawMode.None)]
        [Description("Indicates the title color that is to be used for text that is rendered in association with the axis (such as axis title)")]
        public ChartTitleDrawMode TitleDrawMode
        {
            get 
            {
                return m_titleDrawMode; 
            }

            set
            {
                if (m_titleDrawMode != value)
                {
                    m_titleDrawMode = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a collection of pens by using which internal line of Polar and Radar chart is drawned.        
        /// </summary>
        [Description("Gets or sets a collection of pens by using which internal line of Polar and Radar chart is drawned."), DefaultValue(null)]
        public Pen[] Pens
        {
            get
            {
                return m_pens;
            }
            set
            {
                if (m_pens != value)
                {
                    m_pens = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the format for axis labels. If the value type of the axis is double, this format will be used to format axis labels for display.
        /// </summary>
        [DefaultValue(""), NotifyParentProperty(true)]
        [Description("Indicates the format for axis labels for Double value type. See \"Numeric Format Strings\".")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public string Format
        {
            get
            {
                return m_format;
            }

            set
            {
                if (m_format != value)
                {
                    m_format = value;
                    m_needUpdateVisibleLables = true;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the interval that gets calculated by the nice range calculation engine 
        /// should be in Years, Months, Weeks, Days, Hours, Minutes, 
        /// Seconds or MilliSeconds. This setting is used only if the ValueType of the axis is set to DateTime. Default value 
        /// is Auto.
        /// </summary>
        [DefaultValue(ChartDateTimeIntervalType.Auto), NotifyParentProperty(true)]
        [Description("Specifies whether the interval that gets calculated by the nice range calculation engine.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public ChartDateTimeIntervalType IntervalType
        {
            get
            {
                return m_intervalType;
            }

            set
            {

                if (m_intervalType != value)
                {
                    m_intervalType = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be reversed. When reversed, the axis will render points from right to left if horizontal and
        /// top to bottom when vertical.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the axis should be reversed.")]
        public bool Inversed
        {
            get
            {
                return m_inversed;
            }

            set
            {

                if (m_inversed != value)
                {
                    m_inversed = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the label intersect action. Labels can intersect on the axis if they are too close to each other. ChartAxis offers several options to enhance the display
        /// of the axis when such intersection occurs. Please see <see cref="ChartLabelIntersectAction"/> for more information.
        /// </summary>
        /// <remarks>
        ///	LabelIntersectAction is applicable for horizontally orientated axes only.
        /// </remarks>
        [DefaultValue(ChartLabelIntersectAction.None), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the label intersect action")]
        public ChartLabelIntersectAction LabelIntersectAction
        {
            get
            {
                return m_intersectAction;
            }
            set
            {

                if (m_intersectAction != value)
                {
                    m_intersectAction = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates whether partially visible axis labels should be hidden.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), Description("Hides partially visible axis labels."), ChartTemplate(ChartTemplateSet.Simple)]
        public bool HidePartialLabels
        {
            get
            {
                return m_hidePartialLabels;
            }

            set
            {
                if (m_hidePartialLabels != value)
                {
                    m_hidePartialLabels = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether labels can be rotated.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether labels can be rotated.")]
        public bool LabelRotate
        {
            get
            {
                return m_labelRotate;
            }

            set
            {

                if (m_labelRotate != value)
                {
                    m_labelRotate = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the angle at which labels are to be rotated.
        /// </summary>
        [DefaultValue(0), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the angle at which labels are to be rotated.")]
        public int LabelRotateAngle
        {
            get
            {
                return m_labelRotateAngle;
            }

            set
            {

                if (m_labelRotateAngle != value)
                {
                    m_labelRotateAngle = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the collection of labels associated with this axis.
        /// </summary>
        [TypeConverter(typeof(CollectionConverter)), Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartAxisLabelCollection Labels
        {
            get
            {
                return m_labels as ChartAxisLabelCollection;
            }
        }

        /// <summary>
        ///    Use this property to assign a custom implementation of <see cref="IChartAxisLabelModel"/>. If you are working with the
        ///    default label collection, use <see cref="ChartAxis.Labels"/>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public IChartAxisLabelModel LabelsImpl
        {
            get
            {
                return m_labels;
            }

            set
            {
                if (m_labels != value)
                {
                    if (this.Labels != null)
                    {
                        this.Labels.Changed -= new EventHandler(this.OnNeedResize);
                    }

                    m_labels = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Collection of grouping labels associated with this axis.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartAxisGroupingLabelCollection GroupingLabels
        {
            get
            {
                ChartAxisGroupingLabelCollection defaultLabels = m_groupingLabels as ChartAxisGroupingLabelCollection;

                //				if( defaultLabels == null )
                //				{
                //					throw new InvalidOperationException( "Default label implementation has been replaced with custom implementation." );
                //				}

                return defaultLabels;
            }
        }

        /// <summary>
        ///    Use this property to assign a custom implementation of <see cref="IChartAxisLabelModel"/>. If you are working with the
        ///    default label collection, use <see cref="ChartAxis.Labels"/>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IChartAxisGroupingLabelModel GroupingLabelsImpl
        {
            get
            {
                return m_groupingLabels;
            }

            set
            {
                if (this.GroupingLabels != null)
                {
                    this.GroupingLabels.Changed -= new EventHandler(this.OnNeedResize);
                }

                m_groupingLabels = value;
                this.OnDimensionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns attributes of the primary axis line. Please refer to <see cref="LineInfo"/> for more information on these attributes and
        /// how they can change the appearance of the axis line.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        [Description("Contains attributes of the axis line.")]
        [ChartTemplate(ChartTemplateSet.Content)]
        public LineInfo LineType
        {
            get
            {
                return m_lineType;
            }
        }

        /// <summary>
        /// Gets or sets the log base that is to be used when value is logarithimic. Default is base 10.
        /// </summary>
        [DefaultValue(10), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("The log base that is to be used when value is logarithimic.")]
        public int LogBase
        {
            get
            {
                return m_logBase;
            }

            set
            {
                if (m_logBase != value)
                {
                    m_logBase = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Offset. It specifies the offset that should be applied to the automatically calculated range's start value.
        /// </summary>
        [DefaultValue(0.0), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the offset that should be applied to the automatically calculated range's start value")]
        public double Offset
        {
            get
            {
                return m_offset;
            }

            set
            {
                if (m_offset != value)
                {
                    m_offset = value;
                    this.InvalidateRanges();
                }
            }
        }

        /// <summary>
        /// Gets or sets the PointOffset. It specifies the points offset that should be applied to the automatically calculated range's start value.
        /// </summary>
        [DefaultValue(0.0), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the points offset that should be applied to the automatically calculated range's start value")]
        public double PointOffset
        {
            get
            {
                return m_pointOffset;
            }

            set
            {
                if (m_pointOffset != value)
                {
                    m_pointOffset = value;
                    this.InvalidateRanges();
                }
            }
        }
        
        /// <summary>
        /// If this axis is a secondary axis, setting this property to True will cause
        /// it to move to the opposite side of the primary axis. This property is False
        /// by default.
        /// </summary>
        [DefaultValue(false), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("If this axis is a secondary axis, setting this property to True will cause it to move to the opposite side of the primary axis")]
        public bool OpposedPosition
        {
            get
            {
                return m_opposedPosition;
            }

            set
            {

                if (m_opposedPosition != value)
                {
                    m_opposedPosition = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }
    
	    /// <summary>
        /// Gets or sets a value indicates whether label is located inside or outside of chart area.
        /// </summary>
        /// <value>The legends placement.</value>
        [Description("Specifies label is located inside or outside of chart area."), DefaultValue(ChartPlacement.Outside)]       
        public ChartPlacement AxisLabelPlacement
        {
            get
            {
                return m_axisLabelPlacement;
            }

            set
            {

                if (m_axisLabelPlacement != value)
                {
                    m_axisLabelPlacement = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the Orientation of the axis. You cannot change the orientation of primary axes. Primary axes are the ones that are
        /// created and available by default in the Axes collection.
        /// </summary>
        [NotifyParentProperty(true), Browsable(false), Description("Gets or sets the Orientation of the axis.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (m_orientation != value)
                {
                    m_orientation = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the range for an axis. By default, the chart will automatically calculate the range that is to be displayed. The range property allows you
        /// to change this range to be any range of your choice. Set ChartAxis.RangeType to Set for this to take effect.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Indicates the range for an axis.")]
        [ChartTemplate(ChartTemplateSet.ContentBehavior)]
        public MinMaxInfo Range
        {
            get
            {
                return m_range;
            }

            set
            {
                this.RangeType = ChartAxisRangeType.Set;
                SetRange(value);
            }
        }

        /// <summary>
        ///  Gets or sets the range type. <see cref="ChartAxisRangeType"/>
        /// </summary>
        [DefaultValue(ChartAxisRangeType.Auto), NotifyParentProperty(true)]
        [Description("Indicates whether the range should be calculated automatically.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public ChartAxisRangeType RangeType
        {
            get
            {
                return m_rangeType;
            }

            set
            {
                if (m_rangeType != value)
                {
                    m_rangeType = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of places that is to be used for rounding when numbers are used for display (default is 2).
        /// If this property less zero, rounding is disable.
        /// </summary>
        [DefaultValue(2), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple), Description("Gets or sets the number of places that is to be used for rounding when numbers are used for display (default is 2). If this property less zero, rounding is disable.")]
        public int RoundingPlaces
        {
            get
            {
                return m_roundingPlaces;
            }

            set
            {
                if (m_roundingPlaces != value)
                {
                    m_roundingPlaces = value;
                    m_needUpdateVisibleLables = true;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of small ticks to be displayed on the axis. By default, small ticks are not displayed.
        /// </summary>
        [DefaultValue(typeof(Size), "1, 1"), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the size of small ticks to be displayed on the axis.")]
        public Size SmallTickSize
        {
            get
            {
                return m_smallTickSize;
            }

            set
            {

                if (m_smallTickSize != value)
                {
                    m_smallTickSize = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of small ticks to be displayed per major interval. Default is 0.
        /// </summary>
        [DefaultValue(0), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the number of small ticks to be displayed per major interval.")]
        public int SmallTicksPerInterval
        {
            get
            {
                return m_smallTicksPerInterval;
            }

            set
            {
                if (m_smallTicksPerInterval != value)
                {
                    m_smallTicksPerInterval = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the collection of strip lines. Please refer to <see cref="ChartStripLine"/> for more information.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartStripLineCollection StripLines
        {
            get
            {
                return m_stripLines;
            }
        }

        /// <summary>
        /// Gets or sets the color of ticks that are rendered on the axis.
        /// </summary>
        [DefaultValue(typeof(Color), "ControlText"), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the color of ticks that are rendered on the axis.")]
        public Color TickColor
        {
            get
            {
                return m_tickColor;
            }

            set
            {
                if (m_tickColor != value)
                {
                    m_tickColor = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of ticks that are rendered on the axis.
        /// </summary>
        [DefaultValue(typeof(Size), "1, 1"), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the size of ticks that are rendered on the axis.")]
        public Size TickSize
        {
            get
            {
                return m_tickSize;
            }

            set
            {

                if (m_tickSize != value)
                {
                    m_tickSize = value;
                    OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        #region Title properties
        /// <summary>
        /// Gets or sets the title of this axis.
        /// </summary>
        [DefaultValue(""), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the title text of this axis")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Title
        {
            get
            {
                return m_title;
            }

            set
            {

                if (m_title != value)
                {
                    m_title = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between title and labels.
        /// </summary>
        /// <value>The spacing.</value>
        [DefaultValue(4f), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the spacing between title and labels.")]
        public float TitleSpacing
        {
            get 
            { 
                return m_titleSpacing;
            }

            set
            {
                if (m_titleSpacing != value)
                {
                    m_titleSpacing = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the axis title.
        /// </summary>
        [DefaultValue(StringAlignment.Center), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the alignment of title.")]
        public StringAlignment TitleAlignment
        {
            get
            {
                return m_titleAlignment;
            }

            set
            {

                if (m_titleAlignment != value)
                {
                    m_titleAlignment = value;
                    this.OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the ToolTip of the axis.
        /// </summary>
        [Description("Indicates the ToolTip of the axis."), Category("Appearance"), DefaultValue(""), ChartTemplate(ChartTemplateSet.Simple)]
        public string ToolTip
        {
            get
            {
                return m_toolTip;
            }

            set
            {
                if (m_toolTip != value)
                {
                    m_toolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of value that this axis is displaying. For the types supported, refer to <see cref="ChartValueType"/>.
        /// </summary>
        /// <remarks>
        /// If the <b>ChartValueType.Custom</b> type is set, labels gets from the <see cref="ChartAxis.LabelsImpl"/> by index or position of label.
        /// Elsewhere labels is generated by value of label. 
        /// <p/>
        /// The <b>ChartValueType.Custom</b> can't guarantee correct position of labels. 
        /// You can use the other ways to implements the custom labels, such as to use the <see cref="ChartAxis.FormatLabel"/> event.
        /// </remarks>
        [DefaultValue(ChartValueType.Double), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the type of value that this axis is displaying.")]
        public ChartValueType ValueType
        {
            get
            {
                return m_valueType;
            }

            set
            {

                if (m_valueType != value)
                {
                    m_valueType = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mode of drawing of tick labels.
        /// </summary>
        [DefaultValue(ChartAxisTickLabelDrawingMode.AutomaticMode), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the mode of drawing of tick labels.")]
        public ChartAxisTickLabelDrawingMode TickLabelsDrawingMode
        {
            get
            {
                return tickLabelDrawingMode;
            }

            set
            {
                if (tickLabelDrawingMode != value)
                {
                    tickLabelDrawingMode = value;
                    //					this.OnAppearanceChanged(EventArgs.Empty);
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the visible range when zoomed.
        /// </summary>
        /// <remarks>
        ///	Don't try to change this property manually. The value will be changed if any of related properties is changed.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual MinMaxInfo VisibleRange
        {
            get
            {
                if (m_needUpdate)
                {
                    this.RecalculateRanges();
                    m_needUpdate = false;
                }

                return m_visibleRange;
            }
        }

        /// <summary>
        /// Gets the range that is currently zoomed in.   
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.ContentBehavior)]
        public MinMaxInfo ZoomedRange
        {
            get
            {
                if (m_needUpdate)
                {
                    this.RecalculateRanges();
                    m_needUpdate = false;
                }

                return m_zoomedRange;
            }
        }

        /// <summary>
        /// Gets or sets the factor that is to be used to calculate Zoomed range.  
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomFactor
        {
            get
            {
                return m_zoomFactor;
            }

            set
            {
                if (SmartDateZoom && ValueType == ChartValueType.DateTime)
                {
                    //value = SetSmartZoomFactor(value);
                }

                this.SetZooming(value, m_zoomPosition);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value(ZoomPosition) displayed on this axis when zoomed as a fraction of the total range. For example
        /// if the total range is 20 and the minimum value currently displayed is 10, the ZoomPosition will be
        /// 0.5 (10/20).    
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomPosition
        {
            get
            {
                return m_zoomPosition;
            }

            set
            {
                this.SetZooming(m_zoomFactor, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom actions.
        /// </summary>
        /// <value>The zoom actions.</value>
        [DefaultValue(ChartZoomingAction.None), Description("Zooming action allowed to axis."), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        public ChartZoomingAction ZoomActions
        {
            get 
            { 
                return m_zoomingAction;
            }

            set 
            { 
                m_zoomingAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the the parameter types of <see cref="IChartAxisLabelModel.GetLabelAt"/> method.
        /// </summary>
        /// <value>The parameter type.</value>
        [DefaultValue(ChartCustomLabelsParameter.Index)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartCustomLabelsParameter CustomLabelsParameter
        {
            get
            { 
                return m_customLabelsParameter; 
            }

            set 
            { 
                m_customLabelsParameter = value; 
            }
        }

        #region ShouldSerialize and Reset methods
        /// <summary>
        /// Resets the fore color to default value.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetForeColor()
        {
            this.ForeColor = Color.Empty;
        }

        /// <summary>
        /// Resets the color of the title to default value.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetTitleColor()
        {
            this.TitleColor = Color.Empty;
        }

        /// <summary>
        /// Resets the color of the tick.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ResetTickColor()
        {
            this.TickColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Determines if the DateTimeOffset property was modified.
        /// </summary>
        /// <returns>True if property was modified, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeDateTimeOffset()
        {
            if (this.DateTimeOffset == TimeSpan.Parse("00:00:00"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if the ForeColor property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeForeColor()
        {
            return !m_foreColor.IsEmpty;
        }

        /// <summary>
        /// Determines if the ForeColor property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeTitleColor()
        {
            return !m_titleColor.IsEmpty;
        }

        /// <summary>
        /// Determines if OriginDate property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeOriginDate()
        {
            return m_origin != 0;
        }

        /// <summary>
        /// Determines if SmallTickSize property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeSmallTickSize()
        {
            return (this.SmallTickSize.Height != 1) || (this.SmallTickSize.Width != 1);
        }

        /// <summary>
        /// Determines if the TickColor property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeTickColor()
        {
            return this.TickColor != SystemColors.ControlText;
        }

        /// <summary>
        /// Determines if TickSize property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeTickSize()
        {
            if ((this.TickSize.Height == 1) && (this.TickSize.Width == 1))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if the size property was modified.
        /// </summary>
        /// <returns>True if size property was modified, otherwise false.</returns>
        protected bool ShouldSerializeSize()
        {
            return false;
        }

        /// <summary>
        /// Determine if the RangeType property was modified.
        /// </summary>
        /// <returns>True if Range property was modified, otherwise false.</returns>
        protected bool ShouldSerializeRange()
        {
            return m_rangeType == ChartAxisRangeType.Set;
        }

        /// <summary>
        /// Gets the default orientation.
        /// </summary>
        protected ChartOrientation DefaultOrientation
        {
            get
            {
                return m_defaultAxisOrienation;
            }
        }

        /// <summary>
        /// Calculates value indicates that orientation property will be serialized by designer.
        /// </summary>
        /// <returns>True if value will be serialized, otherwise false.</returns>
        protected bool ShouldSerializeOrientation()
        {
            return Orientation != DefaultOrientation;
        }

        /// <summary>
        /// Resets the orientation.
        /// </summary>
        protected void ResetOrientation()
        {
            Orientation = m_defaultAxisOrienation;
        }

        /// <summary>
        /// Calculates value indicates that orientation property will be serialized by designer.
        /// </summary>
        /// <returns>True if value will be serialized, otherwise false.</returns>
        protected bool ShouldSerializeInterlacedGridInterior()
        {
            return InterlacedGridInterior != new BrushInfo(Color.LightGray);
        }

        /// <summary>
        /// Resets the orientation.
        /// </summary>
        protected void ResetInterlacedGridInterior()
        {
            InterlacedGridInterior = new BrushInfo(Color.LightGray);
        }
        #endregion

        #region Internal properties
        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        internal ChartAxisLayout Layout
        {
            get
            {
                return m_layout;
            }

            set
            {
                if (m_layout == null)
                {
                    m_layout = value;
                }
                else
                {
                    if (value == null)
                    {
                        m_layout = value;
                    }
                    else
                    {
                        throw new ArgumentException("Axis is added to layout already.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartAxis"/> is primary.
        /// </summary>
        internal bool Primary
        {
            get
            {
                if (m_area != null)
                {
                    if (m_orientation == ChartOrientation.Horizontal)
                    {
                        return this == m_area.PrimaryXAxis;
                    }

                    return this == m_area.PrimaryYAxis;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the visible labels.
        /// </summary>
        /// <value>The visible labels.</value>
        private ChartAxisLabel[] VisibleLabels
        {
            get
            {
                if (m_needUpdateVisibleLables)
                {
                    this.RecalculateVisibleLabels(m_area);
                    m_needUpdateVisibleLables = false;
                }

                return m_visibleLables;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ChartAxis"/> is indexed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is indexed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsIndexed
        {
            get
            {
                if (m_area == null) return false;
                if (m_area.Chart == null) return false;

                if (m_area.AxesType == ChartAreaAxesType.Rectangular)
                {
                    if (m_area.RequireInvertedAxes)
                    {
                        if (m_orientation == ChartOrientation.Horizontal) return false;
                    }
                    else
                    {
                        if (m_orientation == ChartOrientation.Vertical) return false;
                    }
                }
                else
                {
                    return false;
                }

                return m_area.IsIndexed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether breaks is enabled.
        /// </summary>
        /// <value><c>true</c> if breaks is enabled; otherwise, <c>false</c>.</value>
        private bool BreaksEnabled
        {
            get
            {
                if (!m_makeBreaks)
                    return false;

                if (m_zoomFactor != 1d)
                    return false;

                if (m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
                {
                    if (m_valueType != ChartValueType.Double)
                        return false;
                }

                return !m_breakRanges.IsEmpty;
            }
        }
        #endregion

        /// <summary>
        /// Internal property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IChartScrollBar ScrollBar
        {
            get 
            { 
                return m_scrollBar; 
            }

            set 
            { 
                m_scrollBar = value; 
            }
        }        

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether this instance is visible.")]      
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool IsVisible
        {
            get 
            {
                return m_isVisible; 
            }

            set
            {
                if (m_isVisible != value)
                {
                    m_isVisible = value;
                    this.OnDimensionsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets LebelsOffset. It specifies the offset that should be applied to the automatically calculated labels.
        /// </summary>
        /// <value>The labels offset.</value>
        [DefaultValue(0d), NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the offset that should be applied to the automatically calculated labels.")]
        public double LabelsOffset
        {
            get
            { 
                return m_labelsOffset;
            }

            set
            {
                if (m_labelsOffset != value)
                {
                    m_labelsOffset = value;
                    this.OnIntervalsChanged(EventArgs.Empty);
                }
            }
        }       

		 /// <summary>
        /// Get or Sets the string format of label.Default value is StringFormat.GenericDefault.
        /// </summary>
        [Description("Get or Sets the string format for the label.Default value is StringFormat.GenericDefault.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringFormat LabelStringFormat
        {
            get
            {
                return m_labelStringFormat;
            }
            set
            {
                if (m_labelStringFormat != value)
                    m_labelStringFormat = value;
            }

        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when appearance was changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public event EventHandler AppearanceChanged;

        /// <summary>
        /// Occurs then dimensions was changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public event EventHandler DimensionsChanged;

        /// <summary>
        /// Occurs then intervals was changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public event EventHandler IntervalsChanged;

        /// <summary>
        /// Occurs when visible range was changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public event EventHandler VisibleRangeChanged;

        /// <summary>
        /// Event for dynamic formatting of axis labels.
        /// </summary>
        /// <example>
        /// xAxis.FormatLabel += new ChartFormatAxisLabelEventHandler(XAxis_FormatLabel);<p/>
        /// <b>...</b><p/>
        /// void XAxis_FormatLabel(object sender, ChartFormatAxisLabelEventArgs args)<p/>
        /// {<p/>
        ///	   args.Label = "Category" + args.Value;<p/>
        ///    args.Handled = true;<p/>
        /// }
        /// </example>
        /// <seealso cref="ChartFormatAxisLabelEventHandler"/>
        /// <seealso cref="ChartFormatAxisLabelEventArgs"/>
        [Browsable(false)]
        public event ChartFormatAxisLabelEventHandler FormatLabel;

        /// <summary>
        /// Occurs when <see cref="ChartAxis.ZoomFactor"/> or <see cref="ChartAxis.ZoomPosition"/> properties are changing.
        /// </summary>
        [Browsable(false)]
        public event ChartAxisZoomingEventHandler Zooming;

        /// <summary>
        /// Occurs when <see cref="ChartAxis.ZoomFactor"/> or <see cref="ChartAxis.ZoomPosition"/> properties are changed.
        /// </summary>
        [Browsable(false)]
        public event EventHandler Zoomed;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxis"/> class.
        /// </summary>
        public ChartAxis()
            : this(ChartOrientation.Horizontal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxis"/> class.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public ChartAxis(ChartOrientation orientation)
        {
            m_orientation = orientation;
            m_defaultAxisOrienation = orientation;
            m_dateTimeInterval = this.DateTimeRange.DefaultInterval;
            
            ChartAxisLabelCollection labels = new ChartAxisLabelCollection();
            labels.Changed += new EventHandler(this.OnNeedResize);
            m_labels = labels;

            m_breakInfo.Changed += new EventHandler(this.OnNeedRedraw);

            m_breakRanges = new ChartAxisRange(this);
            m_breakRanges.Changed += new EventHandler(OnBreakRangesChanged);

            ChartAxisGroupingLabelCollection groupingLabels = new ChartAxisGroupingLabelCollection();
            groupingLabels.Changed += new EventHandler(this.OnNeedResize);
            m_groupingLabels = groupingLabels;

            m_stripLines = new ChartStripLineCollection();
            m_stripLines.Changed += new EventHandler(this.OnNeedRedraw);

            m_lineType = new LineInfo();
            m_lineType.SettingsChanged += new EventHandler(this.OnNeedRedraw);

            m_gridLineType = new LineInfo();
            m_gridLineType.SettingsChanged += new EventHandler(this.OnNeedRedraw);

            m_minorGridLineType.SettingsChanged += new EventHandler(this.OnNeedRedraw);

            m_range.SettingsChanged += new EventHandler(this.OnNeedResize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxis"/> class.
        /// </summary>
        /// <param name="isPrimary">if set to <c>true</c> axis is primary.</param>
        [Obsolete("Primary axis is computed automatically, as first vertical or horizontal axis.")]
        public ChartAxis(bool isPrimary)
            : this(ChartOrientation.Horizontal)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="area">The area.</param>
        internal void SetOwner(ChartArea area)
        {
            m_area = area;
        }

        /// <summary>
        /// Unsubscribes the specified target from all events.
        /// </summary>
        /// <param name="target">The target.</param>
        internal void Unsubscribe(object target)
        {
            AppearanceChanged = Unsubscribe(AppearanceChanged, target) as EventHandler;
            DimensionsChanged = Unsubscribe(DimensionsChanged, target) as EventHandler;
            IntervalsChanged = Unsubscribe(IntervalsChanged, target) as EventHandler;
            VisibleRangeChanged = Unsubscribe(VisibleRangeChanged, target) as EventHandler;
            FormatLabel = Unsubscribe(FormatLabel, target) as ChartFormatAxisLabelEventHandler;
        }

        /// <summary>
        /// Unsubscribes object from specified delegate.
        /// </summary>
        /// <param name="del">The delegate to unsubscribe.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        internal Delegate Unsubscribe(Delegate del, object target)
        {
            if (null == del)
                return null;
            if (null == target)
                return del;

            Delegate[] list = del.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                if (target == list[i].Target)
                    list[i] = null;
            }

            return Delegate.Combine(list);
        }

        /// <summary>
        /// Gets the dimension.
        /// </summary>
        /// <param name="g">The graphics content.</param>
        /// <param name="chartarea">The chart area.</param>
        /// <returns>Dimension.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public float GetDimension(Graphics g, ChartArea chartarea)
        {
            return GetDimension(g, chartarea, chartarea.RenderBounds);
        }

        /// <summary>
        /// Gets the dimension.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="chartarea">The chartarea.</param>
        /// <param name="renderBounds">The render bounds.</param>
        /// <returns></returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public float GetDimension(Graphics g, ChartArea chartarea, RectangleF renderBounds)
        {
            float dim = m_orientation == ChartOrientation.Vertical ? m_tickSize.Width : m_tickSize.Height;

            if (m_needUpdateVisibleLables)
            {
                this.RecalculateVisibleLabels(chartarea);
                m_needUpdateVisibleLables = false;
            }

            dim += this.DoTickLabelsLayout(g, this.Rect, dim,chartarea);

            m_ticksAndLabelsDimension = dim;

            dim += this.DoGroupingLabelsLayout(g, dim);

            string title = this.Title;
            float length = m_orientation == ChartOrientation.Vertical
                ? renderBounds.Height : renderBounds.Width;
            
            if (!string.IsNullOrEmpty(title) && this.AxisLabelPlacement == ChartPlacement.Outside)
            {
                dim += m_titleSpacing;

                if (m_titleDrawMode == ChartTitleDrawMode.Wrap)
                {
                    dim += g.MeasureString(title, this.TitleFont).Height;
                }
                else
                {
                    if (m_titleDrawMode == ChartTitleDrawMode.Ellipsis)
                    {
                        title = DrawingHelper.EllipsesText(g, title, this.TitleFont, length);
                    }

                    dim += g.MeasureString(title, this.TitleFont).Height;
                }
            }

            return m_isVisible ? m_dimension = dim : m_dimension = 0;
        }

		/// <summary>
        /// Gets the dimension of title.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="chartarea">The chartarea.</param>
        /// <param name="renderBounds">The render bounds.</param>
        /// <returns></returns>
        /// <internalonly/>
        internal float GetTitleDimention(Graphics g, ChartArea chartarea, RectangleF renderBounds)
        {
            float dim=0;
            string title = this.Title;
            float length = m_orientation == ChartOrientation.Vertical
                ? renderBounds.Height : renderBounds.Width;

            if (!string.IsNullOrEmpty(title) && this.AxisLabelPlacement == ChartPlacement.Inside)
            {
                dim += m_titleSpacing;

                if (m_titleDrawMode == ChartTitleDrawMode.Wrap)
                {
                    dim += g.MeasureString(title, this.TitleFont, (int)length).Height;
                }
                else
                {
                    if (m_titleDrawMode == ChartTitleDrawMode.Ellipsis)
                    {
                        title = DrawingHelper.EllipsesText(g, title, this.TitleFont, length);
                    }

                    dim += g.MeasureString(title, this.TitleFont).Height;
                }
            }
            return dim;
        }
        /// <summary>
        /// Draws the axis.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="chartarea">The chartarea.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void DrawAxis(Graphics g, ChartArea chartarea)
        {
            Pen tickPen;
            Pen smallTickPen;
            if (chartarea.RequireInvertedAxes)
            {
             CrossInvertedAxis(chartarea);         
            }
            else
            {
            CrossAxis(chartarea);          
            }
                                  
            adjustPlotAreaMargins = chartarea.AdjustPlotAreaMargins;
            chartPlotAreaMargins = chartarea.ChartPlotAreaMargins;
            
            if (m_needUpdateDimension)
            {
                this.GetDimension(g, chartarea);
                if (chartarea.ReDrawAxes != true)
                      m_needUpdateDimension = false;
            }

            float opposedCoef;
			if (AxisLabelPlacement == ChartPlacement.Inside)
                    opposedCoef = OpposedPosition ? 1 : -1;
                else
                    opposedCoef = OpposedPosition ? -1 : 1;

            if (Orientation == ChartOrientation.Horizontal)
            {
                #region Horizontal
                tickPen = new Pen(m_tickColor, m_tickSize.Width);
                smallTickPen = new Pen(m_tickColor, SmallTickSize.Width);

                //Axis Line:
                g.DrawLine(LineType.Pen, Location.X, Location.Y, Location.X + RealLength, Location.Y);
                float tempopposedCoef = opposedCoef;
                for (int i = 0; i < m_visibleLables.Length; i++)
                {
                    if (m_visibleLables[i].AxisLabelPlacement != this.AxisLabelPlacement)
                        opposedCoef =  -(opposedCoef);
                    else
                        opposedCoef = tempopposedCoef;
                    float tlx = this.GetCoordinateFromValue(m_visibleLables[i].DoubleValue);
                    g.DrawLine(tickPen, tlx, Location.Y, tlx, Location.Y + opposedCoef * TickSize.Height);
                }

                if (m_smallTicksPerInterval > 0)
                {
                    IEnumerable<float> ticks = this.GetSmallTicks();

                    foreach (float tick in ticks)
                    {
                        g.DrawLine(smallTickPen, tick, Location.Y,
                            tick, Location.Y + opposedCoef * SmallTickSize.Height);
                    }
                }
                #endregion
            }
            else
            {
                #region Vertical
                tickPen = new Pen(TickColor, TickSize.Height);
                smallTickPen = new Pen(TickColor, SmallTickSize.Height);
                //Axis Line:
                g.DrawLine(LineType.Pen, Location.X, Location.Y, Location.X, Location.Y - RealLength);                
                float tempopposedCoef = opposedCoef;
                for (int i = 0; i < m_visibleLables.Length; i++)
                {
                    if (m_visibleLables[i].AxisLabelPlacement != this.AxisLabelPlacement)
                        opposedCoef = -(opposedCoef);
                      else
                        opposedCoef = tempopposedCoef;
                    float tly = this.GetCoordinateFromValue(m_visibleLables[i].DoubleValue);
                    g.DrawLine(tickPen, Location.X, tly, Location.X - opposedCoef * TickSize.Width, tly);
                }

                if (m_smallTicksPerInterval > 0)
                {
                    IEnumerable<float> ticks = this.GetSmallTicks();

                    foreach (float tick in ticks)
                    {
                        g.DrawLine(smallTickPen, Location.X,
                            tick, Location.X - opposedCoef * SmallTickSize.Width, tick);
                    }
                }
                #endregion
            }

            if (chartarea.Chart.NeedRegionUpdate)
            {
                chartarea.Chart.ChartRegions.Add(new ChartRegion(new Region(this.Rect), ChartRegionType.Axis,m_toolTip,chartarea.Axes.IndexOf(this), "Axes region"));
            }

            this.DrawAxisText(g, Rect);
            this.DrawTickLabels(g,chartarea);

            if (m_drawTickLabelGrid)
            {
                this.DrawTickLabelGrids(g, tickPen, this.Rect);
            }

            tickPen.Dispose();
            smallTickPen.Dispose();

            for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
            {
                ChartAxisGroupingLabel label = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                float cPos = opposedCoef * this.groupingLabelsRowsDimensions[label.Row];

                if (m_orientation == ChartOrientation.Horizontal)
                {
                    cPos = m_location.Y + cPos;
                }
                else
                {
                    cPos = m_location.X - cPos;
                }

                label.Draw(g, cPos, this);
                chartarea.Chart.ChartRegions.Add(label.GetRegion(this));                
            }
        }
        /// <summary>
        /// Cross the axis. Location of the axis is changed based on the specified value.
        /// </summary>
        /// <param name="chartarea">The chartarea.</param>
        /// <internalonly />
        internal void CrossAxis(ChartArea chartarea)
        {
            if (!double.IsNaN(this.Crossing)&& this.Primary )
            {
                if (this.Orientation == ChartOrientation.Vertical)
                {
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryXAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryXAxis.Range.Min : this.Crossing;
                    ChartPoint cpt = new ChartPoint(this.Crossing, this.ZoomedRange.Min);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);
                }
                else
                {
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryYAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryYAxis.Range.Min : this.Crossing;
                    ChartPoint cpt = new ChartPoint(this.ZoomedRange.Min, this.Crossing);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);
                }
            }
            else if (!double.IsNaN(this.Crossing) && !this.Primary)
            {
                if (this.Orientation == ChartOrientation.Vertical)
                {
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryXAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryXAxis.Range.Min : this.Crossing;
                    double pty = (chartarea.YAxesLayoutMode == ChartAxesLayoutMode.SideBySide) ? chartarea.PrimaryYAxis.ZoomedRange.Max : chartarea.PrimaryYAxis.ZoomedRange.Min;
                    ChartPoint cpt = new ChartPoint(this.Crossing, pty);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);
                }
                else
                {
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryYAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryYAxis.Range.Min : this.Crossing;
                    double ptx = (chartarea.XAxesLayoutMode == ChartAxesLayoutMode.SideBySide) ? chartarea.PrimaryXAxis.ZoomedRange.Max : chartarea.PrimaryXAxis.ZoomedRange.Min;
                    ChartPoint cpt = new ChartPoint(ptx, this.Crossing);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);
                }
            }
        }

        /// <summary>
        /// Cross the Inverted axis. Location of the axis is changed based on the specified value.
        /// </summary>
        /// <param name="chartarea">The chartarea.</param>
        /// <internalonly />
        internal void CrossInvertedAxis(ChartArea chartarea)
        {
            if (!double.IsNaN(this.Crossing))
            {
                if (this.Orientation == ChartOrientation.Vertical)
                {
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryXAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryXAxis.Range.Min : this.Crossing;
                    ChartPoint cpt = new ChartPoint(this.ZoomedRange.Min, this.Crossing);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);
                }
                else
                {
                    
                    this.Crossing = (this.Crossing == Double.MaxValue) ? chartarea.PrimaryYAxis.Range.Max : (this.Crossing == Double.MinValue) ? chartarea.PrimaryYAxis.Range.Min : this.Crossing;
                    ChartPoint cpt = new ChartPoint(this.Crossing, this.ZoomedRange.Min);
                    Point pt = chartarea.GetPointByValue(cpt);
                    this.Location = new PointF(pt.X, pt.Y);

                }
            }
        }
        /// <summary>
        /// Draws the axis.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="chartarea">The chartarea.</param>
        /// <param name="z">The z.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void DrawAxis(Graphics3D g, ChartArea chartarea, float z)
        {
            if (VisibleRange.Delta <= 0)
            {
                return;
            }
            if (chartarea.RequireInvertedAxes)
            {
                CrossInvertedAxis(chartarea);
            }
            else
            {
                CrossAxis(chartarea);
            }
            ArrayList pths = new ArrayList();

            Pen tickPen;
            Pen smallTickPen;

            float opposedCoef;
			if (AxisLabelPlacement == ChartPlacement.Inside)
                    opposedCoef = OpposedPosition ? 1 : -1;
                else
                    opposedCoef = OpposedPosition ? -1 : 1;
					
            float dim = GetDimension(g.Graphics, chartarea);
            adjustPlotAreaMargins = chartarea.AdjustPlotAreaMargins;
            chartPlotAreaMargins = chartarea.ChartPlotAreaMargins;

            GraphicsPath gp = new GraphicsPath();

            if (Orientation == ChartOrientation.Horizontal)
            {
                #region Horizontal
                tickPen = new Pen(m_tickColor, m_tickSize.Width);
                smallTickPen = new Pen(m_tickColor, SmallTickSize.Width);

                //Axis Line:
                float x = Rect.X;
                float y = (!m_opposedPosition) ? m_location.Y : (m_location.Y - m_tickSize.Height);

                gp.AddLines(new PointF[]{ new PointF( m_location.X + m_length, m_location.Y ),
                                  new PointF( m_location.X, m_location.Y ),
                                  new PointF( m_location.X, m_location.Y+1 ),
                                  new PointF( m_location.X, m_location.Y ) });
                gp.CloseFigure();

                for (int i = 0; i < m_visibleLables.Length; i++)
                {
                    float tlx = this.GetCoordinateFromValue(m_visibleLables[i].DoubleValue);
                    gp.AddLine(tlx, Location.Y, tlx, Location.Y + opposedCoef * TickSize.Height);
                    gp.CloseFigure();
                }

                if (m_smallTicksPerInterval > 0)
                {
                    IEnumerable<float> ticks = this.GetSmallTicks();

                    foreach (float tick in ticks)
                    {
                        gp.AddLine(tick, Location.Y,
                            tick, Location.Y + opposedCoef * SmallTickSize.Height);
                        gp.CloseFigure();
                    }
                }
                #endregion
            }
            else
            {
                #region Vertical
                tickPen = new Pen(TickColor, TickSize.Height);
                smallTickPen = new Pen(TickColor, SmallTickSize.Height);

                //Axis Line:
                gp.AddLines(new PointF[]{ new PointF( Location.X, Location.Y - RealLength ),
                                  new PointF( Location.X, Location.Y ),
                                  new PointF( Location.X+1, Location.Y ),
                                  new PointF( Location.X, Location.Y ) });
                gp.CloseFigure();

                for (int i = 0; i < m_visibleLables.Length; i++)
                {
                    float tly = this.GetCoordinateFromValue(m_visibleLables[i].DoubleValue);
                    gp.AddLine(Location.X, tly, Location.X - opposedCoef * TickSize.Width, tly);
                    gp.CloseFigure();
                }

                if (m_smallTicksPerInterval > 0)
                {
                    IEnumerable<float> ticks = this.GetSmallTicks();

                    foreach (float tick in ticks)
                    {
                        gp.AddLine(Location.X, tick, Location.X + opposedCoef * SmallTickSize.Width, tick);
                        gp.CloseFigure();
                    }
                }
                #endregion
            }

            Path3D textGp = this.DrawAxisText(g, Rect, z);

            if (textGp != null)
            {
                pths.Add(textGp);
            }

            for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
            {
                ChartAxisGroupingLabel label = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                float cPos = opposedCoef * this.groupingLabelsRowsDimensions[label.Row];

                if (m_orientation == ChartOrientation.Horizontal)
                {
                    cPos = m_location.Y + cPos;
                }
                else
                {
                    cPos = m_location.X - cPos;
                }

                label.Draw(g, cPos, this);
                chartarea.Chart.ChartRegions.Add(label.GetRegion(this));
            }


            this.DrawTickLabels(g, pths, z,chartarea);

            pths.Add(Path3D.FromGraphicsPath(gp, z, m_lineType.Pen));

            Path3DCollect p3dcll = new Path3DCollect((Path3D[])pths.ToArray(typeof(Path3D)));
            g.AddPolygon(p3dcll);

            RectangleF rect = this.Rect;
            Polygon[] plg = g.CreateRectangle(new Vector3D(rect.X, rect.Y, 0), rect.Size, null, null);

            for (int i = 0; i < plg.Length; i++)
            {
                plg[i].RegionData = new ChartRegionData(ChartRegionType.Axis, m_toolTip, "Axes region");
            }

            tickPen.Dispose();
            smallTickPen.Dispose();
        }

        /// <summary>
        /// Docks to specified bounds.
        /// </summary>
        /// <param name="rect">The bounds.</param>
        internal RectangleF DockToRectangle(RectangleF rect)
        {
            if (m_orientation == ChartOrientation.Horizontal)
            {
                if (m_opposedPosition)
                {
                    m_location = new PointF(rect.Left, rect.Top);
                    rect.Y -= m_dimension;
                    rect.Height += m_dimension;
                }
                else
                {
                    m_location = new PointF(rect.Left, rect.Bottom);
                    rect.Height += m_dimension;
                }

                m_length = rect.Width;
            }
            else
            {
                if (m_opposedPosition)
                {
                    m_location = new PointF(rect.Right, rect.Bottom);
                    rect.Width += m_dimension;
                }
                else
                {
                    m_location = new PointF(rect.Left, rect.Bottom);
                    rect.X -= m_dimension;
                    rect.Width += m_dimension;
                }

                m_length = rect.Height;
            }

            m_scaleLablesCoef = m_scaleLables ? RealLength / m_scaleLength : 1f;

            return rect;
        }

        #region Zooming methods
        /// <summary>
        /// Changes the <see cref="ChartAxis.ZoomFactor"/> and the <see cref="ChartAxis.ZoomPosition"/> by center.
        /// </summary>
        /// <param name="factor">The new zoom factor.</param>
        public void CenteredZoom(double factor)
        {
            this.SetZooming(factor, m_zoomPosition + 0.5 * (m_zoomFactor - factor));
        }

        /// <summary>
        /// Zooms by range.
        /// </summary>
        /// <param name="zoomRange"></param>
        public void ZoomRange(DoubleRange zoomRange)
        {
            this.SetZooming(zoomRange.Delta / m_range.Delta, (zoomRange.Start - m_range.min) / m_range.Delta);
        }

        /// <summary>
        /// Multiplies the zoom factor by center.
        /// </summary>
        /// <param name="mulFactor">The mul factor.</param>
        public void MulZoomCenter(double mulFactor)
        {
            this.CenteredZoom(mulFactor * m_zoomFactor);
        }

        /// <summary>
        /// Resets the zoom.
        /// </summary>
        public void ResetZoom()
        {
            this.SetZooming(1, 0);
        }

        /// <summary>
        /// Zoom axis and updates zoom factor and zoom position.
        /// </summary>
        /// <param name="upPoint">The start point of zoomed rectangle.</param>
        /// <param name="downPoint">The end point of zoomed rectangle.</param>
        /// <param name="minZoomFactor">The min zoom factor.</param>
        /// <returns>True if axis was updated, otherwise false.</returns>
        public bool Zoom(PointF upPoint, PointF downPoint, double minZoomFactor)
        {
            bool result = false;
            float min = this.GetCoordinateFromValue(this.VisibleRange.Min);
            float max = this.GetCoordinateFromValue(this.VisibleRange.Max);
            if (this.Orientation == ChartOrientation.Horizontal)
            {
                if (!this.Inversed)
                {
                    result = (min < downPoint.X && max > upPoint.X) || (downPoint.X < min && upPoint.X > min) || (downPoint.X < max && upPoint.X > max) || (upPoint.X < min && downPoint.X > min) || (upPoint.X < max && downPoint.X > max);
                    if ((downPoint.X < min && upPoint.X > min) || (downPoint.X < max && upPoint.X > max))
                    {
                        upPoint.X = upPoint.X < max ? upPoint.X : max;
                        downPoint.X = downPoint.X > min ? downPoint.X : min;
                    }
                    if ((upPoint.X < min && downPoint.X > min) || (upPoint.X < max && downPoint.X > max))
                    {
                        upPoint.X = upPoint.X < min ? min : upPoint.X;
                        downPoint.X = downPoint.X > max ? max : downPoint.X;
                    }
                }
                else
                {
                    result = (max < downPoint.X && min > upPoint.X) || (downPoint.X < max && upPoint.X > max) || (downPoint.X < min && upPoint.X > min) || (upPoint.X < max && downPoint.X > max) || (upPoint.X < min && downPoint.X > min);
                    if ((downPoint.X < min && upPoint.X > min) || (downPoint.X < max && upPoint.X > max))
                    {
                        upPoint.X = upPoint.X > min ? min : upPoint.X;
                        downPoint.X = downPoint.X < max ? max : downPoint.X;
                    }
                    if ((upPoint.X < max && downPoint.X > max) || (upPoint.X < min && downPoint.X > min))
                    {
                        upPoint.X = upPoint.X < max ? max : upPoint.X;
                        downPoint.X = downPoint.X > min ? min : downPoint.X;
                    }
                }
            }
            else
            {
                if (!this.Inversed)
                {
                    result = (max < downPoint.Y && min > upPoint.Y) || (downPoint.Y > min && upPoint.Y < min) || (downPoint.Y > max && upPoint.Y < max) || (upPoint.Y > min && downPoint.Y < min) || (upPoint.Y > max && downPoint.Y < max);
                    if ((downPoint.Y > min && upPoint.Y < min) || (downPoint.Y > max && upPoint.Y < max))
                    {
                        upPoint.Y = upPoint.Y < max ? max : upPoint.Y;
                        downPoint.Y = downPoint.Y > min ? min : downPoint.Y;
                    }
                    if ((upPoint.Y > min && downPoint.Y < min) || (upPoint.Y > max && downPoint.Y < max))
                    {
                        upPoint.Y = upPoint.Y > min ? min : upPoint.Y;
                        downPoint.Y = downPoint.Y < max ? max : downPoint.Y;
                    }
                }
                else
                {
                    result = (max > downPoint.Y && min < upPoint.Y) || (downPoint.Y > max && upPoint.Y < max) || (downPoint.Y > min && upPoint.Y < min) || (upPoint.Y > min && downPoint.Y < min) || (upPoint.Y > max && downPoint.Y < max);
                    if ((downPoint.Y > max && upPoint.Y < max) || (downPoint.Y > min && upPoint.Y < min))
                    {
                        upPoint.Y = upPoint.Y < min ? min : upPoint.Y;
                        downPoint.Y = downPoint.Y > max ? max : downPoint.Y;
                    }
                    if ((upPoint.Y > min && downPoint.Y < min) || (upPoint.Y > max && downPoint.Y < max))
                    {
                        upPoint.Y = upPoint.Y > max ? max : upPoint.Y;
                        downPoint.Y = downPoint.Y < min ? min : downPoint.Y;
                    }
                }
            }

            if (result)
            {
                double start = this.GetRealValue(upPoint);
                double end = this.GetRealValue(downPoint);
                
                ChartMath.MinMax(start, end, out start, out end);
                if (end - start > 0)
                {
                    if (m_valueType == ChartValueType.Logarithmic)
                    {
                        start = Math.Log(start, m_logBase);
                        end = Math.Log(end, m_logBase);
                    }
                    else
                    {
                        double niceInterval = this.CalculateNiceInternal((end - start) / m_desiredIntervals);

                        start = Math.Max(m_range.min, ChartMath.Round(start, niceInterval));
                        end = Math.Min(m_range.max, ChartMath.Round(end, niceInterval));
                    }

                    if ((m_range.Contains(start)) && (start != end))
                    {

                        double zf = ChartMath.MinMax((end - start) / m_range.Delta, minZoomFactor, 1);
                        double zp = ChartMath.MinMax((start - m_range.Min) / m_range.Delta, 0, 1 - zf);

                        this.SetZooming(zf, zp);
                    }
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// The ChartAxis by default creates a display range based on data. If you do not wish to use this
        /// range you can set this range yourself using this method. When you set a custom range you have to
        /// set <see cref="RangeType"/> to <see cref="ChartAxisRangeType.Set"/>.
        /// </summary>
        /// <param name="value" type="Syncfusion.Windows.Forms.Chart.MinMaxInfo">
        ///     <para>
        ///     Range information with minimum and maximum values to be used.    
        ///     </para>
        /// </param>
        public void SetRange(MinMaxInfo value)
        {
            if (value.interval <= 0)
                value.interval = this.Range.interval;

            if (!m_range.Equals(value))
            {
                m_range.SettingsChanged -= new EventHandler(OnRangeChanged);
                m_range = value;
                m_range.SettingsChanged += new EventHandler(OnRangeChanged);

                this.InvalidateRanges();
            }
        }

        /// <summary>
        /// Sets the nice range.
        /// </summary>
        /// <param name="baseRange">The base range.</param>
        /// <param name="pattingType">Type of the patting.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetNiceRange(DoubleRange baseRange, ChartAxisRangePaddingType pattingType)
        {
            if (this.IsIndexed)
            {
                #region Indexed type
                if (pattingType == ChartAxisRangePaddingType.None)
                {
                    this.SetRange(new MinMaxInfo(0, m_area.Chart.IndexValues.Count - 1, 1));
                }
                else
                {
                    this.SetRange(new MinMaxInfo(-1, m_area.Chart.IndexValues.Count, 1));
                }
                #endregion
            }
            else if (m_valueType == ChartValueType.DateTime)
            {
                #region DateTime type
                DateTime start = DateTime.FromOADate(baseRange.Start);
                DateTime end = DateTime.FromOADate(baseRange.End);

                if (start == end)
                {
                    start = start.AddDays(-0.5);
                    end = end.AddDays(0.5);
                }

                ChartDateTimeNiceRangeMaker dateTimeNiceRangeMaker = new ChartDateTimeNiceRangeMaker();

                dateTimeNiceRangeMaker.DesiredIntervals = m_desiredIntervals;
                dateTimeNiceRangeMaker.RangePaddingType = pattingType;
                dateTimeNiceRangeMaker.DesiredIntervalType = this.IntervalType;
                dateTimeNiceRangeMaker.PreferZero = this.PreferZero;
                dateTimeNiceRangeMaker.ForceZero = this.ForceZero;

                ChartDateTimeRange niceRange = dateTimeNiceRangeMaker.MakeNiceRange(start, end);

                double min = this.DateTimeToDouble(niceRange.Start);
                double max = this.DateTimeToDouble(niceRange.End);

                int intervalsCount = ChartDateTimeInterval.GetIntervalCount(niceRange.DefaultInterval.Iterator());
                this.SetRange(new MinMaxInfo(min, max, (max - min) / intervalsCount));
                #endregion
            }
            else
            {
                #region Double and Logarithmic types
                if (baseRange.Delta == 0)
                {
                    baseRange = DoubleRange.Inflate(baseRange, 0.5);
                }
                if (baseRange.Delta == 0)
                {
                    //Fix for adding large double value(more than 16 digit) as points in chart series   
                    baseRange = DoubleRange.Inflate(baseRange, baseRange.Start / 2);
                } 
                if (m_valueType == ChartValueType.Logarithmic)
                {
                    double logMin = Math.Log(baseRange.Start <= 0 ? double.Epsilon : baseRange.Start, m_logBase);
                    double logMax = Math.Log(baseRange.End <= 0 ? double.Epsilon : baseRange.End, m_logBase);

                    if (Double.IsNaN(logMin) || Double.IsInfinity(logMin)) logMin = float.Epsilon;
                    if (Double.IsNaN(logMax) || Double.IsInfinity(logMax)) logMax = 1;

                    baseRange = new DoubleRange(logMin, logMax);
                }

                if (m_forceZero)
                {
                    baseRange += 0;
                }

                double interval = baseRange.Delta / m_desiredIntervals;

                if (m_valueType == ChartValueType.Logarithmic && Math.Abs(interval) < 1)
                {
                    interval = Math.Sign(interval);
                }
                else
                {
                    interval = this.CalculateNiceInternal(interval);
                }

                baseRange = this.CalculatePadding(baseRange, interval, pattingType);
                baseRange = this.TweakToZero(baseRange, interval);

                this.SetRange(new MinMaxInfo(baseRange.Start, baseRange.End, interval));
                #endregion
            }
        }

        /// <summary>
        /// Marks the axis and related rendering information as out of date.
        /// </summary>
        public void Update()
        {
            m_needUpdate = true;
        }

        /// <summary>
        /// Disables events raising (<see cref="AppearanceChanged"/>, <see cref="DimensionsChanged"/>, 
        /// <see cref="IntervalsChanged"/>, <see cref="VisibleRangeChanged"/>).
        /// </summary>
        /// <seealso cref="ChartAxis.Melt"/>
        /// <internalonly/>
        public void Freeze()
        {
            m_isFreezed = true;
        }

        /// <summary>
        /// Enables events raising (<see cref="AppearanceChanged"/>, <see cref="DimensionsChanged"/>, 
        /// <see cref="IntervalsChanged"/>, <see cref="VisibleRangeChanged"/>).
        /// </summary>
        /// <seealso cref="ChartAxis.Freeze"/>
        /// <internalonly/>
        public void Melt()
        {
            m_isFreezed = false;
        }

        /// <summary>
        /// Gets the visible value on the chart by specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public float GetVisibleValue(double value)
        {
            return (float)(m_length * this.ValueToCoeficient(value));
        }

        /// <summary>
        /// Calculates real value for given point.
        /// </summary>
        /// <param name="p">The point to calculate real value.</param>
        /// <returns>Real value.</returns>
        public double GetRealValue(PointF p)
        {
            return this.GetRealValue(m_orientation == ChartOrientation.Horizontal ?
                p.X - m_location.X : m_location.Y - p.Y);
        }

        /// <summary>
        /// Gets the real value in pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Calculated value.</returns>
        public double GetRealValue(float value)
        {
            return this.CoeficientToValue(value / m_length);
        }

        /// <summary>
        /// Gets the coordinate from value.
        /// </summary>
        /// <param name="value">The value to get coordinate.</param>
        /// <returns>Coordinate that represent value.</returns>
        public float GetCoordinateFromValue(double value)
        {
            if (m_orientation == ChartOrientation.Horizontal)
                return (float)(m_location.X + m_length * this.ValueToCoeficient(value));
            else
                return (float)(m_location.Y - m_length * this.ValueToCoeficient(value));
        }

        /// <summary>
        ///  Add breaks at the specified range.
        /// </summary>
        /// <param name="mm">Break's ><see cref="MinMaxInfo"/>.</param>
        [Obsolete("Use other AddBreak method or BreakInfo property.")
        , EditorBrowsable(EditorBrowsableState.Never)]
        public void AddBreak(MinMaxInfo mm)
        {
            m_breakRanges.Union(new DoubleRange(mm.Min, mm.Max));
        }

        /// <summary>
        /// Add breaks at the specified range.
        /// </summary>
        /// <param name="from">Value from.</param>
        /// <param name="to">Value to.</param>
        public void AddBreak(double from, double to)
        {
            m_breakRanges.Union(new DoubleRange(from, to));
        }

        /// <summary>
        ///  Clears all the breaks.
        /// </summary>
        public void ClearBreaks()
        {
            m_breakRanges.Clear();
        }

        /// <summary>
        /// Calculates the axis layout.
        /// </summary>
        /// <param name="rect">The <see cref="RectangleF"/> to place axis.</param>
        /// <param name="spacing">The spacing.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns><see cref="RectangleF"/> where axis is placed.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RectangleF CalculateAxis(RectangleF rect, SizeF spacing, float dimension)
        {
            RectangleF res = rect;
            spacing = new SizeF(spacing.Width + dimension, spacing.Height + dimension);

            m_needUpdateDimension = true;

            if (m_orientation == ChartOrientation.Horizontal)
            {
                if (m_locationType != ChartAxisLocationType.Set)
                {
                    float x = rect.Left;
                    if (LocationType == ChartAxisLocationType.AntiLabelCut)
                        x = Location.X;

                    if (m_opposedPosition)
                    {
                        Location = new PointF(x, rect.Top);
                        res = new RectangleF(rect.Left, rect.Top - spacing.Height,
                            rect.Width, rect.Height + spacing.Height);
                    }
                    else
                    {
                        Location = new PointF(x, rect.Bottom);
                        res = new RectangleF(rect.Left, rect.Top, rect.Width,
                            rect.Height + spacing.Height);
                    }
                    
                }

                if (m_autoSize)
                {                    
                    if (rect.Right - Location.X > 0)
                    {
                        m_length = rect.Right - Location.X;
                    }
                }
            }
            else
            {
                if (m_locationType != ChartAxisLocationType.Set)
                {
                    float y = rect.Bottom;
                    if (LocationType == ChartAxisLocationType.AntiLabelCut)
                        y = Location.Y;

                    if (m_opposedPosition)
                    {
                        Location = new PointF(rect.Right, y);
                        res = new RectangleF(rect.Left, rect.Top,
                            rect.Width + spacing.Width, rect.Height);
                    }
                    else
                    {
                        Location = new PointF(rect.Left, y);
                        res = new RectangleF(rect.Left - spacing.Width, rect.Top,
                            rect.Width + spacing.Width, rect.Height);
                    }
                }

                if (m_autoSize)
                {                    
                    if (Location.Y - rect.Top > 0)
                    {
                        m_length = Location.Y - rect.Top;
                    }
                }
            }

            m_scaleLablesCoef = m_scaleLables ? RealLength / m_scaleLength : 1f;

            return res;
        }

        /// <summary>
        /// Sets the zoom position from scroll bar value.
        /// </summary>
        /// <param name="scrollValue">The scroll value.</param>
        /// <param name="updateVisibleRange">Indicates that visible range need to be recalculated.</param>
        public void SetZoomPositionFromScrollBarValue(double scrollValue, bool updateVisibleRange)
        {
            if (m_orientation == ChartOrientation.Vertical)
            {
                scrollValue = 1 - m_zoomFactor - scrollValue;
            }

            if (m_inversed)
            {
                this.SetZooming(m_zoomFactor, 1 - m_zoomFactor - scrollValue);
            }
            else
            {
                this.SetZooming(m_zoomFactor, scrollValue);
            }
        }

        /// <summary>
        /// Calculates scroll bar position from zoom position.
        /// </summary>
        /// <returns>Calculated scroll bar position.</returns>
        public double GetScrollBarValueFromZoomPosition()
        {
            double ret = (Orientation == ChartOrientation.Vertical) ?
                (1 - m_zoomFactor) - m_zoomPosition : m_zoomPosition;

            if (!m_inversed)
            {
                return ret;
            }
            else
            {
                return (1 - m_zoomFactor) - ret;
            }
        }

        #region Drawing methods
        /// <summary>
        /// Draw grid on the <see cref="ChartArea"/>.
        /// </summary>
        /// <param name="graph">The <see cref="ChartGraph"/> to draw grid lines.</param>
        /// <param name="rect">The <see cref="RectangleF"/> to draw lines.</param>
        internal void DrawInterlacedGrid(ChartGraph graph, RectangleF rect)
        {
            if (m_interlacedGrid)
            {
                DoubleRange vRange = new DoubleRange(this.VisibleRange.min, this.VisibleRange.max);

                if (m_gridDrawMode == ChartAxisGridDrawingMode.Default)
                {
                    #region Draw the grid by tick labels
                    IEnumerable interlacedGridRects = this.GetInterlacedGridRects(rect);

                    foreach (RectangleF rc in interlacedGridRects)
                    {
                        graph.DrawRect(m_interlacedGridInterior, null, rc);
                    }
                    #endregion
                }
                else
                {
                    #region Draw the grid by grouping labels
                    bool isInterlaced = true;

                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    {
                        ChartAxisGroupingLabel cagl = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                        if ((cagl.Row == 0) && vRange.IsIntersects(cagl.Range))
                        {
                            if (isInterlaced)
                            {
                                float cs = this.GetCoordinateFromValue(cagl.Range.Start);
                                float ce = this.GetCoordinateFromValue(cagl.Range.End);

                                if (m_orientation == ChartOrientation.Horizontal)
                                {
                                    cs = ChartMath.MinMax(cs, rect.Left, rect.Right);
                                    ce = ChartMath.MinMax(ce, rect.Left, rect.Right);

                                    graph.DrawRect(m_interlacedGridInterior, null, Math.Min(cs, ce), rect.Top, Math.Abs(ce - cs), rect.Height);
                                }
                                else
                                {
                                    cs = ChartMath.MinMax(cs, rect.Top, rect.Bottom);
                                    ce = ChartMath.MinMax(ce, rect.Top, rect.Bottom);

                                    graph.DrawRect(m_interlacedGridInterior, null, rect.Left, Math.Min(cs, ce), rect.Width, Math.Abs(ce - cs));
                                }

                                isInterlaced = false;
                            }
                            else
                            {
                                isInterlaced = true;
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Draw grid on the <see cref="ChartArea"/>.
        /// </summary>
        /// <param name="graph">The <see cref="ChartGraph"/> to draw grid lines.</param>
        /// <param name="rect">The <see cref="RectangleF"/> to draw lines.</param>
        internal void DrawGridLines(ChartGraph graph, RectangleF rect)
        {
            MinMaxInfo vRange = this.VisibleRange;
            Pen bordersPen = m_gridLineType.Pen;

			SmoothingMode gSmoothingMode = graph.SmoothingMode;
            graph.SmoothingMode = SmoothingMode.None;
			
            if (this.Orientation == ChartOrientation.Horizontal)
            {
                graph.DrawLine(bordersPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                graph.DrawLine(bordersPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
            }
            else
            {
                graph.DrawLine(bordersPen, rect.Left, rect.Top, rect.Right, rect.Top);
                graph.DrawLine(bordersPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }

            if (m_drawMinorGrid && m_smallTicksPerInterval > 0)
            {
                if (this.GridDrawMode == ChartAxisGridDrawingMode.Default)
                {
                    #region Draw the minor grid by tick labels
                    if (m_drawMinorGrid && m_smallTicksPerInterval > 0)
                    {
                        IEnumerable smalTicks = this.GetSmallTicks();

                        foreach (float cor in smalTicks)
                        {
                            if (m_orientation == ChartOrientation.Vertical)
                            {
                                graph.DrawLine(m_minorGridLineType.Pen, rect.Left, cor, rect.Right, cor);
                            }
                            else
                            {
                                graph.DrawLine(m_minorGridLineType.Pen, cor, rect.Top, cor, rect.Bottom);
                            }
                        }
                    }
                    #endregion
                }
            }

            if (m_drawGrid)
            {
                if (this.GridDrawMode == ChartAxisGridDrawingMode.Default)
                {
                    #region Draw the grid by tick labels
                    for (int i = 0; i < m_visibleLables.Length; i++)
                    {
                        ChartAxisLabel label = m_visibleLables[i];
                        if(double.IsInfinity(label.DoubleValue))
                        {
                            throw new ArgumentException("Invalid Value type.");
                        }
                        double value = label.IsAuto ? label.DoubleValue - m_pointOffset : label.DoubleValue;
                        float pos = this.GetCoordinateFromValue(value);

                        if (m_orientation == ChartOrientation.Vertical)
                        {
                            graph.DrawLine(m_gridLineType.Pen, rect.Left, pos, rect.Right, pos);
                        }
                        else
                        {
                            graph.DrawLine(m_gridLineType.Pen, pos, rect.Top, pos, rect.Bottom);
                        }
                    }

                    if (m_customOrigin && vRange.Contains(m_origin))
                    {
                        float pos = this.GetCoordinateFromValue(m_origin);

                        if (m_orientation == ChartOrientation.Vertical)
                        {
                            graph.DrawLine(m_gridLineType.Pen, rect.Left, pos, rect.Right, pos);
                        }
                        else
                        {
                            graph.DrawLine(m_gridLineType.Pen, pos, rect.Top, pos, rect.Bottom);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Draw the grid by grouping labels
                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    {
                        ChartAxisGroupingLabel cagl = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                        if ((cagl.Row == 0) && vRange.Contains(cagl.Range.Start) && vRange.Contains(cagl.Range.End))
                        {
                            float cs = this.GetCoordinateFromValue(cagl.Range.Start);
                            float ce = this.GetCoordinateFromValue(cagl.Range.End);

                            if (Orientation == ChartOrientation.Horizontal)
                            {
                                graph.DrawLine(this.GridLineType.Pen, cs, rect.Top, cs, rect.Bottom);
                                graph.DrawLine(this.GridLineType.Pen, ce, rect.Top, ce, rect.Bottom);
                            }
                            else
                            {
                                graph.DrawLine(this.GridLineType.Pen, rect.Left, cs, rect.Right, cs);
                                graph.DrawLine(this.GridLineType.Pen, rect.Left, ce, rect.Right, ce);
                            }
                        }
                    }
                    #endregion
                }
            }
			graph.SmoothingMode = gSmoothingMode;
        }

        /// <summary>
        /// Draws the interlaced grid.
        /// </summary>
        /// <param name="g">The <see cref="Graphics3D"/> to render gris lines.</param>
        /// <param name="rect">The <see cref="RectangleF"/> to draw lines.</param>
        /// <param name="z">The z-coord.</param>
        /// <returns>
        /// 	<see cref="Polygon"/> that represent grid lines.
        /// </returns>
        internal Polygon DrawInterlacedGrid(Graphics3D g, RectangleF rect, float z)
        {
            if (m_interlacedGrid)
            {
                GraphicsPath gp = new GraphicsPath();
                DoubleRange vRange = new DoubleRange(this.VisibleRange.min, this.VisibleRange.max);

                if (m_gridDrawMode == ChartAxisGridDrawingMode.Default)
                {
                    #region Draw the grid by tick labels
                    IEnumerable interlacedGridRects = this.GetInterlacedGridRects(rect);

                    foreach (RectangleF rc in interlacedGridRects)
                    {
                        gp.AddRectangle(rc);
                    }
                    #endregion
                }
                else
                {
                    #region Draw the grid by grouping labels
                    bool isInterlaced = true;

                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    {
                        ChartAxisGroupingLabel cagl = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                        if ((cagl.Row == 0) && vRange.IsIntersects(cagl.Range))
                        {
                            if (isInterlaced)
                            {
                                float cs = this.GetCoordinateFromValue(cagl.Range.Start);
                                float ce = this.GetCoordinateFromValue(cagl.Range.End);

                                if (m_orientation == ChartOrientation.Horizontal)
                                {
                                    cs = ChartMath.MinMax(cs, rect.Left, rect.Right);
                                    ce = ChartMath.MinMax(ce, rect.Left, rect.Right);

                                    gp.AddRectangle(new RectangleF(Math.Min(cs, ce), rect.Top, Math.Abs(ce - cs), rect.Height));
                                }
                                else
                                {
                                    cs = ChartMath.MinMax(cs, rect.Top, rect.Bottom);
                                    ce = ChartMath.MinMax(ce, rect.Top, rect.Bottom);

                                    gp.AddRectangle(new RectangleF(rect.Left, Math.Min(cs, ce), rect.Width, Math.Abs(ce - cs)));
                                }

                                gp.CloseFigure();
                                isInterlaced = false;
                            }
                            else
                            {
                                isInterlaced = true;
                            }
                        }
                    }
                    #endregion
                }

                return Path3D.FromGraphicsPath(gp, z, m_interlacedGridInterior, null);
            }

            return null;
        }

        /// <summary>
        /// Draws grid's lines.
        /// </summary>
        /// <param name="g">The <see cref="Graphics3D"/> to render gris lines.</param>
        /// <param name="rect">The <see cref="RectangleF"/> to draw lines.</param>
        /// <param name="z">The z-coord.</param>
        /// <returns>
        /// 	<see cref="Polygon"/> that represent grid lines.
        /// </returns>
        internal Polygon DrawGridLines(Graphics3D g, RectangleF rect, float z)
        {
            GraphicsPath gp = new GraphicsPath();
            GraphicsPath gpM = new GraphicsPath();
            MinMaxInfo vRange = this.VisibleRange;

            if (m_drawMinorGrid && m_smallTicksPerInterval > 0)
            {
                if (this.GridDrawMode == ChartAxisGridDrawingMode.Default)
                {
                    #region Draw the minor grid by tick labels
                    if (m_drawMinorGrid && m_smallTicksPerInterval > 0)
                    {
                        IEnumerable smalTicks = this.GetSmallTicks();

                        foreach (float cor in smalTicks)
                        {
                            if (m_orientation == ChartOrientation.Vertical)
                            {
                                gpM.AddLine(rect.Left, cor, rect.Right, cor);
                                gpM.CloseFigure();
                            }
                            else
                            {
                                gpM.AddLine(cor, rect.Top, cor, rect.Bottom);
                                gpM.CloseFigure();
                            }
                        }
                    }
                    #endregion
                }
            }

            if (m_gridDrawMode == ChartAxisGridDrawingMode.Default)
            {
                #region Draw the grid by tick labels
                for (int i = 0; i < m_visibleLables.Length; i++)
                {
                    ChartAxisLabel label = m_visibleLables[i];
                    double value = label.IsAuto ? label.DoubleValue - m_pointOffset : label.DoubleValue;
                    float pos = this.GetCoordinateFromValue(value);

                    if (m_orientation == ChartOrientation.Vertical)
                    {
                        gp.AddLine(rect.Left, pos, rect.Right, pos);
                        gp.CloseFigure();
                    }
                    else
                    {
                        gp.AddLine(pos, rect.Top, pos, rect.Bottom);
                        gp.CloseFigure();
                    }
                }

                if (m_customOrigin && vRange.Contains(m_origin))
                {
                    float pos = this.GetCoordinateFromValue(m_origin);

                    if (m_orientation == ChartOrientation.Vertical)
                    {
                        gp.AddLine(rect.Left, pos, rect.Right, pos);
                        gp.CloseFigure();
                    }
                    else
                    {
                        gp.AddLine(pos, rect.Top, pos, rect.Bottom);
                        gp.CloseFigure();
                    }
                }
                #endregion
            }
            else
            {
                #region Draw the grid by grouping labels
                for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                {
                    ChartAxisGroupingLabel cagl = this.GroupingLabelsImpl.GetGroupingLabelAt(i);

                    if ((cagl.Row == 0) && vRange.Contains(cagl.Range.Start) && vRange.Contains(cagl.Range.End))
                    {
                        float cs = this.GetCoordinateFromValue(cagl.Range.Start);
                        float ce = this.GetCoordinateFromValue(cagl.Range.End);

                        if (Orientation == ChartOrientation.Horizontal)
                        {
                            gp.AddLine(cs, rect.Top, cs, rect.Bottom);
                            gp.CloseFigure();
                            gp.AddLine(ce, rect.Top, ce, rect.Bottom);
                            gp.CloseFigure();
                        }
                        else
                        {
                            gp.AddLine(rect.Left, cs, rect.Right, cs);
                            gp.CloseFigure();
                            gp.AddLine(rect.Left, ce, rect.Right, ce);
                            gp.CloseFigure();
                        }
                    }
                }
                #endregion
            }

            if (gpM.PointCount > 0)
            {
                Path3DCollect res = new Path3DCollect(Path3D.FromGraphicsPath(gp, z, m_gridLineType.Pen));
                res.Add(Path3D.FromGraphicsPath(gpM, z, m_minorGridLineType.Pen));
                return res;
            }

            return Path3D.FromGraphicsPath(gp, z, m_gridLineType.Pen);
        }

        /// <summary>
        /// Draw breaks.
        /// </summary>
        /// <param name="g"><see cref="Graphics"/> to render breaks.</param>
        /// <param name="rect"><see cref="RectangleF"/> to render breaks.</param>
        internal void DrawBreaks(Graphics g, RectangleF rect)
        {
            if (this.BreaksEnabled)
            {
                double pos = 0;
                bool isVer = m_orientation == ChartOrientation.Vertical;

                if (m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
                {
                    for (int i = 0, ci = m_breakRanges.Segments.Count - 1; i < ci; i++)
                    {
                        ChartAxisSegment axisSegment = m_breakRanges.Segments[i] as ChartAxisSegment;
                        pos += axisSegment.Length;

                        float f = (float)(pos * m_length);

                        PointF pt1 = isVer ? new PointF(rect.Left, this.Location.Y - f) : new PointF(this.Location.X + f, rect.Top);
                        PointF pt2 = isVer ? new PointF(rect.Right, this.Location.Y - f) : new PointF(this.Location.X + f, rect.Bottom);

                        m_breakInfo.DrawBreakLine(g, pt1, pt2);
                    }
                }
                else if (m_breakRanges.BreaksMode == ChartBreaksMode.Manual)
                {
                    foreach (DoubleRange brkRange in m_breakRanges.Breaks)
                    {
                        if (VisibleRange.Contains(brkRange.End))
                        {
                            float f = this.GetCoordinateFromValue(brkRange.End);

                            PointF pt1 = isVer ? new PointF(rect.Left, f) : new PointF(f, rect.Top);
                            PointF pt2 = isVer ? new PointF(rect.Right, f) : new PointF(f, rect.Bottom);

                            m_breakInfo.DrawBreakLine(g, pt1, pt2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw breaks.
        /// </summary>
        /// <param name="g"><see cref="Graphics3D"/> to render breaks.</param>
        /// <param name="rect"><see cref="System.Drawing.RectangleF"/> to render breaks.</param>
        internal void DrawBreaks(Graphics3D g, RectangleF rect)
        {
            if (this.BreaksEnabled)
            {
                double pos = 0;
                bool isVer = m_orientation == ChartOrientation.Vertical;

                if (m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
                {
                    for (int i = 0, ci = m_breakRanges.Segments.Count - 1; i < ci; i++)
                    {
                        ChartAxisSegment axisSegment = m_breakRanges.Segments[i] as ChartAxisSegment;
                        pos += axisSegment.Length;

                        float f = (float)(pos * m_length);

                        PointF pt1 = isVer ? new PointF(rect.Left, rect.Bottom - f) : new PointF(rect.Left + f, rect.Top);
                        PointF pt2 = isVer ? new PointF(rect.Right, rect.Bottom - f) : new PointF(rect.Left + f, rect.Bottom);

                        m_breakInfo.DrawBreakLine(g, pt1, pt2);
                    }
                }
                else if (m_breakRanges.BreaksMode == ChartBreaksMode.Manual)
                {
                    foreach (DoubleRange brkRange in m_breakRanges.Breaks)
                    {
                        if (VisibleRange.Contains(brkRange.End))
                        {
                            float f = this.GetCoordinateFromValue(brkRange.End);

                            PointF pt1 = isVer ? new PointF(rect.Left, f) : new PointF(f, rect.Top);
                            PointF pt2 = isVer ? new PointF(rect.Right, f) : new PointF(f, rect.Bottom);

                            m_breakInfo.DrawBreakLine(g, pt1, pt2);
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Subtructs location of axis and returns necessary value by orientation.
        /// </summary>
        /// <param name="pt"><see cref="System.Drawing.PointF"/> to convert.</param>
        /// <returns>Calculated value by orientation.</returns>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public float PointToVisibleValue(PointF pt)
        {
            bool isHor = m_orientation == ChartOrientation.Horizontal;
            bool isSub = isHor ? m_inversed : !m_inversed;
            float pos = isHor ? pt.X : pt.Y;
            float loc = isHor ? Location.X : Location.Y - m_length;

            return isSub ? (RealLength - pos + loc) : (pos - loc);
        }

        /// <summary>
        /// Values to coefficient.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double ValueToCoeficient(double value)
        {
            MinMaxInfo vRange = this.VisibleRange;
            DoubleRange vdRange = new DoubleRange(vRange.min, vRange.max);

            if (vdRange.Delta == 0)
            {
                value = 0;
            }
            else
            {
                if (m_valueType == ChartValueType.Logarithmic)
                {
                    if (value <= 0)
                    {
                        value = Math.Log(double.Epsilon, m_logBase);
                    }
                    else
                    {
                        value = Math.Log(value, m_logBase);
                    }
                }

                if (this.BreaksEnabled)
                {
                    value = m_breakRanges.ValueToCoeficient(value);
                }
                else
                {
                    value = vdRange.Extrapolate(value);
                }
            }

            return m_inversed ? 1d - value : value;
        }

        /// <summary>
        /// Coeficients to value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double CoeficientToValue(double value)
        {
            MinMaxInfo vRange = this.VisibleRange;

            if (vRange.Delta == 0)
            {
                value = 0d;
            }
            else
            {
                DoubleRange vdRange = new DoubleRange(vRange.min, vRange.max);

                value = m_inversed ? 1d - value : value;

                if (this.BreaksEnabled)
                {
                    value = m_breakRanges.CoeficientToValue(value);
                }
                else
                {
                    value = vdRange.Interpolate(value);
                }

                if (m_valueType == ChartValueType.Logarithmic)
                {
                    value = Math.Pow(m_logBase, value);
                }
            }

            return value;
        }
        #endregion

        #region Nice range calculation
        /// <summary>
        /// Calculates the nice range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="paddingType">Type of the padding.</param>
        internal void CalculateNiceRange(ref DoubleRange range, ref double interval, ChartAxisRangePaddingType paddingType)
        {
            interval = this.CalculateNiceInternal(interval);
            range = this.CalculatePadding(range, interval, paddingType);
            //range = this.TweakToZero(range, inter0val);
        }

        /// <summary>
        /// Calculates the nice internal.
        /// </summary>
        /// <param name="desiredInterval">The desired interval.</param>
        /// <returns></returns>
        private double CalculateNiceInternal(double desiredInterval)
        {
            double mul = Math.Pow(10d, Math.Floor(Math.Log10(desiredInterval)));
            double intervalDiv = desiredInterval / mul;

            foreach (double div in c_intervalDivs)
            {
                if (intervalDiv < div)
                {
                    return div * mul;
                }
            }

            return desiredInterval;
        }

        /// <summary>
        /// Calculates the nice internal.
        /// </summary>
        /// <param name="desiredInterval">The desired interval.</param>
        /// <returns></returns>
        private double CalculateNiceInternalEx(double desiredInterval)
        {
            double mul = Math.Pow(10d, Math.Floor(Math.Log10(desiredInterval)));
            double intervalDiv = desiredInterval / mul;
            double minDelta = double.MaxValue;

            foreach (double div in c_intervalDivs)
            {
                double delta = Math.Abs(div - intervalDiv);

                if (delta < minDelta)
                {
                    minDelta = delta;
                    desiredInterval = div * mul;
                }
            }

            return desiredInterval;
        }

        /// <summary>
        /// Calculates the padding.
        /// </summary>
        /// <param name="baseRange">The base range.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="paddingType">Type of the padding.</param>
        /// <returns></returns>
        private DoubleRange CalculatePadding(DoubleRange baseRange,double interval, ChartAxisRangePaddingType paddingType)
        {
            bool checkZero = m_forceZero || m_preferZero;

            double start = ChartMath.Round(baseRange.Start, interval, false);
            double end = ChartMath.Round(baseRange.End, interval, true);

            if (paddingType == ChartAxisRangePaddingType.Calculate)
            {
                if (this.m_margin)
                {
                    if (baseRange.Start - start < interval / 2)
                    {
                        if (!checkZero || start != 0)
                        {
                            start -= interval ;
                        }
                    }
                    if (end - baseRange.End < interval / 2)
                    {
                        if (!checkZero || end != 0)
                        {
                           
                            end += interval ;
                        }
                    }
                }

                if (this.ForceZero && start != 0)
                {
                    start = 0;
                }

                if (this.ForceZeroToDouble && end < 0)
                {
                    end = 0;
                }
                if (this.ForceZeroToDouble && end > 0)
                {
                    start = 0;
                }

              
            }

            return new DoubleRange(start, end);
        }

        /// <summary>
        /// Tweaks to zero.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        private DoubleRange TweakToZero(DoubleRange range, double interval)
        {
            if (m_forceZero)
            {
                range = DoubleRange.Union(range, 0);
            }
            else if (m_preferZero)
            {
                DoubleRange minRange = new DoubleRange(0, interval / 2);
                DoubleRange maxRange = new DoubleRange(0, -interval / 2);

                if (minRange.Inside(range.Start) || maxRange.Inside(range.End))
                {
                    range = DoubleRange.Union(range, 0);
                }
            }

            return range;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Raises the appearance changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnAppearanceChanged(EventArgs e)
        {
            if (!m_isFreezed && this.AppearanceChanged != null)
            {
                this.AppearanceChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the dimensions changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnDimensionsChanged(EventArgs e)
        {
            m_needUpdateVisibleLables = true;

            if (!m_isFreezed && this.DimensionsChanged != null)
            {
                this.DimensionsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the intervals changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnIntervalsChanged(EventArgs e)
        {
            m_needUpdateVisibleLables = true;

            if (!m_isFreezed && this.IntervalsChanged != null)
            {
                this.IntervalsChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the visible range changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnVisibleRangeChanged(EventArgs e)
        {
            m_needUpdateVisibleLables = true;

            if (!m_isFreezed && this.VisibleRangeChanged != null)
            {
                this.VisibleRangeChanged(this, e);
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Gets the offset label.
        /// </summary>
        /// <param name="sz">The sz.</param>
        /// <returns></returns>
        internal float GetOffsetLabel(float sz)
        {
            float res = 0;

            if (m_labelAligment == StringAlignment.Far)
            {
                res = sz / 2;
            }
            else if (m_labelAligment == StringAlignment.Near)
            {
                res = -sz / 2;
            }

            return res;
        }

        /// <summary>
        /// Sets the default orientation.
        /// </summary>
        /// <param name="value">The value to set orientation.</param>
        internal void SetDefaultOrientation(ChartOrientation value)
        {
            m_defaultAxisOrienation = value;
        }
        #endregion

        #region Helper methdos

        #region Layouting
        /// <summary>
        /// Perfoms the tick labels layout.
        /// </summary>
        /// <param name="g">The graphics content.</param>
        /// <param name="labelsBounds">The labels bounds.</param>
        /// <param name="spacing">The spacing.</param>
        /// <param name="chartarea"></param>
        /// <returns></returns>
        private float DoTickLabelsLayout(Graphics g, RectangleF labelsBounds, float spacing,ChartArea chartarea)
        {
            ChartAxisLabel[] visibleLabels = this.VisibleLabels;
            int oppcoef;
            if (this.AxisLabelPlacement == ChartPlacement.Inside)
              oppcoef = m_opposedPosition ? 1 : -1;
            else
              oppcoef = m_opposedPosition ? -1 : 1;
			  
            float dimension = 0;

            if (m_orientation == ChartOrientation.Horizontal)
            {
                float tx = labelsBounds.X;
                float ty;             
                if (this.AxisLabelPlacement == ChartPlacement.Inside)
                    ty = m_opposedPosition ? labelsBounds.Top + spacing : labelsBounds.Bottom - spacing;
                else
                    ty = m_opposedPosition ? labelsBounds.Bottom - spacing : labelsBounds.Top + spacing;               
                float length = labelsBounds.Width;
               
                ContentAlignment cntAlignment = ContentAlignment.MiddleCenter;

                #region Set Content Alignment

				if (this.AxisLabelPlacement == ChartPlacement.Inside)
				{
				switch (m_labelAligment)
                    {
                        case StringAlignment.Center:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.BottomCenter : ContentAlignment.TopCenter;
                            break;
                        case StringAlignment.Far:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.BottomRight : ContentAlignment.TopRight;
                            break;
                        case StringAlignment.Near:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.BottomLeft : ContentAlignment.TopLeft;
                            break;
                    }
                }
				else
				{
				switch (m_labelAligment)
                {
                    case StringAlignment.Center:
                        cntAlignment = m_opposedPosition ?
                            ContentAlignment.TopCenter : ContentAlignment.BottomCenter;
                        break;
                    case StringAlignment.Far:
                        cntAlignment = m_opposedPosition ?
                            ContentAlignment.TopRight : ContentAlignment.BottomRight;
                        break;
                    case StringAlignment.Near:
                        cntAlignment = m_opposedPosition ?
                            ContentAlignment.TopLeft : ContentAlignment.BottomLeft;
                        break;
                }
				}
                #endregion

                switch (m_intersectAction)
                {
                    case ChartLabelIntersectAction.None:
                        #region ChartLabelIntersectAction.None layout
                        {
                            float maxH = 0;

                            for (int i = 0; i < visibleLabels.Length; i++)
                            {
                                ChartAxisLabel cal = visibleLabels[i];
                                RectangleF bounds = RectangleF.Empty;
                                float x = tx + (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                                PointF connectPoint = this.GetMarginsPoint(g, new PointF(x, ty), cal.Measure(g, this));

                                if (m_labelRotate)
                                {
                                    bounds = cal.Arrange(connectPoint, cntAlignment, m_labelRotateAngle, m_rotateFromTicks);
                                }
                                else
                                {
                                    bounds = cal.Arrange(connectPoint, cntAlignment);
                                }

                                maxH = Math.Max(maxH, bounds.Height);
                            }

                            dimension += maxH;
                        }
                        #endregion
                        break;

                    case ChartLabelIntersectAction.Wrap:
                        #region ChartLabelIntersectAction.Wrap layout
                        {
                            float maxH = 0;
                            PointF[] wrapPoss = new PointF[visibleLabels.Length];

                            for (int i = 0; i < visibleLabels.Length; i++)
                            {
                                ChartAxisLabel cal = visibleLabels[i];
                                float x = tx + (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                                wrapPoss[i] = new PointF(x, ty);
                                cal.Measure(g, this);
                            }

                            for (int i = 0, c = visibleLabels.Length; i < c; i++)
                            {
                                float width = float.MaxValue;
                                ChartAxisLabel cal = visibleLabels[i];

                                if (i > 0)
                                    width = Math.Min(width, wrapPoss[i].X - wrapPoss[i - 1].X);
                                if (i < c - 1)
                                    width = Math.Min(width, wrapPoss[i + 1].X - wrapPoss[i].X);

                                cal.Measure(g, width, this);

                                maxH = Math.Max(maxH, cal.Arrange(wrapPoss[i], cntAlignment).Height);
                            }

                            dimension += maxH;
                        }
                        #endregion
                        break;

                    case ChartLabelIntersectAction.MultipleRows:
                        #region ChartLabelIntersectAction.MultipleRows layout
                        {
                            int[] rows = new int[visibleLabels.Length];
                            float[] lastX = new float[c_rowsCount] { float.MinValue, float.MinValue, float.MinValue, float.MinValue };
                            float[] maxY = new float[c_rowsCount + 1] { 0, 0, 0, 0, 0 };

                            for (int i = 0; i < visibleLabels.Length; i++)
                            {
                                int row = 0;
                                float ms = float.MaxValue;
                                ChartAxisLabel cal = visibleLabels[i];
                                float x = tx + (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                                PointF connectPoint = GetMarginsPoint(g, new PointF(x, ty), cal.Measure(g, this));
                                RectangleF bounds = cal.Arrange(connectPoint, cntAlignment);

                                for (int j = 0; j < c_rowsCount; j++)
                                {
                                    if (lastX[j] < bounds.Left)
                                    {
                                        row = j;
                                        break;
                                    }
                                    else if (lastX[j] - bounds.Left < ms)
                                    {
                                        ms = lastX[row] - bounds.Left;
                                        row = j;
                                    }
                                }

                                lastX[row] = bounds.Right;
                                maxY[row + 1] = Math.Max(maxY[row + 1], bounds.Height);
                                rows[i] = row;
                            }

                            for (int i = 1; i < maxY.Length; i++)
                            {
                                maxY[i] += maxY[i - 1];
                            }

                            for (int i = 0; i < visibleLabels.Length; i++)
                            {
                                ChartAxisLabel cal = visibleLabels[i];
                                PointF loc = cal.Bounds.Location;

                                loc.Y += oppcoef * maxY[rows[i]];

                                cal.Arrange(loc);
                            }

                            dimension += maxY[c_rowsCount];
                        }
                        #endregion
                        break;

                    case ChartLabelIntersectAction.Rotate:
                        #region ChartLabelIntersectAction.Rotate layout
                        {
                            float maxH = 0;
                            float minW = float.MaxValue;
                            float maxW = 0;
                            float lastX = 0;
                            PointF[] wrapPoss = new PointF[visibleLabels.Length];

                            for (int i = 0; i < visibleLabels.Length; i++)
                            {
                                ChartAxisLabel cal = visibleLabels[i];
                                float x = tx + (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                                PointF connectPoint = GetMarginsPoint(g, new PointF(x, ty), cal.Measure(g, this));
                                RectangleF bounds = cal.Arrange(connectPoint, cntAlignment);
                                wrapPoss[i] = new PointF(x, ty);
                                if ((i != 0) && (bounds.Left - lastX < 0))
                                {
                                    minW = Math.Min(minW, bounds.Right - lastX);
                                }

                                lastX = bounds.Right;
                                maxH = Math.Max(maxH, bounds.Height);
                                maxW = Math.Max(maxW, bounds.Width);
                           
                            }

                            if (minW != float.MaxValue)
                            {
                                float angle = (float)(ChartMath.ToDegrees * Math.Atan(maxH / minW));

                                angle = oppcoef * (angle < 0 ? 90 : angle);

                                if (angle != 0)
                                {
                                    for (int i = 0; i < visibleLabels.Length; i++)
                                    {
                                        ChartAxisLabel cal = visibleLabels[i];
                                        PointF connectPoint = new PointF(tx + GetVisibleValue(cal.DoubleValue), ty);
                                        RectangleF bounds = cal.Arrange(connectPoint, cntAlignment, angle, m_rotateFromTicks);
                                        maxH = Math.Max(maxH, bounds.Height);
                                        
                                       
                                        
                                    }
                                    
                                    if (maxH > chartarea.Height/2)
                                    {
                                        float width = maxH = chartarea.Height / 2;
                                         float angle1 = (float)(ChartMath.ToDegrees * Math.Atan(maxH / minW));

                                         angle1 = oppcoef * (angle < 0 ? 90 : angle);

                                        if (angle1 != 0)
                                        {
                                             for (int i = 0, c = visibleLabels.Length; i < c; i++)
                                            {
                                               ChartAxisLabel cal = visibleLabels[i];
                                              cal.Measure(g, width, this);
                                              maxH = Math.Max(maxH, cal.Arrange(wrapPoss[i], cntAlignment, angle1, m_rotateFromTicks).Height);
                                            }
                                        }
                     
                                    }
                                  
                                }
                            }

                            dimension += maxH;
                        }
                        #endregion
                        break;
                }
            }
            else
            {
               float tx ; 
                if (this.AxisLabelPlacement == ChartPlacement.Inside)
                    tx = m_opposedPosition ? labelsBounds.Right - spacing : labelsBounds.Left + spacing;
                else
                    tx = m_opposedPosition ? labelsBounds.Left + spacing : labelsBounds.Right - spacing;
                float ty = labelsBounds.Bottom;
                float length = labelsBounds.Height;
                
                ContentAlignment cntAlignment = ContentAlignment.MiddleCenter;

                #region Set Content Alignment

                if (this.AxisLabelPlacement == ChartPlacement.Inside)
                {
                    switch (m_labelAligment)
                    {
                        case StringAlignment.Center:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
                            break;
                        case StringAlignment.Far:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.BottomLeft : ContentAlignment.BottomRight;
                            break;
                        case StringAlignment.Near:
                            cntAlignment = m_opposedPosition ?
                               ContentAlignment.TopLeft : ContentAlignment.TopRight;
                            break;
                    }
                }
                else
                {
                    switch (m_labelAligment)
                    {
                        case StringAlignment.Center:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                            break;
                        case StringAlignment.Far:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.BottomRight : ContentAlignment.BottomLeft;
                            break;
                        case StringAlignment.Near:
                            cntAlignment = m_opposedPosition ?
                                ContentAlignment.TopRight : ContentAlignment.TopLeft;
                            break;
                    }
                }
                #endregion

                #region ChartLabelIntersectAction.None layout
                float maxW = 0;

                for (int i = 0; i < visibleLabels.Length; i++)
                {
                    ChartAxisLabel cal = visibleLabels[i];
                    RectangleF bounds = RectangleF.Empty;
                    float y = ty - (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                    PointF connectPoint = GetMarginsPoint(g, new PointF(tx, y), cal.Measure(g, this));

                    if (m_labelRotate)
                    {
                        bounds = cal.Arrange(connectPoint, cntAlignment, m_labelRotateAngle, m_rotateFromTicks);
                    }
                    else
                    {
                        bounds = cal.Arrange(connectPoint, cntAlignment);
                    }

                    maxW = Math.Max(maxW, bounds.Width);
                }
                if (maxW > chartarea.Width / 2)
                {
                    float width = chartarea.Width / 2;
                    for (int i = 0; i < visibleLabels.Length; i++)
                    {
                        ChartAxisLabel cal = visibleLabels[i];
                        RectangleF bounds = RectangleF.Empty;
                        float y = ty - (float)(length * this.ValueToCoeficient(cal.DoubleValue));
                        PointF connectPoint = GetMarginsPoint(g, new PointF(tx, y), cal.Measure(g,width, this));
                      

                        if (m_labelRotate)
                        {
                            bounds = cal.Arrange(connectPoint, cntAlignment, m_labelRotateAngle, m_rotateFromTicks);
                        }
                        else
                        {
                            bounds = cal.Arrange(connectPoint, cntAlignment);
                        }
                        cal.Measure(g, width, this);
                        //maxW = Math.Max(maxW, bounds.Width);

                    }
                maxW = chartarea.Width / 2;
                }
                dimension += maxW;
                #endregion
            }

            if (m_drawTickLabelGrid)
            {
                dimension += m_tickLabelGridPadding;
            }

            return dimension;
        }

        /// <summary>
        /// Updates grouping labels layout.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/>.</param>
        /// <param name="spacing">The spacing.</param>
        /// <returns>Spacing.</returns>
        private float DoGroupingLabelsLayout(Graphics g, float spacing)
        {
            float dimension = spacing;

            if (m_orientation == ChartOrientation.Horizontal)
            {
                for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    this.GroupingLabelsImpl.GetGroupingLabelAt(i).GridDimension = 0;

                for (int row = 0; row < this.groupingLabelsRowsDimensions.Length; row++)
                {
                    float maxGroupLabHeight = 0;

                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    {
                        if (row == this.GroupingLabelsImpl.GetGroupingLabelAt(i).Row)
                        {
                            float h = this.GroupingLabelsImpl.GetGroupingLabelAt(i).GetSize(g, this).Height;
                            if (maxGroupLabHeight < h)
                                maxGroupLabHeight = h;
                        }
                    }

                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                        if (row == this.GroupingLabelsImpl.GetGroupingLabelAt(i).Row)
                            this.GroupingLabelsImpl.GetGroupingLabelAt(i).GridDimension = maxGroupLabHeight;

                    dimension += maxGroupLabHeight;

                    this.groupingLabelsRowsDimensions[row] = dimension;
                }
            }
            else
            {
                for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    this.GroupingLabelsImpl.GetGroupingLabelAt(i).GridDimension = 0;

                for (int row = 0; row < this.groupingLabelsRowsDimensions.Length; row++)
                {
                    float maxGroupLabWidth = 0;
                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                    {
                        if (row == this.GroupingLabelsImpl.GetGroupingLabelAt(i).Row)
                        {
                            float w = this.GroupingLabelsImpl.GetGroupingLabelAt(i).GetSize(g, this).Width;
                            if (maxGroupLabWidth < w)
                                maxGroupLabWidth = w;
                        }
                    }

                    for (int i = 0; i < this.GroupingLabelsImpl.Count; i++)
                        if (row == this.GroupingLabelsImpl.GetGroupingLabelAt(i).Row)
                            this.GroupingLabelsImpl.GetGroupingLabelAt(i).GridDimension = maxGroupLabWidth;
                    dimension += maxGroupLabWidth;
                    this.groupingLabelsRowsDimensions[row] = dimension;
                }
            }

            return dimension - spacing;
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draws the tick labels.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to draw.</param>
        /// <param name="chartArea"></param>
        private void DrawTickLabels(Graphics g,ChartArea chartArea)
        {
            RectangleF rect = this.Rect;

            for (int i = 0; i < m_visibleLables.Length; i++)
            {
                ChartAxisLabel cal = m_visibleLables[i];

                if (m_hidePartialLabels)
                {
                    if (m_orientation == ChartOrientation.Horizontal)
                    {
                        if (cal.Bounds.Left < rect.Left || cal.Bounds.Right > rect.Right)
                        {
                            cal = null;
                        }
                    }
                    else
                    {
                        if (cal.Bounds.Top < rect.Top || cal.Bounds.Bottom > rect.Bottom)
                        {
                            cal = null;
                        }
                    }
                }

                if (cal != null)
                {
                    cal.Draw(g, this,chartArea);
                }
            }
        }

        /// <summary>
        /// Draws all tick labels.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to draw.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="z">The z-coord.</param>
        /// <param name="chartArea"></param>
        private void DrawTickLabels(Graphics3D g, IList paths, float z,ChartArea chartArea)
        {
            RectangleF rect = this.Rect;

            for (int i = 0; i < m_visibleLables.Length; i++)
            {
                ChartAxisLabel cal = m_visibleLables[i];

                if (m_hidePartialLabels)
                {
                    if (m_orientation == ChartOrientation.Horizontal)
                    {
                        if (cal.Bounds.Left < rect.Left || cal.Bounds.Right > rect.Right)
                        {
                            cal = null;
                        }
                    }
                    else
                    {
                        if (cal.Bounds.Top < rect.Top || cal.Bounds.Bottom > rect.Bottom)
                        {
                            cal = null;
                        }
                    }
                }

                if (cal != null)
                {
                    Path3D path3D = cal.Draw3D(g, this, z,chartArea);

                    if (path3D != null)
                    {
                        paths.Add(path3D);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the tick label grids.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to draw.</param>
        /// <param name="pen">The <see cref="System.Drawing.Pen"/> to draw tick.</param>
        /// <param name="rect">The <see cref="System.Drawing.RectangleF"/> to place tick.</param>
        private void DrawTickLabelGrids(Graphics g, Pen pen, RectangleF rect)
        {
            RectangleF clipRect = rect;
            clipRect.Inflate(1, 1);

            if (m_orientation == ChartOrientation.Horizontal)
            {
                float hGrid = m_ticksAndLabelsDimension;
                float yGrid = m_opposedPosition ? rect.Bottom - hGrid : rect.Top;

                for (int i = 0, c = m_visibleLables.Length; i < c; i++)
                {
                    double prev = (i > 0 ? m_visibleLables[i - 1].DoubleValue : double.NaN);
                    double nextv = (i < c - 1 ? m_visibleLables[i + 1].DoubleValue : double.NaN);
                    double currv = m_visibleLables[i].DoubleValue;

                    if (double.IsNaN(prev) && double.IsNaN(nextv))
                    {
                        prev = VisibleRange.Min;
                        nextv = VisibleRange.Max;
                    }
                    else if (double.IsNaN(prev))
                    {
                        prev = 2 * currv - nextv;
                    }
                    else if (double.IsNaN(nextv))
                    {
                        nextv = 2 * currv - prev;
                    }

                    if (VisibleRange.Intersects(new MinMaxInfo(prev, nextv, 1)))
                    {
                        float pre = GetCoordinateFromValue((prev + currv) / 2);
                        float next = GetCoordinateFromValue((nextv + currv) / 2);

                        RectangleF gridCell = new RectangleF(Math.Min(pre, next), yGrid, Math.Abs(pre - next), hGrid);
                        GraphicsContainer gContainer = DrawingHelper.BeginTransform(g);

                        g.SetClip(clipRect);

                        DrawingHelper.DrawRectangleF(g, pen, gridCell);
                        DrawingHelper.EndTransform(g, gContainer);
                    }
                }
            }
            else
            {
                float wGrid = m_ticksAndLabelsDimension;
                float xGrid = m_opposedPosition ? rect.Left : rect.Right - wGrid;

                for (int i = 0, c = m_visibleLables.Length; i < c; i++)
                {
                    double prev = (i > 0 ? m_visibleLables[i - 1].DoubleValue : double.NaN);
                    double nextv = (i < c - 1 ? m_visibleLables[i + 1].DoubleValue : double.NaN);
                    double currv = m_visibleLables[i].DoubleValue;

                    if (double.IsNaN(prev) && double.IsNaN(nextv))
                    {
                        prev = VisibleRange.Min;
                        nextv = VisibleRange.Max;
                    }
                    else if (double.IsNaN(prev))
                    {
                        prev = 2 * currv - nextv;
                    }
                    else if (double.IsNaN(nextv))
                    {
                        nextv = 2 * currv - prev;
                    }

                    if (VisibleRange.Intersects(new MinMaxInfo(prev, nextv, 1)))
                    {
                        float pre = GetCoordinateFromValue((prev + currv) / 2);
                        float next = GetCoordinateFromValue((nextv + currv) / 2);

                        RectangleF gridCell = new RectangleF(xGrid, Math.Min(pre, next), wGrid, Math.Abs(pre - next));
                        GraphicsContainer gContainer = DrawingHelper.BeginTransform(g);

                        g.SetClip(clipRect);

                        DrawingHelper.DrawRectangleF(g, pen, gridCell);
                        DrawingHelper.EndTransform(g, gContainer);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the axis title.
        /// </summary>
        /// <param name="g">The graphics to draw.</param>
        /// <param name="rect">The rectangle to draw.</param>
        private void DrawAxisText(Graphics g, RectangleF rect)
        {
            string title = this.Title;
            RectangleF textRect = rect;

            if (m_titleDrawMode == ChartTitleDrawMode.Wrap)
            {
                textRect.Size = g.MeasureString(title, this.TitleFont, (int)m_length);
            }
            else
            {
                if (m_titleDrawMode == ChartTitleDrawMode.Ellipsis)
                {
                    title = DrawingHelper.EllipsesText(g, title, this.TitleFont, m_length);
                }

                textRect.Size = g.MeasureString(title, this.TitleFont);
            }

            if (m_orientation == ChartOrientation.Horizontal)
            {
                switch (m_titleAlignment)
                {
                    case StringAlignment.Center:
                        textRect.X += (rect.Width - textRect.Width) / 2;
                        break;
                    case StringAlignment.Far:
                        textRect.X += rect.Width - textRect.Width;
                        break;
                }

				if (AxisLabelPlacement == ChartPlacement.Inside)
                {
                    if (m_opposedPosition)
                    {
                        textRect.Y = rect.Top - textRect.Height;
                    }
                    else
                    {
                        textRect.Y = rect.Bottom;
                    }
                }
                else if (!m_opposedPosition)
                {
                    textRect.Y = rect.Bottom - textRect.Height;
                }

                using (SolidBrush sb = new SolidBrush(this.TitleColor))
                {
                    g.DrawString(title, this.TitleFont, sb, textRect, DrawingHelper.NoClipFormat);
                }
            }
            else
            {
                textRect.Y = 0;

                switch (m_titleAlignment)
                {
                    case StringAlignment.Near:
                        textRect.X = 0;
                        break;
                    case StringAlignment.Center:
                        textRect.X = (rect.Height - textRect.Width) / 2;
                        break;
                    case StringAlignment.Far:
                        textRect.X = rect.Height - textRect.Width;
                        break;
                }

                GraphicsContainer cont = DrawingHelper.BeginTransform(g);

				if (AxisLabelPlacement == ChartPlacement.Inside)
                {
                    if (m_opposedPosition)
                    {
                        g.TranslateTransform(rect.Right + textRect.Height, rect.Top);
                        g.RotateTransform(90);
                    }
                    else
                    {
                        g.TranslateTransform(rect.Left - textRect.Height, rect.Bottom);
                        g.RotateTransform(-90);
                    }
                }
                else if (m_opposedPosition)
                {
                    g.TranslateTransform(rect.Right, rect.Top);
                    g.RotateTransform(90);
                }
                else
                {
                    g.TranslateTransform(rect.Left, rect.Bottom);
                    g.RotateTransform(-90);
                }

                using (SolidBrush sb = new SolidBrush(this.TitleColor))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(title, this.TitleFont, sb, textRect, DrawingHelper.NoClipFormat);
                }

                DrawingHelper.EndTransform(g, cont);
            }
        }

        /// <summary>
        /// Reanders axis' text.
        /// </summary>
        /// <param name="g">The graphics to render text.</param>
        /// <param name="rect">The rectangle to render.</param>
        /// <param name="z">The z-coord.</param>
        /// <returns></returns>
        private Path3D DrawAxisText(Graphics3D g, RectangleF rect, float z)
        {
            Font font = this.TitleFont;
            GraphicsPath gp = new GraphicsPath();

            if (m_title != "")
            {
                string title = this.Title;
                RectangleF textRect = rect;

                if (m_titleDrawMode == ChartTitleDrawMode.Wrap)
                {
                    textRect.Size = g.Graphics.MeasureString(title, this.TitleFont, (int)m_length);
                }
                else
                {
                    if (m_titleDrawMode == ChartTitleDrawMode.Ellipsis)
                    {
                        title = DrawingHelper.EllipsesText(g.Graphics, title, this.TitleFont, m_length);
                    }

                    textRect.Size = g.Graphics.MeasureString(title, this.TitleFont);
                }

                if (Orientation == ChartOrientation.Horizontal)
                {
                    switch (m_titleAlignment)
                    {
                        case StringAlignment.Center:
                            textRect.X += (rect.Width - textRect.Width) / 2;
                            break;
                        case StringAlignment.Far:
                            textRect.X += rect.Width - textRect.Width;
                            break;
                    }

					if (AxisLabelPlacement == ChartPlacement.Inside)
                    {
                    if (m_opposedPosition)
                    {
                        textRect.Y = rect.Top - textRect.Height;
                    }
                    else
                    {
                        textRect.Y = rect.Bottom;
                    }
                    }	
                    else if (!m_opposedPosition)
                    {
                        textRect.Y = rect.Bottom - textRect.Height;
                    }

                    RenderingHelper.AddTextPath(gp, g.Graphics, title, font, textRect);
                }
                else
                {
                    textRect.Y = 0;

                    switch (m_titleAlignment)
                    {
                        case StringAlignment.Near:
                            textRect.X = 0;
                            break;
                        case StringAlignment.Center:
                            textRect.X = (rect.Height - textRect.Width) / 2;
                            break;
                        case StringAlignment.Far:
                            textRect.X = rect.Height - textRect.Width;
                            break;
                    }

                    Matrix transform = new Matrix();

					if (AxisLabelPlacement == ChartPlacement.Inside)
                    {
                        if (m_opposedPosition)
                        {
                            transform.Translate(rect.Right + textRect.Height, rect.Top);
                            transform.Rotate(90);
                        }
                        else
                        {
                            transform.Translate(rect.Left - textRect.Height, rect.Bottom);
                            transform.Rotate(-90);
                        }
                    }
                    else if (m_opposedPosition)
                    {
                        transform.Translate(rect.Right, rect.Top);
                        transform.Rotate(90);
                    }
                    else
                    {
                        transform.Translate(rect.Left, rect.Bottom);
                        transform.Rotate(-90);
                    }

                    RenderingHelper.AddTextPath(gp, g.Graphics, title, font, textRect);
                    gp.Transform(transform);
                }
            }

            return gp.PointCount > 0 ? Path3D.FromGraphicsPath(gp, z, new SolidBrush(this.TitleColor)) : null;
        }
        #endregion

        #region Events handlers
        /// <summary>
        /// Handles visual properties changed events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnNeedRedraw(object sender, EventArgs args)
        {
            this.OnAppearanceChanged(args);
        }

        /// <summary>
        /// Handles layout properties changed events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnNeedResize(object sender, EventArgs args)
        {
            m_needUpdate = true;
            this.OnDimensionsChanged(args);
        }

        /// <summary>
        /// Called when range is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnRangeChanged(object sender, EventArgs args)
        {
            if (m_range.Interval == 0)
                throw new ArgumentException("Interval can't zero");

            m_rangeType = ChartAxisRangeType.Set;

            this.OnNeedResize(sender, args);
        }

        /// <summary>
        /// Called when break ranges is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnBreakRangesChanged(object sender, EventArgs args)
        {
            this.OnIntervalsChanged(args);
        }
        #endregion

        #region Raise events methods
        /// <summary>
        /// Raises FormatLabel event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Forms.Chart.ChartFormatAxisLabelEventArgs"/> instance containing the event data.</param>
        protected void RaiseFormatLabel(object sender, ChartFormatAxisLabelEventArgs args)
        {
            if (this.FormatLabel != null)
            {
                this.FormatLabel(sender, args);
            }
        }
        #endregion

        /// <summary>
        /// Gets the margins point.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns></returns>
        private PointF GetMarginsPoint(Graphics g, PointF pt, SizeF sz)
        {
            float x = pt.X;
            float y = pt.Y;

            if (edgeLabelsDrawingMode == ChartAxisEdgeLabelsDrawingMode.Shift)
            {
                if (Orientation == ChartOrientation.Horizontal)
                {
                    float off = GetOffsetLabel(sz.Width) - sz.Width / 2;

                    if (adjustPlotAreaMargins == ChartSetMode.AutoSet)
                    {
                        x = Math.Max(x, Location.X - off);
                        x = Math.Min(x, Location.X + RealLength - sz.Width - off);
                    }
                    else if (adjustPlotAreaMargins == ChartSetMode.UserSet)
                    {
                        x = Math.Max(x, Location.X + chartPlotAreaMargins.Left - off);
                        x = Math.Min(x, Location.X + RealLength - chartPlotAreaMargins.Right - sz.Width - off);
                    }
                }
                else
                {
                    float off = GetOffsetLabel(sz.Height) - sz.Height / 2;

                    if (adjustPlotAreaMargins == ChartSetMode.AutoSet)
                    {
                        y = Math.Max(y, Location.Y - RealLength - off);
                        y = Math.Min(y, Location.Y - sz.Height - off);
                    }
                    else if (adjustPlotAreaMargins == ChartSetMode.UserSet)
                    {
                        y = Math.Max(y, Location.Y - RealLength + chartPlotAreaMargins.Top - off);
                        y = Math.Min(y, Location.Y - chartPlotAreaMargins.Bottom - sz.Height - off);
                    }
                }
            }
            else if (edgeLabelsDrawingMode == ChartAxisEdgeLabelsDrawingMode.ClippingProtection)
            {
                RectangleF rf = m_area.ClientRectangle;
                if (Orientation == ChartOrientation.Horizontal)
                {
                    float off = GetOffsetLabel(sz.Width) - sz.Width / 2;

                    x = Math.Max(x, rf.Left - off);
                    x = Math.Min(x, rf.Right - sz.Width - off);
                }
                else
                {
                    float off = GetOffsetLabel(sz.Height) - sz.Height / 2;

                    y = Math.Max(y, rf.Top - off);
                    y = Math.Min(y, rf.Bottom - sz.Height - off);
                }
            }
            return new PointF(x, y);
        }

        /// <summary>
        /// Converts the type of to value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>Converted value.</returns>
        private double ConvertToValueType(double value)
        {
            if (m_valueType == ChartValueType.Logarithmic)
            {
                return Math.Pow(m_logBase, value);
            }

            return value;
        }

        /// <summary>
        /// Returns a nice zoom factor by taking into account the range of the axis.
        /// </summary>
        private double SetSmartZoomFactor(double zoomFact)
        {
            MinMaxInfo range = Range;
            DateTime now = new DateTime(2005, 8, 17, 0, 0, 0);
            double delta = range.Delta;
            double minBound = 0.5d;
            double maxBound = 1.5d;

            double yearFactor = (now.AddYears(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfYearsFactor = maxBound * yearFactor;
            double halfYearFactor = minBound * yearFactor;

            double monthFactor = (now.AddMonths(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfMonthFactor = maxBound * monthFactor;
            double halfMonthFactor = minBound * monthFactor;

            double weekFactor = (now.AddDays(7).ToOADate() - now.ToOADate()) / delta;
            double oneHalfWeekFactor = maxBound * weekFactor;
            double halfWeekFactor = minBound * weekFactor;

            double dayFactor = (now.AddDays(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfDayFactor = maxBound * dayFactor;
            double halfDayFactor = minBound * dayFactor;

            double hourFactor = (now.AddHours(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfHourFactor = maxBound * hourFactor;
            double halfHourFactor = minBound * hourFactor;

            double minuteFactor = (now.AddHours(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfMinuteFactor = maxBound * minuteFactor;
            double halfMinuteFactor = minBound * minuteFactor;

            double secondFactor = (now.AddSeconds(1).ToOADate() - now.ToOADate()) / delta;
            double oneHalfSecondFactor = maxBound * secondFactor;
            double halfSecondFactor = minBound * secondFactor;

            #region Old
            //if (zoomFact < oneHalfSecondFactor)// && zoomFact > halfSecondFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomSecondLevelLabelFormat;
            //  return secondFactor;
            //}
            //else if (zoomFact < oneHalfMinuteFactor)//&& zoomFact > halfMinuteFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomMinuteLevelLabelFormat;
            //  return minuteFactor;
            //}
            //else if (zoomFact < oneHalfHourFactor)//&& zoomFact > halfHourFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomHourLevelLabelFormat;
            //  return hourFactor;
            //}
            //else if (zoomFact < oneHalfDayFactor)// && zoomFact > halfDayFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomDayLevelLabelFormat;
            //  return dayFactor;
            //}
            //else if (zoomFact < oneHalfWeekFactor)// && zoomFact > halfWeekFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomWeekLevelLabelFormat;
            //  return weekFactor;
            //}
            //else if (zoomFact < oneHalfMonthFactor)// && zoomFact > halfMonthFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomMonthLevelLabelFormat;
            //  return monthFactor;
            //}
            //else if (zoomFact < oneHalfYearsFactor && zoomFact > halfYearFactor)
            //{
            //  currentSmartDateTimeFormat = SmartDateZoomYearLevelLabelFormat;
            //  return yearFactor;
            //}
            //else
            //{
            //  if (zoomFact > oneHalfYearsFactor)
            //  {
            //    currentSmartDateTimeFormat = this.DateTimeFormat;
            //    return zoomFact;
            //  }
            //}
            #endregion

            if (zoomFact < 2 * minuteFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomMinuteLevelLabelFormat;
                return minuteFactor;
            }
            else if (zoomFact < 2 * hourFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomHourLevelLabelFormat;
                return hourFactor;
            }
            else if (zoomFact < 2 * dayFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomDayLevelLabelFormat;
                return dayFactor;
            }
            else if (zoomFact < 2 * weekFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomWeekLevelLabelFormat;
                return weekFactor;
            }
            else if (zoomFact < 2 * monthFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomMonthLevelLabelFormat;
                return monthFactor;
            }
            else if (zoomFact < 2 * yearFactor)
            {
                currentSmartDateTimeFormat = SmartDateZoomYearLevelLabelFormat;
                return yearFactor;
            }
            else
            {
                currentSmartDateTimeFormat = this.DateTimeFormat;
            }

            return zoomFact;
        }

        /// <summary>
        /// Generates the label for given value and chart area.
        /// </summary>
        /// <param name="value">The value to generate label.</param>
        /// <param name="area">The <see cref="ChartArea"/>.</param>
        /// <param name="index">The index of label.</param>
        /// <returns>Generated label.</returns>
        internal ChartAxisLabel GenerateLabel(double value, ChartArea area, int index)
        {
            ChartAxisLabel label = new ChartAxisLabel();

            label.IsAuto = true;
            label.ValueType = ChartValueType.Custom;
            label.DoubleValue = value;

            if (m_scaleLables && m_scaleLablesCoef >= 0)
            {
                Font font = this.Font;
                float sz = 0.5f * font.Size * (1 + m_scaleLablesCoef);
                label.Font = new Font(font.FontFamily, sz, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            }
            else
            {
                label.Font = this.Font;
            }

            if ((area.Chart.Series.Count != 0) && this.IsIndexed)
            {
                value = area.Chart.IndexValues.GetValue(value);
            }

            #region Get text of label by value
            switch (m_valueType)
            {
                case ChartValueType.Logarithmic:
                    #region Logarithmic Value type
                    {
                        label.CustomText = value.ToString(m_format);
                    }
                    #endregion
                    break;

                case ChartValueType.Double:
                    #region Double Value type
                    {
                        if (m_roundingPlaces > -1)
                        {
                            label.CustomText = Math.Round(value, Math.Min(m_roundingPlaces, c_maxRoundingPlaces)).ToString(m_format);
                        }
                        else
                        {
                            label.CustomText = value.ToString(m_format);
                        }
                    }
                    #endregion
                    break;

                case ChartValueType.DateTime:
                    #region DateTime Value type
                    if (smartDateZoom)
                    {
                        DateTimeFormatInfo dtIF = new CultureInfo(smartDateZoomLabelsCulture, false).DateTimeFormat;
                        label.CustomText = DateTime.FromOADate(value).ToString(currentSmartDateTimeFormat, dtIF);
                    }
                    else
                    {
                        if (m_dateTimeFormat == "")
                        {
                            label.CustomText = this.DoubleToDateTime(value).ToShortDateString();
                        }
                        else
                        {
                            label.CustomText = this.DoubleToDateTime(value).ToString(m_dateTimeFormat);
                        }
                    }
                    #endregion
                    break;

                case ChartValueType.Custom:
                    #region Custom Value type
                    {
                        if (m_customLabelsParameter == ChartCustomLabelsParameter.Index)
                        {
                            if (index > -1 && index < this.LabelsImpl.Count)
                            {
                                label.CustomText = this.LabelsImpl.GetLabelAt(index).Text;
                                label.ToolTip = this.LabelsImpl.GetLabelAt(index).ToolTip;
                            }
                        }
                        else
                        {
                            int intValue = (int)value;

                            if (intValue > -1 && intValue < this.LabelsImpl.Count)
                            {
                                label.CustomText = this.LabelsImpl.GetLabelAt(intValue).Text;
                                label.ToolTip = this.LabelsImpl.GetLabelAt(intValue).ToolTip;
                            }
                        }
                    }
                    #endregion
                    break;
            }
            #endregion

            ChartFormatAxisLabelEventArgs args = new ChartFormatAxisLabelEventArgs(label.CustomText, value, this);
            
            this.RaiseFormatLabel(this, args);
            label.m_axisLabelPlacement = this.AxisLabelPlacement;

            if (args.Handled)
            {
                label.AxisLabelPlacement  = args.AxisLabelPlacement;
                label.CustomText = args.Label;
                label.ToolTip = args.ToolTip;            
            }

            return label;
        }

        /// <summary>
        /// Gets the small ticks values.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<float> GetSmallTicks()
        {
            List<float> ticks = new List<float>();

            if (this.BreaksEnabled && m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
            {
                foreach (ChartAxisSegment segment in m_breakRanges.Segments)
                {
                    double start = segment.Range.Start;
                    double end = segment.Range.End;
                    double interval = segment.Interval / (m_smallTicksPerInterval + 1);

                    if (m_valueType == ChartValueType.Logarithmic)
                    {
                        for (double value = start; value < end; value++)
                    { 
                        int count = 1;
                        double Loginterval = Math.Pow(m_logBase, this.VisibleRange.interval);
                        double Dinterval = Loginterval / (m_smallTicksPerInterval + 2);
                        double Dvalue = Math.Pow(m_logBase, value);
                        for (double i = (Dinterval + 1); i < 10 && count <= m_smallTicksPerInterval; i += Dinterval)
                        {
                          double tickValues = Dvalue * i;
                          ticks.Add(this.GetCoordinateFromValue((tickValues)));
                          count += 1;
                        }
                    }
                    }
                    else
                    {
                        for (double value = start; value <= end; value += interval)
                        {
                            ticks.Add(this.GetCoordinateFromValue(value));
                        }
                    }
                }
            }
            else
            {
                double start = this.VisibleRange.min;
                double end = this.VisibleRange.max;
                double interval;
                if (m_valueType == ChartValueType.DateTime && m_visibleLables.Length > 1)
                {
                    interval = Math.Abs(m_visibleLables[0].DoubleValue - m_visibleLables[1].DoubleValue) / (m_smallTicksPerInterval + 1);
                }
                else
                {
                    interval = this.VisibleRange.interval / (m_smallTicksPerInterval + 1);
                }
                if (m_valueType == ChartValueType.Logarithmic)
                {
                    
                    for (double value = start; value < end; value++)
                    {
                        int count = 1;
                        double Loginterval = Math.Pow(m_logBase, this.VisibleRange.interval);
                        double Dinterval = Loginterval / (m_smallTicksPerInterval + 2);
                        double Dvalue = Math.Pow(m_logBase, value);
                        for (double i = (Dinterval + 1); i < 10 && count <= m_smallTicksPerInterval; i += Dinterval)
                        {
                          double tickValues = Dvalue * i;
                          ticks.Add(this.GetCoordinateFromValue((tickValues)));
                          count += 1;
                        }
                    }

                }
                else
                {
                    for (double value = start; value <= end; value += interval)
                    {
                        ticks.Add(this.GetCoordinateFromValue(value));
                    }
                }
            }

            return ticks;
        }

        /// <summary>
        /// Gets the interlaced grid rects.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        private IEnumerable<RectangleF> GetInterlacedGridRects(RectangleF bounds)
        {
            List<RectangleF> rects = new List<RectangleF>();

            if (this.BreaksEnabled && m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
            {
                foreach (ChartAxisSegment segment in m_breakRanges.Segments)
                {
                    double start = segment.Range.Start;
                    double end = segment.Range.End;
                    double interval = 2 * segment.Interval;
                    double width = segment.Interval;

                    for (double value = start; value < end; value += interval)
                    {
                        RectangleF rect = new RectangleF();

                        double v1 = value;
                        double v2 = value + width;

                        if (m_valueType == ChartValueType.Logarithmic)
                        {
                            v1 = Math.Pow(m_logBase, v1);
                            v2 = Math.Pow(m_logBase, v2);
                        }

                        float c1 = this.GetCoordinateFromValue(v1);
                        float c2 = this.GetCoordinateFromValue(v2);

                        if (m_orientation == ChartOrientation.Horizontal)
                        {
                            c1 = ChartMath.MinMax(c1, bounds.Left, bounds.Right);
                            c2 = ChartMath.MinMax(c2, bounds.Left, bounds.Right);

                            rect.Y = bounds.Y;
                            rect.Height = bounds.Height;
                            rect.X = Math.Min(c1, c2);
                            rect.Width = Math.Abs(c2 - c1);
                        }
                        else
                        {
                            c1 = ChartMath.MinMax(c1, bounds.Top, bounds.Bottom);
                            c2 = ChartMath.MinMax(c2, bounds.Top, bounds.Bottom);

                            rect.X = bounds.X;
                            rect.Width = bounds.Width;
                            rect.Y = Math.Min(c1, c2);
                            rect.Height = Math.Abs(c2 - c1);
                        }

                        rects.Add(rect);
                    }
                }
            }
            else
            {
                double start = this.VisibleRange.min;
                double end = this.VisibleRange.max;
                double interval = 2 * this.VisibleRange.interval;
                double width = this.VisibleRange.interval;

                for (double value = start; value < end; value += interval)
                {
                    RectangleF rect = new RectangleF();

                    double v1 = value;
                    double v2 = value + width;

                    if (m_valueType == ChartValueType.Logarithmic)
                    {
                        v1 = Math.Pow(m_logBase, v1);
                        v2 = Math.Pow(m_logBase, v2);
                    }

                    float c1 = this.GetCoordinateFromValue(v1);
                    float c2 = this.GetCoordinateFromValue(v2);

                    if (m_orientation == ChartOrientation.Horizontal)
                    {
                        c1 = ChartMath.MinMax(c1, bounds.Left, bounds.Right);
                        c2 = ChartMath.MinMax(c2, bounds.Left, bounds.Right);

                        rect.Y = bounds.Y;
                        rect.Height = bounds.Height;
                        rect.X = Math.Min(c1, c2);
                        rect.Width = Math.Abs(c2 - c1);
                    }
                    else
                    {
                        c1 = ChartMath.MinMax(c1, bounds.Top, bounds.Bottom);
                        c2 = ChartMath.MinMax(c2, bounds.Top, bounds.Bottom);

                        rect.X = bounds.X;
                        rect.Width = bounds.Width;
                        rect.Y = Math.Min(c1, c2);
                        rect.Height = Math.Abs(c2 - c1);
                    }

                    rects.Add(rect);
                }
            }

            return rects;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        /// <param name="zoomFactor">The zoom factor.</param>
        /// <param name="zoomPosition">The zoom position.</param>
        /// <returns></returns>
        private MinMaxInfo CalculateVisibleRange(double zoomFactor, double zoomPosition)
        {
            MinMaxInfo result = m_range.Clone();

            if (m_zoomFactor != 1d)
            {
                bool isIndexed = this.IsIndexed;
                double interval = this.CalculateNiceInternalEx(m_range.Interval * m_zoomFactor);
                double start = m_range.Min + m_range.Delta * m_zoomPosition;
                double end = start + m_range.Delta * m_zoomFactor;

                if (this.ValueType != ChartValueType.Logarithmic)
                {
                    int intervalCount = (int)Math.Ceiling(m_range.Delta * m_zoomFactor / interval);
                    if (this.ValueType != ChartValueType.DateTime && this.RangeType != ChartAxisRangeType.Set && m_zoomPosition != 0)
                    start = Math.Max(ChartMath.Round(start, interval), m_range.min);
                    end = start + intervalCount * interval;
                }

                if (isIndexed)
                {
                    interval = Math.Max(0, Math.Ceiling(interval));
                }

                if (interval < m_zoomedRange.Delta)
                {
                    if (isIndexed)
                    {
                        result = new MinMaxInfo(start, end, 1);
                    }
                    else
                    {
                        result = new MinMaxInfo(start, end, interval);
                    }
                }
            }

            return new MinMaxInfo(result.min + m_offset + m_pointOffset, result.max + m_offset + m_pointOffset, result.interval);
        }

        /// <summary>
        /// Recalculates the visible ranges.
        /// </summary>
        private void RecalculateRanges()
        {
            m_zoomedRange = m_range.Clone();
            bool isIndexed = this.IsIndexed;

            if (m_zoomFactor != 1d)
            {
                double interval = this.CalculateNiceInternalEx(m_range.Interval * m_zoomFactor);
                double start = m_range.Min + m_range.Delta * m_zoomPosition;
                double end = start + m_range.Delta * m_zoomFactor;

                if (this.ValueType != ChartValueType.Logarithmic)
                {
                    int intervalCount = (int)Math.Ceiling(m_range.Delta * m_zoomFactor / interval);
                    if (this.ValueType != ChartValueType.DateTime && this.RangeType != ChartAxisRangeType.Set && m_zoomPosition != 0)
                    start = Math.Max(ChartMath.Round(start, interval), m_range.min);
                    end = start + intervalCount * interval;
                }

                if (isIndexed)
                {
                    interval = Math.Max(0, Math.Ceiling(interval));
                }

                if (interval < m_zoomedRange.Delta)
                {
                    if (isIndexed)
                    {
                        m_zoomedRange = new MinMaxInfo(start, end, 1);
                    }
                    else
                    {
                        m_zoomedRange = new MinMaxInfo(start, end, interval);
                    }
                }

                if (isIndexed)
                {
                    m_labelOffset = Math.Round(m_zoomedRange.min) - m_zoomedRange.min;
                }
                else
                {
                    m_labelOffset = 0d;
                }
            }
            else
            {
                m_labelOffset = 0d;
            }

            if (this.smartDateZoom)
            {
                this.SetSmartZoomFactor(m_zoomFactor);
            }

            m_visibleRange = new MinMaxInfo(m_zoomedRange.Min - m_offset - m_pointOffset,
                m_zoomedRange.Max - m_offset - m_pointOffset, m_zoomedRange.interval);
        }

        /// <summary>
        /// Invalidates the ranges.
        /// </summary>
        private void InvalidateRanges()
        {
            m_needUpdate = true;

            this.OnVisibleRangeChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Recalculates the visible labels.
        /// </summary>
        /// <param name="area">The area.</param>
        private void RecalculateVisibleLabels(ChartArea area)
        {
            List<ChartAxisLabel> vLabels = new List<ChartAxisLabel>();

            MinMaxInfo vRange = this.VisibleRange;
            MinMaxInfo aRange = this.Range;

            if (m_tickDrawingOperationMode == ChartAxisTickDrawingOperationMode.NumberOfIntervalsFixed)
            {
                aRange = vRange;
            }

            #region Automatic labels
            if ((tickLabelDrawingMode & ChartAxisTickLabelDrawingMode.AutomaticMode) == ChartAxisTickLabelDrawingMode.AutomaticMode)
            {
                if (this.BreaksEnabled && m_breakRanges.BreaksMode == ChartBreaksMode.Auto)
                {
                    int index = 0;

                    for (int i = 0, ci = m_breakRanges.Segments.Count; i < ci; i++)
                    {
                        ChartAxisSegment segment = m_breakRanges.Segments[i] as ChartAxisSegment;
                        double pos = segment.Range.Start;

                        while ((i == ci - 1) && (pos == segment.Range.End)
                            || (pos < segment.Range.End))
                        {
                            index++;
                            vLabels.Add(this.GenerateLabel(this.ConvertToValueType(pos), area, index));
                            pos += segment.Interval;
                        }
                    }
                }
                else
                {
                    double value = aRange.Min + m_labelOffset;
                    double interval = aRange.Interval;
                    IEnumerator intervalEnumerator = null;
                    bool getNext = true;
                    int index = 0;

                    if ((m_valueType == ChartValueType.DateTime)
                        && (m_dateTimeInterval.Type != ChartDateTimeIntervalType.Auto))
                    {
                        DateTime start = DateTime.FromOADate(aRange.Min);
                        DateTime end = DateTime.FromOADate(aRange.Max);
                        intervalEnumerator = m_dateTimeInterval.Iterator(start, end).GetEnumerator();
                        intervalEnumerator.Reset();
                        intervalEnumerator.MoveNext();
                    }

                    while (getNext)
                    {
                        double labelValue = value + m_labelsOffset + m_pointOffset;

                        if (this.BreaksEnabled)
                        {
                            if (m_breakRanges.IsVisible(labelValue))
                            {
                                vLabels.Add(this.GenerateLabel(ConvertToValueType(labelValue), area, index));
                            }
                        }
                        else
                        {
                            if (this.Inside(vRange, labelValue - m_pointOffset))
                            {
                                vLabels.Add(this.GenerateLabel(ConvertToValueType(labelValue), area, index));
                            }
                        }

                        index++;

                        if (intervalEnumerator != null)
                        {
                            if (getNext = intervalEnumerator.MoveNext())
                            {
                                DateTime dValue = (DateTime)intervalEnumerator.Current;
                                value = dValue.ToOADate();
                            }
                        }
                        else
                        {
                            value += interval;
                            getNext = (this.ValueType == ChartValueType.Double) ? (value - vRange.max < 1e-15) : (value - vRange.max < 0.000001); 
                        }
                    }
                }
            }
            #endregion

            #region Custom labels
            if ((tickLabelDrawingMode & ChartAxisTickLabelDrawingMode.UserMode) == ChartAxisTickLabelDrawingMode.UserMode)
            {
                for (int i = 0; i < this.LabelsImpl.Count; i++)
                {
                    ChartAxisLabel cal = this.LabelsImpl.GetLabelAt(i);

                    if (vRange.Contains(cal.DoubleValue) && !(this.BreaksEnabled && m_breakRanges.IsVisible(cal.DoubleValue)))
                    {
                        vLabels.Add(cal);
                    }
                }
            }
            #endregion

            m_visibleLables = vLabels.ToArray();

            Array.Sort(m_visibleLables, new ChartAxisLabelByDoubleValueComparer(m_inversed));
        }

        /// <summary>
        /// Insides the specified MinMaxInfo.
        /// </summary>
        /// <param name="mmi">The MinMaxInfo.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool Inside(MinMaxInfo mmi, double value)
        {
            if (value - mmi.min < -0.000001) return false;
            if (value - mmi.max > 0.000001) return false;

            return true;
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to <see cref="Double"/>.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/>.</param>
        /// <returns></returns>
        private double DateTimeToDouble(DateTime dateTime)
        {
            if (dateTime < c_dateTimeZero)
            {
                return dateTime.Date.ToOADate() - (dateTime.ToOADate() % 1);
            }

            return dateTime.ToOADate();
        }

        /// <summary>
        /// Converts the <see cref="Double"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private DateTime DoubleToDateTime(double value)
        {
            if (value < 0)
            {
                return DateTime.FromOADate(Math.Floor(value) - 1 - value % 1);
            }

            return DateTime.FromOADate(value);
        }

        /// <summary>
        /// Zoomings the allowed.
        /// </summary>
        /// <param name="zoomFactor">The zoom factor.</param>
        /// <param name="zoomPosition">The zoom position.</param>
        /// <returns></returns>
        private void SetZooming(double zoomFactor, double zoomPosition)
        {
            if (zoomFactor > 1)
            {
                zoomPosition = 0;
            }
            else
            {
                zoomPosition = ChartMath.MinMax(zoomPosition, 0d, 1d - zoomFactor);
            }

            if (m_zoomFactor != zoomFactor || m_zoomPosition != zoomPosition)
            {
                bool calceled = false;

                if (this.Zooming != null)
                {
                    MinMaxInfo range = this.CalculateVisibleRange(zoomFactor, zoomPosition);
                    ChartAxisZoomingArgs args = new ChartAxisZoomingArgs(this, range, zoomFactor, zoomPosition);

                    this.Zooming(this, args);

                    calceled = args.Cancel;
                }

                if (!calceled)
                {
                    m_zoomFactor = zoomFactor;
                    m_zoomPosition = zoomPosition;

                    this.InvalidateRanges();

                    if (this.Zoomed != null)
                    {
                        this.Zoomed(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_area != null)
            {
                m_area.Axes.Remove(this);
                m_area = null;
            }

            if (m_stripLines != null)
            {
                m_stripLines.Changed -= new EventHandler(this.OnNeedRedraw);
                m_stripLines.Clear();
                m_stripLines = null;
            }

            m_visibleLables = null;
            m_labelStringFormat.Dispose();
            m_lineType.SettingsChanged -= new EventHandler(OnNeedRedraw);
            m_gridLineType.SettingsChanged -= new EventHandler(OnNeedRedraw);

            if (m_range != null)
            {
                m_range.SettingsChanged -= new EventHandler(OnNeedResize);
                m_range = null;
            }

            if (this.GroupingLabels != null)
            {
                this.GroupingLabels.Changed -= new EventHandler(this.OnNeedResize);
            }

            if (this.Labels != null)
            {
                this.Labels.Changed -= new EventHandler(this.OnNeedResize);
            }
        }
        #endregion
    }
}