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
    public class ButtonProperties
    {

        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool repeatButton = false;
        //Enumeration Values
        private ButtonSize size = ButtonSize.Normal;
        private Contents contentType = Contents.TextOnly;
        private ImagePositions imagePosition = ImagePositions.ImageLeft;
        //String Values
        private String height = "";
        private String width = "";
        private String text = null;
        private String cssClass = "";
        private String prefixIcon = null;
        private String suffixIcon = null;

        //Events 
        private String create = null;
        private String click = null;
        private String destroy = null;

        //Button 
        private Button button = new Button();

        #endregion

        public ButtonProperties() {  }
        //public ButtonProperties(String id, Button button)
        //{
        //    button.ID = id;
        //    this.button = button;
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
        [JsonProperty("repeatButton")]
        [DefaultValue(false)]
        public bool RepeatButton
        {
            get { return this.repeatButton; }
            set { this.repeatButton = value; }
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
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("prefixIcon")]
        [DefaultValue(null)]
        public String PrefixIcon
        {
            get { return this.prefixIcon; }
            set { this.prefixIcon = value; }
        }
        [JsonProperty("suffixIcon")]
        [DefaultValue(null)]
        public String SuffixIcon
        {
            get { return this.suffixIcon; }
            set { this.suffixIcon = value; }
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
