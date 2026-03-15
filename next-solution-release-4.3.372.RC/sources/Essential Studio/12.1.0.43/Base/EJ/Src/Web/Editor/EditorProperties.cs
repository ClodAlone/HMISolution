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
    public class EditorProperties
    {
        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool persist = false;
        private bool showSpinButton = true;
        private bool strictMode = false;
        private bool readOnly = false;
        //Integer Values
        private double minValue = -1.7976931348623157e+308;
        private double maxValue = 1.7976931348623157e+308;
        private int incrementStep = 1;
        private int decimals = 0;
        //String Values
        private string width = "";
        private string height = "";
        private string cssClass = "";
        private string waterMarkText = "";
        private string localize = "en-US";
        private string value = null;
        private string name = null;
        //Events
        private string create = null;
        private string change = null;
        private string focusIn = null;
        private string focusOut = null;
        private string destroy = null;

        //Editor
        private Numeric numeric = new Numeric();
        private Percent percent = new Percent();
        private Currency currency = new Currency();
        #endregion
        public EditorProperties() {}
        //public EditorProperties(String id, Editor editor)
        //{
        //    editor.ID = id;
        //    this.editor = editor;
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
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        [JsonProperty("showSpinButton")]
        [DefaultValue(true)]
        public bool ShowSpinButton
        {
            get { return this.showSpinButton; }
            set { this.showSpinButton = value; }
        }
        [JsonProperty("strictMode")]
        [DefaultValue(false)]
        public bool StrictMode
        {
            get { return this.strictMode; }
            set { this.strictMode = value; }
        }
         [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        //Integer Values
        [JsonProperty("minValue")]
        [DefaultValue(-1.7976931348623157e+308)]
        public double MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }
        [JsonProperty("maxValue")]
        [DefaultValue(1.7976931348623157e+308)]
        public double MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }
        [JsonProperty("incrementStep")]
        [DefaultValue(1)]
        public int IncrementStep
        {
            get { return this.incrementStep; }
            set { this.incrementStep = value; }
        }
        [JsonProperty("decimals")]
        [DefaultValue(0)]
        public int Decimals
        {
            get { return this.decimals; }
            set { this.decimals = value; }
        }
        //String Values
        [JsonProperty("width")]
        [DefaultValue("")]
        public string Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("height")]
        [DefaultValue("")]
        public string Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("waterMarkText")]
        [DefaultValue("")]
        public string WaterMarkText
        {
            get { return this.waterMarkText; }
            set { this.waterMarkText = value; }
        }
        [JsonProperty("localize")]
        [DefaultValue("en-US")]
        public string Localize
        {
            get { return this.localize; }
            set { this.localize = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public string Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public string Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("focusIn")]
        [DefaultValue(null)]
        public string FocusIn
        {
            get { return this.focusIn; }
            set { this.focusIn = value; }
        }
        [JsonProperty("focusOut")]
        [DefaultValue(null)]
        public string FocusOut
        {
            get { return this.focusOut; }
            set { this.focusOut = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public string Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
        #region ShouldSerialize Methods

        #endregion
    }
}
