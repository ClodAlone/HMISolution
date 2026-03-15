#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart.Renderers
{
	/// <summary>
	/// 
	/// </summary>
	class ChartHeatMapRenderer : ChartSeriesRenderer
	{
		#region Internal types
		/// <summary>
		/// 
		/// </summary>
		class HeatRectangle
		{
			public RectangleF Rectangle;
			public double AreaCoeficient;
			public double ColorCoeficient;
			public ChartStyledPoint StyledPoint;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Get description of regions.
		/// </summary>
		/// <value></value>
		protected override string RegionDescription
		{
			get
			{
				return "HeatMap Chart Renderer";
			}
		}
		/// <summary>
		/// Gets count of require Y values of the points.
		/// </summary>
		/// <value></value>
		protected override int RequireYValuesCount
		{
			get
			{
				return 2;
			}
		}
		/// <summary>
		/// Indicates how much space this type will use.
		/// </summary>
		/// <value></value>
		public override ChartUsedSpaceType FillSpaceType
		{
			get
			{
				return ChartUsedSpaceType.All;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartHeatMapRenderer"/> class.
		/// </summary>
		/// <param name="series">ChartSeries that will be rendered by this renderer instance.</param>
		public ChartHeatMapRenderer(ChartSeries series)
			: base(series)
		{
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs2D args)
		{
			RectangleF bounds = args.Bounds;
			ChartHeatMapConfigItem config = args.Series.ConfigItems.HeatMapItem;

            if (config.DisplayColorSwatch)
			{
				float dim = this.DrawColorSwatch(args);

				bounds.Y += dim;
				bounds.Height -= dim;
			}

			double minColors = double.MaxValue;
			double maxColors = double.MinValue;

			ChartStyledPoint[] points = this.PrepearePoints();
			HeatRectangle[] rects = new HeatRectangle[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				rects[i] = new HeatRectangle(); 
				rects[i].StyledPoint = points[i];

				minColors = Math.Min(minColors, points[i].YValues[1]);
				maxColors = Math.Max(maxColors, points[i].YValues[1]);
			}

			for (int i = 0; i < points.Length; i++)
			{
				rects[i].ColorCoeficient = (points[i].YValues[1] - minColors) / (maxColors - minColors);
			}

			this.ComputeAreaCoeficient(rects, 0, rects.Length);
			
			Array.Sort<HeatRectangle>(rects, new Comparison<HeatRectangle>(CompareHeatRectangles));

            switch (config.HeatMapStyle)
			{
				case ChartHeatMapLayoutStyle.Rectangular:
					this.RectangleLayout(rects, bounds);
					break;

				case ChartHeatMapLayoutStyle.Vertical:
					this.VerticalLayout(rects, bounds);
					break;

				case ChartHeatMapLayoutStyle.Horizontal:
					this.HorizontalLayout(rects, bounds);
					break;
			}

			foreach (HeatRectangle rect in rects)
			{
				ChartRegion rgn = this.DrawRectangle(args, rect);

				if (rgn != null)
				{
					args.Chart.ChartRegions.Add(rgn);
				}
			}

			base.Render(args);
		}
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public override void Render(ChartRenderArgs3D args)
		{
			ChartRenderArgs2D args2D = new ChartRenderArgs2D(args.Chart, args.Series);

			args2D.Graph = new ChartGDIGraph(args.Graph.Graphics);
			args2D.Bounds = args.Bounds;

			this.Render(args2D);
		}
		/// <summary>
		/// Draws the color swatch element.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		private float DrawColorSwatch(ChartRenderArgs2D args)
		{
			float dimention = this.SeriesStyle.GdipFont.Height;
			ChartHeatMapConfigItem configItem = args.Series.ConfigItems.HeatMapItem;
            float dblMargins = configItem.LabelMargins;
			Font font = this.SeriesStyle.GdipFont;
			Brush textBrush = new SolidBrush(this.SeriesStyle.TextColor);

            SizeF fromSize = args.Graph.MeasureString(configItem.StartText, font);
			SizeF toSize = args.Graph.MeasureString(configItem.EndText, font);
            SizeF titleSize = configItem.DisplayTitle ?
				args.Graph.MeasureString(args.Series.Text, font) : SizeF.Empty;		
			
			if (!fromSize.IsEmpty) fromSize.Width += dblMargins;
            if (!toSize.IsEmpty) toSize.Width += dblMargins; 
			
			dimention = Math.Max(dimention, fromSize.Height);
			dimention = Math.Max(dimention, toSize.Height);
			dimention = Math.Max(dimention, titleSize.Height);

			RectangleF bounds = Rectangle.Ceiling(args.Bounds);
			bounds.Height = dimention;

			if (!titleSize.IsEmpty)
			{
				RectangleF tltRect = Rectangle.Ceiling(new RectangleF(bounds.Location, titleSize));

				args.Graph.DrawString(args.Series.Text, font, 
					textBrush, tltRect, DrawingHelper.CenteredFormat);
				args.Graph.DrawRect(Pens.Black, tltRect);

				bounds.X += titleSize.Width;
				bounds.Width -= titleSize.Width;
			}

			if (!fromSize.IsEmpty)
			{
				RectangleF fromRect = Rectangle.Ceiling(new RectangleF(bounds.Location, fromSize));

                args.Graph.DrawString(configItem.StartText, font,
					textBrush, fromRect, DrawingHelper.CenteredFormat);
				args.Graph.DrawRect(Pens.Black, Rectangle.Ceiling(fromRect));

				bounds.X += fromRect.Width;
				bounds.Width -= fromRect.Width;
			}

			if (!toSize.IsEmpty)
			{
				RectangleF toRect = Rectangle.Ceiling(new RectangleF(bounds.Location, toSize));

				toRect.X = bounds.Right - toRect.Width;

				args.Graph.DrawString(configItem.EndText, font,
					textBrush, toRect, DrawingHelper.CenteredFormat);
				args.Graph.DrawRect(Pens.Black, Rectangle.Ceiling(toRect));

				bounds.Width -= toRect.Width;
			}

			textBrush.Dispose();

			if (!bounds.IsEmpty)
			{
				using (LinearGradientBrush brush = new LinearGradientBrush(Rectangle.Ceiling(bounds),
                    configItem.LowestValueColor, configItem.HighestValueColor, LinearGradientMode.Horizontal))
				{
					ColorBlend blend = new ColorBlend();

					blend.Positions = new float[] { 0, 0.5f, 1 };
                    blend.Colors = new Color[]{ configItem.LowestValueColor, 
						configItem.MiddleValueColor, configItem.HighestValueColor};

					brush.InterpolationColors = blend;

					args.Graph.DrawRect(brush, Pens.Black, Rectangle.Ceiling(bounds));
				}
			}

			return dimention;
		}
		/// <summary>
		/// Arranges elements in vertical.
		/// </summary>
		/// <param name="rects">The rects.</param>
		/// <param name="bounds">The bounds.</param>
		private void VerticalLayout(IList<HeatRectangle> rects, RectangleF bounds)
		{
			float y = bounds.Y;

			foreach (HeatRectangle rect in rects)
			{
				float height = (float)(rect.AreaCoeficient * bounds.Height); 
				rect.Rectangle = new RectangleF(bounds.X, y, bounds.Width, height);
				y += height;
			}
		}
		/// <summary>
		/// Arranges elements in horizontal.
		/// </summary>
		/// <param name="rects">The rects.</param>
		/// <param name="bounds">The bounds.</param>
		private void HorizontalLayout(IList<HeatRectangle> rects, RectangleF bounds)
		{
			float x = bounds.X;

			foreach (HeatRectangle rect in rects)
			{
				float width = (float)(rect.AreaCoeficient * bounds.Width);
				rect.Rectangle = new RectangleF(x, bounds.Y, width, bounds.Height);
				x += width;
			}
		}
		/// <summary>
		/// Arranges elements in ractangles.
		/// </summary>
		/// <param name="rects">The rects.</param>
		/// <param name="bounds">The bounds.</param>
		private void RectangleLayout(IList<HeatRectangle> rects, RectangleF bounds)
		{
			RectangleF currectBounds = bounds;
			List<HeatRectangle> collector = new List<HeatRectangle>();

			double valueCollector = 0;
			bool vertical = currectBounds.Height < currectBounds.Width;
			double breakLength = this.GetAmount(rects, 0, currectBounds, vertical);
			double area = bounds.Width * bounds.Height;

			for (int i = 0, ci = rects.Count - 1; i <= ci; i++)
			{
				collector.Add(rects[i]);
				valueCollector += rects[i].AreaCoeficient;

				float totalLength = vertical ? currectBounds.Height : currectBounds.Width;

				if (area * valueCollector / breakLength > totalLength || i == ci)
				{
					RectangleF subBounds = currectBounds;

					this.ComputeAreaCoeficient(collector, 0, collector.Count);
					this.ComputeAreaCoeficient(rects, i + 1, rects.Count - i - 1);

					float length = (float)(area * valueCollector / totalLength);

					if (vertical)
					{
						subBounds.Width = length;
						currectBounds.X += length;
						currectBounds.Width -= length;

						this.VerticalLayout(collector, subBounds);
					}
					else
					{
						subBounds.Height = length;
						currectBounds.Y += length;
						currectBounds.Height -= length;

						this.HorizontalLayout(collector, subBounds);
					}

					if (i != ci)
					{
						breakLength = this.GetAmount(rects, i, currectBounds, vertical);
						area = currectBounds.Width * currectBounds.Height;
						vertical = currectBounds.Height < currectBounds.Width;
						valueCollector = 0;
						collector.Clear();
					}
				}
			}
		}
		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		private ChartRegion DrawRectangle(ChartRenderArgs2D args, HeatRectangle rect)
		{ 
			ChartStyleInfo style = rect.StyledPoint.Style;
			ChartHeatMapConfigItem configItem = args.Series.ConfigItems.HeatMapItem;
			Color color = LeprColor(configItem, rect.ColorCoeficient);

			using (SolidBrush brush = new SolidBrush(color))
			{
				args.Graph.DrawRect(brush, style.GdipPen, Rectangle.Round(rect.Rectangle));
			}

			if (style.DisplayText)
			{
				bool showLabel = true;
				Font font = style.GdipFont;
				StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
				string text = this.GetText(style.Text, configItem);
				float width = int.MaxValue;// (float)Math.Truncate(rect.Rectangle.Width);

				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Center;

                if (configItem.EnableLabelRotation && rect.Rectangle.Width < rect.Rectangle.Height)
				{
					stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
					//width = (float)Math.Truncate(rect.Rectangle.Height);
				}

                if (configItem.AllowLabelsAutoFit)
				{
					SizeF textSize = args.Graph.MeasureString(text, font, width, stringFormat);

					float cx = rect.Rectangle.Width / textSize.Width;
					float cy = rect.Rectangle.Height / textSize.Height;
                    float fontSize = Math.Max(configItem.MinimumFontSize, 
						(float)Math.Truncate(Math.Min(cx, cy) * font.Size));

					if (fontSize < font.Size)
					{
						font = new Font(font.FontFamily, fontSize, font.Style);
					}
				}

                if (!configItem.ShowLargeLabels)
				{
					SizeF textSize = args.Graph.MeasureString(text, font, width, stringFormat);

					showLabel = textSize.Width < Math.Ceiling(rect.Rectangle.Width) 
						&& textSize.Height < Math.Ceiling(rect.Rectangle.Height);
				}

				if (showLabel)
				{
					using (SolidBrush brush = new SolidBrush(style.TextColor))
					{
						args.Graph.DrawString(text, font, brush, rect.Rectangle, stringFormat);
					}
				}
			}

			return args.UpdateRegions ? new ChartRegion(new Region(rect.Rectangle),
				args.SeriesIndex, rect.StyledPoint.Index, rect.StyledPoint.ToolTip, 
				this.RegionDescription) : null;
		}
		/// <summary>
		/// Return the truncate text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="configItem">The config item.</param>
		/// <returns></returns>
		private string GetText(string text, ChartHeatMapConfigItem configItem)
		{
            if (configItem.EnableLabelsTruncation && configItem.MaximumCharacters > 0
                && configItem.MaximumCharacters < text.Length)
			{
                return text.Substring(0, configItem.MaximumCharacters) + "...";
			}

			return text;
		}
		/// <summary>
		/// Returns the maximal length of rectangle.
		/// </summary>
		/// <param name="rects">The rects.</param>
		/// <param name="index">The index.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="vertival">if set to <c>true</c> [vertival].</param>
		/// <returns></returns>
		private double GetAmount(IList<HeatRectangle> rects, int index, RectangleF bounds, bool vertival)
		{
			double area = rects[index].AreaCoeficient * bounds.Width * bounds.Height;
			return area / (1.2 * Math.Sqrt(area));
		}
		/// <summary>
		/// Computes the area coeficient.
		/// </summary>
		/// <param name="rects">The rects.</param>
        /// <param name="start"></param>
        /// <param name="length"></param>
		private void ComputeAreaCoeficient(IList<HeatRectangle> rects, int start, int length)
		{
			double sumArea = 0;

			for (int i = start, to = start + length; i < to; i++)
			{
				sumArea += rects[i].StyledPoint.YValues[0];
			}

			for (int i = start, to = start + length; i < to; i++)
			{
				rects[i].AreaCoeficient = rects[i].StyledPoint.YValues[0] / sumArea;
			}
		}
		/// <summary>
		/// Leprs the color.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private Color LeprColor(ChartHeatMapConfigItem item, double value)
		{
			if (value < 0.5)
			{
                return DrawingHelper.LeprColor(item.LowestValueColor, item.MiddleValueColor, 2 * value);
			}

             if (value == 1)
                return DrawingHelper.LeprColor(item.MiddleValueColor, item.HighestValueColor, value);
            else
                return DrawingHelper.LeprColor(item.HighestValueColor, item.MiddleValueColor,Math.Round((1 - value),15)* 2);
		}
		/// <summary>
		/// Compares the heat rectangles.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		private static int CompareHeatRectangles(HeatRectangle x, HeatRectangle y)
		{
			return y.AreaCoeficient.CompareTo(x.AreaCoeficient);
		}
		/// <summary>
		/// Overloaded. Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal override void RenderAdornments(Graphics g)
		{
		}
		/// <summary>
		/// Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal override void RenderAdornments(Graphics3D g)
		{
		}
		#endregion
	}
}
