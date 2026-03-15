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
    public class SubGauge
    {
                             
        #region fields

        //Integer values
        private int subGaugeHeight=150;
        private int subGaugeWidth=150;

        //object values       
        private CircularLocation location = new CircularLocation();

        #endregion

        #region Properties
        //Integer values
        [JsonProperty("subGaugeHeight")]
        [DefaultValue(150)]
        public int SubGaugeHeight
        {
            get { return this.subGaugeHeight; }
            set { this.subGaugeHeight = value; }
        }
        [JsonProperty("subGaugeWidth")]
        [DefaultValue(150)]
        public int SubGaugeWidth
        {
            get { return this.subGaugeWidth; }
            set { this.subGaugeWidth = value; }
        }
        //Object values
        [JsonProperty("location")]
        public CircularLocation Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        #endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new CircularLocation()))
                return true;
            else
                return false;
        }   
        #endregion

    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class SubGaugeBuilder
    {
        CircularScales scales;
        private List<SubGauge> subgauge = new List<SubGauge>();                    
        SubGauge subGauge = new SubGauge();
        public SubGaugeBuilder(CircularScales subGauge)
        {
            this.scales = subGauge;
            this.subgauge = subGauge.SubGauge;
        }

        //Integers
        public SubGaugeBuilder SubGaugeHeight(int subGaugeHeight)
        {
            subGauge.SubGaugeHeight = subGaugeHeight;
            return this;
        }
        public SubGaugeBuilder SubGaugeWidth(int subGaugeWidth)
        {
            subGauge.SubGaugeWidth = subGaugeWidth;
            return this;
        }
        // Location Values
        public SubGaugeBuilder Location(Action<CircularLocationBuilder> location)
        {            
            var builder = new CircularLocationBuilder(this.subGauge);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.SubGauge.Add(subGauge);
            subGauge = new SubGauge();            
        }
    }
}
