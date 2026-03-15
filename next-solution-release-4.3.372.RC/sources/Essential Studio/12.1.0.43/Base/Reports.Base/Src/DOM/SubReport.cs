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

namespace Syncfusion.RDL.DOM
{
    public class SubReport : ReportItem
    {
        public string ReportName { get; set; }
        public Parameters Parameters { get; set; }
        public string NoRowsMessage { get; set; }
        public bool MergeTransactions { get; set; }
        public bool KeepTogether { get; set; }
        public bool OmitBorderOnPageBreak { get; set; }

        public bool ShouldSerializeMergeTransactions()
        {
            return this.MergeTransactions != false;
        }

        public void ResetMergeTransactions()
        {
            this.MergeTransactions = false;
        }

        public bool ShouldSerializeKeepTogether()
        {
            return this.KeepTogether != false;
        }

        public void ResetKeepTogether()
        {
            this.KeepTogether = false;
        }

        public bool ShouldSerializeOmitBorderOnPageBreak()
        {
            return this.OmitBorderOnPageBreak != false;
        }

        public void ResetOmitBorderOnPageBreak()
        {
            this.OmitBorderOnPageBreak = false;
        }
    }
}
