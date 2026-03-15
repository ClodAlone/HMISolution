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

using	System;
using	System.ComponentModel;
using	System.Diagnostics;
using	System.Drawing;
using	System.Windows.Forms;

using	Syncfusion.ComponentModel;
using	Syncfusion.Diagnostics;

namespace	Syncfusion.Windows.Forms
{
		///	<summary>
		///		 Helper	class for <see cref="ButtonBar"/>. Manages <see	cref="InternalButton"/>	items.
		///	</summary>
	[
	ToolboxItem(false),
	]
	public class InternalButtonBar:	Disposable
		{
		private	static object	startstopSemaphor	=	true;
		private	static object	elapsedSemaphor	=	true;
		private	InternalButton[] buttons = null;
		private	Control	parent = null;
		private	Rectangle	bounds;
		private	Size buttonSize	=	new	Size(8,8);
		private	int	activeButton = -1;
		private	bool dirty = true;
		private	bool flatLook	=	false;
		private	Timer	repeatClickEventTimer	=	null;
		private	int	repeatClickDelay = 80;
				internal Rectangle placeHolderBounds = Rectangle.Empty;
				private	int	LogicalWidth = 0;
				internal bool	paintPlaceHolder = true;
		int	minRepeatClickDelay	=	5;

		///	<summary>
		///	Initializes	an <see	cref="InternalButtonBar"/> and attaches	it to a	control.
		///	</summary>
		///	<param name="parent">The parent	control.</param>
				public InternalButtonBar(Control parent)
		{
			Debug.Assert(parent	!= null, "Must pass	valid	window control.");
			this.parent	=	parent;
			this.buttons = new InternalButton[0];
			WireEvents();
				}

		void WireEvents()
		{
			if (parent !=	null)
			{
				parent.MouseDown +=	new	MouseEventHandler(OnMouseDownEvent);
				parent.MouseMove +=	new	MouseEventHandler(OnMouseMoveEvent);
				parent.MouseUp +=	new	MouseEventHandler(OnMouseUpEvent);
				parent.MouseLeave	+= new EventHandler(OnMouseLeaveEvent);

				ButtonBar	bar	=	parent as	ButtonBar;
				if (bar	!= null)
					bar.CancelMode +=	new	EventHandler(OnCancelModeEvent);
			}
		}

		void UnwireEvents()
		{
			if (parent !=	null)
			{
				parent.MouseDown -=	new	MouseEventHandler(OnMouseDownEvent);
				parent.MouseMove -=	new	MouseEventHandler(OnMouseMoveEvent);
				parent.MouseUp -=	new	MouseEventHandler(OnMouseUpEvent);
				parent.MouseLeave	-= new EventHandler(OnMouseLeaveEvent);

				ButtonBar	bar	=	parent as	ButtonBar;
				if (bar	!= null)
					bar.CancelMode -=	new	EventHandler(OnCancelModeEvent);
			}
		}

		///	<override/>
		protected	override void	Dispose(bool disposing)
		{
			if (disposing)
			{
				CancelMode();

				if (this.buttons !=	null)
				{
					foreach	(InternalButton	button in	buttons)
						if (button !=	null)
							button.Dispose();
						this.buttons = null;
				}

				UnwireEvents();
				this.parent	=	null;

				if (this.repeatClickEventTimer !=	null)
					this.repeatClickEventTimer.Dispose();

				this.repeatClickEventTimer = null;
			}

			base.Dispose(	disposing	);
		}

		///	<summary>
		///	Called when	a button is	clicked.
		///	</summary>
		///	<param name="button">The <see cref="InternalButton"/> that was clicked.</param>
		///	<remarks>
		///	Called by OnMouseDownEvent.
		///	</remarks>
		protected	virtual	void OnClickedButton(InternalButton	button)
		{
			try
			{
				IInternalButtonParent	target = parent	as IInternalButtonParent;
				if (target !=	null)
					target.OnClickedButton(button);
			}
			catch(Exception	ex)
			{
				// something happend - let's cancel	any	pending	actions.
				CancelMode();
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this,	ex))
					throw;

			}
		}

		internal Rectangle ReverseRectangleRTL(Rectangle r)
		{
			if (this.parent.RightToLeft	== RightToLeft.Yes)
				return new Rectangle(this.bounds.X + this.bounds.Right - r.Right,	r.Top, r.Width,	r.Height);
			return r;
		}


		///	<summary>
		///	Called from	parent control to draw this	bar.
		///	</summary>
		///	<param name="g">A Graphics object.</param>
		public void	Paint(Graphics g)
		{
			Debug.Assert(g !=	null,	"Must	pass valid graphics.");
			try
			{
				if (bounds.IsEmpty ||	buttonSize.IsEmpty)
				{
					Debug.WriteLine("InternalButtonBar.Paint:	Bounds or	ButtonSize is	empty. Nothing to	draw.");
					return;
				}

				if (!g.ClipBounds.IntersectsWith(bounds))
					return;	// nothing to	draw

				// if	dirty	redraw all else	only those buttons that	are	dirty.

				if (LogicalWidth ==	0)
					RecalcLayout(false);

				for	(int i = 0;	i	<	this.buttons.Length; i++)
				{
										InternalButton button	=	this.buttons[i];
										if (button ==	null)
										{
												if (paintPlaceHolder)
												{
														if (this.parent	!= null)
														{

																Brush	br = new SolidBrush(((Control) parent).BackColor);
																g.FillRectangle(br,	ReverseRectangleRTL(this.placeHolderBounds));
																br.Dispose();
														}
														else
															 g.FillRectangle(SystemBrushes.ScrollBar,	ReverseRectangleRTL(this.placeHolderBounds));
												}
												continue;	// placeholder
										}

					button.Paint(g,	ReverseRectangleRTL(button.Bounds),	flatLook,	bounds);
				}

								if (LogicalWidth < bounds.Width	&& placeHolderBounds.IsEmpty)
								{
										Rectangle	rect = new Rectangle(bounds.Left+LogicalWidth, bounds.Top,
												bounds.Width-LogicalWidth, bounds.Height);
										g.FillRectangle(SystemBrushes.ScrollBar, ReverseRectangleRTL(rect));
								}
			}
			finally
			{
				this.dirty = false;
			}
		}

		///	<summary>
		///	Checks if mouse	is over	a button and returns the zero-based	button index or	-1.
		///	</summary>
		///	<param name="x">X-coordinate of	mouse pointer.</param>
		///	<param name="y">Y-coordinate of	mouse pointer.</param>
		///	<returns>Zero-based	button index; -1 if not over a button.</returns>
		public int HitTest(int x,	int	y)
		{
			if (bounds.Contains(x, y))
			{
				for	(int i = 0;	i	<	this.buttons.Length; i++)
				{
										InternalButton button	=	this.buttons[i];
					if (button !=	null &&	ReverseRectangleRTL(button.Bounds).Contains(x, y))
						return i;
				}
			}

			return -1;
		}

		///	<summary>
		///	Initializes	ToolTips boundaries.
		///	</summary>
				public void	InitToolTips()
				{
						RecalcLayout(true);
				}

		///	<summary>
		///	Reinitializes and hides	ToolTips.
		///	</summary>
		public void	ResetToolTips()
		{
			if (buttons	!= null)
								foreach	(InternalButton	button in	buttons)
						if (button !=	null)
												button.ResetToolTip();
		}

		///	<summary>
		///	Recalculates boundaries	of child buttons.
		///	</summary>
		///	<param name="initToolTip">True if ToolTips should be initialized too.</param>
				public void	RecalcLayout(bool	initToolTip)
				{
						int	logWidth = 0;
						int	emptyButton	=	-1;
						Size emptyButtonSize = Size.Empty;

						int	scrollPos	=	0; //	scrolling	(not implemented)
						int	delta	=	0; //	intersecting buttons (not	implemented)
						placeHolderBounds	=	Rectangle.Empty;

						Rectangle	rect = new Rectangle(bounds.Left-scrollPos,	bounds.Top,	0, bounds.Height);
						for	(int i = 0;	i	<	this.buttons.Length; i++)
						{
								InternalButton button	=	this.buttons[i];
								if (button ==	null)
								{
										// placeholder
										if (emptyButton	!= -1)
												throw	new	Exception("A second	empty	button detected.");
										emptyButton	=	i;
								}
								else
								{
										button.AdjustSize();
										if (button.SpinButton	!= SpinButtonType.None)
										{
												if (i+1	<	this.buttons.Length	&& this.buttons[i+1].SpinButton	!= SpinButtonType.None)
												{
														i++;
														this.buttons[i].AdjustSize();
												}
										}

										if (button.Size.IsEmpty)
												// default width
												logWidth +=	this.buttonSize.Width;
										else
												// individual	button size
												logWidth +=	button.Size.Width;
								}
						}

						if (emptyButton	!= -1)
						{
				if (this.bounds.Width	>	logWidth)
									emptyButtonSize	=	new	Size(this.bounds.Width - logWidth, bounds.Height);
				else
					emptyButtonSize	=	Size.Empty;

						}
						bool upDownSkipped = false;

						// Apply bounds.
						for	(int i = 0;	i	<	this.buttons.Length; i++)
						{
								InternalButton button	=	this.buttons[i];

								if (button ==	null)
								{
										rect.Width = emptyButtonSize.Width;
										placeHolderBounds	=	rect;
								}
								else
								{
										if (button.Size.IsEmpty)
												// default width
												rect.Width = this.buttonSize.Width;

										else
												// individual	button size
												rect.Width = button.Size.Width;

										button.Bounds	=	rect;

										if (initToolTip)
												button.InitToolTip(Rectangle.Intersect(ReverseRectangleRTL(button.Bounds), bounds));

										if (button.SpinButton	!= SpinButtonType.None
												&& i+1 < this.buttons.Length
												&& this.buttons[i+1].SpinButton	!= SpinButtonType.None
												&& !upDownSkipped)
										{
												upDownSkipped	=	true;
												continue;
										}
								}

								upDownSkipped	=	false;
								rect.Offset(rect.Width-delta,	0);
						}

						LogicalWidth = logWidth;
				}

		///	<summary>
		///	Cancels current action.
		///	</summary>
				public void	CancelMode()
		{
			StopTimer();
			ResetHovered();
			ResetPushed();
			this.activeButton	=	-1;
			InvalidateIfDirty();
		}

		///	<summary>
		///	Repaints only if marked dirty.
		///	</summary>
		public void	InvalidateIfDirty()
		{
			if (this.Dirty)
				parent.Invalidate(bounds);
		}


		///	<internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected	void OnCancelModeEvent(object	sender,	EventArgs	e)
				{
						CancelMode();
				}

		///	<internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected	void OnMouseDownEvent(object sender, MouseEventArgs	e)
				{
			Debug.Assert(sender	== parent, "OnMouseDownEvent:	sender must	be same	as parent.");
			Debug.Assert(buttons !=	null,	"Buttons are not initialized.");

						// Cancel	if another mouse button	was	pressed
			if (this.Capture)
						{
								CancelMode();
								return;
						}

			ResetHovered();
			int	index	=	HitTest(e.X, e.Y);
			if (index	!= -1)
			{
				InternalButton button	=	buttons[index];
				if (button.Enabled)
				{
					activeButton = index;
					button.Pushed	=	true;

						InvalidateIfDirty();
					if (button.Pushed)
						OnClickedButton(button);

					// make	sure operation was not canceled	...
					if (this.Capture &&	button.Pushed)
										{
												if (button.RepeatClick)
							StartTimer();
										}
					else
						CancelMode();
				}
			}
				}

		///	<internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected	void OnMouseUpEvent(object sender, MouseEventArgs	e)
				{
			Debug.Assert(sender	== parent, "OnMouseDownEvent:	sender must	be same	as parent.");
			Debug.Assert(buttons !=	null,	"Buttons are not initialized.");
			CancelMode();
				}

		///	<internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected	void OnMouseMoveEvent(object sender, MouseEventArgs	e)
				{
			Debug.Assert(sender	== parent, "OnMouseDownEvent:	sender must	be same	as parent.");
			Debug.Assert(buttons !=	null,	"Buttons are not initialized.");
			int	index	=	HitTest(e.X, e.Y);
			if (this.Capture)
			{
								InternalButton button	=	buttons[activeButton];
				if (index	!= activeButton)
				{
					button.Pushed	=	false;
					StopTimer();
				}
				else if	(!button.Pushed)
				{
					button.Pushed	=	true;
										if (!button.Pushed)
												// Couldn't	push button	-	seems	disabled.
												CancelMode();
										else if	(button.RepeatClick)
												StartTimer();
				}
			}
			else if	(!parent.Capture)
			{
				for	(int i = 0;	i	<	buttons.Length;	i++)
										if (buttons[i] !=	null)
						buttons[i].Hovered = (i	== index);
			}

			InvalidateIfDirty();
				}

		///	<internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected	void OnMouseLeaveEvent(object	sender,	EventArgs	e)
				{
			Debug.Assert(sender	== parent, "OnMouseDownEvent:	sender must	be same	as parent.");
			Debug.Assert(buttons !=	null,	"Buttons are not initialized.");

			ResetHovered();
			ResetPushed();
			InvalidateIfDirty();
				}

				private	void StartTimer()
				{
			lock(startstopSemaphor)
			{
				Debug.Assert(this.repeatClickEventTimer	== null, "Oops - StartTimer	called twice.");
				repeatClickEventTimer	=	new	Timer();
				repeatClickEventTimer.Tick +=	new	EventHandler(OnTimerElapsed);
				repeatClickEventTimer.Interval = repeatClickDelay;
				repeatClickEventTimer.Enabled	=	true;
			}
				}

		private	void StopTimer()
		{
			lock(startstopSemaphor)
			{
				if (repeatClickEventTimer	!= null)
				{
					repeatClickEventTimer.Tick -=	new	EventHandler(OnTimerElapsed);
					repeatClickEventTimer.Dispose();
					repeatClickEventTimer	=	null;
				}
			}
		}

		int	lastTimer	=	int.MinValue;

		private	void OnTimerElapsed(object source, EventArgs e)
		{
			if (repeatClickEventTimer	== null	|| Control.MouseButtons	== MouseButtons.None)
				return;	// This	is just	the	completion call	-	we don't want	to handle	that.

			if (Environment.TickCount	<	lastTimer	+	minRepeatClickDelay)
				return;

			if (repeatClickEventTimer.Interval > 35	+	minRepeatClickDelay)
				repeatClickEventTimer.Interval -=	35;	// accelerate
			else if	(repeatClickEventTimer.Interval	>	10 + minRepeatClickDelay)
				repeatClickEventTimer.Interval -=	5; //	accelerate
			else
				repeatClickEventTimer.Interval = minRepeatClickDelay;

			if (!this.Capture)
			{
				Debug.WriteLine("Unnecessary TimerEvent	was	fired. Timer has been	stopped.");
				// invalid state
				StopTimer();
			}
			else
			{
				OnClickedButton(this.buttons[this.activeButton]);
			}

			lastTimer	=	Environment.TickCount;
		}

		///	<summary>
		///	Gets / sets the	button array.
		///	</summary>
		public InternalButton[]	Buttons
		{
			get
			{
				return this.buttons;
			}
			set
			{
				CancelMode();
				this.buttons = value;
								ComputeButtonSize();
				this.dirty = true;
			}
		}

		// Parent	might	check	this to	see	if we	process	mouse	messages.
		///	<summary>
		///  Indicates whether a button is currently pressed.
		///	</summary>
		public bool	Capture
		{
			get
			{
				return this.activeButton !=	-1;
			}
		}

		// Client	bounds.
		///	<summary>
		///	Gets / sets the	boundaries of this bar.
		///	</summary>
		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				if (this.bounds	!= value)
				{
					this.bounds	=	value;
					this.dirty = true;

										ComputeButtonSize();
										RecalcLayout(false);

										if (this.placeHolderBounds.IsEmpty &&	LogicalWidth < value.Width)
												this.bounds.Width	=	this.buttons.Length*this.ButtonSize.Width;
				}
			}
		}

				private	void ComputeButtonSize()
				{
						if (buttons.Length > 0)
						{
								Size size	=	new	Size(this.bounds.Width / buttons.Length, this.bounds.Height);
								InternalButton button	=	buttons[0];
								if (button !=	null)
										this.ButtonSize	=	button.GetPreferredSize(size);
						}
				}

		///	<summary>
		///	Gets / sets the	default	size for buttons in	this bar.
		///	</summary>
		public Size	ButtonSize
		{
			get
			{
				return buttonSize;
			}
			set
			{
				buttonSize = value;
				this.dirty = true;
			}
		}

		///	<summary>
		///	Indicates whether it is flat look	for	buttons.
		///	</summary>
		public bool	FlatLook
		{
			get
			{
				return flatLook;
			}
			set
			{
				flatLook = value;
				this.dirty = true;
			}
		}

		///	<summary>
		///	Indicates whether any button is	dirty or sets all buttons dirty.
		///	</summary>
				public bool	Dirty
				{
			// Check if	this bar or	any	button is	dirty
			get
			{
				if (this.dirty)
					return true;
				else if	(buttons !=	null)	foreach	(InternalButton	button in	buttons)
				{
					if (button !=	null &&	button.Dirty)
						return true;
				}
				return false;
			}
			// Mark	this bar dirty or	reset	all	buttons.
			set
			{
				this.dirty = value;
				if (buttons	!= null)
										foreach	(InternalButton	button in	buttons)
							if (button !=	null)
														button.Dirty = value;
			}
		}

		///	<summary>
		///	Indicates whether any button is	enabled	or sets	all	buttons	enabled	/ disabled.
		///	</summary>
				public bool	Enabled
				{
			// Check if	any	button is	Enabled.
			get
			{
				if (buttons	!= null)
										foreach	(InternalButton	button in	buttons)
						{
							if (button !=	null &&	button.Enabled)
								return true;
						}
				return false;
			}
			// Apply value to	all	buttons.
			set
			{
				if (buttons	!= null)
										foreach	(InternalButton	button in	buttons)
							if (button !=	null)
														button.Enabled = value;

				// If	buttons	are	disabled while processing	mouse, cancel	immediately.
				if (!value &&	Dirty)
					CancelMode();
			}
		}


		///	<summary>
		///	Indicates whether any button is	in hovered state.
		///	</summary>
		public bool	Hovered
				{
			// Check if	any	button is	Hovered.
			get
			{
				if (buttons	!= null)
										foreach	(InternalButton	button in	buttons)
						{
							if (button !=	null &&	button.Hovered)
								return true;
						}
				return false;
			}
		}

		///	<summary>
		///	Resets hovered state for all buttons.
		///	</summary>
		public void	ResetHovered()
		{
			if (buttons	!= null)
								foreach	(InternalButton	button in	buttons)
						if (button !=	null)
												button.Hovered = false;
		}

		///	<summary>
		///	Indicates whether any button is	in pushed state.
		///	</summary>
		public bool	Pushed
				{
			// Check if	any	button is	pushed.
			get
			{
				if (buttons	!= null)
										foreach	(InternalButton	button in	buttons)
						{
							if (button !=	null &&	button.Pushed)
								return true;
						}
				return false;
			}
		}

		///	<summary>
		///	Resets pushed state	for	all	buttons.
		///	</summary>
		public void	ResetPushed()
		{
			if (buttons	!= null)
								foreach	(InternalButton	button in	buttons)
						if (button !=	null)
												button.Pushed	=	false;
		}

		///	<summary>
		///	Gets / sets the	delay until	the	button starts firing click events
		///	when the user holds	down the mouse button.
		///	</summary>
		public int RepeatClickDelay
		{
			get
			{
				return repeatClickDelay;
			}
			set
			{
				repeatClickDelay = value;
			}
		}


		///	<summary>
		///	Gets / sets the	minimum	delay between clicks when scrolling	is accelerated.
		///	</summary>
		public int MinRepeatClickDelay
		{
			get
			{
				return minRepeatClickDelay;
			}
			set
			{
				minRepeatClickDelay	=	value;
			}
		}
		}
}
