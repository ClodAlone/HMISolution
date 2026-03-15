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
using System.Collections;
using System.Windows;
#if !WINRT
using System.Windows.Media; 
#endif
#if WINRT
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data; 
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Layout;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Controller;
namespace Syncfusion.UI.Xaml.Diagram
{

	public partial class NodePort :  INodePort
	{
        #region NodeOffsetX

		
		public double NodeOffsetX
        {
            get { return (double)GetValue(NodeOffsetXProperty); }
            set { SetValue(NodeOffsetXProperty, value); }
        }

        public static readonly DependencyProperty NodeOffsetXProperty =
            DependencyProperty.Register(NodePortConstants.NodeOffsetX, typeof(double), typeof(NodePort), new PropertyMetadata(0d, OnNodeOffsetXChanged));

        private static void OnNodeOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodePort).OnNodeOffsetXChanged(e);
        }	
	
    #endregion

        #region NodeOffsetY

		
		public double NodeOffsetY
        {
            get { return (double)GetValue(NodeOffsetYProperty); }
            set { SetValue(NodeOffsetYProperty, value); }
        }

        public static readonly DependencyProperty NodeOffsetYProperty =
            DependencyProperty.Register(NodePortConstants.NodeOffsetY, typeof(double), typeof(NodePort), new PropertyMetadata(0d, OnNodeOffsetYChanged));

        private static void OnNodeOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodePort).OnNodeOffsetYChanged(e);
        }	
	
    #endregion

        #region Node

		
		public object Node
        {
            get { return (object)GetValue(NodeProperty); }
            set { SetValue(NodeProperty, value); }
        }

        public static readonly DependencyProperty NodeProperty =
            DependencyProperty.Register(NodePortConstants.Node, typeof(object), typeof(NodePort), new PropertyMetadata(null, OnNodeChanged));

        private static void OnNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodePort).OnNodeChanged(e);
        }	
	
    #endregion

        #region UnitMode

		
		public UnitMode UnitMode
        {
            get { return (UnitMode)GetValue(UnitModeProperty); }
            set { SetValue(UnitModeProperty, value); }
        }

        public static readonly DependencyProperty UnitModeProperty =
            DependencyProperty.Register(NodePortConstants.UnitMode, typeof(UnitMode), typeof(NodePort), new PropertyMetadata(UnitMode.Fraction, OnUnitModeChanged));

        private static void OnUnitModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodePort).OnUnitModeChanged(e);
        }	
	
    #endregion

	}

	public partial class Node :  INode
	{
        #region OffsetX

		
		public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(NodeConstants.OffsetX, typeof(double), typeof(Node), new PropertyMetadata(0d, OnOffsetXChanged));

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnOffsetXChanged(e);
        }	
	
    #endregion

        #region OffsetY

		
		public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(NodeConstants.OffsetY, typeof(double), typeof(Node), new PropertyMetadata(0d, OnOffsetYChanged));

        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnOffsetYChanged(e);
        }	
	
    #endregion

        #region RotateAngle

		
		public double RotateAngle
        {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register(NodeConstants.RotateAngle, typeof(double), typeof(Node), new PropertyMetadata(0d, OnRotateAngleChanged));

        private static void OnRotateAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnRotateAngleChanged(e);
        }	
	
    #endregion

        #region SnapToObject

		
		public SnapToObject SnapToObject
        {
            get { return (SnapToObject)GetValue(SnapToObjectProperty); }
            set { SetValue(SnapToObjectProperty, value); }
        }

        public static readonly DependencyProperty SnapToObjectProperty =
            DependencyProperty.Register(NodeConstants.SnapToObject, typeof(SnapToObject), typeof(Node), new PropertyMetadata(SnapToObject.None, OnSnapToObjectChanged));

        private static void OnSnapToObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnSnapToObjectChanged(e);
        }	
	
    #endregion

        #region Flip

		
		public Flip Flip
        {
            get { return (Flip)GetValue(FlipProperty); }
            set { SetValue(FlipProperty, value); }
        }

        public static readonly DependencyProperty FlipProperty =
            DependencyProperty.Register(NodeConstants.Flip, typeof(Flip), typeof(Node), new PropertyMetadata(Flip.None, OnFlipChanged));

        private static void OnFlipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnFlipChanged(e);
        }	
	
    #endregion

        #region UnitWidth

		
		public double UnitWidth
        {
            get { return (double)GetValue(UnitWidthProperty); }
            set { SetValue(UnitWidthProperty, value); }
        }

        public static readonly DependencyProperty UnitWidthProperty =
            DependencyProperty.Register(NodeConstants.UnitWidth, typeof(double), typeof(Node), new PropertyMetadata(double.NaN, OnUnitWidthChanged));

        private static void OnUnitWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnUnitWidthChanged(e);
        }	
	
    #endregion

        #region UnitHeight

		
		public double UnitHeight
        {
            get { return (double)GetValue(UnitHeightProperty); }
            set { SetValue(UnitHeightProperty, value); }
        }

        public static readonly DependencyProperty UnitHeightProperty =
            DependencyProperty.Register(NodeConstants.UnitHeight, typeof(double), typeof(Node), new PropertyMetadata(double.NaN, OnUnitHeightChanged));

        private static void OnUnitHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnUnitHeightChanged(e);
        }	
	
    #endregion

        #region Shape

		
		public object Shape
        {
            get { return (object)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(NodeConstants.Shape, typeof(object), typeof(Node), new PropertyMetadata(null, OnShapeChanged));

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnShapeChanged(e);
        }	
	
    #endregion

        #region ShapeStyle

		
		public Style ShapeStyle
        {
            get { return (Style)GetValue(ShapeStyleProperty); }
            set { SetValue(ShapeStyleProperty, value); }
        }

        public static readonly DependencyProperty ShapeStyleProperty =
            DependencyProperty.Register(NodeConstants.ShapeStyle, typeof(Style), typeof(Node), new PropertyMetadata(null, OnShapeStyleChanged));

        private static void OnShapeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnShapeStyleChanged(e);
        }	
	
    #endregion

        #region ParentGroup

		
		public object ParentGroup
        {
            get { return (object)GetValue(ParentGroupProperty); }
            set { SetValue(ParentGroupProperty, value); }
        }

        public static readonly DependencyProperty ParentGroupProperty =
            DependencyProperty.Register(NodeConstants.ParentGroup, typeof(object), typeof(Node), new PropertyMetadata(null, OnParentGroupChanged));

        private static void OnParentGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnParentGroupChanged(e);
        }	
	
    #endregion

        #region IsExpanded

		
		public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(NodeConstants.IsExpanded, typeof(bool), typeof(Node), new PropertyMetadata(true, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnIsExpandedChanged(e);
        }	
	
    #endregion

        #region Pivot

		
		public Point Pivot
        {
            get { return (Point)GetValue(PivotProperty); }
            set { SetValue(PivotProperty, value); }
        }

        public static readonly DependencyProperty PivotProperty =
            DependencyProperty.Register(NodeConstants.Pivot, typeof(Point), typeof(Node), new PropertyMetadata(new Point(0.5, 0.5), OnPivotChanged));

        private static void OnPivotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnPivotChanged(e);
        }	
	
    #endregion

        #region AutoBind

		
		public bool AutoBind
        {
            get { return (bool)GetValue(AutoBindProperty); }
            set { SetValue(AutoBindProperty, value); }
        }

        public static readonly DependencyProperty AutoBindProperty =
            DependencyProperty.Register(NodeConstants.AutoBind, typeof(bool), typeof(Node), new PropertyMetadata(true, OnAutoBindChanged));

        private static void OnAutoBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnAutoBindChanged(e);
        }	
	
    #endregion

        #region ID

		
		public object ID
        {
            get { return (object)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register(NodeConstants.ID, typeof(object), typeof(Node), new PropertyMetadata(null, OnIDChanged));

        private static void OnIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnIDChanged(e);
        }	
	
    #endregion

        #region Key

		
		public object Key
        {
            get { return (object)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(NodeConstants.Key, typeof(object), typeof(Node), new PropertyMetadata(null, OnKeyChanged));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnKeyChanged(e);
        }	
	
    #endregion

        #region IsSelected

		
		public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(NodeConstants.IsSelected, typeof(bool), typeof(Node), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnIsSelectedChanged(e);
        }	
	
    #endregion

        #region ZIndex

		
		public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register(NodeConstants.ZIndex, typeof(int), typeof(Node), new PropertyMetadata(0, OnZIndexChanged));

        private static void OnZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnZIndexChanged(e);
        }	
	
    #endregion

        #region Annotations

		
		public object Annotations
        {
            get { return (object)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register(NodeConstants.Annotations, typeof(object), typeof(Node), new PropertyMetadata(null, OnAnnotationsChanged));

        private static void OnAnnotationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnAnnotationsChanged(e);
        }	
	
    #endregion

        #region Constraints

		
		public NodeConstraints Constraints
        {
            get { return (NodeConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        public static readonly DependencyProperty ConstraintsProperty =
            DependencyProperty.Register(NodeConstants.Constraints, typeof(NodeConstraints), typeof(Node), new PropertyMetadata(NodeConstraints.Default, OnConstraintsChanged));

        private static void OnConstraintsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnConstraintsChanged(e);
        }	
	
    #endregion

        #region Ports

		
		public object Ports
        {
            get { return (object)GetValue(PortsProperty); }
            set { SetValue(PortsProperty, value); }
        }

        public static readonly DependencyProperty PortsProperty =
            DependencyProperty.Register(NodeConstants.Ports, typeof(object), typeof(Node), new PropertyMetadata(null, OnPortsChanged));

        private static void OnPortsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Node).OnPortsChanged(e);
        }	
	
    #endregion

	}

	public partial class Connector :  IConnector
	{
        #region SourceNode

		
		public object SourceNode
        {
            get { return (object)GetValue(SourceNodeProperty); }
            set { SetValue(SourceNodeProperty, value); }
        }

        public static readonly DependencyProperty SourceNodeProperty =
            DependencyProperty.Register(ConnectorConstants.SourceNode, typeof(object), typeof(Connector), new PropertyMetadata(null, OnSourceNodeChanged));

        private static void OnSourceNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSourceNodeChanged(e);
        }	
	
    #endregion

        #region TargetNode

		
		public object TargetNode
        {
            get { return (object)GetValue(TargetNodeProperty); }
            set { SetValue(TargetNodeProperty, value); }
        }

        public static readonly DependencyProperty TargetNodeProperty =
            DependencyProperty.Register(ConnectorConstants.TargetNode, typeof(object), typeof(Connector), new PropertyMetadata(null, OnTargetNodeChanged));

        private static void OnTargetNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnTargetNodeChanged(e);
        }	
	
    #endregion

        #region SourcePort

		
		public IPort SourcePort
        {
            get { return (IPort)GetValue(SourcePortProperty); }
            set { SetValue(SourcePortProperty, value); }
        }

        public static readonly DependencyProperty SourcePortProperty =
            DependencyProperty.Register(ConnectorConstants.SourcePort, typeof(IPort), typeof(Connector), new PropertyMetadata(null, OnSourcePortChanged));

        private static void OnSourcePortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSourcePortChanged(e);
        }	
	
    #endregion

        #region TargetPort

		
		public IPort TargetPort
        {
            get { return (IPort)GetValue(TargetPortProperty); }
            set { SetValue(TargetPortProperty, value); }
        }

        public static readonly DependencyProperty TargetPortProperty =
            DependencyProperty.Register(ConnectorConstants.TargetPort, typeof(IPort), typeof(Connector), new PropertyMetadata(null, OnTargetPortChanged));

        private static void OnTargetPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnTargetPortChanged(e);
        }	
	
    #endregion

        #region Segments

		
		public IList<IConnectorSegment> Segments
        {
            get { return (IList<IConnectorSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register(ConnectorConstants.Segments, typeof(IList<IConnectorSegment>), typeof(Connector), new PropertyMetadata(null, OnSegmentsChanged));

        private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSegmentsChanged(e);
        }	
	
    #endregion

        #region SourcePoint

		
		public Point SourcePoint
        {
            get { return (Point)GetValue(SourcePointProperty); }
            set { SetValue(SourcePointProperty, value); }
        }

        public static readonly DependencyProperty SourcePointProperty =
            DependencyProperty.Register(ConnectorConstants.SourcePoint, typeof(Point), typeof(Connector), new PropertyMetadata(new Point(0,0), OnSourcePointChanged));

        private static void OnSourcePointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSourcePointChanged(e);
        }	
	
    #endregion

        #region TargetPoint

		
		public Point TargetPoint
        {
            get { return (Point)GetValue(TargetPointProperty); }
            set { SetValue(TargetPointProperty, value); }
        }

        public static readonly DependencyProperty TargetPointProperty =
            DependencyProperty.Register(ConnectorConstants.TargetPoint, typeof(Point), typeof(Connector), new PropertyMetadata(new Point(0,0), OnTargetPointChanged));

        private static void OnTargetPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnTargetPointChanged(e);
        }	
	
    #endregion

        #region ConnectorGeometryStyle

		
		public Style ConnectorGeometryStyle
        {
            get { return (Style)GetValue(ConnectorGeometryStyleProperty); }
            set { SetValue(ConnectorGeometryStyleProperty, value); }
        }

        public static readonly DependencyProperty ConnectorGeometryStyleProperty =
            DependencyProperty.Register(ConnectorConstants.ConnectorGeometryStyle, typeof(Style), typeof(Connector), new PropertyMetadata(null, OnConnectorGeometryStyleChanged));

        private static void OnConnectorGeometryStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnConnectorGeometryStyleChanged(e);
        }	
	
    #endregion

        #region SourceDecorator

		
		public object SourceDecorator
        {
            get { return (object)GetValue(SourceDecoratorProperty); }
            set { SetValue(SourceDecoratorProperty, value); }
        }

        public static readonly DependencyProperty SourceDecoratorProperty =
            DependencyProperty.Register(ConnectorConstants.SourceDecorator, typeof(object), typeof(Connector), new PropertyMetadata(null, OnSourceDecoratorChanged));

        private static void OnSourceDecoratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSourceDecoratorChanged(e);
        }	
	
    #endregion

        #region TargetDecorator

		
		public object TargetDecorator
        {
            get { return (object)GetValue(TargetDecoratorProperty); }
            set { SetValue(TargetDecoratorProperty, value); }
        }

        public static readonly DependencyProperty TargetDecoratorProperty =
            DependencyProperty.Register(ConnectorConstants.TargetDecorator, typeof(object), typeof(Connector), new PropertyMetadata("M0,0 L10,5 L0,10 L 0,0", OnTargetDecoratorChanged));

        private static void OnTargetDecoratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnTargetDecoratorChanged(e);
        }	
	
    #endregion

        #region SourceDecoratorStyle

		
		public Style SourceDecoratorStyle
        {
            get { return (Style)GetValue(SourceDecoratorStyleProperty); }
            set { SetValue(SourceDecoratorStyleProperty, value); }
        }

        public static readonly DependencyProperty SourceDecoratorStyleProperty =
            DependencyProperty.Register(ConnectorConstants.SourceDecoratorStyle, typeof(Style), typeof(Connector), new PropertyMetadata(null, OnSourceDecoratorStyleChanged));

        private static void OnSourceDecoratorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnSourceDecoratorStyleChanged(e);
        }	
	
    #endregion

        #region TargetDecoratorStyle

		
		public Style TargetDecoratorStyle
        {
            get { return (Style)GetValue(TargetDecoratorStyleProperty); }
            set { SetValue(TargetDecoratorStyleProperty, value); }
        }

        public static readonly DependencyProperty TargetDecoratorStyleProperty =
            DependencyProperty.Register(ConnectorConstants.TargetDecoratorStyle, typeof(Style), typeof(Connector), new PropertyMetadata(null, OnTargetDecoratorStyleChanged));

        private static void OnTargetDecoratorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnTargetDecoratorStyleChanged(e);
        }	
	
    #endregion

        #region BridgeSpace

		
		public double BridgeSpace
        {
            get { return (double)GetValue(BridgeSpaceProperty); }
            set { SetValue(BridgeSpaceProperty, value); }
        }

        public static readonly DependencyProperty BridgeSpaceProperty =
            DependencyProperty.Register(ConnectorConstants.BridgeSpace, typeof(double), typeof(Connector), new PropertyMetadata(15d, OnBridgeSpaceChanged));

        private static void OnBridgeSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnBridgeSpaceChanged(e);
        }	
	
    #endregion

        #region AutoBind

		
		public bool AutoBind
        {
            get { return (bool)GetValue(AutoBindProperty); }
            set { SetValue(AutoBindProperty, value); }
        }

        public static readonly DependencyProperty AutoBindProperty =
            DependencyProperty.Register(ConnectorConstants.AutoBind, typeof(bool), typeof(Connector), new PropertyMetadata(true, OnAutoBindChanged));

        private static void OnAutoBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnAutoBindChanged(e);
        }	
	
    #endregion

        #region ID

		
		public object ID
        {
            get { return (object)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register(ConnectorConstants.ID, typeof(object), typeof(Connector), new PropertyMetadata(null, OnIDChanged));

        private static void OnIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnIDChanged(e);
        }	
	
    #endregion

        #region Key

		
		public object Key
        {
            get { return (object)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(ConnectorConstants.Key, typeof(object), typeof(Connector), new PropertyMetadata(null, OnKeyChanged));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnKeyChanged(e);
        }	
	
    #endregion

        #region IsSelected

		
		public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(ConnectorConstants.IsSelected, typeof(bool), typeof(Connector), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnIsSelectedChanged(e);
        }	
	
    #endregion

        #region ZIndex

		
		public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register(ConnectorConstants.ZIndex, typeof(int), typeof(Connector), new PropertyMetadata(0, OnZIndexChanged));

        private static void OnZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnZIndexChanged(e);
        }	
	
    #endregion

        #region Annotations

		
		public object Annotations
        {
            get { return (object)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register(ConnectorConstants.Annotations, typeof(object), typeof(Connector), new PropertyMetadata(null, OnAnnotationsChanged));

        private static void OnAnnotationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnAnnotationsChanged(e);
        }	
	
    #endregion

        #region Constraints

		
		public ConnectorConstraints Constraints
        {
            get { return (ConnectorConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        public static readonly DependencyProperty ConstraintsProperty =
            DependencyProperty.Register(ConnectorConstants.Constraints, typeof(ConnectorConstraints), typeof(Connector), new PropertyMetadata(ConnectorConstraints.Default, OnConstraintsChanged));

        private static void OnConstraintsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnConstraintsChanged(e);
        }	
	
    #endregion

        #region BezierSmoothness

		
		public BezierSmoothness BezierSmoothness
        {
            get { return (BezierSmoothness)GetValue(BezierSmoothnessProperty); }
            set { SetValue(BezierSmoothnessProperty, value); }
        }

        public static readonly DependencyProperty BezierSmoothnessProperty =
            DependencyProperty.Register(ConnectorConstants.BezierSmoothness, typeof(BezierSmoothness), typeof(Connector), new PropertyMetadata(BezierSmoothness.None, OnBezierSmoothnessChanged));

        private static void OnBezierSmoothnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnBezierSmoothnessChanged(e);
        }	
	
    #endregion

        #region ParentGroup

		
		public object ParentGroup
        {
            get { return (object)GetValue(ParentGroupProperty); }
            set { SetValue(ParentGroupProperty, value); }
        }

        public static readonly DependencyProperty ParentGroupProperty =
            DependencyProperty.Register(ConnectorConstants.ParentGroup, typeof(object), typeof(Connector), new PropertyMetadata(null, OnParentGroupChanged));

        private static void OnParentGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Connector).OnParentGroupChanged(e);
        }	
	
    #endregion

	}

	public partial class Group :  IGroup
	{
        #region Nodes

		
		public object Nodes
        {
            get { return (object)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register(GroupConstants.Nodes, typeof(object), typeof(Group), new PropertyMetadata(null, OnNodesChanged));

        private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Group).OnNodesChanged(e);
        }	
	
    #endregion

        #region Connectors

		
		public object Connectors
        {
            get { return (object)GetValue(ConnectorsProperty); }
            set { SetValue(ConnectorsProperty, value); }
        }

        public static readonly DependencyProperty ConnectorsProperty =
            DependencyProperty.Register(GroupConstants.Connectors, typeof(object), typeof(Group), new PropertyMetadata(null, OnConnectorsChanged));

        private static void OnConnectorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Group).OnConnectorsChanged(e);
        }	
	
    #endregion

        #region Groups

		
		public object Groups
        {
            get { return (object)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register(GroupConstants.Groups, typeof(object), typeof(Group), new PropertyMetadata(null, OnGroupsChanged));

        private static void OnGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Group).OnGroupsChanged(e);
        }	
	
    #endregion

	}

	public partial class Selector :  ISelector
	{
        #region QuickCommands

		
		public Visibility QuickCommands
        {
            get { return (Visibility)GetValue(QuickCommandsProperty); }
            set { SetValue(QuickCommandsProperty, value); }
        }

        public static readonly DependencyProperty QuickCommandsProperty =
            DependencyProperty.Register(SelectorConstants.QuickCommands, typeof(Visibility), typeof(Selector), new PropertyMetadata(Visibility.Visible, OnQuickCommandsChanged));

        private static void OnQuickCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Selector).OnQuickCommandsChanged(e);
        }	
	
    #endregion

	}

	public partial class SfDiagram :  IGraph
	{
        #region Nodes

		
		public object Nodes
        {
            get { return (object)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register(SfDiagramConstants.Nodes, typeof(object), typeof(SfDiagram), new PropertyMetadata(null, OnNodesChanged));

        private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnNodesChanged(e);
        }	
	
    #endregion

        #region Connectors

		
		public object Connectors
        {
            get { return (object)GetValue(ConnectorsProperty); }
            set { SetValue(ConnectorsProperty, value); }
        }

        public static readonly DependencyProperty ConnectorsProperty =
            DependencyProperty.Register(SfDiagramConstants.Connectors, typeof(object), typeof(SfDiagram), new PropertyMetadata(null, OnConnectorsChanged));

        private static void OnConnectorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnConnectorsChanged(e);
        }	
	
    #endregion

        #region Groups

		
		public object Groups
        {
            get { return (object)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register(SfDiagramConstants.Groups, typeof(object), typeof(SfDiagram), new PropertyMetadata(null, OnGroupsChanged));

        private static void OnGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnGroupsChanged(e);
        }	
	
    #endregion

        #region DefaultConnectorType

		
		public ConnectorType DefaultConnectorType
        {
            get { return (ConnectorType)GetValue(DefaultConnectorTypeProperty); }
            set { SetValue(DefaultConnectorTypeProperty, value); }
        }

        public static readonly DependencyProperty DefaultConnectorTypeProperty =
            DependencyProperty.Register(SfDiagramConstants.DefaultConnectorType, typeof(ConnectorType), typeof(SfDiagram), new PropertyMetadata(ConnectorType.Orthogonal, OnDefaultConnectorTypeChanged));

        private static void OnDefaultConnectorTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnDefaultConnectorTypeChanged(e);
        }	
	
    #endregion

        #region SelectedItems

		
		public object SelectedItems
        {
            get { return (object)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(SfDiagramConstants.SelectedItems, typeof(object), typeof(SfDiagram), new PropertyMetadata(null, OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnSelectedItemsChanged(e);
        }	
	
    #endregion

        #region Constraints

		
		public GraphConstraints Constraints
        {
            get { return (GraphConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        public static readonly DependencyProperty ConstraintsProperty =
            DependencyProperty.Register(SfDiagramConstants.Constraints, typeof(GraphConstraints), typeof(SfDiagram), new PropertyMetadata(GraphConstraints.Default, OnConstraintsChanged));

        private static void OnConstraintsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnConstraintsChanged(e);
        }	
	
    #endregion

        #region BezierSmoothness

		
		public BezierSmoothness BezierSmoothness
        {
            get { return (BezierSmoothness)GetValue(BezierSmoothnessProperty); }
            set { SetValue(BezierSmoothnessProperty, value); }
        }

        public static readonly DependencyProperty BezierSmoothnessProperty =
            DependencyProperty.Register(SfDiagramConstants.BezierSmoothness, typeof(BezierSmoothness), typeof(SfDiagram), new PropertyMetadata(BezierSmoothness.None, OnBezierSmoothnessChanged));

        private static void OnBezierSmoothnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnBezierSmoothnessChanged(e);
        }	
	
    #endregion

        #region Tool

		
		public Tool Tool
        {
            get { return (Tool)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register(SfDiagramConstants.Tool, typeof(Tool), typeof(SfDiagram), new PropertyMetadata(Tool.MultipleSelect, OnToolChanged));

        private static void OnToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnToolChanged(e);
        }	
	
    #endregion

        #region DrawingTool

		
		public DrawingTool DrawingTool
        {
            get { return (DrawingTool)GetValue(DrawingToolProperty); }
            set { SetValue(DrawingToolProperty, value); }
        }

        public static readonly DependencyProperty DrawingToolProperty =
            DependencyProperty.Register(SfDiagramConstants.DrawingTool, typeof(DrawingTool), typeof(SfDiagram), new PropertyMetadata(DrawingTool.Connector, OnDrawingToolChanged));

        private static void OnDrawingToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnDrawingToolChanged(e);
        }	
	
    #endregion

        #region MultipleSelectionMode

		
		public MultipleSelectionMode MultipleSelectionMode
        {
            get { return (MultipleSelectionMode)GetValue(MultipleSelectionModeProperty); }
            set { SetValue(MultipleSelectionModeProperty, value); }
        }

        public static readonly DependencyProperty MultipleSelectionModeProperty =
            DependencyProperty.Register(SfDiagramConstants.MultipleSelectionMode, typeof(MultipleSelectionMode), typeof(SfDiagram), new PropertyMetadata(MultipleSelectionMode.Default, OnMultipleSelectionModeChanged));

        private static void OnMultipleSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnMultipleSelectionModeChanged(e);
        }	
	
    #endregion

        #region ViewDictionary

		
		public DataTemplateDictionary ViewDictionary
        {
            get { return (DataTemplateDictionary)GetValue(ViewDictionaryProperty); }
            set { SetValue(ViewDictionaryProperty, value); }
        }

        public static readonly DependencyProperty ViewDictionaryProperty =
            DependencyProperty.Register(SfDiagramConstants.ViewDictionary, typeof(DataTemplateDictionary), typeof(SfDiagram), new PropertyMetadata(null, OnViewDictionaryChanged));

        private static void OnViewDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnViewDictionaryChanged(e);
        }	
	
    #endregion

        #region LayoutManager

		
		public LayoutManager LayoutManager
        {
            get { return (LayoutManager)GetValue(LayoutManagerProperty); }
            set { SetValue(LayoutManagerProperty, value); }
        }

        public static readonly DependencyProperty LayoutManagerProperty =
            DependencyProperty.Register(SfDiagramConstants.LayoutManager, typeof(LayoutManager), typeof(SfDiagram), new PropertyMetadata(null, OnLayoutManagerChanged));

        private static void OnLayoutManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnLayoutManagerChanged(e);
        }	
	
    #endregion

        #region KnownTypes

		
		public GetTypes KnownTypes
        {
            get { return (GetTypes)GetValue(KnownTypesProperty); }
            set { SetValue(KnownTypesProperty, value); }
        }

        public static readonly DependencyProperty KnownTypesProperty =
            DependencyProperty.Register(SfDiagramConstants.KnownTypes, typeof(GetTypes), typeof(SfDiagram), new PropertyMetadata(null, OnKnownTypesChanged));

        private static void OnKnownTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnKnownTypesChanged(e);
        }	
	
    #endregion

        #region SnapSettings

		
		public SnapSettings SnapSettings
        {
            get { return (SnapSettings)GetValue(SnapSettingsProperty); }
            set { SetValue(SnapSettingsProperty, value); }
        }

        public static readonly DependencyProperty SnapSettingsProperty =
            DependencyProperty.Register(SfDiagramConstants.SnapSettings, typeof(SnapSettings), typeof(SfDiagram), new PropertyMetadata(null, OnSnapSettingsChanged));

        private static void OnSnapSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnSnapSettingsChanged(e);
        }	
	
    #endregion

        #region PageSettings

		
		public IPageSettings PageSettings
        {
            get { return (IPageSettings)GetValue(PageSettingsProperty); }
            set { SetValue(PageSettingsProperty, value); }
        }

        public static readonly DependencyProperty PageSettingsProperty =
            DependencyProperty.Register(SfDiagramConstants.PageSettings, typeof(IPageSettings), typeof(SfDiagram), new PropertyMetadata(null, OnPageSettingsChanged));

        private static void OnPageSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnPageSettingsChanged(e);
        }	
	
    #endregion

        #region ExportSettings

		  #if SyncfusionFramework4_5_1 && WINRT		
		
		public ExportSettings ExportSettings
        {
            get { return (ExportSettings)GetValue(ExportSettingsProperty); }
            set { SetValue(ExportSettingsProperty, value); }
        }

        public static readonly DependencyProperty ExportSettingsProperty =
            DependencyProperty.Register(SfDiagramConstants.ExportSettings, typeof(ExportSettings), typeof(SfDiagram), new PropertyMetadata(null, OnExportSettingsChanged));

        private static void OnExportSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnExportSettingsChanged(e);
        }	
		#endif
	
    #endregion

        #region PrintingService

		  #if SyncfusionFramework4_5_1 && WINRT		
		
		public PrintingService PrintingService
        {
            get { return (PrintingService)GetValue(PrintingServiceProperty); }
            set { SetValue(PrintingServiceProperty, value); }
        }

        public static readonly DependencyProperty PrintingServiceProperty =
            DependencyProperty.Register(SfDiagramConstants.PrintingService, typeof(PrintingService), typeof(SfDiagram), new PropertyMetadata(null, OnPrintingServiceChanged));

        private static void OnPrintingServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnPrintingServiceChanged(e);
        }	
		#endif
	
    #endregion

        #region HorizontalRuler

		
		public Ruler HorizontalRuler
        {
            get { return (Ruler)GetValue(HorizontalRulerProperty); }
            set { SetValue(HorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty HorizontalRulerProperty =
            DependencyProperty.Register(SfDiagramConstants.HorizontalRuler, typeof(Ruler), typeof(SfDiagram), new PropertyMetadata(null, OnHorizontalRulerChanged));

        private static void OnHorizontalRulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnHorizontalRulerChanged(e);
        }	
	
    #endregion

        #region VerticalRuler

		
		public Ruler VerticalRuler
        {
            get { return (Ruler)GetValue(VerticalRulerProperty); }
            set { SetValue(VerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty VerticalRulerProperty =
            DependencyProperty.Register(SfDiagramConstants.VerticalRuler, typeof(Ruler), typeof(SfDiagram), new PropertyMetadata(null, OnVerticalRulerChanged));

        private static void OnVerticalRulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDiagram).OnVerticalRulerChanged(e);
        }	
	
    #endregion

	}
}
