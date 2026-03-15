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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartCustomPoint class can be used to set text or symbols at a particular point on the chart area.
    /// </summary>
    [TypeConverter(typeof(ChartInstanceConverter))]
    public class ChartCustomPoint
    {
        #region Members
        private ChartCustomPointType m_type = ChartCustomPointType.ChartCoordinates;
        private double m_xvalue = 0;
        private double m_yvalue = 0;
        private string m_text = "";
        private ChartTextOrientation m_alignment = ChartTextOrientation.Center;
        private ChartSymbolInfo m_symbol = new ChartSymbolInfo();
        private bool m_drawMarker;
        private ChartFontInfo m_font = new ChartFontInfo();
        private Color m_color = SystemColors.ControlText;
        private int m_seriesIndex = -1;
        private int m_pointIndex = -1;
        private float m_offset = 0;
        private BrushInfo m_interior = new BrushInfo(Color.Transparent);
        private ChartImageCollection m_images = null;
        private DrawShape shape = null;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when settings is changed.
        /// </summary>
        public event EventHandler SettingsChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the image list to be used.
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartImageCollection Images
        {
            get
            {
                return m_images;
            }

            set
            {
                if (m_images != value)
                {
                    m_images = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the border information that is to be associated with this custom point.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
        public ChartLineInfo Border
        {
            get
            {
                return m_symbol.Border;
            }

            set
            {
                m_symbol.Border = value;
            }
        }

        /// <summary>
        /// Gets or sets the interior brush information that is to be associated with this custom point.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]    
        public BrushInfo Interior
        {
            get
            {
                return m_interior;
            }

            set
            {
                m_interior = new BrushInfo(value);
                this.OnSettingsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the offset of text that is to be associated with this point, from the rendering position of this point.
        /// </summary>
        [DefaultValue(0f), Category("Data")]
        public float Offset
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
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the point to be followed.
        /// <seealso cref="ChartCustomPointType.PointFollow"/>
        /// </summary>
        [DefaultValue(-1), Category("Data")]
        public int PointIndex
        {
            get
            {
                return m_pointIndex;
            }

            set
            {
                if (m_pointIndex != value)
                {
                    m_pointIndex = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the series that holds the point to be followed.
        /// <see cref="ChartCustomPointType.PointFollow"/>
        /// </summary>
        [DefaultValue(-1), Category("Data")]
        public int SeriesIndex
        {
            get
            {
                return m_seriesIndex;
            }

            set
            {
                if (m_seriesIndex != value)
                {
                    m_seriesIndex = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the X value of the custom point when the primary X axis of the chart is DateTime.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Data")]
        public DateTime DateXValue
        {
            get
            {
                return DateTime.FromOADate(m_xvalue);
            }

            set
            {
                this.XValue = value.ToOADate();
                this.OnSettingsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Y value of the custom point when the primary Y axis of the chart is DateTime.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Data")]
        public DateTime DateYValue
        {
            get
            {
                return DateTime.FromOADate(m_yvalue);
            }

            set
            {
                this.YValue = value.ToOADate();
                this.OnSettingsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the symbol information that is to be associated with the custom point.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]     
        public ChartSymbolInfo Symbol
        {
            get
            {
                return m_symbol;
            }

            set
            {
                if (m_symbol != value)
                {
                    if (m_symbol != null)
                    {
                        m_symbol.Changed -= new StyleChangedEventHandler(OnStyleChanged);
                    }

                    m_symbol = value;

                    if (m_symbol != null)
                    {
                        m_symbol.Changed += new StyleChangedEventHandler(OnStyleChanged);
                    }

                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether marker is visible.
        /// </summary>
        /// <value><c>true</c> if marker is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Category("Appearance")]     
        public bool ShowMarker
        {
            get 
            {
                return m_drawMarker; 
            }

            set
            {
                if (m_drawMarker != value)
                {
                    m_drawMarker = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        [DefaultValue(typeof(Color), "ControlText"), Category("Appearance")]
        public Color Color
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
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the text.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
     , Category("Appearance")]
        public ChartFontInfo Font
        {
            get
            {
                return m_font;
            }

            set
            {
                if (m_font != value)
                {
                    if (m_font != null)
                    {
                        m_font.Changed -= new StyleChangedEventHandler(OnStyleChanged);
                    }

                    m_font = value;

                    if (m_font != null)
                    {
                        m_font.Changed += new StyleChangedEventHandler(OnStyleChanged);
                    }

                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text in relation to the point.
        /// </summary>
        [DefaultValue(ChartTextOrientation.Center), Category("Appearance")]
        public ChartTextOrientation Alignment
        {
            get
            {
                return m_alignment;
            }

            set
            {
                if (m_alignment != value)
                {
                    m_alignment = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text of the custom point.
        /// </summary>
        [DefaultValue(""), Category("Appearance")]
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
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the X value of the custom point when the primary X axis of the chart is of type double.
        /// </summary>
        [DefaultValue(0d), Category("Data")]
        public double YValue
        {
            get
            {
                return m_yvalue;
            }

            set
            {
                if (m_yvalue != value)
                {
                    m_yvalue = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y value of the custom point when the primary Y axis of the chart is of type double.
        /// </summary>
        [DefaultValue(0d), Category("Data")]
        public double XValue
        {
            get
            {
                return m_xvalue;
            }

            set
            {
                if (m_xvalue != value)
                {
                    m_xvalue = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates how the XValue and YValue will be used.
        /// </summary>
        [DefaultValue(ChartCustomPointType.ChartCoordinates), Category("Data")]
        public ChartCustomPointType CustomType
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
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Assign the Shape class properties values to custom point to draw in the chartarea
        /// </summary>
        /// <param name="shape">The chart custom point shape</param>
        public DrawShape AddShape(DrawShape shape)
        {
            this.shape = shape;
            return shape;
        }
        /// <summary>
        /// Draws the specified graph.
        /// </summary>
        /// <param name="area">The chartarea</param>
        /// <param name="graph">The graph.</param>
        /// <param name="point">The point.</param>
        internal protected virtual void Draw(ChartArea area, ChartGraph graph, PointF point)
        {
            if (area.Chart.NeedRegionUpdate)
            {
                Region rgn = new Region(new RectangleF(point.X - m_symbol.Size.Width / 2, point.Y - m_symbol.Size.Height / 2, m_symbol.Size.Width, m_symbol.Size.Height));                    
                area.Chart.ChartRegions.Add(new ChartRegion(rgn, this.YValue.ToString(), "Custom point"));
            }

            using (Brush brush = new SolidBrush(m_symbol.Color))
            {
                RenderingHelper.DrawPointSymbol(graph, m_symbol.Shape, m_symbol.Marker, m_symbol.Size, m_symbol.Offset, m_symbol.ImageIndex, brush, m_symbol.Border.GdipPen, m_images, point, m_drawMarker);
            }

            if (m_text != "" || shape != null)
            {
                SizeF textSize = graph.MeasureString(m_text, m_font.GdipFont);
                if (shape == null)
                {
                point = this.GetTextPoint(point, textSize);

                using (SolidBrush sb = new SolidBrush(m_color))
                {
                    if (m_font.Orientation == 0)
                    {
                        graph.DrawString(m_text, m_font.GdipFont, sb, new RectangleF(point, textSize));
                    }
                    else
                    {
                        Matrix transform = new Matrix();
                        transform.Translate(point.X, point.Y + textSize.Height / 2);
                        transform.Rotate(m_font.Orientation);

                        graph.PushTranfsorm();
                        graph.Transform = transform;
                        graph.DrawString(m_text, m_font.GdipFont, sb, new RectangleF(-m_offset, -textSize.Height / 2f, textSize.Width, textSize.Height));
                        graph.PopTransform();
                    }
                }
                }
                else
                {
                    #region Draw custom shape around the custom points.
                    string text = (shape.Text == null || shape.Text == "") ? m_text : shape.Text;

                    if (text != null && text != "")
                    {
                        SizeF temp = Size.Empty;
                        string[] lines = Regex.Split(text, "<br/>");
                        foreach (string line in lines)
                        {
                            SizeF lineSize = graph.MeasureString(line, shape.Font.GdipFont);
                            if (temp.Width < lineSize.Width)
                            {
                                temp.Width = lineSize.Width;
                            }
                            temp.Height += lineSize.Height;
                        }
                        temp.Width += 5;
                        if (shape.Size.IsEmpty)
                            shape.Size = temp.ToSize();

                        #region Selct shape Type to draw in the chart area
                        switch (shape.Type)
                        {

                            case ChartCustomShape.Circle:

                                using (Brush brush = new SolidBrush(shape.Color))
                                {
                                    temp = SizeF.Empty;
                                    temp.Height = shape.Size.Height + shape.Size.Height / 3;
                                    temp.Width = shape.Size.Width + shape.Size.Width / 4;

                                    shape.Size = temp.ToSize();
                                    point = this.GetShapePoint(point);
                                    graph.DrawPath(brush, shape.Border.GdipPen,
                                                   ChartSymbolHelper.GetPathCircle(new RectangleF(point, shape.Size)));
                                }

                                //Draw wrapper string within the shape
                                using (Brush brush = new SolidBrush(shape.TextColor))
                                {
                                    foreach (string line in lines)
                                    {
                                        SizeF lineSize = graph.MeasureString(line, shape.Font.GdipFont);
                                        graph.DrawString(line, shape.Font.GdipFont, brush,
                                                         new RectangleF(point.X + shape.Size.Width / 8, point.Y + shape.Size.Height / 8, shape.Size.Width,
                                                                        shape.Size.Height));
                                        point.Y = point.Y + lineSize.Height;
                                    }
                                }
                                break;

                            case ChartCustomShape.Pentagon:

                                using (Brush brush = new SolidBrush(shape.Color))
                                {
                                    temp = SizeF.Empty;
                                    temp.Height = shape.Size.Height + shape.Size.Height / 2;
                                    temp.Width = shape.Size.Width + shape.Size.Width / 3;

                                    shape.Size = temp.ToSize();
                                    point = this.GetShapePoint(point);
                                    graph.DrawPath(brush, shape.Border.GdipPen,
                                                   ChartSymbolHelper.GetPathPentagon(new RectangleF(point, shape.Size)));
                                }

                                //Draw wrapper string within the shape
                                using (Brush brush = new SolidBrush(shape.TextColor))
                                {
                                    foreach (string line in lines)
                                    {
                                        SizeF lineSize = graph.MeasureString(line, shape.Font.GdipFont);
                                        graph.DrawString(line, shape.Font.GdipFont, brush,
                                                         new RectangleF(point.X + shape.Size.Width / 7, point.Y + shape.Size.Height / 8, shape.Size.Width,
                                                                        shape.Size.Height));
                                        point.Y = point.Y + lineSize.Height;
                                    }
                                }
                                break;

                            case ChartCustomShape.Hexagon:

                                using (Brush brush = new SolidBrush(shape.Color))
                                {
                                    temp = SizeF.Empty;
                                    temp.Height = shape.Size.Height + shape.Size.Height / 2;
                                    temp.Width = shape.Size.Width + shape.Size.Width / 3;

                                    shape.Size = temp.ToSize();
                                    point = this.GetShapePoint(point);
                                    graph.DrawPath(brush, shape.Border.GdipPen,
                                                   ChartSymbolHelper.GetPathHexagon(new RectangleF(point, shape.Size)));
                                }

                                //Draw wrapper string within the shape
                                using (Brush brush = new SolidBrush(shape.TextColor))
                                {
                                    foreach (string line in lines)
                                    {
                                        SizeF lineSize = graph.MeasureString(line, shape.Font.GdipFont);
                                        graph.DrawString(line, shape.Font.GdipFont, brush,
                                                         new RectangleF(point.X + shape.Size.Width / 6, point.Y + shape.Size.Height / 6, shape.Size.Width,
                                                                        shape.Size.Height));
                                        point.Y = point.Y + lineSize.Height;
                                    }
                                }
                                break;


                            case ChartCustomShape.Square:
                            default:

                                point = this.GetShapePoint(point);
                                using (Brush brush = new SolidBrush(shape.Color))
                                {
                                    graph.DrawPath(brush, shape.Border.GdipPen,
                                                   ChartSymbolHelper.GetPathSquare(new RectangleF(point, shape.Size)));
                                }

                                //Draw wrapper string within the shape
                                using (Brush brush = new SolidBrush(shape.TextColor))
                                {
                                    foreach (string line in lines)
                                    {
                                        SizeF lineSize = graph.MeasureString(line, shape.Font.GdipFont);
                                        graph.DrawString(line, shape.Font.GdipFont, brush,
                                                         new RectangleF(point.X, point.Y, shape.Size.Width,
                                                                        shape.Size.Height));
                                        point.Y = point.Y + lineSize.Height;
                                    }
                                }
                                break;
                        }
                        #endregion Switch case end
                        shape.Size = Size.Empty;
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Draws the specified graph.
        /// </summary>
        /// <param name="area">The chartarea</param>
        /// <param name="graph">The graph.</param>
        /// <param name="point">The point.</param>
        internal protected virtual void Draw(ChartArea area, Graphics3D graph, Vector3D point)
        {
            if (area.Chart.NeedRegionUpdate)
            {
                PointF pointF = area.Transform3D.ToScreen(point);
                Region rgn = new Region(new RectangleF(pointF.X - m_symbol.Size.Width / 2,pointF.Y - m_symbol.Size.Height / 2, m_symbol.Size.Width, m_symbol.Size.Height));
                area.Chart.ChartRegions.Add(new ChartRegion(rgn, this.YValue.ToString(), "Custom point"));
            }

            Rectangle rc = new Rectangle((int)(point.X - m_symbol.Size.Width / 2), (int)(point.Y - m_symbol.Size.Height / 2), m_symbol.Size.Width, m_symbol.Size.Height);
           // if (m_symbol.Shape != ChartSymbolShape.Image)
           if ((m_symbol.Shape != ChartSymbolShape.Image) && (m_symbol.Shape !=ChartSymbolShape.None))
           {
                GraphicsPath gp = ChartSymbolHelper.GetPathSymbol(m_symbol.Shape, rc);
                graph.AddPolygon(Path3D.FromGraphicsPath(gp, point.Z, new SolidBrush(m_symbol.Color), m_symbol.Border.GdipPen));
              
            }
            else if (m_symbol.ImageIndex >= 0 && m_symbol.ImageIndex < m_images.Count)
            {
                graph.AddPolygon(Image3D.FromImage(m_images[m_symbol.ImageIndex], rc, (float)point.Z));
            }

            if (m_text != "")
            {
                PointF ptF = new PointF((float)point.X, (float)point.Y);
                SizeF textSize = graph.Graphics.MeasureString(m_text, m_font.GdipFont);

                ptF = this.GetTextPoint(ptF, textSize);

                if (!ptF.IsEmpty)
                {
                    GraphicsPath tgp = new GraphicsPath();

                    if (m_font.Orientation == 0)
                    {
                        RectangleF r = new RectangleF(ptF, textSize);
                        tgp.AddString(m_text, m_font.GdipFont.FontFamily, (int)m_font.GdipFont.Style, RenderingHelper.GetFontSizeInPixels(m_font.GdipFont), r, StringFormat.GenericDefault);                            
                    }
                    else
                    {
                        Matrix mtr = new Matrix();
                        mtr.Translate(ptF.X + textSize.Width / 2, ptF.Y + textSize.Height / 2f);
                        mtr.Rotate(m_font.Orientation, MatrixOrder.Prepend);
                        RectangleF r = new RectangleF(new PointF(m_offset, -textSize.Height / 2f), textSize);
                        tgp.AddString(m_text, m_font.GdipFont.FontFamily, (int)m_font.GdipFont.Style, RenderingHelper.GetFontSizeInPixels(m_font.GdipFont), r, StringFormat.GenericDefault);
                        tgp.Transform(mtr);
                    }

                    graph.AddPolygon(Path3D.FromGraphicsPath(tgp, point.Z - 1, new SolidBrush(m_color)));
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SettingsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (SettingsChanged != null)
            {
                this.SettingsChanged(this, e);
            }
        }

        /// <summary>
        /// Called when border is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Styles.StyleChangedEventArgs"/> instance containing the event data.</param>
        private void OnStyleChanged(object sender, StyleChangedEventArgs e)
        {
            this.OnSettingsChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets the text point.
        /// </summary>
        /// <param name="basePoint">The base point.</param>
        /// <param name="textSize">Size of the text.</param>
        /// <returns>Returns the Text point.</returns>
        private PointF GetTextPoint(PointF basePoint, SizeF textSize)
        {
            PointF pt = basePoint;

            #region Orientation cases
            switch (m_alignment)
            {
                case ChartTextOrientation.Up:
                    pt.X -= textSize.Width / 2;
                    pt.Y -= textSize.Height;
                    break;

                case ChartTextOrientation.Down:
                    pt.X -= textSize.Width / 2;
                    break;

                case ChartTextOrientation.Left:
                    pt.X -= textSize.Width;
                    pt.Y -= textSize.Height / 2;
                    break;

                case ChartTextOrientation.Right:
                    pt.Y -= textSize.Height / 2;
                    break;

                case ChartTextOrientation.UpLeft:
                    pt.X -= textSize.Width;
                    pt.Y -= textSize.Height;
                    break;

                case ChartTextOrientation.DownLeft:
                    pt.X -= textSize.Width;
                    break;

                case ChartTextOrientation.UpRight:
                    pt.Y -= textSize.Height;
                    break;

                case ChartTextOrientation.Center:
                    pt.X -= textSize.Width / 2;
                    pt.Y -= textSize.Height / 2;
                    break;

                case ChartTextOrientation.Smart:
                    pt.X -= textSize.Width / 2;
                    pt.Y -= textSize.Height / 2;
                    break;

                case ChartTextOrientation.RegionUp:
                    pt.X -= textSize.Width / 2;
                    pt.Y -= m_symbol.Size.Height / 2 + textSize.Height;
                    break;

                case ChartTextOrientation.RegionDown:
                    pt.X -= textSize.Width / 2;
                    pt.Y += m_symbol.Size.Height / 2;
                    break;

                case ChartTextOrientation.RegionCenter:
                case ChartTextOrientation.SymbolCenter:
                    pt.X -= textSize.Width / 2;
                    pt.Y -= textSize.Height / 2;
                    break;
            }
            #endregion

            return pt;
        }
        /// <summary>
        /// Gets the Rectangle point.
        /// </summary>
        /// <param name="basePoint">The base point.</param>
        /// <param name="textSize">Size of the text.</param>
        /// <returns>Returns the Rectangle point.</returns>
        /// 
        private PointF GetShapePoint(PointF basePoint)
        {
            PointF pt = basePoint;

            #region Orientation cases
            switch (shape.Position)
            {
                case ChartTextOrientation.Up:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y -= (shape.Size.Height + m_symbol.Size.Height);
                    break;

                case ChartTextOrientation.Down:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y += m_symbol.Size.Height;
                    break;

                case ChartTextOrientation.Left:
                    pt.X -= (shape.Size.Width + m_symbol.Size.Width);
                    pt.Y -= shape.Size.Height / 2;
                    break;

                case ChartTextOrientation.Right:
                    pt.X += m_symbol.Size.Width;
                    pt.Y -= shape.Size.Height / 2;
                    break;

                case ChartTextOrientation.UpLeft:
                    pt.X -= shape.Size.Width;
                    pt.Y -= (shape.Size.Height + m_symbol.Size.Height);
                    break;

                case ChartTextOrientation.UpRight:
                    pt.Y -= (shape.Size.Height + m_symbol.Size.Height);
                    break;

                case ChartTextOrientation.DownLeft:
                    pt.X -= shape.Size.Width;
                    pt.Y += m_symbol.Size.Height;
                    break;

                case ChartTextOrientation.DownRight:
                    pt.Y += m_symbol.Size.Height;
                    break;

                case ChartTextOrientation.Center:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y -= shape.Size.Height / 2;
                    break;

                case ChartTextOrientation.Smart:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y -= shape.Size.Height / 2;
                    break;

                case ChartTextOrientation.RegionUp:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y -= (shape.Size.Height + m_symbol.Size.Height);
                    break;

                case ChartTextOrientation.RegionDown:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y += m_symbol.Size.Height;
                    break;

                case ChartTextOrientation.RegionCenter:
                case ChartTextOrientation.SymbolCenter:
                    pt.X -= shape.Size.Width / 2;
                    pt.Y -= shape.Size.Height / 2;
                    break;
            }
            #endregion

            return pt;
        }
        #endregion
    }
    /// <summary>
    /// The DrawShape class can be used to set shape to particular custom point on the chart area.
    /// </summary>
    public class DrawShape
    {
        #region memeber
        private Size m_size = Size.Empty;
        private Color m_color = Color.White;
        private Color m_textColor = Color.Black;
        private string m_text = "";
        private ChartFontInfo m_font = new ChartFontInfo();
        private ChartLineInfo m_border = new ChartLineInfo();
        private ChartTextOrientation m_position = ChartTextOrientation.Up;
        private ChartCustomShape m_type = ChartCustomShape.Square;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the font of the text.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
     , Category("Appearance")]
        public ChartFontInfo Font
        {
            get { return m_font; }

            set { m_font = value; }
        }

        /// <summary>
        /// Gets or sets the style of the shape that is to be displayed.
        /// Default shape is Square. 
        /// It will support the limitted shape(Square, Circle, Hexagon, Pentagon) draw around the custom point
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DefaultValue("Square"), Description("The style of the shape to be displayed. It will support the limitted shape(Square, Circle, Hexagon, Pentagon) draw around the custom point")
        ]
        public ChartCustomShape Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
        /// <summary>
        /// Gets or sets the text of the custom point.
        /// </summary>
        [DefaultValue(""), Category("Appearance")]
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
                }
            }
        }

        ///<summary>
        /// Gets or sets background color to the Shape. 
        /// Default color is "White"
        /// </summary>
        [DefaultValue(typeof(Color), "Shape Background color"), Category("Appearance")]
        public Color Color
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
                }
            }
        }

        ///<summary>
        /// Gets or sets text color to the custom shape. 
        /// Default color is "White"
        /// </summary>
        [DefaultValue(typeof(Color), "Shape text color"), Category("Appearance")]
        public Color TextColor
        {
            get
            {
                return m_textColor;
            }

            set
            {
                if (m_textColor != value)
                {
                    m_textColor = value;
                }
            }
        }

        ///<summary>
        /// Gets or sets size to the Shape to draw around the custom points
        /// Default Size is (50, 50)
        /// </summary>
        [DefaultValue(typeof(Size), "Set shape size"), Category("Appearance")]
        public Size Size
        {
            get
            {
                return m_size;
            }

            set
            {
                if (m_size != value)
                {
                    m_size = value;
                }
            }
        }

        ///<summary>
        /// Gets or sets border to the custom shape.
        /// </summary>
        [DefaultValue(typeof(ChartLineInfo), "Draw border of shape"), Category("Appearance")]
        public ChartLineInfo Border
        {
            get { return m_border; }
            set
            {
                if (m_border != value)
                    m_border = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the shape in relation to the point.
        /// </summary>
        [DefaultValue(ChartTextOrientation.Up), Category("Appearance")]
        public ChartTextOrientation Position
        {
            get
            {
                return m_position;
            }

            set
            {
                if (m_position != value)
                {
                    m_position = value;
                }
            }
        }
        #endregion
    }
}