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


namespace Syncfusion.JavaScript
{
    public class DatePickerPropertiesBuilder
    {
        public DatePicker datePicker;

        public DatePickerPropertiesBuilder(DatePicker datePicker)
        { this.datePicker = new DatePicker(datePicker.ID, datePicker.DatePickerModel); }

        public DatePickerPropertiesBuilder()
        {
        }
        //Boolean values
        public DatePickerPropertiesBuilder Enabled()
        {
            datePicker.DatePickerModel.Enabled = true;
            return this;
        }
        public DatePickerPropertiesBuilder Enabled(bool enabled)
        {
            datePicker.DatePickerModel.Enabled = enabled;
            return this;
        }
        public DatePickerPropertiesBuilder RoundedCorner()
        {
            datePicker.DatePickerModel.RoundedCorner = true;
            return this;
        }
        public DatePickerPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            datePicker.DatePickerModel.RoundedCorner = roundedCorner;
            return this;
        }
        public DatePickerPropertiesBuilder Rtl()
        {
            datePicker.DatePickerModel.Rtl = true;
            return this;
        }
        public DatePickerPropertiesBuilder Rtl(bool rtl)
        {
            datePicker.DatePickerModel.Rtl = rtl;
            return this;
        }
        public DatePickerPropertiesBuilder ShowDateIcon()
        {
            datePicker.DatePickerModel.ShowDateIcon = true;
            return this;
        }
        public DatePickerPropertiesBuilder ShowDateIcon(bool showDateIcon)
        {
            datePicker.DatePickerModel.ShowDateIcon = showDateIcon;
            return this;
        }
        public DatePickerPropertiesBuilder ShowFooter()
        {
            datePicker.DatePickerModel.ShowFooter = true;
            return this;
        }
        public DatePickerPropertiesBuilder ShowFooter(bool showFooter)
        {
            datePicker.DatePickerModel.ShowFooter = showFooter;
            return this;
        }
        public DatePickerPropertiesBuilder DisplayInline()
        {
            datePicker.DatePickerModel.DisplayInline = true;
            return this;
        }
        public DatePickerPropertiesBuilder DisplayInline(bool displayInline)
        {
            datePicker.DatePickerModel.DisplayInline = displayInline;
            return this;
        }
        public DatePickerPropertiesBuilder DisplayDefaultDate()
        {
            datePicker.DatePickerModel.DisplayDefaultDate = true;
            return this;
        }
        public DatePickerPropertiesBuilder DisplayDefaultDate(bool displayDefaultDate)
        {
            datePicker.DatePickerModel.DisplayDefaultDate = displayDefaultDate;
            return this;
        }
        public DatePickerPropertiesBuilder ShowOtherMonths()
        {
            datePicker.DatePickerModel.ShowOtherMonths = true;
            return this;
        }
        public DatePickerPropertiesBuilder ShowOtherMonths(bool showOtherMonths)
        {
            datePicker.DatePickerModel.ShowOtherMonths = showOtherMonths;
            return this;
        }
        public DatePickerPropertiesBuilder Persist()
        {
            datePicker.DatePickerModel.Persist = true;
            return this;
        }
        public DatePickerPropertiesBuilder Persist(bool persist)
        {
            datePicker.DatePickerModel.Persist = persist;
            return this;
        }
        public DatePickerPropertiesBuilder ReadOnly()
        {
            datePicker.DatePickerModel.ReadOnly = true;
            return this;
        }
        public DatePickerPropertiesBuilder ReadOnly(bool readOnly)
        {
            datePicker.DatePickerModel.ReadOnly = readOnly;
            return this;
        }
        public DatePickerPropertiesBuilder StrictMode()
        {
            datePicker.DatePickerModel.StrictMode = true;
            return this;
        }
        public DatePickerPropertiesBuilder StrictMode(bool strictMode)
        {
            datePicker.DatePickerModel.StrictMode = strictMode;
            return this;
        }
        //EnumValues
        public DatePickerPropertiesBuilder DayHeaderFormat(Header dayHeaderFormat)
        {
            datePicker.DatePickerModel.DayHeaderFormat = dayHeaderFormat;
            return this;
        }
        public DatePickerPropertiesBuilder StartLevel(Period startLevel)
        {
            datePicker.DatePickerModel.StartLevel = startLevel;
            return this;
        }
        public DatePickerPropertiesBuilder DepthLevel(Period depthLevel)
        {
            datePicker.DatePickerModel.DepthLevel = depthLevel;
            return this;
        }
        //DateValues
        public DatePickerPropertiesBuilder MinDate(String minDate)
        {
            datePicker.DatePickerModel.MinDate = minDate;
            return this;
        }
        public DatePickerPropertiesBuilder MaxDate(String maxDate)
        {
            datePicker.DatePickerModel.MaxDate = maxDate;
            return this;
        }
        //Int Values
        public DatePickerPropertiesBuilder StartDay(int startDay)
        {
            datePicker.DatePickerModel.StartDay = startDay;
            return this;
        }
        public DatePickerPropertiesBuilder StepMonths(int stepMonths)
        {
            datePicker.DatePickerModel.StepMonths = stepMonths;
            return this;
        }
        //String Values
        public DatePickerPropertiesBuilder Height(String height)
        {
            datePicker.DatePickerModel.Height = height;
            return this;
        }
        public DatePickerPropertiesBuilder Width(String width)
        {
            datePicker.DatePickerModel.Width = width;
            return this;
        }
        public DatePickerPropertiesBuilder CssClass(String cssClass)
        {
            datePicker.DatePickerModel.CssClass = cssClass;
            return this;
        }
        public DatePickerPropertiesBuilder DateFormat(String dateFormat)
        {
            datePicker.DatePickerModel.DateFormat = dateFormat;
            return this;
        }
        public DatePickerPropertiesBuilder WaterMarkText(String waterMarkText)
        {
            datePicker.DatePickerModel.WaterMarkText = waterMarkText;
            return this;
        }
        public DatePickerPropertiesBuilder Localize(String localize)
        {
            datePicker.DatePickerModel.Localize = localize;
            return this;
        }
        public DatePickerPropertiesBuilder HeaderFormat(String headerFormat)
        {
            datePicker.DatePickerModel.HeaderFormat = headerFormat;
            return this;
        }
        public DatePickerPropertiesBuilder ButtonText(String buttonText)
        {
            datePicker.DatePickerModel.ButtonText = buttonText;
            return this;
        }
        public DatePickerPropertiesBuilder Value(String value)
        {
            datePicker.DatePickerModel.Value = value;
            return this;
        }
        public DatePickerPropertiesBuilder TagName(String tagName)
        {
            datePicker.DatePickerModel.TagName = tagName;
            return this;
        }
        //Events
        public DatePickerPropertiesBuilder ClientSideEvents(Action<DatePickerClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new DatePickerClientSideEventsBuilder(this.datePicker.DatePickerModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(datePicker.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
