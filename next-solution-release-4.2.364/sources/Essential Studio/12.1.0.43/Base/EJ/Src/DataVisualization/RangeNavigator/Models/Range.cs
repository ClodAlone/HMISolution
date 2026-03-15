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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class NavigatorRange
   {
       private string m_start=null;
       private string m_end=null;

       [JsonProperty("start")]
       [DefaultValue(null)]
       public string Start
       {
           get { return this.m_start; }
           set { this.m_start = value; }
       }

       [JsonProperty("end")]
       [DefaultValue(null)]
       public string End
       {
           get { return this.m_end; }
           set { this.m_end = value; }
       }

   }
   public class ZoomCordinates
   {
       private string m_zoomPosition = "0";
       private string m_zoomFactor = "1";

       [JsonProperty("start")]
       [DefaultValue("0")]
       public string ZoomPosition
       {
           get { return this.m_zoomPosition; }
           set { this.m_zoomPosition = value; }
       }

       [JsonProperty("end")]
       [DefaultValue("1")]
       public string ZoomFactor
       {
           get { return this.m_zoomFactor; }
           set { this.m_zoomFactor = value; }
       }

   }
   public class SelectedRange
   {
       private string m_start = null;
       private string m_end = null;

       [JsonProperty("start")]
       [DefaultValue(null)]
       public string Start
       {
           get { return this.m_start; }
           set { this.m_start = value; }
       }

       [JsonProperty("end")]
       [DefaultValue(null)]
       public string End
       {
           get { return this.m_end; }
           set { this.m_end = value; }
       }

   }
}
