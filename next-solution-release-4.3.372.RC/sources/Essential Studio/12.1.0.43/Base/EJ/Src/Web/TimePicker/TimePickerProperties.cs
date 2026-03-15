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
    public class TimePickerProperties
    {

        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool readOnly = false;
        private bool showButton = true;
        private bool persist = false;

        //String Values
        private String height = "";
        private String width = "";
        private String value = null;
        private String cssClass = "";
        private String timeFormat = "";
        private String localize ="en-US";
        private String minTime = "12:00:00AM";
        private String maxTime = "11:59:59PM";
        private String popupHeight = "191px";
        private String popupWidth = "auto";
    
        //int value
        private int interval = 30;
        private int hourInterval = 1;
        private int minInterval = 1;
        private int secInterval = 1;
        //Events 
        private String create = null;
        private String focusIn = null;
        private String focusOut = null;
        private String change = null;
        private String select = null;
        private String destroy = null;

        private TimePicker timepicker = new TimePicker();

        #endregion

         public TimePickerProperties() {  }
        //public TimePickerProperties(String id, TimePicker timepicker)
        //{
        //    timepicker.ID = id;
        //    this.timepicker = timepicker;
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
        [JsonProperty("showButton")]
        [DefaultValue(true)]
        public bool ShowButton
        {
            get { return this.showButton; }
            set { this.showButton = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
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
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("timeFormat")]
        [DefaultValue(null)]
        public String TimeFormat
        {
            get { return this.timeFormat; }
            set { this.timeFormat = value; }
        }
        [JsonProperty("localize")]
        [DefaultValue("en-US")]
        public String Localize
        {
            get { return this.localize; }
            set { this.localize = value; }
        }
        [JsonProperty("minTime")]
        [DefaultValue("12:00:00AM")]
        public String MinTime
        {
            get { return this.minTime; }
            set { this.minTime = value; }
        }
        [JsonProperty("maxTime")]
        [DefaultValue("11:59:59PM")]
        public String MaxTime
        {
            get { return this.maxTime; }
            set { this.maxTime = value; }
        }
        [JsonProperty("popupHeight")]
        [DefaultValue("191px")]
        public String PopupHeight
        {
            get { return this.popupHeight; }
            set { this.popupHeight = value; }
        }
        [JsonProperty("popupWidth")]
        [DefaultValue("auto")]
        public String PopupWidth
        {
            get { return this.popupWidth; }
            set { this.popupWidth = value; }
        }
        [JsonProperty("interval")]
        [DefaultValue(30)]
        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }
        [JsonProperty("hourInterval")]
        [DefaultValue(1)]
        public int HourInterval
        {
            get { return this.hourInterval; }
            set { this.hourInterval = value; }
        }
        [JsonProperty("minInterval")]
        [DefaultValue(1)]
        public int MinInterval
        {
            get { return this.minInterval; }
            set { this.minInterval = value; }
        }
        [JsonProperty("secInterval")]
        [DefaultValue(1)]
        public int SecInterval
        {
            get { return this.secInterval; }
            set { this.secInterval = value; }
        }
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("focusOut")]
        [DefaultValue(null)]
        public String FocusOut
        {
            get { return this.focusOut; }
            set { this.focusOut = value; }
        }
        [JsonProperty("focusIn")]
        [DefaultValue(null)]
        public String FocusIn
        {
            get { return this.focusIn; }
            set { this.focusIn = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("select")]
        [DefaultValue(null)]
        public String Select
        {
            get { return this.select; }
            set { this.select = value; }
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

        #endregion
    }
}
