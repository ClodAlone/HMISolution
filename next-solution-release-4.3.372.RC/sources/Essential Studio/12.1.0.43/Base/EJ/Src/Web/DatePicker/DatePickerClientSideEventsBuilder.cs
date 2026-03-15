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
    public class DatePickerClientSideEventsBuilder
    {
        private DatePickerProperties datePickerModel;
        public DatePickerClientSideEventsBuilder(DatePickerProperties datePickerProp)
        {
            datePickerModel = datePickerProp;
        }
        //Events
        public DatePickerClientSideEventsBuilder Create(String create)
        {
            datePickerModel.Create = create;
            return this;
        }
        public DatePickerClientSideEventsBuilder Open(String open)
        {
            datePickerModel.Open = open;
            return this;
        }
        public DatePickerClientSideEventsBuilder Close(String close)
        {
            datePickerModel.Close = close;
            return this;
        }
        public DatePickerClientSideEventsBuilder Select(String select)
        {
            datePickerModel.Select = select;
            return this;
        }
        public DatePickerClientSideEventsBuilder Change(String change)
        {
            datePickerModel.Change = change;
            return this;
        }
        public DatePickerClientSideEventsBuilder FocusOut(String focusOut)
        {
            datePickerModel.FocusOut = focusOut;
            return this;
        }
        public DatePickerClientSideEventsBuilder FocusIn(String focusIn)
        {
            datePickerModel.FocusIn = focusIn;
            return this;
        }
        public DatePickerClientSideEventsBuilder Destroy(String destroy)
        {
            datePickerModel.Destroy = destroy;
            return this;
        }
    }
}
