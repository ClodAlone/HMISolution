// <copyright file="TabControlSettings.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent the class for the Tab Control Settings
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    public class TabControlSettings
    {
        #region Private members

        /// <summary>
        /// Stores the Dictionary items.
        /// </summary>
        private Dictionary<string, object> m_items;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the tab strip placement.
        /// </summary>
        /// <value>The tab strip placement.</value>
        public Dock TabStripPlacement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public Dictionary<string, object> Items
        {
            get
            {
                return m_items;
            }

            set
            {
                m_items = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlSettings"/> class.
        /// </summary>
        public TabControlSettings()
        {
            m_items = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlSettings"/> class.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        public TabControlSettings(TabControl tabControl)
            : this()
        {
            int index = 0;
            TabStripPlacement = tabControl.TabStripPlacement;

            foreach (TabItemExt item in tabControl.Items)
            {
                if (item.Name != string.Empty)
                {
                    m_items.Add(item.Name, new ItemInfo(index, item.Header, item.Visibility));
                }
                else
                {
                    throw new ArgumentException("Name property can not be empty");
                }

                index++;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represent the ItemInfo Struct
    /// </summary>
    [Serializable]
    public struct ItemInfo
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header { get; set; }

        /// <summary>
        /// Gets or sets the visibility.
        /// </summary>
        /// <value>The visibility.</value>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInfo"/> struct.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="header">The header.</param>
        /// <param name="visibility">The visibility.</param>
        public ItemInfo(int index, object header, Visibility visibility)
            : this()
        {
            Index = index;
            Header = header;
            Visibility = visibility;
        }
    }
}