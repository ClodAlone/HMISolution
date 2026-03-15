#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Scroll;
using System.ComponentModel;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public abstract class LanguageBase : DependencyObject
    {
        #region local variables

        /// <summary>
        /// instance for Name property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string langName;

        /// <summary>
        /// instance for FileExtension property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string fileExtension;

        /// <summary>
        /// instance for case sensitive property.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        private bool caseSensitive;

        /// <summary>
        /// instance for TextForeground property.  default color is black.
        /// </summary>
        private Brush textForeground = Brushes.Black;

        /// <summary>
        /// instance for BlockStart property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string blockStart;

        /// <summary>
        /// instance for BlockEnd property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string blockEnd;

        /// <summary>
        /// Private Lexem collection object for get the comments type of lexems.
        /// </summary>
        internal IEnumerable<ILexem> commentsCol = null;

        /// <summary>
        /// Private Lexem collection object for get the operators type of lexems.
        /// </summary>
        internal IEnumerable<ILexem> operatorsCol = null;

        /// <summary>
        /// Private Lexem collection object for get the keywords type of lexems.
        /// </summary>
        internal IEnumerable<ILexem> keywordsCol = null;

        /// <summary>
        /// Private Lexem collection object for get the literals type of lexems.
        /// </summary>
        internal IEnumerable<ILexem> literalsCol = null;

        /// <summary>
        /// Private Brush object for get the current Foreground of Lexems.
        /// </summary>
        internal Brush currentColor;

        /// <summary>
        /// internal bool variable to identify if the thread is running
        /// </summary>
        internal bool isThreadRunning = false;

        /// <summary>
        /// internal variable to hold the current block listener
        /// </summary>
        internal BlockListener currentListener = null;

        /// <summary>
        /// internal stack object of type BlockListener to hold the parent blocks while applying expand and collapse.
        /// </summary>
        internal Stack<BlockListener> blocksStack;

        /// <summary>
        /// internal variable to hold the list of scopes availables. Its updated in the ApplyExpandCollapse and used in Intellisense to identify the items to
        /// be displayed in the intellisense in auto mode at different scopes.
        /// </summary>
        internal List<ScopeDefinition> scopeDefinitions;

        /// <summary>
        /// internal variable that holds all the namespace and types in a tree structure. Its constructed based on the assemblies added as reference in EditControl instance.
        /// </summary>
        internal EditTypeCollection typesCollection;

        /// <summary>
        /// internal variable that holds the collection of instances in the current scope.
        /// </summary>
        internal EditTypeCollection instancesCollection;

        /// <summary>
        /// internal variable that holds the list of namespaces being added in the content.
        /// </summary>
        internal List<string> includedNamespaces;

        /// <summary>
        /// internal variable that holds the reference to current scope.
        /// </summary>
        internal ScopeDefinition currentScope = null;

        /// <summary>
        /// internal collection objects to hold items generated from Language Lexem
        /// </summary>
        internal EditTypeCollection defaultItems = null;

        /// <summary>
        /// internal collection objects to hold items generated based on current scope and selected item.
        /// </summary>
        internal EditTypeCollection customItems = null;

        /// <summary>
        ///
        /// </summary>
        internal DispatcherTimer timer = null;

        /// <summary>
        ///
        /// </summary>
        private int timerValue = 0;

        /// <summary>
        ///
        /// </summary>
        internal LineItemExpandInformation currentItem = null;

        /// <summary>
        ///
        /// </summary>
        internal Stack<IIntellisenseItem> selectedIntellisenseItems = null;

        /// <summary>
        ///
        /// </summary>
        internal IIntellisenseItem previousSelectedItem = null;

        /// <summary>
        ///
        /// </summary>
        internal List<BlockListener> indentBlocks = null;

        #endregion local variables

        #region Dependency Properties

        /// <summary>
        /// Gets or Sets a collection of type of ILexem indicating the language configurations. It has been modified to IEnumerable type to allow the
        /// users to binding a custom collection as language lexems.
        /// </summary>
        public IEnumerable Lexem
        {
            get { return (IEnumerable)GetValue(LexemProperty); }
            set { SetValue(LexemProperty, value); }
        }

        /// <summary>
        /// Dependency property for Lexem property
        /// </summary>
        public static readonly DependencyProperty LexemProperty =
            DependencyProperty.Register("Lexem", typeof(IEnumerable), typeof(LanguageBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLexemsChanged)));

        /// <summary>
        /// Gets or Sets a collection of type of IFormat indicating the language configurations. It has been modified to IEnumerable type to allow the
        /// users to binding a custom collection as language formats.
        /// </summary>
        public IEnumerable Formats
        {
            get { return (IEnumerable)GetValue(FormatsProperty); }
            set { SetValue(FormatsProperty, value); }
        }

        /// <summary>
        /// Dependency property for Formats property
        /// </summary>
        public static readonly DependencyProperty FormatsProperty =
            DependencyProperty.Register("Formats", typeof(IEnumerable), typeof(LanguageBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFormatsChanged)));

        /// <summary>
        /// Gets or Sets a value indicating whether the language supports outlining
        /// </summary>
        public bool SupportsOutlining
        {
            get { return (bool)GetValue(SupportsOutliningProperty); }
            set { SetValue(SupportsOutliningProperty, value); }
        }

        /// <summary>
        /// Dependency property for Supports Outlining property
        /// </summary>
        public static readonly DependencyProperty SupportsOutliningProperty =
            DependencyProperty.Register("SupportsOutlining", typeof(bool), typeof(LanguageBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the selected should be indented or removed when tab key is pressed.
        /// </summary>
        public bool IsIndentSelectionOnTabEnabled
        {
            get { return (bool)GetValue(IsIndentSelectionOnTabEnabledProperty); }
            set { SetValue(IsIndentSelectionOnTabEnabledProperty, value); }
        }

        /// <summary>
        /// Dependency property for IsIndentSelectionOnTabEnabled Property
        /// </summary>
        public static readonly DependencyProperty IsIndentSelectionOnTabEnabledProperty =
            DependencyProperty.Register("IsIndentSelectionOnTabEnabled", typeof(bool), typeof(LanguageBase), new FrameworkPropertyMetadata(true));

        #endregion Dependency Properties

        #region Properties

        /// <summary>
        /// Gets a value indicating the parent EditControl reference.
        /// </summary>
        public EditControl ParentControl { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating the Regex to be applied for splitting the text in lines.
        /// </summary>
        public string SplitLinesRegex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the Regex to be applied for splitting the lines into individual tokens.
        /// </summary>
        public string SplitWordsRegex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the text in lines has to be splitted in to tokens.
        /// </summary>
        public bool IsSplitTextToWords { get; set; }

        /// <summary>
        /// internal property to hold a reference to current BlockCodes instance used in applying syntax colors
        /// </summary>
        internal BlockCodes CurrentBlock { get; set; }

        /// <summary>
        /// internal property to hold a reference to current IFormat instance used in applying in syntax colors.
        /// </summary>
        internal IFormat CurrentFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the language supports syntax highlighting
        /// </summary>
        public bool ApplyColoring { get; set; }

        /// <summary>
        /// Gets or sets Name of the Language.
        /// </summary>
        /// <remarks>
        /// <para>Specifies the Name of the Language.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.String</para>
        /// </value>
        public string Name
        {
            get
            {
                return langName;
            }

            set
            {
                langName = value;
            }
        }

        /// <summary>
        /// Gets or sets File Extension supported by the language
        /// </summary>
        /// <remarks>
        /// <para>Specifies the file extension.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.String</para>
        /// </value>
        public string FileExtension
        {
            get
            {
                return fileExtension;
            }

            set
            {
                if (value.TrimStart(' ').StartsWith("."))
                {
                    fileExtension = value;
                }
                else
                {
                    fileExtension = "." + value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Language has case sensitive or not
        /// </summary>
        /// <remarks>
        /// Specifies the particular language is case sensitive or not.
        /// </remarks>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return caseSensitive;
            }

            set
            {
                caseSensitive = value;
            }
        }

        /// <summary>
        /// Gets or sets foreground brush to be applied when no Lexems are applicable for
        /// the text
        /// </summary>
        /// <remarks>
        /// <para>Specifies the particular language Text Foreground.</para>
        /// </remarks>
        /// <value>
        /// <para>TextForeground = Brushes.Green;</para>
        /// </value>
        public Brush TextForeground
        {
            get
            {
                return textForeground;
            }

            set
            {
                textForeground = value;
            }
        }

        /// <summary>
        /// Gets or sets BlockStart property. Specifies the start symbol that denotes start of a block of code
        /// </summary>
        /// <remarks>
        /// Specifies the Block start text.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockStart
        {
            get
            {
                return blockStart;
            }

            set
            {
                blockStart = value;
            }
        }

        /// <summary>
        /// Gets or sets BlockEnd property. Specifies the end symbol that denotes end of block of code
        /// </summary>
        /// <remarks>
        /// Specifies the Block End text.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockEnd
        {
            get
            {
                return blockEnd;
            }

            set
            {
                blockEnd = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the language supports Intellisense or not
        /// </summary>
        /// <remarks>
        /// Specifies the particular language supports Intellisense or not.
        /// </remarks>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool SupportsIntellisense
        {
            get;
            set;
        }

        /// <summary>
        /// internal variable to hold a list of Blocks including preprocessors in the entire content.
        /// </summary>
        internal List<BlockListener> DocumentBlocks
        {
            get;
            set;
        }

        /// <summary>
        ///Gets or sets a value indicating the text to be displayed when a block is collapsed. By default its set to "..."
        /// </summary>
        public string EllipsisText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Types tree generated based on the assembly references.
        /// </summary>
        public EditTypeCollection TypesCollection
        {
            get
            {
                return this.typesCollection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating a char on which the sub-items of the intellisense items to displayed.
        /// </summary>
        public char IntellisenseDrillDownChar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating when the selected intellisense items to be appended. The string given will be coverted to individual characters internally and
        /// verified to append the selected item to text.
        /// </summary>
        public string IntellisenseCommitCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected intellisense items to be appended when space bar is pressed.
        /// </summary>
        public bool CommitsIntellisenseItemOnSpaceBar
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        internal bool isUpdating = false;

        /// <summary>
        ///
        /// </summary>
        public List<BlockListener> IndentableBlocks
        {
            get
            {
                return indentBlocks;
            }
            internal set
            {
                indentBlocks = value;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Edit.LanguageBase">LanguageBase</see> class.
        /// </summary>
        /// <param name="control">represents the parent EditControl</param>
        public LanguageBase(EditControl control)
        {
            if (control == null)
            {
                throw new NullReferenceException("EditControl reference cannot be null");
            }
            this.BlockStart = string.Empty;
            this.BlockEnd = string.Empty;
            this.IsSplitTextToWords = true;
            this.ParentControl = control;
            this.SplitLinesRegex = "\r\n|\n";
            this.SplitWordsRegex = @"\w+[\s|\W]|[\s|\W]+";
            DocumentBlocks = new List<BlockListener>();
            this.EllipsisText = "...";
            blocksStack = new Stack<BlockListener>();
            scopeDefinitions = new List<ScopeDefinition>();
            this.IntellisenseDrillDownChar = '.';
            this.IntellisenseCommitCharacters = @"{}[]().,:;+-*/%&|^!~=<>?@#'\\""";
            this.CommitsIntellisenseItemOnSpaceBar = true;
        }

        #endregion Constructor

        #region Implementation

        /// <summary>
        /// A helper method to split the text in the EditControl in to individual lines.
        /// </summary>
        public virtual void SplitTextToLines()
        {
            this.ParentControl.isReinitializeLines = false;
            string[] splittedText = Regex.Split(this.ParentControl.Text, this.SplitLinesRegex);
            if (ParentControl != null)
            {
                ParentControl.Lines.Clear();
            }
            ParentControl.isAddingLinesCompleted = false;
            foreach (string str in splittedText)
            {
                LineItem item = new LineItem(str);
                if (ParentControl.Lines.Count == splittedText.Length - 1)
                {
                    ParentControl.isAddingLinesCompleted = true;
                }

                ParentControl.Lines.Add(item);
            }

            CalculatePreferredWidth();
            this.ParentControl.isReinitializeLines = true;
        }

        /// <summary>
        /// Helper method to calculate preferred width to be applied to the ScrollColumn based on the text.
        /// </summary>
        internal void CalculatePreferredWidth()
        {
            if (ParentControl != null && ParentControl.Lines.Count > 0)
            {
                var maxWidth = ParentControl.Lines.Max(line => line.Text.Length);
                var maxWidthItem = ParentControl.Lines.Where(item => item.Text.Length == maxWidth);
                if (maxWidthItem.Count() > 0)
                {
                    LineItem item = maxWidthItem.ElementAt(0) as LineItem;
                    double itemWidth = 0.0, currentLineItemWidth = 0.0;
                    if (this.ParentControl.ScrollControl != null)
                    {
                        var currentLineItem = this.ParentControl.ScrollControl.CurrentLineItem;
                        if (currentLineItem != null)
                            currentLineItemWidth = Utils.GetWidth(currentLineItem.Text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
                        itemWidth = Utils.GetWidth(item.Text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
                    }

                    if (currentLineItemWidth >= itemWidth)
                    {
                        itemWidth = currentLineItemWidth;
                    }

                    if (ParentControl.PreferredWidth < itemWidth)
                    {
                        ParentControl.SetValue(EditControl.PreferredWidthProperty, itemWidth + 50);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to apply coloring to a line item.
        /// </summary>
        /// <param name="item"></param>
        internal virtual void ApplyFormats(LineItem item)
        {

            if (item.WordsCollection != null && ApplyColoring)
            {
                foreach (WordDetails details in item.WordsCollection)
                {
                    IFormat format = ApplyColor(details.Text, this.ParentControl.Lines.IndexOf(item) + 1);
                    UpdateWordFormats(details, format);
                }

                if (CurrentBlock != null && CurrentBlock.CloseAtEOL)
                {
                    CurrentBlock = null;
                }
            }

            item.LineStartFormat = this.CurrentFormat;
            item.LineStartBlock = this.CurrentBlock;
            item.Refresh();
        }

        /// <summary>
        /// Helper method to Split text in to individual words based on SplitWordsRegex
        /// </summary>
        /// <param name="text">represents the text to be splitted</param>
        /// <returns>a list of WordDetails objects after splitting the text</returns>
        internal virtual IList<WordDetails> SplitTextToWords(string text)
        {
            if (!IsSplitTextToWords)
            {
                return null;
            }

            IList<WordDetails> details = new List<WordDetails>();
            MatchCollection matches = Regex.Matches(text, SplitWordsRegex);
            int index = 0;
            double width = 0d;
            Point start = new Point(0, 0);

            foreach (Match match in matches)
            {
                string temp = text.Substring(index, match.Captures[0].Index - index);
                if (temp != string.Empty)
                {
                    WordDetails word = new WordDetails();
                    word.Text = temp;
                    word.StartIndex = index;
                    word.EndIndex = index + word.Text.Length;
                    width = Utils.GetWidth(word.Text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
                    word.StartPosition = start;
                    start = new Point(start.X + width, start.Y);
                    word.EndPosition = start;
                    details.Add(word);
                }

                WordDetails matchword = new WordDetails();
                matchword.Text = match.Value;
                matchword.StartIndex = match.Captures[0].Index;
                matchword.EndIndex = match.Captures[0].Index + match.Captures[0].Length;
                width = Utils.GetWidth(matchword.Text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
                matchword.StartPosition = start;
                start = new Point(start.X + width, start.Y);
                matchword.EndPosition = start;
                details.Add(matchword);
                index = match.Captures[0].Index + match.Captures[0].Length;
            }

            if (index < text.Length)
            {
                WordDetails word = new WordDetails();
                word.Text = text.Substring(index);
                word.StartIndex = index;
                word.EndIndex = text.Length - 1;
                width = Utils.GetWidth(word.Text, ParentControl.FontFamily, ParentControl.FontSize, ParentControl.Foreground);
                word.StartPosition = start;
                start = new Point(start.X + width, start.Y);
                word.EndPosition = start;
                details.Add(word);
            }

            return details;
        }

        /// <summary>
        /// Helper method to Split text in to individual words based on SplitWordsRegex
        /// </summary>
        /// <param name="text">represents the text to be splitted</param>
        /// <param name="lineNumber">represents the line number</param>
        /// <returns>a list of WordDetails objects after splitting the text</returns>
        internal virtual IList<WordDetails> SplitTextToWords(string text, int lineNumber)
        {
            return null;
        }

        /// <summary>
        /// Helper method to apply coloring to the text based on the Lexems in the Language configurations
        /// </summary>
        /// <param name="text">The text to apply color.</param>
        /// <param name="line">The line number of the text.</param>
        /// <returns>Returns the IFormat</returns>
        protected virtual IFormat ApplyColor(string text, int line)
        {
            return null;
        }

        /// <summary>
        /// Helper method to update text styles in the words property of the LineItem
        /// </summary>
        /// <param name="words">The words from the word details collection.</param>
        /// <param name="format">The format of the word.</param>

        internal virtual void UpdateWordFormats(WordDetails words, IFormat format)
        {
            if (format != null)
            {
                if (format.FontFamily == null)
                {
                    words.Font = ParentControl.FontFamily;
                }
                else
                {
                    words.Font = format.FontFamily;
                }

                if (double.IsNaN(format.FontSize) || format.FontSize == 0d)
                {
                    words.FontSize = ParentControl.FontSize;
                }
                else
                {
                    words.FontSize = format.FontSize;
                }

                words.Foreground = format.Foreground;
            }
            else
            {
                words.Foreground = ParentControl.Foreground;
                words.Font = ParentControl.FontFamily;
                words.FontSize = ParentControl.FontSize;
            }
        }

        /// <summary>
        /// internal helper method to reset the line's coloring when the text is changed.
        /// </summary>
        /// <param name="item">represents the line item object</param>
        internal virtual void ResetLine(LineItem item)
        {
            if (item != null)
            {
                if (this.IsSplitTextToWords)
                {
                    if (item.WordsCollection == null || (item.WordsCollection != null && item.Text != this.GetSplittedText(item)))
                    {
                        item.WordsCollection = SplitTextToWords(item.Text);
                    }

                    if (this.ApplyColoring)
                    {
                        if (item.LineNumber > 0)
                        {
                            ResetColor(item.LineNumber - 1);
                        }
                        else
                        {
                            var index = this.ParentControl.Lines.IndexOf(item);
                            ResetColor(index);
                        }
                    }
                }
                item.Refresh();
            }
        }

        /// <summary>
        /// Helper method to get the splitted text from a LineItem object
        /// </summary>
        /// <param name="item">represents a LineItem</param>
        /// <returns>the splitted string </returns>
        private string GetSplittedText(LineItem item)
        {
            StringBuilder splittedText = new StringBuilder(string.Empty);
            if (item != null && item.WordsCollection != null)
            {
                foreach (WordDetails word in item.WordsCollection)
                {
                    splittedText.Append(word.Text);
                }
            }
            return splittedText.ToString();
        }

        /// <summary>
        /// internal helper method to reset the color of the a line
        /// </summary>
        /// <param name="line"></param>
        internal virtual void ResetColor(int line)
        {
            for (int i = line; i >= 0 && i < ParentControl.Lines.Count; i++)
            {
                var lineItem = ParentControl.Lines[i];
                if (i == line)
                {
                    this.CurrentBlock = lineItem.LineStartBlock;
                    this.CurrentFormat = lineItem.LineStartFormat;
                }
                else
                {
                    if (this.CurrentBlock == lineItem.LineStartBlock)
                    {
                        return;
                    }
                }
                this.ApplyFormats(lineItem);
            }
        }

        /// <summary>
        /// Helper method to generate Regex pattern to tokenize procedural code using the lexems in Language configurations
        /// </summary>
        /// <returns>Retturns the Regex pattern.</returns>
        internal virtual string GeneratePattern()
        {
            StringBuilder string1 = new StringBuilder();
            if (commentsCol != null)
            {
                foreach (ILexem comm in commentsCol)
                {
                    if (comm != null)
                    {
                        if (comm.IsRegex)
                        {
                            string1.Append(comm.StartText + "|");

                            if (comm.IsMultiline)
                            {
                                string1.Append(comm.EndText + "|");
                            }
                        }
                        else
                        {
                            string1.Append(Regex.Escape(comm.StartText) + "|");

                            if (comm.IsMultiline)
                            {
                                string1.Append(Regex.Escape(comm.EndText) + "|");
                            }
                        }
                    }
                }
            }

            if (literalsCol != null)
            {
                foreach (ILexem str in literalsCol)
                {
                    if (str != null)
                    {
                        string1.Append(Regex.Escape(str.StartText) + "|");

                        if (str.IsMultiline || str.StartText != str.EndText)
                        {
                            string1.Append(Regex.Escape(str.EndText) + "|");
                        }
                    }
                }
            }

            if (operatorsCol != null)
            {
                foreach (Lexem str in operatorsCol)
                {
                    if (str != null) string1.Append(AppendEscapeForPattern(str.StartText));
                }
            }

            if (keywordsCol != null)
            {
                foreach (Lexem str in keywordsCol)
                {
                    if (str != null) string1.Append(Regex.Escape(str.StartText));
                }
            }

            string1 = new StringBuilder(string1.ToString().TrimEnd('|'));
            string1.Append("|[?=\\t]|[?=\\s]");
            return string1.ToString();
        }

        /// <summary>
        /// Appends the escape for pattern.
        /// </summary>
        /// <param name="str">The text to append.</param>
        /// <returns>Returns the append text.</returns>
        internal string AppendEscapeForPattern(string str)
        {
            StringBuilder string1 = new StringBuilder();

            char[] splitstr = str.ToCharArray();
            StringBuilder temp = new StringBuilder();
            foreach (char c in splitstr)
            {
                if (c == '-' | c == '<' | c == '>')
                {
                    temp.Append(@"\" + c);
                }
                else if (!char.IsLetterOrDigit(c))
                {
                    temp.Append(Regex.Escape(c.ToString()));
                }
            }

            string1.Append("[?=" + temp.ToString() + "]|");
            return string1.ToString();
        }

        /// <summary>
        /// Helper method to retreive IFormat object from the Formats Collection based on FormatName
        /// </summary>
        /// <returns>Retturns the IFormat object.</returns>
        internal IFormat GetFormat(string p)
        {
            try
            {
                if (this.Formats != null && p != null)
                {
                    return this.Formats.OfType<IFormat>().Where(format => format.FormatName == p).ElementAt(0);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// PropertyChangedCallback for Lexem DependencyProperty
        /// </summary>
        /// <param name="obj">represents the dependency object</param>
        /// <param name="e">represents the DependencypPropertyChangedEventArgs</param>
        private static void OnLexemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as LanguageBase).OnLexemsChanged(e);
        }

        /// <summary>
        /// Helper method to perform operations when the Lexem property gets changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLexemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                commentsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
                literalsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Literals);
                operatorsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Operator);
                keywordsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Keyword || lex.LexemType == EditTokenType.CodeSnippet || lex.LexemType == EditTokenType.Operator || lex.LexemType == EditTokenType.NamespaceDeclaration);
                this.SplitWordsRegex = GeneratePattern();
            }
            else
            {
                commentsCol = null;
                literalsCol = null;
                operatorsCol = null;
                keywordsCol = null;
            }
        }

        /// <summary>
        /// PropertyChangedCallback for Formats DependencyProperty
        /// </summary>
        /// <param name="obj">represents the dependency object</param>
        /// <param name="e">represents the DependencypPropertyChangedEventArgs</param>
        private static void OnFormatsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as LanguageBase).OnFormatsChanged(e);
        }

        /// <summary>
        /// Helper method to perform operations when the Formats property gets changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFormatsChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Helper method to Apply Expansions for the content in the EditControl.
        /// </summary>
        public virtual void ApplyExpandItems()
        {
            if (this.ParentControl.EnableOutlining && this.SupportsOutlining && !isThreadRunning)
            {
                BackgroundWorker workerExpand = new BackgroundWorker();
                List<object> parameters = new List<object>();
                parameters.Add(new List<LineItemExpandInformation>((this.ParentControl.Lines).Select(line => line.GetLineItemExpandDetails()).ToList<LineItemExpandInformation>()));
                parameters.Add(GetBlockStartList());
                parameters.Add(new List<ILexem>(this.Lexem.OfType<ILexem>()));
                if (this.ParentControl.AssemblyReferences != null)
                {
                    parameters.Add(new List<Uri>(this.ParentControl.AssemblyReferences));
                }
                if (DocumentBlocks != null)
                {
                    this.DocumentBlocks.Clear();
                }

                if (IndentableBlocks != null)
                {
                    this.IndentableBlocks.Clear();
                }
                else
                {
                    this.IndentableBlocks = new List<BlockListener>();
                }
                workerExpand.DoWork += (sender, args) =>
                {
                    lock (this)
                    {
                        isThreadRunning = true;
                        List<object> parametersList = args.Argument as List<object>;

                        List<LineItemExpandInformation> itemsCollection = parametersList[0] as List<LineItemExpandInformation>;
                        List<BlockListener> langBlocks = parametersList[1] as List<BlockListener>;
                        List<ILexem> lexems = parametersList[2] as List<ILexem>;
                        List<Uri> assemblies = parametersList.Count > 3 ? parametersList[3] as List<Uri> : null;

                        if (this.scopeDefinitions != null && this.scopeDefinitions.Count > 0)
                        {
                            this.scopeDefinitions.Clear();
                        }

                        if (this.blocksStack != null)
                        {
                            this.blocksStack.Clear();
                        }

                        currentListener = null;
                        this.InitializeApplyExpandCollapse();
                        foreach (LineItemExpandInformation item in itemsCollection)
                        {
                            item.LineStartBlockListener = currentListener;
                            item.ParentListeners = blocksStack;
                            ApplyExpandCollapseArgs expandArgs = new ApplyExpandCollapseArgs()
                            {
                                ExpandInformation = item,
                                Source = itemsCollection,
                                LanguageBlocks = langBlocks,
                                Assemblies = assemblies,
                                Lexems = lexems
                            };

                            this.ApplyExpandCollapse(expandArgs);
                        }
                        args.Result = itemsCollection;
                    }
                };
                workerExpand.RunWorkerCompleted += (sender, args) =>
                {
                    List<LineItemExpandInformation> itemsCollection = args.Result as List<LineItemExpandInformation>;
                    for (int i = 0; i < itemsCollection.Count; i++)
                    {
                        if (this.ParentControl.Lines.Count > i)
                        {
                            var lineItem = this.ParentControl.Lines[i];
                            int childLines = lineItem.ContainsLines ? lineItem.EndLine - lineItem.StartLine : 0;
                            int newChildLines = itemsCollection[i].ContainsLines ? itemsCollection[i].EndLine - itemsCollection[i].StartLine : 0;
                            if ((!lineItem.IsExpanded && !itemsCollection[i].ContainsLines) || (!lineItem.IsExpanded && childLines != newChildLines))
                            {
                                itemsCollection[i].ToggleExpansion = true;
                            }

                            lineItem.ContainsLines = itemsCollection[i].ContainsLines;
                            lineItem.IsExpanded = itemsCollection[i].IsExpanded;
                            lineItem.ParentListeners = itemsCollection[i].ParentListeners;
                            lineItem.StartLine = itemsCollection[i].StartLine;
                            lineItem.EndLine = itemsCollection[i].EndLine;
                            if (itemsCollection[i].ParentLineNumber > 0)
                            {
                                var parentItem = this.ParentControl.Lines[itemsCollection[i].ParentLineNumber - 1];
                                bool prevValue = lineItem.IsEndLine ? true : false;
                                if (parentItem.EndLine - 1 == i)
                                {
                                    lineItem.IsEndLine = true;
                                }
                                else
                                {
                                    lineItem.IsEndLine = false;
                                }

                                lineItem.ParentLineNumber = -1;
                            }
                            lineItem.ParentLineNumber = itemsCollection[i].ParentLineNumber;
                            lineItem.LineStartBlock = itemsCollection[i].LineStartBlock;
                            lineItem.LineStartFormat = itemsCollection[i].LineStartFormat;
                            lineItem.LineStartBlockListener = itemsCollection[i].LineStartBlockListener;
                            lineItem.PreprocessorText = itemsCollection[i].PreprocessorText;
                            lineItem.ContainsPreprocessor = itemsCollection[i].ContainsPreprocessor;
                            if (itemsCollection[i].ToggleExpansion)
                            {
                                this.ParentControl.ExpandLine(i);
                            }
                        }
                    }
                    itemsCollection.Clear();
                    itemsCollection = null;
                    parameters.Clear();
                    parameters = null;
                    isThreadRunning = false;
                };

                workerExpand.RunWorkerAsync(parameters);
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void InitializeApplyExpandCollapse()
        {
        }

        /// <summary>
        /// Helper method to refresh lines expansions from a specified line number. Here line number refers to the index (starts from 0)
        /// </summary>
        /// <param name="line"></param>
        public virtual void RefreshExpandItems(int line)
        {
            if (this.ParentControl.EnableOutlining && this.SupportsOutlining && !isThreadRunning)
            {
                BackgroundWorker workerExpand = new BackgroundWorker();
                List<object> parameters = new List<object>();
                parameters.Add(new List<LineItemExpandInformation>((this.ParentControl.Lines).Select(lineitem => lineitem.GetLineItemExpandDetails()).ToList<LineItemExpandInformation>()));
                parameters.Add(GetBlockStartList());
                parameters.Add(new List<ILexem>(this.Lexem.OfType<ILexem>()));
                if (this.ParentControl.AssemblyReferences != null)
                {
                    parameters.Add(new List<Uri>(this.ParentControl.AssemblyReferences));
                }
                else
                {
                    parameters.Add(new List<Uri>());
                }
                parameters.Add(line);
                workerExpand.DoWork += (sender, args) =>
                {
                    isThreadRunning = true;
                    List<object> parametersList = args.Argument as List<object>;

                    List<LineItemExpandInformation> itemsCollection = parametersList[0] as List<LineItemExpandInformation>;
                    List<BlockListener> langBlocks = parametersList[1] as List<BlockListener>;
                    List<ILexem> lexems = parametersList[2] as List<ILexem>;
                    List<Uri> assemblies = parametersList.Count > 3 ? parametersList[3] as List<Uri> : null;
                    int lineStart = (int)parametersList[4];
                    int endline = itemsCollection.Count;

                    if (blocksStack != null)
                    {
                        this.blocksStack.Clear();
                    }

                    for (int i = lineStart; i < itemsCollection.Count; i++)
                    {
                        var item = itemsCollection[i];
                        if (i == lineStart)
                        {
                            currentListener = item.LineStartBlockListener;
                            blocksStack = item.ParentListeners;
                            if (currentListener != null)
                            {
                                currentItem = itemsCollection[currentListener.ParentLineNumber - 1];
                            }
                        }
                        else
                        {
                            if (item.LineStartBlockListener == currentListener)
                            {
                                endline = i;
                                break;
                            }
                        }
                        ApplyExpandCollapseArgs expandArgs = new ApplyExpandCollapseArgs()
                        {
                            ExpandInformation = item,
                            Source = itemsCollection,
                            LanguageBlocks = langBlocks,
                            Assemblies = assemblies,
                            Lexems = lexems
                        };

                        this.ApplyExpandCollapse(expandArgs);
                    }
                    List<object> results = new List<object>();
                    results.Add(itemsCollection);
                    results.Add(endline);
                    args.Result = results;
                };
                workerExpand.RunWorkerCompleted += (sender, args) =>
                {
                    List<object> tempParameters = args.Result as List<object>;
                    List<LineItemExpandInformation> itemsCollection = tempParameters[0] as List<LineItemExpandInformation>;
                    int endline = (int)tempParameters[1];
                    for (int i = line; i < endline; i++)
                    {
                        this.ParentControl.Lines[i].ContainsLines = itemsCollection[i].ContainsLines;
                        this.ParentControl.Lines[i].IsExpanded = itemsCollection[i].IsExpanded;
                        this.ParentControl.Lines[i].ParentListeners = itemsCollection[i].ParentListeners;
                        this.ParentControl.Lines[i].StartLine = itemsCollection[i].StartLine;
                        this.ParentControl.Lines[i].EndLine = itemsCollection[i].EndLine;
                        this.ParentControl.Lines[i].ParentLineNumber = itemsCollection[i].ParentLineNumber;
                        this.ParentControl.Lines[i].LineStartBlock = itemsCollection[i].LineStartBlock;
                        this.ParentControl.Lines[i].LineStartFormat = itemsCollection[i].LineStartFormat;
                        this.ParentControl.Lines[i].LineStartBlockListener = itemsCollection[i].LineStartBlockListener;
                        this.ParentControl.Lines[i].PreprocessorText = itemsCollection[i].PreprocessorText;
                        this.ParentControl.Lines[i].ContainsPreprocessor = itemsCollection[i].ContainsPreprocessor;
                    }
                    itemsCollection.Clear();
                    itemsCollection = null;
                    parameters.Clear();
                    parameters = null;
                    isThreadRunning = false;
                };

                workerExpand.RunWorkerAsync(parameters);
            }
        }

        /// <summary>
        /// Helper method for perform expand collapse for line items. This method can be overridden if custom expand logics has be implemented.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void ApplyExpandCollapse(ApplyExpandCollapseArgs args)
        {
        }

        /// <summary>
        /// Helper method to generate the list of BlockListener objects from language Lexem
        /// </summary>
        /// <returns></returns>
        internal virtual List<BlockListener> GetBlockStartList()
        {
            return null;
        }

        /// <summary>
        /// Helper method that returns the text to be displayed when a line is collapsed.
        /// </summary>
        /// <param name="item">represents the line item</param>
        /// <returns>a string to be displayed when a line is collapsed</returns>
        internal virtual string GetCollapsedItemText(LineItem item)
        {
            return item.Text;
        }

        /// <summary>
        /// Helper method that performs the rendering of line item.
        /// </summary>
        /// <param name="item">represents the line item</param>
        /// <param name="context">represents the drawingcontext object</param>
        internal virtual void OnLineItemRender(LineItem item, DrawingContext context)
        {
            if (this.ApplyColoring && item.WordsCollection != null)
            {
                Point startPosition = new Point();
                if (!item.ContainsLines || (item.ContainsLines && item.IsExpanded))
                {
                    foreach (WordDetails word in item.WordsCollection)
                    {
                        FormattedText formattext = Utils.GetFormattedText(word.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, word.Foreground);
                        Point pos = new Point();
                        pos.X = startPosition.X;
                        pos.Y = 0;
                        word.StartPosition = startPosition;
                        context.DrawText(formattext, pos);
                        startPosition.X += double.Parse(string.Format("{0:0.00}", formattext.WidthIncludingTrailingWhitespace));
                    }
                }
                else
                {
                    BlockListener block = item.GetPreprocessorType();
                    FormattedText text = null;
                    if (item.ContainsPreprocessor && block != null)
                    {
                        foreach (WordDetails word in item.WordsCollection)
                        {
                            if (word.Text.StartsWith(block.BlockStart))
                            {
                                break;
                            }
                            text = Utils.GetFormattedText(word.Text, item.ParentControl.FontFamily, item.ParentControl.FontSize, word.Foreground);
                            Point pos = new Point();
                            pos.X = startPosition.X;
                            pos.Y = 0;
                            word.StartPosition = startPosition;
                            context.DrawText(text, pos);
                            startPosition.X += double.Parse(string.Format("{0:0.00}", text.WidthIncludingTrailingWhitespace));
                        }
                        //startPosition.X = Utils.GetWidth(item.Text.Substring(0, item.Text.IndexOf(block.BlockStart)), ParentControl.FontFamily, ParentControl.FontSize, Brushes.Gray) + 1;
                        text = Utils.GetFormattedText(item.PreprocessorText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.CollapsedTextForeground);
                    }
                    else
                    {
                        if (block == null && item.ContainsPreprocessor)
                        {
                            item.ContainsPreprocessor = false;
                        }

                        foreach (WordDetails word in item.WordsCollection)
                        {
                            FormattedText formattext = Utils.GetFormattedText(word.Text, item.ParentControl.FontFamily, item.ParentControl.FontSize, word.Foreground);
                            Point pos = new Point();
                            pos.X = startPosition.X;
                            pos.Y = 0;
                            word.StartPosition = startPosition;
                            context.DrawText(formattext, pos);
                            startPosition.X += double.Parse(string.Format("{0:0.00}", formattext.WidthIncludingTrailingWhitespace));
                        }
                        text = Utils.GetFormattedText(this.EllipsisText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.CollapsedTextForeground);
                    }
                    item.EllipsisPosition = startPosition;
                    context.DrawText(text, startPosition);
                    context.DrawRoundedRectangle(Brushes.Transparent, new Pen(this.ParentControl.CollapsedTextForeground, 0.5d), new Rect(startPosition, new Size(text.WidthIncludingTrailingWhitespace, item.ParentControl.LineHeight)), 1, 1);
                }
            }
            else
            {
                context.DrawText(Utils.GetFormattedText(item.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground), new Point(0, 0));
            }
        }

        /// <summary>
        /// Helper method to update the SelectionPointer when the item is in collapsed state.
        /// </summary>
        /// <param name="item">represents the collapsed item</param>
        internal virtual void UpdateCollapsedItemSelectionPointer(LineItem item)
        {
        }

        /// <summary>
        /// Helper method to calculate the end index of the selection for a collapsed lineitem. This method helps to specify different end index for different languages.
        /// </summary>
        /// <param name="lineitem">represents the collapsed line item</param>
        /// <param name="regularEndIndex">represents the end index in expanded state</param>
        /// <returns></returns>
        internal virtual int GetCollapsedItemSelectionEndIndex(LineItem lineitem, int regularEndIndex)
        {
            if (regularEndIndex == lineitem.Text.Length)
            {
                return this.GetCollapsedItemText(lineitem).Length;
            }
            return regularEndIndex;
        }

        /// <summary>
        /// Helper method perform the Comment Command operations for an individual line
        /// </summary>
        internal virtual void OnCommentCommandExecute(LineItem line)
        {
        }

        /// <summary>
        /// Helper method perform the Comment Command operations for lines in a selected range
        /// </summary>
        internal virtual void OnCommentCommandExecute(SelectionPointer pointer)
        {
        }

        /// <summary>
        /// Helper method perform the Comment Command operations for an individual line
        /// </summary>
        internal virtual void OnCommentCommandExecute(LineItem line, int index, ILexem commentLexem)
        {
        }

        /// <summary>
        /// Helper method perform the Uncomment Command operations for an individual line
        /// </summary>
        internal virtual void OnUncommentCommandExecute(LineItem line)
        {
        }

        /// <summary>
        /// Helper method perform the Uncomment Command operations for an individual line
        /// </summary>
        internal virtual void OnUncommentCommandExecute(LineItem line, ILexem commentLexem)
        {
        }

        /// <summary>
        /// Helper method perform the Comment Command operations for lines in a selected range
        /// </summary>
        internal virtual void OnUncommentCommandExecute(SelectionPointer pointer)
        {
        }

        /// <summary>
        /// Helper method to perform any language related operations when the user inputs any character in EditControl.
        /// </summary>
        /// <param name="currentLineNumber">represents the current line number</param>
        /// <param name="e">represent TextCompositionEventArgs</param>
        internal virtual void OnTextInput(int currentLineNumber, System.Windows.Input.TextCompositionEventArgs e)
        {
        }

        #endregion Implementation

        #region Intellisense

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void ShowIntellisenseBox(EditIntellisenseArgs args)
        {
            args.CurrentScope = this.GetScopeDefinition(this.ParentControl.LineNumber - 1);

            WordDetails word = this.ParentControl.ScrollControl.GetCurrentWord(args.CursorIndex);
            if (word != null)
            {
                int count = this.ApplyFilterToIntellisense(word.Text); ;
                if (count == 1)
                {
                    this.ParentControl.UpdateSelectedIntellisenseItem(this.ParentControl.intellisenseBox.SelectedItem as IIntellisenseItem);
                    return;
                }
            }

            this.OnIntellisenseBoxOpening(args);
            this.ParentControl.RaiseIntellisenseBoxOpeningEvent(args);

            if (!args.Cancel)
            {
                this.ParentControl.intellisenseBox.ItemsSource = args.ItemsSource;
                this.PositionIntellisenseBox(args);
            }
        }

        internal virtual void OnIntellisenseBoxOpening(EditIntellisenseArgs args)
        {
            if (this.ParentControl.IntellisenseMode == IntellisenseMode.Custom)
            {
                args.ItemsSource = this.ParentControl.IntellisenseCustomItemsSource.OfType<IIntellisenseItem>();
            }
        }

        /// <summary>
        /// Method to adjust the position of the intellisense box.
        /// </summary>
        /// <param name="args">represents the cursor index</param>
        public virtual void PositionIntellisenseBox(EditIntellisenseArgs args)
        {
            if (ParentControl.intellisenseBox.ItemsSource != null)
            {
                VisibleLinesCollection collection = this.ParentControl.ScrollControl.ScrollRows.GetVisibleLines();
                var visiblelines = collection.Where(vline => vline.LineIndex == args.LineIndex);
                VisibleLineInfo visibleline = null;
                if (visiblelines.Count() > 0)
                {
                    visibleline = visiblelines.ElementAt(0);
                }

                double xPos = this.ParentControl.ScrollControl.Caret != null ? this.ParentControl.ScrollControl.Caret.CaretPosition.X - 10 : 0;
                double yPos = visibleline != null ? visibleline.Origin + this.ParentControl.LineHeight : this.ParentControl.LineHeight;
                this.ParentControl.intellisensePopup.PlacementRectangle = new Rect(xPos, yPos, 300, 200);
                if (!this.ParentControl.isIntellisenseBoxOpen)
                {
                    this.ParentControl.intellisensePopup.IsOpen = true;
                    this.ParentControl.intellisensePopup.StaysOpen = false;
                    this.ParentControl.isIntellisenseBoxOpen = true;
                }
            }
            else
            {
                this.HideIntellisensePopup();
            }
        }

        internal ScopeDefinition GetScopeDefinition(int line)
        {
            var scope = this.scopeDefinitions.Where(scp => scp.StartLine <= line).OrderByDescending(scp => scp.StartLine);
            if (scope.Count() > 0)
            {
                return scope.ElementAt(0);
            }
            return null;
        }

        /// <summary>
        /// Helper method to iterate through the namespace tree and add items accordingly
        /// </summary>
        /// <param name="infoList"></param>
        /// <param name="typeName"></param>
        private void UpdateEditTypeInfoList(EditTypeCollection infoList, string typeName)
        {
            //EditTypeCollection infoList = listItem != null && listItem.Count() > 0 ? new EditTypeCollection(listItem as IEnumerable<EditTypeInfo>) : new EditTypeCollection();
            if (typeName.IndexOf(this.IntellisenseDrillDownChar) > 0)
            {
                string tempStr = typeName.Substring(0, typeName.IndexOf(this.IntellisenseDrillDownChar)).TrimEnd(this.IntellisenseDrillDownChar).TrimStart(this.IntellisenseDrillDownChar);
                string remaingStr = typeName.Substring(Math.Min(typeName.IndexOf(this.IntellisenseDrillDownChar) + 1, typeName.Length));
                EditTypeInfo tempItem = infoList.GetEditTypeInfo(tempStr) as EditTypeInfo;
                if (tempItem == null)
                {
                    tempItem = new EditTypeInfo()
                    {
                        Name = tempStr,
                        IsNamespace = true
                    };
                    if (remaingStr != string.Empty)
                    {
                        tempItem.NestedItems = new EditTypeCollection();
                        this.UpdateEditTypeInfoList(tempItem.NestedItems as EditTypeCollection, remaingStr);
                    }
                    infoList.Add(tempItem as EditTypeInfo);
                }
                else
                {
                    if (tempItem.NestedItems == null)
                    {
                        tempItem.NestedItems = new EditTypeCollection();
                    }
                    this.UpdateEditTypeInfoList(tempItem.NestedItems as EditTypeCollection, remaingStr);
                }
            }
            else if (typeName.Trim().Length > 0)
            {
                EditTypeInfo tempItem = infoList.GetEditTypeInfo(typeName) as EditTypeInfo;
                if (tempItem == null)
                {
                    tempItem = new EditTypeInfo()
                    {
                        Name = typeName,
                        IsNamespace = true
                    };
                    infoList.Add(tempItem);
                }
            }
        }

        internal BackgroundWorker worker = null;

        /// <summary>
        /// Helper method to load generate namespace tree.
        /// </summary>
        /// <param name="assemblies">represents the list of assemblies added</param>
        internal void InitializeNamespaces(IEnumerable<Uri> assemblies)
        {
            if (assemblies == null)
                return;

            if (!EnvironmentTest.IsSecurityGranted)
            {
                return;
            }

            this.ParentControl.isAssemblyInitializing = true;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (s, args) =>
            {
                IEnumerable<Uri> uriList = new List<Uri>(args.Argument as IEnumerable<Uri>);
                EditTypeCollection infoList = new EditTypeCollection();
                foreach (Uri uri in uriList)
                {
                    Assembly assembly = Assembly.LoadFile(uri.LocalPath);
                    var types = assembly.GetTypes().Where(type => type.IsPublic).GroupBy(type => type.Namespace);
                    foreach (var item in types)
                    {
                        UpdateEditTypeInfoList(infoList, item.Key);
                    }
                }
                List<object> results = new List<object>();
                results.Add(infoList);
                results.Add(uriList.Count());
                args.Result = results;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                List<object> resultsList = args.Result as List<object>;
                var Collection = resultsList[0] as EditTypeCollection;
                if (Collection != null)
                {
                    this.typesCollection = Collection;
                }
                this.ParentControl.isAssemblyInitializing = false;
                if ((int)resultsList[1] != this.ParentControl.AssemblyReferences.Count())
                {
                    this.ParentControl.EndAssemblyInit();
                }
            };
            worker.RunWorkerAsync(assemblies);
        }

        internal IIntellisenseItem GetIntellisenseItem(IEnumerable<IIntellisenseItem> itemsCollection, string itemName)
        {
            if (itemsCollection != null)
            {
                var items = itemsCollection.Where(itm => itm.Text == itemName);
                if (items.Count() > 0)
                {
                    return items.ElementAt(0);
                }
            }
            return null;
        }

        internal bool ContainstIntellisenseItem(IEnumerable<IIntellisenseItem> itemsCollection, string itemName)
        {
            if (itemsCollection != null)
            {
                var items = itemsCollection.Where(itm => itm.Text == itemName);
                if (items.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Helper method to select the item in the intellisense listbox when the text is typed by the user.
        /// </summary>
        /// <param name="intellisenseFilterString">represents current word text</param>
        /// <returns>index of the selected item.</returns>
        internal int ApplyFilterToIntellisense(string intellisenseFilterString)
        {
            IEnumerable<IIntellisenseItem> collection = null;
            if (this.ParentControl.intellisenseBox.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ParentControl.intellisenseBox.ItemsSource);
                collection = view.SourceCollection.OfType<IIntellisenseItem>();
            }

            if (collection == null)
            {
                return -1;
            }
            var indices = collection.Where(item => item.Text.ToLower().StartsWith(intellisenseFilterString.ToLower()));
            if (indices.Count() > 0)
            {
                IIntellisenseItem tempItem = indices.ElementAt(0);
                if (intellisenseFilterString.Length > 1 || (intellisenseFilterString.Length == 1 && tempItem.Text.Length == 1))
                {
                    this.ParentControl.intellisenseBox.SelectedItem = tempItem;
                    this.ParentControl.intellisenseBox.ScrollIntoView(this.ParentControl.intellisenseBox.SelectedItem);
                }
                else
                {
                    this.ParentControl.intellisenseBox.ScrollIntoView(tempItem);
                }
            }
            else
            {
                this.ParentControl.intellisenseBox.SelectedItem = null;
            }

            return indices.Count();
        }

        internal IIntellisenseItem GetMatchingIntellisenseItem(string intellisenseStartString)
        {
            IEnumerable<IIntellisenseItem> collection = null;
            if (this.ParentControl.intellisenseBox.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ParentControl.intellisenseBox.ItemsSource);
                collection = view.SourceCollection.OfType<IIntellisenseItem>();
            }

            if (collection == null)
            {
                return null;
            }
            var indices = collection.Where(item => item.Text.ToLower() == intellisenseStartString.ToLower());
            if (indices.Count() > 0)
            {
                IIntellisenseItem tempItem = indices.ElementAt(0);
                return tempItem;
            }
            else
            {
                return collection.ElementAt(0);
            }
        }

        internal IIntellisenseItem GetItemStartingWith(string intellisenseStartString)
        {
            IEnumerable<IIntellisenseItem> collection = null;
            if (this.ParentControl.intellisenseBox.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ParentControl.intellisenseBox.ItemsSource);
                collection = view.SourceCollection.OfType<IIntellisenseItem>();
            }

            if (collection == null)
            {
                return null;
            }
            var indices = collection.Where(item => item.Text.ToLower().StartsWith(intellisenseStartString.ToLower()));
            if (indices.Count() > 0)
            {
                IIntellisenseItem tempItem = indices.ElementAt(0);
                return tempItem;
            }
            else
            {
                return collection.ElementAt(0);
            }
        }

        /// <summary>
        /// Helper method to check if the line content contains Namespace declaration.
        /// </summary>
        /// <param name="scope">represents the ScopeDefinition of the line</param>
        /// <param name="leadingText">represents the leading text from the current location</param>
        /// <returns>whether the text contains the namespace declaration or not</returns>
        internal bool LineContainsNamespaceDeclaration(ScopeDefinition scope, string leadingText)
        {
            var namespaceTerms = this.GetMatchingNamespaceDeclarationLexems(scope, leadingText);
            if (namespaceTerms != null && namespaceTerms.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper method to get NamespaceDeclaration type of ILexem from Lexem property.
        /// </summary>
        /// <param name="scope">represents the ScopeDefinition of the line</param>
        /// <param name="leadingText">represents the leading text from the current location</param>
        /// <returns>a collection of ILexem object matching the namespace declaration text if any</returns>
        internal IEnumerable<ILexem> GetMatchingNamespaceDeclarationLexems(ScopeDefinition scope, string leadingText)
        {
            if ((scope == null || (scope != null && scope.Type == ScopeLevel.Namespace)) && this.Lexem.OfType<ILexem>().Count() > 0)
            {
                if (leadingText.Trim().Length > 0 && this.Lexem != null && this.Lexem.OfType<ILexem>().Count() > 0)
                {
                    var namespaceTerms = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.NamespaceDeclaration && ((lex.IsRegex && Regex.Match(leadingText, lex.StartText).Success) || (!lex.IsRegex && leadingText.Trim().StartsWith(lex.StartText))));
                    return namespaceTerms;
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method to get the namespace ScopeDefinition of specified line
        /// </summary>
        /// <param name="line">represents the line number</param>
        /// <returns>a value indicating the ScopeDefinition of the specified line</returns>
        internal ScopeDefinition GetNamespaceScopeDefinition(int line)
        {
            var scope = this.scopeDefinitions.Where(scp => scp.StartLine <= line && scp.Type == ScopeLevel.Namespace).OrderByDescending(scp => scp.StartLine);
            if (scope.Count() > 0)
            {
                return scope.ElementAt(0);
            }
            return null;
        }

        internal virtual void OnDrillDownIntellisense(EditIntellisenseArgs args)
        {
            if (args.SelectedItem == null && args.CursorIndex > 0)
            {
                string tempString = GetPreviousString(args.CursorIndex);
                if (tempString != string.Empty)
                {
                    int ind = tempString.LastIndexOf(this.IntellisenseDrillDownChar);
                    IIntellisenseItem selectedItem = null;

                    if (this.ParentControl.IntellisenseMode == IntellisenseMode.Custom)
                    {
                        if (this.ParentControl.IntellisenseCustomItemsSource != null)
                        {
                            selectedItem = this.GetTypeFromString(new EditTypeCollection(this.ParentControl.IntellisenseCustomItemsSource.OfType<IIntellisenseItem>()), tempString.TrimEnd(this.IntellisenseDrillDownChar));
                        }
                    }
                    else
                    {
                        selectedItem = this.GetTypeFromString(this.TypesCollection, tempString);
                    }

                    if (selectedItem == null)
                    {
                        selectedItem = this.GetTypeFromString(this.instancesCollection, tempString);
                    }

                    if (selectedItem != null)
                    {
                        args.SelectedItem = selectedItem;
                    }
                }
            }

            if (args.SelectedItem != null)
            {
                IIntellisenseItem selectedItem = args.SelectedItem;
                args.ItemsSource = selectedItem.NestedItems;
                this.ParentControl.RaiseIntellisenseDrillDownEvent(args);
                if (!args.Cancel && args.ItemsSource != null)
                {
                    this.ParentControl.intellisenseBox.ItemsSource = args.ItemsSource;
                    this.PositionIntellisenseBox(args);
                }
            }
        }

        internal string GetPreviousString(int cursorindex)
        {
            string tempstring = string.Empty;
            WordDetails word = this.ParentControl.ScrollControl.GetCurrentWord(cursorindex - 1);
            char[] commitChars = this.IntellisenseCommitCharacters.ToCharArray();
            if (word != null)
            {
                var text = this.ParentControl.ScrollControl.CurrentLineItem.Text;
                tempstring = word.Text;
                if (word.StartIndex > 1 && text.Substring(word.StartIndex - 1, 1) == this.IntellisenseDrillDownChar.ToString())
                {
                    var temp = GetPreviousString(word.StartIndex - 2);
                    if (temp != string.Empty)
                    {
                        tempstring = temp + this.IntellisenseDrillDownChar.ToString() + tempstring;
                    }
                }
            }

            return tempstring;
        }

        /// <summary>
        /// Helper method to iterate to the types collection tree and find the type given. This is a recursive method that splits the text based with "." and iterates to returns the nested types under the type name.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="typeName"></param>
        /// <returns>a collection of EditTypeInfo objects under the type name</returns>
        internal IEnumerable<IIntellisenseItem> GetTypesFromString(EditTypeCollection collection, string typeName)
        {
            if (typeName.Trim() == string.Empty || collection == null)
            {
                return collection;
            }

            string tempStr = typeName;
            string remaingStr = string.Empty;
            if (typeName.IndexOf(this.IntellisenseDrillDownChar) > 0)
            {
                tempStr = typeName.Substring(0, typeName.IndexOf(this.IntellisenseDrillDownChar)).TrimEnd(this.IntellisenseDrillDownChar).TrimStart(this.IntellisenseDrillDownChar);
                remaingStr = typeName.Substring(Math.Min(typeName.IndexOf(this.IntellisenseDrillDownChar) + 1, typeName.Length));
            }

            EditTypeInfo tempItem = collection.GetEditTypeInfo(tempStr) as EditTypeInfo;
            if (tempItem != null && remaingStr != string.Empty && tempItem.NestedItems != null)
            {
                return GetTypesFromString(tempItem.NestedItems as EditTypeCollection, remaingStr);
            }
            else if (tempItem != null)
            {
                return tempItem.NestedItems;
            }

            return null;
        }

        /// <summary>
        /// Helper method to iterate to the types collection tree and find the type given. This is a recursive method that splits the text based with "." and iterates to returns the nested types under the type name.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="typeName"></param>
        /// <returns>a collection of EditTypeInfo objects under the type name</returns>
        internal IIntellisenseItem GetTypeFromString(EditTypeCollection collection, string typeName)
        {
            if (typeName.Trim() == string.Empty || collection == null)
            {
                return null;
            }

            string tempStr = typeName;
            string remaingStr = string.Empty;
            if (typeName.IndexOf(this.IntellisenseDrillDownChar) > 0)
            {
                tempStr = typeName.Substring(0, typeName.IndexOf(this.IntellisenseDrillDownChar)).TrimEnd(this.IntellisenseDrillDownChar).TrimStart(this.IntellisenseDrillDownChar);
                remaingStr = typeName.Substring(Math.Min(typeName.IndexOf(this.IntellisenseDrillDownChar) + 1, typeName.Length));
            }

            IIntellisenseItem tempItem = collection.GetEditTypeInfo(tempStr);
            if (tempItem != null && remaingStr != string.Empty && tempItem.NestedItems != null)
            {
                return GetTypeFromString(new EditTypeCollection(tempItem.NestedItems), remaingStr);
            }
            else if (tempItem != null && tempItem.Text == typeName)
            {
                return tempItem;
            }
            else if (!this.CaseSensitive && tempItem != null && tempItem.Text.ToLower() == typeName.ToLower())
            {
                return tempItem;
            }

            return null;
        }

        /// <summary>
        /// Helper method to get an EditTypeInfo instances from TypesCollection based on the full name of the type.
        /// </summary>
        /// <param name="typeName">represents full name of the type including namespace</param>
        /// <returns>EditTypeInfo instance from the types collection.</returns>
        internal IIntellisenseItem GetBaseTypeFromFullName(string typeName)
        {
            if (typeName == null)
                return null;

            IEnumerable<IIntellisenseItem> types = this.GetTypesFromString(this.TypesCollection, typeName);
            if (types != null && types.Count() > 0)
            {
                var getItems = types.Where(item => (item as EditTypeInfo).Name == typeName);
                if (getItems.Count() > 0)
                {
                    return getItems.ElementAt(0);
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method to get an EditTypeInfo instances from TypesCollection based on the name of the type.
        /// </summary>
        /// <param name="typeName">represents full name of the type without namespace</param>
        /// <returns>EditTypeInfo instance from the types collection.</returns>
        internal IIntellisenseItem GetBaseType(string typeName)
        {
            if (includedNamespaces != null && includedNamespaces.Count > 0)
            {
                foreach (string str in includedNamespaces)
                {
                    IEnumerable<IIntellisenseItem> types = this.GetTypesFromString(this.TypesCollection, str);
                    if (types != null && types.Count() > 0)
                    {
                        var getItems = types.Where(item => (item as EditTypeInfo).Name == typeName);
                        if (getItems.Count() > 0)
                        {
                            return getItems.ElementAt(0);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Method to Hide Intellisense Popup.
        /// </summary>
        public void HideIntellisensePopup()
        {
            this.ParentControl.intellisensePopup.IsOpen = false;
            this.ParentControl.isIntellisenseBoxOpen = false;
            this.ParentControl.intellisenseFilterString = string.Empty;
            this.ParentControl.intellisenseBox.ItemsSource = null;
            this.ParentControl.intellisensePopup.StaysOpen = true;
        }

        /// <summary>
        /// Helper method to load generate types in namespace tree.
        /// </summary>
        /// <param name="assemblies">represents the list of assemblies added</param>
        internal void UpdateTypesForIncludedNamespaces(IEnumerable<Uri> assemblies)
        {
            if (assemblies == null)
                return;
            List<object> parameters = new List<object>();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                List<object> arguments = args.Argument as List<object>;
                IEnumerable<Uri> uriList = arguments[0] as IEnumerable<Uri>;
                EditTypeCollection infoList = arguments[1] as EditTypeCollection;
                List<string> includedNS = arguments[2] as List<string>;
                if (infoList == null)
                {
                    infoList = new EditTypeCollection();
                }
                if (includedNS != null)
                {
                    foreach (Uri uri in uriList)
                    {
                        Assembly assembly = Assembly.LoadFile(uri.LocalPath);
                        var types = assembly.GetTypes().Where(type => type.IsPublic).GroupBy(type => type.Namespace);
                        foreach (var item in types)
                        {
                            if (includedNS.Contains(item.Key))
                            {
                                foreach (Type type in item)
                                {
                                    UpdateEditTypeInfoList(infoList, type.FullName, type);
                                }
                            }
                        }
                    }
                }
                args.Result = infoList;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                var Collection = args.Result as EditTypeCollection;
                if (Collection != null)
                {
                    this.typesCollection = new EditTypeCollection(Collection);
                }
            };
            parameters.Add(assemblies);
            parameters.Add(this.typesCollection);
            parameters.Add(this.includedNamespaces);
            worker.RunWorkerAsync(parameters);
        }

        private void UpdateEditTypeInfoList(EditTypeCollection infoList, string typeName, Type type)
        {
            if (typeName.IndexOf(this.IntellisenseDrillDownChar) > 0)
            {
                string tempStr = typeName.Substring(0, typeName.IndexOf(this.IntellisenseDrillDownChar)).TrimEnd(this.IntellisenseDrillDownChar).TrimStart(this.IntellisenseDrillDownChar);
                string remaingStr = typeName.Substring(Math.Min(typeName.IndexOf(this.IntellisenseDrillDownChar) + 1, typeName.Length));
                EditTypeInfo tempItem = infoList.GetEditTypeInfo(tempStr) as EditTypeInfo;
                if (tempItem == null)
                {
                    tempItem = new EditTypeInfo()
                    {
                        Name = type.Name,
                        IsClass = type.IsClass,
                        IsInterface = type.IsInterface,
                        Namespace = type.Namespace,
                        IsEnum = type.IsEnum,
                        IsProperty = type.IsAbstract,
                        IsEvent = type.IsSealed,
                        BaseType = type.BaseType != null ? this.GetBaseTypeFromFullName(type.BaseType.FullName) as EditTypeInfo : null,
                    };
                    if (remaingStr != string.Empty)
                    {
                        tempItem.NestedItems = new EditTypeCollection();
                        this.UpdateEditTypeInfoList(tempItem.NestedItems as EditTypeCollection, remaingStr, type);
                    }
                    infoList.Add(tempItem);
                }
                else
                {
                    if (tempItem.NestedItems == null)
                    {
                        tempItem.NestedItems = new EditTypeCollection();
                    }
                    this.UpdateEditTypeInfoList(tempItem.NestedItems as EditTypeCollection, remaingStr, type);
                }
            }
            else if (typeName.Trim().Length > 0)
            {
                EditTypeInfo tempItem = infoList.GetEditTypeInfo(typeName) as EditTypeInfo;
                if (tempItem == null)
                {
                    tempItem = new EditTypeInfo()
                    {
                        Name = type.Name,
                        IsClass = type.IsClass,
                        IsInterface = type.IsInterface,
                        Namespace = type.Namespace,
                        IsEnum = type.IsEnum,
                        IsProperty = type.IsAbstract,
                        IsEvent = type.IsSealed,
                        BaseType = type.BaseType != null ? this.GetBaseTypeFromFullName(type.BaseType.FullName) as EditTypeInfo : null
                    };
                    infoList.Add(tempItem);
                }
            }
        }

        #endregion Intellisense

        #region Delete and Backspace Codes

        /// <summary>
        /// Helper method for delete key activity
        /// </summary>
        protected internal virtual void ExecuteDeleteText()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000d);
                timer.Tick += new EventHandler(timer_Tick);
            }
            timer.Stop();

            string remtext = string.Empty;
            if (this.ParentControl.SelectedText != string.Empty && this.ParentControl.ScrollControl.TextSelectionPointer != null)
            {
                remtext = this.ParentControl.SelectedText;
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Delete,
                    LineNumber = this.ParentControl.ScrollControl.TextSelectionPointer.StartLine + 1,
                    CursorIndex = this.ParentControl.CursorIndex,
                    Text = remtext,
                    IsSelected = true,
                    SelectedText = remtext,
                    Pointer = this.ParentControl.ScrollControl.TextSelectionPointer,
                    IsSelectAll = this.ParentControl.ScrollControl.isSelectedAll
                });

                int index = this.ParentControl.ScrollControl.RemoveSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(index);
                }
                return;
            }

            if (this.ParentControl.Lines[this.ParentControl.LineNumber - 1].IsExpanded || (!this.ParentControl.Lines[this.ParentControl.LineNumber - 1].IsExpanded && !this.ParentControl.Lines[this.ParentControl.LineNumber - 1].ContainsPreprocessor && this.ParentControl.CursorIndex < this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text.Length))
            {
                if (this.ParentControl.CursorIndex == this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text.Length)
                {
                    if (this.ParentControl.Lines.Count > this.ParentControl.LineNumber)
                    {
                        string tempText = this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text;
                        string txt = this.ParentControl.Lines[this.ParentControl.LineNumber].Text;
                        if (!this.ParentControl.Lines[this.ParentControl.LineNumber].IsExpanded && tempText.Trim() != string.Empty)
                        {
                            this.ParentControl.ExpandLine(this.ParentControl.LineNumber);
                        }
                        else if (!this.ParentControl.Lines[this.ParentControl.LineNumber].IsExpanded && tempText.Trim() == string.Empty)
                        {
                            var nextItem = this.ParentControl.Lines[this.ParentControl.LineNumber];
                            var expandInfo = nextItem.GetLineItemExpandDetails();
                            var currentItem = this.ParentControl.Lines[this.ParentControl.LineNumber - 1];
                            currentItem.CopyExpandDetails(nextItem);
                            currentItem.StartLine = expandInfo.StartLine - 1;
                            currentItem.EndLine = expandInfo.EndLine - 1;
                            var listener = nextItem.GetPreprocessorType();
                            if (listener != null)
                            {
                                this.DocumentBlocks.Remove(listener);
                                listener.ParentLineNumber -= 1;
                                this.DocumentBlocks.Add(listener);
                            }
                        }
                        else if (this.ParentControl.Lines[this.ParentControl.LineNumber].IsExpanded && this.ParentControl.Lines[this.ParentControl.LineNumber].ContainsLines)
                        {
                            this.ParentControl.Lines[this.ParentControl.LineNumber].ContainsLines = false;
                            this.ParentControl.Lines[this.ParentControl.LineNumber - 1].ContainsLines = false;
                        }
                        this.ParentControl.ScrollControl.rowheights.RemoveLines(this.ParentControl.LineNumber, 1, null);
                        this.ParentControl.Lines.RemoveAt(this.ParentControl.LineNumber);

                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = this.ParentControl.LineNumber,
                            CursorIndex = this.ParentControl.CursorIndex,
                            Text = remtext,
                            NewLine = true
                        });

                        this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text = this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text + txt;
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(0);
                        double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                        if (x > this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - this.ParentControl.ScrollControl.HScrollBar.SmallChange);
                        }
                        timer.Start();
                    }
                }
                else
                {
                    if (this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text != string.Empty)
                    {
                        var lineItem = this.ParentControl.Lines[this.ParentControl.LineNumber - 1];
                        remtext = lineItem.Text.Substring(this.ParentControl.CursorIndex, 1);
                        lineItem.Text = lineItem.Text.Remove(this.ParentControl.CursorIndex, 1);
                        lineItem.TextWidth = Utils.GetWidth(lineItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);

                        if (this.ParentControl.ScrollControl.Caret == null)
                        {
                            lineItem.SetCursorOnLoad = true;
                            lineItem.SetCursorIndex = this.ParentControl.CursorIndex;
                            this.ParentControl.ScrollControl.SetVerticalOffset(this.ParentControl.LineNumber - 1);
                        }
                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = this.ParentControl.LineNumber,
                            CursorIndex = this.ParentControl.CursorIndex,
                            Text = remtext
                        });
                        timer.Start();
                    }
                }
            }
            else
            {
                var lineItem = this.ParentControl.Lines[this.ParentControl.LineNumber - 1];
                if (lineItem.ContainsPreprocessor)
                {
                    BlockListener listener = lineItem.GetPreprocessorType();
                    int startIndex = lineItem.Text.IndexOf(listener.BlockStart);
                    if (this.ParentControl.CursorIndex < startIndex)
                    {
                        remtext = lineItem.Text.Substring(this.ParentControl.CursorIndex, 1);
                        lineItem.Text = lineItem.Text.Remove(this.ParentControl.CursorIndex, 1);
                        lineItem.TextWidth = Utils.GetWidth(lineItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);

                        if (this.ParentControl.ScrollControl.Caret == null)
                        {
                            lineItem.SetCursorOnLoad = true;
                            lineItem.SetCursorIndex = this.ParentControl.CursorIndex;
                            this.ParentControl.ScrollControl.SetVerticalOffset(this.ParentControl.LineNumber - 1);
                        }
                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = this.ParentControl.LineNumber,
                            CursorIndex = this.ParentControl.CursorIndex,
                            Text = remtext
                        });
                        timer.Start();
                    }
                    else if (this.ParentControl.CursorIndex == startIndex)
                    {
                        SelectionPointer tempPointer = new SelectionPointer();
                        tempPointer.StartLine = this.ParentControl.LineNumber - 1;
                        tempPointer.EndLine = lineItem.EndLine - 1;
                        tempPointer.StartIndex = startIndex;
                        tempPointer.EndIndex = this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
                        remtext = lineItem.Text.Substring(startIndex);
                        remtext += this.ParentControl.GetTextRange(tempPointer.StartLine + 1, lineItem.EndLine);
                        int cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(tempPointer);
                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = tempPointer.StartLine + 1,
                            CursorIndex = this.ParentControl.CursorIndex,
                            Text = remtext,
                            IsSelected = true,
                            SelectedText = remtext,
                            Pointer = tempPointer,
                        });

                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(cursorindex);
                        }
                        timer.Start();
                    }
                    else if (this.ParentControl.CursorIndex == this.GetCollapsedItemText(lineItem).Length)
                    {
                        if (lineItem.EndLine < this.ParentControl.Lines.Count)
                        {
                            string txt = this.ParentControl.Lines[lineItem.EndLine].Text;
                            if (!this.ParentControl.Lines[lineItem.EndLine].IsExpanded)
                            {
                                this.ParentControl.ExpandLine(lineItem.EndLine);
                            }

                            int ind = this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
                            this.ParentControl.ScrollControl.rowheights.RemoveLines(lineItem.EndLine, 1, null);
                            this.ParentControl.Lines.RemoveAt(lineItem.EndLine);
                            this.ParentControl.UndoManager.IsNewUndoItem = true;
                            this.ParentControl.UndoManager.Add(new EditAction()
                            {
                                Action = ActionType.Delete,
                                LineNumber = lineItem.EndLine,
                                CursorIndex = ind,
                                Text = remtext,
                                NewLine = true
                            });

                            if (txt.Trim() != string.Empty)
                            {
                                this.ParentControl.ExpandLine(lineItem.EndLine);
                            }

                            this.ParentControl.Lines[lineItem.EndLine - 1].Text = this.ParentControl.Lines[lineItem.EndLine - 1].Text + txt;
                            this.ParentControl.ExpandLine(this.ParentControl.Lines.IndexOf(lineItem));
                            this.ParentControl.ScrollControl.ScrollRows.ScrollInView(lineItem.EndLine - 1);
                            this.ParentControl.ScrollControl.MoveCursorToLineItem(lineItem.EndLine - 1);
                            if (this.ParentControl.ScrollControl.Caret != null)
                            {
                                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(0);
                                double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                                if (x > this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth)
                                {
                                    this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - this.ParentControl.ScrollControl.HScrollBar.SmallChange);
                                }
                            }
                            else
                            {
                                this.ParentControl.Lines[lineItem.EndLine - 1].SetCursorIndex = ind;
                                this.ParentControl.Lines[lineItem.EndLine - 1].SetCursorOnLoad = true;
                            }
                            timer.Start();
                        }
                    }
                }
                else
                {
                    string collapsedText = this.GetCollapsedItemText(lineItem);
                    int ellipsisIndex = collapsedText.IndexOf(this.EllipsisText);
                    if (this.ParentControl.CursorIndex == lineItem.Text.Length)
                    {
                        SelectionPointer tempPointer = new SelectionPointer();
                        tempPointer.StartLine = this.ParentControl.LineNumber - 1;
                        tempPointer.EndLine = lineItem.EndLine - 1;
                        tempPointer.StartIndex = this.ParentControl.CursorIndex;
                        tempPointer.EndIndex = this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
                        remtext = Environment.NewLine + this.ParentControl.GetTextRange(this.ParentControl.LineNumber, lineItem.EndLine);
                        int cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(tempPointer);
                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = tempPointer.StartLine + 1,
                            CursorIndex = this.ParentControl.CursorIndex,
                            Text = remtext,
                            IsSelected = true,
                            SelectedText = remtext,
                            Pointer = tempPointer,
                        });

                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(cursorindex);
                        }
                        timer.Start();
                    }
                    else if (this.ParentControl.CursorIndex == ellipsisIndex)
                    {
                        this.DeleteTextAfterEllipsis(lineItem);
                        timer.Start();
                    }
                    else
                    {
                        if (lineItem.EndLine < this.ParentControl.Lines.Count)
                        {
                            string txt = this.ParentControl.Lines[lineItem.EndLine].Text;
                            if (!this.ParentControl.Lines[lineItem.EndLine].IsExpanded)
                            {
                                this.ParentControl.ExpandLine(lineItem.EndLine);
                            }
                            int ind = this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
                            this.ParentControl.ScrollControl.rowheights.RemoveLines(lineItem.EndLine, 1, null);
                            this.ParentControl.Lines.RemoveAt(lineItem.EndLine);
                            this.ParentControl.UndoManager.IsNewUndoItem = true;
                            this.ParentControl.UndoManager.Add(new EditAction()
                            {
                                Action = ActionType.Delete,
                                LineNumber = lineItem.EndLine,
                                CursorIndex = ind,
                                Text = remtext,
                                NewLine = true
                            });

                            if (txt.Trim() != string.Empty)
                            {
                                this.ParentControl.ExpandLine(this.ParentControl.Lines.IndexOf(lineItem));
                            }

                            this.ParentControl.Lines[lineItem.EndLine - 1].Text = this.ParentControl.Lines[lineItem.EndLine - 1].Text + txt;
                            this.ParentControl.ScrollControl.ScrollRows.ScrollInView(lineItem.EndLine - 1);
                            this.ParentControl.ScrollControl.MoveCursorToLineItem(lineItem.EndLine - 1);
                            if (this.ParentControl.ScrollControl.Caret != null)
                            {
                                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(0);
                                double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                                if (x > this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth)
                                {
                                    this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - this.ParentControl.ScrollControl.HScrollBar.SmallChange);
                                }
                            }
                            else
                            {
                                this.ParentControl.Lines[lineItem.EndLine - 1].SetCursorIndex = ind;
                                this.ParentControl.Lines[lineItem.EndLine - 1].SetCursorOnLoad = true;
                            }
                            timer.Start();
                        }
                    }
                }
            }

            this.ParentControl.isReinitializeLines = false;
            this.ParentControl.Text = this.ParentControl.GetText();
            this.ParentControl.isReinitializeLines = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timerValue += 1;
            if (timerValue == 3)
            {
                this.ApplyExpandItems();
                timerValue = 0;
                timer.Stop();
            }
        }

        private void DeleteTextAfterEllipsis(LineItem lineItem)
        {
            string collapsedItemText = this.GetCollapsedItemText(lineItem);
            int ellipsisIndex = collapsedItemText.IndexOf(this.EllipsisText);
            int selectionEndIndex = GetCollapsedItemSelectionEndIndex(lineItem, lineItem.Text.Length);
            string remtext = string.Empty;
            SelectionPointer tempPointer = new SelectionPointer();
            tempPointer.StartLine = this.ParentControl.LineNumber - 1;
            tempPointer.EndLine = lineItem.EndLine - 1;
            tempPointer.StartIndex = ellipsisIndex >= 0 ? ellipsisIndex : lineItem.Text.Length;
            tempPointer.EndIndex = selectionEndIndex >= 0 ? selectionEndIndex : this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
            remtext = Environment.NewLine + this.ParentControl.GetTextRange(this.ParentControl.LineNumber, lineItem.EndLine - 1);
            this.ParentControl.UndoManager.IsNewUndoItem = true;
            this.ParentControl.UndoManager.Add(new EditAction()
            {
                Action = ActionType.Delete,
                LineNumber = this.ParentControl.LineNumber,
                CursorIndex = this.ParentControl.CursorIndex,
                Text = remtext,
                IsSelected = true,
                SelectedText = remtext,
                Pointer = tempPointer,
            });

            int index = this.ParentControl.ScrollControl.RemoveSelectedText(tempPointer);
            if (this.ParentControl.ScrollControl.Caret != null)
            {
                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(index);
            }
        }

        /// <summary>
        /// Helper method for backspace key activity
        /// </summary>
        protected internal virtual void ExecuteBackspace()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000d);
                timer.Tick += new EventHandler(timer_Tick);
            }

            timer.Stop();
            string remtext = string.Empty;
            int startLine = this.ParentControl.LineNumber - 1;
            LineItem lineItem = this.ParentControl.Lines[startLine];
            if (this.ParentControl.SelectedText != string.Empty && this.ParentControl.ScrollControl.TextSelectionPointer != null)
            {
                remtext = this.ParentControl.SelectedText;
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                EditAction action = new EditAction();
                action.Action = ActionType.Backspace;
                action.LineNumber = this.ParentControl.ScrollControl.TextSelectionPointer.StartLine + 1;
                action.CursorIndex = this.ParentControl.CursorIndex;
                action.Text = remtext;
                action.SelectedText = remtext;
                action.Pointer = this.ParentControl.ScrollControl.TextSelectionPointer;
                action.IsSelectAll = this.ParentControl.ScrollControl.isSelectedAll;
                action.IsSelected = true;
                this.ParentControl.UndoManager.Add(action);

                int index = this.ParentControl.ScrollControl.RemoveSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(index);
                }
                this.ParentControl.isReinitializeLines = false;
                this.ParentControl.Text = this.ParentControl.GetText();
                this.ParentControl.isReinitializeLines = true;
                this.ApplyExpandItems();
                return;
            }

            string collapsedItemText = this.GetCollapsedItemText(lineItem);
            int ellipsisIndex = collapsedItemText.IndexOf(this.EllipsisText);
            int cursorIndex = this.ParentControl.CursorIndex;

            BlockListener listener = lineItem.ContainsPreprocessor ? lineItem.GetPreprocessorType() : null;

            if (lineItem.IsExpanded || (!lineItem.IsExpanded && lineItem.ContainsPreprocessor && cursorIndex <= lineItem.Text.IndexOf(listener.BlockStart)) || (!lineItem.IsExpanded && !lineItem.ContainsPreprocessor && cursorIndex != ellipsisIndex + EllipsisText.Length && cursorIndex < collapsedItemText.Length))
            {
                #region Normal Backspace Activity

                if (cursorIndex > 0)
                {
                    if (lineItem.Text != string.Empty)
                    {
                        if (this.ParentControl.isIntellisenseBoxOpen)
                        {
                            string removableText = lineItem.Text.Substring(cursorIndex - 1, 1);
                            if (removableText == this.ParentControl.CurrentLanguage.IntellisenseDrillDownChar.ToString() || removableText.Trim() == string.Empty)
                            {
                                this.ParentControl.CurrentLanguage.HideIntellisensePopup();
                                this.ParentControl.intellisenseBox.SelectedItem = null;
                            }
                            else
                            {
                                WordDetails word = this.ParentControl.ScrollControl.GetCurrentWord(cursorIndex - 1);
                                if (word != null && word.Text.Length > 1)
                                {
                                    this.ApplyFilterToIntellisense(word.Text.Substring(0, word.Text.Length - 1));
                                }
                            }
                        }

                        remtext = lineItem.Text.Substring(Math.Min(cursorIndex - 1, lineItem.Text.Length), 1);
                        lineItem.Text = lineItem.Text.Remove(Math.Min(cursorIndex - 1, lineItem.Text.Length), 1);
                        lineItem.TextWidth = Utils.GetWidth(lineItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(-1);
                        if (this.ParentControl.ScrollControl.Caret.CaretPosition.X > this.ParentControl.ScrollControl.ViewportWidth && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.HScrollBar.SmallChange && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.ExtentWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - (this.ParentControl.ScrollControl.HScrollBar.SmallChange * 2));
                        }
                        else if (this.ParentControl.ScrollControl.HorizontalOffset > 0 && this.ParentControl.ScrollControl.Caret.CaretPosition.X < this.ParentControl.ScrollControl.ViewportWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(0);
                        }

                        if (this.ParentControl.isIntellisenseBoxOpen && lineItem.Text.Trim() == string.Empty)
                        {
                            this.ParentControl.CurrentLanguage.HideIntellisensePopup();
                        }

                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Backspace,
                            LineNumber = this.ParentControl.LineNumber,
                            CursorIndex = cursorIndex - 1,
                            Text = remtext
                        });
                        timer.Start();
                    }
                }
                else
                {
                    int tempind = 0;
                    string tempTxt = string.Empty;
                    if (this.ParentControl.LineNumber > 1)
                    {
                        LineItem prevItem = this.ParentControl.Lines[startLine - 1];
                        if (prevItem.ParentLineNumber > 0)
                        {
                            LineItem prevItemParent = this.ParentControl.Lines[prevItem.ParentLineNumber - 1];
                            if (!prevItemParent.IsExpanded)
                            {
                                this.ParentControl.ExpandLine(prevItem.ParentLineNumber - 1);
                                this.ParentControl.ScrollControl.ScrollRows.ScrollInView(startLine - 1);
                            }
                        }

                        if (!lineItem.IsExpanded && tempTxt.Trim() != string.Empty)
                        {
                            this.ParentControl.ExpandLine(startLine);
                        }
                        else if (!lineItem.IsExpanded)
                        {
                            var expandInfo = lineItem.GetLineItemExpandDetails();
                            if (expandInfo != null)
                            {
                                var item = this.ParentControl.Lines[startLine - 1];
                                item.CopyExpandDetails(lineItem);
                                item.StartLine = expandInfo.StartLine - 1;
                                item.EndLine = expandInfo.EndLine - 1;
                                item.ContainsLines = true;
                                item.IsExpanded = false;
                                var linelistener = lineItem.GetPreprocessorType();
                                if (listener != null)
                                {
                                    this.DocumentBlocks.Remove(linelistener);
                                    linelistener.ParentLineNumber -= 1;
                                    this.DocumentBlocks.Add(linelistener);
                                }
                            }
                        }

                        if (lineItem.Text.Length > 0)
                        {
                            tempind = this.ParentControl.Lines[startLine - 1].Text.Length;
                            tempTxt = this.ParentControl.Lines[startLine - 1].Text;
                            this.ParentControl.Lines[startLine - 1].Text = this.ParentControl.Lines[startLine - 1].Text + lineItem.Text;
                        }
                        else
                        {
                            tempind = this.ParentControl.Lines[startLine - 1].Text.Length;
                        }

                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Backspace,
                            LineNumber = startLine,
                            CursorIndex = tempind,
                            Text = remtext,
                            NewLine = true
                        });
                        this.ParentControl.ScrollControl.rowheights.RemoveLines(startLine, 1, null);

                        this.ParentControl.Lines.RemoveAt(startLine);
                        this.ParentControl.ScrollControl.MoveCursorToLineItem(startLine - 1);
                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(tempind);
                            double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                            if (x > (this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth) || (x < this.ParentControl.ScrollControl.HorizontalOffset))
                            {
                                this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X);
                            }
                        }
                        else
                        {
                            this.ParentControl.Lines[startLine - 1].SetCursorIndex = tempind;
                            this.ParentControl.Lines[startLine - 1].SetCursorOnLoad = true;
                        }
                        this.ApplyExpandItems();
                    }
                }

                #endregion Normal Backspace Activity
            }
            else
            {
                SelectionPointer tempPointer = new SelectionPointer();
                tempPointer.StartLine = startLine;
                tempPointer.EndLine = lineItem.EndLine - 1;
                if (lineItem.ContainsPreprocessor)
                {
                    tempPointer.StartIndex = lineItem.Text.IndexOf(listener.BlockStart);
                    tempPointer.EndIndex = GetCollapsedItemSelectionEndIndex(lineItem, lineItem.Text.Length);
                }
                else
                {
                    tempPointer.StartIndex = ellipsisIndex;
                    tempPointer.EndIndex = GetCollapsedItemSelectionEndIndex(lineItem, lineItem.Text.Length);
                }
                remtext = this.GetTextInCollapsedArea(lineItem);
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.Backspace,
                    LineNumber = startLine + 1,
                    CursorIndex = this.ParentControl.CursorIndex,
                    Text = remtext,
                    IsSelected = true,
                    SelectedText = remtext,
                    Pointer = tempPointer,
                });

                int index = this.ParentControl.ScrollControl.RemoveSelectedText(tempPointer);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(index);
                }
                timer.Start();
            }

            this.ParentControl.isReinitializeLines = false;
            this.ParentControl.Text = this.ParentControl.GetText();
            this.ParentControl.isReinitializeLines = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        protected internal virtual string GetTextInCollapsedArea(LineItem lineItem)
        {
            StringBuilder builder = new StringBuilder();
            if (lineItem.ContainsPreprocessor)
            {
                BlockListener listener = lineItem.GetPreprocessorType();
                if (listener != null)
                {
                    int startIndex = lineItem.Text.IndexOf(listener.BlockStart);
                    builder.Append(lineItem.Text.Substring(Math.Max(startIndex, 0)));
                    builder.Append(Environment.NewLine);
                }
                builder.Append(this.ParentControl.GetTextRange(lineItem.StartLine, lineItem.EndLine - 1));
            }
            else
            {
                builder.Append(Environment.NewLine);
                builder.Append(this.ParentControl.GetTextRange(lineItem.StartLine - 1, lineItem.EndLine - 1));
            }
            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineitem"></param>
        /// <returns></returns>
        protected internal virtual int GetSelectionEndIndex(LineItem lineitem)
        {
            if (lineitem.ContainsLines)
            {
                return this.ParentControl.Lines[lineitem.EndLine - 1].Text.Length;
            }
            return lineitem.Text.Length;
        }

        #endregion Delete and Backspace Codes

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        protected internal virtual int GetIndentLevel(int lineNumber)
        {
            if (!this.SupportsOutlining || this.IndentableBlocks == null)
                return 0;

            var indentableLines = this.IndentableBlocks.Where(line => ((line.IgnoreEndBlock && line.ParentLineNumber <= lineNumber) || (!line.IgnoreEndBlock && line.ParentLineNumber < lineNumber)) && ((line.IgnoreEndBlock && line.EndLineNumber >= lineNumber + 1) || (!line.IgnoreEndBlock && line.EndLineNumber > lineNumber + 1)));
            return indentableLines.Count();
        }

        internal virtual bool CheckBlockStarts(LineItem tempLineItem)
        {
            return false;
        }

        internal virtual bool CheckBlockEnds(LineItem tempLineItem)
        {
            return false;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ScopeDefinition
    {
        /// <summary>
        ///
        /// </summary>
        public ScopeLevel Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int EndLine
        {
            get
            {
                if (StartItem != null)
                {
                    return StartItem.EndLine;
                }
                return 0;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public LineItemExpandInformation StartItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ScopeDefinition()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ScopeName;
        }
    }
}