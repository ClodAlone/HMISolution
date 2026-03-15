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
    public class PointerGradient
    {
        #region fields

        //Object values
        private List<ColorInfo> colorInfo = new List<ColorInfo>();
        #endregion

        #region properties
        //object values
        [JsonProperty("colorInfo")]
        public List<ColorInfo> ColorInfo
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
    public class PointerGradientBuilder
    {
        MarkerPointers markerPointer;
        private List<PointerGradient> pointerGradients = new List<PointerGradient>();
        PointerGradient pointerGradient = new PointerGradient();
        public PointerGradientBuilder(MarkerPointers pointerGradient)
        {
            this.markerPointer = pointerGradient;
            this.pointerGradients = pointerGradient.PointerGradient;
        }
        // ColorInfo Values
        public PointerGradientBuilder ColorInfo(Action<ColorInfoBuilder> colorinfo)
        {
            var colorInfo = new List<ColorInfo>();
            pointerGradient.ColorInfo = colorInfo;
            var builder = new ColorInfoBuilder(this.pointerGradient);
            if (colorinfo != null)
                colorinfo.Invoke(builder);
            return this;
        }
        public void Add()
        {
            markerPointer.PointerGradient.Add(pointerGradient);
            pointerGradient = new PointerGradient();
        }
    }

}
