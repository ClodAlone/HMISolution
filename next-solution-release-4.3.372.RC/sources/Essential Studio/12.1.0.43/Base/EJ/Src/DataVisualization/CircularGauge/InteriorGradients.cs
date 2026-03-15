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
    public class InteriorGradients
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
    public class InteriorGradientBuilder
    {    
        CircularGauge cirGauge;
        private List<InteriorGradients> interiorGradients = new List<InteriorGradients>();
        InteriorGradients interiorGradient=new InteriorGradients();
        public InteriorGradientBuilder(CircularGauge interiorGradient)
        {
            this.cirGauge=interiorGradient;
            this.interiorGradients = cirGauge.CircularGaugeModel.InteriorGradient;
        }
        // ColorInfo Values
        public InteriorGradientBuilder ColorInfo(Action<CircularColorInfoBuilder> colorinfo)
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
            cirGauge.CircularGaugeModel.InteriorGradient.Add(interiorGradient);
            interiorGradient=new InteriorGradients();            
        }

    }
}
