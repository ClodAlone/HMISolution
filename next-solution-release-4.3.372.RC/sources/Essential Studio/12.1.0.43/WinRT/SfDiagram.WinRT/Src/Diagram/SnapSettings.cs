#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Diagram
{

    public interface ISnapSettings : INotifyPropertyChanged
    {
        Gridlines VerticalGridlines { get; set; }
        Gridlines HorizontalGridlines { get; set; }
        SnapConstraints SnapConstraints { get; set; }
        SnapToObject SnapToObject { get; set; }
        double SnapAngle { get; set; }
    }

    internal interface IInternalSnapSettings : ISnapSettings
    {
        bool CanSnapToHorizontalGridlines(ConnectorConstraints constraints);
        bool CanSnapToVerticalGridlines(ConnectorConstraints constraints);
        bool CanSnapToHorizontalGridlines(NodeConstraints constraints);
        bool CanSnapToVerticalGridlines(NodeConstraints constraints);
        bool CanSnapAngle(NodeConstraints constraints);
    }

    public partial class SnapSettings : ISnapSettings
    {
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    /// <summary>
    /// The parameters to represent the possible snaps and the proposed changes if the snaps are accepted.
    /// </summary>
    public class SnapParameter
    {
        private SnapChanges _snapChanges;

        /// <summary>
        /// Gets or sets the property that will be changed bacause of the snap.
        /// </summary>
        public SnapChanges SnapChanges
        {
            get { return _snapChanges; }
            private set { _snapChanges = value; }
        }

        private SnapReason _snapReason;

        /// <summary>
        /// Gets or sets the reason or target of the snap
        /// </summary>
        public SnapReason SnapReason
        {
            get { return _snapReason; }
            private set { _snapReason = value; }
        }

        private object _snapInfo;

        /// <summary>
        /// Gets or sets the information about the Target
        /// </summary>
        public object SnapInfo
        {
            get { return _snapInfo; }
            private set { _snapInfo = value; }
        }

        private SnapState _current;

        /// <summary>
        /// Gets or sets the current status of the  object that is being dragged.
        /// </summary>
        public SnapState Current
        {
            get { return _current; }
            set { _current = value; }
        }

        private SnapState _proposed;

        /// <summary>
        /// Gets or sets the proposed status of the  object that is being dragged.
        /// </summary>
        public SnapState Proposed
        {
            get { return _proposed; }
            set { _proposed = value; }
        }


        internal SnapParameter(SnapReason reason, SnapChanges changes, SnapState current, SnapState proposed, SnapInfo targetInfo)
        {
            SnapChanges = changes;
            SnapReason = reason;
            Current = current;
            Proposed = proposed;
            SnapInfo = targetInfo;
        }

        internal SnapParameter()
        {
        }
    }


    /// <summary>
    /// Status of the node that is being interacted.
    /// </summary>
    public struct SnapState
    {
        private double? _x;

        /// <summary>
        /// Gets or sets the X position of the object.
        /// </summary>
        public double? X
        {
            get { return _x; }
        }

        private double? _y;

        /// <summary>
        /// Gets or sets the Y position of the object.
        /// </summary>
        public double? Y
        {
            get { return _y; }
        }

        private double? _angle;

        /// <summary>
        /// Gets or sets Rotate angle of the object.
        /// </summary>
        public double? Angle
        {
            get { return _angle; }
        }

        private double? _width;

        /// <summary>
        /// Gets or sets width of the object.
        /// </summary>
        public double? Width
        {
            get { return _width; }
        }

        private double? _height;

        /// <summary>
        /// Gets or sets height of the object.
        /// </summary>
        public double? Height
        {
            get { return _height; }
        }

        internal SnapState(double? x, double? y, double? width, double? height, double? angle)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _angle = angle;
        }
    }

    /// <summary>
    /// Information about the Snap Target
    /// </summary>
    public class SnapInfo
    {
    }

    /// <summary>
    /// Information about the Target Gridline
    /// </summary>
    public class GridlineSnapInfo : SnapInfo
    {
        private double _target;

        /// <summary>
        /// Gets or sets the position of the target Gridline.
        /// </summary>
        public double Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private Side _side;

        /// <summary>
        /// Gets or sets the Side of the moving object has to be snapped
        /// </summary>
        public Side Side
        {
            get { return _side; }
            set { _side = value; }
        }

        internal GridlineSnapInfo(double target, Side side)
        {
            Target = target;
            Side = side;
        }

    }

    /// <summary>
    /// Informatoin about the target object.
    /// </summary>
    public class ObjectSnapInfo : SnapInfo
    {
        private object _target;

        /// <summary>
        /// Gets or sets object towards which the moving object has to be snapped
        /// </summary>
        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private SnapToObject _snapToObject;

        /// <summary>
        /// Gets or Sets the reason for the snap
        /// </summary>
        public SnapToObject SnapToObject
        {
            get { return _snapToObject; }
            set { _snapToObject = value; }
        }

        internal ObjectSnapInfo(object target, SnapToObject snapToObject)
        {
            Target = target;
            SnapToObject = snapToObject;
        }

    }


    /// <summary>
    ///Information about the Collection of connectors that are nearer to the moving object.
    /// </summary>
    public class SegmentSnapInfo : SnapInfo
    {
        private List<TargetConnector> _target;
        /// <summary>
        /// Gets or sets the collection of connectors that are nearer to the moving object
        /// </summary>
        public List<TargetConnector> TargetConnectors
        {
            get { return _target; }
            set { _target = value; }
        }

        internal SegmentSnapInfo(List<TargetConnector> target)
        {
            TargetConnectors = target;
        }
    }
    /// <summary>
    /// Connector that is nearer to the moving object.
    /// </summary>
    public class TargetConnector
    {
        private Connector _targetConnector;

        /// <summary>
        /// Gets or sets the Connector that is nearer to the moving object
        /// </summary>
        public Connector Connector
        {
            get { return _targetConnector; }
            private set { _targetConnector = value; }
        }

        private IConnectorSegment _targetSegment;

        /// <summary>
        /// Gets or sets the nearest segment of the target connector.
        /// </summary>
        public IConnectorSegment TargetSegment
        {
            get { return _targetSegment; }
            private set { _targetSegment = value; }
        }

        private Point _segmentStartPoint;

        /// <summary>
        /// Gets or sets the starting point of the TargetSegment
        /// </summary>
        public Point SegmentStartPoint
        {
            get { return _segmentStartPoint; }
            private set { _segmentStartPoint = value; }
        }

        private List<Point> _intersectingPoint;

        /// <summary>
        /// Gets or sets the collection of intersecting points between the target segment and the moving object.
        /// </summary>
        public List<Point> IntersectingPoints
        {
            get { return _intersectingPoint; }
            set { _intersectingPoint = value; }
        }
        internal TargetConnector(Connector target, IConnectorSegment targetsegment, Point segmentStartingPoint, List<Point> intersectingPoints)
        {
            Connector = target;
            TargetSegment = targetsegment;
            SegmentStartPoint = segmentStartingPoint;
            IntersectingPoints = intersectingPoints;
        }

    }

    /// <summary>
    /// Information about the objects that are equally spaced.
    /// </summary>
    public class EqualSpaceSnapInfo : SnapInfo
    {
        private object _target;

        /// <summary>
        /// Gets or sets the nearest object among the equally spaced objects.
        /// </summary>
        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private SnapToObject _snapToObject;

        /// <summary>
        /// Gets or sets the reason of the snap.
        /// </summary>
        public SnapToObject SnapToObject
        {
            get { return _snapToObject; }
            set { _snapToObject = value; }
        }

        private double _distance;

        /// <summary>
        /// Gets or sets the distance between equally spaced objects
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        private List<object> _equallySpacedObjects;
        /// <summary>
        /// Gets or sets the collection of objects that are equally spaced
        /// </summary>
        public List<object> EquallySpacedObjects
        {
            get { return _equallySpacedObjects; }
            set { _equallySpacedObjects = value; }
        }

        internal EqualSpaceSnapInfo(object target, SnapToObject snapToObject, double distance, List<object> spacedObjects)
        {
            Target = target;
            SnapToObject = snapToObject;
            Distance = distance;
            EquallySpacedObjects = spacedObjects;
        }
    }

    /// <summary>
    /// Information about the objects that are of the same size of the moving object.
    /// </summary>
    public class SameSizeSnapInfo : SnapInfo
    {
        private List<object> _target;

        /// <summary>
        /// Gets or sets the collection of same size objects
        /// </summary>
        public List<object> SameSizeObjects
        {
            get { return _target; }
            set { _target = value; }
        }

        private SnapToObject _snapToObject;

        /// <summary>
        /// Gets or sets the reason of Snap.
        /// </summary>
        public SnapToObject SnapToObject
        {
            get { return _snapToObject; }
            set { _snapToObject = value; }
        }

        internal SameSizeSnapInfo(List<object> target, SnapToObject snapToObject)
        {
            SameSizeObjects = target;
            SnapToObject = snapToObject;
        }
    }
}
