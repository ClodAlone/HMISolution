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
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Diagnostics;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides a property that lets you toggle support for Intelli-Mouse panning.
	/// </summary>
	public interface ISupportIntelliMouse
	{
		/// <summary>
		/// Toggles support for Intelli-Mouse panning. When the user presses the middle mouse button and drags the mouse,
		/// the window will scroll.
		/// </summary>
		/// <remarks>
		/// The latest Intelli-Mouse drivers have also built-in </remarks>.
		bool EnableIntelliMouse { get; set; }
	}

	/// <summary>
	/// Implements support for Intelli-Mouse panning. When the user presses the middle mouse button and drags the mouse,
	/// the window will scroll. <see cref="ScrollControl"/> has built-in support for this call. You only have
	/// to enable <see cref="ScrollControl.EnableIntelliMouse"/>.
	/// </summary>
	/// <example>
	/// The following code enables support for IntelliMouseDragScroll:
	/// <code lang="C#">
	///     public bool EnableIntelliMouse
	///         {
	///             get
	///             {
	///                 return imm != null and imm.Enabled;
	///             }
	///             set
	///             {
	///                 if (value != EnableIntelliMouse)
	///                 {
	///                     if (imm == null)
	///                     {
	///                         imm = new IntelliMouseDragScroll(this, true);
	///                         imm.AllowScrolling = ScrollBars.Both;
	///                         imm.DragScroll += new IntelliMouseDragScrollEventHandler(IntelliMouseDragScrollEvent);
	///                     }
	///                     imm.Enabled = value;
	///                 }
	///             }
	///         }
	///
	///         void IntelliMouseDragScrollEvent(object sender, IntelliMouseDragScrollEventArgs e)
	///         {
	///             int dy = e.Dy;
	///             int dx = e.Dx;
	///
	///             this.disableAutoScroll = true;
	///             if (Math.Abs(dy) > Math.Abs(dx))
	///             {
	///                 VScrollBar.SendScrollMessage(dy > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement);
	///             }
	///             else
	///             {
	///                 HScrollBar.SendScrollMessage(dx > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement);
	///             }
	///             this.disableAutoScroll = false;
	///         }
	/// </code>
	/// </example>
	public class IntelliMouseDragScroll: NonFinalizeDisposable
	{
		// Fields
		[ThreadStaticAttribute] static IntelliMouseDragScroll activeImm = null;
		private bool isDragging = false;
		private static object startstopSemaphor = true;
		private static object elapsedSemaphor = true;
		private Point startPoint = Point.Empty;
		private bool vertical = true;
		private bool horizontal = true;
		private Rectangle bitmapRect = Rectangle.Empty;
		private MouseButtons buttonState = MouseButtons.None;
		private Size offset = new Size();
		private Control control;
		private int downMouseTick = int.MinValue;
		private bool clickHold = false;
		private bool enabled = false;
		private bool hookMouseDownMessage = false;

		private Timer repeatScrollEventTimer = null;
		private long nTimerCount = 0;

		[ThreadStaticAttribute] internal static DragWindow _dragWindow = null;
		const string manifestPrefix = "Syncfusion.Windows.Forms.Scrolling.";
		const int clickHoldTicks = 750;

		internal static DragWindow dragWindow
		{
			get
			{
				if (_dragWindow == null)
					_dragWindow = new DragWindow();
				return _dragWindow;
			}
		}

		// Events

		/// <summary>
		/// Occurs when the user has dragged the mouse outside the scrolling bitmap.
		/// </summary>
		public event IntelliMouseDragScrollEventHandler DragScroll;

		// Properties

		/// <summary>
		/// Returns the active <see cref="IntelliMouseDragScroll"/> object, if any.
		/// </summary>
		public static IntelliMouseDragScroll ActiveIntelliMouseDragScroll
		{
			get
			{
				return activeImm;
			}
		}

		/// <summary>
		/// Gets / sets the scrolling direction.
		/// </summary>
		public ScrollBars AllowScrolling
		{
			get
			{
				return (vertical?ScrollBars.Vertical:ScrollBars.None)|(horizontal?ScrollBars.Horizontal:ScrollBars.None);
			}
			set
			{
				vertical = (value & ScrollBars.Vertical) != 0;
				horizontal = (value & ScrollBars.Horizontal) != 0;
			}
		}

		/// <summary>
		/// Indicates whether the user is dragging.
		/// </summary>
		public bool IsDragging
		{
			get
			{
				return this.isDragging;
			}
		}

		// Constructors
		/// <summary>
		/// Initializes the <see cref="IntelliMouseDragScroll"/> object.
		/// </summary>
		/// <param name="control">The control to add this functionality to.</param>
		public IntelliMouseDragScroll(Control control)
			: this(control, false)
		{
		}

		/// <summary>
		/// Initializes the <see cref="IntelliMouseDragScroll"/> object.
		/// </summary>
		/// <param name="control">The control to add this functionality to.</param>
		/// <param name="hookMouseDownMessage">True if <see cref="IntelliMouseDragScroll"/> should listen for MouseDown event; 
        /// False if dragging should be started manually by calling StartDrag.</param>
		public IntelliMouseDragScroll(Control control, bool hookMouseDownMessage)
		{
			this.control = control;
			this.hookMouseDownMessage = hookMouseDownMessage;
			WireControl();
		}

		void WireControl()
		{
			if (enabled)
			{
				if (hookMouseDownMessage)
				{
					if (!(control is ScrollControl))
						control.MouseDown += new MouseEventHandler(ControlMouseDown);
					else
						((ScrollControl) control).ScrollControlMouseDown += new CancelMouseEventHandler(ControlBeforeMouseDown);
				}

				control.MouseUp += new MouseEventHandler(ControlMouseUp);
				control.Click += new EventHandler(ControlClick);

				if (control is ICancelModeProvider)
					((ICancelModeProvider) control).CancelMode += new EventHandler(ControlCancelMode);

				//			if (control is IScrollbarsVisibleChanged)
				//				((IScrollbarsVisibleChanged) control).ScrollbarsVisibleChanged += new EventHandler(ScrollbarsVisibleChanged);
			}
		}

		void UnwireControl()
		{
			if (enabled)
			{
				if (!(control is ScrollControl))
					control.MouseDown -= new MouseEventHandler(ControlMouseDown);
				else
					((ScrollControl) control).ScrollControlMouseDown -= new CancelMouseEventHandler(ControlBeforeMouseDown);
				control.MouseUp -= new MouseEventHandler(ControlMouseUp);
				control.Click -= new EventHandler(ControlClick);
				if (control is ICancelModeProvider)
					((ICancelModeProvider) control).CancelMode -= new EventHandler(ControlCancelMode);
			}
		}


		/// <summary>
		/// Indicates whether to toggle the Intelli-Mouse feature on or off.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					UnwireControl();
					enabled = value;
					WireControl();
				}
			}
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				StopTimer();
				UnwireControl();
				control = null;
				cursor = null;
			}
			base.Dispose(disposing);
		}

		// Methods
		/// <summary>
		/// Starts the IntelliMouse dragging at the given screen coordinates.
		/// </summary>
		/// <param name="startPoint">Screen coordinates, e.g. Control.MousePosition.</param>
		public void StartDrag(Point startPoint)
		{
			this.StopDrag();

			string bitmapName;
			if (this.vertical && this.horizontal)
				bitmapName = "IMALL.BMP";
			else if (this.vertical)
				bitmapName = "IMVERT.BMP";
			else if (this.horizontal)
				bitmapName = "IMHORZ.BMP";
			else
				return;

			Bitmap bmp = GetBitmap(bitmapName);

			if (bmp != null)
			{
				this.isDragging = true;
				this.startPoint = startPoint;
				this.buttonState = Control.MouseButtons;

				this.bitmapRect = new Rectangle(startPoint.X-bmp.Width/4, startPoint.Y-bmp.Height/4,
					bmp.Width/2, bmp.Height/2);

				Point mousePoint = Control.MousePosition;
				//offset = new Size(bmp.Width/2+3, bmp.Height/2+3);
				//startPoint -= offset;
				SetCursor(control, ScrollCursors.DragWheelAllCursor);

				IntelliMouseDragScroll.dragWindow.ShowWindowTopMost();
				IntelliMouseDragScroll.dragWindow.WindowCursor = ScrollCursors.DragWheelAllCursor;
				IntelliMouseDragScroll.dragWindow.TransparencyKey = Color.Red;
				IntelliMouseDragScroll.dragWindow.DragBitmap = bmp;

				IntelliMouseDragScroll.dragWindow.StartDrag(mousePoint);
				Cursor.Current = ScrollCursors.DragWheelAllCursor;
				//Cursor.Position = startPoint;
				StartTimer();
				activeImm = this;
			}
		}

		/// <summary>
		/// Stops the Intelli-Mouse dragging.
		/// </summary>
		public void StopDrag()
		{
			try
			{
				StopTimer();

				if (!this.isDragging)
					return;

				Point mousePos = Control.MousePosition;
				mousePos.Offset(offset.Width, offset.Height);
				Cursor.Position = mousePos;
				SetCursor(control, Cursors.Default);

				this.isDragging = false;
				IntelliMouseDragScroll.dragWindow.StopDrag();
				IntelliMouseDragScroll.dragWindow.StopDrag();
			}
			finally
			{
				activeImm = null;
			}
		}

		void ControlMouseDown(object sender, MouseEventArgs e)
		{
			if (IsDragging)
				StopDrag();
			else if (e.Button == MouseButtons.Middle)
			{
				StartDrag(Control.MousePosition);
				downMouseTick = Environment.TickCount;
			}
			clickHold = false;
		}

		void ControlBeforeMouseDown(object sender, CancelMouseEventArgs e)
		{
			if (IsDragging)
			{
				StopDrag();
				e.Cancel = true;
			}
			else if (e.MouseEventArgs.Button == MouseButtons.Middle)
			{
				StartDrag(Control.MousePosition);
				downMouseTick = Environment.TickCount;
				e.Cancel = true;
			}
			clickHold = false;
		}

		void ControlMouseUp(object sender, MouseEventArgs e)
		{
			if (!clickHold)
				StopDrag();
		}

		void ControlClick(object sender, EventArgs e)
		{
			if (!clickHold)
				StopDrag();
		}

		void ControlCancelMode(object sender, EventArgs e)
		{
			StopDrag();
		}

		/// <summary>
		/// Returns the bitmap from manifest. Red background in bitmap will be made transparent.
		/// </summary>
		/// <param name="bitmapName"></param>
		/// <returns></returns>
		static Bitmap GetBitmap(string bitmapName)
		{
			Bitmap bitmap = null;

			try
			{
				Type type = typeof(IntelliMouseDragScroll);
				Stream stream = type.Module.Assembly.GetManifestResourceStream(manifestPrefix + bitmapName);
				bitmap = new Bitmap(stream);
				if (bitmap != null)
					bitmap.MakeTransparent(Color.Red);
			}
			catch(System.Exception exception)
			{
				MessageBox.Show(exception.Message);
				throw;
			}

			return bitmap;
		}

		private void SetCursor(Control control, Cursor cursor)
		{
			if (control is ScrollControl)
				((ScrollControl) control).OverrideCursor = cursor;
			control.Cursor = cursor;
			this.cursor = cursor;
		}

		Cursor cursor = null;

		/// <summary>
		/// Returns the cursor to be displayed.
		/// </summary>
		public Cursor Cursor
		{
			get
			{
				return cursor;
			}
		}

		private void CheckAction()
		{
			Point mousePos = Control.MousePosition;
			if (!control.Bounds.Contains(control.Parent.PointToClient(mousePos)))
			{
				downMouseTick = int.MaxValue;
				return;
			}
			else if (bitmapRect.Contains(mousePos))
			{
				if (Environment.TickCount - downMouseTick > clickHoldTicks)
					clickHold = true;
				SetCursor(control, ScrollCursors.DragWheelAllCursor);
				SlowTimer();
			}
			else
			{
				downMouseTick = int.MaxValue;
				int dx = 0, dy = 0;
				if (horizontal)
					dx = mousePos.X-(startPoint.X);
				if (vertical)
					dy = mousePos.Y-(startPoint.Y);

				if (Math.Abs(dy) > Math.Abs(dx))
					dx = 0;
				else
					dy = 0;

				if (RaiseIntelliMouseDragScroll(ref dx, ref dy))
				{
					if (Math.Abs(dy) > Math.Abs(dx))
					{
						if (Math.Abs(dy) < bitmapRect.Height)
							SlowTimer();
						if (dy > 0)
							SetCursor(control, ScrollCursors.DragWheelSouthCursor);
						else
							SetCursor(control, ScrollCursors.DragWheelNorthCursor);
					}
					else
					{
						if (Math.Abs(dx) < bitmapRect.Width)
							SlowTimer();
						if (dx > 0)
							SetCursor(control, ScrollCursors.DragWheelEastCursor);
						else
							SetCursor(control, ScrollCursors.DragWheelWestCursor);
					}

				}
			}
		}

		private bool RaiseIntelliMouseDragScroll(ref int dx, ref int dy)
		{
			if (this.DragScroll != null)
			{
				IntelliMouseDragScrollEventArgs ea = new IntelliMouseDragScrollEventArgs(dx, dy);
				try
				{
					this.DragScroll(this, ea);
					dx = ea.DX;
					dy = ea.DY;
					return !ea.Cancel;
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
					return false;
				}
			}
			return true;
		}

		private void StartTimer()
		{
			lock(startstopSemaphor)
			{
				Debug.Assert(this.repeatScrollEventTimer == null, "Oops - StartTimer called twice.");
				repeatScrollEventTimer = new Timer();
				repeatScrollEventTimer.Tick += new EventHandler(OnTimerElapsed);
				repeatScrollEventTimer.Interval = 200; // milliseconds
				nTimerCount = 0;
				repeatScrollEventTimer.Enabled = true;
			}
		}

		private void SlowTimer()
		{
			repeatScrollEventTimer.Interval = 200; // milliseconds
			nTimerCount = 0;
		}

		private void StopTimer()
		{
			lock(startstopSemaphor)
			{
				if (repeatScrollEventTimer != null)
				{
					repeatScrollEventTimer.Tick -= new EventHandler(OnTimerElapsed);
					repeatScrollEventTimer.Dispose();
					repeatScrollEventTimer = null;
					nTimerCount = 0;
				}
			}
		}

		private void OnTimerElapsed(object source, EventArgs e)
		{
			try
			{
				if (repeatScrollEventTimer == null || !repeatScrollEventTimer.Enabled)
					return; // This is just the completion call - we don't want to handle that

				nTimerCount++;

				if (repeatScrollEventTimer.Interval > 25)
					repeatScrollEventTimer.Interval = Math.Max(25, repeatScrollEventTimer.Interval-25); // accelerate
				else
					repeatScrollEventTimer.Interval = 10; // accelerate

				CheckAction();
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				NativeMethods.SendMessage(control.Handle, NativeMethods.WM_CANCELMODE, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}
