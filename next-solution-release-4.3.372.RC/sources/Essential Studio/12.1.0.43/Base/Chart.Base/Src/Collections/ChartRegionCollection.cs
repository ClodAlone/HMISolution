#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the collection of <see cref="ChartRegion"/>.
    /// </summary>
    public sealed class ChartRegionCollection : Collection<ChartRegion>
    {
        #region Public methods
        /// <summary>
        /// Retrieves the <see cref="ChartRegion"/> at the specified coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>It returns the region around the hit test point.</returns>
        public ChartRegion HitTest(Point point)
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                ChartRegion rgn = this[i];

                if (rgn != null && rgn.Region != null && rgn.Region.IsVisible(point))
                {
                    return rgn;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the <see cref="ChartRegion"/> at the specified coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>It returns the region around the hit test point.</returns>
        public ChartRegion HitTest(PointF point)
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                ChartRegion rgn = this[i];

                if (rgn != null && rgn.Region != null && rgn.Region.IsVisible(point))
                {
                    return rgn;
                }
            }

            return null;
        }
        #endregion
    }
}
