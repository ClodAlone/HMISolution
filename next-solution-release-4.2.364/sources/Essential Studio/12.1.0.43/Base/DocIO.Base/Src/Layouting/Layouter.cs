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

#region file using directives
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
#endregion

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Class represents the layouter.
    /// </summary>
    internal class Layouter : ILCOperator
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private DrawingContext m_drawingContext;
        /// <summary>
        /// Client LayoutArea
        /// </summary>
        private RectangleF m_clientLayoutArea;
        /// <summary>
        /// Holds the absolute table height
        /// </summary>
        private float m_FloatingTableBottom = float.MinValue;
        /// Page Top margin of the client area
        /// </summary>
        private float m_pageTop;
        /// <summary>
        /// Is Layouting Header Footer
        /// </summary>
        private bool m_isLayoutingHeaderFooter;
        private int m_currPageIndex;
        private List<SplitWidgetContainer> m_footnoteSplittedWidgets = new List<SplitWidgetContainer>();
        private List<SplitWidgetContainer> m_endnoteSplittedWidgets = null;
        private List<Entity> m_endnotesInstance = null;
        private TabsLayoutInfo.LayoutTab m_previousTab;
        private float m_previousTabWidth = 0f;
        private RectangleF m_frameLayoutArea;
        /// hold the wrapping difference value.
        ///</summary>
        private float m_wrappingDifference;
        /// <summary>
        private float m_frameHeight;
        /// <summary>
        /// Is Footnote need to be restart
        /// </summary>
        internal bool m_bisNeedToRestartFootnote = true;
        /// <summary>
        /// Is Endnote need to be restart
        /// </summary>
        internal bool m_bisNeedToRestartEndnote = true;
        /// <summary>
        /// Is Footnote ID need to be resatrt each page
        /// </summary>
        internal bool m_bisNeedToRestartFootnoteID = true;
         /// </summary>
        /// </summary>
        /// Is Endnote ID need to be resatrt each page
        /// </summary>
        internal bool m_bisNeedToRestartEndnoteID = true;
        #endregion

        #region Events
        /// <summary>
        /// 
        /// </summary>
        public delegate void LeafLayoutEventHandler(object sender, LayoutedWidget ltWidget);

        /// <summary>
        /// 
        /// </summary>
        public event LeafLayoutEventHandler LeafLayoutAfter;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the current page index
        /// </summary>
        internal int CurrPageIndex
        {
            get
            {
                return m_currPageIndex;
            }
        }
        /// <summary>
        /// Gets or sets the custom graphics.
        /// </summary>
        /// <value>The custom graphics.</value>
        public DrawingContext DrawingContext
        {
            get
            {
                return m_drawingContext;
            }
        }
        /// <summary>
        /// Gets the Client LayoutArea.
        /// </summary>
        /// <value>The ClientLayoutArea.</value>
        internal RectangleF ClientLayoutArea
        {
            get
            {
                return m_clientLayoutArea;
            }
        }
        /// <summary>
        /// Get and set the absolute table height 
        /// </summary>
        internal float FloatingTableBottom
        {
            get
            {
                return m_FloatingTableBottom;
            }
            set
            {
                m_FloatingTableBottom = value;
            }
        }
        /// <summary>
        /// Gets the Top margin of the Page
        /// </summary>
        /// <value>The PageTopMargin.<value>
        internal float PageTopMargin
        {
            get
            {
                return m_pageTop;
            }
        }
        /// <summary>
        /// Gets the frame layout area.
        /// </summary>
        /// <value>The frame layout area.</value>
        internal RectangleF FrameLayoutArea
        {
            get
            {
                if (m_frameLayoutArea == null)
                    m_frameLayoutArea = new RectangleF();
                return m_frameLayoutArea;
            }
            set
            {
                m_frameLayoutArea = value;
            }
        }
        /// <summary>
        /// Gets the frame Height with Height type as Atleast
        /// </summary>
        /// <value>The frame height</value>
        internal float FrameHeight
        {
            get
            {
                return m_frameHeight;
            }
            set
            {
                m_frameHeight = value;
            }
        }
        /// <summary>
        /// Gets the IsLayoutingHeaderFooter.
        /// </summary>
        /// <value>The IsLayoutingHeaderFooter.</value>
        internal bool IsLayoutingHeaderFooter
        {
            get
            {
                return m_isLayoutingHeaderFooter;
            }
        }
        /// <summary>
        /// Get or set the wrapping difference.
        /// </summary>
        /// <value>The widget container.</value>
        internal float WrappingDifference
        {
            get
            {
                return m_wrappingDifference;
            }
            set
            {
                m_wrappingDifference = value;
            }
        }
        /// <summary>
        /// Gets the TOC levels.
        /// </summary>
        /// <value>The TOC levels.</value>
        internal Dictionary<int, List<string>> TOCLevels
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).TOCLevels;
            }
        }
        /// <summary>
        /// Gets a value indicating whether updating TOC.
        /// </summary>
        /// <value><c>true</c> if updating TOC; otherwise, <c>false</c>.</value>
        internal bool UpdatingTOC
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).m_UpdatingToc;
            }
        }
        /// <summary>
        /// Gets a value indicating whether updating Page fields.
        /// </summary>
        /// <value><c>true</c> if updating Page fields; otherwise, <c>false</c>.</value>
        internal bool UpdatingPageFields
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).m_UpdatingPageFields;
            }
        }
        /// <summary>
        /// Gets the Footnote widgets.
        /// </summary>
        internal LayoutedWidgetList FootnoteWidgets
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).CurrentPage.FootnoteWidgets;
            }
        }
        /// <summary>
        /// Gets the Endnote widgets.
        /// </summary>
        internal LayoutedWidgetList EndnoteWidgets
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).CurrentPage.EndnoteWidgets;
            }
        }
        /// <summary>
        /// Gets the Endnote section index.
        /// </summary>
        internal List<int> EndNoteSectionIndex
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).CurrentPage.EndNoteSectionIndex;
            }
        }
        /// <summary>
        /// Gets the Footnote section index.
        /// </summary>
        internal List<int> FootNoteSectionIndex
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).CurrentPage.FootNoteSectionIndex;
            }
        }
        /// <summary>
        /// Get the Splitted Footnote widgets
        /// </summary>
        internal List<SplitWidgetContainer> FootnoteSplittedWidgets
        {
            get
            {
                if (m_footnoteSplittedWidgets == null)
                    m_footnoteSplittedWidgets = new List<SplitWidgetContainer>();
                return m_footnoteSplittedWidgets;
            }
            set
            {
                m_footnoteSplittedWidgets = value;
            }
        }
        /// <summary>
        /// Get or set the Splitted Endnote widgets
        /// </summary>
        internal List<SplitWidgetContainer> EndnoteSplittedWidgets
        {
            get
            {
                if (m_endnoteSplittedWidgets == null)
                    m_endnoteSplittedWidgets = new List<SplitWidgetContainer>();
                return m_endnoteSplittedWidgets;
            }
            set
            {
                m_endnoteSplittedWidgets = value;
            }
        }
        /// <summary>
        /// Get or set the Endnote instances
        /// </summary>
        internal List<Entity> EndnotesInstances
        {
            get
            {
                if (m_endnotesInstance == null)
                    m_endnotesInstance = new List<Entity>();
                return m_endnotesInstance;
            }
            set
            {
                m_endnotesInstance = value;
            }
        }
        /// <summary>
        /// Gets the wrapping entity collection.
        /// </summary>
        internal List<FloatingItem> FloatingItems
        {
            get
            {
                return (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).m_FloatingItems;
            }
        }
        /// <summary>
        /// Gets or sets the previous tab.
        /// </summary>
        /// <value>The previous tab.</value>
        internal TabsLayoutInfo.LayoutTab PreviousTab
        {
            get
            {
                if (m_previousTab == null)
                    m_previousTab = new TabsLayoutInfo.LayoutTab();
                return m_previousTab;
            }
            set
            {
                m_previousTab = value;
            }
        }
        /// <summary>
        /// Gets or sets the width of the previous tab.
        /// </summary>
        /// <value>The width of the previous tab.</value>
        internal float PreviousTabWidth
        {
            get
            {
                return m_previousTabWidth;
            }
            set
            {
                m_previousTabWidth = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Layout specified widget container.
        /// <remarks>Method use ILayoutProcessHandler for control layout process</remarks>
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="dc">The dc.</param>
        public void Layout(IWidgetContainer widget, ILayoutProcessHandler handler, DrawingContext dc, int currPageIndex)
        {
            m_isLayoutingHeaderFooter = (widget is Syncfusion.DocIO.DLS.HeaderFooter);
            m_currPageIndex = currPageIndex;
            RectangleF area;
            bool isContinuousSection = false;
            bool isSplittedWidget = true;
            IWidgetContainer currWidget = widget;
            List<IWidgetContainer> prevWidgets = new List<IWidgetContainer>();
            List<LayoutedWidget> prevLayoutedtWidgets = new List<LayoutedWidget>();
            bool isCurrentWidgetNeedToLayout = false;
            int columnIndex = 0;
            m_drawingContext = dc;
            #region #dbg
#if DEBUG_LAYOUTING 
      /* DEBUG code */ Trace.WriteLine( "<Layouting-Start>");
#endif
            #endregion

            // Main layout cycle
            while (handler.GetNextArea(out area,ref columnIndex, ref isContinuousSection, isSplittedWidget,ref m_pageTop))
            {
                if (area.Equals(RectangleF.Empty))
                    break;
                //Sets the current client layout area
                m_clientLayoutArea = area;
                //clear the wrapping difference.
                m_wrappingDifference = float.MinValue;
                #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ DBG_NextLayoutingArea( area );
#endif
                #endregion
                m_bisNeedToRestartFootnote = true;
                m_bisNeedToRestartFootnoteID = true;
                m_bisNeedToRestartEndnote = true;
                m_bisNeedToRestartEndnoteID = true;
                if (columnIndex == 0)
                {
                    FootnoteWidgets.Clear();
                    FootNoteSectionIndex.Clear();
                }
                LayoutContext lc = LayoutContext.Create(currWidget, this, area.Width);
                //Sets the client layout area right margin
                lc.ClientLayoutAreaRight = area.Width;
                LayoutedWidget ltWidget = lc.Layout(area);
                if (FootnoteSplittedWidgets.Count == 0)
                    m_bisNeedToRestartFootnoteID = true;
                if (EndnoteSplittedWidgets.Count == 0)
                    m_bisNeedToRestartEndnoteID = true;
                IsCurrentWidgetNeedToLayout(lc, columnIndex, ref isSplittedWidget, ref isCurrentWidgetNeedToLayout, isContinuousSection);
                if (isCurrentWidgetNeedToLayout)
                    continue;
                if (!isContinuousSection)
                {
                    //Clear all previous layouted widgets if not continuous section
                    prevWidgets.Clear();
                    prevLayoutedtWidgets.Clear();
                    handler.PushLayoutedWidget(ltWidget, m_clientLayoutArea, m_bisNeedToRestartFootnoteID, m_bisNeedToRestartEndnoteID);
                }
                else
                {
                    //Insert the layouted widgets of continuous section
                    prevWidgets.Insert(prevWidgets.Count, currWidget);
                    prevLayoutedtWidgets.Insert(prevLayoutedtWidgets.Count, ltWidget);
                }

                #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ Trace.WriteLine( "<Push-LayoutedWidget> " + ltWidget.Bounds.ToString() );
#endif
                #endregion

                if ((lc.IsEnsureSplitted()
                    || (lc.State == LayoutState.NotFitted
                    && lc.SplittedWidget != null
                    && (lc.SplittedWidget is SplitWidgetContainer)
                    && ((lc.SplittedWidget as SplitWidgetContainer)[0] is Syncfusion.DocIO.DLS.WSection)))
                    && !m_isLayoutingHeaderFooter)
                {
                    //If section with NoBreak not fits in the remaining client area, then create new page.
                    if (lc.State == LayoutState.NotFitted && lc.SplittedWidget != null)
                        (this.LeafLayoutAfter.Target as Syncfusion.DocIO.DLS.Rendering.DocumentLayouter).IsCreateNewPage = true;

                    SplitWidgetContainer splittedWC = lc.SplittedWidget as SplitWidgetContainer;
                    bool isLayoutedWidgetNeedToPushed = isContinuousSection;
                    bool bContinue = handler.HandleSplittedWidget(splittedWC, lc.State, ltWidget, ref isLayoutedWidgetNeedToPushed);

                    if (bContinue)
                    {
                        #region #dbg
#if DEBUG_LAYOUTING
        /* DEBUG code */ Trace.WriteLine( "<Handle-Splitted> " + lc.State.ToString() );
#endif
                        #endregion
                        if (isLayoutedWidgetNeedToPushed && isContinuousSection)
                        {
                            //Push all the previous layouted widgets of continuous section
                            int count = prevLayoutedtWidgets.Count;
                            for (int i = 0; i < count; i++)
                            {
                                handler.PushLayoutedWidget(prevLayoutedtWidgets[0], m_clientLayoutArea, m_bisNeedToRestartFootnoteID, m_bisNeedToRestartEndnoteID);
                                prevWidgets.RemoveAt(0);
                                prevLayoutedtWidgets.RemoveAt(0);
                            }
                        }
                        //Updates the layouted widget of continuous section
                        else if (isContinuousSection)
                            handler.HandleLayoutedWidget(ltWidget);
                        currWidget = splittedWC;
                        continue;
                    }
                    else
                    {
                        //Updates the layouted widget of continuous section
                        handler.HandleLayoutedWidget(ltWidget);
                        currWidget = prevWidgets[0];
                        continue;
                    }
                }

                // If current widget not splitted - break main cycle
                break;
            } // End of main layout cycle
        }
        /// <summary>
        /// check whether the current widget is need to be layout
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="isSplittedWidget"></param>
        /// <param name="isCurrentWidgetNeedToLayout"></param>
        /// <param name="isContinuousSection"></param>
        private void IsCurrentWidgetNeedToLayout(LayoutContext lc, int columnIndex, ref bool isSplittedWidget, ref bool isCurrentWidgetNeedToLayout, bool isContinuousSection)
        {
            if (lc.IsEnsureSplitted()
                    && !isCurrentWidgetNeedToLayout
                    && isContinuousSection
                    && columnIndex == 0
                    && lc.SplittedWidget is SplitWidgetContainer
                    && !((lc.SplittedWidget as SplitWidgetContainer)[0] is WSection)
                    && (lc.SplittedWidget as SplitWidgetContainer)[0] is SplitWidgetContainer
                    && ((lc.SplittedWidget as SplitWidgetContainer)[0] as SplitWidgetContainer).RealWidgetContainer is WSection)
            {
                WSection section = ((lc.SplittedWidget as SplitWidgetContainer)[0] as SplitWidgetContainer).RealWidgetContainer as WSection;
                if (section.Columns.Count > 1)
                {
                    float colWidth = section.Columns[0].Width;
                    bool isEqualColumnWidth = true;
                    //Checks whether columns have equal width or not
                    foreach (Column column in section.Columns)
                    {
                        if (colWidth != column.Width)
                        {
                            isEqualColumnWidth = false;
                            break;
                        }
                        colWidth = column.Width;
                    }
                    if (!(isEqualColumnWidth || section.PageSetup.EqualColumnWidth))
                    {
                        isSplittedWidget = false;
                        isCurrentWidgetNeedToLayout = true;
                    }
                }
            }
            else
            {
                isSplittedWidget = true;
                isCurrentWidgetNeedToLayout = false;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sends the event that current leaf widget lay outed complete.
        /// </summary>
        /// <param name="ltWidget">The widget.</param>
        void ILCOperator.SendLeafLayoutAfter(LayoutedWidget ltWidget)
        {
            if (LeafLayoutAfter != null)
            {
                LeafLayoutAfter(this, ltWidget);
            }
        }
        #endregion

        #region DEBUG
#if DEBUG_LAYOUTING
    /// <summary>
    /// 
    /// </summary>
    /// <param name="area"></param>
    private void DBG_NextLayoutingArea( RectangleF area )
    {
      Trace.WriteLine( "<Next-Area>" + area.ToString() );
    }
#endif
        #endregion
    }
    /// <summary>
    /// Class to hold the floating items. 
    /// </summary>
    internal class FloatingItem
    {
        #region Fields
        private RectangleF m_textWrappingBounds = new RectangleF();
        private Entity m_FloatingEntity = null;
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set the wrapping bounds.
        /// </summary>
        /// <value></value>
        internal RectangleF TextWrappingBounds
        {
            get
            {
                return m_textWrappingBounds;
            }
            set
            {
                m_textWrappingBounds = value;
            }
        }
        /// <summary>
        /// Get or Set the wrapping element instances.
        /// </summary>
        /// <value></value>
        internal Entity FloatingEntity
        {
            get
            {
                return m_FloatingEntity;
            }
            set
            {
                m_FloatingEntity = value;
            }
        }
        /// <summary>
        /// Gets wrapping style.
        /// </summary>
        /// <value></value>
        internal TextWrappingStyle TextWrappingStyle
        {
            get
            {
                if (m_FloatingEntity is WTable)
                {
                    WTable table = m_FloatingEntity as WTable;
                    if (table.IsFrame)
                    {
                        if (!(table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.WrapFrameAround != FrameWrapMode.NotBeside
                        && table.Rows[0].Cells[0].Paragraphs[0].ParagraphFormat.WrapFrameAround != FrameWrapMode.None))
                            return TextWrappingStyle.TopAndBottom;
                    }
                    else if (table.m_isTextBox)
                        return table.m_textBoxFormat.TextWrappingStyle;
                }
                else if (m_FloatingEntity is WParagraph)
                {
                    WParagraph paragraph = m_FloatingEntity as WParagraph;
                    if (!(paragraph.ParagraphFormat.WrapFrameAround != FrameWrapMode.None
                   && paragraph.ParagraphFormat.WrapFrameAround != FrameWrapMode.NotBeside))
                        return TextWrappingStyle.TopAndBottom;
                }
                else if (m_FloatingEntity is WPicture)
                    return (m_FloatingEntity as WPicture).TextWrappingStyle;
                else if (m_FloatingEntity is WTextBox)
                    return (m_FloatingEntity as WTextBox).TextBoxFormat.TextWrappingStyle;
                else if (m_FloatingEntity is Shape)
                    return (m_FloatingEntity as Shape).WrapFormat.TextWrappingStyle;
                return TextWrappingStyle.Square;

            }
        }
        /// <summary>
        /// Gets wrapping type
        /// </summary>
        /// <value></value>
        internal TextWrappingType TextWrappingType
        {
            get
            {
                if (m_FloatingEntity is WPicture)
                    return (m_FloatingEntity as WPicture).TextWrappingType;
                else if (m_FloatingEntity is Shape)
                    return (m_FloatingEntity as Shape).WrapFormat.TextWrappingType;
                else if (m_FloatingEntity is WTextBox)
                    return (m_FloatingEntity as WTextBox).TextBoxFormat.TextWrappingType;
                else if (m_FloatingEntity is WTable && (m_FloatingEntity as WTable).m_isTextBox)
                    return (m_FloatingEntity as WTable).m_textBoxFormat.TextWrappingType;
                return TextWrappingType.Both;
            }
        }
        /// <summary>
        /// Gets allow overlap value
        /// </summary>
        /// <value></value>
        internal bool AllowOverlap
        {
            get
            {
                if (m_FloatingEntity is WTable)
                {
                    WTable table = m_FloatingEntity as WTable;
                    if (table.m_isTextBox && table.m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                        return table.m_textBoxFormat.AllowOverlap;
                }
                else if (m_FloatingEntity is WPicture && (m_FloatingEntity as WPicture).TextWrappingStyle != TextWrappingStyle.Inline)
                    return (m_FloatingEntity as WPicture).AllowOverlap;
                else if (m_FloatingEntity is Shape  && (m_FloatingEntity as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                    return (m_FloatingEntity as Shape).WrapFormat.AllowOverlap;
                else
                    return false;
                return true;
            }
        }
        #endregion
    }
}

#endif