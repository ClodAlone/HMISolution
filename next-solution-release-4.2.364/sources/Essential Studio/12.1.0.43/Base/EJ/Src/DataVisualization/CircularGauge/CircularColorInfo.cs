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
    public class CircularColorInfo
    {
        #region fields

        //Integer values
        private int colorStop = 0;

        //String values
        private string color = "#FFFFFF";
        #endregion

        #region properties
        //Integer values
        [JsonProperty("colorStop")]
        [DefaultValue(0)]
        public int ColorStop
        {
            get { return this.colorStop; }
            set { this.colorStop = value; }
        }
        [JsonProperty("color")]
        [DefaultValue("#FFFFFF")]
        public String Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        #endregion

    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularColorInfoBuilder
    {
        private List<CircularColorInfo> colorinfo = new List<CircularColorInfo>();
        CircularColorInfo colorInfo = new CircularColorInfo();

        InteriorGradients interiorGradient;
        public CircularColorInfoBuilder(InteriorGradients colorInfo)
        {
            this.interiorGradient = colorInfo;
            this.colorinfo = interiorGradient.ColorInfo;
        }

        CapInteriorGradient capinteriorGradients;
        public CircularColorInfoBuilder(CapInteriorGradient colorInfo)
        {
            this.capinteriorGradients = colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }

        CircularPointerGradient pointerGradient;
        public CircularColorInfoBuilder(CircularPointerGradient colorInfo)
        {
            this.pointerGradient = colorInfo;
            this.colorinfo = pointerGradient.ColorInfo;
        }

        CircularRangeGradient rangeGradient;
        public CircularColorInfoBuilder(CircularRangeGradient colorInfo)
        {
            this.rangeGradient = colorInfo;
            this.colorinfo = rangeGradient.ColorInfo;
        }

        //String Values
        public CircularColorInfoBuilder Color(String color)
        {
            this.colorInfo.Color = color;
            return this;
        }
        //Integer values
        public CircularColorInfoBuilder ColorStop(int colorStop)
        {
            this.colorInfo.ColorStop = colorStop;
            return this;
        }

        public void Add()
        {
            colorinfo.Add(colorInfo);
            colorInfo = new CircularColorInfo();
        }

    }
}
