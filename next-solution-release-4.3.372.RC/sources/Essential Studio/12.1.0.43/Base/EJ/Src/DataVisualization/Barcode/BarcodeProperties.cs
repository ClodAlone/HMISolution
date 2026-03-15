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
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class BarcodeProperties
    {
        # region fields
        private bool displayText = true;
        private string text = null;
        private BarcodeSymbolType symbologyType = BarcodeSymbolType.QRBarcode;
        private string textColor = "black";
        private string lightBarColor = "white";
        private string darkBarColor = "black";
        private object quietZone = null;
        private int narrowBarWidth = 1;
        private int wideBarWidth = 3;
        private int barHeight = 150;
        private int barcodeToTextGapHeight = 10;
        private int xDimension = 4;
        private bool encodeStartStopSymbol = true;
        private bool enabled = true;

        //Events 
        private String load = null;
        # endregion

        //public BarcodeProperties()
        //{
        //}

        # region Properties
        [JsonProperty("displayText")]
        [DefaultValue(true)]
        public bool DisplayText
        {
            get { return this.displayText; }
            set { this.displayText = value; }
        }
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("symbologyType")]        
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(BarcodeSymbolType.QRBarcode)]
        public BarcodeSymbolType SymbologyType
        {
            get { return this.symbologyType; }
            set { this.symbologyType = value; }
        }
        [JsonProperty("textColor")]
        [DefaultValue("black")]
        public String TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        [JsonProperty("lightBarColor")]
        [DefaultValue("white")]
        public String LightBarColor
        {
            get { return this.lightBarColor; }
            set { this.lightBarColor = value; }
        }
        [JsonProperty("darkBarColor")]
        [DefaultValue("black")]
        public String DarkBarColor
        {
            get { return this.darkBarColor; }
            set { this.darkBarColor = value; }
        }
        [JsonProperty("quietZone")]
        //[DefaultValue(null)]
        public object QuietZone
        {
            get { return this.quietZone; }
            set { this.quietZone = value; }
        }
        [JsonProperty("narrowBarWidth")]
        [DefaultValue(1)]
        public int NarrowBarWidth
        {
            get { return this.narrowBarWidth; }
            set { this.narrowBarWidth = value; }
        }
        [JsonProperty("wideBarWidth")]
        [DefaultValue(3)]
        public int WideBarWidth
        {
            get { return this.wideBarWidth; }
            set { this.wideBarWidth = value; }
        }
        [JsonProperty("barHeight")]
        [DefaultValue(150)]
        public int BarHeight
        {
            get { return this.barHeight; }
            set { this.barHeight = value; }
        }
        [JsonProperty("barcodeToTextGapHeight")]
        [DefaultValue(10)]
        public int BarcodeToTextGapHeight
        {
            get { return this.barcodeToTextGapHeight; }
            set { this.barcodeToTextGapHeight = value; }
        }
        [JsonProperty("xDimension")]
        [DefaultValue(4)]
        public int XDimension 
        {
            get { return this.xDimension; }
            set { this.xDimension = value; }
        }
        [JsonProperty("encodeStartStopSymbol")]
        [DefaultValue(true)]
        public bool EncodeStartStopSymbol
        {
            get { return this.encodeStartStopSymbol; }
            set { this.encodeStartStopSymbol = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        //Events 
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
        }
        # endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializeQuietZone()
        {
            if (Utils.PropertyCompare(QuietZone, new QuietZone()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
