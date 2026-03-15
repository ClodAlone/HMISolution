#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
#if WINRT_USING
using Syncfusion.UI.Xaml.Diagram.Controller;
using Windows.Foundation; 
#endif
using System.ComponentModel;
using System.Collections;
#if !WPF
using Syncfusion.UI.Xaml.Diagram.Utility;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IConnectorSegment : INotifyPropertyChanged
    {
        BezierSmoothness BezierSmoothness { get; set; }
        SegmentConstraints Constraints { get; set; }
        //Editable Editable { get; set; }
    }

    internal interface IInternalSegment : IConnectorSegment, IWrapper
    {
        Action<PropChangedEventArgs<IInternalSegment>> SegmentChangedCallback { get; set; }
        IInternalSegment Prev { get; set; }
        IInternalSegment Next { get; set; }
        Point? EndPoint { get; set; }
    }

    public interface ILineSegment : IConnectorSegment
    {
        Point? Point { get; set; }
    }

    internal interface ILineInternal : ILineSegment
    {
    }

    public interface ILineSegmentLength : IConnectorSegment
    {
        DoubleExt Length { get; set; }
        DoubleExt Angle { get; set; }
        RelativeMode AngleMode { get; set; }
    }

    internal interface ILineLengthInternal : ILineSegmentLength
    {
    }

    public interface IOrthogonalSegment : IConnectorSegment
    {
        DoubleExt Length { get; set; }
        OrthogonalDirection Direction { get; set; }
    }

    internal interface IOrthoInternal : IOrthogonalSegment
    {
    }

    public interface IQuadraticCurveSegment : IConnectorSegment
    {
        Point? Point1 { get; set; }
        Point? Point2 { get; set; }
    }

    internal interface IQuadraticCurveInternal : IQuadraticCurveSegment 
    {
    }

    public interface ICubicCurveSegment : IConnectorSegment
    {
        Point? Point1 { get; set; }
        Point? Point2 { get; set; }
        Point? Point3 { get; set; }
        Vector ? Vector1 { get; set; }
        Vector ? Vector2 { get; set; }
    }

    internal interface ICubicCurveInternal : ICubicCurveSegment
    {
    }

    [Flags]
    public enum Editable
    {
        None,
        Delete = 1 << 0,
        Length = 1 << 1,
        Angle = 1 << 2,
        Point = 1 << 3,
        All = Delete | Length | Angle | Point
    }

    public enum ConnectorType
    {
        Line,
        Orthogonal,
        QuadraticBezier,
        CubicBezier,
# if WINRT
        PolyCubicBezier
#endif
    }

    public enum RelativeMode
    {
        Absolute,
        Relative
    }

    public enum OrthogonalDirection
    {
        /// <summary>
        /// Angle is chosen internally
        /// </summary>
        Auto = 2,
        /// <summary>
        /// Absolute angle: 180 Degree
        /// </summary>
        Left = 180,
        /// <summary>
        /// Absolute angle: 270 Degree or -90 Degree
        /// </summary>
        Top = 270,
        /// <summary>
        /// Absolute angle: 0 degree
        /// </summary>
        Right = 0,
        /// <summary>
        /// Absolute angle: 90 degree
        /// </summary>
        Bottom = 90,
        /// <summary>
        /// Relative Angle: 0 degree
        /// </summary>
        Straight = 1,
        /// <summary>
        /// Relative Angle: 90 degree
        /// </summary>
        ClockWise90 = 91,
        /// <summary>
        /// Relative Angle: 180 degree
        /// </summary>
        Opposite = 181,
        /// <summary>
        /// Relative Angle: 270 degree or -90 degree
        /// </summary>
        AntiClockWise90 = 271
    }

    public interface IConnectorSegments : IList<IConnectorSegment>, INotifyCollectionChanged
    {
    }

    public class ConnectorSegments : ObservableCollection<IConnectorSegment>, IConnectorSegments
    {
    }
}