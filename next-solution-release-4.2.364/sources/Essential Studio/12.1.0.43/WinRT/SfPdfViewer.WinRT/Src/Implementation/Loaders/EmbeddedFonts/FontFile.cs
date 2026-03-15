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

namespace Syncfusion.Pdf
{
    internal class FontFile
    {
        #region Constants
        /// <summary>
        /// constant used in eexec and charset decode 
        /// </summary>
        private const int c1 = 52845;

        /// <summary>
        /// constant used in eexec and charset decode
        /// </summary>
        private const int c2 = 22719;
        #endregion

        #region Fields
        /// <summary>
        /// Represents the number of random bytes in stream to ignore
        /// </summary>
        private int m_skipBytes = 4;

        /// <summary>
        ///  Variable to hold the glyph string and its glyph shapes 
        /// </summary>
        private Dictionary<string, byte[]> m_glyphs = new Dictionary<string, byte[]>();

        /// <summary>
        ///  Variable to hold the character code and character
        /// </summary>
        internal Dictionary<int, string> m_differenceEncoding = new Dictionary<int, string>();

        /// <summary>
        ///  Variable to hold the font matrix
        /// </summary>
        internal double[] m_fontMatrix = new double[6];

        /// <summary>
        ///  Variable to cff glyphs 
        /// </summary>
        internal CffGlyphs m_cffGlyphs = new CffGlyphs();
        #endregion

        #region Constructor
        /// <summary>
        /// needed so CIDFOnt0 can extend 
        /// </summary>
        public void Type1()
        {
        }
        #endregion

        #region Implemetation
        /// <summary>
        /// Parse the difference encoding
        /// </summary>
        private void ParseDifferenceEncoding(StreamReader br)
        {
            string line, name, rawVal, base1, val;
            int code, ptr;
            bool isHex = true;

            while ((line = br.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("readonly"))
                    break;
                int token = 0;
                if (line.StartsWith("dup") && line.Contains("/"))
                {
                    string[] info = line.Split(new string[] { " ", "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (info.Length >= 3)
                    {
                        string dup = info[token];
                        token += 1;
                        rawVal = info[token];

                        ptr = rawVal.IndexOf('#');
                        if (ptr == -1)
                            code = int.Parse(rawVal);
                        else
                        {
                            base1 = rawVal.Substring(0, ptr);
                            val = rawVal.Substring(ptr + 1, rawVal.Length);
                            code = int.Parse(val);
                        }
                        token += 1;
                        name = info[token];

                        m_differenceEncoding.Add(code, name);

                        char c = name[0];
                        if (c == 'B' || c == 'C' || c == 'c' || c == 'G')
                        {
                            int i = 1, l = name.Length;
                            while (!isHex && i < l)
                                isHex = Char.IsLetter(name[i++]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle encoding for type1 fonts 
        /// </summary>
        internal CffGlyphs ParseType1FontFile(byte[] content)
        {

            StreamReader br = new StreamReader(new MemoryStream(content));
            string line;

            while (true)
            {

                line = br.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("/Encoding 256 array"))
                {
                    ParseDifferenceEncoding(br);
                }
                else if (line.StartsWith("/lenIV"))
                {
                    string[] vals = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    m_skipBytes = Convert.ToInt32(vals[1]);
                }
                else if (line.IndexOf("/FontMatrix") != -1)
                {

                    int startP, endP;
                    string values = "";

                    startP = line.IndexOf('[');
                    if (startP != -1)
                    {
                        endP = line.IndexOf(']');
                        values = line.Substring(startP + 1, endP - (startP + 1));
                    }
                    else
                    {
                        startP = line.IndexOf('{');
                        if (startP != -1)
                        {
                            endP = line.IndexOf('}');
                            values = line.Substring(startP + 1, endP - (startP + 1));
                        }
                    }
                    string[] matrixValues = values.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 6; i++)
                    {
                        m_fontMatrix[i] = Convert.ToDouble(matrixValues[i]);
                    }
                }
            }

            if (br != null)
            {
                try
                {
                    br.Dispose();
                }
                catch (Exception)
                {

                }
            }

            int glyphCount = 0;
            m_cffGlyphs.Glyphs = ParseEncodedContent(content);
            m_cffGlyphs.FontMatrix = m_fontMatrix;
            m_cffGlyphs.DifferenceEncoding = m_differenceEncoding;
            glyphCount = m_glyphs.Count;
            return m_cffGlyphs;
        }

        /// <summary>
        /// parse the encoded part from a type 1 font
        /// </summary>
        public Dictionary<string, byte[]> ParseEncodedContent(byte[] cont)
        {

            int glyphCount = 0;
            string line;
            string rd = "rd", nd = "nd";
            int size = cont.Length, start = -1, end = -1, i, cipher, plain;

            for (i = 4; i < size; i++)
            {
                if ((cont[i - 3] == 101) && (cont[i - 2] == 120) && (cont[i - 1] == 101) && (cont[i] == 99))
                {
                    start = i + 1;
                    while (cont[start] == 10 || cont[start] == 13)
                    {
                        start++;
                    }
                    i = size;
                }
            }

            if (start != -1)
            {
                for (i = start; i < size - 10; i++)
                {
                    if ((cont[i] == 99) && (cont[i + 1] == 108) && (cont[i + 2] == 101) && (cont[i + 3] == 97) && (cont[i + 4] == 114) && (cont[i + 5] == 116) && (cont[i + 6] == 111) && (cont[i + 7] == 109) && (cont[i + 8] == 97) && (cont[i + 9] == 114) && (cont[i + 10] == 107))
                    {
                        end = i - 1;
                        while ((cont[end] == 10) || (cont[end] == 13))
                        {
                            end--;
                        }
                        i = size;
                    }
                }
            }

            if (end == -1)
            {
                end = size;
            }

            int r = 55665;
            int n = 4;

            for (i = start; i < start + (n * 2); i++)
            {
                char c = (char)(cont[i]);
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))
                {
                    //is okay
                }
                else
                {
                    break;
                }
            }
            MemoryStream bos = new MemoryStream(end - start);
            if (start != -1)
            {

                for (i = start; i < end; i++)
                {
                    cipher = cont[i];
                    plain = (cipher ^ (r >> 8));
                    r = ((cipher + r) * c1 + c2);
                    if (i > start + n)
                    {
                        bos.WriteByte((byte)plain);
                    }

                }

                cont = bos.ToArray();
                bos.Position = 0;
            }

            StreamReader br = new StreamReader(bos);

            while (true)
            {

                line = br.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (line.StartsWith("/lenIV"))
                {
                    string[] vals = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    m_skipBytes = Convert.ToInt32(vals[1]);
                }
            }

            br.Dispose();

            int l = cont.Length;
            int p = 0;
            int subStart = -1;
            start = -1;

            while (p < l)
            {

                if (p == l)
                {
                    break;
                }

                if ((p + 11 < l) && cont[p] == 47 && cont[p + 1] == 67 && cont[p + 2] == 104 && cont[p + 3] == 97 && cont[p + 4] == 114 && cont[p + 5] == 83 && cont[p + 6] == 116 && cont[p + 7] == 114 && cont[p + 8] == 105 && cont[p + 9] == 110 && cont[p + 10] == 103 && cont[p + 11] == 115)
                {
                    start = p + 11;
                }
                else if ((p + 5 < l) && cont[p] == 47 && cont[p + 1] == 83 && cont[p + 2] == 117 && cont[p + 3] == 98 && cont[p + 4] == 114 && cont[p + 5] == 115)
                {
                    subStart = p + 6;
                }

                if (subStart > -1 && start > -1)
                {
                    break;
                }

                p++;

            }
            Dictionary<string, byte[]> glyphs = new Dictionary<string, byte[]>();

            if (start == -1)
            {

            }
            else
            {
                glyphs = ExtractFontData(m_skipBytes, cont, start, rd, l, nd);
                glyphCount = glyphs.Count;
            }

            if (subStart > -1)
            {
                glyphs = ExtractSubroutineData(m_skipBytes, cont, subStart, start, rd, l, nd);
            }

            return glyphs;
        }

        /// <summary>
        /// extract the subroutine data
        /// </summary>
        private Dictionary<string, byte[]> ExtractSubroutineData(int skipBytes, byte[] cont, int start, int charStart, string rd, int l, string nd)
        {

            int count;

            while (cont[start] == 32 || cont[start] == 10 || cont[start] == 13)
            {
                start++;
            }

            StringBuilder tmp = new StringBuilder();
            while (true)
            {
                char c = (char)cont[start];
                if (c == ' ')
                {
                    break;
                }
                tmp.Append(c);
                start++;
            }

            count = Convert.ToInt32(tmp.ToString());

            for (int i = 0; i < count; i++)
            {

                while ((start < l))
                {
                    if (((cont[start - 2] == 100) && (cont[start - 1] == 117) && (cont[start] == 112)) | (start == charStart))
                    {
                        break;
                    }
                    start++;
                }

                if (start == charStart)
                {
                    i = count;
                }
                else
                {
                    while ((cont[start + 1] == 32))
                    {
                        start++;
                    }

                    StringBuilder glyph = new StringBuilder("subrs");
                    while (true)
                    {
                        start++;
                        char c = (char)cont[start];
                        if (c == ' ')
                        {
                            break;
                        }

                        glyph.Append(c);

                    }

                    tmp = new StringBuilder();
                    while (true)
                    {
                        start++;
                        char c = (char)cont[start];
                        if (c == ' ')
                        {
                            break;
                        }
                        tmp.Append(c);

                    }

                    int byteCount = Convert.ToInt32(tmp.ToString());

                    while ((cont[start] == 32))
                    {
                        start++;
                    }

                    start = start + rd.Length + 1;
                    byte[] stream = GetStream(skipBytes, start, byteCount, cont);

                    m_glyphs.Add(glyph.ToString(), stream);
                    start = start + byteCount + nd.Length;
                }
            }
            return m_glyphs;
        }

        /// <summary>
        /// Extract Font Data
        /// </summary>
        private Dictionary<string, byte[]> ExtractFontData(int skipBytes, byte[] cont, int start, string rd, int l, string nd)
        {

            int total = cont.Length, glyphCount = 0;

            while (start < total && cont[start] != 47)
            {
                start++;
            }

            int end = start;

            while (start < l)
            {

                if (cont[end] == 47)
                {

                    end = end + 2;
                    while (end < total)
                    {

                        if (cont[end - 1] == 124 && (cont[end] == 45 || cont[end] == 48) && (cont[end + 1] == 10 || cont[end + 1] == 13))
                        {
                            break;
                        }
                        if (cont[end - 1] == 'N' && cont[end] == 'D')
                        {
                            break;
                        }

                        end++;
                    }
                }

                if (total - end < 3 || (cont[end - 1] != 47 && cont[end] == 101 && cont[end + 1] == 110 && cont[end + 2] == 100))
                {
                    break;
                }

                end++;
            }

            while (start <= end)
            {

                StringBuilder glyph = new StringBuilder(20);

                while (true)
                {
                    start++;
                    char c = (char)cont[start];
                    if (c == ' ')
                    {
                        break;
                    }
                    glyph.Append(c);

                }

                start++;

                StringBuilder tmp = new StringBuilder();
                while (true)
                {
                    char c = (char)cont[start];
                    if (c == ' ')
                    {
                        break;
                    }
                    tmp.Append(c);
                    start++;
                }

                int byteCount = Convert.ToInt32(tmp.ToString());

                while ((cont[start] == 32))
                {
                    start++;
                }

                start = start + rd.Length + 1;
                byte[] stream = GetStream(skipBytes, start, byteCount, cont);


                m_glyphs.Add(glyph.ToString(), stream);

                glyphCount++;

                start = start + byteCount + nd.Length;

                while ((start <= end) && (cont[start] != 47))
                {
                    start++;
                }

            }

            return m_glyphs;
        }

        /// <summary>
        ///extract bytestream with char data
        ///</summary>
        private byte[] GetStream(int skipBytes, int start, int byteCount, byte[] cont)
        {

            MemoryStream bos = new MemoryStream();
            int r = 4330, cipher, plain;

            for (int i = 0; i < byteCount; i++)
            {
                cipher = cont[start + i];

                plain = (cipher ^ (r >> 8));
                r = ((cipher + r) * c1 + c2);
                if (i >= skipBytes)
                {
                    sbyte test = (sbyte)plain;
                    bos.WriteByte((byte)plain);

                }
            }

            return bos.ToArray();
        }
        #endregion
    }
}
