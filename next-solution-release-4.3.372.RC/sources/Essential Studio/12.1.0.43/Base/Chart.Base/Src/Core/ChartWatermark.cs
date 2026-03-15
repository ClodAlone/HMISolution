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
using System.Drawing.Drawing2D;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the watermark
    /// </summary>
    public enum ChartWaterMarkOrder
    {
        /// <summary>
        /// The watermark will be rendered over chart.
        /// </summary>
        Over,

        /// <summary>
        /// The watermark will be rendered behind chart.
        /// </summary>
        Behind
    }

    /// <summary>
    /// Represents the watermark properties.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ChartWatermark
    {
        #region Constants
        private const float c_persentToByte = 2.55f;
        #endregion

        #region Members
        private string m_text = string.Empty;
        private readonly ChartArea m_area;
        private ChartAlignment m_verticalAlignment = ChartAlignment.Near;
        private ChartAlignment m_horizontalAlignment = ChartAlignment.Near;
        private Font m_font;
        private Color m_color = Color.Empty;
        private Image m_image;
        private Size? m_imageSize = null;
        private ChartWaterMarkOrder m_zOrder = ChartWaterMarkOrder.Over;
        private ChartThickness m_margin = new ChartThickness(10);
        private float m_opacity = 60;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DefaultValue(""), Description("Specifies the title text of watermark"), NotifyParentProperty(true)]
        public string Text
        {
            get 
            {
                return m_text; 
            }

            set
            {
                if (m_text != value)
                {
                    m_text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        [Description("Specifies the title font of watermark"), NotifyParentProperty(true)]
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
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        [DefaultValue(ChartAlignment.Near), Description("Indicates the vertical alignment of watermark"), NotifyParentProperty(true)]
        public ChartAlignment VerticalAlignment
        {
            get
            {
                return m_verticalAlignment; 
            }

            set
            {
                if (m_verticalAlignment != value)
                {
                    m_verticalAlignment = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        [DefaultValue(ChartAlignment.Near), Description("Indicates the horizontal alignment of watermark"), NotifyParentProperty(true)]
        public ChartAlignment HorizontalAlignment
        {
            get 
            {
                return m_horizontalAlignment; 
            }

            set
            {
                if (m_horizontalAlignment != value)
                {
                    m_horizontalAlignment = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        [Description("Indicates the title color of watermark"), NotifyParentProperty(true)]
        
        public Color TextColor
        {
            get 
            {
                return m_color.IsEmpty ? m_area.Chart.ForeColor : m_color; 
            }

            set
            {
                if (m_color != value)
                {
                    m_color = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [DefaultValue(null), Description("Specifies the image of watermark"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Image Image
        {
            get 
            {
                return m_image; 
            }

            set
            {
                if (m_image != value)
                {
                    m_image = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        [Browsable(false)]
        [DefaultValue(null), Description("Indicates the image size of watermark"), NotifyParentProperty(true)]
        public Size? ImageSize
        {
            get 
            {
                return m_imageSize; 
            }

            set
            {
                if (m_imageSize != value)
                {
                    m_imageSize = value;

                    if (m_image != null)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the depth order.
        /// </summary>
        /// <value>The depth order.</value>
        [DefaultValue(ChartWaterMarkOrder.Over), Description("Indicates the depth order of watermark"), NotifyParentProperty(true)]
        public ChartWaterMarkOrder ZOrder
        {
            get
            {
                return m_zOrder; 
            }

            set
            {
                if (m_zOrder != value)
                {
                    m_zOrder = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>The margin.</value>
        [DefaultValue(typeof(ChartThickness), "10; 10; 10; 10"), Description("Indicates the margin of watermark"), NotifyParentProperty(true)]
        public ChartThickness Margin
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
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        [DefaultValue(60f), Description("Indicates the title opacity of watermark"), NotifyParentProperty(true)]
        public float Opacity
        {
            get 
            {
                return m_opacity; 
            }

            set
            {
                if (m_opacity != value)
                {
                    m_opacity = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether watermark is visible.
        /// </summary>
        /// <value>
        /// 	<c>True</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsVisible
        {
            get
            {
              return (m_image != null) || (!string.IsNullOrEmpty(m_text));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartWatermark"/> class.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        internal ChartWatermark(ChartArea chartArea)
        {
            m_area = chartArea;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws watermark by the specified graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="bounds">The bounds.</param>
        internal void Draw(ChartGraph graph, RectangleF bounds)
        {
            RectangleF imageBounds;
            RectangleF textBounds;

            this.ComputeBounds(graph, bounds, out imageBounds, out textBounds);

            if (m_image != null)
            {
                graph.DrawImage(m_image, imageBounds);
            }

            if (!string.IsNullOrEmpty(m_text))
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb((int)(c_persentToByte * m_opacity), this.TextColor)))
                {
                    graph.DrawString(m_text, this.Font, sb, textBounds);
                }
            }
        }

        /// <summary>
        /// Draws watermark by the specified G3D.
        /// </summary>
        /// <param name="g3d">The G3D.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>Returns Polygon.</returns>
        internal Polygon Draw(Graphics3D g3d, RectangleF bounds, float z)
        {
            Polygon imgPoly = null;
            Polygon textPoly = null;
            RectangleF imageBounds;
            RectangleF textBounds;

            this.ComputeBounds(new ChartGDIGraph(g3d.Graphics), bounds, out imageBounds, out textBounds);

            if (m_image != null)
            {
                imgPoly = Image3D.FromImage(m_image, imageBounds, z);
            }

            if (!string.IsNullOrEmpty(m_text))
            {
                GraphicsPath gp = new GraphicsPath();
                RenderingHelper.AddTextPath(gp, g3d.Graphics, m_text, this.Font, textBounds);

                textPoly = Path3D.FromGraphicsPath(gp, z, new SolidBrush(Color.FromArgb((int)(c_persentToByte * m_opacity), this.TextColor)));
            }

            if (imgPoly == null)
            {
                return textPoly;
            }
            else if (textPoly == null)
            {
                return imgPoly;
            }

            return new Path3DCollect(new Polygon[] { imgPoly, textPoly });
        }

        /// <summary>
        /// Computes the bounds.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="imageBounds">The image bounds.</param>
        /// <param name="textBounds">The text bounds.</param>
        private void ComputeBounds(ChartGraph graph, RectangleF bounds, out RectangleF imageBounds, out RectangleF textBounds)
        {
            bounds = m_margin.Deflate(bounds);

            SizeF resultSize = new SizeF();
            SizeF imageSize = new SizeF();

            if (m_image != null)
            {
                if (m_imageSize.HasValue)
                {
                    imageSize = m_imageSize.Value;
                }
                else
                {
                    imageSize = new SizeF(m_image.Width, m_image.Height);
                }
            }

            SizeF textSize = graph.MeasureString(m_text, this.Font, bounds.Width - imageSize.Width);

            resultSize = new SizeF(imageSize.Width + textSize.Width,
                Math.Max(imageSize.Height, textSize.Height));
            bounds = LayoutHelper.AlignRectangle(bounds, resultSize, m_horizontalAlignment, m_verticalAlignment);

            imageBounds = LayoutHelper.AlignRectangle(bounds, imageSize, ContentAlignment.MiddleLeft);
            textBounds = LayoutHelper.AlignRectangle(bounds, textSize, ContentAlignment.MiddleRight);
        }

        /// <summary>
        /// Indicates whether the font should be serialized.
        /// </summary>
        /// <returns>Indicates whether the font should be serialized or not.</returns>
        private bool ShouldSerializeFont()
        {
            return m_font != null;
        }

        /// <summary>
        /// Indicates whether the text color should be serialized.
        /// </summary>
        /// <returns>Indicates whether the text color should be serialized.</returns>
        private bool ShouldSerializeTextColor()
        {
            return !m_color.IsEmpty;
        }

        /// <summary>
        /// Resets the font.
        /// </summary>
        private void ResetFont()
        {
            this.Font = null;
        }

        /// <summary>
        /// Resets the text color.
        /// </summary>
        private void ResetTextColor()
        {
            m_color = Color.Empty;
        }

        /// <summary>
        /// Invalidates the chart.
        /// </summary>
        private void Invalidate()
        {
            if (this.IsVisible)
            {
                m_area.Redraw(true);
            }
        }
        #endregion
    }
}
