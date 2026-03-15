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
    public class BulletFont : Font
    {
        #region Fields
        private String color = String.Empty;
        private String fontWeight = "Regular";
        private double opacity = 1;
        #endregion

        #region Properties
        [JsonProperty("color")]
        //[DefaultValue(null)]
        public String FontColor
        {
            get { return this.color; }
            set { this.color = value; }
        }

        [JsonProperty("fontWeight")]
        [DefaultValue("regular")]
        public String FontWeight
        {
            get { return this.fontWeight; }
            set { this.fontWeight = value; }
        }

        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public double Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }

        
        #endregion
    }
}
