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
using Syncfusion.JavaScript.DataVisualization.Models.Controls;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class DiagramProperties
    {
        private String height;
        private String width;
        private String borderColor;
        private int borderWidth;
        private Collection nodes;
        private Collection connectors;
        private Boolean enableContextMenu;
        private SnapSettings snapSettings;
        private PageSettings pageSettings;
        private Layout layout;
        private Boolean autoSize;
        private Boolean autoScroll;
        private Boolean enableVisualGuide;


        public DiagramProperties()
        {
            this.borderColor = "#DADADA";
            this.borderWidth = 1;
            this.nodes = new Collection();
            this.connectors = new Collection();
            this.snapSettings = new SnapSettings();
            this.pageSettings = new PageSettings();
            this.enableContextMenu = true;
            this.autoSize = false;
            this.autoScroll = true;
            this.enableVisualGuide = true;
            this.layout = new Layout();
        }
        [JsonProperty("height")]
        [DefaultValue("500px")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue("600px")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        [JsonProperty("borderColor")]
        [DefaultValue("#DADADA")]
        public String BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        [JsonProperty("borderWidth")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }

        [JsonProperty("enableContextMenu")]
        [DefaultValue(true)]
        public Boolean EnableContextMenu
        {
            get { return this.enableContextMenu; }
            set { this.enableContextMenu = value; }
        }

        [JsonProperty("autoScroll")]
        [DefaultValue(true)]
        public Boolean AutoScroll
        {
            get { return this.autoScroll; }
            set { this.autoScroll = value; }
        }

        [JsonProperty("autoSize")]
        [DefaultValue(false)]
        public Boolean AutoSize
        {
            get { return this.autoSize; }
            set { this.autoSize = value; }
        }

        [JsonProperty("enableVisualGuide")]
        [DefaultValue(true)]
        public Boolean EnableVisualGuide
        {
            get { return this.enableVisualGuide; }
            set { this.enableVisualGuide = value; }
        }

        [JsonProperty("nodes")]
        public Collection Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        [JsonProperty("connectors")]
        public Collection Connectors
        {
            get { return this.connectors; }
            set { this.connectors = value; }
        }

        [JsonProperty("pageSettings")]
        public PageSettings PageSettings
        {
            get { return this.pageSettings; }
            set { this.pageSettings = value; }
        }

        [JsonProperty("snapSettings")]
        public SnapSettings SnapSettings
        {
            get { return this.snapSettings; }
            set { this.snapSettings = value; }
        }

        [JsonProperty("layout")]
        public Layout Layout
        {
            get { return this.layout; }
            set { this.layout = value; }
        }
    }
}
