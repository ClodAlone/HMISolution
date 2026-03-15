// <copyright file="_structs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Joins the control and its template.
    /// <para>Used for initializing the <see cref="Syncfusion.Windows.Tools.Controls.CustomAnimation"/> objects.</para>
    /// </summary>
    public struct OwnerTemlpateMapping
    {
        #region Private member
        /// <summary>
        /// The control.
        /// </summary>
        private FrameworkElement m_owner;
        
        /// <summary>
        /// The template of the control.
        /// </summary>
        private FrameworkTemplate m_template;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameworkElement"/>
        /// </value>
        /// <seealso cref="FrameworkElement"/>
        public FrameworkElement Owner
        {
            get
            {
                return m_owner;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Owner cannot be null");
                }

                m_owner = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the template of the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameworkElement"/>
        /// </value>
        /// <seealso cref="FrameworkElement"/>
        public FrameworkTemplate Template
        {
            get
            {
                return m_template;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Template cannot be null");
                }

                m_template = value;
            }
        }
        #endregion
    }
}
