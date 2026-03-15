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
    public class RadioButtonClientSideEventsBuilder
    {
         private RadioButtonProperties radiobuttonModel;
         public RadioButtonClientSideEventsBuilder(RadioButtonProperties buttonProp)
        {
            radiobuttonModel = buttonProp;
        }
        //Events
        public RadioButtonClientSideEventsBuilder Create(String create)
        {
            radiobuttonModel.Create = create;
            return this;
        }
        public RadioButtonClientSideEventsBuilder BeforeChange(String beforeChange)
        {
            radiobuttonModel.BeforeChange = beforeChange;
            return this;
        }
        
        public RadioButtonClientSideEventsBuilder Change(String change)
        {
            radiobuttonModel.Change = change;
            return this;
        }
        public RadioButtonClientSideEventsBuilder Destroy(String destroy)
        {
            radiobuttonModel.Destroy = destroy;
            return this;
        }
       
    }
}
