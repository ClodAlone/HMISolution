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
using System.IO;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using System.Drawing.Drawing2D;

namespace Syncfusion.PdfViewer.Base
{
    class GlyphWriter
    {
        private MemoryStream glyphStream;
        private static String charSet = "0123456789.ee -";
        public bool is1C = false;
        private String[] charForGlyphIndex;
        private int max = 100;
        double[] operands = new double[100];
        private int operandReached = 0;
        private float[] pt;
        private double xs = -1, ys = -1, x = 0, y = 0;
        private int ptCount = 0;
        int currentOp = 0;
        private int hintCount = 0;
        private bool allowAll = false;
        private double h;
        private GraphicsPath Path;
        private PointF CurrentLocation = new PointF();
        private List<GraphicsPath> m_tempSubPaths = new List<GraphicsPath>();
        private PdfUnitConvertor m_convertor = new PdfUnitConvertor();
        internal Dictionary<string, byte[]> glyphs = new Dictionary<string, byte[]>();
        internal Dictionary<int, string> UnicodeCharMapTable;
        /// <summary>
        /// Represents the graphics object
        /// </summary>
        public GlyphWriter(Dictionary<string, byte[]> glyphCharSet, bool isC1)
        {
            glyphs = glyphCharSet;
            this.is1C = isC1;
        }

        public int ParseOperand(byte[] glyphChars, int pos, double[] values, int valuePointer)
        {

            int glyphData, i;
            double x = 0;

            glyphData = glyphChars[pos];

            if ((glyphData < 28) | (glyphData == 31))
            {
                //Incorrect type1C operand
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
                    buf[i++] = charSet[b0];
                    if (i == 64)
                        break;
                    if (b0 == 0xc)
                        buf[i++] = '-';
                    if (i == 64)
                        break;
                    if (b1 == 0xf)
                        break;
                    buf[i++] = charSet[b0];
                    if (i == 64)
                        break;
                    if (b1 == 0xc)
                        buf[i++] = '-';
                }
                x = (Double.Parse(new String(buf, 0, i)));

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
        private double SideBearingWidth()
        {

            double yy;

            double val = operands[operandReached - 2];
            y = val;

            val = operands[operandReached - 1];
            x = val;

            xs = x;
            ys = y;
            allowAll = true;
            yy = y;

            h = operands[operandReached - 3];

            return yy;
        }
        private bool ProcessFlex(int lastKey, bool isFlex, int routine)
        {
            if ((isFlex) && (ptCount == 14) && (routine == 0))
            {
                isFlex = false;
                for (int i = 0; i < 12; i = i + 6)
                {
                    AddBezierCurve(new string[] { pt[i].ToString(), pt[i + 1].ToString(), pt[i + 2].ToString(), pt[i + 3].ToString(), pt[i + 4].ToString(), pt[i + 5].ToString() });

                }
            }
            else if ((!isFlex) && (routine >= 0) && (routine <= 2))
            {
                isFlex = true;
                ptCount = 0;
                pt = new float[16];
            }
            return isFlex;
        }

        private void StandardEncodingAccent(int rawInt, int currentOp)
        {

            //StandardFonts.checkLoaded(StandardFonts.STD);
            //float adx = (float)(operands[currentOp + 1]);
            //float ady = (float)(operands[currentOp + 2]);
            //String bchar = StandardFonts.getUnicodeChar(StandardFonts.STD, (int)operands[currentOp + 3]);
            //String achar = StandardFonts.getUnicodeChar(StandardFonts.STD, (int)operands[currentOp + 4]);

            //double preX = x;
            //y = 0;
            //glyphParser(bchar, rawInt, 0);

            //BeginPath(new string[] { "0", "0" });
            //x = adx + preX;
            //y = ady;
            //glyphParser(achar, rawInt, 0);
        }

        private void Flex1()
        {
            double dx = 0, dy = 0, x1 = x, y1 = y;

            for (int count = 0; count < 10; count = count + 2)
            {
                dx += operands[count];
                dy += operands[count + 1];
            }
            bool isHorizontal = (Math.Abs(dx) > Math.Abs(dy));

            for (int points = 0; points < 6; points = points + 2)
            {
                x += operands[points];
                y += operands[points + 1];
                pt[points] = (float)x;
                pt[points + 1] = (float)y;
            }
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });


            for (int points = 0; points < 4; points = points + 2)
            {
                x += operands[points + 6];
                y += operands[points + 7];
                pt[points] = (float)x;
                pt[points + 1] = (float)y;
            }

            if (isHorizontal)
            {
                x += operands[10];
                y = y1;
            }
            else
            {
                x = x1;
                y += operands[10];
            }
            pt[4] = (float)x;
            pt[5] = (float)y;
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });
        }

        private void Flex()
        {
            for (int curves = 0; curves < 12; curves = curves + 6)
            {
                for (int points = 0; points < 6; points = points + 2)
                {
                    x += operands[curves + points];
                    y += operands[curves + points + 1];
                    pt[points] = (float)x;
                    pt[points + 1] = (float)y;
                }
                AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

            }
        }


        private void HybridFlex()
        {
            x += operands[0];
            pt[0] = (float)x;
            pt[1] = (float)y;
            x += operands[1];
            y += operands[2];
            pt[2] = (float)x;
            pt[3] = (float)y;
            x += operands[3];
            pt[4] = (float)x;
            pt[5] = (float)y;
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });


            x += operands[4];
            pt[0] = (float)x;
            pt[1] = (float)y;
            x += operands[5];
            pt[2] = (float)x;
            pt[3] = (float)y;
            x += operands[6];
            pt[4] = (float)x;
            pt[5] = (float)y;
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

        }

        private void HybridFlex1()
        {
            x += operands[0];
            y += operands[1];
            pt[0] = (float)x;
            pt[1] = (float)y;
            x += operands[2];
            y += operands[3];
            pt[2] = (float)x;
            pt[3] = (float)y;
            x += operands[4];
            pt[4] = (float)x;
            pt[5] = (float)y;
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

            x += operands[5];
            pt[0] = (float)x;
            pt[1] = (float)y;
            x += operands[6];
            y += operands[7];
            pt[2] = (float)x;
            pt[3] = (float)y;
            x += operands[8];
            pt[4] = (float)x;
            pt[5] = (float)y;
            AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

        }

        private void SetCurrentPoint()
        {
            x = operands[0];
            y = operands[1];

            BeginPath(new string[] { x.ToString(), y.ToString() });
        }

        private void Div()
        {

            double value = operands[operandReached - 2] / operands[operandReached - 1];

            if (operandReached > 0)
                operandReached--;
            operands[operandReached - 1] = value;


        }
        private void VerticalMoveTo(bool isFirst)
        {
            if ((isFirst) && (operandReached == 2))
                currentOp++;
            y = y + operands[currentOp];

            BeginPath(new string[] { x.ToString(), y.ToString() });
            xs = x;
            ys = y;

        }

        private void RelativeLineTo()
        {
            int lineCount = operandReached / 2;
            while (lineCount > 0)
            {
                x += operands[currentOp];
                y += operands[currentOp + 1];
                AddLine(new string[] { x.ToString(), y.ToString() });
                currentOp += 2;
                lineCount--;

            }
        }

        private void HorizontalVerticalLineTo(int key)
        {
            bool isHor = (key == 6);
            int start = 0;
            while (start < operandReached)
            {
                if (isHor)
                    x += operands[start];
                else
                    y += operands[start];
                AddLine(new string[] { x.ToString(), y.ToString() });

                start++;
                isHor = !isHor;
            }
        }

        private void RelativeRCurveTo()
        {
            int curveCount = (operandReached) / 6;

            while (curveCount > 0)
            {
                float[] coords = new float[6];
                x += operands[currentOp];
                y += operands[currentOp + 1];
                coords[0] = (float)x;
                coords[1] = (float)y;

                x += operands[currentOp + 2];
                y += operands[currentOp + 3];
                coords[2] = (float)x;
                coords[3] = (float)y;

                x += operands[currentOp + 4];
                y += operands[currentOp + 5];
                coords[4] = (float)x;
                coords[5] = (float)y;
                AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });

                currentOp += 6;
                curveCount--;
            }
        }
       
        private  void EndChar(int rawInt)
        {
            float adx = (float)(x + operands[currentOp]);
            float ady = (float)(y + operands[currentOp + 1]);
         
           
            string bchar = getunicodechar(((int)(operands[currentOp + 2])));
            string achar = getunicodechar(((int)(operands[currentOp + 3])));
            x = 0; y = 0;

            glyphParser(bchar.ToString(), rawInt, 500,null);
            ClosePath();
            x = adx;
            y = ady;
            glyphParser(achar.ToString(), rawInt, 500,null);
            

        }

        private void RelativeCurveLine()
        {
            //curves
            int curveCount = (operandReached - 2) / 6;
            while (curveCount > 0)
            {
                float[] coords = new float[6];
                x += operands[currentOp];
                y += operands[currentOp + 1];
                coords[0] = (float)x;
                coords[1] = (float)y;

                x += operands[currentOp + 2];
                y += operands[currentOp + 3];
                coords[2] = (float)x;
                coords[3] = (float)y;

                x += operands[currentOp + 4];
                y += operands[currentOp + 5];
                coords[4] = (float)x;
                coords[5] = (float)y;

                AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });

                currentOp += 6;
                curveCount--;
            }

            // line
            x += operands[currentOp];
            y += operands[currentOp + 1];
            AddLine(new string[] { x.ToString(), y.ToString() });
            currentOp += 2;

        }

        private void Pop()
        {

            if (operandReached > 0)
                operandReached--;
        }

        private void Hsbw()
        {
            x = x + operands[0];
            BeginPath(new string[] { x.ToString(), "0" });
            allowAll = true;
        }

        private void ClosePath()
        {
            if (xs != -1)
                AddLine(new string[] { xs.ToString(), ys.ToString() });

            xs = -1; //flag as unset

        }


        private int Mask(int p, int lastKey)
        {
            hintCount += operandReached / 2;

            int count = hintCount;
            while (count > 0)
            {
                p++;
                count = count - 8;
            }
            return p;
        }

        private void HorizontalMoveTo(bool isFirst)
        {
            if ((isFirst) && (operandReached == 2))
                currentOp++;

            double val = operands[currentOp];
            x = x + val;
            BeginPath(new string[] { x.ToString(), y.ToString() });
            xs = x;
            ys = y;

        }

        private void RelativeMoveTo(bool isFirst)
        {
            if ((isFirst) && (operandReached == 3))
                currentOp++;

            double val = operands[currentOp + 1];
            y = y + val;
            val = operands[currentOp];
            x = x + val;
            BeginPath(new string[] { x.ToString(), y.ToString() });

            xs = x;
            ys = y;
        }


        private void VHHVCurveTo(int key)
        {
            bool isHor = (key == 31);
            while (operandReached >= 4)
            {
                operandReached -= 4;
                if (isHor)
                    x += operands[currentOp];
                else
                    y += operands[currentOp];
                pt[0] = (float)x;
                pt[1] = (float)y;
                x += operands[currentOp + 1];
                y += operands[currentOp + 2];
                pt[2] = (float)x;
                pt[3] = (float)y;
                if (isHor)
                {
                    y += operands[currentOp + 3];
                    if (operandReached == 1)
                        x += operands[currentOp + 4];
                }
                else
                {
                    x += operands[currentOp + 3];
                    if (operandReached == 1)
                        y += operands[currentOp + 4];
                }
                pt[4] = (float)x;
                pt[5] = (float)y;
                AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

                currentOp += 4;

                isHor = !isHor;
            }
        }

        private void VVHHCurveTo(int key)
        {

            bool isVV = (key == 26);
            if ((operandReached & 1) == 1)
            {
                if (isVV)
                    x += operands[0];
                else
                    y += operands[0];
                currentOp++;
            }

            //note odd co-ord order
            while (currentOp < operandReached)
            {
                if (isVV)
                    y += operands[currentOp];
                else
                    x += operands[currentOp];
                pt[0] = (float)x;
                pt[1] = (float)y;
                x += operands[currentOp + 1];
                y += operands[currentOp + 2];
                pt[2] = (float)x;
                pt[3] = (float)y;
                if (isVV)
                    y += operands[currentOp + 3];
                else
                    x += operands[currentOp + 3];
                pt[4] = (float)x;
                pt[5] = (float)y;
                currentOp += 4;
                AddBezierCurve(new string[] { pt[0].ToString(), pt[1].ToString(), pt[2].ToString(), pt[3].ToString(), pt[4].ToString(), pt[5].ToString() });

            }
        }

        private void RelativeLineCurve()
        {
            //lines
            int lineCount = (operandReached - 6) / 2;
            while (lineCount > 0)
            {
                x += operands[currentOp];
                y += operands[currentOp + 1];
                AddLine(new string[] { x.ToString(), y.ToString() });

                currentOp += 2;
                lineCount--;
            }
            //curves
            float[] coords = new float[6];
            x += operands[currentOp];
            y += operands[currentOp + 1];
            coords[0] = (float)x;
            coords[1] = (float)y;

            x += operands[currentOp + 2];
            y += operands[currentOp + 3];
            coords[2] = (float)x;
            coords[3] = (float)y;

            x += operands[currentOp + 4];
            y += operands[currentOp + 5];
            coords[4] = (float)x;
            coords[5] = (float)y;

            AddBezierCurve(new string[] { coords[0].ToString(), coords[1].ToString(), coords[2].ToString(), coords[3].ToString(), coords[4].ToString(), coords[5].ToString() });


            currentOp += 6;
        }

        private int EndChar(int rawInt, int dicEnd)
        {
            int p;

            if (operandReached == 5)
            { //allow for width and 4 chars
                operandReached--;
                currentOp++;
            }
            if (operandReached == 4)
            {
                EndChar(rawInt);
            }
            else
                ClosePath();            //.CloseFigure();//factory.closePath();

            p = dicEnd;
            return p;
        }

        //internal PathGeometry

        internal object glyphParser(string glyphChar, int rawInt, float currentWidth,Dictionary<int,string> unicodeCharMapTable)
        {
            byte[] glyphBytes = null;
            if(unicodeCharMapTable!=null)
            UnicodeCharMapTable = unicodeCharMapTable;
            if (glyphChar == null)
            {
                if (glyphChar == null)
                    glyphChar = ".notdef";
            }
            BeginPath(new string[] { "0", "0" });

            glyphBytes = glyphs[glyphChar];

            if (glyphBytes != null)
            {
                int[] tempTest = new int[glyphBytes.Length];

                for (int j = 0; j < tempTest.Length; j++)
                {
                    int temp = (int)glyphBytes[j];
                    if (temp > 256 / 2)
                    {
                        tempTest[j] = (int)glyphBytes[j] - 256;
                    }
                    else
                        tempTest[j] = (int)glyphBytes[j];
                }

                bool isFirst = true;
                int pointCount = 0;
                int commandCount = -1;
                int i = 0, last = 0, next, key = 0, lastKey, glyphCount = glyphBytes.Length, lastVal = 0;
                currentOp = 0;
                hintCount = 0;
                double ymin = 999999, ymax = 0, yy = 1000;
                bool isFlex = false;
                pt = new float[6];

                h = 100000;
                if (is1C)
                {
                    operands = new double[max];
                    operandReached = 0;
                    allowAll = true;

                }

                while (i < glyphCount)
                {
                    next = glyphBytes[i];
                    if (next > 31 || next == 28)
                    {
                        last = i;
                        i = ParseOperand(glyphBytes, i, operands, operandReached);
                        lastVal = (int)operands[operandReached];//nextVal;
                        operandReached++;
                    }
                    else
                    {
                        commandCount++;
                        lastKey = key;
                        key = next;
                        i++;
                        currentOp = 0;

                        if (key == 12)
                        {
                            key = glyphBytes[i];
                            i++;

                            if (key == 7)
                            {
                                yy = SideBearingWidth();
                                operandReached = 0;
                            }
                            else if (allowAll)
                            {
                                switch (key)
                                {
                                    case 16:
                                        {
                                            isFlex = ProcessFlex(lastKey, isFlex, lastVal);
                                            operandReached = 0; //move to first operator
                                            break;
                                        }
                                    case 33:
                                        {
                                            SetCurrentPoint();
                                            operandReached = 0;
                                            break;
                                        }
                                    case 34:
                                        {
                                            HybridFlex();
                                            operandReached = 0; //move to first operator
                                            break;
                                        }
                                    case 35:
                                        {
                                            Flex();
                                            operandReached = 0; //move to first operator
                                        }
                                        break;
                                    case 36:
                                        {
                                            HybridFlex1();
                                            operandReached = 0; //move to first operator
                                        }
                                        break;
                                    case 37:
                                        {
                                            Flex1();
                                            operandReached = 0; //move to first operator
                                        }
                                        break;
                                    case 6:
                                        {
                                            StandardEncodingAccent(rawInt, currentOp);
                                            operandReached = 0; //move to first operator
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
                                        { //dotsection
                                            operandReached = 0; //move to first operator

                                        }
                                        break;
                                    default:
                                        operandReached = 0; //move to first operator
                                        break;
                                }
                            }
                        }
                        else if (key == 13)
                        { //hsbw (T1 only)
                            Hsbw();
                            operandReached = 0; //move to first operator
                        }
                        else if (allowAll)
                        { //other one byte ops
                            if (key == 0)
                            { //reserved
                            }
                            else if ((key == 1) | (key == 3) | (key == 18) | (key == 23))
                            { //hstem vstem hstemhm vstemhm
                                hintCount += operandReached / 2;
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 4)
                            { //vmoveto
                                if (isFlex)
                                {
                                    double val = operands[currentOp];
                                    y = y + val;
                                    pt[pointCount] = (float)x;
                                    pointCount++;
                                    pt[pointCount] = (float)y;
                                    pointCount++;

                                }
                                else
                                    VerticalMoveTo(isFirst);
                                operandReached = 0; //move to first operator
                            }
                            else if ((key == 5))
                            {//rlineto
                                RelativeLineTo();
                                operandReached = 0; //move to first operator
                            }
                            else if ((key == 6) | (key == 7))
                            {//hlineto or vlineto
                                HorizontalVerticalLineTo(key);
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 8)
                            {//rrcurveto
                                RelativeRCurveTo();
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 9)
                            { //closepath (T1 only)
                                ClosePath();
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 10 || (key == 29))
                            { //callsubr and callgsubr

                                if (1 == 2 && is1C && isFirst)
                                {
                                    xs = -1;
                                    ys = -1;

                                }
                                if (!is1C && key == 10 && (lastVal >= 0) && (lastVal <= 2) && lastKey != 11 && operandReached > 5)
                                {//last key stops spurious match in multiple sub-routines
                                    isFlex = ProcessFlex(lastKey, isFlex, lastVal);
                                    operandReached = 0; //move to first operator

                                }
                                else
                                {
                                    int localBias = 0, globalBias = 0;
                                    //								factor in bias
                                    if (key == 10)
                                        lastVal = lastVal + localBias;
                                    else
                                        lastVal = lastVal + globalBias;

                                    byte[] subrsGlyph = null;
                                    if (key == 10)
                                    { //local subroutine

                                        subrsGlyph = glyphs["subrs" + (lastVal)];


                                    }
                                    else
                                    { //global subroutine

                                        subrsGlyph = glyphs["global" + (lastVal)];
                                    }

                                    if (subrsGlyph != null)
                                    {

                                        int newLength = subrsGlyph.Length;
                                        int oldLength = glyphBytes.Length;
                                        int totalLength = newLength + oldLength - 2;

                                        glyphCount = glyphCount + newLength - 2;
                                        //workout length of new stream
                                        byte[] combinedStream = new byte[totalLength];

                                        Array.Copy(glyphBytes, 0, combinedStream, 0, last);
                                        Array.Copy(subrsGlyph, 0, combinedStream, last, newLength);
                                        Array.Copy(glyphBytes, i, combinedStream, last + newLength, oldLength - i);

                                        glyphBytes = combinedStream;

                                        i = last;

                                        if (operandReached > 0)
                                            operandReached--;


                                    }

                                }
                                //operandReached=0; //move to first operator
                            }
                            else if (key == 11)
                            { //return

                                //operandReached=0; //move to first operator
                            }
                            else if ((key == 14))
                            { //endchar
                                i = EndChar(rawInt, glyphCount);
                                operandReached = 0; //move to first operator
                                i = glyphCount + 1;
                            }
                            else if (key == 16)
                            { //blend

                                operandReached = 0; //move to first operator
                            }
                            else if ((key == 19) | (key == 20))
                            { //hintmask //cntrmask
                                i = Mask(i, lastKey);
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 21)
                            {//rmoveto
                                if (isFlex)
                                {
                                    double val = operands[currentOp + 1];
                                    y = y + val;
                                    val = operands[currentOp];
                                    x = x + val;
                                    pt[pointCount] = (float)x;
                                    pointCount++;

                                    pt[pointCount] = (float)y;
                                    pointCount++;

                                }
                                else
                                    RelativeMoveTo(isFirst);
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 22)
                            { //hmoveto
                                if (isFlex)
                                {
                                    double val = operands[currentOp];
                                    x = x + val;
                                    pt[pointCount] = (float)x;
                                    pointCount++;
                                    pt[pointCount] = (float)y;
                                    pointCount++;
                                }
                                else
                                    HorizontalMoveTo(isFirst);
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 24)
                            { //rcurveline
                                RelativeCurveLine();
                                operandReached = 0; //move to first operator
                            }
                            else if (key == 25)
                            { //rlinecurve
                                RelativeLineCurve();
                                operandReached = 0; //move to first operator
                            }
                            else if ((key == 26) | (key == 27))
                            { //vvcurve hhcurveto
                                VVHHCurveTo(key);
                                operandReached = 0; //move to first operator
                            }
                            else if ((key == 30) | (key == 31))
                            {	//vhcurveto/hvcurveto
                                VHHVCurveTo(key);
                                operandReached = 0; //move to first operator
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

                if (yy > h)
                    ymin = yy - h;


                if ((ymax) < yy)
                {
                    ymin = 0;

                }
                //else if ((yy == ymax))
                //{
                //}
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
            return FillPath("abcd");
        }

        private GraphicsPath FillPath(string mode)
        {
            if (Path != null)
            {

                if (m_tempSubPaths.Count > 0)
                {
                    foreach (GraphicsPath path in m_tempSubPaths)
                    {
                        try
                        {
                            if (Path == null)
                            {
                                Path = new GraphicsPath();
                            }
                            if (path != null && path.PointCount > 0)
                            {
                                path.CloseAllFigures();
                                Path.AddPath(path, true);
                            }
                        }
                        catch
                        {
                            if (path != null && path.PointCount > 0)
                            {
                                Path = new GraphicsPath();
                                Path.AddPath(path, true);
                            }
                        }
                    }
                    m_tempSubPaths.Clear();
                }
                if (mode == "Alternate")
                    Path.FillMode = FillMode.Winding;
                else
                    Path.FillMode = FillMode.Alternate;

            }
            return Path;
        }

        private void AddLine(string[] line)
        {
            Path.AddLine(CurrentLocation.X, CurrentLocation.Y, float.Parse(line[0]), -float.Parse(line[1]));
            CurrentLocation = new PointF(float.Parse(line[0]), -float.Parse(line[1]));
        }

        private void AddBezierCurve(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point3 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            PointF point4 = new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void AddBezierCurve2(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point3 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point4 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = point4;
        }

        private void AddBezierCurve3(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point3 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            PointF point4 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = point4;
        }

        private void BeginPath(string[] point)
        {
            try
            {
                if (Path != null && Path.PointCount > 0)
                {
                    m_tempSubPaths.Add(Path);
                    Path = new GraphicsPath();
                }
                else
                {
                    Path = new GraphicsPath();
                }
            }
            catch
            {
                Path = new GraphicsPath();
            }

            CurrentLocation = new PointF(float.Parse(point[0]), -float.Parse(point[1]));
        }
        public string getunicodechar(int charval)
        {
            string unicodechar = UnicodeCharMapTable[charval];
            return unicodechar;
        }

    }
}
