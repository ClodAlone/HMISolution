#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Windows.Foundation;
#if SyncfusionFramework4_5
using Syncfusion.DirectXWrapper.WinRT;
namespace Syncfusion.PdfViewer.Base
{
    internal class TextElement
    {
        internal int textRenderingMode = 0;
        internal bool IsType1Font = false;
        internal bool IsC1 = false;
        internal Dictionary<int, string> differenceTable = new Dictionary<int, string>();
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();
        internal Dictionary<string, string> differenceMappedTable = new Dictionary<string, string>();
        float CharSizeMultiplier = 0.001f;
        internal string m_fontName;
        internal Syncfusion.DirectXWrapper.WinRT.FontStyle m_fontStyle;
        internal float m_fontSize;
        internal string m_fontEncoding;
        private float m_customFontSize;
        internal string m_text;
        internal global::Windows.UI.Color m_brushColor;
        internal float m_wordSpacing;
        internal float m_characterSpacing;
        internal float m_textScaling = 100;
        private FontFace m_font;
        internal bool isNegativeFont;
        internal Syncfusion.DirectXWrapper.WinRT.FontWeight m_fontWeight;
        internal static Dictionary<global::Windows.UI.Color, GraphicBrush> m_graphicsBrushes = new Dictionary<global::Windows.UI.Color, GraphicBrush>();
        //TextBlock m_textBlock = new TextBlock();
        TextLayout textLayout = null;
        // TODO - To implement in wrapper.
        TextLayout1 textLayout1 = null;
        private static Dictionary<string, string> fontList = new Dictionary<string, string>();
        private Dictionary<string, double> m_reverseMapTable;
        internal Dictionary<int, int> FontGlyphWidths;
        private int m_defaultWidth;
        private bool IsTexRotated = false;
        internal Dictionary<Double, string> CharacterMapTable;

        //  Table to convert the Mac OS Roman characters 0x80-0xFF to Unicode
        long[] MacRomanToUnicode ={
          196  ,  197,  199,  201,  209,  214,  220,  225,  224,  226,  228,  227,  229,  231,   233,   232,
          234  ,  235,  237,  236,  238,  239,  241,  243,  242,  244,  246,  245,  250,  249,   251,   252,
          8224 ,  176,  162,  163,  167, 8226,  182,  223,  174,  169, 8482,  180,  168, 8800,   198,   216,
          8734 ,  177, 8804, 8805,  165,  181, 8706, 8721, 8719,  960, 8747,  170,  186,  937,   230,   248,
          191  ,  161,  172, 8730,  402, 8776, 8710,  171,  187, 8230,  160,  192,  195,  213,   338,   339,
          8211 , 8212, 8220, 8221, 8216, 8217,  247, 9674,  255,  376, 8260, 8364, 8249, 8250, 64257, 64258,
          8225 ,  183, 8218, 8222, 8240,  194,  202,  193,  203,  200,  205,  206,  207,  204,   211,   212,
          63743,  210,  218,  219,  217,  305,  710,  732,  175,  728,  729,  730,  184,  733,   731,   711  };

        internal TextElement(string text)
        {
            m_text = text;
        }

        ~TextElement()
        {
            if (graphics != null)
                graphics.Dispose();
            graphics = null;

            m_font = null;
            m_text = string.Empty;
        }

        internal string FontName
        {
            get
            {
                return m_fontName;
            }
            set
            {
                m_fontName = value;
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

        internal Syncfusion.DirectXWrapper.WinRT.FontWeight FontWeight
        {
            get
            {
                return m_fontWeight;
            }
            set
            {
                m_fontWeight = value;
            }
        }
        internal FontFace Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        internal Syncfusion.DirectXWrapper.WinRT.FontStyle FontStyle
        {
            get
            {
                return m_fontStyle;
            }
            set
            {
                m_fontStyle = value;
            }
        }

        internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }

        internal string FontEncoding
        {
            get
            {
                return m_fontEncoding;
            }
            set
            {
                m_fontEncoding = value;
            }
        }

        internal float CustomFontSize
        {
            get
            {
                return m_customFontSize;
            }
            set
            {
                m_customFontSize = value;
            }
        }

        internal string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        internal global::Windows.UI.Color BrushColor
        {
            get
            {
                return m_brushColor;
            }
            set
            {
                m_brushColor = value;
            }
        }

        internal float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }
            set
            {
                m_wordSpacing = value;
            }
        }

        internal float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }
            set
            {
                m_characterSpacing = value;
            }
        }
        internal float TextScaling
        {
            get
            {
                return m_textScaling;
            }
            set
            {
                m_textScaling = value;
            }

        }

        internal int DefaultWidth
        {
            get
            {
                return m_defaultWidth;
            }
            set
            {
                m_defaultWidth = value;
            }
        }

        internal Dictionary<double, string> CidToGidMap
        {
            get
            {
                return m_CidToGidMap;
            }
            set
            {
                m_CidToGidMap = value;
            }
        }
        Graphics graphics = new Graphics();
        GraphicBrush textBrush;
        TextFormat textFormat;
        private Dictionary<double, string> m_CidToGidMap;

        /// <summary>
        /// Renders the Text to the panel
        /// </summary>
        /// <param name="g">graphics element</param>
        /// <param name="currentLocation">location in which the graphics is to be drawn</param>
        internal float Render(out bool textRotated, Graphics2D g, System.Drawing.PointF currentLocation, global::Windows.UI.Color StrokingColorSpace)
        {
            if (StrokingColorSpace == new global::Windows.UI.Color())
            {
                BrushColor = global::Windows.UI.Colors.Black;
            }
            else
            {
                BrushColor = StrokingColorSpace;
            }
            if (textRenderingMode == 3)
                BrushColor = global::Windows.UI.Colors.Transparent;
            string fontName = FontName;
            int wordIndex;
            //textFont = null;
            float changeInX = currentLocation.X;
            if (FontSize < 0)
            {
                FontSize = -FontSize;
            }
            if (CustomFontSize < 0)
            {
                CustomFontSize = -CustomFontSize;
            }
            string[] words;
            if (this.Font != null)
                words = new string[] { this.Text };
            else
                words = this.Text.Split(' ');

            //graphics = new Graphics();
            textLayout = new TextLayout(Text, new TextFormat(FontName, CustomFontSize), 400, 300);
            TextLayout1 textlayout1 = new TextLayout1(textLayout);
            PointF location = currentLocation;
            wordIndex = 0;
            if (!m_graphicsBrushes.ContainsKey(BrushColor))
            {
                textBrush = new GraphicBrush(BrushColor);
                m_graphicsBrushes.Add(BrushColor, textBrush);
            }
            else
                textBrush = m_graphicsBrushes[BrushColor];
            textFormat = new TextFormat(FontName, CustomFontSize, FontStyle, FontWeight);

            float spaceWidth;
            textLayout = new TextLayout(" ", textFormat, 400, 300);
            TextMetrics textMetrics = textLayout.GetTextMetrics();
            if (FontGlyphWidths != null && FontGlyphWidths.ContainsKey(32))
                spaceWidth = FontGlyphWidths[32] * FontSize * CharSizeMultiplier;
            else
                spaceWidth = textMetrics.WidthIncludingTrailingWhitespace;
            if (this.Font != null)
            {
                int lengthOfStringToBeRendered;
                int wordsLength = words.Length;
                foreach (string word in words)
                {
                    string stringToBeRendered;
                    stringToBeRendered = word;
                    lengthOfStringToBeRendered = stringToBeRendered.Length;
                    float modifiedWidth = 0;
                    wordIndex++;

                    Syncfusion.DirectXWrapper.WinRT.Matrix mtrx = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);

                    int[] codePoints = new int[lengthOfStringToBeRendered];

                    char[] charsToBeRendered = stringToBeRendered.ToCharArray();
                    int lenghtOfCharactersToBeRendered = charsToBeRendered.Length;
                    for (int i = 0; i < lenghtOfCharactersToBeRendered; i++)
                    {
                        codePoints[i] = (int)charsToBeRendered[i];
                    }

                    short[] glyphIndices = new short[codePoints.Length];
                    this.Font.GetGlyphIndices(codePoints, out glyphIndices);

                    GlyphMetrics[] metrics = new GlyphMetrics[codePoints.Length];
                    this.Font.GetGdiCompatibleGlyphMetrics(this.FontSize, 1, mtrx, true, glyphIndices, false, out metrics);

                    SizeF actualSize = new SizeF();
                    actualSize.Width = metrics[0].AdvanceWidth;
                    actualSize.Height = textMetrics.Height;
                    int letterCount = 0;

                    foreach (char letter in stringToBeRendered)
                    {
                        bool isWordGlyph = false;
                        string mappedChar = string.Empty;
                        int mappedValue = 0;
                        if (this.IsType1Font)
                        {
                            Syncfusion.Pdf.GlyphWriter gw = new Syncfusion.Pdf.GlyphWriter(m_cffGlyphs.Glyphs, g, IsC1);
                            float xScaleFactor;
                            float yScaleFactor;
                            if (IsC1)
                            {
                                xScaleFactor = (float)FontSize * CharSizeMultiplier;
                                yScaleFactor = (float)FontSize * CharSizeMultiplier;
                            }
                            else
                            {
                                xScaleFactor = (float)m_cffGlyphs.FontMatrix[0] * (float)FontSize;
                                yScaleFactor = (float)m_cffGlyphs.FontMatrix[3] * (float)FontSize;
                            }
                            Syncfusion.DirectXWrapper.WinRT.Matrix temp = g.Transform;

                            float height = FontSize;
                            g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor * g.Transform.M11, g.Transform.M12, g.Transform.M21, yScaleFactor * g.Transform.M22, (location.X * g.Transform.M11 + g.Transform.OffsetX), g.Transform.OffsetY + (location.Y + height) * g.Transform.M22);
                            mappedChar = letter.ToString();
                            mappedValue = (int)letter;
                            if (differenceTable.ContainsValue(word) && differenceMappedTable.ContainsValue(word))
                            {
                                mappedChar = word;
                                mappedValue = differenceTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault();
                                isWordGlyph = true;
                            }
                            else if (differenceMappedTable.ContainsValue(mappedChar))
                            {
                                mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == mappedChar).Select(kvp => kvp.Key).FirstOrDefault());
                                if (differenceTable.ContainsKey(mappedValue))
                                {
                                    mappedChar = differenceTable[mappedValue];
                                }
                            }
                            else if (differenceMappedTable.ContainsValue(word))
                            {
                                isWordGlyph = true;
                                mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault());
                                if (differenceTable.ContainsKey(mappedValue))
                                {
                                    mappedChar = differenceTable[mappedValue];
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

                            if (gw.glyphs.ContainsKey(mappedChar))
                            {
                                GeometryGroup path;
                                if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                {
                                    path = gw.glyphParser(mappedChar, mappedValue, 50);
                                    m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                    g.FillGeometry(path, new GraphicBrush(BrushColor));
                                }
                                else
                                {
                                    path = m_cffGlyphs.RenderedPath[mappedChar];
                                    g.FillGeometry(path, new GraphicBrush(BrushColor));
                                }
                            }
                            else if (gw.glyphs.ContainsKey(mappedValue.ToString()))
                            {
                                GeometryGroup path;
                                if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                {
                                    path = gw.glyphParser(mappedValue.ToString(), mappedValue, 50);
                                    m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                    g.FillGeometry(path, new GraphicBrush(BrushColor));
                                }
                                else
                                {
                                    path = m_cffGlyphs.RenderedPath[mappedChar];
                                    g.FillGeometry(path, new GraphicBrush(BrushColor));
                                }
                            }
                            else
                            {
                                string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                if (gw.glyphs.ContainsKey(encodedMappedChar))
                                {
                                    GeometryGroup path;
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                    {
                                        path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                        m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                    }
                                    else
                                    {
                                        path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                    }
                                    g.FillGeometry(path, new GraphicBrush(BrushColor));
                                }
                                else
                                {
                                    if (ReverseMapTable.ContainsKey(mappedChar))
                                    {
                                        encodedMappedChar = ReverseMapTable[encodedMappedChar].ToString();
                                        if (gw.glyphs.ContainsKey(encodedMappedChar))
                                        {
                                            GeometryGroup path;
                                            if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                            {
                                                path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                                m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                            }
                                            else
                                            {
                                                path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            }
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                    }
                                }
                            }
                            g.Transform = temp;

                            float width1 = 0;

                            if (FontGlyphWidths != null)
                            {
                                if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                {
                                    width1 = FontGlyphWidths[(int)mappedValue];
                                    if (IsC1)
                                        width1 *= FontSize * CharSizeMultiplier;
                                    else
                                        width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                    width1 = width1 / 100 * TextScaling;
                                }
                                else if (FontGlyphWidths.ContainsKey((int)letter))
                                {
                                    width1 = FontGlyphWidths[(int)letter];
                                    if (IsC1)
                                        width1 *= FontSize * CharSizeMultiplier;
                                    else
                                        width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                    width1 = width1 / 100 * TextScaling;
                                }
                                if (CharacterMapTable.Count != 0)
                                {
                                    foreach (KeyValuePair<double, string> value in CharacterMapTable)
                                    {
                                        if (value.Value.Equals(mappedChar))
                                            mappedValue = (int)value.Key;
                                    }
                                    if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                    {
                                        width1 = FontGlyphWidths[(int)mappedValue];
                                        width1 *= (CharSizeMultiplier * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                    else
                                    {
                                        width1 = DefaultWidth;
                                        width1 *= (CharSizeMultiplier * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                }
                                else
                                {
                                    width1 = DefaultWidth;
                                    width1 *= (CharSizeMultiplier * FontSize);
                                    width1 = width1 / 100 * TextScaling;
                                }
                            }
                            else
                            {
                                width1 = DefaultWidth;
                                width1 *= (CharSizeMultiplier * FontSize);
                                width1 = width1 / 100 * TextScaling;
                            }

                            modifiedWidth = (width1 / 100) * TextScaling;
                            if (letterCount < word.Length)
                                location.X += modifiedWidth + CharacterSpacing;
                            else
                                location.X += modifiedWidth;
                            if (isWordGlyph)
                                break;
                        }
                        else
                        {
                            letterCount++;
                            GlyphRun glyphRun = new GlyphRun();
                            glyphRun.FontFace = this.Font;
                            glyphRun.FontSize = FontSize;

                            if (CidToGidMap != null)
                            {
                                if (CidToGidMap.Count > 0)
                                    if (m_CidToGidMap.ContainsKey((int)letter))
                                    {
                                        string mappingString = m_CidToGidMap[(int)letter];
                                        stringToBeRendered = mappingString;
                                    }
                            }
                            else
                                stringToBeRendered = letter.ToString();
                            lengthOfStringToBeRendered = stringToBeRendered.Length;
                            codePoints = new int[lengthOfStringToBeRendered];

                            charsToBeRendered = stringToBeRendered.ToCharArray();
                            lenghtOfCharactersToBeRendered = charsToBeRendered.Length;
                            for (int i = 0; i < lenghtOfCharactersToBeRendered; i++)
                            {
                                codePoints[i] = (int)charsToBeRendered[i];
                            }

                            this.Font.GetGlyphIndices(codePoints, out glyphIndices);//{ 43 };
                            glyphRun.Indices = glyphIndices;

                            float ascentAdjustment = (((float)glyphRun.FontFace.Metrics.Ascent) / 1000) + (((float)glyphRun.FontFace.Metrics.Descent) / 1000);
                            ascentAdjustment = FontSize;
                            g.DrawGlyphRun(new global::Windows.Foundation.Point(location.X, location.Y + ascentAdjustment), glyphRun, new GraphicBrush(BrushColor), MeasuringMode.GdiClassic);
                            mtrx = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, location.X, location.Y);

                            metrics = new GlyphMetrics[codePoints.Length];
                            this.Font.GetGdiCompatibleGlyphMetrics(this.FontSize, 1, mtrx, true, glyphIndices, false, out metrics);

                            SizeF letterSize = new SizeF();

                            if (metrics != null)
                            {
                                letterSize.Width = metrics[0].LeftSideBearing;
                                letterSize.Height = metrics[0].AdvanceHeight;
                            }

                            if (FontGlyphWidths != null)
                            {
                                if (FontGlyphWidths.ContainsKey((int)letter))
                                {
                                    letterSize.Width = FontGlyphWidths[(int)letter];
                                    letterSize.Width *= (CharSizeMultiplier * glyphRun.FontSize);
                                }
                            }
                            modifiedWidth = letterSize.Width;

                            if (letterCount < lengthOfStringToBeRendered)
                                location.X += modifiedWidth + CharacterSpacing;
                            else
                                location.X += modifiedWidth;

                            if (letter == ' ')
                                location.X += (WordSpacing + CharacterSpacing);
                        }
                    }
                    if (wordIndex < wordsLength)
                        location.X += (WordSpacing + CharacterSpacing);
                }
            }
            else
            {
                Rect textRect;
                //DrawingStateBlock tempGS;
                SizeF letterSize = new SizeF();

                if (CharacterSpacing == 0 && WordSpacing == 0 && this.FontGlyphWidths == null)
                {
                    textRect = new Rect(location.X, location.Y, (int)1000, (int)1000);
                    textLayout = new TextLayout(Text, textFormat, 1400, 1300);
                    textMetrics = textLayout.GetTextMetrics();
                    if (isNegativeFont)
                    {
                        //letterSize.Width = textMetrics.Width;
                        //letterSize.Height = textMetrics.Height;
                        //tempGS = g.SaveDrawingState();
                        Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                        g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                        g.DrawText(textLayout, textBrush, new Rect(location.X, -(location.Y + FontSize * 2), 1, 1));
                        //g.RestoreDrawingState(tempGS);
                        g.Transform = tempMatrix;
                    }
                    else
                    {
                        g.DrawText(textLayout, textBrush, new Rect(location.X, location.Y, 1, 1));
                    }
                    location.X += (textMetrics.Width / 100) * TextScaling;
                }
                else if ((this.FontGlyphWidths == null || this.FontGlyphWidths.Count == 0)&&!this.IsType1Font)
                {
                    int wordsLength = words.Length;
                    int wordLength;
                    int letterCount = 0;
                    // TODO - To implement in wrapper
                    foreach (string word in words)
                    {
                        float modifiedWidth = 0;
                        wordIndex++;

                        foreach (char letter in word)
                        {
                            letterCount++;
                            string mappedChar = string.Empty;
                            int mappedValue = 0;
                            bool isWordGlyph = false;
                            if (this.IsType1Font)
                            {
                                Syncfusion.Pdf.GlyphWriter gw = new Syncfusion.Pdf.GlyphWriter(m_cffGlyphs.Glyphs, g, this.IsC1);
                                float xScaleFactor;
                                float yScaleFactor;
                                if (IsC1)
                                {
                                    xScaleFactor = (float)FontSize * CharSizeMultiplier;
                                    yScaleFactor = (float)FontSize * CharSizeMultiplier;
                                }
                                else
                                {
                                    xScaleFactor = (float)m_cffGlyphs.FontMatrix[0] * (float)FontSize;
                                    yScaleFactor = (float)m_cffGlyphs.FontMatrix[3] * (float)FontSize;
                                }
                                Syncfusion.DirectXWrapper.WinRT.Matrix temp = g.Transform;

                                float height = FontSize;
                                g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor * g.Transform.M11, g.Transform.M12, g.Transform.M21, yScaleFactor * g.Transform.M22, (location.X * g.Transform.M11 + g.Transform.OffsetX), g.Transform.OffsetY + (location.Y + height) * g.Transform.M22);
                                mappedChar = letter.ToString();
                                mappedValue = (int)letter;
                                if (differenceTable.ContainsValue(word) && differenceMappedTable.ContainsValue(word))
                                {
                                    mappedChar = word;
                                    mappedValue = differenceTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault();
                                    isWordGlyph = true;
                                }
                                else if (differenceMappedTable.ContainsValue(mappedChar))
                                {
                                    mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == mappedChar).Select(kvp => kvp.Key).FirstOrDefault());
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
                                    }
                                }
                                else if (differenceMappedTable.ContainsValue(word))
                                {
                                    isWordGlyph = true;
                                    mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault());
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
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

                                if (gw.glyphs.ContainsKey(mappedChar))
                                {
                                    GeometryGroup path;
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                    {
                                        path = gw.glyphParser(mappedChar, mappedValue, 50);
                                        m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                    else
                                    {
                                        path = m_cffGlyphs.RenderedPath[mappedChar];
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                }
                                else
                                {
                                    string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                    if (gw.glyphs.ContainsKey(encodedMappedChar))
                                    {
                                        GeometryGroup path;
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                        {
                                            path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                            m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                        }
                                        else
                                        {
                                            path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                        }
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                    else
                                    {
                                        Dictionary<string, double> reverseMappingTable = new Dictionary<string, double>();
                                        foreach (KeyValuePair<double, string> obj in CharacterMapTable)
                                        {
                                            reverseMappingTable.Add(obj.Value, obj.Key);
                                        }
                                        if (reverseMappingTable.ContainsKey(mappedChar))
                                        {
                                            encodedMappedChar = reverseMappingTable[encodedMappedChar].ToString();
                                            GeometryGroup path;
                                            if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                            {
                                                path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                                m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                            }
                                            else
                                            {
                                                path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            }
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                    }
                                }
                                g.Transform = temp;

                                float width1 = 0;
                                if (FontGlyphWidths != null)
                                {
                                    if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                    {
                                        width1 = FontGlyphWidths[(int)mappedValue];
                                        if (IsC1)
                                            width1 *= FontSize * CharSizeMultiplier;
                                        else
                                            width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                    else if (FontGlyphWidths.ContainsKey((int)letter))
                                    {
                                        width1 = FontGlyphWidths[(int)letter];
                                        if (IsC1)
                                            width1 *= FontSize * CharSizeMultiplier;
                                        else
                                            width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                }
                                modifiedWidth = (width1 / 100) * TextScaling;
                                if (letterCount < word.Length)
                                    location.X += modifiedWidth + CharacterSpacing;
                                else
                                    location.X += modifiedWidth;
                                if (isWordGlyph)
                                    break;
                            }

                        }
                        if (!this.IsType1Font)
                        {
                            wordLength = word.Length;
                            location.X += CharacterSpacing;

                            textRect = new Rect(location.X, location.Y, (int)1000, (int)1000);

                            textLayout = new TextLayout(word.ToString(), textFormat, 400, 300);
                            textMetrics = textLayout.GetTextMetrics();
                            letterSize.Width = textMetrics.Width;
                            letterSize.Height = textMetrics.Height;

                            textLayout1 = new TextLayout1(textLayout);
                            TextRange textRange = new TextRange();
                            textRange.StartPosition = 0;
                            textRange.Length = 1;
                            textLayout1.SetCharacterSpacing(0, CharacterSpacing, 0, textRange);

                            modifiedWidth = (letterSize.Width / 100) * TextScaling;
                            try
                            {
                                if (isNegativeFont)
                                {
                                    //tempGS = g.SaveDrawingState();
                                    Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                    g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                    g.DrawText(textLayout1, textBrush, new Rect(location.X, -(location.Y + FontSize * 2), 1, 1));
                                    //g.RestoreDrawingState(tempGS);
                                    g.Transform = tempMatrix;
                                }
                                else
                                {
                                    g.DrawText(textLayout1, textBrush, new Rect(location.X, location.Y, 1, 1));
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            location.X += modifiedWidth;
                            // break;
                        }
                        if (wordIndex < wordsLength)
                        {
                            location.X += (WordSpacing + spaceWidth + CharacterSpacing);
                        }
                    }
                }
                else
                {
                    int wordsLength = words.Length;
                    int wordLength;

                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                    if (g.Transform.M12 != 0 && g.Transform.M21 != 0 && this.IsType1Font)
                    {
                        if (IsC1)
                        {
                            IsTexRotated = true;
                            location.Y *= 1000;
                        }
                    }
                    foreach (string word in words)
                    {
                        float modifiedWidth = 0;
                        wordIndex++;

                        int letterCount = 0;
                        wordLength = word.Length;
                        //location.X += CharacterSpacing;
                        string mappedChar = word;
                        int mappedValue = 0;
                        bool isWordGlyph = false;

                        foreach (char letter in word)
                        {
                            if (this.IsType1Font)
                            {
                                Syncfusion.Pdf.GlyphWriter gw = new Syncfusion.Pdf.GlyphWriter(m_cffGlyphs.Glyphs, g, this.IsC1);
                                float xScaleFactor;
                                float yScaleFactor;
                                if (IsC1)
                                {
                                    xScaleFactor = (float)FontSize * CharSizeMultiplier;
                                    yScaleFactor = (float)FontSize * CharSizeMultiplier;
                                }
                                else
                                {
                                    xScaleFactor = (float)m_cffGlyphs.FontMatrix[0] * (float)FontSize;
                                    yScaleFactor = (float)m_cffGlyphs.FontMatrix[3] * (float)FontSize;
                                }
                                Syncfusion.DirectXWrapper.WinRT.Matrix temp = g.Transform;

                                float height = FontSize;

                                if (g.Transform.M12 == 0 || g.Transform.M21 == 0)
                                {
                                    IsTexRotated = false;
                                    g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor * g.Transform.M11, g.Transform.M12, g.Transform.M21, yScaleFactor * g.Transform.M22, (location.X * g.Transform.M11 + g.Transform.OffsetX), g.Transform.OffsetY + (location.Y + height) * g.Transform.M22);
                                }
                                else
                                {
                                    float newx = location.X;
                                    float newy = location.Y;
                                    Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, newx, newy);
                                    Syncfusion.DirectXWrapper.WinRT.Matrix unitMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);

                                    Syncfusion.DirectXWrapper.WinRT.Matrix tranfMatrix = g.Multiply(unitMatrix, tempMatrix);
                                    tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);
                                    tranfMatrix = g.Multiply(tranfMatrix, tempMatrix);

                                    tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor, 0, 0, yScaleFactor, 1, 1);
                                    tranfMatrix = g.Multiply(tranfMatrix, tempMatrix);

                                    tranfMatrix = g.Multiply(tranfMatrix, g.Transform);
                                    g.Transform = tranfMatrix;
                                }

                                mappedChar = letter.ToString();
                                mappedValue = (int)letter;
                                if (differenceTable.ContainsValue(word) && differenceMappedTable.ContainsValue(word))
                                {
                                    mappedChar = word;
                                    mappedValue = differenceTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault();
                                    isWordGlyph = true;
                                }
                                else if (differenceMappedTable.ContainsValue(mappedChar))
                                {
                                    mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == mappedChar).Select(kvp => kvp.Key).FirstOrDefault());
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
                                    }
                                }
                                else if (differenceMappedTable.ContainsValue(word))
                                {
                                    isWordGlyph = true;
                                    mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == word).Select(kvp => kvp.Key).FirstOrDefault());
                                    if (differenceTable.ContainsKey(mappedValue))
                                    {
                                        mappedChar = differenceTable[mappedValue];
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

                                if (gw.glyphs.ContainsKey(mappedChar))
                                {
                                    GeometryGroup path;
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                    {
                                        path = gw.glyphParser(mappedChar, mappedValue, 50);
                                        m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                    else
                                    {
                                        path = m_cffGlyphs.RenderedPath[mappedChar];
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                }
                                else
                                {
                                    string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                    if (gw.glyphs.ContainsKey(encodedMappedChar))
                                    {
                                        GeometryGroup path;
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                        {
                                            path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                            m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                        }
                                        else
                                        {
                                            path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                        }
                                        g.FillGeometry(path, new GraphicBrush(BrushColor));
                                    }
                                    else
                                    {
                                        Dictionary<string, double> reverseMappingTable = new Dictionary<string, double>();
                                        foreach (KeyValuePair<double, string> obj in CharacterMapTable)
                                        {
                                            reverseMappingTable.Add(obj.Value, obj.Key);
                                        }
                                        if (reverseMappingTable.ContainsKey(mappedChar))
                                        {
                                            encodedMappedChar = reverseMappingTable[mappedChar].ToString();
                                            if (gw.glyphs.ContainsKey(encodedMappedChar))
                                            {
                                                GeometryGroup path;
                                                if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                                {
                                                    path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                                    m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                                }
                                                else
                                                {
                                                    path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                                }
                                                g.FillGeometry(path, new GraphicBrush(BrushColor));
                                            }
                                        }
                                    }
                                }
                                g.Transform = temp;

                                float width1 = 0;
                                if (FontGlyphWidths != null)
                                {
                                    if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                    {
                                        width1 = FontGlyphWidths[(int)mappedValue];
                                        if (IsC1)
                                            width1 *= FontSize * CharSizeMultiplier;
                                        else
                                            width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);

                                        width1 = width1 / 100 * TextScaling;
                                    }
                                    else if (FontGlyphWidths.ContainsKey((int)letter))
                                    {
                                        width1 = FontGlyphWidths[(int)letter];
                                        if (IsC1)
                                            width1 *= FontSize * CharSizeMultiplier;
                                        else
                                            width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                }
                                if (width1 == 0)
                                {
                                    textLayout = new TextLayout(letter.ToString(), textFormat, 400, 300);
                                    textMetrics = textLayout.GetTextMetrics();
                                    letterSize.Width = textMetrics.Width;
                                    width1 = letterSize.Width;
                                }
                                modifiedWidth = (width1 / 100) * TextScaling;
                                if (IsTexRotated)
                                    modifiedWidth *= 1000;
                                if (letterCount < word.Length)
                                    location.X += modifiedWidth + CharacterSpacing;
                                else
                                    location.X += modifiedWidth;
                                if (isWordGlyph)
                                    break;
                            }
                            else
                            {
                                textRect = new Rect(location.X, location.Y, (int)1000, (int)1000);
                                letterCount++;
                                textLayout = new TextLayout(letter.ToString(), textFormat, 400, 300);
                                textMetrics = textLayout.GetTextMetrics();
                                letterSize.Width = textMetrics.Width;
                                letterSize.Height = textMetrics.Height;

                                textLayout1 = new TextLayout1(textLayout);

                                TextRange textRange = new TextRange();
                                textRange.StartPosition = 0;
                                textRange.Length = 10000;
                                textLayout1.SetCharacterSpacing(0, CharacterSpacing, 0, textRange);

                                modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                if (FontGlyphWidths != null)
                                {
                                    if (FontGlyphWidths.ContainsKey((int)letter))
                                    {
                                        float width = FontGlyphWidths[(int)letter];
                                        width *= (CharSizeMultiplier * this.FontSize);
                                        modifiedWidth = width / 100 * TextScaling;
                                    }
                                }
                                try
                                {
                                    byte b = (byte)letter;
                                    if (b > 126 && m_fontEncoding == "MacRomanEncoding")
                                    {
                                        long x = MacRomanToUnicode[(byte)letter - 128];
                                        char c = (char)x;
                                        textLayout = new TextLayout(c.ToString(), textFormat, 400, 300);
                                        textMetrics = textLayout.GetTextMetrics();
                                        letterSize.Width = textMetrics.Width;
                                        letterSize.Height = textMetrics.Height;
                                        textLayout1 = new TextLayout1(textLayout);
                                        textRange = new TextRange();
                                        textRange.StartPosition = 0;
                                        textRange.Length = 10000;
                                        textLayout1.SetCharacterSpacing(0, CharacterSpacing, 0, textRange);
                                        modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                        if (FontGlyphWidths != null)
                                        {
                                            if (FontGlyphWidths.ContainsKey((int)c))
                                            {
                                                float width = FontGlyphWidths[(int)letter];
                                                width *= (CharSizeMultiplier * this.FontSize);
                                                if (width != 0)
                                                    modifiedWidth = width / 100 * TextScaling;
                                            }
                                        }
                                        if (isNegativeFont)
                                        {
                                            //tempGS = g.SaveDrawingState();
                                            Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                            g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                            textRect = new Rect(location.X, -(location.Y + FontSize * 2), (int)1000, (int)1000);
                                            g.DrawText(c.ToString(), textFormat, textBrush, textRect);
                                            //g.RestoreDrawingState(tempGS);
                                            g.Transform = tempMatrix;
                                        }
                                        else
                                        {
                                            g.DrawText(textLayout, textBrush, textRect);
                                        }
                                    }
                                    else
                                    {


                                        if (isNegativeFont)
                                        {
                                            //tempGS = g.SaveDrawingState();
                                            Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                            g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                            g.DrawText(textLayout1, textBrush, new Rect(location.X, -(location.Y + FontSize * 2), 1, 1));
                                            //g.RestoreDrawingState(tempGS);
                                            g.Transform = tempMatrix;
                                        }
                                        else
                                        {
                                            g.DrawText(textLayout1, textBrush, new Rect(location.X, location.Y, 1, 1));
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }

                                if (letterCount < wordLength)
                                    location.X += (modifiedWidth + CharacterSpacing);
                                else
                                    location.X += modifiedWidth;
                            }
                        }

                        if (wordIndex < wordsLength)
                        {
                            if (IsTexRotated)
                                location.X += (WordSpacing + spaceWidth * 1000 + CharacterSpacing);
                            else
                                location.X += (WordSpacing + spaceWidth + CharacterSpacing);
                        }
                    }
                }
            }
            changeInX = location.X - changeInX;

            textlayout1 = null;
            textLayout1 = null;
            textLayout = null;
            textFormat = null;
            textBrush = null;
            //graphics = null;
            textRotated = IsTexRotated;
            return changeInX;
        }

        internal float RenderWithSpace(out bool textRotated, Graphics2D g, System.Drawing.PointF currentLocation, List<string> decodedList, global::Windows.UI.Color StrokingColorSpace)
        {
            if (StrokingColorSpace == new global::Windows.UI.Color())
            {
                BrushColor = global::Windows.UI.Colors.Black;
            }
            else
            {
                BrushColor = StrokingColorSpace;
            }
            if (textRenderingMode == 3)
                BrushColor = global::Windows.UI.Colors.Transparent;
            string fontName = FontName;
            //textFont = null;
            float modifiedWidth = 0;
            float changeInX = currentLocation.X;

            float textSpace;

            if (FontSize < 0)
            {
                FontSize = -FontSize;
            }
            //graphics = new Graphics();
            textFormat = new TextFormat(FontName, CustomFontSize, FontStyle, FontWeight);
            if (!m_graphicsBrushes.ContainsKey(BrushColor))
            {
                textBrush = new GraphicBrush(BrushColor);
                m_graphicsBrushes.Add(BrushColor, textBrush);
            }
            else
                textBrush = m_graphicsBrushes[BrushColor];
            textLayout = new TextLayout(Text, textFormat, 400, 300);
            TextMetrics textMetrics = textLayout.GetTextMetrics();

            SizeF tempSize = new SizeF();
            tempSize.Width = textMetrics.WidthIncludingTrailingWhitespace;
            tempSize.Height = textMetrics.Height;
            PointF location = currentLocation;

            float wordIndex = 0;
            string stringToBeRendered;
            int[] codePoints;
            char[] charsToBeRendered;
            short[] glyphIndices;
            GlyphMetrics[] metrics;
            GlyphRun glyphRun;
            if (this.Font != null)
            {
                int lenghtOfDecodedList = decodedList.Count;
                int wordLength;
                foreach (string word in decodedList)
                {
                    if (float.TryParse(word, out textSpace))
                    {
                        float textSize = FontSize;
                        textSpace = textSpace * (textSize / 1000);
                        location.X -= textSpace;
                        continue;
                    }
                    wordLength = word.Length;
                    //float modifiedWidth = 0;
                    wordIndex++;

                    Syncfusion.DirectXWrapper.WinRT.Matrix mtrx = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);


                    stringToBeRendered = word.ToString();
                    codePoints = new int[stringToBeRendered.Length];

                    charsToBeRendered = stringToBeRendered.ToCharArray();
                    for (int i = 0; i < charsToBeRendered.Length; i++)
                    {
                        codePoints[i] = (int)charsToBeRendered[i];
                    }
                    glyphIndices = new short[codePoints.Length];
                    this.Font.GetGlyphIndices(codePoints, out glyphIndices);

                    metrics = new GlyphMetrics[codePoints.Length];
                    this.Font.GetGdiCompatibleGlyphMetrics(this.FontSize, 1, mtrx, true, glyphIndices, false, out metrics);

                    SizeF actualSize = new SizeF();
                    actualSize.Width = metrics[0].AdvanceWidth;
                    actualSize.Height = textMetrics.Height;
                    int letterCount = 0;


                    string text = word.Remove(wordLength - 1, 1);
                    foreach (char letter in text)
                    {
                        letterCount++;

                        glyphRun = new GlyphRun();
                        glyphRun.FontFace = this.Font;
                        glyphRun.FontSize = FontSize;
                        if (CidToGidMap != null)
                        {
                            if (CidToGidMap.Count > 0)
                                if (m_CidToGidMap.ContainsKey((int)letter))
                                {
                                    string mappingString = m_CidToGidMap[(int)letter];
                                    stringToBeRendered = mappingString;
                                }
                        }
                        else
                            stringToBeRendered = letter.ToString();
                        codePoints = new int[stringToBeRendered.Length];

                        charsToBeRendered = stringToBeRendered.ToCharArray();
                        for (int i = 0; i < charsToBeRendered.Length; i++)
                        {
                            codePoints[i] = (int)charsToBeRendered[i];
                        }

                        this.Font.GetGlyphIndices(codePoints, out glyphIndices);//{ 43 };
                        glyphRun.Indices = glyphIndices;

                        float ascentAdjustment = (((float)glyphRun.FontFace.Metrics.Ascent) / 1000) + (((float)glyphRun.FontFace.Metrics.Descent) / 1000);
                        ascentAdjustment = FontSize;
                        g.DrawGlyphRun(new global::Windows.Foundation.Point(location.X, location.Y + ascentAdjustment), glyphRun, new GraphicBrush(BrushColor), MeasuringMode.GdiClassic);
                        mtrx = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, location.X, location.Y);

                        metrics = new GlyphMetrics[codePoints.Length];
                        this.Font.GetGdiCompatibleGlyphMetrics(this.FontSize, 1, mtrx, true, glyphIndices, false, out metrics);

                        SizeF letterSize = new SizeF();

                        if (metrics != null)
                        {
                            letterSize.Width = metrics[0].LeftSideBearing;
                            letterSize.Height = metrics[0].AdvanceHeight;
                        }

                        if (FontGlyphWidths != null)
                        {
                            if (FontGlyphWidths.ContainsKey((int)letter))
                            {
                                letterSize.Width = FontGlyphWidths[(int)letter];
                                letterSize.Width *= (CharSizeMultiplier * glyphRun.FontSize);
                            }
                            else if (DefaultWidth != 0)
                            {
                                letterSize.Width = DefaultWidth;
                                letterSize.Width *= (CharSizeMultiplier * glyphRun.FontSize);
                            }
                        }
                        modifiedWidth = letterSize.Width;

                        if (letter == ' ')
                        {
                            location.X += modifiedWidth + this.WordSpacing;
                            continue;
                        }

                        if (letterCount < wordLength)
                            location.X += modifiedWidth + CharacterSpacing;
                        else
                            location.X += modifiedWidth;
                    }
                }
            }
            else
            {
                //DrawingStateBlock tempGS;
                textLayout = new TextLayout(" ", textFormat, 400, 300);
                textMetrics = textLayout.GetTextMetrics();
                float spaceWidth = textMetrics.WidthIncludingTrailingWhitespace;
                Rect textRect;
                int letterCount = 0;
                SizeF letterSize = new SizeF();
                if (g.Transform.M12 != 0 && g.Transform.M21 != 0 && this.IsType1Font)
                {
                    if (IsC1)
                    {
                        IsTexRotated = true;
                        location.X *= 1000;
                        location.Y *= 1000;
                    }
                }
                foreach (string element in decodedList)
                {
                    if (float.TryParse(element, out textSpace))
                    {
                        float textSize = FontSize;
                        textSpace = textSpace * (textSize / 1000);
                        textSpace = textSpace - this.CharacterSpacing;
                        location.X -= textSpace;
                    }
                    else
                    {
                        string text = element.Remove(element.Length - 1, 1);

                        if (CharacterSpacing == 0 && WordSpacing == 0 && (this.FontGlyphWidths == null || this.FontGlyphWidths.Count == 0))
                        {
                            textRect = new Rect(location.X, location.Y, (int)1000, (int)1000);
                            textLayout = new TextLayout(text.ToString(), textFormat, 1400, 1300);
                            textMetrics = textLayout.GetTextMetrics();
                            if (isNegativeFont)
                            {
                                //tempGS = g.SaveDrawingState();
                                Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                //g.DrawText(this.Text.ToString(), textFormat, textRect, textBrush);
                                g.DrawText(textLayout, textBrush, new Rect(0, 0, location.X, -location.Y));
                                //g.RestoreDrawingState(tempGS);
                                g.Transform = tempMatrix;
                            }
                            else
                            {
                                g.DrawText(textLayout, textBrush, textRect);
                            }
                            location.X += (textMetrics.WidthIncludingTrailingWhitespace) / 100 * TextScaling;
                        }
                        else if ((FontGlyphWidths == null || this.FontGlyphWidths.Count == 0) && !IsType1Font)
                        {
                            string[] words = text.Split(' ');
                            wordIndex = 0;
                            int wordsLength = words.Length;
                            int wordLength;
                            foreach (string word in words)
                            {
                                modifiedWidth = 0;
                                wordIndex++;

                                wordLength = word.Length;
                                location.X += CharacterSpacing;

                                textRect = new Rect(location.X, location.Y, (int)1000, (int)1000);
                                letterCount++;
                                textLayout = new TextLayout(word.ToString(), textFormat, 400, 300);
                                textMetrics = textLayout.GetTextMetrics();
                                letterSize.Width = textMetrics.Width;
                                letterSize.Height = textMetrics.Height;

                                textLayout1 = new TextLayout1(textLayout);
                                TextRange textRange = new TextRange();
                                textRange.StartPosition = 0;
                                textRange.Length = 1;
                                textLayout1.SetCharacterSpacing(0, CharacterSpacing, 0, textRange);

                                modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                try
                                {
                                    if (isNegativeFont)
                                    {
                                        //tempGS = g.SaveDrawingState();
                                        Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                        g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                        g.DrawText(textLayout1, textBrush, new Rect(location.X, -location.Y, 1, 1));
                                        //g.RestoreDrawingState(tempGS);
                                        g.Transform = tempMatrix;
                                    }
                                    else
                                    {
                                        g.DrawText(textLayout1, textBrush, new Rect(location.X, location.Y, 1, 1));
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }

                                if (wordIndex < wordsLength)
                                {
                                    location.X += modifiedWidth + WordSpacing + spaceWidth;
                                }
                                else
                                    location.X += (modifiedWidth);
                            }
                        }
                        else
                        {
                            int textLength = text.Length;
                            letterCount = 0;

                            foreach (char letter in text)
                            {
                                bool isWordGlyph = false;
                                letterCount++;
                                if (letter == ' ')
                                {
                                    if (FontGlyphWidths.ContainsKey((int)' '))
                                    {
                                        spaceWidth = FontGlyphWidths[(int)' '];
                                        spaceWidth *= (CharSizeMultiplier * this.FontSize);
                                    }
                                    if (IsTexRotated)
                                        location.X += spaceWidth * 1000 + this.WordSpacing;
                                    else
                                        location.X += spaceWidth + this.WordSpacing;
                                    continue;
                                }
                                string mappedChar = string.Empty;
                                string textToRender = letter.ToString();
                                int mappedValue = 0;
                                if (this.IsType1Font)
                                {
                                    Syncfusion.Pdf.GlyphWriter gw = new Syncfusion.Pdf.GlyphWriter(m_cffGlyphs.Glyphs, g, this.IsC1);
                                    float xScaleFactor;
                                    float yScaleFactor;
                                    if (IsC1)
                                    {
                                        xScaleFactor = (float)FontSize * CharSizeMultiplier;
                                        yScaleFactor = (float)FontSize * CharSizeMultiplier;
                                    }
                                    else
                                    {
                                        xScaleFactor = (float)m_cffGlyphs.FontMatrix[0] * (float)FontSize;
                                        yScaleFactor = (float)m_cffGlyphs.FontMatrix[3] * (float)FontSize;
                                    }
                                    Syncfusion.DirectXWrapper.WinRT.Matrix temp = g.Transform;

                                    float height = FontSize;
                                    if (g.Transform.M12 == 0 || g.Transform.M21 == 0)
                                    {
                                        IsTexRotated = false;
                                        g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor * g.Transform.M11, g.Transform.M12, g.Transform.M21, yScaleFactor * g.Transform.M22, (location.X * g.Transform.M11 + g.Transform.OffsetX), g.Transform.OffsetY + (location.Y + height) * g.Transform.M22);
                                    }
                                    else
                                    {
                                        float newx = location.X;
                                        float newy = location.Y;
                                        Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, newx, newy);
                                        Syncfusion.DirectXWrapper.WinRT.Matrix unitMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);

                                        Syncfusion.DirectXWrapper.WinRT.Matrix tranfMatrix = g.Multiply(unitMatrix, tempMatrix);
                                        tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);
                                        tranfMatrix = g.Multiply(tranfMatrix, tempMatrix);

                                        tempMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(xScaleFactor, 0, 0, yScaleFactor, 1, 1);
                                        tranfMatrix = g.Multiply(tranfMatrix, tempMatrix);

                                        tranfMatrix = g.Multiply(tranfMatrix, g.Transform);
                                        g.Transform = tranfMatrix;
                                    }
                                    mappedChar = letter.ToString();
                                    mappedValue = (int)letter;
                                    if ((differenceTable.ContainsValue(text) && differenceMappedTable.ContainsValue(text)))
                                    {
                                        mappedChar = text;
                                        mappedValue = differenceTable.Where(kvp => kvp.Value == text).Select(kvp => kvp.Key).FirstOrDefault();
                                        isWordGlyph = true;
                                    }
                                    else if (differenceTable.ContainsKey((int)letter))
                                    {
                                        mappedChar = differenceTable[letter];
                                        mappedValue = (int)letter;
                                    }
                                    else if (differenceMappedTable.ContainsValue(mappedChar))
                                    {
                                        mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == mappedChar).Select(kvp => kvp.Key).FirstOrDefault());
                                        if (differenceTable.ContainsKey(mappedValue))
                                        {
                                            mappedChar = differenceTable[mappedValue];
                                        }
                                    }
                                    else if (differenceMappedTable.ContainsValue(text))
                                    {
                                        isWordGlyph = true;
                                        mappedValue = int.Parse(differenceMappedTable.Where(kvp => kvp.Value == text).Select(kvp => kvp.Key).FirstOrDefault());
                                        if (differenceTable.ContainsKey(mappedValue))
                                        {
                                            mappedChar = differenceTable[mappedValue];
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

                                    if (gw.glyphs.ContainsKey(mappedChar))
                                    {
                                        GeometryGroup path;
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                        {
                                            path = gw.glyphParser(mappedChar, mappedValue, 50);
                                            m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                        else
                                        {
                                            path = m_cffGlyphs.RenderedPath[mappedChar];
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                    }
                                    else if (gw.glyphs.ContainsKey(mappedValue.ToString()))
                                    {
                                        GeometryGroup path;
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                        {
                                            path = gw.glyphParser(mappedValue.ToString(), mappedValue, 50);
                                            m_cffGlyphs.RenderedPath.Add(mappedChar, path);
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                        else
                                        {
                                            path = m_cffGlyphs.RenderedPath[mappedChar];
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                    }
                                    else
                                    {
                                        string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                        if (gw.glyphs.ContainsKey(encodedMappedChar))
                                        {
                                            GeometryGroup path;
                                            if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                            {
                                                path = gw.glyphParser(encodedMappedChar, mappedValue, 50);
                                                m_cffGlyphs.RenderedPath.Add(encodedMappedChar, path);
                                            }
                                            else
                                            {
                                                path = m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            }
                                            g.FillGeometry(path, new GraphicBrush(BrushColor));
                                        }
                                    }

                                    g.Transform = temp;

                                    float width1 = 0;
                                    if (FontGlyphWidths != null)
                                    {
                                        if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                        {
                                            width1 = FontGlyphWidths[(int)mappedValue];
                                            if (IsC1)
                                                width1 *= FontSize * CharSizeMultiplier;
                                            else
                                                width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                            width1 = width1 / 100 * TextScaling;
                                        }
                                        else if (FontGlyphWidths.ContainsKey((int)letter))
                                        {
                                            width1 = FontGlyphWidths[(int)letter];
                                            if (IsC1)
                                                width1 *= FontSize * CharSizeMultiplier;
                                            else
                                                width1 *= ((float)m_cffGlyphs.FontMatrix[0] * FontSize);
                                            width1 = width1 / 100 * TextScaling;
                                        }
                                        if (CharacterMapTable.Count != 0)
                                        {
                                            foreach (KeyValuePair<double, string> value in CharacterMapTable)
                                            {
                                                if (value.Value.Equals(mappedChar))
                                                    mappedValue = (int)value.Key;
                                            }
                                            if (FontGlyphWidths.ContainsKey((int)mappedValue))
                                            {
                                                width1 = FontGlyphWidths[(int)mappedValue];
                                                width1 *= (CharSizeMultiplier * FontSize);
                                                width1 = width1 / 100 * TextScaling;
                                            }
                                            else
                                            {
                                                width1 = DefaultWidth;
                                                width1 *= (CharSizeMultiplier * FontSize);
                                                width1 = width1 / 100 * TextScaling;
                                            }
                                        }
                                        else
                                        {
                                            width1 = DefaultWidth;
                                            width1 *= (CharSizeMultiplier * FontSize);
                                            width1 = width1 / 100 * TextScaling;
                                        }
                                    }
                                    else
                                    {
                                        width1 = DefaultWidth;
                                        width1 *= (CharSizeMultiplier * FontSize);
                                        width1 = width1 / 100 * TextScaling;
                                    }
                                    if (width1 == 0)
                                    {
                                        textLayout = new TextLayout(letter.ToString(), textFormat, 400, 300);
                                        textMetrics = textLayout.GetTextMetrics();
                                        letterSize.Width = textMetrics.Width;
                                        width1 = letterSize.Width;
                                    }
                                    modifiedWidth = (width1 / 100) * TextScaling;
                                    if (IsTexRotated)
                                        modifiedWidth *= 1000;
                                    if (letterCount < text.Length)
                                        location.X += modifiedWidth + CharacterSpacing;
                                    else
                                        location.X += modifiedWidth;
                                    if (isWordGlyph)
                                        break;
                                }
                                else
                                {
                                    letterSize = new SizeF();
                                    textLayout = new TextLayout(textToRender, textFormat, 400, 300);
                                    textMetrics = textLayout.GetTextMetrics();
                                    letterSize.Width = textMetrics.Width;
                                    letterSize.Height = textMetrics.Height;

                                    modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                    if (FontGlyphWidths != null && letter != 'f')
                                    {
                                        if (FontGlyphWidths.ContainsKey((int)letter))
                                        {
                                            float width = FontGlyphWidths[(int)letter];
                                            width *= (CharSizeMultiplier * this.FontSize);
                                            if (width != 0)
                                                modifiedWidth = width / 100 * TextScaling;
                                        }
                                    }


                                    try
                                    {
                                        if ((int)letter > 127)
                                        {
                                            char HTMLChar = Syncfusion.Pdf.FontStructure.ResolveHTMLCharToASCII((int)letter);
                                            textLayout = new TextLayout(HTMLChar.ToString(), textFormat, 400, 300);
                                        }
                                        textRect = new Rect(location.X, location.Y, /*location.X + modifiedWidth*/1000, (int)1000);
                                        byte b = (byte)letter;
                                        if (b > 126 && m_fontEncoding == "MacRomanEncoding")
                                        {
                                            long x = MacRomanToUnicode[(byte)letter - 128];
                                            char c = (char)x;
                                            textLayout = new TextLayout(letter.ToString(), textFormat, 400, 300);
                                            textMetrics = textLayout.GetTextMetrics();
                                            letterSize.Width = textMetrics.Width;
                                            letterSize.Height = textMetrics.Height;
                                            modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                            if (FontGlyphWidths != null)
                                            {
                                                if (FontGlyphWidths.ContainsKey((int)letter))
                                                {
                                                    float width = FontGlyphWidths[(int)letter];
                                                    width *= (CharSizeMultiplier * this.FontSize);
                                                    modifiedWidth = width / 100 * TextScaling;
                                                }
                                            }
                                            if (isNegativeFont)
                                            {
                                                Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                                g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                                textRect = new Rect(location.X, -location.Y, (int)1000, (int)1000);
                                                g.DrawText(textLayout, textBrush, textRect);
                                                g.Transform = tempMatrix;
                                            }
                                            else
                                            {
                                                g.DrawText(textLayout, textBrush, textRect);
                                            }
                                        }
                                        else
                                        {
                                            if (isNegativeFont)
                                            {
                                                Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = g.Transform;
                                                g.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(g.Transform.M11, g.Transform.M12, g.Transform.M21, -g.Transform.M22, g.Transform.OffsetX, g.Transform.OffsetY);
                                                textRect = new Rect(location.X, -location.Y, (int)1000, (int)1000);
                                                g.DrawText(textLayout, textBrush, textRect);

                                                g.Transform = tempMatrix;
                                            }
                                            else
                                            {
                                                g.DrawText(textLayout, textBrush, textRect);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                    if (letterCount < textLength)
                                        location.X += (modifiedWidth + CharacterSpacing);
                                    else
                                        location.X += modifiedWidth;
                                }
                            }
                        }
                    }
                    //tempGS = null;
                }
            }
            //textBrush.Dispose();
            //textLayout1.Dispose();
            //textLayout.Dispose();
            //textFormat.Dispose();

            textLayout1 = null;
            textLayout = null;
            textFormat = null;
            textBrush = null;
            glyphRun = null;
            glyphIndices = null;

            //graphics.Dispose();
            //graphics = null;
            changeInX = location.X - changeInX;
            fontName = null;
            location = PointF.Empty;
            tempSize = SizeF.Empty;

            decodedList = null;
            textRotated = IsTexRotated;
            return changeInX;
        }

        /// <summary>
        /// Removes the escape sequence characters in the given text
        /// </summary>
        /// <param name="text">text with the escape sequence</param>
        /// <returns>Text without escape sequence</returns>
        private string SkipEscapeSequence(string text)
        {
            int index = -1;
            do
            {
                index = text.IndexOf("\\", (index + 1));
                if (index >= 0)
                {
                    if (text.Length > index + 1)
                    {
                        string nextLiteral = text[index + 1].ToString();
                        if ((index >= 0) && (nextLiteral == "\\" || nextLiteral == "(" || nextLiteral == ")" || nextLiteral == "\n"))
                            text = text.Remove(index, 1);
                    }
                    else
                    {
                        text = text.Remove(index, 1);
                        index = -1;
                    }
                }

            }
            while (index >= 0);
            if (text.Contains("\n"))
                text = text.Replace("\n", "");
            return text;
        }



        //private bool IsFontInstalled(string fontName)
        //{
        //    using (Font testFont = new Font(fontName, 8))
        //    {
        //        return 0 == string.Compare(
        //          fontName,
        //          testFont.Name,
        //          StringComparison.InvariantCultureIgnoreCase);
        //    }
        //}


    }
}
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;

namespace Syncfusion.PdfViewer.Base
{
    internal class TextElement
    {
        internal float textLeading = 0;
        internal List<string> DecodedList = new List<string>();
        internal int textRenderingMode = 0;
        internal bool IsType1Font = false;
        internal bool IsC1 = false;
        internal Dictionary<int, string> differenceTable = new Dictionary<int, string>();
        internal Dictionary<string, string> differenceMappedTable = new Dictionary<string, string>();
        float CharSizeMultiplier = 0.001f;
        internal string m_fontName;
        internal FontStyle fontStyle;
        internal float m_fontSize;
        internal string m_fontEncoding;
        private float m_customFontSize;
        internal string m_text;
        //internal global::Windows.UI.Color m_brushColor;
        internal float m_wordSpacing;
        internal float m_characterSpacing;
        internal float m_textScaling = 100;
        internal FontWeight fontWeight;
        internal bool isNegativeFont;
        private static Dictionary<string, string> fontList = new Dictionary<string, string>();
        internal Dictionary<int, int> FontGlyphWidths;
        private int m_defaultWidth;
        internal string fontNameReference = string.Empty;
        internal TextElement(string text)
        {
            m_text = text;
        }

        ~TextElement()
        {
            m_text = string.Empty;
        }

        internal string FontName
        {
            get
            {
                return m_fontName;
            }
            set
            {
                m_fontName = value;
            }
        }

        internal float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }

        internal string FontEncoding
        {
            get
            {
                return m_fontEncoding;
            }
            set
            {
                m_fontEncoding = value;
            }
        }

        internal float CustomFontSize
        {
            get
            {
                return m_customFontSize;
            }
            set
            {
                m_customFontSize = value;
            }
        }

        internal string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        //internal global::Windows.UI.Color BrushColor
        //{
        //    get
        //    {
        //        return m_brushColor;
        //    }
        //    set
        //    {
        //        m_brushColor = value;
        //    }
        //}

        internal float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }
            set
            {
                m_wordSpacing = value;
            }
        }

        internal float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }
            set
            {
                m_characterSpacing = value;
            }
        }
        internal float TextScaling
        {
            get
            {
                return m_textScaling;
            }
            set
            {
                m_textScaling = value;
            }

        }

        internal int DefaultWidth
        {
            get
            {
                return m_defaultWidth;
            }
            set
            {
                m_defaultWidth = value;
            }
        }

        internal Dictionary<double, string> CidToGidMap
        {
            get
            {
                return m_CidToGidMap;
            }
            set
            {
                m_CidToGidMap = value;
            }
        }

        private Dictionary<double, string> m_CidToGidMap;


        #region TextSearch
        internal float CalculateTextWidthRenderer(PointF currentLocation, global::Windows.UI.Xaml.Media.Matrix transformMatrix, bool IsRenderWithSapce, out bool IsStandardFont, PdfPageResources resource)
        {
            this.FontName = ReplaceFontNameForStandardFonts(this.FontName);
            int wordIndex = 0;
            float changeInX = currentLocation.X;
            PointF location = currentLocation;
            float spaceWidth = 0;

            FontStructure fontResource = resource.Resources[fontNameReference] as FontStructure;
            FontGlyphWidths = fontResource.FontGlyphWidths;
            if (FontGlyphWidths != null && FontGlyphWidths.ContainsKey(32))
            {
                IsStandardFont = false;
                spaceWidth = FontGlyphWidths[32] * FontSize * CharSizeMultiplier; ;
            }
            else
            {
                IsStandardFont = true;
                TextBlock dummyTextBlock = new TextBlock();
                dummyTextBlock.FontFamily = new FontFamily(this.FontName);
                dummyTextBlock.FontSize = this.FontSize;
                dummyTextBlock.FontStyle = FontStyle.Normal;
                dummyTextBlock.FontWeight = this.fontWeight;
                dummyTextBlock.Text = " a";
                dummyTextBlock.Measure(new global::Windows.Foundation.Size(0, 0));
                dummyTextBlock.Arrange(new Rect(0, 0, 0, 0));
                float tempWidth = (float)dummyTextBlock.ActualWidth;

                dummyTextBlock = new TextBlock();
                dummyTextBlock.FontFamily = new FontFamily(this.FontName);
                dummyTextBlock.FontSize = this.FontSize;
                dummyTextBlock.FontStyle = FontStyle.Normal;
                dummyTextBlock.FontWeight = this.fontWeight;
                dummyTextBlock.Text = "a";
                dummyTextBlock.Measure(new global::Windows.Foundation.Size(0, 0));
                dummyTextBlock.Arrange(new Rect(0, 0, 0, 0));
                spaceWidth = tempWidth - (float)dummyTextBlock.ActualWidth;
            }

            if (!IsRenderWithSapce)
            {
                string[] words;
                words = this.Text.Split(' ');
                int wordsLength = words.Length;
                int wordLength;
                foreach (string word in words)
                {
                    float modifiedWidth = 0;
                    wordIndex++;
                    int letterCount = 0;
                    wordLength = word.Length;
                    foreach (char letter in word)
                    {
                        if (FontGlyphWidths != null && FontGlyphWidths.ContainsKey((int)letter))
                        {
                            IsStandardFont = false;
                            float width = FontGlyphWidths[(int)letter];
                            width *= (CharSizeMultiplier * this.FontSize);
                            modifiedWidth = width / 100 * TextScaling;
                        }
                        else
                        {
                            IsStandardFont = true;
                            letterCount++;
                            if (letter != ' ')
                            {
                                modifiedWidth = FindCharWidthByTextBlock(letter);
                            }
                            else
                            {
                                modifiedWidth = spaceWidth;
                            }
                        }

                        if (letterCount < wordLength)
                            location.X += (modifiedWidth + CharacterSpacing);
                        else
                            location.X += modifiedWidth;
                    }

                    if (wordIndex < wordsLength)
                    {
                        location.X += (WordSpacing + spaceWidth + CharacterSpacing);
                    }
                }
                changeInX = location.X - changeInX;
                return changeInX;
            }
            else if (IsRenderWithSapce)
            {
                string searchableText = string.Empty;
                int wordElementIndex = 0;
                foreach (string element in DecodedList)
                {
                    wordElementIndex++;
                    float textSpace = 0;
                    if (float.TryParse(element, out textSpace))
                    {
                        float textSize = FontSize;
                        textSpace = textSpace * (textSize / 1000);
                        textSpace = textSpace - this.CharacterSpacing;
                        location.X -= textSpace;
                    }
                    else
                    {
                        if (element != string.Empty)
                        {
                            string word = element.Remove(element.Length - 1, 1);
                            float modifiedWidth = 0;
                            foreach (char letter in word)
                            {
                                searchableText += letter.ToString();
                                if (FontGlyphWidths != null && FontGlyphWidths.ContainsKey((int)letter))
                                {
                                    float width = FontGlyphWidths[(int)letter];
                                    width *= (CharSizeMultiplier * this.FontSize);
                                    modifiedWidth = width / 100 * TextScaling;
                                    if (letter == ' ')
                                    {
                                        modifiedWidth += this.WordSpacing;
                                    }
                                    modifiedWidth += this.CharacterSpacing;
                                }
                                else
                                {
                                    if (letter != ' ')
                                    {
                                        modifiedWidth = FindCharWidthByTextBlock(letter);
                                        modifiedWidth += this.CharacterSpacing;
                                    }
                                    else
                                    {
                                        modifiedWidth = spaceWidth + this.WordSpacing;
                                        modifiedWidth += this.CharacterSpacing;
                                    }
                                }
                                location.X += modifiedWidth;
                                if (searchableText.Equals(this.Text))
                                {
                                    if (wordElementIndex < DecodedList.Count && float.TryParse(DecodedList[wordElementIndex], out textSpace))
                                    {
                                        float textSize = FontSize;
                                        textSpace = textSpace * (textSize / 1000);
                                        textSpace = textSpace - this.CharacterSpacing;
                                        location.X -= textSpace;
                                    }
                                    changeInX = location.X - changeInX;
                                    return changeInX;
                                }
                            }
                        }
                        else
                        {
                            location.X += spaceWidth;
                        }
                        if (searchableText.Equals(this.Text))
                        {
                            changeInX = location.X - changeInX;
                            return changeInX;
                        }
                    }
                }
            }
            changeInX = location.X - changeInX;
            return changeInX;
        }

        private float FindCharWidthByTextBlock(char letter)
        {
            float modifiedWidth = 0;
            TextBlock dummyTextBlock = new TextBlock();
            dummyTextBlock.FontFamily = new FontFamily(this.FontName);
            dummyTextBlock.FontSize = this.FontSize;
            dummyTextBlock.FontStyle = FontStyle.Normal;
            dummyTextBlock.FontWeight = this.fontWeight;
            dummyTextBlock.Text = letter.ToString();
            dummyTextBlock.Measure(new global::Windows.Foundation.Size(0, 0));
            dummyTextBlock.Arrange(new Rect(0, 0, 0, 0));
            double width = dummyTextBlock.ActualWidth;
            modifiedWidth = (float)width / 100 * TextScaling;
            return modifiedWidth;
        }

        private string ReplaceFontNameForStandardFonts(string standardFontName)
        {
            string systemFontName = standardFontName;
            switch (standardFontName)
            {
                case "Helvetica":
                    systemFontName = "Arial";
                    break;
                case "Courier":
                    systemFontName = "Courier New";
                    break;
                case "Times-Roman":
                    systemFontName = "Times New Roman";
                    break;
            }
            return systemFontName;
        }
        #endregion
    }
}
#endif
