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

#define FLAT

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Handles the <see cref="SplitterControl.PaneCreated"/> or the <see cref="SplitterControl.PaneClosing"/> events.
	/// </summary>
	public delegate void SplitterPaneEventHandler( object sender, SplitterPaneEventArgs e );

	/// <summary>
	/// Provides event data for the <see cref="SplitterControl.PaneCreated"/>
	/// or the <see cref="SplitterControl.PaneClosing"/> event.
	/// </summary>
	public class SplitterPaneEventArgs: SyncfusionEventArgs
	{
		Control control;
		int row;
		int column;
		Control mainControl;

		internal SplitterPaneEventArgs( Control control, int row, int column, Control mainControl )
		{
			this.control = control;
			this.row = row;
			this.column = column;
			this.mainControl = mainControl;
		}

		/// <summary>
		/// Returns the control inside the specified pane.
		/// </summary>
		[TraceProperty( true )]
		public Control Control
		{
			get
			{
				return control;
			}
		}

		/// <summary>
		/// Returns the zero-based row number of the pane.
		/// </summary>
		[TraceProperty( true )]
		public int Row
		{
			get
			{
				return row;
			}
		}

		/// <summary>
		/// Returns the zero-based column number of the pane.
		/// </summary>
		[TraceProperty( true )]
		public int Column
		{
			get
			{
				return column;
			}
		}

		/// <summary>
		/// Returns a reference to the control at the top-left pane.
		/// </summary>
		[TraceProperty( true )]
		public Control MainControl
		{
			get
			{
				return mainControl;
			}
		}
	}

	/// <summary>
	/// Defines an interface that provides methods for creating and hiding controls for
	/// a splitter pane inside a <see cref="SplitterControl"/>.
	/// </summary>
	public interface ISplitterPaneFactory
	{

		/// <summary>
		/// Creates a new control for the specified splitter pane.
		/// </summary>
		/// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
		/// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
		/// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
		/// <param name="mainControl">The control in the first splitter pane.</param>
		/// <returns>A new instance of a control.</returns>
		Control CreateNewControl( int row, int column, Control mainControl, Control parent );

		/// <summary>
		/// Hides / disposes the control for the specified splitter pane.
		/// </summary>
		/// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
		/// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
		/// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
		/// <param name="control">The control in the splitter pane that should be hidden.</param>
		void DisposeControl( int row, int column, Control control, Control parent );
	}

	/// <summary>
	/// This is the default implementation of the <see cref="ISplitterPaneFactory"/> and manages
	/// creating and hiding of controls for
	/// a splitter pane inside a <see cref="SplitterControl"/>.
	/// </summary>
	/// <remarks>
	/// You can get or replace this object with the <see cref="SplitterControl.SplitterPaneFactory"/> property
	/// of a <see cref="SplitterControl"/> control.</remarks>
	public class SplitterPaneFactory: ISplitterPaneFactory
	{
		/// <summary>
		/// Creates a new control for the specified splitter pane.
		/// </summary>
		/// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
		/// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
		/// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
		/// <param name="mainControl">The control in the first splitter pane.</param>
		/// <returns>A new instance of a control.</returns>
		public Control CreateNewControl( int row, int column, Control mainControl, Control parent )
		{
			Control newControl = null;
			ISplitterPaneSupport sps = mainControl as ISplitterPaneSupport;
			if( sps == null || !sps.FillSplitterPane )
				return null;
			ICreateNewWindow createNewWindow = mainControl as ICreateNewWindow;
			if( createNewWindow != null )
				newControl = createNewWindow.CreateNewControl( parent, row, column );
			return newControl;
		}

		/// <summary>
		/// Hides / disposes the control for the specified splitter pane.
		/// </summary>
		/// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
		/// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
		/// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
		/// <param name="control">The control in the splitter pane that should be hidden.</param>
		public void DisposeControl( int row, int column, Control control, Control parent )
		{
		}

	}

	[Syncfusion.Documentation.DocumentationExclude()]
	internal class SplitterInfo
	{
		public SplitterInfo( Control splitterControl )
		{
			this.splitterControl = splitterControl;
		}

		public void Dispose()
		{
			for( int row = 0; row < 2; row++ )
			{
				for( int col = 0; col < 2; col++ )
				{
					if( splitterPanes != null && splitterPanes[row, col] != null )
						splitterPanes[row, col].Dispose();
				}
			}
		}

		internal Control splitterControl;

		internal Rectangle horizontalBarBounds;
		internal Rectangle verticalBarBounds;
		internal Rectangle innerBounds;
		internal Rectangle sizeGripBounds;

		internal Rectangle hLeftBarBounds;
		internal Rectangle hSplitBarBounds;
		internal Rectangle hRightBarBounds;

		internal Rectangle vTopBarBounds;
		internal Rectangle vSplitBarBounds;
		internal Rectangle vBottomBarBounds;

		internal Control[,] splitterPanes = new Control[2, 2];

		internal int activeRow;
		internal int activeColumn;

		// Splitter bar

		internal SplitterLayout splitterLayout = null;

		internal DynamicSplitBars splitBars = DynamicSplitBars.Both;
	}


	/// <summary>
	/// A splitter control provides support for dynamic splitting of the viewable area.
	/// </summary>
	/// <remarks>
	/// The controls shown inside the splitter control must implement
	/// the <see cref="ISplitterPaneSupport"/> interface. Additionally, these controls need to have built-in logic
	/// that allows displaying one set of data in different views. <para/>
	/// The <see cref="IScrollBarWrapperContainer"/> interface should be implemented if scrollbars of the child
	/// pane should be shared with the parent splitter frame. <para/>
	/// The controls in the pane should also implement <see cref="ICreateNewWindow"/>. This allows the control
	/// to create new panes and initialize them when an additional row or column is opened in the splitter control.
	/// If a control does not implement <see cref="ICreateNewWindow"/>, the splitter control will call the
	/// <see cref="ISplitterPaneFactory.CreateNewControl"/> of the <see cref="SplitterPaneFactory"/>.<para/>
	/// Essential Grid's GridControlBase is a control that provides all of this logic and can be dropped into a
	/// splitter control and be dynamically split by an end user. <para/>
	/// <see cref="ScrollControl"/> also provides part of the logic to be used inside a splitter frame, but
	/// you still need to implement the logic for displaying one set of data in different views in your derived controls.
	/// <para/>
	/// See the SplitterControlDemo and TabBarSplitterControlDemo samples for examples on how to use <see cref="SplitterControl"/>
	/// in a form.
	/// </remarks>
	[ToolboxItem( true )]
	[System.Drawing.ToolboxBitmap( typeof( Syncfusion.Windows.Forms.SplitterControl ), "ToolboxIcons.SplitterControl.bmp" )]
	[Description( "A splitter control provides support for dynamic splitting of the viewable area." )]
	public class SplitterControl:
		ContainerControl,
		IControlToolTipProvider,
		IInternalSplitterParent,
		IScrollBarFrame,
		IDynamicSplitterFrame,
		IThemedControl,
		IContainerControl,
        IVisualStyle 
	{
		// Events
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
		[Description( "Occurs when the window receives a WM_CANCELMODE message." )]
		public event EventHandler CancelMode;

		/// <summary>
		/// Occurs when the <see cref="SplitBars"/> property has changed.
		/// </summary>
		[Description( "Occurs when the SplitBars property has changed." )]
		public event EventHandler SplitBarsChanged;

		/// <summary>
		/// Occurs when the vertical splitter position has changed.
		/// </summary>
		[Description( "Occurs when the vertical splitter position has changed." )]
		public event EventHandler VSplitPosChanged;

		/// <summary>
		/// Occurs when the horizontal splitter position has changed.
		/// </summary>
		[Description( "Occurs when the horizontal splitter position has changed." )]
		public event EventHandler HSplitPosChanged;

		/// <summary>
		/// Occurs when the splitter layout has changed.
		/// </summary>
		[Description( "Occurs when the splitter layout has changed." )]
		public event EventHandler SplitterLayoutChanged;

		/// <summary>
		/// Occurs when the <see cref="ButtonLook"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ButtonLook property has changed." )]
		public event EventHandler ButtonLookChanged;

		/// <summary>
		/// Occurs when the <see cref="ScrollBarAppearance"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ScrollBarAppearance property has changed." )]
		public event EventHandler ScrollBarAppearanceChanged;

		/// <summary>
		/// Occurs when the <see cref="Office2007ColorScheme"/> property has changed.
		/// </summary>
		[Description( "Occurs when the Office2007ColorScheme property has changed." )]
		public event EventHandler Office2007ScrollBarsColorSchemeChanged;

		/// <summary>
		/// Occurs when the <see cref="ScrollBarColor"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ScrollBarColor property has changed." )]
		public event EventHandler ScrollBarColorChanged;

		/// <summary>
		/// Occurs when the <see cref="ShowToolTips"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ShowToolTips property has changed." )]
		public event EventHandler ShowToolTipsChanged;

		/// <summary>
		/// Occurs when the <see cref="SupportsFlatScrollBars"/> property has changed.
		/// </summary>
		[Description( "Occurs when the SupportsFlatScrollBars property has changed." )]
		public event EventHandler SupportsFlatScrollBarsChanged;

		/// <summary>
		/// Occurs when the <see cref="Office2007ScrollBars"/> property has changed.
		/// </summary>
		[Description( "Occurs when the SupportsOffice2007FlatScrollBars property has changed." )]
		public event EventHandler Office2007ScrollBarsChanged;

		/// <summary>
		/// Occurs when the ThemesEnabled property changes.
		/// </summary>
		[Description( "Occurs when the ThemesEnabled property has changed." )]
		public event EventHandler ThemeChanged;


		// Fields
		internal ControlToolTip toolTipProvider = null;
		private bool showToolTips = false;
		private ButtonLook buttonLook = Syncfusion.Windows.Forms.ButtonLook.Flat;
		bool supportsFlatScrollBars = false;
		bool office2007ScrollBars = false;
		private bool themesEnabled = false;

		internal SplitterInfo splitterInfo;
		internal SplitterInfo oSplitterInfo;
		internal ISplitterPaneFactory splitterPaneFactory = new SplitterPaneFactory();
		private Cursor overrideSplitCursor = null;
		internal Control[] vScrollBars = new Control[2];
		internal Control[] hScrollBars = new Control[2];
		internal InternalSplitter horizontalSplitterBar = null;
		internal InternalSplitter verticalSplitterBar = null;
		private ThemedWindowDrawing themedDrawing = null;

#if FLAT
		private FlatScrollBarStyle scrollBarAppearance = FlatScrollBarStyle.Flat;
#endif

		private Office2007ColorScheme office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;

		private Color scrollBarColor = SystemColors.ScrollBar;

		// Temporary variables used for dragging splitterbar.
		Rectangle oldHSplitRect = Rectangle.Empty;
		Rectangle oldVSplitRect = Rectangle.Empty;
		private int relativeSplitBarWidth;
		private int relativeSplitBarHeight;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);



		/// <summary>
		/// Initializes a new splitter control.
		/// </summary>
		public SplitterControl()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SplitterControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			oSplitterInfo = splitterInfo = new SplitterInfo( this );
			horizontalSplitterBar = new InternalSplitter( this, InternalSplitterKind.HorizontalBar );
			verticalSplitterBar = new InternalSplitter( this, InternalSplitterKind.VerticalBar );
			SetStyle( ControlStyles.ResizeRedraw, false );
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | WhidbeyCompatibleControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.Selectable, true );
			SetStyle( ControlStyles.ContainerControl, true );
#if FLAT
			ButtonLook = ButtonLook.Flat;
#else
			ButtonLook = ButtonLook.Normal;
#endif
			if( XPThemes.IsThemedOS )
			{
				if( themedDrawing == null )
					themedDrawing = new ThemedWindowDrawing();
			}
            CTRLSIZE = this.Size;

		}
        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        } 
		/// <summary>
		/// Returns the number of visible row panes.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int RowCount
		{
			get
			{
				return ( VSplitPos != 100 ) ? 2 : 1;
			}
		}

		/// <summary>
		/// Returns the number of visible column panes.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int ColumnCount
		{
			get
			{
				return ( HSplitPos != 100 ) ? 2 : 1;
			}
		}

		/// <summary>
		/// Indicates whether the rows were split at the given y coordinate.
		/// </summary>
		/// <param name="cy">The vertical position in percentages of the splitter control's height.</param>
		/// <returns>True if rows were split successfully; False if they were already split or the operation aborted.</returns>
		public bool SplitRow( int cy )
		{
			if( cy >= splitterInfo.innerBounds.Height || cy <= 0 )
				return false;

			CurrentLayout._vSplitPos = cy * 100 / ( splitterInfo.verticalBarBounds.Height - 14 );
			PerformLayout();
			return true;
		}

		/// <summary>
		/// Indicates whether the columns were split horizontally at the specified x coordinate.
		/// </summary>
		/// <param name="cx">The horizontal position in in percentages of the splitter control's width.</param>
		/// <returns>True if columns were split successfully; False if they were already split or the operation aborted.</returns>
		public bool SplitColumn( int cx )
		{
			if( cx >= splitterInfo.innerBounds.Width || cx < 0 )
				return false;

			CurrentLayout._hSplitPos = cx * 100 / ( splitterInfo.horizontalBarBounds.Width - 14 );
			PerformLayout();
			return true;
		}

		/// <summary>
		/// Deletes the splitter panes at the specified row.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		public void DeleteRow( int row )
		{
			if( RowCount == 2 && row < 2 )
			{
				SuspendLayout();

				for( int column = 0; column <= 1; column++ )
				{
					DisposePane( row, column );
					if( row == 0 )
					{
						splitterInfo.splitterPanes[0, column] = splitterInfo.splitterPanes[1, column];
						splitterInfo.splitterPanes[1, column] = null;
					}
				}
				if( row == 0 )
					CopyScrollBarSettings( vScrollBars[0], vScrollBars[1] );
				CurrentLayout._vSplitPos = 100;				
				splitterInfo.activeRow = 0;
				relativeSplitBarHeight = 0;

				Control pane = GetPane( splitterInfo.activeRow, splitterInfo.activeColumn );
				( (IContainerControl)splitterInfo.splitterControl ).ActivateControl( pane );

				ResumeLayout();
				Invalidate();
			}
		}

		/// <summary>
		/// Deletes the splitter panes at the specified column.
		/// </summary>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public void DeleteColumn( int column )
		{
			if( ColumnCount == 2 && column < 2 )
			{
				SuspendLayout();

				for( int row = 0; row <= 1; row++ )
				{
					DisposePane( row, column );
					if( column == 0 )
					{
						splitterInfo.splitterPanes[row, 0] = splitterInfo.splitterPanes[row, 1];
						splitterInfo.splitterPanes[row, 1] = null;
					}
				}
				if( column == 0 )
					CopyScrollBarSettings( hScrollBars[0], hScrollBars[1] );
				CurrentLayout._hSplitPos = 100;
				splitterInfo.activeColumn = 0;
				relativeSplitBarWidth = 0;

				Control pane = GetPane( splitterInfo.activeRow, splitterInfo.activeColumn );
				( (IContainerControl)splitterInfo.splitterControl ).ActivateControl( pane );

				ResumeLayout();
				Invalidate();
			}
		}

		void CopyScrollBarSettings( Control target, Control source )
		{
			ScrollBarAdapter sbSource = new ScrollBarAdapter( GetContainedScrollBar( source ) );
			ScrollBarAdapter sbTarget = new ScrollBarAdapter( GetContainedScrollBar( target ) );
			try
			{
				if( sbTarget != null && sbSource != null )
				{
					sbTarget.Minimum = sbSource.Minimum;
					sbTarget.Maximum = sbSource.Maximum;
					sbTarget.SmallChange = sbSource.SmallChange;
					sbTarget.LargeChange = sbSource.LargeChange;
					sbTarget.Value = sbSource.Value;
					sbTarget.Enabled = sbSource.Enabled;
				}
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		/// <summary>
		/// Returns the splitter pane at the specified row and column. If there is no pane found at the
		/// specified row and column a pane will be created on demand with a call to <see cref="OnCreateNewControl"/>.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <returns>The control at the pane.</returns>
		public virtual Control GetPane( int row, int column )
		{
			row = row % 2;
			column = column % 2;

			if( row == 0 && column == 0 && splitterInfo.splitterPanes[0, 0] == null && splitterInfo.splitterControl.Controls.Count > 0 )
			{
				foreach( Control control in splitterInfo.splitterControl.Controls )
				{
					if( !( control is ScrollBar || control is IScrollBarContainer || control is TabBarPage || control is ScrollBarCustomDraw ) )
					{
						splitterInfo.splitterPanes[row, column] = control;
						control.GotFocus += new EventHandler( OnChildControlGotFocus );
						control.Enter += new EventHandler( OnChildControlEnter );
                        if (this.Parent == null)
                            control.Dock = DockStyle.None;
						break;
					}
				}
			}

			if( splitterInfo.splitterPanes[row, column] == null )
			{
				splitterInfo.splitterControl.SuspendLayout();
				Control sourcePane = null;
				if( column > 0 )
				{
					sourcePane = splitterInfo.splitterPanes[row, 0];
				}
				else if( row > 0 )
				{
					sourcePane = splitterInfo.splitterPanes[0, column];
				}
				Control control = OnCreateNewControl( row, column, sourcePane );
				if( control != null )
				{
					splitterInfo.splitterPanes[row, column] = control;
					//splitterInfo.splitterControl.Controls.Add(control);
					control.GotFocus += new EventHandler( OnChildControlGotFocus );
					control.Enter += new EventHandler( OnChildControlEnter );
				}
				splitterInfo.splitterControl.ResumeLayout( false );
			}

			return splitterInfo.splitterPanes[row, column];
		}

		/// <summary>
		/// Returns the splitter pane at the specified row and column. If there is no pane found at the
		/// specified row and column a null reference will be returned.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <returns>The control at the pane.</returns>
		public virtual Control GetPaneInternal( int row, int column )
		{
			return splitterInfo.splitterPanes[row, column];
		}

		/// <summary>
		/// Disposes the specified pane.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public virtual void DisposePane( int row, int column )
		{
			Control pane = splitterInfo.splitterPanes[row, column];
			if( pane != null )
			{
				// Work around DotNet 1.1 Framework issue with locking up application after closing a pane.
				// Update: The problem seems to have gone away and instead the workaround causes issues now.
				// Commented it out for version 3.2.0.0 when 1.1 sp installed.
				//                Form f = this.FindForm();
				//                if (f != null && f.ActiveControl != null)
				//                {
				//					if (pane == f.ActiveControl || pane.Contains(f.ActiveControl) || f.ActiveControl.Contains(pane))
				//					{
				//						f.ActiveControl = null;
				//					}
				//                }
				//
				//                if (this.ActivePane == pane)
				//					this.ActiveControl = null;

				ISplitterPaneSupport iPane = pane as ISplitterPaneSupport;
				if( iPane != null )
					iPane.PaneClosing();
#if DEBUG

				if( Switches.Workbook.TraceVerbose )

					TraceUtil.TraceCurrentMethodInfo( this, row, column );
#else

				;
#endif

				pane.GotFocus -= new EventHandler( OnChildControlGotFocus );
				pane.Enter -= new EventHandler( OnChildControlEnter );

				this.OnPaneClosing( new SplitterPaneEventArgs( pane, row, column, splitterInfo.splitterControl ) );

				splitterPaneFactory.DisposeControl( row, column, pane, splitterInfo.splitterControl );
				splitterInfo.splitterPanes[row, column] = null;
				this.SuspendLayout();
				// Note: If you do not want Dispose to be called on pane control, user has the option
				// to set pane.Parent = null in OnPaneClosing event or DisposeControl handler.
				// Then the Remove call below will do nothing.
				if( pane.Parent != null )
				{
					// Fix for defect 1955: Removing the active pane causes ActivateControl be called which
					// then causes in the GetActiveControlFixPane method the pane to be recreated.
					// GetActiveControlFixPane will check disposePane and in that case simply return null.
					disposePane = pane;
					this.splitterInfo.splitterControl.Controls.Remove( pane );
					disposePane = null;
				}
				if( iPane != null )
					iPane.PaneClosed();
				this.ResumeLayout( false );
			}
		}

		Control disposePane = null;

		void OnChildControlGotFocus( object sender, EventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( "--- BEGIN ---", this, sender );
#else
			;
#endif

			Control c = sender as Control;
			if( this.inActivateControl == 0 && !validatingFailed && c != null && ( this.ActiveControl == c || c.Contains( this.ActiveControl ) ) )
			{
				int row, column;
				if( FindPane( c, out row, out column ) )
				{
					splitterInfo.activeRow = row;
					splitterInfo.activeColumn = column;
				}
			}
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( "--- END ---", this, sender );
#else
			;
#endif

		}

		void OnChildControlEnter( object sender, EventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( "--- BEGIN ---", this, sender );
#else
			;
#endif

			Control c = sender as Control;

			int row, column;
			if( this.inActivateControl == 0 && FindPane( c, out row, out column ) )
			{
				splitterInfo.activeRow = row;
				splitterInfo.activeColumn = column;
			}
#if DEBUG

			if( Switches.Workbook.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( "--- END ---", this, sender );
#else

			;
#endif

		}

		/// <summary>
		/// Creates a new control for the specified splitter pane.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <param name="mainControl">The control in the first splitter pane.</param>
		/// <returns>A new instance of the control.</returns>
		protected virtual Control OnCreateNewControl( int row, int column, Control mainControl )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, row, column );
#else
			;
#endif

			SuspendLayout();
			splitterInfo.splitterControl.SuspendLayout();
			Control control = splitterPaneFactory.CreateNewControl( row, column, mainControl, splitterInfo.splitterControl );

			try
			{
				if( control == null )
					return null;

				control.Visible = false;

				ScrollControl sc1 = mainControl as ScrollControl;
				ScrollControl sc2 = control as ScrollControl;
				if( sc1 != null && sc2 != null )
				{
					sc2.VScroll = false;
					sc2.HScroll = false;
					sc2.AccelerateScrolling = sc1.AccelerateScrolling;
					sc2.AllowIncreaseSmallChange = sc1.AllowIncreaseSmallChange;
				}

				IScrollBarWrapperContainer scMain = mainControl as IScrollBarWrapperContainer;
				IScrollBarWrapperContainer scTarget = control as IScrollBarWrapperContainer;
				if( scTarget != null )
				{
					if( scMain != null )
					{
						scTarget.HScrollBar.SupportsScrollTips = scMain.HScrollBar.SupportsScrollTips;
						scTarget.HScrollBar.SupportsThumbTrack = scMain.HScrollBar.SupportsThumbTrack;
						scTarget.VScrollBar.SupportsScrollTips = scMain.VScrollBar.SupportsScrollTips;
						scTarget.VScrollBar.SupportsThumbTrack = scMain.VScrollBar.SupportsThumbTrack;
					}

					if( row > 0 )
					{
						IScrollBarWrapperContainer scRow = splitterInfo.splitterPanes[0, column] as IScrollBarWrapperContainer;
						if( scRow != null )
							scRow.HScrollBar.CopyTo( scTarget.HScrollBar );
					}
					if( column > 0 )
					{
						IScrollBarWrapperContainer scColumn = splitterInfo.splitterPanes[row, 0] as IScrollBarWrapperContainer;
						if( scColumn != null )
							scColumn.VScrollBar.CopyTo( scTarget.VScrollBar );
					}
				}

				control.GotFocus += new EventHandler( this.OnChildControlGotFocus );
				control.Enter += new EventHandler( this.OnChildControlEnter );
				control.AllowDrop = mainControl.AllowDrop;
				control.CausesValidation = mainControl.CausesValidation;

				ISupportIntelliMouse im1 = mainControl as ISupportIntelliMouse;
				ISupportIntelliMouse im2 = control as ISupportIntelliMouse;
				if( im1 != null && im2 != null )
					im2.EnableIntelliMouse = im1.EnableIntelliMouse;

				OnPaneCreated( new SplitterPaneEventArgs( control, row, column, mainControl ) );
			}
			finally
			{
				splitterInfo.splitterControl.ResumeLayout( false );
				ResumeLayout( false );
			}
			return control;
		}

		/// <override/>
		protected override void OnControlAdded( ControlEventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( e.Control, this );
#else
			;
#endif

			_ControlAdded( e );

			SuspendLayout();
			IThemedControl themedControl = e.Control as IThemedControl;
			if( themedControl != null )
				themedControl.ThemesEnabled = this.ThemesEnabled;
			ResumeLayout( false );
		}

		/// <summary>
		/// Occurs after the control to be displayed in a new pane has been created. Use this
		/// event to implement additional initialization for the new control.
		/// </summary>
		/// <remarks>
		/// PaneCreated is an ideal hook to add handler for events in the new control.
		/// </remarks>
		[Description( "Occurs after the control to be displayed in a new pane has been created." )]
		public event SplitterPaneEventHandler PaneCreated;

		/// <summary>
		/// Raises the <see cref="PaneCreated"/> event.
		/// </summary>
		/// <param name="e">A <see cref="SplitterPaneEventArgs" /> that contains the event data.</param>
		protected virtual void OnPaneCreated( SplitterPaneEventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, e );
#else
			;
#endif

			if( PaneCreated != null )
				PaneCreated( this, e );
		}

		/// <summary>
		/// Occurs after a row or column is hidden and before the control that is displayed in the pane
		/// is disposed. Use this event to implement additional clean up for the control before <see cref="DisposePane"/>
		/// is called.
		/// </summary>
		/// <remarks>
		/// PaneClosing is an ideal hook to unwire event handlers from the control.
		/// </remarks>
		[Description( "Occurs after a row or column is hidden and before the control that is displayed in the pane is disposed." )]
		public event SplitterPaneEventHandler PaneClosing;

		/// <summary>
		/// Raises the <see cref="PaneClosing"/> event.
		/// </summary>
		/// <param name="e">A <see cref="SplitterPaneEventArgs" /> that contains the event data.</param>
		protected virtual void OnPaneClosing( SplitterPaneEventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, e );
#else
			;
#endif

			if( PaneClosing != null )
				PaneClosing( this, e );
		}


		/// <summary>
		/// Returns the row and column indices for a child pane.
		/// </summary>
		/// <param name="control">The control to search for.</param>
		/// <param name="row">A placeholder where the row is returned.</param>
		/// <param name="column">A placeholder where the column is returned.</param>
		/// <returns>True if the control is a pane; False if the control was not a child pane.</returns>
		public bool FindPane( Control control, out int row, out int column )
		{
			if( control != null )
			{
                for (row = 0; row <= 1; row++)
                    for (column = 0; column <= 1; column++)
                        if (splitterInfo != null && splitterInfo.splitterPanes[row, column] == control)
                            return true;
			}

			row = -1;
			column = -1;
			return false;
		}

		/// <summary>
		/// Gets or sets the active pane in the splitter control.
		/// </summary>
		[
		Browsable( false ),
		Description( @"The currently active control." ),
		Category( @"Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public Control ActivePane
		{
			set
			{
				int row, column;
				if( FindPane( value, out row, out column ) )
				{
					SetActivePane( row, column );
				}
			}
			get
			{
				return GetPane( splitterInfo.activeRow, splitterInfo.activeColumn );
			}
		}

		/// <summary>
		/// Sets the active pane in the splitter control specified by row and column.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public void SetActivePane( int row, int column )
		{
			if( row != splitterInfo.activeRow || column != splitterInfo.activeColumn )
			{
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( this, row, column );
#else
				;
#endif

				IContainerControl cc = ( (IContainerControl)splitterInfo.splitterControl );
				Control pane = GetPane( row, column );
				if( cc.ActivateControl( pane ) )
				{
					splitterInfo.activeRow = row;
					splitterInfo.activeColumn = column;
				}
				else
					ActivePane.Focus();
			}
		}

		/// <summary>
		/// Indicates whether there is a next or previous pane that can be activated.
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		/// <returns>True if activating next or previous pane is good; False if already at last or first pane.</returns>
		public bool CanActivateNext( bool prev )
		{
			if( !prev )
				return splitterInfo.activeRow < RowCount - 1 || splitterInfo.activeRow < ColumnCount - 1;
			else
				return splitterInfo.activeRow > 0 || splitterInfo.activeRow > 0;
		}

		/// <summary>
		/// Returns the default size of the control.
		/// </summary>
		protected override Size DefaultSize
		{
			get
			{
				return new Size( 130, 80 );
			}
		}

		/// <override/>
		protected override bool ProcessDialogKey( Keys key )
		{
			if( IsDisposed )
				return true;
#if DEBUG

			if( Switches.Workbook.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( this, key );
#else

			;
#endif


			Keys keyCode = key & Keys.KeyCode;
			bool shiftKeyDown = ( Control.ModifierKeys & Keys.Shift ) != Keys.None;
			bool menuKeyDown = ( Control.ModifierKeys & Keys.Menu ) != Keys.None;

			switch( keyCode )
			{
				case Keys.F6:
				this.ActivateNext( shiftKeyDown );
                break;
			}
			return base.ProcessDialogKey( key );
		}

		/// <summary>
		/// Activates the next or previous pane.
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		public void ActivateNext( bool prev )
		{
			int row = splitterInfo.activeRow;
			int column = splitterInfo.activeColumn;

			if( !prev )
			{
				if( column < ColumnCount - 1 )
					column++;
				else if( row < RowCount - 1 )
				{
					row++;
					column = 0;
				}
				else
					row = column = 0;
			}
			else
			{
				if( column > 0 )
					column--;
				else if( row > 0 )
				{
					row--;
					column = ColumnCount - 1;
				}
				else
				{
					column = ColumnCount - 1;
					row = RowCount - 1;
				}
			}
			SetActivePane( row, column );
		}


		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                foreach( Control ctrl in this.Controls )
                {
                    ctrl.GotFocus -= new EventHandler(ChildGotFocus);
                    ctrl.LostFocus -= new EventHandler(ChildLostFocus);
                }
				if( horizontalSplitterBar != null )
				{
					horizontalSplitterBar.Dispose();
					horizontalSplitterBar = null;
				}

				if( verticalSplitterBar != null )
				{
					verticalSplitterBar.Dispose();
					verticalSplitterBar = null;
				}

				if( toolTipProvider != null )
				{
					toolTipProvider.Destroy();
					toolTipProvider = null;
				}

				if( themedDrawing != null )
				{
					themedDrawing.Dispose();
					themedDrawing = null;
				}
				if( this._themedScrollBarDrawing != null )
				{
					this._themedScrollBarDrawing.Dispose();
					this._themedScrollBarDrawing = null;
				}
                if( splitterInfo != null )
                {
                    splitterInfo.Dispose() ;
                    splitterInfo = null;
                }
                if( oSplitterInfo != null )
                {
                    oSplitterInfo.Dispose();
                    oSplitterInfo = null;
                }
                hScrollBars = null;
                vScrollBars = null;
			}
			base.Dispose( disposing );
		}

		/// <override/>
		protected override/*Control*/ CreateParams CreateParams
		{
			[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
			get
			{
				System.Windows.Forms.CreateParams cp = base.CreateParams;
				cp.Style |= (int)WS.CLIPCHILDREN;
				switch( this.borderStyle )
				{
					case BorderStyle.Fixed3D:
					cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
					break;
					case BorderStyle.FixedSingle:
					cp.Style |= 0x800000; // WS_BORDER
					break;
				}

				if( this.RightToLeft == RightToLeft.Yes )
					cp.ExStyle = ( cp.ExStyle | NativeMethods.WS_EX_RTLREADING );
				else
					cp.ExStyle = ( cp.ExStyle & ~NativeMethods.WS_EX_RTLREADING );

				return cp;
			}
		}

		BorderStyle borderStyle = BorderStyle.Fixed3D;

		/// <summary>
		/// Gets or sets the border style of the Splitter Control.
		/// </summary>
		[
		Category( "Appearance" ),
		DefaultValue( BorderStyle.Fixed3D ),
		Description( "The border style of the control." )
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
					if( !Enum.IsDefined( typeof( System.Windows.Forms.BorderStyle ), value ) )
						throw new InvalidEnumArgumentException( "value", ( (int)( value ) ), typeof( BorderStyle ) );
					this.borderStyle = value;
					UpdateStyles();
				}
			}
		}


		/// <summary>
		/// Overridden method.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string _paneDesc = Name + String.Format( "P{0}.{1}/{2}.{3} ",
				this.splitterInfo.activeRow, this.splitterInfo.activeColumn,
				this.RowCount, this.ColumnCount );

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

			if( this.validatingFailed )
				_paneDesc += "Vfa";

			if( this.CanFocus )
				_paneDesc += "Cfo";

			_paneDesc += ": ";
			_paneDesc +=
				( this.ActiveControl == null || this.ActiveControl.IsDisposed )
				? "null"
				: this.ActiveControl.ToString();

			return _paneDesc;
			//			return String.Concat(new String[] {
			//					"SplitterControl: {",
			//					"Name: ", Name, ", ",
			//					"Row: ", this.splitterInfo.activeRow.ToString(), ", ",
			//					"Col: ", this.splitterInfo.activeColumn.ToString(), ", ",
			//					"Rows: ", RowCount.ToString(), ", ",
			//					"Columns: ", ColumnCount.ToString(), ",",
			//					"ActiveControl: ", (this.ActiveControl == null || this.ActiveControl.IsDisposed) ? "null" : this.ActiveControl.ToString(), "} "
			//				});
		}

		/// <summary>
		/// Returns the scrollbar for the specified control.
		/// </summary>
		/// <param name="c">The control for which you want to get the scrollbar.</param>
		/// <returns></returns>
		internal static Control GetContainedScrollBar( Control c )
		{
			if( c is ScrollBar )
				return c;
			else if( c is ScrollBarCustomDraw )
				return c;
			else if( c is IScrollBarContainer )
				return ( (IScrollBarContainer)c ).ScrollBar;
			else
				return null;
		}

		/// <summary>
		/// Indicates whether the scrollbar belongs to the active pane.
		/// </summary>
		/// <param name="control">The control associated with the scrollbar.</param>
		/// <param name="sbType">Specifies the vertical or horizontal scrollbar.</param>
		/// <returns>True if active; False otherwise.</returns>
		/// <remarks>
		///
		/// </remarks>
		/// <example>
		/// ScrollControl checks IsActive to find out if it is the target of a HScroll event.
		/// <code lang="C#">
		/// 		protected virtual void OnHScroll(object sender, ScrollEventArgs se)
		/// 		{
		/// 			try
		/// 			{
		/// 				IScrollBarFrame sbf = GetScrollBarFrameOfComponent(this);
		/// 				if (sbf != null &amp;&amp; !sbf.IsActive(this, ScrollBars.Horizontal))
		/// 					return;
		///
		///					...
		/// </code>
		/// </example>
		public bool IsActive( Control control, ScrollBars sbType )
		{
			int row, column;
			if( this.FindPane( control, out row, out column ) )
			{
				if( sbType == ScrollBars.Vertical )
					return column == this.splitterInfo.activeColumn;
				else
					return row == this.splitterInfo.activeRow;
			}
			return false;
		}

		/// <summary>
		/// Returns the horizontal scrollbar associated with the control.
		/// </summary>
		/// <param name="control">A child pane.</param>
		/// <returns>The scrollbar for the pane.</returns>
		public virtual Control GetHScrollBar( Control control )
		{
			int row, column;
			if( !FindPane( control, out row, out column ) )
				column = 0;
            if (hScrollBars != null)
                return GetContainedScrollBar(hScrollBars[column]);
            else
                return null;
		}

		/// <summary>
		/// Returns the vertical scrollbar associated with the control.
		/// </summary>
		/// <param name="control">A child pane.</param>
		/// <returns>The scrollbar for the pane.</returns>
		public virtual Control GetVScrollBar( Control control )
		{
			int row, column;
			if( !FindPane( control, out row, out column ) )
				row = 0;
            if (vScrollBars != null)
                return GetContainedScrollBar(vScrollBars[row]);
            else
                return null;
		}
		#region WndProc
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSetCursor( ref Message m )
		{
			if( overrideSplitCursor != null )
			{
				Cursor.Current = overrideSplitCursor;
			}
			else
				Cursor.Current = Cursor;
		}

		/// <summary>
		/// Raises the <see cref="CancelMode"/> event.
		/// </summary>
		/// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
		protected virtual void OnCancelMode( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, e );
#else
			;
#endif

			if( CancelMode != null )
				CancelMode( this, e );

			if( this.horizontalSplitterBar != null )
				this.horizontalSplitterBar.CancelMode();
			if( this.verticalSplitterBar != null )
				this.verticalSplitterBar.CancelMode();

			if( toolTipProvider != null )
				toolTipProvider.ActivateToolTip();

			for( int n = 0; n < 5; n++ )
				ResumeLayout( false );
		}

		/// <summary>
		///     Handles the WM_SETCURSOR message.
		/// </summary>
		/// <internalonly/>
		private void WmSetCursor( ref Message m )
		{

			// Accessing through the Handle property has side effects that break this
			// logic. You must use InternalHandle.
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

		private void WmNcPaint( ref Message msg )
		{
			bool themed = XPThemes.IsThemedOS && ThemesEnabled;
			if( themed && themedDrawing != null )
				themedDrawing.DrawThemedBorderColor( this, ref msg );
			base.WndProc( ref msg );
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message msg )
		{
			if( DesignMode )
				base.WndProc( ref msg );
			else switch( msg.Msg )
				{
					case NativeMethods.WM_NCPAINT:
					this.WmNcPaint( ref msg );
					break;
					case NativeMethods.WM_SETCURSOR:
					WmSetCursor( ref msg );
					break;
					case 31/*WM_CANCELMODE*/:
					OnCancelMode( EventArgs.Empty );
					break;
					default:
					base.WndProc( ref msg );
					break;
				}

		}
		#endregion
		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ThemesEnabled );
#else
			;
#endif

			if( this.ThemeChanged != null )
			{
				try
				{
					this.ThemeChanged( this, e );
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
						throw;
				}
			}
			foreach( Control control in Controls )
			{
				IThemedControl themedControl = control as IThemedControl;
				if( themedControl != null )
					themedControl.ThemesEnabled = this.ThemesEnabled;
			}
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		[Description( "Indicates whether themes are enabled for this control." ), DefaultValue(false)]
		public virtual bool ThemesEnabled
		{
			get { return this.themesEnabled; }
			set
			{
				if( this.themesEnabled != value )
				{
					this.themesEnabled = value;
					this.OnThemeChanged( EventArgs.Empty );
				}
			}
		}


		#region ToolTips
		/// <summary>
		///     Indicates whether ToolTips are being shown for tabs that have ToolTips set on
		///     them.
		/// </summary>
		[
			DefaultValue( false ),
			Description( "Indicates whether ToolTips are being shown for tabs that have ToolTips set on them." )
		]
		public virtual bool ShowToolTips
		{
			get
			{
				return showToolTips;
			}
			set
			{
				if( showToolTips != value )
				{
					showToolTips = value;

					if( !value && toolTipProvider != null )
					{
						toolTipProvider.Destroy();
						toolTipProvider = null;
					}
					OnShowToolTipsChanged( EventArgs.Empty );
					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ShowToolTipsChanged"/> event.
		/// </summary>
		/// <param name="e">Event Data.</param>
		protected virtual void OnShowToolTipsChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ShowToolTips );
#else
			;
#endif

			if( ShowToolTipsChanged != null )
				ShowToolTipsChanged( this, e );
		}

		ControlToolTip IControlToolTipProvider.GetControlToolTip()
		{
			if( showToolTips && !DesignMode && toolTipProvider == null )
			{
				toolTipProvider = new ControlToolTip( this );
				toolTipProvider.CreateToolTipHandle();
			}

			return toolTipProvider;
		}

		/// <override/>
		protected override void OnHandleDestroyed( EventArgs e )
		{
			ResetToolTips();

			if( toolTipProvider != null )
			{
				toolTipProvider.Destroy();
				toolTipProvider = null;
			}
#if DEBUG

			if( Switches.SplitterControlEvents.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( this );
#else

			;
#endif

			base.OnHandleDestroyed( e );
		}

		/// <summary>
		/// Reinitializes and hides ToolTips.
		/// </summary>
		public virtual void ResetToolTips()
		{
			if( horizontalSplitterBar != null )
				horizontalSplitterBar.ResetToolTip();

			if( verticalSplitterBar != null )
				verticalSplitterBar.ResetToolTip();
		}

		internal ControlToolTip ToolTipProvider
		{
			get
			{
				return toolTipProvider;
			}
		}

		/// <override/>
		protected override void OnMouseDown( MouseEventArgs mevent )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, mevent.X, mevent.Y, mevent.Button, mevent.Clicks );
#else
			;
#endif

			base.OnMouseDown( mevent );

			if( toolTipProvider != null )
				toolTipProvider.DeactivateToolTip();
		}

		/// <override/>
		protected override void OnMouseUp( MouseEventArgs mevent )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, mevent.X, mevent.Y, mevent.Button, mevent.Clicks );
#else
			;
#endif
			base.OnMouseUp( mevent );

			if( this.IsHandleCreated && toolTipProvider != null )
				toolTipProvider.ActivateToolTip();
		}
		#endregion


		/// <summary>
		/// Gets or sets the button look for the arrow buttons.
		/// </summary>
		[
			//Category("ScrollButtons"),
#if FLAT
		DefaultValue( ButtonLook.Flat ),
#else
		DefaultValue(ButtonLook.Normal),
#endif
 Description( "Indicates if arrow buttons should be drawn flat or raised." )
		]
		public virtual ButtonLook ButtonLook
		{
			get
			{
				return buttonLook;
			}
			set
			{
				if( buttonLook != value )
				{
					buttonLook = value;
					OnButtonLookChanged( EventArgs.Empty );
					Refresh();
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ButtonLookChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnButtonLookChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ButtonLook );
#else
			;
#endif

			if( ButtonLookChanged != null )
				ButtonLookChanged( this, e );
		}


		/// <summary>
		/// <see cref="SplitterLayout"/> holds information about the current vertical and horizontal split positions.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public SplitterLayout CurrentLayout
		{
			get
			{
				if( splitterInfo.splitterLayout == null )
					splitterInfo.splitterLayout = new SplitterLayout();

				return splitterInfo.splitterLayout;
			}
		}

		/// <summary>
		/// Returns either an <see cref="IScrollBarContainer"/> that has a reference to a scrollbar or creates a scrollbar.
		/// </summary>
		/// <param name="sbType">Indicates horizontal or vertical scrollbar.</param>
		/// <param name="index">The zero-based row or column index of the scrollbar.</param>
		/// <returns>A control that is derived from <see cref="ScrollBar"/> or implements <see cref="IScrollBarContainer"/>.</returns>
		protected virtual Control CreateScrollBarContainer( ScrollBars sbType, int index )
		{
			return CreateScrollBar( sbType, index );
		}

		/// <summary>
		/// Creates a scrollbar for the specified row or column index.
		/// </summary>
		/// <param name="sbType">Indicates horizontal or vertical scrollbar.</param>
		/// <param name="index">The zero-based row or column index of the scrollbar.</param>
		/// <returns>A <see cref="ScrollBar"/>.</returns>
		protected virtual Control CreateScrollBar( ScrollBars sbType, int index )
		{
#if FLAT
#pragma warning disable //Fix for the issue 13781
            if (SupportsFlatScrollBars)
#pragma warning restore
            {
                FlatScrollBar fsb;
                if (sbType == ScrollBars.Horizontal)
                {
                    fsb = new FlatHScrollBar();
                }
                else
                {
                    fsb = new FlatVScrollBar();
                }
                fsb.Appearance = scrollBarAppearance;
                fsb.BackColor = scrollBarColor;
                return fsb;
            }
#endif

			if( Office2007ScrollBars || GridOfficeScrollBars == OfficeScrollBars.Office2007 )
			{
				ScrollBarCustomDraw officesb;
				if( sbType == ScrollBars.Horizontal )
				{
					officesb = new HScrollBarCustomDraw();
				}
				else
				{
					officesb = new VScrollBarCustomDraw();
				}

				officesb.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
				officesb.OfficeColorScheme = office2007ScrollBarsColorScheme;
				//  officesb.BackColor = scrollBarColor;
				return officesb;
			}
            else if ( GridOfficeScrollBars == OfficeScrollBars.Office2010 )
            {
                ScrollBarCustomDraw officesb;
                if (sbType == ScrollBars.Horizontal)
                {
                    officesb = new HScrollBarCustomDraw();
                }
                else
                {
                    officesb = new VScrollBarCustomDraw();
                }

                officesb.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                officesb.Office2010ColorScheme = office2010ScrollBarsColorScheme;
                return officesb;
            }
			else if (MetroScrollBar)
			{
				ScrollBarCustomDraw metroSB;
				if (sbType == ScrollBars.Horizontal)
				{
					metroSB = new HScrollBarCustomDraw();
				}
				else
				{
					metroSB = new VScrollBarCustomDraw();
				}

				metroSB.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                if (GridOfficeScrollBars != OfficeScrollBars.Metro)
                {
                    Color metroBlue = ColorTranslator.FromHtml("#119EDA");
                    metroSB.MetroColorTable.ThumbNormal = Color.DarkGray;
                    metroSB.MetroColorTable.ThumbChecked = metroSB.MetroColorTable.ArrowChecked = metroBlue;
                    metroSB.MetroColorTable.ThumbPushed = metroBlue;
                    metroSB.MetroColorTable.ArrowNormal = Color.DarkGray;
                }
				return metroSB;
			}
			ScrollBar sb;
			if( sbType == ScrollBars.Horizontal )
				sb = new HScrollBar();
			else
				sb = new VScrollBar();
			sb.BackColor = scrollBarColor;
			return sb;
		}

		internal const int SWP_NOSIZE = 1; // 0x0001
		internal const int SWP_NOMOVE = 2; // 0x0002
		internal const int SWP_NOZORDER = 4; // 0x0004
		internal const int SWP_NOACTIVATE = 16; // 0x0010
		internal const int SWP_NOREDRAW = 32; // 0x0010
		internal const int SWP_SHOWWINDOW = 64; // 0x0040
		internal const int SWP_HIDEWINDOW = 128; // 0x0080
		internal const int SWP_DRAWFRAME = 32; // 0x0020



		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			if( Size.IsEmpty || Disposing ) // || this.isFormClosing)
				return;
            if (this.parentForm != null && this.parentForm.Disposing)
                return;
#if DEBUG

			if( Switches.Workbook.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( this, Size, Disposing, RowCount, ColumnCount );
#else

			;
#endif


			if( ToolTipProvider != null )
				ResetToolTips();

			for( int column = 0; column < ColumnCount; column++ )
			{
				if( hScrollBars[column] == null )
				{
					hScrollBars[column] = CreateScrollBarContainer( ScrollBars.Horizontal, column );
					hScrollBars[column].TabStop = false;
				}
			}

			for( int row = 0; row < RowCount; row++ )
			{
				if( vScrollBars[row] == null )
				{
					vScrollBars[row] = CreateScrollBarContainer( ScrollBars.Vertical, row );
					vScrollBars[row].TabStop = false;
				}
			}
			int sbHeight = this.ShowHorizontalScrollBar ? SystemInformation.HorizontalScrollBarHeight : 0;
            int sbWidth = this.ShowVerticalScrollBar ? SystemInformation.HorizontalScrollBarThumbWidth : 0;
            int splitBarWidth = 7;
            if (EnableTouchMode)
            {
                sbHeight = (int)(SystemInformation.HorizontalScrollBarHeight * 1.5F);
                splitBarWidth = (splitBarWidth + 3);
                sbWidth = (int)(SystemInformation.HorizontalScrollBarThumbWidth * 1.5F);
            }
			splitterInfo.innerBounds = ClientRectangle;
			splitterInfo.innerBounds.Width -= sbWidth;
			splitterInfo.innerBounds.Height -= sbHeight;

			int innerBottom = splitterInfo.innerBounds.Bottom;
			splitterInfo.horizontalBarBounds = new Rectangle( splitterInfo.innerBounds.Left, innerBottom, splitterInfo.innerBounds.Width, sbHeight );
			splitterInfo.verticalBarBounds = new Rectangle( splitterInfo.innerBounds.Right, splitterInfo.innerBounds.Top, sbWidth, splitterInfo.innerBounds.Height );
			splitterInfo.sizeGripBounds = new Rectangle( splitterInfo.innerBounds.Right, innerBottom, sbWidth, sbHeight );

			if( !this.DesignMode && HSplitPos != 100 )
			{
				int hPos = splitterInfo.horizontalBarBounds.Width * HSplitPos / 100 - splitBarWidth;
				splitterInfo.hLeftBarBounds = new Rectangle( 0, splitterInfo.horizontalBarBounds.Top, hPos, sbHeight );
				splitterInfo.hSplitBarBounds = new Rectangle( hPos, 0, splitBarWidth, Height );
				hPos += splitBarWidth;
				splitterInfo.hRightBarBounds = new Rectangle( hPos, splitterInfo.horizontalBarBounds.Top, splitterInfo.innerBounds.Width - hPos, sbHeight );
			}
			else
			{
				splitterInfo.hLeftBarBounds = splitterInfo.horizontalBarBounds;
				if( ( SplitBars & DynamicSplitBars.SplitColumns ) != 0 )
				{
					splitterInfo.hLeftBarBounds.Width -= splitBarWidth;
					splitterInfo.hSplitBarBounds = new Rectangle( splitterInfo.hLeftBarBounds.Right, splitterInfo.horizontalBarBounds.Top, splitBarWidth, sbHeight );
				}
				else
					splitterInfo.hSplitBarBounds = Rectangle.Empty;
				splitterInfo.hRightBarBounds = Rectangle.Empty;
			}

			if( !this.DesignMode && VSplitPos != 100 )
			{
				int vPos = splitterInfo.verticalBarBounds.Height * VSplitPos / 100 - splitBarWidth;
				splitterInfo.vTopBarBounds = new Rectangle( splitterInfo.verticalBarBounds.Left, 0, sbWidth, vPos );
				splitterInfo.vSplitBarBounds = new Rectangle( 0, vPos, Width, splitBarWidth );
				vPos += splitBarWidth;
				splitterInfo.vBottomBarBounds = new Rectangle( splitterInfo.verticalBarBounds.Left, vPos, sbWidth, splitterInfo.innerBounds.Height - vPos );
			}
			else
			{
				splitterInfo.vTopBarBounds = splitterInfo.verticalBarBounds;
				if( ( SplitBars & DynamicSplitBars.SplitRows ) != 0 )
				{
					splitterInfo.vSplitBarBounds = new Rectangle( splitterInfo.vTopBarBounds.Left, splitterInfo.vTopBarBounds.Top, sbWidth, splitBarWidth );
					splitterInfo.vTopBarBounds.Height -= splitBarWidth;
					splitterInfo.vTopBarBounds.Y += splitBarWidth;
				}
				else
					splitterInfo.vSplitBarBounds = Rectangle.Empty;
				splitterInfo.vBottomBarBounds = Rectangle.Empty;
			}

			hScrollBars[0].Bounds = this.ReverseRectangleRTL( splitterInfo.hLeftBarBounds );
			if( splitterInfo.hRightBarBounds.IsEmpty )
			{
				if( hScrollBars[1] != null )
					hScrollBars[1].Visible = false;
			}
			else
			{
				hScrollBars[1].Bounds = this.ReverseRectangleRTL( splitterInfo.hRightBarBounds );
				hScrollBars[1].Visible = true;
			}
            if(horizontalSplitterBar!=null)
			horizontalSplitterBar.Bounds = this.ReverseRectangleRTL( splitterInfo.hSplitBarBounds );

			vScrollBars[0].Bounds = this.ReverseRectangleRTL( splitterInfo.vTopBarBounds );
			if( splitterInfo.vBottomBarBounds.IsEmpty )
			{
				if( vScrollBars[1] != null )
					vScrollBars[1].Visible = false;
			}
			else
			{
				vScrollBars[1].Bounds = this.ReverseRectangleRTL( splitterInfo.vBottomBarBounds );
				vScrollBars[1].Visible = true;
			}
            if(verticalSplitterBar!=null)
			verticalSplitterBar.Bounds = this.ReverseRectangleRTL( splitterInfo.vSplitBarBounds );

			oldHSplitRect = Rectangle.Empty;
			oldVSplitRect = Rectangle.Empty;

			for( int column = 0; column < ColumnCount; column++ )
			{
				if( hScrollBars[column].Parent == null )
				{
					Controls.Add( hScrollBars[column] );
				}
			}

			for( int row = 0; row < RowCount; row++ )
			{
				if( vScrollBars[row].Parent == null )
				{
					Controls.Add( vScrollBars[row] );
				}
			}

			SuspendLayout();


			Invalidate( ReverseRectangleRTL( splitterInfo.innerBounds ) );
			Invalidate( ReverseRectangleRTL( splitterInfo.hSplitBarBounds ) );
			Invalidate( ReverseRectangleRTL( splitterInfo.vSplitBarBounds ) );
			Invalidate( ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );

			int rowCount = DesignMode ? 1 : RowCount;
			int columnCount = DesignMode ? 1 : ColumnCount;
			for( int r = 0; r < rowCount; r++ )
			{
				for( int c = 0; c < columnCount; c++ )
				{
					GetPane( r, c );
				}
			}

			for( int r = 0; r < rowCount; r++ )
			{
				int row = rowCount - r - 1;
				Rectangle rowBounds;
				if( rowCount == 1 )
					rowBounds = splitterInfo.innerBounds;
				else
				{
					if( row == 0 )
						rowBounds = splitterInfo.vTopBarBounds;
					else
						rowBounds = splitterInfo.vBottomBarBounds;
					rowBounds.X = splitterInfo.innerBounds.Left;
					rowBounds.Width = splitterInfo.innerBounds.Width;
				}

				for( int c = 0; c < columnCount; c++ )
				{
					int column = columnCount - c - 1;
					Rectangle columnBounds;
					if( columnCount == 1 )
						columnBounds = splitterInfo.innerBounds;
					else
					{
						if( column == 0 )
							columnBounds = splitterInfo.hLeftBarBounds;
						else
							columnBounds = splitterInfo.hRightBarBounds;

						columnBounds.Y = splitterInfo.innerBounds.Top;
						columnBounds.Height = splitterInfo.innerBounds.Height;
					}
					Rectangle paneBounds = Rectangle.Intersect( rowBounds, columnBounds );
					Control pane = GetPane( row, column );
					if( pane != null )
					{
						ISplitterPaneSupport sps = pane as ISplitterPaneSupport;
						if( sps != null && sps.FillSplitterPane )
						{
							if( pane.Parent == null )
								pane.SuspendLayout();
#if DEBUG
							if( Switches.Workbook.TraceVerbose )
								TraceUtil.TraceCurrentMethodInfo( this, row, column, paneBounds );
#else
							;
#endif

							if( pane.Parent == null )
							{
								Rectangle bounds = ReverseRectangleRTL( paneBounds );
								//if (pane.Parent is TabBarPage && this.RightToLeft == RightToLeft.Yes)
								//	bounds.X -= this.vScrollBars[0].Bounds.Width;
								pane.Bounds = bounds;
							}
							else
							{
								Point loc = ReverseRectangleRTL( paneBounds ).Location;
								//if (loc.X < 20 && pane.Parent is TabBarPage && this.RightToLeft == RightToLeft.Yes)
								//	loc.X -= this.vScrollBars[0].Bounds.Width;
								pane.Location = loc;
								pane.Size = paneBounds.Size;
							}
							pane.TabStop = true;
							pane.TabIndex = row * 2 + column;
							//pane.BringToFront();
							if( pane.Parent == null )
								pane.ResumeLayout( false );
						}
					}
				}
			}

			ResumeLayout();
#if DEBUG

			if( Switches.SplitterControlEvents.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( this, levent.AffectedProperty );
#else

			;
#endif

			base.OnLayout( levent );
		}

		internal Rectangle ReverseRectangleRTL( Rectangle r )
		{
			if( this.RightToLeft == RightToLeft.Yes )
				return new Rectangle( this.ClientRectangle.Width - r.Right, r.Top, r.Width, r.Height );
			return r;
		}

		internal bool validatingFailed = false;

		Control IContainerControl.ActiveControl
		{
			get
			{
				return base.ActiveControl;
			}
			set
			{
				ActivateControl( value );
			}
		}

		/// <override/>
		protected override void OnParentChanged( EventArgs e )
		{
			Form f = Parent == null ? null : FindForm();
			if( parentForm != f )
			{
				if( parentForm != null )
					parentForm.Closing -= new CancelEventHandler( FormClosing );
				parentForm = f;
				if( parentForm != null )
					parentForm.Closing += new CancelEventHandler( FormClosing );
			}
		}

		void FormClosing( object sender, CancelEventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( sender, this );
#else
			;
#endif

		}

		Form parentForm = null;

		int inActivateControl = 0;

		internal Control GetActiveControlFixPane()
		{
			if( this.disposePane != null )
				return null;

			Control c = ActiveControl;
			if( c != null )
			{
				if( this.ActivePane != null && this.ActivePane.Contains( c ) )
					c = ActivePane;
			}
			return c;
		}

		/// <summary>
		/// Indicates whether the specified control is activated .
		/// </summary>
		/// <param name="c">The <see cref="Control"/> to be activated.</param>
		/// <returns>True if the control is successfully activated; false otherwise.</returns>
		/// <remarks>
		/// The control must be a child of the container control.
		/// </remarks>
		public bool ActivateControl( Control c )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( inActivateControl, "----BEGIN----", this, c );
#else
			;
#endif

			try
			{
				inActivateControl++;
				this.validatingFailed = false;
				Control active = GetActiveControlFixPane();
				if( c != active )
				{
					bool valid = true;
					CancelEventArgs e = new CancelEventArgs();
					// should I do a GetActiveControlFixPane here ...?
					if( c != null && active != null && c.CausesValidation )
						valid = Validate();
					if( !valid )
						this.validatingFailed = true;
					else
					{
						base.ActiveControl = null;
						if( c != null )
						{
							bool cv = c.CausesValidation;
							c.CausesValidation = false;
							base.ActiveControl = c;
							c.CausesValidation = cv;
						}
						if( ActiveControl != c && !( c != null && c.Contains( ActiveControl ) ) )
							this.validatingFailed = true;
					}
				}

				int row, column;
				if( FindPane( ActiveControl, out row, out column ) )
				{
					splitterInfo.activeRow = row;
					splitterInfo.activeColumn = column;
				}

				return !validatingFailed;
			}
			finally
			{
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( inActivateControl, this.validatingFailed, "----END----", this, ActiveControl );
#else
				;
#endif

				inActivateControl--;
			}
		}

		/// <summary>
		/// Gets or sets the Active control.
		/// </summary>
		public new Control ActiveControl
		{
			get
			{
				return base.ActiveControl;
			}
			set
			{
				ActivateControl( value );
			}
		}


		// Focus
		bool isActiveControl = false;
		bool isValidating = false;
		bool isDeactivatedCalled = false;
		bool hasControlFocus = false;
		bool isValidated = false;

		/// <summary>
		/// Indicates whether the <see cref="OnValidating"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool HasControlFocus
		{
			get
			{
				return hasControlFocus;
			}
		}

		/// <override/>
		protected override void OnEnter( EventArgs e )
		{
			isActiveControl = true;
			isValidating = false;
			isValidated = false;
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			base.OnEnter( e );
		}

		/// <override/>
		protected override void OnLeave( EventArgs e )
		{
			isActiveControl = false;
			isValidating = false;
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			base.OnLeave( e );

			if( ( !this.CausesValidation || this.IsValidated ) && !hasControlFocus )
				OnDeactivated( e );
		}

		/// <override/>
		protected override void OnValidating( CancelEventArgs e )
		{
			isValidating = true;
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( e.Cancel, this );
#else
			;
#endif

			base.OnValidating( e );

			if( !e.Cancel )
			{
				Control c = GetActiveControlFixPane();
				if( c != null && !c.CausesValidation )
					e.Cancel = !this.Validate();
			}
		}

		/// <override/>
		protected override void OnValidated( EventArgs e )
		{
			isValidating = false;
			isValidated = true;
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			base.OnValidated( e );

			if( !isActiveControl && !hasControlFocus )
				OnDeactivated( e );
		}

		/// <override/>
		protected override void OnLostFocus( EventArgs e )
		{
			RaiseControlLostFocus();
		}

		/// <override/>
		protected override void OnGotFocus( EventArgs e )
		{
			RaiseControlGotFocus();
		}

		/// <summary>
		/// Occurs when both <see cref="OnControlLostFocus"/> and <see cref="OnLeave"/> occur.
		/// </summary>
		[Description( "Occurs when both OnControlLostFocus and OnLeave occur." )]
		public event EventHandler Deactivated;

		/// <summary>
		/// Raises the <see cref="Deactivated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
		protected virtual void OnDeactivated( EventArgs e )
		{
			isDeactivatedCalled = true;
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			if( Deactivated != null )
				Deactivated( this, e );

			base.ActiveControl = null;
		}

		void ChildGotFocus( object sender, EventArgs e )
		{
			RaiseControlGotFocus();
		}

		void ChildLostFocus( object sender, EventArgs e )
		{
			RaiseControlLostFocus();
		}

		/// <summary>
		/// Indicates whether this control contains focus. If <see cref="ActiveControl"/>
		/// implements <see cref="IQueryFocusInside"/>, the <see cref="IQueryFocusInside.QueryFocusInside"/>
		/// method is called on the <see cref="ActiveControl"/>.
		/// </summary>
		/// <returns>True if the control or any child control has focus; False otherwise.</returns>
		public virtual bool QueryFocusInside()
		{
			if( ContainsFocus )
				return true;

			IQueryFocusInside qfi = this.ActiveControl as IQueryFocusInside;
			if( qfi != null )
				return qfi.QueryFocusInside();

			return false;
		}

		/// <summary>
		/// Raises the <see cref="Control.GotFocus"/> event. This method is called when the control
		/// or any child control gets focus and this control did not have focus before.
		/// </summary>
		/// <remarks>
		/// Inheriting classes should override this method instead of overriding <see cref="Control.OnGotFocus"/>
		/// because <see cref="OnControlGotFocus"/> is also called when child controls gets focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlGotFocus()
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			base.OnGotFocus( EventArgs.Empty );

			Control c = ActiveControl;
			if( c != null && !c.ContainsFocus )
			{
				if( c.CanFocus )
					c.Focus();
				else if( c.Parent != null && c.Parent.CanFocus )
					c.Parent.Focus();
				else
				{
					c = ActivePane;
					if( c != null && c.CanFocus )
						c.Focus();
				}
			}
		}

		/// <summary>
		/// Cancels any prior <see cref="ISupportUpdating.BeginUpdate"/> calls for child controls that implement <see cref="ISupportUpdating"/>.
		/// </summary>
		public void CancelUpdate()
		{
			foreach( Control c in this.Controls )
			{
				ISupportUpdating su = c as ISupportUpdating;
				if( su != null )
				{
					while( su.Updating )
						su.EndUpdate();
				}
			}
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
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

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
		/// This method is called if the control's <see cref="OnControlLostFocus"/> notification occurs
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
			// The following line emulates pressing the <ALT> key:
			//			SendKeys.Send("%");
		}

		void RaiseControlGotFocus()
		{
			if( !this.hasControlFocus )
			{
				hasControlFocus = true;
				OnControlGotFocus();
			}
		}

		void RaiseControlLostFocus()
		{
			if( !hasControlFocus )
			{
				TraceUtil.TraceCurrentMethodInfo( "-Duplicate call-", isValidating, this );
			}
			else if( !QueryFocusInside() )
			{
				hasControlFocus = false;
				OnControlLostFocus();
			}
		}

		/// <override/>
		protected override void OnControlRemoved( System.Windows.Forms.ControlEventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( e.Control, this );
#else
			;
#endif

			base.OnControlRemoved( e );

			e.Control.GotFocus -= new EventHandler( ChildGotFocus );
			e.Control.LostFocus -= new EventHandler( ChildLostFocus );
		}

		void _ControlAdded( System.Windows.Forms.ControlEventArgs e )
		{
			base.OnControlAdded( e );

			e.Control.GotFocus += new EventHandler( ChildGotFocus );
			e.Control.LostFocus += new EventHandler( ChildLostFocus );
		}

		private void DrawSplitHelper( Rectangle rect )
		{
			rect.Intersect( ClientRectangle );
			Rectangle r = RectangleToScreen( rect );
			ControlPaint.FillReversibleRectangle( r, SystemColors.Window );
		}

		DynamicSplitBars IDynamicSplitterFrame.SplitBars
		{
			get
			{
				return SplitBars;
			}
		}

		int IDynamicSplitterFrame.RowCount
		{
			get
			{
				return RowCount;
			}
		}

		int IDynamicSplitterFrame.ColumnCount
		{
			get
			{
				return ColumnCount;
			}
		}

		Control IDynamicSplitterFrame.ActivePane
		{
			get
			{
				return ActivePane;
			}
			set
			{
				ActivePane = value;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating what split behavior is supported. Rows, Columns or Both.
		/// </summary>
		[RefreshProperties( RefreshProperties.Repaint ),
		Description( "Indicates the splitter behavior that should be supported." )]
		public virtual DynamicSplitBars SplitBars
		{
			get
			{
				return splitterInfo != null ? splitterInfo.splitBars : DynamicSplitBars.None;
			}
			set
			{
				if( SplitBars != value )
				{
					splitterInfo.splitBars = value;
					UpdateSplitter();
					OnSplitBarsChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="SplitBarsChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnSplitBarsChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.SplitBars );
#else
			;
#endif

			try
			{
				if( SplitBarsChanged != null )
					SplitBarsChanged( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal splitter position in percentages of the splitter control's width.
		/// </summary>
		[RefreshProperties( RefreshProperties.Repaint )]
		[DefaultValue( 100 )]
		[Description( "The horizontal splitter position in percentages of the width." )]
		public virtual int HSplitPos
		{
			get
			{
				if( ( SplitBars & DynamicSplitBars.SplitColumns ) == 0 )
					return 100;

				SplitterLayout splitterLayout = SplitterLayout;
				int hSplitPos = splitterLayout._hSplitPos;
				if( hSplitPos < 5 || hSplitPos > 98 )
					hSplitPos = 100;

				return hSplitPos;
			}
			set
			{
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( this, value );
#else
				;
#endif

				if( value < 5 )
					value = 0;
				else if( value > 98 )
					value = 100;

				if( value != HSplitPos )
				{
					SplitterLayout splitterLayout = SplitterLayout;
					UpdateSplitter( value, splitterLayout._vSplitPos );
					OnHSplitPosChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="HSplitPosChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnHSplitPosChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.HSplitPos );
#else
			;
#endif

			try
			{
				if( HSplitPosChanged != null )
					HSplitPosChanged( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		/// <summary>
		/// Gets or sets the vertical splitter position in percentages of the splitter control's height.
		/// </summary>
		[RefreshProperties( RefreshProperties.Repaint )]
		[DefaultValue( 100 )]
		[Description( "The horizontal splitter position in percentages of the height." )]
		public virtual int VSplitPos
		{
			get
			{
				if( ( SplitBars & DynamicSplitBars.SplitRows ) == 0 )
					return 100;

				SplitterLayout splitterLayout = SplitterLayout;
				int vSplitPos = splitterLayout._vSplitPos;
				if( vSplitPos < 5 || vSplitPos > 98 )
					vSplitPos = 100;

				return vSplitPos;
			}
			set
			{
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( this, value );
#else
				;
#endif

				if( value < 5 )
					value = 0;
				else if( value > 98 )
					value = 100;

				if( value != VSplitPos )
				{
					SplitterLayout splitterLayout = SplitterLayout;
					UpdateSplitter( splitterLayout._hSplitPos, value );
					OnVSplitPosChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="VSplitPosChanged"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnVSplitPosChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.VSplitPos );
#else
			;
#endif

			try
			{
				if( VSplitPosChanged != null )
					VSplitPosChanged( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		void UpdateSplitter()
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#else
			;
#endif

			SplitterLayout splitterLayout = SplitterLayout;
			UpdateSplitter( splitterLayout._hSplitPos, splitterLayout._vSplitPos );
		}

		void UpdateSplitter( int hSplitPos, int vSplitPos )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, hSplitPos, vSplitPos );
#else
			;
#endif

			SuspendLayout();
			if( hSplitPos == 100 && ColumnCount > 1 )
				DeleteColumn( 1 );
			if( vSplitPos == 0 && RowCount > 1 )
				DeleteRow( 1 );
			if( hSplitPos == 0 && ColumnCount > 1 )
			{
				DeleteColumn( 0 );
				hSplitPos = 100;
			}
			if( vSplitPos == 100 && RowCount > 1 )
			{
				DeleteRow( 0 );
				vSplitPos = 100;
			}
			SplitterLayout splitterLayout = SplitterLayout;
			splitterLayout._hSplitPos = hSplitPos;
			splitterLayout._vSplitPos = vSplitPos;
			ResumeLayout( false );
			Refresh();
		}

		/// <summary>
		/// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
		/// </summary>
		public override void Refresh()
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, Created );
#else
			;
#endif

			if( !Created )
				return;

			PerformLayout();

			for( int row = 0; row < this.RowCount; row++ )
			{
				for( int column = 0; column < ColumnCount; column++ )
				{
					Control pane = GetPane( row, column );
					if( pane != null )
					{
						pane.Visible = true;
						if( !splitterInfo.splitterControl.Controls.Contains( pane ) )
						{
							splitterInfo.splitterControl.Controls.Add( pane );
						}
						IScrollBarWrapperContainer sc = pane as IScrollBarWrapperContainer;
						if( sc != null )
							sc.UpdateScrollBars();
					}
				}
			}
		}

		/// <override/>
		protected override void OnHandleCreated( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, Handle );
#else
			;
#endif

			base.OnHandleCreated( e );
			Refresh();
		}

		/// <summary>
		/// Gets or sets the <see cref="SplitterLayout"/> that holds information about current vertical and horizontal split positions.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public virtual SplitterLayout SplitterLayout
		{
			get
			{
				if( splitterInfo.splitterLayout == null )
				{
					splitterInfo.splitterLayout = new SplitterLayout();
				}
				return splitterInfo.splitterLayout;
			}
			set
			{
				if( splitterInfo.splitterLayout != value )
				{
					splitterInfo.splitterLayout = value;
					UpdateSplitter();
					OnSplitterLayoutChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="SplitterLayoutChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnSplitterLayoutChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.SplitterLayout );
#else
			;
#endif

			try
			{
				if( SplitterLayoutChanged != null )
					SplitterLayoutChanged( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		/// <summary>
		/// Occurs when the user drags the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="x">The current horizontal position in pixels.</param>
		/// <param name="y">The current vertical position in pixels.</param>
		public virtual void OnMoveSplitter( object sender, int x, int y )
		{
			int sbHeight = SystemInformation.HorizontalScrollBarHeight;
			int sbWidth = SystemInformation.VerticalScrollBarWidth;
			Rectangle splitBarBounds = Rectangle.Empty;

			if( sender == horizontalSplitterBar )
			{
				if( !oldHSplitRect.IsEmpty )
				{
					DrawSplitHelper( oldHSplitRect );
					oldHSplitRect = Rectangle.Empty;
				}

				int vs = 0;
				if( this.RightToLeft == RightToLeft.Yes )
					vs = SystemInformation.VerticalScrollBarWidth;
				int nx = Math.Min( Math.Max( 0, horizontalSplitterBar.Bounds.X - vs + x ), splitterInfo.horizontalBarBounds.Width - 1 );
				relativeSplitBarWidth = nx * 100 / ( splitterInfo.horizontalBarBounds.Width - 14 );

				splitBarBounds = new Rectangle( relativeSplitBarWidth * splitterInfo.horizontalBarBounds.Width / 100 - 7 + vs, 0, 7, Height );
				if( this.RightToLeft == RightToLeft.Yes )
					relativeSplitBarWidth = 100 - ( nx ) * 100 / ( splitterInfo.horizontalBarBounds.Width + 14 );

				if( !splitBarBounds.IsEmpty )
				{
					splitBarBounds.Intersect( splitterInfo.innerBounds );
					//                    Trace.WriteLine(String.Format("DrawSplitHelper({0})", splitBarBounds));
					DrawSplitHelper( splitBarBounds );
					oldHSplitRect = splitBarBounds;
				}
			}
			else if( sender == verticalSplitterBar )
			{
				if( !oldVSplitRect.IsEmpty )
				{
					DrawSplitHelper( oldVSplitRect );
					oldVSplitRect = Rectangle.Empty;
				}

				int ny = Math.Min( Math.Max( 0, verticalSplitterBar.Bounds.Y + y ), splitterInfo.verticalBarBounds.Height - 1 );
				relativeSplitBarHeight = ny * 100 / ( splitterInfo.verticalBarBounds.Height - 14 );
				splitBarBounds = new Rectangle( 0, relativeSplitBarHeight * splitterInfo.verticalBarBounds.Height / 100 - 7, Width, 7 );

				if( !splitBarBounds.IsEmpty )
				{
					splitBarBounds.Intersect( splitterInfo.innerBounds );
					//                    Trace.WriteLine(String.Format("DrawSplitHelper({0})", splitBarBounds));
					DrawSplitHelper( splitBarBounds );
					oldVSplitRect = splitBarBounds;
				}
			}

		}

		/// <summary>
		/// Occurs after the user moves the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		public virtual void OnMovedSplitter( object sender )
		{
			if( sender != null )
			{
				if( !oldHSplitRect.IsEmpty )
				{
					DrawSplitHelper( oldHSplitRect );
					oldHSplitRect = Rectangle.Empty;
				}
				if( !oldVSplitRect.IsEmpty )
				{
					DrawSplitHelper( oldVSplitRect );
					oldVSplitRect = Rectangle.Empty;
				}
				if( sender == horizontalSplitterBar )
				{
					HSplitPos = relativeSplitBarWidth;
				}
				else if( sender == verticalSplitterBar )
				{
					VSplitPos = relativeSplitBarHeight;
				}
			}
			else
				Refresh();
		}

		void IInternalSplitterParent.InvalidateSplitter( object sender )
		{
		}

		Cursor IInternalSplitterParent.OverrideCursor
		{
			get
			{
				return overrideSplitCursor;
			}
			set
			{
				if( overrideSplitCursor != value )
				{
					overrideSplitCursor = value;

					if( IsHandleCreated )
					{
						NativeMethods.SendMessage( Handle, NativeMethods.WM_SETCURSOR, Handle, NativeMethods.HTCLIENT );
					}
				}
			}
		}

		/// <override/>
		protected override void OnPaint( PaintEventArgs pe )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, Size );
#else
			;
#endif

			lock( this )
			{
				if( !splitterInfo.innerBounds.IsEmpty )
				{
					pe.Graphics.FillRectangle( SystemBrushes.Window, this.ReverseRectangleRTL( splitterInfo.innerBounds ) );
				}

				Rectangle hBounds = Rectangle.Empty;
				if( horizontalSplitterBar != null && !horizontalSplitterBar.Bounds.IsEmpty )
				{
                    if (this is TabBarSplitterControl)
                        horizontalSplitterBar.Style = (this as TabBarSplitterControl).Style;
                    horizontalSplitterBar.Paint(pe.Graphics, ButtonLook == ButtonLook.Flat);
                    hBounds = this.ReverseRectangleRTL(horizontalSplitterBar.Bounds);
                }

				Rectangle vBounds = Rectangle.Empty;
				if( verticalSplitterBar != null && !verticalSplitterBar.Bounds.IsEmpty )
				{
                    if (this is TabBarSplitterControl)
                        verticalSplitterBar.Style = (this as TabBarSplitterControl).Style;
					verticalSplitterBar.Paint( pe.Graphics, ButtonLook == ButtonLook.Flat );
					vBounds = this.ReverseRectangleRTL( verticalSplitterBar.Bounds );

					if( vBounds.IntersectsWith( hBounds ) )
					{
						Rectangle intersect = Rectangle.Intersect( hBounds, vBounds );
                        if (!MetroScrollBar)
                        {
                            intersect.Width -= 3;
                            intersect.X++;
                            pe.Graphics.FillRectangle(SystemBrushes.Control, this.ReverseRectangleRTL(intersect));
                        }
                        
                    }
				}

				pe.Graphics.ExcludeClip( this.ReverseRectangleRTL( hBounds ) );
				pe.Graphics.ExcludeClip( this.ReverseRectangleRTL( vBounds ) );

				if( ShowSizeGrip )
				{
					if( !splitterInfo.sizeGripBounds.IsEmpty )
					{
						bool themed = XPThemes.IsThemedOS && ( (IThemedControl)this ).ThemesEnabled;
						if( themed )
							this.themedScrollBarDrawing.DrawSizeBox( pe.Graphics, this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );
						else
							ControlPaint.DrawSizeGrip( pe.Graphics, BackColor, this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );
						pe.Graphics.ExcludeClip( this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );
					}
				}
				else
				{
					if( !splitterInfo.sizeGripBounds.IsEmpty )
					{
						pe.Graphics.FillRectangle( SystemBrushes.Control, this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );
						pe.Graphics.ExcludeClip( this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ) );
					}
				}
#if DEBUG

				if( Switches.SplitterControlEvents.TraceVerbose )

					TraceUtil.TraceCurrentMethodInfo( this, pe.ClipRectangle );
#else

				;
#endif

				base.OnPaint( pe );
			}
		}

		private ThemedScrollBarDrawing themedScrollBarDrawing
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsAppThemed )
				{
					if( _themedScrollBarDrawing == null )
						this._themedScrollBarDrawing = new ThemedScrollBarDrawing();
				}
				return this._themedScrollBarDrawing;
			}
		}

		private ThemedScrollBarDrawing _themedScrollBarDrawing = null;


		bool showSizeGrip = false;

		/// <summary>
		/// Gets or sets the border style of the RecordNavigationControl.
		/// </summary>
		[
		Category( "Appearance" ),
		DefaultValue( false ),
		Description( "Gets or sets if a size grip box should be shown." )
		]
		public bool ShowSizeGrip
		{
			get
			{
				return showSizeGrip;
			}
			set
			{
				if( showSizeGrip != value )
				{
					showSizeGrip = value;
					Refresh();
					OnShowSizeGripChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Occurs when <see cref="ShowSizeGrip"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ShowSizeGrip property has changed." )]
		public event EventHandler ShowSizeGripChanged;


		/// <summary>
		/// Raises the <see cref="SplitterControl.ShowSizeGripChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnShowSizeGripChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ShowSizeGrip );
#else
			;
#endif

			if( ShowSizeGripChanged != null )
				ShowSizeGripChanged( this, e );
		}




#if FLAT
		/// <summary>
		/// Toggles between standard and flat scrollbars.
		/// </summary>
		[
		Browsable( false ),
		Description( "Toggle between standard and flat scrollbars." ),
		DefaultValue( false ),
		Obsolete( "Use Office2007ScrollBars instead." )
		]
		public bool SupportsFlatScrollBars
		{
			get
			{
				return supportsFlatScrollBars;
			}
			set
			{
				if( supportsFlatScrollBars != value )
				{
					SuspendLayout();
					supportsFlatScrollBars = value;
					int n;
					for( n = 0; n < 2; n++ )
					{
						if( hScrollBars[n] is IScrollBarContainer )
						{
							IScrollBarContainer isc = hScrollBars[n] as IScrollBarContainer;
							if( isc != null )
								isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n );
						}
						else if( hScrollBars[n] != null )
						{
							Controls.Remove( hScrollBars[n] );
							hScrollBars[n] = this.CreateScrollBar( ScrollBars.Horizontal, n );
							Controls.Add( hScrollBars[n] );
						}
					}
					for( n = 0; n < 2; n++ )
					{
						if( vScrollBars[n] is IScrollBarContainer )
						{
							IScrollBarContainer isc = vScrollBars[n] as IScrollBarContainer;
							if( isc != null )
								isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n ) as ScrollBar;
						}
						else if( vScrollBars[n] != null )
						{
							Controls.Remove( vScrollBars[n] );
							vScrollBars[n] = this.CreateScrollBar( ScrollBars.Vertical, n );
							Controls.Add( vScrollBars[n] );
						}
					}
					ResumeLayout( false );
					Refresh();
				}
				OnSupportsFlatScrollBarsChanged( EventArgs.Empty );
			}
		}

		/// <summary>
		/// Raises the <see cref="SplitterControl.SupportsFlatScrollBarsChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnSupportsFlatScrollBarsChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
                TraceUtil.TraceCurrentMethodInfo(this, this.Office2007ScrollBars);
#else
			;
#endif

			if( SupportsFlatScrollBarsChanged != null )
				SupportsFlatScrollBarsChanged( this, e );
		}



		/// <summary>
		/// Gets or sets the style of flat scrollbars.
		/// </summary>
		[
		Browsable( false ),
		Description( "Style of flat scrollbars." ),
		DefaultValue( FlatScrollBarStyle.Flat ),
		Obsolete( "Use Office2007ScrollBarsColorScheme instead." )
		]
		public FlatScrollBarStyle ScrollBarAppearance
		{
			get
			{
				return scrollBarAppearance;
			}
			set
			{
				if( scrollBarAppearance != value )
				{
					scrollBarAppearance = value;
					if( this.SupportsFlatScrollBars )
					{
						FlatScrollBar sb;
						for( int n = 0; n < 2; n++ )
						{
							sb = GetContainedScrollBar( hScrollBars[n] ) as FlatScrollBar;
							if( sb != null )
								sb.Appearance = scrollBarAppearance;
						}
						for( int n = 0; n < 2; n++ )
						{
							sb = GetContainedScrollBar( vScrollBars[n] ) as FlatScrollBar;
							if( sb != null )
								sb.Appearance = scrollBarAppearance;
						}
					}
					OnScrollBarAppearanceChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollBarAppearanceChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnScrollBarAppearanceChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.Office2007ScrollBarsColorScheme );
#else
			;
#endif

			if( ScrollBarAppearanceChanged != null )
				ScrollBarAppearanceChanged( this, e );
		}

		/// <summary>
		/// Gets or sets the Backcolor for flat scrollbars.
		/// </summary>
		[
		Browsable( false ),
		Description( "Backcolor for flat scrollbars." ),
		Obsolete( "Use Office2007ScrollBarsColorScheme instead." )
		]
		public Color ScrollBarColor
		{
			get
			{
				return scrollBarColor;
			}
			set
			{
				if( scrollBarColor != value )
				{
					SuspendLayout();
					scrollBarColor = value;
					if( this.SupportsFlatScrollBars )
					{
						int n;
						for( n = 0; n < 2; n++ )
						{
							if( hScrollBars[n] is FlatScrollBar )
							{
								Controls.Remove( hScrollBars[n] );
								hScrollBars[n] = this.CreateScrollBar( ScrollBars.Horizontal, n );
								Controls.Add( hScrollBars[n] );
							}
							else
							{
								IScrollBarContainer isc = hScrollBars[n] as IScrollBarContainer;
								if( isc != null && isc.ScrollBar is FlatScrollBar )
									isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n );
							}
						}
						for( n = 0; n < 2; n++ )
						{
							if( vScrollBars[n] is FlatScrollBar )
							{
								Controls.Remove( vScrollBars[n] );
								vScrollBars[n] = this.CreateScrollBar( ScrollBars.Vertical, n );
								Controls.Add( vScrollBars[n] );
							}
							else
							{
								IScrollBarContainer isc = vScrollBars[n] as IScrollBarContainer;
								if( isc != null && isc.ScrollBar is FlatScrollBar )
									isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n ) as ScrollBar;
							}
						}
					}
					OnScrollBarColorChanged( EventArgs.Empty );
					ResumeLayout();
					Refresh();
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ScrollBarColorChanged"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnScrollBarColorChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.Office2007ScrollBarsColorScheme );
#else
			;
#endif

			if( ScrollBarColorChanged != null )
				ScrollBarColorChanged( this, e );
		}
		private bool ShouldSerializeScrollBarColor()
		{
			return scrollBarColor != SystemColors.ScrollBar && supportsFlatScrollBars && !this.Office2007ScrollBars;
		}

		/// <summary>
		/// Resets the <see cref="ScrollBarColor"/> to default.
		/// </summary>
		public void ResetScrollBarColor()
		{
			scrollBarColor = SystemColors.ScrollBar;
		}
#endif
		internal bool metroScrollBar = false;
		internal bool MetroScrollBar
		{
			get { return metroScrollBar; }
			set
			{
				metroScrollBar = value;
                Office2007ScrollBars = true;
				for (int n = 0; n < 2; n++)
				{
					if (hScrollBars[n] is IScrollBarContainer)
					{
						IScrollBarContainer isc = hScrollBars[n] as IScrollBarContainer;
						if (isc != null)
							isc.ScrollBar = this.CreateScrollBar(ScrollBars.Horizontal, n);
					}
				}
                Office2007ScrollBars = false;
			}
		}
		/// <summary>
		/// Toggles between standard and Office2007 scrollbars.
		/// </summary>
		[
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
					SuspendLayout();
					office2007ScrollBars = value;
					int n;

					for( n = 0; n < 2; n++ )
					{
						if( hScrollBars[n] is IScrollBarContainer )
						{
							IScrollBarContainer isc = hScrollBars[n] as IScrollBarContainer;
							if( isc != null )
								isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n );
						}
						else if( hScrollBars[n] != null )
						{
							Controls.Remove( hScrollBars[n] );
							hScrollBars[n].Dispose();
							hScrollBars[n] = this.CreateScrollBar( ScrollBars.Horizontal, n );
							Controls.Add( hScrollBars[n] );
						}
					}
					for( n = 0; n < 2; n++ )
					{
						if( vScrollBars[n] is IScrollBarContainer )
						{
							IScrollBarContainer isc = vScrollBars[n] as IScrollBarContainer;
							if( isc != null )
								isc.ScrollBar = this.CreateScrollBar( ScrollBars.Horizontal, n ) as ScrollBar;
						}
						else if( vScrollBars[n] != null )
						{
							Controls.Remove( vScrollBars[n] );
							vScrollBars[n].Dispose();
							vScrollBars[n] = this.CreateScrollBar( ScrollBars.Vertical, n );
							Controls.Add( vScrollBars[n] );
						}
					}
					ResumeLayout( false );
					Refresh();
				}

				OnOffice2007ScrollBarsChanged( EventArgs.Empty );
                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(OfficeScrollBars.Office2007);
                OnOfficeScrollBarsChanged(eventArgs);
            }
		}

		/// <summary>
		/// Raises the <see cref="SplitterControl.Office2007ScrollBarsChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnOffice2007ScrollBarsChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.Office2007ScrollBars );
#else
			;
#endif

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
		/// Gets or sets the style of Office2007 scroll bars
		/// </summary>
		[
		Browsable( true ),
		Description( "Office 2007 style scrollbars" ),
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
						for( int n = 0; n < 2; n++ )
						{
							ScrollBarCustomDraw sb = GetContainedScrollBar( hScrollBars[n] ) as ScrollBarCustomDraw;
							if( sb != null )
								sb.OfficeColorScheme = office2007ScrollBarsColorScheme;
						}

						for( int n = 0; n < 2; n++ )
						{
							ScrollBarCustomDraw sb = GetContainedScrollBar( vScrollBars[n] ) as ScrollBarCustomDraw;
							if( sb != null )
								sb.OfficeColorScheme = office2007ScrollBarsColorScheme;
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
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.Office2007ScrollBarsColorScheme );
#else
			;
#endif

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
        /// Occurs when the <see cref="Office2007ScrollBars"/> property has changed.
        /// </summary>
        [Category("Appearance"), Description("Occurs when the SupportsOfficeFlatScrollBars property has changed.")]
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

        #region Office2010 Scrollbars
        private Office2010ColorScheme office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
        OfficeScrollBars gridOfficeScrollBars = OfficeScrollBars.None;

        /// <summary>
        /// Gets or sets the Office like scrollbars.
        /// </summary>
        [
        Description("Gets or sets the Office like scrollbars"),
        DefaultValue(OfficeScrollBars.None),
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
                    SuspendLayout();
                    gridOfficeScrollBars = value;
                    if (value == OfficeScrollBars.Metro)
                        this.MetroScrollBar = true;
                    else
                        this.MetroScrollBar = false;
                    int n;

                    for (n = 0; n < 2; n++)
                    {
                        if (hScrollBars[n] is IScrollBarContainer)
                        {
                            IScrollBarContainer isc = hScrollBars[n] as IScrollBarContainer;
                            if (isc != null)
                                isc.ScrollBar = this.CreateScrollBar(ScrollBars.Horizontal, n);
                        }
                        else if (hScrollBars[n] != null)
                        {
                            Controls.Remove(hScrollBars[n]);
                            hScrollBars[n].Dispose();
                            hScrollBars[n] = this.CreateScrollBar(ScrollBars.Horizontal, n);
                            Controls.Add(hScrollBars[n]);
                        }
                    }
                    for (n = 0; n < 2; n++)
                    {
                        if (vScrollBars[n] is IScrollBarContainer)
                        {
                            IScrollBarContainer isc = vScrollBars[n] as IScrollBarContainer;
                            if (isc != null)
                                isc.ScrollBar = this.CreateScrollBar(ScrollBars.Horizontal, n) as ScrollBar;
                        }
                        else if (vScrollBars[n] != null)
                        {
                            Controls.Remove(vScrollBars[n]);
                            vScrollBars[n].Dispose();
                            vScrollBars[n] = this.CreateScrollBar(ScrollBars.Vertical, n);
                            Controls.Add(vScrollBars[n]);
                        }
                    }
                    ResumeLayout(false);
                    Refresh();
                }

                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(value);
                OnOfficeScrollBarsChanged(eventArgs);
            }
        }
        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars.
        /// </summary>
        [
        Browsable(true),
        Description("Office 2010 style scrollbars."),
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
						for( int n = 0; n < 2; n++ )
						{
							ScrollBarCustomDraw sb = GetContainedScrollBar( hScrollBars[n] ) as ScrollBarCustomDraw;
							if( sb != null )
                                sb.Office2010ColorScheme = office2010ScrollBarsColorScheme;
						}

						for( int n = 0; n < 2; n++ )
						{
							ScrollBarCustomDraw sb = GetContainedScrollBar( vScrollBars[n] ) as ScrollBarCustomDraw;
							if( sb != null )
                                sb.Office2010ColorScheme = office2010ScrollBarsColorScheme;
						}

                        OnOffice2010ScrollBarsColorSchemeChanged(EventArgs.Empty);
                    }

                    if (this.DesignMode)
                        GridOfficeScrollBars = OfficeScrollBars.Office2010;
                }
            }
        }
        # endregion

        /// <summary>
		/// Toggles support for using the control inside a dynamic splitter window and sharing scrollbars
		/// with the parent window.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ISplitterPaneFactory SplitterPaneFactory
		{
			get
			{
				return splitterPaneFactory;
			}
			set
			{
				splitterPaneFactory = value;
			}
		}

		/// <override/>
		protected override void OnVisibleChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, Visible );
#else
			;
#endif

			base.OnVisibleChanged( e );
		}

		internal Rectangle InnerBounds
		{
			get
			{
				return this.splitterInfo.innerBounds;
			}
		}

		bool showVerticalScrollBar = true;

		/// <summary>
		///     Toggles visibility of the vertical scrollbar.
		/// </summary>
		[Description( "Toggles visibility of the vertical scrollbar." )]
		[DefaultValue( true )]
		public virtual bool ShowVerticalScrollBar
		{
			get
			{
				return showVerticalScrollBar;
			}
			set
			{
				if( showVerticalScrollBar != value )
				{
					showVerticalScrollBar = value;
					OnShowVerticalScrollBarChanged( EventArgs.Empty );
					PerformLayout();
				}
			}
		}

		bool showHorizontalScrollBar = true;

		/// <summary>
		///     Toggles visibility of the Horizontal scrollbar.
		/// </summary>
		[Description( "Toggles visibility of the Horizontal scrollbar." )]
		[DefaultValue( true )]
		public virtual bool ShowHorizontalScrollBar
		{
			get
			{
				return showHorizontalScrollBar;
			}
			set
			{
				if( showHorizontalScrollBar != value )
				{
					showHorizontalScrollBar = value;
					OnShowHorizontalScrollBarChanged( EventArgs.Empty );
					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="ShowVerticalScrollBar"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ShowVerticalScrollBar property has changed." )]
		public event EventHandler ShowVerticalScrollBarChanged;


		/// <summary>
		/// Raises the <see cref="SplitterControl.ShowVerticalScrollBarChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnShowVerticalScrollBarChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ShowVerticalScrollBar );
#else
			;
#endif

			if( ShowVerticalScrollBarChanged != null )
				ShowVerticalScrollBarChanged( this, e );
		}

		/// <summary>
		/// Occurs when the <see cref="ShowHorizontalScrollBar"/> property has changed.
		/// </summary>
		[Description( "Occurs when the ShowHorizontalScrollBar property has changed." )]
		public event EventHandler ShowHorizontalScrollBarChanged;


		/// <summary>
		/// Raises the <see cref="SplitterControl.ShowHorizontalScrollBarChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnShowHorizontalScrollBarChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this, this.ShowHorizontalScrollBar );
#else
			;
#endif

			if( ShowHorizontalScrollBarChanged != null )
				ShowHorizontalScrollBarChanged( this, e );
		}

	}
}
