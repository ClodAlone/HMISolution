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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class NavigationControl
    {
        public NavigationControl()
        {

        }

        #region Fields

         private bool enableNavigation= false;
         private Orientation orientation=Orientation.Vertical;
         private ShapePoint absolutePosition= new ShapePoint(0,0);
         private DockPosition dockPosition = DockPosition.CenterLeft;
        
        #endregion

        #region Properties

         [JsonProperty("enableNavigation")]
         [DefaultValue(false)]
         public bool EnableNavigation
         {
             get { return this.enableNavigation; }
             set { this.enableNavigation = value; }
         }


         [JsonProperty("orientation")]
         [DefaultValue(Orientation.Vertical)]
         public Orientation Orientation
         {
             get { return this.orientation; }
             set { this.orientation = value; }
         }


         [JsonProperty("absolutePosition")]
         [DefaultValue(null)]
         public ShapePoint AbsolutePosition
         {
             get { return this.absolutePosition; }
             set { this.absolutePosition = value; }
         }


         [JsonProperty("dockPosition")]
         [DefaultValue(DockPosition.CenterLeft)]
         [JsonConverter(typeof(StringEnumConverter))]
         public DockPosition DockPosition
         {
             get { return this.dockPosition; }
             set { this.dockPosition = value; }
         }

        #endregion
    }
}
