#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DoughnutSegment3D
    /// </summary>
    public class DoughnutSegment3D : PieSegment3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoughnutSegment3D"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="center">The center.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="height">The height.</param>
        /// <param name="r">The r.</param>
        /// <param name="i">The i.</param>
        /// <param name="y">The y.</param>
        /// <param name="insideRadius">The inside radius.</param>
        public DoughnutSegment3D(ChartSeries3D series, Vector3D center, double start, double end, double height, double r, int i, double y, double insideRadius)
            : base(series, center, start, end, height, r, i, y, insideRadius)
        {

        }

    }
}
