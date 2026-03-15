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

using System.Drawing;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// 
    /// </summary>
    internal interface ILayoutProcessHandler
    {
        /// <summary>
        /// Gets the next free area.
        /// </summary>
        /// <param name="rect">The rectangle of allowed area.</param>
        /// <param name="isContinuousSection">The isContinuousSection.</param>
        /// <returns>True if area allowed, else False</returns>
        bool GetNextArea(out RectangleF rect, ref int columnIndex, ref bool isContinuousSection, bool isSplittedWidget, ref float topMargin);

        /// <summary>
        /// Pushes the LayoutedWidget to external holder.
        /// </summary>
        /// <param name="ltWidget">The LayoutedWidget.</param>
        void PushLayoutedWidget(LayoutedWidget ltWidget, RectangleF layoutArea, bool isNeedToRestartFootnote, bool m_bisNeedToRestartEndnoteID);

        /// <summary>
        /// Handles the splitted widget.
        /// </summary>
        /// <param name="stWidgetContainer">The splitted widget container.</param>
        /// <param name="state">The current state of layout context.</param>
        /// <param name="ltWidget">The LayoutedWidget.</param>
        /// <param name="isLayoutedWidgetNeedToPushed">The isLayoutedWidgetNeedToPushed.</param>
        /// <returns>True for continue layout process, False - for stopping</returns>
        bool HandleSplittedWidget(SplitWidgetContainer stWidgetContainer, LayoutState state, LayoutedWidget ltWidget, ref bool isLayoutedWidgetNeedToPushed);

        /// <summary>
        /// Handle the LayoutedWidget to external holder.
        /// </summary>
        /// <param name="ltWidget">The LayoutedWidget.</param>
        void HandleLayoutedWidget(LayoutedWidget ltWidget);
    }
}

#endif
