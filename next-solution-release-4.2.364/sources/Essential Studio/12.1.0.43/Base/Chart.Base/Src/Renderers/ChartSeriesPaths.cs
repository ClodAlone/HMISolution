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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Abstract class of series segment.
	/// </summary>
	public abstract class ChartSegment
	{
		#region Members
		/// <summary>
		/// The bounds of segment.
		/// </summary>
		protected RectangleF m_bounds;
		/// <summary>
		/// The drawing order of segment.
		/// </summary>
		protected int m_zOrder;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the bounds.
		/// </summary>
		/// <value>The bounds.</value>
		public RectangleF Bounds
		{
			get { return m_bounds; }
			set { m_bounds = value; }
		}
		/// <summary>
		/// Gets or sets the drawing order.
		/// </summary>
		/// <value>The drawing order.</value>
		public int ZOrder
		{
			get { return m_zOrder; }
			set { m_zOrder = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Draws the segment to specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/> instance.</param>
		public virtual void Draw(Graphics g)
		{
		}
		/// <summary>
		/// Gets the region of segment.
		/// </summary>
		/// <returns></returns>
		public virtual ChartRegion GetChartRegion()
		{
			return null;
		}
		#endregion
	}

	/// <summary>
	/// Represents the simple geometry element.
	/// </summary>
	public class ChartSeriesPath : ChartSegment
	{
		#region Internal types
		class ChartPrimitive
		{
			public GraphicsPath Path;
			public Pen Pen;
			public Brush Brush;
			public BrushInfo BrushInfo;
            public String BoxName;
		}
		#endregion

		#region Members
		private ArrayList m_primitives = new ArrayList();
		private ChartRegionData m_regionData;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the region data.
		/// </summary>
		/// <value>The region data.</value>
		public ChartRegionData RegionData
		{
			get
			{
				return m_regionData;
			}
			set
			{
				m_regionData = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSeriesPath"/> class.
		/// </summary>
		public ChartSeriesPath()
			: this(null, null, null, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSeriesPath"/> class.
		/// </summary>
		/// <param name="gp">The <see cref="GraphicsPath"/>.</param>
		/// <param name="br">The <see cref="BrushInfo"/>.</param>
		/// <param name="pen">The <see cref="Pen"/>.</param>
		public ChartSeriesPath(GraphicsPath gp, BrushInfo br, Pen pen)
			: this(gp, br, pen, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSeriesPath"/> class.
		/// </summary>
		/// <param name="gp">The <see cref="GraphicsPath"/>.</param>
		/// <param name="br">The <see cref="BrushInfo"/>.</param>
		/// <param name="pen">The <see cref="Pen"/>.</param>
		/// <param name="crd">The <see cref="ChartRegionData"/>.</param>
		public ChartSeriesPath(GraphicsPath gp, BrushInfo br, Pen pen, ChartRegionData crd)
		{
			this.AddPrimitive(gp, pen, br);
			m_regionData = crd;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Adds the graphical primitive.
		/// </summary>
		/// <param name="gp">The <see cref="GraphicsPath"/>.</param>
		/// <param name="pen">The <see cref="Pen"/>.</param>
		/// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
		public void AddPrimitive(GraphicsPath gp, Pen pen, BrushInfo brushInfo)
		{
			ChartPrimitive primitive = new ChartPrimitive();

			primitive.Path = gp;
			primitive.Pen = pen;
			primitive.Brush = null;
			primitive.BrushInfo = brushInfo;

			m_primitives.Add(primitive);
		}
        /// <summary>
        /// Adds the graphical primitive.
        /// </summary>
        /// <param name="gp">The <see cref="GraphicsPath"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
        /// <param name="boxName">The <see cref="String"/>.</param>
        public void AddPrimitive(GraphicsPath gp, Pen pen, BrushInfo brushInfo, String boxName)
        {
            ChartPrimitive primitive = new ChartPrimitive();

            primitive.Path = gp;
            primitive.Pen = pen;
            primitive.Brush = null;
            primitive.BrushInfo = brushInfo;
            primitive.BoxName = boxName;
            m_primitives.Add(primitive);
        }
		/// <summary>
		/// Draws the segment to specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/> instance.</param>
		public override void Draw(Graphics g)
		{
			foreach (ChartPrimitive primitive in m_primitives)
			{
				if (primitive.Path != null)
				{
					if (primitive.Brush != null)
					{
						g.FillPath(primitive.Brush, primitive.Path);
					}
                    else if (primitive.BrushInfo != null && !primitive.BrushInfo.IsEmpty)
					{
                        if (primitive.BoxName != null)
                        {
                            ColorBlend c_cylinderPhong;
                            Color[] colors;
                            float[] positions;
                            ColorBlend colorBlend = new ColorBlend();

                            if (primitive.BoxName == "BoxRight")
                                ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(0x90, primitive.BrushInfo.BackColor), Color.FromArgb(0x90, Color.Black), Color.FromArgb(200, Color.Black), Math.PI / 4, 30, out colors, out positions);
                            else
                                ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(200, primitive.BrushInfo.BackColor), Color.FromArgb(0x90, primitive.BrushInfo.BackColor), Color.FromArgb(100, Color.Black), Math.PI / 4, 30, out colors, out positions);

                            colorBlend.Positions = positions;
                            colorBlend.Colors = colors;
                            c_cylinderPhong = colorBlend;

                             using (LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0, 0, 1, 1),
                            Color.Black, Color.White, LinearGradientMode.Vertical))
                            {
                                lgb.InterpolationColors = c_cylinderPhong;
                                g.FillPath(lgb, primitive.Path);
                            }
                        }
                        else
                        {
                            BrushPaint.FillPath(g, primitive.Path, primitive.BrushInfo);
                        }
                        
					}

					if (primitive.Pen != null)
					{
						g.DrawPath(primitive.Pen, primitive.Path);
					}
				}
			}
		}
		/// <summary>
		/// Draws the segment to specified <see cref="ChartGraph"/>.
		/// </summary>
		/// <param name="cg">The <see cref="ChartGraph"/>.</param>
		public void Draw(ChartGraph cg)
		{
			foreach (ChartPrimitive primitive in m_primitives)
			{
				if (primitive.Path != null)
				{
					if (primitive.Brush != null)
					{
						cg.DrawPath(primitive.Brush, primitive.Pen, primitive.Path);
					}
					else
					{
						cg.DrawPath(primitive.BrushInfo, primitive.Pen, primitive.Path);
					}
				}
			}
		}
        /// <summary>
        /// Draws the segment to specified <see cref="ChartGraph"/>.
        /// </summary>
        /// <param name="cg">The <see cref="ChartGraph"/>.</param>
        /// <param name="g">The <see cref="String"/>.</param>
        public void Draw(ChartGraph cg, String g)
        {
            if (cg != null)
            {
                foreach (ChartPrimitive primitive in m_primitives)
                {
                    if (primitive.Path != null)
                    {
                        if (primitive.Brush != null)
                        {
                            cg.DrawPath(primitive.Brush, primitive.Pen, primitive.Path);
                        }
                        else if (primitive.BoxName  == "BoxCenter")
                        {
                            cg.DrawPath(primitive.BrushInfo, primitive.Pen, primitive.Path);

                        }
                        else
                        {
                            ColorBlend c_cylinderPhong;
                            Color[] colors;
                            float[] positions;
                            ColorBlend colorBlend = new ColorBlend();

                            if (primitive.BoxName  == "BoxRight")
                                ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(0x90, primitive.BrushInfo.BackColor), Color.FromArgb(0x90, Color.Black), Color.FromArgb(200, Color.Black), Math.PI / 4, 30, out colors, out positions);
                            else
                                ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(200, primitive.BrushInfo.BackColor), Color.FromArgb(0x90, primitive.BrushInfo.BackColor), Color.FromArgb(100, Color.Black), Math.PI / 4, 30, out colors, out positions);

                            colorBlend.Positions = positions;
                            colorBlend.Colors = colors;
                            c_cylinderPhong = colorBlend;

                            cg.DrawPath(primitive.BrushInfo, primitive.Pen, primitive.Path);
                            using (LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0, 0, 1, 1),
                            Color.Black, Color.White, LinearGradientMode.Vertical))
                            {
                                lgb.InterpolationColors = c_cylinderPhong;
                                cg.DrawPath(lgb, primitive.Pen, primitive.Path);
                            }
                        }
                    }
                }
            }
        }
		/// <summary>
		/// Gets the region of segment.
		/// </summary>
		/// <returns></returns>
		public override ChartRegion GetChartRegion()
		{
			if (m_primitives.Count > 0)
			{
				Region region = new Region(RectangleF.Empty);

				foreach (ChartPrimitive primitive in m_primitives)
				{
					if (primitive.Path != null)
					{
						region.Union(primitive.Path);
					}
				}

				return m_regionData == null ? null : m_regionData.GetChartRegion(region);
			}

			return null;
		}
		#endregion
	}
}
