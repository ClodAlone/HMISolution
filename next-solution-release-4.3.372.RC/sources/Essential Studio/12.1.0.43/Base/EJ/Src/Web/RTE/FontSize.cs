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
    public class FontSize
    {
        #region Fields
        private String text = "1";
        private String value = "1";
        #endregion
        #region Properties
        [JsonProperty("text")]
        [DefaultValue("1")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("1")]
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
    
    public class FontSizeBuilder
    {
        RTE rte = new RTE();
        private List<FontSize> fontSize = new List<FontSize>();
        FontSize fontsize = new FontSize();
        public FontSizeBuilder(RTE fontSize)
        {
            this.rte = fontSize;
            this.fontSize = rte.RTEModel.FontSize;
        }
        public FontSizeBuilder()
        {
        }
        public FontSizeBuilder Text(String text)
        {
            fontsize.Text = text;
            return this;
        }
        public FontSizeBuilder Value(String value)
        {
            fontsize.Value = value;
            return this;
        }
        public void Add()
        {
            rte.RTEModel.FontSize.Add(fontsize);
            fontsize= new FontSize();
        }
    }
}
