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
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart range column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="RangeColumnSeries"/>
    [ClassReference(IsReviewed = false)]
    public class RangeColumnSegment : ColumnSegment
    {

        #region fields

        private RangeColumnSeries containerSeries;

        #endregion

        #region properties

        public double High { get; set; }

        public double Low { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for RangeColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public RangeColumnSegment(double x1, double y1, double x2, double y2, RangeColumnSeries series, object item)
            : base(x1,y1,x2,y2)
        {
            base.Series = series;
            base.Item = item;
            this.containerSeries = series;
        }

        #endregion

        #region methods

        public override void Update(IChartTransformer transformer)
        {
            High = Top;
            Low = Bottom;
            base.Update(transformer);
        }

        #endregion
    }
}
