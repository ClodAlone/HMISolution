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
    public class DropDownListProperties
    {
        #region Fields
        //Boolean Values
        private bool roundedCorner = false;
        private bool showPopupOnLoad = false;
        private bool rtl = false;
        private bool enabled = true;
        private bool caseSensitive = false;
        private bool showCheckbox = false;
        private bool checkAll = false;
        private bool uncheckAll = false;
        private bool persist = false;
        private bool incrementalSearch = false;
        private bool readOnly = false;
        private bool boxmodel = true;
        private bool multiSelectMode = false;
        //object Values
        private object dataSource = new object();
        private object fields = new DropDownListFields();
        //array values
        private List<int> multiSelectedItemsIndex = new List<int>() { };        
        //Integer Values
        private int listSize = 0;
        private int selectedItem = -1;
        //String Values
        private String cssClass = "";
        private String value = "";
        private String itemValue = "";
        private String text = "";
        private String height = "";
        private String width = "";
        private String popupPanelHeight = "152";
        private String popupPanelWidth = "auto";
        private String targetId = null;
        private String template = null;
        private String selectedTo = null;
        private string query = null;
        private String waterMark = null;
        //Events
        private String create = null;
        private String popupHide = null;
        private String popupShown = null;
        private String beforePopupShown = null;
        private String change = null;
        private String select = null;
        private String checkChange = null;
        private String destroy = null;
        //DropDownList
        private DropDownList dropDownList = new DropDownList();
        #endregion
        public DropDownListProperties() {  }
        //public DropDownListProperties(String id, DropDownList dropDownList)
        //{
        //    dropDownList.ID = id;
        //    this.dropDownList = dropDownList;
        //}
        #region Properties
        //Boolean values
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        [JsonProperty("showPopupOnLoad")]
        [DefaultValue(false)]
        public bool ShowPopupOnLoad
        {
            get { return this.showPopupOnLoad; }
            set { this.showPopupOnLoad = value; }
        }
        [JsonProperty("multiSelectMode")]
        [DefaultValue(false)]
        public bool MultiSelectMode
        {
            get { return this.multiSelectMode; }
            set { this.multiSelectMode = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("caseSensitive")]
        [DefaultValue(false)]
        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set { this.caseSensitive = value; }
        }
        [JsonProperty("showCheckbox")]
        [DefaultValue(false)]
        public bool ShowCheckbox
        {
            get { return this.showCheckbox; }
            set { this.showCheckbox = value; }
        }
        [JsonProperty("checkAll")]
        [DefaultValue(false)]
        public bool CheckAll
        {
            get { return this.checkAll; }
            set { this.checkAll = value; }
        }
        [JsonProperty("uncheckAll")]
        [DefaultValue(false)]
        public bool UncheckAll
        {
            get { return this.uncheckAll; }
            set { this.uncheckAll = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        [JsonProperty("incrementalSearch")]
        [DefaultValue(false)]
        public bool IncrementalSearch
        {
            get { return this.incrementalSearch; }
            set { this.incrementalSearch = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("boxmodel")]
        [DefaultValue(false)]
        public bool Boxmodel
        {
            get { return this.boxmodel; }
            set { this.boxmodel = value; }
        }
        //object values
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("fields")]
        public object DropDownListFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //array values
        [JsonProperty("multiSelectedItemsIndex")]
        public List<int> MultiSelectedItemsIndex
        {
            get { return this.multiSelectedItemsIndex; }
            set { this.multiSelectedItemsIndex = value; }
        }
        //Integer Values
        [JsonProperty("listSize")]
        [DefaultValue(0)]
        public int ListSize
        {
            get { return this.listSize; }
            set { this.listSize = value; }
        }
        [JsonProperty("selectedItem")]
        [DefaultValue(-1)]
        public int SelectedItem
        {
            get { return this.selectedItem; }
            set { this.selectedItem = value; }
        }
        //String values
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
        [JsonProperty("value")]
        [DefaultValue("")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("itemValue")]
        [DefaultValue("")]
        public String ItemValue
        {
            get { return this.itemValue; }
            set { this.itemValue = value; }
        }
        [JsonProperty("text")]
        [DefaultValue("")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("popupPanelHeight")]
        [DefaultValue("152")]
        public String PopupPanelHeight
        {
            get { return this.popupPanelHeight; }
            set { this.popupPanelHeight = value; }
        }
        [JsonProperty("popupPanelWidth")]
        [DefaultValue("auto")]
        public String PopupPanelWidth
        {
            get { return this.popupPanelWidth; }
            set { this.popupPanelWidth = value; }
        }
        [JsonProperty("targetId")]
        [DefaultValue(null)]
        public String TargetId
        {
            get { return this.targetId; }
            set { this.targetId = value; }
        }
        [JsonProperty("waterMark")]
        [DefaultValue(null)]
        public String WaterMark
        {
            get { return this.waterMark; }
            set { this.waterMark = value; }
        }
        [JsonProperty("template")]
        [DefaultValue(null)]
        public String Template
        {
            get { return this.template; }
            set { this.template = value; }
        }
        [JsonProperty("selectedTo")]
        [DefaultValue(null)]
        public String SelectedTo
        {
            get { return this.selectedTo; }
            set { this.selectedTo = value; }
        }
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("popupHide")]
        [DefaultValue(null)]
        public String PopupHide
        {
            get { return this.popupHide; }
            set { this.popupHide = value; }
        }
        [JsonProperty("popupShown")]
        [DefaultValue(null)]
        public String PopupShown
        {
            get { return this.popupShown; }
            set { this.popupShown = value; }
        }
        [JsonProperty("beforePopupShown")]
        [DefaultValue(null)]
        public String BeforePopupShown
        {
            get { return this.beforePopupShown; }
            set { this.beforePopupShown = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("select")]
        [DefaultValue(null)]
        public String Select
        {
            get { return this.select; }
            set { this.select = value; }
        }
        [JsonProperty("checkChange")]
        [DefaultValue(null)]
        public String CheckChange
        {
            get { return this.checkChange; }
            set { this.checkChange = value; }
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
        public bool ShouldSerializeDropDownListFields()
        {
            if (Utils.PropertyCompare(DropDownListFields, new DropDownListFields()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMultiSelectedItemsIndex()
        {
            if (MultiSelectedItemsIndex.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDataSource()
        {
            if (typeof(DataSource).IsAssignableFrom(this.DataSource.GetType()))
            {
                if (Utils.PropertyCompare(DataSource, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.DataSource is IEnumerable)
            {
                ICollection data = DataSource as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        #endregion
    }
}
