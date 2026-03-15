#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the tab control serialization parameters.
    /// </summary>
    [XmlRootAttribute(ElementName = "TabControlParams", IsNullable = false)]
    public class TabControlParams
    {
        #region Private members
        /// <summary>
        /// Internal items list
        /// </summary>
        private List<ItemInfo> mItems;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the tab strip placement.
        /// </summary>
        /// <value>The tab strip placement.</value>
        public TabStripPlacement TabStripPlacement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public List<ItemInfo> Items
        {
            get
            {
                return mItems;
            }

            set
            {
                mItems = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the TabControlParams class.
        /// </summary>
        public TabControlParams()
        {
            mItems = new List<ItemInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the TabControlParams class.
        /// </summary>
        /// <param name="tabControl">The tab control.</param>
        public TabControlParams(TabControlAdv tabControl)
            : this()
        {
            int index = 0;
            TabStripPlacement = tabControl.TabStripPlacement;

            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                TabItemAdv item = tabControl.Items[i] as TabItemAdv;
                if (item != null)
                {
                    if (item.Name == string.Empty)
                    {
                        item.EnsureName();
                    }

                    mItems.Add(new ItemInfo(item.Name, index, item.Header, item.Visibility));
                }

                index++;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents tab item info class.
    /// </summary>
    public class ItemInfo
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the visibility.
        /// </summary>
        /// <value>The visibility.</value>
        public Visibility Visibility
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the ItemInfo class.
        /// </summary>
        public ItemInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ItemInfo class.
        /// </summary>
        /// <param name="name">Represents name.</param>
        /// <param name="index">Represents index.</param>
        /// <param name="header">Represents header.</param>
        /// <param name="visibility">Represents visibility.</param>
        public ItemInfo(string name, int index, object header, Visibility visibility)
        {
            Name = name;
            Index = index;
            Header = header;
            Visibility = visibility;
        }
    } 
}
