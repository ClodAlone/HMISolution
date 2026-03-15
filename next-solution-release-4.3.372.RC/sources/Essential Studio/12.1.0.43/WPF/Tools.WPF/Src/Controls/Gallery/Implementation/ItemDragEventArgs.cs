// <copyright file="ItemDragEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class contains the information about event which is
    /// raised when item is dragging or dragged.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ItemDragEventArgs : RoutedEventArgs
    {
        #region Private fields
        /// <summary>
        /// Item that is dragging.
        /// </summary>
        private GalleryItem m_dragItem;

        /// <summary>
        /// Item parent.
        /// </summary>
        private GalleryGroup m_sourceGroup;

        /// <summary>
        /// Group in which the item was added.
        /// </summary>
        private GalleryGroup m_targetGroup;
        #endregion

        #region Properties
        /// <summary>
        /// Gets item that is dragging.
        /// </summary>
        public GalleryItem Item
        {
            get
            {
                return m_dragItem;
            }

            internal set
            {
                m_dragItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the item that is dragging.
        /// </summary>
        public GalleryGroup SourceGroup
        {
            get
            {
                return m_sourceGroup;
            }

            set
            {
                m_sourceGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the group in which the item was added.
        /// </summary>
        public GalleryGroup TargetGroup
        {
            get
            {
                return m_targetGroup;
            }

            set
            {
                m_targetGroup = value;
            }
        }
        #endregion
    }
}
