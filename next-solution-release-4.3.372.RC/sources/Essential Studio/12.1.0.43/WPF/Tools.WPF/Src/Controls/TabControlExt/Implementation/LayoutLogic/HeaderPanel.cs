// <copyright file="HeaderPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Single element layout panel.
    /// </summary>
    /// <property name="flag" value="Finished"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HeaderPanel : Panel
    {
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="HeaderPanel"/> class.
        /// </summary>
        static HeaderPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(HeaderPanel));
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Provides a required override for the MeasureOverride method.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        protected override Size MeasureOverride(Size availableSize)
        {
            TabControlExt tabControl = (TabControlExt)TemplatedParent;
            TabPanelAdv panelAdv = (TabPanelAdv)InternalChildren[0];
            TabLayoutPanel layoutPanel;
            if (panelAdv != null && panelAdv.Content is ScrollViewer)
                layoutPanel = (TabLayoutPanel)(panelAdv.Content as ScrollViewer).Content;
            else
                layoutPanel = (TabLayoutPanel)panelAdv.Content;
            if ((tabControl.TabItemLayout != TabItemLayoutType.SingleLine) && (tabControl.TabStripPlacement == Dock.Left || tabControl.TabStripPlacement == Dock.Right))
            {
                if (!tabControl.RotateTextWhenVertical)
                {
                    if (layoutPanel.m_RowHeight < tabControl.M_tabItemHeight
                        && layoutPanel.VisibleItemsCount > 0
                        && layoutPanel.m_NumRows == 1)
                    {
                        layoutPanel.m_RowHeight = tabControl.M_tabItemHeight;
                        availableSize.Height = (layoutPanel.m_RowHeight * layoutPanel.m_NumRows) + (TabLayoutPanel.TAB_INTERSECTION_FACTOR * layoutPanel.m_NumRows);
                    }
                    else
                        availableSize.Height = availableSize.Height;
                   
                }
                else
                {
                    if (layoutPanel != null && layoutPanel.m_NumRows > 1 && layoutPanel.VisibleItemsCount > 0)
                    {
                        layoutPanel.m_RowHeight = (layoutPanel.m_RowHeight <tabControl.M_tabItemHeight) ? tabControl.M_tabItemHeight:layoutPanel.m_RowHeight;
                        availableSize.Height = (layoutPanel.m_RowHeight * layoutPanel.m_NumRows) + (TabLayoutPanel.TAB_INTERSECTION_FACTOR * layoutPanel.m_NumRows); 
                    }
                    else
                        availableSize.Height = availableSize.Height;

                }
            }

            panelAdv.Measure(availableSize);
            availableSize = panelAdv.DesiredSize;
            if (tabControl.TabItemLayout == TabItemLayoutType.SingleLine)
                panelAdv.EnsureTabScrollStyle();
            return availableSize;
        }

        /// <summary>
        /// Provide a required override for the ArrangeOverride method.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children</param>
        /// <returns>The actual size used.</returns>
        /// <property name="flag" value="Finished"/>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            InternalChildren[0].Arrange(new Rect(new Point(0, 0), finalSize));
            if (finalSize.Height >= 0.1 && finalSize.Width != 0)
            {
                return new Size(finalSize.Width, finalSize.Height - 0.1);
            }

            return finalSize;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
                if (docking != null)
                {
                    docking.OnMouseLeaveOnHeaderPanel(this, e);
                }
                base.OnMouseLeave(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
                if (docking != null)
                {
                    docking.OnMouseMoveOnHeaderPanel(this, e);
                }
                base.OnMouseMove(e);
            }
        }

        #region TouchEvents
#if !SyncfusionFramework3_5
        protected override void OnTouchLeave(TouchEventArgs e)
        {
            DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
            if (docking != null && docking.IsTouchEnabled)
            {
                if (e.GetTouchPoint(this).Position.Y > this.ActualHeight)
                {
                    docking.OnTouchLeaveOnHeaderPanel(this, e);
                }
            }
            base.OnTouchLeave(e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            DockingManager docking = (DockingManager)VisualUtils.FindAncestor(this, typeof(DockingManager));
            if (docking != null && docking.IsTouchEnabled)
            {
                docking.OnTouchMoveOnHeaderPanel(this, e);
            }
            base.OnTouchMove(e);
        }
#endif
        #endregion

        #endregion
    }
}
