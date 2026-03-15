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
    public delegate void AutoGeneratingColumnEventHandler(object sender, AutoGeneratingColumnArgs e);

    public class AutoGeneratingColumnArgs : GridCancelEventArgs
    {
        public GridColumn Column { get; set; }

        public AutoGeneratingColumnArgs(GridColumn column, object originalSource)
            : base(originalSource)
        {
            Column = column;
        }
    }
}
