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
    public class CheckBoxProperties
    {
        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool rtl = false;
        private bool persist = false;
       private bool check = false;
        private bool indeterminate = false;
        private bool indeterminateState = false;
        //Enumeration Values
        private Size size = Size.Small;
        //String Values
        private String value = null;
        private String cssClass = "";
        private String name = null;
        private String id = null;
        private String text = "";
        private String idPrefix = "ej";
        //Events 
        private String create = null;
        private String beforeChange = null;
        private String change = null;
        private String destroy = null;

        //Check Box
        private CheckBox checkbox = new CheckBox();
        #endregion

        public CheckBoxProperties() {  }
        //public CheckBoxProperties(String id, CheckBox checkbox)
        //{
        //    checkbox.ID = id;
        //    this.checkbox = checkbox;
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
        [JsonProperty("indeterminate")]
        [DefaultValue(false)]
        public bool Indeterminate
        {
            get { return this.indeterminate; }
            set { this.indeterminate = value; }
        }
        [JsonProperty("indeterminateState")]
        [DefaultValue(false)]
        public bool IndeterminateState
        {
            get { return this.indeterminateState; }
            set { this.indeterminateState = value; }
        }
        [JsonProperty("check")]
        [DefaultValue(false)]
        public bool Check
        {
            get { return this.check; }
            set { this.check = value; }
        }

        //enum values
        [JsonProperty("size")]
        [DefaultValue(Size.Small)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Size Size
        {
            get { return this.size; }
            set { this.size = value; }
        }

        //string values
        [JsonProperty("id")]
        [DefaultValue(null)]
        public String Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        [JsonProperty("name")]
        [DefaultValue(null)]
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("text")]
        [DefaultValue("")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("idPrefix")]
        [DefaultValue("ej")]
        public String IdPrefix
        {
            get { return this.idPrefix; }
            set { this.idPrefix = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        //events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("beforeChange")]
        [DefaultValue(null)]
        public String BeforeChange
        {
            get { return this.beforeChange; }
            set { this.beforeChange = value; }
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
