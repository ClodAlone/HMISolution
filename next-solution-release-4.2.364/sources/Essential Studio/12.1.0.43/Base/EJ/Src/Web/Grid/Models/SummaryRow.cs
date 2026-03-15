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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class SummaryRows<T> where T : class
    {
        private String title = null;
        private bool showCaptionSummary = false;
        private bool showTotalSummary = true;
        private List<SummaryColumn<T>> summaryColumn = new List<SummaryColumn<T>>();

        //Properties

        [JsonProperty("title")]
        [DefaultValue(null)]
        public String Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        [JsonProperty("summaryColumns")]
        public List<SummaryColumn<T>> SummaryColumns
        {
            get { return this.summaryColumn; }
            set { this.summaryColumn = value; }
        }
        [JsonProperty("showCaptionSummary")]
        [DefaultValue(false)]
        public bool ShowCaptionSummary
        {
            get { return this.showCaptionSummary; }
            set { this.showCaptionSummary = value; }
        }
        [JsonProperty("showTotalSummary")]
        [DefaultValue(true)]
        public bool ShowTotalSummary
        {
            get { return this.showTotalSummary; }
            set { this.showTotalSummary = value; }
        }
        #region ShouldSerialize Methods

        public bool ShouldSerializeSummaryColumns()
        {
            if (SummaryColumns.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
