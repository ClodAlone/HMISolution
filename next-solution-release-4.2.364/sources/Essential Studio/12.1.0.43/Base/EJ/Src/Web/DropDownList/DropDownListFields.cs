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
    public class DropDownListFields
    {
        #region Fields
        private String id = null;
        private String imageUrl = null;
        private String imageAttributes = null;
        private String spriteCSS = null;
        private String text = null;
        private String value = null;
        private String htmlAttributes = null;
        private String selected = null;
        #endregion
        #region Properties
        [JsonProperty("id")]
        [DefaultValue(null)]
        public String Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        [JsonProperty("imageUrl")]
        [DefaultValue(null)]
        public String ImageUrl
        {
            get { return this.imageUrl; }
            set { this.imageUrl = value; }
        }
        [JsonProperty("imageAttributes")]
        [DefaultValue(null)]
        public String ImageAttributes
        {
            get { return this.imageAttributes; }
            set { this.imageAttributes = value; }
        }
        [JsonProperty("spriteCSS")]
        [DefaultValue(null)]
        public String SpriteCSS
        {
            get { return this.spriteCSS; }
            set { this.spriteCSS = value; }
        }

        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("htmlAttributes")]
        [DefaultValue(null)]
        public String HtmlAttributes
        {
            get { return this.htmlAttributes; }
            set { this.htmlAttributes = value; }
        }
        [JsonProperty("selected")]
        [DefaultValue(null)]
        public String Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }
        #endregion
    }
}

namespace Syncfusion.JavaScript
{
    
    public class DropDownListFieldsBuilder
    {
        private DropDownListFields fields = new DropDownListFields();
        public DropDownListFieldsBuilder(DropDownListFields fields)
        {
            this.fields = fields;
        }
        public DropDownListFieldsBuilder ID(String id)
        {
            this.fields.Id = id;
            return this;
        }
        public DropDownListFieldsBuilder ImageUrl(String imageUrl)
        {
            this.fields.ImageUrl = imageUrl;
            return this;
        }
        public DropDownListFieldsBuilder ImageAttributes(String imageAttributes)
        {
            this.fields.ImageAttributes = imageAttributes;
            return this;
        }
        public DropDownListFieldsBuilder SpriteCSS(String spriteCSS)
        {
            this.fields.SpriteCSS = spriteCSS;
            return this;
        }
        public DropDownListFieldsBuilder Value(String value)
        {
            this.fields.Value = value;
            return this;
        }
        public DropDownListFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public DropDownListFieldsBuilder HtmlAttributes(String htmlAttributes)
        {
            this.fields.HtmlAttributes = htmlAttributes;
            return this;
        }
        public DropDownListFieldsBuilder Selected(String selected)
        {
            this.fields.Selected = selected;
            return this;
        }
    }
}
