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
    public class OlapClientProperties
    {
        #region PrivateVariables
        private string url = string.Empty;
        private string title = string.Empty;
        private string cssClass = string.Empty;
        private OlapGridLayout gridLayout = OlapGridLayout.Normal;
        private ProgressMode progressMode = ProgressMode.Infinite;
        private object displayOptions = new OlapClientDisplayOptions();
        private object serviceMethods = new OlapClientServiceMethods();
        private Dictionary<String, object> customObject = new Dictionary<string, object>();
        //Events
        private string beforeServiceInvoke = null;
        private string afterServiceInvoke = null;
        private string load = null;
        private string chartPreRender = null;
        private string clientSuccess = null;
        private string clientError = null;
        private string clientComplete = null;
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
        [JsonProperty("title")]
        [DefaultValue("")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        [JsonProperty("gridLayout")]
        [DefaultValue(OlapGridLayout.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OlapGridLayout GridLayout
        {
            get { return this.gridLayout; }
            set { this.gridLayout = value; }
        }
        [JsonProperty("displayOptions")]
        public object DisplayOptions
        {
            get { return this.displayOptions; }
            set { this.displayOptions = value; }
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
        //Events
        [JsonProperty("load")]
        [DefaultValue(null)]
        public string Load
        {
            get { return this.load; }
            set { this.load = value; }
        }
        [JsonProperty("chartPreRender")]
        [DefaultValue(null)]
        public string ChartPreRender
        {
            get { return this.chartPreRender; }
            set { this.chartPreRender = value; }
        }
        [JsonProperty("clientSuccess")]
        [DefaultValue(null)]
        public string ClientSuccess
        {
            get { return this.clientSuccess; }
            set { this.clientSuccess = value; }
        }
        [JsonProperty("clientError")]
        [DefaultValue(null)]
        public string ClientError
        {
            get { return this.clientError; }
            set { this.clientError = value; }
        }
        [JsonProperty("clientComplete")]
        [DefaultValue(null)]
        public string ClientComplete
        {
            get { return this.clientComplete; }
            set { this.clientComplete = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public string Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
    }
}
