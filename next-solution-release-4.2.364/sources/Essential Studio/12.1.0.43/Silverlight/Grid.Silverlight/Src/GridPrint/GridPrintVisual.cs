#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Primitives;

    /// <summary>
    /// For internal use.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridPrintVisual : FrameworkElement 
    {
        /// <summary>
        /// For internal use.
        /// </summary>
        public GridPrintVisual()
        {
           // this.ContentVisual = new DrawingVisual();
            //var count=this.VisualChildrenCount;
            //this.GetVisualChildren();
           
            
            //this.GetVisualChildrenAndSelf();
            //this.AddVisualChild(this.ContentVisual);
            //this.AddLogicalChild(this.ContentVisual);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        //public DrawingVisual ContentVisual
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// For internal use.
        /// </summary>
        protected  int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        //protected Visual GetVisualChild(int index)
        //{
        //    if (index < 0 || index >= 1)
        //    {
        //        throw new ArgumentException("index");
        //    }

        //    return this.ContentVisual;
        //}
    }
}
