#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Syncfusion.DocIO.DLS
{
    internal class FindUtils
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        internal const string DEF_WHOLE_WORD_BEFORE = @"(?<=^|\W|\t)";
        internal const string DEF_WHOLE_WORD_AFTER = @"(?=$|\W|\t)";
        internal const string DEF_WHOLE_WORD_EMPTY = DEF_WHOLE_WORD_BEFORE + DEF_WHOLE_WORD_AFTER;
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether [is pattern empty] [the specified pattern].
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>
        /// 	<c>true</c> if it is the specified pattern empty, set to <c>true</c>.
        /// </returns>
        internal static bool IsPatternEmpty(Regex pattern)
        {
            string patternRegex = pattern.ToString();

            return (patternRegex.Length == 0 || patternRegex == DEF_WHOLE_WORD_EMPTY);
        }
        /// <summary>
        /// Strings to regex.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it is specified to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        internal static Regex StringToRegex(string given, bool caseSensitive, bool wholeWord)
        {
            given = Regex.Escape(given);

            if (wholeWord)
            {
                given = DEF_WHOLE_WORD_BEFORE + given + DEF_WHOLE_WORD_AFTER;
            }

            return new Regex(given, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// Gets the start index of the range.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="start">The start.</param>
        /// <param name="tr">The Text Range.</param>
        /// <returns></returns>
        internal static int GetStartRangeIndex(WParagraph para, int start, out WTextRange tr)
        {
            tr = null;
            int startIndex = 0;

            for (int i = 0, length = para.Items.Count; i < length; i++)
            {
                tr = para[i] as WTextRange;

                if (tr != null && tr.StartPos + tr.TextLength >= start)
                {
                    startIndex = i;
                    break;
                }
            }

            return startIndex;
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    internal class TextFinder
    {
        #region Fields
        private List<WParagraph> m_linePCol;
        #endregion

        #region Properties
        /// <summary>
        /// TextFinder variable.
        /// </summary>
        [ThreadStatic]
        public static TextFinder m_instance;
        /// <summary>
        /// Gets the instance of TextReplacer.
        /// </summary>
        /// <value>The instance.</value>
        public static TextFinder Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new TextFinder();
                }

                return m_instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<WParagraph> SingleLinePCol
        {
            get
            {
                if (m_linePCol == null)
                    m_linePCol = new List<WParagraph>();

                return m_linePCol;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Finds the text by specified pattern.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="onlyFirstMacth">if it is only first macth, set to <c>true</c>.</param>
        /// <returns></returns>
        public TextSelectionList Find(WParagraph para, Regex pattern, bool onlyFirstMatch)
        {
            string text = para.Text;
            MatchCollection matches = pattern.Matches(text);
            TextSelectionList selections = new TextSelectionList();

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    int mStart = match.Index;
                    int mEnd = match.Index + match.Length;

                    TextSelection textSel = new TextSelection(para, mStart, mEnd);
                    textSel.SelectionChain = selections;
                    selections.Add(textSel);

                    if (onlyFirstMatch)
                    {
                        break;
                    }
                }
            }

            FindInItems(para, pattern, onlyFirstMatch, selections);

            return selections;
        }
        /// <summary>
        /// Finds the in items.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="onlyFirstMacth">if it is only first match, set to <c>true</c>.</param>
        /// <param name="selections">The selections.</param>
        private static void FindInItems(WParagraph para, Regex pattern, bool onlyFirstMatch, TextSelectionList selections)
        {
            if (!(selections.Count > 0 && onlyFirstMatch))
            {
                foreach (ParagraphItem item in para.Items)
                {
                    WTextBody tb = GetTextBody(item);

                    if (tb != null)
                    {
                        if (onlyFirstMatch)
                        {
                            TextSelection tSel = tb.Find(pattern);

                            if (tSel != null)
                            {
                                selections.Add(tSel);
                                break;
                            }
                        }
                        else
                        {
                            TextSelectionList tSels = tb.FindAll(pattern);

                            if (tSels != null && tSels.Count > 0)
                            {
                                selections.AddRange(tSels);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets the text body of paragraph item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static WTextBody GetTextBody(ParagraphItem item)
        {
            WTextBody tb = null;

            switch (item.EntityType)
            {
                case EntityType.Comment:
                    WComment comm = (WComment)item;
                    tb = comm.TextBody;
                    break;
                case EntityType.Footnote:
                    WFootnote foot = (WFootnote)item;
                    tb = foot.TextBody;
                    break;
                case EntityType.TextBox:
                    WTextBox tbox = (WTextBox)item;
                    tb = tbox.TextBoxBody;
                    break;
                case EntityType.AutoShape:
                    Shape shape = (Shape)item;
                    tb = shape.TextBody;
                    break;
                default:
                    break;
            }

            return tb;
        }
        #endregion

        #region Implementation / single-line find
        /// <summary>
        /// Finds the text by specified pattern in single-line mode.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public TextSelection[] FindSingleLine(WTextBody textBody, Regex pattern)
        {
            if (textBody.Items.Count == 0)
                return null;

            return FindSingleLine(textBody, pattern, 0, textBody.Items.Count - 1);
        }
        /// <summary>
        /// Finds the text by specified pattern in single-line mode.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        public TextSelection[] FindSingleLine(WTextBody textBody, Regex pattern, int startIndex, int endIndex)
        {
            TextSelection[] selection = null;
            WParagraph para = null;
            WTable table = null;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (textBody.Items[i] is WParagraph)
                {
                    para = textBody.Items[i] as WParagraph;
                    selection = FindInItems(para, pattern, 0, para.Items.Count - 1);
                    if (selection != null)
                        return selection;
                    else
                        selection = FindSingleLine(pattern);
                }
                else if (textBody.Items[i] is WTable)
                {
                    table = textBody.Items[i] as WTable;
                    selection = FindSingleLine(table, pattern);
                }

                if (selection != null)
                    return selection;
            }

            return FindSingleLine(pattern);
        }
        /// <summary>
        /// Finds the in items.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal TextSelection[] FindInItems(WParagraph para, Regex pattern, int startIndex, int endIndex)
        {
            if (!SingleLinePCol.Contains(para))
                SingleLinePCol.Add(para);

            TextSelection[] selection = null;
            ParagraphItem item = null;
            WTextBody tb = null;

            for (int i = startIndex; i <= endIndex; i++)
            {
                item = para[i];

                tb = GetTextBody(item);
                if (tb != null)
                    selection = FindSingleLine(tb, pattern);

                if (selection != null)
                    return selection;
            }

            return selection;
        }
        /// <summary>
        /// Finds the single-line text by pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal TextSelection[] FindSingleLine(Regex pattern)
        {
            if (m_linePCol == null || m_linePCol.Count == 0)
                return null;

            string text = string.Empty;
            WParagraph para = null;
            Match match = null;
            StringBuilder textBuilder = new StringBuilder();

            // First search for selection in paragraph's text
            for (int i = 0, cnt = m_linePCol.Count; i < cnt; i++)
            {
                para = m_linePCol[i];
                textBuilder.Append(para.Text);

                if (i == cnt - 1)
                {
                    // Check for match for all paragraph's collection text
                    text = textBuilder.ToString();
                    match = pattern.Match(text);
                }
            }

            if (match != null && match.Success)
            {
                TextSelectionList selection;
                // If paragraphs contain specified expression, "find" TextSelection
                // for each paragraph, which contain a part of specified text.      
                int startIndex = match.Index;
                int endIndex = startIndex + match.Length;

                // If expression is enclosed in all paragraphs of text body...
                if (startIndex == 0 && endIndex == text.Length)
                {
                    selection = new TextSelectionList();
                    foreach (WParagraph par in m_linePCol)
                    {
                        if (match.Length == par.Text.Length)
                        {
                            TextSelection sel = new TextSelection(par, 0, par.Text.Length);
                            selection.Add(sel);
                        }
                    }
                }
                else
                {
                    selection = FindSingleLine(m_linePCol, match);
                }

                if (selection != null && selection.Count > 0)
                {
                    m_linePCol.Clear();
                    return selection.ToArray();
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the text by specified pattern in table using single-line mode.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal TextSelection[] FindSingleLine(WTable table, Regex pattern)
        {
            TextSelection[] selection = null;
            foreach (WTableRow row in table.Rows)
                foreach (WTableCell cell in row.Cells)
                {
                    selection = FindSingleLine(cell as WTextBody, pattern);
                    if (selection != null)
                        return selection;
                }

            return selection;
        }
        /// <summary>
        /// Forms the list of text selections.
        /// </summary>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        private TextSelectionList FindSingleLine(List<WParagraph> paragraphs, Match match)
        {
            int startIndex = match.Index;
            int endIndex = startIndex + match.Length;
            string text = string.Empty;
            int textLastIndex = 0;
            int textFirstIndex = 0;
            TextSelectionList selection = null;

            foreach (WParagraph para in paragraphs)
            {
                int startCharPos = -1;
                int endCharPos = -1;

                textFirstIndex = text.Length;
                text += para.Text;
                textLastIndex = text.Length;

                if (textFirstIndex <= startIndex && startIndex <= textLastIndex)
                {
                    selection = new TextSelectionList();
                    startCharPos = startIndex - textFirstIndex;
                }

                if (textFirstIndex <= endIndex && endIndex <= textLastIndex + 1)
                    endCharPos = endIndex - textFirstIndex;

                if (startCharPos != -1 || endCharPos != -1)
                {
                    if (startCharPos != -1 && endCharPos != -1)
                    {
                        selection.Add(new TextSelection(para, startCharPos, endCharPos));
                        break;
                    }
                    else if (startCharPos != -1 && startCharPos < para.Text.Length)
                    {
                        selection.Add(new TextSelection(para, startCharPos, para.Text.Length));
                    }
                    else if (endCharPos != -1 && endCharPos <= para.Text.Length)
                    {
                        selection.Add(new TextSelection(para, 0, endCharPos));
                        break;
                    }
                }
                else if (textFirstIndex > startIndex && textLastIndex < endIndex && para.Text != string.Empty)
                {
                    selection.Add(new TextSelection(para, 0, para.Text.Length));
                }
            }

            return selection;
        }
        #endregion
    }
    /// <summary>
    /// Class provides replacing method for the specified paragraph
    /// </summary>
    internal class TextReplacer
    {
        #region Properties
        /// <summary>
        /// TextReplacer variable.
        /// </summary>
        [ThreadStatic]
        public static TextReplacer m_instance;
        /// <summary>
        /// Gets the instance of TextReplacer.
        /// </summary>
        /// <value>The instance.</value>
        public static TextReplacer Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new TextReplacer();
                }

                return m_instance;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Replaces the specified paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replace.</param>
        /// <returns></returns>
        public int Replace(WParagraph para, Regex pattern, string replacement)
        {
            string text = para.Text;
            MatchCollection matches = pattern.Matches(text);

            if (matches.Count > 0)
            {
                int offset = 0;
                int currOffset = 0;
                int repLength = replacement.Length;
                int mStart = 0;
                int mLength = 0;

                foreach (Match match in matches)
                {
                    mStart = match.Index + offset;
                    mLength = match.Length;
                    currOffset = repLength - match.Length;

                    para.ReplaceWithoutCorrection(mStart, mLength, replacement);
                    // 1. find start range
                    // 2. remove internal items
                    // 3. correct last range start/length
                    // 4. correct next items start
                    WTextRange tr;
                    int startIndex = FindUtils.GetStartRangeIndex(para, mStart + 1, out tr);
                    int trEndIndex = tr.StartPos + tr.TextLength;
                    tr.SafeText = false;

                    if (trEndIndex >= mStart + mLength)
                    {
                        tr.TextLength += currOffset;
                    }
                    else
                    {
                        WTextRange nextTr;
                        RemoveInternalItems(para, mStart + mLength, startIndex + 1, out nextTr);
                        int mEnd = mStart + mLength;

                        if (nextTr != null)
                        {
                            nextTr.TextLength -= mEnd - nextTr.StartPos;
                            nextTr.StartPos = mEnd + currOffset;
                        }

                        tr.TextLength = mEnd + currOffset - tr.StartPos;
                        startIndex += 1;
                    }

                    CorrectNextItems(para, startIndex + 1, currOffset);
                    offset += currOffset;

                    if (para.Document.ReplaceFirst)
                        break;
                }
            }

            int retCount = matches.Count;

            if (!(para.Document.ReplaceFirst && retCount > 0))
            {
                retCount += ReplaceInItems(para, pattern, replacement);
            }

            return retCount;
        }
        /// <summary>
        /// Replaces the in items.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        private static int ReplaceInItems(WParagraph para, Regex pattern, string replacement)
        {
            int matches = 0;

            foreach (ParagraphItem item in para.Items)
            {
                WTextBody tb = null;

                switch (item.EntityType)
                {
                    case EntityType.Comment:
                        WComment comm = (WComment)item;
                        tb = comm.TextBody;
                        break;
                    case EntityType.Footnote:
                        WFootnote foot = (WFootnote)item;
                        tb = foot.TextBody;
                        break;
                    case EntityType.TextBox:
                        WTextBox tbox = (WTextBox)item;
                        tb = tbox.TextBoxBody;
                        break;
                    case EntityType.AutoShape:
                        Shape shape = (Shape)item;
                        tb = shape.TextBody;
                        break;
                    default:
                        break;
                }

                if (tb != null)
                {
                    matches += tb.Replace(pattern, replacement);
                }
            }

            return matches;
        }
        /// <summary>
        /// Replaces the single line.
        /// </summary>
        /// <param name="findText">The find text.</param>
        /// <param name="replacement">The replacement.</param>
        internal void ReplaceSingleLine(TextSelection[] findText, string replacement)
        {
            if (findText == null || findText.Length == 0)
                return;

            TextSelection selection = null;
            WTextRange selRange = null;
            int lastSelIndex = findText.Length - 1;

            // Remove all selections except first one.
            for (int i = lastSelIndex; i > 0; i--)
            {
                selection = findText[i];
                selection.SplitAndErase();
                RemoveOwnerPara(selection);
            }

            // Modify first selection with replacement.
            selection = findText[0];
            selRange = selection.GetAsOneRange();
            selRange.Text = replacement;
        }
        /// <summary>
        /// Replaces the single=line selection with TextSelection.
        /// </summary>
        /// <param name="findText">Single-line selection to replace.</param>
        /// <param name="replacement">The replacement.</param>
        internal void ReplaceSingleLine(TextSelection[] findText, TextSelection replacement)
        {
            if (findText == null || findText.Length == 0)
                return;

            TextSelection selection = null;
            int lastSelIndex = findText.Length - 1;

            // Remove all selections except first one.
            for (int i = lastSelIndex; i >= 0; i--)
            {
                selection = findText[i];
                int selIndex = selection.SplitAndErase();

                if (i == 0)
                {
                    WParagraph ownerPara = selection.OwnerParagraph;
                    replacement.CopyTo(ownerPara, selIndex, false, null);
                }
                else
                {
                    RemoveOwnerPara(selection);
                }
            }
        }
        /// <summary>
        /// Replaces the single-line  selection with TextBodypart.
        /// </summary>
        /// <param name="findText">Single-line selection to replace.</param>
        /// <param name="replacement">The replacement.</param>
        internal void ReplaceSingleLine(TextSelection[] findText, TextBodyPart replacement)
        {
            if (findText == null || findText.Length == 0)
                return;

            TextSelection selection = null;
            int lastSelIndex = findText.Length - 1;

            // Remove all selections except first one.
            for (int i = lastSelIndex; i >= 0; i--)
            {
                selection = findText[i];
                int selIndex = selection.SplitAndErase();

                if (i == 0)
                {
                    WParagraph ownerPara = selection.OwnerParagraph;
                    replacement.PasteAt(ownerPara.OwnerTextBody, ownerPara.GetIndexInOwnerCollection(), selIndex);
                }
                else
                {
                    RemoveOwnerPara(selection);
                }
            }
        }
        /// <summary>
        /// Removes the owner paragraph from selection in case paragraph is empty.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private void RemoveOwnerPara(TextSelection selection)
        {
            WParagraph para = selection.OwnerParagraph;
            if (para.Items.Count == 0)
                para.RemoveSelf();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Removes the internal items.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="end"></param>
        /// <param name="startIndex">Start index of the range.</param>
        /// <param name="nextTr"></param>
        private void RemoveInternalItems(WParagraph para, int end, int startIndex, out WTextRange nextTr)
        {
            int nextTrEndIndex = 0;
            bool last = false;
            nextTr = null;

            for (int i = startIndex; i < para.Items.Count; i++)
            {
                nextTr = para[i] as WTextRange;

                if (nextTr != null)
                {
                    nextTrEndIndex = nextTr.StartPos + nextTr.TextLength;

                    if (nextTrEndIndex > end)
                        break;
                    else if (nextTrEndIndex == end)
                        last = true;
                }

                ((ParagraphItemCollection)para.Items).UnsafeRemoveAt(i);

                if (last)
                {
                    nextTr = (i < para.Items.Count) ? para[i] as WTextRange : null;
                    break;
                }

                i--;
            }
        }
        /// <summary>
        /// Corrects the next items.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="offset">The offset.</param>
        private void CorrectNextItems(WParagraph para, int startIndex, int offset)
        {
            for (int i = startIndex, length = para.Items.Count; i < length; i++)
            {
                ParagraphItem item = para[i] as ParagraphItem;
                item.StartPos += offset;
            }
        }
        #endregion
    }
}
