//-------------------------------------------------------------------------------------------------
// <copyright file="Reports.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;


#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// A collection of <see cref="OlapReport"/>.
    /// </summary>
    [Serializable]
    public class OlapReportCollection : CollectionBase
#else

using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// A collection of <see cref="OlapReport"/>.
    /// </summary>
    [DataContract]
    public class OlapReportCollection : ObservableCollection<OlapReport>
#endif
    {
        #region Public Methods 
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.OlapReport"/> with the specified name.
        /// </summary>
        /// <value><see cref="OlapReport"/></value>
        public OlapReport this[string name]
        {
            get
            {
                return this.FindReportByName(name);
            }

            set
            {
                OlapReport report = this.FindReportByName(name);
                report = value;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.OlapReport"/> at the specified index.
        /// </summary>
        /// <value></value>
        public OlapReport this[int index]
        {
            get
            {
                return base.List[index] as OlapReport;
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns>The index of OlapReport in its collection.</returns>
        public int Add(OlapReport report)
        {
            return base.List.Add(report);
        }

        /// <summary>
        /// Removes the specified report.
        /// </summary>
        /// <param name="report">The report.</param>
        public void Remove(OlapReport report)
        {
            base.List.Remove(report);
        }

#endif
        /// <summary>
        /// Finds the name of the report by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="OlapReport"/>.</returns>
        public OlapReport FindReportByName(string name)
        {
#if !SILVERLIGHT
            foreach (OlapReport report in base.List)
#else
            foreach (OlapReport report in base.Items)
#endif
            {
                if (report.Name == name)
                {
                    return report;
                }
            }

            return null;
        }

        
        #endregion
    }
}
