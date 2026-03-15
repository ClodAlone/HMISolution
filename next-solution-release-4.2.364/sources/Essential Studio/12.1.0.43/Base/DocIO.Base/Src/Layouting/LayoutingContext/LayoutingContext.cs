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
using System.Diagnostics;
using System.Drawing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.Rendering;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.Rendering;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Represents an layouting operator.
    /// </summary>
    internal interface ILCOperator
    {
        /// <summary>
        /// Gets or sets the custom graphics.
        /// </summary>
        /// <value>The custom graphics.</value>
        DrawingContext DrawingContext
        {
            get;
        }

        /// <summary>
        /// Sends the event that current leaf widget lay outed complete.
        /// </summary>
        /// <param name="ltWidget">The widget.</param>
        void SendLeafLayoutAfter(LayoutedWidget ltWidget);
    }

    /// <summary>
    /// Summary description for LayoutContext.
    /// </summary>
    internal abstract class LayoutContext
    {
        #region Constants
        /// <summary>
        /// The minimum width.
        /// </summary>
        internal const float DEF_MIN_WIDTH = 16f;
        /// <summary>
        /// The maximum width.
        /// </summary>
        internal const float MAX_WIDTH = 1584;
        #endregion
        #region Fields
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    private static string m_DBGIdentSpace = "";
#endif
        /// <summary>
        /// 
        /// </summary>
        protected LayoutState m_ltState = LayoutState.Unknown;

        /// <summary>
        /// 
        /// </summary>
        protected IWidget m_sptWidget = null;

        /// <summary>
        /// 
        /// </summary>
        protected IWidget m_notFittedWidget = null;
        /// <summary>
        /// 
        /// </summary>
        protected IWidget m_widget;

        /// <summary>
        /// 
        /// </summary>
        protected LayoutedWidget m_ltWidget = null;

        /// <summary>
        /// 
        /// </summary>
        protected bool m_bSkipAreaSpacing = false;

        //protected LayoutArea m_clientArea;
        /// <summary>
        /// 
        /// </summary>
        protected LayoutArea m_layoutArea;

        /// <summary>
        /// 
        /// </summary>
        protected ILCOperator m_lcOperator;
        /// <summary>
        /// 
        /// </summary>
        internal bool m_bIsNeedToWrap = true;

        /// <summary>
        /// 
        /// </summary>
        protected bool m_bIsVerticalNotFitted;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, SplitWidgetContainer[]> m_splittedWidget = new Dictionary<int, SplitWidgetContainer[]>();
        /// <summary>
        /// Right margin of client layout area
        /// </summary>
        private float m_clientLayoutAreaRight;
        /// <summary>
        /// Denotes if tab with stop position beyond right margin exists
        /// </summary>
        internal bool m_isTabStopBeyondRightMarginExists = false;
        /// <summary>
        /// Denotes whether area is updated based on isTabStopBeyondRightMarginExists
        /// </summary>
        internal bool m_isAreaUpdated = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the splitted widget.
        /// </summary>
        /// <value>The splitted widget.</value>
        public IWidget SplittedWidget
        {
            get
            {
                return m_sptWidget;
            }
        }

        /// <summary>
        /// Gets the layouted state.
        /// </summary>
        /// <value>The state.</value>
        public LayoutState State
        {
            get
            {
                return m_ltState;
            }
        }

        /// <summary>
        /// Gets the layout info.
        /// </summary>
        /// <value>The layout info.</value>
        public ILayoutInfo LayoutInfo
        {
            get
            {
                return m_widget.LayoutInfo;
            }
        }

        /// <summary>
        /// Gets the layout area.
        /// </summary>
        /// <value>The layout area.</value>
        public LayoutArea LayoutArea
        {
            get
            {
                return m_layoutArea;
            }
        }

        /// <summary>
        /// Gets the drawing context.
        /// </summary>
        /// <value>The drawing context.</value>
        public DrawingContext DrawingContext
        {
            get
            {
                return m_lcOperator.DrawingContext;
            }
        }

        /// <summary>
        /// Gets the bounds padding right.
        /// </summary>
        /// <value>The bounds padding right.</value>
        public double BoundsPaddingRight
        {
            get
            {
                return m_widget.LayoutInfo.Paddings.Right +
                  m_widget.LayoutInfo.Margins.Right;
            }
        }

        /// <summary>
        /// Gets the bounds padding bottom.
        /// </summary>
        /// <value>The bounds padding bottom.</value>
        public double BoundsPaddingBottom
        {
            get
            {
                return m_widget.LayoutInfo.Paddings.Bottom +
                  m_widget.LayoutInfo.Margins.Bottom;
            }
        }

        /// <summary>
        /// Gets the widget.
        /// </summary>
        /// <value>The widget.</value>
        public IWidget Widget
        {
            get
            {
                return m_widget;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is vertical not fitted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is vertical not fitted; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerticalNotFitted
        {
            get
            {
                return m_bIsVerticalNotFitted;
            }
        }
        /// <summary>
        /// Gets or sets the client layout area right.
        /// </summary>
        /// <value>
        /// The client active layout right.
        /// </value>
        internal float ClientLayoutAreaRight
        {
            get
            {
                return m_clientLayoutAreaRight;
            }
            set
            {
                m_clientLayoutAreaRight = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutContext"/> class.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LayoutContext(IWidget widget, ILCOperator lcOperator)
        {
            m_widget = widget;
            m_sptWidget = widget;
            m_lcOperator = lcOperator;
#if DEBUG_LAYOUTING      
      DBG_CreateNextChildContext( this );
#endif
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Layouts the specified widget.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public abstract LayoutedWidget Layout(RectangleF rect);

        /// <summary>
        /// Determines whether ensure splitted.
        /// </summary>
        /// <returns>
        /// <c>true</c> if ensure splitted; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnsureSplitted()
        {
            return State == LayoutState.Splitted && SplittedWidget != null;
        }

        /// <summary>
        /// Does the layout after.
        /// </summary>
        protected virtual void DoLayoutAfter()
        { }
        #endregion

        #region Implementation
        /// <summary>
        /// Layout Footnote
        /// </summary>
        /// <param name="footnote"></param>
        /// <returns></returns>
        internal void LayoutFootnote(WFootnote footnote, LayoutedWidget currLtWidget)
        {
            if (m_bIsNeedToWrap)
            {
                float height = 0f;
                float bottom = currLtWidget.Bounds.Bottom + (float)footnote.GetOwnerParagraph().m_layoutInfo.Margins.Bottom + currLtWidget.m_footnoteHeight;
                //Layout footnote separator
                if ((m_lcOperator as Layouter).m_bisNeedToRestartFootnote)
                {
                    LayoutFootnoteTextBody(footnote.Document.Footnotes.Separator, ref height, m_layoutArea.ClientActiveArea.Bottom - bottom);
                    (m_lcOperator as Layouter).m_bisNeedToRestartFootnote = false;
                }
                float minHeight = 0f;
                //Calculate minimum height to layout the footnote textbody
                if (footnote.TextBody.LastParagraph != null)
                {
                    minHeight = DrawingContext.MeasureString(" ", footnote.TextBody.LastParagraph.BreakCharacterFormat.Font, null).Height;
                }
                //Split the entire footnote textbody into next page when it's not fitted in the current page
                if (minHeight > (m_layoutArea.ClientActiveArea.Bottom - bottom))
                {
                    (m_lcOperator as Layouter).FootnoteSplittedWidgets.Add(new SplitWidgetContainer(footnote.TextBody, footnote.TextBody.Items[0], 0));
                    //Reset the Footnote ID when it restart with each page
                    if ((m_lcOperator as Layouter).m_bisNeedToRestartFootnoteID)
                    {
                        DocumentLayouter.m_footnoteIDRestartEachPage = 1;
                        (m_lcOperator as Layouter).m_bisNeedToRestartFootnoteID = false;
                    }
                    WParagraph paragraph = footnote.GetOwnerParagraph();
                    if (paragraph != null && (paragraph.OwnerTextBody.Owner as WSection).PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
                    {
                        (footnote.m_layoutInfo as FootnoteLayoutInfo).FootnoteID = (footnote.m_layoutInfo as FootnoteLayoutInfo).GetFootnoteID(footnote, DocumentLayouter.m_footnoteIDRestartEachPage++);
                        if (footnote.CustomMarkerIsSymbol || (footnote.CustomMarker != string.Empty))
                        {
                            DocumentLayouter.m_footnoteIDRestartEachPage--;
                        }
                    }
                }
                //Layout Footnote textbody
                else
                {
                    LayoutFootnoteTextBody(footnote.TextBody, ref height, this.m_layoutArea.ClientActiveArea.Bottom - bottom);
                }
                currLtWidget.m_footnoteHeight += height;
                (footnote.m_layoutInfo as FootnoteLayoutInfo).FootnoteHeight = height;
                //Clear the footnote widgets when the footnote widgets contain footnote separator textbody alone
                if ((m_lcOperator as Layouter).FootnoteWidgets.Count < 2)
                {
                    (m_lcOperator as Layouter).FootnoteWidgets.Clear();
                    (m_lcOperator as Layouter).FootNoteSectionIndex.Clear();
                }
            }
        }
        /// <summary>
        /// Layout Endnote
        /// </summary>
        /// <param name="endnote"></param>
        /// <returns></returns>
        internal void LayoutEndnote(WFootnote endnote, LayoutedWidget currLtWidget)
        {
            if (m_bIsNeedToWrap)
            {
                float height = 0f;
                float bottom = currLtWidget.Bounds.Bottom + (float)endnote.GetOwnerParagraph().m_layoutInfo.Margins.Bottom + currLtWidget.m_endnoteHeight;
                //Layout endnote separator
                if ((m_lcOperator as Layouter).m_bisNeedToRestartEndnote)
                {
                    LayoutEndnoteTextBody(endnote.Document.Footnotes.Separator, ref height, m_layoutArea.ClientActiveArea.Bottom - bottom);
                    (m_lcOperator as Layouter).m_bisNeedToRestartEndnote = false;
                }
                float minHeight = 0f;
                //Calculate minimum height to layout the endnote textbody
                if (endnote.TextBody.LastParagraph != null)
                {
                    minHeight = DrawingContext.MeasureString(" ", endnote.TextBody.LastParagraph.BreakCharacterFormat.Font, null).Height;
                }
                //Split the entire endnote textbody into next page when it's not fitted in the current page
                if (minHeight > (m_layoutArea.ClientActiveArea.Bottom - bottom))
                {
                    (m_lcOperator as Layouter).EndnoteSplittedWidgets.Add(new SplitWidgetContainer(endnote.TextBody, endnote.TextBody.Items[0], 0));
                    //Reset the Endnote ID when it restart with each page
                    if ((m_lcOperator as Layouter).m_bisNeedToRestartEndnote)
                    {
                        DocumentLayouter.m_footnoteIDRestartEachPage = 1;
                        (m_lcOperator as Layouter).m_bisNeedToRestartEndnote = false;
                    }
                    WParagraph paragraph = endnote.GetOwnerParagraph();
                    if (paragraph != null && (paragraph.OwnerTextBody.Owner as WSection).PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
                    {
                        (endnote.m_layoutInfo as FootnoteLayoutInfo).FootnoteID = (endnote.m_layoutInfo as FootnoteLayoutInfo).GetFootnoteID(endnote, DocumentLayouter.m_endnoteIDRestartEachPage++);
                        if (endnote.CustomMarkerIsSymbol || (endnote.CustomMarker != string.Empty))
                        {
                            DocumentLayouter.m_endnoteIDRestartEachPage--;
                        }
                    }
                }
                //Layout Endnote textbody
                else
                {
                    LayoutEndnoteTextBody(endnote.TextBody, ref height, this.m_layoutArea.ClientActiveArea.Bottom - bottom);
                }
                currLtWidget.m_endnoteHeight += height;
                (endnote.m_layoutInfo as FootnoteLayoutInfo).Endnoteheight = height;
                //Clear the endnote widgets when the endnote widgets contain endnote separator textbody alone
                if ((m_lcOperator as Layouter).EndnoteWidgets.Count < 2)
                {
                    (m_lcOperator as Layouter).EndnoteWidgets.Clear();
                    (m_lcOperator as Layouter).EndNoteSectionIndex.Clear();
                }
                
            }
        }
        /// <summary>
        /// Layout Footnote TextBody
        /// </summary>
        /// <param name="widgetContainer"></param>
        /// <param name="height"></param>
        /// <param name="clientHeight"></param>
        internal void LayoutFootnoteTextBody(IWidgetContainer widgetContainer, ref float height, float clientHeight)
        {
            LayoutContext context = Create(widgetContainer, m_lcOperator, (m_lcOperator as Layouter).ClientLayoutArea.Width);
            float y = (this.m_lcOperator as Layouter).ClientLayoutArea.Y;
            //Get Y position to layout footnote textbody
            if ((m_lcOperator as Layouter).FootnoteWidgets.Count > 0)
            {
                y = (m_lcOperator as Layouter).FootnoteWidgets[(m_lcOperator as Layouter).FootnoteWidgets.Count - 1].Bounds.Bottom;
            }
            //Get height to layout footnote textbody
            if ((m_lcOperator as Layouter).FootnoteWidgets.Count == 1)
            {
                clientHeight -= (m_lcOperator as Layouter).FootnoteWidgets[0].Bounds.Height;
            }
            RectangleF rect = new RectangleF((m_lcOperator as Layouter).ClientLayoutArea.X, y, (m_lcOperator as Layouter).ClientLayoutArea.Width, clientHeight);
            LayoutedWidget ltWidget = context.Layout(rect);
            //Check whether the footnote textbody is fitted or not
            if ((context.State == LayoutState.Splitted) || (context.State == LayoutState.NotFitted))
            {
                //Add splitted widget into footnote splitted widgets collection
                if (context.SplittedWidget is SplitWidgetContainer)
                {
                    (m_lcOperator as Layouter).FootnoteSplittedWidgets.Add(context.SplittedWidget as SplitWidgetContainer);
                    (m_lcOperator as Layouter).FootnoteWidgets.Add(ltWidget);
                }
                else if (context.SplittedWidget is WTextBody)
                {
                    (m_lcOperator as Layouter).FootnoteSplittedWidgets.Add(new SplitWidgetContainer(widgetContainer, (context.SplittedWidget as WTextBody).Items[0], 0));
                }
            }
            else
            {
                //Add layouted widget into footnote widgets collection
                (m_lcOperator as Layouter).FootnoteWidgets.Add(ltWidget);
            }
            height += ltWidget.Bounds.Height;
        }
        /// <summary>
        /// Layout Endnote TextBody
        /// </summary>
        /// <param name="widgetContainer"></param>
        /// <param name="height"></param>
        /// <param name="clientHeight"></param>
        internal void LayoutEndnoteTextBody(IWidgetContainer widgetContainer, ref float height, float clientHeight)
        {
            LayoutContext context = Create(widgetContainer, m_lcOperator, (m_lcOperator as Layouter).ClientLayoutArea.Width);
            float y = (this.m_lcOperator as Layouter).ClientLayoutArea.Y;
            //Get Y position to layout endnote textbody
            if ((m_lcOperator as Layouter).EndnoteWidgets.Count > 0)
            {
                y = (m_lcOperator as Layouter).EndnoteWidgets[(m_lcOperator as Layouter).EndnoteWidgets.Count - 1].Bounds.Bottom;
            }
            //Get height to layout endnote textbody
            if ((m_lcOperator as Layouter).EndnoteWidgets.Count == 1)
            {
                clientHeight -= (m_lcOperator as Layouter).EndnoteWidgets[0].Bounds.Height;
            }
            RectangleF rect = new RectangleF((m_lcOperator as Layouter).ClientLayoutArea.X, y, (m_lcOperator as Layouter).ClientLayoutArea.Width, clientHeight);
            LayoutedWidget ltWidget = context.Layout(rect);
            //Check whether the endnote textbody is fitted or not
            if ((context.State == LayoutState.Splitted) || (context.State == LayoutState.NotFitted))
            {
                //Add splitted widget into endnote splitted widgets collection
                if (context.SplittedWidget is SplitWidgetContainer)
                {
                    (m_lcOperator as Layouter).EndnoteSplittedWidgets.Add(context.SplittedWidget as SplitWidgetContainer);
                    (m_lcOperator as Layouter).EndnoteWidgets.Add(ltWidget);
                }
                else if (context.SplittedWidget is WTextBody)
                {
                    (m_lcOperator as Layouter).EndnoteSplittedWidgets.Add(new SplitWidgetContainer(widgetContainer, (context.SplittedWidget as WTextBody).Items[0], 0));
                }
            }
            else
            {
                //Add layouted widget into endnote widgets collection
                (m_lcOperator as Layouter).EndnoteWidgets.Add(ltWidget);
            }
            height += ltWidget.Bounds.Height;
        }
        /// <summary>
        /// Update footnote widgets
        /// </summary>
        internal void UpdateFootnoteWidgets(LayoutedWidget ltWidget)
        {
            if (ltWidget.Widget is WParagraph)
            {
                UpdateFootnoteWidgets(ltWidget.Widget as WParagraph);
                return;
            }
            for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
            {
                if (!(ltWidget.ChildWidgets[i].Widget is WFootnote))
                {
                    continue;
                }
                bool flag = false;
                int index = (m_lcOperator as Layouter).FootnoteWidgets.Count - 1;
                //Remove the footnote widgets when the layouted footnote marker has been removed from the layouted widget collection
                while (index >= 0)
                {
                    WTextBody body = ((m_lcOperator as Layouter).FootnoteWidgets[index].Widget is WTextBody)
                                    ? ((m_lcOperator as Layouter).FootnoteWidgets[index].Widget as WTextBody)
                                    : (((m_lcOperator as Layouter).FootnoteWidgets[index].Widget as SplitWidgetContainer).RealWidgetContainer as WTextBody);
                    if ((body.Owner as WFootnote) == ltWidget.ChildWidgets[i].Widget)
                    {
                        (m_lcOperator as Layouter).FootnoteWidgets.RemoveAt(index);
                        //Reset FootnoteID
                        ltWidget.ChildWidgets[i].Widget.InitLayoutInfo();
                        flag = true;
                        break;
                    }
                    index--;
                }
                //Remove the footnote splitted widgets when the layouted footnote marker has been removed from the layouted widget collection
                if (!flag)
                {
                    for (index = (m_lcOperator as Layouter).FootnoteSplittedWidgets.Count - 1; index >= 0; index--)
                    {
                        if ((((m_lcOperator as Layouter).FootnoteSplittedWidgets[index].RealWidgetContainer as WTextBody).Owner as WFootnote) == ltWidget.ChildWidgets[i].Widget)
                        {
                            (m_lcOperator as Layouter).FootnoteSplittedWidgets.RemoveAt(index);
                            //Reset FootnoteID
                            ltWidget.ChildWidgets[i].Widget.InitLayoutInfo();
                            break;
                        }
                    }
                }
            }
            //Clear the footnote widgets when the footnote widgets contain footnote separator textbody alone
            if ((m_lcOperator as Layouter).FootnoteWidgets.Count < 2)
            {
                (m_lcOperator as Layouter).FootnoteWidgets.Clear();
                (m_lcOperator as Layouter).FootNoteSectionIndex.Clear();
            }
        }
        /// <summary>
        /// Update footnote widgets
        /// </summary>
        internal void UpdateFootnoteWidgets(WParagraph paragraph)
        {
            for (int i = 0; i < paragraph.ChildEntities.Count; i++)
            {
                if (paragraph.ChildEntities[i] is WFootnote)
                {
                    bool flag = false;
                    //Remove the footnote widgets when the layouted footnote marker has been removed from the layouted widget collection
                    for (int j = (m_lcOperator as Layouter).FootnoteWidgets.Count - 1; j >= 0; j--)
                    {
                        WTextBody textbody = ((m_lcOperator as Layouter).FootnoteWidgets[j].Widget is WTextBody)
                                             ? ((m_lcOperator as Layouter).FootnoteWidgets[j].Widget as WTextBody)
                                             : ((m_lcOperator as Layouter).FootnoteWidgets[j].Widget as SplitWidgetContainer).RealWidgetContainer as WTextBody;
                        if ((textbody.Owner as WFootnote) == paragraph.ChildEntities[i])
                        {
                            (m_lcOperator as Layouter).FootnoteWidgets.RemoveAt(j);
                            (m_lcOperator as Layouter).FootNoteSectionIndex.RemoveAt(j);
                            //Reset FootnoteID
                            (paragraph.ChildEntities[i] as IWidget).InitLayoutInfo();
                            flag = true;
                            break;
                        }
                    }
                    //Remove the footnote splitted widgets when the layouted footnote marker has been removed from the layouted widget collection
                    if (!flag)
                    {
                        for (int j = (m_lcOperator as Layouter).FootnoteSplittedWidgets.Count - 1; j >= 0; j--)
                        {
                            if ((((m_lcOperator as Layouter).FootnoteSplittedWidgets[j].RealWidgetContainer as WTextBody).Owner as WFootnote) == paragraph.ChildEntities[i])
                            {
                                (m_lcOperator as Layouter).FootnoteSplittedWidgets.RemoveAt(j);
                                //Reset FootnoteID
                                (paragraph.ChildEntities[i] as IWidget).InitLayoutInfo();
                                break;
                            }
                        }
                    }
                }
            }
            //Clear the footnote widgets when the footnote widgets contain footnote separator textbody alone
            if ((m_lcOperator as Layouter).FootnoteWidgets.Count < 2)
            {
                (m_lcOperator as Layouter).FootnoteWidgets.Clear();
                (m_lcOperator as Layouter).FootNoteSectionIndex.Clear();
            }
        }
        /// <summary>
        /// Creates the layout area.
        /// </summary>
        /// <param name="rect">The Rectangle.</param>
        protected void CreateLayoutArea(RectangleF rect)
        {
            if (m_bSkipAreaSpacing)
            {
                m_layoutArea = new LayoutArea(rect);
                UpdateParagraphYPositionBasedonTextWrap();
            }
            else
            {
                m_layoutArea = new LayoutArea(rect, LayoutInfo, m_widget);
                if (!(m_lcOperator as Layouter).IsLayoutingHeaderFooter && (LayoutInfo as ParagraphLayoutInfo) != null
                    && Math.Round((LayoutInfo as ParagraphLayoutInfo).YPosition, 2) == Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))
                {
                    LayoutInfo.IsFirstItemInPage = true;
                    float topPad = (float)(LayoutInfo.Margins.Top + LayoutInfo.Paddings.Top);
                    WParagraph paragraph = (m_widget is WParagraph) ? (m_widget as WParagraph)
                        : (m_widget is SplitWidgetContainer) ? (m_widget as SplitWidgetContainer).RealWidgetContainer as WParagraph : null;
                    if (paragraph != null && !paragraph.IsInCell)
                    {
                        //Update Top Margin of the paragraph
                        UpdateParagraphTopMargin(paragraph);
                        if (LayoutInfo.Paddings.Top != 0 || LayoutInfo.Margins.Top != 0)
                            m_layoutArea.UpdateBounds(topPad);
                    }
                }
                if (m_widget is WParagraph //Check whether the current widget is Paragraph or not for added bounds points for floating paragraph item
                    || ((m_widget is SplitWidgetContainer) && (m_widget as SplitWidgetContainer).RealWidgetContainer is WParagraph
                    && (m_widget as SplitWidgetContainer).LayoutInfo.IsFirstItemInPage))//Check whether the current widget is split widget container or not and the owner of split widget container is paragraph or not and which is first item in page or not
                    LayoutTextWrapWidgets(m_widget);
            }
        }
        /// <summary>
        ///layout the bounds for wrapping element in paragraph collection
        /// </summary>
        private void LayoutTextWrapWidgets(IWidget widget)
        {
            //Check whether the current child widget is paragraph or not and floating element layouted or not.
            if ((widget is WParagraph) && !(widget as WParagraph).IsFloatingItemsLayouted || (widget is SplitWidgetContainer))
            {
                WParagraph para = null;
                int paraItemIndex = 0;
                
                if (widget is SplitWidgetContainer)
                {
                    //Get the owner paragraph 
                    para = ((widget as SplitWidgetContainer).WidgetInnerCollection.Owner as WParagraph);
                    //Get the current child index from the paragraph collection
                    paraItemIndex = para.ChildEntities.IndexOf((widget as SplitWidgetContainer).m_currentChild as IEntity);
                }
                else
                    para = (widget as WParagraph);
                //check whether the current child index greater than 0
                if (paraItemIndex >= 0)
                {
                    for (int i = paraItemIndex; i < para.ChildEntities.Count; i++)//Iterate the paragaph items
                    {
                        if (IsFloatingItem(para.ChildEntities[i]))
                        {
                            LayoutContext lc = LayoutContext.Create((para.ChildEntities[i] as IWidget), m_lcOperator, (m_lcOperator as Layouter).ClientLayoutArea.Width);
                            RectangleF rect = new RectangleF(m_layoutArea.ClientActiveArea.X, m_layoutArea.ClientActiveArea.Y, m_layoutArea.ClientActiveArea.Width, m_layoutArea.ClientActiveArea.Height);
                            para.IsFloatingItemsLayouted = true;
                            LayoutedWidget ltWidget = lc.Layout(rect);
                            if (ltWidget != null)
                            {
                                if (para.ChildEntities[i] is WTextBox)//add wrapping element bounds to the collection
                                {
                                    FloatingItem floatingItem = new FloatingItem();
                                    floatingItem.TextWrappingBounds = new RectangleF(ltWidget.Bounds.X, ltWidget.Bounds.Y, ltWidget.Bounds.Width, ltWidget.Bounds.Height);
                                    floatingItem.FloatingEntity = para.ChildEntities[i] as Entity;
                                    (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                                    (para.ChildEntities[i] as WTextBox).TextBoxFormat.IsWrappingBoundsAdded = true;
                                    (para.ChildEntities[i] as WTextBox).TextBoxFormat.WrapCollectionIndex = (m_lcOperator as Layouter).FloatingItems.Count - 1;
                                }
                                else if (para.ChildEntities[i] is WPicture)
                                {
                                    //Add floatting elements bounds values to collection
                                    WPicture picture = para.ChildEntities[i] as WPicture;
                                    FloatingItem floatingItem = new FloatingItem();
                                    floatingItem.TextWrappingBounds = new RectangleF(ltWidget.Bounds.X - (picture.DistanceFromLeft),
                                                                                    ltWidget.Bounds.Y - (picture.DistanceFromTop),
                                                                                    ltWidget.Bounds.Width + (picture.DistanceFromRight) + (picture.DistanceFromLeft),
                                                                                    ltWidget.Bounds.Height + (picture.DistanceFromBottom) + (picture.DistanceFromTop));
                                    floatingItem.FloatingEntity = para.ChildEntities[i] as Entity;
                                    (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                                    (para.ChildEntities[i] as WPicture).IsWrappingBoundsAdded = true;
                                    (para.ChildEntities[i] as WPicture).WrapCollectionIndex = (m_lcOperator as Layouter).FloatingItems.Count - 1;
                                }
                                else if (para.ChildEntities[i] is Shape)
                                {
                                    //Add floatting elements bounds values to collection
                                    Shape shape = para.ChildEntities[i] as Shape;
                                    FloatingItem floatingItem = new FloatingItem();
                                    floatingItem.TextWrappingBounds = new RectangleF(ltWidget.Bounds.X - (shape.WrapFormat.DistanceLeft),
                                                                                    ltWidget.Bounds.Y - (shape.WrapFormat.DistanceTop),
                                                                                    ltWidget.Bounds.Width + (shape.WrapFormat.DistanceRight) + (shape.WrapFormat.DistanceLeft),
                                                                                    ltWidget.Bounds.Height + (shape.WrapFormat.DistanceBottom) + (shape.WrapFormat.DistanceTop));
                                    floatingItem.FloatingEntity = para.ChildEntities[i] as Entity;
                                    (m_lcOperator as Layouter).FloatingItems.Add(floatingItem);
                                    (para.ChildEntities[i] as Shape).WrapFormat.IsWrappingBoundsAdded = true;
                                    (para.ChildEntities[i] as Shape).WrapFormat.WrapCollectionIndex = (m_lcOperator as Layouter).FloatingItems.Count - 1;
                                }
                                ltWidget.InitLayoutInfo();//Initialize the initlayoutinfo after add the wrapping bounds
                            }
                        }
                    }
                }
                float xPosition = (para.m_layoutInfo as ParagraphLayoutInfo).XPosition;
                if (para.IsFloatingItemsLayouted)
                {
                    UpdateParagraphXPositionBasedOnTextWrap(para, (para.m_layoutInfo as ParagraphLayoutInfo).XPosition, (para.m_layoutInfo as ParagraphLayoutInfo).YPosition);
                    //Check whteher xposition updated or not
                    if (xPosition != (para.m_layoutInfo as ParagraphLayoutInfo).XPosition)
                        para.IsXpositionUpated = true;
                }
            }
        }
        /// <summary>
        /// Update X position of the paragraph based on TextWrap
        /// </summary>
        /// <param name="childWidget"></param>
        internal void UpdateParagraphXPositionBasedOnTextWrap(WParagraph paragraph, float xPosition, float yPosition)
        {
            #region textwrap
            //Update Layout area based on text wrap
            if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter
                && !IsInFootnote(paragraph))
            {
                RectangleF clientLayoutArea = (m_lcOperator as Layouter).ClientLayoutArea;
                clientLayoutArea.X = xPosition;
                clientLayoutArea.Y = yPosition;
                SizeF size = (paragraph as IWidget).LayoutInfo.Size;
                for (int i = 0; i < (m_lcOperator as Layouter).FloatingItems.Count; i++)
                {
                    RectangleF textWrappingBounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                    TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                    TextWrappingType textWrappingType = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingType;
                    if (!(clientLayoutArea.X > textWrappingBounds.Right + DEF_MIN_WIDTH || clientLayoutArea.Right < textWrappingBounds.X - DEF_MIN_WIDTH))
                    {
                        if ((m_lcOperator as Layouter).FloatingItems.Count > 0 && ((clientLayoutArea.Y + size.Height > textWrappingBounds.Y && clientLayoutArea.Y < (textWrappingBounds.Bottom))) && textWrappingStyle != TextWrappingStyle.Inline && textWrappingStyle != TextWrappingStyle.TopAndBottom && textWrappingStyle != TextWrappingStyle.InFrontOfText && textWrappingStyle != TextWrappingStyle.Behind)
                        {
                            float rightIndent = (float)((paragraph as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Right;
                            rightIndent = rightIndent < 0 ? Math.Abs(rightIndent) : 0;
                            if (paragraph.ParagraphFormat.HorizontalAlignment != Syncfusion.DocIO.DLS.HorizontalAlignment.Left && (clientLayoutArea.X < textWrappingBounds.X && clientLayoutArea.X + size.Width > textWrappingBounds.X))
                            {
                                (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = clientLayoutArea.X;
                            }
                            else if (clientLayoutArea.X >= textWrappingBounds.X && clientLayoutArea.X < textWrappingBounds.Right)
                            {
                                clientLayoutArea.Width = clientLayoutArea.Width - (textWrappingBounds.Right - clientLayoutArea.X) - rightIndent;
                                //checks minimum width
                                if (clientLayoutArea.Width < DEF_MIN_WIDTH)
                                {
                                    clientLayoutArea.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right - rightIndent;
                                    if (clientLayoutArea.Width < DEF_MIN_WIDTH)
                                    {
                                        (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = clientLayoutArea.X;
                                    }
                                    else
                                    {
                                        (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = textWrappingBounds.Right;
                                    }
                                }
                                else
                                {
                                    (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = textWrappingBounds.Right;
                                }
                            }
                            else if ((textWrappingBounds.X - DEF_MIN_WIDTH > clientLayoutArea.X && clientLayoutArea.Right > textWrappingBounds.X)
                                     || (clientLayoutArea.X > textWrappingBounds.X && clientLayoutArea.X > textWrappingBounds.Right))
                            {
                                (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = clientLayoutArea.X;
                            }
                            else if (clientLayoutArea.X > textWrappingBounds.X - DEF_MIN_WIDTH && clientLayoutArea.X < textWrappingBounds.Right)
                            {
                                (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition = textWrappingBounds.Right;
                            }
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Update Paragraph Y position based on TextWrap
        /// </summary>
        private void UpdateParagraphYPositionBasedonTextWrap()
        {
            if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                && m_widget is WParagraph && !IsInFrame(m_widget as WParagraph) && !IsInFootnote(m_widget as WParagraph)
                && GetFloattingItemIndex(m_widget as WParagraph) == -1
                && !(m_widget as WParagraph).IsFloatingItemsLayouted)//Ignore the floating item bounds if already this added to the Collection.
            {
                string text = (m_widget as WParagraph).Text;
                SizeF size = m_widget.LayoutInfo.Size;
                #region textwrap
                //Update Layout area based on text wrap
                if (!(m_lcOperator as Layouter).IsLayoutingHeaderFooter)
                {
                    RectangleF clientLayoutArea = m_layoutArea.ClientActiveArea;
                    RectangleF rect = m_layoutArea.ClientActiveArea;
                    ParagraphLayoutInfo paraInfo = (LayoutInfo as ParagraphLayoutInfo);
                    for (int i = 0; i < (m_lcOperator as Layouter).FloatingItems.Count; i++)
                    {
                        RectangleF textWrappingBounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                        TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                        TextWrappingType textWrappingType = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingType;
                        if (!(clientLayoutArea.X > textWrappingBounds.Right + DEF_MIN_WIDTH || clientLayoutArea.Right < textWrappingBounds.X - DEF_MIN_WIDTH))
                        {
                            if ((m_lcOperator as Layouter).FloatingItems.Count > 0 && ((rect.Y + size.Height > textWrappingBounds.Y
                                && rect.Y < (textWrappingBounds.Bottom))) && textWrappingStyle != TextWrappingStyle.Inline
                                && textWrappingStyle != TextWrappingStyle.TopAndBottom && textWrappingStyle != TextWrappingStyle.InFrontOfText
                                && textWrappingStyle != TextWrappingStyle.Behind)
                            {
                                float rightIndent = (float)paraInfo.Margins.Right;
                                float leftIndent = (float)paraInfo.Margins.Left;
                                if (paraInfo.IsFirstLine)
                                    leftIndent = (float)paraInfo.Margins.Left + paraInfo.FirstLineIndent;
                                rightIndent = rightIndent < 0 ? Math.Abs(rightIndent) : 0;
                                if ((m_widget as WParagraph).ParagraphFormat.HorizontalAlignment != Syncfusion.DocIO.DLS.HorizontalAlignment.Left
                                    && (rect.X < textWrappingBounds.X && rect.X + size.Width > textWrappingBounds.X))
                                {
                                    if (rect.Right > textWrappingBounds.X)
                                        rect.Width = rect.Width - (rect.Right - textWrappingBounds.X);
                                    if (rect.Width < DEF_MIN_WIDTH)
                                    {
                                        m_layoutArea.UpdateBoundsBasedOnTextWrap(textWrappingBounds);
                                        paraInfo.YPosition = m_layoutArea.ClientActiveArea.Y;
                                    }
                                }
                                else if (rect.X >= textWrappingBounds.X && rect.X < textWrappingBounds.Right)
                                {
                                    rect.Width = rect.Width - (textWrappingBounds.Right - rect.X) - rightIndent;
                                    //checks minimum width
                                    if (rect.Width < DEF_MIN_WIDTH)
                                    {
                                        rect.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right - rightIndent;
                                        if (rect.Width < DEF_MIN_WIDTH)
                                        {
                                            m_layoutArea.UpdateBoundsBasedOnTextWrap(textWrappingBounds);
                                            paraInfo.YPosition = m_layoutArea.ClientActiveArea.Y;
                                        }
                                    }
                                }
                                else if (textWrappingBounds.X > rect.X && rect.Right > textWrappingBounds.X)
                                {
                                    rect.Width = textWrappingBounds.X - rect.X - rightIndent;
                                    //checks minimum width
                                    if (rect.Width < DEF_MIN_WIDTH)
                                    {
                                        rect.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right - rightIndent;
                                        if (rect.Width < DEF_MIN_WIDTH)
                                        {
                                            if (Math.Round(rect.X, 2) == Math.Round(GetPageMarginLeft(m_widget as WParagraph) + leftIndent, 2))
                                            {
                                                m_layoutArea.UpdateBoundsBasedOnTextWrap(textWrappingBounds);
                                                paraInfo.YPosition = m_layoutArea.ClientActiveArea.Y;
                                            }
                                        }
                                    }
                                }
                            }
                            else if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                                && ((rect.Y >= textWrappingBounds.Y
                                && rect.Y < (textWrappingBounds.Bottom))
                                || ((rect.Y + size.Height >= textWrappingBounds.Y)
                                && (rect.Y + size.Height < (textWrappingBounds.Bottom))))
                                && textWrappingStyle == TextWrappingStyle.TopAndBottom)
                            {
                                m_layoutArea.UpdateBoundsBasedOnTextWrap(textWrappingBounds);
                                paraInfo.YPosition = m_layoutArea.ClientActiveArea.Y;
                            }
                        }
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Determine whether the paragraph is in textbox and wrapping added to the collection or not
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal int GetFloattingItemIndex(Entity entity)
        {
            while (entity != null && entity.EntityType != EntityType.TextBox || !((entity is WTable) && (entity as WTable).m_isTextBox))
            {
                if (entity == null)
                    break;
                if (entity.EntityType == EntityType.TextBox || entity is Shape || ((entity is WTable) && (entity as WTable).m_isTextBox))
                    break;
                entity = entity.Owner;
            }
            if (entity != null && (entity is WTable) && (entity as WTable).m_isTextBox)
                return (entity as WTable).m_textBoxFormat.WrapCollectionIndex;
            else if (entity != null && entity is WTextBox)
                return (entity as WTextBox).TextBoxFormat.WrapCollectionIndex;
            else if (entity != null && entity is Shape)
                return (entity as Shape).WrapFormat.WrapCollectionIndex;
            return -1;
        }
        /// <summary>
        /// Determine whether the paraItem is in floating item or not.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal bool IsFloatingItem(Entity entity)
        {
            if ((entity is WPicture &&
                            ((entity as WPicture).TextWrappingStyle == TextWrappingStyle.Square//Check whether it is the floating element or not
                            || (entity as WPicture).TextWrappingStyle == TextWrappingStyle.Through
                            || (entity as WPicture).TextWrappingStyle == TextWrappingStyle.Tight
                            || (entity as WPicture).TextWrappingStyle == TextWrappingStyle.TopAndBottom))
                            || (entity is Shape
                            && ((entity as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Square//Check whether it is the floating element or not
                            || (entity as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Through
                            || (entity as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Tight
                            || (entity as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.TopAndBottom))
                            || (entity is WTextBox &&
                            ((entity as WTextBox).TextBoxFormat.TextWrappingStyle == TextWrappingStyle.Square//Check whether it is the floating element or not
                            || ((entity as WTextBox).TextBoxFormat.TextWrappingStyle == TextWrappingStyle.Through
                            || (entity as WTextBox).TextBoxFormat.TextWrappingStyle == TextWrappingStyle.Tight
                            || (entity as WTextBox).TextBoxFormat.TextWrappingStyle == TextWrappingStyle.TopAndBottom))))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Determine whether the paragraph is in footnote
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal bool IsInFootnote(WParagraph paragraph)
        {
            Entity ent= paragraph.Owner as Entity;
            while (!(ent is WFootnote))
            {
                if (ent == null)
                    break;
                else
                    ent = ent.Owner;
            }
            if (ent is WFootnote)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Get Left Margin of the Page
        /// </summary>
        /// <returns></returns>
        internal float GetPageMarginLeft(WParagraph paragraph)
        {
            float pageMarginLeft = (m_lcOperator as Layouter).ClientLayoutArea.Left;
            if (paragraph != null && paragraph.IsInCell)
            {
                //TableCellLeftMargin value is assigned when the Tab is inside Table cell.
                pageMarginLeft = ((paragraph.Owner as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
            }
            return pageMarginLeft;
        }
        /// <summary>
        /// Update Top Margin of the paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        private void UpdateParagraphTopMargin(WParagraph paragraph)
        {
            Borders borders = paragraph.ParagraphFormat.Borders;
            if (!borders.NoBorder && borders.Top.BorderType != BorderStyle.None && LayoutInfo.Paddings.Top == 0)
            {
                LayoutInfo.Paddings.Top = borders.Top.Space;
            }
            if (LayoutInfo.Margins.Top == 0)
            {
                if (paragraph.ParagraphFormat.SpaceBeforeAuto && !paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing)
                {
                    if ((LayoutInfo as ParagraphLayoutInfo).ListValue != string.Empty || paragraph.IsFirstParagraphOfDocument())
                        LayoutInfo.Margins.Top = 0;
                    else
                        LayoutInfo.Margins.Top = 14;
                }
                else
                    LayoutInfo.Margins.Top = paragraph.ParagraphFormat.BeforeSpacing;
            }
            if (!paragraph.IsFirstParagraphOfDocument())
            {
                if (!(paragraph.OwnerTextBody.Owner is SDTBlockContent)
                    && (IsPageBreak(m_widget) || IsSectionBreak(paragraph) || paragraph.ParagraphFormat.PageBreakBefore
                    || (paragraph.PreviousSibling != null && (paragraph.PreviousSibling is WParagraph)
                    && (paragraph.PreviousSibling as WParagraph).ParagraphFormat.PageBreakAfter)
                    || IsTOC(paragraph)))
                {
                    if (paragraph.ParagraphFormat.SpaceBeforeAuto && !paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing)
                    {
                        LayoutInfo.Margins.Top = 14;
                    }
                    else
                        LayoutInfo.Margins.Top = paragraph.ParagraphFormat.BeforeSpacing;
                }
                else
                    LayoutInfo.Margins.Top = 0;
            }
        }
        /// <summary>
        /// Checks whether the paragraph is a TOC
        /// </summary>
        /// <param name="txtRange">Text Range</param>
        /// <returns></returns>
        private bool IsTOC(WParagraph paragraph)
        {
            Hyperlink hyperlink;
            if (paragraph.ChildEntities.FirstItem != null && ((paragraph.ChildEntities.FirstItem is TableOfContent)
                || ((paragraph.ChildEntities.FirstItem is WField)
                && (paragraph.ChildEntities.FirstItem as WField).FieldType == FieldType.FieldHyperlink)
                && ((hyperlink = new Hyperlink(paragraph.ChildEntities.FirstItem as WField)).BookmarkName != null
                && hyperlink.BookmarkName.StartsWith("_Toc"))))
            {
                if ((IsPageBreak(m_widget) || (paragraph.ParagraphFormat.PageBreakBefore)
                            || (paragraph.PreviousSibling != null && (paragraph.PreviousSibling is WParagraph)
                            && (paragraph.PreviousSibling as WParagraph).ParagraphFormat.PageBreakAfter)))
                    return true;
                else
                    return false;
            }
            return false;
        }
        /// <summary>
        /// Check whether the current widget is have page break
        /// </summary>
        /// <param name="childWidget"></param>
        /// <returns></returns>
        private bool IsPageBreak(IWidget childWidget)
        {
            if ((childWidget is SplitWidgetContainer) && (childWidget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
            {
                WParagraph para = (childWidget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
                int count = para.ChildEntities.Count;
                if (count > (childWidget as SplitWidgetContainer).Count && para.ChildEntities[count - 1 - (childWidget as SplitWidgetContainer).Count] is Break)
                {
                    Break br = ((Entity)para.ChildEntities[count - 1 - (childWidget as SplitWidgetContainer).Count]) as Break;
                    return (br.BreakType == BreakType.PageBreak || br.BreakType == BreakType.ColumnBreak);
                }
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Determine the whether the paragraph is have section break
        /// </summary>
        private bool IsSectionBreak(WParagraph para)
        {
            if ((para.Owner as WTextBody) != null && !para.IsInCell)
            {
                bool isFirstParagraph = (para.Owner as WTextBody).Items[0] == para;
                IWSection currentSection = GetBaseEntity(para) as WSection;
                return (isFirstParagraph && currentSection != null
                         && (currentSection.BreakCode == SectionBreakCode.NewPage
                         || currentSection.BreakCode == SectionBreakCode.Oddpage
                         || currentSection.BreakCode == SectionBreakCode.EvenPage));
            }
            else
                return false;
        }
        /// <summary>
        /// Creates the layout area.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="cellPadding">The cell padding.</param>
        protected void CreateLayoutArea(RectangleF rect, Paddings cellPadding)
        {
            LayoutInfo.Paddings.Left = cellPadding.Left;
            LayoutInfo.Paddings.Right = cellPadding.Right;
            LayoutInfo.Paddings.Top = cellPadding.Top;
            LayoutInfo.Paddings.Bottom = cellPadding.Bottom;

            m_layoutArea = new LayoutArea(rect, LayoutInfo, m_widget);
        }

        /// <summary>
        /// Creates the layouted widget.
        /// </summary>
        /// <param name="location">The location.</param>
        protected void CreateLayoutedWidget(PointF location)
        {
            m_ltWidget = new LayoutedWidget(m_widget);
            RectangleF bounds = m_ltWidget.Bounds;

            location.X += (float)LayoutInfo.Margins.Left;
            location.Y += (float)LayoutInfo.Margins.Top;
            bounds.Location = location;

            m_ltWidget.Bounds = bounds;
        }
        /// <summary>
        /// Updates the width of client area and client active area
        /// </summary>
        /// <param name="width">width.</param>
        /// <returns></returns>
        protected void UpdateAreaWidth()
        {
            m_layoutArea.UpdateWidth();
        }
        #endregion

        #region Utility methods
        /// <summary>
        /// Creates the specified widget.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="lcOperator">The lc operator.</param>
        /// <param name="width">The width of current lay out area.</param>
        /// <returns></returns>
        public static LayoutContext Create(IWidget widget, ILCOperator lcOperator, float width)
        {
            // Test widget as IWidgetContainer
            IWidgetContainer wtContainer = widget as IWidgetContainer;
            if (wtContainer != null)
            {
                if (wtContainer.LayoutInfo.IsLineContainer)
                {
                    return new LCLineContainer(wtContainer, lcOperator);
                }

                return new LCContainer(wtContainer, lcOperator);
            }

            // Test widget as ILeafWidget
            ILeafWidget leafWidget = widget as ILeafWidget;
            if (widget is WField
                && (widget as WField).FieldType == FieldType.FieldSymbol)
            {
                leafWidget = (widget as WField).GetAsSymbol() as ILeafWidget;
            }
            if (leafWidget != null)
            {
                return new LeafLayoutContext(leafWidget, lcOperator);
            }

            // Test widget as ITableWidget 
            ITableWidget table = null;

            if (widget is ITableWidget)
                table = widget as ITableWidget;
            else if (widget is WTextBox)
                table = (widget as WTextBox).GetAsTable((lcOperator as Layouter).CurrPageIndex);

            if (table != null)
            {
                return new LCTable(table, lcOperator);
            }

            // Test widget as SplitTableWidget
            SplitTableWidget spltTable = widget as SplitTableWidget;
            if (spltTable != null)
            {
                return new LCTable(spltTable, lcOperator);
            }

            throw new ArgumentException("Invalid widget type: " + widget.GetType());
        }
        #endregion

        #region Implementation Frame position.
        /// <summary>
        /// Determines whether the specified paragraph is in frame.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns>
        /// 	<c>true</c> if [is in frame] [the specified paragraph]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsInFrame(WParagraph paragraph)
        {;
            if (paragraph != null
               && paragraph.ParagraphFormat.IsFrame
               && !paragraph.IsInCell)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the Location of frame and draw the frame content.
        /// </summary>
        /// <param name="dc">dc.</param>
        /// <param name="ltWidget">ltWidget.</param>
        /// <returns></returns>
        internal RectangleF GetFrameBounds(WParagraph paragraph, RectangleF bounds)
        {
            WSection section = GetBaseEntity(paragraph) as WSection;
            WParagraphFormat paraFormat = paragraph.ParagraphFormat;
            float posX = bounds.X;
            float posY = bounds.Y;
            float frameWidth = bounds.Width;
            if (paraFormat.FrameWidth != 0)
                frameWidth = paraFormat.FrameWidth;
            float height = bounds.Height;
            Entity ent = GetBaseTextBody(paragraph);
            if (ent is HeaderFooter)
                height = (ent.Owner as WSection).PageSetup.PageSize.Height;
            float frameHeight = 0;
            bool isAtleastHeight = false;
            if (paraFormat.FrameHeight != 0)
            {
                ushort heightValue = (ushort)(paraFormat.FrameHeight * DLSConstants.TwipsInOnePoint);
                isAtleastHeight = (heightValue & (1 << 15)) != 0;
                frameHeight = ((heightValue & ((1 << 15) - 1)) / DLSConstants.TwipsInOnePoint);
            }
            if (!isAtleastHeight && paraFormat.FrameHeight != 0)
                height = frameHeight;
            if (section != null)
            {
                posX = GetPositionX(paraFormat, section, bounds, frameWidth);
                posY = GetPositionY(paraFormat, section, bounds, height);
            }
            if (!(paragraph.IsInCell && paraFormat.FrameWidth == 0))
                bounds = new RectangleF(posX, posY, frameWidth, height);

            if (paraFormat.IsPreviousParagraphInSameFrame())
                return (m_lcOperator as Layouter).FrameLayoutArea;

            (m_lcOperator as Layouter).FrameLayoutArea = bounds;
            if (isAtleastHeight)
                (m_lcOperator as Layouter).FrameHeight = frameHeight;
            return bounds;
        }
        /// <summary>
        /// Get Base Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private Entity GetBaseTextBody(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity.Owner == null)
                    return baseEntity;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WSection || baseEntity is HeaderFooter));

            return baseEntity;
        }
        /// <summary>
        /// Get Base Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        internal Entity GetBaseEntity(Entity entity)
        {
            Entity ent = entity;
            while (!(ent is WSection))
            {
                if ((ent is WTable)
                    && (ent as WTable).m_isTextBox
                    && (ent as WTable).m_textBoxFormat.OwnerBase is WTextBox)
                    ent = (ent as WTable).m_textBoxFormat.OwnerBase as Entity;
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            return ent;
        }
        /// <summary>
        /// Positions the X.
        /// </summary>
        /// <param name="paraFormat">The para format.</param>
        /// <param name="section">The section.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="frameWidth">Width of the frame.</param>
        /// <returns></returns>
        private float GetPositionX(WParagraphFormat paraFormat, WSection section, RectangleF bounds, float frameWidth)
        {
            float posX = 0;
            switch ((short)paraFormat.FrameX)
            {
                case (short)PageNumberAlignment.Left:
                case (short)PageNumberAlignment.Inside:
                    switch (paraFormat.FrameHorizontalPos)
                    {
                        case (byte)FrameHorzAnchor.Text:
                            posX = (m_lcOperator as Layouter).ClientLayoutArea.X;
                            break;
                        case (byte)FrameHorzAnchor.Margin:
                            posX = section.PageSetup.Margins.Left;
                            break;
                        case (byte)FrameHorzAnchor.Page:
                            posX = 0;
                            break;
                    }
                    break;
                case (short)PageNumberAlignment.Center:
                    switch (paraFormat.FrameHorizontalPos)
                    {
                        case (byte)FrameHorzAnchor.Text:
                            posX = (m_lcOperator as Layouter).ClientLayoutArea.Left + ((m_lcOperator as Layouter).ClientLayoutArea.Width) / 2;
                            break;
                        case (byte)FrameHorzAnchor.Margin:
                            posX = section.PageSetup.Margins.Left + (section.PageSetup.PageSize.Width - section.PageSetup.Margins.Right - section.PageSetup.Margins.Left) / 2;
                            break;
                        case (byte)FrameHorzAnchor.Page:
                            posX = (section.PageSetup.PageSize.Width) / 2;
                            break;
                    }
                    break;
                case (short)PageNumberAlignment.Outside:
                case (short)PageNumberAlignment.Right:
                    switch (paraFormat.FrameHorizontalPos)
                    {
                        case (byte)FrameHorzAnchor.Text:
                            posX = (m_lcOperator as Layouter).ClientLayoutArea.Width + (m_lcOperator as Layouter).ClientLayoutArea.Left;
                            break;
                        case (byte)FrameHorzAnchor.Margin:
                            posX = section.PageSetup.PageSize.Width - section.PageSetup.Margins.Right;
                            break;
                        case (byte)FrameHorzAnchor.Page:
                            posX = section.PageSetup.PageSize.Width;
                            break;
                    }
                    break;
                default:
                    switch (paraFormat.FrameHorizontalPos)
                    {
                        case (byte)FrameHorzAnchor.Text:
                            posX = (m_lcOperator as Layouter).ClientLayoutArea.X + paraFormat.FrameX;
                            break;
                        case (byte)FrameHorzAnchor.Margin:
                            posX = section.PageSetup.Margins.Left + paraFormat.FrameX;
                            break;
                        case (byte)FrameHorzAnchor.Page:
                            posX = paraFormat.FrameX;
                            break;
                    }
                    break;
            }
            return posX;
        }
        /// <summary>
        /// Gets the Y Position of frame.
        /// </summary>
        /// <param name="paraFormat">paraformat.</param>
        /// <param name="section">section.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="frameHeight">Height of the frame.</param>
        /// <returns></returns>
        /// ///
        private float GetPositionY(WParagraphFormat paraFormat, WSection section, RectangleF bounds, float frameHeight)
        {
            float posY = 0;
            float pageTop = section.PageSetup.Margins.Top;
            float pageBottom = section.PageSetup.Margins.Bottom;
            float pageClientHeight = section.PageSetup.PageSize.Height - pageTop - pageBottom;
            switch ((short)paraFormat.FrameY)
            {
                case (short)FrameVerticalPosition.Top:
                case (short)FrameVerticalPosition.Inside:
                    switch (paraFormat.FrameVerticalPos)
                    {
                        case (byte)FrameVertAnchor.Margin:
                            posY = pageTop;
                            break;
                        case (byte)FrameVertAnchor.Page:
                            posY = 0;
                            break;
                    }
                    break;
                case (short)FrameVerticalPosition.Center:
                    switch (paraFormat.FrameVerticalPos)
                    {
                        case (byte)FrameVertAnchor.Margin:
                            posY = pageTop + pageClientHeight / 2;
                            break;
                        case (byte)FrameVertAnchor.Page:
                            posY = section.PageSetup.PageSize.Height / 2;
                            break;
                    }
                    break;
                case (short)FrameVerticalPosition.Outside:
                case (short)FrameVerticalPosition.Bottom:
                    switch (paraFormat.FrameVerticalPos)
                    {
                        case (byte)FrameVertAnchor.Margin:
                            posY = pageTop + pageClientHeight;
                            break;
                        case (byte)FrameVertAnchor.Page:
                            posY = section.PageSetup.PageSize.Height;
                            break;
                    }
                    break;
                default:
                    switch (paraFormat.FrameVerticalPos)
                    {
                        case (byte)FrameVertAnchor.Margin:
                            posY = pageTop + paraFormat.FrameY;
                            break;
                        case (byte)FrameVertAnchor.Page:
                            posY = paraFormat.FrameY;
                            break;
                        case (byte)FrameVertAnchor.Text:
                            posY = bounds.Y + paraFormat.FrameY;
                            break;
                    }
                    break;
            }
            return posY;
        }
        #endregion

        #region DEBUG
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected internal void DBG_CreateNextChildContext( LayoutContext childContext )
    {
#if DEBUG_LAYOUTING
      if( childContext != null )
      {
        DBG_WriteIn( DBG_GetContextInfo( childContext ), "<Create>"  );
      }
      DBG_RightIndent();
#endif
    }
    protected internal void DBG_RightIndent()
    {
#if DEBUG_LAYOUTING
       m_DBGIdentSpace += "  ";
#endif
    }
    protected internal void DBG_LeftIndent()
    {
#if DEBUG_LAYOUTING
      m_DBGIdentSpace = m_DBGIdentSpace.Substring( 0, m_DBGIdentSpace.Length - 2 );
#endif
    }
    protected internal void DBG_WriteSpec( string text, string category )
    {
#if DEBUG_LAYOUTING
      Trace.WriteLine( text + " ---------------------------------------", category );
#endif
    }
    protected internal void DBG_WriteIn( string text, string category )
    {
#if DEBUG_LAYOUTING
      Trace.WriteLine( text,
        m_DBGIdentSpace +
        ( m_DBGIdentSpace.Length / 2 ).ToString( "##00" ) + category
      );
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="childContext"></param>
    protected internal void DBG_CommitChildContext( LayoutContext childContext )
    {
      DBG_UpdateLA( childContext );

#if DEBUG_LAYOUTING
      DBG_LeftIndent();

      if( childContext != null )
      {
        DBG_WriteIn( DBG_GetContextInfo( childContext ), "<Commit>" );
      }
#endif
    }
    /// <summary>
    /// DBs the g_ update LA.
    /// </summary>
    /// <param name="context">The context.</param>
    protected internal void DBG_UpdateLA( LayoutContext context )
    {
#if DEBUG_LAYOUTING
      if( context != null )
      {
        RectangleF ca = context.LayoutArea.ClientActiveArea;
        DBG_WriteIn(
          string.Format( "{{X : {0}, Y : {1}, Width: {02}, Height: {3}}}",ca.X, ca.Y, ca.Width, ca.Height ), 
          "<ClientLA>" );
      }
#endif
    }
    /// <summary>
    /// /
    /// </summary>
    /// <param name="childContext"></param>
    /// <returns></returns>
    private string DBG_GetContextInfo( LayoutContext childContext )
    {
#if DEBUG_LAYOUTING
      string strState = ( childContext.State == LayoutState.Unknown )
        ? ""
        : "[" + childContext.State.ToString() + "]";
      string strType = "";

      if (childContext.GetType() == typeof(LeafLayoutContext))
      {
        strType = "Leaf";
      }
      else if( childContext.GetType() == typeof( LCLineContainer ) )
      {
        strType = "LineContainer";
      }
      else if( childContext.GetType() == typeof( LCContainer ) )
      {
        strType = "Container";
      }
      else if( childContext.GetType() == typeof( LCTable ) )
      {
        strType = "Table";
      }
      else
      {
        strType = "UnknownContext";
      }

      Type type = childContext.Widget.GetType();
      string typeName = type.Name;
      if(typeName == "SplitWidgetContainer")
      {
        typeName = "SWC-" + (childContext.Widget as SplitWidgetContainer).RealWidgetContainer.GetType().Name;
      }
      return strState + strType + "(" + typeName + ")";
        //childContext.DBG_ChildIndex.ToString() + " / " +
        //childContext.DBG_ChildMax.ToString();

#else
      return string.Empty;
#endif
    }
#endif
        #endregion
    }
}

#endif