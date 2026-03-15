#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Media; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface INode : IGroupable
    {
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        Point Pivot { get; set; }
        double RotateAngle { get; set; }
        SnapToObject SnapToObject { get; set; }
        double MinWidth { get; set; }
        double MaxWidth { get; set; }
        //double Width { get; set; }
        double UnitWidth { get; set; }
        double MinHeight { get; set; }
        double MaxHeight { get; set; }
        //double Height { get; set; }
        double UnitHeight { get; set; }
        object Shape { get; set; }
        Style ShapeStyle { get; set; }
        NodeConstraints Constraints { get; set; }
        object Content { get; set; }
        DataTemplate ContentTemplate { get; set; }
        object Ports { get; set; }
        Flip Flip { get; set; }
        bool IsExpanded { get; set; }
    }

    public interface INodeInfo :
        IGroupableInfo
    {
        void ScaleTo(double newWidth, double newHeight,
                     Point? pivot, bool aspectRatio = false);

        void RotateTo(double newAngle, Point? pivot);
        void DragXTo(double newValue);
        void DragYTo(double newValue);
        void MovePivotTo(Point newPivot);
        Size DesiredSize { get; }
        double ActualWidth { get; }
        double ActualHeight { get; }


        IEnumerable<IConnector> InOutConnectors { get; }
        IEnumerable<IConnector> InConnectors { get; }
        IEnumerable<IConnector> OutConnectors { get; }

        IEnumerable<INode> Neighbors { get; }
        IEnumerable<INode> InNeighbors { get; }
        IEnumerable<INode> OutNeighbors { get; }

    }



    internal interface IInternalNode :
        INode,
        INodeInfo,
        //IWrapper<TID, TNode, TConnector, TGroup, TSource, IView>,
        IInternalGroupable
    {
        IEnumerable<INode> Children { get; }
        bool HasChild { get; }
        Rect Rectangle { get; }

        //MatrixExt Matrix { get; }
        TransformState TransformState { get; }
        bool CanStartTransform(TransformState state);
        void EndTransform(TransformState state);
        bool CanDrag();
        bool CanRotate();
        bool CanScale();
        Point GetIntersection(Point point);
        Point GetIntersection(IInternalNode end);
        void InializeRelationship();
        void UpdateBoundsCorners();
        OrthogonalDirection GetDirection(Point port);

        IEnumerable<IInternalConnector> InternalInOutConnectors { get; }
        IEnumerable<IInternalConnector> InternalInConnectors { get; }
        IEnumerable<IInternalConnector> InternalOutConnectors { get; }

        IEnumerable<IInternalNode> InternalNeighbors { get; }
        IEnumerable<IInternalNode> InternalInNeighbors { get; }
        IEnumerable<IInternalNode> InternalOutNeighbors { get; }
        IEnumerable<IInternalNode> InternalChildren { get; }

        IInternalNode ParentNode { get; }
        IInternalNode FirstChild { get; }
        IInternalNode LastChild { get; }
        IInternalNode PreviousSibling { get; }
        IInternalNode NextSibling { get; }


        /*
         * Properties ment for radial and table layout, these properties should will be removed in future.
         */
        int Stage { get; set; }
        double TempX { get; set; }
        double TempY { get; set; }
        bool Visited { get; set; }
        bool SubTreeVal { get; set; }
        double SegmentOffset { get; set; }

        ObservableElements<INodePort, IInternalNodePort>
            InternalPorts { get; }
    }


    internal interface IProtectedNode : IInternalNode
    {
        void SetDesiredSize(Size desiredSize);
        void SetActualSize(Size actualSize);
        void UpdateCompositeTrans();
        void Invalidate(bool measure = true, bool arrange = true);
    }

    //internal interface IInternalNode<TID, TNode, TConnector, TGroup, out TSource> :
    //    INodeInfo,
    //    //IWrapper<TID, TNode, TConnector, TGroup, TSource, IView>,
    //    IInternalGroupable<TID, TNode, TConnector, TGroup, TSource>,
    //    IInternalNode
    //{
    //}

    public class PropertyMapping : Attribute
    {
        public string Property;
    }
         
}
