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
    public class RotatorFields
    {
        #region RotatorFields
        private String text = "text";
        private String url = "url";
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
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
   
    public class RotatorFieldsBuilder
    {
        private RotatorFields fields = new RotatorFields();
        public RotatorFieldsBuilder(RotatorFields fields)
        {
            this.fields = fields;
        }
        public RotatorFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public RotatorFieldsBuilder Url(String url)
        {
            this.fields.Url = url;
            return this;
        }
    }

}