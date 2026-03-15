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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class CircularTicks
    {
        #region Fields

        //String Values
        private string tickColor = null;

        //Interger Values
        private int distanceFromScale = 0;
        private int angle = 0;
        private int tickHeight = 16;
        private int tickWidth = 3;

        //Enumeration Values
        private CircularTickstyles tickstyle = CircularTickstyles.Major;
        private TickPositions tickPosition = TickPositions.Near;

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
        [JsonProperty("distanceFromScale")]
        [DefaultValue(0)]
        public int DistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("angle")]
        [DefaultValue(0)]
        public int Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }
        [JsonProperty("tickHeight")]
        [DefaultValue(16)]
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
        [JsonProperty("tickStyle")]
        [DefaultValue(CircularTickstyles.Major)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CircularTickstyles Tickstyle
        {
            get { return this.tickstyle; }
            set { this.tickstyle = value; }
        }
        [JsonProperty("tickPosition")]
        [DefaultValue(TickPositions.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TickPositions TickPosition
        {
            get { return this.tickPosition; }
            set { this.tickPosition = value; }
        }
        #endregion


    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
        public class CircularTicksBuilder
        {        
            
            CircularScales scales;
            private List<CircularTicks> ticks = new List<CircularTicks>();
            CircularTicks tick = new CircularTicks();
            public CircularTicksBuilder(CircularScales tick)
            {                
                this.scales = tick;
                this.ticks = tick.Ticks;
            }
            //String Values
            public CircularTicksBuilder TickColor(String tickColor)
            {
                tick.TickColor = tickColor;
                return this;
            }
            //Integers
            public CircularTicksBuilder DistanceFromScale(int DistanceFromScale)
            {
                tick.DistanceFromScale = DistanceFromScale;
                return this;
            }
            public CircularTicksBuilder Angle(int angle)
            {
                tick.Angle = angle;
                return this;
            }
            public CircularTicksBuilder TickHeight(int tickHeight)
            {
                tick.TickHeight = tickHeight;
                return this;
            }
            public CircularTicksBuilder TickWidth(int tickWidth)
            {
                tick.TickWidth = tickWidth;
                return this;
            }
            //EnumValues
            public CircularTicksBuilder Tickstyle(CircularTickstyles tickstyle)
            {
                tick.Tickstyle = tickstyle;
                return this;
            }
            public CircularTicksBuilder TickPosition(TickPositions tickPosition)
            {
                tick.TickPosition = tickPosition;
                return this;
            }
            public void Add()
            {
                scales.Ticks.Add(tick);
                tick = new CircularTicks();                
            }
    }
}