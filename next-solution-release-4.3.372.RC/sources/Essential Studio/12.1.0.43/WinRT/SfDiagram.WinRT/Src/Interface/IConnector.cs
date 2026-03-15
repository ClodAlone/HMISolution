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
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IConnector : IGroupable
    {
        Point SourcePoint { get; set; }
        Point TargetPoint { get; set; }
        Style ConnectorGeometryStyle { get; set; }
        object SourceDecorator { get; set; }
        object TargetDecorator { get; set; }
        Style SourceDecoratorStyle { get; set; }
        Style TargetDecoratorStyle { get; set; }
        //double SourceEndSpace { get; set; }
        //double TargetEndSpace { get; set; }
        IList<IConnectorSegment> Segments { get; set; }
        //SegmentCount MinimumSegments { get; set; }
        ConnectorConstraints Constraints { get; set; }
        object SourceNode { get; set; }
        object TargetNode { get; set; }
        IPort SourcePort { get; set; }
        IPort TargetPort { get; set; }
        BezierSmoothness BezierSmoothness { get; set; }
        double BridgeSpace { get; set; }
    }
    
    public interface IConnectorInfo :
        IGroupableInfo
    {
    }

    internal interface IInternalConnector :
        IConnector,
        IConnectorInfo,
        IWrapper,
        IInternalGroupable
    {
        IConnectorSegments InializeSegments();
        TSegment NewConnectorSegment<TSegment>() where TSegment : IConnectorSegment;
        IInternalNode KnownSourceNode { get; set; }
        IInternalNode KnownTargetNode { get; set; }
        DragState SourceDragState { get; set; }
        DragState TargetDragState { get; set; }

        IInternalNodePort KnownSourcePort { get; set; }
        IInternalNodePort KnownTargetPort { get; set; }
        IInternalSegment SelectedSegment { get; set; }
        PathSegmentCollection InternalSegments { get; }
        PathFigure PathFigure { get; }
        void UpdateGeometry();
        void UpdateThums();
        void PrepareThums();
        void DisposeThums();
        void UpdateDecorator();
        bool IsIntersect(Rect rect);
        void UpdateRouting();
        void UpdateBridging();
        void UpdateParentBridging(Rect rect);
        bool CanBridge();
        bool CanRoute();
        bool CanSourceDrag();
        bool CanTargetDrag();
        bool CanDrag();
    }
}
