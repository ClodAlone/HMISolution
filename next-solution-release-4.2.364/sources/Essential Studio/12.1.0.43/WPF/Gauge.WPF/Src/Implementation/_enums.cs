// <copyright file="_enums.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Specifies the frame types used for drawing Circular Gauge control.
    /// </summary>  
    public enum GaugeFrameType
    {
        /// <summary>
        /// Circular gauge with default template will be displayed as a full circle.
        /// </summary>
        FullCircle = 0,

        /// <summary>
        /// Circular gauge with default template will be displayed as a half circle.
        /// </summary>
        HalfCircle = 1,

        /// <summary>
        /// Circular gauge with dark first and second frames.
        /// </summary>
        CircularWithDarkOuterFrames = 2,

        /// <summary>
        /// Circular gauge frame with inner top gradient.
        /// </summary>
        CircularWithInnerTopGradient = 3,

        /// <summary>
        /// Circular gauge frame with inner left gradient.
        /// </summary>
        CircularWithInnerLeftGradient = 4,

        /// <summary>
        /// Circular gauge with center gradient.
        /// </summary>
        CircularCenterGradient = 5,
        
        /*
        /// <summary>
        /// Curved circular gauge 
        /// </summary>
        CurvedHalfCircle=6,
        */

        /// <summary>
        /// Counter clockwise half circular gauge 
        /// </summary>
        CounterclockwiseHalfCircle = 6,

        /// <summary>
        /// Left side half circle
        /// </summary>
        LeftHalfCircle = 7,

        /// <summary>
        /// Right side Half circle
        /// </summary>
        RightHalfCircle = 8
    }

    /// <summary>
    /// Specifies the frame types used for drawing Linear Gauge control.
    /// </summary>  
    public enum LinearGaugeFrameType
    {
        /// <summary>
        /// Linear gauge with default template will be displayed as a Rectangle.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Linear gauge with Bolted rectangle frame type.
        /// </summary>
        BoltedRectangle = 1,

        /// <summary>
        /// Linear gauge with Cropped rectangle frame type.
        /// </summary>
        CroppedRectangle = 2,

        /// <summary>
        /// Linear gauge frame with inner rounded rectangle.
        /// </summary>
        RoundedRectangleWithInnerGradient = 3,
    }

    /// <summary>
    /// Specifies the frame types used for drawing digital gauge control.
    /// </summary>  
    public enum DigitalGaugeFrameType
    {
        /// <summary>
        /// Digital gauge with default template will be displayed as a Rectangle.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Digital gauge with Bolted rectangle frame style.
        /// </summary>
        BoltedRectangle = 1,

        /// <summary>
        /// Digital gauge with Cropped rectangle frame style.
        /// </summary>
        CroppedRectangle = 2,

        /// <summary>
        /// Digital gauge frame with inner gradient.
        /// </summary>
        RoundedRectangleWithInnerGradient = 3,
    }

    /// <summary>
    /// Specifies different pointer cap shapes.
    /// </summary>    
    public enum PointerCapType
    {
        /// <summary>
        /// Default cap type.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Custom cap type. Geometry can be applied through PointerCapCustomGeometry property.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies the placement of visual element relative to the scale.
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
    /// Specifies the direction of the scale.
    /// </summary>    
    public enum ScaleDirection
    {
        /// <summary>
        /// Element is placed inside the scale.
        /// </summary>
        Clockwise = 0,

        /// <summary>
        /// Element is placed outside the scale.
        /// </summary>
        CounterClockwise
    }

    /// <summary>
    /// Specifies Pointer types.
    /// </summary>    
    public enum PointerNeedleType
    {
        /// <summary>
        /// Pointer gets displayed as a needle.
        /// </summary>
        Needle = 0,

        /// <summary>
        /// Pointer gets displayed as a marker.
        /// </summary>
        Marker,

        /// <summary>
        /// Pointer gets displayed as a bar.
        /// </summary>
        Bar
    }

    /// <summary>
    /// Specifies Tick types.
    /// </summary>    
    public enum TickStyle
    {
        /// <summary>
        /// Ticks denote major interval.
        /// </summary>
        MajorTick = 0,

        /// <summary>
        /// Ticks denote minor interval.
        /// </summary>
        MinorTick
    }

    /// <summary>
    /// Specifies the shape of the Tick.
    /// </summary>    
    public enum TickShape
    {
        /// <summary>
        /// Tick gets displayed in rectangular form.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Tick gets displayed in rectangular form with rounded corners.
        /// </summary>
        RoundedRectangle,

        /// <summary>
        /// Tick gets displayed in elliptical form.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Tick gets displayed in triangular form.
        /// </summary>
        Triangle
    }

    /// <summary>
    /// Specifies the shape of the Indicator.
    /// </summary>    
    public enum IndicatorStyle
    {
        /// <summary>
        /// Rectangle shaped Indicator.
        /// </summary>
        RectangularLED = 0,

        /// <summary>
        /// Circle shaped Indicator.
        /// </summary>
        CircularLED,

        /// <summary>
        /// RoundedRectangle shaped Indicator.
        /// </summary>
        RoundedRectangularLED,

        /// <summary>
        /// Indicator is displayed as a text
        /// </summary>
        /// <seealso cref="StateIndicator.Text"/>
        /// <seealso cref="StateIndicator.ActiveText"/>
        Text,

        /// <summary>
        /// Custom Geometry can be applied through IndicatorCustomGeometry property.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies the resize modes of images.
    /// </summary>    
    public enum GaugeImageResizeMode
    {
        /// <summary>
        /// Image size is unaltered.
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
        /// Marker gets displayed in rectangular form.
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// Marker gets displayed in triangular form.
        /// </summary>
        Triangle,

        /// <summary>
        /// Marker gets displayed in elliptical form.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Marker gets displayed in diamond form.
        /// </summary>
        Diamond,

        /// <summary>
        /// Marker gets displayed in trapezoidal form.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Marker gets displayed in pentagonal form.
        /// </summary>
        Pentagon,

        /// <summary>
        /// Custom Geometry can be applied through MarkerCustomGeometry property.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies different bar pointer styles.
    /// </summary>    
    public enum BarStyle
    {
        /// <summary>
        /// Bar pointer with thermo meter style
        /// </summary>
        Thermometer = 0,

        /// <summary>
        /// Bar pointer with Rectangle style
        /// </summary>
        Rectangle,

        /// <summary>
        /// Custom Geometry can be applied through BarCustomGeometry property.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies different needle styles.
    /// </summary>    
    public enum NeedleStyle
    {
        /// <summary>
        /// Needle gets displayed in triangular form.
        /// </summary>
        Triangle = 0,

        /// <summary>
        /// Needle gets displayed in rectangular form.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Needle gets displayed in trapezoidal form.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Needle gets displayed in the shape of an arrow.
        /// </summary>
        Arrow,

        /// <summary>
        /// Custom Geometry can be applied through NeedleCustomGeometry property.
        /// </summary>
        Custom
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
    /// Specify Linear scale style.
    /// </summary>
    public enum LinearScaleStyle
    {
        /// <summary>
        /// Rectangle shaped Linear Gauge.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Rounded rectangle shaped Linear Gauge.
        /// </summary>
        RoundedRectangle,

        /// <summary>
        /// Linear scale with thermo meter style
        /// </summary>
        Thermometer,

        /// <summary>
        /// Custom Geometry can be applied through ScaleCustomGeometry property.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies the Position of Units in RollingGauge.
    /// </summary>
	public enum UnitPosition
    {
        /// <summary>
        /// Unit will appear at the starting of the RollingGauge.
        /// </summary>
        Start=0,
        /// <summary>
        /// Unit will appear at the end of the RollingGauge.
        /// </summary>
        End
    }
  
    /// <summary>
    /// Specifies the Rolling Direction of RollingGauge.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Segments Rotate in AntiClockwiseDirection.
        /// </summary>
        AnitClockwise=0,
        /// <summary>
        /// Segments Rotate in Clockwise Direction.
        /// </summary>
        Clockwise
    }

}
