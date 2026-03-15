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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class LinearLocation
    {
        #region LocationFields
        private int x = 50;
        private int y = 50;
        #endregion

        #region Properties
        [JsonProperty("x")]
        [DefaultValue(50)]
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        [JsonProperty("y")]
        [DefaultValue(50)]
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
    public class LinearLocationBuilder
    {
        
        private LinearLocation locations = new LinearLocation();
        LinearLocation location = new LinearLocation();
        Scales scales;
        public LinearLocationBuilder(Scales location)
        {
            this.scales = location;
            this.locations = this.scales.ScaleLocation;
        }

        CustomLabel customlabels;
        public LinearLocationBuilder(CustomLabel location)
        {
            this.customlabels = location;
            this.locations = customlabels.Location;
        }

        Indicators indicators;
        public LinearLocationBuilder(Indicators location)
        {
            this.indicators = location;
            this.locations = indicators.Location;
        }
        public LinearLocationBuilder X(int x)
        {
            this.locations.X = x;
            return this;
        }
        public LinearLocationBuilder Y(int y)
        {
            this.locations.Y = y;
            return this;
        }
        

    }
}
