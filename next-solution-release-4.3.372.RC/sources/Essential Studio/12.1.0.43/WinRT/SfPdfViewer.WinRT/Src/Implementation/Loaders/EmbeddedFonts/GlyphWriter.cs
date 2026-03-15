#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.DirectXWrapper.WinRT;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.Pdf
{
    internal class GlyphWriter
    {
        #region Fields
        /// <summary>
        /// Represents the graphics object
        /// </summary>
        internal Graphics2D m_grpx;
        /// <summary>
        ///  Variable to hold the glyph string and its glyph shapes 
        /// </summary>
        internal Dictionary<string, byte[]> glyphs = new Dictionary<string, byte[]>();
        /// <summary>
        /// Represents the character set
        /// </summary>
        private string m_charSet = "0123456789.ee -";
        /// <summary>
        /// Represents the Type1 custom font
        /// </summary>
        private bool is1C = false;
        /// <summary>
        /// Represents the glyph operands
        /// </summary>
        private double[] m_operands = new double[100];
        /// <summary>
        /// Represents the intialise the operand
        /// </summary>
        private int m_operandReached = 0;
        /// <summary>
        /// Represents the glyphs points
        /// </summary>
        private float[] m_point;
        /// <summary>
        /// Represents the co-ordinates
        /// </summary>
        private double xs = -1, ys = -1, x = 0, y = 0;
        /// <summary>
        /// Represents the glyph points count
        /// </summary>
        private int m_pointCount = 0;
        /// <summary>
        /// Represents the current operand value
        /// </summary>
        int m_currentOperand = 0;
        /// <summary>
        /// Represents the hint count
        /// </summary>
        private int m_hintCount = 0;
        /// <summary>
        /// Represents the allow all operands
        /// </summary>
        private bool m_allowAll = false;
        /// <summary>
        /// Represents the height
        /// </summary>
        private double m_height;
        /// <summary>
        /// Represents the path geometry
        /// </summary>
        private PathGeometry m_path;
        /// <summary>
        /// Represents the collection rendered path
        /// </summary>
        List<PathRenderer> m_pathRenderList = new List<PathRenderer>();
        /// <summary>
        /// Represents the geometry path
        /// </summary>
        private GeometrySink m_geoPath;
        /// <summary>
        /// Represents the matrix path
        /// </summary>
        List<Syncfusion.DirectXWrapper.WinRT.Matrix> m_pathMatrix = new List<Syncfusion.DirectXWrapper.WinRT.Matrix>();
        /// <summary>
        /// Indicates whether path sink closed.
        /// </summary>
        private bool IsPathSinkClosed = false;
        /// <summary>
        /// Represents the mitter length
        /// </summary>
        private float m_mitterLength;
        /// <summary>
        /// Represents the clip rectangle
        /// </summary>
        private System.Drawing.RectangleF m_clipRectangle;
        /// <summary>
        /// Represents the current location
        /// </summary>
        private System.Drawing.PointF m_currentLocation = System.Drawing.PointF.Empty;
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the mitter length.
        /// </summary>
        private float MitterLength
        {
            get
            {
                return m_mitterLength;
            }
            set
            {
                m_mitterLength = value;
            }

        }

        /// <summary>
        /// Gets or sets the clip rectangle
        /// </summary>
        private System.Drawing.RectangleF ClipRectangle
        {
            get
            {
                return m_clipRectangle;
            }
            set
            {
                m_clipRectangle = value;
            }
        }

        /// <summary>
        /// Gets or sets the current location
        /// </summary>
        private System.Drawing.PointF CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets geometry path sink
        /// </summary>
        private GeometrySink PathSink
        {
            get
            {
                return m_geoPath;
            }
            set
            {
                m_geoPath = value;
            }
        }
        #endregion

        #region Constructor
        public GlyphWriter(Dictionary<string, byte[]> glyphCharSet, Graphics2D grpx,bool isC1)
        {
            glyphs = glyphCharSet;
            m_grpx = grpx;
            this.is1C = isC1;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Parse the cff operands
        /// </summary>
        private int ParseOperand(byte[] glyphChars, int pos, double[] values, int valuePointer)
        {

            int glyphData, i;
            double x = 0;

            glyphData = glyphChars[pos];

            if ((glyphData < 28) | (glyphData == 31))
            {
            }
            else if (glyphData == 28)
            {
                x = (glyphChars[pos + 1] << 8) + (glyphChars[pos + 2]);
                pos += 3;
            }
            else if (glyphData == 255)
            {

                if (is1C)
                {
                    int top = ((glyphChars[pos + 1]) << 8) + (glyphChars[pos + 2]);
                    if (top > 32768)
                        top = 65536 - top;
                    double numb = top;
                    double dec = ((glyphChars[pos + 3]) << 8) + (glyphChars[pos + 4]);
                    x = numb + (dec / 65536);
                    if (glyphChars[pos + 1] < 0)
                    {
                        x = -x;
                    }
                }
                else
                {
                    x =
                        ((glyphChars[pos + 1]) << 24)
                            + ((glyphChars[pos + 2]) << 16)
                            + ((glyphChars[pos + 3]) << 8)
                            + (glyphChars[pos + 4]);

                }

                pos += 5;
            }
            else if (glyphData == 29)
            {
                x =
                    ((glyphChars[pos + 1]) << 24)
                        + ((glyphChars[pos + 2]) << 16)
                        + ((glyphChars[pos + 3]) << 8)
                        + (glyphChars[pos + 4]);
                pos += 5;
            }
            else if (glyphData == 30)
            {

                char[] buf = new char[65];
                pos += 1;
                i = 0;
                while (i < 64)
                {
                    int b = glyphChars[pos++];

                    int b0 = (b >> 4);
                    int b1 = b;

                    if (b0 == 0xf)
                        break;
                    buf[i++] = m_charSet[b0];
                    if (i == 64)
                        break;
                    if (b0 == 0xc)
                        buf[i++] = '-';
                    if (i == 64)
                        break;
                    if (b1 == 0xf)
                        break;
                    buf[i++] = m_charSet[b0];
                    if (i == 64)
                        break;
                    if (b1 == 0xc)
                        buf[i++] = '-';
                }
                x = (Double.Parse(new String(buf, 0, i), CultureInfo.InvariantCulture));

            }
            else if (glyphData < 247)
            {
                x = glyphData - 139;
                pos++;
            }
            else if (glyphData < 251)
            {
                x = ((glyphData - 247) << 8) + (glyphChars[pos + 1]) + 108;
                pos += 2;
            }
            else
            {
                x = -((glyphData - 251) << 8) - (glyphChars[pos + 1]) - 108;
                pos += 2;
            }

            values[valuePointer] = x;

            return pos;
        }

        /// <summary>
        /// Calculate side bearing width
        /// </summary>
        private double SideBearingWidth()
        {

            double yy;

            double val = m_operands[m_operandReached - 2];
            y = val;

            val = m_operands[m_operandReached - 1];
            x = val;

            xs = x;
            ys = y;
            m_allowAll = true;
            yy = y;

            m_height = m_operands[m_operandReached - 3];

            return yy;
        }

        /// <summary>
        /// Process flex to draw the bezier curve
        /// </summary>
        private bool ProcessFlex(int lastKey, bool isFlex, int routine)
        {
            if ((isFlex) && (m_pointCount == 14) && (routine == 0))
            {
                isFlex = false;
                for (int i = 0; i < 12; i = i + 6)
                {
                    AddBezierCurve(new string[] { m_point[i].ToString(), m_point[i + 1].ToString(), m_point[i + 2].ToString(), m_point[i + 3].ToString(), m_point[i + 4].ToString(), m_point[i + 5].ToString() });

                }
            }
            else if ((!isFlex) && (routine >= 0) && (routine <= 2))
            {
                isFlex = true;
                m_pointCount = 0;
                m_point = new float[16];
            }
            return isFlex;
        }

        /// <summary>
        /// draw the glyphs for flex1
        /// </summary>
        private void Flex1()
        {
            double dx = 0, dy = 0, x1 = x, y1 = y;

            for (int count = 0; count < 10; count = count + 2)
            {
                dx += m_operands[count];
                dy += m_operands[count + 1];
            }
            bool isHorizontal = (Math.Abs(dx) > Math.Abs(dy));

            for (int points = 0; points < 6; points = points + 2)
            {
                x += m_operands[points];
                y += m_operands[points + 1];
                m_point[points] = (float)x;
                m_point[points + 1] = (float)y;
            }
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });


            for (int points = 0; points < 4; points = points + 2)
            {
                x += m_operands[points + 6];
                y += m_operands[points + 7];
                m_point[points] = (float)x;
                m_point[points + 1] = (float)y;
            }

            if (isHorizontal)
            {
                x += m_operands[10];
                y = y1;
            }
            else
            {
                x = x1;
                y += m_operands[10];
            }
            m_point[4] = (float)x;
            m_point[5] = (float)y;
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });
        }

        /// <summary>
        /// draw the glyphs for flex
        /// </summary>
        private void Flex()
        {
            for (int curves = 0; curves < 12; curves = curves + 6)
            {
                for (int points = 0; points < 6; points = points + 2)
                {
                    x += m_operands[curves + points];
                    y += m_operands[curves + points + 1];
                    m_point[points] = (float)x;
                    m_point[points + 1] = (float)y;
                }
                AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

            }
        }

        /// <summary>
        /// draw the glyphs for hybrid flex
        /// </summary>
        private void HybridFlex()
        {
            x += m_operands[0];
            m_point[0] = (float)x;
            m_point[1] = (float)y;
            x += m_operands[1];
            y += m_operands[2];
            m_point[2] = (float)x;
            m_point[3] = (float)y;
            x += m_operands[3];
            m_point[4] = (float)x;
            m_point[5] = (float)y;
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });


            x += m_operands[4];
            m_point[0] = (float)x;
            m_point[1] = (float)y;
            x += m_operands[5];
            m_point[2] = (float)x;
            m_point[3] = (float)y;
            x += m_operands[6];
            m_point[4] = (float)x;
            m_point[5] = (float)y;
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

        }

        /// <summary>
        /// draw the glyphs for hybrid flex1
        /// </summary>
        private void HybridFlex1()
        {
            x += m_operands[0];
            y += m_operands[1];
            m_point[0] = (float)x;
            m_point[1] = (float)y;
            x += m_operands[2];
            y += m_operands[3];
            m_point[2] = (float)x;
            m_point[3] = (float)y;
            x += m_operands[4];
            m_point[4] = (float)x;
            m_point[5] = (float)y;
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

            x += m_operands[5];
            m_point[0] = (float)x;
            m_point[1] = (float)y;
            x += m_operands[6];
            y += m_operands[7];
            m_point[2] = (float)x;
            m_point[3] = (float)y;
            x += m_operands[8];
            m_point[4] = (float)x;
            m_point[5] = (float)y;
            AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

        }

        /// <summary>
        /// Set the current point
        /// </summary>
        private void SetCurrentPoint()
        {
            x = m_operands[0];
            y = m_operands[1];

            BeginPath(new string[] { x.ToString(), y.ToString() });
        }

        /// <summary>
        /// Compute the div Command
        /// </summary>
        private void Div()
        {

            double value = m_operands[m_operandReached - 2] / m_operands[m_operandReached - 1];

            if (m_operandReached > 0)
                m_operandReached--;
            m_operands[m_operandReached - 1] = value;


        }

        /// <summary>
        /// Compute the path with Vertival move to
        /// </summary>
        private void VerticalMoveTo(bool isFirst)
        {
            if ((isFirst) && (m_operandReached == 2))
                m_currentOperand++;
            y = y + m_operands[m_currentOperand];

            BeginPath(new string[] { x.ToString(), y.ToString() });
            xs = x;
            ys = y;

        }

        /// <summary>
        /// Compute the path with relative lineto
        /// </summary>
        private void RelativeLineTo()
        {
            int lineCount = m_operandReached / 2;
            while (lineCount > 0)
            {
                x += m_operands[m_currentOperand];
                y += m_operands[m_currentOperand + 1];
                AddLine(new string[] { x.ToString(), y.ToString() });
                m_currentOperand += 2;
                lineCount--;

            }
        }

        /// <summary>
        /// Compute the path with horizontal vertival move to
        /// </summary>
        private void HorizontalVerticalLineTo(int key)
        {
            bool isHor = (key == 6);
            int start = 0;
            while (start < m_operandReached)
            {
                if (isHor)
                    x += m_operands[start];
                else
                    y += m_operands[start];
                AddLine(new string[] { x.ToString(), y.ToString() });

                start++;
                isHor = !isHor;
            }
        }

        /// <summary>
        /// Compute the path with relative curveto
        /// </summary>
        private void RelativeRCurveTo()
        {
            int curveCount = (m_operandReached) / 6;

            while (curveCount > 0)
            {
                float[] coords = new float[6];
                x += m_operands[m_currentOperand];
                y += m_operands[m_currentOperand + 1];
                coords[0] = (float)x;
                coords[1] = (float)y;

                x += m_operands[m_currentOperand + 2];
                y += m_operands[m_currentOperand + 3];
                coords[2] = (float)x;
                coords[3] = (float)y;

                x += m_operands[m_currentOperand + 4];
                y += m_operands[m_currentOperand + 5];
                coords[4] = (float)x;
                coords[5] = (float)y;
                AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });

                m_currentOperand += 6;
                curveCount--;
            }
        }

        /// <summary>
        /// Compute the path with relative curveto
        /// </summary>
        private void RelativeCurveLine()
        {
            int curveCount = (m_operandReached - 2) / 6;
            while (curveCount > 0)
            {
                float[] coords = new float[6];
                x += m_operands[m_currentOperand];
                y += m_operands[m_currentOperand + 1];
                coords[0] = (float)x;
                coords[1] = (float)y;

                x += m_operands[m_currentOperand + 2];
                y += m_operands[m_currentOperand + 3];
                coords[2] = (float)x;
                coords[3] = (float)y;

                x += m_operands[m_currentOperand + 4];
                y += m_operands[m_currentOperand + 5];
                coords[4] = (float)x;
                coords[5] = (float)y;

                AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });

                m_currentOperand += 6;
                curveCount--;
            }

            x += m_operands[m_currentOperand];
            y += m_operands[m_currentOperand + 1];
            AddLine(new string[] { x.ToString(), y.ToString() });
            m_currentOperand += 2;

        }

        /// <summary>
        /// Compute the pop command
        /// </summary>
        private void Pop()
        {

            if (m_operandReached > 0)
                m_operandReached--;
        }

        /// <summary>
        /// Compute the path with horizotal side bearing width
        /// </summary>
        private void Hsbw()
        {
            x = x + m_operands[0];
            BeginPath(new string[] { x.ToString(), "0" });
            m_allowAll = true;
        }

        /// <summary>
        /// Compute the closepath command
        /// </summary>
        private void ClosePath()
        {
            if (xs != -1)
                AddLine(new string[] { xs.ToString(), ys.ToString() });

            xs = -1;

        }

        /// <summary>
        /// Compute the mask command
        /// </summary>
        private int Mask(int p, int lastKey)
        {
            m_hintCount += m_operandReached / 2;

            int count = m_hintCount;
            while (count > 0)
            {
                p++;
                count = count - 8;
            }
            return p;
        }

        /// <summary>
        /// Compute the path with horizontal moveto
        /// </summary>
        private void HorizontalMoveTo(bool isFirst)
        {
            if ((isFirst) && (m_operandReached == 2))
                m_currentOperand++;

            double val = m_operands[m_currentOperand];
            x = x + val;
            BeginPath(new string[] { x.ToString(), y.ToString() });
            xs = x;
            ys = y;

        }

        /// <summary>
        /// Compute the path with relative moveto
        /// </summary>
        private void RelativeMoveTo(bool isFirst)
        {
            if ((isFirst) && (m_operandReached == 3))
                m_currentOperand++;

            double val = m_operands[m_currentOperand + 1];
            y = y + val;
            val = m_operands[m_currentOperand];
            x = x + val;
            BeginPath(new string[] { x.ToString(), y.ToString() });

            xs = x;
            ys = y;
        }

        /// <summary>
        /// Compute the path with vertical horizontal horizontal vertical curveto
        /// </summary>
        private void VHHVCurveTo(int key)
        {
            bool isHor = (key == 31);
            while (m_operandReached >= 4)
            {
                m_operandReached -= 4;
                if (isHor)
                    x += m_operands[m_currentOperand];
                else
                    y += m_operands[m_currentOperand];
                m_point[0] = (float)x;
                m_point[1] = (float)y;
                x += m_operands[m_currentOperand + 1];
                y += m_operands[m_currentOperand + 2];
                m_point[2] = (float)x;
                m_point[3] = (float)y;
                if (isHor)
                {
                    y += m_operands[m_currentOperand + 3];
                    if (m_operandReached == 1)
                        x += m_operands[m_currentOperand + 4];
                }
                else
                {
                    x += m_operands[m_currentOperand + 3];
                    if (m_operandReached == 1)
                        y += m_operands[m_currentOperand + 4];
                }
                m_point[4] = (float)x;
                m_point[5] = (float)y;
                AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

                m_currentOperand += 4;

                isHor = !isHor;
            }
        }

        /// <summary>
        /// Compute the path with vertical vertical horizontal horizontal curveto
        /// </summary>
        private void VVHHCurveTo(int key)
        {

            bool isVV = (key == 26);
            if ((m_operandReached & 1) == 1)
            {
                if (isVV)
                    x += m_operands[0];
                else
                    y += m_operands[0];
                m_currentOperand++;
            }

            while (m_currentOperand < m_operandReached)
            {
                if (isVV)
                    y += m_operands[m_currentOperand];
                else
                    x += m_operands[m_currentOperand];
                m_point[0] = (float)x;
                m_point[1] = (float)y;
                x += m_operands[m_currentOperand + 1];
                y += m_operands[m_currentOperand + 2];
                m_point[2] = (float)x;
                m_point[3] = (float)y;
                if (isVV)
                    y += m_operands[m_currentOperand + 3];
                else
                    x += m_operands[m_currentOperand + 3];
                m_point[4] = (float)x;
                m_point[5] = (float)y;
                m_currentOperand += 4;
                AddBezierCurve(new string[] { m_point[0].ToString(), m_point[1].ToString(), m_point[2].ToString(), m_point[3].ToString(), m_point[4].ToString(), m_point[5].ToString() });

            }
        }

        /// <summary>
        /// Compute the path with relatice line curveto
        /// </summary>
        private void RelativeLineCurve()
        {
            int lineCount = (m_operandReached - 6) / 2;
            while (lineCount > 0)
            {
                x += m_operands[m_currentOperand];
                y += m_operands[m_currentOperand + 1];
                AddLine(new string[] { x.ToString(), y.ToString() });

                m_currentOperand += 2;
                lineCount--;
            }
            float[] coords = new float[6];
            x += m_operands[m_currentOperand];
            y += m_operands[m_currentOperand + 1];
            coords[0] = (float)x;
            coords[1] = (float)y;

            x += m_operands[m_currentOperand + 2];
            y += m_operands[m_currentOperand + 3];
            coords[2] = (float)x;
            coords[3] = (float)y;

            x += m_operands[m_currentOperand + 4];
            y += m_operands[m_currentOperand + 5];
            coords[4] = (float)x;
            coords[5] = (float)y;

            AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });


            m_currentOperand += 6;
        }

        /// <summary>
        /// Compute the path with end char
        /// </summary>
        private int EndChar(int rawInt, int dicEnd)
        {
            int p;

            if (m_operandReached == 5)
            {
                m_operandReached--;
                m_currentOperand++;
            }

            p = dicEnd;
            return p;
        }

        /// <summary>
        /// parse the glyphs paths
        /// </summary>
        internal GeometryGroup glyphParser(string glyphChar, int rawInt, float currentWidth)
        {
            byte[] glyphStream = null;

            if (glyphChar == null)
            {
                if (glyphChar == null)
                    glyphChar = ".notdef";
            }
            BeginPath(new string[] { "0", "0" });

            glyphStream = glyphs[glyphChar];

            if (glyphStream == null)
            {
                if (glyphStream == null)
                    glyphStream = glyphs[".notdef"];
            }

            if (glyphStream != null)
            {

                bool isFirst = true;
                m_pointCount = 0;
                int commandCount = -1;
                int i = 0, last = 0, next, key = 0, lastKey, glyphCount = glyphStream.Length, lastVal = 0;
                m_currentOperand = 0;
                m_hintCount = 0;
                double ymin = 999999, ymax = 0, yy = 1000;
                bool isFlex = false;
                m_point = new float[6];

                m_height = 100000;
                if (is1C)
                {
                    m_operands = new double[100];
                    m_operandReached = 0;
                    m_allowAll = true;
                }

                while (i < glyphCount)
                {
                    next = glyphStream[i];
                    if (next > 31 || next == 28)
                    {
                        last = i;
                        i = ParseOperand(glyphStream, i, m_operands, m_operandReached);
                        lastVal = (int)m_operands[m_operandReached];//nextVal;
                        m_operandReached++;
                    }
                    else
                    {
                        commandCount++;
                        lastKey = key;
                        key = next;
                        i++;
                        m_currentOperand = 0;

                        if (key == 12)
                        {
                            key = glyphStream[i];
                            i++;

                            if (key == 7)
                            {
                                yy = SideBearingWidth();
                                m_operandReached = 0;
                            }
                            else if (m_allowAll)
                            {
                                switch (key)
                                {
                                    case 16:
                                        {
                                            isFlex = ProcessFlex(lastKey, isFlex, lastVal);
                                            m_operandReached = 0;
                                            break;
                                        }
                                    case 33:
                                        {
                                            m_operandReached = 0;
                                            break;
                                        }
                                    case 34:
                                        {
                                            HybridFlex();
                                            m_operandReached = 0;
                                            break;
                                        }
                                    case 35:
                                        {
                                            Flex();
                                            m_operandReached = 0;
                                        }
                                        break;
                                    case 36:
                                        {
                                            HybridFlex1();
                                            m_operandReached = 0;
                                        }
                                        break;
                                    case 37:
                                        {
                                            Flex1();
                                            m_operandReached = 0;
                                        }
                                        break;
                                    case 6:
                                        {
                                            m_operandReached = 0;
                                        }
                                        break;
                                    case 12:
                                        {
                                            Div();
                                        }
                                        break;
                                    case 17:
                                        {
                                            Pop();
                                        }
                                        break;
                                    case 0:
                                        {
                                            m_operandReached = 0;

                                        }
                                        break;
                                    default:
                                        m_operandReached = 0;
                                        break;
                                }
                            }
                        }
                        else if (key == 13)
                        {
                            Hsbw();
                            m_operandReached = 0;
                        }
                        else if (m_allowAll)
                        {
                            if ((key == 1) | (key == 3) | (key == 18) | (key == 23))
                            {
                                m_hintCount += m_operandReached / 2;
                                m_operandReached = 0;
                            }
                            else if (key == 4)
                            {
                                if (isFlex)
                                {
                                    double val = m_operands[m_currentOperand];
                                    y = y + val;
                                    m_point[m_pointCount] = (float)x;
                                    m_pointCount++;
                                    m_point[m_pointCount] = (float)y;
                                    m_pointCount++;

                                }
                                else
                                    VerticalMoveTo(isFirst);
                                m_operandReached = 0;
                            }
                            else if ((key == 5))
                            {
                                RelativeLineTo();
                                m_operandReached = 0;
                            }
                            else if ((key == 6) | (key == 7))
                            {
                                HorizontalVerticalLineTo(key);
                                m_operandReached = 0;
                            }
                            else if (key == 8)
                            {
                                RelativeRCurveTo();
                                m_operandReached = 0;
                            }
                            else if (key == 9)
                            {
                                ClosePath();
                                m_operandReached = 0;
                            }
                            else if (key == 10 || (key == 29))
                            {

                                if (1 == 2 && is1C && isFirst)
                                {
                                    xs = -1;
                                    ys = -1;

                                }
                                if (!is1C && key == 10 && (lastVal >= 0) && (lastVal <= 2) && lastKey != 11 && m_operandReached > 5)
                                {
                                    isFlex = ProcessFlex(lastKey, isFlex, lastVal);
                                    m_operandReached = 0;

                                }
                                else
                                {
                                    int localBias = 0, globalBias = 0;
                                    if (key == 10)
                                        lastVal = lastVal + localBias;
                                    else
                                        lastVal = lastVal + globalBias;

                                    byte[] subrsGlyph = null;
                                    if (key == 10)
                                    {
                                        subrsGlyph = glyphs["subrs" + lastVal];
                                    }
                                    else
                                    {
                                        subrsGlyph = glyphs["global" + lastVal];
                                    }

                                    if (subrsGlyph != null)
                                    {
                                        int newLength = subrsGlyph.Length;
                                        int oldLength = glyphStream.Length;
                                        int totalLength = newLength + oldLength - 2;

                                        glyphCount = glyphCount + newLength - 2;
                                        byte[] combinedStream = new byte[totalLength];

                                        Array.Copy(glyphStream, 0, combinedStream, 0, last);
                                        Array.Copy(subrsGlyph, 0, combinedStream, last, newLength);
                                        Array.Copy(glyphStream, i, combinedStream, last + newLength, oldLength - i);

                                        glyphStream = combinedStream;

                                        i = last;

                                        if (m_operandReached > 0)
                                            m_operandReached--;
                                    }

                                }
                            }
                            else if ((key == 14))
                            {
                                i = EndChar(rawInt, glyphCount);
                                m_operandReached = 0;
                                i = glyphCount + 1;
                            }
                            else if (key == 16)
                            {

                                m_operandReached = 0;
                            }
                            else if ((key == 19) | (key == 20))
                            {
                                i = Mask(i, lastKey);
                                m_operandReached = 0;
                            }
                            else if (key == 21)
                            {
                                if (isFlex)
                                {
                                    double val = m_operands[m_currentOperand + 1];
                                    y = y + val;
                                    val = m_operands[m_currentOperand];
                                    x = x + val;
                                    m_point[m_pointCount] = (float)x;
                                    m_pointCount++;

                                    m_point[m_pointCount] = (float)y;
                                    m_pointCount++;

                                }
                                else
                                    RelativeMoveTo(isFirst);
                                m_operandReached = 0;
                            }
                            else if (key == 22)
                            {
                                if (isFlex)
                                {
                                    double val = m_operands[m_currentOperand];
                                    x = x + val;
                                    m_point[m_pointCount] = (float)x;
                                    m_pointCount++;
                                    m_point[m_pointCount] = (float)y;
                                    m_pointCount++;
                                }
                                else
                                    HorizontalMoveTo(isFirst);
                                m_operandReached = 0;
                            }
                            else if (key == 24)
                            {
                                RelativeCurveLine();
                                m_operandReached = 0;
                            }
                            else if (key == 25)
                            {
                                RelativeLineCurve();
                                m_operandReached = 0;
                            }
                            else if ((key == 26) | (key == 27))
                            {
                                VVHHCurveTo(key);
                                m_operandReached = 0;
                            }
                            else if ((key == 30) | (key == 31))
                            {
                                VHHVCurveTo(key);
                                m_operandReached = 0;
                            }
                        }

                        if (ymin > y)
                            ymin = y;

                        if (ymax < y)
                            ymax = y;

                        if (key != 19 && key != 29 && key != 10)
                            isFirst = false;
                    }
                }

                if (yy > m_height)
                    ymin = yy - m_height;


                if ((ymax) < yy)
                {
                    ymin = 0;

                }
                else
                {

                    float dy = (float)(ymax - (yy - ymin));

                    if ((dy < 0))
                    {

                        if (yy - ymax <= dy)
                            ymin = dy;
                        else
                            ymin = ymin - dy;
                    }
                    else
                        ymin = 0;

                    if (ymin < 0)
                        ymin = 0;
                }
            }
            return FillPath("", true, true, m_grpx);
        }

        /// <summary>
        /// Fill the path
        /// </summary>
        private GeometryGroup FillPath(string rule, bool Close, bool Stroke, Graphics2D graphics)
        {

            if (PathSink != null && IsPathSinkClosed == false)
            {
                if (Close)
                    PathSink.EndFigure(FigureEnd.Closed);
                else
                    PathSink.EndFigure(FigureEnd.Open);
                PathSink.Close();
                IsPathSinkClosed = true;
            }

            if (PathSink == null && m_pathRenderList.Count == 0)
                return null;

            PathGeometry[] pathGeos = new PathGeometry[m_pathRenderList.Count + 1];
            pathGeos[m_pathRenderList.Count] = m_path;

            if (m_pathRenderList.Count > 0)
            {
                Matrix tempMatrix = graphics.Transform;
                int i = 0;

                foreach (PathRenderer pth in m_pathRenderList)
                {
                    GeometrySink geoSink = pth.PathSink;
                    PathGeometry geoPath = pth.PathGeomentry;
                    if (!pth.IsCurrentPathSinkClosed)
                    {
                        if (Close)
                            geoSink.EndFigure(FigureEnd.Closed);
                        else
                            geoSink.EndFigure(FigureEnd.Open);
                        geoSink.Close();
                    }
                    pathGeos[i] = geoPath;
                    i++;
                }
                graphics.Transform = tempMatrix;
                m_pathMatrix.Clear();
                m_pathRenderList.Clear();
            }

            GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Winding, pathGeos);
            return geoGroup;

        }

        /// <summary>
        /// Add line to the path
        /// </summary>
        private void AddLine(string[] line)
        {
            if (!IsPathSinkClosed)
            {
                PathSink.AddLine(new global::Windows.Foundation.Point(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture)));
            }
            else
            {
                PathSink = m_path.Open();
                PathSink.AddLine(new global::Windows.Foundation.Point(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture)));
            }
            CurrentLocation = new System.Drawing.PointF(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Add bezier curve
        /// </summary>
        private void AddBezierCurve(string[] curve)
        {

            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new global::Windows.Foundation.Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            bezier.Point2 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            bezier.Point3 = new global::Windows.Foundation.Point(float.Parse(curve[4], CultureInfo.InvariantCulture), -float.Parse(curve[5], CultureInfo.InvariantCulture));
            PathSink.AddBezier(bezier);
            CurrentLocation = new System.Drawing.PointF(float.Parse(curve[4], CultureInfo.InvariantCulture), -float.Parse(curve[5], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Add sub paths
        /// </summary>
        private void AddSubPaths(Graphics2D graphics)
        {
            if (PathSink != null)
            {
                PathRenderer renderer = new PathRenderer();
                renderer.PathGeomentry = m_path;
                renderer.PathSink = PathSink;
                renderer.IsCurrentPathSinkClosed = IsPathSinkClosed;
                m_pathRenderList.Add(renderer);
                m_pathMatrix.Add(graphics.Transform);
            }
        }

        /// <summary>
        /// Begin the path
        /// </summary>
        private void BeginPath(string[] point)
        {
            AddSubPaths(m_grpx);
            m_path = m_grpx.CreatePathGeometry();
            PathSink = m_path.Open();
            IsPathSinkClosed = false;
            PathSink.SetFillMode(FillMode.Alternate);
            PathSink.BeginFigure(new global::Windows.Foundation.Point(float.Parse(point[0], CultureInfo.InvariantCulture), -float.Parse(point[1], CultureInfo.InvariantCulture)), FigureBegin.Filled);
        }
        #endregion
    }
}
