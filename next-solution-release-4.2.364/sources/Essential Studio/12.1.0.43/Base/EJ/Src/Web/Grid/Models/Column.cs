#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;


namespace Syncfusion.JavaScript.Models
{
    public class Column<T> where T:class
    {
        private String field = null;
        private String headerText = null;
        private bool allowGrouping = true;
        private bool allowSorting = true;
        private bool allowFiltering = true;
        private bool allowEditing = true;
        private bool columnTemplate = false;
        private EditingType editingType = EditingType.StringEdit;
        private TextAlign textAlign =TextAlign.Left;
        private bool isUnbound = false;
        private bool isIdentity = false;
        private bool key = false;
        private bool visible = true;
        private object dataSource = new object();
        private String foreignKeyField = null;
        private String foreignKeyValue = null;
        private String headerTemplateId = null;
        private String templateId = null;
        private int width;
        private string cssClass = null;
        private object defaultValue = null;
        private Dictionary<String, object> validationRules = new Dictionary<String, object>();
        private String format = null;
        private Dictionary<String, object> customAttributes = new Dictionary<string, object>();
        private List<Commands<T>> commands = new List<Commands<T>>();
        //private NumericEditParams numericeditparam = null;
        [JsonProperty("field")]
        [DefaultValue(null)]
        public String Field
        {
            get { return this.field; }
            set { this.field = value; }
        }
        [JsonProperty("headerText")]
        [DefaultValue(null)]
        public String HeaderText
        {
            get { return this.headerText; }
            set { this.headerText = value; }
        }
        [JsonProperty("allowGrouping")]
        [DefaultValue(true)]
        public bool AllowGrouping
        {
            get { return this.allowGrouping; }
            set { this.allowGrouping = value; }
        }
        [JsonProperty("allowSorting")]
        [DefaultValue(true)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }
        [JsonProperty("allowFiltering")]
        [DefaultValue(true)]
        public bool AllowFiltering
        {
            get { return this.allowFiltering; }
            set { this.allowFiltering = value; }
        }
        [JsonProperty("allowEditing")]
        [DefaultValue(true)]
        public bool AllowEditing
        {
            get { return this.allowEditing; }
            set { this.allowEditing = value; }
        }
        [JsonProperty("columnTemplate")]
        [DefaultValue(false)]
        public bool ColumnTemplate
        {
            get { return this.columnTemplate; }
            set { this.columnTemplate = value; }
        }
        [JsonProperty("columnEditType")]
        [DefaultValue(EditingType.StringEdit)]
        [JsonConverter(typeof(StringEnumConverter))]
        public EditingType EditingType
        {
            get { return this.editingType; }
            set { this.editingType = value; }
        }
        [JsonProperty("textAlign")]
        [DefaultValue(TextAlign.Left)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlign TextAlign
        {
            get { return this.textAlign; }
            set { this.textAlign = value; }
        }
        [JsonProperty("isUnBound")]
        [DefaultValue(false)]
        public bool IsUnbound
        {
            get { return this.isUnbound; }
            set { this.isUnbound = value; }
        }
        [JsonProperty("isIdentity")]
        [DefaultValue(false)]
        public bool IsIdentity
        {
            get { return this.isIdentity; }
            set { this.isIdentity = value; }
        }
        [JsonProperty("key")]
        [DefaultValue(false)]
        public bool Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("foreignKeyField")]
        [DefaultValue(null)]
        public String ForeignKeyField
        {
            get { return this.foreignKeyField; }
            set { this.foreignKeyField = value; }
        }
        [JsonProperty("foreignKeyValue")]
        [DefaultValue(null)]
        public String ForeignKeyValue
        {
            get { return this.foreignKeyValue; }
            set { this.foreignKeyValue = value; }
        }
        [JsonProperty("headerTemplateId")]
        [DefaultValue(null)]
        public String HeaderTemplateId
        {
            get { return this.headerTemplateId; }
            set { this.headerTemplateId = value; }
        }
        [JsonProperty("templateId")]
        [DefaultValue(null)]
        public String TemplateId
        {
            get { return this.templateId; }
            set { this.templateId = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(0)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        [JsonProperty("defaultValue")]
        public object DefaultValue
        {
            get { return this.defaultValue; }
            set { this.defaultValue = value; }
        }
        [JsonProperty("validationRules")]

        public Dictionary<String,object> ValidationRules
        {
            get { return this.validationRules; }
            set { this.validationRules = value; }
        }
        [JsonProperty("format")]
        [DefaultValue(null)]
        public String Format
        {
            get { return this.format; }
            set { this.format = value; }
        }
        [JsonProperty("customAttributes")]

        public Dictionary<String,object> CustomAttributes
        {
            get { return this.customAttributes; }
            set { this.customAttributes = value; }
        }
        [JsonProperty("commands")]
       
        public List<Commands<T>> Commands
        {
            get { return this.commands; }
            set { this.commands = value; }
        }
        
        
        #region ShouldSerialize Methods

        public bool ShouldSerializeCustomAttributes()
        {
            if (CustomAttributes.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCommands()
        {
            if (Commands.Count != 0)
                return true;
            else
                return false;
        }
       
        public bool ShouldSerializeValidationRules()
        {
            if (ValidationRules.Count != 0)
                return true;
            else
                return false;
        }

        #endregion
    }
}
