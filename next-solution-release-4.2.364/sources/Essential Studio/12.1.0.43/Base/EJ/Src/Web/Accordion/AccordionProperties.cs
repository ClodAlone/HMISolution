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
using System.Collections.ObjectModel;

namespace Syncfusion.JavaScript.Models
{
    public class AccordionProperties
    {
        #region Fields
        //Boolean Values
        private bool collapsible = false;
        private bool rtl = false;
        private bool allowKeyboardNavigation = true;
        private bool multipleOpen = false;
        private bool roundedCorner = false;
        private bool persist = false;
        private bool enabled = true;

        //Object values
        private object ajaxOptions = new jQueryAjaxOptions();
        private object iconCSS = new IconCSS();

        //Enumeration values
        private HeightStyle heightStyle = HeightStyle.Content;
       
        //int Array
        private List<String> disabledItems = new List<String>() { };
        private List<String> selectedItems = new List<String>() { };
        
       
        //String Values
        private String cssClass = "";
        private String events = "click";

        //Int Values
        private int selectedItemIndex = 0;

        //events
        private String create = null;
        private String ajaxLoad = null;
        private String ajaxBeforeLoad = null;
        private String active = null;
        private String beforeActive = null;
        private String ajaxSuccess = null;
        private String ajaxError = null;
        private String destroy = null;


        private Accordion accordion = new Accordion();
        #endregion

        public AccordionProperties()
        {
            this.Items = new List<AccordionBaseItem>();
        }
        //public AccordionProperties(string id)
        //{
        //    accordion.ID = id;
        //    this.accordion = accordion;
        //}

        #region properties
        //Boolean values
        [JsonProperty("collapsible")]
        [DefaultValue(false)]
        public bool Collapsible
        {
            get { return this.collapsible; }
            set { this.collapsible = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("allowKeyboardNavigation")]
        [DefaultValue(true)]
        public bool AllowKeyboardNavigation
        {
            get { return this.allowKeyboardNavigation; }
            set { this.allowKeyboardNavigation = value; }
        }
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("multipleOpen")]
        [DefaultValue(false)]
        public bool MultipleOpen
        {
            get { return this.multipleOpen; }
            set { this.multipleOpen = value; }
        }
        //Object values
        [JsonProperty("ajaxOptions")]
        public object AjaxOptions
        {
            get { return this.ajaxOptions; }
            set { this.ajaxOptions = value; }
        }
        [JsonProperty("iconCSS")]
        public object IconCSS
        {
            get { return this.iconCSS; }
            set { this.iconCSS = value; }
        }
        //Enum Values
        [JsonProperty("heightStyle")]
        [DefaultValue(HeightStyle.Content)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HeightStyle HeightStyles
        {
            get { return this.heightStyle; }
            set { this.heightStyle = value; }
        }
        //String Array
        [JsonProperty("disabledItems")]
        public List<String> DisabledItems
        {
            get { return this.disabledItems; }
            set { this.disabledItems = value; }
        }
        [JsonProperty("selectedItems")]
        public List<String> SelectedItems
        {
            get { return this.selectedItems; }
            set { this.selectedItems = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("events")]
        [DefaultValue("click")]
        public String Events
        {
            get { return this.events; }
            set { this.events = value; }
        }
        //Integer Values
        [JsonProperty("selectedItemIndex")]
        [DefaultValue(0)]
        public int SelectedItemIndex
        {
            get { return this.selectedItemIndex; }
            set { this.selectedItemIndex = value; }
        }
        //Event values
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("ajaxLoad")]
        [DefaultValue(null)]
        public String AjaxLoad
        {
            get { return this.ajaxLoad; }
            set { this.ajaxLoad = value; }
        }
        [JsonProperty("ajaxBeforeLoad")]
        [DefaultValue(null)]
        public String AjaxBeforeLoad
        {
            get { return this.ajaxBeforeLoad; }
            set { this.ajaxBeforeLoad = value; }
        }
        [JsonProperty("active")]
        [DefaultValue(null)]
        public String Active
        {
            get { return this.active; }
            set { this.active = value; }
        }
        [JsonProperty("beforeActive")]
        [DefaultValue(null)]
        public String BeforeActive
        {
            get { return this.beforeActive; }
            set { this.beforeActive = value; }
        }
        [JsonProperty("ajaxSuccess")]
        [DefaultValue(null)]
        public String AjaxSuccess
        {
            get { return this.ajaxSuccess; }
            set { this.ajaxSuccess = value; }
        }
        [JsonProperty("ajaxError")]
        [DefaultValue(null)]
        public String AjaxError
        {
            get { return this.ajaxError; }
            set { this.ajaxError = value; }
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
        public bool ShouldSerializeIconCSS()
        {
            if (Utils.PropertyCompare(IconCSS, new IconCSS()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeAjaxOptions()
        {
            if (Utils.PropertyCompare(AjaxOptions, new jQueryAjaxOptions()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDisabledItems()
        {
            if (DisabledItems.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSelectedItems()
        {
            if (SelectedItems.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
       
        
        [JsonIgnore]
        public List<AccordionBaseItem> Items { get; set; }
    }
}
