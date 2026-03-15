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
    public class BubbleSetting
    {
        #region Fields
          private double minSize = 10;
          private double maxSize = 20;
		  private string bubbleColor ="gray";
          private string bubbleColorValuepath=null;
          private string bubbleValuepath= null;
          private string tooltipTemplate = null;
          private ColorMapping colorMappings = null;
          private bool showToolTip = false;

        #endregion

        public BubbleSetting()
        {
        }

        #region Properties

          [JsonProperty("bubbleColor")]
          [DefaultValue("gray")]
          public string BubbleColor
          {
              get { return this.bubbleColor; }
              set { this.bubbleColor = value; }
          }

          [JsonProperty("bubbleColorValuepath")]
          [DefaultValue(null)]
          public string BubbleColorValuepath
          {
              get { return this.bubbleColorValuepath; }
              set { this.bubbleColorValuepath = value; }
          }

          [JsonProperty("bubbleValuepath")]
          [DefaultValue(null)]
          public string BubbleValuepath
          {
              get { return this.bubbleValuepath; }
              set { this.bubbleValuepath = value; }
          }


          [JsonProperty("minSize")]
          [DefaultValue(10)]
          public double MinSize
          {
              get { return this.minSize; }
              set { this.minSize = value; }
          }

          [JsonProperty("maxSize")]
          [DefaultValue(20)]
          public double MaxSize
          {
              get { return this.maxSize; }
              set { this.maxSize = value; }
          }

          [JsonProperty("colorMappings")]
          public ColorMapping ColorMappings
          {
              get { return this.colorMappings; }
              set { this.colorMappings = value; }
          }

          [JsonProperty("tooltipTemplate")]
          [DefaultValue(null)]
          public string TooltipTemplate
          {
              get { return this.tooltipTemplate; }
              set { this.tooltipTemplate = value; }
          }

          [JsonProperty("showToolTip")]
          [DefaultValue(false)]
          public bool ShowToolTip
          {
              get { return this.showToolTip; }
              set { this.showToolTip = value; }
          }

        #endregion
    }
}
