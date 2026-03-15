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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// MouseControllerDispatcher coordinates mouse events among competing mouse controllers. Based on
	/// the position of the mouse and context of the control every registered controller's HitTest method
	/// is called to determine the best controller for the following mouse action. This controller will then
	/// receive mouse events.
	/// </summary>
	/// <remarks>
	/// Any Mouse Controller needs to implement the IMouseController interface.<para/>
	/// In its implementation of MouseController.HitTest, the mouse controller should determine whether your
	/// controller wants to handle the mouse events based current context.<para/>
	/// MouseControllerDispatcher will call HitTest for each Mouse Controller that has been registered with
	/// Add(IMouseController). The Mouse Controller that wins the vote will receive all Mouse hovering events
	/// like MouseHoverEnter, MouseHover and MouseHoverLeave as long as its HitTest method indicates that it wants to
	/// handle the mouse event. A MouseHoverLeave notification is guaranteed after MouseHoverEnter has been called.<para/>
	/// When the user presses the mouse, a MouseDown will be sent to the controller. All subsequent mouse events
	/// will then go to that specific controller until the user releases the mouse or the mouse operations is cancelled.
	/// A call to either MouseUp or CancelMode is guaranteed after a controller MouseDown method was called.<para/>
	/// Mouse controllers are registered by calling the Add method.<para/>
	/// If the control that MouseControllerDispatcher should be associated with is derived from ScrollControl,
	/// you should use ScrollControllMouseControllerDispatcher because it will automatically hook itself up
	/// with mouse events from ScrollControl. <para/>
	/// Otherwise if you want to attach MouseControllerDispatcher to a different type of Control, you need to
	/// delegate mouse events to MouseControllerDispatcher. MouseControllerDispatcher provides ProcessXYZ methods
	/// for every mouse event that should be forwarded. Simply call these methods from your mouse event handlers in
	/// your control.
	/// </remarks>
	public class MouseControllerDispatcher: NonFinalizeDisposable, IEnumerable
	{
		// events
		/// <summary>
		/// Indicates that the active controller has changed.
		/// </summary>
		/// <remarks>
		/// Active controller is the controller that is receiving MouseDown, MouseMove and MouseUp messages when the user
		/// has pressed a mouse button.</remarks>
		public event EventHandler ActiveControllerChanged;

		// Fields
		private Control owner;
		private ArrayList mouseControllers = new ArrayList();
		private IMouseController activeController = null;
		private IMouseController mouseHoverController = null;
		private MouseButtons savedMouseButton = MouseButtons.None;
		private bool inMouseUp;
		private MouseEventArgs mouseUpEventArgs = null;
		private bool isMouseTracking = false;
		private Rectangle trackMouse = Rectangle.Empty;
		private int mouseDownTick;
		private Point mouseDownPoint;
		private MouseEventArgs lastMouseEventArgs;
		private Timer timer = null; //new Timer();

		// ctors
		/// <summary>
		/// Initializes a new MouseControllerDispatcher object and associates it with the parent control.
		/// </summary>
		/// <param name="owner"></param>
		public MouseControllerDispatcher(Control owner)
		{
			this.owner = owner;
			owner.MouseLeave += new EventHandler(ControlMouseLeave);
		}

		/// <summary>
		/// Resets the dispatcher and calls Dispose for any registered mouse controller and unregisters all mouse controllers.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                if (timer != null)
                {
                    timer.Tick -= new EventHandler(TimerTick);
                    timer.Dispose();
                }
				owner.MouseLeave -= new EventHandler(ControlMouseLeave);
				activeController = null;
				mouseHoverController = null;
				foreach (object obj in mouseControllers)
				{
					if (obj is IDisposable)
						((IDisposable) obj).Dispose();
				}
				mouseControllers.Clear();
			}

			base.Dispose( disposing );
		}

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
#if DEBUG

			if (Switches.MouseController.TraceVerbose && owner != null && controller != null)

			    TraceUtil.TraceCurrentMethodInfo(owner.Name, controller.Name);
#else

			;
#endif

			mouseControllers.Add(controller);
		}

		/// <summary>
		/// Removes a mouse controller.
		/// </summary>
		/// <param name="controller"></param>
		public void Remove(IMouseController controller)
		{
			if (controller == activeController)
				throw new ArgumentException("Removing active IMouseController object is not allowed.", "controller");

			if (controller == mouseHoverController)
				mouseHoverController = null;
#if DEBUG

            if (Switches.MouseController.TraceVerbose && owner != null && controller != null)
                
			    TraceUtil.TraceCurrentMethodInfo(owner.Name, controller.Name);
#else

			;
#endif

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
		/// Search a mouse controller by comparing with the name returned from IMouseController.Name.
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
		}

		/// <summary>
		/// Changes the active controller and raises an ActiveControllerChanged event.
		/// </summary>
		/// <param name="value"></param>
		void SetActiveController(IMouseController value)
		{
			if (this.activeController != value)
			{
#if DEBUG
                if (Switches.MouseController.TraceVerbose && owner != null)
                    TraceUtil.TraceCurrentMethodInfo(owner.Name, value);
#else
				;
#endif

				this.activeController = value;
				OnActiveControllerChanged(EventArgs.Empty);
			}
		}

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
		protected void SetMouseHoverController(IMouseController value, EventArgs e)
		{
			if (value != mouseHoverController)
			{
				if (mouseHoverController != null)
				{
#if DEBUG
					Trace.WriteLineIf(Switches.MouseController.TraceVerbose, "MouseControllerDispatcher.MouseHoverLeave " + mouseHoverController.Name);
#endif
					mouseHoverController.MouseHoverLeave(e);
				}
				mouseHoverController = value;
				if (mouseHoverController != null)
				{
#if DEBUG
					Trace.WriteLineIf(Switches.MouseController.TraceVerbose, "MouseControllerDispatcher.MouseHoverEnter " + mouseHoverController.Name);
#endif
					mouseHoverController.MouseHoverEnter();
				}
			}
		}


		/// <summary>
		/// Raises the <see cref="MouseControllerDispatcher.ActiveControllerChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnActiveControllerChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.MouseControllerDispatcherEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.ActiveController);
#else
			;
#endif

			try
			{
				if (ActiveControllerChanged != null)
					ActiveControllerChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
				SetActiveController(null);
			}
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return mouseControllers.GetEnumerator();
		}

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
		public int HitTest(Point point)
		{
			IMouseController mc;
			return HitTest(point, MouseButtons.Left, 1, out mc);
		}
		
		/// <summary>
		/// HitTest loops through all controllers and call HitTest on each of them. Only one mouse controller
		/// can get voted to receive mouse messages. 
		/// </summary>
		/// <param name="point">The point in client coordinates to be hit tested.</param>
		/// <param name="mouseButton">The mouse button that is pressed.</param>
		/// <returns>The result identifying the hit-test context.</returns>
		/// <genoverload/>
		public int HitTest(Point point, MouseButtons mouseButton)
		{
			IMouseController mc;
			return HitTest(point, mouseButton, 1, out mc);
		}
		
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
		public int HitTest(Point point, MouseButtons mouseButton, out IMouseController controller)
		{
			return HitTest(point, mouseButton, 1, out controller);
		}

		int hitTestCode = 0;

		/// <summary>
		/// Returns the last HitTest value returned that was non-zero. Check this property
		/// if you need to make decision on your mouse controller's HitTest.
		/// </summary>
		public int LastHitTestCode
		{
			get
			{
				return hitTestCode;
			}
		}

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
		public int HitTest(Point point, MouseButtons mouseButton, int clicks, out IMouseController controller)
		{
			hitTestCode = 0;
			controller = null;

			if (this.Owner.IsDisposed)
				return 0;

			if (point.X == int.MaxValue || point.Y == int.MaxValue)
				return 0;
			MouseEventArgs mouseEventArgs = new MouseEventArgs(mouseButton, clicks, point.X, point.Y, 0);
			// Get a vote which controller should get MouseMoveOver message.
			for (int n = 0; n < mouseControllers.Count; n++)
			{
				IMouseController mc = mouseControllers[n] as IMouseController;
                if (mc == null)
                    continue;
				int hc = mc.HitTest(mouseEventArgs, controller);
				if (hc != 0)
				{
					hitTestCode = hc;
					controller = mc;
				}
			}
			return hitTestCode;
		}


		private bool allowDoubleClickTimer = true;
		
		/// <summary>
		/// Property AllowDoubleClickTimer (bool).
		/// </summary>
		public bool AllowDoubleClickTimer
		{
			get
			{
				return this.allowDoubleClickTimer;
			}
			set
			{
				this.allowDoubleClickTimer = value;
			}
		}

		/// <summary>
		/// Call this method from your control's MouseMove handler.
		/// </summary>
		/// <param name="e"></param>
		public void ProcessMouseMove(MouseEventArgs e)
		{
			if (this.activeController != null)
			{
				if (e.Button != savedMouseButton)
				{
#if DEBUG
					if (Switches.MouseController.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo("Cancel");
#else
					;
#endif

					ProcessCancelMode();
					return;
				}
				this.activeController.MouseMove(e);
                if (owner.Cursor != this.DisplayCursor)
                    Cursor.Current = this.DisplayCursor;
			}
			else if (e.Button == MouseButtons.None)
			{
				IMouseController mouseHoverController;
				Point pt = new Point(e.X, e.Y);
				// Get a vote which controller should get MouseMoveOver message.
				int clicks = 1;
				if (Environment.TickCount - this.mouseDownTick <= SystemInformation.DoubleClickTime)
				{
					if (Math.Abs(mouseDownPoint.X - e.X) < SystemInformation.DoubleClickSize.Width
						&& Math.Abs(mouseDownPoint.Y - e.Y) < SystemInformation.DoubleClickSize.Height)
						clicks = 2;
					lastMouseEventArgs = e;

					if (timer == null && this.AllowDoubleClickTimer)
					{
						timer = new Timer();
						timer.Interval = SystemInformation.DoubleClickTime;
						timer.Tick += new EventHandler(TimerTick);
						timer.Start();
					}
				}
				int hitTestCode = HitTest(pt, MouseButtons.Left, clicks, out mouseHoverController);
				if (TrackMouse.Contains(pt) && mouseHoverController != null)
				{
#if DEBUG
					if (Switches.MouseController.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo(pt, TrackMouse);
#else
					;
#endif

					ProcessMouseDown(new MouseEventArgs(MouseButtons.None, 0, e.X, e.Y, 0));
				}
				else
				{
					SetMouseHoverController(mouseHoverController, e);
					if (MouseHoverController != null)
						MouseHoverController.MouseHover(e);
				}
                Cursor.Current = this.DisplayCursor;
			}		
		}

//		/// <summary>
//		/// Call this method from your control's MouseMove handler.
//		/// </summary>
//		/// <param name="e"></param>
//		public void ProcessMouseMoveHandled(MouseEventArgs e)
//		{
//			if (this.activeController != null)
//			{
//				if (e.Button != savedMouseButton)
//				{
//					TraceUtil.TraceCurrentMethodInfoIf(Switches.MouseController.TraceVerbose, "Cancel");
//					ProcessCancelMode();
//					return;
//				}
//				this.activeController.MouseMoveHandled(e);
//				owner.Cursor = this.DisplayCursor;
//			}
//		}

		void TimerTick(object sender, EventArgs e)
		{
			if (Environment.TickCount - this.mouseDownTick < SystemInformation.DoubleClickTime)
				return;

            Timer t = sender as Timer;
            try
            {
                t.Tick -= new EventHandler(TimerTick);
                t.Dispose();
                timer = null;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;
            }
            
            if (this.activeController == null && !this.isMouseTracking)
			{
				ProcessMouseMove(lastMouseEventArgs);
			}
			this.mouseDownTick = 0;
		}

		void ControlMouseLeave(object sender, EventArgs e)
		{
			SetMouseHoverController(null, e);
		}

		/// <summary>
		/// Returns the cursor to be displayed.
		/// </summary>
		public Cursor DisplayCursor
		{
			get
			{
				if (activeController != null)
					return activeController.Cursor;
				else if (mouseHoverController != null)
					return mouseHoverController.Cursor;

                if (this.Owner != null && !this.Owner.IsDisposed)
                    return this.Owner.Cursor;
                return null;
			}
		}

		/// <summary>
		/// Call this method from your control MouseDown handler.
		/// </summary>
		/// <param name="e"></param>
		public void ProcessMouseDown(MouseEventArgs e)
		{
			if (this.Owner.IsDisposed)
				return;
#if DEBUG

            if (Switches.MouseController.TraceVerbose && owner != null)
                
			    TraceUtil.TraceCurrentMethodInfo("Begin MouseControllerDispatcher.ProcessMouseDown " + e.Button.ToString() + " " + e.Clicks.ToString() + " " + owner.Name);
#else

			;
#endif

			try
			{
				Point pt = new Point(e.X, e.Y);
				if (e.Button == MouseButtons.None && TrackMouse.Contains(pt))
				{
#if DEBUG
					if (Switches.MouseController.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo("Start Tracking");
#else
					;
#endif

					// Start Mouse tracking
					trackMouse = Rectangle.Empty;
					isMouseTracking = true;
					// and simulate a mouse down
				}
				else if (isMouseTracking && activeController != null)
				{
#if DEBUG
					if (Switches.MouseController.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo("End Tracking");
#else
					;
#endif

					// End Mouse tracking
					isMouseTracking = false;
					ProcessMouseUp(e);
					// and continue with a regular mouse down
				}
				else if (activeController != null)
				{
#if DEBUG
					if (Switches.MouseController.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo("CancelMode");
#else
					;
#endif

					ProcessCancelMode();
					return;
				}
				// This might be a different controller than in MouseOver. In MouseOver, we
				// specified MouseButtons.Left. Now we specify e.Button. If the user clicks
				// the middle button, a different controller that did not give HitTest feedback
				// earlier in MouseMove might handle the event.
				IMouseController controller;
				int clicks = 1;
				if (Environment.TickCount - this.mouseDownTick <= SystemInformation.DoubleClickTime)
				{
					if (Math.Abs(mouseDownPoint.X - e.X) < SystemInformation.DoubleClickSize.Width
						&& Math.Abs(mouseDownPoint.Y - e.Y) < SystemInformation.DoubleClickSize.Height)
						clicks = 2;
				}
				//int hitTestCode = HitTest(new Point(e.X, e.Y), e.Button, clicks, out controller);
//				if (clicks == 2)
//					Trace.WriteLine("Double Click" + (controller != null ? controller.ToString() : "null"));
				SetMouseHoverController(null, e);
				int hitTestCode = HitTest(new Point(e.X, e.Y), e.Button, clicks, out controller);
				SetActiveController(controller);
				if (activeController != null)
				{
					activeController.MouseDown(e);
					savedMouseButton = e.Button;
                    Cursor.Current = this.DisplayCursor;
				}
				mouseDownTick = Environment.TickCount;
				mouseDownPoint = new Point(e.X, e.Y);
			}
			finally
			{
#if DEBUG
                if (Switches.MouseController.TraceVerbose && owner != null)
				    TraceUtil.TraceCurrentMethodInfo("End MouseControllerDispatcher.ProcessMouseDown " + e.Button.ToString() + " " + e.Clicks.ToString() + " " + owner.Name);
#else
				;
#endif

			}
		}

		/// <summary>
		/// Call this method from your control's MouseUp handler.
		/// </summary>
		/// <param name="e"></param>
		public void ProcessMouseUp(MouseEventArgs e)
		{
			if (this.Owner.IsDisposed)
				return;
#if DEBUG

            if (Switches.MouseController.TraceVerbose && owner != null)
			    TraceUtil.TraceCurrentMethodInfo("Begin MouseControllerDispatcher.ProcessMouseUp " + e.Button.ToString() + " " + e.Clicks.ToString() + " " + owner.Name);
#else

			;
#endif

			try
			{
				if (activeController != null)
				{
					inMouseUp = true;
					activeController.MouseUp(e);
					mouseUpEventArgs = e;
					inMouseUp = false;
				}
				activeController = null;
                if (owner != null && !owner.IsDisposed)
                    Cursor.Current = this.DisplayCursor;
			}
			finally
			{
#if DEBUG
                if (Switches.MouseController.TraceVerbose && owner != null)
                    TraceUtil.TraceCurrentMethodInfo("End MouseControllerDispatcher.ProcessMouseUp " + e.Button.ToString() + " " + e.Clicks.ToString() + " " + owner.Name);
#else
				;
#endif

			}
		}

//		You should check for e.Clicks == 2 in MouseDown event and then
//      handle event as needed in MouseController.
//		public void ProcessDoubleClick(EventArgs e)
//		{
//			Trace.WriteLineIf(Switches.MouseController.TraceVerbose, "Begin MouseControllerDispatcher.ProcessDoubleClick ");
//			try
//			{
//				if (activeController != null)
//				{
//					inMouseUp = true;
//					mouseUpEventArgs = new MouseEventArgs(mouseUpEventArgs.Button, 2, mouseUpEventArgs.X, mouseUpEventArgs.Y, mouseUpEventArgs.Delta);
//					activeController.MouseUp(mouseUpEventArgs);
//					inMouseUp = false;
//				}
//				activeController = null;
//				owner.Cursor = this.DisplayCursor;
//			}
//			finally
//			{
//				Trace.WriteLineIf(Switches.MouseController.TraceVerbose, "End MouseControllerDispatcher.ProcessDoubleClick ");
//			}
//		}
//

		/// <summary>
		/// Call this method from your control's CancelMode handler.
		/// </summary>
		public void ProcessCancelMode()
		{
			if (this.Owner.IsDisposed)
				return;

			if (inMouseUp)
				return; // probably a message box displayed in MouseUp.

			isMouseTracking = false;
#if DEBUG

            if (Switches.MouseController.TraceVerbose && owner != null)
                TraceUtil.TraceCurrentMethodInfo("Begin MouseControllerDispatcher.ProcessCancelMode " + " " + owner.Name);
#else

			;
#endif

			try
			{
				if (activeController != null)
					activeController.CancelMode();
				activeController = null;
				savedMouseButton = MouseButtons.None;
                Cursor.Current = this.DisplayCursor;
			}
			finally
			{
#if DEBUG
                if (Switches.MouseController.TraceVerbose && owner != null)
                    TraceUtil.TraceCurrentMethodInfo("End MouseControllerDispatcher.ProcessCancelMode " + " " + owner.Name);
#else
				;
#endif

			}
		}

		/// <summary>
		/// Returns a reference to the associated control.
		/// </summary>
		public Control Owner
		{
			get
			{
				return owner;
			}
		}

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


		/// <summary>
		/// Enables support for mouse tracking.
		/// </summary>
		/// <remarks>
		/// Specify the bounds where the mouse tracking should start. As soon as the user moves the mouse
		/// over the specified region, MouseControllerDispatcher will simulate a mouse down event. When the user presses
		/// any mouse button MouseControllerDispatcher will simulate a mouse up and resets the mouse tracking mode. After
		/// the initial click on a mouse button, mouse processing will work as usual.<par/>
		/// <note type="note">Mouse tracking lets you easily simulate the behavior of windows combo boxes.</note>
		/// </remarks>
		/// <example>This example enables Mouse Tracking after the drop-down has been shown.
		/// <code lang="C#">
		/// public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
		/// {
		///     this.ListControlPart.grid.MouseControllerDispatcher.TrackMouse =
		///         this.ListControlPart.grid.RangeInfoToRectangle(GridRangeInfo.Rows(
		///                 this.ListControlPart.grid.TopRowIndex,
		///             this.ListControlPart.grid.RowCount));
		/// }
		/// </code>
		/// </example>
		public Rectangle TrackMouse
		{
			get
			{
				return trackMouse;
			}
			set
			{
#if DEBUG
				if (Switches.MouseController.TraceVerbose)
				    TraceUtil.TraceCurrentMethodInfo(value);
#else
				;
#endif

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
				trackMouse = Rectangle.Empty;
				isMouseTracking = false;
				ProcessCancelMode();
			}
		}


		/// <summary>
		/// Indicates that the value of the TrackMouse property has changed.
		/// </summary>
		public event EventHandler TrackMouseChanged;


		/// <summary>
		/// Raises the <see cref="MouseControllerDispatcher.TrackMouseChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnTrackMouseChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.MouseControllerDispatcherEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.trackMouse);
#else
			;
#endif

			if (TrackMouseChanged != null)
				TrackMouseChanged(this, e);
		}

	}


	/// <summary>
	/// ScrollControllMouseControllerDispatcher is a specialized version of MouseControllerDispatcher
	/// that automatically wires itself up with a ScrollControl.
	/// </summary>
	/// <remarks>
	/// If the control that MouseControllerDispatcher should be associated with is derived from ScrollControl,
	/// you should use ScrollControllMouseControllerDispatcher because it will automatically hook itself up
	/// with mouse events from ScrollControl. <para/>
	/// No initialization is necessary.<para/>
	/// You can register MouseControllers with:
	/// <code>
	/// resizeCellsController = new GridResizeCellsMouseController(this);
	/// MouseControllerDispatcher.Add(resizeCellsController);
	/// </code>
	/// See ScrollControl.MouseControllerDispatcher property.
	/// </remarks>
	public class ScrollControllMouseControllerDispatcher: MouseControllerDispatcher
	{
		ScrollControl owner;
		private MouseEventArgs savedMouseMoveEventArgs = null;
		bool ignoreUICues = false;

		/// <summary>
		/// Initializes a <see cref="ScrollControllMouseControllerDispatcher"/> object and associates it with a <see cref="ScrollControl"/>.
		/// </summary>
		/// <param name="owner">The <see cref="ScrollControl"/> this object is associated with.</param>
		public ScrollControllMouseControllerDispatcher(ScrollControl owner)
			: base(owner)
		{
			this.owner = owner;
			WireEvents();
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnwireEvents();
			}
			base.Dispose(disposing);
		}

		void WireEvents()
		{
			owner.ScrollControlMouseDown += new CancelMouseEventHandler(ScrollControlBeforeMouseDown);
			//owner.ScrollControlMouseMove += new CancelMouseEventHandler(ScrollControlBeforeMouseMove);
			owner.ScrollControlHandledMouseMove += new MouseEventHandler(ScrollControlMouseMoveHandled);
			owner.ScrollControlMouseUp += new CancelMouseEventHandler(ScrollControlBeforeMouseUp);
			owner.CancelMode += new EventHandler(ScrollControlCancelMode);
			owner.ChangeUICues += new UICuesEventHandler(ScrollControlChangeUICues);
			owner.VisibleChanged += new EventHandler(ScrollControlCancelMode);
		}

		void UnwireEvents()
		{
			owner.ScrollControlMouseDown -= new CancelMouseEventHandler(ScrollControlBeforeMouseDown);
			//owner.ScrollControlMouseMove -= new CancelMouseEventHandler(ScrollControlBeforeMouseMove);
			owner.ScrollControlHandledMouseMove -= new MouseEventHandler(ScrollControlMouseMoveHandled);
			owner.ScrollControlMouseUp -= new CancelMouseEventHandler(ScrollControlBeforeMouseUp);
			owner.CancelMode -= new EventHandler(ScrollControlCancelMode);
			owner.ChangeUICues -= new UICuesEventHandler(ScrollControlChangeUICues);
			owner.VisibleChanged -= new EventHandler(ScrollControlCancelMode);
			owner.WindowScrolled -= new ScrollWindowEventHandler(ScrollControlWindowScrolled);
			owner.WindowScrolling -= new ScrollWindowEventHandler(ScrollControlWindowScrolling);
		}


		void ScrollControlChangeUICues(object sender, UICuesEventArgs e)
		{
			if (!ignoreUICues && !owner.IgnoreUICues)
				this.ProcessCancelMode();
		}

		void ScrollControlWindowScrolling(object sender, ScrollWindowEventArgs e)
		{
			if (Control.MouseButtons == MouseButtons.None && MouseHoverController != null)
			{
				// Hovering might have changed if this Paint is the result of scrolling the window.
				// This makes sure the hovered control gets a chance to refresh.
				SetMouseHoverController(null, e);
				this.Owner.Update();
			}
		}

		void ScrollControlWindowScrolled(object sender, ScrollWindowEventArgs e)
		{
			if (Control.MouseButtons == MouseButtons.None)
			{
				if (savedMouseMoveEventArgs != null)
				{
					ProcessMouseMove(savedMouseMoveEventArgs);
				}
			}
		}

		void ScrollControlBeforeMouseDown(object sender, CancelMouseEventArgs e)
		{
			ignoreUICues = true;
			ProcessMouseDown(e.MouseEventArgs);
			ignoreUICues = false;
		}
//		void ScrollControlBeforeMouseMove(object sender, CancelMouseEventArgs e)
//		{
//			savedMouseMoveEventArgs = e.MouseEventArgs;
//			ProcessMouseMove(e.MouseEventArgs);
//		}
		void ScrollControlMouseMoveHandled(object sender, MouseEventArgs e)
		{
			savedMouseMoveEventArgs = e;
			ProcessMouseMove(e);
		}
		void ScrollControlBeforeMouseUp(object sender, CancelMouseEventArgs e)
		{
			ProcessMouseUp(e.MouseEventArgs);
		}
		void ScrollControlCancelMode(object sender, EventArgs e)
		{
			ProcessCancelMode();
		}
	}
}
