// <copyright file="GroupBarParams.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Used for serializing GroupBar object in isolated storage ( to save GroupBar object state ).
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    public class GroupBarParams
    {
        #region Private members
        /// <summary>
        /// GroupBar items.
        /// </summary>
        private ArrayList m_barItems;

        private Orientation m_orientation;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets GroupBar items.
        /// </summary>
        /// <value>
        /// Type: <see cref="ArrayList"/>
        /// </value>
        /// <seealso cref="ArrayList"/>
        public ArrayList GroupBarItems
        {
            get
            {
                return m_barItems;
            }

            set
            {
                m_barItems = value;
            }
        }

        // <summary>
        /// Gets or sets GroupBar orentation.
        /// </summary>
        /// <value>
        /// Type: <see cref="Orientation"/>
        /// </value>
        /// <seealso cref="Orientation"/>
        public Orientation Orentation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                m_orientation = value;
            }
        }
        
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarParams"/> class.
        /// </summary>
        public GroupBarParams()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarParams"/> class.
        /// </summary>
        /// <param name="groupBar">The group bar.</param>
        public GroupBarParams(GroupBar groupBar)
        {
            m_barItems = new ArrayList();
            foreach (GroupBarItem item in groupBar.Items)
            {
                m_barItems.Add(item);
            }

            m_orientation = groupBar.Orientation;
        }
        #endregion
    }
}
