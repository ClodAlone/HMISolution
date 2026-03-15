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
using System.Collections;

namespace Syncfusion.Windows.Shared
{
    /// <exclude/>
    public class SimpleTextBoundary
    {
        // Constants
        /// <summary>
        /// 
        /// </summary>
        public const string BreakChars = "*=()[]{}<>";
        /// <summary>
        /// 
        /// </summary>
        public static int ENGLISH = 1;
        /// <summary>
        /// 
        /// </summary>
        public static int FRENCH = 2;
        /// <summary>
        /// 
        /// </summary>
        public static int GERMAN = 3;

        /// <summary>
        /// 
        /// </summary>
        public const string NoBreakButLookIntoChars = @"/\-";
        /// <summary>
        /// 
        /// </summary>
        public const int NoBreakButLookIntoCharsLength = 3;
        /// <summary>
        /// 
        /// </summary>
        public static char[] noBreakChars = new char[] { 
            ',', '.', '!', '@', '#', '$', '%', '^', '&', '+', '|', ';', ':', '\'', '"', '?', 
            '~', '`', '’', '”', '\x001f'
        };

        // Fields
        private int m_eLanguageParsing = ENGLISH;
        private bool m_bSeparateHyphenWords = false;
        private int m_nPos;
        /// <summary>
        /// 
        /// </summary>
        protected string m_sText;
        private bool m_V2Parser = true;

        // Properties
        /// <summary>
        /// 
        /// </summary>
        public int LanguageParsing
        {
            get
            {
                return m_eLanguageParsing;
            }
            set
            {
                m_eLanguageParsing = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SeparateHyphenWords
        {
            get
            {
                return m_bSeparateHyphenWords;
            }
            set
            {
                m_bSeparateHyphenWords = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool V2Parser
        {
            get
            {
                return m_V2Parser;
            }
            set
            {
                m_V2Parser = value;
            }
        }

        // Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="message"></param>
        /// <param name="allowEnd"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void CheckLegalOffset(int offset, string message, bool allowEnd)
        {
            if (((offset < 0) || (offset > m_sText.Length)) || (!allowEnd && (offset == m_sText.Length)))
            {
                throw new ArgumentOutOfRangeException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual int Following(int offset)
        {
            CheckLegalOffset(offset, "Following(" + offset + ") offset out of bounds", false);
            m_nPos = offset;
            bool flag = IsNonWhiteSpace(m_nPos);
            while ((m_nPos < m_sText.Length) && (IsNonWhiteSpace(m_nPos) == flag))
            {
                m_nPos++;
            }
            return m_nPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual bool IsAtNonBreakingWhiteSpace(int position)
        {
            return IsNonBreakingWhiteSpace(position, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="previousIsIfThisIs"></param>
        /// <param name="nextIsIfThisIs"></param>
        /// <returns></returns>
        protected virtual bool IsNonBreakingWhiteSpace(int position, bool previousIsIfThisIs, bool nextIsIfThisIs)
        {
            if (((m_eLanguageParsing == ENGLISH) || (m_sText[position] != '\'')) || !IsBetweenWordChars(position, previousIsIfThisIs, nextIsIfThisIs))
            {
                int num;
                if (((m_eLanguageParsing != ENGLISH) && ((m_sText[position] == '\x0092') || (m_sText[position] == '’'))) && IsBetweenWordChars(position, previousIsIfThisIs, nextIsIfThisIs))
                {
                    return false;
                }
                if (((m_eLanguageParsing != GERMAN) && m_bSeparateHyphenWords) && (m_sText[position] == '-'))
                {
                    return false;
                }
                if (((m_eLanguageParsing == GERMAN) && (m_sText[position] == '-')) && char.IsLetterOrDigit(m_sText[position - 1]))
                {
                    return true;
                }
                for (num = 0; num < noBreakChars.Length; num++)
                {
                    if (m_sText[position] == noBreakChars[num])
                    {
                        return IsBetweenWordChars(position, previousIsIfThisIs, nextIsIfThisIs);
                    }
                }
                for (num = 0; num < @"/\-".Length; num++)
                {
                    if (m_sText[position] == @"/\-"[num])
                    {
                        return IsBetweenWordChars(position, previousIsIfThisIs, nextIsIfThisIs);
                    }
                }
                for (num = 0; num < "*=()[]{}<>".Length; num++)
                {
                    if (m_sText[position] == "*=()[]{}<>"[num])
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual bool IsNonWhiteSpace(int position)
        {
            if (position == m_sText.Length)
            {
                return false;
            }
            if (m_V2Parser)
            {
                return (char.IsLetterOrDigit(m_sText[position]) || IsAtNonBreakingWhiteSpace(position));
            }
            return ((char.IsLetterOrDigit(m_sText[position]) || ((((m_eLanguageParsing == ENGLISH) && (m_sText[position] == '\'')) && (((position + 1) != m_sText.Length) && char.IsLetterOrDigit(m_sText[position + 1]))) && ((position > 0) && char.IsLetterOrDigit(m_sText[position - 1])))) || ((!m_bSeparateHyphenWords && (m_sText[position] == '-')) && (((((position + 1) != m_sText.Length) && char.IsLetterOrDigit(m_sText[position + 1])) && (position > 0)) && char.IsLetterOrDigit(m_sText[position - 1]))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual bool IsBetweenWordChars(int position)
        {
            return IsBetweenWordChars(position, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="previousIsIfThisIs"></param>
        /// <param name="nextIsIfThisIs"></param>
        /// <returns></returns>
        protected virtual bool IsBetweenWordChars(int position, bool previousIsIfThisIs, bool nextIsIfThisIs)
        {
            return (((((position + 1) != m_sText.Length) && ((nextIsIfThisIs || char.IsLetterOrDigit(m_sText[position + 1])) || IsNonBreakingWhiteSpace(position + 1, true, false))) && (position > 0)) && ((previousIsIfThisIs || char.IsLetterOrDigit(m_sText[position - 1])) || IsNonBreakingWhiteSpace(position - 1, false, true)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual bool IsBoundary(int offset)
        {
            if ((offset < 0) || (offset > m_sText.Length))
            {
                throw new ArgumentOutOfRangeException(string.Concat(new object[] { "IsBoundary offset out of bounds ", offset, " >= ", m_sText.Length }));
            }
            return (((offset == 0) || (offset == m_sText.Length)) || (Following(offset - 1) == offset));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual bool IsBoundaryLeft(int offset)
        {
            if (((offset < m_sText.Length) && IsBoundary(offset)) && (offset > 0))
            {
                return (!IsNonWhiteSpace(offset - 1) && IsNonWhiteSpace(offset));
            }
            return (((offset == 0) && (m_sText.Length > 0)) && char.IsLetterOrDigit(m_sText[0]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual bool IsBoundaryRight(int offset)
        {
            return (((((offset < m_sText.Length) && IsBoundary(offset)) && (offset > 0)) || ((offset == m_sText.Length) && (offset > 0))) && IsNonWhiteSpace(offset - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int Last()
        {
            m_nPos = m_sText.Length;
            return m_nPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual int Preceding(int offset)
        {
            CheckLegalOffset(offset, "Preceding(offset) [" + offset + "] offset out of bounds", true);
            if (offset == 0)
            {
                return -1;
            }
            m_nPos = offset - 1;
            bool flag = IsNonWhiteSpace(m_nPos);
            while ((m_nPos >= 0) && (IsNonWhiteSpace(m_nPos) == flag))
            {
                m_nPos--;
            }
            m_nPos++;
            return m_nPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public virtual void SetText(string t)
        {
            m_sText = t;
            m_nPos = 0;
        }
    }

    /// <exclude/>
    public class AdvancedTextBoundary :
        SimpleTextBoundary
    {
        // Fields
        private int[] m_arrEndIgnoreRegPos;
        private string[] m_arrFormatTags = new string[] { 
            "font", "strong", "<big>", "</big>", "<small>", "</small>", "<tt>", "</tt>", "<em>", "</em>", "<pre>", "</pre>", "<b>", "</b>", "<s>", "</s>", 
            "<i>", "</i>", "<u>", "</u>", "<span>", "</span>"
         };
        private bool m_bIgnoreXML = false;
        private int m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx = -1;
        private int m_nPreviousIsInsideXMLTag_Pos = -1;
        private int[] m_nStartIgnoreRegPos;
        
        // Properties
        /// <summary>
        /// This boolean property is used to igonore XML
        /// </summary>
        public bool IgnoreXML
        {
            get
            {
                return m_bIgnoreXML;
            }
            set
            {
                m_bIgnoreXML = value;
            }
        }

        // Methods
        private bool EndFormatTag(int offset)
        {
            int num;
            if ((offset > 0) && (base.m_sText[offset - 1] == '>'))
            {
                num = Array.BinarySearch<int>(m_arrEndIgnoreRegPos, offset);
                if (num >= 0)
                {
                    string str = base.m_sText.Substring(m_nStartIgnoreRegPos[num], m_arrEndIgnoreRegPos[num] - m_nStartIgnoreRegPos[num]).ToLower();
                    foreach (string str2 in m_arrFormatTags)
                    {
                        if ((str.IndexOf(str2) > -1) && (m_nStartIgnoreRegPos[num] > 0))
                        {
                            return IsNonWhiteSpace(m_nStartIgnoreRegPos[num] - 1);
                        }
                    }
                }
                return false;
            }
            if (base.m_sText[offset] == '&')
            {
                num = Array.BinarySearch<int>(m_arrEndIgnoreRegPos, offset);
                if (num < 0)
                {
                    return false;
                }
                if (base.m_sText.Substring(m_nStartIgnoreRegPos[num], m_arrEndIgnoreRegPos[num] - m_nStartIgnoreRegPos[num]).ToLower() == "&nbsp;")
                {
                    return false;
                }
                return ((m_nStartIgnoreRegPos[num] == 0) || IsNonWhiteSpace(m_nStartIgnoreRegPos[num] - 1));
            }
            return false;
        }

        private bool StartFormatTag(int offset)
        {
            int num;
            if (base.m_sText[offset] == '<')
            {
                num = Array.BinarySearch<int>(m_nStartIgnoreRegPos, offset);
                if (num >= 0)
                {
                    string str = base.m_sText.Substring(offset, m_arrEndIgnoreRegPos[num] - offset).ToLower();
                    foreach (string str2 in m_arrFormatTags)
                    {
                        if (str.IndexOf(str2) > -1)
                        {
                            return IsNonWhiteSpace(m_arrEndIgnoreRegPos[num]);
                        }
                    }
                }
                return false;
            }
            if (base.m_sText[offset] == '&')
            {
                num = Array.BinarySearch<int>(m_nStartIgnoreRegPos, offset);
                if (num < 0)
                {
                    return false;
                }
                if (base.m_sText.Substring(offset, m_arrEndIgnoreRegPos[num] - offset).ToLower() == "&nbsp;")
                {
                    return false;
                }
                return IsNonWhiteSpace(m_arrEndIgnoreRegPos[num]);
            }
            return false;
        }

        private int FindPosOfCloser(int offset, char closer)
        {
            return FindPosOfCloser(offset, base.m_sText.IndexOf(closer, offset));
        }

        private int FindPosOfCloser(int offset, int expectedCloserOffset)
        {
            int num = expectedCloserOffset;
            if (num > -1)
            {
                int num2 = 0;
                for (int i = offset; i <= num; i++)
                {
                    if (base.m_sText[i] == '<')
                    {
                        num2++;
                    }
                    if (base.m_sText[i] == '>')
                    {
                        num2--;
                    }
                }
                if (num2 > 0)
                {
                    num = FindPosOfCloser(offset, base.m_sText.IndexOf('>', num + 1));
                }
            }
            return num;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override int Following(int offset)
        {
            if (!IgnoreXML)
            {
                return base.Following(offset);
            }
            int position = offset;
            CheckLegalOffset(offset, "Following(" + offset + ") offset out of bounds", false);
            bool flag = IsNonWhiteSpace(offset);
            while ((position < base.m_sText.Length) && ((IsNonWhiteSpace(position) == flag) || StartFormatTag(position)))
            {
                offset = position;
                int startIndex = base.Following(offset);
                if ((base.m_sText[startIndex - 1] == '<') || ((base.m_sText[startIndex - 1] == '&') && (base.m_sText.IndexOf(';', startIndex) < (startIndex + 9))))
                {
                    offset = startIndex;
                }
                int index = IsInsideXMLTag(offset);
                if (index > -1)
                {
                    position = m_arrEndIgnoreRegPos[index];
                }
                else if (offset < base.m_sText.Length)
                {
                    position = base.Following(offset);
                }
                else
                {
                    position = base.m_sText.Length;
                }
            }
            return position;
        }

        private void IdRegions(string t)
        {
            char closer = '>';
            List<object> list = new List<object>();
            List<object> list2 = new List<object>();
            int num = -1;
            int startIndex = -1;
            int index = 0;
            int num4 = -1;
            while (((num = t.IndexOf('<', index)) > -1) || (((startIndex = t.IndexOf('&', index)) > -1) && (((num4 = t.IndexOf(';', startIndex)) < (startIndex + 9)) && (num4 > -1))))
            {
                if ((((num != -1) && ((startIndex = t.IndexOf('&', index)) > -1)) && ((num4 = t.IndexOf(';', startIndex)) < (startIndex + 9))) && (num4 > -1))
                {
                }
                if ((((num >= 0) && (num < startIndex)) || ((startIndex == -1) || (num4 == -1))) || (num4 > (startIndex + 9)))
                {
                    closer = '>';
                }
                else if (num4 > -1)
                {
                    closer = ';';
                    num = startIndex;
                }
                list.Add(num);
                index = FindPosOfCloser(num, closer);
                if (index > -1)
                {
                    if (t.Substring(num + 1, index - num).Trim().ToLower().StartsWith("style"))
                    {
                        index = t.ToLower().IndexOf("</style", num);
                        if (index > -1)
                        {
                            index += 8;
                        }
                        else
                        {
                            index = t.Length;
                        }
                    }
                    else if (t.Substring(num + 1, index - num).Trim().ToLower().StartsWith("script"))
                    {
                        index = t.ToLower().IndexOf("</script", num);
                        if (index > -1)
                        {
                            index += 9;
                        }
                        else
                        {
                            index = t.Length;
                        }
                    }
                    else if (t.Substring(num + 1, index - num).Trim().ToLower().StartsWith("!--"))
                    {
                        index = t.ToLower().IndexOf("-->", num);
                        if (index > -1)
                        {
                            index += 3;
                        }
                        else
                        {
                            index = t.Length;
                        }
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    index = t.Length;
                }
                list2.Add(index);
            }
            m_nStartIgnoreRegPos = (int[])ToArray(list, num.GetType());
            m_arrEndIgnoreRegPos = (int[])ToArray(list2, num.GetType());
        }
        /// <summary>
        /// Returns array from the list of object
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Array ToArray(List<object> list, Type type)
        {
            Array array = Array.CreateInstance(type, list.Count);
            int i= 0;
            foreach (object obj in list)
            {
                if (obj.GetType() == type)
                {
                    array.SetValue(obj, i);
                    i++;
                }
            }
            return array;
        }
        /// <summary>
        /// Ruturns boolean value by checking IsNonBreakingWhiteSpace
        /// </summary>
        /// <param name="position"></param>
        /// <param name="previousIsIfThisIs"></param>
        /// <param name="nextIsIfThisIs"></param>
        /// <returns></returns>
        protected override bool IsNonBreakingWhiteSpace(int position, bool previousIsIfThisIs, bool nextIsIfThisIs)
        {
            if ((base.m_sText[position] == '&') && IgnoreXML)
            {
                return false;
            }
            return base.IsNonBreakingWhiteSpace(position, previousIsIfThisIs, nextIsIfThisIs);
        }
        /// <summary>
        /// Returns boolean value by checking IsNonWhiteSpace
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool IsNonWhiteSpace(int position)
        {
            if (position == base.m_sText.Length)
            {
                return false;
            }
            return ((!m_bIgnoreXML || (((base.m_sText[position] != '&') || (base.m_sText.IndexOf(';', position, ((position + 9) > base.m_sText.Length) ? (base.m_sText.Length - position) : 9) == -1)) && (IsInsideXMLTag(position) == -1))) && base.IsNonWhiteSpace(position));
        }

        private int IsInsideXMLTag(int pos)
        {
            int num = pos;
            int index = -1;
            if ((m_nPreviousIsInsideXMLTag_Pos > -1) && ((m_nPreviousIsInsideXMLTag_Pos == (pos - 1)) || (m_nPreviousIsInsideXMLTag_Pos == pos)))
            {
                if ((m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx < m_nStartIgnoreRegPos.Length) && (m_nStartIgnoreRegPos[m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx] >= pos))
                {
                    index = m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx;
                }
                else if (((m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx + 1) < m_nStartIgnoreRegPos.Length) && (m_nStartIgnoreRegPos[m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx + 1] >= pos))
                {
                    index = m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx + 1;
                }
            }
            if (index == -1)
            {
                index = Array.BinarySearch<int>(m_nStartIgnoreRegPos, num);
                if (index < 0)
                {
                    index = ~index;
                }
            }
            m_nPreviousIsInsideXMLTag_Pos = pos;
            m_nPreviousIsInsideXMLTag_NextIgnoreRegionIdx = index;
            int num3 = index - 1;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if ((index < (m_nStartIgnoreRegPos.Length - 1)) && (m_nStartIgnoreRegPos[index] == num))
            {
                num3 = index;
            }
            if (num3 >= m_nStartIgnoreRegPos.Length)
            {
                num3 = m_nStartIgnoreRegPos.Length - 1;
            }
            if (((m_nStartIgnoreRegPos.Length > 0) && (m_nStartIgnoreRegPos[num3] <= num)) && (m_arrEndIgnoreRegPos[num3] > num))
            {
                return num3;
            }
            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override int Preceding(int offset)
        {
            if (!IgnoreXML)
            {
                return base.Preceding(offset);
            }
            int num = offset;
            CheckLegalOffset(offset, "Preceeding(" + offset + ") offset out of bounds", true);
            bool flag = IsNonWhiteSpace(offset - 1);
            while ((num > 0) && ((IsNonWhiteSpace(num - 1) == flag) || EndFormatTag(num)))
            {
                offset = num;
                int num2 = base.Preceding(offset);
                if ((num2 > 0) && (base.m_sText[num2] == '>'))
                {
                    offset = num2;
                }
                int index = Array.BinarySearch<int>(m_arrEndIgnoreRegPos, offset);
                if (index < 0)
                {
                    index = ~index;
                }
                int num4 = index;
                if ((index < (m_arrEndIgnoreRegPos.Length - 1)) && (m_arrEndIgnoreRegPos[index] == offset))
                {
                    num4 = index;
                }
                if (num4 >= m_nStartIgnoreRegPos.Length)
                {
                    num4 = m_nStartIgnoreRegPos.Length - 1;
                }
                if (((m_nStartIgnoreRegPos.Length > 0) && (m_nStartIgnoreRegPos[num4] < offset)) && (m_arrEndIgnoreRegPos[num4] >= offset))
                {
                    num = m_nStartIgnoreRegPos[num4];
                }
                else if (offset < base.m_sText.Length)
                {
                    num = base.Preceding(offset);
                }
                else
                {
                    num = 0;
                }
            }
            return num;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public override void SetText(string t)
        {
            base.SetText(t);
            if (IgnoreXML)
            {
                IdRegions(t);
            }
        }
    }
}
