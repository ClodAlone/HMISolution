#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using Syncfusion.Pdf.Graphics;
using Syncfusion.PdfViewer.Base;
using Syncfusion.Pdf;

namespace Syncfusion.Windows.PdfViewer
{
    internal class WPFGraphics : FrameworkElement
    {
        #region Members
        internal bool IsType1Font = false;
        internal bool Is1C = false;
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();
        internal Dictionary<string, byte[]> m_type1FontGlyphs = new Dictionary<string, byte[]>();
        internal Dictionary<int, string> differenceTable = new Dictionary<int, string>();
        internal Dictionary<string, string> differenceMappedTable = new Dictionary<string, string>();
        internal Dictionary<int, string> differenceEncoding = new Dictionary<int, string>();
        internal double type1Height;
        internal Dictionary<Double, string> CharacterMapTable;
        internal string renderedText = string.Empty;
        VisualCollection m_children;
        DrawingVisual m_visual;
        DrawingContext m_context;
        float m_defaultGlyphWidth;
        Stack<int> transformations = new Stack<int>();
        int m_pushCount = 0;
        const float CharSizeMultiplier = 0.001f;
        Dictionary<double, string> m_charactermapping = null;
        Dictionary<string, double> m_reverseMapTable;
        bool m_IsNegativeFont;
        #endregion

        #region Properties
        protected override int VisualChildrenCount
        {
            get { return m_children.Count; }
        }

        public DrawingContext Context
        {
            get
            {
                return m_context;
            }
        }

        public DrawingVisual Visual
        {
            get
            {
                return m_visual;
            }
        }

        public bool IsNegativeFont
        {
            get
            {
                return m_IsNegativeFont;
            }
            set
            {
                m_IsNegativeFont = value;
            }
        }

        internal float DefaultGlyphWidth
        {
            get
            {
                return m_defaultGlyphWidth;
            }
            set
            {
                m_defaultGlyphWidth = value;
            }
        }

        internal Dictionary<string, double> ReverseMapTable
        {
            get
            {
                return m_reverseMapTable;
            }
            set
            {
                m_reverseMapTable = value;
            }
        }
        #endregion

        public Dictionary<double, string> CharacterMapping
        {
            set
            {
                m_charactermapping = value;
            }
        }
        public WPFGraphics()
        {
            m_children = new VisualCollection(this);
        }

        #region Implementation
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= m_children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return m_children[index];
        }

        public void CreateGraphics(double width, double height)
        {
            m_children = new VisualCollection(this);
            m_visual = new DrawingVisual();
            m_context = m_visual.RenderOpen();

            this.Width = width;
            this.Height = height;

            this.FillRectangle(new SolidColorBrush(Colors.White), new Rect(0, 0, Width, Height));
        }

        public void FlushDrawing()
        {
            m_context.Close();

            if (!m_children.Contains(m_visual))
                m_children.Add(m_visual);
        }


        public void PushTranslateTransform(double x, double y, bool pageInit)
        {
            if (!pageInit)
                m_pushCount++;

            m_context.PushTransform(
                new TranslateTransform(x, y));
        }

        public void PushScaleTransform(double scaleX, double scaleY)
        {
            m_pushCount++;

            m_context.PushTransform(
                new ScaleTransform(scaleX, scaleY));
        }

        public void PushSkewTransform(double angleX, double angleY)
        {
            //m_pushCount++;

            m_context.PushTransform(
                new SkewTransform(angleX, angleY));
        }

        public void PushRotateTransform(double angle)
        {
            m_pushCount++;

            m_context.PushTransform(
                new RotateTransform(angle));
        }

        public void PopTransform()
        {
            m_pushCount--;

            m_context.Pop();
        }

        public double DrawString(string text, PIFont font, Color color, double wordSpacing, double characterSpacing, double textScaling,
            double x, double y, bool rtl, GlyphTypeface gtf, Dictionary<int, int> gWidths)
        {
            renderedText = string.Empty;
            double textScale = (textScaling <= 0) ? 1 : textScaling;
            double modifiedWidth = 0;
            FlowDirection flowDirection = rtl ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            Typeface typeFace = GetTypeFace(font);
            Brush textBrush = new SolidColorBrush(color);

            if (gtf == null)
            {
                typeFace.TryGetGlyphTypeface(out gtf);
            }
            double size = font.Size;
            string[] words;
            if (wordSpacing > 0)
            {
                string space = " ";
                ushort spaceIndex = gtf.CharacterToGlyphMap[space[0]];
                double spaceWidth = gtf.AdvanceWidths[spaceIndex] * size;
                spaceWidth = spaceWidth / 100 * textScaling;
                words = SplitText(text);
                wordSpacing += spaceWidth;
            }
            else
                words = new string[] { text };
            ushort[] glyphIndexes;
            double[] advanceWidths;
            double left = x;
            int wordIndex = 0;

            foreach (string word in words)
            {
                wordIndex++;
                int letterCount = 0;
                float Type1FontSpaceWidth = 0;
                foreach (char letter in word)
                {
                    letterCount++;
                    #region Type1Font
                    if (this.IsType1Font)
                    {
                        string mappedChar = letter.ToString();
                        int mappedValue = (int)letter;

                        mappedChar = letter.ToString();
                        mappedValue = (int)letter;
                        if (differenceTable.ContainsValue(word) && differenceMappedTable.ContainsValue(word))
                        {
                            mappedChar = word;
                            foreach (KeyValuePair<int, string> entry in differenceTable)
                            {
                                if (entry.Value == word)
                                {
                                    mappedValue = entry.Key;
                                    break;
                                }
                            }
                            //isWordGlyph = true;
                        }
                        else if (differenceMappedTable.ContainsValue(mappedChar))
                        {
                            foreach (KeyValuePair<string, string> entry in differenceMappedTable)
                            {
                                if (entry.Value == mappedChar)
                                {
                                    mappedValue = int.Parse(entry.Key);
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
                                    }
                                    break;
                                }
                            }
                        }
                        else if (differenceMappedTable.ContainsValue(word))
                        {
                            //isWordGlyph = true;
                            foreach (KeyValuePair<string, string> entry in differenceMappedTable)
                            {
                                if (entry.Value == word)
                                {
                                    mappedValue = int.Parse(entry.Key);
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
                                    }
                                    break;
                                }
                            }

                        }
                        else if (differenceTable.ContainsKey(mappedValue))
                        {
                            mappedChar = differenceTable[mappedValue];
                        }
                        else if (m_cffGlyphs.DifferenceEncoding.ContainsKey(mappedValue))
                        {
                            mappedChar = m_cffGlyphs.DifferenceEncoding[mappedValue];
                        }

                        double width = 0;
                        double height;
                        if (!Is1C)
                            height = type1Height * CharSizeMultiplier * size;
                        else
                            height = font.Size;
                        WPFGlyphWriter glyphWriter = new WPFGlyphWriter(m_cffGlyphs.Glyphs, this.Is1C);
                        glyphWriter.is1C = this.Is1C;

                        if (glyphWriter.glyphs.ContainsKey(mappedChar))
                        {

                            PathGeometry pathGeom;
                            if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                            {
                                pathGeom = (PathGeometry)glyphWriter.glyphParser(mappedChar, mappedValue, 500);
                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                m_context.PushTransform(new TranslateTransform(left, y + height));
                                m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                m_context.DrawGeometry(textBrush, null, pathGeom);
                                m_context.Pop();
                                m_context.Pop();
                                m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                            }
                            else
                            {
                                pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[mappedChar];
                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                m_context.PushTransform(new TranslateTransform(left, y + height));
                                m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                m_context.DrawGeometry(textBrush, null, pathGeom);
                                m_context.Pop();
                                m_context.Pop();
                            }
                        }
                        else if (glyphWriter.glyphs.ContainsKey(mappedValue.ToString()))
                        {
                            PathGeometry pathGeom;
                            if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                            {
                                pathGeom = (PathGeometry)glyphWriter.glyphParser(mappedValue.ToString(), mappedValue, 500);
                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                m_context.PushTransform(new TranslateTransform(left, y + height));
                                m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                m_context.DrawGeometry(textBrush, null, pathGeom);
                                m_context.Pop();
                                m_context.Pop();
                                m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                            }
                            else
                            {
                                pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[mappedChar];
                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                m_context.PushTransform(new TranslateTransform(left, y + height));
                                m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                m_context.DrawGeometry(textBrush, null, pathGeom);
                                m_context.Pop();
                                m_context.Pop();
                            }
                        }
                        //encoded mapped char
                        else
                        {
                            string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);
                            if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                            {
                                PathGeometry pathGeom;
                                if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                {
                                    pathGeom = glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500);
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                    m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                }
                                else
                                {
                                    pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                }
                            }
                            else
                            {
                                if (ReverseMapTable.ContainsKey(mappedChar))
                                {
                                    encodedMappedChar = ReverseMapTable[mappedChar].ToString();
                                    if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                    {
                                        PathGeometry pathGeom;
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                        {
                                            pathGeom = glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500);
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            m_context.PushTransform(new TranslateTransform(left, y + height));
                                            m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                            m_context.DrawGeometry(textBrush, null, pathGeom);
                                            m_context.Pop();
                                            m_context.Pop();
                                            m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                        }
                                        else
                                        {
                                            pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            m_context.PushTransform(new TranslateTransform(left, y + height));
                                            m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                            m_context.DrawGeometry(textBrush, null, pathGeom);
                                            m_context.Pop();
                                            m_context.Pop();
                                        }
                                    }
                                }
                            }

                        }

                        if (gWidths != null)
                        {
                            if (gWidths.ContainsKey((int)mappedValue))
                            {
                                width = gWidths[(int)mappedValue];
                                width *= (CharSizeMultiplier * size);
                                width = width / 100 * textScaling;
                            }
                            else
                            {
                                if (CharacterMapTable.Count != 0)
                                {
                                    foreach (KeyValuePair<double, string> value in CharacterMapTable)
                                    {
                                        if (value.Value.Equals(mappedChar))
                                            mappedValue = (int)value.Key;
                                    }
                                    if (gWidths.ContainsKey((int)mappedValue))
                                    {
                                        width = gWidths[(int)mappedValue];
                                        width *= (CharSizeMultiplier * size);
                                        width = width / 100 * textScaling;
                                    }
                                    else
                                    {
                                        width = DefaultGlyphWidth;
                                        width *= (CharSizeMultiplier * size);
                                        width = width / 100 * textScaling;
                                    }
                                }
                                else
                                {
                                    width = DefaultGlyphWidth;
                                    width *= (CharSizeMultiplier * size);
                                    width = width / 100 * textScaling;
                                }
                            }
                        }
                        else
                        {
                            width = DefaultGlyphWidth;
                            width *= (CharSizeMultiplier * size);
                            width = width / 100 * textScaling;
                        }
                        modifiedWidth = (float)((width / 100) * (float)textScaling);
                        if (letterCount < word.Length)
                            left += modifiedWidth + 0;
                        else
                            left += modifiedWidth;

                        if (gWidths != null)//Space after a word
                        {
                            if (gWidths.ContainsKey((int)' '))
                            {
                                Type1FontSpaceWidth = gWidths[(int)' '];
                                Type1FontSpaceWidth *= (float)(CharSizeMultiplier * size);
                                Type1FontSpaceWidth = (float)(Type1FontSpaceWidth / 100 * textScaling);
                            }
                        }
                    }
                    #endregion
                    #region NonType1Font
                    else
                    {
                        try
                        {
                            ushort glyphIndex;

                            if ((int)letter == 133 || (int)letter == 128 || ((int)letter > 144 && (int)letter < 152))
                            {
                                glyphIndexes = new ushort[MapCharacter(letter).Length];
                                advanceWidths = new double[MapCharacter(letter).Length];
                                glyphIndex = gtf.CharacterToGlyphMap[MapCharacter(letter)[0]];
                            }
                            else
                            {
                                glyphIndexes = new ushort[letter.ToString().Length];
                                advanceWidths = new double[letter.ToString().Length];
                                glyphIndex = gtf.CharacterToGlyphMap[letter.ToString()[0]];
                            }
                            glyphIndexes[0] = glyphIndex;

                            double width = gtf.AdvanceWidths[glyphIndex] * size;
                            double height = gtf.AdvanceHeights[glyphIndex] * size;
                            if (gWidths != null)
                            {
                                if (gWidths.ContainsKey((int)letter))
                                {
                                    width = gWidths[(int)letter];
                                    width *= (CharSizeMultiplier * size);
                                    width = width / 100 * textScaling;
                                }
                            }
                            advanceWidths[0] = width;

                            GlyphRun glyphRun = new GlyphRun(gtf, 0, false, size,
                                glyphIndexes, new Point(left, y + height), advanceWidths, null, null, null, null,
                                null, null);

                            if (IsNegativeFont)
                            {
                                Transform transform = new MatrixTransform(new Matrix(1, 0, 0, -1, 0, 2 * y + 2 * height));
                                m_context.PushTransform(transform);
                                m_context.DrawGlyphRun(textBrush, glyphRun);
                                m_context.Pop();

                                modifiedWidth = (width / 100) * textScaling;
                                if (modifiedWidth < 0)
                                    modifiedWidth = -modifiedWidth;
                            }
                            else
                            {
                                m_context.DrawGlyphRun(textBrush, glyphRun);
                                modifiedWidth = (width / 100) * textScaling;
                            }

                            if (letterCount < word.Length)
                                left += modifiedWidth + characterSpacing;
                            else
                                left += modifiedWidth;
                        }
                        catch (Exception)
                        {
                            FormattedText ft = new FormattedText(letter.ToString(), Thread.CurrentThread.CurrentUICulture,
                                flowDirection, typeFace, font.Size, textBrush);

                            if ((int)letter == 133 || ((int)letter > 144 && (int)letter < 152))
                            {
                                ft = new FormattedText(MapCharacter(letter), Thread.CurrentThread.CurrentUICulture, flowDirection, typeFace, font.Size, textBrush);
                            }
                            if (m_charactermapping != null && (int)letter > 127)
                            {
                                if (m_charactermapping.Count > 0)
                                {
                                    if (m_charactermapping.ContainsKey(Convert.ToDouble((int)letter)))
                                    {
                                        string unicodeletter = m_charactermapping[Convert.ToDouble((int)letter)];
                                        ft = new FormattedText(unicodeletter, Thread.CurrentThread.CurrentUICulture,
                                    flowDirection, typeFace, font.Size, textBrush);
                                    }
                                }
                            }
                            m_context.DrawText(
                                ft, new Point(left, y));
                            modifiedWidth = (ft.Width / 100) * textScaling;
                            if (letterCount < word.Length)
                                left += modifiedWidth + characterSpacing;
                            else
                                left += modifiedWidth;
                        }
                    }
                    #endregion
                }
                if (!this.IsType1Font || Type1FontSpaceWidth == 0)
                {
                    left += (wordSpacing + characterSpacing);
                }
                else
                {
                    if (wordIndex < words.Length)
                    {
                        left += Type1FontSpaceWidth;
                    }
                }
            }

            return x - left;
        }

        public double DrawStringWithSpacing(List<string> decodedList, PIFont font, Color color, double wordSpacing, double characterSpacing, double textScaling,
            double x, double y, bool rtl, GlyphTypeface gtf, Dictionary<int, int> gWidths)
        {
            renderedText = string.Empty;
            double textScale = (textScaling <= 0) ? 1 : textScaling;
            FlowDirection flowDirection = rtl ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            Typeface typeFace = GetTypeFace(font);
            Brush textBrush = new SolidColorBrush(color);

            if (gtf == null)
            {
                typeFace.TryGetGlyphTypeface(out gtf);
            }

            float textSpace;
            double modifiedWidth = 0;
            ushort[] glyphIndexes;
            double[] advanceWidths;
            double left = x;
            float size = font.Size;
            foreach (string word in decodedList)
            {
                if (float.TryParse(word, out textSpace))
                {
                    float textSize = font.Size;
                    textSpace = textSpace * (textSize / 1000);
                    textSpace = (float)(textSpace - characterSpacing);
                    left -= textSpace;
                }
                else
                {
                    string text = word.Remove(word.Length - 1, 1);
                    int letterCount = 0;
                    foreach (char letter in text)
                    {
                        letterCount++;
                        #region Type1Font
                        if (this.IsType1Font)
                        {
                            string mappedChar = letter.ToString();
                            int mappedValue = (int)letter;

                            mappedChar = letter.ToString();
                            mappedValue = (int)letter;
                            if (differenceTable.ContainsValue(word) && differenceMappedTable.ContainsValue(word))
                            {
                                mappedChar = word;
                                foreach (KeyValuePair<int, string> entry in differenceTable)
                                {
                                    if (entry.Value == word)
                                    {
                                        mappedValue = entry.Key;
                                        break;
                                    }
                                }
                                //isWordGlyph = true;
                            }
                            else if (differenceMappedTable.ContainsValue(mappedChar))
                            {
                                foreach (KeyValuePair<string, string> entry in differenceMappedTable)
                                {
                                    if (entry.Value == mappedChar)
                                    {
                                        mappedValue = int.Parse(entry.Key);
                                        if (differenceTable.ContainsKey(mappedValue))
                                        {
                                            mappedChar = differenceTable[mappedValue];
                                        }
                                        break;
                                    }
                                }
                            }
                            else if (differenceMappedTable.ContainsValue(word))
                            {
                                //isWordGlyph = true;
                                foreach (KeyValuePair<string, string> entry in differenceMappedTable)
                                {
                                    if (entry.Value == word)
                                    {
                                        mappedValue = int.Parse(entry.Key);
                                        if (differenceTable.ContainsKey(mappedValue))
                                        {
                                            mappedChar = differenceTable[mappedValue];
                                        }
                                        break;
                                    }
                                }

                            }
                            else if (differenceTable.ContainsKey(mappedValue))
                            {
                                mappedChar = differenceTable[mappedValue];
                            }
                            else if (m_cffGlyphs.DifferenceEncoding.ContainsKey(mappedValue))
                            {
                                mappedChar = m_cffGlyphs.DifferenceEncoding[mappedValue];
                            }

                            //Drawing the shape
                            double width = 0;
                            double height1;
                            if (!Is1C)
                                height1 = type1Height * CharSizeMultiplier * size;
                            else
                                height1 = font.Size;
                            WPFGlyphWriter glyphWriter = new WPFGlyphWriter(m_cffGlyphs.Glyphs, this.Is1C);
                            glyphWriter.is1C = this.Is1C;
                            if (glyphWriter.glyphs.ContainsKey(mappedChar))
                            {
                                PathGeometry pathGeom;
                                PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                {
                                    pathGeom = glyphWriter.glyphParser(mappedChar, mappedValue, 500);
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height1));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                    m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                                }
                                else
                                {
                                    pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[mappedChar];
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height1));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                }
                            }
                            else if (glyphWriter.glyphs.ContainsKey(mappedValue.ToString()))
                            {
                                PathGeometry pathGeom;
                                PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                {
                                    pathGeom = glyphWriter.glyphParser(mappedValue.ToString(), mappedValue, 500);
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height1));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                    m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                                }
                                else
                                {
                                    pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[mappedChar];
                                    double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                    m_context.PushTransform(new TranslateTransform(left, y + height1));
                                    m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                    m_context.DrawGeometry(textBrush, null, pathGeom);
                                    m_context.Pop();
                                    m_context.Pop();
                                }
                            }
                            else
                            {
                                string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                {
                                    PathGeometry pathGeom;
                                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                    {
                                        pathGeom = glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500);
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        m_context.PushTransform(new TranslateTransform(left, y + height1));
                                        m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                        m_context.DrawGeometry(textBrush, null, pathGeom);
                                        m_context.Pop();
                                        m_context.Pop();
                                        m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                    }
                                    else
                                    {
                                        pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        m_context.PushTransform(new TranslateTransform(left, y + height1));
                                        m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                        m_context.DrawGeometry(textBrush, null, pathGeom);
                                        m_context.Pop();
                                        m_context.Pop();
                                    }
                                }
                                //CID KEYED 
                                else if (glyphWriter.glyphs.ContainsKey(mappedValue.ToString()))
                                {
                                    PathGeometry pathGeom;
                                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                    {
                                        pathGeom = glyphWriter.glyphParser(mappedValue.ToString(), mappedValue, 500);
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        m_context.PushTransform(new TranslateTransform(left, y + height1));
                                        m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                        m_context.DrawGeometry(textBrush, null, pathGeom);
                                        m_context.Pop();
                                        m_context.Pop();
                                        m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                    }
                                    else
                                    {
                                        pathGeom = (PathGeometry)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        m_context.PushTransform(new TranslateTransform(left, y + height1));
                                        m_context.PushTransform(new ScaleTransform(scaleFactor, scaleFactor));
                                        m_context.DrawGeometry(textBrush, null, pathGeom);
                                        m_context.Pop();
                                        m_context.Pop();
                                    }

                                }
                            }
                            if (gWidths != null)
                            {
                                if (gWidths.ContainsKey((int)mappedValue))
                                {
                                    width = gWidths[(int)mappedValue];
                                    width *= (CharSizeMultiplier * size);
                                    width = width / 100 * textScaling;
                                }
                                else
                                {
                                    if (CharacterMapTable.Count != 0)
                                    {
                                        foreach (KeyValuePair<double, string> value in CharacterMapTable)
                                        {
                                            if (value.Value.Equals(mappedChar))
                                                mappedValue = (int)value.Key;
                                        }
                                        if (gWidths.ContainsKey((int)mappedValue))
                                        {
                                            width = gWidths[(int)mappedValue];
                                            width *= (CharSizeMultiplier * size);
                                            width = width / 100 * textScaling;
                                        }
                                        else
                                        {
                                            width = DefaultGlyphWidth;
                                            width *= (CharSizeMultiplier * size);
                                            width = width / 100 * textScaling;
                                        }
                                    }
                                    else
                                    {
                                        width = DefaultGlyphWidth;
                                        width *= (CharSizeMultiplier * size);
                                        width = width / 100 * textScaling;
                                    }
                                }
                            }
                            else
                            {
                                width = DefaultGlyphWidth;
                                width *= (CharSizeMultiplier * size);
                                width = width / 100 * textScaling;
                            }

                            modifiedWidth = (float)((width / 100) * (float)textScaling);
                            if (letterCount < text.Length)
                                left += modifiedWidth + characterSpacing;
                            else
                                left += modifiedWidth;

                        }
                        #endregion
                        #region NonType1Fonts
                        else
                        {
                            try
                            {
                                renderedText += letter.ToString();
                                //letterCount++;
                                ushort glyphIndex;
                                if ((int)letter == 133 || ((int)letter > 144 && (int)letter < 152))
                                {
                                    string mappedChar = MapCharacter(letter);
                                    glyphIndexes = new ushort[mappedChar.Length];
                                    advanceWidths = new double[mappedChar.Length];
                                    glyphIndex = gtf.CharacterToGlyphMap[mappedChar[0]];
                                }
                                else
                                {
                                    glyphIndexes = new ushort[letter.ToString().Length];
                                    advanceWidths = new double[letter.ToString().Length];
                                    glyphIndex = gtf.CharacterToGlyphMap[letter.ToString()[0]];
                                }
                                glyphIndexes[0] = glyphIndex;

                                double width = gtf.AdvanceWidths[glyphIndex] * font.Size;
                                double height = gtf.AdvanceHeights[glyphIndex] * font.Size;
                                if (gWidths != null)
                                {
                                    if (gWidths.ContainsKey((int)letter))
                                    {
                                        width = gWidths[(int)letter];
                                        width *= (CharSizeMultiplier * font.Size);
                                        width = width / 100 * textScaling;
                                    }
                                }

                                advanceWidths[0] = width;

                                GlyphRun glyphRun = new GlyphRun(gtf, 0, false, font.Size,
                                    glyphIndexes, new Point(left, y + height), advanceWidths, null, null, null, null,
                                    null, null);

                                if (IsNegativeFont)
                                {
                                    Transform transform = new MatrixTransform(new Matrix(1, 0, 0, -1, 0, 2 * y + 2 * height));
                                    m_context.PushTransform(transform);
                                    m_context.DrawGlyphRun(textBrush, glyphRun);
                                    m_context.Pop();

                                    modifiedWidth = (width / 100) * textScaling;
                                    if (modifiedWidth < 0)
                                        modifiedWidth = -modifiedWidth;
                                }
                                else
                                {
                                    m_context.DrawGlyphRun(textBrush, glyphRun);
                                    if (textScaling != 100)
                                        modifiedWidth = width;
                                    else
                                        modifiedWidth = (width / 100) * textScaling;
                                }
                                if (letter.ToString() == " ")
                                    modifiedWidth += wordSpacing;
                                if (letterCount < word.Length)
                                    left += modifiedWidth + characterSpacing;
                                else
                                    left += modifiedWidth;
                            }
                            catch (Exception)
                            {
                                FormattedText ft = new FormattedText(letter.ToString(), Thread.CurrentThread.CurrentUICulture,
                                    flowDirection, typeFace, font.Size, textBrush);

                                if ((int)letter == 133 || ((int)letter > 144 && (int)letter < 152))
                                {
                                    ft = new FormattedText(MapCharacter(letter), Thread.CurrentThread.CurrentUICulture, flowDirection, typeFace, font.Size, textBrush);
                                }
                                m_context.DrawText(
                                    ft, new Point(left, y));
                                modifiedWidth = (ft.Width / 100) * textScaling;
                                if (letterCount < word.Length)
                                    left += modifiedWidth + characterSpacing;
                                else
                                    left += modifiedWidth;
                            }
                        }
                        #endregion
                    }
                }
            }
            return x - left;
        }

        public void DrawRectangle(string brush, string pen, double penWidth, Rect bounds)
        {
            Brush fill = null;
            Pen borderPen = null;

            if (brush != null)
                fill = GetBrushFromColorString(brush);

            if (pen != null)
                borderPen = GetPenFromColorString(pen, penWidth);

            m_context.DrawRectangle(fill, borderPen, bounds);
        }

        public void DrawRectangle(Pen pen, Rect bounds)
        {
            m_context.DrawRectangle(null, pen, bounds);
        }

        public void FillRectangle(Brush brush, Rect bounds)
        {
            m_context.DrawRectangle(brush, null, bounds);
        }

        public void DrawPath(Pen pen, PathFigure figure)
        {
            PathGeometry path = new PathGeometry();
            path.Figures.Add(figure);

            DrawPath(pen, path);
        }

        public void DrawPath(Pen pen, PathGeometry path)
        {
            m_context.DrawGeometry(null, pen, path);
        }

        public void PushClip(Rect clipBounds)
        {
            m_pushCount++;
            RectangleGeometry geometry = new RectangleGeometry(clipBounds);

            m_context.PushClip(geometry);
        }

        public void PushClip(PathFigure figure)
        {
            PathGeometry path = new PathGeometry();
            Point startPoint = figure.StartPoint;
            if (figure != null)
                path.Figures.Add(figure);
            //if (startPoint.Y >= 0)
            {
                m_pushCount++;
                m_context.PushClip(path);
            }
        }
        public void Push()
        {
            transformations.Push(m_pushCount);
            m_pushCount = 0;
        }

        public void Pop()
        {
            int transformationsDone = m_pushCount;//transformations.Pop();
            for (int i = 0; i < transformationsDone; i++)
            {
                m_context.Pop();
            }
            m_pushCount = transformations.Pop();
        }

        public void FillPath(Brush brush, PathFigure figure)
        {
            PathGeometry path = new PathGeometry();
            path.Figures.Add(figure);

            FillPath(brush, path);
        }

        public void FillPath(Brush brush, PathGeometry path)
        {
            m_context.DrawGeometry(brush, null, path);
        }

        public void DrawLine(Pen pen, double x1, double y1, double x2, double y2)
        {
            m_context.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
        }

        public void DrawImage(Stream imageStream, Rect bounds)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                imageStream.Position = 0;
                bitmap.StreamSource = imageStream;
                bitmap.EndInit();
                m_context.DrawImage(bitmap, bounds);
                bitmap = null;
            }
            catch (Exception exce)
            {
                PdfViewerExceptions exceptions = new PdfViewerExceptions();
                exceptions.Exceptions.Append("Exception in DrawImage()+\r\n" + exce.StackTrace + "\r\n");
            }
        }
        #endregion

        #region Helper Methods
        string[] SplitText(string text)
        {
            return text.Split(null);
        }

        FormattedText GetFormattedText(string text, PIFont font, string color, bool rtl)
        {
            FlowDirection flowDirection = rtl ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            Typeface typeFace = GetTypeFace(font);
            Brush textBrush = GetBrushFromColorString(color);

            FormattedText formattedText = new FormattedText(
                text,
                Thread.CurrentThread.CurrentUICulture,
                flowDirection,
                typeFace,
                font.Size,
                textBrush);

            return formattedText;
        }

        internal Typeface GetTypeFace(PIFont font)
        {

            Typeface typeFace = new Typeface(
                new FontFamily((font.Name == "Helvetica" ? font.Name = "Arial" : font.Name)),
                GetFontStyle(font.Style),
                GetFontWeight(font.Style),
                FontStretches.Normal,
                new FontFamily("Times New Roman") //Fallback font.
                );

            return typeFace;
        }

        FontStyle GetFontStyle(PIFontStyle style)
        {
            PIFontStyle newstyle = PIFontStyle.Bold;
            newstyle |= PIFontStyle.Italic;

            if (style == PIFontStyle.Italic || style == newstyle)
                return FontStyles.Italic;
            else
                return FontStyles.Normal;

        }

        FontWeight GetFontWeight(PIFontStyle style)
        {
            PIFontStyle newstyle = PIFontStyle.Bold;
            newstyle |= PIFontStyle.Italic;

            if (style == PIFontStyle.Bold || style == newstyle)
                return FontWeights.Bold;
            else
                return FontWeights.Regular;
        }

        Color GetColorFromString(string clr)
        {
            Color color;
            try
            {
                color = (Color)ColorConverter.ConvertFromString(clr);
            }
            catch (Exception)
            {
                color = Colors.Black;
            }

            return color;
        }

        Brush GetBrushFromColorString(string clr)
        {
            return new SolidColorBrush(
                GetColorFromString(clr));
        }

        Pen GetPenFromColorString(string clr, double thickness)
        {
            return new Pen(
                GetBrushFromColorString(clr),
                thickness);
        }

        private string MapCharacter(char smartQuoteChar)
        {
            switch ((int)smartQuoteChar)
            {
                case 128:
                    {
                        return (@"€");
                    }
                case 133:
                    {
                        return (@"…");
                    }
                case 145:
                    {
                        return (@"‘");
                    }
                case 146:
                    {
                        return (@"’");
                    }
                case 147:
                    {
                        return (@"“");
                    }
                case 148:
                    {
                        return (@"”");
                    }
                case 150:
                    {
                        return (@"–");
                    }
                case 151:
                    {
                        return (@"—");
                    }
                default:
                    {
                        return (smartQuoteChar.ToString());
                    }
            }
        }
        #endregion
    }
}
