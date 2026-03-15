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
    public class MenuProperties
    {
        #region Fields
        //Boolean Values
        private bool openOnClick = false;
        private bool centerAlign = false;
        private bool showTopLevelArrows = true;
        private bool showBottomLevelArrows = true;
        private bool enableSeparator = true;
        private bool enabled = true;
        private bool rtl = false;
        //Enum Type
        private Orientation orientation = Orientation.Horizontal;
        private MenuType menuType = MenuType.NormalMenu;
        private Direction subMenuDirection = Direction.Right;
		private Animation animation = Animation.Default;
        //object
        private object fields = new MenuFields();
        //String Values
        private string height = "";
        private string width = "";
        private string contextTargetId = null;
        private string cssClass = "";
        //Events
        private string create = null;
        private string beforeContextOpen = null;
        private string contextOpen = null;
        private string contextClose = null;
        private string mouseOver = null;
        private string mouseOut = null;
        private string click = null;
        private string keyDown = null;
        private string destroy = null;

        //Menu 
        private Menu menu = new Menu();
        #endregion
        public MenuProperties() {
            this.Items = new List<MenuBaseItem>();
        }
        //public MenuProperties(String id, Menu menu)
        //{
        //    menu.ID = id;
        //    this.menu = menu;
        //}
        #region Properties
        [JsonProperty("openOnClick")]
        [DefaultValue(false)]
        public bool OpenOnClick
        {
            get { return this.openOnClick; }
            set { this.openOnClick = value; }
        }
        [JsonProperty("centerAlign")]
        [DefaultValue(false)]
        public bool CenterAlign
        {
            get { return this.centerAlign; }
            set { this.centerAlign = value; }
        }
        [JsonProperty("showTopLevelArrows")]
        [DefaultValue(true)]
        public bool ShowTopLevelArrows
        {
            get { return this.showTopLevelArrows; }
            set { this.showTopLevelArrows = value; }
        }
        [JsonProperty("showBottomLevelArrows")]
        [DefaultValue(false)]
        public bool ShowBottomLevelArrows
        {
            get { return this.showBottomLevelArrows; }
            set { this.showBottomLevelArrows = value; }
        }
        [JsonProperty("enableSeparator")]
        [DefaultValue(true)]
        public bool EnableSeparator
        {
            get { return this.enableSeparator; }
            set { this.enableSeparator = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        //Enum values
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }
        [JsonProperty("menuType")]
        [DefaultValue(MenuType.NormalMenu)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MenuType MenuType
        {
            get { return this.menuType; }
            set { this.menuType = value; }
        }
		[JsonProperty("animation")]
        [DefaultValue(Animation.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Animation Animation
        {
            get { return this.animation; }
            set { this.animation = value; }
        }
        [JsonProperty("subMenuDirection")]
        [DefaultValue(Direction.Right)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction SubMenuDirection
        {
            get { return this.subMenuDirection; }
            set { this.subMenuDirection = value; }
        }
        //object values
        [JsonProperty("fields")]
        public object MenuFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //string values
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("contextTargetId")]
        [DefaultValue(null)]
        public String ContextTargetId
        {
            get { return this.contextTargetId; }
            set { this.contextTargetId = value; }
        }
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
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("beforeContextOpen")]
        [DefaultValue(null)]
        public String BeforeContextOpen
        {
            get { return this.beforeContextOpen; }
            set { this.beforeContextOpen = value; }
        }
        [JsonProperty("contextOpen")]
        [DefaultValue(null)]
        public String ContextOpen
        {
            get { return this.contextOpen; }
            set { this.contextOpen = value; }
        }
        [JsonProperty("contextClose")]
        [DefaultValue(null)]
        public String ContextClose
        {
            get { return this.contextClose; }
            set { this.contextClose = value; }
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
        [JsonProperty("click")]
        [DefaultValue(null)]
        public String Click
        {
            get { return this.click; }
            set { this.click = value; }
        }
        [JsonProperty("keyDown")]
        [DefaultValue(null)]
        public String KeyDown
        {
            get { return this.keyDown; }
            set { this.keyDown = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        
        [JsonIgnore]
        public List<MenuBaseItem> Items
        {
            get;
            set;
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeMenuFields()
        {
            if (Utils.PropertyCompare(MenuFields, new MenuFields()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMenuBaseItem()
        {
            if (Utils.PropertyCompare(Items, new MenuBaseItem()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
