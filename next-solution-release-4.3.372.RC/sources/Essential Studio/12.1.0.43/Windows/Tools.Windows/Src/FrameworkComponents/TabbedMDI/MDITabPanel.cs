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
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// 
	/// </summary>
	public class MdiDropDownButton
	{
		#region Constants
		/// <summary>
		/// Minimum width of a MDIDropDownButton.
		/// </summary>
		private const int WIDTH = 16;
		/// <summary>
		/// Minimum height of a MDIDropDownButton.
		/// </summary>
		private const int HEIGHT = 15;
		/// <summary>
		/// Minimum indent for a MDIDropDownButton.
		/// </summary>
		private const int INDENT = 3;
		/// <summary>
		/// 
		/// </summary>
		private static readonly Color c_cHotBackColor = Color.FromArgb( 242, 210, 101 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Color c_cPressedBackColor = Color.FromArgb( 255, 238, 194 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Color c_cBorderColor = Color.FromArgb( 72, 72, 109 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Color c_cNormalForeColor = Color.Black;
		/// <summary>
		/// 
		/// </summary>
		private static readonly Color c_cPressedForeColor = Color.White;
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private bool m_bRightToLeft = false;
		/// <summary>
		/// 
		/// </summary>
		private Size m_szButton = new Size( WIDTH, HEIGHT );
		/// <summary>
		/// 
		/// </summary>
		private Rectangle m_rcBounds = Rectangle.Empty;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIsPressed = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIsHighlighted = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIsPushed = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIsScrollNeeded = false;
		/// <summary>
		/// 
		/// </summary>
		private TabAlignment m_mdiTabAlignment = TabAlignment.Top;
		#endregion

		#region Delegates
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Click;
		#endregion

		#region Properties
        #region Fields
		/// <summary>
		/// An instance of the MDITabPanel.
		/// </summary>
		private MDITabPanel m_MDITabPanel = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets an instance of MDITabPanel.
		/// </summary>
		public MDITabPanel MDITabPanel
		{
			get
			{
				return m_MDITabPanel;
			}
			set
			{
				m_MDITabPanel = value;
			}
		}
		#endregion
		/// <summary>
		/// Gets size of a button.
		/// </summary>
		public Size Size
		{
			get
			{
				return m_szButton;
			}
		}
		/// <summary>
		/// Gets location of a button.
		/// </summary>
		public Point Location
		{
			get
			{
				return m_rcBounds.Location;
			}
		}
		/// <summary>
		/// Gets or sets if button location is right to left.
		/// </summary>
		public bool RightToLeft
		{
			get
			{
				return m_bRightToLeft;
			}
			set
			{
				m_bRightToLeft = value;
			}
		}
		/// <summary>
		/// Gets button bounds.
		/// </summary>
		public Rectangle Bounds
		{
			get
			{
				return m_rcBounds;
			}
		}
		/// <summary>
		/// Indicates if button is pressed.
		/// </summary>
		public bool IsPressed
		{
			get
			{
				return m_bIsPressed;
			}
		}
		/// <summary>
		/// Gets button highlighted state.
		/// </summary>
		public bool IsHighlighted
		{
			get
			{
				return m_bIsHighlighted;
			}
		}
		/// <summary>
		/// Gets or sets button pushed state.
		/// </summary>
		public bool IsPushed
		{
			get
			{
				return m_bIsPushed;
			}
			set
			{
				m_bIsPushed = value;
			}
		}
		/// <summary>
		/// Gets or sets if scroll need mode is active of button.
		/// </summary>
		public bool IsScrollNeeded
		{
			get
			{
				return m_bIsScrollNeeded;
			}
			set
			{
				m_bIsScrollNeeded = value;
			}
		}
		/// <summary>
		/// Gets or sets a tab control alignment.
		/// </summary>
		public TabAlignment TabAlignment
		{
			get
			{
				return m_mdiTabAlignment;
			}
			set
			{
				m_mdiTabAlignment = value;
			}
		}
		/// <summary>
		/// Gets a value indicating whether the tabAlignment is set to right or left.
		/// </summary>
		public bool IsVerticalAlignment
		{
			get
			{
				return ( m_mdiTabAlignment == TabAlignment.Left || m_mdiTabAlignment == TabAlignment.Right );
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		public MdiDropDownButton()
		{
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabPanelBounds"></param>
		/// <returns></returns>
		internal RectangleF HandleAdjustTabPanelBounds( RectangleF pTabPanelBounds )
		{

			if( !this.IsVerticalAlignment )
			{
				pTabPanelBounds.Width -= m_szButton.Width + INDENT * 2;

				if( m_bRightToLeft )
				{
					pTabPanelBounds.Offset( m_szButton.Width + INDENT * 2, 0 );
				}
			}
			else
			{
				pTabPanelBounds.Height -= m_szButton.Height + INDENT * 2;

				if( m_bRightToLeft )
				{
					pTabPanelBounds.Offset( 0, m_szButton.Height + INDENT * 2 );
				}
			}

			return pTabPanelBounds;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		internal void HandleOnPaint( PaintEventArgs e )
		{
			Graphics g = e.Graphics;

			SmoothingMode smModeOld = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.None;

			DrawBackground( g );
			DrawForeground( g );

			g.SmoothingMode = smModeOld;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		internal bool HandleOnMouseDown( MouseEventArgs e )
		{
			bool bReturn = false;

			if( !m_bIsPressed && e.Button == MouseButtons.Left )
			{
				Point p = new Point( e.X, e.Y );

				if( m_rcBounds.Contains( p ) )
				{
					m_bIsPressed = true;
					bReturn = true;
				}
			}
			return bReturn;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		internal bool HandleOnMouseUp( MouseEventArgs e )
		{
			bool bReturn = false;

			if( m_bIsPressed )
			{
				Point p = new Point( e.X, e.Y );

				if( m_rcBounds.Contains( p ) )
				{
					m_bIsPushed = true;
					this.OnClick();
				}

				m_bIsPressed = false;
				bReturn = true;
			}

			return bReturn;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		internal bool HandleOnMouseMove( MouseEventArgs e )
		{
			bool bReturn = false;

			Point p = new Point( e.X, e.Y );

			if( m_rcBounds.Contains( p ) )
			{
				if( !m_bIsHighlighted )
				{
					m_bIsHighlighted = true;
					bReturn = true;
				}
			}
			else
			{
				if( m_bIsHighlighted )
				{
					m_bIsHighlighted = false;
					bReturn = true;
				}
			}

			return bReturn;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		internal bool HandleOnMouseLeave( EventArgs e )
		{
			bool bReturn = false;

			if( m_bIsHighlighted )
			{
				m_bIsHighlighted = false;
				bReturn = true;
			}

			return bReturn;
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnClick()
		{
			if( Click != null )
			{
				Click( this, new EventArgs() );
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		private void DrawBackground( Graphics g )
		{
			bool bDrawPressed = m_bIsPressed || m_bIsPushed;

			if( bDrawPressed || m_bIsHighlighted )
			{
                if (MDITabPanel != null && MDITabPanel is TabControlAdv && (MDITabPanel as TabControlAdv).TabStyle == typeof(TabRendererMetro))
                {
                    Color clFill = (bDrawPressed) ? (MDITabPanel as TabControlAdv).ActiveTabColor  :ControlPaint.Light((MDITabPanel as TabControlAdv).ActiveTabColor);

                    using (Brush brush = new SolidBrush(clFill))
                    {
                        g.FillRectangle(brush, m_rcBounds);
                    }
                }
                else
                {
                    Color clFill = (bDrawPressed) ? c_cHotBackColor : c_cPressedBackColor;

                    using (Brush brush = new SolidBrush(clFill))
                    {
                        g.FillRectangle(brush, m_rcBounds);
                    }
                    using (Pen pen = new Pen(c_cBorderColor))
                    {
                        Rectangle borderRect = new Rectangle(m_rcBounds.X, m_rcBounds.Y, m_rcBounds.Width - 1, m_rcBounds.Height - 1);
                        g.DrawRectangle(pen, borderRect);
                    }
                }
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		private void DrawForeground( Graphics g )
		{
			Rectangle rect = m_rcBounds;

			if( this.IsVerticalAlignment )
			{
				Matrix m = new Matrix();
				m.RotateAt( ( m_mdiTabAlignment == TabAlignment.Left ) ? -90 : 90,
					new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
				g.Transform = m;
			}

			Size szDrop = new Size( ( int ) rect.Width / 2, ( int ) rect.Width / 4 );
			Point pt = new Point( rect.X + ( ( rect.Height - szDrop.Width ) / 2 ) + 1, ( int ) rect.Y + ( rect.Height / 2 ) );

			using( GraphicsPath path = new GraphicsPath() )
			{
				int iHalfWidth = ( int ) szDrop.Width / 2;

				path.AddLine( iHalfWidth + pt.X, szDrop.Height + pt.Y, pt.X, pt.Y );
				path.AddLine( pt.X, pt.Y, szDrop.Width + pt.X, pt.Y );
				path.AddLine( szDrop.Width + pt.X, pt.Y, iHalfWidth + pt.X, szDrop.Height + pt.Y );

				using( Brush brush = new SolidBrush( ( this.IsPressed ) ? c_cPressedForeColor : c_cNormalForeColor ) )
				{
					g.FillPath( brush, path );
				}
			}

			if( m_bIsScrollNeeded )
			{
				Size szScroll = new Size( szDrop.Width, ( int ) szDrop.Height / 2 );
				Point ptScroll = new Point( pt.X, ( int ) rect.Y + ( rect.Height / 4 ) );

				using( Brush brush = new SolidBrush( ( this.IsPressed ) ? c_cPressedForeColor : c_cNormalForeColor ) )
				{
					g.FillRectangle( brush, new Rectangle( ptScroll, szScroll ) );
				}
			}

			g.Transform.Reset();
		}
		/// <summary>
		/// Recalculate position and size of button.
		/// </summary>
		/// <param name="clientRect"> Client rectangle. </param>
		public void Layout( Rectangle pClientRect )
		{
			Point pt = Point.Empty;

			if( !this.IsVerticalAlignment )
			{
				pt.X = pClientRect.Width - m_szButton.Width - INDENT;
				pt.Y = ( pClientRect.Height - m_szButton.Height ) / 2;

				if( m_bRightToLeft )
				{
					pt.X = pClientRect.Width - pt.X + INDENT;
				}
			}
			else
			{
				pt.X = ( pClientRect.Width - m_szButton.Width ) / 2;
				pt.Y = pClientRect.Height - m_szButton.Height - INDENT;

				if( m_bRightToLeft )
				{
					pt.Y = pClientRect.Height - pt.Y + INDENT;
				}
			}

			m_rcBounds = new Rectangle( pt, m_szButton );
		}
		#endregion
	}

	/// <summary>
	/// A Popup menu class with MDITabPanel's reference.
	/// </summary>
	[ToolboxItem( false )]
	class MdiDropDownPopupMenu : PopupMenu
	{
		#region Fields
		/// <summary>
		/// An instance of the MDITabPanel.
		/// </summary>
		private MDITabPanel m_MDITabPanel = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets an instance of MDITabPanel.
		/// </summary>
		public MDITabPanel MDITabPanel
		{
			get
			{
				return m_MDITabPanel;
			}
			set
			{
				m_MDITabPanel = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="askParent"></param>
		/// <returns></returns>
		public override bool IsRelatedControl( Control control, bool askParent )
		{
			if( m_MDITabPanel != null && control == m_MDITabPanel )
			{
				Point pt = m_MDITabPanel.PointToClient( Control.MousePosition );

				if( m_MDITabPanel.DropDownButton.Bounds.Contains( pt ) )
				{
					return true;
				}
			}

			return base.IsRelatedControl( control, askParent );
		}
		#endregion
	}

	/// <summary>
	/// The tab control used to render a tab group in a tabbed MDI UI managed by the 
	/// <see cref="TabbedMDIManager"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To access this tab control, override <see cref="TabbedMDIManager.CreateMDITabPanel"/> (you can provide
	/// a custom derived class, if necessary) 
	/// and/or <see cref="TabbedMDIManager.InitMDITabPanel"/>.</para>
	/// </remarks>
	[DesignTimeVisible( false ),
	ToolboxItem( false )]
	public class MDITabPanel : TabControlAdv, ITabbedMDIBarItemEvents
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		private const int DEF_LOCATION_OFFSET = 2;
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public const int CLOSE_BUTTON_AREA_WIDTH = 20;
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public const int CLOSE_BUTTON_AREA_HEIGHT = CLOSE_BUTTON_AREA_WIDTH - 4;
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		internal TabbedMDIManager m_MDIManager;
		/// <summary>
		/// 
		/// </summary>
		private int dragovercount = 0;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bSuspendChildActivation = false;
		/// <summary>
		/// 
		/// </summary>
		private MdiDropDownButton m_DropDownButton = null;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bCloseButtonHit = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bCloseButtonClicked = false;
		/// <summary>
		/// Drop down button popup menu.
		/// </summary>
		private MdiDropDownPopupMenu m_pmDropDownPopup = null;
		/// <summary>
		/// Determines if scrollbar buttons are needed.
		/// </summary>
		private bool m_bNeedScroll = false;
		/// <summary>
		/// 
		/// </summary>
		private ArrayList m_lstMDIChildForms = null;
        /// <summary>
        /// Hash table used for saving the state of the form before adding to Tabbed MDI.
        /// </summary>
        private Hashtable m_htMDIChildrenState;
		#endregion

		#region Event handlers
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.OnSelectedIndexChanged"/>.
		/// </summary>
		/// <param name="e">EventArgs that contains event data.</param>
		protected override void OnSelectedIndexChanged( EventArgs e )
		{
			base.OnSelectedIndexChanged( e );

			this.UpdateActiveTabFont();
			
			if (this.SelectedIndex != -1 && this.SelectedTab != null && !m_bSuspendChildActivation )
			{
				Form mdiChild = ( Form ) this.SelectedTab.Tag;

				// Customer reports that sometimes a mdi child's ContainsFocus
				// returns true even if it's not active, seems more like a bug in the
				// framework. However, we will ignore this check unless 
				// this breaks someother scenario.
				//if(!mdiChild.ContainsFocus)

				bool lockHostForm = Environment.OSVersion.Version.Major < 6;

				if(lockHostForm)
					this.m_MDIManager.LockHostFormUpdate();

				mdiChild.Activate();

				if (lockHostForm)
					this.m_MDIManager.UnlockHostFormUpdate();

				Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			if( this.DropDownButtonVisible )
			{
				this.DropDownButton.RightToLeft = ( this.RightToLeft == RightToLeft.Yes );
			}

			base.OnRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DropDownButton_Click( object sender, EventArgs e )
		{
			bool needShow = true;

			if( m_pmDropDownPopup == null )
			{
				this.InitDropDownButtonPopupMenu();
			}
			else if( m_pmDropDownPopup.IsShowing() )
			{
				m_pmDropDownPopup.Hide();
				needShow = false;
			}

			if( needShow )
			{
				this.FillDropDownButtonPopupMenu();

				Point pt = this.DropDownButton.Location;
				if( !this.IsVerticalAlignment )
				{
					pt.Y = this.DropDownButton.Bounds.Bottom + 1;

					if( this.IsMirrored )
					{
						pt.X += this.DropDownButton.Size.Width;
					}
				}
				else
				{
					pt.X = this.DropDownButton.Bounds.Right + 1;

					if( this.IsMirrored )
					{
						pt.Y += this.DropDownButton.Size.Height;
					}
				}

				DropDownPopupEventArgs eventArgs = new DropDownPopupEventArgs( m_pmDropDownPopup.ParentBarItem, pt );
				m_MDIManager.OnDropDownPopup( eventArgs );

				if( !eventArgs.Cancel )
				{
					m_pmDropDownPopup.Show( this, eventArgs.Location );
				}
				else if( this.DropDownButton.IsPushed )
				{
					this.DropDownButton.IsPushed = false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DropDown_PopupClosed( object sender, EventArgs e )
		{
			if( this.DropDownButton.IsPushed )
			{
				this.DropDownButton.IsPushed = false;
				this.Invalidate( this.DropDownButton.Bounds );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MDIChild_Clicked( object sender, EventArgs e )
		{
			if( sender is Form )
			{
				this.BringSelectedTabToView();
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets drop down button control.
		/// </summary>
		public MdiDropDownButton DropDownButton
		{
			get
			{
				if( m_DropDownButton == null )
				{
					m_DropDownButton = new MdiDropDownButton();
					m_DropDownButton.RightToLeft = ( this.RightToLeft == RightToLeft.Yes );
					m_DropDownButton.TabAlignment = this.Alignment;

					m_DropDownButton.Click += new EventHandler( DropDownButton_Click );
				}

				return m_DropDownButton;
			}
		}
		/// <summary>
		/// Gets drop down button control visibility.
		/// </summary>
		private bool DropDownButtonVisible
		{
			get
			{
				if( m_MDIManager != null )
				{
					return m_MDIManager.DropDownButtonVisible;
				}

				return false;
			}
		}

		/// <summary>
		/// Gets or sets if button for each Tab is shown.
		/// </summary>
		[Obsolete("Please, use ShowTabCloseButton property instead.")]
		public bool ShowCloseButton
		{
			get
			{
				return this.ShowTabCloseButton;
			}
			set
			{
				this.ShowTabCloseButton = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the tabAlignment is set to right or left.
		/// </summary>
		public bool IsVerticalAlignment
		{
			get
			{
				return ( Alignment == TabAlignment.Left || Alignment == TabAlignment.Right );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool CloseButtonHit
		{
			get
			{
				return m_bCloseButtonHit;
			}
			set
			{
				if( m_bCloseButtonHit != value )
				{
					m_bCloseButtonHit = value;

					if( m_MDIManager.CloseButtonVisible )
					{
						Invalidate( GetCloseButtonBounds(), false );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsCloseButtonActive()
		{
			// Disable button hit if the selected form's ControlBox is invisible
			Form frmSelected = this.GetSelectedForm();

			if( frmSelected != null )
				return frmSelected.ControlBox;
			else
				return false;
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool CloseButtonClicked
		{
			get
			{
				return m_bCloseButtonClicked;
			}
			set
			{
				if( m_bCloseButtonClicked != value )
				{
					m_bCloseButtonClicked = value;
					this.Invalidate( GetCloseButtonBounds(), false );
				}
			}
		}
        /// </override>
        public override Font ActiveTabFont
        {
            get
            {
                return base.ActiveTabFont;
            }
            set
            {
                if( this.TabPanelData.ActiveTabFont != value )
                {
                    this.TabPanelData.ActiveTabFont = value;
                    this.UpdateActiveTabFont();
                }
            }
        }
		#endregion

		#region Initialization
		/// <summary>
		/// Creates a new instance of the MDITabPanel class.
		/// </summary>
		/// <param name="manager">The corresponding <see cref="TabbedMDIManager"/> instance.</param>
		public MDITabPanel( TabbedMDIManager manager )
		{
			m_MDIManager = manager;
			//((MDITabPanelData)this.TabPanelData).manager = this.manager;
			m_lstMDIChildForms = new ArrayList();
            m_htMDIChildrenState = new Hashtable();
			this.TabStyle = typeof( TabRenderer2D );
			this.UserMoveTabs = true;
			this.AllowDrop = true;
			this.ShowToolTips = true;
			this.FocusOnTabClick = false;
			this.ShowTabCloseButton = manager.ShowCloseButton;
			this.ShowCloseButtonForActiveTabOnly = manager.ShowCloseButtonForActiveTabOnly;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_DropDownButton != null )
				{
					m_DropDownButton.Click -= new EventHandler( DropDownButton_Click );
				}

				if( m_pmDropDownPopup != null && m_pmDropDownPopup.ParentBarItem != null )
				{
					m_pmDropDownPopup.ParentBarItem.PopupClosed -= new EventHandler( DropDown_PopupClosed );
				}
			}

            this.TabPanelData.ImageList = null;
            this.TabPanelData = null;
			base.Dispose( disposing );
            GC.Collect(2, GCCollectionMode.Forced);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragLeave( EventArgs e )
		{
			this.dragovercount = 0;
			base.OnDragLeave( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			this.dragovercount++;

			Point pt = new Point( drgevent.X, drgevent.Y );
			pt = this.PointToClient( pt );

			if( this.dragovercount > 10 )
			{
				int tabIndex = this.HitTestTabs( pt );
				if( tabIndex != -1 )
				{
					if( this.SelectedIndex != tabIndex )
						this.SelectedIndex = tabIndex;
					else
						this.ActivateAndFocusSeletedChildForm();
				}
			}

			base.OnDragOver( drgevent );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rendererNew"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void RendererChanged( TabPanelRenderer rendererNew )
		{
			base.RendererChanged( rendererNew );
			SingleLineTabPanelRenderer renderer = this.Renderer as SingleLineTabPanelRenderer;
			if( renderer != null )
			{
				renderer.PadX = 5;
				renderer.PadY = 2;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		/// <param name="e">MouseEventArgs that contains the event data.</param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
            base.OnMouseMove( e );

			if( this.DropDownButtonVisible )
			{
				if( this.DropDownButton.HandleOnMouseMove( e ) )
				{
					this.Invalidate( this.DropDownButton.Bounds );
				}
			}

			if( GetCloseButtonBounds().Contains( new Point( e.X, e.Y ) )
				&& this.IsCloseButtonActive() )
				this.CloseButtonHit = true;
			else
				this.CloseButtonHit = false;
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		/// <param name="e">MouseEventArgs that contains the event data.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			bool bValidated = ( m_MDIManager != null && m_MDIManager.MdiParent != null &&
				m_MDIManager.MdiParent.Validate() );

			if( bValidated || !m_MDIManager.CausesFormValidation )
			{
				if( e.Button == MouseButtons.Left && this.GetCloseButtonBounds().Contains( new Point( e.X, e.Y ) )
					&& this.IsCloseButtonActive() && m_MDIManager.CloseButtonVisible )
				{
					this.CloseButtonClicked = true;
					//this.Invalidate(this.GetCloseButtonBounds());
					return;
				}

				if( this.DropDownButtonVisible )
				{
					if( this.DropDownButton.HandleOnMouseDown( e ) )
					{
						this.Invalidate( this.DropDownButton.Bounds );
					}
				}

				int selIndex = this.SelectedIndex;

				base.OnMouseDown( e );

				if( selIndex == this.SelectedIndex )
				{
					// If not the current active child.
					if( m_MDIManager.MdiParent.ActiveMdiChild != ( Form ) this.SelectedTab.Tag )
					{
						if( !m_MDIManager.CausesFormValidation || m_MDIManager.ValidateFocusedChildForm() )
							this.ActivateAndFocusSeletedChildForm();
					}
				}

				m_sbScrollButtons.Visible = !m_MDIManager.DropDownButtonVisible && m_bNeedScroll;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		/// <param name="e">EventArgs that contains the event data.</param>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );			

			this.CloseButtonHit = false;

			if( this.DropDownButtonVisible )
			{
				this.DropDownButton.HandleOnMouseLeave( e );
			}

		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		/// <param name="e">MouseEventArgs that contains the event data.</param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );
			// OnMouseUp could be called after this control gets disposed, if it is a right
			// mouse up that follows a context menu popup (in whose handler the control could have been
			// destroyed).
			if( this.IsHandleCreated )
			{
				bool oldClicked = this.CloseButtonClicked;
				this.CloseButtonClicked = false;

				if( this.DropDownButtonVisible )
				{
					if( this.DropDownButton.HandleOnMouseUp( e ) )
					{
						this.Invalidate( this.DropDownButton.Bounds );
					}
				}

				if( ( oldClicked && this.GetCloseButtonBounds().Contains( new Point( e.X, e.Y ) ) ) || ( m_MDIManager.CloseOnMiddleButtonClick && e.Button == MouseButtons.Middle && this.GetTabRect( this.SelectedIndex ).Contains( e.X, e.Y ) ) )
				{
					Form f = ( Form ) this.SelectedTab.Tag;

					m_MDIManager.LockMDIClientUpdate();
					m_MDIManager.CloseChildForm( f );
					m_MDIManager.UnLockMDIClientUpdate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool ValidateFocusedTab()
		{
			if( !m_MDIManager.CausesFormValidation )
				return true;
			else
				// Validate the form in this group as well as other groups, via the TabHost
				return m_MDIManager.ValidateFocusedChildForm();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabPanelBounds"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override RectangleF AdjustTabPanelBounds( RectangleF tabPanelBounds )
		{
			if( m_MDIManager.CloseButtonVisible )
			{
				if( this.IsVerticalAlignment )
				{
					tabPanelBounds.Height -= CLOSE_BUTTON_AREA_WIDTH;

					if( GetIsMirroredForVerticalAlignment() )
					{
						tabPanelBounds.Offset( 0, CLOSE_BUTTON_AREA_WIDTH );
					}
				}
				else
				{
					tabPanelBounds.Width -= CLOSE_BUTTON_AREA_WIDTH;

					if( GetIsMirroredForVerticalAlignment() )
					{
						tabPanelBounds.Offset( CLOSE_BUTTON_AREA_WIDTH, 0 );
					}
				}
			}

			if( this.DropDownButtonVisible )
			{
				tabPanelBounds = this.DropDownButton.HandleAdjustTabPanelBounds( tabPanelBounds );
			}

			return base.AdjustTabPanelBounds( tabPanelBounds );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabPanelBounds"></param>
		/// <param name="scrollNeeded"></param>
		protected override void AdjustScrollButtonDimensions( ref RectangleF tabPanelBounds, bool scrollNeeded )
		{
			if( this.DropDownButtonVisible )
			{
				this.DropDownButton.IsScrollNeeded = scrollNeeded;
				scrollNeeded = false;
			}

			m_bNeedScroll = scrollNeeded;

			base.AdjustScrollButtonDimensions( ref tabPanelBounds, scrollNeeded );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fromPaint"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void Layout( Graphics g, bool fromPaint )
		{
			SetNeedLayout( false );

			if( this.DropDownButtonVisible )
			{
				Rectangle rect = this.ClientRectangle;

				if( m_MDIManager.CloseButtonVisible )
				{
					if( !this.IsVerticalAlignment )
					{
						rect.Width -= CLOSE_BUTTON_AREA_WIDTH;
					}
					else
					{
						rect.Height -= CLOSE_BUTTON_AREA_WIDTH;
					}
				}

				this.DropDownButton.Layout( rect );
			}
            
			this.Renderer.Layout( g, fromPaint );

			switch( this.Alignment )
			{
				case TabAlignment.Bottom:
                    if( !this.BorderVisible )
					{
                        this.Location = new Point( 0, DEF_LOCATION_OFFSET );
                    }
					break;

				case TabAlignment.Right:
                    if( !this.BorderVisible )
                    {
                        this.Location = new Point( DEF_LOCATION_OFFSET, 0 );
                    }
					break;

				default:
					this.Location = Point.Empty;
					break;

			}

			ComputeTabPanelBounds();

			UpdateSelectedTabPage( true );
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		/// <param name="e">PaintEventArgs that contains the event data.</param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			if( m_MDIManager.CloseButtonVisible )
			{
				PaintCloseButton( e, m_MDIManager.CloseButtonColor );
			}
			if( this.DropDownButtonVisible )
			{
				DropDownButton.MDITabPanel = this;
				DropDownButton.HandleOnPaint( e );
			}
		}

        /// </override>
        protected override void SetRegion()
        {
            //do nothing here
        }
		#endregion

		#region Implementation
		/// <summary>
		/// Gets preferred size of a MDITabPanel.
		/// </summary>
		/// <returns></returns>
		internal SizeF GetMDITabPanelPreferredSize()
		{
			SizeF szPreferred = SizeF.Empty;

			using( Graphics g = CreateGraphics() )
			{
				Renderer.GetPreferredSize( g, ref szPreferred );
			}

			return szPreferred;
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void ForceLayout()
		{
			if( NeedLayout && this.IsHandleCreated )
			{
				Graphics g = this.CreateGraphics();
				this.Layout( g, false );
				g.Dispose();
			}
		}
		/// <summary>
		/// Indicates whether an mdi child Form is "hosted" within this tab control.
		/// </summary>
		/// <param name="mdiChild">An mdi child Form.</param>
		/// <returns>True if the Form is part of this tab control; false otherwise.</returns>
		public bool IsHosting( Form mdiChild )
		{
			foreach( TabPageAdv tabPage in this.TabPages )
			{
				if( tabPage.Tag == mdiChild )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Update close and drop down buttons.
		/// </summary>
		public void UpdateCloseAndDropDownButtons()
		{
			ComputeTabPositions();
			ComputeTabPanelBounds();
			InvalidatePanel();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Rectangle GetCloseButtonBounds()
		{
			Rectangle bounds = Rectangle.Empty;
			bool bIsMirrored = this.GetIsMirroredForVerticalAlignment();

			Rectangle panelBounds = this.ClientRectangle;
			Rectangle scrollButtonBounds = ( ScrollButtons == null || !ScrollButtons.Visible ) ?
				Rectangle.Empty : ScrollButtons.Bounds;

			if( this.IsVerticalAlignment )
			{
				panelBounds.Size = new Size( panelBounds.Height, panelBounds.Width );
			}

			int nCloseX = 0;
			if( bIsMirrored )
			{
				int scrollButtonsRight = scrollButtonBounds.Right;
				nCloseX = panelBounds.Left + scrollButtonsRight;
			}
			else
			{
				nCloseX = panelBounds.Right - CLOSE_BUTTON_AREA_WIDTH;
			}

			bounds = new Rectangle( nCloseX, panelBounds.Top,
				CLOSE_BUTTON_AREA_WIDTH, panelBounds.Height );

			// Adjust width
			bounds.Offset( 2, 0 );
			bounds.Width -= 4;
			// Adjust Height
			bounds.Offset( 0, ( bounds.Height - CLOSE_BUTTON_AREA_HEIGHT ) / 2 );
			bounds.Height -= ( bounds.Height - CLOSE_BUTTON_AREA_HEIGHT );

			if( this.IsVerticalAlignment )
			{
				bounds = new Rectangle( bounds.Y, bounds.X,
					bounds.Height, bounds.Width );
			}

			if( bounds.Width < 0 )
				bounds.Width = 0;
			if( bounds.Height < 0 )
				bounds.Height = 0;
			return bounds;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="buttonColor"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void PaintCloseButton( PaintEventArgs e, Color buttonColor )
		{
			if( !IsCloseButtonActive() )
				return;

			Graphics gfxPaint = e.Graphics;

			gfxPaint.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle bounds = this.GetCloseButtonBounds();

			using( CMirroredDrawer mdDrawer = new CMirroredDrawer( gfxPaint, bounds, GetIsMirroredForVerticalAlignment() ) )
			{
				Graphics gfxCanvas = mdDrawer.VirtualGfx;
				Rectangle rectCanvas = mdDrawer.VirtualBounds;

				// Paint the CloseButtonArea
				if( bounds.Height > 0 && bounds.Width > 0 )
				{
					if( !this.IsOffice2003Style && !this.IsOneNoteStyle && !this.IsOneNoteStyleFlatTabs )
					{
                        using(Brush brush =new SolidBrush( this.TabPanelBackColor ))
                            gfxCanvas.FillRectangle(brush, rectCanvas);
					}
				}

				// Paint the X button
				if( this.CloseButtonHit == true && bounds.Width != 0 & bounds.Height != 0 )
				{
					Color clDarkDark = Color.Empty;
					Color clDark = Color.Empty;
					Color clLightLight = Color.Empty;
					Color clLight = Color.Empty;
                    if (!this.IsMetroStyle)
                    {
                        if (this.CloseButtonClicked)
                        {
                            clDarkDark = SystemColors.ControlDarkDark;
                            clDark = SystemColors.ControlDark;
                            clLightLight = SystemColors.ControlLight;
                            clLight = SystemColors.ControlLightLight;
                        }
                        else
                        {
                            clDarkDark = SystemColors.ControlLightLight;
                            clDark = SystemColors.ControlLight;
                            clLightLight = SystemColors.ControlDark;
                            clLight = SystemColors.ControlDarkDark;
                        }
                        using (Pen penDarkDark = new Pen(clDarkDark))
                        {
                            gfxCanvas.DrawLine(penDarkDark, new Point(rectCanvas.X, rectCanvas.Y), new Point(rectCanvas.Right - 2, rectCanvas.Y));
                            gfxCanvas.DrawLine(penDarkDark, new Point(rectCanvas.X, rectCanvas.Y), new Point(rectCanvas.X, rectCanvas.Bottom - 2));
                        }

                        using (Pen penDark = new Pen(clDark))
                        {
                            gfxCanvas.DrawLine(penDark, new Point(rectCanvas.X + 1, rectCanvas.Y + 1), new Point(rectCanvas.Right - 3, rectCanvas.Y + 1));
                            gfxCanvas.DrawLine(penDark, new Point(rectCanvas.X + 1, rectCanvas.Y + 1), new Point(rectCanvas.X + 1, rectCanvas.Bottom - 3));
                        }

                        using (Pen penLight = new Pen(clLightLight))
                        {
                            gfxCanvas.DrawLine(penLight, new Point(rectCanvas.X + 1, rectCanvas.Bottom - 2), new Point(rectCanvas.Right - 2, rectCanvas.Bottom - 2));
                            gfxCanvas.DrawLine(penLight, new Point(rectCanvas.Right - 2, rectCanvas.Y + 1), new Point(rectCanvas.Right - 2, rectCanvas.Bottom - 2));
                        }

                        using (Pen penLightLight = new Pen(clLight))
                        {
                            gfxCanvas.DrawLine(penLightLight, new Point(rectCanvas.X, rectCanvas.Bottom - 1), new Point(rectCanvas.Right - 1, rectCanvas.Bottom - 1));
                            gfxCanvas.DrawLine(penLightLight, new Point(rectCanvas.Right - 1, rectCanvas.Y), new Point(rectCanvas.Right - 1, rectCanvas.Bottom - 1));
                        }
                        if (this.CloseButtonClicked)
                        {
                            rectCanvas.Offset(1, 1);
                        }
                    }
                    else
                    {
                        if (this.CloseButtonClicked)
                        {
                            using (Brush brush = new SolidBrush(this.ActiveTabColor))
                                gfxPaint.FillRectangle(brush, bounds);
                        }
                        else
                        {
                            using (SolidBrush brush = new SolidBrush(ControlPaint.Light(this.ActiveTabColor)))
                                gfxPaint.FillRectangle(brush, bounds);
                        }
                    }                   
                }

				Color btnColor = Color.Black;
				if( !this.IsCloseButtonActive() )
				{
					btnColor = SystemColors.GrayText;
				}
				else
				{
					btnColor = buttonColor;
				}

                if (m_MDIManager.ShowCloseButtonBackColor)
                {
                    SolidBrush brush = new SolidBrush(m_MDIManager.CloseButtonBackColor );
                    e.Graphics.FillRectangle(brush, bounds);
                    brush.Dispose();
                }
				if( bounds.Width != 0 && bounds.Height != 0 )
				{
					Pen pen = new Pen( btnColor, 1 );
					gfxCanvas.DrawLine( pen, rectCanvas.Left + 4, rectCanvas.Top + 4,
						rectCanvas.Right - 5, rectCanvas.Bottom - 5 );
					gfxCanvas.DrawLine( pen, rectCanvas.Right - 5, rectCanvas.Top + 4,
						rectCanvas.Left + 4, rectCanvas.Bottom - 5 );
                    pen.Dispose();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void ActivateAndFocusSeletedChildForm()
		{
			// Check to see if we need to activate the mdi child doc.
			Form mdiChild = ( Form ) this.SelectedTab.Tag;
			mdiChild.Activate();

			if( !mdiChild.ContainsFocus && mdiChild.IsHandleCreated )	// Form.Activate() fails to set the focus if the child is already active
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.SetFocus( mdiChild.Handle );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateTabBasedOnFormOnFront()
		{
			if( this.TabCount == 0 || m_MDIManager.m_mdiClient == null )
			{
				return;
			}

			TabPageAdv topTab = null;
			int topMostZ = Int32.MaxValue;

			foreach( TabPageAdv tabPage in this.TabPages )
			{
				Form mdiChild = ( Form ) tabPage.Tag;
				int z = m_MDIManager.m_mdiClient.Controls.IndexOf( mdiChild );
				// Find the child Form on top of the Controls list:
				if( z < topMostZ )
				{
					topMostZ = z;
					topTab = tabPage;
				}
			}

			if( topTab != null )
			{
				this.SuspendChildActivation();
				// Update the selected-tab based on the form on top:
				this.SelectedTab = topTab;
				this.ResumeChildActivation();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void SuspendChildActivation()
		{
			m_bSuspendChildActivation = true;
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void ResumeChildActivation()
		{
			m_bSuspendChildActivation = false;
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void DetachMdiChildren()
		{
			foreach( Form form in m_lstMDIChildForms )
			{
				RemoveMdiChild( form, false );
				m_MDIManager.RemoveMDIChildFromHash( form, false );
			}

			m_lstMDIChildForms.Clear();
            m_htMDIChildrenState.Clear();
		}
		/// <summary>
		/// Adds an mdi child Form to this tab control (tab group).
		/// </summary>
		/// <param name="mdiChild">The Form to add.</param>
		/// <param name="prevTabData">Settings for the tab.</param>
		/// <remarks>
		/// <para>
		/// This method will be called to add an mdi child Form to the tab group represented
		/// by this tab control.
		/// </para>
		/// <para>
		/// To access or provide a custom tab page, override the <see cref="CreateTab"/> method.
		/// </para>
		/// </remarks>
		protected internal virtual void AddMdiChild( Form mdiChild, MDIChildTabData prevTabData )
		{
			// Child already initialized.
			if( m_lstMDIChildForms.Contains( mdiChild ) )
				return;

			// Create a new tab corresponding to this mdi child.
			MDIChildTabData tabData = new MDIChildTabData( this.TabPanelData, mdiChild, m_MDIManager, prevTabData );

			if( mdiChild.Icon != null )
			{
				tabData.Icon = mdiChild.Icon;
			}

			TabPageAdv tabPage = this.CreateTab( tabData, this.CurDefaultTabPanelProperties );
			tabPage.Tag = mdiChild;

			if (this.m_MDIManager.childForms.Contains(mdiChild))
			tabPage.ShowCloseButton = true;
			mdiChild.Click += new EventHandler( this.MDIChild_Clicked );

			this.TabPages.Add( tabPage );

			// Change the form styles to make it look like maximized.
            this.ChangeFormStyles( mdiChild, true );
			m_lstMDIChildForms.Add( mdiChild );

			this.SelectedTab = tabPage;
		}
		/// <summary>
		/// Creates a new tab in the tab group.
		/// </summary>
		/// <param name="tabData">The data for this tab.</param>
		/// <param name="defaultProps">Default properties of this tab.</param>
		/// <returns>A <see cref="TabPageAdv"/> instance.</returns>
		protected virtual TabPageAdv CreateTab( MDIChildTabData tabData, ITabPanelDefaultProperties defaultProps )
		{
			return new TabPageAdv( tabData, defaultProps );
		}
		/// <summary>
		/// Removes the tab page corresponding to the mdi child Form.
		/// </summary>
		/// <param name="mdiChild">The mdi child Form.</param>
		/// <param name="removeFromHashtable">Internal flag.</param>
		/// <returns>True if successfully removed; false otherwise.</returns>
		protected internal virtual bool RemoveMdiChild( Form mdiChild, bool pRemoveFromHashtable )
		{
			if( !m_lstMDIChildForms.Contains( mdiChild ) )
				return false;

            this.ChangeFormStyles( mdiChild, false );

            TabPageAdv tabPage = this.GetTabPageAdvFromForm( mdiChild );

			if( tabPage != null )
			{
				this.TabPages.Remove( tabPage );
				tabPage.Dispose();
			}

			if( pRemoveFromHashtable )
			{
				m_lstMDIChildForms.Remove( mdiChild );
                m_htMDIChildrenState.Remove( mdiChild );
			}

			mdiChild.Click -= new EventHandler( this.MDIChild_Clicked );

			return true;
		}
		/// <summary>
		/// Returns the <see cref="TabPageAdv"/> of a given mdi child Form.
		/// </summary>
		/// <param name="mdiChild">The mdi child Form.</param>
		/// <returns>The <see cref="TabPageAdv"/> instance associated with this Form. Can be null.</returns>
		public TabPageAdv GetTabPageAdvFromForm( Form mdiChild )
		{
			foreach( TabPageAdv tabPage in TabPages )
			{
				if( tabPage.Tag == mdiChild )
					return tabPage;
			}
			return null;
		}
		/// <summary>
		/// Returns the selected mdi child Form in this tab group.
		/// </summary>
		/// <returns>A child <see cref="System.Windows.Forms.Form"/> instance.</returns>
		public Form GetSelectedForm()
		{
			return this.SelectedTab != null ? this.SelectedTab.Tag as Form : null;
		}
        /// <summary>
		/// 
		/// </summary>
		/// <param name="mdiChild"></param>
		/// <param name="bSet"></param>
		private void ChangeFormStyles( Form mdiChild, bool bSet )
		{
			if( bSet )
			{
				MDIChildState oldState = new MDIChildState( mdiChild.FormBorderStyle,
					mdiChild.ControlBox, mdiChild.HelpButton, mdiChild.MaximizeBox,
					mdiChild.MinimizeBox, mdiChild.SizeGripStyle,
					mdiChild.Dock, mdiChild.StartPosition );

				m_htMDIChildrenState[ mdiChild ] = oldState;

				mdiChild.SuspendLayout();
				mdiChild.StartPosition = FormStartPosition.Manual;
				mdiChild.FormBorderStyle = FormBorderStyle.None;
				//mdiChild.ControlBox = false;
				mdiChild.HelpButton = false;
				mdiChild.MaximizeBox = false;
				mdiChild.MinimizeBox = false;
				mdiChild.SizeGripStyle = SizeGripStyle.Hide;
				mdiChild.Dock = DockStyle.None;	// Setting to Top is preferrable but the 
				// position depends on the child index
				// which gets modified as an mdi child gets activated.
				mdiChild.ResumeLayout( false );
			}
			else
			{
				MDIChildState oldState = m_htMDIChildrenState[ mdiChild ] as MDIChildState;

				if( oldState == null )
				{
					return;
				}

				mdiChild.StartPosition = oldState.StartPosition;
				mdiChild.FormBorderStyle = oldState.FormBorderStyle;
				mdiChild.ControlBox = oldState.ControlBox;
				mdiChild.HelpButton = oldState.HelpButton;
				mdiChild.MaximizeBox = oldState.MaximizeBox;
				mdiChild.MinimizeBox = oldState.MinimizeBox;
				mdiChild.SizeGripStyle = oldState.SizeGripStyle;
				mdiChild.Dock = oldState.Dock;
			}
		}
		/// <summary>
		/// Initialize drop down button popup menu.
		/// </summary>
		private void InitDropDownButtonPopupMenu()
		{
			m_pmDropDownPopup = new MdiDropDownPopupMenu();
			m_pmDropDownPopup.MDITabPanel = this;
			ParentBarItem pbi = new ParentBarItem();
			pbi.PopupClosed += new EventHandler( DropDown_PopupClosed );

			m_pmDropDownPopup.ParentBarItem = pbi;
		}
		/// <summary>
		/// Fill drop down button popup menu.
		/// </summary>
		private void FillDropDownButtonPopupMenu()
		{
			ParentBarItem pbi = m_pmDropDownPopup.ParentBarItem;
			if( pbi == null )
			{
				pbi = new ParentBarItem();
			}
			else
			{
				pbi.Items.Clear();
			}

			foreach( TabPageAdv tp in this.TabPages )
			{
				TabbedMDIBarItem bi = new TabbedMDIBarItem( this, tp.Text,
					this.ImageList, tp.ImageIndex, tp.Enabled, tp );

				pbi.Items.Add( bi );
			}
		}
		#endregion

		#region ***TabbedMDIBarItem
		/// <summary>
		/// 
		/// </summary>
		class TabbedMDIBarItem : BarItem
		{
			#region Fields
			/// <summary>
			/// 
			/// </summary>
			private ITabbedMDIBarItemEvents m_events = null;
			#endregion

			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			protected TabbedMDIBarItem()
			{
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="events"></param>
			/// <param name="text"></param>
			/// <param name="imageList"></param>
			/// <param name="imageIndex"></param>
			/// <param name="enabled"></param>
			/// <param name="tabPage"></param>
			public TabbedMDIBarItem( ITabbedMDIBarItemEvents events, string pText,
				ImageList pImageList, int pImageIndex, bool pEnabled, TabPageAdv pTabPage )
			{
				this.Text = pText;
				this.ImageList = pImageList;
				this.ImageIndex = pImageIndex;
				this.Enabled = pEnabled;
				this.Tag = pTabPage;

				m_events = events;
			}
			#endregion

			#region Overrides
			protected override void OnItemClicked( EventArgs args )
			{
				m_events.OnClick( this );

				base.OnItemClicked( args );
			}
			#endregion
		}
		#endregion

		#region ITabbedMDIBarItemEvents
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		void ITabbedMDIBarItemEvents.OnClick( object sender )
		{
			TabbedMDIBarItem mdiBarItem = sender as TabbedMDIBarItem;

			if( mdiBarItem != null )
			{
				TabPageAdv tpPage = mdiBarItem.Tag as TabPageAdv;

				if( tpPage != null )
				{
					this.SelectedTab = tpPage;
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Represents the <see cref="TabData"/> associated with each tab in a tabbed MDI tab control (tab group).
	/// </summary>
	public class MDIChildTabData : TabData
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private Form m_mdiChild;
		/// <summary>
		/// 
		/// </summary>
		private ITabPanelData m_TabPanelData;
		/// <summary>
		/// 
		/// </summary>
		private TabbedMDIManager m_mdiManager;
		/// <summary>
		/// 
		/// </summary>
		private Icon m_icon = null;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the mdi child form associated with this tab.
		/// </summary>
		public Form MdiChild
		{
			get
			{
				return m_mdiChild;
			}
		}
		/// <summary>
		/// Gets or sets the text for the tab.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This override, always returns the associated form's Text property.
		/// </para>
		/// </remarks>
		public override string Text
		{
			get
			{
				if( m_mdiChild is ITabbedMDIChildForm )
				{
					bool bValidValueAvailable = false;

					ITabbedMDIChildForm mdiChildForm = m_mdiChild as ITabbedMDIChildForm;
					string sText = mdiChildForm.GetCustomTabText( out bValidValueAvailable );

					if( bValidValueAvailable )
					{
						return sText;
					}
				}

				return m_mdiChild.Text;
			}
			set
			{
				if( value != m_mdiChild.Text )
				{
					m_mdiChild.Text = ( value == null ) ? string.Empty : value;
				}
			}
		}
		/// <summary>
		/// Gets or Sets icon to be displayed.
		/// </summary>
		public Icon Icon
		{
			get
			{
				return ( m_mdiManager.UseIconsInTabs ) ? m_icon : null;
			}
			set
			{
				if( value != m_icon )
				{
					m_icon = value;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="TabData.ImageIndex"/>.
		/// </summary>
		public override int ImageIndex
		{
			get
			{
				if( m_mdiManager.UseIconsInTabs || m_mdiManager.shouldKeepImageIndex )
				{
					return base.ImageIndex;
				}
				else
				{
					return -1;
				}
			}
			set
			{
				base.ImageIndex = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates a new instance of the MDIChildTabData.
		/// </summary>
		/// <param name="tabPanelData">The tab panel data. </param>
		/// <param name="mdiChild"> The corresponding child form. </param>
		/// <param name="manager"> The corresponding <see cref="TabbedMDIManager"/></param>
		public MDIChildTabData( ITabPanelData pTabPanelData, Form pMdiChild, TabbedMDIManager pMdiManager, MDIChildTabData pPrevTabData )
		{
			m_TabPanelData = pTabPanelData;
			m_mdiChild = pMdiChild;
			m_mdiManager = pMdiManager;

			CopyFrom( pPrevTabData );

			m_mdiChild.GotFocus += new EventHandler( MDIChild_Focused );
			m_mdiChild.TextChanged += new EventHandler( MDIChild_TextChanged );
		}
		/// <summary>
		/// Creates a new instance of the MDIChildTabData.
		/// </summary>
		/// <param name="pPrevTabData"></param>
		public MDIChildTabData( MDIChildTabData pPrevTabData )
		{
			m_TabPanelData = pPrevTabData.m_TabPanelData;
			m_mdiChild = pPrevTabData.m_mdiChild;
			m_mdiManager = pPrevTabData.m_mdiManager;

			CopyFrom( pPrevTabData );
		}
		/// <summary>
		/// Creates a new instance of the MDIChildTabData.
		/// </summary>
		public MDIChildTabData()
		{
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MDIChild_TextChanged( object sender, EventArgs e )
		{
			OnBoundsAffected();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MDIChild_Focused( object sender, EventArgs e )
		{
			m_TabPanelData.SelectedIndex = m_TabPanelData.TabsData.IndexOf( this );
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pPrevTabData"></param>
		protected void CopyFrom( MDIChildTabData pPrevTabData )
		{
			if( pPrevTabData != null )
			{
				m_icon = pPrevTabData.m_icon;
			}

			base.CopyFrom( pPrevTabData );
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_mdiManager = null;
				m_TabPanelData = null;
				m_icon = null;

				if( m_mdiChild != null )
				{
					m_mdiChild.GotFocus -= new EventHandler( MDIChild_Focused );
					m_mdiChild.TextChanged -= new EventHandler( MDIChild_TextChanged );
					m_mdiChild = null;
				}
			}

			base.Dispose( disposing );
		}
		#endregion
	}

    /// <summary>
	/// 
	/// </summary>
	internal class MDIChildState
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private FormBorderStyle m_BorderStyle;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bControlBox;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bHelpButton;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bMaximizeBox;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bMinimizeBox;
		//internal bool showInTaskBar;
		/// <summary>
		/// 
		/// </summary>
		private SizeGripStyle m_szGripStyle;
		/// <summary>
		/// 
		/// </summary>
		private DockStyle m_dockStyle;
		/// <summary>
		/// 
		/// </summary>
		private FormStartPosition m_frmStartPosition;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		internal FormBorderStyle FormBorderStyle
		{
			get
			{
				return m_BorderStyle;
			}
			set
			{
				m_BorderStyle = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool ControlBox
		{
			get
			{
				return m_bControlBox;
			}
			set
			{
				m_bControlBox = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool HelpButton
		{
			get
			{
				return m_bHelpButton;
			}
			set
			{
				m_bHelpButton = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool MaximizeBox
		{
			get
			{
				return m_bMaximizeBox;
			}
			set
			{
				m_bMaximizeBox = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool MinimizeBox
		{
			get
			{
				return m_bMinimizeBox;
			}
			set
			{
				m_bMinimizeBox = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal SizeGripStyle SizeGripStyle
		{
			get
			{
				return m_szGripStyle;
			}
			set
			{
				m_szGripStyle = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal DockStyle Dock
		{
			get
			{
				return m_dockStyle;
			}
			set
			{
				m_dockStyle = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal FormStartPosition StartPosition
		{
			get
			{
				return m_frmStartPosition;
			}
			set
			{
				m_frmStartPosition = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pFrmBorderStyle"></param>
		/// <param name="pControlBox"></param>
		/// <param name="pHelpButton"></param>
		/// <param name="pMaximizeBox"></param>
		/// <param name="pMinimizeBox"></param>
		/// <param name="pSizeGripStyle"></param>
		/// <param name="pDockStyle"></param>
		/// <param name="pFrmStartPosition"></param>
		internal MDIChildState( FormBorderStyle pFrmBorderStyle, bool pControlBox, bool pHelpButton,
			bool pMaximizeBox, bool pMinimizeBox, SizeGripStyle pSizeGripStyle,
			DockStyle pDockStyle, FormStartPosition pFrmStartPosition )
		{
			m_BorderStyle = pFrmBorderStyle;
			m_bControlBox = pControlBox;
			m_bHelpButton = pHelpButton;
			m_bMaximizeBox = pMaximizeBox;
			m_bMinimizeBox = pMinimizeBox;
			m_szGripStyle = pSizeGripStyle;
			m_dockStyle = pDockStyle;
			m_frmStartPosition = pFrmStartPosition;
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public interface ITabbedMDIBarItemEvents
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		void OnClick( object sender );
	}
}