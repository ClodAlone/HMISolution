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
    public class PointerGradient2
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
    public class PointerGradient2Builder
    {
        LinearGauge linearGauge;
        private List<PointerGradient2> pointerGradients2 = new List<PointerGradient2>();
        PointerGradient2 pointerGradient2 = new PointerGradient2();
        public PointerGradient2Builder(LinearGauge pointerGradient2)
        {
            this.linearGauge = pointerGradient2;
            this.pointerGradients2 = pointerGradient2.LinearGaugeModel.PointerGradient2;
        }
        // ColorInfo Values
        public PointerGradient2Builder ColorInfo(Action<ColorInfoBuilder> colorinfo)
        {
            var colorInfo = new List<ColorInfo>();
            pointerGradient2.ColorInfo = colorInfo;
            var builder = new ColorInfoBuilder(this.pointerGradient2);
            if (colorinfo != null)
                colorinfo.Invoke(builder);
            return this;
        }
        public void Add()
        {
            linearGauge.LinearGaugeModel.PointerGradient2.Add(pointerGradient2);
            pointerGradient2 = new PointerGradient2();
        }
    }

}
