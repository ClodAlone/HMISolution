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
    public class TreeViewProperties
    {
        #region Fields
        //Boolean Values
        private bool showCheckbox = false;
        private bool dragAndDrop = false;
        private bool dropChild = false;
        private bool dropSibling = true;
        private bool dragAndDropAcrossControl = true;
        private bool allowEdit = false;
        private bool allowKeyboardNavigation = true;
        private bool autoCheckParentNode = false;
        private bool loadOnDemand = false;
        private bool rtl = false;
        private bool persist = false;
        private bool enabled = true;
        //Object Values
        private object fields = new TreeViewFields();
        private List<int> expandedNodes = new List<int>() { };        
        //String Values
        private String cssClass = "";
        private String template = null;
        private String expandEvent = "dblclick";
        private String width = null;
        private String height = null;
        //Events
        private String create = null;
        private String click = null;
        private String beforeExpand = null;
        private String beforeEdit = null;
        private String expand = null;
        private String beforeCollapse = null;
        private String collapse = null;
        private String select = null;
        private String check = null;
        private String uncheck = null;
        private String inlineEditValidation = null;
        private String keyPress = null;
        private String dragStart = null;
        private String drag = null;
        private String dragStop = null;
        private String dropped = null;
        private String destroy = null;
        //TreeView
        private TreeView treeview = new TreeView();
        #endregion
        public TreeViewProperties() {
            this.Items = new List<TreeViewBaseItem>();
        }
        //public TreeViewProperties(String id, TreeView treeview)
        //{
        //    treeview.ID = id;
        //    this.treeview = treeview;
        //}
         #region Properties
        //Boolean values
        [JsonProperty("showCheckbox")]
        [DefaultValue(false)]
        public bool ShowCheckbox
        {
            get { return this.showCheckbox; }
            set { this.showCheckbox = value; }
        }
        [JsonProperty("dragAndDrop")]
        [DefaultValue(false)]
        public bool DragAndDrop
        {
            get { return this.dragAndDrop; }
            set { this.dragAndDrop = value; }
        }
        [JsonProperty("dropChild")]
        [DefaultValue(false)]
        public bool DropChild
        {
            get { return this.dropChild; }
            set { this.dropChild = value; }
        }
        [JsonProperty("dropSibling")]
        [DefaultValue(true)]
        public bool DropSibling
        {
            get { return this.dropSibling; }
            set { this.dropSibling = value; }
        }
        [JsonProperty("dragAndDropAcrossControl")]
        [DefaultValue(true)]
        public bool DragAndDropAcrossControl
        {
            get { return this.dragAndDropAcrossControl; }
            set { this.dragAndDropAcrossControl = value; }
        }
        [JsonProperty("allowEdit")]
        [DefaultValue(false)]
        public bool AllowEdit
        {
            get { return this.allowEdit; }
            set { this.allowEdit = value; }
        }
        [JsonProperty("allowKeyboardNavigation")]
        [DefaultValue(true)]
        public bool AllowKeyboardNavigation
        {
            get { return this.allowKeyboardNavigation; }
            set { this.allowKeyboardNavigation = value; }
        }
        [JsonProperty("autoCheckParentNode")]
        [DefaultValue(false)]
        public bool AutoCheckParentNode
        {
            get { return this.autoCheckParentNode; }
            set { this.autoCheckParentNode = value; }
        }
        [JsonProperty("loadOnDemand")]
        [DefaultValue(false)]
        public bool LoadOnDemand
        {
            get { return this.loadOnDemand; }
            set { this.loadOnDemand = value; }
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
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        //object values
        [JsonProperty("fields")]
        public object TreeViewFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //Array values
        [JsonProperty("expandedNodes")]
        public List<int> ExpandedNodes
        {
            get { return this.expandedNodes; }
            set { this.expandedNodes = value; }
        }
        //string values
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("template")]
        [DefaultValue(null)]
        public String Template
        {
            get { return this.template; }
            set { this.template = value; }
        }
        [JsonProperty("expandEvent")]
        [DefaultValue("dblclick")]
        public String ExpandEvent
        {
            get { return this.expandEvent; }
            set { this.expandEvent = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(null)]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("height")]
        [DefaultValue(null)]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
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
        [JsonProperty("beforeExpand")]
        [DefaultValue(null)]
        public String BeforeExpand
        {
            get { return this.beforeExpand; }
            set { this.beforeExpand = value; }
        }
        [JsonProperty("expand")]
        [DefaultValue(null)]
        public String Expand
        {
            get { return this.expand; }
            set { this.expand = value; }
        }
        [JsonProperty("beforeCollapse")]
        [DefaultValue(null)]
        public String BeforeCollapse
        {
            get { return this.beforeCollapse; }
            set { this.beforeCollapse = value; }
        }
        [JsonProperty("collapse")]
        [DefaultValue(null)]
        public String Collapse
        {
            get { return this.collapse; }
            set { this.collapse = value; }
        }
        [JsonProperty("select")]
        [DefaultValue(null)]
        public String Select
        {
            get { return this.select; }
            set { this.select = value; }
        }
        [JsonProperty("check")]
        [DefaultValue(null)]
        public String Check
        {
            get { return this.check; }
            set { this.check = value; }
        }
        [JsonProperty("uncheck")]
        [DefaultValue(null)]
        public String Uncheck
        {
            get { return this.uncheck; }
            set { this.uncheck = value; }
        }
        [JsonProperty("inlineEditValidation")]
        [DefaultValue(null)]
        public String InlineEditValidation
        {
            get { return this.inlineEditValidation; }
            set { this.inlineEditValidation = value; }
        }

        [JsonProperty("beforeEdit")]
        [DefaultValue(null)]
        public String BeforeEdit
        {
            get { return this.beforeEdit; }
            set { this.beforeEdit = value; }
        }
        [JsonProperty("keyPress")]
        [DefaultValue(null)]
        public String KeyPress
        {
            get { return this.keyPress; }
            set { this.keyPress = value; }
        }
        [JsonProperty("dragStart")]
        [DefaultValue(null)]
        public String DragStart
        {
            get { return this.dragStart; }
            set { this.dragStart = value; }
        }
        [JsonProperty("drag")]
        [DefaultValue(null)]
        public String Drag
        {
            get { return this.drag; }
            set { this.drag = value; }
        }
        [JsonProperty("dragStop")]
        [DefaultValue(null)]
        public String DragStop
        {
            get { return this.dragStop; }
            set { this.dragStop = value; }
        }
        [JsonProperty("dropped")]
        [DefaultValue(null)]
        public String Dropped
        {
            get { return this.dropped; }
            set { this.dropped = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        [JsonIgnore]
        public List<TreeViewBaseItem> Items
        {
            get;
            set;
        }
        #endregion
        #region ShouldSerialize Methods
        
        public bool ShouldSerializeTreeViewFields()
        {
            if (Utils.PropertyCompare(TreeViewFields, new TreeViewFields()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeExpandedNodes()
        {
            if (ExpandedNodes.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTreeViewBaseItem()
        {
            if (Utils.PropertyCompare(Items, new TreeViewBaseItem()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
