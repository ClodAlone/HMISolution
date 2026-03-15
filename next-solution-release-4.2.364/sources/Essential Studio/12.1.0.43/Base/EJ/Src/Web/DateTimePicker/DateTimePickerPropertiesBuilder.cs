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
using Syncfusion.JavaScript;



namespace Syncfusion.JavaScript
{
   public class DateTimePickerPropertiesBuilder
    {
       public DateTimePicker dateTimePicker;
       public DateTimePickerPropertiesBuilder(DateTimePicker dateTimePicker)
       {
           this.dateTimePicker = new DateTimePicker(dateTimePicker.ID, dateTimePicker.DateTimePickerModel);
       }
       public DateTimePickerPropertiesBuilder()
       {
       }
       //Int Values
       public DateTimePickerPropertiesBuilder Interval(int interval)
       {
           dateTimePicker.DateTimePickerModel.Interval = interval;
           return this;
       }
       public DateTimePickerPropertiesBuilder StartDay(int startDay)
       {
           dateTimePicker.DateTimePickerModel.StartDay = startDay;
           return this;
       }
       public DateTimePickerPropertiesBuilder StepMonths(int stepMonths)
       {
           dateTimePicker.DateTimePickerModel.StepMonths = stepMonths;
           return this;
       }
       public DateTimePickerPropertiesBuilder TimePopupWidth(int timePopupWidth)
       {
           dateTimePicker.DateTimePickerModel.TimePopupWidth = timePopupWidth;
           return this;
       }
       //Boolean values
       public DateTimePickerPropertiesBuilder Enabled()
       {
           dateTimePicker.DateTimePickerModel.Enabled = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder Enabled(bool enabled)
       {
           dateTimePicker.DateTimePickerModel.Enabled = enabled;
           return this;
       }
       public DateTimePickerPropertiesBuilder RoundedCorner()
       {
           dateTimePicker.DateTimePickerModel.RoundedCorner = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder RoundedCorner(bool roundedCorner)
       {
           dateTimePicker.DateTimePickerModel.RoundedCorner = roundedCorner;
           return this;
       }
       public DateTimePickerPropertiesBuilder Rtl()
       {
           dateTimePicker.DateTimePickerModel.Rtl = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder Rtl(bool rtl)
       {
           dateTimePicker.DateTimePickerModel.Rtl = rtl;
           return this;
       }
       public DateTimePickerPropertiesBuilder ShowOtherMonths()
       {
           dateTimePicker.DateTimePickerModel.ShowOtherMonths = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder ShowOtherMonths(bool showOtherMonths)
       {
           dateTimePicker.DateTimePickerModel.ShowOtherMonths = showOtherMonths;
           return this;
       }
       public DateTimePickerPropertiesBuilder Persist()
       {
           dateTimePicker.DateTimePickerModel.Persist = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder Persist(bool persist)
       {
           dateTimePicker.DateTimePickerModel.Persist = persist;
           return this;
       }
       public DateTimePickerPropertiesBuilder ReadOnly()
       {
           dateTimePicker.DateTimePickerModel.ReadOnly = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder ReadOnly(bool readOnly)
       {
           dateTimePicker.DateTimePickerModel.ReadOnly = readOnly;
           return this;
       }
       public DateTimePickerPropertiesBuilder ShowButton()
       {
           dateTimePicker.DateTimePickerModel.ShowButton = true;
           return this;
       }
       public DateTimePickerPropertiesBuilder ShowButton(bool ShowButton)
       {
           dateTimePicker.DateTimePickerModel.ShowButton = ShowButton;
           return this;
       }
       public DateTimePickerPropertiesBuilder CssClass(String cssClass)
       {
           dateTimePicker.DateTimePickerModel.CssClass = cssClass;
           return this;
       }
       public DateTimePickerPropertiesBuilder Localize(String localize)
       {
           dateTimePicker.DateTimePickerModel.Localize = localize;
           return this;
       }
       public DateTimePickerPropertiesBuilder Value(String value)
       {
           dateTimePicker.DateTimePickerModel.Value = value;
           return this;
       }
       public DateTimePickerPropertiesBuilder DateTimeFormat(String dateTimeFormat)
       {
           dateTimePicker.DateTimePickerModel.DateTimeFormat = dateTimeFormat;
           return this;
       }
       public DateTimePickerPropertiesBuilder TimeDisplayFormat(String timeDisplayFormat)
       {
           dateTimePicker.DateTimePickerModel.TimeDisplayFormat = timeDisplayFormat;
           return this;
       }
       public DateTimePickerPropertiesBuilder Width(String width)
       {
           dateTimePicker.DateTimePickerModel.Width = width;
           return this;
       }
       public DateTimePickerPropertiesBuilder Height(String height)
       {
           dateTimePicker.DateTimePickerModel.Height = height;
           return this;
       }
       public DateTimePickerPropertiesBuilder MinValue(String min)
       {
           dateTimePicker.DateTimePickerModel.MinValue = min;
           return this;
       }
       public DateTimePickerPropertiesBuilder MaxValue(String max)
       {
           dateTimePicker.DateTimePickerModel.MaxValue = max;
           return this;
       }
       public DateTimePickerPropertiesBuilder HeaderFormat(String headerFormat)
       {
           dateTimePicker.DateTimePickerModel.HeaderFormat = headerFormat;
           return this;
       }       
       //EnumValues
       public DateTimePickerPropertiesBuilder DayHeaderFormat(Header dayHeaderFormat)
       {
           dateTimePicker.DateTimePickerModel.DayHeaderFormat = dayHeaderFormat;
           return this;
       }
       public DateTimePickerPropertiesBuilder StartLevel(Period startLevel)
       {
           dateTimePicker.DateTimePickerModel.StartLevel = startLevel;
           return this;
       }
       public DateTimePickerPropertiesBuilder DepthLevel(Period depthLevel)
       {
           dateTimePicker.DateTimePickerModel.DepthLevel = depthLevel;
           return this;
       }
       //object values
       public DateTimePickerPropertiesBuilder DateTimePickerButtonText(Action<ButtonTextBuilder> buttonText)
       {
           var builder = new ButtonTextBuilder(this.dateTimePicker.DateTimePickerModel);           
           if (buttonText != null)
               buttonText.Invoke(builder);
           return this;
       }
       //Events
       public DateTimePickerPropertiesBuilder ClientSideEvents(Action<DateTimePickerClientSideEventsBuilder> clientSideEvents)
       {
           var builder = new DateTimePickerClientSideEventsBuilder(this.dateTimePicker.DateTimePickerModel);
           if (clientSideEvents != null)
               clientSideEvents.Invoke(builder);
           return this;
       }
       //Render 
       public HtmlString Render()
       {
           return new HtmlString(dateTimePicker.Render().ToString());
       }
       public override String ToString()
       {

           return Render().ToString();
       }
    }
}
