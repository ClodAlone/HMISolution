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
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.Olap.Models
{
    public class OlapChartProperties: ChartProperties
    {
        #region PrivateVariables
        private string url = string.Empty;
        private string cssClass = string.Empty;
        private ProgressMode progressMode = ProgressMode.Infinite;
        private object serviceMethods = new OlapChartServiceMethods();
        private Dictionary<String, object> customObject = new Dictionary<string, object>();
        //Events
        private string beforeServiceInvoke = null;
        private string afterServiceInvoke = null;
        private string drillSuccess = null;
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
        [JsonProperty("progressMode")]
        [DefaultValue(ProgressMode.Infinite)]
        [JsonConverter(typeof(StringEnumConverter))]
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
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion

        public bool ShouldSerializePrimaryXAxis()
        {
            if (Syncfusion.JavaScript.Utils.PropertyCompare(PrimaryXAxis, new Axis()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializePrimaryYAxis()
        {
            if (Syncfusion.JavaScript.Utils.PropertyCompare(PrimaryYAxis, new Axis()))
                return true;
            else
                return false;
        }
    }
}
