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
using System.Drawing;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Caption
    {
        #region Fields
        private int textAngle = 0;
        private object location = new BulletLocation();
      //  private Point location = new Point(17,30);
        private String text = "";
        private object font = new BulletFont();

      //  private Font font = new Font("Segoe UI", "12px", FontStyle.Regular);
      //  private double opacity = 1;
        private object subTitle = new object();
        #endregion

        #region Properties
        [JsonProperty("subtitle")]
        public object SubTitle
        {
            get { return this.subTitle; }
            set { this.subTitle = value; }
        }

        [JsonProperty("textAngle")]
        [DefaultValue(0)]
        public int TextAngle
        {
            get { return this.textAngle; }
            set { this.textAngle = value; }
        }

        [JsonProperty("location")]
        //    [DefaultValue(new Point(110,10))]
        public object Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [JsonProperty("text")]
        [DefaultValue("")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.font; }
            set { this.font = value; }
        }

        #endregion

        #region ShouldSerialize Methods

        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new BulletLocation()))
                return true;
            else
                return false;
        }
        
        public bool ShouldSerializeSubTitle()
        {
            if (Utils.PropertyCompare(SubTitle, new SubTitle()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new BulletFont()))
                return true;
            else
                return false;
        }

        #endregion
    }

}
