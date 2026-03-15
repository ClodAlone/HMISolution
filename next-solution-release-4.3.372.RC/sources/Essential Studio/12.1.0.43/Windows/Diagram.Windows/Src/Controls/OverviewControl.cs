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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;
using System.Drawing.Imaging;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// <para>
    /// The OverviewControl class implements an overview window that provides a perspective view of the diagram model. 
    /// The OverviewControl is initialized with a diagram model and a view, and upon display renders a view 
    /// of the diagram that is scaled to the control's bounds.
    /// </para>
    /// <para>
    /// The control has a Viewport window that is positioned over the diagram display and can be moved and / or resized 
    /// using the mouse to dynamically change the view transform that maps the diagram's world coordinates onto the 
    /// view coordinates. The viewport's origin corresponds to the origin of the diagram's view and moving the viewport 
    /// will shift the view's origin by an equivalent extent. The viewport's size is proportional to the view's 
    /// magnification and resizing the viewport allows the view to be zoomed in and out.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(OverviewControl), "ToolboxIcons.OverviewControl.bmp")]
    [Description("Overview control that provides a perspective view of the diagram model.")]
    public class OverviewControl
        : Panel
    {
        #region Class constants
        [Documentation.DocumentationExclude()]
        protected const int nMinWidth = 50;
        [Documentation.DocumentationExclude()]
        protected const int nMinHeight = 50;

        /// <summary>
        /// Name of the field used to store DrawPorts property value.
        /// </summary>
        private const string c_strDRAW_PORTS = @"m_bDrawPorts";
        #endregion

        #region Class members
        [Documentation.DocumentationExclude()]
        protected Diagram m_diagram;
        [Documentation.DocumentationExclude()]
        protected Model dgmModel;
        [Documentation.DocumentationExclude()]
        protected View dgmView;
        [Documentation.DocumentationExclude()]
        protected RectangleF rcClientArea;
        [Documentation.DocumentationExclude()]
        protected RectangleF rcDisplayArea;
        [Documentation.DocumentationExclude()]
        protected ViewportRenderer vpRenderer;

        /// <summary>
        /// Bitmap Buffer drawing.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected bool bRedrawBuffer = true;
        [Documentation.DocumentationExclude()]
        protected Bitmap bmpBuffer;

        /// <summary>
        /// Viewport dragging.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected Point ptDragStart;
        [Documentation.DocumentationExclude()]
        protected RectMarkers dragMarker;
        [Documentation.DocumentationExclude()]
        protected RectangleF rcInDragArea;

        /// <summary>
        /// Model to Viewport Scaling factor.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected float scaleFactor = 1.0f;
        [Documentation.DocumentationExclude()]
        protected bool bZoomSlider = false;        
        [Documentation.DocumentationExclude()]
        protected bool bViewportInitiated = false;
        private bool m_detached = true;
        private RectangleF m_newBounds;
        private RectangleF m_oldBounds;

        #endregion

        #region Delegate declaration.

        private ViewPortBoundsChangingEventHandler changingHandler;
        private ViewPortBoundsChangedEventHandler changedHandler;

        /// <summary>
        /// Occurs when view port bounds is changing.
        /// </summary>
        [Description("Occurs when the control's viewport bounds changing")]
        public event ViewPortBoundsChangingEventHandler ViewPortBoundsChanging
        {
            add
            {
                changingHandler += value;
            }
            remove
            {
                changingHandler -= value;
            }
        }

        /// <summary>
        /// Occurs when view port bounds is changed.
        /// </summary>
        [Description("Occurs when the control's viewport bounds changed")]
        public event ViewPortBoundsChangedEventHandler ViewPortBoundsChanged
        {
            add
            {
                changedHandler += value;
            }
            remove
            {
                changedHandler -= value;
            }
        }

        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the diagram control.
        /// </summary>
        /// <value>The diagram.</value>  
        public Diagram Diagram
        {
            get
            {
                return m_diagram;
            }
            set
            {
                if (m_diagram != value)
                {                    
                    if (value != null)
                    {
                        m_diagram = value;
                        this.Model = m_diagram.Model;
                        this.View = m_diagram.View;
                        SubscribeDiagramEvents(true);
                        m_detached = m_diagram.Model != null ? false : true;
                    }
                    else
                    {
                        this.Model = null;
                        this.View = null;
                        SubscribeDiagramEvents(false);
                        m_detached = true;
                        m_diagram = value;
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Forms.Diagram.Model"/> that the <see cref="OverviewControl"/> is initialized with.
        /// </summary>
        /// <value>The model.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected Model Model
        {
            get
            {
                return dgmModel;
            }
            set
            {
                if (this.dgmModel != value)
                {
                    if (this.dgmModel != null)
                        this.SubscribeModelEvents(false);
                    this.dgmModel = value;
                    if (this.dgmModel != null)
                    {                        
                        this.SubscribeModelEvents(true);
                        this.RecalculateOverviewBounds();
                        if (this.dgmView != null)
                        {
                            this.ApplyViewOriginToViewport();
                            this.ApplyViewZoomToViewport();
                        }
                        this.bRedrawBuffer = true;                        
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Forms.Diagram.View"/> that the <see cref="OverviewControl"/> is tied to.
        /// </summary>
        /// <value>The view.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected View View
        {
            get 
            { 
                return this.dgmView; 
            }
            set
            {
                if (this.dgmView != value)
                {
                    if (this.dgmView != null)
                        this.SubscribeViewEvents(false);
                    this.dgmView = value;
                    if (this.dgmView != null)
                    {                        
                        this.SubscribeViewEvents(true);
                        this.ApplyViewOriginToViewport();
                        this.ApplyViewZoomToViewport();                        
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the bounds of the OverviewControl's Viewport window.
        /// </summary>
        /// <value>The viewport bounds.</value>
        /// <remarks>
        /// The Viewport window location and size relative to the <see cref="OverviewControl"/>'s diagram display
        /// determine the <see cref="Forms.Diagram.View.Origin"/> and <see cref="Forms.Diagram.View.Magnification"/> property values
        /// for the diagram's <see cref="Forms.Diagram.View"/>.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RectangleF ViewportBounds
        {
            get
            {
                return this.vpRenderer.ViewportRect;
            }
            set
            {
                m_oldBounds = this.vpRenderer.ViewportRect;
                m_newBounds = value;

                if (this.vpRenderer.ViewportRect != value && OnViewportBoundsChanging(ref m_newBounds, m_oldBounds))
                {
                    this.vpRenderer.ViewportRect = m_newBounds;
                    this.dgmModel.BeginUpdate();
                    if (bViewportInitiated)
                    {
                        this.ApplyViewportZoomToView();
                        this.ApplyViewportOriginToView();
                    }

                    this.dgmModel.EndUpdate();

                    if (changedHandler != null)
                    {
                        ViewPortBoundsChangedEventArgs args = new ViewPortBoundsChangedEventArgs(this.vpRenderer.ViewportRect);
                        changedHandler(args);
                    }

                    bRedrawBuffer = true;
                }

                // this.Diagram.Invalidate(true);
            }
        }

        #endregion

        #region Public constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewControl"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public OverviewControl(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewControl"/> class.
        /// </summary>
        public OverviewControl()
        {
#if !NO_LICENSE
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(OverviewControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif

#if SyncfusionFramework2_0
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
#else
            this.SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true);
#endif
            this.ForeColor = Color.Red;
            this.BackColor = SystemColors.AppWorkspace;
            this.vpRenderer = new ViewportRenderer(this);
        }

        #endregion

        #region Events subscribing

        /// <summary>
        /// Subscribes the diagram events.
        /// </summary>
        /// <param name="subscribe">if set to <c>true</c> subscribe events.</param>
        [Documentation.DocumentationExclude()]
        public void SubscribeDiagramEvents(bool subscribe)
        {
            if (this.m_diagram != null)
            {
                if (subscribe)
                {
                    m_diagram.SizeChanged += new EventHandler(Diagram_SizeChanged);
                    m_diagram.EventSink.PropertyChanged += new PropertyChangedEventHandler(EventSink_ModelPropertyChanged);
                }
                else
                {
                    m_diagram.SizeChanged -= new EventHandler(Diagram_SizeChanged);
                    m_diagram.EventSink.PropertyChanged -= new PropertyChangedEventHandler(EventSink_ModelPropertyChanged);
                }
            }
        }

        /// <summary>
        /// Subscribes the model events.
        /// </summary>
        /// <param name="subscribe">if set to <c>true</c> subscribe events.</param>
        protected void SubscribeModelEvents(bool subscribe)
        {
            if (this.dgmModel != null)
            {
                if (subscribe)
                {
                    dgmModel.EventSink.NodeCollectionChanged += new CollectionExEventHandler(OnModelChildrenChangeComplete);
                    dgmModel.EventSink.PropertyChanged += new PropertyChangedEventHandler(OnModelPropertyChanged);
                    dgmModel.EventSink.FlipChanged += new FlipChangedEventHandler(EventSink_FlipChanged);
                    dgmModel.EventSink.RotationChanged += new RotationChangedEventHandler(EventSink_RotationChanged);
                    dgmModel.EventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
                    dgmModel.EventSink.PinPointChanged += new PinPointChangedEventHandler(EventSink_PinPointChanged);
                    dgmModel.EventSink.SizeChanged += new SizeChangedEventHandler(EventSink_SizeChanged);
                    dgmModel.EventSink.VertexChanged += new VertexChangedEventHandler(EventSink_VertexChanged);
                }
                else
                {
                    dgmModel.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(OnModelChildrenChangeComplete);
                    dgmModel.EventSink.PropertyChanged -= new PropertyChangedEventHandler(OnModelPropertyChanged);
                    dgmModel.EventSink.FlipChanged -= new FlipChangedEventHandler(EventSink_FlipChanged);
                    dgmModel.EventSink.RotationChanged -= new RotationChangedEventHandler(EventSink_RotationChanged);
                    dgmModel.EventSink.PinOffsetChanged -= new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
                    dgmModel.EventSink.PinPointChanged -= new PinPointChangedEventHandler(EventSink_PinPointChanged);
                    dgmModel.EventSink.SizeChanged -= new SizeChangedEventHandler(EventSink_SizeChanged);
                    dgmModel.EventSink.VertexChanged -= new VertexChangedEventHandler(EventSink_VertexChanged);
                }
            }
        }

        /// <summary>
        /// Subscribes the view events.
        /// </summary>
        /// <param name="subscribe">if set to <c>true</c> subscribe events.</param>
        protected void SubscribeViewEvents(bool subscribe)
        {
            if (this.dgmView != null)
            {
                if (subscribe)
                {
                    this.Diagram.EventSink.OriginChanged += new ViewOriginEventHandler(OnViewOriginChanged);
                    this.Diagram.EventSink.MagnificationChanged += new ViewMagnificationEventHandler(OnViewMagnificationChanged);
                }
                else
                {
                    this.Diagram.EventSink.OriginChanged -= new ViewOriginEventHandler(OnViewOriginChanged);
                    this.Diagram.EventSink.MagnificationChanged -= new ViewMagnificationEventHandler(OnViewMagnificationChanged);
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the <see cref="E:ModelChildrenChangeComplete"/> event.
        /// </summary>
        /// <param name="evtargs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        protected void OnModelChildrenChangeComplete(CollectionExEventArgs evtargs)
        {
            if (!m_detached)
            {
                bRedrawBuffer = true;
                Invalidate(true);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ModelPropertyChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        protected void OnModelPropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (!m_detached)
            {
                if (evtArgs.PropertyName == DPN.DocumentScale 
                    || evtArgs.PropertyName == DPN.SizeToContent
                    || evtArgs.PropertyName == "NodeScale.Width"
                    || evtArgs.PropertyName == "NodeScale.Height")
                {                    
                    RecalculateOverviewBounds();
                    ApplyViewOriginToViewport();
                    ApplyViewZoomToViewport();                    
                }
                bRedrawBuffer = true;
                Invalidate(true);
            }
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_FlipChanged(FlipChangedEventArgs evtArgs)
        {
            bRedrawBuffer = true;
            Invalidate(true);
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_RotationChanged(RotationChangedEventArgs evtArgs)
        {
            bRedrawBuffer = true;
            Invalidate(true);
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_PinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            bRedrawBuffer = true;
            Invalidate(true);
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_PinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            bRedrawBuffer = true;
            Invalidate(true);
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_SizeChanged(SizeChangedEventArgs evtArgs)
        {            
            RecalculateOverviewBounds();
            ApplyViewOriginToViewport();
            ApplyViewZoomToViewport();            
            bRedrawBuffer = true;
            Invalidate(true);
        }

        private void EventSink_VertexChanged(VertexChangedEventArgs evtArgs)
        {
            bRedrawBuffer = true;
            Invalidate(true);
        }

        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        private void EventSink_ModelPropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.Model)
            {
                this.Model = this.m_diagram.Model;
                bRedrawBuffer = true;
                Invalidate(true);
            }
            if (evtArgs.PropertyName == DPN.View)
            {
                this.View = this.m_diagram.View;               
            }
        }

        /// <summary>
        /// Called when model is disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="evtargs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        protected void OnModelDisposed(object sender, EventArgs evtargs)
        {
            this.Model = null;
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:ViewOriginChanged"/> event.
        /// </summary>
        /// <param name="evtargs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        protected void OnViewOriginChanged(ViewOriginEventArgs evtargs)
        {
            if (!m_detached)
            {
                this.bRedrawBuffer = false;                
                ApplyViewOriginToViewport();                
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ViewMagnificationChanged"/> event.
        /// </summary>
        /// <param name="evtargs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewMagnificationEventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        [EventHandlerPriority(true)]
        protected void OnViewMagnificationChanged(ViewMagnificationEventArgs evtargs)
        {
            if (!m_detached)
            {
                this.bRedrawBuffer = false;                
                ApplyViewZoomToViewport();                
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the diagram control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [Documentation.DocumentationExclude()]
        private void Diagram_SizeChanged(object sender, EventArgs e)
        {
            if (!m_detached)
            {                
                RecalculateOverviewBounds();
                ApplyViewOriginToViewport();
                ApplyViewZoomToViewport();                
                this.Invalidate();
            }
        }

        /// <summary>
        /// Call the Viewport bounds changing event.
        /// </summary>
        /// <param name="newRect">New ViewPort bounds</param>
        /// <param name="oldRect">Old ViewPort bounds</param>
        /// <returns>true, if bounds changed otherwise false</returns>
        private bool OnViewportBoundsChanging(ref RectangleF newRect, RectangleF oldRect)
        {
            bool cancel = false;

            if (changingHandler != null)
            {
                newRect = CalculateViewZoomToViewport(newRect);
                ViewPortBoundsChangingEventArgs args = new ViewPortBoundsChangingEventArgs(newRect, oldRect);
                changingHandler(args);
                cancel = args.Cancel;
            }
            else if (changedHandler != null)
            {
                newRect = CalculateViewZoomToViewport(newRect);
            }
            return !cancel;
        }

        #endregion

        #region Document drawing

        /// <summary>
        /// Draws to bitmap buffer.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        [Documentation.DocumentationExclude()]
        protected void DrawToBitmapBuffer(Graphics gfx)
        {
            if (this.bmpBuffer != null)
            {
                this.bmpBuffer.Dispose();
                this.bmpBuffer = null;
            }

            if ((this.rcDisplayArea.Width != 0) && (this.rcDisplayArea.Height != 0))
                
                // this.bmpBuffer = new Bitmap((int)Math.Ceiling(this.rcDisplayArea.Width), (int)Math.Ceiling(this.rcDisplayArea.Height));
                this.bmpBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            Graphics bmpgrfx = Graphics.FromImage(this.bmpBuffer);

            GraphicsState state = bmpgrfx.Save();

            bmpgrfx.PageScale = scaleFactor;
            bmpgrfx.PageUnit = GraphicsUnit.Pixel;

            PointF ptModelLocation = GetDocumentOffset();

            Matrix mtxTransform = new Matrix(1, 0, 0, 1, ptModelLocation.X, ptModelLocation.Y);
            bmpgrfx.MultiplyTransform(mtxTransform, MatrixOrder.Append);

            // Draw document backgroud
            RectangleF rcDoc = MeasureUnitsConverter.ToPixels(this.Model.Bounds, this.Model.MeasurementUnits);
            using (Brush brushFill = this.Model.BackgroundStyle.CreateBrush(bmpgrfx, rcDoc))
            {
                bmpgrfx.FillRectangle(brushFill, Geometry.ConvertRectangle(rcDoc));
            }

            if (this.Model.BackgroundImage != null)
            {
                ImageLayout imageLayout = this.Model.BackgroundImageLayout;
                Image imgBackground = this.Model.BackgroundImage;

                RectangleF rect;
                float left, right, top;
                ImageAttributes attr = new ImageAttributes();
                //RectangleF bounds = new RectangleF(this.Location.X, this.Location.Y, this.Width, this.Height);
                bmpgrfx.FillRectangle(new SolidBrush(Color.White), rcDoc);
                switch (this.Model.BackgroundImageLayout)
                {
                    case ImageLayout.Center:
                        rect = new RectangleF(rcDoc.X + (rcDoc.Width / 2 - imgBackground.Width / 2), rcDoc.Y + rcDoc.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                        bmpgrfx.DrawImage(imgBackground, rect);
                        break;
                    case ImageLayout.Tile:
                        attr.SetWrapMode(WrapMode.Tile);

                        // Location of three edges of rectangle for drawingrfx.
                        PointF p1 = new PointF(rcDoc.X, rcDoc.Y);
                        PointF p2 = new PointF(rcDoc.Right, rcDoc.Y);
                        PointF p3 = new PointF(p1.X, rcDoc.Bottom);
                        PointF[] destPoints = { p1, p2, p3 };
                        RectangleF srcRect = new RectangleF(0, 0, rcDoc.Width, rcDoc.Height);
                        bmpgrfx.DrawImage(imgBackground, destPoints, srcRect, GraphicsUnit.Pixel, attr);
                        break;
                    case ImageLayout.Stretch:
                        bmpgrfx.InterpolationMode = InterpolationMode.Bicubic;
                        left = rcDoc.X;
                        top = rcDoc.Y;
                        rect = new RectangleF(left, top, rcDoc.Width, rcDoc.Height);
                        bmpgrfx.DrawImage(imgBackground, rect);
                        break;
                    case ImageLayout.AlignLeft:
                        left = rcDoc.X;
                        rect = new RectangleF(left, rcDoc.Y + rcDoc.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                        bmpgrfx.DrawImage(imgBackground, rect);
                        break;
                    case ImageLayout.AlignRight:
                        right = rcDoc.X + (rcDoc.Width - imgBackground.Width);
                        rect = new RectangleF(right, rcDoc.Y + rcDoc.Height / 2 - imgBackground.Height / 2, imgBackground.Width, imgBackground.Height);
                        bmpgrfx.DrawImage(imgBackground, rect);
                        break;
                }
            }

            // Append model's Renderering style
            this.Model.RenderingStyle.ApplySettings(bmpgrfx);

            // Render Model representation on given graphics
            NodeCollection nodesModel = dgmModel.Nodes;

            if (nodesModel.Count > 0)
            {
                // node to DrawPorts property association
                Hashtable hash = new Hashtable();

                foreach (Node node in nodesModel)
                {
                    // use reflection to update DrawPorts property
                    UpdateDrawPorts(node, hash);

                    node.Draw(bmpgrfx);

                    // restore previous value
                    RestoreDrawPortsValue(hash, node);
                }
            }

            // restore graphics state
            gfx.Restore(state);
            
            // !!this.bRedrawBuffer = false;
        }

        /// <summary>
        /// Draws the document.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected void DrawDocument(Graphics gfx)
        {
            if (this.bRedrawBuffer)
            {
                DrawToBitmapBuffer(gfx);
            }

            if (null != bmpBuffer)
            {
                gfx.DrawImage(bmpBuffer, new PointF(0, 0));
            }
        }

        /// <summary>
        /// Draws the <see cref="Syncfusion.Windows.Forms.Diagram.Controls.OverviewControl"/>'s viewport rectangle.
        /// </summary>
        /// <param name="grfx">A <see cref="System.Drawing.Graphics"/> object</param>
        protected virtual void DrawViewport(Graphics grfx)
        {
            if ((this.ViewportBounds == RectangleF.Empty) && (rcDisplayArea != RectangleF.Empty))
            {
                // Calculate the new zoom factor using the ratio of the view size to the scaled viewport size.
                SizeF viewsize = MeasureUnitsConverter.ToPixels(this.Model.LogicalSize, this.Model.MeasurementUnits);

                RectangleF rcview = new RectangleF(this.View.Origin, viewsize);

                RectangleF rcscaledview = new RectangleF(
                    new PointF(rcview.Left * this.scaleFactor, rcview.Top * this.scaleFactor),
                    new SizeF(rcview.Width * this.scaleFactor, rcview.Height * this.scaleFactor));

                this.ViewportBounds = new RectangleF(
                    this.rcDisplayArea.Left + rcscaledview.Left, 
                    this.rcDisplayArea.Top + rcscaledview.Top,
                    rcscaledview.Width, 
                    rcscaledview.Height);
            }

            if (!(this.Diagram.Size.Width == 0 || this.Diagram.Size.Height == 0))
            {
                this.vpRenderer.DrawViewport(grfx, this.ForeColor);
            }
        }

        #endregion

        #region Apply changes to document

        /// <summary>
        /// Applies the viewport origin to view.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ApplyViewportOriginToView()
        {
            if (dgmView != null)
            {
                // Set the view origin based on the viewport's pan location.
                PointF origin = new PointF(
                    (this.ViewportBounds.X - this.rcDisplayArea.X) / this.scaleFactor,
                    (this.ViewportBounds.Y - this.rcDisplayArea.Y) / this.scaleFactor);

                this.View.Origin = new PointF(origin.X, origin.Y);
            }
        }

        /// <summary>
        /// Applies the viewport zoom to view.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ApplyViewportZoomToView()
        {
            if (dgmView != null)
            {
                // Calculate the new zoom factor using the ratio of the view size to the scaled viewport size.
                SizeF viewsize = m_diagram.ClientRectangle.Size;
                float zoomfactorX = viewsize.Width / (this.ViewportBounds.Width / scaleFactor);
                float zoomfactorY = viewsize.Height / (this.ViewportBounds.Height / scaleFactor);
                float zoomfactor = ((zoomfactorX > zoomfactorY) ? zoomfactorX : zoomfactorY) * 100f;
                ZoomType zoomType = this.View.ZoomType;
                this.View.ZoomType = ZoomType.TopLeft;
                this.View.Magnification = zoomfactor;
                this.View.ZoomType = zoomType;
            }
        }

        /// <summary>
        /// Applies the view origin to viewport.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ApplyViewOriginToViewport()
        {
            if (this.View != null)
            {
                // Set the viewport origin to correspond to the view's origin.
                PointF vieworigin = this.View.Origin;

                PointF zoomedorigin = new PointF(vieworigin.X, vieworigin.Y);

                RectangleF rcviewport = this.ViewportBounds;

                if(!bViewportInitiated)
                    rcviewport.Location = new PointF(
                                    this.rcDisplayArea.X + (zoomedorigin.X * scaleFactor),
                                    this.rcDisplayArea.Y + (zoomedorigin.Y * scaleFactor));

                this.vpRenderer.ViewportRect = rcviewport;
            }
        }

        /// <summary>
        /// Applies the view zoom to viewport.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ApplyViewZoomToViewport()
        {
            if ((this.dgmModel != null) && (this.dgmView != null))
            {
                // Set the viewport size to be relative to the view's zoom value.
                SizeF viewsize = // MeasureUnitsConverter.ToPixels( this.Model.Size, this.Model.MeasurementUnits );
                        new SizeF(this.m_diagram.ClientSize.Width, this.m_diagram.ClientSize.Height);
                SizeF viewzoom = new SizeF(this.View.Magnification / 100f, this.View.Magnification / 100f);

                SizeF viewportsize = new SizeF(
                    (viewsize.Width / viewzoom.Width) * scaleFactor,
                    (viewsize.Height / viewzoom.Height) * scaleFactor);

                this.ViewportBounds = new RectangleF(this.ViewportBounds.Location, viewportsize);
            }
        }

        #endregion

        #region Class overrides

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="meargs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <override/>
        protected override void OnMouseDown(MouseEventArgs meargs)
        {
            base.OnMouseDown(meargs);

            if ((dgmModel != null) && (dgmView != null))
            {
                // Store the mouse down position in ptDragStart.
                Point mousepos = new Point(meargs.X, meargs.Y);
                this.ptDragStart = PointToScreen(mousepos);

                RectangleF ltCorner = this.vpRenderer.rcLTcorner;
                RectangleF rtCorner = this.vpRenderer.rcRTcorner;
                RectangleF rbCorner = this.vpRenderer.rcRBcorner;
                RectangleF lbCorner = this.vpRenderer.rcLBcorner;
                RectangleF bmEdge = this.vpRenderer.rcBottomEdge;
                RectangleF ltEdge = this.vpRenderer.rcLeftEdge;
                RectangleF rtEdge = this.vpRenderer.rcRightEdge;
                RectangleF tpEdge = this.vpRenderer.rcTopEdge;

                if (HandlesHitTesting.TouchMode)
                {
                    ltCorner.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    rtCorner.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    rbCorner.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    lbCorner.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    bmEdge.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    ltEdge.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    rtEdge.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                    tpEdge.Inflate(vpRenderer.nViewportBorder, vpRenderer.nViewportBorder);
                }

                // Set the dragMarker to indicate the area of the viewport that was clicked on.
                if (this.ViewportBounds.Contains(mousepos))
                    this.dragMarker = RectMarkers.Viewport;
                else if (ltCorner.Contains(mousepos))
                    this.dragMarker = RectMarkers.LTCorner;
                else if (rtCorner.Contains(mousepos))
                    this.dragMarker = RectMarkers.RTCorner;
                else if (rbCorner.Contains(mousepos))
                    this.dragMarker = RectMarkers.RBCorner;
                else if (lbCorner.Contains(mousepos))
                    this.dragMarker = RectMarkers.LBCorner;
                else if (ltEdge.Contains(mousepos))
                    this.dragMarker = RectMarkers.LeftEdge;
                else if (tpEdge.Contains(mousepos))
                    this.dragMarker = RectMarkers.TopEdge;
                else if (rtEdge.Contains(mousepos))
                    this.dragMarker = RectMarkers.RightEdge;
                else if (bmEdge.Contains(mousepos))
                    this.dragMarker = RectMarkers.BottomEdge;
                else
                {
                    this.dragMarker = RectMarkers.None;
                    this.rcInDragArea = new RectangleF(mousepos.X, mousepos.Y, 0, 0);
                }

                // The mouse click occurred within the viewport bounds.
                if (this.dragMarker != RectMarkers.None) 
                {
                    this.rcInDragArea = this.ViewportBounds;
                    if (this.dragMarker != RectMarkers.Viewport)
                        this.vpRenderer.SizingRect = this.rcInDragArea;
                }

                this.bRedrawBuffer = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="meargs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <override/>
        protected override void OnMouseMove(MouseEventArgs meargs)
        {
            base.OnMouseMove(meargs);
            if(this.Diagram != null)
              if ((this.Diagram.Size.Width == 0 || this.Diagram.Size.Height == 0))
                return;

            if ((this.dgmModel != null) && (this.dgmView != null))
            {
                Point mousepos = new Point(meargs.X, meargs.Y);
                Point screenpos = this.PointToScreen(mousepos);

                if ((this.ptDragStart == Point.Empty) && (this.dragMarker == RectMarkers.None))
                {
                    if (this.ViewportBounds.Contains(mousepos))
                    {
                        if (this.Cursor != Cursors.SizeAll)
                            this.Cursor = Cursors.SizeAll;
                    }
                    else if (this.vpRenderer.rcLTcorner.Contains(mousepos) || this.vpRenderer.rcRBcorner.Contains(mousepos))
                    {
                        if (this.Cursor != Cursors.SizeNWSE)
                            this.Cursor = Cursors.SizeNWSE;
                    }
                    else if (this.vpRenderer.rcRTcorner.Contains(mousepos) || this.vpRenderer.rcLBcorner.Contains(mousepos))
                    {
                        if (this.Cursor != Cursors.SizeNESW)
                            this.Cursor = Cursors.SizeNESW;
                    }
                    else if (this.vpRenderer.rcLeftEdge.Contains(mousepos) || this.vpRenderer.rcRightEdge.Contains(mousepos))
                    {
                        if (this.Cursor != Cursors.SizeWE)
                            this.Cursor = Cursors.SizeWE;
                    }
                    else if (this.vpRenderer.rcTopEdge.Contains(mousepos) || this.vpRenderer.rcBottomEdge.Contains(mousepos))
                    {
                        if (this.Cursor != Cursors.SizeNS)
                            this.Cursor = Cursors.SizeNS;
                    }
                    else if (this.Cursor != Cursors.Default)
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else if (this.ptDragStart != Point.Empty) 
                {
                    // A drag operation is in progress.
                    RectangleF rccurrent = this.rcInDragArea;
                    PointF ptoffset = new PointF(screenpos.X - this.ptDragStart.X, screenpos.Y - this.ptDragStart.Y);
                    this.ptDragStart = screenpos;

                    float fOffsetX = rccurrent.Width / rccurrent.Height * ptoffset.Y;
                    float fOffsetY = rccurrent.Height / rccurrent.Width * ptoffset.X;

                    switch (this.dragMarker)
                    {
                        case RectMarkers.Viewport: // In a pan operation.
                            {
                                PointF newlocation = new PointF(rccurrent.Left + ptoffset.X, rccurrent.Top + ptoffset.Y);

                                float xmaxoffset = (this.rcClientArea.Width - this.rcDisplayArea.Width) / 2;
                                float ymaxoffset = (this.rcClientArea.Height - this.rcDisplayArea.Height) / 2;

                                if (Math.Round(this.rcInDragArea.Left) <= Math.Round(this.rcDisplayArea.Left - xmaxoffset))
                                {
                                    if (/*(mousepos.X < this.rcDisplayArea.Left) || */ newlocation.X < this.rcInDragArea.Left)
                                        newlocation.X = this.rcDisplayArea.Left - xmaxoffset;
                                }
                                if (Math.Round(this.rcInDragArea.Top) <= Math.Round(this.rcDisplayArea.Top - ymaxoffset))
                                {
                                    if (/*(mousepos.Y < this.rcDisplayArea.Top) || */ newlocation.Y < this.rcInDragArea.Top)
                                        newlocation.Y = this.rcDisplayArea.Top - ymaxoffset;
                                }
                                if (Math.Round(this.rcInDragArea.Right) >= Math.Round(this.rcDisplayArea.Right + xmaxoffset))
                                {
                                    if (/*(mousepos.X > this.rcDisplayArea.Right) || */ newlocation.X > this.rcInDragArea.Left)
                                        newlocation.X = this.rcDisplayArea.Right + xmaxoffset - this.ViewportBounds.Width;
                                }
                                if (Math.Round(this.rcInDragArea.Bottom) >= Math.Round(this.rcDisplayArea.Bottom + ymaxoffset))
                                {
                                    if (/*(mousepos.Y > this.rcDisplayArea.Bottom) || */ newlocation.Y > this.rcInDragArea.Top)
                                        newlocation.Y = this.rcDisplayArea.Bottom + ymaxoffset - this.ViewportBounds.Height;
                                }

                                this.rcInDragArea.Location = newlocation;
                                
                                // this.ViewportBounds = this.rcInDragArea;
                                this.vpRenderer.ViewportRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.LTCorner:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.X + ptoffset.X, rccurrent.Y + fOffsetY, rccurrent.Width - ptoffset.X, rccurrent.Height - fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.RTCorner:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left, rccurrent.Top - fOffsetY, rccurrent.Width + ptoffset.X, rccurrent.Height + fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.RBCorner:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left, rccurrent.Top, rccurrent.Width + ptoffset.X, rccurrent.Height + fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.LBCorner:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left + ptoffset.X, rccurrent.Top, rccurrent.Width - ptoffset.X, rccurrent.Height - fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.LeftEdge:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left + ptoffset.X, rccurrent.Top + fOffsetY / 2f, rccurrent.Width - ptoffset.X, rccurrent.Height - fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.TopEdge:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left + fOffsetX / 2f, rccurrent.Top + ptoffset.Y, rccurrent.Width - fOffsetX, rccurrent.Height - ptoffset.Y);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.RightEdge:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left, rccurrent.Top - fOffsetY / 2f, rccurrent.Width + ptoffset.X, rccurrent.Height + fOffsetY);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.BottomEdge:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left - fOffsetX / 2f, rccurrent.Top, rccurrent.Width + fOffsetX, rccurrent.Height + ptoffset.Y);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                        case RectMarkers.None:
                            {
                                this.rcInDragArea = new RectangleF(rccurrent.Left, rccurrent.Top, rccurrent.Width + ptoffset.X, rccurrent.Height + ptoffset.X);
                                this.vpRenderer.SizingRect = this.rcInDragArea;
                            }
                            break;
                    }
                }
            }

            // Diagram.HScrollBar.Value = (int)this.View.Origin.X ;
            // Diagram.VScrollBar.Value = (int)this.View.Origin.Y ;
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="meargs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <override/>
        protected override void OnMouseUp(MouseEventArgs meargs)
        {
            base.OnMouseUp(meargs);

            if ((this.dgmModel != null) && (this.dgmView != null))
            {
                if (this.ptDragStart != Point.Empty)
                {
                    bViewportInitiated = true;
                    if (this.dragMarker != RectMarkers.Viewport)
                    {
                        this.vpRenderer.SizingRect = RectangleF.Empty;
                        if ((this.rcInDragArea.Width > 5) && (this.rcInDragArea.Height > 5))
                            this.ViewportBounds = this.rcInDragArea;
                        else
                        {
                            RectangleF viewport = this.vpRenderer.ViewportRect;
                            this.ViewportBounds = new RectangleF(new PointF(this.rcInDragArea.X - (viewport.Width / 2), this.rcInDragArea.Y - (viewport.Height / 2)), viewport.Size);
                        }
                    }
                    else
                    {
                        ApplyViewportOriginToView();
                        Diagram.Invalidate();
                    }
                    bViewportInitiated = false;
                    ptDragStart = Point.Empty;
                    dragMarker = RectMarkers.None;
                    rcInDragArea = RectangleF.Empty;

                    // Diagram.HScrollBar.Value = (int)this.View.Origin.X;
                    // Diagram.VScrollBar.Value = (int)this.View.Origin.Y;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            if (!m_detached)
            {
                ApplyViewportOriginToView();
                Diagram.Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="peargs">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <override/>
        protected override void OnPaint(PaintEventArgs peargs)
        {
            if (dgmModel != null)
            {
                Graphics gfx = peargs.Graphics;

                DrawDocument(gfx);
                DrawViewport(gfx);
            }
            else
            {
                base.OnPaint(peargs);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.dgmModel != null)
            {
                this.SubscribeModelEvents(false);
                this.SubscribeViewEvents(false);
                this.SubscribeDiagramEvents(false);
                this.dgmModel = null;
                this.dgmView = null;

                if (this.bmpBuffer != null)
                    this.bmpBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new <see cref="P:System.Windows.Forms.Control.Left"/> property value of the control.</param>
        /// <param name="y">The new <see cref="P:System.Windows.Forms.Control.Top"/> property value of the control.</param>
        /// <param name="width">The new <see cref="P:System.Windows.Forms.Control.Width"/> property value of the control.</param>
        /// <param name="height">The new <see cref="P:System.Windows.Forms.Control.Height"/> property value of the control.</param>
        /// <param name="specified">A bitwise combination of the <see cref="T:System.Windows.Forms.BoundsSpecified"/> values.</param>
        /// <override/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            if ((width > nMinWidth) && (height > nMinHeight))
            {
                this.rcClientArea = new RectangleF(x, y, width, height);

                if ((this.dgmModel != null) && (this.dgmView != null))
                {                    
                    this.RecalculateOverviewBounds();
                    this.ApplyViewOriginToViewport();
                    this.ApplyViewZoomToViewport();
                    this.bRedrawBuffer = true;                    
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region Class utility methods

        /// <summary>
        /// Restores the draw ports value.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="node">The node.</param>
        private void RestoreDrawPortsValue(Hashtable hash, Node node)
        {
            bool bPrevValue;

            if (hash.ContainsKey(node))
            {
                bPrevValue = (bool)hash[node];

                if (bPrevValue)
                {
                    UpdateDrawPortsValue(node, bPrevValue);
                }
            }

            if (node is ICompositeNode)
            {
                RestoreDrawPortsValue(hash, node as ICompositeNode);
            }
        }

        /// <summary>
        /// Restores the draw ports value.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="nodeComposite">The node composite.</param>
        private void RestoreDrawPortsValue(Hashtable hash, ICompositeNode nodeComposite)
        {
            // update children
            Node nodeTemp;

            for (int i = 0, nLength = nodeComposite.ChildCount; nLength > i; i++)
            {
                nodeTemp = nodeComposite.GetChild(i);

                if (hash.ContainsKey(nodeTemp))
                {
                    bool bPrevValue = (bool)hash[nodeTemp];

                    if (bPrevValue)
                    {
                        UpdateDrawPortsValue(nodeTemp, bPrevValue);
                    }
                }

                if (nodeTemp is ICompositeNode)
                {
                    RestoreDrawPortsValue(hash, nodeTemp as ICompositeNode);
                }
            }
        }

        /// <summary>
        /// Updates the draw ports value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="bValue">The value.</param>
        private void UpdateDrawPortsValue(Node node, bool bValue)
        {
            // use reflection to update node's DrawProperty value
            // in order to skip raising events
            FieldInfo fi = typeof(Node).GetField(c_strDRAW_PORTS, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fi != null)
            {
                fi.SetValue(node, bValue);
            }
        }

        /// <summary>
        /// Updates the draw ports.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="hash">The hash with port flags where key - node, value - flag value.</param>
        private void UpdateDrawPorts(Node node, Hashtable hash)
        {
            ICompositeNode nodeComposite = node as ICompositeNode;

            if (nodeComposite != null)
            {
                UpdateDrawPorts(nodeComposite, hash);
            }
            else
            {
                bool bDrawPorts = UpdateDrawPorts(node);

                if (!hash.ContainsKey(node))
                    hash.Add(node, bDrawPorts);
            }
        }

        /// <summary>
        /// Updates the draw ports.
        /// </summary>
        /// <param name="nodeComposite">The node composite.</param>
        /// <param name="hash">The hashtable where key is composite node, value is flag value.</param>
        private void UpdateDrawPorts(ICompositeNode nodeComposite, Hashtable hash)
        {
            // update group node
            bool bDrawPorts = UpdateDrawPorts(nodeComposite as Node);

            if (!hash.ContainsKey(nodeComposite))
                hash.Add(nodeComposite, bDrawPorts);

            // update children
            Node nodeTemp;

            for (int i = 0, nLength = nodeComposite.ChildCount; nLength > i; i++)
            {
                nodeTemp = nodeComposite.GetChild(i);

                if (nodeTemp is ICompositeNode)
                {
                    UpdateDrawPorts(nodeTemp as ICompositeNode, hash);
                }
                else
                {
                    bDrawPorts = UpdateDrawPorts(nodeTemp);

                    if (!hash.ContainsKey(nodeTemp))
                        hash.Add(nodeTemp, bDrawPorts);
                }
            }
        }

        /// <summary>
        /// Updates the draw ports.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>true, if update is needed.</returns>
        private bool UpdateDrawPorts(Node node)
        {
            bool bValueToReturn = node.DrawPorts;

            if (node.DrawPorts)
            {
                // use reflection to update node's DrawProperty value
                // in order to skip raising events
                FieldInfo fi = typeof(Node).GetField(c_strDRAW_PORTS, BindingFlags.NonPublic | BindingFlags.Instance);

                if (fi != null)
                {
                    fi.SetValue(node, false);
                }
            }

            return bValueToReturn;
        }

        /// <summary>
        /// Recalculates the overview bounds.
        /// </summary>
        protected virtual void RecalculateOverviewBounds()
        {
            if (this.dgmModel == null)
                throw new InvalidOperationException("Control does not have a model.");

            SizeF modelsize = new SizeF(this.dgmModel.Bounds.Width, this.dgmModel.Bounds.Height);
            if (this.Model.MeasurementUnits != MeasureUnits.Pixel)
            {
                modelsize = MeasureUnitsConverter.ToPixels(modelsize, this.Model.MeasurementUnits);
            }

            float xscale = this.rcClientArea.Width / modelsize.Width;
            float yscale = this.rcClientArea.Height / modelsize.Height;
            this.scaleFactor = (xscale < yscale) ? xscale : yscale;
            float cxscale = (this.rcClientArea.Width / xscale) * this.scaleFactor;
            float cyscale = (this.rcClientArea.Height / yscale) * this.scaleFactor;
            this.rcDisplayArea = new RectangleF((this.rcClientArea.Width - cxscale) / 2, (this.rcClientArea.Height - cyscale) / 2, cxscale, cyscale);
        }

        /// <summary>
        /// Detaches the diagram.
        /// </summary>
        public void DetachDiagram()
        {
            m_detached = true;
            if (this.dgmModel != null)
            {
                this.SubscribeModelEvents(false);
                this.dgmModel = null;
            }
            if (this.dgmView != null)
            {
                this.SubscribeViewEvents(false);
                this.dgmView = null;
            }
            if (this.m_diagram != null)
            {
                this.SubscribeDiagramEvents(false);
                this.m_diagram = null;
            }
        }

        #endregion

        #region Class helper methods

        private PointF GetDocumentOffset()
        {
            PointF szOffsetToReturn = PointF.Empty;

            SizeF szModel = SizeF.Empty;
            SizeF szModelSize = MeasureUnitsConverter.ToPixels(this.Model.LogicalSize, this.Model.MeasurementUnits);
            szModel.Width = szModelSize.Width * scaleFactor;
            szModel.Height = szModelSize.Height * scaleFactor;

            szOffsetToReturn.X = (this.ClientSize.Width / 2 - szModel.Width / 2) / scaleFactor;
            szOffsetToReturn.Y = (this.ClientSize.Height / 2 - szModel.Height / 2) / scaleFactor;

            return szOffsetToReturn;
        }

        /// <summary>
        /// Calculate the view zoom to viewport rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>The viewport bounds</returns>
        private RectangleF CalculateViewZoomToViewport(RectangleF rect)
        {
            if (dgmView != null)
            {
                // Calculate the new zoom factor using the ratio 
                // of the view size to the scaled viewport size.
                SizeF viewsize = m_diagram.ClientRectangle.Size;
                float zoomfactorX = viewsize.Width / (rect.Width / scaleFactor);
                float zoomfactorY = viewsize.Height / (rect.Height / scaleFactor);
                float zoomfactor = ((zoomfactorX > zoomfactorY) ? zoomfactorX : zoomfactorY) * 100f;
                SizeF viewzoom = new SizeF(zoomfactor / 100f, zoomfactor / 100f);

                SizeF viewportsize = new SizeF(
                    (viewsize.Width / viewzoom.Width) * scaleFactor,
                    (viewsize.Height / viewzoom.Height) * scaleFactor);

                rect = new RectangleF(rect.Location, viewportsize);
            }
            return rect;
        }

        #endregion
    }

    /// <summary>
    /// Rectangle marker
    /// </summary>
    public enum RectMarkers
    {
        /// <summary>
        /// No marker.
        /// </summary>
        None = 0,

        /// <summary>
        /// Left corner.
        /// </summary>
        LTCorner = 1,

        /// <summary>
        /// Right corner.
        /// </summary>
        RTCorner = 2,

        /// <summary>
        /// Right bottom corner.
        /// </summary>
        RBCorner,

        /// <summary>
        /// Left bottom corner.
        /// </summary>
        LBCorner,

        /// <summary>
        /// Left edge.
        /// </summary>
        LeftEdge,

        /// <summary>
        /// Top edge.
        /// </summary>
        TopEdge,

        /// <summary>
        /// Right edge.
        /// </summary>
        RightEdge,

        /// <summary>
        /// Bottom edge.
        /// </summary>
        BottomEdge,

        /// <summary>
        /// View port.
        /// </summary>
        Viewport
    }

    /// <summary>
    /// Viewport renderer.
    /// </summary>
    [Documentation.DocumentationExclude()]
    public sealed class ViewportRenderer
    {
        public int nViewportBorder = 4;
        public RectangleF rcLTcorner = RectangleF.Empty;
        public RectangleF rcRTcorner = RectangleF.Empty;
        public RectangleF rcRBcorner = RectangleF.Empty;
        public RectangleF rcLBcorner = RectangleF.Empty;
        public RectangleF rcLeftEdge = RectangleF.Empty;
        public RectangleF rcTopEdge = RectangleF.Empty;
        public RectangleF rcRightEdge = RectangleF.Empty;
        public RectangleF rcBottomEdge = RectangleF.Empty;

        private RectangleF rcViewport = RectangleF.Empty;
        private RectangleF rcSizing = RectangleF.Empty;
        private Region rgnSizing = new Region();

        /// <summary>
        /// Overview control.
        /// </summary>
        private OverviewControl hostCtrl = null;

        /// <summary>
        /// Gets or sets the viewport rectangle.
        /// </summary>
        /// <value>The viewport rectangle.</value>
        public RectangleF ViewportRect
        {
            get
            {
                return this.rcViewport;
            }
            set
            {
                if (this.rcViewport != value)
                {
                    if (this.rcViewport != RectangleF.Empty)
                        this.InvalidateBorderRect();
                    this.rcViewport = value;
                    if (this.rcViewport != RectangleF.Empty)
                    {
                        this.CalculateBorderRect();
                        this.InvalidateBorderRect();
                    }
                    this.hostCtrl.Update();
                }
            }
        }

        /// <summary>
        /// Gets or sets the sizing rect.
        /// </summary>
        /// <value>The sizing rect.</value>
        public RectangleF SizingRect
        {
            get
            {
                return this.rcSizing;
            }
            set
            {
                if (this.rcSizing != value)
                {
                    if (this.rcSizing != RectangleF.Empty)
                        this.hostCtrl.Invalidate(this.rgnSizing);
                  
                     if (!float.IsNaN(value.Height) && !float.IsNaN(value.Width))
                     {
                    this.rcSizing = value;
                     }                   
                  
                    if (this.rcSizing != RectangleF.Empty)
                    {
                        this.CalculateSizingRegion();
                        this.hostCtrl.Invalidate(this.rgnSizing);
                    }
                    this.hostCtrl.Update();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewportRenderer"/> class.
        /// </summary>
        /// <param name="hostctrl">The host control.</param>
        public ViewportRenderer(OverviewControl hostctrl)
        {
            this.hostCtrl = hostctrl;
        }

        private void CalculateBorderRect()
        {
            if (this.rcViewport.Width == 0.0 || float.IsNaN(this.rcViewport.Width) || this.rcViewport.Height == 0.0 || float.IsNaN(this.rcViewport.Height))
                return;
            nViewportBorder = HandlesHitTesting.TouchMode ? 8 : 4;
            RectangleF rcview = RectangleF.Inflate(this.rcViewport, nViewportBorder, nViewportBorder);
            SizeF cornersize = new Size(nViewportBorder + 2, nViewportBorder + 2);

            this.rcLTcorner = new RectangleF(new PointF(rcview.Left - 1, rcview.Top - 1), cornersize);
            this.rcRTcorner = new RectangleF(new PointF(rcview.Right - nViewportBorder, rcview.Top - 1), cornersize);
            this.rcRBcorner = new RectangleF(new PointF(rcview.Right - nViewportBorder, rcview.Bottom - nViewportBorder), cornersize);
            this.rcLBcorner = new RectangleF(new PointF(rcview.Left - 1, rcview.Bottom - nViewportBorder), cornersize);

            this.rcTopEdge = new RectangleF(rcview.Left + nViewportBorder + 2, rcview.Top, this.rcViewport.Width - 3, nViewportBorder);
            this.rcRightEdge = new RectangleF(rcview.Right - nViewportBorder + 1, rcview.Top + nViewportBorder + 2, nViewportBorder, this.rcViewport.Height - 3);
            this.rcBottomEdge = new RectangleF(rcview.Left + nViewportBorder + 2, rcview.Bottom - nViewportBorder + 1, this.rcViewport.Width - 3, nViewportBorder);
            this.rcLeftEdge = new RectangleF(rcview.Left, rcview.Top + nViewportBorder + 2, nViewportBorder, this.rcViewport.Height - 3);
        }

        private void InvalidateBorderRect()
        {
            if (this.rcViewport.Width == 0.0 || float.IsNaN(this.rcViewport.Width) || this.rcViewport.Height == 0.0 || float.IsNaN(this.rcViewport.Height))
                return;

            GraphicsPath viewrectpath = new GraphicsPath();
            viewrectpath.AddRectangles(new RectangleF[] 
                                       { 
                                       this.rcLTcorner, 
                                       this.rcRTcorner, 
                                       this.rcRBcorner, 
                                       this.rcLBcorner, 
                                       this.rcTopEdge, 
                                       this.rcRightEdge, 
                                       this.rcBottomEdge, 
                                       this.rcLeftEdge 
                                       });

            Region viewrectregion = new Region(viewrectpath);
            this.hostCtrl.Invalidate(viewrectregion);
            viewrectregion.Dispose();
            viewrectpath.Dispose();
        }

        private void CalculateSizingRegion()
        {
            nViewportBorder = HandlesHitTesting.TouchMode ? 8 : 4;
            GraphicsPath sizingpath = new GraphicsPath();
            sizingpath.AddRectangle(new RectangleF(this.rcSizing.Left, this.rcSizing.Top, this.rcSizing.Width, nViewportBorder));
            sizingpath.AddRectangle(new RectangleF(this.rcSizing.Right - nViewportBorder, this.rcSizing.Top + nViewportBorder, nViewportBorder, this.rcSizing.Height - (2 * nViewportBorder)));
            sizingpath.AddRectangle(new RectangleF(this.rcSizing.Left, this.rcSizing.Bottom - nViewportBorder + 1, this.rcSizing.Width, nViewportBorder));
            sizingpath.AddRectangle(new RectangleF(this.rcSizing.Left, this.rcSizing.Top + nViewportBorder, nViewportBorder, this.rcSizing.Height - (2 * nViewportBorder)));
            this.rgnSizing.MakeEmpty();
            this.rgnSizing.Union(sizingpath);
            sizingpath.Dispose();
        }

        /// <summary>
        /// Draws the viewport to graphics.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="brushclr">The brush color.</param>
        public void DrawViewport(Graphics grfx, Color brushclr)
        {
            // Draw the Viewport border rect.
            SolidBrush clrbrush = new SolidBrush(brushclr);
            grfx.FillRectangle(clrbrush, this.rcLTcorner);
            grfx.FillRectangle(clrbrush, this.rcRTcorner);
            grfx.FillRectangle(clrbrush, this.rcRBcorner);
            grfx.FillRectangle(clrbrush, this.rcLBcorner);
            grfx.FillRectangle(clrbrush, this.rcTopEdge);
            grfx.FillRectangle(clrbrush, this.rcRightEdge);
            grfx.FillRectangle(clrbrush, this.rcBottomEdge);
            grfx.FillRectangle(clrbrush, this.rcLeftEdge);
            clrbrush.Dispose();

            // If a sizing rect is present, then draw it using a hatch brush.
            if (this.rcSizing != RectangleF.Empty)
            {
                HatchBrush htchbrush = new HatchBrush(HatchStyle.Cross, SystemColors.Control);
                grfx.FillRegion(htchbrush, this.rgnSizing);
                htchbrush.Dispose();
            }
        }
    }
}