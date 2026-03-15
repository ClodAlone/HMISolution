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
using System.Drawing;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class QualitativeRanges
    {
        #region Fields
        private double rangeEnd;
        private String rangeStroke = String.Empty; 
        private double rangeOpacity = 1;
        #endregion

        #region Properties
        [JsonProperty("rangeEnd")]
        [DefaultValue(null)]
        public double RangeEnd
        {
            get { return this.rangeEnd; }
            set { this.rangeEnd = value; }
        }

        [JsonProperty("rangeStroke")]
        //[DefaultValue(null)]
        public String RangeStroke
        {
            get { return this.rangeStroke; }
            set { this.rangeStroke = value; }
        }

        [JsonProperty("rangeOpacity")]
        [DefaultValue(1)]
        public double RangeOpacity
        {
            get { return this.rangeOpacity; }
            set { this.rangeOpacity = value; }
        }
        #endregion
    }

   
}
