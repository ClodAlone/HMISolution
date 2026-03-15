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
    /// Represents the Day Name Cell Class.
    /// </summary>
    public class DayNameCell : Cell
    {
        /// <summary>
        /// Gets or sets the visible month.
        /// </summary>
        /// <value>The visible month.</value>
        protected internal int VisibleMonth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the visible year.
        /// </summary>
        /// <value>The visible year.</value>
        protected internal int VisibleYear
        {
            get;
            set;
        }
    }
}