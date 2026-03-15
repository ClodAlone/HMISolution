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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Week Number Cell Class.
    /// </summary>
    public class WeekNumberCell : Cell
    {
        /// <summary>
        /// Corner radius for the cell.
        /// </summary>
        private CornerRadius mCornerRadius;

        /// <summary>
        /// Gets or sets m_CornerRadius.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CellCornerRadius
        {
            get
            {
                return mCornerRadius;
            }

            set
            {
                mCornerRadius = value;
            }
        }
    }
}