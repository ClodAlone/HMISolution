// <copyright file="StyleItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Used for storing date/style pairs.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class StyleItem
    {
        #region Private members

        /// <summary>
        /// Defines item date.
        /// </summary>
        private DateTime m_date;

        /// <summary>
        /// Defines item style.
        /// </summary>
        private Style m_style;

        #endregion Private members

        #region Initilization

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleItem"/> class.
        /// </summary>
        public StyleItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleItem"/> class.
        /// </summary>
        /// <param name="date">The date value.</param>
        /// <param name="style">The style value.</param>
        public StyleItem(DateTime date, Style style)
        {
            m_date = date;
            m_style = style;
        }

        #endregion Initilization

        #region Properties

        /// <summary>
        /// Gets or sets date of the item.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// </value>
        /// <seealso cref="DateTime"/>
        public DateTime Date
        {
            get
            {
                return m_date;
            }

            set
            {
                m_date = value;
            }
        }

        /// <summary>
        /// Gets or sets style of the item.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        /// <seealso cref="Style"/>
        public Style Style
        {
            get
            {
                return m_style;
            }

            set
            {
                m_style = value;
            }
        }

        #endregion Properties
    }
}