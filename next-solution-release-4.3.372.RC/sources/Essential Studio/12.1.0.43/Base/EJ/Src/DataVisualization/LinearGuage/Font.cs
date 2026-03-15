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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Font
    {
        #region FontFields
        //String Values
        private String size = "12px";
        private String fontFamily = "Segoe UI";
        private String fontStyle = "Normal";
        #endregion

        #region FontProperties
        //String Values
        [JsonProperty("size")]
        [DefaultValue(null)]
        public String Size
        {
            get { return this.size; }
            set { this.size = value; }
        }
        [JsonProperty("fontFamily")]
        [DefaultValue(null)]
        public String FontFamily
        {
            get { return this.fontFamily; }
            set { this.fontFamily = value; }
        }
        [JsonProperty("fontStyle")]
        [DefaultValue(null)]
        public String FontStyle
        {
            get { return this.fontStyle; }
            set { this.fontStyle = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class FontBuilder
    {
        private Font fonts = new Font();
        Font font = new Font();
        Labels labels;
        public FontBuilder(Labels font)
        {
            this.labels = font;
            this.fonts = labels.Font;
        }

        CustomLabel customlabels;
        public FontBuilder(CustomLabel font)
        {
            this.customlabels = font;
            this.fonts = font.Font;
        }

        Indicators indicators;
        public FontBuilder(Indicators font)
        {
            this.indicators = font;
            this.fonts = font.Font;
        }
        //String Values
        public FontBuilder Size(String size)
        {
            this.fonts.Size = size;
            return this;
        }
        public FontBuilder FontFamily(String fontFamily)
        {
            this.fonts.FontFamily = fontFamily;
            return this;
        }
        public FontBuilder FontStyle(String fontStyle)
        {
            this.fonts.FontStyle = fontStyle;
            return this;
        }

    }
}
