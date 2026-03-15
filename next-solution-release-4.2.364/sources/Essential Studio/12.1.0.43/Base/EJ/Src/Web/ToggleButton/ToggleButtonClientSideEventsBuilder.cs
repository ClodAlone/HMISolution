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
    public class ToggleButtonClientSideEventsBuilder
    {
        private ToggleButtonProperties togglebuttonModel;
        public ToggleButtonClientSideEventsBuilder(ToggleButtonProperties togglebuttonProp)
        {
            togglebuttonModel = togglebuttonProp;
        }
        //Events
        public ToggleButtonClientSideEventsBuilder Create(String create)
        {
            togglebuttonModel.Create = create;
            return this;
        }
        public ToggleButtonClientSideEventsBuilder Click(String click)
        {
            togglebuttonModel.Click = click;
            return this;
        }
        public ToggleButtonClientSideEventsBuilder Change(String change)
        {
            togglebuttonModel.Change = change;
            return this;
        }
        public ToggleButtonClientSideEventsBuilder Destroy(String destroy)
        {
            togglebuttonModel.Destroy = destroy;
            return this;
        }
    }
}
