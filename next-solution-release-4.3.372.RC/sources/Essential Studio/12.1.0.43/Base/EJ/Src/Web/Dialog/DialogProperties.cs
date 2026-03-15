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
    public class DialogProperties
    {
        #region Fields

        //Int Values
        private int minHeight = 150;
        private int minWidth = 150;
        private int width = 400;
        private int zIndex = 1000;

        //Boolean Values
        private bool autoOpen = true;
        private bool closeOnEscape = true;
        private bool draggable = true;
        private bool modal = false;
        private bool resizable = true;
        private bool windowResizing = false;
        private bool showHeader = true;
        private bool rtl = false;
        private bool allowKeyboardNavigation = true;
        private bool enabled = true;
        private bool roundedCorner = false;
        private bool persist = false;

        //String Values
        private String closeText = "close";
        private String height = "auto";
        private String maxHeight = null;
        private String maxWidth = null;
        private String content = null;
        private String loadUrl = null;
        private String title = "";
        private String cssClass = "";
        private String customIconCss = null;
        private String contentContainer = null;

        //Object Values
        private object position = new Position();
        private object ajaxOptions = new jQueryAjaxOptions();

        //String array
        private List<String> iconAction = new List<string>() { "close" };
        

        //Events
        private String create = null;
        private String beforeClose = null;
        private String close = null;
        private String beforeOpen = null;
        private String open = null;
        private String drag = null;
        private String dragStart = null;
        private String dragStop = null;
        private String resize = null;
        private String resizeStart = null;
        private String resizeStop = null;
        private String load = null;
        private String ajaxSuccess = null;
        private String ajaxError = null;
        private String destroy = null;

        //Dialog 
        private Dialog dialog = new Dialog();
       
        #endregion

        public DialogProperties() {
            this.Items = new DialogBaseItem();
        }
        //public DialogProperties(String id, Dialog dialog)
        //{
        //    dialog.ID = id;
        //    this.dialog = dialog;
        //}

        #region Properties
        [JsonProperty("minHeight")]
        [DefaultValue(150)]
        public int MinHeight
        {
            get { return this.minHeight; }
            set { this.minHeight = value; }
        }
        [JsonProperty("minWidth")]
        [DefaultValue(150)]
        public int MinWidth
        {
            get { return this.minWidth; }
            set { this.minWidth = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(400)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("zIndex")]
        [DefaultValue(1000)]
        public int ZIndex
        {
            get { return this.zIndex; }
            set { this.zIndex = value; }
        }

        //Boolean values
        [JsonProperty("autoOpen")]
        [DefaultValue(true)]
        public bool AutoOpen
        {
            get { return this.autoOpen; }
            set { this.autoOpen = value; }
        }
        [JsonProperty("closeOnEscape")]
        [DefaultValue(true)]
        public bool CloseOnEscape
        {
            get { return this.closeOnEscape; }
            set { this.closeOnEscape = value; }
        }
        [JsonProperty("draggable")]
        [DefaultValue(true)]
        public bool Draggable
        {
            get { return this.draggable; }
            set { this.draggable = value; }
        }
        [JsonProperty("modal")]
        [DefaultValue(false)]
        public bool Modal
        {
            get { return this.modal; }
            set { this.modal = value; }
        }
        [JsonProperty("resizable")]
        [DefaultValue(true)]
        public bool Resizable
        {
            get { return this.resizable; }
            set { this.resizable = value; }
        }
        [JsonProperty("windowResizing")]
        [DefaultValue(false)]
        public bool WindowResizing
        {
            get { return this.windowResizing; }
            set { this.windowResizing = value; }
        }
        [JsonProperty("showHeader")]
        [DefaultValue(true)]
        public bool ShowHeader
        {
            get { return this.showHeader; }
            set { this.showHeader = value; }
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

        //String values
        [JsonProperty("closeText")]
        [DefaultValue("close")]
        public String CloseText
        {
            get { return this.closeText; }
            set { this.closeText = value; }
        }
        [JsonProperty("height")]
        [DefaultValue("auto")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("maxHeight")]
        [DefaultValue(null)]
        public String MaxHeight
        {
            get { return this.maxHeight; }
            set { this.maxHeight = value; }
        }
        [JsonProperty("maxWidth")]
        [DefaultValue(null)]
        public String MaxWidth
        {
            get { return this.maxWidth; }
            set { this.maxWidth = value; }
        }
        [JsonProperty("content")]
        [DefaultValue(null)]
        public String Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
        [JsonProperty("loadUrl")]
        [DefaultValue(null)]
        public String LoadUrl
        {
            get { return this.loadUrl; }
            set { this.loadUrl = value; }
        }
        [JsonProperty("title")]
        [DefaultValue("")]
        public String Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("customIconCss")]
        [DefaultValue(null)]
        public String CustomIconCss
        {
            get { return this.customIconCss; }
            set { this.customIconCss = value; }
        }
        [JsonProperty("contentContainer")]
        [DefaultValue(null)]
        public String ContentContainer
        {
            get { return this.contentContainer; }
            set { this.contentContainer = value; }
        }

        //Object values
        [JsonProperty("ajaxOptions")]
        public object AjaxOptions
        {
            get { return this.ajaxOptions; }
            set { this.ajaxOptions = value; }
        }
        [JsonProperty("position")]
        public object Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        //String Array
        [JsonProperty("iconAction")]        
        public List<String> IconAction
        {
            get { return this.iconAction; }
            set { this.iconAction = value; }
        }

        //Event values
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("beforeClose")]
        [DefaultValue(null)]
        public String BeforeClose
        {
            get { return this.beforeClose; }
            set { this.beforeClose = value; }
        }
        [JsonProperty("close")]
        [DefaultValue(null)]
        public String Close
        {
            get { return this.close; }
            set { this.close = value; }
        }
        [JsonProperty("beforeOpen")]
        [DefaultValue(null)]
        public String BeforeOpen
        {
            get { return this.beforeOpen; }
            set { this.beforeOpen = value; }
        }
        [JsonProperty("open")]
        [DefaultValue(null)]
        public String Open
        {
            get { return this.open; }
            set { this.open = value; }
        }
        [JsonProperty("drag")]
        [DefaultValue(null)]
        public String Drag
        {
            get { return this.drag; }
            set { this.drag = value; }
        }
        [JsonProperty("dragStart")]
        [DefaultValue(null)]
        public String DragStart
        {
            get { return this.dragStart; }
            set { this.dragStart = value; }
        }
        [JsonProperty("dragStop")]
        [DefaultValue(null)]
        public String DragStop
        {
            get { return this.dragStop; }
            set { this.dragStop = value; }
        }
        [JsonProperty("resize")]
        [DefaultValue(null)]
        public String Resize
        {
            get { return this.resize; }
            set { this.resize = value; }
        }
        [JsonProperty("resizeStart")]
        [DefaultValue(null)]
        public String ResizeStart
        {
            get { return this.resizeStart; }
            set { this.resizeStart = value; }
        }
        [JsonProperty("resizeStop")]
        [DefaultValue(null)]
        public String ResizeStop
        {
            get { return this.resizeStop; }
            set { this.resizeStop = value; }
        }
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
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
        public bool ShouldSerializePosition()
        {
            if (Utils.PropertyCompare(Position, new Position()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeIconAction()
        {
            if (IconAction.Count != 0)
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
        #endregion
        [JsonIgnore]
        public DialogBaseItem Items { get; set; }
    }
}
