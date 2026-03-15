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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class OlapReportCollection : Collection<OlapReport>
    {
        #region Public Methods 
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

        public OlapReport this[int index]
        {
            get
            {
                return base.Items[index] as OlapReport;
            }

            set
            {
                base.Items[index] = value;
            }
        }

        public void Add(OlapReport report)
        {
            base.Items.Add(report);
        }

        public OlapReport FindReportByName(string name)
        {
            foreach (OlapReport report in base.Items)
            {
                if (report.Name == name)
                {
                    return report;
                }
            }

            return null;
        }

        public void Remove(OlapReport report)
        {
            base.Items.Remove(report);
        }
        #endregion
    }
}
