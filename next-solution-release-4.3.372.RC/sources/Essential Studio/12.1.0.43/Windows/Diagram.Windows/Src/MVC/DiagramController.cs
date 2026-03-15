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
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices.WinAPI;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Implements a generic diagram controller containing a default set of tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The DiagramController class implements a concrete controller with a generic
    /// diagramming user interface. It overrides the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.Initialize"/>
    /// method and registers all available tools in the Diagram Windows assembly. 
    /// A tool is an object that implements a distinct functionality for the 
    /// controller. Tools can be added to the controller using the 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.RegisterTool"/> method 
    /// in order to customize the behavior of the controller.
    /// </para>
    /// <para>
    /// The DiagramController is responsible for coordinating the activation and
    /// deactivation of tools. The <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.ActivateTool(Syncfusion
    /// .Windows.Forms.Diagram.Tool)"/> 
    /// method activates a specified tool. The 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.DeactivateTool"/>
    /// method deactivates a specified tool. Tools can be accessed by name with the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.GetTool"/> method.
    /// </para>
    /// <para>
    /// The DiagramController captures all mouse events and performs hit testing on
    /// the diagram. Nodes, ports, and vertices hit by mouse movements are
    /// tracked by the controller. Tools can access this hit testing
    /// information in the controller. In other words, the controller provides
    /// basic hit testing services to tools so that each tool doesn't need to
    /// implement (and possible duplicate) that hit testing logic. The following
    /// properties and methods provide access to the hit testing state information:
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.MouseLocation"/>,
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.NodesHit"/>,
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    [Serializable]
    public class DiagramController
        : Controller,
          IMouseController
    {
        #region Class members
        private Node m_nodeToInsert = null;
        private bool m_bIsPaletteNodeClicked;
        private RenderingHelperStyle m_resizingStyle;
        private RenderingHelperStyle m_draggingStyle;
        private RenderingHelperStyle m_rotatingStyle;
        private bool m_bInPlaceEditing = true;
        private Guides m_guides;
        private bool m_bAllowCopyAtCtrlDrag = true;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramController"/> class.
        /// </summary>
        public DiagramController()
            : base()
        {
            m_szPasteOffset = new SizeF(10, 10);
            this.tools = new ArrayList();
            this.toolTable = new Hashtable();
            this.guid = System.Guid.NewGuid();
            scrollEventReceivers = new ArrayList();
            m_textEditor = new TextEditor(this);
            m_inPlaceEditor = new InPlaceEditor(this);
            RegisterStandardTools();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramController"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public DiagramController(Syncfusion.Windows.Forms.Diagram.View view)
        {
            m_szPasteOffset = new SizeF(10, 10);
            this.guid = System.Guid.NewGuid();
            m_inPlaceEditor = new InPlaceEditor(this);
            m_textEditor = new TextEditor(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the document needs refresh.
        /// </summary>
        internal bool NeedDocumentRefresh
        {
            get 
            { 
                return m_bNeedDocumentRefresh;
            }
            set
            {
                if (value != m_bNeedDocumentRefresh)
                {
                    m_bNeedDocumentRefresh = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the palette node is clicked.
        /// </summary>
        internal bool IsPaletteNodeClicked
        {
            get
            {               
                return m_bIsPaletteNodeClicked;
            }
            set
            {
                if (m_bIsPaletteNodeClicked != value)
                {
                    m_bIsPaletteNodeClicked = value;                   
                }
            }
        }
       
        /// <summary>
        /// Gets or sets a node to insert the model through InsertNodeTool
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Node NodeToInsert
        {
            get
            {
                if (m_nodeToInsert == null)
                {
                    m_nodeToInsert = new Rectangle(0, 0, 100, 50);
                }
                return m_nodeToInsert;
            }
            set
            {
                if (m_nodeToInsert != value)
                    m_nodeToInsert = value;
            }
        }
       
        /// <summary>
        /// Gets or sets the paste offset that indicate offset of next paste object.
        /// </summary>
        /// <value>The paste offset.</value>
        [Browsable(false)]
        public SizeF PasteOffset
        {
            get { return m_szPasteOffset; }
            set { m_szPasteOffset = value; }
        }

        /// <summary>
        /// Gets the text editor instance.
        /// </summary>
        /// <value>The text editor.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextEditor TextEditor
        {
            get
            {
                if (m_textEditor == null)
                {
                    m_textEditor = new TextEditor(this);
                }

                return m_textEditor;
            }
        }

        /// <summary>
        /// Gets the in place editor instance.
        /// </summary>
        /// <value>The text editor.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InPlaceEditor InPlaceEditor
        {
            get
            {
                if (m_inPlaceEditor == null)
                {
                    m_inPlaceEditor = new InPlaceEditor(this);
                }

                return m_inPlaceEditor;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether in place editing is enabled or not.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InPlaceEditing
        {
            get { return m_bInPlaceEditing; }
            set
            {
                if (value != m_bInPlaceEditing)
                    m_bInPlaceEditing = value;
            }
        }

        /// <summary>
        /// Gets the diagram Guides.
        /// </summary>
        [Browsable(true)]
        [Category("Misc")]
        [Description("Guides")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guides Guides
        {
            get
            {
                if (m_guides == null)
                {
                    m_guides = new Guides();
                    m_guides.UpdateServiceReferences(this);
                }

                return m_guides;
            }
        }

        /// <summary>
        /// Gets the list of nodes that were hit during the last received mouse event.
        /// </summary>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.DiagramController.MouseLocation"/>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NodeCollection NodesHit
        {
            get
            {
                if (m_nodesHit == null)
                    m_nodesHit = new NodeCollection();

                return m_nodesHit;
            }
        }

        /// <summary>
        /// Gets or sets the diagram active tool.
        /// </summary>
        /// <value>The active tool.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Tool ActiveTool
        {
            get
            {
                if (m_toolActive == null)
                    m_toolActive = GetTool("SelectTool");

                return m_toolActive;
            }
            set
            {
                if (m_toolActive != value)
                {
                    if (m_toolActive != null)
                    {
                        m_toolActive.DeactivateTool();

                        OnToolDeactivated(new ToolEventArgs(m_toolActive));
                    }

                    m_toolActive = value;

                    // select tool must be active
                    if (m_toolActive == null)
                    {
                        m_toolActive = GetTool("SelectTool");
                    }

                    if (m_toolActive != null)
                    {
                        m_toolActive.ActivateTool();

                        if (this.MouseLocation != PointF.Empty)
                            Cursor.Current = m_toolActive.ActionCursor;

                        OnToolActivated(new ToolEventArgs(m_toolActive));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the System.Windows.Forms.Control that owns the view that this
        /// controller is attached to.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual Control ParentControl
        {
            get
            {
                if (this.Viewer != null)
                {
                    // return this.View.ParentControl;
                }
                return (Control)m_viewer;
            }
        }

        /// <summary>
        /// Gets or sets cursor to use inside the view.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cursor Cursor
        {
            get 
            { 
                return this.Viewer.Cursor; 
            }
            set
            {
                if (this.Viewer != null)
                {
                    this.Viewer.Cursor = value;
                }
            }
        }

        /// <summary>
        /// Gets the last known location of the mouse pointer.
        /// </summary>
        /// <remarks>
        /// This property is updated each time the controller receives a mouse event:
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.DiagramController.NodesHit"/>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF MouseLocation
        {
            get { return mouseLocation; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mouse tracking is enabled.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool MouseTrackingEnabled
        {
            get
            {
                return mouseTrackingEnabled;
            }
            set
            {
                mouseTrackingEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets resizing style of rendering helper
        /// </summary>
        [DefaultValue(RenderingHelperStyle.GhostCopy)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RenderingHelperStyle ResizingStyle
        {
            get
            {
                return m_resizingStyle;
            }
            set
            {
                if (m_resizingStyle != value)
                    m_resizingStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets dragging style of rendering helper
        /// </summary>
        [DefaultValue(RenderingHelperStyle.GhostCopy)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RenderingHelperStyle DraggingStyle
        {
            get
            {
                return m_draggingStyle;
            }
            set
            {
                if (m_draggingStyle != value)
                    m_draggingStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets rotating style of rendering helper
        /// </summary>
        [DefaultValue(RenderingHelperStyle.GhostCopy)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RenderingHelperStyle RotatingStyle
        {
            get
            {
                return m_rotatingStyle;
            }
            set
            {
                if (m_rotatingStyle != value)
                    m_rotatingStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the controller allows copying a node while pressing Ctrl+drag or Ctrl+Shift+drag.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowCopyAtCtrlDrag
        {
            get
            {
                return m_bAllowCopyAtCtrlDrag;
            }
            set
            {
                if (m_bAllowCopyAtCtrlDrag != value)
                    m_bAllowCopyAtCtrlDrag = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            Syncfusion.Windows.Forms.ScrollControl parentScrollCtl = this.ParentControl as Syncfusion.Windows.Forms.ScrollControl;

            // sign to scrolling change events
            if (parentScrollCtl != null)
            {
                parentScrollCtl.VerticalScroll += new ScrollEventHandler(this.OnVerticalScroll);
                parentScrollCtl.HorizontalScroll += new ScrollEventHandler(this.OnHorizontalScroll);
            }

            if (m_inPlaceEditor == null)
                m_inPlaceEditor = new InPlaceEditor(this);

            InitialiizeController();
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        public void Uninitialize()
        {
            // Disconnect event handlers from parent control.
            Syncfusion.Windows.Forms.ScrollControl parentScrollCtl = this.ParentControl as Syncfusion.Windows.Forms.ScrollControl;

            if (parentScrollCtl != null)
            {
                // Remove scrolling change events
                parentScrollCtl.VerticalScroll -= new ScrollEventHandler(this.OnVerticalScroll);
                parentScrollCtl.HorizontalScroll -= new ScrollEventHandler(this.OnHorizontalScroll);
            }

            if (m_inPlaceEditor != null)
                m_inPlaceEditor.Dispose();

            UninitialiizeController();
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Initializes the controller.
        /// </summary>
        protected virtual void InitialiizeController()
        { 
        }

        /// <summary>
        /// Uninitializes the controller.
        /// </summary>
        protected virtual void UninitialiizeController()
        { 
        }

        /// <summary>
        /// Raise the tool activated event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ToolEventArgs"/> instance containing the event data.</param>
        protected virtual void OnToolActivated(ToolEventArgs evtArgs)
        {
            DiagramViewerEventSink evtSink =
                ((Controls.Diagram)this.ParentControl).EventSink as DiagramViewerEventSink;

            if (evtSink != null)
            {
                evtSink.RaiseToolActivated(evtArgs);
            }
        }

        /// <summary>
        /// Raise the tool deactivated event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ToolEventArgs"/> instance containing the event data.</param>
        protected virtual void OnToolDeactivated(ToolEventArgs evtArgs)
        {
            DiagramViewerEventSink evtSink =
                ((Controls.Diagram)this.ParentControl).EventSink as DiagramViewerEventSink;

            if (IsPaletteNodeClicked)
                IsPaletteNodeClicked = false;
            if (evtSink != null)
            {
                evtSink.RaiseToolDeactivated(evtArgs);
            }
        }

        /// <summary>
        /// Called when a mouse down event occurs.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// This method updates the current mouse location and then performs hit testing
        /// for handles and nodes.
        /// </remarks>
        public virtual void OnMouseDown(MouseEventArgs evtArgs)
        {
            if (this.Model == null) return;

            m_previousSelected = new NodeCollection(this.SelectionList, false);

            if (m_textEditor != null && m_textEditor.IsEditing)
            {
                m_textEditor.EndEdit(true);
            }

            //Deactivate the InPlaceEditor if it is active
            if (m_inPlaceEditor != null && m_inPlaceEditor.Visible)
            {
                m_inPlaceEditor.EndEdit();
            }

            DeactivateControlNode();

            if (this.ActiveTool != null)
                this.ActiveTool = this.ActiveTool.ProcessMouseDown(evtArgs);

            // update cursor
            //Fix SD209, no need to update the diagram(viewer) cursor.
            //We can get the current cursor of viewer by ActiveTool.ActionCursor

            //this.Viewer.Cursor = this.ActiveTool.CurrentToolCursor;
        }

        /// <summary>
        /// Called when a mouse move event occurs.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// This method updates the current mouse location and then performs hit testing
        /// for handles and nodes.
        /// </remarks>
        public virtual void OnMouseMove(MouseEventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.IsPaletteNodeClicked)
            {
                using (Bitmap bmpNode = new Bitmap(18, 18))
                {
                    using (Graphics gfx = Graphics.FromImage(bmpNode))
                    {
                        gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                        Node node = NodeToInsert.Clone() as Node;
                        node.Size = new SizeF(16, 16);
                        node.Draw(gfx, true);
                    }
                    using (Bitmap bmpCursor = new Bitmap(32, 32))
                    {
                        using (Graphics gfx = Graphics.FromImage(bmpCursor))
                        {
                            gfx.DrawRectangle(Pens.Transparent, new System.Drawing.Rectangle(0, 0, 32, 32));
                            gfx.DrawLine(Pens.Black, new Point(6, 0), new Point(6, 12));
                            gfx.DrawLine(Pens.Black, new Point(0, 6), new Point(12, 6));
                            gfx.DrawImage(bmpNode, new Point(8, 8));
                        }
                        if (this.ActiveTool.CurrentToolCursor != Cursors.No)
                        {                            
                            if (this.m_bUpdateCursor)
                                Cursor.Current = ImageCursor.CreateCursor(bmpCursor, 6, 6);
                            this.m_bUpdateCursor = false;
                        }
                        else
                            this.m_bUpdateCursor = true;                            
                    }
                }               
            }          
            mouseLocation = ConvertToModelCoordinates(new PointF(evtArgs.X, evtArgs.Y));

            if (this.ActiveTool != null && !this.ActiveTool.InAction)
            {
                // assign m_nodeHit new value
                NodeCollection nodes = this.GetAllNodesAtPoint(this.Model, this.MouseLocation, false);

                // if mouse is not over m_nodeHit -> send it MouseLeave
                if (m_nodeHit != null && (!nodes.Contains(m_nodeHit) || m_nodeHit != nodes.First))
                {
                    this.Viewer.EventSink.RaiseNodeMouseLeave(new NodeMouseEventArgs(m_nodeHit));
                    ((IDispatchNodeEvents)m_nodeHit).MouseLeave(evtArgs);
                    m_nodeHit = null;
                }

                // if mouse is over some node -> send it MouseEnter
                if (!nodes.IsEmpty && m_nodeHit != nodes.First)
                {
                    this.Viewer.EventSink.RaiseNodeMouseEnter(new NodeMouseEventArgs(nodes.First));
                    ((IDispatchNodeEvents)nodes.First).MouseEnter(evtArgs);
                    m_nodeHit = nodes.First;
                }

                m_nodesHit = nodes;
            }

            // process mouse move for active tool
            ProcessToolMouseMove(evtArgs);
        }

        /// <summary>
        /// Called when a mouse up event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// This method updates the current mouse location and then performs hit testing
        /// for handles and nodes.
        /// </remarks>
        public virtual void OnMouseUp(MouseEventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.IsPaletteNodeClicked)
            {
                ImageCursor.Destroy();
                this.IsPaletteNodeClicked = false;
                this.m_bUpdateCursor = true;
            }
            if (this.ActiveTool != null)
            {
                this.ActiveTool = this.ActiveTool.ProcessMouseUp(evtArgs);
                this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Called when a mouse leave event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        public virtual void OnMouseLeave(EventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (!this.m_bUpdateCursor)
            {
                this.m_bUpdateCursor = true;
                ImageCursor.Destroy();
            }
            if (m_nodesHit != null)
            {
                // assign m_nodeHit new value
                NodeCollection nodes = this.GetAllNodesAtPoint(this.Model, this.MouseLocation);

                // if mouse is not over m_nodeHit -> send it MouseLeave
                if (m_nodeHit != null && nodes.Contains(m_nodeHit))
                {
                    this.Viewer.EventSink.RaiseNodeMouseLeave(new NodeMouseEventArgs(m_nodeHit));
                    ((IDispatchNodeEvents)m_nodeHit).MouseLeave(evtArgs);
                    m_nodeHit = null;
                }
            }

            if (this.ActiveTool != null && !this.ActiveTool.InAction)
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Called when a click event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// Iterates through the nodes hit by the click event and notifies them
        /// of the click by calling the 
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents.Click"/>
        /// method.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents"/>
        /// </remarks>
        public virtual void OnClick(EventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.ActiveTool != null)
                this.ActiveTool.ProcessClick();

            NodeCollection nodes = this.GetAllNodesAtPoint(this.Model, this.MouseLocation, true);
            if (nodes == null || nodes.Count == 0)
                return;

            Node nodeHit = nodes.First;
            ControlNode ctrlNode = nodeHit as ControlNode;
			if (ctrlNode == null)
            {
                ICompositeNode container = nodeHit as ICompositeNode;
                if (container != null)
                {
                   ctrlNode = GetControlNode(container);
                }                
            }
            if (((this.ActiveTool is MoveTool) && !((MoveTool)this.ActiveTool).Moved) &&           
                (ctrlNode != null) &&
                ((ctrlNode.ActivateStyle == ActivateStyle.Click) ||
                (ctrlNode.ActivateStyle == ActivateStyle.ClickPassThrough) ||
                (ctrlNode.ActivateStyle == ActivateStyle.SelectedClick)))
            {
                if (ctrlNode.ActivateStyle == ActivateStyle.SelectedClick)
                {
                    // activate if node is selected.
                    if (m_previousSelected != null && m_previousSelected.Contains(ctrlNode))
                        AcivateControlNode(ctrlNode);
                }
                else
                {
                    AcivateControlNode(ctrlNode);
                }

                // manually send hosting control mouse down message
                if (m_ctrlNodeActive != null && ctrlNode.ActivateStyle == ActivateStyle.ClickPassThrough)
                {
                    if (m_ctrlNodeActive.HostingControl.IsHandleCreated)
                    {
                        IntPtr hwnd = m_ctrlNodeActive.HostingControl.Handle;
                        int wparam = 0x0001; // MK_LBUTTON
                        PointF ptScene = ConvertFromModelToClientCoordinates(mouseLocation);
                        Point ptscreen =
                            ((Control)this.Viewer).PointToScreen(new Point((int)ptScene.X, (int)ptScene.Y));
                        Point ptclient = m_ctrlNodeActive.HostingControl.PointToClient(ptscreen);
                        int lparam = Macros.MAKELPARAM(ptclient.X, ptclient.Y);
                        if (m_ctrlNodeActive.HostingControl.HasChildren)
                        {
                            Control childCtrl = m_ctrlNodeActive.HostingControl.GetChildAtPoint(ptclient);
                            if (childCtrl != null)
                            {
                                hwnd = childCtrl.Handle;
                            }
                        }
                        Window.SendMessage(hwnd, 513, wparam, lparam); // WM_LBUTTONDOWN
                        Window.SendMessage(hwnd, 514, wparam, lparam); // WM_LBUTTONUP
                    }
                }
            }

            this.Viewer.EventSink.RaiseNodeClick(new NodeMouseEventArgs(nodeHit));
            ((IDispatchNodeEvents)nodeHit).Click(evtArgs);
        }

        /// <summary>
        /// Called when the drag enter event occurs.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        public virtual void OnDragEnter(DragEventArgs evtArgs)
        {
            if (m_textEditor != null && m_textEditor.IsEditing)
            {
                m_textEditor.EndEdit(true);
            }

            //Deactivate the InPlaceEditor if it is active
            if (m_inPlaceEditor != null && !m_inPlaceEditor.IsDisposed)
            {
                m_inPlaceEditor.EndEdit();
            }

            DeactivateControlNode();
        }

        /// <summary>
        /// Called when a double click event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// Iterates through the nodes hit by the double click event and notifies
        /// them of the click by calling the 
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents.DoubleClick"/>
        /// method.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IDispatchNodeEvents"/>
        /// </remarks>
        public virtual void OnDoubleClick(EventArgs evtArgs)
        {
            if (this.Model == null) return;

            bool bEditing = false;
            NodeCollection nodes = this.GetAllNodesAtPoint(this.Model, this.MouseLocation, true);
            Node nodeHit = null;
            if ((nodes != null) && (nodes.Count > 0))
                nodeHit = nodes.First;

            // if active tool is move tool and it moved its nodes -> no editor can be activated
            if (this.ActiveTool is LineSegmentTool || ((this.ActiveTool is MoveTool && !((MoveTool)this.ActiveTool).Moved)
                && (nodes == null || nodes.Count > 0)))
            {
                nodeHit = nodes.First;
                if (nodeHit != null)
                {
                    ControlNode ctrlNode = nodeHit as ControlNode;

                    if ((ctrlNode != null) &&
                        (ctrlNode.ActivateStyle == ActivateStyle.DoubleClick))
                    {
                        AcivateControlNode(ctrlNode);
                    }
                    else if (m_textEditor.IsEditable(nodeHit))
                    {
                        m_textEditor.BeginEdit(nodeHit, false);
                    }
                    else if(InPlaceEditing)
                    {
                        bool success = false;
                        if(this.View.SelectionList.Contains(nodeHit))
                            success = m_inPlaceEditor.IsEditable(nodeHit);
                        else
                            success = m_inPlaceEditor.IsEditable(this.SelectionList.First);
                        if (success)
                            m_inPlaceEditor.BeginEdit();
                    }
                }
            }
            else
            {
                if ((this.ActiveTool != null) && (!bEditing))
                    this.ActiveTool = this.ActiveTool.ProcessDoubleClick((MouseEventArgs)evtArgs);
            }

            if (nodeHit != null)
            {
                if (!(this.ActiveTool is PanTool) && !(this.ActiveTool is ZoomTool))
                {
                this.Viewer.EventSink.RaiseNodeDoubleClick(new NodeMouseEventArgs(nodeHit));
                ((IDispatchNodeEvents)nodeHit).DoubleClick(evtArgs);
           		}
            }
        }

        /// <summary>
        /// Called when a key down event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        public virtual void OnKeyDown(KeyEventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.ActiveTool != null)
                this.ActiveTool = this.ActiveTool.ProcessKeyDown(evtArgs);

            this.Viewer.UpdateView();
        }

        /// <summary>
        /// Called when a key up event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        public virtual void OnKeyUp(KeyEventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.ActiveTool != null)
                this.ActiveTool = this.ActiveTool.ProcessKeyUp(evtArgs);

            this.Viewer.UpdateView();
        }

        /// <summary>
        /// Called when a key press event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        public virtual void OnKeyPress(KeyPressEventArgs evtArgs)
        {
            if (this.Model == null) return;

            if (this.ActiveTool != null)
                this.ActiveTool = this.ActiveTool.ProcessKeyPress(evtArgs);

            this.Viewer.UpdateView();
        }

        /// <summary>
        /// Called when a VerticalScroll event is received.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// Iterates through all tools that implement the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IScrollEventReceiver"/>
        /// interface and forwards the event onto them.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IScrollEventReceiver"/>
        /// </remarks>
        protected virtual void OnVerticalScroll(object sender, ScrollEventArgs evtArgs)
        {
            foreach (IScrollEventReceiver rcvr in this.scrollEventReceivers)
            {
                rcvr.VerticalScroll(evtArgs);
            }

            m_bUpdateCursor = false;

            // process mouse move for active tool
            ProcessToolMouseMove(null);

            m_bUpdateCursor = true;
        }

        /// <summary>
        /// Called when a HorizontalScroll event is received.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// Iterates through all tools that implement the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.IScrollEventReceiver"/>
        /// interface and forwards the event onto them.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IScrollEventReceiver"/>
        /// </remarks>
        protected virtual void OnHorizontalScroll(object sender, ScrollEventArgs evtArgs)
        {
            foreach (IScrollEventReceiver rcvr in this.scrollEventReceivers)
            {
                rcvr.HorizontalScroll(evtArgs);
            }

            m_bUpdateCursor = false;

            // process mouse move for active tool
            ProcessToolMouseMove(null);

            m_bUpdateCursor = true;
        }
        #endregion

        #region Tool management
        /// <summary>
        /// Registers the standard interactive tools used by the diagram.
        /// </summary>
        protected virtual void RegisterStandardTools()
        {
            RegisterTool(new SelectTool(this));
            RegisterTool(new RotateTool(this));
            RegisterTool(new LineTool(this));
            RegisterTool(new BezierTool(this));
            RegisterTool(new OrthogonalLineTool(this));
            RegisterTool(new PolyLineTool(this));
            RegisterTool(new RectangleTool(this));
            RegisterTool(new RoundRectTool(this));
            RegisterTool(new SplineTool(this));
            RegisterTool(new PolygonTool(this));
            RegisterTool(new EllipseTool(this));
            RegisterTool(new CurveTool(this));
            RegisterTool(new ClosedCurveTool(this));
            RegisterTool(new DirectedLineConnectorTool(this));
            RegisterTool(new LineConnectorTool(this));
            RegisterTool(new OrthogonalConnectorTool(this));
            RegisterTool(new TextTool(this));
            RegisterTool(new RichTextTool(this));
            RegisterTool(new ZoomTool(this));
            RegisterTool(new PanTool(this));
            RegisterTool(new BitmapTool(this));
            RegisterTool(new PolyLineConnectorTool(this));
            RegisterTool(new PencilTool(this));
            RegisterTool(new SemiCircleTool(this));
            RegisterTool(new InsertNodeTool(this));
            RegisterTool(new ConnectionPointTool(this));
            RegisterTool(new OrgLineConnectorTool(this));
            // RegisterTool( new ControlNodeTool( this ) );
        }

        /// <summary>
        /// Adds a new tool to the controller.
        /// </summary>
        /// <param name="tool">Tool object to register.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool RegisterTool(Tool tool)
        {
            bool success = false;
            if (tool.Name != null && !this.toolTable.Contains(tool.Name))
            {
                this.toolTable.Add(tool.Name, tool);
                this.tools.Add(tool);

                success = true;
            }
            return success;
        }

        /// <summary>
        /// Removes a previously registered tool from the Controller.
        /// </summary>
        /// <param name="tool">Tool object to unregister.</param>
        /// <returns>True if the unregistration is successful; otherwise False.</returns>
        public bool UnRegisterTool(Tool tool)
        {
            bool success = false;
            if (this.toolTable.Contains(tool.Name))
            {
                if (this.tools.Contains(tool) == true)
                    this.tools.Remove(tool);

                this.toolTable.Remove(tool.Name);

                success = true;
            }
            return success;
        }

        /// <summary>
        /// Returns the Tool object matching the given name.
        /// </summary>
        /// <param name="toolName">Name of Tool object to return.</param>
        /// <returns>Tool object matching the given name, or NULL if not found.</returns>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
        /// </remarks>
        public Tool GetTool(string toolName)
        {
            if (this.toolTable.Contains(toolName))
            {
                return (Tool)this.toolTable[toolName];
            }
            return null;
        }

        /// <summary>
        /// Returns an array of all Tool objects registered with the controller.
        /// </summary>
        /// <returns>Array of registered Tool objects.</returns>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
        /// </remarks>
        public Tool[] GetAllTools()
        {
            int numTools = this.toolTable.Keys.Count;
            Tool[] tools = new Tool[numTools];
            string[] toolNames = new string[numTools];
            this.toolTable.Keys.CopyTo(toolNames, 0);
            for (int toolIdx = 0; toolIdx < numTools; toolIdx++)
            {
                tools[toolIdx] = this.toolTable[toolNames[toolIdx]] as Tool;
            }
            return tools;
        }

        /// <summary>
        /// Activates the Tool object matching the given name.
        /// </summary>
        /// <param name="toolName">Name of Tool to activate.</param>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// This method first locates the tool matching the given name and
        /// then passes it to the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.ActivateTool
        /// (Syncfusion.Windows.Forms.Diagram.Tool)"/> 
        /// method.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
        /// </remarks>
        public bool ActivateTool(string toolName)
        {
            return this.ActivateTool(this.GetTool(toolName));
        }

        /// <summary>
        /// Activates the given Tool object.
        /// </summary>
        /// <param name="tool">Tool to activate.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool ActivateTool(Tool tool)
        {
            this.ActiveTool = tool;
            return true;
        }

        /// <summary>
        /// Deactivates the given Tool object.
        /// </summary>
        /// <param name="tool">The Tool to deactivate.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool DeactivateTool(Tool tool)
        {
            this.ActiveTool = null;
            return true;
        }
        #endregion

        #region Clipboard
        /// <summary>
        /// Gets a value indicating whether if there are any selected nodes that can be removed from the
        /// the model.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanCut
        {
            get
            {
                return
                    (this.SelectionList.Count > 0)
                    || (this.TextEditor.IsEditing && this.TextEditor.CurrentText.Length > 0);
            }
        }

        /// <summary>
        /// Remove the currently selected nodes from the diagram and move them to the clipboard.
        /// </summary>
        public virtual void Cut()
        {
            //Checks whether the selected nodes can be removed from the model.
            if (CanCut)
            {
                // When Controller.Cut is invoked and a TextNode is in the process of being edited, 
                // clear the texteditor and copy it's contents to the clipboard.
                if (this.TextEditor.IsEditing)
                    ((ITextEdit)this.TextEditor).Cut();
                else
                {
                    // Create a collection to store the nodes on the clipboard.
                    NodeCollection clipboardNodes = new ClipboardNodeCollection(guid);

                    foreach (Node curNode in this.SelectionList)
                    {
                        Node nodeClone = (Node)curNode.Clone();

                        // convert to model coordinatess
                        HandlesHitTesting.ConvertToModelCoordinates(nodeClone, curNode.Parent as Node);

                        clipboardNodes.Add(nodeClone);
                    }

                    // look through connections
                    foreach (Node curNode in this.SelectionList)
                    {
                        PreserveNodeConnections(curNode, clipboardNodes);
                    }

                    // delete selected nodes
                    Delete();

                    // Copy the nodes to the clipboard.
                    Clipboard.SetDataObject(clipboardNodes, false);
                }

                // Reset the paste index.
                pasteIndex = 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are any selected nodes that can be copied to the clipboard.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanCopy
        {
            get
            {
                return
                    (this.SelectionList.Count > 0)
                    || (this.TextEditor.IsEditing && this.TextEditor.CurrentText.Length > 0);
            }
        }

        /// <summary>
        /// Copy the currently selected nodes to the clipboard.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>
        /// and copies the selected nodes into it. It then copies the new
        /// NodeCollection to the clipboard.
        /// </remarks>
        public virtual void Copy()
        {
            //Checks whether the nodes can be copied to the clipboard.
            if (CanCopy)
            {
                // When Controller.Copy is invoked and a TextNode is in the process of being edited,
                // copy the contents of the textnode editor to the clipboard.
                if (this.TextEditor.IsEditing)
                    Clipboard.SetDataObject(this.TextEditor.CurrentText, false);
                else
                {
                    // Create a collection to store the nodes on the clipboard.
                    NodeCollection clipboardNodes = new ClipboardNodeCollection(guid);

                    foreach (Node curNode in this.SelectionList)
                    {
                        Node nodeClone = CloneNode(curNode);

                        // convert to model coordinatess
                        HandlesHitTesting.ConvertToModelCoordinates(nodeClone, curNode.Parent as Node);

                        clipboardNodes.Add(nodeClone);
                    }

                    // look through connections
                    foreach (Node curNode in this.SelectionList)
                    {
                        PreserveNodeConnections(curNode, clipboardNodes);
                    }

                    // Copy the nodes to the clipboard.
                    Clipboard.SetDataObject(clipboardNodes, false);
                }

                // Set the paste index to offset by 1 space.
                pasteIndex = 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is any data in the clipboard that can be pasted
        /// into the model.
        /// </summary>
        /// <remarks>
        /// This method checks the clipboard to see if a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>
        /// is available.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanPaste
        {
            get
            {
                bool canPaste = false;

                try
                {
                    IDataObject clipboardData = Clipboard.GetDataObject();

                    if (clipboardData.GetDataPresent(typeof(ClipboardNodeCollection)))
                        canPaste = true;

                    // If the clipboard contains string data and a textnode is being edited, then allow the paste
                    if (clipboardData.GetDataPresent(typeof(string)) && this.TextEditor.IsEditing)
                        canPaste = true;
                }
                catch (Exception)
                {
                    // Invalid Data
                }

                return canPaste;
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard to the diagram.
        /// </summary>
        /// <remarks>
        /// If a <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>
        /// is available on the clipboard, this method gets it and inserts
        /// it into the diagram.
        /// </remarks>
        public virtual void Paste()
        {
             //Checks whether the data in the clipboard that can be pasted into the model.
            if (CanPaste)
            {
                IDataObject clipboardData = Clipboard.GetDataObject();

                if (clipboardData.GetDataPresent(typeof(ClipboardNodeCollection)))
                {
                    ClipboardNodeCollection clipboardNodes =
                        (ClipboardNodeCollection)clipboardData.GetData(typeof(ClipboardNodeCollection));

                    if (clipboardNodes != null /*&& clipboardNodes.CompareSourceGuid( guid )*/ )
                    {
                        if (this.Model != null && clipboardNodes.Count > 0)
                        {
                            this.Model.HistoryManager.StartAtomicAction("Insert Nodes");

                            int n;
                            if (clipboardNodes.Count == 1)
                                this.Model.AppendChild(clipboardNodes.First);
                            else
                                this.Model.AppendChildren(clipboardNodes, out n);

                            if (pasteIndex > 0)
                            {
                                // The nodes being pasted were put on the clipboard by this controller.
                                // Offset the nodes so that they are not pasted directly on top of the
                                // original source nodes.
                                SizeF pasteOffsetWorld = m_szPasteOffset;
                                pasteOffsetWorld.Width = pasteOffsetWorld.Width * this.pasteIndex;
                                pasteOffsetWorld.Height = pasteOffsetWorld.Height * this.pasteIndex;

                                MoveNodes(clipboardNodes, pasteOffsetWorld);
                            }

                            pasteIndex++;

                            this.Model.HistoryManager.EndAtomicAction();
                        }
                    }
                }
                else if (clipboardData.GetDataPresent(typeof(string)))
                {
                    string txtdata = (string)clipboardData.GetData(typeof(string));

                    if (txtdata != null)
                        ((ITextEdit)this.TextEditor).Paste(txtdata);
                }
            }
        }

        /// <summary>
        /// Gets the inserted nodes locations.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="locationDrop">The location drop.</param>
        /// <returns>The inserted nodes locations.</returns>
        private SizeF GetInsertedNodesOffset(NodeCollection nodes, PointF locationDrop)
        {
            SizeF nodesOffset = SizeF.Empty;

            using (GraphicsPath region = new GraphicsPath())
            {
                foreach (Node node in nodes)
                    region.AddRectangle(node.BoundingRectangle);

                PointF location = region.GetBounds().Location;

                float offsetX = locationDrop.X - location.X;
                float offsetY = locationDrop.Y - location.Y;

                nodesOffset = new SizeF(offsetX, offsetY);                                
            }

            return nodesOffset;
        }

        /// <summary>
        /// Paste the contents of the clipboard to the diagram.
        /// </summary>
        /// <param name="layername">Name of the layer in which to insert the nodes.</param>
        /// <param name="location">The location at which to insert the nodes.</param>
        /// <remarks>
        /// If a <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>
        /// is available on the clipboard, this method gets it and inserts
        /// it into the diagram.
        /// </remarks>
        public virtual void Paste(string layername, PointF location)
        {
            IDataObject clipboardData = Clipboard.GetDataObject();

            if (clipboardData.GetDataPresent(typeof(ClipboardNodeCollection)))
            {
                ClipboardNodeCollection clipboardNodes =
                    (ClipboardNodeCollection)clipboardData.GetData(typeof(ClipboardNodeCollection));

                if (clipboardNodes != null /*&& clipboardNodes.CompareSourceGuid( guid )*/ )
                {
                    if (this.Model != null && clipboardNodes.Count > 0)
                    {
                        this.Model.Layers[layername].Enabled = true;

                        SizeF pasteOffsetWorld = GetInsertedNodesOffset(clipboardNodes, location);

                        this.Model.HistoryManager.StartAtomicAction("Insert Nodes");

                        int n;
                        if (clipboardNodes.Count == 1)
                            this.Model.AppendChild(clipboardNodes.First);
                        else
                            this.Model.AppendChildren(clipboardNodes, out n);

                        MoveNodes(clipboardNodes, pasteOffsetWorld);
                                               
                        this.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
            else if (clipboardData.GetDataPresent(typeof(string)))
            {
                string txtdata = (string)clipboardData.GetData(typeof(string));

                if (txtdata != null)
                    ((ITextEdit)this.TextEditor).Paste(txtdata);
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard to the diagram.
        /// </summary>       
        /// <param name="location">The location at which to insert the nodes.</param>
        /// <remarks>
        /// If a <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>
        /// is available on the clipboard, this method gets it and inserts
        /// it into the diagram.
        /// </remarks>
        public virtual void Paste(PointF location)
        {
            IDataObject clipboardData = Clipboard.GetDataObject();

            if (clipboardData.GetDataPresent(typeof(ClipboardNodeCollection)))
            {
                ClipboardNodeCollection clipboardNodes =
                    (ClipboardNodeCollection)clipboardData.GetData(typeof(ClipboardNodeCollection));

                if (clipboardNodes != null)
                {
                    if (this.Model != null && clipboardNodes.Count > 0)
                    {                        
                        SizeF pasteOffsetWorld = GetInsertedNodesOffset(clipboardNodes, location);
                        this.Model.HistoryManager.StartAtomicAction("Insert Nodes");
                        int n;
                        if (clipboardNodes.Count == 1)
                            this.Model.AppendChild(clipboardNodes.First);
                        else
                            this.Model.AppendChildren(clipboardNodes, out n);
                        MoveNodes(clipboardNodes, pasteOffsetWorld);
                        this.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
            else if (clipboardData.GetDataPresent(typeof(string)))
            {
                string txtdata = (string)clipboardData.GetData(typeof(string));

                if (txtdata != null)
                    ((ITextEdit)this.TextEditor).Paste(txtdata);
            }
        }

        #endregion

        #region Hit Testing
        /// <summary>
        /// Gets the node at point in model coordinates.
        /// </summary>
        /// <param name="ptMouseLocation">The given point in model coordinates.</param>
        /// <returns>The node at the given point.</returns>
        public Node GetNodeAtPoint(PointF ptMouseLocation)
        {
            Node nodeToReturn = null;

            PointF ptPoint = this.ConvertFromModelToClientCoordinates(ptMouseLocation);
            NodeCollection nodes = GetNodesAtPoint(Geometry.ConvertPoint(ptPoint));

            if (nodes.Count > 0)
            {
                nodeToReturn = nodes.First;
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Gets all nodes include composite node children at point in model coordinates.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        /// <param name="ptPoint">The given point in model coordinates.</param>
        /// <returns>The nodes at the given point.</returns>
        public NodeCollection GetAllNodesAtPoint(ICompositeNode compositeNode, PointF ptPoint)
        {
            return GetAllNodesAtPoint(compositeNode, ptPoint, true);
        }

        /// <summary>
        /// Gets all nodes include composite node children at point in model coordinates.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        /// <param name="ptPoint">The given point in model coordinates.</param>
        /// <param name="bOnlyCanSelected">if set to <c>true</c> nodes filtered by allow selection.</param>
        /// <returns>The nodes at the given point.</returns>
        public NodeCollection GetAllNodesAtPoint(ICompositeNode compositeNode, PointF ptPoint, bool bOnlyCanSelected)
        {
            return HandlesHitTesting.GetAllNodesAtPoint(compositeNode, ptPoint, bOnlyCanSelected, true);
        }

        /// <summary>
        /// Gets the nodes at point in client coordinates.
        /// </summary>
        /// <param name="ptMouseLocation">The given point in client coordinates.</param>
        /// <returns>The nodes at the given point.</returns>
        public NodeCollection GetNodesAtPoint(Point ptMouseLocation)
        {
            NodeCollection nodesToReturn = new NodeCollection();
            
            // 1 - Convert to model coordinates
            PointF ptModelLocation = this.ConvertToModelCoordinates(ptMouseLocation);
            
            // 2 - iterate through model nodes hit performing hit test
            NodeCollection nodes = this.Model.Nodes;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                Node nodeCur = nodes[i] as Node;

                if (nodeCur != null && nodeCur.ContainsPoint(ptModelLocation))
                {
                    nodesToReturn.Add(nodeCur);
                }
            }

            return nodesToReturn;
        }
        #endregion

        #region IMouseController interface

        /// <summary>
        /// Gets the cursor currently assigned to the controller.
        /// </summary>
        Cursor IMouseController.Cursor
        {
            get
            {
                // if (this.refView != null)
                // {
                //   return this.refView.Cursor;
                // }
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the mouse controller.
        /// </summary>
        string IMouseController.Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        void IMouseController.CancelMode()
        {
        }

        int IMouseController.HitTest(MouseEventArgs mouseEventArgs, IMouseController controller)
        {
            return int.MaxValue;
        }

        void IMouseController.MouseDown(MouseEventArgs e)
        {
        }

        void IMouseController.MouseHover(MouseEventArgs e)
        {
        }

        void IMouseController.MouseHoverEnter()
        {
        }

        void IMouseController.MouseHoverLeave(EventArgs e)
        {
        }

        void IMouseController.MouseMove(MouseEventArgs e)
        {
        }

        void IMouseController.MouseUp(MouseEventArgs e)
        {
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Clone node or group with all children.
        /// </summary>
        /// <param name="curNode">Node to clone.</param>
        /// <returns>Clone instance.</returns>
        private Node CloneNode(Node curNode)
        {
            Node cloned = null;
            ICompositeNode compositeNode = curNode as ICompositeNode;

            if (compositeNode != null)
            {
                NodeCollection children = new NodeCollection();

                for (int i = 0, length = compositeNode.ChildCount; i < length; i++)
                {
                    Node node = compositeNode.GetChild(i);
                    children.Add(CloneNode(node));
                }

                compositeNode = (ICompositeNode)curNode.Clone();
                compositeNode.RemoveAllChildren();
                int index;
                compositeNode.AppendChildren(children, out index);
                cloned = compositeNode as Node;
            }
            else
            {
                cloned = (Node)curNode.Clone();
            }

            return cloned;
        }

        /// <summary>
        /// Check if node can be selected.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns><b>True</b> if node can be select, otherwise - <b>false</b>.</returns>
        private bool CanSelectNode(Node node)
        {
            bool bSuccess = true;

            if (node != null && (!node.Visible || !node.EditStyle.AllowSelect))
            {
                bSuccess = false;
            }

            return bSuccess;
        }
        private void PreserveNodeConnections(Node curNode, NodeCollection nodesClipboard)
        {
            EndPoint endPoint;
            IEndPointContainer endPointContainer;
            Node nodePortContainer;
            ConnectionPoint port;
            
            // if endpointcontainer connected to node
            // in selection list found --> reflect it on cloned nodes 
            if (curNode is IEndPointContainer)
            {
                endPointContainer = (IEndPointContainer)curNode;
                endPoint = endPointContainer.HeadEndPoint;
                port = endPoint.Port;

                // if connected
                if (port != null)
                {
                    nodePortContainer = port.Container;
                    
                    // with node in selection list ->
                    // update clipboard nodes
                    if (Contains(this.SelectionList, nodePortContainer))
                    {
                        Node nodeCloned = FindNodeByName(nodePortContainer.FullName, nodesClipboard);
                        Node nodeEndPointContainerCloned = FindNodeByName(curNode.FullName, nodesClipboard);

                        ConnectionPoint portCloned = nodeCloned.Ports.FindConnectionPointByID(port.ID);
                        if (portCloned != null)
                            portCloned.TryConnect(((IEndPointContainer)nodeEndPointContainerCloned).HeadEndPoint);
                    }
                }

                endPoint = endPointContainer.TailEndPoint;
                port = endPoint.Port;

                // if connected
                if (port != null)
                {
                    nodePortContainer = port.Container;
                    
                    // with node in selection list
                    // update clipboard nodes
                    if (Contains(this.SelectionList, nodePortContainer))
                    {
                        Node nodeCloned = FindNodeByName(nodePortContainer.FullName, nodesClipboard);
                        Node nodeEndPointContainerCloned = FindNodeByName(curNode.FullName, nodesClipboard);
                        ConnectionPoint portCloned = nodeCloned.Ports.FindConnectionPointByID(port.ID);
                        if (portCloned != null)
                            portCloned.TryConnect(((IEndPointContainer)nodeEndPointContainerCloned).TailEndPoint);
                    }
                }
            }
            else if (curNode is ICompositeNode)
            {
                ICompositeNode nodeComposite = (ICompositeNode)curNode;
                int nCount = 0;
                int nLength = nodeComposite.ChildCount;
                Node nodeTemp;

                while (nLength > nCount)
                {
                    nodeTemp = nodeComposite.GetChild(nCount);
                    PreserveNodeConnections(nodeTemp, nodesClipboard);
                    nCount++;
                }
            }
        }
        private bool Contains(NodeCollection nodesToSearchThrough, Node nodeToSearchFor)
        {
            bool bSuccess = false;
            ICompositeNode nodeComposite;

            // search through given node collection including composite nodes
            foreach (Node node in nodesToSearchThrough)
            {
                // quit loop if node found
                if (node.Equals(nodeToSearchFor))
                {
                    bSuccess = true;
                    break;
                }

                nodeComposite = node as ICompositeNode;

                if (nodeComposite != null)
                {
                    bSuccess = Contains(nodeComposite, nodeToSearchFor);

                    if (bSuccess)
                        break;
                }
            }

            return bSuccess;
        }
        private bool Contains(ICompositeNode nodeComposite, Node nodeToSearchFor)
        {
            bool bSuccess = false;
            int nCount = 0;
            int nLength = nodeComposite.ChildCount;
            ICompositeNode nodeCompTemp;
            Node nodeCur;
            
            // search through given node collection including composite nodes
            while (nLength > nCount)
            {
                // get node
                nodeCur = nodeComposite.GetChild(nCount);
                
                // quit loop if node found
                if (nodeCur.Equals(nodeToSearchFor))
                {
                    bSuccess = true;
                    break;
                }

                // if node is composite search through its children
                nodeCompTemp = nodeCur as ICompositeNode;

                if (nodeCompTemp != null)
                {
                    bSuccess = Contains(nodeCompTemp, nodeToSearchFor);

                    if (bSuccess)
                        break;
                }

                nCount++;
            }

            return bSuccess;
        }
        private Node FindNodeByName(string strNodeName, NodeCollection nodesCollToSearch)
        {
            strNodeName = strNodeName.Substring(strNodeName.IndexOf('.') + 1);
            Node nodeToReturn = null;
            ICompositeNode nodeComposite;

            // search through given node collection including composite nodes
            foreach (Node node in nodesCollToSearch)
            {
                // quit loop if node found
                if (node.Name == strNodeName)
                {
                    nodeToReturn = node;
                    break;
                }

                nodeComposite = node as ICompositeNode;

                if (nodeComposite != null)
                {
                    nodeToReturn = FindNodeByName(strNodeName, nodeComposite);

                    if (nodeToReturn != null)
                        break;
                }
            }

            return nodeToReturn;
        }
        private Node FindNodeByName(string strNodeName, ICompositeNode nodeToSearchThrough)
        {
            Node nodeToReturn = null;
            int nCount = 0;
            int nLength = nodeToSearchThrough.ChildCount;
            ICompositeNode nodeComposite;
            Node nodeCur;
            
            // search through given node collection including composite nodes
            while (nLength > nCount)
            {
                // get node
                nodeCur = nodeToSearchThrough.GetChild(nCount);
                nodeCur.Parent = nodeToSearchThrough;
                // quit loop if node found
                if (nodeCur.FullName == strNodeName)
                {
                    nodeToReturn = nodeCur;
                    break;
                }

                // if node is composite search through its children
                nodeComposite = nodeCur as ICompositeNode;

                if (nodeComposite != null)
                {
                    nodeToReturn = FindNodeByName(strNodeName, nodeComposite);

                    if (nodeToReturn != null)
                        break;
                }

                nCount++;
            }

            return nodeToReturn;
        }
        private PointF GetLocalPoint(Node parent, PointF ptLocation)
        {
            PointF[] ptPoint = new PointF[] { ptLocation };

            if (parent != null)
            {
                Matrix mtxTransformation = parent.GetTransformations();
                parent.AppendFlipTransforms(mtxTransformation);
                mtxTransformation.Multiply(HandlesHitTesting.GetParentsTransformations(parent), MatrixOrder.Append);

                mtxTransformation.Invert();

                mtxTransformation.TransformPoints(ptPoint);
            }

            return ptPoint[0];
        }
        private void DeactivateControlNode()
        {
            if (m_ctrlNodeActive != null)
            {
                m_ctrlNodeActive.Deactivate();
                m_ctrlNodeActive = null;
            }
        }
        private void AcivateControlNode(ControlNode ctrlNodeActivating)
        {
            if (m_ctrlNodeActive == null)
            {
                // to client coordinates calc its location here
                SizeF szSize = ((IUnitIndependent)ctrlNodeActivating).GetSize(MeasureUnits.Pixel);
                PointF[] pts = new PointF[]
                    {
                        PointF.Empty,
                        new PointF( szSize.Width, 0 ),
                        new PointF( 0, szSize.Height ),
                        new PointF( szSize.Width, szSize.Height )
                    };

                Matrix mtx = HandlesHitTesting.GetParentsTransformations(ctrlNodeActivating, true);
                mtx.TransformPoints(pts);

                // Assign Hositng Control new location
                RectangleF rcTemp = Geometry.CreateRect(pts);
                rcTemp = ConvertFromModelToClientCoordinates(rcTemp);

                PointF ptLoc = PointF.Empty;
                ptLoc.X = (rcTemp.X + rcTemp.Width / 2) - szSize.Width / 2;
                ptLoc.Y = (rcTemp.Y + rcTemp.Height / 2) - szSize.Height / 2;

                ctrlNodeActivating.Activate(Geometry.ConvertPoint(ptLoc));

                if (ctrlNodeActivating.Activated)
                    m_ctrlNodeActive = ctrlNodeActivating;
            }
        }

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        /// <param name="handlePos">The handle pos.</param>
        /// <param name="node">The node.</param>
        public void SetCursor(BoxPosition handlePos, Node node)
        {
            if (node == null)
                throw new ArgumentNullException(" node can't be null ");

            handlePos = ResizeTool.GetResizeCursors(handlePos, node);

            switch (handlePos)
            {
                case BoxPosition.TopCenter:
                case BoxPosition.BottomCenter:
                    ChangeCursor(Cursors.SizeNS);
                    break;

                case BoxPosition.MiddleLeft:
                case BoxPosition.MiddleRight:
                    ChangeCursor(Cursors.SizeWE);
                    break;

                case BoxPosition.TopLeft:
                case BoxPosition.BottomRight:
                    ChangeCursor(Cursors.SizeNWSE);
                    break;

                case BoxPosition.TopRight:
                case BoxPosition.BottomLeft:
                    ChangeCursor(Cursors.SizeNESW);
                    break;
            }
        }

        private Cursor prevCursor = Cursors.Arrow;

        /// <summary>
        /// Changes the mouse cursor.
        /// </summary>
        /// <param name="newCursor">The new cursor.</param>
        protected void ChangeCursor(Cursor newCursor)
        {
            if (this.prevCursor == null)
            {
                this.prevCursor = Cursor.Current;
            }
            this.Cursor = newCursor;
        }

        /// <summary>
        /// Restores the saved cursor.
        /// </summary>
        public void RestoreCursor()
        {
            if (this.prevCursor != null)
            {
                this.Cursor = this.prevCursor;
                this.prevCursor = null;
            }
        }

        /// <summary>
        /// Process mouse move of active tool.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void ProcessToolMouseMove(MouseEventArgs evtArgs)
        {
            // update current active tool cursor
            if (this.ActiveTool != null)
            {
                // create mouse event arguments instance if need
                if (evtArgs == null)
                {
                    Point ptMousePos = this.ParentControl.PointToClient(Control.MousePosition);
                    evtArgs = new MouseEventArgs(Control.MouseButtons, 0, ptMousePos.X, ptMousePos.Y, 0);
                }

                this.ActiveTool = this.ActiveTool.ProcessMouseMove(evtArgs);

                // update cursor
                //if (m_bUpdateCursor)
                //    Cursor.Current = this.ActiveTool.CurrentToolCursor;
                if (m_bUpdateCursor)
                {
                    if (this.ActiveTool.IsImageCursor(this.ActiveTool.ActionCursor) || (this.ActiveTool.ActionCursor.ToString() != Cursors.Default.ToString() && !this.ActiveTool.InAction))
                        Cursor.Current = this.ActiveTool.ActionCursor;
                    else
                        Cursor.Current = this.ActiveTool.CurrentToolCursor;
                }
                this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Gets the top node under mouse cursor.
        /// </summary>
        /// <param name="ptMouse">The mouse cursor position.</param>
        /// <returns>Top node under mouse cursor.</returns>
        public INode GetNodeUnderMouse(Point ptMouse)
        {
            INode nodeToReturn = null;

            NodeCollection nodes = GetNodesAtPoint(ptMouse);

            if (nodes.Count > 0)
                nodeToReturn = nodes.First;

            return nodeToReturn;
        }

        /// <summary>
        /// Move the nodes by given offset.
        /// </summary>
        /// <param name="nodesToMove">The nodes to move.</param>
        /// <param name="moveOffset">The move offset.</param>
        private void MoveNodes(NodeCollection nodesToMove, SizeF moveOffset)
        {
            this.Model.LinkManager.BeginSynchronization();
            this.Model.BridgeManager.BeginUpdateIntersection();

            foreach (Node node in nodesToMove)
            {
                node.Translate(moveOffset.Width, moveOffset.Height);
               
            }

            this.Model.LinkManager.EndSynchronization();
            this.Model.BridgeManager.EndUpdateIntersection();
        }

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
                model.LineBridgingEnabled = bBringinEnable;
                bBringinEnable = false;
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
		private ControlNode GetControlNode(ICompositeNode container)
        {
            ControlNode ctrlNode = null;
            NodeCollection child = this.GetAllNodesAtPoint(container, this.MouseLocation);
            foreach (Node node in child)
            {
                ICompositeNode parent = node as ICompositeNode;
                if (parent != null)
                    ctrlNode = GetControlNode(parent);
                else
                {
                    if (node is ControlNode)
                    {
                        if (ctrlNode == null)
                            ctrlNode = node as ControlNode;
                        else if (node.ZOrder > ctrlNode.ZOrder)
                            ctrlNode = node as ControlNode;
                    }
                }
            }
            return ctrlNode;
        }

        /// <summary>
        /// Draws the Guides
        /// </summary>
        /// <param name="renderHelper">Node</param>
        /// <param name="gfx">Graphics</param>
        /// <param name="curNodeBounds">Node bounds</param>
        public void DrawGuides(Node renderHelper, Graphics gfx, RectangleF curNodeBounds)
        {
            PointF ptStart = PointF.Empty, ptEnd = PointF.Empty;

            Pen pen = this.Guides.LineStyle.CreatePen();
            NodeCollection modelNodes = this.View.Model.Nodes;
            RectangleF clientBounds = this.View.ClientRectangle;
            PointF ptCenter = new PointF(curNodeBounds.X + curNodeBounds.Width / 2, curNodeBounds.Y + curNodeBounds.Height / 2);
            RectangleF m_refreshBounds = curNodeBounds;
            foreach (Node node in modelNodes)
            {
                if (node.FullName != renderHelper.FullName || (Control.ModifierKeys == Keys.Control && node.FullName == renderHelper.FullName))
                {
                    //Draws the Guides
                    RectangleF ndBounds = node.BoundingRectangle;
                    if ((this.Guides.Type & GuideTypes.Center) == GuideTypes.Center)
                    {
                        if (ptCenter.X == ndBounds.Left || ptCenter.X == node.PinPoint.X || ptCenter.X == ndBounds.Right)
                        {
                            if (curNodeBounds.Top > ndBounds.Top)
                            {
                                ptStart = new PointF(ptCenter.X, ndBounds.Top - 10);
                                ptEnd = new PointF(ptCenter.X, curNodeBounds.Top + curNodeBounds.Height + 10);
                            }
                            else
                            {
                                ptStart = new PointF(ptCenter.X, curNodeBounds.Top - 10);
                                ptEnd = new PointF(ptCenter.X, ndBounds.Top + ndBounds.Height + 10);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }

                        if (ptCenter.Y == ndBounds.Top || ptCenter.Y == node.PinPoint.Y || ptCenter.Y == ndBounds.Bottom)
                        {
                            if (curNodeBounds.Left > ndBounds.Left)
                            {
                                ptStart = new PointF(ndBounds.Left - 10, ptCenter.Y);
                                ptEnd = new PointF(curNodeBounds.Left + curNodeBounds.Width + 10, ptCenter.Y);
                            }
                            else
                            {
                                ptStart = new PointF(curNodeBounds.Left - 10, ptCenter.Y);
                                ptEnd = new PointF(ndBounds.Left + ndBounds.Width + 10, ptCenter.Y);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }
                        RectangleF guideBounds = Geometry.CreateRect(ptStart, ptEnd);
                        m_refreshBounds = RectangleF.Union(m_refreshBounds, guideBounds);
                    }
                    if ((this.Guides.Type & GuideTypes.Boundary) == GuideTypes.Boundary)
                    {
                        if (curNodeBounds.Left == ndBounds.Left || curNodeBounds.Left == node.PinPoint.X || curNodeBounds.Left == ndBounds.Right)
                        {
                            if (curNodeBounds.Top > ndBounds.Top)
                            {
                                ptStart = new PointF(curNodeBounds.Left, ndBounds.Top - 10);
                                ptEnd = new PointF(curNodeBounds.Left, curNodeBounds.Top + curNodeBounds.Height + 10);
                            }
                            else
                            {
                                ptStart = new PointF(curNodeBounds.Left, curNodeBounds.Top - 10);
                                ptEnd = new PointF(curNodeBounds.Left, ndBounds.Top + ndBounds.Height + 10);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }

                        if (curNodeBounds.Right == ndBounds.Left || curNodeBounds.Right == node.PinPoint.X || curNodeBounds.Right == ndBounds.Right)
                        {
                            if (curNodeBounds.Top > ndBounds.Top)
                            {
                                ptStart = new PointF(curNodeBounds.Right, ndBounds.Top - 10);
                                ptEnd = new PointF(curNodeBounds.Right, curNodeBounds.Top + curNodeBounds.Height + 10);
                            }
                            else
                            {
                                ptStart = new PointF(curNodeBounds.Right, curNodeBounds.Top - 10);
                                ptEnd = new PointF(curNodeBounds.Right, ndBounds.Top + ndBounds.Height + 10);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }

                        if (curNodeBounds.Top == ndBounds.Top || curNodeBounds.Top == node.PinPoint.Y || curNodeBounds.Top == ndBounds.Bottom)
                        {
                            if (curNodeBounds.Left > ndBounds.Left)
                            {
                                ptStart = new PointF(ndBounds.Left - 10, curNodeBounds.Top);
                                ptEnd = new PointF(curNodeBounds.Left + curNodeBounds.Width + 10, curNodeBounds.Top);
                            }
                            else
                            {
                                ptStart = new PointF(curNodeBounds.Left - 10, curNodeBounds.Top);
                                ptEnd = new PointF(ndBounds.Left + ndBounds.Width + 10, curNodeBounds.Top);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }

                        if (curNodeBounds.Bottom == ndBounds.Top || curNodeBounds.Bottom == node.PinPoint.Y || curNodeBounds.Bottom == ndBounds.Bottom)
                        {
                            if (curNodeBounds.Left > ndBounds.Left)
                            {
                                ptStart = new PointF(ndBounds.Left - 10, curNodeBounds.Bottom);
                                ptEnd = new PointF(curNodeBounds.Left + curNodeBounds.Width + 10, curNodeBounds.Bottom);
                            }
                            else
                            {
                                ptStart = new PointF(curNodeBounds.Left - 10, curNodeBounds.Bottom);
                                ptEnd = new PointF(ndBounds.Left + ndBounds.Width + 10, curNodeBounds.Bottom);
                            }
                            gfx.DrawLine(pen, ptStart, ptEnd);
                        }
                        RectangleF guideBounds = Geometry.CreateRect(ptStart, ptEnd);
                        m_refreshBounds = RectangleF.Union(m_refreshBounds, guideBounds);
                    }
                    DrawMarginGuide(gfx, pen, curNodeBounds, ndBounds, m_refreshBounds);
                }
            }   
        }

        /// <summary>
        /// Draws the margin for node.
        /// </summary>
        /// <param name="gfx">Graphics</param>
        /// <param name="pen">Pen</param>
        /// <param name="curNodeBounds">Render helper bounds</param>
        /// <param name="ndBounds">Node bounds</param>
        private void DrawMarginGuide(Graphics gfx, Pen pen, RectangleF curNodeBounds, RectangleF ndBounds, RectangleF refreshBounds)
        {
            //Draws the margin line
            if ((this.Guides.Type & GuideTypes.Margin) == GuideTypes.Margin)
            {
                if (ndBounds.Left - this.Guides.Margin == curNodeBounds.Right)
                {
                    float y = 0;
                    if (ndBounds.Bottom < curNodeBounds.Top)
                        y = ndBounds.Bottom;
                    else
                        y = ndBounds.Top;

                    gfx.DrawLine(pen, new PointF(ndBounds.Left, y), new PointF(ndBounds.Left - this.Guides.Margin, y));
                    gfx.DrawLine(pen, new PointF(ndBounds.Left - 5, y - 5), new PointF(ndBounds.Left, y));
                    gfx.DrawLine(pen, new PointF(ndBounds.Left - 5, y + 5), new PointF(ndBounds.Left, y));
                    gfx.DrawLine(pen, new PointF(curNodeBounds.Right + 5, y - 5), new PointF(curNodeBounds.Right, y));
                    gfx.DrawLine(pen, new PointF(curNodeBounds.Right + 5, y + 5), new PointF(curNodeBounds.Right, y));
                }

                if (ndBounds.Right + this.Guides.Margin == curNodeBounds.Left)
                {
                    float y = 0;
                    if (ndBounds.Bottom < curNodeBounds.Top)
                        y = ndBounds.Bottom;
                    else
                        y = ndBounds.Top;

                    gfx.DrawLine(pen, new PointF(ndBounds.Right, y), new PointF(ndBounds.Right + this.Guides.Margin, y));
                    gfx.DrawLine(pen, new PointF(ndBounds.Right + 5, y - 5), new PointF(ndBounds.Right, y));
                    gfx.DrawLine(pen, new PointF(ndBounds.Right + 5, y + 5), new PointF(ndBounds.Right, y));
                    gfx.DrawLine(pen, new PointF(curNodeBounds.Left - 5, y - 5), new PointF(curNodeBounds.Left, y));
                    gfx.DrawLine(pen, new PointF(curNodeBounds.Left - 5, y + 5), new PointF(curNodeBounds.Left, y));
                }

                if (ndBounds.Top - this.Guides.Margin == curNodeBounds.Bottom)
                {
                    float x = 0;
                    if (ndBounds.Right < curNodeBounds.Left)
                        x = ndBounds.Right;
                    else
                        x = ndBounds.Left;

                    gfx.DrawLine(pen, new PointF(x, ndBounds.Top), new PointF(x, ndBounds.Top - this.Guides.Margin));
                    gfx.DrawLine(pen, new PointF(x - 5, ndBounds.Top - 5), new PointF(x, ndBounds.Top));
                    gfx.DrawLine(pen, new PointF(x + 5, ndBounds.Top - 5), new PointF(x, ndBounds.Top));
                    gfx.DrawLine(pen, new PointF(x - 5, curNodeBounds.Bottom + 5), new PointF(x, curNodeBounds.Bottom));
                    gfx.DrawLine(pen, new PointF(x + 5, curNodeBounds.Bottom + 5), new PointF(x, curNodeBounds.Bottom));
                }

                if (ndBounds.Bottom + this.Guides.Margin == curNodeBounds.Top)
                {
                    float x = 0;
                    if (ndBounds.Right < curNodeBounds.Left)
                        x = ndBounds.Right;
                    else
                        x = ndBounds.Left;

                    gfx.DrawLine(pen, new PointF(x, ndBounds.Bottom), new PointF(x, ndBounds.Bottom + this.Guides.Margin));
                    gfx.DrawLine(pen, new PointF(x - 5, ndBounds.Bottom + 5), new PointF(x, ndBounds.Bottom));
                    gfx.DrawLine(pen, new PointF(x + 5, ndBounds.Bottom + 5), new PointF(x, ndBounds.Bottom));
                    gfx.DrawLine(pen, new PointF(x - 5, curNodeBounds.Top - 5), new PointF(x, curNodeBounds.Top));
                    gfx.DrawLine(pen, new PointF(x + 5, curNodeBounds.Top - 5), new PointF(x, curNodeBounds.Top));
                }
                refreshBounds = RectangleF.Union(refreshBounds, ndBounds);                
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(refreshBounds));
            }
        }        
        #endregion

        #region Class overrides
        /// <summary>
        /// Documents the node collection changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="T:Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void Document_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            bool bAttachParent = (evtArgs.ChangeType == CollectionExChangeType.Insert
                                   || evtArgs.ChangeType == CollectionExChangeType.Set)
                                    ? true : false;
            
            // give parent for control node hosting control
            if (evtArgs.Elements != null && evtArgs.Elements.Count > 0)
            {
                ControlNode nodeCtrl;
                ICompositeNode nodeComp;

                foreach (INode node in evtArgs.Elements)
                {
                    nodeComp = node as ICompositeNode;

                    if (nodeComp != null)
                    {
                        UpdateChildCN(nodeComp, bAttachParent);
                    }
                    else
                    {
                        nodeCtrl = node as ControlNode;

                        if (nodeCtrl != null)
                        {
                            if (bAttachParent)
                                nodeCtrl.HCParent = this.Viewer as Control;
                            else
                                nodeCtrl.HCParent = null;
                        }
                    }
                }
            }

            base.Document_NodeCollectionChanged(evtArgs);
        }

        private void UpdateChildCN(ICompositeNode nodeComp, bool bAttachParent)
        {
            int nLength = nodeComp.ChildCount;
            int nCount = 0;
            Node nodeCur;
            ControlNode nodeCtrl;
            ICompositeNode nodeComposite;

            while (nLength > nCount)
            {
                nodeCur = nodeComp.GetChild(nCount);

                nodeCtrl = nodeCur as ControlNode;

                if (nodeCtrl != null)
                {
                    if (bAttachParent)
                        nodeCtrl.HCParent = this.Viewer as Control;
                    else
                        nodeCtrl.HCParent = null;
                }
                else
                {
                    nodeComposite = nodeCur as ICompositeNode;

                    if (nodeComposite != null)
                    {
                        UpdateChildCN(nodeComposite, bAttachParent);
                    }
                }

                nCount++;
            }
        }

        /// <summary>
        /// Handles the view origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="T:Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void View_OriginChanged(ViewOriginEventArgs evtArgs)
        {
            this.TextEditor.EndEdit(false);

            DeactivateControlNode();

            base.View_OriginChanged(evtArgs);
        }

        /// <summary>
        /// Handles the CommandStarted event of the HistoryManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void HistoryManager_CommandStarted(object sender, EventArgs e)
        {
            this.TextEditor.EndEdit(false);

            DeactivateControlNode();

            base.HistoryManager_CommandStarted(sender, e);
        }

        /// <summary>
        /// Events the sink property changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="T:Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected override void EventSink_PropertyChanging(PropertyChangingEventArgs evtArgs)
        {
            base.EventSink_PropertyChanging(evtArgs);

            if (evtArgs.PropertyName == DPN.Magnification && this.TextEditor.IsEditing)
            {
                this.TextEditor.EndEdit(true);
            }
        }

        /// <summary>
        /// Documents the property changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected override void Document_PropertyChanging(PropertyChangingEventArgs evtArgs)
        {
            ControlNode ctrlNode = evtArgs.PropertyContainer as ControlNode;

            if (ctrlNode != null && evtArgs.PropertyName == DPN.Activate && (bool)evtArgs.NewValue)
            {
                m_ctrlNodeActive = ctrlNode;
            }

            if (evtArgs.PropertyName == DPN.Visible && !(bool)evtArgs.NewValue)
            {
                this.TextEditor.EndEdit(true);
                DeactivateControlNode();
            }

            base.Document_PropertyChanging(evtArgs);
        }
        #endregion

        #region Fields
        /// <summary>
        /// Collection of selected nodes before tool processing.
        /// </summary>
        private NodeCollection m_previousSelected = null;

        /// <summary>
        /// Flag indicating if the cursor will set from active tool.
        /// </summary>
        private bool m_bUpdateCursor = true;

        /// <summary>
        /// Paste Offset.
        /// </summary>
        protected SizeF m_szPasteOffset;
        private NodeCollection m_nodesHit;
        private Node m_nodeHit;
        private ArrayList scrollEventReceivers;
        private Tool m_toolActive;

        /// <summary>
        /// Text editor registered to this controller.
        /// </summary>
        private TextEditor m_textEditor;

        /// <summary>
        /// InPlaceEditor for label registered to this controller.
        /// </summary>
        private InPlaceEditor m_inPlaceEditor;

        /// <summary>
        /// List of tools registered with this controller.
        /// </summary>
        private ArrayList tools;

        /// <summary>
        /// Hashtable that maps tool names onto tool objects.
        /// </summary>
        private Hashtable toolTable;

        /// <summary>
        /// References the currently active Tool.
        /// </summary>
        protected Tool accctiveTool;

        /// <summary>
        /// Unique ID used to identify the source of copy and paste operations.
        /// </summary>
        protected Guid guid = Guid.Empty;

        /// <summary>
        /// Number of consecutive paste operations - used to offset nodes pasted.
        /// </summary>
        protected int pasteIndex;

        /// <summary>
        /// Last known position of the mouse pointer.
        /// </summary>
        private PointF mouseLocation;

        /// <summary>
        /// Flag indicating if the controller should track mouse movements in the view.
        /// </summary>
        private bool mouseTrackingEnabled = true;

        /// <summary>
        /// Active control node.
        /// </summary>
        private ControlNode m_ctrlNodeActive;
        #endregion
    }

    /// <summary>
    /// This class encapsulates event arguments for events fired by the
    /// controller that are caused by
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Tool"/> objects.
    /// </summary>
    public class ToolEventArgs
        : EventArgs
    {
        #region Class members
        private Tool m_tool;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolEventArgs"/> class.
        /// </summary>
        /// <param name="tool">The UI Tool.</param>
        public ToolEventArgs(Tool tool)
        {
            if (tool == null)
                throw new ArgumentNullException("tool");

            m_tool = tool;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the tool object that generated the event.
        /// </summary>
        public Tool Tool
        {
            get { return m_tool; }
        }
        #endregion
    }

    /// <summary>
    /// Delegate used for tool events.
    /// </summary>
    /// <param name="evtArgs">Event args</param>
    public delegate void ToolEventHandler(ToolEventArgs evtArgs);

    /// <summary>
    /// Specifies Rendering helper style.
    /// </summary>
    public enum RenderingHelperStyle
    {
        /// <summary>
        /// Ghost copy style.
        /// </summary>
        GhostCopy,

        /// <summary>
        /// Dashed outline style
        /// </summary>
        DashedOutline,

        /// <summary>
        /// Solid outline style
        /// </summary>
        SolidOutline,

        /// <summary>
        /// Filled rectangle style
        /// </summary>
        FilledRectangle
    }
}
