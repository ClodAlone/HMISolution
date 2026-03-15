#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class TreeMapRangeColorMapping : ColorMappings
    {
        #region Fields

       
        private double from;

        private double to;

        private string legendlabel;

        #endregion

        #region Properties

        [JsonProperty("from")]
        [DefaultValue(-1)]
        public double From
        {
            get { return this.from; }
            set { this.from = value; }
        }

        [JsonProperty("to")]
        [DefaultValue(-1)]
        public double To
        {
            get { return this.to; }
            set { this.to = value; }
        }


        [JsonProperty("legendLabel")]
        [DefaultValue(null)]
        public string Legendlabel
        {
            get { return this.legendlabel; }
            set { this.legendlabel = value; }
        }

       
        #endregion

    }

    public class TreeMapDesaturationColorMapping : ColorMappings
    {
        #region Fields

        private double from;

        private double to;
       
        private double rangeMinimum;

        private double rangeMaximum;

        #endregion

        #region Properties

        [JsonProperty("from")]
        [DefaultValue(0)]
        public double From
        {
            get { return this.from; }
            set { this.from = value; }
        }

        [JsonProperty("to")]
        [DefaultValue(0)]
        public double To
        {
            get { return this.to; }
            set { this.to = value; }
        }      

        [JsonProperty("rangeMinimum")]
        [DefaultValue(0)]
        public double RangeMinimum
        {
            get { return this.rangeMinimum; }
            set { this.rangeMinimum = value; }
        }


        [JsonProperty("rangeMaximum")]
        [DefaultValue(0)]
        public double RangeMaximum
        {
            get { return this.rangeMaximum; }
            set { this.rangeMaximum = value; }
        }

        #endregion

    }

    public class TreeMapUniColorMapping 
    {
        private string color;

        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get { return this.color; }
            set { this.color = value; }
        }
    }

    public class TreeMapPaletteColorMapping : ColorMappings
    {
        private List<string> colors;

        public TreeMapPaletteColorMapping()
        {
            this.colors = new List<string>();
        }

        [JsonProperty("colors")]
        public List<string> Colors
        {
            get { return this.colors; }
            set { this.colors = value; }
        }
    }
  
}
