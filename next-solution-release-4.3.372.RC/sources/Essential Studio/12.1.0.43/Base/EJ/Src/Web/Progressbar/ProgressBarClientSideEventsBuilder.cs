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
    public class ProgressBarClientSideEventsBuilder
    {
        private ProgressBarProperties progressBarModel;
        public ProgressBarClientSideEventsBuilder(ProgressBarProperties progressBarProp)
        {
            progressBarModel = progressBarProp;
        }
        //Events
         public ProgressBarClientSideEventsBuilder Create(String create)
        {
            progressBarModel.Create = create;
            return this;
        }
        public ProgressBarClientSideEventsBuilder Start(String start)
        {
            progressBarModel.Start = start;
            return this;
        }
        public ProgressBarClientSideEventsBuilder Change(String change)
        {
            progressBarModel.Change = change;
            return this;
        }
        public ProgressBarClientSideEventsBuilder Complete(String complete)
        {
            progressBarModel.Complete = complete;
            return this;
        }
        public ProgressBarClientSideEventsBuilder Destroy(String destroy)
        {
            progressBarModel.Destroy = destroy;
            return this;
        }
    }
}
