// <copyright file="ColorItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
  /// Describes color item from the system color items list.
  /// </summary>
    internal class xColorItem
    {
        #region Private Members
        /// <summary>
        /// Name of the color item
        /// </summary>
        private string m_name;

        /// <summary>
        /// Brush of the color item
        /// </summary>
        private SolidColorBrush m_brush;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Name of the color item
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        public string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets current Brush of the color item
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public SolidColorBrush Brush
        {
            get
            {
                return m_brush;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorItem"/> class.
        /// </summary>
        /// <param name="name">Name of the ColorItem.</param>
        /// <param name="brush">Brush of the ColorItem.</param>
        public xColorItem(string name, SolidColorBrush brush)
        {
            m_name = name;
            m_brush = brush;
            Color color = brush.Color;
        }

        #endregion
    }
}
