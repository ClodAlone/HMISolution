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
using Microsoft.SqlServer.Server;
using System.Collections.ObjectModel;

namespace Syncfusion.JavaScript.Models
{
    public class RTEproperties
    {
        #region Fields
        //Boolean Values
        private bool allowEdit = true;
        private bool allowKeyboardNavigation = true;
        private bool enabled = true;
        private bool showToolBar = true;
        private bool showHtmlSource = true;
        private bool showWordCount = true;
        private bool showHtmlTagInfo = true;
        private bool showClearAll = true;
        private bool showClearFormat = true;
        private bool showFontOption = true;
        private bool showCustomTable = true;
        private bool showFooter = false;
        private bool rtl = false;
        private bool persist = false;
        private bool resizable = true;

        //String Values
        private String cssClass = "";
        private String width = "786";
        private String height = "370";
        private String maxWidth = null;
        private String maxHeight = null;
        private String minWidth = "400";
        private String minHeight = "280";
        private String value = null;
        private String localization = "en-US";
        private String name = "";

        //Int values
        private int maxLength = 1000;
        private int tableRows=7;
        private int tableColumns=9;
        private int colorPaletteRows=5;
        private int colorPaletteColumns=8;
        private int undoStackLimit=50;

       //Events
        private String create = null;
        private String change = null;
        private String execute = null;
        private String keydown = null;
        private String keyup = null;
        private String destroy = null;
        private String iframeAttribute = "";
        private List<String> toolsList = new List<String>() { "font", "style", "alignment", "lists", "indenting", "copyPaste", "doAction", "links", "images", "tables", "scripts", "casing", "formatStyle","clear", "custom" };
        private List<String> colorCode = new List<String>(){"000000", "993300", "333300", "003300", "003366", "000080", "333399", "333333", "800000", "FF6600", "808000", "008000", "008080", "0000FF", "666699", "808080", "FF0000",
			            "FF9900", "99CC00", "339966", "33CCCC", "3366FF", "800080", "999999", "FF00FF", "FFCC00", "FFFF00", "00FF00", "00FFFF", "00CCFF", "993366", "C0C0C0", "FF99CC", "FFCC99",
			            "FFFF99", "CCFFCC", "CCFFFF", "99CCFF", "CC99FF", "FFFFFF"};       

        private List<Format> format = new List<Format>();
        private List<FontName> fontName = new List<FontName>();
        private List<FontSize> fontSize = new List<FontSize>();
        //Object values
        private object tools = new RTEtools();

         //Auto Complete
        private RTE rte = new RTE();
        #endregion

        public RTEproperties()
        {
            this.Items = new RTEBaseItem();
        }
        //public RTEproperties(String id, RTE rte)
        //{
        //    rte.ID = id;
        //    this.rte = rte;
        //}
        #region Properties
        //Boolean values
        [JsonProperty("allowEdit")]
        [DefaultValue(true)]
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
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("showToolBar")]
        [DefaultValue(true)]
        public bool ShowToolBar
        {
            get { return this.showToolBar; }
            set { this.showToolBar = value; }
        }
        [JsonProperty("showHtmlSource")]
        [DefaultValue(true)]
        public bool ShowHtmlSource
        {
            get { return this.showHtmlSource; }
            set { this.showHtmlSource = value; }
        }
        [JsonProperty("showWordCount")]
        [DefaultValue(true)]
        public bool ShowWordCount
        {
            get { return this.showWordCount; }
            set { this.showWordCount = value; }
        }
        [JsonProperty("showHtmlTagInfo")]
        [DefaultValue(true)]
        public bool ShowHtmlTagInfo
        {
            get { return this.showHtmlTagInfo; }
            set { this.showHtmlTagInfo = value; }
        }
        [JsonProperty("showClearAll")]
        [DefaultValue(true)]
        public bool ShowClearAll
        {
            get { return this.showClearAll; }
            set { this.showClearAll = value; }
        }
        [JsonProperty("showClearFormat")]
        [DefaultValue(true)]
        public bool ShowClearFormat
        {
            get { return this.showClearFormat; }
            set { this.showClearFormat = value; }
        }
        [JsonProperty("showFontOption")]
        [DefaultValue(true)]
        public bool ShowFontOption
        {
            get { return this.showFontOption; }
            set { this.showFontOption = value; }
        }
        [JsonProperty("showCustomTable")]
        [DefaultValue(true)]
        public bool ShowCustomTable
        {
            get { return this.showCustomTable; }
            set { this.showCustomTable = value; }
        }
        [JsonProperty("showFooter")]
        [DefaultValue(false)]
        public bool ShowFooter
        {
            get { return this.showFooter; }
            set { this.showFooter = value; }
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
        [JsonProperty("resizable")]
        [DefaultValue(true)]
        public bool Resizable
        {
            get { return this.resizable; }
            set { this.resizable = value; }
        }
        //
        //String values
        [JsonProperty("iframeAttribute")]
        [DefaultValue("")]
        public String IFrameAttribute
        {
            get { return this.iframeAttribute; }
            set { this.iframeAttribute = value; }
        }
        [JsonProperty("fontName")]
        public List<FontName> FontName
        {
            get { return this.fontName; }
            set { this.fontName = value; }
        }
        [JsonProperty("fontSize")]
        public List<FontSize> FontSize
        {
            get { return this.fontSize; }
            set { this.fontSize = value; }
        }
        [JsonProperty("format")]
        public List<Format> Format
        {
            get { return this.format; }
            set { this.format = value; }
        }
        //String values
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("localization")]
        [DefaultValue("en-US")]
        public String Localization
        {
            get { return this.localization; }
            set { this.localization = value; }
        }
        [JsonProperty("width")]
        [DefaultValue("786")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("height")]
        [DefaultValue("370")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("maxWidth")]
        [DefaultValue("")]
        public String MaxWidth
        {
            get { return this.maxWidth; }
            set { this.maxWidth = value; }
        }
        [JsonProperty("maxHeight")]
        [DefaultValue("")]
        public String MaxHeight
        {
            get { return this.maxHeight; }
            set { this.maxHeight = value; }
        }
        [JsonProperty("minWidth")]
        [DefaultValue("400")]
        public String MinWidth
        {
            get { return this.minWidth; }
            set { this.minWidth = value; }
        }
        [JsonProperty("minHeight")]
        [DefaultValue("280")]
        public String MinHeight
        {
            get { return this.minHeight; }
            set { this.minHeight = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("name")]
        [DefaultValue("")]
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        [JsonProperty("maxLength")]
        [DefaultValue(1000)]
        public int MaxLength
        {
            get { return this.maxLength; }
            set { this.maxLength = value; }
        }
        [JsonProperty("tableRows")]
        [DefaultValue(7)]
        public int TableRows
        {
            get { return this.tableRows; }
            set { this.tableRows = value; }
        }
        [JsonProperty("tableColumns")]
        [DefaultValue(9)]
        public int TableColumns
        {
            get { return this.tableColumns; }
            set { this.tableColumns = value; }
        }
        [JsonProperty("colorPaletteRows")]
        [DefaultValue(5)]
        public int ColorPaletteRows
        {
            get { return this.colorPaletteRows; }
            set { this.colorPaletteRows = value; }
        }
        [JsonProperty("colorPaletteColumns")]
        [DefaultValue(8)]
        public int ColorPaletteColumns
        {
            get { return this.colorPaletteColumns; }
            set { this.colorPaletteColumns = value; }
        }
        [JsonProperty("undoStackLimit")]
        [DefaultValue(50)]
        public int UndoStackLimit
        {
            get { return this.undoStackLimit; }
            set { this.undoStackLimit = value; }
        }  
        //string array
        [JsonProperty("colorCode")]
        public List<String> ColorCode
        {
            get { return this.colorCode; }
            set { this.colorCode = value; }
        }
        [JsonProperty("toolsList")]
        public List<String> ToolsList
        {
            get { return this.toolsList; }
            set { this.toolsList = value; }
        }
        //objects
        [JsonProperty("tools")]
        public object Tools
        {
            get { return this.tools; }
            set { this.tools = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("execute")]
        [DefaultValue(null)]
        public String Execute
        {
            get { return this.execute; }
            set { this.execute = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("keydown")]
        [DefaultValue(null)]
        public String Keydown
        {
            get { return this.keydown; }
            set { this.keydown = value; }
        }
        [JsonProperty("keyup")]
        [DefaultValue(null)]
        public String Keyup
        {
            get { return this.keyup; }
            set { this.keyup = value; }
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
        public bool ShouldSerializeRTEtools()
        {
            if (Utils.PropertyCompare(Tools, new RTEtools()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeToolsList()
        {
            if (ToolsList.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeColorCode()
        {
            if (ColorCode.Count != 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFontName()
        {
            if (FontName.Count !=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeFontSize()
        {
            if (FontSize.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeFormat()
        {
            if (Format.Count != 0)
                return true;
            else
                return false;
        }
       #endregion

        [JsonIgnore]
        public RTEBaseItem Items { get; set; }
    }
}
