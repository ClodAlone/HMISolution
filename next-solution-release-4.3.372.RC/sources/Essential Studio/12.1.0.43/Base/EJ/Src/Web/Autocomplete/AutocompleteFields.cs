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
    public class AutocompleteFields
    {
        #region AutocompleteFields
        private String uniqueKey = null;
        private String text = null;
        private String htmlAttr = null;
        #endregion
        #region Properties
        [JsonProperty("uniqueKey")]
        [DefaultValue(null)]
        public String UniqueKey
        {
            get { return this.uniqueKey; }
            set { this.uniqueKey = value; }
        }
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("htmlAttr")]
        [DefaultValue(null)]
        public String HtmlAttr
        {
            get { return this.htmlAttr; }
            set { this.htmlAttr = value; }
        }
        #endregion
    }
    public class AutocompleteFieldsBuilder
    {
         private AutocompleteFields fields = new AutocompleteFields();
         public AutocompleteFieldsBuilder(AutocompleteFields fields)
        {
            this.fields = fields;
        }
         public AutocompleteFieldsBuilder UniqueKey(String uniqueKey)
        {
            this.fields.UniqueKey = uniqueKey;
            return this;
        }
         public AutocompleteFieldsBuilder Text(String text)
         {
             this.fields.Text = text;
             return this;
         }
         public AutocompleteFieldsBuilder HtmlAttr(String htmlAttr)
         {
             this.fields.HtmlAttr = htmlAttr;
             return this;
         }
    }
}
