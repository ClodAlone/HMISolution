#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents <see cref="ChartSeries"/> read only collection.
    /// </summary>
    public class SeriesReadOnlyCollection : ReadOnlyCollection<ChartSeries>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesReadOnlyCollection"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        public SeriesReadOnlyCollection(ChartSeriesCollection series) : base(series) { }
    }
}
