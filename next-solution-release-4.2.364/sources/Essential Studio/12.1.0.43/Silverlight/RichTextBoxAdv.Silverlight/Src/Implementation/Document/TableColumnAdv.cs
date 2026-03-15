#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TableColumnAdv : DependencyObject
    {
        private double preferredwidth = 0.0;
        private double minwidth = 0.0;
        private double maxwidth = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public TableColumnAdv()
        {

        }

        /// <summary>
        /// It gets /sets the Preferred width of the Column.
        /// </summary>
        internal double PreferredWidth
        {
            get
            {
                return preferredwidth;
            }
            set
            {
                preferredwidth = value;
            }
        }

        /// <summary>
        /// It gets / sets the MinWidth of the Column.
        /// </summary>
        internal double MinWidth
        {
            get
            {
                return minwidth;
            }
            set
            {
                minwidth = value;
            }
        }

        /// <summary>
        /// It gets/sets the MaxWidth of the Column.
        /// </summary>
        internal double MaxWidth
        {
            get
            {
                return maxwidth;
            }
            set
            {
                maxwidth = value;
            }
        }
    }
}
