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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class TagCloudFields
    {
        #region Fields
        private String text = "text";
        private String url = "url";
        private String frequency = "frequency";
        #endregion
        #region Properties
        [JsonProperty("text")]
        [DefaultValue("text")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("url")]
        [DefaultValue("url")]
        public String Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
        [JsonProperty("frequency")]
        [DefaultValue("frequency")]
        public String Frequency
        {
            get { return this.frequency; }
            set { this.frequency = value; }
        }
        #endregion
    }
}

namespace Syncfusion.JavaScript
{
   

    public class TagCloudFieldsBuilder
    {
        private TagCloudFields fields = new TagCloudFields();
        public TagCloudFieldsBuilder(TagCloudFields fields)
        {
            this.fields = fields;
        }
        public TagCloudFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public TagCloudFieldsBuilder Url(String url)
        {
            this.fields.Url = url;
            return this;
        }
        public TagCloudFieldsBuilder Frequency(String frequency)
        {
            this.fields.Frequency = frequency;
            return this;
        }
    }

}