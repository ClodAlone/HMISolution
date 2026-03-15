// <copyright file="ItemHeader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class presents header of switch's items in QuickTab mode.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ItemHeader : Control
    {
        #region Constants
        /// <summary>
        /// Defines name of button in template.
        /// </summary>
        private const string BUTTON_NAME = "PART_Button";
        #endregion

        #region Private member
        /// <summary>
        /// This member presents the close button.
        /// </summary>
        private ContentControl m_button = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ItemHeader"/> class.
        /// </summary>
        static ItemHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemHeader), new FrameworkPropertyMetadata(typeof(ItemHeader)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the close button.
        /// </summary>
        /// <value>The close button.</value>
        public ContentControl CloseButton
        {
            get
            {
                return m_button;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_button = GetTemplateChild(BUTTON_NAME) as ContentControl;

            if (null == m_button)
            {
                throw new NotImplementedException("Incorrect template");
            }
        }
        #endregion
    }
}