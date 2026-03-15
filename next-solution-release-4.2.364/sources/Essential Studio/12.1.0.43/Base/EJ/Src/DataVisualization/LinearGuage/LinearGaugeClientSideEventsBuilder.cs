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
    public class LinearGaugeClientSideEventsBuilder
    {
        private LinearGaugeProperties LinearGaugeModel;
        public LinearGaugeClientSideEventsBuilder(LinearGaugeProperties lineargaugeProp)
        {
            LinearGaugeModel = lineargaugeProp;
        }
        //Events
        public LinearGaugeClientSideEventsBuilder Create(String create)
        {
            LinearGaugeModel.Create = create;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawTicks(String drawTicks)
        {
            LinearGaugeModel.DrawTicks = drawTicks;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawLabels(String drawLabels)
        {
            LinearGaugeModel.DrawLabels = drawLabels;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawBarPointers(String drawBarPointers)
        {
            LinearGaugeModel.DrawBarPointers = drawBarPointers;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawMarkerPointers(String drawMarkerPointers)
        {
            LinearGaugeModel.DrawMarkerPointers = drawMarkerPointers;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawRange(String drawRange)
        {
            LinearGaugeModel.DrawRange = drawRange;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawCustomLabel(String drawCustomLabel)
        {
            LinearGaugeModel.DrawCustomLabel = drawCustomLabel;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder DrawIndicators(String drawIndicators)
        {
            LinearGaugeModel.DrawIndicators = drawIndicators;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder Load(String load)
        {
            LinearGaugeModel.Load = load;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder Init(String init)
        {
            LinearGaugeModel.Init = init;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder RenderComplete(String renderComplete)
        {
            LinearGaugeModel.RenderComplete = renderComplete;
            return this;
        }
        public LinearGaugeClientSideEventsBuilder Destroy(String destroy)
        {
            LinearGaugeModel.Destroy = destroy;
            return this;
        }
    }
}
