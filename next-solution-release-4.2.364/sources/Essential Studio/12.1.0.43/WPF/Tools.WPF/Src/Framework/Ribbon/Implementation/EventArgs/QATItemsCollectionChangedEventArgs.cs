// <copyright file="QATItemsCollectionChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Drawing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Provides data for the QATItemsCollectionChanged event.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class QATItemsCollectionChangedEventArgs : EventArgs
    {
        #region Private members
        /// <summary>
        /// Represents the Old Items
        /// </summary>
        private IList m_oldItems;

        /// <summary>
        /// Represents the New Items
        /// </summary>
        private IList m_newItems;

        /// <summary>
        /// Represents the collection affected
        /// </summary>
        private QATItemsContainer type;


        private QATAction qataction;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the old items.
        /// </summary>
        /// <value>The old items.</value>
        public IList OldItems
        {
            get
            {
                return m_oldItems;
            }

            set
            {
                m_oldItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the new items.
        /// </summary>
        /// <value>The new items.</value>
        public IList NewItems
        {
            get
            {
                return m_newItems;
            }

            set
            {
                m_newItems = value;
            }
        }


        public QATItemsContainer QATItemsContainer
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }


        public QATAction QATAction
        {
            get
            {
                return qataction;
            }
            set
            {
                qataction = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="QATItemsCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldItems">The old items.</param>
        /// <param name="newItems">The new items.</param>
        public QATItemsCollectionChangedEventArgs(IList oldItems, IList newItems,QATItemsContainer collection,QATAction action)
        {
            m_oldItems = oldItems;
            m_newItems = newItems;
            type = collection;
            qataction = action;
        }
        #endregion
    }

    public enum QATItemsContainer
    {
        QAT,
        CustomizationDialog
    }

    public enum QATAction
    {
        Add,
        Remove
    }
}
