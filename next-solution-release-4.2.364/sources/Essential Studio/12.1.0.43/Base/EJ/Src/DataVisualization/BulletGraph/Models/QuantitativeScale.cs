#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class QuantitativeScale
    {
        #region Fields
     //   private Point location = new Point(110, 10);
        
        private int minimum = 0;
        private int maximum = 100;
        private int interval = 2;
        private int minorTicksPerInterval = 3;
        private int majorTickSize = 15;
        private int minorTickSize = 7;
        private String majorTickStroke = String.Empty;
        private String minorTickStroke = String.Empty;
        private double majorTickStrokeThickness = 1;
        private double minorTickStrokeThickness = 0.5;
        private TickPosition tickPosition = TickPosition.Below;
        private int featuredMeasureBarStrokeThickness = 12;
        private int comparativeMeasureSymbolStrokeThickness = 3;
        private String featuredMeasureBarStroke = String.Empty;
        private String comparativeMeasureSymbolStroke = String.Empty;

        private object labels = new BulletLabels();
        private object location = new BulletLocation();
        private List<FeatureMeasure> featureMeasure = new List<FeatureMeasure>();
        #endregion

        #region Properties

        [JsonProperty("labels")]
       // [DefaultValue(null)]
        public object Labels
        {
            get { return this.labels; }
            set { this.labels = value; }
        }

        [JsonProperty("featureMeasure")]
        public List<FeatureMeasure> FeatureMeasure
        {
            get { return this.featureMeasure; }
            set { this.featureMeasure = value; }
        }

        [JsonProperty("location")]
    //    [DefaultValue(new Point(110,10))]
        public object Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [JsonProperty("minimum")]
        [DefaultValue(0)]
        public int Minimum
        {
            get { return this.minimum; }
            set { this.minimum = value; }
        }

        [JsonProperty("maximum")]
        [DefaultValue(10)]
        public int Maximum
        {
            get { return this.maximum; }
            set { this.maximum = value; }
        }

        [JsonProperty("interval")]
        [DefaultValue(2)]
        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }

        [JsonProperty("minorTicksPerInterval")]
        [DefaultValue(3)]
        public int MinorTicksPerInterval
        {
            get { return this.minorTicksPerInterval; }
            set { this.minorTicksPerInterval = value; }
        }

        [JsonProperty("majorTickSize")]
        [DefaultValue(15)]
        public int MajorTickSize
        {
            get { return this.majorTickSize; }
            set { this.majorTickSize = value; }
        }

        [JsonProperty("minorTickSize")]
        [DefaultValue(7)]
        public int MinorTickSize
        {
            get { return this.minorTickSize; }
            set { this.minorTickSize = value; }
        }

        [JsonProperty("majorTickStroke")]
        //[DefaultValue(null)]
        public String MajorTickStroke
        {
            get { return this.majorTickStroke; }
            set { this.majorTickStroke = value; }
        }

        [JsonProperty("minorTickStroke")]
        //[DefaultValue(null)]
        public String MinorTickStroke
        {
            get { return this.minorTickStroke; }
            set { this.minorTickStroke = value; }
        }

        [JsonProperty("majorTickStrokeThickness")]
        [DefaultValue(1)]
        public double MajorTickStrokeThickness
        {
            get { return this.majorTickStrokeThickness; }
            set { this.majorTickStrokeThickness = value; }
        }

        [JsonProperty("minorTickStrokeThickness")]
        [DefaultValue(0.5)]
        public double MinorTickStrokeThickness
        {
            get { return this.minorTickStrokeThickness; }
            set { this.minorTickStrokeThickness = value; }
        }

        [JsonProperty("tickPosition")]
        [DefaultValue(TickPosition.Below)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TickPosition TickPosition
        {
            get { return this.tickPosition; }
            set { this.tickPosition = value; }
        }

        [JsonProperty("featuredMeasureBarStrokeThickness")]
        [DefaultValue(12)]
        public int FeaturedMeasureBarStrokeThickness
        {
            get { return this.featuredMeasureBarStrokeThickness; }
            set { this.featuredMeasureBarStrokeThickness = value; }
        }

        [JsonProperty("comparativeMeasureSymbolStrokeThickness")]
        [DefaultValue(3)]
        public int ComparativeMeasureSymbolStrokeThickness
        {
            get { return this.comparativeMeasureSymbolStrokeThickness; }
            set { this.comparativeMeasureSymbolStrokeThickness = value; }
        }

        [JsonProperty("featuredMeasureBarStroke")]
        //[DefaultValue(null)]
        public String FeaturedMeasureBarStroke
        {
            get { return this.featuredMeasureBarStroke; }
            set { this.featuredMeasureBarStroke = value; }
        }

        [JsonProperty("comparativeMeasureSymbolStroke")]
        //[DefaultValue(null)]
        public String ComparativeMeasureSymbolStroke
        {
            get { return this.comparativeMeasureSymbolStroke; }
            set { this.comparativeMeasureSymbolStroke = value; }
        }

        #endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new BulletLocation()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeLabels()
        {
            if (Utils.PropertyCompare(Labels, new BulletLabels()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFeatureMeasure()
        {
            if (FeatureMeasure.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }


    public class BulletLocation
    {
        #region Fields
        private int x; 
        private int y; 
        #endregion

        #region Properties
        [JsonProperty("x")]
        [DefaultValue(null)]
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        [JsonProperty("y")]
        [DefaultValue(null)]
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        #endregion
    }

    public class BulletLocationBuilder
    {
        private BulletLocation location;
        private QuantitativeScale qs_model;
        private Caption cap_model;
        private SubTitle sub_model;

        public BulletLocationBuilder(BulletLocation options)
        {
            this.location = options;
        }

        public BulletLocationBuilder(BulletLocation options, QuantitativeScale scale)
        {
            this.location = options;
            this.qs_model = scale;
            this.qs_model.Location = options;
            
        }

        public BulletLocationBuilder(BulletLocation options, Caption cap_Options)
        {
            this.location = options;
            this.cap_model = cap_Options;
            this.cap_model.Location = options;

        }

        public BulletLocationBuilder(BulletLocation options, SubTitle sub_Options)
        {
            this.location = options;
            this.sub_model = sub_Options;
            this.sub_model.Location = options;

        }
        public BulletLocationBuilder x(int x)
        {
            this.location.X = x;
            return this;
        }
        public BulletLocationBuilder y(int y)
        {
            this.location.Y = y;
            return this;
        }

    }
    

    

    

    
}
