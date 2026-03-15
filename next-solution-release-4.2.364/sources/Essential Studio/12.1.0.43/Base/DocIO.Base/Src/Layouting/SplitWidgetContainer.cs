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
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS;
using System.Drawing;

#endregion

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for SplitWidgetContainer.
    /// </summary>
    internal class SplitWidgetContainer : IWidgetContainer
    {
        #region Fields
        private IWidgetContainer m_container;
        internal IWidget m_currentChild;
        private int m_firstIndex;
        private int m_count = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitWidgetContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public SplitWidgetContainer(IWidgetContainer container)
        {
            m_container = container;
            m_currentChild = null;
            m_firstIndex = m_container.Count;
            m_count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitWidgetContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="currentChild">The current child.</param>
        /// <param name="firstIndex">Index of the first.</param>
        public SplitWidgetContainer(IWidgetContainer container, IWidget currentChild, int firstIndex)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (currentChild == null)
                throw new ArgumentNullException("currentChild");
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException("firstIndex", firstIndex, "Value can not be less 0");

            m_container = container;
            m_currentChild = currentChild;
            m_firstIndex = firstIndex;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the real widget container.
        /// </summary>
        /// <value>The real widget container.</value>
        public IWidgetContainer RealWidgetContainer
        {
            get
            {
                SplitWidgetContainer splitContainer = m_container as SplitWidgetContainer;

                if (splitContainer == null)
                {
                    return m_container;
                }

                return splitContainer.RealWidgetContainer;
            }
        }
        #endregion
        
        #region IWidget Members
        /// <summary>
        /// Gets layout info.
        /// </summary>
        /// <value></value>
        public ILayoutInfo LayoutInfo
        {
            get
            {
                return m_container.LayoutInfo;
            }
        }

        /// <summary>
        /// Draw range to graphics.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="layoutedWidget">The lay outed widget.</param>
        public void Draw(DrawingContext dc, LayoutedWidget layoutedWidget)
        {
            if (layoutedWidget.Widget is SplitWidgetContainer && layoutedWidget.ChildWidgets.Count > 0)
            {
                LayoutedWidget child = layoutedWidget.ChildWidgets[0];
                if (child.Widget is SplitWidgetContainer && child.ChildWidgets.Count > 0)
                {
                    LayoutedWidget LastLine = child.ChildWidgets[child.ChildWidgets.Count - 1];
                    int count = LastLine.ChildWidgets.Count;
                    if (count > 0 && LastLine.ChildWidgets[0].HorizontalAlign != HorizontalAlignment.Distributed 
                          && (LastLine.ChildWidgets[count - 1].Widget is SplitStringWidget || LastLine.ChildWidgets[count-1].Widget is WTextRange))
                    {
                        for (int i = 0; i < LastLine.ChildWidgets.Count; i++)
                        {
                            LastLine.ChildWidgets[i].IsLastLine = true;
                        }

                        LastLine.ChildWidgets[0].Bounds = new RectangleF(LastLine.ChildWidgets[0].Bounds.X, LastLine.ChildWidgets[0].Bounds.Y,
                                                           LastLine.ChildWidgets[0].Bounds.Width - Convert.ToSingle(LastLine.ChildWidgets[0].Spaces * LastLine.ChildWidgets[0].WordSpace), LastLine.ChildWidgets[0].Bounds.Height);
                        for (int i = 1; i < LastLine.ChildWidgets.Count; i++)
                        {
                            LayoutedWidget prev = LastLine.ChildWidgets[i - 1];
                            LayoutedWidget curr = LastLine.ChildWidgets[i];
                            if (!(((prev.Widget is WTable) && (prev.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                                 || ((curr.Widget is WTable) && (curr.Widget as WTable).m_textBoxFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                                 || ((prev.Widget is WPicture) && (prev.Widget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline)
                                 || ((curr.Widget is WPicture) && (curr.Widget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline)
                                 || ((prev.Widget is Shape) && (prev.Widget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                                 || ((curr.Widget is Shape) && (curr.Widget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)))
                            {
                                float width = curr.Bounds.Width - ((curr.SubWidth != 0.0f) ? (Convert.ToSingle(curr.Spaces * curr.WordSpace)) : 0.0f);
                                curr.Bounds = new RectangleF(prev.Bounds.X + prev.Bounds.Width, curr.Bounds.Y, width, curr.Bounds.Height);
                            }
                        }
                    }
                }
            }
            RealWidgetContainer.Draw(dc, layoutedWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
        }
        #endregion

        #region IWidgetContainer Members
        /// <summary>
        /// Gets count of child widgets.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return m_container.Count - m_firstIndex;
            }
        }

        /// <summary>
        /// Gets child widget by index.
        /// </summary>
        /// <value></value>
        public IWidget this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return m_currentChild;
                }

                return m_container[index + m_firstIndex];
            }
        }
        /// <summary>
        /// Get child widgets
        /// </summary>
        public EntityCollection WidgetInnerCollection
        {
            get
            {
                return m_container.WidgetInnerCollection;
            }
        }
        #endregion
    }
}

#endif