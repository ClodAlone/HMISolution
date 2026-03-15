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
    public class CircularRangeGradient
    {
        #region fields

        //Object values
        private List<CircularColorInfo> colorInfo = new List<CircularColorInfo>();
        #endregion

        #region properties
        //object values
        [JsonProperty("colorInfo")]
        public List<CircularColorInfo> ColorInfo
        {
            get { return this.colorInfo; }
            set { this.colorInfo = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeColorInfo()
        {
            if (ColorInfo.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularRangeGradientBuilder
    {
        CircularRanges ranges;
        private List<CircularRangeGradient> rangeGradients = new List<CircularRangeGradient>();
        CircularRangeGradient rangeGradient = new CircularRangeGradient();
        public CircularRangeGradientBuilder(CircularRanges rangeGradient)
        {
            this.ranges = rangeGradient;
            this.rangeGradients = rangeGradient.RangeGradient;
        }
        // ColorInfo Values
        public CircularRangeGradientBuilder ColorInfo(Action<CircularColorInfoBuilder> colorinfo)
        {
            var colorInfo = new List<CircularColorInfo>();
            rangeGradient.ColorInfo = colorInfo;
            var builder = new CircularColorInfoBuilder(this.rangeGradient);
            if (colorinfo != null)
                colorinfo.Invoke(builder);
            return this;
        }
        public void Add()
        {
            ranges.RangeGradient.Add(rangeGradient);
            rangeGradient = new CircularRangeGradient();
        }
    }

}
