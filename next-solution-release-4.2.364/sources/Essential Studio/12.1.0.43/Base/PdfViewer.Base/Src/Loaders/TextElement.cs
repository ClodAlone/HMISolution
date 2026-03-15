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
using Syncfusion.Pdf.Graphics;
using System.Drawing.Drawing2D;
using System.Drawing;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf;

namespace Syncfusion.PdfViewer.Base
{

    internal class TextElement
    {
        internal Dictionary<string, double> ReverseMapTable = new Dictionary<string, double>();
        internal bool IsType1Font = false;
        internal bool Is1C = false;
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();
        internal Dictionary<string, byte[]> m_type1FontGlyphs = new Dictionary<string, byte[]>();
        internal Font textFont = null;
        internal String renderedText = string.Empty;
        float CharSizeMultiplier = 0.001f;
        internal string m_fontName;
        internal FontStyle m_fontStyle;
        internal float m_fontSize;
        internal string m_fontEncoding;
        internal string m_text;
        internal Color m_brushColor;
        internal float m_wordSpacing;
        internal float m_characterSpacing;
        internal float m_textScaling = 100;
        internal int m_renderingMode = 0;
        private Font m_font;
        internal bool isNegativeFont;
        private static Dictionary<string, string> fontList = new Dictionary<string, string>();
        PdfViewerExceptions exceptions = new PdfViewerExceptions();
        internal Dictionary<int, int> FontGlyphWidths;
        internal float DefaultGlyphWidth;
        internal bool IsTransparentText = false;
        internal bool IsCID;
        internal Dictionary<Double, string> CharacterMapTable;
        internal Dictionary<int, string> differenceTable = new Dictionary<int, string>();
        internal Dictionary<string, string> differenceMappedTable = new Dictionary<string, string>();
        internal Dictionary<int, int> OctDecMapTable;
        internal Dictionary<int, int> CidToGidReverseMapTable;
        internal Dictionary<int, string> UnicodeCharMapTable;
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

        internal Font Font
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

        internal FontStyle FontStyle
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

        internal Color BrushColor
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
        internal int RenderingMode
        {
            get
            {
                return m_renderingMode;
            }
            set
            {
                m_renderingMode = value;
            }
        }
        /// <summary>
        /// Renders the Text to the panel
        /// </summary>
        /// <param name="g">graphics element</param>
        /// <param name="currentLocation">location in which the graphics is to be drawn</param>
        internal float Render(System.Drawing.Graphics g, System.Drawing.PointF currentLocation, double textScaling, Dictionary<int, int> gWidths, double type1Height,
            Dictionary<int, string> differenceTable, Dictionary<string, string> differenceMappedTable, Dictionary<int, string> differenceEncoding)
        {
            renderedText = string.Empty;
            string fontName = FontName;
            int wordIndex;
            textFont = null;
            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float changeInX = currentLocation.X;
            if (FontSize < 0)
            {
                FontSize = -FontSize;
            }

            if (this.Font != null)
            {
                FontName = CheckFontName(this.Font.Name);
                textFont = this.Font;
            }
            else
            {
                CheckFontStyle(FontName);
                FontName = CheckFontName(FontName);
                textFont = new Font(FontName, FontSize, FontStyle);
            }
            string[] words = this.Text.Split(' ');
            SizeF tempSize = g.MeasureString(Text, textFont);
            PointF location = currentLocation;
            wordIndex = 0;
            if (IsTransparentText)
                BrushColor = Color.Transparent;
            double size = FontSize;
            if (!IsType1Font && m_fontEncoding != "MacRomanEncoding" && m_fontEncoding != "WinAnsiEncoding" && !isNegativeFont)
            {
                string newword = words[0];
                for (int i = 1; i < words.Length; i++)
                {
                    newword += (char)32;
                    newword += words[i];
                }
                words = new string[1];//this.Text.Split(' ');
                words[0] = newword;
            }
            foreach (string word in words)
            {
                float modifiedWidth = 0;
                wordIndex++;
                SizeF actualSize = g.MeasureString(word, textFont);
                int letterCount = 0;
                StringFormat format = new StringFormat(StringFormat.GenericTypographic);
                float Type1FontSpaceWidth = 0;
                bool isWordGlyph = false;
                for (int i = 0; i < word.Length; i++)
                {
                    char letter = word[i];
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
                                    i = word.Length - 1;
                                    break;
                                }
                            }
                            isWordGlyph = true;
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
                            isWordGlyph = true;
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

                        try
                        {
                            double width = 0;
                            double height;
                            if (!Is1C)
                                height = type1Height * CharSizeMultiplier * size;
                            else
                                height = FontSize;
                            GlyphWriter glyphWriter = new GlyphWriter(m_cffGlyphs.Glyphs, this.Is1C);
                            glyphWriter.is1C = this.Is1C;
                            if (!IsCID)
                            {
                                if (glyphWriter.glyphs.ContainsKey(mappedChar))
                                {
                                    Matrix m = g.Transform;
                                    GraphicsPath pathGeom;
                                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                    {
                                        pathGeom = (GraphicsPath)glyphWriter.glyphParser(mappedChar, mappedValue, 500, UnicodeCharMapTable);
                                        m_convertor = new PdfUnitConvertor();
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                        g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                        g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                        m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                                        g.Transform = m;
                                    }
                                    else
                                    {
                                        pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[mappedChar];
                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                        g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                        g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                        g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                        g.Transform = m;
                                    }
                                }
                                else
                                {
                                    string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);
                                    if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                    {
                                        Matrix m = g.Transform;
                                        GraphicsPath pathGeom;
                                        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                        {
                                            pathGeom = (GraphicsPath)glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500, UnicodeCharMapTable);
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                            m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                        }
                                        else
                                        {
                                            pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                        }
                                        g.Transform = m;
                                    }
                                }
                            }
                            else
                            {
                               string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);
                                if (ReverseMapTable.ContainsKey(mappedChar))
                                {
                                    encodedMappedChar = ReverseMapTable[mappedChar].ToString();
                                    if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                    {
                                        Matrix m = g.Transform;
                                        GraphicsPath pathGeom;
                                        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                        {
                                            pathGeom = (GraphicsPath)glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500, UnicodeCharMapTable);
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                            m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                        }
                                        else
                                        {
                                            pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                        }
                                        g.Transform = m;
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
                                else if (OctDecMapTable != null && OctDecMapTable.Count != 0)
                                {
                                    mappedValue = OctDecMapTable[mappedValue];
                                    if (gWidths.ContainsKey((int)mappedValue))
                                    {
                                        width = gWidths[(int)mappedValue];
                                        width *= (CharSizeMultiplier * size);
                                        width = width / 100 * textScaling;
                                    }
                                    else
                                    {
                                        width = this.DefaultGlyphWidth;
                                        width *= (CharSizeMultiplier * size);
                                        width = width / 100 * textScaling;
                                    }
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
                                            width = this.DefaultGlyphWidth;
                                            width *= (CharSizeMultiplier * size);
                                            width = width / 100 * textScaling;
                                        }
                                    }
                                    else
                                    {
                                        width = this.DefaultGlyphWidth;
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
                            if (modifiedWidth == 0)
                            {

                            }
                            if (letterCount < word.Length)
                                location.X += modifiedWidth + this.CharacterSpacing;
                            else
                                location.X += modifiedWidth;

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
                        catch
                        {
                            GraphicsState tempGS;
                            SizeF letterSize = g.MeasureString(letter.ToString(), textFont, PointF.Empty, format);
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
                                    if (isNegativeFont)
                                    {
                                        tempGS = g.Save();
                                        g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                        g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                        g.Restore(tempGS);
                                    }
                                    else
                                    {
                                        g.DrawString(c.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                        SizeF csize = g.MeasureString(c.ToString(), textFont, PointF.Empty, format);
                                        modifiedWidth = (csize.Width / 100) * TextScaling;
                                    }
                                }
                                else
                                {
                                    if (isNegativeFont)
                                    {
                                        tempGS = g.Save();
                                        g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                        g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                        g.Restore(tempGS);
                                    }
                                    else
                                    {
                                        if (letter > 127 && letter <= 255 && m_fontEncoding == "WinAnsiEncoding")
                                        {
                                            string equivalentLetter = Encoding.Default.GetString(new byte[] { (byte)letter });
                                            g.DrawString(equivalentLetter, textFont, (new Pen(BrushColor).Brush), location, format);
                                            letterSize = g.MeasureString(equivalentLetter, textFont, PointF.Empty, format);
                                            modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                        }
                                        else
                                            g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                    }
                                    renderedText += letter.ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Exceptions.Append("\r\nCharacter not rendered " + letter.ToString() + "\r\n" + ex.StackTrace);
                                continue;
                            }
                            if (letterCount < word.Length)
                                location.X += (modifiedWidth + CharacterSpacing);
                            else
                                location.X += modifiedWidth;
                        }
                    }
                    #endregion
                    else
                    {
                        GraphicsState tempGS;
                        SizeF letterSize = g.MeasureString(letter.ToString(), textFont, PointF.Empty, format);
                        modifiedWidth = (letterSize.Width / 100) * TextScaling;
                        if (modifiedWidth == 0 && letter == 32)
                        {
                            StringFormat space = new StringFormat(StringFormat.GenericTypographic);
                            space.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                            modifiedWidth = g.MeasureString(" ", textFont, PointF.Empty, space).Width;
                        }
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
                                if (isNegativeFont)
                                {
                                    tempGS = g.Save();
                                    g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                    g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                    g.Restore(tempGS);
                                }
                                else
                                {
                                    g.DrawString(c.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                    SizeF csize = g.MeasureString(c.ToString(), textFont, PointF.Empty, format);
                                    modifiedWidth = (csize.Width / 100) * TextScaling;
                                }
                            }
                            else
                            {
                                if (isNegativeFont)
                                {
                                    tempGS = g.Save();
                                    g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                    g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                    g.Restore(tempGS);
                                }
                                else
                                {
                                    if (letter > 127 && letter <= 255 && m_fontEncoding == "WinAnsiEncoding")
                                    {
                                        string equivalentLetter = Encoding.Default.GetString(new byte[] { (byte)letter });
                                        g.DrawString(equivalentLetter, textFont, (new Pen(BrushColor).Brush), location, format);
                                        letterSize = g.MeasureString(equivalentLetter, textFont, PointF.Empty, format);
                                        modifiedWidth = (letterSize.Width / 100) * TextScaling;
                                    }
                                    else
                                    {
                                        if (letter < 33)
                                            g.DrawString(MapEscapeSequence(letter), textFont, (new Pen(BrushColor).Brush), location, format);
                                        else
                                            g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                    }
                                }
                                renderedText += letter.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptions.Exceptions.Append("\r\nCharacter not rendered " + letter.ToString() + "\r\n" + ex.StackTrace);
                            continue;
                        }
                        if (letterCount < word.Length)
                            location.X += (modifiedWidth + CharacterSpacing);
                        else
                            location.X += modifiedWidth;
                    }
                }
                if (!this.IsType1Font || Type1FontSpaceWidth == 0)
                {
                    StringFormat spaceFormat = new StringFormat(StringFormat.GenericTypographic);
                    spaceFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                    float spaceWidth = g.MeasureString(" ", textFont, PointF.Empty, spaceFormat).Width;
                    if (wordIndex < words.Length)
                    {
                        location.X += (WordSpacing + spaceWidth + CharacterSpacing);
                    }
                }
                else
                {
                    if (wordIndex < words.Length)
                    {
                        location.X += Type1FontSpaceWidth;
                    }
                }
            }
            changeInX = location.X - changeInX;
            return changeInX;
        }

        internal float RenderWithSpace(System.Drawing.Graphics g, System.Drawing.PointF currentLocation, List<string> decodedList, List<float> characterSpacings, double textScaling, Dictionary<int, int> gWidths, double type1Height,
            Dictionary<int, string> differenceTable, Dictionary<string, string> differenceMappedTable, Dictionary<int, string> differenceEncoding)
        {
            renderedText = string.Empty;
            string fontName = FontName;
            textFont = null;
            float modifiedWidth = 0;
            float changeInX = currentLocation.X;
            float textSpace;
            PdfUnitConvertor convertor = new PdfUnitConvertor();
            if (FontSize < 0)
            {
                FontSize = -FontSize;
            }

            if (this.Font != null)
            {
                FontName = CheckFontName(this.Font.Name);
                textFont = this.Font;
            }
            else
            {
                CheckFontStyle(FontName);
                FontName = CheckFontName(FontName);
                textFont = new Font(FontName, FontSize, FontStyle);
            }

            SizeF tempSize = g.MeasureString(Text, textFont);
            PointF location = currentLocation;
            float size = FontSize;
            double y = currentLocation.Y;
            foreach (string word in decodedList)
            {
                GraphicsState tempGS;
                StringFormat format = new StringFormat(StringFormat.GenericTypographic);

                StringFormat spaceFormat = new StringFormat(StringFormat.GenericTypographic);
                spaceFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                float spaceWidth = g.MeasureString(" ", textFont, PointF.Empty, spaceFormat).Width;

                if (float.TryParse(word, out textSpace))
                {
                    float textSize = textFont.SizeInPoints;
                    textSpace = textSpace * (textSize / 1000);
                    textSpace = textSpace - this.CharacterSpacing;
                    location.X -= textSpace;
                }
                else
                {
                    if (word[0] >= 3584 && word[0] <= 3711)
                    {
                        float newwidth = 0;
                        string thaistring = string.Empty;
                        List<char> thailist = new List<char>();
                        thaistring = word.Remove(word.Length - 1, 1);
                        g.DrawString(thaistring, textFont, (new Pen(BrushColor).Brush), location, format);
                        SizeF thaistringsize = g.MeasureString(thaistring, textFont, PointF.Empty, format);
                        newwidth = (thaistringsize.Width / 100) * TextScaling;
                        location.X += newwidth;
                    }
                    else
                    {
                        string text = word.Remove(word.Length - 1, 1);
                        int letterCount = 0;
                        for (int i = 0; i < text.Length; i++)
                        {
                            char letter = text[i];
                            letterCount++;
                            bool isWordGlyph = false;
                            if (this.IsType1Font)
                            {
                                GlyphWriter glyphWriter = new GlyphWriter(m_cffGlyphs.Glyphs, this.Is1C);
                                string mappedChar = letter.ToString();
                                int mappedValue = (int)letter;

                                mappedChar = letter.ToString();
                                mappedValue = (int)letter;
                                if ((differenceTable.ContainsValue(text) && differenceMappedTable.ContainsValue(text)))
                                {
                                    mappedChar = text;
                                    foreach (KeyValuePair<int, string> entry in differenceTable)
                                    {
                                        if (entry.Value == text)
                                        {
                                            mappedValue = entry.Key;
                                            isWordGlyph = true;
                                            i = text.Length - 1;
                                            break;
                                        }
                                    }
                                    isWordGlyph = true;
                                }
                                else if (differenceTable.ContainsKey((int)letter))
                                {
                                    mappedChar = differenceTable[letter];
                                    mappedValue = (int)letter;
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
                                else if (differenceMappedTable.ContainsValue(text))
                                {
                                    isWordGlyph = true;
                                    foreach (KeyValuePair<string, string> entry in differenceMappedTable)
                                    {
                                        if (entry.Value == text)
                                        {
                                            mappedValue = int.Parse(entry.Key);
                                        }
                                        if (differenceTable.ContainsKey(mappedValue))
                                        {
                                            mappedChar = differenceTable[mappedValue];
                                        }
                                        break;
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

                                try
                                {
                                    double width = 0;
                                    double height1;
                                    if (!Is1C)
                                        height1 = type1Height * CharSizeMultiplier * size;
                                    else
                                        height1 = FontSize;
                                    glyphWriter = new GlyphWriter(m_cffGlyphs.Glyphs, this.Is1C);
                                    glyphWriter.is1C = this.Is1C;
                                    if(!IsCID)
                                    {
                                    if (glyphWriter.glyphs.ContainsKey(mappedChar))
                                    {
                                        Matrix m = g.Transform;
                                        GraphicsPath pathGeom;
                                        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                        if (!m_cffGlyphs.RenderedPath.ContainsKey(mappedChar))
                                        {
                                            pathGeom = (GraphicsPath)glyphWriter.glyphParser(mappedChar, mappedValue, 500,UnicodeCharMapTable);
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(y + height1));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                            m_cffGlyphs.RenderedPath.Add(mappedChar, pathGeom);
                                        }
                                        else
                                        {
                                            pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[mappedChar];
                                            double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                            g.TranslateTransform((float)location.X, (float)(y + height1));
                                            g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                            g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                        }
                                        g.Transform = m;
                                    }
                                
                                    else
                                    {
                                        string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);

                                        if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                        {
                                            Matrix m = g.Transform;
                                            GraphicsPath pathGeom;
                                            PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                            if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                            {
                                                pathGeom = (GraphicsPath)glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500, UnicodeCharMapTable);
                                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                                g.TranslateTransform((float)location.X, (float)(y + height1));
                                                g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                                g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                                m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                            }
                                            else
                                            {
                                                pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                                double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                                g.TranslateTransform((float)location.X, (float)(y + height1));
                                                g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                                g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                            }
                                            g.Transform = m;
                                        }
                                    }
                                    }
                                   else
                                        {
                                            string encodedMappedChar = Syncfusion.Pdf.FontStructure.GetCharCode(mappedChar);
                                            if (ReverseMapTable.ContainsKey(mappedChar))
                                            {
                                                encodedMappedChar = ReverseMapTable[mappedChar].ToString();
                                                if (glyphWriter.glyphs.ContainsKey(encodedMappedChar))
                                                {
                                                    Matrix m = g.Transform;
                                                    GraphicsPath pathGeom;
                                                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                                    if (!m_cffGlyphs.RenderedPath.ContainsKey(encodedMappedChar))
                                                    {
                                                        pathGeom = (GraphicsPath)glyphWriter.glyphParser(encodedMappedChar, mappedValue, 500,UnicodeCharMapTable);
                                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                                        g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height1));
                                                        g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                                        g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                                        m_cffGlyphs.RenderedPath.Add(encodedMappedChar, pathGeom);
                                                    }
                                                    else
                                                    {
                                                        pathGeom = (GraphicsPath)m_cffGlyphs.RenderedPath[encodedMappedChar];
                                                        double scaleFactor = scaleFactor = CharSizeMultiplier * size;
                                                        g.TranslateTransform((float)location.X, (float)(currentLocation.Y + height1));
                                                        g.ScaleTransform((float)scaleFactor, (float)scaleFactor);
                                                        g.FillPath((new Pen(BrushColor).Brush), pathGeom);
                                                    }
                                                    g.Transform = m;
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
                                        else if (OctDecMapTable != null && OctDecMapTable.Count != 0)
                                        {
                                            mappedValue = OctDecMapTable[mappedValue];
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
                                        location.X += modifiedWidth + CharacterSpacing;
                                    else
                                        location.X += modifiedWidth;

                                    if (mappedValue == 32 || (int)letter == 32)
                                    {
                                        location.X += WordSpacing;
                                    }
                                    if (isWordGlyph)
                                        break;
                                }
                                catch
                                {
                                    #region Exceptionhandling
                                    if (letter == ' ')
                                    {
                                        location.X += spaceWidth + this.WordSpacing;
                                        renderedText += " ";
                                        continue;
                                    }
                                    SizeF letterSize = g.MeasureString(letter.ToString(), textFont, PointF.Empty, format);
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
                                            if (isNegativeFont)
                                            {
                                                tempGS = g.Save();
                                                g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                                g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                                g.Restore(tempGS);
                                            }
                                            else
                                            {
                                                g.DrawString(c.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                                SizeF csize = g.MeasureString(c.ToString(), textFont, PointF.Empty, format);
                                                modifiedWidth = (csize.Width / 100) * TextScaling;
                                            }

                                        }
                                        else
                                        {
                                            if (isNegativeFont)
                                            {
                                                tempGS = g.Save();
                                                g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                                g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                                g.Restore(tempGS);
                                            }
                                            else
                                            {
                                                if (RenderingMode == 1)
                                                {
                                                    GraphicsPath path = new GraphicsPath();
                                                    FontFamily fontf = new FontFamily(textFont.Name);
                                                    path.AddString(letter.ToString(), fontf, 1, textFont.Size, new Point((int)location.X, (int)location.Y), format);

                                                    g.DrawPath(new Pen(BrushColor), path);
                                                    path.Dispose();
                                                    fontf.Dispose();
                                                }
                                                else
                                                    g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                            }
                                            renderedText += letter.ToString();
                                        }

                                    }

                                    catch (Exception ex)
                                    {
                                        exceptions.Exceptions.Append("\r\nCharacter not rendered " + letter.ToString() + "\r\n" + ex.StackTrace);
                                        continue;
                                    }
                                    if (letterCount < text.Length)
                                        location.X += (modifiedWidth + CharacterSpacing);
                                    else
                                        location.X += modifiedWidth;
                                    #endregion
                                }
                            }
                            else
                            {
                                if (letter == ' ')
                                {
                                    location.X += spaceWidth + this.WordSpacing + CharacterSpacing;
                                    renderedText += " ";
                                    continue;
                                }
                                SizeF letterSize = g.MeasureString(letter.ToString(), textFont, PointF.Empty, format);
                                modifiedWidth = (letterSize.Width / 100) * TextScaling;

                                if (CidToGidReverseMapTable != null && CidToGidReverseMapTable.Count > 0)
                                {
                                    int mappedValue = letter;
                                    mappedValue = CidToGidReverseMapTable[mappedValue];
                                    if (gWidths != null)
                                    {
                                        if (gWidths.ContainsKey((int)mappedValue))
                                        {
                                            float width = gWidths[(int)mappedValue];
                                            width *= (CharSizeMultiplier * size);
                                            modifiedWidth = width / 100 * TextScaling;
                                        }
                                    }
                                }
                                else if (FontGlyphWidths != null)
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
                                        if (isNegativeFont)
                                        {
                                            tempGS = g.Save();
                                            g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                            g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                            g.Restore(tempGS);
                                        }
                                        else
                                        {
                                            g.DrawString(c.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                            SizeF csize = g.MeasureString(c.ToString(), textFont, PointF.Empty, format);
                                            modifiedWidth = (csize.Width / 100) * TextScaling;
                                        }

                                    }
                                    else
                                    {
                                        if (isNegativeFont)
                                        {
                                            tempGS = g.Save();
                                            g.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 2 * location.Y + 2 * letterSize.Height));
                                            g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                            g.Restore(tempGS);
                                        }
                                        else
                                        {
                                            if (RenderingMode == 1)
                                            {
                                                GraphicsPath path = new GraphicsPath();
                                                FontFamily fontf = new FontFamily(textFont.Name);
                                                path.AddString(letter.ToString(), fontf, 1, textFont.Size, new Point((int)location.X, (int)location.Y), format);

                                                g.DrawPath(new Pen(BrushColor), path);
                                                path.Dispose();
                                                fontf.Dispose();
                                            }
                                            else
                                            {
                                                if (letter < 33)
                                                    g.DrawString(MapEscapeSequence(letter), textFont, (new Pen(BrushColor).Brush), location, format);
                                                else
                                                    g.DrawString(letter.ToString(), textFont, (new Pen(BrushColor).Brush), location, format);
                                            }
                                        }
                                        renderedText += letter.ToString();
                                    }

                                }

                                catch (Exception ex)
                                {
                                    exceptions.Exceptions.Append("\r\nCharacter not rendered " + letter.ToString() + "\r\n" + ex.StackTrace);
                                    continue;
                                }
                                if (letterCount < text.Length)
                                    location.X += (modifiedWidth + CharacterSpacing);
                                else
                                    location.X += modifiedWidth;
                            }
                        }
                    }
                }
            }
            changeInX = location.X - changeInX;
            return changeInX;
        }

        internal string MapEscapeSequence(char letter)
        {
            switch ((int)letter)
            {
                case 32:
                    return ((char)0).ToString();
                case 13:
                    return ((char)65529).ToString();
                case 11:
                    return ((char)65532).ToString();
                case 10:
                    return ((char)65531).ToString();
                case 9:
                    return ((char)65530).ToString();
                default:
                    return letter.ToString();
            }
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

        internal void CheckFontStyle(string fontName)
        {
            if (fontName.Contains("Regular"))
            {
                FontStyle = FontStyle.Regular;
            }
            else if (fontName.Contains("Bold"))
            {
                FontStyle = FontStyle.Bold;
            }
            else if (fontName.Contains("Italic"))
            {
                FontStyle = FontStyle.Italic;
            }

        }

        private bool IsFontInstalled(string fontName)
        {
            using (Font testFont = new Font(fontName, 8))
            {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        internal static string CheckFontName(string fontName)
        {
            string sInput = fontName;

            if (sInput.Contains("#20"))
                sInput = sInput.Replace("#20", " ");
            string[] sReturn = new string[1];
            sReturn[0] = "";
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    if (!CUPPER.Contains(sInput[iIndex - 1].ToString()))
                    {
                        iArrayCount++;
                        string[] sTemp = new string[iArrayCount + 1];
                        Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                        sReturn = sTemp;
                    }
                }
                sReturn[iArrayCount] += sChar;
            }


            fontName = string.Empty;
            foreach (string word in sReturn)
            {
                fontName += word + " ";
            }

            if (fontName.Contains("Zapf"))
                fontName = "MS Gothic";
            if (fontName.Contains("Times"))
                fontName = "Times New Roman";
            if (fontName == "Bookshelf Symbol Seven")
                fontName = "Bookshelf Symbol 7";
            if (fontName.Contains("Courier"))
                fontName = "Courier New";
            if (fontName.Contains("Free") && fontName.Contains("9"))
                fontName = "Free 3 of 9";

            if (fontName.Contains("Regular"))
                fontName = fontName.Replace("Regular", "");
            else if (fontName.Contains("Bold"))
                fontName = fontName.Replace("Bold", "");
            else if (fontName.Contains("Italic"))
                fontName = fontName.Replace("Italic", "");

            fontName = fontName.Trim();
            return fontName;
        }
    }
}
