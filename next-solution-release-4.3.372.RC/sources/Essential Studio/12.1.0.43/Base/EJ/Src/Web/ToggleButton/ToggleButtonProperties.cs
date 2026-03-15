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
    public class ToggleButtonProperties
    {
        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool checkedStatus = false;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool persist = false;

        //Enumeration Values
        private ButtonSize size = ButtonSize.Normal;
        private Contents contentType = Contents.TextOnly;
        private ImagePositions imagePosition = ImagePositions.ImageLeft;

        //String Values
        private String height = "";
        private String width = "";
        private String defaultText = null;
        private String activeText = null;
        private String cssClass = "";
        private String defaultPrefixIcon = null;
        private String defaultSuffixIcon = null;
        private String activePrefixIcon = null;
        private String activeSuffixIcon = null;
       
        //Events 
        private String create = null;
        private String click = null;
        private String change = null;
        private String destroy = null;
        //Button 
        private ToggleButton togglebutton = new ToggleButton();

        #endregion

        public ToggleButtonProperties() { }
        //public ToggleButtonProperties(String id, ToggleButton togglebutton)
        //{
        //    togglebutton.ID = id;
        //    this.togglebutton = togglebutton;
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
        [JsonProperty("checkedStatus")]
        [DefaultValue(false)]
        public bool CheckedStatus
        {
            get { return this.checkedStatus; }
            set { this.checkedStatus = value; }
        }
        //Enum values
        [JsonProperty("size")]
        [DefaultValue(ButtonSize.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ButtonSize Size
        {
            get { return this.size; }
            set { this.size = value; }
        }
        [JsonProperty("contentType")]
        [DefaultValue(Contents.TextOnly)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Contents ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }
        [JsonProperty("imagePosition")]
        [DefaultValue(ImagePositions.ImageLeft)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ImagePositions ImagePosition
        {
            get { return this.imagePosition; }
            set { this.imagePosition = value; }
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
        [JsonProperty("defaultText")]
        [DefaultValue(null)]
        public String DefaultText
        {
            get { return this.defaultText; }
            set { this.defaultText = value; }
        }
        [JsonProperty("activeText")]
        [DefaultValue(null)]
        public String ActiveText
        {
            get { return this.activeText; }
            set { this.activeText = value; }
        }
        [JsonProperty("defaultPrefixIcon")]
        [DefaultValue(null)]
        public String DefaultPrefixIcon
        {
            get { return this.defaultPrefixIcon; }
            set { this.defaultPrefixIcon = value; }
        }
        [JsonProperty("defaultSuffixIcon")]
        [DefaultValue(null)]
        public String DefaultSuffixIcon
        {
            get { return this.defaultSuffixIcon; }
            set { this.defaultSuffixIcon = value; }
        }
        [JsonProperty("activePrefixIcon")]
        [DefaultValue(null)]
        public String ActivePrefixIcon
        {
            get { return this.activePrefixIcon; }
            set { this.activePrefixIcon = value; }
        }
        [JsonProperty("activeSuffixIcon")]
        [DefaultValue(null)]
        public String ActiveSuffixIcon
        {
            get { return this.activeSuffixIcon; }
            set { this.activeSuffixIcon = value; }
        }
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("click")]
        [DefaultValue(null)]
        public String Click
        {
            get { return this.click; }
            set { this.click = value; }
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

        #endregion
       
    }
}
