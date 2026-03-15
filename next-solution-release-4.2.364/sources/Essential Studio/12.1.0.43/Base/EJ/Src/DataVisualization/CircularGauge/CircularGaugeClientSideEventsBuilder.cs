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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularGaugeClientSideEventsBuilder
    {
        private CircularGaugeProperties CircularGaugeModel;
        public CircularGaugeClientSideEventsBuilder(CircularGaugeProperties circularguageProp)
        {
            CircularGaugeModel = circularguageProp;
        }

        //Events        
        public CircularGaugeClientSideEventsBuilder DrawTicks(String drawTicks)
        {
            CircularGaugeModel.DrawTicks = drawTicks;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder DrawLabels(String drawLabels)
        {
            CircularGaugeModel.DrawLabels = drawLabels;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder DrawPointerCap(String drawPointerCap)
        {
            CircularGaugeModel.DrawPointerCap = drawPointerCap;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder RenderComplete(String renderComplete)
        {
            CircularGaugeModel.RenderComplete = renderComplete;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder DrawRange(String drawRange)
        {
            CircularGaugeModel.DrawRange = drawRange;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder DrawCustomLabel(String drawCustomLabel)
        {
            CircularGaugeModel.DrawCustomLabel = drawCustomLabel;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder DrawIndicators(String drawIndicators)
        {
            CircularGaugeModel.DrawIndicators = drawIndicators;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder MouseClick(String mouseClick)
        {
            CircularGaugeModel.MouseClick = mouseClick;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder MouseClickMove(String mouseClickMove)
        {
            CircularGaugeModel.MouseClickMove = mouseClickMove;
            return this;
        }
        public CircularGaugeClientSideEventsBuilder MouseClickUp(String mouseClickUp)
        {
            CircularGaugeModel.MouseClickUp = mouseClickUp;
            return this;
        }
    }
}
