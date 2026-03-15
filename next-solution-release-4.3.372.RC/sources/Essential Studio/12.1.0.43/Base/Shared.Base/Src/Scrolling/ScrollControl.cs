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

#region file using directives
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Localization;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// This interface provides properties for accessing a vertical and horizontal <see cref="ScrollBarWrapper"/>
	/// and an <see cref="UpdateScrollBars"/> method.
	/// </summary>
	public interface IScrollBarWrapperContainer
	{
		/// <summary>
		/// Returns a reference to an object with vertical scrollbar settings of the control.
		/// </summary>
		ScrollBarWrapper VScrollBar { get; }

		/// <summary>
		/// Returns a reference to an object with horizontal scrollbar settings of the control.
		/// </summary>
		ScrollBarWrapper HScrollBar { get; }

		/// <summary>
		/// Updates scrollbars to reflect recent changes in scroll position, minimum and maximum scroll position values.
		/// </summary>
		void UpdateScrollBars();
	}

	/// <summary>
	/// Provides support for <see cref="QueryFocusInside"/> method.
	/// </summary>
	public interface IQueryFocusInside
	{
		/// <summary>
		/// Indicates whether this control contains focus. Override this method if you
		/// want to show drop-down windows and indicate the control has not lost focus when
		/// the drop-down is shown.
		/// </summary>
		/// <returns>True if the control or any child control has focus; false otherwise.</returns>
		bool QueryFocusInside();
	}

	/// <summary>
	/// A ScrollTip window is a top-level window that gives feedback about the
	/// current scroll position when the user grabs a scrollbar thumb and drags it.
	/// </summary>
	public class ScrollTipWindow: TopLevelWindow
	{
		/// <summary></summary>
		private BorderStyle borderStyle = BorderStyle.FixedSingle;
		/// <summary></summary>
		private StringFormat sf;

		/// <summary>
		/// Initializes a new <see cref="ScrollTipWindow"/>.
		/// </summary>
		public ScrollTipWindow()
		{
			this.SetStyle( ControlStyles.AllPaintingInWmPaint | WhidbeyCompatibleControlStyles.DoubleBuffer, true );
			this.SetStyle( ControlStyles.Selectable, false );
			BackColor = SystemColors.Info;
			ForeColor = SystemColors.InfoText;

			sf = new StringFormat( StringFormatFlags.NoWrap | StringFormatFlags.NoClip );
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			sf.Trimming = StringTrimming.Character;
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( sf != null )
				{
					sf.Dispose();
					sf = null;
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Raises the <see cref="Control.TextChanged"/> event and refreshes the contents of the window.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> with event data.</param>
		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );
			Refresh();
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="pe"/>
		protected override void OnPaint( PaintEventArgs pe )
		{
			Graphics g = pe.Graphics;
			SolidBrush textBrush = new SolidBrush( this.ForeColor );
			g.DrawString( Text, Font, textBrush, ClientRectangle, sf );
		}

		/// <override/>
		/// <summary></summary>
		protected override CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle = ( cp.ExStyle | 0x80 /*WS_EX_TOOLWINDOW*/ );

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

		/// <summary><para>Gets / sets the border style of the control.</para></summary>
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
					if( !Enum.IsDefined( typeof( BorderStyle ), value ) )
					{
						throw new InvalidEnumArgumentException( "value", ( (int)( value ) ), typeof( BorderStyle ) );
					}
					this.borderStyle = value;
					UpdateStyles();
				}
			}
		}

		/// <summary>
		/// Gets / sets the text layout information for the text in the ScrollTip.
		/// </summary>
		public StringFormat Format
		{
			get
			{
				return sf;
			}
			set
			{
				if( sf != value )
				{
					if( sf != null )
					{
						sf.Dispose();
					}
					sf = value;
				}
			}
		}

		/// <summary>
		/// Returns the optimal size for the window to fit the given text.
		/// </summary>
		/// <param name="text">The text that should fit into the window.</param>
		/// <returns>A <see cref="Size"/> with the window size of the ScrollTip.</returns>
		public Size GetPreferredSize( string text )
		{
			Graphics g = CreateGraphics();
			Size size = g.MeasureString( text, Font ).ToSize();
			size.Width += 8;
			size.Height += 8;
			g.Dispose();
			return size;
		}
	}

	/// <summary>
	/// Specifies the current ScrollTip state when a <see cref="ScrollControl.ScrollTip"/> event was raised.
	/// </summary>
	public enum ScrollTipActions
	{
		/// <summary>
		/// The user has grabbed the thumb. The ScrollTip should be shown.
		/// </summary>
		ThumbTrack,

		/// <summary>
		/// The user has released the thumb. The ScrollTip should be hidden.
		/// </summary>
		ThumbPosition,

		/// <summary>
		/// The user is dragging the thumb. The ScrollTip text should be updated.
		/// </summary>
		Scroll
	}

	/// <summary>
	/// Handles the <see cref="ExceptionManager.ExceptionCatched"/> event.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	public delegate void ExceptionCatchedEventHandler( object sender, ExceptionCatchedEventArgs e );

	/// <summary>
	/// Provides data for the <see cref="ExceptionManager.ExceptionCatched"/> event.
	/// </summary>
	public sealed class ExceptionCatchedEventArgs: SyncfusionEventArgs
	{
		/// <summary></summary>
		private Exception ex;

		/// <summary>
		/// Constructs a <see cref="ExceptionCatchedEventArgs"/> object.
		/// </summary>
		/// <param name="ex">The exception that was cached.</param>
		public ExceptionCatchedEventArgs( Exception ex )
		{
			this.ex = ex;
		}

		/// <summary>
		/// Returns the exception that was cached.
		/// </summary>
		[TraceProperty( true )]
		public Exception Exception
		{
			get
			{
				return ex;
			}
		}
	}

	/// <summary>
	/// Handles the ScrollTip event.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	public delegate void ScrollTipFeedbackEventHandler( object sender, ScrollTipFeedbackEventArgs e );

	/// <summary>
	/// Provides data for the <see cref="ScrollControl.ScrollTip"/> event.
	/// </summary>
	public class ScrollTipFeedbackEventArgs: SyncfusionCancelEventArgs
	{
		/// <summary></summary>
		private ScrollBars scrollBars;
		/// <summary></summary>
		private ScrollTipActions action;
		/// <summary></summary>
		private int value;
		/// <summary></summary>
		private string text;
		/// <summary></summary>
		private Size size;
		/// <summary></summary>
		private Point location;
		/// <summary></summary>
		private Font font;
		/// <summary></summary>
		private Color foreColor;
		/// <summary></summary>
		private Color backColor;
		/// <summary></summary>
		private BorderStyle borderStyle;
		/// <summary></summary>
		private StringFormat sf;

		/// <summary></summary>
		/// <param name="scrollBars"/>
		/// <param name="action"/>
		/// <param name="value"/>
		/// <param name="text"/>
		/// <param name="size"/>
		/// <param name="location"/>
		/// <param name="font"/>
		/// <param name="foreColor"/>
		/// <param name="backColor"/>
		/// <param name="borderStyle"/>
		/// <param name="sf"/>
		internal ScrollTipFeedbackEventArgs( ScrollBars scrollBars, ScrollTipActions action,
		  int value, string text, Size size, Point location, Font font, Color foreColor,
		  Color backColor, BorderStyle borderStyle, StringFormat sf )
		{
			this.scrollBars = scrollBars;
			this.action = action;
			this.value = value;
			this.text = text;
			this.size = size;
			this.location = location;
			this.font = font;
			this.foreColor = foreColor;
			this.backColor = backColor;
			this.borderStyle = borderStyle;
			this.sf = sf;
		}

		/// <summary>
		/// Returns the scrollbar that is the source of this event.
		/// </summary>
		[TraceProperty( true )]
		public ScrollBars ScrollBar
		{
			get
			{
				return scrollBars;
			}
		}

		/// <summary>
		/// Returns a <see cref="ScrollTipActions"/> value that specifies the user action that led to this event.
		/// </summary>
		[TraceProperty( true )]
		public ScrollTipActions Action
		{
			get
			{
				return action;
			}
		}

		/// <summary>
		/// Returns the new scroll position.
		/// </summary>
		[TraceProperty( true )]
		public int Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// Gets / sets the text to display in the ScrollTip. You can change this text in your event handler during
		/// a Scroll action.
		/// </summary>
		[TraceProperty( true )]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		/// <summary>
		/// Gets / sets the size of the ScrollTip window. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack action.
		/// </summary>
		[TraceProperty( true )]
		public Size Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
			}
		}

		/// <summary>
		/// Gets / sets the location of the ScrollTip window. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack action.
		/// </summary>
		[TraceProperty( true )]
		public Point Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
			}
		}

		/// <summary>
		/// Gets / sets the font to be used for the ScrollTip text. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack or Scroll action.
		/// </summary>
		[TraceProperty( true )]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
			}
		}

		/// <summary>
		/// Gets / sets the text color to be used for the ScrollTip text. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack or Scroll action.
		/// </summary>
		[TraceProperty( true )]
		public Color ForeColor
		{
			get
			{
				return foreColor;
			}
			set
			{
				foreColor = value;
			}
		}

		/// <summary>
		/// Gets / sets the backcolor to be used for the ScrollTip text. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack or Scroll action.
		/// </summary>
		[TraceProperty( true )]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
			}
		}

		/// <summary>
		/// Gets / sets the border style to be used for the ScrollTip text. You can adjust the scroll window size in your event handler
		/// when handling a ThumbTrack action.
		/// </summary>
		[TraceProperty( true )]
		public BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
			}
		}

		/// <summary>
		/// Gets / sets the text layout information for the text in the ScrollTip.
		/// </summary>
		[TraceProperty( true )]
		public StringFormat Format
		{
			get
			{
				return sf;
			}
			set
			{
				sf = value;
			}
		}
	}

	/// <summary>
	/// BeginUpdateOptions details which drawing operations should be performed during a batch of updates.
	/// </summary>
	[
	Flags,
	]
	public enum BeginUpdateOptions
	{
		/// <summary>
		/// The control suspends any drawing and invalidation and will do a complete refresh when EndUpdate is called.
		/// </summary>
		None=0,
		/// <summary>
		/// Regions that need to be redrawn afterward should be marked invalid by calling the controls Invalidate method.
		/// </summary>
		Invalidate=1,
		/// <summary>
		/// ScrollWindow will scroll the window.
		/// </summary>
		ScrollWindow=2,
		/// <summary>
		/// Scrollbars should be synchronized with the current scroll position.
		/// </summary>
		SynchronizeScrollBars=4,
		/// <summary>
		/// Allows invalidating regions, scrolling and synchronizes the scrollbar thumb.
		/// </summary>
		InvalidateAndScroll=Invalidate | ScrollWindow | SynchronizeScrollBars
	}

	/// <summary>
	/// Contains data for the WindowScrolling and WindowScrolled event.
	/// </summary>
	/// <remarks>
	/// ScrollWindow will raise a WindowScrolling event before it scrolls the window and a WindowScrolled event after the scrolling.
	/// </remarks>
	public sealed class ScrollWindowEventArgs: SyncfusionEventArgs
	{
		/// <summary></summary>
		private int xAmount;
		/// <summary></summary>
		private int yAmount;
		/// <summary></summary>
		private Rectangle rect;
		/// <summary></summary>
		private Rectangle clipRect;
		/// <summary></summary>
		private Rectangle updateRect;

		/// <summary>
		/// Initializes a new <see cref="ScrollWindowEventArgs"/>.
		/// </summary>
		/// <param name="xAmount">The horizontal scroll distance in pixel.</param>
		/// <param name="yAmount">The vertical scroll distance in pixel.</param>
		/// <param name="rect">The bounds of the rectangle that is scrolled.</param>
		/// <param name="clipRect">Clipping rectangle.</param>
		/// <param name="updateRect">The rectangle that was scrolled into view.</param>
		public ScrollWindowEventArgs( int xAmount, int yAmount, Rectangle rect, Rectangle clipRect, Rectangle updateRect )
		{
			this.xAmount = xAmount;
			this.yAmount = yAmount;
			this.rect = rect;
			this.clipRect = clipRect;
			this.updateRect = updateRect;
		}

		/// <summary>
		/// Returns the horizontal scroll distance in pixels.
		/// </summary>
		[TraceProperty( true )]
		public int XAmount
		{
			get
			{
				return xAmount;
			}
		}
		/// <summary>
		/// Returns the vertical scroll distance in pixels.
		/// </summary>
		[TraceProperty( true )]
		public int YAmount
		{
			get
			{
				return yAmount;
			}
		}
		/// <summary>
		/// Returns the bounds of the rectangle that is scrolled.
		/// </summary>
		[TraceProperty( true )]
		public Rectangle Rect
		{
			get
			{
				return rect;
			}
		}
		/// <summary>
		/// Returns the Clipping rectangle.
		/// </summary>
		[TraceProperty( true )]
		public Rectangle ClipRect
		{
			get
			{
				return clipRect;
			}
		}
		/// <summary>
		/// Returns the rectangle that was scrolled into view.
		/// </summary>
		[TraceProperty( true )]
		public Rectangle UpdateRect
		{
			get
			{
				return updateRect;
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Concat(
			  "ScrollWindowEventArgs { xAmount=", xAmount.ToString(),
			  ", yAmount=", yAmount.ToString(),
			  ", rect=", rect.ToString(),
			  ", clipRect=", clipRect.ToString(),
			  ", updateRect=", updateRect.ToString(),
			  "}" );
		}
	}

	/// <summary>
	/// Handles the scroll window event.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	public delegate void ScrollWindowEventHandler( object sender, ScrollWindowEventArgs e );

	/// <summary>
	/// Provides a global hook for exceptions that have been cached inside the framework and gives you
	/// the option to provide specialized handling of the exception. You can also temporarily suspend and resume
	/// caching exceptions.
	/// </summary>
	/// <remarks>
	/// The Syncfusion framework notifies <see cref="ExceptionManager"/> about exceptions that
	/// are cached by calling <see cref="ExceptionManager.RaiseExceptionCatched(object, System.Exception)"/> or <see cref="ExceptionManager"/>.<para/>
	/// The <see cref="ExceptionManager.RaiseExceptionCatched(object, System.Exception)"/> method will raise the <see cref="ExceptionCatched"/>
	/// event. By handling the <see cref="ExceptionCatched"/> event, your code can analyze the exception that was cached
	/// and optionally let it bubble up by rethrowing the exception.<para/>
	/// Your code can also temporarily suspend and resume caching exceptions. This is useful if you want to provide your
	/// own exception handling. Just call <see cref="SuspendCatchExceptions"/> to disable handling exceptions and <see cref="ResumeCatchExceptions"/>
	/// to resume caching exceptions.<para/>
	/// You also have the options to disable caching exceptions altogether by setting <see cref="PassThroughExceptions"/> to True.<para/>
	/// Note: All static settings for this class are thread local.
	/// </remarks>
	/// <example><code lang="C#">
	/// // The following example demonstrates temporarily suspending exception caching when calling a base class version
	/// // of a method.
	///         protected override void OnMouseDown(MouseEventArgs e)
	///             {
	///             ExceptionManager.SuspendCatchExceptions();
	///             try
	///             {
	///                 base.OnMouseDown(e);
	///                 ExceptionManager.ResumeCatchExceptions();
	///             }
	///             catch (Exception ex)
	///             {
	///                 ExceptionManager.ResumeCatchExceptions();
	///                     // Notify exception manager about the catched exception and
	///                     // give it a chance to optionally rethrow the exception if necessary
	///                     // (e.g. if this OnMouseDown was called from another class that
	///                     // wants to provide its own exception handling).
	///                 if (!ExceptionManager.RaiseExceptionCatched(this, ex))
	/// 					throw ex;
	///                 // handle exception here
	///                 MessageBox.Show(ex.ToString());
	///             }
	///         }
	/// </code></example>
	/// <example><code lang="C#">
	/// // This code sample shows how exceptions are handled within the framework:
	///                 try
	///                 {
	///                     CurrentCell.Refresh();
	///                 }
	///                 catch (Exception ex)
	///                 {
	///                     TraceUtil.TraceExceptionCatched(ex);
	///                     if (!ExceptionManager.RaiseExceptionCatched(this, ex))
	/// 						throw ex;
	///                 }
	/// </code></example>
	public class ExceptionManager
	{
		/// <summary></summary>
		[ThreadStatic]
		private static bool passThroughExceptions = false;

		/// <summary></summary>
		[ThreadStatic]
		private static int suspendCatchExceptions = 0;

		/// <summary></summary>
		[ThreadStatic]
		private static EventHandlerList _ehl = null;

		/// <summary></summary>
		private static object onExceptionCatchedKey = new object();

		/// <summary></summary>
		private static EventHandlerList ehl
		{
			get
			{
				if( _ehl == null )
				{
					_ehl = new EventHandlerList();
				}
				return _ehl;
			}
		}

		/// <summary>
		/// Occurs when an exception was cached within the framework and <see cref="ExceptionManager"/> was notified.
		/// </summary>
		public static event ExceptionCatchedEventHandler ExceptionCatched
		{
			add
			{
				ehl.AddHandler( onExceptionCatchedKey, value );
			}
			remove
			{
				ehl.RemoveHandler( onExceptionCatchedKey, value );
			}
		}

		/// <summary>
		/// Lets you disable caching exceptions altogether by setting <see cref="PassThroughExceptions"/> to True.<para/></summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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

		/// <summary>
		/// Temporarily suspends caching exceptions. 
		/// </summary>
		public static void SuspendCatchExceptions()
		{
			suspendCatchExceptions++;
		}

		/// <summary>
		/// Temporarily resumes caching exceptions. 
		/// </summary>
		public static void ResumeCatchExceptions()
		{
			if( suspendCatchExceptions > 0 )
			{
				suspendCatchExceptions--;
			}
		}

		/// <summary>
		/// Indicates whether exceptions should be cached or if they should bubble up. <see cref="RaiseExceptionCatched(object, Syncfusion.Windows.Forms.ExceptionCatchedEventArgs)"/>
		/// calls this method.
		/// </summary>
		/// <returns></returns>
		public static bool ShouldCatchExceptions()
		{
			return suspendCatchExceptions == 0 && !passThroughExceptions;
		}

		/// <overload>
		/// Raises the <see cref="ExceptionCatched"/> event.
		/// </overload>
		/// <summary>
		/// Raises the <see cref="ExceptionCatched"/> event. If caching exceptions has been disabled
		/// by a <see cref="SuspendCatchExceptions"/> call or if <see cref="PassThroughExceptions"/> has been set to True,
		/// the exception is rethrown.
		/// </summary>
		/// <param name="e">A <see cref="ExceptionCatchedEventArgs"/> that contains the event data.</param>
		/// <returns></returns>
		/// <param name="sender"/>
		public static bool RaiseExceptionCatched( object sender, ExceptionCatchedEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.Exception.ToString() );
			}
#endif

			ExceptionCatchedEventHandler eh = (ExceptionCatchedEventHandler)ehl[onExceptionCatchedKey];
			if( eh != null )
			{
				eh( sender, e );
			}

			return ShouldCatchExceptions();
		}

		/// <summary>
		/// Raises the <see cref="ExceptionCatched"/> event. If caching exceptions has been disabled
		/// by a <see cref="SuspendCatchExceptions"/> call or if <see cref="PassThroughExceptions"/> has been set to True,
		/// the exception is rethrown.
		/// </summary>
		/// <param name="ex">A <see cref="Exception"/> that was cached.</param>
		/// <returns></returns>
		/// <param name="sender"/>
		public static bool RaiseExceptionCatched( object sender, Exception ex )
		{
			return RaiseExceptionCatched( sender, new ExceptionCatchedEventArgs( ex ) );
		}
	}

	/// <summary>
	/// Specifies the acceleration behavior for scrollbars.
	/// </summary>
	public enum AccelerateScrollingBehavior
	{
		/// <summary>
		/// Disable scrollbar acceleration.
		/// </summary>
		None,
		/// <summary>
		/// Default, moderate acceleration after the user scrolled 60 increments.
		/// </summary>
		Default,
		/// <summary>
		/// Acceleration after the user scrolled 20 increments.
		/// </summary>
		Fast,
		/// <summary>
		/// Immediate acceleration after the user scrolled 4 increments.
		/// </summary>
		Immediate
	}

	/// <summary>
	/// Specifies the type of autoscrolling, either scrollbar acceleration or mouse dragging outside window bounds.
	/// </summary>
	public enum AutoScrollReason
	{
		/// <summary>
		/// The user held down a scrollbar button.
		/// </summary>
		AccelarateScrollbar,
		/// <summary>
		/// The user has dragged the mouse outside the autoscroll bounds.
		/// </summary>
		MouseDragging,
		/// <summary>
		/// The user is moving the mouse over the control during OLE drag-and-drop operation.
		/// </summary>
		OleDragOver
	}

	/// <summary>
	/// Handles the <see cref="ScrollControl.StartAutoScrolling"/> event.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	public delegate void StartAutoScrollingEventHandler( object sender, StartAutoScrollingEventArgs e );

	/// <summary>
	/// Provides data for the <see cref="ScrollControl.StartAutoScrolling"/> event which is called
	/// when accelerated scrollbar scrolling or mouse drag-scrolling starts.
	/// </summary>
	public sealed class StartAutoScrollingEventArgs: SyncfusionCancelEventArgs
	{
		/// <summary></summary>
		private AutoScrollReason reason;
		/// <summary></summary>
		private int accStartInterval;
		/// <summary></summary>
		private int accStepInterval;
		/// <summary></summary>
		private int accMinInterval;
		/// <summary></summary>
		private int accDelayScrollTimer;
		/// <summary></summary>
		private ScrollBars direction;

		/// <summary></summary>
		/// <param name="reason"/>
		/// <param name="direction"/>
		/// <param name="accStartInterval"/>
		/// <param name="accStepInterval"/>
		/// <param name="accMinInterval"/>
		/// <param name="accDelayScrollTimer"/>
		internal StartAutoScrollingEventArgs( AutoScrollReason reason, ScrollBars direction, int accStartInterval, int accStepInterval, int accMinInterval, int accDelayScrollTimer )
		{
			this.reason = reason;
			this.direction = direction;
			this.accStartInterval = accStartInterval;
			this.accStepInterval = accStepInterval;
			this.accMinInterval = accMinInterval;
			this.accDelayScrollTimer = accDelayScrollTimer;
		}

		/// <summary>
		/// Returns the type of autoscrolling, either scrollbar acceleration or mouse dragging outside window bounds.
		/// </summary>
		[TraceProperty( true )]
		public AutoScrollReason Reason
		{
			get
			{
				return reason;
			}
		}

		/// <summary>
		/// Returns the scroll bar direction: vertical, horizontal, or both.
		/// </summary>
		[TraceProperty( true )]
		public ScrollBars Direction
		{
			get
			{
				return direction;
			}
		}

		/// <summary>
		/// Gets / sets the interval that is initially assigned to the timer for recurring scrolling.
		/// </summary>
		[TraceProperty( true )]
		public int AccStartInterval
		{
			get
			{
				return accStartInterval;
			}
			set
			{
				accStartInterval = value;
			}
		}

		/// <summary>
		/// Gets / sets the interval step that the timer should be decreased from time to time.
		/// </summary>
		[TraceProperty( true )]
		public int AccStepInterval
		{
			get
			{
				return accStepInterval;
			}
			set
			{
				accStepInterval = value;
			}
		}

		/// <summary>
		/// Gets / sets the minimum interval for the fastest scroll speed.
		/// </summary>
		[TraceProperty( true )]
		public int AccMinInterval
		{
			get
			{
				return accMinInterval;
			}
			set
			{
				accMinInterval = value;
			}
		}

		/// <summary></summary>
		internal int AccDelayScrollTimer
		{
			get
			{
				return accDelayScrollTimer;
			}
			set
			{
				accDelayScrollTimer = value;
			}
		}
	}

	/// <summary>
	/// Defines a base class for custom controls that support scrolling behavior.
	/// </summary>
	/// <remarks>
	/// The ScrollControl class acts as a base class for controls that require the
	/// ability to scroll. To allow a control to display scrollbars as needed,
	/// set the AutoScroll property to True. To select which scrollbars should be visible,
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
	/// you want to show. Instead, you can also implement IMouseController and add the object
	/// to MouseControllerDispatcher.<para/></remarks>
	[
	ToolboxItem( false ),
	]
	public class ScrollControl:
	  Control,
	  ICancelModeProvider,
	  ISplitterPaneSupport,
	  IScrollBarWrapperContainer,
	  ISupportUpdating,
	  ISupportIntelliMouse,
	  IQueryFocusInside,
	  INonClientPaintingSupport,
	  IDisposable,
      IVisualStyle 
	{
		#region Class members
		/// <summary></summary>
		private bool hasDoubleBufferSurface = false;
		/// <summary></summary>
		private bool allowIncreaseSmallChange = true;
		/// <summary></summary>
		private bool isPaneClosed = false;
		// Fields
		/// <summary></summary>
		private static object elapsedSemaphor = true;
		/// <summary></summary>
		private static object startstopSemaphor = true;

		/// <summary></summary>
		private int lastMouseTick = int.MinValue;
		/// <summary></summary>
		private ScrollBarWrapper hScrollBar;
		/// <summary></summary>
		private ScrollBarWrapper vScrollBar;
		/// <summary></summary>
		private bool hScroll = false;
		/// <summary></summary>
		private bool vScroll = false;
		/// <summary></summary>
		private Timer repeatScrollEventTimer = null;
		/// <summary></summary>
		private ScrollBars autoScrolling = ScrollBars.None;
		/// <summary></summary>
		private Message repeatScrollMessage;
		/// <summary></summary>
		private ScrollEventType repeatScrollEventType;
		/// <summary></summary>
		private ScrollBarWrapper repeatScrollBar = null;
		/// <summary></summary>
		private bool disableAutoScroll = false;
		/// <summary></summary>
		private int savedHValue = -1;
		/// <summary></summary>
		private int savedVValue = -1;
		/// <summary></summary>
		private bool fillSplitterPane = true;
		/// <summary></summary>
		private Cursor overrideCursor = null;
		//private MouseProcHooker hooker = null;
		/// <summary></summary>
		private Rectangle autoScrollBounds = Rectangle.Empty;
		/// <summary></summary>
		private IntelliMouseDragScroll imm;
		/// <summary></summary>
		private bool disableScrollWindow = false;
		/// <summary></summary>
		private int updateCount;
		/// <summary></summary>
		private bool paintPending;
		/// <summary></summary>
		private bool lockScrollBars = false;
		/// <summary></summary>
		private ScrollControllMouseControllerDispatcher mouseControllerDispatcher = null;
		/// <summary></summary>
		private BeginUpdateOptions updateOptions;
		/// <summary></summary>
		private Point renderOriginPoint = Point.Empty; // see Graphics.RenderingOrigin
		/// <summary></summary>
		private bool useSharedScrollBars = false;
		/// <summary></summary>
		private bool supportsThumbTrack = false;
		/// <summary></summary>
		private BorderStyle borderStyle = BorderStyle.None;
		//		private bool supportsScrollTips = false;
		/// <summary></summary>
		private ScrollTipWindow scrollTip;
		/// <summary></summary>
		private bool isThumbTracking = false;
		/// <summary></summary>
		private ScrollBars thumbBar = ScrollBars.None;
		/// <summary></summary>
		private string scrollTipFormat = " Position {0} ";
		/// <summary></summary>
		private Size insideScrollMargins = new Size( 10, 10 );
		/// <summary></summary>
		private bool ignoreUICues = false;
		/// <summary></summary>
		private Point mousePosition;
		/// <summary></summary>
		private Form wiredParentForm;
		/// <summary></summary>
		private static bool discardPaintMessagesAfterBeginUpdate = false;
		/// <summary></summary>
		private bool inDispose = false;
		/// <summary>
		/// Internal only.
		/// </summary>
		internal long nTimerCount = 0;

		/// <summary>
		/// Internal only.
		/// </summary>
		internal long nDelayScrollTimer = 60;
		/// <summary></summary>
		private ThemedWindowDrawing _themedDrawing = null;
		/// <summary></summary>
		private ThemedScrollBarDrawing _themedScrollBarDrawing = null;
		/// <summary></summary>
		private bool isPaneClosing = false;
		/// <summary></summary>
		private int vScrollIncrement = 1;
		/// <summary></summary>
		private int hScrollIncrement = 1;
		/// <summary></summary>
		private bool isOnMouseWheel = false;
		/// <summary></summary>
		private bool smoothMouseWheelScrolling = true;
		/// <summary></summary>
		private int mouseWheelScrollLines = 0;
		/// <summary></summary>
		private bool inMouseDragScroll = false;
		/// <summary></summary>
		private SizeBox sizeBox;
		/// <internalonly/>
		/// <summary></summary>
		[DocumentationExclude()]
		internal Exception mouseEventException = null;
		/// <summary></summary>
		private IntPtr cachedRgn = IntPtr.Zero;
		/// <summary></summary>
		private int accStartInterval = 700;
		/// <summary></summary>
		private int accStepInterval = 10;
		/// <summary></summary>
		private int accMinInterval = 40;
		/// <summary></summary>
		private int accDelayScrollTimer = 60;
		/// <summary></summary>
		private AccelerateScrollingBehavior accelerateScrolling = AccelerateScrollingBehavior.Default;
		/// <summary></summary>
		private SizeGripStyle showSizeGrip = SizeGripStyle.Auto;
		/// <summary></summary>
		private bool allowSizeGrip = false;
		/// <summary></summary>
		private bool forceSizeBox = false;
		/// <summary></summary>
		private bool smartSizeBox = false;
		// Focus
		/// <summary></summary>
		private bool isActiveControl = false;
		/// <summary></summary>
		private bool isValidating = false;
		/// <summary></summary>
		private bool isDeactivatedCalled = false;
		/// <summary></summary>
		private bool hasControlFocus = false;
		/// <summary></summary>
		private bool isValidated = false;
		/// <summary></summary>
		private bool isMousePressed = false;
		/// <summary></summary>
		private DoubleBufferSurface doubleBufferSurface;
		/// <summary></summary>
		private Rectangle savedBounds;
		/// <summary></summary>
		private bool allowRaiseMouseMoveInOnDragOver = true;
		/// <summary></summary>
		private bool inOleDragOver = false;
		/// <summary></summary>
		private bool onPaintCalled = false;
		/// <summary></summary>
		private bool preJitPaint = false;
		/// <summary></summary>
		private bool m_bUpdatingStyles;

		//
		/// <summary>
		/// The SizeGripStyle behavior has been changed after version 6.1. 
		/// For old behavior set this to true. Default value is false.
		/// </summary>
		public static bool IgnoreSizeGripStyleHideShow = false;
		#endregion

		#region Class properties
		/// <summary>
		/// Indicates whether the time the first time the control is drawn should be optimized
		/// by calling OnPaint before the control is made visible and so that all relevant code for drawing
		/// has been jitted (Just in Time)
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
		/// Disables or specifies the direction for automatic scrolling when the user drags
		/// the mouse cursor out of the scrolling area.
		/// </summary>
		/// <remarks><list type=""><item>ScrollBars.None will disable scrolling.</item><item>ScrollBars.Horizontal will enable horizontal scrolling.</item><item>ScrollBars.Vertical will enable vertical scrolling.</item><item>ScrollBars.Horizontal|ScrollBars.Vertical will enable both horizontal and vertical scrolling.</item></list></remarks>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public ScrollBars AutoScrolling
		{
			get
			{
				return autoScrolling;
			}
			set
			{
				if( value != autoScrolling )
				{
					autoScrolling = value;
					nDelayScrollTimer = accDelayScrollTimer;
					if( autoScrolling == ScrollBars.None )
					{
						StopAutoScrollTimer();
					}
					OnAutoScrollingChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Lets you check or specify the setting if the window should be scrolled when ScrollWindow is called.
		/// </summary>
		/// <remarks><para>If DisableScrollWindow is True, any calls to the ScrollWindow method will simply invalidate the affect region. The rendering origin will
		/// still be recorded correctly and WindowScrolling and WindowScrolled events will be raised.</para><para>If DisableScrollWindow is False, the ScrollWindow will scroll the contents of the control.
		/// </para><para>DisableScrollWindow will return True if BeginUpdate was called without the BeginUpdateOptions.ScrollWindow option.
		/// </para></remarks>
		/// <seealso cref="ScrollControl.BeginUpdate()"/>
		/// <seealso cref="ScrollControl.Updating"/>
		/// <seealso cref="ScrollControl.ScrollWindow"/>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
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
		/// Indicates whether the control should scroll while the user is dragging a scrollbar thumb.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Obsolete( "Use VerticalThumbTrack and HorizontalThumbTrack properties instead." )
		]
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
		/// Indicates whether the control should scroll while the user is dragging a vertical scrollbar thumb.
		/// </summary>
		[
		Browsable( true ),
		Category( "Scrolling" ),
		Description( "Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb." ),
		DefaultValue( false )
		]
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
		/// Indicates whether the control should scroll while the user is dragging a horizontal scrollbar thumb.
		/// </summary>
		[
		Browsable( true ),
		Category( "Scrolling" ),
		Description( "Specifies if the control should scroll while the user is dragging a horizontal scrollbar thumb." ),
		DefaultValue( false )
		]
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
		/// Indicates whether the control should show ScrollTips while the user is dragging a vertical scrollbar thumb.
		/// </summary>
		[
		Browsable( true ),
		Category( "Scrolling" ),
		Description( "Specifies if the control should show ScrollTips while the user is dragging a vertical scrollbar thumb." ),
		DefaultValue( false )
		]
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
		/// Indicates whether the control should show ScrollTips while the user is dragging a horizontal scrollbar thumb.
		/// </summary>
		[
		Browsable( true ),
		Category( "Scrolling" ),
		Description( "Specifies if the control should show ScrollTips while the user is dragging a horizontal scrollbar thumb." ),
		DefaultValue( false )
		]
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
		/// Gets or sets a size grip that should be drawn at the bottom-right corner if both scrollbars
		/// are visible.
		/// </summary>
		[
		DefaultValue( SizeGripStyle.Auto ),
		Category( "Scrolling" ),
		Description( "Specifies if a size grip should be drawn at the bottom-right corner if both scrollbars are visible." )
		]
		public SizeGripStyle SizeGripStyle
		{
			get
			{
				return this.showSizeGrip;
			}
			set
			{
				this.showSizeGrip = value;
			}
		}

		/// <summary>
		/// Indicates whether a size grip can be drawn inside the SizeBox and if the <see cref="Form.SizeGripStyle"/>
		/// of the parent form is allowed to be changed.
		/// </summary>
		[
		DefaultValue( false ),
		Browsable( false ),
		Category( "Scrolling" ),
		Description( "Specifies if a size grip can be drawn inside the SizeBox and if the SizeGripStyle of the parent form is allowed to be changed." ),
		EditorBrowsable( EditorBrowsableState.Never )
		]
		public bool AllowSizeGrip
		{
			get
			{
				return this.allowSizeGrip;
			}
			set
			{
				this.allowSizeGrip = value;
			}
		}

		/// <summary>
		/// Indicates whether the size box should always be drawn when both scrollbars are visible. This
		/// property differs from <see cref="SmartSizeBox"/> such that the control will not
		/// check the docking state and parent form to determine whether to show the size box.
		/// Note: Another better solution is drawing NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </summary>
		/// <remarks>
		/// Showing the size box works around a problem with .NET controls because by
		/// default the area at the bottom right is not drawn and that can cause
		/// drawing glitches. Note: Another better solution is drawing the NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </remarks>
		[
		DefaultValue( false ),
		Browsable( false ),
		Category( "Scrolling" ),
		Description( "Specifies if size box should always be drawn when both scrollbars are visible." ),
		EditorBrowsable( EditorBrowsableState.Never )
		]
		public bool ForceSizeBox
		{
			get
			{
				return this.forceSizeBox;
			}
			set
			{
				this.forceSizeBox = value;
			}
		}

		/// <summary>
		/// Indicates whether the size box should be drawn when both scrollbars are visible
		/// and the control is not a docked window in an MDIChild window. Note: Another better solution is drawing the NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </summary>
		/// <remarks>
		/// Showing the size box works around a problem with .NET controls because by
		/// default the the area at the bottom right is not drawn and that can cause
		/// drawing glitches. Note: Another better solution is drawing NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </remarks>
		[
		DefaultValue( true ),
		Browsable( false ),
		Category( "Scrolling" ),
		Description( "Specifies if size box should be drawn when both scrollbars are visible and the control is not docked in an MDIChild window." ),
		EditorBrowsable( EditorBrowsableState.Never )
		]
		public bool SmartSizeBox
		{
			get
			{
				return this.smartSizeBox;
			}
			set
			{
				this.smartSizeBox = value;
			}
		}

		/// <summary>
		/// Indicates whether the control is currently scrolling and the user drags
		/// the mouse outside the inner scrolling area.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public bool AutoScrollTimerEnable
		{
			get
			{
				return repeatScrollEventTimer != null;
			}
		}

		/// <summary>
		/// Returns a reference to an object with vertical scrollbar settings of the control.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
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
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public ScrollBarWrapper HScrollBar
		{
			get
			{
				return hScrollBar;
			}
		}

		/// <summary>
		/// Internal only. Toggles thumb track mode. See ScrollBarWrapper.IsThumbTracking for a public getter for this state.
		/// </summary>
		internal bool IsThumbTracking
		{
			get
			{
				return isThumbTracking;
			}
			set
			{
				if( isThumbTracking != value )
				{
					isThumbTracking = value;
					if( isThumbTracking )
					{
						ScrollTipFeedbacks();
					}
					else
					{
						HideScrollTips();
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the scroll control can increase the <see cref="ScrollBar.SmallChange"/>
		/// property while doing accelerated scrolling. If this is True, the <see cref="ScrollBar.SmallChange"/>
		/// will be set to 3 after 40 rows and to 5 after 80 rows.
		/// </summary>
		/// <remarks>
		/// By default the scroll control will increase the scrolling step after a while. Set this to
		/// False if you want to enforce scrolling only one row at a time even with accelerated scrolling.
		/// </remarks>
		[
		DefaultValue( true ),
		Category( "Scrolling" ),
		Description( "Specifies if the scroll control can increase the ScrollBar.SmallChange property when doing accelerated scrolling." )
		]
		public virtual bool AllowIncreaseSmallChange
		{
			get
			{
				return allowIncreaseSmallChange;
			}
			set
			{
				allowIncreaseSmallChange = value;
			}
		}

		/// <summary>
		/// Gets or sets the outer scrolling area. Typically the client area of the control.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public Rectangle AutoScrollBounds
		{
			get
			{
				if( autoScrollBounds.IsEmpty )
				{
					return ClientRectangle;
				}
				return autoScrollBounds;
			}
			set
			{
				autoScrollBounds = value;
			}
		}

		/// <summary>
		/// Returns the inside scrolling area. The control will scroll if the user drags
		/// the mouse outside this area.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public virtual Rectangle InsideScrollBounds
		{
			get
			{
				Rectangle r = AutoScrollBounds;
				r.Inflate( -InsideScrollMargins.Width, -InsideScrollMargins.Height );
				return r;
			}
		}

		/// <summary>
		/// Gets or sets the default margins for the scrolling area when the user moves the mouse to the
		/// margin between InsideScrollBounds and AutoScrollBounds.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
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
		/// Returns a reference to the <see cref="ScrollTipWindow"/> for this control. Can be NULL.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public ScrollTipWindow ScrollTip
		{
			get
			{
				return scrollTip;
			}
		}

		/// <summary>
		/// Gets or sets the text to be displayed in the ScrollTip window with a place holder for scroll position . For example, " Position {0} "
		/// </summary>
		[
		DefaultValue( " Position {0} " ),
		Category( "Scrolling" ),
		Description( "The text to be displayed in the ScrollTip window with a place holder for scroll position, e.g. Position {0}." )
		]
		public string ScrollTipFormat
		{
			get
			{
				return scrollTipFormat;
			}
			set
			{
				scrollTipFormat = value;
			}
		}

		/// <summary>
		/// Immediately changes the shown cursor.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		internal Cursor OverrideCursor
		{
			get
			{
				return overrideCursor;
			}
			set
			{
				if( overrideCursor != value )
				{
					overrideCursor = value;

					try
					{
						if( !this.DesignMode && IsHandleCreated )
						{
							NativeMethods.SendMessage( Handle, NativeMethods.WM_SETCURSOR, Handle, (IntPtr)NativeMethods.HTCLIENT );
						}
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
						{
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the control is handling a <see cref="Control.MouseDown"/> event.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsMousePressed
		{
			get
			{
				return isMousePressed;
			}
			set
			{
				this.isMousePressed = value;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="OnValidating"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsValidating
		{
			get
			{
				return isValidating;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="OnValidated"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsValidated
		{
			get
			{
				return isValidated;
			}
		}

		/// <summary>
		/// Indicates whether <see cref="OnEnter"/> has been called. <see cref="OnLeave"/> resets this flag.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsActiveControl
		{
			get
			{
				return isActiveControl;
			}
		}

		/// <summary>
		/// Indicates whether <see cref="OnDeactivated"/> has been called. <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsDeactivated
		{
			get
			{
				return isDeactivatedCalled;
			}
		}

		/// <summary>
		/// Indicates whether <see cref="OnControlGotFocus"/> has been called. <see cref="OnControlLostFocus"/> resets this flag.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool HasControlFocus
		{
			get
			{
				return hasControlFocus;
			}
		}

		/// <summary>
		/// ScrollControlMouseController checks this to see if it should cancel
		/// existing mouse operation and call ScrollControlMouseController.CancelMode
		/// when a UICuesChanged event is sent. That can happen when user activates
		/// another application or simply when styles for a child window have changed.
		/// </summary>
		[
		DocumentationExclude(),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
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
		/// Returns the Pane information.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public virtual string PaneDesc
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				builder.Append( ( this.Parent != null ) ? " " + this.Parent.Name : string.Empty );
				builder.Append( ( this.Name != null && this.Name.Length > 0 ) ? this.Name : this.Text );
				builder.Append( " " );

				if( Parent is IDynamicSplitterFrame )
				{
					IDynamicSplitterFrame sf = (IDynamicSplitterFrame)Parent;
					int row, column;

					if( sf.FindPane( this, out row, out column ) )
					{
						builder.AppendFormat( "P{0}.{1}", row, column );
					}
				}

				builder.Append( ( this.IsActiveControl ) ? "Act" : string.Empty );
				builder.Append( ( this.IsValidating ) ? "Val" : string.Empty );
				builder.Append( ( this.IsValidated ) ? "Vok" : string.Empty );
				builder.Append( ( this.HasControlFocus ) ? "Foh" : string.Empty );
				builder.Append( ( this.QueryFocusInside() ) ? "Foi" : string.Empty );
				builder.Append( ( this.CanFocus ) ? "Cfo" : string.Empty );
				builder.Append( ( this.IsMousePressed ) ? "Mop" : string.Empty );

				return builder.ToString();
			}
		}

		/// <summary>
		/// Lets you override the scroll behavior for rolling the mouse wheel. Default is SystemInformation.MouseWheelScrollLines.
		/// </summary>
		[
		Category( "Scrolling" ),
		Description( "Lets you control scrolling behavior when the user rolls the mouse wheel." )
		]
		public int MouseWheelScrollLines
		{
			get
			{
				return mouseWheelScrollLines > 0 ? mouseWheelScrollLines : SystemInformation.MouseWheelScrollLines;
			}
			set
			{
				mouseWheelScrollLines = value;
			}
		}

		/// <summary>
		/// Indicates whether the control should perform one scroll command (faster) or
		/// if it should perform multiple scroll commands with smaller increments (smoother)
		/// when user rolls mouse wheel.
		/// </summary>
		[
		DefaultValue( true ),
		Category( "Scrolling" ),
		Description( "Lets you control scrolling behavior when the user rolls the mouse wheel." )
		]
		public bool SmoothMouseWheelScrolling
		{
			get
			{
				return smoothMouseWheelScrolling;
			}
			set
			{
				smoothMouseWheelScrolling = value;
			}
		}

		/// <summary>
		/// Gets or sets the multiplier for mouse wheel scrolling.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public virtual int VScrollIncrement
		{
			get
			{
				return this.vScrollIncrement;
			}
			set
			{
				this.vScrollIncrement = value;
			}
		}

		/// <summary>
		/// Gets or sets the multiplier for mouse wheel scrolling.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public virtual int HScrollIncrement
		{
			get
			{
				return this.hScrollIncrement;
			}
			set
			{
				this.hScrollIncrement = value;
			}
		}

		/// <summary>
		/// Enables shared scrollbars. Use this if the control is not embedded in a container control
		/// that implements IScrollBarFrame and you want to provide your own scrollbars.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
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

		/// <summary><para>Gets / sets the border style of the control.</para></summary>
		[
		SRCategory( @"Appearance" ),
		DefaultValue( BorderStyle.None ),
		DispId( -504 /*0xFFFFFE08*/ ),
		Description( @"The border style of the control." )
		]
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
					if( !Enum.IsDefined( typeof( BorderStyle ), value ) )
					{
						throw new InvalidEnumArgumentException( "value", ( (int)( value ) ), typeof( BorderStyle ) );
					}
					this.borderStyle = value;
					UpdateStyles();
				}
			}
		}
		/// <summary><para>
		///      Indicates whether the horizontal scroll bar is visible.
		///    </para></summary>
		/// <value><para><see langword="True"/> if the horizontal scroll bar is
		///       visible; <see langword="False"/> otherwise.
		///    </para></value>
		/// <seealso cref="ScrollableControl.HScroll"/>
		[
		DefaultValue( false ),
		Category( "Scrolling" ),
		RefreshProperties( RefreshProperties.All ),
        Description( "Indicates whether the horizontal scroll bar is visible." )
		]
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
		/// <summary><para>
		///       Indicates whether the vertical scroll bar is visible.
		///    </para></summary>
		/// <value><para><see langword="True"/> if the vertical scroll bar is
		///       visible; <see langword="False"/> otherwise.
		///    </para></value>
		/// <seealso cref="ScrollableControl.HScroll"/>
		[
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.All ),
		Category( "Scrolling" ),
        Description( "Indicates whether the vertical scroll bar is visible." )
		]
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
		/// Indicates whether the splitter control has closed the pane with this control.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsSplitterPaneClosed
		{
			get
			{
				return isPaneClosed;
			}
		}

		/// <summary>
		/// MouseControllerDispatcher coordinates mouse events among competing mouse controllers. Based on
		/// the position of the mouse and context of the control every registered controller's HitTest method
		/// is called to determine the best controller for the following mouse action. This controller will then
		/// receive mouse events.
		/// </summary>
		/// <remarks>
		/// See <see cref="Syncfusion.Windows.Forms.MouseControllerDispatcher"/> for more information.
		/// </remarks>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public MouseControllerDispatcher MouseControllerDispatcher
		{
			get
			{
				// Will get instantiated on demand when user tries to add IMouseControllers.
				if( mouseControllerDispatcher == null && !this.IsDisposed )
				{
					mouseControllerDispatcher = new ScrollControllMouseControllerDispatcher( this );
				}
				return mouseControllerDispatcher;
			}
		}

		/// <summary>
		/// Returns a reference to the active mouse controller that is receiving MouseDown, MouseMove, MouseUp messages when the user
		/// has pressed a mouse button.
		/// </summary>
		/// <seealso cref="MouseControllerDispatcher"/>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public IMouseController ActiveController
		{
			get
			{
				return MouseControllerDispatcher.ActiveController;
			}
		}

		/// <summary></summary>
		bool ISplitterPaneSupport.FillSplitterPane
		{
			get
			{
				return this.FillSplitterPane;
			}
		}

		/// <summary>
		/// Toggles support for using the control inside a dynamic splitter window and sharing scrollbars
		/// with the parent window.
		/// </summary>
		[
		RefreshProperties( RefreshProperties.All ),
		Description( "Toggles support for using the control inside a dynamic splitter window and sharing scrollbars with the parent window." ),
		Category( "Scrolling" )
		]
		public bool /*ISplitterPaneSupport*/ FillSplitterPane
		{
			get
			{
				return fillSplitterPane;
			}
			set
			{
				if( value != fillSplitterPane )
				{
					if( VScroll || HScroll )
					{
						this.SetVisibleScrollbars( false, false );
					}
					fillSplitterPane = value;

					try
					{
						OnFillSplitterPaneChanged( EventArgs.Empty );
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
						{
							throw;
						}
					}

				}
			}
		}

		/// <summary>
		/// Indicates whether the splitter control is closing the pane with this control.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsSplitterPaneClosing
		{
			get
			{
				return isPaneClosing;
			}
		}

		/// <summary>
		/// Toggles support for Intelli-Mouse panning. When the user presses the middle mouse button and drags the mouse,
		/// the window will scroll.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Toggles support for Intelli-Mouse panning." ),
		Category( "Scrolling" )
		]
		public bool EnableIntelliMouse
		{
			get
			{
				return imm != null && imm.Enabled;
			}
			set
			{
				if( value != EnableIntelliMouse )
				{
					if( imm == null )
					{
						imm = new IntelliMouseDragScroll( this, true );
						imm.AllowScrolling = ScrollBars.Both;
						imm.DragScroll += new IntelliMouseDragScrollEventHandler( IntelliMouseDragScrollEvent );
					}
					imm.Enabled = value;
				}
			}
		}

		/// <summary></summary>
		private ThemedWindowDrawing themedDrawing
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsAppThemed )
				{
					if( _themedDrawing == null )
					{
						_themedDrawing = new ThemedWindowDrawing( this );
					}
				}
				return this._themedDrawing;
			}
		}

		/// <summary></summary>
		private ThemedScrollBarDrawing themedScrollBarDrawing
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsAppThemed )
				{
					if( _themedScrollBarDrawing == null )
					{
						this._themedScrollBarDrawing = new ThemedScrollBarDrawing( this );
					}
				}
				return this._themedScrollBarDrawing;
			}
		}

		/// <summary>
		/// When you call BeginUpdate(), the control by default does not handle WM_PAINT messages. Only
		/// once you call EndUpdate they will be processed. If this causes problems in your application, you can
		/// set this static property to True. In such cases, WM_PAINT messages will be simply discarded and
		/// any invalid regions will be validated.
		/// </summary>
		/// <remarks>
		/// There is a problem with the default implementation of BeginUpdate. If a screen region is marked
		/// invalid, the WndProc will be repeatedly called with WM_PAINT at the the top of the WndProc
		/// until EndUpdate is called. This can cause your application to freeze if another window gets created
		/// or if you make a web service call and WndProc messages need to be processed.<para/>
		/// Setting DiscardPaintMessagesAfterBeginUpdate to True will help avoid these scenarios.
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

		/// <summary>
		/// Returns true if object is executing <see cref="Dispose(bool)"/> method call.
		/// </summary>
		[
		XmlIgnore,
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool IsDisposing
		{
			get
			{
				return inDispose;
			}
		}

		/// <summary>
		/// Property MousePosition (Point) - cached Control.MousePosition. The variable is set
		/// before any WM_MOUSE* messages being processed.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
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
		/// Gets or sets the acceleration behavior for scrollbars.
		/// </summary>
		[
		DefaultValue( AccelerateScrollingBehavior.Default ),
		Category( "Scrolling" ),
		Description( "Specifies the acceleration behavior for scrollbars." )
		]
		public AccelerateScrollingBehavior AccelerateScrolling
		{
			get
			{
				return accelerateScrolling;
			}
			set
			{
				if( accelerateScrolling != value )
				{
					accelerateScrolling = value;
					OnAccelerateScrollingChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Indicates whether OnMouseMove should be called from OnDragOver.
		/// </summary>
		[
		XmlIgnore,
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool AllowRaiseMouseMoveInOnDragOver
		{
			get
			{
				return allowRaiseMouseMoveInOnDragOver;
			}
			set
			{
				allowRaiseMouseMoveInOnDragOver = value;
			}
		}

		// Size is overridden here to prevent Code Generation in Designer.
		/// <override/>
		/// <summary>
		/// Gets or sets the control's size. Size is overridden here to prevent Code Generation in Designer.
		/// </summary>
		[Category( "Layout" )]
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
		// Size is overridden here to prevent Code Generation in Designer.
		/// <override/>
		/// <summary>
		/// Gets or sets the control's bounds. Size is overridden here to prevent Code Generation in Designer.
		/// </summary>
		[Browsable( false )]
		public new Rectangle Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				base.Bounds = value;
			}
		}

		// TabIndex is overridden here to prevent Code Generation in Designer.
		/// <override/>
		/// <summary>Gets or sets the TabIndex.
		/// TabIndex is overridden here to prevent Code Generation in Designer.</summary>
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

		// Child Focus
		// Excel Selection Frame
		// Scrollbars

		/// <summary>
		/// Returns the settings for the current BeginUpdate option.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public virtual BeginUpdateOptions UpdateOptions
		{
			get
			{
				return updateOptions;
			}
		}

		/// <summary></summary>
		bool ISupportUpdating.Updating
		{
			get
			{
				return Updating;
			}
		}

		/// <summary>
		/// Indicates whether BeginUpdate() has been called and the painting for a control is suspended.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public bool Updating
		{
			get
			{
				return updateCount > 0;
			}
		}

		/// <summary>
		/// Indicates whether there are updates pending for the control when painting is suspended by BeginUpdate.
		/// </summary>
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Advanced ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public bool PaintPending
		{
			get
			{
				// see also WndProc (WM_PAINT)
				return paintPending;
			}
		}

		/// <summary>
		/// Determines if EnableDoubleBufferSurface method was called and double buffering
		/// using the DoubleBufferSurface is enabled. The DoubleBufferSurface buffering
		/// is different from the automatic .NET double buffering and also a bit slower but
		/// it reduces flicker if lots of scrolling is used and gdi drawing for individual cells
		/// in a grid.
		/// </summary>
		[Browsable( false )]
		public bool HasDoubleBufferSurface
		{
			get
			{
				return hasDoubleBufferSurface;
			}
		}

		/// <summary>
		/// Returns the DoubleBufferSurface if EnableDoubleBufferSurface() was called. Otherwise
		/// the method returns null.
		/// </summary>
		[Browsable( false )]
		public DoubleBufferSurface DoubleBufferSurface
		{
			get
			{
				if( !this.hasDoubleBufferSurface )
				{
					return null;
				}

				if( Bounds == savedBounds && doubleBufferSurface != null )
				{
					return doubleBufferSurface;
				}

				// Need to recreate buffer if size of control was changed.
				savedBounds = Bounds;

				if( doubleBufferSurface != null )
				{
					doubleBufferSurface.Dispose();
				}

				// Do not dispose the graphics object (gr). It will be attached to 
				// DoubleBufferSurface object and continue to be used from that
				// surface object after this method returns.
                Graphics gr = base.CreateGraphics();
                OnPrepareDoubleBufferSurfaceGraphics(gr);

                // Creates a BufferedGraphics instance associated with Form1, and with 
                // dimensions the same size as the drawing surface of Form1.
                doubleBufferSurface = new DoubleBufferSurface(gr, this.DisplayRectangle);

				return doubleBufferSurface;
			}
		}

		/// <summary>
		/// Indicates whether <see cref="Control.UpdateStyles()"/> is internally called.
		/// </summary>
		protected bool UpdatingStyles
		{
			get
			{
				return m_bUpdatingStyles;
			}
		}

		#endregion

		#region Class events
		/// <summary>
		/// Occurs when both <see cref="OnControlLostFocus"/> and <see cref="OnLeave"/> occur.
		/// </summary>
		[Category( "Focus" ), Description( "Occurs when both OnControlLostFocus and OnLeave occur." )]
		public event EventHandler Deactivated;
		/// <summary>
		/// Occurs when the <see cref="AccelerateScrolling"/> property has been changed.
		/// </summary>
		[Category( "Scrolling" ), Description( "Occurs when the AccelerateScrolling property has been changed." )]

		public event EventHandler AccelerateScrollingChanged;
		/// <summary>
		/// Occurs when vertical scrollbar is moved.
		/// </summary>
		[Category( "Scrolling" ), Description( "Occurs when vertical scrollbar is moved." )]
		public event ScrollEventHandler VerticalScroll;
		/// <summary>
		/// Occurs when horizontal scrollbar is moved.
		/// </summary>
        [Category( "Scrolling" ), Description("Occurs when horizontal scrollbar is moved.")]
		public event ScrollEventHandler HorizontalScroll;
		/// <summary>
		/// Occurs when the user presses the mouse wheel and drags the mouse.
		/// </summary>
		/// <remarks>
		/// Set <see cref="IntelliMouseDragScrollEventArgs.Scrolled"/> of <see cref="IntelliMouseDragScrollEventArgs"/>
		/// to True if you provide customized scrolling in your event handler.
		/// </remarks>
		[Category( "Scrolling" ), Description( "Occurs when the user presses the mouse wheel and drags the mouse." )]

		public event IntelliMouseDragScrollEventHandler IntelliMouseDragScrolling;
		/// <summary>
		/// The ShowContextMenu event occurs when the user right-clicks inside
		/// the control.
		/// </summary>
		/// <remarks>
		/// <para/>
		/// You can cancel showing a content menu when
		/// you assign True to <see cref="CancelEventArgs.Cancel"/>.
		/// <para/>
		/// </remarks>
        [Category( "Scrolling" ), Description("The ShowContextMenu event occurs when the user right-clicks inside the control.")]
		public event ShowContextMenuEventHandler ShowContextMenu;
		/// <summary>
		/// Occurs when accelerated scrollbar scrolling or mouse drag-scrolling starts.
		/// </summary>
		[
		Description( "Occurs when accelerated scrollbar scrolling or mouse drag-scrolling starts." ),
		Category( "Scrolling" )
		]
		public event StartAutoScrollingEventHandler StartAutoScrolling;
		/// <summary>
		/// Occurs when the splitter control has closed the pane with this control.
		/// </summary>
		[
		Description( "Occurs when the splitter control has closed the pane with this control." ),
		Category( "Scrolling" )
		]
		public event EventHandler SplitterPaneClosed;
		/// <summary>
		/// Occurs when the splitter control is closing the pane with this control.
		/// </summary>
		[
		Description( "Occurs when the splitter control is closing the pane with this control." ),
		Category( "Scrolling" )
		]
		public event EventHandler SplitterPaneClosing;
		/// <summary>
		/// Occurs when the user is dragging the scrollbar thumb.
		/// </summary>
		[
		Description( "Occurs when the user is dragging the scrollbar thumb." ),
		Category( "Scrolling" )
		]
		public event ScrollTipFeedbackEventHandler ScrollTipFeedback;
		/// <summary>
		/// Occurs when scrollbars are hidden or shown.
		/// </summary>
		[
		Description( "Occurs when scrollbars are hidden or shown." ),
		Category( "Scrolling" )
		]
		public event EventHandler ScrollbarsVisibleChanged;
		/// <summary>
		/// Occurs when the user holds the Control Key and rolls the mouse wheel.
		/// </summary>
		[
		Description( "Occurs when the user holds the Control Key and rolls the mouse wheel." ),
		Category( "Scrolling" )
		]
		public event MouseWheelZoomEventHandler MouseWheelZoom;
		/// <summary>
		/// Occurs when the <see cref="ScrollControl.FillSplitterPane"/> value has changed.
		/// </summary>
		[
		Description( "Occurs when the FillSplitterPane value has changed." ),
		Category( "Scrolling" )
		]
		public event EventHandler FillSplitterPaneChanged;
		/// <summary>
		/// Occurs when <see cref="ScrollControl.BeginUpdate(BeginUpdateOptions)"/> has been called the first time or <see cref="ScrollControl.EndUpdate(bool)"/>
		/// has been called the last time.
		/// </summary>
		[
		Description( " Occurs when BeginUpdate has been called the first time or EndUpdate has been called the last time." ),
		Category( "Behavior" ),
		Browsable( false )
		]
		public event EventHandler UpdatingChanged;
		/// <summary>
		/// Occurs after the window has been scrolled.
		/// </summary>
		[
		Description( "Occurs after the window has been scrolled." ),
		Category( "Scrolling" ),
		Browsable( false )
		]
		public event ScrollWindowEventHandler WindowScrolled;
		/// <summary>
		/// Occurs while the window is being scrolled.
		/// </summary>
		[
		Description( "Occurs while the window is being scrolled." ),
		Category( "Scrolling" ),
		Browsable( false )
		]
		public event ScrollWindowEventHandler WindowScrolling;
		/// <summary>
		/// Occurs before a <see cref="Control.MouseDown"/> is raised and allows you to cancel the mouse event.
		/// </summary>
		[
		Description( "Occurs before a MouseDown event is raised and allows you to cancel the mouse event." ),
		Category( "Behavior" )
		]
		public event CancelMouseEventHandler ScrollControlMouseDown;
		/// <summary>
		/// Occurs after a <see cref="Control.MouseDown"/> is raised.
		/// </summary>
		[
		Description( "Occurs after a MouseDown is raised." ),
		Category( "Behavior" )
		]
		public event MouseEventHandler ScrollControlHandledMouseDown;
		/// <summary>
		/// Occurs before a <see cref="Control.MouseMove"/> is raised and allows you to cancel the mouse event.
		/// </summary>
		[
		Description( "Occurs before a MouseMove is raised and allows you to cancel the mouse event." ),
		Category( "Behavior" )
		]
		public event CancelMouseEventHandler ScrollControlMouseMove;
		/// <summary>
		/// Occurs after a <see cref="Control.MouseMove"/> event is raised and after auto-scrolling.
		/// </summary>
		[
		Description( "Occurs after a MouseMovet event is raised and after auto-scrolling." ),
		Category( "Behavior" )
		]
		public event MouseEventHandler ScrollControlHandledMouseMove;
		/// <summary>
		/// Occurs before a <see cref="Control.MouseUp"/> is raised and allows you to cancel the mouse event.
		/// </summary>
		[
		Description( "Occurs before a MouseUp is raised and allows you to cancel the mouse event." ),
		Category( "Behavior" )
		]
		public event CancelMouseEventHandler ScrollControlMouseUp;
		/// <summary>
		/// Occurs after a <see cref="Control.MouseUp"/> event is raised.
		/// </summary>
		[
		Description( "Occurs after a MouseUp event is raised." ),
		Category( "Behavior" )
		]
		public event MouseEventHandler ScrollControlHandledMouseUp;
		/// <summary>
		/// Occurs when the window receives a WM_CANCELMODE message.
		/// </summary>
		/// <remarks>
		/// WM_CANCELMODE is sent to cancel certain modes, such as mouse capture.
		/// For example, the system sends this message to the active window when a
		/// dialog box or message box is displayed. Certain functions also send this
		/// message explicitly to the specified window regardless of whether it is the
		/// active window. For example, the EnableWindow function sends this message
		/// when disabling the specified window.
		/// </remarks>
		[
		Description( "Occurs when the window receives a WM_CANCELMODE message." ),
		Category( "Behavior" )
		]
		public event EventHandler CancelMode;
		/// <summary>
		/// Occurs when the AutoScrolling property is changed.
		/// </summary>
		/// <remarks>
		/// If you want to prevent autoscrolling, you should handle this event
		/// and reset the AutoScrolling property to ScrollBars.None.
		/// </remarks>
		[
		Description( "Occurs when AutoScrolling property is changed." ),
		Category( "Behavior" )
		]
		public event EventHandler AutoScrollingChanged;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes a new instance of <see cref="ScrollControl"/>.
		/// </summary>
		public ScrollControl()
		{
			//Switches.ScrollControl.Level = TraceLevel.Verbose; //.TraceVerbose
			imm = new IntelliMouseDragScroll( this, true );
			imm.AllowScrolling = ScrollBars.Both;
			imm.DragScroll += new IntelliMouseDragScrollEventHandler( IntelliMouseDragScrollEvent );

			hScrollBar = new ScrollBarWrapper( this, ScrollBars.Horizontal );
			vScrollBar = new ScrollBarWrapper( this, ScrollBars.Vertical );
			//vScrollBar.SupportsThumbTrack = true;
			//hScrollBar.SupportsThumbTrack = true;
			WireScrollEvents();
		}

		/// <summary>
		/// Disposes the control.
		/// </summary>
		public new void Dispose()
		{
			inDispose = true;
			( (Component)this ).Dispose();
			inDispose = false;
			//isDisposed = true;
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( doubleBufferSurface != null )
				{
					doubleBufferSurface.Dispose();
				}

				if( repeatScrollEventTimer != null )
				{
					repeatScrollEventTimer.Tick -= new EventHandler( repeatScrollEventTimer_Elapsed );
					repeatScrollEventTimer.Dispose();
					repeatScrollEventTimer = null;
				}
				repeatScrollBar = null;
				overrideCursor = null;

				if( wiredParentForm != null )
				{
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				}
				wiredParentForm = null;

				if( mouseControllerDispatcher != null )
				{
					mouseControllerDispatcher.Dispose();
					mouseControllerDispatcher = null;
				}

				UnwireScrollEvents();

				if( mouseControllerDispatcher != null )
				{
					mouseControllerDispatcher.Dispose();
					mouseControllerDispatcher = null;
				}
				if( this._themedDrawing != null )
				{
					this._themedDrawing.Dispose();
					this._themedDrawing = null;
				}
				if( this._themedScrollBarDrawing != null )
				{
					this._themedScrollBarDrawing.Dispose();
					this._themedScrollBarDrawing = null;
				}
				if( this.sizeBox != null )
				{
					this.Controls.Remove( sizeBox );
					sizeBox.Dispose();
					sizeBox = null;
				}

				if( scrollTip != null )
				{
					scrollTip.Dispose();
					scrollTip = null;
				}

				if( this.imm != null )
				{
					imm.DragScroll -= new IntelliMouseDragScrollEventHandler( IntelliMouseDragScrollEvent );
					imm.Dispose();
					imm = null;
				}

				if( cachedRgn != IntPtr.Zero )
				{
					NativeMethods.DeleteObject( cachedRgn );
					this.cachedRgn = IntPtr.Zero;
				}

			}
			base.Dispose( disposing );
		}
		#endregion

		#region Class codeDom serialization
		/// <summary>
		/// Raises a CancelMode for the active mouse controller for this control.
		/// </summary>
		/// <remarks><see cref="ScrollControl.MouseControllerDispatcher"/> holds a collection of mouse controllers.
		/// </remarks>
		public void ResetMouseController()
		{
			if( mouseControllerDispatcher != null )
			{
				mouseControllerDispatcher.ProcessCancelMode();
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeFillSplitterPane()
		{
			if( GetScrollBarFrameOfComponent( this ) == null )
			{
				return false;
			}

			return this.FillSplitterPane != false;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeBorderStyle()
		{
			return !this.FillSplitterPane;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeHScroll()
		{
			return !this.FillSplitterPane;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeVScroll()
		{
			return !this.FillSplitterPane;
		}

		/// <summary>
		/// Indicates whether serialize <see cref="MouseWheelScrollLines"/> property value or not.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeMouseWheelScrollLines()
		{
			return mouseWheelScrollLines > 0;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeInsideScrollMargins()
		{
			return insideScrollMargins.Width != 10 || insideScrollMargins.Height != 10;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeSize()
		{
			return !this.FillSplitterPane;
		}

		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeTabIndex()
		{
			return !this.FillSplitterPane;
		}
		#endregion

		#region Class static methods
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="n"/>
		internal static int HIWORD( int n )
		{
			return ( ( n >> 16 ) & 0xffff );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="low"/>
		/// <param name="high"/>
		private static int MAKELPARAM( int low, int high )
		{
			return ( ( high << 16 ) | ( low & 0xffff ) );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="n"/>
		private static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="n"/>
		private static int LOWORD( IntPtr n )
		{
			return LOWORD( (int)n );
		}
		#endregion

		#region Class Public Methods
		/// <exclude/>
		/// <summary>
		/// Offsets the <see cref="Graphics.RenderingOrigin"/> point.
		/// </summary>
		/// <param name="x"/>
		/// <param name="y"/>
		public void OffsetRenderOriginPoint( int x, int y )
		{
			renderOriginPoint.X += x;
			renderOriginPoint.Y += y;
		}

		/// <summary>
		/// Returns PointToClient(LastMousePosition).
		/// </summary>
		/// <returns></returns>
		public Point LastMousePositionToClient()
		{
			return PointToClient( this.mousePosition );
		}

		/// <overload>
		/// Returns a value indicating the context at a given mouse position.
		/// </overload>
		/// <summary>
		/// Returns a value indicating the context at a given mouse position.
		/// </summary>
		/// <param name="point">The mouse position in client coordinates.</param>
		/// <returns>A <see cref="System.Int32"/> value indicating the context at a given mouse position; zero if no context found.</returns>
		/// <remarks>
		/// Any Mouse Controller needs to implement the IMouseController interface.<para/>
		/// In its implementation of MouseController.HitTest the mouse controller determines whether it
		/// wants to handle the mouse events for the current mouse position.<para/>
		/// MouseControllerDispatcher will call HitTest for each Mouse Controller that has been registered with
		/// Add(IMouseController). The Mouse Controller that wins the vote will be returned together with the
		/// context value its HitTest implementation returned.<para/></remarks>
		/// <seealso cref="MouseControllerDispatcher"/>
		public int HitTest( Point point )
		{
			IMouseController mc;
			return MouseControllerDispatcher.HitTest( point, MouseButtons.Left, 1, out mc );
		}

		/// <summary>
		/// Returns a value indicating the context at a given mouse position.
		/// </summary>
		/// <param name="point">The mouse position in client coordinates.</param>
		/// <param name="mouseButton">Indicates which mouse button was pressed.</param>
		/// <returns>A <see cref="System.Int32"/> value indicating the context at a given mouse position; 0 if no context found.</returns>
		/// <genoverload/>
		public int HitTest( Point point, MouseButtons mouseButton )
		{
			IMouseController mc;
			return MouseControllerDispatcher.HitTest( point, mouseButton, 1, out mc );
		}

		/// <summary>
		/// Returns a value indicating the context at a given mouse position.
		/// </summary>
		/// <param name="point">The mouse position in client coordinates.</param>
		/// <param name="mouseButton">Indicates which mouse button was pressed.</param>
		/// <param name="controller">A place holder where the controller is returned that won the vote.</param>
		/// <returns>A <see cref="System.Int32"/> value indicating the context at a given mouse position; 0 if no context found.</returns>
		/// <genoverload/>
		public int HitTest( Point point, MouseButtons mouseButton, out IMouseController controller )
		{
			return MouseControllerDispatcher.HitTest( point, mouseButton, 1, out controller );
		}

		/// <summary>
		/// Returns a value indicating the context at a given mouse position.
		/// </summary>
		/// <param name="point">The mouse position in client coordinates.</param>
		/// <param name="mouseButton">Indicates which mouse button was pressed.</param>
		/// <param name="clicks">Specifies the number of times the mouse button was pressed and released.</param>
		/// <param name="controller">A place holder where the controller is returned that won the vote.</param>
		/// <returns>A <see cref="System.Int32"/> value indicating the context at a given mouse position; 0 if no context found.</returns>
		/// <genoverload/>
		public int HitTest( Point point, MouseButtons mouseButton, int clicks, out IMouseController controller )
		{
			return MouseControllerDispatcher.HitTest( point, mouseButton, clicks, out controller );
		}
		#endregion

		#region WndProc
		/// <summary>
		/// Overridden. Changes <see cref="System.Windows.Forms.CreateParams.Style"/> to show or hide scrollbars and also consider the control's
		/// <see cref="ScrollControl.BorderStyle"/> setting.
		/// </summary>
		protected override /*Control*/ CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
			get
			{
				CreateParams cp = base.CreateParams;
				if( this.HScroll && this.GridOfficeScrollBars == OfficeScrollBars.None && !this.MetroScrollBars )
				{
					cp.Style = ( cp.Style | NativeMethods.WS_HSCROLL );
				}
				else
				{
					cp.Style = ( cp.Style & ~NativeMethods.WS_HSCROLL );
				}

                if( this.VScroll && this.GridOfficeScrollBars == OfficeScrollBars.None && !this.MetroScrollBars )
				{
					cp.Style = ( cp.Style | NativeMethods.WS_VSCROLL );
				}
				else
				{
					cp.Style = ( cp.Style & ~NativeMethods.WS_VSCROLL );
				}

				if( this.RightToLeft == RightToLeft.Yes )
				{
					cp.ExStyle = ( cp.ExStyle | NativeMethods.WS_EX_RTLREADING );
				}
				else
				{
					cp.ExStyle = ( cp.ExStyle & ~NativeMethods.WS_EX_RTLREADING );
				}

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

		/// <override/>
		/// <summary></summary>
		/// <param name="msg"/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message msg )
		{
			switch( msg.Msg )
			{
#if CTLCOLOR
        case 0x0019 /*NativeMethods.WM_CTLCOLOR*/:
        case 0x2019 /*NativeMethods.WM_CTLCOLOR*/:
          if( HScrollBar.IsReflect )
          {
            HScrollBar.WmReflectScroll( ref msg );
          }
          if( VScrollBar.IsReflect )
          {
            VScrollBar.WmReflectScroll( ref msg );
          }
          break;
#endif
				case NativeMethods.WM_NCCALCSIZE:
				this.WmNcCalcSize( ref msg );
				break;
				case NativeMethods.WM_NCPAINT:
				this.WmNcPaint( ref msg );
				break;
				case NativeMethods.WM_HSCROLL:
				this.WmHScroll( ref msg );
				break;
				case NativeMethods.WM_VSCROLL:
				this.WmVScroll( ref msg );
				break;
				case NativeMethods.WM_SETCURSOR:
				WmSetCursor( ref msg );
				break;
				case NativeMethods.WM_CANCELMODE:
				OnCancelMode( EventArgs.Empty );
				goto default;
				case NativeMethods.WM_KILLFOCUS:
				if( updateCount > 0 )
				{
					while( updateCount > 1 )
					{
						EndUpdate();
					}
				}
				goto default;
				/*case NativeMethods.WM_SHOWWINDOW:
					  base.WndProc(ref msg);
					  OnVisibleChanged(this, EventArgs.Empty);
					  break;*/
				case NativeMethods.WM_PAINT:
				paintPending = ( updateCount > 0 && msg.HWnd == this.Handle );
				if( !paintPending )
				{
					WmPaint( ref msg );
				}
				else
				{
					if( ScrollControl.DiscardPaintMessagesAfterBeginUpdate )
					{
						NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
						NativeMethods.BeginPaint( Handle, ref ps );
						NativeMethods.EndPaint( Handle, ref ps );
						msg.Result = IntPtr.Zero;
					}

					//						TraceUtil.TraceCurrentMethodInfo("PaintPending", paintPending, msg, Handle);
					if( IsValidating )
					{
						//							TraceUtil.TraceCurrentMethodInfo("CancelUpdate", paintPending, msg, Handle);
						CancelUpdate();
						paintPending = false;
						WmPaint( ref msg );
					}

				}
				break;
				case NativeMethods.WM_KEYDOWN:
				if( this.mouseControllerDispatcher != null )
				{
					// why Keys.Alt ???
					if( ( (Keys)(int)msg.WParam ) == Keys.Escape || Control.ModifierKeys == Keys.Alt )
					{
						mouseControllerDispatcher.ProcessCancelMode();
					}
				}
				base.WndProc( ref msg );
				break;

				case 513 /*0x201 WM_LBUTTONDOWN*/:
				case 519 /*0x207 WM_MBUTTONDOWN*/:
				case 523 /*0x20b WM_XBUTTONDOWN*/:
				case 518 /*0x206 WM_RBUTTONDBLCLK*/:
				case 516 /*0x204 WM_RBUTTONDOWN*/:
				case 515 /*0x203 WM_LBUTTONDBLCLK*/:
				case 521 /*0x209 WM_MBUTTONDBLCLK*/:
				case 525 /*0x20d WM_XBUTTONDBLCLK*/:
				mousePosition = Control.MousePosition;
				WmMouseDown( ref msg );
				break;

				case 514 /*0x202 WM_LBUTTONUP*/:
				case 520 /*0x208 WM_MBUTTONUP*/:
				case 524 /*0x20c WM_XBUTTONUP*/:
				case 517 /*0x205 WM_RBUTTONUP*/:
				mousePosition = Control.MousePosition;
				WmMouseUp( ref msg );
				break;

				case NativeMethods.WM_MOUSEMOVE:
				case NativeMethods.WM_MOUSEHOVER:
				mousePosition = Control.MousePosition;
				if( this.inMouseDragScroll || repeatScrollEventTimer == null )
				{
					base.WndProc( ref msg );
				}
				break;

				case NativeMethods.WM_CONTEXTMENU: // 0x7b
				{
					Point pt = new Point( NativeMethods.LOWORD( msg.LParam ), NativeMethods.HIWORD( msg.LParam ) );
					ShowContextMenuEventArgs e = new ShowContextMenuEventArgs( pt );
					OnShowContextMenu( e );
					if( e.Cancel )
					{
						return;
					}
					else
					{
						goto default;
					}
				}
				case NativeMethods.WM_NCHITTEST:
				base.WndProc( ref msg );
				if( msg.Result.ToInt32() == NativeMethods.HTBOTTOMRIGHT && SizeGripStyle == SizeGripStyle.Hide
                        && !IgnoreSizeGripStyleHideShow )
				{
					msg.Result = new IntPtr( NativeMethods.HTGROWBOX );
				}
				break;
				case NativeMethods.WM_SIZE:
				base.WndProc( ref msg );
				if( !IgnoreSizeGripStyleHideShow )
					this.WmNcPaint( ref msg );
				break;
				default:
				base.WndProc( ref msg );
				break;
			}
		}

		/// <summary></summary>
		/// <param name="msg"/>
		private void WmNcCalcSize( ref Message msg )
		{
			if( IntPtr.Zero == msg.WParam )
			{
				NativeMethods.RECT rect = (NativeMethods.RECT)msg.GetLParam( typeof( NativeMethods.RECT ) );

				Rectangle rc = Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
				OnNcCalcSize( ref rc );
				rect = new NativeMethods.RECT( rc );

				Marshal.StructureToPtr( rect, msg.LParam, false );
			}
			else
			{
				NativeMethods.NCCALCSIZE_PARAMS csp = (NativeMethods.NCCALCSIZE_PARAMS)msg.GetLParam(
				  typeof( NativeMethods.NCCALCSIZE_PARAMS ) );

				NativeMethods.RECT rect = csp.rgrc0;
				Rectangle rc = Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
				OnNcCalcSize( ref rc );
				csp.rgrc0 = new NativeMethods.RECT( rc );

				Marshal.StructureToPtr( csp, msg.LParam, false );
			}

			base.WndProc( ref msg );
		}

		/// <summary></summary>
		/// <param name="msg"/>
		private void WmNcPaint( ref Message msg )
		{
			if( this.HScroll && this.VScroll )
			{
				if( cachedRgn != IntPtr.Zero )
				{
					NativeMethods.DeleteObject( cachedRgn );
					this.cachedRgn = IntPtr.Zero;
				}

				if( this.GridOfficeScrollBars == OfficeScrollBars.None && !this.MetroScrollBars )
					this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref msg );
			}

			bool themed = this is IThemedControl && XPThemes.IsThemedOS && ( (IThemedControl)this ).ThemesEnabled;
			if( themed && themedDrawing != null )
			{
				themedDrawing.DrawThemedBorderColor( this, ref msg );
			}

			base.WndProc( ref msg );
		}

		/// <summary></summary>
		/// <param name="msg"/>
		private void WmPaint( ref Message msg )
		{
			if( scrollbarsDirty )
				this.ChangeScrollbars();

			if( this.HasDoubleBufferSurface )
			{
				NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
				NativeMethods.BeginPaint( Handle, ref ps );
				Rectangle rc = Rectangle.FromLTRB( ps.rcPaint_left, ps.rcPaint_top, ps.rcPaint_right, ps.rcPaint_bottom );
				PaintEventArgs pe = new PaintEventArgs( DoubleBufferSurface.Graphics, rc );
				OnPaint( pe );
				DoubleBufferSurface.Render();
				NativeMethods.EndPaint( Handle, ref ps );
			}
			else
			{
				base.WndProc( ref msg );
			}
		}

		/// <summary>
		///     Handles the WM_SETCURSOR message
		/// </summary>
		/// <internalonly/>
		/// <param name="m"/>
		private void WmSetCursor( ref Message m )
		{
			// Accessing through the Handle property has side effects that break this
			// logic. You must use Handle.
			//
			if( m.WParam == Handle && ( (int)m.LParam & 0x0000FFFF ) == NativeMethods.HTCLIENT )
			{
				OnSetCursor( ref m );
			}
			else
			{
				DefWndProc( ref m );
			}
		}

		/// <summary></summary>
		/// <param name="m"/>
		private void WmHScroll( ref Message m )
		{
			base.WndProc( ref m );

			if( HScrollBar.IsReflect )
			{
				HScrollBar.ReflectScrollMessage( ref m );
			}
		}

		/// <summary></summary>
		/// <param name="m"/>
		private void WmVScroll( ref Message m )
		{
			base.WndProc( ref m );

			if( VScrollBar.IsReflect )
			{
				VScrollBar.ReflectScrollMessage( ref m );
			}
		}

		/// <summary></summary>
		/// <param name="msg"/>
		private void WmMouseDown( ref Message msg )
		{
			this.isMousePressed = true;
			base.WndProc( ref msg );
		}

		/// <summary></summary>
		/// <param name="msg"/>
		private void WmMouseUp( ref Message msg )
		{
			this.isMousePressed = false;
			base.WndProc( ref msg );
		}
		#endregion
        #region Metro Scrollbars
        private bool m_bMetroScrollBars;
        private MetroColorTable m_metroColorTable;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual MetroColorTable MetroColorTable
        {
            get
            {
                if (m_metroColorTable == null)
                {
                    m_metroColorTable = new MetroColorTable();
                }
                return m_metroColorTable;
            }
            set
            {
                if (value != m_metroColorTable)
                {
                    m_metroColorTable = value;
                    if (HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                    {
                        ScrollBarCustomDraw scrollBar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
                        if(scrollBar.VisualStyle == ScrollBarCustomDrawStyles.Metro)
                            scrollBar.MetroColorTable = m_metroColorTable;
                    }
                    if (VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                    {
                        ScrollBarCustomDraw scrollBar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
                        if (scrollBar.VisualStyle == ScrollBarCustomDrawStyles.Metro)
                            scrollBar.MetroColorTable = m_metroColorTable;
                    }
                    if (m_bMetroScrollBars && sizeBox != null)
                        sizeBox.BackColor = m_metroColorTable.ScrollerBackground;
                    if (this.IsHandleCreated)
                    {
                        scrollbarsDirty = true;
                        Invalidate();  // WmPaint will call ChangeScrollbars()
                    }
                }
            }
        }

        [
        SRCategory(@"Appearance"),
        Description("Toggle between standard and metro scrollbars."),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.All)
        ]
        public virtual bool MetroScrollBars
        {
            get
            {
                return m_bMetroScrollBars;
            }
            set
            {
                if (value != m_bMetroScrollBars)
                {
                    m_bMetroScrollBars = value;
                    if (HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                    {
                        ScrollBarCustomDraw scrollBar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
                        scrollBar.MetroColorTable = m_metroColorTable;
                        scrollBar.VisualStyle = ScrollBarCustomDrawStyles.Metro ;                       
                    }
                    if (VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                    {
                        ScrollBarCustomDraw scrollBar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
                        scrollBar.MetroColorTable = m_metroColorTable;
                        scrollBar.VisualStyle = ScrollBarCustomDrawStyles.Metro;                       
                    }
                    if (this.IsHandleCreated)
                    {
                        scrollbarsDirty = true;
                        Invalidate();  // WmPaint will call ChangeScrollbars()
                    }
                }
            }
        }
        #endregion

        #region Office2007 Scrollbars
        private Office2007ColorScheme office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
		bool office2007ScrollBars = false;        

		/// <summary>
		/// Toggles between standard and Office2007 scrollbars.
		/// </summary>
		[
		SRCategory( @"Appearance" ),
		Description( "Toggle between standard and Office2007 scrollbars." ),
		DefaultValue( false ),
		]
		public virtual bool Office2007ScrollBars
		{
			get
			{
				return office2007ScrollBars;
			}
			set
			{
				if( office2007ScrollBars != value )
				{
					office2007ScrollBars = value;
                    if (value) this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    else
                        this.GridOfficeScrollBars = OfficeScrollBars.None;

					if( this.IsHandleCreated )
					{
						scrollbarsDirty = true;
						Invalidate();  // WmPaint will call ChangeScrollbars()
					}
				}

				OnOffice2007ScrollBarsChanged( EventArgs.Empty );
                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(OfficeScrollBars.Office2007);
                OnOfficeScrollBarsChanged(eventArgs);
			}
		}

		bool scrollbarsDirty = false;

		void ChangeScrollbars()
		{
			SyncReflectScrollBars();
			UpdateStyles();
			AdjustSizeBox();
			scrollbarsDirty = false;
		}

		/// <summary>
		/// Raises the <see cref="Office2007ScrollBarsChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnOffice2007ScrollBarsChanged( EventArgs e )
		{
			if( Office2007ScrollBarsChanged != null )
				Office2007ScrollBarsChanged( this, e );
		}



        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                if (this.Office2007ScrollBars)
                {
                    if (value == "Office2007Blue")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Blue;
                    else if (value == "Office2007Silver")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Black;
                    else if (value == "Managed")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Managed;
                }
                else
                {
                    if (value == "Office2007Blue")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                    else if (value == "Office2007Silver")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                    else if (value == "Managed")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Managed;
                }
            }
        }


		/// <summary>
		/// Gets or sets the style of Office2007 scroll bars.
		/// </summary>
		[
		SRCategory( @"Appearance" ),
		Browsable( true ),
		Description( "Office 2007 style scrollbars." ),
		DefaultValue( Office2007ColorScheme.Blue ),
		]
		public virtual Office2007ColorScheme Office2007ScrollBarsColorScheme
		{
			get
			{
				return office2007ScrollBarsColorScheme;
			}
			set
			{
				if( this.office2007ScrollBarsColorScheme != value )
				{
					office2007ScrollBarsColorScheme = value;
					if( this.Office2007ScrollBars || this.GridOfficeScrollBars == OfficeScrollBars.Office2007 )
					{
						if( HScrollBar.InnerScrollBar is ScrollBarCustomDraw )
						{
							ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
							sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
							sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;

						}

						if( VScrollBar.InnerScrollBar is ScrollBarCustomDraw )
						{
							ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
							sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
							sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
						}

						OnOffice2007ScrollBarsColorSchemeChanged( EventArgs.Empty );
					}

					if( this.DesignMode )
						Office2007ScrollBars = true;
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="OnOffice2007ScrollBarsColorSchemeChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnOffice2007ScrollBarsColorSchemeChanged( EventArgs e )
		{
			if( Office2007ScrollBarsColorSchemeChanged != null )
				Office2007ScrollBarsColorSchemeChanged( this, e );
		}

        # region Office2010ScrollBarsColorSchemeChanged
        /// <summary>
        /// Occurs when the <see cref="Office2010ScrollBarsColorScheme"/> property has changed.
        /// </summary>
        [Category("Appearance"), Description("Occurs when the Office2010ScrollBarsColorScheme property has changed.")]
        public event EventHandler Office2010ScrollBarsColorSchemeChanged;
        
        /// <summary>
        /// Raises the <see cref="OnOffice2010ScrollBarsColorSchemeChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnOffice2010ScrollBarsColorSchemeChanged(EventArgs e)
        {
            if (Office2010ScrollBarsColorSchemeChanged != null)
                Office2010ScrollBarsColorSchemeChanged(this, e);
        }
        # endregion

        # region  OfficeScrollBarsChanged
        /// <summary>
        /// Occurs when the <see cref="OfficeScrollBarsChanged"/> property has changed.
        /// </summary>
        [Category("Appearance"), Description("Occurs when the Supports MS-OfficeFlatScrollBars property has changed.")]
        public event OfficeScrollBarsEventHandler OfficeScrollBarsChanged;

        public delegate void OfficeScrollBarsEventHandler(object sender, OfficeScrollBarsEventArgs e);
        /// <summary>
        /// Raises the <see cref='OfficeScrollBarsChanged'/> event
        /// </summary>
        /// <param name="e"> Office scrollbar type </param>
        protected virtual void OnOfficeScrollBarsChanged(OfficeScrollBarsEventArgs e)
        {
            if (OfficeScrollBarsChanged != null)
            {
                OfficeScrollBarsChanged(this, e);
            }
        }
        /// <summary>
        /// Provides the data about <see cref="OfficeScrollBarsChanged"/> event of a <see cref="ScrollControl"/>.
        /// </summary>
        public class OfficeScrollBarsEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new <see cref="CurrentRecordEventArgs"/>.
            /// </summary>
            /// <param name="record">The record index.</param>
            public OfficeScrollBarsEventArgs(OfficeScrollBars gridOfficeScrollBars)
            {
                this.gridOfficeScrollBars = gridOfficeScrollBars;
            }

            /// <summary>
            /// Gets or sets the Office scroll bars
            /// </summary>
            public OfficeScrollBars GridOfficeScrollBars
            {
                get
                {
                    return gridOfficeScrollBars;
                }
                set
                {
                    if (gridOfficeScrollBars != value)
                    {
                        gridOfficeScrollBars = value;
                    }
                }
            }
            private OfficeScrollBars gridOfficeScrollBars;
        }

        # endregion

        /// <summary>
		/// Occurs when the <see cref="Office2007ScrollBarsColorScheme"/> property has changed.
		/// </summary>
        [Category( "Appearance" ), Description("Occurs when the Office2007ScrollBarsColorScheme property has changed.")]
		public event EventHandler Office2007ScrollBarsColorSchemeChanged;

		/// <summary>
		/// Occurs when the <see cref="Office2007ScrollBars"/> property has changed.
		/// </summary>
        [Category( "Appearance" ), Description("Occurs when the SupportsOffice2007FlatScrollBars property has changed.")]
		public event EventHandler Office2007ScrollBarsChanged;


		/// <summary>
		/// Gets the rectangle that represents the client area of the control. If custom
		/// scroll bars (ScrollBarCustomDraw, Office2007 style) are shown in the client area, then
		/// this method will remove the area occupied by the scrollbars.
		/// </summary>
		public new Rectangle ClientRectangle
		{
			get
			{
				bool vCustomScroll = ( vScroll && this.VScrollBar.InnerScrollBar is ScrollBarCustomDraw );
				bool hCustomScroll = ( hScroll && this.HScrollBar.InnerScrollBar is ScrollBarCustomDraw );

				Rectangle r = base.ClientRectangle;
				if( hCustomScroll )
				{
					r.Height -= this.HScrollBar.InnerScrollBar.Height;
				}

				if( vCustomScroll )
				{
					r.Width -= this.VScrollBar.InnerScrollBar.Width;

					if( this.RightToLeft == RightToLeft.Yes )
						r.X += this.VScrollBar.InnerScrollBar.Width;
				}

				// Note: Beware that derived controls could call base class version of the ClientRectangle
				// property. Also beware that derived controls could assume ClientRectangle.X equals 0 even
				// when RightToLeft.Yes is set.
				//
				// With derive grid control this is not a problem since the grid has a GridControlBase.GridBounds
				// property which by default returns this ClientRectangle result. Also, the derived grid control
				// does not assume GridBounds.X to be 0. Therefore scrollbars and scrolling will work just fine with 
				// grid control when you set RightToLeft.Yes.

				return r;
			}
		}

		/// <summary>
		/// Gets or sets the height and width of the client area of the control. If custom
		/// scroll bars (ScrollBarCustomDraw, Office2007 style) are shown the client area, then
		/// this method will remove the area occupied by the scrollbars.
		/// </summary>
		public new Size ClientSize
		{
			get
			{
				bool vCustomScroll = ( vScroll && this.VScrollBar.InnerScrollBar is ScrollBarCustomDraw );
				bool hCustomScroll = ( hScroll && this.HScrollBar.InnerScrollBar is ScrollBarCustomDraw );

				Size r = base.ClientSize;
				if( hCustomScroll )
				{
					r.Height -= this.HScrollBar.InnerScrollBar.Height;
				}

				if( vCustomScroll )
				{
					r.Width -= this.VScrollBar.InnerScrollBar.Width;
				}

				return r;
			}
			set
			{
				bool vCustomScroll = ( vScroll && this.VScrollBar.InnerScrollBar is ScrollBarCustomDraw );
				bool hCustomScroll = ( hScroll && this.HScrollBar.InnerScrollBar is ScrollBarCustomDraw );

				Size r = value;

				if( hCustomScroll )
				{
					r.Height += this.HScrollBar.InnerScrollBar.Height;
				}

				if( vCustomScroll )
				{
					r.Width += this.VScrollBar.InnerScrollBar.Width;
				}

				base.ClientSize = r;
			}
		}

		internal int ReflectPosition( int position )
		{
			if( this.HScrollBar != null )
			{
				return ( ( this.HScrollBar.Minimum + ( ( this.HScrollBar.Maximum - this.HScrollBar.LargeChange ) + 1 ) ) - position );
			}

			return -1;
		}
		#endregion

        #region Office2010 Scrollbars
        private Office2010ColorScheme office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
        OfficeScrollBars gridOfficeScrollBars = OfficeScrollBars.None;

        /// <summary>
        /// Gets or sets the Office like scrollbars.
        /// </summary>
        [
        SRCategory(@"Appearance"),
        Description("Gets or sets the MS-Office like scrollbars"),
        DefaultValue(OfficeScrollBars.None),
        RefreshProperties(RefreshProperties.All)
        ]
        public virtual OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return gridOfficeScrollBars;
            }
            set
            {
                if (gridOfficeScrollBars != value)
                {
                    gridOfficeScrollBars = value;
                    if (this.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                    {
                        if (HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }
                        if (VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }
                    }
                    else if (this.GridOfficeScrollBars == OfficeScrollBars.Office2007)
                    {
                        if (HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
                            sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
                        }
                        if (VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
                            sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
                        }
                    }
                    if (this.IsHandleCreated)
                    {
                        scrollbarsDirty = true;
                        this.Invalidate();
                    }
                }

                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(value);
                OnOfficeScrollBarsChanged(eventArgs);
            }
        }


        /// <summary>
        /// Gets or sets the style of MS Office2010 scroll bars.
        /// </summary>
        [
        SRCategory(@"Appearance"),
        Browsable(true),
        Description("MS Office 2010 style scrollbars."),
        DefaultValue(Office2010ColorScheme.Blue),
        ]
        public virtual Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return office2010ScrollBarsColorScheme;
            }
            set
            {
                if (this.office2010ScrollBarsColorScheme != value)
                {
                    office2010ScrollBarsColorScheme = value;
                    if (this.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                    {
                        if (HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)HScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }
                        if (VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)VScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }

                        OnOffice2010ScrollBarsColorSchemeChanged(EventArgs.Empty);
                    }

                    if (this.DesignMode)
                        GridOfficeScrollBars = OfficeScrollBars.Office2010;
                }
            }
        }

        #endregion

        #region Class overrides
        /// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnParentChanged( EventArgs e )
		{
			if( /*!this.DesignMode && */this.fillSplitterPane )
			{
				if( GetScrollBarFrameOfComponent( this ) == null
                  && GetDynamicSplitterFrameOfComponent( this ) == null )
				{
					this.fillSplitterPane = false;
				}
			}

			Form f = FindForm();
			if( f != this.wiredParentForm )
			{
				if( wiredParentForm != null )
				{
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				}
				wiredParentForm = f;
				if( wiredParentForm != null )
				{
					wiredParentForm.Enter += new EventHandler( wiredParentForm_Enter );
				}
			}

			hScrollBar.WireParent();
			vScrollBar.WireParent();
			base.OnParentChanged( e );
		}

		/// <summary>
		/// Handles mouse wheel processing for our scrollbars.
		/// </summary>
		/// <param name="e"/>
		protected override /*Control*/ void OnMouseWheel( MouseEventArgs e )
		{
			StopAutoScrollTimer();
			disableAutoScroll = true;
			isOnMouseWheel = true;
			try
			{
				int increment = 1;
				if( e.Delta != 0 )
				{
					if( ( Control.ModifierKeys & Keys.Control ) != 0 )
					{
						MouseWheelZoomEventArgs ea = new MouseWheelZoomEventArgs( e.Delta );
						OnMouseWheelZoom( ea );
					}
					else
					{
						ScrollBarWrapper sb = null;
						Control target = this;
						if( !HScrollBar.IsEmpty && HScrollBar.Maximum > HScrollBar.Minimum )
						{
							sb = HScrollBar;
							increment = HScrollIncrement;
						}

						// Scroll horizontal if shift key is pressed or if there is no vertical scrollbar.
						if( sb == null || ( Control.ModifierKeys & Keys.Shift ) == 0 )
						{
							if( !VScrollBar.IsEmpty && VScrollBar.Maximum > VScrollBar.Minimum )
							{
								sb = VScrollBar;
								increment = VScrollIncrement;
							}
						}

						if( sb != null )
						{
							// TODO: Check registry Control Panel\\Desktop\\WheelScrollLines
							// TODO: Also check SystemInformation.NativeMouseWheelSupport  to
							// avoid conflicts
							int lines = 1;
							if( e.Delta > 0 )
							{
								lines = -lines;
							}

							ScrollBars b = autoScrolling;
							autoScrolling = ScrollBars.None;
							int repeats = smoothMouseWheelScrolling ? MouseWheelScrollLines : 1;
							if( !smoothMouseWheelScrolling )
							{
								lines = lines * MouseWheelScrollLines;
							}
							for( int n = 0; n < repeats; n++ )
							{
								if( sb == VScrollBar )
								{
									int newValue = VScrollBar.Value + lines * increment;
									newValue = Math.Max( VScrollBar.Minimum, Math.Min( newValue, VScrollBar.Maximum - VScrollBar.LargeChange + 1 ) );
									if( newValue != VScrollBar.Value )
									{
										this.OnVScroll( this, new ScrollEventArgs( ScrollEventType.ThumbTrack, newValue ) );
									}
								}
								else
								{
									int newValue = HScrollBar.Value + lines * increment;
									newValue = Math.Max( HScrollBar.Minimum, Math.Min( newValue, HScrollBar.Maximum - HScrollBar.LargeChange + 1 ) );
									if( newValue != HScrollBar.Value )
									{
										this.OnHScroll( this, new ScrollEventArgs( ScrollEventType.ThumbTrack, newValue ) );
									}
								}
							}
							autoScrolling = b;
						}
					}
				}

				base.OnMouseWheel( e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
			}
			finally
			{
				disableAutoScroll = false;
				isOnMouseWheel = false;
			}
		}

		/// <summary>
		/// Raises the <see cref="SplitterPaneClosed"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSplitterPaneClosed( EventArgs e )
		{
			if( SplitterPaneClosed != null )
			{
				SplitterPaneClosed( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="SplitterPaneClosing"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSplitterPaneClosing( EventArgs e )
		{
			if( SplitterPaneClosing != null )
			{
				SplitterPaneClosing( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.FillSplitterPaneChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnFillSplitterPaneChanged( EventArgs e )
		{
			if( !FillSplitterPane )
			{
				// Make the control smaller so that user can resize it.
				Rectangle r = Bounds;
				r.Inflate( -Width / 4, -Height / 4 );
				Bounds = r;
			}
			else
			{
				BorderStyle = BorderStyle.None;
				Bounds = Parent.ClientRectangle;
			}
			PerformLayout();
			Refresh();
#if DEBUG

			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this.FillSplitterPane );
			}
#endif

			if( FillSplitterPaneChanged != null )
			{
				FillSplitterPaneChanged( this, e );
			}
		}

		/// <summary>
		/// Fires the ScrollbarsVisibleChanged event.
		/// </summary>
		/// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollbarsVisibleChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this.HScroll, this.VScroll );
			}
#endif

			if( ScrollbarsVisibleChanged != null )
			{
				ScrollbarsVisibleChanged( this, e );
			}
			this.AdjustSizeBox();
		}

		/// <summary>
		/// Override this method to implement zooming in your derived control.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnMouseWheelZoom( MouseWheelZoomEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( PaneDesc, e );
			}
#endif

			if( MouseWheelZoom != null )
			{
				MouseWheelZoom( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.ScrollControlMouseDown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="CancelMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlMouseDown( CancelMouseEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( PaneDesc, e );
			}
#endif

			if( ScrollControlMouseDown != null )
			{
				ScrollControlMouseDown( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.ScrollControlMouseMove"/> event.
		/// </summary>
		/// <param name="e">A <see cref="CancelMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlMouseMove( CancelMouseEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( PaneDesc, e );
			}
#endif

			if( ScrollControlMouseMove != null )
			{
				ScrollControlMouseMove( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.ScrollControlMouseUp"/> event.
		/// </summary>
		/// <param name="e">A <see cref="CancelMouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlMouseUp( CancelMouseEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( PaneDesc, e );
			}
#endif

			if( ScrollControlMouseUp != null )
			{
				ScrollControlMouseUp( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControlHandledMouseUp"/> event.
		/// </summary>
		/// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlHandledMouseUp( MouseEventArgs e )
		{
			if( ScrollControlHandledMouseUp != null )
			{
				ScrollControlHandledMouseUp( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControlHandledMouseMove"/> event.
		/// </summary>
		/// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlHandledMouseMove( MouseEventArgs e )
		{
			if( ScrollControlHandledMouseMove != null )
			{
				ScrollControlHandledMouseMove( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControlHandledMouseDown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollControlHandledMouseDown( MouseEventArgs e )
		{
			if( ScrollControlHandledMouseDown != null )
			{
				ScrollControlHandledMouseDown( this, e );
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override /*Control*/ void OnMouseDown( MouseEventArgs e )
		{
			if( RaiseCancelMouseEvent( e, new CancelMouseDelegate( OnScrollControlMouseDown ) ) )
			{
				// Update IntelliMouseDragging before mouse down gets processed.
				if( imm != null )
				{
					ScrollBars sb = ScrollBars.None;
					if( VScrollBar.Maximum > VScrollBar.Minimum )
					{
						sb |= ScrollBars.Vertical;
					}
					if( HScrollBar.Maximum > HScrollBar.Minimum )
					{
						sb |= ScrollBars.Horizontal;
					}
					imm.AllowScrolling = sb;
				}
			}
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.X, e.Y, e.Button, e.Clicks );
			}
#endif

			base.OnMouseDown( e );
			if( this.IsHandleCreated )
			{
				OnScrollControlHandledMouseDown( e );
			}
 			nDelayScrollTimer = 4;
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override /*Control*/ void OnMouseUp( MouseEventArgs e )
		{
			if( RaiseCancelMouseEvent( e, new CancelMouseDelegate( OnScrollControlMouseUp ) ) )
			{
				// reserved for future use ... (right now overriding OnScrollControlMouseUp
				// doesn't give any other result than OnMouseUp by ittself)
			}

			if( this.IsDisposed )
			{
				return;
			}
#if DEBUG

			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.X, e.Y, e.Button, e.Clicks );
			}
#endif

			base.OnMouseUp( e );
			if( this.IsHandleCreated )
			{
				OnScrollControlHandledMouseUp( e );
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override /*Control*/ void OnMouseMove( MouseEventArgs e )
		{
			try
			{
                if( !RaiseCancelMouseEvent( e, new CancelMouseDelegate( OnScrollControlMouseMove ) ) )
				{
					return;
				}

				// TODO: IntelliMouseDragScroll could wire to ScrollControlMouseMove event
				// and set Cancel to true ...
				if( IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null )
				{
					return;
				}

				ScrollBarWrapper sb = null;
				ScrollEventType et = ScrollEventType.SmallIncrement;

				int thisTick = Environment.TickCount;

				Point mousePoint = new Point( e.X, e.Y ); // this.PointToClient(Control.MousePosition);//
				Rectangle autoScrollBounds = AutoScrollBounds;
				Rectangle insideScrollBounds = InsideScrollBounds;

				if( AutoScrolling != ScrollBars.None &&
                  Control.MouseButtons != MouseButtons.None &&
                  !autoScrollBounds.IsEmpty
					/* comment this out if you want to avoid scrolling when
							   * the user drags the mouse outside of the AutoScrollBounds area
												   && autoScrollBounds.Contains(mousePoint)*/
				  )
				{
					bool isScrolling = false;
					if( ( AutoScrolling & ScrollBars.Horizontal ) != 0 )
					{
						bool rightToLeft = HScrollBar.InnerScrollBar != null && HScrollBar.InnerScrollBar.RightToLeft == RightToLeft.Yes;
                     	if( mousePoint.X <= insideScrollBounds.Left+1 &&
                          ( !rightToLeft && HScrollBar.Value > HScrollBar.Minimum
                          || rightToLeft && HScrollBar.Value + HScrollBar.LargeChange <= HScrollBar.Maximum )
						  )
						{
							isScrolling = true;
							sb = HScrollBar;
							et = ScrollEventType.SmallDecrement;
						}
						else if( mousePoint.X > insideScrollBounds.Right &&
                          ( !rightToLeft && HScrollBar.Value + HScrollBar.LargeChange <= HScrollBar.Maximum
                          || rightToLeft && HScrollBar.Value > HScrollBar.Minimum )
						  )
						{
							isScrolling = true;
							sb = HScrollBar;
							et = ScrollEventType.SmallIncrement;
						}
						inMouseDragScroll = isScrolling;
						if( sb != null && nDelayScrollTimer < 0 )
						{
							if( nDelayScrollTimer < -20 || thisTick > lastMouseTick + 50 )
							{
								lastMouseTick = thisTick;
								sb.SendScrollMessage( et );
							}
						}
					}

					sb = null;
					if( ( AutoScrolling & ScrollBars.Vertical ) != 0 )
					{
						if( mousePoint.Y < insideScrollBounds.Top &&
                          VScrollBar.Value > VScrollBar.Minimum )
						{
							isScrolling = true;
							sb = VScrollBar;
							et = ScrollEventType.SmallDecrement;
						}
						else if( mousePoint.Y > insideScrollBounds.Bottom &&
                          VScrollBar.Value + VScrollBar.LargeChange <= VScrollBar.Maximum )
						{
							isScrolling = true;
							sb = VScrollBar;
							et = ScrollEventType.SmallIncrement;
						}

						inMouseDragScroll = isScrolling;
						if( sb != null && nDelayScrollTimer < 0 )
						{
							if( nDelayScrollTimer < -20 || thisTick > lastMouseTick + 50 )
							{
								sb.SendScrollMessage( et );
								lastMouseTick = thisTick;
							}
						}
					}
             		inMouseDragScroll = isScrolling;
					if( this.inOleDragOver )
					{
                        if( !isScrolling )
						{
							nTimerCount = 0;
							this.nDelayScrollTimer = this.accDelayScrollTimer;
          				}
						else
						{
							nDelayScrollTimer--;
						}
					}
					else
					{
						if( !this.AutoScrollTimerEnable && isScrolling && --nDelayScrollTimer < 0 )
						{
							StartAutoScrollingEventArgs ea = new StartAutoScrollingEventArgs( AutoScrollReason.MouseDragging, this.AutoScrolling, 200, 25, 5, 4 );
							OnStartAutoScrolling( ea );
							if( !ea.Cancel )
							{
								accStartInterval = ea.AccStartInterval;
								accStepInterval = ea.AccStepInterval;
								accMinInterval = ea.AccMinInterval;
								accDelayScrollTimer = ea.AccDelayScrollTimer;
								StartAutoScrollTimer( Message.Create( Handle, NativeMethods.WM_MOUSEMOVE, (IntPtr)0, (IntPtr)MAKELPARAM( mousePoint.X, mousePoint.Y ) ) );
							}
						}
						else if( this.AutoScrollTimerEnable && !isScrolling )
						{
                           	StopAutoScrollTimer();
							this.nDelayScrollTimer = 4;
						}
					}
				}
				else if( this.AutoScrollTimerEnable )
				{
                  	StopAutoScrollTimer();
					this.nDelayScrollTimer = 4;
				}
			}
			finally
			{
#if DEBUG
				if( Switches.ScrollControlEvents.TraceVerbose )
				{
					TraceUtil.TraceCurrentMethodInfo( e.X, e.Y, e.Button, e.Clicks );
				}
#endif

				base.OnMouseMove( e );
				if( this.IsHandleCreated )
				{
					OnScrollControlHandledMouseMove( e );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="StartAutoScrolling"/> event.
		/// </summary>
		/// <param name="e">A <see cref="StartAutoScrollingEventArgs"/> that contains the event data.</param>
		protected virtual void OnStartAutoScrolling( StartAutoScrollingEventArgs e )
		{
			if( StartAutoScrolling != null )
			{
				StartAutoScrolling( this, e );
			}
		}

		/// <internalonly/>
		/// <summary></summary>
		/// <param name="m"/>
		[DocumentationExclude()]
		protected virtual void OnSetCursor( ref Message m )
		{
			if( IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null )
			{
				Cursor.Current = imm.Cursor;
			}
			else
			{
				Cursor cursor = null;
				if( this.mouseControllerDispatcher != null )
				{
					cursor = this.mouseControllerDispatcher.DisplayCursor;
				}

				if( cursor != null )
				{
					Cursor.Current = cursor;
				}
				else
				{
					Cursor.Current = ( overrideCursor != null ) ? overrideCursor : Cursors.Default;
				}
			}
		}

		/// <override/>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
			Cursor.Current = Cursors.Default;
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnMouseHover( EventArgs e )
		{
			StopAutoScrollTimer();
			base.OnMouseHover( e );
		}

		/// <summary>
		/// Handles the <see cref="ScrollBarWrapper.ValueChanged"/> of the horizontal scrollbar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnHScrollBarValueChanged( object sender, EventArgs e )
		{
		}

		/// <summary>
		/// Handles the <see cref="ScrollBarWrapper.ValueChanged"/> of the vertical scrollbar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnVScrollBarValueChanged( object sender, EventArgs e )
		{
		}

		/// <summary>
		/// Raises the <see cref="HorizontalScroll"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ScrollEventArgs"/> that contains the event data.</param>
		protected virtual void OnHorizontalScroll( ScrollEventArgs e )
		{
			if( HorizontalScroll != null )
			{
				HorizontalScroll( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="VerticalScroll"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ScrollEventArgs"/> that contains the event data.</param>
		protected virtual void OnVerticalScroll( ScrollEventArgs e )
		{
			if( VerticalScroll != null )
			{
				VerticalScroll( this, e );
			}
		}

		/// <summary><para>
		///       Listens for the horizontal scrollbar's scroll event.
		///    </para></summary>
		/// <param name="sender">
		///    A <see cref="System.Object"/> that contains data about the control.
		/// </param>
		/// <param name="se">
		///    A <see cref="System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
		/// </param>
		protected virtual void OnHScroll( object sender, ScrollEventArgs se )
		{
			try
			{
				IScrollBarFrame sbf = GetScrollBarFrameOfComponent( this );
				if( sbf != null && !sbf.IsActive( this, ScrollBars.Horizontal ) )
				{
					return;
				}

				OnHorizontalScroll( se );

				thumbBar = ScrollBars.Horizontal;
				IsThumbTracking = se.Type == ScrollEventType.ThumbTrack && !this.isOnMouseWheel;
				if( IsThumbTracking )
				{
					UpdateScrollTips( se );
					return;
				}

				int oldValue = HScrollBar.Value;
#if DEBUG
				if( Switches.ScrollControl.TraceVerbose )
				{
					TraceUtil.TraceCurrentMethodInfo( se.NewValue, oldValue, AutoScrollTimerEnable, this.nDelayScrollTimer, this.nTimerCount );
				}
#endif
				if( (this.GridOfficeScrollBars != OfficeScrollBars.None || this.MetroScrollBars) && this.RightToLeft == RightToLeft.Yes )
				{
					oldValue = this.ReflectPosition( oldValue );
				}

				if( se.NewValue != oldValue )
				{
					ScrollEventType saType = se.Type;

					switch( saType )
					{
						case ScrollEventType.SmallDecrement:
						case ScrollEventType.SmallIncrement:
						if( IntelliMouseDragScroll.ActiveIntelliMouseDragScroll == null )
						{
							IncreaseSmallChange( HScrollBar, nTimerCount );
							if( !AutoScrollTimerEnable && AutoScrolling == ScrollBars.None )
							{
								StartAutoScrollingEventArgs ea = new StartAutoScrollingEventArgs( AutoScrollReason.AccelarateScrollbar, ScrollBars.Horizontal, accStartInterval, accStepInterval, accMinInterval, accDelayScrollTimer );
								OnStartAutoScrolling( ea );
								if( !ea.Cancel )
								{
									accStartInterval = ea.AccStartInterval;
									accStepInterval = ea.AccStepInterval;
									accMinInterval = ea.AccMinInterval;
									accDelayScrollTimer = ea.AccDelayScrollTimer;
									StartAutoScrollTimer( HScrollBar, saType );
								}
							}
						}
						return;
						case ScrollEventType.EndScroll:
						StopAutoScrollTimer();
						break;
					}
				}
				if( !this.AutoScrollTimerEnable )
					nDelayScrollTimer = accDelayScrollTimer;
			}
			finally
			{
				if( se.NewValue >= HScrollBar.Minimum && se.NewValue <= HScrollBar.Maximum )
				{
					HScrollBar.Value = se.NewValue;
				}
			}

		}

		/// <summary><para>
		///       Listens for the vertical scrollbar's scroll event.
		///    </para></summary>
		/// <param name="sender">
		///    A <see cref="System.Object"/> that contains data about the control.
		/// </param>
		/// <param name="se">
		///    A <see cref="System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
		/// </param>
		protected virtual void OnVScroll( object sender, ScrollEventArgs se )
		{
			try
			{
				IScrollBarFrame sbf = GetScrollBarFrameOfComponent( this );
				if( sbf != null && !sbf.IsActive( this, ScrollBars.Vertical ) )
				{
					return;
				}

				OnVerticalScroll( se );

				thumbBar = ScrollBars.Vertical;
				IsThumbTracking = se.Type == ScrollEventType.ThumbTrack && !this.isOnMouseWheel;
				if( IsThumbTracking )
				{
					UpdateScrollTips( se );
					return;
				}

				if( se.Type == ScrollEventType.ThumbPosition )
				{
					OnVScrollBarValueChanged( this, EventArgs.Empty );
				}

				int oldValue = VScrollBar.Value;
#if DEBUG
				if( Switches.ScrollControl.TraceVerbose )
				{
					TraceUtil.TraceCurrentMethodInfo( se.NewValue, oldValue, AutoScrollTimerEnable, this.nDelayScrollTimer, this.nTimerCount );
				}
#endif

				if( se.NewValue != savedVValue )
				{
					ScrollEventType saType = se.Type;

					switch( saType )
					{
						case ScrollEventType.SmallDecrement:
						case ScrollEventType.SmallIncrement:
						if( IntelliMouseDragScroll.ActiveIntelliMouseDragScroll == null )
						{
							IncreaseSmallChange( VScrollBar, nTimerCount );
							if( !AutoScrollTimerEnable && AutoScrolling == ScrollBars.None )
							{
								StartAutoScrollingEventArgs ea = new StartAutoScrollingEventArgs( AutoScrollReason.AccelarateScrollbar, ScrollBars.Vertical, accStartInterval, accStepInterval, accMinInterval, accDelayScrollTimer );
								OnStartAutoScrolling( ea );
								if( !ea.Cancel )
								{
									accStartInterval = ea.AccStartInterval;
									accStepInterval = ea.AccStepInterval;
									accMinInterval = ea.AccMinInterval;
									accDelayScrollTimer = ea.AccDelayScrollTimer;
									StartAutoScrollTimer( VScrollBar, saType );
								}
							}
						}
						return;
						case ScrollEventType.EndScroll:
						StopAutoScrollTimer();
						break;
					}
				}
				if( !this.AutoScrollTimerEnable )
					nDelayScrollTimer = accDelayScrollTimer;
			}
			finally
			{
				if( se.NewValue >= VScrollBar.Minimum && se.NewValue <= VScrollBar.Maximum )
				{
					VScrollBar.Value = se.NewValue;
				}
			}

		}

		/// <remarks>
		/// Raises the ShowContextMenu event when the user right-clicks inside
		/// the control.
		/// <para/>
		/// You can cancel showing a content menu when
		/// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/></remarks>
		/// <summary></summary>
		/// <param name="e"/>
		protected virtual void OnShowContextMenu( ShowContextMenuEventArgs e )
		{
			if( ShowContextMenu != null )
			{
				ShowContextMenu( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="CancelMode"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCancelMode( EventArgs e )
		{
			this.isMousePressed = false;

			while( this.updateCount > 0 )
			{
				this.updateCount = 1;
				EndUpdate();
			}

			for( int n = 0; n < 5; n++ )
			{
				ResumeLayout( false );
			}

			StopAutoScrollTimer();
			this.IsThumbTracking = false;
#if DEBUG

			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e );
			}
#endif

			if( CancelMode != null )
			{
				try
				{
					CancelMode( this, e );
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="IntelliMouseDragScrolling"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnIntelliMouseDragScrolling( IntelliMouseDragScrollEventArgs e )
		{
			if( IntelliMouseDragScrolling != null )
			{
				IntelliMouseDragScrolling( this, e );
			}
		}

		/// <summary>
		/// Method calculate new client area size. If you want to increase Non-Client
		/// area for control and draw on it then this is method especially for you.
		/// </summary>
		/// <param name="client">Rectangle of the control client area.</param>
		protected virtual void OnNcCalcSize( ref Rectangle client )
		{
			// do nothing here if don't want to change non client area
		}

		/// <summary>
		/// Raises the <see cref="ScrollTip"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ScrollTipFeedbackEventArgs"/> that contains the event data.</param>
		protected virtual void OnScrollTipFeedback( ScrollTipFeedbackEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e );
			}
#endif

			if( ScrollTipFeedback != null )
			{
				ScrollTipFeedback( this, e );
			}
		}

		/// <summary>
		/// Factory method that creates a <see cref="ScrollTipWindow"/> for displaying ScrollTips.
		/// </summary>
		/// <returns></returns>
		protected virtual ScrollTipWindow OnCreateScrollTipWindow()
		{
			return new ScrollTipWindow();
		}

		/// <internalonly/>
		/// <summary></summary>
		/// <param name="se"/>
		[DocumentationExclude()]
		protected virtual void UpdateScrollTips( ScrollEventArgs se )
		{
			if( this.thumbBar == ScrollBars.Vertical )
			{
				if( !VScrollBar.SupportsScrollTips )
				{
					return;
				}
			}
			else if( this.thumbBar == ScrollBars.Horizontal )
			{
				if( !HScrollBar.SupportsScrollTips )
				{
					return;
				}
			}
			if( scrollTip == null )
			{
				return;
			}

			string text = String.Format( scrollTipFormat, se.NewValue );

			StringFormat sf = new StringFormat( StringFormatFlags.NoWrap | StringFormatFlags.NoClip );
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			sf.Trimming = StringTrimming.Character;

			ScrollTipFeedbackEventArgs e = new ScrollTipFeedbackEventArgs( this.thumbBar, ScrollTipActions.Scroll, se.NewValue, text,
			  scrollTip.Size, scrollTip.Location, scrollTip.Font, scrollTip.ForeColor, scrollTip.BackColor, scrollTip.BorderStyle, sf );

			OnScrollTipFeedback( e );

			scrollTip.ForeColor = e.ForeColor;
			scrollTip.BackColor = e.BackColor;
			scrollTip.Location = e.Location;
			scrollTip.MinimumSize = e.Size;
			scrollTip.Size = e.Size;
			scrollTip.Font = e.Font;
			scrollTip.Format = (StringFormat)e.Format.Clone();
			scrollTip.BorderStyle = e.BorderStyle;
			scrollTip.Text = e.Text;
			scrollTip.ShowWindowTopMost();
			sf.Dispose();
		}

		/// <summary>
		/// Raises the <see cref="AutoScrollingChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnAutoScrollingChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this.AutoScrolling );
			}
#endif

			if( AutoScrollingChanged != null )
			{
				AutoScrollingChanged( this, e );
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.UpdatingChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// The event handler for this event can check <see cref="ScrollControl.Updating"/>
		/// to determine if <see cref="ScrollControl.BeginUpdate()"/> or <see cref="ScrollControl.EndUpdate()"/>
		/// was called.
		/// </remarks>
		protected virtual void OnUpdatingChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( "UpdateCount=", this.updateCount, "PaintPending=", this.paintPending, "UpdateOptions=", this.UpdateOptions );
			}
#endif

			if( UpdatingChanged != null )
			{
				UpdatingChanged( this, e );
			}
		}

		/// <summary>
		///   Overridden. See the<see cref="System.Windows.Forms.Control.Invalidated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="System.Windows.Forms.InvalidateEventArgs"/> that contains the event data.</param>
		protected override void OnInvalidated( InvalidateEventArgs e )
		{
#if DEBUG
			if( Switches.BeginEndUpdate.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( "ScrollControl.OnInvalidated(" + e.InvalidRect.ToString() + ")" );
			}
#endif

			base.OnInvalidated( e );
			if( updateCount > 0 )
			{
				paintPending = true;
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.WindowScrolled"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ScrollWindowEventArgs"/> that contains the event data.</param>
		protected virtual void OnWindowScrolled( ScrollWindowEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e );
			}
#endif

			if( WindowScrolled != null )
			{
				try
				{
					WindowScrolled( this, e );
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollControl.WindowScrolling"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ScrollWindowEventArgs"/> that contains the event data.</param>
		protected virtual void OnWindowScrolling( ScrollWindowEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlEvents.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e );
			}
#endif

			if( WindowScrolling != null )
			{
				try
				{
					WindowScrolling( this, e );
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					{
						throw;
					}
				}
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnEnter( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			isActiveControl = true;
			isValidating = false;
			isValidated = false;
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			base.OnEnter( e );
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnLeave( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			isActiveControl = false;
			isValidating = false;
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			base.OnLeave( e );

			if( ( !this.CausesValidation || this.IsValidated ) && !hasControlFocus )
			{
				OnDeactivated( e );
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnValidating( CancelEventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			isValidating = true;
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.Cancel, this );
			}
#endif

			base.OnValidating( e );
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnValidated( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			isValidating = false;
			isValidated = true;
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			base.OnValidated( e );

			if( !isActiveControl && !hasControlFocus )
			{
				OnDeactivated( e );
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnLostFocus( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			RaiseControlLostFocus();
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnGotFocus( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			RaiseControlGotFocus();
		}

		/// <summary>
		/// Raises the <see cref="Deactivated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnDeactivated( EventArgs e )
		{
			isDeactivatedCalled = true;
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			if( Deactivated != null )
			{
				Deactivated( this, e );
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.Control, this );
			}
#endif

			base.OnControlRemoved( e );

			e.Control.GotFocus -= new EventHandler( ChildGotFocus );
			e.Control.LostFocus -= new EventHandler( ChildLostFocus );
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnControlAdded( ControlEventArgs e )
		{
#if DEBUG
			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( e.Control, this );
			}
#endif

			base.OnControlAdded( e );

			e.Control.GotFocus += new EventHandler( ChildGotFocus );
			e.Control.LostFocus += new EventHandler( ChildLostFocus );
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnSizeChanged( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			base.OnSizeChanged( e );
			if( !ClientRectangle.IsEmpty )
			{
				AdjustSizeBox();
				if( wiredParentForm != null && sizeBox != null )
					sizeBox.ParentFormWindowState = wiredParentForm.WindowState;

			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnLocationChanged( EventArgs e )
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			base.OnLocationChanged( e );
			if( !ClientRectangle.IsEmpty )
			{
				AdjustSizeBox();
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnPaint( PaintEventArgs e )
		{
			onPaintCalled = true;
			base.OnPaint( e );
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );
			EnsurePaintCodeJitted( false );

			Form f = FindForm();
			if( f != null )
			{
				if( wiredParentForm != null )
				{
					wiredParentForm.Enter -= new EventHandler( wiredParentForm_Enter );
				}
				wiredParentForm = f;
				if( wiredParentForm != null )
				{
					wiredParentForm.Enter += new EventHandler( wiredParentForm_Enter );
				}
			}
		}

		/// <override/>
		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			if( IsSplitterPaneClosing || this.Parent == null )
			{
				return;
			}

			EnsurePaintCodeJitted( false );
			if( Visible )
			{
				SyncReflectScrollBars();
			}

			this.AdjustSizeBox();
		}

		/// <summary>
		/// Raises the <see cref="Control.GotFocus"/> event. This method is called when the control
		/// or any child control gets focus and this control did not have focus before.
		/// </summary>
		/// <remarks>
		/// Inheriting classes should override this method instead of overriding <see cref="Control.OnGotFocus"/>
		/// because <see cref="OnControlGotFocus"/> is also called when child controls get focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlGotFocus()
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}
#if DEBUG

			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			base.OnGotFocus( EventArgs.Empty );
		}

		/// <summary>
		/// Raises the <see cref="Control.LostFocus"/> event. This method is called when the control
		/// or any child control loses focus and the new focused control is not a child of this control.
		/// </summary>
		/// <remarks>
		/// Inheriting classes should override this method instead of overriding <see cref="Control.OnLostFocus"/>
		/// because <see cref="OnControlLostFocus"/> is also called when child controls lose focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlLostFocus()
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}
#if DEBUG

			if( Switches.ScrollControlFocus.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( this );
			}
#endif

			base.OnLostFocus( EventArgs.Empty );

			if( !isActiveControl && !isValidating && !( CausesValidation && !this.isValidated ) )
			{
				OnDeactivated( EventArgs.Empty );
			}
			else
			{
				CancelUpdate();

				if( isValidating )
				{
					OnValidatingLostFocus();
				}
			}
		}

		/// <summary>
		/// This method is called if the control's <see cref="OnControlLostFocus"/> notification occurs
		/// while handling a <see cref="Control.Validating"/> event. This typically occurs if a
		/// message box is displayed from a <see cref="Control.Validating"/> event handler.
		/// </summary>
		protected virtual void OnValidatingLostFocus()
		{
			// Sometimes when users display a message box while the control is validated,
			// the message box is shown behind the application window and the user has
			// the impression the application is locked up.
			//
			// Pressing the <ALT> key will make the message box appear correctly in front of
			// the window.
			//
			// The following line emulates pressing the <ALT> key.
			//			SendKeys.Send("%");
		}

		/// <summary>
		/// Minimizes the time the first time the control is drawn. Calling
		/// OnPaint before the control is made visible ensures that all relevant code for drawing
		/// has been jitted.
		/// </summary>
		protected virtual void OnEnsurePaintCodeJitted()
		{
			if( !onPaintCalled )
			{
				Bitmap bmp = new Bitmap( 2, 2 );
				Graphics g = Graphics.FromImage( bmp );
				this.OnPaint( new PaintEventArgs( g, new Rectangle( Point.Empty, bmp.Size ) ) );
				g.Dispose();
				bmp.Dispose();
			}
		}

		/// <summary>
		/// Raises the <see cref="AccelerateScrollingChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnAccelerateScrollingChanged( EventArgs e )
		{
			if( AccelerateScrollingChanged != null )
			{
				AccelerateScrollingChanged( this, e );
			}

			_ApplyAccelerateScrolling();
		}
    
		/// <override/>
		/// <summary></summary>
		/// <param name="drgevent"/>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			inOleDragOver = true;

			// Adds support for AutoScrolling when control is a OLE Drop Target.
			// (simply set AutoScrolling in OnDragEnter and OnDragLeave ...)
			base.OnDragOver( drgevent );

			if( !AllowRaiseMouseMoveInOnDragOver )
			{
				return;
			}

			// This condition  was commented by Lucas in order to
			// fix issue 146 ( part 1 - resolve scrolling when DragDropEffects is none )

			#region /* comments */
			//if (drgevent.Effect != DragDropEffects.None)
			#endregion

			{
				Point pt = new Point( drgevent.X, drgevent.Y );
				pt = PointToClient( pt );

				// Immeditaley start autoscrolling when
				if( !this.AutoScrollTimerEnable && this.AutoScrolling != ScrollBars.None )
				{
					StartAutoScrollingEventArgs ea = new StartAutoScrollingEventArgs( AutoScrollReason.OleDragOver, this.AutoScrolling, 200, 25, 5, 4 );
					OnStartAutoScrolling( ea );
					if( !ea.Cancel )
					{
						accStartInterval = ea.AccStartInterval;
						accStepInterval = ea.AccStepInterval;
						accMinInterval = ea.AccMinInterval;
						accDelayScrollTimer = ea.AccDelayScrollTimer;
						// just using WM_DROPFILES to identify this message later.
						StartAutoScrollTimer( Message.Create( Handle, NativeMethods.WM_DROPFILES, (IntPtr)0, (IntPtr)MAKELPARAM( pt.X, pt.Y ) ) );
					}
				}

                if( !this.AutoScrollTimerEnable)
                {
					this.OnMouseMove( new MouseEventArgs( Control.MouseButtons, 1, pt.X, pt.Y, 0 ) );
				}
   			}

			inOleDragOver = false;
		}
		#endregion

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void wiredParentForm_Enter( object sender, EventArgs e )
		{
			if( !this.IsActiveControl )
			{
				OnEnter( e );
			}
		}

		/// <summary></summary>
		private void WireScrollEvents()
		{
			vScrollBar.Scroll += new ScrollEventHandler( this.OnVScroll );
			hScrollBar.Scroll += new ScrollEventHandler( this.OnHScroll );
			vScrollBar.ValueChanged += new EventHandler( this.OnVScrollBarValueChanged );
			hScrollBar.ValueChanged += new EventHandler( this.OnHScrollBarValueChanged );
		}

		/// <summary></summary>
		private void UnwireScrollEvents()
		{
			vScrollBar.Scroll -= new ScrollEventHandler( this.OnVScroll );
			hScrollBar.Scroll -= new ScrollEventHandler( this.OnHScroll );
			vScrollBar.ValueChanged -= new EventHandler( this.OnVScrollBarValueChanged );
			hScrollBar.ValueChanged -= new EventHandler( this.OnHScrollBarValueChanged );
		}

		/// <summary></summary>
		void ISplitterPaneSupport.PaneClosing()
		{
			this.isPaneClosing = true;

			UnwireScrollEvents();
			try
			{
				OnSplitterPaneClosing( EventArgs.Empty );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
			}
		}

		/// <summary></summary>
		void ISplitterPaneSupport.PaneClosed()
		{
			this.isPaneClosed = true;
			try
			{
				OnSplitterPaneClosed( EventArgs.Empty );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
			}

		}

		/// <summary>
		///     Given a component, this retrieves the <see cref="IScrollBarFrame"/> that it is parented to;
		///     NULL if it is not parented to any <see cref="IScrollBarFrame"/>.
		/// </summary>
		/// <param name="comp">
		///     The component to check.
		/// </param>
		/// <returns>
		///     A <see cref="IScrollBarFrame"/> that the component is parented to; NULL if
		///     no such interface exists. This will return the component if it
		///     is an instance of <see cref="IScrollBarFrame"/>.
		/// </returns>
		public static IScrollBarFrame GetScrollBarFrameOfComponent( object comp )
		{
			Control c = comp as Control;
			while( c != null )
			{
				if( c is IScrollBarFrame )
				{
					return (IScrollBarFrame)c;
				}

				c = c.Parent;
			}

			return null;
		}

		/// <summary>
		///     Given a component, this retrieves the <see cref="IDynamicSplitterFrame"/> that it is parented to;
		///     NULL if it is not parented to any <see cref="IDynamicSplitterFrame"/>.
		/// </summary>
		/// <param name="comp">
		///     The component to check.
		/// </param>
		/// <returns>
		///     An <see cref="IDynamicSplitterFrame"/> that the component is parented to; NULL if
		///     no such interface exists. This will return the component if it
		///     is an instance of <see cref="IDynamicSplitterFrame"/>.
		/// </returns>
		public static IDynamicSplitterFrame GetDynamicSplitterFrameOfComponent( object comp )
		{
			Control c = comp as Control;
			while( c != null )
			{
				if( c is IDynamicSplitterFrame )
				{
					return (IDynamicSplitterFrame)c;
				}

				c = c.Parent;
			}

			return null;
		}

		/// <summary>
		/// Delegates the MouseWheelEvent from a child control.
		/// </summary>
		/// <param name="e">A MouseEventArgs that holds event data.</param>
		/// <remarks><code>
		/// public class GridTextBox: RichTextBox
		/// {
		/// 	private GridTextBoxCell parent;
		/// 	protected override void OnMouseWheel(MouseEventArgs e)
		/// 	{
		/// 		parent.Grid.ProcessMouseWheel(e);
		/// 	}
		/// }
		/// </code></remarks>
		public void ProcessMouseWheel( MouseEventArgs e )
		{
			OnMouseWheel( e );
		}

		void ResetCustomScrollBar( ScrollBarWrapper sb )
		{
			if( sb.InnerScrollBar is ScrollBarCustomDraw )
			{
				Controls.Remove( sb.InnerScrollBar );
				sb.InnerScrollBar = null;
			}
		}

		/// <summary></summary>
		private void SyncReflectScrollBars()
		{
			if( useSharedScrollBars )
			{
				return;
			}

			if( IsSplitterPaneClosing )
			{
				return;
			}

			IScrollBarFrame frame = this.FillSplitterPane ? GetScrollBarFrameOfComponent( this ) : null;
			if( frame != null )
			{
				this.vScroll = false;
				this.hScroll = false;
				ResetCustomScrollBar( VScrollBar );
				ResetCustomScrollBar( HScrollBar );

				Control hc = frame.GetHScrollBar( this ); ;
				ScrollBarAdapter hBar = hc != null ? new ScrollBarAdapter( hc ) : null;
				Control vc = frame.GetVScrollBar( this );
				ScrollBarAdapter vBar = vc != null ? new ScrollBarAdapter( vc ) : null;

				if( hBar != null )
				{
					if( hBar.LargeChange > hBar.Maximum )
					{
						hBar.LargeChange = hBar.Maximum;
					}

					if( hBar.SmallChange > hBar.LargeChange )
					{
						hBar.SmallChange = hBar.LargeChange;
					}

					this.HScrollBar.SmallChange = hBar.SmallChange;
					this.HScrollBar.LargeChange = hBar.LargeChange;
				}

				this.HScrollBar.InnerScrollBar = hc;

				if( vBar != null )
				{
					if( vBar.LargeChange > vBar.Maximum )
					{
						vBar.LargeChange = vBar.Maximum;
					}

					if( vBar.SmallChange > vBar.LargeChange )
					{
						vBar.SmallChange = vBar.LargeChange;
					}

					this.VScrollBar.SmallChange = vBar.SmallChange;
					this.VScrollBar.LargeChange = vBar.LargeChange;

				}
				this.VScrollBar.InnerScrollBar = vc;
			}
			else
			{
				if( vScroll )
				{
					if( this.GridOfficeScrollBars == OfficeScrollBars.Office2007 )
					{
						if( !( VScrollBar.InnerScrollBar is ScrollBarCustomDraw ) )
						{
							ScrollBarCustomDraw sbar = new VScrollBarCustomDraw();
							sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
							sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
							this.VScrollBar.InnerScrollBar = sbar;
                            if (DpiAware)
                            {
                                sbar = DpiScrollBehavior(sbar);
                            }
							Controls.Add( sbar );
						}
					}
                    else if (this.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                    {
                        if (!(VScrollBar.InnerScrollBar is ScrollBarCustomDraw))
                        {
                            ScrollBarCustomDraw sbar = new VScrollBarCustomDraw();
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                            this.VScrollBar.InnerScrollBar = sbar;
                            if (DpiAware)
                            {
                                sbar = DpiScrollBehavior(sbar);
                            }
                            Controls.Add(sbar);
                        }
                    }
                    else if (this.MetroScrollBars)
                    {
                        if (!(VScrollBar.InnerScrollBar is ScrollBarCustomDraw))
                        {                           
                            ScrollBarCustomDraw scrollBar = new VScrollBarCustomDraw();
                            scrollBar.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                            scrollBar.MetroColorTable = this.MetroColorTable;                           
                            this.VScrollBar.InnerScrollBar = scrollBar;
                            if (DpiAware)
                            {
                                scrollBar = DpiScrollBehavior(scrollBar);
                            }
                            Controls.Add(scrollBar);
                        }
                    }
                    else
                    {
                        if (!(this.VScrollBar.InnerScrollBar is ReflectScrollBar))
                        {
                            ResetCustomScrollBar(VScrollBar);
                            ReflectScrollBar r=new ReflectScrollBar(this, ScrollBars.Vertical);
                            this.VScrollBar.InnerScrollBar = r;
                            r.Dispose();
                        }
                    }
				}
				else
				{
					ResetCustomScrollBar( VScrollBar );
					this.VScrollBar.InnerScrollBar = null;
				}

				if( hScroll )
				{
					if( this.GridOfficeScrollBars == OfficeScrollBars.Office2007 )
					{
						if( !( HScrollBar.InnerScrollBar is ScrollBarCustomDraw ) )
						{
							ScrollBarCustomDraw sbar = new HScrollBarCustomDraw();
							sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
							sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
							this.HScrollBar.InnerScrollBar = sbar;
                            if (DpiAware)
                            {
                                sbar = DpiScrollBehavior(sbar);
                            }
							Controls.Add( sbar );
						}
					}
                    else if (this.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                    {
                        if (!(HScrollBar.InnerScrollBar is ScrollBarCustomDraw))
                        {
                            ScrollBarCustomDraw sbar = new HScrollBarCustomDraw();
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                            this.HScrollBar.InnerScrollBar = sbar;
                            if (DpiAware)
                            {
                                sbar = DpiScrollBehavior(sbar);
                            }
                            Controls.Add(sbar);
                        }
                    }
                    else if (this.MetroScrollBars)
                    {
                        if (!(HScrollBar.InnerScrollBar is ScrollBarCustomDraw))
                        {
                            ScrollBarCustomDraw scrollBar = new HScrollBarCustomDraw();
                            scrollBar.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                            scrollBar.MetroColorTable = this.MetroColorTable;                            
                            this.HScrollBar.InnerScrollBar = scrollBar;
                            if (DpiAware)
                            {
                                scrollBar = DpiScrollBehavior(scrollBar);
                            }
                            Controls.Add(scrollBar);
                        }
                    }
                    else
                    {
                        if (!(HScrollBar.InnerScrollBar is ReflectScrollBar))
                        {
                            ResetCustomScrollBar(HScrollBar);
                            ReflectScrollBar r1=new ReflectScrollBar(this, ScrollBars.Horizontal);
                            this.HScrollBar.InnerScrollBar = r1;
                            r1.Dispose();
                        }
                    }
				}
				else
				{
					ResetCustomScrollBar( HScrollBar );
					this.HScrollBar.InnerScrollBar = null;
				}
			}
		}
        /// <summary>
        /// The scrollbar height and width will be increased, when the DPI of system is increased.
        /// </summary>
        /// <param name="scrollBar"></param>
        /// <returns>ScrollBarCustomDraw</returns>
        private ScrollBarCustomDraw DpiScrollBehavior(ScrollBarCustomDraw scrollBar)
        {
            using (Graphics grapics = this.CreateGraphics())
            {
                if (grapics.DpiY > 96)
                {
                    int calcWidth = (int)(grapics.DpiY * scrollBar.Width) / 96;
                    int calcHeight = (int)(grapics.DpiX * scrollBar.Height) / 96;
                    scrollBar.Size = new Size(calcWidth, calcHeight);
                }
                return scrollBar;
            }
        }
        private bool dpiAware = false;
        /// <summary>
        /// Gets or sets the boolean value for enhanching the apperance settings for DpiAware enabled Application.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DpiAware
        {
            get
            {
                return dpiAware;
            }
            set
            {
                dpiAware = value;
            }
        }
		/// <summary>
		///     Displays / hides the horizontal and vertical autoscrollbars. This will
		///     also adjust the values of formState to reflect the new state.
		/// </summary>
		/// <param name="horiz">
		///     True if the horizontal scrollbar should be displayed.
		/// </param>
		/// <param name="vert">
		///     True if the vertical scrollbar should be displayed.
		/// </param>
		/// <returns>
		///     True if the form needs to be layed out again.
		/// </returns>
		private bool SetVisibleScrollbars( bool horiz, bool vert )
		{
#if DEBUG
			if( Switches.ScrollControl.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( horiz, vert );
			}
#endif

			if( horiz == this.hScroll &&
              vert == this.vScroll )
			{
				return false;
			}

			this.hScroll = horiz;
			this.vScroll = vert;

			this.UpdateStyles();
			this.SyncReflectScrollBars();

			try
			{
				OnScrollbarsVisibleChanged( EventArgs.Empty );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
			}

			return true;
		}

		/// <summary>
		/// Resets the <see cref="MouseWhellScrollLines"/> property.
		/// </summary>
		public void ResetMouseWheelScrollLines()
		{
			mouseWheelScrollLines = 0;
		}

		/// <summary>
		/// Raises the specified mouse event and catches any exception. If an exception is caught, NotifyCancelMode
		/// is called. Returns False if event should be ignored by the grid.
		/// </summary>
		/// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
		/// <param name="d">A delegate that handles the event.</param>
		/// <returns>False if CancelMouseEventArgs.Cancel is True; True otherwise</returns>
		internal bool RaiseCancelMouseEvent( MouseEventArgs e, CancelMouseDelegate d )
		{
			CancelMouseEventArgs cmea = new CancelMouseEventArgs( e );
			try
			{
				mouseEventException = null;
				d( cmea );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				this.mouseEventException = ex;
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
				cmea.Cancel = true;
				if( !this.IsDisposed )
				{
					NotifyCancelMode();
				}
			}
			return this.IsHandleCreated && !cmea.Cancel;
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void IntelliMouseDragScrollEvent( object sender, IntelliMouseDragScrollEventArgs e )
		{
			int dy = e.DY;
			int dx = e.DX;

			this.disableAutoScroll = true;

			try
			{
				OnIntelliMouseDragScrolling( e );
				if( !e.Cancel && !e.Scrolled )
				{
					if( Math.Abs( dy ) > Math.Abs( dx ) )
					{
						VScrollBar.SendScrollMessage( dy > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement );
					}
					else
					{
						HScrollBar.SendScrollMessage( dx > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement );
					}
				}
			}
			finally
			{
				this.disableAutoScroll = false;
			}
		}

		/// <summary>Method force to invalidate Non Client area of control.</summary>
		protected internal virtual void InvalidateNc()
		{
			if( this.IsHandleCreated )
			{
				NativeMethods.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero,
				  NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
			}
		}

		/// <summary>Invalidate specified rectangle in Non Client area.</summary>
		/// <param name="rc">Rectangle to Invalidate.</param>
		protected internal virtual void InvalidateNc( Rectangle rc )
		{
			if( this.IsHandleCreated )
			{
				NativeMethods.RECT rect = new NativeMethods.RECT( rc );

				NativeMethods.RedrawWindow( this.Handle, ref rect, IntPtr.Zero,
				  NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
			}
		}

		/// <summary>Invalidate and wait till area update on Non Client area.</summary>
		protected virtual void UpdateNc()
		{
			if( this.IsHandleCreated )
			{
				NativeMethods.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero,
				  NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE |
                  NativeMethods.RDW_UPDATENOW );
			}
		}
		/// <summary>
		///  Method draws non-client area of an ScrollControl.
		/// </summary>
		/// <param name="ncEventArgs"> EventArgs with Non-client info for drawing. </param>
		protected internal virtual void OnNCPaint( NCPaintEventArgs ncEventArgs )
		{
			if( this.HScroll && this.VScroll 
                && this.GridOfficeScrollBars == OfficeScrollBars.None && !this.MetroScrollBars && !IgnoreSizeGripStyleHideShow )
			{
				Graphics g = ncEventArgs.Graphics;
				Rectangle bounds = ncEventArgs.DisplayRectangle;

				int style = NativeMethods.GetWindowLong( Handle, NativeMethods.GWL_STYLE );
				int dx = 0;

				if( ( style & NativeMethods.WS_BORDER ) != 0 )
				{
					dx = 1;
				}

				style = NativeMethods.GetWindowLong( Handle, NativeMethods.GWL_EXSTYLE );

				if( ( style & NativeMethods.WS_EX_CLIENTEDGE ) != 0 )
				{
					dx += 2;
				}

				if( this.RightToLeft == RightToLeft.Yes )
				{
					bounds = new Rectangle( dx, Bounds.Height - SystemInformation.HorizontalScrollBarHeight - dx,
						SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight );
				}
				else
				{
					bounds = new Rectangle( Bounds.Width - SystemInformation.VerticalScrollBarWidth - dx, Bounds.Height - SystemInformation.HorizontalScrollBarHeight - dx,
						SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight );
				}

				using( Brush brush = new SolidBrush( SystemColors.Control ) )
				{
					g.FillRectangle( brush, bounds );
				}

				if( this.SizeGripStyle == SizeGripStyle.Show )
				{
					bool themed = this is IThemedControl && XPThemes.IsThemedOS && ( (IThemedControl)this ).ThemesEnabled;
					if( themed )
					{
						this.themedScrollBarDrawing.DrawSizeBox( g, bounds );
					}
					else
					{
						ControlPaint.DrawSizeGrip( g, BackColor, bounds );
					}
				}
			}

			Rectangle windowRectInScreen = ncEventArgs.WindowInScreenRectangle;
			ncEventArgs.ClipRegion = NativeMethods.CreateRectRgn( windowRectInScreen.Left, windowRectInScreen.Top, windowRectInScreen.Right, windowRectInScreen.Bottom );
		}


		/// <summary></summary>
		/// <returns></returns>
		/// <param name="e"/>
		/// <param name="displayRect"/>
		/// <param name="windowRectInScreen"/>
		IntPtr INonClientPaintingSupport.NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			NCPaintEventArgs ncEventArgs = new NCPaintEventArgs( e.Graphics, e.ClipRectangle, displayRect, windowRectInScreen, IntPtr.Zero );
			OnNCPaint( ncEventArgs );

			return ncEventArgs.ClipRegion;
		}
		/// <summary>
		/// Raises a <see cref="ScrollControl.CancelMode"/> event.
		/// </summary>
		public void NotifyCancelMode()
		{
			OnCancelMode( EventArgs.Empty );
		}

		/// <exclude/>
		/// <summary>
		/// Increases small change of the ScrollBar.
		/// </summary>
		/// <param name="scrollBar"/>
		/// <param name="nTimerCount"/>
		public virtual void IncreaseSmallChange( ScrollBarWrapper scrollBar, long nTimerCount )
		{
			if( allowIncreaseSmallChange )
			{
				if( nTimerCount > 80 )
				{
					scrollBar.SmallChange = 5;
				}
				else if( nTimerCount > 40 )
				{
					scrollBar.SmallChange = 3;
				}
				else
				{
					scrollBar.SmallChange = 1;
				}
			}
		}

		/// <summary></summary>
		private void ScrollTipFeedbacks()
		{
			if( this.thumbBar == ScrollBars.Vertical )
			{
				if( !VScrollBar.SupportsThumbTrack )
				{
					vScrollBar.ValueChanged -= new EventHandler( this.OnVScrollBarValueChanged );
				}
				if( !VScrollBar.SupportsScrollTips )
				{
					return;
				}
			}
			else if( this.thumbBar == ScrollBars.Horizontal )
			{
				if( !HScrollBar.SupportsThumbTrack )
				{
					hScrollBar.ValueChanged -= new EventHandler( this.OnHScrollBarValueChanged );
				}
				if( !HScrollBar.SupportsScrollTips )
				{
					return;
				}
			}

			if( scrollTip == null )
			{
				scrollTip = OnCreateScrollTipWindow();
			}

			if( scrollTip == null )
			{
				return;
			}

			Size size = scrollTip.GetPreferredSize( String.Format( scrollTipFormat, int.MaxValue ) );
			Point loc;

			if( thumbBar == ScrollBars.Vertical )
			{
				loc = this.PointToScreen( new Point( this.ClientRectangle.Width - size.Width - SystemInformation.HorizontalScrollBarThumbWidth, ( this.ClientRectangle.Height - size.Height ) / 2 ) );
			}
			else
			{
				loc = this.PointToScreen( new Point( ( this.ClientRectangle.Width - size.Width ) / 2, this.ClientRectangle.Height - size.Height - SystemInformation.VerticalScrollBarThumbHeight ) );
			}

			Font font = FontUtil.CreateFont( Font, FontStyle.Bold );

			if( font == null )
			{
				return;
			}

			StringFormat sf = new StringFormat( StringFormatFlags.NoWrap | StringFormatFlags.NoClip );
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			sf.Trimming = StringTrimming.Character;

			ScrollTipFeedbackEventArgs e = new ScrollTipFeedbackEventArgs( this.thumbBar, ScrollTipActions.ThumbTrack, -1, "",
			  size, loc, font, SystemColors.InfoText, SystemColors.Info, BorderStyle.FixedSingle, sf );

			if( e.Cancel )
			{
				return;
			}

			OnScrollTipFeedback( e );

			scrollTip.ForeColor = e.ForeColor;
			scrollTip.BackColor = e.BackColor;
			scrollTip.Location = e.Location;
			scrollTip.MinimumSize = e.Size;
			scrollTip.Size = e.Size;
			scrollTip.Font = e.Font;
			scrollTip.Format = (StringFormat)e.Format.Clone();
			scrollTip.BorderStyle = e.BorderStyle;
			sf.Dispose();
		}

		/// <summary></summary>
		private void HideScrollTips()
		{
			if( this.thumbBar == ScrollBars.Vertical )
			{
				if( !VScrollBar.SupportsThumbTrack )
				{
					vScrollBar.ValueChanged += new EventHandler( this.OnVScrollBarValueChanged );
				}
				if( !VScrollBar.SupportsScrollTips )
				{
					return;
				}
			}
			else if( this.thumbBar == ScrollBars.Horizontal )
			{
				if( !HScrollBar.SupportsThumbTrack )
				{
					hScrollBar.ValueChanged += new EventHandler( this.OnHScrollBarValueChanged );
				}
				if( !HScrollBar.SupportsScrollTips )
				{
					return;
				}
			}

			if( scrollTip == null )
			{
				return;
			}

			ScrollTipFeedbackEventArgs e = new ScrollTipFeedbackEventArgs( this.thumbBar, ScrollTipActions.ThumbPosition, -1, "",
			  Size.Empty, Point.Empty, null, Color.Empty, Color.Empty, BorderStyle.None, null );

			OnScrollTipFeedback( e );

			if( scrollTip != null )
			{
				scrollTip.Hide();
			}
		}

		/// <summary>
		/// Resets the <see cref="InsideScrollMargins"/> property to its default value.
		/// </summary>
		public void ResetInsideScrollMargins()
		{
			insideScrollMargins = new Size( 10, 10 );
		}

		/// <summary>
		/// Starts the AutoScroll timer.
		/// </summary>
		/// <param name="scrollBar">The scrollbar to be automatically scrolled.</param>
		/// <param name="saType">The event to be sent to this scrollbar.</param>
		protected void StartAutoScrollTimer( ScrollBarWrapper scrollBar, ScrollEventType saType )
		{
			repeatScrollBar = scrollBar;
			repeatScrollEventType = saType;
			StartAutoScrollTimer( Message.Create( IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero ), 50 );
		}

		/// <overload>
		/// Starts the AutoScroll timer.
		/// </overload>
		/// <summary>
		/// Starts the AutoScroll timer.
		/// </summary>
		/// <param name="m">The message to be sent to the current control.</param>
		protected void StartAutoScrollTimer( Message m )
		{
			StartAutoScrollTimer( m, accStartInterval );
		}

		/// <summary></summary>
		private void _ApplyAccelerateScrolling()
		{
			switch( this.accelerateScrolling )
			{
				case AccelerateScrollingBehavior.None:
				case AccelerateScrollingBehavior.Immediate:
				accStartInterval = 200;
				accStepInterval = 25;
				accMinInterval = 5;
				accDelayScrollTimer = 4;
				break;
				case AccelerateScrollingBehavior.Fast:
				accStartInterval = 500;
				accStepInterval = 20;
				accMinInterval = 15;
				accDelayScrollTimer = 20;
				break;
				case AccelerateScrollingBehavior.Default:
				accStartInterval = 700;
				accStepInterval = 10;
				accMinInterval = 40;
				accDelayScrollTimer = 60;
				break;
			}
			this.nDelayScrollTimer = accDelayScrollTimer;
		}

		/// <summary>
		/// Starts the AutoScroll timer.
		/// </summary>
		/// <param name="m">The message to be sent to the current control.</param>
		/// <param name="interval">The initial interval for sending messages.</param>
		protected void StartAutoScrollTimer( Message m, int interval )
		{
           	if( disableAutoScroll
              || accelerateScrolling == AccelerateScrollingBehavior.None
              || repeatScrollEventTimer != null )
			{
				return;
			}
#if DEBUG
			if( Switches.Timers.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( interval, nDelayScrollTimer );
			}
#endif
			if( --nDelayScrollTimer > 1 )
			{
				return;
			}

			//			CancelEventArgs ea = new CancelEventArgs(false);
			//			OnQueryCanAccelerateScrolling(ea);
			//			if (ea.Cancel)
			//				return;

			VScrollBar.SmallChange = 1;
			HScrollBar.SmallChange = 1;

			lock( startstopSemaphor )
			{
				Debug.Assert( this.repeatScrollEventTimer == null, "Oops - StartTimer called twice." );
				repeatScrollMessage = Message.Create( m.HWnd, m.Msg, m.WParam, m.LParam );
				repeatScrollEventTimer = new Timer();
				repeatScrollEventTimer.Interval = interval; // milliseconds
				repeatScrollEventTimer.Tick += new EventHandler( repeatScrollEventTimer_Elapsed );
				nTimerCount = 0;

				repeatScrollEventTimer.Enabled = true;
			}
#if MOUSEHOOK
      // With IBM ThinkPad's Trakpoint Scrolling behavior, no ScrollEnd
      // message is sent when the user releases the middle mouse button.
      // MessageHooker will detect when the mouse button is released and
      // stop the timer.
      //hooker = new MouseProcHooker(Handle, this);
      //hooker.HookMessages = true;
#endif
		}

		/// <summary></summary>
		internal void SlowAutoScrollTimer()
		{
			lock( startstopSemaphor )
			{
				if( AutoScrollTimerEnable )
				{
					repeatScrollEventTimer.Interval = accStartInterval;
				} // 500 milliseconds
				nTimerCount = 0;
			}
		}

		/// <summary></summary>
		internal void StopAutoScrollTimer()
		{
            inMouseDragScroll = false;
			savedHValue = -1;
			savedVValue = -1;
			if( this.AllowIncreaseSmallChange )
			{
				VScrollBar.SmallChange = 1;
				HScrollBar.SmallChange = 1;
			}
#if MOUSEHOOK
      if( hooker != null )
      {
        hooker.HookMessages = false;
        hooker = null;
      }
#endif
			lock( startstopSemaphor )
			{
				repeatScrollBar = null;
				Timer savedTimer = repeatScrollEventTimer;
				repeatScrollEventTimer = null;
				if( savedTimer != null )
				{
					savedTimer.Tick -= new EventHandler( repeatScrollEventTimer_Elapsed );
					savedTimer.Dispose();
				}
				nTimerCount = 0;
				nDelayScrollTimer = accDelayScrollTimer;
			}
			_ApplyAccelerateScrolling();
		}

		/// <summary></summary>
		/// <param name="source"/>
		/// <param name="e"/>
		private void repeatScrollEventTimer_Elapsed( object source, EventArgs e )
		{
			Timer timer = source as Timer;
			try
			{
				if( repeatScrollEventTimer == null || !repeatScrollEventTimer.Enabled )
				{
					return;
				} // This is just the completion call - we don't want to handle that

				nTimerCount++;

				if( repeatScrollEventTimer.Interval > accStepInterval )
				{
					repeatScrollEventTimer.Interval = Math.Max( accStepInterval, repeatScrollEventTimer.Interval - accStepInterval );
				} // accelerate
				else
				{
					repeatScrollEventTimer.Interval = accMinInterval;
				} // accelerate

				if( Control.MouseButtons == MouseButtons.None )
				{
					StopAutoScrollTimer();
					return;
				}

				if( repeatScrollBar != null )
				{
					if( HScrollBar.Value == savedHValue && VScrollBar.Value == savedVValue )
					{
						StopAutoScrollTimer();
						return;
					}

					savedHValue = HScrollBar.Value;
					savedVValue = VScrollBar.Value;

					repeatScrollBar.SendScrollMessage( repeatScrollEventType );
				}
				else
				{
					// WM_DROPFILES is specified by OnDragOver to indicate that this is because of timer started in OnDragOver.
					if( repeatScrollMessage.Msg == NativeMethods.WM_MOUSEMOVE || repeatScrollMessage.Msg == NativeMethods.WM_DROPFILES )
					{
						Point mousePoint = this.PointToClient( Control.MousePosition );
						repeatScrollMessage.LParam = (IntPtr)MAKELPARAM( mousePoint.X, mousePoint.Y );
						if( repeatScrollMessage.Msg == NativeMethods.WM_DROPFILES )
						{
							this.inOleDragOver = true;
						}
						OnMouseMove( new MouseEventArgs( Control.MouseButtons, 0, mousePoint.X, mousePoint.Y, 0 ) );
						this.inOleDragOver = false;
					}
					else
					{
						SendMessage( repeatScrollMessage.Msg, repeatScrollMessage.WParam, repeatScrollMessage.LParam );
					}
				}
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				StopAutoScrollTimer();
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
				{
					throw;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="wparam"/>
		/// <param name="lparam"/>
		internal IntPtr SendMessage( int msg, IntPtr wparam, IntPtr lparam )
		{
			return NativeMethods.SendMessage( this.Handle, msg, wparam, lparam );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="wparam"/>
		/// <param name="lparam"/>
		internal IntPtr SendMessage( int msg, IntPtr wparam, int lparam )
		{
			return NativeMethods.SendMessage( this.Handle, msg, wparam, (IntPtr)lparam );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="wparam"/>
		/// <param name="lparam"/>
		internal IntPtr SendMessage( int msg, int wparam, IntPtr lparam )
		{
			return NativeMethods.SendMessage( this.Handle, msg, (IntPtr)wparam, lparam );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="wparam"/>
		/// <param name="lparam"/>
		internal IntPtr SendMessage( int msg, int wparam, int lparam )
		{
			return NativeMethods.SendMessage( this.Handle, msg, (IntPtr)wparam, (IntPtr)lparam );
		}

		/// <summary>
		/// Call this method from your control's OnPaint method to ensure correct
		/// rendering origin for brushes and patterns.
		/// </summary>
		/// <param name="g">The graphics object.</param>
		protected void FixRenderOrigin( Graphics g )
		{
			g.RenderingOrigin = renderOriginPoint;
		}

		/// <summary>
		/// Updates scrollbars to reflect recent changes in scroll position, minimum and maximum scroll position values.
		/// </summary>
		public virtual void UpdateScrollBars()
		{
			if( IsSplitterPaneClosing )
			{
				return;
			}
			SyncReflectScrollBars();
			HScrollBar.InitScrollBar();
			VScrollBar.InitScrollBar();
		}

		/// <summary>
		/// Call this method to check if you should do any update calculations for the view in your control and to notify scroll control
		/// that the control's content needs to be updated.
		/// </summary>
		/// <returns>True if you should invalidate areas that need to be redrawn; False if a complete Refresh for the control is pending
		/// and therefore invalidating the view is not necessary.</returns>
		public bool ShouldPrepareUpdate()
		{
			return ShouldPrepareUpdate( false );
		}

		/// <summary>
		/// Call this method to check if you should do any update calculations for the view in your control and to notify scroll control
		/// that the control's content needs to be updated.
		/// </summary>
		/// <param name="markPaintPending">If markPaintPending is True, ScrollControl will assume the control needs to be repainted in a subsequent EndUpdate call.</param>
		/// <returns>True if you should Invalidate regions to be repainted in your control. It will return False if a complete Refresh of
		/// the control is pending and you don't need to invalidate individual regions of your control.</returns>
		public bool ShouldPrepareUpdate( bool markPaintPending )
		{
			if( markPaintPending )
			{
#if DEBUG
				if( Switches.BeginEndUpdate.TraceVerbose )
				{
					TraceUtil.TraceCurrentMethodInfo( markPaintPending );
				}
#endif

				paintPending |= markPaintPending;
			}
			return updateCount == 0 || ( updateOptions & BeginUpdateOptions.Invalidate ) != BeginUpdateOptions.None;
		}

		/// <summary>
		/// Suspends the painting of the control until the <see cref="EndUpdate()"/> method is called.
		/// </summary>
		/// <remarks>
		/// When many paints are made to the appearance of a control, you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.
		/// </remarks>
		public void BeginUpdate()
		{
			BeginUpdate( BeginUpdateOptions.None );
		}

		/// <summary>
		/// Suspends the painting of the control until the <see cref="EndUpdate()"/> method is called.
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
		/// <seealso cref="ScrollControl.ShouldPrepareUpdate()"/>
		/// <seealso cref="ScrollControl.EndUpdate()"/>
		public virtual void BeginUpdate( BeginUpdateOptions options )
		{
			// see also WndProc (WM_PAINT)
#if DEBUG
			if( Switches.BeginEndUpdate.TraceVerbose )
			{
				TraceUtil.TraceCurrentMethodInfo( options, "Level " + this.updateCount.ToString() );
				TraceUtil.TraceCalledFrom( 8 );
				Trace.Indent();
			}
#endif

			// Since we can't invalidate then we should also wait with updating scrollbars.
			if( ( options & BeginUpdateOptions.Invalidate ) == BeginUpdateOptions.None )
			{
				options = BeginUpdateOptions.None;
			}

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
			{
				this.updateOptions = this.updateOptions & options;
			}
		}

		/// <summary>
		/// Calls <see cref="ScrollBarWrapper.BeginUpdate"/> for both scrollbars.
		/// </summary>
		protected virtual void OnBeginUpdateScrollBars()
		{
			this.VScrollBar.BeginUpdate();
			this.HScrollBar.BeginUpdate();
		}

		/// <summary>
		/// Calls <see cref="ScrollBarWrapper.EndUpdate"/> for both scrollbars.
		/// </summary>
		protected virtual void OnEndUpdateScrollBars()
		{
			this.VScrollBar.EndUpdate();
			this.HScrollBar.EndUpdate();
		}

		/// <summary>
		/// Resumes the painting of the control suspended by calling the BeginUpdate method.
		/// </summary>
		/// <remarks>
		/// When many paints are made to the appearance of a control, you should invoke the
		/// BeginUpdate method to temporarily freeze the drawing of the control. This results
		/// in less distraction to the user and a performance gain. After all updates have
		/// been made, invoke the EndUpdate method to resume drawing of the control.
		/// </remarks>
		/// <seealso cref="ScrollControl.BeginUpdate()"/>
		public void EndUpdate()
		{
			EndUpdate( true );
		}

		/// <summary>
		/// Cancels any prior <see cref="BeginUpdate()"/> calls.
		/// </summary>
		/// <seealso cref="ScrollControl.BeginUpdate()"/>
		public void CancelUpdate()
		{
			while( this.updateCount > 0 )
			{
				EndUpdate();
			}
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
		/// <param name="update"/>
       private bool initial = true;
		public virtual void EndUpdate( bool update )
		{
			if( this.updateCount > 0 )
			{
				this.updateCount--;
#if DEBUG
				if( Switches.BeginEndUpdate.TraceVerbose )
				{
					Trace.Unindent();
					TraceUtil.TraceCurrentMethodInfo( update, String.Concat( "Level ",
					  this.updateCount.ToString(),
					  ": PaintPending = ",
					  paintPending.ToString() ) );
					TraceUtil.TraceCalledFrom( 8 );
				}
#endif
				if( this.updateCount == 0 )
				{
					if (this.lockScrollBars)
					{
						this.lockScrollBars = false;

                        if (this.GetType().Name == "TreeViewAdv" || initial)
                        {
                           initial = false;
                           scrollbarsDirty = true;
                        }
						OnEndUpdateScrollBars();
					}
					OnUpdatingChanged( EventArgs.Empty );
					if( paintPending && update )
					{
						if( ( updateOptions & BeginUpdateOptions.Invalidate ) != BeginUpdateOptions.None )
						{
							Update();
						}
						else
						{
							Refresh();
						}
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
		/// <para>If DisableScrollWindow is True, any calls to the ScrollWindow method will simply invalidate the affect region. The rendering origin will
		/// still be recorded correctly and WindowScrolling and WindowScrolled events will be raised.</para><para>If DisableScrollWindow is False, ScrollWindow will scroll the contents of the control.
		/// </para></remarks>
		/// <returns></returns>
		/// <param name="xAmount"/>
		/// <param name="yAmount"/>
		/// <param name="rect"/>
		/// <param name="clipRect"/>
		/// <param name="allowUpdate"/>
		[EditorBrowsable( EditorBrowsableState.Advanced )]
		public virtual Rectangle ScrollWindow( int xAmount, int yAmount, Rectangle rect, Rectangle clipRect, bool allowUpdate )
		{
			// TODO: Do I need a PermissionSet since ScrollWindow calls NativeMethods ?
			//Rectangle invalidRect = Rectangle.Empty;

			OnWindowScrolling( new ScrollWindowEventArgs( xAmount, yAmount, rect, clipRect, Rectangle.Empty ) );

			// Note: there might be a problem when changing column widths in a grid and
			// using ScrollWindow because renderOriginPoint should not be changed then.
			OffsetRenderOriginPoint( xAmount, yAmount );

			if( DisableScrollWindow )
			{
				if( !rect.IsEmpty )
				{
					clipRect = Rectangle.Union( rect, clipRect );
				}
				Invalidate( clipRect );
			}
			else if( HasDoubleBufferSurface )
			{
				Rectangle invalidRect = DoubleBufferSurface.ScrollWindow( xAmount, yAmount, rect, clipRect );
				if( allowUpdate )
				{
					ScrollWindowInvalidate( invalidRect );
				}
			}
			else
			{
				try
				{
					NativeMethods.RECT lpRect = new NativeMethods.RECT( rect );
					NativeMethods.RECT lpClipRect = new NativeMethods.RECT( clipRect );
					NativeMethods.RECT rcUpdate = new NativeMethods.RECT();
					int flags = NativeMethods.SW_SCROLLCHILDREN; //|NativeMethods.SW_INVALIDATE;
					//					if (bErase)
					//						flags |= NativeMethods.SW_ERASE;

					if( allowUpdate )
					{
						IntPtr hRgnUpdate = NativeMethods.CreateRectRgn( 0, 0, 0, 0 );
						if( rect.IsEmpty )
						{
							NativeMethods.ScrollWindowEx( Handle, xAmount, yAmount, (NativeMethods.COMRECT)null, ref lpClipRect, hRgnUpdate, ref rcUpdate, flags );
						}
						else
						{
							NativeMethods.ScrollWindowEx( Handle, xAmount, yAmount, ref lpRect, ref lpClipRect, hRgnUpdate, ref rcUpdate, flags );
						}

						NativeMethods.RECT[] nativeRects = NativeMethods.RegionCracker.CrackRegionData( hRgnUpdate );
						for( int n = 0; n < nativeRects.Length; n++ )
						{
							if( n == 0 && n < nativeRects.Length - 1 )
							{
								this.Update();
							}
							ScrollWindowInvalidate( new Rectangle( nativeRects[n].left, nativeRects[n].top, nativeRects[n].Width, nativeRects[n].Height ) );
							if( n < nativeRects.Length - 1 )
							{
								this.Update();
							}
						}
						NativeMethods.DeleteObject( hRgnUpdate );
					}
					else
					{
						flags = NativeMethods.SW_SCROLLCHILDREN | NativeMethods.SW_INVALIDATE;
						if( rect.IsEmpty )
						{
							NativeMethods.ScrollWindowEx( Handle, xAmount, yAmount, (NativeMethods.COMRECT)null, ref lpClipRect, IntPtr.Zero, ref rcUpdate, flags );
						}
						else
						{
							NativeMethods.ScrollWindowEx( Handle, xAmount, yAmount, ref lpRect, ref lpClipRect, IntPtr.Zero, ref rcUpdate, flags );
						}
					}
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					{
						throw;
					}
					// In case we don't have permission to call ScrollWindowEx disable future calls.
					disableScrollWindow = true;
				}
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

			if( !disableScrollWindow && !allowUpdate )
			{
				ScrollWindowInvalidate( updateRect );
			}

			OnWindowScrolled( new ScrollWindowEventArgs( xAmount, yAmount, rect, clipRect, updateRect ) );

			//			invalidRect = Rectangle.FromLTRB(rcUpdate.left, rcUpdate.top, rcUpdate.right, rcUpdate.bottom);
			//			OnInvalidated(new InvalidateEventArgs(invalidRect));
			return updateRect;
		}

		/// <exclude/>
		/// <summary></summary>
		/// <param name="r"/>
		protected virtual void ScrollWindowInvalidate( Rectangle r )
		{
			Invalidate( r );
		}

		/// <exclude/>
		/// <summary>
		/// Discards paint messages.
		/// </summary>
		/// <returns></returns>
		public Rectangle DiscardPaintMessages()
		{
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			NativeMethods.BeginPaint( Handle, ref ps );
			Rectangle rc = Rectangle.FromLTRB( ps.rcPaint_left, ps.rcPaint_top, ps.rcPaint_right, ps.rcPaint_bottom );
			NativeMethods.EndPaint( Handle, ref ps );
			return rc;
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void ChildGotFocus( object sender, EventArgs e )
		{
			RaiseControlGotFocus();
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void ChildLostFocus( object sender, EventArgs e )
		{
			RaiseControlLostFocus();
		}

		/// <summary>
		/// Indicates whether this control contains focus. Override this method if you
		/// want to show drop-down windows and indicate the control has not lost focus when
		/// the drop-down is shown.
		/// </summary>
		/// <returns>True if the control or any child control has focus; false otherwise.</returns>
		public virtual bool QueryFocusInside()
		{
			return ContainsFocus;
		}

		/// <summary></summary>
		private void RaiseControlGotFocus()
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			if( !this.hasControlFocus )
			{
				hasControlFocus = true;
				OnControlGotFocus();
			}
		}

		/// <summary></summary>
		private void RaiseControlLostFocus()
		{
			if( this.Disposing || this.IsDisposed )
			{
				return;
			}

			if( !hasControlFocus )
			{
#if DEBUG
				if( Switches.ScrollControlFocus.TraceVerbose )
				{
					TraceUtil.TraceCurrentMethodInfo( "-Duplicate call-", isValidating, this );
				}
#endif

			}
			else if( !QueryFocusInside() )
			{
				hasControlFocus = false;
				OnControlLostFocus();
			}
		}

		/// <override/>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			base.OnRightToLeftChanged( e );

			if( this.IsHandleCreated && !ClientRectangle.IsEmpty )
			{
				AdjustSizeBox();
			}
		}

		/// <summary></summary>
		private void AdjustSizeBox()
		{
			// Check for custom scrollbars.
			bool vCustomScroll = ( vScroll && this.VScrollBar.InnerScrollBar is ScrollBarCustomDraw );
			bool hCustomScroll = ( hScroll && this.HScrollBar.InnerScrollBar is ScrollBarCustomDraw );

			// Position custom scrollbars.
			if( vCustomScroll )
			{
				ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.VScrollBar.InnerScrollBar;
				Rectangle r = base.ClientRectangle;

				if( !( this.RightToLeft == RightToLeft.Yes ) )
					r.X = r.Width - sbar.Width;

				r.Width = sbar.Width;

				int height = r.Height;
				if( hCustomScroll )
				{
					height -= this.HScrollBar.InnerScrollBar.Height;
					r.Height = height;
				}

				sbar.Bounds = r;
				sbar.BringToFront();
			}

			if( hCustomScroll )
			{
				ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.HScrollBar.InnerScrollBar;
				Rectangle r = base.ClientRectangle;
				r.Y = r.Height - sbar.Height;
				r.Height = sbar.Height;

				int width = r.Width;
				if( vCustomScroll )
				{
					width -= this.VScrollBar.InnerScrollBar.Width;
					r.Width = width;
					if( this.RightToLeft == RightToLeft.Yes )
						r.X += this.VScrollBar.InnerScrollBar.Width;
				}

				sbar.Bounds = r;
				sbar.BringToFront();
			}

			// Need to place a size box in bottom-right corner when drawing custom scrollbars.
			bool showSizeBox = ( ( vCustomScroll && hCustomScroll ) || forceSizeBox ) && this.Visible;
			bool showSizeGrip = false;
			Form f = Parent as Form;

			if( smartSizeBox && Visible && f != null && this.HScroll && this.VScroll )
			{
				//Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(this.Dock, f.WindowState, f.IsMdiChild, f.SizeGripStyle, f.FormBorderStyle, f.ClientSize, f.IsMdiContainer);
				if( this.Dock != DockStyle.Fill )
				{
					showSizeBox = true;
					showSizeGrip = false;
				}
				else if( !f.IsMdiChild )
				{
					showSizeGrip = f.FormBorderStyle == FormBorderStyle.Sizable && f.WindowState == FormWindowState.Normal;
					if( showSizeGrip && allowSizeGrip )
					{
						if( this.allowSizeGrip )
						{
							f.SizeGripStyle = SizeGripStyle.Show;
						}
					}
					else
					{
						if( this.allowSizeGrip )
						{
							f.SizeGripStyle = SizeGripStyle.Hide;
						}
						showSizeBox = true;
					}
				}
				else
				{
					showSizeBox = true;
					showSizeGrip = f.WindowState != FormWindowState.Maximized;
				}
			}

			if( showSizeBox )
			{
				if( sizeBox == null )
				{
					sizeBox = new SizeBox( this );
                    if (!this.MetroScrollBars)
                        sizeBox.BackColor = SystemColors.Control;
                    else
                        sizeBox.BackColor = MetroColorTable.ScrollerBackground;
					sizeBox.Visible = false;
					this.Controls.Add( sizeBox );
				}

				sizeBox.SizeGripStyle = this.SizeGripStyle;

				sizeBox.Bounds = new Rectangle( this.VScrollBar.InnerScrollBar.Left, this.HScrollBar.InnerScrollBar.Top,
					SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight );

				sizeBox.Visible = true;
				sizeBox.BringToFront();
			}
			else if( sizeBox != null )
			{
				sizeBox.Visible = false;
			}
		}

		/// <summary>
		/// Checks if the control is visible and a window handle has been created.
		/// If it has not been drawn before, it calls <see cref="OnEnsurePaintCodeJitted"/>.
		/// </summary>
		/// <param name="ignoreVisible">Set this True if you want to force a call to
		/// <see cref="OnEnsurePaintCodeJitted"/> even if the control is not visible and / or
		/// no window handle has been created.</param>
		public void EnsurePaintCodeJitted( bool ignoreVisible )
		{
			if( ( ignoreVisible || this.Visible && this.IsHandleCreated ) && !onPaintCalled )
			{
				OnEnsurePaintCodeJitted();
				onPaintCalled = true;
			}
		}

		/// <summary>
		/// Enables the DoubleBufferSurface for this control. The DoubleBufferSurface buffering
		/// is different from the automatic .NET double buffering and also a bit slower but
		/// it reduces flicker if lots of scrolling is used and gdi drawing for individual cells
		/// in a grid.
		/// </summary>
		public void EnableDoubleBufferSurface()
		{
			SetStyle( ControlStyles.Opaque, true );
			SetStyle( ControlStyles.ResizeRedraw, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
#if ! ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			SetStyle( ControlStyles.OptimizedDoubleBuffer, false );
#else
      SetStyle( ControlStyles.DoubleBuffer, false );
#endif
			this.hasDoubleBufferSurface = true;
		}

		/// <summary></summary>
		/// <param name="gr"/>
		private void OnPrepareDoubleBufferSurfaceGraphics( Graphics gr )
		{
		}

		/// <summary>
		/// Forces the assigned styles to be reapplied to the control.
		/// </summary>
		public new void UpdateStyles()
		{
			m_bUpdatingStyles = true;

			base.UpdateStyles();

			m_bUpdatingStyles = false;
		}

		#region Class internal declarations
		/// <summary>
		/// Delegate for ScrollControlMouseDown, OnBeforeMouseMove and ScrollControlMouseUp methods.
		/// </summary>
		/// <returns></returns>
		/// <param name="e"/>
		internal delegate void CancelMouseDelegate( CancelMouseEventArgs e );
		#endregion
	}

	/// <summary></summary>
	internal class SizeBox: Control
	{
		/// <summary></summary>
		private Control target;
		/// <summary> </summary>
		private FormWindowState parentFormWindowState;
		/// <summary></summary>
		SizeGripStyle sizeGripStyle;
		/// <summary></summary>
		/// <param name="target"/>
		public SizeBox( Control target )
		{
			this.target = target;
			this.TabStop = false;
			SetStyle( ControlStyles.Selectable | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, false );
		}

		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this._themedScrollBarDrawing != null )
				{
					this._themedScrollBarDrawing.Dispose();
					this._themedScrollBarDrawing = null;
				}
				target = null;
			}
			base.Dispose( disposing );
		}

		public FormWindowState ParentFormWindowState
		{
			get { return parentFormWindowState; }
			set
			{
				parentFormWindowState = value;
				Invalidate();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SizeGripStyle SizeGripStyle
		{
			get { return sizeGripStyle; }
			set { sizeGripStyle = value; }
		}

		/// <summary></summary>
		bool ShowSizeGrip
		{
			get
			{
				if( ScrollControl.IgnoreSizeGripStyleHideShow )
					return CanResizeThroughSizeBox();

				switch( sizeGripStyle )
				{
					case SizeGripStyle.Auto:
					return CanResizeThroughSizeBox();
					case SizeGripStyle.Hide:
					return false;
					case SizeGripStyle.Show:
					return true;
					default:
					return false;
				}
			}
		}

		bool CanResizeThroughSizeBox()
		{
			bool showGrip = ( sizeGripStyle != SizeGripStyle.Hide || ScrollControl.IgnoreSizeGripStyleHideShow ) && parentFormWindowState != FormWindowState.Maximized
                    && this.RightToLeft != RightToLeft.Yes && ( target.Parent != null && target.Parent is Form
                    && ( target.Dock == DockStyle.Fill || target.Dock == DockStyle.Bottom || target.Dock == DockStyle.Right ) );

			// isDockedToParentFormRightBottom
			if( showGrip )
			{
				return this.target.Parent.ClientRectangle.Bottom == target.Bounds.Bottom &&
                   this.target.Parent.ClientRectangle.Right == this.target.Bounds.Right;
			}
			return false;
		}


		/// <summary></summary>
		private ThemedScrollBarDrawing themedScrollBarDrawing
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsAppThemed )
				{
					if( _themedScrollBarDrawing == null )
					{
						this._themedScrollBarDrawing = new ThemedScrollBarDrawing( this );
					}
				}
                else if (this._themedScrollBarDrawing == null)
                {
                    this._themedScrollBarDrawing = new ThemedScrollBarDrawing(this);
                }
				return this._themedScrollBarDrawing;
			}
		}

		/// <summary></summary>
		private ThemedScrollBarDrawing _themedScrollBarDrawing = null;

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( ShowSizeGrip )
			{
				bool bMirrored = ( this.RightToLeft == RightToLeft.Yes );

				using( CMirroredDrawer md3DBorder = new CMirroredDrawer( e.Graphics, this.ClientRectangle, bMirrored, false ) )
				{
					bool themed = target is IThemedControl && XPThemes.IsThemedOS && ( (IThemedControl)target ).ThemesEnabled;
					if( themed )
					{
						themedScrollBarDrawing.DrawSizeBox( md3DBorder.VirtualGfx, md3DBorder.VirtualBounds );
					}
					else
					{
						ControlPaint.DrawSizeGrip( md3DBorder.VirtualGfx, BackColor, md3DBorder.VirtualBounds );
					}
				}
			}
			base.OnPaint( e );
		}

		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_NCHITTEST:
				{
					base.WndProc( ref m );
					if( m.Result.ToInt32() == NativeMethods.HTCLIENT && CanResizeThroughSizeBox() )
					{
						m.Result = new IntPtr( NativeMethods.HTBOTTOMRIGHT );
					}

					break;
				}
				default:
				base.WndProc( ref m );
				break;
			}

		}
	}

	/// <summary>
	/// Handles a cancellable <see cref="ScrollControl.ShowContextMenu"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name=" e">An <see cref="ShowContextMenuEventArgs"/> that contains the event data.</param>
	/// <returns></returns>
	/// <param name="e"/>
	public delegate void ShowContextMenuEventHandler( object sender, ShowContextMenuEventArgs e );

	/// <summary>
	/// Provides data about the cancellable <see cref="ScrollControl.ShowContextMenu"/> event.
	/// </summary>
	/// <remarks>
	/// ShowContextMenuEventArgs is a custom event argument class used by the
	/// <see cref="ScrollControl.ShowContextMenu"/> event when the user right-clicks inside
	/// the control.
	/// <para/>
	/// You can cancel showing a context menu when
	/// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/></remarks>
	public sealed class ShowContextMenuEventArgs: SyncfusionCancelEventArgs
	{
		/// <summary></summary>
		private Point point;

		// TODO: Sample how to allow maximum height.

		/// <summary>
		/// Initializes a new object.
		/// </summary>
		/// <param name="point">The mouse location in screen coordinates.</param>
		public ShowContextMenuEventArgs( Point point )
		{
			this.point = point;
		}

		/// <summary>
		/// Returns the mouse location in screen coordinates.
		/// </summary>
		[TraceProperty( true )]
		public Point Point
		{
			get
			{
				return point;
			}
		}
	}
}