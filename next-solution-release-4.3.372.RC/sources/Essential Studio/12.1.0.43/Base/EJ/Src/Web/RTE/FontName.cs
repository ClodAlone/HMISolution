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
    public class FontName
    {
        #region Fields
        private String text = "Segoe UI";
        private String value = "Segoe UI";
        #endregion
        #region Properties
        [JsonProperty("text")]
        [DefaultValue("Segoe UI")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("Segoe UI")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    
    public class FontNameBuilder
    {
         RTE rte = new RTE();
        private List<FontName> fontName = new List<FontName>();
        FontName fontname = new FontName();
        public FontNameBuilder(RTE fontName)
        {
            this.rte = fontName;
            this.fontName = rte.RTEModel.FontName;
        }
        public FontNameBuilder()
        {
        }
        public FontNameBuilder Text(String text)
        {
            fontname.Text = text;
            return this;
        }
        public FontNameBuilder Value(String value)
        {
            fontname.Value = value;
            return this;
        }
        public void Add()
        {
            rte.RTEModel.FontName.Add(fontname);
            fontname = new FontName();
        }
    }
}
