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
    public class MaskEditProperties
    {
        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool error = false;
        private bool readOnly = false;
        private bool hidePromptOnLeave = false;
        //Enumeration Values
        private TextAlign textAlign = TextAlign.Left;
        private InputMode inputMode = InputMode.Text;
        //String Values
        private String height = "";
        private String width = "";
        private String mask = "";
        private String value = "";
        private String waterMarkText = "";
        private String cssClass = "";
        private String customChar = null;
        //Events
        private String create = null;
        private String onKeyDown = null;
        private String keyUp = null;
        private String keyPress = null;
        private String change = null;
        private String mouseOver = null;
        private String mouseOut = null;
        private String focusIn = null;
        private String focusOut = null;
        private String destroy = null;
        //MaskEdit 
        private MaskEdit maskEdit = new MaskEdit();
        #endregion
        public MaskEditProperties() {  }
        //public MaskEditProperties(String id, MaskEdit maskEdit)
        //{
        //    maskEdit.ID = id;
        //    this.maskEdit = maskEdit;
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
        [JsonProperty("error")]
        [DefaultValue(false)]
        public bool Error
        {
            get { return this.error; }
            set { this.error = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("hidePromptOnLeave")]
        [DefaultValue(false)]
        public bool HidePromptOnLeave
        {
            get { return this.hidePromptOnLeave; }
            set { this.hidePromptOnLeave = value; }
        }
        //Enum values
        [JsonProperty("textAlign")]
        [DefaultValue(TextAlign.Left)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlign TextAlign
        {
            get { return this.textAlign; }
            set { this.textAlign = value; }
        }
        [JsonProperty("inputMode")]
        [DefaultValue(InputMode.Text)]
        [JsonConverter(typeof(StringEnumConverter))]
        public InputMode InputMode
        {
            get { return this.inputMode; }
            set { this.inputMode = value; }
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
        [JsonProperty("mask")]
        [DefaultValue("")]
        public String Mask
        {
            get { return this.mask; }
            set { this.mask = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("waterMarkText")]
        [DefaultValue("")]
        public String WaterMarkText
        {
            get { return this.waterMarkText; }
            set { this.waterMarkText = value; }
        }
        [JsonProperty("customChar")]
        [DefaultValue(null)]
        public String CustomChar
        {
            get { return this.customChar; }
            set { this.customChar = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("onKeyDown")]
        [DefaultValue(null)]
        public String OnKeyDown
        {
            get { return this.onKeyDown; }
            set { this.onKeyDown = value; }
        }
        [JsonProperty("keyUp")]
        [DefaultValue(null)]
        public String KeyUp
        {
            get { return this.keyUp; }
            set { this.keyUp = value; }
        }
        [JsonProperty("keyPress")]
        [DefaultValue(null)]
        public String KeyPress
        {
            get { return this.keyPress; }
            set { this.keyPress = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("mouseOver")]
        [DefaultValue(null)]
        public String MouseOver
        {
            get { return this.mouseOver; }
            set { this.mouseOver = value; }
        }
        [JsonProperty("mouseOut")]
        [DefaultValue(null)]
        public String MouseOut
        {
            get { return this.mouseOut; }
            set { this.mouseOut = value; }
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
        #region ShouldSerialize Methods

        #endregion

    }
}
