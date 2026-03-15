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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Shapes;
#endif
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.Diagnostics;
using Syncfusion.UI.Xaml.Diagram.Controller;
using System.ComponentModel;

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class SnapSettingsWrapper : WrapperBase, IInternalSnapSettings
    {
        private List<Path> _mUnUsedLines;
        private List<Path> _mUsedLines;
        private double _mAdjustableDistance=5;

        public SnapSettingsWrapper(SnapSettings snapSettings, SharedData sharedData):base(sharedData)
        {
            Source = snapSettings;
            SharedData = sharedData;
            SharedData.GridLinePanel = new GridLinePanel();
            _mUsedLines = new List<Path>();
            _mUnUsedLines = new List<Path>();
        }

        internal void OnSnapSettingsChanged()
        {
            Source = SharedData.Graph.SnapSettings;
        }

        protected override void SharedDataInitialized()
        {
            //throw new NotImplementedException();
        }

        protected override void SourceChanged()
        {
            OnPropertyChanged(SnapSettingsConstants.HorizontalGridlines);
            OnPropertyChanged(SnapSettingsConstants.VerticalGridlines);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case SnapSettingsConstants.HorizontalGridlines:
                    if (_mHorizontalGridlines != null)
                    {
                        _mHorizontalGridlines.PropertyChanged -= Gridlines_PropertyChanged;
                    }
                    if (HorizontalGridlines != null)
                    {
                        _mHorizontalGridlines = HorizontalGridlines;
                        _mHorizontalGridlines.PropertyChanged += Gridlines_PropertyChanged;
                    }
                    else
                    {
                        _mHorizontalGridlines = new Gridlines();
                    }
                    
                    if (SharedData.ScrollViewer != null)
                        UpdateGridlines(SharedData.ScrollViewer.Viewport);
                    break;
                case SnapSettingsConstants.VerticalGridlines:
                    if (_mVerticalGridlines != null)
                    {
                        _mVerticalGridlines.PropertyChanged -= Gridlines_PropertyChanged;
                    }
                    if (VerticalGridlines != null)
                    {
                        _mVerticalGridlines = VerticalGridlines;
                        _mVerticalGridlines.PropertyChanged += Gridlines_PropertyChanged;
                    }
                    else
                    {
                        _mVerticalGridlines = new Gridlines();
                    }
                                   

                    if (SharedData.ScrollViewer != null)
                        UpdateGridlines(SharedData.ScrollViewer.Viewport);
                    break;
                case SnapSettingsConstants.SnapConstraints:
                    if (SnapConstraints.Contains(SnapConstraints.HorizontalLines) || SnapConstraints.Contains(SnapConstraints.VerticalLines))
                    {
                        if (SharedData.ScrollViewer != null)
                        {
                            UpdateGridlines(SharedData.ScrollViewer.Viewport);
                        }
                    }
                    else
                    {
                        ClearGridlines();
                    }
                    break;
            }
        }

        void Gridlines_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Strokes") || e.PropertyName.Equals("LinesInterval"))
            {
                if (SnapConstraints.Contains(SnapConstraints.HorizontalLines) ||
               SnapConstraints.Contains(SnapConstraints.VerticalLines))
                {
                    if (SharedData.Graph.ScrollInfo != null)
                    {
                        UpdateGridlines(SharedData.Graph.ScrollInfo.Viewport);
                    }
                }
            }
        }

        public bool CanSnapToHorizontalGridlines(ConnectorConstraints constraints)
        {
            if (constraints.Contains(ConnectorConstraints.InheritSnapping))
                return SnapConstraints.Contains(SnapConstraints.SnapToHorizontalLines);
            else
                return constraints.Contains(ConnectorConstraints.SnapToHorizontalLines);
        }

        public bool CanSnapToVerticalGridlines(ConnectorConstraints constraints)
        {
            if (constraints.Contains(ConnectorConstraints.InheritSnapping))
                return SnapConstraints.Contains(SnapConstraints.SnapToVerticalLines);
            else
                return constraints.Contains(ConnectorConstraints.SnapToVerticalLines);
        }

        public bool CanSnapToHorizontalGridlines(NodeConstraints constraints)
        {
            if (constraints.Contains(NodeConstraints.InheritSnapping))
                return SnapConstraints.Contains(SnapConstraints.SnapToHorizontalLines);
            else
                return constraints.Contains(NodeConstraints.SnapToHorizontalLines);
        }

        public bool CanSnapToVerticalGridlines(NodeConstraints constraints)
        {
            if (constraints.Contains(NodeConstraints.InheritSnapping))
                return SnapConstraints.Contains(SnapConstraints.SnapToVerticalLines);
            else
                return constraints.Contains(NodeConstraints.SnapToVerticalLines);
        }

        public bool CanSnapAngle(NodeConstraints constraints)
        {
            if (constraints.Contains(NodeConstraints.InheritSnapping))
                return SnapConstraints.Contains(SnapConstraints.Rotation);
            else
                return constraints.Contains(NodeConstraints.SnapAngle);
        }

        private bool CanCreateGuidelines(SnapToObject guidelines, IInternalNode node)
        {
            if (node.Constraints.Contains(NodeConstraints.InheritSnapToObject))
            {
                return SharedData.Graph.SnapSettings.SnapToObject.Contains(guidelines);
            }
            else
            {
                return node.SnapToObject.Contains(guidelines);
            }
        }

        public IEnumerable<double> GetVerticalSnapInterval()
        {
            return _mVerticalGridlines.SnapInterval;
        }

        public IEnumerable<double> GetHorizontalSnapInterval()
        {
            return _mHorizontalGridlines.SnapInterval;
        }

        #region Finding Targets of snapping

        internal void FindPossipleSnaps(IInternalNode node, Point _currentPosition, ref SnapParameter horizontalSnap, ref SnapParameter verticalSnap)
        {
            double delx = _currentPosition.X - node.OffsetX;
            double dely = _currentPosition.Y - node.OffsetY;
            Rect viewport = SharedData.ScrollViewer.Viewport;
            double currentzoom=SharedData.ScrollViewer.CurrentZoom;
            viewport = new Rect(viewport.X / currentzoom, viewport.Y / currentzoom, viewport.Width/currentzoom, viewport.Height/currentzoom);
            Rect horizontalRect = new Rect(viewport.X, node.Bounds.Top - 6, viewport.Width, node.Bounds.Height + 12);
            Rect verticalRect = new Rect(node.Bounds.Left - 6, viewport.Top, node.Bounds.Width + 12, viewport.Height);
            List<IInternalNode> verticallyIntersectedNodes = FindNodes(node, verticalRect, viewport);
            List<IInternalNode> horizontallyIntersectedNodes = FindNodes(node, horizontalRect, viewport);

            if (horizontalSnap == null)
            {
                horizontalSnap = GetPossibleHorizontalSnaps(verticallyIntersectedNodes, node, ref delx);
                if (horizontalSnap == null)
                {
                    if (CanCreateGuidelines(SnapToObject.HorizontalSpacing, node) && horizontallyIntersectedNodes.Count > 2)
                    {
                        if (horizontallyIntersectedNodes.Contains(node))
                            horizontalSnap = GetHorizontallySpacedNodes(node, horizontallyIntersectedNodes, ref delx);
                    }
                }
            }

            if (verticalSnap == null)
            {
                verticalSnap = GetPossibleVerticalSnaps(horizontallyIntersectedNodes, node, ref dely);
                if (verticalSnap == null)
                {
                    if (CanCreateGuidelines(SnapToObject.VerticalSpacing, node) && verticallyIntersectedNodes.Count > 2)
                    {
                        if (verticallyIntersectedNodes.Contains(node))
                            verticalSnap = GetVerticallySpacedNodes(node, verticallyIntersectedNodes, ref dely);
                    }
                }
            }
        }

        internal void FindTargetConnectors(IInternalNode node, double delx, double dely, ref List<SnapParameter> Snaps)
        {
            Rect bounds = node.Bounds;
            Rect target = new Rect(bounds.Left + delx - 6, bounds.Top + dely - 6, bounds.Width + 12, bounds.Height + 12);

            List<IInternalConnector> connectors = FindConnectors(node, target, SharedData.ScrollViewer.Viewport);
            if (connectors.Count > 0)
            {
                List<TargetConnector> targetConnectors = new List<TargetConnector>();
                foreach (IInternalConnector conn in connectors)
                {
                    if (conn.Segments != null && !conn.Segments.Any(seg => (!(seg is ILineSegment))) &&
                         conn.Segments.Count > 0 && (node.InOutConnectors == null || !node.InternalInOutConnectors.Contains(conn)))
                    {
                        Point st = conn.SourcePoint;
                        Point endPt = new Point(0, 0);
                        List<Point> nodepts = new List<Point>() { 
                            new Point(bounds.Left-5,bounds.Top-5), 
                            new Point(bounds.Right+5,bounds.Top-5),
                            new Point(bounds.Left-5,bounds.Bottom+5), 
                            new Point(bounds.Right+5,bounds.Bottom+5)};

                        foreach (IConnectorSegment segment in conn.Segments)
                        {
                            endPt = (segment as ILineSegment).Point ?? conn.TargetPoint;
                            List<Point> pts = new List<Point>() { st, endPt };
                            List<Point> intersectPoints = pts.Intersect(nodepts, false).ToList();
                            if (intersectPoints.Count > 0)
                            {
                                targetConnectors.Add(new TargetConnector(conn.View as Connector, segment, st, intersectPoints));
                            }
                            st = endPt;
                        }

                    }
                }
                if (targetConnectors.Count > 0)
                {
                    double? x = node.OffsetX + delx;
                    double? y = node.OffsetY + dely;
                    Snaps.Add(new SnapParameter(
                        SnapReason.Segment,
                        SnapChanges.X | SnapChanges.Y,
                        new SnapState(x, y, null, null, null),
                        new SnapState(null, null, null, null, null),
                        new SegmentSnapInfo(targetConnectors)));
                }
            }
        }

        private SnapParameter GetPossibleVerticalSnaps(List<IInternalNode> nodelist, IInternalNode node, ref double dely)
        {
            if (nodelist.Count > 0 && nodelist.Contains(node))
            {
                var nodes = from nod in nodelist
                            orderby Math.Abs(node.Center.X - nod.Center.X) + Math.Abs(node.Center.Y - nod.Center.Y)
                                ascending
                            where node != nod
                            select nod;
                foreach (IInternalNode nod in nodes)
                {
                    if (Math.Abs(node.Center.Y + dely - nod.Center.Y) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.HorizontalCenter, node))
                    {
                        double? current = node.OffsetY + dely;
                        double? proposed = node.OffsetY + nod.Center.Y - node.Center.Y;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.Y,
                            new SnapState(null, current, null, null, null),
                            new SnapState(null, proposed, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.HorizontalCenter));
                    }
                    else if (Math.Abs(node.Bounds.Top + dely - nod.Bounds.Top) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.TopTop, node))
                    {
                        double? current = node.OffsetY + dely;
                        double? proposed = node.OffsetY + nod.Bounds.Top - node.Bounds.Top;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.Y,
                            new SnapState(null, current, null, null, null),
                            new SnapState(null, proposed, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.TopTop));
                    }
                    else if (Math.Abs(node.Bounds.Top + dely - nod.Bounds.Bottom) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.TopBottom, node))
                    {
                        double? current = node.OffsetY + dely;
                        double? proposed = node.OffsetY + nod.Bounds.Bottom - node.Bounds.Top;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.Y,
                            new SnapState(null, current, null, null, null),
                            new SnapState(null, proposed, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.TopBottom));
                    }
                    else if (Math.Abs(node.Bounds.Bottom + dely - nod.Bounds.Bottom) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.BottomBottom, node))
                    {
                        double? current = node.OffsetY + dely;
                        double? proposed = node.OffsetY + nod.Bounds.Bottom - node.Bounds.Bottom;
                        return new SnapParameter(
                            SnapReason.Sides, SnapChanges.Y,
                            new SnapState(null, current, null, null, null),
                            new SnapState(null, proposed, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.BottomBottom));
                    }
                    else if (Math.Abs(node.Bounds.Bottom + dely - nod.Bounds.Top) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.BottomTop, node))
                    {
                        double? current = node.OffsetY + dely;
                        double? proposed = node.OffsetY + nod.Bounds.Top - node.Bounds.Bottom;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.Y,
                            new SnapState(null, current, null, null, null),
                            new SnapState(null, proposed, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.BottomTop));
                    }
                }
            }
            return null;
        }

        private SnapParameter GetPossibleHorizontalSnaps(List<IInternalNode> nodelist, IInternalNode node, ref double delx)
        {
            if (nodelist.Count > 0 && nodelist.Contains(node))
            {
                var nodes = from nod in nodelist
                            orderby Math.Abs(node.Center.Y - nod.Center.Y) + Math.Abs(node.Center.X - nod.Center.X)
                            ascending
                            where node != nod
                            select nod;

                foreach (IInternalNode nod in nodes)
                {
                    if (Math.Abs(node.Center.X + delx - nod.Center.X) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.VerticalCenter, node))
                    {
                        double? current = node.OffsetX + delx;
                        double? proposed = node.OffsetX + nod.Center.X - node.Center.X;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.X,
                            new SnapState(current, null, null, null, null),
                            new SnapState(proposed, null, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.VerticalCenter));
                    }
                    else if (Math.Abs(node.Bounds.Left + delx - nod.Bounds.Left) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.LeftLeft, node))
                    {
                        double? current = node.OffsetX + delx;
                        double? proposed = node.OffsetX + nod.Bounds.Left - node.Bounds.Left;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.X,
                            new SnapState(current, null, null, null, null),
                            new SnapState(proposed, null, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.LeftLeft));
                    }
                    else if (Math.Abs(node.Bounds.Left + delx - nod.Bounds.Right) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.LeftRight, node))
                    {
                        double? current = node.OffsetX + delx;
                        double? proposed = node.OffsetX + nod.Bounds.Right - node.Bounds.Left;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.X,
                            new SnapState(current, null, null, null, null),
                            new SnapState(proposed, null, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.LeftRight));
                    }
                    else if (Math.Abs(node.Bounds.Right + delx - nod.Bounds.Right) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.RightRight, node))
                    {
                        double? current = node.OffsetX + delx;
                        double? proposed = node.OffsetX + nod.Bounds.Right - node.Bounds.Right;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.X,
                            new SnapState(current, null, null, null, null),
                            new SnapState(proposed, null, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.RightRight));
                    }
                    else if (Math.Abs(node.Bounds.Right + delx - nod.Bounds.Left) <= _mAdjustableDistance
                        && CanCreateGuidelines(SnapToObject.RightLeft, node))
                    {
                        double? current = node.OffsetX + delx;
                        double? proposed = node.OffsetX + nod.Bounds.Left - node.Bounds.Right;
                        return new SnapParameter(
                            SnapReason.Sides,
                            SnapChanges.X,
                            new SnapState(current, null, null, null, null),
                            new SnapState(proposed, null, null, null, null),
                            new ObjectSnapInfo(nod.Source, SnapToObject.RightLeft));
                    }
                }
            }
            return null;
        }

        internal SnapParameter GetPossibleSnapsOnResizing(IInternalNode node, double? value, Side side)
        {
            Rect viewport = SharedData.ScrollViewer.Viewport;
            double currentzoom = SharedData.ScrollViewer.CurrentZoom;
            viewport = new Rect(viewport.X / currentzoom, viewport.Y / currentzoom, viewport.Width/currentzoom, viewport.Height/currentzoom);
            if (side == Side.Left || side == Side.Right)
            {
                Rect r = new Rect(new Point(node.Bounds.Left, viewport.Top),
                    new Point(node.Bounds.Right, viewport.Bottom));

                List<IInternalNode> nodesinview = new List<IInternalNode>();
                List<IInternalNode> nodelist = FindNodes(node, r, nodesinview, viewport);

                if (nodelist != null && nodelist.Count() > 1)
                {
                    if (side == Side.Left && CanCreateGuidelines(SnapToObject.LeftLeft,node))
                    {

                        double left = node.Bounds.Right - value.Value;
                        var nodes = from nod in nodelist
                                    orderby Math.Abs(left - nod.Bounds.Left) + Math.Abs(node.Center.Y - nod.Center.Y)
                                        ascending
                                    where node != nod
                                    select nod;
                        foreach (IInternalNode nod in nodes)
                        {
                            if (Math.Abs(left - nod.Bounds.Left) <= _mAdjustableDistance && node != nod)
                            {
                                double? width = value.Value - (nod.Bounds.Left - left);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Width,
                                    new SnapState(null, null, value, null, null),
                                    new SnapState(null, null, width, null, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.LeftLeft));
                            }
                            else if (Math.Abs(left - nod.Bounds.Right) <= _mAdjustableDistance && node != nod)
                            {
                                double? width = value.Value - (nod.Bounds.Right - left);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Width,
                                    new SnapState(null, null, value, null, null),
                                    new SnapState(null, null, width, null, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.LeftRight));
                            }
                        }
                    }
                    else if( CanCreateGuidelines(SnapToObject.RightRight,node))
                    {
                        double right = node.Bounds.Left + value.Value;
                        var nodes = from nod in nodelist
                                    orderby Math.Abs(right - nod.Bounds.Right) + Math.Abs(node.Center.Y - nod.Center.Y)
                                        ascending
                                    where node != nod
                                    select nod;
                        foreach (IInternalNode nod in nodes)
                        {
                            if (Math.Abs(right - nod.Bounds.Right) <= _mAdjustableDistance && node != nod)
                            {
                                double? width = value.Value + (nod.Bounds.Right - right);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Width, new SnapState(null, null, value, null, null),
                                    new SnapState(null, null, width, null, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.RightRight));
                            }
                            else if (Math.Abs(right - nod.Bounds.Left) <= _mAdjustableDistance && node != nod)
                            {
                                double? width = value.Value + (nod.Bounds.Left - right);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size, SnapChanges.Width,
                                    new SnapState(null, null, value, null, null),
                                    new SnapState(null, null, width, null, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.RightLeft));
                            }
                        }

                    }

                }
                return FindSameSizeObjects(nodesinview, side, value, node);
            }
            else if (side == Side.Top || side == Side.Bottom)
            {
                Rect r = new Rect(new Point(viewport.Left, node.Bounds.Top), new Point(viewport.Right, node.Bounds.Bottom));
                List<IInternalNode> nodesinview = new List<IInternalNode>();
                List<IInternalNode> nodelist = FindNodes(node, r, nodesinview, viewport);

                if (nodelist != null && nodelist.Count() > 1)
                {
                    if (side == Side.Top && CanCreateGuidelines(SnapToObject.TopTop, node))
                    {
                        double top = node.Bounds.Bottom - value.Value;
                        var nodes = from nod in nodelist
                                    orderby Math.Abs(node.Center.X - nod.Center.X) + Math.Abs(node.Center.Y - nod.Center.Y)
                                        ascending
                                    where node != nod
                                    select nod;
                        foreach (IInternalNode nod in nodes)
                        {
                            if (Math.Abs(top - nod.Bounds.Top) <= _mAdjustableDistance && node != nod)
                            {
                                double? height = value.Value - (nod.Bounds.Top - top);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Height,
                                    new SnapState(null, null, null, value, null),
                                    new SnapState(null, null, null, height, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.TopTop));
                            }
                            else if (Math.Abs(top - nod.Bounds.Bottom) <= _mAdjustableDistance && node != nod)
                            {
                                double? height = value.Value - (nod.Bounds.Bottom - top);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Height,
                                    new SnapState(null, null, null, value, null),
                                    new SnapState(null, null, null, height, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.TopBottom));
                            }
                        }
                    }
                    else if(CanCreateGuidelines(SnapToObject.BottomBottom,node))
                    {
                        double bottom = node.Bounds.Top + value.Value;
                        var nodes = from nod in nodelist
                                    orderby Math.Abs(node.Center.X - nod.Center.X) + Math.Abs(node.Center.Y - nod.Center.Y)
                                        ascending
                                    where node != nod
                                    select nod;
                        foreach (IInternalNode nod in nodes)
                        {
                            if (Math.Abs(bottom - nod.Bounds.Bottom) <= _mAdjustableDistance && node != nod)
                            {
                                double? height = value.Value + (nod.Bounds.Bottom - bottom);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Height,
                                    new SnapState(null, null, null, value, null),
                                    new SnapState(null, null, null, height, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.BottomBottom));
                            }
                            else if (Math.Abs(bottom - nod.Bounds.Top) <= _mAdjustableDistance && node != nod)
                            {
                                double? height = value.Value + (nod.Bounds.Top - bottom);
                                return new SnapParameter(
                                    SnapReason.Sides | SnapReason.Size,
                                    SnapChanges.Height,
                                    new SnapState(null, null, null, value, null),
                                    new SnapState(null, null, null, height, null),
                                    new ObjectSnapInfo(nod.Source, SnapToObject.BottomTop));
                            }
                        }
                    }


                }
                return FindSameSizeObjects(nodesinview, side, value, node);
            }
            return null;
        }

        private SnapParameter FindSameSizeObjects(List<IInternalNode> nodesinview, Side side, double? value, IInternalNode node)
        {
            if ((side == Side.Left || side == Side.Right) && CanCreateGuidelines(SnapToObject.Width, node))
            {
                double nodewidth = value.Value;
                nodesinview = (from nod in nodesinview
                               orderby (Math.Abs(node.Center.X - nod.Center.X) + Math.Abs(node.Center.Y - nod.Center.Y))
                               where node != nod &&
                                   Math.Abs(nodewidth - nod.Bounds.Width) <= _mAdjustableDistance
                               select nod).ToList();

                if (nodesinview.Count > 0)
                {
                    List<object> targets = new List<object>();
                    double? width = nodesinview[0].Bounds.Width;
                    foreach (IInternalNode nod in nodesinview)
                    {
                        targets.Add(nod.Source);
                    }
                    if (side == Side.Left)
                        return new SnapParameter(
                            SnapReason.Size,
                            SnapChanges.Width,
                            new SnapState(null, null, value, null, null),
                            new SnapState(null, null, width, null, null),
                            new SameSizeSnapInfo(targets, SnapToObject.Width | SnapToObject.Left));
                    else
                        return new SnapParameter(
                            SnapReason.Size,
                            SnapChanges.Width,
                            new SnapState(null, null, value, null, null),
                            new SnapState(null, null, width, null, null),
                            new SameSizeSnapInfo(targets, SnapToObject.Width | SnapToObject.Right));

                }
            }
            else if ((side == Side.Top | side == Side.Bottom) && CanCreateGuidelines(SnapToObject.Height, node))
            {
                double heigh = value.Value;
                nodesinview = (from nod in nodesinview
                               orderby (Math.Abs(node.Center.X - nod.Center.X) + Math.Abs(node.Center.Y - nod.Center.Y))
                               where node != nod &&
                                   Math.Abs(heigh - nod.Bounds.Height) <= _mAdjustableDistance
                               select nod).ToList();
                if (nodesinview.Count > 0)
                {
                    double? height = nodesinview[0].Bounds.Height;
                    List<object> targets = new List<object>();
                    foreach (IInternalNode nod in nodesinview)
                    {
                        targets.Add(nod.Source);
                    }

                    if (side == Side.Top)
                        return new SnapParameter(
                            SnapReason.Size,
                            SnapChanges.Height,
                            new SnapState(null, null, null, value, null),
                            new SnapState(null, null, null, height, null),
                            new SameSizeSnapInfo(targets, SnapToObject.Height | SnapToObject.Top));
                    else
                        return new SnapParameter(
                            SnapReason.Size,
                            SnapChanges.Height,
                            new SnapState(null, null, null, value, null),
                            new SnapState(null, null, null, height, null),
                            new SameSizeSnapInfo(targets, SnapToObject.Height | SnapToObject.Bottom));
                }
            }
            return null;
        }

        private SnapParameter GetVerticallySpacedNodes(IInternalNode node, List<IInternalNode> lst, ref double dely)
        {
            lst = (from nod in lst orderby nod.OffsetY ascending select nod).ToList();
            int index = lst.IndexOf(node);
            double distInBottom = 0;
            double distInTop = 0;
            double actualdistInTop = 0;
            double actualdistInBottom = 0;
            List<object> equallySpacedObjects = new List<object>();
            if (index < lst.Count - 2)
                distInBottom = lst[index + 2].Bounds.Top - lst[index + 1].Bounds.Bottom;
            if (index > 1)
                distInTop = lst[index - 1].Bounds.Top - lst[index - 2].Bounds.Bottom;
            if (index < lst.Count - 1)
                actualdistInBottom = lst[index + 1].Bounds.Top - node.Bounds.Bottom - dely;
            if (index > 0)
                actualdistInTop = node.Bounds.Top + dely - lst[index - 1].Bounds.Bottom;
            if (Math.Abs(actualdistInTop - actualdistInBottom) <= _mAdjustableDistance && actualdistInBottom >= 10)
            {
                FindEquallySpacedObjectsAtTop(lst, actualdistInTop, index, ref equallySpacedObjects);
                equallySpacedObjects.Add(node.Source);
                FindEquallySpacedObjectsAtBottom(lst, actualdistInBottom, index, ref equallySpacedObjects);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentValue = node.OffsetY + dely;
                    double? newValue = node.OffsetY + (actualdistInBottom - actualdistInTop) / 2 + dely;
                    if (actualdistInTop < actualdistInBottom)
                    {
                        return new SnapParameter(
                            SnapReason.EqualSpace,
                            SnapChanges.Y,
                            new SnapState(null, currentValue, null, null, null),
                            new SnapState(null, newValue, null, null, null),
                            new EqualSpaceSnapInfo(lst[index - 1].Source,
                                SnapToObject.VerticalSpacing, actualdistInTop + (actualdistInBottom - actualdistInTop) / 2, equallySpacedObjects));
                    }
                    else
                    {
                        return new SnapParameter(
                            SnapReason.EqualSpace,
                            SnapChanges.Y,
                            new SnapState(null, currentValue, null, null, null),
                            new SnapState(null, newValue, null, null, null),
                            new EqualSpaceSnapInfo(lst[index + 1].Source,
                                SnapToObject.VerticalSpacing, actualdistInTop + (actualdistInBottom - actualdistInTop) / 2, equallySpacedObjects));
                    }
                }
            }
            else if (Math.Abs(actualdistInTop - distInTop) <= _mAdjustableDistance && distInTop > 0)
            {
                FindEquallySpacedObjectsAtTop(lst, distInTop, index, ref equallySpacedObjects);
                equallySpacedObjects.Add(node.Source);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentValue = node.OffsetY + dely;
                    double? newValue = node.OffsetY + distInTop - actualdistInTop + dely;
                    return new SnapParameter(
                        SnapReason.EqualSpace,
                        SnapChanges.Y,
                        new SnapState(null, currentValue, null, null, null),
                        new SnapState(null, newValue, null, null, null),
                        new EqualSpaceSnapInfo(lst[index - 1].Source, SnapToObject.VerticalSpacing, distInTop, equallySpacedObjects));
                }
            }
            else if (Math.Abs(actualdistInBottom - distInBottom) <= _mAdjustableDistance && distInBottom > 0)
            {
                equallySpacedObjects.Add(node.Source);
                FindEquallySpacedObjectsAtBottom(lst, distInBottom, index, ref equallySpacedObjects);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentValue = node.OffsetY + dely;
                    double? newValue = node.OffsetY + actualdistInBottom - distInBottom + dely;
                    return new SnapParameter(SnapReason.EqualSpace,
                        SnapChanges.Y,
                        new SnapState(null, currentValue, null, null, null),
                        new SnapState(null, newValue, null, null, null),
                        new EqualSpaceSnapInfo(lst[index + 1].Source, SnapToObject.VerticalSpacing, distInBottom, equallySpacedObjects));
                }
            }
            return null;
        }

        private void FindEquallySpacedObjectsAtTop(List<IInternalNode> lst, double distInTop, int index, ref List<object> equallySpacedObjects)
        {
            if (distInTop > 10)
            {
                for (int i = index; i > 0; i--)
                {
                    if (i == index)
                    {
                        equallySpacedObjects.Insert(0, lst[i - 1].Source);
                    }
                    else if ((int)(lst[i].Bounds.Top - lst[i - 1].Bounds.Bottom) == (int)distInTop)
                    {
                        equallySpacedObjects.Insert(0, lst[i - 1].Source);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void FindEquallySpacedObjectsAtBottom(List<IInternalNode> lst, double distInBottom, int index, ref List<object> equallySpacedObjects)
        {

            if (distInBottom > 10)
            {
                for (int i = index; i < lst.Count - 1; i++)
                {
                    if (i == index)
                    {
                        equallySpacedObjects.Add(lst[i + 1].Source);
                    }
                    else if ((lst[i + 1].Bounds.Top - lst[i].Bounds.Bottom) == distInBottom)
                    {
                        equallySpacedObjects.Add(lst[i + 1].Source);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private SnapParameter GetHorizontallySpacedNodes(IInternalNode node, List<IInternalNode> lst, ref double delx)
        {
            lst = (from nod in lst orderby nod.Bounds.Left ascending select nod).ToList();
            List<object> equallySpacedObjects = new List<object>();
            int index = lst.IndexOf(node);
            double distInRight = 0;
            double distInLeft = 0;
            double actualdistanceinRight = 0;
            double actualDistanceinLeft = 0;

            if (index < lst.Count - 2)
            {
                distInRight = lst[index + 2].Bounds.Left - lst[index + 1].Bounds.Right;
            }
            if (index > 1)
            {
                distInLeft = lst[index - 1].Bounds.Left - lst[index - 2].Bounds.Right;
            }

            if (index < lst.Count - 1)
                actualdistanceinRight = lst[index + 1].Bounds.Left - node.Bounds.Right - delx;
            if (index > 0)
                actualDistanceinLeft = node.Bounds.Left + delx - lst[index - 1].Bounds.Right;

            if (Math.Abs(actualDistanceinLeft - actualdistanceinRight) <= _mAdjustableDistance && actualDistanceinLeft > 0 && actualdistanceinRight > 0)
            {
                FindEquallySpacedObjectsAtRight(lst, actualDistanceinLeft, index, ref equallySpacedObjects);
                equallySpacedObjects.Add(node.Source);
                FindEquallySpacedObjectsAtLeft(lst, actualdistanceinRight, index, ref equallySpacedObjects);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentValue = node.OffsetX + delx;
                    double? newValue = node.OffsetX + (actualdistanceinRight - actualDistanceinLeft) / 2 + delx;
                    if (actualDistanceinLeft < actualdistanceinRight)
                    {
                        return new SnapParameter(
                            SnapReason.EqualSpace,
                            SnapChanges.X,
                            new SnapState(currentValue, null, null, null, null),
                            new SnapState(newValue, null, null, null, null),
                            new EqualSpaceSnapInfo(lst[index - 1].Source, SnapToObject.HorizontalSpacing,
                                actualDistanceinLeft + (actualdistanceinRight - actualDistanceinLeft) / 2, equallySpacedObjects));

                    }
                    else
                    {
                        return new SnapParameter(
                            SnapReason.EqualSpace,
                            SnapChanges.X,
                            new SnapState(currentValue, null, null, null, null),
                            new SnapState(newValue, null, null, null, null),
                            new EqualSpaceSnapInfo(lst[index + 1].Source, SnapToObject.HorizontalSpacing,
                                actualDistanceinLeft + (actualdistanceinRight - actualDistanceinLeft) / 2, equallySpacedObjects));
                    }
                }
            }
            else if (Math.Abs(actualdistanceinRight - distInRight) <= _mAdjustableDistance && distInRight > 0)
            {
                equallySpacedObjects.Add(node.Source);
                FindEquallySpacedObjectsAtLeft(lst, distInRight, index, ref equallySpacedObjects);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentPosition = node.OffsetX + delx;
                    double? newPosition = node.OffsetX + actualdistanceinRight - distInRight + delx;
                    return new SnapParameter(
                        SnapReason.EqualSpace,
                        SnapChanges.X,
                        new SnapState(currentPosition, null, null, null, null),
                        new SnapState(newPosition, null, null, null, null),
                        new EqualSpaceSnapInfo(lst[index + 1].Source, SnapToObject.HorizontalSpacing, distInRight, equallySpacedObjects));
                }
            }
            else if (Math.Abs(actualDistanceinLeft - distInLeft) <= _mAdjustableDistance && distInLeft > 0)
            {
                FindEquallySpacedObjectsAtRight(lst, distInLeft, index, ref equallySpacedObjects);
                equallySpacedObjects.Add(node.Source);
                if (equallySpacedObjects.Count > 2)
                {
                    double? currentPosition = node.OffsetX + delx;
                    double? newPosition = node.OffsetX + distInLeft - actualDistanceinLeft + delx;
                    return new SnapParameter(
                        SnapReason.EqualSpace, SnapChanges.X,
                        new SnapState(currentPosition, null, null, null, null),
                        new SnapState(newPosition, null, null, null, null),
                        new EqualSpaceSnapInfo(lst[index - 1].Source, SnapToObject.HorizontalSpacing, distInLeft, equallySpacedObjects));
                }
            }


            return null;
        }

        private void FindEquallySpacedObjectsAtLeft(List<IInternalNode> lst, double distInRight, int index, ref List<object> equallySpacedObjects)
        {
            if (distInRight > 10)
            {
                for (int i = index; i < lst.Count - 1; i++)
                {
                    if (i == index)
                    {
                        equallySpacedObjects.Add(lst[i + 1].Source);
                    }
                    else if ((int)Math.Abs(lst[i + 1].Bounds.Left - lst[i].Bounds.Right) == (int)distInRight)
                    {
                        equallySpacedObjects.Add(lst[i + 1].Source);
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }

        private void FindEquallySpacedObjectsAtRight(List<IInternalNode> lst, double distInLeft, int index, ref List<object> equallySpacedObjects)
        {
            if (distInLeft > 10)
            {
                for (int i = index; i > 0; i--)
                {
                    if (i == index)
                    {
                        equallySpacedObjects.Insert(0, lst[i - 1].Source);
                    }
                    else if ((int)Math.Abs(lst[i].Bounds.Left - lst[i - 1].Bounds.Right) == (int)distInLeft)
                    {
                        equallySpacedObjects.Insert(0, lst[i - 1].Source);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private List<IInternalNode> FindNodes(IInternalNode node, Rect child, Rect viewport)
        {
            List<IInternalNode> nodelist = new List<IInternalNode>();
            List<Quad> quads = SharedData.SpatialSearch.FindQuads(viewport);
            foreach (var quad in quads)
            {
                if (quad.objects.Count > 0)
                {
                    foreach (IInternalGroupable nd in quad.objects)
                    {
                        if (nd is IInternalNode && !(nd is ISelector))
                        {
                            Rect nod = nd.Bounds;
                            if (!nodelist.Contains(nd as IInternalNode) && child.IsIntersect(ref nod))
                                nodelist.Add(nd as IInternalNode);
                        }
                    }
                }
            }
            return nodelist;
        }

        private List<IInternalNode> FindNodes(IInternalNode node, Rect child, List<IInternalNode> nodesinView, Rect viewport)
        {
            List<IInternalNode> nodelist = new List<IInternalNode>();
            List<Quad> quads = SharedData.SpatialSearch.FindQuads(viewport);
            foreach (var quad in quads)
            {
                if (quad.objects.Count > 0)
                {
                    foreach (IInternalGroupable nd in quad.objects)
                    {
                        if (nd is IInternalNode && !(nd is ISelector))
                        {
                            Rect nod = nd.Bounds;
                            if (!nodelist.Contains(nd as IInternalNode) && child.IsIntersect(ref nod))
                            {
                                nodelist.Add(nd as IInternalNode);
                            }
                            if (!nodesinView.Contains(nd as IInternalNode) && viewport.IsIntersect(ref nod))
                            {
                                nodesinView.Add(nd as IInternalNode);
                            }
                        }
                    }
                }
            }
            return nodelist;
        }

        private List<IInternalConnector> FindConnectors(IInternalNode node, Rect target, Rect viewport)
        {
            List<IInternalConnector> connectors = new List<IInternalConnector>();
            List<Quad> quads = SharedData.SpatialSearch.FindQuads(viewport);
            foreach (var quad in quads)
            {
                if (quad.objects.Count > 0)
                {
                    foreach (IInternalGroupable nd in quad.objects)
                    {
                        if (nd is IInternalConnector)
                        {
                            Rect nod = nd.Bounds;
                            if (!connectors.Contains(nd as IInternalConnector) && target.IsIntersect(ref nod))
                            {
                                connectors.Add(nd as IInternalConnector);
                            }
                        }
                    }
                }
            }
            return connectors;
        }


        #endregion

        # region Snapping

        internal void SnapSize(List<SnapParameter> snaps, IInternalNode node, ref double? _newWidth, ref double? _newHeight)
        {
            bool SnapAcceptedd = true;
            (node.View as Node).OnSnap(snaps, out SnapAcceptedd);
            if (SnapAcceptedd)
            {
                foreach (SnapParameter snap in snaps)
                {
                    switch (snap.SnapReason)
                    {
                        case SnapReason.Size:
                            CreateSameSizeLines(snap, ref _newWidth, ref _newHeight, node);
                            break;
                        case SnapReason.Sides | SnapReason.Size:
                            CreateGuidelineOnResizing(snap, node, ref _newWidth, ref _newHeight);
                            break;
                        case SnapReason.GridLine | SnapReason.Size:
                            SnapSize(snap, node, ref _newWidth, ref _newHeight);
                            break;
                    }

                }
            }
        }

        internal void SnapPosition(List<SnapParameter> snaps, IInternalNode node)
        {
            bool SnapAcceptedd = true;
            if (node.View != null)
                (node.View as Node).OnSnap(snaps, out SnapAcceptedd);
            if (SnapAcceptedd)
            {
                foreach (SnapParameter snap in snaps)
                {
                    switch (snap.SnapReason)
                    {
                        case SnapReason.GridLine:
                            SnapPosition(snap, node);
                            break;
                        case SnapReason.Sides:
                            CreateGuidelinesOnDragging(snap, node);
                            break;
                        case SnapReason.EqualSpace:
                            CreateSpacingLines(snap, node);
                            break;
                        case SnapReason.Segment:
                            break;
                        case SnapReason.Angle:
                            break;
                    }

                }
            }
        }

        private void SnapPosition(SnapParameter snap, IInternalNode node)
        {
            switch (snap.SnapChanges)
            {
                case SnapChanges.X:
                    node.OffsetX = snap.Proposed.X.Value;
                    break;
                case SnapChanges.Y:
                    node.OffsetY = snap.Proposed.Y.Value;
                    break;
            }
        }

        private void SnapSize(SnapParameter snap, IInternalNode node, ref double? _newWidth, ref double? _newHeight)
        {
            switch (snap.SnapChanges)
            {
                case SnapChanges.Width:
                    _newWidth = snap.Proposed.Width;
                    break;
                case SnapChanges.Height:
                    _newHeight = snap.Proposed.Height;
                    break;

            }
        }

        internal void SnapRotationAngle(List<SnapParameter> snaps, IInternalNode node)
        {
            bool SnapAcceptedd = true;
            (node.View as Node).OnSnap(snaps, out SnapAcceptedd);
            if (SnapAcceptedd)
            {
                foreach (SnapParameter snap in snaps)
                {
                    if (snap.SnapChanges == SnapChanges.Angle)
                    {
                        node.RotateAngle = snap.Proposed.Angle.Value;
                    }
                }
            }
        }

        internal void SnapToInOutNeighbours(IInternalNode node, Point currentPosition, ref SnapParameter horizontalSnap, ref SnapParameter verticalSnap)
        {
            double x = currentPosition.X - node.OffsetX;
            double y = currentPosition.Y - node.OffsetY;
            if (node.InternalOutConnectors != null && node.InternalOutConnectors.Count() > 0)
            {
                foreach (IInternalConnector conn in node.InternalOutConnectors)
                {
                    if (Math.Abs(conn.SourcePoint.X + x - conn.TargetPoint.X) <= 5)
                    {
                        node.OffsetX += conn.TargetPoint.X - conn.SourcePoint.X;
                        horizontalSnap = new SnapParameter();
                    }
                    else if (Math.Abs(conn.SourcePoint.Y + y - conn.TargetPoint.Y) <= 5)
                    {
                        node.OffsetY += conn.TargetPoint.Y - conn.SourcePoint.Y;
                        verticalSnap = new SnapParameter();
                    }
                }
            }
            else if (node.InternalInConnectors != null && node.InternalInConnectors.Count() > 0)
            {
                foreach (IInternalConnector conn in node.InternalInConnectors)
                {
                    if (Math.Abs(conn.TargetPoint.X + x - conn.SourcePoint.X) <= 5)
                    {
                        node.OffsetX += conn.SourcePoint.X - conn.TargetPoint.X;
                        horizontalSnap = new SnapParameter();
                    }
                    else if (Math.Abs(conn.TargetPoint.Y + y - conn.SourcePoint.Y) <= 5)
                    {
                        node.OffsetY += conn.SourcePoint.Y - conn.TargetPoint.Y;
                        verticalSnap = new SnapParameter();
                    }
                }
            }
        }

        private void CreateGuidelinesOnDragging(SnapParameter snap, IInternalNode node)
        {
            ObjectSnapInfo info = snap.SnapInfo as ObjectSnapInfo;
            IInternalNode target = null;
            if (info.Target is IGroup)
            {
                target = SharedData.Graph.GetGroupWrapper(info.Target, false);
            }
            else if (info.Target is INode)
            {
                target = SharedData.Graph.GetNodeWrapper(info.Target, false);
            }
            if (target != null)
            {
                switch (info.SnapToObject)
                {
                    case SnapToObject.VerticalCenter:
                        node.OffsetX = snap.Proposed.X.Value;
                        DrawGuideLines(new Point(node.Center.X, Math.Min(node.Bounds.Top, target.Bounds.Top)), new Point(node.Center.X, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        if ((int)node.Bounds.Left == (int)target.Bounds.Left)
                        {
                            DrawGuideLines(new Point(node.Bounds.Left, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                           new Point(node.Bounds.Left, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        }
                        if ((int)node.Bounds.Right == (int)target.Bounds.Right)
                        {
                            DrawGuideLines(new Point(node.Bounds.Right, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                           new Point(node.Bounds.Right, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        }
                        break;
                    case SnapToObject.LeftLeft:
                        node.OffsetX = snap.Proposed.X.Value;
                        DrawGuideLines(new Point(node.Bounds.Left, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(node.Bounds.Left, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.LeftRight:
                        node.OffsetX = snap.Proposed.X.Value;
                        DrawGuideLines(new Point(node.Bounds.Left, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(node.Bounds.Left, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.RightRight:
                        node.OffsetX = snap.Proposed.X.Value;
                        DrawGuideLines(new Point(node.Bounds.Right, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(node.Bounds.Right, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.RightLeft:
                        node.OffsetX = snap.Proposed.X.Value;
                        DrawGuideLines(new Point(node.Bounds.Right, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(node.Bounds.Right, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.HorizontalCenter:
                        node.OffsetY = snap.Proposed.Y.Value;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Center.Y),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Center.Y));
                        if ((int)node.Bounds.Top == (int)target.Bounds.Top)
                        {
                            DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Top),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Top));
                        }
                        if ((int)node.Bounds.Bottom == (int)target.Bounds.Bottom)
                        {
                            DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Bottom),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Bottom));
                        }
                        break;
                    case SnapToObject.TopTop:
                        node.OffsetY = snap.Proposed.Y.Value;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Top),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Top));
                        break;
                    case SnapToObject.TopBottom:
                        node.OffsetY = snap.Proposed.Y.Value;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Top),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Top));
                        break;
                    case SnapToObject.BottomBottom:
                        node.OffsetY = snap.Proposed.Y.Value;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Bottom),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Bottom));
                        break;
                    case SnapToObject.BottomTop:
                        node.OffsetY = snap.Proposed.Y.Value;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), node.Bounds.Bottom),
                            new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), node.Bounds.Bottom));
                        break;
                }
            }
            //throw new NotImplementedException();
        }

        private void CreateGuidelineOnResizing(SnapParameter snap, IInternalNode node, ref double? _newWidth, ref double? _newHeight)
        {
            ObjectSnapInfo info = snap.SnapInfo as ObjectSnapInfo;
            IInternalNode target = null;
            if (info.Target is IGroup)
            {
                target = SharedData.Graph.GetGroupWrapper(info.Target, false);
            }
            else if (info.Target is INode)
            {
                target = SharedData.Graph.GetNodeWrapper(info.Target, false);
            }
            if (target != null)
            {
                switch (info.SnapToObject)
                {
                    case SnapToObject.LeftLeft:
                        _newWidth = snap.Proposed.Width;
                        DrawGuideLines(new Point(target.Bounds.Left, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(target.Bounds.Left, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.LeftRight:
                        _newWidth = snap.Proposed.Width;
                        DrawGuideLines(new Point(target.Bounds.Right, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(target.Bounds.Right, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.RightRight:
                        _newWidth = snap.Proposed.Width;
                        DrawGuideLines(new Point(target.Bounds.Right, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(target.Bounds.Right, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.RightLeft:
                        _newWidth = snap.Proposed.Width;
                        DrawGuideLines(new Point(target.Bounds.Left, Math.Min(node.Bounds.Top, target.Bounds.Top)),
                            new Point(target.Bounds.Left, Math.Max(node.Bounds.Bottom, target.Bounds.Bottom)));
                        break;
                    case SnapToObject.TopTop:
                        _newHeight = snap.Proposed.Height;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), target.Bounds.Top),
                        new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), target.Bounds.Top));
                        break;
                    case SnapToObject.TopBottom:
                        _newHeight = snap.Proposed.Height;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), target.Bounds.Bottom),
                        new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), target.Bounds.Bottom));
                        break;
                    case SnapToObject.BottomBottom:
                        _newHeight = snap.Proposed.Height;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), target.Bounds.Bottom),
                        new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), target.Bounds.Bottom));
                        break;
                    case SnapToObject.BottomTop:
                        _newHeight = snap.Proposed.Height;
                        DrawGuideLines(new Point(Math.Min(node.Bounds.Left, target.Bounds.Left), target.Bounds.Top),
                        new Point(Math.Max(node.Bounds.Right, target.Bounds.Right), target.Bounds.Top));
                        break;
                    case SnapToObject.Width:
                        CreateSameSizeLines(snap, ref _newWidth, ref _newHeight, node);
                        break;
                    case SnapToObject.Height:
                        CreateSameSizeLines(snap, ref _newWidth, ref _newHeight, node);
                        break;
                }
            }
        }

        private void CreateSameSizeLines(SnapParameter snap, ref double? _newWidth, ref double? _newHeight, IInternalNode node)
        {
            SameSizeSnapInfo snaptonode = snap.SnapInfo as SameSizeSnapInfo;
            if (snaptonode.SnapToObject.Contains(SnapToObject.Width))
            {
                _newWidth = snap.Proposed.Width;
                if (snaptonode.SnapToObject.Contains(SnapToObject.Left))
                {
                    DrawHeadedLine(new Point(node.Bounds.Right - _newWidth.Value, node.Bounds.Top - 5), new Point(node.Bounds.Right, node.Bounds.Top - 5));
                }
                else
                {
                    DrawHeadedLine(new Point(node.Bounds.Left, node.Bounds.Top - 5), new Point(node.Bounds.Left + _newWidth.Value, node.Bounds.Top - 5));
                }
                foreach (object obj in snaptonode.SameSizeObjects)
                {
                    IInternalNode target = null;
                    if (obj is IGroup)
                    {
                        target = SharedData.Graph.GetGroupWrapper(obj, false);
                    }
                    else if (obj is INode)
                    {
                        target = SharedData.Graph.GetNodeWrapper(obj, false);
                    }
                    if (target != null)
                        DrawHeadedLine(new Point(target.Bounds.Left, target.Bounds.Top - 5), new Point(target.Bounds.Right, target.Bounds.Top - 5));
                    //}
                }
            }
            else
            {
                _newHeight = snap.Proposed.Height;
                if (snaptonode.SnapToObject.Contains(SnapToObject.Top))
                {
                    DrawHeadedLine(new Point(node.Bounds.Right + 5, node.Bounds.Bottom - _newHeight.Value),
                        new Point(node.Bounds.Right + 5, node.Bounds.Bottom));
                }
                else
                {
                    DrawHeadedLine(new Point(node.Bounds.Right + 5, node.Bounds.Top),
                        new Point(node.Bounds.Right + 5, node.Bounds.Top + _newHeight.Value));
                }

                foreach (object obj in snaptonode.SameSizeObjects)
                {
                    IInternalNode target = null;
                    if (obj is IGroup)
                    {
                        target = SharedData.Graph.GetGroupWrapper(obj, false);
                    }
                    else if (obj is INode)
                    {
                        target = SharedData.Graph.GetNodeWrapper(obj, false);
                    }
                    if (target != null)
                        DrawHeadedLine(new Point(target.Bounds.Right + 5, target.Bounds.Top),
                            new Point(target.Bounds.Right + 5, target.Bounds.Bottom));
                }
            }
        }

        private void CreateSpacingLines(SnapParameter snap, IInternalNode node)
        {
            switch ((snap.SnapInfo as EqualSpaceSnapInfo).SnapToObject)
            {
                case SnapToObject.HorizontalSpacing:
                    List<Point> points = new List<Point>();
                    double bottom = 0;
                    node.OffsetX = snap.Proposed.X.Value;
                    int i = 0;
                    List<object> equallyspacedObjects = (snap.SnapInfo as EqualSpaceSnapInfo).EquallySpacedObjects;
                    foreach (object obj in equallyspacedObjects)
                    {
                        IInternalNode nod = null;
                        if (!(obj is IGroup))
                        {
                            nod = SharedData.Graph.GetNodeWrapper(obj, false);
                        }
                        else
                            nod = SharedData.Graph.GetGroupWrapper(obj, false);
                        if (nod != null)
                        {
                            if (i == 0)
                            {

                                points.Add(new Point(nod.Bounds.Right, 0));
                            }
                            else if (i == equallyspacedObjects.Count - 1)
                            {
                                points[i - 1] = new Point(points[i - 1].X, nod.Bounds.Left);
                            }
                            else
                            {
                                points[i - 1] = new Point(points[i - 1].X, nod.Bounds.Left);
                                points.Add(new Point(nod.Bounds.Right, 0));
                            }
                            if (bottom < nod.Bounds.Bottom)
                            {
                                bottom = nod.Bounds.Bottom;
                            }
                            i++;
                        }
                    }
                    if (points.Count > 1)
                    {
                        foreach (Point pt in points)
                        {
                            DrawHeadedLine(new Point(pt.X, bottom + 5), new Point(pt.Y, bottom + 5));
                        }
                    }
                    break;
                case SnapToObject.VerticalSpacing:
                    points = new List<Point>();
                    node.OffsetY = snap.Proposed.Y.Value;
                    double right = 0;
                    i = 0;
                    equallyspacedObjects = (snap.SnapInfo as EqualSpaceSnapInfo).EquallySpacedObjects;
                    foreach (object obj in equallyspacedObjects)
                    {
                        IInternalNode nod = null;
                        if (!(obj is IGroup))
                        {
                            nod = SharedData.Graph.GetNodeWrapper(obj, false);
                        }
                        else
                            nod = SharedData.Graph.GetGroupWrapper(obj, false);
                        if (nod != null)
                        {
                            if (i == 0)
                            {
                                points.Add(new Point(nod.Bounds.Bottom, 0));
                            }
                            else if (i == equallyspacedObjects.Count - 1)
                            {
                                points[i - 1] = new Point(points[i - 1].X, nod.Bounds.Top);
                            }
                            else
                            {
                                points[i - 1] = new Point(points[i - 1].X, nod.Bounds.Top);
                                points.Add(new Point(nod.Bounds.Bottom, 0));
                            }
                            if (right < nod.Bounds.Right)
                            {
                                right = nod.Bounds.Right;
                            }
                            i++;
                        }
                    }
                    if (points.Count > 1)
                    {
                        foreach (Point pt in points)
                        {
                            DrawHeadedLine(new Point(right + 5, pt.X), new Point(right + 5, pt.Y));
                        }
                    }
                    break;
            }
        }

        private void DrawGuideLines(Point start, Point End)
        {
            Rect viewport = SharedData.ScrollViewer.Viewport;
            Point pt = new Point(viewport.X, viewport.Y);
            SharedData.Adorner.DrawGuideLine(start, End,"Guidelines");
        }

        private void DrawHeadedLine(Point start, Point end)
        {
            Rect viewport = SharedData.ScrollViewer.Viewport;
            Point pt = new Point(viewport.X, viewport.Y);
            SharedData.Adorner.DrawGuideLine(start, end);
        }

       #endregion

       # region Gridlines

        internal void PrepareUnit()
        {
            UpdatePatternSize();
            if (SharedData.ScrollViewer != null)
                UpdateGridlines(SharedData.ScrollViewer.Viewport);
        }

        private void UpdatePatternSize()
        {
            if (SharedData.Graph.SnapSettings.HorizontalGridlines == null && SharedData.Graph.VerticalRuler != null)
            {
                FindIntervals(SharedData.Graph.VerticalRuler, _mHorizontalGridlines);
            }
            if (SharedData.Graph.SnapSettings.VerticalGridlines == null && SharedData.Graph.HorizontalRuler != null)
            {
                FindIntervals(SharedData.Graph.HorizontalRuler, _mVerticalGridlines);
            }

        }

        private void FindIntervals(Controls.Ruler ruler, Gridlines gridlines)
        {
            double patternSize = 0;
            double segmentWidth = 0;
            int intervals = 0;
            ruler.GetRulerSegmentValues(out segmentWidth, out intervals);
            patternSize = segmentWidth;
            patternSize = SharedData.Unit.ToPixel(patternSize);
            double size = patternSize / intervals;
            List<double> lineinterval = new List<double>()
            {
                0.8,size-0.8
            };
            for (int i = 1; i < intervals; i++)
            {
                lineinterval.Add(0.25);
                lineinterval.Add(size - 0.25);
            }
            List<double> interval = new List<double>() { size / SharedData.ScrollViewer.CurrentZoom };

            if (gridlines.SnapInterval != interval)
            {
                gridlines.SnapInterval = interval;
            }
            if (gridlines.LinesInterval != lineinterval)
            {
                gridlines.LinesInterval = lineinterval;
            }
        }

        internal void UpdateGridlines(Rect view)
        {
            if (view != new Rect(0, 0, 0, 0) && view != Rect.Empty)
            {
                int levelX = 1;
                int levelY = 1;
                int linesPerPatterninY = 0;
                int linesPerPatterninX = 0;
                double sizX = 0;
                double sizY = 0;
                int countx = 0;
                int county = 0;
                double startx = 0;
                double starty = 0;
                //Updating LineIntervals and SnapIntervals
                UpdatePatternSize();
                List<Style> horizontalStrokes = _mHorizontalGridlines.Strokes.ToList();
                List<Style> verticalStrokes = _mVerticalGridlines.Strokes.ToList();
                List<double> horizontalIntervals = _mHorizontalGridlines.LinesInterval.ToList();
                List<double> verticalIntervals = _mVerticalGridlines.LinesInterval.ToList();
                SnapConstraints snapConstraints = SharedData.Graph.SnapSettings.SnapConstraints;
                double zoomx = _mVerticalGridlines.DynamicZoom && (VerticalGridlines != null ||
                                SharedData.Graph.HorizontalRuler == null) ?
                                SharedData.ScrollViewer.CurrentZoom : 1;
                double zoomy = _mHorizontalGridlines.DynamicZoom && (HorizontalGridlines != null ||
                                SharedData.Graph.VerticalRuler == null) ?
                                SharedData.ScrollViewer.CurrentZoom : 1;

                //Calculating the Start points of Gridlines
                if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                {
                    linesPerPatterninX = verticalIntervals.Count / 2;

                    InitializeGridlines(verticalIntervals, ref sizX, ref startx, ref levelX, ref countx, view.X, view.Width,
                        zoomx);
                }
                if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                {
                    linesPerPatterninY = horizontalIntervals.Count / 2;

                    InitializeGridlines(horizontalIntervals, ref sizY, ref starty, ref levelY, ref county, view.Y, view.Height,
                          zoomy);
                }

                //Drawing New Gridlines
                if (_mUsedLines.Count == 0)
                {
                    if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                    {
                        for (int i = 0; i < countx; i++)
                        {
                            DrawLinePattern(startx + sizX * i, starty, i, levelX, zoomx,
                                "Vertical", view, verticalIntervals, verticalStrokes);
                        }
                    }
                    if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                    {
                        for (int i = 0; i < county; i++)
                        {
                            DrawLinePattern(startx, starty + sizY * i, i, levelY, zoomy,
                                "Horizontal", view, horizontalIntervals, horizontalStrokes);
                        }
                    }
                }
                else
                {
                    // Reusing and adding needed lines
                    if (_mUsedLines.Count == (countx * linesPerPatterninX + county * linesPerPatterninY))
                    {
                        if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                        {
                            for (int i = 0; i < countx; i++)
                            {
                                ChangeLinePattern(startx + sizX * i, starty, i, levelX, zoomx,
                                    "Vertical", view, verticalIntervals, verticalStrokes);
                            }
                        }
                        if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                        {
                            for (int i = 0; i < county; i++)
                            {
                                int ind = countx * linesPerPatterninX + i * linesPerPatterninY;
                                ChangeLinePattern(startx, starty + sizY * i, ind, levelY, zoomy,
                                    "Horizontal", view, horizontalIntervals, horizontalStrokes);
                            }
                        }
                    }
                    else if (_mUsedLines.Count < ((countx * linesPerPatterninX + county * linesPerPatterninY)))
                    {
                        if (_mUsedLines.Count >= (countx * linesPerPatterninX))
                        {
                            if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                            {
                                if (_mUsedLines.Count >= (countx * linesPerPatterninX))
                                {
                                    for (int i = 0; i < countx; i++)
                                    {
                                        ChangeLinePattern(startx + sizX * i, starty, i, levelX, zoomx,
                                            "Vertical", view, verticalIntervals, verticalStrokes);
                                    }
                                }
                            }
                            if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                            {
                                double unChangedLines = _mUsedLines.Count - countx * linesPerPatterninX;
                                int patternsTobeChanged = (int)(unChangedLines / linesPerPatterninY);
                                for (int i = 0; i < patternsTobeChanged; i++)
                                {
                                    int ind = countx * linesPerPatterninX + i * linesPerPatterninY;
                                    ChangeLinePattern(startx, starty + sizY * i, ind, levelY, zoomy,
                                        "Horizontal", view, horizontalIntervals, horizontalStrokes);
                                }
                                double diff = county - patternsTobeChanged;
                                for (int i = 0; i < diff; i++)
                                {
                                    DrawLinePattern(startx, starty + sizY * (i + patternsTobeChanged),
                                                  (int)(countx + i + patternsTobeChanged), levelY, zoomy,
                                                  "Horizontal", view, horizontalIntervals, horizontalStrokes);
                                }
                            }
                        }
                        else
                        {
                            if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                            {
                                int patternsTobeChanged = (int)(_mUsedLines.Count / linesPerPatterninX);
                                for (int i = 0; i < patternsTobeChanged; i++)
                                {
                                    ChangeLinePattern(startx + sizX * i, starty, i, levelX, zoomx,
                                        "Vertical", view, verticalIntervals, verticalStrokes);
                                }
                                double diff = countx - patternsTobeChanged;
                                for (int i = 0; i < diff; i++)
                                {
                                    DrawLinePattern(startx + sizX * (i + patternsTobeChanged), starty, i, levelX, zoomx,
                                        "Vertical", view, verticalIntervals, verticalStrokes);
                                }
                            }
                            if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                            {
                                for (int i = 0; i < county; i++)
                                {
                                    DrawLinePattern(startx, starty + sizY * i, countx + i, levelY, zoomy,
                                        "Horizontal", view, horizontalIntervals, horizontalStrokes);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (snapConstraints.Contains(SnapConstraints.VerticalLines))
                        {
                            for (int i = 0; i < countx; i++)
                            {
                                ChangeLinePattern(startx + sizX * i, starty, i, levelX, zoomx,
                                    "Vertical", view, verticalIntervals, verticalStrokes);
                            }
                        }
                        if (snapConstraints.Contains(SnapConstraints.HorizontalLines))
                        {
                            for (int i = 0; i < county; i++)
                            {
                                int ind = countx * linesPerPatterninX + i * linesPerPatterninY;
                                ChangeLinePattern(startx, starty + sizY * i, ind, levelY, zoomy,
                                    "Horizontal", view, horizontalIntervals, horizontalStrokes);
                            }
                        }

                        int addedlines = countx * linesPerPatterninX + county * linesPerPatterninY;
                        for (int i = _mUsedLines.Count - 1; i >= addedlines; i--)
                        {
                            _mUnUsedLines.Add(_mUsedLines[i]);
                            RemoveGridline(_mUsedLines[i], i);
                        }
                    }
                }
            }
        }


        private void InitializeGridlines(IEnumerable<double> intervalsinX, ref double sizX, ref double startx,
            ref int levelX, ref int countx, double view_start, double view_end, double currentZoom)
        {
            int countinX = intervalsinX.Count();
            sizX = intervalsinX.Sum();

            double minGridSizeX = sizX / 2;
            double maxGridSizeX = sizX * 2;

            sizX = sizX * currentZoom;
            countx = (int)(view_end / sizX);
            if (countx > 0)
                countx += 2;
            if (sizX < minGridSizeX)
            {
                countx /= 2;
                sizX = sizX * 2;
                levelX--;
                while (sizX < minGridSizeX)
                {
                    countx /= 2;
                    sizX = sizX * 2;
                    levelX--;
                }
                countx++;
            }
            if (sizX > maxGridSizeX)
            {
                if (countx == 0)
                {
                    countx = countx + 2;
                }
                levelX++;
                countx *= 2;
                sizX = sizX / 2;
                while (sizX > maxGridSizeX)
                {
                    countx *= 2;
                    sizX = sizX / 2;
                    levelX++;
                }
            }
            startx = (view_start) % sizX;
            if (startx < 0)
            {
                startx = sizX - startx;
                while (startx > 0)
                {
                    startx -= sizX;
                }
            }
            else
            {
                startx *= -1;
            }
        }

        private void ChangeLinePattern(double startx, double starty, int index, int level,
            double currentZoom, string axis, Rect view, List<double> intervals, List<Style> strokes)
        {
            double end = 0;
            int count;
            double power = Math.Pow(2, level - 1);
            if (axis.Equals("Horizontal"))
            {
                count = strokes.Count;
                for (int i = 0; i < intervals.Count - 1; i = i + 2)
                {
                    SharedData.GridLinePanel.ChangePoints(new Point(0, starty + end), new Point(view.Width, starty + end), _mUsedLines[index + i / 2],
                        intervals[i], strokes[i % count]);
                    end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                }
            }
            else
            {
                count = strokes.Count;
                for (int i = 0; i < intervals.Count - 1; i = i + 2)
                {
                    SharedData.GridLinePanel.ChangePoints(new Point(startx + end, 0), new Point(startx + end, view.Height),
                        _mUsedLines[index * intervals.Count / 2 + i / 2], intervals[i], strokes[i % count]);
                    end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                }
            }

        }

        private void DrawLinePattern(double startx, double starty, int index, int level,
            double currentZoom, string axis, Rect view, List<double> intervals, List<Style> strokes)
        {
            double power = Math.Pow(2, level - 1);
            if (axis.Equals("Vertical"))
            {
                int count = strokes.Count;
                if (_mUnUsedLines.Count < intervals.Count / 2)
                {
                    _mUnUsedLines.Clear();
                    double end = 0;
                    for (int i = 0; i < intervals.Count - 1; i = i + 2)
                    {
                        Path path = SharedData.GridLinePanel.DrawLine(new Point((startx + end), 0),
                            new Point(startx + end, view.Height), intervals[i], strokes[i % count]);
                        AddGridLine(path);
                        end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                    }
                }
                else
                {
                    double end = 0;
                    for (int i = 0; i < intervals.Count - 1; i = i + 2)
                    {
                        SharedData.GridLinePanel.ChangePoints(new Point((startx + end), 0),
                            new Point(startx + end, view.Height), _mUnUsedLines[0],
                            intervals[i], strokes[i % count]);
                        end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                        AddGridLine(_mUnUsedLines[0]);
                        _mUnUsedLines.RemoveAt(0);
                    }
                }
            }
            else if (axis.Equals("Horizontal"))
            {
                int count = strokes.Count;
                if (_mUnUsedLines.Count < intervals.Count / 2)
                {
                    _mUnUsedLines.Clear();
                    double end = 0;
                    for (int i = 0; i < intervals.Count - 1; i = i + 2)
                    {
                        Path path = SharedData.GridLinePanel.DrawLine(new Point(0, starty + end),
                            new Point(view.Width, starty + end), intervals[i], strokes[i % count]);
                        AddGridLine(path);
                        end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                    }
                }
                else
                {
                    double end = 0;
                    for (int i = 0; i < intervals.Count - 1; i = i + 2)
                    {
                        SharedData.GridLinePanel.ChangePoints(new Point((startx), starty + end),
                                                                    new Point(view.Width, starty + end),
                                                                   _mUnUsedLines[0]
                                                                   , intervals[i], strokes[i % count]);
                        AddGridLine(_mUnUsedLines[0]);
                        end += ((intervals[i] + intervals[i + 1]) * currentZoom) / power;
                        _mUnUsedLines.RemoveAt(0);
                    }
                }
            }
        }

        private void ClearGridlines()
        {
            if (SharedData.GridLinePanel.Children.Count > 0)
            {
                for (int i = _mUsedLines.Count - 1; i >= 0; i--)
                {
                    _mUnUsedLines.Add(_mUsedLines[i]);
                    RemoveGridline(_mUsedLines[i], i);
                }
            }
        }

        private void RemoveGridline(Path path, int index)
        {
            SharedData.GridLinePanel.Children.Remove(path);
            _mUsedLines.RemoveAt(index);
        }

        private void AddGridLine(Path path)
        {
            _mUsedLines.Add(path);
            SharedData.GridLinePanel.Children.Add(path);
        }
        #endregion
        
       
    }

}
