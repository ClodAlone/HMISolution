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
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript.Models
{
    public class RTEtools
    {
        #region Fields
        private List<String> toolsList = new List<String>() { "font", "style", "alignment", "lists", "indenting", "copyPaste", "doAction", "links", "images", "tables", "scripts", "casing", "formatStyle", "custom", "clear" };
        private List<String> font = new List<String>() { "fontName", "fontSize", "fontColor", "backgrounColor" };
        private List<String> style = new List<String>() { "bold", "italic", "underline", "strikethrough" };
        private List<String> alignment = new List<String>() { "justifyLeft", "justifyCenter", "justifyRight", "justifyFull" };
        private List<String> lists = new List<String>() { "unorderedList", "orderedList" };
        private List<String> indenting = new List<String>() { "outdent", "indent" };
        private List<String> copyPaste = new List<String>() { "cut", "copy", "paste" };
        private List<String> doAction = new List<String>() { "undo", "redo" };
        private List<String> links = new List<String>() { "createLink", "unlink" };
        private List<String> images = new List<String>() { "image", "video" };
        private List<String> clear = new List<String>() { "clearFormat", "clearAll" };
        private List<String> tables = new List<String>() { "createTable" };
        private List<String> scripts = new List<String>() { "superscript", "subscript" };
        private List<String> casing = new List<String>() { "upperCase", "lowerCase" };
        private List<String> formatStyle = new List<String>() { "format" };       
        private object custom = new CustomTool();
        #endregion
        #region Properties
        [JsonProperty("style")]
        public List<String> Style
        {
            get { return this.style; }
            set { this.style = value; }
        }
        [JsonProperty("toolsList")]
        public List<String> ToolsList
        {
            get { return this.toolsList; }
            set { this.toolsList = value; }
        }
        [JsonProperty("font")]
        public List<String> Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        [JsonProperty("alignment")]
        public List<String> Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }
        [JsonProperty("lists")]
        public List<String> Lists
        {
            get { return this.lists; }
            set { this.lists = value; }
        }
        [JsonProperty("indenting")]
        public List<String> Indenting
        {
            get { return this.indenting; }
            set { this.indenting = value; }
        }
        [JsonProperty("copyPaste")]
        public List<String> CopyPaste
        {
            get { return this.copyPaste; }
            set { this.copyPaste = value; }
        }
        [JsonProperty("doAction")]
        public List<String> DoAction
        {
            get { return this.doAction; }
            set { this.doAction = value; }
        }
        [JsonProperty("clear")]
        public List<String> Clear
        {
            get { return this.clear; }
            set { this.clear = value; }
        }
        [JsonProperty("links")]
        public List<String> Links
        {
            get { return this.links; }
            set { this.links = value; }
        }
        [JsonProperty("images")]
        public List<String> Images
        {
            get { return this.images; }
            set { this.images = value; }
        }
        [JsonProperty("tables")]
        public List<String> Tables
        {
            get { return this.tables; }
            set { this.tables = value; }
        }
        [JsonProperty("scripts")]
        public List<String> Scripts
        {
            get { return this.scripts; }
            set { this.scripts = value; }
        }
        [JsonProperty("casing")]
        public List<String> Casing
        {
            get { return this.casing; }
            set { this.casing = value; }
        }
        [JsonProperty("formatStyle")]
        public List<String> FormatStyle
        {
            get { return this.formatStyle; }
            set { this.formatStyle = value; }
        }
        [JsonProperty("custom")]
        public object CustomTool
        {
            get { return this.custom; }
            set { this.custom = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeCustomTool()
        {
            if (Utils.PropertyCompare(CustomTool, new CustomTool()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeFont()
        {
            if (Font.Count != 0)
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
        public bool ShouldSerializeStyle()
        {
            if (Style.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeAlignment()
        {
            if (Alignment.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeLists()
        {
            if (Lists.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeClear()
        {
            if (Clear.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeImages()
        {
            if (Images.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTables()
        {
            if (Tables.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeScripts()
        {
            if (Scripts.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeIndenting()
        {
            if (Indenting.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCopyPaste()
        {
            if (CopyPaste.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDoAction()
        {
            if (DoAction.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeLinks()
        {
            if (Links.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCasing()
        {
            if (Casing.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeFormatStyle()
        {
            if (FormatStyle.Count != 0)
                return true;
            else
                return false;
        }        
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    
    public class RTEtoolsBuilder
    {
        private RTEtools tools = new RTEtools();
        public RTEtoolsBuilder(RTEtools tools)
        {
            this.tools=tools;
        }
        public RTEtoolsBuilder Font(List<String> font)
        {
            this.tools.Font = font;
            return this;
        }
        public RTEtoolsBuilder Style(List<String> style)
        {
            this.tools.Style=style;
           return this;
        }
        public RTEtoolsBuilder ToolsList(List<String> toolsList)
        {
            this.tools.ToolsList = toolsList;
            return this;
        }
        public RTEtoolsBuilder Clear(List<String> clear)
        {
            this.tools.Clear = clear;
            return this;
        }
        public RTEtoolsBuilder Alignment(List<String> alignment)
        {
            this.tools.Alignment = alignment;
            return this;
        }
        public RTEtoolsBuilder Lists(List<String> lists)
        {
            this.tools.Lists = lists;
            return this;
        }
        public RTEtoolsBuilder Indenting(List<String> indenting)
        {
            this.tools.Indenting = indenting;
            return this;
        }
        public RTEtoolsBuilder CopyPaste(List<String> copyPaste)
        {
            this.tools.CopyPaste = copyPaste;
            return this;
        }

        public RTEtoolsBuilder DoAction(List<String> doAction)
        {
            this.tools.DoAction = doAction;
            return this;
        }
        public RTEtoolsBuilder Images(List<String> images)
        {
            this.tools.Images = images;
            return this;
        }
        public RTEtoolsBuilder Links(List<String> links)
        {
            this.tools.Links = links;
            return this;
        }
        public RTEtoolsBuilder Tables(List<String> tables)
        {
            this.tools.Tables = tables;
            return this;
        }
        public RTEtoolsBuilder Scripts(List<String> scripts)
        {
            this.tools.Scripts = scripts;
            return this;
        }

        public RTEtoolsBuilder Casing(List<String> casing)
        {
            this.tools.Casing = casing;
            return this;
        }
        public RTEtoolsBuilder FormatStyle(List<String> formatStyle)
        {
            this.tools.FormatStyle = formatStyle;
            return this;
        }
        public RTEtoolsBuilder CustomTool(Action<CustomToolBuilder> custom)
        {
            var t = new CustomTool();
            this.tools.CustomTool = t;
            var builder = new CustomToolBuilder(t);
            if (custom != null)
                custom.Invoke(builder);
            return this;
        }
        
    
    }
}
