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
using System.Threading.Tasks;

namespace Syncfusion.EJ.DataVisualization
{
    public partial class ClientSideEventsBuilder
    {
        private MapProperties mapModel;
        public ClientSideEventsBuilder(MapProperties mapProp)
        {
            mapModel = mapProp;
        }
        public ClientSideEventsBuilder ZoomedIn(String zoomedIn)
        {
            mapModel.ZoomedIn = zoomedIn;
            return this;
        }

        public ClientSideEventsBuilder ZoomedOut(String zoomedOut)
        {
            mapModel.ZoomedOut = zoomedOut;
            return this;
        }

        public ClientSideEventsBuilder Panning(String panning)
        {
            mapModel.Panning = panning;
            return this;
        }

        public ClientSideEventsBuilder Panned(String panned)
        {
            mapModel.Panned = panned;
            return this;
        }

        public ClientSideEventsBuilder ShapeSelected(String shapeSelected)
        {
            mapModel.ShapeSelected = shapeSelected;
            return this;
        }
    }
}
