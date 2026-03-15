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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using PointF = System.Drawing.PointF;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// The ChartArea is the actual rendering area of the plot.
	/// It provides a canvas on which the chart is rendered.
	/// </summary>
	public class ChartArea : IDisposable, IChartArea
	{
		#region Constants
		private const float c_maxScale = 1.5f;
		private const float c_minScale = 0.5f;
		private const float c_max2DRotate = 90f;
		private const float c_max3DRotate = 360f;
		internal const double CircularChartOffset = ChartMath.HlfPI;
		#endregion

		#region Members
        private static PointF oldcurpos;
        private bool m_cursorRedraw = false;
        private ChartStyleInfo m_seriesStyle=new ChartStyleInfo();
        private Point m_cursorLocation;
		private bool m_boundsByAxis = true;
		private readonly IChartAreaHost m_chart;
		private ChartAxisCollection m_axes = null;
		private SizeF m_axisSpacing = new SizeF(2, 2);
		private ChartAxesInfoBar m_chartAxesLabelInfoBar = null;

		private ChartThickness m_axesThickness = new ChartThickness(0);
        private Color m_defaultColor = Color.Red;
        private Rectangle bounds;      
		private bool m_needRedraw = true;
		private bool m_hidePartialLabels = false;
		private ChartAxis xAxis = null;
		private ChartAxis yAxis = null;
		private bool m_requireAxes = true;
        private bool m_changeAppearance=true;
		private bool m_requireInvertedAxes = false;
		private ChartSetMode m_adjustPlotAreaMargins = ChartSetMode.AutoSet;
		private bool m_series3D = false;
		private bool m_leaveTree = false;
		private bool m_autoScale = false;
		private float m_depth = 50;
		private float m_rotation = 30;
		private float m_tilt = 30;
		private float m_turn = 0f;
		private ChartMargins areaMargins = new ChartMargins(10, 10, 10, 10);
		private ChartMargins m_chartPlotAreaMargins = new ChartMargins(10, 10, 10, 10);
		private string m_chartAreaTooltip = String.Empty;
		private ChartCustomPointCollection m_customPoints;
		private BorderStyle m_borderStyle = BorderStyle.None;
		private BrushInfo m_backInterior = null;
		private BrushInfo m_gridInterior = new BrushInfo(Color.White);
		private Image m_backImage = null;
        private Image m_interiorBackImage = null;
		private bool m_realSeries3D = false;
		private float m_scale3DCoeficient = 1f;

		private Vector3D m_rotateCenter = Vector3D.Empty;
		private Matrix3D m_viewMatrix3D = Matrix3D.Identity;
		private Transform3D m_transform3D = new Transform3D();

		private ChartAreaCursorCollection m_interactiveCursors = new ChartAreaCursorCollection();
		private Graphics3DState m_3dSettings = new Graphics3DState();
		private Color m_borderColor = SystemColors.ControlText;
		private int m_borderWidth = 1;
		private BspNode rootNode = null;
		private bool m_divideArea = false;
        private bool m_multiplePies = false;
        private bool reDrawAxes = false;
		private double m_fullStackMax = 100;
		private ChartAxesLayoutMode m_xAxesLayoutMode = ChartAxesLayoutMode.Stacking;
		private ChartAxesLayoutMode m_yAxesLayoutMode = ChartAxesLayoutMode.Stacking;

		private SizeF m_minSize = new SizeF(200, 100);

		private Rectangle m_bounds = Rectangle.Empty;
		private Rectangle m_clientRectangle = Rectangle.Empty;
		private RectangleF m_renderbounds = RectangleF.Empty;
		private Rectangle m_globalRenderbounds = Rectangle.Empty;
		private readonly ChartWatermark m_watermark;

		private ChartSeriesParameters m_seriesParameters = null;
		private bool m_isIndexed = false;
        private bool m_isAllowGap = true;
		private ChartAxisLayoutCollection m_xLayouts = null;
		private ChartAxisLayoutCollection m_yLayouts = null;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether this instance is indexed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is indexed; otherwise, <c>false</c>.
		/// </value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool IsIndexed
		{
			get
            {
                return m_isIndexed; 
            }

			set
			{
				if (m_isIndexed != value)
				{
					m_isIndexed = value;

					if (value)
					{
						m_chart.Series.RaiseResetEvent();
					}

					foreach (ChartSeries series in m_chart.Series)
					{
						series.UpdateRenderer(ChartUpdateFlags.Indexed);
					}

					if (!value)
					{
						m_chart.Series.RaiseResetEvent();
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is indexed with gap or not when empty points are used.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsAllowGap
        {
            get
            {
                return m_isAllowGap; 
            }

            set
            {
                if (m_isAllowGap != value)
                {
                    m_isAllowGap = value;                                      
                    m_chart.Series.RaiseResetEvent();
                   
                    foreach (ChartSeries series in m_chart.Series)
                    {
                        series.UpdateRenderer(ChartUpdateFlags.Indexed);
                    }                  
                }
            }
        }

        /// <summary>
        /// Gets or sets whether  the cursor need to redraw or not.
        /// </summary>
        public bool CursorReDraw
        {
            get
            {
              return  m_cursorRedraw;

            }
            set
            {
                m_cursorRedraw = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor location of chart.
        /// </summary>
        
        public Point CursorLocation
        {
            get
            {
                return m_cursorLocation;
            }
            set
            {
                m_cursorLocation = value;
            }
        }
		/// <summary>
		/// Owner of this chart area.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IChartAreaHost Chart
		{
			get { return m_chart; }
		}

		#region Border
		/// <summary>
		/// Gets or sets the border width of the ChartArea. Default is 1.
		/// </summary>
		[DefaultValue(1), Description("Specifies the border width of the ChartArea.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public int BorderWidth
		{
			get 
            {
                return m_borderWidth; 
            }

			set
			{
				if (m_borderWidth != value)
				{
					m_borderWidth = value;
					Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets the bordercolor of the ChartArea. Default is SystemColors.ControlText.
		/// </summary>
		[DefaultValue(typeof(Color), "ControlText"), ChartTemplate(ChartTemplateSet.Simple),
		 Description("Specifies the bordercolor of the ChartArea.")]
		public Color BorderColor
		{
			get
            {
                return m_borderColor; 
            }

			set
			{
				if (m_borderColor != value)
				{
					m_borderColor = value;
					Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets the style of the border that is to be rendered around the ChartArea. Default is None.
		/// </summary>
		[DefaultValue(BorderStyle.None), ChartTemplate(ChartTemplateSet.Simple),
		 Description("Specifies the style of the border that is to be rendered around the ChartArea.")]
		public BorderStyle BorderStyle
		{
			get
			{
				return m_borderStyle;
			}
			set
			{
				if (m_borderStyle != value)
				{
					m_borderStyle = value;
					this.Redraw(true);
				}
			}
		}
		#endregion

		#region Layout
		/// <summary>
		/// Gets or sets the width of the rectangular area that is to be occupied by this ChartArea.    
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Width
		{
			get
			{
				return m_clientRectangle.Width;
			}
			set
			{
				if (m_bounds.Width != value)
				{
					m_bounds.Width = value;
					this.CalculateSizes(m_bounds);
				}
			}
		}
		/// <summary>
		/// Gets or sets the height of the rectangular area that is to be occupied by this ChartArea.    
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Height
		{
			get
			{
				return m_bounds.Height;
			}
			set
			{
				if (m_bounds.Height != value)
				{
					m_bounds.Height = value;
					this.CalculateSizes(m_bounds);
				}
			}
		}
		/// <summary>
		/// Returns the y coordinate of the top edge of the rectangular area that is to be occupied by this ChartArea.     
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Top
		{
			get
			{
				return m_bounds.Top;
			}
		}
		/// <summary>
		/// Returns the x coordinate of the right edge of the rectangular area that is to be occupied by the ChartArea.   
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Right
		{
			get
			{
				return m_bounds.Right;
			}
		}
		/// <summary>
		/// Returns the x coordinate of the left edge of the rectangular area that is to be occupied by the ChartArea.   
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Left
		{
			get
			{
				return m_bounds.Left;
			}
		}
		/// <summary>
		/// Returns the y coordinate of the bottom edge of the rectangular area that is to be occupied by this ChartArea.   
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public int Bottom
		{
			get
			{
				return m_bounds.Bottom;
			}
		}
		/// <summary>
		/// Returns the bounds occupied by this ChartArea.    
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]        
		public Rectangle Bounds
		{
			get
			{
				return m_bounds;
			}
			set
			{
				if (m_bounds != value)
				{
					this.CalculateSizes(m_bounds);
				}
			}
		}
		/// <summary>
		/// Returns the Rectangle in client co-ordinates that is occupied by this ChartArea.    
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Rectangle ClientRectangle
		{
			get
			{
				return m_clientRectangle;
			}
			set
			{
				if (m_clientRectangle != value)
				{
					m_clientRectangle = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the size of the rectangular area that is to be occupied by the ChartArea.    
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public Size Size
		{
			get
			{
				return m_clientRectangle.Size;
			}
			set
			{
				if (m_bounds.Size != value)
				{
					m_bounds.Size = value;
					this.CalculateSizes(m_bounds);
				}
			}
		}
		/// <summary>
		///     Gets or sets the location of the rectangular area that is to be occupied by this ChartArea.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point Location
		{
			get
			{
				return m_clientRectangle.Location;
			}
			set
			{
				if (m_bounds.Location != value)
				{
					m_bounds.Location = value;
					this.CalculateSizes(m_bounds);
				}
			}
		}
		/// <summary>
		/// Returns the radius of the Radar chart occupied by this ChartArea.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Radius
		{
			get
			{
				return GetRadarRadius(xAxis.Font.Height);
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether area should be divided for each simple chart (Pie, Funnel...).
		/// </summary>
		/// <value><c>true</c> if area should be divided; otherwise, <c>false</c>.</value>
		[DefaultValue(false), Description("Indicates whether area should be divided for each simple chart (Pie, Funnel...)")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public bool DivideArea
		{
			get
			{
				return m_divideArea;
			}
			set
			{
				if (m_divideArea != value)
				{
					m_divideArea = value;
					this.Redraw(true);
				}
			}
		}

        /// <summary>
        /// If set to true, multiple pie chart series will be rendered in the same chart area.
        /// </summary>
        /// <value></value>
        [DefaultValue(false), Description("If set to true, multiple pie chart series will be rendered in the same chart area.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool MultiplePies
        {
            get
            {
                return m_multiplePies;
            }
            set
            {
                if (m_multiplePies != value)
                {
                    m_multiplePies = value;
                    this.Redraw(true);
                }
            }
        }
        /// <summary>
        /// If set to true, Chart axes labels will be rendered each time chart updates.
        /// </summary>
        /// <value></value>
        [DefaultValue(false), Description("If set to true, Chart axes labels will be rendered each time chart updates")]
        public bool ReDrawAxes
        {
            get
            {
                return reDrawAxes;
            }
            set
            {
                if (reDrawAxes != value)
                {
                    reDrawAxes = value;
                    this.Redraw(true);
                }
            }
        }
		/// <summary>
		/// Returns the center point of this ChartArea.     
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF Center
		{
			get
			{
				return ChartMath.GetCenter(this.RenderBounds);
			}
		}
		/// <summary>
		/// Returns the global rectangular bounds used for rendering.  
		/// </summary>
		/// <value>The render global bounds.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle RenderGlobalBounds
		{
			get
			{
				Rectangle rc = ClientRectangle;

				if (m_boundsByAxis && AxesType == ChartAreaAxesType.Rectangular)
				{
					float rLeft = float.MaxValue;
					float rTop = float.MaxValue;
					float rRight = float.MinValue;
					float rBottom = float.MinValue;

					foreach (ChartAxis axis in this.Axes)
					{
						if (axis.Orientation == ChartOrientation.Horizontal)
						{
							rLeft = Math.Min(axis.Rect.Left, rLeft);
							rRight = Math.Max(axis.Rect.Right, rRight);
						}
						else
						{ 
							rTop = Math.Min(axis.Rect.Top, rTop);
							rBottom = Math.Max(axis.Rect.Bottom, rBottom);
						}
					}

					rc = Rectangle.FromLTRB((int)rLeft, (int)(rTop - OffsetY), 
						(int)(rRight + OffsetX), (int)rBottom);
				}
				else
				{
					rc = GetGlobalBoundsByRect(rc);
				}

				return rc;
			}
		}
		/// <summary>
		/// Returns the actual rectangular bounds used for rendering.   
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle RenderBounds
		{
			get
			{
				Rectangle res = this.RenderGlobalBounds;

				if (this.DrawingMode == DrawingMode.Pseudo3D)
				{
					res = GetBoundsByRect(res);
				}

				return res;
			}
		}
		/// <summary>
		/// Returns the margins that will be deduced from the rectangular area that represents the ChartArea.
		/// Negative values are supported.
		/// </summary>
		[Description("Indicates the margins that will be deduced from the ChartArea`s representation rectangle.")
	     , Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.ContentBehavior)]
		public ChartMargins ChartAreaMargins
		{
			get
			{
				if (areaMargins == null)
				{
					areaMargins = new ChartMargins(10, 10, 10, 10);
					areaMargins.Changed += new EventHandler(OnChangingRedraw);
				}

				return areaMargins;
			}
			set
			{
				if (areaMargins != value)
				{
					areaMargins.Changed -= new EventHandler(OnChangingRedraw);
					areaMargins = value;
					areaMargins.Changed += new EventHandler(OnChangingRedraw);
					OnChangingRedraw(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 3D mode properties
		/// <summary>
		/// Returns the X axis offset value that is to be used when rendering in 3D mode.   
		/// </summary>
		[Browsable(false)]
		public float OffsetX
		{
			get
			{
				if (this.DrawingMode == DrawingMode.Pseudo3D && m_requireAxes)
				{
					return (float)(m_depth * Math.Sin(m_rotation * ChartMath.ToRadians));
				}
				else
				{
					return 0;
				}
			}
		}
		/// <summary>
		/// Returns the Y axis offset value that is to be used when rendering in 3D mode.   
		/// </summary>
		[Browsable(false)]
		public float OffsetY
		{
			get
			{
				if (this.DrawingMode == DrawingMode.Pseudo3D && m_requireAxes)
				{
					return (float)(m_depth * Math.Sin(m_tilt * ChartMath.ToRadians));
				}
				else
				{
					return 0;
				}
			}
		}
		/// <summary>
		/// Indicates whether the ChartArea is to be rendered in 3D. Default value is false.    
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Series3D
		{
			get
			{
				return m_series3D;
			}
			set
			{
				if (m_series3D != value)
				{
					m_series3D = value;
					m_leaveTree = m_leaveTree && m_realSeries3D && m_series3D;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Indicates whether the ChartArea is to be rendered in 3D. Default value is false.    
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RealSeries3D
		{
			get
			{
				return m_realSeries3D;
			}
			set
			{
				if (m_realSeries3D != value)
				{
					m_realSeries3D = value;

					if (!m_realSeries3D)
					{
						this.Rotation = this.Rotation;
						this.Tilt = this.Tilt;
						this.Turn = this.Turn;
					}

					m_leaveTree = m_leaveTree && m_realSeries3D && m_series3D;
					m_chart.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets the perception of depth that is to be used when the ChartArea is rendered in 3D. Default is 50.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Depth
		{
			get
			{
				return m_depth;
			}
			set
			{
				if (m_depth != value)
				{
					m_depth = (value == 0) ? 1F : value;

					if (m_series3D)
					{
						this.CalculateAxesSizes();
						this.Redraw(true);
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the rotational angle that is to be used when the ChartArea is rendered in 3D. Default is 30.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				value = this.RoundRotation(value);

				if (m_rotation != value)
				{
					m_rotation = value;

					if (m_series3D)
					{
						this.CalculateAxesSizes();
						this.Redraw(true);
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the tilt that is to be used when the ChartArea is rendered in 3D. Default is 30.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Tilt
		{
			get
			{
				return m_tilt;
			}
			set
			{
                if (this.Chart.Series[0].Type != ChartSeriesType.Pie || this.MultiplePies)
				value = this.RoundRotation(value);

				if (m_tilt != value)
				{
					m_tilt = value;

					if (m_series3D)
					{
						this.CalculateAxesSizes();
						this.Redraw(true);
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the turn that is to be used when the ChartArea is rendered in real 3D only. Default is 0.0f.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Turn
		{
			get
			{
				return m_turn;
			}
			set
			{
				value = this.RoundRotation(value);

				if (m_turn != value)
				{
					m_turn = value;

					if (m_series3D)
					{
						this.CalculateAxesSizes();
						this.Redraw(true);
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether area should scale automatically in 3D mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if area should scale automatically; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false), Description("Indicates whether area should scale automatically in 3D mode")]
		public bool AutoScale
		{
			get
			{
				return m_autoScale;
			}
			set
			{
				if (m_autoScale != value)
				{
					m_autoScale = value;
					Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets a scale value in 3D mode.
		/// </summary>
		/// <value></value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Scale3DCoeficient
		{
			get
			{
				return m_scale3DCoeficient;
			}
			set
			{
				value = ChartMath.MinMax(value, c_minScale, c_maxScale);

				if (m_scale3DCoeficient != value)
				{
					m_scale3DCoeficient = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets transformation for real 3d mode.
		/// </summary>
		/// <value></value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Transform3D Transform3D
		{
			get
			{
				return m_transform3D;
			}
		}
		/// <summary>
		/// Gets the real 3D mode settings.
		/// </summary>
		/// <value>The real 3D mode settings.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Graphics3DState Settings3D
		{
			get
			{
				return m_3dSettings;
			}
		}
		#endregion

		#region Background
		/// <summary>
		/// Gets or sets the background brush of the chart area.
		/// </summary>
		[ChartTemplate(ChartTemplateSet.Simple),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		 Description("Specifies the background brush of the chart area.")]
		public BrushInfo BackInterior
		{
			get
			{
				return m_backInterior != null ? m_backInterior : m_chart.BackInterior;
			}
			set
			{
				if (m_backInterior != value)
				{
					m_backInterior = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets the grid back interior.
		/// </summary>
		/// <value>The grid back interior.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public BrushInfo GridBackInterior
		{
			get
            {
                return m_gridInterior; 
            }

			set 
			{
				if (m_gridInterior != value)
				{
					m_gridInterior = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets the image that is to be used as the background for this ChartArea.    
		/// </summary>
		[DefaultValue(null), Description("Specifies the image that is to be used as the background for this ChartArea.")]
		public Image BackImage
		{
			get
			{
				return m_backImage;
			}
			set
			{
				if (m_backImage != value)
				{
					m_backImage = value;
					this.Redraw(true);
				}
			}
		}

        /// <summary>
        /// Gets or sets the image that is to be used as the background for this ChartArea Interior.    
        /// </summary>
        [DefaultValue(null)
     , Description("Specifies the image that is to be used as the background for this ChartArea interior.")]
        public Image InteriorBackImage
        {
            get
            {
                return m_interiorBackImage;
            }
            set
            {
                if (m_interiorBackImage != value)
                {
                    m_interiorBackImage = value;
                    this.Redraw(true);
                }
            }
        }
		#endregion

		#region Axes properties
		/// <summary>
		/// Specifies whether the ChartArea requires axes to be rendered (for the Chart types being rendered).
		/// </summary>
		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool RequireAxes
		{
			get { return m_requireAxes; }
			set { m_requireAxes = value; }
		}
        /// <summary>
        /// Specifies whether to change the appearance of chart.
        /// </summary>
        [DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool LegacyAppearance
        {
            get
            {
                return m_changeAppearance;
            }
            set
            {
                m_changeAppearance = value;
                this.DoAppearanceChange();
            }
        }
		/// <summary>
		/// Indicates whether Chart requires Inverted Axes
		/// </summary>
		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool RequireInvertedAxes
		{
			get { return m_requireInvertedAxes; }
			set { m_requireInvertedAxes = value; }
		}

		/// <summary>
		/// Collection of axes associated with this chart. You can add and remove axes from this collection.
		/// Primary X and Y axes may not be removed.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartAxisCollection Axes
		{
			get
			{
				if (m_axes == null)
				{
					m_axes = new ChartAxisCollection();
					m_axes.Changed += new ChartListChangeHandler(OnAxesChanged);

					xAxis = new ChartAxis(ChartOrientation.Horizontal);
					yAxis = new ChartAxis(ChartOrientation.Vertical);

					m_axes.AddRange(new ChartAxis[] { xAxis, yAxis });
				}

				return m_axes;
			}
		}

		/// <summary>
		/// Gets or sets the spacing between different axes on the same side of the ChartArea. This spacing is useful when you display multiple
		/// axes side by side.
		/// </summary>
		[Description("The spacing between different axes on the same side of the ChartArea."), Category("Axes")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public SizeF AxisSpacing
		{
			get
			{
				return m_axisSpacing;
			}
			set
			{
				if (m_axisSpacing != value)
				{
					m_axisSpacing = value;
					this.Redraw(true);
				}
			}
		}

		/// <summary>
		/// The primary X axis.
		/// </summary>
		[Description("The primary X axis."), Category("Axes")]
		public ChartAxis PrimaryXAxis
		{
			get
			{
				for (int i = 0; i < Axes.Count; i++)
				{
					if (m_axes[i].Orientation == ChartOrientation.Horizontal)
					{
						return xAxis = Axes[i];
					}
				}

				return null;
			}
		}
		/// <summary>
		/// The primary Y axis.
		/// </summary>
		[Description("The primary Y axis."), Category("Axes")]
		public ChartAxis PrimaryYAxis
		{
			get
			{
				for (int i = 0; i < Axes.Count; i++)
				{
					if (Axes[i].Orientation == ChartOrientation.Vertical)
					{
						return yAxis = Axes[i];
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Gets or sets the minimum size of this ChartArea.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SizeF MinSize
		{
			get
			{
				return m_minSize;
			}
			set
			{
				m_minSize = value;
			}
		}
		/// <summary>
		/// Returns the margins of ChartArea (excluding label width and height).
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.ContentBehavior)]
		public ChartMargins ChartPlotAreaMargins
		{
			get
			{
				if (m_chartPlotAreaMargins == null)
				{
					m_chartPlotAreaMargins = new ChartMargins();
					m_chartPlotAreaMargins.Changed += new EventHandler(OnChangingRedraw);
				}

				return m_chartPlotAreaMargins;
			}
			set
			{
				if (m_chartPlotAreaMargins != value)
				{
					if (m_chartPlotAreaMargins != null)
						m_chartPlotAreaMargins.Changed -= new EventHandler(OnChangingRedraw);

					m_chartPlotAreaMargins = value;

					if (m_chartPlotAreaMargins != null)
						m_chartPlotAreaMargins.Changed += new EventHandler(OnChangingRedraw);

					OnChangingRedraw(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the mode of drawing the edge labels. Default is AutoSet.
		/// </summary>
		[DefaultValue(ChartSetMode.AutoSet)]
		[Description("Gets or sets the mode of drawing the edge labels. Default is AutoSet.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
		public ChartSetMode AdjustPlotAreaMargins
		{
			get
			{
				return m_adjustPlotAreaMargins;
			}
			set
			{
				if (m_adjustPlotAreaMargins != value)
				{
					m_adjustPlotAreaMargins = value;
					Redraw(false);
				}
			}
		}

		/// <summary>
		/// Gets the information of axes bar representation.
		/// </summary>
		/// <value></value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ChartAxesInfoBar AxesInfoBar
		{
			get
			{
				return m_chartAxesLabelInfoBar;
			}
		}
		/// <summary>
		/// Gets or sets the maximal value of full stracking charts.
		/// </summary>
		/// <value>The maximal value of full stracking charts.</value>
		[DefaultValue(100d), Description("The maximal value of full stracking charts")]
		public double FullStackMax
		{
			get
			{
				return m_fullStackMax;
			}
			set
			{
				if (m_fullStackMax != value)
				{
					m_fullStackMax = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Specifies the way in which multiple X-axes will be rendered. Default is Stacking.
		/// </summary>
		[DefaultValue(ChartAxesLayoutMode.Stacking)]
		[Description("Specifies the way in which multiple X-axes will be rendered. Default is Stacking.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public ChartAxesLayoutMode XAxesLayoutMode
		{
			get
            {
                return m_xAxesLayoutMode; 
            }

			set
			{
				if (m_xAxesLayoutMode != value)
				{
					m_xAxesLayoutMode = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Specifies the way in which multiple Y-axes will be rendered. Default is Stacking.
		/// </summary>
		[DefaultValue(ChartAxesLayoutMode.Stacking)]
		[Description("Specifies the way in which multiple Y-axes will be rendered. Default is Stacking.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public ChartAxesLayoutMode YAxesLayoutMode
		{
			get 
            {
                return m_yAxesLayoutMode; 
            }

			set
			{
				if (m_yAxesLayoutMode != value)
				{
					m_yAxesLayoutMode = value;
					this.Redraw(true);
				}
			}
		}
		/// <summary>
		/// Gets the X axes layouts.
		/// </summary>
		/// <value>The X axes layouts.</value>
        [Description("Gets the X axes layouts")]
		public ChartAxisLayoutCollection XLayouts
		{
			get 
			{
				return m_xLayouts; 
			}
		}
		/// <summary>
		/// Gets the Y axes layouts.
		/// </summary>
		/// <value>The Y axes layouts.</value>
        [Description("Gets the Y axes layouts")]
		public ChartAxisLayoutCollection YLayouts
		{
			get { return m_yLayouts; }
		}
		#endregion

		#region Obsolete properties
		/// <summary>
		/// Gets or sets the current Redraw flag state. If true, the ChartArea representation is out of date and needs to be refreshed.    
		/// </summary>
		[Obsolete("This property isn't used anymore.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool NeedRedraw
		{
			get
            {
                return m_needRedraw; 
            }

			set
			{
				m_needRedraw = value;
			}
		}
		/// <summary>
		/// Indicates whether partially visible axis labels are hidden.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
	 , Obsolete("Use ChartAxis.HidePartialLabels instead."), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HidePartialLabels
		{
			get { return m_hidePartialLabels; }
			set
			{
				m_hidePartialLabels = value;

				foreach(ChartAxis axis in this.Axes)
				{
					axis.HidePartialLabels = value;
				}
			}
		}
		/// <summary>
		/// Obsolete.
		/// </summary>
		[Obsolete("This property isn't used anymore.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Vector3D RotateCenter
		{
			get
			{
				return m_rotateCenter;
			}
			set
			{
				if (m_rotateCenter != value)
				{
					m_rotateCenter = value;
					Redraw(true);
				}
			}
		}
		/// <summary>
		/// In a PieChart, if set to false, the legend will be displayed with one legend item for each slice in the Pie. Default is false.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
	, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Indicates whether area should be divided for each simple chart (Pie, Funnel...)")]
		[Obsolete("Use DivideArea property.")]
		public bool VisibleAllPies
		{
			get
			{
				return this.DivideArea;
			}
			set
			{
				this.DivideArea = value;
			}
		}
		/// <summary>
		/// List of chart regions
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use Chart.ChartRegions collection.")]
		public IList ChartRegions
		{
			get { return m_chart.ChartRegions; }
		}
		/// <summary>
		/// Old (obsolete) property. Use XAxesLayoutMode and YAxesLayoutMode instead. 
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)
		, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		, Obsolete("Use XAxesLayoutMode and YAxesLayoutMode instead")]
		public bool AxesSideBySide
		{
			get
			{
				return (m_xAxesLayoutMode == ChartAxesLayoutMode.SideBySide)
					|| (m_yAxesLayoutMode == ChartAxesLayoutMode.SideBySide);
			}
			set
			{
				if (value)
				{
					XAxesLayoutMode = ChartAxesLayoutMode.SideBySide;
					YAxesLayoutMode = ChartAxesLayoutMode.SideBySide;
				}
				else
				{
					XAxesLayoutMode = ChartAxesLayoutMode.Stacking;
					YAxesLayoutMode = ChartAxesLayoutMode.Stacking;
				}
			}
		}
		/// <summary>
		/// Gets or sets the quality of text rendering. Default is AntiAlias.
		/// </summary>
		[DefaultValue(TextRenderingHint.SystemDefault)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)
		, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets or sets the quality of text rendering. Default is AntiAlias.")]
		[Obsolete("This property isn't used anymore. Use Chart.TextRenderingHint property.")]
		public TextRenderingHint TextRenderingHint
		{
			get { return m_chart.TextRenderingHint; }
			set
			{
				m_chart.TextRenderingHint = value;
			}
		}
		/// <summary>
		/// Indicates if <see cref="RenderBounds"/> is calculated by including the label width and height of the axes. Default is true. 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public bool BoundsByAxes
		{
			get
			{
				return m_boundsByAxis;
			}
			set
			{
				m_boundsByAxis = value;
			}
		}
		#endregion

		#region Misc
		/// <summary>
		/// Gets the water mark information.
		/// </summary>
		/// <value>The water mark.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		, ChartTemplate(ChartTemplateSet.Content), Description( "Represents the watermark information." )]
		public ChartWatermark Watermark
		{
			get { return m_watermark; }
		}
		/// <summary>
		/// Gets or sets the ToolTip text associated with this ChartArea.
		/// </summary>
		[DefaultValue(""), Description("ToolTip text associated with this ChartArea")]        
		public string ChartAreaToolTip
		{
			get
			{
				return m_chartAreaTooltip;
			}
			set
			{
				if (m_chartAreaTooltip != value)
				{
					m_chartAreaTooltip = value;
				}
			}
		}
		#endregion

		/// <summary>
		/// Collection of interactive cursors that renders custom cursors on the chart area.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartAreaCursorCollection InteractiveCursors
		{
			get
			{
				return m_interactiveCursors;
			}
		}
		/// <summary>
		/// Collection of custom points that are to be rendered in this ChartArea. 
		/// Custom points can be added as markers at a specific location in the chart.
		/// </summary>
		/// <remarks>
		/// <seealso cref="ChartCustomPointCollection"/>
		/// </remarks>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartCustomPointCollection CustomPoints
		{
			get
			{
				if (m_customPoints == null)
				{
					m_customPoints = new ChartCustomPointCollection();
					m_customPoints.Changed += new ChartListChangeHandler(OnCustomPointsListChanged);
				}

				return m_customPoints;
			}
		}
		/// <summary>
		/// Gets the series rendering parameters.
		/// </summary>
		/// <value>The series parameters.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartSeriesParameters SeriesParameters
		{
			get
			{
				return m_seriesParameters;
			}
		}

		#region Internal properties
		/// <summary>
		/// Gets the drawing mode.
		/// </summary>
		/// <value>The drawing mode.</value>
		internal DrawingMode DrawingMode
		{
			get
			{
				if (!m_series3D) return DrawingMode.Simple2D;
				if (m_realSeries3D) return DrawingMode.Real3D;

				return DrawingMode.Pseudo3D;
			}
			set
			{
				switch (value)
				{
					case DrawingMode.Simple2D:
						m_series3D = false;
						m_realSeries3D = false;
						break;

					case DrawingMode.Pseudo3D:
						m_realSeries3D = false;
						m_series3D = true;
						break;

					case DrawingMode.Real3D:
						m_series3D = true;
						m_realSeries3D = true;
						break;
				}
			}
		}
		/// <summary>
		/// Gets or sets the type of the axes.
		/// </summary>
		/// <value>The type of the axes.</value>
		internal ChartAreaAxesType AxesType
		{
			get
			{
				if (m_requireAxes)
				{
					return ChartAreaAxesType.Rectangular;
				}
				else if(m_chart.Polar || m_chart.Radar)
				{
					return ChartAreaAxesType.Circular;
				}

				return ChartAreaAxesType.None;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Matrix BackMatrix
		{
			get
			{
				return new Matrix(RenderBounds, new PointF[]
                    {
                      new PointF(RenderBounds.Left + OffsetX, RenderBounds.Top - OffsetY),
                      new PointF(RenderBounds.Right + OffsetX, RenderBounds.Top - OffsetY),
                      new PointF(RenderBounds.Left + OffsetX, RenderBounds.Bottom - OffsetY)
                    });
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Matrix BottomMatrix
		{
			get
			{
                RectangleF rect = new RectangleF();
                if (!double.IsNaN(this.PrimaryXAxis.Crossing))
                {
                    rect = new RectangleF(RenderBounds.Left, bounds.Y -OffsetY, RenderBounds.Width, OffsetY);
                }
                else
                {
                    rect = new RectangleF(RenderBounds.Left, RenderBounds.Bottom - OffsetY, RenderBounds.Width, OffsetY);
                }
				return new Matrix(rect, new PointF[]{
                                               new PointF(rect.Left + OffsetX, rect.Top),
                                               new PointF(rect.Right + OffsetX, rect.Top),
                                               new PointF(rect.Left, rect.Bottom)
                                             });
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Matrix LeftMatrix
		{
			get
			{
				 RectangleF rect=new RectangleF();
                 if (!double.IsNaN(this.PrimaryYAxis.Crossing))
                 {
                     rect = new RectangleF(bounds.Left, RenderBounds.Top, OffsetX, RenderBounds.Height);
                 }
                 else
                 {
                     rect = new RectangleF(RenderBounds.Left, RenderBounds.Top, OffsetX, RenderBounds.Height);
                 }
				return new Matrix(rect, new PointF[]{
                                               new PointF(rect.Left, rect.Top),
                                               new PointF(rect.Right, rect.Top - OffsetY),
                                               new PointF(rect.Left, rect.Bottom)
                                             });
			}
		}
		#endregion

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor. ChartArea requires a host which implements <see cref="IChartAreaHost"/>. Currently this is implemented
		/// only by the chart. However, it is possible that other controls that wish to aggregate the chart will implement this
		/// interface.    
		/// </summary>
		/// <param name="chart" type="Syncfusion.Windows.Forms.Chart.IChartAreaHost">
		///     <para>
		///     Host interface.   
		///     </para>
		/// </param>
		public ChartArea(IChartAreaHost chart)
		{
			if (chart == null)
				throw new ArgumentNullException("chart");

			m_chart = chart;

			m_3dSettings.Changed += new EventHandler(OnChangingRedraw);

			m_chartAxesLabelInfoBar = new ChartAxesInfoBar();
			m_chartAxesLabelInfoBar.Changed += new EventHandler(OnChangingRedraw);

			m_interactiveCursors.Changed += new ChartListChangeHandler(OnInteractiveCursorsChanged);

			m_watermark = new ChartWatermark(this);
			m_seriesParameters = new ChartSeriesParameters(this);

			m_xLayouts = new ChartAxisLayoutCollection(this);
			m_yLayouts = new ChartAxisLayoutCollection(this);
		}
		#endregion

		#region Public methods

		#region Drawing methods
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void Draw(PaintEventArgs e)
		{
			this.Draw(e, ChartPaintFlags.All);
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void Draw(PaintEventArgs e, ChartPaintFlags flags)
		{
			if ((ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
				&& (RenderBounds.Width > 0 && RenderBounds.Height > 0))
			{
				if (this.RequireAxes)
				{
					m_xLayouts.Validate(ChartOrientation.Horizontal);
					m_yLayouts.Validate(ChartOrientation.Vertical);
				}

				m_needRedraw = false;
				Graphics g = e.Graphics;

				if (m_chart.NeedRegionUpdate)
				{
					m_chart.ChartRegions.Add(new ChartRegion(new Region(m_clientRectangle), m_chartAreaTooltip, "ChartArea region"));
				}

				g.TextRenderingHint = m_chart.TextRenderingHint;
				g.SmoothingMode = m_chart.SmoothingMode;

				if (this.IsPaintFlag(flags, ChartPaintFlags.Background))
				{
					BrushPaint.FillRectangle(g, m_clientRectangle, BackInterior);

					if (m_backImage != null)
					{
						g.DrawImage(m_backImage, m_clientRectangle);
					}
				}

				Rectangle clip = m_clientRectangle;
				clip.Width++;
				clip.Height++;
				g.SetClip(clip);

				if (this.DrawingMode == DrawingMode.Real3D)
				{
					Draw3D(e, flags);
				}
				else
				{
					Draw2D(e, flags);
				}

				g.ResetClip();

				if (this.IsPaintFlag(flags, ChartPaintFlags.Border))
				{
					Rectangle borderRect = new Rectangle(m_bounds.X, m_bounds.Y, m_bounds.Width-1, m_bounds.Height-1);
					DrawingHelper.DrawBorder(g, m_borderColor, m_borderWidth, borderRect, m_borderStyle);
				}
			}
		}
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DrawZoomingRange(Graphics g)
		{
			if ((ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
				&& (RenderBounds.Width > 0 && RenderBounds.Height > 0) && !(RealSeries3D && Series3D))
			{
				#region Zooming

				//HIGHLIGHTING OF ZOOMING RANGE

				Rectangle bounds = RenderBounds;

				if ((m_chart.EnableXZooming || m_chart.EnableYZooming) && (!m_chart.InteractiveCursorMouseDown))
				{
					Point pt = m_chart.MouseDownPosition;
					Point clickPoint = m_chart.ClickPoint;

					if (Control.MouseButtons == MouseButtons.Left && (clickPoint != Point.Empty) && (pt != Point.Empty))
					{
						Rectangle rect = new Rectangle(Math.Min(pt.X, clickPoint.X), Math.Min(pt.Y, clickPoint.Y), Math.Abs(pt.X - clickPoint.X), Math.Abs(pt.Y - clickPoint.Y));
						Rectangle tempRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

						// Setting highlighting rect to the axis area.
						rect.X = Math.Max(tempRect.Left, bounds.Left);
						rect.Y = Math.Max(tempRect.Top, bounds.Top);
						rect.Width = Math.Min(tempRect.Right, bounds.Right) - rect.X;
						rect.Height = Math.Min(tempRect.Bottom, bounds.Bottom) - rect.Y;

						if (!m_chart.EnableXZooming)
						{
							rect.X = bounds.X;
							rect.Width = bounds.Width;
							pt.X = -10;
						}

						if (!m_chart.EnableYZooming)
						{
							rect.Y = bounds.Y;
							rect.Height = bounds.Height;
							pt.Y = -10;
						}

						int alpha = (int)(byte.MaxValue * m_chart.Zooming.Opacity);
						BrushPaint.FillRectangle(g, rect, new BrushInfo(alpha, m_chart.Zooming.Interior));

						if (m_chart.Zooming.ShowBorder)
						{
							g.DrawRectangle(m_chart.Zooming.Border.Pen, rect);
						}
					}
				}
				#endregion
			}
		}
		#endregion

		/// <summary>
		/// Gets the series bounds.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		public RectangleF GetSeriesBounds(ChartSeries series)
		{
			if (m_divideArea && this.AxesType == ChartAreaAxesType.None)
			{
				int seriesIndex = m_chart.Model.Series.VisibleList.IndexOf(series);
				int seriesCount = m_chart.Model.Series.VisibleList.Count;

				return RenderingHelper.GetBounds(seriesIndex, seriesCount, this.RenderBounds);
			}

			return this.RenderBounds;
		}

		/// <summary>
		/// Returns the x axis associated with this chartseries.
		/// </summary>
		/// <param name="series">A ChartSeries whose ChartAxis we are interested in.</param>
		/// <returns>The corresponding ChartAxis.</returns>
		public ChartAxis GetXAxis(ChartSeries series)
		{
			return series.XAxis == null ? this.PrimaryXAxis : series.XAxis;
		}
		/// <summary>
		/// Returns the y axis associated with this chartseries.
		/// </summary>
		/// <param name="series">A ChartSeries whose ChartAxis we are interested in.</param>
		/// <returns>The corresponding ChartAxis.</returns>
		public ChartAxis GetYAxis(ChartSeries series)
		{
			return series.YAxis == null ? this.PrimaryYAxis : series.YAxis;
		}
		/// <summary>
		/// Arranges the <see cref="ChartArea"/> elements.
		/// </summary>
		/// <param name="rect">The bounds of <see cref="ChartArea"/>.</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CalculateSizes(Rectangle rect)
		{
			m_bounds = rect;
			m_clientRectangle = rect;
			m_renderbounds = this.RenderBounds;

			if (m_borderStyle != BorderStyle.None || m_borderWidth > 0)
			{
				m_clientRectangle.Inflate(-m_borderWidth, -m_borderWidth);
				m_clientRectangle.Width--;
				m_clientRectangle.Height--;
			}

			using (Graphics g = m_chart.GetGraphics())
			{
				this.CalculateLabelSizes(g, m_clientRectangle);
				this.CalculateAxesSizes();

				this.CalculateLabelSizes(g, this.RenderBounds);
				this.CalculateAxesSizes();
			}

			m_renderbounds = this.RenderBounds;
		}

		#region Gets coordinate by real point and back
		/// <summary>
		/// Returns the chartpoint value at this real point (in client co-ordinates).
		/// </summary>
		/// <returns>The corresponding ChartPoint.</returns>
		public ChartPoint GetValueByPoint(Point pt)
		{
			ChartPoint res = null;

			if (RequireAxes)
			{
				if (this.RequireInvertedAxes)
				{
					res = GetValueByPointInversed(xAxis, yAxis, CorrectionTo(pt));
				}
				else
				{
					res = GetValueByPointNormal(xAxis, yAxis, CorrectionTo(pt));
				}
			}
			else if (m_chart.Radar)
			{
				res = GetValueByPointPolar(xAxis, yAxis, CorrectionTo(pt));
			}

			return res;
		}
		/// <summary>
        /// Gets the real point value at this chart point.
		/// </summary>
		/// <returns>The corresponding Point in client-coordinates.</returns>
		public Point GetPointByValue(ChartPoint cpt)
		{
			return CorrectionFrom(GetPointByValueInternal(xAxis, yAxis, cpt));
		}

        /// <summary>
        /// Gets the real point value at this chart point. Use this method when multiple axes are used in the chart
        /// </summary>
        /// <returns>The corresponding Point in client-coordinates.</returns>
        public Point GetPointByValue(ChartSeries series, ChartPoint cpt)
        {          
            return CorrectionFrom(GetPointByValueInternalMulAxes(series.ActualXAxis, series.ActualYAxis, cpt));
        }
        

		#endregion

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (m_axes != null)
			{
			m_axes.Changed -= new ChartListChangeHandler(OnAxesChanged);

			for (int i = m_axes.Count - 1; i >= 0; i--)
			{
				m_axes[i].Unsubscribe(this);
				m_axes[i].Dispose();
				m_axes.RemoveAt(i);
			}

			m_axes = null;
			}
            if(m_seriesStyle != null)
                m_seriesStyle = null;

			if (m_interactiveCursors != null)
			{
				m_interactiveCursors.Clear();
				m_interactiveCursors.Changed -= new ChartListChangeHandler(this.OnInteractiveCursorsChanged);
				m_interactiveCursors = null;
			}

			ChartPlotAreaMargins.Changed -= new EventHandler(OnChangingRedraw);
			ChartAreaMargins.Changed -= new EventHandler(OnChangingRedraw);

			m_3dSettings.Changed -= new EventHandler(OnChangingRedraw);
			m_chartAxesLabelInfoBar.Changed -= new EventHandler(OnChangingRedraw);
		}
		#endregion

		#region Implementation

		#region Helper methods of drawing in 2D mode
		/// <summary>
		/// Draws the 2D dimentions area.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		/// <param name="flags">The flags.</param>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void Draw2D(PaintEventArgs e, ChartPaintFlags flags)
		{
			Graphics g = e.Graphics;
			RectangleF startClip = g.ClipBounds;
			PixelOffsetMode startPixelOffsetMode = g.PixelOffsetMode;
			ChartAreaAxesType axesType = this.AxesType;
			ChartGDIGraph graph = new ChartGDIGraph(e.Graphics);
            ChartStripLineZorder zOrder = ChartStripLineZorder.Behind;

			if (axesType == ChartAreaAxesType.Rectangular)
			{
				this.DrawGrid(graph);

				if (m_chartAxesLabelInfoBar.Visible)
				{
					m_chartAxesLabelInfoBar.Draw(g, PrimaryXAxis, PrimaryYAxis);
				}
              g.SetClip(Rectangle.Inflate(this.RenderGlobalBounds, 1, 1), CombineMode.Intersect);            
				this.DrawStripLines(graph,zOrder);
			}
			else if (axesType == ChartAreaAxesType.Circular)
			{
                this.DrawRadarAxes(g, PrimaryXAxis, PrimaryYAxis);
			}

			if (m_watermark.ZOrder == ChartWaterMarkOrder.Behind)
			{
				if (m_series3D)
				{
					graph.PushTranfsorm();
					graph.Transform = this.BackMatrix;
					m_watermark.Draw(graph, this.RenderBounds);
					graph.PopTransform();
				}
				else
				{
					m_watermark.Draw(graph, this.RenderBounds);
				}
			}

            //// Fix of D10875
            //// g.PixelOffsetMode = PixelOffsetMode.Half;
            m_chart.Series.DrawSeries(g, m_chart);
            if (axesType == ChartAreaAxesType.Circular)
			{			
                yAxis.DrawAxis(g, this);
			}

			this.DrawCustomPoints(graph);

			g.PixelOffsetMode = startPixelOffsetMode;
			g.SetClip(startClip);

			if (axesType == ChartAreaAxesType.Rectangular
				&& (flags & ChartPaintFlags.Axes) == ChartPaintFlags.Axes)
			{
				this.DrawAxes(g);
			}
            if (axesType == ChartAreaAxesType.Rectangular)
            {
                zOrder = ChartStripLineZorder.Over;
                this.DrawStripLines(graph, zOrder);
            }
			if (m_watermark.ZOrder == ChartWaterMarkOrder.Over)
			{
				m_watermark.Draw(graph, this.RenderBounds);
			}

			#region Interacive cursors
			if (IsPaintFlag(flags, ChartPaintFlags.InteractiveCursors))
			{
				Rectangle bounds = this.RenderBounds;
                PointF curpos;
				foreach (ChartInteractiveCursor cursor in m_interactiveCursors)
				{
                   
                    if (cursor.MoveToChartArea == true && this.CursorReDraw == true)
                    {
                       
                        if (this.RenderBounds.Contains(this.CursorLocation) ||((this.CursorLocation.X==this.RenderBounds.Right && this.CursorLocation.Y <=this.RenderBounds.Bottom) || (this.CursorLocation.Y==this.RenderBounds.Bottom && this.CursorLocation.X <=this.RenderBounds.Right && this.CursorLocation.X >=this.RenderBounds.Left)))
                        {
                            curpos = this.CursorLocation;
                            cursor.LineLocation = curpos;
                            oldcurpos = curpos;
                        }
                        else
                        {
                            curpos = oldcurpos;
                            cursor.LineLocation = curpos;
                        }
                        
                    }
                    else
                    {
                        curpos = cursor.Location;
                    }
                    Color vColor = cursor.Color != m_defaultColor ? cursor.Color : cursor.VerticalCursorColor;
                    Color hColor = cursor.Color != m_defaultColor ? cursor.Color : cursor.HorizontalCursorColor;

                    if (cursor.CursorOrientation == InteractiveCursorOrientation.Horizontal)
                    {                        
                            using (Pen pen = new Pen(hColor))
                            {
                                g.DrawLine(pen, bounds.Left, curpos.Y, bounds.Right, curpos.Y);
                            }                 
                        if(cursor.ShowPointSymbol && cursor.MoveToChartArea)
                          DrawSymbol(curpos, g);
                    }
                    else if (cursor.CursorOrientation == InteractiveCursorOrientation.Vertical)
                    {
                            using (Pen pen = new Pen(vColor))
                            {
                                g.DrawLine(pen, curpos.X, bounds.Top, curpos.X, bounds.Bottom);
                            }
                       if(cursor.ShowPointSymbol && cursor.MoveToChartArea)
                           DrawSymbol(curpos, g);
                    }
                    else
                    {                    
                        using (Pen pen = new Pen(hColor))
                        {
                            g.DrawLine(pen, bounds.Left, curpos.Y, bounds.Right, curpos.Y);
                        }
                        using (Pen pen = new Pen(vColor))
                        {
                            g.DrawLine(pen, curpos.X, bounds.Top, curpos.X, bounds.Bottom);
                        }
                       if(cursor.ShowPointSymbol && cursor.MoveToChartArea)
                        DrawSymbol(curpos,g);
                    }
				}
			}
			#endregion
		}

        /// <summary>
        ///Draws the Symbol for each point
        /// </summary>
        /// <param name="curpos">The <see cref="System.Drawing.PointF"/>Get the Cursor position.</param>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/>graphics to draw the symbol.</param>        
        private void DrawSymbol(PointF curpos,Graphics g)
        {
            foreach (ChartSeries series in this.Chart.Series)
            {
                foreach (ChartPoint seriespoint in series.Points)
                {
                    ChartPoint cPoint = new ChartPoint(seriespoint.X, seriespoint.YValues[0]);
                    PointF point = this.GetPointByValue(cPoint);
                    if ((point.X == curpos.X || point.Y == curpos.Y) && (this.CursorReDraw == true))
                    {
                        int index = series.Points.IndexOf(cPoint);
                        if (m_seriesStyle.Symbol != null && m_seriesStyle.Symbol.Shape!=ChartSymbolShape.None)
                            series.Styles[index].Symbol = m_seriesStyle.Symbol;
                        else
                        {
                            series.Styles[index].Symbol.Shape = ChartSymbolShape.Circle;
                            series.Styles[index].Symbol.Color = series.Style.Interior != null ? series.Style.Interior.BackColor : series.BackColor;
                        }

                        RenderingHelper.DrawPointSymbol(g, series.Styles[index], point, false);
                    }

                }
            }
        }

        /// <summary>
        ///Sets the  customized symbol for series points when moving the interactive cursor. 
        /// </summary>
        /// <param name="symbolInfo">The ChartSymbolInfo </param>
     
        public void SetSeriesSymbolForCursor(ChartSymbolInfo symbolInfo)
        {
        
            m_seriesStyle.Symbol = symbolInfo;
        }
           
		/// <summary>
		/// Render the axes.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> axes render to.</param>
		private void DrawAxes(Graphics g)
		{
			foreach (ChartAxis axis in this.Axes)
			{
				if (axis.IsVisible)
				{
					axis.DrawAxis(g, this);
				}

				axis.DrawBreaks(g, m_renderbounds);
			}
		}
		/// <summary>
		/// Draws the radar axes.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> axes render to.</param>
		/// <param name="xAxis">The X axis.</param>
		/// <param name="yAxis">The Y axis.</param>
		private void DrawRadarAxes(Graphics g, ChartAxis xAxis, ChartAxis yAxis)
		{
			PointF center = Center;
			bool drawTickLabels = PrimaryXAxis.TickLabelsDrawingMode != ChartAxisTickLabelDrawingMode.None;
			int ycount = yAxis.Range.NumberOfIntervals;

			float radius = GetRadarRadius(drawTickLabels ? PrimaryXAxis.Font.Height : 0);
			bool isRadar = (m_chart.RadarStyle == ChartRadarAxisStyle.Polygon) && (!m_chart.Polar);
			if (radius <= 0) return;

			int count = xAxis.IsIndexed ? m_chart.IndexValues.Count : xAxis.Range.NumberOfIntervals;

			GraphicsPath gp = GetRadarPath(center, radius, isRadar, count);
			BrushPaint.FillPath(g, gp, m_gridInterior);

            if (InteriorBackImage != null)
            {
                g.SetClip(gp);
                RectangleF rect = gp.GetBounds();
                g.DrawImage(InteriorBackImage, rect);
                g.ResetClip();
            }

            bool isRadarCircle = (m_chart.RadarStyle == ChartRadarAxisStyle.Circle) && (!m_chart.Polar);
            bool isPolar = m_chart.Polar;     
            if (isPolar)
            {
                count = 12;
            }

			for (int i = 0; i <= count; i++)    
			{
                double labelAngle = i * ChartMath.DblPI / count;               
               
                double angle = ChartArea.CircularChartOffset + labelAngle;       				

				float x = center.X + (float)Math.Cos(angle) * radius;
				float y = center.Y - (float)Math.Sin(angle) * radius;

                Pen pen = xAxis.LineType.Pen;               
                
                  if ((xAxis.Pens != null) && (xAxis.Pens.Length > 0))
                  {
                     pen = this.GetRadarPen(xAxis, i);

                     if (i == 0 || i== count)
                     {
                       pen = yAxis.LineType.Pen;
                     }
                  }

                g.DrawLine(pen, center, new PointF(x, y));
                
				if (drawTickLabels)
				{
                    ChartAxisLabel label = null;

                    if (isRadar || isRadarCircle) 
					{
						if (xAxis.IsIndexed)
						{
							label = this.PrimaryXAxis.GenerateLabel(i, this, i);
						}
						else
						{
							double min = xAxis.Range.Min;
							double max = xAxis.Range.Max;
							double interval = this.PrimaryXAxis.Range.Interval;
							double v = (min + interval * i) - xAxis.Offset;

                            if (xAxis.Inversed)
                            {
                                v = (max - interval * i) - xAxis.Offset;
                            }
                            else
                            {
                                v = (min + interval * i) - xAxis.Offset;
                            }

							label = this.PrimaryXAxis.GenerateLabel(v, this, i);
						}
					}
					else
					{
						label = this.PrimaryXAxis.GenerateLabel(labelAngle, this, i);

						if ((isPolar && (xAxis.RangeType == ChartAxisRangeType.Auto)) || xAxis.IsIndexed)                        
                        {                           
                            if (xAxis.Inversed)
                            {
                                double lblv = ChartMath.DblPI - (ChartMath.DblPI / 12 * i);
                                label = this.PrimaryXAxis.GenerateLabel(lblv, this, i);
                            }
                            else
                            {
                                label = this.PrimaryXAxis.GenerateLabel(labelAngle, this, i);
                            }
                        }

                        else if (isPolar)
                        {
                            double min = xAxis.Range.Min;
                            double max = xAxis.Range.Max;
                            double interval = this.PrimaryXAxis.Range.Interval;
                            double del = (max > min) ? (max - min) : (min - max);
                            double v = del / 12 * i;
                           
                            if (xAxis.Inversed)
                            {
                              v = max - (del / 12 * i);                               
                            }
                            else
                            {                                                                
                              v = del / 12 * i;                                
                            }

                            label = this.PrimaryXAxis.GenerateLabel(v, this, i);
                        }  
					}
                                        
					SizeF sz = label.Measure(g, this.PrimaryXAxis);

					float tx = center.X + (float)Math.Cos(angle) * (radius + 5 + Math.Abs(sz.Height / 2f * (float)Math.Sin(angle)) + Math.Abs(sz.Width / 2f * (float)(Math.Cos(angle)))) - sz.Width / 2f;
					float ty = center.Y - (float)Math.Sin(angle) * (radius + 5 + Math.Abs(sz.Height / 2f * (float)Math.Sin(angle)) + Math.Abs(sz.Width / 2f * (float)(Math.Cos(angle)))) - sz.Height / 2f;

					if (i == 0)
					{
						tx -= (float)(sz.Width * Math.Sin(angle)) / 2;
						ty -= (float)(sz.Height * Math.Cos(angle)) / 2;
					}
					if (i == count)
					{
						tx += (float)(sz.Width * Math.Sin(angle)) / 2;
						ty += (float)(sz.Height * Math.Cos(angle)) / 2;
					}

					using (Brush brush = new SolidBrush(label.Color))                    
					{
						g.DrawString(label.Text, label.Font, brush, tx, ty);
					}                                     
				}
			}

			for (int i = 1; i < ycount + 1; i++)
			{
				float rad = i * (radius / ycount);
                Pen pen = yAxis.LineType.Pen;               

                if ((yAxis.Pens != null) && (yAxis.Pens.Length > 0))
                {
                    pen = this.GetRadarPen(yAxis, i - 1);

                    if (i == ycount)
                    {
                        pen = xAxis.LineType.Pen;
                    }
                }                

                g.DrawPath(pen, this.GetRadarPath(center, rad, isRadar, count));
			}

            yAxis.DrawAxis(g, this);
		}
		/// <summary>
		/// Draws the custom points.
		/// </summary>
		/// <param name="graph">The <see cref="ChartGraph"/> points render to.</param>
		private void DrawCustomPoints(ChartGraph graph)
		{
			foreach (ChartCustomPoint ccp in this.CustomPoints)
			{
				ccp.Draw(this, graph, this.GetCustomPointLocation(ccp));
			}
		}

        /// <summary>
        /// This method will return the specified pen from the specified axis's Pens collection.
        /// Currently, This method is used only for Polar and Radar chart types.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="index">The index of the pen in the Axis's Pens collection.</param>
        internal Pen GetRadarPen(ChartAxis axis, int index)
        {
            Pen pen = null;
            
            pen = axis.Pens[index % axis.Pens.Length];

            if (pen==null)
            {            
             pen = axis.LineType.Pen;
            }

            return pen;
        }

		/// <summary>
		/// Draws the grid.
		/// </summary>
		/// <param name="graph">The graph.</param>
		private void DrawGrid(ChartGraph graph)
		{
            bounds= RenderBounds;
            Rectangle cbounds = bounds;
            RectangleF hWallRect;
			if (m_series3D)
			{
                graph.PushTranfsorm();
                graph.MultiplyTransform(this.BackMatrix);
                this.DrawSimpleGrid(graph, bounds, null);
                graph.PopTransform();
                if (this.RequireInvertedAxes)
                {
                    if (!double.IsNaN(this.PrimaryXAxis.Crossing))
                    {
                        this.PrimaryXAxis.Crossing = (this.PrimaryXAxis.Crossing == Double.MaxValue) ? this.PrimaryYAxis.Range.Max : (this.PrimaryXAxis.Crossing == Double.MinValue) ? this.PrimaryYAxis.Range.Min : this.PrimaryXAxis.Crossing;
                        ChartPoint cpt = new ChartPoint(this.PrimaryXAxis.Crossing, this.PrimaryXAxis.Range.Min);
                        Point pt = this.GetPointByValue(cpt);
                        bounds = new Rectangle(cbounds.X, pt.Y, bounds.Width, bounds.Height);                   //Assign the Crossed Y value to bounds.
                        hWallRect = new RectangleF(bounds.Left, bounds.Y - OffsetY, bounds.Width, OffsetY);     //Modified the Horizontal Rect as per the bounds value.
                    }
                    else
                    {
                        hWallRect = new RectangleF(bounds.Left, bounds.Bottom - OffsetY, bounds.Width, OffsetY);  //Default horizontal rect value when without crossing.
                    }

                }
                else
                {
                    if (!double.IsNaN(this.PrimaryXAxis.Crossing))
                    {
                        this.PrimaryXAxis.Crossing = (this.PrimaryXAxis.Crossing == Double.MaxValue) ? this.PrimaryYAxis.Range.Max : (this.PrimaryXAxis.Crossing == Double.MinValue) ? this.PrimaryYAxis.Range.Min : this.PrimaryXAxis.Crossing;
                        ChartPoint cpt = new ChartPoint(this.PrimaryXAxis.Range.Min,this.PrimaryXAxis.Crossing);
                        Point pt = this.GetPointByValue(cpt);
                        bounds = new Rectangle(cbounds.X, pt.Y, bounds.Width, bounds.Height);                   //Assign the Crossed Y value to bounds.
                        hWallRect = new RectangleF(bounds.Left, bounds.Y - OffsetY, bounds.Width, OffsetY);     //Modified the Horizontal Rect as per the bounds value.
                    }
                    else
                    {
                        hWallRect = new RectangleF(bounds.Left, bounds.Bottom - OffsetY, bounds.Width, OffsetY);  //Default horizontal rect value when without crossing.
                    }
                }
                if (this.OffsetY > 0.5f)
                {
                    graph.PushTranfsorm();
                    graph.MultiplyTransform(this.BottomMatrix);
                    this.DrawSimpleGrid(graph, hWallRect, ChartOrientation.Horizontal);
                    graph.PopTransform();
                }
                if (this.RequireInvertedAxes)
                {
                    if (!double.IsNaN(this.PrimaryYAxis.Crossing))
                    {
                        this.PrimaryYAxis.Crossing = (this.PrimaryYAxis.Crossing == Double.MaxValue) ? this.PrimaryXAxis.Range.Max : (this.PrimaryYAxis.Crossing == Double.MinValue) ? this.PrimaryXAxis.Range.Min : this.PrimaryYAxis.Crossing;
                        ChartPoint cpt = new ChartPoint(this.PrimaryYAxis.Range.Min, this.PrimaryYAxis.Crossing);
                        Point pt = this.GetPointByValue(cpt);
                        bounds = new Rectangle(pt.X, cbounds.Y, bounds.Width, bounds.Height);       
                    }
                }
                else
                {
                    if (!double.IsNaN(this.PrimaryYAxis.Crossing))
                    {
                        this.PrimaryYAxis.Crossing = (this.PrimaryYAxis.Crossing == Double.MaxValue) ? this.PrimaryXAxis.Range.Max : (this.PrimaryYAxis.Crossing == Double.MinValue) ? this.PrimaryXAxis.Range.Min : this.PrimaryYAxis.Crossing;
                        ChartPoint cpt = new ChartPoint(this.PrimaryYAxis.Crossing,this.PrimaryYAxis.Range.Min);
                        Point pt = this.GetPointByValue(cpt);
                        bounds = new Rectangle(pt.X, cbounds.Y, bounds.Width, bounds.Height);              //Assign the Crossed X value to bounds.
                    }
                }

                 RectangleF vWallRect = new RectangleF(bounds.Left, cbounds.Top, OffsetX, bounds.Height); //Modified vertical rect value as per the bounds value.
                 if (this.OffsetX > 0.5f)
                 {
                     graph.PushTranfsorm();
                     graph.MultiplyTransform(this.LeftMatrix);
                     this.DrawSimpleGrid(graph, vWallRect, ChartOrientation.Vertical);
                     graph.PopTransform();
                 }
			}
			else
			{
				this.DrawSimpleGrid(graph, bounds, null);
			}
		}
		/// <summary>
		/// Draws the strip lines.
		/// </summary>
		/// <param name="graph">The <see cref="ChartGraph"/>.</param>
        /// <param name="zOrder"></param>
		private void DrawStripLines(ChartGraph graph,ChartStripLineZorder zOrder)
		{
			foreach (ChartAxis axis in this.Axes)
			{
				foreach (ChartStripLine stripLine in axis.StripLines)
				{
					if (stripLine.Enabled)
					{
                        if (stripLine.ZOrder == zOrder)
                        {
                            stripLine.Draw(graph, this.GetStripLineRects(axis, stripLine));
                        }

					}
				}
			}
		}
		/// <summary>
		/// Draws the axis lines.
		/// </summary>
		/// <param name="graph">The graph.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="orientation">The orientation.</param>
		private void DrawSimpleGrid(ChartGraph graph, RectangleF bounds, ChartOrientation? orientation)
		{
			graph.DrawRect(m_gridInterior, null, bounds);

            if (InteriorBackImage != null)
            {
                graph.DrawImage(InteriorBackImage, bounds);
            }

			foreach (ChartAxis axis in this.Axes)
			{
				if( !orientation.HasValue || orientation.Value == axis.Orientation )
				{
					axis.DrawInterlacedGrid(graph, bounds);
				}
			}

			foreach (ChartAxis axis in this.Axes)
			{
				if( !orientation.HasValue || orientation.Value == axis.Orientation )
				{
					axis.DrawGridLines(graph, bounds);
				}
			}
		}
		/// <summary>
		/// Gets the radar path.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="radius">The radius.</param>
		/// <param name="isRadar">if set to <c>true</c> [is radar].</param>
		/// <param name="count">The count.</param>
		/// <returns></returns>
		private GraphicsPath GetRadarPath(PointF center, float radius, bool isRadar, int count)
		{
			GraphicsPath gp = new GraphicsPath();

			if (isRadar)
			{
				PointF[] ptArray = new PointF[count];

				for (int i = 0; i < count; i++)
				{
					double angle = i * 2f * Math.PI / count + Math.PI / 2f;

					ptArray[i] = new PointF(center.X + (float)(radius * Math.Cos(angle)),
						center.Y - (float)(radius * Math.Sin(angle)));
				}

				gp.AddPolygon(ptArray);
			}
			else
			{
				gp.AddArc(center.X - radius, center.Y - radius, 2 * radius, 2 * radius, 0, 360);
			}

			return gp;
		}
		/// <summary>
		/// Gets the strip line rects.
		/// </summary>
		/// <param name="axis">The axis.</param>
		/// <param name="stripLine">The strip line.</param>
		/// <returns></returns>
		private RectangleF[] GetStripLineRects(ChartAxis axis, ChartStripLine stripLine)
		{
			MinMaxInfo axisRange = axis.VisibleRange;
			Rectangle bounds = this.RenderBounds;

			double startStrip = stripLine.StartAtAxisPosition ? axis.Range.Min + stripLine.Offset : stripLine.Start;
			double endStrip = stripLine.StartAtAxisPosition ? axis.Range.Max : stripLine.End;
			ArrayList rects = new ArrayList();
                     
			for (double x = startStrip; (stripLine.Period == 0) || (stripLine.Period > 0 && x < endStrip)
				|| (stripLine.Period < 0 && x > endStrip); x += stripLine.Period)
			{
				double from = x;
                double to = x + stripLine.Width;                               

				if (axis.IsIndexed)
				{
					from = m_chart.IndexValues.GetIndex(from);
					to = m_chart.IndexValues.GetIndex(to);
				}
				
                if ((axisRange.Contains(from) || axisRange.Contains(to)) || (axisRange.Contains(from) && (stripLine.FixedWidth!=0)))
				{
					float start = axis.GetCoordinateFromValue(from);
					float end = axis.GetCoordinateFromValue(to);

                    if (stripLine.FixedWidth != 0)
                    {
                        end = start + (float)stripLine.FixedWidth;
                    }

					if (axis.Orientation == ChartOrientation.Horizontal)
					{
						rects.Add(ChartMath.CorrectRect(start, bounds.Top, end, bounds.Bottom));
					}
					else
					{
						rects.Add(ChartMath.CorrectRect(bounds.Left, start, bounds.Right, end));
					}
				}

				if (stripLine.Period == 0)
				{
					break;
				}
			}
            
			return rects.ToArray(typeof(RectangleF)) as RectangleF[];
		}
		/// <summary>
		/// Gets the custom point location.
		/// </summary>
		/// <param name="customPoint">The custom point.</param>
		/// <returns></returns>
		private PointF GetCustomPointLocation(ChartCustomPoint customPoint)
		{
			PointF pt = PointF.Empty;

			switch (customPoint.CustomType)
			{
				case ChartCustomPointType.Percent:
					pt = new PointF((float)(Width * customPoint.XValue / 100), Height - (float)(Height * customPoint.YValue / 100));
					break;

				case ChartCustomPointType.Pixel:
					pt = new PointF((float)customPoint.XValue, (float)customPoint.YValue);
					break;

				case ChartCustomPointType.ChartCoordinates:
				    if ((customPoint.SeriesIndex >= 0))
                    		       {
                        		//For multiple axes custom point
                       			 int seriesIndex = customPoint.SeriesIndex;
                       			 ChartSeries series = m_chart.Series[seriesIndex];
                        		 ChartPoint chartPoint = new ChartPoint(customPoint.XValue, customPoint.YValue);
                        		 pt = this.GetPointByValue(series, chartPoint);
                   		       }
					else
                		       {
					pt = this.GetPointByValueInternal(xAxis, yAxis, new ChartPoint(customPoint.XValue, customPoint.YValue));
				       }	
					break;

				case ChartCustomPointType.PointFollow:
					{
						int serIndex = customPoint.SeriesIndex;
						int ptIndex = customPoint.PointIndex;

						if (serIndex >= 0 && serIndex < m_chart.Series.Count && ptIndex >= 0 && ptIndex < m_chart.Series[serIndex].Points.Count)
						{
							ChartSeries series = m_chart.Series[serIndex];

							pt = this.GetPointByValueInternal(series.XAxis, series.YAxis, series.Points[ptIndex]);
						}
					}
					break;
			}

			return pt;
		}
		#endregion

		#region Helper methods of drawing in 3D mode
		/// <summary>
		/// Draws the 3D dimentions area.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		/// <param name="flags">The flags.</param>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void Draw3D(PaintEventArgs e, ChartPaintFlags flags)
		{
			ChartAreaAxesType axesType = this.AxesType;
			Graphics3D g3d = new Graphics3D(e.Graphics);

			g3d.LoadState(m_3dSettings);
			g3d.Regions = m_chart.ChartRegions;
			g3d.Transform = this.UpdateTransform3D();

			Path3DCollect workPlane = CreateWorkPlane(-m_depth);

			if (!m_leaveTree || rootNode == null)
			{
				ChartSeriesCollection series = m_chart.Series;
                ChartStripLineZorder zorder_3d = ChartStripLineZorder.Behind;
				if (axesType == ChartAreaAxesType.Rectangular)
				{
					this.DrawAxes(g3d);
					this.DrawGrid(g3d);

					series.DrawSeriesNamesInDepth(g3d, m_chart);

					if (m_chartAxesLabelInfoBar.Visible)
					{
						m_chartAxesLabelInfoBar.Draw(g3d, PrimaryXAxis, PrimaryYAxis);
					}
					// Clipping is implemented here via Clip polygongs.
					// These polygons are added below and they affect all polygons
					// which are added after.
					#region clipping
					Rectangle bounds1 = GetGlobalBoundsByRect(m_clientRectangle);
					g3d.AddPolygon(new Polygon(new Vector3D[]{
                                                       new Vector3D( bounds1.Left, bounds1.Top, 0 ),
                                                       new Vector3D( bounds1.Left, bounds1.Top, m_depth ),
                                                       new Vector3D( bounds1.Left, bounds1.Bottom, m_depth ),
                                                       new Vector3D( bounds1.Left, bounds1.Bottom, 0 ) }, true));

					g3d.AddPolygon(new Polygon(new Vector3D[]{
                                                       new Vector3D( bounds1.Left, bounds1.Top, 0 ),
                                                       new Vector3D( bounds1.Right, bounds1.Top, 0 ),
                                                       new Vector3D( bounds1.Right, bounds1.Top, m_depth ),
                                                       new Vector3D( bounds1.Left, bounds1.Top, m_depth ) }, true));

					g3d.AddPolygon(new Polygon(new Vector3D[]{
                                                       new Vector3D( bounds1.Left, bounds1.Bottom, 0 ),
                                                       new Vector3D( bounds1.Left, bounds1.Bottom, m_depth ),
                                                       new Vector3D( bounds1.Right, bounds1.Bottom, m_depth ),
                                                       new Vector3D( bounds1.Right, bounds1.Bottom, 0 ) }, true));

					g3d.AddPolygon(new Polygon(new Vector3D[]{
                                                       new Vector3D( bounds1.Right, bounds1.Top, 0 ),
                                                       new Vector3D( bounds1.Right, bounds1.Bottom, 0 ),
                                                       new Vector3D( bounds1.Right, bounds1.Bottom, m_depth ),
                                                       new Vector3D( bounds1.Right, bounds1.Top, m_depth ) }, true));
					#endregion clipping

					this.DrawStripLines(g3d,zorder_3d);
				}
				else if (axesType == ChartAreaAxesType.Circular)
				{
					DrawRadarAxes(g3d, PrimaryXAxis, PrimaryYAxis);
				}

				series.DrawSeries(g3d, m_chart);

                if (axesType == ChartAreaAxesType.Circular)
				{					
                    yAxis.DrawAxis(g3d, this, -1); 
				}

				this.DrawCustomPoints(g3d);
                if (axesType == ChartAreaAxesType.Rectangular)
                {
                    zorder_3d = ChartStripLineZorder.Over;
                    this.DrawStripLines(g3d, zorder_3d);
                }
				if (m_watermark.ZOrder == ChartWaterMarkOrder.Over )//|| this.AxesType == ChartAreaAxesType.None )
				{
					g3d.AddPolygon(m_watermark.Draw(g3d, this.RenderBounds, 0));
				}
				else
				{
					if( m_chart.Model.FirstSeries != null )
					{
						float depth = m_chart.Model.FirstSeries.Renderer.GetTotalDepth();
						g3d.AddPolygon(m_watermark.Draw(g3d, this.RenderBounds, depth + 1));
					}
					else
					{
						g3d.AddPolygon(m_watermark.Draw(g3d, this.RenderBounds, m_depth));
					}
				}

				#region Zooming

				//HIGHLIGHTING OF ZOOMING RANGE

				Rectangle bounds = RenderBounds;

				if ((m_chart.EnableXZooming || m_chart.EnableYZooming) && (!m_chart.InteractiveCursorMouseDown))
				{
					Point pt = m_chart.MouseDownPosition;
					Point clickPoint = m_chart.ClickPoint;

					if (Control.MouseButtons == MouseButtons.Left && (clickPoint != Point.Empty) && (pt != Point.Empty))
					{
						Point p1 = CorrectionTo(pt);
						Point p2 = CorrectionTo(clickPoint);

						float minX = m_chart.EnableXZooming ? Math.Max(Math.Min(p1.X, bounds.Right), bounds.Left) : bounds.Left;
						float maxX = m_chart.EnableXZooming ? Math.Max(Math.Min(p2.X, bounds.Right), bounds.Left) : bounds.Right;
						float minY = m_chart.EnableYZooming ? Math.Max(Math.Min(p1.Y, bounds.Bottom), bounds.Top) : bounds.Top;
						float maxY = m_chart.EnableYZooming ? Math.Max(Math.Min(p2.Y, bounds.Bottom), bounds.Top) : bounds.Bottom;

						int alpha = (int)(byte.MaxValue * m_chart.Zooming.Opacity);
						BrushInfo brushInfo = new BrushInfo(alpha, m_chart.Zooming.Interior);
						Pen borderPen = m_chart.Zooming.ShowBorder ? m_chart.Zooming.Border.Pen : null;

						Polygon plg = new Polygon(new Vector3D[]{ new Vector3D( minX, minY, 0 ),
                                                       new Vector3D( minX, maxY, 0 ),
                                                       new Vector3D( maxX, maxY, 0 ),
                                                       new Vector3D( maxX, minY, 0 ) }, brushInfo, borderPen);

						workPlane.Add(plg);
					}
				}
				//END HIGHLIGHTING OF ZOOMING RANGE

				#endregion

				#region interacive cursors
				if (IsPaintFlag(flags, ChartPaintFlags.InteractiveCursors))
				{
					for (int i = 0; i < m_interactiveCursors.Count; i++)
					{
						PointF curpos = m_interactiveCursors[i].Location;
						Pen p = new Pen(m_interactiveCursors[i].Color, 1);
						Polygon plg1 = new Polygon(new Vector3D[]{ new Vector3D( bounds.Left, curpos.Y, 0 ), 
                                                      new Vector3D( bounds.Right , curpos.Y, 0 ),
                                                      new Vector3D( bounds.Right , curpos.Y+1, 0 ),
                                                      new Vector3D( bounds.Left , curpos.Y+1, 0 )}, p);
						Polygon plg2 = new Polygon(new Vector3D[]{ new Vector3D( curpos.X, bounds.Top, 0 ), 
                                                      new Vector3D( curpos.X, bounds.Bottom, 0 ),
                                                      new Vector3D( curpos.X+1, bounds.Bottom, 0 ),
                                                      new Vector3D( curpos.X+1, bounds.Top, 0 )}, p);
						workPlane.Add(plg1);
						workPlane.Add(plg2);
					}
				}
				#endregion

				g3d.AddPolygon(workPlane);
				g3d.PrepairView();

				if (m_chart.NeedRegionUpdate || !m_chart.CalcRegions)
				{
					rootNode = g3d.RootNode;
				}
				else
				{
					rootNode = null;
				}
			}
			else
			{
				g3d.RootNode = rootNode;
			}

			g3d.View3D();

			//m_leaveTree = true;
		}
		/// <summary>
		/// Draws the grid.
		/// </summary>
		/// <param name="g">The <see cref="Graphics3D"/>.</param>
		private void DrawGrid(Graphics3D g)
		{
			Rectangle bounds = this.RenderGlobalBounds;
			Path3DCollect backWall = new Path3DCollect(Polygon.CreateRectangle(bounds, m_depth, m_gridInterior, null));

			foreach( ChartAxis axis in this.Axes)
			{
				backWall.Add(axis.DrawInterlacedGrid(g, bounds, m_depth));
			}

			foreach (ChartAxis axis in this.Axes)
			{
				backWall.Add(axis.DrawGridLines(g, bounds, m_depth));
			}

			g.AddPolygon(backWall);

			if (m_depth > 0)
			{
				Rectangle hWallBounds = new Rectangle(bounds.Left, -(int)m_depth, bounds.Width, (int)m_depth);
				Rectangle vWallBounds = new Rectangle(-(int)m_depth, bounds.Top, (int)m_depth, bounds.Height);

				foreach (ChartAxis axis in this.Axes)
				{
					if (axis.Orientation == ChartOrientation.Horizontal)
					{
						Path3DCollect wallPoly = new Path3DCollect(Polygon.CreateRectangle(hWallBounds, axis.Location.Y, m_gridInterior, null));
						wallPoly.Add(axis.DrawInterlacedGrid(g, hWallBounds, axis.Location.Y));
						wallPoly.Add(axis.DrawGridLines(g, hWallBounds, axis.Location.Y));

						if (wallPoly != null)
						{
							wallPoly.Transform(Matrix3D.Tilt((float)Math.PI / 2));
							g.AddPolygon(wallPoly);
						}
					}
					else
					{
						Path3DCollect wallPoly = new Path3DCollect(Polygon.CreateRectangle(vWallBounds, axis.Location.X, m_gridInterior, null));
						wallPoly.Add(axis.DrawInterlacedGrid(g, vWallBounds, axis.Location.X));
						wallPoly.Add(axis.DrawGridLines(g, vWallBounds, axis.Location.X));

						if (wallPoly != null)
						{
							wallPoly.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
							g.AddPolygon(wallPoly);
						}
					}
				}
			}
		}
		/// <summary>
		/// Draws the axes.
		/// </summary>
		/// <param name="g">The <see cref="Graphics3D"/>.</param>
		private void DrawAxes(Graphics3D g)
		{
			foreach (ChartAxis axis in this.Axes)
			{
				if (axis.IsVisible)
				{
					axis.DrawAxis(g, this, 0f);
				}

				axis.DrawBreaks(g, this.RenderBounds);
			}
		}
		/// <summary>
		/// Draws the radar axes.
		/// </summary>
		/// <param name="g">The <see cref="Graphics3D"/>.</param>
		/// <param name="xAxis">The X axis.</param>
		/// <param name="yAxis">The Y axis.</param>
		private void DrawRadarAxes(Graphics3D g, ChartAxis xAxis, ChartAxis yAxis)
		{
			PointF center = Center;
			int ycount = PrimaryYAxis.Range.NumberOfIntervals;

			int count = this.PrimaryXAxis.Range.NumberOfIntervals;
			if (this.PrimaryXAxis.IsIndexed)
				count = m_chart.IndexValues.Count;

			bool isRadar = m_chart.RadarStyle == ChartRadarAxisStyle.Polygon && (!m_chart.Polar);
			float radius = GetRadarRadius(xAxis.Font.Height);
			if (radius <= 0) return;

			Path3DCollect p3dc = CreateWorkPlane(m_depth);
			p3dc.Add(Path3D.FromGraphicsPath(GetRadarPath(center, radius, isRadar, count), m_depth, m_gridInterior));
			GraphicsPath gp = new GraphicsPath();

			for (int i = 1; i < ycount + 1; i++)
			{                                   
               if (yAxis.Pens != null)
               {
                    GraphicsPath gp2 = new GraphicsPath();
                    gp2.AddPath(GetRadarPath(center, i * (radius / ycount), isRadar, count), false);
                    Pen pen = yAxis.LineType.Pen;
                    if ((yAxis.Pens != null) && (yAxis.Pens.Length > 0))
                    {
                        pen = this.GetRadarPen(yAxis, i - 1);

                        if (i == 0 || i == count)
                        {
                            pen = xAxis.LineType.Pen;
                        }
                    }

                    p3dc.Add(Path3D.FromGraphicsPath(gp2, m_depth, pen));
                }                
                else
                {
                    gp.AddPath(GetRadarPath(center, i * (radius / ycount), isRadar, count), false);
                }
			}

            if(yAxis.Pens==null)
              p3dc.Add(Path3D.FromGraphicsPath(gp, m_depth, yAxis.LineType.Pen));

			gp = new GraphicsPath();
            
			double startAngle = ChartArea.CircularChartOffset;
			double yAxisAngle = startAngle;           

            bool isRadarCircle = (m_chart.RadarStyle == ChartRadarAxisStyle.Circle) && (!m_chart.Polar);
            bool isPolar = m_chart.Polar;       
            if (isPolar)
            {
                count = 12;
            }

			for (int i = 0; i <= count; i++)
			{
				double angle = i * 2f * Math.PI / count + startAngle;

				ChartAxisLabel label = null;

                if (isRadar || isRadarCircle)
				{
					if (xAxis.IsIndexed)
					{
						label = this.PrimaryXAxis.GenerateLabel(i, this, i);
					}
					else
					{
						double min = xAxis.Range.Min;
						double max = xAxis.Range.Max;
						double interval = this.PrimaryXAxis.Range.Interval;
						double v = (min + interval * i) - xAxis.Offset;

                        if (xAxis.Inversed)
                        {
                            v = (max - interval * i) - xAxis.Offset;
                        }
                        else
                        {
                            v = (min + interval * i) - xAxis.Offset;
                        }

						label = this.PrimaryXAxis.GenerateLabel(v, this, i);
					}
				}
				else
				{
					label = this.PrimaryXAxis.GenerateLabel(angle - ChartArea.CircularChartOffset, this, i);

					if ((isPolar && (xAxis.RangeType == ChartAxisRangeType.Auto)) || xAxis.IsIndexed)                    
                    {                      
                        if (xAxis.Inversed)
                        {                             
                            double lblv = ChartMath.DblPI - (ChartMath.DblPI / 12 * i);
                            label = this.PrimaryXAxis.GenerateLabel(lblv, this, i);
                        }
                        else
                        {
                            label = this.PrimaryXAxis.GenerateLabel(angle - ChartArea.CircularChartOffset, this, i);
                        }
                    }
                    else if (isPolar) 
                    {
                        double min = xAxis.Range.Min;
                        double max = xAxis.Range.Max;
                        double del = (max > min) ? (max - min) : (min - max);
                        double v = del / 12 * i;

                        if (xAxis.Inversed)
                        {                            
                           v = max - (del / 12 * i);                           
                        }
                        else
                        {                            
                           v = del / 12 * i;                            
                        }

                        label = this.PrimaryXAxis.GenerateLabel(v, this, i);
                    } 
				}

				SizeF sz = label.Measure(g.Graphics, this.PrimaryXAxis);

				float x = center.X + (float)Math.Cos(angle) * radius;
				float y = center.Y - (float)Math.Sin(angle) * radius;

				float tx = center.X + (float)Math.Cos(angle) * (radius + 5 + Math.Abs(sz.Height / 2f * (float)Math.Sin(angle)) + Math.Abs(sz.Width / 2f * (float)(Math.Cos(angle)))) - sz.Width / 2f;
				float ty = center.Y - (float)Math.Sin(angle) * (radius + 5 + Math.Abs(sz.Height / 2f * (float)Math.Sin(angle)) + Math.Abs(sz.Width / 2f * (float)(Math.Cos(angle)))) /*- sz.Height / 2f*/;
				tx += sz.Width / 2;//fixing microsoft bug
				if (i == 0)
				{
					tx -= (float)(sz.Width * Math.Sin(angle)) / 2;
					ty -= (float)(sz.Height * Math.Cos(angle)) / 2;
				}
				if (i == count)
				{
					tx += (float)(sz.Width * Math.Sin(angle)) / 2;
					ty += (float)(sz.Height * Math.Cos(angle)) / 2;
				}

				gp.AddLine(center, new PointF(x, y));                
				GraphicsPath lgp = new GraphicsPath();
				Brush brush = new SolidBrush(xAxis.ForeColor);
				Font ft = label.Font;
				lgp.AddString(label.Text, ft.FontFamily, (int)ft.Style, RenderingHelper.GetFontSizeInPixels(ft), new PointF(tx, ty), StringFormat.GenericDefault);
				p3dc.Add(Path3D.FromGraphicsPath(lgp, m_depth, brush));

                if (xAxis.Pens != null)
                {
                    GraphicsPath gp2 = new GraphicsPath();
                    gp2.AddLine(center, center); // added this line to improve the appearance of  XAxis's grid lines.
                    Pen pen = xAxis.LineType.Pen;
                    if (xAxis.Pens.Length > 0)
                    {
                        pen = this.GetRadarPen(xAxis, i);

                        if (i == 0 || i == count)
                        {
                            pen = yAxis.LineType.Pen;
                        }
                    }

                    gp2.AddLine(center, new PointF(x, y));
                    p3dc.Add(Path3D.FromGraphicsPath(gp2, m_depth, pen));
                    g.AddPolygon(p3dc);                                                         
                }                
			}

            if (xAxis.Pens == null)
            {
                p3dc.Add(Path3D.FromGraphicsPath(gp, m_depth, xAxis.LineType.Pen));
                g.AddPolygon(p3dc);
            }

            yAxis.DrawAxis(g, this, -1);
		}
		/// <summary>
		/// Updates the real 3D transform.
		/// </summary>
		/// <returns></returns>
		private Transform3D UpdateTransform3D()
		{
			m_transform3D.SetCenter(new Vector3D(Left + Width / 2, Top + Height / 2, m_depth / 2));

			m_transform3D.View = Matrix3D.Transform(0, 0, m_depth);
			m_transform3D.View *= Matrix3D.Turn(-ChartMath.ToRadians * Rotation);
			m_transform3D.View *= Matrix3D.Tilt(-ChartMath.ToRadians * Tilt);
			m_transform3D.View *= Matrix3D.Twist(-ChartMath.ToRadians * Turn);

			if (m_3dSettings.Perspective)
			{
				if (m_3dSettings.AutoPerspective)
				{
					m_transform3D.SetPerspective(this.Width);
				}
				else
				{
					m_transform3D.SetPerspective(m_3dSettings.ZDistant);
				}
			}

			if (m_autoScale)
			{
				Matrix3D transform = m_transform3D.Result;

				Vector3D flt = transform * new Vector3D(Left, Top, 0);
				Vector3D flb = transform * new Vector3D(Left, Bottom, 0);
				Vector3D frt = transform * new Vector3D(Right, Top, 0);
				Vector3D frb = transform * new Vector3D(Right, Bottom, 0);

				Vector3D blt = transform * new Vector3D(Left, Top, m_depth);
				Vector3D blb = transform * new Vector3D(Left, Bottom, m_depth);
				Vector3D brt = transform * new Vector3D(Right, Top, m_depth);
				Vector3D brb = transform * new Vector3D(Right, Bottom, m_depth);

				double[] xVals = new double[] { flt.X, flb.X, frt.X, frb.X, blt.X, blb.X, brt.X, brb.X };
				double[] yVals = new double[] { flt.Y, flb.Y, frt.Y, frb.Y, blt.Y, blb.Y, brt.Y, brb.Y };

				RectangleF rc = RectangleF.FromLTRB((float)ChartMath.Min(xVals), (float)ChartMath.Min(yVals),
					(float)ChartMath.Max(xVals), (float)ChartMath.Max(yVals));

				double cr = rc.Right > Right ? 2 * (Right - (rc.Left + rc.Width / 2)) / rc.Width : 1f;
				double cl = rc.Left < Left ? 2 * ((rc.Left + rc.Width / 2) - Left) / rc.Width : 1f;
				double cb = rc.Bottom > Bottom ? 2 * (Bottom - (rc.Top + rc.Height / 2)) / rc.Height : 1f;
				double ct = rc.Top < Top ? 2 * ((rc.Top + rc.Height / 2) - Top) / rc.Height : 1f;
				double scale = ChartMath.Min(new double[] { Math.Abs(cr), Math.Abs(cl), Math.Abs(cb), Math.Abs(ct) });

				m_transform3D.View = Matrix3D.Scale(scale, scale, scale) * m_transform3D.View;
			}

			return m_transform3D;
		}
		/// <summary>
		/// Creates the work plane.
		/// </summary>
		/// <param name="z">The Z coordinate.</param>
		/// <returns></returns>
		private Path3DCollect CreateWorkPlane(float z)
		{
			Polygon fpl = new Polygon(new Vector3D[]{
                                                 new Vector3D( RenderBounds.Left, RenderBounds.Top, z ),
                                                 new Vector3D( RenderBounds.Right, RenderBounds.Top, z ),
                                                 new Vector3D( RenderBounds.Right, RenderBounds.Bottom, z ),
                                                 new Vector3D( RenderBounds.Left, RenderBounds.Bottom, z )
                                               });
			return new Path3DCollect(new Polygon[] { fpl });
		}
		/// <summary>
		/// Draws the custom points.
		/// </summary>
		/// <param name="g">The <see cref="Graphics3D"/>.</param>
		private void DrawCustomPoints(Graphics3D g)
		{
			if (CustomPoints.Count > 0)
			{
				foreach (ChartCustomPoint ccp in this.CustomPoints)
				{
					PointF pt = this.GetCustomPointLocation(ccp);
					ccp.Draw(this, g, new Vector3D(pt.X, pt.Y, 0));
				}
			}
		}
		/// <summary>
		/// Draws the strip lines.
		/// </summary>
		/// <param name="g">The <see cref="Graphics3D"/>.</param>
        /// <param name="zOrder"></param>
		private void DrawStripLines(Graphics3D g,ChartStripLineZorder zOrder)
		{
			foreach (ChartAxis axis in this.Axes)
			{
				foreach (ChartStripLine stripLine in axis.StripLines)
				{
					if (stripLine.Enabled)
					{
                        if (stripLine.ZOrder == zOrder)
                        {
                            Polygon plg = stripLine.Draw(g, this.GetStripLineRects(axis, stripLine), m_depth);
                            if (plg != null)
                            {
                                g.AddPolygon(plg);
                            }
                        }
					}
				}
			}
		}
		#endregion

		#region ShouldSerialize and Reset properties methods
		/// <summary>
		/// Indicates whether the background interior should be  serialized.
		/// </summary>
		/// <returns></returns>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private bool ShouldSerializeBackInterior()
		{
			return m_backInterior != null;
		}
		/// <summary>
		/// Indicates whether the axis spacing should be serialized
		/// </summary>
		/// <returns></returns>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private bool ShouldSerializeAxisSpacing()
		{
			return (m_axisSpacing != new SizeF(2, 2));
		}
		#endregion

		#region Compute the coordinates
		/// <summary>
        /// Gets the point by value internal.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="cpt">The <see cref="ChartPoint"/>.</param>
		/// <returns></returns>
		private Point GetPointByValueInternal(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
		{
			Point res = Point.Empty;

			if (m_requireAxes)
			{
				if (m_requireInvertedAxes)
				{
					res = GetPointByValueInversed(xAxis, yAxis, cpt);
				}
				else
				{
					res = GetPointByValueNormal(xAxis, yAxis, cpt);
				}
			}
			else if (m_chart.Radar)
			{
				res = GetPointByValuePolar(xAxis, yAxis, cpt);
			}

			return res;
		}


        /// <summary>
        /// Gets the point by value internal. This method is used only when multiple axes are used in the chart
        /// </summary>
        /// <param name="xAxis">The x axis.</param>
        /// <param name="yAxis">The y axis.</param>
        /// <param name="cpt">The <see cref="ChartPoint"/>.</param>
        /// <returns></returns>
        private Point GetPointByValueInternalMulAxes(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
        {
            Point res = Point.Empty;

            if (m_requireAxes)
            {                
               res = GetPointByValueNormalMulAxes(xAxis, yAxis, cpt);               
            }
            else if (m_chart.Radar)
            {
                res = GetPointByValuePolar(xAxis, yAxis, cpt);
            }
            return res;
        }

		/// <summary>
		/// Gets the point by value normal.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="cpt">The <see cref="ChartPoint"/>.</param>
		/// <returns></returns>
		private Point GetPointByValueNormal(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
        {
            return new Point((int)xAxis.GetVisibleValue(cpt.X) + RenderBounds.Left,
                RenderBounds.Bottom - (int)yAxis.GetVisibleValue(cpt.YValues[0]));           
		}

        /// <summary>
        /// Gets the point by value normal. This method is used only when multiple axes are used in the chart.
        /// </summary>
        /// <param name="xAxis">The x axis.</param>
        /// <param name="yAxis">The y axis.</param>
        /// <param name="cpt">The <see cref="ChartPoint"/>.</param>
        /// <returns></returns>
        private Point GetPointByValueNormalMulAxes(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
        {           
            return new Point((int)xAxis.GetCoordinateFromValue(cpt.X), (int)yAxis.GetCoordinateFromValue(cpt.YValues[0]));
        }

		/// <summary>
		/// Gets the point by value inversed.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="cpt">The <see cref="ChartPoint"/>.</param>
		/// <returns></returns>
		private Point GetPointByValueInversed(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
		{
			return new Point((int)xAxis.GetVisibleValue(cpt.YValues[0]) + RenderBounds.Left,
				RenderBounds.Bottom - (int)yAxis.GetVisibleValue(cpt.X));
		}       

		/// <summary>
		/// Gets the value by point inversed.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="pt">The point on chart.</param>
		/// <returns></returns>
		private ChartPoint GetValueByPointInversed(ChartAxis xAxis, ChartAxis yAxis, Point pt)
		{
			return new ChartPoint(yAxis.GetRealValue(pt), xAxis.GetRealValue(pt));
		}
		/// <summary>
		/// Gets the value by point normal.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="pt">The The point on chart.</param>
		/// <returns></returns>
		private ChartPoint GetValueByPointNormal(ChartAxis xAxis, ChartAxis yAxis, Point pt)
		{
			return new ChartPoint(xAxis.GetRealValue(pt), yAxis.GetRealValue(pt));
		}
		/// <summary>
		/// Gets the value by point polar.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="pt">The The point on chart.</param>
		/// <returns></returns>
		private ChartPoint GetValueByPointPolar(ChartAxis xAxis, ChartAxis yAxis, Point pt)
		{
			MinMaxInfo axisXRange = xAxis.VisibleRange;
			MinMaxInfo axisYRange = yAxis.VisibleRange;
			PointF center = Center;

			pt = new Point((int)(pt.X - center.X), (int)(pt.Y - center.Y));
			double x = 1.5 * Math.PI - Math.Atan2(pt.Y, pt.X);
			double y = Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
			y = axisYRange.Delta * y / Radius;

			if (x > 2 * Math.PI)
			{
				x -= 2 * Math.PI;
			}

			return new ChartPoint(x, y);
		}
		/// <summary>
		/// Gets the point by value polar.
		/// </summary>
		/// <param name="xAxis">The x axis.</param>
		/// <param name="yAxis">The y axis.</param>
		/// <param name="cpt">The <see cref="ChartPoint"/>.</param>
		/// <returns></returns>
		private Point GetPointByValuePolar(ChartAxis xAxis, ChartAxis yAxis, ChartPoint cpt)
		{
		        MinMaxInfo axisXRange = xAxis.VisibleRange;
            		MinMaxInfo axisYRange = yAxis.VisibleRange;
            		PointF center = Center;
            		if(m_chart.Radar && m_chart.Polar)
          		{
                		double y = Radius * cpt.YValues[0] / axisYRange.Delta;
                		double x = 1.5 * Math.PI - cpt.X;

                		return new Point((int)(center.X + y * Math.Cos(x)), (int)(center.Y + y * Math.Sin(x)));
	
            		}
           	        else
            		{ 
               		   //Fixed issue for Radar chart custom point 
               		float radius = yAxis.GetVisibleValue(cpt.YValues[0]);	
		            if (xAxis.Inversed)
                	{
                    		double angle = (ChartMath.DblPI * (axisXRange.max -cpt.X ) / axisXRange.Delta) + ChartMath.HlfPI;
                                       
                    		double x = (float)(center.X + radius * Math.Cos(angle));
                    		double y = center.Y - (float)(radius * Math.Sin(angle));               
                                       
                    		return new Point((int)x, (int)y);
                	}
                	else
                	{
                    
                    		double angle = (ChartMath.DblPI * (cpt.X - axisXRange.min) / axisXRange.Delta) + ChartMath.HlfPI;
                                        
                    		double x = (float)(center.X + radius * Math.Cos(angle));
                    		double y = center.Y - (float)(radius * Math.Sin(angle));                
                     
                    		return new Point((int)x, (int)y);
                	}
             		}
		}
        
		#endregion

		/// <summary>
		/// Arranges the axes.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void CalculateAxesSizes()
		{
			if (m_chart.Radar)
			{
				PointF center = Center;
				bool drawlabels = PrimaryXAxis.TickLabelsDrawingMode != ChartAxisTickLabelDrawingMode.None;
				float radius = GetRadarRadius(drawlabels ? PrimaryXAxis.Font.Height : 0);
				PrimaryYAxis.CalculateAxis(new RectangleF(center.X, center.Y - radius, 0, radius), SizeF.Empty, 0);
			}
			else
			{
				RectangleF rc = GetGlobalBoundsByRect(m_clientRectangle);

				if (!m_realSeries3D)
					rc = GetBoundsByRect(Rectangle.Round(rc));

				List<ChartAxis> hAxes = new List<ChartAxis>();
				List<ChartAxis> vAxes = new List<ChartAxis>();

				foreach (ChartAxis axis in this.Axes)
				{
					if (axis.Orientation == ChartOrientation.Horizontal)
				    {
						hAxes.Add(axis);
					}
					else
					{
						vAxes.Add(axis);
					}
				}

				if (m_xLayouts.Count > 0)
						{
					m_xLayouts.Arrange(rc, ChartOrientation.Horizontal);
					}
					else
					{
					#region Default layout
					int hAxesNotAutoCount = 0;
					float hSumNotAuto = 0;
					RectangleF rcfHorizontal = rc;

					foreach (ChartAxis axis in hAxes)
						{
						if (!axis.AutoSize)
						{
							hSumNotAuto += axis.RealLength;
							hAxesNotAutoCount++;
					}
				}

				float sbsWidth = (rc.Width - hSumNotAuto) / (hAxes.Count - hAxesNotAutoCount);
				float sbsCurrX = rcfHorizontal.Left;

				for (int i = 0; i < hAxes.Count; i++)
				{
					ChartAxis ax = (ChartAxis)hAxes[i];

					if (m_xAxesLayoutMode == ChartAxesLayoutMode.SideBySide)
					{
						rcfHorizontal = new RectangleF(sbsCurrX, rc.Y, (ax.AutoSize ? sbsWidth : ax.RealLength), rc.Height);
						sbsCurrX += rcfHorizontal.Width;
					}

						rcfHorizontal = ax.CalculateAxis(Rectangle.Round(rcfHorizontal), 
                            m_axisSpacing, ax.Dimension);
				}
					#endregion
				}

				if (m_yLayouts.Count > 0)
				{
					m_yLayouts.Arrange(rc, ChartOrientation.Vertical);
				}
				else
				{
					#region Default layout
					RectangleF rcfVertical = rc;
					int vAxesNotAutoCount = 0;
					float vSumNotAuto = 0;

					foreach (ChartAxis axis in vAxes)
					{
						if (!axis.AutoSize)
						{
							vSumNotAuto += axis.RealLength;
							vAxesNotAutoCount++;
						}
					}

					float sbsHeight = (rc.Height - vSumNotAuto) / (vAxes.Count - vAxesNotAutoCount);
					float sbsCurrY = rcfVertical.Bottom;

				//        for (int i = vAxes.Count - 1; i >= 0; i--)
				for (int i = 0; i < vAxes.Count; i++)
				{
					ChartAxis ax = (ChartAxis)vAxes[i];

					if (m_yAxesLayoutMode == ChartAxesLayoutMode.SideBySide)
					{
						float axisHeight = (ax.AutoSize ? sbsHeight : ax.RealLength);
						rcfVertical = new RectangleF(rc.X, sbsCurrY - axisHeight, rc.Width, axisHeight);
						sbsCurrY -= rcfVertical.Height;
					}

						rcfVertical = ax.CalculateAxis(Rectangle.Round(rcfVertical), 
                            m_axisSpacing, ax.Dimension);
				}
					#endregion
				}
			}
		}
		/// <summary>
		/// Calculates the label sizes.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="bounds">The RectangleF</param>
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		private void CalculateLabelSizes(Graphics g, RectangleF bounds)
		{
			SizeF nLabelsSize = SizeF.Empty;
			SizeF oLabelsSize = SizeF.Empty;
			SizeF axesScrollBarsSize = SizeF.Empty;
			RectangleF renderBounds = bounds;

			List<ChartAxis> hAxes = new List<ChartAxis>();
			List<ChartAxis> vAxes = new List<ChartAxis>();

			foreach (ChartAxis axis in this.Axes)
			{
				if (axis.Orientation == ChartOrientation.Horizontal)
				{
					hAxes.Add(axis);
				}
				else
				{
					vAxes.Add(axis);
				}
			}

			renderBounds.Width -= this.OffsetX;
			renderBounds.Height -= this.OffsetY;

			if (m_yLayouts.Count > 0)
			{
				float left, right, scrolls;

				m_yLayouts.Measure(g, renderBounds, ChartOrientation.Vertical, out left, out right, out scrolls);

				nLabelsSize.Width = left;
				oLabelsSize.Width = right;
				axesScrollBarsSize.Width = scrolls;
			}
			else
			{
			foreach (ChartAxis axis in this.Axes)
			{
				if (axis.Orientation == ChartOrientation.Vertical)
				{
					float dim = axis.GetDimension(g, this, renderBounds);

					if (axis.IsVisible && axis.LocationType != ChartAxisLocationType.Set)
					{
						#region Vertical axes
						switch (m_yAxesLayoutMode)
						{
							case ChartAxesLayoutMode.Stacking:
								{
								    if (axis.AxisLabelPlacement == ChartPlacement.Inside)
                                    {
                                        float titleSpace = axis.GetTitleDimention(g, this, renderBounds);
                                        if (axis.OpposedPosition)
                                        {
                                            oLabelsSize.Width += m_axisSpacing.Width + titleSpace;
                                        }
                                        else
                                        {
                                            nLabelsSize.Width += m_axisSpacing.Width + titleSpace;
                                        }
                                    }
									else if (axis.OpposedPosition)
									{
										oLabelsSize.Width += dim + m_axisSpacing.Width;
									}
									else
									{
										nLabelsSize.Width += dim + m_axisSpacing.Width;
									}

									if (axis.ScrollBar != null && axis.ScrollBar.Visible)
									{
										axesScrollBarsSize.Width += axis.ScrollBar.Dimension;
									}
								}
								break;

							case ChartAxesLayoutMode.SideBySide:
								{
								    if (axis.AxisLabelPlacement == ChartPlacement.Inside)
                                    {
                                        float titleSpace = axis.GetTitleDimention(g, this, renderBounds);
                                        if (axis.OpposedPosition)
                                        {
                                            oLabelsSize.Width = Math.Max(titleSpace + m_axisSpacing.Width, oLabelsSize.Width);
                                        }
                                        else
                                        {
                                            nLabelsSize.Width = Math.Max(titleSpace + m_axisSpacing.Width, nLabelsSize.Width);
                                        }
                                    }
									else if (axis.OpposedPosition)
									{
										oLabelsSize.Width = Math.Max(dim + m_axisSpacing.Width, oLabelsSize.Width);
									}
									else
									{
										nLabelsSize.Width = Math.Max(dim + m_axisSpacing.Width, nLabelsSize.Width);
									}

									if (axis.ScrollBar != null && axis.ScrollBar.Visible)
									{
										axesScrollBarsSize.Width = Math.Max(axis.ScrollBar.Dimension, axesScrollBarsSize.Width);
									}
								}
								break;
						}
						#endregion
					}
				}
			}
			}

			m_axesThickness.Left = nLabelsSize.Width;
			m_axesThickness.Right = oLabelsSize.Width + axesScrollBarsSize.Width;

			renderBounds.X += m_axesThickness.Left;
			renderBounds.Width -= m_axesThickness.Left + m_axesThickness.Right;

			if (m_xLayouts.Count > 0)
			{
				float left, right, scrolls;

                m_xLayouts.Measure(g, renderBounds, ChartOrientation.Horizontal, out left, out right, out scrolls);

				nLabelsSize.Height = left;
                oLabelsSize.Height = right;
				axesScrollBarsSize.Height = scrolls;
			}
			else
			{ 
			foreach (ChartAxis axis in this.Axes)
			{
				if (axis.Orientation == ChartOrientation.Horizontal)
				{
					float dim = axis.GetDimension(g, this, renderBounds);

					if (axis.IsVisible && axis.LocationType != ChartAxisLocationType.Set)
					{
						#region Horizontal axes
						switch (m_xAxesLayoutMode)
						{
							case ChartAxesLayoutMode.Stacking:
								{
								    if (axis.AxisLabelPlacement == ChartPlacement.Inside)
                                    {
                                        float titleSpace = axis.GetTitleDimention(g, this, renderBounds);
                                        if (axis.OpposedPosition)
                                            oLabelsSize.Height += m_axisSpacing.Height + titleSpace;
                                        else
                                            nLabelsSize.Height += m_axisSpacing.Height + titleSpace;
                                    }
									else if (axis.OpposedPosition)
									{
										oLabelsSize.Height += dim + m_axisSpacing.Height;
									}
									else
									{
										nLabelsSize.Height += dim + m_axisSpacing.Height;
									}

									if (axis.ScrollBar != null && axis.ScrollBar.Visible)
									{
										axesScrollBarsSize.Height += axis.ScrollBar.Dimension;
									}
								}
								break;

							case ChartAxesLayoutMode.SideBySide:
								{
								    if (axis.AxisLabelPlacement == ChartPlacement.Inside)
                                    {
                                        float titleSpace = axis.GetTitleDimention(g, this, renderBounds);
                                        if (axis.OpposedPosition)
                                            oLabelsSize.Height = Math.Max(titleSpace + m_axisSpacing.Height, oLabelsSize.Height);
                                        else
                                            nLabelsSize.Height = Math.Max(titleSpace + m_axisSpacing.Height, nLabelsSize.Height);
                                    }
									else if (axis.OpposedPosition)
									{
										oLabelsSize.Height = Math.Max(dim + m_axisSpacing.Height, oLabelsSize.Height);
									}
									else
									{
										nLabelsSize.Height = Math.Max(dim + m_axisSpacing.Height, nLabelsSize.Height);
									}

									if (axis.ScrollBar != null && axis.ScrollBar.Visible)
									{
										axesScrollBarsSize.Height = Math.Max(axis.ScrollBar.Dimension, axesScrollBarsSize.Height);
									}
								}
								break;
						}
						#endregion
					}
				}
			}
			}

            m_axesThickness.Top = oLabelsSize.Height;
            m_axesThickness.Bottom = nLabelsSize.Height + axesScrollBarsSize.Height;
		}

		/// <summary>
		/// Called when axes is changed.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="args">The args.</param>
		private void OnAxesChanged(ChartBaseList list, ChartListChangeArgs args)
		{
			if (args.NewItems != null)
			{
				foreach (ChartAxis axis in args.NewItems)
				{
					axis.SetOwner(this);

					axis.DimensionsChanged += new EventHandler(OnChangingRedraw);
					axis.AppearanceChanged += new EventHandler(ApearanceChanged);
					axis.AppearanceChanged += new EventHandler(OnChangingRedraw);
					axis.VisibleRangeChanged += new EventHandler(OnChangingRedraw);
					axis.IntervalsChanged += new EventHandler(ApearanceChanged);
					axis.FormatLabel += new ChartFormatAxisLabelEventHandler(OnChartAreaFormatLabel);
				}
			}

			if (args.OldItems != null)
			{
				foreach (ChartAxis axis in args.OldItems)
				{
					axis.SetOwner(null);

					axis.DimensionsChanged -= new EventHandler(OnChangingRedraw);
					axis.AppearanceChanged -= new EventHandler(ApearanceChanged);
					axis.AppearanceChanged -= new EventHandler(OnChangingRedraw);
					axis.VisibleRangeChanged -= new EventHandler(OnChangingRedraw);
					axis.IntervalsChanged -= new EventHandler(ApearanceChanged);
					axis.FormatLabel -= new ChartFormatAxisLabelEventHandler(OnChartAreaFormatLabel);
				}
			}

			m_chart.Redraw(true);
		}
		/// <summary>
		/// Called when custom points list is changed.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="args">The args.</param>
		private void OnCustomPointsListChanged(ChartBaseList list, ChartListChangeArgs args)
		{
			if (args.NewItems != null)
			{
				foreach (ChartCustomPoint ccp in args.NewItems)
				{
					ccp.SettingsChanged += new EventHandler(this.OnChangingRedraw);
				}
			}

			if (args.OldItems != null)
			{
				foreach (ChartCustomPoint ccp in args.OldItems)
				{
					ccp.SettingsChanged -= new EventHandler(this.OnChangingRedraw);
				}
			}

			this.Redraw(true);
		}
		/// <summary>
		/// Called when interactive cursors collection is changed.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="args">The args.</param>
		private void OnInteractiveCursorsChanged(ChartBaseList list, ChartListChangeArgs args)
		{
			if (args.OldItems != null)
			{
				foreach (ChartInteractiveCursor cursor in args.OldItems)
				{
					cursor.Changed -= new EventHandler(this.OnChangingRedraw);
				}
			}

			if (args.NewItems != null)
			{
				foreach (ChartInteractiveCursor cursor in args.NewItems)
				{
					cursor.Changed += new EventHandler(this.OnChangingRedraw);
				}
			}

			this.Redraw(true);
		}
		/// <summary>
		/// Called when it's need to apply label format.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartFormatAxisLabelEventArgs"/> instance containing the event data.</param>
		private void OnChartAreaFormatLabel(object sender, ChartFormatAxisLabelEventArgs e)
		{
			m_chart.OnChartFormatAxisLabel(sender as ChartAxis, e);
		}
		/// <summary>
		/// Called when the axes apearances was changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ApearanceChanged(object sender, EventArgs e)
		{
			m_chart.SeriesChanged(this, new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Changed));
		}
		/// <summary>
		/// Redraws by the specified update.
		/// </summary>
		/// <param name="update">if set to <c>true</c> chart will be updated.</param>
		internal void Redraw(bool update)
		{
			m_chart.Redraw(true);
		}
		/// <summary>
		/// Gets the bounds by rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		private Rectangle GetBoundsByRect(Rectangle rect)
		{
			int dw = (int)this.OffsetX;

			if (Rotation > 0)
			{
				rect.Width -= dw;
			}
			else
			{
				rect.Width -= dw;
				rect.X += dw;
			}

			int dh = (int)this.OffsetY;

			if (Tilt > 0)
			{
				rect.Height -= dh;
				rect.Y += dh;
			}
			else
			{
				rect.Height -= dh;
			}

			return rect;
		}
		/// <summary>
		/// Gets the global bounds by rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		private Rectangle GetGlobalBoundsByRect(Rectangle rect)
		{
			if (this.AxesType == ChartAreaAxesType.Rectangular)
			{
				rect = m_axesThickness.Deflate(rect);
			}
			rect.X += (int)(ChartAreaMargins.Left);
			rect.Width -= (int)(ChartAreaMargins.Left + ChartAreaMargins.Right);
			rect.Y += (int)(ChartAreaMargins.Top);
			rect.Height -= (int)(ChartAreaMargins.Top + ChartAreaMargins.Bottom);

			return rect;
		}

		/// <summary>
		/// Transforms the specified point to the chart plane.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		/// <remarks>
		///  Only for real 3D mode.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		public Point CorrectionFrom(Point pt)
		{
			if (RealSeries3D && Series3D)
			{
				pt = Point.Round(m_transform3D.ToScreen(new Vector3D(pt.X, pt.Y, 0)));
			}

			return pt;
		}
		/// <summary>
		/// Transforms the specified point to the screen plane.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		/// <remarks>
		///  Only for real 3D mode.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		public PointF CorrectionFrom(PointF pt)
		{
			if (RealSeries3D && Series3D)
			{
				pt = m_transform3D.ToScreen(new Vector3D(pt.X, pt.Y, 0));
			}

			return pt;
		}
        /// <summary>
        /// Change the default appearance of chart.
        /// </summary>
        /// <remarks></remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
	
        public void DoAppearanceChange()
        {

            if (this.LegacyAppearance)
            {
                  this.PrimaryXAxis.GridLineType.ForeColor = (this.PrimaryXAxis.GridLineType.ForeColor == Color.Black) ? Color.LightGray : this.PrimaryXAxis.GridLineType.ForeColor;
                this.PrimaryYAxis.GridLineType.ForeColor = (this.PrimaryYAxis.GridLineType.ForeColor == Color.Black) ? Color.LightGray : this.PrimaryYAxis.GridLineType.ForeColor;
                this.PrimaryXAxis.LineType.ForeColor = (this.PrimaryXAxis.LineType.ForeColor == Color.Black) ? Color.LightGray : this.PrimaryXAxis.LineType.ForeColor;
                this.PrimaryYAxis.LineType.ForeColor = (this.PrimaryYAxis.LineType.ForeColor == Color.Black) ? Color.LightGray : this.PrimaryYAxis.LineType.ForeColor;
                this.PrimaryXAxis.TickColor = (this.PrimaryXAxis.TickColor == SystemColors.ControlText) ? Color.LightGray : this.PrimaryXAxis.TickColor;
                this.PrimaryYAxis.TickColor = (this.PrimaryYAxis.TickColor == SystemColors.ControlText) ? Color.LightGray : this.PrimaryYAxis.TickColor;
                this.PrimaryXAxis.Font =(this.PrimaryXAxis.Font.Name =="Verdana" && this.PrimaryXAxis.Font.Size ==  8)?new Font("Segoe UI", 9.0f, FontStyle.Regular):this.PrimaryXAxis.Font;
                this.PrimaryXAxis.TitleFont = (this.PrimaryXAxis.TitleFont.Name == "Verdana" && this.PrimaryXAxis.TitleFont.Size == 8) ? new Font("Segoe UI", 12.0f, FontStyle.Regular) : this.PrimaryXAxis.TitleFont;
                this.PrimaryYAxis.TitleFont = (this.PrimaryYAxis.TitleFont.Name == "Verdana" && this.PrimaryYAxis.TitleFont.Size == 8) ? new Font("Segoe UI", 12.0f, FontStyle.Regular) : this.PrimaryYAxis.TitleFont;
                this.PrimaryYAxis.Font = (this.PrimaryYAxis.Font.Name == "Verdana" && this.PrimaryYAxis.Font.Size == 8) ? new Font("Segoe UI", 9.0f, FontStyle.Regular) : this.PrimaryYAxis.Font;
                this.PrimaryXAxis.ForeColor =(this.PrimaryXAxis.ForeColor== Color.Black)?Color.Black:this.PrimaryXAxis.ForeColor;
                this.PrimaryYAxis.ForeColor = (this.PrimaryYAxis.ForeColor == Color.Black) ? Color.Black : this.PrimaryYAxis.ForeColor;
                this.Chart.ChartInterior =(this.Chart.ChartInterior ==new BrushInfo(Color.White))? new BrushInfo(Color.White) : this.Chart.ChartInterior;
                this.Chart.BackInterior = (this.Chart.BackInterior == new BrushInfo(Color.White)) ? new BrushInfo(Color.White) : this.Chart.BackInterior;
            }
        }
		/// <summary>
		/// Transforms the specified point to the screen plane.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		/// <remarks>
		///  Only for real 3D mode.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		public Point CorrectionTo(Point pt)
		{
			if (RealSeries3D && Series3D)
			{
				Plane3D frontPlane = new Plane3D(new Vector3D(0, 0, 1), 0);
				frontPlane.Transform(m_transform3D.View);
				Vector3D v = m_transform3D.ToPlane(pt, frontPlane);
				pt = new Point((int)v.X, (int)v.Y);
			}

			return pt;
		}
		/// <summary>
		/// Transforms the specified point to the chart plane.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		/// <remarks>
		///  Only for real 3D mode.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		public PointF CorrectionTo(PointF pt)
		{
			if (RealSeries3D && Series3D)
			{
				Plane3D frontPlane = new Plane3D(new Vector3D(0, 0, 1), 0);
				frontPlane.Transform(m_transform3D.View);
				Vector3D v = m_transform3D.ToPlane(pt, frontPlane);
				pt = new Point((int)v.X, (int)v.Y);
			}

			return pt;
		}

		/// <summary>
		/// Calculates  given flag state.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="flag">The flag.</param>
		/// <returns>
		/// 	<c>true</c> if the specified flag is present in the specified value; otherwise, <c>false</c>.
		/// </returns>
		private bool IsPaintFlag(ChartPaintFlags target, ChartPaintFlags flag)
		{
			return (target & flag) == flag;
		}
		/// <summary>
		/// Rounds the rotation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private float RoundRotation(float value)
		{
			if (this.DrawingMode != DrawingMode.Real3D)
			{
				if (value > c_max2DRotate)
				{
					value = c_max2DRotate - 1e-1F;
				}
				else if (value < 1e-1F)
				{
					value = 1e-1F;
				}
			}
			else
			{
				value = (float)(value % c_max3DRotate);
			}

			return value;
		}
		/// <summary>
		/// Gets the radar radius by the specified font size.
		/// </summary>
		/// <param name="fontHeight">Height of the font.</param>
		/// <returns></returns>
		private float GetRadarRadius(float fontHeight)
		{
			RectangleF renderBounds = this.RenderBounds;

			return 0.5f * Math.Min(renderBounds.Width, renderBounds.Height) - fontHeight * 2;
		}
		/// <summary>
		/// Called when is need to redraw the chart.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnChangingRedraw(object sender, EventArgs e)
		{
			this.Redraw(true);
		}
		#endregion

		#region Obsolete methods
		/// <summary>
		/// Gets the front bound by axes.
		/// </summary>
		/// <returns></returns>
		[Obsolete("This method isn't used anymore.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RectangleF GetFrontBoundByAxes()
		{
			return m_chart.Radar ? GetGlobalBoundsByRect(m_clientRectangle) : GetFrontBoundByAxes(false);
		}
		/// <summary>
		/// Gets the front bound by axes.
		/// </summary>
		/// <param name="byAllAxes">if set to <c>true</c> [by all axes].</param>
		/// <returns></returns>
		[Obsolete("This method isn't used anymore.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RectangleF GetFrontBoundByAxes(bool byAllAxes)
		{
			RectangleF res = RectangleF.Empty;

			if (byAllAxes)
			{
				float left = ClientRectangle.Right;
				float top = ClientRectangle.Bottom;
				float right = ClientRectangle.Left;
				float bottom = ClientRectangle.Top;

				for (int i = 0; i < m_axes.Count; i++)
				{
					left = Math.Min(m_axes[i].Rect.Left, left);
					top = Math.Min(m_axes[i].Rect.Top, top);
					right = Math.Min(m_axes[i].Rect.Right, right);
					bottom = Math.Min(m_axes[i].Rect.Bottom, bottom);
				}

				res = RectangleF.FromLTRB(left, top, right, bottom);
			}
			else
			{
				res = new RectangleF(PrimaryXAxis.Rect.X, PrimaryYAxis.Rect.Y,
					PrimaryXAxis.Rect.Width, PrimaryYAxis.Rect.Height);
			}

			return res;
		}
		/// <summary>
		/// Returns the rectangle encompassing the specified axes.
		/// </summary>
		[Obsolete("This method is incorrect."), EditorBrowsable(EditorBrowsableState.Never)]
		public static RectangleF GetAxesRect(ChartAxis ax1, ChartAxis ax2)
		{
			RectangleF horR = RectangleF.Empty, verR = RectangleF.Empty, res = RectangleF.Empty;

			if (ax1.Orientation == ChartOrientation.Horizontal)
				horR = ax1.Rect;
			if (ax2.Orientation == ChartOrientation.Horizontal)
				horR = ax2.Rect;

			if (ax1.Orientation == ChartOrientation.Vertical)
				verR = ax1.Rect;
			if (ax2.Orientation == ChartOrientation.Vertical)
				verR = ax2.Rect;

			if (horR == RectangleF.Empty || verR == RectangleF.Empty) return res;

			return new RectangleF(horR.X, verR.Y, horR.Width, verR.Height);
		}
		/// <summary>
		/// Returns the ChartRegion at the specified index value.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[Obsolete("Use Chart.ChartRegions collection.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ChartRegion GetChartRegion(int index)
		{
			return ChartRegions[index] as ChartRegion;
		}
		/// <summary>
		/// Calculates zoomfactor and zoomposition for x axis.
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public void CalculateXZoomFactorAndZoomPosition(PointF upPoint, PointF downPoint, double zoomFact, double zoomPos, double zoomPrec, out double newZoomFact, out double newZoomPos)
		{
			if (!(RealSeries3D && m_chart.Series3D))
			{
				Rectangle axesRect = this.RenderBounds;

				upPoint.X = Math.Max(axesRect.Left, upPoint.X);
				downPoint.X = Math.Max(axesRect.Left, downPoint.X);
				upPoint.Y = Math.Max(axesRect.Top, upPoint.Y);
				downPoint.Y = Math.Max(axesRect.Top, downPoint.Y);
				upPoint.X = Math.Min(axesRect.Right, upPoint.X);
				downPoint.X = Math.Min(axesRect.Right, downPoint.X);
				upPoint.Y = Math.Min(axesRect.Bottom, upPoint.Y);
				downPoint.Y = Math.Min(axesRect.Bottom, downPoint.Y);

				double zfx = Math.Floor(zoomPrec * axesRect.Width / Math.Abs((double)upPoint.X - (double)downPoint.X));

				newZoomFact = zoomFact / (zfx / zoomPrec);

				newZoomPos = zoomPos + (Math.Min(downPoint.X - axesRect.Left, upPoint.X - axesRect.Left) / (axesRect.Width / zoomFact));
			}
			else
			{
				PointF up = CorrectionTo(upPoint);
				PointF dp = CorrectionTo(downPoint);

				Rectangle axesRect = this.RenderBounds;

				up.X = Math.Max(axesRect.Left, up.X);
				dp.X = Math.Max(axesRect.Left, dp.X);
				up.Y = Math.Max(axesRect.Top, up.Y);
				dp.Y = Math.Max(axesRect.Top, dp.Y);
				up.X = Math.Min(axesRect.Right, up.X);
				dp.X = Math.Min(axesRect.Right, dp.X);
				up.Y = Math.Min(axesRect.Bottom, up.Y);
				dp.Y = Math.Min(axesRect.Bottom, dp.Y);

				double zfx = Math.Floor(zoomPrec * axesRect.Width / Math.Abs((double)up.X - (double)dp.X));

				newZoomFact = zoomFact / (zfx / zoomPrec);

				newZoomPos = zoomPos + (Math.Min(dp.X - axesRect.Left, up.X - axesRect.Left) / (axesRect.Width / zoomFact));
			}
		}
		/// <summary>
		/// Calculates zoomfactor and zoomposition for y axis.
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public void CalculateYZoomFactorAndZoomPosition(PointF upPoint, PointF downPoint, double zoomFact, double zoomPos, double zoomPrec, out double newZoomFact, out double newZoomPos)
		{
			if (!(RealSeries3D && m_chart.Series3D))
			{
				Rectangle axesRect = this.RenderBounds;

				upPoint.X = Math.Max(axesRect.Left, upPoint.X);
				downPoint.X = Math.Max(axesRect.Left, downPoint.X);
				upPoint.Y = Math.Max(axesRect.Top, upPoint.Y);
				downPoint.Y = Math.Max(axesRect.Top, downPoint.Y);
				upPoint.X = Math.Min(axesRect.Right, upPoint.X);
				downPoint.X = Math.Min(axesRect.Right, downPoint.X);
				upPoint.Y = Math.Min(axesRect.Bottom, upPoint.Y);
				downPoint.Y = Math.Min(axesRect.Bottom, downPoint.Y);

				double zfy = Math.Floor(zoomPrec * axesRect.Height / Math.Abs((double)upPoint.Y - (double)downPoint.Y));

				newZoomFact = zoomFact / (zfy / zoomPrec);

				newZoomPos = zoomPos + (Math.Min(upPoint.Y - axesRect.Top, downPoint.Y - axesRect.Top) / (axesRect.Height / zoomFact));
			}
			else
			{
				PointF up = CorrectionTo(upPoint);
				PointF dp = CorrectionTo(downPoint);
				Rectangle axesRect = this.RenderBounds;

				up.X = Math.Max(axesRect.Left, up.X);
				dp.X = Math.Max(axesRect.Left, dp.X);
				up.Y = Math.Max(axesRect.Top, up.Y);
				dp.Y = Math.Max(axesRect.Top, dp.Y);
				up.X = Math.Min(axesRect.Right, up.X);
				dp.X = Math.Min(axesRect.Right, dp.X);
				up.Y = Math.Min(axesRect.Bottom, up.Y);
				dp.Y = Math.Min(axesRect.Bottom, dp.Y);

				double zfy = Math.Floor(zoomPrec * axesRect.Height / Math.Abs((double)up.Y - (double)dp.Y));

				newZoomFact = zoomFact / (zfy / zoomPrec);

				newZoomPos = zoomPos + (Math.Min(up.Y - axesRect.Top, dp.Y - axesRect.Top) / (axesRect.Height / zoomFact));
			}
		}
		#endregion
	}
}