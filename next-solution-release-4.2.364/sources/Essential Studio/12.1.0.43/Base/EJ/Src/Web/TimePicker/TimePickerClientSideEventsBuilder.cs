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
   public class TimePickerClientSideEventsBuilder
    {
       private TimePickerProperties timepickerModel;
       public TimePickerClientSideEventsBuilder(TimePickerProperties timepickerProp)
       {
           timepickerModel = timepickerProp;
       }
       //Events
       public TimePickerClientSideEventsBuilder Create(String create)
       {
           timepickerModel.Create = create;
           return this;
       }
       public TimePickerClientSideEventsBuilder FocusIn(String focusIn)
       {
           timepickerModel.FocusIn = focusIn;
           return this;
       }
       public TimePickerClientSideEventsBuilder FocusOut(String focusOut)
       {
           timepickerModel.FocusOut = focusOut;
           return this;
       }
       public TimePickerClientSideEventsBuilder Change(String change)
       {
           timepickerModel.Change = change;
           return this;
       }
       public TimePickerClientSideEventsBuilder Select(String select)
       {
           timepickerModel.Select = select;
           return this;
       }
       public TimePickerClientSideEventsBuilder Destroy(String destroy)
       {
           timepickerModel.Destroy = destroy;
           return this;
       }
       

    }
}
