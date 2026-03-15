#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Resources;
using FontStyleEnum = System.Windows.FontStyles;
#else
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using FontStyleEnum = Windows.UI.Text.FontStyle;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public abstract class Inline : Node
    {
        #region Fields
        private List<ElementBox> elements;
        internal readonly static char[] WordSplitCharacters = new char[] { ' ', ',', '.', ':', ';', '<', '>', '=', '+', '-', '_', '{', '}', '[', ']', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')','"','\'','?','/','|','\\' };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        internal abstract int Length
        {
            get;
        }
        /// <summary>
        /// Gets or sets the character format.
        /// </summary>
        /// <value>
        /// The character format.
        /// </value>
        public CharacterFormat CharacterFormat
        {
            get
            {
                return (CharacterFormat)GetValue(CharacterFormatProperty);
            }
            set
            {
                SetValue(CharacterFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        internal List<ElementBox> Elements
        {
            get
            {
                return elements;
            }
        }
        /// <summary>
        /// Gets the owner paragraph.
        /// </summary>
        /// <value>
        /// The owner paragraph.
        /// </value>
        internal ParagraphAdv OwnerParagraph
        {
            get
            {
                return Owner as ParagraphAdv;
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the CharacterFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the CharacterFormat dependency property.</returns>
        internal static readonly DependencyProperty CharacterFormatProperty = DependencyProperty.Register("CharacterFormat", typeof(CharacterFormat), typeof(Inline), new PropertyMetadata(null, OnCharacterFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when character format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCharacterFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as CharacterFormat).SetOwner(d as Inline);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        public Inline()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal Inline(Node owner)
            : base(owner)
        {
            CharacterFormat = new CharacterFormat(this);
            elements = new List<ElementBox>();
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal abstract void Dispose();
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal abstract Inline Clone();
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal abstract void InsertText(SelectionAdv selection, string text, int index);
        /// <summary>
        /// Adds the element box.
        /// </summary>
        internal abstract void AddElementBox(LayoutViewer viewer);
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal abstract Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline);
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <param name="endtext">The endtext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal abstract Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline);
        #endregion

        #region Implementations
        /// <summary>
        /// Selects the word.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="index">The index.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        internal void SelectWord(SelectionAdv selection, int index, bool isCaretLeftToInline)
        {
            int startIndex = index;
            string starttext = "";
            Inline startInline = GetWordStartInline(ref startIndex, ref starttext, isCaretLeftToInline);

            int endIndex = index;
            string endtext = "";
            Inline endInline = GetWordEndInline(ref endIndex, ref endtext, isCaretLeftToInline);
            
            startInline = startInline.ValidateTextPosition(ref startIndex);
            TextPosition start = new TextPosition(selection.OwnerControl);
            double startOffset = startInline.OwnerParagraph.GetOffset(startInline, startIndex);
            start.SetPosition(startInline.OwnerParagraph, startOffset);

            endInline = endInline.ValidateTextPosition(ref endIndex);
            TextPosition end = new TextPosition(selection.OwnerControl);
            double endOffset = endInline.OwnerParagraph.GetOffset(endInline, endIndex);
            end.SetPosition(endInline.OwnerParagraph, endOffset);
            //Validates start and end position to select entire field.
            ValidateSelection(start, end);
            selection.Select(start, end);
        }
        /// <summary>
        /// Validates the selection.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        private void ValidateSelection(TextPosition start, TextPosition end)
        {
            string selectionStartIndex = start.GetHierarchicalIndex();
            string selectionEndIndex = end.GetHierarchicalIndex();
            if (selectionStartIndex != selectionEndIndex)
            {
                //Extends selection end to field begin or field end.
                if (TextPosition.IsForwardSelection(selectionStartIndex, selectionEndIndex))
                    end.ValidateForwardFieldSelection(selectionStartIndex, selectionEndIndex);
                else
                    end.ValidateBackwardFieldSelection(selectionStartIndex, selectionEndIndex);
            }
        }
        /// <summary>
        /// Gets the previous element box.
        /// </summary>
        /// <returns></returns>
        internal ElementBox GetPreviousElementBox()
        {
            Inline prevInline = this;
            while (prevInline != null && prevInline.Elements.Count == 0)
            {
                if (prevInline is FieldSeparatorAdv && (prevInline as FieldSeparatorAdv).IsLinkedFieldCharacter())
                    prevInline = (prevInline as FieldSeparatorAdv).FieldBegin;
                prevInline = prevInline.PreviousNode as Inline;
            }
            if (prevInline == null)
                return null;
            return prevInline.Elements.Count > 0 ? prevInline.Elements[prevInline.Elements.Count - 1] : null;
        }
        /// <summary>
        /// Determines whether the specified index is last rendered inline.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the specified index is last rendered inline; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsLastRenderedInline(int index)
        {
            Inline inline = this;
            while (index == inline.Length && inline.NextNode is FieldCharacterAdv)
            {
                Inline nextValidInline = (inline.NextNode as Inline).GetNextValidInline();
                index = 0;
                if (nextValidInline is FieldBeginAdv)
                    inline = nextValidInline;
                if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
                {
                    FieldBeginAdv fieldBegin = inline as FieldBeginAdv;
                    if (fieldBegin.FieldSeparator == null)
                    {
                        inline = fieldBegin.FieldEnd;
                        index = 1;
                    }
                    else
                    {
                        inline = fieldBegin.FieldSeparator;
                        ParagraphAdv paragraph = inline.OwnerParagraph;
                        index = 1;
                        if (paragraph == fieldBegin.FieldEnd.OwnerParagraph
                            && !paragraph.HasValidInline(inline, fieldBegin.FieldEnd))
                            inline = fieldBegin.FieldEnd;
                        else
                            break;
                    }
                }
            }
            return index == inline.Length && inline.NextNode == null;
        }
        /// <summary>
        /// Validates the text position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal Inline ValidateTextPosition(ref int index)
        {
            Inline inline = this;
            if (inline.Length == index && inline.NextNode is FieldCharacterAdv)
            {
                //If inline is last item within field, then set field end as text position.
                Inline nextInline = (inline.NextNode as FieldCharacterAdv).GetNextValidInline();
                if (nextInline is FieldEndAdv)
                {
                    inline = nextInline;
                    index = 1;
                }
            }
            else if (index == 0 && inline.PreviousNode is FieldCharacterAdv)
            {
                Inline prevInline = (inline.PreviousNode as Inline).GetPreviousValidInline();
                inline = prevInline;
                index = inline is FieldCharacterAdv ? 0 : inline.Length;
                if (inline is FieldEndAdv)
                    index++;
            }
            return inline;
        }
        /// <summary>
        /// Gets the previous rendered inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetPreviousRenderedInline()
        {
            Inline inline = this as Inline;
            Inline previousValidInline = null;
            while (inline is FieldCharacterAdv)
            {
                if ((inline as FieldCharacterAdv).IsLinkedFieldCharacter())
                {
                    if (inline is FieldBeginAdv)
                        previousValidInline = inline;
                    else if (inline is FieldEndAdv)
                    {
                        previousValidInline = inline;
                        if ((inline as FieldEndAdv).FieldSeparator == null)
                        {
                            inline = (inline as FieldEndAdv).FieldBegin;
                            previousValidInline = inline;
                        }
                    }
                    else
                    {
                        inline = (inline as FieldSeparatorAdv).FieldBegin;
                        previousValidInline = inline;
                    }
                }
                inline = inline.PreviousNode as Inline;
            }
            if (inline is FieldCharacterAdv || inline == null)
                return previousValidInline == null ? inline : previousValidInline;
            else
                return inline;
        }
        /// <summary>
        /// Gets the next rendered inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetNextRenderedInline()
        {
            Inline inline = this as Inline;
            Inline nextValidInline = null;
            while (inline is FieldCharacterAdv)
            {
                if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
                {
                    nextValidInline = (inline as FieldBeginAdv).GetRenderedField();
                    if (nextValidInline == inline)
                        return nextValidInline;
                }
                else if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
                    nextValidInline = inline;
                inline = inline.NextNode as Inline;
            }
            if (inline is FieldCharacterAdv || inline == null)
                return nextValidInline == null ? inline : nextValidInline;
            else
                return inline;
        }
        /// <summary>
        /// Gets the previous valid inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetPreviousValidInline()
        {
            Inline inline = this as Inline;
            Inline previousValidInline = null;
            while (inline is FieldCharacterAdv)
            {
                if ((inline as FieldCharacterAdv).IsLinkedFieldCharacter())
                {
                    if (inline is FieldBeginAdv)
                        previousValidInline = inline;
                    else if (inline is FieldEndAdv)
                    {
                        previousValidInline = inline;
                        if ((inline as FieldEndAdv).FieldSeparator == null)
                        {
                            inline = (inline as FieldEndAdv).FieldBegin;
                            previousValidInline = inline;
                        }
                    }
                    else
                    {
                        inline = (inline as FieldSeparatorAdv).FieldBegin;
                        previousValidInline = inline;
                    }
                }
                inline = inline.PreviousNode as Inline;
            }
            return previousValidInline == null ? inline : previousValidInline;
        }
        /// <summary>
        /// Gets the next valid inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetNextValidInline()
        {
            Inline inline = this as Inline;
            Inline nextValidInline = null;
            while (inline is FieldCharacterAdv)
            {
                if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
                    return nextValidInline == null ? inline : nextValidInline;
                else if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
                    nextValidInline = inline;
                inline = inline.NextNode as Inline;
            }
            return nextValidInline == null ? inline : nextValidInline;
        }
        /// <summary>
        /// Gets the next rendered inline.
        /// </summary>
        /// <param name="indexInInline">The index in inline.</param>
        /// <returns></returns>
        internal Inline GetNextRenderedInline(int indexInInline)
        {
            Inline inline = this;
            if (inline is FieldBeginAdv)
            {
                FieldBeginAdv fieldBegin = inline as FieldBeginAdv;
                inline = fieldBegin.GetRenderedField();
                if (fieldBegin == inline)
                    return fieldBegin;
                indexInInline = 1;
            }
            while (indexInInline == inline.Length && inline.NextNode is FieldCharacterAdv)
            {
                Inline nextValidInline = (inline.NextNode as Inline).GetNextValidInline();
                if (nextValidInline is FieldBeginAdv)
                {
                    FieldBeginAdv fieldBegin = nextValidInline as FieldBeginAdv;
                    inline = fieldBegin.GetRenderedField();
                    if (fieldBegin == inline)
                        return fieldBegin;
                    indexInInline = 1;
                }
                else
                    inline = nextValidInline;
            }
            return inline;
        }
        /// <summary>
        /// Determines whether this instance is exist before the specified inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <returns>
        ///   <c>true</c> if this instance is exist before the specified inline; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExistBefore(Inline inline)
        {
            if (OwnerParagraph == inline.OwnerParagraph)
                return OwnerParagraph.Inlines.IndexOf(this) < OwnerParagraph.Inlines.IndexOf(inline);
            if (OwnerParagraph.Owner == inline.OwnerParagraph.Owner)
            {
                if (OwnerParagraph.IsInsideTable)
                    return OwnerParagraph.AssociatedCell.Blocks.IndexOf(OwnerParagraph) < inline.OwnerParagraph.AssociatedCell.Blocks.IndexOf(inline.OwnerParagraph);
                else if (OwnerParagraph.Owner is HeaderFooter)
                    return (OwnerParagraph.Owner as HeaderFooter).Blocks.IndexOf(OwnerParagraph) < (inline.OwnerParagraph.Owner as HeaderFooter).Blocks.IndexOf(inline.OwnerParagraph);
                else
                    return OwnerParagraph.Section.Blocks.IndexOf(OwnerParagraph) < inline.OwnerParagraph.Section.Blocks.IndexOf(inline.OwnerParagraph);
            }

            return OwnerParagraph.IsExistBefore(inline.OwnerParagraph);
        }
        /// <summary>
        /// Determines whether this instance is exist after the specified inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <returns>
        ///   <c>true</c> if this instance is exist after the specified inline; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExistAfter(Inline inline)
        {
            if (OwnerParagraph == inline.OwnerParagraph)
                return OwnerParagraph.Inlines.IndexOf(this) > OwnerParagraph.Inlines.IndexOf(inline);
            if (OwnerParagraph.Owner == inline.OwnerParagraph.Owner)
            {
                if (OwnerParagraph.IsInsideTable)
                    return OwnerParagraph.AssociatedCell.Blocks.IndexOf(OwnerParagraph) > inline.OwnerParagraph.AssociatedCell.Blocks.IndexOf(inline.OwnerParagraph);
                else if (OwnerParagraph.Owner is HeaderFooter)
                    return (OwnerParagraph.Owner as HeaderFooter).Blocks.IndexOf(OwnerParagraph) > (inline.OwnerParagraph.Owner as HeaderFooter).Blocks.IndexOf(inline.OwnerParagraph);
                else
                    return OwnerParagraph.Section.Blocks.IndexOf(OwnerParagraph) > inline.OwnerParagraph.Section.Blocks.IndexOf(inline.OwnerParagraph);
            }

            return OwnerParagraph.IsExistAfter(inline.OwnerParagraph);
        }
        /// <summary>
        /// Inserts the inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <param name="index">The index.</param>
        internal void InsertInline(Inline inline, int index)
        {
            if (index == Length)
            {
                int insertIndex = OwnerParagraph.Inlines.IndexOf(this);
                insertIndex++;
                OwnerParagraph.Inlines.Insert(insertIndex, inline);
            }
            else if (index == 0)
            {
                if (PreviousNode == null)
                    OwnerParagraph.Inlines.Insert(0, inline);
                else
                {
                    int insertIndex = OwnerParagraph.Inlines.IndexOf(this);
                    OwnerParagraph.Inlines.Insert(insertIndex, inline);
                }
            }
            else
            {
                int insertIndex = OwnerParagraph.Inlines.IndexOf(this);
                insertIndex++;
                SpanAdv span = new SpanAdv();
                span.CharacterFormat.CopyFormat(CharacterFormat);
                span.Text = (this as SpanAdv).Text.Substring(index);
                (this as SpanAdv).Text = (this as SpanAdv).Text.Remove(index);
                OwnerParagraph.Inlines.Insert(insertIndex, span);
                //Inserts the new inline.
                OwnerParagraph.Inlines.Insert(insertIndex, inline);
            }
        }
        /// <summary>
        /// Gets the previous text inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetPreviousTextInline()
        {
            if (PreviousNode is SpanAdv)
                return PreviousNode as Inline;
            if (PreviousNode is FieldCharacterAdv && (PreviousNode as FieldCharacterAdv).IsLinkedFieldCharacter())
            {
                if (PreviousNode is FieldEndAdv || PreviousNode is FieldBeginAdv)
                    return PreviousNode as Inline;
                return (PreviousNode as FieldSeparatorAdv).FieldBegin;
            }
            if (PreviousNode != null)
                return (PreviousNode as Inline).GetPreviousTextInline();
            return null;
        }
        /// <summary>
        /// Gets the next text inline.
        /// </summary>
        /// <returns></returns>
        internal Inline GetNextTextInline()
        {
            if (NextNode is SpanAdv)
                return NextNode as Inline;
            if (NextNode is FieldCharacterAdv && (NextNode as FieldCharacterAdv).IsLinkedFieldCharacter())
            {
                if (NextNode is FieldEndAdv || NextNode is FieldBeginAdv)
                    return NextNode as Inline;
                return (NextNode as FieldSeparatorAdv).FieldEnd;
            }
            if (NextNode != null)
                return (NextNode as Inline).GetNextTextInline();
            return null;
        }
        /// <summary>
        /// Called when [text changed].
        /// </summary>
        protected void OnTextChanged()
        {
            ClearElements();
            if (OwnerParagraph != null && OwnerParagraph.BaseParent != null)
                OwnerParagraph.Relayout(OwnerParagraph.Inlines.IndexOf(this));
        }
        /// <summary>
        /// Clears the elements.
        /// </summary>
        internal void ClearElements()
        {
            if (Elements.Count == 0)
                return;
            for (int i = 0; i < Elements.Count; i++)
            {
                ElementBox element = Elements[i] as ElementBox;
                //Disposes the text element.
                element.Dispose();
                Elements.Remove(element);
                i--;
            }
        }
        /// <summary>
        /// Gets the element box.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal ElementBox GetElementBox(ref int index)
        {
            ElementBox element = null;
            if (Elements.Count > 0)
                element = Elements[Elements.Count - 1];
            if (element is TextElementBox)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    int length = (Elements[i] is TextElementBox) ? (Elements[i] as TextElementBox).Length : 1;
                    if (length >= index || (length + 1 == index && i == Elements.Count - 1))
                    {
                        element = Elements[i];
                        break;
                    }
                    index -= length;
                }
            }
            return element;
        }
        /// <summary>
        /// Gets the element box.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="moveToNextLine">if set to <c>true</c> [move to next line].</param>
        /// <returns></returns>
        internal ElementBox GetElementBox(ref int index, bool moveToNextLine)
        {
            ElementBox element = null;
            if (Elements.Count > 0)
                element = Elements[Elements.Count - 1];
            if (element is TextElementBox)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    int length = (Elements[i] is TextElementBox) ? (Elements[i] as TextElementBox).Length : 1;
                    if (length >= index || (length + 1 == index && i == Elements.Count - 1))
                    {
                        element = Elements[i];
                        if (moveToNextLine && index == length && i < Elements.Count - 1 && element.CurrentLineWidget != Elements[i + 1].CurrentLineWidget)
                        {
                            //Handled specifically to move the cursor at start of next line.
                            element = Elements[i + 1];
                            index = 0;
                        }
                        break;
                    }
                    index -= length;
                }
            }
            return element;
        }
        /// <summary>
        /// Determines whether the two inlines are equal in style.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <returns>
        ///   <c>true</c> if the two inlines are equal in style; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEqualInStyle(Inline inline)
        {
            if (this == inline)
                return true;
            else
                return CharacterFormat.IsEqualFormat(inline.CharacterFormat);
        }
        #endregion

        #region Caret Position
        /// <summary>
        /// Gets the physical position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal Point GetPhysicalPosition(int index)
        {
            ElementBox element = GetElementBox(ref index, true);
            if (element == null || element.CurrentLineWidget == null)
            {
                if (this is FieldCharacterAdv)
                    return GetFieldCharacterPosition();
                return new Point();
            }
            double top = 0, left = 0;
            top = element.CurrentLineWidget.GetTop();
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (element is ImageElementBox)
                {
                    CharacterFormat format = OwnerParagraph.CharacterFormat;
                    Inline previousInline = GetPreviousTextInline();
                    if (previousInline != null)
                        format = previousInline.CharacterFormat;
                    else
                    {
                        Inline nextInline = GetNextTextInline();
                        if (nextInline != null)
                            format = nextInline.CharacterFormat;
                    }
                    TextHelper.MeasureText(format);
                    double baselineOffset = TextHelper.TextMeasurer.BaselineOffset;
                    if (element.Margin.Top + element.Height - baselineOffset > 0)
                        top += element.Margin.Top + element.Height - baselineOffset;
                }
                else
                {
                    top += element.Margin.Top > 0 ? element.Margin.Top : 0;
                }
                left = element.CurrentLineWidget.GetLeft(element, index);
#if !WPF
            });
#endif
            return new Point(left, top);
        }
        /// <summary>
        /// Gets the field character position.
        /// </summary>
        /// <returns></returns>
        private Point GetFieldCharacterPosition()
        {
            Inline startInline = this;
            if (this is FieldBeginAdv && (this as FieldBeginAdv).IsLinkedFieldCharacter())
            {
                if ((this as FieldBeginAdv).FieldSeparator == null)
                    startInline = (this as FieldBeginAdv).FieldEnd;
                else
                    startInline = (this as FieldBeginAdv).FieldSeparator;
            }
            Inline nextValidInline = null;
            if (startInline.NextNode != null)
                //Check the next node is a valid and returns inline.
                nextValidInline = (startInline.NextNode as Inline).GetNextValidInline();
            //If field separator/end exists at end of paragraph, then move to next paragraph.
            if (nextValidInline == null)
            {
                ParagraphAdv nextParagraph = startInline.OwnerParagraph;
                return nextParagraph.GetEndPosition();
            }
            else
                return nextValidInline.GetPhysicalPosition(0);
        }
        /// <summary>
        /// Gets the height of the caret.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="format">The format.</param>
        /// <param name="isEmptySelection">if set to <c>true</c> [is empty selection].</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <returns></returns>
        internal double GetCaretHeight(int index, CharacterFormat format, bool isEmptySelection, ref double topMargin, ref bool isItalic)
        {
            ElementBox element = GetElementBox(ref index, false);
            if (element == null)
            {
                if (this is FieldCharacterAdv)
                    return GetFieldCharacterHeight(format, isEmptySelection, ref topMargin, ref isItalic);
                return TextHelper.MeasureText(format).Height;
            }
            double maxLineHeight = 0;
            if (element is ImageElementBox)
            {
                Inline previousInline = GetPreviousTextInline();
                Inline nextInline = GetNextTextInline();
                if (previousInline == null && nextInline == null)
                {
                    double top = 0, bottom = 0;
                    maxLineHeight = OwnerParagraph.GetParagraphMarkSize(ref top, ref bottom).Height;
                    isItalic = OwnerParagraph.CharacterFormat.Italic;
                    if (!isEmptySelection)
                        maxLineHeight += OwnerParagraph.ParagraphFormat.AfterSpacing;
                }
                else if (previousInline == null)
                {
                    isItalic = nextInline.CharacterFormat.Italic;
                    return nextInline.GetCaretHeight(0, nextInline.CharacterFormat, isEmptySelection, ref topMargin, ref isItalic);
                }
                else
                {
                    if (nextInline != null)
                    {
                        //Calculates the caret size using image character format.
                        double charHeight = TextHelper.MeasureText(element.Inline.CharacterFormat).Height;
                        double baselineOffset = TextHelper.TextMeasurer.BaselineOffset;
                        maxLineHeight = (element.Margin.Top < 0 && baselineOffset > element.Margin.Top + element.Height) ? element.Margin.Top + element.Height + charHeight - baselineOffset : charHeight;
                        if (!isEmptySelection)
                            maxLineHeight += element.Margin.Bottom;
                    }
                    else
                    {
                        isItalic = previousInline.CharacterFormat.Italic;
                        return previousInline.GetCaretHeight(previousInline.Length, previousInline.CharacterFormat, isEmptySelection, ref topMargin, ref isItalic);
                    }
                }
            }
            else
            {
                BaselineAlignment baselineAlignment = format.BaselineAlignment;
                double elementHeight = element.Height;
                if (baselineAlignment != BaselineAlignment.Normal && isEmptySelection)
                {
                    //Set the caret height as sub/super script text height and updates the top margin for sub script text.
                    elementHeight = elementHeight / 1.5;
                    if (baselineAlignment == BaselineAlignment.Subscript)
                        topMargin = element.Height - elementHeight;
                }
                maxLineHeight = (element.Margin.Top < 0 ? element.Margin.Top : 0) + elementHeight;
                if (!isEmptySelection)
                    maxLineHeight += element.Margin.Bottom;
            }
            if (!isEmptySelection)
                return maxLineHeight;
            double height = TextHelper.MeasureText(format).Height;
            if (height > maxLineHeight)
                height = maxLineHeight;
            return height;
        }
        /// <summary>
        /// Gets the height of the field character.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="isEmptySelection">if set to <c>true</c> [is empty selection].</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <returns></returns>
        private double GetFieldCharacterHeight(CharacterFormat format, bool isEmptySelection, ref double topMargin, ref bool isItalic)
        {
            Inline startInline = this;
            if (this is FieldBeginAdv && (this as FieldBeginAdv).IsLinkedFieldCharacter())
            {
                if ((this as FieldBeginAdv).FieldSeparator == null)
                    startInline = (this as FieldBeginAdv).FieldEnd;
                else
                    startInline = (this as FieldBeginAdv).FieldSeparator;
            }
            Inline nextValidInline = null;
            if (startInline.NextNode != null)
                //Check the next node is a valid and returns inline.
                nextValidInline = (startInline.NextNode as Inline).GetNextValidInline();
            //If field separator/end exists at end of paragraph, then move to next paragraph.
            if (nextValidInline == null)
            {
                ParagraphAdv nextParagraph = startInline.OwnerParagraph;
                double height = TextHelper.GetParagraphMarkSize(format).Height;
                double top = 0, bottom = 0;
                double maxLineHeight = nextParagraph.GetParagraphMarkSize(ref top, ref bottom).Height;
                if (!isEmptySelection)
                {
                    maxLineHeight += bottom;
                    return maxLineHeight;
                }
                if (height > maxLineHeight)
                    height = maxLineHeight;
                return height;
            }
            else
                return nextValidInline.GetCaretHeight(0, format, isEmptySelection, ref topMargin, ref isItalic);
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal LineWidget GetLineWidget(int index)
        {
            ElementBox element = GetElementBox(ref index, true);
            if (element != null)
                return element.CurrentLineWidget;
            Inline startInline = this;
            if (this is FieldBeginAdv && (this as FieldBeginAdv).IsLinkedFieldCharacter())
            {
                if ((this as FieldBeginAdv).FieldSeparator == null)
                    startInline = (this as FieldBeginAdv).FieldEnd;
                else
                    startInline = (this as FieldBeginAdv).FieldSeparator;
            }
            //ToDo: Check previous inline here.
            Inline nextValidInline = null;
            if (startInline.NextNode != null)
                //Check the next node is a valid and returns inline.
                nextValidInline = (startInline.NextNode as Inline).GetNextValidInline();
            //If field separator/end exists at end of paragraph, then move to next paragraph.
            if (nextValidInline == null)
            {
                ParagraphAdv nextParagraph = startInline.OwnerParagraph;
                LineWidget lineWidget = null;
                if (nextParagraph.ParagraphWidgets.Count > 0)
                {
                    Widget widget = nextParagraph.ParagraphWidgets[nextParagraph.ParagraphWidgets.Count - 1] as Widget;
                    if (widget.ChildWidgets.Count > 0)
                        lineWidget = widget.ChildWidgets[widget.ChildWidgets.Count - 1] as LineWidget;
                }
                return lineWidget;
            }
            else
                return nextValidInline.GetLineWidget(0);
        }
        #endregion

        #region Apply Character Format for Selected Contents
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyCharacterFormat(SelectionAdv selection, int startIndex, int endIndex, DependencyProperty property, object value)
        {
            if (startIndex == 0 && endIndex == Length)
                CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
            else if (this is SpanAdv)
                FormatInline(selection, startIndex, endIndex, property, value);
        }
        /// <summary>
        /// Formats the inline.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void FormatInline(SelectionAdv selection, int startIndex, int endIndex, DependencyProperty property, object value)
        {
            int index = OwnerParagraph.Inlines.IndexOf(this);
            SpanAdv span;
            if (startIndex > 0)
            {
                span = new SpanAdv();
                span.CharacterFormat.CopyFormat(CharacterFormat);
                span.Text = (this as SpanAdv).Text.Substring(startIndex, endIndex - startIndex);
                span.CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
                index++;
                OwnerParagraph.Inlines.Insert(index, span);
            }
            if (endIndex < Length)
            {
                span = new SpanAdv();
                span.CharacterFormat.CopyFormat(CharacterFormat);
                span.Text = (this as SpanAdv).Text.Substring(endIndex);
                index++;
                OwnerParagraph.Inlines.Insert(index, span);
            }
            if (startIndex == 0)
            {
                (this as SpanAdv).Text = (this as SpanAdv).Text.Remove(endIndex);
                CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
            }
            else
                (this as SpanAdv).Text = (this as SpanAdv).Text.Remove(startIndex);
        }
        #endregion

        #region Layout Implementations
        /// <summary>
        /// Relayouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void RelayoutItems(LayoutViewer viewer)
        {
            LayoutItems(viewer);
            Inline inline = this;
            while (inline.NextNode is Inline)
            {
                inline = inline.NextNode as Inline;
                inline.LayoutItems(viewer);
            }
            //Updates the rendered line widgets to current page.
            OwnerParagraph.UpdateWidgetToPage(viewer);
            viewer.UpdateClientArea(OwnerParagraph, false);
            OwnerParagraph.LayoutNextItems(viewer);
        }
        /// <summary>
        /// Layouts the field characters.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        private void LayoutFieldCharacters(LayoutViewer viewer)
        {
            if (this is FieldBeginAdv)
            {
                FieldBeginAdv field = this as FieldBeginAdv;
                if (!viewer.IsFieldCode && (field.FieldEnd != null || field.HasFieldEnd))
                {
                    viewer.FieldStack.Push(field);
                    viewer.IsFieldCode = true;
                }
            }
            else if (viewer.FieldStack.Count > 0)
            {
                if (this is FieldSeparatorAdv)
                {
                    FieldBeginAdv field = viewer.FieldStack.Peek();
                    if (field.FieldSeparator == this && (field.FieldEnd != null || field.HasFieldEnd))
                        viewer.IsFieldCode = false;
                }
                else
                {
                    FieldBeginAdv field = viewer.FieldStack.Peek();
                    if (field.FieldEnd == this)
                    {
                        //Removes this field from layouting field stack.
                        viewer.FieldStack.Pop();
                        viewer.IsFieldCode = false;
                        if (field.HasFieldEnd)
                            field.HasFieldEnd = false;
                        //else if (field.FieldSeparator != null)
                        //{
                        //    //Relayout from field begin.
                        //    field.RelayoutItems(viewer);
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutItems(LayoutViewer viewer)
        {
            if (this is FieldCharacterAdv)
            {
                LayoutFieldCharacters(viewer);
                //Invoke MoveToNextLine() if last item of paragraph is FieldSeparator or FieldEnd.
                if (NextNode == null && !viewer.IsFieldCode)
                {
#if !WPF
                    UIDispatcher.Execute(() =>
                    {
#endif
                        if (viewer.LineElements.Count == 0)
                            OwnerParagraph.LayoutEmptyLineWidget(viewer);
                        else
                            MoveToNextLine(viewer);
#if !WPF
                    });
#endif
                }
                return;
            }
            if (viewer.IsFieldCode)
                return;
            //Removes the rendered element.
            if (Elements != null && Elements.Count > 0)
                Elements.Clear();
            AddElementBox(viewer);
            //Lay outs the text and renders in particular location.
            ElementBox element = Elements[0];
            int count = 0;
            double fontSize = 0;
            bool isBold = false, isItalic = false;
            FontFamily fontFamily = null;
            BaselineAlignment baselineAlignment = BaselineAlignment.Normal;
            while (count < Elements.Count)
            {
#if !WPF
                if (viewer.OwnerControl.LoadingCancellationToken.IsCancellationRequested)
                    viewer.OwnerControl.LoadingCancellationToken.ThrowIfCancellationRequested();
                UIDispatcher.Execute(() =>
                {
#endif
                    element = Elements[count];
                    double width = element.Width;
                    double height = element.Height;
                    if (element is TextElementBox && element == Elements[0])
                    {
                        CharacterFormat format = CharacterFormat;
                        fontFamily = format.FontFamily;
                        fontSize = format.FontSize;
                        baselineAlignment = format.BaselineAlignment;
                        isBold = format.Bold;
                        isItalic = format.Italic;
                        width = TextHelper.GetTextSize(element as TextElementBox, fontSize, fontFamily, baselineAlignment, isBold, isItalic);
                        height = element.Height;
                        // Calculates tab width
                        if ((element as TextElementBox).Text == "\t")
                            element.Width = width = (this as SpanAdv).GetTabWidth(viewer);
                    }
                    if (viewer.CurrentHeaderFooter == null && viewer is PageLayoutViewer
                        && viewer.ClientActiveArea.Height < height)
                        OwnerParagraph.MoveToNextPage(viewer);
                    if (width <= viewer.ClientActiveArea.Width)
                        //Fits the text in current line.
                        AddElementToline(element, viewer);
                    else if (element is TextElementBox)
                    {
                        if ((element as TextElementBox).Text == "\t")
                        {
                            MoveToNextLine(viewer);
                            // Recalculates tab width based on new client active area X position
                            element.Width = width = (this as SpanAdv).GetTabWidth(viewer);
                            AddElementToline(element, viewer);
                        }
                        //Splits the text and arrange line by line, till end of text.
                        else
                            SplitTextForClientArea(viewer, element as TextElementBox, fontSize, fontFamily, baselineAlignment, isBold, isItalic, ref width);
                    }
                    else
                        SplitElementForClientArea(element, viewer);
                    count = Elements.IndexOf(element) + 1;
                    if (count >= Elements.Count && this.NextNode == null)
                        MoveToNextLine(viewer);
#if !WPF
                });
#endif
            }
        }
        /// <summary>
        /// Adds the element to line.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="viewer">The viewer.</param>
        private void AddElementToline(ElementBox element, LayoutViewer viewer)
        {
            //Handled specifically for flow layout inorder to get the exceeding width of non text elements.
            if (viewer is FlowLayoutViewer && !(element is TextElementBox)
                && viewer.ClientActiveArea.Width < element.Width
                && viewer.HorizontalWidth < viewer.ClientActiveArea.X + element.Width + viewer.ClientArea.X)
                viewer.HorizontalWidth = viewer.ClientActiveArea.X + element.Width + viewer.ClientArea.X;
            viewer.CutFromLeft(viewer.ClientActiveArea.X + element.Width);
            if (OwnerParagraph.ParagraphFormat.TextAlignment == TextAlignment.Justify && element is TextElementBox)
                SplitTextElementWordByWord(element as TextElementBox, viewer);
            if (this is ImageContainerAdv)
                viewer.LineElements.SkipClipImage = !(this as ImageContainerAdv).IsInlineImage;
            //Adds the text element to the line
            viewer.LineElements.Add(element);
        }
        /// <summary>
        /// Splits the text element word by word.
        /// </summary>
        /// <param name="textElement">The text element.</param>
        /// <param name="viewer">The viewer.</param>
        private void SplitTextElementWordByWord(TextElementBox textElement, LayoutViewer viewer)
        {
            string text = textElement.Text;
            if (text.Trim(' ').Contains(" "))
            {
                CharacterFormat format = CharacterFormat;
                FontFamily fontFamily = format.FontFamily;
                double fontSize = format.FontSize;
                BaselineAlignment baselineAlignment = format.BaselineAlignment;
                bool isBold = format.Bold;
                bool isItalic = format.Italic;
                int index = textElement.Length - text.TrimStart(' ').Length;
                while (index < textElement.Length)
                {
                    index = TextHelper.GetTextIndexAfterSpace(text, index);
                    if (index == 0 || index == textElement.Length)
                        break;
                    if (index < textElement.Length)
                    {
                        TextElementBox splittedElement = new TextElementBox(textElement.Inline);
                        string splittedText = text.Remove(index);
                        text = text.Remove(0, index);
                        if (text.StartsWith(" "))
                            index += text.Length - text.TrimStart(' ').Length;
                        splittedElement.StartIndex = textElement.StartIndex;
                        splittedElement.Length = index;
                        Elements.Insert(Elements.IndexOf(textElement), splittedElement);
                        TextHelper.UpdateTextSize(splittedElement, splittedText, fontSize, fontFamily, baselineAlignment, isBold, isItalic);
                        viewer.LineElements.Add(splittedElement);
                        textElement.StartIndex += index;
                        textElement.Length -= index;
                        textElement.Width -= splittedElement.Width;
                        index = 0;
                    }
                }
            }
        }
        /// <summary>
        /// Splits the element for client area.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="viewer">The viewer.</param>
        private void SplitElementForClientArea(ElementBox element, LayoutViewer viewer)
        {
            if (viewer.LineElements.Count > 0)
                MoveToNextLine(viewer);
            if (viewer.LineElements.Count == 0)
            {
                if (OwnerParagraph.IsInsideTable)
                {
                    element.Width = viewer.ClientArea.Width;
                    if (OwnerParagraph.Inlines.IndexOf(this) == 0)
                        element.Width -= OwnerParagraph.ParagraphFormat.FirstLineIndent;
                }
                //Fits the image/UIElements in current line.
                AddElementToline(element, viewer);
            }
        }
        /// <summary>
        /// Splits the text for client area.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="element">The element.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        /// <param name="isBold">if set to <c>true</c> [is bold].</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="width">The width.</param>
        private void SplitTextForClientArea(LayoutViewer viewer, TextElementBox element, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic, ref double width)
        {
            bool isSplitByWord = true;
            string text = element.Text;
            if (!text.StartsWith(" "))
            {
                double textWidth = width;
                //Checks whether text not starts with white space. If starts with white space, no need to check previous text blocks.
                if (text.TrimEnd(' ').Contains(" "))
                {
                    int index = text.IndexOf(" ") + 1;
                    textWidth = TextHelper.MeasureText(text.Remove(index), fontSize, fontFamily, baselineAlignment, isBold, isItalic).Width;
                }
                if (viewer.ClientActiveArea.Width < textWidth)
                {
                    //Check and split the previous text elements to next line.
                    isSplitByWord = CheckPreviousElement(viewer);
                    if (isSplitByWord)
                        isSplitByWord = textWidth <= viewer.ClientActiveArea.Width;
                }
            }
            if (width <= viewer.ClientActiveArea.Width)
                //Fits the text in current line.
                AddElementToline(element, viewer);
            else if (text.Contains(" ") && isSplitByWord)
                //Split the text by word and fits in current line.
                SplitByWord(viewer, element, text, fontSize, fontFamily, baselineAlignment, isBold, isItalic, ref width);
            else
                //Split the text by character and fits in current line.
                SplitByCharacter(viewer, element, text, fontSize, fontFamily, baselineAlignment, isBold, isItalic, ref width);
        }
        /// <summary>
        /// Gets the previous inline.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Inline GetPreviousInline(ElementBox element)
        {
            if (elements.Contains(element))
                return this;
            Inline inline = PreviousNode as Inline;
            while (inline != null && !inline.elements.Contains(element))
            {
                inline = inline.PreviousNode as Inline;
                if (inline == null)
                    break;
            }
            return inline;
        }
        /// <summary>
        /// Checks the previous element.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        private bool CheckPreviousElement(LayoutViewer viewer)
        {
            bool isSplitByWord = false;
            int lastTextElement = 0;
            for (int i = viewer.LineElements.Count - 1; i >= 0; i--)
            {
                if (viewer.LineElements[i] is TextElementBox)
                {
                    TextElementBox textElement = viewer.LineElements[i] as TextElementBox;
                    lastTextElement = i;
                    if (textElement.Text.EndsWith(" "))
                    {
                        if (i == viewer.LineElements.Count - 1)
                        {
                            MoveToNextLine(viewer);
                            return true;
                        }
                        isSplitByWord = true;
                        break;
                    }
                    else if (textElement.Text == "\t")
                    {
                        // Splits the word to next line along with tab
                        lastTextElement--;
                        isSplitByWord = true;
                        break;
                    }
                    else if (textElement.Text.Contains(" "))
                    {
                        isSplitByWord = true;
                        int index = textElement.Text.LastIndexOf(" ") + 1;
                        //Splits the text element by space.
                        TextElementBox splittedElement = new TextElementBox(textElement.Inline);
                        splittedElement.StartIndex = textElement.StartIndex + index;
                        splittedElement.Length = textElement.Length - index;
                        textElement.Length -= splittedElement.Length;
                        Inline inline = GetPreviousInline(textElement);
                        inline.Elements.Add(splittedElement);
                        TextHelper.UpdateTextSize(splittedElement);
                        textElement.Width -= splittedElement.Width;
                        //Adds the text element to the line
                        viewer.LineElements.Insert(i + 1, splittedElement);
                        break;
                    }
                }
                else
                {
                    //Handled for inline images/UIelements.
                    lastTextElement = i;
                    isSplitByWord = true;
                    break;
                }
            }
            if (isSplitByWord)
            {
                lastTextElement++;
                List<ElementBox> elements = new List<ElementBox>();
                if (lastTextElement < viewer.LineElements.Count)
                {
                    double splitWidth = 0;
                    for (int i = lastTextElement; i < viewer.LineElements.Count; i++)
                    {
                        splitWidth += viewer.LineElements[i].Width;
                        elements.Add(viewer.LineElements[i]);
                        //Removes from the current line.
                        viewer.LineElements.RemoveAt(i);
                        i--;
                    }
                    viewer.UpdateClientWidth(splitWidth);
                    viewer.LineElements.UpdateMaxElement();
                }
                MoveToNextLine(viewer);
                // If splitted by tab, recalulate its width based on new client active area X position
                if (elements.Count > 0 && elements[0] is TextElementBox && (elements[0] as TextElementBox).Text == "\t")
                    elements[0].Width = (this as SpanAdv).GetTabWidth(viewer);
                //Adds the elements to next line.
                for (int i = 0; i < elements.Count; i++)
                {
                    AddElementToline(elements[i], viewer);
                }
                elements.Clear();
            }
            return isSplitByWord;
        }
        /// <summary>
        /// Splits the text by word.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="textElement">The text element.</param>
        /// <param name="text">The text.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        /// <param name="isBold">if set to <c>true</c> [is bold].</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="width">The width.</param>
        private void SplitByWord(LayoutViewer viewer, TextElementBox textElement, string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic, ref double width)
        {
            int index = TextHelper.GetTextSplitIndexByWord(viewer.ClientActiveArea.Width, text, fontSize, fontFamily, baselineAlignment, isBold, isItalic, width);
            if (index > 0 && index < textElement.Length)
            {
                TextElementBox splittedElement = new TextElementBox(textElement.Inline);
                string txt = text.Remove(0, index);
                if (txt.StartsWith(" "))
                    index += txt.Length - txt.TrimStart(' ').Length;
                splittedElement.StartIndex = textElement.StartIndex + index;
                splittedElement.Length = textElement.Length - index;
                textElement.Length -= splittedElement.Length;
                Elements.Add(splittedElement);
                width = TextHelper.GetTextSize(splittedElement, fontSize, fontFamily, baselineAlignment, isBold, isItalic);
                textElement.Width -= splittedElement.Width;
                AddElementToline(textElement, viewer);
                MoveToNextLine(viewer);
            }
        }
        /// <summary>
        /// Splits the text by character.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="textElement">The text element.</param>
        /// <param name="text">The text.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        /// <param name="isBold">if set to <c>true</c> [is bold].</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="width">The width.</param>
        private void SplitByCharacter(LayoutViewer viewer, TextElementBox textElement, string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic, ref double width)
        {
            int index = TextHelper.GetTextSplitIndexByCharacter(viewer.ClientArea.Width, viewer.ClientActiveArea.Width, text, fontSize, fontFamily, baselineAlignment, isBold, isItalic, width);
            string splitText = index == textElement.Length ? text : text.Remove(index);
            double splitWidth = TextHelper.GetTextWidth(splitText);
            if (splitWidth > viewer.ClientActiveArea.Width && viewer.LineElements.Count > 0)
                MoveToNextLine(viewer);
            //Adds the last text element on inline to line elements collection. 
            AddElementToline(textElement, viewer);
            if (index < textElement.Length)
            {
                TextElementBox splittedElement = new TextElementBox(textElement.Inline);
                splittedElement.StartIndex = textElement.StartIndex + index;
                splittedElement.Length = textElement.Length - index;
                textElement.Length -= splittedElement.Length;
                Elements.Add(splittedElement);
                width = TextHelper.GetTextSize(splittedElement, fontSize, fontFamily, baselineAlignment, isBold, isItalic);
                textElement.Width -= splittedElement.Width;
                MoveToNextLine(viewer);
            }
        }
        /// <summary>
        /// Determines whether this instance is in first line of paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns>
        ///   <c>true</c> if this instance is in first line of paragraph; otherwise, <c>false</c>.
        /// </returns>
        private bool IsParagraphFirstLine(ParagraphAdv paragraph)
        {
            if (paragraph.ParagraphWidgets.Count == 1)
            {
                Widget widget = paragraph.ParagraphWidgets[0];
                return widget.ChildWidgets.Count == 0;
            }
            return false;
        }
        /// <summary>
        /// Determines whether this instance is in last line of paragraph.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns>
        ///   <c>true</c> if this instance is in last line of paragraph; otherwise, <c>false</c>.
        /// </returns>
        private bool IsParagraphLastLine(LayoutViewer viewer, ParagraphAdv paragraph)
        {
            Inline lastInline = paragraph.Inlines.Last();
            if (this == lastInline)
                return lastInline.Elements.Count == 0 || viewer.LineElements.Contains(lastInline.Elements[lastInline.Elements.Count - 1]);
            return false;
        }
        /// <summary>
        /// Moves to next line.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        private void MoveToNextLine(LayoutViewer viewer)
        {
            ParagraphAdv ownerParagraph = OwnerParagraph;
            ParagraphFormat paraFormat = ownerParagraph.ParagraphFormat;
            bool isParagraphStart = IsParagraphFirstLine(ownerParagraph);
            bool isParagraphEnd = IsParagraphLastLine(viewer, ownerParagraph);
            double maxDescent = 0, afterSpacing = 0, beforeSpacing = 0, lineSpacing = 0, firstLineIndent = 0;
            //Updates before spacing at the top of Paragraph first line.
            if (isParagraphStart)
            {
                beforeSpacing = ownerParagraph.GetBeforeSpacing();
                firstLineIndent = paraFormat.FirstLineIndent;
            }
            //Updates after spacing at the bottom of Paragraph last line.
            if (isParagraphEnd)
                afterSpacing = paraFormat.AfterSpacing;
            //Calculate the bottom position of current line - max height + line spacing.
            double height = 0;
            if (double.IsNaN(viewer.LineElements.MaxTextElementHeight))
            {
                //Calculate line height and descent based on formatting defined in paragraph.
                height = TextHelper.MeasureText(ownerParagraph.CharacterFormat).Height;
                maxDescent = height - TextHelper.TextMeasurer.BaselineOffset;
            }
            else
            {
                height = viewer.LineElements.MaxTextElementHeight;
                maxDescent = height - viewer.LineElements.MaxTextElementBaselineOffset;
            }
            if (viewer.CurrentHeaderFooter == null && viewer is PageLayoutViewer
                && viewer.ClientActiveArea.Height < beforeSpacing + viewer.LineElements.MaxBaselineOffset + maxDescent)
                ownerParagraph.MoveToNextPage(viewer);
            //Gets line spacing.
            lineSpacing = ownerParagraph.GetLineSpacing(height);
            if (viewer.LineElements.SkipClipImage
                && paraFormat.LineSpacingType == LineSpacingType.Exactly
                && lineSpacing < maxDescent + viewer.LineElements.MaxBaselineOffset)
                lineSpacing = maxDescent + viewer.LineElements.MaxBaselineOffset;
            double subWidth = 0;
            int whiteSpaceCount = 0;
            TextAlignment textAlignment = paraFormat.TextAlignment;
            //Calculates the sub width, for text alignments - Center, Right, Justify.
            if (textAlignment != TextAlignment.Left
                && !(textAlignment == TextAlignment.Justify && isParagraphEnd))
                subWidth = GetSubWidth(viewer, textAlignment == TextAlignment.Justify, ref whiteSpaceCount, firstLineIndent);
            LineWidget line = ownerParagraph.AddLineWidget();
            bool addSubWidth = false;
            LineSpacingType lineSpacingType = paraFormat.LineSpacingType;
            for (int i = 0; i < viewer.LineElements.Count; i++)
            {
                double topMargin = 0, bottomMargin = 0, leftMargin = 0;
                ElementBox element = viewer.LineElements[i];
                AlignLineElements(viewer, element, ref topMargin, ref bottomMargin, ref addSubWidth, subWidth, maxDescent, textAlignment, whiteSpaceCount);
                //Updates line spacing, paragraph after/ before spacing and aligns the text to base line offset.
                if (lineSpacingType == LineSpacingType.Multiple)
                {
                    if (lineSpacing > height)
                        bottomMargin += lineSpacing - height;
                    else
                        topMargin += lineSpacing - height;
                }
                else if (lineSpacingType == LineSpacingType.Exactly)
                    topMargin += lineSpacing - (topMargin + element.Height + bottomMargin);
                else if (lineSpacing > topMargin + element.Height + bottomMargin)
                    topMargin += lineSpacing - (topMargin + element.Height + bottomMargin);
                topMargin += beforeSpacing;
                bottomMargin += afterSpacing;
                if (i == 0)
                {
                    line.Height = topMargin + element.Height + bottomMargin;
                    if (textAlignment == TextAlignment.Right)
                        //Aligns the text as right justified.
                        leftMargin = subWidth;
                    else if (textAlignment == TextAlignment.Center)
                        //Aligns the text as center justified.
                        leftMargin = subWidth / 2;
                }
                element.Margin = new Thickness(leftMargin, topMargin, 0, bottomMargin);
                element.CurrentLineWidget = line;
                line.Children.Add(element);
            }
            viewer.CutFromTop(viewer.ClientActiveArea.Y + line.Height);
            //Clears the previous line elements from collection.
            viewer.LineElements.Clear();
        }
        /// <summary>
        /// Aligns the text elements.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="element">The element.</param>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="bottomMargin">The bottom margin.</param>
        /// <param name="addSubWidth">if set to <c>true</c> add sub width.</param>
        /// <param name="subWidth">Width of the sub.</param>
        /// <param name="maxDescent">The max descent.</param>
        /// <param name="textAlignment">The text alignment.</param>
        /// <param name="whiteSpaceCount">The white space count.</param>
        private void AlignLineElements(LayoutViewer viewer, ElementBox element, ref double topMargin, ref double bottomMargin, ref bool addSubWidth, double subWidth, double maxDescent, TextAlignment textAlignment, int whiteSpaceCount)
        {
            if (element is TextElementBox || element is ListTextElementBox)
            {
                TextElementBox textElement = element as TextElementBox;
                //Updates the text to base line offset.
                double baselineOffset = element is TextElementBox ? textElement.BaselineOffset : (element as ListTextElementBox).BaselineOffset;
                topMargin += viewer.LineElements.MaxBaselineOffset - baselineOffset;
                bottomMargin += maxDescent - (element.Height - baselineOffset);
                if (textElement != null && textAlignment == TextAlignment.Justify && whiteSpaceCount > 0)
                {
                    //Aligns the text as Justified.
                    double width = textElement.Width;
                    string text = textElement.Text;
                    if (!addSubWidth)
                    {
                        text = textElement.Text.TrimStart(' ');
                        addSubWidth = (text.Length > 0);
                    }
                    if (addSubWidth)
                    {
                        int spaceCount = text.Length - text.Replace(" ", "").Length;
                        if (whiteSpaceCount < spaceCount)
                        {
                            width = TextHelper.GetWidthExcludeSpaceAtEnd(textElement);
                            spaceCount = whiteSpaceCount;
                        }
                        if (spaceCount > 0)
                        {
                            textElement.Width = width + subWidth * spaceCount;
                            whiteSpaceCount -= spaceCount;
                        }
                    }
                }
            }
            else
            {
                addSubWidth = true;
                //Updates the Image/UIElement to base line offset.
                topMargin += viewer.LineElements.MaxBaselineOffset - element.Height;
                bottomMargin += maxDescent;
            }
        }
        /// <summary>
        /// Gets the width of the sub.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="justify">if set to <c>true</c> [justify].</param>
        /// <param name="spaceCount">The space count.</param>
        /// <returns></returns>
        private double GetSubWidth(LayoutViewer viewer, bool justify, ref int spaceCount,double firstLineIndent)
        {
            double width = 0;
            bool trimSpace = true;
            string lineText = string.Empty;
            for (int i = viewer.LineElements.Count - 1; i >= 0; i--)
            {
                ElementBox element = viewer.LineElements[i];
                if (element is TextElementBox)
                {
                    lineText = (element as TextElementBox).Text + lineText;
                    if (trimSpace && (element as TextElementBox).Text.Trim(' ') != "")
                    {
                        if ((element as TextElementBox).Text.EndsWith(" "))
                            width += TextHelper.GetWidthExcludeSpaceAtEnd(element as TextElementBox);
                        else
                            width += element.Width;
                        trimSpace = false;
                    }
                    else if (!trimSpace)
                        width += element.Width;
                }
                else
                {
                    lineText = "a" + lineText;
                    trimSpace = false;
                    width += element.Width;
                }
                if (!justify)
                    width = Math.Round(width);
            }
            lineText = lineText.Trim(' ');
            spaceCount = lineText.Length - lineText.Replace(" ", "").Length;
            double subWidth = (viewer.ClientArea.Width - firstLineIndent - width);
            if (subWidth <= 0 || (spaceCount == 0 && justify))
            {
                spaceCount = 0;
                subWidth = 0;
            }
            else if (justify)
                subWidth = subWidth / spaceCount;
            return subWidth;
        }
        #endregion
    }

#if WPF
    [ContentProperty("Text")]
#else
    [ContentProperty(Name = "Text")]
#endif
    public class SpanAdv : Inline
    {
        #region Properties
        string text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnTextChanged();
            }
        }
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        internal override int Length
        {
            get
            {
                if (Text == null)
                    return 0;
                return Text.Length;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanAdv"/> class.
        /// </summary>
        public SpanAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal SpanAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            Inline inline = this;
            int wordStartIndex = -1;
            bool IsinField= IsNodeInField(this as Node);
            if (startIndex > 0)
            {
                if (!IsinField && !isCaretLeftToInline && startIndex > 0 && Text.Length - 1 >= startIndex &&
                    ((Array.IndexOf(WordSplitCharacters, Text[startIndex - 1]) != -1 && Array.IndexOf(WordSplitCharacters, Text[startIndex]) == -1) ||
                   (Array.IndexOf(WordSplitCharacters, Text[startIndex - 1]) == -1 && Array.IndexOf(WordSplitCharacters, Text[startIndex]) != -1)))
                {
                    //No need to set new start index
                    return inline;
                }
                string txt = startIndex >= Text.Length ? Text : Text.Remove(startIndex);
                bool considerSplitCharacters = string.IsNullOrEmpty(starttext.Trim(WordSplitCharacters)) && starttext.StartsWith(" ");
                if (considerSplitCharacters)
                {
                    if (string.IsNullOrEmpty(starttext.Trim(' ')))
                        wordStartIndex = txt.TrimEnd(WordSplitCharacters).Length;
                    else if (considerSplitCharacters)
                    {
                        char[] specialCharacters = new char[] { ',', '.', ':', ';', '<', '>', '=', '+', '-', '_', '{', '}', '[', ']','"','\'','|','\\','?','/', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
                        wordStartIndex = txt.TrimEnd(specialCharacters).Length;
                    }
                    else
                        wordStartIndex = txt.LastIndexOfAny(WordSplitCharacters);
                }
                else
                    wordStartIndex = txt.LastIndexOfAny(WordSplitCharacters);
                if (txt.TrimEnd(WordSplitCharacters).Length != txt.Length && !IsinField)
                {
                    wordStartIndex = txt.Length;
                    for (int i = txt.Length - 1; i >= 0 ; i--)
                    {
                        if (Array.IndexOf(WordSplitCharacters, txt[i]) != -1 && txt[i] != ' ')
                            wordStartIndex--;
                        else
                        {
                            wordStartIndex--;
                            break;
                        }
                    }
                }
                if (this.PreviousNode is FieldSeparatorAdv)
                    wordStartIndex = 0;
                if (wordStartIndex >= 0)
                {

                    if ((wordStartIndex < txt.Length &&
                        ((Array.IndexOf(WordSplitCharacters, txt[wordStartIndex]) != -1 &&
                            wordStartIndex+1 < txt.Length && 
                            Array.IndexOf(WordSplitCharacters, txt[wordStartIndex + 1]) == -1) 
                            ||
                            (Array.IndexOf(WordSplitCharacters, txt[wordStartIndex]) == -1 &&
                            wordStartIndex + 1 < txt.Length && 
                            Array.IndexOf(WordSplitCharacters, txt[wordStartIndex + 1]) != -1))
                            ||
                            (Array.IndexOf(WordSplitCharacters, txt[wordStartIndex]) != -1 &&
                            wordStartIndex - 1 >0 && 
                            Array.IndexOf(WordSplitCharacters, txt[wordStartIndex - 1]) == -1) 
                            ||
                            (Array.IndexOf(WordSplitCharacters, txt[wordStartIndex]) == -1 &&
                            wordStartIndex - 1 > 0 && 
                            Array.IndexOf(WordSplitCharacters, txt[wordStartIndex - 1]) != -1))
                            &&
                            wordStartIndex < txt.Length - 1 && !(this.PreviousNode is FieldSeparatorAdv) || txt[wordStartIndex] == ' ')
                        //Sets the word start as word split char's end.
                        wordStartIndex++;
                    if (txt.Length > wordStartIndex)
                        starttext = txt.Remove(wordStartIndex, txt.Length - wordStartIndex) + starttext;
                }
                else
                    starttext = txt + starttext;
            }
            if (wordStartIndex <= 0 && !(this.PreviousNode !=null && this.PreviousNode is FieldSeparatorAdv))
            {
                if (inline.PreviousNode is Inline && (inline.PreviousNode is SpanAdv || inline.PreviousNode is FieldCharacterAdv
                    || (inline.PreviousNode is ImageContainerAdv && string.IsNullOrEmpty(starttext.Trim(WordSplitCharacters)))))
                {
                    if (inline.PreviousNode is SpanAdv)
                    {
                        string prevNodeText = (inline.PreviousNode as SpanAdv).Text;
                        if (prevNodeText.Length > 0 && Text.Length > 0 &&
                            (Array.IndexOf(WordSplitCharacters, prevNodeText[prevNodeText.Length - 1]) != -1 &&
                            Array.IndexOf(WordSplitCharacters, Text[0]) == -1) ||
                            (Array.IndexOf(WordSplitCharacters, prevNodeText[prevNodeText.Length - 1]) == -1 &&
                            Array.IndexOf(WordSplitCharacters, Text[0]) != -1))
                        {
                            startIndex = 0;
                            return inline;
                        }
                    }
                    startIndex = (inline.PreviousNode as Inline).Length;
                    starttext = string.Empty;
                    Inline prevInline = (inline.PreviousNode as Inline).GetWordStartInline(ref startIndex, ref starttext, isCaretLeftToInline);
                    inline = prevInline;
                }
                else
                    startIndex = 0;
            }
            else
                startIndex = wordStartIndex;
            return inline;
        }

        private bool IsNodeInField(Node node)
        {
            while(node != null)
            {
                if(node is FieldBeginAdv)
                {
                    FieldBeginAdv begin = node as FieldBeginAdv;
                    if (begin.FieldEnd != null)
                        return true;
                    else
                        return false;
                }
                else if(node is FieldEndAdv)
                {
                    return false;
                }
                else if(node is FieldSeparatorAdv)
                {
                    FieldSeparatorAdv separator = node as FieldSeparatorAdv;
                    if (separator.FieldBegin != null && separator.FieldEnd != null)
                        return true;
                    else
                        return false;
                }
                node = node.PreviousNode;
            }
            return false;
        }
        /// <summary>
        /// Gets the end index of split chars.
        /// </summary>
        /// <param name="wordSplitCharacters">The word split characters.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="wordEndIndex">End index of the word.</param>
        /// <returns></returns>
        private int GetEndIndexOfSplitChars(string txt, int wordEndIndex, bool isCaretLeftToInline)
        {
            string splitText = txt[wordEndIndex].ToString();
            bool considerSpecialCharacter = false;
            if (!isCaretLeftToInline && wordEndIndex == 0 && splitText != " ")
                //Handled specially for selecting special characters.
                considerSpecialCharacter = true;
            while ((considerSpecialCharacter && string.IsNullOrEmpty(splitText.Trim(WordSplitCharacters)))
                || splitText == " ")
            {
                if (splitText == " ")
                    considerSpecialCharacter = false;
                wordEndIndex++;
                if (wordEndIndex == txt.Length)
                    break;
                splitText = txt[wordEndIndex].ToString();
            }
            return wordEndIndex;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <param name="endtext">The endtext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            Inline inline = this;
            int wordEndIndex = 0;
            bool isinField = IsNodeInField(this as Node);
            if (endIndex < Length)
            {
                wordEndIndex = Length;
                string txt = endIndex > 0 && Text.Length - 1 >= endIndex ? Text.Remove(0, endIndex) : Text;
                wordEndIndex = txt.IndexOfAny(WordSplitCharacters);
                if (!isinField && isCaretLeftToInline && endIndex > 0 && Text.Length > endIndex && 
                ((Array.IndexOf(WordSplitCharacters, Text[endIndex - 1]) != -1 && Array.IndexOf(WordSplitCharacters, Text[endIndex]) == -1) ||
                   (Array.IndexOf(WordSplitCharacters, Text[endIndex - 1]) == -1 && Array.IndexOf(WordSplitCharacters, Text[endIndex]) != -1)))
                {
                    //No need to set new end index
                    return inline;
                }
                if (txt.TrimStart(WordSplitCharacters).Length != txt.Length && !isinField)
                {
                    wordEndIndex = 0;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        if (Array.IndexOf(WordSplitCharacters, txt[i]) != -1 && txt[i] != ' ')
                            wordEndIndex++;
                        else
                            break;
                    }
                }
                if (wordEndIndex < 0)
                    wordEndIndex = txt.Length;
                else
                {
                    //wordEndIndex = GetEndIndexOfSplitChars(txt, wordEndIndex-1, isCaretLeftToInline);
                    endtext += wordEndIndex < txt.Length ? txt.Remove(0,wordEndIndex) : string.Empty;
                    //wordEndIndex += endIndex;
                }
            }
            if (((endIndex + wordEndIndex) == Length) || wordEndIndex == Length)// && Text.LastIndexOfAny(WordSplitCharacters) != Text.Length - 1)
            {
                if (inline.NextNode is SpanAdv || inline.NextNode is FieldCharacterAdv)
                {
                    if (inline.NextNode is SpanAdv)
                    {
                        string nextNodeText = (inline.NextNode as SpanAdv).Text;
                        if (nextNodeText.Length > 0 && Text.Length > 0 &&
                            (Array.IndexOf(WordSplitCharacters, nextNodeText[0]) != -1 &&
                            Array.IndexOf(WordSplitCharacters, Text[Text.Length - 1]) == -1) ||
                            (Array.IndexOf(WordSplitCharacters, nextNodeText[0]) == -1 &&
                            Array.IndexOf(WordSplitCharacters, Text[Text.Length - 1]) != -1))
                        {
                            endIndex = inline.Length;
                            return inline;
                        }
                    }
                    endIndex = 0;
                    Inline nextInline = (inline.NextNode as Inline).GetWordEndInline(ref endIndex, ref endtext, isCaretLeftToInline);
                    inline = nextInline;
                }
                else
                    endIndex = inline.Length;
            }
            else
                endIndex += wordEndIndex;
            return inline;
        }
        /// <summary>
        /// Adds the element box.
        /// </summary>
        internal override void AddElementBox(LayoutViewer viewer)
        {
            // If text contains tab character, splits it as Seperate SpanAdv
            if (Text.Contains("\t") && Text != "\t")
                SplitByTab(viewer);
            TextElementBox textElement = new TextElementBox(this);
            textElement.StartIndex = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                textElement.Length = Text.Length;
#if !WPF
            });
#endif
            Elements.Add(textElement);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            text = null;
            SetOwner(null);
            ClearElements();
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            SpanAdv span = new SpanAdv();
            span.CharacterFormat.CopyFormat(CharacterFormat);
            span.Text = Text;
            return span;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            if (index > 0)
            {
                Text = Text.Insert(index, text);
            }
            else
            {
                Inline prevInline = GetPreviousTextInline();
                if (prevInline == null)
                {
                    Text = Text.Insert(index, text);
                }
                else
                {
                    index = OwnerParagraph.Inlines.IndexOf(this);
                    SpanAdv span = new SpanAdv();
                    span.CharacterFormat.CopyFormat(prevInline.CharacterFormat);
                    span.Text = text;
                    OwnerParagraph.Inlines.Insert(index, span);
                }
            }
        }
        /// <summary>
        /// Splits the by tab.
        /// </summary>
        /// <param name="viewer">TheLayout viewer.</param>
        private void SplitByTab(LayoutViewer viewer)
        {
            // Splits tab character to seperate SpanAdv
            viewer.OwnerControl.IsLayoutEnabled = false;
            int inlineIndex = OwnerParagraph.Inlines.IndexOf(this);
            string value = Text;
            int index = value.IndexOf('\t');
            string remainder = value.Substring(index + 1);
            SpanAdv newSpan = new SpanAdv();
            newSpan.CharacterFormat.CopyFormat(CharacterFormat);
            OwnerParagraph.Inlines.Insert(inlineIndex + 1, newSpan);
            if (index > 0)
            {
                newSpan.Text = value.Substring(index);
                Text = value.Substring(0, index);
            }
            else if (remainder != string.Empty)
            {
                newSpan.Text = remainder;
                Text = "\t";
            }
            viewer.OwnerControl.IsLayoutEnabled = true;
        }
        /// <summary>
        /// Gets the width of the tab.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        internal double GetTabWidth(LayoutViewer viewer)
        {
            // Calculates tab width based on page left margin and default tab width property
            double defaultTabWidth = OwnerParagraph.Document.DefaultTabWidth;
            double position = viewer.ClientActiveArea.X - (viewer.ClientArea.X - OwnerParagraph.ParagraphFormat.LeftIndent) - viewer.CurrentPage.Margin.Left;
            if (position == 0 || defaultTabWidth == 0)
                return defaultTabWidth;
            else
            {
                double diff = (double)((int)(Math.Round(position, 2) * 100) % (int)(Math.Round(defaultTabWidth, 2) * 100)) / 100;
                double cnt = (Math.Round(position, 2) - diff) / Math.Round(defaultTabWidth, 2);
                double fposition = (cnt + 1) * defaultTabWidth;
                return (fposition - position) > 0 ? fposition - position : defaultTabWidth;
            }
        }
        #endregion
    }

#if WPF
    [ContentProperty("UIElement")]
#else
    [ContentProperty(Name = "UIElement")]
#endif
    internal class UIContainerAdv : Inline
    {
        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        internal override int Length
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Gets or Sets the UIelement.
        /// </summary>
        /// <value>
        /// The UI element.
        /// </value>
        public UIElement UIElement
        {
            get
            {
                return (UIElement)GetValue(UIElementProperty);
            }
            set
            {
                SetValue(UIElementProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the width of the UIelement.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the height of the UIelement.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        /// <returns>The identifier of the Width dependency property.</returns>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(UIContainerAdv), new PropertyMetadata(0, OnWidthChanged));
        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        /// <returns>The identifier of the Height dependency property.</returns>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(UIContainerAdv), new PropertyMetadata(0, OnHeightChanged));
        /// <summary>
        /// Identifies the UIElement dependency property.
        /// </summary>
        /// <returns>The identifier of the UIElement dependency property.</returns>
        public static readonly DependencyProperty UIElementProperty = DependencyProperty.Register("UIElement", typeof(UIElement), typeof(UIContainerAdv), new PropertyMetadata(null, OnUIElementChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when width changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
        }
        /// <summary>
        /// Called when height changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
        }
        /// <summary>
        /// Called when UI element changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnUIElementChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            UIContainerAdv uiContainerAdv = (UIContainerAdv)dependencyObject;
            uiContainerAdv.OnUIElementChanged(args);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UIContainerAdv"/> class.
        /// </summary>
        public UIContainerAdv()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UIContainerAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public UIContainerAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Raises the <see cref="E:UIElementChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnUIElementChanged(DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)args.NewValue;
            ((UIElement)args.NewValue).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Width = ((UIElement)args.NewValue).DesiredSize.Width;
            element.Height = ((UIElement)args.NewValue).DesiredSize.Height;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            startIndex = 0;
            if (startIndex == 0 && PreviousNode is Inline)
            {
                startIndex = (PreviousNode as Inline).Length;
                string txt = "";
                Inline previousInline = (PreviousNode as Inline).GetWordStartInline(ref startIndex, ref txt, isCaretLeftToInline);
                if (string.IsNullOrEmpty(txt.Trim(WordSplitCharacters)) && previousInline is UIContainerAdv)
                {
                    starttext = txt;
                    return previousInline;
                }
            }
            return this;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            if (endIndex < Length)
                endIndex = isCaretLeftToInline ? 0 : Length;
            if (endIndex == Length && NextNode is Inline)
            {
                endIndex = (NextNode as Inline).Length;
                string txt = "";
                Inline nextInline = (NextNode as Inline).GetWordEndInline(ref endIndex, ref txt, isCaretLeftToInline);
                if (string.IsNullOrEmpty(txt.Trim(WordSplitCharacters)))
                {
                    endtext = txt;
                    return nextInline;
                }
            }
            return this;
        }
        /// <summary>
        /// Adds the element box.
        /// </summary>
        internal override void AddElementBox(LayoutViewer viewer)
        {
            UIElementBox uiElement = new UIElementBox(this);
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                uiElement.Width = Width;
                uiElement.Height = Height;
#if !WPF
            });
#endif
            Elements.Add(uiElement);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            ClearValue(HeightProperty);
            ClearValue(UIElementProperty);
            ClearValue(WidthProperty);
            SetOwner(null);
            ClearElements();
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            UIContainerAdv uiContainer = new UIContainerAdv();
            uiContainer.CharacterFormat.CopyFormat(CharacterFormat);
            uiContainer.Width = Width;
            uiContainer.Height = Height;
            uiContainer.UIElement = UIElement;
            return uiContainer;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            Inline previousInline = GetPreviousTextInline();
            Inline nextInline = GetNextTextInline();
            SpanAdv span = new SpanAdv();
            span.Text = text;
            int spanIndex = OwnerParagraph.Inlines.IndexOf(this);
            if (index == Length)
                spanIndex++;
            if (previousInline == null && nextInline == null)
                span.CharacterFormat.CopyFormat(OwnerParagraph.CharacterFormat);
            else if (previousInline == null)
                span.CharacterFormat.CopyFormat(nextInline.CharacterFormat);
            else
                span.CharacterFormat.CopyFormat(previousInline.CharacterFormat);
            OwnerParagraph.Inlines.Insert(spanIndex, span);
        }
        #endregion
    }

    public abstract class FieldCharacterAdv : Inline
    {
        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        internal override int Length
        {
            get
            {
                return 1;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCharacterAdv"/> class.
        /// </summary>
        public FieldCharacterAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCharacterAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal FieldCharacterAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Abstract Methods
        internal abstract bool IsLinkedFieldCharacter();
        internal abstract void UnlinkFieldCharacter(string compositeNodeIndex, bool isAddToFieldCharacters);
        internal abstract void CheckUnlinkFieldCharacter(string compositeNodeIndex, bool isCheckFieldCharacters);
        #endregion

        #region Override Methods
        /// <summary>
        /// Adds the element box.
        /// </summary>
        internal override void AddElementBox(LayoutViewer viewer)
        {
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            SetOwner(null);
            ClearElements();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the layout viewer.
        /// </summary>
        /// <returns></returns>
        internal LayoutViewer GetLayoutViewer()
        {
            if (OwnerParagraph != null)
            {
                SfRichTextBoxAdv richTextBoxAdv = OwnerParagraph.BaseParent;
                if (richTextBoxAdv != null)
                    return richTextBoxAdv.Viewer;
            }
            return null;
        }
        /// <summary>
        /// Removes the after linking.
        /// </summary>
        internal void RemoveAfterLinking()
        {
            (Owner as CompositeNode).RemoveFieldCharacter(this);
        }
        /// <summary>
        /// Gets the rendered inline.
        /// </summary>
        /// <param name="inlineIndex">Index of the inline.</param>
        /// <returns></returns>
        internal Inline GetRenderedInline(ref int inlineIndex)
        {
            Inline prevInline = GetPreviousValidInline();
            while (prevInline is FieldCharacterAdv)
            {
                prevInline = prevInline.GetPreviousTextInline();
                if (prevInline is FieldCharacterAdv)
                    prevInline = prevInline.PreviousNode as Inline;
            }
            if (prevInline != null)
            {
                inlineIndex = prevInline.Length;
                return prevInline;
            }
            inlineIndex = 0;
            Inline nextInline = GetNextRenderedInline(0);
            if (nextInline is FieldBeginAdv)
            {
                nextInline = (nextInline as FieldBeginAdv).FieldSeparator;
                nextInline = nextInline.NextNode as Inline;
                while (nextInline is FieldCharacterAdv)
                {
                    if (nextInline is FieldBeginAdv && (nextInline as FieldCharacterAdv).IsLinkedFieldCharacter())
                    {
                        if ((nextInline as FieldBeginAdv).FieldSeparator == null)
                            nextInline = (nextInline as FieldBeginAdv).FieldEnd;
                        else
                            nextInline = (nextInline as FieldBeginAdv).FieldSeparator;
                    }
                    nextInline = nextInline.NextNode as Inline;
                }
            }
            return nextInline;
        }
        #endregion
    }
    public class FieldBeginAdv : FieldCharacterAdv
    {
        #region Fields
        FieldSeparatorAdv fieldSeparator;
        FieldEndAdv fieldEnd;
        internal bool HasFieldEnd = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the field separator.
        /// </summary>
        /// <value>
        /// The field separator.
        /// </value>
        internal FieldSeparatorAdv FieldSeparator
        {
            get
            {
                return fieldSeparator;
            }
            set
            {
                if (fieldSeparator != value)
                {
                    FieldSeparatorAdv prevFieldSeparator = fieldSeparator;
                    fieldSeparator = value;
                    if (prevFieldSeparator != null)
                    {
                        //Unlinks the previous field begin mark.
                        prevFieldSeparator.FieldBegin = null;
                        prevFieldSeparator = null;
                    }
                    //Sets the current instance as field begin mark.
                    if (fieldSeparator != null)
                        fieldSeparator.FieldBegin = this;
                }
            }
        }
        /// <summary>
        /// Gets or sets the field end.
        /// </summary>
        /// <value>
        /// The field end.
        /// </value>
        internal FieldEndAdv FieldEnd
        {
            get
            {
                return fieldEnd;
            }
            set
            {
                if (fieldEnd != value)
                {
                    FieldEndAdv prevFieldEnd = fieldEnd;
                    fieldEnd = value;
                    if (prevFieldEnd != null)
                    {
                        //Unlinks the previous field begin mark.
                        prevFieldEnd.FieldBegin = null;
                        prevFieldEnd = null;
                    }
                    //Sets the current instance as field begin mark.
                    if (fieldEnd != null)
                        fieldEnd.FieldBegin = this;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBeginAdv"/> class.
        /// </summary>
        public FieldBeginAdv()
            : this(null)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBeginAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal FieldBeginAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            Inline inline = GetPreviousRenderedInline();
            if (inline == this)
                startIndex = 0;
            else
            {
                startIndex = inline.Length;
                inline = inline.GetWordStartInline(ref startIndex, ref starttext, isCaretLeftToInline);
            }
            return inline;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            Inline inline = GetNextRenderedInline();
            if (inline == this)
                endIndex = 0;
            else
            {
                endIndex = 0;
                inline = inline.GetWordEndInline(ref endIndex, ref endtext, isCaretLeftToInline);
            }
            return inline;
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            FieldBeginAdv fieldBegin = new FieldBeginAdv();
            fieldBegin.CharacterFormat.CopyFormat(CharacterFormat);
            return fieldBegin;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            SpanAdv span = new SpanAdv();
            span.Text = text;
            int spanIndex = OwnerParagraph.Inlines.IndexOf(this);
            span.CharacterFormat.CopyFormat(CharacterFormat);
            OwnerParagraph.Inlines.Insert(spanIndex, span);
        }
        internal override bool IsLinkedFieldCharacter()
        {
            return fieldEnd != null;
        }
        /// <summary>
        /// Determines whether to unlink field characters.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <returns>
        ///   <c>true</c> if unlink field characters; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsUnlinkFieldCharacters(string compositeNodeIndex)
        {
            if (fieldEnd == null)
                return false;
            string fieldEndOwnerIndex = fieldEnd.OwnerParagraph.GetHierarchicalIndex("");
            return !fieldEndOwnerIndex.Contains(compositeNodeIndex);
        }
        /// <summary>
        /// Unlinks the field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isAddToFieldCharacters">if set to <c>true</c> [is add to field characters].</param>
        internal override void UnlinkFieldCharacter(string compositeNodeIndex, bool isAddToFieldCharacters)
        {
            LayoutViewer viewer = GetLayoutViewer();
            if (fieldSeparator != null)
            {
                bool isUnlinkSeparator = true;
                if (isAddToFieldCharacters)
                {
                    string fieldOwnerIndex = fieldSeparator.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        isUnlinkSeparator = false;
                }
                if (isUnlinkSeparator)
                {
                    (fieldSeparator.Owner as CompositeNode).AddFieldCharacter(fieldSeparator);
                    fieldSeparator.FieldBegin = null;
                    fieldSeparator = null;
                }
                else
                    (fieldSeparator.Owner as CompositeNode).AddFieldCharacter(fieldSeparator);
            }
            if (fieldEnd != null)
            {
                if (viewer != null)
                {
                    viewer.FieldToLayout = this;
                    viewer.FieldEndParagraph = fieldEnd.OwnerParagraph;
                }
                (fieldEnd.Owner as CompositeNode).AddFieldCharacter(fieldEnd);
                fieldEnd.FieldBegin = null;
                fieldEnd = null;
            }
            if ((Owner as CompositeNode).Fields.Contains(this))
                (Owner as CompositeNode).Fields.Remove(this);
            if (isAddToFieldCharacters)
                (Owner as CompositeNode).AddFieldCharacter(this);
        }
        /// <summary>
        /// Checks the unlink field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isCheckFieldCharacters">if set to <c>true</c> [is check field characters].</param>
        internal override void CheckUnlinkFieldCharacter(string compositeNodeIndex, bool isCheckFieldCharacters)
        {
            if (fieldSeparator != null)
            {
                if (isCheckFieldCharacters)
                {
                    string fieldOwnerIndex = fieldSeparator.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        return;
                }
                fieldSeparator.FieldBegin = null;
                fieldSeparator = null;
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the rendered field.
        /// </summary>
        /// <returns></returns>
        internal Inline GetRenderedField()
        {
            Inline inline = this;
            if (FieldSeparator == null)
                inline = FieldEnd;
            else
            {
                inline = FieldSeparator;
                ParagraphAdv paragraph = inline.OwnerParagraph;
                if (paragraph == FieldEnd.OwnerParagraph && !paragraph.HasValidInline(inline, FieldEnd))
                    inline = FieldEnd;
                else
                    return this;
            }
            return inline;
        }
        /// <summary>
        /// Adds to linked fields.
        /// </summary>
        internal void AddToLinkedFields()
        {
            RemoveAfterLinking();
            if (fieldSeparator != null)
                fieldSeparator.RemoveAfterLinking();
            fieldEnd.RemoveAfterLinking();
            (Owner as CompositeNode).AddField(this);
            if (OwnerParagraph != null && !HasFieldEnd)
            {
                SfRichTextBoxAdv richTextBoxAdv = OwnerParagraph.BaseParent;
                if (richTextBoxAdv != null)
                {
                    LayoutViewer viewer = richTextBoxAdv.Viewer;
                    if (viewer != null)
                    {
                        if (viewer.FieldToLayout != null && viewer.FieldToLayout.OwnerParagraph != null)
                        {
                            string prevStartIndex = viewer.FieldToLayout.GetIndexInOwnerCollection().ToString();
                            prevStartIndex = viewer.FieldToLayout.OwnerParagraph.GetHierarchicalIndex(prevStartIndex);
                            string startIndex = GetIndexInOwnerCollection().ToString();
                            startIndex = OwnerParagraph.GetHierarchicalIndex(startIndex);
                            if (TextPosition.IsForwardSelection(startIndex, prevStartIndex))
                                viewer.FieldToLayout = this;
                        }
                        else
                            viewer.FieldToLayout = this;
                        if (viewer.FieldEndParagraph != null)
                        {
                            string prevEndIndex = viewer.FieldEndParagraph.GetHierarchicalIndex("");
                            string endIndex = fieldEnd.OwnerParagraph.GetHierarchicalIndex("");
                            if (TextPosition.IsForwardSelection(prevEndIndex, endIndex))
                                viewer.FieldEndParagraph = fieldEnd.OwnerParagraph;
                        }
                        else
                            viewer.FieldEndParagraph = fieldEnd.OwnerParagraph;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the field code.
        /// </summary>
        /// <returns></returns>
        internal string GetFieldCode()
        {
            if (fieldEnd == null || OwnerParagraph == null)
                return "";
            bool isNestedFieldCode = false;
            bool isFieldCodeParsed = false;
            return OwnerParagraph.GetFieldCode(this, ref isNestedFieldCode, ref isFieldCodeParsed, this);
        }
        /// <summary>
        /// Inlines the is in field result.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <returns></returns>
        internal bool InlineIsInFieldResult(Inline inline)
        {
            if (fieldEnd != null && fieldSeparator != null)
            {
                if (fieldSeparator.IsExistBefore(inline))
                    return fieldEnd.IsExistAfter(inline);
            }
            return false;
        }
        /// <summary>
        /// Paragraphes the is in field result.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        internal bool ParagraphIsInFieldResult(ParagraphAdv paragraph)
        {
            if (fieldEnd != null && fieldSeparator != null)
            {
                if (fieldSeparator.Owner == paragraph || fieldSeparator.OwnerParagraph.IsExistBefore(paragraph))
                    return (fieldEnd.Owner != paragraph && fieldEnd.OwnerParagraph.IsExistAfter(paragraph));
            }
            return false;
        }
        #endregion
    }

    public class FieldSeparatorAdv : FieldCharacterAdv
    {
        #region Fields
        FieldBeginAdv fieldBegin;
        FieldEndAdv fieldEnd;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the field begin.
        /// </summary>
        /// <value>
        /// The field begin.
        /// </value>
        internal FieldBeginAdv FieldBegin
        {
            get
            {
                return fieldBegin;
            }
            set
            {
                if (fieldBegin != value)
                {
                    FieldBeginAdv prevFieldBegin = fieldBegin;
                    fieldBegin = value;
                    if (prevFieldBegin != null)
                    {
                        //Unlinks the previous field separator mark.
                        prevFieldBegin.FieldSeparator = null;
                        prevFieldBegin = null;
                    }
                    //Sets the current instance as field separator mark.
                    if (fieldBegin != null)
                        fieldBegin.FieldSeparator = this;
                }
            }
        }
        /// <summary>
        /// Gets or sets the field end.
        /// </summary>
        /// <value>
        /// The field end.
        /// </value>
        internal FieldEndAdv FieldEnd
        {
            get
            {
                return fieldEnd;
            }
            set
            {
                if (fieldEnd != value)
                {
                    FieldEndAdv prevFieldEnd = fieldEnd;
                    fieldEnd = value;
                    if (prevFieldEnd != null)
                    {
                        //Unlinks the previous field separator mark.
                        prevFieldEnd.FieldSeparator = null;
                        prevFieldEnd = null;
                    }
                    //Sets the current instance as field separator mark.
                    if (fieldEnd != null)
                        fieldEnd.FieldSeparator = this;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSeparatorAdv"/> class.
        /// </summary>
        public FieldSeparatorAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSeparatorAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal FieldSeparatorAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            Inline inline = GetPreviousRenderedInline();
            if (inline == this)
                startIndex = 0;
            else
            {
                startIndex = inline.Length;
                inline = inline.GetWordStartInline(ref startIndex, ref starttext, isCaretLeftToInline);
            }
            return inline;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            Inline inline = GetNextRenderedInline();
            if (inline == this)
                endIndex = 0;
            else
            {
                endIndex = 0;
                inline = inline.GetWordEndInline(ref endIndex, ref endtext, isCaretLeftToInline);
            }
            return inline;
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            FieldSeparatorAdv fieldSeparator = new FieldSeparatorAdv();
            fieldSeparator.CharacterFormat.CopyFormat(CharacterFormat);
            return fieldSeparator;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            Inline previousInline = GetPreviousTextInline();
            Inline nextInline = GetNextTextInline();
            SpanAdv span = new SpanAdv();
            span.Text = text;
            int spanIndex = OwnerParagraph.Inlines.IndexOf(this);
            if (index == Length)
                spanIndex++;
            if (previousInline == null && nextInline == null)
                span.CharacterFormat.CopyFormat(OwnerParagraph.CharacterFormat);
            else if (previousInline == null)
                span.CharacterFormat.CopyFormat(nextInline.CharacterFormat);
            else
                span.CharacterFormat.CopyFormat(previousInline.CharacterFormat);
            OwnerParagraph.Inlines.Insert(spanIndex, span);
        }
        /// <summary>
        /// Determines whether this instance is linked field character.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is linked field character; otherwise, <c>false</c>.
        /// </returns>
        internal override bool IsLinkedFieldCharacter()
        {
            return fieldBegin != null && fieldEnd != null;
        }
        /// <summary>
        /// Unlinks the field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isAddToFieldCharacters">if set to <c>true</c> [is add to field characters].</param>
        internal override void UnlinkFieldCharacter(string compositeNodeIndex, bool isAddToFieldCharacters)
        {
            if (fieldBegin != null)
            {
                bool isUnlinkSeparator = true;
                if (isAddToFieldCharacters)
                {
                    string fieldOwnerIndex = fieldBegin.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        isUnlinkSeparator = false;
                }
                if (fieldEnd != null)
                {
                    LayoutViewer viewer = GetLayoutViewer();
                    if (viewer != null)
                    {
                        viewer.FieldToLayout = fieldBegin;
                        viewer.FieldEndParagraph = fieldEnd.OwnerParagraph;
                    }
                }
                if (isUnlinkSeparator)
                {
                    (fieldBegin.Owner as CompositeNode).AddFieldCharacter(fieldBegin);
                    fieldBegin.FieldSeparator = null;
                    fieldBegin = null;
                }
                else
                    (fieldBegin.Owner as CompositeNode).AddFieldCharacter(fieldBegin);
            }
            if (fieldEnd != null)
            {
                bool isUnlinkSeparator = true;
                if (isAddToFieldCharacters)
                {
                    string fieldOwnerIndex = fieldEnd.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        isUnlinkSeparator = false;
                }
                if (isUnlinkSeparator)
                {
                    (fieldEnd.Owner as CompositeNode).AddFieldCharacter(fieldEnd);
                    fieldEnd.FieldSeparator = null;
                    fieldEnd = null;
                }
                else
                    (fieldEnd.Owner as CompositeNode).AddFieldCharacter(fieldEnd);
            }
            if (isAddToFieldCharacters)
                (Owner as CompositeNode).AddFieldCharacter(this);
        }
        /// <summary>
        /// Checks the unlink field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isCheckFieldCharacters">if set to <c>true</c> [is check field characters].</param>
        internal override void CheckUnlinkFieldCharacter(string compositeNodeIndex, bool isCheckFieldCharacters)
        {
            if (fieldBegin != null)
            {
                if (isCheckFieldCharacters)
                {
                    string fieldOwnerIndex = fieldBegin.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        return;
                }
                fieldBegin.FieldSeparator = null;
                fieldBegin = null;
            }
            if (fieldEnd != null)
            {
                if (isCheckFieldCharacters)
                {
                    string fieldOwnerIndex = fieldEnd.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        return;
                }
                fieldEnd.FieldSeparator = null;
                fieldEnd = null;
            }
        }
        #endregion
    }
    public class FieldEndAdv : FieldCharacterAdv
    {
        #region Fields
        FieldBeginAdv fieldBegin;
        FieldSeparatorAdv fieldSeparator;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the field begin.
        /// </summary>
        /// <value>
        /// The field begin.
        /// </value>
        internal FieldBeginAdv FieldBegin
        {
            get
            {
                return fieldBegin;
            }
            set
            {
                if (fieldBegin != value)
                {
                    FieldBeginAdv prevFieldBegin = fieldBegin;
                    fieldBegin = value;
                    if (prevFieldBegin != null)
                    {
                        //Unlinks the previous field end mark.
                        prevFieldBegin.FieldEnd = null;
                        prevFieldBegin = null;
                    }
                    //Sets the current instance as field end mark.
                    if (fieldBegin != null)
                        fieldBegin.FieldEnd = this;
                }
            }
        }
        /// <summary>
        /// Gets or sets the field separator.
        /// </summary>
        /// <value>
        /// The field separator.
        /// </value>
        internal FieldSeparatorAdv FieldSeparator
        {
            get
            {
                return fieldSeparator;
            }
            set
            {
                if (fieldSeparator != value)
                {
                    FieldSeparatorAdv prevFieldSeparator = fieldSeparator;
                    fieldSeparator = value;
                    if (prevFieldSeparator != null)
                    {
                        //Unlinks the previous field end mark.
                        prevFieldSeparator.FieldEnd = null;
                        prevFieldSeparator = null;
                    }
                    //Sets the current instance as field end mark.
                    if (fieldSeparator != null)
                        fieldSeparator.FieldEnd = this;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldEndAdv"/> class.
        /// </summary>
        public FieldEndAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldEndAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal FieldEndAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            Inline inline = GetPreviousRenderedInline();
            if (inline == this)
                startIndex = 0;
            else
            {
                startIndex = inline.Length;
                inline = inline.GetWordStartInline(ref startIndex, ref starttext, isCaretLeftToInline);
                //inline = prevInline;
                //if (prevInline is ImageContainerAdv)
            }
            return inline;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            Inline inline = GetNextRenderedInline();
            if (inline == this)
                endIndex = Length;
            else
            {
                endIndex = 0;
                inline = inline.GetWordEndInline(ref endIndex, ref endtext, isCaretLeftToInline);
            }
            return inline;
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            FieldEndAdv fieldEnd = new FieldEndAdv();
            fieldEnd.CharacterFormat.CopyFormat(CharacterFormat);
            return fieldEnd;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            SpanAdv span = new SpanAdv();
            span.Text = text;
            int spanIndex = OwnerParagraph.Inlines.IndexOf(this);
            span.CharacterFormat.CopyFormat(CharacterFormat);
            OwnerParagraph.Inlines.Insert(spanIndex + 1, span);
        }
        /// <summary>
        /// Determines whether this instance is linked field character.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is linked field character; otherwise, <c>false</c>.
        /// </returns>
        internal override bool IsLinkedFieldCharacter()
        {
            return fieldBegin != null;
        }
        /// <summary>
        /// Unlinks the field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isAddToFieldCharacters">if set to <c>true</c> [is add to field characters].</param>
        internal override void UnlinkFieldCharacter(string compositeNodeIndex, bool isAddToFieldCharacters)
        {
            if (fieldBegin != null)
            {
                LayoutViewer viewer = GetLayoutViewer();
                if (viewer != null)
                {
                    viewer.FieldToLayout = fieldBegin;
                    viewer.FieldEndParagraph = OwnerParagraph;
                }
                if ((fieldBegin.Owner as CompositeNode).Fields.Contains(fieldBegin))
                    (fieldBegin.Owner as CompositeNode).Fields.Remove(fieldBegin);
                (fieldBegin.Owner as CompositeNode).AddFieldCharacter(fieldBegin);
                fieldBegin.FieldEnd = null;
                fieldBegin = null;
            }
            if (fieldSeparator != null)
            {
                bool isUnlinkSeparator = true;
                if (isAddToFieldCharacters)
                {
                    string fieldOwnerIndex = fieldSeparator.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        isUnlinkSeparator = false;
                }
                if (isUnlinkSeparator)
                {
                    (fieldSeparator.Owner as CompositeNode).AddFieldCharacter(fieldSeparator);
                    fieldSeparator.FieldEnd = null;
                    fieldSeparator = null;
                }
                else
                    (fieldSeparator.Owner as CompositeNode).AddFieldCharacter(fieldSeparator);
            }
            if (isAddToFieldCharacters)
                (Owner as CompositeNode).AddFieldCharacter(this);
        }
        /// <summary>
        /// Checks the unlink field character.
        /// </summary>
        /// <param name="compositeNodeIndex">Index of the composite node.</param>
        /// <param name="isCheckFieldCharacters">if set to <c>true</c> [is check field characters].</param>
        internal override void CheckUnlinkFieldCharacter(string compositeNodeIndex, bool isCheckFieldCharacters)
        {
            if (fieldSeparator != null)
            {
                if (isCheckFieldCharacters)
                {
                    string fieldOwnerIndex = fieldSeparator.OwnerParagraph.GetHierarchicalIndex("");
                    if (fieldOwnerIndex.Contains(compositeNodeIndex))
                        return;
                }
                fieldSeparator.FieldEnd = null;
                fieldSeparator = null;
            }
        }
        #endregion
    }

    public class ImageContainerAdv : Inline
    {
        #region Fields
        internal bool IsInlineImage = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        internal override int Length
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Gets or Sets the width of the image container.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the height of the image container.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the image bytes.
        /// </summary>
        /// <value>
        /// The image bytes.
        /// </value>
        internal byte[] ImageBytes
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the image string.
        /// </summary>
        /// <value>
        /// The image string.
        /// </value>
        public string ImageString
        {
            get
            {
                return (string)GetValue(ImageStringProperty);
            }
            set
            {
                SetValue(ImageStringProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the ImageSource dependency property.
        /// </summary>
        /// <returns>The identifier of the ImageSource dependency property.</returns>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageContainerAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnImageSourceChanged)));
        /// <summary>
        /// Identifies the ImageString dependency property.
        /// </summary>
        /// <returns>The identifier of the ImageString dependency property.</returns>
        public static readonly DependencyProperty ImageStringProperty = DependencyProperty.Register("ImageString", typeof(string), typeof(ImageContainerAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnImageStringChanged)));
        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        /// <returns>The identifier of the Width dependency property.</returns>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ImageContainerAdv), new PropertyMetadata(0d, OnWidthChanged));
        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        /// <returns>The identifier of the Height dependency property.</returns>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(ImageContainerAdv), new PropertyMetadata(0d, OnHeightChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when image string changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnImageStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageContainerAdv imageContainer = d as ImageContainerAdv;

            if (e.NewValue != null)
            {
                string imageString = e.NewValue.ToString();

                byte[] btyeArr = Convert.FromBase64String(imageString);

                imageContainer.ImageBytes = btyeArr;

                var bmp = new BitmapImage();
#if WPF
                bmp.BeginInit();
                bmp.SetSource(new MemoryStream(btyeArr));
                bmp.EndInit();
#else
                bmp.SetSource(new MemoryStream(btyeArr));
#endif
                imageContainer.ImageSource = bmp;
                imageContainer.Width = bmp.PixelWidth;
                imageContainer.Height = bmp.PixelHeight;
            }
        }
        /// <summary>
        /// Called when image source changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageContainerAdv imagecontainer = d as ImageContainerAdv;
            if (e.NewValue != null)
            {
                if (e.NewValue is BitmapImage)
                {
                    imagecontainer.Width = (e.NewValue as BitmapImage).PixelWidth;
                    imagecontainer.Height = (e.NewValue as BitmapImage).PixelHeight;
                }
#if WPF
                string location = e.NewValue.ToString();
                if (location.Contains("file:"))
                {
                    imagecontainer.ImageBytes = File.ReadAllBytes(location.Replace("file:///",""));
                }
                else if (location.Contains("pack://application:,,,"))
                {
                    Uri uri = new Uri(location, UriKind.RelativeOrAbsolute);
                    if (Uri.IsWellFormedUriString(uri.OriginalString, UriKind.RelativeOrAbsolute))
                    {
                        StreamResourceInfo streaminfo = Application.GetResourceStream(uri);
                        imagecontainer.ImageBytes = streaminfo.Stream.GetBytes();
                    }
                }
#endif
            }
        }
        /// <summary>
        /// Called when width changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
        }
        /// <summary>
        /// Called when height changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageContainerAdv"/> class.
        /// </summary>
        public ImageContainerAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageContainerAdv"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal ImageContainerAdv(Node owner)
            : base(owner)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Gets the word start inline.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="starttext">The starttext.</param>
        /// <param name="isCaretLeftToInline">if set to <c>true</c> [is caret left to inline].</param>
        /// <returns></returns>
        internal override Inline GetWordStartInline(ref int startIndex, ref string starttext, bool isCaretLeftToInline)
        {
            startIndex = 0;
            if (startIndex == 0 && PreviousNode is Inline)
            {
                startIndex = (PreviousNode as Inline).Length;
                string txt = "";
                Inline previousInline = (PreviousNode as Inline).GetWordStartInline(ref startIndex, ref txt, isCaretLeftToInline);
                if (string.IsNullOrEmpty(txt.Trim(WordSplitCharacters)) && previousInline is ImageContainerAdv)
                {
                    starttext = txt;
                    return previousInline;
                }
            }
            return this;
        }
        /// <summary>
        /// Gets the word end inline.
        /// </summary>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal override Inline GetWordEndInline(ref int endIndex, ref string endtext, bool isCaretLeftToInline)
        {
            if (endIndex < Length)
                endIndex = isCaretLeftToInline ? 0 : Length;
            if (endIndex == Length && NextNode is Inline)
            {
                endIndex = (NextNode as Inline).Length;
                string txt = "";
                Inline nextInline = (NextNode as Inline).GetWordEndInline(ref endIndex, ref txt, isCaretLeftToInline);
                if (string.IsNullOrEmpty(txt.Trim(WordSplitCharacters)))
                {
                    endtext = txt;
                    return nextInline;
                }
            }
            return this;
        }
        /// <summary>
        /// Adds the element box.
        /// </summary>
        internal override void AddElementBox(LayoutViewer viewer)
        {
            ImageElementBox imageElement = new ImageElementBox(this);
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                imageElement.Width = Width;
                imageElement.Height = Height;
#if !WPF
            });
#endif
            Elements.Add(imageElement);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            ClearValue(HeightProperty);
            ClearValue(ImageSourceProperty);
            ClearValue(ImageStringProperty);
            ClearValue(WidthProperty);
            ImageBytes = null;
            SetOwner(null);
            ClearElements();
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal override Inline Clone()
        {
            ImageContainerAdv image = new ImageContainerAdv();
            image.CharacterFormat.CopyFormat(CharacterFormat);
            image.ImageSource = ImageSource;
            image.ImageBytes = ImageBytes;
            image.Width = Width;
            image.Height = Height;
            return image;
        }
        /// <summary>
        /// Inserts the text.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        internal override void InsertText(SelectionAdv selection, string text, int index)
        {
            Inline previousInline = GetPreviousTextInline();
            Inline nextInline = GetNextTextInline();
            SpanAdv span = new SpanAdv();
            span.Text = text;
            int spanIndex = OwnerParagraph.Inlines.IndexOf(this);
            if (index == Length)
                spanIndex++;
            if (previousInline == null && nextInline == null)
                span.CharacterFormat.CopyFormat(OwnerParagraph.CharacterFormat);
            else if (previousInline == null)
                span.CharacterFormat.CopyFormat(nextInline.CharacterFormat);
            else
                span.CharacterFormat.CopyFormat(previousInline.CharacterFormat);
            OwnerParagraph.Inlines.Insert(spanIndex, span);
        }
        #endregion
    }
}