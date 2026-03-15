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
   public class DateTimePickerClientSideEventsBuilder
    {
       private DateTimePickerProperties dateTimePickerModel;
       public DateTimePickerClientSideEventsBuilder(DateTimePickerProperties dateTimePickerProp)
       {
           dateTimePickerModel = dateTimePickerProp;
       }
       //Events
       public DateTimePickerClientSideEventsBuilder Create(string create)
       {
           dateTimePickerModel.Create = create;
           return this;
       }
       public DateTimePickerClientSideEventsBuilder Open(string open)
       {
           dateTimePickerModel.Open = open;
           return this;
       }
       public DateTimePickerClientSideEventsBuilder Close(string close)
       {
           dateTimePickerModel.Close = close;
           return this;
       }
       public DateTimePickerClientSideEventsBuilder Change(string change)
       {
           dateTimePickerModel.Change = change;
           return this;
       }
       public DateTimePickerClientSideEventsBuilder Destroy(string destroy)
       {
           dateTimePickerModel.Destroy = destroy;
           return this;
       }

    }
}
