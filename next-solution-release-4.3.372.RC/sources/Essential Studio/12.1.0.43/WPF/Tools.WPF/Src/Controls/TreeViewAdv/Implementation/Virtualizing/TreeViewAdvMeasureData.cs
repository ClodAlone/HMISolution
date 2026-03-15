// <copyright file="TreeViewAdvMeasureData.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Windows;
using System.Diagnostics;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a class that provides functionality for save measured data for item.
    /// </summary>
    internal class TreeViewAdvMeasureData
    {
        #region Members
        /// <summary>
        /// Offset for measure. Using for animation.
        /// </summary>
        private double m_delta = 0;

        /// <summary>
        /// Header size for item.
        /// </summary>
        private Size m_size;

        /// <summary>
        /// Extended size for item.
        /// </summary>
        private Size m_extendedSize;

        /// <summary>
        /// Indicates whether the items are expanded or collapsed.
        /// </summary>
        private bool m_bIsExpanded;

        /// <summary>
        /// Indicates whether the items are visible or not.
        /// </summary>
        private bool m_bIsVisible = true;

        /// <summary>
        /// Hash key for parent of the item.
        /// </summary>
        private object m_parentKey = null;

        /// <summary>
        /// Indicates whether the items are in progress or not.
        /// </summary>
        private bool m_bIsInProgress = false;

        /// <summary>
        /// Indicates whether the items are IsStartExpand or not.
        /// </summary>
        private bool m_bIsStartExpand = false;

        /// <summary>
        /// Indicates whether the items are IsStartCollapse or not.
        /// </summary>
        private bool m_bIsStartCollapse = false;

        /// <summary>
        /// X-axis offset for items.
        /// </summary>
        private double m_itemsOffset;

        private Size m_complatePanelSize;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size of tree view measure data.</value>
        internal Size Size
        {
            get
            {
                return m_size;
            }

            set
            {
                if (value != m_size)
                {
                    m_size = value;
                }
            }
        }

        internal Size CompletePanelSize
        {
            get
            {
                return m_complatePanelSize;
            }

            set
            {
                if (value != m_complatePanelSize)
                {
                    m_complatePanelSize = value;
                }
            }
        }

        /// <summary>
        /// Gets the size of the extended.
        /// </summary>
        /// <value>The size of the extended.</value>
        internal Size ExtendedSize
        {
            get
            {
                Size size;

                if (IsExpanded)
                {
                    size = new Size(InternalExtendedSize.Width, InternalExtendedSize.Height);
                }
                else
                {
                    size = new Size(Size.Width, Size.Height);
                }

                if (m_bIsInProgress)
                {
                    if (Delta >= 0)
                    {
                        size.Height = Delta;
                    }
                }

                return size;
            }
           
        
               
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in progress.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in progress; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInProgress
        {
            get
            {
                return m_bIsInProgress;
            }

            set
            {
                if (value != m_bIsInProgress)
                {
                    m_bIsInProgress = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is start expand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is start expand; otherwise, <c>false</c>.
        /// </value>
        internal bool IsStartExpand
        {
            get
            {
                return m_bIsStartExpand;
            }

            set
            {
                if (value != m_bIsStartExpand)
                {
                    m_bIsStartExpand = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is start collapse.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is start collapse; otherwise, <c>false</c>.
        /// </value>
        internal bool IsStartCollapse
        {
            get
            {
                return m_bIsStartCollapse;
            }

            set
            {
                if (value != m_bIsStartCollapse)
                {
                    m_bIsStartCollapse = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        /// <value>The parent key.</value>
        internal object ParentKey
        {
            get
            {
                return m_parentKey;
            }

            set
            {
                if (value != m_parentKey)
                {
                    m_parentKey = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        internal bool IsExpanded
        {
            get
            {
                return m_bIsExpanded;
            }

            set
            {
                if (value != m_bIsExpanded)
                {
                    m_bIsExpanded = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        /// <value>The delta.</value>
        internal double Delta
        {
            get
            {
                return m_delta;
            }

            set
            {
                if (value != m_delta)
                {
                    m_delta = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the internal extended.
        /// </summary>
        /// <value>The size of the internal extended.</value>
        internal Size InternalExtendedSize
        {
            get
            {
                return m_extendedSize;
            }

            set
            {
                if (value != m_extendedSize)
                {
                    m_extendedSize = value;
                   
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsVisible
        {
            get
            {
                return m_bIsVisible;
            }

            set
            {
                if (m_bIsVisible != value)
                {
                    m_bIsVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value of X-axis offset for items.
        /// </summary>
        internal double ItemsOffset
        {
            get
            {
                return m_itemsOffset;
            }

            set
            {
                if (m_itemsOffset != value)
                {
                    m_itemsOffset = value;
                }
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewAdvMeasureData"/> class.
        /// </summary>
        /// <param name="parentKey">The parent key TreeViewAdvMeasureData.</param>
        /// <param name="itemsOffset">The X-Axis offset of the items.</param>
        /// <param name="size">The size TreeViewAdvMeasureData.</param>
        internal TreeViewAdvMeasureData(object parentKey, double itemsOffset, Size size)
        {
            m_parentKey = parentKey;
            m_itemsOffset = itemsOffset;
            m_size = size;
            m_extendedSize = size;
        }

        internal TreeViewAdvMeasureData(object parentKey, double itemsOffset, Size size, Size completePanelSize)
            : this(parentKey, itemsOffset, size)
        {

            m_parentKey = parentKey;
            m_itemsOffset = itemsOffset;
            m_size = size;
            m_extendedSize = size;
            m_complatePanelSize = completePanelSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewAdvMeasureData"/> class.
        /// </summary>
        /// <param name="parentKey">The parent key.</param>
        /// <param name="item">The item TreeViewAdvMeasureData.</param>
        /// <param name="itemsOffset">The X-Axis offset of the items.</param>
        /// <param name="size">The size TreeViewAdvMeasureData.</param>
        internal TreeViewAdvMeasureData(object parentKey, object item, double itemsOffset, Size size)
            : this(parentKey, itemsOffset, size)
        {
            UIElement ui = item as UIElement;
            if (ui != null)
            {
                m_bIsVisible = ui.Visibility != Visibility.Collapsed;
            }

            TreeViewItemAdv tr = item as TreeViewItemAdv;
            if (tr != null)
            {
                m_bIsExpanded = tr.IsExpanded;
            }
            else
            {
                m_bIsExpanded = false;
            }
        }

       
        #endregion
    }
}
