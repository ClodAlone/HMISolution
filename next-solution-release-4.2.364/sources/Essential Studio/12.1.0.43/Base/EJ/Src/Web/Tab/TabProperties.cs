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
    public class TabProperties
    {
        #region Fields

        //Int values
        private int selectedItemIndex = 0;

        //Boolean Values
        private bool collapsible = false;
        private bool showCloseButton = false;
        private bool rtl = false;
        private bool allowKeyboardNavigation = true;
        private bool roundedCorner = false;
        private bool persist = false;
        private bool enabled = true;

        //Object values
        private object ajaxOptions = new jQueryAjaxOptions();

        //Enumeration values
        private HeightStyle heightStyle = HeightStyle.Content;
        private HeaderPosition headerPosition = HeaderPosition.Top;

        //String Array
        private List<String> disabledItems = new List<string>() { "" };
        
        //String values
        private String height = null;
        private String width = null;
        private String cssClass = "";
        private String events ="click";
        private String idPrefix = "ej-tab-";

        //events
        private String create = null;
        private String ajaxLoad = null;
        private String ajaxBeforeLoad = null;
        private String active = null;
        private String beforeActive = null;
        private String itemAdd = null;
        private String itemRemove = null;
        private String beforeItemRemove = null;
        private String itemEnable = null;
        private String itemDisable = null;
        private String destroy = null;
               
        //Tab
        private Tab tab = new Tab();
        #endregion

        public TabProperties() {  
              this.Items = new List<TabBaseItem>();
        }
        //public TabProperties(String id, Tab tab)
        //{
        //    tab.ID = id;
        //    this.tab = tab;
        //}

        #region Properties
        [JsonProperty("selectedItemIndex")]
        [DefaultValue(0)]
        public int SelectedItemIndex
        {
            get { return this.selectedItemIndex; }
            set { this.selectedItemIndex = value; }
        }

        //Boolean values
        [JsonProperty("collapsible")]
        [DefaultValue(false)]
        public bool Collapsible
        {
            get { return this.collapsible; }
            set { this.collapsible = value; }
        }
        [JsonProperty("showCloseButton")]
        [DefaultValue(false)]
        public bool ShowCloseButton
        {
            get { return this.showCloseButton; }
            set { this.showCloseButton = value; }
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
        //Object values
        [JsonProperty("ajaxOptions")]
        public object AjaxOptions
        {
            get { return this.ajaxOptions; }
            set { this.ajaxOptions = value; }
        }
        //Enum Values
        [JsonProperty("heightStyle")]
        [DefaultValue(HeightStyle.Content)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HeightStyle HeightStyle
        {
            get { return this.heightStyle; }
            set { this.heightStyle = value; }
        }
        [JsonProperty("headerPosition")]
        [DefaultValue(HeaderPosition.Top)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HeaderPosition HeaderPosition
        {
            get { return this.headerPosition; }
            set { this.headerPosition = value; }
        }
        //String Array
        [JsonProperty("disabledItems")]
        public List<String> DisabledItems
        {
            get { return this.disabledItems; }
            set { this.disabledItems = value; }
        }

        //String values
        [JsonProperty("events")]
        [DefaultValue("click")]
        public String Events
        {
            get { return this.events; }
            set { this.events = value; }
        }
        [JsonProperty("height")]
        [DefaultValue(null)]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(null)]
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
         [JsonProperty("idPrefix")]
        [DefaultValue("ej-tab-")]
        public String IdPrefix
        {
            get { return this.idPrefix; }
            set { this.idPrefix = value; }
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
        [JsonProperty("itemAdd")]
        [DefaultValue(null)]
        public String ItemAdd
        {
            get { return this.itemAdd; }
            set { this.itemAdd = value; }
        }
        [JsonProperty("itemRemove")]
        [DefaultValue(null)]
        public String ItemRemove
        {
            get { return this.itemRemove; }
            set { this.itemRemove= value; }
        }
        [JsonProperty("beforeItemRemove")]
        [DefaultValue(null)]
        public String BeforeItemRemove
        {
            get { return this.beforeItemRemove; }
            set { this.beforeItemRemove= value; }
        }
      [JsonProperty("itemEnable")]
        [DefaultValue(null)]
        public String ItemEnable
        {
            get { return this.itemEnable; }
            set { this.itemEnable= value; }
        }
        [JsonProperty("itemDisable")]
        [DefaultValue(null)]
        public String ItemDisable
        {
            get { return this.itemDisable; }
            set { this.itemDisable = value; }
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
        #endregion
        
        [JsonIgnore]
        public List<TabBaseItem> Items { get; set; }
    }
}
