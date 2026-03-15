#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Specifies frame types used for drawing circular gauge control.
    /// </summary>
    /// <remarks>
    /// <para>Gauge can be customized into FullCircle FrameType</para>
    /// </remarks>
    public enum GaugeFrameType
    {
        /// <summary>
        /// Circular gauge with default template will be displayed as full circle.
        /// </summary>
        /// <remarks>
        /// <para>Gauge can be customized into FullCircle FrameType.</para>
        /// </remarks>
        Circular = 0,

        /// <summary>
        /// Circular gauge with default template will be displayed as half circle.
        /// </summary>
        SemiCircular,

        /// <summary>
        /// Circular gauge with default template will be displayed as Quarter circle.
        /// </summary>
        QuarterCircular
    }

    /// <summary>
    /// Specifies frame direction of Quarter Circular gauge.
    /// </summary>
    public enum FrameDirection
    {
        /// <summary>
        /// Quarter Circular gauge in North direction.
        /// </summary>
        North = 0,

        /// <summary>
        /// Quarter Circular gauge in South direction.
        /// </summary>
        South,

        /// <summary>
        /// Quarter Circular gauge in NorthEast direction.
        /// </summary>
        NorthEast,

        /// <summary>
        /// Quarter Circular gauge in NorthWest direction.
        /// </summary>
        NorthWest,

        /// <summary>
        /// Quarter Circular gauge in SouthEast direction.
        /// </summary>
        SouthEast,

        /// <summary>
        /// Quarter Circular gauge in SouthWest direction.
        /// </summary>
        SouthWest,

        /// <summary>
        /// Quarter Circular gauge in East direction.
        /// </summary>
        East,

        /// <summary>
        /// Quarter Circular gauge in West direction.
        /// </summary>
        West
    }

    /// <summary>
    /// Specifies different pointer cap types.
    /// </summary>    
    public enum PointerCapType
    {
        /// <summary>
        /// Default cap type.
        /// </summary>
        Default = 0
    }

    /// <summary>
    /// Specifies different Scale types.
    /// </summary>    
    public enum ScaleTypes
    {
        /// <summary>
        /// Default type.
        /// </summary>
        Linear = 0,
        /// <summary>
        /// Thermometer type.
        /// </summary>
        Thermometer

    }

    /// <summary>
    /// Specifies placement of visual element relatively to the scale.
    /// </summary>    
    public enum ScalePlacement
    {
        /// <summary>
        /// Element is placed inside the scale.
        /// </summary>
        Inside = 0,

        /// <summary>
        /// Element is placed outside the scale.
        /// </summary>
        Outside,

        /// <summary>
        /// Element is placed over the scale.
        /// </summary>
        Cross
    }

    /// <summary>
    /// Specifies pointer types.
    /// </summary>    
    public enum PointerNeedleType
    {
        /// <summary>
        /// Pointer displays as a needle.
        /// </summary>
        Needle = 0,

        /// <summary>
        /// Pointer displays as a marker.
        /// </summary>
        Marker,

        /// <summary>
        /// Pointer is displays as a bar.
        /// </summary>
        Bar
    }

    /// <summary>
    /// Specifies tick types.
    /// </summary>    
    public enum TickStyle
    {
        /// <summary>
        /// Tick with major interval.
        /// </summary>
        MajorTick = 0,

        /// <summary>
        /// Tick with middle interval.
        /// </summary>
        MidTick,

        /// <summary>
        /// Tick with minor interval.
        /// </summary>
        MinorTick
    }

    /// <summary>
    /// Specifies different tick look.
    /// </summary>    
    public enum TickShape
    {
        /// <summary>
        /// Tick displays in rectangular form.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Tick displays in elliptical form.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Tick is displayed in triangular form.
        /// </summary>
        Triangle
    }

    /// <summary>
    /// Specifies different indicator styles.
    /// </summary>    
    public enum IndicatorStyle
    {
        /// <summary>
        /// Indicator displays in rectangular form.
        /// </summary>
        RectangularLED = 0,

        /// <summary>
        /// Indicator displays in elliptical form.
        /// </summary>
        CircularLED,

        /// <summary>
        /// Indicator displays in text form.
        /// </summary>
        Text
    }

    /// <summary>
    /// Specifies resize modes of images.
    /// </summary>    
    public enum GaugeImageResizeMode
    {
        /// <summary>
        /// Image keeps actual size.
        /// </summary>
        None = 0,

        /// <summary>
        /// Image resizes to specified width and height.
        /// </summary>
        Stretch
    }

    /// <summary>
    /// Specifies different marker styles.
    /// </summary>    
    public enum MarkerStyle
    {
        /// <summary>
        /// Marker displays in rectangular form.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Marker displays in triangular form.
        /// </summary>
        Triangle,

        /// <summary>
        /// Marker displays in elliptical form.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Marker displays in diamond form.
        /// </summary>
        Diamond,

        /// <summary>
        /// Marker displays in trapezoidal form.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Marker displays in pentagonal form.
        /// </summary>
        Pentagon
    }

    /// <summary>
    /// Specifies different needle styles.
    /// </summary>    
    public enum NeedleStyle
    {
        /// <summary>
        /// Needle displays in triangular form.
        /// </summary>
        Triangle = 0,

        /// <summary>
        /// Needle displays in rectangular form.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Needle displays in trapezoidal form.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Needle displays in arrow form.
        /// </summary>
        Arrow,

        /// <summary>
        /// Needle displays in needle form.
        /// </summary>
        Needle
    }

    /// <summary>
    /// Specifies the orientation of the Gauge.
    /// </summary>
    public enum GaugeOrientation
    {
        /// <summary>
        /// Horizontal gauge orientation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical gauge orientation.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Specifies the types of CircularKnob.
    /// </summary>
    public enum KnobType
    {
        /// <summary>
        /// CircularKnob indicator is a point
        /// </summary>
        Point,

        /// <summary>
        /// CircularKnob indicator is a line.
        /// </summary>
        Line,

        /// <summary>
        /// CircularKnob indicator similar to Cog.
        /// </summary>
        Cog
    }

    /// <summary>
    /// Specifies whether character should contain seven or fourteen segments.
    /// </summary>
    public enum CharacterType
    {
        /// <summary>
        /// Character is displayed with seven segments. Used for displaying numeric characters.
        /// </summary>
        SegmentSeven,

        /// <summary>
        /// Character is displayed with fourteen segments. Used for displaying alpha-numeric characters.
        /// </summary>
        SegmentFourteen
    }

    /// <summary>
    /// Specifies the UnitPosition of Rolling Gauge
    /// </summary>
    public enum UnitPosition
    {
        /// <summary>
        /// Unit position is start
        /// </summary>
        Start = 0,
        /// <summary>
        /// Unit position is End
        /// </summary>
        End
    }

    /// <summary>
    /// Specifies Rolling Direction of Rolling Gauge.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Direction is AntiClockwise
        /// </summary>
        AnitClockwise = 0,
        /// <summary>
        /// Direction is clock wise
        /// </summary>
        Clockwise
    }
    /// <summary>
    /// Specifies GaugeVisualStyle of Rolling Gauge.
    /// </summary>
    public enum GaugeVisualStyle
    {
        /// <summary>
        /// Gauge visual style is set as Blend
        /// </summary>
        Blend=0,
        /// <summary>
        /// Gauge visual style is set as default
        /// </summary>
        Default=1,
        /// <summary>
        /// Gauge visual style is set as Office2003
        /// </summary>
        Office2003=2,
        /// <summary>
        /// Gauge visual style is set as Metro
        /// </summary>
        Metro=3,
        /// <summary>
        /// Gauge visual style is set as Office2007Black
        /// </summary>
        Office2007Black=4,
        /// <summary>
        /// Gauge visual style is set as Office2007Blue
        /// </summary>
        Office2007Blue=5,
        /// <summary>
        /// Gauge visual style is set as Office2007Silver
        /// </summary>
        Office2007Silver=6,
        /// <summary>
        /// Gauge visual style is set as VS2010
        /// </summary>
        VS2010=7
    }
}
