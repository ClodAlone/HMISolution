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
using System.Reflection;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public abstract class ProceduralLanguageBase : LanguageBase
    {
        #region Initialization

        /// <summary>
        ///
        /// </summary>
        /// <param name="control"></param>
        public ProceduralLanguageBase(EditControl control)
            : base(control)
        {
            this.IntellisenseDrillDownChar = '.';
        }

        #endregion Initialization

        #region Overrides

        internal override string GetCollapsedItemText(LineItem item)
        {
            BlockListener block = item.GetPreprocessorType();
            if (block != null && block.IsPreprocessor)
            {
                var index = item.Text.IndexOf(block.BlockStart);
                if (index >= 0)
                {
                    return item.Text.Substring(0, index) + item.Text.Substring(index + block.BlockStart.Length).TrimStart();
                }
                return item.Text;
            }
            else
            {
                return item.Text + this.EllipsisText;
            }
        }

        internal override void UpdateCollapsedItemSelectionPointer(LineItem item)
        {
            var index = this.ParentControl.Lines.IndexOf(item);
            if (!item.ContainsPreprocessor)
            {
                this.ParentControl.ScrollControl.UpdateSelectionPointer(index, item.EndLine - 1, item.Text.Length, item.ParentControl.Lines[item.EndLine - 1].Text.Length);
            }
            else
            {
                var ellipsisIndex = item.Text.IndexOf(item.GetPreprocessorType().BlockStart);
                this.ParentControl.ScrollControl.UpdateSelectionPointer(index, item.EndLine - 1, ellipsisIndex, item.ParentControl.Lines[item.EndLine - 1].Text.Length);
            }
        }

        internal override void OnCommentCommandExecute(LineItem line)
        {
            ILexem commentLexem = null;
            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => !lexem.IsMultiline);

                foreach (ILexem lexem in commentLexems)
                {
                    commentLexem = lexem;
                }
            }

            this.OnCommentCommandExecute(line, commentLexem);
            this.ParentControl.ScrollControl.ClearSelection();
            this.ParentControl.ScrollControl.UpdateSelectionPointer(line.LineNumber - 1, line.LineNumber - 1, 0, line.Text.Length);
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
            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => !lexem.IsMultiline);

                foreach (ILexem lexem in commentLexems)
                {
                    commentLexem = lexem;
                }
            }

            if (pointer != null && commentLexem != null)
            {
                for (int i = pointer.StartLine; i <= pointer.EndLine; i++)
                {
                    this.OnCommentCommandExecute(this.ParentControl.Lines[i], commentLexem);
                }
                this.ParentControl.ScrollControl.ClearSelection();
                this.ParentControl.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, 0, this.ParentControl.Lines[pointer.EndLine].Text.Length);
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

        internal void OnCommentCommandExecute(LineItem line, ILexem commentLexem)
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
                if (commentLexem.ShowAlternateIntellisenseText)
                {
                    line.Text = line.Text.Insert(index, commentLexem.IntellisenseDisplayText);
                }
                else
                {
                    line.Text = line.Text.Insert(index, commentLexem.StartText);
                }
            }
        }

        internal override void OnCommentCommandExecute(LineItem line, int index, ILexem commentLexem)
        {
            if (line.Text.Trim() == string.Empty)
                return;

            if (commentLexem != null)
            {
                if (commentLexem.ShowAlternateIntellisenseText)
                {
                    line.Text = line.Text.Insert(index, commentLexem.IntellisenseDisplayText);
                }
                else
                {
                    line.Text = line.Text.Insert(index, commentLexem.StartText);
                }
            }
        }

        internal override void OnUncommentCommandExecute(LineItem line)
        {
            ILexem commentLexem = null;
            if (line.Text.Trim() == string.Empty)
                return;

            int index = 0;

            var words = line.WordsCollection.Where(word => word.Text.Trim() != string.Empty).OrderBy(word => word.StartIndex);
            if (words.Count() > 0)
            {
                WordDetails word = words.ElementAt(0);
                index = words.ElementAt(0).StartIndex;
                var matchingComments = commentsCol.Where(comment => (comment.ShowAlternateIntellisenseText && comment.IntellisenseDisplayText == word.Text) || (!comment.ShowAlternateIntellisenseText && comment.StartText == word.Text));
                if (matchingComments.Count() > 0)
                {
                    commentLexem = matchingComments.ElementAt(0);
                }
            }

            this.OnUncommentCommandExecute(line, index, commentLexem);
            this.ParentControl.ScrollControl.ClearSelection();
            this.ParentControl.ScrollControl.UpdateSelectionPointer(line.LineNumber - 1, line.LineNumber - 1, 0, line.Text.Length);
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

        internal override void OnUncommentCommandExecute(SelectionPointer pointer)
        {
            ILexem commentLexem = null;
            if (this.commentsCol != null && this.commentsCol.Count() > 0)
            {
                var commentLexems = this.commentsCol.Where(lexem => !lexem.IsMultiline);

                foreach (ILexem lexem in commentLexems)
                {
                    commentLexem = lexem;
                }
            }

            Dictionary<LineItem, UndoComment> replacedLexems = new Dictionary<LineItem, UndoComment>();
            if (pointer != null && commentLexem != null)
            {
                for (int i = pointer.StartLine; i <= pointer.EndLine; i++)
                {
                    int index = 0;
                    var line = this.ParentControl.Lines[i];
                    var words = line.WordsCollection.Where(word => word.Text.Trim() != string.Empty).OrderBy(word => word.StartIndex);
                    if (words.Count() > 0)
                    {
                        WordDetails word = words.ElementAt(0);
                        index = words.ElementAt(0).StartIndex;
                        var matchingComments = commentsCol.Where(comment => (comment.ShowAlternateIntellisenseText && comment.IntellisenseDisplayText == word.Text.Trim()) || (!comment.ShowAlternateIntellisenseText && comment.StartText == word.Text));
                        if (matchingComments.Count() > 0)
                        {
                            ILexem newMatch = matchingComments.ElementAt(0);
                            if (newMatch != null && newMatch != commentLexem)
                            {
                                replacedLexems.Add(line, new UndoComment() { CommentLexem = newMatch, CursorIndex = index });
                                this.OnUncommentCommandExecute(line, index, newMatch);
                            }
                            else
                            {
                                replacedLexems.Add(line, new UndoComment() { CommentLexem = commentLexem, CursorIndex = index });
                                this.OnUncommentCommandExecute(line, index, commentLexem);
                            }
                        }
                        else
                        {
                            replacedLexems.Add(line, new UndoComment() { CommentLexem = commentLexem, CursorIndex = index });
                            this.OnUncommentCommandExecute(line, index, commentLexem);
                        }
                    }
                    else
                    {
                        replacedLexems.Add(line, new UndoComment() { CommentLexem = commentLexem, CursorIndex = index });
                        this.OnUncommentCommandExecute(line, index, commentLexem);
                    }
                }

                this.ParentControl.ScrollControl.ClearSelection();
                this.ParentControl.ScrollControl.UpdateSelectionPointer(pointer.StartLine, pointer.EndLine, 0, this.ParentControl.Lines[pointer.EndLine].Text.Length);
                this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                this.ParentControl.UndoManager.IsNewUndoItem = true;
                this.ParentControl.UndoManager.Add(new EditAction()
                {
                    Action = ActionType.UncommentSelection,
                    Pointer = pointer,
                    Text = this.ParentControl.SelectedText,
                    IsSelected = true,
                    LineNumber = this.ParentControl.ScrollControl.CurrentLineItem.LineNumber,
                    UncommentedLexems = replacedLexems,
                    Lexem = commentLexem
                });
                this.ParentControl.ScrollControl.MoveCursorToLineItem(pointer.EndLine);
                if (this.ParentControl.ScrollControl.Caret != null)
                {
                    this.ParentControl.ScrollControl.Caret.MoveToEnd();
                }
            }
        }

        internal void OnUncommentCommandExecute(LineItem line, int index, ILexem commentLexem)
        {
            if (commentLexem != null)
            {
                string startText = commentLexem.ShowAlternateIntellisenseText ? commentLexem.IntellisenseDisplayText : commentLexem.StartText;
                int commentTextIndex = line.Text.IndexOf(startText);
                if (commentTextIndex >= 0 && commentTextIndex <= index)
                {
                    line.Text = line.Text.Remove(index, startText.Length);
                }
            }
        }

        internal override bool CheckBlockStarts(LineItem tempLineItem)
        {
            return tempLineItem.Text.Contains(this.BlockStart);
        }

        internal override bool CheckBlockEnds(LineItem tempLineItem)
        {
            return tempLineItem.Text.Contains(this.BlockEnd);
        }

        #endregion Overrides

        #region Implementation

        #region Syntax Highlighting

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected override IFormat ApplyColor(string text, int line)
        {
            bool startTextEndTextDiff = false;

            if (literalsCol != null)
            {
                foreach (ILexem literal in literalsCol)
                {
                    if (literal != null)
                    {
                        if (literal.EndText == literal.StartText) continue;
                        {
                            if (!text.Contains(literal.EndText)) continue;
                            {
                                startTextEndTextDiff = true;
                            }
                        }
                    }

                    if (CurrentFormat != null)
                    {
                        currentColor = CurrentFormat.Foreground;
                    }

                    CurrentBlock = null;
                }
            }

            if (CurrentBlock != null)
            {
                if (line != CurrentBlock.BlockStartLine && CurrentBlock.CloseAtEOL)
                {
                    CurrentBlock = null;
                }
            }

            if (CurrentBlock == null && text != ".")
            {
                if (commentsCol != null)
                {
                    IEnumerable<ILexem> matchingcomments = from comments in commentsCol
                        where
                            (comments.IsRegex && Regex.Match(text, comments.StartText).Success) ||
                            (!comments.IsRegex && text.Trim().Equals(comments.StartText))
                        select comments;

                    if (!CaseSensitive)
                    {
                        matchingcomments = from comments in commentsCol
                            where
                                (comments.IsRegex && Regex.Match(text, comments.StartText).Success) ||
                                (!comments.IsRegex && text.Trim().ToLower().Equals(comments.StartText))
                            select comments;
                    }

                    foreach (ILexem comment in matchingcomments)
                    {
                        if (comment != null)
                        {
                            CurrentBlock = new BlockCodes(line, !comment.IsMultiline, comment.StartText, comment.EndText)
                            {
                                LexemType = comment.LexemType
                            };
                            CurrentFormat = GetFormat(comment.FormatName);
                        }

                        if (CurrentFormat != null)
                        {
                            currentColor = CurrentFormat.Foreground;
                        }
                        else
                        {
                            currentColor = ParentControl.Foreground;
                        }
                        break;
                    }
                }

                if (CurrentBlock == null)
                {
                    if (literalsCol != null)
                    {
                        IEnumerable<ILexem> matchingliterals = from literal in literalsCol
                            where
                                (literal.IsRegex && Regex.Match(text, literal.StartText).Success) ||
                                (!literal.IsRegex && text.Trim().StartsWith(literal.StartText))
                            select literal;

                        if (!CaseSensitive)
                        {
                            matchingliterals = from literal in literalsCol
                                where
                                    (literal.IsRegex && Regex.Match(text, literal.StartText).Success) ||
                                    text.Trim().ToLower().StartsWith(literal.StartText.ToLower())
                                select literal;
                        }

                        foreach (ILexem literal in matchingliterals)
                        {
                            if (literal != null)
                            {
                                CurrentBlock = new BlockCodes(line, !literal.IsMultiline, literal.StartText,
                                    literal.EndText)
                                {
                                    LexemType = literal.LexemType
                                };
                                CurrentFormat = GetFormat(literal.FormatName);
                            }
                            currentColor = CurrentFormat != null ? CurrentFormat.Foreground : TextForeground;
                        }
                    }

                    if (CurrentBlock == null)
                    {
                        if (!startTextEndTextDiff)
                        {
                            currentColor = TextForeground;
                            CurrentFormat = null;
                        }

                        if (keywordsCol != null)
                        {
                            IEnumerable<ILexem> matchingwords = from words in keywordsCol
                                where
                                    (words.IsRegex && Regex.Match(text.Trim(), words.StartText).Success) ||
                                    (!words.IsRegex && words.StartText == text.Trim())
                                select words;

                            if (!CaseSensitive)
                            {
                                matchingwords = from words in keywordsCol
                                    where
                                        (words.IsRegex &&
                                         Regex.Match(text.Trim(), words.StartText, RegexOptions.IgnoreCase).Success) ||
                                        (!words.IsRegex &&
                                         String.Equals(words.StartText, text.Trim(),
                                             StringComparison.CurrentCultureIgnoreCase))
                                    select words;
                            }

                            foreach (ILexem match in matchingwords)
                            {
                                CurrentBlock = null;
                                if (match != null) CurrentFormat = GetFormat(match.FormatName);
                                if (CurrentFormat != null)
                                {
                                    currentColor = CurrentFormat != null ? CurrentFormat.Foreground : TextForeground;
                                }
                            }
                        }
                    }
                }

                return CurrentFormat;
            }
            if (CurrentBlock != null && CurrentBlock.BlockEndText != null)
            {
                if (text == CurrentBlock.BlockEndText || text.Trim().EndsWith(CurrentBlock.BlockEndText))
                {
                    CurrentBlock = null;
                }

                return CurrentFormat;
            }
            CurrentBlock = null;

            return null;
        }

        #endregion Syntax Highlighting

        #region Expand Collapse

        private bool ignoreEndBlock = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected override void ApplyExpandCollapse(ApplyExpandCollapseArgs args)
        {
            bool createListener = false;
            if (args.LanguageBlocks == null)
            {
                return;
            }
            var selblocks = from blk in args.LanguageBlocks
                            where (((args.ExpandInformation.Text.Trim().StartsWith(blk.BlockStart) && blk.IsPreprocessor) || (args.ExpandInformation.Text.Trim() == blk.BlockStart)
                            || (blk.IsRegex && CheckRegexMatch(args.ExpandInformation.Text, blk.BlockStart) && CheckParentLexemType(blk, currentListener))))
                            select blk;

            if (selblocks.Count() > 0 && !args.ExpandInformation.Text.Trim().EndsWith(";"))
            {
                var block = selblocks.ElementAt(0);
                bool blockStatus = false;
                if (CheckCommentBlock(args.ExpandInformation))
                {
                    if (block.IsCollapsible)
                    {
                        if (!block.IsPreprocessor && args.Source.IndexOf(args.ExpandInformation) + 1 < args.Source.Count)
                        {
                            var tempLine = args.Source[args.Source.IndexOf(args.ExpandInformation) + 1];
                            if (CheckBlockStart(args.Source.IndexOf(args.ExpandInformation) + 1, args.Source))
                            {
                                createListener = true;
                            }
                            else
                            {
                                createListener = false;
                            }
                        }
                        else if (block.IsPreprocessor)
                        {
                            createListener = true;
                        }
                    }
                    else
                    {
                        var tempLine = args.Source[args.Source.IndexOf(args.ExpandInformation) + 1];
                        bool blockstartStatus = CheckBlockStart(args.Source.IndexOf(args.ExpandInformation) + 1, args.Source);
                        if (blockstartStatus)
                        {
                            blockStatus = true;
                            ignoreEndBlock = true;
                        }
                        else
                        {
                            blockStatus = false;
                            ignoreEndBlock = false;
                        }
                    }

                    if (block.IsIndent)
                    {
                        if (this.IndentableBlocks == null)
                        {
                            this.IndentableBlocks = new List<BlockListener>();
                        }

                        var tempBlock = new BlockListener()
                        {
                            BlockStart = block.BlockStart,
                            BlockEnd = block.BlockEnd,
                            IsPreprocessor = block.IsPreprocessor,
                            ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1,
                            IsRegex = block.IsRegex,
                            CheckParentType = block.CheckParentType,
                            ParentLexemType = block.ParentLexemType,
                            LexemType = block.LexemType,
                            ScopeLevel = block.ScopeLevel,
                            IsCollapsible = block.IsCollapsible,
                            IsIndent = block.IsIndent,
                            StartLine = args.Source.IndexOf(args.ExpandInformation) + 2
                        };

                        if (createListener)
                        {
                            tempBlock.IgnoreEndBlock = false;
                        }
                        else if (blockStatus)
                        {
                            tempBlock.IgnoreEndBlock = false;
                        }
                        else
                        {
                            tempBlock.IgnoreEndBlock = true;
                        }

                        tempBlock.EndLineNumber = tempBlock.StartLine;
                        this.IndentableBlocks.Add(tempBlock);
                    }
                }
                if (createListener)
                {
                    if (currentListener != null)
                    {
                        args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;
                        args.ExpandInformation.StartLine = args.Source.IndexOf(args.ExpandInformation) + 2;

                        if (blocksStack == null)
                        {
                            blocksStack = new Stack<BlockListener>();
                        }
                        blocksStack.Push(currentListener);
                    }
                    currentListener = new BlockListener()
                    {
                        BlockStart = block.BlockStart,
                        BlockEnd = block.BlockEnd,
                        IsPreprocessor = block.IsPreprocessor,
                        ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1, //block.IsPreprocessor ? args.ExpandInformation.LineNumber : args.ExpandInformation.LineNumber - 1,
                        IsRegex = block.IsRegex,
                        CheckParentType = block.CheckParentType,
                        ParentLexemType = block.ParentLexemType,
                        LexemType = block.LexemType,
                        ScopeLevel = block.ScopeLevel,
                        IsCollapsible = block.IsCollapsible,
                        IsIndent = block.IsIndent,
                    };

                    if (!block.IsPreprocessor)
                    {
                        bool addDefinition = true;
                        var scopesIncluded = this.scopeDefinitions.Where(scope => scope.StartLine == currentListener.ParentLineNumber);
                        if (scopesIncluded.Count() > 0)
                        {
                            ScopeDefinition define = scopesIncluded.ElementAt(0);
                            if (define.Type != currentListener.ScopeLevel)
                            {
                                this.scopeDefinitions.Remove(define);
                                addDefinition = true;
                            }
                            else
                            {
                                addDefinition = false;
                            }
                        }
                        if (addDefinition)
                        {
                            var definition = this.CreateScopeDefinition(args, currentListener);
                            if (definition != null)
                            {
                                this.scopeDefinitions.Add(definition);
                            }
                        }
                    }

                    args.ExpandInformation.ContainsPreprocessor = block.IsPreprocessor;
                    args.ExpandInformation.PreprocessorText = block.IsPreprocessor ? args.ExpandInformation.Text.Trim().Substring(block.BlockStart.Length).Trim() : string.Empty;
                    args.ExpandInformation.ContainsLines = true;
                    int tempInd = args.Source.IndexOf(args.ExpandInformation);
                    args.ExpandInformation.StartLine = block.IsPreprocessor ? tempInd + 1 : tempInd + 2;
                    currentListener.StartLine = args.ExpandInformation.StartLine;
                }
                else if (currentListener != null)
                {
                    args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;
                    args.ExpandInformation.IsExpanded = true;
                    args.ExpandInformation.ContainsLines = false;
                }
                else
                {
                    args.ExpandInformation.ParentLineNumber = -1;
                    args.ExpandInformation.IsExpanded = true;
                    args.ExpandInformation.ContainsLines = false;
                }
            }
            else if (currentListener != null)
            {
                args.ExpandInformation.ContainsLines = false;
                args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;
                if (((currentListener.IsRegex && currentListener.BlockEnd != null && CheckRegexEnds(args.ExpandInformation.Text.Trim(), currentListener.BlockEnd)) || (!currentListener.IsRegex && currentListener.BlockEnd != null && args.ExpandInformation.Text.Trim().EndsWith(currentListener.BlockEnd))) && !ignoreEndBlock)
                {
                    var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];
                    parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                    var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                    if (indentListener.Count() > 0)
                    {
                        var indentBlock = indentListener.ElementAt(0);
                        indentBlock.IgnoreEndBlock = false;
                        indentBlock.EndLineNumber = parentLineItem.EndLine;
                    }
                    currentListener.EndLineNumber = parentLineItem.EndLine;
                    DocumentBlocks.Add(currentListener);
                    currentListener = null;
                    if (blocksStack == null)
                    {
                        blocksStack = new Stack<BlockListener>();
                    }
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                }
                else if (currentListener.BlockEnd == null)
                {
                    currentListener = null;
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                }
                else if (currentListener.BlockEnd != null && args.ExpandInformation.Text.Trim().EndsWith(currentListener.BlockEnd) && ignoreEndBlock)
                {
                    ignoreEndBlock = false;
                }
            }
            else
            {
                args.ExpandInformation.ContainsLines = false;
                args.ExpandInformation.ParentLineNumber = -1;

                if (!args.ExpandInformation.IsExpanded)
                {
                    args.ExpandInformation.IsExpanded = true;
                    args.ExpandInformation.ToggleExpansion = true;
                }
                args.ExpandInformation.IsExpanded = true;
                args.ExpandInformation.ContainsLines = false;
            }
        }

        private bool CheckRegexEnds(string input, string pattern)
        {
            Match match = Regex.Match(input, pattern);
            if (match != null && match.Success)
            {
                return match.Index + 1 == input.TrimEnd().Length;
            }
            return false;
        }

        internal bool CheckCommentBlock(LineItemExpandInformation item)
        {
            if (item.LineStartBlock != null && item.LineStartBlock.LexemType == EditTokenType.Comment)
            {
                return false;
            }

            if (commentsCol != null)
            {
                LineItemExpandInformation tempItem = item;
                RegexOptions options = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                var blocks = commentsCol.Where(lexem => (!lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText)) || (lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText) &&
                    (tempItem.Text.IndexOf(lexem.EndText) == -1 || tempItem.Text.IndexOf(lexem.EndText) == tempItem.Text.Length - lexem.EndText.Length)));
                if (blocks.Count() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        internal bool CheckParentLexemType(BlockListener blk, BlockListener currentListener)
        {
            if (blk.CheckParentType && currentListener != null)
            {
                return blk.ParentLexemType == currentListener.LexemType;
            }
            else if (blk.CheckParentType)
            {
                return false;
            }
            return true;
        }

        private bool CheckRegexMatch(string text, string pattern)
        {
            return Regex.IsMatch(text.Trim(), pattern);
        }

        internal override List<BlockListener> GetBlockStartList()
        {
            List<BlockListener> returnlist = new List<BlockListener>();
            if (this.Lexem != null)
            {
                foreach (Lexem prep in this.Lexem.OfType<ILexem>().Where<ILexem>(lexem => lexem.LexemType == EditTokenType.Preprocessor || lexem.LexemType == EditTokenType.CodeSnippet || lexem.LexemType == EditTokenType.Property))
                {
                    returnlist.Add(new BlockListener()
                    {
                        BlockStart = prep.StartText,
                        BlockEnd = prep.EndText,
                        IsPreprocessor = prep.LexemType == EditTokenType.Preprocessor,
                        IsRegex = prep.IsRegex,
                        CheckParentType = prep.CheckParentLexemType,
                        LexemType = prep.LexemType,
                        ParentLexemType = prep.ParentLexemType,
                        ScopeLevel = prep.ScopeLevel,
                        IsIndent = prep.Indent,
                        IsCollapsible = prep.IsCollapsible,
                        EndBlockOnRecurrence = prep.EndBlockOnRecurrence
                    });
                }
            }
            return returnlist;
        }

        private bool CheckBlockStart(int line, List<LineItemExpandInformation> collection)
        {
            while (line < collection.Count)
            {
                var lineItem = collection[line];
                if ((lineItem.Text.Trim() != string.Empty || (line > 0 && collection[line - 1].Text.Trim() != string.Empty)) && (lineItem.Text.Trim().StartsWith(this.BlockStart) || (line > 0 && collection[line - 1].Text.Trim().Contains(this.BlockStart))))
                {
                    return true;
                }
                else if (lineItem.Text.Trim() == string.Empty)
                {
                    line += 1;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        #endregion Expand Collapse

        #region Intellisense

        internal virtual ScopeDefinition CreateScopeDefinition(ApplyExpandCollapseArgs args, BlockListener currentListener)
        {
            ScopeDefinition definition = new ScopeDefinition();
            definition.Type = currentListener.ScopeLevel;
            definition.StartLine = currentListener.ParentLineNumber;
            LineItemExpandInformation item = args.ExpandInformation;
            definition.StartItem = item;
            switch (currentListener.ScopeLevel)
            {
                case ScopeLevel.None:
                    break;

                case ScopeLevel.Namespace:
                    int index = item.Text.ToLower().IndexOf("namespace") + 9;
                    string tempStr = item.Text.Substring(index).TrimEnd(';');
                    definition.ScopeName = tempStr;
                    break;

                case ScopeLevel.Class:
                    this.InitializeIncludedNamespace(definition, definition.StartLine, args);
                    this.UpdateTypesForIncludedNamespaces(args.Assemblies);
                    break;

                case ScopeLevel.Member:
                    break;

                case ScopeLevel.StaticMember:
                    break;

                default:
                    break;
            }
            return definition;
        }

        internal override void OnIntellisenseBoxOpening(EditIntellisenseArgs args)
        {
            if (this.ParentControl.IntellisenseMode == IntellisenseMode.Auto && this.SupportsIntellisense)
            {
                if (args.CurrentScope == null)
                {
                    this.ApplyItemsSourceForNullScope(args);
                }
                else
                {
                    if (args.CurrentScope.Type == ScopeLevel.Class || args.CurrentScope.Type == ScopeLevel.Member || args.CurrentScope.Type == ScopeLevel.None)
                    {
                        ApplyItemsSourceForClassScope(args);
                    }
                }
            }
            else
            {
                base.OnIntellisenseBoxOpening(args);
            }
        }

        internal override void OnDrillDownIntellisense(EditIntellisenseArgs args)
        {
            if (this.ParentControl.IntellisenseMode == IntellisenseMode.Auto && this.SupportsIntellisense)
            {
                if (args.SelectedItem == null && args.CursorIndex > 0)
                {
                    string tempString = GetPreviousString(args.CursorIndex);
                    if (tempString != string.Empty)
                    {
                        int ind = tempString.LastIndexOf(this.IntellisenseDrillDownChar);
                        int indexEndLine = this.ParentControl.Lines[args.LineIndex].Text.IndexOf(';');
                        if (indexEndLine == -1 || (indexEndLine > 0 && indexEndLine > args.CursorIndex))
                        {
                            IIntellisenseItem selectedItem = this.GetTypeFromString(this.TypesCollection, tempString);

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
                }

                if (args.SelectedItem == null)
                {
                    this.HideIntellisensePopup();
                    return;
                }

                if (args.CurrentScope == null || (args.CurrentScope != null && args.CurrentScope.Type == ScopeLevel.Namespace))
                {
                    if (args.SelectedItem != null)
                    {
                        IIntellisenseItem selectedItem = args.SelectedItem;
                        if (selectedItem != null && selectedItem.NestedItems != null && selectedItem.NestedItems.Count() > 0)
                        {
                            args.ItemsSource = selectedItem.NestedItems.Where(type => (type as EditTypeInfo).IsNamespace);
                        }
                        else
                        {
                            args.ItemsSource = null;
                        }
                    }
                }
                else
                {
                    if ((args.SelectedItem as EditTypeInfo).IsClass)
                    {
                        defaultItems.Clear();
                        EditTypeInfo infoObj = args.SelectedItem as EditTypeInfo;
                        if (infoObj.NestedItems != null && infoObj.NestedItems.Count() > 0)
                        {
                            customItems = new EditTypeCollection(infoObj.NestedItems);
                        }
                        else
                        {
                            var infoItems = InitializeTypeInfoMembers(args);
                            if (infoItems != null)
                            {
                                customItems = new EditTypeCollection(infoItems);
                            }
                        }
                    }
                    else if ((args.SelectedItem as EditTypeInfo).IsInstance || (args.SelectedItem as EditTypeInfo).IsProperty)
                    {
                        defaultItems.Clear();
                        EditTypeInfo infoObj = args.SelectedItem as EditTypeInfo;
                        if (infoObj.NestedItems != null && infoObj.NestedItems.Count() > 0)
                        {
                            customItems = new EditTypeCollection(infoObj.NestedItems);
                        }
                        else
                        {
                            var infoItems = InitializeInstanceMembers(args);
                            if (infoItems != null)
                            {
                                customItems = new EditTypeCollection(infoItems);
                                infoObj.NestedItems = infoItems;
                            }
                        }
                    }

                    args.ItemsSource = customItems.OrderBy(item => item.Text);
                }
            }
            else
            {
                base.OnDrillDownIntellisense(args);
            }

            this.ParentControl.RaiseIntellisenseDrillDownEvent(args);
            if (!args.Cancel && args.ItemsSource != null && args.ItemsSource.Count() > 0)
            {
                this.ParentControl.intellisenseBox.ItemsSource = args.ItemsSource;
                this.PositionIntellisenseBox(args);
            }
            else
            {
                this.HideIntellisensePopup();
            }
        }

        private void ApplyItemsSourceForNullScope(EditIntellisenseArgs args)
        {
            if (args.CursorIndex > 0)
            {
                var tempString = GetPreviousString(args.CursorIndex);
                if (tempString != string.Empty)
                {
                    int ind = tempString.LastIndexOf(this.IntellisenseDrillDownChar);
                    if (ind > 0)
                    {
                        IIntellisenseItem selectedItem = this.GetTypeFromString(this.TypesCollection, tempString.Substring(0, ind).TrimEnd(this.IntellisenseDrillDownChar));
                        if (selectedItem != null)
                        {
                            args.ItemsSource = selectedItem.NestedItems;
                            return;
                        }
                    }
                }
            }

            if (args.SelectedItem == null)
            {
                if (this.Lexem.OfType<ILexem>().Count() > 0)
                {
                    var lexemItems = from lex in this.Lexem.OfType<ILexem>()
                                     where ((args.CurrentScope == null || args.CurrentScope != null && args.CurrentScope.Type == ScopeLevel.Namespace) &&
                                     (!lex.ExcludeItemInIntellisense && (lex.LexemType == EditTokenType.Preprocessor || lex.LexemType == EditTokenType.NamespaceDeclaration || lex.LexemType == EditTokenType.CodeSnippet)))
                                     select new EditTypeInfo() { Name = lex.IntellisenseDisplayText, IsLexem = true, LexemType = lex.LexemType };

                    defaultItems = new EditTypeCollection(lexemItems.OfType<IIntellisenseItem>());
                }

                if (args.CurrentScope == null && this.TypesCollection != null)
                {
                    ILexem namespaceLexem = this.GetNamespaceDeclarationLexem(this.Lexem.OfType<ILexem>());
                    if (namespaceLexem != null)
                    {
                        var lineitem = this.ParentControl.Lines[args.LineIndex];
                        string leadingText = lineitem.Text.Substring(0, args.CursorIndex);
                        if (LineContainsNamespaceDeclaration(args.CurrentScope, leadingText))
                        {
                            var namespaces = this.TypesCollection.Where(item => (item as EditTypeInfo).IsNamespace);
                            foreach (EditTypeInfo info in namespaces)
                            {
                                defaultItems.Add(info);
                            }
                        }
                    }
                }

                args.ItemsSource = defaultItems.OfType<IIntellisenseItem>().OrderBy(item => item.Text);
            }
        }

        private void ApplyItemsSourceForClassScope(EditIntellisenseArgs args)
        {
            if (args.SelectedItem == null)
            {
                List<IIntellisenseItem> collection = new List<IIntellisenseItem>();

                IEnumerable<IIntellisenseItem> classItems = this.InitializeCustomItemsForClassScope(args);
                if (classItems != null)
                {
                    collection.AddRange(classItems);
                }

                foreach (IIntellisenseItem item in collection)
                {
                    if (!typesCollection.ContainsType(item.Text))
                    {
                        typesCollection.Add(item);
                    }
                }

                if (customItems == null)
                {
                    customItems = new EditTypeCollection();
                }

                var items = IdentifyMembers(args.CurrentScope, args.LineIndex);
                if (items != null)
                {
                    instancesCollection = new EditTypeCollection();
                    foreach (EditTypeInfo info in items)
                    {
                        var existingItem = customItems.GetEditTypeInfo(info.Name) as EditTypeInfo;
                        if (existingItem == null || (existingItem != null && !existingItem.IsInstance))
                        {
                            instancesCollection.Add(info);
                        }
                        if (!collection.Contains(info as IIntellisenseItem))
                        {
                            collection.Add(info);
                        }
                    }
                }

                customItems = new EditTypeCollection(collection);

                if (this.Lexem.OfType<ILexem>().Count() > 0)
                {
                    var lexemItems = from lex in this.Lexem.OfType<ILexem>()
                                     where !lex.ExcludeItemInIntellisense && (lex.LexemType == EditTokenType.Keyword || lex.LexemType == EditTokenType.CodeSnippet || lex.LexemType == EditTokenType.Preprocessor)
                                     select new EditTypeInfo() { Name = lex.IntellisenseDisplayText, IsLexem = true, LexemType = lex.LexemType };

                    defaultItems = new EditTypeCollection(lexemItems.OfType<IIntellisenseItem>());
                    collection.AddRange(lexemItems.OfType<IIntellisenseItem>());
                }

                args.ItemsSource = collection.OrderBy(item => item.Text);
            }
        }

        private IEnumerable<IIntellisenseItem> InitializeCustomItemsForClassScope(EditIntellisenseArgs args)
        {
            if (this.includedNamespaces != null)
            {
                List<IIntellisenseItem> typesList = new List<IIntellisenseItem>();
                foreach (string str in includedNamespaces)
                {
                    IEnumerable<IIntellisenseItem> types = this.GetTypesFromString(this.TypesCollection, str);
                    if (types != null)
                    {
                        typesList.AddRange(types);
                    }
                }
                return new EditTypeCollection(typesList);
            }
            return null;
        }

        internal virtual List<EditTypeInfo> IdentifyMembers(ScopeDefinition definition, int line)
        {
            var scopes = this.scopeDefinitions.Where(scope => scope.StartLine < line && scope.EndLine >= line);
            var excludedScopes = this.scopeDefinitions.Where(scope => scope.StartLine < definition.StartLine && scope.EndLine < definition.StartLine);
            List<EditTypeInfo> membersList = new List<EditTypeInfo>();
            if (definition.Type == ScopeLevel.Member || definition.Type == ScopeLevel.None)
            {
                if (scopes.Count() > 0)
                {
                    ScopeDefinition classScope = null;

                    var classscopes = scopes.Where(scope => scope.Type == ScopeLevel.Class);
                    if (classscopes.Count() > 0)
                    {
                        classScope = classscopes.ElementAt(0);
                    }

                    //if (classScope == null)
                    //    return null;

                    List<int> searchableLines = new List<int>();
                    if (classScope != null)
                    {
                        searchableLines = GetLinesInScope(classScope, excludedScopes, line);
                    }
                    else
                    {
                        searchableLines = GetLinesInScope(definition, excludedScopes, line);
                    }

                    foreach (int i in searchableLines)
                    {
                        var item = this.ParentControl.Lines[i];
                        Match match = Regex.Match(item.Text, @"(?'type'[\w]+)\s(?'name'[\w]+);|(?'type'[\w]+)\s(?'name'[\w]+)\s+=");
                        if (match.Success && CheckCommentBlock(item.GetLineItemExpandDetails()))
                        {
                            EditTypeInfo info = new EditTypeInfo()
                            {
                                Name = match.Groups[2].Value,
                                BaseType = GetBaseType(match.Groups[1].Value) as EditTypeInfo,
                                IsInstance = true
                            };
                            membersList.Add(info);
                        }
                    }
                }
            }
            return membersList;
        }

        internal List<int> GetLinesInScope(ScopeDefinition classScope, IEnumerable<ScopeDefinition> excludedScopes, int endLine)
        {
            List<int> linesSearchable = new List<int>();
            var excludedLines = excludedScopes.OrderBy(scope => scope.StartLine);
            for (int i = classScope.StartLine; i <= endLine; i++)
            {
                var findItem = excludedLines.Where(scope => scope.StartLine == i);
                if (findItem.Count() > 0)
                {
                    i = findItem.ElementAt(0).EndLine;
                }
                else
                {
                    linesSearchable.Add(i);
                }
            }
            return linesSearchable;
        }

        internal void InitializeIncludedNamespace(ScopeDefinition definition, int line, ApplyExpandCollapseArgs args)
        {
            if (definition.StartLine > 0)
            {
                ILexem namespaceLexem = this.GetNamespaceDeclarationLexem(args.Lexems);
                if (namespaceLexem != null)
                {
                    ScopeDefinition parentScope = this.GetNamespaceScopeDefinition(definition.StartLine - 1);
                    if (parentScope != null && parentScope.Type == ScopeLevel.Namespace)
                    {
                        IEnumerable<LineItemExpandInformation> namespacelines = null;
                        RegexOptions option = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                        if (this.CaseSensitive)
                        {
                            namespacelines = args.Source.Where(item => ((args.Source.IndexOf(item) < parentScope.StartLine - 1) ||
                                 (args.Source.IndexOf(item) > parentScope.StartLine - 1 && args.Source.IndexOf(item) < definition.StartLine - 1))
                                 && ((namespaceLexem.IsRegex && Regex.Match(item.Text, namespaceLexem.StartText, option).Success) ||
                                 (!namespaceLexem.IsRegex && item.Text.Trim().StartsWith(namespaceLexem.StartText))));
                        }
                        else
                        {
                            namespacelines = args.Source.Where(item => ((args.Source.IndexOf(item) < parentScope.StartLine - 1) ||
                                 (args.Source.IndexOf(item) > parentScope.StartLine - 1 && args.Source.IndexOf(item) < definition.StartLine - 1))
                                 && ((namespaceLexem.IsRegex && Regex.Match(item.Text, namespaceLexem.StartText, option).Success) ||
                                 (!namespaceLexem.IsRegex && item.Text.ToLower().Trim().StartsWith(namespaceLexem.StartText.ToLower()))));
                        }
                        if (namespacelines.Count() > 0)
                        {
                            this.includedNamespaces = includedNamespaces == null ? new List<string>() : this.includedNamespaces;

                            if (!includedNamespaces.Contains(parentScope.ScopeName.Trim()))
                            {
                                includedNamespaces.Add(parentScope.ScopeName.Trim());
                            }

                            foreach (LineItemExpandInformation str in namespacelines)
                            {
                                string tempStr = string.Empty;
                                int index = 0;
                                if (namespaceLexem.IsRegex)
                                {
                                    Match match = Regex.Match(str.Text, namespaceLexem.StartText, option);
                                    index = match.Index + match.Value.Length;
                                }
                                else
                                {
                                    if (this.CaseSensitive)
                                    {
                                        index = str.Text.IndexOf(namespaceLexem.StartText) + namespaceLexem.StartText.Length;
                                    }
                                    else
                                    {
                                        index = str.Text.ToLower().IndexOf(namespaceLexem.StartText.ToLower()) + namespaceLexem.StartText.Length;
                                    }
                                }
                                tempStr = str.Text.Substring(index).TrimEnd(';').Trim();
                                if (tempStr != string.Empty && !includedNamespaces.Contains(tempStr))
                                {
                                    includedNamespaces.Add(tempStr);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void InitializeIncludedNamespace(ScopeDefinition definition, int line)
        {
            if (definition.StartLine > 0)
            {
                ILexem namespaceLexem = this.GetNamespaceDeclarationLexem(this.Lexem.OfType<ILexem>());
                if (namespaceLexem != null)
                {
                    ScopeDefinition parentScope = this.GetNamespaceScopeDefinition(definition.StartLine - 1);
                    if (parentScope != null && parentScope.Type == ScopeLevel.Namespace)
                    {
                        IEnumerable<LineItem> namespacelines = null;
                        RegexOptions option = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                        if (this.CaseSensitive)
                        {
                            namespacelines = this.ParentControl.Lines.Where(item => ((this.ParentControl.Lines.IndexOf(item) < parentScope.StartLine - 1) ||
                                 (this.ParentControl.Lines.IndexOf(item) > parentScope.StartLine - 1 && this.ParentControl.Lines.IndexOf(item) < definition.StartLine - 1))
                                 && ((namespaceLexem.IsRegex && Regex.Match(item.Text, namespaceLexem.StartText, option).Success) ||
                                 (!namespaceLexem.IsRegex && item.Text.Trim().StartsWith(namespaceLexem.StartText))));
                        }
                        else
                        {
                            namespacelines = this.ParentControl.Lines.Where(item => ((this.ParentControl.Lines.IndexOf(item) < parentScope.StartLine - 1) ||
                                 (this.ParentControl.Lines.IndexOf(item) > parentScope.StartLine - 1 && this.ParentControl.Lines.IndexOf(item) < definition.StartLine - 1))
                                 && ((namespaceLexem.IsRegex && Regex.Match(item.Text, namespaceLexem.StartText, option).Success) ||
                                 (!namespaceLexem.IsRegex && item.Text.ToLower().Trim().StartsWith(namespaceLexem.StartText.ToLower()))));
                        }
                        if (namespacelines.Count() > 0)
                        {
                            this.includedNamespaces = includedNamespaces == null ? new List<string>() : this.includedNamespaces;

                            if (!includedNamespaces.Contains(parentScope.ScopeName.Trim()))
                            {
                                includedNamespaces.Add(parentScope.ScopeName.Trim());
                            }

                            foreach (LineItem str in namespacelines)
                            {
                                string tempStr = string.Empty;
                                int index = 0;
                                if (namespaceLexem.IsRegex)
                                {
                                    Match match = Regex.Match(str.Text, namespaceLexem.StartText, option);
                                    index = match.Index + match.Value.Length;
                                }
                                else
                                {
                                    if (this.CaseSensitive)
                                    {
                                        index = str.Text.IndexOf(namespaceLexem.StartText) + namespaceLexem.StartText.Length;
                                    }
                                    else
                                    {
                                        index = str.Text.ToLower().IndexOf(namespaceLexem.StartText.ToLower()) + namespaceLexem.StartText.Length;
                                    }
                                }
                                tempStr = str.Text.Substring(index).TrimEnd(';').Trim();
                                if (tempStr != string.Empty && !includedNamespaces.Contains(tempStr))
                                {
                                    includedNamespaces.Add(tempStr);
                                }
                            }
                        }
                    }
                }
            }
        }

        private ILexem GetNamespaceDeclarationLexem(IEnumerable<ILexem> lexems)
        {
            ILexem namespaceLexem = null;
            if (lexems != null && lexems.Count() > 0)
            {
                var lexemItems = lexems.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.NamespaceDeclaration);
                if (lexemItems.Count() > 0)
                {
                    namespaceLexem = lexemItems.ElementAt(0);
                }
            }
            return namespaceLexem;
        }

        private IEnumerable<IIntellisenseItem> InitializeInstanceMembers(EditIntellisenseArgs args)
        {
            EditTypeInfo info = (args.SelectedItem as EditTypeInfo).BaseType;
            if (info == null)
                return new EditTypeCollection();

            string typeName = info.Namespace + "." + info.Name;
            List<IIntellisenseItem> infoList = new List<IIntellisenseItem>();
            if (args.Assemblies != null)
            {
                foreach (Uri uri in args.Assemblies)
                {
                    Assembly assembly = Assembly.LoadFile(uri.LocalPath);
                    var types = assembly.GetTypes().Where(type => type.FullName == typeName);
                    if (types.Count() > 0)
                    {
                        Type selectedType = types.ElementAt(0);

                        var properties = selectedType.GetProperties().Select(item => new EditTypeInfo()
                        {
                            Name = item.Name,
                            BaseType = item.PropertyType != null ? this.GetTypeFromString(this.TypesCollection, item.PropertyType.FullName) as EditTypeInfo : null,
                            IsStatic = true,
                            IsPublic = true,
                            IsProperty = true
                        });
                        infoList.AddRange(properties.OfType<IIntellisenseItem>());

                        var methods = selectedType.GetMethods().Where(method => method.IsPublic && !method.IsSpecialName).Distinct().Select(item => new EditTypeInfo()
                        {
                            Name = item.Name,
                            BaseType = item.ReturnType != null ? this.GetTypeFromString(this.TypesCollection, item.ReturnType.FullName) as EditTypeInfo : null,
                            IsStatic = true,
                            IsPublic = true,
                            IsMethod = true
                        });

                        foreach (EditTypeInfo type in methods)
                        {
                            var items = infoList.Where(typeItem => (typeItem as EditTypeInfo).IsMethod && typeItem.Text == type.Name);
                            if (items.Count() == 0)
                            {
                                infoList.Add(type);
                            }
                        }

                        var events = selectedType.GetEvents().Where(eventItem => !eventItem.IsSpecialName).Select(item => new EditTypeInfo()
                        {
                            Name = item.Name,
                            BaseType = item.EventHandlerType != null ? this.GetBaseTypeFromFullName(item.EventHandlerType.FullName) as EditTypeInfo : null,
                            IsStatic = true,
                            IsPublic = true,
                            IsEvent = true
                        });
                        infoList.AddRange(events.OfType<IIntellisenseItem>());
                    }
                }
                info.NestedItems = new EditTypeCollection(infoList);
            }
            return info.NestedItems;
        }

        private IEnumerable<IIntellisenseItem> InitializeTypeInfoMembers(EditIntellisenseArgs args)
        {
            EditTypeInfo info = args.SelectedItem as EditTypeInfo;
            string typeName = info.Namespace + "." + info.Name;
            List<IIntellisenseItem> infoList = new List<IIntellisenseItem>();
            if (args.Assemblies != null)
            {
                foreach (Uri uri in args.Assemblies)
                {
                    Assembly assembly = Assembly.LoadFile(uri.LocalPath);
                    var types = assembly.GetTypes().Where(type => type.FullName == typeName);
                    if (types.Count() > 0)
                    {
                        Type selectedType = types.ElementAt(0);

                        var properties = selectedType.GetFields().Select(item => new EditTypeInfo()
                        {
                            Name = item.Name,
                            BaseType = item.FieldType != null ? this.GetBaseTypeFromFullName(item.FieldType.FullName) as EditTypeInfo : null,
                            IsStatic = true,
                            IsPublic = true,
                            IsInstance = true
                        });
                        infoList.AddRange(properties.OfType<IIntellisenseItem>());
                    }
                }
                info.NestedItems = infoList;
            }
            return info.NestedItems;
        }

        #endregion Intellisense

        #endregion Implementation
    }
}