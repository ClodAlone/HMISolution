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
    public class IndicatorLocation
    {
        #region Fields
        private int x = 0;
        private int y = 0;
        #endregion

        #region Properties
        [JsonProperty("x")]
        [DefaultValue(0)]
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        [JsonProperty("y")]
        [DefaultValue(0)]
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class IndicatorLocationBuilder
    {
        CircularIndicators indicator;
        private object indicatorlocation = new IndicatorLocation();                                   
        IndicatorLocation indicatorLocation = new IndicatorLocation();
        public IndicatorLocationBuilder(CircularIndicators indicatorLocation)
        {
            this.indicator = indicatorLocation;
            this.indicatorlocation = indicatorLocation.IndicatorLocation;
        }

        //Integer values
        public IndicatorLocationBuilder X(int x)
        {
            this.indicatorLocation.X = x;
            return this;
        }
        public IndicatorLocationBuilder Y(int y)
        {
            this.indicatorLocation.Y = y;
            return this;
        }
        //public void Add()
        //{
        //    indicator.IndicatorLocation.
        //    scales.CustomLabel.Add(customLabels);
        //    customLabels = new CustomLabel();
        //}

    }
}
