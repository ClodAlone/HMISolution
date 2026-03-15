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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class DateTimePickerProperties
    {
        #region Fields
        //integer values
        private int interval = 30;
        private int startDay=0;
        private int stepMonths=1;
        private int timePopupWidth = 100;
        //Boolean values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool showOtherMonths = true;
        private bool persist = false;
        private bool readOnly = false;
        private bool showButton = true;
        //String values
        private string cssClass = "";
        private string localize = "en-US";
        private string value = "";
        private string dateTimeFormat = "";
        private string timeDisplayFormat = "";
        private string width = "";
        private string height = "";
        private string min = "1/1/1900 12:00 AM";
        private string max = "12/31/2099 11:59 PM";
        private string headerFormat = "MMMM yyyy";
        //Enum values
        private Header dayHeaderFormat = Header.ShowHeaderMin;
        private Period startLevel = Period.Month;
        private Period depthLevel = Period.None;
        //object data
        private ButtonText buttonText = new ButtonText();
         //Events
        private string create = null;
        private string open = null;
        private string close = null;
        private string change = null;
        private string destroy = null;
        //DateTimePicker
        private DateTimePicker dateTimePicker = new DateTimePicker();
        #endregion
        public DateTimePickerProperties() { }

        #region Properties
        [JsonProperty("interval")]
        [DefaultValue(30)]
        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }
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
        [JsonProperty("timePopupWidth")]
        [DefaultValue(100)]
        public int TimePopupWidth
        {
            get { return this.timePopupWidth; }
            set { this.timePopupWidth = value; }
        }
        //Boolean values
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
        [JsonProperty("showButton")]
        [DefaultValue(false)]
        public bool ShowButton
        {
            get { return this.showButton; }
            set { this.showButton = value; }
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
        //Date Values
        [JsonProperty("min")]
        [DefaultValue("1/1/1900 12:00 AM")]
        public String MinValue
        {
            get { return this.min; }
            set { this.min = value; }
        }
        [JsonProperty("max")]
        [DefaultValue("12/31/2099 11:59 PM")]
        public String MaxValue
        {
            get { return this.max; }
            set { this.max = value; }
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
        [JsonProperty("timeDisplayFormat")]
        [DefaultValue("")]
        public String TimeDisplayFormat
        {
            get { return this.timeDisplayFormat; }
            set { this.timeDisplayFormat = value; }
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
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("dateTimeFormat")]
        [DefaultValue("")]
        public String DateTimeFormat
        {
            get { return this.dateTimeFormat; }
            set { this.dateTimeFormat = value; }
        }
        //Object
        [JsonProperty("buttonText")]
        public ButtonText DateTimePickerButtonText
        {
            get { return this.buttonText; }
            set { this.buttonText = value; }
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
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
        #region ShouldSerialize Methods

        public bool ShouldSerializeDateTimePickerButtonText()
        {
            if (Utils.PropertyCompare(DateTimePickerButtonText, new ButtonText()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
