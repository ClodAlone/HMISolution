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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Provides methods to draw the primitives of the <see cref="PostScriptImage"/>.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PostScriptGraphics
    {
        #region members
        private TextWriter m_writer;
        private CultureInfo m_cultureInfo = CultureInfo.InvariantCulture;
        private PostScriptImage m_image;
		private ColorBlend colorBlend = new ColorBlend();
        private Color[] colors;
        private float[] positions;
        private static bool m_stripLine = false;
        private static bool fillPath = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PostScriptGraphics"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/>.</param>
        /// <param name="image">The <see cref="PostScriptImage"/>.</param>
        public PostScriptGraphics(TextWriter textWriter, PostScriptImage image)
        {
            m_writer = textWriter;
            m_image = image;

            ////making coordinate system the same like in GDI
            m_writer.WriteLine(" 1 -1 scale ");
            m_writer.WriteLine(" 0 -" + m_image.Size.Height.ToString(m_cultureInfo) + " translate ");
        }
        #endregion

        /// <summary>
        /// Indicates whether the strip line is enabled.
        /// </summary>
        internal static bool StripLine
        {
            get
            {
                return m_stripLine;
            }
            set
            {
                m_stripLine = value;
            }
        }
        #region Public methods
        /// <summary>
        /// Draws the <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            fillPath = false;
		if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            SaveGraphicState();
            SetColor(pen.Color);
            string pathStr = GetPathStringFromPath(path);
            m_writer.WriteLine(pathStr);            
            m_writer.Flush();
            RestoreGraphicState();
			}
        }

        /// <summary>
        /// Fills the <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="path">The path.</param>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            SaveGraphicState();
            string sdn = SetBrushColor(brush);            
            if (sdn != null)
            {
                string pathStr = GetPathStringFromPath(path);
                m_writer.WriteLine(pathStr);
                if (brush is LinearGradientBrush)
                {
					phongShadingColors(Color.FromArgb(0x90, Color.Black), Color.FromArgb(0x90, Color.Black),
                       Color.FromArgb(100, Color.White), Math.PI / 4, 30, out colors, out positions);
                    colorBlend.Positions = positions;
                    colorBlend.Colors = colors;
                    LinearGradientBrush lgbr = (brush as LinearGradientBrush);
                    ColorBlend cb = lgbr.InterpolationColors;
                    if (!phongColors(cb, colorBlend))
                    {
                        m_writer.WriteLine(" clip ");
                        m_writer.WriteLine(sdn + " shfill ");
                    }
                }
                else
                {
                    m_writer.WriteLine(" clip ");
                    m_writer.WriteLine(sdn + " shfill ");
                }
            }
            else
            {
                fillPath = true;
                string pathStr = GetPathStringFromPath(path);
                m_writer.WriteLine(pathStr);
                fillPath = false;
            }
            m_writer.Flush();
            RestoreGraphicState();
        }

        /// <summary>
        /// Draws the polygon.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        public void DrawPolygon(Pen pen, PointF[] path)
        {
		if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            SaveGraphicState();
            SetColor(pen.Color);
            string pathStr = GetPathStringFromArray(path);
            m_writer.WriteLine(pathStr + " stroke ");
            m_writer.Flush();
            RestoreGraphicState();
			}
        }

        /// <summary>
        /// Fills the polygon.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="path">The path.</param>
        public void FillPolygon(Brush brush, PointF[] path)
        {
            SaveGraphicState();
            string pathStr = GetPathStringFromArray(path);
            m_writer.WriteLine(pathStr);
            string sdn = SetBrushColor(brush);
            if (sdn != null)
            {
                m_writer.WriteLine(" clip ");
                m_writer.WriteLine(sdn + " shfill ");
            }
            else
            {
                m_writer.WriteLine(" fill ");
            }

            m_writer.Flush();
            RestoreGraphicState();
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
		if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            SaveGraphicState();
            SetColor(pen.Color);
            m_writer.WriteLine(GetPathStringFromRectangle(x, y, width, height) + " stroke ");
            m_writer.Flush();
            RestoreGraphicState();
			}
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rect">The rect.</param>
        public void DrawRectangle(Pen pen, Rectangle rect)
        {
		if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
		   }
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
                SaveGraphicState();
                m_writer.WriteLine(GetPathStringFromRectangle(x, y, width, height));
                string sdn = SetBrushColor(brush);
                if (sdn != null)
                {
                    m_writer.WriteLine(" clip ");
                    m_writer.WriteLine(sdn + " shfill ");
                }
                else
                {
                    if ((brush as SolidBrush).Color != Color.Transparent)
                        m_writer.WriteLine(" fill ");
                }

                m_writer.Flush();
                RestoreGraphicState();
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rect.</param>
        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rect.</param>
        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
         if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            SaveGraphicState();
            SetColor(pen.Color);
            m_writer.WriteLine(GetPathStringFromLine(x1, y1, x2, y2) + pen.Width + " setlinewidth" + " stroke ");
          
            RestoreGraphicState();
           }
        }

        /// <summary>
        /// Draws the bezier curve.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        /// <param name="x4">The x4.</param>
        /// <param name="y4">The y4.</param>
        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
		if (pen.Color.ToArgb() !=Color.Transparent.ToArgb())
           {
            SaveGraphicState();
            SetColor(pen.Color);
            m_writer.WriteLine(GetPathStringFromBezier(x1, y1, x2, y2, x3, y3, x4, y4) + " stroke ");
            RestoreGraphicState();
		   }
        }

        /// <summary>
        /// Translates the transform.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public void TranslateTransform(Single dx, Single dy)
        {
            m_writer.WriteLine(GetMatrixStringFromTranslate(dx, dy) + " concat");
            m_writer.Flush();
        }
        /// <summary>
        /// Rotate the transform.
        /// </summary>
        /// <param name="dx">The dx.</param>
        public void RotateTransform(Single dx)
        {
            m_writer.WriteLine(dx.ToString(m_cultureInfo) + " " + " rotate ");
            m_writer.Flush();
        }
        /// <summary>
        /// Translates the transform.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="order">The order.</param>
        public void TranslateTransform(Single dx, Single dy, MatrixOrder order)
        {
        }

        /// <summary>
        /// Clears the image by the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Clear(Color color)
        {
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="stringformat"></param>
        /// <param name="editableText"></param>
        public void DrawString(string s, Font font, Brush brush, float x, float y,float width, float height,StringFormat stringformat,bool editableText)
        {
             if (editableText)
            {
                if (s != null)
                {
                    SaveGraphicState();
                    SetBrushColor(brush);
                    m_writer.WriteLine("1 -1 scale");
                 
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddString(s, font.FontFamily, (int)font.Style, RenderingHelper.GetFontSizeInPixels(font), new RectangleF(x, y, width, height), stringformat);

                    RectangleF r = gp.GetBounds();
                    float deltaY = font.Size * (font.FontFamily.GetLineSpacing(font.Style) - font.FontFamily.GetCellAscent(font.Style)) / font.FontFamily.GetEmHeight(font.Style);
                    float deltaX = font.Size * 0.3f; // gdi doesn' allow to get character spacing, so we try to guess it
                    TranslateTransform((Single)(x - r.Left + deltaX), (Single)(y - r.Top + deltaY));

                    m_writer.WriteLine(r.X + " " + -(r.Y +(3.5*deltaY) )+ " " + "moveto");
                    m_writer.WriteLine("/" + font.FontFamily.Name.ToString().Replace(" ", "") + "-" + font.Style.ToString() + " " + "findfont" + " " + RenderingHelper.GetFontSizeInPixels(font) + " " + "scalefont setfont");

                    m_writer.WriteLine("(" + s + ")" + " " + "show");
                    RestoreGraphicState();
                }
            }
            else
			{
                     
            SetBrushColor(brush);
            GraphicsPath gp = new GraphicsPath();
            gp.AddString(s, font.FontFamily, (int)font.Style, RenderingHelper.GetFontSizeInPixels(font), new RectangleF(x, y, width,height), stringformat);
            
            RectangleF r = gp.GetBounds();
            float deltaY = font.Size * (font.FontFamily.GetLineSpacing(font.Style) - font.FontFamily.GetCellAscent(font.Style)) / font.FontFamily.GetEmHeight(font.Style);
            float deltaX = font.Size * 0.3f; // gdi doesn' allow to get character spacing, so we try to guess it
            if (!StripLine)
            {
                SaveGraphicState();
                TranslateTransform((Single)(x - r.Left + deltaX), (Single)(y - r.Top + deltaY));            
            }
            StripLine = false;

            string pathStr = GetPathStringFromPath(gp);
            m_writer.WriteLine(pathStr);
            m_writer.WriteLine(" fill ");
            m_writer.Flush();
            RestoreGraphicState();
			}
        }

        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void SetClip(RectangleF rect)
        {
            if (rect.Width < 4000000 && rect.Height < 4000000)
            {
                ////writer.WriteLine( " clipsave " + GetPathStringFromRectangle( rect ) + " clip " );
                m_writer.WriteLine(" initclip " + GetPathStringFromRectangle(rect) + " clip ");
            }

            m_writer.Flush();
        }

        /// <summary>
        /// Intersects the clip.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void IntersectClip(RectangleF rect)
        {
            if (rect.Width < 4000000 && rect.Height < 4000000)
                m_writer.WriteLine(GetPathStringFromRectangle(rect) + " clip ");
            m_writer.Flush();
        }

        /// <summary>
        /// Resets the clip.
        /// </summary>
        public void ResetClip()
        {
            ////writer.WriteLine( " cliprestore " );
            m_writer.WriteLine(" initclip ");
        }

        /// <summary>
        /// Saves the image to file by the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Save(string name)
        {
        }

        /// <summary>
        /// Saves the correct states.
        /// </summary>
        public void BeginContainer()
        {
            SaveGraphicState();
        }

        /// <summary>
        /// Reverts the saved states.
        /// </summary>
        public void EndContainer()
        {
            RestoreGraphicState();
        }

        /// <summary>
        /// Concats the transform.
        /// </summary>
        /// <param name="m">The m.</param>
        public void ConcatTransform(Matrix m)
        {
            if (StripLine)
            {
                SaveGraphicState();
            }
            m_writer.WriteLine(GetMatrixStringFromMatrix(m) + " concat");
            m_writer.Flush();
        }

        /// <summary>
        /// Sets the transform.
        /// </summary>
        /// <param name="m">The <see cref="Matrix"/>.</param>
        public void SetTransform(Matrix m)
        {
            m_writer.WriteLine(GetMatrixStringFromMatrix(m) + " setmatrix");
            m_writer.Flush();
        }

        #endregion

        #region helping methods

        /// <summary>
        /// Gets the path string from path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns string.</returns>
        private string GetPathStringFromPath(GraphicsPath path)
        {
            PathData pd = path.PathData;
            StringBuilder sb = new StringBuilder(100);
            bool closed = false;

            sb.Append(" newpath ");
            for (int i = 0, length = pd.Types.Length; i < length; i++)
            {
                byte type = pd.Types[i];
                if ((type & (byte)PathPointType.Bezier) == (byte)PathPointType.Bezier)
                {
                    for (int j = 0; (j < 3) && (i + j) < length; j++)
                    {
                        sb.Append(pd.Points[i + j].X.ToString(m_cultureInfo));
                        sb.Append(" ");
                        sb.Append(pd.Points[i + j].Y.ToString(m_cultureInfo));
                        sb.Append(" ");
                    }

                    sb.Append(" curveto\n");
                    i += 2;
                    type = pd.Types[i];
                    closed = false;
                }
                else

                    if ((type & (byte)PathPointType.Line) == (byte)PathPointType.Line)
                    {
                        sb.Append(" ");
                        sb.Append(pd.Points[i].X.ToString(m_cultureInfo));
                        sb.Append(" ");
                        sb.Append(pd.Points[i].Y.ToString(m_cultureInfo));
                        sb.Append(" ");
                        sb.Append("lineto\n");
                        sb.Append(" ");
                        closed = false;
                    }
                    else

                        if (type == (byte)PathPointType.Start)
                        {
                            if ((i > 0) && !closed)
                            {
                                sb.Append(" closepath ");
                                if (fillPath)
                                    sb.Append(" fill ");
                                else
                                    sb.Append(" stroke ");
                                closed = true;
                            }

                            sb.Append(pd.Points[i].X.ToString(m_cultureInfo));
                            sb.Append(" ");
                            sb.Append(pd.Points[i].Y.ToString(m_cultureInfo));
                            sb.Append(" ");
                            sb.Append("moveto ");
                            sb.Append(" ");
                            closed = false;
                        }
                
                if ((type & (byte)PathPointType.CloseSubpath) != 0)
                {
                    if (!closed)
                    {
                        sb.Append(" closepath ");
                        if (fillPath)
                            sb.Append(" fill ");
                        else
                            sb.Append(" stroke ");
                        closed = true;
                    }
                }
            }
            if (!closed)
            {
                if (fillPath)
                    sb.Append(" fill ");
                else
                    sb.Append(" stroke ");
                closed = true;
            }
            return sb.ToString();

        }

        private string GetPathStringFromArray(PointF[] path)
        {
            StringBuilder sb = new StringBuilder(100);

            int length = path.Length;

            if (length < 1) return " newpath closepath";

            sb.Append("newpath ");
            sb.Append(path[0].X.ToString(m_cultureInfo));
            sb.Append(" ");
            sb.Append(path[0].Y.ToString(m_cultureInfo));
            sb.Append(" ");
            sb.Append("moveto ");

            for (int i = 1; i < length; i++)
            {
                sb.Append(path[i].X.ToString(m_cultureInfo));
                sb.Append(" ");
                sb.Append(path[i].Y.ToString(m_cultureInfo));
                sb.Append(" ");
                sb.Append("lineto ");
            }

            sb.Append(" closepath ");

            return sb.ToString();
        }

        /// <summary>
        /// Gets the path string from line.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>Returns path string from line.</returns>
        private string GetPathStringFromLine(float x1, float y1, float x2, float y2)
        {
            return (" newpath " +
                    x1.ToString(m_cultureInfo) + " " +
                    y1.ToString(m_cultureInfo) + " " +
                    "moveto " +
                    x2.ToString(m_cultureInfo) + " " +
                    y2.ToString(m_cultureInfo) +
                    " lineto closepath ");
        }

        /// <summary>
        /// Gets the path string from bezier.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        /// <param name="x4">The x4.</param>
        /// <param name="y4">The y4.</param>
        /// <returns>Path string from bezier.</returns>
        private string GetPathStringFromBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            return (" newpath " +
                x1.ToString(m_cultureInfo) + " " +
                y1.ToString(m_cultureInfo) + " " +
                "moveto " +
                x2.ToString(m_cultureInfo) + " " +
                y2.ToString(m_cultureInfo) + " " +
                x3.ToString(m_cultureInfo) + " " +
                y3.ToString(m_cultureInfo) + " " +
                x4.ToString(m_cultureInfo) + " " +
                y4.ToString(m_cultureInfo) +
                " curveto ");
        }

        /// <summary>
        /// Gets the path string from rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Path string from rectangle.</returns>
        private string GetPathStringFromRectangle(float x, float y, float width, float height)
        {
            return (" newpath " +
                    x.ToString(m_cultureInfo) + " " +
                    y.ToString(m_cultureInfo) +
                    " moveto " +
                    (x + width).ToString(m_cultureInfo) + " " +
                    y.ToString(m_cultureInfo) + " " +
                    "lineto " +
                    (x + width).ToString(m_cultureInfo) + " " +
                    (y + height).ToString(m_cultureInfo) +
                    " lineto " +
                    x.ToString(m_cultureInfo) + " " +
                    (y + height).ToString(m_cultureInfo) + " " +
                    "lineto closepath ");
        }

        /// <summary>
        /// Gets the path string from rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Path string from rectangle.</returns>
        private string GetPathStringFromRectangle(RectangleF rect)
        {
            return GetPathStringFromRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Saves the state of the graphic.
        /// </summary>
        private void SaveGraphicState()
        {
            m_writer.WriteLine(" gsave ");
            m_writer.Flush();
        }

        /// <summary>
        /// Restores the state of the graphic.
        /// </summary>
        private void RestoreGraphicState()
        {
            m_writer.WriteLine(" grestore ");
            m_writer.Flush();
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="color">The color.</param>
        private void SetColor(Color color)
        {
            m_writer.WriteLine(GetStringFromColor(color) + " setrgbcolor ");
            m_writer.Flush();
        }

        /// <summary>
        /// Gets the color of the string from.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>Returns string from color.</returns>
        private string GetStringFromColor(Color color)
        {
            string r = ((float)(color.R / 255.0f)).ToString(m_cultureInfo).Replace(",", ".");
            string g = ((float)(color.G / 255.0f)).ToString(m_cultureInfo).Replace(",", ".");
            string b = ((float)(color.B / 255.0f)).ToString(m_cultureInfo).Replace(",", ".");
            return (r + " " + g + " " + b + " ");
        }

        /// <summary>
        /// Sets the color of the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>Returns string.</returns>
        private string SetBrushColor(Brush brush)
        {
            if (brush is LinearGradientBrush)
            {
                LinearGradientBrush lgbr = (brush as LinearGradientBrush);

                RectangleF rc = lgbr.Rectangle;

                ColorBlend cb = null;
                try
                {
                    cb = lgbr.InterpolationColors;
                }
                catch (Exception)
                {
                }

                PointF p1 = new PointF(lgbr.Rectangle.X, lgbr.Rectangle.Y);
                PointF p2 = new PointF(lgbr.Rectangle.Right, lgbr.Rectangle.Top);
                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(p1, p2);
                gp.Transform(lgbr.Transform);

                if (cb != null)
                {
                    if (cb.Colors.Length > 1)
                    {
                        return SetLinearGradientColor(cb.Colors, gp.PathPoints[0], gp.PathPoints[1]);
                    }
                }
                else
                {
                    return SetLinearGradientColor(lgbr.LinearColors[0], lgbr.LinearColors[1], gp.PathPoints[0], gp.PathPoints[1]);
                }
            }
            else
                if (brush is PathGradientBrush)
                {
                    PathGradientBrush pgbr = (brush as PathGradientBrush);

                    RectangleF rc = pgbr.Rectangle;

                    PointF p1 = new PointF((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2);
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddLine(p1, p1);
                    gp.Transform(pgbr.Transform);
                    float r = (rc.Width + rc.Height) / 4;

                    return SetRadialGradientColor(pgbr.CenterColor, pgbr.SurroundColors[0], gp.PathPoints[0], 0, gp.PathPoints[1], r);
                }
                else
                    if (brush is SolidBrush)
                    {
                        SetColor((brush as SolidBrush).Color);
                    }
                    else
                    {
                        SetColor(Color.White);
                    }

            return null;
        }

        /// <summary>
        /// Gets the matrix string from matrix.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns>Returns Matrix string from Matrix.</returns>
        private string GetMatrixStringFromMatrix(Matrix m)
        {
            return ("[ " + m.Elements[0].ToString(m_cultureInfo) + " "
                             + m.Elements[1].ToString(m_cultureInfo) + " "
                             + m.Elements[2].ToString(m_cultureInfo) + " "
                             + m.Elements[3].ToString(m_cultureInfo) + " "
                             + m.Elements[4].ToString(m_cultureInfo) + " "
                             + m.Elements[5].ToString(m_cultureInfo) + " ]");
        }

        /// <summary>
        /// Gets the matrix string from translate.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        /// <returns>Returns the matrix string from translate.</returns>
        private string GetMatrixStringFromTranslate(Single dx, Single dy)
        {
            return ("[ " + "1" + " "
                + "0" + " "
                + "0" + " "
                + "1" + " "
                + dx.ToString(m_cultureInfo) + " "
                + dy.ToString(m_cultureInfo) + " ]");
        }

        /// <summary>
        /// Sets the font.
        /// </summary>
        /// <param name="font">The font.</param>
        private void SetFont(Font font)
        {
            Font tf = new Font(font.FontFamily, font.Size, font.Style, GraphicsUnit.Pixel);
            float factor = (float)tf.Size / tf.SizeInPoints;
            float h = font.SizeInPoints * factor;
            m_writer.WriteLine("/" + font.Name.Replace(" ", "") + " findfont " +
                                                h.ToString(m_cultureInfo) +
                                                " scalefont" +
                                                " setfont ");
            m_writer.Flush();
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void MoveTo(float x, float y)
        {
            m_writer.WriteLine(x.ToString(m_cultureInfo) + " " + y.ToString(m_cultureInfo) + " moveto");
            m_writer.Flush();
        }

        /// <summary>
        /// Sets the color of the linear gradient.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <param name="rect">The rect.</param>
        /// <returns>Returns string.</returns>
        private string SetLinearGradientColor(Color c1, Color c2, RectangleF rect)
        {
            return SetLinearGradientColor(c1, c2, new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Top));
        }

        /// <summary>
        /// Sets the color of the linear gradient.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>Returns LinearShadingDictionary name.</returns>
        private string SetLinearGradientColor(Color c1, Color c2, PointF p1, PointF p2)
        {
            LinearColorFunction scf = new LinearColorFunction(c1, c2);
            scf = (LinearColorFunction)m_image.Dictionaries.Add(scf);

            LinearShadingDictionary lsd = new LinearShadingDictionary(p1, p2, scf.Name);
            lsd = (LinearShadingDictionary)m_image.Dictionaries.Add(lsd);
            m_writer.Flush();
            return lsd.Name;
        }

        /// <summary>
        /// Sets the color of the linear gradient.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>Returns string.</returns>
        private string SetLinearGradientColor(Color[] colors, PointF p1, PointF p2)
        {
            if (colors.Length > 2)
            {
                StitchingColorFunction scf = new StitchingColorFunction(colors);
                scf = (StitchingColorFunction)m_image.Dictionaries.Add(scf);

                LinearShadingDictionary lsd = new LinearShadingDictionary(p1, p2, scf.Name);
                lsd = (LinearShadingDictionary)m_image.Dictionaries.Add(lsd);
                m_writer.Flush();
                return lsd.Name;
            }
            else
            {
                return SetLinearGradientColor(colors[0], colors[1], p1, p2);
            }
        }

        /// <summary>
        /// Sets the color of the radial gradient.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="r1">The r1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="r2">The r2.</param>
        /// <returns> Returns RadialShadingDictionary name.</returns>
        private string SetRadialGradientColor(Color c1, Color c2, PointF p1, float r1, PointF p2, float r2)
        {
            LinearColorFunction lcf = new LinearColorFunction(c1, c2);
            lcf = (LinearColorFunction)m_image.Dictionaries.Add(lcf);
            RadialShadingDictionary rsd = new RadialShadingDictionary(p1, r1, p2, r2, lcf.Name);
            rsd = (RadialShadingDictionary)m_image.Dictionaries.Add(rsd);
            m_writer.Flush();
            return rsd.Name;
        }

		 /// <summary>
        /// Gets the phong shading blend.
        /// </summary>
        /// <param name="ambientColor">Color of the ambient.</param>
        /// <param name="diffusiveColor">Color of the diffusive.</param>
        /// <param name="lightColor">Color of the light.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="phong_alpha">The phong_alpha.</param>
        /// <param name="colors">The colors.</param>
        /// <param name="positions">The positions.</param>
        private static void phongShadingColors(Color ambientColor, Color diffusiveColor, Color lightColor, double alpha, double phong_alpha, out Color[] colors, out float[] positions)
        {
            double R = 0.5;

            int colornum = 10;
            Color i_a = ambientColor;
            Color i_d = diffusiveColor;
            Color i_s = lightColor;

            double ka = 0.45f;
            double kd = 0.55f;
            double ks = 0.9f;
            double dpos = 1.0 / (colornum - 1);

            positions = new float[colornum];
            colors = new Color[colornum];
            for (int i = 0; i < colornum; i++)
            {
                double pos = i * dpos;
                double beta = Math.Asin((pos - R) / R);
                double x = R + R * Math.Sin(beta);

                double r = 0, g = 0, b = 0;

                r = ka * i_a.R;
                g = ka * i_a.G;
                b = ka * i_a.B;

                double t = Math.Max(Math.Cos(alpha + beta), 0);
                r += kd * i_d.R * t;
                g += kd * i_d.G * t;
                b += kd * i_d.B * t;

                t = Math.Pow(Math.Abs(Math.Cos(alpha + 2 * beta)), phong_alpha);
                if (Math.Abs(alpha + 2 * beta) > Math.PI / 2) t = 0;
                r += ks * i_s.R * t;
                g += ks * i_s.G * t;
                b += ks * i_s.B * t;

                r = Math.Min(255, r);
                g = Math.Min(255, g);
                b = Math.Min(255, b);

                positions[i] = (float)(pos);
                colors[i] = Color.FromArgb(ambientColor.A, (int)r, (int)g, (int)b);
            }
            positions[colornum - 1] = 1;
        }
        private bool phongColors(ColorBlend c1, ColorBlend c2)
        {
            bool res = false;
            if (c1.Colors.Length == c2.Colors.Length)
            {
                for (int i = 0; i < c1.Colors.Length; i++)
                {
                    if (c1.Colors[i] == c2.Colors[i] && c1.Positions[i] == c2.Positions[i])
                        res = true;
                    else
                    {
                        res = false;
                        break;
                    }
                }
            }
            return res;
        }
        #endregion
    }
}
