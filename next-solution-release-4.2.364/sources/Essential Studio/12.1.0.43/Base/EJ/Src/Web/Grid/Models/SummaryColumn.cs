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
     public class SummaryColumn<T> where T: class
    {
        private SummaryType summaryType;
        private String displayColumn = null;
        private String suffix = null;
        private String prefix = null;
        private String dataMember;
        private String format = null;
        private String customSummaryValue;
        
        //Properties
        [JsonProperty("summaryType")]
        [DefaultValue(null)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SummaryType SummaryType
        {
            get { return this.summaryType; }
            set { this.summaryType = value; }
        }
        [JsonProperty("displayColumn")]
        [DefaultValue(null)]
        public String DisplayColumn
        {
            get { return this.displayColumn; }
            set { this.displayColumn = value; }
        }
        [JsonProperty("suffix")]
        [DefaultValue(null)]
        public String Suffix
        {
            get { return this.suffix; }
            set { this.suffix = value; }
        }
        [JsonProperty("prefix")]
        [DefaultValue(null)]
        public String Prefix
        {
            get { return this.prefix; }
            set { this.prefix = value; }
        }
        [JsonProperty("dataMember")]
        [DefaultValue(null)]
        public String DataMember
        {
            get { return this.dataMember; }
            set { this.dataMember = value; }
        }
        [JsonProperty("format")]
        [DefaultValue(null)]
        public String Format
        {
            get { return this.format; }
            set { this.format = value; }
        }
        [JsonProperty("customSummaryValue")]
        [DefaultValue(null)]
        public String CustomSummaryValue
        {
            get { return this.customSummaryValue; }
            set { this.customSummaryValue = value; }
        }
    }
}
