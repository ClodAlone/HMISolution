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
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;
namespace Syncfusion.JavaScript.DataVisualization
{
    public class DigitalGaugeClientSideEventsBuilder
    {
        private DigitalGaugeProperties digitalgaugeModel;
        public DigitalGaugeClientSideEventsBuilder(DigitalGaugeProperties digitalgaugeProp)
        {
            digitalgaugeModel = digitalgaugeProp;
        }
        //Events
        public DigitalGaugeClientSideEventsBuilder Create(String create)
        {
            digitalgaugeModel.Create = create;
            return this;
        }
        public DigitalGaugeClientSideEventsBuilder Init(String init)
        {
            digitalgaugeModel.Init = init;
            return this;
        }
        public DigitalGaugeClientSideEventsBuilder Load(String load)
        {
            digitalgaugeModel.Load = load;
            return this;
        }
        public DigitalGaugeClientSideEventsBuilder RenderComplete(String renderComplete)
        {
            digitalgaugeModel.RenderComplete =renderComplete;
            return this;
        }
        public DigitalGaugeClientSideEventsBuilder Destroy(String destroy)
        {
            digitalgaugeModel.Destroy = destroy;
            return this;
        }
        
    }
}
