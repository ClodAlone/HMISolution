// <copyright file="DatesCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Collection for storing selected dates.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DatesCollection : ObservableCollection<DateTime>
    {
        #region Private members

        /// <summary>
        /// Indicates whether insertion of items into the collection is allowed.
        /// </summary>
        private bool m_allowInsert = true;

        #endregion Private members

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether insertion of items
        /// into the collection is allowed.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        protected internal bool AllowInsert
        {
            get
            {
                return m_allowInsert;
            }

            set
            {
                m_allowInsert = value;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Inserts an element into the collection at the specified
        /// index.
        /// </summary>
        /// <param name="index">The zero-based index at which item
        /// should be inserted.</param>
        /// <param name="item">The <see cref="System.DateTime"/> object to insert.</param>
        /// <exception cref="NotSupportedException">Cannot add new date to collection if
        /// property is set to false.</exception>
        protected override void InsertItem(int index, DateTime item)
        {
            if (AllowInsert)
            {
                if (!Contains(item))
                {
                    base.InsertItem(index, item);
                }
            }
            else
            {
                throw new NotSupportedException("Cannot add new Date to collection because AllowSellection is false");
            }
        }

        #endregion Implementation
    }
}