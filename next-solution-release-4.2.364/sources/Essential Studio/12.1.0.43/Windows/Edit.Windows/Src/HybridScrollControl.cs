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
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Runtime;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit
{
	#region *** ReflectScrollBar
	[ToolboxItem( false )]
	internal class ReflectScrollBar
		: ScrollBar
		, IScrollBar
	{
		#region Internal Classes
		internal enum SIF
		{
			RANGE = 0x0001,
			PAGE = 0x0002,
			POS = 0x0004,
			DISABLENOSCROLL = 0x0008,
			TRACKPOS = 0x0010,
			ALL = SIF.RANGE | SIF.PAGE | SIF.POS | SIF.TRACKPOS
		}
		#endregion

		#region Fields
		private bool supportsThumbTrack;
		private bool supportsScrollTips;
		private bool isThumbTracking = false;
		private Control reflectParent;
		private ScrollBars scrollBarType;
		private int m_iValue;
		private int m_iMinimum;
		private int m_iMaximum;
		private int m_iLargeChange;
		private int m_iSmallChange;
		#endregion

		#region Properties
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Control ReflectParent
		{
			get
			{
				return reflectParent;
			}
		}
		/// <summary>
		/// true if currently in thumb drag mode
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IsThumbTracking
		{
			get
			{
				return isThumbTracking;
			}
		}
		/// <summary>
		/// Specifies if the associated control should scroll while the user is dragging a scrollbars thumb
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies if the associated control should scroll while the user is dragging a scrollbars thumb." )]
		[DefaultValue( false )]
		public bool SupportsThumbTrack
		{
			get
			{
				return supportsThumbTrack;
			}
			set
			{
				supportsThumbTrack = value;
			}
		}
		/// <summary>
		/// Specifies if the parent control should show scroll tips while the user is dragging a scrollbars thumb
		/// </summary>
		/// <remarks>
		/// <see cref="ScrollControl"/> checks this property to determine if scrolltips should be displayed.
		/// </remarks>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies if the control should show scroll tips while the user is dragging a scrollbars thumb." )]
		[DefaultValue( false )]
		public bool SupportsScrollTips
		{
			get
			{
				return supportsScrollTips;
			}
			set
			{
				if( supportsScrollTips != value )
				{
					supportsScrollTips = value;
				}
			}
		}
		[DefaultValueAttribute( 10 )]
		[CategoryAttribute( "Behavior" )]
		public new int LargeChange
		{
			get
			{
				return m_iLargeChange;
			}
			set
			{
				if( m_iLargeChange != value )
				{
					base.LargeChange = value;
					m_iLargeChange = value;
					UpdateScrollInfo();
				}
			}
		}
		[CategoryAttribute( "Behavior" )]
		[DefaultValueAttribute( 100 )]
		public new int Maximum
		{
			get
			{
				return m_iMaximum;
			}
			set
			{
				if( m_iMaximum != value )
				{
					base.Maximum = value;
					m_iMaximum = value;
					UpdateScrollInfo();
				}
			}
		}
		[CategoryAttribute( "Behavior" )]
		[DefaultValueAttribute( 0 )]
		public new int Minimum
		{
			get
			{
				return m_iMinimum;
			}
			set
			{
				if( m_iMinimum != value )
				{
					base.Minimum = value;
					m_iMinimum = value;
					UpdateScrollInfo();
				}
			}
		}
		[CategoryAttribute( "Behavior" )]
		[DefaultValueAttribute( 1 )]
		public new int SmallChange
		{
			get
			{
				return m_iSmallChange;
			}
			set
			{
				if( m_iSmallChange != value )
				{
					base.SmallChange = value;
					m_iSmallChange = value;
					this.UpdateScrollInfo();
				}
			}
		}
		[DefaultValueAttribute( 0 )]
		[CategoryAttribute( "Behavior" )]
		[BindableAttribute( true )]
		public new int Value
		{
			get
			{
				return m_iValue;
			}
			set
			{
				if( m_iValue != value )
				{
					m_iValue = value;
					base.Value = value;
					UpdateScrollInfo();
				}
			}
		}
		#endregion

		#region Initialization
		public ReflectScrollBar( Control reflectParent, ScrollBars scrollBarType )
		{
			this.reflectParent = reflectParent;
			this.scrollBarType = scrollBarType;

			SCROLLINFO si = new SCROLLINFO();
			si.cbSize = Marshal.SizeOf( typeof( SCROLLINFO ) );
			si.fMask = ( int )SIF.ALL;

			if( ScrollApi.GetScrollInfo( this.reflectParent.Handle, this.ScrollInfoBar, ref si ) && si.nPos != -1 )
			{
				base.Minimum = si.nMin;
				base.Maximum = si.nMax;
				base.Value = Math.Max( Math.Min( si.nPos, si.nMax ), si.nMin );
				base.LargeChange = si.nPage;
				base.SmallChange = 1;
			}
		}
		#endregion

		#region Static Methods
		public static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}
		public static int LOWORD( IntPtr n )
		{
			return LOWORD( ( int )n );
		}
		#endregion

		#region Public Methods
		public void ReflectScrollMessage( ref Message m )
		{
			SCROLLINFO si;
			ScrollEventArgs sa;
			ScrollEventType saType = ( ScrollEventType )LOWORD( m.WParam );

#if SCROLLTIP
            bool relayScrollTip = false;
#endif

			if( this.RightToLeft == RightToLeft.Yes )
			{
				switch( saType )
				{
					case ScrollEventType.First:
						saType = ScrollEventType.Last;
						break;
					case ScrollEventType.Last:
						saType = ScrollEventType.First;
						break;
					case ScrollEventType.SmallDecrement:
						saType = ScrollEventType.SmallIncrement;
						break;
					case ScrollEventType.SmallIncrement:
						saType = ScrollEventType.SmallDecrement;
						break;
					case ScrollEventType.LargeDecrement:
						saType = ScrollEventType.LargeIncrement;
						break;
					case ScrollEventType.LargeIncrement:
						saType = ScrollEventType.LargeDecrement;
						break;
				}
			}

			int newValue = this.Value;
			isThumbTracking = false;
			switch( saType )
			{
				case ScrollEventType.First:
					newValue = this.Minimum;
					break;
				case ScrollEventType.Last:
					newValue = ( ( this.Maximum - this.LargeChange ) + 1 );
					break;
				case ScrollEventType.SmallDecrement:
					newValue = Math.Max( ( this.Value - this.SmallChange ), this.Minimum );
					break;
				case ScrollEventType.SmallIncrement:
					newValue = Math.Min( ( this.Value + this.SmallChange ), ( ( this.Maximum - this.LargeChange ) + 1 ) );
					break;
				case ScrollEventType.LargeDecrement:
					newValue = Math.Max( ( this.Value - this.LargeChange ), this.Minimum );
					break;
				case ScrollEventType.LargeIncrement:
					newValue = Math.Min( ( this.Value + this.LargeChange ), ( ( this.Maximum - this.LargeChange ) + 1 ) );
					break;

				case ScrollEventType.ThumbTrack:
					isThumbTracking = true;

					goto case ScrollEventType.ThumbPosition;

				case ScrollEventType.ThumbPosition:
					//					int position = ( int )( ( 0xffff0000 & ( int )m.WParam ) >> 16 );
					//					if( position != 0 )
					//					{
					//						newValue = position;
					//					}
					//					else
					//					{
					si = new SCROLLINFO();
					si.cbSize = Marshal.SizeOf( typeof( SCROLLINFO ) );
					si.fMask = ( int )SIF.ALL;
					ScrollApi.GetScrollInfo( this.reflectParent.Handle, this.ScrollInfoBar, ref si );

					int pos = /*( isThumbTracking ) ? */( si.nTrackPos ) /*: ( si.nPos )*/;
					if( this.RightToLeft == RightToLeft.Yes )
					{
						newValue = this.ReflectPosition( pos );
					}
					else
					{
						newValue = pos;
					}
					//					}

					break;
			}

			sa = new ScrollEventArgs( saType, newValue );

			this.OnScroll( sa );
			base.Value = Math.Max( Math.Min( sa.NewValue, Maximum ), Minimum );
		}
		void IScrollBar.UpdateScrollInfo()
		{
			UpdateScrollInfo();
		}
		#endregion

		#region Protected Methods
		protected new void UpdateScrollInfo()
		{
			SCROLLINFO si;
			if( this.IsHandleCreated )
				base.UpdateScrollInfo();
			else if( this.reflectParent != null )
			{
				si = new SCROLLINFO( ( int )( SIF.ALL | SIF.DISABLENOSCROLL ),
					this.Minimum,
					this.Maximum,
					Math.Min( this.LargeChange, ( ( this.Maximum - this.Minimum ) + 1 ) ),
					( this.RightToLeft == RightToLeft.Yes )
					? this.ReflectPosition( this.Value )
					: this.Value );


				ScrollApi.SetScrollInfo( this.reflectParent.Handle, this.ScrollInfoBar, ref si, true );
			}
		}
		#endregion

		#region Overrides
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode = true )]
		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{

#if CTLCOLOR
            case 0x0019/*WM_CTLCOLOR*/:
            case 0x2019/*WM_CTLCOLOR*/:
                WmReflectCtlColor(ref m);
                break;
#endif

				case 0x2114/*WM_HSCROLL*/:
				case 0x2115/*WM_VSCROLL*/:
				case 0x0114/*WM_HSCROLL*/:
				case 0x0115/*WM_VSCROLL*/:
					ReflectScrollMessage( ref m );
					break;

				default:
					base.WndProc( ref m );
					break;
			}
		}
		#endregion

		#region Private Properties
		private int ScrollInfoBar
		{
			get
			{
				return this.scrollBarType == ScrollBars.Horizontal ? 0 : 1;
			}
		}
		#endregion

		#region Private Methods
		private int ReflectPosition( int position )
		{
			if( scrollBarType == ScrollBars.Horizontal )
				return ( ( this.Minimum + ( ( this.Maximum - this.LargeChange ) + 1 ) ) - position );

			return position;
		}
		#endregion

		#region if CTLCOLOR
#if CTLCOLOR
	    protected override void OnBackColorChanged(EventArgs e)  
	    {
            this.RecreateBrush();
            this.Invalidate();
            base.OnBackColorChanged(e);
	    }

        private void EnsureBrushCreated()  
	    {
	    	if (this.brushHandle != IntPtr.Zero)
                return;

            Color color = Color.Green;
	    	if (ColorTranslator.ToOle(color) == 0)
	    	   this.brushHandle = NativeMethods.GetSysColorBrush((ColorTranslator.ToOle(color) & 0xff));
            else
	    	   this.brushHandle = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(color));
	    }

	    private void RecreateBrush()  
	    {
            if (this.brushHandle != IntPtr.Zero) 
                NativeMethods.DeleteObject(this.brushHandle);
            this.brushHandle = IntPtr.Zero;
            this.EnsureBrushCreated();
	    }

		static int HIWORD(int n)  
		{
			return ((n >> 16) & 0xffff);
		}
		
		private void WmReflectCtlColor(ref Message m)
        {
            if (HIWORD((int) m.LParam) ==  0x0005/*CTLCOLOR_SCROLLBAR*/)
            {
                IntPtr hdc = m.WParam;
                NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(this.ForeColor));
                NativeMethods.SetBkColor(hdc, ColorTranslator.ToWin32(this.BackColor));
                this.EnsureBrushCreated();
                m.Result = this.brushHandle;
            }
        }
#endif
		#endregion
	}
	#endregion

	#region *** IScrollBar
	///	<summary>
	///	Defines	an interface that	provides	all	properties to	configure	a	scrollbar.
	///	</summary>
	public interface IScrollBar
	{
		#region Properties
		///	<summary>
		///	Gets or	sets a value to	be added to	or subtracted	from to	the	Value	property when	the	scroll box is	moved	a	large	distance.
		///	</summary>
		int LargeChange { get; set;	}
		///	<summary>
		///	Gets or	sets the upper limit of	values of	the	scrollable range.
		///	</summary>
		int Maximum { get; set;	}
		///	<summary>
		///	Gets or	sets the lower limit of	values of	the	scrollable range.
		///	</summary>
		int Minimum { get; set;	}
		///	<summary>
		///	Gets or	sets the value to	be added to	or subtracted	from to	the	Value	property when	the	scroll box is	moved	a	small	distance.
		///	</summary>
		int SmallChange { get; set;	}
		///	<summary>
		///	Gets or	sets a numeric value that	represents the current position	of the scroll	box	on the scroll	bar	control.
		///	</summary>
		int Value { get; set;	}
		///	<summary>
		///	Gets or	sets a numeric enabled that	represents the current position	of the scroll	box	on the scroll	bar	control.
		///	</summary>
		bool Enabled { get;	set; }
		///	<summary>
		///	Enables	or disables	thumbtrack feature for the scrollbar.
		///	</summary>
		bool SupportsThumbTrack { get; set;	}
		///	<summary>
		///	Checks if	scrollbar	is in	thumb	drag mode
		///	</summary>
		bool IsThumbTracking { get;	}
		///	<summary>
		///	Specifies	if scroll	tips should	be shown for the scrollbar.
		///	</summary>
		bool SupportsScrollTips { get; set;	}
		#endregion

		#region Methods
		///	<summary>
		///	Updates	the	scrollbar	with latest	changes	to current position	and	scrollable range.
		///	</summary>
		void UpdateScrollInfo();
		#endregion
	}
	#endregion

	#region *** IScrollBarContainer
	///	<summary>
	///	Provides a property that	returns	a	reference	to a scrollbar contained in	a	user control.
	///	</summary>
	///	<remarks>
	///	Splittercontrol	and	Workbookcontrol	check	for	IScrollBarContainer	to get the
	///	scrollbar. This	enables	you	to replaces	the	shared scrollbar with	any	user control
	///	that also	has	a	scrollbar. An	example	is the <see	cref="RecordNavigationScrollBar"/>.
	///	</remarks>
	public interface IScrollBarContainer
	{
		#region Properties
		///	<summary>
		///	Returns	the	contained	scrollbar	in a user	control.
		///	</summary>
		ScrollBar ScrollBar { get; set;	}
		#endregion
	}
	#endregion

	#region *** ScrollBarWrapper
	///	<summary>
	///	ScrollbarWrapper manages scrollbars	for	a	control	and	hides	details	about	the	scrollbar
	///	from the control that	utilizes ScrollBarWrapper. This	allows you to	replace	the	concrete
	///	ScrollBar	with <see	cref="ReflectScrollBar"/>, <see	cref="FlatScrollBar"/> a <see	cref="ScrollBar"/>
	///	or any custom	scrollbar	implementation.
	///	</summary>
	public class ScrollBarWrapper
		: IDisposable
		, IScrollBarContainer
		, IScrollBar
	{
		#region Fields
		private ScrollBar scrollBar;
		private IScrollBar iScrollBar;
		private Control parent;
		private ScrollBars sbType;
		private bool wired = false;
		private Control savedParentParent = null;
		private bool locked = false;
		private int updateCount = 0;
		private bool supportsThumbTrack = false;
		private bool supportsScrollTips = false;
		private bool isThumbTracking = false;
		// cached	data
		private int value = 0;
		private int minimum = 0;
		private int maximum = 100;
		private int largeChange = 10;
		private int smallChange = 1;
		private bool enabled = true;
		private bool init = false;
		private RightToLeft m_rightToLeft;
		#endregion

		#region Properties
		///	<summary>
		///	Specifies	if the control should	scroll while the user	is dragging	a	scrollbars thumb
		///	</summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies if	the	control	should scroll	while	the	user is	dragging a scrollbars	thumb." )]
		[DefaultValue( false )]
		public bool SupportsThumbTrack
		{
			get
			{
				return supportsThumbTrack;
			}
			set
			{
				if( supportsThumbTrack != value )
				{
					supportsThumbTrack = value;
					if( iScrollBar != null )
						iScrollBar.SupportsThumbTrack = supportsThumbTrack;
				}
			}
		}
		///	<summary>
		///	Specifies	if the parent	control	should show	scroll tips	while	the	user is	dragging a scrollbars	thumb
		///	</summary>
		///	<remarks>
		///	<see cref="ScrollControl"/>	checks this	property to	determine	if scrolltips	should be	displayed.
		///	</remarks>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies if	the	control	should show	scroll tips	while	the	user is	dragging a scrollbars	thumb." )]
		[DefaultValue( false )]
		public bool SupportsScrollTips
		{
			get
			{
				return supportsScrollTips;
			}
			set
			{
				if( supportsScrollTips != value )
				{
					supportsScrollTips = value;
					if( iScrollBar != null )
						iScrollBar.SupportsScrollTips = supportsScrollTips;
				}
			}
		}
		///	<summary>
		///	Checks if	updating is	locked.	See	<see cref="BeginUpdate"/>
		///	</summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool Locked
		{
			get
			{
				return this.locked;
			}
		}
		///	<summary>
		///	A	reference	to the ScrollBar that	is contained in	this wrapper class.
		///	</summary>
		public ScrollBar InnerScrollBar
		{
			get
			{
				return scrollBar;
			}
			set
			{
				if( scrollBar != value )
				{
					init = true;
					UnwireScrollEvent();
					scrollBar = value;
					iScrollBar = value as IScrollBar;
					InitScrollBar();
					if( parent != null && parent.Visible || IsReflect )
						WireScrollEvent();
					init = false;
				}
			}
		}
		///	<summary>
		///	true if	this is	a	Reflecting scrollbar,	false	if it	is a simple	scrollbar
		///	</summary>
		public bool IsReflect
		{
			get
			{
				return ( iScrollBar is ReflectScrollBar ) || ( scrollBar is ReflectScrollBar );
			}
		}
		///	<summary>
		///	true if	currently	in thumb drag	mode
		///	</summary>
		public bool IsThumbTracking
		{
			get
			{
				return isThumbTracking;
			}
		}
		///	<summary>
		///	true if	this is	a	Reflecting scrollbar,	false	if it	is a simple	scrollbar
		///	</summary>
		public bool IsEmpty
		{
			get
			{
				return scrollBar == null;
			}
		}
		///	<summary>
		///	Gets or	sets a value to	be added to	or subtracted	from to	the	Value	property when	the	scroll box is	moved	a	large	distance.
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
				if( !locked )
				{
					if( iScrollBar != null )
						iScrollBar.LargeChange = value;
					else if( scrollBar != null )
						scrollBar.LargeChange = value;
				}
				this.largeChange = value;
			}
		}
		///	<summary>
		///	Gets or	sets the upper limit of	values of	the	scrollable range.
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
				if( !locked )
				{
					if( iScrollBar != null )
						iScrollBar.Maximum = value;
					else if( scrollBar != null )
						scrollBar.Maximum = value;
				}
				this.maximum = value;
			}
		}
		///	<summary>
		///	Gets or	sets the lower limit of	values of	the	scrollable range.
		///	</summary>
		public int Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if( !locked )
				{
					if( iScrollBar != null )
						iScrollBar.Minimum = value;
					else if( scrollBar != null )
						scrollBar.Minimum = value;
				}
				this.minimum = value;
			}
		}
		///	<summary>
		///	Gets or	sets the value to	be added to	or subtracted	from to	the	Value	property when	the	scroll box is	moved	a	small	distance.
		///	</summary>
		public int SmallChange
		{
			get
			{
				return this.smallChange;
			}
			set
			{
				if( !locked )
				{
					if( iScrollBar != null )
						iScrollBar.SmallChange = value;
					else if( scrollBar != null )
						scrollBar.SmallChange = value;
				}
				this.smallChange = value;
			}
		}
		///	<summary>
		///	Gets or	sets a numeric value that	represents the current position	of the scroll	box	on the scroll	bar	control.
		///	</summary>
		public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				try
				{
					if( !locked )
					{
						if( iScrollBar != null )
							iScrollBar.Value = value;
						else if( scrollBar != null )
							scrollBar.Value = value;
					}

					if( this.value != value )
					{
						this.value = value;
					}
				}
				catch
				{
				}
			}
		}
		///	<summary>
		///	Gets or	sets a numeric enabled that	represents the current position	of the scroll	box	on the scroll	bar	control.
		///	</summary>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if( !locked )
				{
					if( iScrollBar != null )
						iScrollBar.Enabled = value;
					else if( scrollBar != null )
						scrollBar.Enabled = value;
				}
				this.enabled = value;
			}
		}
		/// <summary>
		/// Gets or sets visibility.
		/// </summary>
		public bool Visible
		{
			get
			{
				return ( scrollBar != null ) ? scrollBar.Visible : false;
			}
			set
			{
				if( scrollBar != null )
					scrollBar.Visible = value;
			}
		}
		///	<summary>
		///	Returns	a	reference	to the parent	control.
		///	</summary>
		public Control Parent
		{
			get
			{
				return parent;
			}
		}
		/// <summary>
		/// Gets or sets RTL.
		/// </summary>
		public RightToLeft RightToLeft
		{
			get
			{
				return m_rightToLeft;
			}
			set
			{
				if( value != m_rightToLeft )
				{
					this.value = 0;
					m_rightToLeft = value;

					InitScrollBar();
				}
			}
		}
		/// <summary>
		/// Gets or sets inner scroll bar.
		/// </summary>
		ScrollBar IScrollBarContainer.ScrollBar
		{
			get
			{
				return InnerScrollBar;
			}
			set
			{
				if( value != InnerScrollBar )
				{
					InnerScrollBar = value;
				}
			}
		}
		#endregion

		#region Initialization & Finalization
		///	<summary>
		///	<para>Initializes	a	new	instance of	the	<see cref="ScrollBarWrapper" />	class.</para>
		///	</summary>
		///	<param name="parent">The parent	control.</param>
		///	<param name="sbType">The scrollbar type: horizontal	or vertical.</param>
		public ScrollBarWrapper( Control parent, ScrollBars sbType )
		{
			this.parent = parent;
			this.sbType = sbType;
			if( parent != null )
			{
				parent.VisibleChanged += new EventHandler( ParentVisibleChanged );
				parent.ParentChanged += new EventHandler( ParentParentChanged );
				ParentParentChanged( this, EventArgs.Empty );
			}
		}
		///	<summary>
		///	<para>Initializes	a	new	instance of	the	<see cref="ScrollBarWrapper" />	class.</para>
		///	</summary>
		///	<param name="parent">The parent	control.</param>
		///	<param name="sbType">The scrollbar type: horizontal	or vertical.</param>
		///	<param name="scrollBar">The	scrollbar	object to	be managed by	this instance.</param>
		public ScrollBarWrapper( Control parent, ScrollBars sbType, ScrollBar scrollBar )
			: this( parent, sbType )
		{
			init = true;
			this.scrollBar = scrollBar;
			// ReflectScrollBar	is derived from	ScrollBar	but	ScrollBar
			// provides	no hooks to	override Minimum,	Value, LargeChange
			// values. Therefore I explicitly	need to	cast to	ReflectScrollBar.
			iScrollBar = scrollBar as IScrollBar;
			init = false;
			FetchScrollBar();
		}
		///	<summary>
		///	Implements the <see	cref="IDisposable.Dispose"/> method	and	release	all	managed	resource for this	object.
		///	</summary>
		public void Dispose()
		{
			if( parent != null )
			{
				parent.VisibleChanged -= new EventHandler( ParentVisibleChanged );
				parent.ParentChanged -= new EventHandler( ParentParentChanged );
			}
			if( savedParentParent != null )
				savedParentParent.VisibleChanged -= new EventHandler( ParentVisibleChanged );
			UnwireScrollEvent();
			if( IsReflect )
				( ( IDisposable )iScrollBar ).Dispose();
			this.scrollBar = null;
			this.iScrollBar = null;
			this.parent = null;
		}
		#endregion

		#region Public Methods
		///	<summary>
		///	Copy all information to	another	<see cref="ScrollBarWrapper"/> object.
		///	</summary>
		///	<param name="target">The <see	cref="ScrollBarWrapper"/>	to receive all copied	information.</param>
		public void CopyTo( ScrollBarWrapper target )
		{
			if( target != null )
			{
				target.Minimum = this.Minimum;
				target.Maximum = this.Maximum;
				target.SmallChange = this.SmallChange;
				target.LargeChange = this.LargeChange;
				target.Value = this.Value;
				target.Enabled = this.Enabled;
				target.SupportsThumbTrack = this.SupportsThumbTrack;
				target.SupportsScrollTips = this.SupportsScrollTips;
			}
		}
		///	<summary>
		///	Suspends updating	the	scrollbar	until	<see cref="EndUpdate"/>	is called.
		///	</summary>
		[EditorBrowsable( EditorBrowsableState.Advanced )]
		public void BeginUpdate()
		{
			this.updateCount++;	// see also	WndProc	(WM_PAINT)
			this.locked = true;
		}
		///	<summary>
		///	Resumes	updating the scrollbar after a <see	cref="BeginUpdate"/> call.
		///	</summary>
		[EditorBrowsable( EditorBrowsableState.Advanced )]
		public void EndUpdate()
		{
			if( --updateCount == 0 )
			{
				this.locked = false;
				this.InitScrollBar();
			}
		}
		///	<summary>
		///	Fetches	scrollbar	information	from the managed scrollbar object	and	updates	the	information
		///	in the <see	cref="ScrollBarWrapper"/>	object.
		///	</summary>
		public void FetchScrollBar()
		{
			if( init || locked )
				return;

			if( iScrollBar != null )
			{
				this.value = iScrollBar.Value;
				this.enabled = iScrollBar.Enabled;

				//	ALEXK: RTL support
				if( iScrollBar is ScrollBar )
					this.m_rightToLeft = ( ( ScrollBar )iScrollBar ).RightToLeft;
			}
			else if( scrollBar != null )
			{
				this.value = scrollBar.Value;
				this.enabled = scrollBar.Enabled;

				//	ALEXK: RTL support
				this.m_rightToLeft = scrollBar.RightToLeft;
			}
		}
		///	<summary>
		///	Applies	scrollbar	information	to the managed scrollbar object	bases	on the information
		///	in the current <see	cref="ScrollBarWrapper"/>	object.
		///	</summary>
		public void InitScrollBar()
		{
			if( locked ) return;

			if( iScrollBar != null )
			{
				iScrollBar.Minimum = this.minimum;
				iScrollBar.Maximum = this.maximum;
				iScrollBar.LargeChange = this.largeChange;
				iScrollBar.SmallChange = this.smallChange;
				iScrollBar.Value = Math.Max( Math.Min( this.maximum, this.value ), this.minimum );
				iScrollBar.Enabled = this.enabled;
				iScrollBar.SupportsThumbTrack = supportsThumbTrack;
				iScrollBar.SupportsScrollTips = supportsScrollTips;

				//	ALEXK: RTL support
				if( iScrollBar is ScrollBar )
					( ( ScrollBar )iScrollBar ).RightToLeft = m_rightToLeft;

				iScrollBar.UpdateScrollInfo();
			}
			else if( scrollBar != null )
			{
				scrollBar.LargeChange = this.largeChange;
				scrollBar.SmallChange = this.smallChange;
				scrollBar.Minimum = this.minimum;
				scrollBar.Maximum = this.maximum;
				scrollBar.Value = Math.Max( Math.Min( this.maximum, this.value ), this.minimum );

				//	ALEXK: RTL support
				scrollBar.RightToLeft = m_rightToLeft;
				scrollBar.Enabled = this.enabled;
			}
		}
		///	<summary>
		///	Call this	for	reflected	scrollbars from	your parent	controls <see	cref="Control.WndProc"/> method	if you
		///	want to	support	reflected	scrollbars (those	window scrollbars	that you enable	with WS_VSCROLL	and
		///	WS_HSCROLL window	styles).
		///	</summary>
		///	<param name="m">The	<see cref="Message"/>	that was passed	as argument	to <see	cref="Control.WndProc"/>.</param>
		public void ReflectScrollMessage( ref Message m )
		{
			if( m.Msg == ( int )Msg.WM_HSCROLL || m.Msg == ( int )Msg.WM_VSCROLL )
				if( iScrollBar is ReflectScrollBar )
				{
					( ( ReflectScrollBar )iScrollBar ).ReflectScrollMessage( ref m );
				}
				else
				{
				}

			// Other messages	are	ignored
		}
		///	<summary>
		///	Sends	or emulates	a	scroll event.
		///	</summary>
		///	<param name="et">ScrollEventType.</param>
		public void SendScrollMessage( ScrollEventType et )
		{
			if( !locked )
			{
				try
				{
					int msg = ( this.sbType == ScrollBars.Horizontal ) ? ( int )Msg.WM_HSCROLL : ( int )Msg.WM_VSCROLL;
					if( IsReflect )
						WinAPI.SendMessage( parent.Handle, msg, ( IntPtr )et, IntPtr.Zero );
					else if( scrollBar != null )
					{
						WinAPI.SendMessage( scrollBar.Handle, msg | ( int )Msg.WM_REFLECT, ( IntPtr )et, IntPtr.Zero );
						if( iScrollBar != null )
							iScrollBar.UpdateScrollInfo();
					}
				}
				finally
				{
				}
			}
		}
		void IScrollBar.UpdateScrollInfo()
		{
			if( iScrollBar != null )
			{
				iScrollBar.UpdateScrollInfo();
			}
		}
		#endregion

		#region Events
		///	<summary>
		///	<para>Occurs when	the	scroll box has been moved by	either a mouse or	keyboard action.</para>
		///	</summary>
		public event ScrollEventHandler Scroll;
		///	<summary>
		///	<para>Occurs when	the	<see cref="System.Windows.Forms.ScrollBar.Value" />	property has changed,	either by	a
		///	<see cref="System.Windows.Forms.ScrollBar.Scroll"	/> event or	programatically.</para>
		///	</summary>
		public event EventHandler ValueChanged;
		#endregion

		#region Internal Methods
		internal void WireParent()
		{
			if( savedParentParent != null )
				savedParentParent.VisibleChanged -= new EventHandler( ParentVisibleChanged );
			savedParentParent = this.Parent != null ? this.Parent.Parent : null;
			if( savedParentParent != null )
				savedParentParent.VisibleChanged += new EventHandler( ParentVisibleChanged );
		}

		#endregion

		#region Private Methods
		private void ParentParentChanged( object sender, EventArgs e )
		{
			WireParent();
		}
		private void ParentVisibleChanged( object sender, EventArgs e )
		{
			if( !IsReflect )
			{
				if( parent.Visible )
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
		private void WireScrollEvent()
		{
			if( scrollBar != null && !wired )
			{
				scrollBar.Scroll += new ScrollEventHandler( OnInnerScroll );
				scrollBar.ValueChanged += new EventHandler( OnInnerValueChanged );
				wired = true;
			}
		}
		private void UnwireScrollEvent()
		{
			if( scrollBar != null && wired )
			{
				scrollBar.Scroll -= new ScrollEventHandler( OnInnerScroll );
				scrollBar.ValueChanged -= new EventHandler( OnInnerValueChanged );
				wired = false;
			}
		}
		private void OnInnerScroll( object sender, ScrollEventArgs se )
		{
			if( se.Type == ScrollEventType.ThumbTrack )
			{
				isThumbTracking = true;
			}
			else
				isThumbTracking = false;


			FetchScrollBar();
			if( Scroll != null )
				Scroll( this, se );

			if( se.Type == ScrollEventType.ThumbPosition )
				OnInnerValueChanged( this, se );
		}
		///	<summary>
		///		<para>Raises the <see	cref="ValueChanged"	/> event.</para>
		///	</summary>
		///	<param name="sender">.</param>
		///	<param name="e">An <see	cref="System.EventArgs"	/> that	contains the event data.</param>
		private void OnInnerValueChanged( object sender, EventArgs e )
		{
			if( ValueChanged != null )
				ValueChanged( this, e );
		}
		#endregion
	}
	#endregion

	#region *** BeginUpdateOptions
	/// <summary>
	/// BeginUpdateOptions details which drawing operations should be performed during a batch of updates.
	/// </summary>
	[Flags]
	public enum BeginUpdateOptions
	{
		/// <summary>
		/// The control suspends any drawing and invalidating and will do a complete refresh when EndUpdate is called.
		/// </summary>
		None = 0,
		/// <summary>
		/// Regions that need to be redrawn afterward should be marked invalid by calling the controls Invalidate method.
		/// </summary>
		Invalidate = 1,
		/// <summary>
		/// ScrollWindow will scroll the window.
		/// </summary>
		ScrollWindow = 2,
		/// <summary>
		/// Scrollbars should be synchronized with the current scroll position.
		/// </summary>
		SynchronizeScrollBars = 4,
		/// <summary>
		/// Allows invalidating regions, scrolling and synchronizes the scrollbar thumb.
		/// </summary>
		InvalidateAndScroll = Invalidate | ScrollWindow | SynchronizeScrollBars
	}
	#endregion

	#region *** ExceptionManager
	/// <summary>
	/// Provides a global hook for exceptions that have been catched inside the framework and gives you
	/// the option to provide specialized handling of the exception. You can also temporarily suspend and resume
	/// catching exceptions.
	/// </summary>
	/// <remarks>
	/// The Syncfusion framework notifies <see cref="ExceptionManager"/> about exceptions that
	/// are catched by calling ExceptionManager.RaiseExceptionCatched or ExceptionManager.
	/// The RaiseExceptionCatched method will raise the ExceptionCatched
	/// event. By handling the ExceptionCatched  event your code can analyze the exception that was catched
	/// and optionally let it bubble up by rethrowing the exception.<para/>
	/// Your code can also temporariliy suspend and resume catching exceptions. This is usefull if you want to provide your
	/// own exception handling. Just call <see cref="SuspendCatchExceptions"/> to disable handling exceptions and <see cref="ResumeCatchExceptions"/>
	/// to resume catching exceptions.<para/>
	/// You also have the options to disable catching exceptions alltogether by setting <see cref="PassThroughExceptions"/> to true.<para/>
	/// Note: All static settings for this class are thread local.
	/// </remarks>
	/// <example>
	/// <code lang="C#">
	/// // The following example demonstrates temporarily suspending exception catching when calling a base class version
	/// // of a method.
	///
	///         protected override void OnMouseDown(MouseEventArgs e)
	///             {
	///             ExceptionManager.SuspendCatchExceptions();
	///
	///             try
	///             {
	///                 base.OnMouseDown(e);
	///                 ExceptionManager.ResumeCatchExceptions();
	///             }
	///             catch (Exception ex)
	///             {
	///                 ExceptionManager.ResumeCatchExceptions();
	///
	///                     // Notify exception manager about the catched exception and
	///                     // give it a chance to optionally rethrow the exception if necessary
	///                     // (e.g. if this OnMouseDown was called from another class that
	///                     // wants to provide its own exception handling).
	///                 if (!ExceptionManager.RaiseExceptionCatched(this, ex))
	///						throw ex;
	///
	///                 // handle exception here
	///                 MessageBox.Show(ex.ToString());
	///             }
	///         }
	/// </code>
	/// </example>
	/// <example>
	/// <code lang="C#">
	/// // This code snippet shows how exceptions are handled within the framework.
	///                 try
	///                 {
	///                     CurrentCell.Refresh();
	///                 }
	///                 catch (Exception ex)
	///                 {
	///                     TraceUtil.TraceExceptionCatched(ex);
	///                     if (!ExceptionManager.RaiseExceptionCatched(this, ex))
	///							throw ex;
	///                 }
	/// </code>
	/// </example>
	public class ExceptionManager
	{
		#region Static Fields
		[ThreadStatic]
		private static bool passThroughExceptions = false;
		[ThreadStatic]
		private static int suspendCatchExceptions = 0;
		[ThreadStatic]
		private static EventHandlerList _ehl = null;
		private static object onExceptionCatchedKey = new object();
		#endregion

		#region Public Static Methods
		/// <summary>
		/// Temporariliy suspend and resume catching exceptions. Call <see cref="SuspendCatchExceptions"/> to disable handling exceptions and <see cref="ResumeCatchExceptions"/>
		/// to resume catching exceptions.
		/// </summary>
		public static void SuspendCatchExceptions()
		{
			suspendCatchExceptions++;
		}
		/// <summary>
		/// Temporariliy suspend and resume catching exceptions. Call <see cref="SuspendCatchExceptions"/> to disable handling exceptions and <see cref="ResumeCatchExceptions"/>
		/// to resume catching exceptions.
		/// </summary>
		public static void ResumeCatchExceptions()
		{
			if( suspendCatchExceptions > 0 )
				suspendCatchExceptions--;
		}
		/// <summary>
		/// Determines if exceptions should be catched or if they should bubble up.
		/// calls this method.
		/// </summary>
		public static bool ShouldCatchExceptions()
		{
			return suspendCatchExceptions == 0 && !passThroughExceptions;
		}
		#endregion

		#region Static Properties
		/// <summary>
		/// Lets you disable catching exceptions alltogether by setting <see cref="PassThroughExceptions"/> to true.<para/>
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public static bool PassThroughExceptions
		{
			get
			{
				return passThroughExceptions;
			}
			set
			{
				passThroughExceptions = value;
			}
		}
		#endregion

		#region Private Static Properties
		private static EventHandlerList ehl
		{
			get
			{
				if( _ehl == null )
					_ehl = new EventHandlerList();
				return _ehl;
			}
		}
		#endregion
	}
	#endregion

	#region *** ScrollControl
	/// <summary>
	/// Defines a base class for custom controls that support scrolling behavior.
	/// </summary>
	/// <remarks>
	/// The ScrollControl class acts as a base class for controls that require the
	/// ability to scroll. To allow a control to display scroll bars as needed,
	/// set the AutoScroll property to true. To select which scroll bars should be visible,
	/// set the VScroll and HScroll properties. <para/>
	/// You can also associate stand-alone scrollbars with the ScrollControl. The VertScrollBar
	/// and HorizScrollBar properties allow you to associate external scrollbars. This is of benefit
	/// if you want to share one scrollbar with a parent control. For example, if the ScrollControl
	/// is a view inside a workbook or dynamic splitter frame.<para/>
	/// ScrollControl supports automatic scrolling when the user drags the mouse. In a grid
	/// when the user starts selecting cells the user can drag the mouse outside the grid
	/// area and the grid will automatically scroll. To enable auto scrolling, override the
	/// OnMouseDown event in your derived control and initialize the AutoScrolling, AutoScrollBounds
	/// and InsideScrollBounds properties.<para/>
	/// When the user scrolls your control and holds down the mouse on the down or up arrow
	/// of the scrollbar, the scrolling speed will accelerate.<para/>
	/// The FixRenderOrigin method will ensure correct initialization of the rendering origin
	/// for brushes and patterns. You can call FixRenderOrigin from your control's OnPaint method.
	/// ScrollControl supports scrolling with the mouse wheel and also cooperates fine with
	/// with the IntelliMouseDragScroll class.<para/>
	/// If you want to provide context information about your control and change the cursor on
	/// the fly while the user moves the mouse, set the OverrideCursor property to the cursor
	/// you want to show. Instead you can also implement IMouseController and add the object
	/// to MouseControllerDispatcher.<para/>
	/// </remarks>
	[ToolboxItem( false )]
	public class ScrollControl
		: Control
	{
		#region Static Fields
		static bool discardPaintMessagesAfterBeginUpdate = false;
		#endregion

		#region Fields
		private static object elapsedSemaphor = true;
		private static object startstopSemaphor = true;
		private ScrollBarWrapper hScrollBar;
		private ScrollBarWrapper vScrollBar;
		private bool hScroll = false;
		private bool vScroll = false;
		private Rectangle autoScrollBounds = Rectangle.Empty;
		private bool disableScrollWindow = false;
		private int updateCount;
		private bool paintPending;
		private bool lockScrollBars = false;
		private BeginUpdateOptions updateOptions;
		private Point renderOriginPoint = Point.Empty; // see Graphics.RenderingOrigin
		private bool useSharedScrollBars = false;
		private bool supportsThumbTrack = false;
		private BorderStyle borderStyle = BorderStyle.None;
		private Size insideScrollMargins = new Size( 10, 10 );
		private bool ignoreUICues = false;
		private Point mousePosition;
		private Form wiredParentForm;
		private IntPtr cachedRgn = IntPtr.Zero;
		// Focus
		private bool isActiveControl = false;
		private bool isValidating = false;
		private bool isDeactivatedCalled = false;
		private bool hasControlFocus = false;
		private bool isValidated = false;
		#endregion

		#region Static Properties
		/// <summary>
		/// When you call BeginUpdate() the control by default does not handle WM_PAINT messages. Only
		/// once you call EndUpdate they will be processed. If this causes problems in your application, you can
		/// set this static property to true. In such cases WM_PAINT messages will be simply discarded and
		/// any invalid regions will be validated.
		/// </summary>
		/// <remarks>
		/// There is a problem with the default implementation of BeginUpdate. If a screen region is marked
		/// invalid the WndProc will be repeatedly called with WM_PAINT at the the top of the WndProc
		/// until EndUpdate is called. This can cause your application to freeze if another window gets created
		/// or if you make a web service call and WndProc messages need to be processed.<para/>
		/// Setting DiscardPaintMessagesAfterBeginUpdate = true will help avoid these scenarios.
		/// </remarks>
		public static bool DiscardPaintMessagesAfterBeginUpdate
		{
			get
			{
				// see also Syncfusion Technical Support Incident 13493
				return discardPaintMessagesAfterBeginUpdate;
			}
			set
			{
				discardPaintMessagesAfterBeginUpdate = value;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Property MousePosition (Point) - cached Control.MousePosition. The variable is set
		/// before any WM_MOUSE* messages are processed.
		/// </summary>
		[DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Hidden )]
		[Browsable( false )]
		public Point LastMousePosition
		{
			get
			{
				return this.mousePosition;
			}
			set
			{
				this.mousePosition = value;
			}
		}
		/// <summary>
		/// <para>Gets or sets the border style of the control.</para>
		/// </summary>
		[DefaultValue( BorderStyle.None )]
		[DispId( -504 /*0xFFFFFE08*/)]
		[Category( "Appearance" )]
		[Description( @"The border style of the control." )]
		public BorderStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				if( this.borderStyle != value )
				{
					if( !Enum.IsDefined( typeof( System.Windows.Forms.BorderStyle ), value ) )
						throw new InvalidEnumArgumentException( "value", ( ( int )( value ) ), typeof( BorderStyle ) );
					this.borderStyle = value;
					UpdateStyles();
				}
			}
		}
		/// <summary>
		/// <para>Gets or sets a value indicating whether the vertical scroll bar is visible.
		/// </para>
		/// </summary>
		/// <value>
		/// <para><see langword="true"/> if the vertical scroll bar is visible; otherwise, <see langword="false"/>.</para>
		/// </value>
		/// <seealso cref="ScrollableControl.HScroll"/>
		[DefaultValue( false )]
		[Browsable( false )]
		[Category( "Scrolling" )]
		[RefreshProperties( RefreshProperties.All )]
		public bool HScroll
		{
			get
			{
				return hScroll;
			}
			set
			{
				if( hScroll != value )
				{
					SetVisibleScrollbars( value, vScroll );
				}
			}
		}
		/// <summary>
		/// <para>Gets or sets a value indicating whether the vertical scroll bar is visible.</para>
		/// </summary>
		/// <value>
		/// <para><see langword="true"/> if the vertical scroll bar is visible; otherwise, <see langword="false"/>.</para>
		/// </value>
		/// <seealso cref="ScrollableControl.HScroll"/>
		[DefaultValue( false )]
		[RefreshProperties( RefreshProperties.All )]
		[Category( "Scrolling" )]
		[Browsable( false )]
		public bool VScroll
		{
			get
			{
				return vScroll;
			}
			set
			{
				if( vScroll != value )
				{
					SetVisibleScrollbars( hScroll, value );
				}
			}
		}
		/// <summary>
		/// ScrollControlMouseController checks this to see if it should cancel
		/// existing mouse operation and call ScrollControlMouseController.CancelMode
		/// when a UICuesChanged event is sent. That can happen when user activates
		/// another application or simply when styles for a child window have changed.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IgnoreUICues
		{
			get
			{
				return ignoreUICues;
			}
			set
			{
				ignoreUICues = value;
			}
		}
		/// <summary>
		/// Enable shared scrollbars. Use this if the control is not embedded in a container control
		/// that implements IScrollBarFrame and you want to provide your own scrollbars.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool UseSharedScrollBars
		{
			get
			{
				return useSharedScrollBars;
			}
			set
			{
				useSharedScrollBars = value;
			}
		}
		/// <summary>
		/// The default margins for the scrolling area when the user moves the mouse to the
		/// margin between InsideScrollBounds and AutoScrollBounds.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Size InsideScrollMargins
		{
			get
			{
				return insideScrollMargins;
			}
			set
			{
				insideScrollMargins = value;
			}
		}
		/// <summary>
		/// Returns a reference to an object with vertical scrollbar settings of the control.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ScrollBarWrapper VScrollBar
		{
			get
			{
				return vScrollBar;
			}
		}
		/// <summary>
		/// Returns a reference to an object with horizontal scrollbar settings of the control.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ScrollBarWrapper HScrollBar
		{
			get
			{
				return hScrollBar;
			}
		}
		/// <summary>
		/// Size is overriden here to prevent Code Generation in Designer.
		/// </summary>
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}
		/// <summary>
		/// TabIndex is overriden here to prevent Code Generation in Designer.
		/// </summary>
		public new int TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex = value;
			}
		}
		/// <summary>
		/// Returns the settings for the current BeginUpdate option.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public virtual BeginUpdateOptions UpdateOptions
		{
			get
			{
				return updateOptions;
			}
		}
		/// <summary>
		/// Determines if BeginUpdate() has been called and the painting for a control is suspended.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public bool Updating
		{
			get
			{
				return updateCount > 0;
			}
		}
		/// <summary>
		/// Determines if there are updates pending for the control when painting is suspended with BeginUpdate.
		/// </summary>
		[Browsable( false )]
		[EditorBrowsable( EditorBrowsableState.Advanced )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool PaintPending
		{
			get
			{
				// see also WndProc (WM_PAINT)
				return paintPending;
			}
		}
		/// <summary>
		/// Lets you check or specify the setting if the window should be scrolled when ScrollWindow is called.
		/// </summary>
		/// <remarks>
		/// <para>If DisableScrollWindow is true any calls to the ScrollWindow method will simply invalidate the affect region. The rendering origin will
		/// still be recorded correctly and WindowScrolling and WindowScrolled events will be raised.</para>
		/// <para>If DisableScrollWindow is false ScrollWindow will scroll the contents of the control.
		/// </para>
		/// <para>If DisableScrollWindow will return true if BeginUpdate was called without the BeginUpdateOptions.ScrollWindow option.
		/// </para>
		/// </remarks>
		/// <seealso cref="T:ScrollControl.BeginUpdate"/>
		/// <seealso cref="T:ScrollControl.Updating"/>
		/// <seealso cref="T:ScrollControl.ScrollWindow"/>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool DisableScrollWindow
		{
			get
			{
				return disableScrollWindow || Updating && ( updateOptions & BeginUpdateOptions.ScrollWindow ) == BeginUpdateOptions.None
					|| GetStyle( ControlStyles.SupportsTransparentBackColor )
					|| this.BackgroundImage != null;
			}
			set
			{
				disableScrollWindow = value;
			}
		}
		/// <summary>
		/// Specifies if the control should scroll while the user is dragging a scrollbars thumb
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Obsolete( "Use VerticalThumbTrack and HorizontalThumbTrack properties instead." )]
		public bool SupportsThumbTrack
		{
			get
			{
				return supportsThumbTrack;
			}
			set
			{
				if( supportsThumbTrack != value )
				{
					supportsThumbTrack = value;
					HScrollBar.SupportsThumbTrack = value;
					VScrollBar.SupportsThumbTrack = value;
				}
			}
		}
		/// <summary>
		/// Specifies if the control should scroll while the user is dragging a vertical scrollbars thumb.
		/// </summary>
		[Browsable( true )]
		[Category( "Scrolling" )]
		[Description( "Specifies if the control should scroll while the user is dragging a vertical scrollbars thumb." )]
		[DefaultValue( false )]
		public virtual bool VerticalThumbTrack
		{
			get
			{
				return VScrollBar.SupportsThumbTrack;
			}
			set
			{
				VScrollBar.SupportsThumbTrack = value;
			}
		}
		/// <summary>
		/// Specifies if the control should scroll while the user is dragging a horizontal scrollbars thumb.
		/// </summary>
		[Browsable( true )]
		[Category( "Scrolling" )]
		[Description( "Specifies if the control should scroll while the user is dragging a horizontal scrollbars thumb." )]
		[DefaultValue( false )]
		public virtual bool HorizontalThumbTrack
		{
			get
			{
				return HScrollBar.SupportsThumbTrack;
			}
			set
			{
				HScrollBar.SupportsThumbTrack = value;
			}
		}
		/// <summary>
		/// Specifies if the control should show scroll tips while the user is dragging a vertical scrollbars thumb.
		/// </summary>
		[Browsable( true )]
		[Category( "Scrolling" )]
		[Description( "Specifies if the control should show scroll tips while the user is dragging a vertical scrollbars thumb." )]
		[DefaultValue( false )]
		public virtual bool VerticalScrollTips
		{
			get
			{
				return VScrollBar.SupportsScrollTips;
			}
			set
			{
				VScrollBar.SupportsScrollTips = value;
			}
		}
		/// <summary>
		/// Specifies if the control should show scroll tips while the user is dragging a horizontal scrollbars thumb.
		/// </summary>
		[Browsable( true )]
		[Category( "Scrolling" )]
		[Description( "Specifies if the control should show scroll tips while the user is dragging a horizontal scrollbars thumb." )]
		[DefaultValue( false )]
		public virtual bool HorizontalScrollTips
		{
			get
			{
				return HScrollBar.SupportsScrollTips;
			}
			set
			{
				HScrollBar.SupportsScrollTips = value;
			}
		}
		/// <summary>
		/// Indicates if the <see cref="OnValidating"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> reset this flag.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IsValidating
		{
			get
			{
				return isValidating;
			}
		}
		/// <summary>
		/// Indicates if the <see cref="OnValidated"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> reset this flag.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IsValidated
		{
			get
			{
				return isValidated;
			}
		}
		/// <summary>
		/// Indicates if the <see cref="OnEnter"/> has been called. <see cref="OnLeave"/> resets this flag.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IsActiveControl
		{
			get
			{
				return isActiveControl;
			}
		}
		/// <summary>
		/// Indicates if both <see cref="OnDeactivated"/> has been called. <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool IsDeactivated
		{
			get
			{
				return isDeactivatedCalled;
			}
		}
		/// <summary>
		/// Indicates if both <see cref="OnControlGotFocus"/> has been called. <see cref="OnControlLostFocus"/> resets this flag.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool HasControlFocus
		{
			get
			{
				return hasControlFocus;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes a new instance of <see cref="ScrollControl"/>
		/// </summary>
		public ScrollControl()
		{
			hScrollBar = new ScrollBarWrapper( this, ScrollBars.Horizontal );
			vScrollBar = new ScrollBarWrapper( this, ScrollBars.Vertical );
			WireScrollEvents();
		}
		/// <summary>
		/// Releases the unmanaged resources used by the Control and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( wiredParentForm != null )
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				wiredParentForm = null;

				UnwireScrollEvents();

				if( cachedRgn != IntPtr.Zero )
				{
					GDIAppi.DeleteObject( cachedRgn );
					this.cachedRgn = IntPtr.Zero;
				}

				hScrollBar.Dispose();
				vScrollBar.Dispose();

			}
			base.Dispose( disposing );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Returns PointToClient(LastMousePosition).
		/// </summary>
		/// <returns>PointToClient(LastMousePosition).</returns>
		public Point LastMousePositionToClient()
		{
			return PointToClient( this.mousePosition );
		}
		/// <summary>
		/// Call this method if you want to delegate MouseWheelEvent from a
		/// child control.
		/// </summary>
		/// <param name="e">A MouseEventArgs that holds event data.</param>
		/// <remarks>
		/// <code>
		/// public class GridTextBox: RichTextBox
		/// {
		///		private GridTextBoxCell parent;
		///		protected override void OnMouseWheel(MouseEventArgs e)
		///		{
		///			parent.Grid.ProcessMouseWheel(e);
		///		}
		///	}
		///	</code>
		/// </remarks>
		public void ProcessMouseWheel( MouseEventArgs e )
		{
			OnMouseWheel( e );
		}
		/// <summary>
		/// Pane information.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public virtual string PaneDesc
		{
			get
			{
				string _paneDesc = Parent != null ? " " + Parent.Name : "";
				if( this.Name.Length > 0 )
					_paneDesc = this.Name;
				else
					_paneDesc = this.Text;

				_paneDesc += " ";

				if( this.IsActiveControl )
					_paneDesc += "Act";

				if( this.IsValidating )
					_paneDesc += "Val";

				if( this.IsValidated )
					_paneDesc += "Vok";

				if( this.HasControlFocus )
					_paneDesc += "Foh";

				if( this.QueryFocusInside() )
					_paneDesc += "Foi";

				if( this.CanFocus )
					_paneDesc += "Cfo";

				return _paneDesc;
			}
		}
		/// <summary>
		/// Update scrollbars to reflect recent changes in scroll position, minimum and maximum scroll position values.
		/// </summary>
		public virtual void UpdateScrollBars()
		{
			SyncReflectScrollBars();
			HScrollBar.InitScrollBar();
			VScrollBar.InitScrollBar();
		}
		/// <summary>
		/// Call this method to check if you should do any update calculations for the view in your control and to notify scrollcontrol
		/// that the controls content need to be updated.
		/// </summary>
		/// <returns>true if you should invalidate areas that need to be redrawn; false if a complete Refresh for the control is pending
		/// and therefore invalidating the view is not necessary.</returns>
		public bool ShouldPrepareUpdate()
		{
			return ShouldPrepareUpdate( false );
		}
		/// <summary>
		/// Call this method to check if you should do any update calculations for the view in your control and to notify scrollcontrol
		/// that the controls content need to be updated.
		/// </summary>
		/// <param name="markPaintPending">If markPaintPending is true ScrollControl will assume the control needs to be repainted in a subsequent EndUpdate call.</param>
		/// <returns>true if you should Invalidate regions to be repainted in your control. It will return false if a complete Refresh of
		/// the control is pending and you don't need to invalidate individual regions of your control.</returns>
		public bool ShouldPrepareUpdate( bool markPaintPending )
		{
			if( markPaintPending )
			{
				paintPending |= markPaintPending;
			}
			return updateCount == 0 || ( updateOptions & BeginUpdateOptions.Invalidate ) != BeginUpdateOptions.None;
		}
		/// <summary>
		/// Suspends the painting of the control until the <see cref="T:EndUpdate"/> method is called.
		/// </summary>
		/// <remarks>
		/// When many paint are made to the appearance of a control you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user, and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.
		/// </remarks>
		public void BeginUpdate()
		{
			BeginUpdate( BeginUpdateOptions.None );
		}
		/// <summary>
		/// Suspends the painting of the control until the <see cref="T:EndUpdate"/> method is called.
		/// </summary>
		/// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
		/// <remarks>
		/// <para>When many paint are made to the appearance of a control you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user, and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.</para>
		/// <para>
		/// Pass BeginUpdateOptions if you do not want to do a complete Refresh of the control and instead
		/// want to have certain regions of your control be invalidated or scroll the contents of control.</para>
		/// If you call BeginUpdate() and then later EndUpdate() the control will know if a paint is pending and only
		/// refresh the control if a paint is pending. Either call to ShouldPrepareUpdate, Invalidate or a WM_PAINT message during
		/// the BeginUpdate EndUpdate block will signal the control that a paint is pending.
		/// </remarks>
		/// <seealso cref="T:ScrollControl.ShouldPrepareUpdate"/>
		/// <seealso cref="T:ScrollControl.EndUpdate"/>
		public virtual void BeginUpdate( BeginUpdateOptions options )
		{
			// see also WndProc (WM_PAINT)

			// Since we can't invalidate then we should also wait with updating scrollbars
			if( ( options & BeginUpdateOptions.Invalidate ) == BeginUpdateOptions.None )
				options = BeginUpdateOptions.None;

			bool lockScrollBars = ( options & BeginUpdateOptions.SynchronizeScrollBars ) == BeginUpdateOptions.None;

			if( lockScrollBars && !this.lockScrollBars )
			{
				this.lockScrollBars = lockScrollBars;
				OnBeginUpdateScrollBars();
			}

			if( this.updateCount++ == 0 )
			{
				this.updateOptions = options;
				this.paintPending = false;
				OnUpdatingChanged( EventArgs.Empty );
			}
			else
				this.updateOptions = this.updateOptions & options;
		}
		/// <summary>
		/// Resumes the painting of the control suspended by calling the BeginUpdate method.
		/// </summary>
		/// <remarks>
		/// When many paint are made to the appearance of a control you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user, and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.
		/// </remarks>
		/// <seealso cref="T:ScrollControl.BeginUpdate"/>
		public void EndUpdate()
		{
			EndUpdate( true );
		}
		/// <summary>
		/// Cancels any prior <see cref="T:BeginUpdate"/> calls.
		/// </summary>
		/// <seealso cref="T:ScrollControl.BeginUpdate"/>
		public void CancelUpdate()
		{
			while( this.updateCount > 0 )
				EndUpdate();
		}
		/// <summary>
		/// Resumes the painting of the control suspended by calling the BeginUpdate method.
		/// </summary>
		/// <param name="update">Updated when set to true.</param>
		/// <remarks>
		/// When many paint are made to the appearance of a control you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user, and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.
		/// </remarks>
		public virtual void EndUpdate( bool update )
		{
			if( this.updateCount > 0 )
			{
				this.updateCount--;
				if( this.updateCount == 0 )
				{
					if( this.lockScrollBars )
					{
						this.lockScrollBars = false;
						OnEndUpdateScrollBars();
					}
					OnUpdatingChanged( EventArgs.Empty );
					if( paintPending && update )
					{
						if( ( updateOptions & BeginUpdateOptions.Invalidate ) != BeginUpdateOptions.None )
							Update();
						else
							Refresh();
					}
				}
			}
		}
		/// <summary>
		/// Scrolls the contents of the control similar to the ScrollWindow Windows API.
		/// </summary>
		/// <remarks>
		/// The method will raise a WindowScrolling event before the contents are scrolled and a WindowScrolled event after
		/// the contents have been scrolled.
		/// <para>If DisableScrollWindow is true any calls to the ScrollWindow method will simply invalidate the affect region. The rendering origin will
		/// still be recorded correctly and WindowScrolling and WindowScrolled events will be raised.</para>
		/// <para>If DisableScrollWindow is false ScrollWindow will scroll the contents of the control.
		/// </para>
		/// </remarks>
		/// <param name="xAmount">Amount to scroll by x.</param>
		/// <param name="yAmount">Amount to scroll by y.</param>
		/// <param name="rect">Rectangle to scroll.</param>
		/// <param name="clipRect">Clip.</param>
		/// <returns>Updated rectangle.</returns>
		[EditorBrowsable( EditorBrowsableState.Advanced )]
		public Rectangle ScrollWindow( int xAmount, int yAmount, Rectangle rect, Rectangle clipRect )
		{
			// Note: there might be a problem when changing column widths in a grid and
			// using ScrollWindow because renderOriginPoint should not be changed then.
			renderOriginPoint.X += xAmount;
			renderOriginPoint.Y += yAmount;

			if( DisableScrollWindow )
			{
				if( !rect.IsEmpty )
					clipRect = Rectangle.Union( rect, clipRect );
				Invalidate( clipRect );
			}
			else
			{
				RECT lpRect = ( RECT )rect;
				RECT lpClipRect = ( RECT )clipRect;
				RECT rcUpdate = new RECT();
				int flags = 3;

				if( rect.IsEmpty )
					ScrollApi.ScrollWindowEx( Handle, xAmount, yAmount, ( COMRECT )null, ref lpClipRect, IntPtr.Zero, ref rcUpdate, flags );
				else
					ScrollApi.ScrollWindowEx( Handle, xAmount, yAmount, ref lpRect, ref lpClipRect, IntPtr.Zero, ref rcUpdate, flags );
			}

			Rectangle updateRect = clipRect;
			if( xAmount < 0 )
			{
				updateRect.Width = -xAmount;
				updateRect.X += clipRect.Width + xAmount;
			}
			else if( xAmount > 0 )
			{
				updateRect.Width = xAmount;
			}

			if( yAmount < 0 )
			{
				updateRect.Height = -yAmount;
				updateRect.Y += clipRect.Height + yAmount;
			}
			else if( yAmount > 0 )
			{
				updateRect.Height = yAmount;
			}

			return updateRect;
		}
		/// <summary>
		/// Determines if this control contains focus. Override this method if you
		/// want to show dropdown windows and indicate the control has not lost focus when
		/// the dropdown is shown.
		/// </summary>
		/// <returns>true if the control or any child control has focus; false otherwise.</returns>
		public virtual bool QueryFocusInside()
		{
			return ContainsFocus;
		}
		#endregion

		#region PrePaintJitting
		bool onPaintCalled = false;
		bool preJitPaint = false;
		/// <summary>
		/// Lets you specify if the time the first time the control is drawn should be optimized
		/// by calling OnPaint before the control is made visible and so that all relevant code for drawing
		/// has been jitted.
		/// </summary>
		protected bool PreJitPaint
		{
			get
			{
				return preJitPaint;
			}
			set
			{
				preJitPaint = value;
			}
		}
		/// <summary>
		/// Performs painting-related operations.
		/// </summary>
		/// <param name="e">PaintEventArgs.</param>
		protected override void OnPaint( PaintEventArgs e )
		{
			onPaintCalled = true;
			base.OnPaint( e );
		}
		/// <summary>
		/// Performs handle creating-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );
			EnsurePaintCodeJitted( false );

			Form f = FindForm();
			if( f != null )
			{
				if( wiredParentForm != null )
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				wiredParentForm = f;
				if( wiredParentForm != null )
					wiredParentForm.Enter += new EventHandler( wiredParentForm_Enter );
			}
		}
		/// <summary>
		/// Performs visibility changing-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			if( this.Parent == null )
				return;

			EnsurePaintCodeJitted( false );
			if( Visible )
				SyncReflectScrollBars();

		}
		/// <summary>
		/// This method is called to reduce the time the first time the control is drawn. Calling
		/// OnPaint before the control is made visible ensures that all relevant code for drawing
		/// has been jitted.
		/// </summary>
		protected virtual void OnEnsurePaintCodeJitted()
		{
			return;
		}
		/// <summary>
		/// This method checks if the control is visible, a window handle has been created
		/// and if it has not been drawn before, it calls <see cref="OnEnsurePaintCodeJitted"/>.
		/// </summary>
		/// <param name="ignoreVisible">Set this true if you want to force a call to
		/// <see cref="OnEnsurePaintCodeJitted"/> even if the control is not visible and/or
		/// no window handle has been created.</param>
		public void EnsurePaintCodeJitted( bool ignoreVisible )
		{
			if( ( ignoreVisible || this.Visible && this.IsHandleCreated ) && !onPaintCalled )
			{
				OnEnsurePaintCodeJitted();
				onPaintCalled = true;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when <see cref="T:ScrollControl.BeginUpdate"/> has been called the first time or <see cref="T:ScrollControl.EndUpdate"/>
		/// has been called the last time.
		/// </summary>
		[Description( " Occurs when BeginUpdate has been called the first time or EndUpdate has been called the last time." )]
		[Category( "Behavior" )]
		[Browsable( false )]
		public event EventHandler UpdatingChanged;
		/// <summary>
		/// Raised when horizontal scrolling is being performed.
		/// </summary>
		public event ScrollEventHandler HorizontalScroll;
		/// <summary>
		/// Raised when vertical scrolling is being performed.
		/// </summary>
		public event ScrollEventHandler VerticalScroll;
		/// <summary>
		/// Occurs when both <see cref="OnControlLostFocus"/> and <see cref="OnLeave"/> occured.
		/// </summary>
		public event EventHandler Deactivated;
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method fires the ScrollbarsVisibleChanged
		/// </summary>
		/// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
		protected virtual void OnScrollbarsVisibleChanged( EventArgs e )
		{
		}
		/// <summary>
		/// Called when horizontal scrolling is being performed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="se">ScrollEventArgs.</param>
		protected virtual void OnHScroll( object sender, ScrollEventArgs se )
		{
		}
		/// <summary>
		/// Called when vertical scrolling is being performed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="se">ScrollEventArgs.</param>
		protected virtual void OnVScroll( object sender, ScrollEventArgs se )
		{
		}
		/// <summary>
		/// Call this method from your controls OnPaint method to ensure correct rendering origin for brushes and patterns.
		/// </summary>
		/// <param name="g">The graphics object.</param>
		protected void FixRenderOrigin( Graphics g )
		{
		}
		/// <summary>
		/// Call <see cref="ScrollBarWrapper.BeginUpdate"/> for both scrollbars.
		/// </summary>
		protected virtual void OnBeginUpdateScrollBars()
		{
			this.VScrollBar.BeginUpdate();
			this.HScrollBar.BeginUpdate();
		}
		/// <summary>
		/// Call <see cref="ScrollBarWrapper.EndUpdate"/> for both scrollbars.
		/// </summary>
		protected virtual void OnEndUpdateScrollBars()
		{
			this.VScrollBar.EndUpdate();
			this.HScrollBar.EndUpdate();
		}
		/// <summary>
		/// Raises the <see cref="ScrollControl.UpdatingChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		/// <remarks>
		/// The event handler for this event can check <see cref="ScrollControl.Updating"/>
		/// to determine if <see cref="T:ScrollControl.BeginUpdate"/> or <see cref="T:ScrollControl.EndUpdate"/>
		/// was called.
		/// </remarks>
		protected virtual void OnUpdatingChanged( EventArgs e )
		{
			if( UpdatingChanged != null )
				UpdatingChanged( this, e );
		}
		/// <summary>
		/// Raises the <see cref="Deactivated"/> event.
		/// </summary>
		/// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
		protected virtual void OnDeactivated( EventArgs e )
		{
			isDeactivatedCalled = true;
			if( Deactivated != null )
				Deactivated( this, e );
		}
		/// <summary>
		/// Raises the <see cref="Control.GotFocus"/> event. This method is called when the control
		/// or any child control got focus and this control did not have focus before.
		/// </summary>
		/// <remarks>
		/// Inheriting classed should override this method instead of overriding <see cref="Control.OnGotFocus"/>
		/// because <see cref="OnControlGotFocus"/> is also called when child controls got focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlGotFocus()
		{
			if( this.Disposing || this.IsDisposed )
				return;

			base.OnGotFocus( EventArgs.Empty );
		}
		/// <summary>
		/// Raises the <see cref="Control.LostFocus"/> event. This method is called when the control
		/// or any child control looses focus and the new focused control is not a child of this control.
		/// </summary>
		/// <remarks>
		/// Inheriting classed should override this method instead of overriding <see cref="Control.OnLostFocus"/>
		/// because <see cref="OnControlLostFocus"/> is also called when child controls loose focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlLostFocus()
		{
			if( this.Disposing || this.IsDisposed )
				return;

			base.OnLostFocus( EventArgs.Empty );

			if( !isActiveControl && !isValidating && !( CausesValidation && !this.isValidated ) )
				OnDeactivated( EventArgs.Empty );
			else
			{
				CancelUpdate();

				if( isValidating )
					OnValidatingLostFocus();
			}
		}
		/// <summary>
		/// This method is called if the controls <see cref="OnControlLostFocus"/> notification occurs
		/// while handling a <see cref="Control.Validating"/> event. This typically occurs if a
		/// message box is displayed from a <see cref="Control.Validating"/> event handler.
		/// </summary>
		protected virtual void OnValidatingLostFocus()
		{
			// Sometimes when users display a message box while the control is validated,
			// the message box is shown behind the application window and the users has
			// the impression the application is locked up.
			//
			// Pressing the <ALT> key will make the message box appear correctly in front of
			// the window.
			//
			// The following line emulates pressing the <ALT> key.
			//			SendKeys.Send("%");
		}
		#endregion

		#region Internal Static Methods
		internal static int HIWORD( int n )
		{
			return ( ( n >> 16 ) & 0xffff );
		}
		#endregion

		#region Internal Methods
		internal IntPtr SendMessage( int msg, IntPtr wparam, IntPtr lparam )
		{
			return WinAPI.SendMessage( this.Handle, msg, wparam, lparam );
		}
		internal IntPtr SendMessage( int msg, IntPtr wparam, int lparam )
		{
			return WinAPI.SendMessage( this.Handle, msg, wparam, ( IntPtr )lparam );
		}
		internal IntPtr SendMessage( int msg, int wparam, IntPtr lparam )
		{
			return WinAPI.SendMessage( this.Handle, msg, ( IntPtr )wparam, lparam );
		}
		internal IntPtr SendMessage( int msg, int wparam, int lparam )
		{
			return WinAPI.SendMessage( this.Handle, msg, ( IntPtr )wparam, ( IntPtr )lparam );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Performs parent changing-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnParentChanged( EventArgs e )
		{
			Form f = FindForm();
			if( f != this.wiredParentForm )
			{
				if( wiredParentForm != null )
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				wiredParentForm = f;
				if( wiredParentForm != null )
					wiredParentForm.Enter += new EventHandler( wiredParentForm_Enter );
			}

			hScrollBar.WireParent();
			vScrollBar.WireParent();
			base.OnParentChanged( e );
		}
		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="msg">The Windows Message to process.</param>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode = true )]
		protected override void WndProc( ref Message msg )
		{
			switch( msg.Msg )
			{
				case ( int )Msg.WM_HSCROLL:
					this.WmHScroll( ref msg );
					break;
				case ( int )Msg.WM_VSCROLL:
					msg.Result = ( IntPtr )1;
					this.WmVScroll( ref msg );
					break;
				case ( int )Msg.WM_KILLFOCUS:
					if( updateCount > 0 )
					{
						while( updateCount > 1 )
							EndUpdate();
					}
					goto default;

				default:
					base.WndProc( ref msg );
					break;
			}
		}
		/// <summary>
		/// Overriden. Changes <see cref="System.Windows.Forms.CreateParams.Style"/> to show or hide scrollbars and also consider the controls
		/// <see cref="ScrollControl.BorderStyle"/> setting.
		/// </summary>
		protected override CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode = true )]
			get
			{
				System.Windows.Forms.CreateParams cp = base.CreateParams;

				if( this.HScroll )
					cp.Style = ( cp.Style | 1048576 );
				else
					cp.Style = ( cp.Style & ~1048576 );

				if( this.VScroll )
					cp.Style = ( cp.Style | 2097152 );
				else
					cp.Style = ( cp.Style & ~2097152 );

				if( this.RightToLeft == RightToLeft.Yes )
					cp.ExStyle = ( cp.ExStyle | 8192 );
				else
					cp.ExStyle = ( cp.ExStyle & ~8192 );

				switch( this.borderStyle )
				{
					case BorderStyle.Fixed3D:
						cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
						break;
					case BorderStyle.FixedSingle:
						cp.Style |= 0x800000; // WS_BORDER
						break;
				}
				return cp;
			}
		}
		/// <summary>
		/// Overriden. See <see cref="System.Windows.Forms.Control.Invalidated" /> event.
		/// </summary>
		/// <param name="e">An <see cref="System.Windows.Forms.InvalidateEventArgs" /> that contains the event data.</param>
		protected override void OnInvalidated( InvalidateEventArgs e )
		{
			base.OnInvalidated( e );
			if( updateCount > 0 )
				paintPending = true;
		}
		/// <summary>
		/// Performs mouse entering-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnEnter( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			isActiveControl = true;
			isValidating = false;
			isValidated = false;
			base.OnEnter( e );
		}
		/// <summary>
		/// Performs mouse leaving-related operations.
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected override void OnLeave( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			isActiveControl = false;
			isValidating = false;
			base.OnLeave( e );

			if( ( !this.CausesValidation || this.IsValidated ) && !hasControlFocus )
				OnDeactivated( e );
		}
		/// <summary>
		/// Performs validating-related operations.
		/// </summary>
		/// <param name="e">CancelEventArgs.</param>
		protected override void OnValidating( CancelEventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			isValidating = true;
			base.OnValidating( e );
		}
		/// <summary>
		/// Performs validating-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnValidated( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			isValidating = false;
			isValidated = true;
			base.OnValidated( e );

			if( !isActiveControl && !hasControlFocus )
				OnDeactivated( e );
		}
		/// <summary>
		/// Performs focus loosing-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnLostFocus( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			RaiseControlLostFocus();
		}
		/// <summary>
		/// OnGotFocus.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnGotFocus( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			RaiseControlGotFocus();
		}
		/// <summary>
		/// Performs control removing-related operations.
		/// </summary>
		/// <param name="e">ControlEventArgs.</param>
		protected override void OnControlRemoved( System.Windows.Forms.ControlEventArgs e )
		{
			base.OnControlRemoved( e );

			e.Control.GotFocus -= new EventHandler( ChildGotFocus );
			e.Control.LostFocus -= new EventHandler( ChildLostFocus );
		}
		/// <summary>
		/// Performs control adding-related operations.
		/// </summary>
		/// <param name="e">ControlEventArgs.</param>
		protected override void OnControlAdded( System.Windows.Forms.ControlEventArgs e )
		{
			base.OnControlAdded( e );

			e.Control.GotFocus += new EventHandler( ChildGotFocus );
			e.Control.LostFocus += new EventHandler( ChildLostFocus );
		}
		/// <summary>
		/// Performs size changing-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnSizeChanged( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			base.OnSizeChanged( e );
		}
		/// <summary>
		/// Performs location changing-related operations.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnLocationChanged( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
				return;

			base.OnLocationChanged( e );
		}
		#endregion

		#region Event Handlers
		private void wiredParentForm_Enter( object sender, EventArgs e )
		{
			if( !this.IsActiveControl )
				OnEnter( e );
		}
		/// <summary>
		///    <para>
		///       Listens
		///       for the horizontal scrollbar's scroll event.
		///    </para>
		/// </summary>
		/// <param name="sender">
		///    An <see cref="System.Object"/> that contains data about the control.
		/// </param>
		/// <param name="se">
		///    A <see cref="System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
		/// </param>
		protected virtual void OnHScrollInternal( object sender, ScrollEventArgs se )
		{
			OnHScroll( sender, se );

			if( HorizontalScroll != null )
				HorizontalScroll( this, se );

			if( se.NewValue >= HScrollBar.Minimum && se.NewValue <= HScrollBar.Maximum )
				HScrollBar.Value = se.NewValue;
		}
		/// <summary>
		///    <para>
		///       Listens
		///       for the vertical scrollbar's scroll event.
		///    </para>
		/// </summary>
		/// <param name="sender">
		///    An <see cref="System.Object"/> that contains data about the control.
		/// </param>
		/// <param name="se">
		///    A <see cref="System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
		/// </param>
		protected virtual void OnVScrollInternal( object sender, ScrollEventArgs se )
		{
			OnVScroll( sender, se );

			if( VerticalScroll != null )
				VerticalScroll( this, se );

			if( se.NewValue >= VScrollBar.Minimum && se.NewValue <= VScrollBar.Maximum )
				VScrollBar.Value = se.NewValue;
		}
		private void ChildGotFocus( object sender, EventArgs e )
		{
			RaiseControlGotFocus();
		}
		private void ChildLostFocus( object sender, EventArgs e )
		{
			RaiseControlLostFocus();
		}
		#endregion

		#region Private Static Methods
		private static int MAKELPARAM( int low, int high )
		{
			return ( ( high << 16 ) | ( low & 0xffff ) );
		}
		private static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}
		private static int LOWORD( IntPtr n )
		{
			return LOWORD( ( int )n );
		}
		#endregion

		#region Private Methods
		private void WireScrollEvents()
		{
			vScrollBar.Scroll += new ScrollEventHandler( this.OnVScrollInternal );
			hScrollBar.Scroll += new ScrollEventHandler( this.OnHScrollInternal );
		}
		private void UnwireScrollEvents()
		{
			vScrollBar.Scroll -= new ScrollEventHandler( this.OnVScrollInternal );
			hScrollBar.Scroll -= new ScrollEventHandler( this.OnHScrollInternal );
		}
		private void SyncReflectScrollBars()
		{
			if( useSharedScrollBars )
				return;

			if( vScroll )
			{
				if( this.VScrollBar.InnerScrollBar == null )
					this.VScrollBar.InnerScrollBar = new ReflectScrollBar( this, ScrollBars.Vertical );
			}
			else
			{
				this.VScrollBar.Maximum = 0;
				this.VScrollBar.LargeChange = 0;
				this.VScrollBar.Visible = false;
				this.VScrollBar.InnerScrollBar = null;
			}

			if( hScroll )
			{
				if( this.HScrollBar.InnerScrollBar == null )
					this.HScrollBar.InnerScrollBar = new ReflectScrollBar( this, ScrollBars.Horizontal );
			}
			else
			{
				this.HScrollBar.Maximum = 0;
				this.HScrollBar.LargeChange = 0;
				this.HScrollBar.Visible = false;
				this.HScrollBar.InnerScrollBar = null;
			}
		}
		/// <summary>
		///     Actually displays or hides the horiz and vert autoscrollbars. This will
		///     also adjust the values of formState to reflect the new state
		/// </summary>
		/// <param name='horiz'>
		///     True if the horiz scrollbar should be displayed
		/// </param>
		/// <param name='vert'>
		///     True if the vert scrollbar should be displayed
		/// </param>
		/// <returns>
		///     True if the form needs to be re-layed out
		/// </returns>
		private bool SetVisibleScrollbars( bool horiz, bool vert )
		{
			if( horiz == this.hScroll &&
				vert == this.vScroll )
				return false;

			this.hScroll = horiz;
			this.vScroll = vert;

			this.UpdateStyles();
			this.SyncReflectScrollBars();

			OnScrollbarsVisibleChanged( EventArgs.Empty );

			return true;
		}
		private void WmHScroll( ref Message m )
		{
			base.WndProc( ref m );

			if( HScrollBar.IsReflect )
				HScrollBar.ReflectScrollMessage( ref m );
		}
		private void WmVScroll( ref Message m )
		{
			base.WndProc( ref m );

			if( VScrollBar.IsReflect )
				VScrollBar.ReflectScrollMessage( ref m );
		}
		private void RaiseControlGotFocus()
		{
			if( this.Disposing || this.IsDisposed )
				return;

			if( !this.hasControlFocus )
			{
				hasControlFocus = true;
				OnControlGotFocus();
			}
		}
		private void RaiseControlLostFocus()
		{
			if( this.Disposing || this.IsDisposed )
				return;

			if( !hasControlFocus )
			{
			}
			else if( !QueryFocusInside() )
			{
				hasControlFocus = false;
				OnControlLostFocus();
			}
		}
		#endregion

		#region ShouldSerialize & Reset Methods
		internal bool ShouldSerializeInsideScrollMargins()
		{
			return insideScrollMargins.Width != 10 || insideScrollMargins.Height != 10;
		}
		/// <summary>
		/// Resets the <see cref="InsideScrollMargins"/> property to its default value.
		/// </summary>
		internal void ResetInsideScrollMargins()
		{
			insideScrollMargins = new Size( 10, 10 );
		}
		#endregion
	}
	#endregion

	#region *** HybridScrollControl
	/// <summary>
	/// Hybrid between Syncfusion's scroller and windows scroller. 
	/// </summary>
	public abstract class HybridScrollControl
		: ScrollControl
	{
		#region Enums
		/// <summary>
		/// Direction of scrolling.
		/// </summary>
		public enum ScrollDirection
		{
			/// <summary>
			/// Up direction.
			/// </summary>
			Up,
			/// <summary>
			/// Down direction.
			/// </summary>
			Down
		}
		#endregion

		#region Class constants
		/// <summary>
		/// Offset in pixels of the mouse from the border of the control,
		/// where fast scrolling starts.
		/// </summary>
		private const int DEF_FAST_SCROLL_OFFSET = 20;
		/// <summary>
		/// Interval of the autoscroll timer.
		/// </summary>
		private const int DEF_SCROLLING_INTERVAL = 1;
		#endregion

		#region Class members
		/// <summary>
		/// Specifies whether horizontal scroller should be disabled.
		/// </summary>
		private bool m_bDisableHorizontalScroller;
		/// <summary>
		/// Specifies whether vertical scroller should be disabled.
		/// </summary>
		private bool m_bDisableVerticalScroller;
		/// <summary>
		/// Left offset.
		/// </summary>
		private int m_ScrollOffsetLeft;
		/// <summary>
		/// Right offset.
		/// </summary>
		private int m_ScrollOffsetRight;
		/// <summary>
		/// Top offset.
		/// </summary>
		private int m_ScrollOffsetTop;
		/// <summary>
		/// Bottom offset.
		/// </summary>
		private int m_ScrollOffsetBottom;
		/// <summary>
		/// Timer, used for autoscrolling.
		/// </summary>
		private System.Windows.Forms.Timer m_AutoScrollTimer;
		/// <summary>
		/// Step of the slow scrolling.
		/// </summary>
		private int m_SlowScrollingStep = 1;
		/// <summary>
		/// Step of the fast scrolling.
		/// </summary>
		private int m_FastScrollingStep = 5;
		/// <summary>
		/// Destination from the control`s border, the fast scrolling is activated after.
		/// </summary>
		private int m_FastScrollOffset = 30;
		/// <summary>
		/// Old vertical small change.
		/// </summary>
		private int m_oldSmallChangeV;
		/// <summary>
		/// Old horisontal small change.
		/// </summary>
		private int m_oldSmallChangeH;
		/// <summary>
		/// Current position of the auto-scrolling.
		/// </summary>
		private float m_scrollPosition;
		/// <summary>
		/// Time of the last scrolling.
		/// </summary>
		private DateTime m_lastScrollTime = DateTime.MinValue;
		/// <summary>
		/// 
		/// </summary>
		private double m_ScrollingSpeed;
		/// <summary>
		/// Custom cursor.
		/// </summary>
		private Cursor m_defCursor;
		/// <summary>
		/// Indicates whether mouse left button was pressed in area that allows further autoscrolling.
		/// </summary>
		private bool m_bAllowAutoscroll;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets value that specifies whether horizontal scroller should be disabled.
		/// </summary>
		public bool DisableHorizontalScroller
		{
			get
			{
				return m_bDisableHorizontalScroller;
			}
			set
			{
				if( value != m_bDisableHorizontalScroller )
				{
					m_bDisableHorizontalScroller = value;
					UpdateScrollBarsVisibility();
				}
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether vertical scroller should be disabled.
		/// </summary>
		public bool DisableVerticalScroller
		{
			get
			{
				return m_bDisableVerticalScroller;
			}
			set
			{
				if( value != m_bDisableVerticalScroller )
				{
					m_bDisableVerticalScroller = value;
					UpdateScrollBarsVisibility();
				}
			}
		}
		/// <summary>
		/// Left offset.
		/// </summary>
		[Browsable( false )]
		public int ScrollOffsetLeft
		{
			get
			{
				return m_ScrollOffsetLeft;
			}
			set
			{
				m_ScrollOffsetLeft = value;
			}
		}
		/// <summary>
		/// Right offset.
		/// </summary>
		[Browsable( false )]
		public int ScrollOffsetRight
		{
			get
			{
				return m_ScrollOffsetRight;
			}
			set
			{
				m_ScrollOffsetRight = value;
			}
		}
		/// <summary>
		/// Top offset.
		/// </summary>
		[Browsable( false )]
		public int ScrollOffsetTop
		{
			get
			{
				return m_ScrollOffsetTop;
			}
			set
			{
				m_ScrollOffsetTop = value;
			}
		}
		/// <summary>
		/// Bottom offset.
		/// </summary>
		[Browsable( false )]
		public int ScrollOffsetBottom
		{
			get
			{
				return m_ScrollOffsetBottom;
			}
			set
			{
				m_ScrollOffsetBottom = value;
			}
		}
		/// <summary>
		/// Control's virtual size.
		/// </summary>
		/// <remarks>
		/// If control's client area is smaller then virtual size, then
		/// scrollers will be visible.
		/// </remarks>
		[Browsable( false )]
		public Size VirtualSize
		{
			get
			{
				return new Size( HScrollBar.Maximum, VScrollBar.Maximum );
			}
			set
			{
				if( HScrollBar.Maximum != value.Width ||
					VScrollBar.Maximum != value.Height )
				{
					HScrollBar.Maximum = value.Width;
					VScrollBar.Maximum = value.Height;

					UpdateScrollBarsVisibility();
					UpdateScrollBarsSize();
					CorrectPosition();
				}
			}
		}
        internal bool resetCached = false;
		/// <summary>
		/// Position of the scroller.
		/// </summary>
		[Browsable( false )]
		public virtual Point AutoScrollPosition
		{
			get
			{
				return new Point( -HScrollBar.Value, -VScrollBar.Value );
			}
            set
            {
                if (AutoScrollPosition != value && !resetCached)
                {
                    int oldValueY = VScrollBar.Value;
                    int oldValueX = HScrollBar.Value;
                    if (oldValueY != value.Y)
                        SetScrollerPosition(VScrollBar, value.Y);

                    if (oldValueX != value.X)
                        SetScrollerPosition(HScrollBar, value.X);
                }
            }
		}
		/// <summary>
		/// Step of the slow scrolling.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( 1 )]
		[Description( "Count of lines, that will be scrolled in slow aoutscroll mode." )]
		public int SlowScrollingStep
		{
			get
			{
				return m_SlowScrollingStep;
			}
			set
			{
				m_SlowScrollingStep = value;
			}
		}
		/// <summary>
		/// Step of the fast scrolling.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( 5 )]
		[Description( "Count of lines, that will be scrolled in fast aoutscroll mode." )]
		public int FastScrollingStep
		{
			get
			{
				return m_FastScrollingStep;
			}
			set
			{
				m_FastScrollingStep = value;
			}
		}
		/// <summary>
		/// Distance from the control`s border, where scrolling starts
		/// using <see cref="FastScrollingStep"/> instead of <see cref="SlowScrollingStep"/>.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( 30 )]
		[Description( "Distance in pixels from the control`s border, where fast autoscrolling starts." )]
		public int FastScrollOffset
		{
			get
			{
				return m_FastScrollOffset;
			}
			set
			{
				m_FastScrollOffset = value;
			}
		}
		/// <summary>
		/// Gets value, indicating whether control is now in autoscroll mode.
		/// </summary>
		protected bool IsAutoScrolling
		{
			get
			{
				return m_AutoScrollTimer.Enabled;
			}
		}
		/// <summary>
		/// Gets or sets custom cursor for mouse pointer.
		/// </summary>
		public new Cursor DefaultCursor
		{
			get
			{
				return ( m_defCursor == null ) ? ( Cursor.Current ) : ( m_defCursor );
			}
			set
			{
				if( m_defCursor != null ) m_defCursor.Dispose();

				m_defCursor = value;
			}
		}
		/// <summary>
		/// Rectangle for autoscrolling.
		/// </summary>
		protected virtual Rectangle AutoScrollRectangle
		{
			get
			{
				return new Rectangle( this.ScrollOffsetLeft, this.ScrollOffsetTop,
					this.ClientRectangle.Width - this.ScrollOffsetLeft - this.ScrollOffsetRight,
					this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom );
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when size of the scrollbars changes.
		/// </summary>
		internal event EventHandler ScrollbarsSizeUpdated;
		/// <summary>
		/// Event that is raised when visibility of scrollbars is updated.
		/// </summary>
		internal event EventHandler ScrollbarsVisibilityUpdated;
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Updates size and page size of the scrollbars.
		/// </summary>
		protected virtual void UpdateScrollBarsSize()
		{
			VScrollBar.LargeChange = ( DisableHorizontalScroller )
				? 0 : ( int )Math.Round( Math.Max( 0f,
				ClientRectangle.Height - ScrollOffsetBottom ) );
			HScrollBar.LargeChange = ( DisableHorizontalScroller )
				? 0 : ( int )Math.Round( Math.Max( 0f,
				this.ClientRectangle.Width - ScrollOffsetRight ) );

			if( null != ScrollbarsSizeUpdated )
			{
				ScrollbarsSizeUpdated( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Updates visibility of the ScrollBars.
		/// </summary>
		protected internal virtual void UpdateScrollBarsVisibility()
		{
			HScroll = !DisableHorizontalScroller && ( VirtualSize.Width > this.ClientRectangle.Width - ScrollOffsetRight );
			VScroll = !DisableVerticalScroller && ( VirtualSize.Height > ClientRectangle.Height - ScrollOffsetBottom );

			if( ScrollbarsVisibilityUpdated != null )
			{
				ScrollbarsVisibilityUpdated( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Scrolls control vertically by specified amount of lines.
		/// </summary>
		/// <param name="fLinesCount">Count of lines to scroll. Must be always positive.</param>
		/// <param name="direction">Direction of scrolling.</param>
		protected abstract void ScrollLines( ScrollDirection direction, float fLinesCount );
		/// <summary>
		/// Scrolls control vertically by specified amount of lines.
		/// </summary>
		/// <param name="fLinesCount">Count of lines to scroll.</param>
		protected void ScrollLines( float fLinesCount )
		{
			if( fLinesCount == 0 )
				return;

			ScrollLines( ( fLinesCount <= 0 ) ? ScrollDirection.Up : ScrollDirection.Down, Math.Abs( fLinesCount ) );
		}
		/// <summary>
		/// Scrolls specified scroller.
		/// </summary>
		/// <param name="offset"></param>
		private void ScrollHorisontalFastOrSlow( int offset )
		{
			if( HScrollBar.Maximum > this.ClientRectangle.Width - this.ScrollOffsetLeft - this.ScrollOffsetRight )
			{
				int oldChange = HScrollBar.SmallChange;

				if( Math.Abs( offset ) >= m_FastScrollOffset )
					HScrollBar.SmallChange = m_FastScrollingStep * 16 / 10;
				else
					HScrollBar.SmallChange = m_SlowScrollingStep * 16 / 10;

				ScrollEventType scrollType =
					( offset > 0 ) ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;

				HScrollBar.SendScrollMessage( scrollType );
				HScrollBar.SmallChange = oldChange;
			}
		}
		/// <summary>
		/// Scrolls specified scroller.
		/// </summary>
		/// <param name="scroller"></param>
		/// <param name="offset"></param>
		internal void ScrollScroller( ScrollBarWrapper scroller, int offset )
		{
			int oldChange = scroller.SmallChange;
			ScrollEventType scrollType =
				( offset > 0 ) ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;

			offset = Math.Abs( offset );
			scroller.SmallChange = offset;
			int iLargeChange = scroller.LargeChange;

			do
			{
				if( offset > iLargeChange )
					offset -= iLargeChange;
				else
					offset = 0;

				scroller.SendScrollMessage( scrollType );

			}
			while( offset > 0 );

			scroller.SmallChange = oldChange;
		}
		/// <summary>
		/// Sets new position of the scroller. Supports short smooth jumps and long jumps.
		/// </summary>
		/// <param name="wrapper">Scroller to be scrolled.</param>
		/// <param name="position">New position of the scroller.</param>
		private void SetScrollerPosition( ScrollBarWrapper wrapper, int position )
		{
			position = Math.Min( position, wrapper.Maximum - wrapper.LargeChange );
			position = Math.Max( position, wrapper.Minimum );

			if( position == wrapper.Value ) return;

			// Scrolling is inside one window.
			bool smallScroll = Math.Abs( wrapper.Value - position ) < wrapper.LargeChange;

			// Fixes problems with scrolling when scrollers are hidden.
			if( ( wrapper == HScrollBar && !HScroll ) || ( wrapper == VScrollBar && !VScroll ) )
				wrapper.Value = position;

			// Setting up position of the scroller.
			ScrollApi.SetScrollPos( this.Handle,
				( wrapper == HScrollBar ) ? ScrollerConst.Horizontal : ScrollerConst.Vertical, position, true );

			// Message sending to update internal data about reflect scroller.
			Message message = new Message();
			message.Msg = ( int )( ( wrapper == HScrollBar ) ? ( int )Msg.WM_HSCROLL : ( int )Msg.WM_VSCROLL );
			int wparam = ( int )ScrollCommands.ThumbPosition | ( position << 16 );
			message.WParam = ( IntPtr )wparam;

			wrapper.ReflectScrollMessage( ref message );

			if( !smallScroll )
			{
				Invalidate();
			}
		}
		/// <summary>
		/// Scrolls window by specified amounts of pixels.
		/// </summary>
		/// <param name="xAmount">X amount to scroll.</param>
		/// <param name="yAmount">Y amount to scroll.</param>
		protected void ScrollWindow( int xAmount, int yAmount )
		{
			Rectangle bounds = Rectangle.Empty;
            if (this.RightToLeft == RightToLeft.Yes)
            {
                xAmount *= -1;
                bounds = new System.Drawing.Rectangle
                    (
                    0,ScrollOffsetTop,
                    this.ClientRectangle.Width - ScrollOffsetLeft - ScrollOffsetRight,
                    this.ClientRectangle.Height - ScrollOffsetTop - ScrollOffsetBottom
                    );
            }
            else
            {
                bounds = new System.Drawing.Rectangle(
                    ScrollOffsetLeft + 2, ScrollOffsetTop,
                    this.ClientRectangle.Width - ScrollOffsetLeft - ScrollOffsetRight - 2,
                    this.ClientRectangle.Height - ScrollOffsetTop - ScrollOffsetBottom
                    );
            }
			this.ScrollWindow( xAmount, yAmount, bounds, bounds );
		}
		/// <summary>
		/// Corrects scrollers position.
		/// </summary>
		protected virtual void CorrectPosition()
		{
			SetScrollerPosition( HScrollBar, HScrollBar.Value );
			SetScrollerPosition( VScrollBar, VScrollBar.Value );
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Handler of the WM_SIZE event.
		/// </summary>
		/// <remarks>
		/// On window resize scrollers are updated:
		/// their maximum size changes and their page size changes.
		/// </remarks>
		/// <param name="e">EventArgs.</param>
		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );
			UpdateScrollBarsSize();
			UpdateScrollBarsVisibility();

			CorrectScroller( HScrollBar );
			CorrectScroller( VScrollBar );
		}
		/// <summary>
		/// Correctes value of the given scrollbar.
		/// </summary>
		/// <param name="bar">ScrollBarWrapper.</param>
		private void CorrectScroller( ScrollBarWrapper bar )
		{
			int old = bar.Value;
			bar.Value = Math.Max( bar.Minimum,
				Math.Min( bar.Maximum - bar.LargeChange, bar.Value ) );

			if( old != bar.Value )
				Invalidate();
		}
		/// <summary>
		/// Processes horizontal scroll event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="se">ScrollEventArgs.</param>
		protected override void OnHScroll( object sender, ScrollEventArgs se )
		{
			int xAmount = ( this.HScrollBar.Value - se.NewValue );
			ScrollWindow( xAmount, 0 );

			base.OnHScroll( sender, se );
		}
		/// <summary>
		/// Processes vertical scroll event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="se">ScrollEventArgs.</param>
		protected override void OnVScroll( object sender, ScrollEventArgs se )
		{
			int yAmount = ( this.VScrollBar.Value - se.NewValue );
			ScrollWindow( 0, yAmount );

			base.OnVScroll( sender, se );
		}
		/// <summary>
		/// Enables autoscrolling.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			m_oldSmallChangeV = VScrollBar.SmallChange;
			m_oldSmallChangeH = HScrollBar.SmallChange;

            //if (CheckIfCanStartAutoscroll())
            //{
				Capture = true;
				m_bAllowAutoscroll = true;
				m_AutoScrollTimer.Enabled = false;
            //}
            //else
            //{
            //    m_bAllowAutoscroll = false;
            //}

			m_scrollPosition = VScrollBar.Value;
			m_lastScrollTime = DateTime.MinValue;
			base.OnMouseDown( e );
		}
		/// <summary>
		/// Starts or stops timer depending on the position of the mouse.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			bool timerEnabled = m_AutoScrollTimer.Enabled;

			Rectangle rect = new Rectangle( ScrollOffsetLeft, ScrollOffsetTop,
				this.ClientRectangle.Width - ScrollOffsetLeft - ScrollOffsetRight,
				this.ClientRectangle.Height - ScrollOffsetTop - ScrollOffsetBottom );

			timerEnabled = ( Control.MouseButtons == MouseButtons.Left && this.Focused && m_bAllowAutoscroll );
			timerEnabled &= !rect.Contains( e.X, e.Y );

			m_AutoScrollTimer.Enabled = timerEnabled;

			if( !timerEnabled )
				m_ScrollingSpeed = 0;

			base.OnMouseMove( e );
		}
		/// <summary>
		/// Checks whether Autoscrolling can be started when user presses mouse button down.
		/// </summary>
		/// <returns>If return value is true, autoscrolling will be allowed.</returns>
		protected virtual bool CheckIfCanStartAutoscroll()
		{
			Point point = PointToClient( MousePosition );

			Rectangle ClientRectangle = new Rectangle( ScrollOffsetLeft, ScrollOffsetTop,
				this.ClientRectangle.Width - ScrollOffsetLeft - ScrollOffsetRight,
				this.ClientRectangle.Height - ScrollOffsetTop - ScrollOffsetBottom );

			return ClientRectangle.Contains( point );
		}
		/// <summary>
		/// Disables autoscrolling.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			if( m_AutoScrollTimer.Enabled )
			{
				m_AutoScrollTimer.Enabled = false;
				Capture = false;

				VScrollBar.SmallChange = m_oldSmallChangeV;
				HScrollBar.SmallChange = m_oldSmallChangeH;
			}

			base.OnMouseUp( e );
		}
		/// <summary>
		/// Stops scrolling.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			if( m_AutoScrollTimer.Enabled )
			{
				m_AutoScrollTimer.Enabled = false;

				VScrollBar.SmallChange = m_oldSmallChangeV;
				HScrollBar.SmallChange = m_oldSmallChangeH;
			}
		}

		/// <summary>
		/// Handles mouse wheel processing for our scrollbars.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseWheel( MouseEventArgs e )
		{
			if( e.Delta != 0 )
			{
				if( ( Control.ModifierKeys & System.Windows.Forms.Keys.Control ) != 0 )
				{
					base.OnMouseWheel( e );
				}
				else
				{
					if( ( Control.ModifierKeys & System.Windows.Forms.Keys.Shift ) == 0 &&
						!VScrollBar.IsEmpty && VScrollBar.Maximum > VScrollBar.Minimum )
					{
						float amount = ( float )( e.Delta / 120.0 * SystemInformation.MouseWheelScrollLines );
						ScrollLines( ( e.Delta > 0 ) ? ScrollDirection.Up : ScrollDirection.Down, Math.Abs( amount ) );
					}
					else if( !HScrollBar.IsEmpty && HScrollBar.Maximum > HScrollBar.Minimum )
					{
						int newValue = HScrollBar.Value + -( e.Delta / 120 ) * 20;
						ScrollEventType et = ( e.Delta > 0 ) ? ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement;

						if( newValue >= HScrollBar.Minimum )
							this.OnHScrollInternal( this, new ScrollEventArgs( et, Math.Min( newValue, HScrollBar.Maximum - HScrollBar.LargeChange + 1 ) ) );
					}

					m_scrollPosition = VScrollBar.Value;
					m_lastScrollTime = DateTime.MinValue;

					MouseEventArgs args = new MouseEventArgs( Control.MouseButtons, 0, e.X, e.Y, 0 );
					OnMouseMove( args );
				}
			}
		}

		/// <summary>
		/// Called before scrolling by timer on every timer tick. 
		/// </summary>
		/// <param name="yOffset">Offset by y.</param>
		protected virtual void BeforeAutoScroll( int yOffset )
		{
		}
		/// <summary>
		/// Called after scrolling by timer on every timer tick. 
		/// </summary>
		protected virtual void AfterAutoScroll()
		{
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes sublying ScrollControl.
		/// </summary>
		public HybridScrollControl()
		{
			m_AutoScrollTimer = new System.Windows.Forms.Timer();
			m_AutoScrollTimer.Enabled = false;
			m_AutoScrollTimer.Interval = DEF_SCROLLING_INTERVAL;
			m_AutoScrollTimer.Tick += new EventHandler( m_AutoScrollTimer_Tick );
		}

		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Handler of the Tick event of the AutoScroll timer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_AutoScrollTimer_Tick( object sender, EventArgs e )
		{
			Point point = PointToClient( MousePosition );

			Rectangle clientRectangle = this.AutoScrollRectangle;

			if( Control.MouseButtons != MouseButtons.Left || !this.Focused )
			{
				m_AutoScrollTimer.Enabled = false;
				Capture = false;
				return;
			}

			if( clientRectangle.Contains( point ) ) return;

			if( m_lastScrollTime == DateTime.MinValue )
				m_lastScrollTime = DateTime.Now;

			int xoffset = 0;
			int yoffset = 0;

			if( point.X < clientRectangle.Left )
				xoffset = point.X - clientRectangle.Left;
			else if( point.X > clientRectangle.Right )
				xoffset = point.X - clientRectangle.Right;

			if( point.Y < clientRectangle.Top )
				yoffset = point.Y - clientRectangle.Top;
			else if( point.Y > clientRectangle.Bottom )
				yoffset = point.Y - clientRectangle.Bottom;

			bool canHScroll = ( xoffset > 0 ) ? HScrollBar.Value < HScrollBar.Maximum : HScrollBar.Value > HScrollBar.Minimum;
			bool canVScroll = ( yoffset > 0 ) ? VScrollBar.Value < VScrollBar.Maximum : VScrollBar.Value > VScrollBar.Minimum;

			if( !canHScroll && !canVScroll ) return;

			if( xoffset != 0 && canHScroll )
				ScrollHorisontalFastOrSlow( xoffset );

			if( yoffset != 0 )
			{
				ScrollEventArgs args = new ScrollEventArgs( ( yoffset > 0 ) ?
					ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement, 0 );

				double timespan = ( ( TimeSpan )( DateTime.Now - m_lastScrollTime ) ).TotalMilliseconds;
				float scrollingSpeed = Math.Min( 60f,
					( float )Math.Pow(
					Math.Abs( yoffset ) / 5, 1.9f ) );

				double deltaSpeed = ( scrollingSpeed - m_ScrollingSpeed );
				deltaSpeed = Math.Min( 0.5, Math.Abs( deltaSpeed ) ) * Math.Sign( deltaSpeed );

				m_ScrollingSpeed += deltaSpeed;

				float m_scrollDeltaFloat =
					Math.Min(
					( float )( m_ScrollingSpeed * timespan / 20 )
					, VScrollBar.LargeChange / 3f )
					* Math.Sign( yoffset );
				m_scrollPosition += m_scrollDeltaFloat;

				m_scrollPosition = Math.Min( m_scrollPosition, Math.Max( VScrollBar.Maximum - VScrollBar.LargeChange, 0 ) );
				m_scrollPosition = Math.Max( 0, m_scrollPosition );


				if( VScrollBar.Value != ( int )m_scrollPosition )
				{
					int delta = ( int )m_scrollPosition - VScrollBar.Value;

					if( delta != 0 )
						BeforeAutoScroll( delta );

					args.NewValue = ( int )m_scrollPosition;
					OnVScrollInternal( this, args );
				}
			}

			if( xoffset != 0 || yoffset != 0 )
				AfterAutoScroll();

			Update();
			m_lastScrollTime = DateTime.Now;
		}
		#endregion
	}
	#endregion

	#region *** IntelliScrollableControl
	/// <summary>
	/// Scrollable control, that supports intelli mouse.
	/// </summary>
	public abstract class IntelliScrollableControl
		: HybridScrollControl
	{
		#region Internal Classes
		/// <summary>
		/// Current scrolling direction.
		/// </summary>
		[Flags]
		protected enum ScrollingDirection
		{
			/// <summary>
			/// None direction.
			/// </summary>
			None = 0,
			/// <summary>
			/// Up direction.
			/// </summary>
			Up = 1,
			/// <summary>
			/// Down direction.
			/// </summary>
			Down = 2,
			/// <summary>
			/// Left direction.
			/// </summary>
			Left = 4,
			/// <summary>
			/// Right direction.
			/// </summary>
			Right = 8,
			/// <summary>
			/// All directions.
			/// </summary>
			All = 16,
			/// <summary>
			/// Invalid direction.
			/// </summary>
			Invalid = 32,
			/// <summary>
			/// UpLeft direction.
			/// </summary>
			UpLeft = Up | Left,
			/// <summary>
			/// UpRight direction.
			/// </summary>
			UpRight = Up | Right,
			/// <summary>
			/// DownLeft direction.
			/// </summary>
			DownLeft = Down | Left,
			/// <summary>
			/// DownRight direction.
			/// </summary>
			DownRight = Down | Right
		}
		#endregion

		#region Constants
		/// <summary>
		/// Name of the resource with common movement cursor.
		/// </summary>
		private const string m_CommonCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMA.CUR";
		/// <summary>
		/// Name of the resource with up movement cursor.
		/// </summary>                                       
		private const string m_UpCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMU.CUR";
		/// <summary>
		/// Name of the resource with down movement cursor.
		/// </summary>
		private const string m_DownCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMD.CUR";
		/// <summary>
		/// Name of the resource with left movement cursor.
		/// </summary>
		private const string m_LeftCursorName = "Syncfusion.Windows.Forms.Edit.Images.IML.CUR";
		/// <summary>
		/// Name of the resource with down movement cursor.
		/// </summary>
		private const string m_RightCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMR.CUR";
		/// <summary>
		/// Name of the resource with up left movement cursor.
		/// </summary>                                       
		private const string m_UpLeftCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMUL.cur";
		/// <summary>
		/// Name of the resource with down right movement cursor.
		/// </summary>
		private const string m_DownRightCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMDR.cur";
		/// <summary>
		/// Name of the resource with down left movement cursor.
		/// </summary>
		private const string m_DownLeftCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMDL.cur";
		/// <summary>
		/// Name of the resource with down right movement cursor.
		/// </summary>
		private const string m_UpRightCursorName = "Syncfusion.Windows.Forms.Edit.Images.IMUR.cur";
		/// <summary>
		/// WM_SETCURSOR message.
		/// </summary>
		private const int WM_SETCURSOR = 0x0020;
		/// <summary>
		/// Minimum distance between click position and currnt mouse position to begin scrolling.
		/// </summary>
		private const int DEF_CURSOR_MIN_DISTANCE = 15;
		#endregion

		#region Static Fields
		/// <summary>
		/// Common movement cursor.
		/// </summary>
		private static Cursor m_CommonCursor;
		/// <summary>
		/// Up movement cursor.
		/// </summary>
		private static Cursor m_UpCursor;
		/// <summary>
		/// Down movement cursor.
		/// </summary>
		private static Cursor m_DownCursor;
		/// <summary>
		/// Left movement cursor.
		/// </summary>
		private static Cursor m_LeftCursor;
		/// <summary>
		/// Right movement cursor.
		/// </summary>
		private static Cursor m_RightCursor;
		/// <summary>
		/// Up movement cursor.
		/// </summary>
		private static Cursor m_UpLeftCursor;
		/// <summary>
		/// Down movement cursor.
		/// </summary>
		private static Cursor m_DownLeftCursor;
		/// <summary>
		/// Left movement cursor.
		/// </summary>
		private static Cursor m_DownRightCursor;
		/// <summary>
		/// Right movement cursor.
		/// </summary>
		private static Cursor m_UpRightCursor;
		#endregion

		#region Fields
		/// <summary>
		/// Last point, where user clicked by middle button.
		/// </summary>
		private Point m_mouseClickPoint;
		/// <summary>
		/// Determines, whether middle button is in pressed state.
		/// </summary>
		private bool m_pressed;
		/// <summary>
		/// Current scrolling direction.
		/// </summary>
		private ScrollingDirection m_direction = ScrollingDirection.None;
		/// <summary>
		/// Old scrolling direction.
		/// <seealso cref="UpdateCursor"/>
		/// </summary>
		private ScrollingDirection m_olddirection = ScrollingDirection.None;
		/// <summary>
		/// Scrolling speed factor.
		/// </summary>
		private float m_ScrollingSpeedFactor = 1.6f;
		/// <summary>
		/// Number, the distance will be divided to when calculating speed.
		/// </summary>
		private float m_distanceDivisionFactor = 12;
		/// <summary>
		/// Timer, used for scrolling.
		/// </summary>
		private Timer m_scrollTimer;
		/// <summary>
		/// Scrolling speed.
		/// </summary>
		private float m_ScrollingSpeed;
		/// <summary>
		/// Scrool position, saved when mouse is clicked.
		/// </summary>
		private PointF m_oldScrollPosition;
		/// <summary>
		/// Special cursor, not used if null.
		/// </summary>
		private Cursor m_cursorSpecial;
		#endregion

		#region Static Properties
		/// <summary>
		/// Gets cursor, that shows four directions all together.
		/// </summary>
		public static Cursor CommonCursor
		{
			get
			{
				if( m_CommonCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_CommonCursorName );

					m_CommonCursor = new Cursor( stream );
				}

				return m_CommonCursor;
			}
		}
		/// <summary>
		/// GET cursor with Up-right arrow.
		/// </summary>
		public static Cursor UpRightCursor
		{
			get
			{
				if( m_UpRightCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_UpRightCursorName );

					m_UpRightCursor = new Cursor( stream );
				}

				return m_UpRightCursor;
			}
		}
		/// <summary>
		/// GET cursor with down-left arrow.
		/// </summary>
		public static Cursor DownLeftCursor
		{
			get
			{
				if( m_DownLeftCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_DownLeftCursorName );

					m_DownLeftCursor = new Cursor( stream );
				}

				return m_DownLeftCursor;
			}
		}
		/// <summary>
		/// GET cursor with down-right arrow.
		/// </summary>
		public static Cursor DownRightCursor
		{
			get
			{
				if( m_DownRightCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_DownRightCursorName );

					m_DownRightCursor = new Cursor( stream );
				}

				return m_DownRightCursor;
			}
		}
		/// <summary>
		/// GET cursor with up-left arrow.
		/// </summary>
		public static Cursor UpLeftCursor
		{
			get
			{
				if( m_UpLeftCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_UpLeftCursorName );

					m_UpLeftCursor = new Cursor( stream );
				}

				return m_UpLeftCursor;
			}
		}
		/// <summary>
		/// GET cursor with right arrow.
		/// </summary>
		public static Cursor RightCursor
		{
			get
			{
				if( m_RightCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_RightCursorName );

					m_RightCursor = new Cursor( stream );
				}

				return m_RightCursor;
			}
		}
		/// <summary>
		/// GET cursor with left arrow.
		/// </summary>
		public static Cursor LeftCursor
		{
			get
			{
				if( m_LeftCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_LeftCursorName );

					m_LeftCursor = new Cursor( stream );
				}

				return m_LeftCursor;
			}
		}
		/// <summary>
		/// GET cursor with down arrow.
		/// </summary>
		public static Cursor DownCursor
		{
			get
			{
				if( m_DownCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_DownCursorName );

					m_DownCursor = new Cursor( stream );
				}

				return m_DownCursor;
			}
		}
		/// <summary>
		/// GET cursor, with up-arrow.
		/// </summary>
		public static Cursor UpCursor
		{
			get
			{
				if( m_UpCursor == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( m_UpCursorName );

					m_UpCursor = new Cursor( stream );
				}

				return m_UpCursor;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// GET state of the Intelly scrolling.
		/// </summary>
		[Browsable( false )]
		public bool IsIntellyScrollActive
		{
			get
			{
				return m_pressed;
			}
		}
		/// <summary>
		/// GET, SET special cursor, that should be used instead of the default control's cursor.
		/// </summary>
		protected Cursor SpecialCursor
		{
			get
			{
				return m_cursorSpecial;
			}
			set
			{
				m_cursorSpecial = value;
			}
		}
		/// <summary>
		/// Indicates whether cursor changing is allowed.
		/// </summary>
		protected virtual bool IsCursorChangingAllowed
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Initializes timer, used for scrolling.
		/// </summary>
		public IntelliScrollableControl()
		{
			m_scrollTimer = new Timer();
			m_scrollTimer.Tick += new EventHandler( ScrollTimerTick );
			m_scrollTimer.Interval = 1;
			m_scrollTimer.Stop();
		}
		/// <summary>
		/// Disposes timer, used for scrolling.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( m_scrollTimer != null )
			{
				m_scrollTimer.Dispose();
				m_scrollTimer = null;
			}

			base.Dispose( disposing );
		}

		#endregion

		#region Overrides
		/// <summary>
		/// Processes presses of the middle button.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Middle || ( e.Button != MouseButtons.Middle && m_pressed ) )
			{
				m_pressed = !m_pressed;
			}

			m_mouseClickPoint = new Point( e.X, e.Y );
			Capture = m_pressed;

			m_direction = ( m_pressed ) ? ScrollingDirection.All : ScrollingDirection.None;
			UpdateCursor();

			m_oldScrollPosition.X = -AutoScrollPosition.X;
			m_oldScrollPosition.Y = -AutoScrollPosition.Y;

			base.OnMouseDown( e );
		}
		/// <summary>
		/// Processes mouse movement, sets scrolling mode and speed.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			if( m_pressed )
			{
				float xDistance = e.X - m_mouseClickPoint.X;
				float yDistance = e.Y - m_mouseClickPoint.Y;
				m_ScrollingSpeed = 0;

				if( Math.Abs( xDistance ) <= DEF_CURSOR_MIN_DISTANCE && Math.Abs( yDistance ) <= DEF_CURSOR_MIN_DISTANCE )
					m_direction = ScrollingDirection.All;
				else
				{
					m_direction = ScrollingDirection.None;

					if( Math.Abs( xDistance ) > DEF_CURSOR_MIN_DISTANCE )
						m_direction |= ( xDistance > 0 ) ? ScrollingDirection.Right : ScrollingDirection.Left;

					if( Math.Abs( yDistance ) > DEF_CURSOR_MIN_DISTANCE )
						m_direction |= ( yDistance > 0 ) ? ScrollingDirection.Down : ScrollingDirection.Up;

					float distance = ( float )Math.Sqrt( xDistance * xDistance + yDistance * yDistance );
					m_ScrollingSpeed = ( float )Math.Pow( ( Math.Abs( distance ) - DEF_CURSOR_MIN_DISTANCE ) / m_distanceDivisionFactor, m_ScrollingSpeedFactor );
				}

				m_scrollTimer.Enabled = ( m_direction != ScrollingDirection.None ) &&
					( m_direction != ScrollingDirection.All );

				UpdateCursor();
			}

			base.OnMouseMove( e );
		}
		/// <summary>
		/// Stops scrolling.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Middle && m_direction != ScrollingDirection.All )
			{
				Capture = false;
				m_pressed = false;
				m_scrollTimer.Stop();
				m_direction = ScrollingDirection.None;
				m_oldScrollPosition = PointF.Empty;
				UpdateCursor();
			}

			base.OnMouseUp( e );
		}
		/// <summary>
		/// Processes WM_SETCURSOR.
		/// </summary>
		/// <param name="m">Windows message to process.</param>
		[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == WM_SETCURSOR && this.IsCursorChangingAllowed )
			{
				if( m_pressed )
					UpdateCursor();
				else
				{
					Point point = this.PointToClient( Control.MousePosition );

					if( this.ClientRectangle.Contains( point ) || Capture )
						Cursor.Current = ( m_cursorSpecial != null ) ? m_cursorSpecial : this.Cursor;
					else
						Cursor.Current = this.DefaultCursor;
				}
			}
			else
				base.WndProc( ref m );
		}
		/// <summary>
		/// Resets cached info about current cursor.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnCursorChanged( EventArgs e )
		{
			base.OnCursorChanged( e );

			ResetCursor();
			UpdateCursor();
		}
		/// <summary>
		/// Stops scrolling.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );
			m_pressed = false;
			m_scrollTimer.Stop();
			this.Capture = false;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Resets cached info about current cursor.
		/// </summary>
		protected new void ResetCursor()
		{
			m_olddirection = ScrollingDirection.Invalid;
		}
		/// <summary>
		/// Updates current cursor according to the scroll direction.
		/// </summary>
		protected void UpdateCursor()
		{
			if( !m_pressed )
				m_direction = ScrollingDirection.None;

			if( m_direction == m_olddirection ) return;

			if( m_direction == ScrollingDirection.All )
				Cursor.Current = CommonCursor;
			else if( m_direction == ScrollingDirection.None )
				Cursor.Current = this.Cursor;
			else if( m_direction == ScrollingDirection.Left )
				Cursor.Current = LeftCursor;
			else if( m_direction == ScrollingDirection.Right )
				Cursor.Current = RightCursor;
			else if( m_direction == ScrollingDirection.Up )
				Cursor.Current = UpCursor;
			else if( m_direction == ScrollingDirection.Down )
				Cursor.Current = DownCursor;
			else if( m_direction == ScrollingDirection.UpLeft )
				Cursor.Current = UpLeftCursor;
			else if( m_direction == ScrollingDirection.DownRight )
				Cursor.Current = DownRightCursor;
			else if( m_direction == ScrollingDirection.UpRight )
				Cursor.Current = UpRightCursor;
			else if( m_direction == ScrollingDirection.DownLeft )
				Cursor.Current = DownLeftCursor;
			else Cursor.Current = this.Cursor;

			m_olddirection = m_direction;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Processes timer Tick events and scrolls window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void ScrollTimerTick( object sender, EventArgs e )
		{
			if( m_pressed )
			{
				this.Capture = true;
				m_scrollTimer.Stop();

				if( ( m_direction & ScrollingDirection.Up ) == ScrollingDirection.Up )
				{
					m_oldScrollPosition.Y -= m_ScrollingSpeed;
				}
				else if( ( m_direction & ScrollingDirection.Down ) == ScrollingDirection.Down )
				{
					m_oldScrollPosition.Y += m_ScrollingSpeed;
				}

				if( ( m_direction & ScrollingDirection.Left ) == ScrollingDirection.Left )
				{
					m_oldScrollPosition.X -= m_ScrollingSpeed;
				}
				else if( ( m_direction & ScrollingDirection.Right ) == ScrollingDirection.Right )
				{
					m_oldScrollPosition.X += m_ScrollingSpeed;
				}

				m_oldScrollPosition.X = Math.Max( 0, m_oldScrollPosition.X );
				m_oldScrollPosition.X = Math.Min( Math.Max( 0, VirtualSize.Width - ClientSize.Width ), m_oldScrollPosition.X );
				m_oldScrollPosition.Y = Math.Max( 0, m_oldScrollPosition.Y );
				m_oldScrollPosition.Y = Math.Min( Math.Max( 0, VirtualSize.Height - ClientSize.Height ), m_oldScrollPosition.Y );
				AutoScrollPosition = new Point( ( int )m_oldScrollPosition.X, ( int )m_oldScrollPosition.Y );

				m_scrollTimer.Start();
			}
		}
		#endregion
	}
	#endregion
}