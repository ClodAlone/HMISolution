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
    public class ColorInfo
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
    public class ColorInfoBuilder
    {
        private List<ColorInfo> colorinfo = new List<ColorInfo>();
        ColorInfo colorInfo = new ColorInfo();

        PointerGradient1 pointerGradient1;
        public ColorInfoBuilder(PointerGradient1 colorInfo)
        {
            this.pointerGradient1 = colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }
        PointerGradient2 pointerGradient2;
        public ColorInfoBuilder(PointerGradient2 colorInfo)
        {
            this.pointerGradient2 = colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }
        ScaleBarGradient scalebarGradient;
        public ColorInfoBuilder(ScaleBarGradient colorInfo)
        {
            this.scalebarGradient= colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }

        PointerGradient pointerGradient;
        public ColorInfoBuilder(PointerGradient colorInfo)
        {
            this.pointerGradient = colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }

        RangeGradient rangeGradient;
        public ColorInfoBuilder(RangeGradient colorInfo)
        {
            this.rangeGradient = colorInfo;
            this.colorinfo = colorInfo.ColorInfo;
        }

        //String Values
        public ColorInfoBuilder Color(String color)
        {
            this.colorInfo.Color = color;
            return this;
        }
        //Integer values
        public ColorInfoBuilder ColorStop(int colorStop)
        {
            this.colorInfo.ColorStop = colorStop;
            return this;
        }

        public void Add()
        {
            colorinfo.Add(colorInfo);
            colorInfo = new ColorInfo();
        }

    }
}
