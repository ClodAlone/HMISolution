#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public delegate void QueryUnbounColumnValueHandler(object sender, GridUnboundColumnEventsArgs e);

    public class GridUnboundColumnEventsArgs : GridEventArgs
    {
        #region ctor
        internal GridUnboundColumnEventsArgs(UnBoundActions action, object value, GridColumn column, object record, object originalSource)
            : base(originalSource)
        {
            this.UnBoundAction = action;
            this.Value = value;
            this.Column = column;
            this.Record = record;
        }
        #endregion

        #region Public properties
        public GridColumn Column { get; internal set; }
        public object Value { get; set; }
        public object Record { get; internal set; }
        public UnBoundActions UnBoundAction { get; internal set; }
        #endregion
    }

    #region Enum UnBoundAction
    public enum UnBoundActions
    {
        QueryData,
        CommitData
    }
    #endregion

}
