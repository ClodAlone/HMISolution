#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;


namespace Syncfusion.JavaScript
{
   public class TimePickerPropertiesBuilder
    {
       public TimePicker timepicker;

       public TimePickerPropertiesBuilder(TimePicker timepicker)
       { this.timepicker = new TimePicker(timepicker.ID, timepicker.TimePickerModel); }
        
        public TimePickerPropertiesBuilder()
        {
        }
        //Boolean values
        public TimePickerPropertiesBuilder ReadOnly()
        {
            timepicker.TimePickerModel.ReadOnly = true;
            return this;
        }
        public TimePickerPropertiesBuilder ReadOnly(bool readOnly)
        {
            timepicker.TimePickerModel.ReadOnly = readOnly;
            return this;
        }
        public TimePickerPropertiesBuilder ShowButton()
        {
            timepicker.TimePickerModel.ShowButton = true;
            return this;
        }
        public TimePickerPropertiesBuilder ShowButton(bool showButton)
        {
            timepicker.TimePickerModel.ShowButton = showButton;
            return this;
        }
        public TimePickerPropertiesBuilder RoundedCorner()
        {
            timepicker.TimePickerModel.RoundedCorner = true;
            return this;
        }
        public TimePickerPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            timepicker.TimePickerModel.RoundedCorner = roundedCorner;
            return this;
        }
        public TimePickerPropertiesBuilder Rtl()
        {
            timepicker.TimePickerModel.Rtl = true;
            return this;
        }
        public TimePickerPropertiesBuilder Rtl(bool rtl)
        {
            timepicker.TimePickerModel.Rtl = rtl;
            return this;
        }
        public TimePickerPropertiesBuilder Enabled()
        {
            timepicker.TimePickerModel.Enabled = true;
            return this;
        }
        public TimePickerPropertiesBuilder Enabled(bool enabled)
        {
            timepicker.TimePickerModel.Enabled = enabled;
            return this;
        }
        public TimePickerPropertiesBuilder Persist()
        {
            timepicker.TimePickerModel.Persist = true;
            return this;
        }
        public TimePickerPropertiesBuilder Persist(bool persist)
        {
            timepicker.TimePickerModel.Persist = persist;
            return this;
        }
       //int values
        public TimePickerPropertiesBuilder Interval(int interval)
        {
            timepicker.TimePickerModel.Interval = interval;
            return this;
        }
        public TimePickerPropertiesBuilder HourInterval(int hourInterval)
        {
            timepicker.TimePickerModel.HourInterval = hourInterval;
            return this;
        }
        public TimePickerPropertiesBuilder MinInterval(int minInterval)
        {
            timepicker.TimePickerModel.MinInterval = minInterval;
            return this;
        }
        public TimePickerPropertiesBuilder SecInterval(int secInterval)
        {
            timepicker.TimePickerModel.SecInterval = secInterval;
            return this;
        }
       //String values
        public TimePickerPropertiesBuilder CssClass(String cssClass)
        {
            timepicker.TimePickerModel.CssClass = cssClass;
            return this;
        }
        public TimePickerPropertiesBuilder TimeFormat(String timeFormat)
        {
            timepicker.TimePickerModel.TimeFormat = timeFormat;
            return this;
        }
        public TimePickerPropertiesBuilder Value(String value)
        {
            timepicker.TimePickerModel.Value = value;
            return this;
        }
        public TimePickerPropertiesBuilder Localize(String localize)
        {
            timepicker.TimePickerModel.Localize = localize;
            return this;
        }
        public TimePickerPropertiesBuilder Height(String height)
        {
            timepicker.TimePickerModel.Height = height;
            return this;
        }
        public TimePickerPropertiesBuilder Width(String width)
        {
            timepicker.TimePickerModel.Width = width;
            return this;
        }
        public TimePickerPropertiesBuilder MinTime(String minTime)
        {
            timepicker.TimePickerModel.MinTime = minTime;
            return this;
        }
        public TimePickerPropertiesBuilder MaxTime(String maxTime)
        {
            timepicker.TimePickerModel.MaxTime = maxTime;
            return this;
        }
        public TimePickerPropertiesBuilder PopupHeight(String popupHeight)
        {
            timepicker.TimePickerModel.PopupHeight = popupHeight;
            return this;
        }
        public TimePickerPropertiesBuilder PopupWidth(String popupWidth)
        {
            timepicker.TimePickerModel.PopupWidth = popupWidth;
            return this;
        }
        //Events
        public TimePickerPropertiesBuilder ClientSideEvents(Action<TimePickerClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new TimePickerClientSideEventsBuilder(this.timepicker.TimePickerModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(timepicker.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
