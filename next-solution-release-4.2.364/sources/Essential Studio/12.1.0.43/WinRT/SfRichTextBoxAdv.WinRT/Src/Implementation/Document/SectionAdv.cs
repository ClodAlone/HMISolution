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
using System.Collections.Specialized;
#if WPF
using System.Windows.Media;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
#if WPF
    [ContentProperty("Blocks")]
#else
    [ContentProperty(Name = "Blocks")]
#endif
    public class SectionAdv : CompositeNode
    {
        #region Fields
        private List<BodyWidget> bodyWidgets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header footers.
        /// </summary>
        /// <value>
        /// The header footers.
        /// </value>
        public HeaderFooters HeaderFooters
        {
            get
            {
                return (HeaderFooters)GetValue(HeaderFootersProperty);
            }
            set
            {
                SetValue(HeaderFootersProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the section format.
        /// </summary>
        /// <value>
        /// The section format.
        /// </value>
        public SectionFormat SectionFormat
        {
            get
            {
                return (SectionFormat)GetValue(SectionFormatProperty);
            }
            set
            {
                SetValue(SectionFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets the blocks.
        /// </summary>
        /// <value>
        /// The blocks.
        /// </value>
        public BlockAdvCollection Blocks
        {
            get
            {
                return ChildNodes as BlockAdvCollection;
            }
        }
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                return Owner as DocumentAdv;
            }
        }
        /// <summary>
        /// Gets the base parent.
        /// </summary>
        /// <value>
        /// The base parent.
        /// </value>
        internal SfRichTextBoxAdv BaseParent
        {
            get
            {
                if (Document != null)
                    return Document.OwnerControl;
                return null;
            }
        }
        /// <summary>
        /// Gets the body widgets.
        /// </summary>
        /// <value>
        /// The body widgets.
        /// </value>
        internal List<BodyWidget> BodyWidgets
        {
            get
            {
                return bodyWidgets;
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the HeaderFooters dependency property.
        /// </summary>
        /// <returns>The identifier of the HeaderFooters dependency property.</returns>
        internal static readonly DependencyProperty HeaderFootersProperty = DependencyProperty.Register("HeaderFooters", typeof(HeaderFooters), typeof(SectionAdv), new PropertyMetadata(null, OnHeaderFootersChanged));
        /// <summary>
        /// Identifies the SectionFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the SectionFormat dependency property.</returns>
        internal static readonly DependencyProperty SectionFormatProperty = DependencyProperty.Register("SectionFormat", typeof(SectionFormat), typeof(SectionAdv), new PropertyMetadata(null, OnSectionFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when header footers changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnHeaderFootersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is HeaderFooters)
                (e.OldValue as HeaderFooters).SetOwner(null);
            if (e.NewValue is HeaderFooters)
                (e.NewValue as HeaderFooters).SetOwner((SectionAdv)d);
            //Layouts sections again
        }
        /// <summary>
        /// Called when section format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnSectionFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ToDo layouts sections again
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAdv" /> class.
        /// </summary>
        public SectionAdv()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAdv" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal SectionAdv(Node owner)
            : base(owner)
        {
            ChildNodes = new BlockAdvCollection(this);
            SectionFormat = new SectionFormat(this);
            HeaderFooters = new HeaderFooters(this);
            bodyWidgets = new List<BodyWidget>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SectionAdv Clone()
        {
            SectionAdv section = new SectionAdv();
            section.SectionFormat.CopyFormat(SectionFormat);
            HeaderFooters.CloneItemsTo(section.HeaderFooters);
            foreach (BlockAdv block in Blocks)
            {
                section.Blocks.Add(block.Clone());
            }
            return section;
        }
        /// <summary>
        /// Adds the body widget.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        internal BodyWidget AddBodyWidget(Rect area)
        {
            BodyWidget bodyWidget = new BodyWidget(this);
            bodyWidget.Width = area.Width;
            bodyWidget.Location = new Point(area.X, area.Y);
            bodyWidgets.Add(bodyWidget);
            return bodyWidget;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SectionFormat.Dispose();
            ClearValue(SectionFormatProperty);
            HeaderFooters.Dispose();
            ClearValue(HeaderFootersProperty);
            SetOwner(null);
            for (int i = 0; i < Blocks.Count; i++)
            {
                BlockAdv block = Blocks[i];
                block.Dispose();
                Blocks.Remove(block);
                i--;
            }
            for (int i = 0; i < BodyWidgets.Count; i++)
            {
                BodyWidget widget = BodyWidgets[i];
                widget.Dispose();
                BodyWidgets.Remove(widget);
                i--;
            }
        }
        /// <summary>
        /// Gets the current header footer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal HeaderFooter GetCurrentHeaderFooter(HeaderFooterType type)
        {
            if ((HeaderFooters.ChildNodes[(int)type] as HeaderFooter).Blocks.Count == 0
                && PreviousNode != null)
                return (PreviousNode as SectionAdv).GetCurrentHeaderFooter(type);
            return HeaderFooters.ChildNodes[(int)type] as HeaderFooter;
        }
        /// <summary>
        /// Updates the list items.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        internal bool UpdateListItems(BlockAdv block)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                bool isListUpdated = Blocks[i].UpdateListItems(block);
                if (isListUpdated)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Layouts the items.
        /// </summary>
        internal void LayoutItems()
        {
            LayoutViewer viewer = Document.OwnerControl.Viewer;
            //ClearWidgets();
            PageAdv page;
            if (!viewer.IsFieldCode)
            {
                page = viewer.CreateNewPage(this);
                if (viewer.Pages.Count > 1)
                {
                    int pageIndex = 0;
                    foreach (PageAdv prevPage in viewer.Pages)
                    {
                        int prevSectionIndex = prevPage.Section.GetIndexInOwnerCollection();
                        int sectionIndex = GetIndexInOwnerCollection();
                        if (prevSectionIndex > sectionIndex || prevPage == page)
                            break;
                        pageIndex++;
                    }
                    if (pageIndex < viewer.Pages.Count - 1)
                        viewer.InsertPage(pageIndex, page);
                }
            }
            //else
            //{
            //    page = viewer.Pages[viewer.Pages.Count - 1];
            //    //Need to update the client area, based section breaks.
            //    page.BodyWidgets.Add(AddBodyWidget(viewer.ClientActiveArea));
            //}
            for (int i = 0; i < Blocks.Count; i++)
            {
                viewer.UpdateClientArea(Blocks[i], true);
                Blocks[i].LayoutItems(viewer);
                viewer.UpdateClientArea(Blocks[i], false);
            }
        }
        /// <summary>
        /// Layouts the specified block index.
        /// </summary>
        /// <param name="blockIndex">Index of the block.</param>
        internal void Layout(int blockIndex)
        {
            DocumentAdv ownerDocument = Document;
            LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
            if (viewer.FieldEndParagraph != null && viewer.FieldEndParagraph.BaseParent == null && blockIndex < Blocks.Count)
            {
                //If field end mark or its entire owner is removed, sets the current paragraph as relayout end.
                viewer.FieldEndParagraph = Blocks[blockIndex] is ParagraphAdv ? Blocks[blockIndex] as ParagraphAdv
                    : (Blocks[blockIndex] as TableAdv).GetFirstParagraphInFirstCell();
            }
            if (viewer.FieldToLayout != null)
            {
                ParagraphAdv ownerParagraph = viewer.FieldToLayout.OwnerParagraph;
                Inline fieldBegin = viewer.FieldToLayout;
                viewer.FieldToLayout = null;
                if (ownerParagraph != null && ownerParagraph.BaseParent != null)
                {
                    //If field separator or end mark or its entire owner is removed, relayouts from the field begin.
                    ownerParagraph.Relayout(fieldBegin.GetIndexInOwnerCollection());
                    return;
                }
            }
            if (viewer.Pages.Count == 0)
                viewer.CreateNewPage(this);
            //If the content is in text body, updates the client area based on section formattings.
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                viewer.UpdateClientArea(SectionFormat);
#if !WPF
            });
#endif
            if (blockIndex > 0)
            {
                Widget prevWidget;
                if (Blocks[blockIndex - 1] is ParagraphAdv)
                    prevWidget = (Blocks[blockIndex - 1] as ParagraphAdv).ParagraphWidgets[(Blocks[blockIndex - 1] as ParagraphAdv).ParagraphWidgets.Count - 1];
                else
                    prevWidget = (Blocks[blockIndex - 1] as TableAdv).TableWidgets[(Blocks[blockIndex - 1] as TableAdv).TableWidgets.Count - 1];
                viewer.CutFromTop(prevWidget.Location.Y + prevWidget.Height);
            }
            if (blockIndex < 0)
                blockIndex = 0;
            if (blockIndex < Blocks.Count)
            {
                //Updates the client area based on current paragraph.
                BlockAdv block = Blocks[blockIndex];
                //Updates list values of previous rendered paragraphs.
                if (ownerDocument.OwnerControl.IsDocumentLoaded && !ownerDocument.OwnerControl.IsPastingContent)
                    ownerDocument.UpdateListItems(block);
                block.ClearWidgets();
                if (viewer.Pages.Count == 0)
                    viewer.CreateNewPage(this);
                viewer.UpdateClientArea(block, true);
                block.LayoutItems(viewer);
                viewer.UpdateClientArea(block, false);
                block.LayoutNextItems(viewer);
            }
        }
        /// <summary>
        /// Clears the widgets.
        /// </summary>
        internal void ClearWidgets()
        {
            if (HeaderFooters != null)
            {
                for (int i = 0; i < HeaderFooters.ChildNodes.Count; i++)
                {
                    (HeaderFooters.ChildNodes[i] as HeaderFooter).ClearWidgets();
                }
            }
            for (int i = 0; i < BodyWidgets.Count; i++)
            {
                BodyWidget widget = BodyWidgets[i];
                widget.Dispose();
                BodyWidgets.Remove(widget);
                i--;
            }
        }
        /// <summary>
        /// Gets the next selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetNextSelection(SelectionAdv selection)
        {
            if (NextNode is SectionAdv)
            {
                BlockAdv block = (NextNode as SectionAdv).Blocks[0];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                {
                    if (selection.IsEmpty || selection.IsForward)
                        return (block as TableAdv).GetLastParagraphInFirstRow();
                    else
                        return (block as TableAdv).Rows[0].GetNextParagraph(selection);
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the previous selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousSelection(SelectionAdv selection)
        {
            if (PreviousNode is SectionAdv)
            {
                BlockAdv block = (PreviousNode as SectionAdv).Blocks[(PreviousNode as SectionAdv).Blocks.Count - 1];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                {
                    if (!selection.IsForward)
                        return (block as TableAdv).GetFirstParagraphInLastRow();
                    else
                        return (block as TableAdv).Rows[(block as TableAdv).Rows.Count - 1].GetPreviousParagraph(selection);
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the next paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetNextParagraph()
        {
            if (NextNode is SectionAdv)
            {
                BlockAdv block = (NextNode as SectionAdv).Blocks[0];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetFirstParagraphInFirstCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the previous paragraph.
        /// </summary>
        /// <returns></returns>
        internal ParagraphAdv GetPreviousParagraph()
        {
            if (PreviousNode is SectionAdv)
            {
                BlockAdv block = (PreviousNode as SectionAdv).Blocks[(PreviousNode as SectionAdv).Blocks.Count - 1];
                if (block is ParagraphAdv)
                    return block as ParagraphAdv;
                else
                    return (block as TableAdv).GetLastParagraphInLastCell();
            }
            return null;
        }
        /// <summary>
        /// Gets the previous block.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetPreviousBlock()
        {
            if (PreviousNode is SectionAdv)
                return (PreviousNode as SectionAdv).Blocks[(PreviousNode as SectionAdv).Blocks.Count - 1];
            return null;
        }
        /// <summary>
        /// Combines the section.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="nextSection">The next section.</param>
        internal void CombineSection(SelectionAdv selection, SectionAdv nextSection)
        {
            //Removes this section from document.
            Document.Sections.Remove(this);
            int insertIndex = 0;
            for (int i = 0; i < Blocks.Count; i++, insertIndex++)
            {
                BlockAdv block = Blocks[i];
                //Moves the block to next section.
                nextSection.Blocks.Insert(insertIndex, block);
                i--;
            }
            selection.CurrentHistoryInfo.RemovedNodes.Add(this);
        }
        /// <summary>
        /// Splits the section.
        /// </summary>
        /// <param name="newSection">The new section.</param>
        /// <param name="splitBlock">The split block.</param>
        internal void SplitSection(SectionAdv newSection, BlockAdv splitBlock)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                BlockAdv block = Blocks[i];
                if (block == splitBlock)
                    break;
                block.RemoveBlock();
                //Moves the block to next section.
                newSection.Blocks.Add(block);
                i--;
            }
            //Inserts the new section before current section in document.
            int insertIndex = GetIndexInOwnerCollection();
            Document.Sections.Insert(insertIndex, newSection);
        }
        #endregion
    }
}
