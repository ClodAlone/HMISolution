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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Text;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Represents the method that will handle the <see cref="BarItem.DrawToolbarItem"/> event of the <see cref="BarItem"/> class.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="drawItemInfo">A <see cref="DrawToolbarItemEventArgs"/> that contains the event data.</param>
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void DrawToolbarItemEventHandler( object sender, DrawToolbarItemEventArgs drawItemInfo );

	[Syncfusion.Documentation.DocumentationExclude()]
	public class DrawToolbarItemEventArgs: DrawItemEventArgs
	{
		private bool mousedown = false;
		private bool dropdown = false;
		private Color bgColor2 = Color.Empty;
		public delegate void DrawDefaultBackground( DrawToolbarItemEventArgs drawItemInfo, bool useThemes );
		public delegate void DrawDefaultBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes );
		public delegate void DrawDefaultInterior( DrawToolbarItemEventArgs drawItemInfo );

		[Syncfusion.Documentation.DocumentationExclude()]
		internal DrawDefaultBackground defaultDrawBackground;
		[Syncfusion.Documentation.DocumentationExclude()]
		internal DrawDefaultBorders defaultDrawBorders;
		[Syncfusion.Documentation.DocumentationExclude()]
		internal DrawDefaultInterior defaultDrawInterior;

		public DrawToolbarItemEventArgs( Graphics g,
			bool mouseDown, bool dropdown,
			Font font, Rectangle bounds, int index,
			DrawItemState state, Color foreColor, Color backColor, Color backColor2,
			Rectangle boundsInterior,
			DrawDefaultBackground defaultDrawBackground,
			DrawDefaultBorders defaultDrawBorders,
			DrawDefaultInterior defaultDrawInterior )
			: base( g, font, bounds, index, state, foreColor, backColor )
		{
			this.defaultDrawBackground = defaultDrawBackground;
			this.defaultDrawBorders = defaultDrawBorders;
			this.defaultDrawInterior = defaultDrawInterior;

			//			this.backColor = backColor;
			//			this.foreColor = foreColor;
			this.rectInterior = boundsInterior;
			this.mousedown = mouseDown;
			this.dropdown = dropdown;
			this.bgColor2 = backColor2;
		}

		private Rectangle rectInterior;
		public Rectangle BoundsInterior
		{
			get { return rectInterior; }
		}

		public Color BackColor2
		{
			get { return this.bgColor2; }
		}

		public bool MouseDown
		{
			get { return this.mousedown; }
		}

		public bool DropDown
		{
			get { return this.dropdown; }
		}

		public override void DrawBackground()
		{
			if( defaultDrawBackground != null )
				defaultDrawBackground( this, true );
		}
		public void DrawBorders()
		{
			if( this.defaultDrawBorders != null )
				defaultDrawBorders( this, true );
		}
		public void DrawInterior()
		{
			if( this.defaultDrawInterior != null )
				defaultDrawInterior( this );
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IToolbarControl
	{
		// A temporary workaround to deactivate the comboboxbaritem
		void OnMouseDownOutside();
	}
	interface IBarItemContainerControl
	{
		Rectangle GetBoundsOf( BarItem item );
		BarItem HitTest( int x, int y );
		AccessibleStates GetAccessibilityState( BarItem item );
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarControl: IThemedControl
	{
		void OnBarBoundsAffected();
		void OnRepaint( RectangleF affectedRect );
		Graphics GetGraphics();
		Control GetControl();
		bool DesignMode { get; }
		ThemedToolBarDrawing ThemedDrawing { get; }
		bool ShouldPreProcessTab();
		bool NeedKey( Keys key );
		bool UseControlForeColor { get; }
		bool LargeIcons { get; }
		bool IsRightToLeft { get; }
		bool RequiresActiveFormForMouseTrack();
		bool ShouldDelegateBGDrawingToParentWhenThemed();
		IBarHost BarHost { get; }
		VisualStyle Style { get; set; }
		IPopupParent PopupParent { get; }
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarRenderer: IPopupChild, IPopupParent, IDisposable
	{
		void StartKeyboardNavigation();
		void StopKeyboardNavigation();
		void OnFormDeactivated();
		bool IsKeyboardNavigationOn();
		bool CanStartKeyboardNavigation();
		bool HintViaHotKeyPrefix { get; set; }
		CommandBarDockState Alignment { get; set; }
		void Layout( IGraphicsProvider gs );
		Bar Bar { get; set; }
		bool Customizing { get; }
		bool DndCustomizing { get; }
		RectangleF Bounds { get; set; }
		bool ThemesEnabled { get; set; }
		VisualStyle Style { get; set; }
		bool LargeIcons { get; }
		bool IsShowingDropdown();
		void GetPreferredSize( IGraphicsProvider gp, ref SizeF size );
		void InvalidateCachedTextSizes();
		Rectangle GetBarItemBounds( int i );
		void OnPaint( Graphics g, Rectangle clipRect );
		int HitTestBarItems( PointF mousePosition );
		void OnMouseMove( MouseEventArgs e );
		void OnMouseLeave( EventArgs e );
		bool OnMouseDown( MouseEventArgs e );
		void OnMouseUp( MouseEventArgs e );
		void OnMouseWheel( MouseEventArgs e );
		bool ProcessMnemonic( char charCode );
		bool ProcessShortcut( char c );
		void OnDragOver( DragEventArgs drgevent );
		void OnDragDrop( System.Windows.Forms.DragEventArgs e );
		void OnDragLeave( EventArgs e );
		void OnGiveFeedback( GiveFeedbackEventArgs gfbevent );
		bool ProcessCmdKey( ref Message msg, Keys keyData );
		void ComputeBarItemPositions( IGraphicsProvider gp );
		IBarControl GetBarControl();
		void SetHotTrack( BarItemRenderer renderer, bool hotTrack );
		void RemoveRenderer( IBarItemRenderer renderer );
		void UpdateRenderers();
		BarItem DelayedPerformClickOnBarItem { get; set; }
		PopupRelativeAlignment GetFirstPopupAlignPreference();
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarItemRenderer: IDisposable
	{
		SizeF GetPreferredSize( IGraphicsProvider gp );
		void InvalidateCachedTextSizes();
		BarItem BarItem { get; set; }
		RectangleF Bounds { get; set; }
		bool NeedCenterVAlign { get; }
		bool HotTrack { get; set; }
		bool Active { get; set; }
		bool Visible { get; set; }
		bool ShowingDropDown { get; }
		bool ThemesEnabled { get; set; }
		VisualStyle Style { get; set; }
		void DropDown( bool show, bool setDefaultSelection, Queue pbiQueue );
		bool HitTest( PointF mousePosition );
		bool ShouldDrawText();
		bool ProcessKeyDown( Keys key );
		void OnMouseUp( Point pointMouseUp );
		void OnMouseDown( Point pointMouseDown );
		void OnMouseMove( Point pointMouseMove );
		void OnMouseWheel( bool isUp );
		void OnPaint( Graphics g, Rectangle clipRect );
		void DrawSeparator( Graphics g );
		void BarItemPropertyChanged( Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e );
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDropDownItem
	{
		void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType );
		Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign );
		Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlign );
		Control GetPopupParentControl();
		void AfterChildClosing();
		bool IsRelatedControl( Control ctl );
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public abstract class BarRenderer: IBarRenderer, IDndTrackingControl,
		IMouseHookHLProcClient, IKeyboardProcHookClient, IDisposable, IMessageFilter
	{
		#region PRIVATE_MEMBERS
		private bool m_bNeedDropDown = false;
		private bool m_bIsKeyboardNavigation = false;
		private bool m_bActivateOnExit = false;
		private bool m_bNeedStop = true;
		private CustomizingPopupMenu customizationPopup;
		internal CustomizationDndHelper dndHelper;
        internal ArrayList barItemRenderers;
		private bool needLayout = false;
		protected RectangleF tdbounds;
		protected ToolTip tooltip;
		protected IBarControl parent;
		private Bar bar;
		protected CommandBarDockState barAlignment = CommandBarDockState.None;
		protected bool ignoreMouseMove = false;
		protected int ignoreMouseMoveTick = int.MinValue;
		private bool ignoreMouseUp = false;
		internal IntPtr lastActiveWindow = IntPtr.Zero;
		protected int currentDndDropDown = -1;
		private BarItems invisibleBarItems = null;
		private MouseProcHookerUtil mouseHooker = null;
		private EfficientMenuTracker efficentTracker = null;
		private Point mouseDownPoint = Point.Empty;
		private bool themesEnabeld = false;
		private BarItem delayedPerformClickBarItem = null;
		private bool hintViaHotKeyPrefix = false;
		private bool m_bKeyboardNavigationStartedByMouse = false;
		private VisualStyle style = VisualStyle.OfficeXP;
		/// <summary>
		/// Indicates if BarRenderer is in dragging mode
		/// </summary>
		private bool m_bDragging = false;

		protected float padX = 2.0F;
		protected float padY = 2.0F;
		protected float separatorAreaX = 6.0F;

		private bool m_bSuppressIsMenuClickingChange = false;
		#endregion PRIVATE_MEMBERS
		#region MEMBERS_ITEM_POSITION_AND_COUNT_SENSITIVE
		protected int currentHotTrackItem = -1;
		protected int currentTooltipItem = -1;
		private int customizingItemIndex = -1;
		protected int mouseDownItem = -1;
        protected int clickedItem = -1;
		#endregion MEMBERS_ITEM_POSITION_AND_COUNT_SENSITIVE
		public abstract void GetPreferredSize( IGraphicsProvider gp, ref SizeF preferredSize );
		public virtual void InvalidateCachedTextSizes()
		{
			foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
				barItemRenderer.InvalidateCachedTextSizes();
		}
		public abstract void ComputeBarItemPositions( IGraphicsProvider gp );

		public bool IsKeyboardNavigationOn()
		{
			if( this.bar != null )
				return m_bIsKeyboardNavigation;
			else
				return false;
		}

		public bool CanStartKeyboardNavigation()
		{
			// First Item
			int newItem = 0;
			// Skip items that cannot be selected
			while( newItem < this.barItemRenderers.Count &&
				//(!this.ShouldDrawVisible(this.bar.Items[newItem])
				( !( (IBarItemRenderer)this.barItemRenderers[newItem] ).Visible
				|| !this.bar.Items[newItem].Enabled
				|| ( this.bar.Items[newItem] is StaticBarItem ) )
				)
				newItem++;

			if( newItem < this.barItemRenderers.Count )
				return true;
			else
				return false;
		}

		public void StartKeyboardNavigation()
		{
			m_bNeedStop = false;
			m_bIsKeyboardNavigation = true;
			PopupManager.SetCurrentPopupClient( this, true,
				false );

			if( this.currentHotTrackItem == -1 )
				this.MoveSelection( MoveHint.moveFirst );

			this.HintViaHotKeyPrefix = false;

			this.parent.GetControl().Invalidate();
			Form parentForm = this.parent.GetControl().FindForm();
			if( parentForm != null )
			{
				BarManager.OnStartingMenuNavigation( parentForm, this.parent.GetControl() );
			}

			m_bNeedStop = true;
		}
		public void StopKeyboardNavigation()
		{
			if( !m_bNeedStop ) return;

			m_bIsKeyboardNavigation = false;
			PopupManager.SetCurrentPopupClient( this, false, false );
			//			this.bar.Manager.SetActivePopup(this, false);
            if (this.clickedItem < 0 && !(this.SelectedItem is ComboBoxBarItem))
            {
                this.SetCurrentTrackItem(-1, false, false);
            }
			//			this.LoseFocus();

			if( null != this.parent )
			{
				this.parent.GetControl().Invalidate();
				Form parentForm = this.parent.GetControl().FindForm();
				if( parentForm != null )
				{
					BarManager.OnStoppingMenuNavigation( parentForm, this.parent.GetControl() );
				}
			}

			m_bKeyboardNavigationStartedByMouse = false;
		}

		public void OnFormDeactivated()
		{
			if( !this.IsShowingDropdown() )
				// When showing dropdown, the child control in the dropdown could have taken focus.
				this.StopKeyboardNavigation();
		}

		#region IGNORE_MOUSE_LOGIC
		private Timer ignoreMouseUpDownTimer;
		// Once this flag is set, the flag will be on for the next 150 ms - this will avoid processing
		// the mouse down and up that is received after the dropdown is shown, in this case, for 150 ms.
		private bool IgnoreMouseUpDown
		{
			get
			{
				if( ignoreMouseUpDownTimer != null )
					return true;
				else
					return false;
			}
			set
			{
				if( value == true )
				{
					if( this.ignoreMouseUpDownTimer == null )
						this.ignoreMouseUpDownTimer = new System.Windows.Forms.Timer();

					this.ignoreMouseUpDownTimer.Interval = 300;
					this.ignoreMouseUpDownTimer.Stop();
					this.ignoreMouseUpDownTimer.Enabled = true;
					this.ignoreMouseUpDownTimer.Tick += new EventHandler( IgnoreMouseUpDownTimer_Tick );
				}
				else
				{
					if( this.ignoreMouseUpDownTimer != null )
					{
						this.ignoreMouseUpDownTimer.Stop();
						this.ignoreMouseUpDownTimer.Dispose();
						this.ignoreMouseUpDownTimer = null;
					}
				}
			}
		}

		private void IgnoreMouseUpDownTimer_Tick( object sender, EventArgs e )
		{
			this.IgnoreMouseUpDown = false;
		}
		#endregion IGNORE_MOUSE_LOGIC

		public bool HintViaHotKeyPrefix
		{
			get
			{
				MainFrameBarManager manager = this.GetMainManager();
				bool bHintViaHotKeyPrefix = SystemInformationExt.KeyboardCuesAlwaysOn || this.hintViaHotKeyPrefix
					|| ( !m_bKeyboardNavigationStartedByMouse && ( this.IsKeyboardNavigationOn() || this.IsShowingDropdown() ) )	// Don't show keyboard cues if BarItem is activated by mouse
					|| this.Customizing;

				foreach( BarItemRenderer bir in this.barItemRenderers )
				{
					bir.BarItem.HintViaHotKeyPrefix = bHintViaHotKeyPrefix;
				}

				return bHintViaHotKeyPrefix;
			}
			set
			{
				if( this.hintViaHotKeyPrefix != value )
				{
					this.hintViaHotKeyPrefix = value;

					if( this.parent != null )
					{
						this.parent.GetControl().Invalidate();
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal BarItem CurrentHotTrackItem
		{
			get
			{
				if( this.currentHotTrackItem == -1 )
					return null;

				IBarItemRenderer renderer = ( (IBarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] );

				return renderer.BarItem;
			}
		}


		protected internal bool NeedDropDown
		{
			get
			{
				return m_bNeedDropDown;
			}
			set
			{
				if( value != m_bNeedDropDown )
				{
					m_bNeedDropDown = value;
				}
			}
		}
		protected virtual bool ShouldDrawVisible( BarItem item, bool excludeControlBasedItems )
		{
			if( excludeControlBasedItems && item is IRequiresControl && ( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
				return false;

			if( this.bar.Manager != null )
			{
				MainFrameBarManager mainManager = this.bar.Manager.MainFrameBarManager;
				if( mainManager != null )
					return this.bar.ShouldDrawVisible( item );
			}
			return item.Visible | this.Customizing;
		}

		public virtual Rectangle GetBarItemBounds( int index )
		{
			if( index < 0 || index >= this.barItemRenderers.Count )
				return Rectangle.Empty;

			IBarItemRenderer renderer = (IBarItemRenderer)this.barItemRenderers[index];

			return this.GetBoundsOf( renderer.BarItem );
		}

		internal int CustomizingItemIndex
		{
			get { return this.customizingItemIndex; }
			set
			{
				if( this.Customizing && this.customizingItemIndex != value )
				{
					int oldItem = this.customizingItemIndex;
					this.customizingItemIndex = value;

					this.InvalidateBarItems( new int[] { oldItem, this.customizingItemIndex } );

					OnCustomizingItemIndexChanged( EventArgs.Empty );
				}
			}
		}

		internal event EventHandler CustomizingItemIndexChanged;

		protected void OnCustomizingItemIndexChanged( EventArgs e )
		{
			if( null != this.CustomizingItemIndexChanged )
			{
				this.CustomizingItemIndexChanged( this, e );
			}
		}

		public BarItem DelayedPerformClickOnBarItem
		{
			get { return this.delayedPerformClickBarItem; }
			set { this.delayedPerformClickBarItem = value; }
		}
		public bool NeedLayout
		{
			get { return needLayout; }
		}
		protected void SetNeedLayout( bool needLayout )
		{
			this.needLayout = needLayout;
			if( this.needLayout && this.parent != null )
				this.parent.GetControl().Invalidate();
		}
		public virtual void Layout( IGraphicsProvider gp )
		{
			if( NeedLayout )
			{
				SetNeedLayout( false );
			}
			ComputeBarItemPositions( gp );
		}
		public RectangleF Bounds
		{
			get
			{
				return ApplyDrawingTransform( tdbounds, false );
			}
			set
			{
				bool newValue = false;

				if( this.bar == null )
				{
					if( tdbounds != value )
					{
						tdbounds = value;
						newValue = true;
					}
				}
				else
				{
					RectangleF newTdBounds = ApplyDrawingTransform( value, true );
					if( tdbounds != newTdBounds )
					{
						tdbounds = newTdBounds;
						newValue = true;
					}
				}
				if( newValue )
				{
					//				this.SetNeedLayout(true);
					// Graphics g = this.parent.GetGraphics();
					GraphicsProvider gp = new GraphicsProvider( this.parent.GetControl() );
					this.Layout( gp );
					gp.Dispose();
				}
			}
		}
		public VisualStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				if( this.style != value )
				{
					this.style = value;
					foreach( IBarItemRenderer renderer in this.barItemRenderers )
					{
						renderer.Style = value;

						// Set Style for ParentBarItems
						ParentBarItem barItem = renderer.BarItem as ParentBarItem;
						if( barItem != null )
						{
							barItem.Style = value;
						}
					}

					// Ensure that the parent and myself stay in sync.
					this.parent.Style = value;
					this.OnBoundsAffected();
				}
			}
		}
		public bool ThemesEnabled
		{
			get { return this.themesEnabeld; }
			set
			{
				if( this.themesEnabeld != value )
				{
					this.themesEnabeld = value;
					foreach( IBarItemRenderer renderer in this.barItemRenderers )
						renderer.ThemesEnabled = value;

					// Ensure that the parent and myself stay in sync.
					this.parent.ThemesEnabled = value;

					//					if(this.themesEnabeld)
					//						this.parent.GetControl().BackColor = System.Drawing.Color.Transparent;
					//					else
					//						this.parent.GetControl().ResetBackColor();
					this.OnBoundsAffected();
				}
			}
		}
		public bool LargeIcons
		{
			get
			{
				return this.parent.LargeIcons;
			}
		}
		bool IPopupParent.IsRightToLeft
		{
			get
			{
				return this.parent.IsRightToLeft;
			}
		}

		protected virtual RectangleF ApplyDrawingTransform( RectangleF rect, bool apply )
		{
			RectangleF retValue;

			using( Graphics g = this.parent.GetGraphics() )
			{
				retValue = ApplyTransform( g, this.Alignment, rect, apply );
			}

			return retValue;
		}

		protected virtual PointF ApplyDrawingTransform( PointF point, bool apply )
		{
			RectangleF dummyRect = new RectangleF( point.X, point.Y, 0, 0 );
			dummyRect = this.ApplyDrawingTransform( dummyRect, apply );
			return new PointF( dummyRect.Left, dummyRect.Top );
		}

		[Obsolete( "Use RectangleF ApplyTransform( Graphics g, CommandBarDockState align, RectangleF rect, bool apply ) instead." )]
		public static RectangleF ApplyTransform( Graphics g, CommandBarDockState align, BarStyle barStyle, RectangleF rect, bool apply )
		{
			return ApplyTransform( g, align, rect, apply );
		}

		public static RectangleF ApplyTransform( Graphics g, CommandBarDockState align, RectangleF rect, bool apply )
		{
			if( !( align == CommandBarDockState.Left || align == CommandBarDockState.Right ) )
				return rect;

			// Vertically aligned and required rotation of text
			PointF[] points = new PointF[2];

			Matrix transformMatrix = null;

			if( apply )
			{
				g.RotateTransform( 270.0F );
				points[0].X = rect.Right - 1; points[0].Y = rect.Top;
				points[1].X = rect.Left; points[1].Y = rect.Bottom - 1;
			}
			else
			{
				g.RotateTransform( -270.0F );
				points[0].X = rect.Left; points[0].Y = rect.Bottom - 1;
				points[1].X = rect.Right - 1; points[1].Y = rect.Top;
			}

			transformMatrix = g.Transform;
			g.ResetTransform();

			transformMatrix.TransformPoints( points );

			points[0] = Point.Round( points[0] );
			points[1] = Point.Round( points[1] );

			return new RectangleF( points[0].X, points[0].Y, points[1].X - points[0].X + 1F,
				points[1].Y - points[0].Y + 1F );
		}

		internal bool ActivateFormFromBar
		{
			get
			{
				return m_bActivateFormFromBar;
			}
			set
			{
				m_bActivateFormFromBar = value;
			}
		}

		/// <summary>
		/// Variable to detect if application is active or inactive.
		/// </summary>
		protected bool m_bXPMenuActive = true;

		internal bool XPMenuActive
		{
			get
			{
				return m_bXPMenuActive;
			}
			set
			{
				if( m_bXPMenuActive != value )
				{
					m_bXPMenuActive = value;
				}
			}
		}

		public BarRenderer( IBarControl parent )
		{
			this.parent = parent;
			this.barItemRenderers = new ArrayList();

			CreateToolTip();

			Control ctlParent = parent.GetControl();
			ctlParent.ParentChanged += new EventHandler( ctlParent_ParentChanged );

			if( ctlParent.Parent != null )
			{
				if( !this.DesignMode )
					if( this.bar != null && this.bar.manager != null )
					{
						MessageFilterEntryHelper.AddMessageFilter( this, false, this.bar.manager.Form );
					}
					else
					{
						MessageFilterEntryHelper.AddMessageFilter( this, false );
					}
			}

			//this.customizationPopup = new CustomizingPopupMenu();
			this.invisibleBarItems = new BarItems();
			this.efficentTracker = new EfficientMenuTracker( this );

			InitDropDownTimer();
		}

		protected void CreateToolTip()
		{
			DisposeTooltip();

			tooltip = new ToolTip();

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			tooltip.StripAmpersands = true;
#endif

			tooltip.ShowAlways = true;
			tooltip.AutomaticDelay = 0;
			tooltip.AutoPopDelay = 0;
			tooltip.ReshowDelay = 0;
			tooltip.InitialDelay = 0;
		}

		virtual protected CustomizingPopupMenu CustomizationPopup
		{
			get
			{
				if( this.customizationPopup == null )
					this.customizationPopup = this.CreateCustomizationPopup();

				return this.customizationPopup;
			}
		}
		protected virtual CustomizingPopupMenu CreateCustomizationPopup()
		{
			return new CustomizingPopupMenu();
		}
		public virtual void Detach()
		{
			this.CloseDropdowns();
			this.ResetDragging();
			this.ResetHotTracking();
			if( this.customizationPopup != null )
			{
				this.customizationPopup.Dispose();
				this.customizationPopup = null;
			}
			if( this.bar != null )
			{
				//				this.MouseHooker.HookMessages = false;
				if( dndHelper != null )
					this.dndHelper.ParentItem = null;
				bar.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );
				if( bar.Items != null )
				{
					bar.Items.CollectionChanged -= new CollectionChangeEventHandler( this.BarItems_CollectionChanged );
					bar.Items.ItemPropertyChanged -=
						new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );
				}
				if( this.bar.Manager != null )
				{
					this.bar.Manager.CustomizingItemChanged
						-= new EventHandler( this.CustomizingItem_Changed );
					this.bar.Manager.PropertyChanged
						-= new SyncfusionPropertyChangedEventHandler( this.Property_Changed );

					this.bar.Manager.CustomizationDone
						-= new EventHandler( this.CustomizationDone );
					this.bar.Manager.CustomizationBegin
						-= new EventHandler( this.CustomizationBegin );
					MainFrameBarManager mainFrameBarManager = this.bar.Manager.MainFrameBarManager;
					if( mainFrameBarManager != null )
					{
						mainFrameBarManager.CustomizationDone
							-= new EventHandler( this.CustomizationDone );
						mainFrameBarManager.CustomizationBegin
							-= new EventHandler( this.CustomizationBegin );
					}
				}
				this.RemoveBarItemRenderers( true );

				Control ctlParent = parent.GetControl();
				Form frmParent = ctlParent.FindForm();

				if( ctlParent != null )
				{
					ctlParent.ParentChanged -= new EventHandler( ctlParent_ParentChanged );
				}

				if( null != frmParent )
				{
					frmParent.ParentChanged -= new EventHandler( frmParent_ParentChanged );
				}

				if( !this.DesignMode )
					MessageFilterEntryHelper.RemoveMessageFilter( this );
			}
		}

		public BarItems InvisibleBarItems
		{
			get { return this.invisibleBarItems; }
		}

		private MouseProcHookerUtil MouseHooker
		{
			get
			{
				if( this.mouseHooker != null )
					this.mouseHooker = new MouseProcHookerUtil( this.parent.GetControl().Handle, (IMouseHookHLProcClient)this );

				return mouseHooker;
			}
			set
			{
				mouseHooker = value;
			}
		}

		~BarRenderer()
		{
			this.Dispose( false );
		}

		public void Dispose()
		{
			this.IgnoreMouseUpDown = false;
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.DisposeTooltip();
				this.Detach();
				if( this.mouseHooker != null )
				{
					this.mouseHooker.HookMessages = false;
					mouseHooker.Dispose();
					mouseHooker = null;
				}
				if(bar != null)
                bar.Items.ItemPropertyChanged -= new SyncfusionPropertyChangedEventHandler(this.Bar_PropertyChanged);
                m_hideDropDownTimer.Tick -= new EventHandler(m_hideDropDownTimer_Tick);

				this.efficentTracker.Dispose();
				this.efficentTracker = null;

				this.invisibleBarItems.Clear();
				this.invisibleBarItems = null;

				Control ctlParent = this.parent.GetControl();
				ctlParent.ParentChanged -= new EventHandler( ctlParent_ParentChanged );

				this.barItemRenderers.Clear();
				this.barItemRenderers = null;
				this.parent = null;
				this.bar = null;
				if (dndHelper != null)
				{
					this.dndHelper.Dispose();
					this.dndHelper = null;
				}
			}
		}

		protected void DisposeTooltip()
		{
			if( null != this.tooltip )
			{
				this.tooltip.Dispose();
				this.tooltip = null;
			}
		}

		public CommandBarDockState Alignment
		{
			get { return this.barAlignment; }
			set
			{
				// Check this for better performance.
				if( this.barAlignment != value )
				{
					this.barAlignment = value;
					//				this.SetNeedLayout(true);
					this.OnBoundsAffected();
				}
			}
		}

		/// <summary>
		/// Gets a value indicating wether the renderer has a vertical alignment.
		/// </summary>
		public bool IsVerticallyAligned
		{
			get
			{
				return this.Alignment == CommandBarDockState.Left
					|| this.Alignment == CommandBarDockState.Right;
			}
		}

		public bool Customizing
		{
			get
			{
				bool bDesign = false;

				if( this.bar != null && this.bar.Manager != null )
				{
					bDesign = this.bar.Manager.Customizing;
				}
				else if( null != this.parent )
				{
					bDesign = this.parent.DesignMode;
				}

				return bDesign;
			}
		}
		public bool DndCustomizing
		{
			get
			{
				if( this.bar != null && this.bar.Manager != null )
					return this.bar.Manager.DndCustomizing;
				else
					return this.parent.DesignMode;
			}
		}
		public Bar Bar
		{
			get
			{
				return bar;
			}
			set
			{
				if( bar != value )
				{
					if( bar != null )
					{
						Detach();
					}
					bar = value;
					if( bar != null )
					{
						if( !DesignMode )
						{
							if( this.bar.manager != null )
							{
								MessageFilterEntryHelper.AddMessageFilter( this, false, this.bar.manager.Form );
							}
							else
							{
								MessageFilterEntryHelper.AddMessageFilter( this, false );
							}
						}

						bar.PropertyChanged += new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );

						if( null != bar.Items )
						{
							bar.Items.CollectionChanged += new CollectionChangeEventHandler( this.BarItems_CollectionChanged );
							bar.Items.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );
						}

						if( this.bar.Manager != null )
						{
							this.bar.Manager.CustomizingItemChanged
								+= new EventHandler( this.CustomizingItem_Changed );
							this.bar.Manager.PropertyChanged
								+= new SyncfusionPropertyChangedEventHandler( this.Property_Changed );

							this.bar.Manager.CustomizationDone
								+= new EventHandler( this.CustomizationDone );
							this.bar.Manager.CustomizationBegin
								+= new EventHandler( this.CustomizationBegin );

							MainFrameBarManager mainFrameBarManager = this.bar.Manager.MainFrameBarManager;
							if( mainFrameBarManager != null )
							{
								mainFrameBarManager.CustomizationDone
									+= new EventHandler( this.CustomizationDone );
								mainFrameBarManager.CustomizationBegin
									+= new EventHandler( this.CustomizationBegin );
								this.ThemesEnabled = mainFrameBarManager.ThemesEnabled;
								this.Style = mainFrameBarManager.Style;
							}
							if( this.bar.Manager.DesignMode )
								this.CustomizationPopup.dummyManager.SetUseHooksForMenus( true );
							else if( this.customizationPopup != null )
								this.customizationPopup.dummyManager.SetUseHooksForMenus( false );

							//							if(this.bar.Manager.DesignMode)
							//							{
							//								this.MouseHooker.HookMessages = true;
							//							}
						}
						if( null != bar.Items )
						{
							OnBarItemsCollectionChanged();
						}
					}
				}
			}
		}
		public bool IsShowingDropdown()
		{
			if( this.currentHotTrackItem != -1 && this.currentHotTrackItem < this.barItemRenderers.Count )
				return
					( (BarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] ).ShowingDropDown;
			else
				return false;
		}
		#region IPopupChild_Parent_Imp

		private MainFrameBarManager GetMainManager()
		{
			MainFrameBarManager barManager = this.Bar.Manager as MainFrameBarManager;
			if( barManager != null ) return barManager;

			ChildFrameBarManager childMan = this.Bar.Manager as ChildFrameBarManager;
			if( childMan != null )
			{
				barManager = childMan.MainFrameBarManager;
			}

			return barManager;
		}

		//IPopupParent interfaces
		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			MainFrameBarManager mainBarMan = GetMainManager();
			if( mainBarMan != null )
			{
				mainBarMan.ShouldHidePopup = false;
			}
			IDropDownItem dropDownRenderer = null;

			MenuGrid grid = childUI as MenuGrid;
			if( grid != null )
			{
				m_bNeedDropDown = !grid.IsEscapeKeyPressed;
			}

			if( this.currentDndDropDown != -1 )
			{
				dropDownRenderer = ( this.barItemRenderers[this.currentDndDropDown] ) as IDropDownItem;
				this.currentDndDropDown = -1;
			}
			else if( this.currentHotTrackItem != -1 && this.currentHotTrackItem < this.barItemRenderers.Count )
				dropDownRenderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;

			if( dropDownRenderer != null )
			{
				if( !this.efficentTracker.Tracking )
				{
					this.mouseDownItem = -1;
				}
                if (childUI is ListBoxContainer)
                {
                    if ((childUI as ListBoxContainer).Mouseup)
                    {
                        if (dropDownRenderer is BarItemRenderer)
                        {
                            if (!(dropDownRenderer as BarItemRenderer).Active)
                            {
                                (dropDownRenderer as BarItemRenderer).Active = true;
                            }
                        }
                    }
                }
				dropDownRenderer.ChildClosing( childUI, popupCloseType );
			}

			if( this.efficentTracker.Tracking )
				this.efficentTracker.SetDelayedPostChildClosingArgs( popupCloseType, dropDownRenderer );
			else
				this.PostChildClosing( popupCloseType, dropDownRenderer );
		}
		private void PostChildClosing( PopupCloseType popupCloseType, IDropDownItem dropDownItem )
		{
			if( !this.IsShowingDropdown() )
			{
				MainFrameBarManager manager = this.GetMainManager();
				if( popupCloseType != PopupCloseType.Canceled )
					this.StopKeyboardNavigation();
				else if( manager != null && manager.bNeedStartKeyboardNavigation )
				{
					this.StartKeyboardNavigation();
				}
			}

			if( dropDownItem != null )
				dropDownItem.AfterChildClosing();
		}

		protected IDropDownItem GetCurrentRenderer()
		{
			IDropDownItem renderer = null;

			if( this.Customizing && this.currentDndDropDown != -1 )
				renderer = ( this.barItemRenderers[this.currentDndDropDown] ) as IDropDownItem;
			else if( this.currentHotTrackItem != -1 )
				renderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;

			return renderer;
		}

		//		bool OverrideCloseChild();
		public virtual Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign )
		{
			IDropDownItem renderer = GetCurrentRenderer();

			if( renderer != null )
				return renderer.GetLocationForPopupAlignment( prevAlign, out newAlign );

			newAlign = PopupRelativeAlignment.Default;
			return Point.Empty;
		}

		public virtual Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlign )
		{
			IDropDownItem renderer = null;

			if( this.Customizing && this.currentDndDropDown != -1 )
				renderer = ( this.barItemRenderers[this.currentDndDropDown] ) as IDropDownItem;
			else if( this.currentHotTrackItem != -1 )
				renderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;

			if( renderer != null )
				return renderer.GetBorderOverlapCue( rAlign );

			return null;
		}

		public virtual Control GetPopupParentControl()
		{
			Control parentControl = null;
			IDropDownItem ddiRenderer = null;

			if( this.Customizing && this.currentDndDropDown >= 0 )
			{
				ddiRenderer = ( this.barItemRenderers[this.currentDndDropDown] ) as IDropDownItem;
			}
			else if( this.currentHotTrackItem >= 0 )
			{
				ddiRenderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;
			}

			if( ddiRenderer != null )
			{
				parentControl = ddiRenderer.GetPopupParentControl();
			}

			if( parentControl == null && this.parent != null )
			{
				parentControl = this.parent.GetControl();
			}

			return parentControl;
		}

		public virtual bool IsRelatedControl( Control control, bool askPopupParent )
		{
			IDropDownItem renderer = null;

			if( this.Customizing && this.currentDndDropDown != -1 )
				renderer = ( this.barItemRenderers[this.currentDndDropDown] ) as IDropDownItem;
			else if( this.currentHotTrackItem != -1 )
				renderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;

			Control parentControl = null;
			if( renderer != null )
				parentControl = renderer.GetPopupParentControl();

			bool bIsRelated = parentControl != null && control == parentControl;

			if( !bIsRelated && null != this.parent )
			{
				bIsRelated = ( control == this.parent.GetControl() );
			}

			if( !bIsRelated && null != renderer )
			{
				bIsRelated = renderer.IsRelatedControl( control );
			}

			return bIsRelated;
		}
		public virtual void HidePopup( PopupCloseType popupCloseType )
		{
			bool showing = this.IsShowingDropdown();

			if( showing )
			{
				IDropDownItem dropDownRenderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as IDropDownItem;
				BarItemRenderer renderer = ( this.barItemRenderers[this.currentHotTrackItem] ) as BarItemRenderer;

				if( dropDownRenderer != null )
				{
					renderer.DropDown( false, false, null );
				}
			}

			if( showing )
				this.StopKeyboardNavigation();
		}
		public virtual bool IsShowing()
		{
			if( this.bar != null )
				//				return this.bar.Manager.ActivePopup == this;
				return PopupManager.ActivePopupClient == this;
			else
				return false;
		}
		public IPopupParent PopupParent
		{
			get { return null != this.parent ? this.parent.PopupParent : null; }
		}
		private bool ProcessMouseMessage( Control destination, int msg )
		{
			if( msg == NativeMethods.WM_MOUSEMOVE || msg == NativeMethods.WM_MOUSELEAVE || msg == NativeMethods.WM_MOUSEHOVER
				|| msg == NativeMethods.WM_NCMOUSEMOVE || msg == NativeMethods.WM_NCMOUSELEAVE || msg == NativeMethods.WM_NCMOUSEHOVER )
			{
				// If the destination is in the parent chain, let it pass through
				if( destination != null && null != this.parent )
				{
					return destination != this.parent.GetControl() && this.IsRelatedControl( destination, true );
				}
				else
				{
					return false;
				}
			}
			else
				this.VeryifyMouseBasedDeactivation( destination, msg );

			return false;
		}
		public virtual bool MouseMessage( ref Message m )
		{
			Control destination = Control.FromHandle( m.HWnd );

			return this.ProcessMouseMessage( destination, m.Msg );
		}
		bool IMouseHookHLProcClient.MouseHookProc( int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo )
		{
			if( !this.DesignMode )
			{
				Control destinationControl = Control.FromHandle( hwnd );

				if( ( parent != null && destinationControl == this.parent.GetControl() )
					|| !this.IsRelatedControl( destinationControl, true ) )
				{
					return this.ProcessMouseMessage( destinationControl, msg );
				}
			}
			return false;
		}
		public virtual bool KeyboardMessage( ref Message m )
		{
			//			bool processed = false;
			Keys keys = (Keys)m.WParam.ToInt32();
			//			if(m.Msg == 0x0100 || m.Msg == 0x0102 )
			//			{
			//				processed = this.ProcessKeyDown(keys);
			//			}
			//
			//			if(!processed && !this.Customizing)
			//			{
			//				if(m.Msg == 260 /*WM_KEYDOWN*/)
			//				{
			//					this.PostProcessKeyDown(m.LParam.ToInt32(), keys);
			//				}
			//			}
			//
			//			if( processed ) return true;

			if( this.Customizing )
			{
				return false;
			}
			else
			{
				IBarControl barCtl = this.GetBarControl();

				if( barCtl != null )
				{
					return barCtl.NeedKey( keys );
				}
				else
				{
					return false;
				}
			}
		}

		bool IKeyboardProcHookClient.KeyboardHookProc( int wParam, int lParam )
		{
			Keys keys = (Keys)wParam;
				if( keys == Keys.Escape )
				{
                    foreach (BarItemRenderer br in this.barItemRenderers)
                    {
                        if (br.Active)
                        {
                            if (br != null && (br is ComboBoxItemRenderer))
                            {
                                br.HotTrack = false;
                                br.Active = false;
                                br.BarItem.PerformUnselected();
                                break;
                            }
                        }
                    }
					
				}
			bool processed = false;

			if( this.IsKeyboardNavigationOn() )
			{
				if( keys != Keys.Menu )
				{
					if( ( lParam & 0x80000000 ) == 0 )
					{
						processed = this.ProcessKeyDown( keys );
					}

					if( !processed && !this.Customizing
					&& ( lParam & 0x80000000 ) == 0 )
					{
						this.PostProcessKeyDown( lParam, keys );
					}

					return processed;
				}

				if( this.Customizing )
				{
					processed = false;
				}
				else
				{
					IBarControl barCtl = this.GetBarControl();

					if( null != barCtl )
					{
						processed = barCtl.NeedKey( keys );
					}
				}
			}

			return processed;
		}
		private void PostProcessKeyDown( int lParam, Keys key )
		{
			if( ( key == Keys.Menu || key == Keys.F10 )
				&& ( lParam & 0x40000000 ) == 0 )//0x1E
			{
				if( this.bar != null && this.bar.Manager != null && this.bar.Manager.MainFrameBarManager != null )
				{
					if( this.currentHotTrackItem != -1 )
					{
						OnHitEscape();
					}

					bool bIgnore = ( key == Keys.F10 && !this.IsShowingDropdown() );

					this.bar.Manager.MainFrameBarManager.ignoreNextAltKeyUp = bIgnore;
				}
			}
		}

		protected virtual void VeryifyMouseBasedDeactivation( Control destinationControl, int msg )
		{
			switch( msg )
			{
				case NativeMethods.WM_MOUSEACTIVATE: // 0x0021 
				case NativeMethods.WM_LBUTTONDOWN: // 0x0201 
				case NativeMethods.WM_RBUTTONDOWN: // 0x0204 
				case NativeMethods.WM_MBUTTONDOWN: // 0x0207 
				if( this.IsRelatedControl( destinationControl, true ) ||
						( destinationControl as MenuGrid != null ) )
				{
					break;
				}
				goto case NativeMethods.WM_MOUSEWHEEL;

				case NativeMethods.WM_MOUSEWHEEL: // 0x020a 
				case NativeMethods.WM_NCLBUTTONDOWN: // 0x00a1 
				case NativeMethods.WM_NCRBUTTONDOWN: // 0x00a4 
				case NativeMethods.WM_NCMBUTTONDOWN: // 0x00a7 
				if( !this.IsRelatedControl( destinationControl, false ) &&
						( destinationControl as MenuGrid == null ) )
				{
					if( msg == NativeMethods.WM_MOUSEWHEEL &&
							this.CurrentHotTrackItem != null )
					{
						this.SetCurrentTrackItem( -1, false, false );
					}

					// click outside 
					this.HidePopup( PopupCloseType.Deactivated );
				}
				break;
			}
		}
		#endregion IPopupChild_Imp
		protected virtual IBarItemRenderer CreateNewRenderer( BarItem barItem )
		{
			IBarItemRenderer renderer = null;
			if( barItem is DropDownBarItem
				|| barItem is ParentBarItem )
			{
				renderer = new DropDownBarItemRenderer( this );
			}
			else if( barItem is ComboBoxBarItem )
			{
				ComboBoxBarItem comboItem = barItem as ComboBoxBarItem;
				if( comboItem.Editable )
					renderer = new EditableComboRenderer( this );
				else
					renderer = new ComboBoxItemRenderer( this );
			}
			else if( barItem is StaticBarItem )
			{
				renderer = new StaticBarItemRenderer( this );
			}
			else if( barItem is TextBoxBarItem )
			{
				renderer = new TextBoxBarItemRenderer( this );
			}
			else
				renderer = new BarItemRenderer( this );

			if( renderer != null )
			{
				renderer.BarItem = barItem;
				renderer.ThemesEnabled = this.ThemesEnabled;
				renderer.Style = this.Style;
			}

			return renderer;
		}
		private void BarItems_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			OnBarItemsCollectionChanged();
		}

		protected virtual void RemoveBarItemRenderers( bool forceRemove )
		{
			for( int i = this.barItemRenderers.Count - 1; i >= 0; i-- )
			{
				IBarItemRenderer barItemRenderer = (IBarItemRenderer)this.barItemRenderers[i];
				if( forceRemove || this.bar.Items.IndexOf( barItemRenderer.BarItem ) == -1 )
					this.RemoveRenderer( barItemRenderer, i );
			}
		}

		void IBarRenderer.RemoveRenderer( IBarItemRenderer renderer )
		{
			int curPos = this.barItemRenderers.IndexOf( renderer );
			if( curPos != -1 )
				this.RemoveRenderer( renderer, curPos );
		}

		void IBarRenderer.UpdateRenderers()
		{
			this.OnBarItemsCollectionChanged();
		}

		protected virtual void RemoveRenderer( IBarItemRenderer renderer, int curPos )
		{
			if( renderer == null )
				return;

			BarItem barItem = renderer.BarItem;
			if( barItem != null )
				barItem.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.BarItem_PropertyChanged );

			renderer.Active = false;
			renderer.DropDown( false, false, null );
			barItemRenderers.RemoveAt( curPos );
			renderer.Dispose();
		}

		protected virtual void OnBarItemsCollectionChanged()
		{
			this.CloseDropdowns();
			this.ResetHotTracking();
			this.currentTooltipItem = -1;
			if( this.Customizing )
				this.SetCustomizingItemIndex();
			// Parse through the new collection and update barItemRenderers list
			int i = 0;
			BarItems barItems = this.bar.Items;
			if( barItems.Count < this.barItemRenderers.Count )
			{
				RemoveBarItemRenderers( false );
			}

			if( this.parent.GetControl().Parent != null )
			{
				CommandBar cBar = this.parent.GetControl().Parent as CommandBar;
				if( cBar != null && cBar.cdbParent != null )
				{
					cBar.bRedockNeededInternal = true;
				}
			}

			foreach( BarItem barItem in barItems )
			{
				bool bNewBarItem = false;
				if( barItemRenderers.Count > i )
				{
					// Verify if the existing renderer is bound to the right barItem.
					BarItem oldBarItem = ( (IBarItemRenderer)barItemRenderers[i] ).BarItem;
					if( oldBarItem != barItem )
					{
						RemoveRenderer( (IBarItemRenderer)barItemRenderers[i], i );

						bNewBarItem = true;	// Possibly a new barItem
					}
				}
				else
					bNewBarItem = true;

				// Setup handlers for new barItem
				if( bNewBarItem )
				{
					// insert new renderer.
					IBarItemRenderer renderer = CreateNewRenderer( barItem );
					if( renderer != null )
						barItemRenderers.Insert( i, renderer );

					barItem.PropertyChanged += new SyncfusionPropertyChangedEventHandler( this.BarItem_PropertyChanged );
				}
				i++;
			}

			this.OnBoundsAffected();
			this.SetNeedLayout( true );
		}

		protected virtual void OnBoundsAffected()
		{
			if( null != parent )
			{
				parent.OnBarBoundsAffected();
			}
		}
		protected virtual void CustomizationDone( object sender, EventArgs e )
		{
			if( sender == this.bar.Manager && sender != null )
				this.CloseDropdowns();
			this.CustomizingItemIndex = -1;
			this.OnBoundsAffected();
			this.parent.GetControl().Invalidate();
		}
		protected virtual void CustomizationBegin( object sender, EventArgs e )
		{
			this.OnBoundsAffected();
			this.parent.GetControl().Invalidate();
		}
		private void CustomizingItem_Changed( object sender, EventArgs e )
		{
			this.SetCustomizingItemIndex();
		}
		private void Property_Changed( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "ThemesEnabled" )
			{
				this.ThemesEnabled = (bool)e.NewValue;
			}
			else if( e.PropertyName == "Style" )
			{
				this.Style = (VisualStyle)e.NewValue;
			}
			this.OnBoundsAffected();
		}
		protected virtual void Bar_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "Visible" && this.parent != null )
			{
				CommandBar cBar = this.parent.GetControl().Parent as CommandBar;
				if( cBar != null )
				{
					if( cBar.cdbParent != null )
					{
						cBar.bRedockNeededInternal = true;
					}
				}
			}

			OnBoundsAffected();
		}
		private void SetCustomizingItemIndex()
		{
			if( this.bar.Manager != null )
			{
				BarItem item = this.bar.Manager.CustomizingItem;
				if( item != null )
				{
					int localIndex = this.bar.Items.IndexOf( item );
					this.CustomizingItemIndex = localIndex;
				}
				else
					this.CustomizingItemIndex = -1;
			}
			else
				this.CustomizingItemIndex = -1;
		}
		protected virtual void InvalidateBarItems( int[] barItemIndices )
		{
			if( !this.parent.GetControl().Visible || !this.parent.GetControl().IsHandleCreated )
				return;

			foreach( int index in barItemIndices )
			{
				RectangleF rectBar = RectangleF.Empty;

				if( ( index != -1 ) && ( index < barItemRenderers.Count ) )
					rectBar = ( (IBarItemRenderer)barItemRenderers[index] ).Bounds;
				else
					continue;

				RectangleF rectInflated = new RectangleF( (float)Math.Floor( rectBar.Left ), (float)Math.Floor( rectBar.Top ),
					(float)Math.Ceiling( rectBar.Right ) - (float)Math.Floor( rectBar.Left ) + 1,
					(float)Math.Ceiling( rectBar.Bottom ) - (float)Math.Floor( rectBar.Top ) + 1
					);
				// Invoke a paint
				this.parent.OnRepaint( ApplyDrawingTransform( rectInflated, false ) );
			}
		}

		protected virtual void BarItem_PropertyChanged( object sender, Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e )
		{
			// Change in this property will not have any effect.
			if( e.PropertyName == "SeparatorIndices" )
				return;

			BarItem barItem = (BarItem)sender;
			int index = this.bar.Items.IndexOf( barItem );

			if( this.barItemRenderers.Count <= index )
				return;

			( (IBarItemRenderer)barItemRenderers[index] ).BarItemPropertyChanged( e );

			if( e.PropertyChangeEffect == PropertyChangeEffect.NeedLayout )
				this.OnBoundsAffected();
			if( this.parent.GetControl().IsHandleCreated && this.parent.GetControl().Visible )
			{
				// Get the rect to redraw
				RectangleF barItemRect = ( (IBarItemRenderer)barItemRenderers[index] ).Bounds;

				// Invoke a paint
				this.parent.OnRepaint( ApplyDrawingTransform( barItemRect, false ) );
			}
		}

		public bool PreFilterMessage( ref Message m )
		{

			if( m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST && m.Msg != NativeMethods.WM_CHAR )
			{
				bool processed = ( this as IKeyboardProcHookClient ).KeyboardHookProc( (int)m.WParam.ToInt64(), (int)m.LParam.ToInt64() );

				if( processed )
				{
					return true;
				}
			}
			if (this.IsKeyboardNavigationOn())
			{
				switch (m.Msg)
				{
					case NativeMethods.WM_MOUSEACTIVATE: // 0x0021 
					case NativeMethods.WM_LBUTTONDOWN: // 0x0201 
					case NativeMethods.WM_RBUTTONDOWN: // 0x0204 
					case NativeMethods.WM_MBUTTONDOWN: // 0x0207 
					case NativeMethods.WM_NCLBUTTONDOWN: // 0x00a1
					case NativeMethods.WM_NCRBUTTONDOWN: // 0x00a4
					case NativeMethods.WM_NCMBUTTONDOWN: // 0x00a7
						{
							Control destinationControl = Control.FromHandle(m.HWnd);
							if (!IsShowingDropdown() && !this.IsRelatedControl(destinationControl, false))
							{
								this.ResetHotTracking();
								this.StopKeyboardNavigation();
							}
							break;
						}
				}
			}

			XPToolBar parentBar = parent as XPToolBar;

			if( ( parentBar != null ) && ( parentBar.TopLevelControl != null ) && ( parentBar.TopLevelControl.Focused ) )
			{
				if( m.Msg == NativeMethods.WM_SYSKEYDOWN )
				{
					// check if ALT is pressed
					if( (int)m.WParam == 0x12 )
					{
						HintViaHotKeyPrefix = true;
					}
				}
				else if( m.Msg == NativeMethods.WM_SYSKEYUP )
				{
					// check if ALT is released
					if( (int)m.WParam == 0x12 )
					{
						HintViaHotKeyPrefix = false;

						// strange, but WM_SYSKEYDOWN does not come after WM_SYSKEYUP
						// Need to reset whis behavior by sending WM_SYSKEYUP
						Control ctrl = this.parent.BarHost as Control;

						Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( ctrl.Handle, NativeMethods.WM_SYSKEYUP, 0, 0 );
					}
				}
			}

			return false;
		}

		public virtual void OnPaint( Graphics g, Rectangle clipRect )
		{
			if( NeedLayout )
			{
				GraphicsProvider gph = new GraphicsProvider( g );
				this.Layout( gph );
				gph.Dispose();
			}

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled
				&& this.parent.ShouldDelegateBGDrawingToParentWhenThemed() )
			{
				IntPtr hdc = g.GetHdc();
                try
                {
                    Syncfusion.Runtime.InteropServices.NativeMethods.RECT rc = new Syncfusion.Runtime.InteropServices.NativeMethods.RECT(clipRect);
                    Syncfusion.Runtime.InteropServices.NativeMethods.DrawThemeParentBackground(this.GetControl().Handle, hdc, ref rc);
                }
                finally
                {
                    g.ReleaseHdc(hdc);
                }
			}
			if( this.barItemRenderers.Count > 0 )
			{
				Region oldClipRegion = g.Clip;
				Region newClipRegion = new Region( this.Bounds );
				g.SetClip( newClipRegion, CombineMode.Intersect );

				// Invoke each barItemRenderer's Paint
				foreach( IBarItemRenderer renderer in this.barItemRenderers )
				{
					renderer.OnPaint( g, clipRect );
				}

				this.DrawAdornments( g );

				g.SetClip( oldClipRegion, CombineMode.Replace );
				newClipRegion.Dispose();
			}
			if( this.dndHelper != null )
				this.dndHelper.PaintCueCursor( g );
		}

		protected virtual void DrawAdornments( Graphics g )
		{
			bool previousItemWasSeparator = false;
			bool firstItemDrawn = false;
			foreach( IBarItemRenderer renderer in this.barItemRenderers )
			{
				bool itemVisible = renderer.Visible && this.ShouldDrawVisible( renderer.BarItem, false );
				if( this.bar.IsGroupBeginning( renderer.BarItem )
					&& !previousItemWasSeparator && firstItemDrawn )
				{
					// if the item is invisible, draw the separator only if the item is from the main manager
					if( itemVisible ||
						renderer.BarItem.Manager == null || renderer.BarItem.Manager is MainFrameBarManager )
					{
						previousItemWasSeparator = true;
						renderer.DrawSeparator( g );
					}
				}
				if( itemVisible )
				{
					previousItemWasSeparator = false;
					firstItemDrawn = true;
				}
			}

			if( this.Customizing && this.CustomizingItemIndex != -1
				&& ( this.bar.Manager == null || this.bar.Items.Contains( this.bar.Manager.CustomizingItem ) ) )
			{
				IBarItemRenderer renderer = this.barItemRenderers[this.CustomizingItemIndex] as IBarItemRenderer;
				RectangleF itemBounds = renderer.Bounds;
				// Remove any transforms
				itemBounds = this.ApplyDrawingTransform( itemBounds, false );
				itemBounds.Width -= 2;

				ComboBoxItemRenderer comboRenderer = renderer as ComboBoxItemRenderer;
				if( comboRenderer != null )
				{
					itemBounds = CorrectComboSelectionBounds( comboRenderer, itemBounds );
				}
				itemBounds.Height -= 2;
				itemBounds.Offset( 1, 1 );
                using (Pen pen = new Pen(SystemColors.ControlDark))
                {
                    g.DrawRectangle(pen, Rectangle.Round(itemBounds));
                }
			}
		}

		private const float DEF_EPS = 0.00000001f;

		public static bool IsInteger( float value )
		{
			float decValue = (float)Math.Round( value, 0 );
			return ( Math.Abs( decValue - value ) < DEF_EPS );
		}
		/// <summary>
		/// Corrects customization selection rectangle for ComoDropDown item.
		/// </summary>
		private RectangleF CorrectComboSelectionBounds( ComboBoxItemRenderer renderer, RectangleF bounds )
		{
			if( renderer == null )
				throw new ArgumentNullException( "renderer " );

			RectangleF itemBounds = bounds;
			itemBounds.Offset( ComboBoxItemRenderer.DEF_BORDER_OFFSET, 0 );
			itemBounds.Width -= ComboBoxItemRenderer.DEF_BORDER_OFFSET + ComboBoxItemRenderer.DEF_SELECTION_OFFSET;

			return itemBounds;
		}

		protected virtual void OnStartDragging()
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( this.Bar != null && this.Bar.Manager != null )
			{
				this.Bar.Manager.CustomDrag = true;
			}
#endif
			m_bDragging = true;
			this.CloseDropdowns();
		}
		protected virtual void OnEndDragging()
		{
			this.ResetDragging();
			this.CloseDropdowns();
			m_bDragging = false;
		}

		protected virtual void CloseDropdowns()
		{
			foreach( IBarItemRenderer renderer in this.barItemRenderers )
			{
				renderer.Active = false;
				renderer.DropDown( false, false, null );
			}
		}

		public virtual void OnMouseMove( MouseEventArgs e )
		{
			bool bActiveFormCheckOverride = false;

			// Ignore the first mouse move after mouse down
			if( Environment.TickCount < ignoreMouseMoveTick )
			{
				return;
			}
			if( this.ignoreMouseMove )
			{
				this.ignoreMouseMove = false;
				return;
			}

			if( this.mouseDownPoint != Point.Empty && this.mouseDownPoint == new Point( e.X, e.Y ) )
				return;

			this.mouseDownPoint = Point.Empty;

			if( this.parent.RequiresActiveFormForMouseTrack() )
			{
				if( this.bar.Manager != null
					// This should normally be defined, but some customers tried showing child window bars in 
					// child XPToolBars - unsupported stuff. But, this check is necessary in such scenarios.
					&& this.bar.Manager.MainFrameBarManager != null )
				{

					bActiveFormCheckOverride = this.bar.Manager.MainFrameBarManager.BarItemActiveFormCheckOverride;

					if( !this.bar.Manager.DesignMode )
					{

						if( this.ActivateFormFromBar )
						{
							if( Form.ActiveForm != this.bar.Manager.MainFrameBarManager.Form
								&& ( !this.bar.Manager.IsCustomizationDialogCreated
								|| Form.ActiveForm != this.bar.Manager.CustomizationDialog ) && bActiveFormCheckOverride == false
								)
								return;
						}
						else
						{
							BarControlInternal bcInternal = (BarControlInternal)this.parent;
							if( bcInternal != null )
								if( this.bar.Manager.IsCustomizationDialogCreated
									&& Form.ActiveForm == null
									|| !bcInternal.XPMenuActive
									)
								{
									return;
								}
						}
					}
				}
				else
				{
					Form activeMDIChild = null;
					Form activeForm = Form.ActiveForm;

					if( null != activeForm )
					{
						activeMDIChild = activeForm.ActiveMdiChild;
					}

					Form controlsForm = this.GetControl().FindForm();

					if( activeForm != controlsForm
						&& activeMDIChild != controlsForm
						// If the active form is the Owner of the child's form, that's good enough too.
							&& null != controlsForm && controlsForm.Owner != activeForm )
					{
						Control parentControl = controlsForm;
						// Also make sure that the control's Form is not a control-child of the 
						// active form (a non-mdi scenario).
						while( parentControl != null && parentControl != activeForm )
						{
							parentControl = parentControl.Parent;
						}

						if( parentControl == null )
							return;
					}
				}
			}
			// If some other popup (bar or menu) has the focus, then do nothing.
			IPopupChild popupChild = PopupManager.ActivePopupClient;
			if( popupChild != null && popupChild != this )
			{
				// But if other popup is actually a PopupControlContainer that hosts this XPToolbar, then allow mouse processing
				if( popupChild.GetPopupParentControl() == null
					|| WinFormsUtils.IsParent( popupChild.GetPopupParentControl(), this.GetControl() ) )
				{
					// If I am a parent or GrandParent, then I will go ahead and process mouse move.
					IPopupParent parent = popupChild.PopupParent;
					while( parent != null )
					{
						if( parent == this )
							break;
						if( parent is IPopupChild )
							parent = ( (IPopupChild)parent ).PopupParent;
						else
							parent = null;
					}
					if( parent == null )
						return;
				}
			}

			if( this.barItemRenderers.Count <= 0 )
				return;

			Point mousePosition = new Point( e.X, e.Y );

			// Start a drag and drop, when pressed and moved.
			if( ( this.bar.AllowCustomizing || this.DesignMode ) && this.Customizing && e.Button == MouseButtons.Left
				&& this.customizingItemIndex != -1
				&& this.mouseDownItem == this.customizingItemIndex )
			{
				BarItem selectedItem = this.bar.Items[customizingItemIndex];
				BarManager barMgr = this.bar.Manager;

				if( null != barMgr && barMgr.CanStartDragging( selectedItem ) )
				{
					this.OnStartDragging();
					this.ValidateDndHelper();
					this.dndHelper.Dragging = true;
					BarItemDndData dndData = new BarItemDndData( selectedItem, this.bar );
					DragDropEffects dropEffect = DragDropEffects.None;

					try
					{
						dropEffect =
							this.GetControl().DoDragDrop( new DataObject( typeof( BarItem ).FullName, dndData ),
							this.GetAllowedDragEffects() );
					}
					catch( Exception ex )
					{
						System.Diagnostics.Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
					}

					if( this.bar.Manager.MainFrameBarManager != null )
						this.bar.Manager.MainFrameBarManager.EndDragAndDropCustomizing();

					this.OnEndDragging();
					// If source was not the same as destination
					if( dndHelper.Dragging )
					{
						if( dropEffect == DragDropEffects.Move )
						{
							this.dndHelper.RemoveItem( selectedItem );
							this.CustomizingItemIndex = -1;
						}
						this.dndHelper.Dragging = false;
					}
				}
			}
			else if( !this.Customizing )
			{
				// Transform it to the renderer co-ords
				RectangleF rectMousePos = this.ApplyDrawingTransform( new RectangleF( (PointF)mousePosition, new SizeF( 0, 0 ) ), true );
				mousePosition = new Point( (int)Math.Round( rectMousePos.Left ), (int)Math.Round( rectMousePos.Top ) );

				// Current item is active (not dropped down), so track only that item
				if( this.currentHotTrackItem != -1 )
				{
					IBarItemRenderer renderer = ( (IBarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] );
					if( renderer.Active && !renderer.ShowingDropDown )
					{
						if( renderer.Bounds.Contains( mousePosition ) )
						{
							renderer.OnMouseMove( mousePosition );
							return;
						}
					}
				}

				int newHitBarItem = -1;

				if( this.tdbounds.Contains( mousePosition ) )
					newHitBarItem = HitTestBarItems( mousePosition );

				if( ( !this.IsKeyboardNavigationOn() && !this.IsShowingDropdown() )
					|| ( newHitBarItem >= 0 && ( this.mouseDownItem < 0 || m_bNeedDropDown ) ) )
				{
					m_bIsMouseMoveProcessing = !this.IsMainMenu;

					bool bPrevSuppress = this.SuppressIsMenuClickingChange;
					this.SuppressIsMenuClickingChange = true;

					this.SetCurrentTrackItem( newHitBarItem, false, false );

					this.SuppressIsMenuClickingChange = bPrevSuppress;

					m_bIsMouseMoveProcessing = false;
				}

				Control ctlParent = this.parent.GetControl();
				if (newHitBarItem == -1)
					SuperToolTip.SetToolTips(ctlParent, null);

				if (newHitBarItem != this.currentTooltipItem)
				{
					// Update cache
					this.currentTooltipItem = newHitBarItem;

					bool tooltipSet = false;
					string tooltipText = string.Empty;
					BarItem barItem = null;

					// Set the new tooltipitem
					if( newHitBarItem != -1 )
					{
						barItem = this.bar.Items[newHitBarItem];

						bool bShowTooltip = true;
						ParentBarItem pbi = barItem as ParentBarItem;

						if( null != pbi )
						{
							object barRenderer = this.barItemRenderers[newHitBarItem];
							BarItemRenderer renderer = barRenderer as BarItemRenderer;

							bShowTooltip = !renderer.ShowingDropDown;
						}

						if( bShowTooltip && barItem.ShowTooltip )
						{
							if( barItem.Tooltip != String.Empty )
								tooltipText = barItem.Tooltip;
							else if( !( barItem is StaticBarItem ) )
							{
								tooltipText = barItem.Text;

								// Include the shortcut in the tooltip text
								Shortcut shortcut = barItem.Shortcut;

								if( shortcut != Shortcut.None )
								{
									tooltipText += " (";
									tooltipText += barItem.ShortcutText;
									tooltipText += ")";
								}
							}
							tooltipSet = true;
						}
					}

					Form frmParent = ctlParent.FindForm();
					bool bActiveDockedForm = false;

					if( null != frmParent )
					{
						Control ctlFormParent = frmParent.Parent;

						if( null != ctlFormParent )
						{
							DockHost dhHost = ctlFormParent as DockHost;

							bActiveDockedForm = null != dhHost;
						}
					}

					// Workaround when BarItem is used in a embedded form
					XPToolBar barHost = null;
					if( this.parent.BarHost is XPToolBar )
					{
						barHost = this.parent.BarHost as XPToolBar;
						bActiveFormCheckOverride = barHost.BarItemActiveFormCheckOverride;// For MFC
					}

					if( bActiveDockedForm || frmParent == Form.ActiveForm
						|| ( Form.ActiveForm != null && frmParent == Form.ActiveForm.ActiveMdiChild )
						|| this.IsFloating || bActiveFormCheckOverride )
					{
						if( null != this.parent /*&& this.parent.BarHost is XPToolBar*/ )
						{
							CreateToolTip();
						}

						if( bActiveFormCheckOverride )// Doesn't work in MFC otherwise
							this.tooltip.SetToolTip( ctlParent, string.Empty );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						tooltip.Active = false;
#endif

						this.tooltip.SetToolTip( ctlParent, tooltipSet ? tooltipText : string.Empty );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						tooltip.Active = true;

						SuperToolTip.SetToolTips( ctlParent, barItem );
#endif
					}
				}
			}
		}

		public void SetHotTrack( BarItemRenderer renderer, bool hotTrack )
		{
            if (hotTrack)
            {
                this.clickedItem = this.barItemRenderers.IndexOf(renderer);
                this.SetCurrentTrackItem(this.barItemRenderers.IndexOf(renderer), false, false);
            }
            else if (this.barItemRenderers.IndexOf(renderer) == this.currentHotTrackItem)
                this.ResetHotTracking();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal class EfficientMenuTracker: IDisposable
		{
			private BarRenderer barRenderer;
			PopupCloseType popupCloseType;
			IDropDownItem dropDownItem;
			bool callPostChildClosing = false;
			bool traking = false;

			internal EfficientMenuTracker( BarRenderer br )
			{
				this.barRenderer = br;
			}

			public bool Tracking
			{
				get { return this.traking; }
				set
				{
					if( this.traking != value )
					{
						this.traking = value;
						if( !this.traking )
							this.PostSetCurrentTrackItem();
					}
				}
			}

			public void SetDelayedPostChildClosingArgs( PopupCloseType popupCloseType, IDropDownItem dropDownItem )
			{
				this.callPostChildClosing = true;
				this.popupCloseType = popupCloseType;
				this.dropDownItem = dropDownItem;
			}

			private void PostSetCurrentTrackItem()
			{
				if( this.callPostChildClosing )
				{
					this.callPostChildClosing = false;
					this.barRenderer.PostChildClosing( this.popupCloseType, this.dropDownItem );
				}
			}

			public virtual void Dispose()
			{
				this.Dispose( true );
				GC.SuppressFinalize( this );
			}

			protected virtual void Dispose( bool disposing )
			{
				this.barRenderer = null;
				this.dropDownItem = null;
			}
		}

		/// <summary>
		/// Returns the currently selected bar item in the corresponding toolbar.
		/// </summary>
		/// <value>This will return null if no bar item is currently selected.</value>
		public BarItem SelectedItem
		{
			get
			{
				if( this.currentHotTrackItem == -1 )
					return null;
				else
					return this.Bar.Items[this.currentHotTrackItem];
			}
		}

		private Timer m_hideDropDownTimer = null;
		private DropDownBarItemRenderer m_rendererToHide = null;
		private bool m_bIsMouseMoveProcessing = false;
		private const int DEF_HIDE_DROPDOWN_INTERVAL = 1000;

		private void InitDropDownTimer()
		{
			m_hideDropDownTimer = new Timer();
			m_hideDropDownTimer.Interval = DEF_HIDE_DROPDOWN_INTERVAL;
			m_hideDropDownTimer.Tick += new EventHandler( m_hideDropDownTimer_Tick );
		}

		private void m_hideDropDownTimer_Tick( object sender, EventArgs e )
		{
			m_hideDropDownTimer.Stop();

			if( m_rendererToHide != null )
			{
				PerformHidingDropDown();
			}
		}

		private bool StartHidingDropDown( IBarItemRenderer renderer )
		{
			bool succeed = false;

			DropDownBarItemRenderer dropDownRenderer = renderer as DropDownBarItemRenderer;

			if( !IsMainMenu )
			{
				if( m_bIsMouseMoveProcessing )
				{
					if( !m_hideDropDownTimer.Enabled && dropDownRenderer != null &&
						dropDownRenderer.IsShowing() )
					{
						MainFrameBarManager barManager = this.GetMainManager();

						if( barManager != null && barManager.PopupCloseTimer > 0 )
						{
							m_hideDropDownTimer.Interval = barManager.PopupCloseTimer;
							m_rendererToHide = dropDownRenderer;
							m_hideDropDownTimer.Start();

							succeed = true;
						}
					}
				}
				else if( m_hideDropDownTimer.Enabled )
				{
					StopHidingDropDown();
				}
			}

			return succeed;
		}

		protected internal void CancelHidingDropDown()
		{
			if( !this.IsMainMenu && m_hideDropDownTimer.Enabled )
			{
				BarItemRenderer rendererToShow = ( this.CurrentHotTrackItem == null ) ? null :
					this.barItemRenderers[this.currentHotTrackItem] as BarItemRenderer;

				if( m_rendererToHide != null && rendererToShow != null &&
					rendererToShow != m_rendererToHide )
				{
					rendererToShow.HotTrack = false;
					rendererToShow.BarItem.PerformUnselected();

				}

				if( m_rendererToHide != null && m_rendererToHide.ShowingDropDown )
				{
					m_hideDropDownTimer.Stop();

					this.currentHotTrackItem = this.barItemRenderers.IndexOf( m_rendererToHide );

					m_rendererToHide = null;
				}
			}
		}

		private void StopHidingDropDown()
		{
			if( m_rendererToHide != null )
			{
				m_rendererToHide.HotTrack = false;
				m_rendererToHide.BarItem.PerformUnselected();
				m_rendererToHide = null;
			}

			m_hideDropDownTimer.Stop();
		}

		private void PerformHidingDropDown()
		{
			BarItemRenderer rendererToShow = ( this.CurrentHotTrackItem == null ) ? null :
			   this.barItemRenderers[this.currentHotTrackItem] as BarItemRenderer;

			if( m_rendererToHide != rendererToShow )
			{
				if( m_rendererToHide != null && m_rendererToHide != rendererToShow )
				{
					m_rendererToHide.HotTrack = false;
					m_rendererToHide.BarItem.PerformUnselected();
				}

				if( rendererToShow != null )
				{
					DropDownBarItemRenderer rendererToDropDown = rendererToShow as DropDownBarItemRenderer;

					if( rendererToDropDown != null )
					{
						ignoreMouseMove = true;
						this.ignoreMouseMoveTick = Environment.TickCount + 100;
						rendererToShow.DropDown( true, false, null );
					}
					else
					{
						m_bNeedDropDown = false;
						this.StopKeyboardNavigation();
					}
				}
			}
		}

		protected bool IsMainMenu
		{
			get
			{
				bool bIsMainMenu = ( ( this.bar.BarStyle & BarStyle.IsMainMenu ) !=
					BarStyle.None );

				return bIsMainMenu;
			}
		}

		internal void SetCurrentTrackItem( int newHitBarItem, bool forceDropDown, bool setDefaultSelection, Queue pbiQueue )
		{
			if( null == this.barItemRenderers )
			{
				return;
			}

			if( newHitBarItem >= this.barItemRenderers.Count )
				newHitBarItem = -1;

			if( newHitBarItem == -1 )
			{
				m_bNeedDropDown = false;
			}
			if( newHitBarItem != this.currentHotTrackItem )
			{
				this.efficentTracker.Tracking = true;
				bool showDropDown = forceDropDown;
				// Reset the previous hottrackitem
				if( this.currentHotTrackItem != -1 )
				{
					IBarItemRenderer renderer = ( (IBarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] );

					this.parent.OnRepaint(
						Rectangle.Ceiling( this.ApplyDrawingTransform( Rectangle.Ceiling( renderer.Bounds ), false ) )
						);

					if( StartHidingDropDown( renderer ) )
					{
						showDropDown = false;
					}
					else
					{
						if( m_rendererToHide != renderer || ( m_rendererToHide != null && !m_rendererToHide.IsShowing() ) )
						{
                            if (!(renderer is ComboBoxItemRenderer))
							showDropDown |= m_bNeedDropDown;
							renderer.HotTrack = false;
							renderer.BarItem.PerformUnselected();

						}
					}
				}

				this.currentHotTrackItem = newHitBarItem;

				// Set the new hottrackitem (only if not Customizing)
				if( newHitBarItem != -1 && !this.Customizing )
				{
					IBarItemRenderer renderer = this.barItemRenderers[newHitBarItem] as IBarItemRenderer;
					renderer.HotTrack = true;
					if( renderer.BarItem.Enabled )
					{
						//renderer.BarItem.SetCorrespondingAccessibleObject(this.parent.GetControl().AccessibilityObject.GetFocused());
						renderer.BarItem.PerformSelected();
						//renderer.BarItem.SetCorrespondingAccessibleObject(null);
						BarItemsContainerAccessibleObject containerAccObject = this.parent.GetControl().AccessibilityObject as BarItemsContainerAccessibleObject;
						int childIndex = containerAccObject.GetAccessibleObjectIndexFromBarItem( renderer.BarItem );
						if( childIndex != -1 )
							containerAccObject.NotifyClients( AccessibleEvents.Focus, childIndex );
					}
					if( showDropDown )
					{
						if( !m_hideDropDownTimer.Enabled )
						{
							ignoreMouseMove = true;
							this.ignoreMouseMoveTick = Environment.TickCount + 100;
							renderer.DropDown( showDropDown, setDefaultSelection, pbiQueue );
						}
					}
				}
				this.efficentTracker.Tracking = false;

				if( m_bShouldChangeSelectedItem )
				{
					MainFrameBarManager mainBarManager = this.GetMainManager();
					if( mainBarManager != null )
					{
						mainBarManager.SelectedItem = this.CurrentHotTrackItem;
					}
				}
			}
			else if( forceDropDown )
			{
				// Make sure that the tracking item is dropped down.
				( (IBarItemRenderer)this.barItemRenderers[newHitBarItem] ).DropDown( true, setDefaultSelection, pbiQueue );
			}
		}

		public void SetCurrentTrackItem( int newHitBarItem, bool forceDropDown, bool setDefaultSelection )
		{
			SetCurrentTrackItem( newHitBarItem, forceDropDown, setDefaultSelection, null );
		}

		public int HitTestBarItems( PointF mousePosition )
		{
			int i = 0, hitBarItemIndex = -1;
			foreach( IBarItemRenderer renderer in this.barItemRenderers )
			{
				if( renderer.Visible && renderer.HitTest( mousePosition ) )
				{
					hitBarItemIndex = i;
					break;
				}
				i++;
			}
			return hitBarItemIndex;
		}

		public virtual void OnMouseLeave( EventArgs e )
		{
			bool bIsLeaving = true;
			if( currentHotTrackItem >= 0 )
			{
				EditableComboRenderer renderer = this.barItemRenderers[this.currentHotTrackItem] as EditableComboRenderer;

				if( renderer != null )
				{
					bIsLeaving = !renderer.HitTestInternal();
				}
			}
            
            if(!this.IsShowingDropdown())
             this.StopKeyboardNavigation();
        
			if( this.currentHotTrackItem != -1
				&& !( (IBarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] ).Active
				&& !this.IsKeyboardNavigationOn()
				&& !this.IsShowingDropdown() && bIsLeaving )
				ResetHotTracking();
			// Checking Focused to workaround this problem:
			// When this method gets called and the form is not focussed, the form is brought to front
			// (of other top-level forms in an SDI app). Happens only once during the course of the app.
			// Don't care if floating, though.
			if( this.parent.GetControl().FindForm() == Form.ActiveForm
				|| ( Form.ActiveForm != null
				&& this.parent.GetControl().FindForm() == Form.ActiveForm.ActiveMdiChild )
				|| this.IsFloating )
			{
				this.tooltip.SetToolTip( this.parent.GetControl(), "" );
			}
			this.currentTooltipItem = -1;
		}
		private bool IsFloating
		{
			get
			{
				if( this.bar != null
					&& this.bar.Manager != null
					&& this.bar.Manager.MainFrameBarManager != null
					&& this.GetBarControl().GetControl() != null )
					return this.bar.Manager.MainFrameBarManager.Form
						!= this.GetBarControl().GetControl().FindForm();

				return false;
			}
		}
		protected int selIndexOnDragStart = -1;
		protected bool internalDragging = false;

		protected virtual bool MoveSelection( MoveHint hint )
		{
			bool success = false;

			if( null != this.barItemRenderers )
			{
				switch( hint )
				{
					case MoveHint.moveFirst:
					{
						// First Item
						int newItem = 0;
						// Skip items that cannot be selected
						while( newItem < this.barItemRenderers.Count &&
								//(!this.ShouldDrawVisible(this.bar.Items[newItem])
								( !( (IBarItemRenderer)this.barItemRenderers[newItem] ).Visible
								|| !this.bar.Items[newItem].Enabled
								|| ( this.bar.Items[newItem] is StaticBarItem ) )
							)
							newItem++;

						if( newItem < this.barItemRenderers.Count )
						{
							this.SetCurrentTrackItem( newItem, false, true );
							success = true;
						}

						break;
					}

					case MoveHint.moveLast:
					{
						// Last items
						int newItem = this.barItemRenderers.Count - 1;
						// Skip items that cannot be selected
						while( newItem >= 0 &&
								//(!this.ShouldDrawVisible(this.bar.Items[newItem])
								( !( (IBarItemRenderer)this.barItemRenderers[newItem] ).Visible
								|| !this.bar.Items[newItem].Enabled
								|| ( this.bar.Items[newItem] is StaticBarItem ) )
							)
							newItem--;

						if( newItem >= 0 )
						{
							this.SetCurrentTrackItem( newItem, false, true );
							success = true;
						}

						break;
					}

					case MoveHint.moveNext:
					{
						if( this.currentHotTrackItem == -1 )
						{
							success = this.MoveSelection( MoveHint.moveFirst );
						}
						else
						{
							int newItem = this.currentHotTrackItem + 1;

							// Skip rows that cannot be selected
							while( newItem < this.barItemRenderers.Count
									&&
									//(!this.ShouldDrawVisible(this.bar.Items[newItem])
									( !( (IBarItemRenderer)this.barItemRenderers[newItem] ).Visible
									|| !this.bar.Items[newItem].Enabled
									|| ( this.bar.Items[newItem] is StaticBarItem ) )
								)
								newItem++;

							if( newItem < this.barItemRenderers.Count )
							{
								this.SetCurrentTrackItem( newItem, false, true );
								success = true;
							}
							else
							{
								success = this.MoveSelection( MoveHint.moveFirst );
							}
						}

						break;
					}

					case MoveHint.movePrevious:
					{
						if( this.currentHotTrackItem == -1 )
						{
							success = this.MoveSelection( MoveHint.moveLast );
						}
						else
						{
							int newItem = this.currentHotTrackItem - 1;

							// Skip items that cannot be selected
							while( newItem >= 0 &&
									//(!this.ShouldDrawVisible(this.bar.Items[newItem])
									( !( (IBarItemRenderer)this.barItemRenderers[newItem] ).Visible
									|| !this.bar.Items[newItem].Enabled
									|| ( this.bar.Items[newItem] is StaticBarItem ) )
								)
								newItem--;

							if( newItem >= 0 )
							{
								this.SetCurrentTrackItem( newItem, false, true );
								success = true;
							}
							else
							{
								success = this.MoveSelection( MoveHint.moveLast );
							}
						}

						break;
					}
				}
			}

			return success;
		}

		protected virtual void OnRightMouseDown( MouseEventArgs e )
		{
			Point mousePosition = new Point( e.X, e.Y );
			// Transform it to the renderer co-ords
			RectangleF rectMousePos = this.ApplyDrawingTransform( new RectangleF( (PointF)mousePosition, new SizeF( 0, 0 ) ), true );
			mousePosition = new Point( (int)Math.Round( rectMousePos.Left ), (int)Math.Round( rectMousePos.Top ) );

			int hitItem = this.HitTestBarItems( mousePosition );

			if( hitItem != -1 )
			{
				if( this.customizingItemIndex != hitItem )
					this.OnMouseDown( new MouseEventArgs( MouseButtons.Left, e.Clicks,
						e.X, e.Y, e.Delta ) );

				if( this.customizingItemIndex == -1 )
					return;

				if( hitItem == this.customizingItemIndex )
				{
					this.CloseDropdowns();

					this.ValidateDndHelper();

					CustomizingPopupMenu custPopupMenu = this.CustomizationPopup;

					if( null != custPopupMenu && this.bar.Manager != null)
					{
						custPopupMenu.Show( this.parent.GetControl(), new Point( e.X, e.Y ),
							this.bar.Items[this.customizingItemIndex] as BarItem,
							this.bar, this.dndHelper );
					}
				}
			}
		}

		private bool m_bActivateFormFromBar = false;

		public virtual bool OnMouseDown( MouseEventArgs e )
		{
			CancelHidingDropDown();
            if ((this.clickedItem >= 0 && !(this.SelectedItem is ComboBoxBarItem)) || ((this.SelectedItem is ComboBoxBarItem) && ((!(this.SelectedItem as ComboBoxBarItem).Editable))))
            {
                if ((this.SelectedItem is ComboBoxBarItem))
                {
                    int i = 0;
                    foreach (BarItemRenderer br in this.barItemRenderers)
                    {                        
                        if (br.BarItem != null && br.BarItem == this.SelectedItem)
                        {
                            this.clickedItem = i;
                        }
                        i++;
                    }
                }
                else
                    this.clickedItem = -1;
            }
			if( this.barItemRenderers.Count <= 0
				|| this.IgnoreMouseUpDown )
			{
				return false;
			}

			if( m_bActivateFormFromBar )
			{
				if( bar.manager != null && bar.manager.Form != null )
				{
					if( !bar.manager.Form.Focused )
					{
						bar.manager.Form.Activate();
					}
				}
			}

			bool customizing = this.Customizing;
			bool showingDropDown = this.IsShowingDropdown();
			int oldHotTrackItem = this.currentHotTrackItem;
            if (this.currentHotTrackItem >= 0)
            {
                foreach (BarItemRenderer br in this.barItemRenderers)
                {
                    if (this.barItemRenderers[this.currentHotTrackItem] != br)
                    {
                        if (br != null && (br is ComboBoxItemRenderer))
                        {
                            br.Active = false;
                        }
                    }
                }
            }

			IntPtr hWnd = Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus();
			Control withFocus = Control.FromHandle( hWnd );
			if( withFocus is IToolbarControl )
			{
				( (IToolbarControl)withFocus ).OnMouseDownOutside();
			}

			// Get the mouse positions
			Point mousePosition = new Point( e.X, e.Y );

			// Transform it to the renderer co-ords
			RectangleF rectMousePos = this.ApplyDrawingTransform( new RectangleF( (PointF)mousePosition, new SizeF( 0, 0 ) ), true );
			mousePosition = new Point( (int)Math.Round( rectMousePos.Left ), (int)Math.Round( rectMousePos.Top ) );

			int hitBarItemIndex = mouseDownItem = this.HitTestBarItems( mousePosition );
			// Start customization if Alt key down
			if( ( Control.ModifierKeys == Keys.Alt
				|| Control.ModifierKeys == ( Keys.Control | Keys.Alt ) )
				&& e.Button == MouseButtons.Left
				&& ( this.bar.AllowCustomizing || this.DesignMode ) )
			{
				if( this.bar.Manager != null && this.bar.Manager.MainFrameBarManager != null )
				{
					if( hitBarItemIndex != -1 )
					{
						BarItem hitItem = this.Bar.Items[hitBarItemIndex];
						if( !this.bar.Manager.MainFrameBarManager.IsItemCustomizable( hitItem ) )
							return false;
					}
					this.bar.Manager.MainFrameBarManager.StartDragAndDropCustomizing( this );
					customizing = this.Customizing;
				}
			}

			if( e.Button == MouseButtons.Right )
			{
				if( customizing && ( this.bar.AllowCustomizing || this.DesignMode ) )
				{
					this.OnRightMouseDown( e );
					if( this.bar.Manager != null && this.bar.Manager.MainFrameBarManager != null )
						this.bar.Manager.MainFrameBarManager.EndDragAndDropCustomizing();
					return true;
				}
				return false;
			}

			if( this.currentHotTrackItem == -1 || this.currentHotTrackItem != hitBarItemIndex )
			{
				// Inactivate the currentHitItem
				this.ResetHotTracking();
			}

			if( customizing )
			{
				if( this.bar.AllowCustomizing || this.DesignMode )
				{
					if( hitBarItemIndex != -1 )
					{
						this.CustomizingItemIndex = hitBarItemIndex;
						if( this.bar.Manager != null )
						{
							if( this.customizingItemIndex != -1 )
								this.bar.Manager.CustomizingItem
									= this.bar.Items[this.customizingItemIndex];
							else
								this.bar.Manager.CustomizingItem = null;
						}
					}
				}
			}

			if( hitBarItemIndex != -1 )
			{
				ignoreMouseMove = true;
				this.ignoreMouseMoveTick = Environment.TickCount + 100;
				// Need this since sometimes mouse down will be called before any mouse move.
				this.SetCurrentTrackItem( hitBarItemIndex, false, false );

				if( !customizing && !this.IsShowingDropdown() /*&& this.bar.Manager != null*/)
				{

					if( this.IsKeyboardNavigationOn() )
					{
						m_bKeyboardNavigationStartedByMouse = false;
					}
					else
					{
						m_bKeyboardNavigationStartedByMouse = true;
						//PopupManager.SetCurrentPopupClient(this, true, false/*!this.bar.Manager.DesignMode*/);
						this.StartKeyboardNavigation();
					}
				}

				if( !customizing || this.bar.AllowCustomizing || this.DesignMode )
					( (IBarItemRenderer)this.barItemRenderers[hitBarItemIndex] ).OnMouseDown( mousePosition );
			}
			else
				this.StopKeyboardNavigation();

			if( !customizing )
			{
				this.SetCurrentTrackItem( hitBarItemIndex, false, false );
			}
			if( !showingDropDown && this.IsShowingDropdown()
				|| ( this.IsShowingDropdown() && ( oldHotTrackItem != this.currentHotTrackItem ) ) )
			{
				this.mouseDownPoint = new Point( e.X, e.Y );

				m_bNeedDropDown = true;
				this.IgnoreMouseUpDown = true;
				this.ignoreMouseUp = true;
			}

			BarManager barMan = this.Bar.Manager;

			if( null != barMan && null != barMan.MainFrameBarManager )
			{
				if( ( m_bXPMenuActive && this.ActivateFormFromBar ) || hitBarItemIndex < 0 )
				{
					barMan.MainFrameBarManager.Form.Activate();
				}
			}
            if(this.SelectedItem!=null)
            this.SelectedItem.OnItemMouseDown(e);

			return false;
		}

		private bool m_bShouldChangeSelectedItem = true;

		public virtual void OnMouseUp( MouseEventArgs e )
		{
			m_bShouldChangeSelectedItem = false;

			if( m_bDragging && this.CustomizingItemIndex != -1 && this.bar.Manager != null && this.bar.Manager.MainFrameBarManager != null
				&& this.bar.Manager.MainFrameBarManager.DndCustomizing )
			{
				this.bar.Manager.MainFrameBarManager.EndDragAndDropCustomizing();
				this.OnEndDragging();
			}

			if( this.IgnoreMouseUpDown || this.ignoreMouseUp )
			{
				this.ignoreMouseUp = false;
				return;
			}

			IBarItemRenderer renderer = null;

			if( this.currentHotTrackItem >= 0 || this.mouseDownItem >= 0 )
			{
				int item = this.mouseDownItem;

				if( item < 0 )
				{
					item = this.currentHotTrackItem;
				}

				if( item >= 0 )
				{
					renderer = ( (IBarItemRenderer)this.barItemRenderers[item] );

					// Get the mouse positions
					Point mousePosition = new Point( e.X, e.Y );

					// Transform it to the renderer co-ords
					RectangleF rectMousePos = this.ApplyDrawingTransform( new RectangleF( (PointF)mousePosition, new SizeF( 0, 0 ) ), true );
					mousePosition = new Point( (int)Math.Round( rectMousePos.Left ), (int)Math.Round( rectMousePos.Top ) );

					renderer.OnMouseUp( mousePosition );
				}
			}

			if( this.DelayedPerformClickOnBarItem != null )
			{
				m_bShouldChangeSelectedItem = false;
			}

			if( !this.IsShowingDropdown() )
				this.StopKeyboardNavigation();

			// Necessary to release capture in case, the handler starts a message pump.
			this.parent.GetControl().Capture = true;
			this.parent.GetControl().Capture = false;
            
            // Removed the Item Click event on Mouse up and implemented it Click and Double click sepearately 
            //(Bug Report SD - 1244)
            // -Kathir

            if (this.DelayedPerformClickOnBarItem != null)
            {
                BarItem clickedItem = this.DelayedPerformClickOnBarItem;
                //// Set this to null before invoking Click. 
                //// Otherwise if the user called Application.DoEvents in a loop in the Click handler
                //// then DelayedPerformClickOnBarItem will not be cleared until we come out of the DoEvents loop.
                this.DelayedPerformClickOnBarItem = null;
                clickedItem.PerformClick();
                clickedItem.OnItemMouseUp(e);
            }

			this.ignoreMouseMove = false;
			this.ignoreMouseMoveTick = int.MinValue;
			m_bShouldChangeSelectedItem = true;
		}

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseClick"/>.
        /// </summary>
        /// <param name="e"></param>
        //public virtual void OnMouseClick(MouseEventArgs e)
        //{
        //    BarItem clickedItem = this.CurrentHotTrackItem;
        //    if (clickedItem != null)
        //        clickedItem.PerformClick();
        //}

        /// <summary>
        /// See <see cref="System.Windows.Forms.Control.OnMouseDoubleClick"/>.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            
            BarItem clickedItem = this.CurrentHotTrackItem;
            if (clickedItem != null)
                clickedItem.PerformDoubleClick();
        }

		public virtual void OnMouseWheel( MouseEventArgs e )
		{
            if (currentHotTrackItem == -1)
                currentHotTrackItem = this.clickedItem >= 0 ? this.clickedItem : currentHotTrackItem;

			if( this.currentHotTrackItem >= 0 && this.barItemRenderers.Count > this.currentHotTrackItem )
			{
				IBarItemRenderer renderer = ( (IBarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] );
				ComboBoxItemRenderer comboRenderer = renderer as ComboBoxItemRenderer;
				bool needProcess = true;

				if( comboRenderer != null )
				{
					needProcess = !comboRenderer.IgnoreExternalMouseMessages;
				}

				if( needProcess)
                {
                    if (this.SelectedItem is ComboBoxBarItem)
                    {
                        if (this.clickedItem >= 0)
                        {
                            renderer = ((IBarItemRenderer)this.barItemRenderers[this.clickedItem]);
                            comboRenderer = renderer as ComboBoxItemRenderer;
                            if(!renderer.ShowingDropDown &&  renderer.Active)
                            renderer.OnMouseWheel(e.Delta > 0);
                        }
                        else if (!(this.SelectedItem as ComboBoxBarItem).Editable)
                        {
                            if (!renderer.ShowingDropDown)
                                renderer.OnMouseWheel(e.Delta > 0);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                        renderer.OnMouseWheel(e.Delta > 0);
                }
			}
		}

		public virtual bool ProcessShortcut( char c )
		{
			bool bProcessed = false;

			if( null != this.bar )
			{
				char ckey = Char.ToLower( c );
				BarItem item = this.bar.Items.GetItemFromHotKey( ckey );

				if( item != null && item.Visible && item.Enabled )
				{
					int newSelIndex = this.bar.Items.IndexOf( item );
					IBarItemRenderer renderer = this.barItemRenderers[newSelIndex] as IBarItemRenderer;

					// Process the shortcut only if the item's text is being drawn.
					if( renderer.ShouldDrawText() )
					{
						if( renderer as DropDownBarItemRenderer != null )
						{
							this.StartKeyboardNavigation();
							this.SetCurrentTrackItem( newSelIndex, true, true );
							this.parent.GetControl().Update();
						}
						else
						{
							item.PerformClick();
						}
					}

					bProcessed = true;
				}
			}

			return bProcessed;
		}

		public virtual bool ProcessMnemonic( char charCode )
		{
			if( !this.IsKeyboardNavigationOn() )
			{
				if( !this.IsShowingDropdown() && !this.Customizing
					&& ( Control.ModifierKeys & Keys.Alt ) != Keys.None )
				{
					return this.ProcessShortcut( charCode );
				}
				else
				{
					return false;
				}
			}


			if( !this.IsShowingDropdown() && !this.Customizing )
			{
				if( Control.ModifierKeys == Keys.None )
				{
					BarItem item = this.bar.Items.GetItemFromHotKey( Char.ToLower( charCode ) );
					if( item != null )
					{
						int newSelIndex = this.bar.Items.IndexOf( item );
						this.SetCurrentTrackItem( newSelIndex, true, true );
						this.parent.GetControl().Update();

						// If not a DropDown item, PerformClick and go back to the old item
						//					BarItemRenderer renderer = this.barItemRenderers[newSelIndex] as BarItemRenderer;
						//					if(!(renderer is IDropDownItem))
						//					{
						//						item.PerformClick();
						//
						//						if(oldSelIndex != -1 && 
						//							(this.Bar.BarStyle & BarStyle.IsMainMenu) > 0)
						//							this.SetCurrentTrackItem(oldSelIndex, false, true);
						//					}

						return true;
					}
				}
				else if( ( Control.ModifierKeys & Keys.Alt ) != Keys.None )
				{
					return this.ProcessShortcut( charCode );
				}
			}

			return false;
		}

		//		bool IMouseHookProcClient.MouseHookProc(int nCode, int wParam, Point point, int dwExtraInfo)
		//		{
		//			if (nCode == 0)
		//			{
		//				IntPtr hWnd = IntPtr.Zero;
		//				switch (wParam)
		//				{
		//					case  NativeMethods.WM_RBUTTONDOWN:
		//						hWnd = NativeMethods.WindowFromPoint(point.X, point.Y);
		//						if(hWnd == this.parent.GetControl().Handle)
		//						{
		//							Point clPoint = this.parent.GetControl().PointToClient(point);
		//							this.OnRightMouseDown(new MouseEventArgs(MouseButtons.Right,
		//								1, clPoint.X, clPoint.Y, 0));
		//							return true;
		//						}
		//						break;
		//					case  NativeMethods.WM_RBUTTONUP: // 0x0021 
		//					{
		//						hWnd = NativeMethods.WindowFromPoint(point.X, point.Y);
		//						// Prevent the Designer from getting this RButtonUp
		//						if(hWnd == this.parent.GetControl().Handle)
		//						{
		//							// Prevent the message from being sent to other hooks and windows
		//							return true;
		//						}
		//					}
		//						break;
		//				}
		//			}
		//			return false;
		//		}

		private IPopupControlContainer GetParentPopupContainer( Control control )
		{
			if( control == null )
				throw new ArgumentNullException( "control" );

			IPopupControlContainer parentPopup = null;

			while( control != null )
			{
				parentPopup = control.Parent as IPopupControlContainer;

				if( parentPopup != null )
					break;
				else
					control = control.Parent;

			}

			return parentPopup;
		}

		protected virtual bool ProcessKeyDown( Keys key )
		{
			bool handled = false;

			Control focusedControl = Control.FromHandle( NativeMethods.GetFocus() );

			if( focusedControl != null )
			{
				IPopupControlContainer parentPopup = GetParentPopupContainer( focusedControl );

				if( parentPopup != null && ( parentPopup.PopupHost != null || parentPopup is MenuGrid ) )
				{
					return false;
				}
			}

			if( currentHotTrackItem >= 0 && currentHotTrackItem < barItemRenderers.Count )
			{
				IBarItemRenderer itemRenderer = this.barItemRenderers[this.currentHotTrackItem] as IBarItemRenderer;

				if( itemRenderer != null )
				{
					EditableComboRenderer editableComboRenderer = itemRenderer as EditableComboRenderer;
					if( editableComboRenderer != null && editableComboRenderer.TextBox.Focused )
					{
						return false;
					}

					handled = itemRenderer.ProcessKeyDown( key );

					#region DropDownBarItem handling
					if( !handled )
					{
						DropDownBarItemRenderer dropDownRenderer = itemRenderer as DropDownBarItemRenderer;

						if( null != dropDownRenderer && dropDownRenderer.IsShowing() )
						{
							DropDownBarItem ddbi = dropDownRenderer.BarItem as DropDownBarItem;

							if( null != ddbi )
							{
								PopupControlContainer popupContainer = ddbi.PopupControlContainer;	// Not null if showing
								Form activeForm = this.parent.GetControl().FindForm();
								Control activeCtl = null;

								if( null != activeForm )
								{
									//Fix for the defect 1171: Navigation using keyboard in treeViewAdv contained in a DropDownBarItem fails to work
									activeCtl = activeForm.ActiveControl != null ? activeForm.ActiveControl : popupContainer.Controls[0];
								}

								if( null == activeCtl || ( activeCtl is Form ) )
								{
									activeCtl = Control.FromHandle( NativeMethods.GetActiveWindow() );
								}

								PopupHost popupHost = activeCtl as PopupHost;

								if( null != popupHost && null != popupHost.PopupControlContainer )
								{
									activeCtl = popupHost.PopupControlContainer as Control;
								}

								while( null != activeCtl && popupContainer != activeCtl )
								{
									activeCtl = activeCtl.Parent;
								}

								if( popupContainer == activeCtl )
								{
									return false;
								}
							}
						}
					}
					#endregion

					if( handled ) return true;
				}
			}
			if( !this.Customizing )
			{
				if( this.CurrentHotTrackItem != null && !this.CurrentHotTrackItem.Enabled )
				{
					return handled;
				}

				bool bShift = ( Control.ModifierKeys & Keys.Shift ) != Keys.None;
				bool bCtrl = ( Control.ModifierKeys & Keys.Control ) != Keys.None;
				bool bRTL = ( null != this.parent ) ? this.parent.IsRightToLeft : false;

				if( key == Keys.Tab && bCtrl && this.GetBarControl().ShouldPreProcessTab() )
				{
					if( bShift )
					{
						this.parent.BarHost.MoveMenuNavigation( false/*backward*/);
					}
					else
					{
						this.parent.BarHost.MoveMenuNavigation( true/*forward*/);
					}
				}
				else if( key == Keys.Left )
				{
					m_bNeedDropDown = this.IsShowingDropdown();
					SetIgnoreAltKeyUp( true );
                    handled = bRTL ? this.MoveSelection(MoveHint.moveNext) : this.MoveSelection(MoveHint.movePrevious);
				}
				else if( key == Keys.Right )
				{
					m_bNeedDropDown = this.IsShowingDropdown();
					SetIgnoreAltKeyUp( true );
                    handled = bRTL ? this.MoveSelection(MoveHint.movePrevious) : this.MoveSelection(MoveHint.moveNext);
                }
                else if (key == Keys.Tab && this.GetBarControl().ShouldPreProcessTab())
                {
                    m_bNeedDropDown = this.IsShowingDropdown();
                    SetIgnoreAltKeyUp(true);
                    handled = bShift ? this.MoveSelection(MoveHint.movePrevious) : this.MoveSelection(MoveHint.moveNext);
                }
				else if( key == Keys.Home )
				{
					SetIgnoreAltKeyUp( true );
					handled = this.MoveSelection( MoveHint.moveFirst );
				}
				else if( key == Keys.End )
				{
					SetIgnoreAltKeyUp( true );
					handled = this.MoveSelection( MoveHint.moveLast );
				}
				else if( key == Keys.Down || key == Keys.Enter )
				{
					SetIgnoreAltKeyUp( true );

					if( this.currentHotTrackItem != -1 )
					{
						ignoreMouseMove = true;
						this.ignoreMouseMoveTick = Environment.TickCount + 100;
						BarItemRenderer renderer = ( (BarItemRenderer)this.barItemRenderers[this.currentHotTrackItem] );
						if( !( renderer is IDropDownItem ) )
						{
							if( key == Keys.Enter )
							{
								BarItem item = renderer.BarItem;

								if( !m_bKeyboardNavigationStartedByMouse )
									this.StopKeyboardNavigation();

								// Just in case... see notes in the other Capture = false call.
								this.parent.GetControl().Capture = false;
								if( item.Enabled )
								{
									item.PerformClick();
									handled = true;
								}
							}
						}
						else
						{
							if( !renderer.ShowingDropDown )
							{
								m_bNeedDropDown = true;
								renderer.DropDown( true, true, null );
								handled = true;
							}
							else if( key == Keys.Enter )
							{
								renderer.ShowingDropDown = false;
								handled = true;
							}
						}
					}
				}
				else if( key == Keys.Escape )
				{
					this.OnHitEscape();
					handled = true;
				}             

				if( !handled )
				{
					if( key >= Keys.D0 && key <= Keys.Z )
					{
						this.ProcessMnemonic( (char)(int)key );
						handled = this.IsKeyboardNavigationOn();
					}
				}

				if( !handled && IsKeyboardNavigationOn() )
				{
					if( ( ( ( key & Keys.Oem8 ) != Keys.None ) ||
						key == Keys.Space ) && ( key < Keys.F1 || key > Keys.F12 ) )
					{
                        if (!bShift)
                            handled = true;
                        else
                            handled = false;
					}
				}
			}
			return handled;
		}

		private void SetIgnoreAltKeyUp( bool bIgnore )
		{
			if( null != this.Bar )
			{
				MainFrameBarManager mainBarMan = this.Bar.manager as MainFrameBarManager;

				if( null != mainBarMan )
				{
					mainBarMan.ignoreNextAltKeyUp = bIgnore;
				}
			}
		}

		private void OnHitEscape()
		{
			//( this.bar.manager as MainFrameBarManager ).ignoreNextAltKeyUp = false;
			m_bActivateOnExit = IsShowingDropdown();
			this.ResetHotTracking();

			this.StopKeyboardNavigation();

			if( m_bActivateOnExit )
			{
				this.StartKeyboardNavigation();
				SetIgnoreAltKeyUp( true );
			}
			else
			{
				SetIgnoreAltKeyUp( false );
			}
		}
		public virtual bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			bool handled = false;

			if( this.IsKeyboardNavigationOn() )
			{
				Keys key = keyData;
				handled = this.ProcessKeyDown( key );
			}
			else if( this.IsShowingDropdown() && keyData == Keys.Escape )
			{
				// Special case where the barcontrol has focus and hit escape when combo is dropdown.
				this.OnHitEscape();
			}

			return handled;
		}
		protected void ResetHotTracking()
		{
			if( this.currentHotTrackItem != -1 )
			{
				int prevHotTrackItem = this.currentHotTrackItem;
				this.currentHotTrackItem = -1;
				if( prevHotTrackItem < this.barItemRenderers.Count )
				{
					( (IBarItemRenderer)this.barItemRenderers[prevHotTrackItem] ).HotTrack = false;
					( (IBarItemRenderer)this.barItemRenderers[prevHotTrackItem] ).Active = false;
					( (IBarItemRenderer)this.barItemRenderers[prevHotTrackItem] ).BarItem.PerformUnselected();
				}

				MainFrameBarManager mainBarMan = this.GetMainManager();
				if( mainBarMan != null )
				{
					mainBarMan.SelectedItem = null;
				}
			}
		}
		protected virtual void ResetDragging()
		{
			this.mouseDownItem = -1;
		}

		#region DNDSTUFF
		protected internal virtual void ValidateDndHelper()
		{
			if( this.dndHelper == null )
				this.dndHelper = new CustomizationDndHelper( this );
			this.dndHelper.ParentItem = this.bar;
		}
		protected virtual DragDropEffects GetAllowedDragEffects()
		{
			return DragDropEffects.Copy | DragDropEffects.None | DragDropEffects.Move;
		}
		public virtual void OnDragOver( DragEventArgs drgevent )
		{
			if( !this.Customizing || !( this.bar.AllowCustomizing || this.DesignMode ) )
			{
				//drgevent.Effect = DragDropEffects.None;
				return;
			}

			this.ValidateDndHelper();

			this.dndHelper.OnDragOver( drgevent );

			PointF mousePos = this.parent.GetControl().PointToClient( new Point( drgevent.X, drgevent.Y ) );
			mousePos = this.ApplyDrawingTransform( mousePos, true );

			int hitItemIndex = this.HitTestBarItems( mousePos );
			// Close any drop downs that are not over the current mouse pos.
			if( hitItemIndex != this.currentDndDropDown && this.currentDndDropDown != -1 )
			{
				IBarItemRenderer renderer = this.barItemRenderers[currentDndDropDown] as IBarItemRenderer;
				renderer.DropDown( false, false, null );
			}
			if( this.currentHotTrackItem != hitItemIndex && this.currentHotTrackItem != -1 )
			{
				IBarItemRenderer renderer = this.barItemRenderers[currentHotTrackItem] as IBarItemRenderer;
				renderer.DropDown( false, false, null );
			}

			if( hitItemIndex != -1
				&& !( this.dndHelper.Dragging && this.currentHotTrackItem == hitItemIndex ) )
			{
				IBarItemRenderer renderer = this.barItemRenderers[hitItemIndex] as IBarItemRenderer;
				this.currentDndDropDown = hitItemIndex;
                renderer.Active = true;
				renderer.DropDown( true, false, null );
				if( !renderer.ShowingDropDown )
					this.currentDndDropDown = -1;
			}

			// If there are no current drop downs, then set myself the active popup

			if( this.currentDndDropDown == -1 )
			{
				// Make myself the active popup. This will hide drop downs in other bars
				// PopupManager.SetCurrentPopupClient(this, true, false);

				// Or just hide the active popup
				if( PopupManager.ActivePopupClient != null )
				{
					PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Deactivated );
				}
			}
		}

		public virtual void OnDragDrop( System.Windows.Forms.DragEventArgs e )
		{
			if( !( this.bar.AllowCustomizing || this.DesignMode ) )
				return;

			this.ValidateDndHelper();
			this.dndHelper.OnDragDrop( e );

			if( this.bar.Manager != null )
				this.CustomizingItemIndex = this.bar.Items.IndexOf( this.bar.Manager.CustomizingItem );
			// Doing this will let me know that source is same as destination (when DoDragDrop returns)
			this.dndHelper.Dragging = false;
			this.ResetHotTracking();
			this.CloseDropdowns();
		}

		public virtual void OnDragLeave( EventArgs e )
		{
			if( !( this.bar.AllowCustomizing || this.DesignMode ) )
				return;

			this.ValidateDndHelper();
			if( this.dndHelper != null )
				this.dndHelper.Reset();
		}

		public virtual void OnGiveFeedback( GiveFeedbackEventArgs gfbevent )
		{
			switch( gfbevent.Effect )
			{
				case DragDropEffects.Copy:
				Cursor.Current = DragCursors.CopyCursor;
				gfbevent.UseDefaultCursors = false; break;
				case DragDropEffects.Move:
				Cursor.Current = DragCursors.DragCursor;
				gfbevent.UseDefaultCursors = false; break;
				case DragDropEffects.None:
				Cursor.Current = DragCursors.NodropCursor;
				gfbevent.UseDefaultCursors = false; break;
			}
		}

		#region IDndTrackingControl_Override
		public Control GetControl()
		{
			return this.parent.GetControl();
		}

		public bool DesignMode
		{
			get
			{
				bool bDesignMode = false;

				if( null != this.parent )
				{
					bDesignMode = this.parent.DesignMode;

					if( !bDesignMode )
					{
						BarControlInternal bcIntenral = parent as BarControlInternal;

						if( bcIntenral != null && bcIntenral.Bar != null )
						{
							bDesignMode = bcIntenral.Bar.DesignMode;
						}
					}

				}

				if( this.bar != null )
				{
					BarManager barMgr = this.bar.Manager;

					if( null != barMgr )
					{
						bDesignMode = barMgr.DesignMode;
					}
				}

				return bDesignMode;
			}
		}
		public bool CanDropItem( BarItem item )
		{
			if( item is ListBarItem )
				return false;
			else
				return true;
		}
		public BarItem HitTestBarItem( Point mousePosition, ref bool beforeOrAfter )
		{
			// Transform it to the renderer co-ords
			RectangleF rectMousePos = this.ApplyDrawingTransform( new RectangleF( (PointF)mousePosition, new SizeF( 0, 0 ) ), true );
			Point transformedMousePosition = new Point( (int)Math.Round( rectMousePos.Left ), (int)Math.Round( rectMousePos.Top ) );

			int hitIndex = this.HitTestBarItems( transformedMousePosition );

			if( hitIndex >= 0 )
			{
				BarItemRenderer renderer = ( (BarItemRenderer)this.barItemRenderers[hitIndex] );
				BarItem selected = renderer.BarItem;
				Rectangle itemBounds = Rectangle.Ceiling( this.ApplyDrawingTransform( Rectangle.Ceiling( renderer.Bounds ), false ) );
				if( ( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
				{
					if( mousePosition.Y - itemBounds.Top
						> itemBounds.Bottom - mousePosition.Y )
						beforeOrAfter = false;
					else
						beforeOrAfter = true;
				}
				else
				{
					if( mousePosition.X - itemBounds.Left
						> itemBounds.Right - mousePosition.X )
						beforeOrAfter = false;
					else
						beforeOrAfter = true;
				}

				return selected;
			}
			else
			{
				// Check if before the first separator
				if( this.bar.Items.Count > 0 )
				{
					if( this.bar.IsGroupBeginning( this.bar.Items[0] )
						&& this.ShouldDrawVisible( this.bar.Items[0], true ) )
					{
						// If the first item has a separator and mouse before it, then return
						// null and beforeOrAfter == true
						BarItemRenderer renderer = this.barItemRenderers[0] as BarItemRenderer;
						Rectangle rendererBounds =
							Rectangle.Ceiling( this.ApplyDrawingTransform( Rectangle.Ceiling( renderer.Bounds ), false ) );
						Rectangle firstSeparatorBounds = Rectangle.Empty;
						// Horizontal
						if( !( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
							firstSeparatorBounds
								= new Rectangle( rendererBounds.Left - 3, rendererBounds.Top, 3, rendererBounds.Height );
						// Vertical
						else
							firstSeparatorBounds
								= new Rectangle( rendererBounds.Left, rendererBounds.Top - 3, rendererBounds.Width, 3 );

						if( firstSeparatorBounds.Contains( mousePosition ) )
							beforeOrAfter = true;
					}
				}
				else
					beforeOrAfter = true; // Allow inserting the first barItem.
			}

			if( beforeOrAfter == false )
			{
				// No items in the list, insert in front.
				bool atleast1ItemVisible = false;
				foreach( BarItemRenderer renderer in this.barItemRenderers )
				{
					if( renderer.Bounds.Width > 0 )
					{
						atleast1ItemVisible = this.ShouldDrawVisible( renderer.BarItem, true );
						break;
					}
				}
				if( !atleast1ItemVisible )
					beforeOrAfter = true;
			}
			return null;
		}

		public Rectangle GetBoundsOf( BarItem item )
		{
			if( this.ShouldDrawVisible( item, true ) )
			{
				int itemIndex = this.bar.Items.IndexOf( item );
				if( itemIndex != -1 )
				{
					BarItemRenderer renderer = this.barItemRenderers[itemIndex] as BarItemRenderer;
					return Rectangle.Ceiling( this.ApplyDrawingTransform( renderer.Bounds, false ) );
				}
			}

			return Rectangle.Empty;
		}
		public virtual PopupRelativeAlignment GetFirstPopupAlignPreference()
		{
			bool bRTL = null != this.parent ? this.parent.IsRightToLeft : false;
			PopupRelativeAlignment praAlign = bRTL ? PopupRelativeAlignment.BottomRight : PopupRelativeAlignment.BottomLeft;

			switch( this.Alignment )
			{
				default:
				case CommandBarDockState.Top:
				case CommandBarDockState.Float:
				praAlign = bRTL ? PopupRelativeAlignment.BottomRight : PopupRelativeAlignment.BottomLeft;
				break;
				case CommandBarDockState.Left:
				praAlign = bRTL ? PopupRelativeAlignment.LeftBottom : PopupRelativeAlignment.RightBottom;
				break;
				case CommandBarDockState.Bottom:
				praAlign = bRTL ? PopupRelativeAlignment.TopLeft : PopupRelativeAlignment.TopRight;
				break;
				case CommandBarDockState.Right:
				praAlign = bRTL ? PopupRelativeAlignment.RightBottom : PopupRelativeAlignment.LeftBottom;
				break;
			}

			return praAlign;
		}
		public IBarControl GetBarControl()
		{
			return this.parent;
		}

		public Rectangle GetCueRect( BarItem barItem, bool beforeOrAfter )
		{
			if( barItem == null && beforeOrAfter )
			{
				Rectangle cueRect = Rectangle.Empty;
				if( this.bar.Items.Count > 0 )
				{
					// Inserting before the first separator
					Rectangle firstItemBounds = this.GetBoundsOf( this.bar.Items[0] );
					if( firstItemBounds.Width > 0 && firstItemBounds.Height > 0 )
					{
						// 3 pixes for separator and 2 for padX
						firstItemBounds.Offset( -5, 0 );
						firstItemBounds.Width = 7;
						cueRect = firstItemBounds;
					}
					else
					{
						Rectangle myBounds = Rectangle.Ceiling( this.Bounds );
						cueRect = new Rectangle( myBounds.Left, myBounds.Top, 7, myBounds.Height );
					}
				}
				else
				{
					Rectangle insertionCorner = Rectangle.Ceiling( this.tdbounds );
					insertionCorner.Width = 7;
					cueRect = insertionCorner;
				}
				cueRect = Rectangle.Ceiling( this.ApplyDrawingTransform( cueRect, false ) );
				return cueRect;
			}

			Rectangle itemBounds = this.GetBoundsOf( barItem );
			if( beforeOrAfter )
			{
				if( ( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
					return new Rectangle( itemBounds.Left, itemBounds.Top, itemBounds.Width, 7 );
				else
					return new Rectangle( itemBounds.Left, itemBounds.Top, 7, itemBounds.Height );
			}
			else
			{
				if( ( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
					return new Rectangle( itemBounds.Left, itemBounds.Bottom - 8, itemBounds.Width, 7 );
				else
					return new Rectangle( itemBounds.Right - 8, itemBounds.Top, 7, itemBounds.Height );
			}
		}

		#endregion IDndTrackingControl_Override

		#endregion DNDSTUFF

		#region Private event handlers

		private void frmParent_ParentChanged( object sender, EventArgs e )
		{
			CreateToolTip();
		}

		private void ctlParent_ParentChanged( object sender, EventArgs e )
		{
			Control ctlParent = this.parent.GetControl();
			Form frmParent = ctlParent.FindForm();

			if( !this.DesignMode )
			{
				if( ctlParent == null )
				{
					MessageFilterEntryHelper.RemoveMessageFilter( this );
				}
				else
				{
					if( this.bar != null && this.bar.manager != null )
					{
						MessageFilterEntryHelper.AddMessageFilter( this, false, this.bar.manager.Form );
					}
					else
					{
						MessageFilterEntryHelper.AddMessageFilter( this, false );
					}
				}
			}

			if( null != frmParent )
			{
				frmParent.ParentChanged += new EventHandler( frmParent_ParentChanged );
			}
		}

		#endregion Private event handlers

		internal bool SuppressIsMenuClickingChange
		{
			get
			{
				return m_bSuppressIsMenuClickingChange;
			}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			private
#endif
 set
			{
				m_bSuppressIsMenuClickingChange = value;
			}
		}


    }
	[Syncfusion.Documentation.DocumentationExclude()]
	public class SingleLineBarRenderer: BarRenderer
	{
		public SingleLineBarRenderer( IBarControl parent )
			: base( parent )
		{
		}

		protected SizeF lastKnownPreferredSize = SizeF.Empty;

		protected override void OnBoundsAffected()
		{
			lastKnownPreferredSize = SizeF.Empty;
			base.OnBoundsAffected();
		}

		/// <param name="bounds"> BarControl's bounds for specified renderer. </param>
		/// <returns> Changed client bounds according to specified renderer. </returns>
		protected virtual RectangleF CorrectDisplayBounds( RectangleF bounds )
		{
			return bounds;
		}

		public virtual void AfterComputeBarItemPositions( IGraphicsProvider gp )
		{
			// Do nothing here.
		}
		public override void ComputeBarItemPositions( IGraphicsProvider gp )
		{
			if( barItemRenderers.Count <= 0 )
				return;

			this.InvisibleBarItems.Clear();

			// Making sure preferred height is known, so that renderers can be centered if necessary
			if( this.lastKnownPreferredSize == Size.Empty )
			{
				this.GetPreferredSize( gp );
			}

			/*
			 * NOTE: 22 october 2004
			 * PROBLEM: Bad calculation of the Y coordinate for bar items.
			 * In some cases Y coordinate (centeredTop variable) becomes invalid.
			 * SOLUTION: using tdbounds instead of Bounds property which
			 * rotates bounds. Also added max value calculating.
			 */
			float maxHeight = Math.Max( this.lastKnownPreferredSize.Height,
			  tdbounds.Height ) - padY;

			// Compute the beginning left and bottom and height
			float left = tdbounds.X + padX;
			left = (float)Math.Round( (double)left );
			float top = tdbounds.Y + padY / 2;

			bool previousItemWasSeparator = false;
			bool firstItemDrawn = false;
			bool anyItemHiddenDueToLackOfWidth = false;

			// iterate the renderers and update their bounds
			foreach( IBarItemRenderer renderer in barItemRenderers )
			{
				bool drawItemVisible = this.ShouldDrawVisible( renderer.BarItem, true );
				bool bRTL = this.parent.IsRightToLeft;

				// Check if beginning of a group
				if( this.Bar.IsGroupBeginning( renderer.BarItem )
					&& !previousItemWasSeparator && firstItemDrawn )
				{
					// if the item is invisible, draw the separator only if the item is from the main manager
					if( drawItemVisible ||
						renderer.BarItem.Manager == null || renderer.BarItem.Manager is MainFrameBarManager )
					{
						previousItemWasSeparator = true;
						left += this.separatorAreaX;
					}
				}

				// Compute Preferred Size
				SizeF barItemSize = renderer.GetPreferredSize( gp );
				float height = barItemSize.Height;
				float centeredTop = top;

				if( height < maxHeight && renderer.NeedCenterVAlign )
				{
					centeredTop += ( maxHeight - height ) / 2;
					if( centeredTop + height > tdbounds.Height )
						// then give up
						centeredTop = top;
				}
				// Set the bounds even if not visible, but mark them as invisible
				float fBarItemWidth = barItemSize.Width;
				float fLeft = bRTL ? ( tdbounds.Width - left - fBarItemWidth + padX ) : left;
				renderer.Bounds = new RectangleF( fLeft, centeredTop, fBarItemWidth, height );

				bool addToInvisibleList = false;

				RectangleF boudns = this.tdbounds;

				boudns = CorrectDisplayBounds( boudns );

				if( ( bRTL && ( fLeft < boudns.Left ) )
					|| ( fLeft + fBarItemWidth > boudns.Right ) )
				{
					if( drawItemVisible )
						addToInvisibleList = true;

					drawItemVisible = false;
				}

				if( anyItemHiddenDueToLackOfWidth
					&& this.Bar.AllowItemsReorderOnShrunk == false )
				{
					if( drawItemVisible )
						addToInvisibleList = true;

					drawItemVisible = false;
				}

				if( addToInvisibleList || ( !drawItemVisible && this.ShouldDrawVisible( renderer.BarItem, false ) ) )
                {
                    this.InvisibleBarItems.Add(renderer.BarItem);
                    anyItemHiddenDueToLackOfWidth = true;
                }

				if( drawItemVisible )
				{
					firstItemDrawn = true;
					previousItemWasSeparator = false;
					renderer.Visible = true;
					left += fBarItemWidth;
				}
				else
					renderer.Visible = false;
			}

			AfterComputeBarItemPositions( gp );
		}

		protected SizeF GetPreferredSize( IGraphicsProvider gp )
		{
			SizeF returnValue = SizeF.Empty;
			GetPreferredSize( gp, ref returnValue );
			return returnValue;
		}

		public override void GetPreferredSize( IGraphicsProvider gp, ref SizeF preferredSize )
		{
			bool shouldDrawImage = false;
			Size imageSize = Size.Empty;

			if( preferredSize == SizeF.Empty )
			{
				// Return Minimum Size
				preferredSize = new SizeF( padX, padY );
				if( barItemRenderers.Count > 0 )
					preferredSize += ( (IBarItemRenderer)barItemRenderers[0] ).GetPreferredSize( gp );
			}
			else
			{
				int separatorCount = 0;
				bool firstItemDrawn = false;
				preferredSize = new SizeF( padX, padY );
				if( ( this.Bar.BarStyle & BarStyle.MultiLine ) == 0
					&& barItemRenderers.Count > 0 )
				{
					// Single line logic...
					// BarItem's preferred size
					foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
					{
						BarItem item = barItemRenderer.BarItem;

						bool groupBeginner = false;
						if( firstItemDrawn && this.Bar.IsGroupBeginning( item ) )
						{
							separatorCount++;
							groupBeginner = true;
						}

						if( !shouldDrawImage )
							shouldDrawImage = ( barItemRenderer as BarItemRenderer ).ShouldDrawImage();

						if( this.ShouldDrawVisible( item, true ) )
						{
							firstItemDrawn = true;

							SizeF preferredBarItemSize = barItemRenderer.GetPreferredSize( gp );
							preferredSize.Width += preferredBarItemSize.Width;

							if( preferredSize.Height < ( preferredBarItemSize.Height + padY ) )
								preferredSize.Height = preferredBarItemSize.Height + padY;
						}
						else
						{
							Size szImage = item.GetImageSizeInternal( this.parent.LargeIcons );

							if( imageSize.Width < szImage.Width )
							{
								imageSize.Width = szImage.Width;
							}
							if( imageSize.Height < szImage.Height )
							{
								imageSize.Height = szImage.Height;
							}
						}

						if( !this.ShouldDrawVisible( barItemRenderer.BarItem, true ) && groupBeginner )
						{
							// if the item is invisible, draw the separator only if the item is from the main manager
							if( !( barItemRenderer.BarItem.Manager == null || barItemRenderer.BarItem.Manager is MainFrameBarManager ) )
								separatorCount--;
						}
					}
					// ... space for separators 
					preferredSize.Width += separatorCount * this.separatorAreaX;
				}

				if( preferredSize.Width < 10 )
					preferredSize.Width = 10;
			}

			if( preferredSize.Width < 10 )
				preferredSize.Width = 10;

			if( shouldDrawImage && this.parent != null &&
				( preferredSize.Height < ( imageSize.Height + padY + BarItemRenderer.PadY * 2 ) ) )
			{
				preferredSize.Height = imageSize.Height + padY + BarItemRenderer.PadY * 2;
			}

			if( preferredSize.Height <= 22 )
				preferredSize.Height = 30;

			lastKnownPreferredSize = preferredSize;
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DropDownBarItemRenderer: BarItemRenderer, IDropDownItem//, IPopupParent, IPopupChild
	{
		protected IPopupChild popupChild;
		protected IPopupParent popupParent;

		public DropDownBarItemRenderer( IBarRenderer parent )
			: base( parent )
		{
			this.popupParent = parent;
		}

		public override bool ShowingDropDown
		{
			get
			{
				return base.ShowingDropDown;
			}
			set
			{
				base.ShowingDropDown = value;

				if( !IsDisposed )
				{
					MainFrameBarManager barMan = GetMainManager();

					if( null != barMan )
					{
						barMan.ShouldHidePopup = value;
					}
				}
			}
		}

		#region PROPERTIES
		public override bool Active
		{
			get { return base.Active; }
			set
			{
				if( base._active != value )
				{
					base.Active = value;
					if( base._active == false )
						this.ShowingDropDown = false;
				}
			}
		}
		protected bool ShouldDrawDropDown()
		{
			bool dropDownStyle = IsItemDropDownStyle( this.BarItem );

			if( this.BarItem is ParentBarItem && !dropDownStyle
				&& this.IsParentMainMenu )
				return false;
			else
				return true;
		}
		public override bool ShouldDrawText()
		{
			bool dropDownStyle = IsItemDropDownStyle( this.BarItem );

			if( this.BarItem is ParentBarItem && !dropDownStyle
				&& this.IsParentMainMenu )
				return true;

			return base.ShouldDrawText();
		}
		protected internal override bool ShouldDrawImage()
		{
			bool dropDownStyle = IsItemDropDownStyle( this.BarItem );

			if( this.BarItem is ParentBarItem && !dropDownStyle
				&& this.IsParentMainMenu
				&& this.BarItem.Text.Length != 0 )
				return false;

			return base.ShouldDrawImage();
		}
		#endregion PROPERTIES
		#region PARENT_INTERACTION
		public override SizeF GetPreferredSize( IGraphicsProvider gp )
		{
			SizeF preferredSize = base.GetPreferredSize( gp );

			if( this.ShouldDrawDropDown() )
			{
				if( !( this.parent.Alignment == CommandBarDockState.Left || this.parent.Alignment == CommandBarDockState.Right ) )
					preferredSize.Width += GetDropDownAreaWidth();
				else
					preferredSize.Height += GetDropDownAreaWidth();
			}
			// For borders
			return preferredSize;
		}

		private Point OffsetPointByDropDownArea( Point point )
		{
			// If showing drop-down arrow
			if( this.ShouldDrawDropDown() )
			{
				bool bRTL = this.IsRTL;
				// Adjust the hit point so that the Contains check below will be true only if hit 
				// outside the drop-down area
				if( !( this.parent.Alignment == CommandBarDockState.Left || this.parent.Alignment == CommandBarDockState.Right ) )
				{
					int nOffset = GetDropDownAreaWidth() + PadX;
					point.X += bRTL ? -nOffset : nOffset;
				}
				else
				{
					int nOffset = GetDropDownAreaWidth() + PadY;
					point.Y += bRTL ? nOffset : -nOffset;
				}
			}
			return point;
		}

		static internal bool IsItemDropDownStyle( BarItem barItem )
		{
			if( barItem is IParentBarItem
				&& ( (IParentBarItem)barItem ).ParentStyle == ParentBarItemStyle.DropDown )
				return true;
			else
				return false;
		}

		public override void OnMouseDown( Point pointMouseDown )
		{
			// Called because mouse was pressed within my bounds
			if( !this.BarItem.Enabled && !this.parent.Customizing )
				return;

			if( !this.ShowingDropDown )
			{
				this.Active = true;

				if( !this.parent.Customizing && IsItemDropDownStyle( this.BarItem ) )
				{
					pointMouseDown = this.OffsetPointByDropDownArea( pointMouseDown );
					if( !this.Bounds.Contains( pointMouseDown ) )
					{
						this.ShowingDropDown = true;
					}
				}
				else
				{
					this.ShowingDropDown = true;
					MenuGrid grid = popupChild as MenuGrid;
					if( grid != null )
					{
						grid.IsClicking = true;
					}
				}
			}
			else if( this.parent.Customizing )
			{
				this.ShowingDropDown = false;
				this.Active = false;
			}
		}
		public override void OnMouseUp( Point pointMouseUp )
		{
			if( !this.BarItem.Enabled && !this.parent.Customizing )
				return;

			if( this.ShowingDropDown )
				this.ShowingDropDown = false;
			else
			{
				bool performClick = false;
				if( IsItemDropDownStyle( this.BarItem ) )
				{
					pointMouseUp = this.OffsetPointByDropDownArea( pointMouseUp );

					// Check if mouse was up from within my bounds
					if( this.Bounds.Contains( pointMouseUp ) )
						performClick = true;
				}

				if( !this.ShowingDropDown )
					this.Active = false;

				if( performClick )
					this.parent.DelayedPerformClickOnBarItem = this.BarItem;
			}
		}
		public override void OnMouseMove( Point pointMouseMove )
		{
			if( !this.BarItem.Enabled || this.ShowingDropDown )
				return;

			if( IsItemDropDownStyle( this.BarItem ) )
				pointMouseMove = this.OffsetPointByDropDownArea( pointMouseMove );
			this.HotTrack = this.Bounds.Contains( pointMouseMove );
		}

		#endregion PARENT_INTERACTION
		#region DRAWING
		protected override void DrawTextAndImage( Graphics g, RectangleF rectTextAndImage, Font textFont, Color textColor, Color bgColor, DrawItemState state )
		{
			// Draw the DropDown arrow if necessary
			bool bDrawDropDown = this.ShouldDrawDropDown();

			if( bDrawDropDown && !( rectTextAndImage.Width <= 0 || rectTextAndImage.Height <= 0 ) )
			{
				bool bRTL = this.IsRTL;
				CommandBarDockState cbdsParentDocking = this.parent.Alignment;

				if( bRTL )
				{
					if( CommandBarDockState.Left != cbdsParentDocking && CommandBarDockState.Right != cbdsParentDocking )
					{
						rectTextAndImage.X += this.GetDropDownAreaWidth() + 2;
					}
					else
					{
						rectTextAndImage.Y -= this.GetDropDownAreaWidth() + 2;
					}
				}

				base.DrawTextAndImage( g, rectTextAndImage, textFont, textColor, bgColor, state );

				if( !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled && !IsParentMainMenu
					&& IsItemDropDownStyle( this.BarItem )
					&& this.Style != VisualStyle.Office2007
                    && this.Style != VisualStyle.Office2010
					&& this.Style != VisualStyle.Office2007Outlook ) )
				{
					GraphicsState oldState = g.Save();
					g.ResetTransform();
					g.SmoothingMode = SmoothingMode.HighQuality;

					RectangleF curBounds = ApplyTransform( g, this.parent.Alignment, this.Bounds, false );
					float anchorY = 0;
					float offset = 0;

					if (this.BarItem.ResizeGlyphToFit)
						offset = (ComboBoxItemRenderer.GetDropDownArrowHeight(this.BarItem.ResizeGlyphToFit,Rectangle.Ceiling(Bounds)) / 2) + 1;
					else
						offset = (ComboBoxItemRenderer.GetDropDownArrowHeight() / 2) + 1;

					 anchorY = curBounds.Top + curBounds.Height / 2 + offset;

					anchorY = (float)Math.Ceiling( anchorY );

					RectangleF ddBounds = curBounds;
					ddBounds.X = bRTL ? ( curBounds.Left ) : ( curBounds.Right - this.GetDropDownAreaWidth() - 2 );
					ddBounds.Width = this.GetDropDownAreaWidth();
                    if (this.BarItem != null && this.BarItem is ParentBarItem && (this.BarItem as ParentBarItem).Orientation == Orientation.Vertical && this.BarItem.ResizeGlyphToFit)
                    {
                        ddBounds.X -= 5;
                        ddBounds.Width = this.GetDropDownAreaWidth() + (this.szText.Width / 3);
                    }
					Point[] dropDownArrowBounds = ComboBoxItemRenderer.GetDropDownBorderBounds(this.BarItem.ResizeGlyphToFit ,Rectangle.Ceiling( ddBounds ) );

					GraphicsPath path = new GraphicsPath();
					path.AddLines( dropDownArrowBounds );

					using( Brush b = new SolidBrush( textColor ) )
					using( Region r = new Region( path ) )
					{
						g.FillRegion( b, r );
					}

					g.Restore( oldState );
				}
			}
			else
			{
				base.DrawTextAndImage( g, rectTextAndImage, textFont, textColor, bgColor, state );
			}
		}
		protected override RectangleF GetDrawingBounds()
		{
			RectangleF bounds = this.Bounds;
			if( ( this.HotTrack || this.Active ) && this.IsTextOnly() )
			{
				bounds.Y++;
				bounds.Height -= 3;
			}
			return bounds;
		}
		protected override RectangleF GetTextAndImageRect( Graphics g, Rectangle clippingRect )
		{
			RectangleF rectTextAndImage = base.GetTextAndImageRect( g, clippingRect );

			if( this.ShouldDrawDropDown() )
				rectTextAndImage.Width -= this.GetDropDownAreaWidth();

			return rectTextAndImage;
		}

		protected override Color GetHighlightItemLightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.MenuItemHotColorLight;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.BarItemHighlightLightColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.BarItemHighlightLightColor;
					break;
				}
                case VisualStyle.Metro:
                {
                    if (this.BarItem is ParentBarItem)
                        color = (this.BarItem as ParentBarItem).MetroColor;
                    else if(this.parent.GetBarControl() != null && this.parent.GetBarControl().GetControl() != null)
                        color = (this.parent.GetBarControl().GetControl() as BarControlInternal).MetroColor;
                    else
                        color = MenuColors.SelColor;
                    break;
                }
				default:
				{
					color = MenuColors.SelColor;
					break;
				}
			}

			return color;
		}

		protected override Color GetPressedItemLightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.MenuMarginColorLight;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.DDBarItemLightColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.DDBarItemLightColor;
					break;
				}
				default:
				{
					color = color = this.parent.GetBarControl().GetControl().BackColor;
					break;
				}
			}

			return color;
		}

		protected override Color GetPressedItemDarktColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.MenuMarginColorDark;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.DDBarItemDarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.DDBarItemDarkColor;
					break;
				}
				default:
				{
					color = this.parent.GetBarControl().GetControl().BackColor;
					break;
				}
			}

			return color;
		}

		protected override Color GetBGColor()
		{
			Color bgColor = this.parent.GetBarControl().GetControl().BackColor;

			if( this.ShowHighlightRectangle )
			{
				if( this.HotTrack && this.Active )
				{
					if( IsItemDropDownStyle( this.BarItem ) && !this.ShowingDropDown )
					{
						bgColor = GetHighlightItemLightColor( this.Style );
					}
					else
					{
						bgColor = GetPressedItemLightColor( this.Style );
					}
				}
				else if( ( this.HotTrack || this.Active ) && !this.ShowingDropDown )
				{
					bgColor = ( this.BarItem.Checked ) ?
						GetCheckedItemLightColor( this.Style ) : GetHighlightItemLightColor( this.Style );
				}
				else
				{
					if( this.Style != VisualStyle.Office2003
						&& this.Style != VisualStyle.VS2005
						&& this.Style != VisualStyle.Office2007
                        && this.Style != VisualStyle.Office2010
						&& this.Style != VisualStyle.Office2007Outlook )
					{
						if( this.BarItem.Checked )
							bgColor = Color.FromArgb( 80, bgColor );
					}
					else
					{
						if( this.BarItem.Checked )
							bgColor = GetCheckedItemLightColor( this.Style );
					}
				}
			}

			return bgColor;
		}

		protected override Color GetBGColor2()
		{
			Color bgColor = base.GetBGColor2();

			if( this.ShowHighlightRectangle && this.HotTrack && this.Active
				&& IsItemDropDownStyle( this.BarItem ) && !this.ShowingDropDown )
			{
				bgColor = GetHighlightItemDarkColor( this.Style );
			}

			return bgColor;
		}


		protected override Color GetForeColor( DrawItemState state )
		{
			return base.GetForeColor( state );
		}

		protected override Color GetForeColorDefault( DrawItemState state )
		{
			Color foreColor;

			if( this.BarItem != null && this.BarItem.CustomNormalTextColor != Color.Empty )
			{
				foreColor = this.BarItem.CustomNormalTextColor;
			}
			else
			{
				foreColor = this.parent.GetBarControl().UseControlForeColor ?
					this.parent.GetBarControl().GetControl().ForeColor : MenuColors.MenuTextColor;
			}

			if( ( state & DrawItemState.Disabled ) > 0 )
			{
				if( this.BarItem != null && this.BarItem.CustomDisabledTextColor != Color.Empty )
				{
					foreColor = this.BarItem.CustomDisabledTextColor;
				}
				else
				{
					foreColor = MenuColors.DisabledToolbarItemTextColorBase;
				}
			}
			else if( ( this.HotTrack || this.Active ) && !this.ShowingDropDown )
			{
				if( this.Active )
				{
					if( this.BarItem != null && this.BarItem.CustomActiveTextColor != Color.Empty )
					{
						foreColor = this.BarItem.CustomActiveTextColor;
					}
					else
					{
						foreColor = MenuColors.MenuActiveTextColor;
					}
				}
				else
				{
					if( this.BarItem.Enabled )
						foreColor = MenuColors.SelTextColor;
				}

			}

			return foreColor;
		}


		protected override bool IsMouseDown()
		{
			if( this.HotTrack && this.Active
					&& !this.ShowingDropDown )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets brush of the pressed DropDown button amenably with VisualStyle.
		/// </summary>
		private Brush GetsPressedDropDownBrush( VisualStyle style, Rectangle bounds )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new LinearGradientBrush( bounds, Office2003Colors.PressedSelColor,
						Office2003Colors.MenuItemHotColorDark, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( bounds, Office2007OutlookColors.BarItemPressLightColor,
						Office2007OutlookColors.BarItemPressDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( bounds, VS2005Colors.BarItemPressLightColor,
						VS2005Colors.BarItemPressDarkColor, LinearGradientMode.Vertical );
					break;
				}
                case VisualStyle.Metro:
                {
                    if(this.parent.GetBarControl()!=null && this.parent.GetBarControl().GetControl()!=null)
                        brush = new SolidBrush((this.parent.GetBarControl().GetControl() as BarControlInternal).MetroColor);
                    else
                         brush = new SolidBrush(Color.Blue);
                    break;
                }
				default:
				{
					brush = new SolidBrush( MenuColors.PressedSelColor );
					break;
				}
			}

			return brush;
		}

		protected override ItemState GetDrawState()
		{
			ItemState state = base.GetDrawState();

			if( this.Active )
			{
				state = ItemState.Collapsed;
			}

			return state;
		}

		protected override void DrawBackground( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			if( this.Style == VisualStyle.Office2007 )
			{
				if( this.ShowHighlightRectangle )
				{
					ItemState state = GetDrawState();
					bool bHorizontal = this.IsHorizontalAligned;

					if( !this.IsHorizontalAligned
						&& ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
					{
						bHorizontal = true;
					}

					Office2007BarItemPainter.DrawDropDownBarItem( drawItemInfo.Graphics, drawItemInfo.Bounds, state, bHorizontal );
				}

				return;
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                if (this.ShowHighlightRectangle)
                {
                    ItemState state = GetDrawState();
                    bool bHorizontal = this.IsHorizontalAligned;

                    if (!this.IsHorizontalAligned
                        && (this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical) > 0)
                    {
                        bHorizontal = true;
                    }

                    Office2010BarItemPainter.DrawDropDownBarItem(drawItemInfo.Graphics, drawItemInfo.Bounds, state, bHorizontal);
                }

                return;
            }

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
				&& this.Style != VisualStyle.Office2007Outlook )
			{
				if( !IsItemDropDownStyle( this.BarItem ) )
					base.DrawBackground( drawItemInfo, useThemes );
				else
				{
					bool bRTL = this.IsRTL;

					ThemedToolBarDrawing themedDrawing = this.parent.GetBarControl().ThemedDrawing;
					themedDrawing.DrawMirrored = bRTL;

					// Drawing the split button
					Rectangle bgBounds = drawItemInfo.Bounds;
					int nDDWidth = this.GetDropDownAreaWidth() + BarItemRenderer.PadX;
					Rectangle bgSplitBounds = bgBounds;
					bgSplitBounds.Width = bgBounds.Width - nDDWidth;

					if( bRTL )
					{
						bgSplitBounds.X += nDDWidth;
					}

					int state = this.GetCurrentThemeState( drawItemInfo );
					if( this.ShowingDropDown )
						state = ThemeStates.TS_HOT;

					Graphics graphix = drawItemInfo.Graphics;

					themedDrawing.DrawButton( graphix, ThemeParts.TP_SPLITBUTTON, state,
						bgSplitBounds );

					// Drawing the split button drop-down portion
					Rectangle bgSplitDDBounds = new Rectangle( bRTL ? bgBounds.Left : bgSplitBounds.Right,
						bgBounds.Y, nDDWidth, bgBounds.Height );

					if( this.ShowingDropDown )
						state = ThemeStates.TS_PRESSED;

					themedDrawing.DrawButton( graphix, ThemeParts.TP_SPLITBUTTONDROPDOWN, state,
						bgSplitDDBounds );
				}
			}
            else if(this.Style == VisualStyle.Metro)
			{
				base.DrawBackground( drawItemInfo, useThemes );

				// If mouse down in the clickable portion, then just redraw that portion.
				if( this.HotTrack && this.Active)
				{
					Rectangle bgBounds = drawItemInfo.Bounds;
					Brush bgBrush = GetsPressedDropDownBrush( this.Style, bgBounds );

					if( IsItemDropDownStyle( this.BarItem ) )
					{
						int nDDWidth = this.GetDropDownAreaWidth() + BarItemRenderer.PadX + 1;

						if( this.IsRTL )
						{
							bgBounds.X += nDDWidth;
						}

						bgBounds.Width -= nDDWidth;
					}

					drawItemInfo.Graphics.FillRectangle( bgBrush, bgBounds );
				}
			}
			else
			{
				base.DrawBackground( drawItemInfo, useThemes );

				// If mouse down in the clickable portion, then just redraw that portion.
				if( this.HotTrack && this.Active
					&& !this.ShowingDropDown )
				{
					Rectangle bgBounds = drawItemInfo.Bounds;
					Brush bgBrush = GetsPressedDropDownBrush( this.Style, bgBounds );

					if( IsItemDropDownStyle( this.BarItem ) )
					{
						int nDDWidth = this.GetDropDownAreaWidth() + BarItemRenderer.PadX + 1;

						if( this.IsRTL )
						{
							bgBounds.X += nDDWidth;
						}

						bgBounds.Width -= nDDWidth;
					}

					drawItemInfo.Graphics.FillRectangle( bgBrush, bgBounds );
				}
			}
		}

		protected override void DrawBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
				&& this.Style != VisualStyle.Office2007Outlook )
				return;

			Graphics g = drawItemInfo.Graphics;

            if (this.Style ==VisualStyle.Metro )
            {
                if (this.ShowingDropDown)
                {
                    Pen borderPen = GetSelectedBorderPen(this.Style);
                    Rectangle curBounds = drawItemInfo.Bounds;

                    curBounds.Width -= 1;
                    //curBounds.Height -= 1;
                    g.FillRectangle(new SolidBrush(drawItemInfo.BackColor), curBounds);
                }
                return;
            }
			if( ( ( this.HotTrack || this.Active ) && !this.ShowingDropDown )
				|| this.BarItem.Checked )
			{
				base.DrawBorders( drawItemInfo, useThemes );
				// Dividing line
				if( IsItemDropDownStyle( this.BarItem ) && this.ShowHighlightRectangle )
				{
					Rectangle curBounds = drawItemInfo.Bounds;
					Pen borderPen = GetHighlightedBorderPen( this.Style );

					float left = this.IsRTL ? ( curBounds.Left + 1 + this.GetDropDownAreaWidth() + PadX ) : ( curBounds.Right - 1 - this.GetDropDownAreaWidth() - PadX );
					g.DrawLine( borderPen, left, curBounds.Top, left, curBounds.Bottom - 1 );
				}
			}

            if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
            {
                return;
            }

			if( this.ShowingDropDown )
			{
				Pen borderPen = GetSelectedBorderPen( this.Style );
				Rectangle curBounds = drawItemInfo.Bounds;

				curBounds.Width -= 1;
				//curBounds.Height -= 1;
				g.DrawRectangle( borderPen, curBounds );
			}
		}

		/// <summary>
		/// Gets pen for highlighted border of the DropDownBarItem amenably with VisualStyle.
		/// </summary>
		private Pen GetHighlightedBorderPen( VisualStyle style )
		{
			Pen pen = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					pen = new Pen( Office2003Colors.SelBorderColor );
					break;
				}
                case VisualStyle.Office2010:
                {
                    pen = new Pen(Office2010BarItemPainter.ColorTable.BarItemHighlightBorderColor);
                    break;
                }
                case VisualStyle.Office2007:
                {
                    pen = new Pen(Office2007BarItemPainter.ColorTable.BarItemHighlightBorderColor);
                    break;
                }
				case VisualStyle.Office2007Outlook:
				{
					pen = new Pen( Office2007OutlookColors.BarItemHighlightBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.BarItemHighlightBorderColor );
					break;
				}
				default:
				{
					pen = new Pen( MenuColors.SelBorderColor );
					break;
				}
			}

			return pen;
		}


		/// <summary>
		/// Gets pen for selected border of the DropDownBarItem.
		/// </summary>
		private Pen GetSelectedBorderPen( VisualStyle style )
		{
			Pen pen = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					pen = new Pen( Office2003Colors.DropdownBorderColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					pen = new Pen( Office2007OutlookColors.DDBarItemBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.DDBarItemBorderColor );
					break;
				}
				default:
				{
					pen = new Pen( MenuColors.DropDownBorderColor );
					break;
				}
			}

			return pen;
		}
		#endregion DRAWING
		#region DROPDOWN
		public bool IsShowing()
		{
			if( this.popupChild != null
				&& this.popupChild.IsShowing() )
				return true;
			else
				return false;
		}
		protected override bool ShowDropDown( Queue pbiQueue )
		{
			if( this.IsShowing() )
				return true;

			Graphics g = this.parent.GetBarControl().GetGraphics();
			RectangleF transformedBounds = ApplyTransform( g, this.parent.Alignment, this.Bounds, false );
			g.Dispose();

			if( this.BarItem is ParentBarItem )
			{
				ParentBarItem item = this.BarItem as ParentBarItem;

				// TODO: Instead provide a method in ParentBarItem that can be queried before dropdown
				if( !( item is ToolbarListBarItem ) || !this.parent.Customizing )
					popupChild = this.ShowChildrenUI( item, this.parent, pbiQueue );
			}
			else if( this.BarItem is DropDownBarItem )
			{
				if( !this.parent.Customizing )
				{
					DropDownBarItem item = this.BarItem as DropDownBarItem;
					if( item.PopupControlContainer != null )
					{
						PopupControlContainer popupContainer = item.PopupControlContainer;

						popupChild = popupContainer;

						//popupContainer.IgnoreDeactivate = true;
						popupContainer.PopupParent = this.parent;
						popupContainer.RightToLeft = this.parent.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;

						if( null != popupContainer.PopupHost )
						{
							popupContainer.PopupHost.IgnoreWorkingArea = item.IgnoreWorkingArea;
						}

						popupContainer.ShowPopup( Point.Empty );
					}
				}
			}
			return this.IsShowing();
		}
		public virtual IPopupChild ShowChildrenUI( ParentBarItem item, /*Point pos,*/ IPopupParent parentUI, Queue pbiQueue )
		{
			// The menuGrid will release itself based on settings
			MenuGrid menuGrid = XPMenuGridFactory.GetMenuGridToDeploy();

			menuGrid.IgnoreWorkingArea = item.IgnoreWorkingArea;

			//Show the menu
			menuGrid.Show( item, Point.Empty, parentUI, this.showDefaultSelectionInDropdown, pbiQueue );

			return menuGrid;
		}

		public virtual bool KeyboardMessage( ref Message m )
		{
			if( this.popupParent is INeedKeyboardMessages )
			{
				INeedKeyboardMessages parent = (INeedKeyboardMessages)this.popupParent;
				return parent.KeyboardMessage( ref m );
			}
			return false;
		}

		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			this.popupChild = null;
			this.ShowingDropDown = false;
		}

		public virtual void AfterChildClosing()
		{
		}
		protected override void HideDropDown()
		{
			if( this.popupChild != null )
				this.popupChild.HidePopup( PopupCloseType.Canceled );
		}
		public virtual Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlign )
		{
			Graphics g = this.parent.GetBarControl().GetGraphics();
			RectangleF transformedBounds = ApplyTransform( g, this.parent.Alignment, this.GetDrawingBounds(), false );
			g.Dispose();

			Control parentControl = this.parent.GetBarControl().GetControl();

			Rectangle bounds = parentControl.RectangleToScreen( Rectangle.Ceiling( transformedBounds ) );

			return PopupUtils.ComputeDefaultBorderOverlapCue( rAlign, bounds );
		}

		private RectangleF GetBounds()
		{
			Graphics g = parent.GetBarControl().GetGraphics();
			RectangleF transformedBounds = ApplyTransform( g, this.parent.Alignment, this.GetDrawingBounds(), false );
			g.Dispose();

			return transformedBounds;
		}

		public Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign, bool bIsRightToLeft )
		{
			RectangleF transformedBounds = GetBounds();

			Control parentControl = this.parent.GetBarControl().GetControl();

			Point pos = PopupUtils.ComputeDefaultPopupAlignment( prevAlign, out newAlign,
				this.GetFirstAlignPreference(), this.GetLastAlignPreference(), Rectangle.Ceiling( transformedBounds ) );

			pos.X = ( bIsRightToLeft ) ? (int)transformedBounds.Right : pos.X;

			pos = parentControl.PointToScreen( pos );

			return pos;
		}

		public Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign )
		{
			RectangleF transformedBounds = GetBounds();

			Control parentControl = this.parent.GetBarControl().GetControl();

			Point pos = PopupUtils.ComputeDefaultPopupAlignment( prevAlign, out newAlign,
				this.GetFirstAlignPreference(), this.GetLastAlignPreference(), Rectangle.Ceiling( transformedBounds ) );

			pos = parentControl.PointToScreen( pos );

			return pos;
		}
		protected virtual PopupRelativeAlignment GetLastAlignPreference()
		{
			if( this.parent.Alignment == CommandBarDockState.Left
				|| this.parent.Alignment == CommandBarDockState.Left )
				return PopupRelativeAlignment.RightTop;
			else
				return PopupRelativeAlignment.BottomLeft;
		}
		protected PopupRelativeAlignment GetFirstAlignPreference()
		{
			return this.parent.GetFirstPopupAlignPreference();
		}
		public Control GetPopupParentControl()
		{
			return null;
		}

		public virtual bool IsRelatedControl( Control ctl )
		{
			bool bRelated = false;
			DropDownBarItem ddbi = this.BarItem as DropDownBarItem;

			if( null != ddbi )
			{
				PopupControlContainer ppc = ddbi.PopupControlContainer;

				if( null != ppc )
				{
					bRelated = ppc.Contains( ctl );
				}
			}

			return bRelated;
		}

		#endregion DROPDOWN
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class StaticBarItemRenderer: BarItemRenderer
	{
		public StaticBarItemRenderer( IBarRenderer parent )
			: base( parent )
		{
		}
		public StaticBarItem StaticBarItem
		{
			get { return this.BarItem as StaticBarItem; }
		}
		public override bool HotTrack
		{
			get { return false; }
			set { }
		}

		public override bool Active
		{
			get
			{
				return false;
			}
			set { }
		}
		public override void OnMouseMove( Point pointMouseMove )
		{
		}
		protected override Color GetBGColor()
		{
			return this.parent.GetBarControl().GetControl().BackColor;
		}
		protected override void DrawBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			Graphics g = drawItemInfo.Graphics;

			Pen borderPen = new Pen( this.StaticBarItem.FlatBorderColor );
			Rectangle curBounds = drawItemInfo.Bounds;
			curBounds.Width -= 1;
			curBounds.Height -= 1;
			// Deflate a bit for static borders
			curBounds.Inflate( -1, 0 );
			g.DrawRectangle( borderPen, curBounds );
            borderPen.Dispose();
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarItemRenderer: IBarItemRenderer
	{
		#region PRIVATE_MEMBERS
		private BarItem barItem;
		private bool hotTrack = false;
		internal bool _active = false;
		private bool visible = true;
		protected IBarRenderer parent;
		private RectangleF bounds;	// based on horizontal alignment
		protected RectangleF lastDrawnBounds = RectangleF.Empty;
		private bool showingDropDown = false;
		protected bool showDefaultSelectionInDropdown = false;
		private bool themesEnabled = false;
		private VisualStyle style = VisualStyle.OfficeXP;
		private bool textSizeKnown = false;
		internal SizeF szText = SizeF.Empty;
		private Queue m_queueParentBarItems = null;

		public static int ImageTextPadding = 2;
		public static int PadX = 2;
		public static int PadY = 3;
		public static int DropDownAreaX = 9;

		private const int m_cDefaultPaddingForThemesX = 4;
		private const int m_cDefaultPaddingForThemesY = 6;

		internal static readonly Point DEF_PADDING = Point.Empty;

		internal int PaddingForThemesX
		{
			get
			{
				if( BarItem != null )
				{
					return BarItem.PaddingForThemesX;
				}
				else
				{
					return m_cDefaultPaddingForThemesX;
				}
			}
		}

		internal int PaddingForThemesY
		{
			get
			{
				if( BarItem != null )
				{
					return BarItem.PaddingForThemesY;
				}
				else
				{
					return m_cDefaultPaddingForThemesY;
				}
			}
		}

		internal Point Padding
		{
			get
			{
				if( BarItem != null )
				{
					return BarItem.Padding;
				}
				else
				{
					return DEF_PADDING;
				}
			}
		}


		protected bool IsParentMainMenu
		{
			get { return ( this.parent.Bar.BarStyle & BarStyle.IsMainMenu ) > 0; }
		}
		public virtual bool ShouldDrawText()
		{
			if( ( this.barItem != null &&
				this.barItem.PaintStyle != PaintStyle.Default &&
				this.barItem.PaintStyle != PaintStyle.TextOnlyInMenus )
				|| ( this.barItem.ImageIndex == -1 && this.BarItem.Image == null ) )
				return true;
			else
				return false;
		}
		protected internal virtual bool ShouldDrawImage()
		{
			if( this.barItem != null && ( this.barItem.PaintStyle != PaintStyle.TextOnly ) )
				return true;
			else
				return false;
		}
		#endregion PRIVATE_MEMBERS

		#region PROPERTIES

		public VisualStyle Style
		{
			get { return this.style; }
			set
			{
				VisualStyle oldStyle = this.style;
				this.style = value;

				this.BarItemPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "Style", oldStyle, this.style ) );
			}
		}
		public bool ThemesEnabled
		{
			get { return this.themesEnabled; }
			set { this.themesEnabled = value; }
		}

		public virtual bool NeedCenterVAlign
		{
			// In large fonts for example, the image will be smaller than the text, so image only items need to be center aligned.
			get { return true; }
		}

		public virtual bool HotTrack
		{
			get
			{
				if( this.parent.Customizing )
					return false;
				return hotTrack;
			}
			set
			{
				if( hotTrack != value )
				{
					if( value == true && ( !this.barItem.Enabled && !this.parent.Customizing ) )
						value = false;
					if( hotTrack != value )
					{
						hotTrack = value;
						if( !hotTrack )
							this.ShowingDropDown = false;
						this.Repaint();
					}
				}
			}
		}
		public virtual bool Active
		{
			get
			{
				if(this.parent != null && this.parent.Customizing )
					return false;
				return this._active;
			}
			set
			{
				if( value && !this.barItem.Enabled && !this.parent.Customizing )
					return;
				if( this._active != value )
				{
					this._active = value;
					this.Repaint();

					OnActiveChanged();
				}
			}
		}

		protected MainFrameBarManager GetMainManager()
		{
			MainFrameBarManager barManager = this.barItem.Manager as MainFrameBarManager;
			if( barManager != null ) return barManager;

			ChildFrameBarManager childMan = barItem.Manager as ChildFrameBarManager;
			if( childMan != null )
			{
				barManager = childMan.MainFrameBarManager;
			}

			return barManager;
		}

		public virtual bool ShowingDropDown
		{
			get { return this.showingDropDown; }
			set
			{
				if( ( value && !this.barItem.Enabled && !this.parent.Customizing )
					|| !( this is IDropDownItem ) )
					return;
				if(this.showingDropDown != value )
				{
					this.showingDropDown = value;
					// Show or Close drop-down
					if ((_active||this.HotTrack) && this.showingDropDown)
					{
						this.showingDropDown = this.ShowDropDown( this.m_queueParentBarItems );
						if( m_bIsDisposed )
						{
							this.Active = false;
							this.HideDropDown();
						}
                        else
                        {
                            if (this.showingDropDown)
                                this.Active = true;
                        }
					}
                    else if ((this.Active))
					{
						// Make sure to call Active = false before hiding dropdown.
                        if (this is DropDownBarItemRenderer)
                            this.Active = false;
						this.HideDropDown();
					}
                    else
                    {
                        this.HideDropDown();
                    }

					if( !m_bIsDisposed )
					{
						this.Repaint();
					}
				}
                else if (this.Active && this.showingDropDown != value)
                {
                    this.showingDropDown = value;
                    // Show or Close drop-down
                    if ((this.Active || this.HotTrack) && this.showingDropDown)
                    {
                        this.showingDropDown = this.ShowDropDown(this.m_queueParentBarItems);
                        if (m_bIsDisposed)
                        {
                            this.Active = false;
                            this.HideDropDown();
                        }

                    }
                    else if ((this.Active))
                    {
                        // Make sure to call Active = false before hiding dropdown.
                        if (this is DropDownBarItemRenderer)
                            this.Active = false;
                        this.HideDropDown();
                    }

                    if (!m_bIsDisposed)
                    {
                        this.Repaint();
                    }
                }
			}
		}
		public virtual bool Visible
		{
			get { return this.visible; }
			set { this.visible = value; }
		}

		/// <internalonly/>
		internal protected bool IsRTL
		{
			get
			{
				return ( null != this.parent ) ? this.parent.IsRightToLeft : false;
			}
		}

		public void DropDown( bool show, bool setDefaultSelection, Queue pbiQueue )
		{
			if( show )
			{
				this.showDefaultSelectionInDropdown = setDefaultSelection;

				m_queueParentBarItems = pbiQueue;
				this.ShowingDropDown = true;
				m_queueParentBarItems = null;

				this.showDefaultSelectionInDropdown = false;
			}
			else
				this.ShowingDropDown = false;
		}

		public virtual BarItem BarItem
		{
			get
			{
				return barItem;
			}
			set
			{
				barItem = value;
			}
		}
		public virtual RectangleF Bounds
		{
			get { return bounds; }
			set
			{
				bounds = value;
			}
		}

		#endregion PROPERTIES

		#region Overrides

		internal virtual void OnActiveChanged()
		{
		}

		#endregion

		#region INTIALIZATION
		public BarItemRenderer( IBarRenderer parent )
		{
			this.parent = parent;
		}
		#endregion INTIALIZATION

		#region PARENT_INTERACTION
		~BarItemRenderer()
		{
			this.Dispose( false );
		}
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		private bool m_bIsDisposed = false;

		public bool IsDisposed
		{
			get
			{
				return m_bIsDisposed;
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.barItem = null;
				this.parent = null;
				m_bIsDisposed = true;
			}
		}
		protected void Repaint()
		{
			Graphics g = this.parent.GetBarControl().GetGraphics();
			RectangleF redrawBounds = ApplyTransform( g, this.parent.Alignment, this.bounds, false );
			g.Dispose();
			this.parent.GetBarControl().OnRepaint( redrawBounds );
		}
		public virtual void BarItemPropertyChanged( Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e )
		{
			// Will delete cached data.
			if( e.PropertyChangeEffect == PropertyChangeEffect.NeedLayout )
			{
				this.InvalidateCachedTextSizes();
			}
		}
		protected bool IsHorizontalAligned
		{
			get
			{
				if( this.parent.Alignment == CommandBarDockState.Top
					|| this.parent.Alignment == CommandBarDockState.Bottom
					|| this.parent.Alignment == CommandBarDockState.Float
					|| this.parent.Alignment == CommandBarDockState.None/*when in XPToolBar*/)
					return true;
				else
					return false;
			}
		}
		private bool DrawTextBelowImage
		{
			get
			{
				return ( this.parent.Bar.BarStyle & BarStyle.TextBelowImage ) > 0;
			}
		}
		public virtual void InvalidateCachedTextSizes()
		{
			this.textSizeKnown = false;
			this.szText = SizeF.Empty;
		}
		public virtual SizeF GetPreferredSize( IGraphicsProvider gp )
		{
			// Compute the size based on baritem properties
			// Compute assuming horizontal bar alignment.
			// Get Text size
			SizeF preferredSize = SizeF.Empty;

			this.AddTextPreferredSize( gp, ref preferredSize );

			Size imageSize = this.BarItem.GetImageSizeInternal( this.parent.LargeIcons );

			// Rotate ImageSize based on alignment (Images will be drawn straight whatever the alignment is.
			if( !this.IsHorizontalAligned )
			{
				imageSize = new Size( imageSize.Height, imageSize.Width );

				if( ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
					preferredSize = new SizeF( preferredSize.Height, preferredSize.Width );
			}

			// If images present
			bool shouldDrawImage = this.ShouldDrawImage();
			bool drawingImage = false;
			if( shouldDrawImage && ( imageSize.Width > 0 || imageSize.Height > 0 ) )
			{
				// If Image associated
				if( barItem.ImageIndex != -1 || this.BarItem.Image != null )
				{
					drawingImage = true;
					if( this.IsHorizontalAligned && this.DrawTextBelowImage )
					{
						preferredSize.Height += imageSize.Height;
						if( preferredSize.Width < ( BarItemRenderer.ImageTextPadding + imageSize.Width ) )
							preferredSize.Width += ( BarItemRenderer.ImageTextPadding + imageSize.Width );
					}
					else
					{
						preferredSize.Width += BarItemRenderer.ImageTextPadding;
						preferredSize.Width += imageSize.Width;
					}
				}
				// Do this even if there is no image associated.
				preferredSize.Height = preferredSize.Height < imageSize.Height ?
					imageSize.Height : preferredSize.Height;
			}

			preferredSize.Width += ( BarItemRenderer.PadX * 2 );
			preferredSize.Height += ( BarItemRenderer.PadY * 2 );

			preferredSize.Width += 1;// We want the preferred width to be an odd no: 23.

			if( !drawingImage && this.IsParentMainMenu )
				preferredSize.Width += 4;

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
                && this.Style != VisualStyle.Office2007
                && this.Style != VisualStyle.Office2010
                && this.Style != VisualStyle.Metro 
				&& this.Style != VisualStyle.Office2007Outlook )
			{
				preferredSize.Width += PaddingForThemesX;
				preferredSize.Height += PaddingForThemesY;
			}
			else
			{
				preferredSize.Width += Padding.X;
				preferredSize.Height += Padding.Y;
			}

			return preferredSize;
		}
        int MaximumSize = 0;
		public virtual void AddTextPreferredSize( IGraphicsProvider gp, ref SizeF preferredSize )
		{
            if (barItem.SizeToFit)
			{
                if (this.ShouldDrawText() && barItem.Text != "")
				{
                    if (!this.textSizeKnown)
                    {
                        Font barItemFont = this.parent.GetBarControl().GetControl().Font;

					if( this.barItem != null && this.barItem.CustomTextFont != null )
					{
						barItemFont = this.barItem.CustomTextFont;
					}

					ProvideFontInfoEventArgs args = new ProvideFontInfoEventArgs( barItemFont );
					barItem.OnProvideFontInfo( args );
					barItemFont = args.Font;

					Size size = MeasureText( gp.Graphics, barItem.Text, barItemFont );

                    if ((barItem is ParentBarItem) && (barItem as ParentBarItem).MultiLine && size.Width > 20)
                    {
                        string getText = WordWrap(barItem.Text, (barItem as ParentBarItem).WrapLength);
                        size = MeasureText(gp.Graphics, getText, barItemFont);
                        size.Height -= 10;
                    }

                    if ((barItem is ParentBarItem) && (barItem as ParentBarItem).Orientation == Orientation.Vertical)
                    {
                        foreach (ParentBarItem item in parent.Bar.Items)
                        {
                            if (MeasureText(gp.Graphics, item.Text, item.CustomTextFont).Width > MaximumSize)
                            {
                                MaximumSize = MeasureText(gp.Graphics, item.Text, item.CustomTextFont).Width;
                            }

                        }
                        string getText = barItem.Text;
                        int height = MeasureText(gp.Graphics, getText, barItemFont).Height;
                        size = new Size(height, MaximumSize);
                    }
                    
					size.Height++;
					size.Width++;

					this.textSizeKnown = true;
					this.szText = new SizeF( size.Width, size.Height );
				}

				preferredSize.Width += this.szText.Width;
				preferredSize.Height += this.szText.Height;
			}
            }
            else
            {
                preferredSize.Height = 15;
                preferredSize.Width = 30;
            }
		}

        internal static int BreakLine(string text, int pos, int max)
        {
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max;
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;
            return i + 1;
        }

        internal static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            if (width < 1)
                return text;

            for (pos = 0; pos < text.Length; pos = next)
            {
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

		/// <summary>
		/// Measures text.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="text">The text.</param>
		/// <param name="font">The text's font.</param>
		/// <param name="textFormat">The text's format.</param>
		/// <returns>Size of the measured text.</returns>
		protected Size MeasureText( Graphics g, string text, Font font, TextFormatFlags textFormat )
		{
			Size size;

			using( TextRendererDC dc = new TextRendererDC( g ) )
			{
				size = TextRenderer.MeasureText( dc, text, font, new Size( int.MaxValue, int.MaxValue ), textFormat );

				if( ( parent.Alignment & ( CommandBarDockState.Left | CommandBarDockState.Right ) ) != CommandBarDockState.None )
				{
					size.Width += 6;
				}
			}

			return size;
		}

		/// <summary>
		/// Measures text with default text format.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="text">The text.</param>
		/// <param name="font">The text's font.</param>
		/// <returns>Size of the measured text.</returns>
		protected Size MeasureText( Graphics g, string text, Font font )
		{
			return MeasureText( g, text, font, TextFormatFlags.Default );
		}

		public virtual void OnMouseDown( Point pointMouseDown )
		{
			// Called because mouse was pressed within my bounds
			if( !this.barItem.Enabled && !this.parent.Customizing )
				return;

			this.Active = true;
		}

		public virtual void OnMouseUp( Point pointMouseUp )
		{
			if( !this.barItem.Enabled && !this.parent.Customizing )
				return;

			bool performClick = false;
			if( this.Active && this.HitTest( pointMouseUp ) )
				performClick = true;

			this.Active = false;

			if( performClick )
				this.parent.DelayedPerformClickOnBarItem = this.barItem;
		}
		public virtual void OnMouseMove( Point pointMouseMove )
		{
			this.HotTrack = this.HitTest( pointMouseMove );
		}
		public virtual void OnMouseWheel( bool isUp )
		{
		}

		#endregion PARENT_INTERACTION

		#region DRAWING
		protected int GetDropDownAreaWidth()
		{
			if (this.BarItem.ResizeGlyphToFit)
				return (int) (5f * (this.szText.Width) / 13f);
			else
				return (int)(BarItemRenderer.DropDownAreaX * (SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the checkboxes.*/));
		}
		protected virtual void DrawFocusRect( Graphics g, RectangleF focusRect, Color fore, Color back )
		{
			ControlPaint.DrawFocusRectangle( g, Rectangle.Round( focusRect ), fore, back );
		}

		[Obsolete( "Not used." )]
		public virtual RectangleF GetTextPosition( Graphics g, string text, Font font, RectangleF rectLayout, StringFormat stringformat )
		{
			return RectangleF.Empty;
		}

		public virtual RectangleF GetTextPosition( Graphics g, string text, Font font, RectangleF rectLayoutF, TextFormatFlags format )
		{
			format &= ~TextFormatFlags.HidePrefix;

			Rectangle rectLayout = Rectangle.Round( rectLayoutF );
			Size size = Size.Empty;

			if( this.parent.Alignment != CommandBarDockState.Left && this.parent.Alignment != CommandBarDockState.Right )
			{
				size = MeasureText( g, text, font, format );
			}
			else
			{
				GraphicsState oldState = g.Save();
				g.ResetTransform();

				size = MeasureText( g, text, font, format );

				g.Restore( oldState );

				if( ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
				{
					size = new Size( size.Height, size.Width );
				}
			}

			size.Height++;
			size.Width++;

			Rectangle rectText = new Rectangle( rectLayout.X, rectLayout.Y, size.Width, size.Height );

			if( (format & TextFormatFlags.Right) != 0 )
			{
				rectText.Offset( rectLayout.Width - rectText.Width, 0 );
			}
			else if( (format & TextFormatFlags.HorizontalCenter) != 0 )
			{
				rectText.Offset( ( rectLayout.Width - rectText.Width ) / 2, 0 );
			}

			if( (format & TextFormatFlags.Bottom) != 0 )
			{
				rectText.Offset( 0, rectLayout.Height - rectText.Height );
			}
			else if( (format & TextFormatFlags.VerticalCenter) != 0 )
			{
				rectText.Offset( 0, ( rectLayout.Height - rectText.Height ) / 2 );
			}

			return rectText;
		}

		public virtual void DrawSeparator( Graphics g )
		{
			DrawSeparator( g, this.bounds );
		}

		protected virtual void DrawSeparator( Graphics g, RectangleF fBounds )
		{
			this.ApplyTransform( g );

			switch( this.Style )
			{
				case VisualStyle.Office2003:
				{
					DrawSeparatorOffice2003( g, fBounds );
					break;
				}
				case VisualStyle.VS2005:
				{
					DrawSeparatorVS2005( g, fBounds );
					break;
				}				
                case VisualStyle.Office2010:
                {
                    DrawSeparatorOffice2010(g, fBounds);
                    break;
                }
                case VisualStyle.Office2007Outlook:
				case VisualStyle.Office2007:
				{
					DrawSeparatorOffice2007( g, fBounds );
					break;
				}
				default:
				{
					DrawSeparatorDefault( g, fBounds );
					break;
				}
			}

			g.ResetTransform();
		}

		/// <summary>
		/// Draws separator for dafault visual style.
		/// </summary>
		private void DrawSeparatorDefault( Graphics g, RectangleF fBounds )
		{
			// Draw the separator line 3 pixes to the left of the renderer bounds
			Pen pen = new Pen( SystemColors.ControlDark, 1 );
			float top = fBounds.Y;
			float bottom = fBounds.Bottom;
			bool bRTL = this.IsRTL;
			float fX = bRTL ? ( fBounds.Right + 3 ) : ( fBounds.Left - 3 );

			g.DrawLine( pen, fX, top, fX, bottom );
			pen.Dispose();
		}

		/// <summary>
		/// Draws separator for Office2003 visual style.
		/// </summary>
		private void DrawSeparatorOffice2003( Graphics g, RectangleF fBounds )
		{
			Pen pen = new Pen( Office2003Colors.SeparatorColor, 1 );
			Pen shadowPen = new Pen( SystemColors.ControlLightLight, 1 );
			float top = fBounds.Y + 3.0f; ;
			float bottom = fBounds.Bottom - 5.0f;
			bool bRTL = this.IsRTL;
			float fX = bRTL ? ( fBounds.Right + 3 ) : ( fBounds.Left - 3 );
			float fShadowX = bRTL ? ( fX - 1 ) : ( fX + 1 );

			g.DrawLine( pen, fX, top, fX, bottom );
			g.DrawLine( shadowPen, fShadowX, top + 1, fShadowX, bottom + 1 );
			pen.Dispose();
			shadowPen.Dispose();
		}

		/// <summary>
		/// Draws separator for VS2005 visual style.
		/// </summary>
		private void DrawSeparatorVS2005( Graphics g, RectangleF fBounds )
		{
			Pen pen = new Pen( VS2005Colors.BarItemSeparatorColor, 1 );
			float top = fBounds.Y + 3.0f; ;
			float bottom = fBounds.Bottom - 5.0f;
			bool bRTL = this.IsRTL;
			float fX = bRTL ? ( fBounds.Right + 3 ) : ( fBounds.Left - 3 );

			g.DrawLine( pen, fX, top, fX, bottom );
			pen.Dispose();
		}

		/// <summary>
		/// Draws separator for Office2007 visual style.
		/// </summary>
		private void DrawSeparatorOffice2007( Graphics g, RectangleF fBounds )
		{
			Office2007BarItemPainter.DrawSeparator( g, fBounds, this.IsRTL );
		}

        /// <summary>
        /// Draws separator for Office2010 visual style.
        /// </summary>
        private void DrawSeparatorOffice2010(Graphics g, RectangleF fBounds)
        {
            Office2010BarItemPainter.DrawSeparator(g, fBounds, this.IsRTL);
        }

		protected bool IsTextOnly()
		{
			bool shouldDrawImage = this.ShouldDrawImage();

			if( shouldDrawImage )
			{
				IList imageList = barItem.GetImageListInternal( this.parent.LargeIcons );
				if( imageList != null )
				{
					int idx = barItem.ImageIndex;
					if( idx >= 0 && idx < imageList.Count )
					{
						return false;
					}
				}
			}
			return true;
		}

		[Obsolete( "Not used." )]
		protected virtual void DrawTextAndImage( Graphics g, RectangleF rectTextAndImage, Font textFont, Brush textBrush, Color bgColor, DrawItemState state )
		{
		}

		protected virtual void DrawTextAndImage( Graphics g, RectangleF rectTextAndImage, Font textFont, Color textColor, Color bgColor, DrawItemState state )
		{
			RectangleF rectText, rectImage;

			if( rectTextAndImage.Width <= 0 ||
				rectTextAndImage.Height <= 0 )
				return;

			bool shouldDrawImage = this.ShouldDrawImage();
			bool enabled = !( ( state & DrawItemState.Disabled ) > 0 );
			bool bRTL = this.IsRTL;

			ImageExt image = null;
			bool bDisposeImage = false;
			Size imageSize = Size.Empty;

			IList imageList = barItem.GetImageListInternal( this.parent.LargeIcons );

			// Determine text position
			TextFormatFlags textFormat = TextFormatFlags.Default;
			BarManager barMan = BarItem.barManager;

			if( null != barMan && RightToLeft.Yes == barMan.RightToLeft )
			{
				textFormat |= TextFormatFlags.RightToLeft;
			}

			if( ( !shouldDrawImage || this.barItem.ImageIndex < 0 || imageList == null || imageList.Count <= this.barItem.ImageIndex ) && this.BarItem.Image == null )
			{
				textFormat |= TextFormatFlags.HorizontalCenter;
			}
			else if( this.IsHorizontalAligned && this.DrawTextBelowImage )
			{
				textFormat |= TextFormatFlags.HorizontalCenter;
				textFormat |= TextFormatFlags.Bottom;	
			}

			rectText = GetTextPosition( g, this.barItem.Text, textFont, rectTextAndImage, textFormat );
            Font barItemFont = this.parent.GetBarControl().GetControl().Font;

            if (this.barItem != null && this.barItem.CustomTextFont != null)
            {
                barItemFont = this.barItem.CustomTextFont;
            }
            Size size = MeasureText(g, barItem.Text, barItemFont);
            if (rectTextAndImage.Width < size.Width)
            {
                rectText = (this.GetDrawingBounds());
                textFormat = TextFormatFlags.Default;
                if (null != barMan && RightToLeft.Yes == barMan.RightToLeft)
                {
                    textFormat |= TextFormatFlags.RightToLeft;
                }

                if ((!shouldDrawImage || this.barItem.ImageIndex < 0 || imageList == null || imageList.Count <= this.barItem.ImageIndex) && this.BarItem.Image == null)
                {
                    textFormat |= TextFormatFlags.HorizontalCenter;
                }
                else if (this.IsHorizontalAligned && this.DrawTextBelowImage)
                {
                    textFormat |= TextFormatFlags.HorizontalCenter;
                    textFormat |= TextFormatFlags.Bottom;
                    rectText.Height -= 5;
                }
            }
            if (!this.barItem.SizeToFit && this.barItem.ImageIndex < 0)
            {
                rectText = (this.GetDrawingBounds());
               
                textFormat = TextFormatFlags.VerticalCenter;
                if (null != barMan && RightToLeft.Yes == barMan.RightToLeft)
                {
                    textFormat |= TextFormatFlags.RightToLeft;
                }

                if ((!shouldDrawImage || this.barItem.ImageIndex < 0 || imageList == null || imageList.Count <= this.barItem.ImageIndex) && this.BarItem.Image == null)
                {
                    textFormat |= TextFormatFlags.HorizontalCenter;
                }
                else if (this.IsHorizontalAligned && this.DrawTextBelowImage)
                {
                    textFormat |= TextFormatFlags.HorizontalCenter;
                    textFormat |= TextFormatFlags.Bottom;
                }           

            }
			if( shouldDrawImage )
			{
				if( this.BarItem.Image != null )
				{
					image = this.BarItem.Image;
					imageSize = this.BarItem.ImageSize;
				}
				else
				{
					int idx = this.barItem.ImageIndex;
					if( imageList != null )
					{
						if( idx >= 0 && idx < imageList.Count )
						{
							image = new ImageExt( imageList[idx] as Image );
							image.ImageTransparentColor = this.barItem.ImageTransparentColor;

							bDisposeImage = true;
						}
						imageSize = barItem.GetImageSizeInternal( this.parent.LargeIcons );
					}

					// Rotate ImageSize based on alignment (Images will be drawn straight whatever the alignment is.
					if( !this.IsHorizontalAligned )
					{
						imageSize = new Size( imageSize.Height, imageSize.Width );
					}
				}
			}

			// Determine Image position
			CommandBarDockState parentAlignment = this.parent.Alignment;

			if( image != null )
			{
				rectImage = new RectangleF( new PointF( rectTextAndImage.Left, rectTextAndImage.Top ), imageSize );

				if( !( this.IsHorizontalAligned && this.DrawTextBelowImage ) )
				{
					// Center it.
					if( rectTextAndImage.Height > rectImage.Height )
						rectImage.Offset( 0F, ( rectTextAndImage.Height - rectImage.Height ) / 2F );

					rectText.Offset( rectImage.Width + BarItemRenderer.ImageTextPadding, 0F );
				}
				else
				{
					if( rectTextAndImage.Width > rectImage.Width )
						rectImage.Offset( ( rectTextAndImage.Width - rectImage.Width ) / 2F, 0F );
				}

				// Adjust alignment
				//if(!this.IsHorizontalAligned)
				//image.RotateFlip( RotateFlipType.Rotate270FlipNone );

				int padX = 1;
				int padY = 1;
				bool isShifted = false;
				bool needOffset = ( barItem.DrawImageMirrored || bRTL ) &&
					( parent.Alignment == CommandBarDockState.Left ||
					parent.Alignment == CommandBarDockState.Right );

				// Don't adjust if in themes mode or Office2003 style.
				if( ( !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
					&& this.Style != VisualStyle.Office2003
					&& this.Style != VisualStyle.VS2005
                    && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                    && this.Style != VisualStyle.Office2010
					&& this.Style != VisualStyle.Office2007Outlook )
					|| !enabled )
				{
					if( this.ShowingDropDown ||
						( ( this.HotTrack || this.Active || !enabled )
						&& !( this.HotTrack && this.Active ) ) )
					{
						if( ( this.HotTrack )
							|| this.ShowingDropDown
							|| ( !this.HotTrack && this.Active ) )
						{
							int nXOffset = barItem.DrawImageMirrored || bRTL ? -padX : padX;
							int nYOffset =
								( ( parent.Alignment == CommandBarDockState.Left || parent.Alignment == CommandBarDockState.Right ) ?
								-padY : padY );

							isShifted = true;

							rectImage.Offset( nXOffset, nYOffset );
						}

						/*
						 * NOTE: 22 october 2004
						 * PROBLEM: bad shadow image orientation using mirror.
						 * SOLUTION: Reseting graphics state and transformation of destination
						 * rectangle.
						 */
						//Rectangle rectImageBounds = new Rectangle( (int)rectImage.Left, (int)rectImage.Top, image.Width, image.Height );
						GraphicsState gs = g.Save();
						g.ResetTransform();
						RectangleF rectImageTrue = ApplyTransform( g, parentAlignment, rectImage, false );
						Rectangle rectImageBounds = new Rectangle( (int)rectImageTrue.Left, (int)rectImageTrue.Top, image.Width, image.Height );

						/*
						* NOTE: 22 october 2004
						* PROBLEM: Bad position of mirrored shadow in Left/Right docking.
						* Transformation of coordinates leads to bad positioning.
						* Without correction shadow will be drawn at the upper right corner from the icon.  
						* SOLUTION: Correcting of coordinates.
						*/
						if( needOffset && isShifted )
						{
							rectImageBounds.X -= 2 * padX;
							rectImageBounds.Y += 2 * padY;
						}

						using( CMirroredDrawer mdDrawer = new CMirroredDrawer( g, rectImageBounds, this.barItem.DrawImageMirrored ) )
						{
							Graphics gfxVirt = mdDrawer.VirtualGfx;
							Rectangle rectVirt = mdDrawer.VirtualBounds;

							Rectangle imRect = new Rectangle( rectVirt.Location, imageSize );

							if( state == DrawItemState.Disabled )
							{
								if( !DrawDisabledImage( gfxVirt, imRect ) && ( image != null ) )
								{
                                    Rectangle rcImage = new Rectangle(rectVirt.Location, imageSize);                                    
                                    image.Draw(gfxVirt,rcImage, DrawItemState.Disabled);
								}
							}
							else if( !( state == DrawItemState.HotLight && this.BarItem.HighlightedImage != null ) )
							{
								if( this.ShowDropShadow )
								{
									image.Draw( g, imRect, DrawItemState.Disabled );
								}
								else if( state != DrawItemState.None )
								{
									image.Draw( g, imRect, state );
								}
							}
							else
							{
								bool bLargeIcons = this.BarItem.Manager != null && this.BarItem.Manager.LargeIcons;
								bool drawShadow = !this.BarItem.IsValidHighlightedImageIndex( bLargeIcons && this.BarItem.LargeImageList != null );

								if( this.ShowDropShadow && drawShadow )
								{
									DrawingUtils.DrawShadow( gfxVirt, image.GetImage(), rectVirt.Left, rectVirt.Top );
								}
							}

						}

						g.Restore( gs );
					}
				}

				isShifted = false;
				if( enabled )
				{
					if( !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
						&& this.Style != VisualStyle.Office2003
						&& this.Style != VisualStyle.VS2005
                        && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                        && this.Style != VisualStyle.Office2010
						&& this.Style != VisualStyle.Office2007Outlook )
					{
						if( ( this.HotTrack && !this.Active ) || this.ShowingDropDown
							|| ( !this.HotTrack && this.Active ) )
						{
							int nXOffset = barItem.DrawImageMirrored || bRTL ? 2 * padX : -2 * padX;
							int nYOffset =
								( ( parent.Alignment == CommandBarDockState.Left || parent.Alignment == CommandBarDockState.Right ) ?
								2 * padY : -2 * padY );

							rectImage.Offset( nXOffset, nYOffset );
							isShifted = true;
						}
					}

					GraphicsState gs = g.Save();
					g.ResetTransform();
					RectangleF rectImageTrue = ApplyTransform( g, parentAlignment, rectImage, false );

					Rectangle rectImageBounds = ( image == null ) ?
						new Rectangle( (int)rectImageTrue.X, (int)rectImageTrue.Y, imageSize.Width, imageSize.Height ) :
						new Rectangle( (int)rectImageTrue.X, (int)rectImageTrue.Y, BarItem.ImageSize.Width, BarItem.ImageSize.Height );

					/*
					  * NOTE: 22 october 2004
					  * PROBLEM: Bad position of mirrored icon in Left/Right docking.
					  * Transformation of coordinates leads to bad positioning.
					  * Without correction icon will be drawn at the bottom left corner from the shadow.  
					  * SOLUTION: Correcting of coordinates.
					  */
					if( needOffset && isShifted )
					{
						rectImageBounds.X += padX;
						rectImageBounds.Y -= 2 * padY;
					}

					if( imageSize.Width > 0 && imageSize.Height > 0 )
					{
						using( CMirroredDrawer mdDrawer = new CMirroredDrawer( g, rectImageBounds, this.barItem.DrawImageMirrored ) )
						{
							Graphics gfxVirt = mdDrawer.VirtualGfx;
							Rectangle rectVirt = mdDrawer.VirtualBounds;

							Rectangle rcImage = new Rectangle( rectVirt.Location, imageSize );

							if (!DrawStateImage(gfxVirt, rcImage, state))
							{
								if( image != null )
								{
									image.Draw( gfxVirt, rcImage, DrawItemState.None );
								}
							}
						}
					}

					g.Restore( gs );

					//g.DrawImage(image, rectImage);

					if( !this.HotTrack && !this.Active && !this.ShowingDropDown
						// When themes is on, the BG is transparent, so we cannot use a alpha-blended BGColor brush to Fill
						&& !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
						&& this.Style != VisualStyle.Office2003
						&& this.Style != VisualStyle.OfficeXP
						&& this.Style != VisualStyle.VS2005
						&& this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                        && this.Style != VisualStyle.Office2010
						&& this.Style != VisualStyle.Office2007Outlook )
						g.FillRectangle( new SolidBrush( Color.FromArgb( 255 - MenuColors.InactiveItemAlphaBlendFactor, bgColor ) ), rectImage );
				}

				if( null != image && bDisposeImage )
				{
					image.Dispose();
				}
			}

			if( this.ShouldDrawText() )
			{
				if( !this.IsHorizontalAligned
					&& ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
					rectText.Height = rectTextAndImage.Height;
				else if( rectTextAndImage.Height > rectText.Height )
				{
					if( !( this.IsHorizontalAligned && this.DrawTextBelowImage ) )
						rectText.Offset( 0F, ( rectTextAndImage.Height - rectText.Height ) / 2F );
				}

				// Just draw string with elipses, if necessary
				TextFormatFlags format = textFormat | TextFormatFlags.EndEllipsis;

				if( this.parent.HintViaHotKeyPrefix || barItem.ShowMnemonicUnderlinesAlways )
				{
					format &= ~TextFormatFlags.HidePrefix;
				}
				else
				{
					format |= TextFormatFlags.HidePrefix;
				}

				GraphicsState savedState = null;

				if( !this.IsHorizontalAligned
					&& ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
				{
					savedState = g.Save();
					g.ResetTransform();
					rectText = BarRenderer.ApplyTransform( g, parentAlignment, rectText, false );
					format |= TextFormatFlags.HorizontalCenter;
				}

				if( rectText.Width > 0 && rectText.Height > 0 )
				{					
					DrawText( g, this.barItem.Text, textFont, textColor, rectText, format );
				}

				if( savedState != null )
				{
					g.Restore( savedState );
				}
			}
		}

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="text">The text.</param>
		/// <param name="font">The text's font.</param>		
		/// <param name="textColor">Color of the text.</param>
		/// <param name="rectText">The rectangle of the text.</param>
		/// <param name="textFormat">The text's format.</param>
		protected void DrawText( Graphics g, string text, Font textFont, Color textColor, RectangleF rectText, TextFormatFlags format )
		{
			Rectangle rcText = Rectangle.Round( rectText );
            if (this.barItem != null && this.barItem is ParentBarItem && (this.barItem as ParentBarItem).MultiLine)
                format = TextFormatFlags.WordBreak;
            int height = MeasureText(g, this.BarItem.Text, textFont).Height;
            Size size = new Size(height, height * this.BarItem.Text.Length);
            using (TextRendererDC dc = new TextRendererDC(g))
            {
                if (this.barItem != null && this.barItem is ParentBarItem) 
                {
                    if((this.barItem as ParentBarItem).Orientation == Orientation.Horizontal)
                    {
                        if ((this.barItem as ParentBarItem).MultiLine && (this.barItem as ParentBarItem).Text.Length > 15)
                        {
                            rcText.Y += 3;
                            rcText.X += 3;
                        }
                    if ((this.barItem as ParentBarItem).UseGDIDrawing)
                            TextRenderer.DrawText(dc, text, textFont, rcText, textColor, format);
                        else
                        {
                            using (SolidBrush brush = new SolidBrush(textColor))
                            {
                                g.DrawString(this.BarItem.Text, textFont, brush, rcText);
                            }
                        }
                    }
                    else if ((this.barItem as ParentBarItem).Orientation == Orientation.Vertical)
                    {
                        g.TranslateTransform(rcText.X + 5, rcText.Bottom - 5);
                        g.RotateTransform(270);
                        StringFormat sf = new StringFormat();
                        sf.HotkeyPrefix = HotkeyPrefix.Show;
                        using (SolidBrush brush = new SolidBrush(textColor))
                        {
                            g.DrawString(text, textFont, brush, 0, 0, sf);
                        }
                        g.ResetTransform();
                    }
                }
                else if(barItem != null)
                {
                    TextRenderer.DrawText(dc, text, textFont, rcText, textColor, format);
                }
            }
		}

		protected virtual RectangleF GetDrawingBounds()
		{
			return this.Bounds;
		}

		protected virtual RectangleF GetTextAndImageRect( Graphics g, Rectangle clippingRect )
		{
			// Determine rect for Text and Image and Adjust for padding
			RectangleF rectTextAndImage = this.Bounds;
			rectTextAndImage = RectangleF.Inflate( rectTextAndImage, -BarItemRenderer.PadX,
				-BarItemRenderer.PadY );

			rectTextAndImage.X += 1;
			rectTextAndImage.Width -= 1;

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
                && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                && this.Style != VisualStyle.Office2010
				&& this.Style != VisualStyle.Office2007Outlook )
			{
				rectTextAndImage.Inflate( -PaddingForThemesX / 2, -PaddingForThemesY / 2 );
			}
			else
			{
				rectTextAndImage.Inflate( -Padding.X / 2, -Padding.Y / 2 );
			}

			rectTextAndImage = ApplyTransform( g, this.parent.Alignment, rectTextAndImage, false );

			return rectTextAndImage;
		}

		protected virtual Color GetBGColor2()
		{
			Color bgColor = Color.Empty;

			if( this.ShowHighlightRectangle && !barItem.IsValidHighlightedImageIndex( parent.LargeIcons ) )
			{
				if( this.barItem.Checked )
				{
					bgColor = GetCheckedItemDarkColor( this.Style );
				}
				else if( this.HotTrack && this.Active )
				{
					bgColor = GetPressedItemDarktColor( this.Style );
				}
				else if( this.HotTrack || this.Active )
				{
					bgColor = ( this.barItem.Checked ) ?
						GetCheckedItemDarkColor( this.Style ) : GetHighlightItemDarkColor( this.Style );
				}
			}

			return bgColor;
		}

		protected bool ShowHighlightRectangle
		{
			get
			{
				bool bShow = true;

				if( this.barItem.Manager != null )
				{
					bShow = this.BarItem.Manager.ShowHighlightRectangle;
				}
				else
				{
					BarRenderer barRenderer = this.parent as BarRenderer;

					if( barRenderer != null )
					{
						XPToolBar xpToolBar = barRenderer.GetBarControl() as XPToolBar;

						if( xpToolBar != null )
						{
							bShow = xpToolBar.ShowHighlightRectangle;
						}
					}
				}

				return bShow;
			}
		}

		protected bool ShowDropShadow
		{
			get
			{
				return ( this.barItem.Manager == null ||
					this.barItem.Manager.ShowDropShadow );
			}
		}

		protected virtual Color GetBGColor()
		{
			Color bgColor = this.parent.GetBarControl().GetControl().BackColor;

			if( this.ShowHighlightRectangle && !barItem.IsValidHighlightedImageIndex( parent.LargeIcons ) )
			{
				if( this.HotTrack && this.Active )
				{
					bgColor = GetPressedItemLightColor( this.Style );
				}
				else if( this.HotTrack || this.Active )
				{
					bgColor = ( this.barItem.Checked ) ?
						GetCheckedItemLightColor( this.Style ) : GetHighlightItemLightColor( this.Style );
				}
				else
				{
					if( this.Style != VisualStyle.Office2003
						&& this.Style != VisualStyle.VS2005
                        && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                        && this.Style != VisualStyle.Office2010
						&& this.Style != VisualStyle.Office2007Outlook )
					{
						if( this.barItem.Checked )
							bgColor = Color.FromArgb( 80, bgColor );
					}
					else
					{
						if( this.barItem.Checked )
						{
							if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
								&& this.ThemesEnabled
                                && this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Metro
                                && this.Style != VisualStyle.Office2010
								&& this.Style != VisualStyle.Office2007Outlook )
							{
								bgColor = Color.FromArgb( 80, bgColor );
							}
							else
							{
								bgColor = GetCheckedItemLightColor( this.Style );
							}
						}
					}
				}
			}

			return bgColor;
		}

		protected virtual Color GetForeColor( DrawItemState state )
		{
			Color foreColor = Color.Empty;

			switch( this.Style )
			{
				case VisualStyle.Office2003:
				{
					foreColor = GetForeColorOffice2003( state );
					break;
				}
				case VisualStyle.VS2005:
				{
					foreColor = GetForeColorVS2005( state );
					break;
				}
				case VisualStyle.Office2007:
				case VisualStyle.Office2007Outlook:
				{
					foreColor = GetForeColorOffice2007( state );
					break;
				}
                case VisualStyle.Office2010:
                {
                    foreColor = GetForeColorOffice2010(state);
                    break;
                }
				default:
				{
					foreColor = GetForeColorDefault( state );
					break;
				}
			}

			return foreColor;
		}

		/// <summary>
		/// Gets fore color for default visual style.
		/// </summary>
		protected virtual Color GetForeColorDefault( DrawItemState state )
		{
			Color foreColor;

			if( this.barItem == null || this.barItem.CustomNormalTextColor == Color.Empty )
			{
				foreColor = this.parent.GetBarControl().UseControlForeColor ?
					this.parent.GetBarControl().GetControl().ForeColor : MenuColors.MenuTextColor;
			}
			else
			{
				foreColor = this.BarItem.CustomNormalTextColor;
			}

			if( ( state & DrawItemState.Disabled ) > 0 )
			{
				foreColor = GetDisabledForeColor( barItem );
			}
			else if( this.HotTrack || this.Active )
			{
				if( this.Active )
				{
					if( this.barItem == null || this.barItem.CustomActiveTextColor == Color.Empty )
					{
						foreColor = MenuColors.MenuActiveTextColor;
					}
					else
					{
						foreColor = this.barItem.CustomActiveTextColor;
					}
				}
				else
				{
					if( this.barItem.Enabled )
						foreColor = MenuColors.SelTextColor;
				}
			}

			return foreColor;
		}

		/// <summary>
		/// Gets fore color for Office2007 visual style.
		/// </summary>
		protected virtual Color GetForeColorOffice2007( DrawItemState state )
		{
			Color foreColor = Color.Empty;

			if( this.barItem == null || this.barItem.CustomNormalTextColor == Color.Empty )
			{
				if( this.BarItem.barManager != null && this.BarItem.barManager.MainFrameBarManager != null
					&& this.BarItem.barManager.MainFrameBarManager.Office2007Theme == Office2007Theme.Black
					&& this.IsParentMainMenu &&
					( ( !this.HotTrack && !this.Active ) || this.barItem is IRequiresControl ) )
				{
					// only Black colorschemes of the Office2007 visual style
					foreColor = Color.White;
				}
				else
				{
					foreColor = this.parent.GetBarControl().UseControlForeColor ?
						this.parent.GetBarControl().GetControl().ForeColor : MenuColors.MenuTextColor;
				}
			}
			else
			{
				foreColor = this.BarItem.CustomNormalTextColor;
			}

			if( ( state & DrawItemState.Disabled ) > 0 )
			{
				foreColor = GetDisabledForeColor( barItem );
			}

			return foreColor;
		}

        /// <summary>
        /// Gets fore color for Office2010 visual style.
        /// </summary>
        protected virtual Color GetForeColorOffice2010(DrawItemState state)
        {
            Color foreColor = Color.Empty;

            if (this.barItem == null || this.barItem.CustomNormalTextColor == Color.Empty)
            {
                if (this.BarItem.barManager != null && this.BarItem.barManager.MainFrameBarManager != null
                    && this.BarItem.barManager.MainFrameBarManager.Office2010Theme == Office2010Theme.Black
                    && this.IsParentMainMenu &&
                    ((!this.HotTrack && !this.Active) || this.barItem is IRequiresControl))
                {
                    // only Black colorschemes of the Office2010 visual style
                    foreColor = Color.White;
                }
                else
                {
                    if ((this.parent.GetBarControl() is XPToolBar) && state == DrawItemState.None)
                    {
                        if ((this.parent.GetBarControl() as XPToolBar).Office2010Theme == Office2010Theme.Black)
                        foreColor = Color.White;
                    }
                    else
                    {
                        foreColor = this.parent.GetBarControl().UseControlForeColor ?
                            this.parent.GetBarControl().GetControl().ForeColor : MenuColors.MenuTextColor;
                    }
                }
            }
            else
            {
                foreColor = this.BarItem.CustomNormalTextColor;
            }

            if ((state & DrawItemState.Disabled) > 0)
            {
                foreColor = GetDisabledForeColor(barItem);
            }

            return foreColor;
        }
		/// <summary>
		/// Gets fore color for VS2005 visual style.
		/// </summary>
		protected virtual Color GetForeColorVS2005( DrawItemState state )
		{
			Color foreColor = GetForeColorDefault( state );

			if( ( state & DrawItemState.Disabled ) == 0
				&& ( this.barItem == null || this.barItem.CustomNormalTextColor == Color.Empty )
				&& !this.parent.GetBarControl().UseControlForeColor )
			{
				foreColor = Color.Black;
			}

			return foreColor;
		}
		/// <summary>
		/// Gets fore color for Office2003 visual style.
		/// </summary>
		protected virtual Color GetForeColorOffice2003( DrawItemState state )
		{
			Color foreColor = Color.Empty;

			if( this.barItem == null || this.barItem.CustomNormalTextColor == Color.Empty )
			{
				foreColor = this.parent.GetBarControl().UseControlForeColor ?
					this.parent.GetBarControl().GetControl().ForeColor : MenuColors.MenuTextColor;
			}
			else
			{
				foreColor = this.BarItem.CustomNormalTextColor;
			}

			if( ( state & DrawItemState.Disabled ) > 0 )
			{
				foreColor = GetDisabledForeColor( barItem );
			}

			return foreColor;
		}
		/// <summary>
		/// Gets disabled forecolor for current BarItem control.
		/// </summary>
		/// <param name="barItem"> Current BarItem control. </param>
		/// <returns> Color that is forecolor for BarItem control. </returns>
		private Color GetDisabledForeColor( BarItem barItem )
		{
			Color clDisabledForeColor = Color.Empty;

			if( this.barItem == null || this.barItem.CustomDisabledTextColor == Color.Empty )
			{
				clDisabledForeColor = MenuColors.DisabledToolbarItemTextColorBase;
			}
			else
			{
				clDisabledForeColor = this.barItem.CustomDisabledTextColor;
			}

			return clDisabledForeColor;
		}
		public virtual void OnPaint( Graphics g, Rectangle clippingRect )
		{
			if( !this.Visible )
				return;

			RectangleF clipRect = BarRenderer.ApplyTransform( g, this.parent.Alignment, clippingRect, true );

			// Draw even if a small portion is visible
			if( !this.Bounds.IntersectsWith( clipRect ) )
				return;

			// Determine current bounds
			RectangleF currentBounds = this.GetDrawingBounds();

			lastDrawnBounds = currentBounds;	// Necessary for efficient painting

			CommandBarDockState align = this.parent.Alignment;

			currentBounds = ApplyTransform( g, align, currentBounds, false );

			RectangleF rectTextAndImage = this.GetTextAndImageRect( g, clippingRect );
			Color bgColor = this.GetBGColor();
			Color bgColor2 = this.GetBGColor2();

			// Determine DrawItemState
			DrawItemState state = DrawItemState.None;
			if( this.HotTrack )
				state |= DrawItemState.HotLight;

			if( !this.barItem.Enabled &&
				( ( !this.parent.Customizing || this.parent.DndCustomizing )
				&& ( this.barItem.Manager == null || !this.barItem.Manager.Customizing || this.barItem.Manager.DndCustomizing ) ) )
				state |= DrawItemState.Disabled;

			// Determine Text font and forecolor
			Color foreColor = this.GetForeColor( state );

			Font textFont = this.parent.GetBarControl().GetControl().Font;
			if( this.barItem != null && this.barItem.CustomTextFont != null )
			{
				textFont = this.barItem.CustomTextFont;
			}

			ProvideFontInfoEventArgs args = new ProvideFontInfoEventArgs( textFont );
			this.barItem.OnProvideFontInfo( args );
			textFont = args.Font;

			// Create DrawToolbarItemEventArgs
			DrawToolbarItemEventArgs drawItemEventArgs = new DrawToolbarItemEventArgs( g,
				this.IsMouseDown(), this.ShowingDropDown, textFont, Rectangle.Round( currentBounds ),
				-1, state, foreColor, bgColor, bgColor2, Rectangle.Round( rectTextAndImage ),
				new DrawToolbarItemEventArgs.DrawDefaultBackground( this.DrawBackground ),
				new DrawToolbarItemEventArgs.DrawDefaultBorders( this.DrawBorders ),
				new DrawToolbarItemEventArgs.DrawDefaultInterior( this.DrawInterior ) );

			if( !this.barItem.OnDrawToolbarItem( drawItemEventArgs ) )
			{
				DrawBackground( drawItemEventArgs, true );
                if (style != VisualStyle.Metro)
				DrawBorders( drawItemEventArgs, true );
				DrawInterior( drawItemEventArgs );
			}
		}

		protected virtual bool IsMouseDown()
		{
			return this.Active;
		}

		protected virtual int GetCurrentThemeState( DrawToolbarItemEventArgs drawItemInfo )
		{
			int state = ThemeStates.TS_NORMAL;
			if( ( drawItemInfo.State & DrawItemState.Disabled ) > 0 )
			{
				if (this.barItem.Checked)
					state = ThemeStates.TS_CHECKED;
				else
					state = ThemeStates.TS_DISABLED;
			}
			else
			{
				if( this.HotTrack )
					state = ThemeStates.TS_HOT;

				if( this.barItem.Checked )
				{
					state = ThemeStates.TS_CHECKED;
					if( this.HotTrack )
						state = ThemeStates.TS_HOTCHECKED;
				}
				if( this.Active )
					state = ThemeStates.TS_PRESSED;
			}
			return state;
		}
		protected virtual void DrawBackground( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			Graphics g = drawItemInfo.Graphics;
			Rectangle bgBounds = drawItemInfo.Bounds;

            if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
			{
				if( this.ShowHighlightRectangle )
				{
					ItemState state = GetDrawState();
					bool bHorizontal = this.IsHorizontalAligned;

					if( !this.IsHorizontalAligned
						&& ( this.parent.Bar.BarStyle & BarStyle.RotateWhenVertical ) > 0 )
					{
						bHorizontal = true;
					}
                    if(this.Style == VisualStyle.Office2007)
					    Office2007BarItemPainter.DrawBarItem( g, bgBounds, state, bHorizontal );
                    else
                        Office2010BarItemPainter.DrawBarItem(g, bgBounds, state, bHorizontal);
				}

				return;
			}

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu && useThemes
				&& this.Style != VisualStyle.Office2007Outlook )
			{
				if( ( this.HotTrack || this.Active ) && !this.ShowHighlightRectangle )
				{
					return;
				}

				ThemedToolBarDrawing themedDrawing = this.parent.GetBarControl().ThemedDrawing;
				// Determine state
				int state = this.GetCurrentThemeState( drawItemInfo );

				themedDrawing.DrawButton( g, ThemeParts.TP_BUTTON, state, bgBounds );
			}
			else
			{
				Color bgColor = drawItemInfo.BackColor;

				// Take a look at BarControlInternal constructor for notes on why we need this check.
				if( bgColor != this.parent.GetBarControl().GetControl().BackColor )
				{
					Brush br = null;

					if( this.barItem.Checked )
					{
						Color selColor = MenuColors.SelColor;

						// Blend 30% of MenuSelColor - as it's done in according CellRenderer
						bgColor = Color.FromArgb(
							(int)( ( 0.3 * selColor.R + bgColor.R ) / 1.3 ),
							(int)( ( 0.3 * selColor.G + bgColor.G ) / 1.3 ),
							(int)( ( 0.3 * selColor.B + bgColor.B ) / 1.3 ) );
					}

					if( drawItemInfo.BackColor2 != Color.Empty &&this.Style != VisualStyle.Metro )
					{
						br = ( this.IsHorizontalAligned ) ?
							new LinearGradientBrush( bgBounds, bgColor, drawItemInfo.BackColor2, LinearGradientMode.Vertical ) :
							new LinearGradientBrush( bgBounds, bgColor, drawItemInfo.BackColor2, LinearGradientMode.Horizontal );
					}
					else
					{
						br = new SolidBrush( bgColor );
					}

					g.FillRectangle( br, bgBounds );
				}
			}
		}

		/// <summary>
		/// Gets state for drawing of the BarItem.
		/// </summary>
		protected virtual ItemState GetDrawState()
		{
			ItemState state = ItemState.Normal;

			if( this.barItem.Checked && this.HotTrack )
			{
				state = ItemState.Collapsed;
			}
			else if( this.barItem.Checked )
			{
				state = ItemState.Checked;
			}
			else if( this.Active )
			{
				state = ItemState.Pressed;
			}
			else if( this.HotTrack )
			{
				state = ItemState.Selected;
			}

			return state;
		}

		protected virtual void DrawBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
				&& useThemes && this.Style != VisualStyle.Office2007Outlook )
				return;

            if ((this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010) && this.Style != VisualStyle.Metro)
			{
				return;
			}

			Graphics g = drawItemInfo.Graphics;
			if( ( ( this.HotTrack || this.Active )
				|| this.barItem.Checked ) && this.ShowHighlightRectangle )
			{
				bool bPressed = this.HotTrack && this.Active;
				Pen borderPen = null;

				if( bPressed )
				{
					borderPen = GetBorderPressPen( this.Style );
				}
				else
				{
					borderPen = ( this.BarItem.Checked ) ? GetBorderCheckPen( this.Style ) :
						GetBorderPen( this.Style );
				}

				Rectangle curBounds = drawItemInfo.Bounds;
				curBounds.Width -= 1;
				curBounds.Height -= 1;
				g.DrawRectangle( borderPen, curBounds );
			}
		}
		protected virtual void DrawInterior( DrawToolbarItemEventArgs drawItemInfo )
		{
			Graphics g = drawItemInfo.Graphics;

			// Convert to horizontal co-ords
			RectangleF rectTextAndImage = BarRenderer.ApplyTransform( g, this.parent.Alignment, drawItemInfo.BoundsInterior, true );

			// Transform g to horizontal co-ords
			this.ApplyTransform( g );

			DrawTextAndImage( g, rectTextAndImage, drawItemInfo.Font, drawItemInfo.ForeColor, drawItemInfo.BackColor, drawItemInfo.State );

			g.ResetTransform();
		}
		#endregion DRAWING

		public virtual bool ProcessKeyDown( Keys key )
		{
			return false;
		}

		protected virtual void HideDropDown()
		{
		}

		protected virtual bool ShowDropDown( Queue pbiQueue )
		{
			return false;
		}

		#region UTILS

		protected virtual void ApplyTransform( Graphics g )
		{
			switch( this.parent.Alignment )
			{
				case CommandBarDockState.Left:
				case CommandBarDockState.Right:
				g.RotateTransform( -270.0F ); return;
				default:
				return;
			}
		}

		public virtual bool HitTest( PointF mousePosition )
		{
			if( this.Bounds.Contains( mousePosition ) )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets pen for border of the BarItem amenably with VisualStyle.
		/// </summary>
		protected Pen GetBorderPen( VisualStyle style )
		{
			Pen pen = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					pen = new Pen( Office2003Colors.SelBorderColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					pen = new Pen( Office2007OutlookColors.BarItemHighlightBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.BarItemHighlightBorderColor );
					break;
				}
				default:
				{
					pen = new Pen( MenuColors.SelBorderColor );
					break;
				}
			}

			return pen;
		}

		/// <summary>
		/// Gets pen for border of the pressed BarItem amenably with VisualStyle.
		/// </summary>
		protected Pen GetBorderPressPen( VisualStyle style )
		{
			Pen pen = GetBorderPen( style );

			if( style == VisualStyle.Office2007Outlook )
			{
				pen = new Pen( Office2007OutlookColors.BarItemPressBorderColor );
			}

			return pen;
		}

		/// <summary>
		/// Gets pen for border of the checked BarItem amenably with VisualStyle.
		/// </summary>
		protected Pen GetBorderCheckPen( VisualStyle style )
		{
			Pen pen = GetBorderPen( style );

			if( style == VisualStyle.Office2007Outlook )
			{
				pen = new Pen( Office2007OutlookColors.BarItemCheckBorderColor );
			}

			return pen;
		}

		/// <summary>
		/// Gets light color of the pressed BarItem amenably with VisualStyle.
		/// </summary>
		protected virtual Color GetPressedItemLightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.PressedSelColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.BarItemPressLightColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.BarItemPressLightColor;
					break;
				}
				default:
				{
					color = MenuColors.PressedSelColor;
					break;
				}
			}

			return color;
		}

		/// <summary>
		/// Gets dark color of the pressed BarItem amenably with VisualStyle.
		/// </summary>
		protected virtual Color GetPressedItemDarktColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.MenuItemHotColorDark;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.BarItemPressDarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.BarItemPressDarkColor;
					break;
				}
				default:
				{
					color = MenuColors.PressedSelColor;
					break;
				}
			}

			return color;
		}

		/// <summary>
		/// Gets light color of the highlighted BarItem amenably with VisualStyle.
		/// </summary>
		protected virtual Color GetHighlightItemLightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.SelColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.BarItemHighlightLightColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.BarItemHighlightLightColor;
					break;
				}
                case VisualStyle.Metro:
                {
                    if (this.parent.GetBarControl() != null && this.parent.GetBarControl().GetControl() != null)
                        color = (this.parent.GetBarControl().GetControl() as BarControlInternal).MetroColor;
                    else
                        color = Color.Blue;
                    break;
                }
				default:
				{
					color = MenuColors.SelColor;
					break;
				}
			}

			return color;
		}

		/// <summary>
		/// Gets dark color of the highlighted BarItem amenably with VisualStyle.
		/// </summary>
		protected virtual Color GetHighlightItemDarkColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.MenuItemHotColorDark;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.BarItemHighlightDarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.BarItemHighlightDarkColor;
					break;
				}
				default:
				{
					color = MenuColors.SelColor;
					break;
				}
			}

			return color;
		}


		/// <summary>
		/// Gets light color of the checked BarItem amenably with VisualStyle.
		/// </summary>
		protected virtual Color GetCheckedItemLightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.CheckedSelColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = ( this.HotTrack ) ? Office2007OutlookColors.BarItemPressLightColor :
							Office2007OutlookColors.BarItemCheckLightColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = ( this.HotTrack ) ? VS2005Colors.BarItemPressLightColor :
							VS2005Colors.BarItemCheckLightColor;
					break;
				}
				default:
				{
					color = MenuColors.CheckedSelColor;
					break;
				}
			}

			return color;
		}

		/// <summary>
		/// Gets dark color of the checked BarItem amenably with VisualStyle.
		/// </summary>
		private Color GetCheckedItemDarkColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.CheckedSelColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = ( this.HotTrack ) ? Office2007OutlookColors.BarItemPressDarkColor :
							Office2007OutlookColors.BarItemCheckDarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = ( this.HotTrack ) ? VS2005Colors.BarItemPressDarkColor :
							VS2005Colors.BarItemCheckDarkColor;
					break;
				}
				default:
				{
					color = MenuColors.CheckedSelColor;
					break;
				}
			}

			return color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="align"></param>
		/// <param name="rect"></param>
		/// <param name="apply"></param>
		/// <returns></returns>
		internal RectangleF ApplyTransform( Graphics g, CommandBarDockState align, RectangleF rect, bool apply )
		{
			return BarRenderer.ApplyTransform(g, align, rect, apply);
				}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		private bool DrawDisabledImage( Graphics g, Rectangle rc )
		{
			IList images = barItem.GetDisabledImageListInternal( parent.LargeIcons );
			if( images != null )
			{
				int idx = barItem.DisabledImageIndex;
				if( idx >= 0 && idx < images.Count )
				{
					Image image = images[idx] as Image;
					if( image != null )
					{
						DrawingUtils.DrawImage( g, image, rc.Left, rc.Top, rc.Width, rc.Height );
						return true;
					}
				}
			}

			if( barItem.DisabledImage != null )
			{
				barItem.DisabledImage.Draw( g, rc, DrawItemState.None );
				return true;
			}

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		private bool DrawStateImage(Graphics g, Rectangle rc, DrawItemState state)
		{
			bool bResult = false;

			if (state == DrawItemState.HotLight)
			{
				bResult = DrawPressedImage(g, rc) || DrawHighlightImage(g, rc);
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		private bool DrawPressedImage(Graphics g, Rectangle rc)
		{
			if (this.Active)
			{
				IList images = barItem.GetPressedImageListInternal(parent.LargeIcons);

				int index = barItem.PressedImageIndex;

				return DrawImageInternal(g, rc, images, index);
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		private bool DrawHighlightImage( Graphics g, Rectangle rc )
		{
			IList images = barItem.GetHighlightImageListInternal(parent.LargeIcons);

			int index = barItem.HighlightedImageIndex;

			if (DrawImageInternal(g, rc, images, index))
			{
				return true;
			}

			if( barItem.HighlightedImage != null )
			{
				barItem.HighlightedImage.Draw( g, rc, DrawItemState.None );

				return true;
			}

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="images"></param>
		/// <param name="idx"></param>
		/// <returns></returns>
		private bool DrawImageInternal(Graphics g, Rectangle rc, IList images, int idx)
		{
			bool bResult = false;

			if (images != null && idx >= 0 && idx < images.Count)
			{
				Image image = images[idx] as Image;
				if (image != null)
				{
					DrawingUtils.DrawImage(g, image, rc.Left, rc.Top, rc.Width, rc.Height);
					bResult = true;
				}
			}

			return bResult;
		}
		#endregion UTILS
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class MultilineBarRenderer: BarRenderer
	{
		protected float lineSeparatorAreaY = 6F;
		Hashtable cachedLineBreaks = null;
		int itemsOnFirstLine = -1;
		public MultilineBarRenderer( IBarControl parent )
			: base( parent )
		{
		}

		protected Hashtable GetLineBreaks( float availableWidth, SizeF[] preferredSizes )
		{
			if( this.barItemRenderers.Count == 0 )
				return null;

			Hashtable lineBreaks = new Hashtable();
			this.itemsOnFirstLine = -1;

			float curLineWidth = padX, curLineHeight = 0;
			IBarItemRenderer latestGroupBeginner = null, latestLineBeginner = null,
				latestLineBreakEntry = this.barItemRenderers[0] as IBarItemRenderer;
			int latestGroupBeginnerIndex = -1;
			bool previousItemWasSeparator = false;

			for( int i = 0; i < this.barItemRenderers.Count; )
			{
				IBarItemRenderer renderer = this.barItemRenderers[i] as IBarItemRenderer;

				// Keep track of line beginner
				if( latestLineBeginner == null )
					latestLineBeginner = renderer;

				bool itemVisible = this.ShouldDrawVisible( renderer.BarItem, true );
				// Keep track of group beginner
				if( this.Bar.IsGroupBeginning( renderer.BarItem )
					&& !previousItemWasSeparator )
				{
					// if the item is invisible, draw the separator only if the item is from the main manager
					if( itemVisible ||
						renderer.BarItem.Manager == null || renderer.BarItem.Manager is MainFrameBarManager )
					{
						previousItemWasSeparator = true;
						latestGroupBeginner = renderer;
						latestGroupBeginnerIndex = i;
						if( latestGroupBeginner != latestLineBeginner )
							curLineWidth += this.separatorAreaX;
					}
				}

				if( itemVisible )
				{
					previousItemWasSeparator = false;
					curLineWidth += preferredSizes[i].Width;
				}

				// If exceeded available width and more than 1 item in this row, then...
				if( curLineWidth > availableWidth
					&& latestLineBeginner != renderer
					&& itemVisible )
				{
					if( latestLineBreakEntry != null )
						lineBreaks[latestLineBreakEntry] = curLineHeight;

					if( latestLineBeginner != latestGroupBeginner
						&& latestGroupBeginner != null )
					{
						// A group beginner is available to wrap, go ahead and wrap
						i = latestGroupBeginnerIndex;
						latestLineBreakEntry = latestGroupBeginner;
						lineBreaks[latestGroupBeginner] = 1;
					}
					else
					{
						// No group beginner available, wrap the current item
						lineBreaks[renderer] = 1;
						latestLineBreakEntry = renderer;
					}

					if( this.itemsOnFirstLine == -1 )
						this.itemsOnFirstLine = i;

					// Refresh states for the new row after the wrap
					latestLineBeginner = latestGroupBeginner = null;
					latestGroupBeginnerIndex = -1;
					curLineWidth = padX;
					curLineHeight = 0;
				}
				else
				{
					if(
						//(renderer.BarItem.Visible || this.Customizing) 
						( itemVisible || renderer.BarItem.PaintStyle != PaintStyle.TextOnly ) && curLineHeight < preferredSizes[i].Height )
						curLineHeight = preferredSizes[i].Height;

					// Enough space available, go ahead...
					i++;
				}
			}
			if( latestLineBreakEntry != null )
				lineBreaks[latestLineBreakEntry] = curLineHeight;

			if( this.itemsOnFirstLine == -1 )
				this.itemsOnFirstLine = this.barItemRenderers.Count;

			return lineBreaks;
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.cachedLineBreaks = null;
        }

		public override void GetPreferredSize( IGraphicsProvider gp, ref SizeF preferredSize )
		{
			float availableWidth = ( this.IsVerticallyAligned ) ? preferredSize.Height : preferredSize.Width;
			preferredSize = this.ComputePreferredSizeAndSetBounds( gp, availableWidth, false );

			bool shouldDrawImage = false;
			Size imageSize = Size.Empty;

			if( barItemRenderers.Count > 0 )
			{
				foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
				{
					BarItem item = barItemRenderer.BarItem;

					if( !this.ShouldDrawVisible( item, true ) )
					{
						shouldDrawImage = ( item.PaintStyle != PaintStyle.TextOnly );
						imageSize = item.GetImageSizeInternal( this.parent.LargeIcons );
					}
				}
			}

			if( preferredSize.Width < 10 )
				preferredSize.Width = 10;

			if( shouldDrawImage && this.parent != null &&
				( preferredSize.Height < ( imageSize.Height + padY + BarItemRenderer.PadY * 2 ) ) )
			{
				preferredSize.Height = imageSize.Height + padY + BarItemRenderer.PadY * 2;
			}

			if( preferredSize.Height < 22 )
				preferredSize.Height = 30;
		}

		public virtual SizeF ComputePreferredSizeAndSetBounds( IGraphicsProvider gp, float availableWidth, bool setBounds )
		{
			if( this.barItemRenderers.Count == 0 )
				return SizeF.Empty;

			if( availableWidth == 0 )
			{
				availableWidth = GetMinimumWidth( gp );
				availableWidth += padX;
			}

			if( setBounds )
				this.InvisibleBarItems.Clear();

			// First get an array of preferredSizes
			SizeF[] preferredSizes = new SizeF[this.barItemRenderers.Count];
			int i = 0;
			foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
				preferredSizes[i++] = barItemRenderer.GetPreferredSize( gp );

			// Compute initial line breaks due to groups
			Hashtable lineBreaks = this.GetLineBreaks( availableWidth, preferredSizes );
			this.cachedLineBreaks = lineBreaks;

			float top = this.tdbounds.Y + padY / 2, currentLineHeight = (float)lineBreaks[this.barItemRenderers[0]];
			float largestLineWidth = 0;
			if( this.barItemRenderers.Count > 0 )
			{
				float usedUpLineWidth = padX;
				int index = -1;
				bool previousItemWasSeparator = false;
				foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
				{
					bool bRTL = this.parent.IsRightToLeft;

					index++;
					SizeF preferredBarItemSize = preferredSizes[index];

					bool drawItemVisible = this.ShouldDrawVisible( barItemRenderer.BarItem, true );

					if( this.Bar.IsGroupBeginning( barItemRenderer.BarItem )
						&& !previousItemWasSeparator )
					{
						if( drawItemVisible ||
							barItemRenderer.BarItem.Manager == null || barItemRenderer.BarItem.Manager is MainFrameBarManager )
						{
							previousItemWasSeparator = true;
							usedUpLineWidth += this.separatorAreaX;
						}
					}

					if( index != 0 && lineBreaks[barItemRenderer] != null )
					{
						if( previousItemWasSeparator )
						{
							// No need of this space because there is a line break instead.
							usedUpLineWidth -= this.separatorAreaX;
						}
						if( drawItemVisible )
							previousItemWasSeparator = false;
						// move to next line
						top += currentLineHeight + this.lineSeparatorAreaY;

						// This item becomes the first item in the new line
						if( largestLineWidth < usedUpLineWidth )
							largestLineWidth = usedUpLineWidth;
						usedUpLineWidth = padX;

						currentLineHeight = (float)lineBreaks[barItemRenderer];
					}

					if( setBounds )
					{
						// Invisible items still needs bounds, since we use the
						// item's bounds to draw the separator lines.
						float width = 0, left = 0;
						width = preferredBarItemSize.Width;
						left = this.tdbounds.X + usedUpLineWidth;

						bool addToInvisibleList = false;

						if( left < this.tdbounds.Left || top < this.tdbounds.Top
							|| left + width > this.tdbounds.Right
							|| top + currentLineHeight > this.tdbounds.Height )
						{
							if( drawItemVisible )
								addToInvisibleList = true;
							drawItemVisible = false;
						}

						if( addToInvisibleList || ( !drawItemVisible && this.ShouldDrawVisible( barItemRenderer.BarItem, false ) ) )
							this.InvisibleBarItems.Add( barItemRenderer.BarItem );

						float itemHeight = currentLineHeight;
						float centeredTop = top;
						// Center it if necessary
						if( preferredBarItemSize.Height < currentLineHeight
							&& barItemRenderer.NeedCenterVAlign )
						{
							centeredTop += ( currentLineHeight - preferredBarItemSize.Height ) / 2;
							itemHeight = preferredBarItemSize.Height;
						}

						if( bRTL )
						{
							barItemRenderer.Bounds = new RectangleF(
								this.tdbounds.Width - this.tdbounds.X - usedUpLineWidth - width,
								centeredTop, width, itemHeight );
						}
						else
						{
							barItemRenderer.Bounds = new RectangleF( this.tdbounds.X + usedUpLineWidth,
								centeredTop, width, itemHeight );
						}

						barItemRenderer.Visible = drawItemVisible;
					}

					if( drawItemVisible )
					{
						previousItemWasSeparator = false;
						usedUpLineWidth += preferredBarItemSize.Width;
					}
				}
				if( largestLineWidth < usedUpLineWidth )
					largestLineWidth = usedUpLineWidth;
			}
			// top includes heights of all the rows except the last one.
			return new SizeF( largestLineWidth, top - this.tdbounds.Top + currentLineHeight + padY / 2 );
		}

		protected float GetMinimumWidth( IGraphicsProvider gp )
		{
			float minWidth = 0;
			if( this.barItemRenderers.Count > 0 )
			{
				foreach( IBarItemRenderer barItemRenderer in this.barItemRenderers )
				{
					float barItemMinWidth = barItemRenderer.GetPreferredSize( gp ).Width;
					if( minWidth < barItemMinWidth )
						minWidth = barItemMinWidth;
				}
			}
			return minWidth;
		}
		public override void ComputeBarItemPositions( IGraphicsProvider gp )
		{
			if( barItemRenderers.Count <= 0 )
				return;

			this.ComputePreferredSizeAndSetBounds( gp, this.tdbounds.Width, true );
		}
		public override void OnPaint( Graphics g, Rectangle clipRect )
		{
			base.OnPaint( g, clipRect );
			// Draw the line separators
			if( this.cachedLineBreaks != null )
			{
				if( ( this.Alignment == CommandBarDockState.Left || this.Alignment == CommandBarDockState.Right ) )
					g.RotateTransform( -270.0F );

				int i = -1;
				foreach( IBarItemRenderer renderer in this.barItemRenderers )
				{
					i++;
					// We don't have to do this for the first line.
					if( i < itemsOnFirstLine )
						continue;

					float left = -1;
					if( this.cachedLineBreaks[renderer] != null
						&& this.Bar.IsGroupBeginning( renderer.BarItem ) )
						left = this.tdbounds.Left;
					else if( this.Bar.IsGroupBeginning( renderer.BarItem ) )
						left = renderer.Bounds.Left;

					if( left != -1 )
					{
						bool itemVisible = renderer.Visible && this.ShouldDrawVisible( renderer.BarItem, false );
						// if the item is invisible, draw the separator only if the item is from the main manager
						if( itemVisible ||
							renderer.BarItem.Manager == null || renderer.BarItem.Manager is MainFrameBarManager )
						{
							// Line break at this renderer, draw a line above this renderer
                            using (Pen pen = new Pen(SystemColors.ControlDark))
                            {
                                g.DrawLine(pen, left, renderer.Bounds.Top - 3,
                                    this.tdbounds.Right - 1, renderer.Bounds.Top - 3);
                            }
						}
					}
				}
				g.ResetTransform();
			}
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ThemedToolBarDrawing: ThemedControlDrawing
	{
		private BarControlInternal barControl;
		public ThemedToolBarDrawing( BarControlInternal control, string classNames )
			: base( classNames )
		{
			this.barControl = control;
		}

		public void DrawButton( Graphics g, int part, int state, Rectangle rect )
		{
			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	/// <summary>
	/// Internal class used to renderer a toolbar or a <see cref="XPToolBar"/>.
	/// </summary>
	[ToolboxItem( false ),
	]
	public class BarControlInternal: Control, IBarControl, IBarItemContainerControl, ICallWndProcListener
	{
		#region PRIVATE_MEMBERS
		protected internal BarRenderer barRenderer;
		private Bar bar;
		private IBarHost barHost;
		private Form hostedForm;
		private MdiSysMenuProvider smProvider = null;
		protected internal bool forceMultiline = false;
		private ThemedToolBarDrawing themedDrawing = null;
		private bool themesEnabled = false;
		private Font font = null;
		private VisualStyle style = VisualStyle.OfficeXP;
		private ToolBarAccessibleObject tbao = null;
		private static Font menuFont = null;
		private IPopupParent popupParent = null;
		internal BarControlInternalWeakContainer barControlInternalWeakContainer = null;

		#endregion PRIVATE_MEMBERS
		#region INIT
		static BarControlInternal()
		{
			// Saving the menufont insted of querying it every time (for better perf)
			menuFont = System.Windows.Forms.SystemInformation.MenuFont;
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler( System_UserPreferenceChanged );
		}
		static void System_UserPreferenceChanged( object sender, UserPreferenceChangedEventArgs e )
		{
			// The menu's size might have changed.
			// Would be better to 
			menuFont = System.Windows.Forms.SystemInformation.MenuFont;
		}
		/// <summary>
		/// Creates a new instance of the BarControlInternal class.
		/// </summary>
		public BarControlInternal()
		{
			// With both these styles turned on painting misbehaves when you 
			// fill the Control with its BackColor, it ends up drawing with
			// a different BackColor (especially in lower color modes).
			// Hence the check in BarItemRenderer.DrawBackground
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer
				| ControlStyles.SupportsTransparentBackColor, true );

			this.AllowDrop = true;
			this.TabStop = false;

			if( XPThemes.IsThemedOS )
				this.themedDrawing = new ThemedToolBarDrawing( this, ThemedControls.TOOLBAR );

			barControlInternalWeakContainer = new BarControlInternalWeakContainer( this );
			MenuColors.MenuColorsChanged += new EventHandler( this.barControlInternalWeakContainer.MenuColorsChangedWeakEventHandler );
			Office2003Colors.MenuColorsChanged += new EventHandler( this.barControlInternalWeakContainer.Office2003ColorsChangedWeakEventHandler );
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler( System_UPChanged );
		}
		internal void System_UPChanged( object sender, UserPreferenceChangedEventArgs e )
		{
			// In case the menu-font was changed:
			if( this.barRenderer != null )
				this.barRenderer.InvalidateCachedTextSizes();
			this.OnBarBoundsAffected();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler( System_UserPreferenceChanged );
				MenuColors.MenuColorsChanged -= new EventHandler( this.MenuColorsChanged );
				Office2003Colors.MenuColorsChanged -= new EventHandler( this.MenuColorsChanged );
				SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler( System_UPChanged );
                MenuColors.MenuColorsChanged -= new EventHandler( this.barControlInternalWeakContainer.MenuColorsChangedWeakEventHandler );
                Office2003Colors.MenuColorsChanged -= new EventHandler( this.barControlInternalWeakContainer.Office2003ColorsChangedWeakEventHandler );
				if( this.themedDrawing != null )
				{
					themedDrawing.Dispose();
					themedDrawing = null;
				}
				if( this.barRenderer != null )
				{
					this.barRenderer.Dispose();
					this.barRenderer = null;
				}
				if( this.smProvider != null )
				{
					this.smProvider.NeedMenuButtonsChanged -= new EventHandler( this.SysMenuProvider_NeedMenuButtonsChanged );
					this.smProvider = null;
				}
				if( this.hostedForm != null )
				{
					if( this.hostedForm != null )
						this.hostedForm.Deactivate -= new EventHandler( this.FormDeactivated );
					this.hostedForm = null;
                }
                barControlInternalWeakContainer = null;
				this.barHost = null;

				if( tbao != null )
				{
					this.tbao.SetBarItemContainer( null );
					this.tbao = null;
				}
			}
			base.Dispose( disposing );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if( this.Bar != null )
			{
				this.tbao = new ToolBarAccessibleObject( this.Bar, this );
				return this.tbao;
			}
			else
				return null;
		}
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		EditorBrowsable( EditorBrowsableState.Never ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public IBarHost BarHost
		{
			get { return this.barHost; }
			set { this.barHost = value; }
		}

		/// <summary>
		/// Returns the <see cref="IBarRenderer"/> associated with this control.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IBarRenderer Renderer
		{
			get { return this.barRenderer; }
		}
		/// <summary>
		/// Specifies the visual style of this Control.
		/// </summary>
		/// <value>A <see cref="VisualStyle"/> value. Default is VisualStyle.OfficeXP.</value>
		/// <remarks>Note that this setting will be ignored when <see cref="ThemesEnabled"/> is turned on and themes are available in the OS.</remarks>
		[DefaultValue( VisualStyle.OfficeXP ),
		Category( "Appearance" ),
		Description( "Specifies the visual style of this Control." )
		]
		public VisualStyle Style
		{
			get { return this.style; }
			set
			{
				if( this.style != value )
				{
					this.style = value;
					this.InitBackColor();
					//this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Style", old, this.style));
					if( this.barRenderer != null )
						this.barRenderer.Style = this.Style;
					if( this.smProvider != null )
						this.smProvider.Style = this.Style;
                    if (this.style == VisualStyle.Metro)
                        this.BackColor = Color.White;
					UpdateColorScheme();
				}
			}
		}
		/// <summary>
		///MetroColor
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		///gets or sets the metrocolor
		/// </summary>
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                if (metroColor != value)
                    metroColor = value;
            }
        }
		private bool IsMainMenu
		{
			get
			{
				if( ( this.Bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
					return true;
				else
					return false;
			}
		}
		[Documentation.DocumentationExclude()]
		protected bool UseThemes
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
					&& this.ThemesEnabled )
					return true;
				else
					return false;
			}
		}
		private void InitBackColor()
		{
			if( ( this.UseThemes
                && style != VisualStyle.Office2007 && style != VisualStyle.Office2010 && this.Style != VisualStyle.Metro 
				&& style != VisualStyle.Office2007Outlook )
				|| style == VisualStyle.OfficeXP
				|| style == VisualStyle.Default )
			{
				this.InitThemedOrXPBackColor();
			}
			else
			{
				this.Init2003BackColor();
			}
		}
		[Documentation.DocumentationExclude()]
		protected virtual void Init2003BackColor()
		{
			this.SetStyle( ControlStyles.SupportsTransparentBackColor, true );
			if( this.HostedForm != null && this.HostedForm is CommandBarForm )
			{
				this.BackColor = this.HostedForm.BackColor;
			}
			else
			{
				this.BackColor = Color.Transparent;
			}
		}
		[Documentation.DocumentationExclude()]
		protected virtual void InitThemedOrXPBackColor()
		{
			this.SetStyle( ControlStyles.SupportsTransparentBackColor, false );
			this.ResetBackColor();
		}
		/// </override>
		public override Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				base.BackColor = value;
			}
		}

		/// <internalonly/>
		[Documentation.DocumentationExclude()]
		internal protected bool IsRTL
		{
			get
			{
				return ( this as IBarControl ).IsRightToLeft;
			}
		}

		[Documentation.DocumentationExclude()]
		public override void ResetBackColor()
		{
			base.ResetBackColor();
		}
		// Due to the setting in InitBackColor.
		// When Style is Office2003 we do not want to serialize the BackColor setting,
		// otherwise Color setting precedes the Style setting in code and that will cause a crash.
		protected bool ShouldSerializeBackColor()
		{
			if( this.style == VisualStyle.Office2003
				&& this.BackColor == Color.Transparent )
				return false;

			MethodInfo mInfo = typeof( Control ).GetMethod( "ShouldSerializeBackColor",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic );
			if( mInfo != null )
			{
				return (bool)mInfo.Invoke( this, new object[] { } );
			}
			return true;
		}
		/// <summary>
		/// Specifies whether XP Themes (visual styles) should be used for this control when
		/// available.
		/// </summary>
		/// <value>True to turn on themes; false otherwise.</value>
		[
		DefaultValue( false ),
		Category( @"Appearance" ),
		Description( "Specifies whether XP Themes (visual styles) should be used for this control when available." )
		]
		public bool ThemesEnabled
		{
			get { return this.themesEnabled; }
			set
			{
				if( this.themesEnabled != value )
				{
					this.themesEnabled = value;
					this.barRenderer.ThemesEnabled = value;
					this.OnThemeChanged( EventArgs.Empty );
					this.InitBackColor();
				}
			}
		}

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged( EventArgs e )
		{
			if( this.ThemeChanged != null )
			{
				this.ThemeChanged( this, e );
			}
		}
		/// <summary>
		/// This event will be fired when the ThemesEnabled property is changed.
		/// </summary>
        [Description("This event will be fired when the ThemesEnabled property is changed.")]
		public event EventHandler ThemeChanged;

		[Syncfusion.Documentation.DocumentationExclude(),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new bool DesignMode
		{
			get
			{
				return base.DesignMode || this.InDesignMode;
			}
		}

		private bool m_bInDesignMode = false;
		protected internal virtual bool InDesignMode
		{
			get
			{
				return m_bInDesignMode;
			}
			set
			{
				m_bInDesignMode = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void GetPreferredSize( ref SizeF preferredSize )
		{
			if( this.barRenderer == null )
			{
				preferredSize = new SizeF( 2, 20 );
				return;
			}
			//Graphics g = this.GetGraphics();
			GraphicsProvider gp = new GraphicsProvider( this );
			this.barRenderer.GetPreferredSize( gp, ref preferredSize );
			gp.Dispose();
			if( this.bar != null && ( this.bar.BarStyle & BarStyle.IsMainMenu ) > 0
				&& this.smProvider != null && this.smProvider.NeedMenuButtons )
			{
				preferredSize.Width += ( ( MdiSysMenuProvider.ButtonHeight * 3 ) + 4 );
				if (this.smProvider.ShowIcon)
					preferredSize.Width += 18;
				if( preferredSize.Height < 20 )
					preferredSize.Height = 20;
			}
		}
		// Returns true if renderer changed.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool UpdateRendererOnBarChange()
		{
			bool curStyleIsMultiline = this.barRenderer is MultilineBarRenderer;

			if(
				this.barRenderer == null
				||
				( ( ( this.bar.BarStyle & BarStyle.MultiLine ) > 0 ) && !curStyleIsMultiline )
				||
				( ( ( this.bar.BarStyle & BarStyle.MultiLine ) == 0 ) && curStyleIsMultiline )
				)
			{
				RendererChanged( null );
				return true;
			}
			else
				return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void RendererChanged( BarRenderer rendererNew )
		{
			if( this.barRenderer != null )
			{
				this.barRenderer.Detach();
				this.barRenderer.Dispose();
				this.barRenderer = null;
			}

			if( rendererNew != null )
			{
				this.barRenderer = rendererNew;
			}
			else
			{
				if( this.bar != null )
				{
					if( ( this.bar.BarStyle & BarStyle.MultiLine ) == 0 && !forceMultiline )
					{
						this.barRenderer = new SingleLineBarRenderer( this );
					}
					else
					{
						this.barRenderer = new MultilineBarRenderer( this );
					}
				}
			}

			if( this.bar != null )
				this.barRenderer.Bar = this.bar;

			if( this.barRenderer != null )
				this.barRenderer.Style = this.Style;

			this.OnBarBoundsAffected();
		}
		#endregion INIT

		#region PROPERTIES

		/// <summary>
		/// Allows you to set a PopupParent to this control. Useful when hosting this in a PopupControlContainer.
		/// </summary>
		[DefaultValue( null ), Browsable( false )]
		public virtual IPopupParent PopupParent
		{
			get { return this.popupParent; }
			set { this.popupParent = value; }
		}

		public override Font Font
		{
			get
			{
				if( this.font == null )
					//return System.Windows.Forms.SystemInformation.MenuFont;
					return BarControlInternal.menuFont;
				else
					return this.font;
			}
			set
			{
				if( this.font != value )
				{
					this.font = value;
					this.OnFontChanged( EventArgs.Empty );
					if( this.barRenderer != null )
						this.barRenderer.InvalidateCachedTextSizes();
					this.OnBarBoundsAffected();
				}
			}
		}

		/// <summary>
		/// Specifies whether font should be serialized.
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ShouldSerializeFont()
		{
			if( this.font == null )
				return false;
			else
				return true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override void ResetFont()
		{
			this.Font = null;
		}

		/// <summary>
		/// Serves to start keyboard based navigation.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void StartKeyboardBasedNavigation()
		{
			this.barRenderer.StartKeyboardNavigation();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool CanStartKeybardBasedNavigation()
		{
			if( ( this.Bar.BarStyle & BarStyle.IsStatusBar ) > 0
				|| !this.Visible
				|| !this.barRenderer.CanStartKeyboardNavigation() )
				return false;
			else
				return true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public bool HintViaHotKeyPrefix
		{
			set
			{
				if( this.barRenderer != null )
				{
					this.barRenderer.HintViaHotKeyPrefix = value;
				}
			}
		}

		/// <summary>
		/// Stops keyboard based navigation.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void StopKeyboardBasedNavigation()
		{
			this.barRenderer.StopKeyboardNavigation();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsKeyboardNavigationOn()
		{
			return this.barRenderer.IsKeyboardNavigationOn();
		}

		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		EditorBrowsable( EditorBrowsableState.Never ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public CommandBarDockState Alignment
		{
			get
			{
				return this.barRenderer.Alignment;
			}
			set
			{
				this.barRenderer.Alignment = value;
			}
		}
		ThemedToolBarDrawing IBarControl.ThemedDrawing
		{
			get { return this.themedDrawing; }
		}

		bool IBarControl.UseControlForeColor
		{
			get { return this.GetUseControlForeColor(); }
		}
		bool IBarControl.RequiresActiveFormForMouseTrack()
		{
			return this.RequiresActiveFormForMouseTrack();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool RequiresActiveFormForMouseTrack()
		{
			return true;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool GetUseControlForeColor()
		{
			return false;
		}

		/// <summary>
		/// Specifies whether the icons should be drawn large.
		/// </summary>
		/// <remarks>
		/// Internal property, not to be used directly.
		/// </remarks>
		[Browsable( false ),
		]
		public virtual bool LargeIcons
		{
			get
			{
				if( this.Bar != null && this.Bar.Manager != null )
					return this.Bar.Manager.LargeIcons;
				return false;
			}
			set { }
		}

		/// <summary>
		/// Gets a value indicating whether control's elements should be drawn for RightToLeft locales.
		/// </summary>
		/// <value>
		/// One of the <see cref="RightToLeft"/> values.
		/// </value>
		bool IBarControl.IsRightToLeft
		{
			get
			{
				bool bRTL = false;

				if( null != this.Bar && null != this.Bar.Manager )
				{
					bRTL = this.Bar.Manager.IsRTL;
				}
				else
				{
					bRTL = RightToLeft.Yes == this.RightToLeft;
				}

				return bRTL;
			}
		}

		[
		Syncfusion.Documentation.DocumentationExclude()
		]
        [Description ("Gets or Sets the Bar.")]
		public virtual Bar Bar
		{
			get { return this.bar; }
			set
			{
				if( this.bar != value )
				{
					if( this.bar != null )
						this.bar.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );

					this.bar = value;

					if( this.bar != null )
						this.bar.PropertyChanged += new SyncfusionPropertyChangedEventHandler( this.Bar_PropertyChanged );

					if( this.tbao != null )
						this.tbao.SetBarItemContainer( this.bar );

					if( this.bar == null )
						this.RendererChanged( null );
					// Update renderer only if necessary...
					else
					{
						if( !this.UpdateRendererOnBarChange() )
							// If renderer was not updated, then just update the Bar.
							this.barRenderer.Bar = this.bar;

						this.SynthesizeName();
					}

					if( this.bar != null )
					{
						BarManager barMgr = this.bar.Manager;

						if( null != barMgr && null != barMgr.MainFrameBarManager )
						{
							this.barRenderer.ActivateFormFromBar = barMgr.MainFrameBarManager.ActivateFormFromBar;
						}
					}
				}
			}
		}
		private void SynthesizeName()
		{
			if( this.Bar == null )
				return;
			string newName = "BarControl_" + this.Bar.BarName;
			newName = newName.Replace( ' ', '_' );

			this.Name = newName;
		}
		#endregion PROPERTIES
		#region PAINTING
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void Bar_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "BarStyle" )
			{
				this.UpdateRendererOnBarChange();
			}
			else if( e.PropertyName == "BarName" )
			{
				this.SynthesizeName();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public void OnBarBoundsAffected()
		{
			if( this.barHost != null )
				this.barHost.OnBarBoundsChanged();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool ShouldPreProcessTab()
		{
			return true;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool NeedKey( Keys key )
		{
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void OnRepaint( RectangleF affectedRect )
		{
			Rectangle ar = Rectangle.Ceiling( affectedRect );
			// Ceiling seems to make (Y = 4.5 and height = 22 as Y=5, and height = 22 for example), this 
			// code will make Y = 4 and height = 24. So that we will not fail to invalidate anything.
			if( ( (float)ar.Y ) > affectedRect.Y )
			{
				ar.Inflate( 0, 1 );
			}
			this.Invalidate( ar, true );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public Graphics GetGraphics()
		{
			return this.CreateGraphics();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public Control GetControl()
		{
			return this;
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnLayout"/>.
		/// </summary>
		/// <param name="levent"></param>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			// Sometimes when the control is being destroyed, layout gets called, hence the check for IsHandleCreated
			if( this.barRenderer != null && this.IsHandleCreated )
			{
				GraphicsProvider gp = new GraphicsProvider( this );
				this.Layout( gp );
				gp.Dispose();
			}
			base.OnLayout( levent );
		}
		protected override void OnHandleCreated( EventArgs e )
		{
			// Recalc on handle created, because renderer bounds are not set until handle is created.
			this.OnLayout( new LayoutEventArgs( this, "Bounds" ) );
			base.OnHandleCreated( e );
		}
		internal void MenuColorsChanged( object sender, EventArgs e )
		{
            // This helps if MenuColors were changed programmatically.
			this.Invalidate();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
			this.Refresh();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected new virtual void Layout( IGraphicsProvider gp )
		{
			Rectangle canvas = this.ClientRectangle;

			if( this.smProvider != null &&
				this.smProvider.NeedMenuButtons )
			{
				bool bRTL = this.IsRTL;
				// icon width and height:
				int controlIconDim = MdiSysMenuProvider.ButtonHeight;

				bool bIsVertical = ( this.Alignment == CommandBarDockState.Left ||
					this.Alignment == CommandBarDockState.Right );

				if( bIsVertical )
				{
					canvas = new Rectangle( canvas.Top, canvas.Left, canvas.Height, canvas.Width );
				}

				this.smProvider.IsVertical = bIsVertical;

				this.smProvider.SystemIconRect = new Rectangle( bRTL ? ( canvas.Right - 1 - controlIconDim ) :
					( canvas.Left + 1 ), canvas.Top + 3, controlIconDim, controlIconDim );
				canvas.X += 1;

				if( !bRTL && this.smProvider.ShowIcon)
				{
					canvas.X += controlIconDim;
				}

				canvas.Width -= ( controlIconDim + 1 );
				if( this.smProvider.WndActiveChild != null )
				{
					int prefWidth = 0;
					if( !this.smProvider.CloseButtonHidden )
					{
						prefWidth = this.smProvider.ShowIcon ? controlIconDim : 0;
						if( !this.smProvider.MinimizeButtonHidden )
							prefWidth = this.smProvider.ShowIcon ? (controlIconDim + 1) * 3 : (controlIconDim + 1) * 2;

						if( bRTL )
						{
							this.smProvider.ControlBoxRect = new Rectangle( canvas.Left + 1, canvas.Top + 3, prefWidth, controlIconDim );
						}
						else
						{
							this.smProvider.ControlBoxRect = new Rectangle( canvas.Right - prefWidth - 1, canvas.Top + 3, prefWidth, controlIconDim );
						}
					}

					if( bRTL )
					{
						canvas.X += prefWidth;
						canvas.Width += prefWidth;
						if (!this.smProvider.ShowIcon)
							canvas.X -= controlIconDim;
					}
					else
					{
						canvas.Width -= prefWidth;
					}
				}

				if( bIsVertical )
				{
					canvas = new Rectangle( canvas.Top, canvas.Left, canvas.Height, canvas.Width );
				}
			}

			this.barRenderer.Bounds = canvas;

			this.barRenderer.Layout( gp );

			this.Invalidate( false );
		}

		/// <summary>
		/// Updates color scheme.
		/// </summary>
		protected virtual void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();
		}


		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			UpdateColorScheme();

			if( this.barRenderer != null )
				this.barRenderer.OnPaint( e.Graphics, e.ClipRectangle );

			if( this.smProvider != null )
				this.smProvider.HandlePaint( e.Graphics );

			base.OnPaint( e );
		}
		public virtual bool ShouldDelegateBGDrawingToParentWhenThemed()
		{
			return true;
		}
		#endregion PAINTING
		#region FOCUS_TRANSFER_LOGIC
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnParentChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );
			this.HostedForm = this.FindForm();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Form HostedForm
		{
			get { return this.hostedForm; }
			set
			{
				if( this.hostedForm != value )
				{
					if( this.hostedForm != null )
						this.hostedForm.Deactivate -= new EventHandler( this.FormDeactivated );

					this.hostedForm = value;

					if( this.hostedForm != null )
						this.hostedForm.Deactivate += new EventHandler( this.FormDeactivated );
				}
			}
		}

		private void FormDeactivated( object sender, EventArgs e )
		{
			if( this.Focused )
			{
				this.barRenderer.OnFormDeactivated();
			}
		}

        public void CallWndProc(int ncode, IntPtr wparam, IntPtr lparam)
        {
            Message msg = (Message)(Marshal.PtrToStructure(lparam, typeof(Message)));
            if (msg.Msg == NativeMethods.WM_LBUTTONDOWN || msg.Msg == NativeMethods.WM_RBUTTONDOWN)
            {
                base.WndProc(ref msg);
            }
        }
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x21/*WM_MOUSEACTIVATE*/ && this.bar != null )
			{
				// FindForm will be null when the control is hosted in a native Form.
				if( this.FindForm() != null )
				{
					if( this.bar.Manager != null && this.bar.Manager.MainFrameBarManager != null )
					{
						// Do this only for floating forms.
						if( !this.bar.Manager.DesignMode
							&& this.bar.Manager.MainFrameBarManager.Form != this.FindForm()
							)
						{
							m.Result = (IntPtr)3;
							return;
						}
					}
					else
					{
						if( Form.ActiveForm != this.FindForm()
							&& ( Form.ActiveForm == null || Form.ActiveForm.ActiveMdiChild != this.FindForm() ) )
							m.Result = (IntPtr)3;
						return;
					}
				}
			}

			base.WndProc( ref m );
		}
		#endregion FOCUS_TRANSFER_LOGIC

		#region MOUSE

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.ProcessCmdKey"/>.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( this.barRenderer.ProcessCmdKey( ref msg, keyData ) )
				return true;
			else
				return base.ProcessCmdKey( ref msg, keyData );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnGotFocus"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus( EventArgs e )
		{
			//Trace.WriteLine("BarControlInternal got focus.");
			base.OnGotFocus( e );
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );
			// OnMouseUp could be called after this control gets disposed, if it is a right
			// mouse up that follows a context menu popup (in whose handler the control could have been
			// destroyed).
			if( this.IsHandleCreated )
			{
				this.barRenderer.OnMouseUp( e );
				if( this.smProvider != null )
					this.smProvider.HandleMouseUp( new Point( e.X, e.Y ) );
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );
			this.barRenderer.OnMouseMove( e );
			if( this.smProvider != null )
				this.smProvider.HandleMouseMove( new Point( e.X, e.Y ) );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
			if( this.barRenderer != null )
				this.barRenderer.OnMouseLeave( e );
			if( this.smProvider != null )
				this.smProvider.HandleMouseLeave();
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( this.barRenderer.OnMouseDown( e ) )
				return;

			this.XPMenuActive = true;
          	if( this.smProvider != null )
				this.smProvider.HandleMouseDown( new Point( e.X, e.Y ) );
			base.OnMouseDown( e );
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseWheel"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel( MouseEventArgs e )
		{
			base.OnMouseWheel( e );

			this.barRenderer.OnMouseWheel( e );
		}
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseClick"/>.
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnMouseClick(MouseEventArgs e)
        //{
        //    if (this.IsHandleCreated)
        //    {
        //        this.barRenderer.OnMouseClick(e);
        //    }
        //    base.OnMouseClick(e);
        //}
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDoubleClick"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                this.barRenderer.OnMouseDoubleClick(e);
                if (this.smProvider != null)
                    this.smProvider.HandleMouseDoubleClick(this.PointToClient(Control.MousePosition));
            }
            base.OnMouseDoubleClick(e);
        }		
		

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void ProcessDragOver( DragEventArgs drgevent )
		{
			this.barRenderer.OnDragOver( drgevent );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragOver"/>.
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			this.ProcessDragOver( drgevent );
			base.OnDragOver( drgevent );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void ProcessDragDrop( System.Windows.Forms.DragEventArgs e )
		{
			this.barRenderer.OnDragDrop( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragDrop"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragDrop( System.Windows.Forms.DragEventArgs e )
		{
			this.ProcessDragDrop( e );
			base.OnDragDrop( e );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void ProcessDragLeave( EventArgs e )
		{
			this.barRenderer.OnDragLeave( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragLeave"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragLeave( EventArgs e )
		{
			this.ProcessDragLeave( e );
			base.OnDragLeave( e );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ProcessGiveFeedback( GiveFeedbackEventArgs gfbevent )
		{
			this.barRenderer.OnGiveFeedback( gfbevent );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnGiveFeedback"/>.
		/// </summary>
		/// <param name="gfbevent"></param>
		protected override void OnGiveFeedback( GiveFeedbackEventArgs gfbevent )
		{
			this.ProcessGiveFeedback( gfbevent );
			base.OnGiveFeedback( gfbevent );
		}
		#endregion MOUSE

		[Syncfusion.Documentation.DocumentationExclude()]
		public void ActLikeMainMenu( Form mdiParent, bool designTime )
		{
			if( mdiParent != null )
			{
				if( mdiParent.IsMdiContainer && !designTime )
				{
					if( this.smProvider == null )
					{
						this.smProvider = MdiSysMenuManager.GetProviderForForm( mdiParent );
						this.smProvider.NeedMenuButtonsChanged +=
							new EventHandler( this.SysMenuProvider_NeedMenuButtonsChanged );

						this.smProvider.PerformingTransform += new TransformEventHandler( smProvider_PerformingTransform );
						this.smProvider.ActivateProvider( this );
						this.smProvider.Style = this.Style;
					}
				}
			}
			else
			{
				if( this.smProvider != null )
				{
					this.smProvider.NeedMenuButtonsChanged -=
						new EventHandler( this.SysMenuProvider_NeedMenuButtonsChanged );
					this.smProvider.PerformingTransform -= new TransformEventHandler(smProvider_PerformingTransform);
					this.smProvider.DeactivateProvider();
					this.smProvider = null;
				}
			}
		}

		AccessibleStates IBarItemContainerControl.GetAccessibilityState( BarItem item )
		{
			AccessibleStates states = AccessibleStates.None;
			if( item.Checked )
				states |= AccessibleStates.Checked;
			if( item.Enabled )
				states |= AccessibleStates.Focusable;
			else
				states |= AccessibleStates.Unavailable;
			if( this.barRenderer != null && this.barRenderer.CurrentHotTrackItem == item )
			{
				if( this.barRenderer.IsKeyboardNavigationOn() )
					states |= AccessibleStates.Focused;
				else
					states |= AccessibleStates.HotTracked;
			}
			return states;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public Rectangle GetBoundsOf( BarItem item )
		{
			if( this.barRenderer != null )
			{
				return this.RectangleToScreen( this.barRenderer.GetBoundsOf( item ) );
			}
			return Rectangle.Empty;
		}

		/// <summary>
		/// Indicates whether the specified coordinates are within BarItem.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Returns the BarItem which bounds contains point with specified coordinates.</returns>
		public BarItem HitTest( int x, int y )
		{
			if( this.barRenderer != null )
			{
				bool beforeOrAfter = false;
				return this.barRenderer.HitTestBarItem( this.PointToClient( new Point( x, y ) ), ref beforeOrAfter );
			}
			else
				return null;
		}
		private void SysMenuProvider_NeedMenuButtonsChanged( object sender, EventArgs e )
		{
			this.OnBarBoundsAffected();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ProcessShortcut( char c )
		{
			bool processed = false;

			if( this.Visible )
			{
				Form form = this.FindForm();
				Control control = Control.FromHandle( NativeMethods.GetFocus() );
				Form activeForm = ( control != null ) ? control.FindForm() :
					Form.ActiveForm;

				bool bShouldProcess = ( ( form == activeForm || ( activeForm != null &&
					( ( activeForm.IsMdiChild && activeForm.MdiParent == form ) ||
					( null != activeForm.Parent && form == activeForm.Parent.FindForm() ) ||
					( form != null && form.Parent == activeForm ) ) ) ) ) || ( form == activeForm.Owner );

				if( bShouldProcess )
				{
					processed = this.barRenderer.ProcessShortcut( c );
				}
			}

			return processed;
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.ProcessMnemonic"/>.
		/// </summary>
		/// <param name="charCode"></param>
		protected override bool ProcessMnemonic( char charCode )
		{
			return this.barRenderer.ProcessMnemonic( charCode );
		}

		internal bool XPMenuActive
		{
			get
			{
				return null != this.barRenderer ? this.barRenderer.XPMenuActive : false;
			}
			set
			{
				if( null != this.barRenderer )
				{
					this.barRenderer.XPMenuActive = value;
				}
			}
		}

		private const int DEF_SYS_MENU_VERTICAL_OFFSET = 3;

		private void smProvider_PerformingTransform( object sender, TransformEventArgs e )
		{
			if( barRenderer != null && ( this.Alignment == CommandBarDockState.Left ||
				this.Alignment == CommandBarDockState.Right ) )
			{
				e.TransformOffsetY = (int)this.barRenderer.Bounds.Width -
					DEF_SYS_MENU_VERTICAL_OFFSET;
			}
		}
	}
}
