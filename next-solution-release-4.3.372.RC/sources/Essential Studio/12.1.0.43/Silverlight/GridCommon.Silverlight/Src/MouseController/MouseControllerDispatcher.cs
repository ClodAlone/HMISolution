#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Browser;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.GridCommon;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{

    /// <summary>
    /// MouseControllerDispatcher has no dependency on ScrollControl. You can use it with any
    /// FrameworkElement-derived class. You only need to forward mouse events to methods from
    /// IMouseEventsTarget interface or add MouseControllerDispatcher to MouseEventTargetCollection.
    /// <para/>
    /// VirtualizingCellsControl uses a derived CellMouseControllerDispatcher class which adds
    /// support for changing context of a mouse operation when pressing mouse inside a textbox
    /// or other UIElement and then switching to a cell selection mode when moving mouse outside textbox.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class MouseControllerDispatcher : NonFinalizeDisposable, IEnumerable
    {
        #region Fields

        private FrameworkElement owner;
        private List<IMouseController> mouseControllers = new List<IMouseController>();
        private IMouseController activeController;
        private IMouseController mouseHoverController;
        private bool isMouseTracking;
        private Rect trackMouse = Rect.Empty;
#if !WinRT
        private MouseEventArgs lastMouseEventArgs;
#else
        private PointerRoutedEventArgs lastMouseEventArgs;
#endif
        private int lastHitTestCode;
        private bool inMouseMove;
        private bool inMouseDown;
        private bool inMouseUp;
        private bool ignoreMouse;
        private bool inCancelMode;
        private bool suspendMouse;
        private ICaptureContext cancelCaptureInfo;
        private bool mouseDown = false;
        private Point mouseDownPoint;
        private Point previousMouseDownPoint;
        private int mouseDownTick = int.MaxValue;
        private int previousMouseDownTick = int.MaxValue;
        private SuspendState suspendState;

        #endregion

        #region Events
        /// <summary>
        /// Indicates that the active controller has changed.
        /// </summary>
        /// <remarks>
        /// Active controller is the controller that is receiving MouseDown, MouseMove and MouseUp messages when the user
        /// has pressed a mouse button.</remarks>
        public event EventHandler ActiveControllerChanged;

        /// <summary>
        /// Indicates that the value of the TrackMouse property has changed.
        /// </summary>
        public event EventHandler TrackMouseChanged;
        #endregion
        #region Ctor, Dispose
        // ctors
        /// <summary>
        /// Initializes a new MouseControllerDispatcher object and associates it with the parent control.
        /// </summary>
        /// <param name="owner"></param>
        public MouseControllerDispatcher(FrameworkElement owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Resets the dispatcher and calls Dispose for any registered mouse controller and unregisters all mouse controllers.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //activeController = null;
                mouseHoverController = null;
                foreach (object obj in mouseControllers)
                {
                    if (obj is IDisposable)
                        ((IDisposable)obj).Dispose();
                }
                mouseControllers.Clear();
                mouseControllers = null;
                if (owner != null)
                    owner = null;
                lastMouseEventArgs = null;
            }

            base.Dispose(disposing);
        }
        #endregion
        #region Owner, Cursor, ToString
        /// <summary>
        /// Returns a reference to the associated control.
        /// </summary>
        public FrameworkElement Owner
        {
            get
            {
                return owner;
            }
        }

#if !WinRT
        /// <summary>
        /// Returns the cursor to be displayed.
        /// </summary>
        public Cursor Cursor
        {
            get
            {
                if (ActiveController != null)
                    return ActiveController.Cursor;
                else if (MouseHoverController != null)
                    return MouseHoverController.Cursor;
                return Cursors.Arrow;
            }
        }
#endif

        /// <override/>
        public override string ToString()
        {
            return String.Concat("A ",
                base.GetType().Name,
                " with ",
                mouseControllers.Count,
                " IMouseControllers."
                );
        }

        #endregion
        #region Manage IMouseController collecion
        /// <summary>
        /// Registers a mouse controller.
        /// </summary>
        /// <param name="controller"></param>
        public void Add(IMouseController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (mouseControllers.Contains(controller))
                throw new ArgumentException("MouseController already exists.", "controller");

            mouseControllers.Add(controller);
        }

        /// <summary>
        /// Removes a mouse controller.
        /// </summary>
        /// <param name="controller"></param>
        public void Remove(IMouseController controller)
        {
            if (controller == ActiveController)
                throw new ArgumentException("Removing active IMouseController object is not allowed.", "controller");

            if (controller == mouseHoverController)
                mouseHoverController = null;

            mouseControllers.Remove(controller);
        }

        /// <summary>
        /// Indicates whether a mouse controller has previously been registered.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool Contains(IMouseController controller)
        {
            return mouseControllers.Contains(controller);
        }

        /// <summary>
        /// Searchs a mouse controller by comparing with the name returned from IMouseController.Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMouseController Find(string name)
        {
            for (int n = 0; n < mouseControllers.Count; n++)
            {
                IMouseController mc = mouseControllers[n] as IMouseController;
                if (mc.Name == name)
                    return mc;
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mouseControllers.GetEnumerator();
        }
        #endregion
        #region Active Controller
        /// <summary>
        /// Returns a reference to the active mouse controller that is receiving MouseDown, MouseMove and MouseUp messages when the user
        /// has pressed a mouse button.
        /// </summary>
        public IMouseController ActiveController
        {
            get
            {
                return this.activeController;
            }
            internal set
            {
                if (this.activeController != value)
                {
                    this.activeController = value;
                    OnActiveControllerChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MouseControllerDispatcher.ActiveControllerChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnActiveControllerChanged(EventArgs e)
        {
            try
            {
                if (ActiveControllerChanged != null)
                    ActiveControllerChanged(this, e);
            }
            catch (Exception ex)
            {
                //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                throw ex;
                //SetActiveController(null);
            }
        }
        #endregion
        #region MouseHoverController
        /// <summary>
        /// Returns the controller that currently receives mouse hovering messages.
        /// </summary>
        protected IMouseController MouseHoverController
        {
            get
            {
                return mouseHoverController;
            }
        }
        /// <summary>
        /// Sets the controller that will receive mouse hovering messages. If the controller is changed,
        /// MouseHoverLeave and MouseHoverEnter calls are made.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="e"></param>
#if !WinRT
        protected void SetMouseHoverController(IMouseController value, MouseEventArgs e)
#else
        protected void SetMouseHoverController(IMouseController value, PointerRoutedEventArgs e)
#endif
        {
            if (value != mouseHoverController)
            {
                if (mouseHoverController != null)
                {
                    mouseHoverController.MouseHoverLeave(e);
                }
                mouseHoverController = value;
                if (mouseHoverController != null)
                {
                    mouseHoverController.MouseHoverEnter(e);
                }
            }
        }
        #endregion
        #region HitTest
        /// <overload>
        /// HitTest loops through all controllers and calls HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </overload>
        /// <summary>
        /// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </summary>
        /// <param name="point">The point in client coordinates to be hit tested.</param>
        /// <returns>The result identifying the hit-test context.</returns>
        /// <remarks>
        /// The current result of the vote gets passed to the next mouse controller. If a controller wants
        /// to handle mouse events, it can decide based on the existing vote if it has higher priority for it
        /// to handle mouse messages than the existing vote.
        /// </remarks>
#if !WinRT
        public int HitTest(Point point)
        {
            IMouseController mc;
            return HitTest(point, MouseButtons.Left, 1, out mc);
        }
#else
        public int HitTest(Point point)
        {
            IMouseController mc;
            return HitTest(point, 1, out mc);
        }
#endif

        /// <summary>
        /// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </summary>
        /// <param name="point">The point in client coordinates to be hit tested.</param>
        /// <param name="mouseButton">The mouse button that is pressed.</param>
        /// <returns>The result identifying the hit-test context.</returns>
        /// <genoverload/>
#if !WinRT
        public int HitTest(Point point, MouseButtons mouseButton)
        {
            IMouseController mc;
            return HitTest(point, mouseButton, 1, out mc);
        }
#else
        //public int HitTest(Point point)
        //{
        //    IMouseController mc;
        //    return HitTest(point, 1, out mc);
        //}
#endif

        /// <summary>
        /// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </summary>
        /// <param name="point">The point in client coordinates to be hit tested.</param>
        /// <param name="mouseButton">The mouse button that is pressed.</param>
        /// <param name="controller">A placeholder where a reference to the winning <see cref="IMouseController"/>
        /// is returned.</param>
        /// <returns>The result identifying the hit-test context.</returns>
        /// <genoverload/>
#if !WinRT
        public int HitTest(Point point, MouseButtons mouseButton, out IMouseController controller)
        {
            return HitTest(point, mouseButton, 1, out controller);
        }
#else
        public int HitTest(Point point, out IMouseController controller)
        {
            return HitTest(point, 1, out controller);
        }
#endif

        /// <summary>
        /// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </summary>
        /// <param name="point">The point in client coordinates to be hit tested.</param>
        /// <param name="mouseButton">The mouse button that is pressed.</param>
        /// <param name="clicks">1 for single-click; 2 for double click.</param>
        /// <param name="controller">A placeholder where a reference to the winning <see cref="IMouseController"/>
        /// is returned.</param>
        /// <returns>The result identifying the hit-test context.</returns>
        /// <genoverload/>
#if !WinRT
        public int HitTest(Point point, MouseButtons mouseButton, int clicks, out IMouseController controller)
        {
            MouseControllerEventArgs mouseControllerEventArgs =
                new MouseControllerEventArgs(null, mouseButton, clicks, point, 0, false);

            return HitTest(mouseControllerEventArgs, out controller);
        }
#else
        public int HitTest(Point point, int clicks, out IMouseController controller)
        {
            MouseControllerEventArgs mouseControllerEventArgs =
                new MouseControllerEventArgs(null, clicks, point, 0, false);

            return HitTest(mouseControllerEventArgs, out controller);
        }
#endif

        /// <summary>
        /// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
        /// can get voted to receive mouse messages. 
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>The result identifying the hit-test context.</returns>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, out IMouseController controller)
        {
            lastHitTestCode = 0;
            controller = null;

            if (double.IsNaN(mouseEventArgs.Location.X) || double.IsNaN(mouseEventArgs.Location.Y))
                return 0;

            mouseEventArgs.CancelCaptureInfo = cancelCaptureInfo;

            // Is mouse cursor over a child UI Element that needs to handle mouse itsself (e.g. checkbox or textbox)
            bool isMouseOverChildElement = false;// cancelCaptureInfo == null && QueryWantsMouseInput(Mouse.PrimaryDevice);

            // In such case each MouseController will take this into consideration (e.g. SelectCellsMouseController.HitTest will
            // return 0 in such case. Other MouseController might decide to still handle the mouse action anyway and
            // in such case the UIElement will not get the mouse action).
            mouseEventArgs.IsMouseOverChildElement = isMouseOverChildElement;

            //TraceUtil.TraceCurrentMethodInfo(Owner.GetType().Name, isMouseOverChildElement);

            // Get a vote which controller should get MouseMoveOver message.
            for (int n = 0; n < mouseControllers.Count; n++)
            {
                IMouseController mc = mouseControllers[n] as IMouseController;
                if (mouseEventArgs.CancelCaptureInfo != null && !mc.SupportsCancelMouseCapture
                    || mouseEventArgs.IsTracking && !mc.SupportsMouseTracking)
                    continue;
                int hc = mc.HitTest(mouseEventArgs, controller);
                if (hc != 0)
                {
                    lastHitTestCode = hc;
                    controller = mc;
                }
            }
            return lastHitTestCode;
        }

        /// <summary>
        /// Queries the WantsMouseInputProperty attached property of the element the mouse is directly over.
        /// </summary>
        /// <param name="mouseDevice">The mouse device.</param>
        /// <returns></returns>
#if !WinRT
        protected virtual bool QueryWantsMouseInput(StylusDevice mouseDevice)
        {
            // TODO: QueryWantsMouseInput
            return true;
            //StylusPointCollection cc = mouseDevice.GetStylusPoints();
            //DependencyObject el = mouseDevice.DirectlyOver as DependencyObject;
            //return el != null && el != Owner && QueryWantsMouseInput(el);
        }
#else
        protected virtual bool QueryWantsMouseInput()
        {
            // TODO: QueryWantsMouseInput
            return true;
            //StylusPointCollection cc = mouseDevice.GetStylusPoints();
            //DependencyObject el = mouseDevice.DirectlyOver as DependencyObject;
            //return el != null && el != Owner && QueryWantsMouseInput(el);
        }
#endif

        // Could make this a delegate ...
        private bool QueryWantsMouseInput(DependencyObject el)
        {
            //return !(Owner is ScrollControl && ((ScrollControl)owner).Children.Contains(el));
            if (VisualContainer.GetWantsMouseInput(el, owner) == false)
                return false;

            //IQueryWantsMouseInput aht = VirtualizingCellsControl.GetQueryWantsMouseInput(el);
            //if (aht != null)
            //    return aht.QueryWantsMouseInput(el);

            return true;
        }

        /// <summary>
        /// Returns the last HitTest value returned that was non-zero. Check this property
        /// if you need to make decision on your mouse controller's HitTest.
        /// </summary>
        public int LastHitTestCode
        {
            get
            {
                return lastHitTestCode;
            }
        }

        #endregion
        #region TrackMouse
        /// <summary>
        /// Enables support for mouse tracking.
        /// </summary>
        /// <remarks>
        /// Specify the bounds where the mouse tracking should start. As soon as the user moves the mouse
        /// over the specified region, MouseControllerDispatcher will simulate a mouse down event. When the user presses
        /// any mouse button MouseControllerDispatcher will simulate a mouse up and resets the mouse tracking mode. After
        /// the inital click on a mouse button, mouse processing will work as usual.<par/>
        /// <note type="note">Mouse tracking lets you easily simulate the behavior of windows combo boxes.</note>
        /// </remarks>
        public Rect TrackMouse
        {
            get
            {
                return trackMouse;
            }
            set
            {
                if (trackMouse != value)
                {
                    if (isMouseTracking)
                        ResetTrackMouse();

                    trackMouse = value;
                    OnTrackMouseChanged(EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Resets support for mouse tracking.
        /// </summary>
        /// <remarks>Call this method after a user interaction that should switch the control back into
        /// normal mouse behavior. For example, when the control gets the focus or when the user clicks a scrollbar.
        /// </remarks>
        public void ResetTrackMouse()
        {
            if (isMouseTracking)
            {
                trackMouse = Rect.Empty;
                isMouseTracking = false;
                CancelMode();
            }
        }

        /// <summary>
        /// Raises the <see cref="MouseControllerDispatcher.TrackMouseChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnTrackMouseChanged(EventArgs e)
        {
            if (TrackMouseChanged != null)
                TrackMouseChanged(this, e);
        }

        /// <summary>
        /// Call this from controllers MouseUp method when you want to enable mouse tracking
        /// when user release mouse button (e.g. let user click on a line and then resize the
        /// line without mouse being pressed down).
        /// </summary>
        public void StartTrackMouse()
        {
            if (!inMouseUp)
                throw new Exception("Only call this from your controllers MouseUp method;");

            if (isMouseTracking)
                throw new Exception("Mouse tracking already enabled. ");

            isMouseTracking = true;
        }
        #endregion
        #region IMouseEventsTarget Members

        /// <summary>
        /// Sets the host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void SetHost(FrameworkElement host)
        {
            this.owner = host;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseEnterEvent"/>�attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void OnMouseEnter(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseLeaveEvent"/>�attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void OnMouseLeave(MouseEventArgs e)
        {
            if (!ignoreMouse && !suspendMouse)
            {
#if !WinRT
                SetMouseHoverController(null, e);
                owner.Cursor = Cursors.Arrow;
#endif
            }
        }

#if !WinRT
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseDownEvent"/>�attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!ignoreMouse && !suspendMouse && ActiveController == null)
            {
                mouseDown = true;
                MouseDown(e);
                UpdateCursor();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseMoveEvent"/>�attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!ignoreMouse && !suspendMouse)
            {
                MouseMove(e);
                UpdateCursor();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseUpEvent"/>�attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!ignoreMouse && !suspendMouse)
            {
                mouseDown = false;
                MouseUp(e);
                UpdateCursor();
            }
        }
#endif


        #endregion
        #region Mouse Handlers and CancelMode
        /// <summary>
        /// Suspends handling mouse events.
        /// </summary>
        public void SuspendMouse()
        {
            suspendMouse = true;
        }

        /// <summary>
        /// Resumes handline mouse events.
        /// </summary>
        public void ResumeMouse()
        {
            suspendMouse = false;
        }
#if !WinRT
        void MouseDown(MouseButtonEventArgs e)
        {
            inMouseDown = true;
            try
            {
                StylusDevice mouseDevice = e.StylusDevice as StylusDevice;
                Point mousePosition = e.GetPosition(owner);
                //mouseDownButton = MouseButtons.Left;// e.GridUtil.GetMouseButton(e);
                mouseDownTick = Environment.TickCount;
                mouseDownPoint = mousePosition;

                if (ActiveController != null)
                {
                    if (isMouseTracking)
                    {
                        // End Mouse tracking
                        MouseUp(e);
                        isMouseTracking = false;
                        ActiveController = null;
                        UpdateCursor();
                        // and continue with a regular mouse down
                    }
                    else
                        CancelMode();
                    return;
                }

                SetMouseHoverController(null, e);

                // This might be a different controller than in MouseOver. In MouseOver, we
                // specified MouseButton.Left. Now we specify e.Button. If the user clicks
                // the middle button, a different controller that did not give HitTest feedback
                // earlier in MouseMove might handle the event.
                int clicks = 1;
                if (IsDoubleClick(mousePosition))
                    clicks = 2;
                IMouseController controller;
                MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, MouseButtons.Left, clicks, mousePosition, 0, true);

                int hitTestCode = HitTest(mouseControllerEventArgs, out controller);

                ActiveController = controller;

                if (ActiveController != null)
                {
                    if (CaptureMouse())
                    {
                        ActiveController.MouseDown(mouseControllerEventArgs);
                        UpdateCursor();
                        e.Handled = true;
                    }
                    else
                        CancelMode();
                }
#if (!SILVERLIGHT && !WinRT)
                else
                {
                    ICellRenderer renderer = mouseControllerEventArgs.DirectlyOverRenderer;
                    IHitTestSelectCells hsc = renderer as IHitTestSelectCells;
                    if (hsc != null)
                    {
                        hsc.MouseDown(owner, mouseControllerEventArgs);
                    }
                }
#endif
            }
            finally
            {
                inMouseDown = false;
            }
        }

        void MouseMove(MouseEventArgs e)
        {
            inMouseMove = true;
            try
            {
                //MouseDevice mouseDevice = e.Device as MouseDevice;
                Point mousePosition = e.GetPosition(owner);
                int clicks = IsDoubleClick(mousePosition) ? 2 : 1;
                MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, inMouseDown ? MouseButtons.Left : MouseButtons.None, clicks, mousePosition, 0, inMouseDown ? true : false);
                mouseControllerEventArgs.CancelCaptureInfo = this.cancelCaptureInfo;

                if (ActiveController != null)
                {
                    ActiveController.MouseMove(mouseControllerEventArgs);
                    UpdateCursor();
                    //e.Handled = true;
                }

                // Regular Hovering, mouse was not pressed.
                if (IsMouseHovering || TrackMouse.Contains(mousePosition))
                {
                    IMouseController mouseHoverController;
                    lastMouseEventArgs = e;
                    // Get a vote which controller should get MouseMoveOver message.

                    //if (clicks == 2 && timer == null && this.AllowDoubleClickTimer)
                    //{
                    //    timer = new DispatcherTimer();
                    //    timer.Interval = new TimeSpan(SystemInformation.DoubleClickTime);
                    //    timer.Tick += new EventHandler(TimerTick);
                    //    timer.Start();
                    //}

                    int hitTestCode = HitTest(mouseControllerEventArgs, out mouseHoverController);
                    if (TrackMouse.Contains(mousePosition) && mouseHoverController != null)
                    {
                        mouseControllerEventArgs.IsTracking = true;
                        IMouseController controller;
                        HitTest(mouseControllerEventArgs, out controller);

                        ActiveController = controller;

                        if (ActiveController != null)
                        {
                            // Start Mouse tracking
                            trackMouse = Rect.Empty;
                            isMouseTracking = true;

                            // and simulate a mouse down  
                            mouseDownPoint = mousePosition;
                            ActiveController.MouseDown(mouseControllerEventArgs);
                        }

                    }
                    else
                    {
                        SetMouseHoverController(mouseHoverController, e);
                        if (MouseHoverController != null)
                        {
                            MouseHoverController.MouseHover(mouseControllerEventArgs);
                            UpdateCursor();
                            //e.Handled = true;
                        }
                        return;

                    }
                }

                // Mouse was pressed, a control has mouse capture (captured != null) but the
                // MouseDown was not handled by this Dispatcher (activeController == null). Instead a child
                // control owns the mouse.

                if (IsMouseCapturedByChildElement)
                {
                    ICaptureContext capturInfo = CreateCaptureInfo(mouseDownPoint);
                    if (capturInfo != null && !capturInfo.PointInContext(mousePosition))
                    {
                        try
                        {
                            ignoreMouse = true;
                            if (capturInfo.CancelMouseCapture()) // Raise a MouseMove event ...
                            {
                                cancelCaptureInfo = capturInfo;

                                mouseControllerEventArgs.Location = mouseDownPoint;

                                IMouseController controller;
                                int hitTestCode = HitTest(mouseControllerEventArgs, out controller);

                                ActiveController = controller;

                                if (ActiveController != null)
                                {
                                    ActiveController.MouseDown(mouseControllerEventArgs);
                                    CaptureMouse();
                                }
                            }
                        }
                        finally
                        {
                            ignoreMouse = false;
                        }
                    }
                }

                if (IsCaptureCanceled && ActiveController != null)
                {
                    if (cancelCaptureInfo.PointInContext(mousePosition))
                    {
                        ActiveController.CancelMode();
                        ActiveController = null;
                        ReleaseMouseCapture();
                        Point pt = cancelCaptureInfo.RecaptureMouse();
                        mouseDownPoint = pt;
                        cancelCaptureInfo = null;
                        UpdateCursor();
                    }
                }
            }
            finally
            {
                inMouseMove = false;
            }
        }

        void MouseUp(MouseEventArgs e)
        {
            previousMouseDownPoint = mouseDownPoint;
            //previousMouseDownButton = mouseDownButton;
            previousMouseDownTick = mouseDownTick;

            inMouseUp = true;
            try
            {
                //MouseDevice mouseDevice = e.Device as MouseDevice;
                Point mousePosition = e.GetPosition(owner);
                MouseButtonEventArgs me = e as MouseButtonEventArgs;
                int clickCount = 1;// me != null ? me.ClickCount : 1;

                MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, MouseButtons.Left, clickCount, mousePosition, 0, false);

                IMouseController controller;
                int hitTestCode = HitTest(mouseControllerEventArgs, out controller);

                if (ActiveController != null)
                {
                    ReleaseMouseCapture();
                    ActiveController.MouseUp(mouseControllerEventArgs);
                }
                //else
                //{
                //    ICellRenderer renderer = mouseControllerEventArgs.DirectlyOverRenderer;
                //    IHitTestSelectCells hsc = renderer as IHitTestSelectCells;
                //    if (hsc != null)
                //    {
                //        hsc.MouseUp(owner, mouseControllerEventArgs);
                //    }
                //}
                // check if user called StartTrackMouse, otherwise turn off active controller
                if (!isMouseTracking)
                    ActiveController = null;

                cancelCaptureInfo = null;
            }
            finally
            {
                inMouseUp = false;
            }

            mouseDownPoint = new Point();
            //mouseDownButton = null;
            mouseDownTick = int.MaxValue;
        }
#endif
        /// <summary>
        /// Cancel any mouse processing.
        /// </summary>
        public void CancelMode()
        {
            isMouseTracking = false;
            if (ignoreMouse || suspendMouse)
                return; // probably a message box displayed in MouseUp.

            mouseDownPoint = new Point();
            //mouseDownButton = null;
            mouseDownTick = int.MaxValue;
            previousMouseDownPoint = mouseDownPoint;
            //previousMouseDownButton = mouseDownButton;
            previousMouseDownTick = mouseDownTick;

            inCancelMode = true;
            try
            {
                suspendState = CreateSuspendState();
                ReleaseMouseCapture();
                if (ActiveController != null)
                    ActiveController.CancelMode();
                ActiveController = null;
                UpdateCursor();
                cancelCaptureInfo = null;
            }
            finally
            {
                inCancelMode = false;
                mouseDown = false;
            }
        }

        /// <summary>
        /// Creates the suspend state when <see cref="CancelMode"/> was called.
        /// </summary>
        /// <returns></returns>
        protected virtual SuspendState CreateSuspendState()
        {
            return new SuspendState(this);
        }

        /// <summary>
        /// Restores the previously saved mode from suspend state created with <see cref="CreateSuspendState"/>.
        /// </summary>
        public void RestoreMode()
        {
            if (suspendState != null)
            {
                suspendState.Restore();
                if (activeController != null)
                {
                    activeController.RestoreMode();
                    CaptureMouse();
                }
                suspendState = null;
            }
        }
        #endregion
        #region ReadOnly State
        /// <summary>
        /// Gets a value indicating whether instance is in MouseDown.
        /// </summary>
        /// <value><c>true</c> if in MouseDown; otherwise, <c>false</c>.</value>
        public bool InMouseDown
        {
            get
            {
                return inMouseDown;
            }
        }

        /// <summary>
        /// Gets a value indicating whether instance is in MouseUp.
        /// </summary>
        /// <value><c>true</c> if in MouseUp; otherwise, <c>false</c>.</value>
        public bool InMouseUp
        {
            get
            {
                return inMouseUp;
            }
        }

        /// <summary>
        /// Gets a value indicating whether instance is in MouseMove.
        /// </summary>
        /// <value><c>true</c> if in MouseMove; otherwise, <c>false</c>.</value>
        public bool InMouseMove
        {
            get
            {
                return inMouseMove;
            }
        }

        /// <summary>
        /// Gets a value indicating whether instance is in CancelMode.
        /// </summary>
        /// <value><c>true</c> if in CancelMode; otherwise, <c>false</c>.</value>
        public bool InCancelMode
        {
            get
            {
                return inCancelMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether mouse events should be ignored.
        /// </summary>
        /// <value><c>true</c> if mouse events should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreMouse
        {
            get
            {
                return ignoreMouse;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse tracking feature is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if mouse tracking feature is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseTracking
        {
            get
            {
                return isMouseTracking;
            }
        }

        /// <summary>
        /// Gets the mouse down location.
        /// </summary>
        /// <value>The mouse down location.</value>
        public Point MouseDownLocation
        {
            get
            {
                return inMouseDown ? mouseDownPoint : previousMouseDownPoint;
            }
        }
        /// <summary>
        /// Gets the mouse down tick (Environment.TickCount at mouse down).
        /// </summary>
        /// <value>The mouse down tick.</value>
        public int MouseDownTick
        {
            get
            {
                return inMouseDown ? mouseDownTick : previousMouseDownTick;
            }
        }
        #endregion
        #region Capture State and CaptureInfo
        /// <summary>
        /// Gets a value indicating whether this instance is mouse captured by child element.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse captured by child element; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsMouseCapturedByChildElement
        {
            get
            {
                if (ActiveController != null)
                    return false;

                return false;
                //DependencyObject captured = Mouse.Captured as DependencyObject;
                //return GridUtil.IsObjectDescendantOfParent(owner, captured);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mouse hovering.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse hovering; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsMouseHovering
        {
            get
            {
                return !InMouseDown;// Mouse.Captured == null && cancelCaptureInfo == null;
            }
        }

        /// <summary>
        /// Creates the capture info when the mouse is pressed
        /// inside a child UIElement.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected virtual ICaptureContext CreateCaptureInfo(Point point)
        {
            // CellMouseControllerDispatcher override this method and returns
            // a CellMouseCaptureInfo. If null is returned then switching context
            // during a mouse operation is not supported.
            //if (owner is VirtualizingCellsControl)
            //    return new CellMouseCaptureInfo((VirtualizingCellsControl)owner, captured);
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mouse captured by child element.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse captured by child element; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaptureCanceled
        {
            get { return cancelCaptureInfo != null; }
        }

        /// <summary>
        /// Gets the cancel capture info.
        /// </summary>
        /// <value>The cancel capture info.</value>
        public ICaptureContext CancelCaptureInfo
        {
            get { return cancelCaptureInfo; }
        }

        #endregion
        #region Helpers
        private bool CaptureMouse()
        {
            ignoreMouse = true;
            try
            {
#if !WinRT
                return owner.CaptureMouse();
#else
                return false;
#endif
            }
            finally
            {
                ignoreMouse = false;
            }
        }

        private void ReleaseMouseCapture()
        {
            ignoreMouse = true;
            try
            {
#if !WinRT
                owner.ReleaseMouseCapture();
#else
                owner.ReleasePointerCaptures();
#endif
            }
            finally
            {
                ignoreMouse = false;
            }
        }

        private void UpdateCursor()
        {
            //TraceUtil.TraceCurrentMethodInfo(Cursor != null ? Cursor.ToString() : "null", mouseHoverController, activeController);
#if !WinRT
            owner.Cursor = Cursor;
#endif
        }

        private bool IsDoubleClick(Point pt)
        {
            return GridUtil.IsDoubleClick(pt, mouseDownTick, previousMouseDownTick, previousMouseDownPoint);
        }
        #endregion
        #region Add AllowDoubleClickTimer later if needed
        //public bool AllowDoubleClickTimer
        //{
        //    get
        //    {
        //        return this.allowDoubleClickTimer;
        //    }
        //    set
        //    {
        //        this.allowDoubleClickTimer = value;
        //    }
        //}

        //private bool allowDoubleClickTimer = true;
        //private DispatcherTimer timer = null; //new DispatcherTimer();
        //private void TimerTick(object sender, EventArgs e)
        //{
        //    if (Environment.TickCount - this.mouseDownTick < SystemInformation.DoubleClickTime)
        //        return;

        //    DispatcherTimer t = sender as DispatcherTimer;
        //    try
        //    {
        //        t.Tick -= new EventHandler(TimerTick);
        //        t.Stop();
        //        timer = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceUtil.TraceExceptionCatched(ex);
        //        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
        //            throw ex;
        //    }

        //    if (this.activeController == null && !this.isMouseTracking)
        //    {
        //        MouseMove(lastMouseEventArgs, true);
        //    }
        //    this.mouseDownTick = 0;
        //}
        #endregion

        #region SuspendState
        /// <summary>
        /// The SupspendState of the MouseControllerDispatcher object when
        /// <see cref="MouseControllerDispatcher.CancelMode"/> was called.
        /// </summary>
        protected class SuspendState
        {
            private IMouseController activeController;
            private int mouseDownTick;
            private Point mouseDownPoint;
            //MouseButtons mouseDownButton = null;
            private int previousMouseDownTick;
            private Point previousMouseDownPoint;
            //MouseButtons previousMouseDownButton = null;
#if !WinRT
            private MouseEventArgs lastMouseEventArgs;
#else
            private PointerRoutedEventArgs lastMouseEventArgs;
#endif
            private int lastHitTestCode;
            private ICaptureContext cancelCaptureInfo;
            bool mouseDown = false;
            MouseControllerDispatcher mc;

            /// <summary>
            /// Initializes a new instance of the <see cref="SuspendState"/> class.
            /// </summary>
            /// <param name="mc">The MouseControllerDispatcher.</param>
            public SuspendState(MouseControllerDispatcher mc)
            {
                this.mc = mc;
                activeController = mc.activeController;
                lastMouseEventArgs = mc.lastMouseEventArgs;
                lastHitTestCode = mc.lastHitTestCode;
                cancelCaptureInfo = mc.cancelCaptureInfo;
                mouseDown = mc.mouseDown;
                mouseDownTick = mc.mouseDownTick;
                //mouseDownButton = mc.mouseDownButton;
                mouseDownPoint = mc.mouseDownPoint;
                previousMouseDownTick = mc.previousMouseDownTick;
                //previousMouseDownButton = mc.previousMouseDownButton;
                previousMouseDownPoint = mc.previousMouseDownPoint;
            }

            /// <summary>
            /// Restores the state for the MouseControllerDispatcher.
            /// </summary>
            public virtual void Restore()
            {
                mc.activeController = activeController;
                mc.mouseDownTick = mouseDownTick;
                mc.lastMouseEventArgs = lastMouseEventArgs;
                mc.lastHitTestCode = lastHitTestCode;
                mc.cancelCaptureInfo = cancelCaptureInfo;
                mc.mouseDown = mouseDown;
                //mc.mouseDownButton = mouseDownButton;
                mc.mouseDownPoint = mouseDownPoint;
                mc.previousMouseDownTick = previousMouseDownTick;
                //mc.previousMouseDownButton = previousMouseDownButton;
                mc.previousMouseDownPoint = previousMouseDownPoint;
            }
        }
        #endregion

#if WinRT
        #region Code for Pointer Press (Mouse Down)

        public void OnPointerPressed(PointerRoutedEventArgs e)
        {
            PointerPressed(e);
        }
        public void PointerPressed(PointerRoutedEventArgs e)
        {
            Point mousePosition = e.GetCurrentPoint(owner).Position;
            mouseDownTick = Environment.TickCount;
            mouseDownPoint = mousePosition;
            inMouseDown = true;
            IMouseController controller;
            MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, 1, e.GetCurrentPoint(owner).Position, 0, true);
            int hitTestCode = HitTest(mouseControllerEventArgs, out controller);
            ActiveController = controller;
            if (ActiveController != null)
            {
                if (owner.CapturePointer(e.Pointer))
                {
                    ActiveController.MouseDown(mouseControllerEventArgs);
                    UpdateCursor();
                }
            }
            inMouseDown = false;
        }

        #endregion

        #region Code for Pointer Released (Mouse Up)

        public void OnPointerReleased(PointerRoutedEventArgs e)
        {
            PointerReleased(e);
        }

        public void PointerReleased(PointerRoutedEventArgs e)
        {
            previousMouseDownPoint = mouseDownPoint;
            previousMouseDownTick = mouseDownTick;
            inMouseUp = true;
            MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, 1, e.GetCurrentPoint(owner).Position, 0, true);
            IMouseController controller;
            int hitTestCode = HitTest(mouseControllerEventArgs, out controller);
            if (ActiveController != null)
            {
                owner.ReleasePointerCaptures();
                ActiveController.MouseUp(mouseControllerEventArgs);
            }
            inMouseUp = false;
        }
        #endregion

        #region Code for Pointer Move (Mouse Move)

        public void OnPointerMove(PointerRoutedEventArgs e)
        {
            PointerMove(e);
        }

        public void PointerMove(PointerRoutedEventArgs e)
        {
            inMouseMove = true;
            Point mousePosition = e.GetCurrentPoint(owner).Position;
            int clicks = IsDoubleClick(mousePosition) ? 2 : 1;
            MouseControllerEventArgs mouseControllerEventArgs = new MouseControllerEventArgs(e, 1, e.GetCurrentPoint(owner).Position, 0, true);
            mouseControllerEventArgs.CancelCaptureInfo = this.cancelCaptureInfo;

            if (ActiveController != null)
            {
                ActiveController.MouseMove(mouseControllerEventArgs);
                UpdateCursor();
            }
            // Regular Hovering, mouse was not pressed.
            if (IsMouseHovering || TrackMouse.Contains(mousePosition))
            {
                IMouseController mouseHoverController;
                lastMouseEventArgs = e;
                // Get a vote which controller should get MouseMoveOver message.

                int hitTestCode = HitTest(mouseControllerEventArgs, out mouseHoverController);
                if (TrackMouse.Contains(mousePosition) && mouseHoverController != null)
                {
                    mouseControllerEventArgs.IsTracking = true;
                    IMouseController controller;
                    HitTest(mouseControllerEventArgs, out controller);

                    ActiveController = controller;

                    if (ActiveController != null)
                    {
                        // Start Mouse tracking
                        trackMouse = Rect.Empty;
                        isMouseTracking = true;

                        // and simulate a mouse down  
                        mouseDownPoint = mousePosition;
                        ActiveController.MouseDown(mouseControllerEventArgs);
                    }

                }
                else
                {
                    SetMouseHoverController(mouseHoverController, e);
                    if (MouseHoverController != null)
                    {
                        MouseHoverController.MouseHover(mouseControllerEventArgs);
                        UpdateCursor();
                    }
                    return;

                }
            }

            // Mouse was pressed, a control has mouse capture (captured != null) but the
            // MouseDown was not handled by this Dispatcher (activeController == null). Instead a child
            // control owns the mouse.

            if (IsMouseCapturedByChildElement)
            {
                ICaptureContext capturInfo = CreateCaptureInfo(mouseDownPoint);
                if (capturInfo != null && !capturInfo.PointInContext(mousePosition))
                {
                    try
                    {
                        ignoreMouse = true;
                        if (capturInfo.CancelMouseCapture()) // Raise a MouseMove event ...
                        {
                            cancelCaptureInfo = capturInfo;

                            mouseControllerEventArgs.Location = mouseDownPoint;

                            IMouseController controller;
                            int hitTestCode = HitTest(mouseControllerEventArgs, out controller);

                            ActiveController = controller;

                            if (ActiveController != null)
                            {
                                ActiveController.MouseDown(mouseControllerEventArgs);
                                CaptureMouse();
                            }
                        }
                    }
                    finally
                    {
                        ignoreMouse = false;
                    }
                }
            }

            if (IsCaptureCanceled && ActiveController != null)
            {
                if (cancelCaptureInfo.PointInContext(mousePosition))
                {
                    ActiveController.CancelMode();
                    ActiveController = null;
                    ReleaseMouseCapture();
                    Point pt = cancelCaptureInfo.RecaptureMouse();
                    mouseDownPoint = pt;
                    cancelCaptureInfo = null;
                    UpdateCursor();
                }
            }
            inMouseMove = false;
        }

        #endregion

        #region Code for Pointer Enter (Mouse Enter)

        void OnPointerEntered(PointerRoutedEventArgs e)
        {
            PointerEntered(e);
        }

        void PointerEntered(PointerRoutedEventArgs e)
        {

        }
        #endregion
#endif
    }

    //public interface IQueryWantsMouseInput
    //{
    //    bool QueryWantsMouseInput(UIElement el);
    //}
}