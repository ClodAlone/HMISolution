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
    public class ColorMappings
    {
        #region Fields

        private string color;

        #endregion

        #region Properties

        [JsonProperty("color")]
        [DefaultValue("blue")]
        public string Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        #endregion


    }

    public class RangeColorMapping : ColorMappings
    {
        #region Fields

        private double from;
        private double to;
        private List<string> gradientColors=new List<string>();

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

        [JsonProperty("gradientColors")]
        public List<string> GradientColors
        {
            get { return this.gradientColors; }
            set { this.gradientColors = value; }
        }
        #endregion
        
    }

    public class EqualColorMapping : ColorMappings
    {
        #region Fields

        private object value;

        #endregion

        #region Properties

        [JsonProperty("value")]
        [DefaultValue(null)]
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion

    }
}
