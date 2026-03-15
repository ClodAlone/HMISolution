#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
#if WPF
using System.Windows.Markup;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Text;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Inlines")]
#else
    [ContentProperty(Name = "Inlines")]
#endif
    public class ParagraphAdv : BlockAdv
    {
        #region Fields
        private List<Widget> paragraphWidgets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the paragraph format.
        /// </summary>
        /// <value>
        /// The paragraph format.
        /// </value>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                return (ParagraphFormat)GetValue(ParagraphFormatProperty);
            }
            set
            {
                SetValue(ParagraphFormatProperty, value);
            }
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
        /// Gets or Sets the inlines
        /// </summary>
        /// <value>
        /// The inlines.
        /// </value>
        public InlineCollection Inlines
        {
            get
            {
                return ChildNodes as InlineCollection;
            }
            internal set
            {
                ChildNodes = value;
            }
        }
        /// <summary>
        /// Gets the paragraph widgets.
        /// </summary>
        /// <value>
        /// The paragraph widgets.
        /// </value>
        internal List<Widget> ParagraphWidgets
        {
            get
            {
                return paragraphWidgets;
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the ParagraphFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the ParagraphFormat dependency property.</returns>
        internal static readonly DependencyProperty ParagraphFormatProperty = DependencyProperty.Register("ParagraphFormat", typeof(ParagraphFormat), typeof(ParagraphAdv), new PropertyMetadata(null, OnParagraphFormatChanged));
        /// <summary>
        /// Identifies the CharacterFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the CharacterFormat dependency property.</returns>
        internal static readonly DependencyProperty CharacterFormatProperty = DependencyProperty.Register("CharacterFormat", typeof(CharacterFormat), typeof(ParagraphAdv), new PropertyMetadata(null, OnCharacterFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when paragraph format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnParagraphFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as ParagraphFormat).SetOwner(d as ParagraphAdv);
        }
        /// <summary>
        /// Called when character format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCharacterFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as CharacterFormat).SetOwner(d as ParagraphAdv);
        }
        #endregion

        #region Constructor
        public ParagraphAdv()
            : this(null)
        {
        }
        internal ParagraphAdv(Node owner)
            : base(owner)
        {
            ChildNodes = new InlineCollection(this);
            CharacterFormat = new CharacterFormat(this);
            ParagraphFormat = new ParagraphFormat(this);
            paragraphWidgets = new List<Widget>();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The BlockAdv.</returns>
        internal override BlockAdv Clone()
        {
            ParagraphAdv paragraph = new ParagraphAdv();
            paragraph.ParagraphFormat.CopyFormat(ParagraphFormat);
            paragraph.CharacterFormat.CopyFormat(CharacterFormat);
            foreach (Inline inline in Inlines)
            {
                paragraph.Inlines.Add(inline.Clone());
            }
            return paragraph;
        }
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        internal override bool UpdateListItems(BlockAdv block)
        {
            if (this == block)
                return true;
            else
            {
#if !WPF
                UIDispatcher.Execute(() =>
                {
#endif
                    DocumentAdv document = Document;
                    ListAdv currentList = document.GetListAdv(ParagraphFormat.ListFormat.ListId);
                    if (currentList != null && currentList.AbstractList != null
                        && currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber] != null)
                    {
                        ListLevelAdv currentListLevel = currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber];
                        //Updates the list numbering from document start for relayouting.
                        if (currentListLevel.ReadLocalValue(ListLevelAdv.BulletCharacterProperty) == DependencyProperty.UnsetValue)
                            document.GetListNumber(currentListLevel);
                    }
#if !WPF
                });
#endif
            }
            return false;
        }
        /// <summary>
        /// Updates the rendered list items.
        /// </summary>
        internal override void UpdateRenderedListItems()
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (!string.IsNullOrEmpty(ParagraphFormat.ListFormat.ListId))
                {
                    DocumentAdv document = Document;
                    ListAdv currentList = document.GetListAdv(ParagraphFormat.ListFormat.ListId);
                    if (currentList != null && currentList.AbstractList != null
                        && currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber] != null)
                    {
                        ListLevelAdv currentListLevel = currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber];
                        //Updates the list numbering from document start for relayouting.
                        if (currentListLevel.ReadLocalValue(ListLevelAdv.BulletCharacterProperty) == DependencyProperty.UnsetValue)
                        {
                            ListTextElementBox element = null;
                            if (paragraphWidgets.Count > 0 && paragraphWidgets[0].ChildWidgets.Count > 0)
                            {
                                LineWidget lineWidget = paragraphWidgets[0].ChildWidgets[0] as LineWidget;
                                if (lineWidget.Children.Count > 0)
                                    element = lineWidget.Children[0] as ListTextElementBox;
                            }
                            if (element != null)
                                element.Text = document.GetListNumber(currentListLevel) + ".";
                        }
                    }
                }
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal override void LayoutItems(LayoutViewer viewer)
        {
            if (viewer.FieldEndParagraph == this)
                //Sets field end paragraph to null, inorder to hold relayouting with this paragraph.
                viewer.FieldEndParagraph = null;
            DocumentAdv ownerDocument = Document;
            if (paragraphWidgets != null
                && paragraphWidgets.Count > 0)
            {
                //Removes the paragraph widget.
                paragraphWidgets.Clear();
            }
            AddParagraphWidget(viewer.ClientActiveArea);
            if (!viewer.IsFieldCode)
            {
#if !WPF
                UIDispatcher.Execute(() =>
                {
#endif
                    if (!string.IsNullOrEmpty(ParagraphFormat.ListFormat.ListId))
                        LayoutList(viewer);
#if !WPF
                });
#endif
            }
            if (IsEmpty())
            {
                if (!viewer.IsFieldCode)
                {
#if !WPF
                    UIDispatcher.Execute(() =>
                    {
#endif
                        LayoutEmptyLineWidget(viewer);
#if !WPF
                    });
#endif
                }
            }
            else
            {
                for (int i = 0; i < Inlines.Count; i++)
                {
                    if (i == 0)
                    {
#if !WPF
                        UIDispatcher.Execute(() =>
                        {
#endif
                            viewer.UpdateClientWidth(-ParagraphFormat.FirstLineIndent);
#if !WPF
                        });
#endif
                    }
                    Inlines[i].LayoutItems(viewer);
                }
            }
            UpdateWidgetToPage(viewer);
        }
        /// <summary>
        /// Layouts the list.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        private void LayoutList(LayoutViewer viewer)
        {
            DocumentAdv document = Document;
            ListAdv currentList = document.GetListAdv(ParagraphFormat.ListFormat.ListId);
            if (currentList == null || currentList.AbstractList == null || currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber] == null)
                return;
            ListLevelAdv currentListLevel = currentList.AbstractList.Levels[ParagraphFormat.ListFormat.ListLevelNumber];
            ListTextElementBox element = new ListTextElementBox(this, currentListLevel);
            if (currentListLevel.ReadLocalValue(ListLevelAdv.BulletCharacterProperty) != DependencyProperty.UnsetValue)
                element.Text = currentListLevel.BulletCharacter;
            else
                element.Text = document.GetListNumber(currentListLevel) + ".";
            TextHelper.UpdateTextSize(element);
            if (element.Width % 30 != 0)
                element.Width = 30 * Math.Ceiling(element.Width / 30);
            viewer.CutFromLeft(viewer.ClientActiveArea.X + element.Width);
            //Adds the text element to the line
            viewer.LineElements.Add(element);
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal override void ClearWidgets()
        {
            if (paragraphWidgets != null
                && paragraphWidgets.Count > 0)
            {
                //Removes the paragraph widget.
                for (int i = 0; i < paragraphWidgets.Count; i++)
                {
                    ParagraphWidget widget = paragraphWidgets[i] as ParagraphWidget;
                    widget.Dispose();
                    paragraphWidgets.Remove(widget);
                    i--;
                }
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            ParagraphFormat.Dispose();
            ClearValue(ParagraphFormatProperty);
            SetOwner(null);
            ClearWidgets();
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                inline.Dispose();
                Inlines.Remove(inline);
                i--;
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Adds the paragraph widget.
        /// </summary>
        /// <param name="area">The area.</param>
        internal void AddParagraphWidget(Rect area)
        {
            ParagraphWidget paragraphWidget = new ParagraphWidget(this);
            paragraphWidget.Width = area.Width;
            paragraphWidget.Location = new Point(area.X, area.Y);
            paragraphWidgets.Add(paragraphWidget);
        }
        /// <summary>
        /// Adds the line widget.
        /// </summary>
        /// <returns></returns>
        internal LineWidget AddLineWidget()
        {
            LineWidget line = null;
            ParagraphWidget paragraphWidget = paragraphWidgets[paragraphWidgets.Count - 1] as ParagraphWidget;
            line = new LineWidget(paragraphWidget);
            line.Width = paragraphWidget.Width;
            paragraphWidget.ChildWidgets.Add(line);
            line.ParagraphWidget = paragraphWidget;
            return line;
        }
        /// <summary>
        /// Combines the paragraph widgets.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        internal void CombineParagraphWidgets(TableCellWidget cellWidget)
        {
            if (paragraphWidgets.Count == 0)
                return;
            ParagraphWidget paragraphWidget = paragraphWidgets[0] as ParagraphWidget;
            if (!cellWidget.ChildWidgets.Contains(paragraphWidget))
                paragraphWidget.UpdateContainerWidget(cellWidget);
            if (paragraphWidgets.Count == 1)
                return;
            for (int i = 1; i < paragraphWidgets.Count; i++)
            {
                ParagraphWidget widget = paragraphWidgets[i] as ParagraphWidget;
                widget.ShiftChildsToWidget(paragraphWidget);
                i--;
            }
        }
        /// <summary>
        /// Shifts the widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal override void ShiftWidgets(LayoutViewer viewer)
        {
            int index = 0;
            BodyWidget prevBodyWidget = GetBodyWidgetOfPreviousBlock(ref index);
            ParagraphWidget prevWidget = null;
            for (int i = 0; i < paragraphWidgets.Count; i++)
            {
                ParagraphWidget widget = paragraphWidgets[i] as ParagraphWidget;
                if (prevWidget != null)
                {
                    widget.ShiftToPreviousWidget(viewer, prevWidget);
                    if (widget.ChildWidgets.Count == 0)
                    {
                        i--;
                        continue;
                    }
                    prevWidget = null;
                    if (prevBodyWidget != widget.ContainerWidget)
                        prevBodyWidget = widget.ContainerWidget as BodyWidget;
                }
                if (viewer is FlowLayoutViewer || widget.IsFitInClientArea(viewer))
                {
                    //Check whether this widget is moved to previous container widget.
                    prevWidget = widget;
                    widget.Location = new Point(widget.Location.X, viewer.ClientActiveArea.Y);
                    viewer.CutFromTop(viewer.ClientActiveArea.Y + widget.Height);
                    //Moves the paragraph widget to previous body widget.
                    if (prevBodyWidget != widget.ContainerWidget)
                    {
                        index++;
                        widget.UpdateContainerWidget(prevBodyWidget, index);
                    }
                }
                else
                {
                    bool isSplittedToNewPage = widget.SplitWidget(viewer, prevBodyWidget, index + 1);
                    prevWidget = null;
                    if (prevBodyWidget != widget.ContainerWidget)
                    {
                        prevBodyWidget = widget.ContainerWidget as BodyWidget;
                        i--;
                    }
                    index = prevBodyWidget.ChildWidgets.IndexOf(widget);
                    if (isSplittedToNewPage)
                        prevBodyWidget = paragraphWidgets[i + 1].ContainerWidget as BodyWidget;
                }
            }
        }
        /// <summary>
        /// Relayouts the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        internal void Relayout(int startIndex)
        {
            if (BaseParent != null)
            {
                SectionAdv section = Section;
                DocumentAdv ownerDocument = section.Document;
                LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
                if (viewer.FieldEndParagraph != null && viewer.FieldEndParagraph.BaseParent == null)
                    //If field end mark or its entire owner is removed, sets the current paragraph as relayout end.
                    viewer.FieldEndParagraph = this;
                if (viewer.FieldToLayout != null)
                {
                    ParagraphAdv ownerParagraph = viewer.FieldToLayout.OwnerParagraph;
                    Inline fieldBegin = viewer.FieldToLayout;
                    viewer.FieldToLayout = null;
                    if (ownerParagraph != null && ownerParagraph.BaseParent != null)
                    {
                        startIndex = fieldBegin.GetIndexInOwnerCollection();
                        //If field separator or end mark or its entire owner is removed, relayouts from the field begin.
                        if (ownerParagraph != this)
                        {
                            ownerParagraph.Relayout(startIndex);
                            return;
                        }
                    }
                }

                if (viewer.BlockToShift == this)
                    Layout();
                else if (ownerDocument.OwnerControl.IsLayoutEnabled)
                {
                    if (viewer.Pages.Count == 0)
                        viewer.CreateNewPage(section);
                    if (IsInsideTable)
                    {
                        TableCellAdv cell = AssociatedCell.GetContainerCell();
                        cell.OwnerRow.Layout(viewer);
                    }
                    else
                    {
#if !WPF
                        UIDispatcher.Execute(() =>
                        {
#endif
                            //If the content is in text body, updates the client area based on section formattings.
                            viewer.UpdateClientArea(section.SectionFormat);
#if !WPF
                        });
#endif
                        //Updates list values of previous rendered paragraphs.
                        if (ownerDocument.OwnerControl.IsDocumentLoaded && !ownerDocument.OwnerControl.IsPastingContent)
                            ownerDocument.UpdateListItems(this);
                        //Updates the client area based on current paragraph.
                        viewer.UpdateClientArea(this, true);
                        if (!IsEmpty() && startIndex > 0)
                        {
                            Inline inline = Inlines[startIndex];
                            ElementBox prevElement = null;
                            LineWidget lineWidget = GetLineWidgetToClear(viewer, inline, ref prevElement);
                            if (lineWidget != null)
                            {
                                //Updates list values of this paragraphs.
                                UpdateListItems(null);
                                lineWidget.ParagraphWidget.Paragraph.ClearLineWidgets(viewer, lineWidget, prevElement);
                                //Relayouts the next items.
                                inline.RelayoutItems(viewer);
                                return;
                            }
                        }
                        Relayout(viewer, section);
                    }
                }
            }
        }
        /// <summary>
        /// Relayouts the specified viewer.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="section">The section.</param>
        private void Relayout(LayoutViewer viewer, SectionAdv section)
        {
            BodyWidget bodyWidget = ParagraphWidgets[0].ContainerWidget as BodyWidget;
            PreservePreviousElements(viewer);
            if (bodyWidget == null)
                viewer.CutFromTop(ParagraphWidgets[0].Location.Y);
            else
            {
                int widgetIndex = bodyWidget.ChildWidgets.IndexOf(ParagraphWidgets[0]);
                if (widgetIndex > 0)
                    viewer.CutFromTop(ParagraphWidgets[0].Location.Y);
                else
                {
                    List<BodyWidget> bodyWidgets = Section.BodyWidgets;
                    int bodyWidgetIndex = bodyWidgets.IndexOf(bodyWidget);
                    if (bodyWidgetIndex > 0)
                    {
                        bodyWidget = bodyWidgets[bodyWidgetIndex - 1];
                        Widget lastWidget = bodyWidget.ChildWidgets[bodyWidget.ChildWidgets.Count - 1] as Widget;
                        viewer.CutFromTop(lastWidget.Location.Y + lastWidget.Height);
                    }
                }
            }
            ClearWidgets();
            if (viewer.Pages.Count == 0)
            {
                viewer.CreateNewPage(section);
                viewer.UpdateClientArea(this, true);
            }
            LayoutItems(viewer);
            viewer.UpdateClientArea(this, false);
            LayoutNextItems(viewer);
        }
        /// <summary>
        /// Preserves the previous elements.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        private void PreservePreviousElements(LayoutViewer viewer)
        {
            if (paragraphWidgets.Count > 0)
            {
                ParagraphWidget paragraphWidget = paragraphWidgets[0] as ParagraphWidget;
                if (paragraphWidget.ChildWidgets.Count > 0)
                {
                    LineWidget lineWidget = paragraphWidget.ChildWidgets[0] as LineWidget;
                    double left = viewer.ClientActiveArea.X;
                    left += lineWidget.PreservePreviousElements(viewer);
                    double firstLineIndent = 0;
#if !WPF
                    UIDispatcher.Execute(() =>
                    {
#endif
                        firstLineIndent = ParagraphFormat.FirstLineIndent;
#if !WPF
                    });
#endif
                    //Since first line widget is taken first line indent is reduced with left.
                    if (left - firstLineIndent != viewer.ClientActiveArea.X)
                    {
                        //Updates list values of this paragraphs.
                        UpdateListItems(null);
                        viewer.CutFromLeft(left);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the line widget to clear.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="inline">The inline.</param>
        /// <param name="prevElement">The prev element.</param>
        /// <returns></returns>
        private LineWidget GetLineWidgetToClear(LayoutViewer viewer, Inline inline, ref ElementBox prevElement)
        {
            int index = Inlines.IndexOf(inline);
            LineWidget startLineWidget = null;
            if (index > 0)
            {
                Inline prevInline = Inlines[index - 1];
                prevElement = prevInline.GetPreviousElementBox();
                if (prevElement != null)
                    startLineWidget = prevElement.CurrentLineWidget;
            }
            if (startLineWidget == null && paragraphWidgets[0].ChildWidgets.Count > 0)
                startLineWidget = paragraphWidgets[0].ChildWidgets[0] as LineWidget;
            return startLineWidget;
        }
        /// <summary>
        /// Clears the line widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="startLineWidget">The start line widget.</param>
        /// <param name="prevElement">The prev element.</param>
        private void ClearLineWidgets(LayoutViewer viewer, LineWidget startLineWidget, ElementBox prevElement)
        {
            if (paragraphWidgets[0].ChildWidgets.Count == 0)
            {
                if (paragraphWidgets[0].ContainerWidget != null
                    && paragraphWidgets[0].ContainerWidget.ChildWidgets.Contains(paragraphWidgets[0]))
                {
                    paragraphWidgets[0].ContainerWidget.ChildWidgets.Remove(paragraphWidgets[0]);
                    paragraphWidgets[0].ContainerWidget.Height -= paragraphWidgets[0].Height;
                }
                viewer.CutFromTop(paragraphWidgets[0].Location.Y);
                return;
            }
            //Clears the line widget starting from current line.
            double top = startLineWidget.ParagraphWidget.Location.Y;
            double left = viewer.ClientActiveArea.X;
            int paraWidgetIndex = paragraphWidgets.IndexOf(startLineWidget.ParagraphWidget);
            for (int i = paraWidgetIndex; i < paragraphWidgets.Count; i++)
            {
                ParagraphWidget paragraphWidget = paragraphWidgets[i] as ParagraphWidget;
                if (paragraphWidget.ContainerWidget != null
                    && paragraphWidget.ContainerWidget.ChildWidgets.Contains(paragraphWidget))
                {
                    paragraphWidget.ContainerWidget.ChildWidgets.Remove(paragraphWidget);
                    paragraphWidget.ContainerWidget.Height -= paragraphWidget.Height;
                }
                if (i == paraWidgetIndex)
                {
                    paraWidgetIndex = -1;
                    int lineIndex = paragraphWidget.ChildWidgets.IndexOf(startLineWidget);
                    for (int j = lineIndex; j < paragraphWidget.ChildWidgets.Count; j++)
                    {
                        LineWidget lineWidget = paragraphWidget.ChildWidgets[j] as LineWidget;
                        if (j == lineIndex)
                        {
                            lineIndex = -1;
                            left += lineWidget.ClearChildWidgets(viewer, prevElement);
                        }
                        else
                            lineWidget.Dispose();
                        if (paragraphWidget.ChildWidgets.Contains(lineWidget))
                        {
                            paragraphWidget.ChildWidgets.Remove(lineWidget);
                            paragraphWidget.Height -= paragraphWidget.ChildWidgets.Count == 0 ? paragraphWidget.Height : lineWidget.Height;
                        }
                        j--;
                    }
                    top += paragraphWidget.Height;
                }
                else
                {
                    paragraphWidget.Dispose();
                    paragraphWidgets.Remove(paragraphWidget);
                    i--;
                }
            }
            viewer.CutFromTop(top);
            viewer.CutFromLeft(left);
        }
        /// <summary>
        /// Layouts the empty line widget.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void LayoutEmptyLineWidget(LayoutViewer viewer)
        {
            //Calculate line height and descent based on formatting defined in paragraph.
            double maxHeight = TextHelper.GetParagraphMarkSize(CharacterFormat).Height;
            double maxDescent = maxHeight - TextHelper.TextMeasurer.BaselineOffset;
            //Calculate the bottom position of current line - max height + line spacing.
            if (!double.IsNaN(viewer.LineElements.MaxTextElementHeight)
                && maxHeight < viewer.LineElements.MaxTextElementHeight)
            {
                maxHeight = viewer.LineElements.MaxTextElementHeight;
                maxDescent = maxHeight - viewer.LineElements.MaxTextElementBaselineOffset;
            }
            double beforeSpacing = GetBeforeSpacing();
            if (viewer.CurrentHeaderFooter == null && viewer is PageLayoutViewer
                && viewer.ClientActiveArea.Height < beforeSpacing + maxHeight)
                MoveToNextPage(viewer);
            //Gets line spacing.
            double lineSpacing = GetLineSpacing(maxHeight);
            LineWidget lineWidget = AddLineWidget();
            double topMargin = 0, bottomMargin = 0, leftMargin = 0, height = maxHeight;
            ListTextElementBox element = null;
            if (viewer.LineElements.Count > 0)
                element = viewer.LineElements[0] as ListTextElementBox;
            if (element != null)
            {
                //Updates the text to base line offset.
                double baselineOffset = element.BaselineOffset;
                topMargin = viewer.LineElements.MaxBaselineOffset - baselineOffset;
                bottomMargin = maxDescent - (element.Height - baselineOffset);
                height = element.Height;
            }
            //Updates line spacing, paragraph after/ before spacing and aligns the text to base line offset.
            LineSpacingType lineSpacingType = ParagraphFormat.LineSpacingType;
            if (lineSpacingType == LineSpacingType.Multiple)
            {
                if (lineSpacing > maxHeight)
                    bottomMargin += lineSpacing - maxHeight;
                else
                    topMargin += lineSpacing - maxHeight;
            }
            else if (lineSpacingType == LineSpacingType.Exactly)
                topMargin += lineSpacing - (topMargin + height + bottomMargin);
            else if (lineSpacing > topMargin + height + bottomMargin)
                topMargin += lineSpacing - (topMargin + height + bottomMargin);
            topMargin += beforeSpacing;
            bottomMargin += ParagraphFormat.AfterSpacing;
            if (element != null)
            {
                TextAlignment textAlignment = ParagraphFormat.TextAlignment;
                if (textAlignment == TextAlignment.Right) //Aligns the text as right justified.
                    leftMargin = viewer.ClientArea.Width - element.Width;
                else if (textAlignment == TextAlignment.Center) //Aligns the text as center justified.
                    leftMargin = (viewer.ClientArea.Width - element.Width) / 2;
                element.Margin = new Thickness(leftMargin, topMargin, 0, bottomMargin);
                element.CurrentLineWidget = lineWidget;
                lineWidget.Height = topMargin + height + bottomMargin;
                lineWidget.Children.Add(element);
            }
            lineWidget.Height = topMargin + height + bottomMargin;
            viewer.CutFromTop(viewer.ClientActiveArea.Y + lineWidget.Height);
            //Clears the previous line elements from collection.
            viewer.LineElements.Clear();
        }
        /// <summary>
        /// Gets the size of the paragraph mark.
        /// </summary>
        /// <param name="topMargin">The top margin.</param>
        /// <param name="bottomMargin">The bottom margin.</param>
        /// <returns></returns>
        internal Size GetParagraphMarkSize(ref double topMargin, ref double bottomMargin)
        {
            Size size = TextHelper.GetParagraphMarkSize(CharacterFormat);
            double baselineOffset = TextHelper.TextMeasurer.BaselineOffset;
            double maxHeight = size.Height;
            double maxBaselineOffset = baselineOffset;
            if (paragraphWidgets.Count > 0 && paragraphWidgets[0].ChildWidgets.Count > 0)
            {
                LineWidget lineWidget = paragraphWidgets[0].ChildWidgets[0] as LineWidget;
                if (lineWidget.Children.Count > 0 && lineWidget.Children[0] is ListTextElementBox
                    && maxHeight < (lineWidget.Children[0] as ListTextElementBox).Height)
                {
                    maxHeight = (lineWidget.Children[0] as ListTextElementBox).Height;
                    maxBaselineOffset = (lineWidget.Children[0] as ListTextElementBox).BaselineOffset;
                }
            }
            //Gets line spacing.
            double lineSpacing = GetLineSpacing(maxHeight);
            double beforeSpacing = GetBeforeSpacing();
            topMargin = maxBaselineOffset - baselineOffset;
            bottomMargin = maxHeight - maxBaselineOffset - (size.Height - baselineOffset);
            //Updates line spacing, paragraph after/ before spacing and aligns the text to base line offset.
            LineSpacingType lineSpacingType = ParagraphFormat.LineSpacingType;
            if (lineSpacingType == LineSpacingType.Multiple)
            {
                if (lineSpacing > maxHeight)
                    bottomMargin += lineSpacing - maxHeight;
                else
                    topMargin += lineSpacing - maxHeight;
            }
            else if (lineSpacingType == LineSpacingType.Exactly)
                topMargin += lineSpacing - (topMargin + size.Height + bottomMargin);
            else if (lineSpacing > topMargin + size.Height + bottomMargin)
                topMargin += lineSpacing - (topMargin + size.Height + bottomMargin);
            topMargin += beforeSpacing;
            bottomMargin += ParagraphFormat.AfterSpacing;
            return size;
        }
        /// <summary>
        /// Gets the before spacing.
        /// </summary>
        /// <returns></returns>
        internal double GetBeforeSpacing()
        {
            double beforeSpacing = 0;
            if (PreviousBlock is ParagraphAdv)
            {
                if ((PreviousBlock as ParagraphAdv).ParagraphFormat.AfterSpacing < ParagraphFormat.BeforeSpacing)
                    beforeSpacing = ParagraphFormat.BeforeSpacing - (PreviousBlock as ParagraphAdv).ParagraphFormat.AfterSpacing;
            }
            else
                beforeSpacing = ParagraphFormat.BeforeSpacing;
            return beforeSpacing;
        }
        /// <summary>
        /// Gets the line spacing.
        /// </summary>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns></returns>
        internal double GetLineSpacing(double maxHeight)
        {
            double lineSpacing = 0;
            switch (ParagraphFormat.LineSpacingType)
            {
                case LineSpacingType.AtLeast:
                case LineSpacingType.Exactly:
                    lineSpacing = ParagraphFormat.LineSpacing;
                    break;
                default:
                    lineSpacing = ParagraphFormat.LineSpacing * maxHeight;
                    break;
            }
            return lineSpacing;
        }
        /// <summary>
        /// Moves to next page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void MoveToNextPage(LayoutViewer viewer)
        {
            ParagraphWidget paragraphWidget = ParagraphWidgets[ParagraphWidgets.Count - 1] as ParagraphWidget;
            List<ElementBox> elements = null;
            if (viewer.LineElements.Count > 0)
            {
                elements = viewer.LineElements.ToList();
                viewer.LineElements.Clear();
            }
            BodyWidget prevBodyWidget = null;
            if (paragraphWidget.ChildWidgets.Count > 0)
            {
                //Updates the rendered line widgets to current page.
                UpdateWidgetToPage(viewer);
                prevBodyWidget = paragraphWidget.ContainerWidget as BodyWidget;
            }
            else
            {
                if (PreviousBlock is ParagraphAdv)
                    prevBodyWidget = (PreviousBlock as ParagraphAdv).ParagraphWidgets[(PreviousBlock as ParagraphAdv).ParagraphWidgets.Count - 1].ContainerWidget as BodyWidget;
                else if (PreviousBlock is TableAdv)
                    prevBodyWidget = (PreviousBlock as TableAdv).TableWidgets[(PreviousBlock as TableAdv).TableWidgets.Count - 1].ContainerWidget as BodyWidget;
            }
            //Create new page and move the contents of current line to next page
            int pageIndex = 0;
            if (prevBodyWidget != null)
                pageIndex = viewer.Pages.IndexOf(prevBodyWidget.Page);
            if (pageIndex == viewer.Pages.Count - 1
                || viewer.Pages[pageIndex + 1].Section != Section)
            {
                PageAdv page = viewer.CreateNewPage(Section);
                if (viewer.Pages[pageIndex + 1].Section != Section)
                    viewer.InsertPage(pageIndex + 1, page);
            }
            else
            {
                viewer.Pages[pageIndex + 1].BoundingRectangle = new Rect(viewer.Pages[pageIndex + 1].BoundingRectangle.X, viewer.Pages[pageIndex].BoundingRectangle.Bottom + 20, viewer.Pages[pageIndex + 1].BoundingRectangle.Width, viewer.Pages[pageIndex + 1].BoundingRectangle.Height);
                //Updates the client area.
#if !WPF
                UIDispatcher.Execute(() =>
                {
#endif
                    viewer.UpdateClientArea(Section.SectionFormat);
#if !WPF
                });
#endif
            }
            viewer.UpdateClientArea(this, true);
            if (paragraphWidget.ChildWidgets.Count > 0)
                AddParagraphWidget(viewer.ClientActiveArea);
            else
                paragraphWidget.UpdateWidgetLocation(viewer.ClientActiveArea);
            if (elements != null && viewer.LineElements.Count == 0)
            {
                double left = viewer.ClientActiveArea.X;
                foreach (ElementBox elt in elements)
                {
                    left += elt.Width;
                    viewer.LineElements.Add(elt);
                }
                viewer.CutFromLeft(left);
            }
        }
        /// <summary>
        /// Updates the widget to page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateWidgetToPage(LayoutViewer viewer)
        {
            ParagraphWidget paragraphWidget = ParagraphWidgets[ParagraphWidgets.Count - 1] as ParagraphWidget;
            if (IsInsideTable)
            {
                //Adds the paragraph widget to owner cell widget.
                TableCellWidget cellWidget = AssociatedCell.TableCellWidgets[AssociatedCell.TableCellWidgets.Count - 1];
                paragraphWidget.Height = viewer.ClientActiveArea.Y - paragraphWidget.Location.Y;
                cellWidget.Height = cellWidget.Height + paragraphWidget.Height;
                cellWidget.ChildWidgets.Add(paragraphWidget);
                paragraphWidget.ContainerWidget = cellWidget;
            }
            else
            {
                paragraphWidget.Height = viewer.ClientActiveArea.Y - paragraphWidget.Location.Y;
                //Adds the paragraph widget to the Header Footer/ Body widget.
                UpdateWidgetsToBody(viewer, paragraphWidget);
                if (viewer is PageLayoutViewer && (viewer as PageLayoutViewer).VisiblePages.Contains((paragraphWidget.ContainerWidget as BodyWidget).Page))
                    paragraphWidget.Render((paragraphWidget.ContainerWidget as BodyWidget).Page);
                else if (viewer is FlowLayoutViewer)
                    paragraphWidget.Render(viewer as FlowLayoutViewer);
            }
        }
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <returns></returns>
        internal double GetLength()
        {
            double index = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                index += Inlines[i].Length;
            }
            return index;
        }
        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEmpty()
        {
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Gets the start offset.
        /// </summary>
        /// <returns></returns>
        internal double GetStartOffset()
        {
            double startOffset = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                    return startOffset;
                startOffset += inline.Length;
            }
            return startOffset;
        }
        /// <summary>
        /// Gets the end offset.
        /// </summary>
        /// <returns></returns>
        internal double GetEndOffset()
        {
            double startOffset = 0;
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                    startOffset = count + inline.Length;
                count += inline.Length;
            }
            return startOffset;
        }
        /// <summary>
        /// Determines whether this instance has valid inline in the specified start and end.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        ///   <c>true</c> if this instance has valid inline in the specified start and end; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasValidInline(Inline start, Inline end)
        {
            for (int i = Inlines.IndexOf(start); i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (inline == end)
                    return false;
                if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the previous valid offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal double GetPreviousValidOffset(double offset)
        {
            if (offset == 0)
                return 0;
            double validOffset = 0;
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (offset <= count + inline.Length)
                    return offset - 1 == count ? validOffset : offset - 1;
                if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                    validOffset = count + inline.Length;
                count += inline.Length;
            }
            return offset - 1 == count ? validOffset : offset - 1;
        }
        /// <summary>
        /// Gets the next valid offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal double GetNextValidOffset(double offset)
        {
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline.Length == 0)
                    continue;
                if (offset < count + inline.Length)
                {
                    if (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                        || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                        return (offset > count ? offset : count) + 1;
                }
                count += inline.Length;
            }
            return offset;
        }
        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal double GetOffset(Inline inline, int index)
        {
            if (inline == null)
                return index;
            double textIndex = index;
            for (int i = 0; i < Inlines.Count; i++)
            {
                if (inline == Inlines[i])
                    break;
                textIndex += Inlines[i].Length;
            }
            return textIndex;
        }
        /// <summary>
        /// Gets the inline.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="indexInInline">The index in inline.</param>
        /// <returns></returns>
        internal Inline GetInline(double offset, ref int indexInInline)
        {
            Inline inline = null;
            double count = 0;
            bool isStarted = false;
            for (int i = 0; i < Inlines.Count; i++)
            {
                inline = Inlines[i];
                if (!isStarted && (inline is SpanAdv || inline is ImageContainerAdv || inline is UIContainerAdv
                    || (inline is FieldCharacterAdv && (inline as FieldCharacterAdv).IsLinkedFieldCharacter())))
                    isStarted = true;
                if (isStarted && offset <= count + inline.Length)
                {
                    indexInInline = (int)(offset - count);
                    return inline;
                }
                count += inline.Length;
            }
            if (offset > count)
                indexInInline = inline == null ? (int)offset : inline.Length;
            return inline;
        }
        /// <summary>
        /// Gets the next start inline.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal Inline GetNextStartInline(double offset)
        {
            int indexInInline = 0;
            Inline inline = GetInline(offset, ref indexInInline);
            if (inline != null && indexInInline == inline.Length && inline.NextNode is FieldCharacterAdv)
            {
                Inline nextValidInline = (inline.NextNode as Inline).GetNextValidInline();
                if (nextValidInline is FieldBeginAdv)
                    inline = nextValidInline;
            }
            return inline;
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal LineWidget GetLineWidget(double offset)
        {
            LineWidget lineWidget = null;
            if (IsEmpty())
            {
                if (paragraphWidgets.Count > 0 && paragraphWidgets[0].ChildWidgets.Count > 0)
                    lineWidget = paragraphWidgets[0].ChildWidgets[0] as LineWidget;
            }
            else
            {
                int indexInInline = 0;
                Inline inline = GetInline(offset, ref indexInInline);
                lineWidget = inline.GetLineWidget(indexInInline);
            }
            return lineWidget;
        }
        /// <summary>
        /// Gets the physical position.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        internal Point GetPhysicalPosition(double offset)
        {
            if (IsEmpty())
            {
                double left = paragraphWidgets[0].Location.X;
                if (paragraphWidgets[0].ChildWidgets.Count > 0)
                {
                    LineWidget lineWidget = paragraphWidgets[0].ChildWidgets[0] as LineWidget;
                    left = lineWidget.GetLeft();
                }
                double topMargin = 0, bottomMargin = 0;
#if !WPF
                UIDispatcher.Execute(() =>
                {
#endif
                    Size size = GetParagraphMarkSize(ref topMargin, ref bottomMargin);
                    if (offset > 0)
                        left += size.Width;
#if !WPF
                });
#endif
                return new Point(left, paragraphWidgets[0].Location.Y + topMargin);
            }
            else
            {
                int indexInInline = 0;
                Inline inline = GetInline(offset, ref indexInInline);
                if (inline.Length == indexInInline && inline.NextNode != null && inline.Elements.Count > 0 && (inline.NextNode as Inline).Elements.Count > 0
                    && inline.Elements[inline.Elements.Count - 1].CurrentLineWidget != (inline.NextNode as Inline).Elements[0].CurrentLineWidget)
                {
                    //Handled specifically to move the cursor at start of next line.
                    inline = inline.NextNode as Inline;
                    indexInInline = 0;
                }
                return inline.GetPhysicalPosition(indexInInline);
            }
        }
        /// <summary>
        /// Gets the end position.
        /// </summary>
        /// <returns></returns>
        internal Point GetEndPosition()
        {
            Widget widget = paragraphWidgets[paragraphWidgets.Count - 1] as Widget;
            double left = widget.Location.X;
            double top = widget.Location.Y;
            LineWidget lineWidget = null;
            if (widget.ChildWidgets.Count > 0)
            {
                lineWidget = widget.ChildWidgets[widget.ChildWidgets.Count - 1] as LineWidget;
                left += lineWidget.GetWidth(false);
            }
            if (lineWidget != null)
                top = lineWidget.GetTop();
            double topMargin = 0, bottomMargin = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                Size size = GetParagraphMarkSize(ref topMargin, ref bottomMargin);
#if !WPF
            });
#endif
            return new Point(left, top + topMargin);
        }
        /// <summary>
        /// Gets the hyperlink display text.
        /// </summary>
        /// <param name="fieldSeparator">The field separator.</param>
        /// <param name="fieldEnd">The field end.</param>
        /// <param name="isNestedField">if set to <c>true</c> [is nested field].</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        internal string GetHyperlinkDisplayText(Inline fieldSeparator, Inline fieldEnd, ref bool isNestedField, ref CharacterFormat format)
        {
            if (this != fieldEnd.OwnerParagraph)
            {
                isNestedField = true;
                return "<<Selection in Document>>";
            }
            string displayText = "";
            int index = Inlines.IndexOf(fieldSeparator);
            for (int i = index + 1; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (inline == fieldEnd)
                    break;
                if (inline is SpanAdv)
                {
                    displayText += (inline as SpanAdv).Text;
                    format = inline.CharacterFormat;
                }
                else if (inline is FieldCharacterAdv)
                {
                    if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
                    {
                        if ((inline as FieldBeginAdv).FieldSeparator == null)
                            index = Inlines.IndexOf((inline as FieldBeginAdv).FieldEnd);
                        else
                            index = Inlines.IndexOf((inline as FieldBeginAdv).FieldSeparator);
                    }
                }
                else
                {
                    isNestedField = true;
                    return "<<Selection in Document>>";
                }
            }
            return displayText;
        }
        /// <summary>
        /// Gets the cloned field result.
        /// </summary>
        /// <param name="fieldSeparator">The field separator.</param>
        /// <returns></returns>
        internal ParagraphAdv GetClonedFieldResult(Inline fieldSeparator)
        {
            ParagraphAdv paragraph = new ParagraphAdv();
            paragraph.CharacterFormat.CopyFormat(CharacterFormat);
            paragraph.ParagraphFormat.CopyFormat(ParagraphFormat);
            int index = Inlines.IndexOf(fieldSeparator);
            for (int i = index; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                paragraph.Inlines.Add(inline.Clone());
            }
            return paragraph;
        }
        /// <summary>
        /// Gets the cloned field result.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="fieldSeparator">The field separator.</param>
        internal void GetClonedFieldResult(SelectionAdv selection, Inline fieldSeparator)
        {
            int index = Inlines.IndexOf(fieldSeparator);
            for (int i = index; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                selection.InsertInlineInternal(inline.Clone());
            }
        }
        #endregion

        #region Remove Character at Offset
        /// <summary>
        /// Removes at offset.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <param name="offset">The offset.</param>
        internal void RemoveAtOffset(HistoryInfo historyInfo, double offset)
        {
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (offset < count + inline.Length)
                {
                    int indexInInline = (int)(offset - count);
                    if (offset == count && inline.Length == 1)
                    {
                        Inlines.Remove(inline);
                        historyInfo.RemovedNodes.Add(inline);
                    }
                    else
                    {
                        SpanAdv span = new SpanAdv();
                        span.CharacterFormat.CopyFormat(inline.CharacterFormat);
                        span.Text = (inline as SpanAdv).Text.Substring(indexInInline, 1);
                        (inline as SpanAdv).Text = (inline as SpanAdv).Text.Remove(indexInInline, 1);
                        historyInfo.RemovedNodes.Add(span);
                    }
                    break;
                }
                count += inline.Length;
            }
        }
        #endregion

        #region Insert Paragraph
        /// <summary>
        /// Inserts the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="insertAfter">if set to <c>true</c> insert after.</param>
        internal void InsertParagraph(ParagraphAdv paragraph, double offset, bool insertAfter)
        {
            if (insertAfter)
            {
                double length = GetLength();
                MoveInlines(paragraph, paragraph.Inlines.Count, offset, length);
            }
            else if (offset > 0)
                MoveInlines(paragraph, 0, 0, offset);
            int insertIndex = GetIndexInOwnerCollection();
            if (insertAfter)
                insertIndex++;
            ((Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(insertIndex, paragraph);
        }
        #endregion

        #region Split Paragraph
        /// <summary>
        /// Splits the paragraph.
        /// </summary>
        /// <param name="offset">The offset.</param>
        internal void SplitParagraph(double offset)
        {
            int insertIndex = 0;
            ParagraphAdv paragraph = new ParagraphAdv();
            if (offset == GetLength())
            {
                //ToDo in future: Need to skip copying formattings to new paragraph, if the style for following paragraph is same style.
                //Copies the format to new paragraph.
                paragraph.ParagraphFormat.CopyFormat(ParagraphFormat);
                paragraph.CharacterFormat.CopyFormat(CharacterFormat);
                insertIndex++;
            }
            else
            {
                paragraph.ParagraphFormat.CopyFormat(ParagraphFormat);
                paragraph.CharacterFormat.CopyFormat(CharacterFormat);
                if (offset > 0)
                    MoveInlines(paragraph, 0, 0, offset);
            }
            insertIndex += GetIndexInOwnerCollection();
            ((Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(insertIndex, paragraph);
        }
        /// <summary>
        /// Splits the paragraph and adds the new paragraph before the splitted paragraph.
        /// </summary>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="endOffset">The end offset.</param>
        /// <returns></returns>
        internal ParagraphAdv SplitParagraph(double startOffset, double endOffset)
        {
            ParagraphAdv paragraph = new ParagraphAdv();
            paragraph.ParagraphFormat.CopyFormat(ParagraphFormat);
            paragraph.CharacterFormat.CopyFormat(CharacterFormat);
            MoveInlines(paragraph, 0, startOffset, endOffset);
            //Inserts new paragraph in the current text position.
            int insertIndex = GetIndexInOwnerCollection();
            ((Owner as CompositeNode).ChildNodes as BlockAdvCollection).Insert(insertIndex, paragraph);
            return paragraph;
        }
        #endregion

        #region Remove Selected contents
        /// <summary>
        /// Removes the content of the selected.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        internal bool RemoveSelectedContent(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            //If end is not table end and start is outside the table, then skip removing the contents and move caret to start position.
            if (end.Paragraph.IsInsideTable
                && end.Paragraph != end.Paragraph.AssociatedCell.OwnerTable.GetLastParagraphInLastCell()
                && (!start.Paragraph.IsInsideTable || start.Paragraph.AssociatedCell.OwnerTable != end.Paragraph.AssociatedCell.OwnerTable))
                return false;

            DeleteSelectedContent(selection, start, end, 2);
            return true;
        }
        #endregion

        #region Highlight Selected Contents
        /// <summary>
        /// Highlights the content of the selected.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void HighlightSelectedContent(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.Highlight(selection, start, end);
            else
            {
                Inline inline = null;
                int index = 0;
                if (!selection.OwnerControl.IsReadOnlyMode && this == end.Paragraph && start.Offset + 1 == end.Offset)
                    inline = GetInline(end.Offset, ref index);
                if (inline is ImageContainerAdv && selection.OwnerControl.IsDocumentLoaded)
                {
                    ImageElementBox elementBox = inline.GetElementBox(ref index) as ImageElementBox;
                    //Adds image resizer to viewer and make it visible around image.
                    selection.OwnerControl.Viewer.PositionImageResizer(elementBox, start, end);
                    selection.OwnerControl.Viewer.ShowImageResizer();
                }
                else
                    Highlight(selection, start, end);
            }
        }
        /// <summary>
        /// Gets the start line widget.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="startElement">The start element.</param>
        /// <param name="selectionStartIndex">Start index of the selection.</param>
        /// <returns></returns>
        private LineWidget GetStartLineWidget(TextPosition start, ref ElementBox startElement, ref int selectionStartIndex)
        {
            double offset = (this == start.Paragraph ? start.Offset : GetStartOffset());
            Inline startInline = GetInline(offset, ref selectionStartIndex);
            if (startInline is FieldCharacterAdv)
                startInline = (startInline as FieldCharacterAdv).GetRenderedInline(ref selectionStartIndex);
            if (offset == GetLength() + 1)
                selectionStartIndex++;
            if (startInline == null)
            {
                if (ParagraphWidgets.Count > 0)
                {
                    ParagraphWidget widget = ParagraphWidgets[0] as ParagraphWidget;
                    return widget.ChildWidgets.Count > 0 ? widget.ChildWidgets[0] as LineWidget : null;
                }
                else
                    return null;
            }
            else
            {
                startElement = startInline.GetElementBox(ref selectionStartIndex);
                if (startElement == null)
                    return startInline.GetLineWidget(selectionStartIndex);
                return startElement.CurrentLineWidget;
            }
        }
        /// <summary>
        /// Gets the end line widget.
        /// </summary>
        /// <param name="end">The end.</param>
        /// <param name="endElement">The end element.</param>
        /// <param name="selectionEndIndex">End index of the selection.</param>
        /// <returns></returns>
        private LineWidget GetEndLineWidget(TextPosition end, ref ElementBox endElement, ref int selectionEndIndex)
        {
            Inline endInline = end.Paragraph.GetInline(end.Offset, ref selectionEndIndex);
            if (endInline is FieldCharacterAdv)
                endInline = (endInline as FieldCharacterAdv).GetRenderedInline(ref selectionEndIndex);
            if (endInline == null)
            {
                if (end.Offset == end.Paragraph.GetLength() + 1)
                    selectionEndIndex = 1;
                if (end.Paragraph.ParagraphWidgets.Count > 0)
                {
                    ParagraphWidget widget = end.Paragraph.ParagraphWidgets[end.Paragraph.ParagraphWidgets.Count - 1] as ParagraphWidget;
                    return widget.ChildWidgets.Count > 0 ? widget.ChildWidgets[widget.ChildWidgets.Count - 1] as LineWidget : null;
                }
                else
                    return null;
            }
            else
            {
                if (end.Offset == end.Paragraph.GetLength() + 1)
                    selectionEndIndex = endInline.Length + 1;
                endElement = endInline.GetElementBox(ref selectionEndIndex);
                if (endElement == null)
                    return endInline.GetLineWidget(selectionEndIndex);
                return endElement.CurrentLineWidget;
            }
        }
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void Highlight(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            int selectionStartIndex = 0, selectionEndIndex = 0;
            ElementBox startElement = null, endElement = null;
            LineWidget startLineWidget = GetStartLineWidget(start, ref startElement, ref selectionStartIndex);
            LineWidget endLineWidget = GetEndLineWidget(end, ref endElement, ref selectionEndIndex);
            double top = startLineWidget.GetTop();
            double left = startLineWidget.GetLeft(startElement, selectionStartIndex);
            if (startLineWidget != null && startLineWidget == endLineWidget)
            {
                //Selection ends in current line.
                double right = endLineWidget.GetLeft(endElement, selectionEndIndex);
                selection.CreateHighlightBorder(startLineWidget, right - left, left, top);
            }
            else
            {
                ParagraphAdv paragraph = this;
                if (startLineWidget != null)
                {
                    if (paragraph != startLineWidget.ParagraphWidget.Paragraph)
                        paragraph = startLineWidget.ParagraphWidget.Paragraph;
                    selection.CreateHighlightBorder(startLineWidget, startLineWidget.GetWidth(true) - (left - startLineWidget.ParagraphWidget.Location.X), left, top);
                    int lineIndex = startLineWidget.ParagraphWidget.ChildWidgets.IndexOf(startLineWidget);
                    //Iterates to last item of paragraph or selection end.
                    int startParagraphWidget = paragraph.ParagraphWidgets.IndexOf(startLineWidget.ParagraphWidget);
                    for (int i = startParagraphWidget; i < paragraph.ParagraphWidgets.Count; i++)
                    {
                        if (i == startParagraphWidget)
                            lineIndex += 1;
                        (paragraph.ParagraphWidgets[i] as ParagraphWidget).Highlight(selection, lineIndex, endLineWidget, endElement, selectionEndIndex);
                        if (paragraph.ParagraphWidgets[i] == endLineWidget.ParagraphWidget)
                            return;
                        else
                            lineIndex = 0;
                    }
                }
                BlockAdv block = paragraph.GetNextRenderedBlock();
                if (block != null)
                    block.Highlight(selection, start, end);
            }
        }
        #endregion

        #region Delete Selected Contents
        /// <summary>
        /// Deletes the content of the selected.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="editAction">The edit action.</param>
        internal void DeleteSelectedContent(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction)
        {
            int indexInInline = 0;
            Inline inline = start.Paragraph.GetInline(start.Offset, ref indexInInline);
            if (inline != null)
                inline = inline.GetNextRenderedInline(indexInInline);
            if (inline is FieldBeginAdv && (inline as FieldBeginAdv).FieldEnd != null)
            {
                double fieldEndOffset = (inline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetOffset((inline as FieldBeginAdv).FieldEnd, 1);
                string fieldEndIndex = (inline as FieldBeginAdv).FieldEnd.OwnerParagraph.GetHierarchicalIndex(fieldEndOffset.ToString());
                string selectionEndIndex = end.GetHierarchicalIndex();
                if (!TextPosition.IsForwardSelection(fieldEndIndex, selectionEndIndex))
                {
                    //If selection end is after field begin, moves selection start to field separator.
                    start.MoveToInline((inline as FieldBeginAdv).FieldSeparator, 1);
                    selection.EditPosition = start.GetHierarchicalIndex();
                    if (selection.CurrentHistoryInfo != null)
                        selection.CurrentHistoryInfo.InsertPosition = selection.EditPosition;
                }
            }
            indexInInline = 0;
            inline = end.Paragraph.GetInline(end.Offset, ref indexInInline);
            if (inline != null)
                inline = inline.GetNextRenderedInline(indexInInline);
            if (inline is FieldEndAdv && (inline as FieldEndAdv).FieldBegin != null)
            {
                double fieldBeginOffset = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetOffset((inline as FieldEndAdv).FieldBegin, 0);
                string fieldBeginIndex = (inline as FieldEndAdv).FieldBegin.OwnerParagraph.GetHierarchicalIndex(fieldBeginOffset.ToString());
                string selectionStartIndex = start.GetHierarchicalIndex();
                if (!TextPosition.IsForwardSelection(selectionStartIndex, fieldBeginIndex))
                {
                    //If field begin is before selection start, move selection end to inline item before field end.
                    Inline prevInline = inline.GetPreviousTextInline();
                    if (prevInline == null)
                        end.MoveBackward();
                    else
                        end.MoveToInline(prevInline, prevInline.Length);
                }
            }
            if (start.Paragraph != this)
            {
                start.Paragraph.DeleteSelectedContent(selection, start, end, editAction);
                return;
            }
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.Delete(selection, start, end, editAction);
            else
                Delete(selection, start, end, editAction);
        }
        /// <summary>
        /// Determines whether to combine with next paragraph.
        /// </summary>
        /// <param name="end">The end.</param>
        /// <returns>
        ///   <c>true</c> if combine with next paragraph; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCombineParagraph(TextPosition end)
        {
            BlockAdv block = end.Paragraph.GetPreviousBlock();
            if (block != null)
            {
                if (block is ParagraphAdv)
                    return this == block;
                else
                {
                    if (IsInsideTable && (block as TableAdv).Contains(AssociatedCell))
                        return false;
                    return end.Offset == 0;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified end paragraph is in adjacent table of this instance.
        /// </summary>
        /// <param name="endParagraph">The end paragraph.</param>
        /// <returns>
        ///   <c>true</c> if the specified end paragraph is in adjacent table of this instance; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEndInAdjacentTable(ParagraphAdv endParagraph)
        {
            string start = GetHierarchicalIndex("");
            string end = endParagraph.GetHierarchicalIndex("");
            string[] selectionStart = start.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] selectionEnd = end.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return selectionStart.Length < selectionEnd.Length;
        }
        /// <summary>
        /// Deletes the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="editAction">The edit action.</param>
        internal override void Delete(SelectionAdv selection, TextPosition start, TextPosition end, byte editAction)
        {
            double paragraphStart = GetStartOffset();
            double endParagraphStartOffset = end.Paragraph.GetStartOffset();
            double startOffset = paragraphStart;
            bool isCombineNextParagraph = false;
            double length = GetLength();
            ParagraphAdv currentParagraph = this;
            SectionAdv section = Section;
            if (this == start.Paragraph)
            {
                startOffset = start.Offset;
                if (end.Paragraph.IsInsideTable)
                    isCombineNextParagraph = IsEndInAdjacentTable(end.Paragraph);
            }
            BlockAdv block = GetNextRenderedBlock();
            if (startOffset > paragraphStart && startOffset == length && (this == end.Paragraph && end.Offset == startOffset + 1
                || block == end.Paragraph && end.Offset == endParagraphStartOffset))
                isCombineNextParagraph = true;
            if (startOffset > paragraphStart && (end.Paragraph != this || end.Offset > length))
            {
                //If selection start is after paragraph start
                //And selection doesnot end with this paragraph Or selection include paragraph mark.
                if (editAction == 4)
                    Copy(selection, startOffset);
                else
                {
                    currentParagraph = SplitParagraph(0, startOffset);
                    if (editAction > 2)
                        Copy(selection);
                    //Removes the current paragraph.
                    RemoveBlock();
                    selection.CurrentHistoryInfo.RemovedNodes.Add(this);
                }
                if (block != null && end.Paragraph != this
                    && !(block == end.Paragraph && end.Offset == endParagraphStartOffset))
                {
                    SectionAdv nextSection = block.Section;
                    if (section != nextSection)
                    {
                        if (editAction < 4)
                            section.CombineSection(selection, nextSection);
                        //Copies the section properties, if this is last paragraph of section.
                        if (editAction > 2)
                            selection.CopySectionFormat(section);
                    }
                    block.Delete(selection, start, end, editAction);
                }
            }
            else if (end.Paragraph == this && end.Offset <= length)
            {
                //If selection end with this paragraph and selection doesnot include paragraph mark.
                if (end.Offset > paragraphStart)
                    //Removes the splitted paragraph.
                    RemoveInlines(selection, startOffset, end.Offset, editAction);
            }
            else
            {
                ParagraphAdv newParagraph = null;
                ParagraphAdv prevParagraph = PreviousBlock as ParagraphAdv;
                if (editAction > 2)
                    DocxExporting.SerializeParagraph(this, selection.WordDocument.LastSection.Body);
                if (editAction < 4)
                {
                    //Checks whether this is last paragraph of owner textbody and previousblock is not paragraph.
                    if (this == end.Paragraph && NextBlock == null && prevParagraph == null)
                    //(prevParagraph == null || isBackSpace && !(block is ParagraphAdv))
                    {
                        //Adds an empty paragraph, to ensure minimal content.
                        newParagraph = new ParagraphAdv();
                        if (editAction == 1)
                        {
                            newParagraph.CharacterFormat.CopyFormat(CharacterFormat);
                            newParagraph.ParagraphFormat.CopyFormat(ParagraphFormat);
                        }
                        ((Owner as CompositeNode).ChildNodes as BlockAdvCollection).Add(newParagraph);
                    }
                    RemoveBlock();
                    selection.CurrentHistoryInfo.RemovedNodes.Add(this);
                    if (newParagraph != null)
                    {
                        selection.EditPosition = newParagraph.GetHierarchicalIndex("0");
                        double offset = newParagraph.GetLength() + 1;
                        selection.CurrentHistoryInfo.EndPosition = newParagraph.GetHierarchicalIndex(offset.ToString());
                    }
                    else if (this == end.Paragraph && NextBlock == null && prevParagraph != null)
                    {
                        if (block == null)
                        {
                            double offset = prevParagraph.GetLength();
                            selection.EditPosition = prevParagraph.GetHierarchicalIndex(offset.ToString());
                            selection.CurrentHistoryInfo.InsertPosition = selection.EditPosition;
                            selection.CurrentHistoryInfo.EndPosition = selection.EditPosition;
                        }
                        else
                        {
                            double offset = GetLength() + 1;
                            prevParagraph = block as ParagraphAdv;
                            if (block is TableAdv)
                                prevParagraph = (block as TableAdv).GetFirstParagraphInFirstCell();
                            selection.EditPosition = prevParagraph.GetHierarchicalIndex("0");
                        }
                    }
                }
                if (end.Paragraph != this && block != null)
                {
                    SectionAdv nextSection = block.Section;
                    if (section != nextSection)
                    {
                        if (editAction < 4)
                            section.CombineSection(selection, nextSection);
                        //Copies the section properties, if this is last paragraph of section.
                        if (editAction > 2)
                            selection.CopySectionFormat(section);
                    }
                    block.Delete(selection, start, end, editAction);
                }
            }
            if (selection.OwnerControl.History.IsUndoing || selection.OwnerControl.History.IsRedoing)
            {
                ParagraphAdv nextParagraph = currentParagraph.GetNextParagraph();
                if (nextParagraph == end.Paragraph && this == start.Paragraph
                    && selection.CurrentHistoryInfo.Action == Actions.Paste)
                {
                    //Combines the current paragraph with end paragraph specific for undo/redo paste action.
                    int insertIndex = 0;
                    for (int i = 0; i < currentParagraph.Inlines.Count; i++)
                    {
                        Inline inline = currentParagraph.Inlines[i];
                        i--;
                        nextParagraph.Inlines.Insert(insertIndex, inline);
                        insertIndex++;
                    }
                    currentParagraph.RemoveBlock();
                    isCombineNextParagraph = false;
                    string offset = selection.EditPosition.Substring(selection.EditPosition.LastIndexOf(";") + 1);
                    selection.EditPosition = nextParagraph.GetHierarchicalIndex(offset);
                }
            }
            if (isCombineNextParagraph)
                currentParagraph.DeleteParagraphMark(selection, editAction);
        }
        /// <summary>
        /// Deletes the paragraph mark.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="editAction">The edit action.</param>
        internal void DeleteParagraphMark(SelectionAdv selection, byte editAction)
        {
            ParagraphAdv nextParagraph = GetNextParagraph();
            if (IsInsideTable && NextBlock == null || nextParagraph == null)
                return;
            SectionAdv section = Section;
            TableAdv table = GetNextRenderedBlock() as TableAdv;
            DocIO.DLS.WParagraph wParagraph = null;
            if (editAction > 2)
            {
                wParagraph = new DocIO.DLS.WParagraph(selection.WordDocument);
                //Adds the copied contents to DocIO Word document instance.
                selection.WordDocument.LastSection.Body.ChildEntities.Add(wParagraph);
            }
            if (nextParagraph.IsInsideTable && table != null && table.Contains(nextParagraph.AssociatedCell))
            {
                if (wParagraph != null)
                {
                    DocxExporting.SerializeCharacterFormat(CharacterFormat, wParagraph.BreakCharacterFormat);
                    DocxExporting.SerializeParagraphFormat(ParagraphFormat, wParagraph.ParagraphFormat);
                }
                if (editAction < 4)
                {
                    SectionAdv nextSection = table.Section;
                    if (section != nextSection)
                        section.CombineSection(selection, nextSection);
                    double offset = 0;
                    for (int i = Inlines.Count - 1; i >= 0; i--)
                    {
                        Inline inline = Inlines[i];
                        offset += inline.Length;
                        nextParagraph.Inlines.Insert(0, inline);
                    }
                    RemoveBlock();
                    if (offset > 0)
                        selection.EditPosition = nextParagraph.GetHierarchicalIndex(offset.ToString());
                }
            }
            else
            {
                if (wParagraph != null)
                {
                    DocxExporting.SerializeCharacterFormat(nextParagraph.CharacterFormat, wParagraph.BreakCharacterFormat);
                    DocxExporting.SerializeParagraphFormat(nextParagraph.ParagraphFormat, wParagraph.ParagraphFormat);
                }
                if (editAction < 4)
                {
                    SectionAdv nextSection = nextParagraph.Section;
                    if (section != nextSection)
                        section.CombineSection(selection, nextSection);
                    for (int i = 0; i < nextParagraph.Inlines.Count; i++)
                    {
                        Inline inline = nextParagraph.Inlines[i];
                        i--;
                        Inlines.Add(inline);
                    }
                    nextParagraph.RemoveBlock();
                    selection.CurrentHistoryInfo.RemovedNodes.Add(nextParagraph);
                }
            }
        }
        /// <summary>
        /// Removes the inlines.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="endOffset">The end offset.</param>
        /// <param name="editAction">The edit action.</param>
        private void RemoveInlines(SelectionAdv selection, double startOffset, double endOffset, byte editAction)
        {
            //Copy as DocIO instance.
            DocIO.DLS.WParagraph wParagraph = editAction > 2 ? new DocIO.DLS.WParagraph(selection.WordDocument) : null;
            //Removes the inline items between selection start and end to new paragraph.
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (startOffset >= count + inline.Length)
                {
                    count += inline.Length;
                    continue;
                }
                int startIndex = 0;
                if (startOffset > count)
                    startIndex = (int)(startOffset - count);
                int endIndex = (int)(endOffset - count);
                if (endIndex > inline.Length)
                    endIndex = inline.Length;
                if (startIndex > 0)
                    count += startIndex;
                if (startIndex == 0 && endIndex == inline.Length)
                {
                    if (wParagraph != null)
                        DocxExporting.SerializePargraphItems(inline, wParagraph, true);
                    if (editAction < 4)
                    {
                        Inlines.Remove(inline);
                        i--;
                        selection.CurrentHistoryInfo.RemovedNodes.Add(inline);
                    }
                }
                else if (inline is SpanAdv)
                {
                    if (wParagraph != null)
                    {
                        DocIO.DLS.IWTextRange wTextRange = wParagraph.AppendText((inline as SpanAdv).Text.Substring(startIndex, endIndex - startIndex));
                        DocxExporting.SerializeCharacterFormat(inline.CharacterFormat, wTextRange.CharacterFormat);
                    }
                    if (editAction < 4)
                    {
                        SpanAdv span = new SpanAdv();
                        span.CharacterFormat.CopyFormat(inline.CharacterFormat);
                        span.Text = (inline as SpanAdv).Text.Substring(startIndex, endIndex - startIndex);
                        selection.CurrentHistoryInfo.RemovedNodes.Add(span);
                        (inline as SpanAdv).Text = (inline as SpanAdv).Text.Remove(startIndex, endIndex - startIndex);
                    }
                }
                if (endOffset <= count + endIndex - startIndex)
                    break;
                count += endIndex - startIndex;
            }
            if (wParagraph != null)
                //Adds the copied contents to DocIO Word document instance.
                selection.WordDocument.LastSection.Body.ChildEntities.Add(wParagraph);
        }
        /// <summary>
        /// Moves the inlines.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="insertIndex">Index of the insert.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="endOffset">The end offset.</param>
        internal void MoveInlines(ParagraphAdv paragraph, int insertIndex, double startOffset, double endOffset)
        {
            //Moves the inline items between selection start and end to new paragraph.
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (startOffset >= count + inline.Length)
                {
                    count += inline.Length;
                    continue;
                }
                int startIndex = 0;
                if (startOffset > count)
                    startIndex = (int)(startOffset - count);
                int endIndex = (int)(endOffset - count);
                if (endIndex > inline.Length)
                    endIndex = inline.Length;
                if (startIndex > 0)
                    count += startIndex;
                if (startIndex == 0 && endIndex == inline.Length)
                {
                    paragraph.Inlines.Insert(insertIndex, inline);
                    insertIndex++;
                    i--;
                }
                else if (inline is SpanAdv)
                {
                    SpanAdv span = new SpanAdv();
                    span.CharacterFormat.CopyFormat(inline.CharacterFormat);
                    span.Text = (inline as SpanAdv).Text.Substring(startIndex, endIndex - startIndex);
                    paragraph.Inlines.Insert(insertIndex, span);
                    insertIndex++;
                    (inline as SpanAdv).Text = (inline as SpanAdv).Text.Remove(startIndex, endIndex - startIndex);
                }
                if (endOffset <= count + endIndex - startIndex)
                    break;
                count += endIndex - startIndex;
            }
        }
        /// <summary>
        /// Copies the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startOffset">The start offset.</param>
        private void Copy(SelectionAdv selection, double startOffset)
        {
            //Copy as DocIO instance.
            DocIO.DLS.WParagraph wParagraph = new DocIO.DLS.WParagraph(selection.WordDocument);
            //Adds the paragraph to DocIO Word document instance.
            selection.WordDocument.LastSection.Body.ChildEntities.Add(wParagraph);
            DocxExporting.SerializeCharacterFormat(CharacterFormat, wParagraph.BreakCharacterFormat);
            DocxExporting.SerializeParagraphFormat(ParagraphFormat, wParagraph.ParagraphFormat);
            //Copies the inline items between selection start and end to new paragraph.
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (startOffset >= count + inline.Length)
                {
                    count += inline.Length;
                    continue;
                }
                int startIndex = 0;
                if (startOffset > count)
                    startIndex = (int)(startOffset - count);
                int endIndex = inline.Length;
                if (startIndex > 0)
                    count += startIndex;
                if (startIndex == 0 && endIndex == inline.Length)
                    DocxExporting.SerializePargraphItems(inline, wParagraph, true);
                else if (inline is SpanAdv)
                {
                    DocIO.DLS.IWTextRange wTextRange = wParagraph.AppendText((inline as SpanAdv).Text.Substring(startIndex, endIndex - startIndex));
                    DocxExporting.SerializeCharacterFormat(inline.CharacterFormat, wTextRange.CharacterFormat);
                }
                count += endIndex - startIndex;
            }
        }
        /// <summary>
        /// Copies the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private void Copy(SelectionAdv selection)
        {
            //Copy as DocIO instance.
            DocIO.DLS.WParagraph wParagraph = new DocIO.DLS.WParagraph(selection.WordDocument);
            //Adds the paragraph to DocIO Word document instance.
            selection.WordDocument.LastSection.Body.ChildEntities.Add(wParagraph);
            DocxExporting.SerializeCharacterFormat(CharacterFormat, wParagraph.BreakCharacterFormat);
            DocxExporting.SerializeParagraphFormat(ParagraphFormat, wParagraph.ParagraphFormat);
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                //Copy as DocIO instance.
                DocxExporting.SerializePargraphItems(inline, wParagraph, true);
            }
        }
        #endregion

        #region Apply Character Format for Selected Contents
        /// <summary>
        /// Applies the character format for selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyCharacterFormatForSelection(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.ApplyCharacterFormat(selection, start, end, property, value);
            else
                ApplyCharacterFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            double startOffset = 0;
            double length = GetLength();
            if (this == start.Paragraph)
            {
                startOffset = start.Offset;
                if (value == DependencyProperty.UnsetValue
                    && (property == CharacterFormat.BoldProperty || property == CharacterFormat.ItalicProperty))
                {
                    CharacterFormat format = CharacterFormat;
                    if (startOffset < length)
                    {
                        int index = 0;
                        Inline inline = GetInline(startOffset, ref index);
                        format = inline.CharacterFormat;
                        if (index == inline.Length && inline.NextNode != null)
                            format = (inline.NextNode as Inline).CharacterFormat;
                    }
                    value = !(bool)format.GetPropertyValue(property);
                }
            }
            double endOffset = end.Offset;
            if (this != end.Paragraph)
            {
                CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
                endOffset = length;
            }
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (startOffset >= count + inline.Length)
                {
                    count += inline.Length;
                    continue;
                }
                int startIndex = 0;
                if (startOffset > count)
                    startIndex = (int)(startOffset - count);
                int endIndex = (int)(endOffset - count);
                int inlineLength = inline.Length;
                if (endIndex > inlineLength)
                    endIndex = inlineLength;
                inline.ApplyCharacterFormat(selection, startIndex, endIndex, property, value);
                if (endOffset <= count + inlineLength)
                    break;
                count += inlineLength;
                if (startIndex > 0)
                    i++;
            }
            if (end.Paragraph == this)
            {
                if (endOffset == length + 1)
                    CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
                return;
            }
            BlockAdv block = GetNextRenderedBlock();
            if (block != null)
                block.ApplyCharacterFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyCharacterFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inlines[i].CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
            }
            CharacterFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
        }
        #endregion

        #region Apply Paragraph Format for Selected Contents
        /// <summary>
        /// Applies the paragraph format for selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyParagraphFormatForSelection(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.ApplyParagraphFormat(selection, start, end, property, value);
            else
                ApplyParagraphFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end, DependencyProperty property, object value)
        {
            ParagraphFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
            if (end.Paragraph == this)
                return;
            BlockAdv block = GetNextRenderedBlock();
            if (block != null)
                block.ApplyParagraphFormat(selection, start, end, property, value);
        }
        /// <summary>
        /// Applies the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal override void ApplyParagraphFormat(SelectionAdv selection, DependencyProperty property, object value)
        {
            ParagraphFormat.ApplyPropertyValue(selection.CurrentHistoryInfo, property, value);
        }
        #endregion

        #region Get Character Format for Selected Contents
        /// <summary>
        /// Gets the character format for selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void GetCharacterFormatForSelection(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.GetCharacterFormat(selection, start, end);
            else
                GetCharacterFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void GetCharacterFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            double startOffset = 0;
            double length = GetLength();
            if (this == start.Paragraph)
            {
                startOffset = start.Offset;
                if (startOffset < length)
                {
                    int index = 0;
                    Inline inline = GetInline(startOffset, ref index);
                    selection.CharacterFormat.CopyFormat(inline.CharacterFormat);
                    if (selection.IsEmpty)
                        return;
                    else if (index == inline.Length && inline.NextNode != null)
                        selection.CharacterFormat.CopyFormat((inline.NextNode as Inline).CharacterFormat);
                }
            }
            double endOffset = end.Offset;
            if (this != end.Paragraph)
            {
                selection.CharacterFormat.CombineFormat(CharacterFormat);
                endOffset = length;
            }
            double count = 0;
            for (int i = 0; i < Inlines.Count; i++)
            {
                Inline inline = Inlines[i];
                if (startOffset >= count + inline.Length)
                {
                    count += inline.Length;
                    continue;
                }
                selection.CharacterFormat.CombineFormat(inline.CharacterFormat);
                if (endOffset <= count + inline.Length)
                    break;
                count += inline.Length;
            }
            if (end.Paragraph == this)
            {
                if (endOffset == length + 1)
                    selection.CharacterFormat.CombineFormat(CharacterFormat);
                return;
            }
            BlockAdv block = GetNextRenderedBlock();
            if (block != null)
                block.GetCharacterFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal override void GetCharacterFormat(SelectionAdv selection)
        {
            for (int i = 0; i < Inlines.Count; i++)
            {
                selection.CharacterFormat.CombineFormat(Inlines[i].CharacterFormat);
            }
            selection.CharacterFormat.CombineFormat(CharacterFormat);
        }
        #endregion

        #region Get Paragraph Format for the selected items
        /// <summary>
        /// Gets the paragraph format for selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal void GetParagraphFormatForSelection(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            //Selection start in cell.
            if (start.Paragraph.IsInsideTable && (!end.Paragraph.IsInsideTable
                || start.Paragraph.AssociatedCell != end.Paragraph.AssociatedCell
                || start.Paragraph.AssociatedCell.IsCellSelected(start, end)))
                start.Paragraph.AssociatedCell.GetParagraphFormat(selection, start, end);
            else
                GetParagraphFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        internal override void GetParagraphFormat(SelectionAdv selection, TextPosition start, TextPosition end)
        {
            if (start.Paragraph == this)
                selection.ParagraphFormat.CopyFormat(ParagraphFormat);
            else
                selection.ParagraphFormat.CombineFormat(ParagraphFormat);
            if (end.Paragraph == this)
                return;
            BlockAdv block = GetNextRenderedBlock();
            if (block != null)
                block.GetParagraphFormat(selection, start, end);
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal override void GetParagraphFormat(SelectionAdv selection)
        {
            selection.ParagraphFormat.CombineFormat(ParagraphFormat);
        }
        #endregion
    }
}
