// <copyright file="DataTemplateItem.cs" company="Syncfusion">
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
    /// Used for storing date/template pairs.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DataTemplateItem
    {
        #region Private members

        /// <summary>
        /// Defines item date.
        /// </summary>
        private DateTime m_date;

        /// <summary>
        /// Defines item template.
        /// </summary>
        private DataTemplate m_template;

        #endregion Private members

        #region Initilization

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateItem"/> class.
        /// </summary>
        public DataTemplateItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateItem"/> class.
        /// </summary>
        /// <param name="date">The date time value.</param>
        /// <param name="template">The data template.</param>
        public DataTemplateItem(DateTime date, DataTemplate template)
        {
            m_date = date;
            m_template = template;
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
        /// Gets or sets template of the item.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate Template
        {
            get
            {
                return m_template;
            }

            set
            {
                m_template = value;
            }
        }

        #endregion Properties
    }
}