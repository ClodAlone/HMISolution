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
    public class Label
    {

        #region Fields

        private bool showLabels = false;
        private bool enableSmartLabel = false;
        private string labelPath = null;
        private LabelSize smartLabelSize = LabelSize.Fixed;
        private double labelLength = 2;
        #endregion

       
        #region Properties

        [JsonProperty("showLabels")]
        [DefaultValue(false)]
        public bool ShowLabels
        {
            get { return this.showLabels; }
            set { this.showLabels = value; }
        }

        [JsonProperty("smartLabelSize")]
        [DefaultValue(LabelSize.Fixed)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LabelSize SmartLabelSize
        {
            get { return this.smartLabelSize; }
            set { this.smartLabelSize = value; }
        }

        [JsonProperty("labelLength")]
        [DefaultValue(2)]
        public double LabelLength
        {
            get { return this.labelLength; }
            set { this.labelLength = value; }
        }

        [JsonProperty("enableSmartLabel")]
        [DefaultValue(false)]
        public bool EnableSmartLabel
        {
            get { return this.enableSmartLabel; }
            set { this.enableSmartLabel = value; }
        }

        [JsonProperty("labelPath")]
        [DefaultValue(null)]
        public string LabelPath
        {
            get { return this.labelPath; }
            set { this.labelPath = value; }
        }

        #endregion

    }
}
