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
using System.Text.RegularExpressions;
using System.Windows;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class VBLanguage : ProceduralLanguageBase
    {
        private BlockListener prevListener = null;

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="control"></param>
        public VBLanguage(EditControl control)
            : base(control)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("/Syncfusion.Edit.Wpf;component/Languages/LanguageResources.xaml", UriKind.Relative);
            IsSplitTextToWords = true;
            this.Lexem = dictionary["VBLexems"] as LexemCollection;
            this.Formats = dictionary["VBFormats"] as FormatsCollection;
            this.Name = "Visual Basic";
            this.FileExtension = ".vb";
            this.TextForeground = control.Foreground;
            this.BlockStart = "{";
            this.BlockEnd = "}";
            this.ApplyColoring = true;
            this.CaseSensitive = false;
            this.SupportsOutlining = true;
            this.SupportsIntellisense = true;
        }

        #endregion Constructor

        #region Overrides

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
                            where (((args.ExpandInformation.Text.Trim().ToLower().StartsWith(blk.BlockStart.ToLower()) && blk.IsPreprocessor) || (args.ExpandInformation.Text.Trim().ToLower() == blk.BlockStart.ToLower())
                            || (blk.IsRegex && CheckRegexMatchIgnoreCase(args.ExpandInformation.Text, blk.BlockStart) && CheckParentLexemType(blk, currentListener))))
                            select blk;
            if (selblocks.Count() > 0)
            {
                var block = selblocks.ElementAt(0);
                if (CheckCommentBlock(args.ExpandInformation))
                {
                    if (!block.IsPreprocessor && args.Source.IndexOf(args.ExpandInformation) + 1 < args.Source.Count)
                    {
                        createListener = true;
                    }
                    else if (block.IsPreprocessor)
                    {
                        createListener = true;
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
                            IgnoreEndBlock = block.EndBlockOnRecurrence,
                            EndBlockOnRecurrence = block.EndBlockOnRecurrence
                        };

                        this.IndentableBlocks.Add(tempBlock);
                    }
                }
                if (createListener)
                {
                    if (currentListener != null)
                    {
                        args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;
                        args.ExpandInformation.StartLine = args.Source.IndexOf(args.ExpandInformation) + 2;
                        if (currentListener.EndBlockOnRecurrence && block.EndBlockOnRecurrence && block.BlockStart == currentListener.BlockStart && block.BlockEnd == currentListener.BlockEnd)
                        {
                            var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];
                            parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation);
                            var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                            if (indentListener.Count() > 0)
                            {
                                var indentBlock = indentListener.ElementAt(0);
                                indentBlock.EndLineNumber = parentLineItem.EndLine;
                            }
                            currentListener.EndLineNumber = parentLineItem.EndLine;
                            if (!currentListener.IsCollapsible)
                            {
                                parentLineItem.ContainsLines = false;
                            }
                            currentListener = null;
                        }
                        else
                        {
                            if (blocksStack == null)
                            {
                                blocksStack = new Stack<BlockListener>();
                            }
                            prevListener = currentListener;
                            blocksStack.Push(currentListener);
                        }
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
                        EndBlockOnRecurrence = block.EndBlockOnRecurrence,
                        IgnoreEndBlock = !block.EndBlockOnRecurrence
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
                if (currentListener.BlockEnd != null && args.ExpandInformation.Text.Trim().EndsWith(currentListener.BlockEnd))
                {
                    var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];
                    parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                    var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                    if (indentListener.Count() > 0)
                    {
                        var indentBlock = indentListener.ElementAt(0);
                        indentBlock.EndLineNumber = parentLineItem.EndLine;
                    }
                    currentListener.EndLineNumber = parentLineItem.EndLine;
                    if (!currentListener.IsCollapsible)
                    {
                        parentLineItem.ContainsLines = false;
                    }
                    DocumentBlocks.Add(currentListener);
                    currentListener = null;
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                }
                else if (currentListener.BlockEnd == null && currentListener.EndBlockOnRecurrence && prevListener != null && args.ExpandInformation.Text.Trim().EndsWith(prevListener.BlockEnd))
                {
                    var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];
                    parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation);
                    var indentListener = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                    if (indentListener.Count() > 0)
                    {
                        var indentBlock = indentListener.ElementAt(0);
                        indentBlock.EndLineNumber = parentLineItem.EndLine;
                    }
                    currentListener.EndLineNumber = parentLineItem.EndLine;
                    if (!currentListener.IsCollapsible)
                    {
                        parentLineItem.ContainsLines = false;
                    }
                    DocumentBlocks.Add(currentListener);
                    currentListener = null;
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                    parentLineItem = args.Source[currentListener.ParentLineNumber - 1];
                    parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation) + 1;
                    var indentListener1 = this.IndentableBlocks.Where(item => item.Equals(currentListener));
                    if (indentListener1.Count() > 0)
                    {
                        var indentBlock = indentListener.ElementAt(0);
                        indentBlock.EndLineNumber = parentLineItem.EndLine;
                    }
                    currentListener.EndLineNumber = parentLineItem.EndLine;
                    if (!currentListener.IsCollapsible)
                    {
                        parentLineItem.ContainsLines = false;
                    }

                    currentListener = null;
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                }
                else if (currentListener.BlockEnd == null && !currentListener.EndBlockOnRecurrence)
                {
                    currentListener = null;
                    if (blocksStack.Count > 0)
                    {
                        currentListener = blocksStack.Pop();
                    }
                }
            }
            else
            {
                args.ExpandInformation.ContainsLines = false;
                //args.ExpandInformation.ParentLineNumber = -1;

                if (!args.ExpandInformation.IsExpanded)
                {
                    args.ExpandInformation.IsExpanded = true;
                    args.ExpandInformation.ToggleExpansion = true;
                }
                args.ExpandInformation.IsExpanded = true;
                args.ExpandInformation.ContainsLines = false;
            }
        }

        internal override bool CheckBlockStarts(LineItem tempLineItem)
        {
            return tempLineItem.ContainsLines;
        }

        internal override bool CheckBlockEnds(LineItem tempLineItem)
        {
            int parentLineNo = tempLineItem.ParentLineNumber;
            if (parentLineNo > 0)
            {
                var parentItem = this.ParentControl.Lines[parentLineNo - 1];
                return (parentItem.EndLine == tempLineItem.LineNumber);
            }
            return false;
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
            return indentableLines.Count();
        }

        #endregion Overrides

        #region Implementation

        private bool CheckRegexMatchIgnoreCase(string text, string pattern)
        {
            RegexOptions options = RegexOptions.IgnoreCase;
            return Regex.IsMatch(text.Trim(), pattern, options);
        }

        internal override ScopeDefinition CreateScopeDefinition(ApplyExpandCollapseArgs args, BlockListener currentListener)
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
                    string tempStr = item.Text.Substring(index);
                    definition.ScopeName = tempStr;
                    break;

                case ScopeLevel.Class:
                    //this.InitializeIncludedNamespace(definition, definition.StartLine, args);
                    //this.UpdateTypesForIncludedNamespaces(args.Assemblies);
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

        internal override List<EditTypeInfo> IdentifyMembers(ScopeDefinition definition, int line)
        {
            var scopes = this.scopeDefinitions.Where(scope => scope.StartLine < line && scope.EndLine >= line);
            var excludedScopes = this.scopeDefinitions.Where(scope => scope.StartLine < definition.StartLine && scope.EndLine < definition.StartLine);
            List<EditTypeInfo> membersList = new List<EditTypeInfo>();
            if (definition.Type == ScopeLevel.Member || definition.Type == ScopeLevel.None)
            {
                if (scopes.Count() > 0)
                {
                    ScopeDefinition classScope = scopes.Where(scope => scope.Type == ScopeLevel.Class).ElementAt(0);
                    List<int> searchableLines = GetLinesInScope(classScope, excludedScopes, line);
                    foreach (int i in searchableLines)
                    {
                        var item = this.ParentControl.Lines[i];
                        Match match = Regex.Match(item.Text, @"(Dim (?'name'\w+) as New (?'type'\w+))|(Dim (?'name'\w+) as (?'type'\w+))", RegexOptions.IgnoreCase);
                        if (match.Success && CheckCommentBlock(item.GetLineItemExpandDetails()))
                        {
                            EditTypeInfo info = new EditTypeInfo()
                            {
                                Name = match.Groups[3].Value,
                                BaseType = GetBaseType(match.Groups[4].Value) as EditTypeInfo,
                                IsInstance = true
                            };
                            membersList.Add(info);
                        }
                    }
                }
            }
            return membersList;
        }

        #endregion Implementation
    }
}