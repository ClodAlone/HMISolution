#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class SymbolPaletteProperties
    {
        private String height;
        private String width;
        private Collection palettes;
        private int paletteItemWidth;
        private int paletteItemHeight;
        private bool showPaletteItemText;
        private string diagramId;
        private bool allowDrag;
        private int headerHeight;
        private int selectedPaletteIndex;

        public SymbolPaletteProperties()
        {
            this.height = "250px";
            this.width = "400px";
            this.paletteItemHeight = 40;
            this.paletteItemWidth = 40;
            this.diagramId = "";
            this.headerHeight = 30;
            this.showPaletteItemText = true;
            this.allowDrag = true;
            this.palettes = new Collection();
            this.selectedPaletteIndex = 0;
        }


        [JsonProperty("height")]
        [DefaultValue("400px")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        [JsonProperty("width")]
        [DefaultValue("250px")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        [JsonProperty("palettes")]
        public Collection Palettes
        {
            get { return this.palettes; }
            set { this.palettes = value; }
        }

        [JsonProperty("paletteItemWidth")]
        [DefaultValue(50)]
        public int PaletteItemWidth
        {
            get { return this.paletteItemWidth; }
            set { this.paletteItemWidth = value; }
        }


        [JsonProperty("paletteItemHeight")]
        [DefaultValue(50)]
        public int PaletteItemHeight
        {
            get { return this.paletteItemHeight; }
            set { this.paletteItemHeight = value; }
        }

        [JsonProperty("showPaletteItemText")]
        [DefaultValue(true)]
        public bool ShowPaletteItemText
        {
            get { return this.showPaletteItemText; }
            set { this.showPaletteItemText = value; }
        }

        [JsonProperty("allowDrag")]
        [DefaultValue(true)]
        public bool AllowDrag
        {
            get { return this.allowDrag; }
            set { this.allowDrag = value; }
        }

        [JsonProperty("diagramId")]
        [DefaultValue("")]
        public string DiagramId
        {
            get { return this.diagramId; }
            set { this.diagramId = value; }
        }

        [JsonProperty("headerHeight")]
        [DefaultValue(50)]
        public int HeaderHeight
        {
            get { return this.headerHeight; }
            set { this.headerHeight = value; }
        }

        [JsonProperty("selectedPaletteIndex")]
        [DefaultValue(0)]
        public int SelectedPaletteIndex
        {
            get { return this.selectedPaletteIndex; }
            set { this.selectedPaletteIndex = value; }
        }
    }
}
