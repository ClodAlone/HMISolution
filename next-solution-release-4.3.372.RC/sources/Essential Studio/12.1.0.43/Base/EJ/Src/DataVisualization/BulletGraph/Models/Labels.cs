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
    public class BulletLabels
    {
        #region Fields
        private String labelStroke = String.Empty;
        private int labelSize = 12;
        private int labelOffset = 15;
        private LabelPosition labelPosition = LabelPosition.Below;
        private object font = new BulletFont();

        #endregion

        #region Properties

        [JsonProperty("labelStroke")]
        //[DefaultValue(null)]
        public String LabelStroke
        {
            get { return this.labelStroke; }
            set { this.labelStroke = value; }
        }

        [JsonProperty("labelSize")]
        [DefaultValue(12)]
        public int LabelSize
        {
            get { return this.labelSize; }
            set { this.labelSize = value; }
        }

        [JsonProperty("labelOffset")]
        [DefaultValue(15)]
        public int LabelOffset
        {
            get { return this.labelOffset; }
            set { this.labelOffset = value; }
        }

        [JsonProperty("labelPosition")]
        [DefaultValue(LabelPosition.Below)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LabelPosition Position
        {
            get { return this.labelPosition; }
            set { this.labelPosition = value; }
        }

        [JsonProperty("font")]
        public object Font
        {
            get { return this.font; }
            set { this.font = value; }
        }

        #endregion

        #region ShouldSerialize methods
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
