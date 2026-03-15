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
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapInteriorGradients
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
namespace Syncfusion.JavaScript.Olap
{
    public class OlapInteriorGradientBuilder
    {    
        OlapGauge olapGauge;
        private List<InteriorGradients> interiorGradients = new List<InteriorGradients>();
        InteriorGradients interiorGradient=new InteriorGradients();
        public OlapInteriorGradientBuilder(OlapGauge interiorGradient)
        {
            this.olapGauge=interiorGradient;
            this.interiorGradients = olapGauge.OlapGaugeModel.InteriorGradient;
        }
        // ColorInfo Values
        public OlapInteriorGradientBuilder ColorInfo(Action<CircularColorInfoBuilder> colorinfo)
        {
            var colorInfo = new List<CircularColorInfo>();            
            interiorGradient.ColorInfo = colorInfo;
            var builder = new CircularColorInfoBuilder(this.interiorGradient);
            if (colorinfo != null)
                colorinfo.Invoke(builder);
            return this;
        }
        public void Add()
        {
            olapGauge.OlapGaugeModel.InteriorGradient.Add(interiorGradient);
            interiorGradient=new InteriorGradients();            
        }

    }
}
