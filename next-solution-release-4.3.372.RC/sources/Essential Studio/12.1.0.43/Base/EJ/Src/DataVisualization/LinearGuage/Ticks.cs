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
    public class LinearTicks
    {
        #region Fields

        //String Values
        private String tickColor = null;
        //Interger Values
        private int xDistanceFromScale = 0;
        private int yDistanceFromScale = 0;
        private int angle = 0;
        private int tickHeight = 10;
        private int tickWidth = 3;
        //Double Values
        private double opacity = 0;
        //Enumeration Values
        private Tickstyles tickstyle = Tickstyles.MajorInterval;
        private TickPlacements tickPlacement = TickPlacements.Near;
        #endregion

        #region Properties

        //String Values
        [JsonProperty("tickColor")]
        [DefaultValue(null)]
        public String TickColor
        {
            get { return this.tickColor; }
            set { this.tickColor = value; }
        }
        //Integer values
        [JsonProperty("xDistanceFromScale")]
        [DefaultValue(0)]
        public int XDistanceFromScale
        {
            get { return this.xDistanceFromScale; }
            set { this.xDistanceFromScale = value; }
        }
        [JsonProperty("yDistanceFromScale")]
        [DefaultValue(0)]
        public int YDistanceFromScale
        {
            get { return this.yDistanceFromScale; }
            set { this.yDistanceFromScale = value; }
        }
        [JsonProperty("angle")]
        [DefaultValue(0)]
        public int Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }
        [JsonProperty("tickHeight")]
        [DefaultValue(10)]
        public int TickHeight
        {
            get { return this.tickHeight; }
            set { this.tickHeight = value; }
        }
        [JsonProperty("tickWidth")]
        [DefaultValue(3)]
        public int TickWidth
        {
            get { return this.tickWidth; }
            set { this.tickWidth = value; }
        }
        //Enumeration Values
        [JsonProperty("tickstyle")]
        [DefaultValue(Tickstyles.MajorInterval)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Tickstyles Tickstyle
        {
            get { return this.tickstyle; }
            set { this.tickstyle = value; }
        }
        [JsonProperty("tickPlacement")]
        [DefaultValue(TickPlacements.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TickPlacements TickPlacement
        {
            get { return this.tickPlacement; }
            set { this.tickPlacement = value; }
        }
        //Double Values
        [JsonProperty("opacity")]
        [DefaultValue(0)]
        public double Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class LinearTicksBuilder
    {
        private List<LinearTicks> tick = new List<LinearTicks>();
        LinearTicks ticks = new LinearTicks();
        Scales scales;
        public LinearTicksBuilder(Scales ticks)
        {
            this.scales = ticks;
            this.tick = ticks.Ticks;
        }
        //String Values
        public LinearTicksBuilder TickColor(String tickColor)
        {
            ticks.TickColor = tickColor;
            return this;
        }
        //Integers
        public LinearTicksBuilder XDistanceFromScale(int xDistanceFromScale)
        {
            ticks.XDistanceFromScale = xDistanceFromScale;
            return this;
        }
        public LinearTicksBuilder YDistanceFromScale(int yDistanceFromScale)
        {
            ticks.YDistanceFromScale = yDistanceFromScale;
            return this;
        }
        public LinearTicksBuilder Angle(int angle)
        {
            ticks.Angle = angle;
            return this;
        }
        public LinearTicksBuilder TickHeight(int tickHeight)
        {
            ticks.TickHeight = tickHeight;
            return this;
        }
        public LinearTicksBuilder TickWidth(int tickWidth)
        {
            this.ticks.TickWidth = tickWidth;
            return this;
        }
        //Double Values
        public LinearTicksBuilder Opacity(double opacity)
        {
            ticks.Opacity = opacity;
            return this;
        }
        //EnumValues
        public LinearTicksBuilder Tickstyle(Tickstyles tickstyle)
        {
            ticks.Tickstyle = tickstyle;
            return this;
        }
        public LinearTicksBuilder TickPlacement(TickPlacements tickPlacement)
        {
            ticks.TickPlacement = tickPlacement;
            return this;
        }
        public void Add()
        {
            scales.Ticks.Add(ticks);
            ticks = new LinearTicks();
        }
    }
}
