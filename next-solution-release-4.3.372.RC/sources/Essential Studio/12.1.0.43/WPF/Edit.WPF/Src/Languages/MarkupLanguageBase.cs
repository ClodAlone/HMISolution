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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public abstract class MarkupLanguageBase : LanguageBase
    {
        #region Local Variables

        /// <summary>
        /// Local variable of SyntaxColoringDetails to store current color information
        /// </summary>
        private SyntaxColoringDetails coloringDetails;

        /// <summary>
        /// Private SyntaxColoringDetails object for get the patterns in stack.
        /// </summary>
        private Stack<SyntaxColoringDetails> patternStack = new Stack<SyntaxColoringDetails>();

        /// <summary>
        /// Private IFormat object for get the Formats in stack.
        /// </summary>
        private Stack<IFormat> formatStack = new Stack<IFormat>();

        /// <summary>
        /// Private IFormat object for get the Formats in stack.
        /// </summary>
        private Stack<BlockCodes> blockStack = new Stack<BlockCodes>();

        /// <summary>
        /// Private Lexem collection object for get the markup type of lexems.
        /// </summary>
        private IEnumerable<ILexem> markupCol = null;

        /// <summary>
        /// Private Lexem collection object for get the other type of lexems.
        /// </summary>
        private IEnumerable<ILexem> othersCol = null;

        internal bool isCommentBlock = false;

        #endregion Local Variables

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="control"></param>
        public MarkupLanguageBase(EditControl control)
            : base(control)
        {
        }

        #endregion Constructor

        #region Implementation

        internal override void UpdateWordFormats(WordDetails words, IFormat format)
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
                words.Foreground = this.TextForeground;
                words.Font = ParentControl.FontFamily;
                words.FontSize = ParentControl.FontSize;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLexemsChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnLexemsChanged(e);
            if (this.Lexem != null)
            {
                markupCol = this.Lexem.OfType<ILexem>().Where(lexem => lexem.LexemType != EditTokenType.Comment);
                commentsCol = this.Lexem.OfType<ILexem>().Where(lexem => lexem.LexemType == EditTokenType.Comment);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void SplitTextToLines()
        {
            StringBuilder tabString = new StringBuilder();
            for (int i = 0; i < this.ParentControl.TabSpaces; i++)
            {
                tabString.Append(" ");
            }
            //this.ParentControl.Text = this.ParentControl.Text.Replace(@"\t", tabString.ToString());
            this.ParentControl.Text = Regex.Replace(this.ParentControl.Text, "\\t", tabString.ToString());
            string[] splittedText = Regex.Split(this.ParentControl.Text, this.SplitLinesRegex);
            if (ParentControl != null)
            {
                ParentControl.Lines.Clear();
            }

            coloringDetails = new SyntaxColoringDetails()
            {
                CurrentColor = this.TextForeground,
                Pattern = GeneratePattern(this.Lexem),
                Lexems = Lexem.OfType<ILexem>().ToList()
            };
            int ind = 0;
            foreach (string str in splittedText)
            {
                LineItem item = new LineItem(str);

                ApplyFormats(item);
                if (ind == splittedText.Length - 1)
                {
                    this.ParentControl.isAddingLinesCompleted = true;
                }

                ParentControl.Lines.Add(item);
                ind++;
            }

            CalculatePreferredWidth();
            ApplyExpandItems();
        }

        internal override void ApplyFormats(LineItem item)
        {
            item.LineStartBlock = this.CurrentBlock;
            item.LineStartFormat = this.CurrentFormat;
            var index = this.ParentControl.Lines.IndexOf(item);
            if (index < 0)
            {
                index = this.ParentControl.Lines.Count;
            }
            IList<WordDetails> listWords = SplitTextToWords(item.Text, index);
            int textIndex = 0;
            foreach (WordDetails word in listWords)
            {
                word.StartIndex = textIndex;
                word.EndIndex = textIndex + word.Text.Length;
                textIndex += word.Text.Length;
            }
            item.WordsCollection = listWords;
            item.Refresh();
        }

        internal override IList<WordDetails> SplitTextToWords(string text, int line)
        {
            IList<WordDetails> details = new List<WordDetails>();
            if (CurrentBlock == null)
            {
                CurrentFormat = null;

                if (coloringDetails != null)
                {
                    GenerateTokens(details, text, coloringDetails.Lexems, line, CurrentBlock);
                }
            }
            else
            {
                if (CurrentBlock.SubLexems != null && CurrentBlock.SubLexems.OfType<ILexem>().Count() > 0)
                {
                    GenerateTokens(details, text, CurrentBlock.SubLexems.OfType<ILexem>(), line, CurrentBlock);
                }
                else
                {
                    GenerateTokens(details, text, coloringDetails.Lexems, line, CurrentBlock);
                }
            }

            return details;
        }

        private void GenerateTokens(IList<WordDetails> details, string text, IEnumerable<ILexem> lexem, int line, BlockCodes currentBlock)
        {
            int index = 0;
            var pattern = GeneratePattern(lexem);
            MatchCollection matches = Regex.Matches(text, pattern);
            IFormat backFormat = CurrentFormat;
            foreach (Match match in matches)
            {
                backFormat = CurrentFormat;
                string temp = text.Substring(index, match.Captures[0].Index - index);
                if (temp != string.Empty)
                {
                    details.Add(GenerateWordDetail(temp, backFormat, index));
                }

                if (this.isCommentBlock)
                {
                    ApplyTokenColor(details, line, match.Value, lexem, CurrentBlock, match.Index);
                }
                else
                {
                    ApplyTokenColor(details, line, match.Value, lexem, currentBlock, match.Index);
                    if (!this.isCommentBlock)
                    {
                        CurrentFormat = backFormat;
                    }
                }

                index = match.Captures[0].Index + match.Captures[0].Length;
            }

            //Add unmatched string to details
            if (index < text.Length)
            {
                if (text.Substring(index) != string.Empty)
                {
                    details.Add(GenerateWordDetail(text.Substring(index), this.isCommentBlock ? CurrentFormat : null, index));
                }
            }
        }

        private WordDetails GenerateWordDetail(string temp, IFormat Format, int startIndex)
        {
            WordDetails word = new WordDetails();
            word.Text = temp;
            word.StartIndex = startIndex;
            word.EndIndex = startIndex + temp.Length;
            UpdateWordFormats(word, Format);
            return word;
        }

        private void ApplyTokenColor(IList<WordDetails> details, int line, string text, IEnumerable<ILexem> Lexems, BlockCodes currentBlock, int index)
        {
            IFormat currentFormat = CurrentFormat;
            if (currentBlock == null)
            {
                var lexemsCollection = Lexems.Where(lex => (lex.IsRegex && Regex.Match(text, lex.StartText).Success) || (!lex.IsRegex && text.StartsWith(lex.StartText)));
                if (lexemsCollection.Count() > 0)
                {
                    var matchingItem = lexemsCollection.ElementAt(0);
                    currentBlock = new BlockCodes()
                    {
                        BlockStartText = matchingItem.StartText,
                        BlockEndText = matchingItem.EndText,
                        BlockStartLine = line,
                        CloseAtEOL = !matchingItem.IsMultiline,
                        SubLexems = matchingItem.SubLexems,
                        LexemType = matchingItem.LexemType
                    };

                    isCommentBlock = matchingItem.LexemType == EditTokenType.Comment;
                    currentFormat = this.GetFormat(matchingItem.FormatName);
                    CurrentFormat = currentFormat;
                }
            }

            if (!isCommentBlock && currentBlock != null && currentBlock.SubLexems != null && currentBlock.SubLexems.OfType<ILexem>().Count() > 0)
            {
                var tempBlock = currentBlock;
                var tempFormat = CurrentFormat;
                GenerateTokens(details, text, currentBlock.SubLexems.OfType<ILexem>(), line, null);
                currentBlock = tempBlock;
                currentFormat = tempFormat;
            }
            else
            {
                if (text != string.Empty)
                {
                    WordDetails word = GenerateWordDetail(text, currentFormat, index);
                    details.Add(word);
                }
            }

            if (currentBlock != null && ((currentBlock.BlockEndText != null && text.EndsWith(Regex.Unescape(currentBlock.BlockEndText))) || currentBlock.CloseAtEOL))
            {
                currentBlock = null;
                currentFormat = null;
            }

            CurrentBlock = currentBlock;
            if (!isCommentBlock)
            {
                CurrentFormat = currentFormat;
            }
        }

        private string GeneratePattern(IEnumerable LexemsCollection)
        {
            if (LexemsCollection != null)
            {
                StringBuilder patternString = new StringBuilder();
                commentsCol = LexemsCollection.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
                othersCol = LexemsCollection.OfType<ILexem>().Where(lex => lex.LexemType != EditTokenType.Comment);

                if (commentsCol != null)
                {
                    foreach (Lexem lexem in commentsCol)
                    {
                        patternString.Append(lexem.StartText + "|");

                        if (lexem.ContainsEndText)
                        {
                            patternString.Append(lexem.EndText + "|");
                        }
                    }
                }

                if (othersCol != null)
                {
                    foreach (Lexem lexem in othersCol)
                    {
                        patternString.Append(lexem.StartText + "|");

                        if (lexem.ContainsEndText)
                        {
                            patternString.Append(lexem.EndText + "|");
                        }
                    }
                }

                patternString = new StringBuilder(patternString.ToString().TrimEnd('|'));
                return patternString.ToString();
            }

            return "[?=\\t]|[?=\\s]";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected override void ApplyExpandCollapse(ApplyExpandCollapseArgs args)
        {
            if (args.LanguageBlocks == null)
            {
                return;
            }

            string startString = this.BlockStart;

            if (currentListener == null)
            {
                if (Regex.IsMatch(args.ExpandInformation.Text, startString) && !args.ExpandInformation.Text.EndsWith("/>"))
                {
                    string endString = Regex.Match(args.ExpandInformation.Text, startString).Value.Replace("<", "</") + ">";
                    if (!Regex.IsMatch(args.ExpandInformation.Text, endString))
                    {
                        currentListener = new BlockListener()
                        {
                            BlockStart = Regex.Match(args.ExpandInformation.Text, startString).Value,
                            BlockEnd = Regex.Match(args.ExpandInformation.Text, startString).Value.Replace("<", "</") + ">",
                            IsPreprocessor = false,
                            ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1
                        };
                        args.ExpandInformation.ParentLineNumber = 0;
                        //args.ExpandInformation.IsExpanded = true;
                        args.ExpandInformation.ContainsLines = false;
                        currentItem = args.ExpandInformation;
                        currentItem.StartLine = args.Source.IndexOf(args.ExpandInformation) + 1;

                        var tempBlock = new BlockListener()
                        {
                            BlockStart = currentListener.BlockStart,
                            BlockEnd = currentListener.BlockEnd,
                            IsPreprocessor = currentListener.IsPreprocessor,
                            ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1,
                            IsRegex = currentListener.IsRegex,
                            CheckParentType = currentListener.CheckParentType,
                            ParentLexemType = currentListener.ParentLexemType,
                            LexemType = currentListener.LexemType,
                            IsCollapsible = currentListener.IsCollapsible,
                            IsIndent = currentListener.IsIndent,
                            StartLine = args.Source.IndexOf(args.ExpandInformation) + 2,
                            IgnoreEndBlock = false
                        };
                        this.IndentableBlocks.Add(tempBlock);
                    }
                }
            }
            else
            {
                args.ExpandInformation.ContainsLines = false;
                args.ExpandInformation.ParentLineNumber = args.Source.IndexOf(currentItem) + 1;
                // currentItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                Match match = Regex.Match(args.ExpandInformation.Text, currentListener.BlockEnd);
                if (match.Success)
                {
                    if (blocksStack != null && blocksStack.Count > 0)
                    {
                        currentItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                        currentItem.ContainsLines = true;
                        var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                        if (indentListener.Count() > 0)
                        {
                            var indentBlock = indentListener.ElementAt(0);
                            indentBlock.IgnoreEndBlock = false;
                            indentBlock.EndLineNumber = currentItem.EndLine;
                        }
                        currentListener = blocksStack.Pop();
                        Match newMatch = Regex.Match(args.ExpandInformation.Text, currentListener.BlockEnd);
                        while (currentListener != null && newMatch.Success && newMatch.Index > match.Index)
                        {
                            currentItem = args.Source[currentListener.ParentLineNumber - 1];
                            args.ExpandInformation.ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1;
                            currentItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;

                            currentItem.ContainsLines = true;
                            indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                            if (indentListener.Count() > 0)
                            {
                                var indentBlock = indentListener.ElementAt(0);
                                indentBlock.IgnoreEndBlock = false;
                                indentBlock.EndLineNumber = currentItem.EndLine;
                            }

                            if (blocksStack.Count > 0)
                            {
                                currentListener = blocksStack.Pop();
                            }
                            else
                            {
                                currentListener = null;
                            }
                        }
                        if (currentListener != null)
                        {
                            currentItem = args.Source[currentListener.ParentLineNumber - 1];
                        }
                    }
                    else
                    {
                        currentItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                        currentItem.ContainsLines = true;
                        var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                        if (indentListener.Count() > 0)
                        {
                            var indentBlock = indentListener.ElementAt(0);
                            indentBlock.IgnoreEndBlock = false;
                            indentBlock.EndLineNumber = currentItem.EndLine;
                        }
                        currentListener = null;
                        currentItem = null;
                    }
                }
                else
                {
                    if (Regex.IsMatch(args.ExpandInformation.Text, startString) && !args.ExpandInformation.Text.TrimEnd().EndsWith("/>") && CheckCommentBlock(args))
                    {
                        string endString = Regex.Match(args.ExpandInformation.Text, startString).Value.Replace("<", "</") + ">";
                        if (!Regex.IsMatch(args.ExpandInformation.Text, endString))
                        {
                            if (blocksStack == null)
                            {
                                blocksStack = new Stack<BlockListener>();
                            }
                            blocksStack.Push(currentListener);
                            currentListener = new BlockListener()
                            {
                                BlockStart = Regex.Match(args.ExpandInformation.Text, startString).Value,
                                BlockEnd = Regex.Match(args.ExpandInformation.Text, startString).Value.Replace("<", "</") + ">",
                                IsPreprocessor = false,
                                ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1
                            };

                            //args.ExpandInformation.IsExpanded = true;
                            //args.ExpandInformation.ContainsLines = true;
                            currentItem = args.ExpandInformation;
                            currentItem.StartLine = args.Source.IndexOf(args.ExpandInformation) + 1;

                            var tempBlock = new BlockListener()
                            {
                                BlockStart = currentListener.BlockStart,
                                BlockEnd = currentListener.BlockEnd,
                                IsPreprocessor = currentListener.IsPreprocessor,
                                ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1,
                                IsRegex = currentListener.IsRegex,
                                CheckParentType = currentListener.CheckParentType,
                                ParentLexemType = currentListener.ParentLexemType,
                                LexemType = currentListener.LexemType,
                                IsCollapsible = currentListener.IsCollapsible,
                                IsIndent = currentListener.IsIndent,
                                StartLine = args.Source.IndexOf(args.ExpandInformation) + 2,
                                IgnoreEndBlock = false
                            };
                            this.IndentableBlocks.Add(tempBlock);
                        }
                    }
                    else if (!Regex.IsMatch(args.ExpandInformation.Text, startString) && args.ExpandInformation.Text.TrimEnd().EndsWith("/>") && CheckCommentBlock(args))
                    {
                        currentItem.ContainsLines = false;
                        args.ExpandInformation.ParentLineNumber = currentItem.ParentLineNumber;
                        if (blocksStack != null && blocksStack.Count > 0)
                        {
                            currentListener = blocksStack.Pop();
                            currentItem = args.Source[currentListener.ParentLineNumber - 1];
                        }
                    }
                }
            }
        }

        internal override void ResetLine(LineItem item)
        {
            ResetColor(this.ParentControl.Lines.IndexOf(item));
        }

        private int applyStartLine = -1;

        /// <summary>
        /// internal helper method to reset the color of the a line
        /// </summary>
        /// <param name="line"></param>
        internal override void ResetColor(int line)
        {
            if (!isUpdating)
            {
                if (applyStartLine == -1)
                {
                    applyStartLine = line;
                }

                if (ParentControl.IsPaste || ParentControl.IsSpaceKeyPressed || ParentControl.IsEnterKeyPressed)
                {
                    this.ApplyFormats(ParentControl.Lines[line]);
                }
                else
                {
                    for (int i = applyStartLine; i >= 0 && i < ParentControl.Lines.Count; i++)
                    {
                        var lineItem = ParentControl.Lines[i];
                        if (i == applyStartLine)
                        {
                            this.CurrentBlock = lineItem.LineStartBlock;
                            this.CurrentFormat = lineItem.LineStartFormat;
                        }
                        this.ApplyFormats(lineItem);
                    }
                }
                applyStartLine = -1;
            }
            else if (ParentControl.IsPaste && isUpdating)
            {
                this.ApplyFormats(ParentControl.Lines[line]);
            }
            else
            {
                if (applyStartLine == -1)
                {
                    applyStartLine = line;
                }
                else
                {
                    applyStartLine = Math.Min(line, applyStartLine);
                }
            }
        }

        /// <summary>
        /// Returns a list of the preprocessers defined in the language configurations
        /// </summary>
        /// <returns>Returns the Lexem List</returns>
        internal override List<BlockListener> GetBlockStartList()
        {
            List<BlockListener> returnlist = new List<BlockListener>();

            if (this.BlockStart != string.Empty && this.BlockStart != null && this.BlockEnd != null)
            {
                returnlist.Add(new BlockListener()
                {
                    BlockStart = this.BlockStart,
                    BlockEnd = this.BlockEnd,
                    IsPreprocessor = false
                });
            }

            if (this.Lexem != null)
            {
                foreach (Lexem prep in this.Lexem.OfType<ILexem>().Where<ILexem>(lexem => lexem.LexemType == EditTokenType.Preprocessor))
                {
                    returnlist.Add(new BlockListener()
                    {
                        BlockStart = prep.StartText,
                        BlockEnd = prep.EndText,
                        IsPreprocessor = true
                    });
                }
            }

            return returnlist;
        }

        internal bool CheckCommentBlock(ApplyExpandCollapseArgs args)
        {
            LineItemExpandInformation item = args.ExpandInformation;

            if (item.LineStartBlock != null && item.LineStartBlock.LexemType == EditTokenType.Comment)
            {
                return false;
            }

            if (args.Lexems != null)
            {
                var comments = args.Lexems.Where(lexem => lexem.LexemType == EditTokenType.Comment);
                LineItemExpandInformation tempItem = item;
                RegexOptions options = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                var blocks = comments.Where(lexem => (!lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText)) || (lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText) &&
                    (tempItem.Text.IndexOf(lexem.EndText) == -1 || tempItem.Text.IndexOf(lexem.EndText) == tempItem.Text.Length - lexem.EndText.Length)));
                if (blocks.Count() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        internal override string GetCollapsedItemText(LineItem item)
        {
            string pattern = @"<[\w:.]+";
            Match matchValue = Regex.Match(item.Text, pattern);
            if (matchValue != null && matchValue.Success)
            {
                return item.Text.Substring(0, matchValue.Index) + matchValue.Value + this.EllipsisText + ">";
            }
            return base.GetCollapsedItemText(item);
        }

        internal override void OnLineItemRender(LineItem item, DrawingContext context)
        {
            if (this.ApplyColoring && item.WordsCollection != null)
            {
                Point startPosition = new Point();
                if (item.IsExpanded)
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
                    if (item.ContainsPreprocessor)
                    {
                        startPosition.X = Utils.GetWidth(item.Text.Substring(0, item.Text.IndexOf(block.BlockStart)), ParentControl.FontFamily, ParentControl.FontSize, this.ParentControl.CollapsedTextForeground) + 1;
                        text = Utils.GetFormattedText(item.PreprocessorText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.CollapsedTextForeground);
                    }
                    else
                    {
                        string collapsedText = this.GetCollapsedItemText(item);
                        int textlength = 0;
                        foreach (WordDetails word in item.WordsCollection)
                        {
                            textlength = word.Text.Length;
                            if (collapsedText.Length >= textlength && collapsedText.Substring(0, textlength) == word.Text)
                            {
                                FormattedText formattext = Utils.GetFormattedText(word.Text, item.ParentControl.FontFamily, item.ParentControl.FontSize, word.Foreground);
                                Point pos = new Point();
                                pos.X = startPosition.X;
                                pos.Y = 0;
                                word.StartPosition = startPosition;
                                context.DrawText(formattext, pos);
                                startPosition.X += double.Parse(string.Format("{0:0.00}", formattext.WidthIncludingTrailingWhitespace));
                                collapsedText = collapsedText.Substring(textlength);
                            }
                            else if (collapsedText.StartsWith(this.EllipsisText))
                            {
                                text = Utils.GetFormattedText(this.EllipsisText, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.CollapsedTextForeground);
                                item.EllipsisPosition = startPosition;
                                context.DrawText(text, startPosition);
                                context.DrawRoundedRectangle(Brushes.Transparent, new Pen(this.ParentControl.CollapsedTextForeground, 0.5d), new Rect(startPosition, new Size(text.WidthIncludingTrailingWhitespace, item.ParentControl.LineHeight)), 1, 1);
                                textlength = this.EllipsisText.Length;
                                collapsedText = collapsedText.Substring(textlength);
                                startPosition.X += double.Parse(string.Format("{0:0.00}", text.WidthIncludingTrailingWhitespace));
                                break;
                            }
                        }
                        if (item.EndLine > 0)
                        {
                            var lastitem = this.ParentControl.Lines[item.EndLine - 1];
                            if (lastitem.WordsCollection != null && lastitem.WordsCollection.Count > 0)
                            {
                                var word = lastitem.WordsCollection[lastitem.WordsCollection.Count - 1];
                                if (word.Text == ">")
                                {
                                    text = Utils.GetFormattedText(word.Text, item.ParentControl.FontFamily, item.ParentControl.FontSize, word.Foreground);
                                    context.DrawText(text, startPosition);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                context.DrawText(Utils.GetFormattedText(item.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground), new Point(0, 0));
            }
        }

        internal override void UpdateCollapsedItemSelectionPointer(LineItem item)
        {
            var index = this.ParentControl.Lines.IndexOf(item);
            var ellipsisIndex = this.GetCollapsedItemText(item).IndexOf(this.EllipsisText);
            this.ParentControl.ScrollControl.UpdateSelectionPointer(index, item.EndLine - 1, ellipsisIndex, item.ParentControl.Lines[item.EndLine - 1].Text.Length - 1);
        }

        internal override int GetCollapsedItemSelectionEndIndex(LineItem lineitem, int regularEndIndex)
        {
            var collapsedText = this.GetCollapsedItemText(lineitem);
            if (this.ParentControl.ScrollControl.TextSelectionPointer != null && this.ParentControl.ScrollControl.TextSelectionPointer.EndLine > lineitem.EndLine)
            {
                return collapsedText.Length;
            }
            else if (this.ParentControl.ScrollControl.TextSelectionPointer != null && lineitem.EndLine >= this.ParentControl.ScrollControl.TextSelectionPointer.EndLine)
            {
                return collapsedText.Length - 1;
            }
            return regularEndIndex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        protected internal override string GetTextInCollapsedArea(LineItem lineItem)
        {
            StringBuilder builder = new StringBuilder();
            var ellipsisIndex = this.GetCollapsedItemText(lineItem).IndexOf(this.EllipsisText);
            builder.Append(lineItem.Text.Substring(ellipsisIndex));
            builder.Append(Environment.NewLine);
            builder.Append(this.ParentControl.GetTextRange(lineItem.StartLine, lineItem.EndLine - 2));
            builder.Append(Environment.NewLine);
            builder.Append(this.ParentControl.Lines[lineItem.EndLine - 1].Text.Substring(0, this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length - 1));
            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineitem"></param>
        /// <returns></returns>
        protected internal override int GetSelectionEndIndex(LineItem lineitem)
        {
            if (lineitem.ContainsLines)
            {
                return this.ParentControl.Lines[lineitem.EndLine - 1].Text.Length - 1;
            }
            return lineitem.Text.Length;
        }

        #endregion Implementation

        #region Overrides

        internal override void OnCommentCommandExecute(LineItem line)
        {
            if (!line.IsExpanded)
            {
                this.ParentControl.ExpandLine(this.ParentControl.Lines.IndexOf(line));
            }

            ILexem commentLexem = null;
            if ((this.commentsCol == null || this.commentsCol != null && this.commentsCol.Count() == 0) && this.Lexem.OfType<ILexem>().Count() > 0)
            {
                commentsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
            }

            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => lexem.IsMultiline);

                if (commentLexems.Count() > 0)
                {
                    commentLexem = commentLexems.ElementAt(0);
                }
            }

            int index = 0;
            Match match = Regex.Match(line.Text, @"\s+");
            if (match.Success)
            {
                index = match.Index + match.Value.Length;
            }

            if (commentLexem != null)
            {
                string tempStr = line.Text.Insert(index, commentLexem.StartText);
                tempStr += commentLexem.EndText;
                line.Text = tempStr;
            }

            this.ParentControl.ScrollControl.ClearSelection();
            this.ParentControl.ScrollControl.UpdateSelectionPointer(line.LineNumber - 1, line.LineNumber - 1, index, line.Text.Length);
            this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
            this.ParentControl.UndoManager.IsNewUndoItem = true;
            this.ParentControl.UndoManager.Add(new EditAction()
            {
                Action = ActionType.CommentSelection,
                LineNumber = line.LineNumber,
                Text = line.Text,
                IsSelected = false,
                Lexem = commentLexem
            });
            this.ParentControl.ScrollControl.MoveCursorToLineItem(this.ParentControl.ScrollControl.TextSelectionPointer.EndLine);
            if (this.ParentControl.ScrollControl.Caret != null)
            {
                this.ParentControl.ScrollControl.Caret.MoveToEnd();
            }
        }

        internal override void OnCommentCommandExecute(SelectionPointer pointer)
        {
            ILexem commentLexem = null;
            if ((this.commentsCol == null || this.commentsCol != null && this.commentsCol.Count() == 0) && this.Lexem.OfType<ILexem>().Count() > 0)
            {
                commentsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
            }

            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => lexem.IsMultiline);

                if (commentLexems.Count() > 0)
                {
                    commentLexem = commentLexems.ElementAt(0);
                }
            }

            if (pointer != null && commentLexem != null)
            {
                int endSelectionIndex = pointer.EndIndex;
                if (pointer.StartLine == pointer.EndLine)
                {
                    string tmpStr = this.ParentControl.Lines[pointer.StartLine].Text.Insert(pointer.StartIndex, commentLexem.StartText);
                    tmpStr = tmpStr.Insert(pointer.EndIndex + commentLexem.StartText.Length, commentLexem.EndText);
                    this.ParentControl.Lines[pointer.StartLine].Text = tmpStr;
                    endSelectionIndex = pointer.EndIndex + commentLexem.StartText.Length + commentLexem.EndText.Length;
                }
                else
                {
                    this.ParentControl.Lines[pointer.StartLine].Text = this.ParentControl.Lines[pointer.StartLine].Text.Insert(pointer.StartIndex, commentLexem.StartText);
                    this.ParentControl.Lines[pointer.EndLine].Text = this.ParentControl.Lines[pointer.EndLine].Text.Insert(pointer.EndIndex, commentLexem.EndText);
                    endSelectionIndex = pointer.EndIndex + commentLexem.EndText.Length;
                }
                this.ParentControl.ScrollControl.ClearSelection();
                this.ParentControl.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, pointer.StartIndex, endSelectionIndex);
                this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.CommentSelection,
                    Pointer = pointer,
                    Text = this.ParentControl.SelectedText,
                    IsSelected = true,
                    LineNumber = this.ParentControl.ScrollControl.CurrentLineItem.LineNumber,
                    Lexem = commentLexem
                });
                this.ParentControl.ScrollControl.MoveCursorToLineItem(pointer.EndLine);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.Caret.MoveToEnd();
                }
            }
        }

        internal override void OnUncommentCommandExecute(LineItem line)
        {
            ILexem commentLexem = null;
            if ((this.commentsCol == null || this.commentsCol != null && this.commentsCol.Count() == 0) && this.Lexem.OfType<ILexem>().Count() > 0)
            {
                commentsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
            }

            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => lexem.IsMultiline);

                if (commentLexems.Count() > 0)
                {
                    commentLexem = commentLexems.ElementAt(0);
                }
            }

            if (commentLexem == null) return;

            int startindex = line.Text.IndexOf(commentLexem.StartText);
            int endindex = line.Text.IndexOf(commentLexem.EndText) - commentLexem.StartText.Length;

            if (startindex >= 0 && endindex > 0 && endindex < line.Text.Length)
            {
                string tmpText = line.Text.Remove(startindex, commentLexem.StartText.Length);
                tmpText = tmpText.Remove(endindex, commentLexem.EndText.Length);
                line.Text = tmpText;

                this.ParentControl.ScrollControl.ClearSelection();
                this.ParentControl.ScrollControl.UpdateSelectionPointer(line.LineNumber - 1, line.LineNumber - 1, startindex, line.Text.Length);
                this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.UncommentSelection,
                    LineNumber = line.LineNumber,
                    Text = line.Text,
                    IsSelected = false,
                    Lexem = commentLexem
                });
                this.ParentControl.ScrollControl.MoveCursorToLineItem(this.ParentControl.ScrollControl.TextSelectionPointer.EndLine);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.Caret.MoveToEnd();
                }
            }
        }

        internal override void OnUncommentCommandExecute(SelectionPointer pointer)
        {
            ILexem commentLexem = null;
            if ((this.commentsCol == null || this.commentsCol != null && this.commentsCol.Count() == 0) && this.Lexem.OfType<ILexem>().Count() > 0)
            {
                commentsCol = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
            }

            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => lexem.IsMultiline);

                if (commentLexems.Count() > 0)
                {
                    commentLexem = commentLexems.ElementAt(0);
                }
            }

            if (pointer != null && commentLexem != null)
            {
                int selectionEndIndex = pointer.EndIndex;

                if (pointer.StartLine == pointer.EndLine)
                {
                    var line = this.ParentControl.Lines[pointer.StartLine];
                    int startindex = line.Text.IndexOf(commentLexem.StartText);
                    int endindex = line.Text.IndexOf(commentLexem.EndText) - commentLexem.StartText.Length;
                    if (startindex >= 0 && endindex > 0 && endindex < line.Text.Length)
                    {
                        string tmpText = line.Text.Remove(startindex, commentLexem.StartText.Length);
                        tmpText = tmpText.Remove(endindex, commentLexem.EndText.Length);
                        line.Text = tmpText;
                        selectionEndIndex = pointer.EndIndex - commentLexem.StartText.Length - commentLexem.EndText.Length;
                    }
                }
                else
                {
                    int startIndex = this.ParentControl.Lines[pointer.StartLine].Text.IndexOf(commentLexem.StartText);
                    int endIndex = this.ParentControl.Lines[pointer.EndLine].Text.IndexOf(commentLexem.EndText);
                    if (startIndex >= pointer.StartIndex && endIndex > 0 && endIndex <= pointer.EndIndex)
                    {
                        this.ParentControl.Lines[pointer.StartLine].Text = this.ParentControl.Lines[pointer.StartLine].Text.Remove(startIndex, commentLexem.StartText.Length);
                        this.ParentControl.Lines[pointer.EndLine].Text = this.ParentControl.Lines[pointer.EndLine].Text.Remove(endIndex, commentLexem.EndText.Length);
                        selectionEndIndex = pointer.EndIndex - commentLexem.EndText.Length;
                    }
                }

                this.ParentControl.ScrollControl.ClearSelection();
                this.ParentControl.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, pointer.StartIndex, selectionEndIndex);
                this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.UncommentSelection,
                    Pointer = pointer,
                    Text = this.ParentControl.SelectedText,
                    IsSelected = true,
                    LineNumber = this.ParentControl.ScrollControl.CurrentLineItem.LineNumber,
                    Lexem = commentLexem
                });
                this.ParentControl.ScrollControl.MoveCursorToLineItem(pointer.EndLine);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.Caret.MoveToEnd();
                }
            }
        }

        internal void OnUncommentCommandExecute(LineItem line, ILexem commentLexem, bool removeEndText)
        {
            if (line.Text.Trim() == string.Empty)
                return;

            int index = 0;
            var words = line.WordsCollection.Where(word => word.Text.Trim() != string.Empty).OrderBy(word => word.StartIndex);
            if (words.Count() > 0)
            {
                index = words.ElementAt(0).StartIndex;
            }

            if (commentLexem != null)
            {
                int commentTextIndex = line.Text.IndexOf(commentLexem.StartText);
                if (commentTextIndex >= 0 && commentTextIndex <= index)
                {
                    string tempStr = line.Text.Remove(index, commentLexem.StartText.Length);
                }
            }
        }

        internal override void OnTextInput(int currentLineNumber, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ">" && currentLineNumber >= 0 && currentLineNumber < this.ParentControl.Lines.Count)
            {
                var line = this.ParentControl.Lines[currentLineNumber];
                if (!line.Text.Trim().EndsWith("/>"))
                {
                    Match match = Regex.Match(line.Text, @"<(?'type'\w+[:\w.]+)");
                    if (match.Success)
                    {
                        var matchStr = string.Format(@"</{0}>", match.Groups[1].Value);
                        line.Text += matchStr;
                    }
                }
            }
        }

        internal override bool CheckBlockStarts(LineItem tempLineItem)
        {
            return (Regex.IsMatch(tempLineItem.Text, this.BlockStart) && !tempLineItem.Text.TrimEnd().EndsWith("/>"));
        }

        internal override bool CheckBlockEnds(LineItem tempLineItem)
        {
            return Regex.IsMatch(tempLineItem.Text, this.BlockEnd);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        protected internal override int GetIndentLevel(int lineNumber)
        {
            if (!this.SupportsOutlining || this.IndentableBlocks == null)
                return 0;

            var indentableLines = this.IndentableBlocks.Where(line => ((line.IgnoreEndBlock && line.ParentLineNumber <= lineNumber) || (!line.IgnoreEndBlock && line.ParentLineNumber < lineNumber + 1)) && ((line.IgnoreEndBlock && line.EndLineNumber >= lineNumber + 1) || (!line.IgnoreEndBlock && line.EndLineNumber > lineNumber + 1)));
            int linecount = indentableLines.Count();
            return linecount;
        }

        #endregion Overrides

        #region Delete and Backspace

        /// <summary>
        /// Helper method for delete key activity
        /// </summary>
        protected internal override void ExecuteDeleteText()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000d);
                timer.Tick += new EventHandler(timer_Tick);
            }
            timerValue = 0;
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
                timer.Start();
                return;
            }

            int startLine = this.ParentControl.LineNumber - 1;
            LineItem lineItem = this.ParentControl.Lines[startLine];
            int cursorIndex = this.ParentControl.CursorIndex;
            string collapsedItemText = this.GetCollapsedItemText(lineItem);
            int ellipsisIndex = collapsedItemText.IndexOf(this.EllipsisText);

            if (lineItem.IsExpanded || (!lineItem.IsExpanded && !lineItem.ContainsPreprocessor && cursorIndex < lineItem.Text.Length && cursorIndex < ellipsisIndex))
            {
                if (cursorIndex == lineItem.Text.Length)
                {
                    if (this.ParentControl.Lines.Count > this.ParentControl.LineNumber)
                    {
                        string txt = this.ParentControl.Lines[this.ParentControl.LineNumber].Text;
                        string tempTxt = this.ParentControl.Lines[this.ParentControl.LineNumber - 1].Text;
                        if (!this.ParentControl.Lines[this.ParentControl.LineNumber].IsExpanded && tempTxt.Trim() != string.Empty)
                        {
                            this.ParentControl.ExpandLine(this.ParentControl.LineNumber);
                        }
                        else if (!this.ParentControl.Lines[this.ParentControl.LineNumber].IsExpanded && tempTxt.Trim() == string.Empty)
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
                            CursorIndex = cursorIndex,
                            Text = remtext,
                            NewLine = true
                        });

                        lineItem.Text = lineItem.Text + txt;
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(0);
                        double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                        if (x > this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - this.ParentControl.ScrollControl.HScrollBar.SmallChange);
                        }
                        this.ApplyExpandItems();
                    }
                }
                else
                {
                    if (lineItem.Text != string.Empty)
                    {
                        remtext = lineItem.Text.Substring(cursorIndex, 1);
                        lineItem.Text = lineItem.Text.Remove(cursorIndex, 1);
                        lineItem.TextWidth = Utils.GetWidth(lineItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);

                        if (this.ParentControl.ScrollControl.Caret == null)
                        {
                            lineItem.SetCursorOnLoad = true;
                            lineItem.SetCursorIndex = cursorIndex;
                            this.ParentControl.ScrollControl.SetVerticalOffset(startLine);
                        }
                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = this.ParentControl.LineNumber,
                            CursorIndex = cursorIndex,
                            Text = remtext
                        });
                        timer.Start();
                    }
                }
            }
            else
            {
                if (cursorIndex == ellipsisIndex + EllipsisText.Length)
                {
                    LineItem endItem = this.ParentControl.Lines[lineItem.EndLine - 1];
                    cursorIndex = endItem.Text.Length - 1;
                    remtext = endItem.Text.Substring(Math.Min(cursorIndex, endItem.Text.Length), 1);
                    this.ParentControl.ExpandLine(startLine);
                    this.ParentControl.ScrollControl.ScrollRows.ScrollInView(lineItem.EndLine - 1);
                    this.ParentControl.ScrollControl.MoveCursorToLineItem(lineItem.EndLine - 1);
                    endItem.Text = endItem.Text.Remove(Math.Min(cursorIndex, endItem.Text.Length), 1);
                    endItem.TextWidth = Utils.GetWidth(endItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);
                    if (this.ParentControl.ScrollControl.Caret != null)
                    {
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(-1);
                        if (this.ParentControl.ScrollControl.Caret.CaretPosition.X > this.ParentControl.ScrollControl.ViewportWidth && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.HScrollBar.SmallChange && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.ExtentWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - (this.ParentControl.ScrollControl.HScrollBar.SmallChange * 2));
                        }
                        else if (this.ParentControl.ScrollControl.HorizontalOffset > 0 && this.ParentControl.ScrollControl.Caret.CaretPosition.X < this.ParentControl.ScrollControl.ViewportWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(0);
                        }
                    }
                    else
                    {
                        endItem.SetCursorIndex = cursorIndex;
                        endItem.SetCursorOnLoad = true;
                    }

                    this.ParentControl.UndoManager.IsNewUndoItem = true;
                    this.ParentControl.UndoManager.Add(new EditAction()
                    {
                        Action = ActionType.Delete,
                        LineNumber = lineItem.EndLine,
                        CursorIndex = cursorIndex,
                        Text = remtext
                    });
                    timer.Start();
                }
                else if (cursorIndex == ellipsisIndex)
                {
                    int selectionEndIndex = GetCollapsedItemSelectionEndIndex(lineItem, lineItem.Text.Length);
                    SelectionPointer tempPointer = new SelectionPointer();
                    tempPointer.StartLine = this.ParentControl.LineNumber - 1;
                    tempPointer.EndLine = lineItem.EndLine - 1;
                    tempPointer.StartIndex = ellipsisIndex >= 0 ? ellipsisIndex : lineItem.Text.Length;
                    tempPointer.EndIndex = selectionEndIndex >= 0 ? selectionEndIndex : this.ParentControl.Lines[lineItem.EndLine - 1].Text.Length;
                    remtext = GetTextInCollapsedArea(lineItem);
                    this.ParentControl.UndoManager.IsNewUndoItem = true;
                    this.ParentControl.UndoManager.Add(new EditAction()
                    {
                        Action = ActionType.Delete,
                        LineNumber = this.ParentControl.LineNumber,
                        CursorIndex = cursorIndex,
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
                    this.ApplyExpandItems();
                }
                else if (cursorIndex == collapsedItemText.Length)
                {
                    if (this.ParentControl.Lines.Count > lineItem.EndLine - 1)
                    {
                        LineItem endLine = this.ParentControl.Lines[lineItem.EndLine - 1];
                        string txt = this.ParentControl.Lines[lineItem.EndLine].Text;
                        if (!this.ParentControl.Lines[lineItem.EndLine].IsExpanded)
                        {
                            this.ParentControl.ExpandLine(lineItem.EndLine);
                        }

                        this.ParentControl.ExpandLine(startLine);
                        this.ParentControl.ScrollControl.rowheights.RemoveLines(lineItem.EndLine, 1, null);
                        this.ParentControl.Lines.RemoveAt(lineItem.EndLine);

                        this.ParentControl.UndoManager.IsNewUndoItem = true;
                        this.ParentControl.UndoManager.Add(new EditAction()
                        {
                            Action = ActionType.Delete,
                            LineNumber = lineItem.EndLine,
                            CursorIndex = endLine.Text.Length,
                            Text = remtext,
                            NewLine = true
                        });

                        endLine.Text = endLine.Text + txt;
                        this.ParentControl.ScrollControl.ScrollRows.ScrollInView(lineItem.EndLine - 1);
                        this.ParentControl.ScrollControl.MoveCursorToLineItem(lineItem.EndLine - 1);
                        if (this.ParentControl.ScrollControl.Caret != null)
                        {
                            this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(cursorIndex);
                            double x = this.ParentControl.ScrollControl.Caret.CaretPosition.X;
                            if (x > this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.ViewportWidth)
                            {
                                this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - this.ParentControl.ScrollControl.HScrollBar.SmallChange);
                            }
                        }
                        else
                        {
                            endLine.SetCursorOnLoad = true;
                            endLine.SetCursorIndex = cursorIndex;
                        }
                        timer.Start();
                    }
                }
            }

            this.ParentControl.isReinitializeLines = false;
            this.ParentControl.Text = this.ParentControl.GetText();
            this.ParentControl.isReinitializeLines = true;
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
        protected internal override void ExecuteBackspace()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000d);
                timer.Tick += new EventHandler(timer_Tick);
            }
            timerValue = 0;
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
                timer.Start();
                return;
            }

            string collapsedItemText = this.GetCollapsedItemText(lineItem);
            int ellipsisIndex = collapsedItemText.IndexOf(this.EllipsisText);
            int cursorIndex = this.ParentControl.CursorIndex;

            BlockListener listener = lineItem.ContainsPreprocessor ? lineItem.GetPreprocessorType() : null;

            if (lineItem.IsExpanded || (!lineItem.IsExpanded && !lineItem.ContainsPreprocessor && cursorIndex != ellipsisIndex + EllipsisText.Length && cursorIndex < collapsedItemText.Length))
            {
                #region Normal Backspace Activity

                if (cursorIndex > 0)
                {
                    if (lineItem.Text != string.Empty)
                    {
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
                            CursorIndex = cursorIndex,
                            Text = remtext
                        });
                        timer.Start();
                    }
                }
                else
                {
                    int tempind = 0;
                    if (this.ParentControl.LineNumber > 1)
                    {
                        if (lineItem.Text.Length > 0)
                        {
                            tempind = this.ParentControl.Lines[startLine - 1].Text.Length;
                            this.ParentControl.Lines[startLine - 1].Text = this.ParentControl.Lines[startLine - 1].Text + lineItem.Text;
                        }
                        else
                        {
                            tempind = this.ParentControl.Lines[startLine - 1].Text.Length;
                        }

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

                        if (!lineItem.IsExpanded)
                        {
                            this.ParentControl.ExpandLine(startLine);
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
                if (cursorIndex == ellipsisIndex + EllipsisText.Length)
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
                else if (cursorIndex == collapsedItemText.Length)
                {
                    LineItem endItem = this.ParentControl.Lines[lineItem.EndLine - 1];
                    cursorIndex = endItem.Text.Length;
                    remtext = endItem.Text.Substring(Math.Min(cursorIndex - 1, endItem.Text.Length), 1);
                    this.ParentControl.ExpandLine(startLine);
                    this.ParentControl.ScrollControl.ScrollRows.ScrollInView(lineItem.EndLine - 1);
                    this.ParentControl.ScrollControl.MoveCursorToLineItem(lineItem.EndLine - 1);
                    endItem.Text = endItem.Text.Remove(Math.Min(cursorIndex - 1, endItem.Text.Length), 1);
                    endItem.TextWidth = Utils.GetWidth(endItem.Text, this.ParentControl.FontFamily, this.ParentControl.FontSize, this.ParentControl.Foreground);
                    if (this.ParentControl.ScrollControl.Caret != null)
                    {
                        this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveTo(-1);
                        if (this.ParentControl.ScrollControl.Caret.CaretPosition.X > this.ParentControl.ScrollControl.ViewportWidth && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.HorizontalOffset + this.ParentControl.ScrollControl.HScrollBar.SmallChange && this.ParentControl.ScrollControl.Caret.CaretPosition.X <= this.ParentControl.ScrollControl.ExtentWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(this.ParentControl.ScrollControl.Caret.CaretPosition.X - (this.ParentControl.ScrollControl.HScrollBar.SmallChange * 2));
                        }
                        else if (this.ParentControl.ScrollControl.HorizontalOffset > 0 && this.ParentControl.ScrollControl.Caret.CaretPosition.X < this.ParentControl.ScrollControl.ViewportWidth)
                        {
                            this.ParentControl.ScrollControl.SetHorizontalOffset(0);
                        }
                    }
                    else
                    {
                        endItem.SetCursorIndex = endItem.Text.Length;
                        endItem.SetCursorOnLoad = true;
                    }

                    this.ParentControl.UndoManager.IsNewUndoItem = true;
                    this.ParentControl.UndoManager.Add(new EditAction()
                    {
                        Action = ActionType.Backspace,
                        LineNumber = lineItem.EndLine,
                        CursorIndex = cursorIndex,
                        Text = remtext
                    });
                }

                timer.Start();
            }

            this.ParentControl.isReinitializeLines = false;
            this.ParentControl.Text = this.ParentControl.GetText();
            this.ParentControl.isReinitializeLines = true;
        }

        private int timerValue = 0;

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

        #endregion Delete and Backspace
    }

    #region Syntax Highlighting Helper Class

    /// <summary>
    /// Internal class used for store syntax coloring details
    /// </summary>
    internal class SyntaxColoringDetails
    {
        /// <summary>
        /// Private string variable for Pattern.
        /// </summary>
        private string pattern;

        /// <summary>
        /// Private Brush object for current color.
        /// </summary>
        private Brush currentColor;

        /// <summary>
        /// Private List&lt;ILexem&gt; object for Lexems.
        /// </summary>
        private List<ILexem> collection;

        /// <summary>
        /// Gets or sets the Pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern
        {
            get
            {
                return pattern;
            }

            set
            {
                pattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the current Lexem.
        /// </summary>
        /// <value>The color of the current Lexem.</value>
        public Brush CurrentColor
        {
            get
            {
                return currentColor;
            }

            set
            {
                currentColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the lexems.
        /// </summary>
        /// <value>The lexems.</value>
        public List<ILexem> Lexems
        {
            get
            {
                return collection;
            }

            set
            {
                collection = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxColoringDetails"/> class.
        /// </summary>
        public SyntaxColoringDetails()
        {
            collection = new List<ILexem>();
        }
    }

    #endregion Syntax Highlighting Helper Class
}