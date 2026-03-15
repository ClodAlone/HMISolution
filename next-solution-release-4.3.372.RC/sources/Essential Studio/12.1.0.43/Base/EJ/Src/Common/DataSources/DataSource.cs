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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataSources
{
    
    public class DataSource
    {
        #region Fields
        private String url = null;
        private String table = null;
        private Object json = null;
        private List<String> headers = null;
        private String accept = "application/json;odata=light;q=1,application/json;odata=verbose;q=0.5";
        private String key = null;
        private bool crossDomain;
        private String jsonp = null;
        private String dataType = null;
        private bool offline = false;
        private bool requireFormat = false;
        //private HTMLTableElement table;
        #endregion

        #region Properties
        [JsonProperty("url")]
        [DefaultValue(null)]
        public String URL
        {
            get { return this.url; }
            set { this.url = value; }
        }
        [JsonProperty("table")]
        [DefaultValue(null)]
        public String Table
        {
            get { return this.table; }
            set { this.table = value; }
        }
        [JsonProperty("json")]
        [DefaultValue(null)]
        public Object Json
        {
            get { return this.json; }
            set { this.json = value; }
        }
        [JsonProperty("headers")]
        [DefaultValue(null)]
        public List<String> Headers
        {
            get { return this.headers; }
            set { this.headers = value; }
        }
        [JsonProperty("accept")]
        [DefaultValue("application/json;odata=light;q=1,application/json;odata=verbose;q=0.5")]
        public String Accept
        {
            get { return this.accept; }
            set { this.accept = value; }
        }
        [JsonProperty("key")]
        [DefaultValue(null)]
        public String Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
        [JsonProperty("crossDomain")]
        [DefaultValue(false)]
        public bool CrossDomain
        {
            get { return this.crossDomain; }
            set { this.crossDomain = value; }
        }
        [JsonProperty("jsonp")]
        [DefaultValue(null)]
        public String Jsonp
        {
            get { return this.jsonp; }
            set { this.jsonp = value; }
        }
        [JsonProperty("dataType")]
        [DefaultValue(null)]
        public String DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        }
        [JsonProperty("offline")]
        [DefaultValue(false)]
        public bool Offline
        {
            get { return this.offline; }
            set { this.offline = value; }
        }
        [JsonProperty("requireFormat")]
        [DefaultValue(false)]
        public bool RequireFormat
        {
            get { return this.requireFormat; }
            set { this.requireFormat = value; }
        }
        #endregion
    }
}
