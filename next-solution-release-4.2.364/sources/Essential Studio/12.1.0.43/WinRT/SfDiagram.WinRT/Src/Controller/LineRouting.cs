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
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#else
using System.Windows.Media;
using System.Windows;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;


namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    class LineRouting
    {
        IInternalConnector connector;
        int count = 0;
        bool _needToRouteFirstSegment = false;
        bool _needToRouteLastSegment = false;
        bool _isrouted = false;
        bool _isRecursivelycalled = false;
        SharedData _mSharedData;

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
        }

        public void UpdateRouting(IInternalConnector connectorwrapper )
        {
            connector = connectorwrapper;
            if (connector.KnownSourceNode != null && connector.KnownTargetNode != null &&
                connector.Segments != null && !(connector.Segments.Any(s => !(s is IOrthogonalSegment))))
            {
                obstacles.Clear();
                GetObstacleRegion();
                if (obstacles.Count > 0)
                {
                    _needToRouteFirstSegment = false;
                    _needToRouteLastSegment = false;
                    count = this.connector.InternalSegments.Count();
                    for (int i = 0; i < count; i++)
                    {
                        LineSegment segment = this.connector.InternalSegments[i] as LineSegment;

                        if (i > 0)
                        {
                            UpdateRouting(segment, connector.InternalSegments[i - 1] as LineSegment);
                        }
                        else
                        {
                            UpdateRouting(segment, null);
                        }

                    }
                    # region Removing first segment overlap on source node if source point is on top
                    if ((connector.SourcePoint.Y == connector.KnownSourceNode.Bounds.Top && (connector.SourcePoint.Y < (connector.InternalSegments[0] as LineSegment).Point.Y) || (long)connector.SourcePoint.Y == (long)connector.KnownSourceNode.Bounds.Bottom && connector.SourcePoint.Y > (connector.InternalSegments[0] as LineSegment).Point.Y) && _isrouted && connector.KnownSourcePort == null)
                    {
                        if (connector.SourcePoint.Y == connector.KnownSourceNode.Bounds.Top && _isrouted && connector.KnownSourceNode.Bounds.Bottom <= (connector.InternalSegments[0] as LineSegment).Point.Y)
                        {
                            connector.SourcePoint = new Point(connector.SourcePoint.X, connector.KnownSourceNode.Bounds.Bottom);

                        }
                        else if (connector.SourcePoint.Y == connector.KnownSourceNode.Bounds.Bottom && _isrouted && connector.KnownSourceNode.Bounds.Top >= (connector.InternalSegments[0] as LineSegment).Point.Y)
                        {
                            connector.SourcePoint = new Point(connector.SourcePoint.X, connector.KnownSourceNode.Bounds.Top);

                        }
                        else
                        {
                            if (connector.SourcePoint.X == (connector.InternalSegments[0] as LineSegment).Point.X)
                            {
                                if (connector.SourcePoint.X < (connector.InternalSegments[1] as LineSegment).Point.X)
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownSourceNode.Bounds.Right, connector.KnownSourceNode.Center.Y), (connector.InternalSegments[1] as LineSegment).Point);
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(0);
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Right, connector.KnownSourceNode.Center.Y);

                                        (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, connector.SourcePoint.Y);

                                    }
                                    else
                                    {
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Right, connector.KnownSourceNode.Center.Y);
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X + 5, connector.SourcePoint.Y);
                                        Point? _ref = (connector.InternalSegments[0] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point(connector.SourcePoint.X + 5, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, 1);

                                    }
                                }
                                else
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownSourceNode.Bounds.Left, connector.KnownSourceNode.Center.Y), (connector.InternalSegments[1] as LineSegment).Point);
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(0);
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Left, connector.KnownSourceNode.Center.Y);

                                        (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, connector.SourcePoint.Y);

                                    }
                                    else
                                    {
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Left, connector.KnownSourceNode.Center.Y);
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X - 5, connector.SourcePoint.Y);
                                        Point? _ref = (connector.InternalSegments[0] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point(connector.SourcePoint.X - 5, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, 1);

                                    }
                                }
                            }
                        }

                        (connector as ConnectorWrapper).isRoutted = true;
                        connector.UpdateGeometry();
                        if (connector.SourceDecorator != null)
                        {
                            (connector.View as Connector).UpdateDecorator();
                        }

                    }
                    # endregion

                    # region Removing first segment overlap on source node if source point is on left
                    if ((connector.SourcePoint.X == connector.KnownSourceNode.Bounds.Left && (connector.SourcePoint.X < (connector.InternalSegments[0] as LineSegment).Point.X) || connector.SourcePoint.X == connector.KnownSourceNode.Bounds.Right && connector.SourcePoint.X > (connector.InternalSegments[0] as LineSegment).Point.X) && _isrouted && connector.KnownSourcePort == null)
                    {
                        if (connector.SourcePoint.X == connector.KnownSourceNode.Bounds.Left && _isrouted && connector.KnownSourceNode.Bounds.Right <= (connector.InternalSegments[0] as LineSegment).Point.X)
                        {
                            connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Left, connector.SourcePoint.Y);


                        }
                        else if (connector.SourcePoint.Y == connector.KnownSourceNode.Bounds.Bottom && _isrouted && connector.KnownSourceNode.Bounds.Top >= (connector.InternalSegments[0] as LineSegment).Point.Y)
                        {
                            connector.SourcePoint = new Point(connector.KnownSourceNode.Bounds.Right, connector.SourcePoint.Y);


                        }


                        else
                        {
                            if (connector.SourcePoint.Y == (connector.InternalSegments[0] as LineSegment).Point.Y)
                            {
                                if (connector.SourcePoint.Y < (connector.InternalSegments[1] as LineSegment).Point.Y)
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Bottom), (connector.InternalSegments[1] as LineSegment).Point);
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(0);
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Bottom);

                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X, (connector.InternalSegments[1] as LineSegment).Point.Y);


                                    }
                                    else
                                    {
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Bottom);
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X, connector.SourcePoint.Y + 5);
                                        Point? _ref = (connector.InternalSegments[0] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point((connector.InternalSegments[1] as LineSegment).Point.X, connector.SourcePoint.Y + 5);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, 1);

                                    }
                                }
                                else
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top), (connector.InternalSegments[1] as LineSegment).Point);
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(0);
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top);

                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X, (connector.InternalSegments[1] as LineSegment).Point.Y);


                                    }
                                    else
                                    {
                                        connector.SourcePoint = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top);
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(connector.SourcePoint.X, connector.SourcePoint.Y - 5);
                                        Point? _ref = (connector.InternalSegments[0] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point((connector.InternalSegments[1] as LineSegment).Point.X, connector.SourcePoint.Y - 5);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, 1);

                                    }
                                }
                            }
                        }
                        (connector as ConnectorWrapper).isRoutted = true;
                        connector.UpdateGeometry();
                        if (connector.SourceDecorator != null)
                        {
                            (connector.View as Connector).UpdateDecorator();
                        }
                    }
                    # endregion

                    # region Removing last segment overlap on target node if target point is on top/bottom
                    if ((connector.TargetPoint.Y == connector.KnownTargetNode.Bounds.Top && (connector.TargetPoint.Y < (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y) || connector.TargetPoint.Y == connector.KnownTargetNode.Bounds.Bottom && connector.TargetPoint.Y > (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y) && _isrouted && connector.KnownTargetPort == null)
                    {
                        if (connector.TargetPoint.Y == connector.KnownTargetNode.Bounds.Top && _isrouted && connector.KnownTargetNode.Bounds.Bottom <= (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y)
                        {
                            connector.TargetPoint = new Point(connector.TargetPoint.X, connector.KnownTargetNode.Bounds.Bottom);
                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;

                        }
                        else if (connector.TargetPoint.Y == connector.KnownTargetNode.Bounds.Bottom && _isrouted && connector.KnownTargetNode.Bounds.Top >= (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y)
                        {
                            connector.TargetPoint = new Point(connector.TargetPoint.X, connector.KnownTargetNode.Bounds.Top);
                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                        }
                        else
                        {
                            if (connector.TargetPoint.X == (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X)
                            {
                                if (connector.TargetPoint.X < (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.X)
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y), new Point((connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.X, connector.KnownTargetNode.Center.Y));
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point((connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X, connector.TargetPoint.Y);

                                    }
                                    else
                                    {
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X + 5, connector.TargetPoint.Y);
                                        Point? _ref = (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point(connector.TargetPoint.X + 5, (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.Y);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, connector.InternalSegments.Count - 2);

                                    }
                                }
                                else
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Center.Y), new Point((connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.X, connector.KnownTargetNode.Center.Y));
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Center.Y);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point((connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X, connector.TargetPoint.Y);

                                    }
                                    else
                                    {
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Center.Y);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X - 5, connector.TargetPoint.Y);
                                        Point? _ref = (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point(connector.TargetPoint.X - 5, (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.Y);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, connector.InternalSegments.Count - 2);

                                    }
                                }
                            }

                        }
                        (connector as ConnectorWrapper).isRoutted = true;
                        connector.UpdateGeometry();
                        if (connector.TargetDecorator != null)
                        {
                            (connector.View as Connector).UpdateDecorator();
                        }
                    }
                    # endregion
                    # region Removing last segment overlap on target node if target point is on left/right
                    if ((connector.TargetPoint.X == connector.KnownTargetNode.Bounds.Left && (connector.TargetPoint.X < (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X) || connector.TargetPoint.X == connector.KnownTargetNode.Bounds.Right && connector.TargetPoint.X > (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X) && _isrouted && connector.KnownTargetPort == null)
                    {
                        if (connector.TargetPoint.X == connector.KnownTargetNode.Bounds.Left && _isrouted && connector.KnownTargetNode.Bounds.Right <= (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y)
                        {
                            connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Right, connector.TargetPoint.Y);
                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;

                        }
                        else if (connector.TargetPoint.Y == connector.KnownTargetNode.Bounds.Bottom && _isrouted && connector.KnownTargetNode.Bounds.Top >= (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y)
                        {
                            connector.TargetPoint = new Point(connector.KnownTargetNode.Bounds.Left, connector.TargetPoint.Y);
                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;

                        }
                        else
                        {
                            if (connector.TargetPoint.Y == (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y)
                            {
                                if (connector.TargetPoint.Y < (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.Y)
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom), new Point(connector.KnownTargetNode.Center.X, (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.Y));
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X, (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y);

                                    }
                                    else
                                    {
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X, connector.TargetPoint.Y + 5);
                                        Point? _ref = (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point((connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.X, connector.TargetPoint.Y + 5);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, connector.InternalSegments.Count - 2);

                                    }
                                }
                                else
                                {
                                    IInternalNode nod = FindObstacle(new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top), new Point(connector.KnownTargetNode.Center.X, (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.Y));
                                    if (nod == null)
                                    {
                                        connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X, (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y);

                                    }
                                    else
                                    {
                                        connector.TargetPoint = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top);
                                        (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                        (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(connector.TargetPoint.X, connector.TargetPoint.Y - 5);
                                        Point? _ref = (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point;
                                        double d = 0;
                                        Point _new = new Point((connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point.X, connector.TargetPoint.Y - 5);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _ref, ref d, _new, connector.InternalSegments.Count - 2);

                                    }
                                }
                            }

                        }
                        (connector as ConnectorWrapper).isRoutted = true;
                        connector.UpdateGeometry();
                        if (connector.TargetDecorator != null)
                        {
                            (connector.View as Connector).UpdateDecorator();
                        }
                    }
                    _isrouted = false;
                    # endregion


                    # region avoiding first segment overlap if source node overlaps on another node

                    if (_needToRouteFirstSegment && connector.KnownSourcePort == null && !_isRecursivelycalled)
                    {
                        bool _isSourceModified = false;
                        Rect r = Rect.Empty;
                        FindObstacles(connector.KnownSourceNode.Bounds, ref r);
                        if (r != Rect.Empty)
                        {

                            if (((long)connector.SourcePoint.X == (long)connector.KnownSourceNode.Center.X && (long)connector.SourcePoint.Y == (long)connector.KnownSourceNode.Bounds.Top) || ((long)connector.SourcePoint.X == (long)connector.KnownSourceNode.Center.X && (long)connector.SourcePoint.Y == (long)connector.KnownSourceNode.Bounds.Bottom))
                            {
                                if (connector.SourcePoint.X < (connector.InternalSegments[1] as LineSegment).Point.X)
                                {

                                    Point _right = new Point(connector.KnownSourceNode.Bounds.Right, connector.KnownSourceNode.Center.Y);

                                    if (!r.Contains(_right))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(0);
                                            connector.SourcePoint = _right;
                                        }
                                        else
                                        {
                                            connector.SourcePoint = _right;
                                            _right = new Point(_right.X + 5, _right.Y);
                                            Point? pt = _right;
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, new Point(_right.X + 5, (connector.InternalSegments[1] as LineSegment).Point.Y), 1);
                                        }
                                        _isSourceModified = true;
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, _right.Y);
                                    }
                                    else
                                    {
                                        Point _left = new Point(connector.KnownSourceNode.Bounds.Left, connector.KnownSourceNode.Center.Y);
                                        if (!r.Contains(_left))
                                        {
                                            (connector.InternalSegments[0] as LineSegment).Point = new Point(_left.X - 5, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                            connector.SourcePoint = _left;
                                            Point? _lef = _left;
                                            Point? _new = new Point(_left.X - 5, _left.Y);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, 0);
                                            _isSourceModified = true;
                                        }
                                    }
                                }
                                else
                                {
                                    Point _left = new Point(connector.KnownSourceNode.Bounds.Left, connector.KnownSourceNode.Center.Y);

                                    if (!r.Contains(_left))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(0);
                                            connector.SourcePoint = _left;
                                        }
                                        else
                                        {
                                            connector.SourcePoint = _left;
                                            _left = new Point(_left.X - 5, _left.Y);
                                            Point? pt = _left;
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, new Point(_left.X - 5, (connector.InternalSegments[1] as LineSegment).Point.Y), 1);
                                        }
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, _left.Y);
                                        _isSourceModified = true;
                                    }
                                    else
                                    {

                                        Point _right = new Point(connector.KnownSourceNode.Bounds.Right, connector.KnownSourceNode.Center.Y);
                                        if (!r.Contains(_right))
                                        {
                                            (connector.InternalSegments[0] as LineSegment).Point = new Point(_right.X + 5, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                            connector.SourcePoint = _right;
                                            Point? _lef = _right;
                                            Point? _new = new Point(_right.X + 5, _right.Y);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, 0);
                                            _isSourceModified = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (connector.SourcePoint.Y < (connector.InternalSegments[1] as LineSegment).Point.Y)
                                {

                                    Point _bottom = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Bottom);

                                    if (!r.Contains(_bottom))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(0);
                                            connector.SourcePoint = _bottom;
                                        }
                                        else
                                        {
                                            connector.SourcePoint = _bottom;
                                            _bottom = new Point(_bottom.X, _bottom.Y + 5);
                                            Point? pt = _bottom;
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, new Point((connector.InternalSegments[1] as LineSegment).Point.X, _bottom.Y + 5), 1);
                                        }
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(_bottom.X, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                        _isSourceModified = true;
                                    }
                                    else
                                    {
                                        Point _top = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top);
                                        if (!r.Contains(_top))
                                        {
                                            (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, _top.Y - 5);
                                            connector.SourcePoint = _top;
                                            Point? _lef = _top;
                                            Point? _new = new Point(_top.X, _top.Y - 5);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, 0);
                                            _isSourceModified = true;
                                        }
                                    }
                                }
                                else
                                {

                                    Point _top = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top);

                                    if (!r.Contains(_top))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(0);
                                            connector.SourcePoint = _top;
                                        }
                                        else
                                        {
                                            connector.SourcePoint = _top;
                                            _top = new Point(_top.X, _top.Y - 5);
                                            Point? pt = _top;
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, new Point((connector.InternalSegments[1] as LineSegment).Point.X, _top.Y - 5), 1);
                                        }
                                        (connector.InternalSegments[0] as LineSegment).Point = new Point(_top.X, (connector.InternalSegments[1] as LineSegment).Point.Y);
                                        _isSourceModified = true;
                                    }
                                    else
                                    {
                                        Point _bottom = new Point(connector.KnownSourceNode.Center.X, connector.KnownSourceNode.Bounds.Top);
                                        if (!r.Contains(_bottom))
                                        {
                                            (connector.InternalSegments[0] as LineSegment).Point = new Point((connector.InternalSegments[1] as LineSegment).Point.X, _bottom.Y - 5);
                                            connector.SourcePoint = _bottom;
                                            Point? _lef = _bottom;
                                            Point? _new = new Point(_bottom.X, _bottom.Y - 5);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, 0);
                                            _isSourceModified = true;
                                        }
                                    }
                                }
                            }
                        }
                        _isRecursivelycalled = true;
                        if (_isSourceModified)
                        {
                            _isRecursivelycalled = true;
                            UpdateRouting(connector);
                            (connector as ConnectorWrapper).isRoutted = true;
                            connector.UpdateGeometry();
                            if (connector.SourceDecorator != null)
                            {
                                (connector.View as Connector).UpdateDecorator();
                            }
                        }
                    }

                    # endregion

                    # region avoiding last segment overlap on target node
                    if (_needToRouteLastSegment && connector.KnownTargetPort == null && !_isRecursivelycalled)
                    {
                        bool _isTargetModified = false;
                        Rect r = Rect.Empty;
                        FindObstacles(connector.KnownTargetNode.Bounds, ref r);
                        if (r != Rect.Empty)
                        {
                            Point third = connector.SourcePoint;
                            if (connector.InternalSegments.Count >= 3)
                                third = (connector.InternalSegments[connector.InternalSegments.Count - 3] as LineSegment).Point;
                            if (((long)connector.TargetPoint.X == (long)connector.KnownTargetNode.Center.X && (long)connector.TargetPoint.Y == (long)connector.KnownTargetNode.Bounds.Top) || ((long)connector.TargetPoint.X == (long)connector.KnownTargetNode.Center.X && (long)connector.TargetPoint.Y == (long)connector.KnownTargetNode.Bounds.Bottom))
                            {
                                if (connector.TargetPoint.X < third.X)
                                {

                                    Point _right = new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y);

                                    if (!r.Contains(_right))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                            connector.TargetPoint = _right;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point((connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X, _right.Y);
                                        }
                                        else
                                        {
                                            connector.TargetPoint = _right;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = _right;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(third.X, _right.Y);

                                        }
                                        _isTargetModified = true;
                                    }
                                    else
                                    {
                                        Point _left = new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Bounds.Top + connector.KnownTargetNode.Bounds.Height / 2);
                                        if (!r.Contains(_left))
                                        {
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_left.X - 5, third.Y);
                                            connector.TargetPoint = _left;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            Point? _lef = _left;
                                            Point? _new = new Point(_left.X - 5, _left.Y);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, connector.InternalSegments.Count - 1);
                                            _isTargetModified = true;
                                        }
                                    }
                                }
                                else
                                {
                                    Point _left = new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Center.Y);

                                    if (!r.Contains(_left))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                            connector.TargetPoint = _left;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point((connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.X, _left.Y);
                                        }
                                        else
                                        {
                                            connector.TargetPoint = _left;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = _left;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(third.X, _left.Y);
                                        }
                                        _isTargetModified = true;
                                    }
                                    else
                                    {
                                        Point _right = new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y);
                                        if (!r.Contains(_right))
                                        {
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_right.X + 5, third.Y);
                                            connector.TargetPoint = _right;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            Point? _lef = _right;
                                            Point? _new = new Point(_right.X + 5, _right.Y);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, connector.InternalSegments.Count - 1);
                                            _isTargetModified = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (connector.TargetPoint.Y < third.Y)
                                {

                                    Point _bottom = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom);

                                    if (!r.Contains(_bottom))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                            connector.TargetPoint = _bottom;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_bottom.X, (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y);
                                        }
                                        else
                                        {
                                            connector.TargetPoint = _bottom;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = _bottom;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_bottom.X, third.Y);

                                        }
                                        _isTargetModified = true;
                                    }
                                    else
                                    {

                                        Point _top = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top);
                                        if (!r.Contains(_top))
                                        {
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(third.X, _top.Y - 5);
                                            connector.TargetPoint = _top;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            Point? _lef = _top;
                                            Point? _new = new Point(_top.X, _top.Y - 5);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, connector.InternalSegments.Count - 1);
                                            _isTargetModified = true;
                                        }
                                    }
                                }
                                else
                                {

                                    Point _top = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top);

                                    if (!r.Contains(_top))
                                    {
                                        if (connector.InternalSegments.Count > 2)
                                        {
                                            connector.InternalSegments.RemoveAt(connector.InternalSegments.Count - 1);
                                            connector.TargetPoint = _top;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_top.X, (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point.Y);
                                        }
                                        else
                                        {
                                            connector.TargetPoint = _top;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = _top;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(_top.X, third.Y);

                                        }
                                        _isTargetModified = true;
                                    }
                                    else
                                    {
                                        Point _bottom = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom);
                                        if (!r.Contains(_bottom))
                                        {
                                            (connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment).Point = new Point(third.X, _bottom.Y + 5);
                                            connector.TargetPoint = _bottom;
                                            (connector.InternalSegments[connector.InternalSegments.Count - 1] as LineSegment).Point = connector.TargetPoint;
                                            Point? _lef = _bottom;
                                            Point? _new = new Point(_bottom.X, _bottom.Y + 5);
                                            double t = 0;
                                            (connector as ConnectorWrapper).InsertLineSegment(ref _lef, ref t, _new.Value, connector.InternalSegments.Count - 1);
                                            _isTargetModified = true;
                                        }
                                    }
                                }
                            }
                        }
                        _isRecursivelycalled = true;
                        if (_isTargetModified)
                        {

                            UpdateRouting(connector);
                            (connector as ConnectorWrapper).isRoutted = true;
                            connector.UpdateGeometry();
                            if (connector.TargetDecorator != null)
                            {
                                (connector.View as Connector).UpdateDecorator();
                            }
                        }
                    }
                    # endregion

                    if (_isRecursivelycalled)
                        _isRecursivelycalled = false;
                }
            }

        }


        private void UpdateRouting(LineSegment segment, LineSegment prev)
        {
            Point st = new Point(0, 0);
            if (prev == null)
            {
                prev = new LineSegment() { Point = connector.SourcePoint };
                st = connector.SourcePoint;
            }
            else
            {
                st = prev.Point;
            }
            List<IInternalNode> nodes = FindObstacles(prev.Point, segment.Point);

            if (nodes.Count > 0 && segment.Point!=prev.Point)
            {
                foreach (IInternalNode node in nodes)
                {
                    if (node != connector.KnownSourceNode && node != connector.KnownTargetNode)
                    {

                        Rect rect = new Rect(st, segment.Point);
                        if ((node.Bounds.IsIntersect(connector.KnownSourceNode.Bounds)
                               ||
                             node.Bounds.Contains(segment.Point)) && connector.InternalSegments.IndexOf(segment) == 0 || (node.Bounds.IsIntersect(connector.KnownTargetNode.Bounds) && connector.InternalSegments.IndexOf(segment) == connector.InternalSegments.Count - 1))
                        {
                            if (node.Bounds.IsIntersect(connector.KnownSourceNode.Bounds))
                                _needToRouteFirstSegment = true;
                            if (node.Bounds.IsIntersect(connector.KnownTargetNode.Bounds))
                                _needToRouteLastSegment = true;
                            continue;
                        }
                        else
                        {
                            //Routing Vertical segments

                            #region VerticalSegmentRouting

                            if (st.X == segment.Point.X)
                            {
                                Point prevleft = new Point(node.Bounds.Left - 5, prev.Point.Y);

                                Point left = new Point(node.Bounds.Left - 5, segment.Point.Y);

                                Point prevRight = new Point(node.Bounds.Right + 5, prev.Point.Y);

                                Point Right = new Point(node.Bounds.Right + 5, segment.Point.Y);

                                bool IsRouted;

                                IsRouted = Route(prev, segment, prevleft, left, prevRight, Right);

                                if (IsRouted)
                                {
                                    //break;
                                }
                                else
                                {
                                    IInternalNode leftObstacle = FindObstacle(prevleft, left);
                                    IInternalNode rightObstacle = FindObstacle(prevRight, Right);
                                    FindPoints(leftObstacle, rightObstacle, ref prevleft, ref left, ref prevRight,
                                               ref Right, segment, prev);
                                    IsRouted = Route(prev, segment, prevleft, left, prevRight, Right);
                                    if (IsRouted)
                                    {

                                    }
                                    else
                                    {
                                        leftObstacle = FindObstacle(prevleft, left);
                                        rightObstacle = FindObstacle(prevRight, Right);
                                        FindPoints(leftObstacle, rightObstacle, ref prevleft, ref left, ref prevRight,
                                                   ref Right, segment, prev);
                                        IsRouted = Route(prev, segment, prevleft, left, prevRight, Right);
                                        if (IsRouted)
                                        {

                                        }
                                        else
                                        {
                                            int j = connector.InternalSegments.IndexOf(segment);
                                            prev.Point = new Point(prev.Point.X + 25, prev.Point.Y);
                                            segment.Point = new Point(segment.Point.X + 25, segment.Point.Y);
                                            if (j == 1)
                                            {
                                                UpdateRouting(prev, null);

                                            }
                                            else
                                            {
                                                if (j > 1)
                                                {
                                                    UpdateRouting(prev, connector.InternalSegments[j - 2] as LineSegment);
                                                }
                                            }
                                            if (j == 0)
                                            {
                                                UpdateRouting(segment, null);
                                            }
                                            else
                                            {
                                                if (j > 0)
                                                {
                                                    UpdateRouting(segment, connector.InternalSegments[j - 1] as LineSegment);
                                                }
                                            }

                                            //(View as Connector).UpdateDecorator();
                                        }
                                    }
                                }

                                #region FirstsegmentRouting

                                if (connector.InternalSegments.IndexOf(segment) == 0)
                                {

                                    Point? pt = segment.Point;
                                    Point old = segment.Point;
                                    double t = 0;
                                    Point? _new;
                                    if (connector.SourcePoint.Y < prev.Point.Y)
                                    {
                                        segment.Point = new Point(connector.SourcePoint.X, connector.SourcePoint.Y + 10);
                                        _new = new Point(old.X, segment.Point.Y);
                                    }
                                    else
                                    {
                                        segment.Point = new Point(connector.SourcePoint.X, connector.SourcePoint.Y - 10);
                                        _new = new Point(old.X, segment.Point.Y);

                                    }
                                    Point _new1 = new Point((connector.InternalSegments[1] as LineSegment).Point.X,
                                                            segment.Point.Y);
                                    if (IsVisible(segment.Point, _new1) &&
                                        IsVisible(_new1, (connector.InternalSegments[1] as LineSegment).Point) && connector.InternalSegments.Count>2)
                                    {
                                        (connector.InternalSegments[1] as LineSegment).Point = _new1;
                                    }
                                    else
                                    {
                                        (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, _new.Value, 1);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _new, ref t, old, 2);
                                        count += 2;
                                    }

                                }

                                #endregion


                                //Routing Last segment


                                #region Last segment Routing


                                if (connector.InternalSegments.IndexOf(segment) == connector.InternalSegments.Count - 1)
                                {
                                    Point? pt = segment.Point;

                                    double t = 0;
                                    Point? _new;
                                    if (connector.TargetPoint.Y < prev.Point.Y)
                                    {
                                        segment.Point = new Point(segment.Point.X, segment.Point.Y + 10);
                                        _new = new Point(this.connector.TargetPoint.X, this.connector.TargetPoint.Y + 10);
                                    }
                                    else
                                    {
                                        segment.Point = new Point(segment.Point.X, segment.Point.Y - 10);
                                        _new = new Point(this.connector.TargetPoint.X, this.connector.TargetPoint.Y - 10);
                                    }
                                    {
                                        if (IsVisible(_new.Value, connector.TargetPoint) || connector.KnownTargetPort != null)
                                        {
                                            (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _new.Value);
                                            (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, connector.TargetPoint);
                                            count += 1;
                                        }
                                        else
                                        {
                                            Point _left = new Point(connector.KnownTargetNode.Bounds.Left, connector.KnownTargetNode.Center.Y);
                                            Point _second = new Point(_left.X - 10, _left.Y);
                                            if (IsVisible(_left, _second))
                                            {
                                                segment.Point = new Point(segment.Point.X, _left.Y);
                                                if (!IsVisible(prev.Point, segment.Point))
                                                    UpdateRouting(segment, prev);
                                                (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _left);
                                                connector.TargetPoint = _left;
                                                (connector as ConnectorWrapper).isRoutted = true;
                                                connector.UpdateGeometry();
                                                (connector.View as Connector).UpdateDecorator();
                                            }
                                            else
                                            {
                                                _left = new Point(connector.KnownTargetNode.Bounds.Left - 5, connector.KnownTargetNode.Bounds.Bottom + 5);
                                                Point _bottom = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Bottom);
                                                _second = new Point(_bottom.X, _bottom.Y + 10);
                                                if (IsVisible(_bottom, _second))
                                                {
                                                    segment.Point = new Point(segment.Point.X, _second.Y);
                                                    _new = new Point(_bottom.X, segment.Point.Y);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _second);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, _bottom);
                                                    connector.TargetPoint = _bottom;
                                                    if (!IsVisible(_second, segment.Point))
                                                    {
                                                        UpdateRouting(connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment, segment);
                                                    }
                                                    (connector as ConnectorWrapper).isRoutted = true;
                                                    connector.UpdateGeometry();
                                                    (connector.View as Connector).UpdateDecorator();
                                                }
                                                else
                                                {
                                                    (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _new.Value);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, connector.TargetPoint);
                                                    count += 1;
                                                }
                                            }
                                        }
                                    }

                                }

                                #endregion

                            }
                            #endregion


                            //Routing Horizontal Segments
                            #region HorizontalsegmentRouting

                            else if (st.Y == segment.Point.Y)
                            {
                                Point prevTop = new Point(prev.Point.X, node.Bounds.Top - 5);

                                Point Top = new Point(segment.Point.X, node.Bounds.Top - 5);

                                Point prevBottom = new Point(prev.Point.X, node.Bounds.Bottom + 5);

                                Point Bottom = new Point(segment.Point.X, node.Bounds.Bottom + 5);

                                bool IsRouted;

                                IsRouted = RouteHSegment(prev, segment, prevTop, Top, prevBottom, Bottom);
                                if (IsRouted)
                                {

                                }
                                else
                                {
                                    IInternalNode topObstacle = FindObstacle(prevTop, Top);
                                    IInternalNode bottomObstacle = FindObstacle(prevBottom, Bottom);
                                    FindPointsforHsegRouting(topObstacle, bottomObstacle, ref prevTop, ref Top,
                                                             ref prevBottom, ref Bottom, segment, prev);

                                    IsRouted = RouteHSegment(prev, segment, prevTop, Top, prevBottom, Bottom);
                                    if (IsRouted)
                                    {

                                    }
                                    else
                                    {

                                        topObstacle = FindObstacle(prevTop, Top);
                                        bottomObstacle = FindObstacle(prevBottom, Bottom);
                                        FindPointsforHsegRouting(topObstacle, bottomObstacle, ref prevTop, ref Top,
                                                                 ref prevBottom, ref Bottom, segment, prev);

                                        IsRouted = RouteHSegment(prev, segment, prevTop, Top, prevBottom, Bottom);
                                        if (IsRouted)
                                        {

                                        }
                                        else
                                        {
                                            int j = connector.InternalSegments.IndexOf(segment);
                                            prev.Point = new Point(prev.Point.X, prev.Point.Y + 25);
                                            segment.Point = new Point(segment.Point.X, segment.Point.Y + 25);
                                            if (j == 1)
                                            {
                                                UpdateRouting(prev, null);
                                            }
                                            else
                                            {
                                                if (j >= 2)
                                                {
                                                    UpdateRouting(prev, connector.InternalSegments[j - 2] as LineSegment);
                                                }
                                            }
                                            if (j == 0)
                                            {
                                                UpdateRouting(segment, null);
                                            }
                                            else
                                            {
                                                if (j > 0)
                                                {
                                                    UpdateRouting(segment, connector.InternalSegments[j - 1] as LineSegment);
                                                }
                                            }
                                            //(View as Connector).UpdateDecorator();
                                        }
                                    }
                                }
                                //Routing Last segment

                                #region Last segment Routing

                                if (connector.InternalSegments.IndexOf(segment) == connector.InternalSegments.Count - 1)
                                {
                                    Point? _new;
                                    Point segPoint = segment.Point;

                                    if (prev.Point.X < connector.TargetPoint.X)
                                    {
                                        segment.Point = new Point(segment.Point.X - 10, segment.Point.Y);
                                        _new = new Point(this.connector.TargetPoint.X - 10, this.connector.TargetPoint.Y);
                                    }
                                    else
                                    {
                                        segment.Point = new Point(segment.Point.X + 10, segment.Point.Y);
                                        _new = new Point(this.connector.TargetPoint.X + 10, this.connector.TargetPoint.Y);
                                    }

                                    Point? pt = segment.Point;
                                    double t = 0;
                                    {
                                        if (IsVisible(_new.Value, connector.TargetPoint) || connector.KnownTargetPort != null)
                                        {
                                            (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _new.Value);
                                            (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, connector.TargetPoint);
                                            count += 1;
                                        }
                                        else
                                        {
                                            Point _top = new Point(connector.KnownTargetNode.Center.X, connector.KnownTargetNode.Bounds.Top);
                                            Point _second = new Point(_top.X, _top.Y - 10);
                                            if (IsVisible(_top, _second))
                                            {
                                                segment.Point = new Point(_top.X, segment.Point.Y);
                                                if (!IsVisible(prev.Point, segment.Point))
                                                    UpdateRouting(segment, prev);
                                                (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _top);
                                                connector.TargetPoint = _top;
                                                (connector as ConnectorWrapper).isRoutted = true;
                                                connector.UpdateGeometry();
                                                (connector.View as Connector).UpdateDecorator();
                                            }
                                            else
                                            {
                                                _top = new Point(connector.KnownTargetNode.Bounds.Right + 5, connector.KnownTargetNode.Bounds.Top - 5);
                                                Point _right = new Point(connector.KnownTargetNode.Bounds.Right, connector.KnownTargetNode.Center.Y);
                                                _second = new Point(_right.X + 10, _right.Y);
                                                if (IsVisible(_right, _second))
                                                {
                                                    segment.Point = new Point(_second.X, segment.Point.Y);
                                                    _new = new Point(segment.Point.X, _right.Y);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _second);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, _right);
                                                    connector.TargetPoint = _right;
                                                    if (!IsVisible(_second, segment.Point))
                                                    {
                                                        UpdateRouting(connector.InternalSegments[connector.InternalSegments.Count - 2] as LineSegment, segment);
                                                    }
                                                    (connector as ConnectorWrapper).isRoutted = true;
                                                    connector.UpdateGeometry();
                                                    (connector.View as Connector).UpdateDecorator();
                                                }
                                                else
                                                {
                                                    (connector as ConnectorWrapper).AddLineSegment(ref pt, ref t, _new.Value);
                                                    (connector as ConnectorWrapper).AddLineSegment(ref _new, ref t, connector.TargetPoint);
                                                    count += 1;
                                                }
                                            }
                                        }
                                    }


                                }



                                #endregion

                                #region First segment Routing

                                if (connector.InternalSegments.IndexOf(segment) == 0)
                                {

                                    Point? pt = segment.Point;
                                    Point old = segment.Point;
                                    double t = 0;
                                    Point? _new;
                                    Point next = (connector.InternalSegments[1] as LineSegment).Point;
                                    if (prev.Point.X < segment.Point.X)
                                    {
                                        segment.Point = new Point(connector.SourcePoint.X + 10, connector.SourcePoint.Y);
                                        _new = new Point(segment.Point.X, old.Y);
                                    }
                                    else
                                    {
                                        segment.Point = new Point(connector.SourcePoint.X - 10, connector.SourcePoint.Y);
                                        _new = new Point(segment.Point.X, old.Y);

                                    }
                                    Point _new1 = new Point(segment.Point.X,
                                                            (connector.InternalSegments[1] as LineSegment).Point.Y);
                                    if (connector.InternalSegments.Count > 3 && IsVisible(segment.Point, _new1) &&
                                        IsVisible(_new1, (connector.InternalSegments[1] as LineSegment).Point) && connector.InternalSegments.Count > 2)
                                    {
                                        (connector.InternalSegments[1] as LineSegment).Point = _new1;

                                    }
                                    else
                                    {

                                        (connector as ConnectorWrapper).InsertLineSegment(ref pt, ref t, _new.Value, 1);
                                        (connector as ConnectorWrapper).InsertLineSegment(ref _new, ref t, old, 2);
                                        count += 2;
                                    }

                                }

                                #endregion


                            }

                            #endregion

                            _isrouted = true;
                            return;
                        }
                    }

                }

            }

        }

        private void FindPointsforHsegRouting(IInternalNode topObstacle, IInternalNode bottomObstacle, ref Point prevTop, ref Point Top, ref Point prevBottom, ref Point Bottom, LineSegment segment, LineSegment prev)
        {
            if (topObstacle != null)
            {
                prevTop = new Point(prev.Point.X, topObstacle.Bounds.Top - 5);
                Top = new Point(segment.Point.X, topObstacle.Bounds.Top - 5);
            }
            if (bottomObstacle != null)
            {
                prevBottom = new Point(prev.Point.X, bottomObstacle.Bounds.Bottom + 5);
                Bottom = new Point(segment.Point.X, bottomObstacle.Bounds.Bottom + 5);
            }
            //throw new NotImplementedException();
        }

        private bool RouteHSegment(LineSegment prev, LineSegment segment, Point prevTop, Point Top, Point prevBottom, Point Bottom)
        {
            bool IsVisibleOnTop = IsVisible(prevTop, Top) && IsVisible(prev.Point, prevTop);
            bool IsVisibleOnBottom = IsVisible(prevBottom, Bottom) &&
                                     IsVisible(prev.Point, prevBottom);
            if (IsVisibleOnTop && IsVisibleOnBottom)
            {
                if (Math.Abs(prevTop.Y - connector.SourcePoint.Y) < Math.Abs((prevBottom.Y - connector.SourcePoint.Y)))
                {
                    prev.Point = prevTop;
                    segment.Point = Top;
                }
                else
                {
                    prev.Point = prevBottom;
                    segment.Point = Bottom;
                }
                return true;
            }
            else if (IsVisibleOnTop || IsVisibleOnBottom)
            {
                if (IsVisibleOnTop)
                {
                    IInternalNode bottomObstacle = FindObstacle(prevBottom, Bottom);
                    if (bottomObstacle != null)
                    {
                        prevBottom = new Point(prev.Point.X, bottomObstacle.Bounds.Bottom + 5);
                        Bottom = new Point(segment.Point.X, bottomObstacle.Bounds.Bottom + 5);


                        IsVisibleOnBottom = IsVisible(prevBottom, Bottom) &&
                                            IsVisible(prev.Point, prevBottom);
                    }
                    if (IsVisibleOnBottom &&
                        Math.Abs(prevTop.Y - connector.SourcePoint.Y) >
                        Math.Abs((prevBottom.Y - connector.SourcePoint.Y)))
                    {
                        prev.Point = prevBottom;
                        segment.Point = Bottom;
                    }
                    else
                    {
                        prev.Point = prevTop;
                        segment.Point = Top;
                    }

                }
                else
                {
                    IInternalNode topObstacle = FindObstacle(prevTop, Top);
                    if (topObstacle != null)
                    {
                        prevTop = new Point(prev.Point.X, topObstacle.Bounds.Top - 5);
                        Top = new Point(segment.Point.X, topObstacle.Bounds.Top - 5);


                        IsVisibleOnTop = IsVisible(prevTop, Top) &&
                                            IsVisible(prev.Point, prevTop);
                    }
                    if (IsVisibleOnTop &&
                        Math.Abs(prevTop.Y - connector.SourcePoint.Y) >
                        Math.Abs((prevBottom.Y - connector.SourcePoint.Y)))
                    {
                        prev.Point = prevTop;
                        segment.Point = Top;
                    }
                    else
                    {
                        prev.Point = prevBottom;
                        segment.Point = Bottom;
                    }

                }
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }

        private void FindPoints(IInternalNode leftObstacle, IInternalNode rightObstacle, ref Point prevleft, ref Point left, ref Point prevRight, ref Point Right, LineSegment segment, LineSegment prev)
        {
            if (leftObstacle != null)
            {
                prevleft = new Point(leftObstacle.Bounds.Left - 5, prev.Point.Y);
                left = new Point(leftObstacle.Bounds.Left - 5, segment.Point.Y);
            }
            if (rightObstacle != null)
            {
                prevRight = new Point(rightObstacle.Bounds.Right + 5, prev.Point.Y);
                Right = new Point(rightObstacle.Bounds.Right + 5, segment.Point.Y);
            }
            //throw new NotImplementedException();
        }

        private bool Route(LineSegment prev, LineSegment segment, Point prevleft, Point left, Point prevRight, Point Right)
        {

            bool IsVisibleOnLeft = IsVisible(prevleft, left) && IsVisible(prev.Point, prevleft);
            bool IsVisibleOnRight = IsVisible(prevRight, Right) && IsVisible(prev.Point, prevRight);
            if (IsVisibleOnLeft && IsVisibleOnRight)
            {

                if (Math.Abs(prevleft.X - connector.SourcePoint.X) < Math.Abs((prevRight.X - connector.SourcePoint.X)))
                {
                    prev.Point = prevleft;
                    segment.Point = left;
                }
                else
                {
                    prev.Point = prevRight;
                    segment.Point = Right;
                }
                return true;
            }
            else if (IsVisibleOnLeft || IsVisibleOnRight)
            {
                if (IsVisibleOnLeft)
                {

                    IInternalNode rightObstacle = FindObstacle(prevRight, Right);
                    if (rightObstacle != null)
                    {
                        prevRight = new Point(rightObstacle.Bounds.Right + 5, prev.Point.Y);
                        Right = new Point(rightObstacle.Bounds.Right + 5, segment.Point.Y);

                        IsVisibleOnRight = IsVisible(prevRight, Right) &&
                                           IsVisible(prev.Point, prevRight);
                    }
                    if (IsVisibleOnRight &&
                        Math.Abs(prevleft.X - connector.SourcePoint.X) >
                        Math.Abs((prevRight.X - connector.SourcePoint.X)))
                    {
                        prev.Point = prevRight;
                        segment.Point = Right;
                    }
                    else
                    {
                        prev.Point = prevleft;
                        segment.Point = left;
                    }

                }
                else
                {
                    IInternalNode leftObstacle = FindObstacle(prevleft, left);
                    if (leftObstacle != null)
                    {
                        prevleft = new Point(leftObstacle.Bounds.Left - 5, prev.Point.Y);
                        left = new Point(leftObstacle.Bounds.Left - 5, segment.Point.Y);
                        IsVisibleOnLeft = IsVisible(prevleft, left) &&
                                          IsVisible(prev.Point, prevleft);
                    }
                    if (IsVisibleOnLeft &&
                        Math.Abs(prevleft.X - connector.SourcePoint.X) <
                        Math.Abs((prevRight.X - connector.SourcePoint.X)))
                    {
                        prev.Point = prevleft;
                        segment.Point = left;
                    }
                    else
                    {
                        prev.Point = prevRight;
                        segment.Point = Right;
                    }
                }
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }

        private List<IInternalNode> FindObstacles(Point prevleft, Point left)
        {
            List<IInternalNode> nodes = new List<IInternalNode>();
            foreach (IInternalNode node in obstacles)
            {
                Rect r = new Rect(prevleft, left);
                if (node.Bounds.IsIntersect(ref r))
                {
                    nodes.Add(node);
                }
            }
            return nodes;
            //throw new NotImplementedException();
        }
        private void FindObstacles(Rect r, ref Rect r1)
        {
            foreach (IInternalNode node in obstacles)
            {
                if (node.Bounds.IsIntersect(ref r))
                {
                    r1.Union(node.Bounds);
                }

            }

            //throw new NotImplementedException();
        }
        private IInternalNode FindObstacle(Point prevleft, Point left)
        {
            foreach (IInternalNode node in obstacles)
            {
                Rect r = new Rect(prevleft, left);
                if (node.Bounds.IsIntersect(ref r))
                {
                    return node;
                }
            }
            return null;
            //throw new NotImplementedException();
        }

        private bool IsVisible(Point point1, Point point2)
        {
            foreach (IInternalNode node in obstacles)
            {
                Rect r = new Rect(point1, point2);
                if (node.Bounds.IsIntersect(ref r) && node != connector.KnownTargetNode && node != connector.KnownSourceNode)
                {
                    return false;
                }
            }
            return true;
            // throw new NotImplementedException();
        }

        List<IInternalNode> obstacles = new List<IInternalNode>();
        private void GetObstacleRegion()
        {
            //foreach (ILineSegment segment in this.Segments)
            {
                foreach (IInternalNode node in _mSharedData.Graph.InternalNodes)
                {
                    //Rect lineBounds=this.Bounds;
                    if (node != connector.KnownSourceNode && node != connector.KnownTargetNode)
                    {
                        obstacles.Add(node);
                    }
                }
            }
            //throw new NotImplementedException();
        }
    }
}
