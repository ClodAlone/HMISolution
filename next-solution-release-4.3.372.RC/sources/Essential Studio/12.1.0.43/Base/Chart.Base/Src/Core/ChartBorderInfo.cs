#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;
using System.IO;
using System.Xml;
using System.Drawing.Imaging;
using System.Resources;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Provides methods to drawing the border by images.
	/// </summary>
	class ChartImageBorder
	{
		#region Constants
		private const float c_byteToFloat = 1 / 255f;
		#endregion

		#region Members
		private Bitmap m_sourceImage = null;

		private RectangleF m_bounds = RectangleF.Empty;

		private Rectangle m_leftTopRect = Rectangle.Empty;
		private Rectangle m_leftBottomRect = Rectangle.Empty;
		private Rectangle m_rightTopRect = Rectangle.Empty;
		private Rectangle m_rightBottomRect = Rectangle.Empty;

		private Rectangle m_topRect = Rectangle.Empty;
		private Rectangle m_bottomRect = Rectangle.Empty;
		private Rectangle m_leftRect = Rectangle.Empty;
		private Rectangle m_rightRect = Rectangle.Empty;

		private RectangleF m_destLeftTopRect = RectangleF.Empty;

		private Region m_tlRegion = null;
		private Region m_trRegion = null;
		private Region m_blRegion = null;
		private Region m_brRegion = null;

		private ChartThickness m_thickness;
		private ChartThickness m_padding;
		private bool m_supportBaseColor = false;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public ChartThickness Padding
		{
			get
			{
				return m_padding;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartImageBorder"/> class.
		/// </summary>
		/// <param name="resources">The resources.</param>
		/// <param name="name">The name.</param>
		public ChartImageBorder(ResourceManager resources, string name)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(resources.GetString(name));

			XmlElement rootElement = xmlDoc.DocumentElement;

			XmlElement ltElement = rootElement["LeftTop"];
			XmlElement rtElement = rootElement["RightTop"];
			XmlElement lbElement = rootElement["LeftBottom"];
			XmlElement rbElement = rootElement["RightBottom"];

			XmlElement lElement = rootElement["Left"];
			XmlElement tElement = rootElement["Top"];
			XmlElement rElement = rootElement["Right"];
			XmlElement bElement = rootElement["Bottom"];

			m_sourceImage = resources.GetObject(rootElement.GetAttribute("image")) as Bitmap;
			m_thickness = ChartThickness.Parse(rootElement.GetAttribute("thickness"));
			m_padding = ChartThickness.Parse(rootElement.GetAttribute("padding"));

			m_leftTopRect = this.ParseRectangle(ltElement.GetAttribute("rect"));
			m_leftBottomRect = this.ParseRectangle(lbElement.GetAttribute("rect"));
			m_rightTopRect = this.ParseRectangle(rtElement.GetAttribute("rect"));
			m_rightBottomRect = this.ParseRectangle(rbElement.GetAttribute("rect"));

			m_topRect = this.ParseRectangle(tElement.GetAttribute("rect"));
			m_leftRect = this.ParseRectangle(lElement.GetAttribute("rect"));
			m_rightRect = this.ParseRectangle(rElement.GetAttribute("rect"));
			m_bottomRect = this.ParseRectangle(bElement.GetAttribute("rect"));

			Color maskColor = this.ParseColor(rootElement.GetAttribute("maskcolor"));

			m_tlRegion = this.GetRegion(m_sourceImage, m_leftTopRect, maskColor);
			m_blRegion = this.GetRegion(m_sourceImage, m_leftBottomRect, maskColor);
			m_trRegion = this.GetRegion(m_sourceImage, m_rightTopRect, maskColor);
			m_brRegion = this.GetRegion(m_sourceImage, m_rightBottomRect, maskColor);
		}
		#endregion

		#region Public methdos
		/// <summary>
		/// Draws the specified g.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="baseColor">Color of the base.</param>
		public void Draw(Graphics g, RectangleF rect, Color baseColor)
		{
			ImageAttributes attr = m_supportBaseColor ? this.GetRecolorAttributes(baseColor) : null;

			RectangleF ltRect = new RectangleF(rect.Left, rect.Top,
				m_thickness.Left, m_thickness.Top);
			RectangleF rtRect = new RectangleF(rect.Right - m_thickness.Right,
				rect.Y, m_thickness.Right, m_thickness.Top);
			RectangleF lbRect = new RectangleF(rect.Left, rect.Bottom - m_thickness.Bottom,
				m_thickness.Left, m_thickness.Bottom);
			RectangleF rbRect = new RectangleF(rect.Right - m_thickness.Right,
				rect.Bottom - m_thickness.Bottom, m_thickness.Right, m_thickness.Bottom);

			RectangleF lRect = new RectangleF(rect.Left, rect.Top + m_thickness.Top,
				m_thickness.Left, rect.Height - m_thickness.Top - m_thickness.Bottom);
			RectangleF tRect = new RectangleF(rect.Left + m_thickness.Left, rect.Top,
				rect.Width - m_thickness.Left - m_thickness.Right, m_thickness.Top);
			RectangleF rRect = new RectangleF(rect.Right - m_thickness.Right, rect.Top + m_thickness.Top,
				m_thickness.Left, rect.Height - m_thickness.Top - m_thickness.Bottom);
			RectangleF bRect = new RectangleF(rect.Left + m_thickness.Left, rect.Bottom - m_thickness.Bottom,
				rect.Width - m_thickness.Left - m_thickness.Right, m_thickness.Bottom);

			this.DrawImage(g, m_leftRect, lRect, attr);
			this.DrawImage(g, m_topRect, tRect, attr);
			this.DrawImage(g, m_rightRect, rRect, attr);
			this.DrawImage(g, m_bottomRect, bRect, attr);

			this.DrawImage(g, m_leftTopRect, ltRect, attr);
			this.DrawImage(g, m_leftBottomRect, lbRect, attr);
			this.DrawImage(g, m_rightTopRect, rtRect, attr);
			this.DrawImage(g, m_rightBottomRect, rbRect, attr);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Builds the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		public void Build(RectangleF rect)
		{
			if (m_bounds != rect)
			{
				m_bounds = rect;

				m_destLeftTopRect = new RectangleF(rect.Left, rect.Top,
					m_thickness.Left, m_thickness.Top);
			}
		}
		/// <summary>
		/// Gets the region.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		public Region GetRegion(RectangleF rect)
		{
			RectangleF trct = m_thickness.Deflate(rect);

			Region region = new Region(rect);
			Region ltRegion = m_tlRegion.Clone();
			Region rtRegion = m_trRegion.Clone();
			Region lbRegion = m_blRegion.Clone();
			Region rbRegion = m_brRegion.Clone();

			ltRegion.Translate(rect.Left, rect.Top);
			rtRegion.Translate(trct.Right, rect.Top);
			lbRegion.Translate(rect.Left, trct.Bottom);
			rbRegion.Translate(trct.Right, trct.Bottom);

			region.Exclude(ltRegion);
			region.Exclude(rtRegion);
			region.Exclude(lbRegion);
			region.Exclude(rbRegion);

			return region;
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="srcRect">The SRC rect.</param>
		/// <param name="destRect">The dest rect.</param>
		/// <param name="attr">The attr.</param>
		private void DrawImage(Graphics g, RectangleF srcRect, RectangleF destRect, ImageAttributes attr)
		{
			if (attr == null)
			{
				g.DrawImage(m_sourceImage, destRect, srcRect, GraphicsUnit.Pixel);
			}
			else
			{
				g.DrawImage(m_sourceImage, Rectangle.Truncate(destRect),
					srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, g.PageUnit, attr);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private ImageAttributes GetRecolorAttributes(Color color)
		{
			ImageAttributes result = new ImageAttributes();
			ColorMatrix colorMatrix = new ColorMatrix();

			colorMatrix.Matrix00 = c_byteToFloat * color.R;
			colorMatrix.Matrix11 = c_byteToFloat * color.G;
			colorMatrix.Matrix22 = c_byteToFloat * color.B;
			colorMatrix.Matrix33 = c_byteToFloat * color.A;

			result.SetWrapMode(WrapMode.Tile);
			result.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			return result;
		}
		/// <summary>
		/// Gets the region.
		/// </summary>
		/// <param name="bmp">The <see cref="Bitmap"/>.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="maskColor">Color of the mask.</param>
		/// <returns></returns>
		private Region GetRegion(Bitmap bmp, Rectangle rect, Color maskColor)
		{
			Region rgn = new Region(Rectangle.Empty);

			for (int i = 0; i < rect.Width; i++)
			{
				for (int j = 0; j < rect.Height; j++)
				{
					if (bmp.GetPixel(rect.X + i, rect.Y + j).ToArgb() == maskColor.ToArgb())
					{
						rgn.Union(new Rectangle(i, j, 1, 1));
					}
				}
			}

			return rgn;
		}
		/// <summary>
		/// Parses the rectangle.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		private Rectangle ParseRectangle(string text)
		{
			string[] svalues = text.Split( ';');

			return new Rectangle(int.Parse(svalues[0].Trim()), int.Parse(svalues[1].Trim()),
				int.Parse(svalues[2].Trim()), int.Parse(svalues[3].Trim()));
		}
		/// <summary>
		/// Parses the color.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		private Color ParseColor(string text)
		{
			string[] svalues = text.Split(';');

			return Color.FromArgb(int.Parse(svalues[0].Trim()), int.Parse(svalues[1].Trim()), int.Parse(svalues[2].Trim()));
		}
		#endregion
	}

	/// <summary>
	/// Specifies the border skin style.
	/// </summary>
	public enum ChartBorderSkinStyle
	{
		/// <summary>
		/// Simple (Flat) border
		/// </summary>
		None,
		/// <summary>
		/// An embossed style of border skin is used. 
		/// </summary>
		Emboss,
		/// <summary>
		/// A bevel style of border skin is used. 
		/// </summary>
		Bevel,
		/// <summary>
		/// An embed style of border skin is used. 
		/// </summary>
		Embed,
		/// <summary>
		/// A frame style of border skin is used. 
		/// </summary>
		Frame,
		/// <summary>
		/// A pinned style of border skin is used. 
		/// </summary>
		Pinned,
		/// <summary>
		/// An open style of border skin is used. 
		/// </summary>
		Open,
		/// <summary>
		/// A roundedDiagonal style of border skin is used. 
		/// </summary>
		RoundedDiagonal,
		/// <summary>
		/// A slice style of border skin is used. 
		/// </summary>
		Slice,
		/// <summary>
		/// An projector style of border skin is used. 
		/// </summary>
		Projector,
		/// <summary>
		/// A gel style of border skin is used. 
		/// </summary>
		/// <remarks>
		///  Gel skin doesn't support background interior and background image.
		/// </remarks>
		Gel,
		/// <summary>
		/// A raised sunken of border skin is used
		/// </summary>
		Sunken,
		/// <summary>
		/// An etched style of border skin is used. 
		/// </summary>
		Etched,
		/// <summary>
		/// A raised style of border skin is used.
		/// </summary>
		Raised,
	}

	/// <summary>
	/// Specifies the position of the image. 
	/// </summary>
	public enum ChartImageLayout
	{
		/// <summary>
		/// The image is left-aligned at the top across the control's client rectangle. 
		/// </summary>
		None,
		/// <summary>
		/// The image is tiled across the control's client rectangle. 
		/// </summary>
		Tile,
		/// <summary>
		/// The image is centered within the control's client rectangle. 
		/// </summary>
		Center,
		/// <summary>
		/// The image is stretched across the control's client rectangle. 
		/// </summary>
		Stretch,
		/// <summary>
		/// The image is enlarged within the control's client rectangle. 
		/// </summary>
		Zoom
	}

	/// <summary>
	/// Provides the methods to draws the chart border.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("Gets or sets the border appearance.")]
	public class ChartBorderInfo
	{
		#region Internal types
		/// <summary>
		/// Specifies the border line appearance.
		/// </summary>
		public class ChartBorderLineInfo
		{
			#region Members
			private Color m_color = Color.DarkGray;
			private float m_width = 2;
			private DashStyle m_dashStyle = DashStyle.Solid;
			private ChartBorderInfo m_owner;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets the foreground color.
			/// </summary>
			/// <value>The foreground color.</value>
			[DefaultValue(typeof(Color), "DarkGray"), NotifyParentProperty(true)]
            [Description("Gets or sets the color of the frame (Sunken, Etched, Raised).")]
            [ChartTemplate(ChartTemplateSet.Simple)]
			public Color ForeColor
			{
				get
                {
                    return m_color; 
                }

				set
				{
					if (m_color != value)
					{
						m_color = value;
						m_owner.RaiseChanged();
					}
				}
			}
			/// <summary>
			/// Gets or sets the width.
			/// </summary>
			/// <value>The width.</value>
			[DefaultValue(2f), NotifyParentProperty(true)]
            [Description("Gets or sets the width of the frame (Sunken, Etched, Raised).")]
            [ChartTemplate(ChartTemplateSet.Simple)]
			public float Width
			{
				get 
                {
                    return m_width; 
                }

				set
				{
					if (m_width != value)
					{
						m_width = value;
						m_owner.RaiseChanged();
					}
				}
			}
			/// <summary>
			/// Gets or sets the dash style.
			/// </summary>
			/// <value>The dash style.</value>
			[DefaultValue(DashStyle.Solid), NotifyParentProperty(true)]
            [Description("Gets or sets the dash style.")]
            [ChartTemplate(ChartTemplateSet.Simple)]
			public DashStyle DashStyle
			{
				get 
                {
                    return m_dashStyle; 
                }

				set
				{
					if (m_dashStyle != value)
					{
						m_dashStyle = value;
						m_owner.RaiseChanged();
					}
				}
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="ChartBorderLineInfo"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			internal ChartBorderLineInfo(ChartBorderInfo owner)
			{
				m_owner = owner;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Creates the pen.
			/// </summary>
			/// <returns></returns>
			internal Pen CreatePen()
			{
				Pen pen = new Pen(m_color, m_width);

				if (m_dashStyle != DashStyle.Custom)
				{
					pen.DashStyle = m_dashStyle;
				}

				return pen;
			}
			#endregion
		}
		#endregion

		#region Constants
		private const string c_resourceName = "Syncfusion.Windows.Forms.Chart.Core.borders";

		private static readonly Color c_shadowColor = Color.FromArgb(0x80, Color.Gray);
		private static readonly Color c_darkColor = Color.FromArgb(0x90, Color.DarkGray);
		private static readonly Color c_lightColor = Color.FromArgb(0x90, Color.White);

		private const float c_pinRadius = 7;
		private const float c_roundRadius = 15;
		private const float c_secondRoundRadius = 8;
		private const float c_shadowOffset = 5;
		private const float c_borderSpacing = 2;

		private const float c_embossedEffect = 10;

		private const float c_pinnedPadding = 35;
		private const float c_etchedPadding = 30;
		private const float c_projectorPadding1 = 15;
		private const float c_projectorPadding2 = 30;
		private const float c_openWallWidth = 30;
		private const float c_roundedDiagonalPadding = 15;

		private const float c_gelBlinkWidth = 30;
		private const float c_gelBlinkOffset = 10;
		private const float c_gelPadding = 25;

		private const float c_sliceOuterRadius = 40;
		private const float c_sliceInnerPadding = 5;

		private static readonly ChartThickness c_openThikness = new ChartThickness(20, 0, 20, 0);

		private static readonly float c_cosSin45 = (float)Math.Cos(Math.PI / 4);
		#endregion

		#region Members
		private ChartBorderLineInfo m_intetior = null;
		private ChartThickness m_thickness = new ChartThickness(0);
		private ChartThickness m_frameThickness = new ChartThickness(15, 30, 15, 15);
		private ChartBorderSkinStyle m_style = ChartBorderSkinStyle.None;
		private Color m_baseColor = Color.Gray;

		private ChartImageBorder m_bevelBorder = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the interior of border line.
		/// </summary>
		/// <value>The interior.</value>
		[TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets the interior of border line.")]
        [ChartTemplate(ChartTemplateSet.Content)]
		public ChartBorderLineInfo Interior
		{
			get { return m_intetior; }
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(ChartBorderSkinStyle.None), NotifyParentProperty(true)]
        [Description("Gets or sets the skin for the border.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public ChartBorderSkinStyle SkinStyle
		{
			get
            {
                return m_style; 
            }
			set
			{
				if (m_style != value)
				{
					m_style = value;
					this.RaiseChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the base.
		/// </summary>
		/// <value>The color of the base.</value>
		[DefaultValue(typeof(Color), "Gray"), NotifyParentProperty(true)]
        [Description("Gets or sets the color of the base.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public Color BaseColor
		{
			get
            {
                return m_baseColor; 
            }

			set
			{
				if (m_baseColor != value)
				{
					m_baseColor = value;
					this.RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the frame thickness.
		/// </summary>
		/// <value>The frame thickness.</value>
		[DefaultValue(typeof(ChartThickness), "15, 30, 15, 15"), NotifyParentProperty(true)]
        [Description("Gets or sets the frame thickness.")]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public ChartThickness FrameThickness
		{
			get
			{
				return m_frameThickness;
			}
			set
			{
				if (m_frameThickness != value)
				{
					m_frameThickness = value;
					this.RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Gets the thickness of correct skin style.
		/// </summary>
		/// <value>The thickness.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
		public ChartThickness Thickness
		{
			get { return m_thickness; }
		}
		/// <summary>
		/// Gets the bevel border.
		/// </summary>
		/// <value>The bevel border.</value>
		private ChartImageBorder BevelBorder
		{
			get
			{
				if (m_bevelBorder == null)
				{
					ResourceManager resources = new ResourceManager(c_resourceName, typeof(ChartBorderInfo).Assembly);
					m_bevelBorder = new ChartImageBorder(resources, "bevel.xml");
				}

				return m_bevelBorder;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when 
		/// </summary>
		public event EventHandler Changed;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartBorderInfo"/> class.
		/// </summary>
		public ChartBorderInfo()
		{
			m_intetior = new ChartBorderLineInfo(this);
			m_thickness = this.ComputeThickness();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Draws the specified g.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		public void Draw(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			SmoothingMode smoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			switch (m_style)
			{
				case ChartBorderSkinStyle.None:
					this.DrawNoneBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Emboss:
					this.DrawEmbossBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Bevel:
					{
						g.Clip = this.BevelBorder.GetRegion(rect);
						this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
						this.BevelBorder.Draw(g, rect, m_baseColor);
						g.ResetClip();
					}
					break;
				case ChartBorderSkinStyle.Embed:
					this.DrawEmbedBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Frame:
					this.DrawFrameBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Pinned:
					this.DrawPinnedBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Open:
					this.DrawOpenBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.RoundedDiagonal:
					this.DrawRoundedDiagonalBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Slice:
					this.DrawSliceBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Projector:
					this.DrawProjectorBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Gel:
					this.DrawGelBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Sunken:
					this.DrawSunkenBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Etched:
					this.DrawEtchedBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
				case ChartBorderSkinStyle.Raised:
					this.DrawRaisedBorder(g, rect, fillBrush, backgroundImage, imageLayout);
					break;
			}

			g.SmoothingMode = smoothingMode; ;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Computes the thickness.
		/// </summary>
		/// <returns></returns>
		private ChartThickness ComputeThickness()
		{
			ChartThickness result = new ChartThickness(0);

			switch (m_style)
			{
				case ChartBorderSkinStyle.None:
					result = new ChartThickness(0);
					break;

				case ChartBorderSkinStyle.Emboss:
					result = new ChartThickness(c_borderSpacing + c_embossedEffect,
						c_borderSpacing + c_embossedEffect,
						c_borderSpacing + c_embossedEffect + c_shadowOffset,
						c_borderSpacing + c_embossedEffect + c_shadowOffset);
					break;

				case ChartBorderSkinStyle.Bevel:
					result = this.BevelBorder.Padding;
					break;

				case ChartBorderSkinStyle.Embed:
					result = new ChartThickness(c_borderSpacing + c_embossedEffect);
					break;

				case ChartBorderSkinStyle.Frame:
					result = ChartThickness.Add(m_frameThickness, new ChartThickness(c_borderSpacing + 5));
					break;

				case ChartBorderSkinStyle.Pinned:
					result = new ChartThickness(c_pinnedPadding);
					break;

				case ChartBorderSkinStyle.Open:
					result = ChartThickness.Add(c_openThikness, new ChartThickness(c_borderSpacing + 5));
					break;

				case ChartBorderSkinStyle.RoundedDiagonal:
					result = new ChartThickness(c_borderSpacing + 15);
					break;

				case ChartBorderSkinStyle.Slice:
					result = new ChartThickness(c_borderSpacing + c_roundRadius + 5);
					break;

				case ChartBorderSkinStyle.Projector:
					result = new ChartThickness(c_borderSpacing + 20, c_borderSpacing + 40, c_borderSpacing + 20, c_borderSpacing + 40);
					break;

				case ChartBorderSkinStyle.Gel:
					result = new ChartThickness(c_gelPadding + c_borderSpacing);
					break;

				case ChartBorderSkinStyle.Sunken:
					result = new ChartThickness(m_intetior.Width);
					break;

				case ChartBorderSkinStyle.Etched:
					result = new ChartThickness(c_borderSpacing + m_intetior.Width,
						c_borderSpacing + m_intetior.Width,
						c_borderSpacing + m_intetior.Width + c_shadowOffset,
						c_borderSpacing + m_intetior.Width + c_shadowOffset);
					break;

				case ChartBorderSkinStyle.Raised:
					result = new ChartThickness(m_intetior.Width);
					break;
			}

			return result;
		}

		/// <summary>
		/// Draws the none border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawNoneBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
		}
		/// <summary>
		/// Draws the emboss border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawEmbossBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			rect.Width -= c_shadowOffset;
			rect.Height -= c_shadowOffset;

			RectangleF shadowRect = rect;

			shadowRect.Offset(c_shadowOffset, c_shadowOffset);

			GraphicsPath gp = RenderingHelper.CreateRoundRect(shadowRect, c_roundRadius);

			this.FillPathGradient(g, gp, shadowRect, Color.Gray, Color.Transparent, 5);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			g.SetClip(outerPath);
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			this.DrawRoundBevel(g, rect, c_roundRadius, c_embossedEffect,
				Color.Transparent, c_lightColor, Color.Transparent, c_darkColor);

			using (Pen outerPen = new Pen(c_darkColor))
			{
				outerPen.Alignment = PenAlignment.Inset;
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the bevel border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawBevelBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			if (!rect.IsEmpty)
			{
				GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

				g.SetClip(outerPath);
				this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
				g.ResetClip();

				this.DrawRoundBevel(g, outerPath, rect, c_embossedEffect,
					c_roundRadius, c_lightColor, c_darkColor, false);

				rect.Inflate(-c_embossedEffect + 1, -c_embossedEffect + 1);
				GraphicsPath innerPath = RenderingHelper.CreateRoundRect(rect, c_secondRoundRadius - 5);

				this.DrawRoundBevel(g, innerPath, rect, c_embossedEffect,
					c_secondRoundRadius, c_lightColor, c_darkColor, true);

				using (Pen pen = new Pen(c_darkColor))
				{
					g.DrawPath(pen, outerPath);
				}
			}
		}
		/// <summary>
		/// Draws the emboss border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawEmbedBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			g.SetClip(outerPath);
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			this.DrawRoundBevel(g, rect, c_roundRadius, c_embossedEffect,
				Color.Transparent, c_darkColor, Color.Transparent, c_lightColor);

			using (Pen outerPen = new Pen(c_darkColor))
			{
				outerPen.Alignment = PenAlignment.Inset;
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the emboss border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawFrameBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			rect = m_frameThickness.Deflate(rect);

			GraphicsPath innerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			using (SolidBrush sb = new SolidBrush(m_baseColor))
			{
				g.FillPath(sb, outerPath);
			}

			using (Pen pen = new Pen(m_baseColor))
			{
				g.DrawPath(pen, outerPath);
			}

			g.SetClip(innerPath);
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			this.DrawRoundBevel(g, rect, c_roundRadius, 5,
				Color.Transparent, m_baseColor, Color.Transparent, Color.White);

			using (Pen outerPen = new Pen(m_baseColor))
			{
				g.DrawPath(outerPen, outerPath);
				g.DrawPath(outerPen, innerPath);
			}
		}
		/// <summary>
		/// Draws the pinned border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawPinnedBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			BrushPaint.FillPath(g, outerPath, ControlPaint.Light(m_baseColor));

			this.DrawRoundBevel(g, rect, c_roundRadius, 10,
				Color.Transparent, Color.White, Color.Transparent, c_darkColor);

			rect = RectangleF.Inflate(rect, -20, -20);

			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);

			SmoothingMode smoothingMode = g.SmoothingMode;

			g.SmoothingMode = SmoothingMode.AntiAlias;

			this.DrawPin(g, new PointF(rect.Left + 15, rect.Top + 15));
			this.DrawPin(g, new PointF(rect.Left + 15, rect.Bottom - 15));
			this.DrawPin(g, new PointF(rect.Right - 15, rect.Top + 15));
			this.DrawPin(g, new PointF(rect.Right - 15, rect.Bottom - 15));

			g.SmoothingMode = smoothingMode;

			using (Pen outerPen = new Pen(c_darkColor))
			{
				outerPen.Alignment = PenAlignment.Inset;
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the emboss border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawOpenBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			g.SetClip(outerPath);
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);

			RectangleF leftRect = new RectangleF(rect.Left, rect.Top, c_openWallWidth, rect.Height);
			RectangleF rightRect = new RectangleF(rect.Right - c_openWallWidth,
				rect.Top, c_openWallWidth, rect.Height);

			if (!leftRect.IsEmpty)
			{
				using (LinearGradientBrush leftBrush = new LinearGradientBrush(leftRect,
					Color.Transparent, m_baseColor, LinearGradientMode.Horizontal))
				{
					ColorBlend colorBlend = new ColorBlend();

					colorBlend.Colors = new Color[] { m_baseColor, ControlPaint.LightLight(m_baseColor), m_baseColor, m_baseColor };
					colorBlend.Positions = new float[] { 0f, 0.25f, 0.6f, 1f };

					leftBrush.InterpolationColors = colorBlend;
					g.FillRectangle(leftBrush, leftRect);
				}
			}

			if (!rightRect.IsEmpty)
			{
				using (LinearGradientBrush rightBrush = new LinearGradientBrush(rightRect,
					m_baseColor, Color.Transparent, LinearGradientMode.Horizontal))
				{
					ColorBlend colorBlend = new ColorBlend();

					colorBlend.Colors = new Color[] { m_baseColor, m_baseColor, ControlPaint.LightLight(m_baseColor), m_baseColor };
					colorBlend.Positions = new float[] { 0f, 0.4f, 0.75f, 1f };

					rightBrush.InterpolationColors = colorBlend;
					g.FillRectangle(rightBrush, rightRect);
				}
			}

			g.ResetClip();

			using (Pen outerPen = new Pen(m_baseColor))
			{
				outerPen.Alignment = PenAlignment.Inset;
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the rounded diagonal border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawRoundedDiagonalBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, 35, 0, 35, 0);

			rect.Inflate(-c_roundedDiagonalPadding, -c_roundedDiagonalPadding);

			if (!rect.IsEmpty)
			{
				if (rect.Width > c_roundedDiagonalPadding
					&& rect.Height > c_roundedDiagonalPadding)
				{
					outerPath.AddEllipse(new RectangleF(rect.Left, rect.Top, 5, 5));
					outerPath.AddEllipse(new RectangleF(rect.Left + 7, rect.Top, 5, 5));
					outerPath.AddEllipse(new RectangleF(rect.Left, rect.Top + 7, 5, 5));

					outerPath.AddEllipse(new RectangleF(rect.Right - 5, rect.Bottom - 5, 5, 5));
					outerPath.AddEllipse(new RectangleF(rect.Right - 12, rect.Bottom - 5, 5, 5));
					outerPath.AddEllipse(new RectangleF(rect.Right - 5, rect.Bottom - 12, 5, 5));
				}

				GraphicsPath innerPath = RenderingHelper.CreateRoundRect(rect, 30, 0, 30, 0);

				g.SetClip(innerPath);
				this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
				g.ResetClip();

				outerPath.AddPath(innerPath, false);
			}

			BrushPaint.FillPath(g, outerPath, m_baseColor);

			using (Pen outerPen = new Pen(c_darkColor))
			{
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the slice border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawSliceBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_sliceOuterRadius);

			rect.Inflate(-c_sliceInnerPadding, -c_sliceInnerPadding);

			using (Pen pen = new Pen(m_baseColor))
			{
				BrushPaint.FillPath(g, outerPath, fillBrush);
				g.DrawPath(pen, outerPath);

				GraphicsPath innerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

				g.SetClip(innerPath);
				this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
				g.ResetClip();

				this.DrawRoundBevel(g, rect, c_roundRadius, c_embossedEffect,
					Color.Transparent, c_lightColor, Color.Transparent, ControlPaint.Light(m_baseColor));

				g.DrawPath(pen, innerPath);
			}
		}
		/// <summary>
		/// Draws the projector border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawProjectorBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			RectangleF backRect = new RectangleF(rect.X + 0.3f * rect.Width, rect.Y, 0.7f * rect.Width, rect.Height);
			GraphicsPath backPath = RenderingHelper.CreateRoundRect(backRect, 15);

			BrushPaint.FillPath(g, backPath, m_baseColor);

			rect.Inflate(0, -c_projectorPadding1);

			GraphicsPath innerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			BrushPaint.FillPath(g, innerPath, ControlPaint.Light(m_baseColor));

			g.SetClip(innerPath);

			GraphicsPath imagePath = new GraphicsPath();

			imagePath.AddBezier(rect.X, rect.Y - 0.1f * rect.Height,
				rect.X + 0.5f * rect.Width, rect.Y + 0.2f * rect.Height,
				rect.X + 1.2f * rect.Width, rect.Y + 0.25f * rect.Height,
				rect.X + 1.3f * rect.Width, rect.Y + 0.3f * rect.Height);
			imagePath.AddBezier(rect.X + 1.3f * rect.Width, rect.Y + 0.32f * rect.Height,
				rect.X + 1.4f * rect.Width, rect.Y + 0.25f * rect.Height,
				rect.X + 0.4f * rect.Width, rect.Y + 0.35f * rect.Height,
				rect.X, rect.Y + 0.3f * rect.Height);
			imagePath.CloseFigure();

			imagePath.AddBezier(rect.X, rect.Y + 0.9f * rect.Height,
				rect.X + 0.8f * rect.Width, rect.Y + 0.9f * rect.Height,
				rect.X + 1.2f * rect.Width, rect.Y + 0.6f * rect.Height,
				rect.X + 1.3f * rect.Width, rect.Y + 0.8f * rect.Height);
			imagePath.AddBezier(rect.X + 1.3f * rect.Width, rect.Y + 0.8f * rect.Height,
				rect.X + 1.7f * rect.Width, rect.Y + 0.9f * rect.Height,
				rect.X, rect.Y + 0.85f * rect.Height,
				rect.X, rect.Y + 1.3f * rect.Height);
			imagePath.CloseFigure();

			using (SolidBrush sb = new SolidBrush(ControlPaint.LightLight(m_baseColor)))
			{
				g.FillPath(sb, imagePath);
			}

			g.ResetClip();

			this.DrawRoundBevel(g, rect, c_roundRadius, 10,
				Color.Transparent, Color.White, Color.Transparent, c_darkColor);

			rect = RectangleF.Inflate(rect, -c_projectorPadding2, -c_projectorPadding2);

			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);

			using (Pen pen = new Pen(ControlPaint.Light(m_baseColor)))
			{
				g.DrawPath(pen, innerPath);
			}
		}
		/// <summary>
		/// Draws the gel border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawGelBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
            rect.Inflate(-c_borderSpacing, -c_borderSpacing);                     
            
			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

            if (rect.Height > 0 && rect.Width > 0)
            {
                using (LinearGradientBrush backBrush = new LinearGradientBrush(rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical))
                {
                    ColorBlend colorBlend = new ColorBlend();

                    colorBlend.Colors = new Color[] { m_baseColor, ControlPaint.Light(m_baseColor), 
					ControlPaint.Light(m_baseColor), m_baseColor };
                    colorBlend.Positions = new float[] { 0f, 50f / rect.Height, 1 - (50f / rect.Height), 1f };

                    backBrush.InterpolationColors = colorBlend;

                    g.FillPath(backBrush, outerPath);
                }

                this.DrawRoundBevel(g, rect, c_roundRadius, 10,
                    Color.Transparent, c_lightColor, Color.Transparent, DrawingHelper.AddColor(m_baseColor, -40));

                RectangleF leftRect = new RectangleF(rect.X, rect.Y + c_gelBlinkOffset,
                    c_gelBlinkWidth, rect.Height - 2 * c_gelBlinkOffset);
                GraphicsPath leftPath = RenderingHelper.CreateRoundRect(leftRect, c_roundRadius);

                this.FillPathGradient(g, leftPath, leftRect, c_lightColor, Color.Transparent, 25);

                RectangleF rightRect = new RectangleF(rect.Right - c_gelBlinkWidth, rect.Y + c_gelBlinkOffset,
                    c_gelBlinkWidth, rect.Height - 2 * c_gelBlinkOffset);
                GraphicsPath rightPath = RenderingHelper.CreateRoundRect(rightRect, c_roundRadius);

                this.FillPathGradient(g, rightPath, rightRect, c_lightColor, Color.Transparent, 25);

                using (Pen pen = new Pen(ControlPaint.Light(m_baseColor)))
                {
                    g.DrawPath(pen, outerPath);
                }
            }
		}
		/// <summary>
		/// Draws the sunken border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawSunkenBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
            rect.Inflate(-c_borderSpacing, -c_borderSpacing);
            
			RectangleF innerRect = RectangleF.Inflate(rect, -m_intetior.Width, -m_intetior.Width);

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);
			GraphicsPath innerPath = RenderingHelper.CreateRoundRect(innerRect, Math.Max(0, c_roundRadius - m_intetior.Width));

			g.SetClip(innerPath);
			this.DrawBackground(g, innerRect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			this.DrawRoundBevel(g, outerPath, rect, m_intetior.Width, c_roundRadius,
				ControlPaint.Dark(m_intetior.ForeColor), ControlPaint.Light(m_intetior.ForeColor), false);
		}
		/// <summary>
		/// Draws the etched border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawEtchedBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			rect.Inflate(-c_borderSpacing, -c_borderSpacing);

			rect.Width -= c_shadowOffset;
			rect.Height -= c_shadowOffset;

			RectangleF shadowRect = rect;

			shadowRect.Offset(c_shadowOffset, c_shadowOffset);

			GraphicsPath gp = RenderingHelper.CreateRoundRect(shadowRect, c_roundRadius);
			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);

			this.FillPathGradient(g, gp, shadowRect, Color.Gray, Color.Transparent, 5);

			rect.Inflate(-m_intetior.Width, -m_intetior.Width);

			g.SetClip(outerPath);
			this.DrawBackground(g, rect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			using (Pen outerPen = m_intetior.CreatePen())
			{
				outerPen.Alignment = PenAlignment.Inset;
				g.DrawPath(outerPen, outerPath);
			}
		}
		/// <summary>
		/// Draws the raised border.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawRaisedBorder(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{   
            rect.Inflate(-c_borderSpacing, -c_borderSpacing);
            
			RectangleF innerRect = RectangleF.Inflate(rect, -m_intetior.Width, -m_intetior.Width);
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;

			GraphicsPath outerPath = RenderingHelper.CreateRoundRect(rect, c_roundRadius);
			GraphicsPath innerPath = RenderingHelper.CreateRoundRect(innerRect, Math.Max(0, c_roundRadius - m_intetior.Width));

			g.SetClip(innerPath);
			this.DrawBackground(g, innerRect, fillBrush, backgroundImage, imageLayout);
			g.ResetClip();

			this.DrawRoundBevel(g, outerPath, rect, m_intetior.Width, c_roundRadius,
				ControlPaint.Dark(m_intetior.ForeColor), ControlPaint.Light(m_intetior.ForeColor), true);
			//this.DrawRoundBevel(g, outerPath, rect, m_intetior.Width, c_roundRadius, 
			//  ControlPaint.Light(m_intetior.ForeColor), ControlPaint.Dark(m_intetior.ForeColor), true);
			g.PixelOffsetMode = PixelOffsetMode.Default;
		}

		/// <summary>
		/// Raises the changed.
		/// </summary>
		private void RaiseChanged()
		{
			m_thickness = this.ComputeThickness();

			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Draws the round bevel.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="cornerRadius">The corner radius.</param>
		/// <param name="bevelRadius">The bevel radius.</param>
		/// <param name="lightColor1">The light color1.</param>
		/// <param name="lightColor2">The light color2.</param>
		/// <param name="darkColor1">The dark color1.</param>
		/// <param name="darkColor2">The dark color2.</param>
		private void DrawRoundBevel(Graphics g, RectangleF rect, float cornerRadius, float bevelRadius,
			Color lightColor1, Color lightColor2, Color darkColor1, Color darkColor2)
		{
			GraphicsPath path = RenderingHelper.CreateRoundRect(rect, cornerRadius);

			g.SetClip(path);
			g.ExcludeClip(new Region(RectangleF.Inflate(rect, -bevelRadius - cornerRadius, -bevelRadius - cornerRadius)));

			RectangleF tempRect = new RectangleF(rect.X - bevelRadius, rect.Y - bevelRadius,
				rect.Width + bevelRadius, rect.Height + bevelRadius);
			GraphicsPath path1 = RenderingHelper.CreateRoundRect(tempRect, cornerRadius);
			//this.DrawPathGradient(g, path1, tempRect, path, darkColor1, darkColor2, 2 * bevelRadius, 3 * bevelRadius);
			this.FillPathGradient(g, path1, tempRect, darkColor1, darkColor2, 2 * bevelRadius);

			tempRect = new RectangleF(rect.X, rect.Y, rect.Width * 2, rect.Height * 2);
			GraphicsPath path2 = RenderingHelper.CreateRoundRect(tempRect, cornerRadius);
			//this.DrawPathGradient(g, path2, tempRect, path, lightColor1, lightColor2, 2 * bevelRadius, 3 * bevelRadius);
			this.FillPathGradient(g, path2, tempRect, lightColor1, lightColor2, 2 * bevelRadius);

			g.ResetClip();
		}
		/// <summary>
		/// Draws the round bevel.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="path">The path.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="bevelRadius">The bevel radius.</param>
		/// <param name="cornerRadius">The corner radius.</param>
		/// <param name="lightColor">Color of the light.</param>
		/// <param name="darkColor">Color of the dark.</param>
		/// <param name="rasied">if set to <c>true</c> [rasied].</param>
		private void DrawRoundBevel(Graphics g, GraphicsPath path, RectangleF rect, float bevelRadius,
			float cornerRadius, Color lightColor, Color darkColor, bool rasied)
		{
			rect.Size = new SizeF(rect.Width + 1, rect.Height + 1);

			GraphicsPath gp = RenderingHelper.CreateRoundRect(rect, rasied ? 0 : cornerRadius, 0, rasied ? cornerRadius : 0, 0);
            if (rect.Height > 0 && rect.Width > 0)
            {
                using (PathGradientBrush outerBrush = new PathGradientBrush(gp))
                {
                    outerBrush.CenterPoint = rasied ? new PointF(rect.Right, rect.Bottom) : new PointF(0, 0);
                    outerBrush.CenterColor = lightColor;
                    outerBrush.SurroundColors = new Color[] { Color.Transparent };
                    outerBrush.FocusScales = new PointF(1 - 2 * bevelRadius / rect.Width,
                        1 - 2 * bevelRadius / rect.Height);

                    using (Pen pen = new Pen(outerBrush, bevelRadius))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        pen.LineJoin = LineJoin.Round;

                        g.DrawPath(pen, path);
                    }

                    outerBrush.CenterPoint = rasied ? new PointF(0, 0) : new PointF(rect.Right, rect.Bottom);
                    outerBrush.CenterColor = darkColor;
                    outerBrush.SurroundColors = new Color[] { Color.Transparent };

                    using (Pen pen = new Pen(outerBrush, bevelRadius))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        pen.LineJoin = LineJoin.Round;

                        g.DrawPath(pen, path);
                    }
                }
            }
		}

		/// <summary>
		/// Fills the path gradient.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="path">The path.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="color1">The color1.</param>
		/// <param name="color2">The color2.</param>
		/// <param name="focusScale">The focus scale.</param>
		private void FillPathGradient(Graphics g, GraphicsPath path,
			RectangleF bounds, Color color1, Color color2, float focusScale)
		{
			if (!bounds.IsEmpty)
			{
				using (PathGradientBrush brush = new PathGradientBrush(path))
				{
					brush.CenterPoint = new PointF(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);
					brush.CenterColor = color1;
					brush.SurroundColors = new Color[] { color2 };
					brush.FocusScales = new PointF(1 - focusScale / bounds.Width, 1 - focusScale / bounds.Height);

					g.FillPath(brush, path);
				}
			}
		}
		/// <summary>
		/// Draws the path gradient.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="brushPath">The brush path.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="path">The path.</param>
		/// <param name="color1">The color1.</param>
		/// <param name="color2">The color2.</param>
		/// <param name="focusScale">The focus scale.</param>
		/// <param name="width">The width.</param>
		private void DrawPathGradient(Graphics g, GraphicsPath brushPath, RectangleF bounds, GraphicsPath path,
			Color color1, Color color2, float focusScale, float width)
		{
			if (!bounds.IsEmpty)
			{
				using (PathGradientBrush brush = new PathGradientBrush(brushPath))
				{
					brush.CenterPoint = new PointF(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);
					brush.CenterColor = color1;
					brush.SurroundColors = new Color[] { color2 };
					brush.FocusScales = new PointF(1 - focusScale / bounds.Width, 1 - focusScale / bounds.Height);

					using (Pen pen = new Pen(brush, width))
					{
						pen.Alignment = PenAlignment.Inset;
						g.DrawPath(pen, path);
					}
				}
			}
		}

		/// <summary>
		/// Draws the pin.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="center">The center.</param>
		private void DrawPin(Graphics g, PointF center)
		{
			using (Pen pen = new Pen(Color.FromArgb(0x80, Color.Gray), 2))
			{
				g.DrawEllipse(pen, center.X - c_pinRadius, center.Y - c_pinRadius,
					2 * c_pinRadius, 2 * c_pinRadius);
				g.DrawLine(pen, center.X + c_cosSin45 * c_pinRadius, center.Y - c_cosSin45 * c_pinRadius,
					center.X - c_cosSin45 * c_pinRadius, center.Y + c_cosSin45 * c_pinRadius);

				pen.Color = Color.FromArgb(0x80, m_baseColor);
				center = new PointF(center.X + 1, center.Y + 1);

				g.DrawEllipse(pen, center.X - c_pinRadius, center.Y - c_pinRadius,
					2 * c_pinRadius, 2 * c_pinRadius);
				g.DrawLine(pen, center.X + c_cosSin45 * c_pinRadius, center.Y - c_cosSin45 * c_pinRadius,
					center.X - c_cosSin45 * c_pinRadius, center.Y + c_cosSin45 * c_pinRadius);
			}
		}
		/// <summary>
		/// Draws the background.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="rect">The rect.</param>
		/// <param name="fillBrush">The fill brush.</param>
		/// <param name="backgroundImage">The background image.</param>
		/// <param name="imageLayout">The image layout.</param>
		private void DrawBackground(Graphics g, RectangleF rect, BrushInfo fillBrush, Image backgroundImage, ChartImageLayout imageLayout)
		{
			BrushPaint.FillRectangle(g, rect, fillBrush);

			if (backgroundImage != null)
			{
				switch (imageLayout)
				{
					case ChartImageLayout.Center:
						g.DrawImage(backgroundImage, rect.X + 0.5f * (rect.Width - backgroundImage.Width),
							rect.Y + 0.5f * (rect.Height - backgroundImage.Height));
						break;

					case ChartImageLayout.None:
						g.DrawImage(backgroundImage, rect.Location);
						break;

					case ChartImageLayout.Stretch:
						g.DrawImage(backgroundImage, rect);
						break;

					case ChartImageLayout.Tile:
						{
							using (TextureBrush brush = new TextureBrush(backgroundImage))
							{
								g.FillRectangle(brush, rect);
							}
						}
						break;

					case ChartImageLayout.Zoom:
						{
							float dx = rect.Width / backgroundImage.Width;
							float dy = rect.Height / backgroundImage.Height;

							if (dx < dy)
							{
								float height = dx * backgroundImage.Height;
								g.DrawImage(backgroundImage, rect.X, rect.Y + 0.5f * (rect.Height - height), rect.Width, height);
							}
							else
							{
								float width = dy * backgroundImage.Width;
								g.DrawImage(backgroundImage, rect.X + 0.5f * (rect.Width - width), rect.Y, width, rect.Height);
							}
						}
						break;
				}
			}
		}
		#endregion
	}
}
