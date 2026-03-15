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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Diagram.Base.Wizard;
using Syncfusion.Runtime.InteropServices.WinAPI;
using Syncfusion.Windows.Forms.Diagram;
using Syncfusion.Windows.Forms.Diagram.Wizards;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Diagram designer.
    /// </summary>
    public class DiagramDesigner :
        ControlDesigner
    {
        private DesignerActionListCollection listCollection;

        /// <summary>
        /// Called when the designer is initialized.
        /// </summary>
        [Obsolete]
        public override void OnSetComponentDefaults()
        {
            base.OnSetComponentDefaults();
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            Model m = (Model)host.CreateComponent(typeof(Model));
            (this.Component as Diagram).Model = m;
            if (DiagramLoadBaseWizard.GetAutoRunWizard())
            {
                using (DiagramWindowsOnLoadWizard wizard = new DiagramWindowsOnLoadWizard(this.Control))
                {
                    wizard.ShowDialog();
                    ((Diagram)this.Component).Load(wizard.EDDPath);
                }
            }
        }

        /// <summary>
        /// Call When the smart tag is clicked.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                listCollection = new DesignerActionListCollection();
                listCollection.Add(new DiagramActionList(this.Component));
                return listCollection;
            }
        }
    }

    /// <summary>
    /// To display the list of Items display in smart tag.
    /// </summary>
    internal class DiagramActionList : DesignerActionList
    {
        private Diagram diagram;
        private DesignerActionUIService UIservice;

        #region Constructor
        public DiagramActionList(IComponent component)
            : base(component)
        {
            diagram = component as Diagram;
            UIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            this.AutoShow = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether view show horizontal and vertical rulers.
        /// </summary>
        public bool ShowRulers
        {
            get
            {
                return diagram.ShowRulers;
            }
            set
            {
               GetPropertyByName("ShowRulers").SetValue(diagram, value);
               this.UIservice.Refresh(this.Component);
            }
        }

        /// <summary>
        /// Gets or sets the height of rulers.
        /// </summary>
        public int RulerHeight
        {
            get
            {
                return diagram.RulersHeight;
            }
            set
            {
                if ((value / 2 - 3) <= 0)
                    return;
                
                GetPropertyByName("RulersHeight").SetValue(diagram, value);
                this.UIservice.Refresh(this.Component);
            }
        }

        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars.
        /// </summary>
        public Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return diagram.Office2007ScrollBarsColorScheme;
            }
            set
            {
                GetPropertyByName("Office2007ScrollBarsColorScheme").SetValue(diagram, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show Office2007ScrollBars.
        /// </summary>
        public bool Office2007ScrollBars
        {
            get
            {
                return diagram.Office2007ScrollBars;
            }
            set
            {
                if (value)
                {
                    diagram.MetroScrollBars = false;
                    GetPropertyByName("Office2007ScrollBars").SetValue(diagram, value);
                }
                else
                {
                    GetPropertyByName("Office2007ScrollBars").SetValue(diagram, value);
                    if (diagram.MetroScrollBars)
                        GetPropertyByName("GridOfficeScrollBars").SetValue(diagram, OfficeScrollBars.Metro);
                    else
                        GetPropertyByName("GridOfficeScrollBars").SetValue(diagram, OfficeScrollBars.None);
                }
                this.UIservice.Refresh(this.Component);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show metro scrollbars.
        /// </summary>
        public bool MetroScrollBars 
        {
            get
            {
                return diagram.MetroScrollBars;
            }
            set
            {
                if (value)
                {
                    diagram.Office2007ScrollBars = false;
                    GetPropertyByName("MetroScrollBars").SetValue(diagram, value);
                }
                else
                {
                    GetPropertyByName("MetroScrollBars").SetValue(diagram, value);
                    if (diagram.Office2007ScrollBars)
                        GetPropertyByName("GridOfficeScrollBars").SetValue(diagram, OfficeScrollBars.Office2007);
                    else
                        GetPropertyByName("GridOfficeScrollBars").SetValue(diagram, OfficeScrollBars.None);
                }                
                this.UIservice.Refresh(this.Component);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the horizontal scroll bar is visible.
        /// </summary>
        public bool HScroll
        {
            get
            {
                return diagram.HScroll;
            }
            set
            {
                GetPropertyByName("HScroll").SetValue(diagram, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vertical scroll bar is visible.
        /// </summary>
        public bool VScroll
        {
            get
            {
                return diagram.VScroll;
            }
            set
            {
                GetPropertyByName("VScroll").SetValue(diagram, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether autoresizing model size 
        /// on out nodes bounds on model.
        /// </summary>
        public bool SizeToContent
        {
            get
            {
                return diagram.Model.SizeToContent;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["SizeToContent"].SetValue(diagram.Model, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether boundary constraints are enabled or not.
        /// </summary>
        public bool BoundaryConstraintsEnabled
        {
            get
            {
                return diagram.Model.BoundaryConstraintsEnabled;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["BoundaryConstraintsEnabled"].SetValue(diagram.Model, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line bridging is enabled for the model.
        /// </summary>
        public bool LineBridgingEnabled
        {
            get
            {
                return diagram.Model.LineBridgingEnabled;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["LineBridgingEnabled"].SetValue(diagram.Model, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line routing is enabled for the model.
        /// </summary>
        public bool LineRoutingEnabled
        {
            get
            {
                return diagram.Model.LineRoutingEnabled;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["LineRoutingEnabled"].SetValue(diagram.Model, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to improve the performance while dragging nodes.
        /// </summary>
        public bool OptimizeLineBridging
        {
            get
            {
                return diagram.Model.OptimizeLineBridging;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["OptimizeLineBridging"].SetValue(diagram.Model, value);
            }
        }

        /// <summary>
        /// Gets or sets properties used to fill the interior of document.
        /// </summary>      
        public FillStyle BackgroundStyle
        {
            get
            {
                return diagram.Model.BackgroundStyle;
            }
            set
            {
                IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host != null)
                {
                    DesignerTransaction transaction = host.CreateTransaction();
                    PropertyDescriptor descriptor = GetPropertyByName("Model");
                    descriptor.GetChildProperties()["BackgroundStyle"].SetValue(diagram.Model, value);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the document.
        /// </summary>
        public PageSize DocumentSize
        {
            get
            {
                return diagram.Model.DocumentSize;
            }
            set
            {
                IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host != null)
                {
                    DesignerTransaction transaction = host.CreateTransaction();
                    PropertyDescriptor descriptor = GetPropertyByName("Model");
                    descriptor.GetChildProperties()["DocumentSize"].SetValue(diagram.Model, value);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Measurement Unit of the Document.
        /// </summary>
        public MeasureUnits MeasurementUnits
        {
            get
            {
                return this.diagram.Model.MeasurementUnits;
            }
            set
            {
                PropertyDescriptor descriptor = GetPropertyByName("Model");
                descriptor.GetChildProperties()["MeasurementUnits"].SetValue(diagram.Model, value);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets and sets the Background style properties to a model.
        /// </summary>
        public void DocumentBackStyle()
        {
            FillStyleDialog dialog = new FillStyleDialog();
            FillStyle style = BackgroundStyle;
            dialog.FillStyle.Color = style.Color;
            dialog.FillStyle.ForeColor = style.ForeColor;
            dialog.FillStyle.ColorAlphaFactor = style.ColorAlphaFactor;
            dialog.FillStyle.ForeColorAlphaFactor = style.ForeColorAlphaFactor;
            dialog.FillStyle.Type = style.Type;
            dialog.FillStyle.GradientAngle = style.GradientAngle;
            dialog.FillStyle.GradientCenter = style.GradientCenter;
            dialog.FillStyle.PathBrushStyle = style.PathBrushStyle;
            dialog.FillStyle.HatchBrushStyle = style.HatchBrushStyle;
            dialog.FillStyle.Texture = style.Texture;
            dialog.FillStyle.TextureWrapMode = style.TextureWrapMode;
            dialog.BackColor = Color.FromArgb(239, 238, 239);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                style.Color = dialog.FillStyle.Color;
                style.ForeColor = dialog.FillStyle.ForeColor;
                style.ColorAlphaFactor = dialog.FillStyle.ColorAlphaFactor;
                style.ForeColorAlphaFactor = dialog.FillStyle.ForeColorAlphaFactor;
                style.Type = dialog.FillStyle.Type;
                style.GradientAngle = dialog.FillStyle.GradientAngle;
                style.GradientCenter = dialog.FillStyle.GradientCenter;
                style.PathBrushStyle = dialog.FillStyle.PathBrushStyle;
                style.HatchBrushStyle = dialog.FillStyle.HatchBrushStyle;
                style.Texture = dialog.FillStyle.Texture;
                style.TextureWrapMode = dialog.FillStyle.TextureWrapMode;
                BackgroundStyle = style;
            }
        }

        /// <summary>
        /// Gets or sets the Document size to the model.
        /// </summary>       
        public void DocumentSizeDisplay()
        {
            PageSizeDialog dialog = new PageSizeDialog();
            dialog.PageSize = DocumentSize;
            Model model = diagram.Model;
            if (model != null)
            {
                RectangleF rcRect = new HandleRenderer().GetBoundingRect(model.Nodes);
                float fWidth = Math.Max(rcRect.Right, model.MinimumSize.Width);
                float fHeight = Math.Max(rcRect.Bottom, model.MinimumSize.Height);

                dialog.ModelContentSize = new SizeF(fWidth, fHeight);
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DocumentSize = dialog.PageSize;
            }
        }

        public void ShowWizard()
        {
            DiagramWindowsOnLoadWizard wizard = new DiagramWindowsOnLoadWizard(diagram);
            {
                wizard.ShowDialog();
                diagram.Load(wizard.EDDPath);
                this.InvalidateComponent(diagram);
            }
        }

        /// <summary>
        /// Gets the Property from the Component.
        /// </summary>
        /// <param name="name">Name of the Property.</param>
        /// <returns>Abstraction of the property</returns>
        public PropertyDescriptor GetPropertyByName(string name)
        {
            PropertyDescriptor property;
            property = TypeDescriptor.GetProperties(diagram)[name];
            if (property == null)
                throw new ArgumentException("Invalid Property Name");
            return property;
        }

        /// <summary>
        /// Gets the list of smart tag items.
        /// </summary>
        /// <returns>Designer action item collection.</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection itemCollection = new DesignerActionItemCollection();

            itemCollection.Add(new DesignerActionHeaderItem("Diagram Properties"));
            itemCollection.Add(new DesignerActionPropertyItem("ShowRulers", "Show Rulers", "Diagram Properties", "Specifies whether Rulers should be enabled "));
            if (ShowRulers)
                itemCollection.Add(new DesignerActionPropertyItem("RulerHeight", "Rulers Height", "Diagram Properties", "Set Rulers Height"));
            itemCollection.Add(new DesignerActionPropertyItem("Office2007ScrollBars", "Office2007 ScrollBars", "Diagram Properties", "Specifies whether Office2007Scrollbars should be enabled."));
            if (Office2007ScrollBars)
                itemCollection.Add(new DesignerActionPropertyItem("Office2007ScrollBarsColorScheme", "Office2007ScrollBarsColorScheme", "Diagram Properties", "Set/Get Office2007 ScrollBars ColorScheme"));
            itemCollection.Add(new DesignerActionPropertyItem("MetroScrollBars", "Metro ScrollBars", "Diagram Properties", "Specifies whether MetroScrollBars should be enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("HScroll", "HScroll", "Diagram Properties", "Specifies whether HScroll should be enabled"));
            itemCollection.Add(new DesignerActionPropertyItem("VScroll", "VScroll", "Diagram Properties", "Specifies whether VScroll should be enabled"));

            itemCollection.Add(new DesignerActionHeaderItem("Model Properties"));
            itemCollection.Add(new DesignerActionPropertyItem("SizeToContent", "SizeToContent", "Model Properties", "Specifies whether SizeToContent should be enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("BoundaryConstraintsEnabled", "Boundary Constraints Enabled", "Model Properties", "Specifies whether BoundaryConstraints should be Enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("LineBridgingEnabled", "Line Bridging Enabled", "Model Properties", "Specifies whether Line Bridging should be enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("LineRoutingEnabled", "Line Routing Enabled", "Model Properties", "Specifies whether Line Routing should be enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("OptimizeLineBridging", "Optimize Line Bridging", "Model Properties", "Specifies whether OptimizeLine Bridging should be enabled."));
            itemCollection.Add(new DesignerActionPropertyItem("MeasurementUnits", "Measurement Units", "Model Properties", "Specifies Unit of measure used for world coordinates."));
            itemCollection.Add(new DesignerActionMethodItem(this, "DocumentBackStyle", "Background Style", "Model Properties", "Fill style for filling document interior."));
            itemCollection.Add(new DesignerActionMethodItem(this, "DocumentSizeDisplay", "Document Size", "Model Properties", "Specifies the size of the document."));
            itemCollection.Add(new DesignerActionMethodItem(this, "ShowWizard", "Show Wizard", true));

            return itemCollection;
        }

        /// <summary>
        /// Invalidates current component
        /// </summary>
        /// <param name="component">The component.</param>
        private void InvalidateComponent(IComponent component)
        {
            if (component.Site != null)
            {
                IDesignerHost iDesignerHost = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                if (iDesignerHost != null)
                {
                    IComponentChangeService componentChangeService = iDesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                    if (componentChangeService != null)
                    {
                        componentChangeService.OnComponentChanged(component, null, null, null);
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Interactive two-dimensional graphics control for diagramming,
    /// technical drawing, visualization, simulation, and technical
    /// drawing applications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control provides a surface for rendering and manipulating 2D
    /// shapes, symbols, text, and images. The user interface supports drag-
    /// and-drop, scaling, rotation, zooming, grouping, ungrouping, connection
    /// points, and many other features.
    /// </para>
    /// <para>
    /// A diagram is composed of three objects: the 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.Model"/>,
    /// the 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.View"/>,
    /// and the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.Controller"/>.
    /// The model-view-controller architecture provides a clear separation between
    /// data, visualization, and user interface. The model contains the data portion
    /// of the diagram, the view is responsible for rendering the diagram, and
    /// the controller handles user interaction. The model, view, and controller
    /// are accessible as properties in this control and can be manipulated
    /// directly.
    /// </para>
    /// <para>
    /// Some of the methods and properties in this class are just wrappers that
    /// call identical methods in the model, view, or controller. For example,
    /// the following two lines of codes are equivalent:
    /// <code>
    /// diagram.Undo();
    /// // Same as
    /// diagram.Controller.Undo();
    /// </code>
    /// Methods that are simple wrappers are documented as such.
    /// </para>
    /// <para>
    /// Graphical objects can be added to a diagram in several ways. One way is
    /// through drag-and-drop. Symbols can be dragged from a
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupView"/>
    /// onto the diagram. Objects can also be added from the clipboard using the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.Paste()"/>
    /// method. Shapes can be drawn onto the diagram by activating one of several
    /// drawing tools such as the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.RectangleTool"/>. Objects
    /// can also be created programmatically and added to the diagram by
    /// calling the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.AppendChild"/> method.
    /// </para>
    /// <para>
    /// Activating user-interface tools is a task commonly performed by applications
    /// using this control. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.ActivateTool"/>
    /// method is used to activate tools. For example, the event handler for a
    /// toolbar button that draws a rectangle would look like this.
    /// <code>
    /// private void drawRectangle_Click(object sender, System.EventArgs e)
    /// {
    ///     this.Diagram.ActivateTool("RectangleTool");
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Calling the <see cref="HistoryManager.Undo"/>
    /// method removes the command on the top of the undo stack and causes an undo
    /// to occur. The
    /// <see cref="HistoryManager.Redo"/>
    /// method will redo the last command that was removed from the undo stack. The
    /// UndoCommand and RedoCommand methods are usually called in response to clicking
    /// Undo and Redo on the Edit menu.
    /// </para>
    /// <para>
    /// One advantage of the model-view-controller architecture is that the
    /// parts are interchangeable . Models, views, and controllers can be swapped
    /// in and out independently. For example, the user interface of the diagram
    /// can be completely replaced by swapping in a different controller
    /// implementation. To accomplish this, you must subclass this class and
    /// override one or more of the following methods:
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateModel"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateView"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateController"/>.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ICommand"/>
    /// </remarks>
    [
    ToolboxItem(true),
    ToolboxBitmap(typeof(Diagram), "ToolboxIcons.Diagram.bmp"),
    Description("Interactive 2D graphics and diagramming."),
    Designer(typeof(DiagramDesigner))
    ]
    public class Diagram :
        ScrollControl, 
        IViewer, 
        IServiceReferenceProvider, 
        ISupportInitialize,
        IPropertyObserver,
        IPropertyContainer
    {
        #region Class constants
        /// <summary>
        /// Default large change for both scrollbars.
        /// </summary>
        private const int c_nLARGE_CHANGE_DEFAULT_VALUE = 10;
        #endregion

        private static bool bDebug = false;
		private bool m_bInUpdate = false;
        private RectangleF m_scrollBounds = RectangleF.Empty;
        private PointF m_ptCurMouse = PointF.Empty;
        private PointF m_ptOrigin = PointF.Empty;
        
        #region ISupportInitialize interface
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            OnDiagramInitialized(this, new EventArgs());
        }
        #endregion

        #region Class members
        private bool m_bUpdate;

        /// <summary>
        /// This field is used to resolve rendering problem on resizing diagram control
        /// ( ClipRectangle while rendering is equal to ClientRectangle !!before!! resizing
        /// when I call Invalidate( this.ClientRectangle ) )
        /// It is set to true on OnSizeChanged(..) and set to false on OnMouseMove(..)
        /// </summary>
        private bool m_bResize;

        /// <summary>
        /// Focus Manager.
        /// </summary>
        private FocusManager m_mgrFocus;
        private ViewerEventSink m_eventSink;

        /// <summary>
        /// Properties for rendering the model.
        /// </summary>
        private Model m_model;
        private View m_view;
        private DiagramController m_controller;
        private LayoutManager layoutManager;
        private bool dragging;
        private float scrollGranularity = 0.1f;
        private bool m_bLockScrollUpdate;
        private bool m_bMagnificationChanged;
        private float nudgeIncrement = 1.0f;
        private PointF ptScrollOriginRef;
        private Ruler m_rulerHorizontal;
        private Ruler m_rulerVertical;
        private bool m_bShowRulers;
        private bool m_bNeedDocumentRefresh = true;
        private bool m_bNeedHRulerRefresh = true;
        private bool m_bNeedVRulerRefresh = true;
        private DiagramDocument m_document;
        private bool c_bFocusNodeOnTab = false;
        private bool m_bDefaultContextMenuEnabled = true;
        string m_strFileName = string.Empty;
        private bool m_bMouseWheelZoom = false;
        private IDataObject m_dataObj = null;
        private Node m_dropNode = null;
        private Bitmap m_dropNodeImage = null;
        private bool m_bTouchMode = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Diagram"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Diagram(IContainer container)
            : this()
        {
		this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
#if !NO_LICENSE
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(Diagram));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Diagram"/> class.
        /// </summary>
        public Diagram()
        {
            // This call is required by the Windows.Forms Form Designer.
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
#if !NO_LICENSE
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(Diagram));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();
            
            // Initialize Essential Diagram.
            // Create the model, view, and controller objects and initialize.
            View = CreateView();

            HookViewEvents();

            m_controller = CreateController();
            m_controller.UpdateServiceReferences(this);
            m_controller.Initialize();

            ////////////////////////////////////////////////////////////////
            // This prevents the base ScrollControl class from actually
            // changing the window origin using ScrollWindowEx() when
            // ScrollControl.ScrollWindow() is called.
            this.DisableScrollWindow = false;
            ////////////////////////////////////////////////////////////////

            // Set scroll bar sizes based on the virtual size of the view.
            UpdateScrollRange();

            this.RulersHeight = 20;
            InitializeDefaultContextMenu();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.View != null)
                {
                    this.View.Dispose();
                    m_view = null;
                }

                if (m_controller != null)
                {
                    m_controller.Uninitialize();
                    m_controller = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // Diagram
            // 
            this.AllowDrop = true;
            this.Size = new System.Drawing.Size(185, 185);
        }

        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
		/// Get the service reference from provider.
		/// </summary>
        /// <param name="typeHandle">The service type.</param>
		/// <returns></returns>
        public object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            return GetService(Type.GetTypeFromHandle(typeHandle));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the current diagram document that contain 
        /// Model, View and Controller components.
        /// </summary>
        /// <value>The diagram document.</value>
        /// <remarks>
        /// Uses to open document in design mode. Can be null.
        /// </remarks>
        [Browsable(true)]
        [Category("Data")]
        [Editor(typeof(DiagramOpener), typeof(UITypeEditor))]
        [TypeConverter(typeof(DiagramDocumentConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        [Description("Gets or sets the current diagram document that contain Model, View and Controller components.")]
        public DiagramDocument Document
        {
            get 
            { 
                return m_document; 
            }
            set
            {
                if (m_document != value)
                {
                    InitDiagram(value);
                    m_document = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal ruler.
        /// </summary>
        /// <value>The horizontal ruler.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ruler HorizontalRuler
        {
            get
            {
                if (m_rulerHorizontal == null)
                {
                    m_rulerHorizontal = new HorizontalRuler();
                    m_rulerHorizontal.UpdateServiceReferences(this);
                    UpdateHRulerBounds();
                }

                return m_rulerHorizontal;
            }
            set
            {
                if (value != m_rulerHorizontal)
                {
                    m_rulerHorizontal = value;
                    m_rulerHorizontal.UpdateServiceReferences(this);
                    UpdateHRulerBounds();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the control accept data that user drags and drops onto it.
        /// </summary>
        [DefaultValue(true)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical ruler.
        /// </summary>
        /// <value>The vertical ruler.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ruler VerticalRuler
        {
            get
            {
                if (m_rulerVertical == null)
                {
                    m_rulerVertical = new VerticalRuler();
                    m_rulerVertical.UpdateServiceReferences(this);
                    UpdateVRulerBounds();
                }

                return m_rulerVertical;
            }
            set
            {
                if (value != m_rulerVertical)
                {
                    m_rulerVertical = value;
                    m_rulerVertical.UpdateServiceReferences(this);
                    UpdateVRulerBounds();
                }
            }
        }

        /// <summary>
        /// Gets the focus manager reference.
        /// </summary>
        /// <value>The focus manager.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FocusManager FocusManager
        {
            get
            {
                if (m_mgrFocus == null)
                {
                    m_mgrFocus = new FocusManager();
                    m_mgrFocus.UpdateServiceReferences(this);
                }

                return m_mgrFocus;
            }
        }

        /// <summary>
        /// Gets or sets the view is responsible for rendering the model onto a window.
        /// </summary>
        /// <remarks>
        /// A view is set inside of a window and has bounds that are measured
        /// in device coordinates. The view renders itself onto a
        /// System.Drawing.Graphics object. The view is created by calling the
        /// virtual method
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateView"/>.
        /// The CreateView method can be overridden in derived classes in order to
        /// plug custom views into the diagram.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("MVC"),
        Description("The View renders the diagram model onto a window.")
        ]
        public View View
        {
            get { return m_view; }
            set
            {
                // raise property changing event
                if (m_view != value && OnPropertyChanging(DPN.View,value))
                {
                    // update references
                    if (m_view != null)
                        m_view.UpdateServiceReferences(null);

                    // assign new value
                    m_view = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.View);

                    // update references
                    if (m_view != null)
                        m_view.UpdateServiceReferences(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets layout manager responsible for updating the layout of the diagram.
        /// </summary>
        [Browsable(true),
        Category("Layout"),
        Description("Layout manager object responsible for layout of nodes in the diagram.")]
        public LayoutManager LayoutManager
        {
            get
            {
                return this.layoutManager;
            }
            set
            {
                if (this.layoutManager != value)
                {
                    if (this.layoutManager != null)
                    {
                        this.layoutManager.Model = null;
                        this.layoutManager.LayoutUpdated -= new EventHandler(OnLayoutUpdated);
                    }

                    this.layoutManager = value;

                    if (this.layoutManager != null)
                    {
                        this.layoutManager.Model = this.Model;
                        this.layoutManager.LayoutUpdated += new EventHandler(OnLayoutUpdated);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the level of granularity for scrolling.
        /// </summary>
        /// <remarks>
        /// This value is to scale the scroll range of the scroll bars. The value
        /// of this property must be greater than 0. This value is multiplied by
        /// virtual size of the view in order to get the scroll range. For example,
        /// if the virtual size of the view is 100x50 and this property is set to
        /// 0.5f, then the horizontal scroll range is set to 0..50 and the vertical
        /// scroll range is set to 0..25.
        /// </remarks>
        [
        Description("Determines the level of granularity for scrolling."),
        DefaultValue(0.1f),
        Category("Scrolling")
        ]
        public float ScrollGranularity
        {
            get
            {
                return this.scrollGranularity;
            }
            set
            {
                if (value > 0.0f)
                {
                    this.scrollGranularity = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", value, Resources.Strings.Messages.Get("ArgumentRange"));
                }
            }
        }

        /// <summary>
        /// Gets or sets number of logical units to move nodes during a nudge operation.
        /// </summary>
        [Description("Number of logical units to move nodes during a nudge operation.")]
        [DefaultValue(1.0f)]
        [Category("Behavior")]
        public float NudgeIncrement
        {
            get
            {
                return this.nudgeIncrement;
            }
            set
            {
                this.nudgeIncrement = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image displayed in the control.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Drawing.Image"></see> that represents the image to display in the background of the control.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new System.Drawing.Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Gets or sets the background image layout as defined in the <see cref="T:System.Windows.Forms.ImageLayout"></see> enumeration.
        /// </summary>
        /// <value></value>
        /// <returns>One of the values of <see cref="T:System.Windows.Forms.ImageLayout"></see> (<see cref="F:System.Windows.Forms.ImageLayout.Center"></see> , <see cref="F:System.Windows.Forms.ImageLayout.None"></see>, <see cref="F:System.Windows.Forms.ImageLayout.Stretch"></see>, <see cref="F:System.Windows.Forms.ImageLayout.Tile"></see>, or <see cref="F:System.Windows.Forms.ImageLayout.Zoom"></see>). <see cref="F:System.Windows.Forms.ImageLayout.Tile"></see> is the default value.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The specified enumeration value does not exist. </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new System.Windows.Forms.ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }
#endif

        /// <summary>
        /// Gets or sets the bounds of scrollable area. Can be set only positive values.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Description("Determines the bounds of scrollable area.")]
        [Browsable(false)]
        [Category("Scrolling")]
        public RectangleF ScrollVirtualBounds
        {
            get { return this.View.ScrollVirtualBounds; }
            set { this.View.ScrollVirtualBounds = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether view default context menu.
        /// </summary>
        /// <value><c>true</c> if show default context menu; otherwise, <c>false</c>.</value>        
        [Description("Gets or sets whether to show the default context menu.")]
        [DefaultValue(true)]
        public bool DefaultContextMenuEnabled
        {
            get
            {
                return m_bDefaultContextMenuEnabled;
            }
            set
            {
                if (value != m_bDefaultContextMenuEnabled)
                {
                    m_bDefaultContextMenuEnabled = value;
                    if (m_bDefaultContextMenuEnabled)
                        InitializeDefaultContextMenu();
                    else
                        this.ContextMenuStrip = null;

                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that whether the touch mode is enabled or not.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]
        public bool TouchMode
        {
            get { return m_bTouchMode; }
            set
            {
                if (value != m_bTouchMode)
                {
                    m_bTouchMode = value;
                    HandlesHitTesting.TouchMode = value;
                }
            }
        }
        #endregion

        #region IViewer
        /// <summary>
        /// Gets the reference to viewer event sink.
        /// </summary>
        /// <value>The event sink.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewerEventSink EventSink
        {
            get
            {
                if (m_eventSink == null)
                {
                    m_eventSink = new DiagramViewerEventSink();
                    m_eventSink.Start();
                }

                return m_eventSink;
            }
        }

        /// <summary>
        /// Gets or sets the height of the rulers.
        /// </summary>
        /// <value>The height of the rulers.</value>
        [DefaultValue(20)]
        [Category("Rulers")]
        [Description("Gets or sets the height of rulers.")]
        public int RulersHeight
        {
            get 
            { 
                return this.HorizontalRuler.Size.Height; 
            }
            set
            {
                if (this.HorizontalRuler != null)
                {
                    this.HorizontalRuler.Size = new Size(this.HorizontalRuler.Size.Width, value);
                }

                if (this.VerticalRuler != null)
                {
                    this.VerticalRuler.Size = new Size(this.VerticalRuler.Size.Width, value);
                }

                Invalidate(this.ClientRectangle);
                UpdateClientArea();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether view show horizontal and vertical rulers.
        /// </summary>
        /// <value><c>true</c> if show rulers; otherwise, <c>false</c>.</value>
        [Category("Rulers")]
        [Description("Gets or sets whether to show the horizontal and vertical rulers.")]
        [DefaultValue(false)]
        public bool ShowRulers
        {
            get 
            { 
                return m_bShowRulers; 
            }
            set
            {
                if (m_bShowRulers != value && OnPropertyChanging(DPN.ShowRulers, value))
                {
                    m_bShowRulers = value;

                    Invalidate(this.ClientRectangle);
                    UpdateClientArea();

                    OnPropertyChanged(DPN.ShowRulers);
                }
            }
        }

        /// <summary>
        /// Gets the view origin.
        /// </summary>
        /// <value>The origin.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF Origin
        {
            get { return this.View.Origin; }
        }

        /// <summary>
        /// Gets the magnification percent.
        /// </summary>
        /// <value>The magnification percent.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Magnification
        {
            get { return this.View.Magnification; }
        }

        /// <summary>
        /// Gets or sets the controller processes input and translates it into commands and actions
        /// on the model and view.
        /// </summary>
        /// <remarks>
        /// The controller defines the user interface. It is created by calling the
        /// virtual method's
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateController"/>.
        /// The CreateController method can be overridden in derived classes in order to
        /// plug custom controllers into the diagram.
        /// </remarks>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("MVC")]
        [Description("Processes input and translates it into commands and actions. Determines interactive behavior of the diagram.")]
        public DiagramController Controller
        {
            get 
            { 
                return this.m_controller; 
            }
            set
            {
                // On property changing
                if (m_controller != value)
                {
                    if(m_controller != null)
                    {
                        m_controller.Uninitialize();
                        m_controller.UpdateServiceReferences(null);
                    }

                    // update references
                    m_controller = value;
                    
                    // we update references only if m_controller not null
                    if (m_controller != null)
                    {                        
                        m_controller.UpdateServiceReferences(this);
                        m_controller.Initialize();                        
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the model contains the hierarchy of graphical nodes that are rendered
        /// onto the view and manipulated by the controller.
        /// </summary>
        /// <remarks>
        /// The model contains the data portion of a diagram. When a diagram is
        /// persisted, it is the Model that is serialized. The model is created
        /// by calling the virtual method
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.CreateModel"/>.
        /// The CreateModel method can be overridden in derived classes in order to
        /// plug custom models into the diagram.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
        /// </remarks>
        [Browsable(true)]
        [Category("MVC")]
        [Description("The Model contains the data portion of the diagram.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ComponentConverter))]
        public Model Model
        {
            get 
            { 
                return m_model; 
            }
            set
            {
                // on property changing
                if (m_model != value)
                {
                    if (m_model != null)
                        UnsubscribeForModelEvents(m_model);

                    // update references + update controller references
                    // subscribe for model event sink
                    m_model = value;

                    if (Container != null)
                    {
                        Container.Add(m_model);
                    }

                    if (m_model != null)
                    {
                        SubscribeForModelEvents(m_model);
                    }
                    
                    // update controller's document reference
                    if (this.Controller != null)
                    {
                        this.Controller.UpdateServiceReferences(this);
                    }
                    
                    // on property changed
                    if (this.View != null)
                    {
                        this.View.UpdateServiceReferences(this);
                        ResetScrollPosition();
                    }

                    // raise property changed event
                    OnPropertyChanged(DPN.Model);

                    if (m_document != null)
                    {
                        // save reference
                        DiagramDocument doc = m_document;
                        
                        // remove document reference temporary
                        m_document = null;
                        
                        // merge new model and save document reference
                        InitDiagram(doc);
                    }

                    if (this.DesignMode)
                    {
                        Invalidate(true);
                    }
                    else
                    {
                        UpdateClientArea();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Tab behavior needed for diagram control.
        /// </summary>
        [Description("Defines behavior of Diagram control on Tab press.")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool FocusNodeOnTab
        {
            get
            {
                return c_bFocusNodeOnTab;
            }
            set
            {
                c_bFocusNodeOnTab = value;
            }
        }


        /// <summary>
        /// Fit the document on control field.
        /// </summary>
        public void FitDocument()
        {
            if (this.Model == null || this.View == null)
                return;

            SizeF szModel = m_model.LogicalSize;
            szModel = new SizeF(MeasureUnitsConverter.Convert(szModel.Width, this.Model.MeasurementUnits, MeasureUnits.Pixel), MeasureUnitsConverter.Convert(szModel.Height, this.Model.MeasurementUnits, MeasureUnits.Pixel));
            SizeF szControl = this.AutoScrollBounds.Size;

            float fWidthFactor = szControl.Width / szModel.Width;
            float fHeightFactor = szControl.Height / szModel.Height;

            // calculate new magnification factor
            float fMagnification = Math.Min(fWidthFactor, fHeightFactor);

            float fOriginX = -(szControl.Width / 2 - (szModel.Width * fMagnification) / 2) / fMagnification;
            float fOriginY = -(szControl.Height / 2 - (szModel.Height * fMagnification) / 2) / fMagnification;

            // calculate new original
            PointF ptOrigin = new PointF(fOriginX, fOriginY);

            // set new origin offset and magnification
            this.View.Magnification = fMagnification * 100f;
            this.View.Origin = ptOrigin;


        }

        /// <summary>
        /// Updates the view area
        /// <param name="node">The node.</param>
        /// </summary>
        public void UpdateView(Node node)
        {
            if (node is ControlNode)
            {
                ((ControlNode)node).UpdateControlSnapshot();
            }
            RectangleF rect = node.BoundingRectangle;
            rect = this.Controller.ConvertFromModelToClientCoordinates(rect);
            this.Controller.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(rect));
            UpdateView();
        }

        /// <summary>
        /// Updates the view area.
        /// </summary>
        public void UpdateView()
        {
            // merge invalid rect with current tool work rect
            System.Drawing.Rectangle rectTool = this.Controller.ActiveTool.ToolWorkRect;

            if (!rectTool.Size.IsEmpty)
            {
                this.Controller.UpdateInfo.UpdateRefreshRect(rectTool);
            }

            // merge FocusManager invalid rect
            System.Drawing.Rectangle rect = this.Controller.ConvertFromModelToClientCoordinates(this.FocusManager.WorkRect);

            if (!rect.Size.IsEmpty)
            {
                this.Controller.UpdateInfo.UpdateRefreshRect(rect);
            }

            if (!this.Controller.UpdateInfo.m_rectUpdate.Size.IsEmpty)
            {
                this.Controller.UpdateInfo.m_rectUpdate.Inflate(2, 2);
                System.Drawing.Rectangle rectTemp = this.Controller.UpdateInfo.m_rectUpdate;

                if (this.ShowRulers)
                {
                    System.Drawing.Rectangle rect1 = System.Drawing.Rectangle.Empty;
                    rect1.Location = new Point(this.RulersHeight, 0);
                    rect1.Size = new Size(this.ClientSize.Width - this.RulersHeight, this.RulersHeight);

                    if (rect1.IntersectsWith(rectTemp))
                    {
                        rectTemp.Height = rectTemp.Bottom - rect1.Y;
                        rectTemp.Y = rect1.Bottom;
                    }

                    rect1 = System.Drawing.Rectangle.Empty;
                    rect1.Location = new Point(0, this.RulersHeight);
                    rect1.Size = new Size(this.RulersHeight, this.ClientSize.Height - this.RulersHeight);

                    if (rect1.IntersectsWith(rectTemp))
                    {
                        rectTemp.Width = rectTemp.Right - rect1.X;
                        rectTemp.X = rect1.Right;
                    }
                }

                m_bNeedDocumentRefresh = true;

                // update document
                Invalidate(rectTemp);
                Update();

                this.Controller.UpdateInfo.m_rectUpdate = System.Drawing.Rectangle.Empty;
            }
            this.UpdateScrollRange();
            m_bNeedDocumentRefresh = true;
        }
        private void UpdateHorizontalRuler(System.Drawing.Rectangle rectHightlight)
        {
            System.Drawing.Rectangle rectInvalid = AcumulateHRulerInvalidArea(rectHightlight);
            Ruler ruler = this.HorizontalRuler;

            ruler.HighlightAreaStart = rectHightlight.X;
            ruler.HighlightAreaWidth = rectHightlight.Width;
            UpdateHRulerBounds();

            if (rectInvalid.X + rectInvalid.Width > 0)
            {
                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = false;

                if (m_bResize)
                {
                    // invalidate whole ruler
                    rectInvalid.X = this.RulersHeight;
                    rectInvalid.Y = 0;
                    rectInvalid.Height = this.RulersHeight;
                    rectInvalid.Width = this.ClientSize.Width - this.RulersHeight;
                }

                Invalidate(rectInvalid);
                Update();

                m_bNeedHRulerRefresh = false;
            }
        }

        private void UpdateHRulerBounds()
        {
            Ruler ruler = this.HorizontalRuler;
            
            // update ruler bounds
            ruler.Location = new Point(this.RulersHeight, 0);
            ruler.Size = new Size(this.ClientSize.Width - this.RulersHeight, this.RulersHeight);
        }

        /// <summary>
        /// Accumulates the H ruler invalid area.
        /// </summary>
        /// <param name="rectHightlight">The highlight rectangle.</param>
        /// <returns>Bounds of the horizontal ruler.</returns>
        protected System.Drawing.Rectangle AcumulateHRulerInvalidArea(System.Drawing.Rectangle rectHightlight)
        {
            Ruler ruler = this.HorizontalRuler;
            System.Drawing.Rectangle rectInvalidPrev = System.Drawing.Rectangle.Empty;

            if (ruler.HighlightAreaWidth > 0)
            {
                // set invalid rect to previous invalid rect
                rectInvalidPrev.X = ruler.HighlightAreaStart;
                rectInvalidPrev.Width = ruler.HighlightAreaWidth;

                // cosider marker position
                if (ruler.MarkerPosition > (ruler.HighlightAreaStart + ruler.HighlightAreaWidth))
                {
                    rectInvalidPrev.Width = ruler.MarkerPosition - ruler.HighlightAreaStart;
                }
                else if (ruler.MarkerPosition < ruler.HighlightAreaStart)
                {
                    rectInvalidPrev.Width = ruler.HighlightAreaStart - ruler.MarkerPosition + ruler.HighlightAreaWidth;
                    rectInvalidPrev.X = ruler.MarkerPosition;
                }
            }
            else
            {
                rectInvalidPrev.X = ruler.MarkerPosition;
                rectInvalidPrev.Width = 1;
            }

            rectInvalidPrev.Height = this.RulersHeight;
            rectInvalidPrev.Width += 1;

            System.Drawing.Rectangle rectInvalidCur = System.Drawing.Rectangle.Empty;
            ruler.MarkerPosition = m_bUpdate ? -1 : PointToClient(MousePosition).X;

            if (!rectHightlight.IsEmpty)
            {
                // set invalid rect to previous invalid rect
                rectInvalidCur.X = rectHightlight.X;
                rectInvalidCur.Width = rectHightlight.Width;

                // cosider marker position
                if (ruler.MarkerPosition > rectHightlight.Right)
                {
                    rectInvalidCur.Width = ruler.MarkerPosition - rectHightlight.X;
                }
                else if (ruler.MarkerPosition < rectHightlight.X)
                {
                    rectInvalidCur.Width = rectHightlight.Right - ruler.MarkerPosition;
                    rectInvalidCur.X = ruler.MarkerPosition;
                }
            }
            else
            {
                rectInvalidCur.X = ruler.MarkerPosition;
                rectInvalidCur.Width = 1;
            }

            rectInvalidCur.Height = this.RulersHeight;
            rectInvalidCur.Width += 1;

            System.Drawing.Rectangle rectToReturn = System.Drawing.Rectangle.Empty;

            if (!rectInvalidPrev.IsEmpty && !rectInvalidCur.IsEmpty)
            {
                rectToReturn = System.Drawing.Rectangle.Union(rectInvalidPrev, rectInvalidCur);
            }

            return rectToReturn;
        }

        /// <summary>
        /// Updates the vertical ruler.
        /// </summary>
        /// <param name="rectHightlight">The hightlight rectangle.</param>
        private void UpdateVerticalRuler(System.Drawing.Rectangle rectHightlight)
        {
            System.Drawing.Rectangle rectInvalid = AcumulateVRulerInvalidArea(rectHightlight);

            Ruler ruler = this.VerticalRuler;

            ruler.HighlightAreaStart = rectHightlight.Y;
            ruler.HighlightAreaWidth = rectHightlight.Height;

            UpdateVRulerBounds();

            if (rectInvalid.Y + rectInvalid.Height > 0)
            {
                m_bNeedVRulerRefresh = true;
                m_bNeedHRulerRefresh = false;

                if (m_bResize)
                {
                    // invalidate whole ruler
                    rectInvalid.X = 0;
                    rectInvalid.Y = this.RulersHeight;
                    rectInvalid.Height = this.ClientSize.Height - this.RulersHeight;
                    rectInvalid.Width = this.RulersHeight;
                }

                Invalidate(rectInvalid);
                Update();

                m_bNeedVRulerRefresh = false;
            }
        }

        /// <summary>
        /// Updates the V ruler bounds.
        /// </summary>
        private void UpdateVRulerBounds()
        {
            Ruler ruler = this.VerticalRuler;
            
            // update ruler bounds
            ruler.Location = new Point(0, this.RulersHeight);
            ruler.Size = new Size(this.RulersHeight, this.ClientSize.Height - this.RulersHeight);
        }

        /// <summary>
        /// Accumulates  the V ruler invalid area.
        /// </summary>
        /// <param name="rectHightlight">The highlight rectangle.</param>
        /// <returns>Bounds of the vertical ruler.</returns>
        protected System.Drawing.Rectangle AcumulateVRulerInvalidArea(System.Drawing.Rectangle rectHightlight)
        {
            Ruler ruler = this.VerticalRuler;
            System.Drawing.Rectangle rectInvalidPrev = System.Drawing.Rectangle.Empty;

            if (ruler.HighlightAreaWidth > 0)
            {
                // set invalid rect to previous invalid rect
                rectInvalidPrev.Y = ruler.HighlightAreaStart;
                rectInvalidPrev.Height = ruler.HighlightAreaWidth;

                // cosider marker position
                if (ruler.MarkerPosition > (ruler.HighlightAreaStart + ruler.HighlightAreaWidth))
                {
                    rectInvalidPrev.Height = ruler.MarkerPosition - ruler.HighlightAreaStart;
                }
                else if (ruler.MarkerPosition < ruler.HighlightAreaStart)
                {
                    rectInvalidPrev.Height = ruler.HighlightAreaStart - ruler.MarkerPosition + ruler.HighlightAreaWidth;
                    rectInvalidPrev.Y = ruler.MarkerPosition;
                }
            }
            else
            {
                rectInvalidPrev.Y = ruler.MarkerPosition;
                rectInvalidPrev.Height = 1;
            }

            rectInvalidPrev.Width = this.RulersHeight;
            rectInvalidPrev.Height += 1;

            System.Drawing.Rectangle rectInvalidCur = System.Drawing.Rectangle.Empty;
            ruler.MarkerPosition = m_bUpdate ? -1 : PointToClient(MousePosition).Y;

            if (!rectHightlight.IsEmpty)
            {
                // set invalid rect to previous invalid rect
                rectInvalidCur.Y = rectHightlight.Y;
                rectInvalidCur.Height = rectHightlight.Height;

                // cosider marker position
                if (ruler.MarkerPosition > rectHightlight.Bottom)
                {
                    rectInvalidCur.Height = ruler.MarkerPosition - rectHightlight.Y;
                }
                else if (ruler.MarkerPosition < rectHightlight.Y)
                {
                    rectInvalidCur.Height = rectHightlight.Bottom - ruler.MarkerPosition;
                    rectInvalidCur.Y = ruler.MarkerPosition;
                }
            }
            else
            {
                rectInvalidCur.Y = ruler.MarkerPosition;
                rectInvalidCur.Height = 1;
            }

            rectInvalidCur.Height += 1;
            rectInvalidCur.Width = this.RulersHeight;

            System.Drawing.Rectangle rectToReturn = System.Drawing.Rectangle.Empty;

            if (!rectInvalidPrev.IsEmpty && !rectInvalidCur.IsEmpty)
            {
                rectToReturn = System.Drawing.Rectangle.Union(rectInvalidPrev, rectInvalidCur);
            }

            return rectToReturn;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the ContextMenu to NULL.
        /// </summary>
        public void DetachContextMenu()
        {
            this.ContextMenu = null;
        }

        /// <summary>
        /// This method creates the model that is attached to the diagram.
        /// </summary>
        /// <returns>A new model object.</returns>
        /// <remarks>
        /// This method can be overidden in derived classes in order to perform
        /// custom initialization of the model or to create custom models derived
        /// from the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Model"/> class.
        /// </remarks>
        public virtual Model CreateModel()
        {
            return new Model();
        }

        /// <summary>
        /// Attach an existing model to the diagram.
        /// </summary>
        /// <param name="value">Model object to attach.</param>
        /// <remarks>
        /// This method can be used to attach a new model object to the diagram.
        /// NOTE: Using this method will cause the contents of the previous model
        /// to be destroyed. Calling this method after your form's InitializeComponent
        /// method is called will cause design-time property values to be lost.
        /// </remarks>
        public void AttachModel(Model value)
        {
            if (m_model != value)
            {
                m_model = value;
                if (Container != null)
                {
                    Container.Add(m_model);
                }
                if (m_view != null)
                {
                    m_view.Model = m_model;
                }
            }
        }

        /// <summary>
        /// This method creates the view that is attached to the diagram.
        /// </summary>
        /// <returns>An instance of the <see cref="Syncfusion.Windows.Forms.Diagram.View"/> type.</returns>
        /// <remarks>
        /// Override this method to create a custom View for the diagram control.
        /// </remarks>
        public virtual View CreateView()
        {
            return new View();
        }

        /// <summary>
        /// This method creates the controller that is attached to the diagram.
        /// </summary>
        /// <returns>Controller object to attach.</returns>
        /// <remarks>
        /// This method can be overidden in derived classes in order to perform
        /// custom initialization of the controller or to create custom controllers
        /// derived from the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controller"/> class.
        /// </remarks>
        public virtual DiagramController CreateController()
        {
            DiagramController ctlr = new DiagramController();
            return ctlr;
        }

        /// <summary>
        /// Sets the X and Y magnification (zoom) values on a scale of 1 to n.
        /// </summary>
        /// <param name="magX">Magnification percent along X axis.</param>
        /// <param name="magY">Magnification percent along Y axis.</param>
        /// <remarks>
        /// This method sets the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
        /// property.
        /// </remarks>
        public void SetMagnification(int magX, int magY)
        {
            if (this.m_view != null)
            {
                this.m_view.Magnification = magX;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Activates the specified tool in the controller.
        /// </summary>
        /// <param name="toolName">Name of tool to activate.</param>
        /// <returns>True if tool activated; otherwise False.</returns>
        public bool ActivateTool(string toolName)
        {
            if (this.m_controller == null)
            {
                throw new InvalidOperationException(Resources.Strings.Messages.Get("ObjectState"));
            }
            return this.m_controller.ActivateTool(toolName);
        }

        /// <summary>
        /// Moves the specified nodes by a X and Y offset.
        /// </summary>
        /// <param name="nodes">Nodes to be moved.</param>
        /// <param name="dx">Distance to move along the X axis.</param>
        /// <param name="dy">Distance to move along the Y axis.</param>
        /// <param name="units">Move offsets measure units.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool MoveNodes(NodeCollection nodes, float dx, float dy, MeasureUnits units)
        {
            bool success = false;
             
            if (nodes.First is PseudoGroup)
            {
                NodeCollection nodesToMove = new NodeCollection();
                PseudoGroup pseudoGroup = nodes.First as PseudoGroup;
                nodesToMove = pseudoGroup.Nodes;
                nodes = nodesToMove;
            }
            if (this.Controller != null && nodes != null && CanPerformMoving())
            {
                IUnitIndependent unitIndependent;
                PointF[] ptPinLocation = new PointF[1];
                Matrix mtxTransformations;

                // wrap Atomic action
                if (this.Model != null)
                {
                    this.Model.HistoryManager.StartAtomicAction("Mode Nodes");
                    //this.Model.LinkManager.BeginSynchronization();
                    //this.Model.BridgeManager.BeginUpdateIntersection();
                    this.Model.BeginUpdate();
                }

                // move nodes
                foreach (Node node in nodes)
                {
                    unitIndependent = node;

                    // get pin location
                    ptPinLocation[0] = unitIndependent.GetPinPoint(units);

                    mtxTransformations = HandlesHitTesting.GetParentsTransformations(node, false);
                    
                    // apply transformations
                    mtxTransformations.TransformPoints(ptPinLocation);

                    // update pin location
                    ptPinLocation[0].X += dx;
                    ptPinLocation[0].Y += dy;

                    mtxTransformations.Invert();
                    mtxTransformations.TransformPoints(ptPinLocation);

                    // set new pin location
                    unitIndependent.SetPinPoint(ptPinLocation[0], units);
                }

                if (this.Model != null)
                {
                    this.Model.EndUpdate();
                    //this.Model.LinkManager.EndSynchronization();
                    //this.Model.BridgeManager.EndUpdateIntersection();
                    this.Model.HistoryManager.EndAtomicAction();
                }
            }

            return success;
        }

        /// <summary>
        /// Nudge the selected components up by
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.NudgeIncrement"/>
        /// units.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool NudgeUp()
        {
            return MoveNodes(GetSelectionList(), 0f, -nudgeIncrement, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Nudge the selected components down by
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.NudgeIncrement"/>
        /// units.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool NudgeDown()
        {
            return MoveNodes(GetSelectionList(), 0f, nudgeIncrement, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Nudge the selected components left by
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.NudgeIncrement"/>
        /// units.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool NudgeLeft()
        {
            return MoveNodes(GetSelectionList(), -nudgeIncrement, 0f, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Nudge the selected components right by
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.NudgeIncrement"/>
        /// units.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool NudgeRight()
        {
            return MoveNodes(GetSelectionList(), nudgeIncrement, 0f, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Rotates the selected nodes about their local origin by the specified
        /// number of degrees.
        /// </summary>
        /// <param name="degrees">Number of degrees to rotate.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Rotate(float degrees)
        {
            if (this.Model != null)
                this.Model.HistoryManager.StartAtomicAction("Rotate Nodes");

            foreach (Node node in GetSelectionList())
            {
                node.RotationAngle += degrees;
            }

            if (this.Model != null)
                this.Model.HistoryManager.EndAtomicAction();

            return true;
        }

        /// <summary>
        /// Flips the selected nodes about their horizontal (X) axis.
        /// </summary>
        public void FlipHorizontal()
        {
            if (this.Model != null)
                this.Model.HistoryManager.StartAtomicAction("FlipX");

            foreach (Node node in GetSelectionList())
            {
                node.FlipX = !node.FlipX;
            }

            if (this.Model != null)
                this.Model.HistoryManager.EndAtomicAction();
        }

        /// <summary>
        /// Gets the selection list.
        /// </summary>
        /// <returns>The nodes in the selection.</returns>
        protected NodeCollection GetSelectionList()
        {
            return (this.Controller.Model.EnableSelectionListSubstitute && this.View.SelectionList.Count > 1)
                ? this.Controller.View.SelectionListSubstitute : this.Controller.View.SelectionList;
        }

        /// <summary>
        /// Flips the selected nodes about their vertical (Y) axis.
        /// </summary>
        public void FlipVertical()
        {
            if (this.Model != null)
                this.Model.HistoryManager.StartAtomicAction("FlipY");

            foreach (Node node in GetSelectionList())
            {
                node.FlipY = !node.FlipY;
            }

            if (this.Model != null)
                this.Model.HistoryManager.EndAtomicAction();
        }

        /// <summary>
        /// Aligns the selected nodes along the left edge of the first node.
        /// </summary>
        public void AlignLeft()
        {
            Align(BoxOrientation.Left);
        }

        /// <summary>
        /// Aligns the selected nodes along the right edge of the first node.
        /// </summary>
        public void AlignRight()
        {
            Align(BoxOrientation.Right);
        }

        /// <summary>
        /// Aligns the selected nodes along the top edge of the first node.
        /// </summary>
        public void AlignTop()
        {
            Align(BoxOrientation.Top);
        }

        /// <summary>
        /// Aligns the selected nodes along the bottom edge of the first node.
        /// </summary>
        public void AlignBottom()
        {
            Align(BoxOrientation.Bottom);
        }

        /// <summary>
        /// Aligns the selected nodes along the horizontal center of the first node.
        /// </summary>
        public void AlignMiddle()
        {
            Align(BoxOrientation.VCenter);
        }

        /// <summary>
        /// Aligns the selected nodes along the vertical center of the first node.
        /// </summary>
        public void AlignCenter()
        {
            Align(BoxOrientation.HCenter);
        }

        /// <summary>
        /// Positions the selected nodes for equal horizontal spacing.
        /// </summary>
        public void SpaceAcross()
        {
            Spacing(SpacingDirection.Across);
        }

        /// <summary>
        /// Positions the selected nodes for equal vertical spacing
        /// </summary>
        public void SpaceDown()
        {
            Spacing(SpacingDirection.Down);
        }

        /// <summary>
        /// Sets the width of the selected nodes to be equal.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool SameWidth()
        {
            bool bSuccess = false;

            if (this.Controller.SelectionList != null && this.Controller.SelectionList.Count > 1)
            {
                SizeF szNodeSize;
                Node firstNode = this.Controller.SelectionList.First;
                MeasureUnits units = MeasureUnits.Pixel;

                if (firstNode != null)
                {
                    SizeF szSize = ((IUnitIndependent)firstNode).GetSize(units);

                    this.Model.HistoryManager.StartAtomicAction("Set Same Width");

                    foreach (Node node in this.Controller.SelectionList)
                    {
                        if (node != firstNode)
                        {
                            szNodeSize = ((IUnitIndependent)node).GetSize(units);
                            szNodeSize.Width = szSize.Width;

                            ((IUnitIndependent)node).SetSize(szNodeSize, units);
                            bSuccess = true;
                        }
                    }

                    this.Model.HistoryManager.EndAtomicAction();
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Sets the height of the selected nodes to be equal.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool SameHeight()
        {
            bool bSuccess = false;

            if (this.Controller.SelectionList != null && this.Controller.SelectionList.Count > 1)
            {
                SizeF szNodeSize;
                Node firstNode = this.Controller.SelectionList.First;
                MeasureUnits units = MeasureUnits.Pixel;

                if (firstNode != null)
                {
                    SizeF szSize = ((IUnitIndependent)firstNode).GetSize(units);

                    this.Model.HistoryManager.StartAtomicAction("Set Same Heigth");

                    foreach (Node node in this.Controller.SelectionList)
                    {
                        if (!node.Equals(firstNode))
                        {
                            szNodeSize = ((IUnitIndependent)node).GetSize(units);
                            szNodeSize.Height = szSize.Height;

                            ((IUnitIndependent)node).SetSize(szNodeSize, units);
                            bSuccess = true;
                        }
                    }

                    this.Model.HistoryManager.EndAtomicAction();
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Sets the width and height of the selected nodes to be equal.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool SameSize()
        {
            bool bSuccess = false;

            if (this.Controller.SelectionList != null && this.Controller.SelectionList.Count > 1)
            {
                Node firstNode = this.Controller.SelectionList.First;
                MeasureUnits units = MeasureUnits.Pixel;

                if (firstNode != null)
                {
                    SizeF szSize = ((IUnitIndependent)firstNode).GetSize(units);

                    this.Model.HistoryManager.StartAtomicAction("Set Same Size");

                    foreach (Node node in this.Controller.SelectionList)
                    {
                        if (node != firstNode)
                        {
                            ((IUnitIndependent)node).SetSize(szSize, units);
                            bSuccess = true;
                        }
                    }

                    this.Model.HistoryManager.EndAtomicAction();
                }
            }

            return bSuccess;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Performs zooming either based on the mouse point or by the center of the view. 
        /// </summary>
        private void CenterBasedZooming()
        {
            PointF ptMouse;
            float fWidthOffset, fHeightOffset;
            float fMagnification = this.Controller.Viewer.Magnification / 100f;
            int nRuler = this.ShowRulers ? this.RulersHeight : 0;

            if (m_bMouseWheelZoom)
            {
                //Zooming based on the mouse position
                ptMouse = this.Controller.ConvertFromModelToClientCoordinates(this.Controller.MouseLocation);
                fWidthOffset = (ptMouse.X - m_ptCurMouse.X) / fMagnification + this.View.Origin.X;
                fHeightOffset = (ptMouse.Y - m_ptCurMouse.Y) / fMagnification + this.View.Origin.Y;
            }
            else
            {
                //Zooming based on the center of the view
                PointF ptCenter = new PointF( this.ClientRectangle.Size.Width / 2, this.ClientRectangle.Size.Height / 2);
                ptMouse = this.Controller.ConvertFromModelToClientCoordinates(new PointF(m_ptOrigin.X + this.ClientRectangle.Size.Width / 2, m_ptOrigin.Y + this.ClientRectangle.Size.Height / 2)); ;
                fWidthOffset = (ptMouse.X - ptCenter.X) / fMagnification + this.View.Origin.X - nRuler;
                fHeightOffset = (ptMouse.Y - ptCenter.Y) / fMagnification + this.View.Origin.Y - nRuler; 
            }  

            //sets the new origin value to the view's origin.
            this.View.Origin = new PointF(fWidthOffset, fHeightOffset);
          
            // lock update scroll origin
            m_bLockScrollUpdate = true;
            // update scroll minimum and maximum value
            UpdateScrollRange();
            // Unlock update scroll origin
            m_bLockScrollUpdate = false;
            // update scroll position to origin
            UpdateScrolls(this.View.Origin);

            m_bMouseWheelZoom = false;
        }
        #endregion

        #region Clipboard

        /// <summary>
        /// Remove the currently selected nodes from the diagram and move
        /// them to the clipboard.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.Cut"/>.
        /// </remarks>
        public void Cut()
        {
            if (this.m_controller != null)
            {
                this.m_controller.Cut();
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are any selected nodes that can be removed from the
        /// the model.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.CanCut"/>.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool CanCut
        {
            get
            {
                if (this.m_controller != null)
                {
                    return this.m_controller.TextEditor.CurrentText.Length > 0 || this.m_controller.CanCut;
                }
                return false;
            }
        }

        /// <summary>
        /// Copy the currently selected nodes to the clipboard.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.Copy"/>.
        /// </remarks>
        public void Copy()
        {
            if (this.m_controller != null)
            {
                this.m_controller.Copy();
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are any selected nodes that can be copied to the clipboard.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.CanCopy"/>.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool CanCopy
        {
            get
            {
                if (this.m_controller != null)
                {
                    return this.m_controller.CanCopy;
                }
                return false;
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard to the diagram.
        /// </summary>
        public void Paste()
        {
            if (this.m_controller != null)
            {
                this.m_controller.Paste();
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard to the diagram at the specified layer and location.
        /// </summary>
        /// <param name="layername">The layername.</param>
        /// <param name="location">The location.</param>
        public void Paste(string layername, PointF location)
        {
            if (this.m_controller != null)
            {
                this.m_controller.Paste(layername, location);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is any data in the clipboard that can be pasted
        /// into the model.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.CanPaste"/>.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool CanPaste
        {
            get
            {
                if (this.m_controller != null)
                {
                    return this.m_controller.CanPaste;
                }
                return false;
            }
        }

        #endregion

        #region Selection
        /// <summary>
        /// Adds all nodes in the model to the SelectionList.
        /// </summary>
        /// <remarks>
        /// Wrapper for
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectAll"/>.
        /// </remarks>
        public void SelectAll()
        {
            if (this.m_controller != null)
            {
                this.m_controller.SelectAll();
            }
        }
        #endregion

        #region Serialization
		
		/// <summary>
        /// Loads the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void Load(string fileName, out Exception exception)
        {
           exception = null;
           
           //if (Path.GetExtension(fileName) == ".edd" || Path.GetExtension(fileName) == ".xml")
           //{
               try
               {
                   FileStream iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                   LoadBinary(iStream, out exception);
                   if (exception != null)
                       LoadSoap(iStream, out exception);
               }
               catch (Exception ex)
               {
                   exception = ex;
               }
           //}
           //else
           //    exception = new Exception("The format is not supported. Please load a diagram file of EDD | XML format.");
           
        }
		
        /// <summary>
        /// Loads the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public virtual void Load(string fileName)
        {
            DiagramDocument doc = null;
            FileStream iStream;

            if (File.Exists(fileName))
            {
                iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                doc = GetDocumentFromStream(iStream);
            }

            if (doc != null)
            {
                InitDiagram(doc);
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="strm">The stream.</param>
        public virtual void Load(Stream strm)
        {
            if (strm == null)
            {
                throw new ArgumentNullException("strm");
            }

            DiagramDocument doc = GetDocumentFromStream(strm);

            if (doc != null)
            {
                InitDiagram(doc);
            }
        }

		/// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="strm">The stream.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void Load(Stream strm, out Exception exception)
        {
            exception = null;
            LoadBinary(strm, out exception);
            if (exception != null)
                LoadSoap(strm, out exception);          
        }
		
        /// <summary>
        /// Saves the diagram to a stream in SOAP format.
        /// </summary>
        /// <param name="strmOut">Stream to serialize the diagram into.</param>
        public virtual void SaveSoap(Stream strmOut)
        {
            SoapFormatter formatter = new SoapFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            formatter.FilterLevel = TypeFilterLevel.Low;
            DiagramDocument doc = new DiagramDocument(m_model, m_view);
            formatter.Serialize(strmOut, doc);
        }

        /// <summary>
        /// Saves the diagram to a file in SOAP format.
        /// </summary>
        /// <param name="fileName">Name of file to save to.</param>
        public virtual void SaveSoap(string fileName)
        {
            FileStream oStream = new FileStream(fileName, FileMode.Create);
            SaveSoap(oStream);
            oStream.Close();
        }

        /// <summary>
        /// Loads the diagram from a stream in SOAP format.
        /// </summary>
        /// <param name="strmIn">Stream to serialize the diagram into.</param>
        public virtual void LoadSoap(Stream strmIn)
        {
            MVCDispose();
            DiagramDocument tempDoc = new DiagramDocument(m_model, m_view);
            SoapFormatter formatter = new SoapFormatter();
            DiagramDocument doc = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                doc = (DiagramDocument)formatter.Deserialize(strmIn);
            }
            catch (SerializationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
            }

            if (doc != null)
            {
                InitDiagram(doc);
            }
            else
            {
                InitDiagram(tempDoc);
            }

        }

		/// <summary>
        /// Loads the diagram from a stream in SOAP format.
        /// </summary>
        /// <param name="strmIn">Stream to serialize the diagram into.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void LoadSoap(Stream strmIn, out Exception exception)
        {
            exception = null;
            DiagramDocument diagramDoc = new DiagramDocument(m_model, m_view);
            MVCDispose();           
            SoapFormatter formatter = new SoapFormatter();
            DiagramDocument doc = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                doc = (DiagramDocument)formatter.Deserialize(strmIn);
            }
            catch (Exception ex)
            {
                doc = diagramDoc;
                exception = ex;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
            }

            if (doc != null)
            {
                InitDiagram(doc);
            }        
        }
		
        /// <summary>
        /// Loads the diagram from a file in SOAP format.
        /// </summary>
        /// <param name="fileName">Name of file to load from.</param>
        public virtual void LoadSoap(string fileName)
        {
            FileStream iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            LoadSoap(iStream);
            iStream.Close();
        }
		
		/// <summary>
        /// Loads the diagram from a file in SOAP format.
        /// </summary>
        /// <param name="fileName">Name of file to load from.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void LoadSoap(string fileName, out Exception exception)
        {
            exception = null;
            //if (Path.GetExtension(fileName) == ".xml")
            //{
                try
                {
                    FileStream iStream = new FileStream(fileName, FileMode.Open,FileAccess.Read);
                    LoadSoap(iStream, out exception);
                    iStream.Close();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            //}
            //else
            //    exception = new Exception("The format is not supported. Please load a diagram file of EDD | XML format.");
        }
		
        /// <summary>
        /// Saves the diagram to a stream in binary format.
        /// </summary>
        /// <param name="strmOut">Stream to serialize the diagram into.</param>
        public virtual void SaveBinary(Stream strmOut)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.TypeFormat = FormatterTypeStyle.TypesAlways;
            DiagramDocument doc = new DiagramDocument(m_model, m_view);
            formatter.Serialize(strmOut, doc);
        }

        /// <summary>
        /// Saves the diagram to a file in binary format.
        /// </summary>
        /// <param name="fileName">Name of file to save to.</param>
        public virtual void SaveBinary(string fileName)
        {
            FileStream oStream = new FileStream(fileName, FileMode.OpenOrCreate);
            SaveBinary(oStream);
            oStream.Close();
        }

        /// <summary>
        /// Loads the diagram from a stream in binary format.
        /// </summary>
        /// <param name="strmIn">Stream to serialize the diagram from.</param>
        public virtual void LoadBinary(Stream strmIn)
        {
			DiagramDocument diagramDoc = new DiagramDocument(m_model, m_view);
            MVCDispose();

            BinaryFormatter formatter = new BinaryFormatter();
            DiagramDocument doc = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                formatter.TypeFormat = FormatterTypeStyle.TypesAlways;
                formatter.Binder = new OldToNewDeserializationBinder();
                doc = (DiagramDocument)formatter.Deserialize(strmIn);
            }
            catch (SerializationException ex)
            {
				doc = diagramDoc;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
            }

            if (doc != null)
            {
                InitDiagram(doc);
            }
        }

		/// <summary>
        /// Loads the diagram from a stream in binary format.
        /// </summary>
        /// <param name="strmIn">Stream to serialize the diagram from.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void LoadBinary(Stream strmIn, out Exception exception)
        {
            exception = null;
            DiagramDocument diagramDoc = new DiagramDocument(m_model, m_view);
            MVCDispose();           
            BinaryFormatter formatter = new BinaryFormatter();
            DiagramDocument doc = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                formatter.TypeFormat = FormatterTypeStyle.TypesAlways;
                formatter.Binder = new OldToNewDeserializationBinder();
                doc = (DiagramDocument)formatter.Deserialize(strmIn);
            }
            catch (Exception ex)
            {
                exception = ex;
                doc = diagramDoc;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
            }

            if (doc != null)
            {
                InitDiagram(doc);
            }
        }
		
        /// <summary>
        /// Loads the diagram from a file in binary format.
        /// </summary>
        /// <param name="fileName">Name of file to load from.</param>
        public virtual void LoadBinary(string fileName)
        {
            FileStream iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            LoadBinary(iStream);
            iStream.Close();
        }
		
		/// <summary>
        /// Loads the diagram from a file in binary format.
        /// </summary>
        /// <param name="fileName">Name of file to load from.</param>
        /// <param name="exception">Get the exception during deserialization if exception occurs</param>
        public virtual void LoadBinary(string fileName, out Exception exception)
        {
            exception = null;
            //if (Path.GetExtension(fileName) == ".edd")
            //{
                try
                {
                    FileStream iStream = new FileStream(fileName, FileMode.Open,FileAccess.Read);
                    LoadBinary(iStream, out exception);
                    iStream.Close();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            //}
            //else
            //    exception = new Exception("The format is not supported. Please load a diagram file of EDD | XML format.");
        }		
        #endregion

        /// <summary>
        /// Exports a representation of the Diagram as a image.
        /// </summary>
        /// <param name="bClipModelBounds">Indicates whether image will be clipped by model bounds.</param>
        /// <returns>Diagram representation as Image</returns>
        public virtual Image ExportDiagramAsImage(bool bClipModelBounds)
        {
            Image imgDiagram = null;

            if ((this.Model != null) && (this.View != null))
            {
                this.UnhookViewEvents();
                imgDiagram = this.View.ExportDiagramAsImage(bClipModelBounds);
                this.HookViewEvents();
            }
            return imgDiagram;
        }

        /// <summary>
        /// Renders a representation of the Diagram onto the provided <see cref="System.Drawing.Graphics"/> object.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        public virtual void ExportDiagramToGraphics(Graphics grfx)
        {
            if ((this.Model != null) && (this.View != null))
            {
                this.UnhookViewEvents();
                this.View.ExportDiagramToGraphics(grfx);
                this.HookViewEvents();
            }
        }

        /// <summary>
        /// Renders a representation of the Diagram onto the provided <see cref="System.Drawing.Graphics"/>
        /// object to specified rectangle.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="rcArea">The area.</param>
        public virtual void ExportDiagramToGraphics(Graphics grfx, RectangleF rcArea)
        {
            if ((this.Model != null) && (this.View != null))
            {
                this.UnhookViewEvents();
                this.View.ExportDiagramToGraphics(grfx, rcArea);
                this.HookViewEvents();
            }
        }

        #region Printing

        /// <summary>
        /// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Diagram.DiagramPrintDocument"/> and initializes it
        /// with the diagram's view.
        /// </summary>
        /// <returns>Diagram print document</returns>
        public virtual DiagramPrintDocument CreatePrintDocument()
        {
            DiagramPrintDocument printDoc = new DiagramPrintDocument(this.View);

            printDoc.DefaultPageSettings = this.View.PageSettings;
            printDoc.PrinterSettings = (System.Drawing.Printing.PrinterSettings)this.View.PageSettings.PrinterSettings.Clone();

            return printDoc;
        }
        #endregion

        #region View Event Handlers
        /// <summary>
        /// Raises the <see cref="E:ViewMagnificationChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewMagnificationEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        private void OnViewMagnificationChanged(ViewMagnificationEventArgs evtArgs)
        {
            m_bMagnificationChanged = true;
            if (this.View.ZoomType == ZoomType.TopLeft)
            {                
                PointF ptOrigin = this.Origin;

                // increment vistual spaces to magfinication factor
                float fMagnification = evtArgs.OriginalMagnification / evtArgs.NewMagnification;

                m_scrollBounds = new RectangleF(
                        ScrollVirtualBounds.X * fMagnification,
                        ScrollVirtualBounds.Y * fMagnification,
                        ScrollVirtualBounds.Width * fMagnification,
                        ScrollVirtualBounds.Height * fMagnification);

                ScrollVirtualBounds = m_scrollBounds;
                // lock update scroll origin
                m_bLockScrollUpdate = true;

                // update scroll minimum and maximum value
                UpdateScrollRange();
                m_bLockScrollUpdate = false;

                // update scroll position to origin by change ptScrollOriginRef value
                UpdateScrolls(ptOrigin);

                SizeF szUnVisibleSize = GetUnvisibleSize();

                float fHScrollMax = szUnVisibleSize.Width + this.ScrollVirtualBounds.Right;
                float fVScrollMax = szUnVisibleSize.Height + this.ScrollVirtualBounds.Bottom;

                // calc max scroll value
                double dHMaxValue = fHScrollMax * this.ScrollGranularity;
                double dVMaxValue = fVScrollMax * this.ScrollGranularity;

                // update origin to new max and min values
                if (this.HScrollBar.Maximum > 0 && dHMaxValue < this.HScrollBar.Value)
                {
                    float fValue = (float)Math.Ceiling(dHMaxValue);
                    fValue = Math.Max(this.HScrollBar.Minimum, fValue);
                    fValue = Math.Min(this.HScrollBar.Maximum, fValue);

                    this.HScrollBar.Value = (int)fValue;
                    UpdateOrigin(new PointF(fValue, 0), ScrollBars.Horizontal);
                }

                if (this.VScrollBar.Maximum > 0 && dVMaxValue < this.VScrollBar.Value)
                {
                    float fValue = (float)Math.Ceiling(dVMaxValue);
                    fValue = Math.Max(this.VScrollBar.Minimum, fValue);
                    fValue = Math.Min(this.VScrollBar.Maximum, fValue);

                    this.VScrollBar.Value = (int)fValue;
                    UpdateOrigin(new PointF(0, fValue), ScrollBars.Vertical);
                }                
            }
            else
            {
                CenterBasedZooming();                
            }            
            // invalidate whole client area
            this.Controller.UpdateInfo.UpdateRefreshRect(this.ClientRectangle);
            UpdateClientArea();
            this.UpdateView();
            m_bMagnificationChanged = false;
        }
        
        [EventHandlerPriority(true)]
        private void EventSink_ScrollVirtualBoundsChanged(ViewScrollVirtualBoundsEventArgs evtArgs)
        {
            UpdateScrollRange();

            PointF ptScrollPosition = ConvertToScrollRanges(this.Origin);
            PointF ptTemp = ptScrollPosition;

            ptTemp.X = Math.Min(this.HScrollBar.Maximum, Math.Max(ptScrollPosition.X, this.HScrollBar.Minimum));
            ptTemp.Y = Math.Min(this.VScrollBar.Maximum, Math.Max(ptScrollPosition.Y, this.VScrollBar.Minimum));

            if (ptTemp != ptScrollPosition)
                UpdateOrigin(ptTemp, ScrollBars.Both);

            // reset origin Reference to update scroll value
            this.ptScrollOriginRef = new PointF(this.Origin.X - 1, 0);
            
            // update scroll position to origin
            UpdateScrolls(this.Origin);
        }

        /// <summary>
        /// Raises the <see cref="E:ViewOriginChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected virtual void OnViewOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // update scroll value to new origin
            UpdateScrolls(evtArgs.NewOrigin);
            UpdateScrollRange();

            if (this.Controller.ActiveTool is PanTool && !m_bMagnificationChanged)
                m_ptOrigin = evtArgs.NewOrigin;

            // invalidate whole client area
            this.Controller.UpdateInfo.UpdateRefreshRect(this.ClientRectangle);

            UpdateClientArea();
        }

        /// <summary>
        /// Raises the <see cref="E:ViewPropertyChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected virtual void OnViewPropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            // invalidate whole client area
            Invalidate(this.ClientRectangle);

            UpdateClientArea();
        }
        
        [EventHandlerPriority(true)]
        private void EventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (this.Controller.CanUpdateView)
            {
                // invalidate whole client area
                Invalidate(this.ClientRectangle);

                UpdateClientArea();
            }
            else if (evtArgs.NodeAffected is Model &&
                !(evtArgs.PropertyName == DPN.EnableSelectionListSubstitute
                || evtArgs.PropertyName == DPN.BoundaryConstraintsEnabled))
            {
                // update client area on Model property change (!BoundaryConstrainsEnabled && !EnableSelectionListSubstitution)
                this.Controller.UpdateInfo.UpdateRefreshRect(this.ClientRectangle);
            }
        }

        /// <summary>
        /// Events the sink_ size changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.SizeChangedEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        private void EventSink_SizeChanged(SizeChangedEventArgs evtArgs)
        {
            // update scroll position
            UpdateScrolls(this.Origin);

            if (this.Controller.CanUpdateView)
            {
                // invalidate whole client area
                Invalidate(this.ClientRectangle);

                UpdateClientArea();
            }
        }

        /// <summary>
        /// Fires when node collection is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Fires when node is rotated.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.RotationChangedEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_RotationChanged(RotationChangedEventArgs evtArgs)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Fires when pin offset is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinOffsetChangedEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_PinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Fires when pin point is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinPointChangedEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_PinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Handles the BridgeGenerationCompleted event of the BridgeManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        private void BridgeManager_BridgeGenerationCompleted(object sender, EventArgs e)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Handles the SynhronizeCompleted event of the LinkManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        private void LinkManager_SynhronizeCompleted(object sender, EventArgs e)
        {
            if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Handles the DocumentEndUpdate event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_DocumentEndUpdate(object sender, EventArgs e)
        {
            if (this.Model != null)
                if (!this.Model.InUpdate && !this.Model.LinkManager.IsSynchronizing && !this.Model.BridgeManager.Generating)
                    UpdateScrollBoundsToContent();
        }

        /// <summary>
        /// Handles the DocumentBeginUpdate event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriority(true)]
        protected void EventSink_DocumentBeginUpdate(object sender, EventArgs e)
        { 
        }
        #endregion

        #region Layout Manager Event Handlers
        /// <summary>
        /// Called after the layout manager updates the model.
        /// </summary>
        /// <param name="sender">Layout manager sending the event.</param>
        /// <param name="evtArgs">Event arguments.</param>
        protected virtual void OnLayoutUpdated(object sender, EventArgs evtArgs)
        {
            UpdateView();
        }

        #endregion

        #region ContextMenu
        private void InitializeDefaultContextMenu()
        {            
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem menu, subMenu;
            System.Drawing.Image image;
            //access to the resource Images
            System.Resources.ResourceManager imagesMgr = new System.Resources.ResourceManager("Syncfusion.Diagram.Base.Resources.Images", typeof(Resources).Assembly);
            
            //Tools menu
            image = (System.Drawing.Image)imagesMgr.GetObject("SelectTool");
            menu = new ToolStripMenuItem("Pointer", image, DefaultContextMenu_Click, Keys.Control | Keys.Shift | Keys.P);
            menu.Name = "Pointer";
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("PanTool");
            menu = new ToolStripMenuItem("Pan", image, DefaultContextMenu_Click, "Pan");
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("ZoomTool");
            menu = new ToolStripMenuItem("Zoom", image, DefaultContextMenu_Click, "Zoom");
            contextMenu.Items.Add(menu);

            //Connector tools menu
            menu = new ToolStripMenuItem("Connectors",null,DefaultContextMenu_Click, "Connectors");
            image = (System.Drawing.Image)imagesMgr.GetObject("LineLinkTool");
            subMenu = new ToolStripMenuItem("Line", image, DefaultContextMenu_Click, "Line");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("DirectedLineLinkTool");
            subMenu = new ToolStripMenuItem("Directed Line", image, DefaultContextMenu_Click, "Directed Line");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("OrthogonalLinkTool");
            subMenu = new ToolStripMenuItem("Orthogonal", image, DefaultContextMenu_Click, "Orthogonal");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("OrgLineConnectorTool");
            subMenu = new ToolStripMenuItem("Orgline", image, DefaultContextMenu_Click, "Orgline");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("PolyLineLinkTool");
            subMenu = new ToolStripMenuItem("Polyline", image, DefaultContextMenu_Click, "Polyline");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SplineTool");
            subMenu = new ToolStripMenuItem("Spline", image, DefaultContextMenu_Click, "Spline");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("BezierTool");
            subMenu = new ToolStripMenuItem("Bezier", image, DefaultContextMenu_Click, "Bezier");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);
            
            //Shapes tools menu
            menu = new ToolStripMenuItem("Shapes");
            menu.Name = "Shapes";
            image = (System.Drawing.Image)imagesMgr.GetObject("PencilTool");
            subMenu = new ToolStripMenuItem("Pencil", image, DefaultContextMenu_Click, "Pencil");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("LineTool");
            subMenu = new ToolStripMenuItem("Line", image, DefaultContextMenu_Click, "Line");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("PolyLineTool");
            subMenu = new ToolStripMenuItem("Polyline", image, DefaultContextMenu_Click, "Polyline");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("CurveTool");
            subMenu = new ToolStripMenuItem("Curve", image, DefaultContextMenu_Click, "Curve");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("ClosedCurveTool");
            subMenu = new ToolStripMenuItem("Closed Curve", image, DefaultContextMenu_Click, "Closed Curve");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("RectangleTool");
            subMenu = new ToolStripMenuItem("Rectangle", image, DefaultContextMenu_Click, "Rectangle");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("RoundRectangleTool");
            subMenu = new ToolStripMenuItem("Round Rectangle", image, DefaultContextMenu_Click, "Round Rectangle");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("EllipseTool");
            subMenu = new ToolStripMenuItem("Ellipse", image, DefaultContextMenu_Click, "Ellipse");
            menu.DropDownItems.Add(subMenu);    
            image = (System.Drawing.Image)imagesMgr.GetObject("PolygonTool");
            subMenu = new ToolStripMenuItem("Polygon", image, DefaultContextMenu_Click, "Polygon");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("BitmapTool");
            subMenu = new ToolStripMenuItem("Bitmap", image, DefaultContextMenu_Click, "Bitmap");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("TextTool");
            subMenu = new ToolStripMenuItem("Text", image, DefaultContextMenu_Click, "Text");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("RichTextTool");
            subMenu = new ToolStripMenuItem("Rich Text", image, DefaultContextMenu_Click, "Rich Text");
            menu.DropDownItems.Add(subMenu);   
            contextMenu.Items.Add(menu);

            //separator
            contextMenu.Items.Add(new ToolStripSeparator());

            menu = new ToolStripMenuItem("Ruler", null, DefaultContextMenu_Click, "Ruler");            
            menu.CheckOnClick = true;
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Document Size", null, DefaultContextMenu_Click, "Document Size");
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Fit to Drawing", null, DefaultContextMenu_Click, "Fit to Drawing");
            menu.CheckOnClick = true;
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Boundary Constraints", null, DefaultContextMenu_Click, "Boundary Constraints");
            menu.CheckOnClick = true;
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Background Style", null, DefaultContextMenu_Click, "Background Style");
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Layout");
            menu = new ToolStripMenuItem("Layout", image, DefaultContextMenu_Click, "Layout");
            contextMenu.Items.Add(menu);

            //separator
            contextMenu.Items.Add(new ToolStripSeparator());

            //File menus
            menu = new ToolStripMenuItem("File", null, DefaultContextMenu_Click, "File");
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("New");
            subMenu = new ToolStripMenuItem("New", image, DefaultContextMenu_Click, Keys.Control | Keys.N);
            subMenu.Name = "New";
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Open");
            subMenu = new ToolStripMenuItem("Open", image, DefaultContextMenu_Click, Keys.Control | Keys.O);
            subMenu.Name = "Open";
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Save");
            subMenu = new ToolStripMenuItem("Save", image, DefaultContextMenu_Click, Keys.Control | Keys.S);
            subMenu.Name = "Save";
            menu.DropDownItems.Add(subMenu);
            subMenu = new ToolStripMenuItem("Save As", null, DefaultContextMenu_Click);
            subMenu.Name = "Save As";
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("PageSetup");
            subMenu = new ToolStripMenuItem("PageSetup", image, DefaultContextMenu_Click, "PageSetup");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("PrintPreview");
            subMenu = new ToolStripMenuItem("Print Preview", image, DefaultContextMenu_Click, "Print Preview");
            menu.DropDownItems.Add(subMenu);            
            image = (System.Drawing.Image)imagesMgr.GetObject("Print");
            subMenu = new ToolStripMenuItem("Print", image, DefaultContextMenu_Click, Keys.Control | Keys.P);
            subMenu.Name = "Print";
            menu.DropDownItems.Add(subMenu);         
            contextMenu.Items.Add(menu);

            //Edit menus
            image = (System.Drawing.Image)imagesMgr.GetObject("Cut");
            menu = new ToolStripMenuItem("Cut", image, DefaultContextMenu_Click,Keys.Control | Keys.X);
            menu.Name = "Cut";
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Copy");
            menu = new ToolStripMenuItem("Copy", image, DefaultContextMenu_Click, Keys.Control | Keys.C);
            menu.Name = "Copy";
            contextMenu.Items.Add(menu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Paste");
            menu = new ToolStripMenuItem("Paste", image, DefaultContextMenu_Click, Keys.Control | Keys.V);
            menu.Name = "Paste";
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Delete", null, DefaultContextMenu_Click, Keys.Delete);
            menu.Name = "Delete";
            contextMenu.Items.Add(menu);
            menu = new ToolStripMenuItem("Select All", null, DefaultContextMenu_Click,Keys.Control | Keys.A);
            menu.Name = "Select All";
            contextMenu.Items.Add(menu);

            //separator
            contextMenu.Items.Add(new ToolStripSeparator());

            //Align menus
            menu = new ToolStripMenuItem("Align", null, DefaultContextMenu_Click, "align");            
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignLeft");
            subMenu = new ToolStripMenuItem("Align Left", image, DefaultContextMenu_Click, "Align Left");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignCenter");
            subMenu = new ToolStripMenuItem("Align Center", image, DefaultContextMenu_Click, "Align Center");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignRight");
            subMenu = new ToolStripMenuItem("Align Right", image, DefaultContextMenu_Click, "Align Right");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignTop");
            subMenu = new ToolStripMenuItem("Align Top", image, DefaultContextMenu_Click, "Align Top");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignMiddle");
            subMenu = new ToolStripMenuItem("Align Middle", image, DefaultContextMenu_Click, "Align Middle");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("AlignBottom");
            subMenu = new ToolStripMenuItem("Align Bottom", image, DefaultContextMenu_Click, "Align Bottom");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);

            //Flip menus
            menu = new ToolStripMenuItem("Flip", null, DefaultContextMenu_Click, "Flip");            
            image = (System.Drawing.Image)imagesMgr.GetObject("FlipHorizontal");
            subMenu = new ToolStripMenuItem("Flip Horizontally", image, DefaultContextMenu_Click, "Flip Horizontally");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("FlipVertical");
            subMenu = new ToolStripMenuItem("Flip Vertically", image, DefaultContextMenu_Click, "Flip Vertically");
            menu.DropDownItems.Add(subMenu);
            subMenu = new ToolStripMenuItem("Flip Both", null, DefaultContextMenu_Click, "Flip Both");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);

            //Grouping menus
            menu = new ToolStripMenuItem("Grouping", null, DefaultContextMenu_Click, "Grouping");            
            image = (System.Drawing.Image)imagesMgr.GetObject("Group");
            subMenu = new ToolStripMenuItem("Group", image, DefaultContextMenu_Click, "Group");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("Ungroup");
            subMenu = new ToolStripMenuItem("Ungroup", image, DefaultContextMenu_Click, "Ungroup");
            menu.DropDownItems.Add(subMenu);  
            contextMenu.Items.Add(menu);

            //Order menus
            menu = new ToolStripMenuItem("Order", null, DefaultContextMenu_Click, "Order");            
            image = (System.Drawing.Image)imagesMgr.GetObject("BringToFront");
            subMenu = new ToolStripMenuItem("Bring To Front", image, DefaultContextMenu_Click, "Bring To Front");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("BringForward");
            subMenu = new ToolStripMenuItem("Bring Forward", image, DefaultContextMenu_Click, "Bring Forward");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SendBackward");
            subMenu = new ToolStripMenuItem("Send Backward", image, DefaultContextMenu_Click, "Send Backward");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SendToBack");
            subMenu = new ToolStripMenuItem("Send To Back", image, DefaultContextMenu_Click, "Send To Back");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);

            //Rotate menus
            menu = new ToolStripMenuItem("Rotate", null, DefaultContextMenu_Click, "Rotate");            
            image = (System.Drawing.Image)imagesMgr.GetObject("RotateRight");
            subMenu = new ToolStripMenuItem("Rotate Right", image, DefaultContextMenu_Click, "Rotate Right");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("RotateLeft");
            subMenu = new ToolStripMenuItem("Rotate Left", image, DefaultContextMenu_Click, "Rotate Left");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);

            //Resize menus
            menu = new ToolStripMenuItem("Resize", null, DefaultContextMenu_Click, "Resize");            
            image = (System.Drawing.Image)imagesMgr.GetObject("SameWidth");
            subMenu = new ToolStripMenuItem("Same Width", image, DefaultContextMenu_Click, "Same Width");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SameHeight");
            subMenu = new ToolStripMenuItem("Same Height", image, DefaultContextMenu_Click, "Same Height");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SameSize");
            subMenu = new ToolStripMenuItem("Same Size", image, DefaultContextMenu_Click, "Same Size");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SpaceAcross");
            subMenu = new ToolStripMenuItem("Space Across", image, DefaultContextMenu_Click, "Space Across");
            menu.DropDownItems.Add(subMenu);
            image = (System.Drawing.Image)imagesMgr.GetObject("SpaceDown");
            subMenu = new ToolStripMenuItem("Space Down", image, DefaultContextMenu_Click, "Space Down");
            menu.DropDownItems.Add(subMenu);
            contextMenu.Items.Add(menu);

            this.ContextMenuStrip = contextMenu;
            imagesMgr.ReleaseAllResources();
        }

        /// <summary>
        /// Called when a context menustrip item is clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="evtArgs">Event arguments.</param>
        private void DefaultContextMenu_Click(object sender, EventArgs evtArgs)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            switch (menuItem.Name)
            {               
                case "Pointer":
                    this.Controller.ActivateTool("SelectTool");
                    Cursor.Current = Cursors.Arrow;
                    this.Refresh();
                    break;
                case "Pan":
                    this.Controller.ActivateTool("PanTool");
                    break;
                case "Zoom":
                    this.Controller.ActivateTool("ZoomTool");
                    break;
                case "Line":
                    if (menuItem.OwnerItem.Name == "Connectors")
                        this.Controller.ActivateTool("LineLinkTool");
                    else
                        this.Controller.ActivateTool("LineTool");
                    break;
                case "Polyline":
                    if (menuItem.OwnerItem.Name == "Connectors")
                        this.Controller.ActivateTool("PolylineLinkTool");
                    else
                        this.Controller.ActivateTool("PolyLineTool");
                    break;
                case "Directed Line":
                    this.Controller.ActivateTool("DirectedLineLinkTool");
                    break;
                case "Orthogonal":
                    this.Controller.ActivateTool("OrthogonalLinkTool");
                    break;
                case "Orgline":
                    this.Controller.ActivateTool("OrgLineConnectorTool");
                    break;
                case "Spline":
                    this.Controller.ActivateTool("SplineTool");
                    break;
                case "Bezier":
                    this.Controller.ActivateTool("BezierTool");
                    break;
                case "Pencil":
                    this.Controller.ActivateTool("PencilTool");
                    break;
                case "Curve":
                    this.Controller.ActivateTool("CurveTool");
                    break;
                case "Closed Curve":
                    this.Controller.ActivateTool("ClosedCurveTool");
                    break;
                case "Rectangle":
                    this.Controller.ActivateTool("RectangleTool");
                    break;
                case "Round Rectangle":
                    this.Controller.ActivateTool("RoundRectTool");
                    break;
                case "Ellipse":
                    this.Controller.ActivateTool("EllipseTool");
                    break;       
                case "Polygon":
                    this.Controller.ActivateTool("PolygonTool");
                    break;
                case "Bitmap":
                    this.Controller.ActivateTool("BitmapTool");
                    break;
                case "Text":
                    this.Controller.ActivateTool("TextTool");
                    break;
                case "Rich Text":
                    this.Controller.ActivateTool("RichTextTool");
                    break;                
                case "Ruler":
                    this.ShowRulers = menuItem.Checked;
                    break;
                case "Fit to Drawing":
                    this.Model.SizeToContent = menuItem.Checked;
                    break;
                case "Boundary Constraints":
                    this.Model.BoundaryConstraintsEnabled = menuItem.Checked;
                    break;
                case "Document Size":
                    using (PageSizeDialog dlgPageSize = new PageSizeDialog())
                    {
                        dlgPageSize.PageSize = (PageSize)this.Model.DocumentSize.Clone();
                        dlgPageSize.PrinterSettings = (System.Drawing.Printing.PageSettings)this.View.PageSettings.Clone();
                        if (dlgPageSize.ShowDialog(this) == DialogResult.OK)
                        {
                            this.Model.DocumentSize = dlgPageSize.PageSize;
                            this.View.PageSettings.Landscape = dlgPageSize.PrinterSettings.Landscape;
                        }
                    }
                    break;
                case "Background Style":
                    using (FillStyleDialog dlgFillStyle = new FillStyleDialog())
                    {
                        if (dlgFillStyle.ShowDialog(this) == DialogResult.OK)
                        {
                            this.Model.BackgroundStyle = dlgFillStyle.FillStyle;
                        }
                    }
                    break;
                case "Layout":
                    using (LayoutDialog dlgLayout = new LayoutDialog(this))
                    {
                        dlgLayout.ShowDialog();
                    }
                    break;
                case "New":
                    m_document = null;
                    this.Model.Clear();                    
                    this.Model = CreateModel();
                    this.m_strFileName = string.Empty;                    
                    break;
                case "Open":
                    using (OpenFileDialog dlgOpen = new OpenFileDialog())
                    {
                        dlgOpen.Filter = "Diagram Files (*.edd)|*.edd|All files (*.*)|*.*";
                        dlgOpen.DefaultExt = "*.edd";
                        dlgOpen.Title = "Open Diagram";
                        if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                        {
                            if (File.Exists(dlgOpen.FileName))
                            {
                                this.Load(dlgOpen.FileName);
                                this.m_strFileName = dlgOpen.FileName;
                            }
                        }
                    }
                    break;                
                case "Save":
                    if (this.m_strFileName == string.Empty)
                    {
                        using (SaveFileDialog dlgSave = new SaveFileDialog())
                        {
                            dlgSave.Filter = "Binary Diagram|*.edd|XML Diagram|*.xml";
                            if (dlgSave.ShowDialog(this) == DialogResult.OK)
                            {
                                if (Path.GetExtension(dlgSave.FileName) == ".edd")
                                    this.SaveBinary(dlgSave.FileName);
                                else
                                    this.SaveSoap(dlgSave.FileName);
                                this.m_strFileName = dlgSave.FileName;
                            }
                        }
                    }
                    else
                    {
                        if (Path.GetExtension(this.m_strFileName) == ".xml")
                            this.SaveSoap(this.m_strFileName);
                        else
                            this.SaveBinary(this.m_strFileName);
                    }
                    break;
                case "Save As":
                    using (SaveFileDialog dlgSaveAs = new SaveFileDialog())
                    {
                        dlgSaveAs.Filter = "Binary Diagram|*.edd|XML Diagram|*.xml|All Files|*.*";
                        if (dlgSaveAs.ShowDialog(this) == DialogResult.OK)
                        {
                            if (Path.GetExtension(dlgSaveAs.FileName) == ".xml")
                                this.SaveSoap(dlgSaveAs.FileName);
                            else
                                this.SaveBinary(dlgSaveAs.FileName);
                            this.m_strFileName = dlgSaveAs.FileName;
                        }
                    }
                    break;
                case "Page Setup":
                    using (PrintSetupDialog dlgPrintSetup = new PrintSetupDialog())
                    {
                        dlgPrintSetup.PageSettings = (System.Drawing.Printing.PageSettings)this.View.PageSettings.Clone();
                        dlgPrintSetup.PrintZoom = (PrintZoom)this.View.PrintZoom.Clone();
                        if (dlgPrintSetup.ShowDialog() == DialogResult.OK)
                        {
                            this.View.PageSettings = dlgPrintSetup.PageSettings;
                            this.View.PrintZoom = dlgPrintSetup.PrintZoom;
                            this.View.RefreshPageSettings();
                        }
                    }
                    break;
                case "Print Preview":
                    System.Drawing.Printing.PrintDocument printDoc = this.CreatePrintDocument();
                    using (PrintPreviewDialog printPreviewDlg = new PrintPreviewDialog())
                    {
                        printPreviewDlg.StartPosition = FormStartPosition.CenterScreen;

                        printDoc.PrinterSettings.FromPage = 0;
                        printDoc.PrinterSettings.ToPage = 0;
                        printDoc.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.AllPages;

                        printPreviewDlg.Document = printDoc;
                        printPreviewDlg.ShowDialog(this);
                    }
                    break;
                case "Print":
                    printDoc = this.CreatePrintDocument();
                    using (PrintDialog printDlg = new PrintDialog())
                    {
                        printDlg.Document = printDoc;
                        printDlg.AllowSomePages = true;
                        if (printDlg.ShowDialog(this) == DialogResult.OK)
                        {
                            printDoc.PrinterSettings = printDlg.PrinterSettings;
                            printDoc.Print();
                        }
                    }
                    break;
                case "Cut":
                    this.Controller.Cut();
                    break;
                case "Copy":
                    this.Controller.Copy();
                    break;
                case "Paste":
                    this.Controller.Paste();
                    break;
                case "Delete":
                    this.Controller.Delete();
                    break;
                case "Select All":
                    this.Controller.SelectAll();
                    break;
                case "Align Left":
                    this.AlignLeft();
                    break;
                case "Align Right":
                    this.AlignRight();
                    break;
                case "Align Middle":
                    this.AlignMiddle();
                    break;
                case "Align Center":
                    this.AlignCenter();
                    break;
                case "Align Bottom":
                    this.AlignBottom();
                    break;
                case "Align Top":
                    this.AlignTop();
                    break;
                case "Flip Horizontally":
                    this.FlipHorizontal();
                    break;
                case "Flip Vertically":                   
                    this.FlipVertical();
                    break;
                case "Flip Both":
                    this.FlipHorizontal();
                    this.FlipVertical();
                    break;
                case "Group":
                    this.Controller.Group();
                    break;
                case "Ungroup":
                    this.Controller.UnGroup();
                    break;
                case "Bring To Front":
                    this.Controller.BringToFront();
                    break;
                case "Bring Forward":
                    this.Controller.BringForward();
                    break;
                case "Send Backward":
                    this.Controller.SendBackward();
                    break;
                case "Send To Back":
                    this.Controller.SendToBack();
                    break;
                case "Rotate Right":
                    this.Rotate(90);
                    break;
                case "Rotate Left":
                    this.Rotate(-90);
                    break;
                case "Same Width":
                    this.SameWidth();
                    break;
                case "Same Height":
                    this.SameHeight();
                    break;
                case "Same Size":
                    this.SameSize();
                    break;
                case "Space Across":
                    this.SpaceAcross();
                    break;
                case "Space Down":
                    this.SpaceDown();
                    break;               
            }
        }              
        #endregion

        #region Scrolling
        /// <summary>
        /// Convert origin position to scroll coordinates using ScrollGranularity.
        /// </summary>
        /// <param name="ptPoint">View origin.</param>
        /// <returns>Origin in scroll coordinates.</returns>
        private PointF ConvertToScrollRanges(PointF ptPoint)
        {
            ptPoint.X = (ptPoint.X + this.ScrollVirtualBounds.X) * this.ScrollGranularity;
            ptPoint.Y = (ptPoint.Y + this.ScrollVirtualBounds.Y) * this.ScrollGranularity;

            return ptPoint;
        }

        /// <summary>
        /// Convert origin position from scroll to view cooridnates using ScrollGranularity.
        /// </summary>
        /// <param name="ptPoint">Scroll origin to covert.</param>
        /// <returns>Origin in view coordinates.</returns>
        private PointF ConvertFromScrollRanges(PointF ptPoint)
        {
            ptPoint.X = ptPoint.X / this.ScrollGranularity - this.ScrollVirtualBounds.X;
            ptPoint.Y = ptPoint.Y / this.ScrollGranularity - this.ScrollVirtualBounds.Y;

            return ptPoint;
        }

        /// <summary>
        /// Get scroll offset what need to scroll document to the bounds.
        /// Calc using formula: all bounds size minus visible model bounds.
        /// </summary>
        /// <returns>Size of scroll offset in pixel units.</returns>
        private SizeF GetUnvisibleSize()
        {
            float fMagnification = this.Magnification / 100f;
            MeasureUnits modelUnits = MeasureUnits.Pixel;
            SizeF szUnvisible = SizeF.Empty;

            // get model measure units and logical size
            if (this.Model != null)
            {
                modelUnits = this.Model.MeasurementUnits;
                szUnvisible = this.Model.LogicalSize;
            }

            if (modelUnits != MeasureUnits.Pixel)
            {
                szUnvisible.Width = MeasureUnitsConverter.ToPixelX(szUnvisible.Width, modelUnits);
                szUnvisible.Height = MeasureUnitsConverter.ToPixelY(szUnvisible.Height, modelUnits);
            }

            szUnvisible.Width -= this.AutoScrollBounds.Width / fMagnification;
            szUnvisible.Height -= this.AutoScrollBounds.Height / fMagnification;

            return szUnvisible;
        }

        /// <summary>
        /// Set default document spaces and scroll minimum and maximum value.
        /// </summary>
        private void ResetScrollPosition()
        {
            MeasureUnits docUnits = this.Model.MeasurementUnits;

            // update virtual bounds
            this.ScrollVirtualBounds = RectangleF.Empty;

            // update scroll min and max value
            UpdateScrollRange();
            
            // reset origin Reference to update scroll value
            this.ptScrollOriginRef = new PointF(this.Origin.X - 1, 0);
            
            // update scroll position to origin
            UpdateScrolls(this.Origin);
        }

        /// <summary>
        /// This method sets the range on the horizontal and vertical scrollbars.
        /// </summary>
        /// <remarks>
        /// The scroll range for the scrollbars is determined by the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.VirtualSize"/> property
        /// of the view and the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.ScrollGranularity"/>
        /// property.
        /// </remarks>
        public virtual void UpdateScrollRange()
        {
            if (this.Model != null)
            {
                m_bLockScrollUpdate = true;
                SizeF szUnVisibleSize = GetUnvisibleSize();

                float fHScrollMax = szUnVisibleSize.Width + this.ScrollVirtualBounds.Right;
                float fVScrollMax = szUnVisibleSize.Height + this.ScrollVirtualBounds.Bottom;
                int nHLargeOffset = this.HScrollBar.LargeChange - 1;
                int nVLargeOffset = this.VScrollBar.LargeChange - 1;

                // set new min and max value
                this.HScrollBar.Minimum = 0;
                this.HScrollBar.Maximum = (int)Math.Ceiling(Math.Max(0, fHScrollMax * this.ScrollGranularity + nHLargeOffset));
                this.VScrollBar.Minimum = 0;
                this.VScrollBar.Maximum = (int)Math.Ceiling(Math.Max(0, fVScrollMax * this.ScrollGranularity + nVLargeOffset));

                // update large change value use magnification
                // int nDefLargeChange = Math.Max((int)( c_nLARGE_CHANGE_DEFAULT_VALUE / ( this.View.Magnification / 100f ) ), 1);
                int nDefLargeChange = (int)(c_nLARGE_CHANGE_DEFAULT_VALUE * (this.View.Magnification / 100f));

                if (this.HScrollBar.Maximum == int.MinValue)
                    this.HScrollBar.Maximum++;
                if (Math.Abs(this.HScrollBar.Minimum) + Math.Abs(this.HScrollBar.Maximum) > nDefLargeChange)
                    this.HScrollBar.LargeChange = nDefLargeChange;
                if (this.VScrollBar.Maximum == int.MinValue)
                    this.VScrollBar.Maximum++;
                if (Math.Abs(this.VScrollBar.Minimum) + Math.Abs(this.VScrollBar.Maximum) > nDefLargeChange)
                    this.VScrollBar.LargeChange = nDefLargeChange;

                m_bLockScrollUpdate = false;
                UpdateScrollBars();

                if (this.ShowRulers)
                {
                    m_bResize = true;
                    m_bNeedVRulerRefresh = false;
                    m_bNeedHRulerRefresh = true;

                  
                    // Reflect current scroll position on rulers
               
                    UpdateHorizontalRuler(System.Drawing.Rectangle.Empty);

                    m_bNeedHRulerRefresh = false;
                    m_bNeedVRulerRefresh = true;
                
                    UpdateVerticalRuler(System.Drawing.Rectangle.Empty);

                    m_bNeedHRulerRefresh = true;
                    m_bNeedVRulerRefresh = true;
                }
                m_bResize = false;
            }
        }

        /// <summary>
        /// Update the scroll position from new origin.
        /// </summary>
        /// <param name="ptNewOrigin">The new origin position.</param>
        protected virtual void UpdateScrolls(PointF ptNewOrigin)
        {
            PointF ptScrollOrigin = ConvertToScrollRanges(ptNewOrigin);
            SizeF szUnvisibleSize = GetUnvisibleSize();
            int nHLargeOffset = this.HScrollBar.LargeChange - 1;
            int nVLargeOffset = this.VScrollBar.LargeChange - 1;
            PointF ptOrigin = PointF.Empty;
            RectangleF rcScrollBounds = this.ScrollVirtualBounds;

            if (m_bMagnificationChanged)
                ptOrigin = ptNewOrigin;
            else
                ptOrigin = ptScrollOrigin;

            // update horizontal scroll minimun and maximum value
            if (ptOrigin.X <= this.HScrollBar.Minimum)
                rcScrollBounds.X = -ptNewOrigin.X;
            else if (ptScrollOrigin.X >= this.HScrollBar.Maximum - nHLargeOffset)
            {
                rcScrollBounds.Width = Math.Min(ptNewOrigin.X - szUnvisibleSize.Width, m_scrollBounds.Width);
            }

            // update vertical scroll minimun and maximum value
            if (ptOrigin.Y <= this.VScrollBar.Minimum)
                rcScrollBounds.Y = -ptNewOrigin.Y;
            else if (ptScrollOrigin.Y >= this.VScrollBar.Maximum - nVLargeOffset)
            {
                rcScrollBounds.Height = Math.Min(ptNewOrigin.Y - szUnvisibleSize.Height, m_scrollBounds.Height);
            }

            this.ScrollVirtualBounds = rcScrollBounds;

            // update minimum and maximum scroll ranges
            this.UpdateScrollRange();

            // if origion changed then update srcoll value
            if (ptNewOrigin != this.ptScrollOriginRef)
            {
                // update origin reference
                this.ptScrollOriginRef = ptNewOrigin;

                int nOriginX = (int)Math.Round(ptScrollOrigin.X);
                int nOriginY = (int)Math.Round(ptScrollOrigin.Y);
                int nHScrollMin = this.HScrollBar.Minimum;
                int nHScrollMax = this.HScrollBar.Maximum - nHLargeOffset;
                int nVScrollMin = this.VScrollBar.Minimum;
                int nVScrollMax = this.VScrollBar.Maximum - nVLargeOffset;

                m_bLockScrollUpdate = true;

                this.HScrollBar.Value = (nOriginX < nHScrollMin) ? Math.Max(nOriginX, nHScrollMin) : Math.Min(nOriginX, nHScrollMax);
                this.VScrollBar.Value = (nOriginY < nVScrollMin) ? Math.Max(nOriginY, nVScrollMin) : Math.Min(nOriginY, nVScrollMax);

                m_bLockScrollUpdate = false;
                this.UpdateScrollBars();
            }
        }

        /// <summary>
        /// Update origin value to current scroll position.
        /// </summary>
        /// <param name="ptScrollOrigin">Current scroll origin in scroll coordinates.</param>
        /// <param name="scrollbars">Indicate what scrollbar value need to update.</param>
        private void UpdateOrigin(PointF ptScrollOrigin, ScrollBars scrollbars)
        {
            // set new origin value
            this.ptScrollOriginRef = ConvertFromScrollRanges(ptScrollOrigin);

            // reset non-changed values
            if (scrollbars == ScrollBars.None || scrollbars == ScrollBars.Vertical)
                this.ptScrollOriginRef.X = this.View.Origin.X;

            if (scrollbars == ScrollBars.None || scrollbars == ScrollBars.Horizontal)
                this.ptScrollOriginRef.Y = this.View.Origin.Y;

            this.View.Origin = Geometry.ConvertPoint(ptScrollOriginRef);
        }

        /// <summary>
        /// Called when the horizontal scrollbar is moved.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="se">Event arguments.</param>
        /// <remarks>
        /// Scrolls the window origin by the specified amount.
        /// </remarks>
        protected override void OnHScroll(object sender, ScrollEventArgs se)
        {
            if (!m_bLockScrollUpdate)
            {
                float fScrollOffset = this.HScrollBar.SmallChange / this.ScrollGranularity;
                int nLargeOffset = this.VScrollBar.LargeChange - 1;
                PointF ptScrollOrigin = new PointF(se.NewValue, this.VScrollBar.Value);
                SizeF szUnvisible = GetUnvisibleSize();
                
                // RectangleF rcScrollBounds = this.ScrollVirtualBounds;

                /*// update horizontal minimum value to continue scrolling
                if (se.Type == ScrollEventType.SmallDecrement && ptScrollOrigin.X <= this.HScrollBar.Minimum)
                {
                    rcScrollBounds.X += fScrollOffset;
                    ptScrollOrigin.X = 0;
                }

                // update horizontal maximum value to continue scrolling
                if (se.Type == ScrollEventType.SmallIncrement && ptScrollOrigin.X + nLargeOffset >= this.HScrollBar.Maximum)
                {
                    rcScrollBounds.Width += fScrollOffset;
                    ptScrollOrigin.X = (rcScrollBounds.Width + rcScrollBounds.X + szUnvisible.Width) * this.ScrollGranularity;
                } */

                // this.ScrollVirtualBounds = rcScrollBounds;
                // update horizontal scroll new value  to set
                se.NewValue = (int)ptScrollOrigin.X;
                if (this.HScrollBar.Value != se.NewValue)
                    m_ptOrigin = new PointF(this.ConvertFromScrollRanges(ptScrollOrigin).X, m_ptOrigin.Y);
                // update origin to scroll position
                UpdateOrigin(ptScrollOrigin, ScrollBars.Horizontal);
            }

            base.OnHScroll(sender, se);
        }

        /// <summary>
        /// Called when the value of the horizontal scroll bar changes.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Updates the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
        /// property of the view.
        /// </remarks>
        protected override void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            if (!m_bLockScrollUpdate)
            {
                // update origin to scroll position
                UpdateOrigin(new PointF(this.HScrollBar.Value, 0), ScrollBars.Horizontal);
            }
        }

        /// <summary>
        /// Called when the vertical scrollbar is moved.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="se">Event arguments.</param>
        /// <remarks>
        /// Scrolls the window origin by the specified amount.
        /// </remarks>
        protected override void OnVScroll(object sender, ScrollEventArgs se)
        {
            if (!m_bLockScrollUpdate)
            {
                // defaul minimum size what will used to restore minimun value
                float fScrollOffset = this.VScrollBar.SmallChange / this.ScrollGranularity;
                int nLargeOffset = this.VScrollBar.LargeChange - 1;
                PointF ptScrollOrigin = new PointF(this.HScrollBar.Value, se.NewValue);
                SizeF szUnvisible = GetUnvisibleSize();
                
                // RectangleF rcScrollBounds = this.ScrollVirtualBounds;

                /*// update vertical minimum value to continue scrolling
                if (se.Type == ScrollEventType.SmallDecrement && ptScrollOrigin.Y <= this.VScrollBar.Minimum)
                {
                    rcScrollBounds.Y += fScrollOffset;
                    ptScrollOrigin.Y = 0;
                }

                // update horizontal maximum value to continue scrolling
                if (se.Type == ScrollEventType.SmallIncrement && ptScrollOrigin.Y + nLargeOffset >= this.VScrollBar.Maximum)
                {
                    rcScrollBounds.Height += fScrollOffset;
                    ptScrollOrigin.Y = (rcScrollBounds.Height + rcScrollBounds.Y + szUnvisible.Height) * this.ScrollGranularity;
                }*/

                // this.ScrollVirtualBounds = rcScrollBounds;
                // update vertical scroll new value to set
                se.NewValue = (int)ptScrollOrigin.Y;
                if (this.VScrollBar.Value != se.NewValue)
                    m_ptOrigin = new PointF(m_ptOrigin.X, this.ConvertFromScrollRanges(ptScrollOrigin).Y);
                // update origin to scroll position
                UpdateOrigin(ptScrollOrigin, ScrollBars.Vertical);
            }

            base.OnVScroll(sender, se);
        }

        /// <summary>
        /// Called when the value of the vertical scroll bar changes.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Updates the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
        /// property of the view.
        /// </remarks>
        protected override void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            if (!m_bLockScrollUpdate)
            {
                // update origin to scroll position
                UpdateOrigin(new PointF(0, this.VScrollBar.Value), ScrollBars.Vertical);
            }
        }

        /// <summary>
        /// Called when the diagram is zoomed through mouse wheel.
        /// </summary>
        /// <param name="e">MouseWheelZoomEventArgs</param>
        protected override void OnMouseWheelZoom(MouseWheelZoomEventArgs e)
        {
            m_bMouseWheelZoom = true;            
            base.OnMouseWheelZoom(e);
            if (e.Delta < 0)
                this.View.Magnification -= this.View.ZoomIncrement;
            else
                this.View.Magnification += this.View.ZoomIncrement; 
        }

        /// <summary>
        /// Updates the virual scroll bounds to model content.
        /// </summary>
        private void UpdateScrollBoundsToContent()
        {
            SizeF szModel = this.Model.DocumentSize.GetSize(this.Model.MeasurementUnits);
            RectangleF rcNewBounds = this.ScrollVirtualBounds;

            // update scroll vistual bounds to model content size
            RectangleF rcVirtualBounds = new RectangleF(
                -rcNewBounds.X, 
                -rcNewBounds.Y,
                szModel.Width, 
                szModel.Height);
            RectangleF rcModelContent = this.Model.Bounds;

            if (!rcVirtualBounds.Contains(rcModelContent))
            {
                if (rcModelContent.X < rcVirtualBounds.X)
                    rcNewBounds.X = -rcModelContent.X;

                if (rcModelContent.Y < rcVirtualBounds.Y)
                    rcNewBounds.Y = -rcModelContent.Y;

                if (rcModelContent.Width > rcVirtualBounds.Width)
                    rcNewBounds.Width = rcModelContent.Width - szModel.Width;

                if (rcModelContent.Height > rcVirtualBounds.Height)
                    rcNewBounds.Height = rcModelContent.Height - szModel.Height;

                // set new bounds value
                this.ScrollVirtualBounds = rcNewBounds;
            }
        }
        #endregion

        #region Drag and Drop
        /// <summary>
        /// Called when the mouse enters the diagram during a drag operation.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Looks to see if a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/> is
        /// available in the System.Windows.Forms.IDataObject provided in the
        /// event arguments. If so, the aggregate bounds of the nodes is
        /// calculated and a tracking rectangle is created to track the nodes
        /// as they are dragged across the diagram.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragDrop"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragLeave"/>
        /// </remarks>
        protected override void OnDragEnter(DragEventArgs e)
        {
            m_dataObj = e.Data;
            m_controller.OnDragEnter(e);
            e.Effect = DragDropEffects.Copy;
            this.dragging = true;
            if (m_dataObj.GetDataPresent(typeof(DragDropData)))
            {
                if (this.Controller.Guides.Enable)
                {
                    if (m_dropNode == null)
                    {
                        DragDropData dragDropData = (DragDropData)m_dataObj.GetData(typeof(DragDropData));
                        m_dropNode = dragDropData.Nodes[0].Clone() as Node;
                        m_dropNodeImage = dragDropData.DragHelper.DragWindow.DragBitmap.Clone() as Bitmap;
                        dragDropData.DragHelper.EndDrag();
                        m_dropNode.PinPoint = MeasureUnitsConverter.Convert(GetDropNodePosition(new POINT(e.X, e.Y)), MeasureUnits.Pixel, m_dropNode.MeasurementUnit);
                        this.Controller.ActiveTool.AlterStyle(m_dropNode);
                    }
                }
                else
                {
                    DragDropData dragDropData = (DragDropData)m_dataObj.GetData(typeof(DragDropData));
                    m_dropNode = dragDropData.Nodes[0].Clone() as Node;
                }
            }
            base.OnDragEnter(e);
        }

        /// <summary>
        /// Called when objects are dropped onto the diagram.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Looks to see if a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/> is
        /// available in the System.Windows.Forms.IDataObject provided in the
        /// event arguments. If so, the NodeCollection is retrieved and
        /// added to the diagram.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragEnter"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragLeave"/>
        /// </remarks>
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            if (dragging)
            {
                // EraseDragRect();
                dragging = false;
            }

            IDataObject dataObj = e.Data;            
            if (dataObj.GetDataPresent(typeof(DragDropData)))
            {
                if (this.Model != null)
                {
                    DragDropData dragDropData = (DragDropData)dataObj.GetData(typeof(DragDropData));
                    PointF ptPosition = GetDropNodePosition(new POINT(e.X, e.Y));
                    foreach (Node dropNode in dragDropData.Nodes)
                    {
                        dropNode.PinPoint = MeasureUnitsConverter.Convert(ptPosition, MeasureUnits.Pixel, dropNode.MeasurementUnit);
                        ptPosition.X += 30;
                        ptPosition.Y += 30;
                    }
                    
                    this.Controller.NeedDocumentRefresh = true;
                    if (dragDropData.Nodes.Count == 1)
                    {
                        this.Model.AppendChild(dragDropData.Nodes[0]);
                        m_dataObj = null;
                    }
                    else
                    {
                        int index;
                        this.Model.AppendChildren(dragDropData.Nodes, out index);
                        m_dataObj = null;
                    }
                    if (this.Controller.Guides.Enable)
                    {
                        m_dropNodeImage.Dispose();
                        m_dropNode = null;
                    }
                    this.Controller.NeedDocumentRefresh = false;
                    //node.SetPinPoint(ptPosition, units);
                    //this.Model.AppendChild(dragDropData.Nodes[0]);
                }
            }

            Focus();

            
        }

        /// <summary>
        /// Called when mouse leaves the diagram during a drag operation.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Clears the tracking rectangle.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragEnter"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.OnDragDrop"/>
        /// </remarks>
        protected override void OnDragLeave(EventArgs e)
        {
            if (this.dragging && m_dataObj != null && m_dataObj.GetDataPresent(typeof(DragDropData)))
            {
                // this.EraseDragRect();
                if (this.Controller.Guides.Enable)
                {
                    DragDropData dragDropData = (DragDropData)m_dataObj.GetData(typeof(DragDropData));
                    if (dragDropData.DragHelper.DragAction == DragAction.Continue)
                    {
                        dragDropData.DragHelper.DragWindow.SetOrigin(new Point(m_dropNodeImage.Width / 2, -5));
                        dragDropData.DragHelper.StartDrag(m_dropNodeImage, Cursor.Position, DragDropEffects.Copy);
                        m_dropNode = null;
                    }
                    else
                    {
                        m_dropNode = null;
                    }
                }
                m_bNeedDocumentRefresh = true;
                Invalidate(this.View.ClientRectangle);
                Update();
            }
            this.dragging = false;
            base.OnDragLeave(e);
        }
        
        /// <summary>
        /// Called when objects are dragged over the control.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Updates tracking when objects are dragged over the diagram.
        /// </remarks>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (this.dragging && m_dropNode != null && this.Controller.Guides.Enable)
            {
                bool boundaryConstraints = this.Model.BoundaryConstraintsEnabled;
                this.Model.BoundaryConstraintsEnabled = false;                
                m_dropNode.PinPoint = MeasureUnitsConverter.Convert(GetDropNodePosition(new POINT(e.X, e.Y)), MeasureUnits.Pixel, m_dropNode.MeasurementUnit);
                this.Controller.Model.BoundaryConstraintsEnabled = boundaryConstraints;
                Invalidate();
            }
            else if (m_dataObj != null)
                e.Effect = DragDropEffects.Copy;
            base.OnDragOver(e);
        }

        protected override void OnStartAutoScrolling(StartAutoScrollingEventArgs e)
        {
            if (e.Reason == AutoScrollReason.OleDragOver)
            {
                e.Cancel = true;               
            }
        }

        /// <summary>
        /// Gets the position of drop node
        /// </summary>
        /// <param name="ptClient">Mouse position</param>
        private PointF GetDropNodePosition(POINT ptClient)
        {
            Window.ScreenToClient(this.Handle, ref ptClient);
            PointF ptDev = new Point(ptClient.X, ptClient.Y);
            MeasureUnits units = MeasureUnits.Pixel;
            IUnitIndependent node = m_dropNode as IUnitIndependent;
            SizeF szPinOffset = node.GetPinPointOffset(units);
            float dMagnification = this.View.Magnification / 100f;
            PointF ptPosition = PointF.Empty;

            ptPosition.X = ptDev.X - szPinOffset.Width * dMagnification;
            ptPosition.Y = ptDev.Y - szPinOffset.Height * dMagnification;
            if (!(this.Controller.Guides.Enable))
                ptPosition.Y += szPinOffset.Height * dMagnification;

            // snap to grid
            ptPosition = this.View.Grid.GetNearestGridPoint(ptPosition, ShowRulers ? RulersHeight : 0);
            ptPosition.X = ptPosition.X + szPinOffset.Width * dMagnification;
            ptPosition.Y = ptPosition.Y + szPinOffset.Height * dMagnification;
            ptPosition = this.Controller.ConvertToModelCoordinates(ptPosition);
            Matrix mtxScale = this.Model.DocumentScale.GetScaleTransformation(this.Model.MeasurementUnits);
            mtxScale.Invert();
            ptPosition = Geometry.AppendMatrix(ptPosition, mtxScale);
            return ptPosition;
        }
               
        /// <summary>
        /// Draws the dragging node.
        /// </summary>
        /// <param name="gfx">graphics</param>
        private void DrawDropNode(Graphics gfx)
        {
            if (m_dropNode != null)
            {
                Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(m_dropNode);
                // append parent scale
                HandlesHitTesting.AppendScaleTransforms(m_dropNode, mtxTemp, true);
                gfx.MultiplyTransform(mtxTemp);
                m_dropNode.Draw(gfx);
                //Draws the guides
                if (this.Controller.Guides.Enable)
                {
                    this.Controller.DrawGuides(m_dropNode, gfx, m_dropNode.BoundingRectangle);
                }
            }
        }            
        
        #endregion

        #region Implementation Methods
        /// <summary>
        /// Wires up the model, view, and controller and performs necessary initialization.
        /// </summary>
        /// <remarks>
        /// This method does not create the model, view, and controller objects. It assumes
        /// that the mode, view, and controller objects have already been created.
        /// This method hooks the model to the view and hooks the view to the
        /// controller.
        /// </remarks>
        protected virtual void MVCInit()
        {
            if (m_controller != null)
            {
                m_controller.Initialize();
            }
        }

        /// <summary>
        /// Destroys the model, view, and controller and ensures that they are completely
        /// disconnected from the diagram control.
        /// </summary>
        protected virtual void MVCDispose()
        {
            if (this.m_model != null)
            {
				if (this.m_model.InUpdate)
                {
                    this.m_model.EndUpdate();
                    m_bInUpdate = true;
                }
                this.m_model.Dispose();
                this.m_model = null;
            }

            if (this.m_view != null)
            {
                this.m_view.UpdateServiceReferences(null);
                this.m_view.Dispose();
                this.m_view = null;
            }

            if (this.m_controller != null)
            {
                this.m_controller.UpdateServiceReferences(null);
            }

            if (m_document != null)
            {
                m_document = null;
            }
        }

        /// <summary>
        /// Subscribes for model events.
        /// </summary>
        /// <param name="model">The model.</param>
        protected void SubscribeForModelEvents(Model model)
        {
            model.EventSink.PropertyChanged += new PropertyChangedEventHandler(EventSink_PropertyChanged);
            model.EventSink.SizeChanged += new SizeChangedEventHandler(EventSink_SizeChanged);
            model.EventSink.NodeCollectionChanged += new CollectionExEventHandler(EventSink_NodeCollectionChanged);
            model.EventSink.PinPointChanged += new PinPointChangedEventHandler(EventSink_PinPointChanged);
            model.EventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
            model.EventSink.RotationChanged += new RotationChangedEventHandler(EventSink_RotationChanged);
            model.EventSink.DocumentBeginUpdate += new EventHandler(EventSink_DocumentBeginUpdate);
            model.EventSink.DocumentEndUpdate += new EventHandler(EventSink_DocumentEndUpdate);

            model.LinkManager.SynhronizeCompleted += new EventHandler(LinkManager_SynhronizeCompleted);
            model.BridgeManager.BridgeGenerationCompleted += new EventHandler(BridgeManager_BridgeGenerationCompleted);
        }

        /// <summary>
        /// Unsubscribes for model events.
        /// </summary>
        /// <param name="model">The model.</param>
        protected void UnsubscribeForModelEvents(Model model)
        {
            model.EventSink.PropertyChanged -= new PropertyChangedEventHandler(EventSink_PropertyChanged);
            model.EventSink.SizeChanged -= new SizeChangedEventHandler(EventSink_SizeChanged);
            model.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(EventSink_NodeCollectionChanged);
            model.EventSink.PinPointChanged -= new PinPointChangedEventHandler(EventSink_PinPointChanged);
            model.EventSink.PinOffsetChanged -= new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
            model.EventSink.RotationChanged -= new RotationChangedEventHandler(EventSink_RotationChanged);
            model.EventSink.DocumentBeginUpdate -= new EventHandler(EventSink_DocumentBeginUpdate);
            model.EventSink.DocumentEndUpdate -= new EventHandler(EventSink_DocumentEndUpdate);

            model.LinkManager.SynhronizeCompleted -= new EventHandler(LinkManager_SynhronizeCompleted);
            model.BridgeManager.BridgeGenerationCompleted -= new EventHandler(BridgeManager_BridgeGenerationCompleted);
        }

        /// <summary>
        /// Hooks up event handlers to the view.
        /// </summary>
        protected virtual void HookViewEvents()
        {
            this.EventSink.OriginChanged += new ViewOriginEventHandler(OnViewOriginChanged);
            this.EventSink.PropertyChanged += new PropertyChangedEventHandler(OnViewPropertyChanged);
            this.EventSink.MagnificationChanged += new ViewMagnificationEventHandler(OnViewMagnificationChanged);
            this.EventSink.ScrollVirtualBoundsChanged += new ViewScrollVirtualBoundsEventHandler(EventSink_ScrollVirtualBoundsChanged);
        }

        /// <summary>
        /// Unhooks event handlers from the view.
        /// </summary>
        protected virtual void UnhookViewEvents()
        {
            this.EventSink.OriginChanged -= new ViewOriginEventHandler(OnViewOriginChanged);
            this.EventSink.PropertyChanged -= new PropertyChangedEventHandler(OnViewPropertyChanged);
            this.EventSink.MagnificationChanged -= new ViewMagnificationEventHandler(OnViewMagnificationChanged);
            this.EventSink.ScrollVirtualBoundsChanged -= new ViewScrollVirtualBoundsEventHandler(EventSink_ScrollVirtualBoundsChanged);
        }

        /// <summary>
        /// Called when the control needs to paint the window.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// This method draws the view onto the System.Drawing.Graphics
        /// object provided by the System.Windows.Forms.PaintEventArgs
        /// parameter. The view is drawn by calling the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Draw(Graphics,RectangleF)"/>
        /// method.
        /// </remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            
            // update DPI values for units converter
            MeasureUnitsConverter.DpiX = gfx.DpiX;
            MeasureUnitsConverter.DpiY = gfx.DpiY;

            if (this.Model != null && !e.ClipRectangle.IsEmpty)
            {
                if (m_bNeedDocumentRefresh)
                {
                    // prepare graphics for rendering
                    PrepareGraphics(gfx);

                    // RENDER BACKGROUND
                    RectangleF rectInvalid = this.Controller.UpdateInfo.m_rectUpdate;

                    if (rectInvalid.Size.IsEmpty)
                    {
                        rectInvalid = this.Controller.ConvertToModelCoordinates(e.ClipRectangle);
                    }
                    else
                    {
                        rectInvalid.Location = rectInvalid.Location;

                        if (rectInvalid != e.ClipRectangle)
                        {
                            rectInvalid = e.ClipRectangle;
                        }

                        rectInvalid = this.Controller.ConvertToModelCoordinates(rectInvalid);
                    }

                    this.View.Draw(gfx, rectInvalid);

                    // RENDER FOREGROUND
                    // draw Focus
                    this.FocusManager.Draw(gfx);
                    
                    // draw ActiveTool
                    if (this.Controller.ActiveTool != null && this.Controller.ActiveTool.InAction)
                    {
                        this.Controller.ActiveTool.Draw(gfx);
                    }

                    if (this.Controller.Guides.Enable && m_dataObj != null)
                    {
                        DrawDropNode(gfx);
                    }
                }

                if (this.ShowRulers)
                    RenderRulers(gfx);
            }
            else
            {
                e.Graphics.Clear(SystemColors.ControlDark);

                base.OnPaint(e);
            }

            if (bDebug)
            {
                PointF ptModelCoord = this.Controller.MouseLocation;
                
                // Convert to model coordinate.
                float width = 10;
                ptModelCoord = this.Controller.ConvertToModelCoordinates(ptModelCoord);
                ptModelCoord = this.Controller.ConvertFromModelToClientCoordinates(ptModelCoord);
                gfx.SetClip(RectangleF.Union(e.ClipRectangle, new RectangleF(ptModelCoord, new SizeF(width, width))));

                using (Pen pn = new Pen(Color.Black, 1f / (this.Magnification / 100f)))
                {
                    GraphicsState save = gfx.Save();

                    // vertical
                    gfx.DrawLine(pn, ptModelCoord.X, ptModelCoord.Y - width, ptModelCoord.X, ptModelCoord.Y + width);
                    
                    // horizontal
                    gfx.DrawLine(pn, ptModelCoord.X - width, ptModelCoord.Y, ptModelCoord.X + width, ptModelCoord.Y);
                    float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * (MeasureUnitsConverter.FromPixelX(this.Font.Size, MeasureUnits.Point)), MeasureUnits.Point, MeasureUnits.Inch));
                    if (fontSize > (1 / 72f))
                    gfx.DrawString(ptModelCoord.ToString(), this.Font, Brushes.Black, ptModelCoord);

                    gfx.Restore(save);
                }
            }
        }
        private void PrepareGraphics(Graphics gfx)
        {
            // Apply transformations
            gfx.PageScale = this.View.Magnification / 100f;
            gfx.PageUnit = GraphicsUnit.Pixel;

            // consider whethr rulers are visible
            if (this.ShowRulers)
            {
                gfx.TranslateTransform(this.RulersHeight / gfx.PageScale, this.RulersHeight / gfx.PageScale);
            }

            // append document origin
            gfx.TranslateTransform(-this.Origin.X, -this.Origin.Y);
        }

        /// <summary>
        /// Renders the rulers.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        private void RenderRulers(Graphics gfx)
        {
            // save state
            GraphicsState stateRuler = gfx.Save();

            System.Drawing.Rectangle rectClip = System.Drawing.Rectangle.Empty;
            rectClip.Location = new Point(this.RulersHeight, 0);
            rectClip.Size = new Size(this.ClientSize.Width - this.RulersHeight, this.RulersHeight);

            gfx.ResetTransform();

            gfx.PageScale = 1;
            gfx.PageUnit = GraphicsUnit.Pixel;
            gfx.SmoothingMode = SmoothingMode.HighSpeed;

            // draw rulers
            // Draw horizontal ruler
            if (m_bNeedHRulerRefresh)
            {
                this.HorizontalRuler.Draw(gfx, View);
            }

            if (m_bNeedVRulerRefresh)
            {
                this.VerticalRuler.Draw(gfx, View);
            }

            // draw left top non-ruler area
            gfx.FillRectangle(SystemBrushes.Control, 0, 0, this.RulersHeight, this.RulersHeight);

            gfx.Restore(stateRuler);
        }

        /// <summary>
        /// Called when the control needs to clear the background.
        /// </summary>
        /// <param name="pevent">Event arguments.</param>
        /// <remarks>
        /// This method does nothing. It is overridden to prevent the
        /// control from painting the background in order to eliminate
        /// flicker.
        /// </remarks>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
			base.OnPaintBackground(pevent);
            // Do not erase background in order to eliminate flicker.
        }

        /// <summary>
        /// Called when the control is resized.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Updates the size of the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram.View"/>
        /// to match the new size of the control.
        /// </remarks>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.ShowRulers)
            {
                UpdateClientArea();
            }

            if (this.DesignMode)
            {
                Invalidate(true);
                base.OnSizeChanged(e);
            }
            else
            {
                base.OnSizeChanged(e);
                m_bResize = true;

                m_bNeedDocumentRefresh = true;
                Invalidate();

                if (this.Model != null)
                {
                    SizeF szUnVisibleSize = GetUnvisibleSize();

                    // calc max size as univisible model size + model line width
                    float fHScrollMax = szUnVisibleSize.Width + ScrollVirtualBounds.Right + this.Model.LineStyle.LineWidth;
                    float fVScrollMax = szUnVisibleSize.Height + ScrollVirtualBounds.Bottom + this.Model.LineStyle.LineWidth;

                    // calc max scroll value
                    double dHMaxValue = fHScrollMax * this.ScrollGranularity;
                    double dVMaxValue = fVScrollMax * this.ScrollGranularity;

                    // update origin to new max and min values
                    if (this.HScrollBar.Maximum > 0 && dHMaxValue < this.HScrollBar.Value)
                    {
                        float fValue = (float)Math.Ceiling(dHMaxValue);
                        fValue = Math.Max(this.HScrollBar.Minimum, fValue);
                        fValue = Math.Min(this.HScrollBar.Maximum, fValue);

                        this.HScrollBar.Value = (int)fValue;
                        UpdateOrigin(new PointF(fValue, 0), ScrollBars.Horizontal);
                    }

                    if (this.VScrollBar.Maximum > 0 && dVMaxValue < this.VScrollBar.Value)
                    {
                        float fValue = (float)Math.Ceiling(dVMaxValue);
                        fValue = Math.Max(this.VScrollBar.Minimum, fValue);
                        fValue = Math.Min(this.VScrollBar.Maximum, fValue);

                        this.VScrollBar.Value = (int)fValue;
                        UpdateOrigin(new PointF(0, fValue), ScrollBars.Vertical);
                    }
                    if (this.View != null)
                        m_view.ClientRectangle = this.ClientRectangle;
                }

                // update minimum and maximum scroll value
                UpdateScrollRange();
            }
        }

        private void UpdateClientArea()
        {
            m_bNeedDocumentRefresh = true;

            if (this.ShowRulers)
            {
                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = true;

                UpdateHRulerBounds();
                UpdateVRulerBounds();
            }

            if (!this.DesignMode)
                Update();
        }

        /// <summary>
        /// Called when diagram is initialized
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="evtArgs">Event args</param>
        protected virtual void OnDiagramInitialized(object sender, EventArgs evtArgs)
        {
            // if( this.DiagramInitialized != null )
            //   this.DiagramInitialized( this, evtArgs );
        }
        #endregion

        #region Overrides
		public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if (this.View != null)
                {
                    if (this.View.BackgroundColor != value)
                        this.View.BackgroundColor = value;
                }
            }
        }
        /// <summary>
        /// Raises the ShowContextMenu event when the user right-clicks inside
        /// the control.
        /// </summary>
        /// <param name="e">Context menu event args.</param>
        /// <remarks>
        /// Raises the ShowContextMenu event when the user right-clicks inside
        /// the control.
        /// <para/>
        /// You can cancel showing a content menu when
        /// you assign True to <see cref="P:System.ComponentModel.CancelEventArgs.Cancel"/>.<para/></remarks>
        protected override void OnShowContextMenu(ShowContextMenuEventArgs e)
        {
            if ((this.ContextMenu == null && this.ContextMenuStrip == null) || ((!(this.Controller.ActiveTool is SelectTool)) && this.Controller.ActiveTool.InAction)
                || ((this.Controller.ActiveTool is SelectTool) && this.Controller.ActiveTool.InAction) || m_model == null)
            {
                e.Cancel = true;
            }

            UpdateDefaultContextMenu();
            base.OnShowContextMenu(e);
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>
        /// true if the character was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bProcessed;
            
            if ((keyData & Keys.Modifiers) == Keys.Alt && (keyData & Keys.KeyCode) == Keys.Back)
            {
                // Alt+Backspace --> undo command
                Model document = this.Controller.Model;

                if (document != null)
                {
                    document.HistoryManager.Undo();
                }

                bProcessed = true;
            }
            else
            {
                bProcessed = base.ProcessCmdKey(ref msg, keyData);
            }

            return bProcessed;
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            
            // Alt, Control, Shift
            //if (e.KeyValue >= 16 && e.KeyValue <= 18) 
            //{
            //    base.OnKeyDown(e);
            //    return;
            //}
            if (m_controller != null)
            {
                m_controller.OnKeyDown(e);
                base.OnKeyDown(e);
            }
            
            if (!e.Handled)
            {
                switch (e.KeyValue)
                {
                    case (int)Keys.Up:
                        e.Handled = ProcessUpKey(e.Shift);
                        break;
                    case (int)Keys.Down:
                        e.Handled = ProcessDownKey(e.Shift);
                        break;
                    case (int)Keys.Left:
                        e.Handled = ProcessLeftKey(e.Shift);
                        break;
                    case (int)Keys.Right:
                        e.Handled = ProcessRightKey(e.Shift);
                        break;
                    case (int)Keys.Enter:
                        e.Handled = ProcessEnterKey(e.Shift);
                        break;
                    case (int)Keys.Tab:
                        if (this.Focused && !IsEditing())
                        {
                            if (c_bFocusNodeOnTab)
                            {
                                ProcessTabKey(e.Shift);
                            }
                            else
                            {
                                if (e.Shift)
                                {
                                    this.Parent.SelectNextControl(this, false, true, true, true);
                                }
                                else
                                {
                                    this.Parent.SelectNextControl(this, true, true, true, true);
                                }
                            }
                        }
                        break;
                    case (int)Keys.Delete:
                        this.Controller.Delete();

                        // e.Handled = true;
                        break;
                    case (int)Keys.Alt:
                        break;
                    case (int)Keys.ShiftKey:
                    case (int)Keys.ControlKey:
                        break;
                    case (int)Keys.Escape:
                        ProcessEscapeKey();
                        break;
                }
            }

                //if (!e.Handled && m_controller != null)
                //{
                //    m_controller.OnKeyDown(e);
                //    base.OnKeyDown(e);
                //}
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyPressEventArgs"/> that contains the event data.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            m_controller.OnKeyPress(e);
            base.OnKeyPress(e);
        }

        /// <summary>
        /// OnSetCursor. Overriden
        /// </summary>
        /// <param name="m">The Message</param>
        /// <internalonly/>
        protected override void OnSetCursor(ref Message m)
        {
            // avoid base class calling
            // to prevent cursor updating by ScrollControl
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            m_controller.OnKeyUp(e);
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Raises the mouse leave event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_controller.OnMouseLeave(e);

            if (this.ShowRulers)
            {
                m_bUpdate = true;
                AcumulateHRulerInvalidArea(System.Drawing.Rectangle.Empty);
                Ruler ruler = this.HorizontalRuler;

                ruler.HighlightAreaStart = 0;
                ruler.HighlightAreaWidth = 0;
                UpdateHRulerBounds();

                AcumulateHRulerInvalidArea(System.Drawing.Rectangle.Empty);
                ruler = this.HorizontalRuler;

                ruler.HighlightAreaStart = 0;
                ruler.HighlightAreaWidth = 0;
                UpdateHRulerBounds();

                m_bNeedDocumentRefresh = true;
                m_bNeedVRulerRefresh = true;
                m_bNeedHRulerRefresh = true;

                Invalidate(this.ClientRectangle);
                Update();
                m_bUpdate = false;
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.ShowRulers)
            {
                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = true;

                UpdateHRulerBounds();
                UpdateVRulerBounds();

                m_bNeedDocumentRefresh = true;
                Invalidate(this.ClientRectangle);
                Update();
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Raises the Mouse Down event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_ptCurMouse = new PointF(e.X, e.Y);
            if (!this.Focused)
            {
                Focus();
            }

            // reset focus
            this.FocusManager.FocusedNode = null;

            m_controller.OnMouseDown(e);
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the mouse up event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_controller.OnMouseUp(e);
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the mouse move event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            m_ptCurMouse = new PointF(e.X, e.Y);
            if (!this.dragging)
                m_controller.OnMouseMove(e);

            if (m_bResize || !m_controller.ActiveTool.InAction || m_bUpdate)
            {
                m_bNeedDocumentRefresh = true;
                m_bResize = true;
            }
            else
            {
                // update ruler's state
                m_bNeedDocumentRefresh = false;
            }

            if (this.ShowRulers)
            {
               
                m_bNeedVRulerRefresh = false;
                m_bNeedHRulerRefresh = true;
             
                // if there are no changes in document and mouse is captured ->
                // reflect current mouse position on rulers
                UpdateHorizontalRuler(this.Controller.ActiveTool.WorkRect);

                m_bNeedHRulerRefresh = false;
                m_bNeedVRulerRefresh = true;
                UpdateVerticalRuler(this.Controller.ActiveTool.WorkRect);

                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = true;
            }

            m_bNeedDocumentRefresh = true;
            m_bResize = false;
            m_bUpdate = false;

            if (bDebug)
                Invalidate();

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            m_controller.OnClick(e);
            base.OnClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DoubleClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            m_controller.OnDoubleClick(e);
            base.OnDoubleClick(e);
        }

        /// <summary>
        /// Raises the Lost Focus event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnLostFocus(EventArgs e)
        {
            if (this.ShowRulers)
            {
                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = true;

                UpdateHRulerBounds();
                UpdateVRulerBounds();
            }

            m_bNeedDocumentRefresh = true;
            Invalidate(this.ClientRectangle);
            Update();

            base.OnLostFocus(e);
        }

        /// <summary>
        /// Raises the Got Focus event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnGotFocus(EventArgs e)
        {
            if (this.ShowRulers)
            {
                m_bNeedHRulerRefresh = true;
                m_bNeedVRulerRefresh = true;

                UpdateHRulerBounds();
                UpdateVRulerBounds();
            }

            m_bNeedDocumentRefresh = true;
            Invalidate(this.ClientRectangle);
            Update();

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Returns an object that represents a service provided by the <see cref="T:System.ComponentModel.Component"/> or by its <see cref="T:System.ComponentModel.Container"/>.
        /// </summary>
        /// <param name="service">A service provided by the <see cref="T:System.ComponentModel.Component"/>.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents a service provided by the <see cref="T:System.ComponentModel.Component"/>, or null if the <see cref="T:System.ComponentModel.Component"/> does not provide the specified service.
        /// </returns>
        protected override object GetService(Type service)
        {
            if (service == typeof(Model))
            {
                return this.Model;
            }
            if (service == typeof(View))
            {
                return this.View;
            }
            if (service == typeof(IPropertyObserver))
            {
                return this;
            }
            if (service == typeof(IViewer))
            {
                return this;
            }
            if (service == typeof(DiagramController))
            {
                return this.Controller;
            }
            if (service == typeof(EventSink) || service == typeof(ViewerEventSink)
                || service == typeof(DiagramViewerEventSink))
            {
                return this.EventSink;
            }

            return base.GetService(service);
        }

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Window.WM_MOUSEACTIVATE)
            {
                m_bUpdate = true;
            }

            // refresh explicitly client area after hiding overlapping window
            if (m.Msg == Window.WM_IME_SETCONTEXT || m.Msg == Window.WM_MOUSEHOVER)
            {
                if (this.ShowRulers)
                {
                    m_bNeedHRulerRefresh = true;
                    m_bNeedVRulerRefresh = true;

                    UpdateHRulerBounds();
                    UpdateVRulerBounds();

                    m_bNeedDocumentRefresh = true;
                    Invalidate(this.ClientRectangle);
                    Update();
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Suspends the painting of the control until the <see cref="M:Syncfusion.Windows.Forms.ScrollControl.EndUpdate"/> method is called.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <remarks><para>When many paints are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.</para><para>
        /// Pass BeginUpdateOptions if you do not want to do a complete Refresh of the control and instead
        /// want to have certain regions of your control be invalidated or scroll the contents of control.</para>
        /// If you call BeginUpdate() and then later EndUpdate(), the control will know if a paint is pending and only
        /// refresh the control if a paint is pending. Calling ShouldPrepareUpdate, Invalidate or a WM_PAINT message during
        /// the BeginUpdate EndUpdate block will signal the control that a paint is pending.
        /// </remarks>
        /// <seealso cref="M:Syncfusion.Windows.Forms.ScrollControl.ShouldPrepareUpdate"/>
        /// <seealso cref="M:Syncfusion.Windows.Forms.ScrollControl.EndUpdate"/>
        public override void BeginUpdate(BeginUpdateOptions options)
        {
            if (this.Model != null)
            {
                this.Model.BeginUpdate();
            }

            base.BeginUpdate(options);
        }

        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </summary>
        /// <param name="update">true, if update is suspended.</param>
        /// <remarks>
        /// When many paint are made to the appearance of a control you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.
        /// </remarks>
        public override void EndUpdate(bool update)
        {
            base.EndUpdate(update);
            if (this.Model != null)
            {
                this.Model.EndUpdate();
            }
        }
        #endregion

        #region IPropertyContainer Members
        /// <summary>
        /// Gets the full name of the container.
        /// </summary>
        /// <value>The full name of the container.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FullContainerName
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the property container by name.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>Object that contains the name of the property container.</returns>
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
        /// <returns>Object containing the property container.</returns>
        protected virtual object GetPropertyContainer(string strPropertyContainerName)
        {
            return null;
        }
        #endregion

        #region IPropertyObserver Members
        /// <summary>
        /// Called when property is changing.
        /// </summary>
        /// <param name="strPropertyFullPath">The full path.</param>
        /// <param name="strPropertyName">The Name.</param>
        /// <param name="oldValue">The old value.</param>
        /// <returns>true, if property changing</returns>
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
        /// Called when property is changed.
        /// </summary>
        /// <param name="strPropertyFullPath">The full path.</param>
        /// <param name="strPropertyName">The property name.</param>
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
        #endregion

        #region Class helper methods
        /// <summary>
        /// Enable quite mode.
        /// </summary>
        /// <param name="bBringinEnable">Briging enable flag.</param>
        private void BeginQuiteMode(ref bool bBringinEnable)
        {
            Model model = this.Model;

            if (model != null)
            {
                model.HistoryManager.Pause();
                model.EventSink.Pause();
                bBringinEnable = model.LineBridgingEnabled;
                model.LineBridgingEnabled = false;
            }
        }

        /// <summary>
        /// Disable quite mode.
        /// </summary>
        /// <param name="bBringinEnable">Bridging enable flag.</param>
        private void EndQuiteMode(bool bBringinEnable)
        {
            Model model = this.Model;

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
                model.LineBridgingEnabled = bBringinEnable;
            }
        }

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
        private DiagramDocument GetDocumentFromStream(Stream strmDiagram)
        {
			DiagramDocument diagramDoc = new DiagramDocument(m_model, m_view);
            MVCDispose();
            DiagramDocument doc = null;
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                formatter.TypeFormat = FormatterTypeStyle.TypesAlways;
                formatter.Binder = new OldToNewDeserializationBinder();
                doc = (DiagramDocument)formatter.Deserialize(strmDiagram);
            }
            catch (Exception)
            {
                // try soap formatter
                try
                {
                    SoapFormatter soapFormatter = new SoapFormatter();
                    // rewind to begining
                    strmDiagram.Position = 0;
                    soapFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                    soapFormatter.TypeFormat = FormatterTypeStyle.TypesAlways;
                    soapFormatter.Binder = new OldToNewDeserializationBinder();
                    doc = (DiagramDocument)soapFormatter.Deserialize(strmDiagram);
                }
                catch (SerializationException e)
                {
					doc = diagramDoc;
                    MessageBox.Show(e.Message);
                }
            }
            finally
            {
                strmDiagram.Close();
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
            }

            return doc;
        }

        private void InitDiagram(DiagramDocument doc)
        {
            if (m_document == doc)
                return;

            Model document = doc.Model;			
            View view = doc.View;

            if (this.m_controller == null)
            {
                m_controller = CreateController();
            }

            if (view != null)
            {
                View = view;
                m_view.ClientRectangle = this.ClientRectangle;
            }

            if (document != null)
            {
				if (m_bInUpdate)
				{
					document.BeginUpdate();
					m_bInUpdate = false;
				}
                if (this.Model != null && this.DesignMode)
                {
                    this.Model.Merge(document, doc.ModelComponent == null);

                    if (doc.ModelComponent == null)
                    {
                        doc.Model.EventSink.PropertyChanged += new PropertyChangedEventHandler(ComponentEventSink_PropertyChanged);
                    }

                    Invalidate(true);
                }
                else
                {
                	m_document = null;
                    this.Model = document;
                }
            }

            m_controller.UpdateServiceReferences(this);

            // iterate through model nodes and update control node hosting control parent
            if (this.Model != null)
            {
                foreach (Node node in this.Model.Nodes)
                {
                    ControlNode nodeCtrl = node as ControlNode;

                    if (nodeCtrl != null)
                    {
                        nodeCtrl.HCParent = this;
                    }
                }
            }

            UpdateScrollRange();
            UpdateScrollBars();

            // set new value
            m_document = doc;
        }

        private void ComponentEventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            Model model = evtArgs.NodeAffected as Model;

            if (model != null)
            {
                Type tpModel = model.GetType();

                // update property value
                PropertyInfo property = tpModel.GetProperty(evtArgs.PropertyName);
                object value = property.GetValue(model, null);
                property.SetValue(this.Model, value, null);
            }
        }

        /// <summary>
        /// Draws the handles to graphics.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="nodesSelected">The nodes selected.</param>
        protected virtual void DrawHandles(Graphics grfx, NodeCollection nodesSelected)
        {
            // Draw handles around nodes in selection list.
            if (nodesSelected != null && nodesSelected.Count > 0)
            {
                foreach (Node nodeCur in nodesSelected)
                {
                    // HandleEditMode resultHandleEditMode = GetHandleEditMode( nodeCur );
                    HandleEditMode resultHandleEditMode = nodeCur.EditStyle.DefaultHandleEditMode;

                    // get cur node parent's transformations 
                    Matrix matrixParentsTransorm = GetParentsTransform(nodeCur);
                    
                    // save graphics state
                    GraphicsState stateSave = grfx.Save();
                    
                    // append parents transforms on given graphics
                    grfx.MultiplyTransform(matrixParentsTransorm);
                    HandleRenderer handleRenderer = new HandleRenderer();
                    handleRenderer.Render(grfx, resultHandleEditMode, nodeCur);                   

                    // restore graphics state
                    grfx.Restore(stateSave);
                }
            }
        }
        private Matrix GetParentsTransform(Node node)
        {
            if (node == null || node.Parent == null)
                throw new ArgumentNullException("node + parent");

            Matrix matrixToReturn = new Matrix();
            Matrix matrixTemp;
            ICompositeNode nodeParent = node.Parent;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model))
            {
                matrixTemp = ((Node)nodeParent).GetTransformations();
                matrixToReturn.Multiply(matrixTemp, MatrixOrder.Append);

                nodeParent = ((Node)nodeParent).Parent;
            }

            return matrixToReturn;
        }
        #endregion

        #region Helper methods
        private void Align(BoxOrientation orientation)
        {
            NodeCollection lstSelections = this.Controller.SelectionList;

            if (lstSelections != null && lstSelections.Count > 1)
            {
                MeasureUnits units = MeasureUnits.Pixel;
                Node firstNode = lstSelections.First;
                SizeF szOffset = new SizeF();

                // get first node bounds parameters
                RectangleF rcFirst = ((IUnitIndependent)firstNode).GetBoundingRectangle(units, true);

                if (firstNode != null)
                {
                    // start record changes to history
                    this.Model.HistoryManager.StartAtomicAction("Set Align");

                    // begin synchronization
                    this.Model.LinkManager.BeginSynchronization();
                    RectangleF rcRect;

                    foreach (Node node in lstSelections)
                    {
                        if (node != firstNode)
                        {
                            // get node bounds parameters
                            rcRect = ((IUnitIndependent)node).GetBoundingRectangle(units, true);

                            // calc align pin offset
                            switch (orientation)
                            {
                                case BoxOrientation.Left:
                                    szOffset = new SizeF(rcFirst.Left - rcRect.Left, 0);
                                    break;
                                case BoxOrientation.Right:
                                    szOffset = new SizeF(rcFirst.Right - rcRect.Right, 0);
                                    break;
                                case BoxOrientation.Top:
                                    szOffset = new SizeF(0, rcFirst.Top - rcRect.Top);
                                    break;
                                case BoxOrientation.Bottom:
                                    szOffset = new SizeF(0, rcFirst.Bottom - rcRect.Bottom);
                                    break;
                                case BoxOrientation.HCenter:
                                    PointF ptFirstCenter = Geometry.CenterPoint(rcFirst);
                                    PointF ptNodeCenter = Geometry.CenterPoint(rcRect);
                                    szOffset = new SizeF(ptFirstCenter.X - ptNodeCenter.X, 0);
                                    break;
                                case BoxOrientation.VCenter:
                                    ptFirstCenter = Geometry.CenterPoint(rcFirst);
                                    ptNodeCenter = Geometry.CenterPoint(rcRect);
                                    szOffset = new SizeF(0, ptFirstCenter.Y - ptNodeCenter.Y);
                                    break;
                            }

                            // set new pin position
                            node.Translate(szOffset.Width, szOffset.Height);
                        }
                    }

                    // end synchronization
                    this.Model.LinkManager.EndSynchronization();

                    // stop record
                    this.Model.HistoryManager.EndAtomicAction();
                }
            }
        }

        private void Spacing(SpacingDirection direction)
        {
            NodeCollection lstSelections = this.Controller.SelectionList;
            int selectionCount = lstSelections.Count;

            if (lstSelections != null && selectionCount > 2)
            {
                MeasureUnits units = MeasureUnits.Pixel;
                Node firstNode = lstSelections.First;
                Node lastNode = lstSelections.Last;

                SizeF szOffset = new SizeF();
                
                // get first node bounds parameters
                RectangleF rcFirst = ((IUnitIndependent)firstNode).GetBoundingRectangle(units, false);
                RectangleF rcLast = ((IUnitIndependent)lastNode).GetBoundingRectangle(units, false);

                if (firstNode != null && lastNode != null)
                {
                    // start record changes to history
                    this.Model.HistoryManager.StartAtomicAction("Set Spacing");
                    RectangleF rcRect;
                    PointF ptCenterPoint;

                    PointF ptFirstCenter = Geometry.CenterPoint(rcFirst);
                    PointF ptLastCenter = Geometry.CenterPoint(rcLast);

                    float fSpacing = 0f;

                    if (direction == SpacingDirection.Across)
                    {
                        fSpacing = (ptLastCenter.X - ptFirstCenter.X) / (selectionCount - 1);
                    }
                    else if (direction == SpacingDirection.Down)
                    {
                        fSpacing = (ptLastCenter.Y - ptFirstCenter.Y) / (selectionCount - 1);
                    }

                    for (int i = 1, nLength = selectionCount - 1; i < nLength; i++)
                    {
                        Node node = lstSelections[i];

                        // get node bounds parameters
                        rcRect = ((IUnitIndependent)node).GetBoundingRectangle(units, false);
                        ptCenterPoint = Geometry.CenterPoint(rcRect);

                        if (direction == SpacingDirection.Across)
                        {
                            szOffset.Width += ptFirstCenter.X + fSpacing - ptCenterPoint.X;
                        }
                        else if (direction == SpacingDirection.Down)
                        {
                            szOffset.Height += ptFirstCenter.Y + fSpacing - ptCenterPoint.Y;
                        }
                        ptFirstCenter = ptCenterPoint;

                        // set new pin position
                        node.Translate(szOffset.Width, szOffset.Height);
                    }

                    // stop record
                    this.Model.HistoryManager.EndAtomicAction();
                }
            }
        }

        /// <summary>
        /// Processes Tab key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if TAB key is pressed.</returns>
        private bool ProcessTabKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;

            if (!IsEditing() && CanPerformMoving())
            {
                bSuccess = true;

                if (bShiftKeyPressed)
                {
                    this.FocusManager.MoveFocusBackward();
                }
                else
                {
                    this.FocusManager.MoveFocusForward();
                }
            }

            UpdateView();
            return bSuccess;
        }

        /// <summary>
        /// Processes Escape key.
        /// </summary>
        /// <returns>true, if process ESCAPE key.</returns>
        private bool ProcessEscapeKey()
        {
            bool bSuccess = false;

            this.FocusManager.FocusedNode = null;

            // If we are editing text -- close editor.
            if (this.Controller.TextEditor.IsEditing)
            {
                this.Controller.TextEditor.EndEdit(false);
                bSuccess = true;
            }
            else if (this.Controller.ActiveTool.InAction)
            {
                // update viewer invalid rect cause it will be empty after Tool.Abort()
                this.Controller.UpdateInfo.UpdateRefreshRect(this.Controller.ActiveTool.WorkRect);

                Tool toolPreceding = this.Controller.ActiveTool.Abort();

                // update viewer
                UpdateView();

                if (this.ShowRulers)
                {
                    // update rulers
                    UpdateHorizontalRuler(System.Drawing.Rectangle.Empty);
                    UpdateVerticalRuler(System.Drawing.Rectangle.Empty);
                }

                this.Controller.ActiveTool = toolPreceding;
                bSuccess = true;
            }
            else if (this.Controller.SelectionList.Count > 0)
            {
                this.Controller.SelectionList.Clear();
                bSuccess = true;
            }

            return bSuccess;
        }

        /// <summary>
        /// Processes Right key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if process right key.</returns>
        private bool ProcessRightKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;
            if (this.Controller.View.SelectionList.IsEmpty)
                return bSuccess;

            if (bShiftKeyPressed)
                bSuccess = this.NudgeRight();
            else
                bSuccess = this.MoveNodes(GetSelectionList(), this.View.Grid.HorizontalSpacing / 2, 0f, MeasureUnits.Pixel);

            return bSuccess;
        }

        /// <summary>
        /// Processes Left key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if process left key.</returns>
        private bool ProcessLeftKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;
            if (this.Controller.View.SelectionList.IsEmpty)
                return bSuccess;

            if (bShiftKeyPressed)
            {
                bSuccess = this.NudgeLeft();
            }
            else
            {
                bSuccess =
                    MoveNodes(GetSelectionList(), -this.View.Grid.HorizontalSpacing / 2, 0f, MeasureUnits.Pixel);
            }

            return bSuccess;
        }

        /// <summary>
        /// Processes Down key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if process down key.</returns>
        private bool ProcessDownKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;
            if (this.Controller.View.SelectionList.IsEmpty)
                return bSuccess;

            if (bShiftKeyPressed)
            {
                bSuccess = NudgeDown();
            }
            else
            {
                bSuccess = MoveNodes(GetSelectionList(), 0f, this.View.Grid.VerticalSpacing / 2, MeasureUnits.Pixel);
            }

            return bSuccess;
        }

        /// <summary>
        /// Processes Up key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if process Up key.</returns>
        private bool ProcessUpKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;
            if (this.Controller.View.SelectionList.IsEmpty)
                return bSuccess;

            if (bShiftKeyPressed)
            {
                bSuccess = NudgeUp();
            }
            else
            {
                bSuccess = MoveNodes(GetSelectionList(), 0f, -this.View.Grid.VerticalSpacing / 2, MeasureUnits.Pixel);
            }

            return bSuccess;
        }

        /// <summary>
        /// Processes Enter key.
        /// </summary>
        /// <param name="bShiftKeyPressed">Indicates whether SHIFT key is pressed.</param>
        /// <returns>true, if process ENTER key.</returns>
        private bool ProcessEnterKey(bool bShiftKeyPressed)
        {
            bool bSuccess = false;

            // Add Focused Node to SelectionList
            Node nodeFocused = this.FocusManager.FocusedNode;

            if (nodeFocused != null)
            {
                NodeCollection nodesSelected = this.View.SelectionList;

                if (bShiftKeyPressed)
                {
                    if (nodesSelected.Contains(nodeFocused))
                        nodesSelected.Remove(nodeFocused);
                    else
                    {
                        if (EditStyle.CanSelect(nodeFocused))
                        {
                            nodesSelected.Add(nodeFocused);
                        }
                    }
                }
                else
                {
                    nodesSelected.Clear();

                    if (EditStyle.CanSelect(nodeFocused))
                    {
                        nodesSelected.Add(nodeFocused);
                    }

                    this.FocusManager.FocusedNode = null;
                }

                bSuccess = true;
            }

            return bSuccess;
        }

        /// <summary>
        /// Checks if move operation can be done.
        /// </summary>
        /// <returns>value specifying if we can perform move operation</returns>
        /// <remarks>
        /// If we are editing text from Text or RichText node skip moving
        /// </remarks>
        protected bool CanPerformMoving()
        {
            bool bCanPerformMoving;

            bCanPerformMoving = !IsEditing() && !this.Controller.ActiveTool.InAction;

            return bCanPerformMoving;
        }

        /// <summary>
        /// Determines whether this instance is editing.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsEditing()
        {
            bool bIsEditing;

            // If we are editing text nodes -- skip moving.
            // 1 - check TextEditor state
            bIsEditing = this.Controller.TextEditor.IsEditing;

            if (!bIsEditing)
            {
                // 2 - check ControlNodeTool
                // if( ( this.Controller.ActiveTool is ControlNodeTool ) && ( ( ( ControlNodeTool )this.Controller.ActiveTool ).IsEditing ) )
                //     bIsEditing = true;
            }

            return bIsEditing;
        }

        private void UpdateDefaultContextMenu()
        {
            if (this.DefaultContextMenuEnabled && m_model != null)
            {
                foreach (ToolStripItem menuItem in this.ContextMenuStrip.Items)
                {
                    ToolStripMenuItem item = menuItem as ToolStripMenuItem;
                    if (item != null)
                    {
                        switch (item.Name)
                        {
                            case "m_ruler":
                                item.Checked = this.ShowRulers;
                                break;
                            case "m_boundaryConstraints":
                                item.Checked = this.Model.BoundaryConstraintsEnabled;
                                break;
                            case "m_sizeToContent":
                                item.Checked = this.Model.SizeToContent;
                                break;
                            case "m_cut":
                                item.Enabled = this.CanCut;
                                break;
                            case "m_copy":
                                item.Enabled = this.CanCopy;
                                break;
                            case "m_paste":
                                item.Enabled = this.CanPaste;
                                break;
                            case "m_delete":
                                item.Enabled = this.Controller.SelectionList.Count > 0;
                                break;
                            case "m_selectAll":
                                item.Enabled = this.Model.Nodes.Count > 0;
                                break;
                            case "m_align":
                                foreach (ToolStripMenuItem drpDwnItem in item.DropDownItems)
                                    drpDwnItem.Enabled = this.Controller.SelectionList.Count >= 2;
                                break;
                            case "m_order":
                            case "m_rotate":
                            case "m_flip":
                                foreach (ToolStripMenuItem drpDwnItem in item.DropDownItems)
                                    drpDwnItem.Enabled = this.Controller.SelectionList.Count > 0;
                                break;
                            case "m_grouping":
                                foreach (ToolStripMenuItem drpDwnItem in item.DropDownItems)
                                {
                                    if (drpDwnItem.ToString() == "Group")
                                        drpDwnItem.Enabled = this.Controller.SelectionList.Count > 1;
                                    else
                                    {
                                        bool isGroup = false;
                                        foreach (Node node in this.Controller.SelectionList)
                                        {
                                            if (node is Group)
                                            {
                                                isGroup = true;
                                            }
                                        }
                                        drpDwnItem.Enabled = isGroup;
                                    }
                                }
                                break;
                            case "m_resize":
                                foreach (ToolStripMenuItem drpDwnItem in item.DropDownItems)
                                    drpDwnItem.Enabled = this.Controller.SelectionList.Count > 1;
                                break;
                        }
                    }
                }               
            }
        }
        #endregion
    }

    /// <summary>
    /// Diagram initialized event handler delegate
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="evtArgs">Event args</param>
    public delegate void DiagramInitializedEventHandler(object sender, EventArgs evtArgs);

    /// <summary>
    /// Direction enums
    /// </summary>
    internal enum Direction
    {
        /// <summary>
        /// Move forward
        /// </summary>
        Forward = 0,

        /// <summary>
        /// Move backward
        /// </summary>
        Backward
    }

    /// <summary>
    /// Node render location comparer.
    /// </summary>
    internal sealed class NodeRenderLocationComparer
        : IComparer
    {
        #region IComparable Members
        public int Compare(object obj1, object obj2)
        {
            if ((obj1 == null) || !(obj1 is Node))
                throw new NullReferenceException("Comparing object is not of Node type.");

            if ((obj2 == null) || !(obj2 is Node))
                throw new NullReferenceException("Comparing object is not of Node type.");

            Node node1 = (Node)obj1;
            Node node2 = (Node)obj2;

            PointF ptRenderOrigin1 = PointF.Empty;
            ptRenderOrigin1.X = node1.PinPoint.X - node1.PinPointOffset.Width;
            ptRenderOrigin1.Y = node1.PinPoint.Y - node1.PinPointOffset.Height;

            PointF ptRenderOrigin2 = PointF.Empty;
            ptRenderOrigin2.X = node2.PinPoint.X - node2.PinPointOffset.Width;
            ptRenderOrigin2.Y = node2.PinPoint.Y - node2.PinPointOffset.Height;

            int nValueToReturn = 1;

            if ((ptRenderOrigin1.Y - ptRenderOrigin2.Y) == 0)
            {
                if ((ptRenderOrigin1.X - ptRenderOrigin2.X) < 0)
                {
                    nValueToReturn = -1;
                }
            }
            else if ((ptRenderOrigin1.Y - ptRenderOrigin2.Y) < 0)
            {
                nValueToReturn = -1;
            }

            return nValueToReturn;
        }
        #endregion
    }
}