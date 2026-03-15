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
    public class CheckBoxClientSideEventsBuilder
    {
        private CheckBoxProperties checkboxModel;
        public CheckBoxClientSideEventsBuilder(CheckBoxProperties checkboxProp)
        {
            checkboxModel =checkboxProp;
        }
        //Events
        public CheckBoxClientSideEventsBuilder Create(String create)
        {
            checkboxModel.Create = create;
            return this;
        }
        public CheckBoxClientSideEventsBuilder BeforeChange(String beforeChange)
        {
            checkboxModel.BeforeChange = beforeChange;
            return this;
        }
        public CheckBoxClientSideEventsBuilder Change(String change)
        {
            checkboxModel.Change = change;
            return this;
        }
        public CheckBoxClientSideEventsBuilder Destroy(String destroy)
        {
            checkboxModel.Destroy = destroy;
            return this;
        }

    }
}
