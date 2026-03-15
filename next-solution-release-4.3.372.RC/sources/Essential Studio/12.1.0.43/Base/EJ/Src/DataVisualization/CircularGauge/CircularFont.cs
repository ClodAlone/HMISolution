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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class CircularFont
    {
        #region FontFields
        //String Values
        private string size = "11px";
        private string fontFamily = "Arial";
        private string fontStyle = "Bold";
        #endregion

        #region FontProperties
        //String Values
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
        [JsonProperty("fontStyle")]
        [DefaultValue("Bold")]
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
    public class CircularFontBuilder
    {
        private CircularFont fonts = new CircularFont();
        CircularFont font = new CircularFont();

        CircularLabels labels;
        public CircularFontBuilder(CircularLabels font)
        {
            this.labels = font;
            this.fonts = font.Font;
        }

        CircularCustomLabel customlabels;
        public CircularFontBuilder(CircularCustomLabel font)
        {
            this.customlabels = font;
            this.fonts = font.Font;
        }

        CircularStateRanges stateranges;
        public CircularFontBuilder(CircularStateRanges font)
        {
            this.stateranges = font;
            this.fonts = font.Font;
        }
        //String Values
        public CircularFontBuilder Size(String size)
        {
            this.fonts.Size = size;            
            return this;
        }
        public CircularFontBuilder FontFamily(String fontFamily)
        {
            this.fonts.FontFamily = fontFamily;            
            return this;
        }
        public CircularFontBuilder FontStyle(String fontStyle)
        {
            this.fonts.FontStyle = fontStyle;
            return this;
        }
        
    }
}
