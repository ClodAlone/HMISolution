#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Grid
{

    public delegate void ResizingColumnsEventHandler(object sender, ResizingColumnsEventArgs e);

    public class ResizingColumnsEventArgs : GridCancelEventArgs
    {
        public ResizingColumnsEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        /// <summary>
        /// Return the Column Index.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get;
            set;
        }
    }
}
