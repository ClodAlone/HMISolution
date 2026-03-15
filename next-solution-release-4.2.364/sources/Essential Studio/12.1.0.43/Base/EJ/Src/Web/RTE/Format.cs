#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class Format
    {
        #region Fields
        private String text = "Paragraph";
        private String value = "p";
        private String spriteCSS = "Segoe UI";
        #endregion
        #region Properties
        [JsonProperty("text")]
        [DefaultValue("Paragraph")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("p")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("spriteCSS")]
        [DefaultValue("e-paragraph")]
        public String SpriteCSS
        {
            get { return this.spriteCSS; }
            set { this.spriteCSS = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    
    public class FormatBuilder
    {
        RTE rte = new RTE();
        private List<Format> formatValue= new List<Format>();
        Format format = new Format();
        public FormatBuilder(RTE formatValue)
        {
            this.rte = formatValue;
            this.formatValue = rte.RTEModel.Format;
        }
        public FormatBuilder()
        {
        }
        public FormatBuilder Text(String text)
        {
            format.Text = text;
            return this;
        }
        public FormatBuilder SpriteCSS(String spriteCSS)
        {
            format.SpriteCSS = spriteCSS;
            return this;
        }
        public void Add()
        {
            rte.RTEModel.Format.Add(format);
            format = new Format();
        }
    }
}
