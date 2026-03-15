#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection
{
    /// <summary>
    /// Class that is responsible for text selection in the document.
    /// </summary>
    internal class SelectionManager
    {
        #region Class constants
        /// <summary>
        /// Whitespace character.
        /// </summary>
        private const char DEF_WHITESPACE = ' ';

        /// <summary>
        /// String added to the data when the last symbol of data is whitespace.
        /// This is done as a workaround.
        /// </summary>
        private const string DEF_LAST_SYMBOL = ".";

        /// <summary>
        /// Default imterval for timer.
        /// </summary>
        private const int DEF_TIMER_INTERVAL = 100;

        /// <summary>
        /// Size of the region for text measuring.
        /// </summary>
        private int DEF_REGION_LENGTH = 32;
        #endregion

        #region Class static members
        /// <summary>
        /// String format for text measuring.
        /// </summary>
        private static StringFormat _formatMeasure;
        #endregion

        #region Class members
        /// <summary>
        /// Indicates whether selection is in process now or if user 
        /// can proceed selection.
        /// </summary>
        private bool m_bSelectionInProcess;

        /// <summary>
        /// Parent control object.
        /// </summary>
        private HTMLUIControl m_control;

        /// <summary>
        /// Last selected symbol info object.
        /// </summary>
        private LetterPair m_lastSymbolInfo;

        /// <summary>
        /// Indices of the selection region in the document.
        /// </summary>
        private SelectionRegion m_selectionRegion;

        /// <summary>
        /// Current index of the symbol.
        /// Is used during symbol creation.
        /// </summary>
        private int m_curSymbolIndex = 0;

        /// <summary>
        /// Start selection point.
        /// </summary>
        private Point m_startPoint;

        /// <summary>
        /// Text displayed in the document.
        /// </summary>
        private StringBuilder m_documentText;

        /// <summary>
        /// Timer object which scrolls the control during selection.
        /// </summary>
        private Timer m_scrollTimer;

        /// <summary>
        /// Collection of all text objects displayed at the current time.
        /// </summary>
        private ArrayList m_textObjects;

        /// <summary>
        /// Collection of selected elements in the document.
        /// </summary>
        private ArrayList m_selectedElements;

        /// <summary>
        /// Dialog for text searching.
        /// </summary>
        private FindDialog m_findDlg;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether selection is in process now or if user can proceed 
        /// selection.
        /// </summary>
        public bool SelectionInProcess
        {
            get
            {
                return m_bSelectionInProcess;
            }
            set
            {
                if (m_bSelectionInProcess != value)
                {
                    m_bSelectionInProcess = value;
                }
            }
        }

        /// <summary>
        /// Gets the selected text in the control.
        /// </summary>
        public string SelectedText
        {
            get
            {
                return GetSelectedText();
            }
        }

        /// <summary>
        /// Gets the object containing the indices of the selected text in the document.
        /// </summary>
        public SelectionRegion Region
        {
            get
            {
                return m_selectionRegion;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating the index of the current symbol in the document.
        /// Used during document calculation.
        /// </summary>
        public int SymbolIndex
        {
            get
            {
                return m_curSymbolIndex;
            }
            set
            {
                if (m_curSymbolIndex != value)
                {
                    m_curSymbolIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document has selected text.
        /// </summary>
        public bool HasSelectedText
        {
            get
            {
                return !this.Region.IsEmpty;
            }
        }

        /// <summary>
        /// Gets an array of selected elements in the document.
        /// </summary>
        public ArrayList SelectedElements
        {
            get
            {
                return m_selectedElements;
            }
        }

        /// <summary>
        /// Gets the text of the document.
        /// </summary>
        public string DocumentText
        {
            get
            {
                return m_documentText.ToString();
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the SelectionManager class
        /// </summary>
        static SelectionManager()
        {
            _formatMeasure = (StringFormat)StringFormat.GenericTypographic.Clone(); //// new StringFormat();
            _formatMeasure.FormatFlags |= StringFormatFlags.NoClip;
            _formatMeasure.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            _formatMeasure.Trimming = StringTrimming.Character;
        }

        /// <summary>
        /// Prevents a default instance of the SelectionManager class from being created
        /// </summary>
        private SelectionManager()
        {
            m_selectionRegion = SelectionRegion.Empty;
            m_textObjects = new ArrayList();
            m_selectedElements = new ArrayList();
        }

        /// <summary>
        /// Initializes a new instance of the SelectionManager class
        /// </summary>
        /// <param name="control">Parent Control object.</param>
        public SelectionManager(HTMLUIControl control)
            : this()
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Resets all data regarding selection.
        /// </summary>
        public void ResetSelection()
        {
            EndSelection();
            ClearSelectedRegion();

            m_lastSymbolInfo = null;
            m_startPoint = Point.Empty;
            m_selectedElements.Clear();
        }

        /// <summary>
        /// Resets data before document recalculation.
        /// </summary>
        public void ResetCalculation()
        {
            EndSelection();
            m_curSymbolIndex = 0;
			m_documentText = null;
            m_documentText = new StringBuilder();
            m_textObjects.Clear();
            this.Region.ResetCalculation();
        }

        /// <summary>
        /// Resets all data regarding selection.
        /// Used when new document is loading.
        /// </summary>
        public void Reset()
        {
            ResetSelection();
            ResetCalculation();
        }

        /// <summary>
        /// Starts the selection process.
        /// </summary>
        /// <param name="point">Start point for the selection.</param>
        public void StartSelection(Point point)
        {
            ResetSelection();

            m_startPoint = point;

            // NOTE: correct this for speed.
            m_control.Invalidate();

            this.SelectionInProcess = true;

            // Capture mouse. It may be needed for control scrolling 
            // during text selecting.
            m_control.Capture = true;
        }

        /// <summary>
        /// Stops the selection process.
        /// </summary>
        public void EndSelection()
        {
            this.SelectionInProcess = false;
            StopTimer();

            // Release mouse capture.
            m_control.Capture = false;
        }

        /// <summary>
        /// Proceeds with the selection process. Symbol at the point will be added to the selected region.
        /// </summary>
        /// <param name="point">Current point for selection.</param>
        /// <remarks>Point is at global coordinates.</remarks>
        public void ProcessSelection(Point point)
        {
            if (this.SelectionInProcess && point != m_startPoint)
            {
                // Try to use cached block.
                Block activeBlock = m_control.DocumentEx.Searcher.ActiveBlock;

                if (activeBlock == null)
                {
                    activeBlock = m_control.DocumentEx.Searcher.GetBlockAtPoint(point);
                }

                if (activeBlock != null)
                {
                    Text text = GetTextObject(activeBlock, point);

                    if (text != null)
                    {
                        LetterPair pair = text.Symbols[point];

                        if (pair != null && pair != m_lastSymbolInfo)
                        {
                            m_selectionRegion.AddSymbolIndex(pair.Number);
                            activeBlock.Owner.ProcessSelection();
                            m_lastSymbolInfo = pair;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts the text searching in the control.
        /// </summary>
        public void ProcessTextSearch()
        {
            if (m_findDlg == null)
            {
                m_findDlg = new FindDialog();
                m_findDlg.FindText += new FindTextEventHandler(FindText);
            }

            if (!m_findDlg.Visible)
            {
                m_findDlg.OpenDialog(m_control);
            }
        }

        /// <summary>
        /// Selects all text in the document.
        /// </summary>
        public void SelectAllText()
        {
            if (m_documentText != null && m_documentText.ToString().Length > 0)
            {
                ResetSelection();
                m_selectionRegion.FirstIndex = 0;
                m_selectionRegion.LastIndex = m_documentText.ToString().Length - 1;

                // NOTE: improve it.
                m_control.Invalidate();
            }
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Invoked by timer for control scrolling.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event data.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            Point clientPt = m_control.PointToClient(Control.MousePosition);
            Point curPt = m_control.DocumentEx.ClientToGlobal(clientPt);

            Margins margins = m_control.DocumentEx.Margins;
            Point startPoint = new Point(margins.Left, margins.Top);
            Rectangle globalRect = new Rectangle(startPoint, m_control.DocumentEx.AutoScrollMinSize);

            // Try to scroll if mouse is outside of client bounds but inside
            // of global bounds.
            if (!m_control.ClientRectangle.Contains(clientPt) &&
              globalRect.Contains(curPt))
            {
                int delta;
                Point scrollPt = m_control.AutoScrollPosition;
                scrollPt.X = Math.Abs(scrollPt.X);
                scrollPt.Y = Math.Abs(scrollPt.Y);
                Rectangle clientRect = m_control.ClientRectangle;

                if (clientPt.Y > clientRect.Bottom)
                {
                    delta = clientPt.Y - clientRect.Bottom;
                    scrollPt.Y += delta;
                }
                else if (clientPt.X > clientRect.Right)
                {
                    delta = clientPt.X - clientRect.Right;
                    scrollPt.X += delta;
                }
                else if (clientPt.Y < clientRect.Top)
                {
                    delta = clientRect.Top - clientPt.Y;
                    scrollPt.Y -= delta;
                }
                else if (clientPt.X < clientRect.Left)
                {
                    delta = clientRect.X - clientPt.X;
                    scrollPt.X -= delta;
                }

                Point pt = m_control.AutoScrollPosition;
                pt.X = Math.Abs(pt.X);
                pt.Y = Math.Abs(pt.Y);

                if (scrollPt != pt)
                {
                    m_control.SetAutoScrollPosition(scrollPt, m_control.DocumentEx);

                    // Reset cached found block and proceed selection.
                    m_control.DocumentEx.Searcher.ResetCache();
                    ProcessSelection(curPt);
                }
            }
        }

        /// <summary>
        /// Raised when text must be found in the control.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void FindText(object sender, FindTextEventArgs e)
        {
            int foundIndex = SearchText(e.Text, e.StartIndex, e.IsForward, e.MatchCase);

            // Text was found.
            if (foundIndex != -1)
            {
                // TODO: improve this.
                m_control.Invalidate();
            }

            e.StartIndex = foundIndex;

            if (e.IsForward && foundIndex >= 0)
            {
                e.StartIndex += e.Text.Length;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Calculates all text in the object.
        /// </summary>
        /// <param name="text">Text object for calcluation.</param>
        /// <param name="bounds">Bounds of the text.</param>
        /// <param name="g">Graphics object.</param>
        /// <param name="font">Font object for this text.</param>
        internal void CalculateText(Text text, Rectangle bounds, Graphics g, Font font)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (g == null)
                throw new ArgumentNullException("g");
            if (font == null)
                throw new ArgumentNullException("font");

            bounds.Width = int.MaxValue;
            bounds.Height = int.MaxValue;

            // Store indexes of the text and element.
            text.StartNumber = this.SymbolIndex;
            BaseElement parentElm = text.Parent.Owner;
            int lastIndex = this.SymbolIndex + text.Value.Length - 1;
            this.Region.SetElementIndex(parentElm, this.SymbolIndex, lastIndex);

            // Store displayed text of the document.
            m_documentText.Append(text.Value);
            CalculateRegion(g, text, font, bounds);
            m_textObjects.Add(text);
            text.IsCalculated = true;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculates the size of each symbol in the text.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="text">Text data for measuring</param>
        /// <param name="font">Font object for this text.</param>
        /// <param name="bounds">Bounds for the text.</param>
        private void CalculateRegion(Graphics g, Text text, Font font, Rectangle bounds)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            // This is done as workaround because last whitespace is merged
            // with previous symbol during character measuring.
            string strValue = text.Value;
            bool bExpanded = CheckCorectData(ref strValue);

            int arrLength = strValue.Length / DEF_REGION_LENGTH +
              ((strValue.Length % DEF_REGION_LENGTH) > 0 ? 1 : 0);
            int startIndex = 0;

            for (int i = 0; i < arrLength; i++)
            {
                CharacterRange[] characterRanges = BuildRange(strValue, startIndex);
                Region[] stringRegions;

                BaseElement._stringFormat.SetMeasurableCharacterRanges(characterRanges);

                stringRegions = g.MeasureCharacterRanges(strValue, font, bounds, BaseElement._stringFormat);

                bool beCareful = bExpanded && (i == arrLength - 1);
                InfillKeyPairCollection(g, text, stringRegions, startIndex, beCareful);

                startIndex += characterRanges.Length;
            }
        }

        /// <summary>
        /// Builds an array of ranges for measuring.
        /// </summary>
        /// <param name="text">String for measuring.</param>
        /// <param name="startIndex">Current start index.</param>
        /// <returns>Array of ranges.</returns>
        private CharacterRange[] BuildRange(string text, int startIndex)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            int length = Math.Min(DEF_REGION_LENGTH, text.Length - startIndex);
            CharacterRange[] characterRanges = new CharacterRange[length];

            for (int i = 0; i < length; i++)
            {
                characterRanges[i] = new CharacterRange(i + startIndex, 1);
            }

            return characterRanges;
        }

        /// <summary>
        /// Infills the collection of LetterPair objects holding information about symbols.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="text">Text data.</param>
        /// <param name="ranges">Array of symbol bounds.</param>
        /// <param name="startIndex">Index of the first symbol.</param>
        /// <param name="modified">Indicates whether string was expanded at the end.</param>
        private void InfillKeyPairCollection(Graphics g, Text text, Region[] ranges, int startIndex, bool modified)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (text == null)
                throw new ArgumentNullException("text");

            if (ranges == null)
                throw new ArgumentNullException("ranges");

            LetterPair keyPair;
            string symbol;
            RectangleF bound;

            for (int i = 0; i < ranges.Length; i++)
            {
                if (!((i == (ranges.Length - 1)) && modified))
                {
                    symbol = text.Value.Substring(startIndex + i, 1);
                    bound = ranges[i].GetBounds(g);

                    keyPair = new LetterPair(symbol, bound, m_curSymbolIndex);
                    text.Symbols.Add(m_curSymbolIndex, keyPair);
                    m_curSymbolIndex++;
                }       
            }
        }

        /// <summary>
        /// Expands string by one symbol if last symbol in the string is a whitespace.
        /// </summary>
        /// <param name="data">String data.</param>
        /// <returns>True if string if modified,; False otherwise.</returns>
        /// <remarks>This is workaround for correct measuring of last
        /// whitespace symbol in the string.</remarks>
        private bool CheckCorectData(ref string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("data - string can not be empty");

            bool bModified = false;
            char lastChar = data[data.Length - 1];

            if (lastChar == DEF_WHITESPACE)
            {
                data += DEF_LAST_SYMBOL;
                bModified = true;
            }

            return bModified;
        }

        /// <summary>
        /// Clears selected region.
        /// </summary>
        private void ClearSelectedRegion()
        {
            m_selectionRegion.ResetSelection();
        }

        /// <summary>
        /// Calculates the selected text in the document.
        /// </summary>
        /// <returns>Text selected in the document.</returns>
        private string GetSelectedText()
        {
            string selectedText = string.Empty;

            if (!this.Region.IsEmpty)
            {
                if (m_documentText != null && m_documentText.ToString().Length > 0)
                {
                    int length = this.Region.LastIndex - this.Region.FirstIndex + 1;
                    selectedText = m_documentText.ToString().Substring(this.Region.FirstIndex, length);
                }
            }

            return selectedText;
        }

        /// <summary>
        /// Searches the text object under the point in the block.
        /// </summary>
        /// <param name="block">Block containing point.</param>
        /// <param name="point">Current point coordinates.</param>
        /// <returns>Text object if found; Null otherwise.</returns>
        private Text GetTextObject(Block block, Point point)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            object elm;
            Rectangle elmRect;
            Text result = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                elm = block[i];
                elmRect = block[elm];

                if (elmRect.Contains(point) && elm is Text)
                {
                    result = elm as Text;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if scrolling of the control is needed and starts it.
        /// </summary>
        internal void CheckScrolling()
        {
            if (m_control.AutoScroll && this.SelectionInProcess)
            {
                Point mousePos = m_control.PointToClient(Control.MousePosition);

                if (!m_control.ClientRectangle.Contains(mousePos) &&
                  m_scrollTimer == null)
                {
                    StartTimer();
                }
                else if (m_control.ClientRectangle.Contains(mousePos) &&
                  m_scrollTimer != null)
                {
                    StopTimer();
                }
            }
        }

        /// <summary>
        /// Starts timer for invoking control scrolling.
        /// </summary>
        private void StartTimer()
        {
            if (m_scrollTimer == null)
            {
                m_scrollTimer = new Timer();
                m_scrollTimer.Tick += new EventHandler(TimerTick);
                m_scrollTimer.Interval = DEF_TIMER_INTERVAL;
            }

            m_scrollTimer.Start();
        }

        /// <summary>
        /// Stops and disposes timer object.
        /// </summary>
        private void StopTimer()
        {
            if (m_scrollTimer != null)
            {
                m_scrollTimer.Stop();
                m_scrollTimer.Dispose();
                m_scrollTimer = null;
            }
        }

        /// <summary>
        /// Searches text object containing symbol with the specified index.
        /// </summary>
        /// <param name="index">Index of the symbol.</param>
        /// <returns>Text object if found; Null otherwise.</returns>
        private Text GetTextByIndex(int index)
        {
            Text result = null;
            Text currText;

            for (int i = 0, len = m_textObjects.Count; i < len; i++)
            {
                currText = m_textObjects[i] as Text;

                if (currText != null && index >= currText.Symbols.MinNumber &&
                 index <= currText.Symbols.MaxNumber)
                {
                    result = currText;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches string data in the text displayed on the control.
        /// If found, selects this text.
        /// </summary>
        /// <param name="value">Search value.</param>
        /// <param name="startIndex">Index from where to start the search.</param>
        /// <param name="isForward">Indicates whether the order of searching is forward.</param>
        /// <param name="matchCase">Indicates whether search must match case.</param>
        /// <returns>Index of text if found; -1 otherwise.</returns>
        private int SearchText(string value, int startIndex, bool isForward, bool matchCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            int index = -1;

            if (value.Length > 0)
            {
                string wholeText = m_documentText.ToString();

                // case is not important.
                if (!matchCase)
                {
                    wholeText = wholeText.ToLower();
                    value = value.ToLower();
                }

                // Order of searching is forward.
                if (isForward && startIndex >= 0 && startIndex < wholeText.Length)
                {
                    index = wholeText.IndexOf(value, startIndex);
                }
                else if (!isForward && startIndex > 0 && startIndex <= wholeText.Length)
                {
                    index = wholeText.LastIndexOf(value, startIndex - 1, startIndex);
                }

                // Text is found. Select text and show it in client area.
                if (index >= 0)
                {
                    // Mark selected area.
                    m_selectionRegion.FirstIndex = index;
                    m_selectionRegion.LastIndex = index + value.Length - 1;

                    Text textObj = GetTextByIndex(m_selectionRegion.LastIndex);
                    Block parentBlock = textObj.Parent;
                    BaseElement parentElement = parentBlock.Owner;
                    RectangleF textRect = parentElement.GetSelectedRect(parentBlock, textObj);
                    m_control.ScrollToRectangle(Rectangle.Ceiling(textRect), parentElement.Document);
                    //// m_control.ScrollToText( textObj );
                }
            }

            return index;
        }
        #endregion
    }
}
