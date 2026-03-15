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
using Syncfusion.JavaScript.Shared;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Olap.Models
{
    public class OlapGridProperties
    {
        #region PrivateVariables
        private string url = string.Empty;
        private string cssClass = string.Empty;
        private OlapGridLayout gridLayout = OlapGridLayout.Normal;
        private bool enableCellContext = false;
        private bool enableValueCellHyperlink = false;
        private bool enableRowHeaderHyperlink = false;
        private bool enableColumnHeaderHyperlink = false;
        private bool enableSummaryCellHyperlink = false;
        private bool enableVirtualScrolling = false;
        private ProgressMode progressMode = ProgressMode.Infinite;
        private Dictionary<string, object> customObject = new Dictionary<string,object>();
        private object serviceMethods = new OlapGridServiceMethods();
        //Events
        private string beforeServiceInvoke = null;
        private string afterServiceInvoke = null;
        private string drillSuccess = null;
        private string cellContextEvent = null;
        private string valueCellHyperlinkClick = null;
        private string rowHeaderHyperlinkClick = null;
        private string columnHeaderHyperlinkClick = null;
        private string summaryCellHyperlinkClick = null;
        private string destroy = null;
        #endregion

        #region Properties
        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("gridLayout")]
        [DefaultValue(OlapGridLayout.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OlapGridLayout GridLayout
        {
            get { return this.gridLayout; }
            set { this.gridLayout = value; }
        }
        [JsonProperty("enableCellContext")]
        [DefaultValue(false)]
        public bool EnableCellContext
        {
            get { return this.enableCellContext; }
            set { this.enableCellContext = value; }
        }
        [JsonProperty("enableValueCellHyperlink")]
        [DefaultValue(false)]
        public bool EnableValueCellHyperlink
        {
            get { return this.enableValueCellHyperlink; }
            set { this.enableValueCellHyperlink = value; }
        }
        [JsonProperty("enableRowHeaderHyperlink")]
        [DefaultValue(false)]
        public bool EnableRowHeaderHyperlink
        {
            get { return this.enableRowHeaderHyperlink; }
            set { this.enableRowHeaderHyperlink = value; }
        }
        [JsonProperty("enableColumnHeaderHyperlink")]
        [DefaultValue(false)]
        public bool EnableColumnHeaderHyperlink
        {
            get { return this.enableColumnHeaderHyperlink; }
            set { this.enableColumnHeaderHyperlink = value; }
        }
        [JsonProperty("enableSummaryCellHyperlink")]
        [DefaultValue(false)]
        public bool EnableSummaryCellHyperlink
        {
            get { return this.enableSummaryCellHyperlink; }
            set { this.enableSummaryCellHyperlink = value; }
        }

        [JsonProperty("enableVirtualScrolling")]
        [DefaultValue(false)]
        public bool EnableVirtualScrolling
        {
            get { return this.enableVirtualScrolling; }
            set { this.enableVirtualScrolling = value; }
        }

        [JsonProperty("progressMode")]
        [DefaultValue(ProgressMode.Infinite)]
        public ProgressMode ProgressMode
        {
            get { return this.progressMode; }
            set { this.progressMode = value; }
        }
        [JsonProperty("serviceMethods")]
        public object ServiceMethods
        {
            get { return this.serviceMethods; }
            set { this.serviceMethods = value; }
        }
        [JsonProperty("customObject")]
        [DefaultValue(null)]
        public Dictionary<String, object> CustomObject
        {
            get { return this.customObject; }
            set { this.customObject = value; }
        }
        //Events
        [JsonProperty("beforeServiceInvoke")]
        [DefaultValue(null)]
        public string BeforeServiceInvoke
        {
            get { return this.beforeServiceInvoke; }
            set { this.beforeServiceInvoke = value; }
        }
        [JsonProperty("afterServiceInvoke")]
        [DefaultValue(null)]
        public string AfterServiceInvoke
        {
            get { return this.afterServiceInvoke; }
            set { this.afterServiceInvoke = value; }
        }
        [JsonProperty("drillSuccess")]
        [DefaultValue(null)]
        public string DrillSuccess
        {
            get { return this.drillSuccess; }
            set { this.drillSuccess = value; }
        }
        [JsonProperty("cellContextEvent")]
        [DefaultValue(null)]
        public string CellContextEvent
        {
            get { return this.cellContextEvent; }
            set { this.cellContextEvent = value; }
        }
        [JsonProperty("valueCellHyperlinkClick")]
        [DefaultValue(null)]
        public string ValueCellHyperlinkClick
        {
            get { return this.valueCellHyperlinkClick; }
            set { this.valueCellHyperlinkClick = value; }
        }
        [JsonProperty("rowHeaderHyperlinkClick")]
        [DefaultValue(null)]
        public string RowHeaderHyperlinkClick
        {
            get { return this.rowHeaderHyperlinkClick; }
            set { this.rowHeaderHyperlinkClick = value; }
        }
        [JsonProperty("columnHeaderHyperlinkClick")]
        [DefaultValue(null)]
        public string ColumnHeaderHyperlinkClick
        {
            get { return this.columnHeaderHyperlinkClick; }
            set { this.columnHeaderHyperlinkClick = value; }
        }
        [JsonProperty("summaryCellHyperlinkClick")]
        [DefaultValue(null)]
        public string SummaryCellHyperlinkClick
        {
            get { return this.summaryCellHyperlinkClick; }
            set { this.summaryCellHyperlinkClick = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
    }
}
