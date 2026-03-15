#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.OlapSilverlight.Wrapper
{
    /// <summary>
    /// Helper class for report wrapping process. 
    /// </summary>
    public class OlapReportWrapper
    {
        /// <summary>
        /// Gets the wrapped <see cref="Syncfusion.OlapSilverlight.Reports.OlapReport"/> from <see cref="Syncfusion.Olap.Reports.OlapReport"/>.
        /// </summary>
        /// <param name="report">The report (<see cref="Syncfusion.Olap.Reports.OlapReport"/>).</param>
        /// <returns>The OlapReport(<see cref="Syncfusion.OlapSilverlight.Reports.OlapReport"/>).</returns>
        public static Syncfusion.OlapSilverlight.Reports.OlapReport GetOlapReportWrapper(Syncfusion.Olap.Reports.OlapReport report)
        {
            string xmlData = Common.Common.SerializeObject<Syncfusion.Olap.Reports.OlapReport>(report);

            Syncfusion.OlapSilverlight.Reports.OlapReport wrapperReport = Common.Common.DeserializeObject<Syncfusion.OlapSilverlight.Reports.OlapReport>(xmlData);

            return wrapperReport;
        }

        /// <summary>
        /// Gets the wrapped <see cref="Syncfusion.Olap.Reports.OlapReport"/> from <see cref="Syncfusion.OlapSilverlight.Reports.OlapReport"/>.
        /// </summary>
        /// <param name="report">The report (<see cref="Syncfusion.OlapSilverlight.Reports.OlapReport"/>).</param>
        /// <returns>The OlapReport(<see cref="Syncfusion.Olap.Reports.OlapReport"/>).</returns>
        public static Syncfusion.Olap.Reports.OlapReport GetOlapReportFromWrapper(Syncfusion.OlapSilverlight.Reports.OlapReport wrapperReport)
        {
            string xmlData = Common.Common.SerializeObject<Syncfusion.OlapSilverlight.Reports.OlapReport>(wrapperReport);

            Syncfusion.Olap.Reports.OlapReport report = Common.Common.DeserializeObject<Syncfusion.Olap.Reports.OlapReport>(xmlData);

            return report;
        }

    }
}
