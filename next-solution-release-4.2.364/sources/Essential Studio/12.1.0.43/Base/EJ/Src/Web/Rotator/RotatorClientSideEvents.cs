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
    public class RotatorClientSideEventsBuilder
    {
        private RotatorProperties rotatorModel;
        public RotatorClientSideEventsBuilder(RotatorProperties rotatorProp)
        {
            rotatorModel = rotatorProp;
        }
        //Events
        public RotatorClientSideEventsBuilder Create(String create)
        {
            rotatorModel.Create = create;
            return this;
        }
        public RotatorClientSideEventsBuilder Change(String change)
        {
            rotatorModel.Change = change;
            return this;
        }
        public RotatorClientSideEventsBuilder Start(String start)
        {
            rotatorModel.Start = start;
            return this;
        }
        public RotatorClientSideEventsBuilder Stop(String stop)
        {
            rotatorModel.Stop = stop;
            return this;
        }
        public RotatorClientSideEventsBuilder Destroy(String destroy)
        {
            rotatorModel.Destroy = destroy;
            return this;
        }
        public RotatorClientSideEventsBuilder ThumbClick(String thumbClick)
        {
            rotatorModel.ThumbClick = thumbClick;
            return this;
        }
        public RotatorClientSideEventsBuilder PagerClick(String pagerClick)
        {
            rotatorModel.PagerClick = pagerClick;
            return this;
        }
    }
}
