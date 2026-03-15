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
    public class SplitterProperties
    {

        #region Fields

        //Boolean Values
        private bool windowResizing = false;
        private bool rtl = false;
        //Enumeration Values
        private Orientation orientation = Orientation.Horizontal;
        
        //list Values
        private List<PaneProperties> properties = new List<PaneProperties>();

        //String Values
        private String height = "";
        private String width = "";
        private String cssClass = "";
        //Integer Values 
        private int animationSpeed = 300;
        //Events 
        private String create = null;
        private String beforeExpandCollapse= null;
        private String expandCollapse=null;
        private String resize = null;
        private String destroy = null;

        //Button 
        private Splitter splitter = new Splitter();

        #endregion

        public SplitterProperties() {

        }
        //public SplitterProperties(String id, Splitter splitter)
        //{
        //    splitter.ID = id;
        //    this.splitter = splitter;
        //}

        #region Properties
        [JsonProperty("windowResizing")]
        [DefaultValue(false)]
        public bool WindowResizing
        {
            get { return this.windowResizing; }
            set { this.windowResizing = value; }
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
       
        //string values
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
       //Integer 
        [JsonProperty("animationSpeed")]
        [DefaultValue(300)]
        public int AnimationSpeed
        {
            get { return this.animationSpeed; }
            set { this.animationSpeed = value; }
        }
        [JsonProperty("properties")]
        public List<PaneProperties> PaneProperties
        {
            get { return this.properties; }
            set { this.properties = value; }
        }
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("beforeExpandCollapse")]
        [DefaultValue(null)]
        public String BeforeExpandCollapse
        {
            get { return this.beforeExpandCollapse; }
            set { this.beforeExpandCollapse = value; }
        }
        [JsonProperty("expandCollapse")]
        [DefaultValue(null)]
        public String ExpandCollapse
        {
            get { return this.expandCollapse; }
            set { this.expandCollapse = value; }
        }
        [JsonProperty("resize")]
        [DefaultValue(null)]
        public String Resize
        {
            get { return this.resize; }
            set { this.resize = value; }
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
        public bool ShouldSerializePaneProperties()
        {
            if (PaneProperties.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
        
    }
}
