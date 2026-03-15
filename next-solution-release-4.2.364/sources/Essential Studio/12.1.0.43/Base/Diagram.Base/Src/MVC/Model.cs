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
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Syncfusion.Documentation;
using ComponentModel_DesignerSerializationVisibility = System.ComponentModel.DesignerSerializationVisibility;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A model is a collection of nodes that are rendered onto a view and
    /// manipulated by a controller.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A model is the data portion of a diagram. It is the root node in a hierarchy
    /// that is rendered onto a view. This class implements the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.ICompositeNode"/> interface,
    /// provides methods for accessing, adding, and removing child nodes. This
    /// includes the following methods:
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.ChildCount"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.GetChild"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.AppendChild"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.InsertChild"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.RemoveChild(int)"/>. Child
    /// nodes in the model can also be accessed through the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.Nodes"/> property.
    /// </para>
    /// <para>
    /// A model maintains a collection of layers in the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.Layers"/>
    /// property. A <see cref="Syncfusion.Windows.Forms.Diagram.Layer"/> is a
    /// collection of nodes that share a common set of default properties and the
    /// same Z-order relative to other layers. A model always contains at least one
    /// layer. Each node in the model belongs to one and only one layer. The model
    /// renders itself onto the view by iterating through the layers and rendering
    /// each one. Each layer is responsible for rendering the nodes belonging to
    /// it. The
    /// <see cref="PropertyContainer"/>
    /// of the <see cref="Syncfusion.Windows.Forms.Diagram.Model.Layers"/>
    /// collection is a reference back to the model, which allows the layers to
    /// inherit properties from the model.
    /// </para>
    /// <para>
    /// A model contains document-level settings such as:
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.MeasurementUnits"/>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ICompositeNode"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IPrint"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    [Serializable]
    [ToolboxItem(false)]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class Model
        : Component,
          INode,
          ICompositeNode,
          IZOrderContainer,
          ISerializable,
          IServiceProvider,
          IDeserializationCallback,
          ISupportInitialize,
          IServiceReferenceProvider,
          IPropertyObserver,
          IPropertyContainer,
          ILayerContainer
    {
        #region Class members
        /// <summary>
        /// Store bridge style.
        /// </summary>
        private BridgeStyle m_bridgeStyle;

        private Hashtable m_lastLineNodeStates = new Hashtable();

        /// <summary>
        /// Indicates whether selection list will be replaced
        /// with pseudo group to enhance selection list rendering speed.
        /// </summary>
        private bool m_bEnableSelectionListSubstitute = true;

        /// <summary>
        /// Defult minimum size. Used for checking size to content.
        /// </summary>
        private SizeF m_modelsMinSize;

        /// <summary>
        /// Flag indication autoresizing model size 
        /// on out nodes bounds on model.
        /// </summary>
        private bool m_bSizeToContent;

        /// <summary>
        /// The background image layout for this component.
        /// </summary>
        private ImageLayout m_BackgroundImageLayout = ImageLayout.Tile;

        /// <summary>
        /// The background image for this component.
        /// </summary>
        private Image m_BackgroundImage = null;

        /// <summary>
        /// Name of the model.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Collection of child nodes belonging to the model.
        /// </summary>
        [DocumentationExclude()]
        private NodeCollection m_nodesChildren;

        /// <summary>
        /// Collection of layers in the model.
        /// </summary>
        [DocumentationExclude()]
        private LayerCollection m_layers;

        /// <summary>
        /// Active Layers collection
        /// </summary>
        private LayerCollection m_layersActive;

        /// <summary>
        /// Maps node names to node objects.
        /// </summary>
        [DocumentationExclude()]
        private Hashtable m_tableName;

        /// <summary>
        /// Properties for drawing document interior.
        /// </summary>
        private FillStyle m_styleFill;

        /// <summary>
        /// Properties for drawing lines in the model.
        /// </summary>
        private LineStyle m_styleLine;

        /// <summary>
        /// Properties for shadows in the model.
        /// </summary>
        private ShadowStyle m_styleShadow;

        /// <summary>
        /// Properties for document RenderingStyle.
        /// </summary>
        private RenderingStyle m_styleRendering;

        /// <summary>
        /// Unit of measure used for world coordinates.
        /// </summary>
        private MeasureUnits m_unitMeasure;

        /// <summary>
        /// Flag to indicate if the model has been modified.
        /// </summary>
        private bool m_bModified;

        /// <summary>
        /// Indicates whether or not boundary constraints are enabled.
        /// </summary>
        private bool m_boundaryConstraintsEnabled;

        /// <summary>
        /// Temporarily freezes redrawing of the model.
        /// </summary>
        [DocumentationExclude()]
        protected bool m_bInUpdate;

        /// <summary>
        /// Indicates if the Dispose() method has been called.
        /// </summary>
        [DocumentationExclude()]
        private bool disposed;

        /// <summary>
        /// Reference to the HeaderFooterData class.
        /// </summary>
        private HeaderFooterData m_headerFooter;

        /// <summary>
        /// Reference to line router.
        /// </summary>
        [NonSerialized]
        private LineRouter m_lineRouter;
        private bool optimizeLineBridging = false;
        private bool m_bLineRouting;
        private bool m_bLineBridging;
        private float m_fLineBridgeSize;
        [NonSerialized]
        private HistoryManager m_mgrHistory;
        [NonSerialized]
        private LinkManager m_mgrLink;
        [NonSerialized]
        private BridgeManager m_mgrBridge;
        [NonSerialized]
        private DocumentEventSink m_eventSink;

        /// <summary>
        /// Indicate that node append to model.
        /// </summary>
        private bool m_bNodeAdd = false;
        private PageScale m_pageScale;
        private PageSize m_pageSize;
        private int m_iUpdateRequests = 0;
        private Node m_cloneNode;
        private float m_marginRight;
        private float m_marginBottom;
        internal float m_fMagnification = 100f;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Model(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        public Model()
        {
            m_strName = "Model";
            m_unitMeasure = MeasureUnits.Pixel;

            // set minimum model size.
            m_modelsMinSize = MeasureUnitsConverter.ToPixels(this.DefaultMinimumSize, m_unitMeasure);
            m_boundaryConstraintsEnabled = true;

            // discard lazy initialization in order NodeCollectionChanged event to be handled first
            // it is important to provide ServiceReferences, Name, Parent before anybody else will handle this event
            m_nodesChildren = new NodeCollection(this);
            m_nodesChildren.UpdateServiceReferences(this);
            SubscribeForChildrenEvents();

            m_fLineBridgeSize = 16f;
            m_bModified = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="src">Source model to copy.</param>
        public Model(Model src)
        {
            this.m_strName = src.m_strName;
            m_pageSize = src.m_pageSize;
            m_pageScale = src.m_pageScale;
            m_boundaryConstraintsEnabled = src.m_boundaryConstraintsEnabled;

            // set minimum model size.
            m_modelsMinSize = src.MinimumSize;
            m_bSizeToContent = src.SizeToContent;

            // nodes
            m_nodesChildren = src.m_nodesChildren.Clone() as NodeCollection;
            m_nodesChildren.Container = this;

            SubscribeForChildrenEvents();

            // layers
            m_layers = (LayerCollection)src.Layers.Clone();
            m_layers.Container = this;
            m_layersActive = src.ActiveLayers.Clone() as LayerCollection;
            m_layersActive.Container = this;

            m_headerFooter = (HeaderFooterData)src.HeaderFooterData.Clone();

            // styles
            m_styleFill = (FillStyle)src.BackgroundStyle.Clone();
            m_styleLine = (LineStyle)src.LineStyle.Clone();
            m_styleShadow = (ShadowStyle)src.ShadowStyle.Clone();
            m_styleRendering = (RenderingStyle)src.RenderingStyle.Clone();

            m_tableName = (Hashtable)src.NameTable.Clone();
            m_unitMeasure = src.m_unitMeasure;
            m_bModified = false;

            m_BackgroundImageLayout = (ImageLayout)src.BackgroundImageLayout;
            if (src.m_BackgroundImage != null)
            {
                m_BackgroundImage = (Image)src.m_BackgroundImage.Clone();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected Model(SerializationInfo info, StreamingContext context)
        {
            SizeF documentSize = SizeF.Empty;
            bool bLineRouting = true;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "documentMinSize":
                        m_modelsMinSize = (SizeF)entry.Value;
                        break;
                    case "documentSize":
                        documentSize = (SizeF)entry.Value;
                        break;
                    case "documentScale":
                        m_pageScale = (PageScale)entry.Value;
                        break;
                    case "sizeToContent":
                        m_bSizeToContent = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "boundaryConstrains":
                        m_boundaryConstraintsEnabled = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "name":
                        m_strName = (string)entry.Value;
                        break;
                    case "children":
                        m_nodesChildren = (NodeCollection)entry.Value;
                        break;
                    case "layers":
                        m_layers = (LayerCollection)entry.Value;
                        break;
                    case "activeLayers":
                        m_layersActive = (LayerCollection)entry.Value;
                        break;
                    case "fillStyle":
                        m_styleFill = (FillStyle)entry.Value;
                        break;
                    case "lineStyle":
                        m_styleLine = (LineStyle)entry.Value;
                        break;
                    case "shadowStyle":
                        m_styleShadow = (ShadowStyle)entry.Value;
                        break;
                    case "renderingStyle":
                        m_styleRendering = (RenderingStyle)entry.Value;
                        break;
                    case "measureUnit":
                        m_unitMeasure = (MeasureUnits)entry.Value;
                        break;
                    case "headerFooter":
                        m_headerFooter = (HeaderFooterData)entry.Value;
                        break;
                    case "lineBridging":
                        m_bLineBridging = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "lineBridgeSize":
                        m_fLineBridgeSize = float.Parse(entry.Value.ToString());
                        break;
                    case "lineRouting":
                        bLineRouting = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "pageScale":
                        m_pageScale = (PageScale)entry.Value;
                        break;
                    case "pageSize":
                        m_pageSize = (PageSize)entry.Value;
                        break;
                    case "backgroundImage":
                        m_BackgroundImage = (Image)entry.Value;
                        break;
                    case "backgroundImageLayout":
                        m_BackgroundImageLayout = (ImageLayout)entry.Value;
                        break;
                }
            }

            this.DocumentSize = new PageSize(documentSize.Width, m_unitMeasure, documentSize.Height, m_unitMeasure);
            this.BackgroundStyle.UpdateServiceReferences(this);
            this.LineStyle.UpdateServiceReferences(this);
            this.ShadowStyle.UpdateServiceReferences(this);
            this.RenderingStyle.UpdateServiceReferences(this);

            //update layers owner
            UpdateLayerCollectionOwner(m_layers);
            UpdateLayerCollectionOwner(m_layersActive);                       

            // load lineRouter manager
            if (bLineRouting)
            {
                try
                {
                    this.LineRouter = (LineRouter)info.GetValue("lineRouter", typeof(LineRouter));
                    this.LineRouter.Model = this;
                }
                catch (SerializationException)
                { 
                }
                finally
                {
                    this.LineRoutingEnabled = bLineRouting;
                }
            }

            RegenerateUniqueNames();
            SubscribeForChildrenEvents();

            m_bModified = false;
        }

        /// <summary>
        /// Special method for proper rendering DiagramWebControl Nodes. Don't use this method!
        /// </summary>
        /// <returns>true, if regnerate unique names.</returns>
        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            EditorBrowsable(EditorBrowsableState.Never),
            Description("Special method for unique names regeneration. Don't use this method implicitly!")
        ]
        public bool RegenerateUniqueNames()
        {
            bool bResult = false;
            bool bFirst = true;
            if (this.Children.Count > 0)
            {
                foreach (Node item in this.Children)
                {
                    if (item != null)
                    {
                        if (bFirst)
                        {
                            m_tableName = null;
                            bFirst = false;
                        }
                        GenerateUniqueName(item, HandlesHitTesting.NameIndex);
                    }
                }

                bResult = true;
            }
            return bResult;
        }

        /// <summary>
        /// Called to release resources held by the model.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if this method is being called explicitly by a call to Dispose()
        /// or by the destructor through the garbage collector.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                this.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(Children_ChangeComplete);
                disposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the model bounds.
        /// </summary>
        /// <remarks>
        /// Location  always equal PointF.Empty.
        /// </remarks>
        /// <value>The bounds.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RectangleF Bounds
        {
            get { return new RectangleF(PointF.Empty, this.LogicalSize); }
        }


        /// <summary>
        /// Gets and Sets the Bottom Margin for the Model.
        /// </summary>
        [Browsable(true)]
        [Description("Defines Bottom Margin for the model with SizeToContent flag set.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public float BottomMargin
        {
            get { return m_marginBottom; }
            set
            {
                if (m_marginBottom != value && OnPropertyChanging(DPN.BottomMargin, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.BottomMargin);

                    // assign new value
                    m_marginBottom = value;

                    // raise property changed event
                    OnPropertyChanged(string.Empty, DPN.BottomMargin);
                }
            }

        }
        /// <summary>
        /// Gets and Sets the Right Margin for the Model.
        /// </summary>
        [Browsable(true)]
        [Description("Defines Right Margin for the model with SizeToContent flag set.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public float RightMargin
        {
            get { return m_marginRight; }
            set
            {
                if (m_marginRight != value && OnPropertyChanging(DPN.RightMargin, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.RightMargin);

                    // assign new value
                    m_marginRight = value;

                    // raise property changed event
                    OnPropertyChanged(string.Empty, DPN.RightMargin);
                }
            }
        }



        /// <summary>
        /// Gets the model node collection.
        /// </summary>
        /// <value>The nodes.</value>
        [Browsable(false)]
        public NodeCollection Nodes
        {
            get
            {
                return this.Children;
            }
        }

        /// <summary>
        /// Gets the default document size.
        /// </summary>
        /// <value>The size of the default document.</value>
        protected SizeF DefaultDocumentSize
        {
            get { return MeasureUnitsConverter.Convert(new SizeF(827f, 1169f), MeasureUnits.Pixel, this.MeasurementUnits); }
        }

        /// <summary>
        /// Gets the default minimum size.
        /// </summary>
        /// <value>The minimum size of the default.</value>
        protected SizeF DefaultMinimumSize
        {
            get { return MeasureUnitsConverter.Convert(new SizeF(105f, 150f), MeasureUnits.Millimeter, this.MeasurementUnits); }
        }

        /// <summary>
        /// Gets or sets the Document size.
        /// </summary>
        /// <remarks>
        /// Specified in current measure units.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [TypeConverter(typeof(SizeFConverter))]
        [Obsolete("This property will be no more supported since next version. Use DocumentSize instead.")]
        public SizeF Size
        {
            get { return this.LogicalSize; }
            set { this.LogicalSize = value; }
        }

        /// <summary>
        /// Gets collection of layers in the model.
        /// </summary>
        /// <value>The layer collection.</value>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayerCollection"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Layer"/>
        /// </remarks>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Layers")]
        [Description("Collection of layers in the model")]
        public LayerCollection Layers
        {
            get
            {
                if (m_layers == null)
                {
                    m_layers = new LayerCollection(this);
                    m_layers.UpdateServiceReferences(this);
                }

                return m_layers;
            }
        }

        /// <summary>
        /// Gets collection of active layers.
        /// </summary>
        /// <value></value>
        /// <remark>
        /// Adding new node to ILayerContainer would add it to all layers
        /// contained in this collection
        /// </remark>
        [Browsable(false)]
        public LayerCollection ActiveLayers
        {
            get
            {
                if (m_layersActive == null)
                {
                    m_layersActive = new LayerCollection(this);
                    m_layersActive.UpdateReferences = false;
                }

                return m_layersActive;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterData"/> instance containing the header and footer settings.
        /// </summary>
        [Browsable(true)]
        [Category("Header and Footer")]
        [Description("Specifies the header and footer data for the diagram.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public HeaderFooterData HeaderFooterData
        {
            get
            {
                if (this.m_headerFooter == null)
                    this.m_headerFooter = this.CreateHeaderFooter();

                return this.m_headerFooter;
            }
        }

        #region Behavior
        /// <summary>
        /// Gets the reference to event sink .
        /// </summary>
        /// <value>The event sink.</value>
        [Browsable(false)]
        public DocumentEventSink EventSink
        {
            get
            {
                if (m_eventSink == null)
                {
                    m_eventSink = new DocumentEventSink();
                    m_eventSink.Start();

                    AddDocumentSinkEvents();
                }

                return m_eventSink;
            }
        }

        /// <summary>
        /// Gets the reference to history manager.
        /// </summary>
        /// <value>The history manager.</value>
        [Browsable(false)]
        public HistoryManager HistoryManager
        {
            get
            {
                if (m_mgrHistory == null)
                {
                    m_mgrHistory = new HistoryManager(this);
                    m_mgrHistory.Start();
                }

                return m_mgrHistory;
            }
            set
            {
                if (value != null)
                {
                    m_mgrHistory = value;
                    m_mgrHistory.Start();
                    m_nodesChildren.UpdateServiceReferences(this);
                    m_layers.UpdateServiceReferences(this);
                }
            }
        }

        /// <summary>
        /// Gets the reference to link manager.
        /// </summary>
        /// <value>The link manager.</value>
        [Browsable(false)]
        public LinkManager LinkManager
        {
            get
            {
                if (m_mgrLink == null)
                {
                    m_mgrLink = new LinkManager(this);
                    m_mgrLink.Start();
                }

                return m_mgrLink;
            }
        }

        /// <summary>
        /// Gets the reference to bridge manager.
        /// </summary>
        /// <value>The bridge manager.</value>
        [Browsable(false)]
        public BridgeManager BridgeManager
        {
            get
            {
                if (m_mgrBridge == null)
                {
                    m_mgrBridge = new BridgeManager(this);
                    m_mgrBridge.Start();
                }

                return m_mgrBridge;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Model"/> is modified.
        /// </summary>
        /// <value><c>true</c> if modified; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get { return m_bModified; }
            set { m_bModified = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line routing is enabled for the model.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Misc")]
        [DefaultValue(true)]
        [Description("Determines if enhanced selection list rendering mode is enabled.")]
        public bool EnableSelectionListSubstitute
        {
            get 
            { 
                return m_bEnableSelectionListSubstitute; 
            }
            set
            {
                if (m_bEnableSelectionListSubstitute != value && OnPropertyChanging(DPN.EnableSelectionListSubstitute, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.EnableSelectionListSubstitute);
                    //// assign new value
                    m_bEnableSelectionListSubstitute = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.EnableSelectionListSubstitute);
                }
            }
        }

        /// <summary>
        /// Gets or sets bridge style.
        /// </summary>
        [
        Browsable(true), DefaultValue(Diagram.BridgeStyle.Arc), Category("Line Routing"),
        Description("Gets or sets bridge style for all connectors in document.")
        ]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BridgeStyle BridgeStyle
        {
            get
            {
                return m_bridgeStyle;
            }
            set
            {
                if (m_bridgeStyle != value && OnPropertyChanging(DPN.BridgeStyle, value))
                {
                    m_bridgeStyle = value;

                    if (HistoryManager != null)
                    {
                        HistoryManager.StartAtomicAction("Set bridge style");
                    }

                    foreach (Node node in this.Nodes)
                    {
                        ConnectorBase connector = node as ConnectorBase;

                        if (connector != null)
                        {
                            connector.BridgeStyle = value;
                        }
                    }

                    if (HistoryManager != null)
                    {
                        HistoryManager.EndAtomicAction();
                    }

                    OnPropertyChanged(DPN.BridgeStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line routing is enabled for the model.
        /// </summary>
        [Browsable(true)]
        [Category("Line Routing")]
        [DefaultValue(false)]
        [Description("Determines if line routing is enabled for this model.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool LineRoutingEnabled
        {
            get 
            { 
                return m_bLineRouting; 
            }
            set
            {
                if (m_bLineRouting != value && OnPropertyChanging(DPN.LineRoutingEnabled, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineRoutingEnabled);

                    // assign new value
                    m_bLineRouting = value;

                    if (m_bLineRouting)
                    {
                        if (this.LineRouter == null)
                        {
                            AStarLineRouter lineRouter = new AStarLineRouter(this);
                            this.LineRouter = lineRouter;
                        }
                    }

                    // raise property changed event
                    OnPropertyChanged(DPN.LineRoutingEnabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the performance will be improved while dragging nodes.
        /// </summary>
        [Browsable(true)]
        [Category("Line Routing")]
        [Description("Improves the performance while dragging nodes.")]
        [DefaultValue(false)]
        public bool OptimizeLineBridging
        {
            get
            {
                return optimizeLineBridging;
            }
            set
            {
                optimizeLineBridging = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line bridging is enabled for the model.
        /// </summary>
        [Browsable(true)]
        [Category("Line Routing")]
        [DefaultValue(false)]
        [Description("Determines if line bridging is enabled for this model.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool LineBridgingEnabled
        {
            get 
            {
                return m_bLineBridging; 
            }
            set
            {
                if (m_bLineBridging != value && OnPropertyChanging(DPN.LineBridgingEnabled, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineBridgingEnabled);

                    // assign new value
                    m_bLineBridging = value;

                    this.BridgeManager.BeginUpdateIntersection();

                    // Update bridges.
                    foreach (Node node in this.Nodes)
                    {
                        UpdateBridges(node);
                    }

                    this.BridgeManager.EndUpdateIntersection();

                    // raise property changed event
                    OnPropertyChanged(DPN.LineBridgingEnabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of bridges added to the line to jump over
        /// other lines.
        /// </summary>
        [Browsable(true)]
        [Category("Line Routing")]
        [DefaultValue(16f)]
        [Description("Determines the size of bridges added to lines to jump over other lines.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float LineBridgeSize
        {
            get 
            { 
                return m_fLineBridgeSize; 
            }
            set
            {
                if (m_fLineBridgeSize != value && OnPropertyChanging(DPN.LineBridgeSize, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineBridgeSize);

                    // assign new value
                    m_fLineBridgeSize = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.LineBridgeSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets the line router manager.
        /// </summary>
        /// <value>The line router.</value>
        [Browsable(true)]
        [Description("Line Router used to route model's connectors.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LineRouter LineRouter
        {
            get 
            { 
                return m_lineRouter; 
            }
            set
            {
                m_lineRouter = value;
                if (m_lineRouter != null)
                    m_lineRouter.Model = this;
            }
        }

        /// <summary>
        /// Gets or sets minimum model auto size for content.
        /// </summary>
        [Browsable(true)]
        [Description("Defines minimum model size with SizeToContent flag set.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(MeasureSizeConverter))]
        public SizeF MinimumSize
        {
            get 
            { 
                return m_modelsMinSize; 
            }
            set
            {
                if (m_modelsMinSize != value && OnPropertyChanging(DPN.MinimumSize, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.MinimumSize);

                    // assign new value
                    m_modelsMinSize = value;

                    // raise property changed event
                    OnPropertyChanged(string.Empty, DPN.MinimumSize);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether autoresizing model size 
        /// when nodes go out of bounds on model.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether Model will be sized to its content, but not less than MinimumSize.")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SizeToContent
        {
            get 
            { 
                return m_bSizeToContent; 
            }
            set
            {
                if (value != m_bSizeToContent && OnPropertyChanging(DPN.SizeToContent, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.SizeToContent);

                    // assign new value
                    m_bSizeToContent = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.SizeToContent);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Background Image Layout for this model.
        /// </summary>
        [Browsable(true)]
        [Description("The Background image layout used for the component.")]
        [DefaultValue(ImageLayout.Tile)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ImageLayout BackgroundImageLayout
        {
            get 
            { 
                return m_BackgroundImageLayout; 
            }
            set
            {
                if (m_BackgroundImageLayout != value && OnPropertyChanging(this.FullContainerName, DPN.BackgroundImageLayout, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.BackgroundImageLayout);

                    // set new value
                    m_BackgroundImageLayout = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.BackgroundImageLayout);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background image for this model.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The Background image used for the component.")]
        [DefaultValue(null)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image BackgroundImage
        {
            get 
            { 
                return m_BackgroundImage; 
            }
            set
            {
                if (m_BackgroundImage != value && OnPropertyChanging(DPN.BackgroundImage, value))
                {
                    // make history entry
                    RecordPropertyChanged(DPN.BackgroundImage);

                    // set new value
                    m_BackgroundImage = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.BackgroundImage);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the model redrawing is enabled.
        /// <see cref="Model.BeginUpdate"/>
        /// <seealso cref="Model.EndUpdate"/>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InUpdate
        {
            get { return m_bInUpdate; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether boundary constraints are enabled or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is True by default. If this property is True, then all nodes
        /// in the model are constrained to the bounds of the model. In other words,
        /// child nodes cannot move, resize, or rotate to a position that leaves the
        /// bounds of the model.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Controls whether the movement of objects is constrained to the bounds of the model.")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BoundaryConstraintsEnabled
        {
            get 
            { 
                return m_boundaryConstraintsEnabled; 
            }
            set
            {
                if (m_boundaryConstraintsEnabled != value && OnPropertyChanging(DPN.BoundaryConstraintsEnabled, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.BoundaryConstraintsEnabled);
                    //// assign new value
                    m_boundaryConstraintsEnabled = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.BoundaryConstraintsEnabled);
                }
            }
        }
        #endregion

        #region Logical Units
        /// <summary>
        /// Gets or sets unit of measure used for world coordinates.
        /// </summary>
        [Browsable(true)]
        [Category("Logical Units")]
        [Description("Logical unit of measurement.")]
        [DefaultValue(MeasureUnits.Pixel)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public MeasureUnits MeasurementUnits
        {
            get 
            { 
                return m_unitMeasure; 
            }
            set
            {
                if (m_unitMeasure != value && OnPropertyChanging(DPN.MeasurementUnits, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.MeasurementUnits);
                    MeasureUnits unitPrev = m_unitMeasure;

                    // assign new value
                    m_unitMeasure = value;

                    // Convert values contained by the model and all children from the old unit of measure to the new unit of measure.
                    ConvertLogicalUnits(unitPrev, value);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.MeasurementUnits);
                    if (value != MeasureUnits.Pixel)
                        this.LogicalSize = new SizeF(this.m_pageSize.PixelWidth, this.m_pageSize.PixelHeight);    

                }
            }
        }

        /// <summary>
        /// Gets or sets the document scale.
        /// </summary>
        /// <value>The document scale.</value>
        [Browsable(true)]
        [Category("Logical Units")]
        [TypeConverter(typeof(PageScaleConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the document scale.")]
        public PageScale DocumentScale
        {
            get
            {
                if (m_pageScale == null)
                {
                    m_pageScale = new PageScale();
                    m_pageScale.UpdateServiceReferences(this);
                }

                return m_pageScale;
            }
            set
            {
                if (m_pageScale != value && OnPropertyChanging(string.Empty, DPN.DocumentScale, value))
                {
                    if (m_pageScale != null)
                        m_pageScale.UpdateServiceReferences(null);

                    // assign new value
                    this.BeginUpdate();

                    m_pageScale = value;
                    m_pageScale.UpdateServiceReferences(this);

                    // make history record
                    RecordPropertyChanged(DPN.DocumentScale);

                    OnPropertyChanged(string.Empty, DPN.DocumentScale);

                    this.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the document.
        /// </summary>
        /// <value>The size of the document.</value>
        [Browsable(true)]
        [Category("Logical Units")]
        [TypeConverter(typeof(PageSizeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the size of the document.")]
        public PageSize DocumentSize
        {
            get
            {
                if (m_pageSize == null)
                {
                    // create deafult A4 pagesize
                    SizeF szDefaultSize = this.DefaultDocumentSize;
                    MeasureUnits units = this.MeasurementUnits;
                    m_pageSize = new PageSize(szDefaultSize.Width, units, szDefaultSize.Height, units);
                    m_pageSize.UpdateServiceReferences(this);
                }

                return m_pageSize;
            }
            set
            {
                SizeF szOffset = SizeF.Empty;

                if (this.DocumentSize != null && value != null)
                {
                    szOffset.Width = m_pageSize.PixelWidth - value.PixelWidth;
                    szOffset.Height = m_pageSize.PixelHeight - value.PixelHeight;
                }

                if (m_pageSize != value && !szOffset.IsEmpty && OnPropertyChanging(string.Empty, DPN.DocumentSize, value))
                {
                    if (m_pageSize != null)
                        m_pageSize.UpdateServiceReferences(null);

                    // make history record
                    RecordPropertyChanged(DPN.DocumentSize);

                    // assign new value
                    m_pageSize = value;
                    m_pageSize.UpdateServiceReferences(this);

                    // raise size changed event
                    OnSizeChanged(szOffset.Width, szOffset.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the logical model size in current measurement units.
        /// </summary>
        /// <value>The logical size in current measurement units.</value>
        [Browsable(true)]
        [Description("Model's size specified in current measure units.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(MeasureSizeConverter))]
        public SizeF LogicalSize
        {
            get
            {
                float fScale = this.DocumentScale.GetScaleFactor(this.MeasurementUnits);
                SizeF szSize = this.DocumentSize.GetSize(this.MeasurementUnits);

                szSize.Width /= fScale;
                szSize.Height /= fScale;

                return szSize;
            }
            set
            {
                float fScale = this.DocumentScale.GetScaleFactor(this.MeasurementUnits);
                SizeF szSize = this.DocumentSize.GetSize(this.MeasurementUnits);
                float fWidth = value.Width * fScale;
                float fHeight = value.Height * fScale;

                if (szSize.Width != fWidth || szSize.Height != fHeight)
                {
                    PageSize pageSize = new PageSize(this.DocumentSize);
                    pageSize.SetSize(fWidth, fHeight, this.MeasurementUnits);

                    // set new calue
                    this.DocumentSize = pageSize;
                }
            }
        }
        #endregion

        #region Styles
        /// <summary>
        /// Gets the diagram rendering style.
        /// </summary>
        /// <remarks>
        /// The rendering style is used to configure graphics.
        /// </remarks>
        /// <value>The rendering style.</value>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Document Rendering style.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RenderingStyle RenderingStyle
        {
            get
            {
                if (m_styleRendering == null)
                {
                    m_styleRendering = new RenderingStyle();
                    m_styleRendering.UpdateServiceReferences(this);
                }

                return m_styleRendering;
            }
        }

        /// <summary>
        /// Gets or sets properties used to fill the interior of document.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The fill style is used to create brushes for painting interior of document.
        /// </para>
        /// <para>
        /// The fill style properties in the model are inherited by child nodes. If a
        /// child node does not have a value assigned to a given fill style property,
        /// then the value assigned to the model is used.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.FillStyle"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model.LineStyle"/>
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Fill style for filling document interior.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FillStyle BackgroundStyle
        {
            get
            {
                if (m_styleFill == null)
                {
                    m_styleFill = new FillStyle();

                    m_styleFill.Color = Color.White;
                    m_styleFill.UpdateServiceReferences(this);
                }

                return m_styleFill;
            }
            set
            {
                if (value != m_styleFill)
                    m_styleFill = value;
            }
        }

        /// <summary>
        /// Gets properties used for document border.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The line style is used to create pen for document border.
        /// </para>
        /// <para>
        /// The line style properties in the model are inherited by child nodes. If a
        /// child node does not have a value assigned to a given line style property,
        /// then the value assigned to the model is used.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Line style for document outline.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineStyle LineStyle
        {
            get
            {
                if (m_styleLine == null)
                {
                    m_styleLine = new LineStyle();
                    m_styleLine.UpdateServiceReferences(this);
                }

                return m_styleLine;
            }
        }

        /// <summary>
        /// Gets or sets properties used to draw document shadow.
        /// </summary>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ShadowStyle"/>
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Properties for document shadow.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowStyle ShadowStyle
        {
            get
            {
                if (m_styleShadow == null)
                {
                    m_styleShadow = new ShadowStyle();
                    m_styleShadow.UpdateServiceReferences(this);
                }

                return m_styleShadow;
            }
            set
            {
                if (value != m_styleShadow)
                    m_styleShadow = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets the children to model children collection.
        /// </summary>
        /// <value>The children.</value>
        protected NodeCollection Children
        {
            get
            {
                if (m_nodesChildren == null)
                {
                    m_nodesChildren = new NodeCollection(this);
                    m_nodesChildren.UpdateServiceReferences(this);
                    this.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Children_ChangeComplete);
                }

                return m_nodesChildren;
            }
        }

        /// <summary>
        /// Gets names hash where key - node name, value - available node index.
        /// Using for generate unique name with indexing.
        /// </summary>
        protected Hashtable NameTable
        {
            get
            {
                if (m_tableName == null)
                    m_tableName = new Hashtable();

                return m_tableName;
            }
        }
        #endregion

        #region Class utility methods

        #region rendering
        /// <summary>
        /// Renders the model onto the given System.Drawing.Graphics object.
        /// </summary>
        /// <param name="grfx">Graphics context object to render onto.</param>
        /// <remarks>
        /// <para>
        /// The model initializes the given System.Drawing.Graphics object with
        /// the its:
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.MeasurementUnits"/>, and
        /// Then it iterates through each layer in its
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.Layers"/>
        /// collections and draws each one.
        /// </para>
        /// </remarks>
        public virtual void Draw(Graphics grfx)
        {
            foreach (Node node in this.Children)
            {
                node.Draw(grfx);
            }
        }

        /// <summary>
        /// Checks the to cache group bitmap.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="hash">The hash.</param>
        public void CheckUseBitmap(ICompositeNode group, Hashtable hash)
        {
            for (int i = 0; i < group.ChildCount; i++)
            {
                Node node = group.GetChild(i);
                if (node is ICompositeNode)
                    if ((node as ICompositeNode).ChildCount > 0)
                        this.CheckUseBitmap((node as ICompositeNode), hash);
                if (node is RichTextNode)
                {
                    hash.Add(node, (node as RichTextNode).UseBitmap);
                    (node as RichTextNode).UseBitmap = true;
                }
            }
        }

        /// <summary>
        /// Uncheck the use bitmap cache.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="hash">The hashtable with flags.</param>
        public void UnCheckUseBitmap(ICompositeNode group, Hashtable hash)
        {
            for (int i = 0; i < group.ChildCount; i++)
            {
                Node node = group.GetChild(i);
                if (node is ICompositeNode)
                    if ((node as ICompositeNode).ChildCount > 0)
                        this.UnCheckUseBitmap((node as ICompositeNode), hash);
                if (node is RichTextNode)
                {
                    (node as RichTextNode).UseBitmap = (bool)hash[node];
                    hash.Remove(node);
                }
            }
        }

        /// <summary>
        /// Renders the specified area of the model to a graphics context.
        /// </summary>
        /// <param name="grfx">Graphics context to render onto.</param>
        /// <param name="rcArea">Area to render.</param>
        /// <remarks>
        /// Only objects that intersect the specified rectangular area are rendered.
        /// </remarks>
        public virtual void Draw(Graphics grfx, RectangleF rcArea)
        {
            RectangleF rectNodeBoundingRectangle;

            foreach (Node node in this.Children)
            {
                rectNodeBoundingRectangle = node.RefreshRect;

                if (rcArea.IntersectsWith(rectNodeBoundingRectangle))
                {
                    node.Draw(grfx);
                }
            }
        }
        #endregion

        /// <summary>
        /// Disables any redrawing of the Model.
        /// <see cref="Model.EndUpdate"/>
        /// <seealso cref="Model.InUpdate"/>
        /// </summary>
        public void BeginUpdate()
        {
            if (!m_bInUpdate)
            {
                m_bInUpdate = true;
                this.LinkManager.BeginSynchronization();
                this.BridgeManager.BeginUpdateIntersection();
                this.EventSink.RaiseDocumentBeginUpdate();
            }
            m_iUpdateRequests++;
        }

        /// <summary>
        /// Enables the redrawing of the Model.
        /// <see cref="Model.BeginUpdate"/>
        /// <seealso cref="Model.InUpdate"/>
        /// </summary>
        public void EndUpdate()
        {
            m_iUpdateRequests--;
            if (m_iUpdateRequests == 0)
            {
                m_bInUpdate = false;
                this.LinkManager.EndSynchronization();
                this.BridgeManager.EndUpdateIntersection();
                this.EventSink.RaiseDocumentEndUpdate();
            }
        }

        /// <summary>
        /// Tests to see if the given node falls within the constraining region
        /// of the composite node.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>
        /// True if node falls within the constraining region; False if it does
        /// not.
        /// </returns>
        public bool CheckBoundaryConstrains(Node node)
        {
            bool bSuccess = true;
            RectangleF rectModelBounds;
            RectangleF rectNodeBoundingRectangle;
            if (m_boundaryConstraintsEnabled)
            {
                rectModelBounds = new RectangleF(
                    new PointF(0, 0), 
                    MeasureUnitsConverter.Convert(this.LogicalSize, this.MeasurementUnits, MeasureUnits.Pixel));

                System.Drawing.Drawing2D.Matrix mtxScale = this.DocumentScale.GetScaleTransformation(this.MeasurementUnits);
                mtxScale.Invert();
                rectModelBounds = Geometry.AppendMatrix(rectModelBounds, mtxScale);
                rectNodeBoundingRectangle = MeasureUnitsConverter.Convert(node.BoundingRectangle, node.MeasurementUnit, MeasureUnits.Pixel);
                rectNodeBoundingRectangle = new RectangleF(
                    (float)Math.Ceiling(rectNodeBoundingRectangle.X), 
                    (float)Math.Ceiling(rectNodeBoundingRectangle.Y),
                    (float)Math.Ceiling(rectNodeBoundingRectangle.Width), 
                    (float)Math.Ceiling(rectNodeBoundingRectangle.Height));

                bSuccess = rectModelBounds.Contains(rectNodeBoundingRectangle);
            }

            return bSuccess;
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("documentMinSize", m_modelsMinSize);
            info.AddValue("documentSize", this.LogicalSize);
            info.AddValue("sizeToContent", m_bSizeToContent);
            info.AddValue("boundaryConstrains", m_boundaryConstraintsEnabled);
            info.AddValue("name", m_strName);
            info.AddValue("children", this.Children);
            info.AddValue("layers", this.Layers);
            info.AddValue("activeLayers", this.ActiveLayers);
            info.AddValue("fillStyle", this.BackgroundStyle);
            info.AddValue("lineStyle", this.LineStyle);
            info.AddValue("shadowStyle", this.ShadowStyle);
            info.AddValue("renderingStyle", this.RenderingStyle);
            info.AddValue("measureUnit", m_unitMeasure);
            info.AddValue("headerFooter", this.HeaderFooterData);
            info.AddValue("lineRouting", m_bLineRouting);
            info.AddValue("lineBridging", m_bLineBridging);
            info.AddValue("lineBridgeSize", m_fLineBridgeSize);
            info.AddValue("backgroundImage", this.BackgroundImage);
            info.AddValue("backgroundImageLayout", this.BackgroundImageLayout);
            info.AddValue("documentScale", this.m_pageScale);

            // if line routing enable -> save line router manager
            if (this.LineRoutingEnabled)
            {
                info.AddValue("lineRouter", m_lineRouter);
            }
        }

        /// <summary>
        /// Creates a <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterData"/> instance for the model. 
        /// </summary>
        /// <returns>The HeaderFooterData object.</returns>
        protected virtual HeaderFooterData CreateHeaderFooter()
        {
            return new HeaderFooterData();
        }

        /// <summary>
        /// Called when deserialization is complete.
        /// </summary>
        /// <param name="sender">Object performing the deserialization.</param>
        protected virtual void OnDeserialization(object sender)
        {
            m_nodesChildren.Owner = this;
            m_nodesChildren.UpdateServiceReferences(this);

            string strRegex = @"([0-9]*$)";
            Regex regex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (this.LineRouter != null)
                this.LineRouter.Pause();
            this.BeginUpdate();

            // update children service references
            foreach (Node curChild in m_nodesChildren)
            {
                curChild.Parent = this;
                curChild.UpdateServiceReferences(this);

                HandlesHitTesting.RegisterNode(this.NameTable, curChild, regex);
            }

            // update collections serveice references
            LayerCollection layers = new LayerCollection(this);
            layers.UpdateServiceReferences(this);

            // update children service references
            foreach (Layer layer in this.Layers)
            {
                layers.Add(layer);
            }

            // swap new collection
            m_layers = layers;

            //this.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Children_ChangeComplete);
            this.EndUpdate();
            if (this.LineRouter != null)
                this.LineRouter.Resume();
        }
        #endregion

        #region Class helper methods

        #region designer serialization helpers
        /// <summary>
        /// Should serialize the size of the model.
        /// </summary>
        /// <returns>true, if serialize size.</returns>
        protected bool ShouldSerializeSize()
        {
            return (this.DefaultDocumentSize != this.LogicalSize);
        }

        /// <summary>
        /// Should serialize the minimum size of the model.
        /// </summary>
        /// <returns>true if serialize minimum size.</returns>
        protected bool ShouldSerializeMinimumSize()
        {
            return (this.DefaultMinimumSize != this.MinimumSize);
        }
        #endregion

        /// <summary>
        /// Called when Z order changing.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The new Z order value.</param>
        /// <returns>true, if Z order changing.</returns>
        protected virtual bool OnZOrderChanging(Node node, ZOrderUpdate changeType, int nNewZOrder)
        {
            ZOrderChangingEventArgs evtArgs = new ZOrderChangingEventArgs(node, changeType, nNewZOrder);

            if (this.EventSink != null)
            {
                this.EventSink.RaiseZOrderChanging(evtArgs);
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Called when Z order changed.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The new Z order value.</param>
        protected virtual void OnZOrderChanged(Node node, ZOrderUpdate changeType, int nNewZOrder)
        {
            ZOrderChangedEventArgs evtArgs = new ZOrderChangedEventArgs(node, changeType, nNewZOrder);

            if (this.EventSink != null)
            {
                this.EventSink.RaiseZOrderChanged(evtArgs);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns the specified type of service object the caller.
        /// </summary>
        /// <param name="svcType">Type of service requested.</param>
        /// <returns>
        /// The object matching the service type requested or NULL if the
        /// service is not supported.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is similar to COM's IUnknown::QueryInterface method,
        /// although more generic. Instead of just returning interfaces,
        /// this method can return any type of object.
        /// </para>
        /// <para>
        /// The following services are supported:
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents"/>,
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>,
        /// <see cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>,
        /// <see cref="Syncfusion.Windows.Forms.Diagram.FillStyle"/>,
        /// <see cref="Syncfusion.Windows.Forms.Diagram.BackgroundStyle"/>
        /// </para>
        /// </remarks>
        protected override object GetService(Type svcType)
        {
            if (svcType == typeof(IDispatchNodeEvents))
            {
                return this;
            }
            else if (svcType == typeof(IZOrderContainer))
            {
                return this;
            }
            else if (svcType == typeof(LineStyle))
            {
                return m_styleLine;
            }
            else if (svcType == typeof(FillStyle))
            {
                return m_styleFill;
            }
            else if (svcType == typeof(ShadowStyle))
            {
                return m_styleShadow;
            }
            else if (svcType == typeof(HeaderFooterData))
            {
                return m_headerFooter;
            }
            else if (svcType == typeof(HistoryManager))
            {
                return this.HistoryManager;
            }
            else if (svcType == typeof(EventSink) || svcType == typeof(DocumentEventSink))
            {
                return this.EventSink;
            }
            else if (svcType == typeof(BridgeManager))
            {
                return this.BridgeManager;
            }
            else if (svcType == typeof(LinkManager))
            {
                return this.LinkManager;
            }
            else if (svcType == typeof(IPropertyObserver))
            {
                return this;
            }
            else if (svcType == typeof(IPropertyContainer))
            {
                return this;
            }

            return base.GetService(svcType);
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public virtual object Clone()
        {
            return new Model(this);
        }
        #endregion

        #region ISerializable
        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);

            m_bModified = false;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns>The object.</returns>
        public object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            return GetService(Type.GetTypeFromHandle(typeHandle));
        }
        #endregion

        #region IServiceProvider interface
        /// <summary>
        /// Returns the specified type of service object to the caller.
        /// </summary>
        /// <param name="svcType">Type of service requested.</param>
        /// <returns>
        /// The object matching the service type requested or NULL if the
        /// service is not supported.
        /// </returns>
        object IServiceProvider.GetService(Type svcType)
        {
            return GetService(svcType);
        }
        #endregion

        #region IPropertyObserver Members
        /// <summary>
        /// Called when model property changing.
        /// </summary>
        /// <param name="strPropertyFullPath">The property full path.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <returns>true, if property changing.</returns>
        public bool OnPropertyChanging(string strPropertyFullPath, string strPropertyName, object oldValue)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyFullPath != string.Empty)
                {
                    strPropName = strPropertyFullPath + "." + strPropertyName;
                }

                bSuccess = this.EventSink.RaisePropertyChangingEvent(this, strPropName, oldValue);
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when model property changed.
        /// </summary>
        /// <param name="strPropertyFullPath">The property full path.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public void OnPropertyChanged(string strPropertyFullPath, string strPropertyName)
        {
            if (this.EventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyFullPath != string.Empty)
                {
                    strPropName = strPropertyFullPath + "." + strPropertyName;
                }

                this.EventSink.RaisePropertyChangedEvent(this, strPropName);
            }
        }

        /// <summary>
        /// Called when model size changed.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        public void OnSizeChanged(float fOffsetX, float fOffsetY)
        {
            if (m_eventSink != null)
            {
                SizeF szOffset = new SizeF(fOffsetX, fOffsetY);
                m_eventSink.RaiseSizeChanged(new SizeChangedEventArgs(this, szOffset));
            }
        }
        #endregion

        #region IPropertyContainer Members
        /// <summary>
        /// Gets the full name of the container.
        /// </summary>
        /// <value>The full name of the container.</value>
        [Browsable(false)]
        public string FullContainerName
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the name of the property container by.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>The container object</returns>
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
                    strSubContainerName = strPropertyContainerName.Substring(0, nSeparatorIndex + 1);
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
                case "FillStyle":
                    objPropertyContainer = this.BackgroundStyle;
                    break;
                case "LineStyle":
                    objPropertyContainer = this.LineStyle;
                    break;
                case "ShadowStyle":
                    objPropertyContainer = this.ShadowStyle;
                    break;
                case "RenderingStyle":
                    objPropertyContainer = this.RenderingStyle;
                    break;
            }

            return objPropertyContainer;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Called when property changing.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property is changing.</returns>
        protected virtual bool OnPropertyChanging(string strPropertyName, object newValue)
        {
            return OnPropertyChanging(this.FullContainerName, strPropertyName, newValue);
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string strPropertyName)
        {
            OnPropertyChanged(this.FullContainerName, strPropertyName);
        }

        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected void RecordPropertyChanged(string strPropertyName)
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordPropertyChanged(this, string.Empty, strPropertyName);
            }
        }

        /// <summary>
        /// Records the Z order changed.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="changeType">Type of the change.</param>
        protected void RecordZOrderChanged(Node node, ZOrderUpdate changeType)
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordZorderChanged(node, changeType);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Removes all nodes from the model and resets its state values.
        /// </summary>
        public void Clear()
        {
            this.RemoveAllChildren();
            m_bModified = false;
        }

        /// <summary>
        /// Compare the state of a line node with the last known state for the same object and return true if
        /// the state has changed
        /// </summary>
        /// <param name="lineNode">The line node.</param>
        /// <returns>true, if line node changed.</returns>
        private bool CheckLineNodeChanged(ConnectorBase lineNode)
        {
            if (OptimizeLineBridging)
            {
                if (m_lastLineNodeStates.ContainsKey(lineNode))
                {
                    ArrayList oldLineSegments = m_lastLineNodeStates[lineNode] as ArrayList;
                    m_lastLineNodeStates[lineNode] = lineNode.LineSegments.Clone();
                    return !CompareLineSegments(oldLineSegments, lineNode.LineSegments);
                }
                else
                    m_lastLineNodeStates.Add(lineNode, lineNode.LineSegments.Clone());
            }
            return true;
        }

        private bool CompareLineSegments(ArrayList arr1, ArrayList arr2)
        {
            // Check if the two arraysLists have the same length
            if (arr1.Count != arr2.Count)
                return false;

            for (int i = 0; i < arr1.Count; i++)
            {
                if (!arr1[i].Equals(arr2[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compares the Z-order of the two given nodes.
        /// </summary>
        /// <param name="node1">First node to compare</param>
        /// <param name="node2">Second node to compare</param>
        /// <returns>
        /// <para>-1 if the node1 is lower in the Z-order than node2</para>
        /// <para>1 if the node1 is higher in the Z-order than node2</para>
        /// <para>0 if the node1 and node2 are equal in Z-order</para>
        /// </returns>
        /// <remarks>
        /// <para>
        /// The nodes must belong to the same layer in order to compare their Z-order. An
        /// ArgumentException is thrown if the nodes do not belong to the same layer.
        /// </para>
        /// </remarks>
        public int CompareZOrder(INode node1, INode node2)
        {
            if (this.m_layers == null)
            {
                throw new InvalidOperationException("Layers collection is null");
            }

            return 0; // this.this.Layers.CompareZOrder(node1, node2);
        }

        /// <summary>
        /// Gets Model bounding rectangle.
        /// </summary>
        /// <returns>bounding Rectangle</returns>
        public RectangleF GetBoundingRect()
        {
            RectangleF rectBoundingToReturn = new RectangleF(new PointF(0, 0), MeasureUnitsConverter.ToPixels(this.LogicalSize,this.MeasurementUnits));
            RectangleF rectBounding;

            foreach (Node node in this.Children)
            {
                rectBounding = node.BoundingRectangle;
                UpdateBoundaries(rectBounding, ref rectBoundingToReturn);
            }

            return rectBoundingToReturn;
        }

        /// <summary>
        /// Tests to see if the model contains all of the points in the
        /// given array.
        /// </summary>
        /// <param name="pts">Array of points to test.</param>
        /// <returns>
        /// true if all of the points fall within the bounds of the model;
        /// otherwise false
        /// </returns>
        public bool ContainsPoints(PointF[] pts)
        {
            bool bSuccess = false;

            foreach (PointF curPt in pts)
            {
                if (!this.ContainsPoint(curPt))
                {
                    bSuccess = false;
                    break;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Tests to see if the given point lies within the bounds
        /// of the model.
        /// </summary>
        /// <param name="ptTesting">Point to test.</param>
        /// <returns>
        /// true if the model contains the point; otherwise false
        /// </returns>
        public bool ContainsPoint(PointF ptTesting)
        {
            RectangleF rcBounds = new RectangleF(0, 0, this.LogicalSize.Width, this.LogicalSize.Height);
            return rcBounds.Contains(ptTesting);
        }

        /// <summary>
        /// Copy document properties to current model container.
        /// </summary>
        /// <param name="document">The source document.</param>
        /// <param name="bCollectionOnly">merge the collection only, if set to <c>true</c>.</param>
        public void Merge(Model document, bool bCollectionOnly)
        {
            if (!bCollectionOnly)
            {
                m_strName = document.m_strName;
                m_pageSize = document.m_pageSize;

                m_boundaryConstraintsEnabled = document.m_boundaryConstraintsEnabled;
                m_pageScale = document.m_pageScale;

                // set minimum model size
                m_modelsMinSize = document.MinimumSize;
                m_bSizeToContent = document.SizeToContent;

                // styles
                m_styleFill = (FillStyle)document.BackgroundStyle.Clone();
                m_styleLine = (LineStyle)document.LineStyle.Clone();
                m_styleShadow = (ShadowStyle)document.ShadowStyle.Clone();
                m_styleRendering = (RenderingStyle)document.RenderingStyle.Clone();
                m_bridgeStyle = document.m_bridgeStyle;

                m_headerFooter = (HeaderFooterData)document.m_headerFooter.Clone();
                m_unitMeasure = document.m_unitMeasure;
                m_fLineBridgeSize = document.m_fLineBridgeSize;

                this.LineRoutingEnabled = document.m_bLineRouting;
                this.LineBridgingEnabled = document.m_bLineBridging;
            }

            // nodes
            m_nodesChildren = document.m_nodesChildren.Clone() as NodeCollection;
            m_nodesChildren.Container = this;

            // layers
            m_layers = (LayerCollection)document.Layers.Clone();
            m_layers.Container = this;
            m_layersActive = document.ActiveLayers.Clone() as LayerCollection;
            m_layersActive.Container = this;

            m_tableName = (Hashtable)document.NameTable.Clone();
            m_bModified = false;
        }
        #endregion

        #region ICompositeNode interface
        /// <summary>
        /// Gets or sets a value indicating whether composite node can be ungrouped.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(false)]
        public bool CanUngroup
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets name of the node.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Must be unique within the scope of the parent node.
        /// </remarks>
        [Browsable(true)]
        [Description("Name of the document")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string Name
        {
            get 
            { 
                return m_strName; 
            }
            set
            {
                if (m_strName != value && OnPropertyChanging(this.FullContainerName, DPN.Name, value))
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
        /// Gets or sets reference to the composite node this node is a child of.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICompositeNode Parent
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets or sets fully qualified name of the node.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The full name is the name of the node concatenated with the names
        /// of all parent nodes.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FullName
        {
            get { return this.Name; }
            set { this.Name = value; }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public INode Root
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the number of child nodes contained by this model.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ChildCount
        {
            get { return this.Children.Count; }
        }

        /// <summary>
        /// Returns the child node at the given index position.
        /// </summary>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        /// <returns>Child node at the given position or NULL if the index is out of range.</returns>
        public Node GetChild(int childIndex)
        {
            return this.Children[childIndex];
        }

        /// <summary>
        /// Returns the child node matching the given name.
        /// </summary>
        /// <param name="childName">Name of node to return.</param>
        /// <returns>Node matching the given name.</returns>
        public Node GetChildByName(string childName)
        {
            return this.Children[childName] as Node;
        }

        /// <summary>
        /// Returns the child node matching the given full name.
        /// </summary>
        /// <param name="composite">The composite node.</param>
        /// <param name="childFullName">Full name of the child.</param>
        /// <returns>Node matching the given full name.</returns>
        public Node GetChildByFullName(ICompositeNode composite, string childFullName)
        {
            Node nodeToReturn = null;

            for (int i = 0, length = composite.ChildCount; i < length && nodeToReturn == null; i++)
            {
                Node node = composite.GetChild(i);
                ICompositeNode group = node as ICompositeNode;

                if (node.FullName == childFullName)
                {
                    nodeToReturn = node;
                    break;
                }
                else if (group != null)
                {
                    nodeToReturn = GetChildByFullName(group, childFullName);
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Returns the index position of the given child node.
        /// </summary>
        /// <param name="child">Child node to query.</param>
        /// <returns>Zero-based index into the collection of child nodes.</returns>
        public int GetChildIndex(Node child)
        {
            return this.Children.IndexOf(child);
        }

        /// <summary>
        /// Appends the given node to the model.
        /// </summary>
        /// <param name="child">Node to append.</param>
        /// <returns>
        /// Zero-based index at which the node was added to the collection or -1 for failure.
        /// </returns>
        public int AppendChild(Node child)
        {
            int childIdx = -1;

            // 1 - Check to see if this node is already in the model.
            if (!this.Children.Contains(child))
            {
                // 2 - Check Constraining region
                if (CheckBoundaryConstrains(child))
                {
                    // start atomic action
                    this.HistoryManager.StartAtomicAction("Insert Nodes");
                    this.BeginUpdate();

                    // 3 - Add the node to the collection of children.
                    childIdx = this.Children.Add(child);

                    // Make this object as the parent of the new child.
                    child.Parent = this;                    

                    // 4 - if node was successfully added to Model's
                    // NodeCollection add it to ActiveLayer's collection
                    AddToActiveLayers(child);

                    this.LinkManager.SynchronizeNodeConnections(child);

                    // end atomic action
                    this.EndUpdate();
                    this.HistoryManager.EndAtomicAction();
                }
            }

            return childIdx;
        }

        /// <summary>
        /// Appends the given collection of nodes as child nodes.
        /// </summary>
        /// <param name="children">Nodes to append.</param>
        /// <param name="startIdx">
        /// Zero-based index at which the first node was added to the collection of child nodes.
        /// </param>
        /// <returns>Number of child nodes appended.</returns>
        public int AppendChildren(NodeCollection children, out int startIdx)
        {
            startIdx = -1;
            bool bCanInsert = false;

            foreach (Node node in children)
            {
                bCanInsert = CheckBoundaryConstrains(node);

                if (bCanInsert)
                    bCanInsert = !this.Children.Contains(node);

                if (!bCanInsert)
                    break;
            }

            if (bCanInsert)
            {
                this.LinkManager.BeginSynchronization();

                // start atomic action
                this.HistoryManager.StartAtomicAction("Insert Nodes");

                startIdx = this.Children.Count;
                this.Children.AddRange(children);
                if (startIdx < this.Children.Count)
                {
                    foreach (Node node in children)
                    {
                        // Make this object as the parent of the new child.
                        node.Parent = this;
                        
                        this.LinkManager.SynchronizeNodeConnections(node);
                    }

                    // if nodes were successfully added to Model's children NodeCollection
                    // add them to ActiveLayers also
                    AddToActiveLayers(children);
                }
                else
                    startIdx = -1;

                // end atomic action
                this.HistoryManager.EndAtomicAction();
                this.LinkManager.EndSynchronization();
            }

            return (bCanInsert && (startIdx != -1)) ? children.Count : 0;
        }

        /// <summary>
        /// Insert the given node into the model at a specific position.
        /// </summary>
        /// <param name="child">Node to insert.</param>
        /// <param name="childIndex">Zero-based index at which to insert the node.</param>
        public void InsertChild(Node child, int childIndex)
        {
            if (!this.Children.Contains(child))
            {
                if (CheckBoundaryConstrains(child))
                {
                    // start atomic action
                    this.HistoryManager.StartAtomicAction("Insert Nodes");
                    //// 1 - insert node to Model node's collection
                    this.Children.Insert(childIndex, child);
                    
                    //// 2 - add inserted node to ActiveLayer's
                    AddToActiveLayers(child);
                    //// end atomic action
                    this.HistoryManager.EndAtomicAction();
                }
            }
        }

        /// <summary>
        /// Removes the child node at the given position.
        /// </summary>
        /// <param name="childIndex">Zero-based index into the collection of child nodes.</param>
        /// <returns>True if the node was successfully removed; otherwise False.</returns>
        public bool RemoveChild(int childIndex)
        {
            if (childIndex < 0 || childIndex > this.Children.Count - 1)
                throw new ArgumentOutOfRangeException("childIndex", childIndex, Resources.Strings.Messages.Get("ArgumentRange"));

            bool bSuccess = false;
            Node child = this.Children[childIndex];

            if (child != null && EditStyle.CanDelete(child))
            {
                // start atomic action
                this.HistoryManager.StartAtomicAction("Remove Node");
                //// 1 - remove node from layers containing it
                RemoveFromContainingLayers(child);              
                //// 2 - remove from Model node's collection
                this.Children.Remove(child);

                //// end atomic action
                this.HistoryManager.EndAtomicAction();
                //// 3 - set operation success flag
                bSuccess = true;
            }

            if (this.LineBridgingEnabled)
            {
                foreach (Node node in this.Children)
                {
                    UpdateBridges(node);
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Removes the range nodes.
        /// </summary>
        /// <param name="nodesToRemove">The nodes to remove.</param>
        /// <returns>true, if remove the nodes.</returns>
        public bool RemoveRange(NodeCollection nodesToRemove)
        {
            bool bSuccess = false;
            this.BeginUpdate();
            // start atomic action
            this.HistoryManager.StartAtomicAction("Remove Nodes");
            this.Children.Remove(nodesToRemove);
            //// end atomic action
            this.HistoryManager.EndAtomicAction();
            if (this.LineBridgingEnabled)
            {
                foreach (Node node in this.Children)
                {
                    UpdateBridges(node);
                }
            }

            nodesToRemove.Clear();
            this.EndUpdate();
            return bSuccess;
        }

        /// <summary>
        /// Removes specified child node.
        /// </summary>
        /// <param name="nodeToRemove">The node to remove.</param>
        /// <returns>
        /// True if the node was successfully removed; otherwise False.
        /// </returns>
        public bool RemoveChild(Node nodeToRemove)
        {
            if (nodeToRemove == null)
                throw new ArgumentNullException("nodeToRemove");

            bool bSuccess = false;

            if (nodeToRemove != null && EditStyle.CanDelete(nodeToRemove))
            {
                // 1 - remove node from layers containing it
                RemoveFromContainingLayers(nodeToRemove);

                // 2 - if node is remove from model then record port collection changes manually
                RecordConnectionChanges(nodeToRemove);

                //// start atomic action
                this.HistoryManager.StartAtomicAction("Remove Node");

                // 3 - remove from Model node's collection
                // check if node's Parent is Model
                if (nodeToRemove.Parent != this)
                {
                    bSuccess = nodeToRemove.Parent.RemoveChild(nodeToRemove);
                }
                else
                {
                    bSuccess = this.Children.Remove(nodeToRemove);
                }
                //// end atomic action
                this.HistoryManager.EndAtomicAction();
                // 4 - set operation success flag
                bSuccess = true;
            }

            if (this.LineBridgingEnabled)
            {
                foreach (Node node in this.Children)
                {
                    UpdateBridges(node);
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Removes all child nodes from the node.
        /// </summary>
        public void RemoveAllChildren()
        {
            BeginUpdate();
            // start atomic action
            this.HistoryManager.StartAtomicAction("Clear Nodes");
            this.Children.Clear();

            // Clear layers node colelctions
            foreach (Layer curLayer in this.Layers)
            {
                curLayer.RemoveAll();
            }
            //// end atomic action
            this.HistoryManager.EndAtomicAction();
            EndUpdate();
        }

        /// <summary>
        /// Tests to see if the given node falls within the constraining region
        /// of the composite node.
        /// </summary>
        /// <param name="node">Node to test.</param>
        /// <returns>
        /// True if node falls within the constraining region; False if it does
        /// not.
        /// </returns>
        public virtual bool CheckConstrainingRegion(Node node)
        {
            return CheckBoundaryConstrains(node);
        }

        /// <summary>
        /// Returns all children that are intersected by the given point.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children hit by the given point.
        /// </param>
        /// <param name="ptModel">Point to test.</param>
        /// <returns>The number of child nodes that intersect the given point.</returns>
        public virtual int GetChildrenAtPoint(NodeCollection childNodes, PointF ptModel)
        {
            int numFound = 0;

            foreach (Node child in this.Nodes)
            {
                if (child.ContainsPoint(ptModel))
                {
                    childNodes.Add(child);
                    numFound++;
                }
            }

            return numFound;
        }

        /// <summary>
        /// Returns all children that intersect the given rectangle.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children hit by the given point.
        /// </param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>The number of child nodes that intersect the given rectangle.</returns>
        public virtual int GetChildrenIntersecting(NodeCollection childNodes, RectangleF rcModel)
        {
            int numFound = 0;
            RectangleF rcNode;

            foreach (Node child in this.Nodes)
            {
                rcNode = ((IUnitIndependent)child).GetBoundingRectangle(this.MeasurementUnits, true);

                if (rcNode.IntersectsWith(rcModel))
                {
                    childNodes.Add(child);
                    numFound++;
                }
            }

            return numFound;
        }

        /// <summary>
        /// Returns all children inside the given rectangle.
        /// </summary>
        /// <param name="childNodes">
        /// Collection in which to add the children inside the specified rectangle.
        /// </param>
        /// <param name="rcModel">Rectangle to test.</param>
        /// <returns>The number of child nodes added to the collection.</returns>
        public virtual int GetChildrenContainedBy(NodeCollection childNodes, RectangleF rcModel)
        {
            int numFound = 0;
            RectangleF rcNode;

            foreach (Node child in this.Nodes)
            {
                rcNode = ((IUnitIndependent)child).GetBoundingRectangle(this.MeasurementUnits, true);

                if (rcModel.Contains(rcNode))
                {
                    childNodes.Add(child);
                    numFound++;
                }
            }

            return numFound;
        }

        /// <summary>
        /// Update bounds size to content size.
        /// </summary>
        public void UpdateCompositeBounds()
        { 
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Converts the logical values contained by the object from one unit of
        /// measure to another.
        /// </summary>
        /// <param name="fromUnits">Units to convert from.</param>
        /// <param name="toUnits">Units to convert to.</param>
        /// <remarks>
        /// <para>
        /// This method converts all logical unit values contained by the object from
        /// one unit of measure to another.
        /// </para>
        /// </remarks>
        protected void ConvertLogicalUnits(MeasureUnits fromUnits, MeasureUnits toUnits)
        {
            this.BeginUpdate();

            // convert model properties
            m_fLineBridgeSize = MeasureUnitsConverter.ConvertX(m_fLineBridgeSize, fromUnits, toUnits);
            m_modelsMinSize = MeasureUnitsConverter.Convert(m_modelsMinSize, fromUnits, toUnits);

            // update BackgroundStyle units
            if (this.BackgroundStyle.InheritContainerMeasureUnits)
            {
                this.BackgroundStyle.MeasureUnit = toUnits;
            }

            // update LineStyle units
            if (this.LineStyle.InheritContainerMeasureUnits)
            {
                this.LineStyle.MeasureUnit = toUnits;
            }

            // update ShadowStyle units
            if (this.ShadowStyle.InheritContainerMeasureUnits)
            {
                this.ShadowStyle.MeasureUnit = toUnits;
            }

            // Iterate through children and convert them.
            foreach (Node curChild in this.Children)
            {
                if (curChild.InheritContainerMeasureUnits)
                    curChild.MeasurementUnit = toUnits;
            }

            this.EndUpdate();
        }
        #endregion

        #region IZOrderContainer interface
        /// <summary>
        /// Gets number of items in the Z-order for this container.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ZOrderDepth
        {
            get { return this.Children.Count; }
        }

        /// <summary>
        /// Returns the Z-order value of the given node.
        /// </summary>
        /// <param name="node">Node to get Z-order for.</param>
        /// <returns>
        /// Zero-based Z-order value of the node or -1 if the node
        /// does not exist.
        /// </returns>
        public int GetZOrder(Node node)
        {
            return this.Children.IndexOf(node);
        }

        /// <summary>
        /// Sets the Z-order of the given node.
        /// </summary>
        /// <param name="node">Node to set Z-order for.</param>
        /// <param name="zOrder">Zero-based Z-order value.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SetZOrder(Node node, int zOrder)
        {
            // Cache ZDepth
            int nZDepth = this.Children.Count;

            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;

            if (node.Parent != this)
            {
                IZOrderContainer nodeZOC = node.Parent as IZOrderContainer;

                if (nodeZOC != null)
                {
                    nodeZOC.SetZOrder(node, zOrder);
                }
            }
            else
            {
                ZOrderUpdate changeType = ZOrderUpdate.Set;

                if ((nZDepth > zOrder) && (zOrder >= 0) && OnZOrderChanging(node, changeType, zOrder))
                {
                    // ensure thread safety
                    lock (this.Children)
                    {
                        //// make history record
                        RecordZOrderChanged(node, changeType);
                        //// 1 - disable events raising on collection changing
                        bool bCurrentMode = this.Children.QuietMode;
                        this.Children.QuietMode = true;
                        //// 2 - get previous ZOrder value
                        prevZOrder = this.Children.IndexOf(node);
                        //// 3 - set new ZOrder value
                        //// 3a - try to remove node at prevZOrder location
                        this.Children.RemoveAt(prevZOrder);
                        //// 3b - insert node at zOrder location
                        this.Children.Insert(zOrder, node);
                        //// 4 - enable events raising on collection changing
                        this.Children.QuietMode = bCurrentMode;
                    }

                    // Update bridges connector by witch removed node is interecions.
                    if (IsBridgable(node))
                    {
                        UpdateBridges(node as ConnectorBase);
                    }

                    // raise PropertyChanged event
                    OnZOrderChanged(node, changeType, zOrder);
                }
            }

            return prevZOrder;
        }

        /// <summary>
        /// Moves the specified node forward in the Z-order.
        /// </summary>
        /// <param name="node">Node to move forward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int BringForward(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;

            if (node.Parent != this)
            {
                IZOrderContainer nodeZOC = node.Parent as IZOrderContainer;

                if (nodeZOC != null)
                {
                    nodeZOC.BringForward(node);
                }
            }
            else
            {
                int nZDepth = this.Children.Count;
                ZOrderUpdate changeType = ZOrderUpdate.Forward;
                int nNewZOrder = this.Children.IndexOf(node) + 1;

                if (nZDepth > nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
                {
                    // ensure thread safety
                    lock (this.Children)
                    {
                        //// make history record
                        RecordZOrderChanged(node, changeType);
                        //// 1 - disable events raising on collection changing
                        bool bCurrentMode = this.Children.QuietMode;
                        this.Children.QuietMode = true;
                        //// 2 - get previous ZOrder value
                        prevZOrder = this.Children.IndexOf(node);
                        //// 3 - set new ZOrder value
                        //// 3a - try to remove node at prevZOrder location
                        if (prevZOrder < this.Children.Count - 1)
                        {
                            this.Children.RemoveAt(prevZOrder);
                            this.LinkManager.Pause();
                            // 3b - bring given node forward
                            this.Children.Insert(prevZOrder + 1, node);
                            this.LinkManager.Resume();
                        }
                        else
                            prevZOrder = -1;

                        // 4 - enable events raising on collection changing
                        this.Children.QuietMode = bCurrentMode;
                    }

                    // Update bridges connector by witch removed node is interecions.
                    if (IsBridgable(node))
                    {
                        UpdateBridges(node as ConnectorBase);
                    }

                    // raise PropertyChanges event
                    OnZOrderChanged(node, changeType, nNewZOrder);
                }
            }

            return prevZOrder;
        }

        /// <summary>
        /// Sends the specified node back in the Z-order.
        /// </summary>
        /// <param name="node">Node to move backward.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SendBackward(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;

            if (node.Parent != this)
            {
                IZOrderContainer nodeZOC = node.Parent as IZOrderContainer;

                if (nodeZOC != null)
                {
                    nodeZOC.SendBackward(node);
                }
            }
            else
            {
                ZOrderUpdate changeType = ZOrderUpdate.Backward;
                int nNewZOrder = this.Children.IndexOf(node) - 1;

                if (nNewZOrder >= 0 && OnZOrderChanging(node, changeType, nNewZOrder))
                {
                    // ensure thread safety
                    lock (this.Children)
                    {
                        //// make history record
                        RecordZOrderChanged(node, changeType);
                        //// 1 - disable events raising on collection changing
                        bool bCurrentMode = this.Children.QuietMode;
                        this.Children.QuietMode = true;
                        //// 2 - get previous ZOrder value
                        prevZOrder = this.Children.IndexOf(node);
                        //// 2 - set new ZOrder value
                        //// 3a - try to remove node at prevZOrder location
                        if (prevZOrder > 0)
                        {
                            this.Children.RemoveAt(prevZOrder);
                            this.LinkManager.Pause();
                            // 3b - bring given node forward
                            this.Children.Insert(prevZOrder - 1, node);
                            this.LinkManager.Resume();
                        }
                        else
                            prevZOrder = -1;

                        // 4 - enable events raising on collection changing
                        this.Children.QuietMode = bCurrentMode;
                    }

                    // Update bridges connector by witch removed node is interecions.
                    if (IsBridgable(node))
                    {
                        UpdateBridges(node as ConnectorBase);
                    }

                    // raise PropertyChanges event
                    this.OnZOrderChanged(node, changeType, nNewZOrder);
                }
            }

            return prevZOrder;
        }

        /// <summary>
        /// Brings the specified node to the front of the Z-order.
        /// </summary>
        /// <param name="node">Node to bring to the front.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int BringToFront(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;

            if (node.Parent != this)
            {
                IZOrderContainer nodeZOC = node.Parent as IZOrderContainer;

                if (nodeZOC != null)
                {
                    nodeZOC.BringToFront(node);
                }
            }
            else
            {
                // Cache ZDepth value
                int nZDepth = this.Children.Count - 1;
                ZOrderUpdate changeType = ZOrderUpdate.Front;
                int nNewZOrder = nZDepth;

                if (nZDepth >= nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
                {
                    // ensure thread safety
                    lock (this.Children)
                    {
                        //// make history record
                        RecordZOrderChanged(node, changeType);
                        //// 1 - disable events raising on collection changing
                        bool bCurrentMode = this.Children.QuietMode;
                        this.Children.QuietMode = true;
                        //// 2 - get previous ZOrder value
                        prevZOrder = this.Children.IndexOf(node);
                        //// 3 - set new ZOrder value
                        //// 3a - try to remove node at prevZOrder location
                        if (prevZOrder < nZDepth)
                        {
                            this.Children.RemoveAt(prevZOrder);
                            this.LinkManager.Pause();
                            // 3b - bring given node to front
                            this.Children.Insert(nNewZOrder, node);
                            this.LinkManager.Resume();
                        }
                        else
                            prevZOrder = -1;

                        // 4 - enable events raising on collection changing
                        this.Children.QuietMode = bCurrentMode;
                    }

                    // Update bridges connector by witch removed node is interecions.
                    if (IsBridgable(node))
                    {
                        UpdateBridges(node as ConnectorBase);
                    }

                    // raise PropertyChanges event
                    OnZOrderChanged(node, changeType, nNewZOrder);
                }
            }

            return prevZOrder;
        }

        /// <summary>
        /// Sends the specified node to the back of the Z-order.
        /// </summary>
        /// <param name="node">Node to send to the back.</param>
        /// <returns>
        /// Previous Z-order position.
        /// </returns>
        public int SendToBack(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            int prevZOrder = -1;

            this.BeginUpdate();

            if (node.Parent != this)
            {
                IZOrderContainer nodeZOC = node.Parent as IZOrderContainer;

                if (nodeZOC != null)
                {
                    nodeZOC.SendToBack(node);
                }
            }
            else
            {
                // Cache ZDepth value
                int nZDepth = this.Children.Count - 1;
                ZOrderUpdate changeType = ZOrderUpdate.Back;
                int nNewZOrder = nZDepth;

                if (nZDepth >= nNewZOrder && OnZOrderChanging(node, changeType, nNewZOrder))
                {
                    // ensure thread safety
                    lock (this.Children)
                    {
                        //// make history record
                        RecordZOrderChanged(node, changeType);
                        //// 1 - disable events raising on collection changing
                        bool bCurrentMode = this.Children.QuietMode;
                        this.Children.QuietMode = true;
                        //// 2 - get previous ZOrder value
                        prevZOrder = this.Children.IndexOf(node);
                        //// 3 - set new ZOrder value
                        //// 3a - try to remove node at prevZOrder location
                        if (prevZOrder != 0)
                        {
                            this.Children.RemoveAt(prevZOrder);
                            this.LinkManager.Pause();
                            // 3b - send given node to back
                            this.Children.Insert(0, node);
                            this.LinkManager.Resume();
                        }
                        else
                            prevZOrder = -1;

                        // 4 - enable events raising on collection changing
                        this.Children.QuietMode = bCurrentMode;
                    }

                    // Update bridges connector by witch removed node is interecions.
                    if (IsBridgable(node))
                    {
                        UpdateBridges(node as ConnectorBase);
                    }

                    // raise PropertyChanges event
                    OnZOrderChanged(node, changeType, nNewZOrder);
                }
            }

            this.EndUpdate();

            return prevZOrder;
        }
        #endregion

        #region Collection Event Handlers
        /// <summary>
        /// Called when children collection change complete.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [DocumentationExclude()]
        [EventHandlerPriorityAttribute(true)]
        protected void Children_ChangeComplete(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner != this)
                return;

            CollectionExChangeType changeType = evtArgs.ChangeType;
            m_bNodeAdd = true;

            // create regex only once
            string strRegex = @"([0-9]*$)";
            Regex regex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            switch (changeType)
            {
                case CollectionExChangeType.Insert:
                    {
                        Node innerNode = (Node)evtArgs.Element;
                        if (innerNode != null)
                        {
                            if (innerNode.Equals(m_cloneNode))
                            {
                                // Register the name in model name table.
                                HandlesHitTesting.RegisterNode(this.NameTable, m_cloneNode, regex);
                                m_cloneNode = null;
                                m_bNodeAdd = false;                               
                                foreach (Layer layer in innerNode.Layers)
                                {
                                    if (!layer.Contains(innerNode))
                                    {
                                        bool isEnabled = layer.Enabled;
                                        layer.Enabled = true;
                                        layer.Add(innerNode);
                                        layer.Enabled = isEnabled;
                                    }
                                }
                                return;
                            }
                        }

                        foreach (Node curNode in evtArgs.Elements)
                        {
                            GenerateUniqueName(curNode, regex);
                            foreach (Layer layer in curNode.Layers)
                            {
                                if (!layer.Contains(curNode))
                                {
                                    bool isEnabled = layer.Enabled;
                                    layer.Enabled = true;
                                    layer.Add(curNode);
                                    layer.Enabled = isEnabled;
                                }
                            }
                        }

                        // Set the modified flag for the model.
                        m_bModified = true;
                        break;
                    }
                case CollectionExChangeType.Remove:
                    {
                        foreach (Node curNode in evtArgs.Elements)
                        {
                            //disconnect all connections
                            DisconnectAllConnections(curNode);

                            // Unregister the name from model name table.
                            HandlesHitTesting.UnregisterNode(this.NameTable, curNode, regex);
                            foreach (Layer layer in curNode.Layers)
                            {
                                if (layer.Contains(curNode))
                                    layer.Remove(curNode);
                            }
                            // Reset parent.
                            curNode.Parent = null;
                        }

                        // Set the modified flag for the model.
                        m_bModified = true;
                        break;
                    }
                case CollectionExChangeType.Clear:
                    {
                        m_bModified = true;
                        this.NameTable.Clear();
                        break;
                    }
                case CollectionExChangeType.Set:
                    foreach (Node curNode in evtArgs.Elements)
                    {
                        GenerateUniqueName(curNode, regex);
                        // Make this object as the parent of the new child.
                        curNode.Parent = this;
                    }
                    // Update the modified flag for the model.
                    m_bModified = true;
                    break;
            }

            m_bNodeAdd = false;
        }

        /// <summary>
        /// Called when children property is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        [DocumentationExclude()]
        [EventHandlerPriorityAttribute(true)]
        protected void Children_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.Name)
            {
                Node node = evtArgs.NodeAffected as Node;

                if (node != null && !m_bNodeAdd)
                {
                    m_bNodeAdd = true;

                    HandlesHitTesting.RegisterNode(this.NameTable, node, HandlesHitTesting.NameIndex);

                    m_bNodeAdd = false;
                }
            }

            m_bModified = true;
        }

        /// <summary>
        /// Called when children property is changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        [DocumentationExclude()]
        [EventHandlerPriorityAttribute(true)]
        protected void Children_PropertyChanging(PropertyChangingEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.Name)
            {
                Node node = evtArgs.PropertyContainer as Node;
                Layer layer = evtArgs.PropertyContainer as Layer;

                if (node != null && !m_bNodeAdd)
                {
                    m_bNodeAdd = true;

                    HandlesHitTesting.UnregisterNode(this.NameTable, node, HandlesHitTesting.NameIndex);

                    m_bNodeAdd = false;
                }
                else if (layer != null && !m_bNodeAdd)
                {
                    m_bNodeAdd = true;
                    string name = evtArgs.NewValue.ToString();

                    if (this.Layers[name] == null && this.Layers.GenerateUniqueName(layer, ref name))
                        layer.Name = name;

                    evtArgs.Cancel = true;
                    m_bNodeAdd = false;
                }
            }
        }
        #endregion

        #region IDeserializationCallback
        /// <summary>
        /// Called when deserialization is complete.
        /// </summary>
        /// <param name="sender">Object performing the deserialization.</param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            OnDeserialization(sender);
        }
        #endregion

        #region ISupportInitialize interface
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        [DocumentationExclude()]
        public void BeginInit()
        { 
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        [DocumentationExclude()]
        public void EndInit()
        {
            m_bModified = false;
        }

        #endregion

        #region Implementation Methods
        /// <summary>
        /// Generate the unique name for node.
        /// </summary>
        /// <param name="curNode">The current node.</param>
        /// <param name="regex">The regular expression.</param>
        private void GenerateUniqueName(Node curNode, Regex regex)
        {
            if (curNode.Parent != this)
                return;

            // Generate unique name for node.
            string nodeName;

            if (GenerateUniqueNodeName(curNode, out nodeName, regex))
            {
                curNode.Name = nodeName;
            }

            m_cloneNode = curNode;

            // Register the name in model name table.
            HandlesHitTesting.RegisterNode(this.NameTable, curNode, regex);
        }

        /// <summary>
        /// Called to generate a unique name when inserting a new node.
        /// </summary>
        /// <param name="obj">Node to generate unique name for.</param>
        /// <param name="nodeName">Node name generated.</param>
        /// <param name="regex">Regex used to split node name.</param>
        /// <returns>
        /// True if a new name was generated; False if the name is already unique.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The implementation of this method first checks to see if the name is
        /// already unique. If it is, then it returns False to the caller and
        /// the nodeName parameter contains the original name. If the node's name
        /// is not unique within the model, then this method adds a numeric suffix
        /// to the name and continues to increment it in a loop until the name
        /// is unique. If the nodeName output parameter contains a value other
        /// than the original node name, then this method returns True.
        /// </para>
        /// <para>
        /// This method can be overriden in derived classes in order to customize or
        /// replace the algorithm for generating unique names.
        /// </para>
        /// </remarks>
        protected virtual bool GenerateUniqueNodeName(Node obj, out string nodeName, Regex regex)
        {
            return HandlesHitTesting.GenerateUniqueNodeName(this.NameTable, obj, out nodeName, regex);
        }
        #endregion

        #region Class helper methods
       
        private void DisconnectAllConnections(Node child)
        {
            foreach (ConnectionPoint port in child.Ports)
                port.DisconnectAll();

            ICompositeNode container = child as ICompositeNode;

            if (container != null)
            {
                for (int i = 0, nLength = container.ChildCount; i < nLength; i++)
                    DisconnectAllConnections(container.GetChild(i));
            }

            if (child is IEndPointContainer)
            {
                IEndPointContainer endPointContainer = (IEndPointContainer)child;

                // HeadEndPoint
                EndPoint endPoint = endPointContainer.HeadEndPoint;
                ConnectionPoint port = endPoint.Port;

                if (port != null)
                {
                    port.Disconnect(endPoint);
                }

                // TailEndPoint
                endPoint = endPointContainer.TailEndPoint;
                port = endPoint.Port;

                if (port != null)
                {
                    port.Disconnect(endPoint);
                }
            }
        }

        /// <summary>
        /// Update bridges given node connectors.
        /// </summary>
        /// <param name="node">The node.</param>
        private void UpdateBridges(Node node)
        {
            ConnectorBase lineNode = node as ConnectorBase;
            ICompositeNode parent = node as ICompositeNode;

            // Update bridges for current line connector.
            if (lineNode != null)
            {
                if (lineNode.LineBridgingEnabled && CheckLineNodeChanged(lineNode))
                    this.BridgeManager.AddToIntersectCollection(lineNode);
            }
            else if (parent != null)
            {
                for (int i = 0, length = parent.ChildCount; i < length; i++)
                {
                    UpdateBridges(parent.GetChild(i));
                }
            }
        }

        /// <summary>
        /// Removes from containing layers.
        /// </summary>
        /// <param name="nodeRemoving">The node removing.</param>
        protected void RemoveFromContainingLayers(Node nodeRemoving)
        {
            if (nodeRemoving == null)
                throw new NullReferenceException("nodeRemoving");

            // 1 - cast to INodeEx interface containing reference
            // to node's containing layers
            Layer layerTemp;
            LayerCollection layersContaining = nodeRemoving.Layers;

            // 2 - iterate through containing layers 
            // removing given node from each of them
            for (int nCounter = 0, nLength = layersContaining.Count; nCounter < nLength; nCounter++)
            {
                layerTemp = layersContaining[nCounter];

                // remove node
                if (layerTemp.Contains(nodeRemoving))
                    layerTemp.Remove(nodeRemoving);
            }
        }

        /// <summary>
        /// Adds given mode to active layers.
        /// </summary>
        /// <param name="nodeToAdd">The node to add.</param>
        protected void AddToActiveLayers(Node nodeToAdd)
        {
            Layer layerTemp;

            // iterate through ActiveLayer's collection
            // adding added node to each layer
            for (int nCounter = 0, nLength = this.ActiveLayers.Count; nCounter < nLength; nCounter++)
            {
                layerTemp = this.ActiveLayers[nCounter];

                layerTemp.Add(nodeToAdd);

                // nodeToAdd.Layers.Add( layerTemp );
            }
        }

        /// <summary>
        /// Adds given node collection to active layers.
        /// </summary>
        /// <param name="nodesToAdd">The nodes to add.</param>
        protected void AddToActiveLayers(NodeCollection nodesToAdd)
        {
            Layer layerTemp;

            for (int nCounter = 0, nLength = this.ActiveLayers.Count; nCounter < nLength; nCounter++)
            {
                layerTemp = this.ActiveLayers[nCounter];

                // add to active layers
                layerTemp.Add(nodesToAdd);
            }
        }
        private void UpdateBoundaries(RectangleF boundsToTest, ref RectangleF rect)
        {
            if (boundsToTest.X < rect.X)
            {
                rect.Width = rect.Width + (rect.X - boundsToTest.X);
                rect.X = boundsToTest.X;
            }

            if (boundsToTest.Top < rect.Top)
            {
                rect.Height = rect.Height + (rect.Y - boundsToTest.Y);
                rect.Y = boundsToTest.Top;
            }

            if (boundsToTest.Right > rect.Right)
                rect.Width = boundsToTest.Right - rect.X;

            if (boundsToTest.Bottom > rect.Bottom)
                rect.Height = boundsToTest.Bottom - rect.Y;
        }
        private bool IsBridgable(Node node)
        {
            return (node is LineConnector) || (node is OrthogonalConnector);
        }
        private void AddDocumentSinkEvents()
        {
            this.EventSink.PinPointChanged += new PinPointChangedEventHandler(m_eventSink_PinPointChanged);
            this.EventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(m_eventSink_PinOffsetChanged);
            this.EventSink.SizeChanged += new SizeChangedEventHandler(m_eventSink_SizeChanged);
            this.EventSink.RotationChanged += new RotationChangedEventHandler(m_eventSink_RotationChanged);
            this.EventSink.FlipChanged += new FlipChangedEventHandler(m_eventSink_FlipChanged);

            this.EventSink.VertexChanged += new VertexChangedEventHandler(EventSink_VertexChanged);
        }
        private void UpdateNodeScale(NodeCollection nodeCollection, PageScale pageScale)
        {
            this.BeginUpdate();

            foreach (Node node in nodeCollection)
            {
                node.NodeScale = (PageScale)pageScale.Clone();
            }

            this.EndUpdate();
        }

        /// <summary>
        /// Used for manually record connection changes for nodes what already removed from model.
        /// </summary>
        /// <param name="node">Node to check connections changes.</param>
        private void RecordConnectionChanges(Node node)
        {
            ICompositeNode compositeNode = node as ICompositeNode;

            if (compositeNode != null)
            {
                for (int i = 0, nLength = compositeNode.ChildCount; i < nLength; i++)
                {
                    RecordConnectionChanges(compositeNode.GetChild(i));
                }
            }

            foreach (ConnectionPoint port in node.Ports)
            {
                if (port.Connections.Count > 0)
                {
                    this.HistoryManager.RecordCollectionChanged(CollectionExChangeType.Clear, port.Connections, new ArrayList(port.Connections), -1);
                }
            }
        }

        /// <summary>
        /// Sign for children property and collection changes.
        /// </summary>
        /// <remarks>
        /// Calling when model component is initializing.
        /// </remarks>
        private void SubscribeForChildrenEvents()
        {
            this.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Children_ChangeComplete);

            this.EventSink.PropertyChanging += new PropertyChangingEventHandler(Children_PropertyChanging);
            this.EventSink.PropertyChanged += new PropertyChangedEventHandler(Children_PropertyChanged);
        }

        /// <summary>
        /// Used to update the layer collections owner
        /// </summary>
        /// <param name="layerCollection">Layer collection to update.</param>
        private void UpdateLayerCollectionOwner(LayerCollection layerCollection)
        {
            if (layerCollection.Owner == null)
                layerCollection.Owner = this;
            foreach (Layer layer in layerCollection.Members)
            {
                if (layer.Container == null)
                    layer.Container = this;
            }
        }
        #endregion

        #region events handlers
        private void m_eventSink_PinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            OnModelModified();
        }
        private void m_eventSink_PinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            OnModelModified();
        }
        private void m_eventSink_SizeChanged(SizeChangedEventArgs evtArgs)
        {
            OnModelModified();
        }
        private void m_eventSink_RotationChanged(RotationChangedEventArgs evtArgs)
        {
            OnModelModified();
        }
        private void m_eventSink_FlipChanged(FlipChangedEventArgs evtArgs)
        {
            OnModelModified();
        }

        private void EventSink_VertexChanged(VertexChangedEventArgs evtArgs)
        {
            OnModelModified();
        }
        private void OnModelModified()
        {
            m_bModified = true;
        }
        #endregion
    }
}
