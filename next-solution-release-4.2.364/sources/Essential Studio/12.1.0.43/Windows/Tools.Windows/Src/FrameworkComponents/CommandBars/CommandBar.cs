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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the edges of the host form along which <see cref="CommandBar"/>s may be docked.
	/// </summary>
	/// <remarks>
	/// The CommandBarDockBorder enumeration is used with the <see cref="CommandBarController.EnabledDockBorders"/>
	/// and <see cref="CommandBar.AllowedDockBorders"/> properties for specifying the host form edges along which
	/// <see cref="CommandBar"/>s may be docked.
	/// <p>
	/// This enumeration has the FlagsAttribute that allows a bitwise combination of its member values.
	/// </p>
	/// </remarks>
	[
	Flags,
	Editor( typeof( Syncfusion.Windows.Forms.Design.EnumFlagsEditor ), typeof( System.Drawing.Design.UITypeEditor ) )
	]
	public enum CommandBarDockBorder
	{
		/// <summary>
		/// None of the host form's borders is enabled for docking.
		/// </summary>
		None=0x0000,
		/// <summary>
		/// The top edge is enabled for docking.
		/// </summary>
		Top=0x0001,
		/// <summary>
		/// The bottom edge is enabled for docking.
		/// </summary>
		Bottom=0x0002,
		/// <summary>
		/// The left edge is enabled for docking.
		/// </summary>
		Left=0x0004,
		/// <summary>
		/// The right edge is enabled for docking.
		/// </summary>
		Right=0x0008
	}


	/// <summary>
	/// Provides state information for a <see cref="CommandBar"/>.
	/// </summary>
	/// <remarks>
	/// The CommandBarDockState enumeration is used with the <see cref="CommandBar.DockState"/>
	/// property for specifying the docked/floating state of the <see cref="CommandBar"/>.
	/// </remarks>
	/// <see cref="CommandBar.DockState"/>
	public enum CommandBarDockState
	{
		/// <summary>
		/// The CommandBar is in an uninitialized state.
		/// </summary>
		None=0x0000,
		/// <summary>
		/// The CommandBar is docked to the top border of the form.
		/// </summary>
		Top=0x0001,
		/// <summary>
		/// The CommandBar is docked to the bottom border of the form.
		/// </summary>
		Bottom=0x0002,
		/// <summary>
		/// The CommandBar is docked to the left border of the form.
		/// </summary>
		Left=0x0004,
		/// <summary>
		/// The CommandBar is docked to the right border of the form.
		/// </summary>
		Right=0x0008,
		/// <summary>
		/// The CommandBar is in a floating state.
		/// </summary>
		Float=0x0020
	}


	/// <summary>
	/// Specifies the type of resizing that a <see cref="CommandBar"/> is undergoing.
	/// </summary>
	/// <remarks>
	/// The CommandBarResizeType enumeration is used by the <see cref="CommandBar.CommandBarWrapping"/> event to provide
	/// information on the type of resizing.
	/// <see cref="CommandBar.CommandBarWrapping"/>
	/// </remarks>
	[
	Flags
	]
	public enum CommandBarResizeType
	{
		/// <summary>
		/// No resizing.
		/// </summary>
		None=0x0000,
		/// <summary>
		/// A floating CommandBar is being resized with the top edge being used as the resize handle.
		/// </summary>
		Top=0x0001,
		/// <summary>
		/// A docked/floating CommandBar is being resized with the bottom edge being used as the resize handle.
		/// </summary>
		Bottom=0x0002,
		/// <summary>
		/// A floating CommandBar is being resized with the left edge being used as the resize handle.
		/// </summary>
		Left=0x0004,
		/// <summary>
		/// A docked/floating CommandBar is being resized with the right edge being used as the resize handle.
		/// </summary>
		Right=0x0008
	}

	[Flags, Syncfusion.Documentation.DocumentationExclude()]
	public enum CBButtons
	{
		None=0x0000,
		DropDown=0x0001,
		Close=0x0002,
		Pressed=0x0004
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum CBButtonState
	{
		Normal=0x0001,
		Hot=0x0002,
		Pressed=0x0003
	}

	///<summary>
	/// Provides information about the <see cref="CommandBar.CommandBarWrapping"/> event.
	///</summary>
	/// <remarks>
	/// CommandBarWrappingEventArgs is a custom event argument class used by the <see cref="CommandBar"/> class
	/// for notifying users of wrap events and also for getting the new size of the CommandBar's client
	/// control during custom wrapping.
	/// </remarks>
	/// <seealso cref="CommandBarWrappingEventHandler"/>.
	public class CommandBarWrappingEventArgs: EventArgs
	{
		#region Class Members
		protected Size szClient = Size.Empty;
		protected CommandBarResizeType cbResizeType = CommandBarResizeType.None;
		#endregion

		#region Class Initialize/Finalize Methods
		/// <summary>
		/// Creates a new instance of the CommandBarWrappingEventArgs class.
		/// </summary>
		/// <param name="clientsize">The current/proposed size of the <see cref="CommandBar"/>'s client control.</param>
		/// <param name="resizetype">A <see cref="CommandBarResizeType"/> value specifying the type of resizing.</param>
		public CommandBarWrappingEventArgs( Size clientsize, CommandBarResizeType resizetype )
		{
			this.szClient = clientsize;
			this.cbResizeType = resizetype;
		}
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets / sets the current/proposed size of the <see cref="CommandBar"/>'s client control.
		/// </summary>
		/// <value>A Size value indicating the width and height for the CommandBar's client control.</value>
		public Size ClientSize
		{
			get { return this.szClient; }
			set { this.szClient = value; }
		}

		/// <summary>
		/// Returns the type of resizing taking place.
		/// </summary>
		/// <value>A <see cref="CommandBarResizeType"/> value.</value>
		public CommandBarResizeType CommandBarResizeType
		{
			get { return this.cbResizeType; }
		}
		#endregion
	}


	/// <summary>
	/// Delegate that will handle the <see cref="CommandBar.CommandBarWrapping"/> event.
	/// </summary>
	/// <param name="obj"> The source of the event.</param>
	/// <param name="arg"> A <see cref="CommandBarWrappingEventArgs"/> that contains the event data.</param>
	public delegate void CommandBarWrappingEventHandler( Object obj, CommandBarWrappingEventArgs arg );


	/// <summary>
	/// Provides data for the <see cref="CommandBar.CommandBarStateChanging"/> event.
	/// </summary>
	/// <remarks>
	/// The CommandBarStateChangingEventArgs is a custom event argument class used by the
	/// <see cref="CommandBar.CommandBarStateChanging"/> event for notifying subscribers
	/// that a <see cref="CommandBar"/>'s dock/float state is about to change.
	/// </remarks>
	/// <see cref="CommandBarStateChangingEventHandler"/>
	public class CommandBarStateChangingEventArgs: EventArgs
	{
		#region Class Members
		protected CommandBarDockState cbarDockState = CommandBarDockState.None;

		/// <summary>
		/// Indicating whether 
		/// the event should be canceled.
		/// </summary>
		protected bool m_cancel = false;

		#endregion

		#region Class Initialize/Finalize Methods
		/// <summary>
		/// Creates an instance of the CommandBarStateChangingEventArgs class.
		/// </summary>
		/// <param name="state">A <see cref="CommandBarDockState"/> value indicating the new state of the <see cref="CommandBar"/>.</param>
		public CommandBarStateChangingEventArgs( CommandBarDockState state )
		{
			this.cbarDockState = state;
		}
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets / sets the <see cref="CommandBar"/>'s new position.
		/// </summary>
		/// <value>A <see cref="CommandBarDockState"/> value.</value>
		public CommandBarDockState NewDockState
		{
			get { return this.cbarDockState; }
			set { this.cbarDockState = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// the event should be canceled.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_cancel;
			}
			set
			{
				if( value != m_cancel )
				{
					m_cancel = value;
				}
			}
		}
		#endregion
	}


	/// <summary>
	/// Delegate that will handle the <see cref="CommandBar.CommandBarStateChanging"/> event.
	/// </summary>
	/// <param name="obj"> The source of the event.</param>
	/// <param name="arg"> A <see cref="CommandBarStateChangingEventArgs"/> value that contains the event data.</param>
	public delegate void CommandBarStateChangingEventHandler( Object obj, CommandBarStateChangingEventArgs arg );


	/// <summary>
	/// Implements a container for creating dockable toolbar, statusbar and rebar type controls.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The CommandBar class is a part of the Essential Tools CommandBars framework and allows
	/// Windows Forms developers to add to their applications dockable command bars similar to
	/// those that are present in the Microsoft Visual Studio.NET and Office XP environments.
	/// </p>
	/// <p>
	/// The CommandBar, similar to to the MFC/Win32 control bars, is a container control that primarily
	/// serves as a host for some other Windows Forms control. The CommandBar class implements the hosting
	/// and layout logic while the contained control provides the functional significance.
	/// </p>
	/// </remarks>
	/// <seealso cref="CommandBarController"/>
	[
	ToolboxItem( false ),
	Designer( typeof( Syncfusion.Windows.Forms.Tools.Design.CommandBarDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) )
	]
	public class CommandBar: System.Windows.Forms.Control, IPopupParent, ICommandBarDesignerInvoke, ICommandBarDesignerMouseHook
	{
		#region Class Members
		[Syncfusion.Documentation.DocumentationExclude()]
		internal CommandBarController cbController = null;

		// Painting Dimension attributes
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int nGprXOff = 2;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int nGprYOff = 4;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int nGprCx = 2;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bRedockNeededInternal = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bRedockNeeded = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bRecalcNeeded = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFireDockStateChanged = false;

		// UI attributes
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bHideGripper = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bDockModeWrapping = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bFloatModeWrapping = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bShowDockModeText = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bHideChevron = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bHideDropDown = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bHideCloseButton = false;

		// Layout attributes
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal  bool bFullRow = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bDisableFloating = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bDisableDocking = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nMaxLength = 200;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nMinLength = 50;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected internal int nDefaultUnitHt = 26;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nCommandBarHt = 32;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nIntegral = 1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bLeadingEdge = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bTrailingEdge = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandBarDockBorder allowedBorders = CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom;

		// Graphics Objects
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected Font ftFloatCaption = null;

		// State Information
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar cdbParent = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandBarDockState cbarDockState = CommandBarDockState.Top;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRowOffsetDir = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRowOffsetInDir = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRCIndex = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bVisible = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bVisibilitySetFlag = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle rcFloat = Rectangle.Empty;

		// Temporary State storage
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandBarDockState cbDockStateT = CommandBarDockState.None;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRowOffsetDirT = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRowOffsetInDirT = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRCIndexT = -1;

		// Temporary Drag State storage
		[Syncfusion.Documentation.DocumentationExclude()]
		protected CommandDockBar cdbParentDrag = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nRowOffsetDrag = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRCIndexDrag = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nRCCountDrag = 0;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected CBButtons cbHilight = CBButtons.None;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected internal bool bDragging = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected internal  Point ptDeltaOff = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected Point ptMMWorkaround = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected Cursor crDefault = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected bool bHideOnMouseUp = false;
		/// <summary>
		/// Indicates whether to refresh floating caption text color and text font.
		/// </summary>
		internal bool m_needRefreshTextData = true;
		/// <summary>
		/// Color for chevron.
		/// </summary>
		private Color m_chevronColor = SystemColors.ControlText;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bBackColorSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bUpdateOffsets = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bRestrictedSizing = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal RowMarker rMarker = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected static int nSysInfoMenuFontHeight = SystemInformation.MenuFont.Height;
		internal CommandBarWeakContainer commandBarWeakContainer = null;
		private bool m_bIsCustomizing = false;
		private PopupControlContainer popupContainer;
		/// <summary>
		/// Reference to renderer for docked CommandBar.
		/// </summary>
		private CommandBarRenderer m_renderer = null;
		/// <summary>
		/// Reference to renderer for floating CommandBar
		/// </summary>
		private CommandBarFloatingRenderer m_rendererFloating = null;
		#endregion

		#region Class Events
		/// <summary>
		/// Occurs when the <see cref="CommandBar"/> is being wrapped.
		/// </summary>
		/// <remarks>
		/// The CommandBarWrapping event is fired whenever the CommandBar is in the process of
		/// being wrapped. Handle this event for customizing the resizing/wrapping behavior
		/// of the CommandBar.
		/// <p>
		/// See <see cref="CommandBarWrappingEventHandler"/> and <see cref="CommandBarWrappingEventArgs"/> for
		/// more information.
		/// </p>
		/// </remarks>
		[
		Category( "CommandBar Events" ),
		Description( "Occurs when the CommandBar is being wrapped." ),
		]
		public event CommandBarWrappingEventHandler CommandBarWrapping;

		/// <summary>
		/// Occurs when the dropdown button on a <see cref="CommandBar"/> is clicked.
		/// </summary>
		/// <remarks>
		/// The CommandBarDropDownClicked event is fired when the dropdown button on a
		/// <see cref="CommandBar"/> is clicked.
		/// </remarks>
		[
		Category( "CommandBar Events" ),
		Description( "Occurs when the dropdown button on the CommandBar is clicked." ),
		]
		public event System.EventHandler CommandBarDropDownClicked;

		/// <summary>
		/// Occurs when a <see cref="CommandBar"/>'s dock/float state is about to change.
		/// </summary>
		/// <remarks>
		/// The CommandBarStateChanging event is fired before a CommandBar's dock/float state changes.
		/// Handle this event to perform any custom processing before state changes.
		/// <see cref="CommandBarStateChangingEventHandler"/> and <see cref="CommandBarStateChangingEventArgs"/>
		/// </remarks>
		/// <seealso cref="CommandBar.CommandBarStateChanged"/>
		[
		Category( "CommandBar Events" ),
		Description( "Occurs just before the CommandBar's dock/float state changes." )
		]
		public event CommandBarStateChangingEventHandler CommandBarStateChanging;

		/// <summary>
		/// Occurs after a <see cref="CommandBar"/>'s dock/float state changes.
		/// </summary>
		/// <remarks>
		/// The CommandBarStateChanged event is fired after a CommandBar's dock/float state changes.
		/// Handle this event to perform any custom processing required after a state change.
		/// </remarks>
		/// <seealso cref="CommandBar.CommandBarStateChanging"/>
		[
		Category( "CommandBar Events" ),
		Description( "Occurs after the CommandBar's dock/float state changes." )
		]
		public event System.EventHandler CommandBarStateChanged;

		/// <summary>
		/// Occurs when a floating <see cref="CommandBar"/> is hidden by the user.
		/// </summary>
		/// <remarks>
		/// The <see cref="CommandBarUserClosed"/> event occurs when a floating CommandBar is hidden as a
		/// result of the user clicking the close button.
		/// </remarks>
		[
		Category( "CommandBar Events" ),
		Description( "Occurs when the user presses the close button." )
		]
		public event System.EventHandler CommandBarUserClosed;

		#endregion

		#region Class Properties

		[
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )
		]
		public static int CaptionHeight
		{
			get { return ( CommandBar.nSysInfoMenuFontHeight + 3 ); }
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )]
		public static int FormMinWidth
		{
			get { return ( CommandBar.CaptionHeight*2 ) + 20; }	// Min width should be equal to the 2 caption button widths + text
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle CaptionRect
		{
			get { return new Rectangle( 1, 1, this.Width-2, CommandBar.CaptionHeight ); }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle GripperRect
		{
			get
			{
				Rectangle rcgripper = Rectangle.Empty;
				if( ( this.bHideGripper == false ) && ( this.cdbParent != null ) )
				{
					Rectangle rcclient = this.ClientRectangle;
					bool bRTL = ( this as IPopupParent ).IsRightToLeft;
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						rcgripper = new Rectangle( ( bRTL ? rcclient.Right - nHeaderOff: rcclient.Left ),
							rcclient.Top, nHeaderOff, rcclient.Height );
					}
					else
					{
						rcgripper = new Rectangle( rcclient.Left,
							( bRTL ? rcclient.Bottom-nHeaderOff : rcclient.Top ),
							rcclient.Width, nHeaderOff );
					}
				}
				return rcgripper;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle DropDownRect
		{
			get
			{
				Rectangle rcdd = ( ( this.cbarDockState == CommandBarDockState.None ) || ( ( this.bHideDropDown ) && ( !this.IsChevronVisible ) ) ) ?
					Rectangle.Empty : GetDropDownRect();
				return rcdd;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle CloseButtonRect
		{
			get
			{
				if( ( this.bHideCloseButton == true ) || ( this.cbarDockState != CommandBarDockState.Float ) )
					return Rectangle.Empty;
				int nCaptionHt = CommandBar.CaptionHeight;
				Rectangle rccaption = this.CaptionRect;
				int nRectLeft = this.IsRTL ? rccaption.Left : rccaption.Right-nCaptionHt;
				return new Rectangle( nRectLeft, rccaption.Top, nCaptionHt, nCaptionHt );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle DragStartRect
		{
			get
			{
				if( this.cbarDockState == CommandBarDockState.None )
					return Rectangle.Empty;

				if( this.cbarDockState == CommandBarDockState.Float )
					return this.CaptionRect;
				else return this.GripperRect;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual int nHeaderOff
		{
			get
			{
				if( this.bHideGripper == false )
					return 8;
				else
					return 0;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual internal int nTrailBtnOff
		{
			get
			{
				if( this.cbarDockState == CommandBarDockState.None )
					return 0;

				int nretval = 0;

				if( this.AllowQuickCustomizing || ( this.HasExternalPopupMenu && ( !this.HideDropDownButton || this.Floating ) ) )
				{
					nretval = 11;
				}
				else if( this.cdbParent != null && !this.bHideChevron && this.cbarDockState != CommandBarDockState.Float )
				{
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						if( this.Width < this.nMaxLength )
							nretval = 11;
					}
					else if( this.Height < this.nMaxLength )
					{
						nretval = 11;
					}
				}
				if( ( nretval > 0 ) && this.ShouldDrawThemed() && cbController != null
					&& cbController.Style != VisualStyle.Office2007
                    && this.cbController.Style != VisualStyle.Office2010
					&& cbController.Style != VisualStyle.Office2007Outlook )
				{
					return 13;
				}

				return nretval;
			}
		}

		protected internal virtual bool HasExternalPopupMenu
		{
			get
			{
				return null != this.PopupMenu;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public CommandBarController Controller
		{
			get { return this.cbController; }
			set
			{
				this.cbController = value;
				if( this.cbController != null )
				{
					if( this.ShouldDrawThemed() 
						|| this.cbController.Style == VisualStyle.Office2003 
						|| this.cbController.Style == VisualStyle.VS2005
						|| this.cbController.Style == VisualStyle.Office2007 
                        || this.cbController.Style == VisualStyle.Office2010
						|| this.cbController.Style == VisualStyle.Office2007Outlook )
					{
						if( this.bBackColorSet == false )
						{
							if( this.Floating )
							{
								base.BackColor = this.Parent.BackColor;
							}
							else
							{
								base.BackColor = System.Drawing.Color.Transparent;
							}
						}
					}

					cbController.StyleChanged += new EventHandler( cbController_StyleChanged );
					cbController.ThemesEnabledChanged += new EventHandler( cbController_ThemesEnabledChanged );

					if( null != this.pupMenu && null != this.pupMenu.ParentBarItem )
					{
						this.pupMenu.ParentBarItem.Style = this.cbController.Style;
					}
				}
			}
		}
		/// <summary>
		/// Gets / sets the current dock or float state for the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>A <see cref="CommandBarDockState"/> value.</value>
		[
		Category( "Layout" ),
		Description( "Contextual position information for the CommandBar." ),
		Browsable( false ),
		Localizable( true )
		]
		public CommandBarDockState DockState
		{
			get { return this.cbarDockState; }
			set
			{
				if( ( this.bDisableDocking == true ) && ( value != CommandBarDockState.Float ) )
					return;

				if( ( this.bDisableFloating == true ) && ( value == CommandBarDockState.Float ) )
					return;

				if( ( this.Parent != null ) && ( this.cbarDockState != value ) )
				{
					this.cbDockStateT = value;
					this.bRedockNeeded = true;

					this.RedockIfNeeded();
					this.RecalcIfNeeded();
				}
				else
					this.cbarDockState = value;
			}
		}

		/// <summary>
		/// Gets / sets the edges of the Form along which the <see cref="CommandBar"/> may be docked.
		/// </summary>
		/// <value>A <see cref="CommandBarDockBorder"/> value.</value>
		[
		Category( "Behavior" ),
		Description( "The edges of the Form to which the CommandBar may be docked." ),
		DefaultValue( CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom ),
		Localizable( true )
		]
		public CommandBarDockBorder AllowedDockBorders
		{
			get { return this.allowedBorders; }

			set
			{
				if( this.allowedBorders != value )
					this.allowedBorders = value;
			}
		}

		/// <summary>
		/// Gets / sets the linear offset of the <see cref="CommandBar"/> within a row.
		/// </summary>
		/// <value> An integer value representing the row offset. </value>
		[
		Category( "Layout" ),
		Description( "The positional offset for the CommandBar within a row." ),
		Browsable( false ),
		Localizable( true )
		]
		public virtual int RowOffset
		{
			get { return this.nRowOffsetDir; }
			set
			{
				if( ( this.Parent != null ) && ( this.nRowOffsetDir != value ) )
				{
					this.nRowOffsetDirT = value;
					this.bRecalcNeeded = true;
					this.Invalidate();
				}

				this.nRowOffsetDir = value;
				this.nRowOffsetInDir = value;
			}
		}

		/// <summary>
		/// Gets / sets the index of the row/column for the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>A zero-based integer value representing the row index.</value>
		[
		Category( "Layout" ),
		Description( "The zero-based index of the row in which the CommandBar is docked." ),
		Browsable( false ),
		Localizable( true )
		]
		public int RowIndex
		{
			get { return this.nRCIndex; }
			set
			{
				if( ( this.Parent != null ) && ( this.nRCIndex != value ) )
				{
					this.nRCIndexT = value;
					if( this.nRowOffsetDirT == -1 )
						this.RowOffset = 0;

					if( cdbParent != null )
					{
						this.nRCIndex = value;
						cdbParent.ReplaceBarLocation( this );
					}

					this.bRedockNeeded = true;

					this.cbController.RecalcLayout( this );

					this.Invalidate();
				}
				else
					this.nRCIndex = value;
			}
		}

		/// <summary>
		/// Gets / sets the minimum linear dimension of the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>An integer value representing the minimum length.</value>
		[
		Category( "Layout" ),
		Description( "The minimum linear dimension for the CommandBar." ),
		Localizable( true )
		]
		public virtual int MinLength
		{
			get { return this.nMinLength; }
			set
			{
				if( ( this.Parent != null ) && ( this.nMinLength != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.nMinLength = value;
			}
		}

		/// <summary>
		/// Gets / sets the maximum(ideal) linear dimension of the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>An integer value representing the ideal length.</value>
		[
		Category( "Layout" ),
		Description( "The maximum/ideal linear dimension for the CommandBar." ),
		Localizable( true )
		]
		public virtual int MaxLength
		{
			get { return this.nMaxLength; }
			set
			{
				if( this.nMaxLength != value )
				{
					int oldlength = this.nMaxLength;
					this.nMaxLength = value;
					if( this.Parent != null )
					{
						if( this.Floating )
						{
							// If in floating mode, then recalculate the float bounds
							SizeF sztext = this.CalcTextLength( CommandBarDockState.Float, true );
							int ntextoff = 0;
							if( sztext.Width > 0 )
								ntextoff = (int)sztext.Width + 2;
							Size szfloat = new Size( ( this.nMaxLength - ( this.nHeaderOff + 2 + ntextoff + 2 + this.nTrailBtnOff ) ) + 2, this.nCommandBarHt );
							int nminwidth = CommandBar.FormMinWidth;
							szfloat.Width = ( szfloat.Width < nminwidth ) ? nminwidth : szfloat.Width;
							this.rcFloat.Size = new Size( szfloat.Width + 4, szfloat.Height + CommandBar.CaptionHeight + 4 );
						}
						else
						{
							if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
							{
								if( this.Width != this.nMaxLength )
								{
									if( ( this.Width < this.nMaxLength && ( this.nMaxLength < oldlength || this.bUpdateOffsets ) ) ||
                                        ( this.Width < oldlength && ( this.nMaxLength > oldlength || this.bUpdateOffsets ) ) )
									{
										if( this.bUpdateOffsets )
										{
											this.bRedockNeeded = true;
											this.bRecalcNeeded = true;

											this.RedockIfNeeded();
											this.RecalcIfNeeded();
										}

										return;
									}

									this.Width += this.nMaxLength - oldlength;
								}
							}
							else
							{
								if( this.Height != this.nMaxLength )
								{
									if( ( this.Height < this.nMaxLength && ( this.nMaxLength < oldlength || this.bUpdateOffsets ) ) ||
                                        ( this.Height < oldlength && ( this.nMaxLength > oldlength || this.bUpdateOffsets ) ) )
									{
										if( this.bUpdateOffsets )
										{
											this.bRedockNeeded = true;
											this.bRecalcNeeded = true;

											this.RedockIfNeeded();
											this.RecalcIfNeeded();
										}

										return;
									}

									this.Height += this.nMaxLength - oldlength;
								}
							}

							if( this.bUpdateOffsets )
							{
								this.UpdateRowsOffsets( this, this.nMaxLength - oldlength );
								this.bUpdateOffsets = false;
							}
						}

						this.bRecalcNeeded = true;
						this.RecalcIfNeeded();

						this.Invalidate();
					}
				}
			}
		}

		private void UpdateRowsOffsets( CommandBar cbar, int offset )
		{
			CommandBar[] cbarray = this.cdbParent.GetRowArray( cbar.nRCIndex );

			if( !cbar.Visible || cbarray.Length == 1 )
				return;

			bool needChangeOffset = true;

			for( int i = 0; i < cbarray.Length; i++ )
			{
				int npadding = 2;
				CommandBar cb = cbarray[i];

				MainFrameBarManager manager = MainFrameBarManager.GetManagerFromForm( cbar.Controller.HostForm ) as MainFrameBarManager;
				if( manager != null && !manager.NeedSaveCustomData )
				{
					needChangeOffset = false;
				}

				if( needChangeOffset && cb != cbar )
				{
					if( ( cb.nRowOffsetInDir > cbar.VRight + npadding ) && ( this.cdbParent.Dock == DockStyle.Top || this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						cb.nRowOffsetInDir += offset;
					}
					else if( ( cb.nRowOffsetInDir > cbar.VBounds.Bottom + npadding ) && ( this.cdbParent.Dock == DockStyle.Left || this.cdbParent.Dock == DockStyle.Right ) )
					{
						cb.nRowOffsetInDir += offset;
					}
				}
			}
		}

		/// <summary>
		/// Gets / sets the ideal lateral dimension of the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>An integer value representing the minimum height.</value>
		[
		Category( "Layout" ),
		Description( "The ideal lateral dimension for the CommandBar." ),
		Localizable( true )
		]
		public virtual int MinHeight
		{
			get { return this.nCommandBarHt; }
			set
			{
				if( ( this.Parent != null ) && ( this.nCommandBarHt != value ) )
				{
                    if (value <= 32 && value > 30)
						this.nCommandBarHt = value;
                    else if (value > 26)
                        this.nCommandBarHt = value;

					if( this.Floating )
						this.bRecalcNeeded = true;
					else
					{
						if( this.cbController != null && this.cdbParent != null )
						{
							int height = this.nCommandBarHt;

							ArrayList cbarray = new ArrayList();

							foreach( CommandBar cb in this.cbController.CommandBars )
							{
								if( cb is CommandBarExt )
								{
									CommandBarExt cbar = cb as CommandBarExt;

									if( cbar.cdbParent != null && cbar.cdbParent == this.cdbParent &&
                                        cbar.cbarDockState != CommandBarDockState.Float &&
                                        cbar.nRCIndex == this.nRCIndex && cbar.BarControl != null )
									{
										SizeF maxsize = new SizeF( Int32.MaxValue, Int32.MaxValue );
										cbar.BarControl.GetPreferredSize( ref maxsize );

										if( height < (int)maxsize.Height + 2 )
										{
											height = (int)maxsize.Height + 2;
										}

										if( cbar != this &&
                                            ( ( ( this.cbarDockState == CommandBarDockState.Top || this.cbarDockState == CommandBarDockState.Bottom ) && cbar.Height != height ) ||
                                            ( ( this.cbarDockState == CommandBarDockState.Left || this.cbarDockState == CommandBarDockState.Right ) && cbar.Width != height ) ) )
											cbarray.Add( cbar );
									}
								}
							}

							int prevValue = 0;
							if( this.cbarDockState == CommandBarDockState.Top || this.cbarDockState == CommandBarDockState.Bottom )
							{
								prevValue = this.Height;
								this.Height = height;

								foreach( CommandBarExt cb in cbarray )
								{
									cb.Height = height;
								}
							}
							else if( this.cbarDockState == CommandBarDockState.Left || this.cbarDockState == CommandBarDockState.Right )
							{
								prevValue = this.Width;
								this.Width = height;

								foreach( CommandBarExt cb in cbarray )
								{
									cb.Width = height;
								}
							}

							if( CommandBar.bDragging )
							{
								float ptxoffset = (float)CommandBar.ptDeltaOff.X / (float)prevValue;
								CommandBar.ptDeltaOff = new Point( (int)( ptxoffset * (float)height ), CommandBar.ptDeltaOff.Y );
							}
						}
					}

					this.Invalidate();
				}
				else
				{
                    if(value<=32 &&value>30)
					this.nCommandBarHt = 32;
				}
			}
		}

		/// <summary>
		/// Gets / sets the incremental step by which the <see cref="CommandBar"/>'s lateral dimension increases when wrapped.
		/// </summary>
		/// <value>An integer value representing the incremental height.</value>
		[
		Category( "Layout" ),
		Description( "Incremental step by which the CommandBar's lateral dimension increases when wrapped." ),
		DefaultValue( 1 ),
		Localizable( true )
		]
		public virtual int IntegralHeight
		{
			get { return this.nIntegral; }
			set
			{
				if( ( this.Parent != null ) && ( this.nIntegral != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.nIntegral = value;
			}
		}

		internal bool CommandBarBaseVisible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[
		Category( "Behavior" ),
		DefaultValue( true ),
		Syncfusion.Documentation.DocumentationExclude(),
		Localizable( true )
		]
		public new virtual bool Visible
		{
			get
			{
				return this.bVisible;
			}
			set
			{
				if( this.bVisible != value )
				{
					this.bVisibilitySetFlag = true;
					if( this.cbController != null )
					{
						if( value )
						{
							if( ( this.Floating && m_rendererFloating == null ) || m_renderer == null )
								this.SetRenderer();

							if( ( this.Parent != null ) && ( this.Parent is CommandBarForm ) )
							{
								this.bVisible = true;
								UtilFuncs.SetVisibleNoActivate( this.Parent, true );
								base.Visible = true;
							}
							else if( this.cdbParent != null )
							{
								this.cdbParent.Controls.Remove( this );
								this.bVisible = true;
								base.Visible = true;

								if( this.nRCIndex == -1 )
								{
									// If the CommandBar is in an as yet uninitialized state, then get the most suitable
									// index/offset information from the parent dockbar
									this.cdbParent.InitializeIndexRowOffsets( this );
								}

								this.cdbParent.bSuspendIndexChanges = true;
								this.DockToParent();
								this.cdbParent.bSuspendIndexChanges = false;

								this.cbController.RecalcLayout( this );
								this.nRowOffsetInDir = this.nRowOffsetDir;
							}
							else
								this.bVisible = true;
						}
						else
						{
							if( ( this.Parent != null ) && ( this.Parent is CommandBarForm ) )
							{
								this.bVisible = false;
								base.Visible = false;
								UtilFuncs.SetVisibleNoActivate( this.Parent, false );
							}
							else if( this.cdbParent != null )
							{
								this.nRCIndexDrag = ( this.cdbParent.GetCommandBarIndex( this ) != -1 ) ? this.cdbParent.GetCommandBarIndex( this ) : this.nRCIndex;
								this.nRCCountDrag = this.cdbParent.RowsCount;

								if( this.nRCIndexDrag != -1 )
									this.cdbParent.bSuspendIndexChanges = true;
								this.cdbParent.RemoveCommandBar( this );
								this.cdbParent.bSuspendIndexChanges = false;

								this.bVisible = false;
								base.Visible = false;
								this.cdbParent.Controls.Add( this );
								this.cbController.RecalcLayout( this );
							}
							else
								this.bVisible = false;
						}
					}
					else
					{
						this.bVisible = value;
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the text caption should be displayed on a docked <see cref="CommandBar"/>.
		/// </summary>
		/// <value>TRUE if the text is to be displayed. The default is TRUE.</value>
		[
		Category( "Behavior" ),
		Description( "Draws the CommandBar with/without the text in the docked position." ),
		DefaultValue( true )
		]
		public virtual bool ShowDockModeText
		{
			get { return this.bShowDockModeText; }
			set
			{
				if( ( this.Parent != null ) && ( this.bShowDockModeText != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.bShowDockModeText = value;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/>'s positioning gripper should be hidden.
		/// </summary>
		/// <value>TRUE if the gripper is to be hidden. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Draws the CommandBar with/without the drag gripper." ),
		DefaultValue( false )
		]
		public bool HideGripper
		{
			get { return this.bHideGripper; }
			set
			{
				if( this.bHideGripper != value )
				{
					this.bHideGripper = value;

					this.bRecalcNeeded = true;

					if( this.Parent != null )
					{
						RecalcIfNeeded();
						SetChildControlBounds();

						Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> should be drawn without a chevron.
		/// </summary>
		/// <remarks>
		/// When a docked CommandBar is sized less than the <see cref="CommandBar.MaxLength"/> value, a
		/// chevron will normally be displayed on it's trailing edge.
		/// </remarks>
		/// <value>TRUE if the chevron is to be hidden. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Draws the CommandBar with/without the chevron." ),
		DefaultValue( false )
		]
		public virtual bool HideChevron
		{
			get { return this.bHideChevron; }
			set
			{
				if( this.bHideChevron != value )
				{
					this.bHideChevron = value;

					this.bRecalcNeeded = true;

					if( this.Parent != null )
					{
						RecalcIfNeeded();
						SetChildControlBounds();

						Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets color for chevron.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "Color for chevron." ),
		DefaultValue( typeof( SystemColors ), "ControlText" )
		]
		public Color ChevronColor
		{
			get
			{
				return m_chevronColor;
			}
			set
			{
				if( value != m_chevronColor )
				{
					m_chevronColor = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/>'s dropdown button should be hidden.
		/// </summary>
		/// <value>TRUE if the dropdown button is to be hidden. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Draws the CommandBar with/without the dropdown button." ),
		DefaultValue( false )
		]
		public bool HideDropDownButton
		{
			get { return this.bHideDropDown; }
			set
			{
				if( this.bHideDropDown != value )
				{
					this.bHideDropDown = value;
					this.bRecalcNeeded = true;

					if( this.Parent != null )
					{
						RecalcIfNeeded();
						SetChildControlBounds();

						Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> will have a close button when floating.
		/// </summary>
		/// <value>TRUE if the close button is to be hidden. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Determines whether the CommandBar will have a close button when floating." ),
		DefaultValue( false )
		]
		public bool HideCloseButton
		{
			get { return this.bHideCloseButton; }
			set
			{
				if( ( this.Parent != null ) && ( this.bHideCloseButton != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.bHideCloseButton = value;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> should wrap when docked.
		/// </summary>
		/// <remarks>
		/// Setting this property to TRUE will force a docked CommandBar to wrap when it's
		/// bounds are less than the <see cref="CommandBar.MaxLength"/> value.
		/// </remarks>
		/// <value>TRUE indicates that the CommandBar will wrap. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Wraps the CommandBar when the bounds are less than the maximum length." ),
		DefaultValue( false )
		]
		public virtual bool DockModeWrapping
		{
			get { return this.bDockModeWrapping; }
			set
			{
				if( ( this.Parent != null ) && ( this.bDockModeWrapping != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.bDockModeWrapping = value;
				if( value == true )
					this.bHideChevron = true;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> should wrap when floating.
		/// </summary>
		/// <remarks>
		/// Setting this property to TRUE will force a floating <see cref="CommandBar"/> to wrap
		/// when it's bounds are less than the <see cref="CommandBar.MaxLength"/> value.
		/// </remarks>
		/// <value>TRUE indicates that the CommandBar will wrap. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Wraps a floating CommandBar when it is resized to less than it's maximum length." ),
		DefaultValue( false )
		]
		public virtual bool FloatModeWrapping
		{
			get { return this.bFloatModeWrapping; }
			set { this.bFloatModeWrapping = value; }
		}


		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> should occupy the entire row when docked.
		/// </summary>
		///<value>TRUE indicates that the CommandBar will occupy the entire row. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "The CommandBar takes up the full row." ),
		DefaultValue( false )
		]
		public virtual bool OccupyFullRow
		{
			get { return this.bFullRow; }
			set
			{
				if( this.bFullRow != value )
				{
					this.bFullRow = value;

					if( this.Parent != null )
					{
						if( value && ( this.cbarDockState != CommandBarDockState.Float ) )
						{
							CommandBar[] cbarray = this.cdbParent.GetRowArray( this.nRCIndex );
							foreach( CommandBar cb in cbarray )
							{
								cb.nRowOffsetDir = 0;
								cb.nRowOffsetInDir = 0;
							}
						}

						this.bRedockNeeded = true;
						this.RedockIfNeeded();

						this.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Returns the current dock/float state of the <see cref="CommandBar"/>.
		/// </summary>
		/// <value>TRUE if the CommandBar is floating.</value>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool Floating
		{
			get { return ( ( this.Parent != null ) && ( this.Parent is CommandBarForm ) ); }
		}

		/// <summary>
		/// Gets / sets the bounds of a floating <see cref="CommandBar"/>.
		/// </summary>
		/// <value>A Rectangle value that represents the float bounds of the CommandBar. </value>
		[
		Category( "Layout" ),
		Description( "The float bounds for the CommandBar." ),
		Browsable( false ),
		Localizable( true )
		]
		public Rectangle FloatBounds
		{
			get { return this.rcFloat; }
			set
			{
				if( ( this.Parent != null ) && ( this.rcFloat != value ) )
				{
					this.bRecalcNeeded = true;
					this.Invalidate();
				}
				this.rcFloat = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeFloatBounds()
		{
			return this.Floating;
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> is allowed to float.
		/// </summary>
		/// <value>TRUE if floating is to be disallowed. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Makes the CommandBar non-floatable." ),
		DefaultValue( false )
		]
		public bool DisableFloating
		{
			get { return this.bDisableFloating; }
			set
			{
				if( this.bDisableFloating != value )
				{
					if( this.bDisableDocking == true )
						return;
					this.bDisableFloating = value;

					if( ( value == true ) && ( this.cbarDockState == CommandBarDockState.Float ) )
						this.DockState = CommandBarDockState.Top;
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> is allowed to dock.
		/// </summary>
		/// <value>TRUE if docking is to be disallowed. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Makes the CommandBar non-dockable." ),
		DefaultValue( false )
		]
		public bool DisableDocking
		{
			get { return this.bDisableDocking; }
			set
			{
				if( this.bDisableDocking != value )
				{
					CommandDockBar dBar = this.cbController.GetDockBar( this.cbarDockState );
					if( dBar != null )
						dBar.bSuspendIndexChanges = true;

					if( this.bDisableFloating == true )
						return;
					this.bDisableDocking = value;

					if( value )
						this.DockState = CommandBarDockState.Float;

					if( dBar != null )
						dBar.bSuspendIndexChanges = false;
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> is always docked to the leading edge.
		/// </summary>
		/// <value>TRUE to enforce leading edge docking. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Permanently docks the CommandBar to the leading edge of the dockborder." ),
		DefaultValue( false ),
		]
		public bool AlwaysLeadingEdge
		{
			get { return this.bLeadingEdge; }
			set
			{
				if( this.bLeadingEdge != value )
				{
					this.bLeadingEdge = value;
					if( value )
					{
						this.bTrailingEdge = false;
						this.DisableFloating = true;
						if( this.Parent != null )
						{
							this.bRedockNeeded = true;
							this.Invalidate();
						}
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="CommandBar"/> is always docked to the trailing edge.
		/// </summary>
		/// <value>TRUE to enforce trailing edge docking. The default is FALSE.</value>
		[
		Category( "Behavior" ),
		Description( "Permanently docks the CommandBar to the trailing edge of the dockborder." ),
		DefaultValue( false ),
		]
		public bool AlwaysTrailingEdge
		{
			get { return this.bTrailingEdge; }
			set
			{
				this.bTrailingEdge = value;
				if( value )
				{
					this.bLeadingEdge = false;
					this.DisableFloating = true;
					if( this.Parent != null )
					{
						this.bRedockNeeded = true;
						this.Invalidate();
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				if( base.BackColor != value )
				{
					base.BackColor = value;
					this.bBackColorSet = true;
				}
			}
		}

		/// <summary>
		/// Gets / sets the font used to display text in the control.
		/// </summary>
		/// <value>A Font object.</value>
		[
		Category( "Appearance" ),
		Description( "The font used to display text in the control." ),
		Localizable( true )
		]
		public new Font Font
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

		[
		Category( "Appearance" ),
		Description( "The mouse cursor used for the CommandBar." ),
		Localizable( true )
		]
		[Syncfusion.Documentation.DocumentationExclude()]
		public override Cursor Cursor
		{
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		// Base properties that need to be hidden in the designer
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool AllowDrop
		{
			get { return base.AllowDrop; }
			set { base.AllowDrop = value; }
		}

		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude(),
		Localizable( true )
		]
		public new bool Enabled
		{
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}

		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public new Point Location
		{
			get { return base.Location; }
			set { base.Location = value; }
		}

		[
			//Browsable(false),
			//EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public new Size Size
		{
			get { return base.Size; }
			set { base.Size = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public static bool IsDragging
		{
			get { return CommandBar.bDragging; }
		}

		/// <summary>
		/// Indicates whether the chevron is currently displayed.
		/// </summary>
		/// <value>TRUE if the chevron is visible; FALSE otherwise</value>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsChevronVisible
		{
			get
			{
				if( ( this.bHideChevron == false ) && ( this.bDockModeWrapping == false ) &&
					( this.cbarDockState != CommandBarDockState.Float ) && ( this.cdbParent != null ) )
				{
					int ndim = 0;
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
						ndim = this.Width;
					else
						ndim = this.Height;
					return ( ndim < this.nMaxLength );
				}
				return false;
			}
		}

		/// <summary>
		/// Gets / sets the Virtual Left property - used for layout.
		/// </summary>
		protected internal int VLeft
		{
			get
			{
				int nVLeft = 0;

				if( this.IsRTL && null != this.cdbParent
					&& ( DockStyle.Top == this.cdbParent.Dock || DockStyle.Bottom == this.cdbParent.Dock ) )
				{
					Rectangle rcClient = this.cdbParent.ClientRectangle;

					nVLeft = rcClient.Width - this.Right;
				}
				else
				{
					nVLeft = this.Left;
				}

				return nVLeft;
			}
			set
			{
				int nVLeft = value;

				if( this.IsRTL && null != this.cdbParent
					&& ( DockStyle.Top == this.cdbParent.Dock || DockStyle.Bottom == this.cdbParent.Dock ) )
				{
					Rectangle rcClient = this.cdbParent.ClientRectangle;

					nVLeft = rcClient.Width - value - this.Width;
				}

				this.Left = nVLeft;
			}
		}

		/// <summary>
		/// Gets / sets the Virtual Right property - used for layout.
		/// </summary>
		protected internal int VRight
		{
			get
			{
				int nVRight = 0;

				if( this.IsRTL && null != this.cdbParent
					&& ( DockStyle.Top == this.cdbParent.Dock || DockStyle.Bottom == this.cdbParent.Dock ) )
				{
					Rectangle rcClient = this.cdbParent.ClientRectangle;

					nVRight = rcClient.Width - this.Left;
				}
				else
				{
					nVRight = this.Right;
				}

				return nVRight;
			}
		}

		/// <summary>
		/// Gets / sets the Virtual Location property - used for layout.
		/// </summary>
		protected internal Point VLocation
		{
			get
			{
				return new Point( this.VLeft, this.Top );
			}
			set
			{
				this.VLeft = value.X;
				this.Top = value.Y;
			}
		}

		/// <summary>
		/// Returns the Virtual Bounds property - used for layout.
		/// </summary>
		protected internal Rectangle VBounds
		{
			get
			{
				Rectangle rcBounds;

				if( this.IsRTL && null != this.cdbParent
					&& ( DockStyle.Top == this.cdbParent.Dock || DockStyle.Bottom == this.cdbParent.Dock ) )
				{
					rcBounds = new Rectangle( this.VLocation, this.Size );
				}
				else
				{
					rcBounds = this.Bounds;
				}

				return rcBounds;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal bool IsRTL
		{
			get
			{
				return ( this as IPopupParent ).IsRightToLeft;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool IsCustomizing
		{
			get
			{
				return m_bIsCustomizing;
			}
		}
		/// <summary>
		/// Gets / sets the PopupContainer control to be displayed when the dropdown button is clicked.
		/// </summary>
		/// <value>An instance of the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> class.</value>
		[
		Description( "The PopupControlContainer displayed when the dropdown button is clicked." ),
		Category( "DropDown" ),
		DefaultValue( null )
		]
		public PopupControlContainer PopupContainer
		{
			get { return this.popupContainer; }
			set
			{
				if( this.popupContainer != value )
				{
					if( this.IsShowingDropDown() )
						this.HideDropDown();
					this.popupContainer = value;
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected PopupMenu pupMenu;
		/// <summary>
		/// Gets / sets the PopupMenu to be displayed when the dropdown button is clicked.
		/// </summary>
		/// <value>An instance of the <see cref="PopupMenu"/> class.</value>
		[
		Description( "The PopupMenu displayed when the dropdown button is clicked." ),
		Category( "DropDown" ),
		DefaultValue( null )
		]
		public PopupMenu PopupMenu
		{
			get { return this.pupMenu; }
			set
			{
				this.pupMenu = value;
				if( this.pupMenu != null && this.pupMenu.ParentBarItem != null
					&& this.Controller != null )
					this.pupMenu.ParentBarItem.Style = this.Controller.Style;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		[Browsable( false )]
		internal CBButtons CmdBarHilight
		{
			get
			{
				return cbHilight;
			}
		}

		#endregion

		#region Class Utility Methods
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool ShouldDrawThemed()
		{
			return ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.cbController != null && this.cbController.ThemesEnabled );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Rectangle GetDropDownRect()
		{
			Rectangle rcdd = Rectangle.Empty;

			bool bRTL = this.IsRTL;

			if( this.cbarDockState == CommandBarDockState.Float )
			{
				int ncaptionht = CommandBar.CaptionHeight;
				int nCaptionLeft = 0;
				Rectangle rccaption = this.CaptionRect;

				if( this.bHideCloseButton )
				{
					nCaptionLeft = bRTL ? 1 : ( rccaption.Right-ncaptionht-1 );
					rcdd = new Rectangle( nCaptionLeft, rccaption.Top,
						ncaptionht, ncaptionht );
				}
				else
				{
					nCaptionLeft = bRTL ? ( ncaptionht+1 ) : ( rccaption.Right-( ncaptionht*2 )-1 );
					rcdd = new Rectangle( nCaptionLeft, rccaption.Top,
						ncaptionht, ncaptionht );
				}

				rcdd = new Rectangle( rcdd.Left, rcdd.Top, rcdd.Width-1, rcdd.Height-1 );
			}
			else
			{
				Rectangle rcclient = this.ClientRectangle;

				if( this.ShouldDrawThemed()
					&& this.cbController.Style != VisualStyle.Office2007
                    && this.cbController.Style != VisualStyle.Office2010
					&& this.cbController.Style != VisualStyle.Office2007Outlook )
				{
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						const int nDDWidth = 14;
						const int nDDLeftPadding = 2;
						const int nDDTopPadding = 1;
						const int nDDBottomMargin = 2;

						rcdd = new Rectangle( ( bRTL ? nDDLeftPadding : rcclient.Right-nDDWidth-nDDLeftPadding ),
							rcclient.Top+nDDTopPadding, nDDWidth, rcclient.Height-nDDBottomMargin );
					}
					else
					{
						const int nDDHeight = 14;
						const int nDDLeftPadding = 1;
						const int nDDTopPadding = 2;
						const int nDDRightMargin = 2;

						rcdd = new Rectangle( rcclient.Left+nDDLeftPadding,
							( bRTL ? nDDTopPadding : rcclient.Bottom-nDDHeight-nDDTopPadding ),
							rcclient.Width-nDDRightMargin, nDDHeight );
					}

					rcdd.Inflate( 0, -1 );
				}
				else if( ( this.cbController != null ) && ( this.cbController.Style == VisualStyle.Office2003
					|| this.cbController.Style == VisualStyle.VS2005 
					|| this.cbController.Style == VisualStyle.Office2007 
                    || this.cbController.Style == VisualStyle.Office2010
					|| this.cbController.Style == VisualStyle.Office2007Outlook ) )
				{
					int ntrailbtnoff = this.nTrailBtnOff;
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						rcdd = new Rectangle( ( bRTL ? 0 : rcclient.Right-ntrailbtnoff ),
							rcclient.Top, ntrailbtnoff, rcclient.Height );
					}
					else
					{
						rcdd = new Rectangle( rcclient.Left,
							( bRTL ? 0 : rcclient.Bottom-ntrailbtnoff ), rcclient.Width, ntrailbtnoff );
					}
				}
				else
				{
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					{
						const int nDDWidth = 10;
						const int nDDLeftPadding = 2;
						const int nDDTopPadding = 1;
						const int nDDBottomMargin = 2;

						rcdd = new Rectangle( ( bRTL ? 0 : rcclient.Right-nDDWidth-nDDLeftPadding ),
							rcclient.Top+nDDTopPadding, nDDWidth, rcclient.Height-nDDBottomMargin );
					}
					else
					{
						const int nDDHeight = 12;
						const int nDDLeftPadding = 1;
						const int nDDTopPadding = 2;
						const int nDDRightMargin = 2;

						rcdd = new Rectangle( rcclient.Left+nDDLeftPadding,
							( bRTL ? 0 : rcclient.Bottom-nDDHeight-nDDTopPadding ),
							rcclient.Width-nDDRightMargin, nDDHeight );
					}
					rcdd.Inflate( 0, -1 );
				}
			}

			return rcdd;
		}

		/// <summary>
		/// Gets visual state for DropDown button.
		/// </summary>
		public CBButtonState GetDropDownState()
		{
			CBButtonState state = CBButtonState.Normal;

			if( ( this.IsShowingDropDown() == true ) || 
				( this.cbHilight == ( CBButtons.DropDown | CBButtons.Pressed ) ) )
			{
				state = CBButtonState.Pressed;
			}
			else if( this.cbHilight == CBButtons.DropDown )
			{
				state = CBButtonState.Hot;
			}

			return state;
		}
		/// <summary>
		/// Gets visual state for close button of the floating CommandBar.
		/// </summary>
		public CBButtonState GetCloseButtonState()
		{
			CBButtonState state = CBButtonState.Normal;

			if( this.cbHilight == ( CBButtons.Close | CBButtons.Pressed ) )
			{
				state = CBButtonState.Pressed;
			}
			else if( this.cbHilight == CBButtons.Close )
			{
				state = CBButtonState.Hot;
			}

			return state;
		}
		/// <summary>
		/// Gets rectangle for display text of the docked CommandBar.
		/// </summary>
		protected internal virtual Rectangle GetTextRectangle()
		{
			Rectangle textRect = Rectangle.Empty;
			int iWidth = 0;
			bool bRTL = this.IsRTL;

			if( ( this.bHideChevron == false ) || ( this.bHideDropDown == false ) )
			{
				iWidth = this.nTrailBtnOff;
			}

			if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
			{
				textRect = new Rectangle( this.nHeaderOff + 2, 0,
					this.Width - ( this.nHeaderOff + 2 ) - iWidth, this.Height );
			}
			else
			{
				textRect = new Rectangle( 0, bRTL ? 0 : this.nHeaderOff + 2,
					this.Width, this.Height - ( this.nHeaderOff + 2 ) - iWidth );
			}

			return textRect;
		}
		/// <summary>
		/// Gets rectangle for display text of the floating CommandBar.
		/// </summary>
		protected internal Rectangle GetFloatingTextRectangle()
		{
			bool bRTL = this.IsRTL;
			int nCaptionHt = CommandBar.CaptionHeight;
			Rectangle captionRect = this.CaptionRect;
			Rectangle textRect = bRTL ? 
				new Rectangle( nCaptionHt * 2 + 2, captionRect.Top,
				captionRect.Width - 2 - ( nCaptionHt * 2 ), captionRect.Height ):
				new Rectangle( captionRect.Left + 1, captionRect.Top,
				captionRect.Width - 2 - ( nCaptionHt * 2 ), captionRect.Height );

			return textRect;
		}
		/// <summary>
		/// Gets font for caption of the floating CommandBar.
		/// </summary>
		protected internal Font GetFloatinCaptionFont()
		{
			return ftFloatCaption;
		}
		/// <summary>
		/// Indicates whether the chevron must be displayed.
		/// </summary>
		protected internal bool IsShowChevron()
		{
			bool bShow = false;
			int iSize = 0;

			if( this.cdbParent != null )
			{
				if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
				{
					iSize = this.Width;
				}
				else
				{
					iSize = this.Height;
				}
			}

			if( !this.bDockModeWrapping && !this.bHideChevron && ( iSize < this.nMaxLength ) )
			{
				bShow = true;
			}

			return bShow;
		}
		/// <summary>
		/// Gets value indicating whether quick customizing is allowed and dropdown arrow should be drawn.
		/// </summary>
		protected internal virtual bool AllowQuickCustomizing
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Indicates whether the text must be displayed.
		/// </summary>
		protected internal bool IsShowText()
		{
			bool bShow = false;

			if( ( this.Text != String.Empty ) && ( this.bShowDockModeText == true ) )
			{
				bShow = true;
			}

			return bShow;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeRowOffset()
		{
			return !this.Floating;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeRowIndex()
		{
			return !this.Floating;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public override void ResetBackColor()
		{
			base.ResetBackColor();
			this.bBackColorSet = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeBackColor()
		{
			return this.bBackColorSet;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeFont()
		{
			return ( this.Font != SystemInformation.MenuFont );
		}

		/// <summary>
		/// Resets the <see cref="CommandBar.Font"/> property to it's default value.
		/// </summary>
		public override void ResetFont()
		{
			this.Font = SystemInformation.MenuFont;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeCursor()
		{
			return ( ( this.Cursor != Cursors.Default ) && ( this.Cursor != Cursors.SizeAll ) );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override void ResetCursor()
		{
			this.Cursor = Cursors.Default;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle CalcChildControlBounds( Size szbar )
		{
			return this.CalcChildControlBounds( szbar, this.DockState );
		}
		/// <summary>
		/// Calculates the CommandBar's maximum length for the specified client width.
		/// </summary>
		/// <summary>
		/// Estimates the <see cref="CommandBar.MaxLength"/> value for the <see cref="CommandBar"/> to
		/// host a client control of the specified width.
		/// </summary>
		/// <param name="nctrlwidth">The client control width.</param>
		/// <returns>The maximum length estimate.</returns>
		public virtual int CalcCommandBarMaxLength( int nctrlwidth )
		{
			SizeF sztext = SizeF.Empty;
			if( this.ShowDockModeText == true )
			{
				Graphics gph = this.CreateGraphics();
				sztext = gph.MeasureString( this.Text, this.Font );
				gph.Dispose();
			}
			int ntextoff = 0;
			if( sztext.Width > 0 )
				ntextoff = (int)sztext.Width+2;
			return this.nHeaderOff + 2 + ntextoff + nctrlwidth + 2 + this.nTrailBtnOff;
		}

		/// <summary>
		/// Calculates the client control bounds for the specified <see cref="CommandBar"/> size and dock position.
		/// </summary>
		/// <param name="szbar">The CommandBar size .</param>
		/// <param name="cdb">A <see cref="CommandBarDockState"/> value.</param>
		/// <returns>The client control bounds.</returns>
		public Rectangle CalcChildControlBounds( Size szbar, CommandBarDockState cdb )
		{
			Rectangle rcbounds = Rectangle.Empty;
			if( cdb == CommandBarDockState.Float )
			{
				Rectangle rccaptn = new Rectangle( 1, 1, szbar.Width-2, CommandBar.CaptionHeight );
				rcbounds = new Rectangle( rccaptn.Left, rccaptn.Bottom+1, rccaptn.Width, szbar.Height-rccaptn.Height-2 );
			}
			else
			{
				bool bRTL = this.IsRTL;
				SizeF sztext = this.CalcTextLength( cdb, false );
				int ntextoff = 0;
				if( sztext.Width > 0 )
					ntextoff = (int)sztext.Width+2;

				if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) )
				{
					Point ptloc = new Point( this.nHeaderOff+2+ntextoff, 1 );
					rcbounds = new Rectangle( ptloc.X, bRTL ? 0 : ptloc.Y, szbar.Width-ptloc.X-2-this.nTrailBtnOff, szbar.Height-2 );
				}
				else
				{
					Point ptloc = new Point( 1, this.nHeaderOff+2+ntextoff );
					rcbounds = new Rectangle( ptloc.X, bRTL ? 0 : ptloc.Y, szbar.Width-2, szbar.Height-ptloc.Y-2-this.nTrailBtnOff );
				}
			}
			this.AdjustChildControlBounds( ref rcbounds );
			return rcbounds;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void ResetTempDefaults()
		{
			this.cbDockStateT = CommandBarDockState.None;
			this.nRowOffsetDirT = -1;
			this.nRowOffsetInDirT = -1;
			this.nRCIndexT = -1;
		}

		private Form GetParentForm()
		{
			Control parent = this.Parent;
			while( parent != null )
			{
				if( ( parent as Form ) != null )
				{
					return ( parent as Form );
				}

				parent = parent.Parent;
			}

			return null;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void RedockIfNeeded()
		{
			if( this.bRedockNeeded )
			{
				Form parentForm = GetParentForm();

				if( parentForm != null )
				{
					parentForm.SuspendLayout();
				}

				this.bRedockNeeded = false;
				Debug.Assert( this.Parent != null );

				if( ( ( this.cbarDockState == CommandBarDockState.Float ) && ( ( this.cbDockStateT == CommandBarDockState.Float ) || ( this.cbDockStateT == CommandBarDockState.None ) ) )
				|| ( ( this.cbarDockState != CommandBarDockState.Float ) && ( this.cbDockStateT == CommandBarDockState.Float ) ) )
				{
					// The CommandBar will either getinto or remain in the floating state at the
					// end of the redocking.
					Debug.Assert( this.bDisableFloating == false );
					if( this.cbarDockState != CommandBarDockState.Float )
					{
						this.FireCommandBarStateChanging( new CommandBarStateChangingEventArgs( CommandBarDockState.Float ) );
						if( this.rcFloat.IsEmpty == true )
							this.EnterFloatMode( this.Location );
						else
							this.EnterFloatMode( this.rcFloat.Location );
						this.FireCommandBarStateChanged( EventArgs.Empty );
						this.cdbParent.LayoutDockBar();
					}
					else
						this.cbController.RecalcLayout( this );
				}
				else
				{
					// Execution gets here if only the CommandBar will remain/getinto a docked state
					// at the end of the redocking.
					Debug.Assert( this.bDisableDocking != true );
					this.FireCommandBarStateChanging( new CommandBarStateChangingEventArgs( this.cbDockStateT ) );
					if( this.cbarDockState != CommandBarDockState.Float )
					{
						this.cdbParent.SetCurrentRowMarker( this );
						this.cdbParent.RemoveCommandBar( this );
						bool buserowmarkers = true;
						if( this.nRCIndexT != -1 )
						{
							this.nRCIndex = this.nRCIndexT;
							buserowmarkers = false; // Do not use RowMarker as a new RCIndex value has been specified for this CommandBar.
						}
						if( this.cbDockStateT != CommandBarDockState.None )
						{
							this.cbarDockState = this.cbDockStateT;
							this.cdbParent = this.cbController.GetDockBar( this.cbarDockState );
							if( buserowmarkers == true )
								buserowmarkers = false;
						}
						if( this.nRowOffsetDirT != -1 )
						{
							this.nRowOffsetDir = this.nRowOffsetDirT;
							this.nRowOffsetInDir = this.nRowOffsetDir;
							if( buserowmarkers == true )
								buserowmarkers = false;
						}
						ArrayList cblist = new ArrayList();
						foreach( CommandBar cb in this.cdbParent.Controls )
						{
							if( cb.Visible )
								cblist.Add( cb );
						}
						foreach( CommandBar cbar in cblist )
						{
							// If there are any commandbars within the parentdockbar with rowindexes
							// less than the current commandbar, then redock them first.
							if( ( ( cbar.cbDockStateT == CommandBarDockState.None )||( cbar.cbDockStateT == this.cbarDockState ) )
							&& ( cbar.nRCIndexT >= 0 ) && ( cbar.nRCIndexT < this.nRCIndex ) )
								cbar.RedockIfNeeded();
						}

						this.cdbParent.AddCommandBar( this, false );
						this.nRowOffsetInDir = this.nRowOffsetDir;
					}
					else
					{
						CommandBarForm frmfloating = this.Parent as CommandBarForm;
						this.rcFloat = frmfloating.Bounds;
						if( this.nRCIndexT != -1 )
							this.nRCIndex = this.nRCIndexT;
						if( this.cbDockStateT != CommandBarDockState.None )
						{
							this.cbarDockState = this.cbDockStateT;
							this.cdbParent = this.cbController.GetDockBar( this.cbarDockState );
						}
						if( this.nRowOffsetDirT != -1 )
						{
							this.nRowOffsetDir = this.nRowOffsetDirT;
							this.nRowOffsetInDir = this.nRowOffsetDir;
						}

						this.cdbParent.AddCommandBar( this, false );
						frmfloating.Visible = false;
						frmfloating.Close();
					}
					this.FireCommandBarStateChanged( EventArgs.Empty );
				}

				this.ResetTempDefaults();

				if( parentForm != null )
				{
					parentForm.ResumeLayout( true );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void RecalcIfNeeded()
		{
			try
			{
				if( this.bRecalcNeeded )
				{
					Debug.Assert( this.Parent != null );
					this.bRecalcNeeded = false;
					if( this.nRowOffsetDirT != -1 )
					{
						this.nRowOffsetDir = this.nRowOffsetDirT;
						this.nRowOffsetInDir = this.nRowOffsetDir;
						this.nRowOffsetDirT = -1;
					}
					if( this.cbController != null )
					{
						this.cbController.RecalcLayout( this );
						if( this.bFireDockStateChanged )
						{
							this.bFireDockStateChanged = false;
							this.OnCommandBarStateChanged( EventArgs.Empty );
						}
					}
				}
			}
			catch( Exception ex )
			{
				Debug.WriteLine( ex.ToString() );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldCreateNewRow()
		{
			Debug.Assert( this.cdbParent != null );

			if( this.nRCIndex == -1 )	// Uninitialized CommandBar.
			{
				this.nRCIndex = this.cdbParent.nRCCount;
				return true;
			}

			// The CommandBar's RowMarker will specify the insertion position within the CommandDockBar
			bool bcreatenewrow = true;
			if( ( this.rMarker != null ) && ( this.rMarker.DockBar == this.cdbParent ) )
			{
				int insertindex = 0;
				int markerindex = this.cdbParent.RowMarkers.IndexOf( this.rMarker );
				for( int nrow=0; nrow<this.cdbParent.nRCCount; nrow++ )
				{
					CommandBar[] cbarray = this.cdbParent.GetRowArray( nrow );
					if( cbarray.GetLength( 0 ) > 0 )
					{
						CommandBar cb = cbarray[0];
						int cbmarkerindex = this.cdbParent.RowMarkers.IndexOf( cb.rMarker );
						if( cbmarkerindex < markerindex )
						{
							insertindex = cb.nRCIndex+1;
						}
						else if( cbmarkerindex == markerindex )
						{
							Debug.Assert( cb.rMarker == this.rMarker );
							insertindex = cb.nRCIndex;
							bcreatenewrow = false;
							break;
						}
					}
				}
				this.nRCIndex = insertindex;
			}
			return bcreatenewrow;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void AdjustRowOffsetsInParentDockBar()
		{
			if( this.cdbParent != null )
			{
				CommandBarDockState border = this.cdbParent.GetDockBorder();
				if( ( border == CommandBarDockState.Top ) || ( border == CommandBarDockState.Bottom ) )
				{
					foreach( CommandBar bar in this.cdbParent.Controls )
					{
						if( bar.Visible )
						{
							bar.nRowOffsetInDir = bar.VLeft;
							bar.nRowOffsetDir = bar.nRowOffsetInDir;
						}
					}
				}
				else
				{
					foreach( CommandBar bar in this.cdbParent.Controls )
					{
						if( bar.Visible )
						{
							bar.nRowOffsetInDir = bar.Location.Y;
							bar.nRowOffsetDir = bar.nRowOffsetInDir;
						}
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Size GetDockWrapSize( Size szbounds )
		{
			if( this.CommandBarWrapping == null )
			{
				if( ( this.cdbParent.Dock == DockStyle.Left ) || ( this.cdbParent.Dock == DockStyle.Right ) )
				{
					szbounds.Height = ( szbounds.Height > this.nMaxLength ) ? this.nMaxLength : szbounds.Height;
					szbounds.Height = ( szbounds.Height < this.nMinLength ) ? this.nMinLength : szbounds.Height;
					float fsizefactor = (float)this.nMaxLength/(float)szbounds.Height;
					if( fsizefactor <= 1.0f )
					{
						if( this.bFullRow == false )
							return new Size( this.nCommandBarHt, this.nMaxLength );
						else
							return new Size( this.nCommandBarHt, this.Parent.Height );
					}
					else
						return new Size( this.nCommandBarHt+( this.nIntegral*(int)Math.Floor( fsizefactor ) ), szbounds.Height );
				}
				else
				{
					szbounds.Width = ( szbounds.Width > this.nMaxLength ) ? this.nMaxLength : szbounds.Width;
					szbounds.Width = ( szbounds.Width < this.nMinLength ) ? this.nMinLength : szbounds.Width;
					float fsizefactor = (float)this.nMaxLength/(float)szbounds.Width;
					if( fsizefactor <= 1.0f )
					{
						if( this.bFullRow == false )
							return new Size( this.nMaxLength, this.nCommandBarHt );
						else
							return new Size( this.Parent.Width, this.nCommandBarHt );
					}
					else
						return new Size( szbounds.Width, this.nCommandBarHt+( this.nIntegral*(int)Math.Floor( fsizefactor ) ) );
				}
			}
			else
			{
				Size szchild = this.CalcChildControlBounds( szbounds ).Size;
				CommandBarResizeType rt = CommandBarResizeType.None;
				if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
					rt = CommandBarResizeType.Right;
				else
					rt = CommandBarResizeType.Bottom;
				CommandBarWrappingEventArgs arg =
					new CommandBarWrappingEventArgs( szchild, rt );
				this.OnCommandBarWrapping( arg );
				return this.GetAdjustedSize( szchild, arg.ClientSize );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual Size GetFloatWrapSize( Size szbounds, CommandBarResizeType rtsize )
		{
			int nCaptionHt = CommandBar.CaptionHeight;
			if( ( szbounds.Width <= 0 ) || ( szbounds.Height <= 0 ) )
				return new Size( this.Width, this.Height-nCaptionHt );
			if( this.CommandBarWrapping == null )
			{
				szbounds.Height -= nCaptionHt;
				SizeF sztext = this.CalcTextLength( CommandBarDockState.Float, true );
				int ntextoff = 0;
				if( sztext.Width > 0 )
					ntextoff = (int)sztext.Width+2;
				int nxoff = this.nHeaderOff+2+ntextoff+2+this.nTrailBtnOff;
				int nmaxbarlength = ( this.nMaxLength - nxoff ) + 2;
				int nminbarlength = ( this.nMinLength - nxoff ) + 2;
				int nminwidth = CommandBar.FormMinWidth;
				nminbarlength = ( nminbarlength < nminwidth ) ? nminwidth : nminbarlength;
				nmaxbarlength = ( nmaxbarlength < nminwidth ) ? nminwidth : nmaxbarlength;

				if( ( rtsize == CommandBarResizeType.Top ) || ( rtsize == CommandBarResizeType.Bottom ) )	// Vertical sizing
				{
					double fsizefactor = 0;
					int nmaxheight = this.nCommandBarHt+( this.nIntegral*(int)Math.Floor( (double)nmaxbarlength/nminbarlength ) );
					int ntemplength = 0;
					if( szbounds.Height <= this.nCommandBarHt )
					{
						ntemplength = nmaxbarlength;
						fsizefactor = 0;
					}
					else if( szbounds.Height >= nmaxheight )
					{
						ntemplength = nminbarlength;
						fsizefactor = nmaxbarlength/nminbarlength;
					}
					else
					{
						fsizefactor = (float)( szbounds.Height - this.nCommandBarHt )/(float)this.nIntegral;
						ntemplength = nmaxbarlength/(int)Math.Ceiling( fsizefactor );
						if( Math.Ceiling( fsizefactor ) == Math.Floor( fsizefactor ) )
							fsizefactor -= 1;
					}
					if( ntemplength == nmaxbarlength )
						fsizefactor = 0;
					return new Size( ntemplength, this.nCommandBarHt+( this.nIntegral*(int)Math.Floor( fsizefactor ) ) );
				}
				else // Horizontal sizing
				{
					float fsizefactor = (float)nmaxbarlength/(float)szbounds.Width;
					int ntemplength = nmaxbarlength/(int)Math.Ceiling( fsizefactor );
					if( Math.Ceiling( fsizefactor ) == Math.Floor( fsizefactor ) )
						fsizefactor -= 1;
					if( ntemplength > nmaxbarlength )
					{
						ntemplength = nmaxbarlength;
						fsizefactor = 0;
					}
					if( ntemplength < nminbarlength )
					{
						ntemplength = nminbarlength;
						fsizefactor = nmaxbarlength/nminbarlength;
					}
					if( ntemplength == nmaxbarlength )
						fsizefactor = 0;
					return new Size( ntemplength, this.nCommandBarHt+( this.nIntegral*(int)Math.Floor( fsizefactor ) ) );
				}
			}
			else
			{
				Size szchild = this.CalcChildControlBounds( szbounds ).Size;
				CommandBarWrappingEventArgs arg =
					new CommandBarWrappingEventArgs( szchild, rtsize );
				this.OnCommandBarWrapping( arg );
				return this.GetAdjustedSize( szchild, arg.ClientSize );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool SetCommandBarSize( int nwidth, int nheight )
		{
			Debug.Assert( ( this.Parent is CommandBarForm ) == false );
			DockStyle dsparent = this.cdbParent.Dock;
			bool badjust = false;
			if( this.bDockModeWrapping == true )
			{
				Size szwrap = this.GetDockWrapSize( new Size( nwidth, nheight ) );
				if( ( dsparent == DockStyle.Top ) || ( dsparent == DockStyle.Bottom ) )
				{
					if( szwrap.Height != nheight )
					{
						int ndim = this.cdbParent.ConfirmNewRowHeight( this, szwrap.Height );
						if( ndim != nheight )
						{
							this.cdbParent.AdjustRowHeight( this, ndim-nheight );
							nheight = ndim;
						}
					}
					if( nwidth != szwrap.Width )
						badjust = true;
					nwidth = szwrap.Width;
				}
				else
				{
					if( szwrap.Width != nwidth )
					{
						int ndim = this.cdbParent.ConfirmNewRowHeight( this, szwrap.Width );
						if( ndim != nwidth )
						{
							this.cdbParent.AdjustRowHeight( this, ndim-nwidth );
							nwidth = ndim;
						}
					}
					if( nheight != szwrap.Height )
						badjust = true;
					nheight = szwrap.Height;
				}
			}

			if( ( this.Width != nwidth ) || ( this.Height != nheight ) )
			{
				this.Width = nwidth;
				this.Height = nheight;

				if( badjust == true )
				{
					this.nRowOffsetDir = this.VLeft;
					this.nRowOffsetInDir = this.nRowOffsetDir;
					this.cdbParent.AdjustNextBarsInRow( this );
				}

				return true;
			}
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Size GetAdjustedSize( Size szmaxctrl, Size szproposedctrl )
		{
			// Make sure that the critical dimension is a multiple of the nCommandBarHt value and is within the allowed lengths.
			CommandBarDockState cbd = this.DockState;
			if( ( cbd == CommandBarDockState.Top ) || ( cbd == CommandBarDockState.Bottom ) || ( cbd == CommandBarDockState.Float ) )
			{
				SizeF sztext = this.CalcTextLength( cbd, true );
				int ntextoff = 0;
				if( sztext.Width > 0 )
					ntextoff = (int)sztext.Width+2;
				int nxinc = this.nHeaderOff+2+ntextoff+2+this.nTrailBtnOff;
				int nyinc = 2;

				Size szmaxbar = new Size( szmaxctrl.Width+nxinc, szmaxctrl.Height+nyinc );
				Size szproposedbar = new Size( szproposedctrl.Width+nxinc, szproposedctrl.Height+nyinc );

				double fdimdiff = (double)( szproposedbar.Height - this.nCommandBarHt )/(double)this.nIntegral;
				if( fdimdiff <= 0 )
					szproposedbar.Height = this.nCommandBarHt;
				else if( Math.Floor( fdimdiff ) != Math.Ceiling( fdimdiff ) )
					szproposedbar.Height = this.nCommandBarHt + ( this.nIntegral*(int)Math.Ceiling( fdimdiff ) );

				if( ( this.OccupyFullRow == true ) && ( cbd != CommandBarDockState.Float ) )
				{
					szproposedbar.Width = this.Parent.Width;
				}
				else
				{
					if( cbd != CommandBarDockState.Float )
						szproposedbar.Width = ( szproposedbar.Width > szmaxbar.Width ) ? szmaxbar.Width : szproposedbar.Width;
					szproposedbar.Width = ( szproposedbar.Width > this.nMaxLength ) ? this.nMaxLength : szproposedbar.Width;
					szproposedbar.Width = ( szproposedbar.Width < this.nMinLength ) ? this.nMinLength : szproposedbar.Width;
				}

				// If in float mode, before returning, remove the nx increment and add 2pxl border to get the
				// floating bar size.
				if( cbd == CommandBarDockState.Float )
					szproposedbar.Width = ( szproposedbar.Width - nxinc )+2;
				return szproposedbar;
			}
			else
			{
				SizeF sztext = this.CalcTextLength( CommandBarDockState.Left, true );
				int ntextoff = 0;
				if( sztext.Width > 0 )
					ntextoff = (int)sztext.Width+2;
				int nxinc = 2;
				int nyinc = this.nHeaderOff+2+ntextoff+2+this.nTrailBtnOff;

				Size szmaxbar = new Size( szmaxctrl.Width+nxinc, szmaxctrl.Height+nyinc );
				Size szproposedbar = new Size( szproposedctrl.Width+nxinc, szproposedctrl.Height+nyinc );

				double fdimdiff = (double)( szproposedbar.Width - this.nCommandBarHt )/(double)this.nIntegral;
				if( fdimdiff <= 0 )
					szproposedbar.Width = this.nCommandBarHt;
				else if( Math.Floor( fdimdiff ) != Math.Ceiling( fdimdiff ) )
					szproposedbar.Width = this.nCommandBarHt + ( this.nIntegral*(int)Math.Ceiling( fdimdiff ) );
				if( ( this.OccupyFullRow == true ) && ( cbd != CommandBarDockState.Float ) )
				{
					szproposedbar.Height = this.Parent.Height;
				}
				else
				{
					szproposedbar.Height = ( szproposedbar.Height > szmaxbar.Height ) ? szmaxbar.Height : szproposedbar.Height;
					szproposedbar.Height = ( szproposedbar.Height > this.nMaxLength ) ? this.nMaxLength : szproposedbar.Height;
					szproposedbar.Height = ( szproposedbar.Height < this.nMinLength ) ? this.nMinLength : szproposedbar.Height;
				}
				return szproposedbar;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected SizeF CalcTextLength( CommandBarDockState cdb, bool bmaxlength )
		{
			SizeF sztext = Size.Empty;
			if( ( this.Text != String.Empty ) && ( this.bShowDockModeText == true ) )
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				Graphics gph = this.CreateGraphics();
				int nCaptionHt = CommandBar.CaptionHeight;
				int ntrailbtnoff = this.nTrailBtnOff;
				if( bmaxlength == false )
				{
					if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) )
						sztext = gph.MeasureString( this.Text, this.Font, this.Width-( this.nHeaderOff+2 )-( ntrailbtnoff+1 ), sf );
					else if( ( cdb == CommandBarDockState.Left ) || ( cdb == CommandBarDockState.Right ) )
						sztext = gph.MeasureString( this.Text, this.Font, this.Height-( this.nHeaderOff+2 )-( ntrailbtnoff+1 ), sf );
					else	// CommandBarDockState.Float
						sztext = gph.MeasureString( this.Text, this.Font, this.CaptionRect.Width-2-( nCaptionHt*2 ), sf );
				}
				else
				{
					int nmaxbarlength = this.nMaxLength-( this.nHeaderOff+2+2+ntrailbtnoff );
					if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) || ( cdb == CommandBarDockState.Float ) )
						sztext = gph.MeasureString( this.Text, this.Font, nmaxbarlength, sf );
					else if( ( cdb == CommandBarDockState.Left ) || ( cdb == CommandBarDockState.Right ) )
						sztext = gph.MeasureString( this.Text, this.Font, nmaxbarlength, sf );
				}
				gph.Dispose();
                sf.Dispose();
			}
			return sztext;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetCommandBarBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			CommandBarDockState cdb = this.DockState;
			if( ( this.cbController != null ) && ( cdb != CommandBarDockState.None ) )
			{
				if( cdb != CommandBarDockState.Float )
				{
					Rectangle rcclient = this.ClientRectangle;
					Rectangle rctext = Rectangle.Empty;
					SizeF sztext = this.CalcTextLength( cdb, true );
					Rectangle rctrail = Rectangle.Empty;
					bool binvalidateall = false;
					int ntrailbtnoff = this.nTrailBtnOff;

					if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) )
					{
						if( ( this.bShowDockModeText == true ) && ( rcclient.Width <= sztext.Width+this.nHeaderOff+2+ntrailbtnoff ) )
							rctext = new Rectangle( this.nHeaderOff+2, rcclient.Top, rcclient.Width-( this.nHeaderOff+2+ntrailbtnoff ), rcclient.Height );
						rctrail = new Rectangle( rcclient.Right-ntrailbtnoff, rcclient.Top, ntrailbtnoff, rcclient.Height );
						if( ( this.ShouldDrawThemed() == false ) && ( this.cbController.Style == VisualStyle.Office2003 ) )
							rctrail.Inflate( 3, 1 );

						if( width < this.nMinLength )
							width = this.nMinLength;

						if( height != this.Height )
							binvalidateall = true;
					}
					else
					{
						if( ( this.bShowDockModeText == true ) && ( rcclient.Height <= sztext.Width+this.nHeaderOff+2+ntrailbtnoff ) )
							rctext = new Rectangle( rcclient.Left, this.nHeaderOff+2, rcclient.Width, rcclient.Height-( this.nHeaderOff+2+ntrailbtnoff ) );
						rctrail = new Rectangle( rcclient.Left, rcclient.Bottom-ntrailbtnoff, rcclient.Width, ntrailbtnoff );
						if( ( this.ShouldDrawThemed() == false ) && ( this.cbController.Style == VisualStyle.Office2003 ) )
							rctrail.Inflate( 1, 3 );

						if( height < this.nMinLength )
							height = this.nMinLength;

						if( width != this.Width )
							binvalidateall = true;
					}

					if( binvalidateall == false )
					{
						if( ( rctext.Width > 0 ) || ( rctext.Height > 0 ) )
							this.Invalidate( rctext, false );

						if( ( this.bHideChevron == false ) || ( this.bHideDropDown == false ) )
							this.Invalidate( rctrail, false );

						base.SetBoundsCore( x, y, width, height, specified );

						rctext = Rectangle.Empty;
						if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) )
						{
							if( ( this.bShowDockModeText == true ) && ( width <= sztext.Width+this.nHeaderOff+2+ntrailbtnoff ) )
								rctext = new Rectangle( this.nHeaderOff+2, rcclient.Top, width-( this.nHeaderOff+2+ntrailbtnoff ), height );

							rctrail = new Rectangle( width-ntrailbtnoff, rcclient.Top, ntrailbtnoff, height );
							if( ( this.ShouldDrawThemed() == false ) && ( this.cbController.Style == VisualStyle.Office2003 ) )
								rctrail.Inflate( 3, 1 );
						}
						else
						{
							if( ( this.bShowDockModeText == true ) && ( height <= sztext.Width+this.nHeaderOff+2+ntrailbtnoff ) )
								rctext = new Rectangle( rcclient.Left, this.nHeaderOff+2, width, height-( this.nHeaderOff+2+ntrailbtnoff ) );

							rctrail = new Rectangle( height-ntrailbtnoff, rcclient.Left, width, ntrailbtnoff );
							if( ( this.ShouldDrawThemed() == false ) && ( this.cbController.Style == VisualStyle.Office2003 ) )
								rctrail.Inflate( 1, 3 );
						}

						if( ( rctext.Width > 0 ) || ( rctext.Height > 0 ) )
							this.Invalidate( rctext, false );

						if( ( this.bHideChevron == false ) || ( this.bHideDropDown == false ) )
							this.Invalidate( rctrail, false );
					}
					else
					{
						base.SetBoundsCore( x, y, width, height, specified );
						this.Invalidate( false );
					}
				}
				else
				{
					base.SetBoundsCore( x, y, width, height, specified );
				}
				if( this.Controls.Count > 0 )
					this.SetChildControlBounds();
			}
			else
				base.SetBoundsCore( x, y, width, height, specified );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetChildControlBounds()
		{
			if( this.Controls.Count <= 0 )
				return;

			CommandBarDockState cdb = this.DockState;
			if( cdb == CommandBarDockState.None )
				return;

			bool bRTL = this.IsRTL;
			Control ctrlchild = this.Controls[0];

			if( cdb != CommandBarDockState.Float )
			{
				SizeF sztext = this.CalcTextLength( cdb, false );
				int ntextoff = 0;
				if( sztext.Width > 0 )
					ntextoff = (int)sztext.Width+2;

				Point childLoc = Point.Empty;
				Size childSz = Size.Empty;
				Rectangle childBounds = Rectangle.Empty;
				if( ( cdb == CommandBarDockState.Top ) || ( cdb == CommandBarDockState.Bottom ) )
				{
					childLoc = new Point( this.nHeaderOff+2+ntextoff, 1 );
					childSz = new Size( this.Width-childLoc.X-2-this.nTrailBtnOff, this.Height-2 );

					if( bRTL )
					{
						childLoc.X = this.nTrailBtnOff+2;
					}

					childBounds = new Rectangle( childLoc, childSz );
					this.AdjustChildControlBounds( ref childBounds );
				}
				else
				{
					childLoc = new Point( 1, ( this.nHeaderOff+2+ntextoff ) );
					childSz = new Size( this.Width-2, this.Height-childLoc.Y-2-this.nTrailBtnOff );

					if( bRTL )
					{
						childLoc.Y = this.nTrailBtnOff + 2;
					}

					childBounds = new Rectangle( childLoc, childSz );
					this.AdjustChildControlBounds( ref childBounds );
				}
				ctrlchild.Location = childBounds.Location;
				ctrlchild.Size = childBounds.Size;
			}
			else
			{
				int nCaptionHt = CommandBar.CaptionHeight;
				Point childLoc = new Point( 1, 1+nCaptionHt );
				Size childSz = new Size( this.Width-2, this.Height-nCaptionHt-2 );

				Rectangle childBounds = new Rectangle( childLoc, childSz );
				this.AdjustChildControlBounds( ref childBounds );

				ctrlchild.Location = childBounds.Location;
				ctrlchild.Size = childBounds.Size;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void AdjustChildControlBounds( ref Rectangle childBounds )
		{
		}

		private Point CorrectBarLocation( Size fornsize, Point ptscr )
		{
			Point location = ptscr;

			Rectangle scrRect = SystemInformation.VirtualScreen;

			//correct x-coordinates
			if( location.X > scrRect.Right - 10 )
			{
				if( location.X > scrRect.Right )
				{
					location.X = scrRect.Right - this.Size.Width;
				}
				else
				{
					location.X = scrRect.Right - 10;
				}
			}

			// correct y-coordinates
			if( location.Y > scrRect.Bottom - 10 )
			{
				if( location.Y > scrRect.Bottom )
				{
					location.Y = scrRect.Bottom - this.Size.Height;
				}
				else
				{
					location.Y = scrRect.Bottom - 10;
				}
			}

			return location;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void EnterFloatMode( Point ptscreen )
		{
			float ptxoffset = 0;
			if( CommandBar.ptDeltaOff != Point.Empty )
				ptxoffset = (float)CommandBar.ptDeltaOff.X/(float)this.Width;

			if( this.cdbParent != null )
			{
				this.cdbParent.RemoveCommandBar( this );
				if( ( this.cdbParent.Dock == DockStyle.Left ) || ( this.cdbParent.Dock == DockStyle.Right ) )
				{
					this.ChangeCommandBarOrientation();
				}
				else
				{
					this.cbController.RecalcBorderLayout( CommandBarDockState.Left );
					this.cbController.RecalcBorderLayout( CommandBarDockState.Right );
				}
			}
			this.cbarDockState = CommandBarDockState.Float;
			CommandBarForm cbf = null;
			if( this.Parent == null )
			{
				cbf = this.CreateFloatingForm();
				cbf.Name = String.Concat( "CommandBarForm", this.Name );
			}
			else	// hosted within a floating form
			{
				Debug.Assert( this.Parent is CommandBarForm );
				cbf = this.Parent as CommandBarForm;
			}
			Color backcolor = this.cbController.BackColor;
			if( ( this.bBackColorSet == true ) && ( this.BackColor != Color.Transparent ) )
				backcolor = this.BackColor;
			cbf.BackColor = backcolor;
			cbf.Size = new Size( 0, 0 );
			cbf.Owner = this.cbController.HostForm;

			NativeMethods.SetWindowPos( cbf.Handle, (IntPtr)NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, 0x0001|0x0002|0x0010|0x0040 ); //  SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_SHOWWINDOW

			if( this.cbController.bLoadVisibility )
				cbf.Visible = true;

			cbf.Controls.Add( this );

			Size formsize = this.CalculateFloatingSize();
			if( ptxoffset != 0 )
			{
				// Reposition the form relative to the cursor to adjust for the change in size.
				CommandBar.ptDeltaOff = new Point( (int)( ptxoffset * (float)formsize.Width ), CommandBar.ptDeltaOff.Y );
				cbf.Location = new Point( ptscreen.X - CommandBar.ptDeltaOff.X, ptscreen.Y );
			}
			else
			{
				cbf.Location = CorrectBarLocation( formsize, ptscreen );
			}

			cbf.Size = formsize;
			this.rcFloat = cbf.Bounds;

			if( this.cbController.HostForm != null && this.cbController.ShouldHostFormGetFocus() )
			{
				this.cbController.HostForm.Focus();
			}
		}

		/// <summary>
		/// Changes the command bar orientation.
		/// </summary>
		protected virtual void ChangeCommandBarOrientation()
		{
			this.Size = new Size( this.Height, this.Width );
		}

		/// <summary>
		/// Gets the size of the floating bar.
		/// </summary>
		protected internal virtual Size CalculateFloatingSize()
		{
			Size szbar = Size.Empty;

			if( !this.bFloatModeWrapping )
			{
				szbar = new Size( ( this.nMaxLength - ( this.nHeaderOff + 2 + 2 + this.nTrailBtnOff ) ) + 2, this.nCommandBarHt );
			}
			else
			{
				if( ( this.rcFloat.Width == 0 ) || ( this.rcFloat.Height == 0 ) )
				{
					szbar = this.GetFloatWrapSize( new Size( ( this.nMaxLength - ( this.nHeaderOff + 2 + 2 + this.nTrailBtnOff ) ) + 2, this.nCommandBarHt ), CommandBarResizeType.Right );
				}
				else
				{
					szbar = this.GetFloatWrapSize( new Size( this.rcFloat.Width - 4, this.rcFloat.Height - 4 ), CommandBarResizeType.Right );
				}
			}

			Size szfloat = new Size( szbar.Width + 4, szbar.Height + 4 + CommandBar.CaptionHeight );
			szfloat.Width = Math.Max( szfloat.Width, CommandBar.FormMinWidth );

			return szfloat;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual CommandBarForm CreateFloatingForm()
		{
			return new CommandBarForm( this.cbController );
		}

		// Accessibility Implementation
		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new CommandBarAccessibleObject( this );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual Control GetPopupParentControl()
		{
			return this;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool IsRelatedControl( Control control, bool askPopupParent )
		{
			return control == this;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual PopupRelativeAlignment GetFirstAlignPreference()
		{
			bool bRTL = this.IsRTL;

			switch( this.DockState )
			{
				default:
				case CommandBarDockState.Top:
				case CommandBarDockState.Float:
				return bRTL ? PopupRelativeAlignment.BottomRight : PopupRelativeAlignment.BottomLeft;
				case CommandBarDockState.Left:
				return bRTL ? PopupRelativeAlignment.LeftBottom : PopupRelativeAlignment.RightBottom;
				case CommandBarDockState.Bottom:
				return bRTL ? PopupRelativeAlignment.TopLeft : PopupRelativeAlignment.TopRight;
				case CommandBarDockState.Right:
				return bRTL ? PopupRelativeAlignment.RightBottom : PopupRelativeAlignment.LeftBottom;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool IsShowingDropDown()
		{
			bool showingDropDown = false;
			if( this.popupContainer != null )
			{
				showingDropDown |= this.popupContainer.IsShowing();
			}
			else if( this.pupMenu != null )
				showingDropDown |= this.pupMenu.IsShowing();

			return showingDropDown;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool ShowDropDown()
		{
			if( this.popupContainer != null )
			{
				this.popupContainer.PopupParent = this;
				this.popupContainer.ParentControl = this;
				this.popupContainer.RightToLeft = this.RightToLeft;
				this.popupContainer.ShowPopup( Point.Empty );
				return true;
			}
			else if( this.pupMenu != null )
			{
				m_bIsCustomizing = true;

				this.pupMenu.ParentBarItem.PopupClosed -= new EventHandler( OnPopupClosed );
				this.pupMenu.ParentBarItem.PopupClosed += new EventHandler( OnPopupClosed );
				//this.pupMenu.Show(this, Point.Empty);
				this.pupMenu.ShowChildrenUI( Point.Empty, this );
			}
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void HideDropDown()
		{
			if( this.popupContainer != null 	&& this.popupContainer.IsShowing() )
				this.popupContainer.HidePopup( PopupCloseType.Deactivated );

			if( this.pupMenu != null && this.pupMenu.IsShowing() && this.pupMenu.ShouldHidePopupOnDeactivate() )
			{
				this.pupMenu.Hide();
			}
		}

		/// <summary>
		/// Updates color scheme.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();

			Office2007Theme theme = ( cbController != null ) ? 
				cbController.Office2007Theme : Office2007Theme.Blue;
			Office2007OutlookColors.UpdateMenuColors( theme );
		}
		/// <summary>
		/// Sets renderer for CommandBar.
		/// </summary>
		protected virtual void SetRenderer()
		{
			VisualStyle style = ( this.cbController == null ) ? VisualStyle.Default : 
				this.cbController.Style;
			CommandBarDockState dockState = this.cbarDockState;

			if( dockState == CommandBarDockState.Float )
			{
				if( m_rendererFloating != null )
				{
					m_rendererFloating.Dispose();
				}
			}
			else
			{
				if( m_renderer != null )
				{
					m_renderer.Dispose();
				}
			}

			if( ShouldDrawThemed() 
				&& style != VisualStyle.Office2007
                && style != VisualStyle.Office2010
				&& style != VisualStyle.Office2007Outlook )
			{
				if( dockState == CommandBarDockState.Float )
				{
					m_rendererFloating = new CommandBarFloatingRendereThemed( this );
				}
				else
				{
					m_renderer = new CommandBarRendererThemed( this );
				}
			}
			else
			{
				switch( style )
				{
                    case VisualStyle.Metro:
                    {
                        if (dockState == CommandBarDockState.Float)
                        {
                            m_rendererFloating = new CommandBarFloatingRendererMetro(this);
                        }
                        else
                        {
                            m_renderer = new CommandBarRendererMetro(this);
                        }
                        break;
                    }
					case VisualStyle.VS2005:
					{
						if( dockState == CommandBarDockState.Float )
						{
							m_rendererFloating = new CommandBarFloatingRendererVS2005( this );
						}
						else
						{
							m_renderer = new CommandBarRendererVS2005( this );
						}
						break;
					}
					case VisualStyle.Office2003:
					{
						if( dockState == CommandBarDockState.Float )
						{
							m_rendererFloating = new CommandBarFloatingRendererOffice2003( this );
						}
						else
						{
							m_renderer = new CommandBarRendererOffice2003( this );
						}
						break;
					}
					case VisualStyle.Office2007Outlook:
					case VisualStyle.Office2007:
					{
						if( dockState == CommandBarDockState.Float )
						{
							m_rendererFloating = new CommandBarFloatingRendererOffice2007( this );
						}
						else
						{
							m_renderer = new CommandBarRendererOffice2007( this );
						}
						break;
					}
                    case VisualStyle.Office2010:
                    {
                        if (dockState == CommandBarDockState.Float)
                        {
                            m_rendererFloating = new CommandBarFloatingRendererOffice2010(this);
                        }
                        else
                        {
                            m_renderer = new CommandBarRendererOffice2010(this);
                        }
                        break;
                    }
					default:
					{
						if( dockState == CommandBarDockState.Float )
						{
							m_rendererFloating = new CommandBarFloatingRendererOfficeXP( this );
						}
						else
						{
							m_renderer = new CommandBarRendererOfficeXP( this );
						}
						break;
					}
				}
			}
		}


		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		internal Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( cbController == null ) ? 
					Office2007Colors.Default : cbController.Office2007ColorTable;

				return colorTable;
			}
		}

        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        internal Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (cbController == null) ?
                    Office2010Colors.Default : cbController.Office2010ColorTable;

                return colorTable;
            }
        }
		#endregion

		#region Class Event Handlers
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnVisibleChanged( EventArgs e )
		{
			// Allow the VisibleChanged event to be fired only if the internal visibility state
			// matches the Control visibility state. Docking/Undocking visibility changes should be ignored.
			if( this.bVisibilitySetFlag )
			{
				base.OnVisibleChanged( e );
				this.bVisibilitySetFlag = false;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );
			this.Invalidate( false );
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnControlAdded"/>.
		/// </summary>
		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );

			e.Control.Dock = DockStyle.None;
			e.Control.Anchor = AnchorStyles.Top|AnchorStyles.Left;

			if( this.cbController != null )
				this.SetChildControlBounds();

			if( !this.DesignProcess )
				this.RecSubscribeChildControlEvents( e.Control, true );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnControlRemoved"/>.
		/// </summary>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
			base.OnControlRemoved( e );

			if( !this.DesignProcess )
				this.RecSubscribeChildControlEvents( e.Control, false );
		}

		// Drill down the child hierarchy and subscribe to the focus/controladded events. The focus events
		// are used for painting the caption area with the selected state.
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecSubscribeChildControlEvents( Control ctrl, bool subscribe )
		{
			if( subscribe )
			{
				ctrl.GotFocus += new System.EventHandler( this.childctrl_GotFocus );
				ctrl.LostFocus += new System.EventHandler( this.childctrl_LostFocus );
				ctrl.ControlAdded += new System.Windows.Forms.ControlEventHandler( this.childctrl_ControlAdded );
				ctrl.ControlRemoved += new System.Windows.Forms.ControlEventHandler( this.childctrl_ControlRemoved );
			}
			else
			{
				ctrl.GotFocus -= new System.EventHandler( this.childctrl_GotFocus );
				ctrl.LostFocus -= new System.EventHandler( this.childctrl_LostFocus );
				ctrl.ControlAdded -= new System.Windows.Forms.ControlEventHandler( this.childctrl_ControlAdded );
				ctrl.ControlRemoved -= new System.Windows.Forms.ControlEventHandler( this.childctrl_ControlRemoved );
			}
			if( ctrl.Controls.Count > 0 )
			{
				IEnumerator iechildren = ctrl.Controls.GetEnumerator();
				while( iechildren.MoveNext() )
				{
					RecSubscribeChildControlEvents( iechildren.Current as Control, subscribe );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void childctrl_GotFocus( Object sender, EventArgs e )
		{
			if( this.cbController != null )
			{
				this.cbController.FocusedCommandBar = this;
			}
			this.Invalidate( this.CaptionRect );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void childctrl_LostFocus( Object sender, EventArgs e )
		{
			if( cbController != null )
			{
				this.cbController.FocusedCommandBar = null;
			}

			this.Invalidate( this.CaptionRect );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void childctrl_ControlAdded( Object sender, ControlEventArgs e )
		{
			this.RecSubscribeChildControlEvents( e.Control, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void childctrl_ControlRemoved( Object sender, ControlEventArgs e )
		{
			this.RecSubscribeChildControlEvents( e.Control, false );
		}

		// A new CommandBar is about to be added to the controller. If any redock/recalc events are
		// pending, then complete them before the add.
		internal void OnNewBarAddedEH( Object obj, EventArgs e )
		{
			if( this.Visible && ( null != this.Parent ) )
			{
				this.RedockIfNeeded();
				this.RecalcIfNeeded();
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
		/// </summary>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );

			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
			int newheight = SystemInformation.MenuFont.Height;
			if( CommandBar.nSysInfoMenuFontHeight != newheight )
				CommandBar.nSysInfoMenuFontHeight = newheight;

			this.SetRenderer();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void MenuColorsChanged( object sender, EventArgs e )
		{
			this.Invalidate();
			if( this.Parent is CommandBarForm )
				this.Parent.Invalidate();
		}

		internal void XPThemes_ThemeChanged( object sender, EventArgs e )
		{
			m_needRefreshTextData = true;
			if( this.Floating )
			{
				this.Invalidate();
			}
		}

		/// <summary>
		/// Raises the <see cref="CommandBar.CommandBarWrapping"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="CommandBarWrappingEventArgs"/> value that contains the event data.</param>
		protected virtual void OnCommandBarWrapping( CommandBarWrappingEventArgs arg )
		{
			if( this.CommandBarWrapping != null )
			{
				CommandBarWrapping( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="CommandBar.CommandBarDropDownClicked"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnCommandBarDropDownClicked( EventArgs arg )
		{
			if( this.CommandBarDropDownClicked != null )
			{
				CommandBarDropDownClicked( this, arg );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireCommandBarStateChanging( CommandBarStateChangingEventArgs args )
		{
			this.OnCommandBarStateChanging( args );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireCommandBarStateChanged( EventArgs args )
		{
			this.OnCommandBarStateChanged( args );
		}

		/// <summary>
		/// Raises the <see cref="CommandBar.CommandBarStateChanging"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="CommandBarStateChanging"/> value that contains the event data.</param>
		protected virtual void OnCommandBarStateChanging( CommandBarStateChangingEventArgs arg )
		{
			if( this.CommandBarStateChanging != null )
			{
				CommandBarStateChanging( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="CommandBar.CommandBarStateChanged"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnCommandBarStateChanged( EventArgs arg )
		{
			if( !this.bRecalcNeeded )
			{
				if( this.CommandBarStateChanged != null )
				{
					this.CommandBarStateChanged( this, arg );
				}
			}
			else
			{
				this.bFireDockStateChanged = true;
			}

			SetRenderer();
		}

		/// <summary>
		/// Raises the <see cref="CommandBar.CommandBarUserClosed"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnCommandBarUserClosed( EventArgs arg )
		{
			if( this.CommandBarUserClosed != null )
				CommandBarUserClosed( this, arg );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnPopupClosed( object sender, EventArgs e )
		{
			m_bIsCustomizing = false;
		}

		private void cbController_StyleChanged( object sender, EventArgs e )
		{
			SetRenderer();
		}

		private void cbController_ThemesEnabledChanged( object sender, EventArgs e )
		{
			SetRenderer();
		}
		#endregion

		#region Class Initialize/Finalize Methods
		/// <summary>
		/// Creates a new instance of the CommandBar class.
		/// </summary>
		public CommandBar()
		{
			InitializeCommandBar();
		}

		protected internal bool DesignProcess
		{
			get
			{
				bool bDesignProcess = false;

				IDesignerHost host = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					IDesigner idsgnr = host.GetDesigner( this );
					if( idsgnr != null )
					{
						bDesignProcess = true;
					}
				}

				return bDesignProcess;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitializeCommandBar()
		{
			this.SetStyle( ControlStyles.Selectable, false );
			this.SetStyle( ControlStyles.StandardDoubleClick
				|ControlStyles.UserPaint|ControlStyles.SupportsTransparentBackColor
				|ControlStyles.AllPaintingInWmPaint|ControlStyles.DoubleBuffer,
				true );
			this.TabStop = false;

			this.Font = SystemInformation.MenuFont;
			this.pupMenu = new PopupMenu();
			this.pupMenu.ParentBarItem = new ParentBarItem();

			if( CommandBar.ftFloatCaption == null )
				CommandBar.InitializeFloatCaptionFont();

			commandBarWeakContainer = new CommandBarWeakContainer( this );
			MenuColors.MenuColorsChanged += new EventHandler( this.commandBarWeakContainer.MenuColorsChangedWeakEventHandler );
			Office2003Colors.MenuColorsChanged += new EventHandler( this.commandBarWeakContainer.Office2003ColorsChangedWeakEventHandler );
			XPThemes.ThemeChanged +=new EventHandler( this.commandBarWeakContainer.XPThemesChangedWeakEventHandler );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal static void InitializeFloatCaptionFont()
		{
			Syncfusion.Runtime.InteropServices.NativeMethods.NONCLIENTMETRICS ncm = new Syncfusion.Runtime.InteropServices.NativeMethods.NONCLIENTMETRICS();
			ncm.cbSize = Marshal.SizeOf( ncm );
			if( Syncfusion.Runtime.InteropServices.NativeMethods.SystemParametersInfo( 0x0029/*SPI_GETNONCLIENTMETRICS*/, 0, ref ncm, 0 ) != 0 )
			{
				if( CommandBar.ftFloatCaption != null )
					CommandBar.ftFloatCaption.Dispose();
				CommandBar.ftFloatCaption = Syncfusion.Drawing.FontUtil.CreateFont( (String)ncm.lfSmCaptionFont.lfFaceName, 8, FontStyle.Bold );
			}
			else if( CommandBar.ftFloatCaption == null )
			{
				CommandBar.ftFloatCaption = Syncfusion.Drawing.FontUtil.CreateFont( SystemInformation.MenuFont.Name, 8, FontStyle.Bold );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public void InitiateFloatingResize( Point ptscreen, int nchittest )
		{

		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( this.rMarker != null )
			{
				this.rMarker.DecreaseRefCount();
				this.rMarker = null;
			}
            
            foreach( Control ctrl in this.Controls )
			{
				this.RecSubscribeChildControlEvents( ctrl, false );
			}

            if (this.cbController != null)
            {
                 this.cbController.StyleChanged-= new EventHandler(cbController_StyleChanged);
                 this.cbController.ThemesEnabledChanged -= new EventHandler(cbController_ThemesEnabledChanged);
                
                 this.cbController = null;
            }

			if( null != this.pupMenu )
			{
				if( null != this.pupMenu.ParentBarItem )
				{
					this.pupMenu.ParentBarItem.Dispose();
					this.pupMenu.ParentBarItem = null;
				}

				this.pupMenu.Dispose();
				this.pupMenu = null;
			}
            if (this.m_renderer != null)
            {
                this.m_renderer.Dispose();
                this.m_renderer = null;
            }
            if (this.commandBarWeakContainer != null)
            {
        	    MenuColors.MenuColorsChanged -= new EventHandler( this.commandBarWeakContainer.MenuColorsChangedWeakEventHandler );
			    Office2003Colors.MenuColorsChanged -= new EventHandler( this.commandBarWeakContainer.Office2003ColorsChangedWeakEventHandler );
                XPThemes.ThemeChanged -= new EventHandler(this.commandBarWeakContainer.XPThemesChangedWeakEventHandler);
                this.commandBarWeakContainer = null;
            }
            if(!this.DesignMode)
			base.Dispose( disposing );
		}

		#endregion

		#region Class Overrides
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		protected override void OnPaint( PaintEventArgs e )
		{
			UpdateColorScheme();

			if( this.cbarDockState == CommandBarDockState.Float )
			{
				if( m_rendererFloating != null )
				{
					m_rendererFloating.Draw( e.Graphics );
				}
			}
			else
			{
				if( m_renderer != null )
				{
					m_renderer.Draw( e.Graphics );
				}
			}

			base.OnPaint( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( ( this.cbController != null ) && !this.DesignProcess )
				this.HandleMouseDown( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( ( this.cbController != null ) && !this.DesignProcess )
				this.HandleMouseMove( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( ( this.cbController != null ) && !this.DesignProcess )
				this.HandleMouseUp( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDoubleClick"/>.
		/// </summary>
		protected override void OnDoubleClick( EventArgs e )
		{
			if( ( this.cbController != null ) && !this.DesignProcess )
				this.HandleDoubleClick( Cursor.Position );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( ( this.cbController != null ) && !this.DesignProcess )
				this.HandleMouseLeave();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.SetBoundsCore"/>.
		/// </summary>
		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			base.SetBoundsCore( x, y, width, height, specified );

			if( ( this.cbController != null ) && ( this.DockState!= CommandBarDockState.None ) && ( this.Controls.Count > 0 ) )
				this.SetChildControlBounds();
			this.Invalidate();
		}

		protected override void OnRightToLeftChanged( EventArgs e )
		{
			base.OnRightToLeftChanged( e );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( this.DockState == CommandBarDockState.Float && this.Parent != null && this.Visible )
			{
				CommandBarForm cBarForm = this.Parent as CommandBarForm;

				if( cBarForm != null )
				{
					cBarForm.BringToFront();
				}
			}
#endif
		}


		#endregion

		#region ICommandBarDesignerInvoke

		void ICommandBarDesignerInvoke.SetChildControlBounds()
		{
			this.SetChildControlBounds();
		}

		CommandDockBar ICommandBarDesignerInvoke.GetCommandDockBarParent()
		{
			return this.cdbParent;
		}

		bool ICommandBarDesignerInvoke.CommandBarBaseVisibility
		{
			get { return this.CommandBarBaseVisible; }
			set { this.CommandBarBaseVisible = value; }
		}

		#endregion

		#region IPopupParent
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			Rectangle rcddbutton = this.DropDownRect;
			if( rcddbutton.Contains( this.PointToClient( Cursor.Position ) ) == false )
			{
				if( this.ShouldDrawThemed() || 
					( this.cbController.Style != VisualStyle.Office2003 
					&& this.cbController.Style != VisualStyle.VS2005 
					&& this.cbController.Style != VisualStyle.Office2007
                    && this.cbController.Style != VisualStyle.Office2010
					&& this.cbController.Style != VisualStyle.Office2007Outlook ) )
				{
					this.Invalidate( Rectangle.Inflate( rcddbutton, 1, 1 ), false );
				}
				else if( null != this.cdbParent )
				{
					if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
						this.Invalidate( Rectangle.Inflate( rcddbutton, 3, 1 ), false );
					else
						this.Invalidate( Rectangle.Inflate( rcddbutton, 1, 3 ), false );
				}
				this.cbHilight = CBButtons.None;
			}
			if( CommandBar.bHideOnMouseUp )
				CommandBar.bHideOnMouseUp = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlign )
		{
			Rectangle rcdropdown = this.DropDownRect;
			rcdropdown.Width += 1;
			rcdropdown.Height += 1;

			return PopupUtils.ComputeDefaultBorderOverlapCue( rAlign,
				this.RectangleToScreen( rcdropdown ) );
		}

		bool IPopupParent.IsRightToLeft
		{
			get
			{
				return ( RightToLeft.Yes == this.RightToLeft );
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign )
		{

			Rectangle rcdropdown = this.DropDownRect;
			rcdropdown.Width += 1;
			rcdropdown.Height += 1;
			Point pos = PopupUtils.ComputeDefaultPopupAlignment( prevAlign, out newAlign,
				this.GetFirstAlignPreference(), PopupRelativeAlignment.RightTop, rcdropdown );

			return this.PointToScreen( pos );
		}

		#endregion

		#region ICommandBarDesignerMouseHook
		// IDesignerMouseHook private implementation
		/// <summary>
		/// Handles the mouse down event.
		/// </summary>
		public virtual void HandleMouseDown( MouseButtons button, Point ptscreen )
		{
			if( button == MouseButtons.Left )
			{
				if( this.DesignProcess )
				{
					ISelectionService iss = this.GetService( typeof( ISelectionService ) ) as ISelectionService;
					if( !iss.GetComponentSelected( this ) )
					{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						iss.SetSelectedComponents( new Object[1] { this }, SelectionTypes.MouseDown);
#else
						iss.SetSelectedComponents( new Object[1] { this }, SelectionTypes.Primary );
#endif
					}
				}

				Point ptclient = this.PointToClient( ptscreen );

				if( this.cbarDockState != CommandBarDockState.Float )
				{
					// The SizeAll cursor is in use - mouse is in the 'hot rect' - drag can commence
					if( ( this.cbHilight == CBButtons.None ) && ( this.DragStartRect.Contains( ptclient ) ) )
					{
						if( !this.DesignProcess )
						{
							if( this.Cursor != Cursors.SizeAll )
							{
								CommandBar.crDefault = this.Cursor;
								this.Cursor = Cursors.SizeAll;
							}
						}

						CommandBar.bDragging = true;
						this.nRCCountDrag = this.cdbParent.RowsCount;

						// If the CommandBar does not have a RowMarker, then get one
						if( this.rMarker == null )
							this.cdbParent.SetCurrentRowMarker( this );
					}
					else
					{
						if( ( this.cbHilight & CBButtons.DropDown ) == CBButtons.DropDown )
						{
							Rectangle rcdropdown = this.DropDownRect;
							if( rcdropdown.Contains( ptclient ) )
							{
								if( this.IsShowingDropDown() )
								{
									CommandBar.bHideOnMouseUp = true;
								}
								else
								{
									this.cbHilight |= CBButtons.Pressed;
									this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
									if( !this.ShowDropDown() )
										this.OnCommandBarDropDownClicked( EventArgs.Empty );
								}
							}
							else
							{
								this.cbHilight = CBButtons.None;
								if( this.IsShowingDropDown() )
									this.HideDropDown();
							}
						}

						if( ( this.cbHilight & CBButtons.Close ) == CBButtons.Close )
						{
							Rectangle rcclose = this.CloseButtonRect;
							if( rcclose.Contains( ptclient ) )
								this.cbHilight |= CBButtons.Pressed;
							else
								this.cbHilight = CBButtons.None;
							this.Invalidate( Rectangle.Inflate( rcclose, 1, 1 ) );
						}
					}
				}
				else	// Floating frame
				{
					if( this.DragStartRect.Contains( ptclient ) || this.GripperRect.Contains( ptclient ) )
					{
						if( this.cbHilight == CBButtons.None )
						{
							if( !this.DesignProcess )
							{
								if( this.Cursor != Cursors.SizeAll )
								{
									CommandBar.crDefault = this.Cursor;
									this.Cursor = Cursors.SizeAll;
								}
							}
							CommandBar.bDragging = true;
						}
						else if( ( this.cbHilight & CBButtons.DropDown ) == CBButtons.DropDown )
						{
							Rectangle rcdropdown = this.DropDownRect;
							if( rcdropdown.Contains( ptclient ) )
							{
								this.cbHilight = CBButtons.DropDown|CBButtons.Pressed;
								if( this.IsShowingDropDown() )
								{
									CommandBar.bHideOnMouseUp = true;
								}
								else
								{
									this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
									if( !this.ShowDropDown() )
										this.OnCommandBarDropDownClicked( EventArgs.Empty );
								}
							}
							else
							{
								this.cbHilight = CBButtons.None;
								if( this.IsShowingDropDown() )
									this.HideDropDown();
							}
						}
						else if( this.cbHilight == CBButtons.Close )
						{
							this.cbHilight = CBButtons.Close|CBButtons.Pressed;
							this.Invalidate( Rectangle.Inflate( this.CloseButtonRect, 1, 1 ) );
						}
					}

					if( this.cbController.HostForm != null && this.cbController.ShouldHostFormGetFocus() )
					{
						this.cbController.HostForm.Focus();
					}
				}
				if( CommandBar.bDragging )
				{
					CommandBar.ptDeltaOff = ptclient;

					if( ( CommandBarDockState.Left == this.DockState || CommandBarDockState.Right == this.DockState )
						&& !this.bHideGripper && this.IsRTL )
					{
						ptDeltaOff.Y -= this.GripperRect.Bottom - this.GripperRect.Height/* - this.CaptionRect.Height*/;
					}

					CommandBar.ptMMWorkaround = this.PointToScreen( ptclient );

					// Store the current drag info in the temporary drag variables
					this.cdbParentDrag = this.cdbParent;
					this.nRowOffsetDrag = this.nRowOffsetInDir;
					this.nRCIndexDrag = this.nRCIndex;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void HandleMouseMove( MouseButtons button, Point ptscreen )
		{
			Point ptclient = this.PointToClient( ptscreen );

			// If the cursor is over the gripper region, set the gripper cursor
			Rectangle rcgripper = this.GripperRect;
			if( !this.DesignProcess )
			{
				if( rcgripper.Contains( ptclient ) )
				{
					if( CommandBar.crDefault == null )
					{
						CommandBar.crDefault = this.Cursor;
						this.Cursor = Cursors.SizeAll;
					}
				}
				else if( ( !CommandBar.bDragging ) && ( CommandBar.crDefault != null ) )
				{
					this.Cursor = CommandBar.crDefault;
					CommandBar.crDefault = null;
				}
			}

			if( !CommandBar.bDragging )
			{
				Rectangle rcdropdown = this.DropDownRect;
				Rectangle rcclose = this.CloseButtonRect;
				if( this.cbarDockState != CommandBarDockState.Float )
				{
					if( !this.DesignProcess )
					{
						if( rcdropdown.Contains( ptclient ) )
						{
							if( !this.cbController.bDisableButtons && ( this.cbHilight == CBButtons.None ) )
							{
								this.cbHilight = CBButtons.DropDown;
								this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
							}
						}
						else if( ( this.cbHilight & CBButtons.DropDown ) == CBButtons.DropDown )
						{
							if( !this.IsShowingDropDown() )
							{
								this.cbHilight = CBButtons.None;
								if( this.ShouldDrawThemed() || ( this.cbController == null ) || 
									( this.cbController.Style != VisualStyle.Office2003 
									&& this.cbController.Style != VisualStyle.VS2005 
									&& this.cbController.Style != VisualStyle.Office2007
                                    && this.cbController.Style != VisualStyle.Office2010
									&& this.cbController.Style != VisualStyle.Office2007Outlook ) )
								{
									this.Invalidate( Rectangle.Inflate( rcdropdown, 1, 1 ), false );
								}
								else
								{
									if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
										this.Invalidate( Rectangle.Inflate( rcdropdown, 3, 1 ), false );
									else
										this.Invalidate( Rectangle.Inflate( rcdropdown, 1, 3 ), false );
								}
							}
						}

						if( rcclose.Contains( ptclient ) )
						{
							if( !this.cbController.bDisableButtons && ( this.cbHilight == CBButtons.None ) )
							{
								this.cbHilight = CBButtons.Close;
								this.Invalidate( Rectangle.Inflate( rcclose, 1, 1 ) );
							}
						}
						else if( ( this.cbHilight & CBButtons.Close ) == CBButtons.Close )
						{
							this.cbHilight = CBButtons.None;
							this.Invalidate( Rectangle.Inflate( rcclose, 1, 1 ) );
						}
					}
				}
				else	// Floating state
				{
					if( !this.DesignProcess )
					{
						if( rcdropdown.Contains( ptclient ) )
						{
							if( !this.cbController.bDisableButtons && ( this.cbHilight == CBButtons.None ) )
							{
								this.cbHilight = CBButtons.DropDown;
								this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
							}
						}
						else if( rcclose.Contains( ptclient ) )
						{
							if( !this.cbController.bDisableButtons && ( this.cbHilight == CBButtons.None ) )
							{
								this.cbHilight = CBButtons.Close;
								this.Invalidate( Rectangle.Inflate( rcclose, 1, 1 ) );
							}
						}

						if( ( ( this.cbHilight & CBButtons.DropDown ) == CBButtons.DropDown ) && ( rcdropdown.Contains( ptclient ) == false ) )
						{
							if( !this.IsShowingDropDown() )
							{
								this.cbHilight = CBButtons.None;
								this.Invalidate( Rectangle.Inflate( rcdropdown, 1, 1 ), false );
							}
						}
						if( ( ( this.cbHilight & CBButtons.Close ) == CBButtons.Close ) && ( rcclose.Contains( ptclient ) == false ) )
						{
							this.cbHilight = CBButtons.None;
							this.Invalidate( Rectangle.Inflate( rcclose, 1, 1 ), false );
						}
					}
				}
			}
			else //(CommandBar.bDragging == true) A drag is in progress, reposition the bar
                if (ptscreen != CommandBar.ptMMWorkaround && button != MouseButtons.None)
				{
					CommandBar.ptMMWorkaround = ptscreen;
					if( this.cbarDockState != CommandBarDockState.Float )
					{
						Rectangle rcparent = this.cdbParent.RectangleToScreen( this.cdbParent.ClientRectangle );
						if( rcparent.Contains( ptscreen ) )
						{
							// Cursor is directly over a commanddockbar. Allow the dockbar to provide the position info
							Point ptlocation = this.cdbParent.HandleDragOver( this, ptscreen );
							if( ( ptlocation.X != -1 ) && ( ptlocation.Y != -1 ) )
							{
								int ndelta = 0;
								if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
								{
									ndelta = this.VLeft - ptlocation.X;
									this.VLocation = new Point( ptlocation.X, ptlocation.Y );
									this.nRowOffsetDir = this.VLeft;
								}
								else
								{
									ndelta = this.Location.Y - ptlocation.Y;
									this.Location = new Point( ptlocation.X, ptlocation.Y );
									this.nRowOffsetDir = this.Location.Y;
								}
								this.cdbParent.AdjustRowOffsets( this, ref ndelta );
							}
							else
								this.cbController.RecalcLayout( this );
						}
						else if( this.cdbParent.GetRowArray( this.nRCIndex ).Length > 1 )
						{
							int ncbheight = this.cdbParent.GetRowMaxHeight( this.nRCIndex, true );
							switch( this.cdbParent.Dock )
							{
								case DockStyle.Top:
								{
									if( ( ptscreen.Y >= rcparent.Bottom ) && ( ptscreen.Y < rcparent.Bottom+ncbheight )
									&& ( this.bLeadingEdge != true ) && ( this.cdbParent.AllowTrailingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = this.cdbParent.nRCCount;
										this.cdbParent.AddCommandBar( this, true );
									}
									else if( ( ptscreen.Y <= rcparent.Top ) && ( ptscreen.Y > rcparent.Top-ncbheight )
									&& ( this.bTrailingEdge != true ) && ( this.cdbParent.AllowLeadingEdgeDocking( this ) ) )
									{
										this.cdbParent.MoveBarFirst( this );
										this.Height = this.nCommandBarHt;

										CommandBar[] cbarray = this.cdbParent.GetRowArray( 1 );
										this.cdbParent.AdjustPreviousRow( this, ref cbarray, this.VBounds );
									}
									break;
								}
								case DockStyle.Bottom:
								{
									if( ( ptscreen.Y <= rcparent.Top ) && ( ptscreen.Y < rcparent.Top-ncbheight )
									&& ( this.bTrailingEdge != true ) && ( this.cdbParent.AllowLeadingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = 0;
										this.cdbParent.AddCommandBar( this, true );
									}
									else if( ( ptscreen.Y >= rcparent.Bottom ) && ( ptscreen.Y < rcparent.Bottom+ncbheight )
									&& ( this.bLeadingEdge != true ) && ( this.cdbParent.AllowTrailingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = this.cdbParent.nRCCount;
										this.cdbParent.AddCommandBar( this, true );
									}
									break;
								}
								case DockStyle.Left:
								{
									if( ( ptscreen.X >= rcparent.Right ) && ( ptscreen.X < rcparent.Right+ncbheight )
									&& ( this.bLeadingEdge != true ) && ( this.cdbParent.AllowTrailingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = this.cdbParent.nRCCount;
										this.cdbParent.AddCommandBar( this, true );
									}
									else if( ( ptscreen.X <= rcparent.Left ) && ( ptscreen.X > rcparent.Right+ncbheight )
									&& ( this.bTrailingEdge != true ) && ( this.cdbParent.AllowLeadingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = 0;
										this.cdbParent.AddCommandBar( this, true );
									}
									break;
								}
								case DockStyle.Right:
								{
									if( ( ptscreen.X <= rcparent.Left ) && ( ptscreen.X > rcparent.Left-ncbheight )
									&& ( this.bTrailingEdge != true ) && ( this.cdbParent.AllowLeadingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = 0;
										this.cdbParent.AddCommandBar( this, true );
									}
									else if( ( ptscreen.Y >= rcparent.Right ) && ( ptscreen.Y < rcparent.Right+ncbheight )
									&& ( this.bLeadingEdge != true ) && ( this.cdbParent.AllowTrailingEdgeDocking( this ) ) )
									{
										this.cdbParent.RemoveCommandBar( this );
										this.nRCIndex = this.cdbParent.nRCCount;
										this.cdbParent.AddCommandBar( this, true );
									}
									break;
								}
							}
							this.cbController.RecalcLayout( this );
						}

						Rectangle rcdockbarouter = this.cdbParent.RectangleToScreen( this.cdbParent.ClientRectangle );
						rcdockbarouter.Inflate( CommandBar.nDefaultUnitHt, CommandBar.nDefaultUnitHt );
						if( ( rcdockbarouter.Contains( ptscreen ) == false ) && ( this.cdbParent.Contains( this ) == true )
						&& ( this.bDisableFloating == false ) )
						{
							CommandBarStateChangingEventArgs arg = new CommandBarStateChangingEventArgs( CommandBarDockState.Float );
							this.FireCommandBarStateChanging( arg );

							if( !arg.Cancel )
							{
								this.EnterFloatMode( ptscreen );
								this.FireCommandBarStateChanged( EventArgs.Empty );
								this.cdbParent.LayoutDockBar();
							}
						}
					}
					else	// (this.cbarDockState == CommandBarDockState.Float) Floating drag
					{
						// If the cursor is over any of the 4 CommandDockBars, then parent the CommandBar to that dockbar
						// and dispose off the floating frame.
						CommandBarForm frmfloating = this.Parent as CommandBarForm;
						CommandDockBar cdb = null;
						CommandBarDockState border = CommandBarDockState.Float;
						if( this.bDisableDocking != true )
							cdb = this.cbController.GetDockBar( ptscreen );
						if( cdb != null )
						{
							border = cdb.GetDockBorder();
							// Allow docking only if the CommandBar is allowed to dock to this border.
							if( ( (int)this.allowedBorders & (int)border ) != (int)border )
								border = CommandBarDockState.Float;
						}
						if( border != CommandBarDockState.Float )
						{
							Rectangle floatbounds = frmfloating.Bounds;

							CommandBarStateChangingEventArgs arg = new CommandBarStateChangingEventArgs( border );
							this.FireCommandBarStateChanging( arg );
							if( !arg.Cancel )
							{
								this.rcFloat = floatbounds;
								this.cdbParent = cdb;
								this.cbarDockState = border;
								Point ptoffset = this.cdbParent.PointToClient( ptscreen );
								float ptxoffset = (float)CommandBar.ptDeltaOff.X / (float)floatbounds.Width;

								if( ( cdb.Dock == DockStyle.Top ) || ( cdb.Dock == DockStyle.Bottom ) )
								{
									if( cdb.RectangleToScreen( cdb.ClientRectangle ).Top >= ptscreen.Y )
									{
										this.nRCIndex = 0;
									}
									else
									{
										this.nRCIndex = cdb.nRCCount;
									}
									this.nRowOffsetDir = ptoffset.X;
									this.nRowOffsetInDir = ptoffset.X;
								}
								else
								{
									if( cdb.RectangleToScreen( cdb.ClientRectangle ).Left >= ptscreen.X )
									{
										this.nRCIndex = 0;
									}
									else
									{
										this.nRCIndex = cdb.nRCCount;
									}
									this.nRowOffsetDir = ptoffset.Y;
									this.nRowOffsetInDir = ptoffset.Y;
								}

								cdb.AddCommandBar( this, false );

								//this.AdjustRowOffsetsInParentDockBar();

								if( border == CommandBarDockState.Left || border == CommandBarDockState.Right )
								{
									this.bRedockNeeded = true;
									this.bRecalcNeeded = true;

									CommandBarDockState prevSate = this.cbDockStateT;
									this.cbDockStateT = border;

									this.RedockIfNeeded();
									this.RecalcIfNeeded();

									this.cbDockStateT = prevSate;
								}

								this.cbController.RecalcLayout( this );

								if( border == CommandBarDockState.Top || border == CommandBarDockState.Bottom )
								{
									this.cbController.RecalcBorderLayout( CommandBarDockState.Left );
									this.cbController.RecalcBorderLayout( CommandBarDockState.Right );
								}
								else if( border == CommandBarDockState.Left || border == CommandBarDockState.Right )
								{
									this.cbController.RecalcBorderLayout( CommandBarDockState.Top );
									this.cbController.RecalcBorderLayout( CommandBarDockState.Bottom );
								}

								frmfloating.Visible = false;
								frmfloating.Close();
								this.OnCommandBarStateChanged( EventArgs.Empty );

								// The CommandBar bounds will have changed. Set the ptDeltaOff relative to the new bounds.
								CommandBar.ptDeltaOff = new Point( (int)( ptxoffset*this.Width ), CommandBar.ptDeltaOff.Y );
							}
						}
						else
						{
							// Reposition the parent frame at the new location.
							frmfloating.Location = new Point( ptscreen.X - CommandBar.ptDeltaOff.X, ptscreen.Y - CommandBar.ptDeltaOff.Y );
						}
					}
				}
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void HandleMouseUp( MouseButtons button, Point ptscreen )
		{
			if( button == MouseButtons.Left )
			{
				if( CommandBar.bDragging )
				{
					if( this.cbarDockState == CommandBarDockState.Float )
					{
						if( CommandBar.crDefault != null )
						{
							this.Cursor = CommandBar.crDefault;
							CommandBar.crDefault = null;
						}

						// Assign the temp drag information to the main drag variables. Doing this ensures that all intermediate
						// drag states are wiped out and double clicking will redock the CommandBar to the correct previously
						// docked position.
						this.cdbParent = this.cdbParentDrag;
						this.nRowOffsetDir = this.nRowOffsetDrag;
						this.nRowOffsetInDir = this.nRowOffsetDir;
						this.nRCIndex = this.nRCIndexDrag;

						this.cdbParentDrag = null;
						this.nRowOffsetDrag = -1;
						this.nRCIndexDrag = -1;
					}
					else	// Dragged around within the dockbar. Update the RowMarker
					{
						this.cdbParent.SetCurrentRowMarker( this );
					}

					if( this.cdbParent != null )
					{
						foreach( CommandBar cbar in this.Controller.CommandBars )
						{
							if( cbar.cdbParent != null && cbar.cdbParent.Controls.Contains( cbar ) )
								cbar.cdbParent.ReplaceBarLocation( cbar );
						}

						int count = this.cdbParent.GetMaxRowIndex() + 1;
						this.cdbParent.nRCCount = Math.Min( this.cdbParent.nRCCount + 1, count );
					}

					CommandBar.bDragging = false;
					CommandBar.ptMMWorkaround = Point.Empty;
					CommandBar.ptDeltaOff = Point.Empty;

					// Update the designer
					if( this.DesignProcess )
					{
						IDesigner idsgnr = ( this.GetService( typeof( IDesignerHost ) ) as IDesignerHost ).GetDesigner( this );
						if( idsgnr != null )
						{
							ICommandBarDesignerComponentInvoke iinvoke = idsgnr as ICommandBarDesignerComponentInvoke;
							Debug.Assert( iinvoke != null );
							iinvoke.RaiseComponentChanged();
						}
					}
				}

				if( ( this.cbHilight & CBButtons.DropDown ) == CBButtons.DropDown )
				{
					Rectangle rcdropdown = this.DropDownRect;
					if( CommandBar.bHideOnMouseUp == true )
					{
						CommandBar.bHideOnMouseUp = false;
						this.cbHilight = CBButtons.DropDown;
						if( this.IsShowingDropDown() )
						{
							this.HideDropDown();
							this.Update();
							this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
						}
					}
					else if( !this.IsShowingDropDown() )
					{
						this.Invalidate( Rectangle.Inflate( rcdropdown, 2, 2 ) );
					}
				}
				if( ( this.cbHilight & CBButtons.Close ) == CBButtons.Close )
				{
					this.cbHilight = CBButtons.Close;
					this.Invalidate( Rectangle.Inflate( this.CloseButtonRect, 1, 1 ) );
					this.Visible = false;
					this.OnCommandBarUserClosed( EventArgs.Empty );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public void HandleDoubleClick( Point ptscreen )
		{
			if( this.cbarDockState == CommandBarDockState.Float )
			{
				Point ptclient = this.PointToClient( ptscreen );
				// On doubleclick, exit parent frame and dock onto previous dockbar
				if( ( this.bDisableDocking != true ) &&
					( this.DragStartRect.Contains( ptclient ) == true ) && ( this.cbHilight == CBButtons.None ) )
				{
					if( this.cdbParent == null )
					{
						this.cdbParent = this.cbController.GetDockBar( CommandBarDockState.Top );
						if( this.cdbParent == null )
							this.cdbParent = this.cbController.GetDockBar( CommandBarDockState.Bottom );
						if( this.cdbParent == null )
							this.cdbParent = this.cbController.GetDockBar( CommandBarDockState.Left );
						if( this.cdbParent == null )
							this.cdbParent = this.cbController.GetDockBar( CommandBarDockState.Right );
					}

					if( this.cdbParent != null )
					{
						CommandBarDockState border = this.cdbParent.GetDockBorder();
						this.OnCommandBarStateChanging( new CommandBarStateChangingEventArgs( border ) );
						CommandBarForm frmfloating = this.Parent as CommandBarForm;
						this.rcFloat = frmfloating.Bounds;
						this.cbarDockState = border;
						if( ( border == CommandBarDockState.Top ) || ( border == CommandBarDockState.Bottom ) )
						{
							this.Left = this.nRowOffsetInDir;
						}
						else
						{
							this.Top = this.nRowOffsetInDir;
						}

						//When commandbar is floating, it's width(or height) isn't set to it max value
						//as it would be when commandbar will be docked. 
						if( border == CommandBarDockState.Top || border == CommandBarDockState.Bottom )
							this.Size = new Size( this.nMaxLength, this.Height );
						else
							this.Size = new Size( this.Width, this.nMaxLength );

						DockToParent();

						if( border == CommandBarDockState.Left || border == CommandBarDockState.Right )
						{
							this.bRedockNeeded = true;
							this.bRecalcNeeded = true;

							CommandBarDockState prevSate = this.cbDockStateT;
							this.cbDockStateT = border;

							this.RedockIfNeeded();
							this.RecalcIfNeeded();

							this.cbDockStateT = prevSate;
						}

						//this.AdjustRowOffsetsInParentDockBar();
						this.cbController.RecalcLayout( this );

						if( border == CommandBarDockState.Top || border == CommandBarDockState.Bottom )
						{
							this.cbController.RecalcBorderLayout( CommandBarDockState.Left );
							this.cbController.RecalcBorderLayout( CommandBarDockState.Right );
						}
						else if( border == CommandBarDockState.Left || border == CommandBarDockState.Right )
						{
							this.cbController.RecalcBorderLayout( CommandBarDockState.Top );
							this.cbController.RecalcBorderLayout( CommandBarDockState.Bottom );
						}

						this.nRowOffsetInDir = this.nRowOffsetDir;

						frmfloating.Visible = false;
						frmfloating.Close();
						this.OnCommandBarStateChanged( EventArgs.Empty );
					}
				}

				if( CommandBar.bDragging )
				{
					if( CommandBar.crDefault != null )
					{
						this.Cursor = CommandBar.crDefault;
						CommandBar.crDefault = null;
					}
					CommandBar.bDragging = false;
					CommandBar.ptDeltaOff = Point.Empty;

					// Update the designer
					if( this.DesignProcess )
					{
						IDesigner idsgnr = ( this.GetService( typeof( IDesignerHost ) ) as IDesignerHost ).GetDesigner( this );
						if( idsgnr != null )
						{
							ICommandBarDesignerComponentInvoke iinvoke = idsgnr as ICommandBarDesignerComponentInvoke;
							Debug.Assert( iinvoke != null );
							iinvoke.RaiseComponentChanged();
						}
					}
				}
			}
		}

		protected virtual void DockToParent()
		{
			int nRowCount = this.cdbParent.RowsCount;
			bool createNewRow = nRowCount > 0 && nRowCount != this.nRCCountDrag && this.nRCIndexDrag >= 0;

			if( this.Floating )
			{
				ArrayList array = new ArrayList();
				foreach( CommandBar cbar in this.Controller.CommandBars )
				{
					if( cbar.Floating && cbar.cdbParent == this.cdbParent && cbar != this )
					{
						array.Add( cbar );
					}
				}

				createNewRow = DockToParentOffsetRowIndex( createNewRow, array );
			}

			this.cdbParent.AddCommandBar( this, createNewRow );

			if( this.nRCIndexDrag != -1 && !this.cdbParent.bSuspendIndexChanges )
			{
				CommandBar[] cbprevarray = this.cdbParent.GetRowArray( this.nRCIndexDrag );
				if( cbprevarray.Length == 0 || !cbprevarray[0].OccupyFullRow )
				{
					this.nRCIndex = this.nRCIndexDrag;
				}
				else
				{
					this.nRCIndexDrag = -1;
				}
			}

			if( this.cdbParent.nRCCount <= this.nRCIndexDrag )
				this.cdbParent.nRCCount = this.nRCIndexDrag + 1;
		}

		private bool DockToParentOffsetRowIndex( bool createNewRow, ArrayList array )
		{
			int indexOffset = 0;
			Hashtable hash = new Hashtable();
			int rowIdx = this.nRCIndex;
			int rowIndexDrag = this.nRCIndexDrag;

			if( createNewRow )
			{
				foreach( CommandBar cbar in array )
				{
					if( cbar.nRCCountDrag != this.nRCCountDrag )
					{
						createNewRow = false;

						if( cbar.nRCIndex < this.nRCIndex && hash[cbar.nRCIndex] == null )
						{
							hash[cbar.nRCIndex] = cbar.nRCIndex;
							indexOffset--;
						}
					}
				}
			}
			else
			{
				foreach( CommandBar cbar in array )
				{
					if( cbar.nRCIndex == this.nRCIndex &&
						cbar.nRCCountDrag > this.nRCCountDrag &&
                        hash[cbar.nRCIndex] == null )
					{
						createNewRow = true;

						hash[cbar.nRCIndex] = cbar.nRCIndex;
						indexOffset++;
					}
				}				
			}

			this.nRCIndex += indexOffset;
			this.nRCIndexDrag += indexOffset;

			foreach( CommandBar cbar in array )
			{
				if( cbar.nRCIndex == rowIdx && ( cbar.nRCIndexDrag < 0 || cbar.nRCIndexDrag == rowIndexDrag ) )
				{
					cbar.nRCIndex = this.nRCIndex;
					cbar.nRCIndexDrag = this.nRCIndexDrag;
				}
				else
				{
					cbar.nRCIndexDrag -= indexOffset;
				}
			}

			return createNewRow;
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void HandleMouseLeave()
		{
			if( ( this.cbHilight != CBButtons.None ) && !this.IsShowingDropDown() )
			{
				switch( this.cbHilight )
				{
					case CBButtons.DropDown:
					this.cbHilight = CBButtons.None;
					Rectangle rcddbtn = this.DropDownRect;
					if( this.ShouldDrawThemed() || ( this.cbController == null ) || 
							( this.cbController.Style != VisualStyle.Office2003 
							&& this.cbController.Style != VisualStyle.VS2005
							&& this.cbController.Style != VisualStyle.Office2007
                            && this.cbController.Style != VisualStyle.Office2010
							&& this.cbController.Style != VisualStyle.Office2007Outlook ) )
					{
						this.Invalidate( Rectangle.Inflate( rcddbtn, 1, 1 ), false );
					}
					else
					{
						if( cdbParent != null )
						{
							if( ( this.cdbParent.Dock == DockStyle.Top ) || ( this.cdbParent.Dock == DockStyle.Bottom ) )
								this.Invalidate( Rectangle.Inflate( rcddbtn, 3, 1 ), false );
							else
								this.Invalidate( Rectangle.Inflate( rcddbtn, 1, 3 ), false );
						}
					}
					break;
					case CBButtons.Close:
					this.cbHilight = CBButtons.None;
					Rectangle rccaption = this.CaptionRect;
					int nCaptionHt = CommandBar.CaptionHeight;
					Rectangle rcclosebtn = this.CloseButtonRect;
					this.Invalidate( Rectangle.Inflate( rcclosebtn, 1, 1 ), false );
					break;
				}
			}

			if( CommandBar.crDefault != null )
			{
				this.Cursor = CommandBar.crDefault;
				CommandBar.crDefault = null;
			}
			if( CommandBar.bDragging || ( CommandBar.ptDeltaOff != Point.Empty ) )
			{
				CommandBar.bDragging = false;
				CommandBar.ptDeltaOff = Point.Empty;
			}
		}

		#endregion
	}

	/// <summary>
	/// ControlAccessibleObject derived class that implements the Accessibility object for the CommandBar.
	/// </summary>
	public class CommandBarAccessibleObject: Control.ControlAccessibleObject
	{
		#region Class Initialize/Finalize Methods
		public CommandBarAccessibleObject( CommandBar cbar )
			: base( cbar )
		{
		}
		#endregion

		#region Class Properties
		public override AccessibleRole Role
		{
			get { return AccessibleRole.ToolBar; }
		}

		public override string Name
		{
			get { return this.Owner.Text; }
		}

		public override Rectangle Bounds
		{
			get { return this.Owner.RectangleToScreen( this.Owner.ClientRectangle ); }
		}

		public override string Description
		{
			get { return String.Concat( this.Owner.Text, " toolbar" ); }
		}

		public override string Help
		{
			get { return String.Empty; }
		}

		public override AccessibleStates State
		{
			get { return AccessibleStates.Moveable; }
		}

		public override string Value
		{
			get { return this.Owner.Text; }
			set { this.Owner.Text = value; }
		}

		public override AccessibleObject Parent
		{
			get
			{
				return base.Parent;
			}
		}

		#endregion

		#region Class Overrides
		// The AccessibleObjects exposed by the CommandBar's client control are the "child" controls for the CommandBar.
		public override int GetChildCount()
		{
			int childcount = 0;
			if( this.Owner.Controls.Count > 0 )
				childcount = this.Owner.Controls[0].AccessibilityObject.GetChildCount();
			if( !( this.Owner as CommandBar ).bHideDropDown )
				childcount++;
			return childcount;
		}

		// Gets the Accessibility object of the CommandBar identified by index.
		public override AccessibleObject GetChild( int index )
		{
			int count = this.GetChildCount();
			if( ( index >= 0 ) && ( index < count ) )
			{
				if( index == ( count-1 ) )
					return new CommandBarDropDownAccessibleItem( this.Owner as CommandBar );
				else
					return this.Owner.Controls[0].AccessibilityObject.GetChild( index );
			}
			return null;
		}

		public override AccessibleObject GetFocused()
		{
			if( this.Owner.Controls.Count > 0 )
				return this.Owner.Controls[0].AccessibilityObject.GetFocused();
			return base.GetFocused();
		}

		public override AccessibleObject HitTest( int x, int y )
		{
			Point pt = this.Owner.PointToClient( new Point( x, y ) );
			CommandBar cbar = this.Owner as CommandBar;
			if( cbar.DropDownRect.Contains( pt ) == true )
				return new CommandBarDropDownAccessibleItem( this.Owner as CommandBar );
			else if( ( cbar.Controls.Count > 0 ) && ( cbar.RectangleToScreen( cbar.Controls[0].Bounds ).Contains( x, y ) ) )
				return cbar.Controls[0].AccessibilityObject.HitTest( x, y );

			return base.HitTest( x, y );
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			if( ( navdir == AccessibleNavigation.Right ) || ( navdir == AccessibleNavigation.Next ) )
			{
				if( this.Owner.Controls.Count > 0 )
					return this.Owner.Controls[0].AccessibilityObject.GetChild( 0 );
				else
					return new CommandBarDropDownAccessibleItem( this.Owner as CommandBar );
			}
			return base.Navigate( navdir );
		}
		#endregion
	}


	public class CommandBarDropDownAccessibleItem: AccessibleObject
	{
		#region Class Members
		CommandBar cmdBarOwner;
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarDropDownAccessibleItem( CommandBar owner )
		{
			this.cmdBarOwner = owner;
		}
		#endregion

		#region Class Overrides
		public override AccessibleStates State
		{
			get { return AccessibleStates.Focusable; }
		}

		public override AccessibleRole Role
		{
			get { return AccessibleRole.ButtonMenu; }
		}

		public override AccessibleObject Parent
		{
			get { return this.cmdBarOwner.AccessibilityObject; }
		}

		public override string Name
		{
			get { return "Toolbar Options"; }
		}

		public override string DefaultAction
		{
			get { return "Open"; }
		}

		public override Rectangle Bounds
		{
			get { return this.cmdBarOwner.RectangleToScreen( this.cmdBarOwner.DropDownRect ); }
		}

		public override string Description
		{
			get { return String.Concat( this.Name, " ", "control" ); }
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			if( ( navdir == AccessibleNavigation.Left ) || ( navdir == AccessibleNavigation.Previous ) )
			{
				CommandBarAccessibleObject cbaccessible = this.cmdBarOwner.AccessibilityObject as CommandBarAccessibleObject;
				cbaccessible.GetChild( cbaccessible.GetChildCount()-2 );
			}
			return base.Navigate( navdir );
		}

		public override void DoDefaultAction()
		{
			this.Select( AccessibleSelection.TakeFocus );
		}
		#endregion
	}

}
