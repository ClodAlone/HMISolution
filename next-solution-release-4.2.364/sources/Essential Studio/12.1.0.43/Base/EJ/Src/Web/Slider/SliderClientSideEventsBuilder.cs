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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SliderClientSideEventsBuilder
    {
        private SliderProperties sliderModel;
        public SliderClientSideEventsBuilder(SliderProperties sliderProp)
        {
            sliderModel = sliderProp;
        }
        //Events
        public SliderClientSideEventsBuilder Create(String create)
        {
            sliderModel.Create = create;
            return this;
        }
        public SliderClientSideEventsBuilder Start(String start)
        {
            sliderModel.Start = start;
            return this;
        }
        public SliderClientSideEventsBuilder Stop(String stop)
        {
            sliderModel.Stop = stop;
            return this;
        }
        public SliderClientSideEventsBuilder Slide(String slide)
        {
            sliderModel.Slide = slide;
            return this;
        }
        public SliderClientSideEventsBuilder Change(String change)
        {
            sliderModel.Change = change;
            return this;
        }
        public SliderClientSideEventsBuilder Destroy(String destroy)
        {
            sliderModel.Destroy = destroy;
            return this;
        }
    }
}
