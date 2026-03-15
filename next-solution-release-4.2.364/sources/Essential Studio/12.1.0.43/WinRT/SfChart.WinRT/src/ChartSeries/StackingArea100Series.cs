#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif



namespace Syncfusion.UI.Xaml.Charts
{
    [ClassReference(IsReviewed = false)]
    public class StackingArea100Series : StackingAreaSeries
    {
       
        /// <summary>
        /// Creates the segments of StackingArea100Series
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            base.CreateSegments();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingArea100Series());
        }
        
    }
}
