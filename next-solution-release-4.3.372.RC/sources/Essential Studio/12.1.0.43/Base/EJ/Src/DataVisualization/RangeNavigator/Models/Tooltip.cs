#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class Tooltip
    {
      #region Field

       private bool m_visible = true;
       private string m_labelFormat = "dd/MM/yyyy";
       private string m_tooltipDisplayMode = "always";
       private string m_tooltipPosition = "top";
       private string m_backgroundColor = null;

       private object m_labelstyles = null;

       #endregion

       #region Property
       [JsonProperty("visible")]
       [DefaultValue(true)]
       public bool Visible
       {
           get { return this.m_visible; }
           set { this.m_visible = value; }
       }
       [JsonProperty("labelFormat")]
       [DefaultValue("dd / MM / yyyy")]
       public string LabelFormat
       {
           get { return this.m_labelFormat; }
           set { this.m_labelFormat = value; }
       }
       [JsonProperty("tooltipDisplayMode")]
       [DefaultValue("always")]
       public string TooltipDisplayMode
       {
           get { return this.m_tooltipDisplayMode; }
           set { this.m_tooltipDisplayMode = value; }
       }
       [JsonProperty("tooltipPosition")]
       [DefaultValue("top")]
       public string TooltipPosition
       {
           get { return this.m_tooltipPosition; }
           set { this.m_tooltipPosition = value; }
       }
       [JsonProperty("backgroundColor")]
       [DefaultValue(null)]
       public string BackgroundColor
       {
           get { return this.m_backgroundColor; }
           set { this.m_backgroundColor = value; }
       }
       [JsonProperty("labelstyles")]
      
       public object Labelstyles
       {
           get { return this.m_labelstyles; }
           set { this.m_labelstyles = value; }
       }
       #endregion

        #region ShouldSerialize
       public bool ShouldSerializeLabelstyles()
       {
           if (Utils.PropertyCompare(Labelstyles, new LabelStyles()))
               return true;
           else
               return false;
       }
        #endregion
    }
}
