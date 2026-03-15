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
    public class TagCloudClientSideEventsBuilder
    {
        private TagCloudProperties tagCloudModel;
        public TagCloudClientSideEventsBuilder(TagCloudProperties tagCloudProp)
        {
            tagCloudModel = tagCloudProp;
        }
        //Events
        public TagCloudClientSideEventsBuilder Create(String create)
        {
            tagCloudModel.Create = create;
            return this;
        }
		 public TagCloudClientSideEventsBuilder Click(String click)
        {
            tagCloudModel.Click = click;
            return this;
        }
        public TagCloudClientSideEventsBuilder MouseOver(String mouseover)
        {
            tagCloudModel.MouseOver = mouseover;
            return this;
        }
        public TagCloudClientSideEventsBuilder MouseOut(String mouseout)
        {
            tagCloudModel.MouseOut = mouseout;
            return this;
        }
        public TagCloudClientSideEventsBuilder destroy(String destroy)
        {
            tagCloudModel.Destroy = destroy;
            return this;
        }
    }
}
