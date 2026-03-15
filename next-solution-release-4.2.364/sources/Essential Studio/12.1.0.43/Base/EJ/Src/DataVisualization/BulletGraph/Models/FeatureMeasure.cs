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
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class FeatureMeasure
    {
        #region Fields
        private double value=0;
        private double comparativeMeasureValue=0;
        private String category = "";
        #endregion

        #region Properties
        [JsonProperty("value")]
        [DefaultValue(0)]
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        [JsonProperty("comparativeMeasureValue")]
        [DefaultValue(0)]
        public double ComparativeMeasure
        {
            get { return this.comparativeMeasureValue; }
            set { this.comparativeMeasureValue = value; }
        }

        [JsonProperty("category")]
        [DefaultValue("")]
        public String Category
        {
            get { return this.category; }
            set { this.category = value; }
        }
        #endregion

    }
}
