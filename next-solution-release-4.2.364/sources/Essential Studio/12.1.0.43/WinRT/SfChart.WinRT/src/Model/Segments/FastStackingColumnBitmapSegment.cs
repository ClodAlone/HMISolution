#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Syncfusion.UI.Xaml.Charts
{
    public class FastStackingColumnSegment : FastColumnBitmapSegment
    {
       #region ctor

        /// <summary>
        /// Called when instance created for FastStackingColumnSegment with following arguments
        /// </summary>
        /// <param name="x1Values"></param>
        /// <param name="y1Values"></param>
        /// <param name="x2Values"></param>
        /// <param name="y2Values"></param>
        /// <param name="series"></param>
        public FastStackingColumnSegment(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values, ChartSeriesBase series)
            : base(x1Values, y1Values, x2Values, y2Values, series)
        {
            base.Series = series;
        }
        #endregion

    }
}

