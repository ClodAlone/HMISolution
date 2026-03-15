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
using	System.Collections;
using	System.ComponentModel;
using	System.Diagnostics;
using	System.Drawing;
using	System.Windows.Forms;

using	Syncfusion.Windows.Forms.Localization;
using	Syncfusion.Diagnostics;
using	Syncfusion.Runtime.InteropServices;

namespace	Syncfusion.Windows.Forms
{
	///	<summary>
	///	Defines	an interface that provides all properties to configure a scrollbar.
	///	</summary>
	public interface IScrollBar
	{
		///	<summary>
		///	Gets or	sets a value to	be added to	or subtracted from the value of the	property when the scroll box is	moved a large distance.
		///	</summary>
		int	LargeChange	{	get; set;	}

		///	<summary>
		///	Gets or sets the upper limit of	values of the scrollable range.
		///	</summary>
		int	Maximum	{	get; set;	}

		///	<summary>
		///	Gets or sets the lower limit of	values of the scrollable range.
		///	</summary>
		int	Minimum	{	get; set;	}

		///	<summary>
		///	Gets or sets the value to be added to or subtracted	from the value of the property when	the	scroll box is moved a small distance.
		///	</summary>
		int	SmallChange	{	get; set;	}

		///	<summary>
		///	Gets or sets a numeric value that represents the current position of the scroll	box	on the scroll bar control.
		///	</summary>
		int	Value	{	get; set;	}

		///	<summary>
		///	Gets or sets a number that represents the current position of the scroll	box	on the scroll bar control.
		///	</summary>
		bool Enabled { get;	set; }

		///	<summary>
		///	Updates	the	scrollbar with latest changes to current position and scrollable range.
		///	</summary>
		void UpdateScrollInfo();

		///	<summary>
		///	Enables	or disables thumbtrack feature for the scrollbar.
		///	</summary>
		bool SupportsThumbTrack	{	get; set;	}

		///	<summary>
		///	Indicates whether the scrollbar is in thumb	drag mode.
		///	</summary>
		bool IsThumbTracking { get;	}

		///	<summary>
		///	Indicates whether ScrollTips should be shown for the scrollbar.
		///	</summary>
		bool SupportsScrollTips	{	get; set;	}
	}

    ///	<summary>
    ///	Defines	an interface that provides all properties to configure a scrollbar.
    ///	</summary>
    public class ScrollBarAdapter
    {
        ScrollBar scrollBar;
        ScrollBarCustomDraw scrollBarCustomDraw; 

        public ScrollBarAdapter(ScrollBarCustomDraw scrollBarCustomDraw)
        {
            this.scrollBarCustomDraw = scrollBarCustomDraw;
        }

        public ScrollBarAdapter(ScrollBar scrollBar)
        {
            this.scrollBar = scrollBar;
        }

        public ScrollBarAdapter(Control c)
        {
            if (c is ScrollBar)
                this.scrollBar = (ScrollBar) c;
            else
                this.scrollBarCustomDraw = (ScrollBarCustomDraw) c;
        }

        ///	<summary>
        ///	Gets /	sets a value to	be added to	or subtracted from the value of the	property when the scroll box is	moved a large distance.
        ///	</summary>
        public virtual int LargeChange 
        { 
            get
            {
              if( scrollBar != null )
                return scrollBar.LargeChange;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.LargeChange;
              else 
                return 0;
            }
            set
            {
              if( scrollBar != null )
                scrollBar.LargeChange = value;
              else if( scrollBarCustomDraw != null )
                scrollBarCustomDraw.LargeChange = value;
            }
        }

        ///	<summary>
        ///	Gets /	sets the upper limit of	values of the scrollable range.
        ///	</summary>
        public virtual int Maximum
        {
            get
            {
              if( scrollBar != null )
                return scrollBar.Maximum;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.Maximum;
              else 
                return 0;
            }
            set
            {
                if (scrollBar != null)
                    scrollBar.Maximum = value;
                else if( scrollBarCustomDraw != null )
                    scrollBarCustomDraw.Maximum = value;
            }
        }


        ///	<summary>
        ///	Gets /	sets the lower limit of	values of the scrollable range.
        ///	</summary>
        public virtual int Minimum
        {
            get
            {
              if( scrollBar != null )
                return scrollBar.Minimum;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.Minimum;
              else
                return 0;
            }
            set
            {
                if (scrollBar != null)
                    scrollBar.Minimum = value;
                else if(scrollBarCustomDraw != null )
                    scrollBarCustomDraw.Minimum = value;
            }
        }


        ///	<summary>
        ///	Gets /	sets the value to be added to or subtracted	from the value of the property when	the	scroll box is moved a small distance.
        ///	</summary>
        public virtual int SmallChange
        {
            get
            {
              if( scrollBar != null )
                return scrollBar.SmallChange;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.SmallChange;
              else 
                return 0;
            }
            set
            {
                if (scrollBar != null)
                    scrollBar.SmallChange = value;
                  else if( scrollBarCustomDraw != null )
                    scrollBarCustomDraw.SmallChange = value;
            }
        }


        ///	<summary>
        ///	Gets /	sets a numeric value that represents the current position of the scroll	box	on the scroll bar control.
        ///	</summary>
        public virtual int Value
        {
            get
            {
                if( scrollBar != null )
                {
                    return scrollBar.Value;
                }
                else if( scrollBarCustomDraw != null )
                {
                    if( this.RightToLeft == RightToLeft.Yes )
                    {
                        return this.ReflectPosition( scrollBarCustomDraw.Value );
                    }
                    else
                    {
                        return scrollBarCustomDraw.Value;
                    }
                }
                else
                    return 0;
            }
            set
            {
                if( scrollBar != null )
                {
                    scrollBar.Value = value;
                }
                else if( scrollBarCustomDraw != null && scrollBarCustomDraw.Value != value )
                {
                    scrollBarCustomDraw.Value = value;
                }
            }
        }

        ///	<summary>
        ///	Gets / sets a number that represents the current position of the scroll	box	on the scroll bar control.
        ///	</summary>
        public virtual bool Enabled
        {
            get
            {
              if( scrollBar != null )
                return scrollBar.Enabled;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.Enabled;
              else
                return false;
            }
            set
            {
                if (scrollBar != null)
                    scrollBar.Enabled = value;
                else if( scrollBarCustomDraw != null )
                    scrollBarCustomDraw.Enabled = value;
            }
        }

        ///	<summary>
        ///	Gets / sets a number that represents the current position of the scroll	box	on the scroll bar control.
        ///	</summary>
        public virtual RightToLeft RightToLeft
        {
            get
            {
              if( scrollBar != null )
                return scrollBar.RightToLeft;
              else if( scrollBarCustomDraw != null )
                return scrollBarCustomDraw.RightToLeft;
              else 
                return 0;
            }
            set
            {
              if( scrollBar != null )
                scrollBar.RightToLeft = value;
              else if( scrollBarCustomDraw != null )
                scrollBarCustomDraw.RightToLeft = value;
            }
        }

        public virtual event EventHandler ValueChanged
        {
            add
            {
                if (scrollBar != null)
                    scrollBar.ValueChanged += value;
                else if( scrollBarCustomDraw != null )
                    scrollBarCustomDraw.ValueChanged += value;
            }
            remove
            {
              if( scrollBar != null )
                scrollBar.ValueChanged -= value;
              else if( scrollBarCustomDraw != null )
                scrollBarCustomDraw.ValueChanged -= value;
            }
        }

        public virtual event ScrollEventHandler Scroll
        {
            add
            {
                if (scrollBar != null)
                    scrollBar.Scroll += value;
                else if( scrollBarCustomDraw != null )
                    scrollBarCustomDraw.Scroll += value;
            }
            remove
            {
              if( scrollBar != null )
                scrollBar.Scroll -= value;
              else if( scrollBarCustomDraw != null )
                scrollBarCustomDraw.Scroll -= value;
            }
        }

        public virtual Control Control
        {
            get
            {
                if (scrollBar != null)
                    return scrollBar;
                else 
                    return scrollBarCustomDraw;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return Control.Handle;
            }
        }

        internal int ReflectPosition( int position )
        {
            if( this.scrollBarCustomDraw != null && this.scrollBarCustomDraw is HScrollBarCustomDraw )
                return ( ( this.Minimum + ( ( this.Maximum - this.LargeChange ) + 1 ) ) - position );

            return position;
        }
    }


	///	<summary>
	///	Returns a reference to a scrollbar contained in a user control.
	///	</summary>
	///	<remarks>
	///	Splittercontrol	and	Workbookcontrol	check for the IScrollBarContainer to get the
	///	scrollbar. This	enables	you	to replace the shared scrollbar with any user control
	///	that also has a	scrollbar. An example is the <see cref="RecordNavigationScrollBar"/>.
	///	</remarks>
	public interface IScrollBarContainer
	{
		///	<summary>
		///	Gets / sets	the	contained scrollbar	in a user control.
		///	</summary>
        Control ScrollBar { get; set;	}
	}

	///	<summary>
	///	ScrollbarWrapper manages scrollbars	for	a control and hides	details	about the scrollbar
	///	from the control that utilizes ScrollBarWrapper. This allows you to	replace	the	concrete
	///	ScrollBar with <see	cref="ReflectScrollBar"/>, <see	cref="FlatScrollBar"/>, <see cref="ScrollBar"/>
	///	or any custom scrollbar	implementation.
	///	</summary>
    public class ScrollBarWrapper : IDisposable, IScrollBarContainer, IScrollBar
	{
		// Fields
        private ScrollBarAdapter scrollBar;
		private	IScrollBar iScrollBar;
		private	Control	parent;
		private	ScrollBars sbType;
		//private	bool inSendMessage = false;
		private	bool wired = false;
		private	Control	savedParentParent	=	null;
		private	bool locked	=	false;
		private	int	updateCount	=	0;
		private	bool supportsThumbTrack	=	false;
		private	bool supportsScrollTips	=	false;
		private	bool isThumbTracking = false;

		// cached	data
		private	int	value	=	0;
		private	int	minimum	=	0;
		private	int	maximum	=	100;
		private	int	largeChange	=	10;
		private	int	smallChange	=	1;
		private	bool enabled = true;
		private	bool init	=	false;
		private	RightToLeft	m_rightToLeft;

		///	<summary>
		///		<para>Occurs when the scroll box has been
		///	    moved by either a mouse or keyboard action.</para>
		///	</summary>
		[
		SRCategory(@"Action"),
		SRDescription(@"Occurs when	the	thumb is moved.")
		]
		public event ScrollEventHandler	Scroll;

		///	<summary>
		///		<para>Occurs when the <see cref="System.Windows.Forms.ScrollBar.Value" /> property has changed,	either by a
        ///	<see cref="System.Windows.Forms.ScrollBar.Scroll"/> event or programmatically.</para>
		///	</summary>
		[
		SRCategory(@"Action"),
		SRDescription(@"Occurs when	the	value of the control changes.")
		]
		public event EventHandler	ValueChanged;

		///	<summary>
		///		<para>Overloaded. Initializes a	new	instance of	the	<see cref="ScrollBarWrapper" />	class.</para>
		///	</summary>
		///	<param name="parent">The parent	control.</param>
		///	<param name="sbType">The scrollbar type: horizontal	or vertical.</param>
		public ScrollBarWrapper(Control	parent,	ScrollBars sbType)
		{
			this.parent	=	parent;
			this.sbType	=	sbType;
			if (parent !=	null)
			{
				parent.VisibleChanged	+= new EventHandler(ParentVisibleChanged);
				parent.ParentChanged +=	new	EventHandler(ParentParentChanged);
				ParentParentChanged(this,	EventArgs.Empty);
			}
		}

		///	<summary>
		///		<para>Initializes a	new	instance of	the	<see cref="ScrollBarWrapper" />	class.</para>
		///	</summary>
		///	<param name="parent">The parent	control.</param>
		///	<param name="sbType">The scrollbar type: horizontal	or vertical.</param>
		///	<param name="scrollBar">The	scrollbar object to	be managed by this instance.</param>
        public ScrollBarWrapper(Control parent, ScrollBars sbType, Control scrollBar)
			:	this(parent, sbType)
		{
			init = true;
			this.scrollBar = scrollBar != null ? new ScrollBarAdapter(scrollBar) : null;
			// ReflectScrollBar	is derived from	ScrollBar but ScrollBar
			// provides	no hooks to	override Minimum, Value, LargeChange
			// values. Therefore I explicitly need to cast to ReflectScrollBar.
			iScrollBar = scrollBar as	IScrollBar;
			init = false;
			FetchScrollBar();
		}

		void ParentParentChanged(object	sender,	EventArgs	e)
		{
			WireParent();
		}

		internal void	WireParent()
		{
			if (savedParentParent	!= null)
				savedParentParent.VisibleChanged -=	new	EventHandler(ParentVisibleChanged);
			savedParentParent	=	this.Parent	!= null	?	this.Parent.Parent : null;
			if (savedParentParent	!= null)
				savedParentParent.VisibleChanged +=	new	EventHandler(ParentVisibleChanged);
		}

		///	<summary>
		///	Implements the <see	cref="IDisposable.Dispose"/> method	and	releases all managed resource for this object.
		///	</summary>
		public void	Dispose()
		{
			if (parent !=	null)
			{
				parent.VisibleChanged	-= new EventHandler(ParentVisibleChanged);
				parent.ParentChanged -=	new	EventHandler(ParentParentChanged);
			}
			if (savedParentParent	!= null)
				savedParentParent.VisibleChanged -=	new	EventHandler(ParentVisibleChanged);
			UnwireScrollEvent();
			if (IsReflect)
				((IDisposable) iScrollBar).Dispose();
			this.scrollBar = null;
			this.iScrollBar	=	null;
			this.parent	=	null;
		}

		void ParentVisibleChanged(object sender, EventArgs e)
		{
			if (!IsReflect)
			{
				if (parent.Visible)
				{
					InitScrollBar();
					WireScrollEvent();
				}
				else
				{
					UnwireScrollEvent();
				}
			}
		}


		///	<summary>
		///	Indicates whether the control should scroll while the user is dragging a scrollbar's thumb.
		///	</summary>
		[
		Browsable(true),
		Category("Behavior"),
		Description("Specifies if the control should scroll	while the user is dragging a scrollbars	thumb."),
		DefaultValue(false)
		]
		public bool	SupportsThumbTrack
		{
			get
			{
				return supportsThumbTrack;
			}
			set
			{
				if (supportsThumbTrack !=	value)
				{
					supportsThumbTrack = value;
					if (iScrollBar !=	null)
						iScrollBar.SupportsThumbTrack	=	supportsThumbTrack;
				}
			}
		}

		///	<summary>
		///	Indicates whether the parent control should show ScrollTips while the user is dragging a scrollbar thumb.
		///	</summary>
		///	<remarks>
		///	<see cref="ScrollControl"/>	Checks this	property to	determine if ScrollTips	should be displayed.
		///	</remarks>
		[
		Browsable(true),
		Category("Behavior"),
		Description("Specifies if the control should show scroll tips while	the	user is	dragging a scrollbar thumb."),
		DefaultValue(false)
		]
		public bool	SupportsScrollTips
		{
			get
			{
				return supportsScrollTips;
			}
			set
			{
				if (supportsScrollTips !=	value)
				{
					supportsScrollTips = value;
					if (iScrollBar !=	null)
						iScrollBar.SupportsScrollTips	=	supportsScrollTips;
				}
			}
		}


		void IScrollBar.UpdateScrollInfo()
		{
			if (iScrollBar !=	null)
			{
				iScrollBar.UpdateScrollInfo();
			}
		}

		///	<summary>
		///	Gets / sets a reference	to the scrollbar that is contained in this wrapper class.
		///	</summary>
		public Control InnerScrollBar
		{
			get
			{
				return scrollBar != null ? scrollBar.Control : null;
			}
			set
			{
				if( InnerScrollBar != value )
				{
					init = true;
					UnwireScrollEvent();
					scrollBar = value != null ? new ScrollBarAdapter( value ) : null;
					iScrollBar = value as IScrollBar;
					InitScrollBar();
					if( parent != null && parent.Visible || IsReflect )
					{
						WireScrollEvent();
					}
					init = false;
				}
			}
		}

        ///	<summary>
		///	Copies all information to another <see cref="ScrollBarWrapper"/> object.
		///	</summary>
		///	<param name="target">The <see cref="ScrollBarWrapper"/>	to receive all copied information.</param>
		public void	CopyTo(ScrollBarWrapper	target)
		{
			try
			{
				if (target !=	null)
				{
					target.Minimum = this.Minimum;
					target.Maximum = this.Maximum;
					target.SmallChange = this.SmallChange;
					target.LargeChange = this.LargeChange;
					target.Value = this.Value;
					target.Enabled = this.Enabled;
					target.SupportsThumbTrack	=	this.SupportsThumbTrack;
					target.SupportsScrollTips	=	this.SupportsScrollTips;
				}
			}
			catch	(Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null,	ex))
					throw;
			}
		}

		Control	IScrollBarContainer.ScrollBar
		{
			get
			{
				return InnerScrollBar;
			}
			set
			{
                if (value != InnerScrollBar)
				{
					InnerScrollBar = value;
				}
			}
		}

		///	<summary>
		///	True if	this is	a Reflecting scrollbar;	False if it	is a simple	scrollbar.
		///	</summary>
		public bool	IsReflect
		{
			get
			{
				return ( iScrollBar is ReflectScrollBar ) || ( scrollBar != null && scrollBar.Control is ReflectScrollBar );
			}
		}

		///	<summary>
		///	True if	this is	a flat scrollbar; False	if it is a simple scrollbar.
		///	</summary>
		public bool	IsFlat
		{
			get
			{
				return iScrollBar	is FlatScrollBar;
			}
		}

		///	<summary>
		///	True if	scroll bar is currently in thumb drag mode.
		///	</summary>
		public bool	IsThumbTracking
		{
			get
			{
				return isThumbTracking;
			}
		}

		///	<summary>
		///	True if	this is	a Reflecting scrollbar;	False if it	is a simple	scrollbar.
		///	</summary>
		public bool	IsEmpty
		{
			get
			{
				return scrollBar ==	null;
			}
		}

		void WireScrollEvent()
		{
			if (scrollBar	!= null	&& !wired)
			{
				scrollBar.Scroll +=	new	ScrollEventHandler(OnInnerScroll);
				scrollBar.ValueChanged +=	new	EventHandler(OnInnerValueChanged);
				wired	=	true;
			}
		}

		void UnwireScrollEvent()
		{
			if (scrollBar	!= null	&& wired)
			{
				scrollBar.Scroll -=	new	ScrollEventHandler(OnInnerScroll);
				scrollBar.ValueChanged -=	new	EventHandler(OnInnerValueChanged);
				wired	=	false;
			}
        }

		void OnInnerScroll(object	sender,	ScrollEventArgs	se)
		{
			if (se.Type	== ScrollEventType.ThumbTrack)
			{
				//				if (!SupportsThumbTrack)
				//					return;
				//				else
				isThumbTracking	=	true;
			}
			else
				isThumbTracking	=	false;


			FetchScrollBar();
			if (Scroll !=	null)
				Scroll(this, se);

			if (se.Type	== ScrollEventType.ThumbPosition)
				OnInnerValueChanged(this,	se);
		}

		///	<summary>
		///		<para>Raises the <see cref="ValueChanged"/> event.</para>
		///	</summary>
		///	<param name="sender">.</param>
		///	<param name="e">An <see	cref="System.EventArgs"	/> that	contains the event data.</param>
		void OnInnerValueChanged(object	sender,	EventArgs	e)
		{
			if (ValueChanged !=	null)
				ValueChanged(this, e);
		}

		///	<summary>
		///	Suspends updating the scrollbar	until <see cref="EndUpdate"/> is called.
		///	</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void	BeginUpdate()
		{
			this.updateCount++;	// see also	WndProc	(WM_PAINT)
			this.locked	=	true;
		}

		///	<summary>
		///	Resumes	updating the scrollbar after a <see	cref="BeginUpdate"/> call.
		///	</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void	EndUpdate()
		{
			if (--updateCount	== 0)
			{
				this.locked	=	false;
				this.InitScrollBar();
			}
		}

		///	<summary>
		///	Indicates whether updating is locked. See <see cref="BeginUpdate"/>.
		///	</summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
		public bool	Locked
		{
			get
			{
				return this.locked;
			}
		}

		///	<summary>
		///	Fetches	scrollbar information from the managed scrollbar object	and	updates	the	information
		///	in the <see	cref="ScrollBarWrapper"/> object.
		///	</summary>
		public void	FetchScrollBar()
		{
			if (init ||	locked)
				return;

			if (iScrollBar !=	null)
			{
				this.largeChange = iScrollBar.LargeChange;
				this.smallChange = iScrollBar.SmallChange;
				this.value = iScrollBar.Value;
				this.minimum = iScrollBar.Minimum;
				this.maximum = iScrollBar.Maximum;
				this.enabled = iScrollBar.Enabled;

				//	ALEXK: RTL support
				if(	iScrollBar is	ScrollBar	)
					this.m_rightToLeft = ((	ScrollBar	)iScrollBar).RightToLeft;
			}
			else if	(scrollBar !=	null)
			{
				this.largeChange = scrollBar.LargeChange;
				this.smallChange = scrollBar.SmallChange;                
                this.value = scrollBar.Value;
				this.minimum = scrollBar.Minimum;
				this.maximum = scrollBar.Maximum;
				this.enabled = scrollBar.Enabled;

				//	ALEXK: RTL support
				this.m_rightToLeft = scrollBar.RightToLeft;
			}
        }

		///	<summary>
		///	Applies	scrollbar information to the managed scrollbar object based	on the information
		///	in the current <see	cref="ScrollBarWrapper"/> object.
		///	</summary>
		public void	InitScrollBar()
		{
			try
			{
				if(	locked ) return;

				if(	iScrollBar !=	null )
				{
					iScrollBar.Minimum = this.minimum;
					iScrollBar.Maximum = this.maximum;
					iScrollBar.LargeChange = this.largeChange;
					iScrollBar.SmallChange = this.smallChange;
					iScrollBar.Value = Math.Max( Math.Min( this.maximum, this.value	), this.minimum	);
					iScrollBar.Enabled = this.enabled;
					iScrollBar.SupportsThumbTrack	=	supportsThumbTrack;
					iScrollBar.SupportsScrollTips	=	supportsScrollTips;

					//	ALEXK: RTL support
					if(	iScrollBar is	ScrollBar	)
						(( ScrollBar )iScrollBar).RightToLeft	=	m_rightToLeft;

					iScrollBar.UpdateScrollInfo();
				}
				else if( scrollBar !=	null )
				{
					scrollBar.LargeChange	=	this.largeChange;
					scrollBar.SmallChange	=	this.smallChange;
					scrollBar.Minimum	=	this.minimum;
					scrollBar.Maximum	=	this.maximum;
					scrollBar.Value	=	Math.Max(Math.Min(this.maximum,	this.value), this.minimum);

					//	ALEXK: RTL support
					scrollBar.RightToLeft	=	m_rightToLeft;
					scrollBar.Enabled	=	this.enabled;
				}
			}
			catch	(Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null,	ex))
					throw;
			}
		}

		///	<summary>
		///	Gets /	sets a value to	be added to	or subtracted from the value of the	property when the scroll box is	moved a large distance.
		///	</summary>
		public int LargeChange
		{
			get
			{
				FetchScrollBar();
				return this.largeChange;
			}
			set
			{
				if (!locked)
				{
					if (iScrollBar !=	null)
						iScrollBar.LargeChange = value;
					else if	(scrollBar !=	null)
						scrollBar.LargeChange	=	value;
				}
				this.largeChange = value;
			}
		}

		///	<summary>
		///	Gets /	sets the upper limit of	values of the scrollable range.
		///	</summary>
		public int Maximum
		{
			get
			{
				FetchScrollBar();
				return this.maximum;
			}
			set
			{
				if (!locked)
				{
					if (iScrollBar !=	null)
						iScrollBar.Maximum = value;
					else if	(scrollBar !=	null)
						scrollBar.Maximum	=	value;
				}
				this.maximum = value;
			}
		}

		///	<summary>
		///	Gets /	sets the lower limit of	values of the scrollable range.
		///	</summary>
		public int Minimum
		{
			get
			{
				FetchScrollBar();
				return this.minimum;
			}
			set
			{
				if (!locked)
				{
					if (iScrollBar !=	null)
						iScrollBar.Minimum = value;
					else if	(scrollBar !=	null)
						scrollBar.Minimum	=	value;
				}
				this.minimum = value;
			}
		}

		///	<summary>
		///	Gets /	sets the value to be added to or subtracted	from the value of the property when	the	scroll box is moved a small distance.
		///	</summary>
		public int SmallChange
		{
			get
			{
				FetchScrollBar();
				return this.smallChange;
			}
			set
			{
				if (!locked)
				{
					if (iScrollBar !=	null)
						iScrollBar.SmallChange = value;
					else if	(scrollBar !=	null)
						scrollBar.SmallChange	=	value;
				}
				this.smallChange = value;
			}
		}

		///	<summary>
		///	Gets /	sets a numeric value that represents the current position of the scroll	box	on the scroll bar control.
		///	</summary>
		public int Value
		{
			get
			{
				FetchScrollBar();
				return this.value;
			}
			set
			{
				try
				{
					if (!locked)
					{
						if (iScrollBar !=	null)
							iScrollBar.Value = value;
						else if	(scrollBar !=	null)
							scrollBar.Value	=	value;
					}
					this.value = value;
				}
				catch
				{
				}
			}
		}

		///	<summary>
		///	Gets /	sets a number that represents the current position of the scroll box on the scroll bar control.
		///	</summary>
		public bool	Enabled
		{
			get
			{
				FetchScrollBar();
				return this.enabled;
			}
			set
			{
				if (!locked)
				{
					if (iScrollBar !=	null)
						iScrollBar.Enabled = value;
					else if	(scrollBar !=	null)
						scrollBar.Enabled	=	value;
				}
				this.enabled = value;
			}
		}

		///	<summary>
		///	Call this for reflected	scrollbars from	your parent	control's <see cref="Control.WndProc"/> method if you
		///	want to	support	reflected scrollbars (those	window scrollbars that you enable with WS_VSCROLL and
		///	WS_HSCROLL window styles).
		///	</summary>
		///	<param name="m">The	<see cref="Message"/> that was passed as argument to <see	cref="Control.WndProc"/>.</param>
		public void	ReflectScrollMessage(ref Message m)
		{
			if (iScrollBar is	ReflectScrollBar
				&& (m.Msg	== NativeMethods.WM_HSCROLL
				|| m.Msg ==	NativeMethods.WM_VSCROLL))
				((ReflectScrollBar)	iScrollBar).ReflectScrollMessage(ref m);

			// Other messages	are	ignored
		}

		//		public bool	InSendMessage
		//		{
		//			get
		//			{
		//				return inSendMessage;
		//			}
		//		}

		//		public void	Refresh()
		//		{
		//			if (!locked	&& scrollBar !=	null)
		//			{
		//				if (IsReflect	&& iScrollBar	!= null)
		//					iScrollBar.UpdateScrollInfo();
		//				else if	(scrollBar !=	null)
		//					scrollBar.Refresh();
		//			}
		//		}


		///	<summary>
		///	Sends or emulates a	scroll event.
		///	</summary>
		///	<param name="et"></param>
		public void	SendScrollMessage(ScrollEventType	et)
		{
			if (!locked)
			{
				//inSendMessage	=	true;
				try
				{
					int	msg	=	(this.sbType ==	ScrollBars.Horizontal) ? NativeMethods.WM_HSCROLL	:	NativeMethods.WM_VSCROLL;
					if (IsReflect)
						NativeMethods.SendMessage(parent.Handle, msg,	(IntPtr) et, IntPtr.Zero);
					else if	(scrollBar !=	null)
					{
                        if (scrollBar.Control is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sb = (ScrollBarCustomDraw) scrollBar.Control;
                            switch (et)
                            {
                                case ScrollEventType.SmallIncrement:
                                    sb.Value += sb.SmallChange;
                                    OnInnerScroll(sb, new ScrollEventArgs(et, sb.Value));
                                    break;
                                case ScrollEventType.SmallDecrement:
                                    sb.Value -= sb.SmallChange;
                                    OnInnerScroll(sb, new ScrollEventArgs(et, sb.Value));
                                    break;
                            }
                        }
                        else
                        {
                            NativeMethods.SendMessage(scrollBar.Handle, msg | NativeMethods.WM_REFLECT, (IntPtr)et, IntPtr.Zero);
                            if (iScrollBar != null)
                                iScrollBar.UpdateScrollInfo();
                        }
					}

				}
				catch	(Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null,	ex))
						throw;
				}
				finally
				{
					//					inSendMessage	=	false;
				}
			}
		}

		///	<summary>
		///	Returns	a reference	to the parent control.
		///	</summary>
		public Control Parent
		{
			get
			{
				return parent;
			}
		}

		public RightToLeft RightToLeft
		{
			get
			{
				return m_rightToLeft;
			}
			set
			{
				if(	value	!= m_rightToLeft )
				{
					this.value = 0;
					m_rightToLeft	=	value;

					InitScrollBar();
				}
			}
		}
    }
}
