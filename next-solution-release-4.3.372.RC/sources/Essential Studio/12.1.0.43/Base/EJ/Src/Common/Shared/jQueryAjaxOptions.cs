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

namespace Syncfusion.JavaScript
{
   public class jQueryAjaxOptions
    {
        #region Fields
        private bool cache = false;
       private bool async = true;
       //String attributes
        private String type = "GET";
        private String contentType = "html";
        private String url = "";
        private String dataType = "html";
        private String[] data = new String[]{""};
        #endregion

        #region Properties
        [JsonProperty("cache")]
        [DefaultValue(false)]
        public bool Cache
        {
            get { return this.cache; }
            set { this.cache = value; }
        }
        [JsonProperty("async")]
        [DefaultValue(true)]
        public bool Async
        {
            get { return this.async; }
            set { this.async = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("GET")]
        public String Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        [JsonProperty("contentType")]
        [DefaultValue("html")]
        public String ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }
        [JsonProperty("url")]
        [DefaultValue("")]
        public String Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
        [JsonProperty("dataType")]
        [DefaultValue("html")]
        public String DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        }
        [JsonProperty("data")]
        [DefaultValue("")]
        public String[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
        #endregion
    }
   public class jQueryAjaxOptionsBuilder
   {
       private jQueryAjaxOptions ajaxOptions = new jQueryAjaxOptions();
       public jQueryAjaxOptionsBuilder(jQueryAjaxOptions ajaxOptions)
       {
           this.ajaxOptions = ajaxOptions;
       }
       public jQueryAjaxOptionsBuilder Cache()
       {
           this.ajaxOptions.Cache = true;
           return this;
       }
       public jQueryAjaxOptionsBuilder Cache(bool cache)
       {
           this.ajaxOptions.Cache = cache;
           return this;
       }
       public jQueryAjaxOptionsBuilder Async()
       {
           this.ajaxOptions.Async = true;
           return this;
       }
       public jQueryAjaxOptionsBuilder Async(bool async)
       {
           this.ajaxOptions.Async = async;
           return this;
       }
       public jQueryAjaxOptionsBuilder Type(String type)
       {
           this.ajaxOptions.Type = type;
           return this;
       }
       public jQueryAjaxOptionsBuilder ContentType(String contentType)
       {
           this.ajaxOptions.ContentType = contentType;
           return this;
       }
       public jQueryAjaxOptionsBuilder Url(String url)
       {
           this.ajaxOptions.Url = url;
           return this;
       }
       public jQueryAjaxOptionsBuilder DataType(String dataType)
       {
           this.ajaxOptions.DataType = dataType;
           return this;
       }
       public jQueryAjaxOptionsBuilder Data(String[] data)
       {
           this.ajaxOptions.Data = data;
           return this;
       }
   }
}
