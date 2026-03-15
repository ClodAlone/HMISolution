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
using Syncfusion.JavaScript.DataVisualization.Models;



namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class DigitalGaugeItems
    {
        #region ItemsFields
        //Boolean values
        private bool enableCustomFont = false;

        //int values
        private int segmentLength = 2;
        private int characterSpacing = 2;
        private int characterOpacity = 1;
        private int characterCount = 1;
        private int segmentWidth = 1;
        private int segmentSpacing = 1;
        private int shadowBlur = 0;
        private int shadowOffsetX = 1;
        private int shadowOffsetY = 1;

        //Double values
        private Double segmentOpacity = 0;

        //objects
        private Object location = new DigitalGaugeLocation();
        private Object font = new DigitalGaugeFont();

        //String values
        private String shadowColor = null;
        private String segmentColor = null;
        private String value = null;
        private String textColor = null;

        //enum values
        private TextAlign textAlign = TextAlign.Left;
        private CharacterType characterType = CharacterType.EightCrossEightDotMatrix;
        #endregion

        #region Properties
        //int
        [JsonProperty("segmentLength")]
        [DefaultValue(2)]
        public int SegmentLength
        {
            get { return this.segmentLength; }
            set { this.segmentLength = value; }
        }
        [JsonProperty("characterSpacing")]
        [DefaultValue(2)]
        public int CharacterSpacing
        {
            get { return this.characterSpacing; }
            set { this.characterSpacing = value; }
        }
        [JsonProperty("characterOpacity")]
        [DefaultValue(1)]
        public int CharacterOpacity
        {
            get { return this.characterOpacity; }
            set { this.characterOpacity = value; }
        }
        [JsonProperty("characterCount")]
        [DefaultValue(1)]
        public int CharacterCount
        {
            get { return this.characterCount; }
            set { this.characterCount = value; }
        }
        [JsonProperty("segmentWidth")]
        [DefaultValue(1)]
        public int SegmentWidth
        {
            get { return this.segmentWidth; }
            set { this.segmentWidth = value; }
        }
        [JsonProperty("segmentSpacing")]
        [DefaultValue(1)]
        public int SegmentSpacing
        {
            get { return this.segmentSpacing; }
            set { this.segmentSpacing = value; }
        }
        [JsonProperty("shadowBlur")]
        [DefaultValue(0)]
        public int ShadowBlur
        {
            get { return this.shadowBlur; }
            set { this.shadowBlur = value; }
        }
        [JsonProperty("shadowOffsetX")]
        [DefaultValue(1)]
        public int ShadowOffsetX
        {
            get { return this.shadowOffsetX; }
            set { this.shadowOffsetX = value; }
        }
        [JsonProperty("shadowOffsetY")]
        [DefaultValue(1)]
        public int ShadowOffsetY
        {
            get { return this.shadowOffsetY; }
            set { this.shadowOffsetY = value; }
        }
        //bool
        [JsonProperty("enableCustomFont")]
        [DefaultValue(false)]
        public bool EnableCustomFont
        {
            get { return this.enableCustomFont; }
            set { this.enableCustomFont = value; }
        }
        //double
        [JsonProperty("segmentOpacity")]
        [DefaultValue(0)]
        public Double SegmentOpacity
        {
            get { return this.segmentOpacity; }
            set { this.segmentOpacity = value; }
        }
        //string 
        [JsonProperty("shadowColor")]
        [DefaultValue(null)]
        public String ShadowColor
        {
            get { return this.shadowColor; }
            set { this.shadowColor = value; }
        }
        [JsonProperty("segmentColor")]
        [DefaultValue(null)]
        public String SegmentColor
        {
            get { return this.segmentColor; }
            set { this.segmentColor = value; }
        }

        [JsonProperty("textColor")]
        [DefaultValue(null)]
        public String TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        //Enum values
        [JsonProperty("characterType")]
        [DefaultValue(CharacterType.EightCrossEightDotMatrix)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CharacterType CharacterType
        {
            get { return this.characterType; }
            set { this.characterType = value; }
        }
        [JsonProperty("textAlign")]
        [DefaultValue(TextAlign.Left)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlign TextAlign
        {
            get { return this.textAlign; }
            set { this.textAlign = value; }
        }
        //Objects
        [JsonProperty("location")]
        public object Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        [JsonProperty("font")]
        public object Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeDigitalGaugeFont()
        {
            if (Utils.PropertyCompare(Font, new DigitalGaugeFont()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDigitalGaugeLocation()
        {
            if (Utils.PropertyCompare(Location, new DigitalGaugeLocation()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{ 
         public class DigitalGaugeItemsBuilder
    {
             DigitalGauge digiGauge;
             private List<DigitalGaugeItems> items = new List<DigitalGaugeItems>();
             DigitalGaugeItems item = new DigitalGaugeItems();
        public DigitalGaugeItemsBuilder(DigitalGauge items)
        {
            this.digiGauge = items;
            this.items = digiGauge.DigitalGaugeModel.DigitalGaugeItems;      
        }
        public DigitalGaugeItemsBuilder()
        {
        }
             //bool
        public DigitalGaugeItemsBuilder EnableCustomFont()
        {
            item.EnableCustomFont = true;
            return this;
        }
              public DigitalGaugeItemsBuilder EnableCustomFont(bool enableCustomFont)
        {
            item.EnableCustomFont = enableCustomFont;
            return this;
        }
             //int
        public DigitalGaugeItemsBuilder SegmentLenth(int segmentLength)
         {
            item.SegmentLength = segmentLength;
            return this;
        }
        public DigitalGaugeItemsBuilder CharacterSpacing(int characterSpacing)
        {
            item.CharacterSpacing = characterSpacing;
            return this;
        }
        public DigitalGaugeItemsBuilder CharacterOpacity(int characterOpacity)
        {
            item.CharacterOpacity = characterOpacity;
            return this;
        }
        public DigitalGaugeItemsBuilder CharacterCount(int characterCount)
        {
            item.CharacterCount = characterCount;
            return this;
        }
        public DigitalGaugeItemsBuilder SegmentWidth(int segmentWidth)
        {
            item.SegmentWidth = segmentWidth;
            return this;
        }

        public DigitalGaugeItemsBuilder SegmentSpacing(int segmentSpacing)
        {
            item.SegmentSpacing = segmentSpacing;
            return this;
        }
        public DigitalGaugeItemsBuilder ShadowBlur(int shadowBlur)
        {
            item.ShadowBlur = shadowBlur;
            return this;
        }
        public DigitalGaugeItemsBuilder ShadowOffsetX(int shadowOffsetX)
        {
            item.ShadowOffsetX = shadowOffsetX;
            return this;
        }
        public DigitalGaugeItemsBuilder ShadowOffsetY(int shadowOffsety)
        {
            item.ShadowOffsetY = shadowOffsety;
            return this;
        }
        public DigitalGaugeItemsBuilder SegmentOpacity(Double segmentOpacity)
        {
            item.SegmentOpacity = segmentOpacity;
            return this;
        }

        public DigitalGaugeItemsBuilder ShadowColor(String shadowColor)
        {
            item.ShadowColor = shadowColor;
            return this;
        }
        public DigitalGaugeItemsBuilder SegmentColor(String segmentColor)
        {
            item.SegmentColor = segmentColor;
            return this;
        }
        public DigitalGaugeItemsBuilder TextColor(String textColor)
        {
            item.TextColor = textColor;
            return this;
        }

        public DigitalGaugeItemsBuilder Value(String value)
        {
            item.Value = value;
            return this;
        }
             //enum
        public DigitalGaugeItemsBuilder CharacterType(CharacterType characterType)
        {
            item.CharacterType = characterType;
            return this;
        }
        public DigitalGaugeItemsBuilder TextAlign(TextAlign textAlign)
        {
           item.TextAlign = textAlign;
            return this;
        }
             //objects
        public DigitalGaugeItemsBuilder Font(Action<DigitalGaugeFontBuilder> font)
        {
            var fon = new DigitalGaugeFont();
            this.item.Font = fon;
            var builder = new DigitalGaugeFontBuilder(fon);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public DigitalGaugeItemsBuilder Location(Action<DigitalGaugeLocationBuilder> location)
        {
            var loc = new DigitalGaugeLocation();
            this.item.Location = loc;
            var builder = new DigitalGaugeLocationBuilder(loc);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        public void Add()
        {
            digiGauge.DigitalGaugeModel.DigitalGaugeItems.Add(item);
            item = new DigitalGaugeItems();
               }


    }
}
