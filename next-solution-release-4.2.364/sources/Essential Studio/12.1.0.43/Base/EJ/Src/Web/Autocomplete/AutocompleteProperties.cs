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
    public class AutocompleteProperties
    {
        #region Fields
        //Boolean Values
        private bool grouping = false;
        private bool distinct = false;
        private bool allowSorting = false;
        private bool roundedCorner = false;
        private bool readOnly = false;
        private bool caseSensitive = false;
        private bool loadingImage = true;
        private bool dropdown = false;
        private bool highlightSearch = false;
        private bool autoFill = false;
        private bool rtl = false;
        private bool enabled = true;
        private bool showNoResults = true;
        private bool persist = false;        
        private bool allowNew = false;

        //Enumeration Values
        private FilterOperatorType filter = FilterOperatorType.StartsWith;
        private SortOrder sortBy = SortOrder.Ascending;
        private MultiSelectModeTypes multiSelectMode = MultiSelectModeTypes.None;
        //Object Values
        private object dataSource = new object();
        private object fields = new AutocompleteFields();        
        //String Values
        private String template = null;
        private String cssClass = "";
        private String watermark = null;
        private String value = "";
        private String delimiter = "";
        private String height = "";
        private String width = "";
        private String noResults = "No Suggestions";
        private String suggestionBoxHeight = "152px";
        private String suggestionBoxWidth = "auto";
        private string query = null;
        private string addNewText = "Add New";
        //Events
        private String create = null;
        private String focusIn = null;
        private String focusOut = null;
        private String change = null;
        private String select = null;
        private String destroy = null;
        //Integer Values 
        private int listSize = 0;
        private int minCharacter = 1;

        //Auto Complete
        private Autocomplete autocomplete = new Autocomplete();
        #endregion
        public AutocompleteProperties() {  }
        //public AutocompleteProperties(String id, Autocomplete autocomplete)
        //{
        //    autocomplete.ID = id;
        //    this.autocomplete = autocomplete;
        //}
        #region Properties
        //Boolean values
        [JsonProperty("grouping")]
        [DefaultValue(false)]
        public bool Grouping
        {
            get { return this.grouping; }
            set { this.grouping = value; }
        }
        [JsonProperty("distinct")]
        [DefaultValue(false)]
        public bool Distinct
        {
            get { return this.distinct; }
            set { this.distinct = value; }
        }
        [JsonProperty("allowSorting")]
        [DefaultValue(false)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("caseSensitive")]
        [DefaultValue(false)]
        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set { this.caseSensitive = value; }
        }
        [JsonProperty("loadingImage")]
        [DefaultValue(true)]
        public bool LoadingImage
        {
            get { return this.loadingImage; }
            set { this.loadingImage = value; }
        }
        [JsonProperty("dropdown")]
        [DefaultValue(false)]
        public bool Dropdown
        {
            get { return this.dropdown; }
            set { this.dropdown = value; }
        }
        [JsonProperty("highlightSearch")]
        [DefaultValue(false)]
        public bool HighlightSearch
        {
            get { return this.highlightSearch; }
            set { this.highlightSearch = value; }
        }
        [JsonProperty("autoFill")]
        [DefaultValue(false)]
        public bool AutoFill
        {
            get { return this.autoFill; }
            set { this.autoFill = value; }
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
        [JsonProperty("showNoResults")]
        [DefaultValue(true)]
        public bool ShowNoResults
        {
            get { return this.showNoResults; }
            set { this.showNoResults = value; }
        }        
        [JsonProperty("allowNew")]
        [DefaultValue(false)]
        public bool AllowNew
        {
            get { return this.allowNew; }
            set { this.allowNew = value; }
        }
        //Enum values
        [JsonProperty("filter")]
        [DefaultValue(FilterOperatorType.StartsWith)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterOperatorType Filter
        {
            get { return this.filter; }
            set { this.filter = value; }
        }
        [JsonProperty("sortBy")]
        [DefaultValue(SortOrder.Ascending)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SortOrder SortBy
        {
            get { return this.sortBy; }
            set { this.sortBy = value; }
        }
        [JsonProperty("multiSelectMode")]
        [DefaultValue(MultiSelectModeTypes.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MultiSelectModeTypes MultiSelectMode
        {
            get { return this.multiSelectMode; }
            set { this.multiSelectMode = value; }
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
        public object AutocompleteFields
        {
            get { return this.fields; }
            set { this.fields = value; }
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
        [JsonProperty("addNewText")]
        [DefaultValue("Add New")]
        public String AddNewText
        {
            get { return this.addNewText; }
            set { this.addNewText = value; }
        }
        [JsonProperty("delimiter")]
        [DefaultValue("")]
        public String Delimiter
        {
            get { return this.delimiter; }
            set { this.delimiter = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("template")]
        [DefaultValue(null)]
        public String Template
        {
            get { return this.template; }
            set { this.template = value; }
        }
        [JsonProperty("watermark")]
        [DefaultValue(null)]
        public String Watermark
        {
            get { return this.watermark; }
            set { this.watermark = value; }
        }
        [JsonProperty("noResults")]
        [DefaultValue("No Suggestions")]
        public String NoResults
        {
            get { return this.noResults; }
            set { this.noResults = value; }
        }
        [JsonProperty("suggestionBoxHeight")]
        [DefaultValue("152px")]
        public String SuggestionBoxHeight
        {
            get { return this.suggestionBoxHeight; }
            set { this.suggestionBoxHeight = value; }
        }
        [JsonProperty("suggestionBoxWidth")]
        [DefaultValue("auto")]
        public String SuggestionBoxWidth
        {
            get { return this.suggestionBoxWidth; }
            set { this.suggestionBoxWidth = value; }
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
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        //Integer values
        [JsonProperty("listSize")]
        [DefaultValue(0)]
        public int ListSize
        {
            get { return this.listSize; }
            set { this.listSize = value; }
        }
        [JsonProperty("minCharacter")]
        [DefaultValue(1)]
        public int MinCharacter
        {
            get { return this.minCharacter; }
            set { this.minCharacter = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeAutocompleteFields()
        {
            if (Utils.PropertyCompare(AutocompleteFields, new AutocompleteFields()))
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
