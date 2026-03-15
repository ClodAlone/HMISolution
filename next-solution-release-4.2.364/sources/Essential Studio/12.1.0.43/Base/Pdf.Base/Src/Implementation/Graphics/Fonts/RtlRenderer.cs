#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// Renderers text and layouts it for RTL support.
    /// </summary>
    internal class RtlRenderer
    {
#region Constants
        /// <summary>
        /// Bitmap used for text shaping.
        /// </summary>
        private static Bitmap s_bmp = new Bitmap(1, 1);

        /// <summary>
        /// Open bracket symbol.
        /// </summary>
        private const char c_openBracket = '(';

        /// <summary>
        /// Close bracket symbol.
        /// </summary>
        private const char c_closeBracket = ')';

        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RtlRenderer"/> class.
        /// </summary>
        private RtlRenderer()
        {
            throw new NotImplementedException();
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Layouts text. Changes blocks position in the RTL text.
        /// Ligates the text if needed.
        /// </summary>
        /// <param name="line">Line of the text.</param>
        /// <param name="font">Font to be used for string printing.</param>
        /// <param name="rtl">Font alignment.</param>
        /// <param name="wordSpace">Indicates whether Word Spacing used or not.</param>
        /// <returns>Layout string.</returns>
        public static string[] Layout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            string[] result = null;

            if (font.Unicode)
            {
                bool useSystem = (font.Font != null);

                if (useSystem)
                {
                    result = SystemLayout(line, font, rtl, wordSpace);
                }
                else
                {
                    result = CustomLayout(line, font, rtl, wordSpace);
                }
            }
            else
            {
                result = new string[1];
                result[0] = line;
            }

            return result;
        }

        /// <summary>
        /// Layouts a string and splits it by the words and using correct lay outing.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="font">Font object.</param>
        /// <param name="rtl">Indicates whether RTL should be applied.</param>
        /// <param name="wordSpace">Indicates whether word spacing is used.</param>
        /// <returns>Array of words if converted, null otherwise.</returns>
        internal static string[] SplitLayout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            bool system = (font.Unicode && font.Font != null);
            string[] words = null;

            if (system)
            {
                words = SystemSplitLayout(line, font, rtl, wordSpace);
            }

            if (!system || words == null)
            {
                words = CustomSplitLayout(line, font, rtl, wordSpace);
            }

            return words;
        }
        #endregion

#region Implementation
        /// <summary>
        /// Determines whether the specified word is english.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// 	<c>true</c> if the specified word is english; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEnglish(string word)
        {
            char c = (word.Length > 0) ? word[0] : '\0';
            return (c >= 0 && c < 0xff);
        }

        /// <summary>
        /// Copies words remaining their order.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of the words.</param>
        /// <param name="result">The resulting array.</param>
        /// <param name="resultIndex">Index of the result.</param>
        private static void KeepOrder(string[] words, int startIndex, int count, string[] result, int resultIndex)
        {
            for (int i = 0, ri = resultIndex - count + 1; i < count; ++i, ++ri)
            {
                result[ri] = words[i + startIndex];
            }
        }

        /// <summary>
        /// Uses system API to layout the text.
        /// </summary>
        /// <param name="line">Line of the text to be layouted.</param>
        /// <param name="font">Font which is used for text printing.</param>
        /// <param name="rtl">Indicates whether we use RTL or RTL lay outing of the text container.</param>
        /// <param name="wordSpace">If true - word spacing is used.</param>
        /// <returns>Layout string.</returns>
        private static string[] SystemLayout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            string[] result = null;

            if (!wordSpace)
            {
                result = new string[1];
                result[0] = SystemLayout(line, font, rtl);
            }
            else
            {
                // Unfortunately, we have to do the following word by word 
                // if we want to get word spacing working.
                string[] words = SplitLayout(line, font, rtl, wordSpace);

                string[] convertedWords = new string[words.Length];

                for (int i = 0, len = words.Length; i < len; ++i)
                {
                    convertedWords[i] = AddChars(font, words[i]);
                }

                result = convertedWords;
            }

            return result;
        }

        /// <summary>
        /// Uses system API to layout the text.
        /// </summary>
        /// <param name="line">Line of the text to be layouted.</param>
        /// <param name="font">Font which is used for text printing.</param>
        /// <param name="rtl">Indicates whether we use RTL or RTL lay outing of the text container.</param>
        /// <returns>Layout string.</returns>
        private static string SystemLayout(string line, PdfTrueTypeFont font, bool rtl)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            // Glyph indices for the current font.
            UInt16[] glyphs = null;
            bool result = GetGlyphIndices(line, font, rtl, out glyphs);

            string layouted = null;

            // Layouted successfully.
            if (result && glyphs != null && glyphs.Length > 0)
            {
                // Add some info to the font.
                layouted = AddChars(font, glyphs);
            }
            else
            {
                // Use custom approach.
                layouted = CustomLayout(line, rtl);
                layouted = AddChars(font, layouted);
            }

            return layouted;
        }

        /// <summary>
        /// Uses manual algorithm for text lay outing.
        /// </summary>
        /// <param name="line">Line of the text to be layouted.</param>
        /// <param name="font">Font which is used for text printing.</param>
        /// <param name="rtl">Indicates whether we use RTL or RTL lay outing of the text container.</param>
        /// <param name="wordSpace">If true - word spacing is used.</param>
        /// <returns>layout string array.</returns>
        private static string[] CustomLayout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            // We have unicode font, but from the file.
            string layouted = CustomLayout(line, rtl);
            string[] result = null;

            // Split the text by words if word spacing is not default.
            if (wordSpace)
            {
                string[] words = layouted.Split(null);
                int count = words.Length;

                for (int i = 0; i < count; i++)
                {
                    words[i] = AddChars(font, words[i]);
                }

                result = words;
            }
            else
            {
                result = new string[1];
                result[0] = AddChars(font, layouted);
            }

            return result;
        }

        /// <summary>
        /// Uses manual algorithm for text lay outing.
        /// </summary>
        /// <param name="line">Line of the text to be layouted.</param>
        /// <param name="rtl">Indicates whether we use RTL or RTL lay outing of the text container.</param>
        /// <returns>layout string.</returns>
        private static string CustomLayout(string line, bool rtl)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            string result = null;

            if (rtl)
            {
                result = CustomRtl(line);
            }
            else
            {
                result = CustomLtr(line);
            }

            return result;
        }

        /// <summary>
        /// Reverses the words if they're RTL.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <returns>The reversed words.</returns>
        /// <remarks>Keep English words in original order.</remarks>
        private static string[] ReverseWords(string[] words)
        {
            if (words == null)
            {
                throw new ArgumentNullException("words");
            }

            int count = words.Length;
            string[] result = new string[count];

            string word = null;
            int rule = 0;
            int engCount = 0;

            for (int i = 0, ci = count - 1; i < count; )
            {
                switch (rule)
                {
                    case 0: // start
                        word = words[i];

                        if (IsEnglish(word))
                        {
                            engCount = 0;
                            rule = 1;
                        }
                        else
                        {
                            result[ci] = word;
                            ++i;
                            --ci;
                        }

                        break;

                    case 1: // english
                        ++engCount;
                        ++i;
                        if (i < count)
                        {
                            word = words[i];
                        }

                        if (!(i < count && IsEnglish(word)))
                        {
                            KeepOrder(words, i - engCount, engCount, result, ci);
                            ci -= engCount;
                            rule = 0;
                        }

                        break;

                    default: // error
                        throw new PdfException("Internal error.");
                }
            }

            return result;
        }
        #endregion

#region System Layout methods
        /// <summary>
        /// Retrieves array of glyph indices.
        /// </summary>
        /// <param name="line">Line of the text.</param>
        /// <param name="font">Current font.</param>
        /// <param name="rtl">Indicates whether we use RTL or RTL lay outing of the text container.</param>
        /// <param name="glyphs">Array of glyph indices.</param>
        /// <returns>True - if succeed, False otherwise.</returns>
        internal static bool GetGlyphIndices(string line, PdfTrueTypeFont font, bool rtl, out UInt16[] glyphs)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            glyphs = null;

            RtlApi.SCRIPT_ITEM[] items = null;
            int count = 0;

            // Use Uniscribe.
            // Split the text to the groups.
            bool result = StringItemize(line, rtl, out items, out count);

            if (result)
            {
                int[] visualToLogical = null;
                int[] logicalToVisual = null;
                byte[] bidi = GetBidiLevel(items, count);

                // Change layout of the groups.
                result = StringLayout(bidi, count, out visualToLogical, out logicalToVisual);

                if (result)
                {
                    // Retieve glyphs used during text rendering.
                    result = StringShape(line, items, font.Font, count, visualToLogical, out glyphs);
                }
            }

            return result;
        }

        /// <summary>
        /// Breaks string to the blocks of the runs.
        /// </summary>
        /// <param name="text">String to be itemized.</param>
        /// <param name="rtl">Indicates whether text container is in RTL form or not.</param>
        /// <param name="items">Array describing each run.</param>
        /// <param name="count">Count of the runs in the string.</param>
        /// <returns>True - if operation succeed, False otherwise.</returns>
        private static bool StringItemize(string text, bool rtl, out RtlApi.SCRIPT_ITEM[] items, out int count)
        {
            if (text == null || text.Length == 0)
            {
                items = null;
                count = 0;

                return false;
            }

            // To prevent dead loop we try to itemize for maxTries times.
            int maxTries = 10;
            uint hr = 0;
            int numTries = 0;
            int itemsCount = text.Length + 1;

            count = 0;

            do
            {
                RtlApi.SCRIPT_CONTROL sc = new RtlApi.SCRIPT_CONTROL();
                RtlApi.SCRIPT_STATE ss = new RtlApi.SCRIPT_STATE();

                itemsCount = Math.Max(RtlApi.DefaultBuffSize, itemsCount);
                items = new RtlApi.SCRIPT_ITEM[itemsCount];

                // If the container is RTL - we have to let it to know.
                if (rtl)
                {
                    ss.val |= RtlApi.RtlLayout;
                }

                //Note : With 64-bit machines, uniscribe has a different behavior.
                hr = (IntPtr.Size == 8) ?
                    RtlApi.ScriptItemize(text, text.Length, itemsCount,
                    ref sc, ref ss, items[0], ref count) :
                    RtlApi.ScriptItemize(text, text.Length, itemsCount,
                       ref sc, ref ss, ref items[0], ref count);

                if ((hr != RtlApi.S_OK && hr != RtlApi.E_OUTOFMEMORY) ||
                    (hr != RtlApi.S_OK && numTries >= maxTries))
                {
                    items = null;
                    count = 0;

                    return false;
                }

                numTries++;
                itemsCount *= 2;

            } while (hr != RtlApi.S_OK);

            return true;
        }

        /// <summary>
        /// Renders each run from the string.
        /// </summary>
        /// <param name="text">Input string text.</param>
        /// <param name="items">Run descriptors.</param>
        /// <param name="font">Font to be used for text printing.</param>
        /// <param name="count">Count of the significant runs in the array.</param>
        /// <param name="visualToLogical">Visual to logical order of the runs.</param>
        /// <param name="glyphs">Resulting glyphs for the specified font.</param>
        /// <returns>True - if operation succeed, False otherwise.</returns>
        private static bool StringShape(string text, RtlApi.SCRIPT_ITEM[] items, Font font, int count, int[] visualToLogical, out UInt16[] glyphs)
        {
            if (text == null || text.Length == 0 ||
                items == null || items.Length < count + 1 || font == null ||
                visualToLogical == null || visualToLogical.Length != count)
            {
                glyphs = null;
                return false;
            }

            IntPtr hFont = IntPtr.Zero;
            try
            {
                // Create resources.
                hFont = font.ToHfont();
            }
            catch
            {
                //Note :  This is an workaround through which fonts are re-substituted if
                //if the specified font is not valid.
                font = new Font("Arial", font.Size, font.Style);
                hFont = font.ToHfont();
            }

            System.Drawing.Graphics g;

            lock (s_bmp)
            {
                g = System.Drawing.Graphics.FromImage(s_bmp);
            }

            IntPtr hDC = g.GetHdc();
            IntPtr hOldObj = GdiApi.SelectObject(hDC, hFont);
            IntPtr scriptCache = IntPtr.Zero;

            int maxTries = 10;
            ArrayList glyphsList = new ArrayList();

            try
            {
                // Render all runs.
                for (int index = 0; index < count; index++)
                {
                    // Get index of the visual run.
                    int visualIndex = visualToLogical[index];

                    // Extract run.
                    int length = items[visualIndex + 1].iCharPos;
                    string run = text.Substring(items[visualIndex].iCharPos,
                        length - items[visualIndex].iCharPos);
                    int numTries = 0;
                    uint hr = 0;
                    int itemsCount = 0;
                    UInt16[] pwOutGlyphs = null;
                    int resCount = 0;

                    // Start to render the run.
                    do
                    {
                        // Suggested in MSDN as initial capacity.
                        itemsCount += run.Length * 3 / 2;
                        pwOutGlyphs = new UInt16[itemsCount];

                        UInt16[] pwLogClust = new UInt16[itemsCount];
                        RtlApi.SCRIPT_VISATTR[] psva = new RtlApi.SCRIPT_VISATTR[itemsCount];

                        resCount = 0;

                        hr = RtlApi.ScriptShape(hDC, ref scriptCache, run, run.Length, itemsCount,
                            ref items[visualIndex].a, ref pwOutGlyphs[0], ref pwLogClust[0],
                            ref psva[0], ref resCount);

                        // Set default script and try again. 
                        // Current font doesn't contain required glyphs.
                        if (hr == RtlApi.USP_E_SCRIPT_NOT_IN_FONT)
                        {
                            items[visualIndex].a.val &= RtlApi.ScriptUndefinedMask;
                        }
                        else if ((hr != RtlApi.S_OK && hr != RtlApi.E_OUTOFMEMORY) ||
                            (hr != RtlApi.S_OK && numTries >= maxTries))
                        {
                            // Unknown error. Exit.
                            glyphs = null;
                            return false;
                        }

                        numTries++;
                    }
                    while (hr != RtlApi.S_OK);

                    // Add glyphs to the array.
                    if (hr == RtlApi.S_OK)
                    {
                        AddGlyphs(glyphsList, pwOutGlyphs, resCount);
                    }
                }
            }
            finally
            {
                // Dispose resources.
                GdiApi.SelectObject(hDC, hOldObj);
                g.ReleaseHdc(hDC);
                g.Dispose();
                g = null;
                GdiApi.DeleteObject(hFont);
            }

            glyphs = (UInt16[])glyphsList.ToArray(typeof(UInt16));
            return true;
        }

        /// <summary>
        /// Layouts the runs in the visual form.
        /// </summary>
        /// <param name="bidi">Bidi array of the runs.</param>
        /// <param name="count">Count of the runs.</param>
        /// <param name="visualToLogical">Pointer to an array that receives the run levels reordered to visual order. </param>
        /// <param name="logicalToVisual">Pointer to an array that receives the visual run positions.</param>
        /// <returns>True - if succeed, False otherwise.</returns>
        private static bool StringLayout(byte[] bidi, int count, out int[] visualToLogical, out int[] logicalToVisual)
        {
            visualToLogical = null;
            logicalToVisual = null;

            bool result = false;

            if (bidi != null && bidi.Length == count && count > 0)
            {
                visualToLogical = new int[count];
                logicalToVisual = new int[count];
                result = true;

                uint hr = RtlApi.ScriptLayout(count, ref bidi[0],
                    ref visualToLogical[0], ref logicalToVisual[0]);

                if (hr != RtlApi.S_OK)
                {
                    result = false;
                    visualToLogical = null;
                    logicalToVisual = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds glyphs to the array.
        /// </summary>
        /// <param name="glyphs">Array of the glyphs.</param>
        /// <param name="pwOutGlyphs">Contains glyphs.</param>
        /// <param name="count">Count of the glyphs.</param>
        private static void AddGlyphs(ArrayList glyphs, UInt16[] pwOutGlyphs, int count)
        {
            if (glyphs == null || pwOutGlyphs == null || pwOutGlyphs.Length < count)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                UInt16 glyph = pwOutGlyphs[i];
                glyphs.Add(glyph);
            }
        }

        /// <summary>
        /// Gets bidi level for the runs.
        /// </summary>
        /// <param name="items">Runs desciprtors.</param>
        /// <param name="count">Count of the runs</param>
        /// <returns>Bidi level array.</returns>
        private static byte[] GetBidiLevel(RtlApi.SCRIPT_ITEM[] items, int count)
        {
            byte[] bidi = null;

            if (items != null && items.Length >= count)
            {
                bidi = new byte[count];

                for (int i = 0; i < count; i++)
                {
                    RtlApi.SCRIPT_STATE state = items[i].a.s;
                    int res = RtlApi.Decrypt(state.val, 0, 5);

                    bidi[i] = (byte)res;
                }
            }

            return bidi;
        }

        /// <summary>
        /// Add information about used glyphs to the font.
        /// </summary>
        /// <param name="font">Font used for text rendering.</param>
        /// <param name="glyphs">Array of used glyphs.</param>
        /// <returns>String in the form to be written to the file.</returns>
        private static string AddChars(PdfTrueTypeFont font, UInt16[] glyphs)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            if (glyphs == null)
            {
                throw new ArgumentNullException("glyphs");
            }

            // Mark the chars as used.
            string text = null;

            font.SetSymbols(glyphs);

            // Create string from the glyphs.
            char[] chars = new char[glyphs.Length];

            for (int i = 0; i < glyphs.Length; i++)
            {
                chars[i] = (char)glyphs[i];
            }

            text = new string(chars);

            byte[] bytes = PdfString.ToUnicodeArray(text, false);

            text = PdfString.ByteToString(bytes);

            return text;
        }

        /// <summary>
        /// Add information about used glyphs to the font.
        /// </summary>
        /// <param name="font">Font used for text rendering.</param>
        /// <param name="line">Line of the text.</param>
        /// <returns>String in the form to be written to the file.</returns>
        private static string AddChars(PdfTrueTypeFont font, string line)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            string text = line;

            UnicodeTrueTypeFont internalFont = font.InternalFont as UnicodeTrueTypeFont;
            TtfReader ttfReader = internalFont.TtfReader;

            font.SetSymbols(text);

            // Reconvert string according to unicode standard.
            text = ttfReader.ConvertString(text);

            byte[] bytes = PdfString.ToUnicodeArray(text, false);

            text = PdfString.ByteToString(bytes);

            return text;
        }

        /// <summary>
        /// Layouts a string and splits it by the words by using system lay outing.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="font">Font object.</param>
        /// <param name="rtl">Indicates whether RTL should be applied.</param>
        /// <param name="wordSpace">Indicates whether word spacing is used.</param>
        /// <returns>Array of words if converted, null otherwise.</returns>
        private static string[] SystemSplitLayout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            string[] words = null;
            UInt16[] glyphs = null;
            bool result = GetGlyphIndices(line, font, rtl, out glyphs);

            if (result && glyphs != null && glyphs.Length > 0)
            {
                TtfReader reader = (font.InternalFont as UnicodeTrueTypeFont).TtfReader;
                char[] chars = new char[glyphs.Length];

                for (int i = 0; i < glyphs.Length; i++)
                {
                    int index = glyphs[i];
                    TtfGlyphInfo glyph = reader.GetGlyph(index);
                    char ch = (char)0;

                    if (!glyph.Empty)
                    {
                        ch = (char)glyph.CharCode;
                        chars[i] = ch;
                    }
                }

                string reversedLine = new string(chars);

                words = reversedLine.Split(null);
            }

            return words;
        }
        #endregion

#region Custom Layout methods
        /// <summary>
        /// Converts string data to RtL format if data contain any RtL symbols.
        /// </summary>
        /// <param name="text">Text data being converted.</param>
        /// <returns>Converted data.</returns>
        private static string CustomRtl(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            string original = text;
            char[] workingData = original.ToCharArray();

            // indicates if last processed symbol was in RTL or LTR format.
            bool lastProcessedIsRTL = true;

            // indicates if next symbol is word or digit is in RTL or LTR format.
            bool nextSymbolRTL = true;

            // Get array of flags about each symbol.
            ushort[] characterCodes = new ushort[text.Length];
            KernelApi.GetStringTypeExW(0x800, StringInfoType.CT_TYPE2, text,
                text.Length, characterCodes);

            int indexCursor = characterCodes.Length - 1;
            int indexLength = 0;

            // Convert string data only if RTL symbols present in the string
            if (ContainsRTLSymbol(characterCodes))
            {
                for (int i = 0, len = characterCodes.Length; i < len; i++)
                {
                    char currentSymbol = original[i];
                    ushort currentCode = characterCodes[i];

                    // revers LTR text in RTL format
                    if (IsLTRText(currentCode))
                    {
                        WriteInLTR(workingData, currentSymbol, true, ref indexCursor, ref indexLength);
                        lastProcessedIsRTL = false;
                    }
                    else if (IsRTLSymbol(currentCode))
                    {
                        SaveSymbol(workingData, currentSymbol, true, ref indexCursor, ref indexLength);
                        lastProcessedIsRTL = true;
                    }
                    else if (IsGeneralEuroNumber(currentCode) ||
                        (IsEuroTerminator(currentCode) &&
                        IsBackEuroNumber(characterCodes, i) && lastProcessedIsRTL))
                    {
                        WriteInLTR(workingData, currentSymbol, true, ref indexCursor, ref indexLength);
                    }
                    else if (IsBracket(currentSymbol))
                    {
                        ReverseBrackets(workingData, currentSymbol, ref indexCursor, ref indexLength);
                    }
                    else
                    {
                        for (int j = i, lenSub = characterCodes.Length; j < lenSub; j++)
                        {
                            if (IsLTRText(characterCodes[j]) ||
                                (!lastProcessedIsRTL && IsEuroNumber(characterCodes[j])))
                            {
                                nextSymbolRTL = false;
                                j = lenSub;

                            }
                            else if (IsRTLSymbol(characterCodes[j]) ||
                                (lastProcessedIsRTL && IsEuroNumber(characterCodes[j])))
                            {
                                nextSymbolRTL = true;
                                j = lenSub;
                            }
                        }

                        if (!lastProcessedIsRTL && !nextSymbolRTL)
                        {
                            WriteInLTR(workingData, currentSymbol, true, ref indexCursor, ref indexLength);
                        }
                        else
                        {
                            SaveSymbol(workingData, currentSymbol, true, ref indexCursor, ref indexLength);
                        }
                    }
                }
            }

            string resultData = new string(workingData);

            return resultData;
        }

        /// <summary>
        /// Converts string data to LtR format if data contain any RtL symbols.
        /// </summary>
        /// <param name="text">Text data being converted.</param>
        /// <returns>Converted data.</returns>
        private static string CustomLtr(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            string original = text;
            char[] workingData = original.ToCharArray();

            // indicates if last processed symbol was in RTL or LTR format.
            bool lastProcessedIsRTL = true;

            // indicates if next symbol is word or digit is in RTL or LTR format.
            bool nextSymbolRTL = true;

            // Get array of flags about each symbol.
            ushort[] characterCodes = new ushort[text.Length];

            KernelApi.GetStringTypeExW(0x800, StringInfoType.CT_TYPE2, text,
                text.Length, characterCodes);

            int indexCursor = 0;
            int indexLength = 0;

            // Convert string data only if RTL symbols present in the string.
            if (ContainsRTLSymbol(characterCodes))
            {
                // check of presence in text RTL symbol
                for (int i = 0, len = characterCodes.Length; i < len; ++i)
                {
                    char currentSymbol = original[i];
                    ushort currentCode = characterCodes[i];

                    if (IsRTLSymbol(currentCode))
                    {
                        WriteInLTR(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                        lastProcessedIsRTL = false;
                    }
                    else if (IsLTRText(currentCode))
                    {
                        SaveSymbol(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                        lastProcessedIsRTL = true;
                    }
                    else if (IsGeneralEuroNumber(currentCode) ||
                        (IsEuroTerminator(currentCode) && IsNextEuroNumber(characterCodes, i)))
                    {
                        if (lastProcessedIsRTL)
                        {
                            SaveSymbol(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                            lastProcessedIsRTL = true;
                        }
                        else
                        {
                            WriteInLTR(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                            indexCursor++;
                            indexLength--;
                            lastProcessedIsRTL = false;
                        }
                    }
                    // revers WHITESPACE
                    else if (IsWhitespace(currentCode) &&
                        IsBackEuroNumber(characterCodes, i) && !lastProcessedIsRTL)
                    {
                        int k = indexLength + indexCursor;
                        while (IsEuroNumber(characterCodes[k - 1]))
                        {
                            indexLength++;
                            indexCursor--;
                            k--;
                        }

                        WriteInLTR(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                        lastProcessedIsRTL = false;

                    }
                    // revers all others data.
                    else
                    {
                        for (int j = i, lenSub = characterCodes.Length; j < lenSub; ++j)
                        {
                            if (IsRTLText(characterCodes[j]) ||
                                (!lastProcessedIsRTL && IsEuroNumber(characterCodes[j])))
                            {
                                nextSymbolRTL = false;
                                j = lenSub;

                            }
                            else if (IsLTRText(characterCodes[j]) ||
                                (lastProcessedIsRTL && IsEuroNumber(characterCodes[j])))
                            {
                                nextSymbolRTL = true;
                                j = lenSub;
                            }
                        }

                        if (!lastProcessedIsRTL && !nextSymbolRTL)
                        {
                            WriteInLTR(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                        }
                        else
                        {
                            SaveSymbol(workingData, currentSymbol, false, ref indexCursor, ref indexLength);
                        }
                    }
                }
            }

            string resultData = new string(workingData);

            return resultData;
        }

        /// <summary>
        /// Checks if current symbol is euro number.
        /// </summary>
        /// <param name="characterCodes">Array of elements types.</param>
        /// <param name="index">Index of current symbol.</param>
        /// <returns>True - if current symbol is euro number, False otherwise.</returns>
        private static bool IsNextEuroNumber(ushort[] characterCodes, int index)
        {
            if (characterCodes == null)
            {
                throw new ArgumentNullException("characterCodes");
            }

            if (index + 1 > characterCodes.Length) return false;

            bool isNextNumber = false;

            if (index < characterCodes.Length - 1 && index >= 0)
            {
                isNextNumber = (characterCodes[index + 1] == (ushort)StringInfoCtype2.C2_EUROPENUMBER);
            }

            return isNextNumber;
        }

        /// <summary>
        /// Checks if current symbol is euro number.
        /// </summary>
        /// <param name="characterCodes">Array of elements types.</param>
        /// <param name="index">Index of current symbol.</param>
        /// <returns>True - if current symbol is euro number, False otherwise.</returns>
        private static bool IsBackEuroNumber(ushort[] characterCodes, int index)
        {
            if (characterCodes == null)
            {
                throw new ArgumentNullException("characterCodes");
            }

            if (index - 1 < 0) return false;

            bool isNextNumber = false;

            if (index < characterCodes.Length - 1 && index >= 0)
            {
                isNextNumber = (characterCodes[index - 1] == (ushort)StringInfoCtype2.C2_EUROPENUMBER);
            }

            return isNextNumber;
        }

        /// <summary>
        /// Presevres symbol fro source string data and saves it to new string data.
        /// </summary>
        /// <param name="convertedData">Array of new resulting data.</param>
        /// <param name="symbol">Current processing symbol.</param>
        /// <param name="rtl">Indicates if we process text in RTL or not.</param>
        /// <param name="indexCursor">Index of current symbol.</param>
        /// <param name="indexLength">Length of symbols group.</param>
        private static void SaveSymbol(char[] convertedData, char symbol, bool rtl, ref int indexCursor, ref int indexLength)
        {
            if (convertedData == null)
            {
                throw new ArgumentNullException("convertedData");
            }

            indexCursor = (rtl) ? indexCursor - indexLength : indexCursor + indexLength;
            indexLength = 0;
            convertedData[indexCursor] = symbol;
            indexCursor = (rtl) ? indexCursor - 1 : indexCursor + 1;
        }

        /// <summary>
        /// Checks if array of flags contains at least on RTL symbol.
        /// </summary>
        /// <param name="characterCodes">Array of flags.</param>
        /// <returns>True if array of flags contains at least on RTL symbol, False otherwise.</returns>
        private static bool ContainsRTLSymbol(ushort[] characterCodes)
        {
            if (characterCodes == null)
            {
                throw new ArgumentNullException("characterCodes");
            }

            bool isRTL = false;

            for (int i = 0, len = characterCodes.Length; i < len; ++i)
            {
                if (characterCodes[i] == (ushort)StringInfoCtype2.C2_RIGHTTOLEFT ||
                    characterCodes[i] == (ushort)StringInfoCtype2.C2_ARABICNUMBER)
                {
                    isRTL = true;
                    break;
                }
            }

            return isRTL;
        }

        /// <summary>
        /// Formats text which is writing from left to right.
        /// </summary>
        /// <param name="convertedData">Array of symbols.</param>
        /// <param name="symbol">Current symbol.</param>
        /// <param name="rtl">Indicates if we process text in RTL or not.</param>
        /// <param name="indexCursor">Index of current symbol.</param>
        /// <param name="indexLength">Length of symbols group.</param>
        private static void WriteInLTR(char[] convertedData, char symbol, bool rtl, ref int indexCursor, ref int indexLength)
        {
            if (convertedData == null)
            {
                throw new ArgumentNullException("convertedData");
            }

            if (rtl)
            {
                for (int j = indexLength; j > 0; j--)
                {
                    convertedData[indexCursor - j] = convertedData[indexCursor - j + 1];
                }

                convertedData[indexCursor] = symbol;
                indexLength++;
            }
            else
            {
                for (int j = indexLength; j > 0; j--)
                {
                    convertedData[j + indexCursor] = convertedData[j + indexCursor - 1];
                }

                convertedData[indexCursor] = symbol;
                indexLength++;
            }
        }

        /// <summary>
        /// Reverses brackets in the text.
        /// </summary>
        /// <param name="convertedData">Array of symbols.</param>
        /// <param name="symbol">Current symbol.</param>
        /// <param name="indexCursor">Index of current symbol.</param>
        /// <param name="indexLength">Length of symbols group.</param>
        private static void ReverseBrackets(char[] convertedData, char symbol, ref int indexCursor, ref int indexLength)
        {
            if (convertedData == null)
                throw new ArgumentNullException("convertedData");

            indexCursor = indexCursor - indexLength;
            indexLength = 0;
            convertedData[indexCursor] = (symbol == c_openBracket) ?
            c_closeBracket : c_openBracket;
            indexCursor--;
        }

        /// <summary>
        /// Checks if symbol code is LTR text.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is LTR text, False othervise.</returns>
        private static bool IsLTRText(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_LEFTTORIGHT);
        }

        /// <summary>
        /// Checks if symbol code is RTL text or number.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is RTL text or number, False othervise.</returns>
        private static bool IsRTLSymbol(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_RIGHTTOLEFT ||
                symbolCode == (ushort)StringInfoCtype2.C2_ARABICNUMBER);
        }

        /// <summary>
        /// Checks if symbol code is RTL text.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is RTL text, False othervise.</returns>
        private static bool IsRTLText(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_RIGHTTOLEFT);
        }

        /// <summary>
        /// Checks if symbol code is euro number with separators.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is euro number with separators, False othervise.</returns>
        private static bool IsGeneralEuroNumber(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_EUROPENUMBER ||
                symbolCode == (ushort)StringInfoCtype2.C2_EUROPESEPARATOR);
        }

        /// <summary>
        /// Checks if symbol code is euro number.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is euro number, False othervise.</returns>
        private static bool IsEuroNumber(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_EUROPENUMBER);
        }

        /// <summary>
        /// Checks if symbol code has euro terminator format.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code has euro terminator format, False othervise.</returns>
        private static bool IsEuroTerminator(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_EUROPETERMINATOR);
        }

        /// <summary>
        /// Checks if symbol code is whitespace.
        /// </summary>
        /// <param name="symbolCode">Symbol code.</param>
        /// <returns>True - if symbol code is whitespace, False othervise.</returns>
        private static bool IsWhitespace(ushort symbolCode)
        {
            return (symbolCode == (ushort)StringInfoCtype2.C2_WHITESPACE);
        }

        /// <summary>
        /// Checks if symbol is bracket.
        /// </summary>
        /// <param name="symbol">Symbol code.</param>
        /// <returns>True - if symbol is bracket, False othervise.</returns>
        private static bool IsBracket(char symbol)
        {
            return (symbol == c_openBracket || symbol == c_closeBracket);
        }

        /// <summary>
        /// Layouts a string and splits it by the words by using custom lay outing.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="font">Font object.</param>
        /// <param name="rtl">Indicates whether RTL should be applied.</param>
        /// <param name="wordSpace">Indicates whether word spacing is used.</param>
        /// <returns>Array of words if converted, null otherwise.</returns>
        private static string[] CustomSplitLayout(string line, PdfTrueTypeFont font, bool rtl, bool wordSpace)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            string reversedLine = CustomLayout(line, rtl);
            string[] words = reversedLine.Split(null);

            return words;
        }
        #endregion
    }
}
#endif