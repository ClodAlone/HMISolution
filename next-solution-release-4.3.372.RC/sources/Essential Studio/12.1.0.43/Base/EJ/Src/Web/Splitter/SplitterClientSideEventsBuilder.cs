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
    public class SplitterClientSideEventsBuilder
    {
        private SplitterProperties splitterModel;
        public SplitterClientSideEventsBuilder(SplitterProperties splitterProp)
        {
            splitterModel = splitterProp;
        }
        //Events
        public SplitterClientSideEventsBuilder Create(String create)
        {
            splitterModel.Create = create;
            return this;
        }
        public SplitterClientSideEventsBuilder BeforeExpandCollapse(String beforeExpandCollapse)
        {
            splitterModel.BeforeExpandCollapse = beforeExpandCollapse;
            return this;
        }
        public SplitterClientSideEventsBuilder ExpandCollapse(String expandCollapse)
        {
            splitterModel.ExpandCollapse = expandCollapse;
            return this;
        }
        public SplitterClientSideEventsBuilder Resize(String resize)
        {
            splitterModel.Resize = resize;
            return this;
        }
        public SplitterClientSideEventsBuilder Destroy(String destroy)
        {
            splitterModel.Destroy = destroy;
            return this;
        }
    }
}
