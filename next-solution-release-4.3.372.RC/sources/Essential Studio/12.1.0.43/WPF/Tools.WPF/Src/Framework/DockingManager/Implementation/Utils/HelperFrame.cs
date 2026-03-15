// <copyright file="HelperFrame.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;  
namespace Syncfusion.Windows.Tools.Controls
{ 
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Specifies the Helper class.
    /// </summary>
    internal class HelperFrame : DependencyObject
    {
        #region Private member
        /// <summary>
        /// Specify the popup.
        /// </summary>
        private Popup m_popup;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get
            {
                if (m_popup == null)
                {
                    return false;
                }

                return m_popup.IsOpen;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Shows the specified location.
        /// </summary>
        /// <param name="location">The location inPoints..</param>
        /// <param name="size">The size of the window.</param>
        public void Show(Point location, Size size)
        {
            if (m_popup == null)
            {
                CreatePopup();
            }

            m_popup.PlacementRectangle = new Rect(location, size);
            UpdateSize();

            m_popup.IsOpen = true;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            if (m_popup != null)
            {
                m_popup.IsOpen = false;
            }
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="location">The location in Points.</param>
        /// <param name="size">The size of the Window.</param>
        public void SetPosition(Point location, Size size)
        {
            Rect bounds = new Rect(location, size);
            m_popup.PlacementRectangle = bounds;
            UpdateSize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the size.
        /// </summary>
        private void UpdateSize()
        {
            AutoTemplatedControl control = (AutoTemplatedControl)m_popup.Child;
            Rect bounds = m_popup.PlacementRectangle;

            control.Width = bounds.Width;
            control.Height = bounds.Height;
        }

        /// <summary>
        /// Creates the popup.
        /// </summary>
        private void CreatePopup()
        {
            m_popup = new Popup
            {
                Placement = PlacementMode.Absolute,
                PlacementRectangle = new Rect(1, 1, 1, 1),
                Child = new AutoTemplatedControl(GetType()),
                AllowsTransparency = true
            };
        }
        #endregion
    }
}
