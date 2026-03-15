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
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGaugeProperties : CircularGaugeProperties
    {
        #region PrivateVariables
        
        //Integer values
         private int rowsCount = 0;
         private int columnsCount = 0;

         //Boolean values
         private bool showTooltip = false;
         private bool showHeaderLabels = false;

         //String values         
         private string url = string.Empty;
        
         //Enumeration values
         private ProgressMode progressMode = ProgressMode.Infinite;

         //Object values
         private object serviceMethods = new OlapChartServiceMethods();
         private Dictionary<String, object> customObject = new Dictionary<string, object>();

        //Events
        private string beforeServiceInvoke = null;
        private string afterServiceInvoke = null;
        private string clientSuccess = null;
        private string clientComplete = null;
        private string clientError = null;
        private string destroy = null;
        #endregion

        #region Properties
        //Integer values
        [JsonProperty("rowsCount")]
        [DefaultValue(0)]
        public int RowsCount
        {
            get { return this.rowsCount; }
            set { this.rowsCount = value; }
        }
        [JsonProperty("columnsCount")]
        [DefaultValue(0)]
        public int ColumnsCount
        {
            get { return this.columnsCount; }
            set { this.columnsCount = value; }
        }
        //Boolean values
        [JsonProperty("showTooltip")]
        [DefaultValue(false)]
        public bool ShowTooltip
        {
            get { return this.showTooltip; }
            set { this.showTooltip = value; }
        }
        [JsonProperty("showTooltip")]
        [DefaultValue(false)]
        public bool ShowHeaderLabels
        {
            get { return this.showHeaderLabels; }
            set { this.showHeaderLabels = value; }
        }

        //String values         
        [JsonProperty("url")]
        [DefaultValue("")]
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
        //Enumeration Values
        [JsonProperty("progressMode")]
        [DefaultValue(ProgressMode.Infinite)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProgressMode ProgressMode
        {
            get { return this.progressMode; }
            set { this.progressMode = value; }
        }
        //object Values
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
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        [JsonProperty("clientSuccess")]
        [DefaultValue(null)]
        public String ClientSuccess
        {
            get { return this.clientSuccess; }
            set { this.clientSuccess = value; }
        }
        [JsonProperty("clientComplete")]
        [DefaultValue(null)]
        public String ClientComplete
        {
            get { return this.clientComplete; }
            set { this.clientComplete = value; }
        }
        [JsonProperty("clientError")]
        [DefaultValue(null)]
        public String ClientError
        {
            get { return this.clientError; }
            set { this.clientError = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeServiceMethods()
        {
            if (ServiceMethods != null)
                return true;
            else
                return false;
        }
         public bool ShouldSerializeInteriorGradient()
         {
             if (InteriorGradient.Count != 0)
                 return true;
             else
                 return false;
         }
         public bool ShouldSerializeScales()
         {
             if (Scales.Count != 0)
                 return true;
             else
                 return false;
         }           
        #endregion
    }
}
