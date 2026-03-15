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

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
    #region Base Config Item
    /// <summary>
    ///     Base class of configuration items that are created for specific chart-types. Configuration items are a convenient way to store
    ///     information specific to certain chart types that may not be applicable to other chart types.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class ChartConfigItem : IChangeNotifyingItem
    {
        #region Members
        private bool noEvents = false;
        #endregion

        #region Events
        /// <summary>
        ///     Event that is raised when a property is changed.
        /// </summary>
        public event SyncfusionPropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        /// <summary>
        ///     Controls whether events are raised or not. If set to True, events are not raised.
        /// </summary>
        protected bool NoEvents
        {
            get
            {
                return this.noEvents;
            }

            set
            {
                this.noEvents = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        ///     Overloaded. Call this method to raise a <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName" type="string">
        ///     <para>
        ///      The name of the property that has changed.
        ///     </para>
        /// </param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(PropertyChangeEffect.NeedRepaint, propertyName, null, null);
        }

        /// <summary>
        ///      Call this method to raise a <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="effect" type="Syncfusion.ComponentModel.PropertyChangeEffect">
        ///     <para>
        ///     The effect that is to take place when a property is changed.
        ///     </para>
        /// </param>
        /// <param name="propertyName" type="string">
        ///     <para>
        ///     The name of the property changed.
        ///     </para>
        /// </param>
        /// <param name="oldValue" type="object">
        ///     <para>
        ///     The old value of the property that is changed.
        ///     </para>
        /// </param>
        /// <param name="newValue" type="object">
        ///     <para>
        ///     The new value of the property that is changed.
        ///     </para>
        /// </param>
        protected void RaisePropertyChanged(PropertyChangeEffect effect, string propertyName, object oldValue, object newValue)
        {
            if (!noEvents)
            {
                SyncfusionPropertyChangedEventArgs args = new SyncfusionPropertyChangedEventArgs(effect, propertyName, oldValue, newValue);

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, args);
                }
            }
        }
        #endregion
    }
    #endregion

    #region Enums
    /// <summary>
    ///   The type of bubble that is to be rendered with Bubble charts.
    /// </summary>
    public enum ChartBubbleType
    {
        /// <summary>
        ///   Circular shaped bubbles are rendered with Bubble charts.
        /// </summary>
        Circle,

        /// <summary>
        ///   Square shaped bubbles are rendered with Bubble charts.
        /// </summary>
        Square,

        /// <summary>
        ///   Images are rendered as bubbles with Bubble charts.
        /// </summary>
        Image,
    }

    /// <summary>
    ///     Defines the behavior in which bubble sizes vary.
    /// </summary>
    public enum ChartBubbleSizingBehavior
    {
        /// <summary>
        ///   Bubble sizes are fixed. This behavior is not supported in the current version.
        /// </summary>
        Fixed,

        /// <summary>
        ///   Bubble sizes are proportional to value.
        /// </summary>
        Proportional,
    }

    /// <summary>
    /// The type of Radar chart that is to be rendered.    
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// series.ConfigItems.RadarItem.Type = ChartRadarDrawType.Symbol;
    /// </code>    
    /// </example>
    public enum ChartRadarDrawType
    {
        /// <summary>
        /// Renders a Radar chart such that points are connected and the enclosed region is filled.    
        /// </summary>
        Area,

        /// <summary>
        /// Renders a Radar chart such that points are connected but the enclosed region is not filled.    
        /// </summary>
        Line,

        /// <summary>
        /// Renders a Radar chart such that points are rendered with the associated symbol. They are not connected to each other.
        /// </summary>
        Symbol
    }

    /// <summary>
    /// Lists the mode in which the pie Gradient should be rendered.
    /// </summary>
    public enum ChartPieFillMode
    {
        /// <summary>
        /// Apply the colors as a whole to the full pie.
        /// </summary>
        AllPie,

        /// <summary>
        /// Apply the colors to each individual slice.
        /// </summary>
        EveryPie
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ChartPieType
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        OutSide,

        /// <summary>
        /// 
        /// </summary>
        InSide,

        /// <summary>
        /// 
        /// </summary>
        Round,

        /// <summary>
        /// 
        /// </summary>
        Bevel,

        /// <summary>
        /// 
        /// </summary>
        Custom
    }

    /// <summary>
    /// Lists the shading mode options for bars and charts.
    /// </summary>
    public enum ChartColumnShadingMode
    {
        /// <summary>
        /// Rendered as a flat rectangle.
        /// </summary>
        FlatRectangle,

        /// <summary>
        /// Rendered in phong style.
        /// </summary>
        PhongCylinder
    }

    /// <summary>
    /// Lists the rendering options for columns when in 3D mode.
    /// </summary>
    public enum ChartColumnType
    {
        /// <summary>
        /// Rendered as a box.
        /// </summary>
        Box,

        /// <summary>
        /// Rendered as a cylinder.
        /// </summary>
        Cylinder
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ChartFinancialColorMode
    {
        /// <summary>
        /// 
        /// </summary>
        Fixed,

        /// <summary>
        /// 
        /// </summary>
        Mixed,

        /// <summary>
        /// 
        /// </summary>
        DarkLight
    }
    #endregion

    #region Config Items

    /// <summary>
    /// Configuration item that pertains to "all" chart types.
    /// </summary>
    public sealed class ChartSeriesParameters
    {
        #region Members
        private ChartArea m_area;
        private float m_seriesPointSpacing = 0.3f;
        private float m_seriesDepthSpacing = 0.1f;
        private float m_seriesSpacing = 0f;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the spacing between series.
        /// </summary>
        /// <value>The series spacing.</value>
        public float SeriesSpacing
        {
            get 
            {
                return m_seriesSpacing; 
            }

            set
            {
                if (m_seriesSpacing != value)
                {
                    m_seriesSpacing = value;
                    m_area.Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between points.
        /// </summary>
        /// <value>The point spacing.</value>
        public float PointSpacing
        {
            get 
            {
                return m_seriesPointSpacing; 
            }

            set
            {
                if (m_seriesPointSpacing != value)
                {
                    m_seriesPointSpacing = value;
                    m_area.Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the depth spacing between series.
        /// </summary>
        /// <value>The series depth spacing.</value>
        public float SeriesDepthSpacing
        {
            get 
            {
                return m_seriesDepthSpacing; 
            }

            set
            {
                if (m_seriesDepthSpacing != value)
                {
                    m_seriesDepthSpacing = value;

                    if (m_area.Series3D)
                    {
                        m_area.Redraw(true);
                    }
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartArea"/> class.
        /// </summary>
        /// <param name="area">The area.</param>
        internal ChartSeriesParameters(ChartArea area)
        {
            if (area == null)
                throw new ArgumentNullException("area");

            m_area = area;
        }
        #endregion
    }

    /// <summary>
    /// Configuration item that pertains to Gantt charts.
    /// </summary>
    public class ChartGanttConfigItem : ChartConfigItem
    {
        #region Members
        private ChartGanttDrawMode m_ganttDrawMode = ChartGanttDrawMode.CustomPointWidthMode;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the drawing mode of Gantt chart
        /// </summary>
        [DefaultValue(ChartGanttDrawMode.CustomPointWidthMode)]
        public ChartGanttDrawMode DrawMode
        {
            get { return m_ganttDrawMode; }
            set
            {
                if (m_ganttDrawMode != value)
                {
                    m_ganttDrawMode = value;
                    this.RaisePropertyChanged("GanttDrawMode");
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Configuration item that pertains to Bubble charts.
    /// </summary>
    public class ChartBubbleConfigItem : ChartConfigItem
    {
        #region Members
        private RectangleF minRect;
        private RectangleF maxRect;
        private ChartBubbleType bubbleType = ChartBubbleType.Circle;
        private bool m_enablePhongStyle = true;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChartBubbleConfigItem()
        {
            this.minRect = new RectangleF(PointF.Empty, new SizeF(25, 25));
            this.maxRect = new RectangleF(PointF.Empty, new SizeF(50, 50));
        }
        #endregion

        #region Properties
        /// <summary>
        ///     The minimum bounds to be used by a bubble. Default is (25, 25).
        /// </summary>
        public RectangleF MinBounds
        {
            get
            {
                return this.minRect;
            }

            set
            {
                if (this.minRect == value)
                {
                    return;
                }

                this.minRect = value;
                this.RaisePropertyChanged("MinBounds");
            }
        }

        /// <summary>
        ///     The maximum bounds to be used by a bubble. Default is (50, 50)
        /// </summary>
        public RectangleF MaxBounds
        {
            get
            {
                return this.maxRect;
            }

            set
            {
                if (this.maxRect == value)
                {
                    return;
                }

                this.maxRect = value;
                this.RaisePropertyChanged("MaxBounds");
            }
        }

        /// <summary>
        ///		The type of the bubble that is to be rendered. Default is Circle.
        /// </summary>
        [DefaultValue(ChartBubbleType.Circle)]
        public ChartBubbleType BubbleType
        {
            get
            {
                return this.bubbleType;
            }

            set
            {
                if (this.bubbleType == value)
                {
                    return;
                }

                this.bubbleType = value;
                this.RaisePropertyChanged("BubbleType");
            }
        }

        /// <summary>
        ///		Specifies if the PhongStyle is enable. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool EnablePhongStyle
        {
            get
            {
                return m_enablePhongStyle;
            }

            set
            {
                if (m_enablePhongStyle != value)
                {
                    m_enablePhongStyle = value;
                    this.RaisePropertyChanged("BubbleType");
                }
            }
        }
        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Pie charts.
    /// </summary>
    public class ChartPieConfigItem : ChartConfigItem
    {
        #region Members
        private float angleOffset = 0.0f;
        private ChartPieFillMode m_mode = ChartPieFillMode.AllPie;
        private ChartPieType m_type = ChartPieType.None;
        private ColorBlend m_colorGradient = null;
        private bool m_heightByAreaDepth = false;
        private bool m_showSeriesTitle = false;
        private ChartAccumulationLabelStyle m_labelStyle = ChartAccumulationLabelStyle.Outside;
        private float m_heightCoef = 0.2f;
        private float m_doughnutCoef = 0.0f;
        private bool m_pieWithSameRadius = false;
        private float m_pieRadius =0.0f;
        private float m_pieTilt = 0.2f;
        private float m_pieHeight = 10f;        
        private SizeF m_pieSize = SizeF.Empty;        
        private bool m_showDataBindLabels = false;
        #endregion

        #region Properties
        /// <summary>
        ///   The offset angle that is to be used when rendering Pie charts. Default is 0f.
        /// </summary>
        [DefaultValue(0.0f)]
        public float AngleOffset
        {
            get
            {
                return this.angleOffset;
            }

            set
            {
                if (this.angleOffset == value)
                {
                    return;
                }

                this.angleOffset = value;
                this.RaisePropertyChanged("AngleOffset");
            }
        }

        /// <summary>
        ///   Gets or sets whether the pie chart render in same radius when the LabelStyle is in Outside or OutsideInCoulmn. Default is false.
        /// </summary>
        [DefaultValue(false)]
        public bool PieWithSameRadius
        {
            get
            {
                return this.m_pieWithSameRadius;
            }

            set
            {
                if (this.m_pieWithSameRadius == value)
                {
                    return;
                }

                this.m_pieWithSameRadius = value;
                this.RaisePropertyChanged("PieWithSameRadius");
            }
        }   

        /// <summary>
        /// Gets or sets the pie radius. 
        /// </summary>
        /// <value>The pie radius.</value>
        [DefaultValue(0.0f)]
        public float PieRadius
        {
            get
            {
                return this.m_pieRadius;
            }

            set
            {
                if (this.m_pieRadius == value)
                {
                    return;
                }

                this.m_pieRadius = value;
                this.RaisePropertyChanged("PieRadius");
            }
        }
     
        /// <summary>
        /// Gets or sets the pie tilt. Use this property when MultiplePies is enabled.
        /// </summary>
        /// <value>The pie tilt.</value>
        [DefaultValue(0.2f)]
        public float PieTilt
        {
            get
            {
                return this.m_pieTilt;
            }

            set
            {
                if (this.m_pieTilt == value)
                {
                    return;
                }

                this.m_pieTilt = value;
                this.RaisePropertyChanged("PieTilt");
            }
        }
      
        /// <summary>
        /// Gets or sets the height of the pie. Use this property when MultiplePies is enabled.
        /// </summary>
        /// <value>The height of the pie.</value>
        [DefaultValue(10f)]
        public float PieHeight
        {
            get
            {
                return this.m_pieHeight;
            }

            set
            {
                if (this.m_pieHeight == value)
                {
                    return;
                }

                this.m_pieHeight = value;
                this.RaisePropertyChanged("PieHeight");
            }
        }       
   
        /// <summary>
        /// Gets or sets the size of the pie. Added for internal purposed and used only when MultiplePies is enabled.
        /// </summary>
        /// <value>The size of the pie.</value>
        public SizeF PieSize
        {
            get
            {
                return this.m_pieSize;
            }

            set
            {
                if (this.m_pieSize == value)
                {
                    return;
                }

                this.m_pieSize = value;
                this.RaisePropertyChanged("PiesSize");
            }
        }
       
        /// <summary>
        /// Gets or sets the painting type of the pie.
        /// </summary>
        /// <value>The painting type of the pie.</value>
        [DefaultValue(ChartPieType.None)]
        public ChartPieType PieType
        {
            get
            {
                return m_type;
            }

            set
            {
                if (m_type != value)
                {
                    m_type = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// Specifies how the <see cref="Gradient"/> should be applied. Default is AllPie.
        /// </summary>
        [DefaultValue(ChartPieFillMode.AllPie)]
        public ChartPieFillMode FillMode
        {
            get
            {
                return m_mode;
            }

            set
            {
                if (m_mode != value)
                {
                    m_mode = value;
                    this.RaisePropertyChanged("Mode");
                }
            }
        }

        /// <summary>
        /// Specifies the gradient colors to use in the chart when PieType is set to Custom. Default is null.
        /// </summary>
        [DefaultValue(null)]
        public ColorBlend Gradient
        {
            get
            {
                return m_colorGradient;
            }

            set
            {
                if (m_colorGradient != value)
                {
                    m_colorGradient = value;
                    this.RaisePropertyChanged("Gradient");
                }
            }
        }

        /// <summary>
        /// Specifies whether the height for the pie is determined through the <see cref="HeightCoeficient"/> property or through the <see cref="ChartArea.Depth"/> property. Default is false (uses HeightCoefficient).
        /// </summary>
        [DefaultValue(false)]
        public bool HeightByAreaDepth
        {
            get
            {
                return m_heightByAreaDepth;
            }

            set
            {
                if (m_heightByAreaDepth != value)
                {
                    m_heightByAreaDepth = value;
                    this.RaisePropertyChanged("HeightByAreaDepth");
                }
            }
        }

        /// <summary>
        /// Specifies the style in which the labels are rendered. Default is Outside.
        /// </summary>
        [DefaultValue(ChartAccumulationLabelStyle.Outside)]
        public ChartAccumulationLabelStyle LabelStyle
        {
            get
            {
                return m_labelStyle;
            }

            set
            {
                if (m_labelStyle != value)
                {
                    m_labelStyle = value;
                    this.RaisePropertyChanged("LabelStyle");
                }
            }
        }

        /// <summary>
        /// Specifies the height of the pie as a factor of the radius of the pie. Valid range is 0f - 1f. Default is 0.2f.
        /// </summary>
        [DefaultValue(0.2f)]
        public float HeightCoeficient
        {
            get
            {
                return m_heightCoef;
            }

            set
            {
                if (m_heightCoef != value)
                {
                    m_heightCoef = value;
                    this.RaisePropertyChanged("HeightCoeficient");
                }
            }
        }

        /// <summary>
        /// Specifies the radius of the doughnut hole in the center as a factor of the radius. Default is 0f.
        /// </summary>
        [DefaultValue(0.0f)]
        public float DoughnutCoeficient
        {
            get
            {
                return m_doughnutCoef;
            }

            set
            {
                if (m_doughnutCoef != value)
                {
                    m_doughnutCoef = value;
                    this.RaisePropertyChanged("DoughnutCoeficient");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether series title is displayed.
        /// </summary>
        /// <value><c>true</c> if series title is displayed; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowSeriesTitle
        {
            get 
            {
                return m_showSeriesTitle; 
            }

            set
            {
                if (m_showSeriesTitle != value)
                {
                    m_showSeriesTitle = value;
                    this.RaisePropertyChanged("ShowSeriesTitle");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether databind labels are displayed.
        /// </summary>
        /// <value><c>true</c> if databind labels are displayed; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether databind labels are displayed."), DefaultValue(false)]
        public bool ShowDataBindLabels
        {
            get 
            {
                return m_showDataBindLabels; 
            }

            set
            {
                if (m_showDataBindLabels != value)
                {
                    m_showDataBindLabels = value;
                    this.RaisePropertyChanged("ShowDataBindLabels");
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Radar charts.
    /// </summary>
    public class ChartRadarConfigItem : ChartConfigItem
    {
        #region Members
        private ChartRadarDrawType type = ChartRadarDrawType.Area;
        #endregion

        #region Properties
        /// <summary>
        /// The type of Radar chart to be rendered. Default is Area.
        /// </summary>
        [DefaultValue(ChartRadarDrawType.Area)]
        public ChartRadarDrawType Type
        {
            get
            {
                return type;
            }

            set
            {
                if (this.type != value)
                {
                    type = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ChartRadarConfigItem()
        {
        }
        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Step charts.
    /// </summary>
    public class ChartStepConfigItem : ChartConfigItem
    {
        #region Members
        private bool inverted = false;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies if the step line is inverted. Default is false.
        /// </summary>
        [DefaultValue(false)]
        public bool Inverted
        {
            get
            {
                return inverted;
            }
            set
            {
                if (this.inverted != value)
                {
                    inverted = value;
                    this.RaisePropertyChanged("Inverted");
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ChartStepConfigItem()
        {
        }
        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Funnel charts.
    /// </summary>
    public class ChartFunnelConfigItem : ChartConfigItem
    {
        #region Members
        private ChartFunnelMode m_funnelMode = ChartFunnelMode.YIsHeight;
        private ChartAccumulationLabelPlacement m_labelPlacement = ChartAccumulationLabelPlacement.Right;
        private ChartAccumulationLabelStyle m_labelStyle = ChartAccumulationLabelStyle.Outside;
        private ChartFigureBase m_figureBase = ChartFigureBase.Circle;
        private bool m_showSeriesTitle = false;
        private bool m_showDataBindLabels = false;
        private float m_gapRatio = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies how the Y values should be interpreted. Default is YIsHeight.
        /// </summary>
        [DefaultValue(ChartFunnelMode.YIsHeight)]
        public ChartFunnelMode FunnelMode
        {
            get
            {
                return m_funnelMode;
            }

            set
            {
                if (m_funnelMode != value)
                {
                    m_funnelMode = value;
                    this.RaisePropertyChanged("FunnelMode");
                }
            }
        }

        /// <summary>
        /// Specifies the positioning of the lables in addition to <see cref="LabelStyle"/>. Default is Right.
        /// </summary>
        [DefaultValue(ChartAccumulationLabelPlacement.Right)]
        public ChartAccumulationLabelPlacement LabelPlacement
        {
            get
            {
                return m_labelPlacement;
            }

            set
            {
                if (m_labelPlacement != value)
                {
                    m_labelPlacement = value;
                    this.RaisePropertyChanged("LabelPlacement");
                }
            }
        }

        /// <summary>
        /// Specifies the positioning of the labels in addition to <see cref="LabelPlacement"/>. Default is Outside.
        /// </summary>
        [DefaultValue(ChartAccumulationLabelStyle.Outside)]
        public ChartAccumulationLabelStyle LabelStyle
        {
            get
            {
                return m_labelStyle;
            }

            set
            {

                if (m_labelStyle != value)
                {
                    m_labelStyle = value;
                    this.RaisePropertyChanged("LabelStyle");
                }
            }
        }

        /// <summary>
        /// Specifies the co-efficient for the gap between the blocks. Default is 0.0f.
        /// </summary>
        [DefaultValue(0.0f)]
        public float GapRatio
        {
            get
            {
                return m_gapRatio;
            }

            set
            {

                if (m_gapRatio != value)
                {
                    m_gapRatio = value;
                    this.RaisePropertyChanged("GapRatio");
                }
            }
        }

        /// <summary>
        /// Specifies the type of base for the funnel. Default is Circle.
        /// </summary>
        [DefaultValue(ChartFigureBase.Circle)]
        public ChartFigureBase FigureBase
        {
            get
            {
                return m_figureBase;
            }

            set
            {
                if (m_figureBase != value)
                {
                    m_figureBase = value;
                    this.RaisePropertyChanged("FigureBase");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether series title is displayed.
        /// </summary>
        /// <value><c>true</c> if series title is displayed; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowSeriesTitle
        {
            get
            {
                return m_showSeriesTitle; 
            }

            set
            {
                if (m_showSeriesTitle != value)
                {
                    m_showSeriesTitle = value;
                    this.RaisePropertyChanged("ShowSeriesTitle");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether databind labels are displayed.
        /// </summary>
        /// <value><c>true</c> if databind labels are displayed; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether databind labels are displayed."), DefaultValue(false)]
        public bool ShowDataBindLabels
        {
            get
            {
                return m_showDataBindLabels; 
            }

            set
            {
                if (m_showDataBindLabels != value)
                {
                    m_showDataBindLabels = value;
                    this.RaisePropertyChanged("ShowDataBindLabels");
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Pyramid charts.
    /// </summary>
    public class ChartPyramidConfigItem : ChartConfigItem
    {
        #region Members
        private ChartPyramidMode m_pyramidMode = ChartPyramidMode.Linear;
        private ChartAccumulationLabelPlacement m_labelPlacement = ChartAccumulationLabelPlacement.Right;
        private ChartAccumulationLabelStyle m_labelStyle = ChartAccumulationLabelStyle.Outside;
        private float m_gapRatio = 0;
        private ChartFigureBase m_figureBase = ChartFigureBase.Square;
        private bool m_showSeriesTitle = false;
        private bool m_showDataBindLabels = false;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the mode in which the Y values should be interpreted. Default is Linear.
        /// </summary>
        [DefaultValue(ChartPyramidMode.Linear)]
        public ChartPyramidMode PyramidMode
        {
            get
            {
                return m_pyramidMode;
            }
            set
            {
                if (m_pyramidMode != value)
                {
                    m_pyramidMode = value;
                    this.RaisePropertyChanged("PyramidMode");
                }
            }
        }

        /// <summary>
        /// Specifies the positioning of the labels in addition to the <see cref="LabelStyle"/> property.
        /// </summary>
        [DefaultValue(ChartAccumulationLabelPlacement.Right)]
        public ChartAccumulationLabelPlacement LabelPlacement
        {
            get
            {
                return m_labelPlacement;
            }

            set
            {

                if (m_labelPlacement != value)
                {
                    m_labelPlacement = value;
                    this.RaisePropertyChanged("LabelPlacement");
                }
            }
        }

        /// <summary>
        /// Specifies the positioning of the labels in addition to the <see cref="LabelPlacement"/> property.
        /// </summary>
        [DefaultValue(ChartAccumulationLabelStyle.Outside)]
        public ChartAccumulationLabelStyle LabelStyle
        {
            get
            {
                return m_labelStyle;
            }

            set
            {

                if (m_labelStyle != value)
                {
                    m_labelStyle = value;
                    this.RaisePropertyChanged("LabelStyle");
                }
            }
        }

        /// <summary>
        /// Specifies the co-efficient that determines the gap between the blocks. Default is 0.0f.
        /// </summary>
        [DefaultValue(0.0f)]
        public float GapRatio
        {
            get
            {
                return m_gapRatio;
            }

            set
            {

                if (m_gapRatio != value)
                {
                    m_gapRatio = value;
                    this.RaisePropertyChanged("GapRatio");
                }
            }
        }

        /// <summary>
        /// Specifies the way in which the pyramid base should be rendered in 3D mode. Default is Square.
        /// </summary>
        [DefaultValue(ChartFigureBase.Square)]
        public ChartFigureBase FigureBase
        {
            get
            {
                return m_figureBase;
            }

            set
            {
                if (m_figureBase != value)
                {
                    m_figureBase = value;
                    this.RaisePropertyChanged("FigureBase");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether series title is displayed.
        /// </summary>
        /// <value><c>true</c> if series title is displayed; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowSeriesTitle
        {
            get
            {
                return m_showSeriesTitle; 
            }

            set
            {
                if (m_showSeriesTitle != value)
                {
                    m_showSeriesTitle = value;
                    this.RaisePropertyChanged("ShowSeriesTitle");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether databind labels are displayed.
        /// </summary>
        /// <value><c>true</c> if databind labels are displayed; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether databind labels are displayed."), DefaultValue(false)]
        public bool ShowDataBindLabels
        {
            get 
            {
                return m_showDataBindLabels; 
            }

            set
            {
                if (m_showDataBindLabels != value)
                {
                    m_showDataBindLabels = value;
                    this.RaisePropertyChanged("ShowDataBindLabels");
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Configuration item that pertains to Pyramid charts.
    /// </summary>
    public class ChartHiLoOpenCloseConfigItem : ChartConfigItem
    {
        #region Members
        private ChartOpenCloseDrawMode m_drawMode = ChartOpenCloseDrawMode.Both;
        private Color m_openTipColor = Color.Empty;
        private Color m_closeTipColor = Color.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the mode in which the Y values should be interpreted. Default is Linear.
        /// </summary>
        [DefaultValue(ChartOpenCloseDrawMode.Both)]
        public ChartOpenCloseDrawMode DrawMode
        {
            get
            {
                return m_drawMode;
            }

            set
            {
                if (m_drawMode != value)
                {
                    m_drawMode = value;
                    this.RaisePropertyChanged("DrawMode");
                }
            }
        }
        /// <summary>
        /// Specifies the color for open tip
        /// </summary>
        public Color OpenTipColor
        {
            get
            {
                return m_openTipColor;
            }

            set
            {
                if (m_openTipColor != value)
                {
                    m_openTipColor = value;
                    this.RaisePropertyChanged("OpenTipColor");
                }
            }
        }
        /// <summary>
        /// Specifies the color for close tip
        /// </summary>
        public Color CloseTipColor
        {
            get
            {
                return m_closeTipColor;
            }

            set
            {
                if (m_closeTipColor != value)
                {
                    m_closeTipColor = value;
                    this.RaisePropertyChanged("CloseTipColor");
                }
            }
        }
        #endregion
    }

    /// <summary>
    ///     Configuration item that pertains to Column charts.
    /// </summary>
    public class ChartColumnConfigItem : ChartConfigItem
    {
        #region Members
        private ChartColumnShadingMode shadingMode = ChartColumnShadingMode.PhongCylinder;
        private Color lightColor = Color.White;
        private double lightAngle = -Math.PI / 4;
        private double phongAlpha = 20;
        private ChartColumnType m_columnType = ChartColumnType.Box;
        private SizeF m_cornerRadius = SizeF.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the column type. Default is Box.
        /// </summary>
        [DefaultValue(ChartColumnType.Box)]
        public ChartColumnType ColumnType
        {
            get
            {
                return m_columnType;
            }

            set
            {
                if (m_columnType != value)
                {
                    m_columnType = value;
                    this.RaisePropertyChanged("ColumnType");
                }
            }
        }

        /// <summary>
        /// Specifies the shading mode used for columns or bars.
        /// Default is ChartColumnShadingMode.PhongCylinder.
        /// </summary>
        [DefaultValue(ChartColumnShadingMode.PhongCylinder)]
        public ChartColumnShadingMode ShadingMode
        {
            get
            {
                return shadingMode;
            }

            set
            {
                if (this.shadingMode != value)
                {
                    shadingMode = value;
                    this.RaisePropertyChanged("ShadingMode");
                }
            }
        }

        /// <summary>
        /// Specifies the color of light when ShadingMode is set to PhongCylinder.
        /// </summary>
        [DefaultValue(typeof(Color), "White")]
        public Color LightColor
        {
            get
            {
                return lightColor;
            }

            set
            {

                if (this.lightColor != value)
                {
                    lightColor = value;
                    this.RaisePropertyChanged("LightColor");
                }
            }
        }

        /// <summary>
        /// Specifies the light angle in horizontal plane when ShadingMode is set to PhongCylinder. Default is (-PI/4).
        /// </summary>
        [DefaultValue(-Math.PI / 4)]
        public double LightAngle
        {
            get
            {
                return lightAngle;
            }

            set
            {

                if (this.lightAngle != value)
                {
                    lightAngle = value;
                    this.RaisePropertyChanged("LightAngle");
                }
            }
        }

        /// <summary>
        /// Specifies the Phong's alpha coefficient used for calculation of specular lighting. Default is 20d.
        /// </summary>
        [DefaultValue(20d)]
        public double PhongAlpha
        {
            get
            {
                return phongAlpha;
            }
            set
            {
                if (this.phongAlpha != value)
                {
                    phongAlpha = value;
                    this.RaisePropertyChanged("PhongAlpha");
                }
            }
        }

        /// <summary>
        /// Specifies the radius of round corners. Default is SizeF.Empty.
        /// </summary>
        public SizeF CornerRadius
        {
            get 
            {
                return m_cornerRadius; 
            }

            set
            {
                m_cornerRadius = value;
                this.RaisePropertyChanged("CornerRadius");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ChartColumnConfigItem()
        {
        }
        #endregion
    }

    /// <summary>
    /// Configuration item that pertains to Financial charts.
    /// </summary>
    public class ChartFinancialConfigItem : ChartConfigItem
    {
        #region Members
        private ChartFinancialColorMode m_colorsMode = ChartFinancialColorMode.Fixed;
        private Color m_upColor = Color.Green;
        private Color m_downColor = Color.Red;
        private byte m_darkLightPower = 0x64;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the colors mode.
        /// </summary>
        /// <value>The colors mode.</value>
        [DefaultValue(ChartFinancialColorMode.Fixed)]
        public ChartFinancialColorMode ColorsMode
        {
            get
            {
                return m_colorsMode;
            }

            set
            {

                if (m_colorsMode != value)
                {
                    m_colorsMode = value;
                    this.RaisePropertyChanged("ColorsMode");
                }
            }
        }

        /// <summary>
        /// Specifies the color with which price-up should be indicated. Default is Green.
        /// </summary>
        [DefaultValue(typeof(Color), "Green")]
        public Color PriceUpColor
        {
            get
            {
                return m_upColor;
            }

            set
            {

                if (m_upColor != value)
                {
                    m_upColor = value;
                    this.RaisePropertyChanged("PriceUpColor");
                }
            }
        }

        /// <summary>
        /// Specifies the color with which price-down should be indicated. Default is Red.
        /// </summary>
        [DefaultValue(typeof(Color), "Red")]
        public Color PriceDownColor
        {
            get
            {
                return m_downColor;
            }
            set
            {
                if (m_downColor != value)
                {
                    m_downColor = value;
                    this.RaisePropertyChanged("PriceDownColor");
                }
            }
        }

        /// <summary>
        /// Specifies the difference between the dark and light colors. Default is 0x64.
        /// </summary>
        [DefaultValue(0x64)]
        public byte DarkLightPower
        {
            get
            {
                return m_darkLightPower;
            }
            set
            {
                if (m_darkLightPower != value)
                {
                    m_darkLightPower = value;
                    this.RaisePropertyChanged("DarkLightPower");
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public ChartFinancialConfigItem()
        {
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChartHistogramConfigItem : ChartConfigItem
    {
        #region Members
        private bool m_showNormalDistribution = false;
        private bool m_showDataPoints = true;
        private int m_numberOfIntervals = 10;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether normal distribution is shown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if normal distribution is shown; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool ShowNormalDistribution
        {
            get
            {
                return m_showNormalDistribution; 
            }

            set
            {
                if (m_showNormalDistribution != value)
                {
                    m_showNormalDistribution = value;
                    this.RaisePropertyChanged("ShowNormalDistribution");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data points is shown.
        /// </summary>
        /// <value><c>true</c> if data points is shown; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowDataPoints
        {
            get 
            {
                return m_showDataPoints; 
            }

            set
            {
                if (m_showDataPoints != value)
                {
                    m_showDataPoints = value;
                    this.RaisePropertyChanged("ShowDataPoints");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of intervals.
        /// </summary>
        /// <value>The number of intervals.</value>
        [DefaultValue(10)]
        public int NumberOfIntervals
        {
            get 
            {
                return m_numberOfIntervals; 
            }

            set
            {
                if (m_numberOfIntervals != value)
                {
                    m_numberOfIntervals = value;
                    this.RaisePropertyChanged("NumberOfIntervals");
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChartErrorBarsConfigItem : ChartConfigItem
    {
        #region Members
        private bool m_enabled = false;
        private SizeF m_symbolSize = new SizeF(10, 10);
        private ChartSymbolShape m_symbolShape = ChartSymbolShape.Diamond;
        private ChartOrientation m_orientation = ChartOrientation.Vertical;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the symbol.
        /// </summary>
        /// <value>The size of the symbol.</value>
        public SizeF SymbolSize
        {
            get 
            {
                return m_symbolSize; 
            }

            set
            {
                if (m_symbolSize != null)
                {
                    m_symbolSize = value;
                    this.RaisePropertyChanged("SymbolSize");
                }
            }
        }

        /// <summary>
        /// Gets or sets the symbol shape.
        /// </summary>
        /// <value>The symbol shape.</value>
        public ChartSymbolShape SymbolShape
        {
            get { return m_symbolShape; }
            set
            {
                if (m_symbolShape != value)
                {
                    m_symbolShape = value;
                    this.RaisePropertyChanged("SymbolShape");
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public ChartOrientation Orientation
        {
            get { return m_orientation; }
            set
            {
                if (m_orientation != value)
                {
                    m_orientation = value;
                    this.RaisePropertyChanged("Orientation");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartErrorBarsConfigItem"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                if (m_enabled != value)
                {
                    m_enabled = value;
                    this.RaisePropertyChanged("Enabled");
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Configuration item that pertains to BoxAndWhisker chart.
    /// </summary>
    public sealed class ChartBoxAndWhiskerConfigItem : ChartConfigItem
    {
        #region Members
        private bool m_percentileMode = false;
        private double m_percentile = 0;
        private double m_outlierWidth = 0;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether chart render in percentile mode or in normal mode.  
        /// </summary>
        /// <value><c>true</c> if [percentile mode]; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether chart render in percentile mode or in normal mode."), DefaultValue(false)]
        public bool PercentileMode
        {
            get
            {
                return m_percentileMode;
            }

            set
            {
                if (value != m_percentileMode)
                {
                    m_percentileMode = value;
                    this.RaisePropertyChanged("PercentileMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the percentile. It should be lie between 0.0 to 0.25 . This value decides the outliers in the chart.
        /// </summary>
        /// <value>The percentile.</value>
        [Description("Gets or sets the percentile. It should be lie between 0.0 to 0.25"), DefaultValue(0)]
        public double Percentile
        {
            get
            {
                return m_percentile;
            }

            set
            {
                if (value != m_percentile)
                {
                    m_percentile = value;
                    this.RaisePropertyChanged("Percentile");
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the outlier. Value should be greater than zero and it starts from 1.
        /// </summary>
        /// <value>The width of the outlier.</value>
        [Description("Gets or sets the width of the outlier. Value should be greater than zero and it starts from 1."), DefaultValue(0)]
        public double OutLierWidth
        {
            get
            {
                return m_outlierWidth;
            }

            set
            {
                if (value != m_outlierWidth)
                {
                    m_outlierWidth = value;
                    this.RaisePropertyChanged("OutLierWidth");
                }
            }
        }

        #endregion
    }


	/// <summary>
	/// Configuration item that pertains to HeatMap charts.
	/// </summary>
	public sealed class ChartHeatMapConfigItem : ChartConfigItem
	{
		#region Members
        private bool m_displayColorSwatch = true;
		private Color m_lowestValueColor = Color.Red;
		private Color m_middleValueColor = Color.Yellow;
		private Color m_highestValueColor = Color.Blue;
		private string m_startText = "";
		private string m_endText = "";
		private float m_labelMargins = 2;
		private bool m_displayTitle = true;
		private int m_maximumCharacters = -1;
        private bool m_enableLabelsTruncation = false;
        private bool m_enableLabelRotation = true; 
        private bool m_allowLabelsAutoFit = true;
		private float m_minimumFontSize = 6;
        private bool m_showLargeLabels = false;
		private ChartHeatMapLayoutStyle m_heatMapStyle = ChartHeatMapLayoutStyle.Rectangular;
		#endregion

		#region Proeprties
		/// <summary>
		/// Gets or sets a value indicating whether color swatch is displayed.
		/// </summary>
		/// <value><c>true</c> if color swatch is displayed; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Description("Indicates whether color swatch is displayed")]
        public bool DisplayColorSwatch 
		{
			get { return m_displayColorSwatch ; }
			set 
			{
				if (m_displayColorSwatch != value)
				{
					m_displayColorSwatch = value;
                    this.RaisePropertyChanged("DisplayColorSwatch");
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the lowest value.
		/// </summary>
		/// <value>The color of the lowest value.</value>
		[DefaultValue(typeof(Color), "Red"), Description( "The color of the lowest value") ]
        public Color LowestValueColor
		{
            get { return m_lowestValueColor; }
			set 
			{
                if (m_lowestValueColor != value)
				{
                    m_lowestValueColor = value;
                    this.RaisePropertyChanged("LowestValueColor");
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the middle value.
		/// </summary>
		/// <value>The color of the middle value.</value>
		[DefaultValue(typeof(Color), "Yellow"), Description("The color of the middle value")]
		public Color MiddleValueColor
		{
            get { return m_middleValueColor; }
			set 
			{
                if (m_middleValueColor != value)
				{
                    m_middleValueColor = value;
                    this.RaisePropertyChanged("MiddleValueColor");
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the highest value.
		/// </summary>
		/// <value>The color of the highest value.</value>
		[DefaultValue(typeof(Color), "Blue"), Description("The color of the highest value")]
		public Color HighestValueColor 
		{
            get { return m_highestValueColor; }
			set 
			{
                if (m_highestValueColor != value)
				{
                    m_highestValueColor = value;
                    this.RaisePropertyChanged("HighestValueColor");
				}
			}
		}
		/// <summary>
		/// Gets or sets "from" text.
		/// </summary>
		/// <value>From text.</value>
		[DefaultValue(""), Description(@"The ""start"" text.")]
        public string StartText
		{
			get { return m_startText; }
			set 
			{
				if (m_startText != value)
				{
					m_startText = value;
                    this.RaisePropertyChanged("StartText");
				}
			}
		}
		/// <summary>
		/// Gets or sets "to" text.
		/// </summary>
		/// <value>To text.</value>
		[DefaultValue(""), Description(@"The ""end"" text.")]
		public string EndText
		{
			get { return m_endText; }
			set 
			{
				if (m_endText != value)
				{
					m_endText = value;
                    this.RaisePropertyChanged("EndText");
				}
			}
		}
		/// <summary>
		/// Gets or sets the margins.
		/// </summary>
		/// <value>The margins.</value>
		[DefaultValue(2f), Description("The text margins.")]
        public float LabelMargins
		{
			get { return m_labelMargins; }
			set 
			{
				if (m_labelMargins != value)
				{
					m_labelMargins = value;
                    this.RaisePropertyChanged("LabelMargins");
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether title is displayed.
		/// </summary>
		/// <value><c>true</c> if title is displayed; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Description("Indicates whether title is displayed")]
        public bool DisplayTitle
		{
			get { return m_displayTitle; }
			set 
			{
				if (m_displayTitle != value)
				{
					m_displayTitle = value;
                    this.RaisePropertyChanged("DisplayTitle");
				}
			}
		}
		/// <summary>
		/// Gets or sets the max characters.
		/// </summary>
		/// <value>The max characters.</value>
		[DefaultValue(-1), Description("The maximal number of label characters.")]
		public int MaximumCharacters
		{
            get { return m_maximumCharacters; }
			set
			{
                if (m_maximumCharacters != value)
				{
                    m_maximumCharacters = value;
                    this.RaisePropertyChanged("MaximumCharacters");
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the large labels should be truncated.
		/// </summary>
		/// <value><c>true</c> if the latge labels should be truncated; otherwise, <c>false</c>.</value>
		[DefaultValue(false), Description("Indicates whether the large labels should be truncated.")]
        public bool EnableLabelsTruncation 
		{
            get { return m_enableLabelsTruncation; }
			set
			{
                if (m_enableLabelsTruncation != value)
				{
                    m_enableLabelsTruncation = value;
                    this.RaisePropertyChanged("EnableLabelsTruncation");
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether is allowed to rotation labels.
		/// </summary>
		/// <value><c>true</c> if is allowed to rotation labels; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Description("Indicates whether is allowed to rotation labels")]
        public bool EnableLabelRotation
		{
            get { return m_enableLabelRotation; }
			set
			{
                if (m_enableLabelRotation != value)
				{
                    m_enableLabelRotation = value;
                    this.RaisePropertyChanged("EnableLabelRotation");
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether labels auto fit is enebled.
		/// </summary>
		/// <value><c>true</c> if labels auto fit is enebled; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Description("Indicates whether labels auto fit is enabled")]
        public bool AllowLabelsAutoFit
		{
            get { return m_allowLabelsAutoFit; }
			set
			{
                if (m_allowLabelsAutoFit != value)
				{
                    m_allowLabelsAutoFit = value;
                    this.RaisePropertyChanged("AllowLabelsAutoFit");
				}
			}
		}
		/// <summary>
		/// Gets or sets the minimal size of the font.
		/// </summary>
		/// <value>The minimal size of the font.</value>
		[DefaultValue(6f), Description("The minimal size of the font")]
        public float MinimumFontSize
		{
            get { return m_minimumFontSize; }
			set
			{
                if (m_minimumFontSize != value)
				{
                    m_minimumFontSize = value;
                    this.RaisePropertyChanged("MinimumFontSize");
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the large labels should be hiden.
		/// </summary>
		/// <value><c>true</c> if the large labels should be hiden; otherwise, <c>false</c>.</value>
		[DefaultValue(false), Description("Indicates whether the large labels should be display")]
        public bool ShowLargeLabels
		{
            get { return m_showLargeLabels; }
			set
			{
                if (m_showLargeLabels != value)
				{
                    m_showLargeLabels = value;
                    this.RaisePropertyChanged("ShowLargeLabels");
				}
			}
		}
		/// <summary>
		/// Gets or sets the layout style.
		/// </summary>
		/// <value>The layout style.</value>
		[DefaultValue(ChartHeatMapLayoutStyle.Rectangular), Description("The HeatMap layout style.")]
        public ChartHeatMapLayoutStyle HeatMapStyle
		{
			get { return m_heatMapStyle; }
			set
			{
				if (m_heatMapStyle != value)
				{
					m_heatMapStyle = value;
                    this.RaisePropertyChanged("HeatMapStyle");
				}
			}
		}
		#endregion
	}

    /// <summary>
    /// Configuration Line that pertains to Line,Spline charts.
    /// </summary>
    public class ChartLineConfigItem : ChartConfigItem
    {
        #region Members
        private bool m_disableLineRegion = false;
        private bool m_disableLineCap = false;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Line cap is enabled or disabled for drawing Line series.  
        /// </summary>
        /// <value><c>true</c> if [disable LineCap]; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether Line cap is enabled or disabled for drawing Line series."), DefaultValue(false)]
        public bool DisableLineCap
        {
            get
            {
                return m_disableLineCap;
            }

            set
            {
                if (value != m_disableLineCap)
                {
                    m_disableLineCap = value;
                    this.RaisePropertyChanged("DisableLineCap");
                }
            }
        }

        /// <summary>
        /// Enable/Disable the Line segement(line between two points) for Line and Spline type.Default value is false.
        /// </summary>
         public bool DisableLineRegion
        {
            get
            {
                return m_disableLineRegion;
            }

            set
            {
              
                    m_disableLineRegion = value;
                    this.RaisePropertyChanged("DisableLineRegion");
                 
            }
        }
        
        #endregion
    }
    #endregion

    #region Config Items Collection
    /// <summary>
    /// Collection of Configuration Items. These items store datas that can be used by the chart and its elements in any
    /// manner.
    /// <seealso cref="ChartConfigItem"/>
    /// </summary>
    /// <remarks>
    /// Pre-defined configuration items may be accessed as shown below.
    /// <code lang="C#">
    /// // access the RadarItem ConfigItem to configure radar charts
    /// series.ConfigItems.RadarItem.Type = ChartRadarDrawType.Symbol;
    /// </code>
    /// </remarks>
    public sealed class ChartSeriesConfigCollection : DictionaryBase
    {
        #region Constants
        /// <summary>
        ///     Standard identifier for Bubble chart configuration information.
        /// </summary>
        public const string BubbleItemName = "BubbleConfigItem";

        /// <summary>
        ///     Standard identifier for Pie chart configuration information.
        /// </summary>
        public const string PieItemName = "PieConfigItem";

        /// <summary>
        ///		Standard identifier for Radar chart configuration information.
        /// </summary>
        public const string RadarItemName = "RadarConfigItem";

        /// <summary>
        ///		Standard identifier for Step chart configuration information.
        /// </summary>
        public const string StepItemName = "StepConfigItem";

        /// <summary>
        ///		Standard identifier for Column chart configuration information.
        /// </summary>
        public const string ColumnItemName = "ColumnConfigItem";

        /// <summary>
        ///		Standard identifier for Column chart configuration information.
        /// </summary>
        public const string FunnelItemName = "FunnelConfigItem";

        /// <summary>
        ///		Standard identifier for Column chart configuration information.
        /// </summary>
        public const string PyramidItemName = "PyramidConfigItem";

        /// <summary>
        ///		Standard identifier for financial charts configuration information.
        /// </summary>
        public const string FinancialItemName = "FinancialConfigItem";

        /// <summary>
        ///		Standard identifier for Gantt chart configuration information.
        /// </summary>
        public const string GanttItemName = "GanttConfigItem";

        /// <summary>
        ///		Standard identifier for Gantt chart configuration information.
        /// </summary>
        public const string HiLoOpenCloseItemName = "HiLoOpenCloseConfigItem";

        /// <summary>
        ///	 Standard identifier for Histogram chart configuration information.
        /// </summary>
        public const string HistogramItemName = "HistogramConfigItem";

        /// <summary>
        ///	Standard identifier for Histogram chart configuration information.
        /// </summary>
        public const string ErrorBarsItemName = "ErrorBarsConfigItem";
		/// <summary>
		///		Standard identifier for HeatMap chart configuration information.
		/// </summary>
		public const string HeatMapItemName = "HeatMapConfigItem";

        /// <summary>
        ///		Standard identifier for BoxAndWhisker chart configuration information.
        /// </summary>
        public const string BoxAndWhiskerItemName = "BoxAndWhiskerConfigItem";
        /// <summary>
        ///		Standard identifier for Line and Spline chart configuration information.
        /// </summary>
        public const string LineSegmentName = "LineConfigItem";
        #endregion

        #region Members
        private ChartColumnConfigItem m_columnItem = null;
        private ChartPieConfigItem m_pieItem = null;
        private ChartRadarConfigItem m_radarItem = null;
        private ChartStepConfigItem m_stepItem = null;
        private ChartFunnelConfigItem m_funnelItem = null;
        private ChartPyramidConfigItem m_pyramidItem = null;
        private ChartFinancialConfigItem m_financialItem = null;
        private ChartGanttConfigItem m_ganttItem = null;
        private ChartHiLoOpenCloseConfigItem m_hiLoOpenCloseItem = null;
        private ChartHistogramConfigItem m_histogramItem = null;
        private ChartBubbleConfigItem m_bubbleItem = null;
        private ChartErrorBarsConfigItem m_errorBars = null;
		private ChartHeatMapConfigItem m_heatMapItem = null;
        private ChartLineConfigItem m_linesegment = null;
        private ChartBoxAndWhiskerConfigItem m_BoxAndWhiskerItem = null;
        private ChartLineConfigItem m_LineItem = null;
        #endregion

        #region Events
        /// <summary>
        ///     Event that is raised when configuration information is changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion

        #region Properties
        /// <summary>
        ///     Standard configuration information for Bubble charts.
        /// </summary>
        public ChartBubbleConfigItem BubbleItem
        {
            get
            {
                return m_bubbleItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for Pie charts.
        /// </summary>
        public ChartPieConfigItem PieItem
        {
            get
            {
                return m_pieItem;
            }
        }
        
        /// <summary>
        ///     Standard configuration information for Radar charts.
        /// </summary>
        public ChartRadarConfigItem RadarItem
        {
            get
            {
                return m_radarItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for Step charts.
        /// </summary>
        public ChartStepConfigItem StepItem
        {
            get
            {
                return m_stepItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for Step charts.
        /// </summary>
        public ChartColumnConfigItem ColumnItem
        {
            get
            {
                return m_columnItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for Funnel charts.
        /// </summary>
        public ChartFunnelConfigItem FunnelItem
        {
            get
            {
                return m_funnelItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for Pyramid charts.
        /// </summary>
        public ChartPyramidConfigItem PyramidItem
        {
            get
            {
                return m_pyramidItem;
            }
        }

        /// <summary>
        ///     Standard configuration information for financial charts.
        /// </summary>
        public ChartFinancialConfigItem FinancialItem
        {
            get
            {
                return m_financialItem;
            }
        }

        /// <summary>
        /// Standard configuration information for Gantt chart.
        /// </summary>
        public ChartGanttConfigItem GanttItem
        {
            get
            {
                return m_ganttItem;
            }
        }

        /// <summary>
        ///  Standard configuration information for HiLoOpenClose chart.
        /// </summary>
        public ChartHiLoOpenCloseConfigItem HiLoOpenCloseItem
        {
            get
            {
                return m_hiLoOpenCloseItem;
            }
        }

        /// <summary>
        ///  Standard configuration information for Histogram chart.
        /// </summary>
        public ChartHistogramConfigItem HistogramItem
        {
            get
            {
                return m_histogramItem;
            }
        }

        /// <summary>
		///  Standard configuration information for error bars.
        /// </summary>
		/// <value>The error bars.</value>
        public ChartErrorBarsConfigItem ErrorBars
        {
            get
            {
                return m_errorBars;
            }
        }

        /// <summary>
        ///  Standard configuration information for BoxAndWhisker chart.
        /// </summary>     
        public ChartBoxAndWhiskerConfigItem BoxAndWhiskerItem 
        {
            get
            {
                return m_BoxAndWhiskerItem;
            }
        }
		/// <summary>
		/// Standard configuration information for HeatMap chart.
		/// </summary>
		/// <value>The heat map item.</value>
		public ChartHeatMapConfigItem HeatMapItem
		{
			get
			{
				return m_heatMapItem;
			}
		}
        /// <summary>
        ///     Standard configuration information for Bubble charts.
        /// </summary>
        public ChartLineConfigItem LineSegment 
        {
            get
            {
                return m_linesegment;
            }
        }
        /// <summary>
        ///  Standard configuration information for Line chart.
        /// </summary>     
        public ChartLineConfigItem LineItem
        {
            get
            {
                return m_LineItem;
            }
        }        

        /// <summary>
        ///     Looks up the collection by name and returns the configuration item.
        /// </summary>
        public ChartConfigItem this[string name]
        {
            get
            {
                return this.Dictionary[name] as ChartConfigItem;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChartSeriesConfigCollection()
        {
            this.Add(ChartSeriesConfigCollection.BubbleItemName, new ChartBubbleConfigItem());
            this.Add(ChartSeriesConfigCollection.PieItemName, new ChartPieConfigItem());
            this.Add(ChartSeriesConfigCollection.RadarItemName, new ChartRadarConfigItem());
            this.Add(ChartSeriesConfigCollection.StepItemName, new ChartStepConfigItem());
            this.Add(ChartSeriesConfigCollection.ColumnItemName, new ChartColumnConfigItem());
            this.Add(ChartSeriesConfigCollection.FunnelItemName, new ChartFunnelConfigItem());
            this.Add(ChartSeriesConfigCollection.PyramidItemName, new ChartPyramidConfigItem());
            this.Add(ChartSeriesConfigCollection.FinancialItemName, new ChartFinancialConfigItem());
            this.Add(ChartSeriesConfigCollection.GanttItemName, new ChartGanttConfigItem());
            this.Add(ChartSeriesConfigCollection.HiLoOpenCloseItemName, new ChartHiLoOpenCloseConfigItem());
            this.Add(ChartSeriesConfigCollection.HistogramItemName, new ChartHistogramConfigItem());
            this.Add(ChartSeriesConfigCollection.ErrorBarsItemName, new ChartErrorBarsConfigItem());
			this.Add(ChartSeriesConfigCollection.HeatMapItemName, new ChartHeatMapConfigItem());
            this.Add(ChartSeriesConfigCollection.BoxAndWhiskerItemName, new ChartBoxAndWhiskerConfigItem());
            this.Add(ChartSeriesConfigCollection.LineSegmentName, new ChartLineConfigItem());
        }
        #endregion

        #region Public methods
        /// <summary>
        ///     Adds the specified configuration item to this collection, with the specified name used for referencing it.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     Name to be used for referencing the specified configuration item.
        ///     </para>
        /// </param>
        /// <param name="item" type="Syncfusion.Windows.Forms.Chart.ChartConfigItem">
        ///     <para>
        ///     Configuration Item to be added.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A void value.
        /// </returns>
        public void Add(string name, ChartConfigItem item)
        {
            this.Dictionary.Add(name, item);
        }

        /// <summary>
        ///     Removes specified configuration item from the collection.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     Reference name of the item to be removed.
        ///     </para>
        /// </param>
        public void Remove(string name)
        {
            this.Dictionary.Remove(name);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.DictionaryBase"/> instance.
        /// </summary>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnInsertComplete(object key, object value)
        {
            this.ChangeConfig(key.ToString(), value);

            this.BroadcastChange();
            this.WireItem(value);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnRemove(object key, object value)
        {
            this.UnwireItem(value);
        }
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnRemoveComplete(object key, object value)
        {
            this.BroadcastChange();
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnSet(object key, object oldValue, object newValue)
        {
            this.UnwireItem(oldValue);
        }
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnSetComplete(object key, object oldValue, object newValue)
        {
            this.ChangeConfig(key.ToString(), newValue);

            this.BroadcastChange();
            this.WireItem(newValue);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnClear()
        {
            foreach (ChartConfigItem item in this.Dictionary.Values)
            {
                this.UnwireItem(item);
            }

            m_columnItem = null;
            m_pieItem = null;
            m_radarItem = null;
            m_stepItem = null;
            m_funnelItem = null;
            m_pyramidItem = null;
            m_financialItem = null;
            m_ganttItem = null;
            m_hiLoOpenCloseItem = null;
            m_histogramItem = null;
            m_bubbleItem = null;
            m_LineItem = null;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnClearComplete()
        {
            this.BroadcastChange();
        }

        /// <summary>
        /// Connects the event handlers.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void WireItem(object value)
        {
            ChartConfigItem item = value as ChartConfigItem;
            item.PropertyChanged += new SyncfusionPropertyChangedEventHandler(this.OnContainedObjectPropertyChanged);
        }

        /// <summary>
        /// Unconnects the event handlers.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void UnwireItem(object value)
        {
            ChartConfigItem item = value as ChartConfigItem;
            item.PropertyChanged -= new SyncfusionPropertyChangedEventHandler(this.OnContainedObjectPropertyChanged);
        }

        /// <summary>
        /// Called when property of contained object is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void OnContainedObjectPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs args)
        {
            this.BroadcastChange();
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void BroadcastChange()
        {
            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Changes the config.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new config.</param>
        private void ChangeConfig(string key, object newValue)
        {
            switch (key)
            {
                case ChartSeriesConfigCollection.BubbleItemName:
                    m_bubbleItem = newValue as ChartBubbleConfigItem;
                    break;
                case ChartSeriesConfigCollection.PieItemName:
                    m_pieItem = newValue as ChartPieConfigItem;
                    break;
                case ChartSeriesConfigCollection.RadarItemName:
                    m_radarItem = newValue as ChartRadarConfigItem;
                    break;
                case ChartSeriesConfigCollection.StepItemName:
                    m_stepItem = newValue as ChartStepConfigItem;
                    break;
                case ChartSeriesConfigCollection.ColumnItemName:
                    m_columnItem = newValue as ChartColumnConfigItem;
                    break;
                case ChartSeriesConfigCollection.FunnelItemName:
                    m_funnelItem = newValue as ChartFunnelConfigItem;
                    break;
                case ChartSeriesConfigCollection.PyramidItemName:
                    m_pyramidItem = newValue as ChartPyramidConfigItem;
                    break;
                case ChartSeriesConfigCollection.FinancialItemName:
                    m_financialItem = newValue as ChartFinancialConfigItem;
                    break;
                case ChartSeriesConfigCollection.GanttItemName:
                    m_ganttItem = newValue as ChartGanttConfigItem;
                    break;
                case ChartSeriesConfigCollection.HiLoOpenCloseItemName:
                    m_hiLoOpenCloseItem = newValue as ChartHiLoOpenCloseConfigItem;
                    break;
                case ChartSeriesConfigCollection.HistogramItemName:
                    m_histogramItem = newValue as ChartHistogramConfigItem;
                    break;
                case ChartSeriesConfigCollection.ErrorBarsItemName:
                    m_errorBars = newValue as ChartErrorBarsConfigItem;
                    break;
				case ChartSeriesConfigCollection.HeatMapItemName:
					m_heatMapItem = newValue as ChartHeatMapConfigItem;
					break;
                case ChartSeriesConfigCollection.BoxAndWhiskerItemName: 
                    m_BoxAndWhiskerItem = newValue as ChartBoxAndWhiskerConfigItem;
                    break;
                case ChartSeriesConfigCollection.LineSegmentName:
                    m_linesegment = newValue as ChartLineConfigItem;
                    m_LineItem = newValue as ChartLineConfigItem;
                    break;
            }
        }
        #endregion
    }
    #endregion
}