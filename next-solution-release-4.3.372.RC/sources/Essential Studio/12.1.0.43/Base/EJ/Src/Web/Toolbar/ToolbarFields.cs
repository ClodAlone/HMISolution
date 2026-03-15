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
    public class ToolbarFields
    {
        #region Fields
        private String id = null;
        private String tooltipText = null;
        private String imageUrl = null;
        private String imageAttributes = null;
        private String spriteCSS = null;
        private String text = null;
        private String htmlAttributes = null;
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
        [JsonProperty("tooltipText")]
        [DefaultValue(null)]
        public String TooltipText
        {
            get { return this.tooltipText; }
            set { this.tooltipText = value; }
        }
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("htmlAttributes")]
        [DefaultValue(null)]
        public String HtmlAttributes
        {
            get { return this.htmlAttributes; }
            set { this.htmlAttributes = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{

    public class ToolbarFieldsBuilder
    {
        private ToolbarFields fields = new ToolbarFields();
        public ToolbarFieldsBuilder(ToolbarFields fields)
        {
            this.fields = fields;
        }
        public ToolbarFieldsBuilder ID(String id)
        {
            this.fields.Id = id;
            return this;
        }
        public ToolbarFieldsBuilder ImageUrl(String imageUrl)
        {
            this.fields.ImageUrl = imageUrl;
            return this;
        }
        public ToolbarFieldsBuilder ImageAttributes(String imageAttributes)
        {
            this.fields.ImageAttributes = imageAttributes;
            return this;
        }
        public ToolbarFieldsBuilder SpriteCSS(String spriteCSS)
        {
            this.fields.SpriteCSS = spriteCSS;
            return this;
        }
        public ToolbarFieldsBuilder TooltipText(String tooltipText)
        {
            this.fields.TooltipText = tooltipText;
            return this;
        }
        public ToolbarFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public ToolbarFieldsBuilder HtmlAttributes(String htmlAttributes)
        {
            this.fields.HtmlAttributes = htmlAttributes;
            return this;
        }
    }
}
