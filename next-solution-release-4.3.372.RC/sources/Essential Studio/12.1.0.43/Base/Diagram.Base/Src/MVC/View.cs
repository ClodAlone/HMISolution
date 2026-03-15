#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices.WinAPI;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A view encapsulates a rectangular area inside of a control and renders
    /// a <see cref="Syncfusion.Windows.Forms.Diagram.Model"/> onto it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A view is responsible for rendering the diagram onto a control surface
    /// (i.e. window). It contains a reference to a
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model"/> which contains the
    /// data portion of the diagram. The view renders the model onto a
    /// System.Drawing.Graphics context object belonging to the control that
    /// the view is hosted in. The view also renders other visual cues and decorations
    /// that are not persisted in the model, such as selection handles.
    /// </para>
    /// <para>
    /// The view is responsible for conversions between world, view, and device
    /// coordinates. The model belongs to the world coordinate space. The view
    /// maps world coordinates onto view coordinates by applying its
    /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
    /// and
    /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
    /// settings, which are used to implement zooming and scrolling. In other
    /// words, world coordinates are mapped to view coordinates by applying
    /// a transformation that translates to the origin and scales by a
    /// magnification percentage. Both world and view coordinates are stored
    /// as floating point numbers. View coordinates are mapped to device
    /// coordinates based on the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.MeasurementUnits"/>
    /// settings in the model and the resolution (DPI) of the output device.
    /// The default PageUnit setting is pixel and the default PageScale setting
    /// is 1, which results in a 1-1 mapping from view to device coordinates.
    /// If the PageUnit is set to Inch, the PageScale is 0.5, and the resolution
    /// is 96 dpi then 1 logical unit in view coordinates will equal
    /// (1 * 96) * 0.5 = 48 pixels.
    /// </para>
    /// <para>
    /// The view provides methods for performing hit testing nodes, selection
    /// handles, vertices, and ports. The hit testing methods take points
    /// in device coordinates and perform the necessary conversion to world
    /// coordinates.
    /// </para>
    /// <para>
    /// The view also provides methods for drawing tracking objects. A tracking
    /// object is an outline of a rectangle or shape that is moved or tracked
    /// across the screen in response to mouse movements. The view has methods
    /// for drawing tracking outlines of rectangles, lines, polygons, curves,
    /// and System.Drawing.GraphicsPath objects.
    /// </para>
    /// <para>
    /// The view contains public methods that can be called to render and
    /// repaint onto the host control. The view uses a technique called back
    /// buffering, which divides rendering into two stages. First, the view
    /// renders onto a memory-based bitmap image (the back buffer). The back
    /// buffer is then painted onto the host control. This technique eliminates
    /// flicker and has the added benefit of leaving the view with an in-memory
    /// representation of the last frame it rendered.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ViewInfo"/>
    /// </remarks>
    [
    Serializable,
    ToolboxItem(false),
    TypeConverter(typeof(ViewConverter))
    ]
    public class View
        : Component,
          IServiceProvider,
          ISerializable,
          IDeserializationCallback,
          IPrint,
          IPropertyObserver,
          IPropertyContainer,
          IServiceReferenceHolder,
          IServiceReferenceProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        public View()
        {
            this.rcBounds = System.Drawing.Rectangle.Empty;
            this.backgroundColor = Color.DarkGray;
            m_nPasteOffsetX = 10;
            m_nPasteOffsetY = 10;
            m_fMagnification = 100f;
            this.dpiX = 96;
            this.dpiY = 96;
            this.handleSize = 6;
            this.handleColor = Color.GreenYellow;
            this.handleOutlineColor = Color.Black;
            this.handleAnchorColor = Color.Gray;
            this.handleDisabledColor = Color.Gray;
            this.m_ptOrigin = new PointF(0, 0);
            m_stylePageBorder = new PageBorderStyle();

            m_styleHandles = new HandleStyles();
            m_styleHandles.UpdateServiceReferences(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        /// <param name="parentControl">Parent control (i.e. window) hosting the view.</param>
        public View(Control parentControl)
        {
            this.rcBounds = System.Drawing.Rectangle.Empty;
            if (parentControl != null)
            {
                this.rcBounds = parentControl.Bounds;
            }
            this.backgroundColor = Color.DarkGray;
            m_fMagnification = 100f;
            this.dpiX = 96;
            this.dpiY = 96;
            this.handleSize = 6;
            this.handleColor = Color.GreenYellow;
            this.handleOutlineColor = Color.Black;
            this.handleAnchorColor = Color.Gray;
            this.handleDisabledColor = Color.Gray;
            this.m_ptOrigin = new PointF(0, 0);
            m_stylePageBorder = new PageBorderStyle();
            this.Initialize(parentControl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        /// <param name="parentControl">Parent control (i.e. window) to host the view.</param>
        /// <param name="top">Top of view bounds.</param>
        /// <param name="left">Left of view bounds.</param>
        /// <param name="width">Width of view bounds.</param>
        /// <param name="height">Height of view bounds.</param>
        public View(Control parentControl, int top, int left, int width, int height)
        {
            this.rcBounds = new System.Drawing.Rectangle(top, left, width, height);
            this.backgroundColor = Color.DarkGray;
            this.m_fMagnification = 100f;
            this.dpiX = 96;
            this.dpiY = 96;
            this.handleSize = 6;
            this.handleColor = Color.GreenYellow;
            this.handleOutlineColor = Color.Black;
            this.handleAnchorColor = Color.Gray;
            this.handleDisabledColor = Color.Gray;
            this.m_ptOrigin = new PointF(0, 0);
            m_stylePageBorder = new PageBorderStyle();
            this.Initialize(parentControl);
        }

        // Used for backward compatibility with the DiagramDoc class
        internal View(ViewInfo vwinfo)
        {
            this.SetViewInfo(vwinfo);
            this.dpiX = 96;
            this.dpiY = 96;

            // this.trackingStyle = new TrackingStyle(this.propertyContainer);
            m_grid = this.CreateGrid();
            m_stylePageBorder = new PageBorderStyle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected View(SerializationInfo info, StreamingContext context)
        {
            this.dpiX = 96;
            this.dpiY = 96;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "origin":
                        m_ptOrigin = (PointF)info.GetValue("origin", typeof(PointF));
                        break;
                    case "magnification":
                        m_fMagnification = info.GetSingle("magnification");
                        break;
                    case "pageborderstyle":
                        m_stylePageBorder = (PageBorderStyle)info.GetValue("pageborderstyle", typeof(PageBorderStyle));
                        break;
                    case "m_grid":
                        m_grid = (LayoutGrid)info.GetValue("m_grid", typeof(LayoutGrid));
                        break;
                    case "handleColor":
                        this.HandleRenderer.HandleColor = (System.Drawing.Color)info.GetValue("handleColor", typeof(System.Drawing.Color));
                        break;
                    case "handleOutlineColor":
                        this.HandleRenderer.HandleOutlineColor = (System.Drawing.Color)info.GetValue("handleOutlineColor", typeof(System.Drawing.Color));
                        break;
                    case "handleDisabledColor":
                        this.HandleRenderer.HandleDisabledColor = (System.Drawing.Color)info.GetValue("handleDisabledColor", typeof(System.Drawing.Color));
                        break;
                    case "backcolor":
                        this.BackgroundColor = (Color)info.GetValue("backcolor", typeof(Color));
                        break;
                    case "printZoom":
                        this.PrintZoom = (PrintZoom)info.GetValue("printZoom", typeof(PrintZoom));
                        break;
                    case "pageSetting":
                        this.PageSettings = (PageSettings)info.GetValue("pageSetting", typeof(PageSettings));
                        break;
                    case "zoomIncrement":
                        this.ZoomIncrement = info.GetInt32("zoomIncrement");
                        break;
                }
            }
            if (this.HandleRenderer.HandleColor.IsEmpty)
            {
                ResetHandleColor();
            }

            if (this.HandleRenderer.HandleOutlineColor.IsEmpty)
            {
                ResetHandleOutlineColor();
            }

            if (this.HandleRenderer.HandleDisabledColor.IsEmpty)
            {
                ResetHandleDisabledColor();
            }

            if (this.BackgroundColor.IsEmpty)
            {
                ResetBackgroundColor();
            }

            if (m_grid != null)
            {
                m_grid.ContainerView = this;
                m_grid.UpdateServiceReferences(this);
            }
        }

        /// <summary>
        /// Creates a ViewInfo object and initializes it with the View data.
        /// </summary>
        /// <returns>A <see cref="Syncfusion.Windows.Forms.Diagram.ViewInfo"/> object.</returns>
        public virtual ViewInfo GetViewInfo()
        {
            return new ViewInfo(this);
        }

        /// <summary>
        /// Initializes the View with data from the ViewInfo object.
        /// </summary>
        /// <param name="vwinfo">A <see cref="Syncfusion.Windows.Forms.Diagram.ViewInfo"/> object.</param>
        public virtual void SetViewInfo(ViewInfo vwinfo)
        {
            this.rcBounds = vwinfo.rcBounds;
            this.m_ptOrigin = vwinfo.ptOrigin;
            this.backgroundColor = vwinfo.backgroundColor;
            this.handleSize = vwinfo.handleSize;
            this.handleColor = vwinfo.handleColor;
            this.handleOutlineColor = Color.Black;
            this.handleAnchorColor = vwinfo.handleAnchorColor;
            this.handleDisabledColor = vwinfo.handleDisabledColor;
        }

        /// <summary>
        /// Called to release resources held by the view.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if this method is being called explicitly by a call to Dispose()
        /// or by the destructor through the garbage collector.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals True, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    this.m_document = null;

                    if (this.parentControl != null)
                    {
                        this.parentControl = null;
                    }

                    if (this.topMarginBuffer != null)
                    {
                        this.topMarginBuffer.Dispose();
                        this.topMarginBuffer = null;
                    }

                    if (this.leftMarginBuffer != null)
                    {
                        this.leftMarginBuffer.Dispose();
                        this.leftMarginBuffer = null;
                    }

                    if (m_styleHandles != null)
                        m_styleHandles.UpdateServiceReferences(null);
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Fields

        private System.Drawing.Rectangle m_clientRectangle;
        private Controller m_controller;
        private NodeCollection m_lstSubstitute;

        /// <summary>
        /// List of selected nodes.
        /// </summary>
        private NodeCollection m_nodesSelected;

        /// <summary>
        /// Reference to the parent control (control hosting the view).
        /// </summary>
        private Control parentControl;

        /// <summary>
        /// Reference to the model attached to the view.
        /// </summary>
        protected Model m_document;

        /// <summary>
        /// Bounds of the view.
        /// </summary>
        protected System.Drawing.Rectangle rcBounds;

        /// <summary>
        /// Color to clear the background with.
        /// </summary>
        private Color backgroundColor;

        /// <summary>
        /// Object that renders grid.
        /// </summary>
        private LayoutGrid m_grid;

        /// <summary>
        /// Handles style.
        /// </summary>
        /// <remarks> Made static in order 
        /// to allow Node access ControlPointStyle
        /// </remarks>
        private static HandleStyles m_styleHandles;

        /// <summary>
        /// Back buffer used to elimate flicker during mouse tracking in the top margin.
        /// </summary>
        private Bitmap topMarginBuffer = null;

        /// <summary>
        /// Back buffer used to elimate flicker during mouse tracking in the bottom margin.
        /// </summary>
        private Bitmap leftMarginBuffer = null;

        /// <summary>
        /// View origin.
        /// </summary>
        protected PointF m_ptOrigin = new PointF(100, 100);

        /// <summary>
        /// Magnification value.
        /// </summary>
        private float m_fMagnification;

        /// <summary>
        /// Zoom increment value.
        /// </summary>
        private int m_iZoomIncrement = 20;

        /// <summary>
        /// Currently active cursor.
        /// </summary>
        private Cursor m_cursor;

        /// <summary>
        /// Horizontal resolution of the device.
        /// </summary>
        private float dpiX;

        /// <summary>
        /// Vertical resolution of the device.
        /// </summary>
        private float dpiY;

        /// <summary>
        /// Size to draw selection handles (in device units).
        /// </summary>
        private int handleSize;

        /// <summary>
        /// Color to draw selection handles.
        /// </summary>
        private Color handleColor;

        /// <summary>
        /// Color to draw selection handles.
        /// </summary>
        private Color handleOutlineColor;

        /// <summary>
        /// Color to draw selection handles for anchor node.
        /// </summary>
        private Color handleAnchorColor;

        /// <summary>
        /// Instance of user defined handle renderer.
        /// </summary>
        private UserHandleRenderer m_customHandleRenderer;

        /// <summary>
        /// Color to draw selection handles when disabled.
        /// </summary>
        private Color handleDisabledColor;

        /// <summary>
        /// Page size of the default printer.
        /// </summary>
        private Size m_pageSize;

        /// <summary>
        /// Flag indicating if the page size is a known value.
        /// </summary>
        private bool m_pageSizeKnown = false;

        /// <summary>
        /// Indicates if the Dispose() method has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Keeps track of the current print page.
        /// </summary>
        private int nCurrentPage = 0;

        /// <summary>
        /// Page settings used for printing.
        /// </summary>
        [NonSerialized]
        protected PageSettings pgSettings = null;

        /// <summary>
        ///  Print zooming or fit to page.
        /// </summary>
        protected PrintZoom prtZoom = null;

        /// <summary>
        ///  Page border style.
        /// </summary>
        private PageBorderStyle m_stylePageBorder;
        internal ViewerEventSink m_eventSink;

        /// <summary>
        /// Indicates whether Mouse Tracking is enabled.
        /// </summary>
        private bool m_bTrackingEnabled;

        /// <summary>
        /// Defines X direction offset when nodes are pasted into the diagram.
        /// </summary>
        private int m_nPasteOffsetX;

        /// <summary>
        /// Defines Y direction offset when nodes are pasted into the diagram.
        /// </summary>
        private int m_nPasteOffsetY;

        /// <summary>
        /// Horizontal and vertical spaces to model bounds.
        /// </summary>
        private RectangleF m_rcVirtualBounds = RectangleF.Empty;

        /// <summary>
        /// Bounding rectangle of a node.
        /// </summary>
        private RectangleF boundingRect;

        /// <summary>
        /// View Handle renderer.
        /// </summary>
        private HandleRenderer m_handleRenderer;

        /// <summary>
        /// Origin of the view.
        /// </summary>
        private PointF origin = PointF.Empty;

        private const int DEF_ORIGIN_OFFSET = 30;       
        
        /// <summary>
        /// Type of the zooming action to be performed
        /// </summary>
        private ZoomType m_zoomType;
        #endregion

        #region Initialization

        /// <summary>
        /// Attaches the view to a given parent control.
        /// </summary>
        /// <param name="parentControl">Parent control hosting the view.</param>
        /// <remarks>
        /// </remarks>
        public virtual void Initialize(Control parentControl)
        {
            if (this.parentControl != null)
            {
                throw new InvalidOperationException(Resources.Strings.Messages.Get("ObjectState"));
            }
            this.parentControl = parentControl;

            if (this.parentControl != null)
            {
                Graphics grfx = this.parentControl.CreateGraphics();
                this.dpiX = grfx.DpiX;
                this.dpiY = grfx.DpiX;
                grfx.Dispose();
            }
        }

        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the view's ClientRectangle.
        /// </summary>          
        [Browsable(false)]
        [Description("Gets or sets the view's ClientRectangle.")]
        public System.Drawing.Rectangle ClientRectangle
        {
            get
            {
                return m_clientRectangle;
            }
            set {
                    if (m_clientRectangle != value)
                    {
                        m_clientRectangle = value;                    
                    }
            }
        }

        /// <summary>
        /// Gets or sets the view's controller.
        /// </summary>          
        [Browsable(false)]
        [Description("Gets or sets the view's controller")]
        public Controller Controller
        {
            get
            {                
                return m_controller;
            }

            set
            {
                if (m_controller != value)
                {
                    m_controller = value;
                }
            }
        }

        /// <summary>
        /// Gets the selection list substitute.
        /// </summary>
        /// <value>The selection list substitute.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NodeCollection SelectionListSubstitute
        {
            get
            {
                if (m_lstSubstitute == null)
                {
                    m_lstSubstitute = new NodeCollection();
                    m_lstSubstitute.UpdateReferences = false;
                    m_nodesSelected.UpdateServiceReferences(this);
                }

                return m_lstSubstitute;
            }
        }

        /// <summary>
        /// Gets the reference to viewer event sink.
        /// </summary>
        /// <value>The event sink.</value>
        protected ViewerEventSink EventSink
        {
            get { return m_eventSink; }
        }

        #region Public
        /// <summary>
        /// Gets or sets the model attached to this view.
        /// </summary>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Model Model
        {
            get
            {
                return this.m_document;
            }
            set
            {
                if (value != this.m_document)
                {
                    m_document = value;
                    this.PageSizeKnown = false;

                    if (m_document != null)
                    {
                        RefreshPageSettings();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets logical origin of the view in world coordinates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property moves the view relative to the world
        /// coordinate space. The value specifies a point in the world
        /// coordinate space that corresponds to the top left corner
        /// of the view.
        /// </para>
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual PointF Origin
        {
            get 
            { 
                return m_ptOrigin; 
            }
            set
            {
                if (m_ptOrigin != value && !float.IsNaN(value.X) 
                    && !float.IsNaN(value.Y))
                {
                    PointF oldOrigin = m_ptOrigin;
                    m_ptOrigin = value;

                    OnOriginChanged(new ViewOriginEventArgs(oldOrigin, m_ptOrigin));
                }
            }
        }

        /// <summary>
        /// Gets the size of the scrollable area in device coordinates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value returned is the width and height of the model converted to
        /// device coordinates. If there is no model attached to the view at
        /// the time of the call, then the size of the view is returned instead.
        /// </para>
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual Size VirtualSize
        {
            get
            {
                Size szVirtual = new Size(this.rcBounds.Width, this.rcBounds.Height);

                if (this.m_document != null)
                {
                    // SizeF szModel = this.ViewToDevice(this.WorldToView(this.m_document.Size));
                    // szVirtual.Width = (int) Math.Round((double) szModel.Width);
                    // szVirtual.Height = (int) Math.Round((double) szModel.Height);
                }

                return szVirtual;
            }
        }

        /// <summary>
        /// Gets or sets the zoom increment value applied to view attached document.
        /// </summary>
        [Description("Specifies zoom increment value applied to view attached document.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(20)]
        public int ZoomIncrement
        {
            get
            {
                return this.m_iZoomIncrement;
            }
            set
            {
                if (this.m_iZoomIncrement != value && OnPropertyChanging(this.FullContainerName, DPN.ZoomIncrement, value))
                {
                    this.m_iZoomIncrement = value;
                    OnPropertyChanged(this.FullContainerName, DPN.ZoomIncrement);
                }
            }
        }

        /// <summary>
        /// Gets or sets the X and Y magnification (zoom) values on a scale of 1 to n.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value is used to zoom the view in and out. The X and Y axes can
        /// be scaled independently. Normally, the X and Y axes will have the
        /// same magnification value.
        /// </para>
        /// <para>
        /// The value of this property along with the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/> are used to
        /// create the view transform, which is used to map world coordinates onto
        /// view coordinates.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.GetViewTransform()"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
        /// </remarks>
        [Browsable(true)]
        [Description("Specifies magnification value applied to view attached document.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public float Magnification
        {
            get 
            { 
                return m_fMagnification; 
            }
            set
            {
                if (m_fMagnification != value && !float.IsNaN(value) && value > 0 
                    && OnPropertyChanging(this.FullContainerName, DPN.Magnification, value))
                {
                    if (value > CommonUsedValues.MAX_MAGNIFICATION)
                    {                        
                        value = CommonUsedValues.MAX_MAGNIFICATION;
                    }
                    if (value < CommonUsedValues.MIN_MAGNIFICATION)
                    {                        
                        value = CommonUsedValues.MIN_MAGNIFICATION;
                    }
                    float old_mag = m_fMagnification;
                    m_fMagnification = value;
                    m_document.m_fMagnification = value;
                    OnMagnificationChanged(new ViewMagnificationEventArgs(old_mag, value));
                    OnPropertyChanged(FullContainerName, DPN.Magnification);
                }
            }
        }

        /// <summary>
        /// Gets width of left margin in device units.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int LeftMargin
        {
            get
            {
                int value = 0;

                // if (this.verticalRuler != null && this.verticalRuler.Visible)
                // {
                //     value = this.RulerSize;
                // }
                return value;
            }
        }

        /// <summary>
        /// Gets bounding rectangle of left margin in device units.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public System.Drawing.Rectangle LeftMarginBounds
        {
            get
            {
                return new System.Drawing.Rectangle(this.rcBounds.X, this.rcBounds.Y, this.LeftMargin, this.rcBounds.Height);
            }
        }

        /// <summary>
        /// Gets height of top margin in device units.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int TopMargin
        {
            get
            {
                int value = 0;

                // if (this.horizontalRuler != null && this.horizontalRuler.Visible)
                // {
                //    value = this.RulerSize;
                // }
                return value;
            }
        }

        /// <summary>
        /// Gets bounding rectangle of top margin in device units.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public System.Drawing.Rectangle TopMarginBounds
        {
            get
            {
                return new System.Drawing.Rectangle(this.rcBounds.X, this.rcBounds.Y, this.rcBounds.Width, this.TopMargin);
            }
        }

        /// <summary>
        /// Gets bounding rectangle of view excluding the margins.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual System.Drawing.Rectangle WorkArea
        {
            get
            {
                System.Drawing.Rectangle workArea = this.rcBounds;
                workArea.X = workArea.Left + this.LeftMargin + 1;
                workArea.Y = workArea.Top + this.TopMargin + 1;
                return workArea;
            }
        }

        /// <summary>
        /// Gets list of currently selected nodes.
        /// </summary>
        /// <remarks>
        /// Provides access to the controller's selection list.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NodeCollection SelectionList
        {
            get
            {
                if (m_nodesSelected == null)
                {
                    m_nodesSelected = new NodeCollection(this);
                    m_nodesSelected.UpdateReferences = false;
                    m_nodesSelected.UpdateServiceReferences(this);
                }

                return m_nodesSelected;
            }
        }

        /// <summary>
        /// Gets node in the selection list that acts as the anchor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The anchor node is always the last node in the selection list.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.SelectionList"/>
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public INode SelectionAnchorNode
        {
            get
            {
                INode anchorNode = null;

                // if (this.selectionList != null)
                // {
                //    anchorNode = this.selectionList.Last;
                // }
                return anchorNode;
            }
        }

        /// <summary>
        /// Gets or sets the color used to clear the view before rendering the diagram.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The background of the view is the region outside of the visible
        /// diagram.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Background color of the view.")
        ]
        public Color BackgroundColor
        {
            get 
            { 
                return backgroundColor; 
            }
            set
            {
                if (backgroundColor != value && OnPropertyChanging(this.FullContainerName, DPN.BackgroundColor, value))
                {
                    backgroundColor = value;
                    OnPropertyChanged(FullContainerName, DPN.BackgroundColor);
                }
            }
        }

        [DocumentationExclude()]
        private bool ShouldSerializeBackgroundColor()
        {
            return (this.backgroundColor != Color.DarkGray);
        }

        [DocumentationExclude()]
        private void ResetBackgroundColor()
        {
            this.backgroundColor = Color.DarkGray;
        }

        /// <summary>
        /// Gets or sets size of selection handles specified in device coordinates.
        /// OBSOLETE - property is not needed and was never implemented before.
        /// Should be removed in next version
        /// </summary>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Size of selection handles."),
        DefaultValue(6),
        Obsolete("Property is not needed and was never implemented before. Should that be removed in next version")
        ]
        public int HandleSize
        {
            get
            {
                return this.handleSize;
            }
            set
            {
                this.handleSize = value;
            }
        }

        /// <summary>
        /// Gets or sets color used to draw selection handles.
        /// OBSOLETE - property will be no more supported since next version. Use HandleRenderer.HandleColor instead.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Color of selection handles."),
        Obsolete("This property will be no more supported since next version. Use HandleRenderer.HandleColor instead.")
        ]
        public Color HandleColor
        {
            get
            {
                return this.handleColor;
            }
            set
            {
                if (value != this.handleColor && OnPropertyChanging(this.FullContainerName, DPN.HandleColor, value))
                {
                    this.handleColor = value;
                    HandleRenderer.HandleColor = value;
                    OnPropertyChanged(FullContainerName, DPN.HandleColor);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets view handle renderer.
        /// </summary>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("View handle renderer")
        ]
        public HandleRenderer HandleRenderer
        {
            get
            {
                if (m_handleRenderer == null)
                    m_handleRenderer = new HandleRenderer();
                return m_handleRenderer;
            }
            set
            {
                if (value != m_handleRenderer && OnPropertyChanging(this.FullContainerName, DPN.HandleRenderer, value))
                {
                    m_handleRenderer = value;
                    OnPropertyChanged(FullContainerName, DPN.HandleRenderer);
                }
            }
        }

        /// <summary>
        /// Gets or sets view custom handle renderer.
        /// </summary>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Custom handle renderer instance")
        ]
        public UserHandleRenderer CustomHandleRenderer
        {
            get
            {
                return this.m_customHandleRenderer;
            }
            set
            {
                if (value != this.m_customHandleRenderer && OnPropertyChanging(this.FullContainerName, DPN.CustomHandleRenderer, value))
                {
                    this.m_customHandleRenderer = value;
                    OnPropertyChanged(FullContainerName, DPN.CustomHandleRenderer);
                }
            }
        }

        [DocumentationExclude()]
        private bool ShouldSerializeHandleColor()
        {
            return (this.handleColor != Color.GreenYellow);
        }

        [DocumentationExclude()]
        private void ResetHandleColor()
        {
            this.HandleRenderer.HandleColor = Color.GreenYellow;
        }

        /// <summary>
        /// Gets or sets color used to draw selection handle outline.
        /// OBSOLETE - property will be no more supported since next version. Use HandleRenderer.HandleOutlineColor instead.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Color of selection handles."),
        Obsolete("This property will be no more supported since next version. Use HandleRenderer.HandleOutlineColor instead.")
        ]
        public Color HandleOutlineColor
        {
            get
            {
                return this.handleOutlineColor;
            }
            set
            {
                if (value != this.handleOutlineColor && OnPropertyChanging(this.FullContainerName, DPN.HandleOutlineColor, value))
                {
                    this.handleOutlineColor = value;
                    HandleRenderer.HandleOutlineColor = value;
                    OnPropertyChanged(FullContainerName, DPN.HandleOutlineColor);
                }
            }
        }

        [DocumentationExclude()]
        private bool ShouldSerializeHandleOutlineColor()
        {
            return (this.handleOutlineColor != Color.Black);
        }

        [DocumentationExclude()]
        private void ResetHandleOutlineColor()
        {
            this.HandleRenderer.HandleOutlineColor = Color.Black;
        }

        /// <summary>
        /// Gets or sets color used for handles of anchor node. 
        /// OBSOLETE - property is not needed and was never implemented before
        /// should be removed in next version
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Color of selection handles for anchor node."),
        Obsolete("Property is not needed and was never implemented before. Should be removed in next version")
        ]
        public Color HandleAnchorColor
        {
            get
            {
                return this.handleAnchorColor;
            }
            set
            {
                this.handleAnchorColor = value;
            }
        }

        [DocumentationExclude()]
        private bool ShouldSerializeHandleAnchorColor()
        {
            return (this.handleAnchorColor != Color.Gray);
        }

        [DocumentationExclude()]
        private void ResetHandleAnchorColor()
        {
            this.handleAnchorColor = Color.Gray;
        }

        /// <summary>
        /// Gets or sets color used for handles when disabled.
        /// OBSOLETE - property will be no more supported since next version. Use HandleRenderer.HandleDisabledColor instead.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Color of selection handles when disabled."),
        Obsolete("This property will be no more supported since next version. Use HandleRenderer.HandleDisabledColor instead.")
        ]
        public Color HandleDisabledColor
        {
            get
            {
                return this.handleDisabledColor;
            }
            set
            {
                if (value != this.handleDisabledColor && OnPropertyChanging(this.FullContainerName, DPN.HandleDisabledColor, value))
                {
                    this.handleDisabledColor = value;
                    HandleRenderer.HandleDisabledColor = value;
                    OnPropertyChanged(FullContainerName, DPN.HandleDisabledColor);
                }
            }
        }

        [DocumentationExclude()]
        private bool ShouldSerializeHandleDisabledColor()
        {
            return (this.handleDisabledColor != Color.Gray);
        }

        [DocumentationExclude()]
        private void ResetHandleDisabledColor()
        {
            this.HandleRenderer.HandleDisabledColor = Color.Gray;
        }

        /// <summary>
        /// Gets grid of evenly spaced points that provide a visual guide to the
        /// user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Draws a matrix of evenly spaced points in the view and provides
        /// snap to grid calculations.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutGrid"/>
        /// </remarks>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Grid of evenly spaced points that provide a visual guide to the user")]
        public LayoutGrid Grid
        {
            get
            {
                if (m_grid == null)
                {
                    m_grid = this.CreateGrid();
                    m_grid.UpdateServiceReferences(this);
                }

                return m_grid;
            }
        }

        /// <summary>
        /// Gets or sets properties used to draw the page border.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Properties used to draw the page border.")]
        public PageBorderStyle PageBorderStyle
        {
            get 
            { 
                return m_stylePageBorder; 
            }
            set
            {
                if (m_stylePageBorder != value)
                {
                    m_stylePageBorder = value;
                    m_stylePageBorder.UpdateServiceReferences(this);

                    this.EventSink.RaisePropertyChangedEvent(this, DPN.PageBorderStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mouse movements are tracked in the margins of the view.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether mouse tracking is enabled.")]
        [DefaultValue(false)]
        public bool MouseTrackingEnabled
        {
            get { return m_bTrackingEnabled; }
            set { m_bTrackingEnabled = value; }
        }

        /// <summary>
        /// Gets or sets cursor currently used in the view.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Cursor Cursor
        {
            get { return m_cursor; }
            set { m_cursor = value; }
        }

        /// <summary>
        /// Gets or sets number of device units to offset nodes in the X direction when they are
        /// pasted into a diagram.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Defines X direction offset when nodes are pasted into the diagram.")]
        [DefaultValue(10)]
        public int PasteOffsetX
        {
            get { return m_nPasteOffsetX; }
            set { m_nPasteOffsetX = value; }
        }

        /// <summary>
        /// Gets or sets number of device units to offset nodes in the Y direction when they are
        /// pasted into a diagram.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Defines Y direction offset when nodes are pasted into the diagram..")]
        [DefaultValue(10)]
        public int PasteOffsetY
        {
            get { return m_nPasteOffsetY; }
            set { m_nPasteOffsetY = value; }
        }

        /// <summary>
        /// Gets or sets page settings to use when creating a print document for the model.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public PageSettings PageSettings
        {
            get
            {
                if (this.pgSettings == null)
                    this.RefreshPageSettings();

                return this.pgSettings;
            }
            set
            {
                if (this.pgSettings != value)
                {
                    this.pgSettings = value;
                    this.RefreshPageSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets the diagram printing zoom value.
        /// <see cref="Syncfusion.Windows.Forms.Diagram.PrintZoom"/>
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public PrintZoom PrintZoom
        {
            get
            {
                if (this.prtZoom == null)
                    prtZoom = new PrintZoom();
                return prtZoom;
            }
            set
            {
                if (this.prtZoom != value)
                    this.prtZoom = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds of scrollable area. Can be set only positive values.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Description("Determines the bounds of scrollable area.")]
        [Browsable(false)]
        public RectangleF ScrollVirtualBounds
        {
            get
            {
                return m_rcVirtualBounds;
            }
            set
            {
                if (value != m_rcVirtualBounds && value.X >= 0 && value.Y >= 0 && value.Width >= 0 && value.Height >= 0)
                {
                    RectangleF rcOldValue = m_rcVirtualBounds;
                    m_rcVirtualBounds = value;

                    m_eventSink.RaiseScrollVirtualBoundsChanged(new ViewScrollVirtualBoundsEventArgs(rcOldValue, value));
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        protected Size PageSize
        {
            get
            { 
                return m_pageSize; 
            }
            set
            {
                if (m_pageSize != value)
                {
                    m_pageSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether page size can be used.
        /// </summary>
        /// <value><c>true</c> if page size can be used; otherwise, <c>false</c>.</value>
        protected bool PageSizeKnown
        {
            get 
            { 
                return m_pageSizeKnown; 
            }
            set
            {
                if (m_pageSizeKnown != value)
                {
                    m_pageSizeKnown = value;
                }
            }
        }
                
        /// <summary>
        /// Gets or sets a value indicating that which type of zooming action to be performed.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The type of the Zooming action to be performed.")]
        public ZoomType ZoomType
        {
            get { return m_zoomType; }
            set { m_zoomType = value; }
        }
        #endregion

        #region Styles
        /// <summary>
        /// Gets the handle styles collection.
        /// </summary>
        /// <value>The handle styles.</value>
        [Browsable(true)]
        [Description("Determines hanlde styles for attached document.")]
        [Category("Appearance")]
        public static HandleStyles HandleStyles
        {
            get { return m_styleHandles; }
        }
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                m_document = null;
                m_eventSink = null;
            }
            else
            {
                m_document = (Model)provider.ProvideServiceReference(typeof(Model).TypeHandle);
                m_eventSink = (ViewerEventSink)provider.ProvideServiceReference(typeof(ViewerEventSink).TypeHandle);
            }

            this.SelectionList.UpdateServiceReferences(provider);
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
        /// <param name="strPropertyName">Name of the property.</param>
        /// <returns>The object.</returns>
        public object GetPropertyContainerByName(string strPropertyName)
        {
            object objToReturn = null;

            switch (strPropertyName)
            {
                case "Grid":
                    objToReturn = m_grid;
                    break;
            }

            return objToReturn;
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
        public bool OnPropertyChanging(string strPropertyContainerName, string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                bSuccess = this.EventSink.RaisePropertyChangingEvent(this, strPropName, newValue);
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public void OnPropertyChanged(string strPropertyContainerName, string strPropertyName)
        {
            if (this.EventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                this.EventSink.RaisePropertyChangedEvent(this, strPropName);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Zoom in the document.
        /// </summary>
        public void ZoomIn()
        {
            if ((m_fMagnification + m_iZoomIncrement) > CommonUsedValues.MAX_MAGNIFICATION)
            {
                this.Magnification = CommonUsedValues.MAX_MAGNIFICATION;
            }
            else
                this.Magnification += this.ZoomIncrement;
        }

        /// <summary>
        /// Zoom out the document.
        /// </summary>
        public void ZoomOut()
        {
            if ((m_fMagnification - m_iZoomIncrement) < CommonUsedValues.MIN_MAGNIFICATION)
            {
                this.Magnification = CommonUsedValues.MIN_MAGNIFICATION;
            }
            else
                this.Magnification -= this.ZoomIncrement;
        }

        /// <summary>
        /// Zoom the document to the actual.
        /// </summary>
        public void ZoomToActual()
        {
            this.Magnification = 100;
        }

        /// <summary>
        /// Zoom the document to the selection
        /// </summary>
        /// <param name="rectSelection">The selection rectangle in client coordinates.</param>
        public void ZoomToSelection(RectangleF rectSelection)
        {
            if (rectSelection.Width != 0 && rectSelection.Height != 0)
            {
                float magWidth = this.Magnification;
                Size szSize = (this.ClientRectangle != RectangleF.Empty) ? this.ClientRectangle.Size : this.Size;

                // convert bounds to model coordinates
                rectSelection = this.Controller.ConvertToModelCoordinates(rectSelection);
                szSize = this.Controller.ConvertToModelCoordinates(szSize);

                // calc zoom factor
                double dWidth = (double)szSize.Width / (double)rectSelection.Width;
                double dHeight = (double)szSize.Height / (double)rectSelection.Height;
                magWidth *= (float)Math.Min(dWidth, dHeight);

                if (magWidth != this.Magnification)
                {
                    //Gets the Diagram view's current zoom type.
                    ZoomType viewType = this.Controller.View.ZoomType;
                    //sets the View's current zoomtype as 'TopLeft' in order to perform the frame zooming.
                    this.Controller.View.ZoomType = ZoomType.TopLeft;
                    PointF ptLocation = this.Controller.ConvertToModelCoordinates(rectSelection.Location);
                    this.Controller.View.Origin = this.Controller.ConvertToModelCoordinates(ptLocation);
                    this.Magnification = magWidth;
                    //revert back the view's zoomtype to current zoomtype
                    this.ZoomType = viewType;
                }
            }
        }

        /// <summary>
        /// Fit the document on control field.
        /// </summary>
        public void FitDocument()
        {
            SizeF szModel = m_document.LogicalSize;
            SizeF szControl = this.Size;

            // calc fit margin
            float fHorizontalMargin = (float)this.LeftMargin;
            float fVerticalMargin = (float)this.TopMargin;

            float fWidthFactor = (szControl.Width - fHorizontalMargin) / szModel.Width;
            float fHeightFactor = (szControl.Height - fVerticalMargin) / szModel.Height;

            // calculate new magnification factor
            float fMagnification = Math.Min(fWidthFactor, fHeightFactor);

            float fOriginX = szControl.Width / 2 - szModel.Width * (fMagnification / 100f) / 2;
            float fOriginY = 0f;

            // calculate new original
            PointF ptOrigin = new PointF(fOriginX, fOriginY);

            // set new origin offset and magnification
            this.Magnification = fMagnification;
            this.Origin = ptOrigin;
        }

        /// <summary>
        /// Refreshes the view's page size with the new bounds obtained from the <see cref="View.PageSettings"/> property.
        /// </summary>
        /// <remarks>
        /// Call this method when the <see cref="Syncfusion.Windows.Forms.Diagram.View"/>'s page settings have undergone a change.
        /// </remarks>
        public virtual void RefreshPageSettings()
        {
            if (this.pgSettings == null)
            {
                this.pgSettings = new PageSettings();
                this.pgSettings.Margins = new Margins(0, 0, 0, 0);
            }

            PageSettings pagesettings = this.pgSettings;
            try
            {
                if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count > 0 && pagesettings.PrinterSettings.IsValid)
                {
                    int nMarginsWidth = pagesettings.Bounds.Size.Width - (pagesettings.Margins.Left + pagesettings.Margins.Right);
                    int nMarginsHeight = pagesettings.Bounds.Size.Height - (pagesettings.Margins.Top + pagesettings.Margins.Bottom);
                    this.PageSize = new Size(nMarginsWidth, nMarginsHeight);
                    if (m_document != null)
                        this.m_document.HeaderFooterData.InitializeHeaderFooterBounds(pagesettings);
                    this.PageSizeKnown = true;
                }
                else
                {
                    if (m_document != null)
                        this.m_document.HeaderFooterData.InitializeHeaderFooterBounds(HeaderFooterData.DefaultPaperSize, new Margins(0, 0, 0, 0));
                    this.PageSizeKnown = false;
                }
            }
            catch
            {
                this.PageSizeKnown = false;
            }
        }

        /// <summary>
        /// Returns a transformation matrix that maps world coordinates to view
        /// coordinates.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        /// <remarks>
        /// <para>
        /// The view transformation maps world coordinates to view coordinates.
        /// It is calculated by translating by the offset specified in the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
        /// property and scaling by the value in the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
        /// property.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
        /// </remarks>
        public virtual Matrix GetViewTransform()
        {           
            return GetViewTransform(this.m_ptOrigin);
        }

        /// <summary>
        /// Returns transform matrix based on the given origin.
        /// </summary>
        /// <param name="origin">origin to tranform</param>
        /// <returns>Transformation matrix.</returns>
        public virtual Matrix GetViewTransform(PointF origin)
        {
            Matrix viewTransform = new Matrix();

            viewTransform.Translate(-origin.X, -origin.Y);
            viewTransform.Translate(this.LeftMargin, this.TopMargin);

            return viewTransform;
        }

        /// <summary>
        /// Scrolls the view origin by a given X and Y offset.
        /// </summary>
        /// <param name="dx">X offset.</param>
        /// <param name="dy">Y offset.</param>
        public virtual void ScrollBy(float dx, float dy)
        {
            if (this.parentControl != null)
            {
                this.m_ptOrigin.X += dx;
                this.m_ptOrigin.Y += dy;
                this.parentControl.Invalidate();
            }
        }

        /// <summary>
        /// Scrolls to the node to bring it into view.
        /// </summary>
        /// <param name="node">The node to bring into view</param>
        public void ScrollToNode(Node node)
        {
            this.Model.BeginUpdate();
            boundingRect = ((IUnitIndependent)node).GetBoundingRectangle(this.Model.MeasurementUnits, true);

            origin = new PointF(boundingRect.X - DEF_ORIGIN_OFFSET, boundingRect.Y - DEF_ORIGIN_OFFSET);

            this.Origin = origin;
            this.Model.EndUpdate();
        }        

        /// <summary>
        /// Scrolls to the invisible node to bring it into view.
        /// </summary>
        /// <param name="node">The node to bring into view</param>        
        /// <param name="position">The position to place the node</param>
        public void ScrollToInvisibleNode(Node node, Positions position)
        {
            this.Model.BeginUpdate();          
            RectangleF visibleBounds = new RectangleF( PointF.Empty, this.ClientRectangle.Size);

            RectangleF nodeBounds = this.Controller.ConvertFromModelToClientCoordinates(node.BoundingRectangle);           
            if (!visibleBounds.Contains(nodeBounds))
            {
                if (position == Positions.Absolute)
                {
                        float dx = 0, dy = 0;
                        if (nodeBounds.Left < visibleBounds.Left)
                            dx = nodeBounds.Left - visibleBounds.Left - 10;
                        else if (nodeBounds.Right > visibleBounds.Right)
                            dx = nodeBounds.Right - visibleBounds.Right + 10;

                        if (nodeBounds.Top < visibleBounds.Top)
                            dy = nodeBounds.Top - visibleBounds.Top - 10;
                        else if (nodeBounds.Bottom > visibleBounds.Bottom)
                            dy = nodeBounds.Bottom - visibleBounds.Bottom + 10;
                        this.Origin = PointF.Add(this.Origin, this.Controller.ConvertToModelCoordinates(new SizeF(dx, dy)));                   
                }
                else
                    this.ScrollToNode(node);
            }            
            this.Model.EndUpdate();
        }

        /// <summary>
        /// Scrolls to the invisible node to bring it into view.
        /// </summary>
        /// <param name="nodeName">The node name</param>        
        /// <param name="position">The position to place the node</param>
        public void ScrollToInvisibleNode(string nodeName, Positions position)
        {
            Node node = this.Model.GetChildByName(nodeName);
            if (node != null)
                ScrollToInvisibleNode(node, position);
        }

        /// <summary>
        /// Scrolls to the node to bring it into view.
        /// </summary>
        /// <param name="nodeName">The node name</param>
        public void ScrollToNode(string nodeName)
        {
            Node node = this.Model.GetChildByName(nodeName);

            if (node != null)
            {
                this.Model.BeginUpdate();
                boundingRect = ((IUnitIndependent)node).GetBoundingRectangle(this.Model.MeasurementUnits, true);

                origin = new PointF(boundingRect.X - DEF_ORIGIN_OFFSET, boundingRect.Y - DEF_ORIGIN_OFFSET);

                this.Origin = origin;
                this.Model.EndUpdate();
            }
        }

        /// <summary>
        /// Takes a device point and returns the nearest grid point.
        /// </summary>
        /// <param name="ptDevIn">Point to snap.</param>
        /// <returns>Point on the grid nearest the input point.</returns>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Grid"/>
        /// </remarks>
        public PointF SnapPointToGrid(PointF ptDevIn)
        {
            LayoutGrid layoutGrid = this.Grid;

            if (layoutGrid != null && layoutGrid.SnapToGrid)
            {
                return layoutGrid.GetNearestGridPoint(ptDevIn);
            }

            return ptDevIn;
        }

        /// <summary>
        /// Takes a device point and returns the nearest grid point.
        /// </summary>
        /// <param name="x">X coordinate of point to snap.</param>
        /// <param name="y">Y coordinate of point to snap.</param>
        /// <returns>Point on the grid nearest the input point.</returns>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Grid"/>
        /// </remarks>
        public PointF SnapPointToGrid(float x, float y)
        {
            LayoutGrid layoutGrid = this.Grid;

            if (layoutGrid != null && layoutGrid.SnapToGrid)
            {
                return layoutGrid.GetNearestGridPoint(new PointF(x, y));
            }

            return new PointF(x, y);
        }

        /// <summary>
        /// Called during PageSetup dialog initialization to workaround a measurement units conversion bug in the PageSetupDialog.
        /// </summary>
        /// <param name="beforePageSetup">TRUE for pre-PageSetup invocation.</param>
        public void ConvertPageMargins(bool beforePageSetup)
        {
            PageSettings pagesettings = this.PageSettings;
            if (pagesettings.PrinterSettings.IsValid)
            {
                bool isMetric = IsMetricCurrentMeasureUnits();
                if (isMetric)
                {
                    PrinterUnit from = beforePageSetup ? PrinterUnit.ThousandthsOfAnInch : PrinterUnit.HundredthsOfAMillimeter;
                    PrinterUnit to = beforePageSetup ? PrinterUnit.HundredthsOfAMillimeter : PrinterUnit.ThousandthsOfAnInch;

                    if (beforePageSetup)
                    {
                        pagesettings.PrinterSettings.DefaultPageSettings.Margins =
                            PrinterUnitConvert.Convert(pagesettings.PrinterSettings.DefaultPageSettings.Margins, from, to);
                        pagesettings.Margins = PrinterUnitConvert.Convert(pagesettings.Margins, from, to);
                    }
                    else
                    {
                        pagesettings.Margins = PrinterUnitConvert.Convert(pagesettings.Margins, from, to);
                    }
                }
            }
        }

        private bool IsMetricCurrentMeasureUnits()
        {
            bool bIsMetric = false;
            if (CultureInfo.CurrentCulture.UseUserOverride)
            {
                int nCharNumner = 2;
                StringBuilder strbldToReturn = new StringBuilder(nCharNumner);
                LocaleInfo.GetLocaleInfo(
                    LocaleInfo.LOCALE_USER_DEFAULT, LocaleInfo.LOCALE_IMEASURE, strbldToReturn, nCharNumner);
                if (int.Parse(strbldToReturn.ToString()) == 0)
                    bIsMetric = true;
            }
            else
            {
                RegionInfo region = RegionInfo.CurrentRegion;
                bIsMetric = region.IsMetric;
            }
            return bIsMetric;
        }

        #endregion

        #region Rendering
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        public virtual void Draw(Graphics grfx)
        { 
        }

        /// <summary>
        /// Renders the view onto a System.Drawing.Graphics context object.
        /// </summary>
        /// <param name="grfx">Graphics context object to render to.</param>
        /// <param name="rectClip">Specifies document's area which will be rendered.</param>
        /// <remarks>
        /// <para>
        /// This method first renders the view onto the back buffer. It fills
        /// the buffer with the view's background color, draws the grid and
        /// page bounds, and then draws the model. Then paints the back
        /// buffer onto the graphics context.
        /// </para>
        /// </remarks>
        public virtual void Draw(Graphics grfx, RectangleF rectClip)
        {
            // Clear
			if(this.BackgroundColor != Color.Transparent)
            grfx.Clear(this.BackgroundColor);

            // Append model's Renderering style
            this.Model.RenderingStyle.ApplySettings(grfx);

            // BACKGROUND
            RectangleF rectModel = MeasureUnitsConverter.ToPixels(this.Model.Bounds, this.Model.MeasurementUnits);

            // Render document backrgound
            DrawDocumentBackground(grfx, rectModel);

            // GRID
            // grid rectangle needed to refresh
            RectangleF rectGrid = RectangleF.Intersect(rectModel, grfx.ClipBounds);
            if (rectGrid.Width > rectClip.Width)
                rectGrid.Width = rectClip.Width;
            if (rectGrid.Height > rectClip.Height)
                rectGrid.Height = rectClip.Height;

            // draw Grid
            this.Grid.Draw(grfx, rectGrid);

            // PAGE BORDERS
            if (this.PageBorderStyle.ShowBorder)
            {
                DrawPageBorders(grfx);
            }

            // DOCUMENT
            if (this.Model != null)
            {
                // normalize clip rectm
                this.Model.Draw(grfx, rectClip);
            }

            // SELECTION HANDLES
            NodeCollection nodesSelected = (this.Model.EnableSelectionListSubstitute && this.SelectionList.Count > 1) ? this.SelectionListSubstitute : this.SelectionList;
            DrawHandles(grfx, nodesSelected);
        }

        /// <summary>
        /// Draws the document background.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="rectDocument">The document bounds.</param>
        protected virtual void DrawDocumentBackground(Graphics gfx, RectangleF rectDocument)
        {
            ShadowStyle styleShadow = this.Model.ShadowStyle;

            if (styleShadow.Visible)
            {
                // Draw document's shadow
                using (Brush brushShadow = styleShadow.CreateBrush(gfx, rectDocument))
                {
                    // Apply shadow offset
                    GraphicsState save = gfx.Save();
                    float fOffsetX = MeasureUnitsConverter.ToPixelX(styleShadow.OffsetX, styleShadow.MeasureUnit);
                    float fOffsetY = MeasureUnitsConverter.ToPixelY(styleShadow.OffsetY, styleShadow.MeasureUnit);

                    gfx.TranslateTransform(fOffsetX, fOffsetY, MatrixOrder.Append);

                    gfx.FillRectangle(brushShadow, rectDocument);

                    // restore graphics state
                    gfx.Restore(save);
                }
            }

            // Draw document's background
            using (Brush brushFill = this.Model.BackgroundStyle.CreateBrush(gfx, new RectangleF(PointF.Empty, rectDocument.Size)))
            {
                gfx.FillRectangle(brushFill, Geometry.ConvertRectangle(new RectangleF(PointF.Empty, rectDocument.Size)));
            }
            if (this.Model.BackgroundImage != null)
            {
                ImageLayout imageLayout = this.Model.BackgroundImageLayout;
                Image imgBackground = this.Model.BackgroundImage;

                GraphicsState state = gfx.Save();
                RectangleF rect;
                float left, right, top;
                ImageAttributes attr = new ImageAttributes();
                //RectangleF bounds = new RectangleF(this.Location.X, this.Location.Y, this.Width, this.Height);
                RectangleF bounds = MeasureUnitsConverter.ToPixels(this.Model.Bounds, this.Model.MeasurementUnits);
                Bitmap bmp = new Bitmap((int)bounds.Width, (int)bounds.Height);
                using (Graphics gfxBg = Graphics.FromImage(bmp))
                {
                    gfxBg.FillRectangle(new SolidBrush(Color.White), bounds);
                    switch (imageLayout)
                    {
                        case ImageLayout.Center:
                            rect = new RectangleF(bounds.X + (bounds.Width / 2 - imgBackground.Width / 2), bounds.Y + bounds.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                            gfxBg.DrawImage(imgBackground, rect);
                            break;
                        case ImageLayout.Tile:
                            attr.SetWrapMode(WrapMode.Tile);
                            // Location of three edges of rectangle for drawingrfx.
                            PointF p1 = new PointF(bounds.X, bounds.Y);
                            PointF p2 = new PointF(bounds.Right, bounds.Y);
                            PointF p3 = new PointF(p1.X, bounds.Bottom);
                            PointF[] destPoints = { p1, p2, p3 };
                            RectangleF srcRect = new RectangleF(0, 0, bounds.Width, bounds.Height);
                            gfxBg.DrawImage(imgBackground, destPoints, srcRect, GraphicsUnit.Pixel, attr);
                            break;
                        case ImageLayout.Stretch:
                            gfxBg.InterpolationMode = InterpolationMode.Bicubic;
                            left = bounds.X;
                            top = bounds.Y;
                            rect = new RectangleF(left, top, bounds.Width, bounds.Height);
                            gfxBg.DrawImage(imgBackground, rect);
                            break;
                        case ImageLayout.AlignLeft:
                            left = bounds.X;
                            rect = new RectangleF(left, bounds.Y + bounds.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                            gfxBg.DrawImage(imgBackground, rect);
                            break;
                        case ImageLayout.AlignRight:
                            right = bounds.X + (bounds.Width - imgBackground.Width);
                            rect = new RectangleF(right, bounds.Y + bounds.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                            gfxBg.DrawImage(imgBackground, rect);
                            break;
                    }
                }
                Bitmap clip = new Bitmap((int)rectDocument.Width, (int)rectDocument.Height);
                using (Graphics gfxClip = Graphics.FromImage(clip))
                {
                    gfxClip.DrawImage(bmp, new RectangleF(0, 0, clip.Width, clip.Height), rectDocument, GraphicsUnit.Pixel);
                }
                gfx.DrawImage(clip, 0, 0);
                gfx.Restore(state);
            }

            // Outline document's bounds
            if (this.Model.LineStyle.LineWidth > 0)
            {
                RectangleF rcBounds = rectDocument;
                float dx = (float)Math.Floor(MeasureUnitsConverter.ToPixelX(this.Model.LineStyle.LineWidth, this.Model.MeasurementUnits) / 2);
                rcBounds.Inflate(-dx, -dx);
                using (Pen penUotline = this.Model.LineStyle.CreatePen())
                {
                    if (rectDocument.X <= 0)
                    {
                        gfx.DrawLine(penUotline, new PointF(dx, 0), new PointF(dx, rcBounds.Height + (2 * dx)));
                    }
                    if (rectDocument.Y <= 0)
                    {
                        gfx.DrawLine(penUotline, new PointF(0, dx), new PointF(rcBounds.Width + (2 * dx), dx));
                    }
                    if (rectDocument.X + rectDocument.Width >= this.Model.Bounds.Width)
                    {
                        gfx.DrawLine(penUotline, new PointF(rcBounds.Width + dx, 0), new PointF(rcBounds.Width + dx, rcBounds.Height + (2 * dx)));
                    }
                    if (rectDocument.Y + rectDocument.Height >= this.Model.Bounds.Height)
                    {
                        gfx.DrawLine(penUotline, new PointF(0, rcBounds.Height + dx), new PointF(rcBounds.Width + (2 * dx), rcBounds.Height + dx));
                    }
                }
            }
        }

        /// <summary>
        /// Draws the upper-left hand corner of the margin where the left and top
        /// margins intersect.
        /// </summary>
        /// <param name="grfx">Graphics context to render to.</param>
        /// <remarks>
        /// <para>
        /// The default implementation fills the rectangular area with the
        /// ruler fill color.
        /// </para>
        /// </remarks>
        protected virtual void DrawMarginIntersection(Graphics grfx)
        {
            // System.Drawing.Rectangle rcRulerIntersection = new System.Drawing.Rectangle(this.rcBounds.X, this.rcBounds.Y, this.LeftMargin, this.TopMargin);
            // Brush brush = new SolidBrush(this.RulerFillColor);
            // grfx.FillRectangle(brush, rcRulerIntersection);
            // brush.Dispose();
        }

        /// <summary>
        /// Exports a representation of the diagram as a bitmap image.
        /// </summary>
        /// <param name="bClipModelBounds">if set to <c>true</c> [b clip model bounds].</param>
        /// <returns>
        /// A <see cref="System.Drawing.Bitmap"/> value.
        /// </returns>
        public virtual Image ExportDiagramAsImage(bool bClipModelBounds)
        {
            Image imgToReturn = null;
            Graphics gfx;
            RectangleF rectBounding = RectangleF.Empty;

            if (this.Model != null)
            {
                if (!bClipModelBounds)
                {
                    // Get Diagram Nodes Bounding Rectangle.
                    rectBounding = this.Model.GetBoundingRect();
                    float borderWidth = (this.Model.LineStyle.LineWidth < 1) ? 1 : this.Model.LineStyle.LineWidth;
                    // Create Image of Model Nodes Bounding Rectangle size.
                    imgToReturn = new Bitmap((int)(Math.Ceiling(rectBounding.Width) + borderWidth), (int)(Math.Ceiling(rectBounding.Height) + borderWidth), PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(imgToReturn);

                    float fNegOffsetY;
                    float fNegOffsetX;
                    ConsiderModelLocation(rectBounding, out fNegOffsetX, out fNegOffsetY);

                    gfx.TranslateTransform(fNegOffsetX, fNegOffsetY, MatrixOrder.Append);
                }
                else
                {
                    SizeF modelSize = this.Model.LogicalSize;
                    imgToReturn = new Bitmap(
                                    (int)(modelSize.Width * this.Magnification / 100f),
                                    (int)(modelSize.Height * this.Magnification / 100f),
                                    PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(imgToReturn);

                    rectBounding = new RectangleF(0, 0, imgToReturn.Width, imgToReturn.Height);
                }

                // Draw Diagram to graphics.
                this.ExportDiagramToGraphics(gfx, rectBounding);
            }

            return imgToReturn;
        }
        private void ConsiderModelLocation(RectangleF rectNodesBounding, out float fNegOffsetX, out float fNegOffsetY)
        {
            fNegOffsetY = 0;
            fNegOffsetX = 0;

            if (rectNodesBounding.X < 0)
                fNegOffsetX = -rectNodesBounding.X;

            if (rectNodesBounding.Y < 0)
                fNegOffsetY = -rectNodesBounding.Y;
        }

        /// <summary>
        /// Draw the diagram to specified graphics.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="rcDrawArea">The drawing area.</param>
        public void ExportDiagramToGraphics(Graphics grfx, RectangleF rcDrawArea)
        {
            if (this.Model != null)
            {
                // Cache the view's current origin, size and magnification.
                float fPreviousMagnification = m_fMagnification;

                // Set the view's magnification to encompass the whole diagram.
                m_fMagnification = CommonUsedValues.fHUNDRED_PERCENT;

                grfx.Clear(this.BackgroundColor);

                // Push the view transform onto the stack.
                grfx.PageUnit = GraphicsUnit.Pixel;
                grfx.PageScale = m_fMagnification / CommonUsedValues.fHUNDRED_PERCENT;

                // Append model's Renderering style
                this.Model.RenderingStyle.ApplySettings(grfx);

                // Fill bounds of model with background color.
                DrawDocumentBackground(grfx, rcDrawArea);

                // Apply the view transformation onto the .bmp graphic and render the model's contents.
                Matrix matrix = GetViewTransform(rcDrawArea.Location);
                grfx.MultiplyTransform(matrix);
                grfx.SetClip(rcDrawArea);

                // Draw the diagram model.
                [REDACTED] hash = new [REDACTED]();
                this.m_document.CheckUseBitmap(this.m_document, hash);
                this.m_document.Draw(grfx, rcDrawArea);
                this.m_document.UnCheckUseBitmap(this.m_document, hash);

                // Restore the magnification
                m_fMagnification = fPreviousMagnification;
            }
        }

        /// <summary>
        /// Renders a representation of the diagram onto the provided <see cref="System.Drawing.Graphics"/> object.
        /// </summary>
        /// <param name="grfx">The graphics.</param>
        public virtual void ExportDiagramToGraphics(Graphics grfx)
        {
            this.ExportDiagramToGraphics(grfx, false);
        }

        /// <summary>
        /// Renders a representation of the diagram onto the provided <see cref="System.Drawing.Graphics"/> object.
        /// </summary>
        /// <param name="grfx">The graphics.</param>
        /// <param name="bClipModelBounds">if set to <c>true</c> clip model bounds.</param>
        public virtual void ExportDiagramToGraphics(Graphics grfx, bool bClipModelBounds)
        {
            if (bClipModelBounds)
            {
                ExportDiagramToGraphics(grfx, RectangleF.Empty);
            }
            else
            {
                // Get Diagram Nodes Bounding Rectangle.
                RectangleF rectBounding = this.Model.GetBoundingRect();

                float fNegOffsetY;
                float fNegOffsetX;
                ConsiderModelLocation(rectBounding, out fNegOffsetX, out fNegOffsetY);

                grfx.TranslateTransform(fNegOffsetX, fNegOffsetY, MatrixOrder.Append);

                ExportDiagramToGraphics(grfx, rectBounding);
            }
        }

        /// <summary>
        /// Draws lines on the view that indicate where page boundaries exist.
        /// </summary>
        /// <param name="grfx">Graphics context object on which to draw.</param>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.PageSettings"/>
        protected virtual void DrawPageBounds(Graphics grfx)
        {
            if (this.m_document != null && this.PageSizeKnown)
            {
                RectangleF mdlWorldBounds = new RectangleF(new PointF(0, 0), m_document.LogicalSize);
                float fMdlLeft = mdlWorldBounds.Left;
                float fMdlTop = mdlWorldBounds.Top;
                float fMdlRight = mdlWorldBounds.Right;
                float fMdlBottom = mdlWorldBounds.Bottom;

                float pageWidth = MeasureUnitsConverter.FromPixelX(this.PageSize.Width, this.Model.MeasurementUnits);
                float pageHeight = MeasureUnitsConverter.FromPixelY(this.PageSize.Height, this.Model.MeasurementUnits);

                Pen pen = new Pen(Color.DarkBlue, 0);
                pen.DashStyle = DashStyle.Dash;
                PointF pt1 = new PointF(fMdlLeft, fMdlTop); // new System.Drawing.PointF(0.0f,0.0f);
                PointF pt2 = new PointF(fMdlLeft, fMdlTop); // new System.Drawing.PointF(0.0f,0.0f);

                // Draw vertical page bounds.
                bool done = false;
                pt1.Y = fMdlTop;
                pt2.Y = fMdlBottom;
                float curX = fMdlLeft;
                while (!done)
                {
                    curX += pageWidth;
                    if (curX > fMdlRight)
                    {
                        done = true;
                    }
                    else
                    {
                        pt1.X = curX;
                        pt2.X = curX;
                        grfx.DrawLine(pen, pt1, pt2);
                    }
                }

                // Draw horizontal page bounds.
                done = false;
                pt1.X = fMdlLeft;
                pt2.X = fMdlRight;
                float curY = fMdlTop;
                while (!done)
                {
                    curY += pageHeight;
                    if (curY > fMdlBottom)
                    {
                        done = true;
                    }
                    else
                    {
                        pt1.Y = curY;
                        pt2.Y = curY;
                        grfx.DrawLine(pen, pt1, pt2);
                    }
                }

                pen.Dispose();
            }
        }

        /// <summary>
        /// Draws a border around the page.
        /// </summary>
        /// <param name="grfx">Graphics context object on which to draw.</param>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.PageBorderStyle"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.PageBorderStyle"/>
        protected virtual void DrawPageBorders(Graphics grfx)
        {
            if (m_document != null && this.PageSizeKnown)
            {
                // as a workaround for pen width with GraphicsUnit.Inch & Milimeters
                // all is converted to pixel units
                SizeF szPixelModelBounds = SizeF.Empty;
                SizeF szModel = m_document.LogicalSize;
                szPixelModelBounds.Width = MeasureUnitsConverter.ToPixelX(szModel.Width, this.Model.MeasurementUnits);
                szPixelModelBounds.Height = MeasureUnitsConverter.ToPixelX(szModel.Height, this.Model.MeasurementUnits);

                float pageWidth = MeasureUnitsConverter.FromPixelX(this.PageSize.Width, this.Model.MeasurementUnits);
                float pageHeight = MeasureUnitsConverter.FromPixelX(this.PageSize.Height, this.Model.MeasurementUnits);

                pageWidth = MeasureUnitsConverter.ToPixelX(pageWidth, this.Model.MeasurementUnits);
                pageHeight = MeasureUnitsConverter.ToPixelX(pageHeight, this.Model.MeasurementUnits);

                // for real dimensions
                pageWidth = pageWidth * (MeasureUnitsConverter.DpiX / 100f);
                pageHeight = pageHeight * (MeasureUnitsConverter.DpiY / 100f);

                // no borders if either of page dimensions is less than zero
                if (pageWidth < 0 || pageHeight < 0) return;

                RectangleF rectPage = new RectangleF(new PointF(0, 0), new SizeF(pageWidth, pageHeight));

                float fRoundingOffset = (int)this.PageBorderStyle.BorderCornerRounding;
                GraphicsUnit gUnit = grfx.PageUnit;
                grfx.PageUnit = GraphicsUnit.Pixel;

                using (Pen pen = this.PageBorderStyle.CreatePen())
                {
                    pen.Width = MeasureUnitsConverter.Convert(pen.Width * 100, MeasureUnits.Pixel, MeasureUnits.Inch);

                    while (rectPage != RectangleF.Empty)
                    {
                        DrawBoundingRectangle(grfx, pen, rectPage, fRoundingOffset);
                        NextPage(ref rectPage, szPixelModelBounds.Width, szPixelModelBounds.Height);
                    }
                }

                grfx.PageUnit = gUnit;
            }
        }
        private void DrawBoundingRectangle(Graphics gph, Pen pen, RectangleF rectPage, float fRoundingOffset)
        {
            float fHalfPenWidth = pen.Width / 2;

            gph.DrawLine(
                pen, 
                new PointF(rectPage.X + fRoundingOffset - fHalfPenWidth, rectPage.Y - fHalfPenWidth),
                new PointF(rectPage.Right - fRoundingOffset - fHalfPenWidth, rectPage.Y - fHalfPenWidth));
            gph.DrawLine(
                pen, 
                new PointF(rectPage.Right - fHalfPenWidth, rectPage.Y + fRoundingOffset - fHalfPenWidth),
                new PointF(rectPage.Right - fHalfPenWidth, rectPage.Bottom - fRoundingOffset - fHalfPenWidth));
            gph.DrawLine(
                pen, 
                new PointF(rectPage.X + fRoundingOffset - fHalfPenWidth, rectPage.Bottom - fHalfPenWidth),
                new PointF(rectPage.Right - fRoundingOffset - fHalfPenWidth, rectPage.Bottom - fHalfPenWidth));
            gph.DrawLine(
                pen, 
                new PointF(rectPage.X - fHalfPenWidth, rectPage.Y + fRoundingOffset - (pen.Width / 2)),
                new PointF(rectPage.X - fHalfPenWidth, rectPage.Bottom - fRoundingOffset - fHalfPenWidth));

            if (fRoundingOffset > 0)
            {
                RectangleF rectArc = new RectangleF(new PointF(0, 0), new SizeF(fRoundingOffset * 2, fRoundingOffset * 2));
                rectArc.Location = new PointF(rectPage.Right - (fRoundingOffset * 2) - fHalfPenWidth, rectPage.Y - fHalfPenWidth);
                gph.DrawArc(pen, rectArc, 270, 90);
                rectArc.Location = new PointF(rectPage.Right - (fRoundingOffset * 2) - fHalfPenWidth, rectPage.Bottom - (fRoundingOffset * 2) - fHalfPenWidth);
                gph.DrawArc(pen, rectArc, 0, 90);
                rectArc.Location = new PointF(rectPage.X - fHalfPenWidth, rectPage.Bottom - (fRoundingOffset * 2) - fHalfPenWidth);
                gph.DrawArc(pen, rectArc, 90, 90);
                rectArc.Location = new PointF(rectPage.X - fHalfPenWidth, rectPage.Y - fHalfPenWidth);
                gph.DrawArc(pen, rectArc, 180, 90);
            }
        }

        private void NextPage(ref RectangleF rectPage, float fModelWidth, float fModelHeight)
        {
            if (rectPage.Right > fModelWidth)
            {
                // if no more pages in current row -- proceed to next row
                if (rectPage.Bottom > fModelHeight)
                    rectPage = RectangleF.Empty;
                else
                {
                    rectPage.X = 0;
                    rectPage.Y += rectPage.Height;
                }
            }
            else
            {
                rectPage.X += rectPage.Width;
            }
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
            return this.GetService(svcType);
        }

        #endregion

        #region IBounds2D interface

        /// <summary>
        /// Gets location of the view in the parent control specified in device coordinates.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Point Location
        {
            get
            {
                return new Point((int)Math.Ceiling(this.Model.Bounds.Left), (int)Math.Ceiling(this.Model.Bounds.Top));
            }
        }

        /// <summary>
        /// Gets or sets size of the view in device coordinates.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Size Size
        {
            get
            {
                return new Size(this.Width, this.Height); // this.rcBounds.Size;
            }
            set
            {
                if (this.rcBounds.Size != value)
                {
                    SizeF oldsize = this.rcBounds.Size;
                    this.rcBounds.Size = value;

                    // this.CreateBackBuffer();
                    // this.OnSizeChanged(new ViewSizeEventArgs(oldsize, value));
                }
            }
        }

        /// <summary>
        /// Gets X coordinate of the location.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int X
        {
            get
            {
                return this.Location.X;
            }
        }

        /// <summary>
        /// Gets Y coordinate of the location.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int Y
        {
            get
            {
                return this.Location.Y;
            }
        }

        /// <summary>
        /// Gets width of the view in device coordinates.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int Width
        {
            get
            {
                if (this.Model != null)
                    return (int)Math.Ceiling(this.Model.LogicalSize.Width);
                else return 0;
            }
        }

        /// <summary>
        /// Gets height of the view in device coordinates.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int Height
        {
            get
            {
                if (this.Model != null)
                    return (int)Math.Ceiling(this.Model.LogicalSize.Height);
                else return 0;
            }
        }

        /// <summary>
        /// Gets bounds of the view in the parent control specified in device coordinates.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public System.Drawing.Rectangle Bounds
        {
            get
            {
                return this.rcBounds;
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        /// <summary>
        /// Called when deserialization is complete.
        /// </summary>
        /// <param name="sender">Object performing the deserialization.</param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this.OnDeserialization(sender);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("origin", m_ptOrigin);
            info.AddValue("magnification", m_fMagnification);
            info.AddValue("pageborderstyle", m_stylePageBorder);
            info.AddValue("m_grid", this.Grid);
            info.AddValue("backcolor", backgroundColor);
            info.AddValue("handleColor", this.handleColor);
            info.AddValue("handleOutlineColor", this.handleOutlineColor);
            info.AddValue("handleDisabledColor", this.handleDisabledColor);
            info.AddValue("printZoom", this.prtZoom);
            info.AddValue("pageSetting", this.pgSettings);
            info.AddValue("zoomIncrement", this.m_iZoomIncrement);
        }

        /// <summary>
        /// Called when deserialization is complete.
        /// </summary>
        /// <param name="sender">Object performing the deserialization.</param>
        protected virtual void OnDeserialization(object sender)
        { 
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Creates the layout grid that is rendered in the view.
        /// </summary>
        /// <returns>Layout grid to attach to grid.</returns>
        protected virtual LayoutGrid CreateGrid()
        {
            return new LayoutGrid(this);
        }

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
        /// </remarks>
        protected override object GetService(Type svcType)
        {
            if (svcType == typeof(EventSink))
            {
                return this.EventSink;
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

        #region Printing

        /// <summary>
        /// Prints a page to the specified output device.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        void IPrint.PrintPage(PrintPageEventArgs evtArgs)
        {
            if (evtArgs.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                if (this.nCurrentPage <= (evtArgs.PageSettings.PrinterSettings.FromPage - 1))
                {
                    this.nCurrentPage = evtArgs.PageSettings.PrinterSettings.FromPage - 1;
                }

                // this is made to prevent a blank page to be printed
                if ((this.nCurrentPage + 1) >= evtArgs.PageSettings.PrinterSettings.ToPage)
                {
                    this.PrintPage(evtArgs);
                    this.nCurrentPage = 0;
                    evtArgs.HasMorePages = false;
                }
                else
                {
                    this.PrintPage(evtArgs);
                }
            }
            else
            {
                this.PrintPage(evtArgs);
            }
        }

        /// <summary>
        /// Prints a page to the specified output device.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected virtual void PrintPage(PrintPageEventArgs evtArgs)
        {
            // quit printing if either of margin bounds dimemsions is less than zero
            if (evtArgs.MarginBounds.Width < 0 || evtArgs.MarginBounds.Height < 0)
            {
                evtArgs.HasMorePages = false;
                return;
            }

            Graphics grfx = evtArgs.Graphics;

            if (nCurrentPage == 0)
            {
                MeasureUnitsConverter.s_fPrevDpiX = MeasureUnitsConverter.DpiX;
                MeasureUnitsConverter.s_fPrevDpiY = MeasureUnitsConverter.DpiY;
            }

            float fFactorX = grfx.DpiX / MeasureUnitsConverter.DpiX;
            float fFactorY = grfx.DpiY / MeasureUnitsConverter.DpiY;

            float modelWidth = MeasureUnitsConverter.Convert(m_document.LogicalSize.Width, m_document.MeasurementUnits, MeasureUnits.Pixel);
            float modelHeight = MeasureUnitsConverter.Convert(m_document.LogicalSize.Height, m_document.MeasurementUnits, MeasureUnits.Pixel);
            SizeF szModel = m_document.LogicalSize;

            float fNotConvertedBoundsWidth = MeasureUnitsConverter.Convert(szModel.Width, m_document.MeasurementUnits, MeasureUnits.Inch) * 100;
            float fNotConvertedBoundsHeight = MeasureUnitsConverter.Convert(szModel.Height, m_document.MeasurementUnits, MeasureUnits.Inch) * 100;

            fNotConvertedBoundsWidth = (float)Math.Max(Math.Round(fNotConvertedBoundsWidth), evtArgs.MarginBounds.Width);
            fNotConvertedBoundsHeight = (float)Math.Max(Math.Round(fNotConvertedBoundsHeight), evtArgs.MarginBounds.Height);
            MeasureUnitsConverter.DpiX = grfx.DpiX;
            MeasureUnitsConverter.DpiY = grfx.DpiY;

            GraphicsState grfxState = grfx.Save();

            float pagewidth = evtArgs.MarginBounds.Width;
            float pageheight = evtArgs.MarginBounds.Height;

            RectangleF margin = new RectangleF(evtArgs.MarginBounds.Location, evtArgs.MarginBounds.Size);

            float headerHeight = this.m_document.HeaderFooterData.Header.GetHeight(grfx, pagewidth, pageheight, margin);
            float footerHeight = this.m_document.HeaderFooterData.Footer.GetHeight(grfx, pagewidth, pageheight, margin);
            pageheight -= (headerHeight + footerHeight);

            SizeF pagesize = new SizeF(pagewidth, pageheight);

            float fScaleRatio;
            RectangleF rcPage = new RectangleF();

            if (this.PrintZoom.UsePrintingZoom)
            {
                fScaleRatio = this.prtZoom.PrintingZoom / 100f;
            }
            else
            {
                fScaleRatio = Math.Min(
                    (pagesize.Width * this.prtZoom.SheetsAcross) / fNotConvertedBoundsWidth,
                    (pagesize.Height * this.prtZoom.SheetsDown) / fNotConvertedBoundsHeight);
            }

            rcPage.Width = pagesize.Width;
            rcPage.Height = pagesize.Height;
            float fColumns = fNotConvertedBoundsWidth / (rcPage.Width / fScaleRatio);
            float fRows = fNotConvertedBoundsHeight / (rcPage.Height / fScaleRatio);
            int nColumns = (fColumns % 1 == 0) ? (int)fColumns : (int)Math.Ceiling(fColumns);
            int nRows = (fRows % 1 == 0) ? (int)fRows : (int)Math.Ceiling(fRows);
            int nTotalPages = nColumns * nRows;

            int nCurrentRow = (int)Math.Floor((double)this.nCurrentPage / nColumns);
            int nCurrentColumn = this.nCurrentPage - (nCurrentRow * nColumns);
            float xOffset = nCurrentColumn * rcPage.Width;
            float yOffset = nCurrentRow * rcPage.Height;

            rcPage.X = xOffset;
            rcPage.Y = yOffset;
            fScaleRatio = (fScaleRatio / MeasureUnitsConverter.DpiX) * 100f;

            this.Model.HeaderFooterData.Header.CurrentPage = this.Model.HeaderFooterData.Footer.CurrentPage = this.nCurrentPage + 1;
            this.Model.HeaderFooterData.Header.TotalPages = this.Model.HeaderFooterData.Footer.TotalPages = nTotalPages;
            /*************Headers And Footers*******************/
            DrawHF(grfx, this.Model.HeaderFooterData.Header);
            DrawHF(grfx, this.Model.HeaderFooterData.Footer);
            /***************************************************/

            float left = evtArgs.PageSettings.Margins.Left;
            float top = evtArgs.PageSettings.Margins.Top;
            grfx.SetClip(new RectangleF(left, top + headerHeight, pagesize.Width, pagesize.Height));

            // Temporarily set the viewmatrix offsets to that of the current page being printed.
            Matrix mxtTemp = new Matrix();
            mxtTemp.Translate(-(rcPage.Width * nCurrentColumn) + left, (-(rcPage.Height * nCurrentRow) + top + headerHeight));
            float fScaleValX = 0f;
            float fScaleValY = 0f;
            float fPrintableAreaWidth = 0f;
            float fPrintableAreaHeight = 0f;
            if (this.PageSettings.Landscape)
            {
                fPrintableAreaWidth = this.PageSettings.PrintableArea.Height;
                fPrintableAreaHeight = this.PageSettings.PrintableArea.Width;
            }
            else
            {
                fPrintableAreaWidth = this.PageSettings.PrintableArea.Width;
                fPrintableAreaHeight = this.PageSettings.PrintableArea.Height;
            }
            if ((fScaleRatio * fFactorX) > 1 && nTotalPages == 1)
            {
                if (this.PageSettings.PrintableArea.Width != modelWidth)
                    fScaleValX = 1 + (1 - (modelWidth / fPrintableAreaWidth));
                else
                    fScaleValX = 1;
            }
            else
                fScaleValX = fScaleRatio * fFactorX;

            if ((fScaleRatio * fFactorY) > 1 && nTotalPages == 1)
            {
                if (this.PageSettings.PrintableArea.Height != modelHeight)
                    fScaleValY = 1 + (1 - (modelHeight / fPrintableAreaHeight));
                else
                    fScaleValY = 1;
            }
            else
                fScaleValY = fScaleRatio * fFactorY;

            mxtTemp.Scale(fScaleValX, fScaleValY);
            //mxtTemp.Scale(fScaleRatio * fFactorX, fScaleRatio * fFactorY);

            // Apply the ViewMatrix transform on the Graphics object and draw the page background.
            grfx.Transform = mxtTemp;

            DrawDocumentBackground(
                grfx, 
                new RectangleF(PointF.Empty, MeasureUnitsConverter.Convert(this.Model.Bounds.Size, this.Model.MeasurementUnits, MeasureUnits.Pixel)));

            Node nodeTemp;
            int nCounter = 0;

            MeasureUnitsConverter.DpiX = MeasureUnitsConverter.s_fPrevDpiX;
            MeasureUnitsConverter.DpiY = MeasureUnitsConverter.s_fPrevDpiY;

            while (nCounter < this.Model.ChildCount)
            {
                nodeTemp = this.Model.GetChild(nCounter);

                if (nodeTemp != null)
                {
                    bool dPrintports = nodeTemp.DrawPorts;
                    nodeTemp.DrawPorts = nodeTemp.PrintPorts;
                    nodeTemp.Draw(grfx);
                    nodeTemp.DrawPorts = dPrintports;
                }

                nCounter++;
            }

            grfx.Restore(grfxState);

            /********************Border************************/
            if (this.PageBorderStyle.ShowBorder)
            {
                int width = evtArgs.PageSettings.PaperSize.Width - (evtArgs.PageSettings.Margins.Right + evtArgs.PageSettings.Margins.Left);
                int height = evtArgs.PageSettings.PaperSize.Height - (evtArgs.PageSettings.Margins.Bottom + evtArgs.PageSettings.Margins.Top);
                RectangleF rectBorder = new RectangleF(new PointF(left, top), new SizeF(width, height));

                // consider orientation
                if (evtArgs.PageSettings.Landscape)
                {
                    float fTmp = rectBorder.Width;
                    rectBorder.Width = rectBorder.Height;
                    rectBorder.Height = fTmp;
                }

                using (Pen pen = this.PageBorderStyle.CreatePen())
                    DrawBoundingRectangle(grfx, pen, rectBorder, (int)this.PageBorderStyle.BorderCornerRounding);
            }
            /**************************************************/

            if (++this.nCurrentPage < nTotalPages)
            {
                evtArgs.HasMorePages = true;
            }
            else
            {
                evtArgs.HasMorePages = false;
                this.nCurrentPage = 0;
            }
        }

        /// <summary>
        /// Appends margins to printed page.
        /// </summary>
        /// <param name="evtArgs">QueryPageSettings arguments.</param>
        void IPrint.QueryPageSettings(QueryPageSettingsEventArgs evtArgs)
        {
            this.QueryPageSettings(evtArgs);
        }

        /// <summary>
        /// Appends margins to printed page.
        /// </summary>
        /// <param name="evtArgs">QueryPageSettings arguments.</param>
        protected virtual void QueryPageSettings(QueryPageSettingsEventArgs evtArgs)
        {
            evtArgs.PageSettings.Margins = this.PageSettings.Margins;
        }

        [DocumentationExclude()]
        private void DrawHF(Graphics grfx, HeaderFooterBase hf)
        {
            if (hf.Visible)
            {
                StringFormat drawFormat = new StringFormat();
                float height = hf.Bounds.RealHeight;
                float width = hf.Bounds.RealWidth;
                float x = hf.Bounds.XLocation;
                float y = hf.Bounds.YLocation;

                // if ( ( height > 0 ) && ( width > 0 ) )
                // {
                if (hf.AutoBounds)
                {
                    drawFormat.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
                }
                else
                {
                    drawFormat.FormatFlags = StringFormatFlags.LineLimit;
                }
                SolidBrush brush = new SolidBrush(hf.ForeColor);
                Font font = hf.Font;

                string headerRight = hf.ComposedRight;
                string headerLeft = hf.ComposedLeft;
                string headerCenter = hf.ComposedCenter;

                Image img = hf.Image;
                if (img != null)
                {
                    DrawHFImage(img, grfx, hf);
                }

                // Checking for empty string after trim operation
                if (headerCenter == null)
                {
                    if (headerLeft == null)
                    {
                        if (headerRight != null)
                        {
                            // if there is only right header draw it using full page width
                            DrawHFRight(grfx, headerRight, font, brush, x, y, width, width, height, drawFormat);
                        }
                    }
                    else if (headerRight != null)
                    {
                        // if there is no center header but left and right make left and right headers width halh page width
                        DrawHFRight(grfx, headerRight, font, brush, x, y, width, width / 2, height, drawFormat);
                        DrawHFLeft(grfx, headerLeft, font, brush, x, y, width / 2, height, drawFormat);
                    }
                    else
                    {
                        DrawHFLeft(grfx, headerLeft, font, brush, x, y, width, height, drawFormat);
                    }
                }
                else
                {
                    // center header is present
                    // if there are no left and right headers draw center header using full page width
                    if ((headerLeft == null) && (headerRight == null))
                    {
                        DrawHFCenter(grfx, headerCenter, font, brush, x, y, width, width, height, drawFormat);
                    }
                    else
                    {
                        // if there is left or right header or both set each header width equal
                        DrawHFLeft(grfx, headerLeft, font, brush, x, y, width / 3, height, drawFormat);
                        DrawHFRight(grfx, headerRight, font, brush, x, y, width, width / 3, height, drawFormat);
                        DrawHFCenter(grfx, headerCenter, font, brush, x, y, width, width / 3, height, drawFormat);
                    }
                }

                // Draw bounding rectangle
                Pen pen = hf.BorderStyle.CreatePen();
                if (pen != null)
                {
                    if (hf is Header)
                    {
                        grfx.DrawRectangle(pen, x + (pen.Width / 2), y - (pen.Width / 2), width, height);
                    }
                    else
                    {
                        grfx.DrawRectangle(pen, x + (pen.Width / 2), y + (pen.Width / 2), width, height);
                    }
                }
            }
        }

        [DocumentationExclude()]
        private void DrawHFImage(Image img, Graphics g, HeaderFooterBase hf)
        {
            GraphicsState state = g.Save();
            RectangleF rect;
            float left;
            float right;
            float top;
            ImageAttributes attr = new ImageAttributes();
            RectangleF bounds = new RectangleF(hf.Bounds.XLocation, hf.Bounds.YLocation, hf.Bounds.RealWidth, hf.Bounds.RealHeight);
            try
            {
                g.SetClip(bounds);
                switch (hf.ImageLayout)
                {
                    case ImageLayout.Center:
                        rect = new RectangleF(bounds.X + (bounds.Width / 2 - img.Width / 2), bounds.Y + bounds.Height / 2 - img.Height / 2, img.Width, img.Height);
                        g.DrawImage(img, rect);
                        break;
                    case ImageLayout.Tile:
                        attr.SetWrapMode(WrapMode.Tile);

                        // Location of three edges of rectangle for drawing.
                        PointF p1 = new PointF(bounds.X, bounds.Y);
                        PointF p2 = new PointF(bounds.Right, bounds.Y);
                        PointF p3 = new PointF(p1.X, bounds.Bottom);
                        PointF[] destPoints = { p1, p2, p3 };
                        RectangleF srcRect = new RectangleF(0, 0, bounds.Width, bounds.Height);
                        g.DrawImage(img, destPoints, srcRect, GraphicsUnit.Pixel, attr);
                        break;
                    case ImageLayout.Stretch:
                        left = hf.BorderStyle.ShowBorder ? bounds.X + hf.BorderStyle.Width : bounds.X;
                        top = hf.BorderStyle.ShowBorder ? bounds.Y + hf.BorderStyle.Width : bounds.Y;
                        rect = new RectangleF(left, top, bounds.Width, bounds.Height);
                        g.DrawImage(img, rect);
                        break;
                    case ImageLayout.AlignLeft:
                        left = hf.BorderStyle.ShowBorder ? bounds.X + hf.BorderStyle.Width : bounds.X;
                        rect = new RectangleF(left, bounds.Y + bounds.Height / 2 - img.Height / 2, img.Width, img.Height);
                        g.DrawImage(img, rect);
                        break;
                    case ImageLayout.AlignRight:
                        right = hf.BorderStyle.ShowBorder ? bounds.X + (bounds.Width - img.Width) - hf.BorderStyle.Width : bounds.X + (bounds.Width - img.Width);
                        rect = new RectangleF(right, bounds.Y + bounds.Height / 2 - img.Height / 2, img.Width, img.Height);
                        g.DrawImage(img, rect);
                        break;
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        [DocumentationExclude()]
        private void DrawHFRight(Graphics grfx, string str, Font font, SolidBrush brush, float x, float y, float pageWidth, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            SizeF size = grfx.MeasureString(str, font, (int)headerWidth);
            RectangleF rect = new RectangleF((x + (pageWidth - size.Width)), y, size.Width, highestHeader);
            grfx.DrawString(str, font, brush, rect, drawFormat);
        }

        [DocumentationExclude()]
        private void DrawHFLeft(Graphics grfx, string str, Font font, SolidBrush brush, float x, float y, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            RectangleF rect = new RectangleF(x, y, headerWidth, highestHeader);
            grfx.DrawString(str, font, brush, rect, drawFormat);
        }

        [DocumentationExclude()]
        private void DrawHFCenter(Graphics grfx, string str, Font font, SolidBrush brush, float x, float y, float pageWidth, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            SizeF size = grfx.MeasureString(str, font, (int)headerWidth);
            RectangleF rect = new RectangleF((x + (pageWidth / 2 - size.Width / 2)), y, size.Width, highestHeader);
            grfx.DrawString(str, font, brush, rect, drawFormat);
        }

        #endregion	// Printing

        #region Class helper methods
        /// <summary>
        /// Called when the origin of the view changes.
        /// </summary>
        /// <param name="evtArgs">A <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> that contains the event data.</param>
        protected virtual void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            if (this.EventSink != null)
            {
                this.EventSink.RaiseOriginChanged(evtArgs);
            }
        }

        /// <summary>
        /// Called when the origin of the view changes.
        /// </summary>
        /// <param name="evtArgs">A <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> that contains the event data.</param>
        protected virtual void OnMagnificationChanged(ViewMagnificationEventArgs evtArgs)
        {
            if (this.EventSink != null)
            {
                this.EventSink.RaiseMagnificationChanged(evtArgs);
            }
        }

        /// <summary>
        /// Draws the handles.
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
                    if (nodeCur.Visible)
                    {
                        HandleEditMode resultHandleEditMode = nodeCur.EditStyle.DefaultHandleEditMode;

                        // get cur node parent's transformations 
                        Matrix matrixParentsTransorm = GetParentsTransform(nodeCur);

                        // save graphics state
                        GraphicsState stateSave = grfx.Save();

                        // append parents transforms on given graphics
                        grfx.MultiplyTransform(matrixParentsTransorm);
                        if (this.CustomHandleRenderer != null)
                        {
                            this.CustomHandleRenderer.Render(grfx, resultHandleEditMode, nodeCur);
                        }
                        else
                        {
                            this.HandleRenderer.Render(grfx, resultHandleEditMode, nodeCur);                            
                        }

                        // restore graphics state
                        grfx.Restore(stateSave);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parents transform.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The matrix</returns>
        private Matrix GetParentsTransform(Node node)
        {
            // return unitary matrix
            if (node is PseudoGroup) return new Matrix();

            if (node == null || node.Parent == null)
                throw new ArgumentNullException("node + parent");

            Matrix matrixToReturn = new Matrix();
            Matrix matrixTemp;
            ICompositeNode nodeParent = node.Parent;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model))
            {
                matrixTemp = ((Node)nodeParent).GetTransformations();
                ((Node)nodeParent).AppendFlipTransforms(matrixTemp);
                matrixToReturn.Multiply(matrixTemp, MatrixOrder.Append);

                nodeParent = ((Node)nodeParent).Parent;
            }

            return matrixToReturn;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">Type handle</param>
        /// <returns>The object.</returns>
        public object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            return GetService(Type.GetTypeFromHandle(typeHandle));
        }
        #endregion
    }

    /// <summary>
    /// Specifies zooming action type.
    /// </summary>
    public enum ZoomType
    {
        /// <summary>
        /// Center based zooming
        /// </summary>
        Center = 0,

        /// <summary>
        /// TopLeft based zooming
        /// </summary>
        TopLeft
    }
}
