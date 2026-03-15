#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a selection of part of text body.
    /// </summary>
    public class TextBodySelection
    {
        #region Fields
        private WTextBody m_textBody;
        private int m_itemStartIndex = -1;
        private int m_itemEndIndex = -1;
        private int m_pItemStartIndex = -1;
        private int m_pItemEndIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the text body.
        /// </summary>
        /// <value>The text body.</value>
        public WTextBody TextBody
        {
            get
            {
                return m_textBody;
            }
        }
        /// <summary>
        /// Gets or sets the start index of the text body item.
        /// </summary>
        /// <value>The start index of the item.</value>
        public int ItemStartIndex
        {
            get
            {
                return m_itemStartIndex;
            }
            set
            {
                m_itemStartIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the end index of the text body item.
        /// </summary>
        /// <value>The end index of the item.</value>
        public int ItemEndIndex
        {
            get
            {
                return m_itemEndIndex;
            }
            set
            {
                m_itemEndIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the start index of the paragraph item.
        /// </summary>
        /// <value>The start index of the item.</value>
        public int ParagraphItemStartIndex
        {
            get
            {
                return m_pItemStartIndex;
            }
            set
            {
                m_pItemStartIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the end index of the paragraph item.
        /// </summary>
        /// <value>The end index of the item.</value>
        public int ParagraphItemEndIndex
        {
            get
            {
                return m_pItemEndIndex;
            }
            set
            {
                m_pItemEndIndex = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodySelection"/> class.
        /// </summary>
        /// <param name="itemStart">The item start.</param>
        /// <param name="itemEnd">The item end.</param>
        public TextBodySelection(ParagraphItem itemStart, ParagraphItem itemEnd)
        {
            WParagraph paraStart = itemStart.OwnerParagraph;
            WParagraph paraEnd = itemEnd.OwnerParagraph;

            if (paraStart.Owner != paraEnd.Owner)
            {
                throw new ArgumentException("itemStart and itemEnd must be contained in one text body");
            }

            m_textBody = paraStart.OwnerTextBody;
            m_itemStartIndex = paraStart.GetIndexInOwnerCollection();
            m_itemEndIndex = paraEnd.GetIndexInOwnerCollection();
            m_pItemStartIndex = itemStart.GetIndexInOwnerCollection();
            m_pItemEndIndex = itemEnd.GetIndexInOwnerCollection();

            ValidateIndexes();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBodySelection"/> class.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="itemStartIndex">Start index of the item.</param>
        /// <param name="itemEndIndex">End index of the item.</param>
        /// <param name="pItemStartIndex">Start index of the paragraph item.</param>
        /// <param name="pItemEndIndex">End index of the paragraph item.</param>
        public TextBodySelection(ITextBody textBody, int itemStartIndex, int itemEndIndex,
          int pItemStartIndex, int pItemEndIndex)
        {

            if (textBody == null)
                throw new ArgumentNullException("textBody");

            m_textBody = (WTextBody)textBody;
            m_itemStartIndex = itemStartIndex;
            m_itemEndIndex = itemEndIndex;
            m_pItemStartIndex = pItemStartIndex;
            m_pItemEndIndex = pItemEndIndex;
            ValidateIndexes();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Shifts the start to end.
        /// </summary>
        internal int ShiftStartToEnd(int endIndexShift, int pEndIndexShift)
        {
            int shiftCount = m_itemEndIndex - m_itemStartIndex;
            m_itemStartIndex += shiftCount;
            m_itemEndIndex += endIndexShift;

            // Shift paragraph item positions
            shiftCount = m_pItemEndIndex - m_pItemStartIndex;
            m_pItemStartIndex += shiftCount;
            if (m_itemStartIndex == m_itemEndIndex)
            {
                //Updates the paragraph item start and end index (Mail group defined as single paragraph).
                m_pItemStartIndex++;
                m_pItemEndIndex += pEndIndexShift + 1;
            }
            else
            {
                m_pItemEndIndex = pEndIndexShift;
            }
            ValidateIndexes();
            return shiftCount;
        }
        /// <summary>
        /// Validates the indexes.
        /// </summary>
        private void ValidateIndexes()
        {
            if (m_itemStartIndex < 0 || m_itemStartIndex >= m_textBody.Items.Count)
                throw new ArgumentOutOfRangeException("m_itemStartIndex",
                  "m_itemStartIndex is less than 0 or greater than " + m_textBody.Items.Count);

            if (m_itemEndIndex < m_itemStartIndex || m_itemEndIndex >= m_textBody.Items.Count)
                throw new ArgumentOutOfRangeException("m_itemEndIndex",
                  "m_itemEndIndex is less than " + m_itemStartIndex + " or greater than " + m_textBody.Items.Count);

            WParagraph paraStart = m_textBody.Items[m_itemStartIndex] as WParagraph;
            WParagraph paraEnd = m_textBody.Items[m_itemEndIndex] as WParagraph;

            if (paraStart != null)
            {
                if (m_pItemStartIndex < 0 || m_pItemStartIndex > paraStart.Items.Count)
                    throw new ArgumentOutOfRangeException("m_pItemStartIndex", "m_pItemStartIndex is less than 0 or greater than " + paraStart.Items.Count);
            }

            if (paraEnd != null)
            {
                if (m_pItemEndIndex < 0 || m_pItemEndIndex > paraEnd.Items.Count)
                    throw new ArgumentOutOfRangeException("m_pItemEndIndex", "m_pItemEndIndex is less than 0 or greater than " + paraEnd.Items.Count);
            }
        }
        #endregion
    }
    /// <summary>
    /// Represent a selection of text inside paragraph.
    /// </summary>
    public class TextSelection : IEnumerable
    {
        #region Fields
        private WParagraph m_para;
        private WTextRange m_startTr;
        private WTextRange m_endTr;
        private List<WTextRange> m_items = new List<WTextRange>();
        private int m_startCut;
        private int m_endCut;
        private int m_startIndex;
        private int m_endIndex;
        private WTextRange[] m_cachedRanges = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the selected text.
        /// </summary>
        /// <value>The selected text.</value>
        public string SelectedText
        {
            get
            {
                if (m_startTr == null || m_endTr == null)
                {
                    return string.Empty;
                }

                /*int startPos = (Count > 1)? 
                    m_startTr.StartPos :
                    m_startTr.StartPos + m_startCut;*/
                // Fixed SD3535 ( selected incorrect text )
                int startPos = m_startTr.StartPos;
                if (m_startCut != 0)
                {
                    startPos += m_startCut;
                }

                    int length = (m_endCut >= 0) ?
                      (m_endTr.StartPos + m_endCut) - startPos :
                      (m_endTr.StartPos + m_endTr.TextLength) - startPos;

                if (length < 0)
                {
                    throw new Exception("Text selection was modified. This could be done while modification of source document.");
                }

                return OwnerParagraph.Text.Substring(startPos, length);
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <value></value>
        public string this[int index]
        {
            get
            {
                WTextRange tr = m_items[index];
                string text = tr.Text;

                if (index == 0 && m_startCut > 0)
                {
                    text = text.Substring(m_startCut);
                }
                if (index == m_items.Count - 1 && m_endCut != -1)
                {
                    text = text.Substring(0, m_endCut - m_startCut);
                }

                return text;
            }
            set
            {
                WTextRange tr = m_items[index];
                string text = value;

                if (index == 0 && m_startCut > 0)
                {
                    text = tr.Text.Substring(0, m_startCut) + text;
                }
                if (index == m_items.Count && m_endCut != -1)
                {
                    text = text + tr.Text.Substring(m_endCut);
                }

                tr.Text = text;
            }
        }
        /// <summary>
        /// Gets the count of text chunks.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }
        /// <summary>
        /// Gets the paragraph owner.
        /// </summary>
        /// <value>The paragraph owner.</value>
        internal WParagraph OwnerParagraph
        {
            get
            {
                if (m_startTr != null)
                    m_para = m_startTr.OwnerParagraph;
                return m_para;
            }
        }
        /// <summary>
        /// Gets the selection chain.
        /// </summary>
        /// <value>The selection chain.</value>
        internal TextSelectionList SelectionChain = null;
        /// <summary>
        /// Gets the start text range.
        /// </summary>
        /// <value>The start text range.</value>
        internal WTextRange StartTextRange
        {
            get
            {
                return m_startTr;
            }
        }
        /// <summary>
        /// Gets the end text range.
        /// </summary>
        /// <value>The end text range.</value>
        internal WTextRange EndTextRange
        {
            get
            {
                return m_endTr;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSelection"/> class.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="startCharPos">The start char position.</param>
        /// <param name="endCharPos">The end char position.</param>
        public TextSelection(WParagraph para, int startCharPos, int endCharPos)
        {
            m_para = para;
            if (m_para.Items.Count == 0)
                return;

            WTextRange tr;
            m_startIndex = FindUtils.GetStartRangeIndex(m_para, startCharPos + 1, out tr);

            if (tr == null)
                return;

            m_startCut = startCharPos - tr.StartPos;
            m_startTr = tr;

            m_endIndex = FindUtils.GetStartRangeIndex(m_para, endCharPos, out tr);

            if (m_endIndex < m_startIndex || tr == null)
            {
                for (int i = para.Items.Count; i > 0; i--)
                {
                    ParagraphItem item = para[i - 1];
                    if (item is WTextRange)
                    {
                        tr = item as WTextRange;
                        break;
                    }
                }
                m_endCut = endCharPos - tr.StartPos - 1;
            }
            else
                m_endCut = endCharPos - tr.StartPos;

            m_endTr = tr;

            if (m_endCut == tr.TextLength)
                m_endCut = -1;

            for (int i = m_startIndex; i <= m_endIndex; i++)
            {
                tr = m_para.Items[i] as WTextRange;

                if (tr != null)
                {
                    m_items.Add(tr);
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the ranges.
        /// </summary>
        /// <returns></returns>
        public WTextRange[] GetRanges()
        {
            if (OwnerParagraph.Items.Count == 0)
                return null;

            EnsureIndexes();

            if (m_startCut > 0 || m_endCut != -1)
            {
                SplitRanges();
            }

            return m_items.ToArray();
        }
        /// <summary>
        /// Gets as one range.
        /// </summary>
        /// <returns></returns>
        public WTextRange GetAsOneRange()
        {
            if (OwnerParagraph.Items.Count == 0)
                return null;

            EnsureIndexes();

            if (m_startCut > 0 || m_endCut != -1)
            {
                SplitRanges();
            }

            if (Count > 1)
            {
                string selText = SelectedText;

                while (m_items.Count > 1)
                {
                    m_items[1].RemoveSelf();
                    m_items.RemoveAt(1);
                }

                m_startTr.Text = selText;
                m_endTr = m_startTr;
            }


            return m_items[0];
        }
        /// <summary>
        /// Splits and erase the content of selection.
        /// </summary>
        internal int SplitAndErase()
        {
            if (OwnerParagraph.Items.Count == 0)
                return 0;

            EnsureIndexes();

            if (m_startCut > 0 || m_endCut != -1)
            {
                SplitRanges();
            }

            if (Count > 0)
            {
                while (m_items.Count > 0)
                {
                    m_items[0].RemoveSelf();
                    m_items.RemoveAt(0);
                }

                m_startTr = null;
                m_endTr = m_startTr;
            }

            return m_startIndex;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Caches the ranges.
        /// </summary>
        internal void CacheRanges()
        {
            if (m_cachedRanges == null)
            {
                WTextRange[] ranges = GetRanges();
                if (ranges == null)
                    return;

                m_cachedRanges = new WTextRange[ranges.Length];

                for (int i = 0, len = ranges.Length; i < len; i++)
                {
                    m_cachedRanges[i] = (WTextRange)ranges[i].Clone();
                }
            }
        }
        /// <summary>
        /// Copies ranges to specified paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="startIndex">The start index.</param>
        internal void CopyTo(WParagraph para, int startIndex, bool saveFormatting, WCharacterFormat srcFormat)
        {
            CacheRanges();

            foreach (WTextRange tr in m_cachedRanges)
            {
                WTextRange cloneRange = (WTextRange)tr.Clone();
                if (saveFormatting && srcFormat != null)
                    cloneRange.CharacterFormat.ImportContainer(srcFormat);
                para.Items.Insert(startIndex, cloneRange);
                startIndex++;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            string[] chunks = new string[Count];

            for (int i = 0, len = Count; i < len; i++)
            {
                chunks[i] = this[i];
            }

            return chunks.GetEnumerator();
        }
        /// <summary>
        /// Ensures the indexes.
        /// </summary>
        private void EnsureIndexes()
        {
            if (m_startTr.Owner != OwnerParagraph || m_endTr.Owner != OwnerParagraph)
                throw new InvalidOperationException();

            int itemsCount = OwnerParagraph.Items.Count;

            if (m_startIndex >= itemsCount || m_startTr != OwnerParagraph.Items[m_startIndex])
                m_startIndex = m_startTr.GetIndexInOwnerCollection();

            if (m_endIndex >= itemsCount || m_endTr != OwnerParagraph.Items[m_endIndex])
                m_endIndex = m_endTr.GetIndexInOwnerCollection();
        }
        /// <summary>
        /// Splits the ranges.
        /// </summary>
        internal void SplitRanges()
        {
            if (m_startCut > 0)
            {
                WTextRange tr = new WTextRange(OwnerParagraph.Document);
                tr.Text = m_startTr.Text.Substring(0, m_startCut);
                tr.CharacterFormat.ImportContainer(m_startTr.CharacterFormat);
                m_startTr.Text = m_startTr.Text.Substring(m_startCut);
                OwnerParagraph.Items.Insert(m_startIndex, tr);
                m_startIndex++;
                m_endIndex++;

                if (SelectionChain != null)
                {
                    UpdateFollowingSelections(true);
                }

                if (Count == 1)
                {
                    if (m_endCut >= 0)
                    {
                        m_endCut -= m_startCut;
                    }
                }
                m_startCut = 0;              
            }

            if (m_endCut > 0)
            {
                WTextRange tr = new WTextRange(OwnerParagraph.Document);
                tr.Text = m_endTr.Text.Substring(m_endCut);
                tr.CharacterFormat.ImportContainer(m_endTr.CharacterFormat);
                m_endTr.Text = m_endTr.Text.Substring(0, m_endCut);
                OwnerParagraph.Items.Insert(m_endIndex + 1, tr);

                if (SelectionChain != null)
                {
                    UpdateFollowingSelections(false);
                }

                m_endCut = -1;
            }
        }
        /// <summary>
        /// Updates the following selections.
        /// </summary>
        private void UpdateFollowingSelections(bool forStart)
        {
            foreach (TextSelection sel in SelectionChain)
            {
                if (sel != this)
                {
                    WTextRange targetTr = forStart ? m_startTr : m_endTr;
                    int offset = forStart ? m_startCut : m_endCut;

                    // Correct for start text range.
                    if (sel.m_startTr == targetTr)
                    {
                        if (!forStart)
                        {
                            sel.m_startTr = (WTextRange)m_endTr.NextSibling;
                            sel.m_items[0] = sel.m_startTr;
                            sel.m_startTr.SafeText = true;
                        }

                        sel.m_startCut -= offset;
                    }

                    // Correct for end text range.
                    if (sel.m_endTr == targetTr)
                    {
                        if (!forStart)
                        {
                            sel.m_endTr = (WTextRange)m_endTr.NextSibling;
                            sel.m_items[sel.m_items.Count - 1] = sel.m_endTr;
                            sel.m_endTr.SafeText = true;
                        }

                        if (sel.m_endCut >= 0)
                        {
                            sel.m_endCut -= offset;
                        }
                    }
                }
            }
        }
        #endregion
    }
    internal class TextSelectionList : List<TextSelection>
    {
    }
}
