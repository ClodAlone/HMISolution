#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class StackingColumn100Series3D : StackingColumnSeries3D
    {
        /// <summary>
        /// Creates the segments of StackingColumn100Series
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            base.CreateSegments();
        }
        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StackingColumn100Series());
        }
    }
}
