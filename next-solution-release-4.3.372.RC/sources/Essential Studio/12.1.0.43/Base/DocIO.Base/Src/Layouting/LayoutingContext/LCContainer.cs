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

#if !SILVERLIGHT

using System;
using System.Drawing;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS.Rendering;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// 
    /// </summary>
    internal class LCContainer : LayoutContext
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        protected int m_curWidgetIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        protected LayoutedWidget m_currChildLW;
        /// <summary>
        /// 
        /// </summary>
        protected bool m_bAtLastOneChildFitted = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the widget container.
        /// </summary>
        /// <value>The widget container.</value>
        protected IWidgetContainer WidgetContainer
        {
            get
            {
                return m_widget as IWidgetContainer;
            }
        }

        /// <summary>
        /// Gets the current child widget.
        /// </summary>
        /// <value>The current child widget.</value>
        protected IWidget CurrentChildWidget
        {
            get
            {
                bool isExist = (m_curWidgetIndex > -1 && m_curWidgetIndex < WidgetContainer.Count);
                return isExist ? WidgetContainer[m_curWidgetIndex] : null;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LCContainer"/> class.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LCContainer(IWidgetContainer widget, ILCOperator lcOperator)
            : base(widget, lcOperator)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Layouts the specified widget.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public override LayoutedWidget Layout(RectangleF rect)
        {
            CreateLayoutArea(rect);
            CreateLayoutedWidget(rect.Location);

            do
            {
                // Creates next child context
                LayoutContext childContext = CreateNextChildContext();

                // If child context NULL - break cycle
                if (childContext == null)
                {
                    if (m_bAtLastOneChildFitted)
                    {
                        m_ltState = LayoutState.Fitted;
                        m_isTabStopBeyondRightMarginExists = false;
                    }
                    break;
                }
                // If child widget is paragraph
                else if (childContext.Widget != null && childContext.Widget is WParagraph)
                {
                    WParagraph paragraph = childContext.Widget as WParagraph;
                    if (paragraph.ParagraphFormat != null)
                    {
                        // If paragraph has right indent, update the right margin
                        childContext.ClientLayoutAreaRight = ClientLayoutAreaRight - paragraph.ParagraphFormat.RightIndent;
                    }
                }
                else
                {
                    childContext.ClientLayoutAreaRight = ClientLayoutAreaRight;
                }
                childContext.m_isTabStopBeyondRightMarginExists = m_isTabStopBeyondRightMarginExists;                
                //Determine whether the layouting context need to be wrap
                childContext.m_bIsNeedToWrap = m_bIsNeedToWrap;
                childContext.LayoutInfo.TextWrap = LayoutInfo.TextWrap;
                DoLayoutChild(childContext);
                m_isTabStopBeyondRightMarginExists = childContext.m_isTabStopBeyondRightMarginExists;
                // "Commit process" for child context in current context
                SaveChildContextState(childContext);
            }
            while (State == LayoutState.Unknown);

            // Executes after layout actions
            DoLayoutAfter();

#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
            return m_ltWidget;
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Creates the next child context.
        /// </summary>
        /// <returns></returns>
        protected virtual LayoutContext CreateNextChildContext()
        {
        RepeatNextWidget:
            IWidget childWidget = CurrentChildWidget;
            //Layout Splitted Footnote textbody
            LayoutFootnoteSplittedWidgets(childWidget);
            LayoutEndnoteSplittedWidgets(childWidget);
            if (childWidget != null)
            {
                if (childWidget.LayoutInfo != null && childWidget.LayoutInfo.IsSkip)
                {
                    if (NextChildWidget())
                    {
                        goto RepeatNextWidget; // Try repeat get child widget
                    }
                    else
                    {
                        m_bAtLastOneChildFitted = true;
                        return null;
                    }
                }
                //Get Frame client area
                if ((childWidget is WTable) && !(childWidget as WTable).TableFormat.WrapTextAround && IsInFrame(childWidget as WTable))
                    GetFrameBounds((childWidget as WTable).Rows[0].Cells[0].Paragraphs[0] as WParagraph, m_layoutArea.ClientActiveArea);
                //Update position of the paragraph
                UpdateParagraphPosition(childWidget);
                if (childWidget is WTextRange)
                {
                    if ((childWidget as WTextRange).OwnerParagraph != null)
                        childWidget.LayoutInfo.IsClipped = ((childWidget as WTextRange).OwnerParagraph as IWidget).LayoutInfo.IsClipped;
                    else
                        childWidget.LayoutInfo.IsClipped = LayoutInfo.IsClipped;
                }
                return LayoutContext.Create(childWidget, m_lcOperator, (float)m_layoutArea.Width);
            }
            m_bAtLastOneChildFitted = true;
            return null;
        }
        /// <summary>
        /// Layout splitted footnote textbody
        /// </summary>
        /// <param name="childWidget"></param>
        private void LayoutFootnoteSplittedWidgets(IWidget childWidget)
        {
            if ((m_lcOperator as Layouter).FootnoteSplittedWidgets.Count > 0
               && (childWidget is SplitWidgetContainer)
               && (childWidget as SplitWidgetContainer).RealWidgetContainer is WSection)
            {
                float height = 0;
                SplitWidgetContainer[] footnoteSplittedWidgets = new SplitWidgetContainer[(m_lcOperator as Layouter).FootnoteSplittedWidgets.Count];
                (m_lcOperator as Layouter).FootnoteSplittedWidgets.CopyTo(footnoteSplittedWidgets);
                (m_lcOperator as Layouter).FootnoteSplittedWidgets.Clear();
                if ((m_lcOperator as Layouter).m_bisNeedToRestartFootnote)
                {
                    WFootnote footnote = (footnoteSplittedWidgets[0].RealWidgetContainer as WTextBody).Owner as WFootnote;
                    LayoutFootnoteTextBody(footnote.Document.Footnotes.ContinuationSeparator, ref height, m_layoutArea.ClientActiveArea.Height);
                    (m_lcOperator as Layouter).m_bisNeedToRestartFootnote = false;
                }
                for (int i = 0; i < footnoteSplittedWidgets.Length; i++)
                {
                    if (i > 0)
                        height = 0;
                    LayoutFootnoteTextBody(footnoteSplittedWidgets[i], ref height, m_layoutArea.ClientActiveArea.Height);
                    CreateLayoutArea(new RectangleF(m_layoutArea.ClientActiveArea.X, m_layoutArea.ClientActiveArea.Y, m_layoutArea.ClientActiveArea.Width, m_layoutArea.ClientActiveArea.Height - height));
                }

                int count = (m_lcOperator as Layouter).FootNoteSectionIndex.Count;
                //Set the corresponding section index for the footnote widgets.
                while ((m_lcOperator as Layouter).FootnoteWidgets.Count > (m_lcOperator as Layouter).FootNoteSectionIndex.Count)
                {
                    (m_lcOperator as Layouter).FootNoteSectionIndex.Add(count + 1);
                }
            }
           
        }
        /// <summary>
        /// Layout splitted endtnote textbody
        /// </summary>
        /// <param name="childWidget"></param>
        private void LayoutEndnoteSplittedWidgets(IWidget childWidget)
        {
            //Check whether the Endnote splitted widgets exist or not
            if ((m_lcOperator as Layouter).EndnoteSplittedWidgets.Count > 0
               && (childWidget is SplitWidgetContainer)
               && (childWidget as SplitWidgetContainer).RealWidgetContainer is WSection)
            {
                float height = 0;
                //Copy EndnoteSplittedWidgets and clear. 
                SplitWidgetContainer[] endnoteSplittedWidgets = new SplitWidgetContainer[(m_lcOperator as Layouter).EndnoteSplittedWidgets.Count];
                (m_lcOperator as Layouter).EndnoteSplittedWidgets.CopyTo(endnoteSplittedWidgets);
                (m_lcOperator as Layouter).EndnoteSplittedWidgets.Clear();
                if ((m_lcOperator as Layouter).m_bisNeedToRestartEndnote)
                {
                    WTextBody footnote = (endnoteSplittedWidgets[0].RealWidgetContainer as WTextBody);
                    LayoutEndnoteTextBody(footnote.Document.Footnotes.ContinuationSeparator, ref height, m_layoutArea.ClientActiveArea.Height);
                    (m_lcOperator as Layouter).m_bisNeedToRestartEndnote = false;
                }
                for (int i = 0; i < endnoteSplittedWidgets.Length; i++)
                {
                    if (i > 0)
                        height = 0;
                    LayoutEndnoteTextBody(endnoteSplittedWidgets[i], ref height, m_layoutArea.ClientActiveArea.Height);
                    CreateLayoutArea(new RectangleF(m_layoutArea.ClientActiveArea.X, m_layoutArea.ClientActiveArea.Y, m_layoutArea.ClientActiveArea.Width, m_layoutArea.ClientActiveArea.Height - height));
                }
                int count = (m_lcOperator as Layouter).EndNoteSectionIndex.Count;
                //Set corresponding section index for the Endnote widgets. 
                while ((m_lcOperator as Layouter).EndnoteWidgets.Count > (m_lcOperator as Layouter).EndNoteSectionIndex.Count)
                {
                    (m_lcOperator as Layouter).EndNoteSectionIndex.Add(count + 1);
                }
            }        

        }
        /// <summary>
        /// Update position of the paragraph
        /// </summary>
        /// <param name="childWidget"></param>
        private void UpdateParagraphPosition(IWidget childWidget)
        {
            if (childWidget is WParagraph)
            {
                //sets a value indicating whether the line is the first line of the paragraph
                (childWidget.LayoutInfo as ParagraphLayoutInfo).IsFirstLine = true;
                //Update Paragraph Top and Bottom Margins
                UpdateParagraphMargins((childWidget as WParagraph));

                if (IsInFrame(childWidget as WParagraph))
                {
                    RectangleF clientArea = GetFrameBounds(childWidget as WParagraph, m_layoutArea.ClientActiveArea);
                    (childWidget.LayoutInfo as ParagraphLayoutInfo).YPosition = clientArea.Y;
                    (childWidget.LayoutInfo as ParagraphLayoutInfo).XPosition = clientArea.X;
                }
                else
                {
                    (childWidget.LayoutInfo as ParagraphLayoutInfo).YPosition = m_layoutArea.ClientActiveArea.Y;                    
                    (childWidget.LayoutInfo as ParagraphLayoutInfo).YPosition -= GetParagraphTopMargin(childWidget as WParagraph);
                    (childWidget.LayoutInfo as ParagraphLayoutInfo).XPosition = m_layoutArea.ClientActiveArea.X;
                }
                UpdateParagraphXPositionBasedOnTextWrap((childWidget as WParagraph), (childWidget.LayoutInfo as ParagraphLayoutInfo).XPosition, (childWidget.LayoutInfo as ParagraphLayoutInfo).YPosition);
            }

            if ((childWidget is SplitWidgetContainer) && (childWidget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
            {
                WParagraph para = (childWidget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
                int count = para.ChildEntities.Count;
                if (count > (childWidget as SplitWidgetContainer).Count && para.ChildEntities[count - 1 - (childWidget as SplitWidgetContainer).Count] is Break)
                {
                    Entity ent = (Entity)para.ChildEntities[count - 1 - (childWidget as SplitWidgetContainer).Count];
                    Break br = ent as Break;
                    if (br.BreakType == BreakType.PageBreak || br.BreakType == BreakType.ColumnBreak)
                    {
                        (para.m_layoutInfo as ParagraphLayoutInfo).YPosition = m_layoutArea.ClientActiveArea.Y;
                        (para.m_layoutInfo as ParagraphLayoutInfo).XPosition = m_layoutArea.ClientActiveArea.X;
                        UpdateParagraphXPositionBasedOnTextWrap(para, (para.m_layoutInfo as ParagraphLayoutInfo).XPosition, (childWidget.LayoutInfo as ParagraphLayoutInfo).YPosition);
                    }
                }
            }
        }
        /// <summary>
        /// Update paragraph Top and Bottom Margins
        /// </summary>
        /// <param name="paragraph"></param>
        private void UpdateParagraphMargins(WParagraph paragraph)
        {
            ParagraphLayoutInfo paraInfo = paragraph.m_layoutInfo as ParagraphLayoutInfo;
            if (DocumentLayouter.IsFirstLayouting)
            {
                paraInfo.TopMargin = paragraph.m_layoutInfo.Margins.Top;
                paraInfo.BottomMargin = paragraph.m_layoutInfo.Margins.Bottom;
            }
            else
            {
                paragraph.m_layoutInfo.Margins.Top = paraInfo.TopMargin;
                paragraph.m_layoutInfo.Margins.Bottom = paraInfo.BottomMargin;
                paragraph.IsFloatingItemsLayouted = false ;
            }
        }
        /// <summary>
        /// Get paragraph top margin
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private float GetParagraphTopMargin(WParagraph paragraph)
        {
            if (!paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing
                && Math.Round((paragraph.m_layoutInfo as ParagraphLayoutInfo).YPosition, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2)
                && paragraph.PreviousSibling != null && (paragraph.PreviousSibling is WParagraph)
                && ((paragraph.ParagraphFormat.BeforeSpacing > (paragraph.PreviousSibling as WParagraph).ParagraphFormat.AfterSpacing && !paragraph.ParagraphFormat.SpaceBeforeAuto)
                || (paragraph.ParagraphFormat.SpaceBeforeAuto && (paragraph.PreviousSibling as WParagraph).ParagraphFormat.AfterSpacing < 14)))
            {
                if (paragraph.ParagraphFormat.SpaceBeforeAuto)
                    return 14;
                else
                    return paragraph.ParagraphFormat.BeforeSpacing;
            }
            else
                return 0;
        }
        /// <summary>
        /// Marks as not fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void MarkAsNotFitted(LayoutContext childContext, bool isFootnote)
        {
            //Reset Footnote layout info when it's not fitted in current page
            if (childContext.Widget is WFootnote)
            {
                childContext.Widget.InitLayoutInfo();
            }
            m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;
            IWidget splittedWidget = null;
            CommitKeepWithNext(ref splittedWidget);
            if (m_ltWidget.ChildWidgets.Count > 0 && m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget is WParagraph)
            {
                m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].IsLastItemInPage = true;
                RectangleF bounds = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Bounds;
                Borders borders = (m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget as WParagraph).ParagraphFormat.Borders;
                if (!borders.NoBorder && borders.Bottom.BorderType != BorderStyle.None && m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget.LayoutInfo.Paddings.Bottom == 0)
                {
                    m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget.LayoutInfo.Paddings.Bottom = borders.Bottom.Space;
                    bounds.Height += borders.Bottom.Space;
                    m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Bounds = bounds;
                }
            }
            //Split the empty section when the entire section has been layouted but still we need to layout the splitted footnote textbody
            WSection section = (m_sptWidget is WSection) ? m_sptWidget as WSection
                : m_sptWidget is SplitWidgetContainer ? (m_sptWidget as SplitWidgetContainer).RealWidgetContainer as WSection : null;
            if (m_bAtLastOneChildFitted || ((m_lcOperator as Layouter).FootnoteWidgets.Count > 1 && section != null
                && ((m_lcOperator as Layouter).ClientLayoutArea.Height - ((m_lcOperator as Layouter).FootnoteWidgets[(m_lcOperator as Layouter).FootnoteWidgets.Count -1].Bounds.Bottom - (m_lcOperator as Layouter).ClientLayoutArea.Y) < CurrentChildWidget.LayoutInfo.Size.Height)))
            {
                if (splittedWidget != null)
                    SplitedUpWidget(splittedWidget,false);
                else
                {
                    //Split the entire paragraph line when the footnote not fitted and splited into next page
                    if (isFootnote && (CurrentChildWidget is WFootnote) && (m_lcOperator as Layouter).FootnoteWidgets.Count == 0)
                    {
                        if (WidgetContainer is SplitWidgetContainer)
                            m_sptWidget = WidgetContainer as SplitWidgetContainer;
                        else
                            m_sptWidget = new SplitWidgetContainer(WidgetContainer, WidgetContainer.WidgetInnerCollection[0] as IWidget, 0);
                        m_ltState = LayoutState.NotFitted;
                        return;
                    }
                    else
                        SplitedUpWidget(CurrentChildWidget,false);
                }
                //Upddate index of split string widget to perform text wrap
                UpdateSplittedWidgetIndex(childContext);
                ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;
                if (paragraphInfo != null)
                    paragraphInfo.IsFirstLine = false;
                m_ltState = LayoutState.Splitted;
            }
            else
            {
                m_ltState = LayoutState.NotFitted;
            }
        }

        /// <summary>
        /// Checks whether page break is present inside the table
        /// </summary>
        /// <param name="LeafWidget"></param>
        /// <remarks>If it is present inside the table, it will not be considered during Doc to PDF conversion.</remarks>
        /// <returns>bool</returns>
        private bool IsPageBreakInTable(IWidget LeafWidget)
        {
            if (LeafWidget is Break)
            {
                Break brk = LeafWidget as Break;
                if (brk.OwnerParagraph != null)
                {
                    IWParagraph para = brk.OwnerParagraph;
                    if (para.IsInCell)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Marks as fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void MarkAsFitted(LayoutContext childContext)
        {
            AddChildLW(childContext);
            //Layout Footnote
            if ((childContext.Widget is WFootnote)
                && (childContext.Widget as WFootnote).FootnoteType == FootnoteType.Footnote)
            {
                IWSection currentSection = GetBaseEntity(childContext.Widget as Entity) as WSection;
                LayoutFootnote(childContext.Widget as WFootnote, m_currChildLW.Owner);
                //Remove layouted footnote widget from the collection when the footnote textbody not fitted in current page and page doesn't contain any other footnotes.
                if ((m_lcOperator as Layouter).FootnoteWidgets.Count == 0)
                {
                    MarkAsNotFitted(childContext, true);
                    //Clear footnote splittedwidgets
                    (m_lcOperator as Layouter).FootnoteSplittedWidgets.Clear();
                    return;
                }
                while ((m_lcOperator as Layouter).FootnoteWidgets.Count > (m_lcOperator as Layouter).FootNoteSectionIndex.Count)
                {
                    (m_lcOperator as Layouter).FootNoteSectionIndex.Add(currentSection.Document.Sections.IndexOf(currentSection));
                }
            }
            else if ((childContext.Widget is WFootnote)
                && (childContext.Widget as WFootnote).FootnoteType == FootnoteType.Endnote)
            {
                (m_lcOperator as Layouter).EndnotesInstances.Add(childContext.Widget as Entity);
            }

            bool isNextChildWidget = NextChildWidget();

            //All the child items are layouted in a table cell then we need to set the splitted widget is Null
            if ((WidgetContainer is SplitWidgetContainer)
                && (WidgetContainer as SplitWidgetContainer).RealWidgetContainer is WTableCell
                && !isNextChildWidget)
            {
                m_sptWidget = null;
            }

            if (childContext.LayoutInfo.IsLineBreak && CurrentChildWidget != null)
            {
                SplitedUpWidget(CurrentChildWidget, false);
                m_ltState = LayoutState.Splitted;
                m_ltWidget.TextTag = "Splitted";
            }
            else if (childContext.LayoutInfo.IsPageBreakItem && !IsPageBreakInTable(childContext.Widget))
            {
                if (CurrentChildWidget != null)
                {
                    SplitedUpWidget(CurrentChildWidget, false);
                }
                else
                {
                    m_sptWidget = new SplitWidgetContainer(WidgetContainer, childContext.Widget, WidgetContainer.Count - 1);
                }
                if (CurrentChildWidget != null && (CurrentChildWidget as Entity).PreviousSibling is WTable)
                    m_ltState = LayoutState.Splitted;
                else
                    m_ltState = LayoutState.Breaked;
            }
            else if (!m_bAtLastOneChildFitted)
            {
                m_bAtLastOneChildFitted = true;
            }
            //Split the empty section when the entire section has been layouted but still we need to layout the splitted footnote textbody
            WSection section = (m_sptWidget is WSection) ? m_sptWidget as WSection 
                : m_sptWidget is SplitWidgetContainer ? (m_sptWidget as SplitWidgetContainer).RealWidgetContainer as WSection : null;
            if (!isNextChildWidget && section != null && m_ltState != LayoutState.Splitted
                && (m_lcOperator as Layouter).FootnoteSplittedWidgets.Count > 0)
            {
                m_sptWidget = new SplitWidgetContainer(WidgetContainer);
                m_ltState = LayoutState.Splitted;
            }
        }
        /// <summary>
        /// Marks as Wrap Text.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void MarkAsWrapText(LayoutContext childContext)
        {
            AddChildLW(childContext);
            //Add split string widget into widget collection based on current widget index.
            int startIndex = m_curWidgetIndex;
            if (WidgetContainer is SplitWidgetContainer)
                startIndex = WidgetContainer.WidgetInnerCollection.InnerList.IndexOf(childContext.Widget);
            if (childContext.Widget is SplitStringWidget)
            {
                if (startIndex == -1)
                {
                    startIndex = (childContext.Widget as SplitStringWidget).m_prevWidgetIndex;
                }
                else
                    (childContext.SplittedWidget as SplitStringWidget).m_prevWidgetIndex = startIndex;
            }
            WidgetContainer.WidgetInnerCollection.InnerList.Insert(startIndex + 1, childContext.SplittedWidget);
            NextChildWidget();
        }
        /// <summary>
        /// Marks as splitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void MarkAsSplitted(LayoutContext childContext)
        {
            m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;
            ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;

            if (paragraphInfo != null)
                paragraphInfo.IsFirstLine = false;

            AddChildLW(childContext);
            //Upddate index of split string widget to perform text wrap
            UpdateSplittedWidgetIndex(childContext);
            SplitedUpWidget(childContext.SplittedWidget,false);
            m_ltState = LayoutState.Splitted;
        }

        /// <summary>
        /// Upddate index of split string widget to perform text wrap
        /// </summary>
        private void UpdateSplittedWidgetIndex(LayoutContext childContext)
        {
            int startIndex = m_curWidgetIndex;
            if (WidgetContainer is SplitWidgetContainer)
                startIndex = WidgetContainer.WidgetInnerCollection.InnerList.IndexOf(childContext.Widget);
            if (childContext.SplittedWidget is SplitStringWidget)
            {
                if (startIndex != -1)
                    (childContext.SplittedWidget as SplitStringWidget).m_prevWidgetIndex = startIndex;
                else if(childContext.Widget is SplitStringWidget)
                    (childContext.SplittedWidget as SplitStringWidget).m_prevWidgetIndex = (childContext.Widget as SplitStringWidget).m_prevWidgetIndex + 1;
            }
        }
        /// <summary>
        /// Marks as breaked.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void MarkAsBreaked(LayoutContext childContext)
        {

            AddChildLW(childContext);
            // update current widget index      
            LayoutedWidget child = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];

            if (child != null)
            {
                LayoutedWidget chld = child.ChildWidgets[child.ChildWidgets.Count - 1];
                //Updates the index of page break item.
                if (chld != null
                    && chld.Widget.LayoutInfo.IsPageBreakItem)
                {
                    for (int i = 0; i < WidgetContainer.Count; i++)
                    {
                        if (WidgetContainer[i] == chld.Widget)
                        {
                            m_curWidgetIndex = i;
                            break;
                        }
                    }
                }
            }

            NextChildWidget();

            if (CurrentChildWidget != null)
            {
                SplitedUpWidget(CurrentChildWidget,false);
                m_ltState = LayoutState.Splitted;
                //Upddate index of split string widget to perform text wrap
                UpdateSplittedWidgetIndex(childContext);
            }
            else
            {
                m_ltState = LayoutState.Breaked;
            }

        }

        /// <summary>
        /// Updates the client area.
        /// </summary>
        protected virtual void UpdateClientArea()
        {
            Spacings margins = m_currChildLW.Widget.LayoutInfo.Margins;
            Spacings paddings = m_currChildLW.Widget.LayoutInfo.Paddings;
            RectangleF bounds = m_currChildLW.Bounds;
            if (m_currChildLW.Widget is WPicture || m_currChildLW.Widget is Shape)
            {
                TextWrappingStyle textWrapStyle = m_currChildLW.Widget is Shape ? (m_currChildLW.Widget as Shape).WrapFormat.TextWrappingStyle : (m_currChildLW.Widget as WPicture).TextWrappingStyle;
                if (textWrapStyle != TextWrappingStyle.Inline)
                    return;
            }
            if (m_currChildLW.Widget is WParagraph)
            {
                if (IsInFrame(m_currChildLW.Widget as WParagraph))
                {
                    ushort heightValue = (ushort)(m_currChildLW.Widget as WParagraph).ParagraphFormat.FrameHeight;
                    bool isAtleastHeight = (heightValue & (1 << 15)) != 0;
                    if (isAtleastHeight)
                    {
                        bounds.Height += (float)(paddings.Bottom);
                    }
                    UpdateFrameBounds(bounds, isAtleastHeight);
                    return;
                }
                else
                {
                    bounds.Height += (float)(paddings.Bottom);
                    m_currChildLW.Bounds = bounds;
                }
            }
            if (m_currChildLW.Widget is WTable)
            {
                WTable table = m_currChildLW.Widget as WTable;
                if (table.m_isTextBox)
                {
                    WTextBoxFormat textboxFormat = table.m_textBoxFormat;
                    if (textboxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                        return;
                }
                else if ((m_currChildLW.Widget as WTable).TableFormat.WrapTextAround)
                    return;
                else if (IsInFrame(table))
                {
                    UpdateFrameBounds(bounds, false);
                    return;
                }
            }
            bounds.X -= (float)margins.Left;
            bounds.Y -= (float)margins.Top;
            bounds.Width += (float)(margins.Left + margins.Right);
            bounds.Height += (float)(margins.Top + margins.Bottom);

            switch (LayoutInfo.ChildrenLayoutDirection)
            {
                case ChildrenLayoutDirection.Horizontal:
                    m_layoutArea.CutFromLeft(bounds.Right);
                    break;
                case ChildrenLayoutDirection.Vertical:
                    float footnoteHeight = 0;
                    m_currChildLW.GetFootnoteHeight(ref footnoteHeight);
                    m_layoutArea.CutFromTop(bounds.Bottom, footnoteHeight);
                    break;
            }
        }
        /// <summary>
        /// Determines whether the specified table is in frame.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// 	<c>true</c> if the specified table is in frame; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInFrame(WTable table)
        {
            bool isOwnerInFrame = true;
            if (table.OwnerTextBody is WTableCell)
                isOwnerInFrame = false;
            if (table.IsFrame
               && isOwnerInFrame)
                return true;
            return false;
        }
        /// <summary>
        /// Updates the frame bounds.
        /// </summary>
        private void UpdateFrameBounds(RectangleF bounds, bool isAtleastHeight)
        {
            Spacings margins = m_currChildLW.Widget.LayoutInfo.Margins;
            m_currChildLW.Bounds = bounds;
            bounds.X -= (float)margins.Left;
            bounds.Y -= (float)margins.Top;
            bounds.Width += (float)(margins.Left + margins.Right);
            bounds.Height += (float)(margins.Top + margins.Bottom);
            float y = bounds.Bottom;

            if (y < (m_lcOperator as Layouter).FrameLayoutArea.Top)
                y = (m_lcOperator as Layouter).FrameLayoutArea.Top;
            else if (y > (m_lcOperator as Layouter).FrameLayoutArea.Bottom)
                y = (m_lcOperator as Layouter).FrameLayoutArea.Bottom;

            RectangleF rect = (m_lcOperator as Layouter).FrameLayoutArea;
            if (isAtleastHeight)
                (m_lcOperator as Layouter).FrameHeight -= bounds.Height;
            rect.Height = (float)(rect.Bottom - y);
            rect.Y = (float)y;
            (m_lcOperator as Layouter).FrameLayoutArea = rect;
        }

        /// <summary>
        /// Changes the childs alignment.
        /// </summary>
        protected virtual double ChangeChildsAlignment()
        { return float.MinValue; }
        #endregion

        #region Implementation
        /// <summary>
        /// Nexts the child widget.
        /// </summary>
        /// <returns></returns>
        protected bool NextChildWidget()
        {
            if (m_curWidgetIndex > -1 &&
              m_curWidgetIndex < WidgetContainer.Count - 1)
            {
                m_curWidgetIndex++;
                return true;
            }
            m_curWidgetIndex = -1;
            return false;
        }

        /// <summary>
        /// Spliteds up widget.
        /// </summary>
        /// <param name="splitWidget">The split widget.</param>
        internal void SplitedUpWidget(IWidget splitWidget,bool isEndNoteSplitWidgets)
        {
            int curWidgetIndex = m_curWidgetIndex;
            if (isEndNoteSplitWidgets && m_curWidgetIndex < 0)
                curWidgetIndex = 0;
            // Creates "splitted widget container"
            m_sptWidget = new SplitWidgetContainer
              (
              WidgetContainer,
              splitWidget,
              curWidgetIndex
              );
        }

        /// <summary>
        /// Commits the child context.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected void SaveChildContextState(LayoutContext childContext)
        {
            //PreviousTabCorrection();

            switch (childContext.State)
            {
                case LayoutState.Unknown:
                    m_ltState = LayoutState.Unknown;
                    break;
                //case LayoutState.Fitting:
                case LayoutState.Fitted:
                    MarkAsFitted(childContext);
                    break;
                case LayoutState.NotFitted:
                    m_ltWidget.IsNotFitted = false;
                    MarkAsNotFitted(childContext, false);
                    m_ltWidget.TextTag = "Splitted";
                    break;
                case LayoutState.Splitted:
                    m_ltWidget.TextTag = "Splitted";
                    MarkAsSplitted(childContext);
                    break;
                case LayoutState.Breaked:
                    MarkAsBreaked(childContext);
                    break;
                case LayoutState.WrapText:
                    MarkAsWrapText(childContext);
                    break;
            }
        }

        /// <summary>
        /// Layouted current child
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected virtual void DoLayoutChild(LayoutContext childContext)
        {
            // If tab stop position is beyond the right margin, update width of the line with a maximum value of 1584
            if (m_isTabStopBeyondRightMarginExists && !m_isAreaUpdated)
            {
                UpdateAreaWidth();
                m_isAreaUpdated = true;
            }
            RectangleF clientArea = m_layoutArea.ClientActiveArea;
            if ((childContext.Widget is WParagraph
                && IsInFrame(childContext.Widget as WParagraph))
                || ((childContext.Widget is WTable) 
                && !(childContext.Widget as WTable).TableFormat.WrapTextAround 
                && IsInFrame(childContext.Widget as WTable)))
                clientArea = (m_lcOperator as Layouter).FrameLayoutArea;
            m_currChildLW = childContext.Layout(clientArea);
            //calculate the wrapping difference which is equal to difference between text body starting position and first item of the text body.
            if ((m_lcOperator as Layouter).WrappingDifference == float.MinValue
                && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter
                && m_currChildLW != null
                && m_currChildLW.ChildWidgets.Count > 0)
                (m_lcOperator as Layouter).WrappingDifference = m_currChildLW.ChildWidgets[0].Bounds.Y - (m_lcOperator as Layouter).ClientLayoutArea.Y;
           
        }

        /// <summary>
        /// Adds the child LW.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected void AddChildLW(LayoutContext childContext)
        {
            //if (childContext.Widget is WTextRange)
            //    System.Diagnostics.Debugger.Break();
            //Skip Fields - TODO : Implement later.
            //if (!(childContext.Widget is WField))
            //{
            m_ltWidget.ChildWidgets.Add(m_currChildLW);
			//Set Owner Widget
            m_currChildLW.Owner = m_ltWidget;
            UpdateClientArea();
            UpdateLWBounds(childContext);
            //}
            //check m_currchild is the end of section or end of last section 
            if ((m_ltWidget.Widget is WordDocument || ((m_ltWidget.Widget is SplitWidgetContainer) && (m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WordDocument))
                && (m_currChildLW.Widget is WSection || (m_currChildLW.Widget is SplitWidgetContainer && (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer is WSection))
                && childContext.State != LayoutState.Splitted)
            {
                //Get the current section.
                IWSection currentSection = m_currChildLW.Widget is WSection ? m_currChildLW.Widget as WSection : (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer as WSection;
                if ((m_lcOperator as Layouter).EndnotesInstances.Count > 0)
                {
                    //check the endnote position is end of document and the current section is last section means layout the Endnote instance. 
                    if (currentSection.Document.EndnotePosition == EndnotePosition.DisplayEndOfDocument && currentSection.Document.Sections.IndexOf(currentSection) == currentSection.Document.Sections.Count - 1)
                    {
                        if ((m_lcOperator as Layouter).EndnotesInstances.Count > 0)
                        {
                            for (int i = 0; i < (m_lcOperator as Layouter).EndnotesInstances.Count; i++)
                            {
                                if (!((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote).IsLayouted)
                                    LayoutEndnote((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote, m_currChildLW.Owner);
                                ((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote).IsLayouted = true;
                            }
                            //Clear the Endnote instance once after layout the Endnote instance. 
                            (m_lcOperator as Layouter).EndnotesInstances.Clear();
                        }

                        //Remove layouted footnote widget from the collection when the footnote textbody not fitted in current page and page doesn't contain any other footnotes.
                        if ((m_lcOperator as Layouter).EndnoteWidgets.Count == 0)
                        {
                            MarkAsNotFitted(childContext, true);
                            //Clear footnote splittedwidgets
                            (m_lcOperator as Layouter).EndnoteWidgets.Clear();
                            (m_lcOperator as Layouter).EndNoteSectionIndex.Clear();
                            return;
                        }
                    }//check the Endnote position is End of section means layout the Endnote instances. 
                    else if (currentSection.Document.EndnotePosition == EndnotePosition.DisplayEndOfSection)
                    {
                        if ((m_lcOperator as Layouter).EndnotesInstances.Count > 0)
                        {
                            for (int i = 0; i < (m_lcOperator as Layouter).EndnotesInstances.Count; i++)
                            {
                                if (!((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote).IsLayouted)
                                    LayoutEndnote((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote, m_currChildLW.Owner);
                                ((m_lcOperator as Layouter).EndnotesInstances[i] as WFootnote).IsLayouted = true;
                            }
                            //clear the Endnote instance. 
                            (m_lcOperator as Layouter).EndnotesInstances.Clear();
                        }

                        //Remove layouted endnote widget from the collection when the footnote textbody not fitted in current page and page doesn't contain any other footnotes.
                        if ((m_lcOperator as Layouter).EndnoteWidgets.Count == 0)
                        {
                            MarkAsNotFitted(childContext, true);
                            //Clear footnote splittedwidgets
                            (m_lcOperator as Layouter).EndnoteSplittedWidgets.Clear();
                            return;
                        }
                    }
                    //Set the section index for the corresponding Endnote. 
                    while ((m_lcOperator as Layouter).EndnoteWidgets.Count > (m_lcOperator as Layouter).EndNoteSectionIndex.Count)
                    {
                        (m_lcOperator as Layouter).EndNoteSectionIndex.Add(currentSection.Document.Sections.IndexOf(currentSection));
                    }
                }
                //if the split widget container have the Endnote item  set the empty section in split widget container and change the state to the splitted for layout the Endnote textbody at the second page.
                if ((m_lcOperator as Layouter).EndnoteSplittedWidgets.Count > 0)
                {
                    WidgetContainer widgetContainer = m_currChildLW.Widget is SplitWidgetContainer ? (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer as WidgetContainer : m_currChildLW.Widget as WidgetContainer;
                    m_sptWidget = new SplitWidgetContainer(widgetContainer);
                    SplitedUpWidget(m_sptWidget, true);
                    m_ltState = LayoutState.Splitted;
                }
            }

        }

        /// <summary>
        /// Updates the LW bounds.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        private void UpdateLWBounds(LayoutContext childContext)
        {
            float linespace = 0.0f;
            RectangleF bounds = m_ltWidget.Bounds;
            bool isExactlyLineSpace = false;
            double rightPad = (m_bSkipAreaSpacing) ?
              0f : childContext.BoundsPaddingRight;
            double bottomPad = (m_bSkipAreaSpacing) ?
              0f : childContext.BoundsPaddingBottom;
            bool isFrameBoundsNeedToBeUpdated = false;
            //double bottomPad = childContext.BoundsPaddingBottom;
            
            double linesapce =ChangeChildsAlignment();
            //Get the Line Space
            if ((m_ltWidget.Widget is WParagraph)
                || ((m_ltWidget.Widget is SplitWidgetContainer)
                && (m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph))
            {
                linespace = GetLineSpacing(m_ltWidget, ref isExactlyLineSpace);
                if (linesapce != float.MinValue)
                    linespace = (float)linesapce;
            }
            float linespaceHeight = linespace;
            if (linespace > bounds.Height && bounds.Height != 0)
            {
                linespaceHeight -= bounds.Height;
            }
            //Update Frame Bounds
            if ((m_ltWidget.Widget is WParagraph) && !(childContext.Widget is ParagraphItem) && IsInFrame(m_ltWidget.Widget as WParagraph))
            {
                LayoutedWidget lastWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];
                WParagraph para = m_ltWidget.Widget as WParagraph;
                IWidget widget = lastWidget.Widget;
                if (lastWidget.ChildWidgets.Count > 0)
                {
                    widget = lastWidget.ChildWidgets[lastWidget.ChildWidgets.Count - 1].Widget;
                }
                if (widget is SplitStringWidget
                    && (widget as SplitStringWidget).SplittedText != null
                    && ((widget as SplitStringWidget).SplittedText == string.Empty
                    ||(widget as SplitStringWidget).SplittedText[(widget as SplitStringWidget).SplittedText.Length - 1] == (widget as SplitStringWidget).RealStringWidget.Text[(widget as SplitStringWidget).RealStringWidget.Text.Length - 1]))
                {
                    widget = (widget as SplitStringWidget).RealStringWidget;
                }
                if ((widget is ParagraphItem))
                {
                    int paraItemsCount = para.ChildEntities.Count;
                    if (para.ChildEntities.IndexOf(widget as Entity) == paraItemsCount - 1)
                    {
                        isFrameBoundsNeedToBeUpdated = true;
                        UpdateFrameBounds(para);
                    }
                    else
                    {
                        for (int i = para.ChildEntities.IndexOf(widget as Entity) + 1; i < para.ChildEntities.Count; i++)
                        {
                            if (!(para.ChildEntities[i] as IWidget).LayoutInfo.IsSkip)
                                break;
                            --paraItemsCount;
                        }
                        if (para.ChildEntities.IndexOf(widget as Entity) == paraItemsCount - 1)
                        {
                            UpdateFrameBounds(para);
                            isFrameBoundsNeedToBeUpdated = true;
                        }
                    }
                }
            }
            RectangleF childBounds = m_currChildLW.Bounds;
            double right = Math.Max(childBounds.Right + rightPad, bounds.Right);
            double bottom = 0.0;
            WTextRange textRange = GetTextRange(childContext.Widget);
            if (IsBottomPositionNeedToBeUpdate(childContext))
            {
                if ((bounds.Bottom > childBounds.Bottom && bounds.X < childBounds.X))
                {
                    bottom = Math.Max(childBounds.Bottom + bottomPad, bounds.Bottom);
                }
                else
                {
                    if (isExactlyLineSpace && (bounds.Height == 0 || Math.Round(bounds.Height, 2) == Math.Round(linespace, 2))
                        && (Math.Round(bounds.Y, 2) == Math.Round(childBounds.Y, 2)
                        || Math.Round(bounds.Y + LayoutInfo.Paddings.Top, 2) == Math.Round(childBounds.Y, 2)))
                    {
                        if (Math.Round(bounds.Height, 2) == Math.Round(linespace, 2))
                            bottom = bounds.Bottom;
                        else
                            bottom = bounds.Bottom + linespace;
                    }
                    else if (linespace > bounds.Height)
                        bottom = Math.Max(childBounds.Bottom + bottomPad, bounds.Bottom + linespaceHeight);
                    else if (isExactlyLineSpace && m_currChildLW.Widget is SplitWidgetContainer && !isFrameBoundsNeedToBeUpdated
                        && (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                        bottom = childBounds.Y + linespace;
                    else
                        bottom = Math.Max(childBounds.Bottom + bottomPad, bounds.Bottom);
                }
            }
            bool isInlinePicture = ((childContext.Widget is WPicture) && (childContext.Widget as WPicture).TextWrappingStyle == TextWrappingStyle.Inline)
                                   || ((childContext.Widget is Shape) && (childContext.Widget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Inline);
            //Get Owner paragraph of the text range
            WParagraph ownerParagraph = textRange != null ? textRange.OwnerParagraph : isInlinePicture ? (childContext.Widget as ParagraphItem).GetOwnerParagraph() : null;
            if (ownerParagraph == null && textRange !=null)
            {
                if (textRange.Owner is SDTInlineContent)
                    ownerParagraph = textRange.GetOwnerParagraph();
                else if(textRange.OwnerParagraph == null)
                    ownerParagraph = textRange.CharacterFormat.BaseFormat.OwnerBase as WParagraph;
            }
            if ((textRange != null
                || isInlinePicture)
                && childBounds.Height != 0
                && (linespace > childBounds.Height
                || isExactlyLineSpace)
                && !childContext.Widget.LayoutInfo.IsLineBreak
                && ownerParagraph != null
                && (ownerParagraph.ParagraphFormat.LineSpacingRule != LineSpacingRule.Multiple
                || (linespace < 12 && Math.Abs(ownerParagraph.ParagraphFormat.LineSpacing) < 12 && !isInlinePicture)))
            {
                float topMargin = (float)ownerParagraph.m_layoutInfo.Margins.Top;

                if (!(LayoutInfo.IsClipped && ((ownerParagraph.IsInCell && (ownerParagraph.OwnerTextBody as WTableCell).OwnerRow.Height < linespace))
                    || ((ownerParagraph.OwnerTextBody.Owner is Shape
                    && ((ownerParagraph.OwnerTextBody.Owner as Shape).m_layoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Height < linespace))))
                {
                    if (linespace < childBounds.Height && isExactlyLineSpace)
                        m_currChildLW.Bounds = new RectangleF(childBounds.X, (float)bottom + topMargin - childBounds.Height, childBounds.Width, childBounds.Height);
                    else if (!isInlinePicture)
                        m_currChildLW.Bounds = new RectangleF(childBounds.X, (float)bottom - childBounds.Height, childBounds.Width, childBounds.Height);
                }
            }
            SizeF size = new SizeF((float)(right - bounds.Left), (float)(bottom - bounds.Top));
            if (isFrameBoundsNeedToBeUpdated)
                size.Width = childBounds.Width;
            if (childContext.Widget is WParagraph)
            {
                WParagraph para = childContext.Widget as WParagraph;
                WParagraphStyle pStyle = para.GetStyle() as WParagraphStyle;
                if (!para.ParagraphFormat.Borders.NoBorder || (pStyle != null && pStyle.ParagraphFormat != null && !pStyle.ParagraphFormat.Borders.NoBorder) ||
                    (pStyle != null && pStyle.BaseStyle != null && pStyle.BaseStyle.ParagraphFormat != null && !(pStyle.BaseStyle.ParagraphFormat.Borders.NoBorder)))
                {
                    size.Width = m_layoutArea.ClientActiveArea.Width;
                }
            }

            if (childContext.Widget is WPicture || childContext.Widget is Shape)
            {
                WPicture pic = childContext.Widget as WPicture;
                Shape shape = childContext.Widget as Shape;
                TextWrappingStyle textWrapStyle = shape != null ? shape.WrapFormat.TextWrappingStyle : pic.TextWrappingStyle;
                bool isWrappingBoundsAdded = shape != null ? shape.WrapFormat.IsWrappingBoundsAdded : pic.IsWrappingBoundsAdded;
                ownerParagraph = (childContext.Widget as ParagraphItem).GetOwnerParagraph();
                if (textWrapStyle == TextWrappingStyle.InFrontOfText || textWrapStyle == TextWrappingStyle.Behind)
                {
                    if (ownerParagraph != null && ownerParagraph.ChildEntities.Count == 1 && m_ltWidget.Bounds.Height == 0f)
                    {
                        float paraLineSpacing = Math.Abs(ownerParagraph.ParagraphFormat.LineSpacing);
                        SizeF emptyElementSize = (ownerParagraph as IWidget).LayoutInfo.Size;
                        if (ownerParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly)
                            size.Height = paraLineSpacing;
                        else if (ownerParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.AtLeast)
                        {
                            if (emptyElementSize.Height > paraLineSpacing)
                                size.Height = emptyElementSize.Height;
                            else
                                size.Height = paraLineSpacing;
                        }
                        else
                            size.Height = emptyElementSize.Height * (paraLineSpacing / 12.0f);
                    }
                    else
                    {
                        size.Height = m_ltWidget.Bounds.Height;
                    }

                }
                //Add bounds values of picture
                if (textWrapStyle != TextWrappingStyle.Inline && !isWrappingBoundsAdded)
                {
                    FloatingItem floatingItem = new FloatingItem();
                    floatingItem.TextWrappingBounds = m_currChildLW.Bounds;
                    floatingItem.FloatingEntity = m_currChildLW.Widget as Entity;
                    (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                }

                //Updates height
                if (textWrapStyle != TextWrappingStyle.Inline
                    && textWrapStyle != TextWrappingStyle.InFrontOfText
                    && textWrapStyle != TextWrappingStyle.Behind
                    && bottom == childBounds.Bottom
                    && (m_ltWidget.Bounds.Height == 0
                    || size.Height != m_ltWidget.Bounds.Height))
                {
                    SizeF emptyElementSize = (ownerParagraph as IWidget).LayoutInfo.Size;
                    size.Height = emptyElementSize.Height;
                }
            }
            if (childContext.Widget is WTable)
            {
                WTable table = childContext.Widget as WTable;
                if (table.m_isTextBox)
                {
                    if (table.m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                    {
                        if (!table.m_textBoxFormat.IsWrappingBoundsAdded)//Ignore the floating item bounds if already this added to the Collection.
                        {
                            FloatingItem floatingItem = new FloatingItem();
                            floatingItem.TextWrappingBounds = new RectangleF(m_currChildLW.ChildWidgets[0].Bounds.X, m_currChildLW.ChildWidgets[0].Bounds.Y, m_currChildLW.Bounds.Width, m_currChildLW.Bounds.Height);
                            floatingItem.FloatingEntity = m_currChildLW.Widget as Entity;
                            (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                        }
                        if (m_ltWidget.Widget is WParagraph)
                        {
                            WParagraph para = m_ltWidget.Widget as WParagraph;
                            if (para != null && m_ltWidget.Bounds.Height == 0f && para.Text == string.Empty)
                            {
                                SizeF emptyElementSize = (para as IWidget).LayoutInfo.Size;
                                float paraLineSpacing = Math.Abs(para.ParagraphFormat.LineSpacing);
                                if (para.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly)
                                    size.Height = paraLineSpacing;
                                else if (para.ParagraphFormat.LineSpacingRule == LineSpacingRule.AtLeast)
                                {
                                    if (emptyElementSize.Height > paraLineSpacing)
                                        size.Height = emptyElementSize.Height;
                                    else
                                        size.Height = paraLineSpacing;
                                }
                                else
                                    size.Height = emptyElementSize.Height * (paraLineSpacing / 12.0f);
                            }
                            else
                                size.Height = m_ltWidget.Bounds.Height;
                        }
                        else
                            size.Height = m_ltWidget.Bounds.Height;
                    }
                }
                else if ((childContext.Widget as WTable).TableFormat.WrapTextAround)
                {
                    RowFormat.TablePositioning tablePosition = (childContext.Widget as WTable).TableFormat.Positioning;
                    FloatingItem floatingItem = new FloatingItem();
                    //Add bounds values to list
                    if (m_currChildLW.ChildWidgets.Count > 0)
                    {   
                        //Get the width of the absoulte table along width the distance from left and distancefromleft value.
                        float width = m_currChildLW.Bounds.Width + tablePosition.DistanceFromLeft + tablePosition.DistanceFromRight;
                        //reduce the table border line width from the width of the absolute table area calculation when the table borderstyle is none. 
                        if ((childContext.Widget as WTable).TableFormat.Borders.Right.BorderType == BorderStyle.None)
                            width = (float)Math.Round(width) - (childContext.Widget as WTable).TableFormat.Borders.Right.LineWidth;
                        floatingItem.TextWrappingBounds = new RectangleF(m_currChildLW.ChildWidgets[0].Bounds.X - tablePosition.DistanceFromLeft, m_currChildLW.ChildWidgets[0].Bounds.Y - tablePosition.DistanceFromTop, width, m_currChildLW.Bounds.Height + tablePosition.DistanceFromTop + tablePosition.DistanceFromBottom);
                    }
                    else
                        floatingItem.TextWrappingBounds = m_currChildLW.Bounds;
                    floatingItem.FloatingEntity = m_currChildLW.Widget as Entity;
                    (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                }
                else if (IsInFrame(table))
                    UpdateFrameBounds(table);
            }
            if (size.Height < 0)
                size.Height = 0;
            if (size.Width < 0)
                size.Width = 0;
            float widgetBottom = (float)(bounds.Y + size.Height);
            if (IsUpdateCellLWBounds(widgetBottom))
                m_ltWidget.Bounds = new RectangleF(bounds.Location, new SizeF(size.Width, size.Height + 0.5f));
            else
                m_ltWidget.Bounds = new RectangleF(bounds.Location, size);
            if (isFrameBoundsNeedToBeUpdated)
                AddFrameBounds();
        }
        /// <summary>
        /// Determine whether the layouted widget bottom position is need to be updated or not
        /// </summary>
        /// <returns></returns>
        private bool IsBottomPositionNeedToBeUpdate(LayoutContext childContext)
        {
            return (!(((m_ltWidget.Widget is WSection) || (m_ltWidget.Widget is HeaderFooter)
                || ((m_ltWidget.Widget is SplitWidgetContainer) && (m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WSection))
                && (((childContext.Widget is WParagraph) && IsInFrame(childContext.Widget as WParagraph))
                || ((childContext.Widget is WTable) && ((childContext.Widget as WTable).TableFormat.WrapTextAround
                || IsInFrame(childContext.Widget as WTable))))));
        }
        /// <summary>
        /// Add Frame Bounds into the text box bounds collection
        /// </summary>
        private void AddFrameBounds()
        {
            RectangleF bounds = m_ltWidget.Bounds;
            WParagraph paragraph = m_ltWidget.Widget as WParagraph;
            bool updateFramePosition = true;
            if (paragraph.IsInCell
                && (paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.IsFrame)
                updateFramePosition = false;
            //indicate whether the frame has single paragraph or not.
            bool isFrameHasSingleParagraph = false;
            if (paragraph != null
               && paragraph.ParagraphFormat.IsFrame
               && updateFramePosition)
            {
                float minWidthFromFrameToPageMargin = 72;
                if (bounds.Bottom < (m_lcOperator as Layouter).FrameLayoutArea.Y)
                    bounds.Height = bounds.Height + ((m_lcOperator as Layouter).FrameLayoutArea.Y - bounds.Bottom);
                if (paragraph.ParagraphFormat.FrameWidth != 0)
                    bounds.Width = paragraph.ParagraphFormat.FrameWidth;
                else
                {
                    //Check if the frame has single paragraph means use corresponding paragraph bounds to frame bounds calculation otherwise use margin bounds to frame bounds calculations.   
                    if (!paragraph.ParagraphFormat.IsNextParagraphInSameFrame() || !paragraph.ParagraphFormat.IsPreviousParagraphInSameFrame())
                    {
                        // Update Vertical Distance from text into the Text wrapping frame bounds
                        bounds.Height += (2 * paragraph.ParagraphFormat.FrameVerticalDistanceFromText);
                        bounds.Y -= paragraph.ParagraphFormat.FrameVerticalDistanceFromText;
                        isFrameHasSingleParagraph = true;
                    }
                    else
                    {
                        bounds.Width = (m_lcOperator as Layouter).FrameLayoutArea.Width;//Update the frame width when it have frame width is 0.
                        // Update Vertical Distance from text into the Text wrapping frame bounds along with margins values.
                        bounds.Height += ((2 * paragraph.ParagraphFormat.FrameVerticalDistanceFromText) + (float)(LayoutInfo.Margins.Top + LayoutInfo.Margins.Bottom));
                        bounds.Y -= (paragraph.ParagraphFormat.FrameVerticalDistanceFromText + (float)LayoutInfo.Margins.Top);
                    }
                }
                FloatingItem floatingItem = new FloatingItem();
                //Update TextWrapping Bounds based on minimum width from frame to page margin
                if (bounds.X > (m_lcOperator as Layouter).ClientLayoutArea.Left
                   && bounds.X - minWidthFromFrameToPageMargin < (m_lcOperator as Layouter).ClientLayoutArea.Left)
                {
                    float diff = bounds.X - (m_lcOperator as Layouter).ClientLayoutArea.Left;
                    bounds.X = (m_lcOperator as Layouter).ClientLayoutArea.Left;
                    bounds.Width += diff;
                    floatingItem.TextWrappingBounds = bounds;
                }
                else
                {
                    if (isFrameHasSingleParagraph)
                    {
                        // Update Horizontal Distance from text into the Text wrapping frame bounds
                        bounds.Width += (2 * paragraph.ParagraphFormat.FrameHorizontalDistanceFromText);
                        bounds.X -= paragraph.ParagraphFormat.FrameHorizontalDistanceFromText;
                    }
                    else
                    {
                        // Update Horizontal Distance from text and margin values into the Text wrapping frame bounds
                        bounds.Width += ((2 * paragraph.ParagraphFormat.FrameHorizontalDistanceFromText) + (float)(LayoutInfo.Margins.Left + LayoutInfo.Margins.Right));
                        bounds.X -= (paragraph.ParagraphFormat.FrameHorizontalDistanceFromText + (float)(LayoutInfo.Margins.Left));
                    }
                    floatingItem.TextWrappingBounds = bounds;
                }
                floatingItem.FloatingEntity = m_ltWidget.Widget as Entity;
                (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
            }
        }
        /// <summary>
        /// Updates the Line space
        /// </summary>
        /// <param name="m_ltWidget"></param>
        /// <returns></returns>
        private float GetLineSpacing(LayoutedWidget m_ltWidget, ref bool isExactlyLineSpace)
        {
            float linespace = 0;
            WTextRange textRange = null;
            WParagraph paragraph = (m_ltWidget.Widget is WParagraph) ? (m_ltWidget.Widget as WParagraph)
                : (m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
            float paraLineSpacing = Math.Abs(paragraph.ParagraphFormat.LineSpacing);
            if (paragraph.SectionEndMark && paragraph.PreviousSibling != null)
                return 0;
            //Measure the line space height
            float maxHeight=0;
            //Get maxheight from the child elements when the paragraph have the textrange.
            if (!(paragraph.BreakCharacterFormat.FontName == "Arial Unicode MS" && paragraph.ChildEntities.Count > 0))
                maxHeight = (paragraph as IWidget).LayoutInfo.Size.Height;
            if (m_ltWidget.ChildWidgets.Count == 1 && (m_ltWidget.ChildWidgets[0].Widget is Break)
                && (m_ltWidget.ChildWidgets[0].Widget as Break).BreakType == BreakType.LineBreak)
            {
                int index = paragraph.ChildEntities.IndexOf(m_ltWidget.ChildWidgets[0].Widget as Entity);
                if (index > 0 && !(paragraph.ChildEntities[index - 1] is Break))
                    return linespace;
                maxHeight = DrawingContext.MeasureString(" ", (m_ltWidget.ChildWidgets[0].Widget as Break).TextRange.CharacterFormat.Font, null).Height;
            }
            if (m_ltWidget.ChildWidgets.Count > 0 && m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget is Break
                && ((m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget as Break).BreakType == BreakType.ColumnBreak))
            {
                bool isNeedToUpdateLineSpace = false;
                for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
                {
                    if (m_ltWidget.ChildWidgets[i].Bounds.Height > 0)
                        isNeedToUpdateLineSpace = true;
                }
                if (!isNeedToUpdateLineSpace)
                    return linespace;
            }
            if (m_ltWidget.ChildWidgets.Count > 0 && m_ltWidget.ChildWidgets[0].Widget is WParagraph && m_ltWidget.ChildWidgets[0].ChildWidgets.Count != 0)
                return linespace;
            if (paragraph.ParagraphFormat.LineSpacingRule != LineSpacingRule.Multiple)
            {
                if (paragraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly)
                    isExactlyLineSpace = true;
                linespace = paraLineSpacing;
            }
            else
            {
                int firstIndexofTextRange = 0;
                for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
                {
                    textRange = GetTextRange(m_ltWidget.ChildWidgets[i].Widget);
                    if (textRange != null && (textRange.m_layoutInfo as TabsLayoutInfo) == null)
                    {
                        if (textRange.OwnerParagraph == null)
                        {
                            maxHeight = (paragraph as IWidget).LayoutInfo.Size.Height;
                            firstIndexofTextRange = i;
                            break;
                        }
                        firstIndexofTextRange = i;
                        string text = GetText(m_ltWidget.ChildWidgets[i].Widget);
                        maxHeight = m_ltWidget.ChildWidgets[i].Widget.LayoutInfo.Size.Height;
                        if (text != textRange.Text)
                            maxHeight = DrawingContext.MeasureTextRange(textRange, text).Height;
                        break;
                    }
                }
                for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
                {
                    textRange = GetTextRange(m_ltWidget.ChildWidgets[i].Widget);
                    if (textRange != null && i != firstIndexofTextRange && textRange.OwnerParagraph != null && (textRange.m_layoutInfo as TabsLayoutInfo) == null)
                    {

                        float height = m_ltWidget.ChildWidgets[i].Widget.LayoutInfo.Size.Height;
                        string text = GetText(m_ltWidget.ChildWidgets[i].Widget);
                        if (text != textRange.Text)
                            height = DrawingContext.MeasureTextRange(textRange, text).Height;
                        if (maxHeight < height)
                            maxHeight = height;
                    }
                }
                ParagraphLayoutInfo paraInfo = (paragraph.m_layoutInfo as ParagraphLayoutInfo);
                if (paraInfo.ListValue != string.Empty && (m_ltWidget.Widget is WParagraph))
                {
                    float height = DrawingContext.MeasureString(" ", paraInfo.CharacterFormat.Font, null,null,true).Height;
                    //Update line space height based on list character font
                    if (maxHeight < height)
                        maxHeight = height;
                }
                linespace = maxHeight * (paraLineSpacing / 12.0f);
                if (linespace < 12 && paraLineSpacing < 12)
                {
                    isExactlyLineSpace = true;
                }
            }
            return linespace;
        }
        /// <summary>
        /// Get Text Whether the LeafWidget is WTextRange or SplitStringWidget
        /// </summary>
        /// <returns></returns>
        private string GetText(IWidget widget)
        {
            return (widget is WTextRange) ? (widget as WTextRange).Text : (widget as SplitStringWidget).SplittedText;
        }
        /// <summary>
        /// Get the Text Range
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private WTextRange GetTextRange(IWidget widget)
        {
            return ((widget is WTextRange) ? (widget as WTextRange)
                     : ((widget is SplitStringWidget) ? (widget as SplitStringWidget).RealStringWidget as WTextRange : null));
        }
        /// <summary>
        /// Updates the frame bounds.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void UpdateFrameBounds(WParagraph paragraph)
        {
            RectangleF bounds = m_ltWidget.Bounds;
            // Updates the frame size for the last item.
            if ((paragraph.NextSibling == null
                || (paragraph.NextSibling is WParagraph
                && !paragraph.ParagraphFormat.IsNextParagraphInSameFrame())
                || (paragraph.NextSibling is WTable
                && !(paragraph.NextSibling as WTable).IsFrame))
                && paragraph.ParagraphFormat.FrameHeight != 0)
            {
                ushort heightValue = (ushort)(paragraph.ParagraphFormat.FrameHeight * DLSConstants.TwipsInOnePoint);
                bool isAtleastHeight = (heightValue & (1 << 15)) != 0;
                float frameHeight = ((heightValue & ((1 << 15) - 1)) / DLSConstants.TwipsInOnePoint);
                if (isAtleastHeight && m_currChildLW.Bounds.Height < (m_lcOperator as Layouter).FrameHeight)
                {
                    bounds.Height = (m_lcOperator as Layouter).FrameHeight;
                    m_currChildLW.Bounds = bounds;
                }
                else if(!isAtleastHeight)
                {
                    bounds.Height = bounds.Height + (m_lcOperator as Layouter).FrameLayoutArea.Bottom - bounds.Bottom;
                    m_currChildLW.Bounds = bounds;
                }
            }
            // Update the horizontal alignment of frame
            UpdateHorizontalAlignment((short)paragraph.ParagraphFormat.FrameX);
            // Update the vertical alignment of frame
            UpdateVerticalAlignment((short)paragraph.ParagraphFormat.FrameY);
        }
        /// <summary>
        /// Updates the frame bounds.
        /// </summary>
        /// <param name="table">The table.</param>
        private void UpdateFrameBounds(WTable table)
        {
            RectangleF bounds = m_currChildLW.Bounds;
            // Updates the frame size for the last item.
            if ((table.NextSibling == null
                || (table.NextSibling is WParagraph
                && !(table.NextSibling as WParagraph).ParagraphFormat.IsFrame)
                || (table.NextSibling is WTable
                && !(table.NextSibling as WTable).IsFrame))
                && table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.FrameHeight != 0)
            {
                WParagraphFormat paraFormat = table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat;
                ushort heightValue = (ushort)(paraFormat.FrameHeight * DLSConstants.TwipsInOnePoint);
                bool isAtleastHeight = (heightValue & (1 << 15)) != 0;
                float frameHeight = ((heightValue & ((1 << 15) - 1)) / DLSConstants.TwipsInOnePoint);
                if (!isAtleastHeight || m_currChildLW.Bounds.Height < frameHeight)
                {
                    bounds.Height = bounds.Height + (m_lcOperator as Layouter).FrameLayoutArea.Bottom - bounds.Bottom;
                    m_currChildLW.Bounds = bounds;
                }
            }

            // Update the horizontal alignment of frame
            UpdateHorizontalAlignment((short)table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.FrameX);
            // Update the vertical alignment of frame
            UpdateVerticalAlignment((short)table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.FrameY);
            FloatingItem floatingItem = new FloatingItem();
            floatingItem.TextWrappingBounds = m_currChildLW.Bounds;
            floatingItem.FloatingEntity = m_currChildLW.Widget as Entity;
            (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
        }
        /// <summary>
        /// Updates the horizontal alignment.
        /// </summary>
        /// <param name="xAlginment">The x alginment.</param>
        protected virtual void UpdateHorizontalAlignment(short xAlignment)
        { }
        /// <summary>
        /// Updates the vertical alignment.
        /// </summary>
        /// <param name="yAlginment">The y alginment.</param>
        private void UpdateVerticalAlignment(short yAlginment)
        {
            RectangleF bounds = m_currChildLW.Bounds;
            switch (yAlginment)
            {
                case (short)FrameVerticalPosition.Center:
                    // Update the center alignment of frame
                    m_currChildLW.ShiftLocation(0, -bounds.Height / 2, true);
                    break;
                case (short)FrameVerticalPosition.Outside:
                case (short)FrameVerticalPosition.Bottom:
                    // Update the bottom alignment of frame
                    m_currChildLW.ShiftLocation(0, -bounds.Height, true);
                    break;
            }
        }
        /// <summary>
        /// Determines whether to update cell LW bounds.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if is update LW cell bounds; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUpdateCellLWBounds(float bottomPosition)
        {
            bool updateCellBounds = false;
            if (m_ltWidget.Widget is WTableCell && (m_ltWidget.Widget as WTableCell).CellFormat.VerticalAlignment == VerticalAlignment.Top
                && m_ltWidget.Widget.LayoutInfo.Margins.Bottom > 0 && m_ltWidget.Widget.LayoutInfo.Paddings.Bottom <= 0
                && (m_ltWidget.Widget as WTableCell).Items.LastItem is WParagraph
                && ((m_ltWidget.Widget as WTableCell).Items.LastItem as WParagraph).m_layoutInfo != null//To avoid the layoutinfo for the last paragraph of the table cell 
                && ((m_ltWidget.Widget as WTableCell).Items.LastItem as IWidget).LayoutInfo.Margins.Bottom == 0
                && !DrawingContext.IsEmptyParagraph(((m_ltWidget.Widget as WTableCell).Items.LastItem as WParagraph))
                && IsCellHeightNeedToBeUpdate(m_ltWidget, bottomPosition))            {
                if ((m_ltWidget.Widget as WTableCell).OwnerRow.HeightType == TableRowHeightType.Exactly)
                {
                    float height = (m_ltWidget.Widget as WTableCell).OwnerRow.Height;
                    if (height < 0)
                        height = -(height);
                    if (height <= 1)
                        updateCellBounds = true;
                }
                else
                    updateCellBounds = true;
            }
            return updateCellBounds;
        }
        /// <summary>
        /// Determine whether the cell height is need to be updated or not
        /// </summary>
        /// <returns></returns>
        private bool IsCellHeightNeedToBeUpdate(LayoutedWidget ltWidget, float bottomPosition)
        {
            if (ltWidget.ChildWidgets.Count > 0)
            {
                LayoutedWidget lastWidget = ltWidget.ChildWidgets[ltWidget.ChildWidgets.Count - 1];
                while (lastWidget.ChildWidgets.Count > 0)
                {
                    lastWidget = lastWidget.ChildWidgets[lastWidget.ChildWidgets.Count - 1];
                }
                if (lastWidget != null && Math.Round(lastWidget.Bounds.Bottom, 2) >= bottomPosition)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether paragraph is empty.
        /// </summary>
        /// <returns></returns>
        internal bool IsEmptyParagraph()
        {
            if (m_currChildLW.Widget != null && m_currChildLW.ChildWidgets.Count > 0)
            {
                for (int i = 0; i < m_currChildLW.ChildWidgets.Count; i++)
                {
                    if (!(m_currChildLW.ChildWidgets[i].Widget is BookmarkStart || m_currChildLW.ChildWidgets[i].Widget is BookmarkEnd || m_currChildLW.ChildWidgets[i].Widget is WFieldMark || (m_currChildLW.ChildWidgets[i].Widget is WTextRange && (m_currChildLW.ChildWidgets[i].Widget as WTextRange).Text.Trim() == string.Empty)))
                    {
                        return false;
                        break;
                    }
                    if (m_currChildLW.ChildWidgets[i].Widget is WParagraph)
                    {
                        LayoutedWidget widget = m_currChildLW.ChildWidgets[i];
                        WParagraph para = m_currChildLW.ChildWidgets[i].Widget as WParagraph;
                        if (para != null && widget.ChildWidgets.Count > 0)
                            for (int j = 0; j < widget.ChildWidgets.Count; j++)
                                if (!(widget.ChildWidgets[i].Widget is BookmarkStart || widget.ChildWidgets[j].Widget is BookmarkEnd || widget.ChildWidgets[j].Widget is WFieldMark || (widget.ChildWidgets[i].Widget is WTextRange && (widget.ChildWidgets[i].Widget as WTextRange).Text.Trim() == string.Empty)))
                                {
                                    return false;
                                }
                    }
                }
            }
            else
                return true;
            return true;
        }
        /// <summary>
        /// Commits the keep with next.
        /// </summary>
        private void CommitKeepWithNext(ref IWidget splittedWidget)
        {
            bool isKeep = IsNeedToCommitKeepWithNext();
            while (m_ltWidget.ChildWidgets.Count > 0 && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter && isKeep)
            {
                bool isKeepWithNext = false;
                IWidget widget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1].Widget;
                if ((widget is WTable) && !((widget as WTable).TableFormat.WrapTextAround || (widget as WTable).OwnerTextBody is WTableCell))
                {
                    LayoutedWidget tableWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];
                    int rowCount = tableWidget.ChildWidgets.Count;
                    while (tableWidget.ChildWidgets.Count > 0)
                    {
                        LayoutedWidget ltWidget = tableWidget.ChildWidgets[tableWidget.ChildWidgets.Count - 1];
                        if (ltWidget.Widget.LayoutInfo.IsKeepWithNext && Math.Round(ltWidget.Bounds.Y, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                        {
                            tableWidget.ChildWidgets.RemoveAt(tableWidget.ChildWidgets.Count - 1);
                            isKeepWithNext = true;
                            rowCount -= 1;
                        }
                        else
                            break;
                    }
                    if (isKeepWithNext)
                    {
                        if (tableWidget.ChildWidgets.Count > 0)
                        {
                            splittedWidget = new SplitTableWidget(tableWidget.Widget as ITableWidget, rowCount + 1);
                            m_ltState = LayoutState.Splitted;
                            m_curWidgetIndex -= 1;
                            break;
                        }
                        else
                        {
                            m_ltWidget.ChildWidgets.RemoveAt(m_ltWidget.ChildWidgets.Count - 1);
                            m_curWidgetIndex -= 1;
                        }
                    }
                    else
                        break;
                }
                else if ((widget.LayoutInfo as ParagraphLayoutInfo) != null
                    && Math.Round((widget.LayoutInfo as ParagraphLayoutInfo).YPosition, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2)
                    && widget.LayoutInfo.IsKeepWithNext && m_bAtLastOneChildFitted)
                {
                    //Update Footnote widgets
                    UpdateFootnoteWidgets(m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1]);
                    m_ltWidget.ChildWidgets.RemoveAt(m_ltWidget.ChildWidgets.Count - 1);
                    m_curWidgetIndex -= 1;
                }
                else
                    break;
            }
        }
        /// <summary>
        /// Gets the owner section.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private bool IsAllTextBodyItemHavingKeepWithNext()
        {
            for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
            {
                if (!(m_ltWidget.ChildWidgets[i].Widget.LayoutInfo.IsKeepWithNext))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Determine whether current widget is need to commit Keep with next property.
        /// </summary>
        /// <returns></returns>
        private bool IsNeedToCommitKeepWithNext()
        {
            bool isKeepWithNext = false;
            if (m_ltWidget.ChildWidgets.Count > 0)
            {
                for (int i = m_ltWidget.ChildWidgets.Count - 1; i > 0; i--)
                {
                    IWidget widget = m_ltWidget.ChildWidgets[i].Widget;
                    if (!((widget is WTable) && (widget as WTable).TableFormat.WrapTextAround))
                    {
                        if (!widget.LayoutInfo.IsKeepWithNext)
                        {
                            isKeepWithNext = true;
                            break;
                        }
                        if (widget.LayoutInfo.IsFirstItemInPage)
                        {
                            if (!DocumentLayouter.IsFirstLayouting)
                                isKeepWithNext = widget.LayoutInfo.IsKeepWithNext;
                            break;
                        }
                    }
                    else
                    {
                        isKeepWithNext = true;
                        break;
                    }
                }
                //Get the owner section for the current table
                WSection section = (m_ltWidget.Widget is WSection) ? m_ltWidget.Widget as WSection
                : m_ltWidget.Widget is SplitWidgetContainer ? (m_ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer as WSection : null;
                //Check whether the current section greater than 1 and page contains the continues break and all text body items having keepwithnext property true.
                //if all condition true means keepwithnext property true otherwise false
                if (section != null && section.BreakCode == SectionBreakCode.NoBreak && section.GetIndexInOwnerCollection() > 0 && IsAllTextBodyItemHavingKeepWithNext())
                    isKeepWithNext = true;
            }
            return isKeepWithNext;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class LCLineContainer : LCContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LCLineContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LCLineContainer(IWidgetContainer container, ILCOperator lcOperator)
            : base(container, lcOperator)
        {
            m_bSkipAreaSpacing = true;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Layouted current child
        /// </summary>
        /// <param name="childContext"></param>
        protected override void DoLayoutChild(LayoutContext childContext)
        {
            // when moving to the next line for layouting
            m_isTabStopBeyondRightMarginExists = false;
            childContext.m_isTabStopBeyondRightMarginExists = false;
            RectangleF clArea = m_layoutArea.ClientActiveArea;

            ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;

            bool isFirst = false;

            if (paragraphInfo != null)
            {
                isFirst = (paragraphInfo.FirstLineIndent != 0);
            }


            if (m_ltWidget.ChildWidgets.Count == 0)
            {
                if (isFirst && (childContext.Widget as SplitWidgetContainer) == null
                    && paragraphInfo.LevelNumber < 0)
                {
                    clArea.X += paragraphInfo.FirstLineIndent;
                    clArea.Width -= paragraphInfo.FirstLineIndent;
                }

                //Update the list indent for level greater than 0.
                if (paragraphInfo.LevelNumber != -1 && !(childContext.Widget is SplitWidgetContainer))
                {
                    if (isFirst)
                    {
                        clArea.X += paragraphInfo.FirstLineIndent + paragraphInfo.ListTab;
                        clArea.Width -= paragraphInfo.FirstLineIndent + paragraphInfo.ListTab;
                    }
                    else if (!isFirst)
                    {
                        clArea.X += paragraphInfo.ListTab;
                        clArea.Width -= paragraphInfo.ListTab;
                    }
                }
                if (childContext.Widget is SplitWidgetContainer && (childContext.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                {
                    WParagraph para = (childContext.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
                    int count = para.ChildEntities.Count;
                    if (count > (childContext.Widget as SplitWidgetContainer).Count && para.ChildEntities[count - 1 - (childContext.Widget as SplitWidgetContainer).Count] is Break)
                    {
                        Entity ent = (Entity)para.ChildEntities[count - 1 - (childContext.Widget as SplitWidgetContainer).Count];
                        Break br = ent as Break;
                        if (br.BreakType == BreakType.PageBreak || br.BreakType == BreakType.ColumnBreak)
                        {
                            clArea.X += paragraphInfo.FirstLineIndent + paragraphInfo.ListTab;
                            clArea.Width -= paragraphInfo.FirstLineIndent + paragraphInfo.ListTab;
                        }
                    }
                }
            }

            m_currChildLW = childContext.Layout(clArea);
            (m_lcOperator as Layouter).PreviousTab = new TabsLayoutInfo.LayoutTab();
            (m_lcOperator as Layouter).PreviousTabWidth = 0f;
        }

        /// <summary>
        /// Creates the next child context.
        /// </summary>
        /// <returns></returns>
        protected override LayoutContext CreateNextChildContext()
        {
            return (WidgetContainer != null)
              ? new LCContainer(WidgetContainer, m_lcOperator) : null;
        }

        /// <summary>
        /// Commits for not fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected override void MarkAsNotFitted(LayoutContext childContext, bool isFootnote)
        {
            m_bIsVerticalNotFitted = childContext.IsVerticalNotFitted;

            if ( /*m_ltState == LayoutState.Fitting AtLastOneChildFitted*/ m_bAtLastOneChildFitted)
            {
                bool isKeepTogether = IsKeepLineTogether(childContext);

                if (!isKeepTogether)
                {
                    if (!IsLastParagraphNeedToBeLayout(childContext) && !m_ltWidget.IsNotFitted)
                    {
                        if (m_notFittedWidget != null)
                        {
                            m_sptWidget = m_notFittedWidget;
                            m_notFittedWidget = null;
                        }
                        else
                        {
                            WParagraph paragraph = GetParagraph();
                            if (paragraph != null && !paragraph.IsInCell)
                            {
                                //Layout Last line of the page when it has splitted
                                LayoutContext lc = LayoutContext.Create(childContext.SplittedWidget, m_lcOperator, (m_lcOperator as Layouter).ClientLayoutArea.Width);
                                RectangleF rect = new RectangleF((m_lcOperator as Layouter).ClientLayoutArea.X, (m_lcOperator as Layouter).ClientLayoutArea.Y, (m_lcOperator as Layouter).ClientLayoutArea.Width, (m_lcOperator as Layouter).ClientLayoutArea.Height);
                                lc.m_bIsNeedToWrap = false;
                                LayoutedWidget widget = lc.Layout(rect);
                                //Update footnote widgets
                                UpdateFootnoteWidgets(widget);
                                widget.InitLayoutInfo();
                                //If the last line is splitted then we need to split this line into next page
                                if (widget.TextTag == "Splitted"
                                    || ((childContext.SplittedWidget is SplitWidgetContainer)
                                    && (((childContext.SplittedWidget as SplitWidgetContainer).m_currentChild is WPicture)
                                    || ((childContext.SplittedWidget as SplitWidgetContainer).m_currentChild is Shape))))
                                {
                                    m_sptWidget = childContext.SplittedWidget;
                                }
                                else
                                {
                                    //If the last line of the paragraph is not splitted then we need to split previous layouted line into next page. (As per MSWord behavior)
                                    LayoutedWidget ltWidget = m_ltWidget.ChildWidgets[m_ltWidget.ChildWidgets.Count - 1];
                                    if (ltWidget.Widget is SplitWidgetContainer)
                                    {
                                        m_sptWidget = ltWidget.Widget as SplitWidgetContainer;
                                        UpdateFootnoteWidgets(ltWidget);
                                        ltWidget.InitLayoutInfo();
                                        m_ltWidget.ChildWidgets.Remove(ltWidget);
                                    }
                                    else
                                        m_sptWidget = childContext.SplittedWidget;
                                }
                            }
                            else
                                m_sptWidget = childContext.SplittedWidget;
                        }
                        m_ltWidget.IsLastItemInPage = true;
                        m_ltState = LayoutState.Splitted;
                    }
                    else if (m_ltWidget.ChildWidgets.Count == 2)
                    {
                        (LayoutInfo as ParagraphLayoutInfo).IsNotFitted = true;
                        m_notFittedWidget = childContext.SplittedWidget;
                        m_ltWidget.IsNotFitted = true;
                        MarkAsSplitted(childContext);
                    }
                    else
                    {
                        m_notFittedWidget = null;
                        m_ltState = LayoutState.NotFitted;
                        //Update footnote widgets
                        UpdateFootnoteWidgets();
                    }
                }
                else
                {
                    //Update footnote widgets
                    UpdateFootnoteWidgets();
                    m_ltState = LayoutState.NotFitted;
                }
            }
            else
            {
                //Update footnote widgets
                UpdateFootnoteWidgets();
                m_ltState = LayoutState.NotFitted;
            }
        }
        /// <summary>
        /// Update footnote widgets
        /// </summary>
        private void UpdateFootnoteWidgets()
        {
            WParagraph paragraph = GetParagraph();
            if (paragraph == null)
                return;
            else
                UpdateFootnoteWidgets(paragraph);
        }
        /// <summary>
        /// Determine whether is KeepLineTogether
        /// </summary>
        /// <param name="childContext"></param>
        /// <returns></returns>
        private bool IsKeepLineTogether(LayoutContext childContext)
        {
            ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;
            bool isKeepTogether = false;
            if (paragraphInfo != null)
            {
                isKeepTogether = paragraphInfo.IsKeepTogether;
                WParagraph paragraph = GetParagraph();
                if (paragraph != null && ((paragraph.IsInCell || IsInFrame(paragraph)))
                    || Math.Round(paragraphInfo.YPosition, 2) == Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                    isKeepTogether = false;
            }
            return isKeepTogether;
        }
        /// <summary>
        /// Determine whether the last paragraph is need to be layout in Next page
        /// </summary>
        /// <returns></returns>
        private bool IsLastParagraphNeedToBeLayout(LayoutContext childContext)
        {
            ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;
            //If the paragraph is fitted single line in the current page then we need to layout the entire last paragraph into next page
            if (paragraphInfo != null)
            {
                WParagraph paragraph = GetParagraph();
                if (paragraph != null && !paragraph.IsInCell
                    && !IsInFrame(paragraph)
                    && (childContext.Widget is SplitWidgetContainer)
                    && Math.Round(paragraphInfo.YPosition, 2) != Math.Round((m_lcOperator as Layouter).PageTopMargin, 2)
                    && m_ltWidget.ChildWidgets.Count < 3 
                    && IsParagraphContainTextRanges())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determine whether the paragraph contain textranges alone.
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private bool IsParagraphContainTextRanges()
        {
            for (int i = 0; i < m_ltWidget.ChildWidgets.Count; i++)
            {
                for (int j = 0; j < m_ltWidget.ChildWidgets[i].ChildWidgets.Count;j++)
                {
                    if ((m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget is WPicture)
                        || m_ltWidget.ChildWidgets[i].ChildWidgets[j].Widget is Shape)
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Commits for fitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected override void MarkAsFitted(LayoutContext childContext)
        {
            ParagraphLayoutInfo paragraphInfo = childContext.LayoutInfo as ParagraphLayoutInfo;
            if (paragraphInfo != null && m_ltWidget.IsNotFitted)
            {
                (LayoutInfo as ParagraphLayoutInfo).IsNotFitted = false;
                MarkAsNotFitted(childContext, false);
            }
            else
            {
                AddChildLW(childContext);
                bool isPageBreak = (paragraphInfo != null) ? paragraphInfo.IsPageBreak : false;
                m_ltState = LayoutState.Fitted;
                if (isPageBreak)
                {
                    m_layoutArea.CutFromTop();
                    m_ltState = LayoutState.Breaked;
                }
            }
        }

        /// <summary>
        /// Commits for splitted.
        /// </summary>
        /// <param name="childContext">The child context.</param>
        protected override void MarkAsSplitted(LayoutContext childContext)
        {
            if ((LayoutInfo as ParagraphLayoutInfo) != null && m_ltWidget.IsNotFitted && m_ltWidget.ChildWidgets.Count > 2)
            {
                (LayoutInfo as ParagraphLayoutInfo).IsNotFitted = false;
                m_ltWidget.IsNotFitted = false;
                MarkAsNotFitted(childContext, false);
            }
            else
            {
                AddChildLW(childContext);
                m_widget = childContext.SplittedWidget;
                //m_ltState = LayoutState.Fitting;
                m_bAtLastOneChildFitted = true;
                if ((LayoutInfo as ParagraphLayoutInfo) != null)
                {
                    (LayoutInfo as ParagraphLayoutInfo).IsFirstLine = false;
                }
            }
        }

        /// <summary>
        /// Updates the client area.
        /// </summary>
        protected override void UpdateClientArea()
        {
            RectangleF bounds = m_currChildLW.Bounds;
            bounds.Height -= (float)m_currChildLW.Widget.LayoutInfo.Margins.Top;
            float footnoteHeight = 0;
            m_currChildLW.GetFootnoteHeight(ref footnoteHeight);
            double bottom = bounds.Bottom;
            //update absolute table bottom as y position when entire word split to the bottom of the absolute table.
            if ((m_lcOperator as Layouter).FloatingTableBottom!= float.MinValue)
            {
                bottom = (m_lcOperator as Layouter).FloatingTableBottom;
                (m_lcOperator as Layouter).FloatingTableBottom = float.MinValue;
            }
            m_layoutArea.CutFromTop(bottom, footnoteHeight);
        }

        /// <summary>
        /// Changes the childs alignment.
        /// </summary>
        protected override double ChangeChildsAlignment()
        {
           double lineSpace=m_currChildLW.AlignBottom(DrawingContext);
            bool isLastLine = false;
            ParagraphLayoutInfo paragraphInfo = m_currChildLW.Widget.LayoutInfo as ParagraphLayoutInfo;
            HorizontalAlignment alignment = (paragraphInfo != null) ? paragraphInfo.Justification : HorizontalAlignment.Left;
            //Calculate the subwidth for the current child widget
            double subWidth = 0;
            float childBoundsRight = 0;
            if (m_currChildLW.ChildWidgets.Count > 1)
            {
                for (int j = 0; j < m_currChildLW.ChildWidgets.Count; j++)
                {
                    EntityType entityType = new EntityType();
                    if (m_currChildLW.ChildWidgets[j].Widget is SplitStringWidget)
                        entityType = EntityType.TextRange;
                    else
                        entityType = (m_currChildLW.ChildWidgets[j].Widget as Entity).EntityType;
                    if (m_currChildLW.ChildWidgets[j].Widget is WTextRange)//assign the entity type as textrange for remaining entity those who are come under the WtextRange ,
                        entityType = EntityType.TextRange;
                    switch (entityType)
                    {
                        case EntityType.Symbol:
                            childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Right;
                            break;
                        case EntityType.Table:
                            {
                                if ((m_currChildLW.ChildWidgets[j].Widget as WTable).m_isTextBox)
                                {
                                    if ((m_currChildLW.ChildWidgets[j].Widget as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.Inline)
                                        childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Right;
                                }
                            }
                            break;
                        case EntityType.OleObject://handle common code both ole object and picture because ole object same like image.
                        case EntityType.Picture:
                            WPicture picture = (m_currChildLW.ChildWidgets[j].Widget is WOleObject) ? ((m_currChildLW.ChildWidgets[j].Widget as WOleObject).OlePicture as WPicture) : m_currChildLW.ChildWidgets[j].Widget as WPicture;
                            if (picture != null && picture.TextWrappingStyle == TextWrappingStyle.Inline)
                                childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Right;
                            break;
                        case EntityType.AutoShape:
                            Shape shape = m_currChildLW.ChildWidgets[j].Widget as Shape;
                            if (shape != null && shape.WrapFormat.TextWrappingStyle == TextWrappingStyle.Inline)
                                childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Right;
                            break;
                        case EntityType.TextRange:
                            {
                                if (m_currChildLW.ChildWidgets[j].Bounds.Width < 0)
                                    childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Left;
                                else
                                    childBoundsRight = m_currChildLW.ChildWidgets[j].Bounds.Right;
                            }
                            break;
                    }
                }
                subWidth = m_layoutArea.ClientActiveArea.Right - childBoundsRight - paragraphInfo.Margins.Right;                
            }
            else
                subWidth = m_layoutArea.ClientActiveArea.Right - m_currChildLW.Bounds.Right - paragraphInfo.Margins.Right;
            WParagraph paragraph = GetParagraph();
            if (alignment != HorizontalAlignment.Right)
                //Update sub width based on the Text wrap
                UpdateSubWidthBasedOnTextWrap(paragraph, ref subWidth);
            //Updates sub width for the text within the frame.
            if (paragraph != null
                && IsInFrame(paragraph))
            {
                if (paragraph.ParagraphFormat.FrameWidth == 0)
                {
                    //Set margin width as sub width.
                    if (paragraph.ParagraphFormat.IsNextParagraphInSameFrame() || paragraph.ParagraphFormat.IsPreviousParagraphInSameFrame())
                        subWidth = m_layoutArea.ClientActiveArea.Right - m_currChildLW.Bounds.Right - paragraphInfo.Margins.Right;
                    else
                        subWidth = 0;
                }
                else
                {
                    subWidth = paragraph.ParagraphFormat.FrameWidth - m_currChildLW.Bounds.Width - paragraphInfo.Margins.Right;
                }
            }
            //skip the subwidth update to zero when the isLastWordFit flag is true.  
            if (subWidth < 0.0f)
            {
                if (m_currChildLW.ChildWidgets.Count > 0 && m_currChildLW.ChildWidgets[m_currChildLW.ChildWidgets.Count - 1].TextTag == "IsLastWordFit")
                    m_currChildLW.ChildWidgets[0].TextTag = null;
                else
                    subWidth = 0.0f;
            }
            //Checks whether the paragraph is last line
            if ((m_currChildLW.Widget is WParagraph && m_currChildLW.TextTag != "Splitted" && m_currChildLW.ChildWidgets.Count > 0)
                || (m_currChildLW.Widget is SplitWidgetContainer && (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph
                && m_currChildLW.TextTag != "Splitted" && m_currChildLW.ChildWidgets.Count > 0))
            {
                if (m_currChildLW.ChildWidgets.Count > 0 && alignment == Syncfusion.Layouting.HorizontalAlignment.Justify)
                {
                    isLastLine = true;
                }
                //Remove split string widgets from the widget collection after layout the last line of the paragraph.
                RemoveSplitStringWidget(paragraph);
            }
            if (!(m_currChildLW.Widget is WParagraph && IsTOC(m_currChildLW.Widget as WParagraph)) && !isLastLine)
            {
                switch (alignment)
                {
                    case HorizontalAlignment.Center:
                        subWidth = subWidth / 2;
                        m_currChildLW.ShiftLocation(subWidth, 0, true);
                        break;
                    case HorizontalAlignment.Right:
                        m_currChildLW.AlignRight(DrawingContext, subWidth);
                        break;
                    case HorizontalAlignment.Distributed:
                    case HorizontalAlignment.Justify:
                        m_currChildLW.AlignJustify(DrawingContext, subWidth);
                        for (int i = 0; i < m_currChildLW.ChildWidgets.Count; i++)
                        {
                            m_currChildLW.ChildWidgets[i].HorizontalAlign = alignment;
                            m_currChildLW.ChildWidgets[i].Bounds = new RectangleF(m_currChildLW.ChildWidgets[i].Bounds.X, m_currChildLW.ChildWidgets[i].Bounds.Y, m_currChildLW.ChildWidgets[i].Bounds.Width /*+ Convert.ToSingle(m_currChildLW.ChildWidgets[i].m_subWidth)*/, m_currChildLW.ChildWidgets[i].Bounds.Height);
                            if (i < m_currChildLW.ChildWidgets.Count - 1)
                                m_currChildLW.ChildWidgets[i + 1].Bounds = new RectangleF(m_currChildLW.ChildWidgets[i + 1].Bounds.Location.X, m_currChildLW.ChildWidgets[i + 1].Bounds.Location.Y, m_currChildLW.ChildWidgets[i + 1].Bounds.Width, m_currChildLW.ChildWidgets[i + 1].Bounds.Height);
                        }
                        break;
                }
            }
            return lineSpace;
        }
        /// <summary>
        /// Remove split string widgets from the widget collection
        /// </summary>
        private void RemoveSplitStringWidget(WParagraph paragraph)
        {
            for (int i = 0; i < paragraph.ChildEntities.Count; i++)
            {
                if ((paragraph.ChildEntities as ParagraphItemCollection).GetCurrentWidget(i) is SplitStringWidget)
                {
                    paragraph.ChildEntities.InnerList.RemoveAt(i);
                    i--;
                }

            }
        }
        /// <summary>
        /// Updates sub width based on the Text wrap
        /// </summary>
        /// <param name="subWidth"></param>
        private void UpdateSubWidthBasedOnTextWrap(WParagraph paragraph,ref double subWidth)
        {
            ParagraphLayoutInfo paragraphInfo = m_currChildLW.Widget.LayoutInfo as ParagraphLayoutInfo;
            if ((m_lcOperator as Layouter).FloatingItems.Count > 0 && m_currChildLW.ChildWidgets.Count > 0 && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter && !(paragraph != null && IsInFootnote(paragraph)))
            {
                float prevWidth = 0;
                for (int i = 0; i < (m_lcOperator as Layouter).FloatingItems.Count; i++)
                {
                    RectangleF textWrappingBounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                    TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                    
                    if (m_layoutArea.ClientActiveArea.Right > textWrappingBounds.X && textWrappingBounds.X > m_currChildLW.Bounds.X
                        && m_currChildLW.Bounds.Bottom > textWrappingBounds.Y && m_currChildLW.Bounds.Y < textWrappingBounds.Bottom
                        && textWrappingStyle != TextWrappingStyle.Inline && textWrappingStyle != TextWrappingStyle.TopAndBottom
                        && textWrappingStyle != TextWrappingStyle.InFrontOfText && textWrappingStyle != TextWrappingStyle.Behind)
                    {
                        float diffwidth = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.X - (float)paragraphInfo.Margins.Right;
                        //Update subwidth based on the previous width difference reduced from subwidth
                        if (Math.Round(prevWidth, 2) < Math.Round(diffwidth, 2))
                        {
                            subWidth = subWidth - (diffwidth - prevWidth);
                            prevWidth = diffwidth;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates the horizontal alignment.
        /// </summary>
        /// <param name="xAlginment">The x alginment.</param>
        protected override void UpdateHorizontalAlignment(short xAlignment)
        {
            RectangleF bounds = m_currChildLW.Bounds;
            switch (xAlignment)
            {
                case (short)PageNumberAlignment.Center:
                    // Update the center alignment of frame
                    m_currChildLW.ShiftLocation(-bounds.Width / 2, 0, false);
                    break;
                case (short)PageNumberAlignment.Outside:
                case (short)PageNumberAlignment.Right:
                    // Update the right alignment of frame
                    m_currChildLW.ShiftLocation(-bounds.Width, 0, false);
                    break;
            }
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        internal WParagraph GetParagraph()
        {
            WParagraph paragraph = null;
            if (m_currChildLW.Widget is WParagraph)
                paragraph = m_currChildLW.Widget as WParagraph;
            else if (m_currChildLW.Widget is SplitWidgetContainer
                && (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                paragraph = (m_currChildLW.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
            return paragraph;
        }
        /// <summary>
        /// Checks the paragraph for TOC
        /// </summary>
        /// <param name="txtRange">paragraph</param>
        /// <returns></returns>
        internal bool IsTOC(WParagraph para)
        {
            if (para != null && para.ChildEntities.FirstItem != null && ((para.ChildEntities.FirstItem is TableOfContent) || ((para.ChildEntities.FirstItem is WField) && (para.ChildEntities.FirstItem as WField).FieldType == FieldType.FieldHyperlink) && (new Hyperlink(para.ChildEntities.FirstItem as WField).BookmarkName != null && new Hyperlink(para.ChildEntities.FirstItem as WField).BookmarkName.StartsWith("_Toc"))))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Does the layout after.
        /// </summary>
        protected override void DoLayoutAfter()
        {
            ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;
            WParagraph para = null;
            if (m_currChildLW.Widget is WParagraph)
                para = m_currChildLW.Widget as WParagraph;
            else if (m_currChildLW.Widget is SplitWidgetContainer)
            {
                SplitWidgetContainer swc = m_currChildLW.Widget as SplitWidgetContainer;
                if (swc.RealWidgetContainer is WParagraph)
                {
                    para = swc.RealWidgetContainer as WParagraph;
                }
            }
            if ((m_lcOperator as Layouter).UpdatingTOC)
                UpdateTOCPageNumber(para);
        }
        /// <summary>
        /// Updates the TOC page number.
        /// </summary>
        /// <param name="para">The para.</param>
        private void UpdateTOCPageNumber(WParagraph para)
        {
            if ((m_lcOperator as Layouter).TOCLevels.Count > 0)
            {
                string styleName = para.StyleName;
                if (styleName != null)
                    styleName = styleName.ToLower().Replace(" ", "");
                else
                    styleName = "normal";
                foreach (KeyValuePair<int, List<string>> level in (m_lcOperator as Layouter).TOCLevels)
                {
                    foreach (string ListStyleName in level.Value)
                    {
                        string levelStyle = ListStyleName.ToLower().Replace(" ", "");
                        if (styleName.StartsWith(levelStyle))
                        {
                            m_lcOperator.SendLeafLayoutAfter(m_ltWidget);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

#endif
