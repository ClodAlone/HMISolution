#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class NavigationControlBuilder
    {
        public NavigationControl navigationControl;

        public MapProperties mapProperties;
        
        public NavigationControlBuilder(NavigationControl navcontrol, MapProperties map)
        {
            this.navigationControl = navcontrol;
            this.mapProperties = map;
            this.mapProperties.NavigationControl = this.navigationControl;
        }

        public NavigationControlBuilder EnableNavigation(bool enableNavigation)
        {
            this.navigationControl.EnableNavigation = enableNavigation;
            return this;
        }

        public NavigationControlBuilder Orientation(Orientation Orientation)
        {
            this.navigationControl.Orientation = Orientation;
            return this;
        }

        public NavigationControlBuilder AbsolutePosition(ShapePoint absolutePosition)
        {
            this.navigationControl.AbsolutePosition = absolutePosition;
            return this;
        }

        public NavigationControlBuilder DockPosition(DockPosition dockPosition)
        {
            this.navigationControl.DockPosition = dockPosition;
            return this;
        }
    }
}
