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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class DatePickerProperties
    {
        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool showDateIcon = true;
        private bool showFooter = true;
        private bool displayInline = false;
        private bool displayDefaultDate = true;
        private bool showOtherMonths = true;
        private bool persist = false;
        private bool readOnly = false;
        private bool strictMode = false;
        //Enum Values
        private Header dayHeaderFormat = Header.ShowHeaderMin;
        private Period startLevel = Period.Month;
        private Period depthLevel =Period.None;
        //Int values
        private int startDay=0;
        private int stepMonths = 1;
        //String values
        private string dateFormat = "";
        private string waterMarkText = "Select date";
        private string cssClass = "";
        private string localize = "en-US";
        private string width = "";
        private string height = "";
        private string headerFormat = "MMMM yyyy";
        private string buttonText = "Today";
        private string minDate = "01/01/1900";
        private string maxDate = "12/31/2099";
        private string value = null;
        private string tagName = null;
        //Events
        private string create = null;
        private string open = null;
        private string close = null;
        private string select = null;
        private string change = null;
        private string focusIn = null;
        private string focusOut = null;
        private string destroy = null;
        //DatePicker 
        private DatePicker datePicker = new DatePicker();
        #endregion
        public DatePickerProperties() {  }
        //public DatePickerProperties(String id, DatePicker datePicker)
        //{
        //    datePicker.ID = id;
        //    this.datePicker = datePicker;
        //}
        #region Properties
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("showDateIcon")]
        [DefaultValue(true)]
        public bool ShowDateIcon
        {
            get { return this.showDateIcon; }
            set { this.showDateIcon = value; }
        }
        [JsonProperty("showFooter")]
        [DefaultValue(true)]
        public bool ShowFooter
        {
            get { return this.showFooter; }
            set { this.showFooter = value; }
        }
        [JsonProperty("displayInline")]
        [DefaultValue(false)]
        public bool DisplayInline
        {
            get { return this.displayInline; }
            set { this.displayInline = value; }
        }
        [JsonProperty("displayDefaultDate")]
        [DefaultValue(true)]
        public bool DisplayDefaultDate
        {
            get { return this.displayDefaultDate; }
            set { this.displayDefaultDate = value; }
        }
        [JsonProperty("showOtherMonths")]
        [DefaultValue(true)]
        public bool ShowOtherMonths
        {
            get { return this.showOtherMonths; }
            set { this.showOtherMonths = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("strictMode")]
        [DefaultValue(false)]
        public bool StrictMode
        {
            get { return this.strictMode; }
            set { this.strictMode = value; }
        }
        //Enum values
        [JsonProperty("dayHeaderFormat")]
        [DefaultValue(Header.ShowHeaderMin)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Header DayHeaderFormat
        {
            get { return this.dayHeaderFormat; }
            set { this.dayHeaderFormat = value; }
        }
        [JsonProperty("startLevel")]
        [DefaultValue(Period.Month)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Period StartLevel
        {
            get { return this.startLevel; }
            set { this.startLevel = value; }
        }
        [JsonProperty("depthLevel")]
        [DefaultValue(Period.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Period DepthLevel
        {
            get { return this.depthLevel; }
            set { this.depthLevel = value; }
        }
       // Date Values
        [JsonProperty("minDate")]
        [DefaultValue("01/01/1900")]
        public String MinDate
        {
            get { return this.minDate; }
            set { this.minDate = value; }
        }
        [JsonProperty("maxDate")]
        [DefaultValue("12/31/2099")]
        public String MaxDate
        {
            get { return this.maxDate; }
            set { this.maxDate = value; }
        }

        //Int values
        [JsonProperty("startDay")]
        [DefaultValue(0)]
        public int StartDay
        {
            get { return this.startDay; }
            set { this.startDay = value; }
        }
        [JsonProperty("stepMonths")]
        [DefaultValue(1)]
        public int StepMonths
        {
            get { return this.stepMonths; }
            set { this.stepMonths = value; }
        }
        //string values
        [JsonProperty("height")]
        [DefaultValue("")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue("")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("dateFormat")]
        [DefaultValue("")]
        public String DateFormat
        {
            get { return this.dateFormat; }
            set { this.dateFormat = value; }
        }
        [JsonProperty("waterMarkText")]
        [DefaultValue("Select date")]
        public String WaterMarkText
        {
            get { return this.waterMarkText; }
            set { this.waterMarkText = value; }
        }
        [JsonProperty("localize")]
        [DefaultValue("en-US")]
        public String Localize
        {
            get { return this.localize; }
            set { this.localize = value; }
        }
        [JsonProperty("headerFormat")]
        [DefaultValue("MMMM yyyy")]
        public String HeaderFormat
        {
            get { return this.headerFormat; }
            set { this.headerFormat = value; }
        }
        [JsonProperty("buttonText")]
        [DefaultValue("Today")]
        public String ButtonText
        {
            get { return this.buttonText; }
            set { this.buttonText = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("tagName")]
        [DefaultValue(null)]
        public String TagName
        {
            get { return this.tagName; }
            set { this.tagName = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("open")]
        [DefaultValue(null)]
        public String Open
        {
            get { return this.open; }
            set { this.open = value; }
        }
        [JsonProperty("close")]
        [DefaultValue(null)]
        public String Close
        {
            get { return this.close; }
            set { this.close = value; }
        }
        [JsonProperty("select")]
        [DefaultValue(null)]
        public String Select
        {
            get { return this.select; }
            set { this.select = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("focusIn")]
        [DefaultValue(null)]
        public String FocusIn
        {
            get { return this.focusIn; }
            set { this.focusIn = value; }
        }
        [JsonProperty("focusOut")]
        [DefaultValue(null)]
        public String FocusOut
        {
            get { return this.focusOut; }
            set { this.focusOut = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
    }
}
