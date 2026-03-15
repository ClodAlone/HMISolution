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
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.IO;
using System.Diagnostics;
using System.IO;
using System.Drawing.Text;
using Syncfusion.Pdf.Exporting;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Native;
using System.Reflection;

namespace Syncfusion.Pdf
{
    internal class FontStructure
    {
        private string m_fontName;
        private FontStyle m_fontStyle;
        private PdfDictionary m_fontDictionary;
        private Dictionary<double, string> m_characterMapTable;
        private Dictionary<string, double> m_reverseMapTable;
        private Dictionary<double, string> m_cidToGidTable;
        private Dictionary<string, string> m_differencesDictionary;
        private const string m_replacementCharacter = "�";
        private Font m_currentFont;
        private float m_fontSize;
        private string m_fontEncoding;
        private bool isGetFontCalled = false;
        private Dictionary<int, int> m_fontGlyphWidth;
        private bool m_containsCmap = true;
        private bool m_fontFileContainsCmap = false;
        private static Dictionary<double, string> tempMapTable = new Dictionary<double, string>();
        private bool m_isSameFont = false;
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Dictionary<int, int> m_octDecMapTable;
        private Dictionary<int, int> m_cidToGidReverseMapTable;
        internal bool IsMappingDone = false;
        bool IsSystemFontExist = false;
        public bool IsTextExtraction = false;
        private Dictionary<int, int> m_fontGlyphWidthMapping;
        private float m_type1GlyphHeight;
        internal Dictionary<string, byte[]> m_type1FontGlyphs = new Dictionary<string, byte[]>();
        internal bool IsType1Font = false;
        internal bool Is1C = false;
        private bool m_isCID;
        internal Dictionary<int, string> differenceTable = new Dictionary<int, string>();
        internal Dictionary<int, string> differenceEncoding = new Dictionary<int, string>();
        internal static Dictionary<long, CffGlyphs> type1FontReference = new Dictionary<long, CffGlyphs>();
        internal static Dictionary<int, string> unicodeCharMapTable;
       
        internal bool IsCID
        {
            get 
            {
                m_isCID = IsCIDFontType();
                return m_isCID;
            }
            set
            {
                m_isCID = value;
            }
 
        }
        internal Dictionary<int, string> UnicodeCharMapTable
        {
            get
            {
                if (unicodeCharMapTable == null)
                {
                    unicodeCharMapTable = GetUnicodeCharMapTable();

                }
                return unicodeCharMapTable;
            }
            set
            {
                unicodeCharMapTable = value;
            }
        }
        /// <summary>
        /// Internal variable that holds cff glyphs
        /// </summary>
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();

        internal Dictionary<string, double> ReverseMapTable
        {
            get
            {
                if (m_reverseMapTable == null)
                {
                    m_reverseMapTable = GetReverseMapTable();
                }
                return m_reverseMapTable;
            }
            set
            {
                m_reverseMapTable = value;
            }
        }

        internal Dictionary<Double, string> CharacterMapTable
        {
            get
            {
                if (m_characterMapTable == null)
                {
                    m_characterMapTable = GetCharacterMapTable();
                }
                return m_characterMapTable;
            }
            set
            {
                m_characterMapTable = value;
            }
        }
        internal Dictionary<string, string> DifferencesDictionary
        {
            get
            {
                if (m_differencesDictionary == null)
                {
                    m_differencesDictionary = GetDifferencesDictionary();
                }
                return m_differencesDictionary;
            }
            set
            {
                m_differencesDictionary = value;
            }
        }
        internal Dictionary<int, int> OctDecMapTable
        {
            get
            {
                if (m_octDecMapTable == null)
                {
                    m_octDecMapTable = new Dictionary<int, int>();
                }
                return m_octDecMapTable;
            }
            set
            {
                m_octDecMapTable = value;
            }
        }

        internal Dictionary<int, int> CidToGidReverseMapTable
        {
            get
            {
                if (m_cidToGidReverseMapTable == null)
                {
                    m_cidToGidReverseMapTable = new Dictionary<int, int>();
                }
                return m_cidToGidReverseMapTable;
            }
            set
            {
                m_cidToGidReverseMapTable = value;
            }
        }

        public FontStructure()
        {

        }
        ~FontStructure()
        {
            tempMapTable = new Dictionary<double, string>();
        }
        public FontStructure(IPdfPrimitive fontDictionary)
        {
            m_fontDictionary = fontDictionary as PdfDictionary;
            fontType = (IPdfPrimitive)m_fontDictionary.Items[new PdfName("Subtype")] as PdfName;
        }
        public MemoryStream fontStream = new MemoryStream();
        /// <summary>
        /// Holds the font name associated with the text element
        /// </summary>
        public string FontName
        {
            get
            {
                if (m_fontName == null)
                {
                    m_fontName = GetFontName();
                }
                return m_fontName;
            }
        }
        /// <summary>
        /// Holds the font style of the text to be decoded.
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                if (m_fontStyle == FontStyle.Regular)
                {
                    m_fontStyle = GetFontStyle();
                }
                return m_fontStyle;
            }

        }

        /// <summary>
        /// Gets and sets whether same font is denoted in more than one XObject.
        /// </summary>
        public bool IsSameFont
        {
            get
            {
                return m_isSameFont;
            }
            set
            {
                m_isSameFont = value;
            }
        }

        internal Dictionary<double, string> CidToGidMap
        {
            get
            {
                return m_cidToGidTable;
            }
        }

        internal Dictionary<int, int> FontGlyphWidths
        {
            get
            {
                if (FontEncoding == "Identity-H")
                {
                    GetGlyphWidths();
                }
                else
                {
                    GetGlyphWidthsNonIdH();
                }
                return m_fontGlyphWidth;
            }
            set
            {
                m_fontGlyphWidth = value;
            }
        }

        internal float DefaultGlyphWidth;

        internal float Type1GlyphHeight
        {
            get
            {
                return m_type1GlyphHeight;
            }
        }

        public Font CurrentFont
        {
            get
            {
                if (m_currentFont == null && !isGetFontCalled)
                    if (m_fontSize != 0)
                        m_currentFont = GetFont(m_fontSize);
                return m_currentFont;
            }
        }

        public float FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
                m_currentFont = GetFont(m_fontSize);
            }
        }

        /// <summary>
        /// Holds the font encoding associated with the text element
        /// </summary>
        public string FontEncoding
        {
            get
            {
                if (m_fontEncoding == null)
                {
                    m_fontEncoding = GetFontEncoding();
                }
                return m_fontEncoding;
            }
        }

        internal bool ContainsCmap
        {
            get
            {
                return m_fontFileContainsCmap;
            }

        }


        /// <summary>
        /// Takes in the encoded text, identifies the type of encoding used, decodes the encoded text, returns the decoded text.
        /// </summary>
        /// <param name="textToDecode">
        /// Encoded string from the PDF document.
        /// </param>
        /// <param name="isSameFont"></param>
        /// <returns>
        /// Decoded string, human readable.
        /// </returns>
        public string Decode(string textToDecode, bool isSameFont)
        {
            string decodedText = string.Empty;
            string encodedText = textToDecode;
            this.m_isSameFont = isSameFont;

            switch (encodedText[0])
            {
                case '(':
                    {
                        if (encodedText.Contains("\\\n"))
                        {
                            StringBuilder sb = new StringBuilder(encodedText);
                            sb.Replace("\\\n", "");
                            encodedText = sb.ToString();
                        }
                        encodedText = encodedText.Substring(1, encodedText.Length - 2);
                        decodedText = GetLiteralString(encodedText);
                        if (m_fontDictionary.ContainsKey(DictionaryProperties.Encoding))
                        {
                            if (m_fontDictionary[DictionaryProperties.Encoding] is PdfName)
                            {
                                if ((m_fontDictionary[DictionaryProperties.Encoding] as PdfName).Value == "Identity-H")
                                {
                                    string text = SkipEscapeSequence(decodedText);

                                    List<byte> bytes = new List<byte>();
                                    foreach (char c in text)
                                    {
                                        bytes.Add((Byte)c);
                                    }
                                    decodedText = Encoding.BigEndianUnicode.GetString(bytes.ToArray());
                                }
                            }
                        }
                    }
                    break;
                case '[':
                    {
                        if (encodedText.Contains("\\\n"))
                        {
                            StringBuilder sb = new StringBuilder(encodedText);
                            sb.Replace("\\\n", "");
                            encodedText = sb.ToString();
                        }
                        encodedText = encodedText.Substring(1, encodedText.Length - 2);
                        while (encodedText.Length > 0)
                        {
                            bool isHex = false;
                            int textStart = encodedText.IndexOf('(');
                            int textEnd = encodedText.IndexOf(')');
                            int textHexStart = encodedText.IndexOf('<');
                            int textHexEnd = encodedText.IndexOf('>');

                            if (textHexStart < textStart && textHexStart > -1)
                            {
                                textStart = textHexStart;
                                textEnd = textHexEnd;
                                isHex = true;
                            }
                            if (textStart < 0)
                            {
                                textStart = encodedText.IndexOf('<');
                                textEnd = encodedText.IndexOf('>');
                                if (textStart >= 0)
                                    isHex = true;
                                else
                                    break;
                            }
                            else if (textEnd > 0)
                            {
                                while (encodedText[textEnd - 1] == '\\')
                                {
                                    if (encodedText.IndexOf(')', textEnd + 1) >= 0)
                                    {
                                        textEnd = encodedText.IndexOf(')', textEnd + 1);
                                    }
                                    else
                                        break;
                                }
                            }


                            string tempString = encodedText.Substring(textStart + 1, textEnd - textStart - 1);
                            if (isHex)
                                decodedText += GetHexaDecimalString(tempString);
                            else
                                decodedText += GetLiteralString(tempString);

                            encodedText = encodedText.Substring(textEnd + 1, encodedText.Length - textEnd - 1);
                        }
                    }
                    break;
                case '<':
                    {
                        string hexEncodedText = encodedText.Substring(1, encodedText.Length - 2);
                        decodedText = GetHexaDecimalString(hexEncodedText);
                    }
                    break;
                default:
                    break;

            }

            if (FontEncoding != "Identity-H" || (FontEncoding == "Identity-H" && this.CurrentFont == null) || (FontEncoding == "Identity-H" && m_containsCmap))
            {
                IsMappingDone = true;
                if (this.CharacterMapTable != null && this.CharacterMapTable.Count > 0)
                    decodedText = MapCharactersFromTable(decodedText);
                else if (this.DifferencesDictionary != null && this.DifferencesDictionary.Count > 0)
                    decodedText = MapDifferences(decodedText);
                else
                    decodedText = SkipEscapeSequence(decodedText);
            }

            if (m_cidToGidTable != null)
                decodedText = MapCidToGid(decodedText);

            if (this.FontName == "ZapfDingbats")
                decodedText = MapZapf(decodedText);
            return decodedText;
        }

        public List<string> DecodeTextTJ(string textToDecode, bool isSameFont)
        {
            string decodedText = string.Empty;
            string encodedText = textToDecode;
            string listElement;
            this.m_isSameFont = isSameFont;
            List<string> decodedList = new List<string>();

            switch (encodedText[0])
            {
                case '(':
                    {
                        if (encodedText.Contains("\\\n"))
                        {
                            StringBuilder sb = new StringBuilder(encodedText);
                            sb.Replace("\\\n", "");
                            encodedText = sb.ToString();
                        }
                        encodedText = encodedText.Substring(1, encodedText.Length - 2);
                        decodedText = GetLiteralString(encodedText);
                        if (m_fontDictionary.ContainsKey(DictionaryProperties.Encoding))
                        {
                            if (m_fontDictionary[DictionaryProperties.Encoding] is PdfName)
                            {
                                if ((m_fontDictionary[DictionaryProperties.Encoding] as PdfName).Value == "Identity-H")
                                {
                                    string text = SkipEscapeSequence(decodedText);

                                    List<byte> bytes = new List<byte>();
                                    foreach (char c in text)
                                    {
                                        bytes.Add((Byte)c);
                                    }
                                    decodedText = Encoding.BigEndianUnicode.GetString(bytes.ToArray());
                                }
                            }
                        }
                    }
                    break;
                case '[':
                    {
                        if (encodedText.Contains("\\\n"))
                        {
                            StringBuilder sb = new StringBuilder(encodedText);
                            sb.Replace("\\\n", "");
                            encodedText = sb.ToString();
                        }
                        encodedText = encodedText.Substring(1, encodedText.Length - 2);
                        while (encodedText.Length > 0)
                        {
                            bool isHex = false;
                            int textStart = encodedText.IndexOf('(');
                            int textEnd = encodedText.IndexOf(')');
                            int textHexStart = encodedText.IndexOf('<');
                            int textHexEnd = encodedText.IndexOf('>');

                            if (textHexStart < textStart && textHexStart > -1)
                            {
                                textStart = textHexStart;
                                textEnd = textHexEnd;
                                isHex = true;
                            }
                            if (textStart < 0)
                            {
                                textStart = encodedText.IndexOf('<');
                                textEnd = encodedText.IndexOf('>');
                                if (textStart >= 0)
                                    isHex = true;
                                else
                                    break;
                            }
                            if (textEnd < 0 && encodedText.Length > 0)
                            {
                                listElement = encodedText;
                                decodedList.Add(listElement);
                                break;
                            }


                            else if (textEnd > 0)
                            {
                                while (encodedText[textEnd - 1] == '\\')
                                {
                                    if (textEnd - 1 > 0 && encodedText[textEnd - 2] == '\\')
                                        break;
                                    if (encodedText.IndexOf(')', textEnd + 1) >= 0)
                                        textEnd = encodedText.IndexOf(')', textEnd + 1);
                                    else
                                        break;
                                }
                            }

                            if (textStart != 0)
                            {
                                listElement = encodedText.Substring(0, textStart);
                                decodedList.Add(listElement);
                            }

                            string tempString = encodedText.Substring(textStart + 1, textEnd - textStart - 1);

                            if (isHex)
                            {
                                listElement = GetHexaDecimalString(tempString);
                                decodedText += listElement;
                            }
                            else
                            {
                                listElement = GetLiteralString(tempString);
                                decodedText += listElement;
                            }

                            if (listElement.Contains("(") || listElement.Contains(")"))
                            {
                                char[] splitters = { '\\' };
                                string[] texts = listElement.Split(splitters);
                                string text;
                                listElement = "";
                                listElement += texts[0];
                                for (int i = 1; i < texts.Length; i++)
                                {
                                    text = texts[i];
                                    if (text.Length != 0)
                                    {
                                        if (text[0] == '(' || text[0] == ')')
                                        {
                                            listElement += texts[i];
                                        }
                                        else
                                        {
                                            listElement += @"\" + text[i];
                                        }
                                    }
                                }
                            }
                            if (FontEncoding != "Identity-H" || (FontEncoding == "Identity-H" && this.CurrentFont == null) || (FontEncoding == "Identity-H" && m_containsCmap))
                            {
                                IsMappingDone = true;
                                if (this.CharacterMapTable != null && this.CharacterMapTable.Count > 0)
                                    listElement = MapCharactersFromTable(listElement);
                                else if (this.DifferencesDictionary != null && this.DifferencesDictionary.Count > 0)
                                    listElement = MapDifferences(listElement);
                                else
                                    listElement = SkipEscapeSequence(listElement);
                            }

                            if (m_cidToGidTable != null)
                                listElement = MapCidToGid(listElement);
                            if (listElement.Length > 0)
                            {
                                if (listElement[0] >= 3584 && listElement[0] <= 3711 && decodedList.Count > 0)
                                {
                                    string previous = decodedList[0];
                                    previous = previous.Remove(previous.Length - 1);
                                    previous += listElement;
                                    listElement = previous;
                                    decodedList[0] = listElement + "s";
                                }
                                else if ((listElement[0] == (char)32 || listElement[0] == (char)47) && listElement.Length > 1)
                                {
                                    if (listElement[1] >= 3584 && listElement[1] <= 3711 && decodedList.Count > 0)
                                    {
                                        string previous = decodedList[0];
                                        previous = previous.Remove(previous.Length - 1);
                                        previous += listElement;
                                        listElement = previous;
                                        decodedList[0] = listElement + "s";
                                    }
                                    else
                                    {
                                        listElement += "s";
                                        decodedList.Add(listElement);
                                    }
                                }
                                else
                                {
                                    listElement += "s";
                                    decodedList.Add(listElement);
                                }
                            }
                            else
                            {
                                listElement += "s";
                                decodedList.Add(listElement);
                            }
                            encodedText = encodedText.Substring(textEnd + 1, encodedText.Length - textEnd - 1);
                        }
                    }
                    break;
                case '<':
                    {
                        string hexEncodedText = encodedText.Substring(1, encodedText.Length - 2);
                        decodedText = GetHexaDecimalString(hexEncodedText);
                    }
                    break;
                default:
                    break;

            }
            decodedText = SkipEscapeSequence(decodedText);
            return decodedList;

        }

        #region Helper methods
        /// <summary>
        /// Decodes the octal text in the encoded text.
        /// </summary>
        /// <param name="encodedText">The text encoded from the PDF document</param>
        /// <returns>Decoded text with replaced octal texts</returns>
        private string GetLiteralString(string encodedText)
        {
            string decodedText = encodedText;
            int octalIndex = -1;
            int limit = 3;
            while (decodedText.Contains("\\") || decodedText.Contains("\0"))
            {
                string octalText = string.Empty;
                if (decodedText.IndexOf('\\', octalIndex + 1) >= 0)
                {
                    octalIndex = decodedText.IndexOf('\\', octalIndex + 1);
                }
                else
                {
                    octalIndex = decodedText.IndexOf('\0', octalIndex + 1);
                    if (octalIndex < 0)
                        break;
                    limit = 2;
                }
                for (int i = octalIndex + 1; i <= octalIndex + limit; i++) //check for octal characters
                {
                    if (i < decodedText.Length)
                    {
                        int val = 0;
                        if (int.TryParse(decodedText[i].ToString(), out val))
                        {
                            if (val <= 8)
                                octalText += decodedText[i];
                        }
                        else
                        {
                            octalText = string.Empty;
                            break;
                        }
                    }
                    else
                        octalText = string.Empty;
                }

                if (octalText != string.Empty)
                {
                    int decimalValue = (int)Convert.ToUInt64(octalText, 8);
                    string temp;
                    char decodedChar = (char)decimalValue;
                    if (this.CharacterMapTable != null && this.CharacterMapTable.Count > 0)
                    {
                        temp = decodedChar.ToString();
                    }
                    else if (this.DifferencesDictionary != null && this.DifferencesDictionary.Count > 0)
                    {
                        temp = decodedChar.ToString();
                    }
                    else
                    {
                        System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(1252);
                        temp = encoding.GetString(new byte[] { Convert.ToByte(decimalValue) });
                        char[] tempchar = temp.ToCharArray();
                        int charvalue = 0;
                        foreach (char tempchar1 in tempchar)
                        {
                            charvalue = (int)tempchar1;
                        }
                        if (!OctDecMapTable.ContainsKey(charvalue))
                            OctDecMapTable.Add(charvalue, decimalValue);

                    }
                    decodedText = decodedText.Remove(octalIndex, limit + 1);
                    decodedText = decodedText.Insert(octalIndex, temp);
                }
            }
            if (decodedText.Contains("\\") && m_fontEncoding != "Identity-H")
            {
                if (decodedText.Length > 1)
                {
                    int index = decodedText.IndexOf("\\");
                    char m_char = decodedText[index + 1];
                    if (m_char == '(' || m_char == ')')
                    {
                        Regex.Unescape(decodedText);
                    }
                }
            }

            return decodedText;
        }
        /// <summary>
        /// Decodes the HEX encoded string.
        /// </summary>
        /// <param name="hexEncodedText">
        /// HEX encoded string.
        /// </param>
        /// <returns>
        /// Decoded string.
        /// </returns>
        private string GetHexaDecimalString(string hexEncodedText)
        {
            string decodedText = string.Empty;
            if (!string.IsNullOrEmpty(hexEncodedText))
            {
                PdfName fontType = (IPdfPrimitive)m_fontDictionary.Items[new PdfName("Subtype")] as PdfName;
                int limit = 2;
                if (fontType.Value != "Type1" && fontType.Value != "TrueType" && fontType.Value != "Type3")
                {
                    limit = 4;
                }
                hexEncodedText = EscapeSymbols(hexEncodedText);
                string tempHexEncodedText = hexEncodedText;
                string tempDecodedText = decodedText;
                string decodedTxt = null;
                while (hexEncodedText.Length > 0)
                {
                    if (hexEncodedText.Length % 4 != 0)
                    {
                        limit = 2;
                    }
                    string hexChar = hexEncodedText.Substring(0, limit);

                    if (m_fontDictionary.ContainsKey(DictionaryProperties.DescendantFonts) && !m_fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
                    {
                        PdfArray descendantArray = m_fontDictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                        if (descendantArray != null)
                        {
                            if ((descendantArray[0] as PdfReferenceHolder) != null)
                            {
                                PdfDictionary descendantDictionary = (descendantArray[0] as PdfReferenceHolder).Object as PdfDictionary;
                                PdfDictionary descriptorDictionary = (descendantDictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                                if (descriptorDictionary != null)
                                {
                                    if (descendantDictionary.ContainsKey(DictionaryProperties.Subtype) && !descriptorDictionary.ContainsKey(DictionaryProperties.FontFile2))
                                    {
                                        PdfName subtype = descendantDictionary[DictionaryProperties.Subtype] as PdfName;
                                        if (subtype.Value == "CIDFontType2")
                                            hexChar = MapHebrewCharacters(hexChar);
                                    }
                                }
                            }
                        }
                    }

                    decodedText += (char)Int64.Parse(hexChar, System.Globalization.NumberStyles.HexNumber);
                    hexEncodedText = hexEncodedText.Substring(limit, hexEncodedText.Length - limit);
                    decodedTxt = decodedText.ToString();
                }
                if ((decodedTxt.Contains("") || decodedTxt.Contains("") || decodedTxt.Contains("")) && tempHexEncodedText.Length < limit)
                {
                    decodedText = tempDecodedText;
                    int hexNum = Int32.Parse(tempHexEncodedText, System.Globalization.NumberStyles.HexNumber);
                    byte[] bytes = BitConverter.GetBytes(hexNum);
                    hexEncodedText = System.Text.Encoding.GetEncoding(1251).GetString(bytes);
                    hexEncodedText = hexEncodedText.Remove(1);
                    decodedText += hexEncodedText;
                }
            }
            return decodedText;
        }

        internal string MapHebrewCharacters(string hexChar)
        {
            if (hexChar.Substring(0, 2) == "02")
            {
                int i = int.Parse(hexChar, System.Globalization.NumberStyles.HexNumber);
                i += 816;
                hexChar = i.ToString("X");
            }
            else if (hexChar.Substring(0, 2) == "00")
            {
                if (hexChar.Substring(2, 1) == "0" || hexChar.Substring(2, 1) == "1")
                {
                    int i = int.Parse(hexChar, System.Globalization.NumberStyles.HexNumber);
                    i += 29;
                    hexChar = i.ToString("X");
                }
                else
                {
                    int i = int.Parse(hexChar, System.Globalization.NumberStyles.HexNumber);
                    i += 1335;
                    hexChar = i.ToString("X");
                }
            }
            return hexChar;
        }

        /// <summary>
        /// Extracts the font name associated with the string.
        /// </summary>
        /// <returns>
        /// Font name.
        /// </returns>
        private string GetFontName()
        {
            string fontName = string.Empty;
            IsSystemFontExist = false;
            List<string> systemFontsCollection = new List<string>();

            if (m_fontDictionary.ContainsKey(DictionaryProperties.BaseFont))
            {
                PdfName baseFont = m_fontDictionary[DictionaryProperties.BaseFont] as PdfName;
                if (baseFont == null)
                {
                    baseFont = ((m_fontDictionary[DictionaryProperties.BaseFont]) as PdfReferenceHolder).Object as PdfName;
                }
                string font = baseFont.Value;
                if (!IsTextExtraction)
                {
                    if (font.Contains("#20") && !font.Contains("+"))
                    {
                        int startIndex = font.LastIndexOf("#20");
                        font = font.Substring(0, startIndex);
                        font = font + "+";
                    }
                    if (font.Contains("+"))
                    {

                    }
                }
                if (!IsSystemFontExist)
                {
                    if (baseFont.Value.Contains("+"))
                    {
                        fontName = baseFont.Value.Split('+')[1];
                    }
                    else
                    {
                        fontName = baseFont.Value;
                    }

                    if (fontName.Contains("-"))
                    {
                        fontName = fontName.Split('-')[0];
                    }
                    else if (fontName.Contains(","))
                    {
                        fontName = fontName.Split(',')[0];
                    }
                    if (fontName.Contains("MT"))
                    {
                        fontName = fontName.Replace("MT", "");
                    }
                }
            }

            return fontName;
        }
        /// <summary>
        /// Extracts the font style associated with the text string
        /// </summary>
        /// <returns>
        /// Font style.
        /// </returns>
        private FontStyle GetFontStyle()
        {
            FontStyle fontStyle = FontStyle.Regular;

            if (m_fontDictionary.ContainsKey(DictionaryProperties.BaseFont))
            {
                PdfName baseFont = m_fontDictionary[DictionaryProperties.BaseFont] as PdfName;
                if (baseFont == null)
                {
                    baseFont = ((m_fontDictionary[DictionaryProperties.BaseFont]) as PdfReferenceHolder).Object as PdfName;
                }
                if (baseFont.Value.Contains("-") || baseFont.Value.Contains(","))
                {
                    string style = string.Empty;

                    if (baseFont.Value.Contains("-"))
                    {
                        style = baseFont.Value.Split('-')[1];
                    }
                    else if (baseFont.Value.Contains(","))
                    {
                        style = baseFont.Value.Split(',')[1];
                    }
                    style = style.Replace("MT", "");
                    switch (style)
                    {
                        case "Italic":
                        case "Oblique":
                            fontStyle = FontStyle.Italic;
                            break;

                        case "Bold":
                        case "BoldMT":
                            fontStyle = FontStyle.Bold;
                            break;

                        case "BoldItalic":
                        case "BoldOblique":
                            fontStyle = FontStyle.Italic | FontStyle.Bold;
                            break;
                    }
                }
                else
                {
                    if (baseFont.Value.Contains("Bold"))
                        fontStyle = FontStyle.Bold;
                    if (baseFont.Value.Contains("BoldItalic") || baseFont.Value.Contains("BoldOblique"))
                        fontStyle = FontStyle.Italic | FontStyle.Bold;
                    if (baseFont.Value.Contains("Italic") || baseFont.Value.Contains("Oblique"))
                        fontStyle = FontStyle.Italic;
                }
            }

            return fontStyle;
        }


        /// <summary>
        /// Extracts the font encoding associated with the text string
        /// </summary>
        /// <returns>
        /// Font style.
        /// </returns>
        private string GetFontEncoding()
        {
            PdfName baseFont = new PdfName();
            string fontEncoding = string.Empty;
            if (m_fontDictionary.ContainsKey(DictionaryProperties.Encoding))
            {
                baseFont = m_fontDictionary[DictionaryProperties.Encoding] as PdfName;
                if (baseFont == null)
                {
                    Type type = (m_fontDictionary[DictionaryProperties.Encoding]).GetType();
                    PdfDictionary baseFontDict = new PdfDictionary();
                    if (type.Name == "PdfDictionary")
                    {
                        baseFontDict = (m_fontDictionary[DictionaryProperties.Encoding]) as PdfDictionary;
                    }
                    else if (type.Name == "PdfReferenceHolder")
                    {
                        baseFontDict = ((m_fontDictionary[DictionaryProperties.Encoding]) as PdfReferenceHolder).Object as PdfDictionary;
                    }

                    if (baseFontDict != null && baseFontDict.ContainsKey(DictionaryProperties.Type))
                    {
                        fontEncoding = (baseFontDict[DictionaryProperties.Type] as PdfName).Value;
                    }
                }
                else
                {
                    fontEncoding = baseFont.Value;
                }
            }
            return fontEncoding;
        }
        public Font GetFont(float size)
        {
            MemoryStream CidToGidStream = new MemoryStream();
            isGetFontCalled = true;
            string fontName = FontName;
            bool isEmbedded = false;
            Font embeddedFont = null;
            FontStyle style;
            if (FontStyle == FontStyle.Regular)
                style = CheckFontStyle(FontName);
            else
                style = FontStyle;

            if (IsSystemFontExist == true)
            {
                embeddedFont = new Font(fontName, size, style);
                return embeddedFont;
            }
            else
            {
                fontName = CheckFontName(fontName);
                embeddedFont = new Font(fontName, size, style);

                #region Identity-H
                if (FontEncoding == "Identity-H")
                {
                    try
                    {
                        PdfDictionary dictionary = m_fontDictionary;
                        if (dictionary.ContainsKey(DictionaryProperties.DescendantFonts))
                        {
                            PdfArray arr = null;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfArray)
                                arr = dictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfReferenceHolder)
                                arr = (dictionary[DictionaryProperties.DescendantFonts] as PdfReferenceHolder).Object as PdfArray;
                            dictionary = arr[0] as PdfDictionary;
                            if (dictionary == null)
                                dictionary = (arr[0] as PdfReferenceHolder).Object as PdfDictionary;

                            if (dictionary.ContainsKey(DictionaryProperties.CIDToGIDMap))
                            {
                                if (dictionary[DictionaryProperties.CIDToGIDMap] is PdfReferenceHolder)
                                {
                                    PdfStream cidToGid = (dictionary[DictionaryProperties.CIDToGIDMap] as PdfReferenceHolder).Object as PdfStream;
                                    PdfDictionary cidToGidDic = (dictionary[DictionaryProperties.CIDToGIDMap] as PdfReferenceHolder).Object as PdfDictionary;
                                    CidToGidStream = cidToGid.InternalStream;

                                    if (cidToGidDic.ContainsKey(DictionaryProperties.Filter))
                                    {
                                        string[] cidToGidFilter = GetFontFilter(cidToGidDic);
                                        if (cidToGidFilter != null)
                                        {
                                            for (int k = 0; k < cidToGidFilter.Length; k++)
                                            {
                                                switch (cidToGidFilter[k])
                                                {
                                                    case "A85":
                                                    case "ASCII85Decode":
                                                        {
                                                            CidToGidStream = DecodeASCII85Stream(CidToGidStream);
                                                            break;
                                                        }
                                                    case "FlateDecode":
                                                        {
                                                            CidToGidStream = DecodeFlateStream(CidToGidStream);
                                                            break;
                                                        }
                                                }
                                            }
                                        }

                                    }
                                    CidToGidStream.Position = 0;
                                    byte[] cidToGidData = CidToGidStream.GetBuffer();
                                    m_cidToGidTable = GetCidToGidTable(cidToGidData);
                                }
                            }

                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }
                        else if (dictionary.ContainsKey(DictionaryProperties.FontDescriptor))
                        {
                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }
                        #region FontFile

                        if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile))
                        {
                            IsType1Font = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFileBytes = str.ToArray();

                                FontFile type1Font = new FontFile();
                                m_cffGlyphs = type1Font.ParseType1FontFile(fontFileBytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion
                        #region FontFile2
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile2))
                        {
                            PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile2] as PdfReferenceHolder).Object as PdfDictionary;
                            MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                            string[] filter = GetFontFilter(fontDictionary);

                            if (filter != null)
                            {
                                for (int k = 0; k < filter.Length; k++)
                                {
                                    switch (filter[k])
                                    {
                                        case "A85":
                                        case "ASCII85Decode":
                                            {
                                                str = DecodeASCII85Stream(str);
                                                break;
                                            }
                                        case "FlateDecode":
                                            {
                                                str = DecodeFlateStream(str);
                                                break;
                                            }
                                        case DictionaryProperties.RunLengthDecode:
                                                {
                                                    str.Position = 0;

                                                    byte[] buffer = (str as MemoryStream).ToArray();
                                                    int pointer = 0, length;
                                                    byte[] decodedBuffer, currentBuffer, tempBuffer;
                                                    decodedBuffer = new byte[0];
                                                    tempBuffer = new byte[0];
                                                    
                                                    while (pointer < buffer.Length - 1)
                                                    {
                                                        tempBuffer = decodedBuffer;
                                                        length = buffer[pointer];
                                                        if (length >= 0 && length <= 127)
                                                        {
                                                            pointer++;
                                                            currentBuffer = new byte[length + 1];
                                                            Array.Copy(buffer, pointer, currentBuffer, 0, currentBuffer.Length);

                                                            decodedBuffer = new byte[tempBuffer.Length + currentBuffer.Length];

                                                            Array.Copy(tempBuffer, 0, decodedBuffer, 0, tempBuffer.Length);
                                                            Array.Copy(currentBuffer, 0, decodedBuffer, tempBuffer.Length, currentBuffer.Length);
                                                            pointer += length + 1;
                                                        }
                                                        else if (length >= 129 && length <= 255)
                                                        {
                                                            pointer++;
                                                            byte arrayValue = buffer[pointer];
                                                            int noOfTimes = 257 - length;
                                                            currentBuffer = new byte[noOfTimes];
                                                            for (int i = 0; i < noOfTimes; i++)
                                                            {
                                                                currentBuffer[i] = arrayValue;
                                                            }

                                                            decodedBuffer = new byte[tempBuffer.Length + currentBuffer.Length];

                                                            Array.Copy(tempBuffer, 0, decodedBuffer, 0, tempBuffer.Length);
                                                            Array.Copy(currentBuffer, 0, decodedBuffer, tempBuffer.Length, currentBuffer.Length);
                                                            pointer++;
                                                        }
                                                        else if (length == 128)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    str = new MemoryStream(decodedBuffer);
                                                    str.Position = 0;
                                                    break;
                                                }
                                    }
                                }
                            }

                            str.Capacity = (int)str.Length;
                            byte[] fontFileBytes = str.ToArray();
                            FontFile2 fontFile = new FontFile2(fontFileBytes);
                            List<TableEntry> entryList = new List<TableEntry>();
                            FontDecode fontDecode = new FontDecode();

                            foreach (string table in fontFile.tableList)
                            {
                                if (table == "name")
                                    continue;
                                if (table == "cmap")
                                {
                                    m_fontFileContainsCmap = true;
                                    if (CharacterMapTable.Count == 0 || m_isSameFont)
                                    {
                                        m_containsCmap = false;
                                        TableEntry cmapEntry = new TableEntry();
                                        CMap cmap = new CMap();
                                        MemoryStream cmapStream = cmap.CreateCMapStream();
                                        cmapStream.Capacity = (int)cmapStream.Length;
                                        byte[] array = cmapStream.GetBuffer();
                                        cmapEntry.id = "cmap";
                                        cmapEntry.bytes = array;
                                        cmapEntry.checkSum = CalculateCheckSum(array);
                                        cmapEntry.length = array.Length;
                                        entryList.Add(cmapEntry);
                                        continue;
                                    }
                                    else
                                        m_containsCmap = true;
                                }
#if !WPF
                                if (table == "glyf" || table == "head" || table == "hhea" || table == "hmtx" || table == "loca" || table == "maxp" || table == "cmap")
#endif
                                {
                                    TableEntry entry = new TableEntry();
                                    entry.id = table;
                                    int tableId = fontFile.getTableID(table);
                                    entry.bytes = fontFile.getTableBytes(tableId);
                                    entry.checkSum = CalculateCheckSum(entry.bytes);
                                    entry.length = entry.bytes.Length;
                                    entryList.Add(entry);
                                }
                            }

                            if (!fontFile.tableList.Contains("cmap"))
                            {
                                m_containsCmap = false;
                                TableEntry cmapEntry = new TableEntry();
                                CMap cmap = new CMap();
                                MemoryStream cmapStream = cmap.CreateCMapStream();
                                cmapStream.Capacity = (int)cmapStream.Length;
                                byte[] array = cmapStream.GetBuffer();
                                cmapEntry.id = "cmap";
                                cmapEntry.bytes = array;
                                cmapEntry.checkSum = CalculateCheckSum(array);
                                cmapEntry.length = array.Length;
                                entryList.Add(cmapEntry);
                            }

                            TableEntry nameEntry = new TableEntry();
                            byte[] array1 = global::Syncfusion.Pdf.Properties.Resources.name;
                            nameEntry.id = "name";
                            nameEntry.bytes = array1;
                            nameEntry.checkSum = CalculateCheckSum(array1);
                            nameEntry.length = array1.Length;
                            entryList.Add(nameEntry);
                            fontStream = fontDecode.CreateFontStream(entryList);

                            byte[] fontArray = fontStream.GetBuffer();

                            int dataLength = fontArray.Length;

                            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
                            Marshal.Copy(fontArray, 0, ptrData, dataLength);

                            uint cFonts = 0;
                            GdiApi.AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);


                            //PASS THE FONT TO THE  PRIVATEFONTCOLLECTION OBJECT
                            pfc.AddMemoryFont(ptrData, dataLength);

                            //FREE THE "UNSAFE" MEMORY
                            Marshal.FreeCoTaskMem(ptrData);

                            GetGlyphWidths();
                            try
                            {
                                using (FontFamily ff = pfc.Families[0])
                                {
                                    embeddedFont = new System.Drawing.Font(ff, size, this.FontStyle);
                                }

                            }
                            catch
                            {
                                try
                                {
                                    using (FontFamily ff = pfc.Families[0])
                                    {
                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Regular);
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        using (FontFamily ff = pfc.Families[0])
                                        {
                                            embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold);
                                        }
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            using (FontFamily ff = pfc.Families[0])
                                            {
                                                embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Italic);
                                            }
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                using (FontFamily ff = pfc.Families[0])
                                                {
                                                    embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Underline);
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold | FontStyle.Italic);
                                                    }
                                                }
                                                catch
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return embeddedFont;
                        }
                        #endregion
                        #region FontFile3
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile3))
                        {
                            //if (fontType.Value == "Type0")
                            //{
                            //    return;
                            //}
                            IsType1Font = true;
                            Is1C = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFile3Bytes = str.ToArray();

                                FontFile3 type1Font = new FontFile3();
                                m_cffGlyphs = type1Font.readType1CFontFile(fontFile3Bytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                                IsCID = type1Font.isCID;
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion

                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                #endregion
                #region WinANSI and BuiltIn
                else if (FontEncoding == "WinAnsiEncoding" || FontEncoding == "" || FontEncoding == "BuiltIn" || FontEncoding == "MacRomanEncoding")
                {
                    try
                    {
                        PdfDictionary dictionary = m_fontDictionary;

                        if (dictionary.ContainsKey(DictionaryProperties.DescendantFonts))
                        {
                            PdfArray arr = null;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfArray)
                                arr = dictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfReferenceHolder)
                                arr = (dictionary[DictionaryProperties.DescendantFonts] as PdfReferenceHolder).Object as PdfArray;
                            dictionary = (arr[0] as PdfReferenceHolder).Object as PdfDictionary;

                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }
                        else if (dictionary.ContainsKey(DictionaryProperties.FontDescriptor))
                        {
                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }

                        if (embeddedFont.Name.Replace(" ", "") == FontName)
                        {
                            return null;
                        }
                        #region FontFile

                        if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile))
                        {
                            IsType1Font = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFileBytes = str.ToArray();

                                FontFile type1Font = new FontFile();
                                m_cffGlyphs = type1Font.ParseType1FontFile(fontFileBytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion
                        #region FontFile2
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile2))
                        {
                            PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile2] as PdfReferenceHolder).Object as PdfDictionary;
                            MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                            string[] filter = GetFontFilter(fontDictionary);

                            if (filter != null)
                            {
                                for (int k = 0; k < filter.Length; k++)
                                {
                                    switch (filter[k])
                                    {
                                        case "A85":
                                        case "ASCII85Decode":
                                            {
                                                str = DecodeASCII85Stream(str);
                                                break;
                                            }
                                        case "FlateDecode":
                                            {
                                                str = DecodeFlateStream(str);
                                                break;
                                            }
                                    }
                                }
                            }

                            str.Capacity = (int)str.Length;
                            byte[] fontFileBytes = str.GetBuffer();
                            FontFile2 fontFile = new FontFile2(fontFileBytes);
                            List<TableEntry> entryList = new List<TableEntry>();
                            FontDecode fontDecode = new FontDecode();


                            foreach (string table in fontFile.tableList)
                            {
                                TableEntry entry = new TableEntry();
                                entry.id = table;
                                int tableId = fontFile.getTableID(table);
                                entry.bytes = fontFile.getTableBytes(tableId);
                                entry.checkSum = CalculateCheckSum(entry.bytes);
                                entry.length = entry.bytes.Length;
                                entryList.Add(entry);
                            }

                            if (!fontFile.tableList.Contains("name"))
                            {
                                TableEntry nameEntry = new TableEntry();
                                byte[] array = global::Syncfusion.Pdf.Properties.Resources.name;
                                nameEntry.id = "name";
                                nameEntry.bytes = array;
                                nameEntry.checkSum = CalculateCheckSum(array);
                                nameEntry.length = array.Length;
                                entryList.Add(nameEntry);
                            }
                            fontStream = fontDecode.CreateFontStream(entryList);

                            byte[] fontArray = fontStream.GetBuffer();
                            int dataLength = fontArray.Length;

                            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
                            Marshal.Copy(fontArray, 0, ptrData, dataLength);

                            uint cFonts = 0;
                            GdiApi.AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);


                            //PASS THE FONT TO THE  PRIVATEFONTCOLLECTION OBJECT
                            pfc.AddMemoryFont(ptrData, dataLength);

                            //FREE THE "UNSAFE" MEMORY
                            Marshal.FreeCoTaskMem(ptrData);
                            try
                            {
                                using (FontFamily ff = pfc.Families[0])
                                {
                                    embeddedFont = new System.Drawing.Font(ff, size);
                                }
                            }
                            catch
                            {
                                try
                                {
                                    using (FontFamily ff = pfc.Families[0])
                                    {
                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Regular);
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        using (FontFamily ff = pfc.Families[0])
                                        {
                                            embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold);
                                        }
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            using (FontFamily ff = pfc.Families[0])
                                            {
                                                embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Italic);
                                            }
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                using (FontFamily ff = pfc.Families[0])
                                                {
                                                    embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Underline);
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold | FontStyle.Italic);
                                                    }
                                                }
                                                catch
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return embeddedFont;
                        }
                        #endregion
                        #region FontFile3
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile3))
                        {
                            //if (fontType.Value == "Type0")
                            //{
                            //    return;
                            //}
                            IsType1Font = true;
                            Is1C = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFile3Bytes = str.ToArray();

                                FontFile3 type1Font = new FontFile3();
                                m_cffGlyphs = type1Font.readType1CFontFile(fontFile3Bytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                                IsCID = type1Font.isCID;
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion
                        else if (dictionary.ContainsKey("FontFamily"))
                        {
                            PdfString embeddedFontName = dictionary["FontFamily"] as PdfString;
                            embeddedFont = new Font(embeddedFontName.Value, FontSize, this.FontStyle);
                            return embeddedFont;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                #endregion
                #region CustomEncoding
                else if (FontEncoding == "Encoding")
                {
                    try
                    {
                        PdfDictionary dictionary = m_fontDictionary;
                        if (dictionary.ContainsKey(DictionaryProperties.Encoding))
                        {
                            if ((dictionary[DictionaryProperties.Encoding] is PdfReferenceHolder))
                            {
                                PdfDictionary encodingDictionary = ((dictionary[DictionaryProperties.Encoding] as PdfReferenceHolder).Object as PdfDictionary);
                                if (encodingDictionary.ContainsKey(DictionaryProperties.Differences))
                                {
                                    PdfArray diffCharTable = encodingDictionary[DictionaryProperties.Differences] as PdfArray;
                                    int i = 0;
                                    //System.Collections.IEnumerator e = diffCharTable.Count;
                                    for (int j = 0; j < diffCharTable.Count; j++)
                                    {
                                        IPdfPrimitive differenceChar = diffCharTable[j];
                                        if (differenceChar is PdfNumber)
                                        {
                                            i = (differenceChar as PdfNumber).IntValue;
                                        }
                                        else
                                        {
                                            string mappedChar = (differenceChar as PdfName).Value;
                                            differenceTable.Add(i, mappedChar);
                                            i += 1;
                                        }

                                    }
                                }
                            }
                        }
                        if (dictionary.ContainsKey(DictionaryProperties.DescendantFonts))
                        {
                            PdfArray arr = null;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfArray)
                                arr = dictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                            if (dictionary[DictionaryProperties.DescendantFonts] is PdfReferenceHolder)
                                arr = (dictionary[DictionaryProperties.DescendantFonts] as PdfReferenceHolder).Object as PdfArray;
                            dictionary = (arr[0] as PdfReferenceHolder).Object as PdfDictionary;

                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }
                        else if (dictionary.ContainsKey(DictionaryProperties.FontDescriptor))
                        {
                            dictionary = (dictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object as PdfDictionary;
                            isEmbedded = true;
                        }

                        if (embeddedFont.Name.Replace(" ", "") == FontName)
                        {
                            return null;
                        }
                        #region FontFile

                        if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile))
                        {
                            IsType1Font = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFileBytes = str.ToArray();

                                FontFile type1Font = new FontFile();
                                m_cffGlyphs = type1Font.ParseType1FontFile(fontFileBytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion
                        #region FontFile2
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile2))
                        {
                            PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile2] as PdfReferenceHolder).Object as PdfDictionary;
                            MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                            string[] filter = GetFontFilter(fontDictionary);

                            if (filter != null)
                            {
                                for (int k = 0; k < filter.Length; k++)
                                {
                                    switch (filter[k])
                                    {
                                        case "A85":
                                        case "ASCII85Decode":
                                            {
                                                str = DecodeASCII85Stream(str);
                                                break;
                                            }
                                        case "FlateDecode":
                                            {
                                                str = DecodeFlateStream(str);
                                                break;
                                            }
                                    }
                                }
                            }

                            str.Capacity = (int)str.Length;
                            byte[] fontFileBytes = str.GetBuffer();
                            FontFile2 fontFile = new FontFile2(fontFileBytes);
                            List<TableEntry> entryList = new List<TableEntry>();
                            FontDecode fontDecode = new FontDecode();

                            foreach (string table in fontFile.tableList)
                            {
                                if (table == "name")
                                    continue;
                                if (table == "cmap")
                                {
                                    m_fontFileContainsCmap = true;
                                    if (CharacterMapTable.Count == 0 || m_isSameFont)
                                    {
                                        m_containsCmap = false;
                                        TableEntry cmapEntry = new TableEntry();
                                        CMap cmap = new CMap();
                                        MemoryStream cmapStream = cmap.CreateCMapStream();
                                        cmapStream.Capacity = (int)cmapStream.Length;
                                        byte[] array = cmapStream.GetBuffer();
                                        cmapEntry.id = "cmap";
                                        cmapEntry.bytes = array;
                                        cmapEntry.checkSum = CalculateCheckSum(array);
                                        cmapEntry.length = array.Length;
                                        entryList.Add(cmapEntry);
                                        continue;
                                    }
                                    else
                                        m_containsCmap = true;
                                }
#if !WPF
                                if (table == "glyf" || table == "head" || table == "hhea" || table == "hmtx" || table == "loca" || table == "maxp" || table == "cmap")
#endif
                                {
                                    TableEntry entry = new TableEntry();
                                    entry.id = table;
                                    int tableId = fontFile.getTableID(table);
                                    entry.bytes = fontFile.getTableBytes(tableId);
                                    entry.checkSum = CalculateCheckSum(entry.bytes);
                                    entry.length = entry.bytes.Length;
                                    entryList.Add(entry);
                                }
                            }

                            if (!fontFile.tableList.Contains("cmap"))
                            {
                                m_containsCmap = false;
                                TableEntry cmapEntry = new TableEntry();
                                CMap cmap = new CMap();
                                MemoryStream cmapStream = cmap.CreateCMapStream();
                                cmapStream.Capacity = (int)cmapStream.Length;
                                byte[] array = cmapStream.GetBuffer();
                                cmapEntry.id = "cmap";
                                cmapEntry.bytes = array;
                                cmapEntry.checkSum = CalculateCheckSum(array);
                                cmapEntry.length = array.Length;
                                entryList.Add(cmapEntry);
                            }

                            TableEntry nameEntry = new TableEntry();
                            byte[] array1 = global::Syncfusion.Pdf.Properties.Resources.name;
                            nameEntry.id = "name";
                            nameEntry.bytes = array1;
                            nameEntry.checkSum = CalculateCheckSum(array1);
                            nameEntry.length = array1.Length;
                            entryList.Add(nameEntry);
                            fontStream = fontDecode.CreateFontStream(entryList);

                            byte[] fontArray = fontStream.GetBuffer();

                            int dataLength = fontArray.Length;

                            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
                            Marshal.Copy(fontArray, 0, ptrData, dataLength);

                            uint cFonts = 0;
                            GdiApi.AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);


                            //PASS THE FONT TO THE  PRIVATEFONTCOLLECTION OBJECT
                            pfc.AddMemoryFont(ptrData, dataLength);

                            //FREE THE "UNSAFE" MEMORY
                            Marshal.FreeCoTaskMem(ptrData);

                            GetGlyphWidths();
                            try
                            {
                                using (FontFamily ff = pfc.Families[0])
                                {
                                    embeddedFont = new System.Drawing.Font(ff, size, this.FontStyle);
                                }

                            }
                            catch
                            {
                                try
                                {
                                    using (FontFamily ff = pfc.Families[0])
                                    {
                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Regular);
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        using (FontFamily ff = pfc.Families[0])
                                        {
                                            embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold);
                                        }
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            using (FontFamily ff = pfc.Families[0])
                                            {
                                                embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Italic);
                                            }
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                using (FontFamily ff = pfc.Families[0])
                                                {
                                                    embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Underline);
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size, FontStyle.Bold | FontStyle.Italic);
                                                    }
                                                }
                                                catch
                                                {
                                                    using (FontFamily ff = pfc.Families[0])
                                                    {
                                                        embeddedFont = new System.Drawing.Font(ff, size);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return embeddedFont;
                        }
                        #endregion
                        #region FontFile3
                        else if (isEmbedded && dictionary.ContainsKey(DictionaryProperties.FontFile3))
                        {
                            //if (fontType.Value == "Type0")
                            //{
                            //    return;
                            //}
                            IsType1Font = true;
                            Is1C = true;
                            long fontReferenceNumber = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Reference.ObjNum;
                            if (!type1FontReference.ContainsKey(fontReferenceNumber))
                            {
                                PdfDictionary fontDictionary = (dictionary[DictionaryProperties.FontFile3] as PdfReferenceHolder).Object as PdfDictionary;
                                MemoryStream str = (fontDictionary as PdfStream).InternalStream;
                                string[] filter = GetFontFilter(fontDictionary);

                                if (filter != null)
                                {
                                    for (int k = 0; k < filter.Length; k++)
                                    {
                                        switch (filter[k])
                                        {
                                            case "A85":
                                            case "ASCII85Decode":
                                                {
                                                    str = DecodeASCII85Stream(str);
                                                    break;
                                                }
                                            case "FlateDecode":
                                                {
                                                    str = DecodeFlateStream(str);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                str.Capacity = (int)str.Length;
                                byte[] fontFile3Bytes = str.ToArray();

                                FontFile3 type1Font = new FontFile3();
                                m_cffGlyphs = type1Font.readType1CFontFile(fontFile3Bytes);
                                type1FontReference.Add(fontReferenceNumber, m_cffGlyphs);
                                IsCID = type1Font.isCID;
                            }
                            else
                                m_cffGlyphs = type1FontReference[fontReferenceNumber];
                        }
                        #endregion
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                #endregion
                else
                {
                    return embeddedFont;
                }
            }
            return null;
        }

        private void GetGlyphWidthsType1()
        {
            int firstChar = 0, lastChar = 0;
            PdfDictionary dictionary = m_fontDictionary;

            if (dictionary.ContainsKey(DictionaryProperties.DW))
                DefaultGlyphWidth = (dictionary[DictionaryProperties.DW] as PdfNumber).FloatValue;

            if (dictionary.ContainsKey(DictionaryProperties.FirstChar))
                firstChar = (dictionary[DictionaryProperties.FirstChar] as PdfNumber).IntValue;
            if (dictionary.ContainsKey(DictionaryProperties.LastChar))
                lastChar = (dictionary[DictionaryProperties.LastChar] as PdfNumber).IntValue;
            m_fontGlyphWidth = new Dictionary<int, int>();
            m_fontGlyphWidthMapping = new Dictionary<int, int>();
            PdfArray w = null;
            int index = 0;
            if (dictionary[DictionaryProperties.Widths] is PdfArray)
                w = dictionary[DictionaryProperties.Widths] as PdfArray;
            if (dictionary[DictionaryProperties.Widths] is PdfReferenceHolder)
                w = (dictionary[DictionaryProperties.Widths] as PdfReferenceHolder).Object as PdfArray;

            if (w != null)
                try
                {
                    for (int i = 0; i < w.Count; i++)
                    {
                        index = firstChar + i;

                        if (this.CharacterMapTable.ContainsKey(index))
                        {
                            string mappingString = CharacterMapTable[index];
                            int entryValue = (int)(mappingString.ToCharArray()[0]);
                            if (!m_fontGlyphWidthMapping.ContainsKey(entryValue))
                                m_fontGlyphWidthMapping.Add(entryValue, (w[i] as PdfNumber).IntValue);
                        }
                        else if (this.DifferencesDictionary.ContainsKey(index.ToString()))
                        {

                            string mappingString = this.DifferencesDictionary[index.ToString()];
                            int entryValue = index;
                            if (mappingString.Length == 1)
                                entryValue = (int)(mappingString.ToCharArray()[0]);
                            if (!m_fontGlyphWidthMapping.ContainsKey(entryValue))
                                m_fontGlyphWidthMapping.Add(entryValue, (w[i] as PdfNumber).IntValue);
                            //return;
                        }
                        else
                        {
                            if (!m_fontGlyphWidthMapping.ContainsKey(index))
                                m_fontGlyphWidthMapping.Add(index, (w[i] as PdfNumber).IntValue);
                        }


                        m_fontGlyphWidth.Add(index, (w[i] as PdfNumber).IntValue);

                    }
                }
                catch
                {
                    m_fontGlyphWidth = null;
                }

        }
        PdfName fontType;
        private void GetGlyphWidthsNonIdH()
        {
            if (fontType.Value == "Type3")
                return;
            int firstChar = 0, lastChar = 0;
            PdfDictionary dictionary = m_fontDictionary;
            if (dictionary.ContainsKey(DictionaryProperties.DW))
                DefaultGlyphWidth = (dictionary[DictionaryProperties.DW] as PdfNumber).FloatValue;
            if (dictionary.ContainsKey(DictionaryProperties.FirstChar))
                firstChar = (dictionary[DictionaryProperties.FirstChar] as PdfNumber).IntValue;
            if (dictionary.ContainsKey(DictionaryProperties.LastChar))
                lastChar = (dictionary[DictionaryProperties.LastChar] as PdfNumber).IntValue;
            m_fontGlyphWidth = new Dictionary<int, int>();
            PdfArray w = null;
            int index = 0;
            if (dictionary[DictionaryProperties.Widths] is PdfArray)
                w = dictionary[DictionaryProperties.Widths] as PdfArray;
            if (dictionary[DictionaryProperties.Widths] is PdfReferenceHolder)
                w = (dictionary[DictionaryProperties.Widths] as PdfReferenceHolder).Object as PdfArray;

            if (w != null)
                try
                {
                    for (int i = 0; i < w.Count; i++)
                    {
                        index = firstChar + i;
                        if (IsMappingDone && (this.CharacterMapTable.Count > 0 || this.DifferencesDictionary.Count > 0))
                        {
                            if (this.CharacterMapTable.ContainsKey(index))
                            {
                                string mappingString = CharacterMapTable[index];
                                int entryValue = index;//(int)(mappingString.ToCharArray()[0]);
                                if (!m_fontGlyphWidth.ContainsKey(entryValue))
                                    m_fontGlyphWidth.Add(entryValue, (w[i] as PdfNumber).IntValue);
                            }
                            else if (this.DifferencesDictionary.ContainsKey(index.ToString()))
                            {
                                string mappingString = this.DifferencesDictionary[index.ToString()];
                                int entryValue = index;
                                //if (mappingString.Length == 1)
                                //    entryValue = (int)(mappingString.ToCharArray()[0]);
                                if (!m_fontGlyphWidth.ContainsKey(entryValue))
                                    m_fontGlyphWidth.Add(entryValue, (w[i] as PdfNumber).IntValue);
                                //return;
                            }
                            else
                            {
                                if (!m_fontGlyphWidth.ContainsKey(index))
                                    m_fontGlyphWidth.Add(index, (w[i] as PdfNumber).IntValue);
                            }
                        }
                        else
                        {
                            m_fontGlyphWidth.Add(index, (w[i] as PdfNumber).IntValue);
                        }
                    }
                }
                catch
                {
                    m_fontGlyphWidth = null;
                }


        }
        private void GetGlyphWidths()
        {
            if (FontEncoding != "Identity-H")
                return;
            PdfDictionary dictionary = m_fontDictionary;
            if (dictionary.ContainsKey(DictionaryProperties.DescendantFonts))
            {
                PdfArray arr = null;
                if (dictionary[DictionaryProperties.DescendantFonts] is PdfArray)
                    arr = dictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                if (dictionary[DictionaryProperties.DescendantFonts] is PdfReferenceHolder)
                    arr = (dictionary[DictionaryProperties.DescendantFonts] as PdfReferenceHolder).Object as PdfArray;
                dictionary = arr[0] as PdfDictionary;
                if (dictionary == null)
                    dictionary = (arr[0] as PdfReferenceHolder).Object as PdfDictionary;
            }
            m_fontGlyphWidth = new Dictionary<int, int>();
            PdfArray w = null;
            int index = 0;
            int endIndex = 0;
            PdfArray widthArray = null;
            if (dictionary[DictionaryProperties.W] is PdfArray)
                w = dictionary[DictionaryProperties.W] as PdfArray;
            if (dictionary[DictionaryProperties.W] is PdfReferenceHolder)
                w = (dictionary[DictionaryProperties.W] as PdfReferenceHolder).Object as PdfArray;
            if (dictionary.ContainsKey(DictionaryProperties.DW))
                DefaultGlyphWidth = (dictionary[DictionaryProperties.DW] as PdfNumber).FloatValue;
            try
            {
                if (w == null)
                {
                    m_fontGlyphWidth = null;
                    return;
                }
                for (int i = 0; i < w.Count; )
                {
                    if (w[i] is PdfNumber)
                        index = (w[i] as PdfNumber).IntValue;
                    i++;
                    if (w[i] is PdfArray)
                    {
                        widthArray = (w[i] as PdfArray);
                        for (int j = 0; j < widthArray.Count; j++)
                        {
                            if (m_cidToGidTable != null)
                            {
                                m_fontGlyphWidth = null;
                                return;
                            }
                            else if (!m_containsCmap)
                                m_fontGlyphWidth.Add(index, (widthArray[j] as PdfNumber).IntValue);
                            else
                            {
                                if (this.CharacterMapTable.ContainsKey(index))
                                {
                                    string mappingString = CharacterMapTable[index];
                                    int entryValue = (int)(mappingString.ToCharArray()[0]);
                                    if (!m_fontGlyphWidth.ContainsKey(entryValue))
                                        m_fontGlyphWidth.Add(entryValue, (widthArray[j] as PdfNumber).IntValue);
                                }
                                else
                                {
                                    if (!m_fontGlyphWidth.ContainsKey(index))
                                        m_fontGlyphWidth.Add(index, (widthArray[j] as PdfNumber).IntValue);
                                }
                            }
                            index++;
                        }
                    }
                    else if (w[i] is PdfNumber)
                    {
                        endIndex = (w[i] as PdfNumber).IntValue;
                        i++;
                        for (; index <= endIndex; index++)
                        {
                            if (!m_fontGlyphWidth.ContainsKey(index))
                                m_fontGlyphWidth.Add(index, (w[i] as PdfNumber).IntValue);
                        }
                    }
                    i++;
                }
            }
            catch
            {
                m_fontGlyphWidth = null;
            }
        }

        private string[] GetFontFilter(PdfDictionary streamDictionary)
        {
            string[] fontFilter = null;

            if (streamDictionary != null)
            {
                if (streamDictionary.ContainsKey("Filter"))
                {
                    if (streamDictionary[DictionaryProperties.Filter] is PdfName)
                    {
                        fontFilter = new string[1];
                        fontFilter[0] = (streamDictionary[DictionaryProperties.Filter] as PdfName).Value;
                    }
                    else if (streamDictionary[DictionaryProperties.Filter] is PdfArray)
                    {
                        PdfArray filters = streamDictionary[DictionaryProperties.Filter] as PdfArray;
                        fontFilter = new string[filters.Count];
                        for (int i = 0; i < filters.Count; i++)
                        {
                            fontFilter[i] = (filters[i] as PdfName).Value;
                        }
                    }
                    else if (streamDictionary[DictionaryProperties.Filter] is PdfReferenceHolder)
                    {
                        PdfArray filters = (streamDictionary[DictionaryProperties.Filter] as PdfReferenceHolder).Object as PdfArray;
                        fontFilter = new string[filters.Count];
                        for (int i = 0; i < filters.Count; i++)
                        {
                            fontFilter[i] = (filters[i] as PdfName).Value;
                        }
                    }
                }
            }

            return fontFilter;
        }

        /// <summary>
        /// Decodes the ASCII85 encoded stream
        /// </summary>
        /// <param name="encodedStream">Encoded stream</param>
        /// <returns>Decoded Stream</returns>
        private MemoryStream DecodeASCII85Stream(MemoryStream encodedStream)
        {
            ASCII85 decoder = new ASCII85();
            byte[] decodedBytes = decoder.decode((encodedStream as MemoryStream).GetBuffer());

            MemoryStream outStream = new MemoryStream(decodedBytes, 0, decodedBytes.Length, true, true);
            outStream.Position = 0;
            return outStream;
        }

        /// <summary>
        /// Decodes the Flate encoded stream
        /// </summary>
        /// <param name="encodedStream">Encoded stream</param>
        /// <returns>Decoded Stream</returns>
        private MemoryStream DecodeFlateStream(MemoryStream encodedStream)
        {
            encodedStream.Position = 0;
            //Skip two bytes
            encodedStream.ReadByte();
            encodedStream.ReadByte();

            DeflateStream s = new DeflateStream(encodedStream, CompressionMode.Decompress, true);
            byte[] buffer = new byte[4096];
            MemoryStream outStream = new MemoryStream();

            do
            {
                int bytesRead = s.Read(buffer, 0, 4096);
                if (bytesRead <= 0)
                    break;
                outStream.Write(buffer, 0, bytesRead);
            } while (true);
            return outStream;
        }

        private Dictionary<double, string> GetCidToGidTable(byte[] cidTOGidmap)
        {
            Dictionary<double, string> mapTable = new Dictionary<double, string>();
            byte[] buffer = new byte[2];
            int counter = 0;
            for (int i = 0; i < cidTOGidmap.Length; i++)
            {
                buffer[0] = cidTOGidmap[i];
                buffer[1] = cidTOGidmap[++i];
                string mapValue = ASCIIEncoding.ASCII.GetString(buffer);
                mapValue = mapValue.Replace("\0", "");
                mapTable.Add(counter, mapValue);
                counter++;
            }
            return mapTable;
        }

        private Dictionary<string, double> GetReverseMapTable()
        {
            m_reverseMapTable = new Dictionary<string, double>();
            foreach (KeyValuePair<double, string> obj in CharacterMapTable)
            {
                if (!m_reverseMapTable.ContainsKey(obj.Value))
                    m_reverseMapTable.Add(obj.Value, obj.Key);
            }
            return m_reverseMapTable;
        }

        /// <summary>
        /// Builds the mapping table that is used to map the decoded text to get the expected text.
        /// </summary>
        /// <returns>
        /// A dictionary with key as the encoded element and value as the value to be mapped to.
        /// </returns>
        private Dictionary<double, string> GetCharacterMapTable()
        {
            //Dictionary<double, double> cMap = new Dictionary<double, double>();
            Dictionary<double, string> mapTable = new Dictionary<double, string>();
            if (m_fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
            {
                IPdfPrimitive unicodeMap = m_fontDictionary[DictionaryProperties.ToUnicode];


                PdfStream mapStream;
                if (unicodeMap is PdfReferenceHolder)
                {
                    mapStream = (unicodeMap as PdfReferenceHolder).Object as PdfStream;
                }
                else
                {
                    mapStream = unicodeMap as PdfStream;
                }

                if (mapStream != null)
                {
                    mapStream.Decompress();
                    string text = Encoding.UTF8.GetString(mapStream.Data, 0, mapStream.Data.Length);
                    bool isRange = true;
                    bool isBfRange = false, isBfChar = false;
                    int start, end, startCmap, endCmap, endPointer;
                    startCmap = text.IndexOf("begincmap");
                    endCmap = text.IndexOf("endcmap");
                    endPointer = startCmap;
                    start = startCmap;
                    end = endCmap;
                    if (endPointer == -1)
                        return mapTable;
                    while (true)
                    {
                        if (!isBfRange)
                        {
                            start = text.IndexOf("beginbfchar", endPointer);
                            if (start < 0)
                            {
                                isBfChar = false;
                                start = startCmap;
                                endPointer = startCmap;
                                end = endCmap;
                            }
                            else
                            {
                                end = text.IndexOf("endbfchar", start);
                                endPointer = end;
                                isRange = false;
                                isBfChar = true;
                            }
                        }
                        if (!isBfChar)
                        {
                            int bfrangestart = text.IndexOf("beginbfrange", endPointer);

                            if (bfrangestart < 0)
                            {
                                isRange = false;
                                isBfRange = false;
                            }
                            else
                            {
                                int bfrangeend = text.IndexOf("endbfrange", endPointer + 5);
                                start = bfrangestart;
                                end = bfrangeend;
                                endPointer = end;
                                isBfRange = true;
                                isRange = true;
                            }
                        }
                        if (isBfChar || isBfRange)
                        {
                            string sub = text.Substring(start, (end - start));
                            List<string> tmp = new List<string>();
                            string m_tmp = sub;
                            int m_start = 0;
                            int m_stop = 0;
                            string m_txt = null;

                            if (isBfChar)
                            {
                                char[] separator = { '\n', '\r' };
                                string[] tableEntry = sub.Split(separator);
                                for (int i = 0; i < tableEntry.Length; i++)
                                {
                                    tmp = GetHexCode(tableEntry[i]);
                                    if (tmp.Count > 1)
                                    {
                                        if (tmp[1].Length > 4)
                                        {
                                            string tableValue = tmp[1];
                                            tableValue = tableValue.Replace(" ", "");
                                            string mapValue = "";
                                            int numberOfCharacters = tableValue.Length / 4;
                                            for (int j = 0; j < numberOfCharacters; j++)
                                            {
                                                char mapChar = (char)Int64.Parse(tableValue.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                                                tableValue = tableValue.Substring(4);
                                                mapValue += mapChar.ToString();
                                            }
                                            if (!(mapTable.ContainsKey(Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber))))
                                            {
                                                mapTable.Add(Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber), mapValue.ToString());
                                            }
                                            continue;
                                        }
                                        if (!(mapTable.ContainsKey(Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber))))
                                        {
                                            char mapValue = (char)Int64.Parse(tmp[1], System.Globalization.NumberStyles.HexNumber);
                                            mapTable.Add(Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber), mapValue.ToString());
                                        }
                                    }
                                }
                            }
                            else if (isBfRange)
                            {
                                double startRange, endRange;
                                char[] separator = { '\n', '\r' };
                                string[] tableEntry = sub.Split(separator);
                                for (int i = 0; i < tableEntry.Length; i++)
                                {
                                    if (tableEntry[i].Contains("["))
                                    {
                                        int subArrayStatIndex = tableEntry[i].IndexOf("[");
                                        int subArrayEndIndex = tableEntry[i].IndexOf("]");
                                        List<string> subArray = new List<string>();
                                        string str = tableEntry[i].Substring(subArrayStatIndex, subArrayEndIndex - subArrayStatIndex);
                                        subArray = GetHexCode(str);
                                        tmp = GetHexCode(tableEntry[i]);
                                        if (tmp.Count > 1)
                                        {
                                            startRange = Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber);
                                            endRange = Int64.Parse(tmp[1], System.Globalization.NumberStyles.HexNumber);
                                            int t = 0;
                                            for (double j = startRange, k = 0; j <= endRange; j++, k++, t++)
                                            {
                                                string mapValueHex = subArray[t];
                                                double hexEquivalent = Convert.ToInt64(mapValueHex, 16);
                                                double equivalent = hexEquivalent;
                                                int hex = (int)equivalent;
                                                string hexString = hex.ToString("x");
                                                double mapValue = Int64.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                                                char mapChar = (char)mapValue;
                                                if (!mapTable.ContainsKey(j))
                                                    mapTable.Add(j, mapChar.ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmp = GetHexCode(tableEntry[i]);
                                        if (tmp.Count == 3)
                                        {

                                            startRange = Int64.Parse(tmp[0], System.Globalization.NumberStyles.HexNumber);
                                            endRange = Int64.Parse(tmp[1], System.Globalization.NumberStyles.HexNumber);
                                            string mapValueHex = tmp[2];
                                            double hexEquivalent = Convert.ToInt64(mapValueHex, 16);
                                            for (double j = startRange, k = 0; j <= endRange; j++, k++)
                                            {
                                                double equivalent = hexEquivalent + k;
                                                int hex = (int)equivalent;
                                                string hexString = hex.ToString("x");
                                                double mapValue = Int64.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                                                char mapChar = (char)mapValue;
                                                if (!mapTable.ContainsKey(j))
                                                    mapTable.Add(j, mapChar.ToString());
                                            }
                                        }
                                        else if (tmp.Count > 1)
                                        {
                                            int semiCount;
                                            semiCount = tmp.Count / 2;
                                            for (int k = 0; k < semiCount; k++)
                                            {
                                                if (!mapTable.ContainsKey(Int64.Parse(tmp[k], System.Globalization.NumberStyles.HexNumber)))
                                                {
                                                    char mapValue = (char)Int64.Parse(tmp[semiCount + k], System.Globalization.NumberStyles.HexNumber);
                                                    mapTable.Add(Int64.Parse(tmp[k], System.Globalization.NumberStyles.HexNumber), mapValue.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            // cMap.Add(mapTable);
            if (m_isSameFont == true)
            {
                foreach (KeyValuePair<double, string> item in mapTable)
                {
                    if (!tempMapTable.ContainsKey(item.Key))
                    {
                        tempMapTable.Add(item.Key, item.Value);
                    }
                    else
                    {
                        tempMapTable.Remove(item.Key);
                        tempMapTable.Add(item.Key, item.Value);
                    }
                }
            }
            return mapTable;
        }

        /// <summary>
        /// Builds the mapping table that is used to map the decoded text to get the expected text.
        /// </summary>		
        private Dictionary<string, string> GetDifferencesDictionary()
        {
            Dictionary<string, string> differencesDictionary = new Dictionary<string, string>();
            PdfDictionary encodingDictionary = null;

            if (m_fontDictionary.ContainsKey(DictionaryProperties.Encoding))
            {
                if (m_fontDictionary[DictionaryProperties.Encoding] is PdfReferenceHolder)
                {
                    encodingDictionary = (m_fontDictionary[DictionaryProperties.Encoding] as PdfReferenceHolder).Object as PdfDictionary;
                }
                else if (m_fontDictionary[DictionaryProperties.Encoding] is PdfDictionary)
                    encodingDictionary = m_fontDictionary[DictionaryProperties.Encoding] as PdfDictionary;

                if (encodingDictionary != null)
                {
                    if (encodingDictionary.ContainsKey(DictionaryProperties.Differences))
                    {
                        int differenceCount = 0;
                        PdfArray differences = encodingDictionary[DictionaryProperties.Differences] as PdfArray;
                        for (int i = 0; i < differences.Count; i++)
                        {
                            string text = string.Empty;

                            if (differences[i] is PdfNumber)
                            {
                                text = (differences[i] as PdfNumber).FloatValue.ToString();
                                differenceCount = int.Parse(text);
                            }
                            else if (differences[i] is PdfName)
                            {
                                text = (differences[i] as PdfName).Value;
                                if ((fontType.Value == "Type1") && (text == ".notdef"))
                                {
                                    text = " ";
                                    differencesDictionary.Add(differenceCount.ToString(), GetLatinCharacter(text));
                                    differenceCount++;
                                }
                                else
                                {
                                    text = GetLatinCharacter(text);
                                    text = GetSpecialCharacter(text);
                                    differencesDictionary.Add(differenceCount.ToString(), GetLatinCharacter(text));
                                    differenceCount++;
                                }
                            }
                            else
                            {
                            }
                        }

                    }
                }

            }

            return differencesDictionary;
        }

        /// <summary>
        ///  Gets Latin Character
        /// </summary>
        /// <param name="decodedCharacter">The decodedCharacter.</param>        
        /// <returns>decodedCharacter</returns>
        //Latin Character Set (APPENDIX D Pdf version-1.7) Page- 997
        internal static string GetLatinCharacter(string decodedCharacter)
        {
            switch (decodedCharacter)
            {
                case "zero":
                    return "0";
                case "one":
                    return "1";
                case "two":
                    return "2";
                case "three":
                    return "3";
                case "four":
                    return "4";
                case "five":
                    return "5";
                case "six":
                    return "6";
                case "seven":
                    return "7";
                case "eight":
                    return "8";
                case "nine":
                    return "9";
                case "aring":
                    return "å";
                case "asciicircum":
                    return "^";
                case "asciitilde":
                    return "~";
                case "asterisk":
                    return "*";
                case "at":
                    return "@";
                case "atilde":
                    return "ã";
                case "backslash":
                    return "\\";
                case "bar":
                    return "|";
                case "braceleft":
                    return "{";
                case "braceright":
                    return "}";
                case "bracketleft":
                    return "[";
                case "bracketright":
                    return "]";
                case "breve":
                    return "˘";
                case "brokenbar":
                    return "|";
                case "bullet3":
                    return "•";
                case "bullet":
                    return "•";
                case "caron":
                    return "ˇ";
                case "ccedilla":
                    return "ç";
                case "cedilla":
                    return "¸";
                case "cent":
                    return "¢";
                case "circumflex":
                    return "ˆ";
                case "colon":
                    return ":";
                case "comma":
                    return ",";
                case "copyright":
                    return "©";
                case "currency1":
                    return "¤";
                case "dagger":
                    return "†";
                case "daggerdbl":
                    return "‡";
                case "degree":
                    return "°";
                case "dieresis":
                    return "¨";
                case "divide":
                    return "÷";
                case "dollar":
                    return "$";
                case "dotaccent":
                    return "˙";
                case "dotlessi":
                    return "ı";
                case "eacute":
                    return "é";
                case "middot":
                    return "˙";
                case "edieresis":
                    return "ë";
                case "egrave":
                    return "è";
                case "ellipsis":
                    return "...";
                case "emdash":
                    return "—";
                case "endash":
                    return "–";
                case "equal":
                    return "=";
                case "eth":
                    return "ð";
                case "exclam":
                    return "!";
                case "exclamdown":
                    return "¡";
                //case "fi":
                //    return "fl";
                case "florin":
                    return "ƒ";
                case "fraction":
                    return "⁄";
                case "germandbls":
                    return "ß";
                case "grave":
                    return "`";
                case "greater":
                    return ">";
                case "guillemotleft4":
                    return "«";
                case "guillemotright4":
                    return "»";
                case "guilsinglleft":
                    return "‹";
                case "guilsinglright":
                    return "›";
                case "hungarumlaut":
                    return "˝";
                case "hyphen5":
                    return "-";
                case "iacute":
                    return "í";
                case "icircumflex":
                    return "î";
                case "idieresis":
                    return "ï";
                case "igrave":
                    return "ì";
                case "less":
                    return "<";
                case "logicalnot":
                    return "¬";
                case "lslash":
                    return "ł";
                case "macron":
                    return "¯";
                case "minus":
                    return "−";
                case "mu":
                    return "μ";
                case "multiply":
                    return "×";
                case "ntilde":
                    return "ñ";
                case "numbersign":
                    return "#";
                case "oacute":
                    return "ó";
                case "ocircumflex":
                    return "ô";
                case "odieresis":
                    return "ö";
                case "oe":
                    return "oe";
                case "ogonek":
                    return "˛";
                case "ograve":
                    return "ò";
                case "onehalf":
                    return "1/2";
                case "onequarter":
                    return "1/4";
                case "onesuperior":
                    return "¹";
                case "ordfeminine":
                    return "ª";
                case "ordmasculine":
                    return "º";
                case "oslash":
                    return "ø";
                case "otilde":
                    return "õ";
                case "paragraph":
                    return "¶";
                case "parenleft":
                    return "(";
                case "parenright":
                    return ")";
                case "percent":
                    return "%";
                case "period":
                    return ".";
                case "periodcentered":
                    return "·";
                case "perthousand":
                    return "‰";
                case "plus":
                    return "+";
                case "plusminus":
                    return "±";
                case "question":
                    return "?";
                case "questiondown":
                    return "¿";
                case "quotedbl":
                    return "\"";
                case "quotedblbase":
                    return "„";
                case "quotedblleft":
                    return "“";
                case "quotedblright":
                    return "”";
                case "quoteleft":
                    return "‘";
                case "quoteright":
                    return "’";
                case "quotesinglbase":
                    return "‚";
                case "quotesingle":
                    return "'";
                case "registered":
                    return "®";
                case "ring":
                    return "˚";
                case "scaron":
                    return "š";
                case "section":
                    return "§";
                case "semicolon":
                    return ";";
                case "slash":
                    return "/";
                case "space6":
                    return " ";
                case "space":
                    return " ";
                case "udieresis":
                    return "ü";
                case "hyphen":
                    return "-";
                case "underscore":
                    return "_";
                case "adieresis":
                    return "ä";
                case "ampersand":
                    return "&";
                case "Adieresis":
                    return "Ä";
                case "Udieresis":
                    return "Ü";
                case "ccaron":
                    return "č";
                case "Scaron":
                    return "Š";
                case "zcaron":
                    return "ž";
                case "sterling":
                    return "£";
                case "agrave":
                    return "à";
                case "ecircumflex":
                    return "ê";
                default:
                    return decodedCharacter;

            }
        }


        /// <summary>
        ///  Gets Latin Character
        /// </summary>
        /// <param name="decodedCharacter">The decodedCharacter.</param>        
        /// <returns>decodedCharacter</returns>
        //Latin Character Set (APPENDIX D Pdf version-1.7) Page- 997
        internal static string GetSpecialCharacter(string decodedCharacter)
        {
            switch (decodedCharacter)
            {
                case "head2right":
                    return "\u27A2";
                case "aacute":
                    return "a\u0301";
                case "eacute":
                    return "e\u0301";
                case "iacute":
                    return "i\u0301";
                case "oacute":
                    return "o\u0301";
                case "uacute":
                    return "u\u0301";
                case "circleright":
                    return "\u27B2";
                case "bleft":
                    return "\u21E6";
                case "bright":
                    return "\u21E8";
                case "bup":
                    return "\u21E7";
                case "bdown":
                    return "\u21E9";
                case "barb4right":
                    return "\u2794";
                case "bleftright":
                    return "\u2B04";
                case "bupdown":
                    return "\u21F3";
                case "bnw":
                    return "\u2B00";
                case "bne":
                    return "\u2B01";
                case "bsw":
                    return "\u2B03";
                case "bse":
                    return "\u2B02";
                case "bdash1":
                    return "\u25AD";
                case "bdash2":
                    return "\u25AB";
                case "xmarkbld":
                    return "\u2717";
                case "checkbld":
                    return "\u2713";
                case "boxxmarkbld":
                    return "\u2612";
                case "boxcheckbld":
                    return "\u2611";
                case "space":
                    return "\u0020";
                case "pencil":
                    return "\u270F";
                case "scissors":
                    return "\u2702";
                case "scissorscutting":
                    return "\u2701";
                case "readingglasses":
                    return "\u2701";
                case "bell":
                    return "\u2701";
                case "book":
                    return "\u2701";
                case "telephonesolid":
                    return "\u2701";
                case "telhandsetcirc":
                    return "\u2701";
                case "envelopeback":
                    return "\u2701";
                case "hourglass":
                    return "\u231B";
                case "keyboard":
                    return "\u2328";
                case "tapereel":
                    return "\u2707";
                case "handwrite":
                    return "\u270D";
                case "handv":
                    return "\u270C";
                case "handptleft":
                    return "\u261C";
                case "handptright":
                    return "\u261E";
                case "handptup":
                    return "\u261D";
                case "handptdown":
                    return "\u261F";
                case "smileface":
                    return "\u263A";
                case "frownface":
                    return "\u2639";
                case "skullcrossbones":
                    return "\u2620";
                case "flag":
                    return "\u2690";
                case "pennant":
                    return "\u1F6A9";
                case "airplane":
                    return "\u2708";
                case "sunshine":
                    return "\u263C";
                case "droplet":
                    return "\u1F4A7";
                case "snowflake":
                    return "\u2744";
                case "crossshadow":
                    return "\u271E";
                case "crossmaltese":
                    return "\u2720";
                case "starofdavid":
                    return "\u2721";
                case "crescentstar":
                    return "\u262A";
                case "yinyang":
                    return "\u262F";
                case "om":
                    return "\u0950";
                case "wheel":
                    return "\u2638";
                case "aries":
                    return "\u2648";
                case "taurus":
                    return "\u2649";
                case "gemini":
                    return "\u264A";
                case "cancer":
                    return "\u264B";
                case "leo":
                    return "\u264C";
                case "virgo":
                    return "\u264D";
                case "libra":
                    return "\u264E";
                case "scorpio":
                    return "\u264F";
                case "saggitarius":
                    return "\u2650";
                case "capricorn":
                    return "\u2651";
                case "aquarius":
                    return "\u2652";
                case "pisces":
                    return "\u2653";
                case "ampersanditlc":
                    return "\u0026";
                case "ampersandit":
                    return "\u0026";
                case "circle6":
                    return "\u25CF";
                case "circleshadowdwn":
                    return "\u274D";
                case "square6":
                    return "\u25A0";
                case "box3":
                    return "\u25A1";
                case "boxshadowdwn":
                    return "\u2751";
                case "boxshadowup":
                    return "\u2752";
                case "lozenge4":
                    return "\u2B27";
                case "lozenge6":
                    return "\u29EB";
                case "rhombus6":
                    return "\u25C6";
                case "xrhombus":
                    return "\u2756";
                case "rhombus4":
                    return "\u2B25";
                case "clear":
                    return "\u2327";
                case "escape":
                    return "\u2353";
                case "command":
                    return "\u2318";
                case "rosette":
                    return "\u2740";
                case "rosettesolid":
                    return "\u273F";
                case "quotedbllftbld":
                    return "\u275D";
                case "quotedblrtbld":
                    return "\u275E";
                case ".notdef":
                    return "\u25AF";
                case "zerosans":
                    return "\u24EA";
                case "onesans":
                    return "\u2460";
                case "twosans":
                    return "\u2461";
                case "threesans":
                    return "\u2462";
                case "foursans":
                    return "\u2463";
                case "fivesans":
                    return "\u2464";
                case "sixsans":
                    return "\u2465";
                case "sevensans":
                    return "\u2466";
                case "eightsans":
                    return "\u2467";
                case "ninesans":
                    return "\u2468";
                case "tensans":
                    return "\u2469";
                case "zerosansinv":
                    return "\u24FF";
                case "onesansinv":
                    return "\u2776";
                case "twosansinv":
                    return "\u2777";
                case "threesansinv":
                    return "\u2778";
                case "foursansinv":
                    return "\u2779";
                case "circle2":
                    return "\u00B7";
                case "circle4":
                    return "\u2022";
                case "square2":
                    return "\u25AA";
                case "ring2":
                    return "\u25CB";
                case "ringbutton2":
                    return "\u25C9";
                case "target":
                    return "\u25CE";
                case "square4":
                    return "\u25AA";
                case "box2":
                    return "\u25FB";
                case "crosstar2":
                    return "\u2726";
                case "pentastar2":
                    return "\u2605";
                case "hexstar2":
                    return "\u2736";
                case "octastar2":
                    return "\u2734";
                case "dodecastar3":
                    return "\u2739";
                case "octastar4":
                    return "\u2735";
                case "registercircle":
                    return "\u2316";
                case "cuspopen":
                    return "\u27E1";
                case "cuspopen1":
                    return "\u2311";
                case "circlestar":
                    return "\u2605";
                case "starshadow":
                    return "\u2730";
                case "deleteleft":
                    return "\u232B";
                case "deleteright":
                    return "\u2326";
                case "scissorsoutline":
                    return "\u2704";
                case "telephone":
                    return "\u260F";
                case "telhandset":
                    return "\u1F4DE";
                case "handptlft1":
                    return "\u261C";
                case "handptrt1":
                    return "\u261E";
                case "handptlftsld1":
                    return "\u261A";
                case "handptrtsld1":
                    return "\u261B";
                case "handptup1":
                    return "\u261D";
                case "handptdwn1":
                    return "\u261F";
                case "xmark":
                    return "\u2717";
                case "check":
                    return "\u2713";
                case "boxcheck":
                    return "\u2611";
                case "boxx":
                    return "\u2612";
                case "boxxbld":
                    return "\u2612";
                case "circlex":
                    return "=\u2314";
                case "circlexbld":
                    return "\u2314";
                case "prohibit":
                case "prohibitbld":
                    return "\u29B8";
                case "ampersanditaldm":
                case "ampersandbld":
                case "ampersandsans":
                case "ampersandsandm":
                    return "\u0026";
                case "interrobang":
                case "interrobangdm":
                case "interrobangsans":
                case "interrobngsandm":
                    return "\u203D";
                default:
                    return decodedCharacter;
            }

        }


        /// <summary>
        /// Takes in the decoded text and maps it with its corresponding entry in the CharacterMapTable
        /// </summary>
        /// <param name="decodedText">decoded text </param>
        /// <returns>Expected text string</returns>
        internal string MapCharactersFromTable(string decodedText)
        {
            string finalText = string.Empty;
            bool skip = false;

            foreach (char character in decodedText)
            {
                if (CharacterMapTable.ContainsKey((int)character) && !skip)
                {
                    string mappingString = CharacterMapTable[(int)character];
                    if (mappingString.Contains(m_replacementCharacter))
                    {
                        int index = mappingString.IndexOf(m_replacementCharacter);
                        mappingString = mappingString.Remove(index, 1);
                    }
                    if (mappingString.Length < 2)
                        finalText += mappingString;
                    else
                        finalText += character.ToString();
                    skip = false;
                }
                else if (tempMapTable.ContainsKey((int)character) && !skip)
                {
                    string mappingString = tempMapTable[(int)character];
                    if (mappingString.Contains(m_replacementCharacter))
                    {
                        int index = mappingString.IndexOf(m_replacementCharacter);
                        mappingString = mappingString.Remove(index, 1);
                    }
                    finalText += mappingString;
                    skip = false;
                }
                else
                {
                    if (skip)
                    {
                        switch (character.ToString())
                        {
                            case "n":
                                if (CharacterMapTable.ContainsKey((int)'\n'))
                                    finalText += CharacterMapTable[(int)'\n'];
                                break;
                            case "r":
                                if (CharacterMapTable.ContainsKey((int)'\r'))
                                    finalText += CharacterMapTable[(int)'\r'];
                                break;
                            case "b":
                                if (CharacterMapTable.ContainsKey((int)'\b'))
                                    finalText += CharacterMapTable[(int)'\b'];
                                break;
                            case "a":
                                if (CharacterMapTable.ContainsKey((int)'\a'))
                                    finalText += CharacterMapTable[(int)'\a'];
                                break;
                            case "f":
                                if (CharacterMapTable.ContainsKey((int)'\f'))
                                    finalText += CharacterMapTable[(int)'\f'];
                                break;
                            case "t":
                                if (CharacterMapTable.ContainsKey((int)'\t'))
                                    finalText += CharacterMapTable[(int)'\t'];
                                break;
                            case "v":
                                if (CharacterMapTable.ContainsKey((int)'\v'))
                                    finalText += CharacterMapTable[(int)'\v'];
                                break;
                            case "'":
                                if (CharacterMapTable.ContainsKey((int)'\''))
                                    finalText += CharacterMapTable[(int)'\''];
                                break;
                            default:
                                {
                                    if (CharacterMapTable.ContainsKey((int)character))
                                        finalText += CharacterMapTable[(int)character];
                                }
                                break;
                        }
                        skip = false;
                    }
                    else if (character == '\\')
                    {
                        skip = true;
                    }
                    else
                    {
                        if (this.DifferencesDictionary != null && this.DifferencesDictionary.Count > 0)
                            finalText += MapDifferences(character.ToString());
                        else
                            finalText += character;
                    }
                }
            }
            return finalText;
        }

        internal string MapCidToGid(string decodedText)
        {
            string finalText = string.Empty;
            bool skip = false;

            foreach (char character in decodedText)
            {
                if (m_cidToGidTable.ContainsKey((int)character) && !skip)
                {
                    string mappingString = m_cidToGidTable[(int)character];
                    if (mappingString.Contains(m_replacementCharacter))
                    {
                        int index = mappingString.IndexOf(m_replacementCharacter);
                        mappingString = mappingString.Remove(index, 1);
                    }
                    if (mappingString.Length > 0)
                        if (!CidToGidReverseMapTable.ContainsKey(mappingString[0]))
                            CidToGidReverseMapTable.Add(mappingString[0], (int)character);
                    finalText += mappingString;
                    skip = false;
                }
                else if (tempMapTable.ContainsKey((int)character) && !skip)
                {
                    string mappingString = tempMapTable[(int)character];
                    if (mappingString.Contains(m_replacementCharacter))
                    {
                        int index = mappingString.IndexOf(m_replacementCharacter);
                        mappingString = mappingString.Remove(index, 1);
                    }
                    finalText += mappingString;
                    skip = false;
                }
                else
                {
                    if (skip)
                    {
                        switch (character.ToString())
                        {
                            case "n":
                                if (m_cidToGidTable.ContainsKey((int)'\n'))
                                    finalText += CharacterMapTable[(int)'\n'];
                                break;
                            case "r":
                                if (m_cidToGidTable.ContainsKey((int)'\r'))
                                    finalText += CharacterMapTable[(int)'\r'];
                                break;
                            case "b":
                                if (m_cidToGidTable.ContainsKey((int)'\b'))
                                    finalText += CharacterMapTable[(int)'\b'];
                                break;
                            case "a":
                                if (m_cidToGidTable.ContainsKey((int)'\a'))
                                    finalText += CharacterMapTable[(int)'\a'];
                                break;
                            case "f":
                                if (m_cidToGidTable.ContainsKey((int)'\f'))
                                    finalText += CharacterMapTable[(int)'\f'];
                                break;
                            case "t":
                                if (m_cidToGidTable.ContainsKey((int)'\t'))
                                    finalText += CharacterMapTable[(int)'\t'];
                                break;
                            case "v":
                                if (m_cidToGidTable.ContainsKey((int)'\v'))
                                    finalText += CharacterMapTable[(int)'\v'];
                                break;
                            case "'":
                                if (m_cidToGidTable.ContainsKey((int)'\''))
                                    finalText += CharacterMapTable[(int)'\''];
                                break;
                            default:
                                {
                                    if (m_cidToGidTable.ContainsKey((int)character))
                                        finalText += CharacterMapTable[(int)character];
                                }
                                break;
                        }
                        skip = false;
                    }
                    else if (character == '\\')
                    {
                        skip = true;
                    }
                }
            }

            return finalText;
        }

        /// <summary>
        /// Takes in the decoded text and maps it with its corresponding entry in the CharacterMapTable
        /// </summary>
        /// <param name="encodedText">encoded text </param>
        /// <returns>Expected text string</returns>
        internal string MapDifferences(string encodedText)
        {
            string decodedText = string.Empty;
            bool skip = false;
            try
            {
                encodedText = Regex.Unescape(encodedText);
            }
            catch (ArgumentException argException)
            {
                if (!string.IsNullOrEmpty(encodedText))
                {
                    encodedText = Regex.Unescape(Regex.Escape(encodedText));
                }
                else
                {
                    throw argException;
                }
            }
            foreach (char character in encodedText)
            {
                if (DifferencesDictionary.ContainsKey(((int)character).ToString()))
                {
                    decodedText += DifferencesDictionary[((int)character).ToString()];

                    if (FontName == "Wingdings")
                    {
                        decodedText = MapDifferenceOfWingDings(decodedText);
                    }

                    string specialCharacter = PdfTextExtractor.GetSpecialCharacter(decodedText);
                    if (decodedText != specialCharacter)
                    {
                        decodedText = decodedText.Replace(decodedText, specialCharacter);
                    }
                    skip = false;
                }
                else
                {
                    if (skip)
                    {
                        switch (character)
                        {
                            case 'n':
                                if (DifferencesDictionary.ContainsKey(((int)'\n').ToString()))
                                    decodedText += DifferencesDictionary[((int)'\n').ToString()];
                                break;
                            case 'r':
                                if (DifferencesDictionary.ContainsKey(((int)'\r').ToString()))
                                    decodedText += DifferencesDictionary[((int)'\r').ToString()];
                                break;
                            default:
                                break;
                        }
                        skip = false;
                    }
                    else if (character == '\\')
                    {
                        skip = true;
                    }
                    else
                        decodedText += character;
                }
            }
            return decodedText;
        }

        private string MapDifferenceOfWingDings(string decodedText)
        {
            if (decodedText.Length > 1 && decodedText.Contains("c"))
            {
                if (decodedText.IndexOf("c") == 0)
                {
                    decodedText = decodedText.Remove(0, 1);
                    int characterValue = 0;
                    int.TryParse(decodedText, out characterValue);
                    decodedText = ((char)characterValue).ToString();
                }
            }
            return decodedText;
        }

        private string SkipEscapeSequence(string text)
        {
            if (text.Contains("\\"))
            {
                int i = text.IndexOf('\\');
                if (i + 1 != text.Length)
                {
                    string escapeSequence = text.Substring(i + 1, 1);
                    switch (escapeSequence)
                    {
                        case "a":
                            text = text.Replace("\\a", "\a");
                            break;
                        case "b":
                            text = text.Replace("\\b", "\b");
                            break;
                        case "f":
                            text = text.Replace("\\f", "\f");
                            break;
                        case "n":
                            text = text.Replace("\\n", "\n");
                            break;
                        case "r":
                            text = text.Replace("\\r", "\r");
                            break;
                        case "t":
                            text = text.Replace("\\t", "\t");
                            break;
                        case "v":
                            text = text.Replace("\\v", "\v");
                            break;
                        case "'":
                            text = text.Replace("\\'", "\'");
                            break;
                        default:
                            {
                                try
                                {
                                    text = Regex.Unescape(text);
                                }
                                catch (ArgumentException argException)
                                {
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        text = Regex.Unescape(Regex.Escape(text));
                                    }
                                    else
                                    {
                                        throw argException;
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return text;
        }
        /// <summary>
        /// Method to remove the new line character
        /// </summary>
        /// <param name="text">Text with new line character</param>
        /// <returns>Text without new line character</returns>
        private string EscapeSymbols(string text)
        {
            while (text.Contains("\n"))
            {
                text = text.Replace("\n", "");
            }
            return text;
        }

        /// <summary>
        /// Organizes the hex string enclosed within the "<" ">" brackets
        /// </summary>
        /// <param name="hexCode">Mapping string in the map table of the document</param>
        /// <returns>list of HEX entries in the string</returns>
        internal static List<string> GetHexCode(string hexCode)
        {
            List<string> tmp = new List<string>();
            string m_tmp = hexCode;
            int m_start = 0;
            int m_stop = 0;
            string m_txt = null;
            for (int j1 = 0; m_start >= 0; j1++)
            {
                m_start = m_tmp.IndexOf('<');
                m_stop = m_tmp.IndexOf('>');
                if (m_start >= 0 && m_stop >= 0)
                {
                    m_txt = m_tmp.Substring(m_start + 1, ((m_stop - 1) - m_start));
                    tmp.Add(m_txt);
                    m_tmp = m_tmp.Substring(m_stop + 1, ((m_tmp.Length - 1) - m_stop));
                }
            }
            return tmp;
        }
        internal static string GetCharCode(string decodedCharacter)
        {
            switch (decodedCharacter)
            {
                case "0":
                    return "zero";
                case "1":
                    return "one";
                case "2":
                    return "two";
                case "3":
                    return "three";
                case "4":
                    return "four";
                case "5":
                    return "five";
                case "6":
                    return "six";
                case "7":
                    return "seven";
                case "8":
                    return "eight";
                case "9":
                    return "nine";
                case "´":
                    return "acute";
                case "å":
                    return "aring";
                case "^":
                    return "asciicircum";
                case "~":
                    return "asciitilde";
                case "*":
                    return "asterisk";
                case "@":
                    return "at";
                case "ã":
                    return "atilde";
                case "\\":
                    return "backslash";
                case "|":
                    return "bar";
                case "{":
                    return "braceleft";
                case "}":
                    return "braceright";
                case "[":
                    return "bracketleft";
                case "]":
                    return "bracketright";
                case "˘":
                    return "breve";
                //case "|":
                //    return "brokenbar";
                //case "•":
                //    return "bullet3";
                case "•":
                    return "bullet";
                case "ˇ":
                    return "caron";
                case "ç":
                    return "ccedilla";
                case "¸":
                    return "cedilla";
                case "¢":
                    return "cent";
                case "ˆ":
                    return "circumflex";
                case ":":
                    return "colon";
                case ",":
                    return "comma";
                case "©":
                    return "copyright";
                case "¤":
                    return "currency1";
                case "†":
                    return "dagger";
                case "‡":
                    return "daggerdbl";
                case "°":
                    return "degree";
                case "¨":
                    return "dieresis";
                case "÷":
                    return "divide";
                case "$":
                    return "dollar";
                case "˙":
                    return "dotaccent";
                case "ı":
                    return "dotlessi";
                case "é":
                    return "eacute";
                //case "˙":
                //    return "ecircumflex";
                case "ë":
                    return "edieresis";
                case "è":
                    return "egrave";
                case "...":
                    return "ellipsis";
                case "—":
                    return "emdash";
                case "–":
                    return "endash";
                case "=":
                    return "equal";
                case "ð":
                    return "eth";
                case "!":
                    return "exclam";
                case "¡":
                    return "exclamdown";
                //case "fi":
                //    return "fl";
                case "ƒ":
                    return "florin";
                case "⁄":
                    return "fraction";
                case "ß":
                    return "germandbls";
                case "`":
                    return "grave";
                case ">":
                    return "greater";
                case "«":
                    return "guillemotleft4";
                case "»":
                    return "guillemotright4";
                case "‹":
                    return "guilsinglleft";
                case "›":
                    return "guilsinglright";
                case "˝":
                    return "hungarumlaut";
                //case "-":
                //    return "hyphen5";
                case "í":
                    return "iacute";
                case "î":
                    return "icircumflex";
                case "ï":
                    return "idieresis";
                case "ì":
                    return "igrave";
                case "<":
                    return "less";
                case "¬":
                    return "logicalnot";
                case "ł":
                    return "lslash";
                case "¯":
                    return "macron";
                case "−":
                    return "minus";
                case "μ":
                    return "mu";
                case "×":
                    return "multiply";
                case "ñ":
                    return "ntilde";
                case "#":
                    return "numbersign";
                case "ó":
                    return "oacute";
                case "ô":
                    return "ocircumflex";
                case "ö":
                    return "odieresis";
                case "oe":
                    return "oe";
                case "˛":
                    return "ogonek";
                case "ò":
                    return "ograve";
                case "1/2":
                    return "onehalf";
                case "1/4":
                    return "onequarter";
                case "¹":
                    return "onesuperior";
                case "ª":
                    return "ordfeminine";
                case "º":
                    return "ordmasculine";
                case "ø":
                    return "oslash";
                case "õ":
                    return "otilde";
                case "¶":
                    return "paragraph";
                case "(":
                    return "parenleft";
                case ")":
                    return "parenright";
                case "%":
                    return "percent";
                case ".":
                    return "period";
                case "·":
                    return "periodcentered";
                case "‰":
                    return "perthousand";
                case "+":
                    return "plus";
                case "±":
                    return "plusminus";
                case "?":
                    return "question";
                case "¿":
                    return "questiondown";
                case "\"":
                    return "quotedbl";
                case "„":
                    return "quotedblbase";
                case "“":
                    return "quotedblleft";
                case "”":
                    return "quotedblright";
                case "‘":
                    return "quoteleft";
                case "’":
                    return "quoteright";
                case "‚":
                    return "quotesinglbase";
                case "'":
                    return "quotesingle";
                case "®":
                    return "registered";
                case "˚":
                    return "ring";
                case "š":
                    return "scaron";
                case "§":
                    return "section";
                case ";":
                    return "semicolon";
                case "/":
                    return "slash";
                //case " ":
                //    return "space6";
                case " ":
                    return "space";
                case "ü":
                    return "udieresis";
                case "-":
                    return "hyphen";
                case "_":
                    return "underscore";
                case "ä":
                    return "adieresis";
                case "&":
                    return "ampersand";
                case "Ä":
                    return "Adieresis";
                case "Ü":
                    return "Udieresis";
                case "č":
                    return "ccaron";
                case "Š":
                    return "Scaron";
                case "ž":
                    return "zcaron";
                case "à":
                    return "agrave";
                case "ê":
                    return "ecircumflex";
                default:
                    return decodedCharacter;
            }
        }
        private int CalculateCheckSum(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            int pos = 0;
            int byte1 = 0;
            int byte2 = 0;
            int byte3 = 0;
            int byte4 = 0;

            for (int i = 0, len = (bytes.Length) / 4; i < len; i++)
            {
                if (i == 195)
                {
                }
                byte4 += (bytes[pos++] & 255);
                byte3 += (bytes[pos++] & 255);
                byte2 += (bytes[pos++] & 255);
                byte1 += (bytes[pos++] & 255);
            }

            int result = byte1;

            result += (byte2 << 8);
            result += (byte3 << 16);
            result += (byte4 << 24);

            return result;
        }

        internal FontStyle CheckFontStyle(string fontName)
        {
            if (fontName.Contains("Regular"))
            {
                return FontStyle.Regular;
            }
            else if (fontName.Contains("Bold"))
            {
                return FontStyle.Bold;
            }
            else if (fontName.Contains("Italic"))
            {
                return FontStyle.Italic;
            }
            return FontStyle.Regular;
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

            if (fontName.Contains("Times"))
                fontName = "Times New Roman";
            if (fontName == "Bookshelf Symbol Seven")
                fontName = "Bookshelf Symbol 7";

            if (fontName.Contains("Regular"))
                fontName = fontName.Replace("Regular", "");
            else if (fontName.Contains("Bold"))
                fontName = fontName.Replace("Bold", "");
            else if (fontName.Contains("Italic"))
                fontName = fontName.Replace("Italic", "");

            fontName = fontName.Trim();
            return fontName;
        }
        private string MapZapf(string encodedText)
        {
            string decodedtext = null;
            foreach (Char character in encodedText)
            {
                int dec = (int)character;
                string result = dec.ToString("X");
                switch (result)
                {
                    case "20":
                        decodedtext += "\u0020";
                        break;
                    case "21":
                        decodedtext += "\u2701";
                        break;
                    case "48":
                        decodedtext += "\u2605";
                        break;
                    case "57":
                        decodedtext += "\u2737";
                        break;
                    case "64":
                        decodedtext += "\u2744";
                        break;
                    case "65":
                        decodedtext += "\u2745";
                        break;
                    case "6C":
                        decodedtext += "\u25CF";
                        break;
                    case "6E":
                        decodedtext += "\u25A0";
                        break;
                    case "6F":
                        decodedtext += "\u274F";
                        break;
                    case "72":
                        decodedtext += "\u2752";
                        break;
                    case "33":
                    case "34":
                        decodedtext += "\u2714";
                        break;
                    case "35":
                    case "36":
                        decodedtext += "\u2716";
                        break;
                    case "38":
                        decodedtext+="\u2717";
                        break;
                    default:
                        decodedtext += "\u2708";
                        break;
                }
            }
            return decodedtext;
        }
        #endregion


        internal  Dictionary<int, string> GetUnicodeCharMapTable()
        {
            string  name, std_Value;
            int  std_value = 0;
            string line = null;
            unicodeCharMapTable = new Dictionary<int, string>();
            StreamReader sr = global::Syncfusion.Pdf.Properties.Resources.standard_encoding;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null)
                    break;
                string[] word = line.Split(' ');
                name = word[0];
                std_Value = word[1];
                if (char.IsDigit(std_Value[0]))
                {
                    std_value = (int)Convert.ToUInt32(std_Value, 8);
                    if (!unicodeCharMapTable.ContainsKey(std_value))
                        unicodeCharMapTable.Add(std_value, name);
                }
            }
            return unicodeCharMapTable;
        }
        internal bool IsCIDFontType()
        {
           
            bool iscid = false;
            if (m_fontDictionary.Items.ContainsKey(new PdfName(DictionaryProperties.DescendantFonts)))
            {
                PdfReferenceHolder descendantFontArrayReference = (IPdfPrimitive)m_fontDictionary.Items[new PdfName(DictionaryProperties.DescendantFonts)] as PdfReferenceHolder;
                if (descendantFontArrayReference  != null)
                {
                    PdfArray descendantFontArray = (descendantFontArrayReference as PdfReferenceHolder).Object as PdfArray;
                    if (descendantFontArray[0] as PdfReferenceHolder != null)
                    {
                        PdfDictionary descendantDictionary = (descendantFontArray[0] as PdfReferenceHolder).Object as PdfDictionary;
                        PdfName subtype = descendantDictionary[DictionaryProperties.Subtype] as PdfName;
                        if (subtype.Value == "CIDFontType2" || subtype.Value == "CIDFontType0")
                            iscid = true;
                    }
                }
            }
            return iscid;
        }
    }
}
