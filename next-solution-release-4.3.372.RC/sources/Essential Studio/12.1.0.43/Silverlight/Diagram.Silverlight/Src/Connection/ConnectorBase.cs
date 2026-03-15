// <copyright file="ConnectorBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents base abstract class for Connectors.
    /// </summary>
    public abstract partial class ConnectorBase : ContentControl, IEdge, ICommon, INodeGroup
    {
        internal PathGeometry VirtualConnectorPathGeometry
        {
            get;
            set;
        }

        internal int zOrder { get { return Canvas.GetZIndex(this); } }


        internal Point m_TempStart
        {
            get;
            set;
        }

        internal Point m_TempEnd
        {
            get;
            set;
        }

        public bool IsDecoratorMovable
        {
            get
            {
                return (bool)GetValue(IsDecoratorMovableProperty);
            }

            set
            {
                SetValue(IsDecoratorMovableProperty, value);
            }
        }

        public bool IsDecoratorVisible
        {
            get
            {
                return (bool)GetValue(IsDecoratorVisibleProperty);
            }

            set
            {
                SetValue(IsDecoratorVisibleProperty, value);
            }
        }

        public string SerializationData
        {
            get { return (string)GetValue(SerializationDataProperty); }
            set { SetValue(SerializationDataProperty, value); }
        }

        public static readonly DependencyProperty SerializationDataProperty = DependencyProperty.Register("SerializationData", typeof(string), typeof(ConnectorBase), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsDecoratorMovableProperty = DependencyProperty.Register("IsDecoratorMovable", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(true));

        public static readonly DependencyProperty IsDecoratorVisibleProperty = DependencyProperty.Register("IsDecoratorVisible", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(true, new PropertyChangedCallback(OnIsDecoretorVisibleChanged)));

        private static void OnIsDecoretorVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as LineConnector).IsDecoratorVisible)
            {
                (d as LineConnector).HeadDecoratorGrid.Children[1].Visibility = Visibility.Visible;
                (d as LineConnector).TailDecoratorGrid.Children[1].Visibility = Visibility.Visible;
            }
            else
            {
                (d as LineConnector).HeadDecoratorGrid.Children[1].Visibility = Visibility.Collapsed;
                (d as LineConnector).TailDecoratorGrid.Children[1].Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [line bridging is enabled].
        /// </summary>
        /// <value><c>true</c> if [line bridging is enabled]; otherwise, <c>false</c>.</value>
        public bool LineBridgingEnabled
        {
            get
            {
                return (bool)GetValue(LineBridgingEnabledProperty);
            }

            set
            {
                SetValue(LineBridgingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value of type TextDecorations (static class)
        /// </summary>

        public TextDecorationCollection LabelTextDecoration
        {
            get { return (TextDecorationCollection)GetValue(LabelTextDecorationProperty); }
            set { SetValue(LabelTextDecorationProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelTextDecoration dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextDecorationProperty =
            DependencyProperty.Register("LabelTextDecoration", typeof(TextDecorationCollection), typeof(ConnectorBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the LabelWidth LineBridgingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty LineBridgingEnabledProperty = DependencyProperty.Register("LineBridgingEnabled", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false, new PropertyChangedCallback(OnLineBridgingEnabledChanged)));

        /// <summary>
        /// Identifies the LineRoutingEabled dependency property.
        /// </summary>
        public static readonly DependencyProperty LineRoutingEnabledProperty = DependencyProperty.Register("LineRoutingEnabled", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false, new PropertyChangedCallback(OnLineRoutingChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line routing enabled.
        /// Default value is true.
        /// </summary>
        public bool LineRoutingEnabled
        {
            get
            {
                return (bool)GetValue(LineRoutingEnabledProperty);
            }

            set
            {
                SetValue(LineRoutingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Called when [line bridging enabled changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineBridgingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector l = d as LineConnector;
            if (l != null && l.bridged)
            {
                l.UpdateConnectorPathGeometry();
                if (l.dview != null && l.dview.Page != null)
                {
                    l.dview.Page.InvalidateMeasure();
                }
            }
        }

        internal double BridgeSpacing { get; set; }
        private static void OnLineRoutingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is LineConnector)
            {
                (d as LineConnector).InvalidateVertexs(d as LineConnector);
            }
        }


        /// <summary>
        /// Identifies the Center dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(ConnectorBase), new PropertyMetadata(new Point(0, 0)));

        /// <summary>
        /// Identifies the ConnectionHeadPort dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectionHeadPortProperty = DependencyProperty.Register("ConnectionHeadPort", typeof(ConnectionPort), typeof(ConnectorBase), new PropertyMetadata(null, new PropertyChangedCallback(OnHeadPortChanged)));

        /// <summary>
        /// Identifies the ConnectionPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectionPointsProperty = DependencyProperty.Register("ConnectionPoints", typeof(List<Point>), typeof(ConnectorBase), new PropertyMetadata(new List<Point>()));

        /// <summary>
        /// Identifies the ConnectionTailPort dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectionTailPortProperty = DependencyProperty.Register("ConnectionTailPort", typeof(ConnectionPort), typeof(ConnectorBase), new PropertyMetadata(null, new PropertyChangedCallback(OnTailPortChanged)));

        /// <summary>
        /// Identifies current ConnectorType.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectorTypeProperty = DependencyProperty.Register("ConnectorType", typeof(ConnectorType), typeof(ConnectorBase), new PropertyMetadata(ConnectorType.Orthogonal, new PropertyChangedCallback(OnConnectorTypeChanged)));

        /// <summary>
        /// Identifies the Distance dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LabelAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty EditorAngleProperty = DependencyProperty.Register("EditorAngle", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies current HeadDecoratorPosition.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadDecoratorPositionProperty = DependencyProperty.Register("HeadDecoratorPosition", typeof(Point), typeof(ConnectorBase), new PropertyMetadata(new Point(0, 0),OnDecoratorPositionChanged));
        
        /// <summary>
        /// Identifies current HeadDecoratorShape.  This is a dependency property.
        /// </summary>        
        public static readonly DependencyProperty HeadDecoratorShapeProperty =
            DependencyProperty.Register("HeadDecoratorShape", typeof(DecoratorShape), typeof(ConnectorBase), new PropertyMetadata(DecoratorShape.None, new PropertyChangedCallback(OnHeadShapeChanged)));
        
        /// <summary>
        /// Identifies current HeadNode.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadNodeProperty = DependencyProperty.Register("HeadNode", typeof(Node), typeof(ConnectorBase), new PropertyMetadata(null, new PropertyChangedCallback(OnHeadNodeChanged)));

        /// <summary>
        /// Identifies the IsGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsLabelEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLabelEditableProperty = DependencyProperty.Register("IsLabelEditable", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(true));

        /// <summary>
        /// Identifies current IntermediatePoints.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IntermediatePointsProperty = DependencyProperty.Register("IntermediatePoints", typeof(List<Point>), typeof(ConnectorBase), new PropertyMetadata(new PropertyChangedCallback(OnIntermediatePointsChanged)));

        /// <summary>
        /// Identifies the IsVertexVisibleProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVertexVisibleProperty = DependencyProperty.Register("IsVertexVisible", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(true, new PropertyChangedCallback(OnIsVertexVisibleChanged)));

        /// <summary>
        /// Identifies the IsVertexMovable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVertexMovableProperty = DependencyProperty.Register("IsVertexMovable", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(true, new PropertyChangedCallback(OnIsVertexMovableChanged)));

        /// <summary>
        /// Identifies the VertexStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty VertexStyleProperty = DependencyProperty.Register("VertexStyle", typeof(Style), typeof(ConnectorBase), new PropertyMetadata(new PropertyChangedCallback(OnVertexStyleChanged)));

        public static readonly DependencyProperty DecoratorAdornerStyleProperty = DependencyProperty.Register("DecoratorAdornerStyle", typeof(Style), typeof(ConnectorBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the LabelAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelAngleChanged)));

        public static readonly DependencyProperty IsLabelDragableProperty = DependencyProperty.Register("IsLabelDragable", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false));
        /// <summary>
        ///  Identifies the LabelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(ConnectorBase), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the LabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(ConnectorBase), new PropertyMetadata(new FontFamily("Verdana")));

        /// <summary>
        /// Identifies the LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(ConnectorBase), new PropertyMetadata(11d));

        /// <summary>
        /// Identifies the LabelFontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(ConnectorBase), new PropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// Identifies the LabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ConnectorBase), new PropertyMetadata(FontWeights.SemiBold));

        /// <summary>
        ///  Identifies the LabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(ConnectorBase), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the LabelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register("LabelHeight", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LabelHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(ConnectorBase), new PropertyMetadata(HorizontalAlignment.Center));
        /// <summary>
        /// Identifies the LabelTemplatePosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplatePositionProperty = DependencyProperty.Register("LabelTemplatePosition", typeof(Point), typeof(ConnectorBase), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnLabelTemplatePositionChanged)));

        /// <summary>
        /// Identifies the LabelTemplateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateAngleProperty = DependencyProperty.Register("LabelTemplateAngle", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(Point), typeof(ConnectorBase), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnLabelPositionChanged)));
        public static readonly DependencyProperty AllowCustomLabelPositionProperty = DependencyProperty.Register("AllowCustomLabelPosition", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ConnectorBase), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        /// Identifies the LabelHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateHorizontalAlignmentProperty = DependencyProperty.Register("LabelTemplateHorizontalAlignment", typeof(HorizontalAlignment), typeof(ConnectorBase), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the Label Template.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ConnectorBase), new PropertyMetadata(new DataTemplate()));

        /// <summary>
        /// Identifies the LabelVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateVerticalAlignmentProperty = DependencyProperty.Register("LabelTemplateVerticalAlignment", typeof(VerticalAlignment), typeof(ConnectorBase), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the LabelTextAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextAlignmentProperty = DependencyProperty.Register("LabelTextAlignment", typeof(TextAlignment), typeof(ConnectorBase), new PropertyMetadata(TextAlignment.Center));

        /// <summary>
        /// Identifies the LabelTextWrapping dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextWrappingProperty = DependencyProperty.Register("LabelTextWrapping", typeof(TextWrapping), typeof(ConnectorBase), new PropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// Identifies the LabelVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty = DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(ConnectorBase), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the LabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(ConnectorBase), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty AutoAdjustPointsProperty = DependencyProperty.Register("AutoAdjustPoints", typeof(bool), typeof(ConnectorBase), new PropertyMetadata(false));

        public static readonly DependencyProperty FirstSegmentLengthProperty = DependencyProperty.Register("FirstSegmentLength", typeof(double), typeof(ConnectorBase), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty LastSegmentLengthProperty = DependencyProperty.Register("LastSegmentLength", typeof(double), typeof(ConnectorBase), new PropertyMetadata(double.NaN));

        public double FirstSegmentLength
        {
            get
            {
                return (double)GetValue(FirstSegmentLengthProperty);
            }

            set
            {
                SetValue(FirstSegmentLengthProperty, value);
            }
        }

        public double LastSegmentLength
        {
            get
            {
                return (double)GetValue(LastSegmentLengthProperty);
            }

            set
            {
                SetValue(LastSegmentLengthProperty, value);
            }
        }

        public bool AutoAdjustPoints
        {
            get
            {
                return (bool)GetValue(AutoAdjustPointsProperty);
            }

            set
            {
                SetValue(AutoAdjustPointsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LabelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(ConnectorBase), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the MeasurementUnit dependency property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitProperty = DependencyProperty.Register("MeasurementUnit", typeof(MeasureUnits), typeof(ConnectorBase), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnUnitsChanged)));

        /// <summary>
        /// Identifies the ParentId dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(ConnectorBase), new PropertyMetadata(new Guid()));

        /// <summary>
        /// Identifies current TailDecoratorPosition.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TailDecoratorPositionProperty = DependencyProperty.Register("TailDecoratorPosition", typeof(Point), typeof(ConnectorBase), new PropertyMetadata(new Point(0, 0)));
        /// <summary>
        /// Identifies current TailDecoratorShape.  This is a dependency property.
        /// </summary>
     
        public static readonly DependencyProperty TailDecoratorShapeProperty =
            DependencyProperty.Register("TailDecoratorShape", typeof(DecoratorShape), typeof(ConnectorBase), new PropertyMetadata(DecoratorShape.Arrow, new PropertyChangedCallback(OnTailShapeChanged)));

        /// <summary>
        /// Identifies current TailNode.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TailNodeProperty = DependencyProperty.Register("TailNode", typeof(Node), typeof(ConnectorBase), new PropertyMetadata(null, new PropertyChangedCallback(OnTailNodeChanged)));

        /// <summary>
        /// Defines the TextWidth property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWidthProperty = DependencyProperty.Register("TextWidth", typeof(double), typeof(ConnectorBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnTextWidthChanged)));


        /// <summary>
        /// Used to set LabelOrientation of the Line Connector.
        /// </summary>
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(LabelOrientation), typeof(ConnectorBase), new PropertyMetadata(LabelOrientation.Auto, new PropertyChangedCallback(OnLabelOrientationChanged)));

        public static readonly DependencyProperty CustomLabelPositionProperty = DependencyProperty.Register("CustomLabelPosition", typeof(CustomLabelPositions), typeof(ConnectorBase), new PropertyMetadata(CustomLabelPositions.Auto, new PropertyChangedCallback(OnCustomLabelPositionChanged)));

        /// <summary>
        /// LabelTemplateOrientationProperty
        /// </summary>
        public static readonly DependencyProperty LabelTemplateOrientationProperty = DependencyProperty.Register("LabelTemplateOrientation", typeof(LabelOrientation), typeof(ConnectorBase), new PropertyMetadata(LabelOrientation.Auto, new PropertyChangedCallback(OnLabelTemplateOrientationChanged)));

        /// <summary>
        /// Used to store start point position value.
        /// </summary>
        internal Point mstartpointposition = new Point(0, 0);

        /// <summary>
        /// Used to store end point position value.
        /// </summary>
        internal Point mendpointposition = new Point(0, 0);

        internal bool DragCancel
        {
            get;
            set;
        }

        internal Point ep, sp;

        /// <summary>
        /// Used to store the groups.
        /// </summary>
        private CollectionExt mgroups = new CollectionExt();
        
        /// <summary>
        /// Used to store the head decorator style property values.
        /// </summary>
        private DecoratorStyle mheadDecoratorStyle;

        /// <summary>
        /// Used to store the new ZIndex value.
        /// </summary>
        private int mnewindex = 0;

        /// <summary>
        /// Used to store the old ZIndex value.
        /// </summary>
        private int moldindex = 0;

        /// <summary>
        /// Used to store the view object.
        /// </summary>
        private DiagramView dview;

        /// <summary>
        /// Used to store head node reference no.
        /// </summary>
        private int hid = -1;

        /// <summary>
        /// Used to store head port reference no.
        /// </summary>
        private int hpid;

        /// <summary>
        /// Used to store ISelected property setting value.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Used to store the line style property values.
        /// </summary>
        private LineStyle lineStyle;

        /// <summary>
        /// Used to store the connector drop point.
        /// </summary>
        private Point mdroppoint = new Point(0, 0);
        
        /// <summary>
        /// Used to store the tail decorator style property values.
        /// </summary>
        private DecoratorStyle mtailDecoratorStyle;

        /// <summary>
        /// Used to store IsDirected property information.
        /// </summary>
        private bool isDirected = false;

        /// <summary>
        /// Used to store the reference number of the nodes and connectors.
        /// </summary>
        private int no = -1;

        /// <summary>
        /// Used to store tail node reference no.
        /// </summary>
        private int tid = -1;

        /// <summary>
        /// Used to store tail port reference no.
        /// </summary>
        private int tpid;

        private DiagramModel m_Model;

        internal void setMode(DiagramModel model)
        {
            m_Model = model;
        }

        protected TreeOrientation Orientation
        {
            get
            {
                if (m_Model != null)
                {
                    return m_Model.Orientation;
                }
                else
                {
                    return TreeOrientation.TopBottom;
                }
            }
        }

        private bool LineVirtual = true;

        public bool AllowVirtualization
        {
            get { return LineVirtual; }
            set { LineVirtual = value; }

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        private bool _FirstLoaded;
        public ConnectorBase()
        {
            this.InitializeRelationship();
            this.Unloaded += new RoutedEventHandler(ConnectorBase_Unloaded);
            this.Loaded += new RoutedEventHandler(FirstLoaded);
            this.BridgeSpacing = 15d;
        }

        void FirstLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(FirstLoaded);
            _FirstLoaded = true;
        }

        void ConnectorBase_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded += new RoutedEventHandler(ConnectorBase_Loaded);
            if (this.HeadNode != null)
            {
                (HeadNode as Node).SizeChanged -= new SizeChangedEventHandler(ConnectorBase_SizeChanged);
                (this.HeadNode as Node).PropertyChanged -= Line_PropertyChanged;
            }

            if (this.TailNode != null)
            {
                (TailNode as Node).SizeChanged -= new SizeChangedEventHandler(ConnectorBase_SizeChanged);
                (this.TailNode as Node).PropertyChanged -= Line_PropertyChanged;
            }
        }

        void ConnectorBase_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ConnectorBase_Loaded;
            if (this.HeadNode != null)
            {
                (HeadNode as Node).SizeChanged += new SizeChangedEventHandler(ConnectorBase_SizeChanged);
                (this.HeadNode as Node).PropertyChanged += Line_PropertyChanged;
            }

            if (this.TailNode != null)
            {
                (TailNode as Node).SizeChanged += new SizeChangedEventHandler(ConnectorBase_SizeChanged);
                (this.TailNode as Node).PropertyChanged += Line_PropertyChanged;
            }

        }

        /// <summary>
        /// Calls property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when HeadNode is changed. 
        /// </summary>
        internal event DependencyPropertyChangedEventHandler HeadNodeChangedEvent;

        /// <summary>
        /// Occurs when TailNode is changed. 
        /// </summary>
        internal event DependencyPropertyChangedEventHandler TailNodeChangedEvent;

        /// <summary>
        /// Gets or sets the head port of the connector.
        /// </summary>
        /// <value>The port to which the connection is to be made.</value>
        /// <remarks>
        /// When specifying the <see cref="ConnectionHeadPort"/>, the <see cref="HeadNode"/> should also be specified.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creating node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// //Adding a port to the node
        /// ConnectionPort port = new ConnectionPort();
        /// port.Node=n;
        /// port.Left=75;
        /// port.Top=10;
        /// port.PortShape = PortShapes.Arrow;
        /// port.PortStyle.Fill = Brushes.Transparent;
        /// port.Height = 11;
        /// port.Width = 11;
        /// n.Ports.Add(port);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// ConnectionPort port1 = new ConnectionPort();
        /// port1.Node=n;
        /// port1.Left=75;
        /// port1.Top=50;
        /// port1.PortShape = PortShapes.Arrow;
        /// port1.PortStyle.Fill = Brushes.Transparent;
        /// port1.Height = 11;
        /// port1.Width = 11;
        /// n1.Ports.Add(port1);
        /// //Creating a connection.
        /// LineConnector o2 = new LineConnector();
        /// o2.ConnectorType = ConnectorType.Straight;
        /// o2.TailNode = n1;
        /// o2.HeadNode = n;
        /// o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        /// //Specifying the port to connect to.
        /// o2.ConnectionHeadPort = port;
        /// o2.ConnectionTailPort = port1;
        /// o2.HeadDecoratorShape=DecoratorShape.Arrow;
        /// o2.TailDecoratorShape=DecoratorShape.Arrow;
        /// Model.Connections.Add(o2);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ConnectionPort"/>
        public ConnectionPort ConnectionHeadPort
        {
            get
            {
                return (ConnectionPort)GetValue(ConnectionHeadPortProperty);
            }

            set
            {
                SetValue(ConnectionHeadPortProperty, value);
                this.OnPropertyChanged("ConnectionHeadPort");
                this.UpdateConnectorPathGeometry();
            }
        }


        /// <summary>
        /// Gets or sets the tail port of the connector.
        /// </summary>
        /// <value>The port to which the connection is to be made.</value>
        /// <remarks>
        /// When specifying the <see cref="ConnectionTailPort"/>, the <see cref="TailNode"/> should also be specified.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creating node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// //Adding a port to the node
        /// ConnectionPort port = new ConnectionPort();
        /// port.Node=n;
        /// port.Left=75;
        /// port.Top=10;
        /// port.PortShape = PortShapes.Arrow;
        /// port.PortStyle.Fill = Brushes.Transparent;
        /// port.Height = 11;
        /// port.Width = 11;
        /// n.Ports.Add(port);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// ConnectionPort port1 = new ConnectionPort();
        /// port1.Node=n;
        /// port1.Left=75;
        /// port1.Top=50;
        /// port1.PortShape = PortShapes.Arrow;
        /// port1.PortStyle.Fill = Brushes.Transparent;
        /// port1.Height = 11;
        /// port1.Width = 11;
        /// n1.Ports.Add(port1);
        /// //Creating a connection.
        /// LineConnector o2 = new LineConnector();
        /// o2.ConnectorType = ConnectorType.Straight;
        /// o2.TailNode = n1;
        /// o2.HeadNode = n;
        /// o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        /// //Specifying the port to connect to.
        /// o2.ConnectionHeadPort = port;
        /// o2.ConnectionTailPort = port1;
        /// Model.Connections.Add(o2);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public ConnectionPort ConnectionTailPort
        {
            get
            {
                return (ConnectionPort)GetValue(ConnectionTailPortProperty);
            }

            set
            {
                SetValue(ConnectionTailPortProperty, value);
                this.OnPropertyChanged("ConnectionTailPort");
                this.UpdateConnectorPathGeometry();
            }
        }

        /// <summary>
        /// Gets or sets the type of connection to be used.This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ConnectorType"/>
        /// Enum specifying the type of the connector to be used.
        /// </value>
        /// <remarks>
        /// Three types of connectors are provided namely Orthogonal, Bezier and Straight. Default value is Orthogonal.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set ConnectorType in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="ConnectorType"/>
        public ConnectorType ConnectorType
        {
            get
            {
                return (ConnectorType)GetValue(ConnectorTypeProperty);
            }

            set
            {
                SetValue(ConnectorTypeProperty, value);
            }
        }

        internal Point PxEndPointPosition
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(EndPointPosition, MeasurementUnit);
            }
            set
            {
                if (this.dview != null && (this as LineConnector).linedragging)
                {
                    if (this.dview.SnapToHorizontalGrid)
                    {
                        value.X = Node.Round(value.X, dview.PxSnapOffsetX);
                    }
                    if (this.dview.SnapToVerticalGrid)
                    {
                        value.Y = Node.Round(value.Y, dview.PxSnapOffsetY);
                    }
                }
                EndPointPosition = MeasureUnitsConverter.FromPixels(value, MeasurementUnit);
            }
        }

        /// <summary>
        /// Gets or sets the end point position.
        /// </summary>
        /// <value>The end point position.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       //Specifies the node
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///         LineConnector o = new LineConnector();
        ///         o.ConnectorType = ConnectorType.Straight;
        ///         o.TailNode = n1;
        ///         o.HeadNode = n;
        ///         o.StartPointPosition=new Point(100,100);
        ///         o.EndPointPosition=new Point(200,200);
        ///         Model.Connections.Add(o);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Point EndPointPosition
        {
            get { return this.mendpointposition; }
            set
            {
                if (dview != null && EndPointPosition != value)
                {
                    if (dview.m_IsCommandInProgress)
                    {
                        if (!dview.Undone && !dview.Redone)
                        {
                            dview.tUndoStack.Push(new LineOperation(LineOperations.Dragged, this as LineConnector));
                        }
                    }
                } this.mendpointposition = value;
            }
        }

        /// <summary>
        /// Gets the groups to which the INodeGroup objects belong.
        /// </summary>
        /// <value>The groups.</value>
        public CollectionExt Groups
        {
            get
            {
                return this.mgroups;
            }
        }

        /// <summary>
        /// Gets or sets the point where the head decorator is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// The point of the head decorator position.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ConnectorType in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.HeadDecoratorPosition = new Point(100,100);
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Point HeadDecoratorPosition
        {
            get
            {
                return (Point)GetValue(HeadDecoratorPositionProperty);
            }

            set
            {
                SetValue(HeadDecoratorPositionProperty, value);
            }
        }

        public double ArcHeight
        {
            get { return (double)GetValue(ArcHeightProperty); }
            set { SetValue(ArcHeightProperty, value); }
        }

        public static readonly DependencyProperty ArcHeightProperty =
            DependencyProperty.Register("ArcHeight", typeof(double), typeof(ConnectorBase), new PropertyMetadata(50d, new PropertyChangedCallback(OnArcHeightChanged)));

        public SweepDirection ArcDirection
        {
            get { return (SweepDirection)GetValue(ArcDirectionProperty); }
            set { SetValue(ArcDirectionProperty, value); }
        }

        public static readonly DependencyProperty ArcDirectionProperty =
            DependencyProperty.Register("ArcDirection", typeof(SweepDirection), typeof(ConnectorBase), new PropertyMetadata(SweepDirection.Clockwise, new PropertyChangedCallback(OnArcDirectionChanged)));

        private static void OnArcHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dc = DiagramPage.GetDiagramControl(d as LineConnector);
            if (dc != null && !dc.View.Undone && !dc.View.Redone && !dc.View.IsLayout)
            {
                LineOperation oper = new LineOperation(LineOperations.Resized, d as LineConnector);
                oper.ArcHeight = (double)e.OldValue;
                dc.View.tUndoStack.Push(oper);

                //dc.View.tUndoStack.Push(new LineOperation(LineOperations.Rotated, d as LineConnector));
            }
        }

        private static void OnDecoratorPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
        private static void OnArcDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dc = DiagramPage.GetDiagramControl(d as LineConnector);
            if (dc != null && !dc.View.Undone && !dc.View.Redone && !dc.View.IsLayout)
            {
                LineOperation oper = new LineOperation(LineOperations.Rotated, d as LineConnector);
                oper.ArcDirection = (SweepDirection)e.OldValue;
                dc.View.tUndoStack.Push(oper);

                //dc.View.tUndoStack.Push(new LineOperation(LineOperations.Rotated, d as LineConnector));
            }
        }

        internal IShape hn, tn;

        /// <summary>
        /// Gets or sets the shape to be used as the head decorator.
        /// </summary>
        /// <value>
        /// Type: <see cref="DecoratorShape"/>
        /// Enum specifying the shape of the head decorator.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set HeadDecoratorShape in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.HeadDecoratorShape = DecoratorShape.Arrow;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        /// Several shapes like None, Arrow, Diamond and Circle have been provided. Default shape is None.
        /// </remarks>
        /// <seealso cref="DecoratorShape"/>
        /// 
        public DecoratorShape HeadDecoratorShape
        {
            get {
                return (DecoratorShape)GetValue(HeadDecoratorShapeProperty); 
            }
            set { 
                SetValue(HeadDecoratorShapeProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the  style to be used for the head decorator.
        /// </summary>
        /// <value>
        /// Type: <see cref="DecoratorStyle"/>
        /// HeadDecoratorStyle for the connector.
        /// </value>
        /// <remarks>
        /// The decorator shapes can be customized by using the various DecoratorStyle properties like Fill, Stroke, StrokeThickness, StrokeStartLineCap, StrokeEndLineCap, StrokeLineJoin .
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set HeadDecoratorStyle in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LineStyle.Fill = Brushes.Red;
        /// connObject.HeadDecoratorStyle.Fill = Brushes.Orange; 
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DecoratorStyle"/>
        public DecoratorStyle HeadDecoratorStyle
        {
            get
            {
                if (this.mheadDecoratorStyle != null)
                {
                    return this.mheadDecoratorStyle;
                }
                else
                {
                    this.mheadDecoratorStyle = new DecoratorStyle();
                }

                return this.mheadDecoratorStyle;
            }

            set
            {
                this.mheadDecoratorStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the first, or source, node upon which this Edge is incident.
        /// </summary>
        /// <value>The head node of the connection</value>
        /// <remarks>
        /// Every node should have a unique name.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creating node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// //Creating a connection.
        /// LineConnector o2 = new LineConnector();
        /// o2.ConnectorType = ConnectorType.Straight;
        /// o2.TailNode = n1;
        /// o2.HeadNode = n;
        /// Model.Connections.Add(o2);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public IShape HeadNode
        {
            get
            {
                return (IShape)GetValue(HeadNodeProperty);
            }

            set
            {
                SetValue(HeadNodeProperty, value);
                this.OnPropertyChanged("HeadNode");
                this.UpdateConnectorPathGeometry();
            }
        }

        public CustomLabelPositions CustomLabelPosition
        {
            get
            {
                return (CustomLabelPositions)GetValue(CustomLabelPositionProperty);
            }

            set
            {
                SetValue(CustomLabelPositionProperty, value);
            }
        }


        public LabelOrientation LabelOrientation
        {
            get
            {
                return (LabelOrientation)GetValue(LabelOrientationProperty);
            }
            set
            {
                SetValue(LabelOrientationProperty, value);
            }
        }


        public LabelOrientation LabelTemplateOrientation
        {
            get
            {
                return (LabelOrientation)GetValue(LabelTemplateOrientationProperty);
            }
            set
            {
                SetValue(LabelTemplateOrientationProperty, value);
            }
        }




        /// <summary>
        /// Gets or sets the head node reference no.
        /// </summary>
        /// <value>The head node reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int HeadNodeReferenceNo
        {
            get
            {
                if (this.HeadNode != null)
                {
                    return this.HeadNode.ReferenceNo;
                }
                else
                {
                    return hid;
                }
            }

            set { this.hid = value; }
        }

        /// <summary>
        /// Gets or sets the head port reference no.
        /// </summary>
        /// <value>The head port reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int HeadPortReferenceNo
        {
            get
            {
                if (this.ConnectionHeadPort != null)
                {
                    return this.ConnectionHeadPort.PortReferenceNo;
                }
                else
                {
                    return hpid;
                }
            }
            set { this.hpid = value; }
        }
        internal string m_ReferenceID;
        /// <summary>
        /// This is used for Internal Purpose to save the GUID value
        /// </summary>
        public string ReferenceID
        {
            get { return this.m_ReferenceID; }
            set { this.m_ReferenceID = value; }
        }

        /// <summary>
        /// Gets or sets a unique identifier for the connector.
        /// </summary>
        /// <value>
        /// Type: <see cref="Guid"/>
        /// Unique ID for the connector.
        /// </value>
        public Guid ID { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Layout is directed or not.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True if the layout is directed, false otherwise.
        /// </value>
        public bool IsDirected
        {
            get { return this.isDirected; }
            set { this.isDirected = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector is grouped.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grouped; otherwise, <c>false</c>.
        /// </value>
        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label editable.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// True, if it can be edited, false otherwise.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set IsLabelEditable in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelVisibility = Visibility.Visible;
        /// connObject.IsLabelEitable = true;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Default Value is true. When this is false, HitTest is also set to false.
        /// When set to true, clicking on the label will make the editable textbox visible.
        /// Enter the new label and press ENTER to apply the changed label,
        /// or press ESC to ignore the new label and revert back to the old one.
        /// </remarks>
        public bool IsLabelEditable
        {
            get { return (bool)GetValue(IsLabelEditableProperty); }
            set { SetValue(IsLabelEditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the intermediate points.
        /// </summary>
        /// <value>The intermediate points.</value>        
        public List<Point> IntermediatePoints
        {
            get
            {
                return (List<Point>)GetValue(IntermediatePointsProperty);
            }

            set
            {
                SetValue(IntermediatePointsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is vertex visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is vertex visible; otherwise, <c>false</c>.
        /// </value>        
        public bool IsVertexVisible
        {
            get
            {
                return (bool)GetValue(IsVertexVisibleProperty);
            }

            set
            {
                SetValue(IsVertexVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is vertex movable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is vertex movable; otherwise, <c>false</c>.
        /// </value>        
        public bool IsVertexMovable
        {
            get
            {
                return (bool)GetValue(IsVertexMovableProperty);
            }

            set
            {
                SetValue(IsVertexMovableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertex style.
        /// </summary>
        /// <value>The vertex style.</value>
        public Style VertexStyle
        {
            get
            {
                return (Style)GetValue(VertexStyleProperty);
            }

            set
            {
                SetValue(VertexStyleProperty, value);
            }
        }

        public Style DecoratorAdornerStyle
        {
            get
            {
                return (Style)GetValue(DecoratorAdornerStyleProperty);
            }

            set
            {
                SetValue(DecoratorAdornerStyleProperty, value);
            }
        }

        /// <summary>
        /// Called when [intermediate points changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIntermediatePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ConnectorBase cbase = d as ConnectorBase;
            //if (e.NewValue is string)
            //{

            //}
        }

        /// <summary>
        /// Called when [is vertex visible changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsVertexVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is LineConnector)
            {
                (d as LineConnector).InvalidateVertexs(d as LineConnector);
            }
        }

        /// <summary>
        /// Called when [is vertex movable changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsVertexMovableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is LineConnector)
            {
                (d as LineConnector).InvalidateVertexs(d as LineConnector);
            }
        }

        /// <summary>
        /// Called when [vertex style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVertexStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ConnectorBase line = d as ConnectorBase;
            //if (line != null)
            //{
            //    (line as LineConnector).InvalidateVertexs((line as LineConnector));
            //}
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector has been selected or not.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True if the connector is selected, false otherwise.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set  LabelTemplate   in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.IsSelected = true;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Label for the connector.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Label for the connector.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set Label in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.Label="Syncfusion";
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label background.
        /// </summary>
        /// <value>The label background. Default value is White</value>
        /// <example>
        /// <para/>This example shows how to set LabelBackground in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.LabelBackground=Brushes.Beige;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush LabelBackground
        {
            get
            {
                return (Brush)GetValue(LabelBackgroundProperty);
            }

            set
            {
                SetValue(LabelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font family.
        /// </summary>
        /// <value>Default value is Arial.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelFontFamily = new FontFamily("Verdana");
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontFamily LabelFontFamily
        {
            get
            {
                return (FontFamily)GetValue(LabelFontFamilyProperty);
            }

            set
            {
                SetValue(LabelFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font size.
        /// </summary>
        /// <value>Default value is 11d.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelFontSize = 14;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }

            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font style.
        /// </summary>
        /// <value>Default value is Normal.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelFontStyle = FontStyles.Italic;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontStyle LabelFontStyle
        {
            get
            {
                return (FontStyle)GetValue(LabelFontStyleProperty);
            }

            set
            {
                SetValue(LabelFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font weight.
        /// </summary>
        /// <value>Default value is SemiBold.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelFontWeight = FontWeights.Bold;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontWeight LabelFontWeight
        {
            get
            {
                return (FontWeight)GetValue(LabelFontWeightProperty);
            }

            set
            {
                SetValue(LabelFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Label Foreground.
        /// </summary>
        /// <value>The label foreground. Default value is Black.</value>
        /// <example>
        /// <para/>This example shows how to set LabelForeground in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.LabelForeground=Brushes.Beige;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush LabelForeground
        {
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }

            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets  the LabelTemplate for the connector.This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// LabelTemplate for the connector.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set  LabelTemplate   in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelTemplate = (ControlTemplate)FindResource( "LabelCustomTemplate" );
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// <para/>This example shows how to write a  LabelTemplate in XAML.
        /// <code language="XAML">
        /// &lt;ControlTemplate x:Key="LabelCustomTemplate"&gt;
        /// &lt;StackPanel Orientation="Horizontal"&gt;
        /// &lt;Image Source="text.png" Width="20" Height="20"/&gt;
        /// &lt;TextBlock Text="Hello"/&gt;
        /// &lt;/StackPanel&gt;
        /// &lt;/ControlTemplate&gt;
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public DataTemplate LabelTemplate
        {
            get
            {
                return (DataTemplate)GetValue(LabelTemplateProperty);
            }

            set
            {
                SetValue(LabelTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label template horizontal alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// Enum specifying the alignment position.
        /// </value>
        /// <remarks>Default HorizontalAlignment is at the Center.</remarks>
        /// <example>
        /// <para/>This example shows how to set LabelTemplateHorizontalAlignment in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.Label="Syncfusion";
        /// connObject.LabelTemplateHorizontalAlignment= HorizontalAlignment.Left;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public HorizontalAlignment LabelTemplateHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(LabelTemplateHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(LabelTemplateHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label template vertical alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalAlignment"/>
        /// Enum specifying the alignment position.
        /// </value>
        /// <remarks>Default VerticalAlignment is at the Top.</remarks>
        /// <example>
        /// <para/>This example shows how to set LabelTemplateVerticalAlignment in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelTemplateVerticalAlignment= VerticalAlignment.Left;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public VerticalAlignment LabelTemplateVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(LabelTemplateVerticalAlignmentProperty);
            }

            set
            {
                SetValue(LabelTemplateVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text alignment.
        /// </summary>
        /// <value>Default value is Center.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelTextAlignment = TextAlignment.Left;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public TextAlignment LabelTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(LabelTextAlignmentProperty);
            }

            set
            {
                SetValue(LabelTextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text wrapping.
        /// </summary>
        /// <value>Default value is NoWrap.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelTextWrapping = TextWrapping.Wrap;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public TextWrapping LabelTextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(LabelTextWrappingProperty);
            }

            set
            {
                SetValue(LabelTextWrappingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label vertical alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalAlignment"/>
        /// Enum specifying the alignment position.
        /// </value>
        /// <remarks>Default VerticalAlignment is at the Top.</remarks>
        /// <example>
        /// <para/>This example shows how to set LabelVerticalAlignment in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.Label="Syncfusion";
        /// connObject.LabelVerticalAlignment= VerticalAlignment.Left;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public VerticalAlignment LabelVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty);
            }

            set
            {
                SetValue(LabelVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label visibility.
        /// </summary>
        /// <value>
        /// Type: <see cref="Visibility"/>
        /// Enum specifying the visibility.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LabelVisibility in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.LabelVisibility = Visibility.Visible;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        /// By default label visibility is set to visible.
        /// </remarks>
        public Visibility LabelVisibility
        {
            get
            {
                return (Visibility)GetValue(LabelVisibilityProperty);
            }

            set
            {
                SetValue(LabelVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>The width of the label. By default it is set to the line width.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LabelWidth=50;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }

            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the line style to be used for the connector.
        /// </summary>
        /// <value>
        /// Type: <see cref="LineStyle"/>
        /// LineStyle for the connector.
        /// </value>
        /// <remarks>
        /// The line connectors can be customized by using the various LineStyle properties like Fill, Stroke, StrokeThickness, StrokeStartLineCap, StrokeEndLineCap, StrokeLineJoin .
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set LineStyle in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LineStyle.Fill = Brushes.Red;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="LineStyle"/>
        public LineStyle LineStyle
        {
            get
            {
                if (this.lineStyle != null)
                {
                    return this.lineStyle;
                }
                else
                {
                    this.lineStyle = new LineStyle(this);
                }

                return this.lineStyle;
            }

            set
            {
                this.lineStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the label horizontal alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// Enum specifying the alignment position.
        /// </value>
        /// <remarks>Default HorizontalAlignment is at the Center. This property will take effect only if the LabelWidth is set.</remarks>
        /// <example>
        /// <para/>This example shows how to set LabelHorizontalAlignment in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.Label="Syncfusion";
        /// connObject.LabelHorizontalAlignment= HorizontalAlignment.Left;
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(LabelHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the new ZIndex value.
        /// </summary>
        /// <value>The new ZIndex value.</value>
        public int NewZIndex
        {
            get { return this.mnewindex; }
            set { this.mnewindex = value; }
        }

        /// <summary>
        /// Gets or sets the old ZIndex value.
        /// </summary>
        /// <value>The old ZIndex value.</value>
        public int OldZIndex
        {
            get { return this.moldindex; }
            set { this.moldindex = value; }
        }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }

        /// <summary>
        /// Gets or sets the reference number of the INodeGroup objects. Used for serialization purposes..
        /// </summary>
        /// <value>The reference no.</value>
        public int ReferenceNo
        {
            get { return this.no; }
            set { this.no = value; }
        }

        internal Point PxStartPointPosition
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(StartPointPosition, MeasurementUnit);
            }
            set
            {
                if (this.dview != null && (this as LineConnector).linedragging)
                {
                    if (this.dview.SnapToHorizontalGrid)
                    {
                        value.X = Node.Round(value.X, dview.PxSnapOffsetX);
                    }
                    if (this.dview.SnapToVerticalGrid)
                    {
                        value.Y = Node.Round(value.Y, dview.PxSnapOffsetY);
                    }
                }
                StartPointPosition = MeasureUnitsConverter.FromPixels(value, MeasurementUnit);
            }
        }

        /// <summary>
        /// Gets or sets the start point position.
        /// </summary>
        /// <value>The start point position.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       //Specifies the node
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///         LineConnector o = new LineConnector();
        ///         o.ConnectorType = ConnectorType.Straight;
        ///         o.TailNode = n1;
        ///         o.HeadNode = n;
        ///         o.StartPointPosition=new Point(100,100);
        ///         o.EndPointPosition=new Point(200,200);
        ///         Model.Connections.Add(o);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Point StartPointPosition
        {
            get { return this.mstartpointposition; }
            set
            {
                if (dview != null && StartPointPosition != value)
                {
                    if (dview.m_IsCommandInProgress)
                    {
                        if (!dview.Undone && !dview.Redone)
                        {
                            dview.tUndoStack.Push(new LineOperation(LineOperations.Dragged, this as LineConnector));
                        }
                    }
                }
                this.mstartpointposition = value;
            }
        }

        /// <summary>
        /// Gets or sets the point where the tail decorator is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// The point of the tail decorator position.
        /// </value>
        public Point TailDecoratorPosition
        {
            get
            {
                return (Point)GetValue(TailDecoratorPositionProperty);
            }

            set
            {
                SetValue(TailDecoratorPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the shape to be used as the tail decorator.
        /// </summary>
        /// <value>
        /// Type: <see cref="DecoratorShape"/>
        /// Enum specifying the shape of the tail decorator.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set TailDecoratorShape in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.Label = "Alarm Rings";
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///         Model.Nodes.Add(n1);
        ///         LineConnector connObject = new LineConnector();
        ///         connObject.ConnectorType = ConnectorType.Straight;
        ///         connObject.TailNode = n1;
        ///         connObject.HeadNode = n;
        ///         connObject.ConnectorType = ConnectorType.Orthogonal;
        ///         connObject.TailDecoratorShape = DecoratorShape.Arrow;
        ///         Model.Connections.Add(connObject);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        /// Several shapes like None, Arrow, Diamond and Circle have been provided. Default shape is Arrow.
        /// </remarks>
        /// <seealso cref="DecoratorShape"/>
        /// 
        public DecoratorShape TailDecoratorShape
        {
            get { return (DecoratorShape)GetValue(TailDecoratorShapeProperty); }
            set { SetValue(TailDecoratorShapeProperty, value); }
        }
      
        /// <summary>
        /// Gets or sets the  style to be used for the tail decorator.
        /// </summary>
        /// <value>
        /// Type: <see cref="DecoratorStyle"/>
        /// TailDecoratorStyle for the connector.
        /// </value>
        /// <remarks>
        /// The decorator shapes can be customized by using the various DecoratorStyle properties like Fill, Stroke, StrokeThickness, StrokeStartLineCap, StrokeEndLineCap, StrokeLineJoin .
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set TailDecoratorStyle in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.Label = "Alarm Rings";
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// LineConnector connObject = new LineConnector();
        /// connObject.ConnectorType = ConnectorType.Straight;
        /// connObject.TailNode = n1;
        /// connObject.HeadNode = n;
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// connObject.LineStyle.Fill = Brushes.Red;
        /// connObject.TailDecoratorStyle.Fill = Brushes.Orange; 
        /// Model.Connections.Add(connObject);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DecoratorStyle"/>
        public DecoratorStyle TailDecoratorStyle
        {
            get
            {
                if (this.mtailDecoratorStyle != null)
                {
                    return this.mtailDecoratorStyle;
                }
                else
                {
                    this.mtailDecoratorStyle = new DecoratorStyle();
                }

                return this.mtailDecoratorStyle;
            }

            set
            {
                this.mtailDecoratorStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the second, or target, node upon which this Edge is incident.
        /// </summary>
        /// <value>The tail node of the connection</value>
        /// <remarks>
        /// Every Node should have unique name.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creating node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// //Creating a connection.
        /// LineConnector o2 = new LineConnector();
        /// o2.ConnectorType = ConnectorType.Straight;
        /// o2.TailNode = n1;
        /// o2.HeadNode = n;
        /// Model.Connections.Add(o2);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public IShape TailNode
        {
            get
            {
                return (IShape)GetValue(TailNodeProperty);
            }

            set
            {
                SetValue(TailNodeProperty, value);
                this.OnPropertyChanged("TailNode");
                this.UpdateConnectorPathGeometry();
            }
        }

        /// <summary>
        /// Gets or sets the tail node reference no.
        /// </summary>
        /// <value>The tail node reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int TailNodeReferenceNo
        {
            get
            {
                if (this.TailNode != null)
                {
                    return this.TailNode.ReferenceNo;
                }
                else
                {
                    return tid;
                }
            }

            set { this.tid = value; }
        }

        /// <summary>
        /// Gets or sets the tail port reference no.
        /// </summary>
        /// <value>The tail port reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int TailPortReferenceNo
        {
            get
            {
                if (this.ConnectionTailPort != null)
                {
                    return this.ConnectionTailPort.PortReferenceNo;
                }
                else
                {
                    return tpid;
                }
            }
            set { this.tpid = value; }
        }

        /// <summary>
        /// Gets or sets the list of connection points
        /// </summary>
        internal List<Point> ConnectionPoints
        {
            get
            {
                return (List<Point>)GetValue(ConnectionPointsProperty);
            }

            set
            {
                SetValue(ConnectionPointsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance of the label from the nodes.
        /// </summary>
        internal double Distance
        {
            get
            {
                return (double)GetValue(DistanceProperty);
            }

            set
            {
                SetValue(DistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the point at which the Connector was dropped.
        /// </summary>
        /// <value>The drop point.</value>
        internal Point DropPoint
        {
            get
            {
                return this.mdroppoint;
            }

            set
            {
                this.mdroppoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle at which the Label is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// The angle .
        /// </value>
        public double LabelAngle
        {
            get
            {
                return (double)GetValue(LabelAngleProperty);
            }

            set
            {
                SetValue(LabelAngleProperty, value);
            }
        }

        internal bool IsLabelDragable
        {
            get { return (bool)GetValue(IsLabelDragableProperty); }
            set { SetValue(IsLabelDragableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the label.
        /// </summary>
        /// <value>The height of the label.</value>
        public double LabelHeight
        {
            get
            {
                return (double)GetValue(LabelHeightProperty);
            }

            set
            {
                SetValue(LabelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label template angle.
        /// </summary>
        /// <value>The label template angle.</value>
        internal double LabelTemplateAngle
        {
            get
            {
                return (double)GetValue(LabelTemplateAngleProperty);
            }

            set
            {
                SetValue(LabelTemplateAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the point where the Label is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// The point of the Label position.
        /// </value>
        public Point LabelPosition
        {
            get
            {
                return (Point)GetValue(LabelPositionProperty);
            }

            set
            {
                SetValue(LabelPositionProperty, value);
            }
        }

        internal bool AllowCustomLabelPosition
        {
            get
            {
                return (bool)GetValue(AllowCustomLabelPositionProperty);
            }

            set
            {
                SetValue(AllowCustomLabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label template position.
        /// </summary>
        /// <value>The label template position.</value>
        internal Point LabelTemplatePosition
        {
            get
            {
                return (Point)GetValue(LabelTemplatePositionProperty);
            }

            set
            {
                SetValue(LabelTemplatePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the measurement unit.
        /// <value>
        /// Type: <see cref="MeasureUnits"/>
        /// Current Measurement unit.
        /// </value>
        /// </summary>
        internal MeasureUnits MeasurementUnit
        {
            get
            {
                return (MeasureUnits)GetValue(MeasurementUnitProperty);
            }

            set
            {
                SetValue(MeasurementUnitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text width.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// </value>
        public double TextWidth
        {
            get
            {
                return (double)GetValue(TextWidthProperty);
            }

            set
            {
                SetValue(TextWidthProperty, value);
            }
        }

        internal void SetSerializationData()
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectorStatePersistence));
            ConnectorStatePersistence content = new ConnectorStatePersistence();
            content.SetIntermediatePoints(this.IntermediatePoints);
            serializer.WriteObject(ms, content);
            ms.Position = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(ms);
            SerializationData = sr.ReadToEnd();
        }

        internal void RetrieveSerializationData()
        {
            if (!this.SerializationData.Equals(String.Empty))
            {
                ConnectorStatePersistence content = new ConnectorStatePersistence();
                System.IO.StringReader sr = new System.IO.StringReader(this.SerializationData);
                System.Xml.XmlReader xr = System.Xml.XmlReader.Create(sr);
                DataContractSerializer serializer = new DataContractSerializer(typeof(ConnectorStatePersistence));
                content = serializer.ReadObject(xr) as ConnectorStatePersistence;

                if (this.IntermediatePoints != null)
                {
                    IntermediatePoints = new List<Point>();
                }
                IntermediatePoints.Clear();
                foreach (Point pt in content.IntermediatePoints)
                {
                    this.IntermediatePoints.Add(pt);
                }
            }
        }

        /// <summary>
        /// Calculates the intersection point of the line with any of the node sides.
        /// </summary>
        /// <param name="node">The node with which the line intersects.</param>
        /// <param name="pt1">The start point of line.</param>
        /// <param name="pt2">The end point of the line.</param>
        /// <param name="rect">The rectangle which contains the node.</param>
        /// <param name="isTop">Flag to indicate the top side.</param>
        /// <param name="isBottom">Flag to indicate the bottom side.</param>
        /// <param name="isLeft">Flag to indicate the left side.</param>
        /// <param name="isRight">Flag to indicate the right side.</param>
        /// <param name="conType">Specifies the ConnectorType.</param>
        /// <returns>Intersection Point</returns>
        public static Point GetLineIntersect(NodeInfo node, Point pt1, Point pt2, Rect rect, out bool isTop, out bool isBottom, out bool isLeft, out bool isRight, ConnectorType conType)
        {
            NodeInfo n = node;
            Point intersectpoint = node.Position;
            isTop = false;
            isBottom = false;
            isLeft = false;
            isRight = false;
            bool isVerticalLine = false;
            double m = 0;
            double xintercept;
            double yintercept;

            double rheight = rect.Height;// MeasureUnitsConverter.ToPixels(rect.Height, n.MeasurementUnit);
            double rwidth = rect.Width;// MeasureUnitsConverter.ToPixels(rect.Width, n.MeasurementUnit);
            double rctop = node.Position.Y - (rheight / 2);
            double rcbottom = node.Position.Y + (rheight / 2);
            double rcleft = node.Position.X - (rwidth / 2);
            double rcright = node.Position.X + (rwidth / 2);

            if ((pt2.X - pt1.X) != 0)
            {
                m = LineSlope(pt1, pt2);
            }
            else
            {
                isVerticalLine = true;
            }

            ////Test top side.

            if ((pt1.Y <= rctop && pt2.Y >= rctop) || (pt2.Y <= rctop && pt1.Y >= rctop))
            {
                if (isVerticalLine)
                {
                    xintercept = pt1.X;
                }
                else
                {
                    xintercept = (double)((rctop + ((m * pt1.X) - pt1.Y)) / m);
                }

                if (xintercept >= rcleft && xintercept <= rcright)
                {
                    if (conType != ConnectorType.Straight)
                    {
                        intersectpoint = new Point(rcleft + (rwidth / 2), rctop);
                    }
                    else
                    {
                        intersectpoint = new Point(xintercept, rctop);
                    }

                    isTop = true;
                }
            }

            ////Test bottom side.

            if ((pt1.Y <= rcbottom && pt2.Y >= rcbottom) || (pt2.Y <= rcbottom && pt1.Y >= rcbottom))
            {
                if (isVerticalLine)
                {
                    xintercept = pt1.X;
                }
                else
                {
                    xintercept = (double)((rcbottom + ((m * pt1.X) - pt1.Y)) / m);
                }

                if (xintercept >= rcleft && xintercept <= rcright)
                {
                    if (conType != ConnectorType.Straight)
                    {
                        intersectpoint = new Point(rcleft + (rwidth / 2), rcbottom);
                    }
                    else
                    {
                        intersectpoint = new Point(xintercept, rcbottom);
                    }

                    isBottom = true;
                }
            }

            ////Test left side.

            if ((!isVerticalLine && (pt1.X <= rcleft && pt2.X >= rcleft)) || (pt2.X <= rcleft && pt1.X >= rcleft))
            {
                yintercept = (double)((m * (rcleft - pt1.X)) + pt1.Y);

                if (yintercept >= rctop && yintercept <= rcbottom)
                {
                    if (conType != ConnectorType.Straight)
                    {
                        intersectpoint = new Point(rcleft, rctop + (rheight / 2));
                    }
                    else
                    {
                        intersectpoint = new Point(rcleft, yintercept);
                    }

                    isLeft = true;
                }
            }

            //// Test right side.

            if ((!isVerticalLine && (pt1.X <= rcright && pt2.X >= rcright)) || (pt2.X <= rcright && pt1.X >= rcright))
            {
                yintercept = (double)((m * (rcright - pt1.X)) + pt1.Y);

                if (yintercept >= rctop && yintercept <= rcbottom)
                {
                    if (conType != ConnectorType.Straight)
                    {
                        intersectpoint = new Point(rcright, rctop + (rheight / 2));
                    }
                    else
                    {
                        intersectpoint = new Point(rcright, yintercept);
                    }

                    isRight = true;
                }
            }

            return intersectpoint;
        }

        /// <summary>
        /// Calculates the intersection point of the orthogonal or Bezier line with any of the node sides.
        /// </summary>
        /// <param name="source">The head node.</param>
        /// <param name="target">The tail node.</param>
        /// <param name="rect">The rectangle which contains the head node.</param>
        /// <param name="trect">The rectangle which contains the tail node.</param>
        /// <param name="isTop">Flag to indicate the top side of rect.</param>
        /// <param name="isBottom">Flag to indicate the bottom side of rect.</param>
        /// <param name="isLeft">Flag to indicate the left side of rect.</param>
        /// <param name="isRight">Flag to indicate the right side of rect.</param>
        /// <param name="tisTop">Flag to indicate the top side of target rectangle.</param>
        /// <param name="tisBottom">Flag to indicate the bottom side of target rectangle.</param>
        /// <param name="tisLeft">Flag to indicate the left side of target rectangle.</param>
        /// <param name="tisRight">Flag to indicate the right side of target rectangle.</param>
        /// <param name="si">The intersection point with respect to head node.</param>
        /// <param name="ti">The intersection point with respect to tail node.</param>
        public static void GetOrthogonalLineIntersect(NodeInfo source, NodeInfo target, Rect rect, Rect trect, out bool isTop, out bool isBottom, out bool isLeft, out bool isRight, out bool tisTop, out bool tisBottom, out bool tisLeft, out bool tisRight, out Point si, out Point ti)
        {
            si = new Point(0, 0);
            ti = new Point(0, 0);
            isTop = false;
            isBottom = false;
            isLeft = false;
            isRight = false;
            tisTop = false;
            tisBottom = false;
            tisLeft = false;
            tisRight = false;
            double sheight = rect.Height;// MeasureUnitsConverter.ToPixels(rect.Height, source.MeasurementUnit);
            double swidth = rect.Width;// MeasureUnitsConverter.ToPixels(rect.Width, source.MeasurementUnit);

            double theight = trect.Height;// MeasureUnitsConverter.ToPixels(trect.Height, target.MeasurementUnit);
            double twidth = trect.Width;// MeasureUnitsConverter.ToPixels(trect.Width, target.MeasurementUnit);
            double rctop = source.Position.Y - (sheight / 2);
            double rcbottom = source.Position.Y + (sheight / 2);
            double rcleft = source.Position.X - (swidth / 2);
            double rcright = source.Position.X + (swidth / 2);
            double trctop = target.Position.Y - (theight / 2);
            double trcbottom = target.Position.Y + (theight / 2);
            double trcleft = target.Position.X - (twidth / 2);
            double trcright = target.Position.X + (twidth / 2);

            ////Test top side.
            if (rctop >= trcbottom)
            {
                si = new Point(rcleft + (swidth / 2), rctop);
                isTop = true;
                ti = new Point(trcleft + (twidth / 2), trcbottom);
                tisBottom = true;
            }

            ////Test bottom side.
            if (rcbottom <= trctop)
            {
                si = new Point(rcleft + (swidth / 2), rcbottom);
                isBottom = true;
                ti = new Point(trcleft + (twidth / 2), trctop);
                tisTop = true;
            }

            if ((rcbottom > trctop) && (rctop < trcbottom))
            {
                if (rcright >= trcleft)
                {
                    if (rcleft <= trcleft)
                    {
                        ////left left
                        si = new Point(rcleft, rctop + (sheight / 2));
                        isLeft = true;
                        ti = new Point(trcleft, trctop + (theight / 2));
                        tisLeft = true;
                    }

                    if (rcleft <= trcright)
                    {
                        ////right right
                        si = new Point(rcright, rctop + (sheight / 2));
                        isRight = true;
                        ti = new Point(trcright, trctop + (theight / 2));
                        tisRight = true;
                    }
                    else
                    {
                        ////left right
                        si = new Point(rcleft, rctop + (sheight / 2));
                        isLeft = true;
                        ti = new Point(trcright, trctop + (theight / 2));
                        tisRight = true;
                    }
                }
                else
                {
                    {
                        ////rightleft
                        si = new Point(rcright, rctop + (sheight / 2));
                        isRight = true;
                        ti = new Point(trcleft, trctop + (theight / 2));
                        tisLeft = true;
                    }
                }
            }
        }

        public static void GetArcLineIntersect(NodeInfo source, NodeInfo target, Rect rect, Rect trect, out bool isTop, out bool isBottom, out bool isLeft, out bool isRight, out bool tisTop, out bool tisBottom, out bool tisLeft, out bool tisRight, out Point si, out Point ti, SweepDirection arcDirection)
        {
            si = new Point(0, 0);
            ti = new Point(0, 0);

            isTop = false;
            isBottom = false;
            isLeft = false;
            isRight = false;
            tisTop = false;
            tisBottom = false;
            tisLeft = false;
            tisRight = false;

            double sheight = rect.Height;
            double swidth = rect.Width;

            double theight = trect.Height;
            double twidth = trect.Width;

            double rcTop = source.Position.Y - sheight / 2;
            double rcBottom = source.Position.Y + sheight / 2;
            double rcLeft = source.Position.X - swidth / 2;
            double rcRight = source.Position.X + swidth / 2;

            double trcTop = target.Position.Y - theight / 2;
            double trcBottom = target.Position.Y + theight / 2;
            double trcLeft = target.Position.X - twidth / 2;
            double trcRight = target.Position.X + twidth / 2;

            double angle = FindAngle(source.Position, target.Position);

            if (arcDirection == SweepDirection.Clockwise)
            {
                if (angle >= 0 && angle <= 45 || angle > 320 && angle <= 360)
                {
                    si = new Point(rcLeft + swidth / 2, rcTop);
                    isTop = true;
                    ti = new Point(trcLeft + twidth / 2, trcTop);
                    tisTop = true;
                }
                else if (angle > 45 && angle <= 140)
                {
                    si = new Point(rcRight, rcTop + sheight / 2);
                    isRight = true;
                    ti = new Point(trcRight, trcTop + theight / 2);
                    tisRight = true;
                }
                else if (angle > 140 && angle <= 220)
                {
                    si = new Point(rcLeft + swidth / 2, rcBottom);
                    isBottom = true;
                    ti = new Point(trcLeft + twidth / 2, trcBottom);
                    tisBottom = true;
                }
                else if (angle > 220 && angle <= 320)
                {
                    si = new Point(rcLeft, rcTop + sheight / 2);
                    isLeft = true;
                    ti = new Point(trcLeft, trcTop + theight / 2);
                    tisLeft = true;
                }
            }
            else
            {
                if (angle >= 0 && angle <= 45 || angle > 320 && angle <= 360)
                {
                    si = new Point(rcLeft + swidth / 2, rcBottom);
                    isBottom = true;
                    ti = new Point(trcLeft + twidth / 2, trcBottom);
                    tisBottom = true;
                }
                else if (angle > 45 && angle <= 140)
                {
                    si = new Point(rcLeft, rcTop + sheight / 2);
                    isLeft = true;
                    ti = new Point(trcLeft, trcTop + theight / 2);
                    tisLeft = true;
                }
                else if (angle > 140 && angle <= 220)
                {
                    si = new Point(rcLeft + swidth / 2, rcTop);
                    isTop = true;
                    ti = new Point(trcLeft + twidth / 2, trcTop);
                    tisTop = true;
                }
                else if (angle > 220 && angle <= 320)
                {
                    si = new Point(rcRight, rcTop + sheight / 2);
                    isRight = true;
                    ti = new Point(trcRight, trcTop + theight / 2);
                    tisRight = true;
                }
            }
        }

        /// <summary>
        /// Gets the tree orthogonal line intersect.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <param name="rect">The source rect.</param>
        /// <param name="trect">The target target rectangle.</param>
        /// <param name="isTop">if set to <c>true</c> [is top].</param>
        /// <param name="isBottom">if set to <c>true</c> [is bottom].</param>
        /// <param name="isLeft">if set to <c>true</c> [is left].</param>
        /// <param name="isRight">if set to <c>true</c> [is right].</param>
        /// <param name="tisTop">if set to <c>true</c> [tis top].</param>
        /// <param name="tisBottom">if set to <c>true</c> [tis bottom].</param>
        /// <param name="tisLeft">if set to <c>true</c> [tis left].</param>
        /// <param name="tisRight">if set to <c>true</c> [tis right].</param>
        /// <param name="si">The start point.</param>
        /// <param name="ti">The end point.</param>
        public static void GetTreeOrthogonalLineIntersect(NodeInfo source, NodeInfo target, Rect rect, Rect trect, out bool isTop, out bool isBottom, out bool isLeft, out bool isRight, out bool tisTop, out bool tisBottom, out bool tisLeft, out bool tisRight, out Point si, out Point ti)
        {
            si = new Point(0, 0);
            ti = new Point(0, 0);
            isTop = false;
            isBottom = false;
            isLeft = false;
            isRight = false;
            tisTop = false;
            tisBottom = false;
            tisLeft = false;
            tisRight = false;
            double sheight = rect.Height;
            double swidth = rect.Width;
            double theight = trect.Height;
            double twidth = trect.Width;
            double rctop = source.Position.Y - (sheight / 2);
            double rcbottom = source.Position.Y + (sheight / 2);
            double rcleft = source.Position.X - (swidth / 2);
            double rcright = source.Position.X + (swidth / 2);
            double trctop = target.Position.Y - (theight / 2);
            double trcbottom = target.Position.Y + (theight / 2);
            double trcleft = target.Position.X - (twidth / 2);
            double trcright = target.Position.X + (twidth / 2);

            if (rcright >= trcleft)
            {
                if (rcleft <= trcleft)
                {
                    ////left left
                    si = new Point(rcleft, rctop + (sheight / 2));
                    isLeft = true;
                    ti = new Point(trcleft, trctop + (theight / 2));
                    tisLeft = true;
                }
                else if (rcleft <= trcright)
                {
                    ////right right
                    si = new Point(rcright, rctop + (sheight / 2));
                    isRight = true;
                    ti = new Point(trcright, trctop + (theight / 2));
                    tisRight = true;
                }
                else
                {
                    ////left right
                    si = new Point(rcleft, rctop + (sheight / 2));
                    isLeft = true;
                    ti = new Point(trcright, trctop + (theight / 2));
                    tisRight = true;
                }
            }
            else
            {
                ////rightleft
                si = new Point(rcright, rctop + (sheight / 2));
                isRight = true;
                ti = new Point(trcleft, trctop + (theight / 2));
                tisLeft = true;
            }
        }

        /// <summary>
        /// Given a Node upon which this Edge is incident, the opposite incident
        /// Node is returned. Throws an exception if the input node is not incident
        /// on this Edge.
        /// </summary>
        /// <param name="node">The node whose adjacent node is to be found</param>
        /// <returns>The node at the other end.</returns>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creating node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// //Creating a connection.
        /// LineConnector o2 = new LineConnector();
        /// o2.ConnectorType = ConnectorType.Straight;
        /// o2.TailNode = n1;
        /// o2.HeadNode = n;
        /// IShape node = o2.AdjacentNode(n1);
        /// Model.Connections.Add(o2);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public IShape AdjacentNode(IShape node)
        {
            if (this.HeadNode != null && (node as IShape) == this.HeadNode)
            {
                if (this.TailNode == null)
                {
                    return null;
                }
                else
                {
                    return this.TailNode as IShape;
                }
            }
            else if (this.TailNode != null && (node as IShape) == this.TailNode)
            {
                if (this.HeadNode == null)
                {
                    return null;
                }
                else
                {
                    return this.HeadNode as IShape;
                }
            }
            else
            {
                throw new Exception("The given node is not part of the edge.");
            }
        }

        //private bool m_IsPixelDefultUnit = true;
        internal bool m_LineDrawing = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dview = DiagramPage.GetDiagramControl((FrameworkElement)this).View;
            //this.MeasurementUnit = (this.dview.Page as DiagramPage).MeasurementUnits;

            if (dview != null && dview.Page != null)
            {
                //if (this.MeasurementUnit != (dview.Page as DiagramPage).MeasurementUnits)
                //{
                //    m_IsPixelDefultUnit = false;
                //}
                System.Windows.Data.Binding measure = new System.Windows.Data.Binding("MeasurementUnits");
                measure.Source = dview.Page;
                this.SetBinding(ConnectorBase.MeasurementUnitProperty, measure);

                if (this.ReadLocalValue(LineConnector.LineBridgingEnabledProperty) == DependencyProperty.UnsetValue)
                {
                    System.Windows.Data.Binding lb = new System.Windows.Data.Binding("LineBridgingEnabled");
                    lb.Source = dview;
                    this.SetBinding(ConnectorBase.LineBridgingEnabledProperty, lb);
                }

                if (this.ReadLocalValue(LineConnector.LineRoutingEnabledProperty) == DependencyProperty.UnsetValue)
                {
                    System.Windows.Data.Binding lr = new System.Windows.Data.Binding("LineRoutingEnabled");
                    lr.Source = dview;
                    this.SetBinding(ConnectorBase.LineRoutingEnabledProperty, lr);
                }
            }
        }

        /// <summary>
        /// Updates the line geometry.
        /// </summary>
        public virtual void UpdateConnectorPathGeometry()
        {
        }

        /// <summary>
        /// Calculates the slope.
        /// </summary>
        /// <param name="pt1">The start Point </param>
        /// <param name="pt2">The end point</param>
        /// <returns>The Slope value</returns>
        internal static double LineSlope(Point pt1, Point pt2)
        {
            double m = 0;
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            if (dx != 0)
            {
                m = dy / dx;
            }
            else
            {
                throw new SlopeUndefinedException();
            }

            return m;
        }

        /// <summary>
        /// Updates the position of the decorator.
        /// </summary>
        /// <param name="line">line connector</param>
        internal void UpdateDecoratorPosition(LineConnector line)
        {
            List<Point> pts = new List<Point>();
            pts = line.ConnectionPoints;

            if (pts.Count > 0)
            {
                line.HeadDecoratorPosition = pts[0];
                //new Point(pts[0].X, pts[0].Y);
                line.TailDecoratorPosition = pts[pts.Count - 1];
                //new Point(pts[pts.Count - 1].X + 2, pts[pts.Count - 1].Y - 2);

                if (ConnectorType == ConnectorType.Straight || ConnectorType == ConnectorType.Orthogonal)
                {
                    if (IntermediatePoints != null && IntermediatePoints.Count > 0)
                    {
                        line.HeadDecoratorAngle = FindAngle(pts[1], pts[0]);
                        line.TailDecoratorAngle = FindAngle(pts[pts.Count - 2], pts[pts.Count - 1]);
                    }
                    else
                    {
                        line.HeadDecoratorAngle = FindAngle(pts[pts.Count - 1], pts[0]);
                        line.TailDecoratorAngle = FindAngle(pts[0], pts[pts.Count - 1]);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the shape.
        /// </summary>
        /// <param name="shape">sets the shape</param>
        internal void SetShape(string shape)
        {

            if (shape == "Head" && (this as LineConnector).HeadShape as Path != null)
            {
                if (HeadDecoratorShape == DecoratorShape.Arrow)
                {
                    ((this as LineConnector).HeadShape as Path).Style = GetArrowStyle((this as LineConnector).HeadDecoratorStyle.Size.Height, (this as LineConnector).HeadDecoratorStyle.Size.Width);//;rs["Arrow"] as Style;
                }
                else if (this.HeadDecoratorShape == DecoratorShape.Diamond)
                {
                    ((this as LineConnector).HeadShape as Path).Style = GetDiamondStyle((this as LineConnector).HeadDecoratorStyle.Size.Height, (this as LineConnector).HeadDecoratorStyle.Size.Width);//rs["Diamond"] as Style;
                }
                else if (this.HeadDecoratorShape == DecoratorShape.Circle)
                {
                    ((this as LineConnector).HeadShape as Path).Style = GetCircleStyle((this as LineConnector).HeadDecoratorStyle.Size.Height, (this as LineConnector).HeadDecoratorStyle.Size.Width);//rs["Circle"] as Style;
                }
                else if (this.HeadDecoratorShape == DecoratorShape.Custom)
                {
                    Style CustomStyle = new Style(typeof(Path));
                    CustomStyle.Setters.Add(new Setter(Path.DataProperty, (this as LineConnector).HeadDecoratorStyle.Data));
                    CustomStyle.Setters.Add(new Setter(Path.FillProperty, (this as LineConnector).HeadDecoratorStyle.Fill));
                    CustomStyle.Setters.Add(new Setter(Path.StrokeProperty, (this as LineConnector).HeadDecoratorStyle.Stroke));
                    CustomStyle.Setters.Add(new Setter(Path.StretchProperty, (this as LineConnector).HeadDecoratorStyle.Stretch));
                    CustomStyle.Setters.Add(new Setter(Path.HeightProperty, (this as LineConnector).HeadDecoratorStyle.Size.Height));
                    CustomStyle.Setters.Add(new Setter(Path.WidthProperty, (this as LineConnector).HeadDecoratorStyle.Size.Width));
                    ((this as LineConnector).HeadShape as Path).Style = CustomStyle;
                }
                else
                {
                    ((this as LineConnector).HeadShape as Path).Style = null;
                }
                    
            }
            else
            {
                if ((this as LineConnector).TailShape as Path != null)
                {
                    if (this.TailDecoratorShape == DecoratorShape.Arrow)
                    {
                        ((this as LineConnector).TailShape as Path).Style = GetArrowStyle((this as LineConnector).TailDecoratorStyle.Size.Height, (this as LineConnector).TailDecoratorStyle.Size.Width);//;rs["Arrow"] as Style;
                    }
                    else if (this.TailDecoratorShape == DecoratorShape.Diamond)
                    {
                        ((this as LineConnector).TailShape as Path).Style = GetDiamondStyle((this as LineConnector).TailDecoratorStyle.Size.Height, (this as LineConnector).TailDecoratorStyle.Size.Width);// rs["Diamond"] as Style;
                    }
                    else if (this.TailDecoratorShape == DecoratorShape.Circle)
                    {
                        ((this as LineConnector).TailShape as Path).Style = GetCircleStyle((this as LineConnector).TailDecoratorStyle.Size.Height, (this as LineConnector).TailDecoratorStyle.Size.Width);// rs["Circle"] as Style;
                    }
                    else if (this.TailDecoratorShape == DecoratorShape.Custom)
                    {
                        Style CustomStyle = new Style(typeof(Path));
                        CustomStyle.Setters.Add(new Setter(Path.DataProperty, (this as LineConnector).TailDecoratorStyle.Data));
                        CustomStyle.Setters.Add(new Setter(Path.FillProperty, (this as LineConnector).TailDecoratorStyle.Fill));
                        CustomStyle.Setters.Add(new Setter(Path.StrokeProperty, (this as LineConnector).TailDecoratorStyle.Stroke));
                        CustomStyle.Setters.Add(new Setter(Path.StretchProperty, (this as LineConnector).TailDecoratorStyle.Stretch));
                        CustomStyle.Setters.Add(new Setter(Path.HeightProperty, (this as LineConnector).TailDecoratorStyle.Size.Height));
                        CustomStyle.Setters.Add(new Setter(Path.WidthProperty, (this as LineConnector).TailDecoratorStyle.Size.Width));
                        ((this as LineConnector).TailShape as Path).Style = CustomStyle;
                    }
                    else
                    {
                        ((this as LineConnector).TailShape as Path).Style = null;
                    }
                }
            }
        }

        private System.Windows.Style GetCircleStyle(double hei, double wid)
        {
            Style CircleStyle = new Style(typeof(Path));
            CircleStyle.Setters.Add(new Setter(Path.DataProperty, "M5,3C5,4.10456949966159,4.10456949966159,5,3,5C1.89543050033841,5,1,4.10456949966159,1,3C1,1.89543050033841,1.89543050033841,1,3,1C4.10456949966159,1,5,1.89543050033841,5,3z"));
            CircleStyle.Setters.Add(new Setter(Path.FillProperty, new SolidColorBrush(Colors.Gray)));
            CircleStyle.Setters.Add(new Setter(Path.StretchProperty, Stretch.Fill));
            CircleStyle.Setters.Add(new Setter(Path.HeightProperty, hei));
            CircleStyle.Setters.Add(new Setter(Path.WidthProperty, wid));
            return CircleStyle;
        }

        private System.Windows.Style GetDiamondStyle(double hei, double wid)
        {
            Style DiamondStyle = new Style(typeof(Path));
            DiamondStyle.Setters.Add(new Setter(Path.DataProperty, "M-5,0 0,-5 5,0 0,5 Z"));
            DiamondStyle.Setters.Add(new Setter(Path.FillProperty, new SolidColorBrush(Colors.Gray)));
            DiamondStyle.Setters.Add(new Setter(Path.StretchProperty, Stretch.Fill));
            DiamondStyle.Setters.Add(new Setter(Path.HeightProperty, hei));
            DiamondStyle.Setters.Add(new Setter(Path.WidthProperty, wid));
            return DiamondStyle;
        }

        private System.Windows.Style GetArrowStyle(double hei, double wid)
        {
            Style ArrowStyle = new Style(typeof(Path));
            ArrowStyle.Setters.Add(new Setter(Path.DataProperty, "M0,0 8,4 0,8 Z"));
            ArrowStyle.Setters.Add(new Setter(Path.FillProperty, new SolidColorBrush(Colors.Gray)));
            ArrowStyle.Setters.Add(new Setter(Path.StretchProperty, Stretch.Fill));
            ArrowStyle.Setters.Add(new Setter(Path.HeightProperty, hei));
            ArrowStyle.Setters.Add(new Setter(Path.WidthProperty, wid));
            return ArrowStyle;
        }

        /// <summary>
        /// Hides the adorner
        /// </summary>
        protected virtual void HideAdorner()
        {
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Line control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name of the property.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Shows the adorner
        /// </summary>
        protected virtual void ShowAdorner()
        {
        }

        /// <summary>
        /// Called when [connector type changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectorTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ConnectorType && e.NewValue is ConnectorType)
            {
                if (e.OldValue.Equals(ConnectorType.Orthogonal) && e.NewValue.Equals(ConnectorType.Straight))
                {
                    (d as LineConnector).IntermediatePoints = new List<Point>();
                }
                (d as LineConnector).InvalidateVertexs(d as LineConnector);
            }
            (d as LineConnector).UpdateConnectorPathGeometry();
        }

        /// <summary>
        /// Calls OnHeadShapeChanged method of the instance, notifies of the dependency property value changes .
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeadShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            if (!(line as LineConnector).IsOverlapped)
            {
                (line as LineConnector).InternalHeadShape = (DecoratorShape)e.NewValue;
            }


        }
        /// <summary>
        /// Calls OnHeadNodeChanged method of the instance, notifies of the dependency property value changes .
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeadNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NodeChangedRoutedEventArgs newEventArgs;
            ConnectorBase cbase = d as ConnectorBase;
            if (cbase.HeadNodeChangedEvent != null)
            {
                cbase.HeadNodeChangedEvent.Invoke(cbase, e);
            }
            DiagramView dview = Node.GetDiagramView(cbase);
            if (e.OldValue != null && e.NewValue != null && e.OldValue != e.NewValue)
            {
                cbase.ConnectionHeadPort = null;
            }
            cbase.HeadNode = (IShape)e.NewValue;

            if (cbase.HeadNode != null)
            {
                
                if ((Node)e.OldValue != null && (Node)e.NewValue != null)
                {
                    newEventArgs = new NodeChangedRoutedEventArgs((Node)e.OldValue, (Node)e.NewValue, cbase as LineConnector);
                }
                else
                    if ((Node)e.OldValue == null && (Node)e.NewValue != null)
                    {
                        newEventArgs = new NodeChangedRoutedEventArgs((Node)e.NewValue, cbase as LineConnector);
                    }
                    
                    else
                    {
                        newEventArgs = new NodeChangedRoutedEventArgs(cbase as LineConnector);
                    }
                if (dview != null)
                {
                    dview.OnHeadNodeChanged((cbase as LineConnector), newEventArgs);
                }
                (cbase as LineConnector).InternalHeadShape = cbase.HeadDecoratorShape;
                cbase.HeadNodeReferenceNo = cbase.HeadNode.ReferenceNo;
                (cbase.HeadNode as Node).SizeChanged += new SizeChangedEventHandler(cbase.ConnectorBase_SizeChanged);
                (cbase.HeadNode as Node).PropertyChanged += new PropertyChangedEventHandler(cbase.Line_PropertyChanged);

            }

            else
            {
                if ((Node)e.OldValue != null && (Node)e.NewValue == null)
                {
                    newEventArgs = new NodeChangedRoutedEventArgs(cbase as LineConnector, (Node)e.OldValue);
                    if (dview != null)
                    {
                        dview.OnHeadNodeChanged((cbase as LineConnector), newEventArgs);
                    }
                }
                (cbase as LineConnector).TailNodeReferenceNo = -1;
                (e.OldValue as Node).PropertyChanged -= new PropertyChangedEventHandler(cbase.Line_PropertyChanged);
            }
        }

        private void ConnectorBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateConnectorPathGeometry();
        }

        /// <summary>
        /// Called when [head port changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeadPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectorBase line = d as ConnectorBase;
            if (e.NewValue != null)
            {

            }
            line.UpdateConnectorPathGeometry();
        }

        /// <summary>
        /// Called when [label changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string oldvalue = (string)e.OldValue;
            string newvalue = (string)e.NewValue;
            ConnectorBase cbase = d as ConnectorBase;
            DiagramView dview = Node.GetDiagramView(cbase);
            LabelConnRoutedEventArgs newEventArgs;
            if (cbase.HeadNode as Node != null && cbase.TailNode as Node != null)
            {
                newEventArgs = new LabelConnRoutedEventArgs((string)e.OldValue, (string)e.NewValue, cbase.HeadNode as Node, cbase.TailNode as Node, cbase as LineConnector);
            }
            else
                if (cbase.HeadNode as Node == null && cbase.TailNode as Node != null)
                {
                    newEventArgs = new LabelConnRoutedEventArgs((string)e.OldValue, (string)e.NewValue, cbase.TailNode as Node, cbase as LineConnector);
                }
                else if (cbase.HeadNode as Node != null && cbase.TailNode as Node == null)
                {
                    newEventArgs = new LabelConnRoutedEventArgs((string)e.OldValue, (string)e.NewValue, cbase as LineConnector, cbase.HeadNode as Node);
                }
                else
                {
                    newEventArgs = new LabelConnRoutedEventArgs((string)e.OldValue, (string)e.NewValue, cbase as LineConnector);
                }
            if (dview != null)
            {
                dview.OnConnectorLabelChanged((cbase as LineConnector), newEventArgs);
            }
        }

        /// <summary>
        /// Called when [label position changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Point pt = (d as LineConnector).LabelPosition;
            if (pt != null)
            {
                //if (double.IsNaN(pt.X))
                {
                    (d as LineConnector).LabelPosition = new Point((double.IsNaN(pt.X) ? 0 : pt.X), (double.IsNaN(pt.Y) ? 0 : pt.Y));
                }
            }
        }

        /// <summary>
        /// Called when [label template position changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelTemplatePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Calls OnTailShapeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTailShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            if (!(line as LineConnector).IsOverlapped)
            {
                (line as LineConnector).InternalTailShape = (DecoratorShape)e.NewValue;
            }
        }
        /// <summary>
        /// Calls OnTailNodeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTailNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NodeChangedRoutedEventArgs newEventArgs;
            ConnectorBase cbase = d as ConnectorBase;
            if (cbase.TailNodeChangedEvent != null)
            {
                cbase.TailNodeChangedEvent.Invoke(cbase, e);
            }
            DiagramView dview = Node.GetDiagramView(cbase);
            if (e.OldValue != null && e.NewValue != null && e.OldValue != e.NewValue)
            {
                cbase.ConnectionTailPort = null;
            }
            if (cbase.TailNode != null)
            {
                
                if ((Node)e.OldValue != null && (Node)e.NewValue != null)
                {
                    newEventArgs = new NodeChangedRoutedEventArgs((Node)e.OldValue, (Node)e.NewValue, cbase as LineConnector);
                }
                else
                    if ((Node)e.OldValue == null && (Node)e.NewValue != null)
                    {
                        newEventArgs = new NodeChangedRoutedEventArgs((Node)e.NewValue, cbase as LineConnector);
                    }
                    
                    else
                    {
                        newEventArgs = new NodeChangedRoutedEventArgs(cbase as LineConnector);

                    }
                if (dview != null)
                {
                    dview.OnTailNodeChanged(cbase as LineConnector, newEventArgs);
                }
                (cbase as LineConnector).InternalTailShape = cbase.TailDecoratorShape;
                cbase.TailNodeReferenceNo = cbase.TailNode.ReferenceNo;
                (cbase.TailNode as Node).PropertyChanged += new PropertyChangedEventHandler(cbase.Line_PropertyChanged);
                (cbase.TailNode as Node).SizeChanged += new SizeChangedEventHandler(cbase.ConnectorBase_SizeChanged);
            }

            else
            {
                if ((Node)e.OldValue != null && (Node)e.NewValue == null)
                {
                    newEventArgs = new NodeChangedRoutedEventArgs(cbase as LineConnector, (Node)e.OldValue);
                    if (dview != null)
                    {
                        dview.OnTailNodeChanged(cbase as LineConnector, newEventArgs);
                    }
                }
                (cbase as LineConnector).TailNodeReferenceNo = -1;
                (e.OldValue as Node).PropertyChanged -= new PropertyChangedEventHandler(cbase.Line_PropertyChanged);
                
            }
        }

        /// <summary>
        /// Called when [tail port changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTailPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectorBase line = d as ConnectorBase;
            if (e.NewValue != null)
            {

            }
            line.UpdateConnectorPathGeometry();
        }

        /// <summary>
        /// Called when [text width changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTextWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectorBase cbase = d as ConnectorBase;
        }

        public static void OnLabelOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector lc = d as LineConnector;
            if (lc != null)
            {
                if (lc.LabelOrientation == LabelOrientation.Vertical)
                {

                }
            }

        }

        public static void OnLabelAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            (line as LineConnector).UpdateConnectorPathGeometry();
        }

        public static void OnCustomLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            if (line.CustomLabelPosition == CustomLabelPositions.Auto)
            {
                (line as LineConnector).IsLabelDragable = false;
                (line as LineConnector).AllowCustomLabelPosition = false;
                (line as LineConnector).UpdateConnectorPathGeometry();
            }
            else if (line.CustomLabelPosition == CustomLabelPositions.Custom)
            {
                (line as LineConnector).IsLabelDragable = false;
                (line as LineConnector).AllowCustomLabelPosition = true;
            }
            else if (line.CustomLabelPosition == CustomLabelPositions.Drag)
            {
                (line as LineConnector).IsLabelDragable = true;
                (line as LineConnector).AllowCustomLabelPosition = true;
            }
        }
        public static void OnLabelTemplateOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector lc = d as LineConnector;
            if (lc != null)
            {
                if (lc.LabelOrientation == LabelOrientation.Vertical)
                {

                }
            }

        }

        /// <summary>
        /// Called when [units changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector lc = d as LineConnector;
            //if (lc != null)
            //{
            //    if (lc.m_IsPixelDefultUnit)
            //    {
            //        lc.StartPointPosition = MeasureUnitsConverter.Convert(lc.StartPointPosition, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            //        lc.EndPointPosition = MeasureUnitsConverter.Convert(lc.EndPointPosition, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            //    }
            //    lc.ConnectionEndSpace = MeasureUnitsConverter.Convert(lc.ConnectionEndSpace, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            //    lc.m_IsPixelDefultUnit = true;
            //}

            if (lc._FirstLoaded)
            {
                lc.StartPointPosition = MeasureUnitsConverter.Convert(lc.StartPointPosition, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                lc.EndPointPosition = MeasureUnitsConverter.Convert(lc.EndPointPosition, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                lc.ConnectionEndSpace = MeasureUnitsConverter.Convert(lc.ConnectionEndSpace, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            }
            else if (lc.ConnectionEndSpace == 6)
            {
                lc.ConnectionEndSpace = MeasureUnitsConverter.Convert(lc.ConnectionEndSpace, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            }
        }

        /// <summary>
        /// finds the angle     
        /// </summary>
        /// <param name="s">start point to find angle between</param>
        /// <param name="e">end point to find angle between</param>
        /// <returns>angle between the points</returns>
        internal static double FindAngle(Point s, Point e)
        {
            Point r = new Point(e.X, s.Y);
            double sr = FindHypo(s, r);
            double re = FindHypo(r, e);
            double es = FindHypo(e, s);
            double ang = Math.Asin(re / es);
            ang = ang * 180 / Math.PI;
            if (s.X < e.X)
            {
                if (s.Y < e.Y)
                {
                }
                else
                {
                    ang = 360 - ang;
                }
            }
            else
            {
                if (s.Y < e.Y)
                {
                    ang = 180 - ang;
                }
                else
                {
                    ang = 180 + ang;
                }
            }

            if (double.IsNaN(ang))
            {
                return 0;
            }
            else
            {
                return ang;
            }
        }

        /// <summary>
        /// Finds the hypotenuse
        /// </summary>
        /// <param name="s">start point to find angle between</param>
        /// <param name="e">end point to find angle between</param>
        /// <returns>angle between the points</returns>
        private static double FindHypo(Point s, Point e)
        {
            double length;
            length = Math.Sqrt(Math.Pow((s.X - e.X), 2) + Math.Pow((s.Y - e.Y), 2));
            return length;
        }

        #region Class fields

        #endregion

        #region Initialization

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        #endregion

        #region Events

        #endregion

        #region Virtual Methods
        internal Rect LineBounds(LineConnector line)
        {
            Rect rect = new Rect();
            if (line.PxStartPointPosition.X < line.PxEndPointPosition.X)
            {
                rect = new Rect(line.PxStartPointPosition.X, line.PxStartPointPosition.Y, Math.Abs(line.PxEndPointPosition.X - line.PxStartPointPosition.X), Math.Abs(line.PxEndPointPosition.Y - line.PxStartPointPosition.Y));
                return rect;
            }
            else
            {
                rect = new Rect(line.PxEndPointPosition.X, line.PxEndPointPosition.Y, Math.Abs(line.PxStartPointPosition.X - line.PxEndPointPosition.X), Math.Abs(line.PxStartPointPosition.Y - line.PxEndPointPosition.Y));
                return rect;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        #endregion

        #region IEdge Members

        #endregion

        #region INodeGroup Members

        #endregion

        #region Methods

        /// <summary>
        /// Represents the slope undefined exception .
        /// </summary>
        internal class SlopeUndefinedException : System.Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SlopeUndefinedException"/> class.
            /// </summary>
            public SlopeUndefinedException()
            {
            }
        }

        #endregion

        /// <summary>
        /// Finds the POI between two poly line.
        /// </summary>
        /// <param name="polyLine1">The poly line1.</param>
        /// <param name="polyLine2">The poly line2.</param>
        /// <param name="self">if set to <c>true</c> [self].</param>
        /// <returns></returns>
        static internal List<Point> FindPOIBetweenTwoPolyLine(List<Point> polyLine1, List<Point> polyLine2, bool self)
        {
            if (self && polyLine2.Count >= 2)
            {
                polyLine2.RemoveAt(0);
                polyLine2.RemoveAt(0);
            }
            List<Point> intersect = new List<Point>();
            for (int i = 0; i < polyLine1.Count - 1; i++)
            {
                intersect.AddRange(FindPOIBetweenLineAndPolyLine(polyLine1[i], polyLine1[i + 1], polyLine2));
                if (self && polyLine2.Count >= 1)
                {
                    polyLine2.RemoveAt(0);
                }
            }
            return intersect;
        }

        /// <summary>
        /// Finds the POI between line and poly line.
        /// </summary>
        /// <param name="lineStart">The line start.</param>
        /// <param name="lineEnd">The line end.</param>
        /// <param name="polyLine">The poly line.</param>
        /// <returns></returns>
        static internal List<Point> FindPOIBetweenLineAndPolyLine(Point lineStart, Point lineEnd, List<Point> polyLine)
        {
            List<Point> intersect = new List<Point>();
            for (int i = 0; i < polyLine.Count - 1; i++)
            {
                Point p = FindPOIBetweenTwoLines2(lineStart, lineEnd, polyLine[i], polyLine[i + 1]);
                if (!p.Equals(new Point(0, 0)))
                {
                    intersect.Add(p);
                }
            }
            return intersect;
        }

        /// <summary>
        /// Finds the POI between two lines2.
        /// </summary>
        /// <param name="startRect">The start rect.</param>
        /// <param name="endRect">The end rect.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        static private Point FindPOIBetweenTwoLines2(Point startRect, Point endRect, Point start, Point end)
        {
            Line l1 = new Line() { X1 = startRect.X, Y1 = startRect.Y, X2 = endRect.X, Y2 = endRect.Y };
            Line l2 = new Line() { X1 = start.X, Y1 = start.Y, X2 = end.X, Y2 = end.Y };
            if (FindPOIBetweenTwoLines(l1, l2, ref end))
            {
                return end;
            }
            else
            {
                return new Point(0, 0);
            }
        }

        public static bool FindPOIBetweenTwoLines(Line L1, Line L2, ref Point POI)
        {
            double d = (L2.Y2 - L2.Y1) * (L1.X2 - L1.X1) - (L2.X2 - L2.X1) * (L1.Y2 - L1.Y1);
            double n_a = (L2.X2 - L2.X1) * (L1.Y1 - L2.Y1) - (L2.Y2 - L2.Y1) * (L1.X1 - L2.X1);
            double n_b = (L1.X2 - L1.X1) * (L1.Y1 - L2.Y1) - (L1.Y2 - L1.Y1) * (L1.X1 - L2.X1);

            if (d == 0)
                return false;

            double ua = n_a / d;
            double ub = n_b / d;

            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                POI.X = L1.X1 + (ua * (L1.X2 - L1.X1));
                POI.Y = L1.Y1 + (ua * (L1.Y2 - L1.Y1));
                return true;
            }
            return false;
        }


        /// <summary>
        /// Lieses the within.
        /// </summary>
        /// <param name="startRect">The start rect.</param>
        /// <param name="endRect">The end rect.</param>
        /// <param name="point">The point.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static bool LiesWithin(Point startRect, Point endRect, Point point, double m)
        {
            if (m != 0)
            {
                if (((startRect.X < point.X && point.X < endRect.X) || (startRect.X > point.X && point.X > endRect.X)) &&
                    ((startRect.Y < point.Y && point.Y < endRect.Y) || (startRect.Y > point.Y && point.Y > endRect.Y)))
                {
                    return true;
                }
            }
            else
            {
                if (((startRect.X <= point.X && point.X <= endRect.X) || (startRect.X >= point.X && point.X >= endRect.X)) &&
                    ((startRect.Y <= point.Y && point.Y <= endRect.Y) || (startRect.Y >= point.Y && point.Y >= endRect.Y)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the length at fraction point.
        /// </summary>
        /// <param name="pathFigure">The path figure.</param>
        /// <param name="at">At.</param>
        /// <param name="fullLength">The full length.</param>
        /// <param name="segmentIndex">Index of the segment.</param>
        /// <returns></returns>
        internal double GetLengthAtFractionPoint(PathFigure pathFigure, Point at, out double fullLength, out int segmentIndex)
        {
            double confirm = 100d;
            fullLength = 0;
            segmentIndex = -1;
            int count = 0;
            double LengthAtFractionPoint = 0;
            if (pathFigure == null)
            {
                return 0;
            }
            //bool isAlreadyFlattened = true;

            //foreach (PathSegment pathSegment in pathFigure.Segments)
            //{
            //    if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
            //    {
            //        isAlreadyFlattened = false;
            //        break;
            //    }
            //}

            //PathFigure pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();
            PathFigure pathFigureFlattened = pathFigure;
            Point pt1 = pathFigureFlattened.StartPoint;
            Point previouspt2 = pt1;

            foreach (PathSegment pathSegment in pathFigureFlattened.Segments)
            {
                IEnumerable<Point> pointCollection;
                if (pathSegment is LineSegment)
                {
                    Point pt2 = (pathSegment as LineSegment).Point;
                    pointCollection = new List<Point>() { pt2 };
                }
                else if (pathSegment is PolyLineSegment)
                {
                    pointCollection = (pathSegment as PolyLineSegment).Points;
                }
                else if (pathSegment is BezierSegment)
                {
                    pointCollection = BezireToPoly(previouspt2, pathSegment as BezierSegment);
                }

                //Arc segment not supported.
                //else if (pathSegment is ArcSegment)
                //{
                //    pointCollection = ArcToPoly(previouspt2, pathSegment as ArcSegment);
                //}
                else
                {
                    pointCollection = new List<Point>();
                }

                foreach (Point pt2 in pointCollection)
                {
                    double suspect = getSlope(pt2, pt1, at);
                    if (suspect < confirm)
                    {
                        confirm = suspect;
                        LengthAtFractionPoint = fullLength + findHypo(at, previouspt2);
                        segmentIndex = count;
                    }
                    fullLength += findHypo(pt2, pt1);
                    pt1 = pt2;
                    previouspt2 = pt2;
                }
                count++;
            }
            return LengthAtFractionPoint;
        }

        internal List<Point> BezireToPoly(Point start, BezierSegment segment)
        {
            BezierSegment bezSeg = segment as BezierSegment;
            Point pt0 = start;
            Point pt1 = bezSeg.Point1;
            Point pt2 = bezSeg.Point2;
            Point pt3 = bezSeg.Point3;

            List<Point> points = new List<Point>();

            FlattenCubicBezier(points, pt0, pt1, pt2, pt3, 10);

            return points;
        }

        //internal List<Point> ArcToPoly(Point start, ArcSegment segment)
        //{
        //    ArcSegment arcSeg = segment as ArcSegment;
        //    List<Point> points = new List<Point>();

        //    FlattenArc(points, start, arcSeg.Point,
        //               arcSeg.Size.Width, arcSeg.Size.Height,
        //               arcSeg.RotationAngle,
        //               arcSeg.IsLargeArc,
        //               arcSeg.SweepDirection == SweepDirection.Counterclockwise,
        //               1);

        //    return points;
        //}

        void FlattenCubicBezier(List<Point> points, Point ptStart, Point ptCtrl1, Point ptCtrl2, Point ptEnd, double tolerance)
        {

            int max = (int)((findHypo(ptCtrl1, ptStart) +
                              findHypo(ptCtrl2, ptCtrl1) +
                              findHypo(ptEnd, ptCtrl2)) / tolerance);

            for (int i = 0; i <= max; i++)
            {
                double t = (double)i / max;

                double x = (1 - t) * (1 - t) * (1 - t) * ptStart.X +
                           3 * t * (1 - t) * (1 - t) * ptCtrl1.X +
                           3 * t * t * (1 - t) * ptCtrl2.X +
                           t * t * t * ptEnd.X;

                double y = (1 - t) * (1 - t) * (1 - t) * ptStart.Y +
                           3 * t * (1 - t) * (1 - t) * ptCtrl1.Y +
                           3 * t * t * (1 - t) * ptCtrl2.Y +
                           t * t * t * ptEnd.Y;

                points.Add(new Point(x, y));
            }
        }

        void FlattenQuadraticBezier(List<Point> points, Point ptStart, Point ptCtrl, Point ptEnd, double tolerance)
        {

            int max = (int)((findHypo(ptCtrl, ptStart) +
                                findHypo(ptEnd, ptCtrl)) / tolerance);

            for (int i = 0; i <= max; i++)
            {
                double t = (double)i / max;

                double x = (1 - t) * (1 - t) * ptStart.X +
                           2 * t * (1 - t) * ptCtrl.X +
                           t * t * ptEnd.X;

                double y = (1 - t) * (1 - t) * ptStart.Y +
                           2 * t * (1 - t) * ptCtrl.Y +
                           t * t * ptEnd.Y;

                points.Add(new Point(x, y));
            }
        }

        #region Arc

        //void FlattenArc(List<Point> points, Point pt1, Point pt2,
        //                double radiusX, double radiusY, double angleRotation,
        //                bool isLargeArc, bool isCounterclockwise,
        //                double tolerance)
        //{
        //    // Adjust for different radii and rotation angle

        //    MatrixHelper matx = new MatrixHelper();
        //    matx.Rotate(-angleRotation);
        //    matx.Scale(radiusY / radiusX, 1);
        //    pt1 = matx.Value.Transform(pt1);
        //    pt2 = matx.Value.Transform(pt2);


        //    // Get info about chord that connects both points
        //    Point midPoint = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);

        //    Vector vect = new PointHelper(pt2) - new PointHelper(pt1);

        //    double halfChord = vect.Length / 2;

        //    // Get vector from chord to center
        //    Vector vectRotated;

        //    // (comparing two Booleans here!)
        //    if (isLargeArc == isCounterclockwise)
        //        vectRotated = new Vector(-vect.Y, vect.X);
        //    else
        //        vectRotated = new Vector(vect.Y, -vect.X);

        //    vectRotated.Normalize();

        //    // Distance from chord to center 
        //    double centerDistance = Math.Sqrt(radiusY * radiusY - halfChord * halfChord);

        //    // Calculate two center points
        //    Point center = midPoint + centerDistance * vectRotated;

        //    // Get angles from center to the two points
        //    double angle1 = Math.Atan2(pt1.Y - center.Y, pt1.X - center.X);
        //    double angle2 = Math.Atan2(pt2.Y - center.Y, pt2.X - center.X);

        //    // (another comparison of two Booleans!)
        //    if (isLargeArc == (Math.Abs(angle2 - angle1) < Math.PI))
        //    {
        //        if (angle1 < angle2)
        //            angle1 += 2 * Math.PI;
        //        else
        //            angle2 += 2 * Math.PI;
        //    }

        //    // Invert matrix for final point calculation
        //    matx.Invert();

        //    // Calculate number of points for polyline approximation
        //    int max = (int)((4 * (radiusX + radiusY) * Math.Abs(angle2 - angle1) / (2 * Math.PI)) / tolerance);

        //    // Loop through the points
        //    for (int i = 0; i <= max; i++)
        //    {
        //        double angle = ((max - i) * angle1 + i * angle2) / max;
        //        double x = center.X + radiusY * Math.Cos(angle);
        //        double y = center.Y + radiusY * Math.Sin(angle);

        //        // Transform the point back
        //        Point pt = matx.Transform(new Point(x, y));
        //        points.Add(pt);
        //    }
        //}

        //void FlattenArc(IList<Point> points, Point pt1, Point pt2,
        //        double radiusX, double radiusY, double angleRotation,
        //        bool isLargeArc, bool isCounterclockwise,
        //        double tolerance)
        //{
        //    // Adjust for different radii and rotation angle
        //    MatrixHelper matx = new MatrixHelper();
        //    //matx.Rotate(angleRotation);
        //    //matx.Scale(radiusY / radiusX, 1);
        //    //pt1 = matx.Value.Transform(pt1);
        //    //pt2 = matx.Value.Transform(pt2);

        //    // Get info about chord that connects both points
        //    Point midPoint = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
        //    Vector vect = new PointHelper(pt2) - new PointHelper(pt1);
        //    double halfChord = vect.Length / 2;

        //    // Get vector from chord to center
        //    Vector vectRotated;

        //    // (comparing two Booleans here!)
        //    if (isLargeArc == isCounterclockwise)
        //        vectRotated = new Vector(-vect.Y, vect.X);
        //    else
        //        vectRotated = new Vector(vect.Y, -vect.X);

        //    vectRotated.Normalize();

        //    // Distance from chord to center 
        //    double centerDistance = Math.Sqrt(radiusY * radiusY - halfChord * halfChord);

        //    // Calculate center point
        //    Point center = midPoint + centerDistance * vectRotated;

        //    // Get angles from center to the two points
        //    double angle1 = Math.Atan2(pt1.Y - center.Y, pt1.X - center.X);
        //    double angle2 = Math.Atan2(pt2.Y - center.Y, pt2.X - center.X);

        //    // (another comparison of two Booleans!)
        //    if (isLargeArc == (Math.Abs(angle2 - angle1) < Math.PI))
        //    {
        //        if (angle1 < angle2)
        //            angle1 += 2 * Math.PI;
        //        else
        //            angle2 += 2 * Math.PI;
        //    }

        //    // Invert matrix for final point calculation
        //    matx.Invert();

        //    // Calculate number of points for polyline approximation
        //    int max = (int)((4 * (radiusX + radiusY) * Math.Abs(angle2 - angle1) / (2 * Math.PI)) / tolerance);

        //    // Loop through the points
        //    for (int i = 0; i <= max; i++)
        //    {
        //        double angle = ((max - i) * angle1 + i * angle2) / max;
        //        double x = center.X + radiusY * Math.Cos(angle);
        //        double y = center.Y + radiusY * Math.Sin(angle);

        //        // Transform the point back
        //        Point pt = matx.Transform(new Point(x, y));
        //        points.Add(pt);
        //    }
        //}

        #endregion

        /// <summary>
        /// Gets the slope.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal double getSlope(Point st, Point en, Point point)
        {
            double three = 3.0;// MeasureUnitsConverter.FromPixels(3.0, this.MeasurementUnit);
            double delx = Math.Abs(st.X - en.X);
            double dely = Math.Abs(st.Y - en.Y);
            double lhs = ((point.Y - st.Y) / (en.Y - st.Y));
            double rhs = ((point.X - st.X) / (en.X - st.X));
            if (double.IsInfinity(lhs) || double.IsInfinity(rhs) || double.IsNaN(lhs) || double.IsNaN(rhs))
            {
                if (st.X == en.X)
                {
                    if (st.Y == en.Y)
                    {
                        return 10000d;
                    }
                    else if (((st.Y > point.Y) && (point.Y > en.Y)) || ((st.Y < point.Y) && (point.Y < en.Y)))
                    {
                        return Math.Abs(st.X - point.X);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else if (st.Y == en.Y)
                {
                    if (((st.X > point.X) && (point.X > en.X)) || ((st.X < point.X) && (point.X < en.X)))
                    {
                        return Math.Abs(st.Y - point.Y);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else
                {
                    return 10000d;
                }
            }
            else if (ConnectorType != ConnectorType.Orthogonal)
            {
                if ((st.X >= point.X && point.X >= en.X) || (st.X <= point.X && point.X <= en.X) || delx < three)
                {
                    if ((st.Y >= point.Y && point.Y >= en.Y) || (st.Y <= point.Y && point.Y <= en.Y) || dely < three)
                    {
                        return Math.Abs(lhs - rhs);
                    }
                    else
                    {
                        return 10000d;
                    }
                }
                else
                {
                    return 10000d;
                }
            }
            else
            {
                return 10000d;
            }
        }

        /// <summary>
        /// Finds the hypo.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        internal static double findHypo(Point s, Point e)
        {
            double length;
            length = Math.Sqrt(Math.Pow((s.X - e.X), 2) + Math.Pow((s.Y - e.Y), 2));
            return length;
        }

        /// <summary>
        /// Finds the angle.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        internal static double findAngle(Point s, Point e)
        {
            Point r = new Point(e.X, s.Y);
            double sr = findHypo(s, r);
            double re = findHypo(r, e);
            double es = findHypo(e, s);
            double ang = Math.Asin(re / es);
            ang = ang * 180 / Math.PI;
            if (s.X < e.X)
            {
                if (s.Y < e.Y)
                {

                }
                else
                {
                    ang = 360 - ang;
                }
            }
            else
            {
                if (s.Y < e.Y)
                {
                    ang = 180 - ang;
                }
                else
                {
                    ang = 180 + ang;
                }
            }
            return ang;
        }

    }
}
