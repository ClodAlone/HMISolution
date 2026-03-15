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
   public class RatingProperties
    {
       #region Fields
       //Int Values
        private int  minValue=0;
        private int maxValue=5;
        private int currentValue=1;
        private int shapeWidth=23;
        private int shapeHeight=23;
        private int incrementStep=1;

        //Boolean Values
        private bool allowReset = true;
        private bool readOnly = false;
        private bool enabled = true;
        private bool showTooltip = true;
        private bool persist = false;

        //Enumeration Values
        private Orientation orientation = Orientation.Horizontal;
        private Precisions precision =Precisions.Full;

        //String Values
        private String height = "";
        private String width = "";
        private String cssClass = "";
        
        //Events 
        private String create = null;
        private String click = null;
        private String mouseOver = null;
        private String mouseOut = null;
        private String valueChanged = null;
        private String destroy = null;

        //Button 
        private Rating rating = new Rating();

        #endregion

        public RatingProperties() { }
        //public RatingProperties(String id, Rating rating)
        //{
        //    rating.ID = id;
        //    this.rating = rating;
        //}

        #region Properties
       //int properties
        [JsonProperty("minValue")]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }
        [JsonProperty("maxValue")]
        [DefaultValue(5)]
        public int MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }
        [JsonProperty("currentValue")]
        [DefaultValue(1)]
        public int CurrentValue
        {
            get { return this.currentValue; }
            set { this.currentValue = value; }
        }
        [JsonProperty("shapeWidth")]
        [DefaultValue(23)]
        public int ShapeWidth
        {
            get { return this.shapeWidth; }
            set { this.shapeWidth = value; }
        }
        [JsonProperty("shapeHeight")]
        [DefaultValue(23)]
        public int ShapeHeight
        {
            get { return this.shapeHeight; }
            set { this.shapeHeight = value; }
        }
        [JsonProperty("incrementStep")]
        [DefaultValue(1)]
        public int IncrementStep
        {
            get { return this.incrementStep; }
            set { this.incrementStep = value; }
        }

       //Boolean values
        [JsonProperty("allowReset")]
        [DefaultValue(true)]
        public bool AllowReset
        {
            get { return this.allowReset; }
            set { this.allowReset = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("showTooltip")]
        [DefaultValue(true)]
        public bool ShowTooltip
        {
            get { return this.showTooltip; }
            set { this.showTooltip = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
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
        [JsonProperty("precision")]
        [DefaultValue(Precisions.Full)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Precisions Precision
        {
            get { return this.precision; }
            set { this.precision = value; }
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
     
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("click")]
        [DefaultValue(null)]
        public String Click
        {
            get { return this.click; }
            set { this.click = value; }
        }
        [JsonProperty("mouseOver")]
        [DefaultValue(null)]
        public String MouseOver
        {
            get { return this.mouseOver; }
            set { this.mouseOver = value; }
        }
        [JsonProperty("mouseOut")]
        [DefaultValue(null)]
        public String MouseOut
        {
            get { return this.mouseOut; }
            set { this.mouseOut = value; }
        }
        [JsonProperty("valueChanged")]
        [DefaultValue(null)]
        public String ValueChanged
        {
            get { return this.valueChanged; }
            set { this.valueChanged = value; }
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

        #endregion

    }
}
