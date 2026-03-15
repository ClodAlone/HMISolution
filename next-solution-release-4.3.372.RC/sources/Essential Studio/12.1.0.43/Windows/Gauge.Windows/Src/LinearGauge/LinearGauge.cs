#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Collections;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Gauge
{
    [Docking(DockingBehavior.Ask), ToolboxItem(true),
    ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Gauge.LinearGauge), "ToolboxIcons.LinearGauge.png")]
    [Designer(typeof(LinearGaugeDesigner))]
    public partial class LinearGauge : Control
    {
        #region Private variables

        private int m_MajorDifference = 20;
        private Color m_BackgroundGradientStartColor;
        private Color m_BackgroundGradientEndColor;
        private Color m_OuterFrameGradientStartColor;
        private Color m_OuterFrameGradientEndColor;
        private Color m_InnerFrameGradientStartColor;
        private Color m_InnerFrameGradientEndColor;
        private Color m_MajorLinesColor;
        private Color m_MinorTickMarkColor;
        private Color m_GaugeBaseColor;
        private Color m_NeedleColor;
        private Color m_ValueIndicatorColor;
        private Color m_ScaleLabelColor;
        private Font m_GaugelabelFont;
        private Color m_GaugeValueColor;
        private ThemeStyle visualStyle;
        private Placement _pointerPlacement;
        private LinearFrameType frameType;
        private int m_MajorTickMarkHeight;
        private int _minorTicksCount;
        private int m_MinorTicksHeight;
        private int m_Width, m_Height, t_width, t_height;
        private float majorTicksDistance;
        private int m_majorTicksCount;
        private float m_minorTicksDistance;
        private float m_minorTicksPixels;
        private int startpoint;
        private Single m_value;
        private Single m_MinValue;
        private Single m_MaxValue;
        private StringFormat sf;

        #endregion 

        #region DataVariables
        private ListChangedEventHandler listChangedHandler;
        private EventHandler positionChangedHandler;
        private object dataSource;
        private string dataMember;
        private CurrencyManager dataManager;
        private ListView list;
        #endregion

        #region Constructor and Initialization
        /// <summary>
        /// Initializes a new instance of LinearGauge class.
        /// </summary>
        public LinearGauge()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(LinearGauge));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            InitializeComponent();
            Initializeproperties();
        }

        /// <summary>
        /// Initializes a basic instance of LinearGauge class.
        /// </summary>
        private void InitializeComponent()
		{
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            this.SuspendLayout();
            this.Name = "LinearGauge";
            m_value = 0;
            m_MinValue = 0;
            m_MaxValue = 120;
			this.ForeColor = System.Drawing.Color.Gray;
			this.Size = new System.Drawing.Size(300, 125);
            _GaugeRanges = new LinearRangeCollection(this);
			this.ResumeLayout(false);
            list = new ListView();
            GaugelabelFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
		}
        /// <summary>
        /// Initializes a new instance of LinearGauge class.
        /// </summary>
        private void Initializeproperties()
        {
            m_BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
            m_BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
            m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
            m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
            m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
            m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
            m_MajorLinesColor = Color.Gray;
            m_MinorTickMarkColor = Color.Gray;
            m_GaugeBaseColor = Color.Gray;
            m_NeedleColor = Color.Gray;
            m_ValueIndicatorColor = Color.Gray;
            m_ScaleLabelColor = Color.Gray;
            m_GaugelabelFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = Color.Gray;
            m_GaugeValueColor = Color.Gray;
            _pointerPlacement = Placement.Center;
            frameType = LinearFrameType.Horizontal;
            m_MajorTickMarkHeight = 15;
            _minorTicksCount = 5;
            m_MinorTicksHeight = 10;
            m_Width = m_Height = t_width = t_height = 0;
            majorTicksDistance = 0;
            m_majorTicksCount = 3;
            m_minorTicksDistance = 0;
            m_minorTicksPixels = 0;
            startpoint = 25;
            sf = new StringFormat();
            visualStyle = ThemeStyle.None;
        }

        #endregion

        #region ShouldSerialize

        protected bool ShouldSerializeBackgroundGradientEndColor()
        {
            return BackgroundGradientEndColor != Color.FromArgb(210, 210, 210);
        }

        protected bool ShouldSerializeBackgroundGradientStartColor()
        {
            return BackgroundGradientStartColor != Color.FromArgb(240, 240, 240);
        }

        protected bool ShouldSerializeOuterFrameGradientStartColor()
        {
            return OuterFrameGradientStartColor != Color.FromArgb(229, 229, 229);
        }

        protected bool ShouldSerializeOuterFrameGradientEndColor()
        {
            return OuterFrameGradientEndColor != Color.FromArgb(172, 172, 172);
        }

        protected bool ShouldSerializeInnerFrameGradientStartColor()
        {
            return InnerFrameGradientStartColor != Color.FromArgb(180, 180, 180);
        }

        protected bool ShouldSerializeInnerFrameGradientEndColor()
        {
            return InnerFrameGradientEndColor != Color.FromArgb(194, 194, 194);
        }

        protected bool ShouldSerializeVisualStyle()
        {
            return VisualStyle != ThemeStyle.None;
        }

        protected bool ShouldSerializeGaugeBaseColor()
        {
            return GaugeBaseColor != Color.Gray;
        }

        protected bool ShouldSerializeNeedleColor()
        {
            return NeedleColor != Color.Gray;
        }

        protected bool ShouldSerializeMajorTickMarkColor()
        {
            return MajorTickMarkColor != Color.Gray;
        }

        protected bool ShouldSerializeMinorTickMarkColor()
        {
            return MinorTickMarkColor != Color.Gray;
        }

        protected bool ShouldSerializeScaleLabelColor()
        {
            return ScaleLabelColor != Color.Gray;
        }

        protected bool ShouldSerializeMajorTicksHeight()
        {
            return MajorTicksHeight != 10;
        }

        protected bool ShouldSerializeMinorTickHeight()
        {
            return MinorTickHeight != 5;
        }

        protected bool ShouldSerializeMajorDifference()
        {
            return MajorDifference != 20;
        }

        protected bool ShouldSerializeValue()
        {
            return Value != 0;
        }

        protected bool ShouldSerializeMaximumValue()
        {
            return MaximumValue != 120;
        }

        protected bool ShouldSerializeMinimumValue()
        {
            return MinimumValue != 0;
        }

        protected bool ShouldSerializeFrameType()
        {
            return LinearFrameType != LinearFrameType.Horizontal;
        }

        protected bool ShouldSerializeTickPlacement()
        {
            return PointerPlacement != Placement.Center;
        }
        
        protected bool ShouldSerializeDisplayRecordIndex()
        {
            return DisplayRecordIndex != 0;
        }

        protected bool ShouldSerializeValueIndicatorColorColor()
        {
            return ValueIndicatorColor != Color.Gray;
        }

        protected bool ShouldSerializeShowNeedle()
        {
            return ShowNeedle != true;
        }

        protected bool ShouldSerializeShowScaleLabel()
        {
            return ShowScaleLabel != true;
        }

        protected void ResetBackgroundGradientEndColor()
        {
            BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
        }

        protected void ResetBackgroundGradientStartColor()
        {
            BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
        }

        protected void ResetOuterFrameGradientStartColor()
        {
            OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
        }

        protected void ResetOuterFrameGradientEndColor()
        {
            OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
        }

        protected void ResetInnerFrameGradientStartColor()
        {
            InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
        }

        protected void ResetInnerFrameGradientEndColor()
        {
            InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
        }

        protected void ResetVisualStyle()
        {
            VisualStyle = ThemeStyle.None;
        }

        protected void ResetGaugeBaseColor()
        {
            GaugeBaseColor = Color.Gray;
        }

        protected void ResetNeedleColor()
        {
            NeedleColor = Color.Gray;
        }

        protected void ResetMajorTickMarkColor()
        {
            MajorTickMarkColor = Color.Gray;
        }

        protected void ResetMinorTickMarkColor()
        {
            MinorTickMarkColor = Color.Gray;
        }        

        protected void ResetScaleLabelColor()
        {
            ScaleLabelColor = Color.Gray;
        }        

        protected void ResetMajorTickMarkHeight()
        {
           MajorTicksHeight = 10;
        }

        protected void ResetMinorTickMarkHeight()
        {
           MinorTickHeight = 5;
        }        

        protected void ResetMajorDifference()
        {
            MajorDifference = 20;
        }
        
        protected void ResetValue()
        {
            Value = 0;
        }

        protected void ResetMaximumValue()
        {
            MaximumValue = 120;
        }

        protected void ResetMinimumValue()
        {
            MinimumValue = 0;
        }  

        protected void ResetFrameType()
        {
            LinearFrameType = LinearFrameType.Horizontal;
        }

        protected void ResetPointerPlacement()
        {
            PointerPlacement = Placement.Center;
        }

        protected void ResetDisplayRecordIndex()
        {
            DisplayRecordIndex = 0;
        }

        protected void ResetValueIndicatorColor()
        {
            ValueIndicatorColor = Color.Gray;
        }

        protected void ResetShowNeedle()
        {
            ShowNeedle = true;
        }


        # endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating the Frame Type of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets a value indicating the Frame Type of the gauge.")
        ]
        public LinearFrameType LinearFrameType
        {
            get
            {
                return frameType;
            }
            set
            {
                frameType = value;
                ChangeFrame();
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating the pointer placement the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets a value indicating the pointer placement the gauge.")
        ]
        public Placement PointerPlacement
        {
            get
            {
                return _pointerPlacement;
            }
            set
            {
                _pointerPlacement = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the minimum value to display on the LinearGauge
        /// </summary>
        [
         Category("Data"),
        Description("Gets or Sets the minimum value to display on the LinearGauge.")
        ]
        public Single MinimumValue
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                if ((m_MinValue != value) && (value < m_MaxValue))
                {
                    m_MinValue = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or Sets the maximum value to display on the LinearGauge
        /// </summary>
        [
        Category("Data"),
        Description("Gets or Sets the maximum value to display on the LinearGauge.")
        ]
        public Single MaximumValue
        {
            get
            {
                return m_MaxValue;
            }
            set
            {
                if ((m_MaxValue != value) && (value > m_MinValue))
                {
                    m_MaxValue = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or Sets a value to divide the ticks from minimum to maximum value.
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to divide the ticks from minimum to maximum value.")
        ]
        public int MajorDifference
        {
            get
            {
                return m_MajorDifference;
            }
            set
            {
                if (value != m_MajorDifference && value > 0)
                {
                    m_MajorDifference = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or Sets a value to draw number of ticks in the gauge.
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to draw number of ticks in the gauge.")
        ]
		internal int MajorTicksCount
		{
            get
            {
                return m_majorTicksCount;
            }
			set
			{
				if (value!=m_majorTicksCount && value>0)
				{
					m_majorTicksCount = value;
					this.Invalidate();
				}
			}
		}
        /// <summary>
        /// Gets or sets the background color of this component.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the background color of this component."),
          Browsable(false)
        ]
        public override Color BackColor
        {
            get
            {
                return Color.Transparent;
            }
        }
        /// <summary>
        /// Specifies the background image for the control
        /// </summary>
        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
        }

        /// <summary>
        /// Specifies the image layout for the background image of control.
        /// </summary>
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
        }

        /// <summary>
        /// Gets or Sets the start color of the gradient inner background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the start color of the gradient inner background.")
        ]
        public Color BackgroundGradientStartColor
        {
            get
            {
                return m_BackgroundGradientStartColor;
            }
            set
            {
                if (m_BackgroundGradientStartColor != value)
                {
                    m_BackgroundGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the end color of the gradient inner background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the end color of the gradient inner background.")
        ]
        public Color BackgroundGradientEndColor
        {
            get
            {
                return m_BackgroundGradientEndColor;
            }
            set
            {
                if (m_BackgroundGradientEndColor != value)
                {
                    m_BackgroundGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the start gradient color for the inner frame.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the start gradient color for the inner frame.")
        ]
        public Color OuterFrameGradientStartColor
        {
            get
            {
                return m_OuterFrameGradientStartColor;
            }
            set
            {
                if (m_OuterFrameGradientStartColor != value)
                {
                    m_OuterFrameGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the end gradient color for the outer frame.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the end gradient color for the outer frame.")
        ]
        public Color OuterFrameGradientEndColor
        {
            get
            {
                return m_OuterFrameGradientEndColor;
            }
            set
            {
                if (m_OuterFrameGradientEndColor != value)
                {
                    m_OuterFrameGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }


        /// <summary>
        /// Gets or Sets the start gradient color for the Frame Border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the start gradient color for the Frame Border.")
        ]
        public Color InnerFrameGradientStartColor
        {
            get
            {
                return m_InnerFrameGradientStartColor;
            }
            set
            {
                if (m_InnerFrameGradientStartColor != value)
                {
                    m_InnerFrameGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the end gradient color for the Frame Border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the end gradient color for the Frame Border.")
        ]
        public Color InnerFrameGradientEndColor
        {
            get
            {
                return m_InnerFrameGradientEndColor;
            }
            set
            {
                if (m_InnerFrameGradientEndColor != value)
                {
                    m_InnerFrameGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Major Lines color of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets a value indicating the Major Lines color of the gauge.")
        ]
        public Color MajorTickMarkColor
        {
            get { return m_MajorLinesColor; }
            set
            {
                if (m_MajorLinesColor != value)
                {
                    m_MajorLinesColor = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Minor Lines color of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets a value indicating the Minor Lines color of the gauge.")
        ]
        public Color MinorTickMarkColor
        {
            get
            {
                return m_MinorTickMarkColor;
            }
            set
            {
                if (m_MinorTickMarkColor != value)
                {
                    m_MinorTickMarkColor = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Gauge base Line Color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or sets a value indicating the Gauge base Line Color of the gauge.")
        ]
        public Color GaugeBaseColor
        {
            get
            {
                return m_GaugeBaseColor;
            }
            set
            {
                if (m_GaugeBaseColor != value)
                {
                    m_GaugeBaseColor = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets the foreground color of this component which used to display the text.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the foreground color of this component which used to display the text."),
          Browsable(true)
        ]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Specifies font for GaugeLabel.
        /// </summary>
        [
            Category("Appearance"),
            Description("Specifies font for GaugeLabel")
        ]
        public Font GaugelabelFont
        {
            get
            {
                return m_GaugelabelFont;
            }
            set
            {
                m_GaugelabelFont = value;
                Refresh();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Needle Color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or sets a value indicating the Needle Color of the gauge.")
        ]
        public Color ValueIndicatorColor
        {
            get
            {
                return m_ValueIndicatorColor;
            }
            set
            {
                if (m_ValueIndicatorColor != value)
                {
                    m_ValueIndicatorColor = value;

                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Needle Color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or sets a value indicating the Needle Color of the gauge.")
        ]
        public Color NeedleColor
        {
            get
            {
                return m_NeedleColor;
            }
            set
            {
                if (m_NeedleColor != value)
                {
                    m_NeedleColor = value;

                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Numbers color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or Sets the color for numeric labels in Linear scale.")
        ]
        public Color ScaleLabelColor
        {
            get
            {
                return m_ScaleLabelColor;
            }
            set
            {
                if (m_ScaleLabelColor != value)
                {
                    m_ScaleLabelColor = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Specifies the visual style.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the visual style.")
        ]
        private bool m_ShowNeedle = true;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide gauge needle.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide gauge needle.
        /// </summary>
        public bool ShowNeedle
        {
            get
            {
                return m_ShowNeedle;
            }
            set
            {
                m_ShowNeedle = value;

                this.Refresh();
            }
        }
        private bool m_ShowScaleLabel = true;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide gauge label.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide gauge label.
        /// </summary>
        public bool ShowScaleLabel
        {
            get
            {
                return m_ShowScaleLabel;
            }
            set
            {
                m_ShowScaleLabel = value;

                this.Refresh();
            }
        }
        /// <summary>
        /// Specifies the visual style.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the visual style.")
        ]
        public ThemeStyle VisualStyle
        {
            get
            {
                return visualStyle;
            }
            set
            {
                visualStyle = value;
                SetVisualStyle(value);
                this.Refresh();

            }
        }

        /// <summary>
        /// Gets or Sets a value to specify the maximum height for the major ticks.
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to specify the maximum height for the major ticks.")
        ]
		public int MajorTicksHeight
		{
            get
            {
                return m_MajorTickMarkHeight;
            }
			set
			{
				if (value!=m_MajorTickMarkHeight && value>-1)
				{
					m_MajorTickMarkHeight = value;
					this.Invalidate();
				}
			}
		}

        /// <summary>
        /// Gets or Sets a value to specify the number of minor ticks to be drawn in the control.
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to specify the number of minor ticks to be drawn in the control.")
        ]
		public int MinorTickCount
		{
            get
            {
                return _minorTicksCount;
            }
			set	{
				if (value!=_minorTicksCount && value>-1 )
				{
					_minorTicksCount = value;
					this.Invalidate();
				}
			}
		}


        /// <summary>
        /// Gets or Sets a value to specify the maximum height for the minor ticks.
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to specify the maximum height for the minor ticks.")
        ]
        
		public int MinorTickHeight
		{
            get
            {
                return m_MinorTicksHeight;
            }
			set	{
				if (value!=m_MinorTicksHeight && value>-1)
				{
					m_MinorTicksHeight = value;
					this.Invalidate();
				}
			}
		}
       
        /// <summary>
        /// Gets or Sets the value to be displayed in the Gauge
        /// </summary>
        [
        Category("Data"),
        Description("Gets or Sets the value to be displayed in the Gauge.")
       ]
        public Single Value
        {
            get
            {
                return m_value;
            }
            set
            {
                value = Math.Min(Math.Max(value, m_MinValue), m_MaxValue);
                if (m_value != value)
                {
                    m_value = value;
                    OnValueChanged();

                    foreach (LinearRange ptrRange in _GaugeRanges)
                    {
                        if ((m_value >= ptrRange.StartValue)
                            && (m_value <= ptrRange.EndValue))
                        {
                            //Entering Range
                            if (!ptrRange.InRange)
                            {
                                ptrRange.InRange = true;
                                OnThresholdValueChanged(ptrRange, m_value);
                            }
                        }
                        else
                        {
                            //Leaving Range
                            if (ptrRange.InRange)
                            {
                                ptrRange.InRange = false;
                                OnThresholdValueChanged(ptrRange, m_value);
                            }
                        }
                    }
                    Refresh();
                }
            }
        }

        #endregion 

        #region Helpers

        /// <summary>
        /// Used to change the frame
        /// </summary>
        private void ChangeFrame()
        {
            if (this.LinearFrameType == LinearFrameType.Horizontal)
            {
                this.MinimumSize = new Size(300, 125);
                this.Size = new Size(300, 125);
            }
            else
            {
                this.MinimumSize = new Size(125, 300);
                this.Size = new Size(125, 300);
            }
            Invalidate();
        }
        

        /// <summary>
        /// Repaints the control
        /// </summary>
        public void RepaintControl()
        {
            Refresh();
        }
        /// <summary>
        /// Returns the graphics path to draw rounded corners in the rectangle
        /// </summary>
        public static GraphicsPath GetRoundPath(Rectangle r, int depth)
        {
            GraphicsPath graphPath = new GraphicsPath();

            graphPath.AddArc(r.X, r.Y, depth, depth, 180, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y, depth, depth, 270, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y + r.Height - depth, depth, depth, 0, 90);
            graphPath.AddArc(r.X, r.Y + r.Height - depth, depth, depth, 90, 90);
            graphPath.AddLine(r.X, r.Y + r.Height - depth, r.X, r.Y + depth / 2);

            return graphPath;
        }
        /// <summary>
        /// Sets the Gauge controls visual styles
        /// </summary>
        private void SetVisualStyle(ThemeStyle style)
        {
            switch (style)
            {
                case ThemeStyle.Blue:
                    m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#ECF4FC");
                    m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#D8E4F2");
                    m_OuterFrameGradientStartColor = ColorTranslator.FromHtml("#CEDDEE");
                    m_OuterFrameGradientEndColor = ColorTranslator.FromHtml("#BDCAD9");
                    m_InnerFrameGradientStartColor = ColorTranslator.FromHtml("#849DBD");
                    m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#D8E4F2");
                    this.NeedleColor = this.ScaleLabelColor = this.GaugeBaseColor = this.MajorTickMarkColor = this.MinorTickMarkColor = ColorTranslator.FromHtml("#5F6F77");
                    break;
                case ThemeStyle.Silver:
                    m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#F5F5F5");
                    m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#E2E3E4");
                    m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                    m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                    m_InnerFrameGradientStartColor = ColorTranslator.FromHtml("#878787");
                    m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#F5F5F5");
                    this.NeedleColor = this.ScaleLabelColor = this.GaugeBaseColor = this.MajorTickMarkColor = this.MinorTickMarkColor = Color.Gray;
                    break;
                case ThemeStyle.Black:
                    m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#323031");
                    m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#232021");
                    m_OuterFrameGradientStartColor = ColorTranslator.FromHtml("#333132");
                    m_OuterFrameGradientEndColor = ColorTranslator.FromHtml("#262324");
                    m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#232021");
                    m_InnerFrameGradientStartColor = ColorTranslator.FromHtml("#070707");
                    this.NeedleColor = this.ScaleLabelColor = this.GaugeBaseColor = this.MajorTickMarkColor = this.MinorTickMarkColor = Color.White;
                    break;
                case ThemeStyle.Metro:
                    this.BackgroundGradientStartColor = Color.White;
                    this.BackgroundGradientEndColor = Color.White;
                    m_OuterFrameGradientStartColor = Color.FromArgb(17, 180, 205);
                    m_OuterFrameGradientEndColor = Color.FromArgb(17, 180, 205);
                    m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
                    m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
                    this.GaugeBaseColor = this.MajorTickMarkColor = this.MinorTickMarkColor = Color.Gray;
                    this.NeedleColor = Color.FromArgb(17, 180, 205);
                    this.ScaleLabelColor = Color.Gray;
                    break;
                default:
                    m_BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
                    m_BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
                    m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                    m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                    m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
                    m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
                    this.GaugeBaseColor = this.NeedleColor = this.MajorTickMarkColor = this.MinorTickMarkColor = Color.Gray;
                    break;
            }

        }
        /// <summary>
        /// Used to draw the Tick lines 
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="graphics"></param>
        private void DrawLines(Graphics g)
		{
            Pen majorTickPen = new Pen(MajorTickMarkColor, 2);
            Pen minorTickPen = new Pen(MinorTickMarkColor);
            Brush brush = new SolidBrush(ForeColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            majorTicksDistance = ((MaximumValue - MinimumValue) / m_MajorDifference);            
            m_majorTicksCount = ((int)(MaximumValue - MinimumValue)/ (m_MajorDifference))+1;
            double majortickValue = MinimumValue;
            float tickPosition = 25f;
            float temp1 = 0;
            float s = (MaximumValue - MinimumValue) % m_MajorDifference;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            float minortickValue = 0;
            float tickPositionminor = 0;
            GraphicsPath path = new GraphicsPath();
            int minorcount = _minorTicksCount;
            if (LinearFrameType == LinearFrameType.Vertical)
            {
                m_minorTicksPixels = ((this.Height - 50) / majorTicksDistance);
                int x = this.Width / 2;
                temp1 = 0;
                for (int L = 1; L <= m_majorTicksCount; L++)
                {
                    g.DrawLine(majorTickPen, x, this.Height - tickPosition, x - m_MajorTickMarkHeight, this.Height - tickPosition);
                    if (ShowScaleLabel)
                        g.DrawString(Math.Round(majortickValue, 2).ToString(),
                                 Font, brush, new PointF(x - m_MajorTickMarkHeight - 25, this.Height - tickPosition), sf);
                    if (L == m_majorTicksCount)
                        minorcount = (_minorTicksCount * (int)Math.Ceiling(s)) /MajorDifference ;
                    if (majortickValue < MaximumValue )
                    {
                        for (int S = 1; S <= minorcount; S++)
                        {
                            minortickValue = (m_minorTicksPixels / (_minorTicksCount + 1)) * S;
                            tickPositionminor = this.Height - (minortickValue + temp1 + 25);
                            g.DrawLine(minorTickPen, x, (float)tickPositionminor, x - MinorTickHeight, (float)tickPositionminor);
                        }
                        temp1 = m_minorTicksPixels * L;
                    }

                    majortickValue += m_MajorDifference;
                    tickPosition += m_minorTicksPixels;
                }
                g.FillRectangle(new SolidBrush(GaugeBaseColor), t_width, startpoint - 1, 5, (((this.majorTicksDistance)) * m_minorTicksPixels) + 2);
                g.FillRectangle(new SolidBrush(ValueIndicatorColor), t_width + 10, startpoint + (majorTicksDistance * m_minorTicksPixels) - (((Value / m_MajorDifference)) * m_minorTicksPixels), 5, (((Value / m_MajorDifference)) * m_minorTicksPixels) + 2);
                brush.Dispose();
                minorTickPen.Dispose();                
            }
            else
            {
                m_minorTicksPixels = ((this.Width - (startpoint * 2)) / majorTicksDistance);

                int y = t_height - MajorTicksHeight;
                
                temp1 = 0;
                int height=y + m_MajorTickMarkHeight;
                for (int L = 1; L <= m_majorTicksCount; L++)
                {
                    g.DrawLine(majorTickPen, tickPosition, y, tickPosition, height);
                    if (ShowScaleLabel)
                        g.DrawString(Math.Round(majortickValue, 2).ToString(), Font, brush, new PointF(tickPosition, y - 10), sf);
                    if (L == m_majorTicksCount)
                        minorcount = (_minorTicksCount * (int)Math.Ceiling(s)) / MajorDifference;
                    if (majortickValue < MaximumValue )
                    {
                        for (int S = 1; S <= minorcount; S++)
                        {
                                minortickValue = (m_minorTicksPixels / (_minorTicksCount + 1)) * S;
                                tickPositionminor = minortickValue + temp1 + startpoint;
                                g.DrawLine(minorTickPen, (float)tickPositionminor, height, (float)tickPositionminor, height - MinorTickHeight);
                        }
                        temp1 = m_minorTicksPixels * L;
                    }

                    majortickValue += m_MajorDifference;
                    tickPosition += m_minorTicksPixels;
                }
                g.FillRectangle(new SolidBrush(GaugeBaseColor), 24, t_height, (((this.majorTicksDistance)) * m_minorTicksPixels)+2, 5);
                g.FillRectangle(new SolidBrush(ValueIndicatorColor), 24 , t_height + 10, (((this.Value / MajorDifference)) * m_minorTicksPixels) + 2, 5);
                
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                
                brush.Dispose();
                minorTickPen.Dispose();      
            }
            
		}
        /// <summary>
        /// Used to draw the Bakcground Frame 
        /// </summary>       
        /// <param name="graphics"></param>
        private void DrawFrame(Graphics g)
        {
            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int tmpSoundCornerRadius = Math.Min(Math.Min(25, this.Width - 2), this.Height - 2);
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            Rectangle innerrect = new Rectangle(rect.X + 10, rect.Y + 10, this.Width - 2 * 10, this.Height - 2 * 10);
            GraphicsPath graphPath = GetRoundPath(rect, tmpSoundCornerRadius);
            GraphicsPath graphInnerPath = GetRoundPath(innerrect, tmpSoundCornerRadius);

            using (LinearGradientBrush brush1 = new LinearGradientBrush(rect,
                    m_OuterFrameGradientStartColor,
                   m_OuterFrameGradientEndColor,
                    LinearGradientMode.Vertical))
            {
                g.FillPath(brush1, graphPath);
                g.DrawPath(new Pen(m_InnerFrameGradientEndColor, 1), graphPath);

            }

            using (LinearGradientBrush brush2 = new LinearGradientBrush(innerrect,
                   m_BackgroundGradientStartColor,
                   m_BackgroundGradientEndColor,
                   LinearGradientMode.Vertical))
            {
                g.FillPath(brush2, graphInnerPath);
                g.DrawPath(new Pen(m_InnerFrameGradientStartColor, 1.6f), graphInnerPath);
            }
        }
        /// <summary>
        /// Used to draw the lines 
        /// </summary>
        private void DrawPointer(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            GraphicsPath path = new GraphicsPath();
            
            int a = (int)Math.Ceiling(((Value / (float)MajorDifference) * m_minorTicksPixels));
            int y = (t_height + m_MajorTickMarkHeight) - MajorTicksHeight;
            Point pt1, pt2, pt3;
            if (LinearFrameType == LinearFrameType.Vertical)
            {
                a = 10 + (int)Math.Ceiling((majorTicksDistance * m_minorTicksPixels)) - a;
                
                if (PointerPlacement == Placement.Near)
                {
                    pt1 = new Point(t_width- 15, 10 + a);
                    pt2 = new Point(t_width - 15, 20 + a);
                    pt3 = new Point(t_width , 15 + a);
                }
                else if (PointerPlacement == Placement.Center)
                {
                    pt1 = new Point(t_width + 20, 10 + a);
                    pt2 = new Point(t_width + 20, 20 + a);
                    pt3 = new Point(t_width+5 , 15 + a);
                }
                else
                {
                    pt1 = new Point(t_width + 28, 10 + a);
                    pt2 = new Point(t_width + 28, 20 + a);
                    pt3 = new Point(t_width + 13, 15 + a);
                }
                
            }
            else
            {
                 a = a + 10;
                 
                 if (PointerPlacement == Placement.Near)
                 {
                     pt1 = new Point(10 + a, y - 15);
                     pt2 = new Point(20 + a, y - 15);
                     pt3 = new Point(15 + a, y  );
                 }
                 else if (PointerPlacement == Placement.Center)
                 {
                     pt1 = new Point(10 + a, y + 20);
                     pt2 = new Point(20 + a, y + 20);
                     pt3 = new Point(15 + a, y + 5);
                 }
                 else
                 {
                     pt1 = new Point(10 + a, y + 27);
                     pt2 = new Point(20 + a, y + 27);
                     pt3 = new Point(15 + a, y +12);
                 }
                 
            }
            path.AddPolygon(new Point[] { pt1, pt2, pt3 });
            g.FillPath((new SolidBrush(NeedleColor)), path);
        }

        #endregion 

        #region Ranges
        /// <summary>
        /// Used to draw the range lines
        /// </summary>
        private void DrawRanges(Graphics gr)
        {

            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            foreach (LinearRange ptrRange in _GaugeRanges)
            {
                int rvalve = (int)Math.Ceiling(MaximumValue - ptrRange.EndValue) / MajorDifference;
                if (ptrRange.EndValue > ptrRange.StartValue)
                {
                    if (LinearFrameType == LinearFrameType.Horizontal)
                    {
                        if (ptrRange.StartValue == 0)
                            gr.FillRectangle(new SolidBrush(ptrRange.Color), startpoint + ptrRange.StartValue, t_height, (((ptrRange.EndValue / MajorDifference)) * m_minorTicksPixels), 5);
                        else
                            gr.FillRectangle(new SolidBrush(ptrRange.Color), 25 + (((ptrRange.StartValue / MajorDifference)) * m_minorTicksPixels), t_height, ((((ptrRange.EndValue - ptrRange.StartValue) / MajorDifference)) * m_minorTicksPixels), 5);
                    }
                    else
                    {
                        float rangeheight = (ptrRange.EndValue / m_MajorDifference) * m_minorTicksPixels;
                        if (ptrRange.StartValue == 0)
                            gr.FillRectangle(new SolidBrush(ptrRange.Color), t_width, startpoint + (majorTicksDistance * m_minorTicksPixels) - rangeheight, 5, rangeheight);
                        else
                            gr.FillRectangle(new SolidBrush(ptrRange.Color), t_width, startpoint + (majorTicksDistance * m_minorTicksPixels) - rangeheight, 5, (((ptrRange.EndValue - ptrRange.StartValue) / MajorDifference) * m_minorTicksPixels));
                    }
                }
            }
        }
        #endregion       

        #region Overrides
        /// <summary>
        /// Overrides base.OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
		{			
			e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
           
            m_Width = this.Width;
            m_Height = this.Height;

            t_width = m_Width / 2;
            t_height = m_Height / 2;
           

            DrawFrame(e.Graphics);
            DrawLines(e.Graphics);
            DrawRanges(e.Graphics);
            if(ShowNeedle)
                DrawPointer(e.Graphics);
			base.OnPaint(e);
        }
        #endregion

        #region Ranges
        /// <summary>
        /// Returns the GaugeRanges of the control.
        /// </summary>
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Returns the GaugeRanges of the control.")]
        public LinearRangeCollection Ranges
        {
            get 
            {
                return _GaugeRanges; 
            }
        }
        private LinearRangeCollection _GaugeRanges;
        #endregion
        /// <summary>
        /// Event argument for <see cref="ValueInRangeChanged"/> event.
        /// </summary>
        public class LinearThresholdValueChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Affected GaugeRange
            /// </summary>
            public LinearRange Range { get; private set; }
            /// <summary>
            /// Gauge Value
            /// </summary>
            public Single Value { get; private set; }
            /// <summary>
            /// True if value is within current range.
            /// </summary>
            public bool InRange { get; private set; }
            public LinearThresholdValueChangedEventArgs(LinearRange range, Single value, bool inRange)
            {
                this.Range = range;
                this.Value = value;
                this.InRange = inRange;
            }
            public LinearThresholdValueChangedEventArgs(Single value)
            {
                this.Value = value;
            }
        }
        #region EventHandler

        [Description("This event is raised when gauge value changed.")]
        public event EventHandler ValueChanged;
        private void OnValueChanged()
        {
            EventHandler e = ValueChanged;
            if (e != null) e(this, null);
        }

        [Description("This event is raised if the value is entering or leaving defined range.")]
        public event EventHandler<LinearThresholdValueChangedEventArgs> ThresholdValueChanged;
        private void OnThresholdValueChanged(LinearRange range, Single value)
        {
            EventHandler<LinearThresholdValueChangedEventArgs> e = ThresholdValueChanged;
            if (e != null) e(this, new LinearThresholdValueChangedEventArgs(range, value, range.InRange));
        }

        #endregion

        #region DataBinding
        #region Context Changed
        protected override void OnBindingContextChanged(EventArgs e)
        {
            this.EnsureDataBinding();
            base.OnBindingContextChanged(e);
        }
        #endregion

        #region EnsureDataBinding
        /// <summary>
        /// Tries to get a new CurrencyManager for new DataBinding
        /// </summary>
        private void EnsureDataBinding()
        {
            if (this.DataSource == null ||
                base.BindingContext == null)
                return;

            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)base.BindingContext[this.DataSource, this.DataMember];
            }
            catch (System.ArgumentException)
            {
                // If no CurrencyManager was found
                return;
            }
            if (this.dataManager != cm)
            {
                // Unwire the old CurrencyManager
                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged -= listChangedHandler;
                    this.dataManager.PositionChanged -= positionChangedHandler;
                }
                this.dataManager = cm;
                // Wire the new CurrencyManager
                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged += listChangedHandler;
                    this.dataManager.PositionChanged += positionChangedHandler;
                }

                // Update metadata and data
                CalculateColumns();
                UpdateAllData();
            }
        }
        #endregion

        #region Item(s) changed from DataSource
        /// <summary>
        /// Datasources get updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset ||
                e.ListChangedType == ListChangedType.ItemMoved)
            {
                // Update all data
                UpdateAllData();
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                // Add new Item
                AddItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                // Change Item
                UpdateItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                // Delete Item
                DeleteItem(e.NewIndex);
            }
            else
            {
                // Update metadata and all data
                CalculateColumns();
                UpdateAllData();
            }
            PopulateGauge();
        }
        #endregion

        #region Item Methods
        /// <summary>
        /// Updates all Items.
        /// </summary>
        private void UpdateAllData()
        {
            list.Items.Clear();
            for (int i = 0; i < DataManager.Count; i++)
            {
                AddItem(i);
            }
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void AddItem(int index)
        {
            ListViewItem item = GetListViewItem(index);
            this.list.Items.Insert(index, item);
        }

        /// <summary>
        /// Updates the data of the item with the DataSource.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void UpdateItem(int index)
        {
            if (index >= 0 &&
                index < list.Items.Count)
            {
                ListViewItem item = GetListViewItem(index);
                list.Items[index] = item;
            }
        }

        /// <summary>
        /// Returns a <see cref="ListViewItem"/> which contains the row-data at given index.
        /// </summary>
        /// <param name="index">The index of the row.</param>
        /// <returns>A item which contains the data.</returns>
        private ListViewItem GetListViewItem(int index)
        {
            object row = DataManager.List[index];
            PropertyDescriptorCollection propColl = DataManager.GetItemProperties();
            ArrayList items = new ArrayList();
            PropertyDescriptor prop = null;
            // Fill value for each column
            foreach (ColumnHeader column in list.Columns)
            {
                prop = propColl.Find(column.Text, false);
                if (prop != null)
                {
                    items.Add(prop.GetValue(row).ToString());
                }
            }
            return new ListViewItem((string[])items.ToArray(typeof(string)));
        }
        private string CollectListViewItem(int index)
        {

            object row = DataManager.List[index];
            PropertyDescriptorCollection propColl = DataManager.GetItemProperties();

            // Fill value for each column
            foreach (ColumnHeader column in list.Columns)
            {
                PropertyDescriptor prop = null;
                prop = propColl.Find(column.Text, false);
                if (prop != null)
                {
                    return prop.GetValue(row).ToString();
                }
                else
                    return "error";
            }
            return "";
        }
        /// <summary>
        /// Delete the item at the given index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void DeleteItem(int index)
        {
            if (index >= 0 &&
                index < list.Items.Count)
                list.Items.RemoveAt(index);
        }

        /// <summary>
        /// Calculates the Colums of the <see cref="BoundListView"/>.
        /// </summary>
        private void CalculateColumns()
        {
            list.Columns.Clear();

            if (dataManager == null)
                return;
            ColumnHeader column;
            foreach (PropertyDescriptor prop in DataManager.GetItemProperties())
            {
                column = new ColumnHeader();
                column.Text = prop.Name;
                list.Columns.Add(column);
            }
        }


        #endregion

        #region Data Properties
        #region DataSource
        /// <summary>
        /// Gets or sets the data source that you want to display the data.
        /// </summary>
        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Category("Data")]
        [Description("Specifies the data source for the control.")]
        [DefaultValue(null)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (this.dataSource != value)
                {
                    this.dataSource = value;
                    EnsureDataBinding();
                }
            }
        }

        #endregion

        #region DataMember
        /// <summary>
        /// Specifies a secondary list of Datasource, to display it
        /// </summary>
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design",
             "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Specifies a secondary list of Datasource, to display it")]
        [DefaultValue(null)]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }
            set
            {
                if (this.dataMember != value)
                {
                    this.dataMember = value;
                    EnsureDataBinding();
                }
            }
        }
        #endregion

        #region CurrencyManager
        /// <summary>
        /// Gets the CurrencyManager of the bound list.
        /// </summary>
        protected CurrencyManager DataManager
        {
            get
            {
                return this.dataManager;
            }
        }
        #endregion

        #endregion

        #region DisplayMember and GaugePopulation

        private BindingMemberInfo displayMember;
        [
        Category("Data"),
       Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design",
           "System.Drawing.Design.UITypeEditor, System.Drawing"),
       DefaultValue(@""),
       TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
       Description(@"Indicates the property to display for the items in this control.")
       ]
        public string DisplayMember
        {
            get
            {
                return this.displayMember.BindingMember;
            }
            set
            {
                BindingMemberInfo displayMember0;

                displayMember0 = this.displayMember;
                try
                {
                    this.SetDataConnection(this.dataSource, new BindingMemberInfo(value), false);
                }
                catch (Exception)
                {
                    this.displayMember = displayMember0;
                }
            }
        }
        /// <summary>
        /// Sets the data connection
        /// </summary>
        /// <param name="newDataSource"></param>
        /// <param name="newDisplayMember"></param>
        /// <param name="force"></param>
        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force)
        {
            CurrencyManager dataManager;

            bool isNewDataSource = !(this.dataSource == newDataSource);
            bool isNewDisplayMember = !this.displayMember.Equals(newDisplayMember);

            if (force || isNewDataSource || isNewDisplayMember)
            {

                if (this.dataSource as IComponent != null)
                    ((IComponent)this.dataSource).Disposed -= new EventHandler(this.DataSourceDisposed);
                this.dataSource = newDataSource;
                this.displayMember = newDisplayMember;

                dataManager = null;
                if (newDataSource != null && this.BindingContext != null && newDataSource != Convert.DBNull)
                    dataManager = (CurrencyManager)this.BindingContext[newDataSource, newDisplayMember.BindingPath];
            }
        }
        /// <summary>
        /// Disposes the data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourceDisposed(object sender, EventArgs e)
        {
            this.SetDataConnection(null, new BindingMemberInfo(string.Empty), true);
        }

        private int displayRecordIndex = 0;
        /// <summary>
        /// Gets or Sets the display record index
        /// </summary>
        [
        Category("Data"),
        Description("Gets or Sets the display record index.")
       ]
        public int DisplayRecordIndex
        {
            get
            {
                return displayRecordIndex;
            }
            set
            {
                displayRecordIndex = value;
                PopulateGauge();
            }
        }

        private void PopulateGauge()
        {
            int index = 0;

            foreach (ColumnHeader c in this.list.Columns)
            {
                if (c.Text == this.displayMember.BindingField)
                {
                    index = list.Columns.IndexOf(c);
                    break;
                }
            }

            if (DataManager != null && DisplayRecordIndex < DataManager.List.Count)
            {
                object row = DataManager.List[DisplayRecordIndex];
                PropertyDescriptorCollection propColl = DataManager.GetItemProperties();
                ColumnHeader column = list.Columns[index];
                PropertyDescriptor prop = propColl.Find(column.Text, false);
                if (prop != null)
                {
                    float val = 0f;
                    if (float.TryParse(prop.GetValue(row).ToString(), out val))
                    {
                        this.Value = val;
                    }
                    else
                    {
                        val = 0;
                        this.Value = val;
                    }
                }
            }
        }

        #endregion

        #endregion
    }

    #region[ Gauge Range ]

    /// <summary>
    /// Event argument for <see cref="ValueInRangeChanged"/> event.
    /// </summary>
    public class LinearThresholdValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Affected GaugeRange
        /// </summary>
        public LinearRange Range { get; private set; }
        /// <summary>
        /// Gauge Value
        /// </summary>
        public Single Value { get; private set; }
        /// <summary>
        /// True if value is within current range.
        /// </summary>
        public bool InRange { get; private set; }
        public LinearThresholdValueChangedEventArgs(LinearRange range, Single value, bool inRange)
        {
            this.Range = range;
            this.Value = value;
            this.InRange = inRange;
        }
        public LinearThresholdValueChangedEventArgs(Single value)
        {
            this.Value = value;
        }
    }

    #endregion
    #region SmartTag
    /// <summary>
    /// Desginer class for LinearGauge
    /// </summary>
    public class LinearGaugeDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public LinearGaugeDesigner()
            : base()
        {
        }


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new LinearGaugeActionList(this.Component));
                }
                return this.actionLists;
            }
        }

#endif
        /// <summary>
        /// Overridden Initialize method.
        /// </summary>
        /// <param name="component">Componnent object</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
    }


    /// <summary>
    /// Designer action list of LinearGauge
    /// </summary>
    public class LinearGaugeActionList : SyncActionListBase<LinearGauge>
    {
        /// <summary>
        /// Initializes a new instance of the LinearGauge class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public LinearGaugeActionList(IComponent component)
            : base(component)
        {
        }


        /// <summary>
        /// Gets/Sets the value for Maximum value
        /// </summary>
        public Single MaximumValue
        {
            get
            {
                Single maximumValue = 10;
                if (this.Control != null)
                {
                    LinearGauge control = this.Control as LinearGauge;
                    maximumValue = control.MaximumValue;
                }
                return maximumValue;
            }
            set
            {
                SetValue("MaximumValue", value);
            }
        }

        /// <summary>
        /// Gets/Sets the value for Maximum value
        /// </summary>
        public Single MinimumValue
        {
            get
            {
                Single minimumValue = 0;
                if (this.Control != null)
                {
                    LinearGauge control = this.Control as LinearGauge;
                    minimumValue = control.MinimumValue;
                }
                return minimumValue;
            }
            set
            {
                SetValue("MinimumValue", value);
            }
        }
        /// <summary>
        /// Gets/Sets the value for Frame style
        /// </summary>
        public LinearFrameType LinearFrameType
        {
            get
            {
                LinearFrameType frameType = LinearFrameType.Horizontal;
                if (this.Control != null)
                {
                    LinearGauge control = this.Control as LinearGauge;
                    frameType = control.LinearFrameType;
                }
                return frameType;
            }

            set
            {
                SetValue("LinearFrameType", value);
            }
        }
        
        /// <summary>
        /// Gets or Sets a value to gauge Major line Difference
        /// </summary>
        public int MajorDifference
        {
            get
            {
                int majorDifference = 20;
                if (this.Control != null)
                {
                    LinearGauge control = this.Control as LinearGauge;
                    majorDifference = control.MajorDifference;
                }
                return majorDifference;
            }
            set
            {
                SetValue("MajorDifference", value);
            }
        }

        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            // Customization Category
            this.AddDesignerActionHeaderItem("Customization");
            this.AddDesignerActionPropertyItem("MaximumValue", "MaximumValue", "Customization", "Gets/Sets the values for Maximum value.");
            this.AddDesignerActionPropertyItem("MinimumValue", "MinimumValue", "Customization", "Gets/Sets the values for Minimum value.");
            this.AddDesignerActionPropertyItem("MajorDifference", "MajorDifference", "Customization", "Gets/Sets the value for Major Difference.");
            this.AddDesignerActionPropertyItem("LinearFrameType", "LinearFrameType", "Customization", " Gets/Sets the value for Frame Type.");
        }
    }
    #endregion
}
