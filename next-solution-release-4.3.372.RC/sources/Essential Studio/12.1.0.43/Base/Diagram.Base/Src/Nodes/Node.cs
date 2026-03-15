#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Abstract class that implement base methods that used to transform and visualize 
    /// diagram objects. Implement moving, rotating, scaling, hittesting and serialization.
    /// </summary>
    /// <remarks>
    /// This is a basic object on the diagram that can represent connectors, images,
    /// text box, controls and other simple shapes.
    /// </remarks>
    [Serializable]
    public abstract class Node
        : IUnitIndependent,
          IDispatchNodeEvents,
          ISerializable,
          ICloneable,
          INode,
          IServiceReferenceHolder,
          IServiceReferenceProvider,
          IPropertyObserver,
          IPropertyContainer,
          IServiceProvider,
          IGraphNode,
          IDeserializationCallback
    {
        #region Class members
        /// <summary>
        /// Member of the LogicalGraphicsPath property.
        /// </summary>
        private GraphicsPath m_grphPath;

        /// <summary>
        /// Indicates whether LineRouter
        /// will treat this node as obstacle.
        /// </summary>
        private bool m_bObstacle;

        /// <summary>
        /// Reference to center port in port collection.
        /// </summary>
        private CentralPort m_centralPort;

        /// <summary>
        /// Indicated whether center port is enable.
        /// </summary>
        private bool m_bEnableCentralPort;

        /// <summary>
        /// Node's name.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Node's parent.
        /// </summary>
        [NonSerialized]
        private ICompositeNode m_parent;

        /// <summary>
        /// Indicates whether node is visible.
        /// </summary>
        private bool m_bVisible;

        /// <summary>
        /// Layers the shape belongs to.
        /// </summary>
        private LayerCollection m_layers;

        /// <summary>
        /// Region used for HitTesting.
        /// </summary>
        /// <remarks>
        /// Internal usage only.
        /// </remarks>
        protected Region[] m_rgnCache;

        /// <summary>
        /// Helper member used to store previous hit test padding value.
        /// If padding value is not equal to value stored in m_fPadding member
        /// cached region used to hit test point is recreated.
        /// </summary>
        /// <remarks>
        /// Internal usage only.
        /// </remarks>
        private float m_fPadding;

        /// <summary>
        /// Padding around the node used for hit testing.
        /// </summary>
        private float m_fHitTestPadding;
        private BoundsInfo m_boundsInternal;

        /// <summary>
        /// Node's rotation angle.
        /// </summary>
        protected float m_fRotationAngle;

        /// <summary>
        /// Indicates whether node is flipped along X axis.
        /// </summary>
        private bool m_bFlipX;

        /// <summary>
        /// Indicates whether node is flipped along Y axis.
        /// </summary>
        private bool m_bFlipY;

        /// <summary>
        /// Properties for creating pens to draw lines.
        /// </summary>
        private LineStyle m_styleLine;

        /// <summary>
        /// Properties for determining edit capabilities.
        /// </summary>
        private EditStyle m_styleEdit;

        /// <summary>
        /// Shape's shadow style.
        /// </summary>
        private ShadowStyle m_styleShadow;
        private ConnectionPointCollection m_ports;

        /// <summary>
        /// Service references provider.
        /// </summary>
        /// <remarks>
        /// Provides references to HistoryManger and DocumentEventSink.
        /// </remarks>
        [NonSerialized]
        private IServiceReferenceProvider m_provider;

        /// <summary>
        /// History manager reference.
        /// </summary>
        [NonSerialized]
        protected HistoryManager m_mgrHistory;

        /// <summary>
        /// DocumentEventSink reference.
        /// </summary>
        [NonSerialized]
        protected DocumentEventSink m_eventSink;

        /// <summary>
        /// Link Manager reference.
        /// </summary>
        [NonSerialized]
        protected LinkManager m_mgrLink;

        /// <summary>
        /// Bridge Manager reference.
        /// </summary>
        [NonSerialized]
        protected BridgeManager m_mgrBridge;

        /// <summary>
        /// Lock updates.
        /// </summary>
        protected bool m_bLockUpdate;

        /// <summary>
        /// Indicates whether node ports are visible.
        /// </summary>
        private bool m_bDrawPorts;

        /// <summary>
        /// Indicates whether node port is Visible while printing or not. 
        /// </summary>
        private bool m_bPrintPorts;

        /// <summary>
        /// Indicates whether node's units will change on its container's MeasureUnits changing.
        /// </summary>
        private bool m_bInheritContainerUnits;

        /// <summary>
        /// Cached node's bounding rectangle.
        /// </summary>
        private RectangleF m_rectBounding;

        /// <summary>
        /// Cached node's refresh rectangle.
        /// </summary>
        /// <remarks>
        /// Defines node's refresh rectangle used during rendering.
        /// Differs from BoundingRectangle as it includes additional data
        /// such as labels, ports, connector bridges etc.
        /// </remarks>
        protected RectangleF m_rectRefresh;

        /// <summary>
        /// Custom data associated with Node.
        /// </summary>
        private object m_objTag;

        /// <summary>
        /// Indicate whether node updating self ports connections.
        /// </summary>
        protected bool m_bPortUpdating = false;

        /// <summary>
        /// Container node.
        /// </summary>
        private Node m_nodeContainer = null;

        /// <summary>
        /// Dynamic property data
        /// </summary>
        private Dictionary<string, object> m_nPropertyBag;

        /// <summary>
        /// Tooltip text associated with the node.
        /// </summary>
        private string m_strToolTipText = string.Empty;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            m_strName = string.Empty;
            m_bVisible = true;
            m_bDrawPorts = true;
            m_bPrintPorts = true;
            m_bInheritContainerUnits = true;
            m_bObstacle = true;
            CreateBoundsInfo();
            this.EnableCentralPort = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public Node(Node src)
        {

            m_strName = src.m_strName;
            m_bVisible = src.Visible;
            m_fHitTestPadding = src.m_fHitTestPadding;
            m_bInheritContainerUnits = src.m_bInheritContainerUnits;

            // recreate bounds info
            PageScale nodeScale = (PageScale)src.BoundsInfo.NodeScale.Clone();
            CreateBoundsInfo(src.BoundsInfo, nodeScale);

            m_boundsInternal.Unit = src.BoundsInfo.Unit;

            m_fRotationAngle = src.RotationAngle;
            m_bFlipX = src.FlipX;
            m_bFlipY = src.FlipY;
            m_bDrawPorts = src.m_bDrawPorts;
            m_bObstacle = src.m_bObstacle;

            //if (src.m_layers != null)
            //    m_layers = (LayerCollection)src.Layers.Clone();

            if (src.m_styleLine != null)
                m_styleLine = (LineStyle)src.LineStyle.Clone();

            if (src.m_styleEdit != null)
                m_styleEdit = (EditStyle)src.EditStyle.Clone();

            if (src.m_styleShadow != null)
                m_styleShadow = (ShadowStyle)src.ShadowStyle.Clone();

            if (src.m_ports != null)
            {
                ConnectionPointCollection ports = new ConnectionPointCollection(this);
                ConnectionPoint portClone;

                foreach (ConnectionPoint port in src.m_ports)
                {
                    portClone = (ConnectionPoint)port.Clone();
                    portClone.Container = this;

                    ports.Add(portClone);
                }

                m_ports = ports;

                m_bEnableCentralPort = src.EnableCentralPort;

                m_rectBounding = src.m_rectBounding;
                m_rectRefresh = src.m_rectRefresh;
            }
            m_objTag = src.m_objTag;
            if (src.m_nPropertyBag != null)
            {
                m_nPropertyBag = new Dictionary<string, object>((Dictionary<string, object>)src.m_nPropertyBag);                
            }
            m_strToolTipText = src.m_strToolTipText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Node(SerializationInfo info, StreamingContext context)
        {
            // Layers. If serialized node did not belong to any layer
            // no need to serialize empty LayerCollection
            bool layerspresent = false;
            bool portspresent = true;

            m_bObstacle = true;
            m_layers = null;

            ConnectionPointCollection ports = null;
            BoundsInfo boundsInternal = null;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    // warning : entry.Value can't be converted to bool object type
                    case "tag":
                        {
                            // deserialize tag
                            m_objTag = entry.Value;
                            IDeserializationCallback des = m_objTag as IDeserializationCallback;

                            if (des != null)
                                des.OnDeserialization(this);
                            break;
                        }
                    case "obstacle":
                        m_bObstacle = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "layerspresent":
                        layerspresent = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "portspresent":
                        portspresent = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "boundingrect":
                        m_rectBounding = (RectangleF)entry.Value;
                        break;
                    case "refreshrect":
                        m_rectRefresh = (RectangleF)entry.Value;
                        break;
                    case "name":
                        m_strName = (string)entry.Value;
                        break;
                    case "visible":
                        m_bVisible = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "drawports":
                        m_bDrawPorts = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "inheritContainerUnits":
                        m_bInheritContainerUnits = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "shadowStyle":
                        m_styleShadow = (ShadowStyle)entry.Value;
                        break;
                    case "ports":
                        ports = (ConnectionPointCollection)entry.Value;
                        break;
                    case "centralPort":
                        m_bEnableCentralPort = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "boundsInfo":
                        boundsInternal = (BoundsInfo)entry.Value;
                        break;
                    case "hitTestingPadding":
                        m_fHitTestPadding = (float)double.Parse(
                            entry.Value.ToString().Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
                        break;
                    case "rotationAngle":
                        m_fRotationAngle = (float)double.Parse(
                            entry.Value.ToString().Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
                        break;
                    case "flipX":
                        m_bFlipX = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "flipY":
                        m_bFlipY = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "lineStyle":
                        m_styleLine = (LineStyle)entry.Value;
                        break;
                    case "editStyle":
                        m_styleEdit = (EditStyle)entry.Value;
                        break;
                    case "nodemeasurementunit":
                        this.BoundsInfo.Unit = (MeasureUnits)entry.Value;
                        break;
                    case "toolTipText":
                        m_strToolTipText = entry.Value.ToString();
                        break;
                    case "PropertyBag":
                        m_nPropertyBag = (Dictionary<string, object>)entry.Value;
                        break;
                }
            }

            if (layerspresent)
            {
                m_layers = (LayerCollection)info.GetValue("layers", typeof(LayerCollection));
            }

            if (portspresent)
            {
                if (ports != null && ports.Count > 0)
                {
                    foreach (object port in ports)
                    {
                        ConnectionPoint point = port as ConnectionPoint;

                        if (point != null)
                            point.Container = this;
                    }

                    ports.Container = this;
                }
                else
                {
                    ports = new ConnectionPointCollection(this);
                }

                m_ports = ports;
            }

            this.LineStyle.UpdateServiceReferences(this);
            this.EditStyle.UpdateServiceReferences(this);

            if (m_styleShadow == null)
                m_styleShadow = new ShadowStyle();

            this.ShadowStyle.UpdateServiceReferences(this);

            // recreate bounds info
            CreateBoundsInfo(boundsInternal);
           
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether node treat as obstacle.
        /// </summary>
        /// <value><c>true</c> if need node treat as obstacle; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Misc")]
        [Description("Indicates whether LineRouting engine will treat node as obstacle while rerouting.")]
        public bool TreatAsObstacle
        {
            get 
            { 
                return m_bObstacle; 
            }
            set
            {
                if (m_bObstacle != value && OnPropertyChanging(this.FullContainerName, DPN.TreatAsObstacle, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.TreatAsObstacle);

                    // set new value
                    m_bObstacle = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.TreatAsObstacle);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether node will draw it ports while printing.
        /// </summary>
        /// <value><c>true</c> if [print ports]; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether ports are visible while print it.")]
        public bool PrintPorts
        {
            get
            {
                return m_bPrintPorts; 
            }
            set
            {
                if (m_bPrintPorts != value)
                {
                    m_bPrintPorts = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether node will draw it ports.
        /// </summary>
        /// <value><c>true</c> if n node will draw it ports; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether ports are visible.")]
        public virtual bool DrawPorts
        {
            get 
            { 
                return m_bDrawPorts; 
            }
            set
            {
                if (m_bDrawPorts != value && OnPropertyChanging(this.FullContainerName, DPN.DrawPorts, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.DrawPorts);

                    // set new value
                    m_bDrawPorts = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.DrawPorts);
                }
            }
        }

        /// <summary>
        /// Gets the node port collection.
        /// </summary>
        /// <value>The ports.</value>
        [Browsable(true)]
        [Category("Misc")]
        [Description("Connection points collection.")]
        public ConnectionPointCollection Ports
        {
            get
            {
                if (m_ports == null)
                {
                    m_ports = new ConnectionPointCollection(this);
                    m_ports.UpdateServiceReferences(this);
                }

                // update central point
                UpdateCentralPort();

                return m_ports;
            }
        }

        /// <summary>
        /// Gets or sets node's ZOrder.
        /// </summary>
        [Browsable(true)]
        [Category("General")]
        [Description("ZOrder of shape.")]
        public int ZOrder
        {
            get
            {
                int nZorder = -1;

                if (this.Parent is IZOrderContainer)
                {
                    nZorder = ((IZOrderContainer)this.Parent).GetZOrder(this);
                }

                return nZorder;
            }
            set
            {
                if (this.ZOrder != value)
                {
                    if (this.Parent is IZOrderContainer)
                    {
                        Model document = this.Root;

                        if (document != null)
                        {
                            document.HistoryManager.StartAtomicAction("Set ZOrder");
                        }

                        ((IZOrderContainer)this.Parent).SetZOrder(this, value);

                        if (document != null)
                        {
                            document.HistoryManager.EndAtomicAction();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether central port is enabled.
        /// </summary>
        /// <value><c>true</c> if central port is enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Description("Indicates whether central port is enabled." +
             "It means that you can connect to node using visual tools and connector will be docked to node shape's path.")]
        public bool EnableCentralPort
        {
            get
            { 
                return m_bEnableCentralPort; 
            }
            set
            {
                if (m_bEnableCentralPort != value && OnPropertyChanging(this.FullContainerName, DPN.EnableCentralPort, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.EnableCentralPort);

                    // set new value before port update
                    m_bEnableCentralPort = value;

                    // update port collection
                    if (m_ports == null)
                    {
                        m_ports = new ConnectionPointCollection(this);
                        m_ports.UpdateServiceReferences(this);
                    }

                    // update cetral port to new value
                    UpdateCentralPort();

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.EnableCentralPort);
                }
            }
        }

        /// <summary>
        /// Gets the center port from port collection.
        /// </summary>
        /// <value>The center port.</value>
        public CentralPort CentralPort
        {
            get
            {
                if (m_bEnableCentralPort && m_centralPort == null)
                {
                    UpdateCentralPort();
                }

                return m_centralPort;
            }
        }

        /// <summary>
        /// Gets or sets Custom data associated with Node.
        /// </summary>
        [Browsable(false)]
        [Category("General")]
        [Description("User-defined data associated with the object.")]
        public object Tag
        {
            get 
            { 
                return m_objTag; 
            }
            set
            {
                if (m_objTag != value && OnPropertyChanging(this.FullContainerName, DPN.Tag, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.Tag);

                    // set new value
                    m_objTag = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.Tag);
                }
            }
        }

        #region common properties
        /// <summary>
        /// Gets or sets node's name.
        /// </summary>
        [Browsable(true)]
        [Category("General")]
        [Description("Unique name of shape.")]
        public string Name
        {
            get 
            { 
                return m_strName; 
            }
            set
            {
                if (m_strName != value && value != string.Empty && OnPropertyChanging(this.FullContainerName, DPN.Name, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.Name);

                    // set new value
                    m_strName = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.Name);
                }
            }
        }

        /// <summary>
        /// Gets fully qualified name of the node.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The full name is the name of the node concatenated with the names
        /// of all parent nodes.
        /// </remarks>
        [Category("General")]
        [Description("Unique name of shape.")]
        public string FullName
        {
            get
            {
                 string strFullName = this.Name;
                 if (this.Parent != null)
                 {
                     if (this.Parent is Node)
                     {
                         strFullName = ((Node)this.Parent).FullName + "." + strFullName;
                     }
                     else if (this.Parent is Model)
                     {
                         strFullName = ((Model)this.Parent).FullName + "." + strFullName;
                     }
                 }
                return strFullName;
            }
        }
        
        /// <summary>
        /// Gets or sets node's container node
        /// </summary>
        [Browsable(false)]
        public Node Container
        {
            get
            {
                return m_nodeContainer;
            }
            set
            {
                if (value != m_nodeContainer)
                    m_nodeContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets node's parent.
        /// </summary>
        [Browsable(false)]
        public ICompositeNode Parent
        {
            get 
            { 
                return m_parent; 
            }
            set
            {
                if (m_parent != value)
                {
                    OnParentChanging();

                    m_parent = value;

                    if (m_mgrLink != null)
                        m_mgrLink.SynchronizeNodeConnections(this);

                    OnParentChanged();
                }
            }
        }

        /// <summary>
        /// Gets the root node in the node hierarchy.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The root node is found by following the chain of parent nodes until
        /// a node is found that has a NULL parent.
        /// </remarks>
        [Browsable(false)]
        public Model Root
        {
            get
            {
                Model modelToReturn = null;

                if (this.Parent != null)
                {
                    if (this.Parent is Model)
                    {
                        modelToReturn = (Model)this.Parent;
                    }
                    else
                    {
                        modelToReturn = ((Node)this.Parent).Root;
                    }
                }

                return modelToReturn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node is visible.
        /// </summary>
        /// <remarks>
        /// This flag can be changed to True only if one of its owner layer are visible.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether node is visible.")]
        public virtual bool Visible
        {
            get 
            { 
                return m_bVisible; 
            }
            set
            {
                if (m_bVisible != value && TryChangeVisibility(value) && OnPropertyChanging(this.FullContainerName, DPN.Visible, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.Visible);

                    // set new value
                    m_bVisible = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.Visible);
                }
            }
        }

        /// <summary>
        /// Gets the layers the shape belongs to.
        /// </summary>
        [Browsable(false)]
        public LayerCollection Layers
        {
            get
            {
                if (m_layers == null)
                {
                    m_layers = new LayerCollection(this);
                    m_layers.UpdateReferences = false;
                }

                return m_layers;
            }
        }

        /// <summary>
        /// Gets or sets the line hit test padding.
        /// </summary>
        /// <value>The line hit test padding.</value>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Padding added to lines during hit testing (in logical units).")]
        public float LineHitTestPadding
        {
            get
            {
                return this.BoundsInfo.GetHitTestPadding(this.BoundsInfo.Unit);
            }
            set
            {
                MeasureUnits unit = this.BoundsInfo.Unit;

                if (value != this.BoundsInfo.GetHitTestPadding(unit))
                {
                    this.BoundsInfo.SetHitTestPadding(value, unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets the measurement unit.
        /// </summary>
        /// <value>The measurement unit.</value>
        [Browsable(true)]
        [Category("General")]
        [Description("Logical unit of measurement.")]
        public MeasureUnits MeasurementUnit
        {
            get 
            { 
                return this.BoundsInfo.Unit; 
            }
            set
            {
                if (this.BoundsInfo.Unit != value)
                {
                    if (m_mgrHistory != null)
                        m_mgrHistory.StartAtomicAction("Measure Units changed");

                    OnMeasurementUnitsChanging(value);

                    this.BoundsInfo.Unit = value;

                    if (m_mgrHistory != null)
                        m_mgrHistory.EndAtomicAction();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether container measure units can be inherited.
        /// </summary>
        /// <value>
        /// <c>true</c> if inherit container measure units; otherwise, <c>false</c>.
        /// </value>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Indicates whether node update its measure units when node's container will change its measure units.")]
        public bool InheritContainerMeasureUnits
        {
            get 
            {
                return m_bInheritContainerUnits; 
            }
            set
            {
                if (m_bInheritContainerUnits != value && OnPropertyChanging(this.FullContainerName, DPN.InheritContainerMeasureUnits, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.InheritContainerMeasureUnits);

                    // set new value
                    m_bInheritContainerUnits = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.InheritContainerMeasureUnits);
                }
            }
        }

        /// <summary>
        /// Gets node graphics path without scale transformation.
        /// </summary>
        /// <value>The logical graphics path.</value>
        protected virtual GraphicsPath LogicalGraphicsPath
        {
            get
            {
                SizeF szSize = this.BoundsInfo.GetSize(MeasureUnits.Pixel);

                if (m_grphPath == null || m_grphPath.GetBounds().Size != szSize)
                {
                    m_grphPath = new GraphicsPath();

                    Matrix mtxScale = GetScaleTransformation();
                    mtxScale.Invert();
                    PointF ptSize = Geometry.AppendMatrix(szSize.ToPointF(), mtxScale);
                    RectangleF rectTemp = new RectangleF(0, 0, Math.Max(ptSize.X, 1), Math.Max(ptSize.Y, 1));
                    m_grphPath.AddRectangle(rectTemp);
                }

                return m_grphPath;
            }
        }

        /// <summary>
        /// Gets cloned node GraphicsPath with appended scale transformations.
        /// </summary>
        /// <value>The graphics path.</value>
        public GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path_return = null;
                GraphicsPath pathToReturn = LogicalGraphicsPath;

                if (pathToReturn != null)
                {
                    lock (pathToReturn)
                    {
                        path_return = (GraphicsPath)pathToReturn.Clone();
                    }

                    // append scale transform
                    Matrix mtxScale = this.GetScaleTransformation();
                    path_return.Transform(mtxScale);
                }

                return path_return;
            }
        }

        /// <summary>
        /// Gets the children to model history manager.
        /// </summary>
        /// <value>The history manager.</value>
        protected HistoryManager HistoryManager
        {
            get { return m_mgrHistory; }
        }

        /// <summary>
        /// Gets the reference to model event sink.
        /// </summary>
        /// <value>The event sink.</value>
        protected DocumentEventSink EventSink
        {
            get { return m_eventSink; }
        }

        /// <summary>
        /// Gets or sets the dynamic property data dictionary.
        /// </summary>       
        [Browsable(true)]
        [Description("Dynamic property data dictionary")]
        [Editor(typeof(PropertyBagEditor), typeof(UITypeEditor))]
        public Dictionary<string, object> PropertyBag
        {
            get
            {
                if (m_nPropertyBag == null)
                    m_nPropertyBag = new Dictionary<string, object>();
                return m_nPropertyBag;
            }           
        }

        /// <summary>
        /// Gets or sets the tooltip text associated with the node.
        /// </summary>
        [Browsable(true)]        
        [Category("General")]
        [Description("Tooltip text associated with the node.")]
        public string ToolTipText
        {
            get { return m_strToolTipText; }
            set
            {
                if (m_strToolTipText != value && OnPropertyChanging(this.FullContainerName, DPN.ToolTipText, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.ToolTipText);

                    // set new value
                    m_strToolTipText = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.ToolTipText);
                }
            }
        }
        #endregion

        #region bounds
        /// <summary>
        /// Gets node's refresh rectangle.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RectangleF RefreshRect
        {
            get { return m_rectRefresh; }
        }

        /// <summary>
        /// Gets or sets node's bounding rectangle.
        /// </summary>
        protected RectangleF BoundingRect
        {
            get { return m_rectBounding; }
            set { m_rectBounding = value; }
        }

        /// <summary>
        /// Gets the reference to bounds info.
        /// </summary>
        /// <value>The bounds info.</value>
        public BoundsInfo BoundsInfo
        {
            get
            {
                if (m_boundsInternal == null)
                {
                    CreateBoundsInfo();
                }

                return m_boundsInternal;
            }
        }

        /// <summary>
        /// Gets the shape's bounding box.
        /// </summary>
        [Browsable(false)]
        public RectangleF BoundingRectangle
        {
            get { return ((IUnitIndependent)this).GetBoundingRectangle(this.BoundsInfo.Unit, false); }
        }

        /// <summary>
        /// Gets or sets size of the node.
        /// </summary>
        [Browsable(true)]
        [Category("Bounds")]
        [Description("Size of the node.")]
        [TypeConverter(typeof(SizeFConverter))]
        public SizeF Size
        {
            get 
            { 
                return ((IUnitIndependent)this).GetSize(this.BoundsInfo.Unit); 
            }
            set
            {
                if (CheckNewSize(value, MeasureUnits.Pixel))
                {
                    // assign new value
                    this.BoundsInfo.SetSize(value, this.BoundsInfo.Unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets the node pin point offset.
        /// </summary>
        /// <value>The pin point offset.</value>
        [Browsable(true)]
        [Category("Bounds")]
        [Description("Offset from PinPoint to node's rendering origin.")]
        [TypeConverter(typeof(SizeFConverter))]
        public SizeF PinPointOffset
        {
            get 
            { 
                return ((IUnitIndependent)this).GetPinPointOffset(this.BoundsInfo.Unit); 
            }
            set
            {
                if (CheckNewPinOffset(value, MeasureUnits.Pixel))
                {
                    // assign new value
                    this.BoundsInfo.SetPinOffset(value, this.BoundsInfo.Unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets the node pin point position.
        /// </summary>
        /// <value>The pin point.</value>
        [Browsable(true)]
        [Category("Bounds")]
        [Description("Defines node's position in document.")]
        [TypeConverter(typeof(PointFConverter))]
        public PointF PinPoint
        {
            get { return this.BoundsInfo.GetPinPoint(this.BoundsInfo.Unit); }
            set { SetPinPoint(value, this.BoundsInfo.Unit); }
        }

        /// <summary>
        /// Gets or sets the node scale.
        /// </summary>
        /// <value>The node scale.</value>
        [Browsable(true)]
        [Category("Bounds")]
        [Description("Defines node's scale in document.")]
        [TypeConverter(typeof(PageScaleConverter))]
        public PageScale NodeScale
        {
            get 
            { 
                return this.BoundsInfo.NodeScale; 
            }
            set
            {
                m_rgnCache = null;
                this.BoundsInfo.NodeScale = value;
            }
        }
        #endregion

        #region styles
        /// <summary>
        /// Gets properties of the shadow applied to the shape.
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(ShadowStyleConverter))]
        [Category("Appearance")]
        [Description("Properties of drop shadow.")]
        public ShadowStyle ShadowStyle
        {
            get
            {
                if (m_styleShadow == null)
                {
                    m_styleShadow = new ShadowStyle();
                }

                return m_styleShadow;
            }
        }

        /// <summary>
        /// Gets line drawing properties for this node.
        /// </summary>
        /// <remarks>
        /// Gets the line style determines the configuration of the pen used to
        /// render lines.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>
        /// </remarks>
        [Browsable(true)]
        [TypeConverter(typeof(LineStyleConverter))]
        [Category("Appearance")]
        [Description("Properties of the pen used for drawing lines.")]
        public LineStyle LineStyle
        {
            get
            {
                if (m_styleLine == null)
                {
                    m_styleLine = new LineStyle();
                }

                return m_styleLine;
            }
        }

        /// <summary>
        /// Gets edit properties for this node.
        /// </summary>
        /// <remarks>
        /// Edit properties determine how this node can be edited.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.EditStyle"/>
        /// </remarks>
        [Browsable(true)]
        [TypeConverter(typeof(EditStyleConverter))]
        [Category("Behavior")]
        [Description("Properties that determine how the shape can be edited.")]
        public EditStyle EditStyle
        {
            get
            {
                if (m_styleEdit == null)
                {
                    m_styleEdit = new EditStyle();
                }

                return m_styleEdit;
            }
        }
        #endregion

        #region transformations
        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        /// <value>The rotation angle in range [ -180; 180 ].</value>
        /// <remarks>
        /// Rotation angle saved in rotation range [ -180; 180 ]
        /// But on set this property new rotation angle must be
        /// in range [ 0; 360 ]
        /// </remarks>
        [Browsable(true)]
        [Category("Transformations")]
        [Description("Node's rotation angle.")]
        [DefaultValue(0f)]
        public virtual float RotationAngle
        {
            get 
            { 
                return m_fRotationAngle; 
            }
            set
            {
                // assign new value
                float angle = value % CommonUsedValues.CIRCLE;
                float fNewAngle = Geometry.ConvertToPartCircle(angle);
                float fChangeAngle = fNewAngle - m_fRotationAngle;

                // start property change
                if (OnRotationChanging(fChangeAngle) && CheckNewRotationAngle(angle, MeasureUnits.Pixel))
                {
                    HistoryManager mngHistory = this.HistoryManager;

                    if (mngHistory != null)
                        mngHistory.StartAtomicAction("Rotation change");

                    // make history record
                    RecordPropertyChanged(DPN.RotationAngle);

                    // set angle in range from -180 to 180;
                    m_fRotationAngle = fNewAngle;

                    // call after change rotating method
                    ChangeRotationBy(fChangeAngle);

                    // update cache region
                    m_rgnCache = null;

                    // Update cached bounding rect
                    UpdateBoundingRectangle();

                    if (mngHistory != null)
                        mngHistory.EndAtomicAction();

                    // finish property change
                    OnRotationChanged(fChangeAngle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip X is enabled.
        /// </summary>
        /// <value><c>true</c> if node vertical flipped ; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [Category("Transformations")]
        [Description("Indicates whether node is flipped relative to X axis")]
        [DefaultValue(false)]
        public virtual bool FlipX
        {
            get 
            { 
                return m_bFlipX; 
            }
            set
            {
                if (OnFlipXChanging(value) && CheckNewFlipValue(value, this.FlipY, MeasureUnits.Pixel))
                {
                    HistoryManager mngHistory = this.HistoryManager;

                    if (mngHistory != null)
                        mngHistory.StartAtomicAction("FlipX change");

                    // make history record
                    RecordPropertyChanged(DPN.FlipX);

                    // assign new value
                    m_bFlipX = value;

                    // call after change method
                    ChangeFlipX(value);

                    // update cache region
                    m_rgnCache = null;

                    // Update cached bounding rect
                    UpdateBoundingRectangle();

                    if (mngHistory != null)
                        mngHistory.EndAtomicAction();

                    OnFlipXChanged(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip Y is enabled.
        /// </summary>
        /// <value><c>true</c> if node horizontal flipped; otherwise, <c>false</c>.</value>
        [Browsable(true)]
        [Category("Transformations")]
        [Description("Indicates whether node is flipped relative to Y axis")]
        [DefaultValue(false)]
        public virtual bool FlipY
        {
            get 
            { 
                return m_bFlipY; 
            }
            set
            {
                if (OnFlipYChanging(value) && CheckNewFlipValue(this.FlipX, value, MeasureUnits.Pixel))
                {
                    HistoryManager mngHistory = this.HistoryManager;

                    if (mngHistory != null)
                        mngHistory.StartAtomicAction("FlipX change");

                    // make history record
                    RecordPropertyChanged(DPN.FlipY);

                    // assign new value
                    m_bFlipY = value;

                    // call after change method
                    ChangeFlipY(value);

                    // update cache region
                    m_rgnCache = null;

                    // Update cached bounding rect
                    UpdateBoundingRectangle();

                    if (mngHistory != null)
                        mngHistory.EndAtomicAction();

                    OnFlipYChanged(value);
                }
            }
        }
        #endregion
        #endregion

        #region ISerializable
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                // update references
                UpdateReferences(null);

                m_mgrHistory = null;
                m_eventSink = null;
                m_mgrBridge = null;
                m_mgrLink = null;
            }
            else
            {
                m_mgrHistory = (HistoryManager)provider.ProvideServiceReference(typeof(HistoryManager).TypeHandle);
                m_eventSink = (DocumentEventSink)provider.ProvideServiceReference(typeof(DocumentEventSink).TypeHandle);
                m_mgrBridge = (BridgeManager)provider.ProvideServiceReference(typeof(BridgeManager).TypeHandle);
                m_mgrLink = (LinkManager)provider.ProvideServiceReference(typeof(LinkManager).TypeHandle);

                // update references
                UpdateReferences(this);
            }

            m_provider = provider;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns>The object.</returns>
        public virtual object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            object objToReturn = GetService(Type.GetTypeFromHandle(typeHandle));

            if (objToReturn == null)
            {
                if (typeHandle.Equals(typeof(HistoryManager).TypeHandle))
                {
                    objToReturn = m_mgrHistory;
                }
                else if (typeHandle.Equals(typeof(DocumentEventSink).TypeHandle) || typeHandle.Equals(typeof(EventSink).TypeHandle))
                {
                    objToReturn = m_eventSink;
                }
                else if (typeHandle.Equals(typeof(LinkManager).TypeHandle))
                {
                    objToReturn = m_mgrLink;
                }
                else if (typeHandle.Equals(typeof(BridgeManager).TypeHandle))
                {
                    objToReturn = m_mgrBridge;
                }
            }

            return objToReturn;
        }
        #endregion

        #region IPropertyContainer Members
        /// <summary>
        /// Gets the full name of the container.
        /// </summary>
        /// <value>The full name of the container.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FullContainerName
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the name of the property container by.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>The object.</returns>
        public object GetPropertyContainerByName(string strPropertyContainerName)
        {
            object objPropertyContainer = null;

            int nSeparatorIndex = strPropertyContainerName.IndexOf(CommonUsedValues.POINT_SEPARATOR);

            if (nSeparatorIndex == -1)
            {
                objPropertyContainer = GetPropertyContainer(strPropertyContainerName);
            }
            else
            {
                string strSubContainerName = strPropertyContainerName.Substring(0, nSeparatorIndex);

                object objSubContainer = GetPropertyContainer(strSubContainerName);
                IPropertyContainer subContainer = objSubContainer as IPropertyContainer;

                if (subContainer != null)
                {
                    strSubContainerName = strPropertyContainerName.Substring(nSeparatorIndex + 1, (strPropertyContainerName.Length - (nSeparatorIndex + 1)));
                    objPropertyContainer = subContainer.GetPropertyContainerByName(strSubContainerName);
                }
            }

            return objPropertyContainer;
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>The object.</returns>
        protected virtual object GetPropertyContainer(string strPropertyContainerName)
        {
            object objPropertyContainer = null;

            switch (strPropertyContainerName)
            {
                case "LineStyle":
                    objPropertyContainer = this.LineStyle;
                    break;
                case "ShadowStyle":
                    objPropertyContainer = this.ShadowStyle;
                    break;
                case "EditStyle":
                    objPropertyContainer = this.EditStyle;
                    break;
                case "BoundsInfo":
                    objPropertyContainer = this.BoundsInfo;
                    break;
            }

            return objPropertyContainer;
        }
        #endregion

        #region IPropertyObserver Members
        /// <summary>
        /// Called when property changing.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property changing.</returns>
        public virtual bool OnPropertyChanging(string strPropertyContainerName, string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                bSuccess = m_eventSink.RaisePropertyChangingEvent(this, strPropName, newValue);
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public virtual void OnPropertyChanged(string strPropertyContainerName, string strPropertyName)
        {
            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                bool bNodeScale = strPropertyName == DPN.NodeScale;

                if (bNodeScale || (strPropertyName != DPN.DrawPorts && strPropertyName != DPN.Name))
                {
                    if (bNodeScale)
                    {
                        OnNodeScaleChanged(strPropertyName);
                        m_mgrLink.SynchronizeNodeConnections(this);
                    }

                    UpdateBoundingRectangle();

                    // update containers's bounding rect
                    UpdateContainerBounds();
                }

                m_eventSink.RaisePropertyChangedEvent(this, strPropName);
            }
        }
        #endregion

        #region Class utility methods

        #region serialization
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (m_objTag != null)
            {
                try
                {
                    info.AddValue("tag", m_objTag);
                }
                catch (SerializationException)
                { 
                }
            }

            info.AddValue("name", m_strName);
            info.AddValue("visible", m_bVisible);
            info.AddValue("drawports", m_bDrawPorts);
            info.AddValue("obstacle", m_bObstacle);
            info.AddValue("inheritContainerUnits", m_bInheritContainerUnits);

            // Layers. If node do not belong to any layer
            // no need to serialize empty LayerCollection
            if (m_layers != null && m_layers.Count > 0)
            {
                info.AddValue("layerspresent", true);
                info.AddValue("layers", m_layers);
            }
            else
            {
                info.AddValue("layerspresent", false);
            }

            info.AddValue("hitTestingPadding", m_fHitTestPadding);
            info.AddValue("boundsInfo", m_boundsInternal);
            info.AddValue("rotationAngle", m_fRotationAngle);
            info.AddValue("flipX", m_bFlipX);
            info.AddValue("flipY", m_bFlipY);
            info.AddValue("lineStyle", m_styleLine);
            info.AddValue("editStyle", m_styleEdit);
            info.AddValue("shadowStyle", m_styleShadow);

            bool portspresent = (m_ports != null && m_ports.Count > 0);
            info.AddValue("portspresent", portspresent);
            if (portspresent)
                info.AddValue("ports", m_ports);

            info.AddValue("centralPort", m_bEnableCentralPort);
            info.AddValue("boundingrect", m_rectBounding);
            info.AddValue("refreshrect", m_rectRefresh);
            info.AddValue("nodemeasurementunit", this.BoundsInfo.Unit);
            info.AddValue("PropertyBag", m_nPropertyBag);
            info.AddValue("toolTipText", m_strToolTipText);
        }
        #endregion

        #region rendering
        /// <summary>
        /// Draws object to the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public void Draw(Graphics gfx)
        {
            if (this.Visible)
            {
                // Save graphics state
                GraphicsState save = gfx.Save();

                // prepare graphics
                PrepareGraphics(gfx);

                // Render shape visual representation
                // on given graphics
                Render(gfx);

                // Render additional data
                RenderContiguousData(gfx);

                // Restore graphics state
                gfx.Restore(save);
            }
        }

        /// <summary>
        /// Draws object to the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="bExcludeTransformation">if set to <c>true</c> to draws the object without the transformation.</param>
        public void Draw(Graphics gfx, bool bExcludeTransformation)
        {
            if (bExcludeTransformation)
            {
                // Save graphics state
                GraphicsState save = gfx.Save();

                // Render shape visual representation
                // on given graphics
                Render(gfx);

                // Render additional data
                RenderContiguousData(gfx);

                // Restore graphics state
                gfx.Restore(save);
            }
            else
            {
                Draw(gfx);
            }
        }

        /// <summary>
        /// Prepares the graphics to draw.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void PrepareGraphics(Graphics gfx)
        {
            // get transformations
            Matrix matrixTemp = GetTransformations();
            AppendFlipTransforms(matrixTemp);

            // Apply transformations
            gfx.MultiplyTransform(matrixTemp);
        }

        /// <summary>
        /// methods used to draw contiguous date. Such as labels or ports
        /// </summary>
        /// <param name="gfx">Graphics to draw on </param>
        protected virtual void RenderContiguousData(Graphics gfx)
        {
            if (this.DrawPorts)
            {
                foreach (ConnectionPoint port in this.Ports)
                {
                    port.Draw(gfx);
                }
            }
        }

        /// <summary>
        /// Renders shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected virtual void Render(Graphics gfx)
        {
            if (this.ShadowStyle.Visible)
            {
                DrawShadow(gfx);
            }
        }

        /// <summary>
        /// Draws shape's shadow on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        private void DrawShadow(Graphics gfx)
        {
            // if graphics path has zero height or width
            // widen corresponding dimension by one pixel.
            RectangleF rcGPBounds = this.GraphicsPath.GetBounds();

            // shadow graphics path's size is empty -> skip shadow rendering
            if (!rcGPBounds.Size.IsEmpty)
            {
                // Save graphics state.
                GraphicsState save = gfx.Save();

                // Append transformations
                // Shadow offset
                float fOffsetX = MeasureUnitsConverter.ToPixelX(this.ShadowStyle.OffsetX, this.ShadowStyle.MeasureUnit);
                float fOffsetY = MeasureUnitsConverter.ToPixelY(this.ShadowStyle.OffsetY, this.ShadowStyle.MeasureUnit);

                gfx.TranslateTransform(fOffsetX, fOffsetY, MatrixOrder.Append);

                // Draw shadow
                DrawShadowInternal(gfx);

                // Restore gfx saved state.
                gfx.Restore(save);
            }
        }
        #endregion

        #region hit testing
        /// <summary>
        /// Performs given point hit test.
        /// </summary>
        /// <param name="ptTest">Point to test</param>
        /// <remarks>
        /// Point must be in pixel units.
        /// </remarks>
        /// <returns>true, if contains point.</returns>
        public virtual bool ContainsPoint(PointF ptTest)
        {
            float fPadding;
            if (HandlesHitTesting.TouchMode)
                fPadding = MeasureUnitsConverter.ToPixelX(CommonUsedValues.TOUCH__HIT_TEST_PADDING, this.BoundsInfo.Unit);
            else
                fPadding = MeasureUnitsConverter.ToPixelX(this.LineHitTestPadding, this.BoundsInfo.Unit);

            bool bSuccess = this.Visible;

            if (bSuccess && m_fPadding != fPadding || m_rgnCache == null)
            {
                // 1 - save padding value;
                m_fPadding = fPadding;

                // 2 - create new region and save it to m_rgnCache;
                UpdateHelperRegion();
            }

            bSuccess = m_rgnCache != null;

            if (bSuccess)
            {
                // check point visibility
                for (int i = 0, nLength = m_rgnCache.Length; i < nLength && bSuccess; i++)
                {
                    bSuccess = m_rgnCache[i].IsVisible(ptTest);
                }
            }

            return bSuccess;
        }
        #endregion

        #region transformations
        /// <summary>
        /// Indicate that selection and resize handles will draw on diagram canvas.
        /// </summary>
        /// <returns>true, if show resize handles.</returns>
        public virtual bool ShowResizeHandles()
        {
            return true;
        }

        /// <summary>
        /// Moves the shape by the given X and Y offsets.
        /// </summary>
        /// <param name="fX">Distance to move along X axis.</param>
        /// <param name="fY">Distance to move along Y axis.</param>
        /// <param name="measureUnits">Specifies translate offsets measure units.</param>
        public void Translate(float fX, float fY, MeasureUnits measureUnits)
        {
            float fPixelX = MeasureUnitsConverter.ConvertX(fX, measureUnits, MeasureUnits.Pixel);
            float fPixelY = MeasureUnitsConverter.ConvertX(fY, measureUnits, MeasureUnits.Pixel);

            Translate(fPixelX, fPixelY);
        }

        /// <summary>
        /// Moves the shape by the given X and Y offsets.
        /// </summary>
        /// <param name="fX">Distance to move along X axis.</param>
        /// <param name="fY">Distance to move along Y axis.</param>
        public void Translate(float fX, float fY)
        {
            if (Math.Round(fX, 1) != 0.0f || Math.Round(fY, 1) != 0.0f)
            {
                MeasureUnits units = MeasureUnits.Pixel;

                // get old pin point
                PointF ptPinPointUnitIndependent = this.BoundsInfo.GetPinPoint(units);

                // calc new pin point
                ptPinPointUnitIndependent.X += fX;
                ptPinPointUnitIndependent.Y += fY;

                // set new pin point
                SetPinPoint(ptPinPointUnitIndependent, units);
            }
        }

        /// <summary>
        /// Rotates the shape a specified number of degrees about its center point.
        /// </summary>
        /// <param name="degrees">Number of degrees to rotate.</param>
        public void Rotate(float degrees)
        {
            // calc current rotating angle in range [0;360]
            // ! Becouse rotation value save in range [ -180; 180 ]
            float angle = Geometry.ConvertToFullCircle(this.RotationAngle);

            this.RotationAngle = angle + degrees;
        }

        /// <summary>
        /// Scales the shape about its center point by a given ratio.
        /// </summary>
        /// <param name="fScaleFactorX">Scaling ratio for X axis.</param>
        /// <param name="fScaleFactorY">Scaling ratio for Y axis.</param>
        public void Scale(float fScaleFactorX, float fScaleFactorY)
        {
            if ((fScaleFactorX <= 0) || fScaleFactorY <= 0)
                throw new ArgumentOutOfRangeException("fScaleFactor");

            if (fScaleFactorX != 1f || fScaleFactorY != 1f)
            {
                this.Size = new SizeF(this.Size.Width * fScaleFactorX, this.Size.Height * fScaleFactorY);
            }
        }

        /// <summary>
        /// Appends filp transformation to given matrix
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void AppendFlipTransforms(Matrix matrix)
        {
            if (m_bFlipX || m_bFlipY)
            {
                PointF ptPinPointUnitIndependent = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);

                // Append flips
                AppendFlipTransforms(matrix, ptPinPointUnitIndependent, m_bFlipX, m_bFlipY);
            }
        }

        /// <summary>
        /// Appends filp transformation to given matrix
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void AppendLocalFlipTransforms(Matrix matrix)
        {
            if (m_bFlipX || m_bFlipY)
            {
                SizeF szPinOffsetUnitIndependent = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

                // Append flips
                AppendFlipTransforms(matrix, szPinOffsetUnitIndependent.ToPointF(), m_bFlipX, m_bFlipY);
            }
        }

        /// <summary>
        /// Gets node's transformations.
        /// </summary>
        /// <returns>The matrix.</returns>
        public Matrix GetTransformations()
        {
            // get PinPoint and PinOffset
            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffet = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

            // rotation angle
            float fAngle = Geometry.ConvertToFullCircle(this.RotationAngle);

            return GetTransformations(ptPinPoint, szPinOffet, fAngle);
        }

        /// <summary>
        /// Gets local node's transformations.
        /// </summary>
        /// <returns>The matrix</returns>
        public Matrix GetLocalTransformations()
        {
            // Get parents rotation angle
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)this).GetPinPointOffset(MeasureUnits.Pixel);

            // rotation angle
            float fAngle = Geometry.ConvertToFullCircle(this.RotationAngle);

            return GetTransformations(szUnitIndependentPinOffset.ToPointF(), szUnitIndependentPinOffset, fAngle);
        }

        /// <summary>
        /// Gets the parent transformation.
        /// </summary>
        /// <param name="bCurrentInclude">if set to <c>true</c> to include current node transformation.</param>
        /// <returns>The matrix</returns>
        protected Matrix GetParentTransformation(bool bCurrentInclude)
        {
            Matrix mtxToReturn = new Matrix();

            if (bCurrentInclude)
            {
                mtxToReturn = GetTransformations();
                AppendFlipTransforms(mtxToReturn);
            }

            Node parentNode = this.Parent as Node;

            if (parentNode != null)
            {
                mtxToReturn.Multiply(parentNode.GetParentTransformation(true), MatrixOrder.Append);
            }

            return mtxToReturn;
        }

        /// <summary>
        /// Appends the flip transforms.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="ptPinPoint">Node's pin point.</param>
        /// <param name="bFlipX">if set to <c>true</c> flip X will flipped vertical.</param>
        /// <param name="bFlipY">if set to <c>true</c> flip Y will flipped horizontal.</param>
        protected void AppendFlipTransforms(Matrix matrix, PointF ptPinPoint, bool bFlipX, bool bFlipY)
        {
            if (bFlipX || bFlipY)
            {
                // Flip transformations            
                // Reset location to origin
                matrix.Translate(-ptPinPoint.X, -ptPinPoint.Y, MatrixOrder.Append);

                // Flip by vertical and horizontal
                if (bFlipX)
                    matrix.Scale(-1.0f, 1.0f, MatrixOrder.Append);

                if (bFlipY)
                    matrix.Scale(1.0f, -1.0f, MatrixOrder.Append);

                // Restore last location
                matrix.Translate(ptPinPoint.X, ptPinPoint.Y, MatrixOrder.Append);
            }
        }

        /// <summary>
        /// Gets the transformations.
        /// </summary>
        /// <param name="ptPinPoint">The pin point.</param>
        /// <param name="szPinOffset">The pin offset.</param>
        /// <param name="fAngle">The rotation angle.</param>
        /// <returns>The matrix</returns>
        protected Matrix GetTransformations(PointF ptPinPoint, SizeF szPinOffset, float fAngle)
        {
            Matrix matrixToReturn = new Matrix();

            // Translate pin on it offsets
            matrixToReturn.Translate(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height, MatrixOrder.Append);

            // Rotate around PinPoint
            matrixToReturn.RotateAt(fAngle, ptPinPoint, MatrixOrder.Append);

            return matrixToReturn;
        }

        /// <summary>
        /// Called when measurement units value changing.
        /// </summary>
        /// <param name="unitsNew">The units new.</param>
        protected virtual void OnMeasurementUnitsChanging(MeasureUnits unitsNew)
        {
            this.ShadowStyle.MeasureUnit = unitsNew;
            this.LineStyle.MeasureUnit = unitsNew;
        }

        /// <summary>
        /// Quites set boundary value without record in history and calling sink events.
        /// </summary>
        /// <param name="bBoundaryConstraintsEnabled">if set to <c>true</c> boundary constraints enabled.</param>
        protected void QuiteBoundarySet(bool bBoundaryConstraintsEnabled)
        {
            // Quite set boundary constrains
            Model model = this.Root;

            // Needed for property change pin point and offset
            // without checking only one changes
            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
                model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }
        #endregion

        #region transform actions
        /// <summary>
        /// Performs additional changes on pin offset value changed.
        /// </summary>
        /// <param name="szOldPinOffset">The old pin offset value.</param>
        /// <param name="szNewPinOffset">The new pin offset value.</param>
        protected virtual void DoPinOffsetRelatedActions(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            Move();

            // update cached bounding rectangle
            //UpdateBoundingRectangle();
        }

        /// <summary>
        /// Performs additional changes on size value changed.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected virtual void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            // update port offset
            UpdatePortPositions(szOldSize, szNewSize);

            // update GrapchsipPath
            UpdateGraphicsPath(szOldSize, szNewSize);

            Move();
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected virtual void DoMoveRelatedActions(float fX, float fY)
        {
            Move();

            // update cached bounding rectangle
            UpdateBoundingRectangle();
        }
        #endregion

        #region Class safe methods
        /// <summary>
        /// Safe pause history recording.
        /// </summary>
        protected void SafeHistoryPause()
        {
            HistoryManager mngHistory = this.HistoryManager;

            if (mngHistory != null)
            {
                mngHistory.Pause();
            }
        }

        /// <summary>
        /// Safe restore history recording.
        /// </summary>
        protected void SafeHistoryResume()
        {
            HistoryManager mngHistory = this.HistoryManager;

            if (mngHistory != null)
            {
                mngHistory.Resume();
            }
        }
        #endregion

        /// <summary>
        /// Called when node scale factor is changed.
        /// </summary>
        /// <param name="strPropertyName">The property name that change scale factor value.</param>
        protected virtual void OnNodeScaleChanged(string strPropertyName)
        { 
        }

        /// <summary>
        /// Draws the node shadow internal.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void DrawShadowInternal(Graphics gfx)
        {
            GraphicsPath gp = this.GraphicsPath;
            RectangleF rcGPBounds = gp.GetBounds();

            if (rcGPBounds.Width == 0)
            {
                gp = new GraphicsPath();
                gp.AddRectangle(new RectangleF(0, 0, this.LineStyle.LineWidth, rcGPBounds.Height));
            }
            if (rcGPBounds.Height == 0)
            {
                gp = new GraphicsPath();
                gp.AddRectangle(new RectangleF(0, 0, rcGPBounds.Width, this.LineStyle.LineWidth));
            }

            if (this.LineStyle.LineWidth == 0) return;

            using (Brush brushShadow = this.ShadowStyle.CreateBrush(gfx, gp.GetBounds()))
            {
                if (!IsClosedPath(gp) || (rcGPBounds.Height == 0) || (rcGPBounds.Width == 0))
                {
                    using (Pen pen = new Pen(Color.FromArgb(80, this.ShadowStyle.Color), this.LineStyle.LineWidth))
                    {
                        gfx.DrawPath(pen, gp);
                    }
                }
                else
                {
                    gfx.FillPath(brushShadow, gp);
                }
            }
        }

        /// <summary>
        /// Updates the bounding rectangle.
        /// </summary>
        protected virtual void UpdateBoundingRectangle()
        {
            // get graphics path bounds
            RectangleF rectBounding = GetPathBounds();

            // get node transformation
            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);

            // set bounding rect
            this.BoundingRect = Geometry.AppendMatrix(rectBounding, mtxTemp);

            // update refresh rect also
            UpdateRefreshRect(rectBounding, mtxTemp);

        }

        /// <summary>
        /// Methods used to get node's GraphicsPath
        /// </summary>
        /// <remarks>
        /// Used primarily for Group node to get
        /// all children paths union with transformations.
        /// </remarks>
        /// <returns>node's GraphicsPath</returns>
        protected virtual RectangleF GetPathBounds()
        {
            RectangleF gPathBounds = this.GraphicsPath.GetBounds();
            gPathBounds = RectangleF.Union(GetPortsOffset(), gPathBounds);

            // consider shadow if it is visible
            if (this.ShadowStyle.Visible)
            {
                float fOffsetX = MeasureUnitsConverter.ConvertX(ShadowStyle.OffsetX, ShadowStyle.MeasureUnit, this.MeasurementUnit);
                float fOffsetY = MeasureUnitsConverter.ConvertY(ShadowStyle.OffsetY, ShadowStyle.MeasureUnit, this.MeasurementUnit);

                RectangleF rcTemp = gPathBounds;
                rcTemp.Offset(fOffsetX, fOffsetY);

                gPathBounds = RectangleF.Union(gPathBounds, rcTemp);
            }
            return gPathBounds;
        }
        private RectangleF GetPortsOffset()
        {
            float maxX = 0f;
            float maxY = 0f;
            float minX = 0f;
            float minY = 0f;
            foreach (ConnectionPoint cnPt in this.Ports)
            {
                maxX = Math.Max(maxX, cnPt.OffsetX);
                maxY = Math.Max(maxY, cnPt.OffsetY);
                minX = Math.Min(minX, cnPt.OffsetX);
                minY = Math.Min(minY, cnPt.OffsetY);
            }
            maxX = (maxX > this.Size.Width) ? maxX : 0f;
            maxY = (maxY > this.Size.Height) ? maxY : 0f;
            return new RectangleF(minX, minY, maxX + (-minX), maxY + (-minY));
        }
        /// <summary>
        /// Updates node's refresh rect
        /// </summary>
        /// <remarks>
        /// Includes all contiguous data like ports, labels etc.
        /// </remarks>
        protected virtual void UpdateRefreshRect()
        {
            // get node transformation
            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);

            // call method with parameters
            UpdateRefreshRect(GetPathBounds(), mtxTemp);
        }

        /// <summary>
        /// Updates node's refresh rect used node graphics path bounds.
        /// </summary>
        /// <param name="rcPathBounds">The node path bounds.</param>
        /// <param name="mtxNodeTransformation">Node matrix transformations.</param>
        protected void UpdateRefreshRect(RectangleF rcPathBounds, Matrix mtxNodeTransformation)
        {
            // accumulate refresh rect
            float fLineWidth = MeasureUnitsConverter.Convert(this.LineStyle.LineWidth / 2, this.LineStyle.MeasureUnit, MeasureUnits.Pixel);
            rcPathBounds.Inflate(fLineWidth, fLineWidth);

            AccumulateRefreshRect(ref rcPathBounds);

            // create rect from transformed points
            m_rectRefresh = Geometry.AppendMatrix(rcPathBounds, mtxNodeTransformation);
        }

        /// <summary>
        /// Accumulates the refresh rect.
        /// </summary>
        /// <param name="rcRefresh">The rc refresh.</param>
        protected virtual void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            // CONNECTION POINTS
            // port location
            PointF ptPLoc;

            // port size
            SizeF szPSize;

            // port bounds in local coordinates
            RectangleF rcPBounds;

            // get connection points positions
            foreach (ConnectionPoint port in this.Ports)
            {
                // get port location
                ptPLoc = port.GetPosition();

                // get port size
                szPSize = port.GraphicsPath.GetBounds().Size;

                // get port bounds
                rcPBounds = Geometry.CreateRect(ptPLoc, szPSize);

                // merge port bounding rect with current refresh rect
                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcPBounds;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcPBounds);
                }
            }

            // NODE's SHADOW
            if (this.ShadowStyle.Visible)
            {
                GraphicsPath gpShadow = this.GraphicsPath;
                RectangleF rcShadow = gpShadow.GetBounds();

                if (!rcShadow.Size.IsEmpty)
                {
                    if (rcShadow.Width == 0)
                    {
                        gpShadow = new GraphicsPath();
                        gpShadow.AddRectangle(new RectangleF(0, 0, 1, rcShadow.Height));
                    }
                    else if (rcShadow.Height == 0)
                    {
                        gpShadow = new GraphicsPath();
                        gpShadow.AddRectangle(new RectangleF(0, 0, rcShadow.Width, 1));
                    }

                    rcShadow = gpShadow.GetBounds();
                    rcShadow.Offset(this.ShadowStyle.OffsetX, this.ShadowStyle.OffsetY);

                    // merge shadow rect with current refresh rect
                    if (rcRefresh.Size.IsEmpty)
                    {
                        rcRefresh = rcShadow;
                    }
                    else
                    {
                        rcRefresh = RectangleF.Union(rcRefresh, rcShadow);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the node scale transformation.
        /// </summary>
        /// <returns>The matrix</returns>
        protected Matrix GetScaleTransformation()
        {
            Matrix mtxScale = this.NodeScale.GetScaleTransformation(this.MeasurementUnit);
            Node parent = this.Parent as Node;

            while (parent != null)
            {
                mtxScale.Multiply(parent.NodeScale.GetScaleTransformation(this.MeasurementUnit));
                parent = parent.Parent as Node;
            }

            Model model = this.Root;

            if (model != null && model.DocumentScale.GetScaleFactor(this.MeasurementUnit) != 1f)
                mtxScale.Multiply(model.DocumentScale.GetScaleTransformation(this.MeasurementUnit));

            return mtxScale;
        }

        /// <summary>
        /// Converts given point from model's to node's coordinates.
        /// </summary>
        /// <param name="ptToConvert">Point to convert.</param>
        /// <returns>Converted point.</returns>
        public PointF ConvertToNodeCoordinates(PointF ptToConvert)
        {
            return ConvertPoint(ptToConvert, true);
        }

        /// <summary>
        /// Converts given point from node's to model's coordinates.
        /// </summary>
        /// <param name="ptToConvert">Point to convert.</param>
        /// <returns>Converted point.</returns>
        public PointF ConvertToModelCoordinates(PointF ptToConvert)
        {
            return ConvertPoint(ptToConvert, false);
        }
        #endregion

        #region Class virtual methods
        /// <summary>
        /// Moves the specified offset.
        /// </summary>
        /// <param name="fX">The x offset.</param>
        /// <param name="fY">The y offset.</param>
        protected void Move(float fX, float fY)
        {
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;

                DoMoveRelatedActions(fX, fY);

                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Called after change the flipX value.
        /// </summary>
        /// <param name="value">New flip x value.</param>
        protected virtual void ChangeFlipX(bool value)
        {
            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.SynchronizeNodeConnections(this);

            // update node connections
            UpdateContainerBounds();

            if (m_mgrBridge != null)
            {
                m_mgrBridge.EndUpdateIntersection();
            }
        }

        /// <summary>
        /// Called after change the flipY value.
        /// </summary>
        /// <param name="value">New flip y value.</param>
        protected virtual void ChangeFlipY(bool value)
        {
            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.SynchronizeNodeConnections(this);

            // update node connections
            UpdateContainerBounds();

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();
        }

        /// <summary>
        /// Called after change the rotation by give angle.
        /// </summary>
        /// <param name="fRotationChange">The rotation angle offset.</param>
        protected virtual void ChangeRotationBy(float fRotationChange)
        {
            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.SynchronizeNodeConnections(this);

            // update node connections
            UpdateContainerBounds();

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();
        }

        /// <summary>
        /// Sets the pin offset.
        /// </summary>
        /// <param name="szOldPinOffset">The old pin offset.</param>
        /// <param name="szNewPinOffset">The new pin offset.</param>
        protected void SetPinOffset(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;

                DoPinOffsetRelatedActions(szOldPinOffset, szNewPinOffset);

                m_bLockUpdate = false;
            }

            // Update cached bounding rect
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Used to update child nodes sizes.
        /// </summary>
        /// <param name="szOldSize">Old size.</param>
        /// <param name="szNewSize">New size.</param>
        protected void SetSize(SizeF szOldSize, SizeF szNewSize)
        {
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;

                DoSizeRelatedActions(szOldSize, szNewSize);

                m_bLockUpdate = false;
            }
            //Update Connector's Port positions
            if (this is ConnectorBase)
                UpdatePortPositions(szOldSize, szNewSize);
            
            // Update cached bounding rect
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Called when parent changing.
        /// </summary>
        protected virtual void OnParentChanging()
        { 
        }

        /// <summary>
        /// Called when parent changed.
        /// </summary>
        protected virtual void OnParentChanged()
        {
            // update parent bounds
            UpdateContainerBounds();
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void UpdateReferences(IServiceReferenceProvider provider)
        {
            // update style references
            this.LineStyle.UpdateServiceReferences(provider);
            this.EditStyle.UpdateServiceReferences(provider);
            this.ShadowStyle.UpdateServiceReferences(provider);
            this.BoundsInfo.UpdateServiceReferences(provider);
            this.Ports.UpdateServiceReferences(provider);

            foreach (ConnectionPoint port in this.Ports)
            {
                port.UpdateServiceReferences(provider);
            }

            if (provider != null)
            {
                m_eventSink = (DocumentEventSink)provider.ProvideServiceReference(typeof(DocumentEventSink).TypeHandle);

                // sign to property change
                if (m_eventSink != null)
                {
                    m_eventSink.PropertyChanging += new PropertyChangingEventHandler(EventSink_PropertyChanging);
                    m_eventSink.PropertyChanged += new PropertyChangedEventHandler(EventSink_PropertyChanged);
                }
            }
            else
            {
                if (m_eventSink != null)
                {
                    m_eventSink.PropertyChanging -= new PropertyChangingEventHandler(EventSink_PropertyChanging);
                    m_eventSink.PropertyChanged -= new PropertyChangedEventHandler(EventSink_PropertyChanged);
                }
            }
        }

        /// <summary>
        /// Logicals the unit change.
        /// </summary>
        /// <param name="unit">The unit.</param>
        protected virtual void LogicalUnitChange(GraphicsUnit unit)
        { 
        }

        /// <summary>
        /// Creates region used for hit testing.
        /// </summary>
        protected virtual void UpdateHelperRegion()
        {
            // save padding value;
            float fPadding;
            if (HandlesHitTesting.TouchMode)
                fPadding = MeasureUnitsConverter.ToPixelX(CommonUsedValues.TOUCH__HIT_TEST_PADDING, this.BoundsInfo.Unit);
            else
                fPadding = MeasureUnitsConverter.ToPixelX(this.LineHitTestPadding, this.BoundsInfo.Unit);

            // 1 - create Pen used to widen cloned GraphicsPath with
            using (Pen pen = new Pen(Color.Black, fPadding))

            // 2 - clone existing shape's GraphicsPath
            using (GraphicsPath pathClone = this.GraphicsPath)
            {
                Matrix matrix = GetTransformations();
                AppendFlipTransforms(matrix);

                pathClone.Transform(matrix);
                RectangleF sz = pathClone.GetBounds();

                if (!(Math.Round(sz.Height) == 0) && !(Math.Round(sz.Width) == 0))
                {
                    // 3 - widen graphics path
                    pathClone.Widen(pen);
                }

                // 4 - create region from widened GraphicsPath
                m_rgnCache = Geometry.CreateRegionFromGraphicsPath(pathClone);
            }
        }

        /// <summary>
        /// Determine whether can widen the specified graphics path.
        /// </summary>
        /// <param name="path">The graphics path.</param>
        /// <returns>
        /// <c>true</c> if can widen the specified graphics path; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanWiden(GraphicsPath path)
        {
            bool bSuccess = false;
            PointF[] pts = path.PathPoints;
            int nLength = pts.Length;

            if (nLength > 1)
            {
                for (int n = 0; nLength - 1 > n; n++)
                {
                    if (pts[n] != pts[n + 1])
                    {
                        bSuccess = true;
                        break;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Raises the mouse single click event.
        /// </summary>
        protected virtual void OnMouseClick( EventArgs e )
        { 
        }

        /// <summary>
        /// Raises the mouse double click event.
        /// </summary>
        protected virtual void OnMouseDoubleClick( EventArgs e )
        { 
        }

        /// <summary>
        /// Raises the mouse enter event.
        /// </summary>
        protected virtual void OnMouseEnter( EventArgs e )
        { 
        }

        /// <summary>
        /// Raises the mouse leave event.
        /// </summary>
        protected virtual void OnMouseLeave( EventArgs e )
        { 
        }

        /// <summary>
        /// Set the pin point position.
        /// </summary>
        /// <param name="ptValue">The pin position.</param>
        /// <param name="unit">The measure unit.</param>
        protected virtual void SetPinPoint(PointF ptValue, MeasureUnits unit)
        {
            if (CheckNewPinPoint(ptValue, unit))
            {
                this.BoundsInfo.SetPinPoint(ptValue, unit);

                UpdateContainerBounds();
            }
        }

        /// <summary>
        /// Raise when node property changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void EventSink_PropertyChanging(PropertyChangingEventArgs evtArgs)
        { 
        }

        /// <summary>
        /// Raise when node property changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void EventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.DocumentScale || evtArgs.PropertyName == "NodeScale.Width" || evtArgs.PropertyName == "NodeScale.Height")
            {
                Model model = this.Root;

                if (model != null)
                {
                    model.EventSink.Pause();
                    model.HistoryManager.Pause();
                    model.BeginUpdate();
                }

                OnNodeScaleChanged(evtArgs.PropertyName);
                m_rgnCache = null;
                UpdateBoundingRectangle();

                // update containers's bounding rect
                UpdateContainerBounds();

                if (model != null)
                {
                    model.EndUpdate();
                    model.HistoryManager.Resume();
                    model.EventSink.Resume();
                }
            }
        }

        /// <summary>
        /// Create the bounds info container.
        /// </summary>
        protected void CreateBoundsInfo()
        {
            CreateBoundsInfo(PointF.Empty, SizeF.Empty, SizeF.Empty);
        }

        /// <summary>
        /// Creates the bounds info.
        /// </summary>
        /// <param name="ptPinPoint">The pin point position.</param>
        /// <param name="szPinPointOffset">The pin point offset.</param>
        /// <param name="szSize">The size.</param>
        protected void CreateBoundsInfo(PointF ptPinPoint, SizeF szPinPointOffset, SizeF szSize)
        {
            // reset previous service references
            if (m_boundsInternal != null)
                m_boundsInternal.UpdateServiceReferences(null);

            // set new value
            m_boundsInternal = new BoundsInfo(
                ptPinPoint, 
                new MoveCallback(this.Move),
                szPinPointOffset, 
                new PinOffsetCallback(this.SetPinOffset),
                szSize, 
                new SizeCallback(this.SetSize));

            // update service references
            m_boundsInternal.UpdateServiceReferences(this);
        }

        /// <summary>
        /// Creates the bounds info.
        /// </summary>
        /// <param name="src">The primary object.</param>
        protected void CreateBoundsInfo(BoundsInfo src)
        {
            CreateBoundsInfo(src, (PageScale)src.NodeScale.Clone());
        }

        /// <summary>
        /// Creates the bounds info.
        /// </summary>
        /// <param name="src">The primary object.</param>
        /// <param name="scale">The scale.</param>
        protected void CreateBoundsInfo(BoundsInfo src, PageScale scale)
        {
            // reset previous service references
            if (m_boundsInternal != null)
                m_boundsInternal.UpdateServiceReferences(null);

            // set new value
            m_boundsInternal = new BoundsInfo(src, new MoveCallback(this.Move), new PinOffsetCallback(this.SetPinOffset), new SizeCallback(this.SetSize), scale);

            // update service references
            m_boundsInternal.UpdateServiceReferences(this);
        }
        #endregion

        #region Class helper methods
        private PointF ConvertPoint(PointF ptToConvert, bool bToNodeCoords)
        {
            PointF[] pts = new PointF[] { ptToConvert };

            Matrix mtx = HandlesHitTesting.GetParentsTransformations(this, true);

            if (bToNodeCoords)
                mtx.Invert();

            mtx.TransformPoints(pts);

            return pts[0];
        }
        private void Move()
        {
            // reset cached Region
            m_rgnCache = null;

            // update node connections
            if (m_mgrLink != null)
            {
                m_mgrLink.SynchronizeNodeConnections(this);
            }
        }

        /// <summary>
        /// Update center point in port collections.
        /// </summary>
        private void UpdateCentralPort()
        {
            // if reference missing then find central port in collection
            if (m_centralPort == null)
            {
                foreach (ConnectionPoint port in m_ports)
                {
                    m_centralPort = port as CentralPort;

                    if (m_centralPort != null)
                        break;
                }
            }

            // remove centranl port
            if (!m_bEnableCentralPort)
            {
                // remove center port from collection
                if (m_ports.Contains(m_centralPort))
                {
                    m_ports.Remove(m_centralPort);
                    m_centralPort.DisconnectAll();
                }
            }
            else
            {
                // if central port can't find in collection - create new
                if (m_centralPort == null)
                {
                    // create ceter port
                    m_centralPort = new CentralPort();
                    m_centralPort.Container = this;
                    m_centralPort.UpdateServiceReferences(this);
                }

                // add center port to collection if port missing
                if (m_centralPort != null && !m_ports.Contains(m_centralPort))
                {
                    m_ports.Add(m_centralPort);
                }
            }
        }

        /// <summary>
        /// Update port positions on change node size.
        /// </summary>
        /// <param name="szOldSize">Old size.</param>
        /// <param name="szNewSize">New size.</param>
        protected void UpdatePortPositions(SizeF szOldSize, SizeF szNewSize)
        {
            float fFactorX = (szOldSize.Width != 0) ? szNewSize.Width / szOldSize.Width : 1;
            float fFactorY = (szOldSize.Height != 0) ? szNewSize.Height / szOldSize.Height : 1;

            foreach (ConnectionPoint port in Ports)
            {
                port.OffsetX = port.OffsetX * fFactorX;
                port.OffsetY = port.OffsetY * fFactorY;
            }
        }

        /// <summary>
        /// Updates the container bounds.
        /// </summary>
        internal protected void UpdateContainerBounds()
        {
            UpdateBoundingRectangle();
            // update parent bounds
            if (this.Parent != null && !m_bLockUpdate)
            {
                this.Parent.UpdateCompositeBounds();
            }
        }

        /// <summary>
        /// Determines whether this EndPoint can update connections the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>
        /// <c>true</c> if this EndPoint can update connections the specified end point; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanUpdateConnections(EndPoint endPoint)
        {
            // check for available port container and if his parent not as
            return (endPoint != null && endPoint.Port != null && endPoint.Port.Container != this && endPoint.Port.Container.Parent != this);
        }

        /// <summary>
        /// Determines whether specified shape is closed.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the specified shape is closed ; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsClosedPath(GraphicsPath path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            bool bClosePath = false;

            if (path.PathData.Points.Length > 0)
            {
                // get path points types
                byte[] pathTypes = path.PathTypes;

                if ((pathTypes[pathTypes.Length - 1] & 128) == 128)
                {
                    bClosePath = true;
                }
            }

            return bClosePath;
        }

        /// <summary>
        /// Gets the cached bounds rectangle from given node.
        /// </summary>
        /// <param name="node">The node to get BoundRect property value.</param>
        /// <returns>The bounds.</returns>
        protected RectangleF GetBoundsRect(Node node)
        {
            return node.BoundingRect;
        }

        /// <summary>
        /// Updates the graphics path and region.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected virtual void UpdateGraphicsPath(SizeF szOldSize, SizeF szNewSize)
        {
            // 1 - calc scale factors
            // prevent divide by zero
            float fScaleFactorX = (szOldSize.Width == 0) ? 1 : szNewSize.Width / szOldSize.Width;
            float fScaleFactorY = (szOldSize.Height == 0) ? 1 : szNewSize.Height / szOldSize.Height;

            // 2 - apply scale factor to graphicspath and region
            Matrix matrixScale = new Matrix(fScaleFactorX, 0, 0, fScaleFactorY, 0, 0);

            // 3 - apply matrix to shape's GraphicsPath
            this.LogicalGraphicsPath.Transform(matrixScale);
        }

        /// <summary>
        /// Gets the upper left point in given units.
        /// </summary>
        /// <param name="unit">The graphics unit.</param>
        /// <returns>The point.</returns>
        protected PointF GetUpperLeftPoint(MeasureUnits unit)
        {
            return this.BoundsInfo.GetUpperLeftPoint(unit);
        }

        private bool TryChangeVisibility(bool bVisible)
        {
            bool bSuccess = true;
            Layer layerTemp;

            if (m_layers != null && bVisible)
            {
                int nLayers = m_layers.Count;

                // visibility state can be changed only if
                // all containing layers have the same visibility state
                for (int nCounter = 0; nCounter < nLayers; nCounter++)
                {
                    layerTemp = m_layers[nCounter];

                    if (!layerTemp.Visible)
                    {
                        bSuccess = false;
                        break;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Updates the ports connections.
        /// </summary>
        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Property name.</param>
        protected void RecordPropertyChanged(string strPropertyName)
        {
            if (m_mgrHistory != null)
            {
                m_mgrHistory.RecordPropertyChanged(this, this.FullContainerName, strPropertyName);
            }
        }

        /// <summary>
        /// Called when rotation changing.
        /// </summary>
        /// <param name="fRotationOffset">The rotation change.</param>
        /// <returns>true, if rotationg changing.</returns>
        private bool OnRotationChanging(float fRotationOffset)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                bSuccess = this.EventSink.RaiseRotationChanging(new RotationChangingEventArgs(this, fRotationOffset));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when rotation changed.
        /// </summary>
        /// <param name="fRotationOffset">The rotation change.</param>
        private void OnRotationChanged(float fRotationOffset)
        {
            if (this.EventSink != null)
            {
                this.EventSink.RaiseRotationChanged(new RotationChangedEventArgs(this, fRotationOffset));
            }
        }

        /// <summary>
        /// Called when flip X changing.
        /// </summary>
        /// <param name="bFlipX">if set to <c>true</c> flipped horizontal .</param>
        /// <returns>true, if flip x changing.</returns>
        private bool OnFlipXChanging(bool bFlipX)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                bSuccess = this.EventSink.RaiseFlipChanging(new FlipChangingEventArgs(this, FlipAxis.AxisX, bFlipX));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when flip X changed.
        /// </summary>
        /// <param name="bFlipX">if set to <c>true</c> flipped horizontal.</param>
        private void OnFlipXChanged(bool bFlipX)
        {
            if (this.EventSink != null)
            {
                this.EventSink.RaiseFlipChanged(new FlipChangedEventArgs(this, FlipAxis.AxisX, bFlipX));
            }
        }

        /// <summary>
        /// Called when flip Y changing.
        /// </summary>
        /// <param name="bFlipY">if set to <c>true</c> flipped vertical.</param>
        /// <returns>true, if flip Y changing.</returns>
        private bool OnFlipYChanging(bool bFlipY)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                bSuccess = this.EventSink.RaiseFlipChanging(new FlipChangingEventArgs(this, FlipAxis.AxisY, bFlipY));
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when Flip Y changed.
        /// </summary>
        /// <param name="bFlipY">if set to <c>true</c> flip vertical.</param>
        private void OnFlipYChanged(bool bFlipY)
        {
            if (this.EventSink != null)
            {
                this.EventSink.RaiseFlipChanged(new FlipChangedEventArgs(this, FlipAxis.AxisY, bFlipY));
            }
        }
        #endregion

        #region Class check constraining
        /// <summary>
        /// Checks the constraining region.
        /// </summary>
        /// <param name="ptPoint">The given point.</param>
        /// <returns>true, if check constraining region</returns>
        protected bool CheckConstrainingRegion(PointF ptPoint)
        {
            return CheckConstrainingRegion(new RectangleF(ptPoint, SizeF.Empty));
        }

        /// <summary>
        /// Checks the constraining region.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>true, if check constraining region.</returns>
        protected virtual bool CheckConstrainingRegion(RectangleF bounds)
        {
            bool bSuccess = true;
            Model model = this.Root;

            if (model != null && model.BoundaryConstraintsEnabled)
            {
                // get model bounds to check rectangle contains
                SizeF szSize = MeasureUnitsConverter.ToPixels(model.LogicalSize, model.MeasurementUnits);
                RectangleF rectModel = new RectangleF(new PointF(0, 0), szSize);

                // check for contains rectangle
                if (!rectModel.Contains(bounds))
                {
                    bSuccess = false;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Checks the constraining region.
        /// </summary>
        /// <param name="matrixTransformation">The matrix append.</param>
        /// <param name="ptPinPoint">Node's pin point.</param>
        /// <param name="szSize">Node's size.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckConstrainingRegion(Matrix matrixTransformation, PointF ptPinPoint, SizeF szSize)
        {
            bool bContains = true;

            if (this.Root != null && this.Root.BoundaryConstraintsEnabled && !this.BoundsInfo.IsResizing)
            {
                // Get current node bounds
                RectangleF rectBounding = new RectangleF(Point.Empty, szSize);

                // get round bounds points
                PointF[] pts = new PointF[]
                {
                    rectBounding.Location,
                    new PointF( rectBounding.Right, rectBounding.Top ),
                    new PointF( rectBounding.Left, rectBounding.Bottom ),
                    new PointF( rectBounding.Right, rectBounding.Bottom )
                };

                PointF[] ptsPin = new PointF[] { ptPinPoint };

                // append parent transformations
                Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(this, false);
                matrixTransformation.Multiply(matrixParent, MatrixOrder.Append);

                // transform bounds points
                matrixTransformation.TransformPoints(pts);
                matrixParent.TransformPoints(ptsPin);
                rectBounding = Geometry.CreateRect(pts);
                rectBounding = RectangleF.Union(rectBounding, new RectangleF(ptsPin[0], SizeF.Empty));

                // check for contains bounds in model
                bContains = CheckConstrainingRegion(rectBounding);
            }

            return bContains;
        }

        /// <summary>
        /// Checks the new pin point.
        /// </summary>
        /// <param name="ptPinPoint">The pin point.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CheckNewPinPoint(PointF ptPinPoint, MeasureUnits unit)
        {
            PointF ptUnitIndependentPinPoint = this.BoundsInfo.GetPinPoint(unit);

            // 1 - check for value change
            bool bConstrain = (ptUnitIndependentPinPoint != ptPinPoint);

            // 2 - check for allow change
            if (bConstrain && ptUnitIndependentPinPoint.X != ptPinPoint.X)
                bConstrain = EditStyle.CanMoveX(this);

            if (bConstrain && ptUnitIndependentPinPoint.Y != ptPinPoint.Y)
                bConstrain = EditStyle.CanMoveY(this);

            // 3 - check for model contains
            if (bConstrain && this.Root != null && this.Root.BoundaryConstraintsEnabled)
            {
                SizeF szPinOffset = this.BoundsInfo.GetPinOffset(unit);
                SizeF szSize = this.BoundsInfo.GetSize(unit);
                float angle = Geometry.ConvertToFullCircle(this.RotationAngle);

                // create matrix transformations
                Matrix mtxTemp = GetTransformations(ptPinPoint, szPinOffset, angle);

                // Append flips
                AppendFlipTransforms(mtxTemp, ptPinPoint, m_bFlipX, m_bFlipY);

                bConstrain = CheckConstrainingRegion(mtxTemp, ptPinPoint, szSize);
            }

            return bConstrain;
        }

        /// <summary>
        /// Checks the new pin offset.
        /// </summary>
        /// <param name="szPinOffset">The pin offset.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckNewPinOffset(SizeF szPinOffset, MeasureUnits unit)
        {
            SizeF szUnitIndependentPinOffset = this.BoundsInfo.GetPinOffset(unit);

            // 1 - check for value change
            bool bConstrain = (szUnitIndependentPinOffset != szPinOffset);

            // 2 - check for negatiev or zero value
            if (!bConstrain && szPinOffset.Width <= 0)
                bConstrain = false;

            if (!bConstrain && szPinOffset.Height <= 0)
                bConstrain = false;

            // 3 - check for allow change
            if (bConstrain && szUnitIndependentPinOffset.Width != szPinOffset.Width)
                bConstrain = EditStyle.CanMoveX(this);

            if (bConstrain && szUnitIndependentPinOffset.Height != szPinOffset.Height)
                bConstrain = EditStyle.CanMoveY(this);

            // 4 - check for model contains
            if (bConstrain)
            {
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(unit);
                SizeF szSize = this.BoundsInfo.GetSize(unit);
                float angle = Geometry.ConvertToFullCircle(this.RotationAngle);

                // create matrix transformations
                Matrix mtxTemp = GetTransformations(ptPinPoint, szPinOffset, angle);

                // Append flips
                AppendFlipTransforms(mtxTemp, ptPinPoint, m_bFlipX, m_bFlipY);

                bConstrain = CheckConstrainingRegion(mtxTemp, ptPinPoint, szSize);
            }

            return bConstrain;
        }

        /// <summary>
        /// Checks the new size.
        /// </summary>
        /// <param name="szSize">The new size.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckNewSize(SizeF szSize, MeasureUnits unit)
        {
            SizeF szUnitIndependentSize = this.BoundsInfo.GetSize(unit);

            // 1 - check for value change
            bool bConstrain = (szUnitIndependentSize != szSize);

            // 2 - check for negatiev or zero value
            if (!bConstrain && szSize.Width <= 0)
                bConstrain = false;

            if (!bConstrain && szSize.Height <= 0)
                bConstrain = false;

            // 3 - check for allow change
            if (bConstrain && szUnitIndependentSize.Width != szSize.Width)
                bConstrain = EditStyle.CanChangeWidth(this);

            if (bConstrain && szUnitIndependentSize.Height != szSize.Height)
                bConstrain = EditStyle.CanChangeHeight(this);

            // 4 - check for model contains
            if (bConstrain)
            {
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(unit);
                SizeF szPinOffset = this.BoundsInfo.GetPinOffset(unit);
                SizeF szOldSize = this.BoundsInfo.GetSize(unit);
                float angle = Geometry.ConvertToFullCircle(this.RotationAngle);

                float fPinOffsetWidthFactor = (szOldSize.Width != 0) ? szPinOffset.Width / szOldSize.Width : 1;
                float fPinOffsetHeigthFactor = (szOldSize.Height != 0) ? szPinOffset.Height / szOldSize.Height : 1;

                SizeF szNewPinOffset = new SizeF(szSize.Width * fPinOffsetWidthFactor, szSize.Height * fPinOffsetHeigthFactor);

                // create matrix transformations
                Matrix mtxTemp = GetTransformations(ptPinPoint, szNewPinOffset, angle);

                // Append flips
                AppendFlipTransforms(mtxTemp, ptPinPoint, m_bFlipX, m_bFlipY);

                bConstrain = CheckConstrainingRegion(mtxTemp, ptPinPoint, szSize);
            }

            return bConstrain;
        }

        /// <summary>
        /// Checks the new rotation angle.
        /// </summary>
        /// <param name="fAngle">The rotation angle.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckNewRotationAngle(float fAngle, MeasureUnits unit)
        {
            fAngle = fAngle % CommonUsedValues.CIRCLE;

            // 1 - check for value change
            bool bConstrain = (Geometry.ConvertToFullCircle(this.RotationAngle) != fAngle);

            // 2 - check for allow change
            if (bConstrain)
                bConstrain = EditStyle.CanRotate(this);

            // 3 - check for model contains
            if (bConstrain)
            {
                // get pin point and pi offset
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(unit);
                SizeF szPinOffset = this.BoundsInfo.GetPinOffset(unit);
                SizeF szSize = this.BoundsInfo.GetSize(unit);

                // Create matrix rotate transform
                Matrix matrixTemp = GetTransformations(ptPinPoint, szPinOffset, fAngle);

                // append flips
                AppendFlipTransforms(matrixTemp);

                bConstrain = CheckConstrainingRegion(matrixTemp, ptPinPoint, szSize);
            }

            return bConstrain;
        }

        /// <summary>
        /// Checks the new flip value.
        /// </summary>
        /// <param name="bFlipX">Flip by vertical if set to <c>true</c>.</param>
        /// <param name="bFlipY">Flip by horizontal if set to <c>true</c>.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckNewFlipValue(bool bFlipX, bool bFlipY, MeasureUnits unit)
        {
            // 1 - check for value change
            bool bConstrain = (bFlipX != m_bFlipX || bFlipY != m_bFlipY);

            // 2 - check for allow change
            if (bConstrain && bFlipX != m_bFlipX)
                bConstrain = EditStyle.CanChangeWidth(this);

            if (bConstrain && bFlipY != m_bFlipY)
                bConstrain = EditStyle.CanChangeHeight(this);

            // 3 - check for model contains
            if (bConstrain)
            {
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(unit);
                SizeF szSize = this.BoundsInfo.GetSize(unit);

                // create matrix flip transform
                Matrix matrixTemp = this.GetTransformations();

                // append flips
                AppendFlipTransforms(matrixTemp, ptPinPoint, bFlipX, bFlipY);

                bConstrain = CheckConstrainingRegion(matrixTemp, ptPinPoint, szSize);
            }

            return bConstrain;
        }
        #endregion

        #region INode Members
        INode INode.Root
        {
            get
            {
                // TODO:  Add Node.Syncfusion.Windows.Forms.Diagram.INode.Root getter implementation
                return null;
            }
        }
        #endregion

        #region IServiceProvider Members
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(ShadowStyle))
            {
                return this.ShadowStyle;
            }
            else if (serviceType == typeof(LineStyle))
            {
                return this.LineStyle;
            }
            else if (serviceType == typeof(EditStyle))
            {
                return this.EditStyle;
            }
            else if (serviceType == typeof(IServiceReferenceHolder))
            {
                return this;
            }
            else if (serviceType == typeof(IServiceReferenceProvider))
            {
                return this;
            }
            else if (serviceType == typeof(IPropertyObserver))
            {
                return this;
            }
            else if (serviceType == typeof(IPropertyContainer))
            {
                return this;
            }

            return null;
        }
        #endregion

        #region IUnitIndependent Members
        /// <summary>
        /// Gets the pin point.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The pin point.</returns>
        PointF IUnitIndependent.GetPinPoint(MeasureUnits unit)
        {
            return this.BoundsInfo.GetPinPoint(unit);
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The size.</returns>
        SizeF IUnitIndependent.GetSize(MeasureUnits unit)
        {
            return this.BoundsInfo.GetSize(unit);
        }

        /// <summary>
        /// Gets the pin point offset.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The pin point offset/</returns>
        SizeF IUnitIndependent.GetPinPointOffset(MeasureUnits unit)
        {
            return this.BoundsInfo.GetPinOffset(unit);
        }

        /// <summary>
        /// Gets the bounding rectangle.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="bRelativeToModel">if set to <c>true</c> to get bounds in model coordinates, otherwise - <c>false</c>.</param>
        /// <returns>
        /// The <see cref="System.Drawing.RectangleF"/>.
        /// </returns>
        RectangleF IUnitIndependent.GetBoundingRectangle(MeasureUnits unit, bool bRelativeToModel)
        {
            RectangleF rcBouds;

            if (bRelativeToModel)
            {
                RectangleF rectBounding = GetPathBounds();

                PointF[] pts = new PointF[]
                {
                    rectBounding.Location,
                    new PointF( rectBounding.X + rectBounding.Width, rectBounding.Y ),
                    new PointF( rectBounding.Right, rectBounding.Bottom ),
                    new PointF( rectBounding.X, rectBounding.Y + rectBounding.Height )
                };

                Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(this, true);
                mtxTemp.TransformPoints(pts);

                rcBouds = Geometry.CreateRect(pts);
            }
            else
            {
                rcBouds = MeasureUnitsConverter.FromPixels(this.BoundingRect, unit);
            }

            return rcBouds;
        }

        /// <summary>
        /// Set the pin point.
        /// </summary>
        /// <param name="ptValue">The location value.</param>
        /// <param name="unit">The unit.</param>
        void IUnitIndependent.SetPinPoint(PointF ptValue, MeasureUnits unit)
        {
            SetPinPoint(ptValue, unit);
        }

        /// <summary>
        /// Set the size.
        /// </summary>
        /// <param name="szValue">The new size value.</param>
        /// <param name="unit">The unit.</param>
        void IUnitIndependent.SetSize(SizeF szValue, MeasureUnits unit)
        {
            if (CheckNewSize(szValue, unit))
            {
                this.BoundsInfo.SetSize(szValue, unit);

                if (this.Parent != null)
                    this.Parent.UpdateCompositeBounds();
            }
        }

        /// <summary>
        /// Set the pin point offset.
        /// </summary>
        /// <param name="szValue">The new offset size value.</param>
        /// <param name="unit">The unit.</param>
        void IUnitIndependent.SetPinPointOffset(SizeF szValue, MeasureUnits unit)
        {
            if (CheckNewPinOffset(szValue, unit))
            {
                this.BoundsInfo.SetPinOffset(szValue, unit);
            }
        }
        #endregion

        #region IDispatchNodeEvents Members
        void IDispatchNodeEvents.Click(EventArgs e)
        {
            OnMouseClick(e);
        }
        void IDispatchNodeEvents.DoubleClick(EventArgs e)
        {
            OnMouseDoubleClick(e);
        }
        void IDispatchNodeEvents.MouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }
        void IDispatchNodeEvents.MouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }
        #endregion

        #region IGraphNode Members
        /// <summary>
        /// Gets collection of all edges entering or leaving the node.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public ICollection Edges
        {
            get
            {
                ArrayList arrEdges = new ArrayList();

                foreach (ConnectionPoint port in this.Ports)
                {
                    foreach (EndPoint endpoint in port.Connections)
                    {
                        if (endpoint.Container != null)
                        {
                            arrEdges.Add(endpoint.Container);
                        }
                    }
                }

                return arrEdges;
            }
        }

        /// <summary>
        /// Gets collection of edges entering the node.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public ICollection EdgesEntering
        {
            get
            {
                ArrayList arrEdges = new ArrayList();

                foreach (ConnectionPoint port in this.Ports)
                {
                    foreach (EndPoint endpoint in port.Connections)
                    {
                        if (endpoint is HeadEndPoint && endpoint.Container != null)
                        {
                            arrEdges.Add(endpoint.Container);
                        }
                    }
                }

                return arrEdges;
            }
        }

        /// <summary>
        /// Gets collection of edges leaving the node.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public ICollection EdgesLeaving
        {
            get
            {
                ArrayList arrEdges = new ArrayList();

                foreach (ConnectionPoint port in this.Ports)
                {
                    foreach (EndPoint endpoint in port.Connections)
                    {
                        if (endpoint is TailEndPoint && endpoint.Container != null)
                        {
                            arrEdges.Add(endpoint.Container);
                        }
                    }
                }

                return arrEdges;
            }
        }
        #endregion

        #region IDeserializationCallback Members
        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
           OnDeserialized();
        }

        /// <summary>
        /// Called when node is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            foreach (ConnectionPoint port in this.Ports)
            {
                port.Container = this;
            }

            UpdateBoundingRectangle();
        }
        #endregion
    }
}
