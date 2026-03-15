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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class DigitalGaugeFont
    {
        #region Fields
        private String size = "11px";
        private String fontFamily = "Arial";
        private FontStyle fontStyle = FontStyle.Italic;
        #endregion

        #region Properties
        [JsonProperty("size")]
        [DefaultValue("11px")]
        public String Size
        {
            get { return this.size; }
            set { this.size = value; }
        }
        [JsonProperty("fontFamily")]
        [DefaultValue("Arial")]
        public String FontFamily
        {
            get { return this.fontFamily; }
            set { this.fontFamily = value; }
        }
         //Enum values
        [JsonProperty("fontStyle")]
        [DefaultValue(FontStyle.Italic)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FontStyle FontStyle
        {
            get { return this.fontStyle; }
            set { this.fontStyle = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class DigitalGaugeFontBuilder
    {
        private DigitalGaugeFont fnt = new DigitalGaugeFont();
        public DigitalGaugeFontBuilder(DigitalGaugeFont fnt)
        {
            this.fnt = fnt;
        }
        public DigitalGaugeFontBuilder Size(String size)
        {
            this.fnt.Size = size;
            return this;
        }
        public DigitalGaugeFontBuilder FontFamily(String fontFamily)
        {
            this.fnt.FontFamily = fontFamily;
            return this;
        }
        public DigitalGaugeFontBuilder FontStyle(FontStyle fontStyle)
        {
            this.fnt.FontStyle = fontStyle;
            return this;
        }

    }
}
