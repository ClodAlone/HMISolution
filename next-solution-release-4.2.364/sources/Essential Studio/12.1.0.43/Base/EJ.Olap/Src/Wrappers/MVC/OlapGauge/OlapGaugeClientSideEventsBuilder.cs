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
using Syncfusion.JavaScript.DataVisualization;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGaugeClientSideEventsBuilder
    {
        private OlapGaugeProperties olapGaugeModel;
        public OlapGaugeClientSideEventsBuilder(OlapGaugeProperties olapGaugeProp)
        {
            olapGaugeModel = olapGaugeProp;
        }
        public OlapGaugeClientSideEventsBuilder BeforeServiceInvoke(string beforeServiceInvoke)
        {
            olapGaugeModel.BeforeServiceInvoke = beforeServiceInvoke;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder AfterServiceInvoke(string afterServiceInvoke)
        {
            olapGaugeModel.AfterServiceInvoke = afterServiceInvoke;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder ClientSuccess(string clientSuccess)
        {
            olapGaugeModel.ClientSuccess = clientSuccess;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder ClientComplete(string clientComplete)
        {
            olapGaugeModel.ClientComplete = clientComplete;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder ClientError(string clientError)
        {
            olapGaugeModel.ClientError = clientError;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawTicks(String drawTicks)
        {
            olapGaugeModel.DrawTicks = drawTicks;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawLabels(String drawLabels)
        {
            olapGaugeModel.DrawLabels = drawLabels;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawPointerCap(String drawPointerCap)
        {
            olapGaugeModel.DrawPointerCap = drawPointerCap;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder RenderComplete(String renderComplete)
        {
            olapGaugeModel.RenderComplete = renderComplete;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawRange(String drawRange)
        {
            olapGaugeModel.DrawRange = drawRange;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawCustomLabel(String drawCustomLabel)
        {
            olapGaugeModel.DrawCustomLabel = drawCustomLabel;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder DrawIndicators(String drawIndicators)
        {
            olapGaugeModel.DrawIndicators = drawIndicators;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder MouseClick(String mouseClick)
        {
            olapGaugeModel.MouseClick = mouseClick;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder MouseClickMove(String mouseClickMove)
        {
            olapGaugeModel.MouseClickMove = mouseClickMove;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder MouseClickUp(String mouseClickUp)
        {
            olapGaugeModel.MouseClickUp = mouseClickUp;
            return this;
        }
        public OlapGaugeClientSideEventsBuilder Destroy(string destroy)
        {
            olapGaugeModel.Destroy = destroy;
            return this;
        }
    }
}
