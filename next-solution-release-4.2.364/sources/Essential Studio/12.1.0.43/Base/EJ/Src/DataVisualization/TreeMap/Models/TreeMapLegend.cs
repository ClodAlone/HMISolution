#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class TreeMapLegend
    {

        #region Fields

        private string legendTemplate=null;
        private double legendIconHeight = 15;
        private double legendIconWidth = 15;

        #endregion

        #region Properties

        [JsonProperty("legendIconHeight")]
        [DefaultValue(15)]
        public double LegendIconHeight
        {
            get { return this.legendIconHeight; }
            set { this.legendIconHeight = value; }
        }

        [JsonProperty("legendIconWidth")]
        [DefaultValue(15)]
        public double LegendIconWidth
        {
            get { return this.legendIconWidth; }
            set { this.legendIconWidth = value; }
        }

        [JsonProperty("legendTemplate")]
        [DefaultValue(null)]
        public string LegendTemplate
        {
            get { return this.legendTemplate; }
            set { this.legendTemplate = value; }
        }


        #endregion
    }
}
